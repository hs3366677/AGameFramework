using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using Debug = UnityEngine.Debug;
using System.Data;

/// <summary>
/// 实现自动读取Excel的DB基类
/// </summary>
public class AutoImportDatabase : MonoBehaviour
{
    /// <summary>
    /// 导入数据
    /// </summary>
    public virtual void ImportData(ExcelData reader)
    {
        // 按顺序记录CSV文件中字段的名称
        List<string> fieldNames = reader.secondNames;
        Dictionary<string, string> rawDatas = new Dictionary<string, string>(fieldNames.Count);

        int colCount = fieldNames.Count;	// 总列数
        int rowCount = reader.Rows.Count;

        string fieldName = null;			// 字段名称
        _AnalyseDbClz(rowCount, (eleType, rowIdx) =>
        {
            int curRow = rowIdx;
            try
            {
                for (int col = 0; col < colCount; ++col)
                {
                    fieldName = fieldNames[col];
                    rawDatas[fieldName] = reader.Rows[curRow][col].ToString();
                }

                return DataImporter.GenSingle(eleType, rawDatas);
            }
            catch (Exception exp)
            {
                Debug.LogError(string.Format("Row {0} import faild.", curRow));

                if (null != exp.InnerException)
                {
                    Debug.LogError(exp.InnerException);
                }
            }

            return null;
        });
    }

    protected void _AnalyseDbClz(int rowCount, Func<Type, int, object> getNextElement)
    {
        FieldInfo[] fieldInfos = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        FieldInfo info = null;
        DbFieldAttribute attr = null;

        for (int i = 0; i < fieldInfos.Length; ++i)
        {
            info = fieldInfos[i];
            attr = Attribute.GetCustomAttribute(info, typeof(DbFieldAttribute), true) as DbFieldAttribute;
            if (attr == null || attr.FieldType == DbFieldType.None) continue;
            object sequenceData = null;

            switch (attr.FieldType)
            {
                case DbFieldType.Array:
                    sequenceData = DataImporter.GenArray(info.FieldType, rowCount,
                        idx => getNextElement(DataImporter.GetArrayElementType(info.FieldType), idx));

                    break;

                case DbFieldType.List:
                    sequenceData = DataImporter.GenList(info.FieldType, rowCount,
                        idx => getNextElement(DataImporter.GetGenericListElementType(info.FieldType), idx));

                    break;
                default:
                    continue;
            }

            info.SetValue(this, sequenceData);
        }
    }
}

/// <summary>
/// 将Excel表格数据转化成C#数据结构
/// </summary>
public class ExcelData
{
    public string className;
    public List<string> firstNames;
    public List<string> secondNames;
    public List<List<string>> Rows = new List<List<string>>();
}