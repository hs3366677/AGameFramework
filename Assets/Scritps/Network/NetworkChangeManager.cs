using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using Protocols;
using Debug = UnityEngine.Debug;

/// <summary>
/// 网络消息事件处理委托
/// 
/// </summary>
/// <param name="msgEntry">消息数据实体</param>
public delegate void NetworkEventHandler(Protocol msgEntry);

/// <summary>
/// 网络通信变化管理器
/// 
/// 维护一个Dictionary做消息和事件的映射，开放注册、注销、通知等操作
/// </summary>
public static class NetworkChangeManager
{
	// 
    private static Dictionary<ProtoID, List<WeakAction>> s_weakNetworkMsgMap = new Dictionary<ProtoID, List<WeakAction>>();

    private static Dictionary<ProtoID, List<NetworkEventHandler>> s_strongNetworkMsgMap = new Dictionary<ProtoID, List<NetworkEventHandler>>();

	/// <summary>
	/// 注册 消息Id和消息响应事件的回调
	/// </summary>
	/// <param name="subject">消息Id</param>
	/// <param name="observer">响应事件的委托方法</param>
	public static void RegisterWeak(ProtoID subject, NetworkEventHandler observer)
	{
		// 已经注册过的消息就在消息列表中追加处理事件
		if (s_weakNetworkMsgMap.ContainsKey(subject))
		{
            WeakAction weakAction = new WeakAction(observer);
            if (s_weakNetworkMsgMap[subject].Contains(weakAction)) return;
            s_weakNetworkMsgMap[subject].Add(weakAction);
		}
		// 注册新消息
		else
		{
            List<WeakAction> weakActions = new List<WeakAction>();
            weakActions.Add(new WeakAction(observer));
            s_weakNetworkMsgMap.Add(subject, weakActions);
		}
	}

    /// <summary>
    /// 注册 消息Id和消息响应事件的回调
    /// </summary>
    /// <param name="subject">消息Id</param>
    /// <param name="observer">响应事件的委托方法</param>
    public static void RegisterStrong(ProtoID subject, NetworkEventHandler observer)
    {
        // 已经注册过的消息就在消息列表中追加处理事件
        if (s_strongNetworkMsgMap.ContainsKey(subject))
        {
            if (s_strongNetworkMsgMap[subject].Contains(observer)) return;
            s_strongNetworkMsgMap[subject].Add(observer);
        }
        // 注册新消息
        else
        {
            List<NetworkEventHandler> actions = new List<NetworkEventHandler>();
            actions.Add(observer);
            s_strongNetworkMsgMap.Add(subject, actions);
        }
    }

	/// <summary>
	/// 注销 消息和事件的映射关系
	/// </summary>
	/// <param name="subject">消息Id</param>
	/// <param name="observer">响应事件的委托方法</param>
	public static void Unregister(ProtoID subject, NetworkEventHandler observer)
	{
		if (s_weakNetworkMsgMap.ContainsKey(subject))
        {
            WeakAction weakAction = new WeakAction(observer);

            if (s_weakNetworkMsgMap[subject].Contains(weakAction))
            {
                s_weakNetworkMsgMap[subject].Remove(weakAction);
            }
			if (s_weakNetworkMsgMap[subject] == null)
			{
				s_weakNetworkMsgMap.Remove(subject);
			}
		}
	}

	/// <summary>
	/// 通知响应回调方法，消息到达
	/// </summary>
	/// <param name="subject">消息Id</param>
	/// <param name="msg">传递给处理方法的参数（服务器传回来的数据）</param>
	public static void Notify(ProtoID subject, Protocol msg)
	{
        List<WeakAction> weakHandlers = null;
        List<NetworkEventHandler> strongHandlers = null;
        //if (subject.BodyType == typeof(LuaProtoBody) && LuaMessageCallback != null)
        //{
        //    LuaMessageCallback(msg);
        //    return;
        //}

        if (s_weakNetworkMsgMap.TryGetValue(subject, out weakHandlers))
		{

            for (int i = weakHandlers.Count - 1; i >= 0; i--)
            {
                if (!weakHandlers[i].Invoke(msg))
                    weakHandlers.RemoveAt(i);
            }
        }
        else if (s_strongNetworkMsgMap.TryGetValue(subject, out strongHandlers))
        {
            for (int i = strongHandlers.Count - 1; i >= 0; i--)
            {
                if (strongHandlers[i] != null)
                    strongHandlers[i](msg);
            }
        }
		else
		{
			Debug.LogWarning(string.Format("消息{0}未注册事件处理方法", subject.ID));
		}
	}
}

/// <summary>
/// 这里使用反射会慢一点，不过问题在消息处理上面问题不大
/// </summary>
public class WeakAction : IEquatable<WeakAction>
{
    public WeakAction(NetworkEventHandler action)
    {
        Method = action.Method;
        Reference = new WeakReference(action.Target);
    }

    public WeakAction(Action action)
    {
        Method = action.Method;
        Reference = new WeakReference(action.Target);
    }

    protected MethodInfo Method { get; private set; }
    protected WeakReference Reference { get; private set; }

    public bool IsAlive
    {
        get { return true; }// Reference.IsAlive; }
    }

    public object Target
    {
        get { return Reference.Target; }
    }

    public bool Invoke(Protocol msg)
    {
        if (Method != null && IsAlive)
        {
            Debug.Log("weak reference invoking..." + Method.Name);
            Method.Invoke(Target, new object[]{msg});
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool Invoke()
    {
        if (Method != null && IsAlive)
        {
            Method.Invoke(Target, null);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool Equals(WeakAction other)
    {
        if (other == null) return false;
        return Method.Equals(other.Method);
    }
}
