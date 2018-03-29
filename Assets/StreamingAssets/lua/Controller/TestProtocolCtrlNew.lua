require "Common/Tdefine"

require "3rd/pblua/login_pb"
require "3rd/pbc/protobuf"
require "3rd/pblua/TestProtoBody_pb"

local sproto = require "3rd/sproto/sproto"
local core = require "sproto.core"
local print_r = require "3rd/sproto/print_r"


TestProtocolCtrl = {};
local this = TestProtocolCtrl;

local panel;
local prompt;
local transform;
local gameObject;

--[[构造函数]]--
--构建函数--
function TestProtocolCtrl.New()
	logWarn("TestProtocolCtrlNew.New--->>");
	return this;
end

function TestProtocolCtrl.Awake()
	logWarn("TestProtocolCtrlNew.Awake--->>");
	panelMgr:CreatePanel('TestProtocol', this.OnCreate);
end

--初始化面板--
function TestProtocolCtrl.OnCreate(objs)

	transform = objs.transform;
	gameObject = objs;

    prompt = transform:GetComponent('LuaBehaviour');
    prompt:AddClick(TestProtocolPanel.btnOpen, this.OnClick);

    networkMgr.register(this.OnMessageRecv);
end

function TestProtocolCtrl.OnMessageRecv(msg)
    --local protocal = buffer:ReadByte();
	local data = buffer:ReadBuffer();

	print("开始反序列化-------------")

    local msg = TestProtoBody_pb.TestProtoBody();--logintest_pb.LoginTestResponse();
	msg:ParseFromString(data);
	
	print("解析成功-------------id: "..msg.id..msg.name)
	
	local logStr = TestProtocolPanel.content.text.."id:"..msg.id.."   name:"..msg.name.."\n";


end

--按钮单击--
function TestProtocolCtrl.OnClick(go)
	print(go.name);

	local sp = sproto.parse [[
    .TestProtoBody {
        testInt 1 : integer
        testString 2 : string
	}
	
	.AddBoby {
		TestProtoBody 0 : *TestProtoBody
	}
	]]
	
	local ab = {
		TestProtoBody =  {
			testInt = 405,
			testString = "吃早餐了"
		}
	}

	local code = sp:encode("AddBoby", ab)

	local buffer = ByteBuffer.New();
    buffer:WriteShort(Protocal.Message);
    buffer:WriteByte(ProtocalType.SPROTO);
	buffer:WriteBuffer(code);
	
	networkMgr:SendLuaMessage("0xff01",buffer);

end