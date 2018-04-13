--UI跳转专用类
UIDataBase = {m_uiName = 0};

UIDataBase.__index = UIDataBase;

function UIDataBase:New(uiName)
	local self = {};
	setmetatable(self,UIDataBase);
	self.m_uiName = uiName;
	return self;
end