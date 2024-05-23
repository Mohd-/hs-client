using System;
using System.Collections.Generic;
using System.Linq;
using bgs;
using PegasusUtil;
using UnityEngine;

// Token: 0x020000B9 RID: 185
public class AchieveManager
{
	// Token: 0x06000992 RID: 2450 RVA: 0x00028688 File Offset: 0x00026888
	private AchieveManager()
	{
		this.LoadAchievesFromDBF();
	}

	// Token: 0x06000994 RID: 2452 RVA: 0x00028741 File Offset: 0x00026941
	public static AchieveManager Get()
	{
		return AchieveManager.s_instance;
	}

	// Token: 0x06000995 RID: 2453 RVA: 0x00028748 File Offset: 0x00026948
	public static void Init()
	{
		if (AchieveManager.s_instance == null)
		{
			AchieveManager.s_instance = new AchieveManager();
			Network network = Network.Get();
			network.RegisterNetHandler(252, new Network.NetHandler(AchieveManager.s_instance.OnAchieves), null);
			network.RegisterNetHandler(282, new Network.NetHandler(AchieveManager.s_instance.OnQuestCanceled), null);
			network.RegisterNetHandler(285, new Network.NetHandler(AchieveManager.s_instance.OnAchieveValidated), null);
			network.RegisterNetHandler(299, new Network.NetHandler(AchieveManager.s_instance.OnEventTriggered), null);
			network.RegisterNetHandler(311, new Network.NetHandler(AchieveManager.s_instance.OnAccountLicenseAchieveResponse), null);
			NetCache.Get().RegisterNewNoticesListener(new NetCache.DelNewNoticesListener(AchieveManager.s_instance.OnNewNotices));
			ApplicationMgr.Get().WillReset += new Action(AchieveManager.s_instance.WillReset);
		}
	}

	// Token: 0x06000996 RID: 2454 RVA: 0x0002884D File Offset: 0x00026A4D
	public static void InitRequests()
	{
		Network.RequestAchieves(false);
	}

	// Token: 0x06000997 RID: 2455 RVA: 0x00028858 File Offset: 0x00026A58
	public static bool IsPredicateTrue(Achievement.Predicate predicate)
	{
		if (predicate == Achievement.Predicate.CAN_SEE_WILD)
		{
			if (CollectionManager.Get().ShouldAccountSeeStandardWild())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000998 RID: 2456 RVA: 0x0002888A File Offset: 0x00026A8A
	public void InitAchieveManager()
	{
		AchieveManager.s_instance.WillReset();
	}

	// Token: 0x06000999 RID: 2457 RVA: 0x00028898 File Offset: 0x00026A98
	public bool IsReady()
	{
		return this.m_allNetAchievesReceived && !this.m_waitingForActiveAchieves && this.m_numEventResponsesNeeded <= 0 && this.m_achieveValidationsToRequest.Count <= 0 && this.m_achieveValidationsRequested.Count <= 0 && NetCache.Get().GetNetObject<NetCache.NetCacheProfileNotices>() != null;
	}

	// Token: 0x0600099A RID: 2458 RVA: 0x00028905 File Offset: 0x00026B05
	public void UpdateActiveAchieves(AchieveManager.ActiveAchievesUpdatedCallback callback)
	{
		this.UpdateActiveAchieves(callback, null);
	}

	// Token: 0x0600099B RID: 2459 RVA: 0x00028910 File Offset: 0x00026B10
	public void UpdateActiveAchieves(AchieveManager.ActiveAchievesUpdatedCallback callback, object userData)
	{
		this.RegisterActiveAchievesUpdatedListener(callback, userData);
		if (this.m_waitingForActiveAchieves)
		{
			return;
		}
		this.m_waitingForActiveAchieves = true;
		Network.RequestAchieves(true);
	}

	// Token: 0x0600099C RID: 2460 RVA: 0x0002893F File Offset: 0x00026B3F
	public bool RemoveActiveAchievesUpdatedListener(AchieveManager.ActiveAchievesUpdatedCallback callback)
	{
		return this.RemoveActiveAchievesUpdatedListener(callback, null);
	}

	// Token: 0x0600099D RID: 2461 RVA: 0x0002894C File Offset: 0x00026B4C
	public bool RemoveActiveAchievesUpdatedListener(AchieveManager.ActiveAchievesUpdatedCallback callback, object userData)
	{
		if (callback == null)
		{
			return false;
		}
		AchieveManager.ActiveAchievesUpdatedListener activeAchievesUpdatedListener = new AchieveManager.ActiveAchievesUpdatedListener();
		activeAchievesUpdatedListener.SetCallback(callback);
		activeAchievesUpdatedListener.SetUserData(userData);
		if (!this.m_activeAchievesUpdatedListeners.Contains(activeAchievesUpdatedListener))
		{
			return false;
		}
		this.m_activeAchievesUpdatedListeners.Remove(activeAchievesUpdatedListener);
		return true;
	}

	// Token: 0x0600099E RID: 2462 RVA: 0x00028998 File Offset: 0x00026B98
	public List<Achievement> GetNewCompletedAchieves()
	{
		List<Achievement> list = Enumerable.ToList<Achievement>(this.m_achievements.Values);
		return list.FindAll(delegate(Achievement obj)
		{
			if (!obj.IsCompleted())
			{
				return false;
			}
			if (obj.IsInternal())
			{
				return false;
			}
			if (obj.RewardTiming == RewardVisualTiming.NEVER)
			{
				return false;
			}
			switch (obj.AchieveType)
			{
			case Achievement.AchType.UNLOCK_HERO:
				return false;
			case Achievement.AchType.UNLOCK_GOLDEN_HERO:
				return false;
			case Achievement.AchType.DAILY_REPEATABLE:
				return false;
			}
			return obj.AcknowledgedProgress < obj.Progress;
		});
	}

	// Token: 0x0600099F RID: 2463 RVA: 0x000289DC File Offset: 0x00026BDC
	private static bool IsActiveQuest(Achievement obj, bool onlyNewlyActive)
	{
		return obj.Active && obj.CanShowInQuestLog && (!onlyNewlyActive || obj.IsNewlyActive());
	}

	// Token: 0x060009A0 RID: 2464 RVA: 0x00028A14 File Offset: 0x00026C14
	public List<Achievement> GetActiveQuests(bool onlyNewlyActive = false)
	{
		return Enumerable.ToList<Achievement>(Enumerable.Where<Achievement>(this.m_achievements.Values, (Achievement obj) => AchieveManager.IsActiveQuest(obj, onlyNewlyActive)));
	}

	// Token: 0x060009A1 RID: 2465 RVA: 0x00028A54 File Offset: 0x00026C54
	public bool HasActiveQuests(bool onlyNewlyActive = false)
	{
		return Enumerable.Any<KeyValuePair<int, Achievement>>(this.m_achievements, (KeyValuePair<int, Achievement> kv) => AchieveManager.IsActiveQuest(kv.Value, onlyNewlyActive));
	}

	// Token: 0x060009A2 RID: 2466 RVA: 0x00028A88 File Offset: 0x00026C88
	public List<Achievement> GetNewlyProgressedQuests()
	{
		List<Achievement> activeQuests = AchieveManager.Get().GetActiveQuests(false);
		return activeQuests.FindAll((Achievement obj) => obj.Progress > obj.AcknowledgedProgress);
	}

	// Token: 0x060009A3 RID: 2467 RVA: 0x00028AC8 File Offset: 0x00026CC8
	public bool HasUnlockedFeature(Achievement.UnlockableFeature feature)
	{
		if (DemoMgr.Get().ArenaIs1WinMode() && feature == Achievement.UnlockableFeature.FORGE)
		{
			return true;
		}
		Achievement achievement = Enumerable.ToList<Achievement>(this.m_achievements.Values).Find((Achievement obj) => obj.UnlockedFeature != null && obj.UnlockedFeature.Value == feature);
		if (achievement == null)
		{
			Debug.LogWarning(string.Format("AchieveManager.HasUnlockedFeature(): could not find achieve that unlocks feature {0}", feature));
			return false;
		}
		return achievement.IsCompleted();
	}

	// Token: 0x060009A4 RID: 2468 RVA: 0x00028B49 File Offset: 0x00026D49
	public Achievement GetAchievement(int achieveID)
	{
		if (!this.m_achievements.ContainsKey(achieveID))
		{
			return null;
		}
		return this.m_achievements[achieveID];
	}

	// Token: 0x060009A5 RID: 2469 RVA: 0x00028B6C File Offset: 0x00026D6C
	public int GetNumAchievesInGroup(Achievement.AchType achieveGroup)
	{
		List<Achievement> achievesInGroup = this.GetAchievesInGroup(achieveGroup);
		return achievesInGroup.Count;
	}

	// Token: 0x060009A6 RID: 2470 RVA: 0x00028B88 File Offset: 0x00026D88
	public List<Achievement> GetCompletedAchieves()
	{
		List<Achievement> list = new List<Achievement>(this.m_achievements.Values);
		return list.FindAll((Achievement obj) => obj.IsCompleted());
	}

	// Token: 0x060009A7 RID: 2471 RVA: 0x00028BCC File Offset: 0x00026DCC
	public List<Achievement> GetAchievesInGroup(Achievement.AchType achieveGroup)
	{
		List<Achievement> list = new List<Achievement>(this.m_achievements.Values);
		return list.FindAll((Achievement obj) => obj.AchieveType == achieveGroup);
	}

	// Token: 0x060009A8 RID: 2472 RVA: 0x00028C0C File Offset: 0x00026E0C
	public List<Achievement> GetAchievesInGroup(Achievement.AchType achieveGroup, bool isComplete)
	{
		List<Achievement> achievesInGroup = this.GetAchievesInGroup(achieveGroup);
		return achievesInGroup.FindAll((Achievement obj) => obj.IsCompleted() == isComplete);
	}

	// Token: 0x060009A9 RID: 2473 RVA: 0x00028C44 File Offset: 0x00026E44
	public Achievement GetUnlockGoldenHeroAchievement(string heroCardID, TAG_PREMIUM premium)
	{
		List<Achievement> achievesInGroup = this.GetAchievesInGroup(Achievement.AchType.UNLOCK_GOLDEN_HERO);
		return achievesInGroup.Find(delegate(Achievement achieveObj)
		{
			RewardData rewardData = achieveObj.Rewards.Find((RewardData rewardObj) => rewardObj.RewardType == Reward.Type.CARD);
			if (rewardData == null)
			{
				return false;
			}
			CardRewardData cardRewardData = rewardData as CardRewardData;
			return cardRewardData != null && cardRewardData.CardID.Equals(heroCardID) && cardRewardData.Premium.Equals(premium);
		});
	}

	// Token: 0x060009AA RID: 2474 RVA: 0x00028C84 File Offset: 0x00026E84
	public bool HasActiveAchievesForEvent(SpecialEventType eventTrigger)
	{
		if (eventTrigger == SpecialEventType.IGNORE)
		{
			return false;
		}
		List<Achievement> list = Enumerable.ToList<Achievement>(this.m_achievements.Values);
		List<Achievement> list2 = list.FindAll((Achievement obj) => obj.EventTrigger == eventTrigger && obj.Enabled && obj.Active);
		return list2.Count > 0;
	}

	// Token: 0x060009AB RID: 2475 RVA: 0x00028CD8 File Offset: 0x00026ED8
	public bool CanCancelQuest(int achieveID)
	{
		if (this.m_disableCancelButtonUntilServerReturns)
		{
			return false;
		}
		if (!this.CanCancelQuestNow())
		{
			return false;
		}
		Achievement achievement = this.GetAchievement(achieveID);
		return achievement != null && achievement.AchieveType == Achievement.AchType.DAILY_QUEST && achievement.Active;
	}

	// Token: 0x060009AC RID: 2476 RVA: 0x00028D23 File Offset: 0x00026F23
	public bool RegisterQuestCanceledListener(AchieveManager.AchieveCanceledCallback callback)
	{
		return this.RegisterQuestCanceledListener(callback, null);
	}

	// Token: 0x060009AD RID: 2477 RVA: 0x00028D30 File Offset: 0x00026F30
	public bool RegisterQuestCanceledListener(AchieveManager.AchieveCanceledCallback callback, object userData)
	{
		AchieveManager.AchieveCanceledListener achieveCanceledListener = new AchieveManager.AchieveCanceledListener();
		achieveCanceledListener.SetCallback(callback);
		achieveCanceledListener.SetUserData(userData);
		if (this.m_achieveCanceledListeners.Contains(achieveCanceledListener))
		{
			return false;
		}
		this.m_achieveCanceledListeners.Add(achieveCanceledListener);
		return true;
	}

	// Token: 0x060009AE RID: 2478 RVA: 0x00028D71 File Offset: 0x00026F71
	public bool RemoveQuestCanceledListener(AchieveManager.AchieveCanceledCallback callback)
	{
		return this.RemoveQuestCanceledListener(callback, null);
	}

	// Token: 0x060009AF RID: 2479 RVA: 0x00028D7C File Offset: 0x00026F7C
	public bool RemoveQuestCanceledListener(AchieveManager.AchieveCanceledCallback callback, object userData)
	{
		AchieveManager.AchieveCanceledListener achieveCanceledListener = new AchieveManager.AchieveCanceledListener();
		achieveCanceledListener.SetCallback(callback);
		achieveCanceledListener.SetUserData(userData);
		return this.m_achieveCanceledListeners.Remove(achieveCanceledListener);
	}

	// Token: 0x060009B0 RID: 2480 RVA: 0x00028DAC File Offset: 0x00026FAC
	public void CancelQuest(int achieveID)
	{
		if (!this.CanCancelQuest(achieveID))
		{
			this.FireAchieveCanceledEvent(achieveID, false);
			return;
		}
		this.m_disableCancelButtonUntilServerReturns = true;
		Network.RequestCancelQuest(achieveID);
	}

	// Token: 0x060009B1 RID: 2481 RVA: 0x00028DDB File Offset: 0x00026FDB
	public bool RegisterNewlyCompletedAchievesListener(AchieveManager.NewlyCompletedAchievesCallback callback)
	{
		return this.RegisterNewlyCompletedAchievesListener(callback, null);
	}

	// Token: 0x060009B2 RID: 2482 RVA: 0x00028DE8 File Offset: 0x00026FE8
	public bool RegisterNewlyCompletedAchievesListener(AchieveManager.NewlyCompletedAchievesCallback callback, object userData)
	{
		AchieveManager.NewlyCompletedAchievesListener newlyCompletedAchievesListener = new AchieveManager.NewlyCompletedAchievesListener();
		newlyCompletedAchievesListener.SetCallback(callback);
		newlyCompletedAchievesListener.SetUserData(userData);
		if (this.m_newlyCompletedAchievesListeners.Contains(newlyCompletedAchievesListener))
		{
			return false;
		}
		this.m_newlyCompletedAchievesListeners.Add(newlyCompletedAchievesListener);
		return true;
	}

	// Token: 0x060009B3 RID: 2483 RVA: 0x00028E29 File Offset: 0x00027029
	public bool RemoveNewlyCompletedAchievesListener(AchieveManager.NewlyCompletedAchievesCallback callback)
	{
		return this.RemoveNewlyCompletedAchievesListener(callback, null);
	}

	// Token: 0x060009B4 RID: 2484 RVA: 0x00028E34 File Offset: 0x00027034
	public bool RemoveNewlyCompletedAchievesListener(AchieveManager.NewlyCompletedAchievesCallback callback, object userData)
	{
		AchieveManager.NewlyCompletedAchievesListener newlyCompletedAchievesListener = new AchieveManager.NewlyCompletedAchievesListener();
		newlyCompletedAchievesListener.SetCallback(callback);
		newlyCompletedAchievesListener.SetUserData(userData);
		return this.m_newlyCompletedAchievesListeners.Remove(newlyCompletedAchievesListener);
	}

	// Token: 0x060009B5 RID: 2485 RVA: 0x00028E61 File Offset: 0x00027061
	public bool RegisterLicenseAddedAchievesUpdatedListener(AchieveManager.LicenseAddedAchievesUpdatedCallback callback)
	{
		return this.RegisterLicenseAddedAchievesUpdatedListener(callback, null);
	}

	// Token: 0x060009B6 RID: 2486 RVA: 0x00028E6C File Offset: 0x0002706C
	public bool RegisterLicenseAddedAchievesUpdatedListener(AchieveManager.LicenseAddedAchievesUpdatedCallback callback, object userData)
	{
		AchieveManager.LicenseAddedAchievesUpdatedListener licenseAddedAchievesUpdatedListener = new AchieveManager.LicenseAddedAchievesUpdatedListener();
		licenseAddedAchievesUpdatedListener.SetCallback(callback);
		licenseAddedAchievesUpdatedListener.SetUserData(userData);
		if (this.m_licenseAddedAchievesUpdatedListeners.Contains(licenseAddedAchievesUpdatedListener))
		{
			return false;
		}
		this.m_licenseAddedAchievesUpdatedListeners.Add(licenseAddedAchievesUpdatedListener);
		return true;
	}

	// Token: 0x060009B7 RID: 2487 RVA: 0x00028EAD File Offset: 0x000270AD
	public bool RemoveLicenseAddedAchievesUpdatedListener(AchieveManager.LicenseAddedAchievesUpdatedCallback callback)
	{
		return this.RemoveLicenseAddedAchievesUpdatedListener(callback, null);
	}

	// Token: 0x060009B8 RID: 2488 RVA: 0x00028EB8 File Offset: 0x000270B8
	public bool RemoveLicenseAddedAchievesUpdatedListener(AchieveManager.LicenseAddedAchievesUpdatedCallback callback, object userData)
	{
		AchieveManager.LicenseAddedAchievesUpdatedListener licenseAddedAchievesUpdatedListener = new AchieveManager.LicenseAddedAchievesUpdatedListener();
		licenseAddedAchievesUpdatedListener.SetCallback(callback);
		licenseAddedAchievesUpdatedListener.SetUserData(userData);
		return this.m_licenseAddedAchievesUpdatedListeners.Remove(licenseAddedAchievesUpdatedListener);
	}

	// Token: 0x060009B9 RID: 2489 RVA: 0x00028EE5 File Offset: 0x000270E5
	public bool HasActiveLicenseAddedAchieves()
	{
		return this.GetActiveLicenseAddedAchieves().Count > 0;
	}

	// Token: 0x060009BA RID: 2490 RVA: 0x00028EF8 File Offset: 0x000270F8
	public bool HasIncompletePurchaseAchieves()
	{
		List<Achievement> list = Enumerable.ToList<Achievement>(this.m_achievements.Values).FindAll((Achievement obj) => !obj.IsCompleted() && obj.Enabled && obj.AchieveTrigger == Achievement.Trigger.PURCHASE);
		return list.Count > 0;
	}

	// Token: 0x060009BB RID: 2491 RVA: 0x00028F44 File Offset: 0x00027144
	public bool HasIncompleteDisenchantAchieves()
	{
		List<Achievement> list = Enumerable.ToList<Achievement>(this.m_achievements.Values).FindAll((Achievement obj) => !obj.IsCompleted() && obj.Enabled && obj.AchieveTrigger == Achievement.Trigger.DISENCHANT);
		return list.Count > 0;
	}

	// Token: 0x060009BC RID: 2492 RVA: 0x00028F90 File Offset: 0x00027190
	public void NotifyOfClick(Achievement.ClickTriggerType clickType)
	{
		List<Achievement> list = Enumerable.ToList<Achievement>(this.m_achievements.Values).FindAll((Achievement obj) => !obj.IsCompleted() && obj.Enabled && obj.AchieveTrigger == Achievement.Trigger.CLICK && obj.ClickType != null && obj.ClickType.Value == clickType);
		foreach (Achievement achievement in list)
		{
			this.m_achieveValidationsToRequest.Add(achievement.ID);
		}
		this.ValidateAchievesNow(null);
	}

	// Token: 0x060009BD RID: 2493 RVA: 0x00029028 File Offset: 0x00027228
	public void NotifyOfCardsGained(List<EntityDef> entityDefs, bool hasGolden)
	{
		HashSet<TAG_CARD_SET> hashSet = new HashSet<TAG_CARD_SET>();
		HashSet<TAG_RACE> hashSet2 = new HashSet<TAG_RACE>();
		HashSet<Achievement> hashSet3 = new HashSet<Achievement>();
		HashSet<Achievement> hashSet4 = new HashSet<Achievement>();
		foreach (EntityDef entityDef in entityDefs)
		{
			hashSet.Add(entityDef.GetCardSet());
			hashSet2.Add(entityDef.GetRace());
		}
		foreach (Achievement achievement in this.m_achievements.Values)
		{
			if (!hashSet3.Contains(achievement) && !hashSet4.Contains(achievement))
			{
				if (!achievement.IsCompleted())
				{
					if ((achievement.AchieveTrigger == Achievement.Trigger.GAIN_CARD || (hasGolden && achievement.AchieveTrigger == Achievement.Trigger.GAIN_GOLDEN_CARD)) && achievement.RaceRequirement != null && hashSet2.Contains(achievement.RaceRequirement.Value))
					{
						hashSet3.Add(achievement);
					}
					else if (achievement.AchieveTrigger == Achievement.Trigger.COMPLETE_CARD_SET && hashSet.Contains(achievement.CardSetRequirement.Value))
					{
						hashSet4.Add(achievement);
					}
				}
			}
		}
		this.AddAchievesToValidate(Enumerable.ToList<Achievement>(hashSet3), Enumerable.ToList<Achievement>(hashSet4));
	}

	// Token: 0x060009BE RID: 2494 RVA: 0x000291D0 File Offset: 0x000273D0
	public void NotifyOfCardGained(EntityDef entityDef, TAG_PREMIUM premium, int totalCount)
	{
		Log.Achievements.Print(string.Concat(new object[]
		{
			"NotifyOfCardGained: ",
			entityDef,
			" ",
			premium,
			" ",
			totalCount
		}), new object[0]);
		List<Achievement> possibleRaceAchieves = Enumerable.ToList<Achievement>(this.m_achievements.Values).FindAll(delegate(Achievement obj)
		{
			if (obj.IsCompleted())
			{
				return false;
			}
			Achievement.Trigger achieveTrigger = obj.AchieveTrigger;
			if (achieveTrigger != Achievement.Trigger.GAIN_CARD)
			{
				if (achieveTrigger != Achievement.Trigger.GAIN_GOLDEN_CARD)
				{
					return false;
				}
				if (premium != TAG_PREMIUM.GOLDEN)
				{
					return false;
				}
			}
			return obj.RaceRequirement != null && obj.RaceRequirement.Value == entityDef.GetRace();
		});
		List<Achievement> possibleCardSetAchieves = Enumerable.ToList<Achievement>(this.m_achievements.Values).FindAll((Achievement obj) => !obj.IsCompleted() && obj.AchieveTrigger == Achievement.Trigger.COMPLETE_CARD_SET && obj.CardSetRequirement != null && obj.CardSetRequirement.Value == entityDef.GetCardSet());
		this.AddAchievesToValidate(possibleRaceAchieves, possibleCardSetAchieves);
	}

	// Token: 0x060009BF RID: 2495 RVA: 0x00029290 File Offset: 0x00027490
	public void NotifyOfPacksReadyToOpen(UnopenedPack unopenedPack, AchieveManager.ActiveAchievesUpdatedCallback callback)
	{
		List<Achievement> list = Enumerable.ToList<Achievement>(this.m_achievements.Values).FindAll(delegate(Achievement obj)
		{
			if (obj.IsCompleted())
			{
				return false;
			}
			if (obj.AchieveTrigger != Achievement.Trigger.PACK_READY_TO_OPEN)
			{
				return false;
			}
			int boosterRequirement = obj.BoosterRequirement;
			return boosterRequirement == unopenedPack.GetBoosterStack().Id && unopenedPack.GetBoosterStack().Count != 0 && unopenedPack.CanOpenPack();
		});
		if (list.Count > 0)
		{
			foreach (Achievement achievement in list)
			{
				this.m_achieveValidationsToRequest.Add(achievement.ID);
			}
			this.ValidateAchievesNow(callback);
		}
	}

	// Token: 0x060009C0 RID: 2496 RVA: 0x00029334 File Offset: 0x00027534
	public void Heartbeat()
	{
		this.CheckTimedTimedEventsAndLicenses(DateTime.UtcNow);
	}

	// Token: 0x060009C1 RID: 2497 RVA: 0x00029341 File Offset: 0x00027541
	public void ValidateAchievesNow(AchieveManager.ActiveAchievesUpdatedCallback callback)
	{
		this.ValidateAchievesNow(callback, null);
	}

	// Token: 0x060009C2 RID: 2498 RVA: 0x0002934C File Offset: 0x0002754C
	public void ValidateAchievesNow(AchieveManager.ActiveAchievesUpdatedCallback callback, object userData)
	{
		if (this.m_achieveValidationsToRequest.Count == 0)
		{
			if (callback != null)
			{
				callback(userData);
			}
			return;
		}
		this.RegisterActiveAchievesUpdatedListener(callback, userData);
		Enumerable.Union<int>(this.m_achieveValidationsRequested, this.m_achieveValidationsToRequest);
		foreach (int achieveID in this.m_achieveValidationsToRequest)
		{
			Network.ValidateAchieve(achieveID);
		}
		this.m_achieveValidationsToRequest.Clear();
	}

	// Token: 0x060009C3 RID: 2499 RVA: 0x000293EC File Offset: 0x000275EC
	public void TriggerLaunchDayEvent()
	{
		if (!this.HasActiveAchievesForEvent(SpecialEventType.LAUNCH_DAY))
		{
			return;
		}
		Player opposingSidePlayer = GameState.Get().GetOpposingSidePlayer();
		if (opposingSidePlayer == null)
		{
			return;
		}
		BnetPlayer bnetPlayer = BnetNearbyPlayerMgr.Get().FindNearbyPlayer(opposingSidePlayer.GetGameAccountId());
		if (bnetPlayer == null)
		{
			return;
		}
		BnetAccountId accountId = bnetPlayer.GetAccountId();
		if (accountId == null)
		{
			return;
		}
		List<BnetPlayer> nearbyPlayers = BnetNearbyPlayerMgr.Get().GetNearbyPlayers();
		BnetPlayer bnetPlayer2 = null;
		foreach (BnetPlayer bnetPlayer3 in nearbyPlayers)
		{
			BnetAccountId accountId2 = bnetPlayer3.GetAccountId();
			if (!(accountId2 == null))
			{
				if (!accountId2.Equals(accountId))
				{
					bnetPlayer2 = bnetPlayer3;
					break;
				}
			}
		}
		if (bnetPlayer2 == null)
		{
			return;
		}
		ulong lastOpponentSessionStartTime;
		if (!BnetNearbyPlayerMgr.Get().GetNearbySessionStartTime(bnetPlayer, out lastOpponentSessionStartTime))
		{
			return;
		}
		ulong otherPlayerSessionStartTime;
		if (!BnetNearbyPlayerMgr.Get().GetNearbySessionStartTime(bnetPlayer2, out otherPlayerSessionStartTime))
		{
			return;
		}
		BnetGameAccountId hearthstoneGameAccountId = bnetPlayer.GetHearthstoneGameAccountId();
		if (hearthstoneGameAccountId == null)
		{
			return;
		}
		BnetGameAccountId hearthstoneGameAccountId2 = bnetPlayer2.GetHearthstoneGameAccountId();
		if (hearthstoneGameAccountId2 == null)
		{
			return;
		}
		this.m_numEventResponsesNeeded++;
		Network.TriggerLaunchEvent(hearthstoneGameAccountId, lastOpponentSessionStartTime, hearthstoneGameAccountId2, otherPlayerSessionStartTime);
	}

	// Token: 0x060009C4 RID: 2500 RVA: 0x00029544 File Offset: 0x00027744
	private void LoadAchievesFromDBF()
	{
		List<AchieveDbfRecord> records = GameDbf.Achieve.GetRecords();
		Map<int, int> map = new Map<int, int>();
		AchieveDbfRecord achieveRecord;
		foreach (AchieveDbfRecord achieveRecord2 in records)
		{
			achieveRecord = achieveRecord2;
			int id = achieveRecord.ID;
			bool enabled = achieveRecord.Enabled;
			string achType = achieveRecord.AchType;
			Achievement.AchType achieveGroup;
			if (EnumUtils.TryGetEnum<Achievement.AchType>(achType, out achieveGroup))
			{
				int achQuota = achieveRecord.AchQuota;
				int race = achieveRecord.Race;
				TAG_RACE? raceReq = default(TAG_RACE?);
				if (race != 0)
				{
					raceReq = new TAG_RACE?((TAG_RACE)race);
				}
				int cardSet = achieveRecord.CardSet;
				TAG_CARD_SET? cardSetReq = default(TAG_CARD_SET?);
				if (cardSet != 0)
				{
					cardSetReq = new TAG_CARD_SET?((TAG_CARD_SET)cardSet);
				}
				string reward = achieveRecord.Reward;
				long rewardData = achieveRecord.RewardData1;
				long rewardData2 = achieveRecord.RewardData2;
				List<RewardData> list = new List<RewardData>();
				TAG_CLASS? classReq = default(TAG_CLASS?);
				string text = reward;
				if (text != null)
				{
					if (AchieveManager.<>f__switch$map7D == null)
					{
						Dictionary<string, int> dictionary = new Dictionary<string, int>(13);
						dictionary.Add("basic", 0);
						dictionary.Add("card", 1);
						dictionary.Add("card2x", 2);
						dictionary.Add("cardback", 3);
						dictionary.Add("cardset", 4);
						dictionary.Add("craftable_golden", 5);
						dictionary.Add("dust", 6);
						dictionary.Add("forge", 7);
						dictionary.Add("gold", 8);
						dictionary.Add("goldhero", 9);
						dictionary.Add("hero", 10);
						dictionary.Add("mount", 11);
						dictionary.Add("pack", 12);
						AchieveManager.<>f__switch$map7D = dictionary;
					}
					int num;
					if (AchieveManager.<>f__switch$map7D.TryGetValue(text, ref num))
					{
						switch (num)
						{
						case 0:
							Debug.LogWarning(string.Format("AchieveManager.LoadAchievesFromFile(): unable to define reward {0} for achieve {1}", reward, id));
							break;
						case 1:
						{
							string cardID = GameUtils.TranslateDbIdToCardId((int)rewardData);
							TAG_PREMIUM premium = (TAG_PREMIUM)rewardData2;
							list.Add(new CardRewardData(cardID, premium, 1));
							break;
						}
						case 2:
						{
							string cardID2 = GameUtils.TranslateDbIdToCardId((int)rewardData);
							TAG_PREMIUM premium2 = (TAG_PREMIUM)rewardData2;
							list.Add(new CardRewardData(cardID2, premium2, 2));
							break;
						}
						case 3:
							list.Add(new CardBackRewardData((int)rewardData));
							break;
						case 6:
							list.Add(new ArcaneDustRewardData((int)rewardData));
							break;
						case 7:
							list.Add(new ForgeTicketRewardData((int)rewardData));
							break;
						case 8:
							list.Add(new GoldRewardData((long)((int)rewardData)));
							break;
						case 9:
						{
							string cardID3 = GameUtils.TranslateDbIdToCardId((int)rewardData);
							TAG_PREMIUM premium3 = (TAG_PREMIUM)rewardData2;
							list.Add(new CardRewardData(cardID3, premium3, 1));
							break;
						}
						case 10:
						{
							classReq = new TAG_CLASS?((TAG_CLASS)rewardData2);
							string basicHeroCardIdFromClass = GameUtils.GetBasicHeroCardIdFromClass(classReq.Value);
							if (!string.IsNullOrEmpty(basicHeroCardIdFromClass))
							{
								list.Add(new CardRewardData(basicHeroCardIdFromClass, TAG_PREMIUM.NORMAL, 1));
							}
							break;
						}
						case 11:
							list.Add(new MountRewardData((MountRewardData.MountType)rewardData));
							break;
						case 12:
						{
							int id2 = (rewardData2 <= 0L) ? 1 : ((int)rewardData2);
							list.Add(new BoosterPackRewardData(id2, (int)rewardData));
							break;
						}
						}
					}
				}
				IL_394:
				string rewardTiming = achieveRecord.RewardTiming;
				RewardVisualTiming rewardTiming2;
				if (!EnumUtils.TryGetEnum<RewardVisualTiming>(rewardTiming, out rewardTiming2))
				{
					continue;
				}
				string unlocks = achieveRecord.Unlocks;
				Achievement.UnlockableFeature? unlockedFeature = default(Achievement.UnlockableFeature?);
				Achievement.UnlockableFeature unlockableFeature;
				if (!string.IsNullOrEmpty(unlocks) && EnumUtils.TryGetEnum<Achievement.UnlockableFeature>(unlocks, out unlockableFeature))
				{
					unlockedFeature = new Achievement.UnlockableFeature?(unlockableFeature);
				}
				AchieveDbfRecord achieveDbfRecord = records.Find((AchieveDbfRecord obj) => obj.NoteDesc == achieveRecord.ParentAch);
				int value = (achieveDbfRecord != null) ? achieveDbfRecord.ID : 0;
				map[id] = value;
				string triggered = achieveRecord.Triggered;
				Achievement.Trigger trigger;
				if (!EnumUtils.TryGetEnum<Achievement.Trigger>(triggered, out trigger))
				{
					trigger = Achievement.Trigger.IGNORE;
				}
				SpecialEventType eventTrigger = SpecialEventType.IGNORE;
				Achievement.ClickTriggerType? clickType = default(Achievement.ClickTriggerType?);
				switch (trigger)
				{
				case Achievement.Trigger.CLICK:
					clickType = new Achievement.ClickTriggerType?((Achievement.ClickTriggerType)rewardData);
					break;
				case Achievement.Trigger.EVENT:
				case Achievement.Trigger.EVENT_TIMING_ONLY:
					eventTrigger = EnumUtils.GetEnum<SpecialEventType>(achieveRecord.Event);
					break;
				}
				int booster = achieveRecord.Booster;
				Achievement achievement = new Achievement(id, enabled, achieveGroup, achQuota, trigger, raceReq, classReq, cardSetReq, clickType, eventTrigger, unlockedFeature, list, rewardTiming2, booster);
				achievement.SetClientFlags((Achievement.ClientFlags)achieveRecord.ClientFlags);
				achievement.SetAltTextPredicate(achieveRecord.AltTextPredicate);
				achievement.SetName(achieveRecord.Name, achieveRecord.AltName);
				achievement.SetDescription(achieveRecord.Description, achieveRecord.AltDescription);
				this.InitAchievement(achievement);
				continue;
				goto IL_394;
			}
		}
		List<Achievement> list2 = Enumerable.ToList<Achievement>(this.m_achievements.Values);
		List<Achievement> list3 = list2.FindAll((Achievement obj) => obj.IsInternal() && obj.Rewards.Count > 0);
		foreach (Achievement achievement2 in list3)
		{
			Achievement achievement3 = this.GetAchievement(map[achievement2.ID]);
			while (achievement3 != null && achievement3.IsInternal())
			{
				achievement3 = this.GetAchievement(map[achievement3.ID]);
			}
			if (achievement3 == null)
			{
				Debug.LogWarning(string.Format("AchieveManager.LoadAchievesFromDBF(): found internal achievement with reward but could not find non-internal parent: {0}", achievement2));
			}
			else
			{
				Achievement achievement4 = this.GetAchievement(achievement3.ID);
				if (achievement4 == null)
				{
					Debug.LogWarning(string.Format("AchieveManager.LoadAchievesFromDBF(): parentAchieve with id {0} for internalRewardAchieve {1} is null!", achievement3, achievement2));
				}
				else
				{
					achievement4.AddChildRewards(achievement2.Rewards);
				}
			}
		}
	}

	// Token: 0x060009C5 RID: 2501 RVA: 0x00029BFC File Offset: 0x00027DFC
	private void InitAchievement(Achievement achievement)
	{
		if (this.m_achievements.ContainsKey(achievement.ID))
		{
			Debug.LogWarning(string.Format("AchieveManager.InitAchievement() - already registered achievement with ID {0}", achievement.ID));
			return;
		}
		this.m_achievements.Add(achievement.ID, achievement);
	}

	// Token: 0x060009C6 RID: 2502 RVA: 0x00029C4C File Offset: 0x00027E4C
	private void WillReset()
	{
		this.m_allNetAchievesReceived = false;
		this.m_waitingForActiveAchieves = false;
		this.m_achieveValidationsToRequest.Clear();
		this.m_achieveValidationsRequested.Clear();
		this.m_activeAchievesUpdatedListeners.Clear();
		this.m_newlyCompletedAchievesListeners.Clear();
		this.m_lastEventTimingValidationByAchieve.Clear();
		this.m_lastCheckLicenseAddedByAchieve.Clear();
		this.m_licenseAddedAchievesUpdatedListeners.Clear();
		this.m_achievements.Clear();
		this.LoadAchievesFromDBF();
	}

	// Token: 0x060009C7 RID: 2503 RVA: 0x00029CC8 File Offset: 0x00027EC8
	private void OnAchieves()
	{
		Network.AchieveList achieveList = Network.Achieves();
		if (!this.m_allNetAchievesReceived)
		{
			this.OnAllAchieves(achieveList);
		}
		else
		{
			this.OnActiveAndNewCompleteAchieves(achieveList);
		}
	}

	// Token: 0x060009C8 RID: 2504 RVA: 0x00029CFC File Offset: 0x00027EFC
	private void OnAllAchieves(Network.AchieveList allAchievesList)
	{
		foreach (Network.AchieveList.Achieve achieve in allAchievesList.Achieves)
		{
			Achievement achievement = this.GetAchievement(achieve.ID);
			if (achievement != null)
			{
				achievement.OnAchieveData(achieve.Progress, achieve.AckProgress, achieve.CompletionCount, achieve.Active, achieve.DateGiven, achieve.DateCompleted, achieve.CanAck);
			}
		}
		NetCache.Get().RegisterNotices(null, null);
		this.CheckAllCardGainAchieves();
		this.m_allNetAchievesReceived = true;
	}

	// Token: 0x060009C9 RID: 2505 RVA: 0x00029DB0 File Offset: 0x00027FB0
	private void OnActiveAndNewCompleteAchieves(Network.AchieveList activeAchievesList)
	{
		List<Achievement> list = Enumerable.ToList<Achievement>(this.m_achievements.Values).FindAll((Achievement obj) => obj.Active);
		List<Achievement> list2 = new List<Achievement>();
		int num = 0;
		foreach (Network.AchieveList.Achieve achieve in activeAchievesList.Achieves)
		{
			Achievement achievement = this.GetAchievement(achieve.ID);
			if (achievement != null)
			{
				Log.Achievements.Print("Processing achievement: " + achievement, new object[0]);
				if (!achieve.Active)
				{
					achievement.OnAchieveData(achieve.Progress, achieve.AckProgress, achieve.CompletionCount, achieve.Active, achieve.DateGiven, achieve.DateCompleted, achieve.CanAck);
					num++;
					list2.Add(achievement);
				}
				else
				{
					achievement.UpdateActiveAchieve(achieve.Progress, achieve.AckProgress, achieve.DateGiven, achieve.CanAck);
					Achievement achievement3 = list.Find((Achievement obj) => obj.ID == achievement.ID);
					if (achievement3 != null)
					{
						list.Remove(achievement3);
					}
				}
			}
		}
		foreach (Achievement achievement2 in list)
		{
			num++;
			list2.Add(achievement2);
			achievement2.Complete();
		}
		AchieveManager.NewlyCompletedAchievesListener[] array = this.m_newlyCompletedAchievesListeners.ToArray();
		foreach (AchieveManager.NewlyCompletedAchievesListener newlyCompletedAchievesListener in array)
		{
			newlyCompletedAchievesListener.Fire(list2);
		}
		AchieveManager.ActiveAchievesUpdatedListener[] array3 = this.m_activeAchievesUpdatedListeners.ToArray();
		this.m_activeAchievesUpdatedListeners.Clear();
		foreach (AchieveManager.ActiveAchievesUpdatedListener activeAchievesUpdatedListener in array3)
		{
			activeAchievesUpdatedListener.Fire();
		}
		if (num > 0)
		{
			NetCache.Get().ReloadNetObject<NetCache.NetCacheProfileNotices>();
		}
		this.m_waitingForActiveAchieves = false;
	}

	// Token: 0x060009CA RID: 2506 RVA: 0x0002A030 File Offset: 0x00028230
	private void OnQuestCanceled()
	{
		Network.CanceledQuest canceledQuest = Network.GetCanceledQuest();
		this.m_disableCancelButtonUntilServerReturns = false;
		if (canceledQuest.Canceled)
		{
			Achievement achievement = this.GetAchievement(canceledQuest.AchieveID);
			achievement.OnCancelSuccess();
			NetCache.NetCacheRewardProgress netObject = NetCache.Get().GetNetObject<NetCache.NetCacheRewardProgress>();
			if (netObject != null)
			{
				netObject.NextQuestCancelDate = canceledQuest.NextQuestCancelDate;
			}
		}
		this.FireAchieveCanceledEvent(canceledQuest.AchieveID, canceledQuest.Canceled);
	}

	// Token: 0x060009CB RID: 2507 RVA: 0x0002A098 File Offset: 0x00028298
	private void OnAchieveValidated()
	{
		ValidateAchieveResponse validatedAchieve = Network.GetValidatedAchieve();
		this.m_achieveValidationsRequested.Remove(validatedAchieve.Achieve);
		if (this.m_achieveValidationsRequested.Count > 0)
		{
			return;
		}
		if (validatedAchieve.HasSuccess && !validatedAchieve.Success)
		{
			return;
		}
		this.UpdateActiveAchieves(null);
		NetCache.Get().ReloadNetObject<NetCache.NetCacheProfileNotices>();
	}

	// Token: 0x060009CC RID: 2508 RVA: 0x0002A0F8 File Offset: 0x000282F8
	private void OnEventTriggered()
	{
		Network.TriggeredEvent triggerEventResponse = Network.GetTriggerEventResponse();
		if (triggerEventResponse.Success)
		{
			if (Enum.IsDefined(typeof(SpecialEventType), triggerEventResponse.EventID))
			{
				SpecialEventType eventID = (SpecialEventType)triggerEventResponse.EventID;
				if (this.HasActiveAchievesForEvent(eventID))
				{
					this.UpdateActiveAchieves(null);
					NetCache.Get().ReloadNetObject<NetCache.NetCacheProfileNotices>();
				}
			}
			else
			{
				Debug.LogWarning(string.Format("AchieveManager.OnEventTriggered(): unknown (successfully triggered) event ID {0}", triggerEventResponse.EventID));
			}
		}
		this.m_numEventResponsesNeeded--;
	}

	// Token: 0x060009CD RID: 2509 RVA: 0x0002A188 File Offset: 0x00028388
	private void OnAccountLicenseAchieveResponse()
	{
		Network.AccountLicenseAchieveResponse accountLicenseAchieveResponse = Network.GetAccountLicenseAchieveResponse();
		if (accountLicenseAchieveResponse.Result != Network.AccountLicenseAchieveResponse.AchieveResult.COMPLETE)
		{
			this.FireLicenseAddedAchievesUpdatedEvent();
			return;
		}
		Log.Rachelle.Print("AchieveManager.OnAccountLicenseAchieveResponse(): achieve {0} is now complete, refreshing achieves", new object[]
		{
			accountLicenseAchieveResponse.Achieve
		});
		this.UpdateActiveAchieves(new AchieveManager.ActiveAchievesUpdatedCallback(this.OnAccountLicenseAchievesUpdated), accountLicenseAchieveResponse.Achieve);
	}

	// Token: 0x060009CE RID: 2510 RVA: 0x0002A1F0 File Offset: 0x000283F0
	private void OnAccountLicenseAchievesUpdated(object userData)
	{
		int num = (int)userData;
		Log.Rachelle.Print("AchieveManager.OnAccountLicenseAchievesUpdated(): refreshing achieves complete, triggered by achieve {0}", new object[]
		{
			num
		});
		this.FireLicenseAddedAchievesUpdatedEvent();
	}

	// Token: 0x060009CF RID: 2511 RVA: 0x0002A228 File Offset: 0x00028428
	private void FireLicenseAddedAchievesUpdatedEvent()
	{
		List<Achievement> activeLicenseAddedAchieves = this.GetActiveLicenseAddedAchieves();
		AchieveManager.LicenseAddedAchievesUpdatedListener[] array = this.m_licenseAddedAchievesUpdatedListeners.ToArray();
		foreach (AchieveManager.LicenseAddedAchievesUpdatedListener licenseAddedAchievesUpdatedListener in array)
		{
			licenseAddedAchievesUpdatedListener.Fire(activeLicenseAddedAchieves);
		}
	}

	// Token: 0x060009D0 RID: 2512 RVA: 0x0002A26E File Offset: 0x0002846E
	private bool RegisterActiveAchievesUpdatedListener(AchieveManager.ActiveAchievesUpdatedCallback callback)
	{
		return this.RegisterActiveAchievesUpdatedListener(callback, null);
	}

	// Token: 0x060009D1 RID: 2513 RVA: 0x0002A278 File Offset: 0x00028478
	private bool RegisterActiveAchievesUpdatedListener(AchieveManager.ActiveAchievesUpdatedCallback callback, object userData)
	{
		if (callback == null)
		{
			return false;
		}
		AchieveManager.ActiveAchievesUpdatedListener activeAchievesUpdatedListener = new AchieveManager.ActiveAchievesUpdatedListener();
		activeAchievesUpdatedListener.SetCallback(callback);
		activeAchievesUpdatedListener.SetUserData(userData);
		if (this.m_activeAchievesUpdatedListeners.Contains(activeAchievesUpdatedListener))
		{
			return false;
		}
		this.m_activeAchievesUpdatedListeners.Add(activeAchievesUpdatedListener);
		return true;
	}

	// Token: 0x060009D2 RID: 2514 RVA: 0x0002A2C4 File Offset: 0x000284C4
	private void OnNewNotices(List<NetCache.ProfileNotice> newNotices)
	{
		foreach (NetCache.ProfileNotice profileNotice in newNotices)
		{
			if (profileNotice.Origin == NetCache.ProfileNotice.NoticeOrigin.ACHIEVEMENT)
			{
				Achievement achievement = this.GetAchievement((int)profileNotice.OriginData);
				if (achievement != null)
				{
					achievement.AddRewardNoticeID(profileNotice.NoticeID);
				}
			}
		}
	}

	// Token: 0x060009D3 RID: 2515 RVA: 0x0002A348 File Offset: 0x00028548
	private bool CanCancelQuestNow()
	{
		if (Vars.Key("Quests.CanCancelManyTimes").GetBool(false))
		{
			return true;
		}
		NetCache.NetCacheRewardProgress netObject = NetCache.Get().GetNetObject<NetCache.NetCacheRewardProgress>();
		if (netObject == null)
		{
			return false;
		}
		long num = DateTime.Now.ToFileTimeUtc();
		return netObject.NextQuestCancelDate <= num;
	}

	// Token: 0x060009D4 RID: 2516 RVA: 0x0002A39C File Offset: 0x0002859C
	private void FireAchieveCanceledEvent(int achieveID, bool success)
	{
		AchieveManager.AchieveCanceledListener[] array = this.m_achieveCanceledListeners.ToArray();
		foreach (AchieveManager.AchieveCanceledListener achieveCanceledListener in array)
		{
			achieveCanceledListener.Fire(achieveID, success);
		}
	}

	// Token: 0x060009D5 RID: 2517 RVA: 0x0002A3D8 File Offset: 0x000285D8
	private void AddAchievesToValidate(List<Achievement> possibleRaceAchieves, List<Achievement> possibleCardSetAchieves)
	{
		foreach (Achievement achievement in possibleRaceAchieves)
		{
			TAG_PREMIUM? premium = default(TAG_PREMIUM?);
			if (achievement.AchieveTrigger == Achievement.Trigger.GAIN_GOLDEN_CARD)
			{
				premium = new TAG_PREMIUM?(TAG_PREMIUM.GOLDEN);
			}
			if (CollectionManager.Get().AllCardsInSetOwned(new TAG_CARD_SET?(TAG_CARD_SET.CORE), default(TAG_CLASS?), default(TAG_RARITY?), new TAG_RACE?(achievement.RaceRequirement.Value), premium))
			{
				if (CollectionManager.Get().AllCardsInSetOwned(new TAG_CARD_SET?(TAG_CARD_SET.EXPERT1), default(TAG_CLASS?), default(TAG_RARITY?), new TAG_RACE?(achievement.RaceRequirement.Value), premium))
				{
					this.m_achieveValidationsToRequest.Add(achievement.ID);
				}
			}
		}
		foreach (Achievement achievement2 in possibleCardSetAchieves)
		{
			if (CollectionManager.Get().AllCardsInSetOwned(new TAG_CARD_SET?(achievement2.CardSetRequirement.Value), default(TAG_CLASS?), default(TAG_RARITY?), default(TAG_RACE?), default(TAG_PREMIUM?)))
			{
				this.m_achieveValidationsToRequest.Add(achievement2.ID);
			}
		}
	}

	// Token: 0x060009D6 RID: 2518 RVA: 0x0002A580 File Offset: 0x00028780
	private void CheckAllCardGainAchieves()
	{
		List<Achievement> possibleRaceAchieves = Enumerable.ToList<Achievement>(this.m_achievements.Values).FindAll(delegate(Achievement obj)
		{
			if (obj.IsCompleted())
			{
				return false;
			}
			Achievement.Trigger achieveTrigger = obj.AchieveTrigger;
			return (achieveTrigger == Achievement.Trigger.GAIN_CARD || achieveTrigger == Achievement.Trigger.GAIN_GOLDEN_CARD) && obj.RaceRequirement != null;
		});
		List<Achievement> possibleCardSetAchieves = Enumerable.ToList<Achievement>(this.m_achievements.Values).FindAll((Achievement obj) => !obj.IsCompleted() && obj.AchieveTrigger == Achievement.Trigger.COMPLETE_CARD_SET && obj.CardSetRequirement != null);
		this.AddAchievesToValidate(possibleRaceAchieves, possibleCardSetAchieves);
		this.ValidateAchievesNow(null);
	}

	// Token: 0x060009D7 RID: 2519 RVA: 0x0002A604 File Offset: 0x00028804
	private void CheckTimedTimedEventsAndLicenses(DateTime utcNow)
	{
		DateTime dateTime = utcNow.ToLocalTime();
		if (dateTime.Ticks - this.m_lastEventTimingAndLicenseAchieveCheck < AchieveManager.TIMED_AND_LICENSE_ACHIEVE_CHECK_DELAY_TICKS)
		{
			return;
		}
		this.m_lastEventTimingAndLicenseAchieveCheck = dateTime.Ticks;
		int num = 0;
		foreach (Achievement achievement in this.m_achievements.Values)
		{
			if (achievement.Enabled && !achievement.IsCompleted() && achievement.Active && achievement.AchieveTrigger == Achievement.Trigger.EVENT_TIMING_ONLY && SpecialEventManager.Get().IsEventActive(achievement.EventTrigger, false) && (!this.m_lastEventTimingValidationByAchieve.ContainsKey(achievement.ID) || dateTime.Ticks - this.m_lastEventTimingValidationByAchieve[achievement.ID] >= AchieveManager.TIMED_ACHIEVE_VALIDATION_DELAY_TICKS))
			{
				Log.Rachelle.Print("AchieveManager.CheckTimedTimedEventsAndLicenses(): checking on timed event achieve {0} time {1}", new object[]
				{
					achievement.ID,
					dateTime
				});
				this.m_lastEventTimingValidationByAchieve[achievement.ID] = dateTime.Ticks;
				this.m_achieveValidationsToRequest.Add(achievement.ID);
				num++;
			}
			if (achievement.IsActiveLicenseAddedAchieve() && (!this.m_lastCheckLicenseAddedByAchieve.ContainsKey(achievement.ID) || utcNow.Ticks - this.m_lastCheckLicenseAddedByAchieve[achievement.ID] >= AchieveManager.CHECK_LICENSE_ADDED_ACHIEVE_DELAY_TICKS))
			{
				Log.Rachelle.Print("AchieveManager.CheckTimedTimedEventsAndLicenses(): checking on license added achieve {0} time {1}", new object[]
				{
					achievement.ID,
					dateTime
				});
				this.m_lastCheckLicenseAddedByAchieve[achievement.ID] = utcNow.Ticks;
				Network.CheckAccountLicenseAchieve(achievement.ID);
			}
		}
		if (num == 0)
		{
			return;
		}
		this.ValidateAchievesNow(null);
	}

	// Token: 0x060009D8 RID: 2520 RVA: 0x0002A810 File Offset: 0x00028A10
	private List<Achievement> GetActiveLicenseAddedAchieves()
	{
		List<Achievement> list = Enumerable.ToList<Achievement>(this.m_achievements.Values);
		return list.FindAll((Achievement obj) => obj.IsActiveLicenseAddedAchieve());
	}

	// Token: 0x040004A5 RID: 1189
	private static readonly long TIMED_ACHIEVE_VALIDATION_DELAY_TICKS = 600000000L;

	// Token: 0x040004A6 RID: 1190
	private static readonly long CHECK_LICENSE_ADDED_ACHIEVE_DELAY_TICKS = (long)((ulong)-1294967296);

	// Token: 0x040004A7 RID: 1191
	private static readonly long TIMED_AND_LICENSE_ACHIEVE_CHECK_DELAY_TICKS = Math.Min(AchieveManager.TIMED_ACHIEVE_VALIDATION_DELAY_TICKS, AchieveManager.CHECK_LICENSE_ADDED_ACHIEVE_DELAY_TICKS);

	// Token: 0x040004A8 RID: 1192
	private static AchieveManager s_instance = null;

	// Token: 0x040004A9 RID: 1193
	private Map<int, Achievement> m_achievements = new Map<int, Achievement>();

	// Token: 0x040004AA RID: 1194
	private bool m_allNetAchievesReceived;

	// Token: 0x040004AB RID: 1195
	private bool m_waitingForActiveAchieves;

	// Token: 0x040004AC RID: 1196
	private int m_numEventResponsesNeeded;

	// Token: 0x040004AD RID: 1197
	private HashSet<int> m_achieveValidationsToRequest = new HashSet<int>();

	// Token: 0x040004AE RID: 1198
	private HashSet<int> m_achieveValidationsRequested = new HashSet<int>();

	// Token: 0x040004AF RID: 1199
	private bool m_disableCancelButtonUntilServerReturns;

	// Token: 0x040004B0 RID: 1200
	private Map<int, long> m_lastEventTimingValidationByAchieve = new Map<int, long>();

	// Token: 0x040004B1 RID: 1201
	private Map<int, long> m_lastCheckLicenseAddedByAchieve = new Map<int, long>();

	// Token: 0x040004B2 RID: 1202
	private long m_lastEventTimingAndLicenseAchieveCheck;

	// Token: 0x040004B3 RID: 1203
	private List<AchieveManager.AchieveCanceledListener> m_achieveCanceledListeners = new List<AchieveManager.AchieveCanceledListener>();

	// Token: 0x040004B4 RID: 1204
	private List<AchieveManager.ActiveAchievesUpdatedListener> m_activeAchievesUpdatedListeners = new List<AchieveManager.ActiveAchievesUpdatedListener>();

	// Token: 0x040004B5 RID: 1205
	private List<AchieveManager.NewlyCompletedAchievesListener> m_newlyCompletedAchievesListeners = new List<AchieveManager.NewlyCompletedAchievesListener>();

	// Token: 0x040004B6 RID: 1206
	private List<AchieveManager.LicenseAddedAchievesUpdatedListener> m_licenseAddedAchievesUpdatedListeners = new List<AchieveManager.LicenseAddedAchievesUpdatedListener>();

	// Token: 0x02000169 RID: 361
	// (Invoke) Token: 0x060013A3 RID: 5027
	public delegate void NewlyCompletedAchievesCallback(List<Achievement> achievements, object userData);

	// Token: 0x0200040D RID: 1037
	// (Invoke) Token: 0x060034D2 RID: 13522
	public delegate void ActiveAchievesUpdatedCallback(object userData);

	// Token: 0x0200040E RID: 1038
	// (Invoke) Token: 0x060034D6 RID: 13526
	public delegate void LicenseAddedAchievesUpdatedCallback(List<Achievement> activeLicenseAddedAchieves, object userData);

	// Token: 0x0200048A RID: 1162
	// (Invoke) Token: 0x0600381E RID: 14366
	public delegate void AchieveCanceledCallback(int achieveID, bool success, object userData);

	// Token: 0x0200048B RID: 1163
	private class AchieveCanceledListener : EventListener<AchieveManager.AchieveCanceledCallback>
	{
		// Token: 0x06003822 RID: 14370 RVA: 0x001135F1 File Offset: 0x001117F1
		public void Fire(int achieveID, bool success)
		{
			this.m_callback(achieveID, success, this.m_userData);
		}
	}

	// Token: 0x0200048C RID: 1164
	private class ActiveAchievesUpdatedListener : EventListener<AchieveManager.ActiveAchievesUpdatedCallback>
	{
		// Token: 0x06003824 RID: 14372 RVA: 0x0011360E File Offset: 0x0011180E
		public void Fire()
		{
			this.m_callback(this.m_userData);
		}
	}

	// Token: 0x0200048D RID: 1165
	private class NewlyCompletedAchievesListener : EventListener<AchieveManager.NewlyCompletedAchievesCallback>
	{
		// Token: 0x06003826 RID: 14374 RVA: 0x00113629 File Offset: 0x00111829
		public void Fire(List<Achievement> achievements)
		{
			this.m_callback(achievements, this.m_userData);
		}
	}

	// Token: 0x0200048E RID: 1166
	private class LicenseAddedAchievesUpdatedListener : EventListener<AchieveManager.LicenseAddedAchievesUpdatedCallback>
	{
		// Token: 0x06003828 RID: 14376 RVA: 0x00113645 File Offset: 0x00111845
		public void Fire(List<Achievement> activeLicenseAddedAchieves)
		{
			this.m_callback(activeLicenseAddedAchieves, this.m_userData);
		}
	}
}
