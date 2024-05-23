using System;
using System.Collections.Generic;
using System.Globalization;
using bgs;

// Token: 0x020000A5 RID: 165
public class Localization
{
	// Token: 0x0600081A RID: 2074 RVA: 0x0001FE44 File Offset: 0x0001E044
	// Note: this type is marked as 'beforefieldinit'.
	static Localization()
	{
		Map<Locale, Locale[]> map = new Map<Locale, Locale[]>();
		map.Add(Locale.enUS, new Locale[1]);
		Map<Locale, Locale[]> map2 = map;
		Locale key = Locale.enGB;
		Locale[] array = new Locale[2];
		array[0] = Locale.enGB;
		map2.Add(key, array);
		map.Add(Locale.frFR, new Locale[]
		{
			Locale.frFR
		});
		map.Add(Locale.deDE, new Locale[]
		{
			Locale.deDE
		});
		map.Add(Locale.koKR, new Locale[]
		{
			Locale.koKR
		});
		map.Add(Locale.esES, new Locale[]
		{
			Locale.esES
		});
		map.Add(Locale.esMX, new Locale[]
		{
			Locale.esMX
		});
		map.Add(Locale.ruRU, new Locale[]
		{
			Locale.ruRU
		});
		map.Add(Locale.zhTW, new Locale[]
		{
			Locale.zhTW
		});
		map.Add(Locale.zhCN, new Locale[]
		{
			Locale.zhCN
		});
		map.Add(Locale.itIT, new Locale[]
		{
			Locale.itIT
		});
		map.Add(Locale.ptBR, new Locale[]
		{
			Locale.ptBR
		});
		map.Add(Locale.plPL, new Locale[]
		{
			Locale.plPL
		});
		map.Add(Locale.jaJP, new Locale[]
		{
			Locale.jaJP
		});
		map.Add(Locale.thTH, new Locale[]
		{
			Locale.thTH
		});
		Localization.LOAD_ORDERS = map;
		Localization.s_instance = new Localization();
		Localization.LOCALE_FROM_OPTIONS = new PlatformDependentValue<bool>(PlatformCategory.OS)
		{
			iOS = true,
			Android = true,
			PC = false,
			Mac = false
		};
	}

	// Token: 0x0600081B RID: 2075 RVA: 0x0001FFA8 File Offset: 0x0001E1A8
	public static void Initialize()
	{
		Locale? locale = default(Locale?);
		if (Localization.LOCALE_FROM_OPTIONS)
		{
			string @string = Options.Get().GetString(Option.LOCALE);
			Locale locale2;
			if (EnumUtils.TryGetEnum<Locale>(@string, out locale2))
			{
				locale = new Locale?(locale2);
			}
		}
		if (locale == null)
		{
			string text = null;
			if (ApplicationMgr.IsPublic())
			{
				text = BattleNet.GetLaunchOption("LOCALE", false);
			}
			if (string.IsNullOrEmpty(text))
			{
				text = Vars.Key("Localization.Locale").GetStr(Localization.DEFAULT_LOCALE_NAME);
			}
			if (ApplicationMgr.IsInternal())
			{
				string str = Vars.Key("Localization.OverrideBnetLocale").GetStr(string.Empty);
				if (!string.IsNullOrEmpty(str))
				{
					text = str;
				}
			}
			Locale locale3;
			if (EnumUtils.TryGetEnum<Locale>(text, out locale3))
			{
				locale = new Locale?(locale3);
			}
			else
			{
				locale = new Locale?(Locale.enUS);
			}
		}
		Localization.SetLocale(locale.Value);
	}

	// Token: 0x0600081C RID: 2076 RVA: 0x00020093 File Offset: 0x0001E293
	public static Locale GetLocale()
	{
		return Localization.s_instance.m_locale;
	}

	// Token: 0x0600081D RID: 2077 RVA: 0x0002009F File Offset: 0x0001E29F
	public static void SetLocale(Locale locale)
	{
		Localization.s_instance.SetPegLocale(locale);
	}

	// Token: 0x0600081E RID: 2078 RVA: 0x000200AC File Offset: 0x0001E2AC
	public static bool IsIMELocale()
	{
		return Localization.GetLocale() == Locale.zhCN || Localization.GetLocale() == Locale.zhTW || Localization.GetLocale() == Locale.koKR;
	}

	// Token: 0x0600081F RID: 2079 RVA: 0x000200DB File Offset: 0x0001E2DB
	public static string GetLocaleName()
	{
		return Localization.s_instance.m_locale.ToString();
	}

	// Token: 0x06000820 RID: 2080 RVA: 0x000200F4 File Offset: 0x0001E2F4
	public static string GetBnetLocaleName()
	{
		string text = Localization.s_instance.m_locale.ToString();
		return string.Format("{0}-{1}", text.Substring(0, 2), text.Substring(2, 2));
	}

	// Token: 0x06000821 RID: 2081 RVA: 0x00020130 File Offset: 0x0001E330
	public static bool SetLocaleName(string localeName)
	{
		if (!Localization.IsValidLocaleName(localeName))
		{
			return false;
		}
		Localization.s_instance.SetPegLocaleName(localeName);
		return true;
	}

	// Token: 0x06000822 RID: 2082 RVA: 0x0002014B File Offset: 0x0001E34B
	public static Locale[] GetLoadOrder(AssetFamily family)
	{
		return Localization.GetLoadOrder(family == AssetFamily.CardTexture || family == AssetFamily.CardPremium);
	}

	// Token: 0x06000823 RID: 2083 RVA: 0x00020160 File Offset: 0x0001E360
	public static Locale[] GetLoadOrder(Locale locale, bool isCardTexture = false)
	{
		Locale[] array = Localization.LOAD_ORDERS[locale];
		if (Network.IsRunning() && BattleNet.GetAccountCountry() == "CHN" && isCardTexture)
		{
			Array.Resize<Locale>(ref array, array.Length + 1);
			Array.Copy(array, 0, array, 1, array.Length - 1);
			array[0] = Locale.zhCN;
		}
		return array;
	}

	// Token: 0x06000824 RID: 2084 RVA: 0x000201BD File Offset: 0x0001E3BD
	public static Locale[] GetLoadOrder(bool isCardTexture = false)
	{
		return Localization.GetLoadOrder(Localization.s_instance.m_locale, isCardTexture);
	}

	// Token: 0x06000825 RID: 2085 RVA: 0x000201CF File Offset: 0x0001E3CF
	public static CultureInfo GetCultureInfo()
	{
		return Localization.s_instance.m_cultureInfo;
	}

	// Token: 0x06000826 RID: 2086 RVA: 0x000201DB File Offset: 0x0001E3DB
	public static bool IsValidLocaleName(string localeName)
	{
		return Enum.IsDefined(typeof(Locale), localeName);
	}

	// Token: 0x06000827 RID: 2087 RVA: 0x000201F0 File Offset: 0x0001E3F0
	public static bool IsValidLocaleName(string localeName, params Locale[] locales)
	{
		if (locales == null || locales.Length == 0)
		{
			return false;
		}
		foreach (Locale locale in locales)
		{
			string text = locale.ToString();
			if (localeName == text)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000828 RID: 2088 RVA: 0x00020240 File Offset: 0x0001E440
	public static bool IsForeignLocale(Locale locale)
	{
		return locale != Locale.enUS;
	}

	// Token: 0x06000829 RID: 2089 RVA: 0x0002024C File Offset: 0x0001E44C
	public static bool IsForeignLocaleName(string localeName)
	{
		Locale locale;
		try
		{
			locale = EnumUtils.Parse<Locale>(localeName);
		}
		catch (Exception)
		{
			return false;
		}
		return Localization.IsForeignLocale(locale);
	}

	// Token: 0x0600082A RID: 2090 RVA: 0x0002028C File Offset: 0x0001E48C
	public static List<Locale> GetForeignLocales()
	{
		List<Locale> list = Localization.s_instance.m_foreignLocales;
		if (list != null)
		{
			return list;
		}
		list = new List<Locale>();
		foreach (object obj in Enum.GetValues(typeof(Locale)))
		{
			Locale locale = (Locale)((int)obj);
			if (locale != Locale.UNKNOWN)
			{
				if (locale != Locale.enUS)
				{
					list.Add(locale);
				}
			}
		}
		Localization.s_instance.m_foreignLocales = list;
		return list;
	}

	// Token: 0x0600082B RID: 2091 RVA: 0x00020334 File Offset: 0x0001E534
	public static List<string> GetForeignLocaleNames()
	{
		List<string> list = Localization.s_instance.m_foreignLocaleNames;
		if (list != null)
		{
			return list;
		}
		list = new List<string>();
		foreach (string text in Enum.GetNames(typeof(Locale)))
		{
			if (!(text == Locale.UNKNOWN.ToString()))
			{
				if (!(text == Localization.DEFAULT_LOCALE_NAME))
				{
					list.Add(text);
				}
			}
		}
		Localization.s_instance.m_foreignLocaleNames = list;
		return list;
	}

	// Token: 0x0600082C RID: 2092 RVA: 0x000203C8 File Offset: 0x0001E5C8
	public static string ConvertLocaleToDotNet(Locale locale)
	{
		string localeName = locale.ToString();
		return Localization.ConvertLocaleToDotNet(localeName);
	}

	// Token: 0x0600082D RID: 2093 RVA: 0x000203E8 File Offset: 0x0001E5E8
	public static string ConvertLocaleToDotNet(string localeName)
	{
		string text = localeName.Substring(0, 2);
		string text2 = localeName.Substring(2, 2).ToUpper();
		return string.Format("{0}-{1}", text, text2);
	}

	// Token: 0x0600082E RID: 2094 RVA: 0x0002041C File Offset: 0x0001E61C
	public static bool DoesLocaleUseDecimalPoint(Locale locale)
	{
		switch (locale)
		{
		case Locale.enUS:
		case Locale.enGB:
		case Locale.koKR:
		case Locale.esMX:
		case Locale.zhTW:
		case Locale.zhCN:
		case Locale.jaJP:
		case Locale.thTH:
			return true;
		case Locale.frFR:
		case Locale.deDE:
		case Locale.esES:
		case Locale.ruRU:
		case Locale.itIT:
		case Locale.ptBR:
		case Locale.plPL:
			return false;
		}
		return true;
	}

	// Token: 0x0600082F RID: 2095 RVA: 0x0002047C File Offset: 0x0001E67C
	private void SetPegLocale(Locale locale)
	{
		string pegLocaleName = locale.ToString();
		this.SetPegLocaleName(pegLocaleName);
	}

	// Token: 0x06000830 RID: 2096 RVA: 0x0002049C File Offset: 0x0001E69C
	private void SetPegLocaleName(string localeName)
	{
		this.m_locale = EnumUtils.Parse<Locale>(localeName);
		string text = Localization.ConvertLocaleToDotNet(this.m_locale);
		this.m_cultureInfo = CultureInfo.CreateSpecificCulture(text);
	}

	// Token: 0x04000435 RID: 1077
	public const Locale DEFAULT_LOCALE = Locale.enUS;

	// Token: 0x04000436 RID: 1078
	public static readonly string DEFAULT_LOCALE_NAME = Locale.enUS.ToString();

	// Token: 0x04000437 RID: 1079
	public static readonly Map<Locale, Locale[]> LOAD_ORDERS;

	// Token: 0x04000438 RID: 1080
	private static Localization s_instance;

	// Token: 0x04000439 RID: 1081
	private Locale m_locale;

	// Token: 0x0400043A RID: 1082
	private CultureInfo m_cultureInfo;

	// Token: 0x0400043B RID: 1083
	private List<Locale> m_foreignLocales;

	// Token: 0x0400043C RID: 1084
	private List<string> m_foreignLocaleNames;

	// Token: 0x0400043D RID: 1085
	public static readonly PlatformDependentValue<bool> LOCALE_FROM_OPTIONS;
}
