require "UI/UI_LogicBase"

UI_Items = UI_LogicBase:New();

function UI_Items:Init(jumpData)
	if jumpData ~= nil then
		local path = "Frame/Item" .. jumpData.itemId .. "/text";
		local item = LuaFramework.Util.Child(self.m_uiObj,path);
		item = item:GetComponent("Text");
		item.text = jumpData.itemCount;
		
		self.m_uiParam.itemId = jumpData.itemId;
		self.m_uiParam.itemCount = jumpData.itemCount;
	end
	
	local closeBtn = LuaFramework.Util.Child(self.m_uiObj,"Close");
	closeBtn = closeBtn:GetComponent("Button");
	closeBtn.onClick:AddListener(function()
				self:OnClickClose();
	end);
	
	
	local add1 = LuaFramework.Util.Child(self.m_uiObj,"Frame/Add1");
	add1 = add1:GetComponent("Button");
	add1.onClick:AddListener(function()
				self:OnClickAdd1();
	end);
	
	local add2 = LuaFramework.Util.Child(self.m_uiObj,"Frame/Add2");
	add2 = add2:GetComponent("Button");
	add2.onClick:AddListener(function()
				self:OnClickAdd2();
	end);
	
	local add3 = LuaFramework.Util.Child(self.m_uiObj,"Frame/Add3");
	add3 = add3:GetComponent("Button");
	add3.onClick:AddListener(function()
				self:OnClickAdd3();
	end);
	
	local add4 = LuaFramework.Util.Child(self.m_uiObj,"Frame/Add4");
	add4 = add4:GetComponent("Button");
	add4.onClick:AddListener(function()
				self:OnClickAdd4();
	end);
	
	local add5 = LuaFramework.Util.Child(self.m_uiObj,"Frame/Add5");
	add5 = add5:GetComponent("Button");
	add5.onClick:AddListener(function()
				self:OnClickAdd5();
	end);
	
	local add6 = LuaFramework.Util.Child(self.m_uiObj,"Frame/Add6");
	add6 = add6:GetComponent("Button");
	add6.onClick:AddListener(function()
				self:OnClickAdd6();
	end);
	
end

function UI_Items:OnClickClose()
	UIManager.BackToPrevUI();
end

function UI_Items:OnClickAdd1()
	local jumpData = UIDataBase:New("UI_Shop");
	jumpData.itemId = 1;
	jumpData.itemCount = 5;
	UIManager.ShowUI("UI_Shop",jumpData);
end

function UI_Items:OnClickAdd2()
	local jumpData = UIDataBase:New("UI_Shop");
	jumpData.itemId = 2;
	jumpData.itemCount = 6;
	UIManager.ShowUI("UI_Shop",jumpData);
end

function UI_Items:OnClickAdd3()
	local jumpData = UIDataBase:New("UI_Shop");
	jumpData.itemId = 3;
	jumpData.itemCount = 7;
	UIManager.ShowUI("UI_Shop",jumpData);
end

function UI_Items:OnClickAdd4()
	local jumpData = UIDataBase:New("UI_Shop");
	jumpData.itemId = 4;
	jumpData.itemCount = 8;
	UIManager.ShowUI("UI_Shop",jumpData);
end

function UI_Items:OnClickAdd5()
	local jumpData = UIDataBase:New("UI_Shop");
	jumpData.itemId = 5;
	jumpData.itemCount = 9;
	UIManager.ShowUI("UI_Shop",jumpData);
end

function UI_Items:OnClickAdd6()
	local jumpData = UIDataBase:New("UI_Shop");
	jumpData.itemId = 6;
	jumpData.itemCount = 10;
	UIManager.ShowUI("UI_Shop",jumpData);
end