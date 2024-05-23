using System;
using System.ComponentModel;
using System.Reflection;

// Token: 0x02000022 RID: 34
public class EnumUtils
{
	// Token: 0x06000305 RID: 773 RVA: 0x0000E888 File Offset: 0x0000CA88
	public static string GetString<T>(T enumVal)
	{
		string text = enumVal.ToString();
		FieldInfo field = enumVal.GetType().GetField(text);
		DescriptionAttribute[] array = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
		if (array.Length > 0)
		{
			return array[0].Description;
		}
		return text;
	}

	// Token: 0x06000306 RID: 774 RVA: 0x0000E8E4 File Offset: 0x0000CAE4
	public static bool TryGetEnum<T>(string str, StringComparison comparisonType, out T result)
	{
		Type typeFromHandle = typeof(T);
		Map<string, object> map;
		EnumUtils.s_enumCache.TryGetValue(typeFromHandle, out map);
		object obj;
		if (map != null && map.TryGetValue(str, out obj))
		{
			result = (T)((object)obj);
			return true;
		}
		foreach (object obj2 in Enum.GetValues(typeFromHandle))
		{
			T t = (T)((object)obj2);
			bool flag = false;
			string @string = EnumUtils.GetString<T>(t);
			if (@string.Equals(str, comparisonType))
			{
				flag = true;
				result = t;
			}
			else
			{
				FieldInfo field = t.GetType().GetField(t.ToString());
				DescriptionAttribute[] array = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Description.Equals(str, comparisonType))
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				if (map == null)
				{
					map = new Map<string, object>();
					EnumUtils.s_enumCache.Add(typeFromHandle, map);
				}
				if (!map.ContainsKey(str))
				{
					map.Add(str, t);
				}
				result = t;
				return true;
			}
		}
		result = default(T);
		return false;
	}

	// Token: 0x06000307 RID: 775 RVA: 0x0000EA84 File Offset: 0x0000CC84
	public static T GetEnum<T>(string str)
	{
		return EnumUtils.GetEnum<T>(str, 4);
	}

	// Token: 0x06000308 RID: 776 RVA: 0x0000EA90 File Offset: 0x0000CC90
	public static T GetEnum<T>(string str, StringComparison comparisonType)
	{
		T result;
		if (EnumUtils.TryGetEnum<T>(str, comparisonType, out result))
		{
			return result;
		}
		string text = string.Format("EnumUtils.GetEnum() - \"{0}\" has no matching value in enum {1}", str, typeof(T));
		throw new ArgumentException(text);
	}

	// Token: 0x06000309 RID: 777 RVA: 0x0000EAC9 File Offset: 0x0000CCC9
	public static bool TryGetEnum<T>(string str, out T outVal)
	{
		return EnumUtils.TryGetEnum<T>(str, 4, out outVal);
	}

	// Token: 0x0600030A RID: 778 RVA: 0x0000EAD3 File Offset: 0x0000CCD3
	public static T Parse<T>(string str)
	{
		return (T)((object)Enum.Parse(typeof(T), str));
	}

	// Token: 0x0600030B RID: 779 RVA: 0x0000EAEC File Offset: 0x0000CCEC
	public static T SafeParse<T>(string str)
	{
		T result;
		try
		{
			result = (T)((object)Enum.Parse(typeof(T), str));
		}
		catch (Exception)
		{
			result = default(T);
		}
		return result;
	}

	// Token: 0x0600030C RID: 780 RVA: 0x0000EB40 File Offset: 0x0000CD40
	public static bool TryCast<T>(object inVal, out T outVal)
	{
		outVal = default(T);
		bool result;
		try
		{
			outVal = (T)((object)inVal);
			result = true;
		}
		catch (Exception)
		{
			result = false;
		}
		return result;
	}

	// Token: 0x0600030D RID: 781 RVA: 0x0000EB94 File Offset: 0x0000CD94
	public static int Length<T>()
	{
		return Enum.GetValues(typeof(T)).Length;
	}

	// Token: 0x04000142 RID: 322
	private static Map<Type, Map<string, object>> s_enumCache = new Map<Type, Map<string, object>>();
}
