using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

// Token: 0x02000168 RID: 360
public class Achievement
{
	// Token: 0x06001370 RID: 4976 RVA: 0x00057198 File Offset: 0x00055398
	public Achievement(int id, bool enabled, Achievement.AchType achieveGroup, int maxProgress, Achievement.Trigger trigger, TAG_RACE? raceReq, TAG_CLASS? classReq, TAG_CARD_SET? cardSetReq, Achievement.ClickTriggerType? clickType, SpecialEventType eventTrigger, Achievement.UnlockableFeature? unlockedFeature, List<RewardData> rewards, RewardVisualTiming rewardTiming, int boosterReq)
	{
		this.m_id = id;
		this.m_enabled = enabled;
		this.m_group = achieveGroup;
		this.m_maxProgress = maxProgress;
		this.m_trigger = trigger;
		this.m_raceReq = raceReq;
		this.m_classReq = classReq;
		this.m_cardSetReq = cardSetReq;
		this.m_clickType = clickType;
		this.m_eventTrigger = eventTrigger;
		this.SetRewards(rewards);
		this.m_unlockedFeature = unlockedFeature;
		this.m_rewardTiming = rewardTiming;
		this.m_boosterReq = boosterReq;
		this.m_progress = 0;
		this.m_ackProgress = Achievement.NEW_ACHIEVE_ACK_PROGRESS;
		this.m_completionCount = 0;
		this.m_active = false;
		this.m_dateGiven = 0L;
		this.m_dateCompleted = 0L;
	}

	// Token: 0x170002F0 RID: 752
	// (get) Token: 0x06001372 RID: 4978 RVA: 0x00057283 File Offset: 0x00055483
	public int ID
	{
		get
		{
			return this.m_id;
		}
	}

	// Token: 0x170002F1 RID: 753
	// (get) Token: 0x06001373 RID: 4979 RVA: 0x0005728B File Offset: 0x0005548B
	public bool Enabled
	{
		get
		{
			return this.m_enabled;
		}
	}

	// Token: 0x170002F2 RID: 754
	// (get) Token: 0x06001374 RID: 4980 RVA: 0x00057293 File Offset: 0x00055493
	public Achievement.AchType AchieveType
	{
		get
		{
			return this.m_group;
		}
	}

	// Token: 0x170002F3 RID: 755
	// (get) Token: 0x06001375 RID: 4981 RVA: 0x0005729B File Offset: 0x0005549B
	public int MaxProgress
	{
		get
		{
			return this.m_maxProgress;
		}
	}

	// Token: 0x170002F4 RID: 756
	// (get) Token: 0x06001376 RID: 4982 RVA: 0x000572A3 File Offset: 0x000554A3
	public TAG_RACE? RaceRequirement
	{
		get
		{
			return this.m_raceReq;
		}
	}

	// Token: 0x170002F5 RID: 757
	// (get) Token: 0x06001377 RID: 4983 RVA: 0x000572AB File Offset: 0x000554AB
	public TAG_CLASS? ClassRequirement
	{
		get
		{
			return this.m_classReq;
		}
	}

	// Token: 0x170002F6 RID: 758
	// (get) Token: 0x06001378 RID: 4984 RVA: 0x000572B3 File Offset: 0x000554B3
	public TAG_CARD_SET? CardSetRequirement
	{
		get
		{
			return this.m_cardSetReq;
		}
	}

	// Token: 0x170002F7 RID: 759
	// (get) Token: 0x06001379 RID: 4985 RVA: 0x000572BB File Offset: 0x000554BB
	public Achievement.ClickTriggerType? ClickType
	{
		get
		{
			return this.m_clickType;
		}
	}

	// Token: 0x170002F8 RID: 760
	// (get) Token: 0x0600137A RID: 4986 RVA: 0x000572C3 File Offset: 0x000554C3
	public SpecialEventType EventTrigger
	{
		get
		{
			return this.m_eventTrigger;
		}
	}

	// Token: 0x170002F9 RID: 761
	// (get) Token: 0x0600137B RID: 4987 RVA: 0x000572CB File Offset: 0x000554CB
	public Achievement.Trigger AchieveTrigger
	{
		get
		{
			return this.m_trigger;
		}
	}

	// Token: 0x170002FA RID: 762
	// (get) Token: 0x0600137C RID: 4988 RVA: 0x000572D3 File Offset: 0x000554D3
	public Achievement.UnlockableFeature? UnlockedFeature
	{
		get
		{
			return this.m_unlockedFeature;
		}
	}

	// Token: 0x170002FB RID: 763
	// (get) Token: 0x0600137D RID: 4989 RVA: 0x000572DB File Offset: 0x000554DB
	public List<RewardData> Rewards
	{
		get
		{
			return this.m_rewards;
		}
	}

	// Token: 0x170002FC RID: 764
	// (get) Token: 0x0600137E RID: 4990 RVA: 0x000572E3 File Offset: 0x000554E3
	public RewardVisualTiming RewardTiming
	{
		get
		{
			return this.m_rewardTiming;
		}
	}

	// Token: 0x170002FD RID: 765
	// (get) Token: 0x0600137F RID: 4991 RVA: 0x000572EB File Offset: 0x000554EB
	public int BoosterRequirement
	{
		get
		{
			return this.m_boosterReq;
		}
	}

	// Token: 0x170002FE RID: 766
	// (get) Token: 0x06001380 RID: 4992 RVA: 0x000572F3 File Offset: 0x000554F3
	public int Progress
	{
		get
		{
			return this.m_progress;
		}
	}

	// Token: 0x170002FF RID: 767
	// (get) Token: 0x06001381 RID: 4993 RVA: 0x000572FB File Offset: 0x000554FB
	public int AcknowledgedProgress
	{
		get
		{
			return this.m_ackProgress;
		}
	}

	// Token: 0x17000300 RID: 768
	// (get) Token: 0x06001382 RID: 4994 RVA: 0x00057303 File Offset: 0x00055503
	public bool CanBeAcknowledged
	{
		get
		{
			return this.m_canAck;
		}
	}

	// Token: 0x17000301 RID: 769
	// (get) Token: 0x06001383 RID: 4995 RVA: 0x0005730B File Offset: 0x0005550B
	public int CompletionCount
	{
		get
		{
			return this.m_completionCount;
		}
	}

	// Token: 0x17000302 RID: 770
	// (get) Token: 0x06001384 RID: 4996 RVA: 0x00057313 File Offset: 0x00055513
	public bool Active
	{
		get
		{
			return this.m_active;
		}
	}

	// Token: 0x17000303 RID: 771
	// (get) Token: 0x06001385 RID: 4997 RVA: 0x0005731B File Offset: 0x0005551B
	public long DateGiven
	{
		get
		{
			return this.m_dateGiven;
		}
	}

	// Token: 0x17000304 RID: 772
	// (get) Token: 0x06001386 RID: 4998 RVA: 0x00057323 File Offset: 0x00055523
	public long DateCompleted
	{
		get
		{
			return this.m_dateCompleted;
		}
	}

	// Token: 0x17000305 RID: 773
	// (get) Token: 0x06001387 RID: 4999 RVA: 0x0005732B File Offset: 0x0005552B
	public bool IsLegendary
	{
		get
		{
			return (this.m_clientFlags & Achievement.ClientFlags.IS_LEGENDARY) != Achievement.ClientFlags.NONE;
		}
	}

	// Token: 0x17000306 RID: 774
	// (get) Token: 0x06001388 RID: 5000 RVA: 0x0005733C File Offset: 0x0005553C
	public bool CanShowInQuestLog
	{
		get
		{
			if ((this.m_clientFlags & Achievement.ClientFlags.SHOW_IN_QUEST_LOG) != Achievement.ClientFlags.NONE)
			{
				return true;
			}
			switch (this.AchieveType)
			{
			case Achievement.AchType.STARTER:
			case Achievement.AchType.DAILY_QUEST:
			case Achievement.AchType.NORMAL_QUEST:
				return true;
			case Achievement.AchType.UNLOCK_HERO:
			case Achievement.AchType.UNLOCK_GOLDEN_HERO:
			case Achievement.AchType.DAILY_REPEATABLE:
			case Achievement.AchType.HIDDEN:
			case Achievement.AchType.INTERNAL_ACTIVE:
			case Achievement.AchType.INTERNAL_INACTIVE:
				return false;
			}
			return false;
		}
	}

	// Token: 0x17000307 RID: 775
	// (get) Token: 0x06001389 RID: 5001 RVA: 0x00057399 File Offset: 0x00055599
	public string Name
	{
		get
		{
			if (!string.IsNullOrEmpty(this.m_altName) && AchieveManager.IsPredicateTrue(this.m_altTextPredicate))
			{
				return this.m_altName;
			}
			return this.m_name;
		}
	}

	// Token: 0x17000308 RID: 776
	// (get) Token: 0x0600138A RID: 5002 RVA: 0x000573C8 File Offset: 0x000555C8
	public string Description
	{
		get
		{
			if (!string.IsNullOrEmpty(this.m_altDescription) && AchieveManager.IsPredicateTrue(this.m_altTextPredicate))
			{
				return this.m_altDescription;
			}
			return this.m_description;
		}
	}

	// Token: 0x0600138B RID: 5003 RVA: 0x000573F7 File Offset: 0x000555F7
	public void SetClientFlags(Achievement.ClientFlags clientFlags)
	{
		this.m_clientFlags = clientFlags;
	}

	// Token: 0x0600138C RID: 5004 RVA: 0x00057400 File Offset: 0x00055600
	public void SetAltTextPredicate(string altTextPredicateName)
	{
		if (string.IsNullOrEmpty(altTextPredicateName))
		{
			this.m_altTextPredicate = Achievement.Predicate.NONE;
		}
		else if (!EnumUtils.TryGetEnum<Achievement.Predicate>(altTextPredicateName, 1, out this.m_altTextPredicate))
		{
			this.m_altTextPredicate = Achievement.Predicate.NONE;
			Error.AddDevFatal("Achievement id={0} name=\"{1}\" has unknown ALT_TEXT_PREDICATE: \"{2}\"", new object[]
			{
				this.ID,
				this.Name,
				altTextPredicateName
			});
		}
	}

	// Token: 0x0600138D RID: 5005 RVA: 0x00057468 File Offset: 0x00055668
	public void SetName(string name, string altName)
	{
		this.m_name = name;
		this.m_altName = altName;
	}

	// Token: 0x0600138E RID: 5006 RVA: 0x00057478 File Offset: 0x00055678
	public void SetDescription(string description, string altDescription)
	{
		this.m_description = description;
		this.m_altDescription = altDescription;
	}

	// Token: 0x0600138F RID: 5007 RVA: 0x00057488 File Offset: 0x00055688
	public void AddChildRewards(List<RewardData> childRewards)
	{
		List<RewardData> list = new List<RewardData>(childRewards);
		this.FixUpRewardOrigins(list);
		foreach (RewardData newRewardData in list)
		{
			RewardUtils.AddRewardDataToList(newRewardData, this.m_rewards);
		}
	}

	// Token: 0x06001390 RID: 5008 RVA: 0x000574F0 File Offset: 0x000556F0
	public void OnAchieveData(int progress, int acknowledgedProgress, int completionCount, bool isActive, long dateGiven, long dateCompleted, bool canAcknowledge)
	{
		this.SetProgress(progress);
		this.SetAcknowledgedProgress(acknowledgedProgress);
		this.m_completionCount = completionCount;
		this.m_active = isActive;
		this.m_dateGiven = dateGiven;
		this.m_dateCompleted = dateCompleted;
		this.m_canAck = canAcknowledge;
		this.AutoAckIfNeeded();
	}

	// Token: 0x06001391 RID: 5009 RVA: 0x00057530 File Offset: 0x00055730
	public void UpdateActiveAchieve(int progress, int acknowledgedProgress, long dateGiven, bool canAcknowledge)
	{
		this.SetProgress(progress);
		this.SetAcknowledgedProgress(acknowledgedProgress);
		this.m_active = true;
		this.m_dateGiven = dateGiven;
		this.m_canAck = canAcknowledge;
		this.AutoAckIfNeeded();
	}

	// Token: 0x06001392 RID: 5010 RVA: 0x00057568 File Offset: 0x00055768
	public void AddRewardNoticeID(long noticeID)
	{
		if (this.m_rewardNoticeIDs.Contains(noticeID))
		{
			return;
		}
		if (this.IsCompleted() && !this.NeedToAcknowledgeProgress(false))
		{
			Network.AckNotice(noticeID);
		}
		this.m_rewardNoticeIDs.Add(noticeID);
	}

	// Token: 0x06001393 RID: 5011 RVA: 0x000575B0 File Offset: 0x000557B0
	public void Complete()
	{
		Log.Achievements.Print("Complete: " + this, new object[0]);
		this.SetProgress(this.MaxProgress);
		this.m_completionCount++;
		this.m_active = false;
		this.m_dateCompleted = DateTime.UtcNow.ToFileTimeUtc();
		this.m_canAck = true;
		this.AutoAckIfNeeded();
	}

	// Token: 0x06001394 RID: 5012 RVA: 0x00057619 File Offset: 0x00055819
	public void OnCancelSuccess()
	{
		this.m_active = false;
	}

	// Token: 0x06001395 RID: 5013 RVA: 0x00057622 File Offset: 0x00055822
	public bool IsInternal()
	{
		return this.AchieveType == Achievement.AchType.INTERNAL_ACTIVE || Achievement.AchType.INTERNAL_INACTIVE == this.AchieveType;
	}

	// Token: 0x06001396 RID: 5014 RVA: 0x0005763C File Offset: 0x0005583C
	public bool IsNewlyActive()
	{
		return this.m_ackProgress == Achievement.NEW_ACHIEVE_ACK_PROGRESS;
	}

	// Token: 0x06001397 RID: 5015 RVA: 0x0005764B File Offset: 0x0005584B
	public bool IsCompleted()
	{
		return this.Progress >= this.MaxProgress;
	}

	// Token: 0x06001398 RID: 5016 RVA: 0x0005765E File Offset: 0x0005585E
	public bool IsActiveLicenseAddedAchieve()
	{
		return this.AchieveTrigger == Achievement.Trigger.ACCOUNT_LICENSE_ADDED && this.Active;
	}

	// Token: 0x06001399 RID: 5017 RVA: 0x00057674 File Offset: 0x00055874
	public void AckCurrentProgressAndRewardNotices()
	{
		this.AckCurrentProgressAndRewardNotices(false);
	}

	// Token: 0x0600139A RID: 5018 RVA: 0x00057680 File Offset: 0x00055880
	public void AckCurrentProgressAndRewardNotices(bool ackIntermediateProgress)
	{
		long[] array = this.m_rewardNoticeIDs.ToArray();
		this.m_rewardNoticeIDs.Clear();
		foreach (long id in array)
		{
			Network.AckNotice(id);
		}
		if (!this.NeedToAcknowledgeProgress(ackIntermediateProgress))
		{
			return;
		}
		this.m_ackProgress = this.Progress;
		if (!this.m_canAck)
		{
			return;
		}
		Network.AckAchieveProgress(this.ID, this.AcknowledgedProgress);
	}

	// Token: 0x0600139B RID: 5019 RVA: 0x000576FC File Offset: 0x000558FC
	public override string ToString()
	{
		return string.Format("[Achievement: ID={0} AchieveGroup={1} Name='{2}' MaxProgress={3} Progress={4} AckProgress={5} IsActive={6} DateGiven={7} DateCompleted={8} Description='{9}' Trigger={10}]", new object[]
		{
			this.ID,
			this.AchieveType,
			this.m_name,
			this.MaxProgress,
			this.Progress,
			this.AcknowledgedProgress,
			this.Active,
			this.DateGiven,
			this.DateCompleted,
			this.m_description,
			this.AchieveTrigger
		});
	}

	// Token: 0x0600139C RID: 5020 RVA: 0x000577AC File Offset: 0x000559AC
	private bool NeedToAcknowledgeProgress(bool ackIntermediateProgress)
	{
		return this.AcknowledgedProgress < this.MaxProgress && this.AcknowledgedProgress != this.Progress && (ackIntermediateProgress || this.Progress <= 0 || this.Progress == this.MaxProgress);
	}

	// Token: 0x0600139D RID: 5021 RVA: 0x00057805 File Offset: 0x00055A05
	private void SetProgress(int progress)
	{
		this.m_progress = Mathf.Clamp(progress, 0, this.MaxProgress);
	}

	// Token: 0x0600139E RID: 5022 RVA: 0x0005781A File Offset: 0x00055A1A
	private void SetAcknowledgedProgress(int acknowledgedProgress)
	{
		this.m_ackProgress = Mathf.Clamp(acknowledgedProgress, Achievement.NEW_ACHIEVE_ACK_PROGRESS, this.Progress);
	}

	// Token: 0x0600139F RID: 5023 RVA: 0x00057834 File Offset: 0x00055A34
	private void AutoAckIfNeeded()
	{
		if (!this.IsInternal() && Achievement.AchType.DAILY_REPEATABLE != this.AchieveType)
		{
			return;
		}
		this.AckCurrentProgressAndRewardNotices();
	}

	// Token: 0x060013A0 RID: 5024 RVA: 0x00057866 File Offset: 0x00055A66
	private void SetRewards(List<RewardData> rewardDataList)
	{
		this.m_rewards = new List<RewardData>(rewardDataList);
		this.FixUpRewardOrigins(this.m_rewards);
	}

	// Token: 0x060013A1 RID: 5025 RVA: 0x00057880 File Offset: 0x00055A80
	private void FixUpRewardOrigins(List<RewardData> rewardDataList)
	{
		foreach (RewardData rewardData in rewardDataList)
		{
			rewardData.SetOrigin(NetCache.ProfileNotice.NoticeOrigin.ACHIEVEMENT, (long)this.ID);
		}
	}

	// Token: 0x04000A01 RID: 2561
	private static readonly int NEW_ACHIEVE_ACK_PROGRESS = -1;

	// Token: 0x04000A02 RID: 2562
	private int m_id;

	// Token: 0x04000A03 RID: 2563
	private bool m_enabled;

	// Token: 0x04000A04 RID: 2564
	private string m_name = string.Empty;

	// Token: 0x04000A05 RID: 2565
	private string m_description = string.Empty;

	// Token: 0x04000A06 RID: 2566
	private Achievement.Predicate m_altTextPredicate;

	// Token: 0x04000A07 RID: 2567
	private string m_altName;

	// Token: 0x04000A08 RID: 2568
	private string m_altDescription;

	// Token: 0x04000A09 RID: 2569
	private Achievement.AchType m_group;

	// Token: 0x04000A0A RID: 2570
	private int m_maxProgress;

	// Token: 0x04000A0B RID: 2571
	private TAG_RACE? m_raceReq;

	// Token: 0x04000A0C RID: 2572
	private TAG_CLASS? m_classReq;

	// Token: 0x04000A0D RID: 2573
	private TAG_CARD_SET? m_cardSetReq;

	// Token: 0x04000A0E RID: 2574
	private Achievement.ClickTriggerType? m_clickType;

	// Token: 0x04000A0F RID: 2575
	private SpecialEventType m_eventTrigger;

	// Token: 0x04000A10 RID: 2576
	private Achievement.Trigger m_trigger;

	// Token: 0x04000A11 RID: 2577
	private Achievement.UnlockableFeature? m_unlockedFeature;

	// Token: 0x04000A12 RID: 2578
	private List<RewardData> m_rewards = new List<RewardData>();

	// Token: 0x04000A13 RID: 2579
	private RewardVisualTiming m_rewardTiming = RewardVisualTiming.IMMEDIATE;

	// Token: 0x04000A14 RID: 2580
	private int m_boosterReq;

	// Token: 0x04000A15 RID: 2581
	private Achievement.ClientFlags m_clientFlags;

	// Token: 0x04000A16 RID: 2582
	private int m_progress;

	// Token: 0x04000A17 RID: 2583
	private int m_ackProgress;

	// Token: 0x04000A18 RID: 2584
	private int m_completionCount;

	// Token: 0x04000A19 RID: 2585
	private bool m_active;

	// Token: 0x04000A1A RID: 2586
	private long m_dateGiven;

	// Token: 0x04000A1B RID: 2587
	private long m_dateCompleted;

	// Token: 0x04000A1C RID: 2588
	private bool m_canAck;

	// Token: 0x04000A1D RID: 2589
	private List<long> m_rewardNoticeIDs = new List<long>();

	// Token: 0x02000225 RID: 549
	public enum UnlockableFeature
	{
		// Token: 0x04001221 RID: 4641
		[Description("daily")]
		DAILY_QUESTS,
		// Token: 0x04001222 RID: 4642
		[Description("forge")]
		FORGE,
		// Token: 0x04001223 RID: 4643
		[Description("naxx1_owned")]
		NAXX_WING_1_OWNED,
		// Token: 0x04001224 RID: 4644
		[Description("naxx2_owned")]
		NAXX_WING_2_OWNED,
		// Token: 0x04001225 RID: 4645
		[Description("naxx3_owned")]
		NAXX_WING_3_OWNED,
		// Token: 0x04001226 RID: 4646
		[Description("naxx4_owned")]
		NAXX_WING_4_OWNED,
		// Token: 0x04001227 RID: 4647
		[Description("naxx5_owned")]
		NAXX_WING_5_OWNED,
		// Token: 0x04001228 RID: 4648
		[Description("naxx1_playable")]
		NAXX_WING_1_PLAYABLE,
		// Token: 0x04001229 RID: 4649
		[Description("naxx2_playable")]
		NAXX_WING_2_PLAYABLE,
		// Token: 0x0400122A RID: 4650
		[Description("naxx3_playable")]
		NAXX_WING_3_PLAYABLE,
		// Token: 0x0400122B RID: 4651
		[Description("naxx4_playable")]
		NAXX_WING_4_PLAYABLE,
		// Token: 0x0400122C RID: 4652
		[Description("naxx5_playable")]
		NAXX_WING_5_PLAYABLE,
		// Token: 0x0400122D RID: 4653
		[Description("vanilla heroes")]
		VANILLA_HEROES
	}

	// Token: 0x02000238 RID: 568
	public enum AchType
	{
		// Token: 0x040012A6 RID: 4774
		OTHER,
		// Token: 0x040012A7 RID: 4775
		[Description("starter")]
		STARTER,
		// Token: 0x040012A8 RID: 4776
		[Description("hero")]
		UNLOCK_HERO,
		// Token: 0x040012A9 RID: 4777
		[Description("goldhero")]
		UNLOCK_GOLDEN_HERO,
		// Token: 0x040012AA RID: 4778
		[Description("daily")]
		DAILY_QUEST,
		// Token: 0x040012AB RID: 4779
		[Description("daily_repeatable")]
		DAILY_REPEATABLE,
		// Token: 0x040012AC RID: 4780
		[Description("hidden")]
		HIDDEN,
		// Token: 0x040012AD RID: 4781
		[Description("internal_active")]
		INTERNAL_ACTIVE,
		// Token: 0x040012AE RID: 4782
		[Description("internal_inactive")]
		INTERNAL_INACTIVE,
		// Token: 0x040012AF RID: 4783
		[Description("login_activated")]
		LOGIN_ACTIVATED,
		// Token: 0x040012B0 RID: 4784
		[Description("normal_quest")]
		NORMAL_QUEST
	}

	// Token: 0x0200026C RID: 620
	public enum ClickTriggerType
	{
		// Token: 0x0400142F RID: 5167
		BUTTON_PLAY = 1,
		// Token: 0x04001430 RID: 5168
		BUTTON_ARENA
	}

	// Token: 0x02000487 RID: 1159
	public enum Trigger
	{
		// Token: 0x04002406 RID: 9222
		[Description("none")]
		IGNORE,
		// Token: 0x04002407 RID: 9223
		[Description("licenseadded")]
		ACCOUNT_LICENSE_ADDED,
		// Token: 0x04002408 RID: 9224
		[Description("adventure_progress")]
		ADVENTURE_PROGRESS,
		// Token: 0x04002409 RID: 9225
		[Description("click")]
		CLICK,
		// Token: 0x0400240A RID: 9226
		[Description("disenchant")]
		DISENCHANT,
		// Token: 0x0400240B RID: 9227
		[Description("cardset")]
		COMPLETE_CARD_SET,
		// Token: 0x0400240C RID: 9228
		[Description("event")]
		EVENT,
		// Token: 0x0400240D RID: 9229
		[Description("event_timing_only")]
		EVENT_TIMING_ONLY,
		// Token: 0x0400240E RID: 9230
		[Description("race")]
		GAIN_CARD,
		// Token: 0x0400240F RID: 9231
		[Description("goldrace")]
		GAIN_GOLDEN_CARD,
		// Token: 0x04002410 RID: 9232
		[Description("purchase")]
		PURCHASE,
		// Token: 0x04002411 RID: 9233
		[Description("win")]
		WIN,
		// Token: 0x04002412 RID: 9234
		[Description("pack_ready_to_open")]
		PACK_READY_TO_OPEN
	}

	// Token: 0x02000488 RID: 1160
	public enum Predicate
	{
		// Token: 0x04002414 RID: 9236
		[Description("none")]
		NONE,
		// Token: 0x04002415 RID: 9237
		[Description("can_see_wild")]
		CAN_SEE_WILD
	}

	// Token: 0x02000489 RID: 1161
	[Flags]
	public enum ClientFlags
	{
		// Token: 0x04002417 RID: 9239
		[Description("none")]
		NONE = 0,
		// Token: 0x04002418 RID: 9240
		[Description("is_legendary")]
		IS_LEGENDARY = 1,
		// Token: 0x04002419 RID: 9241
		[Description("show_in_quest_log")]
		SHOW_IN_QUEST_LOG = 2
	}
}
