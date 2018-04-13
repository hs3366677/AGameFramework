--created from template

--created at 28/03/2018 15:48:25
require "TestLuaItem"
TestLua = {}

AssetDatabase = UnityEditor.AssetDatabase;
local gameObject;
local eventButton;
local uiName = "testUI";
local buttonTemplate;
local quitButton;
local grid;

local items = {};

function TestLua.New()
    print("TestLua Start");
    gameObject = LuaHelper.CreateInstance("Assets/GameModules/Pet/Prefab/TestUI2.prefab");
    print("TestLua End");
    
    for i = 1,10 do
        items[i] = TestLuaItem.New(i, "otome"..i, "otomep"..i);
    end
    
    TestLua.FindObject();
end

function TestLua.FindObject()

    print("TestLua FindObject");
    buttonTemplate = gameObject.transform:Find("ButtonTemplate").gameObject;
    grid = find("Content").transform;

    for i = 1,10 do
        local itemView = newObject(buttonTemplate);
        itemView.transform:Find("id"):GetComponent('Text').text = items[i].id;
        itemView.transform:Find("name"):GetComponent('Text').text = items[i].name;
        itemView.transform:Find("password"):GetComponent('Text').text = items[i].password;
        itemView.transform.parent = grid;
    end
    print("TestLua FindObject End");
    
end

function TestLua.OnClickButton(go)

print("TestLua Start");
    LuaHelper.CreateInstance()
end

TestLua.New();
