require "UI/UI_LogicBase"

UI_Main = UI_LogicBase:New()

function UI_Main:Init(jumpData)
	local jumpBtn = LuaFramework.Util.Child(self.m_uiObj,"Button");
	jumpBtn = jumpBtn:GetComponent("Button");
	jumpBtn.onClick:AddListener(function()
				self:OnClickCharacter();
	end);
	
	local changeBtn = LuaFramework.Util.Child(self.m_uiObj,"Button1");
	changeBtn = changeBtn:GetComponent("Button");
	changeBtn.onClick:AddListener(function()
				self:OnClickItems();
	end);
	
	local backBtn = LuaFramework.Util.Child(self.m_uiObj,"Button2");
	backBtn = backBtn:GetComponent("Button");
	backBtn.onClick:AddListener(function()
				self:OnClickCombine();
	end);
end

function UI_Main:OnClickCharacter()
	UIManager.ShowUI("UI_Character",nil,false);
end

function UI_Main:OnClickItems()
	UIManager.ShowUI("UI_Items",nil,false);
end

function UI_Main:OnClickCombine()
	UIManager.ShowUI("UI_Shop",nil,false);
end