using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestExcelDB:AutoImportDatabase
{
    [DbField(DbFieldType.List)]
	public List<TestExcel> dataList = new List<TestExcel>();
	public static TestExcelDB Instance;
    void Awake() { Instance = this; }
}

[System.Serializable]
public class TestExcel
{
    
 public int key;

 public string name;

 public string skill;

 public float hp;

 public int mp;

 public int attack;

 public int defense;

}
