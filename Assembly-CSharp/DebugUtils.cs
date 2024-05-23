using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

// Token: 0x020001B7 RID: 439
public class DebugUtils
{
	// Token: 0x06001CF4 RID: 7412 RVA: 0x00088076 File Offset: 0x00086276
	[Conditional("UNITY_EDITOR")]
	public static void Assert(bool test)
	{
		if (!test)
		{
			Debug.Break();
		}
	}

	// Token: 0x06001CF5 RID: 7413 RVA: 0x00088083 File Offset: 0x00086283
	[Conditional("UNITY_EDITOR")]
	public static void Assert(bool test, string message)
	{
		if (!test)
		{
			Debug.LogWarning(message);
			Debug.Break();
		}
	}

	// Token: 0x06001CF6 RID: 7414 RVA: 0x00088098 File Offset: 0x00086298
	public static string HashtableToString(Hashtable table)
	{
		string text = string.Empty;
		foreach (object obj in table)
		{
			DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				dictionaryEntry.Key,
				" = ",
				dictionaryEntry.Value,
				"\n"
			});
		}
		return text;
	}

	// Token: 0x06001CF7 RID: 7415 RVA: 0x00088130 File Offset: 0x00086330
	public static int CountParents(GameObject go)
	{
		int num = 0;
		if (go != null)
		{
			Transform parent = go.transform.parent;
			while (parent != null)
			{
				num++;
				parent = parent.transform.parent;
			}
		}
		return num;
	}

	// Token: 0x06001CF8 RID: 7416 RVA: 0x0008817C File Offset: 0x0008637C
	public static string GetHierarchyPath(Object obj, char separator = '.')
	{
		StringBuilder stringBuilder = new StringBuilder();
		DebugUtils.GetHierarchyPath_Internal(stringBuilder, obj, separator);
		return stringBuilder.ToString();
	}

	// Token: 0x06001CF9 RID: 7417 RVA: 0x000881A0 File Offset: 0x000863A0
	public static string GetHierarchyPathAndType(Object obj, char separator = '.')
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[Type]=").Append(obj.GetType().FullName).Append(" [Path]=");
		DebugUtils.GetHierarchyPath_Internal(stringBuilder, obj, separator);
		return stringBuilder.ToString();
	}

	// Token: 0x06001CFA RID: 7418 RVA: 0x000881E8 File Offset: 0x000863E8
	private static bool GetHierarchyPath_Internal(StringBuilder b, Object obj, char separator)
	{
		if (obj == null)
		{
			return false;
		}
		Transform transform = (!(obj is GameObject)) ? ((!(obj is Component)) ? null : ((Component)obj).transform) : ((GameObject)obj).transform;
		List<string> list = new List<string>();
		while (transform != null)
		{
			list.Insert(0, transform.gameObject.name);
			transform = transform.parent;
		}
		if (list.Count > 0 && separator == '/')
		{
			b.Append(separator);
		}
		for (int i = 0; i < list.Count; i++)
		{
			b.Append(list[i]);
			if (i < list.Count - 1)
			{
				b.Append(separator);
			}
		}
		return true;
	}
}
