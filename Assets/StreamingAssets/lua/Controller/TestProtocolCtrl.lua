require "Common/Tdefine"

require "3rd/pblua/logintest_pb"
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

--构建函数--
function TestProtocolCtrl.New()
	logWarn("TestProtocolCtrl.New--->>");
	return this;
end

function TestProtocolCtrl.Awake()
	logWarn("TestProtocolCtrl.Awake--->>");
	panelMgr:CreatePanel('TestProtocol', this.OnCreate);

	Event.AddListener(Protocal.CMD_LUA_PROTO, this.OnMessage2); 
end

--初始化面板--
function TestProtocolCtrl.OnCreate(objs)

	transform = objs.transform;
	gameObject = objs;

    prompt = transform:GetComponent('LuaBehaviour');
	prompt:AddClick(TestProtocolPanel.btnOpen, this.OnTestClick); 
end

function TestProtocolCtrl.OnTestClick(go)
	local login = TestProtoBody_pb.TestProtoBody();--logintest_pb.LoginTestRequest();
    login.id = 2018;
    login.name = '名字';
	--login.email = 'test@163.com';

	for i = 1,2 do
		local infoo = login.infos:add();
		infoo.id = i;
		infoo.name = "第"..i.."个";
		infoo.type = 2;
	end

	--[[ for i = 1,2 do
		local dica = login.dic:add();
		dica.Key = i;

		local info1 = TestProtoBody_pb.TestProtoBody1();
		info1.id = "10";
		info1.name = "hao";
		dica.Value = info1;
	end ]]

	local msg = login:SerializeToString();

	TestProtocolPanel.content.text = msg.."\n";
	print("----------------------"..msg);
    ----------------------------------------------------------------
	local buffer = ByteBuffer.New();
    buffer:WriteShort(Protocal.CMD_LUA_PROTO);
    buffer:WriteBuffer(msg);
    --networkMgr:SendMessage(buffer);
	

	tNetworkCtrl:LuaSendMessage("0xff01",buffer); 
end

--登录返回--
function TestProtocolCtrl.OnMessage2(buffer) 

    this.LoginSproto(buffer);
end

--PBC——lua登录--
function TestProtocolCtrl.LoginSproto(buffer)
    print("回调成功-------------")

    --local protocal = buffer:ReadByte();
	local data = buffer:ReadBuffer();

	print("开始反序列化-------------")

    local msg = TestProtoBody_pb.TestProtoBody();--logintest_pb.LoginTestResponse();
	msg:ParseFromString(data);
	
	print("解析成功-------------id: "..msg.id..msg.name)
	
	local logStr = TestProtocolPanel.content.text.."id:"..msg.id.."   name:"..msg.name.."\n";

	--[[ for i,v in ipairs(msg.infos) 
	    do print("很棒："..v.id..v.name..tostring(v.type)) 
		logStr = logStr..v.id..v.name..tostring(v.type).."\n"
	end 

	for i,v in ipairs(msg.dic) 
	    do print("很棒："..i..tostring(v)) 
		--logStr = logStr..v;
	end  ]]
	
	TestProtocolPanel.content.text = logStr;
end