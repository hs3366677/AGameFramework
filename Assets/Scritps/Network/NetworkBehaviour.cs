using UnityEngine;
using System.Collections.Generic;
using Protocols;

/// <summary>
/// 对MonoBehaviour的拓展
/// 有网络消息传送的脚本可以继承自该类，同时具备MonoBehaviour的生命周期和网络消息的响应能力。
/// </summary>
public class NetworkBehaviour : MonoBehaviour 
{
	// 当前脚本记录的事件响应列表
	private List<KeyValuePair<ProtoID, NetworkEventHandler>> m_HandlerPairs = new List<KeyValuePair<ProtoID,NetworkEventHandler>>();

	/// <summary>
	/// 添加脚本对象要处理的网络协议Id及其响应方法
	/// </summary>
	/// <param name="id">协议Id</param>
	/// <param name="handler">进行响应的方法</param>
	protected void AddNetworkHandler(ProtoID id, NetworkEventHandler handler)
	{
		m_HandlerPairs.Add(new KeyValuePair<ProtoID, NetworkEventHandler> ( id, handler ));
		// 如果脚本处于启用状态，要实时向网络管理器注册事件响应
		if(this.enabled)
		{
			NetworkChangeManager.RegisterWeak(id, handler);
		}
	}

	/// <summary>
	/// 将脚本绑定的响应事件注册到网络管理器
	/// </summary>
	private void EnableHandlers()
	{
		foreach (KeyValuePair<ProtoID, NetworkEventHandler> pair in m_HandlerPairs)
		{
			NetworkChangeManager.RegisterWeak(pair.Key, pair.Value);
		}
	}

	/// <summary>
	/// 从网络管理器中移除当前脚本绑定的响应事件
	/// </summary>
	private void DisableHandlers()
	{
		foreach (KeyValuePair<ProtoID, NetworkEventHandler> pair in m_HandlerPairs)
		{
			NetworkChangeManager.Unregister(pair.Key, pair.Value);
		}
	}

	/// <summary>
	/// 清理脚本绑定的响应事件
	/// 先网络管理器中移除事件，再清空记录的事件列表。
	/// </summary>
	private void CleanHandlers()
	{
		DisableHandlers();
		m_HandlerPairs.Clear();
	}

	/// <summary>
	/// 脚本启用时进行事件的响应
	/// </summary>
	void OnEnable()
	{
        //var localScale = transform.localScale;
        //float widthRate = Screen.width / 1680.0f;
        //transform.localScale = localScale * widthRate;

		EnableHandlers();
		OnEnableEx();
	}

	/// <summary>
	/// 脚本禁用时不再进行事件的响应
	/// </summary>
	void OnDisable()
	{
		DisableHandlers();
		OnDisableEx();
	}

	void OnDestroy()
	{
		CleanHandlers();
		OnDestroyEx();
	}

	/// <summary>
	/// 对MonoBehaviour.OnEnable的拓展
	/// 子类中可以重写该方法实现OnEnable的效果
	/// </summary>
	public virtual void OnEnableEx()
	{
	}

	/// <summary>
	/// 对MonoBehaviour.OnDisable的拓展
	/// 子类中可以重写该方法实现OnDisable的效果
	/// </summary>
	public virtual void OnDisableEx()
	{
	}

	/// <summary>
	/// 对MonoBehaviour.OnDestroy的拓展
	/// 子类中可以重写该方法实现OnDestroy的效果
	/// </summary>
	public virtual void OnDestroyEx()
	{
	}
}
