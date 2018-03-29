using Protocols;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TestHelper 
{
    public static NetworkController GetNetworkController()
    {
        return NetworkController.Instance;
    }
}
