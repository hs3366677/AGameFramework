using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
public class TestData:AutoImportDatabase
{
    [DbField(DbFieldType.List)]
    public List<tData> tDataList = new List<tData>();

    public static TestData Instance;
    void Awake() { Instance = this;
    }

}

[System.Serializable]
public class tData
{
    /// <summary>
    ///id
    /// <summary>
    public int key;
    /// <summary>
    ///名称
    /// <summary>
    public string name ;
    /// <summary>
    ///技能
    /// <summary>
    public string skill ;
    /// <summary>
    ///血量
    /// <summary>
    public float hp ;
    /// <summary>
    ///能量
    /// <summary>
    public int mp ;
    /// <summary>
    ///攻击力
    /// <summary>
    public int attack ;
    /// <summary>
    ///防御力
    /// <summary>
    public int defense ;
}
