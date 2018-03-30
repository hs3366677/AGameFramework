--created from template

--created at 28/03/2018 15:48:25
require "Common/Tdefine"
TestLua = {}

AssetDatabase = UnityEditor.AssetDatabase;
local gameObject;
local eventButton;
local uiName = "testUI";

function TestLua.New()
    print("TestLua Start");
    gameObject = LuaHelper.CreateInstance();
    print("TestLua End");
end

TestLua.New();

