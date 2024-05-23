using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000006 RID: 6
public class AdventureProgressMgr
{
	// Token: 0x06000020 RID: 32 RVA: 0x00002B1C File Offset: 0x00000D1C
	private AdventureProgressMgr()
	{
		this.LoadAdventureMissionsFromDBF();
	}

	// Token: 0x06000022 RID: 34 RVA: 0x00002B63 File Offset: 0x00000D63
	public static AdventureProgressMgr Get()
	{
		return AdventureProgressMgr.s_instance;
	}

	// Token: 0x06000023 RID: 35 RVA: 0x00002B6C File Offset: 0x00000D6C
	public static void Init()
	{
		if (AdventureProgressMgr.s_instance == null)
		{
			AdventureProgressMgr.s_instance = new AdventureProgressMgr();
			Network network = Network.Get();
			network.RegisterNetHandler(306, new Network.NetHandler(AdventureProgressMgr.s_instance.OnAdventureProgress), null);
			NetCache.Get().RegisterNewNoticesListener(new NetCache.DelNewNoticesListener(AdventureProgressMgr.s_instance.OnNewNotices));
			ApplicationMgr.Get().WillReset += new Action(AdventureProgressMgr.s_instance.WillReset);
		}
	}

	// Token: 0x06000024 RID: 36 RVA: 0x00002BE9 File Offset: 0x00000DE9
	public static void InitRequests()
	{
		Network.RequestAdventureProgress();
	}

	// Token: 0x06000025 RID: 37 RVA: 0x00002BF0 File Offset: 0x00000DF0
	public bool RegisterProgressUpdatedListener(AdventureProgressMgr.AdventureProgressUpdatedCallback callback)
	{
		return this.RegisterProgressUpdatedListener(callback, null);
	}

	// Token: 0x06000026 RID: 38 RVA: 0x00002BFC File Offset: 0x00000DFC
	public bool RegisterProgressUpdatedListener(AdventureProgressMgr.AdventureProgressUpdatedCallback callback, object userData)
	{
		if (callback == null)
		{
			return false;
		}
		AdventureProgressMgr.AdventureProgressUpdatedListener adventureProgressUpdatedListener = new AdventureProgressMgr.AdventureProgressUpdatedListener();
		adventureProgressUpdatedListener.SetCallback(callback);
		adventureProgressUpdatedListener.SetUserData(userData);
		if (this.m_progressUpdatedListeners.Contains(adventureProgressUpdatedListener))
		{
			return false;
		}
		this.m_progressUpdatedListeners.Add(adventureProgressUpdatedListener);
		return true;
	}

	// Token: 0x06000027 RID: 39 RVA: 0x00002C45 File Offset: 0x00000E45
	public bool RemoveProgressUpdatedListener(AdventureProgressMgr.AdventureProgressUpdatedCallback callback)
	{
		return this.RemoveProgressUpdatedListener(callback, null);
	}

	// Token: 0x06000028 RID: 40 RVA: 0x00002C50 File Offset: 0x00000E50
	public bool RemoveProgressUpdatedListener(AdventureProgressMgr.AdventureProgressUpdatedCallback callback, object userData)
	{
		if (callback == null)
		{
			return false;
		}
		AdventureProgressMgr.AdventureProgressUpdatedListener adventureProgressUpdatedListener = new AdventureProgressMgr.AdventureProgressUpdatedListener();
		adventureProgressUpdatedListener.SetCallback(callback);
		adventureProgressUpdatedListener.SetUserData(userData);
		if (!this.m_progressUpdatedListeners.Contains(adventureProgressUpdatedListener))
		{
			return false;
		}
		this.m_progressUpdatedListeners.Remove(adventureProgressUpdatedListener);
		return true;
	}

	// Token: 0x06000029 RID: 41 RVA: 0x00002C9A File Offset: 0x00000E9A
	public List<AdventureMission.WingProgress> GetAllProgress()
	{
		return new List<AdventureMission.WingProgress>(this.m_wingProgress.Values);
	}

	// Token: 0x0600002A RID: 42 RVA: 0x00002CAC File Offset: 0x00000EAC
	public AdventureMission.WingProgress GetProgress(int wing)
	{
		if (!this.m_wingProgress.ContainsKey(wing))
		{
			return null;
		}
		return this.m_wingProgress[wing];
	}

	// Token: 0x0600002B RID: 43 RVA: 0x00002CD0 File Offset: 0x00000ED0
	public bool OwnsOneOrMoreAdventureWings(AdventureDbId adventureID)
	{
		foreach (WingDbfRecord wingDbfRecord in GameDbf.Wing.GetRecords())
		{
			if (wingDbfRecord.AdventureId == (int)adventureID)
			{
				if (this.OwnsWing(wingDbfRecord.ID))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0600002C RID: 44 RVA: 0x00002D54 File Offset: 0x00000F54
	public bool OwnsWing(int wing)
	{
		return this.m_wingProgress.ContainsKey(wing) && this.m_wingProgress[wing].IsOwned();
	}

	// Token: 0x0600002D RID: 45 RVA: 0x00002D7C File Offset: 0x00000F7C
	public bool IsWingComplete(AdventureDbId adventureID, AdventureModeDbId modeID, WingDbId wingId)
	{
		List<ScenarioDbfRecord> records = GameDbf.Scenario.GetRecords();
		int num = 0;
		int num2 = 0;
		foreach (ScenarioDbfRecord scenarioDbfRecord in records)
		{
			int adventureId = scenarioDbfRecord.AdventureId;
			if (adventureId == (int)adventureID)
			{
				int modeId = scenarioDbfRecord.ModeId;
				if (modeId == (int)modeID)
				{
					int wingId2 = scenarioDbfRecord.WingId;
					if (wingId2 == (int)wingId)
					{
						num2++;
						if (this.HasDefeatedScenario(scenarioDbfRecord.ID))
						{
							num++;
						}
					}
				}
			}
		}
		return num == num2;
	}

	// Token: 0x0600002E RID: 46 RVA: 0x00002E38 File Offset: 0x00001038
	public bool IsWingLocked(int wingId)
	{
		if (wingId == 14)
		{
			bool flag = this.IsWingComplete(AdventureDbId.LOE, AdventureModeDbId.NORMAL, WingDbId.LOE_TEMPLE_OF_ORSIS);
			bool flag2 = this.IsWingComplete(AdventureDbId.LOE, AdventureModeDbId.NORMAL, WingDbId.LOE_ULDAMAN);
			bool flag3 = this.IsWingComplete(AdventureDbId.LOE, AdventureModeDbId.NORMAL, WingDbId.LOE_RUINED_CITY);
			return !flag || !flag2 || !flag3;
		}
		return false;
	}

	// Token: 0x0600002F RID: 47 RVA: 0x00002E84 File Offset: 0x00001084
	public int GetPlayableAdventureScenarios(AdventureDbId adventureID, AdventureModeDbId modeID)
	{
		List<WingDbfRecord> records = GameDbf.Wing.GetRecords();
		List<ScenarioDbfRecord> records2 = GameDbf.Scenario.GetRecords();
		int num = 0;
		foreach (WingDbfRecord wingDbfRecord in records)
		{
			int id = wingDbfRecord.ID;
			int adventureId = wingDbfRecord.AdventureId;
			AdventureDbId adventureDbId = (AdventureDbId)adventureId;
			if (adventureDbId == adventureID)
			{
				if (this.OwnsWing(id) && this.IsWingOpen(id))
				{
					foreach (ScenarioDbfRecord scenarioDbfRecord in records2)
					{
						if (wingDbfRecord.ID == scenarioDbfRecord.WingId)
						{
							if (scenarioDbfRecord.AdventureId == (int)adventureID)
							{
								if (scenarioDbfRecord.ModeId == (int)modeID)
								{
									if (!AdventureProgressMgr.Get().HasDefeatedScenario(scenarioDbfRecord.ID))
									{
										if (scenarioDbfRecord.ModeId != 3 || AdventureProgressMgr.Get().CanPlayScenario(scenarioDbfRecord.ID))
										{
											num++;
										}
									}
								}
							}
						}
					}
				}
			}
		}
		return num;
	}

	// Token: 0x06000030 RID: 48 RVA: 0x00003008 File Offset: 0x00001208
	public int GetPlayableClassChallenges(AdventureDbId adventureID, AdventureModeDbId modeID)
	{
		int num = 0;
		List<ScenarioDbfRecord> records = GameDbf.Scenario.GetRecords();
		foreach (ScenarioDbfRecord scenarioDbfRecord in records)
		{
			if (scenarioDbfRecord.AdventureId == (int)adventureID)
			{
				if (scenarioDbfRecord.ModeId == (int)modeID)
				{
					if (this.CanPlayScenario(scenarioDbfRecord.ID))
					{
						if (!this.HasDefeatedScenario(scenarioDbfRecord.ID))
						{
							num++;
						}
					}
				}
			}
		}
		return num;
	}

	// Token: 0x06000031 RID: 49 RVA: 0x000030B8 File Offset: 0x000012B8
	public List<CardRewardData> GetCardRewardsForWing(int wing, HashSet<RewardVisualTiming> rewardTimings)
	{
		List<RewardData> rewardsForWing = FixedRewardsMgr.Get().GetRewardsForWing(wing, rewardTimings);
		List<CardRewardData> list = new List<CardRewardData>();
		foreach (RewardData rewardData in rewardsForWing)
		{
			if (rewardData.RewardType == Reward.Type.CARD)
			{
				list.Add(rewardData as CardRewardData);
			}
		}
		return list;
	}

	// Token: 0x06000032 RID: 50 RVA: 0x00003134 File Offset: 0x00001334
	public SpecialEventType GetWingOpenEvent(int wing)
	{
		WingDbfRecord record = GameDbf.Wing.GetRecord(wing);
		if (record == null)
		{
			Debug.LogWarning(string.Format("AdventureProgressMgr.GetWingOpenEvent could not find DBF record for wing {0}, assuming it is has no open event", wing));
			return SpecialEventType.IGNORE;
		}
		string requiredEvent = record.RequiredEvent;
		SpecialEventType result;
		if (!EnumUtils.TryGetEnum<SpecialEventType>(requiredEvent, out result))
		{
			Debug.LogWarning(string.Format("AdventureProgressMgr.GetWingOpenEvent wing={0} could not find SpecialEventType record for event '{1}'", wing, requiredEvent));
			return SpecialEventType.IGNORE;
		}
		return result;
	}

	// Token: 0x06000033 RID: 51 RVA: 0x00003198 File Offset: 0x00001398
	public string GetWingName(int wing)
	{
		WingDbfRecord record = GameDbf.Wing.GetRecord(wing);
		if (record == null)
		{
			Debug.LogWarning(string.Format("AdventureProgressMgr.GetWingName could not find DBF record for wing {0}", wing));
			return string.Empty;
		}
		return record.Name;
	}

	// Token: 0x06000034 RID: 52 RVA: 0x000031E0 File Offset: 0x000013E0
	public bool IsWingOpen(int wing)
	{
		SpecialEventType wingOpenEvent = this.GetWingOpenEvent(wing);
		return SpecialEventManager.Get().IsEventActive(wingOpenEvent, false);
	}

	// Token: 0x06000035 RID: 53 RVA: 0x00003204 File Offset: 0x00001404
	public bool CanPlayScenario(int scenarioID)
	{
		if (DemoMgr.Get().GetMode() == DemoMode.BLIZZCON_2015 && scenarioID != 1061)
		{
			return false;
		}
		if (!this.m_missions.ContainsKey(scenarioID))
		{
			return true;
		}
		AdventureMission adventureMission = this.m_missions[scenarioID];
		if (!adventureMission.HasRequiredProgress())
		{
			return true;
		}
		AdventureMission.WingProgress progress = this.GetProgress(adventureMission.RequiredProgress.Wing);
		return progress != null && progress.MeetsProgressAndFlagsRequirements(adventureMission.RequiredProgress);
	}

	// Token: 0x06000036 RID: 54 RVA: 0x00003284 File Offset: 0x00001484
	public bool HasDefeatedScenario(int scenarioID)
	{
		AdventureMission adventureMission;
		if (!this.m_missions.TryGetValue(scenarioID, out adventureMission))
		{
			return false;
		}
		if (adventureMission.RequiredProgress == null)
		{
			return false;
		}
		AdventureMission.WingProgress progress = this.GetProgress(adventureMission.RequiredProgress.Wing);
		if (progress == null)
		{
			return false;
		}
		bool result;
		if (GameUtils.IsHeroicAdventureMission(scenarioID))
		{
			result = progress.MeetsFlagsRequirement(adventureMission.GrantedProgress.Flags);
		}
		else if (GameUtils.IsClassChallengeMission(scenarioID))
		{
			result = progress.MeetsFlagsRequirement(adventureMission.GrantedProgress.Flags);
		}
		else
		{
			result = progress.MeetsProgressRequirement(adventureMission.GrantedProgress.Progress);
		}
		return result;
	}

	// Token: 0x06000037 RID: 55 RVA: 0x00003324 File Offset: 0x00001524
	public List<CardRewardData> GetImmediateCardRewardsForDefeatingScenario(int scenarioID)
	{
		HashSet<RewardVisualTiming> hashSet = new HashSet<RewardVisualTiming>();
		hashSet.Add(RewardVisualTiming.IMMEDIATE);
		HashSet<RewardVisualTiming> rewardTimings = hashSet;
		return this.GetCardRewardsForDefeatingScenario(scenarioID, rewardTimings);
	}

	// Token: 0x06000038 RID: 56 RVA: 0x0000334C File Offset: 0x0000154C
	public List<CardRewardData> GetCardRewardsForDefeatingScenario(int scenarioID, HashSet<RewardVisualTiming> rewardTimings)
	{
		AdventureMission adventureMission;
		if (!this.m_missions.TryGetValue(scenarioID, out adventureMission))
		{
			return new List<CardRewardData>();
		}
		List<RewardData> list = null;
		if (GameUtils.IsHeroicAdventureMission(scenarioID))
		{
			list = FixedRewardsMgr.Get().GetRewardsForWingFlags(adventureMission.GrantedProgress.Wing, adventureMission.GrantedProgress.Flags, rewardTimings);
		}
		else if (GameUtils.IsClassChallengeMission(scenarioID))
		{
			list = FixedRewardsMgr.Get().GetRewardsForWingFlags(adventureMission.GrantedProgress.Wing, adventureMission.GrantedProgress.Flags, rewardTimings);
		}
		else if (adventureMission.GrantedProgress != null)
		{
			list = FixedRewardsMgr.Get().GetRewardsForWingProgress(adventureMission.GrantedProgress.Wing, adventureMission.GrantedProgress.Progress, rewardTimings);
		}
		List<CardRewardData> list2 = new List<CardRewardData>();
		if (list != null)
		{
			foreach (RewardData rewardData in list)
			{
				if (rewardData.RewardType == Reward.Type.CARD)
				{
					list2.Add(rewardData as CardRewardData);
				}
			}
		}
		return list2;
	}

	// Token: 0x06000039 RID: 57 RVA: 0x00003470 File Offset: 0x00001670
	public bool SetWingAck(int wing, int ackId)
	{
		int num;
		if (this.m_wingAckState.TryGetValue(wing, out num))
		{
			if (ackId < num)
			{
				return false;
			}
			if (ackId == num)
			{
				return true;
			}
		}
		this.m_wingAckState[wing] = ackId;
		Network.AckWingProgress(wing, ackId);
		return true;
	}

	// Token: 0x0600003A RID: 58 RVA: 0x000034B7 File Offset: 0x000016B7
	public bool GetWingAck(int wing, out int ack)
	{
		return this.m_wingAckState.TryGetValue(wing, out ack);
	}

	// Token: 0x0600003B RID: 59 RVA: 0x000034C8 File Offset: 0x000016C8
	public AdventureOption[] GetAdventureOptions()
	{
		NetCache.NetCacheProfileProgress netObject = NetCache.Get().GetNetObject<NetCache.NetCacheProfileProgress>();
		return (netObject != null) ? netObject.AdventureOptions : null;
	}

	// Token: 0x0600003C RID: 60 RVA: 0x000034F4 File Offset: 0x000016F4
	public void SetAdventureOptions(int id, ulong options)
	{
		if (id == 0)
		{
			return;
		}
		NetCache.NetCacheProfileProgress netObject = NetCache.Get().GetNetObject<NetCache.NetCacheProfileProgress>();
		if (netObject != null && netObject.AdventureOptions != null)
		{
			foreach (AdventureOption adventureOption in netObject.AdventureOptions)
			{
				if (adventureOption.AdventureID == id)
				{
					adventureOption.Options = options;
					NetCache.Get().NetCacheChanged<NetCache.NetCacheProfileProgress>();
					break;
				}
			}
		}
		Network.SetAdventureOptions(id, options);
	}

	// Token: 0x0600003D RID: 61 RVA: 0x0000356C File Offset: 0x0000176C
	private void LoadAdventureMissionsFromDBF()
	{
		List<AdventureMissionDbfRecord> records = GameDbf.AdventureMission.GetRecords();
		foreach (AdventureMissionDbfRecord adventureMissionDbfRecord in records)
		{
			int scenarioId = adventureMissionDbfRecord.ScenarioId;
			if (this.m_missions.ContainsKey(scenarioId))
			{
				Debug.LogWarning(string.Format("AdventureProgressMgr.LoadAdventureMissionsFromDBF(): duplicate entry found for scenario ID {0}", scenarioId));
			}
			else
			{
				string noteDesc = adventureMissionDbfRecord.NoteDesc;
				AdventureMission.WingProgress requiredProgress = new AdventureMission.WingProgress(adventureMissionDbfRecord.ReqWingId, adventureMissionDbfRecord.ReqProgress, adventureMissionDbfRecord.ReqFlags);
				AdventureMission.WingProgress grantedProgress = new AdventureMission.WingProgress(adventureMissionDbfRecord.GrantsWingId, adventureMissionDbfRecord.GrantsProgress, adventureMissionDbfRecord.GrantsFlags);
				this.m_missions[scenarioId] = new AdventureMission(scenarioId, noteDesc, requiredProgress, grantedProgress);
			}
		}
	}

	// Token: 0x0600003E RID: 62 RVA: 0x0000364C File Offset: 0x0000184C
	private void WillReset()
	{
		this.m_wingProgress.Clear();
		this.m_wingAckState.Clear();
		this.m_progressUpdatedListeners.Clear();
	}

	// Token: 0x0600003F RID: 63 RVA: 0x00003670 File Offset: 0x00001870
	private void OnAdventureProgress()
	{
		List<Network.AdventureProgress> adventureProgressResponse = Network.GetAdventureProgressResponse();
		foreach (Network.AdventureProgress adventureProgress in adventureProgressResponse)
		{
			this.CreateOrUpdateProgress(true, adventureProgress.Wing, adventureProgress.Progress);
			this.CreateOrUpdateWingFlags(true, adventureProgress.Wing, adventureProgress.Flags);
			this.CreateOrUpdateWingAck(adventureProgress.Wing, adventureProgress.Ack);
		}
	}

	// Token: 0x06000040 RID: 64 RVA: 0x000036FC File Offset: 0x000018FC
	private void OnNewNotices(List<NetCache.ProfileNotice> newNotices)
	{
		List<long> list = new List<long>();
		foreach (NetCache.ProfileNotice profileNotice in newNotices)
		{
			if (profileNotice.Type == NetCache.ProfileNotice.NoticeType.ADVENTURE_PROGRESS)
			{
				NetCache.ProfileNoticeAdventureProgress profileNoticeAdventureProgress = profileNotice as NetCache.ProfileNoticeAdventureProgress;
				if (profileNoticeAdventureProgress.Progress != null)
				{
					this.CreateOrUpdateProgress(false, profileNoticeAdventureProgress.Wing, profileNoticeAdventureProgress.Progress.Value);
				}
				if (profileNoticeAdventureProgress.Flags != null)
				{
					this.CreateOrUpdateWingFlags(false, profileNoticeAdventureProgress.Wing, profileNoticeAdventureProgress.Flags.Value);
				}
				list.Add(profileNotice.NoticeID);
			}
		}
		foreach (long id in list)
		{
			Network.AckNotice(id);
		}
	}

	// Token: 0x06000041 RID: 65 RVA: 0x0000381C File Offset: 0x00001A1C
	private void FireProgressUpdate(bool isStartupAction, AdventureMission.WingProgress oldProgress, AdventureMission.WingProgress newProgress)
	{
		AdventureProgressMgr.AdventureProgressUpdatedListener[] array = this.m_progressUpdatedListeners.ToArray();
		foreach (AdventureProgressMgr.AdventureProgressUpdatedListener adventureProgressUpdatedListener in array)
		{
			adventureProgressUpdatedListener.Fire(isStartupAction, oldProgress, newProgress);
		}
	}

	// Token: 0x06000042 RID: 66 RVA: 0x00003858 File Offset: 0x00001A58
	private void CreateOrUpdateProgress(bool isStartupAction, int wing, int progress)
	{
		if (!this.m_wingProgress.ContainsKey(wing))
		{
			this.m_wingProgress[wing] = new AdventureMission.WingProgress(wing, progress, 0UL);
			Log.Rachelle.Print("AdventureProgressMgr.CreateOrUpdateProgress: creating wing {0} : PROGRESS {1}", new object[]
			{
				wing,
				this.m_wingProgress[wing]
			});
			this.FireProgressUpdate(isStartupAction, null, this.m_wingProgress[wing]);
			return;
		}
		AdventureMission.WingProgress wingProgress = this.m_wingProgress[wing].Clone();
		this.m_wingProgress[wing].SetProgress(progress);
		Log.Rachelle.Print("AdventureProgressMgr.CreateOrUpdateProgress: updating wing {0} : PROGRESS {1} (former progress {2})", new object[]
		{
			wing,
			this.m_wingProgress[wing],
			wingProgress
		});
		this.FireProgressUpdate(isStartupAction, wingProgress, this.m_wingProgress[wing]);
	}

	// Token: 0x06000043 RID: 67 RVA: 0x00003938 File Offset: 0x00001B38
	private void CreateOrUpdateWingFlags(bool isStartupAction, int wing, ulong flags)
	{
		if (!this.m_wingProgress.ContainsKey(wing))
		{
			this.m_wingProgress[wing] = new AdventureMission.WingProgress(wing, 0, flags);
			Log.Rachelle.Print("AdventureProgressMgr.CreateOrUpdateWingFlags: creating wing {0} : PROGRESS {1}", new object[]
			{
				wing,
				this.m_wingProgress[wing]
			});
			this.FireProgressUpdate(isStartupAction, null, this.m_wingProgress[wing]);
			return;
		}
		AdventureMission.WingProgress wingProgress = this.m_wingProgress[wing].Clone();
		this.m_wingProgress[wing].SetFlags(flags);
		Log.Rachelle.Print("AdventureProgressMgr.CreateOrUpdateWingFlags: updating wing {0} : PROGRESS {1} (former flags {2})", new object[]
		{
			wing,
			this.m_wingProgress[wing],
			wingProgress
		});
		this.FireProgressUpdate(isStartupAction, wingProgress, this.m_wingProgress[wing]);
	}

	// Token: 0x06000044 RID: 68 RVA: 0x00003A15 File Offset: 0x00001C15
	private void CreateOrUpdateWingAck(int wing, int ack)
	{
		this.m_wingAckState[wing] = ack;
	}

	// Token: 0x04000015 RID: 21
	private static AdventureProgressMgr s_instance;

	// Token: 0x04000016 RID: 22
	private Map<int, AdventureMission.WingProgress> m_wingProgress = new Map<int, AdventureMission.WingProgress>();

	// Token: 0x04000017 RID: 23
	private Map<int, int> m_wingAckState = new Map<int, int>();

	// Token: 0x04000018 RID: 24
	private Map<int, AdventureMission> m_missions = new Map<int, AdventureMission>();

	// Token: 0x04000019 RID: 25
	private List<AdventureProgressMgr.AdventureProgressUpdatedListener> m_progressUpdatedListeners = new List<AdventureProgressMgr.AdventureProgressUpdatedListener>();

	// Token: 0x02000007 RID: 7
	private class AdventureProgressUpdatedListener : EventListener<AdventureProgressMgr.AdventureProgressUpdatedCallback>
	{
		// Token: 0x06000046 RID: 70 RVA: 0x00003A2C File Offset: 0x00001C2C
		public void Fire(bool isStartupAction, AdventureMission.WingProgress oldProgress, AdventureMission.WingProgress newProgress)
		{
			this.m_callback(isStartupAction, oldProgress, newProgress, this.m_userData);
		}
	}

	// Token: 0x02000009 RID: 9
	// (Invoke) Token: 0x06000050 RID: 80
	public delegate void AdventureProgressUpdatedCallback(bool isStartupAction, AdventureMission.WingProgress oldProgress, AdventureMission.WingProgress newProgress, object userData);
}
