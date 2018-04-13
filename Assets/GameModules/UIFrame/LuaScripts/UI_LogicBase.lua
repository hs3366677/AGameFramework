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

--����UI��Դ�����պ���д
function UI_LogicBase:LoadResource(parent,uiName)
	--����UI��Դ
	self.m_uiObj = LuaFramework.Util.LoadResource(parent,uiName);
	self.m_uiObj.name = uiName;
end

--��ʼ������
function UI_LogicBase:Init(jumpData)
	print("Init is not overrided!")
	return 0;
end

--���ٷ���,����̳в���д�ˣ����Ҫ�����Լ���UI Object!
function UI_LogicBase:Destory()
	print("Destory is not overrided!")
	self.m_uiObj:DestroyImmediate();
end