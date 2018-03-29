TestProtocolPanel = {};
local this = TestProtocolPanel;

--启动事件--
function TestProtocolPanel.Awake(obj)
	gameObject = obj;
	transform = obj.transform;

	this.InitPanel();
	logWarn("Awake lua--->>"..gameObject.name);
end

--初始化面板--
function TestProtocolPanel.InitPanel()
    this.btnOpen = transform:Find("Button").gameObject;
    this.content = transform:Find("Text").gameObject:GetComponent("Text");
end

--单击事件--
function TestProtocolPanel.OnDestroy()
	logWarn("OnDestroy---->>>");
end