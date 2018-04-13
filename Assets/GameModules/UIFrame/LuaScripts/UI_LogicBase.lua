require "UI/UIDataBase"

UI_LogicBase = 
{
	m_uiObj = 0;
};
UI_LogicBase.__index = UI_LogicBase;

function UI_LogicBase:New()
	local self = {};
	setmetatable(self,UI_LogicBase);
	return self;
end

--加载UI资源，待日后重写
function UI_LogicBase:LoadResource(parent,uiName)
	--加载UI资源
	self.m_uiObj = LuaFramework.Util.LoadResource(parent,uiName);
	self.m_uiObj.name = uiName;
end

--初始化方法
function UI_LogicBase:Init(jumpData)
	print("Init is not overrided!")
	return 0;
end

--销毁方法,如果继承并重写了，务必要销毁自己的UI Object!
function UI_LogicBase:Destory()
	print("Destory is not overrided!")
	self.m_uiObj:DestroyImmediate();
end