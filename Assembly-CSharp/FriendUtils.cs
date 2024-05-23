using System;
using System.Collections.Generic;
using bgs;

// Token: 0x020004A8 RID: 1192
public class FriendUtils
{
	// Token: 0x060038D8 RID: 14552 RVA: 0x00115858 File Offset: 0x00113A58
	public static string GetUniqueName(BnetPlayer friend)
	{
		BnetBattleTag bnetBattleTag;
		string result;
		if (FriendUtils.GetUniqueName(friend, out bnetBattleTag, out result))
		{
			return bnetBattleTag.ToString();
		}
		return result;
	}

	// Token: 0x060038D9 RID: 14553 RVA: 0x0011587C File Offset: 0x00113A7C
	public static string GetUniqueNameWithColor(BnetPlayer friend)
	{
		string text = (!friend.IsOnline()) ? "999999ff" : "5ecaf0ff";
		BnetBattleTag battleTag;
		string text2;
		if (FriendUtils.GetUniqueName(friend, out battleTag, out text2))
		{
			return FriendUtils.GetBattleTagWithColor(battleTag, text);
		}
		return string.Format("<color=#{0}>{1}</color>", text, text2);
	}

	// Token: 0x060038DA RID: 14554 RVA: 0x001158C8 File Offset: 0x00113AC8
	public static string GetBattleTagWithColor(BnetBattleTag battleTag, string nameColorStr)
	{
		return string.Format("<color=#{0}>{1}</color><color=#{2}>#{3}</color>", new object[]
		{
			nameColorStr,
			battleTag.GetName(),
			"a1a1a1ff",
			battleTag.GetNumber()
		});
	}

	// Token: 0x060038DB RID: 14555 RVA: 0x00115908 File Offset: 0x00113B08
	public static string GetFriendListName(BnetPlayer friend, bool addColorTags)
	{
		string text = null;
		BnetAccount account = friend.GetAccount();
		if (account != null)
		{
			text = account.GetFullName();
			if (text == null && account.GetBattleTag() != null)
			{
				text = account.GetBattleTag().ToString();
			}
		}
		if (text == null)
		{
			foreach (KeyValuePair<BnetGameAccountId, BnetGameAccount> keyValuePair in friend.GetGameAccounts())
			{
				if (keyValuePair.Value.GetBattleTag() != null)
				{
					text = keyValuePair.Value.GetBattleTag().ToString();
					break;
				}
			}
		}
		if (addColorTags)
		{
			string text2 = (!friend.IsOnline()) ? "999999ff" : "5ecaf0ff";
			return string.Format("<color=#{0}>{1}</color>", text2, text);
		}
		return text;
	}

	// Token: 0x060038DC RID: 14556 RVA: 0x00115A00 File Offset: 0x00113C00
	public static string GetRequestElapsedTimeString(ulong epochMicrosec)
	{
		TimeUtils.ElapsedStringSet stringSet = new TimeUtils.ElapsedStringSet
		{
			m_seconds = "GLOBAL_DATETIME_FRIENDREQUEST_SECONDS",
			m_minutes = "GLOBAL_DATETIME_FRIENDREQUEST_MINUTES",
			m_hours = "GLOBAL_DATETIME_FRIENDREQUEST_HOURS",
			m_yesterday = "GLOBAL_DATETIME_FRIENDREQUEST_DAY",
			m_days = "GLOBAL_DATETIME_FRIENDREQUEST_DAYS",
			m_weeks = "GLOBAL_DATETIME_FRIENDREQUEST_WEEKS",
			m_monthAgo = "GLOBAL_DATETIME_FRIENDREQUEST_MONTH"
		};
		return TimeUtils.GetElapsedTimeStringFromEpochMicrosec(epochMicrosec, stringSet);
	}

	// Token: 0x060038DD RID: 14557 RVA: 0x00115A6C File Offset: 0x00113C6C
	public static string GetLastOnlineElapsedTimeString(ulong epochMicrosec)
	{
		if (epochMicrosec == 0UL)
		{
			return GameStrings.Get("GLOBAL_OFFLINE");
		}
		TimeUtils.ElapsedStringSet stringSet = new TimeUtils.ElapsedStringSet
		{
			m_seconds = "GLOBAL_DATETIME_LASTONLINE_SECONDS",
			m_minutes = "GLOBAL_DATETIME_LASTONLINE_MINUTES",
			m_hours = "GLOBAL_DATETIME_LASTONLINE_HOURS",
			m_yesterday = "GLOBAL_DATETIME_LASTONLINE_DAY",
			m_days = "GLOBAL_DATETIME_LASTONLINE_DAYS",
			m_weeks = "GLOBAL_DATETIME_LASTONLINE_WEEKS",
			m_monthAgo = "GLOBAL_DATETIME_LASTONLINE_MONTH"
		};
		return TimeUtils.GetElapsedTimeStringFromEpochMicrosec(epochMicrosec, stringSet);
	}

	// Token: 0x060038DE RID: 14558 RVA: 0x00115AE8 File Offset: 0x00113CE8
	public static string GetAwayTimeString(ulong epochMicrosec)
	{
		TimeUtils.ElapsedStringSet stringSet = new TimeUtils.ElapsedStringSet
		{
			m_seconds = "GLOBAL_DATETIME_AFK_SECONDS",
			m_minutes = "GLOBAL_DATETIME_AFK_MINUTES",
			m_hours = "GLOBAL_DATETIME_AFK_HOURS",
			m_yesterday = "GLOBAL_DATETIME_AFK_DAY",
			m_days = "GLOBAL_DATETIME_AFK_DAYS",
			m_weeks = "GLOBAL_DATETIME_AFK_WEEKS",
			m_monthAgo = "GLOBAL_DATETIME_AFK_MONTH"
		};
		return TimeUtils.GetElapsedTimeStringFromEpochMicrosec(epochMicrosec, stringSet);
	}

	// Token: 0x060038DF RID: 14559 RVA: 0x00115B54 File Offset: 0x00113D54
	public static int FriendSortCompare(BnetPlayer friend1, BnetPlayer friend2)
	{
		int result = 0;
		if (friend1 == null || friend2 == null)
		{
			return (friend1 != friend2) ? ((friend1 != null) ? -1 : 1) : 0;
		}
		if (!friend1.IsOnline() && !friend2.IsOnline())
		{
			return FriendUtils.FriendNameSortCompare(friend1, friend2);
		}
		if (friend1.IsOnline() && !friend2.IsOnline())
		{
			return -1;
		}
		if (!friend1.IsOnline() && friend2.IsOnline())
		{
			return 1;
		}
		BnetProgramId bestProgramId = friend1.GetBestProgramId();
		BnetProgramId bestProgramId2 = friend2.GetBestProgramId();
		if (FriendUtils.FriendSortFlagCompare(friend1, friend2, bestProgramId == BnetProgramId.HEARTHSTONE, bestProgramId2 == BnetProgramId.HEARTHSTONE, out result))
		{
			return result;
		}
		bool lhsflag = !(bestProgramId == null) && bestProgramId.IsGame();
		bool rhsflag = !(bestProgramId2 == null) && bestProgramId2.IsGame();
		if (FriendUtils.FriendSortFlagCompare(friend1, friend2, lhsflag, rhsflag, out result))
		{
			return result;
		}
		bool lhsflag2 = !(bestProgramId == null) && bestProgramId.IsPhoenix();
		bool rhsflag2 = !(bestProgramId2 == null) && bestProgramId2.IsPhoenix();
		if (FriendUtils.FriendSortFlagCompare(friend1, friend2, lhsflag2, rhsflag2, out result))
		{
			return result;
		}
		return FriendUtils.FriendNameSortCompare(friend1, friend2);
	}

	// Token: 0x060038E0 RID: 14560 RVA: 0x00115CA4 File Offset: 0x00113EA4
	public static bool IsValidEmail(string emailString)
	{
		if (emailString == null)
		{
			return false;
		}
		int num = emailString.IndexOf('@');
		if (num >= 1 && num < emailString.Length - 1)
		{
			int num2 = emailString.LastIndexOf('.');
			if (num2 > num + 1 && num2 < emailString.Length - 1)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060038E1 RID: 14561 RVA: 0x00115CFC File Offset: 0x00113EFC
	private static bool GetUniqueName(BnetPlayer friend, out BnetBattleTag battleTag, out string name)
	{
		battleTag = friend.GetBattleTag();
		name = friend.GetBestName();
		if (battleTag == null)
		{
			return false;
		}
		if (BnetNearbyPlayerMgr.Get().IsNearbyStranger(friend))
		{
			return true;
		}
		foreach (BnetPlayer bnetPlayer in BnetFriendMgr.Get().GetFriends())
		{
			if (bnetPlayer != friend)
			{
				string bestName = bnetPlayer.GetBestName();
				if (string.Compare(name, bestName, true) == 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060038E2 RID: 14562 RVA: 0x00115DB0 File Offset: 0x00113FB0
	private static bool FriendSortFlagCompare(BnetPlayer lhs, BnetPlayer rhs, bool lhsflag, bool rhsflag, out int result)
	{
		if (lhsflag && !rhsflag)
		{
			result = -1;
			return true;
		}
		if (!lhsflag && rhsflag)
		{
			result = 1;
			return true;
		}
		result = 0;
		return false;
	}

	// Token: 0x060038E3 RID: 14563 RVA: 0x00115DDC File Offset: 0x00113FDC
	private static int FriendNameSortCompare(BnetPlayer friend1, BnetPlayer friend2)
	{
		int num = string.Compare(FriendUtils.GetFriendListName(friend1, false), FriendUtils.GetFriendListName(friend2, false), true);
		if (num != 0)
		{
			return num;
		}
		long lo = (long)friend1.GetAccountId().GetLo();
		long lo2 = (long)friend2.GetAccountId().GetLo();
		return (int)(lo - lo2);
	}
}
