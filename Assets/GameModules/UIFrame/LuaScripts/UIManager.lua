require "UI/UI_Main"
require "UI/UI_Character"
require "UI/UI_Items"
require "UI/UI_Shop"

UIManager = {};

g_UIManager = UIManager;

--做任何新UI之前一定要先注册对应类名
local UITypes = {};

--类注册，做新UI必须要先注册，并且保证Prefab名字与你注册的名字一致
function UIManager.RegUIName()
	UITypes["UI_Main"] = UI_Main;
	UITypes["UI_Character"] = UI_Character;
	UITypes["UI_Items"] = UI_Items;
	UITypes["UI_Shop"] = UI_Shop;
end

function UIManager.Init(parent)
	g_UIManager.m_uiRoot = GameObject("UIRoot");
	g_UIManager.m_uiRoot.transform:SetParent(parent.transform);
	LuaFramework.Util.SetLocalPosition(g_UIManager.m_uiRoot.transform,Vector3.zero);
	UIManager.RegUIName();
	UIManager.ClearQueue();
	-- 这里设定是有一个基础UI，所以这个UI不再队列中
	g_UIManager.ShowUI("UI_Main");
	g_UIManager.m_currentUI = nil;
end

--显示UI，先销毁现有的UI，再创建新的，有参数的话会调用Init方法
--jumpData代表下一个打开的UI你传递的默认参数
function UIManager.ShowUI(uiName,jumpData,destorySelf)
	--如果有数据进来代表本次为跳转，需要记录当前的UI名字
	UIManager.SaveCurrentUI();
	UIManager.ChangeUI(uiName,jumpData,destorySelf);
end

function UIManager.SaveCurrentUI()
	if g_UIManager.m_currentUI ~= nil then
		table.insert(g_UIManager.m_uiQueue,g_UIManager.m_currentUI.m_uiParam);
	end
end

function UIManager.ChangeUI(uiName,jumpData)
	--先调用原来的销毁方法
	if g_UIManager.m_currentUI ~= nil then
		g_UIManager.m_currentUI:Destory();
	end
	
	g_UIManager.m_currentUI = UITypes[uiName];
	g_UIManager.m_currentUI.m_uiParam = UIDataBase:New(uiName);
	g_UIManager.m_currentUI:LoadResource(g_UIManager.m_uiRoot.transform,uiName);
	g_UIManager.m_currentUI:Init(jumpData);
end

--清除队列，打断所有跳转
function UIManager.ClearQueue()
	g_UIManager.m_uiQueue = {};
end

--返回到上一个UI，与YY不同，这里允许添加参数
--因为可能存在上个场景做了一些事情使得跳转回来以后界面要发生变化
--这里如果不填就用当时保存的状态，如果有填参数就用参数的状态
function UIManager.BackToPrevUI(jumpData)
	local lastUIData = jumpData;
	if lastUIData == nil then
		lastUIData = g_UIManager.m_uiQueue[#g_UIManager.m_uiQueue];
	end
	
	if lastUIData ~= nil then
		table.remove(g_UIManager.m_uiQueue,#g_UIManager.m_uiQueue);
		UIManager.ChangeUI(lastUIData.m_uiName,lastUIData);
	else
		g_UIManager.m_currentUI:Destory();
		g_UIManager.m_currentUI = nil;
	end
end