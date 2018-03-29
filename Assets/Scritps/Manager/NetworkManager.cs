using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Protocols;
using Debug = UnityEngine.Debug;
using LuaInterface;
using LuaFramework;

namespace LuaFramework{

public class NetworkManager : Manager
{
    NetworkController networkCtrl;
    Protocol m_proto;
    static readonly object m_lockObject = new object();
    static Queue<KeyValuePair<int, ByteBuffer>> mEvents = new Queue<KeyValuePair<int, ByteBuffer>>();

    int mTcpServerSessionId = -1;

    int mTcpClientSessionId = -1;


    /// <summary>
    /// Udp客户端连接的sessionId，暂时用不到
    /// </summary>
    int mUdpClientSessionId;

    void Awake()
    {
        networkCtrl = NetworkController.Instance;
        networkCtrl.AssignLogger(new UnityLogger());

        // 超时回调检测
        StartCoroutine(TimeOutPolling());
    }

    /// <summary>
    /// 交给Command，这里不想关心发给谁。
    /// </summary>
    void Update()
    {
        // 在网络Session池中取消息
        if (mTcpServerSessionId > 0)
        while ((m_proto = networkCtrl.GetMessage(mTcpServerSessionId)) != null)
        {
            try
            {
                Debug.Log("server network change manager recv " + m_proto.GetID().ToUInt16());
                // 处理消息事件
                NetworkChangeManager.Notify(m_proto.GetID(), m_proto);
                // 移除超时回调
                RemoveTimeOutHandlers(m_proto.GetID());
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
            }
        }

        if (mTcpClientSessionId > 0)
        // 在网络Session池中取消息
        while ((m_proto = networkCtrl.GetMessage(mTcpClientSessionId)) != null)
        {
            try
            {
                Debug.Log("client network change manager recv " + m_proto.GetID().ToUInt16());
                // 处理消息事件
                NetworkChangeManager.Notify(m_proto.GetID(), m_proto);
                // 移除超时回调
                RemoveTimeOutHandlers(m_proto.GetID());
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
            }
        }
    }

    void OnDestroy()
    {
        networkCtrl.Disconnect(mTcpServerSessionId);
        networkCtrl.Disconnect(mTcpClientSessionId);
    }

    public void ConnectTcp(string ip, int port)
    {
        mTcpClientSessionId = networkCtrl.InitSession(ip, port, System.Net.Sockets.ProtocolType.Tcp, true);
    }

    public void StartTcpServer(string ip, int port)
    {
        mTcpServerSessionId = networkCtrl.InitSession(ip, port, System.Net.Sockets.ProtocolType.Tcp, false);
    }

    public void SendTcpClientMessage(ProtoBody protoBody)
    {
        networkCtrl.SendMessage(mTcpClientSessionId, protoBody);
    }

    public void SendTcpClientMessage(ProtoID rspProtoId, ProtoBody protoBody)
    {
        Protocol proto = new Protocol(rspProtoId, protoBody);
        networkCtrl.SendMessage(mTcpClientSessionId, proto);
    }
    public void SendTcpClientMessage(UInt16 protoid, byte[] buffer)
    {
        networkCtrl.SendMessage(mTcpClientSessionId, protoid, buffer);
    }

    public void SendTcpClientMessage(UInt16 protoid, ByteBuffer buffer)
    {
        networkCtrl.SendMessage(mTcpClientSessionId, protoid, buffer.ToBytes());
    }

    /// <summary>
    /// 测试用的发送协议
    /// </summary>
    /// <param name="body">要发送的网络协议</param>
    /// <param name="rspProtoId">期望收到的响应Id</param>
    /// <param name="timeOutHandler">协议响应超时的处理处理方法</param>
    public void SendTcpClientMessage(ProtoID rspProtoId, ProtoBody body, Action timeOutHandler, bool isShowWaiting = false)
    {
        //protoTimes[rspProtoId] = Time.realtimeSinceStartup;
        //timeOutHandlers[rspProtoId] = timeOutHandler;
        TimeOutData data = new TimeOutData(rspProtoId, timeOutHandler);

        int dataIndex = timeOutHandlers.IndexOf(data);
        if (dataIndex > 0)
        {
            timeOutHandlers[dataIndex].StartTime = Time.realtimeSinceStartup;
        }
        else
        {
            timeOutHandlers.Add(data);
        }

        SendTcpClientMessage(body);

    }

    public void SendTcpServerMessage(ProtoBody protoBody)
    {
        networkCtrl.SendMessage(mTcpServerSessionId, protoBody);
    }

    public void SendTcpServerMessage(UInt16 protoid, byte[] buffer)
    {
        networkCtrl.SendMessage(mTcpServerSessionId, protoid, buffer);
    }

    public void RegisterCallBack(ProtoID _protoId, NetworkEventHandler _callback)
    {
        NetworkChangeManager.RegisterWeak(_protoId, _callback);
    }

    public void RegisterShortCallBack(UInt16 _protoId, NetworkEventHandler _callback)
    {

        NetworkChangeManager.RegisterStrong((ProtoID)_protoId, _callback);
    }

    /// <summary>
    /// Lua 网络初始化
    /// </summary>
    public void OnInit()
    {
        CallMethod("Start");
    }

    /// <summary>
    /// Lua 网络卸载
    /// </summary>
    public void Unload()
    {
        CallMethod("Unload");
    }

    /// <summary>
    /// 执行Lua方法
    /// </summary>
    public object[] CallMethod(string func, params object[] args)
    {
        return Util.CallMethod("Network", func, args);
    }

    ///------------------------------------------------------------------------------------
    public static void AddEvent(int _event, ByteBuffer data)
    {
        lock (m_lockObject)
        {
            mEvents.Enqueue(new KeyValuePair<int, ByteBuffer>(_event, data));
        }
    }

    #region 协议响应超时处理部分

    private List<TimeOutData> timeOutHandlers = new List<TimeOutData>();

    /// <summary>
    /// 超时的轮询周期
    /// </summary>
    private readonly WaitForSeconds TimeOutPollingPeriod = new WaitForSeconds(1);
    void RemoveTimeOutHandlers(ProtoID _protoId)
    {
        List<TimeOutData> timeOutInvalidList = new List<TimeOutData>();

        foreach (var x in timeOutHandlers)
        {
            if (x.rspProtoId == _protoId)
            {
                timeOutInvalidList.Add(x);
            }
        }

        foreach (var x in timeOutInvalidList)
        {
            timeOutHandlers.Remove(x);
        }

        timeOutInvalidList.Clear();
    }

    /// <summary>
    /// 轮询协议超时
    /// </summary>
    private IEnumerator TimeOutPolling()
    {
        List<TimeOutData> timeOutExecuteList = new List<TimeOutData>();
        while (true)
        {
            yield return TimeOutPollingPeriod;

            foreach (var x in timeOutHandlers)
            {
                if (Time.realtimeSinceStartup - x.StartTime >= x.PROTO_TIME_OUT)
                {
                    timeOutExecuteList.Add(x);
                }
            }

            int len = timeOutExecuteList.Count;
            for (int i = 0; i < len; ++i)
            {
                timeOutExecuteList[i].weakTimeOutAction.Invoke();
                timeOutHandlers.Remove(timeOutExecuteList[i]);
            }
            timeOutExecuteList.Clear();
        }
    }

    #endregion
}


class TimeOutData : IEquatable<TimeOutData>
{

    /// <summary>
    /// 一般协议的超时时间（s）
    /// </summary>
    public float PROTO_TIME_OUT = 20;

    public float StartTime
    {
        get;
        set;
    }
    public ProtoID rspProtoId;
    
    public WeakAction weakTimeOutAction;

    public TimeOutData(ProtoID _rspProtoId, Action timeoutHandler)
    {
        StartTime = Time.realtimeSinceStartup;
        rspProtoId = _rspProtoId;
        weakTimeOutAction = new WeakAction(timeoutHandler);
    }

        
    public bool Equals(TimeOutData other)
    {
        if (other == null) return false;
        if (weakTimeOutAction != null && other.weakTimeOutAction == null) return false;
        return weakTimeOutAction.Equals(other.weakTimeOutAction) && rspProtoId == other.rspProtoId;
    }
}

class UnityLogger : IProtoLogger
{

    public void Log(ProtoLoggerLevel level, string msg, params object[] args)
    {
        switch (level)
        {
            case ProtoLoggerLevel.Debug:
            case ProtoLoggerLevel.Info:
                Debug.LogFormat(msg, args);
                break;
            case ProtoLoggerLevel.Error:
            case ProtoLoggerLevel.Fatal:
                Debug.LogErrorFormat(msg, args);
                break;
            case ProtoLoggerLevel.Warn:
                Debug.LogWarningFormat(msg, args);
                break;
        }
    }

    public void LogModule(ProtoLoggerLevel level, string moduleName, string msg, params object[] args)
    {
        Log(level, msg, args);
    }

    public void LogException(System.Exception e)
    {
        Debug.LogException(e);
    }
}


}