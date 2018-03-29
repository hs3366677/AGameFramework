using ProtoBuf;
using Protocols;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoContract]
public class LoginTest : ProtoBody
{
    [ProtoMember(1)]
    public Int32 id = 1;
    [ProtoMember(2)]
    public string name = "2";
    [ProtoMember(3)]
    public string email = "3";
}
