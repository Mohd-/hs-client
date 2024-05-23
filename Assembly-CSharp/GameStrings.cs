using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

// Token: 0x02000099 RID: 153
public class GameStrings
{
	// Token: 0x06000719 RID: 1817 RVA: 0x0001BC5C File Offset: 0x00019E5C
	public static void LoadAll()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		foreach (object obj in Enum.GetValues(typeof(GameStringCategory)))
		{
			GameStringCategory gameStringCategory = (GameStringCategory)((int)obj);
			if (gameStringCategory != GameStringCategory.INVALID)
			{
				GameStrings.LoadCategory(gameStringCategory);
			}
		}
		float realtimeSinceStartup2 = Time.realtimeSinceStartup;
		Log.Cameron.Print(string.Format("Loading All GameStrings took {0}s)", realtimeSinceStartup2 - realtimeSinceStartup), new object[0]);
	}

	// Token: 0x0600071A RID: 1818 RVA: 0x0001BD04 File Offset: 0x00019F04
	public static void ReloadAll()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		foreach (object obj in Enum.GetValues(typeof(GameStringCategory)))
		{
			GameStringCategory gameStringCategory = (GameStringCategory)((int)obj);
			if (gameStringCategory != GameStringCategory.INVALID)
			{
				if (GameStrings.s_tables.ContainsKey(gameStringCategory))
				{
					GameStrings.UnloadCategory(gameStringCategory);
				}
				GameStrings.LoadCategory(gameStringCategory);
			}
		}
		float realtimeSinceStartup2 = Time.realtimeSinceStartup;
		Log.Cameron.Print(string.Format("Reloading All GameStrings took {0}s)", realtimeSinceStartup2 - realtimeSinceStartup), new object[0]);
	}

	// Token: 0x0600071B RID: 1819 RVA: 0x0001BDC4 File Offset: 0x00019FC4
	public static void WillReset()
	{
		GameStrings.ReloadAll();
	}

	// Token: 0x0600071C RID: 1820 RVA: 0x0001BDCC File Offset: 0x00019FCC
	public static string GetAssetPath(Locale locale, string fileName)
	{
		return FileUtils.GetAssetPath(string.Format("Strings/{0}/{1}", locale, fileName));
	}

	// Token: 0x0600071D RID: 1821 RVA: 0x0001BDF4 File Offset: 0x00019FF4
	public static bool HasKey(string key)
	{
		string text = GameStrings.Find(key);
		return text != null;
	}

	// Token: 0x0600071E RID: 1822 RVA: 0x0001BE10 File Offset: 0x0001A010
	public static string Get(string key)
	{
		string text = GameStrings.Find(key);
		if (text == null)
		{
			return key;
		}
		return GameStrings.ParseLanguageRules(text);
	}

	// Token: 0x0600071F RID: 1823 RVA: 0x0001BE34 File Offset: 0x0001A034
	public static string Format(string key, params object[] args)
	{
		string text = GameStrings.Find(key);
		if (text == null)
		{
			return key;
		}
		text = string.Format(Localization.GetCultureInfo(), text, args);
		return GameStrings.ParseLanguageRules(text);
	}

	// Token: 0x06000720 RID: 1824 RVA: 0x0001BE68 File Offset: 0x0001A068
	public static string FormatPlurals(string key, GameStrings.PluralNumber[] pluralNumbers, params object[] args)
	{
		string text = GameStrings.Find(key);
		if (text == null)
		{
			return key;
		}
		text = string.Format(Localization.GetCultureInfo(), text, args);
		return GameStrings.ParseLanguageRules(text, pluralNumbers);
	}

	// Token: 0x06000721 RID: 1825 RVA: 0x0001BE9A File Offset: 0x0001A09A
	public static string ParseLanguageRules(string str)
	{
		str = GameStrings.ParseLanguageRule4(str, null);
		return str;
	}

	// Token: 0x06000722 RID: 1826 RVA: 0x0001BEA6 File Offset: 0x0001A0A6
	public static string ParseLanguageRules(string str, GameStrings.PluralNumber[] pluralNumbers)
	{
		str = GameStrings.ParseLanguageRule4(str, pluralNumbers);
		return str;
	}

	// Token: 0x06000723 RID: 1827 RVA: 0x0001BEB2 File Offset: 0x0001A0B2
	public static bool HasClassName(TAG_CLASS tag)
	{
		return GameStrings.s_classNames.ContainsKey(tag);
	}

	// Token: 0x06000724 RID: 1828 RVA: 0x0001BEC0 File Offset: 0x0001A0C0
	public static string GetClassName(TAG_CLASS tag)
	{
		string key = null;
		return (!GameStrings.s_classNames.TryGetValue(tag, out key)) ? "UNKNOWN" : GameStrings.Get(key);
	}

	// Token: 0x06000725 RID: 1829 RVA: 0x0001BEF4 File Offset: 0x0001A0F4
	public static string GetClassNameKey(TAG_CLASS tag)
	{
		string text = null;
		return (!GameStrings.s_classNames.TryGetValue(tag, out text)) ? null : text;
	}

	// Token: 0x06000726 RID: 1830 RVA: 0x0001BF1C File Offset: 0x0001A11C
	public static bool HasKeywordName(GAME_TAG tag)
	{
		return GameStrings.s_keywordText.ContainsKey(tag);
	}

	// Token: 0x06000727 RID: 1831 RVA: 0x0001BF2C File Offset: 0x0001A12C
	public static string GetKeywordName(GAME_TAG tag)
	{
		string[] array = null;
		return (!GameStrings.s_keywordText.TryGetValue(tag, out array)) ? "UNKNOWN" : GameStrings.Get(array[0]);
	}

	// Token: 0x06000728 RID: 1832 RVA: 0x0001BF60 File Offset: 0x0001A160
	public static string GetKeywordNameKey(GAME_TAG tag)
	{
		string[] array = null;
		return (!GameStrings.s_keywordText.TryGetValue(tag, out array)) ? null : array[0];
	}

	// Token: 0x06000729 RID: 1833 RVA: 0x0001BF8A File Offset: 0x0001A18A
	public static bool HasKeywordText(GAME_TAG tag)
	{
		return GameStrings.s_keywordText.ContainsKey(tag);
	}

	// Token: 0x0600072A RID: 1834 RVA: 0x0001BF98 File Offset: 0x0001A198
	public static string GetKeywordText(GAME_TAG tag)
	{
		string[] array = null;
		return (!GameStrings.s_keywordText.TryGetValue(tag, out array)) ? "UNKNOWN" : GameStrings.Get(array[1]);
	}

	// Token: 0x0600072B RID: 1835 RVA: 0x0001BFCC File Offset: 0x0001A1CC
	public static string GetKeywordTextKey(GAME_TAG tag)
	{
		string[] array = null;
		return (!GameStrings.s_keywordText.TryGetValue(tag, out array)) ? null : array[1];
	}

	// Token: 0x0600072C RID: 1836 RVA: 0x0001BFF6 File Offset: 0x0001A1F6
	public static bool HasRefKeywordText(GAME_TAG tag)
	{
		return GameStrings.s_refKeywordText.ContainsKey(tag);
	}

	// Token: 0x0600072D RID: 1837 RVA: 0x0001C004 File Offset: 0x0001A204
	public static string GetRefKeywordText(GAME_TAG tag)
	{
		string[] array = null;
		return (!GameStrings.s_refKeywordText.TryGetValue(tag, out array)) ? "UNKNOWN" : GameStrings.Get(array[1]);
	}

	// Token: 0x0600072E RID: 1838 RVA: 0x0001C038 File Offset: 0x0001A238
	public static string GetRefKeywordTextKey(GAME_TAG tag)
	{
		string[] array = null;
		return (!GameStrings.s_refKeywordText.TryGetValue(tag, out array)) ? null : array[1];
	}

	// Token: 0x0600072F RID: 1839 RVA: 0x0001C062 File Offset: 0x0001A262
	public static bool HasRarityText(TAG_RARITY tag)
	{
		return GameStrings.s_rarityNames.ContainsKey(tag);
	}

	// Token: 0x06000730 RID: 1840 RVA: 0x0001C070 File Offset: 0x0001A270
	public static string GetRarityText(TAG_RARITY tag)
	{
		string key = null;
		return (!GameStrings.s_rarityNames.TryGetValue(tag, out key)) ? "UNKNOWN" : GameStrings.Get(key);
	}

	// Token: 0x06000731 RID: 1841 RVA: 0x0001C0A4 File Offset: 0x0001A2A4
	public static string GetRarityTextKey(TAG_RARITY tag)
	{
		string text = null;
		return (!GameStrings.s_rarityNames.TryGetValue(tag, out text)) ? null : text;
	}

	// Token: 0x06000732 RID: 1842 RVA: 0x0001C0CC File Offset: 0x0001A2CC
	public static bool HasRaceName(TAG_RACE tag)
	{
		return GameStrings.s_raceNames.ContainsKey(tag);
	}

	// Token: 0x06000733 RID: 1843 RVA: 0x0001C0DC File Offset: 0x0001A2DC
	public static string GetRaceName(TAG_RACE tag)
	{
		string key = null;
		return (!GameStrings.s_raceNames.TryGetValue(tag, out key)) ? "UNKNOWN" : GameStrings.Get(key);
	}

	// Token: 0x06000734 RID: 1844 RVA: 0x0001C110 File Offset: 0x0001A310
	public static string GetRaceNameKey(TAG_RACE tag)
	{
		string text = null;
		return (!GameStrings.s_raceNames.TryGetValue(tag, out text)) ? null : text;
	}

	// Token: 0x06000735 RID: 1845 RVA: 0x0001C138 File Offset: 0x0001A338
	public static bool HasCardTypeName(TAG_CARDTYPE tag)
	{
		return GameStrings.s_cardTypeNames.ContainsKey(tag);
	}

	// Token: 0x06000736 RID: 1846 RVA: 0x0001C148 File Offset: 0x0001A348
	public static string GetCardTypeName(TAG_CARDTYPE tag)
	{
		string key = null;
		return (!GameStrings.s_cardTypeNames.TryGetValue(tag, out key)) ? "UNKNOWN" : GameStrings.Get(key);
	}

	// Token: 0x06000737 RID: 1847 RVA: 0x0001C17C File Offset: 0x0001A37C
	public static string GetCardTypeNameKey(TAG_CARDTYPE tag)
	{
		string text = null;
		return (!GameStrings.s_cardTypeNames.TryGetValue(tag, out text)) ? null : text;
	}

	// Token: 0x06000738 RID: 1848 RVA: 0x0001C1A4 File Offset: 0x0001A3A4
	public static bool HasCardSetName(TAG_CARD_SET tag)
	{
		return GameStrings.s_cardSetNames.ContainsKey(tag);
	}

	// Token: 0x06000739 RID: 1849 RVA: 0x0001C1B4 File Offset: 0x0001A3B4
	public static string GetCardSetName(TAG_CARD_SET tag)
	{
		string key = null;
		return (!GameStrings.s_cardSetNames.TryGetValue(tag, out key)) ? "UNKNOWN" : GameStrings.Get(key);
	}

	// Token: 0x0600073A RID: 1850 RVA: 0x0001C1E8 File Offset: 0x0001A3E8
	public static string GetCardSetNameKey(TAG_CARD_SET tag)
	{
		string text = null;
		return (!GameStrings.s_cardSetNames.TryGetValue(tag, out text)) ? null : text;
	}

	// Token: 0x0600073B RID: 1851 RVA: 0x0001C210 File Offset: 0x0001A410
	public static bool HasCardSetNameShortened(TAG_CARD_SET tag)
	{
		return GameStrings.s_cardSetNamesShortened.ContainsKey(tag);
	}

	// Token: 0x0600073C RID: 1852 RVA: 0x0001C220 File Offset: 0x0001A420
	public static string GetCardSetNameShortened(TAG_CARD_SET tag)
	{
		string key = null;
		return (!GameStrings.s_cardSetNamesShortened.TryGetValue(tag, out key)) ? "UNKNOWN" : GameStrings.Get(key);
	}

	// Token: 0x0600073D RID: 1853 RVA: 0x0001C254 File Offset: 0x0001A454
	public static string GetCardSetNameKeyShortened(TAG_CARD_SET tag)
	{
		string text = null;
		return (!GameStrings.s_cardSetNamesShortened.TryGetValue(tag, out text)) ? null : text;
	}

	// Token: 0x0600073E RID: 1854 RVA: 0x0001C27C File Offset: 0x0001A47C
	public static bool HasCardSetNameInitials(TAG_CARD_SET tag)
	{
		return GameStrings.s_cardSetNamesInitials.ContainsKey(tag);
	}

	// Token: 0x0600073F RID: 1855 RVA: 0x0001C28C File Offset: 0x0001A48C
	public static string GetCardSetNameInitials(TAG_CARD_SET tag)
	{
		string key = null;
		return (!GameStrings.s_cardSetNamesInitials.TryGetValue(tag, out key)) ? "UNKNOWN" : GameStrings.Get(key);
	}

	// Token: 0x06000740 RID: 1856 RVA: 0x0001C2C0 File Offset: 0x0001A4C0
	public static string GetRandomTip(TipCategory tipCategory)
	{
		int num = 0;
		List<string> list = new List<string>();
		for (;;)
		{
			string text = string.Format("GLUE_TIP_{0}_{1}", tipCategory, num);
			string text2 = GameStrings.Get(text);
			if (text2.Equals(text))
			{
				break;
			}
			if (UniversalInputManager.Get().IsTouchMode())
			{
				string text3 = text + "_TOUCH";
				string text4 = GameStrings.Get(text3);
				if (!text4.Equals(text3))
				{
					text2 = text4;
				}
				if (UniversalInputManager.UsePhoneUI)
				{
					string text5 = text + "_PHONE";
					string text6 = GameStrings.Get(text5);
					if (!text6.Equals(text5))
					{
						text2 = text6;
					}
				}
			}
			list.Add(text2);
			num++;
		}
		if (list.Count == 0)
		{
			Debug.LogError(string.Format("GameStrings.GetRandomTip() - no tips in category {0}", tipCategory));
			return "UNKNOWN";
		}
		int num2 = Random.Range(0, list.Count);
		return list[num2];
	}

	// Token: 0x06000741 RID: 1857 RVA: 0x0001C3BC File Offset: 0x0001A5BC
	public static string GetTip(TipCategory tipCategory, int progress, TipCategory randomTipCategory = TipCategory.DEFAULT)
	{
		int num = 0;
		List<string> list = new List<string>();
		for (;;)
		{
			string text = string.Format("GLUE_TIP_{0}_{1}", tipCategory, num);
			string text2 = GameStrings.Get(text);
			if (text2.Equals(text))
			{
				break;
			}
			if (UniversalInputManager.Get().IsTouchMode())
			{
				string text3 = text + "_TOUCH";
				string text4 = GameStrings.Get(text3);
				if (!text4.Equals(text3))
				{
					text2 = text4;
				}
				if (UniversalInputManager.UsePhoneUI)
				{
					string text5 = text + "_PHONE";
					string text6 = GameStrings.Get(text5);
					if (!text6.Equals(text5))
					{
						text2 = text6;
					}
				}
			}
			list.Add(text2);
			num++;
		}
		if (progress < list.Count)
		{
			return list[progress];
		}
		return GameStrings.GetRandomTip(randomTipCategory);
	}

	// Token: 0x06000742 RID: 1858 RVA: 0x0001C498 File Offset: 0x0001A698
	private static bool LoadCategory(GameStringCategory cat)
	{
		if (GameStrings.s_tables.ContainsKey(cat))
		{
			Debug.LogWarning(string.Format("GameStrings.LoadCategory() - {0} is already loaded", cat));
			return false;
		}
		GameStringTable gameStringTable = new GameStringTable();
		if (!gameStringTable.Load(cat))
		{
			Debug.LogError(string.Format("GameStrings.LoadCategory() - {0} failed to load", cat));
			return false;
		}
		if (ApplicationMgr.IsInternal())
		{
			GameStrings.CheckConflicts(gameStringTable);
		}
		GameStrings.s_tables.Add(cat, gameStringTable);
		return true;
	}

	// Token: 0x06000743 RID: 1859 RVA: 0x0001C514 File Offset: 0x0001A714
	private static bool UnloadCategory(GameStringCategory cat)
	{
		if (!GameStrings.s_tables.Remove(cat))
		{
			Debug.LogWarning(string.Format("GameStrings.UnloadCategory() - {0} was never loaded", cat));
			return false;
		}
		return true;
	}

	// Token: 0x06000744 RID: 1860 RVA: 0x0001C54C File Offset: 0x0001A74C
	private static void CheckConflicts(GameStringTable table)
	{
		Map<string, string>.KeyCollection keys = table.GetAll().Keys;
		GameStringCategory category = table.GetCategory();
		foreach (GameStringTable gameStringTable in GameStrings.s_tables.Values)
		{
			foreach (string text in keys)
			{
				if (gameStringTable.Get(text) != null)
				{
					string message = string.Format("GameStrings.CheckConflicts() - Tag {0} is used in {1} and {2}. All tags must be unique.", text, category, gameStringTable.GetCategory());
					Error.AddDevWarning("GameStrings Error", message, new object[0]);
				}
			}
		}
	}

	// Token: 0x06000745 RID: 1861 RVA: 0x0001C63C File Offset: 0x0001A83C
	private static string Find(string key)
	{
		foreach (GameStringTable gameStringTable in GameStrings.s_tables.Values)
		{
			string text = gameStringTable.Get(key);
			if (text != null)
			{
				return text;
			}
		}
		return null;
	}

	// Token: 0x06000746 RID: 1862 RVA: 0x0001C6AC File Offset: 0x0001A8AC
	private static string[] ParseLanguageRuleArgs(string str, int ruleIndex, out int argStartIndex, out int argEndIndex)
	{
		argStartIndex = -1;
		argEndIndex = -1;
		argStartIndex = str.IndexOf('(', ruleIndex + 2);
		if (argStartIndex < 0)
		{
			Debug.LogWarning(string.Format("GameStrings.ParseLanguageRuleArgs() - failed to parse '(' for rule at index {0} in string {1}", ruleIndex, str));
			return null;
		}
		argEndIndex = str.IndexOf(')', argStartIndex + 1);
		if (argEndIndex < 0)
		{
			Debug.LogWarning(string.Format("GameStrings.ParseLanguageRuleArgs() - failed to parse ')' for rule at index {0} in string {1}", ruleIndex, str));
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(str, argStartIndex + 1, argEndIndex - argStartIndex - 1);
		string text = stringBuilder.ToString();
		MatchCollection matchCollection = Regex.Matches(text, "(?:[0-9]+,)*[0-9]+");
		if (matchCollection.Count > 0)
		{
			stringBuilder.Remove(0, stringBuilder.Length);
			int num = 0;
			foreach (object obj in matchCollection)
			{
				Match match = (Match)obj;
				stringBuilder.Append(text, num, match.Index - num);
				stringBuilder.Append('0', match.Length);
				num = match.Index + match.Length;
			}
			stringBuilder.Append(text, num, text.Length - num);
			text = stringBuilder.ToString();
		}
		string[] array = text.Split(GameStrings.LANGUAGE_RULE_ARG_DELIMITERS);
		int num2 = 0;
		for (int i = 0; i < array.Length; i++)
		{
			string text2 = array[i];
			if (matchCollection.Count > 0)
			{
				stringBuilder.Remove(0, stringBuilder.Length);
				int num3 = 0;
				foreach (object obj2 in matchCollection)
				{
					Match match2 = (Match)obj2;
					if (match2.Index >= num2 && match2.Index < num2 + text2.Length)
					{
						int num4 = match2.Index - num2;
						stringBuilder.Append(text2, num3, num4 - num3);
						stringBuilder.Append(match2.Value);
						num3 = num4 + match2.Length;
					}
				}
				stringBuilder.Append(text2, num3, text2.Length - num3);
				text2 = stringBuilder.ToString();
				num2 += text2.Length + 1;
			}
			text2 = text2.Trim();
			array[i] = text2;
		}
		return array;
	}

	// Token: 0x06000747 RID: 1863 RVA: 0x0001C930 File Offset: 0x0001AB30
	private static string ParseLanguageRule4(string str, GameStrings.PluralNumber[] pluralNumbers = null)
	{
		StringBuilder stringBuilder = null;
		int? num = default(int?);
		int num2 = 0;
		int num3 = 0;
		for (int i = str.IndexOf("|4"); i >= 0; i = str.IndexOf("|4", i + 2))
		{
			num3++;
			int num4;
			int num5;
			string[] array = GameStrings.ParseLanguageRuleArgs(str, i, out num4, out num5);
			if (array != null)
			{
				int num6 = num2;
				int num7 = i - num2;
				string text = str.Substring(num6, num7);
				GameStrings.PluralNumber pluralNumber = null;
				if (pluralNumbers != null)
				{
					int pluralArgIndex = num3 - 1;
					pluralNumber = Array.Find<GameStrings.PluralNumber>(pluralNumbers, (GameStrings.PluralNumber currPluralNumber) => currPluralNumber.m_index == pluralArgIndex);
				}
				int num8;
				if (pluralNumber != null)
				{
					num = new int?(pluralNumber.m_number);
				}
				else if (GameStrings.ParseLanguageRule4Number(array, text, out num8))
				{
					num = new int?(num8);
				}
				else if (num == null)
				{
					Debug.LogWarning(string.Format("GameStrings.ParseLanguageRule4() - failed to parse a number in substring \"{0}\" (indexes {1}-{2}) for rule {3} in string \"{4}\"", new object[]
					{
						text,
						num6,
						num7,
						num3,
						str
					}));
					goto IL_186;
				}
				int pluralIndex = GameStrings.GetPluralIndex(num.Value);
				if (pluralIndex >= array.Length)
				{
					Debug.LogWarning(string.Format("GameStrings.ParseLanguageRule4() - not enough arguments for rule {0} in string \"{1}\"", num3, str));
				}
				else
				{
					string text2 = array[pluralIndex];
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder();
					}
					stringBuilder.Append(text);
					stringBuilder.Append(text2);
					num2 = num5 + 1;
				}
				if (pluralNumber != null && pluralNumber.m_useForOnlyThisIndex)
				{
					num = default(int?);
				}
			}
			IL_186:;
		}
		if (stringBuilder == null)
		{
			return str;
		}
		stringBuilder.Append(str, num2, str.Length - num2);
		return stringBuilder.ToString();
	}

	// Token: 0x06000748 RID: 1864 RVA: 0x0001CAFC File Offset: 0x0001ACFC
	private static bool ParseLanguageRule4Number(string[] args, string betweenRulesStr, out int number)
	{
		if (GameStrings.ParseLanguageRule4Number_Foreward(args[0], out number))
		{
			return true;
		}
		if (GameStrings.ParseLanguageRule4Number_Backward(betweenRulesStr, out number))
		{
			return true;
		}
		number = 0;
		return false;
	}

	// Token: 0x06000749 RID: 1865 RVA: 0x0001CB2C File Offset: 0x0001AD2C
	private static bool ParseLanguageRule4Number_Foreward(string str, out int number)
	{
		number = 0;
		Match match = Regex.Match(str, "(?:[0-9]+,)*[0-9]+");
		return match.Success && GeneralUtils.TryParseInt(match.Value, out number);
	}

	// Token: 0x0600074A RID: 1866 RVA: 0x0001CB6C File Offset: 0x0001AD6C
	private static bool ParseLanguageRule4Number_Backward(string str, out int number)
	{
		number = 0;
		MatchCollection matchCollection = Regex.Matches(str, "(?:[0-9]+,)*[0-9]+");
		if (matchCollection.Count == 0)
		{
			return false;
		}
		Match match = matchCollection[matchCollection.Count - 1];
		return GeneralUtils.TryParseInt(match.Value, out number);
	}

	// Token: 0x0600074B RID: 1867 RVA: 0x0001CBB8 File Offset: 0x0001ADB8
	private static int GetPluralIndex(int number)
	{
		switch (Localization.GetLocale())
		{
		case Locale.frFR:
		case Locale.koKR:
		case Locale.zhTW:
		case Locale.zhCN:
			if (number <= 1)
			{
				return 0;
			}
			return 1;
		case Locale.ruRU:
			switch (number % 100)
			{
			case 11:
			case 12:
			case 13:
			case 14:
				return 2;
			default:
				switch (number % 10)
				{
				case 1:
					return 0;
				case 2:
				case 3:
				case 4:
					return 1;
				default:
					return 2;
				}
				break;
			}
			break;
		case Locale.plPL:
			if (number == 1)
			{
				return 0;
			}
			if (number == 0)
			{
				return 2;
			}
			switch (number % 100)
			{
			case 11:
			case 12:
			case 13:
			case 14:
				return 2;
			default:
				switch (number % 10)
				{
				case 2:
				case 3:
				case 4:
					return 1;
				default:
					return 2;
				}
				break;
			}
			break;
		}
		if (number == 1)
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x040003CE RID: 974
	public const string s_UnknownName = "UNKNOWN";

	// Token: 0x040003CF RID: 975
	private const string NUMBER_PATTERN = "(?:[0-9]+,)*[0-9]+";

	// Token: 0x040003D0 RID: 976
	private static Map<GameStringCategory, GameStringTable> s_tables = new Map<GameStringCategory, GameStringTable>();

	// Token: 0x040003D1 RID: 977
	private static readonly char[] LANGUAGE_RULE_ARG_DELIMITERS = new char[]
	{
		','
	};

	// Token: 0x040003D2 RID: 978
	public static Map<TAG_CLASS, string> s_classNames = new Map<TAG_CLASS, string>
	{
		{
			TAG_CLASS.DEATHKNIGHT,
			"GLOBAL_CLASS_DEATHKNIGHT"
		},
		{
			TAG_CLASS.DRUID,
			"GLOBAL_CLASS_DRUID"
		},
		{
			TAG_CLASS.HUNTER,
			"GLOBAL_CLASS_HUNTER"
		},
		{
			TAG_CLASS.MAGE,
			"GLOBAL_CLASS_MAGE"
		},
		{
			TAG_CLASS.PALADIN,
			"GLOBAL_CLASS_PALADIN"
		},
		{
			TAG_CLASS.PRIEST,
			"GLOBAL_CLASS_PRIEST"
		},
		{
			TAG_CLASS.ROGUE,
			"GLOBAL_CLASS_ROGUE"
		},
		{
			TAG_CLASS.SHAMAN,
			"GLOBAL_CLASS_SHAMAN"
		},
		{
			TAG_CLASS.WARLOCK,
			"GLOBAL_CLASS_WARLOCK"
		},
		{
			TAG_CLASS.WARRIOR,
			"GLOBAL_CLASS_WARRIOR"
		},
		{
			TAG_CLASS.INVALID,
			"GLOBAL_CLASS_NEUTRAL"
		}
	};

	// Token: 0x040003D3 RID: 979
	public static Map<TAG_RACE, string> s_raceNames = new Map<TAG_RACE, string>
	{
		{
			TAG_RACE.BLOODELF,
			"GLOBAL_RACE_BLOODELF"
		},
		{
			TAG_RACE.DRAENEI,
			"GLOBAL_RACE_DRAENEI"
		},
		{
			TAG_RACE.DWARF,
			"GLOBAL_RACE_DWARF"
		},
		{
			TAG_RACE.GNOME,
			"GLOBAL_RACE_GNOME"
		},
		{
			TAG_RACE.GOBLIN,
			"GLOBAL_RACE_GOBLIN"
		},
		{
			TAG_RACE.HUMAN,
			"GLOBAL_RACE_HUMAN"
		},
		{
			TAG_RACE.NIGHTELF,
			"GLOBAL_RACE_NIGHTELF"
		},
		{
			TAG_RACE.ORC,
			"GLOBAL_RACE_ORC"
		},
		{
			TAG_RACE.TAUREN,
			"GLOBAL_RACE_TAUREN"
		},
		{
			TAG_RACE.TROLL,
			"GLOBAL_RACE_TROLL"
		},
		{
			TAG_RACE.UNDEAD,
			"GLOBAL_RACE_UNDEAD"
		},
		{
			TAG_RACE.WORGEN,
			"GLOBAL_RACE_WORGEN"
		},
		{
			TAG_RACE.MURLOC,
			"GLOBAL_RACE_MURLOC"
		},
		{
			TAG_RACE.DEMON,
			"GLOBAL_RACE_DEMON"
		},
		{
			TAG_RACE.SCOURGE,
			"GLOBAL_RACE_SCOURGE"
		},
		{
			TAG_RACE.MECHANICAL,
			"GLOBAL_RACE_MECHANICAL"
		},
		{
			TAG_RACE.ELEMENTAL,
			"GLOBAL_RACE_ELEMENTAL"
		},
		{
			TAG_RACE.OGRE,
			"GLOBAL_RACE_OGRE"
		},
		{
			TAG_RACE.PET,
			"GLOBAL_RACE_PET"
		},
		{
			TAG_RACE.TOTEM,
			"GLOBAL_RACE_TOTEM"
		},
		{
			TAG_RACE.NERUBIAN,
			"GLOBAL_RACE_NERUBIAN"
		},
		{
			TAG_RACE.PIRATE,
			"GLOBAL_RACE_PIRATE"
		},
		{
			TAG_RACE.DRAGON,
			"GLOBAL_RACE_DRAGON"
		}
	};

	// Token: 0x040003D4 RID: 980
	public static Map<TAG_RARITY, string> s_rarityNames = new Map<TAG_RARITY, string>
	{
		{
			TAG_RARITY.COMMON,
			"GLOBAL_RARITY_COMMON"
		},
		{
			TAG_RARITY.EPIC,
			"GLOBAL_RARITY_EPIC"
		},
		{
			TAG_RARITY.LEGENDARY,
			"GLOBAL_RARITY_LEGENDARY"
		},
		{
			TAG_RARITY.RARE,
			"GLOBAL_RARITY_RARE"
		},
		{
			TAG_RARITY.FREE,
			"GLOBAL_RARITY_FREE"
		}
	};

	// Token: 0x040003D5 RID: 981
	public static Map<TAG_CARD_SET, string> s_cardSetNames = new Map<TAG_CARD_SET, string>
	{
		{
			TAG_CARD_SET.CORE,
			"GLOBAL_CARD_SET_CORE"
		},
		{
			TAG_CARD_SET.EXPERT1,
			"GLOBAL_CARD_SET_EXPERT1"
		},
		{
			TAG_CARD_SET.REWARD,
			"GLOBAL_CARD_SET_REWARD"
		},
		{
			TAG_CARD_SET.PROMO,
			"GLOBAL_CARD_SET_PROMO"
		},
		{
			TAG_CARD_SET.FP1,
			"GLOBAL_CARD_SET_NAXX"
		},
		{
			TAG_CARD_SET.PE1,
			"GLOBAL_CARD_SET_GVG"
		},
		{
			TAG_CARD_SET.BRM,
			"GLOBAL_CARD_SET_BRM"
		},
		{
			TAG_CARD_SET.TGT,
			"GLOBAL_CARD_SET_TGT"
		},
		{
			TAG_CARD_SET.LOE,
			"GLOBAL_CARD_SET_LOE"
		},
		{
			TAG_CARD_SET.OG,
			"GLOBAL_CARD_SET_OG"
		},
		{
			TAG_CARD_SET.OG_RESERVE,
			"GLOBAL_CARD_SET_OG_RESERVE"
		},
		{
			TAG_CARD_SET.SLUSH,
			"GLOBAL_CARD_SET_DEBUG"
		}
	};

	// Token: 0x040003D6 RID: 982
	public static Map<TAG_CARD_SET, string> s_cardSetNamesShortened = new Map<TAG_CARD_SET, string>
	{
		{
			TAG_CARD_SET.CORE,
			"GLOBAL_CARD_SET_CORE"
		},
		{
			TAG_CARD_SET.EXPERT1,
			"GLOBAL_CARD_SET_EXPERT1"
		},
		{
			TAG_CARD_SET.REWARD,
			"GLOBAL_CARD_SET_REWARD"
		},
		{
			TAG_CARD_SET.PROMO,
			"GLOBAL_CARD_SET_PROMO"
		},
		{
			TAG_CARD_SET.FP1,
			"GLOBAL_CARD_SET_NAXX"
		},
		{
			TAG_CARD_SET.PE1,
			"GLOBAL_CARD_SET_GVG"
		},
		{
			TAG_CARD_SET.BRM,
			"GLOBAL_CARD_SET_BRM"
		},
		{
			TAG_CARD_SET.TGT,
			"GLOBAL_CARD_SET_TGT_SHORT"
		},
		{
			TAG_CARD_SET.LOE,
			"GLOBAL_CARD_SET_LOE_SHORT"
		},
		{
			TAG_CARD_SET.OG,
			"GLOBAL_CARD_SET_OG_SHORT"
		},
		{
			TAG_CARD_SET.OG_RESERVE,
			"GLOBAL_CARD_SET_OG_RESERVE"
		},
		{
			TAG_CARD_SET.SLUSH,
			"GLOBAL_CARD_SET_DEBUG"
		}
	};

	// Token: 0x040003D7 RID: 983
	public static Map<TAG_CARD_SET, string> s_cardSetNamesInitials = new Map<TAG_CARD_SET, string>
	{
		{
			TAG_CARD_SET.FP1,
			"GLOBAL_CARD_SET_NAXX_SEARCHABLE_SHORTHAND_NAMES"
		},
		{
			TAG_CARD_SET.PE1,
			"GLOBAL_CARD_SET_GVG_SEARCHABLE_SHORTHAND_NAMES"
		},
		{
			TAG_CARD_SET.BRM,
			"GLOBAL_CARD_SET_BRM_SEARCHABLE_SHORTHAND_NAMES"
		},
		{
			TAG_CARD_SET.TGT,
			"GLOBAL_CARD_SET_TGT_SEARCHABLE_SHORTHAND_NAMES"
		},
		{
			TAG_CARD_SET.LOE,
			"GLOBAL_CARD_SET_LOE_SEARCHABLE_SHORTHAND_NAMES"
		},
		{
			TAG_CARD_SET.OG,
			"GLOBAL_CARD_SET_OG_SEARCHABLE_SHORTHAND_NAMES"
		}
	};

	// Token: 0x040003D8 RID: 984
	public static Map<TAG_CARDTYPE, string> s_cardTypeNames = new Map<TAG_CARDTYPE, string>
	{
		{
			TAG_CARDTYPE.HERO,
			"GLOBAL_CARDTYPE_HERO"
		},
		{
			TAG_CARDTYPE.MINION,
			"GLOBAL_CARDTYPE_MINION"
		},
		{
			TAG_CARDTYPE.SPELL,
			"GLOBAL_CARDTYPE_SPELL"
		},
		{
			TAG_CARDTYPE.ENCHANTMENT,
			"GLOBAL_CARDTYPE_ENCHANTMENT"
		},
		{
			TAG_CARDTYPE.WEAPON,
			"GLOBAL_CARDTYPE_WEAPON"
		},
		{
			TAG_CARDTYPE.ITEM,
			"GLOBAL_CARDTYPE_ITEM"
		},
		{
			TAG_CARDTYPE.TOKEN,
			"GLOBAL_CARDTYPE_TOKEN"
		},
		{
			TAG_CARDTYPE.HERO_POWER,
			"GLOBAL_CARDTYPE_HEROPOWER"
		}
	};

	// Token: 0x040003D9 RID: 985
	private static Map<GAME_TAG, string[]> s_keywordText = new Map<GAME_TAG, string[]>
	{
		{
			GAME_TAG.TAUNT,
			new string[]
			{
				"GLOBAL_KEYWORD_TAUNT",
				"GLOBAL_KEYWORD_TAUNT_TEXT"
			}
		},
		{
			GAME_TAG.SPELLPOWER,
			new string[]
			{
				"GLOBAL_KEYWORD_SPELLPOWER",
				"GLOBAL_KEYWORD_SPELLPOWER_TEXT"
			}
		},
		{
			GAME_TAG.DIVINE_SHIELD,
			new string[]
			{
				"GLOBAL_KEYWORD_DIVINE_SHIELD",
				"GLOBAL_KEYWORD_DIVINE_SHIELD_TEXT"
			}
		},
		{
			GAME_TAG.CHARGE,
			new string[]
			{
				"GLOBAL_KEYWORD_CHARGE",
				"GLOBAL_KEYWORD_CHARGE_TEXT"
			}
		},
		{
			GAME_TAG.SECRET,
			new string[]
			{
				"GLOBAL_KEYWORD_SECRET",
				"GLOBAL_KEYWORD_SECRET_TEXT"
			}
		},
		{
			GAME_TAG.STEALTH,
			new string[]
			{
				"GLOBAL_KEYWORD_STEALTH",
				"GLOBAL_KEYWORD_STEALTH_TEXT"
			}
		},
		{
			GAME_TAG.ENRAGED,
			new string[]
			{
				"GLOBAL_KEYWORD_ENRAGED",
				"GLOBAL_KEYWORD_ENRAGED_TEXT"
			}
		},
		{
			GAME_TAG.BATTLECRY,
			new string[]
			{
				"GLOBAL_KEYWORD_BATTLECRY",
				"GLOBAL_KEYWORD_BATTLECRY_TEXT"
			}
		},
		{
			GAME_TAG.FROZEN,
			new string[]
			{
				"GLOBAL_KEYWORD_FROZEN",
				"GLOBAL_KEYWORD_FROZEN_TEXT"
			}
		},
		{
			GAME_TAG.FREEZE,
			new string[]
			{
				"GLOBAL_KEYWORD_FREEZE",
				"GLOBAL_KEYWORD_FREEZE_TEXT"
			}
		},
		{
			GAME_TAG.WINDFURY,
			new string[]
			{
				"GLOBAL_KEYWORD_WINDFURY",
				"GLOBAL_KEYWORD_WINDFURY_TEXT"
			}
		},
		{
			GAME_TAG.DEATHRATTLE,
			new string[]
			{
				"GLOBAL_KEYWORD_DEATHRATTLE",
				"GLOBAL_KEYWORD_DEATHRATTLE_TEXT"
			}
		},
		{
			GAME_TAG.COMBO,
			new string[]
			{
				"GLOBAL_KEYWORD_COMBO",
				"GLOBAL_KEYWORD_COMBO_TEXT"
			}
		},
		{
			GAME_TAG.OVERLOAD,
			new string[]
			{
				"GLOBAL_KEYWORD_OVERLOAD",
				"GLOBAL_KEYWORD_OVERLOAD_TEXT"
			}
		},
		{
			GAME_TAG.SILENCE,
			new string[]
			{
				"GLOBAL_KEYWORD_SILENCE",
				"GLOBAL_KEYWORD_SILENCE_TEXT"
			}
		},
		{
			GAME_TAG.COUNTER,
			new string[]
			{
				"GLOBAL_KEYWORD_COUNTER",
				"GLOBAL_KEYWORD_COUNTER_TEXT"
			}
		},
		{
			GAME_TAG.CANT_BE_DAMAGED,
			new string[]
			{
				"GLOBAL_KEYWORD_IMMUNE",
				"GLOBAL_KEYWORD_IMMUNE_TEXT"
			}
		},
		{
			GAME_TAG.AI_MUST_PLAY,
			new string[]
			{
				"GLOBAL_KEYWORD_AUTOCAST",
				"GLOBAL_KEYWORD_AUTOCAST_TEXT"
			}
		},
		{
			GAME_TAG.SPARE_PART,
			new string[]
			{
				"GLOBAL_KEYWORD_SPAREPART",
				"GLOBAL_KEYWORD_SPAREPART_TEXT"
			}
		},
		{
			GAME_TAG.INSPIRE,
			new string[]
			{
				"GLOBAL_KEYWORD_INSPIRE",
				"GLOBAL_KEYWORD_INSPIRE_TEXT"
			}
		},
		{
			GAME_TAG.TREASURE,
			new string[]
			{
				"GLOBAL_KEYWORD_TREASURE",
				"GLOBAL_KEYWORD_TREASURE_TEXT"
			}
		},
		{
			GAME_TAG.CTHUN,
			new string[]
			{
				"GLOBAL_KEYWORD_CTHUN",
				"GLOBAL_KEYWORD_CTHUN_TEXT"
			}
		},
		{
			GAME_TAG.SHIFTING,
			new string[]
			{
				"GLOBAL_KEYWORD_SHIFTING",
				"GLOBAL_KEYWORD_SHIFTING_TEXT"
			}
		}
	};

	// Token: 0x040003DA RID: 986
	private static Map<GAME_TAG, string[]> s_refKeywordText = new Map<GAME_TAG, string[]>
	{
		{
			GAME_TAG.TAUNT,
			new string[]
			{
				"GLOBAL_KEYWORD_TAUNT",
				"GLOBAL_KEYWORD_TAUNT_REF_TEXT"
			}
		},
		{
			GAME_TAG.SPELLPOWER,
			new string[]
			{
				"GLOBAL_KEYWORD_SPELLPOWER",
				"GLOBAL_KEYWORD_SPELLPOWER_REF_TEXT"
			}
		},
		{
			GAME_TAG.DIVINE_SHIELD,
			new string[]
			{
				"GLOBAL_KEYWORD_DIVINE_SHIELD",
				"GLOBAL_KEYWORD_DIVINE_SHIELD_REF_TEXT"
			}
		},
		{
			GAME_TAG.CHARGE,
			new string[]
			{
				"GLOBAL_KEYWORD_CHARGE",
				"GLOBAL_KEYWORD_CHARGE_TEXT"
			}
		},
		{
			GAME_TAG.SECRET,
			new string[]
			{
				"GLOBAL_KEYWORD_SECRET",
				"GLOBAL_KEYWORD_SECRET_TEXT"
			}
		},
		{
			GAME_TAG.STEALTH,
			new string[]
			{
				"GLOBAL_KEYWORD_STEALTH",
				"GLOBAL_KEYWORD_STEALTH_REF_TEXT"
			}
		},
		{
			GAME_TAG.ENRAGED,
			new string[]
			{
				"GLOBAL_KEYWORD_ENRAGED",
				"GLOBAL_KEYWORD_ENRAGED_TEXT"
			}
		},
		{
			GAME_TAG.BATTLECRY,
			new string[]
			{
				"GLOBAL_KEYWORD_BATTLECRY",
				"GLOBAL_KEYWORD_BATTLECRY_TEXT"
			}
		},
		{
			GAME_TAG.FROZEN,
			new string[]
			{
				"GLOBAL_KEYWORD_FROZEN",
				"GLOBAL_KEYWORD_FROZEN_TEXT"
			}
		},
		{
			GAME_TAG.FREEZE,
			new string[]
			{
				"GLOBAL_KEYWORD_FREEZE",
				"GLOBAL_KEYWORD_FREEZE_TEXT"
			}
		},
		{
			GAME_TAG.WINDFURY,
			new string[]
			{
				"GLOBAL_KEYWORD_WINDFURY",
				"GLOBAL_KEYWORD_WINDFURY_TEXT"
			}
		},
		{
			GAME_TAG.DEATHRATTLE,
			new string[]
			{
				"GLOBAL_KEYWORD_DEATHRATTLE",
				"GLOBAL_KEYWORD_DEATHRATTLE_TEXT"
			}
		},
		{
			GAME_TAG.COMBO,
			new string[]
			{
				"GLOBAL_KEYWORD_COMBO",
				"GLOBAL_KEYWORD_COMBO_TEXT"
			}
		},
		{
			GAME_TAG.OVERLOAD,
			new string[]
			{
				"GLOBAL_KEYWORD_OVERLOAD",
				"GLOBAL_KEYWORD_OVERLOAD_TEXT"
			}
		},
		{
			GAME_TAG.SILENCE,
			new string[]
			{
				"GLOBAL_KEYWORD_SILENCE",
				"GLOBAL_KEYWORD_SILENCE_TEXT"
			}
		},
		{
			GAME_TAG.COUNTER,
			new string[]
			{
				"GLOBAL_KEYWORD_COUNTER",
				"GLOBAL_KEYWORD_COUNTER_TEXT"
			}
		},
		{
			GAME_TAG.CANT_BE_DAMAGED,
			new string[]
			{
				"GLOBAL_KEYWORD_IMMUNE",
				"GLOBAL_KEYWORD_IMMUNE_REF_TEXT"
			}
		},
		{
			GAME_TAG.AI_MUST_PLAY,
			new string[]
			{
				"GLOBAL_KEYWORD_AUTOCAST",
				"GLOBAL_KEYWORD_AUTOCAST_TEXT"
			}
		},
		{
			GAME_TAG.SPARE_PART,
			new string[]
			{
				"GLOBAL_KEYWORD_SPAREPART",
				"GLOBAL_KEYWORD_SPAREPART_TEXT"
			}
		},
		{
			GAME_TAG.INSPIRE,
			new string[]
			{
				"GLOBAL_KEYWORD_INSPIRE",
				"GLOBAL_KEYWORD_INSPIRE_TEXT"
			}
		},
		{
			GAME_TAG.TREASURE,
			new string[]
			{
				"GLOBAL_KEYWORD_TREASURE",
				"GLOBAL_KEYWORD_TREASURE_TEXT"
			}
		},
		{
			GAME_TAG.CTHUN,
			new string[]
			{
				"GLOBAL_KEYWORD_CTHUN",
				"GLOBAL_KEYWORD_CTHUN_TEXT"
			}
		},
		{
			GAME_TAG.SHIFTING,
			new string[]
			{
				"GLOBAL_KEYWORD_SHIFTING",
				"GLOBAL_KEYWORD_SHIFTING_TEXT"
			}
		}
	};

	// Token: 0x02000295 RID: 661
	public class PluralNumber
	{
		// Token: 0x04001505 RID: 5381
		public int m_index;

		// Token: 0x04001506 RID: 5382
		public int m_number;

		// Token: 0x04001507 RID: 5383
		public bool m_useForOnlyThisIndex;
	}
}
