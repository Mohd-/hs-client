using System;
using System.Collections.Generic;
using bgs;
using UnityEngine;

// Token: 0x0200042C RID: 1068
public class NydusLink
{
	// Token: 0x0600360C RID: 13836 RVA: 0x0010B165 File Offset: 0x00109365
	public static string GetBreakingNewsLink()
	{
		return NydusLink.GetLink("alert", true);
	}

	// Token: 0x0600360D RID: 13837 RVA: 0x0010B172 File Offset: 0x00109372
	public static string GetAccountCreationLink()
	{
		return NydusLink.GetLink("creation", true);
	}

	// Token: 0x0600360E RID: 13838 RVA: 0x0010B180 File Offset: 0x00109380
	public static string GetLink(string linkType, bool isMobile)
	{
		string text;
		string text2;
		string text3;
		NydusLink.GetLocalizedLinkVars(out text, out text2, out text3);
		string text4 = (!isMobile) ? string.Empty : "-mobile";
		return string.Format("{0}/WTCG/{1}/client{2}/{3}?targetRegion={4}", new object[]
		{
			text,
			text2,
			text4,
			linkType,
			text3
		});
	}

	// Token: 0x0600360F RID: 13839 RVA: 0x0010B1D4 File Offset: 0x001093D4
	public static string GetSupportLink(string linkType, bool isMobile)
	{
		string text;
		string text2;
		string text3;
		NydusLink.GetLocalizedLinkVars(out text, out text2, out text3);
		string text4 = (!isMobile) ? string.Empty : "-mobile";
		return string.Format("{0}/WTCG/{1}/client{2}/support/{3}?targetRegion={4}", new object[]
		{
			text,
			text2,
			text4,
			linkType,
			text3
		});
	}

	// Token: 0x06003610 RID: 13840 RVA: 0x0010B228 File Offset: 0x00109428
	private static void GetLocalizedLinkVars(out string baseUrl, out string localeString, out string regionString)
	{
		localeString = Localization.GetLocaleName();
		bool flag = ApplicationMgr.GetMobileEnvironment() == MobileEnv.DEVELOPMENT || ApplicationMgr.IsInternal();
		constants.BnetRegion currentRegionId = MobileDeviceLocale.GetCurrentRegionId();
		MobileDeviceLocale.ConnectionData connectionDataFromRegionId = MobileDeviceLocale.GetConnectionDataFromRegionId(currentRegionId, flag);
		try
		{
			regionString = NydusLink.TargetServerToRegion[connectionDataFromRegionId.address];
		}
		catch (KeyNotFoundException)
		{
			Debug.LogWarning("No matching region found for " + connectionDataFromRegionId.address + " to get Nydus Link");
			regionString = "US";
		}
		baseUrl = ((!flag) ? NydusLink.ProdHost : NydusLink.DevHost);
	}

	// Token: 0x04002190 RID: 8592
	private static string DevHost = "https://nydus-qa.web.blizzard.net";

	// Token: 0x04002191 RID: 8593
	private static string ProdHost = "https://nydus.battle.net";

	// Token: 0x04002192 RID: 8594
	private static readonly Map<string, string> TargetServerToRegion = new Map<string, string>
	{
		{
			"us.actual.battle.net",
			"US"
		},
		{
			"hearthmod.com",
			"EU"
		},
		{
			"kr.actual.battle.net",
			"KR"
		},
		{
			"cn.actual.battle.net",
			"CN"
		},
		{
			"bn11-01.battle.net",
			"US"
		}
	};
}
