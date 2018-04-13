

GameTips =
 {
	m_tipsObj = 0;
	m_confirm = 0;
	m_cancel = 0;
	m_close = 0;
	m_onTipsDestory = 0;
};

GameTips.__index = GameTips;
local isShow = false;

--创建一个Tips面板
function GameTips:CreateTips(parent,onClose)
	--创建
	local self = {};
	setmetatable(self,GameTips);
	self.m_tipsObj = self:CreateTipsObj(parent);
	self.m_tipsObj:SetActive(false);
	self.m_onTipsDestory = onClose;
	return self;
end

--刷新一个Tips，这里只需要更改内容和事件就可以了
function GameTips:Refreash_ConfrimCancelTips(content,confrimAction,cancelAction,closeAction)
	self:SetContentText(self.m_tipsObj,content);
	self:BindAction(confrimAction,cancelAction,closeAction,self.m_onTipsDestory);
	
	--由于是确定/取消，所以隐藏是否的按钮
	local yes_noObj = LuaFramework.Util.Child(self.m_tipsObj,"TipsFrame/yes_no");
	yes_noObj:SetActive(false);
	
	local confrim_cancelObj = LuaFramework.Util.Child(self.m_tipsObj,"TipsFrame/confrim_cancel");
	confrim_cancelObj:SetActive(true);
	
	self.m_tipsObj:SetActive(true);
	-- dotween
	--self.m_tween:DOPlay();
	DG.Tweening.DOTween.Restart(self.m_tweenObj,"ShowTips");
end

--刷新一个Tips，这里只需要更改内容和事件就可以了
function GameTips:Refreash_YesNoTips(content,confrimAction,cancelAction,closeAction)
	self:SetContentText(self.m_tipsObj,content);
	self:BindAction(confrimAction,cancelAction,closeAction,self.m_onTipsDestory);
	
	--由于是是/否，所以隐藏确定/取消的按钮
	local confrim_cancelObj = LuaFramework.Util.Child(self.m_tipsObj,"TipsFrame/confrim_cancel");
	confrim_cancelObj:SetActive(false);
	
	local yes_noObj = LuaFramework.Util.Child(self.m_tipsObj,"TipsFrame/yes_no");
	yes_noObj:SetActive(true);
	
	self.m_tipsObj:SetActive(true);
	-- dotween
	DG.Tweening.DOTween.Restart(self.m_tweenObj,"ShowTips");
	--self.m_tween:DOPlay();
end

-- 资源加载，日后需要重写
function GameTips:CreateTipsObj(parent)
	--创建一个弹窗模板
	local tipsObj = LuaFramework.Util.LoadResource(parent,"Tips");

	local tipsComp = tipsObj:GetComponent("Game_Tips");
	tipsComp:InitLuaTable(self);
	
	local tipsFrame = LuaFramework.Util.Child(tipsObj.transform,"TipsFrame");
	self.m_tweenObj = tipsFrame.gameObject;
	
	--local compType = typeof('DG.Tweening.DOTweenAnimation');
	--if tipsFrame ~= nil then
	--	local tweenComps = tipsFrame:GetComponents('DG.Tweening.DOTweenAnimation');
	--	for i = 1,#tweenComps,1 do
	--		tweenComps[i].animationType = DG.Tweening.Core.DOTweenAnimationType.Scale;
	--		tweenComps[i].autoPlay = false;
	--		tweenComps[i].autoKill = false;
	--		tweenComps[i].duration = 0.2;--Tween时间
	--	end
	--end
	
	return tipsObj;
end

--把内容文字设置好
function GameTips:SetContentText(root,text)
	local contentTrans = LuaFramework.Util.Child(root,"TipsFrame/content");
	local contentText = contentTrans:GetComponent("Text");
	contentText.text = text;
end

--绑定回调
function GameTips:BindAction(yesAction,noAction,closeAction,onTipsDestory)
	self.m_confirm = yesAction;
	self.m_cancel = noAction;
	self.m_close = closeAction;
	self.m_onTipsDestory = onTipsDestory;
end

function GameTips.OnClickConfrim(self)
	if self.m_confirm ~= 0 then
		self.m_confirm();
	end
	
	DG.Tweening.DOTween.Restart(self.m_tweenObj,"HideTips");
end

function GameTips.OnClickCancel(self)
	if self.m_cancel ~= 0 then
		self.m_cancel();
	end
	
	DG.Tweening.DOTween.Restart(self.m_tweenObj,"HideTips");
end

function GameTips.OnClickClose(self)
	if self.m_close ~= 0 then
		self.m_close();
	end
	
	DG.Tweening.DOTween.Restart(self.m_tweenObj,"HideTips");
end

function GameTips.OnHideComplete(self)
	self:OnHide();
end

function GameTips:OnHide()
	self.m_tipsObj:SetActive(false);
	
	if self.m_onTipsDestory ~= 0 then
		self.m_onTipsDestory(self);
	end
end