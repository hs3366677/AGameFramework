/********************************************************************
	created:	2015/12/18
	created:	18:12:2015   10:24
	filename: 	F:\Otome\Dev\trunk\Client\Assets\Scripts\Importer\DataImporter.cs
	file path:	F:\Otome\Dev\trunk\Client\Assets\Scripts\Importer
	file base:	DataImporter
	file ext:	cs
	author:		柴猛
	
	purpose:	定义数据导入的工具类
 *  [1/4/2016 柴猛] 
 *  1、添加对enum类型数据的处理
 *  2、自定类型对象导入完成后会调用一个特定的方法，用来做不方便统一处理的导入操作。
 *		方法声明为public void CustomImport(Dictionary<string, string>);
*********************************************************************/
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

public static class DataImporter
{
	/// <summary>
	/// 生成简单一维数组对象，并向数组中填充数据。
	/// 数组元素为int、float、string等简单数据类型，如果不是要自定义string类型的Cast
	/// 数组的类型为int[],float[]等
	/// </summary>
	/// <param name="arrType">数组的Type。注意数组元素的Type，要确保有string到该类型的转换</param>
	/// <param name="rawDatas">数组中的原始数据</param>
	/// <returns>创建的新数组对象（导入了rawDatas中的数据）</returns>
	public static Array GenSimpleArray(Type arrType, List<string> rawDatas)
	{
		Type arrElementType = GetArrayElementType(arrType);

		if (arrElementType == null) return null;

		return GenArray(arrType, rawDatas.Count, idx => Convert.ChangeType(rawDatas[idx], arrElementType));
	}

	/// <summary>
	/// 生成元素类型任意的数组
	/// </summary>
	/// <param name="arrType">数组的Type</param>
	/// <param name="arrLen">数组的长度</param>
	/// <param name="getNextElement">
	/// 获取数组元素的回调。函数调用者要通过该回调，逐个返回要加入到数组中的元素。
	/// arg：idx，当前要加入的元素的索引
	/// return：返回当前要加入到数组的元素
	/// </param>
	public static Array GenArray(Type arrType, int arrLen, Func<int, object> getNextElement)
	{
		Array array = Activator.CreateInstance(arrType, arrLen) as Array;
		if (array != null)
		{
			for (int i = 0; i < arrLen; ++i)
			{
				array.SetValue(getNextElement(i), i);
			}
		}

		return array;
	}

	/// <summary>
	/// 生成简单泛型List对象，并向List中填充数据。
	/// 数组元素为int、float、string等简单数据类型，如果不是要自定义string类型的Cast
	/// </summary>
	/// <param name="listType">List的Type。注意List元素的Type，要确保有string到该类型的转换</param>
	/// <param name="rawDatas">要导入到List中的源数据</param>
	/// <returns>创建的新List对象（导入了rawDatas中的数据）</returns>
	public static object GenSimpleList(Type listType, List<string> rawDatas)
	{
		Type elementType = GetGenericListElementType(listType);
		if (elementType == null) return null;

		return GenList(listType, rawDatas.Count, i => Convert.ChangeType(rawDatas[i], elementType));
	}

	/// <summary>
	/// 生成元素类型任意的泛型List
	/// </summary>
	/// <param name="listType">泛型List的Type</param>
	/// <param name="arrLen">List的元素个数</param>
	/// <param name="getNextElement">
	/// 获取List元素的回调。函数调用者要通过该回调，逐个返回要加入到List中的元素。
	/// arg：idx，当前要加入的元素的索引
	/// return：返回当前要加入到List的元素
	/// </param>
	public static object GenList(Type listType, int arrLen, Func<int, object> getNextElement)
	{
		object list = Activator.CreateInstance(listType);

		if (list != null)
		{
			MethodInfo methodInfo = listType.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);

			for (int i = 0; i < arrLen; ++i)
			{
				methodInfo.Invoke(list, new object[] {getNextElement(i)});
			}
		}

		return list;
	}

	public const string ImportMethodName = "CustomImport";

	/// <summary>
	/// 创建type的对象，并用rawDatas中的数据对已标记的字段进行赋值
	/// </summary>
	/// <param name="type">存储数据的类</param>
	/// <param name="rawDatas">key:字段名称，val：数据</param>
	/// <returns>导入数据后的对象</returns>
	public static object GenSingle(Type type, Dictionary<string, string> rawDatas)
	{
		Dictionary<string, FieldInfo> odm = BuildObjectDataMap(type);
		if (odm.Count == 0)
		{
			return null;
		}

		object objData = Activator.CreateInstance(type);

		object fieldData = null;
		string tmpData = null;
		Type fieldType = null;		// Filed对应的Type
        int col = 0;
		foreach (var field in odm)
		{
			//[29/1/2016 马天] 数据有问题时，报出错误信息，方便查询排错
            col++;
			try
			{
				fieldType = field.Value.FieldType;
				if (IsBasicDataType(fieldType))
				{
					if (rawDatas.TryGetValue(field.Key, out tmpData))
					{
						Type targetType = fieldType.IsEnum ? Enum.GetUnderlyingType(fieldType) : fieldType;
						fieldData = Convert.ChangeType(tmpData, targetType);
					}
					// [3/5/2016 柴猛] 类中的public字段在数据表中找不到相关列时，跳过数据导入
					else
					{
						continue;
					}
				}
				else
				{
					// 1、数组类型
					if (fieldType.IsArray)
					{
						fieldData = ImportSimpleArrayDatas(fieldType, field.Key, rawDatas);
					}
					// 2、List<T>类型
					else if (fieldType.ToString().StartsWith("System.Collections.Generic.List`1"))
					{
						fieldData = ImportSimpleListDatas(fieldType, field.Key, rawDatas);
					}
					// 3、自定义类型
					else
					{
						fieldData = GenSingle(fieldType, rawDatas);
					}
				}

				field.Value.SetValue(objData, fieldData);
			}
			catch (Exception e)
			{
				Debug.LogError(string.Format("Data class '{0}' import faild in col {1}:{2}:{3}", type.Name, col, field.Key, field.Value));
                Debug.LogError(e.Message);
				throw;
			}
		}

		// 各对象自己做不方便统一处理的导入工作
		MethodInfo methodInfo = type.GetMethod(ImportMethodName,
			BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public,
			null, new Type[]{typeof(Dictionary<string, string>)}, null);
		if (methodInfo != null)
		{
			methodInfo.Invoke(objData, new object[] { rawDatas });
		}

		return objData;
	}


	/// <summary>
	/// 导入简单数组的数据
	/// </summary>
	/// <param name="arrType">简单数组的Type</param>
	/// <param name="fieldName">类中的字段名</param>
	/// <param name="rawDatas">要加入到数组中的原始数据</param>
	/// <returns>导入了rawDatas中的数据数组对象</returns>
	public static Array ImportSimpleArrayDatas(Type arrType, string fieldName, Dictionary<string, string> rawDatas)
	{
		Type eleType = GetArrayElementType(arrType);
		if (eleType != null)
		{
			return GenSimpleArray(arrType, SeparateSequeneceDatas(fieldName, rawDatas));
		}

		return null;
	}

	/// <summary>
	/// 导入简单泛型List的数据
	/// </summary>
	/// <param name="listType">简单泛型List的Type</param>
	/// <param name="fieldName">类中的字段名</param>
	/// <param name="rawDatas">要加入到泛型List中的原始数据</param>
	/// <returns>导入了rawDatas中的数据泛型List对象</returns>
	public static object ImportSimpleListDatas(Type listType, string fieldName, Dictionary<string, string> rawDatas)
	{
		Type eleType = GetGenericListElementType(listType);
		if (eleType != null)
		{
			return GenSimpleList(listType, SeparateSequeneceDatas(fieldName, rawDatas));
		}

		return null;
	}


	/// <summary>
	/// 匹配序列数据的正则
	/// </summary>
	public const string SequenceFieldNamePattern = "^{0}_(?<idx>[0-9]*)$";

	/// <summary>
	/// 从rawDatas中分离出指定的序列数据
	/// 格式：rawData中key是[fieldName_idx]格式，导入时会检索符合这个格式的key，并将其对应的value加入的数组中。
	///			数组中元素的顺序是idx的递增序列。
	///			数组长度是数据的个数，和idx的最大值无关。
	/// 注意：重复的idx只会保留1个，被保留的数据不保证位置。
	/// </summary>
	/// <param name="fieldName">类中的字段名</param>
	/// <param name="rawDatas">原始数据</param>
	/// <returns>匹配了fieldName格式的序列数据</returns>
	public static List<string> SeparateSequeneceDatas(string fieldName, Dictionary<string, string> rawDatas)
	{
		Match match = null;
		SortedDictionary<int, string> sortedDatas = new SortedDictionary<int, string>();
		foreach (var pair in rawDatas)
		{
			match = Regex.Match(pair.Key, string.Format(SequenceFieldNamePattern, fieldName));
			int idx = 0;
			if (match.Success && int.TryParse(match.Result("${idx}"), out idx))
			{
				sortedDatas[idx] = pair.Value;
			}
		}

		return new List<string>(sortedDatas.Values);
	}


	/// <summary>
	/// 建立数据存储结构到数据源的映射关系
	/// 映射方式：类的字段名对应数据源(配置表)的列名，用对应列的数据填充对象的字段
	/// 字段映射约定：public字段默认进行映射，非public字段默认不进行映射；
	///				不符合约定的字段，可以使用DataFieldAttribute进行个别的标记。
	/// </summary>
	/// <param name="objType">数据存储结构的Type</param>
	/// <returns>生成的映射关系，key:配置表的列名；val:类中对应字段的信息</returns>
	public static Dictionary<string, FieldInfo> BuildObjectDataMap(Type objType)
	{
		// key：字段名；val：关联的类字段
		Dictionary<string, FieldInfo> clzFieldDic = new Dictionary<string, FieldInfo>();

		FieldInfo[] fieldInfos = objType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		FieldInfo info = null;
		DataFieldAttribute attr = null;
		string fieldName = null;
		for (int i = 0; i < fieldInfos.Length; ++i)
		{
			fieldName = null;
			info = fieldInfos[i];
			attr = Attribute.GetCustomAttribute(info, typeof(DataFieldAttribute), true) as DataFieldAttribute;
			if (attr != null)
			{
				if (attr.Type == DataFieldType.Read)
				{
					fieldName = attr.FieldName ?? info.Name;
				}
			}
			else
			{
				if (info.IsPublic)
				{
					fieldName = info.Name;
				}
			}

			if (!string.IsNullOrEmpty(fieldName))
			{
				clzFieldDic[fieldName] = info;
			}
		}

		return clzFieldDic;
	}

	/// <summary>
	/// 判断type是否是基本数据类型
	/// </summary>
	/// <param name="type">要判断的数据类型对应的Type</param>
	/// <returns>true:是基本数据类型；false:自定义或其他非基本类型</returns>
	public static bool IsBasicDataType(Type type)
	{
		return Type.GetTypeCode(type) != TypeCode.Object;
	}

	/// <summary>
	/// 获取数组元素的Type
	/// 例：传入typeof(int[])返回typeof(int)
	/// </summary>
	/// <param name="arrType">数组的Type</param>
	public static Type GetArrayElementType(Type arrType)
	{
		if (arrType != null && arrType.IsArray)
		{
			string typeName = arrType.ToString();

			if (typeName.EndsWith("[]"))
			{
				return Type.GetType(typeName.Substring(0, typeName.Length - 2));
			}

			return typeof (object);
		}

		return null;
	}

	/// <summary>
	/// 泛型List中元素的匹配正则
	/// </summary>
	public const string ListElementPattern = @"\[.*?\]";


	/// <summary>
	/// 获取泛型List元素的Type
	/// 例：传入typeof(List<int>)返回typeof(int)
	/// </summary>
	/// <param name="genericListType">泛型List的Type</param>
	public static Type GetGenericListElementType(Type genericListType)
	{
		if (genericListType != null)
		{
			string typeName = genericListType.ToString();
			if (typeName.StartsWith("System.Collections.Generic.List`1"))
			{

				Regex regex = new Regex(ListElementPattern, RegexOptions.IgnoreCase);
				Match match = regex.Match(typeName);
				if (!string.IsNullOrEmpty(match.Value))
				{
					return Type.GetType(match.Value.Trim('[', ']'));
				}
			}
		}

		return null;
	}
}