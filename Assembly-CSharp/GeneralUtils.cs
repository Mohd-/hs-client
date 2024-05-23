using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEngine;

// Token: 0x0200011E RID: 286
public static class GeneralUtils
{
	// Token: 0x06000DB7 RID: 3511 RVA: 0x00037AA4 File Offset: 0x00035CA4
	public static void Swap<T>(ref T a, ref T b)
	{
		T t = a;
		a = b;
		b = t;
	}

	// Token: 0x06000DB8 RID: 3512 RVA: 0x00037ACC File Offset: 0x00035CCC
	public static void ListSwap<T>(IList<T> list, int indexA, int indexB)
	{
		T t = list[indexA];
		list[indexA] = list[indexB];
		list[indexB] = t;
	}

	// Token: 0x06000DB9 RID: 3513 RVA: 0x00037AF8 File Offset: 0x00035CF8
	public static void ListMove<T>(IList<T> list, int srcIndex, int dstIndex)
	{
		if (srcIndex == dstIndex)
		{
			return;
		}
		T t = list[srcIndex];
		list.RemoveAt(srcIndex);
		if (dstIndex > srcIndex)
		{
			dstIndex--;
		}
		list.Insert(dstIndex, t);
	}

	// Token: 0x06000DBA RID: 3514 RVA: 0x00037B30 File Offset: 0x00035D30
	public static T[] Combine<T>(T[] arr1, T[] arr2)
	{
		T[] array = new T[arr1.Length + arr2.Length];
		Array.Copy(arr1, 0, array, 0, arr1.Length);
		Array.Copy(arr2, 0, array, arr1.Length, arr2.Length);
		return array;
	}

	// Token: 0x06000DBB RID: 3515 RVA: 0x00037B68 File Offset: 0x00035D68
	public static void Shuffle<T>(T[] arr)
	{
		for (int i = 0; i < arr.Length - 1; i++)
		{
			int num = Random.Range(0, arr.Length - i);
			T t = arr[i];
			arr[i] = arr[i + num];
			arr[i + num] = t;
		}
	}

	// Token: 0x06000DBC RID: 3516 RVA: 0x00037BBC File Offset: 0x00035DBC
	public static T[] Slice<T>(this T[] arr, int start, int end)
	{
		int num = arr.Length;
		if (start < 0)
		{
			start = num + start;
		}
		if (end < 0)
		{
			end = num + end;
		}
		int num2 = end - start;
		if (num2 <= 0)
		{
			return new T[0];
		}
		int num3 = num - start;
		if (num2 > num3)
		{
			num2 = num3;
		}
		T[] array = new T[num2];
		Array.Copy(arr, start, array, 0, num2);
		return array;
	}

	// Token: 0x06000DBD RID: 3517 RVA: 0x00037C16 File Offset: 0x00035E16
	public static T[] Slice<T>(this T[] arr, int start)
	{
		return arr.Slice(start, arr.Length);
	}

	// Token: 0x06000DBE RID: 3518 RVA: 0x00037C22 File Offset: 0x00035E22
	public static T[] Slice<T>(this T[] arr)
	{
		return arr.Slice(0, arr.Length);
	}

	// Token: 0x06000DBF RID: 3519 RVA: 0x00037C30 File Offset: 0x00035E30
	public static bool IsOverriddenMethod(MethodInfo childMethod, MethodInfo ancestorMethod)
	{
		if (childMethod == null)
		{
			return false;
		}
		if (ancestorMethod == null)
		{
			return false;
		}
		if (childMethod.Equals(ancestorMethod))
		{
			return false;
		}
		MethodInfo baseDefinition = childMethod.GetBaseDefinition();
		while (!baseDefinition.Equals(childMethod) && !baseDefinition.Equals(ancestorMethod))
		{
			MethodInfo methodInfo = baseDefinition;
			baseDefinition = baseDefinition.GetBaseDefinition();
			if (baseDefinition.Equals(methodInfo))
			{
				return false;
			}
		}
		return baseDefinition.Equals(ancestorMethod);
	}

	// Token: 0x06000DC0 RID: 3520 RVA: 0x00037CA0 File Offset: 0x00035EA0
	public static bool IsObjectAlive(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (!(obj is Object))
		{
			return true;
		}
		Object @object = (Object)obj;
		return @object;
	}

	// Token: 0x06000DC1 RID: 3521 RVA: 0x00037CD0 File Offset: 0x00035ED0
	public static bool IsCallbackValid(Delegate callback)
	{
		bool flag = true;
		if (callback == null)
		{
			flag = false;
		}
		else if (!callback.Method.IsStatic)
		{
			object target = callback.Target;
			flag = GeneralUtils.IsObjectAlive(target);
			if (!flag)
			{
				Debug.LogError(string.Format("Target for callback {0} is null.", callback.Method.Name));
			}
		}
		return flag;
	}

	// Token: 0x06000DC2 RID: 3522 RVA: 0x00037D2B File Offset: 0x00035F2B
	public static bool IsEditorPlaying()
	{
		return false;
	}

	// Token: 0x06000DC3 RID: 3523 RVA: 0x00037D2E File Offset: 0x00035F2E
	public static void ExitApplication()
	{
		Application.Quit();
	}

	// Token: 0x06000DC4 RID: 3524 RVA: 0x00037D35 File Offset: 0x00035F35
	public static bool IsDevelopmentBuildTextVisible()
	{
		return Debug.isDebugBuild;
	}

	// Token: 0x06000DC5 RID: 3525 RVA: 0x00037D3C File Offset: 0x00035F3C
	public static bool TryParseBool(string strVal, out bool boolVal)
	{
		if (bool.TryParse(strVal, ref boolVal))
		{
			return true;
		}
		string text = strVal.ToLowerInvariant().Trim();
		if (text == "off" || text == "0" || text == "false")
		{
			boolVal = false;
			return true;
		}
		if (text == "on" || text == "1" || text == "true")
		{
			boolVal = true;
			return true;
		}
		boolVal = false;
		return false;
	}

	// Token: 0x06000DC6 RID: 3526 RVA: 0x00037DD4 File Offset: 0x00035FD4
	public static bool ForceBool(string strVal)
	{
		if (string.IsNullOrEmpty(strVal))
		{
			return false;
		}
		string text = strVal.ToLowerInvariant().Trim();
		return text == "on" || text == "1" || text == "true";
	}

	// Token: 0x06000DC7 RID: 3527 RVA: 0x00037E2D File Offset: 0x0003602D
	public static bool TryParseInt(string str, out int val)
	{
		return int.TryParse(str, 511, null, ref val);
	}

	// Token: 0x06000DC8 RID: 3528 RVA: 0x00037E3C File Offset: 0x0003603C
	public static int ForceInt(string str)
	{
		int result = 0;
		GeneralUtils.TryParseInt(str, out result);
		return result;
	}

	// Token: 0x06000DC9 RID: 3529 RVA: 0x00037E55 File Offset: 0x00036055
	public static bool TryParseLong(string str, out long val)
	{
		return long.TryParse(str, 511, null, ref val);
	}

	// Token: 0x06000DCA RID: 3530 RVA: 0x00037E64 File Offset: 0x00036064
	public static long ForceLong(string str)
	{
		long result = 0L;
		GeneralUtils.TryParseLong(str, out result);
		return result;
	}

	// Token: 0x06000DCB RID: 3531 RVA: 0x00037E7E File Offset: 0x0003607E
	public static bool TryParseULong(string str, out ulong val)
	{
		return ulong.TryParse(str, 511, null, ref val);
	}

	// Token: 0x06000DCC RID: 3532 RVA: 0x00037E90 File Offset: 0x00036090
	public static ulong ForceULong(string str)
	{
		ulong result = 0UL;
		GeneralUtils.TryParseULong(str, out result);
		return result;
	}

	// Token: 0x06000DCD RID: 3533 RVA: 0x00037EAA File Offset: 0x000360AA
	public static bool TryParseFloat(string str, out float val)
	{
		return float.TryParse(str, 511, null, ref val);
	}

	// Token: 0x06000DCE RID: 3534 RVA: 0x00037EBC File Offset: 0x000360BC
	public static float ForceFloat(string str)
	{
		float result = 0f;
		GeneralUtils.TryParseFloat(str, out result);
		return result;
	}

	// Token: 0x06000DCF RID: 3535 RVA: 0x00037ED9 File Offset: 0x000360D9
	public static bool RandomBool()
	{
		return Random.Range(0, 2) == 0;
	}

	// Token: 0x06000DD0 RID: 3536 RVA: 0x00037EE5 File Offset: 0x000360E5
	public static float RandomSign()
	{
		return (!GeneralUtils.RandomBool()) ? 1f : -1f;
	}

	// Token: 0x06000DD1 RID: 3537 RVA: 0x00037F00 File Offset: 0x00036100
	public static int UnsignedMod(int x, int y)
	{
		int num = x % y;
		if (num < 0)
		{
			num += y;
		}
		return num;
	}

	// Token: 0x06000DD2 RID: 3538 RVA: 0x00037F1D File Offset: 0x0003611D
	public static bool IsEven(int n)
	{
		return (n & 1) == 0;
	}

	// Token: 0x06000DD3 RID: 3539 RVA: 0x00037F25 File Offset: 0x00036125
	public static bool IsOdd(int n)
	{
		return (n & 1) == 1;
	}

	// Token: 0x06000DD4 RID: 3540 RVA: 0x00037F30 File Offset: 0x00036130
	public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> func)
	{
		if (enumerable == null)
		{
			return;
		}
		foreach (T t in enumerable)
		{
			func.Invoke(t);
		}
	}

	// Token: 0x06000DD5 RID: 3541 RVA: 0x00037F8C File Offset: 0x0003618C
	public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> func)
	{
		if (enumerable == null)
		{
			return;
		}
		int num = 0;
		foreach (T t in enumerable)
		{
			func.Invoke(t, num);
			num++;
		}
	}

	// Token: 0x06000DD6 RID: 3542 RVA: 0x00037FEC File Offset: 0x000361EC
	public static void ForEachReassign<T>(this T[] array, Func<T, T> func)
	{
		if (array == null)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = func.Invoke(array[i]);
		}
	}

	// Token: 0x06000DD7 RID: 3543 RVA: 0x00038028 File Offset: 0x00036228
	public static void ForEachReassign<T>(this T[] array, Func<T, int, T> func)
	{
		if (array == null)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = func.Invoke(array[i], i);
		}
	}

	// Token: 0x06000DD8 RID: 3544 RVA: 0x00038068 File Offset: 0x00036268
	public static bool AreArraysEqual<T>(T[] arr1, T[] arr2)
	{
		if (arr1 == arr2)
		{
			return true;
		}
		if (arr1 == null)
		{
			return false;
		}
		if (arr2 == null)
		{
			return false;
		}
		if (arr1.Length != arr2.Length)
		{
			return false;
		}
		for (int i = 0; i < arr1.Length; i++)
		{
			if (!arr1[i].Equals(arr2[i]))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000DD9 RID: 3545 RVA: 0x000380D5 File Offset: 0x000362D5
	public static bool AreBytesEqual(byte[] bytes1, byte[] bytes2)
	{
		return GeneralUtils.AreArraysEqual<byte>(bytes1, bytes2);
	}

	// Token: 0x06000DDA RID: 3546 RVA: 0x000380DE File Offset: 0x000362DE
	public static T DeepClone<T>(T obj)
	{
		return (T)((object)GeneralUtils.CloneValue(obj, obj.GetType()));
	}

	// Token: 0x06000DDB RID: 3547 RVA: 0x000380FC File Offset: 0x000362FC
	private static object CloneClass(object obj, Type objType)
	{
		object obj2 = GeneralUtils.CreateNewType(objType);
		FieldInfo[] fields = objType.GetFields(52);
		foreach (FieldInfo fieldInfo in fields)
		{
			fieldInfo.SetValue(obj2, GeneralUtils.CloneValue(fieldInfo.GetValue(obj), fieldInfo.FieldType));
		}
		return obj2;
	}

	// Token: 0x06000DDC RID: 3548 RVA: 0x00038154 File Offset: 0x00036354
	private static object CloneValue(object src, Type type)
	{
		if (src != null && type != typeof(string) && type.IsClass)
		{
			if (!type.IsGenericType)
			{
				return GeneralUtils.CloneClass(src, type);
			}
			if (src is IDictionary)
			{
				IDictionary dictionary = src as IDictionary;
				IDictionary dictionary2 = GeneralUtils.CreateNewType(type) as IDictionary;
				Type type2 = type.GetGenericArguments()[0];
				Type type3 = type.GetGenericArguments()[1];
				foreach (object obj in dictionary)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					dictionary2.Add(GeneralUtils.CloneValue(dictionaryEntry.Key, type2), GeneralUtils.CloneValue(dictionaryEntry.Value, type3));
				}
				return dictionary2;
			}
			if (src is IList)
			{
				IList list = src as IList;
				IList list2 = GeneralUtils.CreateNewType(type) as IList;
				Type type4 = type.GetGenericArguments()[0];
				foreach (object src2 in list)
				{
					list2.Add(GeneralUtils.CloneValue(src2, type4));
				}
				return list2;
			}
		}
		return src;
	}

	// Token: 0x06000DDD RID: 3549 RVA: 0x000382C8 File Offset: 0x000364C8
	private static object CreateNewType(Type type)
	{
		object obj = Activator.CreateInstance(type);
		if (obj == null)
		{
			throw new SystemException(string.Format("Unable to instantiate type {0} with default constructor.", type.Name));
		}
		return obj;
	}

	// Token: 0x06000DDE RID: 3550 RVA: 0x000382FC File Offset: 0x000364FC
	public static void DeepReset<T>(T obj)
	{
		Type typeFromHandle = typeof(T);
		T t = Activator.CreateInstance<T>();
		if (t == null)
		{
			throw new SystemException(string.Format("Unable to instantiate type {0} with default constructor.", typeFromHandle.Name));
		}
		FieldInfo[] fields = typeFromHandle.GetFields(60);
		foreach (FieldInfo fieldInfo in fields)
		{
			fieldInfo.SetValue(obj, fieldInfo.GetValue(t));
		}
	}

	// Token: 0x06000DDF RID: 3551 RVA: 0x00038380 File Offset: 0x00036580
	public static void CleanNullObjectsFromList<T>(List<T> list)
	{
		int i = 0;
		while (i < list.Count)
		{
			T t = list[i];
			if (t == null)
			{
				list.RemoveAt(i);
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x06000DE0 RID: 3552 RVA: 0x000383C4 File Offset: 0x000365C4
	public static void CleanDeadObjectsFromList<T>(List<T> list) where T : Component
	{
		int i = 0;
		while (i < list.Count)
		{
			T t = list[i];
			if (t)
			{
				i++;
			}
			else
			{
				list.RemoveAt(i);
			}
		}
	}

	// Token: 0x06000DE1 RID: 3553 RVA: 0x0003840C File Offset: 0x0003660C
	public static void CleanDeadObjectsFromList(List<GameObject> list)
	{
		int i = 0;
		while (i < list.Count)
		{
			GameObject gameObject = list[i];
			if (gameObject)
			{
				i++;
			}
			else
			{
				list.RemoveAt(i);
			}
		}
	}

	// Token: 0x06000DE2 RID: 3554 RVA: 0x00038450 File Offset: 0x00036650
	public static string SafeFormat(string format, params object[] args)
	{
		string result;
		if (args.Length == 0)
		{
			result = format;
		}
		else
		{
			result = string.Format(format, args);
		}
		return result;
	}

	// Token: 0x06000DE3 RID: 3555 RVA: 0x00038478 File Offset: 0x00036678
	public static string GetPatchDir()
	{
		string text = Directory.GetCurrentDirectory();
		text = text.Substring(0, text.LastIndexOf(Path.DirectorySeparatorChar));
		return text.Substring(0, text.LastIndexOf(Path.DirectorySeparatorChar));
	}

	// Token: 0x06000DE4 RID: 3556 RVA: 0x000384B4 File Offset: 0x000366B4
	public static Process RunPegasusCommonScriptWithParams(string scriptName, params string[] scriptParams)
	{
		Process result;
		try
		{
			string patchDir = GeneralUtils.GetPatchDir();
			string text = string.Join(" ", scriptParams);
			string text2 = "bat";
			string text3 = Path.Combine(patchDir, "Pegasus");
			text3 = Path.Combine(text3, "Common");
			text3 = Path.Combine(text3, string.Format("{0}.{1}", scriptName, text2));
			Debug.LogFormat("Running command: {0} {1}", new object[]
			{
				text3,
				text
			});
			Process process = new Process
			{
				StartInfo = 
				{
					FileName = text3,
					Arguments = text
				}
			};
			process.Start();
			result = process;
		}
		catch (Exception ex)
		{
			Blizzard.Log.Error("Failed to run {0}: {1}", new object[]
			{
				scriptName,
				ex.Message
			});
			result = null;
		}
		return result;
	}

	// Token: 0x06000DE5 RID: 3557 RVA: 0x00038598 File Offset: 0x00036798
	public static bool CompleteProcess(Process proc)
	{
		proc.WaitForExit();
		return GeneralUtils.LogCompletedProcess(proc);
	}

	// Token: 0x06000DE6 RID: 3558 RVA: 0x000385A8 File Offset: 0x000367A8
	public static bool CompleteProcess(Process proc, int millisecondTimout)
	{
		if (!proc.WaitForExit(millisecondTimout))
		{
			Debug.LogError(string.Concat(new object[]
			{
				Path.GetFileNameWithoutExtension(proc.StartInfo.FileName),
				" timed out after ",
				millisecondTimout,
				"milliseconds"
			}));
			return false;
		}
		return GeneralUtils.LogCompletedProcess(proc);
	}

	// Token: 0x06000DE7 RID: 3559 RVA: 0x00038605 File Offset: 0x00036805
	public static bool LogCompletedProcess(Process proc)
	{
		return proc.ExitCode == 0;
	}

	// Token: 0x06000DE8 RID: 3560 RVA: 0x00038610 File Offset: 0x00036810
	public static ulong DateTimeToUnixTimeStamp(DateTime time)
	{
		DateTime dateTime;
		dateTime..ctor(1970, 1, 1, 0, 0, 0, 0, 1);
		return (ulong)(time.ToUniversalTime() - dateTime).TotalSeconds;
	}

	// Token: 0x06000DE9 RID: 3561 RVA: 0x00038648 File Offset: 0x00036848
	public static DateTime UnixTimeStampToDateTime(ulong secondsSinceEpoch)
	{
		DateTime dateTime;
		dateTime..ctor(1970, 1, 1, 0, 0, 0, 0, 1);
		return dateTime.AddSeconds(secondsSinceEpoch);
	}

	// Token: 0x04000745 RID: 1861
	public const float DEVELOPMENT_BUILD_TEXT_WIDTH = 115f;
}
