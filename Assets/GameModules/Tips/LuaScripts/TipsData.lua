TipsData = {}; 	--Tips��������Ŀǰ�ص�����û�в����ģ�δ�����Կ��Ǽ������

TipsData.__index = TipsData;

function TipsData:New(priority,tipsType,content,confrmAction , cancelAction, closeAction)
	--����
	local self = {};
	setmetatable(self,TipsData);
	
	self.priority = priority;
	self.tipsType = tipsType;
	self.content = content;
	self.confrimAction = confrmAction;
	self.cancelAction = cancelAction;
	self.closeAction = closeAction;
	
	return self;
end