using System;
using bgs;
using UnityEngine;

// Token: 0x02000AF1 RID: 2801
public class StoreURL
{
	// Token: 0x06006062 RID: 24674 RVA: 0x001CE258 File Offset: 0x001CC458
	public StoreURL(string format, StoreURL.Param param1, StoreURL.Param param2)
	{
		this.m_format = format;
		this.m_param1 = param1;
		this.m_param2 = param2;
	}

	// Token: 0x06006064 RID: 24676 RVA: 0x001CE2DA File Offset: 0x001CC4DA
	public string GetURL()
	{
		return string.Format(this.m_format, this.GetParamString(this.m_param1), this.GetParamString(this.m_param2));
	}

	// Token: 0x06006065 RID: 24677 RVA: 0x001CE300 File Offset: 0x001CC500
	private string GetParamString(StoreURL.Param paramType)
	{
		switch (paramType)
		{
		case StoreURL.Param.LOCALE:
		{
			Locale locale = Localization.GetLocale();
			return locale.ToString();
		}
		case StoreURL.Param.REGION:
		{
			constants.BnetRegion accountRegion = BattleNet.GetAccountRegion();
			if (StoreURL.s_regionToStrMap.ContainsKey(accountRegion))
			{
				return StoreURL.s_regionToStrMap[accountRegion];
			}
			Debug.LogError(string.Format("StoreURL unrecognized region {0}", accountRegion));
			return StoreURL.s_regionToStrMap[1];
		}
		}
		return string.Empty;
	}

	// Token: 0x040047F1 RID: 18417
	private static readonly Map<constants.BnetRegion, string> s_regionToStrMap = new Map<constants.BnetRegion, string>
	{
		{
			1,
			"US"
		},
		{
			2,
			"EU"
		},
		{
			3,
			"KR"
		},
		{
			4,
			"TW"
		},
		{
			5,
			"CN"
		},
		{
			98,
			"US"
		}
	};

	// Token: 0x040047F2 RID: 18418
	private string m_format;

	// Token: 0x040047F3 RID: 18419
	private StoreURL.Param m_param1;

	// Token: 0x040047F4 RID: 18420
	private StoreURL.Param m_param2;

	// Token: 0x02000AF2 RID: 2802
	public enum Param
	{
		// Token: 0x040047F6 RID: 18422
		NONE,
		// Token: 0x040047F7 RID: 18423
		LOCALE,
		// Token: 0x040047F8 RID: 18424
		REGION
	}
}
