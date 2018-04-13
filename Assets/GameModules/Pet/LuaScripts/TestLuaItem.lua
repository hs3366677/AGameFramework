--created from template

--created at 28/03/2018 15:48:25
require "Common/Tdefine"
require "Common/functions"
TestLuaItem = {
    id = "",
    name = "", 
    password = ""
}

TestLuaItem.___index = TestLuaItem;

function TestLuaItem.New(id, name, password)
    local o = {};
    setmetatable(o, self);
    o.id = id;
    o.name = name;
    o.password = password;
    return o;
end
