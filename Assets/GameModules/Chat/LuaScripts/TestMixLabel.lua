--created from template

--created at 13/04/2018 14:21:14
require "Common/Tdefine"
require "Common/functions"

TestMixLabel = {}

local gameObject;
local mixedLabel;
local weirdString = "我是[e-1][e-2]一段[h-超链接文字超链接文字超链接文字超链接文字超链接文字_DoAction]，一个[e-1][e-2]一个[e-1][e-2]一个[e-1][e-2]一个[e-1][e-2]一个[e-1][e-2]一个[e-1][e-2]一个[e-1][e-2]一个[e-1][e-2]，外加一张[i-aaa]和一张[i-bbb]";
function TestMixLabel.New()


    gameObject = LuaHelper.CreateInstance("Assets/GameModules/Chat/Prefabs/GameObject.prefab");
    mixedLabel = gameObject:GetComponent("MixedLabel");
    mixedLabel:Init(weirdString, 200);
end
TestMixLabel.New();