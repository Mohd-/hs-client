using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

// Token: 0x0200020E RID: 526
public static class StringUtils
{
	// Token: 0x06002037 RID: 8247 RVA: 0x0009DD9E File Offset: 0x0009BF9E
	public static string StripNonNumbers(string str)
	{
		return Regex.Replace(str, "[^0-9]", string.Empty);
	}

	// Token: 0x06002038 RID: 8248 RVA: 0x0009DDB0 File Offset: 0x0009BFB0
	public static string StripNewlines(string str)
	{
		return Regex.Replace(str, "[\\r\\n]", string.Empty);
	}

	// Token: 0x06002039 RID: 8249 RVA: 0x0009DDC2 File Offset: 0x0009BFC2
	public static string[] SplitLines(string str)
	{
		return str.Split(StringUtils.SPLIT_LINES_CHARS_ARRAY, 1);
	}

	// Token: 0x0600203A RID: 8250 RVA: 0x0009DDD0 File Offset: 0x0009BFD0
	public static bool CompareIgnoreCase(string a, string b)
	{
		return string.Compare(a, b, 5) == 0;
	}

	// Token: 0x0600203B RID: 8251 RVA: 0x0009DDE0 File Offset: 0x0009BFE0
	public static string ArrayToString(IEnumerable l)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		foreach (object obj in l)
		{
			stringBuilder.Append(obj);
			stringBuilder.Append(", ");
		}
		stringBuilder.Remove(stringBuilder.Length - 2, 2);
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}

	// Token: 0x0600203C RID: 8252 RVA: 0x0009DE7C File Offset: 0x0009C07C
	public static bool Contains(this string str, string val, StringComparison comparison)
	{
		return str.IndexOf(val, comparison) >= 0;
	}

	// Token: 0x0600203D RID: 8253 RVA: 0x0009DE8C File Offset: 0x0009C08C
	public static bool Contains(this string s, char c)
	{
		return s.IndexOf(c) >= 0;
	}

	// Token: 0x040011A7 RID: 4519
	public const string SPLIT_LINES_CHARS = "\n\r";

	// Token: 0x040011A8 RID: 4520
	public const string REGEX_RESERVED_CHARS = "\\*.+?^$()[]{}";

	// Token: 0x040011A9 RID: 4521
	public static readonly char[] SPLIT_LINES_CHARS_ARRAY = "\n\r".ToCharArray();

	// Token: 0x040011AA RID: 4522
	public static readonly char[] REGEX_RESERVED_CHARS_ARRAY = "\\*.+?^$()[]{}".ToCharArray();
}
