require "TipsManager/GameTips"
require "TipsManager/TipsData"

TipsManager = {}; --Tips管理器


g_TipsManager = TipsManager;
local index = 0;

--初始化Tips系统，需要外部传入一个UI根节点
--创建一个TipsRoot，所有Tips的父级，方便管理
function TipsManager.Init(parent)
	g_TipsManager.m_tipsRoot = GameObject("TipsManager");
	g_TipsManager.m_tipsRoot.transform:SetSiblingIndex(1000);--假定Tips从1000开始
	if parent ~= nil then
		g_TipsManager.m_tipsRoot.transform:SetParent(parent.transform);
		LuaFramework.Util.SetLocalPosition(g_TipsManager.m_tipsRoot.transform,Vector3.zero);
		g_TipsManager.m_tipsRoot.transform.localScale = Vector3.one;
	end
	
	--当前正在显示的Tips
	g_TipsManager.m_currentTips = 0;
	--Tips队列
	g_TipsManager.m_tipsList = {};
	--提示的实例
	g_TipsManager.m_tipsInstance = TipsManager.CreateNewTips();
end

--创建一个Tips并加入到队列中，参数（从左到右）：类型，文字内容，点击确定的回调，点击取消的回调，点击关闭回调
function TipsManager.AddTips(priority,tipsType,content,confrmAction , cancelAction, closeAction)
	table.insert(g_TipsManager.m_tipsList,TipsData:New(priority,tipsType,content,confrmAction,cancelAction,closeAction));
	TipsManager.NextTips();
end

--新创建一个Tips
function TipsManager.CreateNewTips()
	local newTips = GameTips:CreateTips(g_TipsManager.m_tipsRoot.transform,TipsManager.OnTipsClose);
	return newTips;
end

--当有Tips关闭时调用
function TipsManager.OnTipsClose(gameTips)
	--if #g_TipsManager.m_tipsList == 0 then
	--	g_TipsManager.m_currentTips = 0;
	--end
	g_TipsManager.m_currentTips = 0;
	g_TipsManager.NextTips();
end

--清理Root下所有的Tips
function TipsManager.ClearAll()
	for i=1 ,#g_TipsManager.m_currnettipsList,1 do
		g_TipsManager.m_currnettipsList[i] = nil;
	end
	g_TipsManager.m_tipsInstance:OnHide();
end

function TipsManager.NextTips()
	--列表为空就什么都不显示
	if #g_TipsManager.m_tipsList == 0 then
		return;
	end
	
	local wantToShowTips = g_TipsManager.m_currentTips;
	local index = 0;
	for i=1,#g_TipsManager.m_tipsList,1 do
		local data = g_TipsManager.m_tipsList[i];
		
		if wantToShowTips == 0 then
			wantToShowTips = data;
			index = i;
		elseif wantToShowTips.priority < data.priority then
			wantToShowTips = data;
			index = i;
		end
	end
	
	
	if wantToShowTips ~= 0 and wantToShowTips ~= g_TipsManager.m_currentTips then
		if g_TipsManager.m_currentTips ~= 0 then
			table.insert(g_TipsManager.m_tipsList,1,g_TipsManager.m_currentTips);
			index = index + 1
		end
		
		table.remove(g_TipsManager.m_tipsList,index);
		g_TipsManager.m_currentTips = wantToShowTips;
		TipsManager.ShowTips(g_TipsManager.m_currentTips,index);
	end
end

--正常显示
function TipsManager.ShowTips(data,index)
	if data.tipsType == 1 then
		g_TipsManager.m_tipsInstance:Refreash_ConfrimCancelTips(data.content,data.confrimAction,data.cancelAction,data.closeAction);
	else
		g_TipsManager.m_tipsInstance:Refreash_YesNoTips(data.content,data.confrimAction,data.cancelAction,data.closeAction);
	end
end