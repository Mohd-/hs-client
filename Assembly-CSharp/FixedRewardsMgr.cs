using System;
using System.Collections;
using System.Collections.Generic;
using PegasusShared;
using UnityEngine;

// Token: 0x0200001E RID: 30
public class FixedRewardsMgr
{
	// Token: 0x060002C0 RID: 704 RVA: 0x0000CED4 File Offset: 0x0000B0D4
	public static void Initialize()
	{
		FixedRewardsMgr.Get();
		AccountLicenseMgr.Get().RegisterAccountLicensesChangedListener(new AccountLicenseMgr.AccountLicensesChangedCallback(FixedRewardsMgr.s_instance.OnAccountLicensesUpdate));
	}

	// Token: 0x060002C1 RID: 705 RVA: 0x0000CEF8 File Offset: 0x0000B0F8
	public static FixedRewardsMgr Get()
	{
		if (FixedRewardsMgr.s_instance == null)
		{
			FixedRewardsMgr.s_instance = new FixedRewardsMgr();
			ApplicationMgr.Get().WillReset += new Action(FixedRewardsMgr.s_instance.WillReset);
		}
		if (!FixedRewardsMgr.s_instance.m_registeredForAdventureProgressUpdates)
		{
			FixedRewardsMgr.s_instance.m_registeredForAdventureProgressUpdates = AdventureProgressMgr.Get().RegisterProgressUpdatedListener(new AdventureProgressMgr.AdventureProgressUpdatedCallback(FixedRewardsMgr.s_instance.OnAdventureProgressUpdate));
		}
		if (!FixedRewardsMgr.s_instance.m_registeredForProfileNotices)
		{
			NetCache.Get().RegisterNewNoticesListener(new NetCache.DelNewNoticesListener(FixedRewardsMgr.s_instance.OnNewNotices));
			FixedRewardsMgr.s_instance.m_registeredForProfileNotices = true;
		}
		if (!FixedRewardsMgr.s_instance.m_registeredForCompletedAchieves)
		{
			AchieveManager.Get().RegisterNewlyCompletedAchievesListener(new AchieveManager.NewlyCompletedAchievesCallback(FixedRewardsMgr.s_instance.OnNewlyCompletedAchieves));
			FixedRewardsMgr.s_instance.m_registeredForCompletedAchieves = true;
		}
		return FixedRewardsMgr.s_instance;
	}

	// Token: 0x060002C2 RID: 706 RVA: 0x0000CFD4 File Offset: 0x0000B1D4
	public void CheckForTutorialComplete()
	{
		List<CardRewardData> cardRewards = new List<CardRewardData>();
		this.CheckForTutorialComplete(cardRewards);
		CollectionManager.Get().AddCardRewards(cardRewards, false);
	}

	// Token: 0x060002C3 RID: 707 RVA: 0x0000CFFC File Offset: 0x0000B1FC
	public void CheckForTutorialComplete(List<CardRewardData> cardRewards)
	{
		NetCache.NetCacheProfileProgress netObject = NetCache.Get().GetNetObject<NetCache.NetCacheProfileProgress>();
		if (netObject == null)
		{
			Debug.LogWarning(string.Format("FixedRewardsMgr.CheckForTutorialComplete(): null == NetCache.NetCacheProfileProgress", new object[0]));
		}
		else
		{
			this.TriggerTutorialProgressAction(FixedRewardsMgr.ShowVisualOption.DO_NOT_SHOW, (int)netObject.CampaignProgress, cardRewards);
		}
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x0000D043 File Offset: 0x0000B243
	public bool IsStartupFinished()
	{
		return this.m_isStartupFinished;
	}

	// Token: 0x060002C5 RID: 709 RVA: 0x0000D04C File Offset: 0x0000B24C
	public void InitStartupFixedRewards()
	{
		List<CardRewardData> cardRewards = new List<CardRewardData>();
		List<AdventureMission.WingProgress> allProgress = AdventureProgressMgr.Get().GetAllProgress();
		this.m_isInitialization = true;
		foreach (AdventureMission.WingProgress wingProgress in allProgress)
		{
			if (wingProgress.MeetsFlagsRequirement(1UL))
			{
				this.TriggerWingProgressAction(FixedRewardsMgr.ShowVisualOption.DO_NOT_SHOW, wingProgress.Wing, wingProgress.Progress, cardRewards);
				this.TriggerWingFlagsAction(FixedRewardsMgr.ShowVisualOption.DO_NOT_SHOW, wingProgress.Wing, wingProgress.Flags, cardRewards);
			}
		}
		this.GrantAchieveRewards(cardRewards);
		this.m_isInitialization = false;
		this.CheckForTutorialComplete(cardRewards);
		this.GrantHeroLevelRewards(cardRewards);
		List<AccountLicenseInfo> allOwnedAccountLicenseInfo = AccountLicenseMgr.Get().GetAllOwnedAccountLicenseInfo();
		foreach (AccountLicenseInfo accountLicenseInfo in allOwnedAccountLicenseInfo)
		{
			this.TriggerAccountLicenseFlagsAction(FixedRewardsMgr.ShowVisualOption.DO_NOT_SHOW, accountLicenseInfo.License, accountLicenseInfo.Flags_, cardRewards);
		}
		CollectionManager.Get().AddCardRewards(cardRewards, false);
		this.m_isStartupFinished = true;
		CollectionManager.Get().FixedRewardsStartupComplete();
	}

	// Token: 0x060002C6 RID: 710 RVA: 0x0000D188 File Offset: 0x0000B388
	public bool ShowFixedRewards(UserAttentionBlocker blocker, HashSet<RewardVisualTiming> rewardVisualTimings, FixedRewardsMgr.DelOnAllFixedRewardsShown allRewardsShownCallback, FixedRewardsMgr.DelPositionNonToastReward positionNonToastRewardCallback, Vector3 rewardPunchScale, Vector3 rewardScale)
	{
		return this.ShowFixedRewards(blocker, rewardVisualTimings, allRewardsShownCallback, positionNonToastRewardCallback, rewardPunchScale, rewardScale, null);
	}

	// Token: 0x060002C7 RID: 711 RVA: 0x0000D1A8 File Offset: 0x0000B3A8
	public bool ShowFixedRewards(UserAttentionBlocker blocker, HashSet<RewardVisualTiming> rewardVisualTimings, FixedRewardsMgr.DelOnAllFixedRewardsShown allRewardsShownCallback, FixedRewardsMgr.DelPositionNonToastReward positionNonToastRewardCallback, Vector3 rewardPunchScale, Vector3 rewardScale, object userData)
	{
		if (!UserAttentionManager.CanShowAttentionGrabber(blocker, "FixedRewardsMgr.ShowFixedRewards:" + blocker))
		{
			return false;
		}
		FixedRewardsMgr.OnAllFixedRewardsShownCallbackInfo onAllFixedRewardsShownCallbackInfo = new FixedRewardsMgr.OnAllFixedRewardsShownCallbackInfo
		{
			m_rewardMapIDsToShow = new List<FixedRewardsMgr.RewardMapIDToShow>(),
			m_onAllRewardsShownCallback = allRewardsShownCallback,
			m_positionNonToastRewardCallback = positionNonToastRewardCallback,
			m_rewardPunchScale = rewardPunchScale,
			m_rewardScale = rewardScale,
			m_userData = userData
		};
		foreach (RewardVisualTiming key in rewardVisualTimings)
		{
			if (this.m_rewardMapIDsToShow.ContainsKey(key))
			{
				onAllFixedRewardsShownCallbackInfo.m_rewardMapIDsToShow.AddRange(this.m_rewardMapIDsToShow[key]);
				this.m_rewardMapIDsToShow[key].Clear();
			}
		}
		if (onAllFixedRewardsShownCallbackInfo.m_rewardMapIDsToShow.Count == 0)
		{
			return false;
		}
		onAllFixedRewardsShownCallbackInfo.m_rewardMapIDsToShow.Sort(delegate(FixedRewardsMgr.RewardMapIDToShow a, FixedRewardsMgr.RewardMapIDToShow b)
		{
			if (a.SortOrder < b.SortOrder)
			{
				return -1;
			}
			if (a.SortOrder > b.SortOrder)
			{
				return 1;
			}
			return 0;
		});
		this.ShowFixedRewards_Internal(blocker, onAllFixedRewardsShownCallbackInfo);
		return true;
	}

	// Token: 0x060002C8 RID: 712 RVA: 0x0000D2D0 File Offset: 0x0000B4D0
	public bool Cheat_ShowFixedReward(int fixedRewardMapID, FixedRewardsMgr.DelPositionNonToastReward positionNonToastRewardCallback, Vector3 rewardPunchScale, Vector3 rewardScale)
	{
		if (!ApplicationMgr.IsInternal())
		{
			return false;
		}
		FixedRewardMapDbfRecord record = GameDbf.FixedRewardMap.GetRecord(fixedRewardMapID);
		int sortOrder = (record != null) ? record.SortOrder : 0;
		FixedRewardsMgr.OnAllFixedRewardsShownCallbackInfo onAllFixedRewardsShownCallbackInfo = new FixedRewardsMgr.OnAllFixedRewardsShownCallbackInfo();
		FixedRewardsMgr.OnAllFixedRewardsShownCallbackInfo onAllFixedRewardsShownCallbackInfo2 = onAllFixedRewardsShownCallbackInfo;
		List<FixedRewardsMgr.RewardMapIDToShow> list = new List<FixedRewardsMgr.RewardMapIDToShow>();
		list.Add(new FixedRewardsMgr.RewardMapIDToShow(fixedRewardMapID, FixedRewardsMgr.RewardMapIDToShow.NO_ACHIEVE_ID, sortOrder));
		onAllFixedRewardsShownCallbackInfo2.m_rewardMapIDsToShow = list;
		onAllFixedRewardsShownCallbackInfo.m_positionNonToastRewardCallback = positionNonToastRewardCallback;
		onAllFixedRewardsShownCallbackInfo.m_rewardPunchScale = rewardPunchScale;
		onAllFixedRewardsShownCallbackInfo.m_rewardScale = rewardScale;
		onAllFixedRewardsShownCallbackInfo.m_showingCheatRewards = true;
		FixedRewardsMgr.OnAllFixedRewardsShownCallbackInfo callbackInfo = onAllFixedRewardsShownCallbackInfo;
		this.ShowFixedRewards_Internal(UserAttentionBlocker.NONE, callbackInfo);
		return true;
	}

	// Token: 0x060002C9 RID: 713 RVA: 0x0000D358 File Offset: 0x0000B558
	public bool CanCraftCard(string cardID, TAG_PREMIUM premium)
	{
		FixedRewardDbfRecord fixedRewardForCard = GameUtils.GetFixedRewardForCard(cardID, premium);
		if (fixedRewardForCard != null)
		{
			NetCache.CardDefinition cardDefinition = new NetCache.CardDefinition
			{
				Name = cardID,
				Premium = premium
			};
			return GameUtils.IsCardRotated(cardID) || this.m_craftableCardRewards.Contains(cardDefinition);
		}
		return true;
	}

	// Token: 0x060002CA RID: 714 RVA: 0x0000D3A4 File Offset: 0x0000B5A4
	public List<RewardData> GetRewardsForWing(int wingID, HashSet<RewardVisualTiming> rewardTimings)
	{
		List<RewardData> list = new List<RewardData>();
		List<FixedRewardActionDbfRecord> list2 = new List<FixedRewardActionDbfRecord>();
		list2.AddRange(GameUtils.GetFixedActionRecords(FixedActionType.WING_PROGRESS));
		list2.AddRange(GameUtils.GetFixedActionRecords(FixedActionType.WING_FLAGS));
		foreach (FixedRewardActionDbfRecord fixedRewardActionDbfRecord in list2)
		{
			if (fixedRewardActionDbfRecord.WingId == wingID)
			{
				list.AddRange(this.GetRewardsForAction(fixedRewardActionDbfRecord.ID, rewardTimings));
			}
		}
		return list;
	}

	// Token: 0x060002CB RID: 715 RVA: 0x0000D43C File Offset: 0x0000B63C
	public List<RewardData> GetRewardsForWingProgress(int wingID, int progress, HashSet<RewardVisualTiming> rewardTimings)
	{
		List<RewardData> list = new List<RewardData>();
		List<FixedRewardActionDbfRecord> fixedActionRecords = GameUtils.GetFixedActionRecords(FixedActionType.WING_PROGRESS);
		foreach (FixedRewardActionDbfRecord fixedRewardActionDbfRecord in fixedActionRecords)
		{
			if (fixedRewardActionDbfRecord.WingId == wingID && fixedRewardActionDbfRecord.WingProgress == progress)
			{
				list.AddRange(this.GetRewardsForAction(fixedRewardActionDbfRecord.ID, rewardTimings));
			}
		}
		return list;
	}

	// Token: 0x060002CC RID: 716 RVA: 0x0000D4C8 File Offset: 0x0000B6C8
	public List<RewardData> GetRewardsForWingFlags(int wingID, ulong flags, HashSet<RewardVisualTiming> rewardTimings)
	{
		List<RewardData> list = new List<RewardData>();
		List<FixedRewardActionDbfRecord> fixedActionRecords = GameUtils.GetFixedActionRecords(FixedActionType.WING_FLAGS);
		foreach (FixedRewardActionDbfRecord fixedRewardActionDbfRecord in fixedActionRecords)
		{
			if (fixedRewardActionDbfRecord.WingId == wingID)
			{
				ulong wingFlags = fixedRewardActionDbfRecord.WingFlags;
				if (wingFlags == flags)
				{
					list.AddRange(this.GetRewardsForAction(fixedRewardActionDbfRecord.ID, rewardTimings));
				}
			}
		}
		return list;
	}

	// Token: 0x060002CD RID: 717 RVA: 0x0000D560 File Offset: 0x0000B760
	private void WillReset()
	{
		this.m_craftableCardRewards.Clear();
		this.m_earnedMetaActionRewards.Clear();
		this.m_rewardMapIDsToShow.Clear();
		this.m_rewardMapIDsAwarded.Clear();
		AdventureProgressMgr.Get().RemoveProgressUpdatedListener(new AdventureProgressMgr.AdventureProgressUpdatedCallback(this.OnAdventureProgressUpdate));
		this.m_registeredForAdventureProgressUpdates = false;
		NetCache.Get().RemoveNewNoticesListener(new NetCache.DelNewNoticesListener(this.OnNewNotices));
		this.m_registeredForProfileNotices = false;
		AchieveManager.Get().RemoveNewlyCompletedAchievesListener(new AchieveManager.NewlyCompletedAchievesCallback(this.OnNewlyCompletedAchieves));
		this.m_registeredForCompletedAchieves = false;
		this.m_isStartupFinished = false;
	}

	// Token: 0x060002CE RID: 718 RVA: 0x0000D5FC File Offset: 0x0000B7FC
	private void OnAdventureProgressUpdate(bool isStartupAction, AdventureMission.WingProgress oldProgress, AdventureMission.WingProgress newProgress, object userData)
	{
		List<CardRewardData> cardRewards = new List<CardRewardData>();
		if (isStartupAction)
		{
			return;
		}
		if (newProgress == null)
		{
			return;
		}
		if (!newProgress.IsOwned())
		{
			return;
		}
		if (oldProgress == null)
		{
			this.TriggerWingProgressAction(FixedRewardsMgr.ShowVisualOption.SHOW, newProgress.Wing, newProgress.Progress, cardRewards);
			this.TriggerWingFlagsAction(FixedRewardsMgr.ShowVisualOption.SHOW, newProgress.Wing, newProgress.Flags, cardRewards);
		}
		else
		{
			bool flag = !oldProgress.IsOwned() && newProgress.IsOwned();
			if (flag || oldProgress.Progress != newProgress.Progress)
			{
				this.TriggerWingProgressAction((!flag) ? FixedRewardsMgr.ShowVisualOption.SHOW : FixedRewardsMgr.ShowVisualOption.DO_NOT_SHOW, newProgress.Wing, newProgress.Progress, cardRewards);
			}
			if (oldProgress.Flags != newProgress.Flags)
			{
				this.TriggerWingFlagsAction(FixedRewardsMgr.ShowVisualOption.SHOW, newProgress.Wing, newProgress.Flags, cardRewards);
			}
		}
		CollectionManager.Get().AddCardRewards(cardRewards, false);
	}

	// Token: 0x060002CF RID: 719 RVA: 0x0000D6DC File Offset: 0x0000B8DC
	private void OnNewNotices(List<NetCache.ProfileNotice> newNotices)
	{
		List<CardRewardData> cardRewards = new List<CardRewardData>();
		foreach (NetCache.ProfileNotice profileNotice in newNotices)
		{
			if (profileNotice.Type == NetCache.ProfileNotice.NoticeType.HERO_LEVEL_UP)
			{
				NetCache.ProfileNoticeLevelUp profileNoticeLevelUp = profileNotice as NetCache.ProfileNoticeLevelUp;
				FixedRewardsMgr.Get().TriggerHeroLevelAction(FixedRewardsMgr.ShowVisualOption.SHOW, profileNoticeLevelUp.HeroClass, profileNoticeLevelUp.NewLevel, cardRewards);
				Network.AckNotice(profileNotice.NoticeID);
			}
		}
		CollectionManager.Get().AddCardRewards(cardRewards, false);
	}

	// Token: 0x060002D0 RID: 720 RVA: 0x0000D774 File Offset: 0x0000B974
	private void OnNewlyCompletedAchieves(List<Achievement> achieves, object userData)
	{
		List<CardRewardData> cardRewards = new List<CardRewardData>();
		foreach (Achievement achievement in achieves)
		{
			this.TriggerAchieveAction(FixedRewardsMgr.ShowVisualOption.SHOW, achievement.ID, cardRewards);
		}
		CollectionManager.Get().AddCardRewards(cardRewards, false);
	}

	// Token: 0x060002D1 RID: 721 RVA: 0x0000D7E4 File Offset: 0x0000B9E4
	private void OnAccountLicensesUpdate(List<AccountLicenseInfo> changedAccountLicenses, object userData)
	{
		ApplicationMgr.Get().StartCoroutine(this.OnAccountLicensesUpdate_DoWork(changedAccountLicenses));
	}

	// Token: 0x060002D2 RID: 722 RVA: 0x0000D7F8 File Offset: 0x0000B9F8
	private IEnumerator OnAccountLicensesUpdate_DoWork(List<AccountLicenseInfo> changedAccountLicenses)
	{
		float startTime = Time.realtimeSinceStartup;
		while (this.m_isInitialization)
		{
			if (Time.realtimeSinceStartup - startTime >= 30f)
			{
				break;
			}
			yield return null;
		}
		List<CardRewardData> cardRewards = new List<CardRewardData>();
		foreach (AccountLicenseInfo accountLicense in changedAccountLicenses)
		{
			if (AccountLicenseMgr.Get().OwnsAccountLicense(accountLicense))
			{
				this.TriggerAccountLicenseFlagsAction(FixedRewardsMgr.ShowVisualOption.FORCE_SHOW, accountLicense.License, accountLicense.Flags_, cardRewards);
			}
		}
		CollectionManager.Get().AddCardRewards(cardRewards, false);
		if (SceneMgr.Get().GetMode() != SceneMgr.Mode.LOGIN && SceneMgr.Get().GetMode() != SceneMgr.Mode.GAMEPLAY)
		{
			HashSet<RewardVisualTiming> hashSet = new HashSet<RewardVisualTiming>();
			hashSet.Add(RewardVisualTiming.IMMEDIATE);
			HashSet<RewardVisualTiming> rewardTimings = hashSet;
			this.ShowFixedRewards(UserAttentionBlocker.NONE, rewardTimings, null, null, Login.REWARD_PUNCH_SCALE, Login.REWARD_SCALE);
		}
		yield break;
	}

	// Token: 0x060002D3 RID: 723 RVA: 0x0000D821 File Offset: 0x0000BA21
	private FixedRewardsMgr.FixedMetaActionReward GetEarnedMetaActionReward(int metaActionID)
	{
		if (!this.m_earnedMetaActionRewards.ContainsKey(metaActionID))
		{
			this.m_earnedMetaActionRewards[metaActionID] = new FixedRewardsMgr.FixedMetaActionReward(metaActionID);
		}
		return this.m_earnedMetaActionRewards[metaActionID];
	}

	// Token: 0x060002D4 RID: 724 RVA: 0x0000D854 File Offset: 0x0000BA54
	private void UpdateEarnedMetaActionFlags(int metaActionID, ulong addFlags, ulong removeFlags)
	{
		FixedRewardsMgr.FixedMetaActionReward earnedMetaActionReward = this.GetEarnedMetaActionReward(metaActionID);
		earnedMetaActionReward.UpdateFlags(addFlags, removeFlags);
	}

	// Token: 0x060002D5 RID: 725 RVA: 0x0000D874 File Offset: 0x0000BA74
	private NetCache.CardDefinition GetCardDefinition(FixedRewardDbfRecord dbfRecReward)
	{
		int cardId = dbfRecReward.CardId;
		string text = GameUtils.TranslateDbIdToCardId(cardId);
		if (text == null)
		{
			return null;
		}
		int cardPremium = dbfRecReward.CardPremium;
		TAG_PREMIUM premium;
		if (!EnumUtils.TryCast<TAG_PREMIUM>(cardPremium, out premium))
		{
			return null;
		}
		return new NetCache.CardDefinition
		{
			Name = text,
			Premium = premium
		};
	}

	// Token: 0x060002D6 RID: 726 RVA: 0x0000D8CC File Offset: 0x0000BACC
	private FixedRewardsMgr.FixedReward GetFixedReward(FixedRewardMapDbfRecord fixedRewardMapRecord)
	{
		FixedRewardsMgr.FixedReward fixedReward = new FixedRewardsMgr.FixedReward();
		FixedRewardDbfRecord record = GameDbf.FixedReward.GetRecord(fixedRewardMapRecord.RewardId);
		if (record == null)
		{
			return fixedReward;
		}
		FixedRewardType type;
		if (!EnumUtils.TryGetEnum<FixedRewardType>(record.Type, out type))
		{
			return fixedReward;
		}
		fixedReward.Type = type;
		switch (type)
		{
		case FixedRewardType.VIRTUAL_CARD:
		case FixedRewardType.REAL_CARD:
		{
			NetCache.CardDefinition cardDefinition = this.GetCardDefinition(record);
			if (cardDefinition != null)
			{
				fixedReward.FixedCardRewardData = new CardRewardData(cardDefinition.Name, cardDefinition.Premium, fixedRewardMapRecord.RewardCount);
				fixedReward.FixedCardRewardData.FixedReward = fixedRewardMapRecord;
			}
			break;
		}
		case FixedRewardType.CARD_BACK:
		{
			int cardBackId = record.CardBackId;
			fixedReward.FixedCardBackRewardData = new CardBackRewardData(cardBackId);
			break;
		}
		case FixedRewardType.CRAFTABLE_CARD:
		{
			NetCache.CardDefinition cardDefinition2 = this.GetCardDefinition(record);
			if (cardDefinition2 != null)
			{
				fixedReward.FixedCraftableCardRewardData = cardDefinition2;
			}
			break;
		}
		case FixedRewardType.META_ACTION_FLAGS:
		{
			int metaActionId = record.MetaActionId;
			ulong metaActionFlags = record.MetaActionFlags;
			fixedReward.FixedMetaActionRewardData = new FixedRewardsMgr.FixedMetaActionReward(metaActionId);
			fixedReward.FixedMetaActionRewardData.UpdateFlags(metaActionFlags, 0UL);
			break;
		}
		}
		return fixedReward;
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x0000D9E0 File Offset: 0x0000BBE0
	private bool QueueRewardVisual(FixedRewardMapDbfRecord fixedRewardMapRecord, int achieveID)
	{
		int id = fixedRewardMapRecord.ID;
		string rewardTiming = fixedRewardMapRecord.RewardTiming;
		RewardVisualTiming rewardVisualTiming;
		if (!EnumUtils.TryGetEnum<RewardVisualTiming>(rewardTiming, out rewardVisualTiming))
		{
			Debug.LogWarning(string.Format("QueueRewardVisual rewardMapID={0} no enum value for reward visual timing {1}, check fixed rewards map", id, rewardVisualTiming));
			rewardVisualTiming = RewardVisualTiming.IMMEDIATE;
		}
		Log.Achievements.Print(string.Concat(new object[]
		{
			"QueueRewardVisual achieveID=",
			achieveID,
			" fixedRewardMapId=",
			fixedRewardMapRecord.ID,
			" ",
			fixedRewardMapRecord.NoteDesc
		}), new object[0]);
		if (rewardVisualTiming == RewardVisualTiming.NEVER)
		{
			return false;
		}
		if (!this.m_rewardMapIDsToShow.ContainsKey(rewardVisualTiming))
		{
			this.m_rewardMapIDsToShow[rewardVisualTiming] = new HashSet<FixedRewardsMgr.RewardMapIDToShow>();
		}
		int sortOrder = fixedRewardMapRecord.SortOrder;
		this.m_rewardMapIDsToShow[rewardVisualTiming].Add(new FixedRewardsMgr.RewardMapIDToShow(fixedRewardMapRecord.ID, achieveID, sortOrder));
		return true;
	}

	// Token: 0x060002D8 RID: 728 RVA: 0x0000DACA File Offset: 0x0000BCCA
	private void TriggerRewardsForAction(int actionID, FixedRewardsMgr.ShowVisualOption showRewardVisual, List<CardRewardData> cardRewards)
	{
		this.TriggerRewardsForAction(actionID, showRewardVisual, cardRewards, FixedRewardsMgr.RewardMapIDToShow.NO_ACHIEVE_ID);
	}

	// Token: 0x060002D9 RID: 729 RVA: 0x0000DADC File Offset: 0x0000BCDC
	private void TriggerRewardsForAction(int actionID, FixedRewardsMgr.ShowVisualOption showRewardVisual, List<CardRewardData> cardRewards, int achieveID)
	{
		List<FixedRewardMapDbfRecord> fixedRewardMapRecordsForAction = GameUtils.GetFixedRewardMapRecordsForAction(actionID);
		foreach (FixedRewardMapDbfRecord fixedRewardMapDbfRecord in fixedRewardMapRecordsForAction)
		{
			int id = fixedRewardMapDbfRecord.ID;
			if (this.m_rewardMapIDsAwarded.Contains(id))
			{
				if (showRewardVisual != FixedRewardsMgr.ShowVisualOption.FORCE_SHOW)
				{
					continue;
				}
			}
			else
			{
				this.m_rewardMapIDsAwarded.Add(id);
			}
			int rewardCount = fixedRewardMapDbfRecord.RewardCount;
			if (rewardCount > 0)
			{
				bool flag = showRewardVisual != FixedRewardsMgr.ShowVisualOption.DO_NOT_SHOW;
				FixedRewardsMgr.FixedReward fixedReward = this.GetFixedReward(fixedRewardMapDbfRecord);
				if (fixedReward.FixedCardRewardData != null)
				{
					bool flag2;
					if (this.m_isInitialization)
					{
						flag = false;
						flag2 = (fixedReward.Type == FixedRewardType.VIRTUAL_CARD);
					}
					else
					{
						flag2 = true;
					}
					if ((!flag || !this.QueueRewardVisual(fixedRewardMapDbfRecord, achieveID)) && flag2)
					{
						cardRewards.Add(fixedReward.FixedCardRewardData);
					}
				}
				if (fixedReward.FixedCardBackRewardData != null && (!flag || !this.QueueRewardVisual(fixedRewardMapDbfRecord, achieveID)))
				{
					CardBackManager.Get().AddNewCardBack(fixedReward.FixedCardBackRewardData.CardBackID);
				}
				if (fixedReward.FixedCraftableCardRewardData != null)
				{
					this.m_craftableCardRewards.Add(fixedReward.FixedCraftableCardRewardData);
				}
				if (fixedReward.FixedMetaActionRewardData != null)
				{
					this.UpdateEarnedMetaActionFlags(fixedReward.FixedMetaActionRewardData.MetaActionID, fixedReward.FixedMetaActionRewardData.MetaActionFlags, 0UL);
					this.TriggerMetaActionFlagsAction(showRewardVisual, fixedReward.FixedMetaActionRewardData.MetaActionID, cardRewards);
				}
			}
		}
	}

	// Token: 0x060002DA RID: 730 RVA: 0x0000DC8C File Offset: 0x0000BE8C
	private void TriggerWingProgressAction(FixedRewardsMgr.ShowVisualOption showRewardVisual, int wingID, int progress, List<CardRewardData> cardRewards)
	{
		List<FixedRewardActionDbfRecord> fixedActionRecords = GameUtils.GetFixedActionRecords(FixedActionType.WING_PROGRESS);
		foreach (FixedRewardActionDbfRecord fixedRewardActionDbfRecord in fixedActionRecords)
		{
			if (fixedRewardActionDbfRecord.WingId == wingID)
			{
				if (fixedRewardActionDbfRecord.WingProgress <= progress)
				{
					this.TriggerRewardsForAction(fixedRewardActionDbfRecord.ID, showRewardVisual, cardRewards);
				}
			}
		}
	}

	// Token: 0x060002DB RID: 731 RVA: 0x0000DD14 File Offset: 0x0000BF14
	private void TriggerWingFlagsAction(FixedRewardsMgr.ShowVisualOption showRewardVisual, int wingID, ulong flags, List<CardRewardData> cardRewards)
	{
		List<FixedRewardActionDbfRecord> fixedActionRecords = GameUtils.GetFixedActionRecords(FixedActionType.WING_FLAGS);
		foreach (FixedRewardActionDbfRecord fixedRewardActionDbfRecord in fixedActionRecords)
		{
			if (fixedRewardActionDbfRecord.WingId == wingID)
			{
				ulong wingFlags = fixedRewardActionDbfRecord.WingFlags;
				if ((wingFlags & flags) == wingFlags)
				{
					this.TriggerRewardsForAction(fixedRewardActionDbfRecord.ID, showRewardVisual, cardRewards);
				}
			}
		}
	}

	// Token: 0x060002DC RID: 732 RVA: 0x0000DDA0 File Offset: 0x0000BFA0
	private void TriggerAchieveAction(FixedRewardsMgr.ShowVisualOption showRewardVisual, int achieveId, List<CardRewardData> cardRewards)
	{
		List<FixedRewardActionDbfRecord> fixedActionRecords = GameUtils.GetFixedActionRecords(FixedActionType.ACHIEVE);
		foreach (FixedRewardActionDbfRecord fixedRewardActionDbfRecord in fixedActionRecords)
		{
			if (fixedRewardActionDbfRecord.AchieveId == achieveId)
			{
				this.TriggerRewardsForAction(fixedRewardActionDbfRecord.ID, showRewardVisual, cardRewards, achieveId);
			}
		}
	}

	// Token: 0x060002DD RID: 733 RVA: 0x0000DE18 File Offset: 0x0000C018
	private void TriggerHeroLevelAction(FixedRewardsMgr.ShowVisualOption showRewardVisual, int classID, int heroLevel, List<CardRewardData> cardRewards)
	{
		List<FixedRewardActionDbfRecord> fixedActionRecords = GameUtils.GetFixedActionRecords(FixedActionType.HERO_LEVEL);
		foreach (FixedRewardActionDbfRecord fixedRewardActionDbfRecord in fixedActionRecords)
		{
			if (fixedRewardActionDbfRecord.ClassId == classID)
			{
				if (fixedRewardActionDbfRecord.HeroLevel <= heroLevel)
				{
					this.TriggerRewardsForAction(fixedRewardActionDbfRecord.ID, showRewardVisual, cardRewards);
				}
			}
		}
	}

	// Token: 0x060002DE RID: 734 RVA: 0x0000DEA0 File Offset: 0x0000C0A0
	private void TriggerTutorialProgressAction(FixedRewardsMgr.ShowVisualOption showRewardVisual, int tutorialProgress, List<CardRewardData> cardRewards)
	{
		List<FixedRewardActionDbfRecord> fixedActionRecords = GameUtils.GetFixedActionRecords(FixedActionType.TUTORIAL_PROGRESS);
		foreach (FixedRewardActionDbfRecord fixedRewardActionDbfRecord in fixedActionRecords)
		{
			if (fixedRewardActionDbfRecord.TutorialProgress <= tutorialProgress)
			{
				this.TriggerRewardsForAction(fixedRewardActionDbfRecord.ID, showRewardVisual, cardRewards);
			}
		}
	}

	// Token: 0x060002DF RID: 735 RVA: 0x0000DF14 File Offset: 0x0000C114
	private void TriggerAccountLicenseFlagsAction(FixedRewardsMgr.ShowVisualOption showRewardVisual, long license, ulong flags, List<CardRewardData> cardRewards)
	{
		List<FixedRewardActionDbfRecord> fixedActionRecords = GameUtils.GetFixedActionRecords(FixedActionType.ACCOUNT_LICENSE_FLAGS);
		foreach (FixedRewardActionDbfRecord fixedRewardActionDbfRecord in fixedActionRecords)
		{
			if (fixedRewardActionDbfRecord.AccountLicenseId == license)
			{
				ulong accountLicenseFlags = fixedRewardActionDbfRecord.AccountLicenseFlags;
				if ((accountLicenseFlags & flags) == accountLicenseFlags)
				{
					this.TriggerRewardsForAction(fixedRewardActionDbfRecord.ID, showRewardVisual, cardRewards);
				}
			}
		}
	}

	// Token: 0x060002E0 RID: 736 RVA: 0x0000DFA0 File Offset: 0x0000C1A0
	private void TriggerMetaActionFlagsAction(FixedRewardsMgr.ShowVisualOption showRewardVisual, int metaActionID, List<CardRewardData> cardRewards)
	{
		FixedRewardActionDbfRecord record = GameDbf.FixedRewardAction.GetRecord(metaActionID);
		if (record == null)
		{
			return;
		}
		ulong metaActionFlags = record.MetaActionFlags;
		FixedRewardsMgr.FixedMetaActionReward earnedMetaActionReward = this.GetEarnedMetaActionReward(metaActionID);
		if (!earnedMetaActionReward.HasAllRequiredFlags(metaActionFlags))
		{
			return;
		}
		this.TriggerRewardsForAction(metaActionID, showRewardVisual, cardRewards);
	}

	// Token: 0x060002E1 RID: 737 RVA: 0x0000DFE8 File Offset: 0x0000C1E8
	private void ShowFixedRewards_Internal(UserAttentionBlocker blocker, FixedRewardsMgr.OnAllFixedRewardsShownCallbackInfo callbackInfo)
	{
		if (callbackInfo.m_rewardMapIDsToShow.Count == 0)
		{
			if (callbackInfo.m_onAllRewardsShownCallback != null)
			{
				callbackInfo.m_onAllRewardsShownCallback(callbackInfo.m_userData);
			}
			return;
		}
		FixedRewardsMgr.RewardMapIDToShow rewardMapIDToShow = callbackInfo.m_rewardMapIDsToShow[0];
		callbackInfo.m_rewardMapIDsToShow.RemoveAt(0);
		FixedRewardMapDbfRecord record = GameDbf.FixedRewardMap.GetRecord(rewardMapIDToShow.RewardMapID);
		FixedRewardsMgr.FixedReward fixedReward = this.GetFixedReward(record);
		RewardData rewardData = null;
		if (fixedReward.FixedCardRewardData != null)
		{
			rewardData = fixedReward.FixedCardRewardData;
		}
		else if (fixedReward.FixedCardBackRewardData != null)
		{
			rewardData = fixedReward.FixedCardBackRewardData;
		}
		if (rewardData == null)
		{
			this.ShowFixedRewards_Internal(blocker, callbackInfo);
			return;
		}
		if (callbackInfo.m_showingCheatRewards)
		{
			rewardData.MarkAsDummyReward();
		}
		if (rewardMapIDToShow.AchieveID != FixedRewardsMgr.RewardMapIDToShow.NO_ACHIEVE_ID)
		{
			Achievement achievement = AchieveManager.Get().GetAchievement(rewardMapIDToShow.AchieveID);
			if (achievement != null)
			{
				achievement.AckCurrentProgressAndRewardNotices();
			}
		}
		bool useQuestToast = record.UseQuestToast;
		if (useQuestToast)
		{
			string name = record.ToastName;
			string description = record.ToastDescription;
			QuestToast.ShowFixedRewardQuestToast(blocker, delegate(object userData)
			{
				this.ShowFixedRewards_Internal(blocker, callbackInfo);
			}, rewardData, name, description);
		}
		else
		{
			rewardData.LoadRewardObject(delegate(Reward reward, object callbackData)
			{
				if (callbackInfo.m_positionNonToastRewardCallback != null)
				{
					callbackInfo.m_positionNonToastRewardCallback(reward);
				}
				RewardUtils.ShowReward(blocker, reward, true, callbackInfo.m_rewardPunchScale, callbackInfo.m_rewardScale, delegate(object showRewardUserData)
				{
					reward.RegisterClickListener(new Reward.OnClickedCallback(this.OnNonToastRewardClicked), callbackInfo);
					reward.EnableClickCatcher(true);
				}, null);
			});
		}
	}

	// Token: 0x060002E2 RID: 738 RVA: 0x0000E184 File Offset: 0x0000C384
	private void OnNonToastRewardClicked(Reward reward, object userData)
	{
		FixedRewardsMgr.OnAllFixedRewardsShownCallbackInfo onAllFixedRewardsShownCallbackInfo = userData as FixedRewardsMgr.OnAllFixedRewardsShownCallbackInfo;
		reward.RemoveClickListener(new Reward.OnClickedCallback(this.OnNonToastRewardClicked), onAllFixedRewardsShownCallbackInfo);
		reward.Hide(true);
		this.ShowFixedRewards_Internal(UserAttentionBlocker.NONE, onAllFixedRewardsShownCallbackInfo);
	}

	// Token: 0x060002E3 RID: 739 RVA: 0x0000E1BC File Offset: 0x0000C3BC
	private List<RewardData> GetRewardsForAction(int actionID, HashSet<RewardVisualTiming> rewardTimings)
	{
		List<RewardData> list = new List<RewardData>();
		List<FixedRewardMapDbfRecord> fixedRewardMapRecordsForAction = GameUtils.GetFixedRewardMapRecordsForAction(actionID);
		foreach (FixedRewardMapDbfRecord fixedRewardMapDbfRecord in fixedRewardMapRecordsForAction)
		{
			int rewardCount = fixedRewardMapDbfRecord.RewardCount;
			if (rewardCount > 0)
			{
				string rewardTiming = fixedRewardMapDbfRecord.RewardTiming;
				RewardVisualTiming rewardVisualTiming;
				if (EnumUtils.TryGetEnum<RewardVisualTiming>(rewardTiming, out rewardVisualTiming))
				{
					if (rewardTimings.Contains(rewardVisualTiming))
					{
						FixedRewardsMgr.FixedReward fixedReward = this.GetFixedReward(fixedRewardMapDbfRecord);
						if (fixedReward.FixedCardRewardData != null)
						{
							list.Add(fixedReward.FixedCardRewardData);
						}
						else if (fixedReward.FixedCardBackRewardData != null)
						{
							list.Add(fixedReward.FixedCardBackRewardData);
						}
					}
				}
			}
		}
		return list;
	}

	// Token: 0x060002E4 RID: 740 RVA: 0x0000E29C File Offset: 0x0000C49C
	private void GrantAchieveRewards(List<CardRewardData> cardRewards)
	{
		AchieveManager achieveManager = AchieveManager.Get();
		if (achieveManager == null)
		{
			Debug.LogWarning(string.Format("FixedRewardsMgr.GrantAchieveRewards(): null == AchieveManager.Get()", new object[0]));
			return;
		}
		List<Achievement> completedAchieves = achieveManager.GetCompletedAchieves();
		foreach (Achievement achievement in completedAchieves)
		{
			FixedRewardsMgr.ShowVisualOption showRewardVisual = (achievement.AcknowledgedProgress >= achievement.Progress) ? FixedRewardsMgr.ShowVisualOption.DO_NOT_SHOW : FixedRewardsMgr.ShowVisualOption.SHOW;
			this.TriggerAchieveAction(showRewardVisual, achievement.ID, cardRewards);
		}
	}

	// Token: 0x060002E5 RID: 741 RVA: 0x0000E33C File Offset: 0x0000C53C
	private void GrantHeroLevelRewards(List<CardRewardData> cardRewards)
	{
		NetCache.NetCacheHeroLevels netObject = NetCache.Get().GetNetObject<NetCache.NetCacheHeroLevels>();
		if (netObject == null)
		{
			Debug.LogWarning(string.Format("FixedRewardsMgr.GrantHeroUnlockRewards(): null == NetCache.NetCacheHeroLevels", new object[0]));
			return;
		}
		foreach (NetCache.HeroLevel heroLevel in netObject.Levels)
		{
			this.TriggerHeroLevelAction(FixedRewardsMgr.ShowVisualOption.DO_NOT_SHOW, (int)heroLevel.Class, heroLevel.CurrentLevel.Level, cardRewards);
		}
	}

	// Token: 0x040000FB RID: 251
	private static FixedRewardsMgr s_instance;

	// Token: 0x040000FC RID: 252
	private bool m_registeredForAdventureProgressUpdates;

	// Token: 0x040000FD RID: 253
	private bool m_registeredForProfileNotices;

	// Token: 0x040000FE RID: 254
	private bool m_registeredForCompletedAchieves;

	// Token: 0x040000FF RID: 255
	private HashSet<NetCache.CardDefinition> m_craftableCardRewards = new HashSet<NetCache.CardDefinition>();

	// Token: 0x04000100 RID: 256
	private Map<int, FixedRewardsMgr.FixedMetaActionReward> m_earnedMetaActionRewards = new Map<int, FixedRewardsMgr.FixedMetaActionReward>();

	// Token: 0x04000101 RID: 257
	private Map<RewardVisualTiming, HashSet<FixedRewardsMgr.RewardMapIDToShow>> m_rewardMapIDsToShow = new Map<RewardVisualTiming, HashSet<FixedRewardsMgr.RewardMapIDToShow>>();

	// Token: 0x04000102 RID: 258
	private HashSet<int> m_rewardMapIDsAwarded = new HashSet<int>();

	// Token: 0x04000103 RID: 259
	private bool m_isStartupFinished;

	// Token: 0x04000104 RID: 260
	private bool m_isInitialization = true;

	// Token: 0x02000153 RID: 339
	// (Invoke) Token: 0x06001231 RID: 4657
	public delegate void DelOnAllFixedRewardsShown(object userData);

	// Token: 0x02000154 RID: 340
	// (Invoke) Token: 0x06001235 RID: 4661
	public delegate void DelPositionNonToastReward(Reward reward);

	// Token: 0x02000155 RID: 341
	private class FixedMetaActionReward
	{
		// Token: 0x06001238 RID: 4664 RVA: 0x0004F423 File Offset: 0x0004D623
		public FixedMetaActionReward(int metaActionID)
		{
			this.MetaActionID = metaActionID;
			this.MetaActionFlags = 0UL;
		}

		// Token: 0x170002EB RID: 747
		// (get) Token: 0x06001239 RID: 4665 RVA: 0x0004F43A File Offset: 0x0004D63A
		// (set) Token: 0x0600123A RID: 4666 RVA: 0x0004F442 File Offset: 0x0004D642
		public int MetaActionID { get; private set; }

		// Token: 0x170002EC RID: 748
		// (get) Token: 0x0600123B RID: 4667 RVA: 0x0004F44B File Offset: 0x0004D64B
		// (set) Token: 0x0600123C RID: 4668 RVA: 0x0004F453 File Offset: 0x0004D653
		public ulong MetaActionFlags { get; private set; }

		// Token: 0x0600123D RID: 4669 RVA: 0x0004F45C File Offset: 0x0004D65C
		public void UpdateFlags(ulong addFlags, ulong removeFlags)
		{
			this.MetaActionFlags |= addFlags;
			this.MetaActionFlags &= ~removeFlags;
		}

		// Token: 0x0600123E RID: 4670 RVA: 0x0004F486 File Offset: 0x0004D686
		public bool HasAllRequiredFlags(ulong requiredFlags)
		{
			return (this.MetaActionFlags & requiredFlags) == requiredFlags;
		}
	}

	// Token: 0x02000156 RID: 342
	private class FixedReward
	{
		// Token: 0x0600123F RID: 4671 RVA: 0x0004F493 File Offset: 0x0004D693
		public FixedReward()
		{
			this.Type = FixedRewardType.UNKNOWN;
			this.FixedCardRewardData = null;
			this.FixedCardBackRewardData = null;
			this.FixedCraftableCardRewardData = null;
			this.FixedMetaActionRewardData = null;
		}

		// Token: 0x04000977 RID: 2423
		public FixedRewardType Type;

		// Token: 0x04000978 RID: 2424
		public CardRewardData FixedCardRewardData;

		// Token: 0x04000979 RID: 2425
		public CardBackRewardData FixedCardBackRewardData;

		// Token: 0x0400097A RID: 2426
		public NetCache.CardDefinition FixedCraftableCardRewardData;

		// Token: 0x0400097B RID: 2427
		public FixedRewardsMgr.FixedMetaActionReward FixedMetaActionRewardData;
	}

	// Token: 0x02000159 RID: 345
	private class OnAllFixedRewardsShownCallbackInfo
	{
		// Token: 0x04000984 RID: 2436
		public List<FixedRewardsMgr.RewardMapIDToShow> m_rewardMapIDsToShow;

		// Token: 0x04000985 RID: 2437
		public FixedRewardsMgr.DelOnAllFixedRewardsShown m_onAllRewardsShownCallback;

		// Token: 0x04000986 RID: 2438
		public FixedRewardsMgr.DelPositionNonToastReward m_positionNonToastRewardCallback;

		// Token: 0x04000987 RID: 2439
		public Vector3 m_rewardPunchScale;

		// Token: 0x04000988 RID: 2440
		public Vector3 m_rewardScale;

		// Token: 0x04000989 RID: 2441
		public object m_userData;

		// Token: 0x0400098A RID: 2442
		public bool m_showingCheatRewards;
	}

	// Token: 0x0200015A RID: 346
	private class RewardMapIDToShow
	{
		// Token: 0x06001247 RID: 4679 RVA: 0x0004F52F File Offset: 0x0004D72F
		public RewardMapIDToShow(int rewardMapID, int achieveID, int sortOrder)
		{
			this.RewardMapID = rewardMapID;
			this.AchieveID = achieveID;
			this.SortOrder = sortOrder;
		}

		// Token: 0x06001249 RID: 4681 RVA: 0x0004F550 File Offset: 0x0004D750
		public override bool Equals(object obj)
		{
			FixedRewardsMgr.RewardMapIDToShow rewardMapIDToShow = obj as FixedRewardsMgr.RewardMapIDToShow;
			return rewardMapIDToShow != null && this.RewardMapID == rewardMapIDToShow.RewardMapID;
		}

		// Token: 0x0600124A RID: 4682 RVA: 0x0004F57A File Offset: 0x0004D77A
		public override int GetHashCode()
		{
			return this.RewardMapID.GetHashCode();
		}

		// Token: 0x0400098B RID: 2443
		public static readonly int NO_ACHIEVE_ID;

		// Token: 0x0400098C RID: 2444
		public int RewardMapID;

		// Token: 0x0400098D RID: 2445
		public int AchieveID;

		// Token: 0x0400098E RID: 2446
		public int SortOrder;
	}

	// Token: 0x0200015B RID: 347
	private enum ShowVisualOption
	{
		// Token: 0x04000990 RID: 2448
		DO_NOT_SHOW,
		// Token: 0x04000991 RID: 2449
		SHOW,
		// Token: 0x04000992 RID: 2450
		FORCE_SHOW
	}
}
