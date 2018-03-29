using System;
using System.Data;
using System.Data.OleDb;
using UnityEngine;
using Excel;
using System.IO;
using System.Collections.Generic;

public class Helper
{

    /// <summary>
    /// 读Excel表格数据
    /// </summary>
    /// <param name="_path"></param>
    /// <returns></returns>
    public static ExcelData GetExcelData(string _path,string _name)
    {
        Debug.Log("文件地址： "+_path);
        if (File.Exists(_path) == false)
        {
            return null;
        }

        FileStream stream = File.Open(_path, FileMode.Open, FileAccess.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        stream.Close();

        int maxCount = 0;
        int rowIndex = 0;
        ExcelData eData = new ExcelData();

        eData.className = _name;
        eData.firstNames = new List<string>();
        eData.secondNames = new List<string>();
        do
        {
            Debug.Log(excelReader.Name);
            while (excelReader.Read())
            {
                if (rowIndex == 1)
                {
                    maxCount = excelReader.FieldCount;

                    for (int i = 0; i < excelReader.FieldCount; i++)
                    {
                        if (excelReader.IsDBNull(i))
                        {
                            maxCount = i;
                            break;
                        }
                        else {
                            eData.firstNames.Add(excelReader.GetValue(i).ToString());
                        }
                    }
                }
                else if (rowIndex == 2)
                {
                    maxCount = excelReader.FieldCount;

                    for (int i = 0; i < excelReader.FieldCount; i++)
                    {
                        if (excelReader.IsDBNull(i))
                        {
                            maxCount = i;
                            break;
                        }
                        else
                        {
                            eData.secondNames.Add(excelReader.GetValue(i).ToString());
                        }
                    }
                }
                else if (rowIndex > 2)
                {
                    List<string> values = new List<string>();

                    for (int i = 0; i < maxCount; i++)
                    {
                        string value = excelReader.IsDBNull(i) ? "" : excelReader.GetValue(i).ToString();
                        Debug.Log(value);
                        values.Add(value);
                    }

                    eData.Rows.Add(values);
                }

                rowIndex++;
            }
        } while (excelReader.NextResult());

        return eData;
    }

    /// <summary>
    /// 读取表格数据
    /// </summary>
    /// <param name="_path">文件绝对路径</param>
    /// <param name="versions12"> 是否是12版本</param>
    /// <returns></returns>
    public static DataTable GetDataTable(string _path, bool _versions12 = false)
    {
        DataSet ds = getData(_path, _versions12);
        if (ds == null)
            return null;
        return ds.Tables[0];
    }
    /// <summary>
    /// 读取表格数据
    /// </summary>
    /// <param name="_path">文件绝对路径</param>
    /// <param name="versions12"> 是否是12版本</param>
    /// <returns></returns>
    public static DataSet getData(string _path, bool versions12 = false)
    {
        var path = _path;
        string fileSuffix = System.IO.Path.GetExtension(path);
        if (string.IsNullOrEmpty(fileSuffix))
            return null;

        try
        {
            using (DataSet ds = new DataSet())
            {
                //判断Excel文件是2003版本还是2007版本
                string connString = "";
                if (versions12 == false)
                    connString = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + path + ";" + ";Extended Properties=\"Excel 8.0;HDR=NO;IMEX=1\"";//HDR=YES  表示第一行是表头，不是数据，HDR=NO，反之
                else
                    connString = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + path + ";" + ";Extended Properties=\"Excel 12.0;HDR=NO;IMEX=1\"";
                //读取文件
                string sql_select = " SELECT * FROM [Sheet1$]";
                using (OleDbConnection conn = new OleDbConnection(connString))
                using (OleDbDataAdapter cmd = new OleDbDataAdapter(sql_select, conn))
                {
                    conn.Open();
                    cmd.Fill(ds);
                }
                if (ds == null || ds.Tables.Count <= 0) return null;
                return ds;
            }
        }
        catch (Exception e)
        {
            if (versions12 == false)
                return getData(_path, true);
            else
            {
                Debug.LogError(e.ToString());
                return null;
                //Microsoft版本有问题
            }
        }
    }


    static bool IsAssignFormat(object _format, object _value)
    {
        bool isSure = false;
        string valueToString = _value.ToString();
        switch (_format.ToString())
        {
            case "int":
                int nValue = 0;
                isSure = int.TryParse(valueToString, out nValue);
                return isSure;
            case "float":
                float fValue = 0.0f;
                isSure = float.TryParse(valueToString, out fValue);
                return isSure;
            case "string":
                return !string.IsNullOrEmpty(valueToString);
            case "int[]":
                if (valueToString.Contains(";"))
                {
                    string[] nValues = valueToString.Split(';');
                    foreach (string item in nValues)
                    {
                        int nV = 0;
                        if (!int.TryParse(valueToString, out nV))
                        {
                            return false;
                        }
                    }
                }
                else
                    return false;
                return true;
            case "float[]":
                if (valueToString.Contains(";"))
                {
                    string[] nValues = valueToString.Split(';');
                    foreach (string item in nValues)
                    {
                        float nV = 0.0f;
                        if (!float.TryParse(valueToString, out nV))
                        {
                            return false;
                        }
                    }
                }
                else
                    return false;
                return true;
            case "string[]":
                if (valueToString.Contains(";"))
                {
                    string[] nValues = valueToString.Split(';');
                    foreach (string item in nValues)
                    {
                        if (string.IsNullOrEmpty(item))
                            return false;
                    }
                }
                else
                    return false;
                return true;
        }
        return !string.IsNullOrEmpty(valueToString);
    }


    /// <summary>
    /// 最大的列数
    /// </summary>
    /// <returns></returns>
    public static int GetMaxLine(object[] _array)
    {
        int mvalue = 0;
        for (int i = 0; i < _array.Length; i++)
        {
            string svalue = _array[i].ToString();
            if (string.IsNullOrEmpty(svalue))
            {
                return mvalue;
            }
            mvalue++;
        }
        return mvalue;
    }
}
