using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Protocols;
using LuaFramework;
using UnityEngine.UI;
//class UnityLogger : IProtoLogger
//{

//    public void Log(ProtoLoggerLevel level, string msg, params object[] args)
//    {
//        switch (level)
//        {
//            case ProtoLoggerLevel.Debug:
//            case ProtoLoggerLevel.Info:
//                Debug.LogFormat(msg, args);
//                break;
//            case ProtoLoggerLevel.Error:
//            case ProtoLoggerLevel.Fatal:
//                Debug.LogErrorFormat(msg, args);
//                break;
//            case ProtoLoggerLevel.Warn:
//                Debug.LogWarningFormat(msg, args);
//                break;
//        }
//    }

//    public void LogModule(ProtoLoggerLevel level, string moduleName, string msg, params object[] args)
//    {
//        Log(level, msg, args);
//    }

//    public void LogException(System.Exception e)
//    {
//        Debug.LogException(e);
//    }
//}

public class TestMono : MonoBehaviour {

    Protocol proto;

    int serverSessionId = -1;

    string mIpAddr;

    [SerializeField]
    private InputField field;

    [SerializeField]
    private InputField resField;

    void Start()
    {
        field.text = Network.player.ipAddress;
        resField.text = "http://172.16.12.200/LuaAssetbundle";
    }
    //void Update()
    //{
    //    if (NetworkController.Instance != null && serverSessionId > 0)
    //    {
    //        // 在网络Session池中取消息
    //        while ((proto = NetworkController.Instance.GetMessage(serverSessionId)) != null)
    //        {

    //            if (proto.GetID().BodyType == typeof(LuaProtoBody))
    //            {
    //                ByteBuffer buffer = new ByteBuffer(((LuaProtoBody)proto.GetBody()).byteArray);
    //                Util.CallMethod("Network", "OnSocket", (int)proto.GetID(), buffer);
    //                Debug.Log(">>>>>>>>>>>>>>>>>>>" + ((LuaProtoBody)proto.GetBody()).byteArray.ToString());
    //            }
    //            else
    //            {

    //                UnityEngine.Debug.Log(serverSessionId + "Received Proto Id " + proto.GetID());
    //                TestProtoBody testBody = proto.GetBody() as TestProtoBody;
    //                if (testBody != null)
    //                    UnityEngine.Debug.Log(serverSessionId + "Received Proto Body " + testBody.id + " " + testBody.name);

    //            }
                
    //        }
    //    }
    //}

    // 将消息序列化为二进制的方法  
    // < param name="model">要序列化的对象< /param>  
    private byte[] Serialize(TestProtoBody model)
    {
        try
        {
            //涉及格式转换，需要用到流，将二进制序列化到流中  
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                //使用ProtoBuf工具的序列化方法  
                ProtoBuf.Serializer.Serialize<TestProtoBody>(ms, model);
                //定义二级制数组，保存序列化后的结果  
                byte[] result = ms.ToArray();
                //将流的位置设为0，起始点  
                ms.Position = 0;
                ms.Write(BitConverter.GetBytes((Int32)result.Length), 0, sizeof(UInt32));
                //将流中的内容读取到二进制数组中  
                ms.Write(result, 0, result.Length);

                result = ms.ToArray();
                return result;
            }
        }
        catch (Exception ex)
        {
            Debug.Log("序列化失败: " + ex.ToString());
            return null;
        }
    }  

    public void OnClickStartServer()
    {
        mIpAddr = field.text;
        AppFacade.Instance.GetManager<NetworkManager>("NetworkManager").StartTcpServer(mIpAddr, 20002);

        AppFacade.Instance.GetManager<NetworkManager>("NetworkManager").RegisterCallBack(405, OnOpenLuaCallback);
        AppFacade.Instance.GetManager<NetworkManager>("NetworkManager").RegisterCallBack(406, OnLuaMsgCallback);

    }

    /// <summary>
    /// 收到打开Lua界面的消息
    /// </summary>
    /// <param name="proto"></param>
    void OnOpenLuaCallback(Protocol proto)
    {
        Debug.Log("收到打开Lua界面的消息，正在打开。。。");

        AppFacade.Instance.GetManager<ResourceManager>("ResourceManager").Initialize();
        AppFacade.Instance.GetManager<LuaManager>("LuaManager").InitStart();
        AppFacade.Instance.GetManager<LuaManager>("LuaManager").DoFile("Logic/Game");         //加载游戏
        //AppFacade.Instance.GetManager<LuaManager>("LuaManager").DoFile("Logic/Network");      //加载网络
        //NetManager.OnInit();                     //初始化网络
        Util.CallMethod("Game", "OnInitOK");     //初始化完成
    }

    void OnLuaMsgCallback(Protocol proto)
    {
        Debug.Log("收到从Lua发来的消息，转发给Lua");
        Protocol newProto = new Protocol(407, proto.GetBody());
        AppFacade.Instance.GetManager<NetworkManager>("NetworkManager").SendTcpServerMessage(407, ((LuaProtoBody)proto.GetBody()).byteArray);
    }
}
