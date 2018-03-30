
CtrlNames = {
	Prompt = "PromptCtrl",
	Message = "MessageCtrl",
    TestProtocol = "TestProtocolCtrl"
}

PanelNames = {
	"PromptPanel",	
	"MessagePanel",
	"TestProtocolPanel"
}


--协议类型--
ProtocalType = {
	BINARY = 0,
	PB_LUA = 1,
	PBC = 2,
	SPROTO = 3,
}
--当前使用的协议类型--
TestProtoType = ProtocalType.BINARY;

Util = LuaFramework.Util;
AppConst = LuaFramework.AppConst;
LuaHelper = LuaFramework.LuaHelper;
ByteBuffer = LuaFramework.ByteBuffer;
THelper = TestHelper;

resMgr = LuaHelper.GetResManager();
panelMgr = LuaHelper.GetPanelManager();
soundMgr = LuaHelper.GetSoundManager();
networkMgr = LuaHelper.GetNetManager();
tNetworkCtrl = TestHelper.GetNetworkController();

Protocol = Protocols.Protocol;
WWW = UnityEngine.WWW;
GameObject = UnityEngine.GameObject;
Resources = UnityEngine.Resources;