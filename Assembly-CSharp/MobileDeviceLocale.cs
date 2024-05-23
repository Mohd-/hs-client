using System;
using System.Collections.Generic;
using bgs;
using UnityEngine;

// Token: 0x02000897 RID: 2199
public class MobileDeviceLocale
{
	// Token: 0x060053C4 RID: 21444 RVA: 0x00190DA4 File Offset: 0x0018EFA4
	public static constants.BnetRegion FindDevRegionByServerVersion(string version)
	{
		foreach (constants.BnetRegion bnetRegion in MobileDeviceLocale.s_regionIdToDevIP.Keys)
		{
			if (version == MobileDeviceLocale.s_regionIdToDevIP[bnetRegion].version)
			{
				return bnetRegion;
			}
		}
		return -1;
	}

	// Token: 0x060053C5 RID: 21445 RVA: 0x00190E24 File Offset: 0x0018F024
	public static constants.BnetRegion GetCurrentRegionId()
	{
		int @int = Options.Get().GetInt(Option.PREFERRED_REGION);
		if (@int < 0)
		{
			if (MobileDeviceLocale.UseClientConfigForEnv())
			{
				constants.BnetRegion bnetRegion = MobileDeviceLocale.FindDevRegionByServerVersion(Vars.Key("Aurora.Version.String").GetStr(string.Empty));
				Log.JMac.Print("Battle.net region from client.config version: " + bnetRegion, new object[0]);
				if (bnetRegion != -1)
				{
					return bnetRegion;
				}
			}
			try
			{
				if (!MobileDeviceLocale.s_countryCodeToRegionId.TryGetValue(MobileDeviceLocale.GetCountryCode(), out @int))
				{
					@int = MobileDeviceLocale.s_defaultRegionId;
				}
			}
			catch (Exception)
			{
			}
		}
		return @int;
	}

	// Token: 0x060053C6 RID: 21446 RVA: 0x00190ECC File Offset: 0x0018F0CC
	public static List<string> GetRegionCodesForCurrentRegionId()
	{
		List<string> list = new List<string>();
		int currentRegionId = MobileDeviceLocale.GetCurrentRegionId();
		foreach (KeyValuePair<string, int> keyValuePair in MobileDeviceLocale.s_countryCodeToRegionId)
		{
			if (keyValuePair.Value == currentRegionId)
			{
				list.Add(keyValuePair.Key);
			}
		}
		return list;
	}

	// Token: 0x060053C7 RID: 21447 RVA: 0x00190F48 File Offset: 0x0018F148
	public static MobileDeviceLocale.ConnectionData GetConnectionDataFromRegionId(constants.BnetRegion region, bool isDev)
	{
		MobileDeviceLocale.ConnectionData result;
		if (isDev)
		{
			if (!MobileDeviceLocale.s_regionIdToDevIP.TryGetValue(region, out result) && !MobileDeviceLocale.s_regionIdToDevIP.TryGetValue(MobileDeviceLocale.s_defaultDevRegion, out result))
			{
				Debug.LogError("Invalid region set for s_defaultDevRegion!  This should never happen!!!");
			}
		}
		else if (!MobileDeviceLocale.s_regionIdToProdIP.TryGetValue(region, out result))
		{
			result = MobileDeviceLocale.s_defaultProdIP;
		}
		return result;
	}

	// Token: 0x060053C8 RID: 21448 RVA: 0x00190FAC File Offset: 0x0018F1AC
	public static Locale GetBestGuessForLocale()
	{
		Locale result = Locale.enUS;
		bool flag = false;
		string text = MobileDeviceLocale.GetLanguageCode();
		try
		{
			flag = MobileDeviceLocale.s_languageCodeToLocale.TryGetValue(text, out result);
		}
		catch (Exception)
		{
		}
		if (!flag)
		{
			text = text.Substring(0, 2);
			try
			{
				flag = MobileDeviceLocale.s_languageCodeToLocale.TryGetValue(text, out result);
			}
			catch (Exception)
			{
			}
		}
		if (!flag)
		{
			int num = 1;
			string countryCode = MobileDeviceLocale.GetCountryCode();
			try
			{
				MobileDeviceLocale.s_countryCodeToRegionId.TryGetValue(countryCode, out num);
			}
			catch (Exception)
			{
			}
			string text2 = text;
			if (text2 != null)
			{
				if (MobileDeviceLocale.<>f__switch$map8B == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
					dictionary.Add("es", 0);
					dictionary.Add("zh", 1);
					MobileDeviceLocale.<>f__switch$map8B = dictionary;
				}
				int num2;
				if (MobileDeviceLocale.<>f__switch$map8B.TryGetValue(text2, ref num2))
				{
					if (num2 == 0)
					{
						if (num == 1)
						{
							result = Locale.esMX;
						}
						else
						{
							result = Locale.esES;
						}
						return result;
					}
					if (num2 == 1)
					{
						if (countryCode == "CN")
						{
							result = Locale.zhCN;
						}
						else
						{
							result = Locale.zhTW;
						}
						return result;
					}
				}
			}
			result = Locale.enUS;
		}
		return result;
	}

	// Token: 0x060053C9 RID: 21449 RVA: 0x001910F8 File Offset: 0x0018F2F8
	public static bool UseClientConfigForEnv()
	{
		bool flag = Vars.Key("Aurora.Env.Override").GetInt(0) != 0;
		bool flag2 = Vars.Key("Aurora.Env.DisableOverrideOnDevices").GetInt(0) != 0;
		if (flag2)
		{
			flag = false;
		}
		string str = Vars.Key("Aurora.Env").GetStr(string.Empty);
		bool flag3 = str != null && !(str == string.Empty);
		return flag && flag3;
	}

	// Token: 0x060053CA RID: 21450 RVA: 0x00191172 File Offset: 0x0018F372
	public static string GetCountryCode()
	{
		return MobileDeviceLocale.GetLocaleCountryCode();
	}

	// Token: 0x060053CB RID: 21451 RVA: 0x00191179 File Offset: 0x0018F379
	public static string GetLanguageCode()
	{
		return MobileDeviceLocale.GetLocaleLanguageCode();
	}

	// Token: 0x060053CC RID: 21452 RVA: 0x00191180 File Offset: 0x0018F380
	private static string GetLocaleCountryCode()
	{
		return string.Empty;
	}

	// Token: 0x060053CD RID: 21453 RVA: 0x00191187 File Offset: 0x0018F387
	private static string GetLocaleLanguageCode()
	{
		return string.Empty;
	}

	// Token: 0x040039EF RID: 14831
	public const string ptrServerVersion = "BETA";

	// Token: 0x040039F0 RID: 14832
	private static Map<string, Locale> s_languageCodeToLocale = new Map<string, Locale>
	{
		{
			"en",
			Locale.enUS
		},
		{
			"fr",
			Locale.frFR
		},
		{
			"de",
			Locale.deDE
		},
		{
			"ko",
			Locale.koKR
		},
		{
			"ru",
			Locale.ruRU
		},
		{
			"it",
			Locale.itIT
		},
		{
			"pt",
			Locale.ptBR
		},
		{
			"pl",
			Locale.plPL
		},
		{
			"ja",
			Locale.jaJP
		},
		{
			"th",
			Locale.thTH
		},
		{
			"en-AU",
			Locale.enUS
		},
		{
			"en-GB",
			Locale.enUS
		},
		{
			"fr-CA",
			Locale.frFR
		},
		{
			"es-MX",
			Locale.esMX
		},
		{
			"zh-Hans",
			Locale.zhCN
		},
		{
			"zh-Hant",
			Locale.zhTW
		},
		{
			"pt-PT",
			Locale.ptBR
		}
	};

	// Token: 0x040039F1 RID: 14833
	private static Map<string, int> s_countryCodeToRegionId = new Map<string, int>
	{
		{
			"AD",
			2
		},
		{
			"AE",
			2
		},
		{
			"AG",
			1
		},
		{
			"AL",
			2
		},
		{
			"AM",
			2
		},
		{
			"AO",
			2
		},
		{
			"AR",
			1
		},
		{
			"AT",
			2
		},
		{
			"AU",
			1
		},
		{
			"AZ",
			2
		},
		{
			"BA",
			2
		},
		{
			"BB",
			1
		},
		{
			"BD",
			1
		},
		{
			"BE",
			2
		},
		{
			"BF",
			2
		},
		{
			"BG",
			2
		},
		{
			"BH",
			2
		},
		{
			"BI",
			2
		},
		{
			"BJ",
			2
		},
		{
			"BM",
			2
		},
		{
			"BN",
			1
		},
		{
			"BO",
			1
		},
		{
			"BR",
			1
		},
		{
			"BS",
			1
		},
		{
			"BT",
			1
		},
		{
			"BW",
			2
		},
		{
			"BY",
			2
		},
		{
			"BZ",
			1
		},
		{
			"CA",
			1
		},
		{
			"CD",
			2
		},
		{
			"CF",
			2
		},
		{
			"CG",
			2
		},
		{
			"CH",
			2
		},
		{
			"CI",
			2
		},
		{
			"CL",
			1
		},
		{
			"CM",
			2
		},
		{
			"CN",
			3
		},
		{
			"CO",
			1
		},
		{
			"CR",
			1
		},
		{
			"CU",
			1
		},
		{
			"CV",
			2
		},
		{
			"CY",
			2
		},
		{
			"CZ",
			2
		},
		{
			"DE",
			2
		},
		{
			"DJ",
			2
		},
		{
			"DK",
			2
		},
		{
			"DM",
			1
		},
		{
			"DO",
			1
		},
		{
			"DZ",
			2
		},
		{
			"EC",
			1
		},
		{
			"EE",
			2
		},
		{
			"EG",
			2
		},
		{
			"ER",
			2
		},
		{
			"ES",
			2
		},
		{
			"ET",
			2
		},
		{
			"FI",
			2
		},
		{
			"FJ",
			1
		},
		{
			"FK",
			2
		},
		{
			"FO",
			2
		},
		{
			"FR",
			2
		},
		{
			"GA",
			2
		},
		{
			"GB",
			2
		},
		{
			"GD",
			1
		},
		{
			"GE",
			2
		},
		{
			"GL",
			2
		},
		{
			"GM",
			2
		},
		{
			"GN",
			2
		},
		{
			"GQ",
			2
		},
		{
			"GR",
			2
		},
		{
			"GS",
			2
		},
		{
			"GT",
			1
		},
		{
			"GW",
			2
		},
		{
			"GY",
			1
		},
		{
			"HK",
			3
		},
		{
			"HN",
			1
		},
		{
			"HR",
			2
		},
		{
			"HT",
			1
		},
		{
			"HU",
			2
		},
		{
			"ID",
			1
		},
		{
			"IE",
			2
		},
		{
			"IL",
			2
		},
		{
			"IM",
			2
		},
		{
			"IN",
			1
		},
		{
			"IQ",
			2
		},
		{
			"IR",
			2
		},
		{
			"IS",
			2
		},
		{
			"IT",
			2
		},
		{
			"JM",
			1
		},
		{
			"JO",
			2
		},
		{
			"JP",
			3
		},
		{
			"KE",
			2
		},
		{
			"KG",
			2
		},
		{
			"KH",
			2
		},
		{
			"KI",
			1
		},
		{
			"KM",
			2
		},
		{
			"KP",
			1
		},
		{
			"KR",
			3
		},
		{
			"KW",
			2
		},
		{
			"KY",
			2
		},
		{
			"KZ",
			2
		},
		{
			"LA",
			1
		},
		{
			"LB",
			2
		},
		{
			"LC",
			1
		},
		{
			"LI",
			2
		},
		{
			"LK",
			1
		},
		{
			"LR",
			2
		},
		{
			"LS",
			2
		},
		{
			"LT",
			2
		},
		{
			"LU",
			2
		},
		{
			"LV",
			2
		},
		{
			"LY",
			2
		},
		{
			"MA",
			2
		},
		{
			"MC",
			2
		},
		{
			"MD",
			2
		},
		{
			"ME",
			2
		},
		{
			"MG",
			2
		},
		{
			"MK",
			2
		},
		{
			"ML",
			2
		},
		{
			"MM",
			1
		},
		{
			"MN",
			2
		},
		{
			"MO",
			3
		},
		{
			"MR",
			2
		},
		{
			"MT",
			2
		},
		{
			"MU",
			2
		},
		{
			"MV",
			2
		},
		{
			"MW",
			2
		},
		{
			"MX",
			1
		},
		{
			"MY",
			1
		},
		{
			"MZ",
			2
		},
		{
			"NA",
			2
		},
		{
			"NC",
			2
		},
		{
			"NE",
			2
		},
		{
			"NG",
			2
		},
		{
			"NI",
			1
		},
		{
			"NL",
			2
		},
		{
			"NO",
			2
		},
		{
			"NP",
			1
		},
		{
			"NR",
			1
		},
		{
			"NZ",
			1
		},
		{
			"OM",
			2
		},
		{
			"PA",
			1
		},
		{
			"PE",
			1
		},
		{
			"PF",
			1
		},
		{
			"PG",
			1
		},
		{
			"PH",
			1
		},
		{
			"PK",
			2
		},
		{
			"PL",
			2
		},
		{
			"PT",
			2
		},
		{
			"PY",
			1
		},
		{
			"QA",
			2
		},
		{
			"RO",
			2
		},
		{
			"RS",
			2
		},
		{
			"RU",
			2
		},
		{
			"RW",
			2
		},
		{
			"SA",
			2
		},
		{
			"SB",
			1
		},
		{
			"SC",
			2
		},
		{
			"SD",
			2
		},
		{
			"SE",
			2
		},
		{
			"SG",
			1
		},
		{
			"SH",
			2
		},
		{
			"SI",
			2
		},
		{
			"SK",
			2
		},
		{
			"SL",
			2
		},
		{
			"SN",
			2
		},
		{
			"SO",
			2
		},
		{
			"SR",
			2
		},
		{
			"ST",
			2
		},
		{
			"SV",
			1
		},
		{
			"SY",
			2
		},
		{
			"SZ",
			2
		},
		{
			"TD",
			2
		},
		{
			"TG",
			2
		},
		{
			"TH",
			1
		},
		{
			"TJ",
			2
		},
		{
			"TL",
			1
		},
		{
			"TM",
			2
		},
		{
			"TN",
			2
		},
		{
			"TO",
			1
		},
		{
			"TR",
			2
		},
		{
			"TT",
			1
		},
		{
			"TV",
			1
		},
		{
			"TW",
			3
		},
		{
			"TZ",
			2
		},
		{
			"UA",
			2
		},
		{
			"UG",
			2
		},
		{
			"US",
			1
		},
		{
			"UY",
			1
		},
		{
			"UZ",
			2
		},
		{
			"VA",
			2
		},
		{
			"VC",
			1
		},
		{
			"VE",
			1
		},
		{
			"VN",
			1
		},
		{
			"VU",
			1
		},
		{
			"WS",
			1
		},
		{
			"YE",
			2
		},
		{
			"YU",
			2
		},
		{
			"ZA",
			2
		},
		{
			"ZM",
			2
		},
		{
			"ZW",
			2
		}
	};

	// Token: 0x040039F2 RID: 14834
	private static int s_defaultRegionId = 1;

	// Token: 0x040039F3 RID: 14835
	private static Map<constants.BnetRegion, MobileDeviceLocale.ConnectionData> s_regionIdToProdIP = new Map<constants.BnetRegion, MobileDeviceLocale.ConnectionData>
	{
		{
			0,
			new MobileDeviceLocale.ConnectionData
			{
				address = "us.actual.battle.net",
				port = 1119,
				version = "product"
			}
		},
		{
			1,
			new MobileDeviceLocale.ConnectionData
			{
				address = "us.actual.battle.net",
				port = 1119,
				version = "product"
			}
		},
		{
			2,
			new MobileDeviceLocale.ConnectionData
			{
				address = "hearthmod.com",
				port = 1119,
				version = "product"
			}
		},
		{
			3,
			new MobileDeviceLocale.ConnectionData
			{
				address = "kr.actual.battle.net",
				port = 1119,
				version = "product"
			}
		},
		{
			4,
			new MobileDeviceLocale.ConnectionData
			{
				address = "kr.actual.battle.net",
				port = 1119,
				version = "product"
			}
		},
		{
			5,
			new MobileDeviceLocale.ConnectionData
			{
				address = "cn.actual.battle.net",
				port = 1119,
				version = "product"
			}
		},
		{
			41,
			new MobileDeviceLocale.ConnectionData
			{
				address = "beta.actual.battle.net",
				port = 1119,
				version = "LOC"
			}
		}
	};

	// Token: 0x040039F4 RID: 14836
	private static MobileDeviceLocale.ConnectionData s_defaultProdIP = new MobileDeviceLocale.ConnectionData
	{
		address = "us.actual.battle.net",
		port = 1119,
		version = "product"
	};

	// Token: 0x040039F5 RID: 14837
	public static Map<constants.BnetRegion, MobileDeviceLocale.ConnectionData> s_regionIdToDevIP = new Map<constants.BnetRegion, MobileDeviceLocale.ConnectionData>
	{
		{
			1,
			new MobileDeviceLocale.ConnectionData
			{
				name = "bn11-dev",
				address = "bn11-01.battle.net",
				port = 1119,
				version = "dev11",
				tutorialPort = 45005
			}
		},
		{
			60,
			new MobileDeviceLocale.ConnectionData
			{
				name = "bn11-qa1",
				address = "bn11-01.battle.net",
				port = 1119,
				version = "qa1",
				tutorialPort = 45008
			}
		},
		{
			61,
			new MobileDeviceLocale.ConnectionData
			{
				name = "bn11-qa2",
				address = "bn11-01.battle.net",
				port = 1119,
				version = "qa2",
				tutorialPort = 45009
			}
		},
		{
			62,
			new MobileDeviceLocale.ConnectionData
			{
				name = "bn11-qa3",
				address = "bn11-01.battle.net",
				port = 1119,
				version = "qa3",
				tutorialPort = 45038
			}
		},
		{
			63,
			new MobileDeviceLocale.ConnectionData
			{
				name = "bn11-eu",
				address = "bn11-01.battle.net",
				port = 11122,
				version = "eu11",
				tutorialPort = 45006
			}
		},
		{
			64,
			new MobileDeviceLocale.ConnectionData
			{
				name = "bn11-kr",
				address = "bn11-01.battle.net",
				port = 11123,
				version = "kr11",
				tutorialPort = 45007
			}
		},
		{
			65,
			new MobileDeviceLocale.ConnectionData
			{
				name = "bn11-rc",
				address = "bn11-01.battle.net",
				port = 1119,
				version = "product",
				tutorialPort = 45025
			}
		},
		{
			66,
			new MobileDeviceLocale.ConnectionData
			{
				name = "bn12-dev",
				address = "bn12-01.battle.net",
				port = 1119,
				version = "dev12",
				tutorialPort = 45012
			}
		},
		{
			67,
			new MobileDeviceLocale.ConnectionData
			{
				name = "bn12-qa4",
				address = "bn12-01.battle.net",
				port = 1119,
				version = "qa4",
				tutorialPort = 45013
			}
		},
		{
			68,
			new MobileDeviceLocale.ConnectionData
			{
				name = "bn12-qa5",
				address = "bn12-01.battle.net",
				port = 1119,
				version = "qa5",
				tutorialPort = 45014
			}
		},
		{
			69,
			new MobileDeviceLocale.ConnectionData
			{
				name = "bn12-qa6",
				address = "bn12-01.battle.net",
				port = 1119,
				version = "qa6",
				tutorialPort = 45039
			}
		},
		{
			70,
			new MobileDeviceLocale.ConnectionData
			{
				name = "bn12-rc",
				address = "bn12-01.battle.net",
				port = 1119,
				version = "product",
				tutorialPort = 45028
			}
		},
		{
			71,
			new MobileDeviceLocale.ConnectionData
			{
				name = "bn8-dev",
				address = "bn8-01.battle.net",
				port = 1119,
				version = "dev8",
				tutorialPort = 45021
			}
		},
		{
			72,
			new MobileDeviceLocale.ConnectionData
			{
				name = "bn8-qa7",
				address = "bn8-01.battle.net",
				port = 1119,
				version = "qa7",
				tutorialPort = 45022
			}
		},
		{
			73,
			new MobileDeviceLocale.ConnectionData
			{
				name = "bn8-qa8",
				address = "bn8-01.battle.net",
				port = 1119,
				version = "qa8",
				tutorialPort = 45036
			}
		},
		{
			74,
			new MobileDeviceLocale.ConnectionData
			{
				name = "bn8-qa9",
				address = "bn8-01.battle.net",
				port = 1119,
				version = "qa9",
				tutorialPort = 45037
			}
		},
		{
			75,
			new MobileDeviceLocale.ConnectionData
			{
				name = "bn8-eu",
				address = "bn8-01.battle.net",
				port = 11122,
				version = "eu8",
				tutorialPort = 45000
			}
		},
		{
			76,
			new MobileDeviceLocale.ConnectionData
			{
				name = "bn8-kr",
				address = "bn8-01.battle.net",
				port = 11123,
				version = "kr8",
				tutorialPort = 45001
			}
		},
		{
			77,
			new MobileDeviceLocale.ConnectionData
			{
				name = "bn8-rc",
				address = "bn8-01.battle.net",
				port = 1119,
				version = "product",
				tutorialPort = 45023
			}
		},
		{
			78,
			new MobileDeviceLocale.ConnectionData
			{
				name = "qa-cn",
				address = "st5-bn-front01.battle.net",
				port = 1119,
				version = "qacn",
				tutorialPort = 45049
			}
		},
		{
			52,
			new MobileDeviceLocale.ConnectionData
			{
				name = "bn11-mschweitzer",
				address = "bn11-01.battle.net",
				port = 1119,
				version = "mschweitzer"
			}
		},
		{
			53,
			new MobileDeviceLocale.ConnectionData
			{
				name = "bn12-mschweitzer",
				address = "bn12-01.battle.net",
				port = 1119,
				version = "mschweitzer"
			}
		}
	};

	// Token: 0x040039F6 RID: 14838
	private static constants.BnetRegion s_defaultDevRegion = 1;

	// Token: 0x020008A4 RID: 2212
	public struct ConnectionData
	{
		// Token: 0x04003A3C RID: 14908
		public string address;

		// Token: 0x04003A3D RID: 14909
		public int port;

		// Token: 0x04003A3E RID: 14910
		public string version;

		// Token: 0x04003A3F RID: 14911
		public string name;

		// Token: 0x04003A40 RID: 14912
		public int tutorialPort;
	}
}
