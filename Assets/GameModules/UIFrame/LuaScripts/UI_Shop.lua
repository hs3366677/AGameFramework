require "UI/UI_LogicBase"

UI_Shop = UI_LogicBase:New();

function UI_Shop:Init(jumpData)
	
	local closeBtn = LuaFramework.Util.Child(self.m_uiObj,"Close");
	closeBtn = closeBtn:GetComponent("Button");
	closeBtn.onClick:AddListener(function()
				self:OnClickClose();
	end);
end

function UI_Shop:OnClickClose()
	UIManager.BackToPrevUI();
end