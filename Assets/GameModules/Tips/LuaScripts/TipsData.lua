TipsData = {}; 	--Tips数据内容目前回调都是没有参数的，未来可以考虑加入参数

TipsData.__index = TipsData;

function TipsData:New(priority,tipsType,content,confrmAction , cancelAction, closeAction)
	--创建
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