using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocols;
using LuaFramework;

public class TestAPI : MonoBehaviour {

	// Use this for initialization
	void Start () {
        byte[] testByteArray = new byte[] { 0x23, 0x20, 0x12, 0x25, 0x66, 0x78, 0x66 };

        //Util.CallMethod("Network", "OnSocket", buffer.Key, buffer.Value);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
