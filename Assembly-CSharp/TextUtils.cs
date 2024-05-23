using System;
using System.Collections.Generic;
using System.Text;

// Token: 0x020001F3 RID: 499
public static class TextUtils
{
	// Token: 0x06001E0E RID: 7694 RVA: 0x0008BC7D File Offset: 0x00089E7D
	public static string DecodeWhitespaces(string text)
	{
		text = text.Replace("\\n", "\n");
		text = text.Replace("\\t", "\t");
		return text;
	}

	// Token: 0x06001E0F RID: 7695 RVA: 0x0008BCA4 File Offset: 0x00089EA4
	public static string EncodeWhitespaces(string text)
	{
		text = text.Replace("\n", "\\n");
		text = text.Replace("\t", "\\t");
		return text;
	}

	// Token: 0x06001E10 RID: 7696 RVA: 0x0008BCCC File Offset: 0x00089ECC
	public static string ComposeLineItemString(List<string> lines)
	{
		if (lines.Count == 0)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string text in lines)
		{
			stringBuilder.AppendLine(text);
		}
		stringBuilder.Remove(stringBuilder.Length - 1, 1);
		return stringBuilder.ToString();
	}

	// Token: 0x06001E11 RID: 7697 RVA: 0x0008BD50 File Offset: 0x00089F50
	public static int CountCharInString(string s, char c)
	{
		int num = 0;
		for (int i = 0; i < s.Length; i++)
		{
			if (s.get_Chars(i) == c)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06001E12 RID: 7698 RVA: 0x0008BD88 File Offset: 0x00089F88
	public static string Slice(this string str, int start, int end)
	{
		int length = str.Length;
		if (start < 0)
		{
			start = length + start;
		}
		if (end < 0)
		{
			end = length + end;
		}
		int num = end - start;
		if (num <= 0)
		{
			return string.Empty;
		}
		int num2 = length - start;
		if (num > num2)
		{
			num = num2;
		}
		return str.Substring(start, num);
	}

	// Token: 0x06001E13 RID: 7699 RVA: 0x0008BDDC File Offset: 0x00089FDC
	public static string Slice(this string str, int start)
	{
		return str.Slice(start, str.Length);
	}

	// Token: 0x06001E14 RID: 7700 RVA: 0x0008BDEB File Offset: 0x00089FEB
	public static string Slice<T>(this string str)
	{
		return str.Slice(0, str.Length);
	}

	// Token: 0x06001E15 RID: 7701 RVA: 0x0008BDFC File Offset: 0x00089FFC
	public static string TransformCardText(Entity entity, GAME_TAG textTag)
	{
		int damageBonus = entity.GetDamageBonus();
		int damageBonusDouble = entity.GetDamageBonusDouble();
		int healingDouble = entity.GetHealingDouble();
		return TextUtils.TransformCardText(damageBonus, damageBonusDouble, healingDouble, entity.GetStringTag(textTag));
	}

	// Token: 0x06001E16 RID: 7702 RVA: 0x0008BE2D File Offset: 0x0008A02D
	public static string TransformCardText(EntityDef entityDef, GAME_TAG textTag)
	{
		return TextUtils.TransformCardText(0, 0, 0, entityDef.GetStringTag(textTag));
	}

	// Token: 0x06001E17 RID: 7703 RVA: 0x0008BE40 File Offset: 0x0008A040
	public static string TransformCardText(int damageBonus, int damageBonusDouble, int healingDouble, string powersText)
	{
		string str = TextUtils.TransformCardTextImpl(damageBonus, damageBonusDouble, healingDouble, powersText);
		return GameStrings.ParseLanguageRules(str);
	}

	// Token: 0x06001E18 RID: 7704 RVA: 0x0008BE60 File Offset: 0x0008A060
	public static string ToHexString(this byte[] bytes)
	{
		char[] array = new char[bytes.Length * 2];
		for (int i = 0; i < bytes.Length; i++)
		{
			int num = bytes[i] >> 4;
			array[i * 2] = (char)(55 + num + (num - 10 >> 31 & -7));
			num = (int)(bytes[i] & 15);
			array[i * 2 + 1] = (char)(55 + num + (num - 10 >> 31 & -7));
		}
		return new string(array);
	}

	// Token: 0x06001E19 RID: 7705 RVA: 0x0008BECB File Offset: 0x0008A0CB
	public static string ToHexString(string str)
	{
		return Encoding.UTF8.GetBytes(str).ToHexString();
	}

	// Token: 0x06001E1A RID: 7706 RVA: 0x0008BEE0 File Offset: 0x0008A0E0
	public static string FromHexString(string str)
	{
		if (str.Length % 2 == 1)
		{
			throw new Exception("Hex string must have an even number of digits");
		}
		byte[] array = new byte[str.Length >> 1];
		for (int i = 0; i < str.Length >> 1; i++)
		{
			array[i] = (byte)((TextUtils.GetHexValue(str.get_Chars(i << 1)) << 4) + TextUtils.GetHexValue(str.get_Chars((i << 1) + 1)));
		}
		return Encoding.UTF8.GetString(array);
	}

	// Token: 0x06001E1B RID: 7707 RVA: 0x0008BF60 File Offset: 0x0008A160
	private static int GetHexValue(char hex)
	{
		return (int)(hex - ((hex >= ':') ? '7' : '0'));
	}

	// Token: 0x06001E1C RID: 7708 RVA: 0x0008BF84 File Offset: 0x0008A184
	public static bool HasBonusDamage(string powersText)
	{
		if (powersText == null)
		{
			return false;
		}
		for (int i = 0; i < powersText.Length; i++)
		{
			char c = powersText.get_Chars(i);
			if (c == '$')
			{
				int j;
				i = (j = i + 1);
				while (j < powersText.Length)
				{
					char c2 = powersText.get_Chars(j);
					if (c2 < '0' || c2 > '9')
					{
						break;
					}
					j++;
				}
				if (j != i)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06001E1D RID: 7709 RVA: 0x0008C00C File Offset: 0x0008A20C
	private static string TransformCardTextImpl(int damageBonus, int damageBonusDouble, int healingDouble, string powersText)
	{
		if (powersText == null)
		{
			return string.Empty;
		}
		if (powersText == string.Empty)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = damageBonus > 0 || damageBonusDouble > 0;
		bool flag2 = healingDouble > 0;
		for (int i = 0; i < powersText.Length; i++)
		{
			char c = powersText.get_Chars(i);
			if (c != '$' && c != '#')
			{
				stringBuilder.Append(c);
			}
			else
			{
				int j;
				i = (j = i + 1);
				while (j < powersText.Length)
				{
					char c2 = powersText.get_Chars(j);
					if (c2 < '0' || c2 > '9')
					{
						break;
					}
					j++;
				}
				if (j != i)
				{
					string text = powersText.Substring(i, j - i);
					int num = Convert.ToInt32(text);
					if (c == '$')
					{
						num += damageBonus;
						for (int k = 0; k < damageBonusDouble; k++)
						{
							num *= 2;
						}
					}
					else if (c == '#')
					{
						for (int l = 0; l < healingDouble; l++)
						{
							num *= 2;
						}
					}
					if ((flag && c == '$') || (flag2 && c == '#'))
					{
						stringBuilder.Append('*');
						stringBuilder.Append(num);
						stringBuilder.Append('*');
					}
					else
					{
						stringBuilder.Append(num);
					}
					i = j - 1;
				}
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x040010C7 RID: 4295
	private const int DEFAULT_STRING_BUILDER_CAPACITY_FUDGE = 10;
}
