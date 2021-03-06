﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class Protocols_NetworkControllerWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(Protocols.NetworkController), typeof(System.Object));
		L.RegFunction("AssignLogger", AssignLogger);
		L.RegFunction("GetMessage", GetMessage);
		L.RegFunction("InitSession", InitSession);
		L.RegFunction("Disconnect", Disconnect);
		L.RegFunction("SendMessage", SendMessage);
		L.RegFunction("LuaSendMessage", LuaSendMessage);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("Instance", get_Instance, null);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AssignLogger(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Protocols.NetworkController obj = (Protocols.NetworkController)ToLua.CheckObject<Protocols.NetworkController>(L, 1);
			Protocols.IProtoLogger arg0 = (Protocols.IProtoLogger)ToLua.CheckObject<Protocols.IProtoLogger>(L, 2);
			obj.AssignLogger(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetMessage(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Protocols.NetworkController obj = (Protocols.NetworkController)ToLua.CheckObject<Protocols.NetworkController>(L, 1);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			Protocols.Protocol o = obj.GetMessage(arg0);
			ToLua.PushObject(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InitSession(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 3)
			{
				Protocols.NetworkController obj = (Protocols.NetworkController)ToLua.CheckObject<Protocols.NetworkController>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);
				int o = obj.InitSession(arg0, arg1);
				LuaDLL.lua_pushinteger(L, o);
				return 1;
			}
			else if (count == 4)
			{
				Protocols.NetworkController obj = (Protocols.NetworkController)ToLua.CheckObject<Protocols.NetworkController>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);
				System.Net.Sockets.ProtocolType arg2 = (System.Net.Sockets.ProtocolType)ToLua.CheckObject(L, 4, typeof(System.Net.Sockets.ProtocolType));
				int o = obj.InitSession(arg0, arg1, arg2);
				LuaDLL.lua_pushinteger(L, o);
				return 1;
			}
			else if (count == 5)
			{
				Protocols.NetworkController obj = (Protocols.NetworkController)ToLua.CheckObject<Protocols.NetworkController>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);
				System.Net.Sockets.ProtocolType arg2 = (System.Net.Sockets.ProtocolType)ToLua.CheckObject(L, 4, typeof(System.Net.Sockets.ProtocolType));
				bool arg3 = LuaDLL.luaL_checkboolean(L, 5);
				int o = obj.InitSession(arg0, arg1, arg2, arg3);
				LuaDLL.lua_pushinteger(L, o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: Protocols.NetworkController.InitSession");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Disconnect(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Protocols.NetworkController obj = (Protocols.NetworkController)ToLua.CheckObject<Protocols.NetworkController>(L, 1);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			obj.Disconnect(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SendMessage(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 3 && TypeChecker.CheckTypes<Protocols.Protocol>(L, 3))
			{
				Protocols.NetworkController obj = (Protocols.NetworkController)ToLua.CheckObject<Protocols.NetworkController>(L, 1);
				int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
				Protocols.Protocol arg1 = (Protocols.Protocol)ToLua.ToObject(L, 3);
				obj.SendMessage(arg0, arg1);
				return 0;
			}
			else if (count == 3 && TypeChecker.CheckTypes<Protocols.ProtoBody>(L, 3))
			{
				Protocols.NetworkController obj = (Protocols.NetworkController)ToLua.CheckObject<Protocols.NetworkController>(L, 1);
				int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
				Protocols.ProtoBody arg1 = (Protocols.ProtoBody)ToLua.ToObject(L, 3);
				obj.SendMessage(arg0, arg1);
				return 0;
			}
			else if (count == 3 && TypeChecker.CheckTypes<System.IO.Stream>(L, 3))
			{
				Protocols.NetworkController obj = (Protocols.NetworkController)ToLua.CheckObject<Protocols.NetworkController>(L, 1);
				int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
				System.IO.Stream arg1 = (System.IO.Stream)ToLua.ToObject(L, 3);
				obj.SendMessage(arg0, arg1);
				return 0;
			}
			else if (count == 4)
			{
				Protocols.NetworkController obj = (Protocols.NetworkController)ToLua.CheckObject<Protocols.NetworkController>(L, 1);
				int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
				ushort arg1 = (ushort)LuaDLL.luaL_checknumber(L, 3);
				byte[] arg2 = ToLua.CheckByteBuffer(L, 4);
				obj.SendMessage(arg0, arg1, arg2);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: Protocols.NetworkController.SendMessage");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LuaSendMessage(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 4);
			Protocols.NetworkController obj = (Protocols.NetworkController)ToLua.CheckObject<Protocols.NetworkController>(L, 1);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			ushort arg1 = (ushort)LuaDLL.luaL_checknumber(L, 3);
			LuaFramework.ByteBuffer arg2 = (LuaFramework.ByteBuffer)ToLua.CheckObject<LuaFramework.ByteBuffer>(L, 4);
			obj.LuaSendMessage(arg0, arg1, arg2);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Instance(IntPtr L)
	{
		try
		{
			ToLua.PushObject(L, Protocols.NetworkController.Instance);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

