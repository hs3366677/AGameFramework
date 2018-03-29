using System;

/// <summary>
/// 数据存储类中标识字段的类型
/// </summary>
public enum DataFieldType
{
	None = 0,

	/// <summary>
	/// 要导入的字段
	/// </summary>
	Read,
	
	/// <summary>
	/// 不需要导入的字段
	/// </summary>
	NotRead,
}


/// <summary>
/// 对数据存储类中的字段进行标记的Attribute
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class DataFieldAttribute : Attribute
{
	public readonly DataFieldType Type;
	
	private string fieldName;

	/// <summary>
	/// CSV文件中的字段(列)名
	/// 如果FieldName为null或Empty，使用字段的名字和csv的列做关联
	/// </summary>
	public string FieldName
	{
		set
		{
			fieldName = string.IsNullOrEmpty(value) ? null : value;
		}
		get { return fieldName; }
	}

	public DataFieldAttribute(DataFieldType op)
	{
		Type = op;
	}
}

/// <summary>
/// DB类中字段的形式
/// </summary>
public enum DbFieldType
{
	None,

	/// <summary>
	/// 数组形式的字段
	/// </summary>
	Array,

	/// <summary>
	/// 以List形式存在的字段，要对List的元素进行导入操作
	/// </summary>
	List,
}

/// <summary>
/// 用来标记一个DB类中的字段
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class DbFieldAttribute : Attribute
{
	public DbFieldType FieldType
	{
		get;
		private set;
	}

	public DbFieldAttribute(DbFieldType fieldType)
	{
		FieldType = fieldType;
	}
}