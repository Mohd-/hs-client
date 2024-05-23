using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using PegasusShared;
using PegasusUtil;
using UnityEngine;

// Token: 0x02000116 RID: 278
public class TavernBrawlManager
{
	// Token: 0x14000006 RID: 6
	// (add) Token: 0x06000D45 RID: 3397 RVA: 0x00035378 File Offset: 0x00033578
	// (remove) Token: 0x06000D46 RID: 3398 RVA: 0x00035391 File Offset: 0x00033591
	public event Action OnTavernBrawlUpdated;

	// Token: 0x170001F3 RID: 499
	// (get) Token: 0x06000D47 RID: 3399 RVA: 0x000353AA File Offset: 0x000335AA
	public bool IsTavernBrawlActive
	{
		get
		{
			return this.CurrentMission() != null && this.CurrentTavernBrawlSeasonEnd > 0;
		}
	}

	// Token: 0x06000D48 RID: 3400 RVA: 0x000353C3 File Offset: 0x000335C3
	public TavernBrawlMission CurrentMission()
	{
		return this.m_currentMission;
	}

	// Token: 0x06000D49 RID: 3401 RVA: 0x000353CC File Offset: 0x000335CC
	public bool SelectHeroBeforeMission()
	{
		return this.m_currentMission != null && this.m_currentMission.canSelectHeroForDeck && !this.m_currentMission.canCreateDeck;
	}

	// Token: 0x06000D4A RID: 3402 RVA: 0x00035408 File Offset: 0x00033608
	public static bool IsInTavernBrawlFriendlyChallenge()
	{
		return (SceneMgr.Get().GetMode() == SceneMgr.Mode.TAVERN_BRAWL || SceneMgr.Get().GetMode() == SceneMgr.Mode.FRIENDLY) && FriendChallengeMgr.Get().IsChallengeTavernBrawl();
	}

	// Token: 0x06000D4B RID: 3403 RVA: 0x00035443 File Offset: 0x00033643
	public bool ShouldNewFriendlyChallengeBeTavernBrawl()
	{
		return SceneMgr.Get().GetMode() == SceneMgr.Mode.TAVERN_BRAWL && this.IsTavernBrawlActive;
	}

	// Token: 0x170001F4 RID: 500
	// (get) Token: 0x06000D4C RID: 3404 RVA: 0x00035460 File Offset: 0x00033660
	public bool HasUnlockedTavernBrawl
	{
		get
		{
			NetCache.NetCacheHeroLevels netObject = NetCache.Get().GetNetObject<NetCache.NetCacheHeroLevels>();
			if (netObject == null)
			{
				return false;
			}
			return Enumerable.Any<NetCache.HeroLevel>(netObject.Levels, (NetCache.HeroLevel l) => l.CurrentLevel.Level >= 20);
		}
	}

	// Token: 0x170001F5 RID: 501
	// (get) Token: 0x06000D4D RID: 3405 RVA: 0x000354A8 File Offset: 0x000336A8
	public bool IsFirstTimeSeeingThisFeature
	{
		get
		{
			return this.s_isFirstTimeSeeingThisFeature && this.IsTavernBrawlActive;
		}
	}

	// Token: 0x170001F6 RID: 502
	// (get) Token: 0x06000D4E RID: 3406 RVA: 0x000354BE File Offset: 0x000336BE
	public bool IsFirstTimeSeeingCurrentSeason
	{
		get
		{
			return this.s_isFirstTimeSeeingCurrentSeason && this.IsTavernBrawlActive;
		}
	}

	// Token: 0x170001F7 RID: 503
	// (get) Token: 0x06000D4F RID: 3407 RVA: 0x000354D4 File Offset: 0x000336D4
	public bool HasSeenTavernBrawlScreen
	{
		get
		{
			return Options.Get().GetInt(Option.LATEST_SEEN_TAVERNBRAWL_SEASON_CHALKBOARD) > 0;
		}
	}

	// Token: 0x170001F8 RID: 504
	// (get) Token: 0x06000D50 RID: 3408 RVA: 0x000354E8 File Offset: 0x000336E8
	public int CurrentTavernBrawlSeasonEnd
	{
		get
		{
			if (this.m_currentMission == null || this.m_currentMission.endDateLocal == null)
			{
				return -1;
			}
			return (int)(this.m_currentMission.endDateLocal.Value - DateTime.Now).TotalSeconds;
		}
	}

	// Token: 0x170001F9 RID: 505
	// (get) Token: 0x06000D51 RID: 3409 RVA: 0x0003553C File Offset: 0x0003373C
	public int NextTavernBrawlSeasonStart
	{
		get
		{
			if (this.m_nextSeasonStartDateLocal == null)
			{
				return -1;
			}
			return (int)(this.m_nextSeasonStartDateLocal.Value - DateTime.Now).TotalSeconds;
		}
	}

	// Token: 0x170001FA RID: 506
	// (get) Token: 0x06000D52 RID: 3410 RVA: 0x0003557C File Offset: 0x0003377C
	public float ScheduledSecondsToRefresh
	{
		get
		{
			if (this.m_scheduledRefreshTimeLocal == null)
			{
				return -1f;
			}
			return (float)(this.m_scheduledRefreshTimeLocal.Value - DateTime.Now).TotalSeconds;
		}
	}

	// Token: 0x170001FB RID: 507
	// (get) Token: 0x06000D53 RID: 3411 RVA: 0x000355BD File Offset: 0x000337BD
	// (set) Token: 0x06000D54 RID: 3412 RVA: 0x000355C5 File Offset: 0x000337C5
	public bool IsRefreshingTavernBrawlInfo { get; set; }

	// Token: 0x06000D55 RID: 3413 RVA: 0x000355D0 File Offset: 0x000337D0
	public int WinStreak()
	{
		return (this.MyRecord != null) ? this.MyRecord.WinStreak : 0;
	}

	// Token: 0x06000D56 RID: 3414 RVA: 0x000355FC File Offset: 0x000337FC
	public int GamesWon()
	{
		return (this.MyRecord != null) ? this.MyRecord.GamesWon : 0;
	}

	// Token: 0x06000D57 RID: 3415 RVA: 0x00035628 File Offset: 0x00033828
	public int RewardProgress()
	{
		return (this.MyRecord != null) ? this.MyRecord.RewardProgress : 0;
	}

	// Token: 0x06000D58 RID: 3416 RVA: 0x00035653 File Offset: 0x00033853
	public DeckRuleset GetDeckRuleset()
	{
		if (this.m_currentMission.deckRuleset != null)
		{
			return this.m_currentMission.deckRuleset;
		}
		return DeckRuleset.GetWildRuleset();
	}

	// Token: 0x06000D59 RID: 3417 RVA: 0x00035676 File Offset: 0x00033876
	public static void Init()
	{
		if (TavernBrawlManager.s_instance == null)
		{
			TavernBrawlManager.s_instance = new TavernBrawlManager();
		}
		TavernBrawlManager.s_instance.InitImpl();
	}

	// Token: 0x06000D5A RID: 3418 RVA: 0x00035696 File Offset: 0x00033896
	public static TavernBrawlManager Get()
	{
		if (TavernBrawlManager.s_instance == null)
		{
			Debug.LogError("Trying to retrieve the Tavern Brawl Manager without calling TavernBrawlManager.Init()!");
		}
		return TavernBrawlManager.s_instance;
	}

	// Token: 0x06000D5B RID: 3419 RVA: 0x000356B4 File Offset: 0x000338B4
	public void StartGame(long deckId = 0L)
	{
		if (this.m_currentMission == null)
		{
			Error.AddDevFatal("TB: m_currentMission is null", new object[0]);
			return;
		}
		PresenceMgr.Get().SetStatus(new Enum[]
		{
			PresenceStatus.TAVERN_BRAWL_QUEUE
		});
		GameMgr.Get().FindGame(16, this.m_currentMission.missionId, deckId, 0L);
	}

	// Token: 0x06000D5C RID: 3420 RVA: 0x00035714 File Offset: 0x00033914
	private bool OnFindGameEvent(FindGameEventData eventData, object userData)
	{
		if (!GameMgr.Get().IsNextTavernBrawl() || GameMgr.Get().IsNextSpectator())
		{
			return false;
		}
		switch (eventData.m_state)
		{
		case FindGameState.CLIENT_CANCELED:
		case FindGameState.CLIENT_ERROR:
		case FindGameState.BNET_QUEUE_CANCELED:
		case FindGameState.BNET_ERROR:
		case FindGameState.SERVER_GAME_CANCELED:
		{
			Enum[] status = PresenceMgr.Get().GetStatus();
			if (status != null && status.Length > 0 && (PresenceStatus)status[0] == PresenceStatus.TAVERN_BRAWL_QUEUE)
			{
				PresenceMgr.Get().SetPrevStatus();
			}
			break;
		}
		}
		return false;
	}

	// Token: 0x06000D5D RID: 3421 RVA: 0x000357B7 File Offset: 0x000339B7
	public bool HasCreatedDeck()
	{
		return this.CurrentDeck() != null;
	}

	// Token: 0x06000D5E RID: 3422 RVA: 0x000357C8 File Offset: 0x000339C8
	public CollectionDeck CurrentDeck()
	{
		foreach (CollectionDeck collectionDeck in CollectionManager.Get().GetDecks().Values)
		{
			if (collectionDeck.Type == 6)
			{
				return collectionDeck;
			}
		}
		return null;
	}

	// Token: 0x06000D5F RID: 3423 RVA: 0x0003583C File Offset: 0x00033A3C
	public bool HasValidDeck()
	{
		if (this.m_currentMission == null || !this.m_currentMission.canCreateDeck)
		{
			return false;
		}
		CollectionDeck collectionDeck = this.CurrentDeck();
		if (collectionDeck == null)
		{
			return false;
		}
		if (!collectionDeck.NetworkContentsLoaded())
		{
			CollectionManager.Get().RequestDeckContents(collectionDeck.ID);
			return false;
		}
		DeckRuleset deckRuleset = this.GetDeckRuleset();
		return deckRuleset == null || this.GetDeckRuleset().IsDeckValid(collectionDeck);
	}

	// Token: 0x06000D60 RID: 3424 RVA: 0x000358B4 File Offset: 0x00033AB4
	public void EnsureScenarioDataReady(TavernBrawlManager.CallbackEnsureServerDataReady callback = null)
	{
		if (this.IsScenarioDataReady)
		{
			if (callback != null)
			{
				callback();
			}
			return;
		}
		if (callback != null)
		{
			if (this.m_serverDataReadyCallbacks == null)
			{
				this.m_serverDataReadyCallbacks = new List<TavernBrawlManager.CallbackEnsureServerDataReady>();
			}
			this.m_serverDataReadyCallbacks.Add(callback);
		}
		TavernBrawlSpec currentTavernBrawl = this.ServerInfo.CurrentTavernBrawl;
		List<AssetRecordInfo> list = new List<AssetRecordInfo>();
		list.Add(new AssetRecordInfo
		{
			Asset = new AssetKey(),
			Asset = 
			{
				Type = 1,
				AssetId = currentTavernBrawl.ScenarioId
			},
			RecordByteSize = currentTavernBrawl.ScenarioRecordByteSize,
			RecordHash = currentTavernBrawl.ScenarioRecordHash
		});
		if (currentTavernBrawl.AdditionalAssets != null && currentTavernBrawl.AdditionalAssets.Count > 0)
		{
			list.AddRange(currentTavernBrawl.AdditionalAssets);
		}
		if (Enumerable.Any<KeyValuePair<int, KeyValuePair<AssetRecordInfo, TavernBrawlManager.LoadCachedAssetCallback>>>(TavernBrawlManager.s_assetRequests, (KeyValuePair<int, KeyValuePair<AssetRecordInfo, TavernBrawlManager.LoadCachedAssetCallback>> kv) => kv.Value.Key.Asset.AssetId == this.m_currentMission.missionId && kv.Value.Key.Asset.Type == 1))
		{
			TavernBrawlManager.LoadCachedAssets(false, new TavernBrawlManager.LoadCachedAssetCallback(this.OnTavernBrawlScenarioLoaded), list.ToArray());
			return;
		}
		if (ApplicationMgr.IsInternal())
		{
			ApplicationMgr.Get().ScheduleCallback(Mathf.Max(0f, Random.Range(-3f, 3f)), false, delegate(object userData)
			{
				TavernBrawlManager tavernBrawlManager = TavernBrawlManager.Get();
				if (tavernBrawlManager.IsScenarioDataReady)
				{
					if (callback != null)
					{
						if (tavernBrawlManager.m_serverDataReadyCallbacks != null)
						{
							tavernBrawlManager.m_serverDataReadyCallbacks.Remove(callback);
						}
						callback();
					}
					return;
				}
				TavernBrawlSpec currentTavernBrawl2 = tavernBrawlManager.ServerInfo.CurrentTavernBrawl;
				List<AssetRecordInfo> list2 = new List<AssetRecordInfo>();
				list2.Add(new AssetRecordInfo
				{
					Asset = new AssetKey(),
					Asset = 
					{
						Type = 1,
						AssetId = currentTavernBrawl2.ScenarioId
					},
					RecordByteSize = currentTavernBrawl2.ScenarioRecordByteSize,
					RecordHash = currentTavernBrawl2.ScenarioRecordHash
				});
				if (currentTavernBrawl2.AdditionalAssets != null && currentTavernBrawl2.AdditionalAssets.Count > 0)
				{
					list2.AddRange(currentTavernBrawl2.AdditionalAssets);
				}
				TavernBrawlManager.LoadCachedAssets(true, new TavernBrawlManager.LoadCachedAssetCallback(tavernBrawlManager.OnTavernBrawlScenarioLoaded), list2.ToArray());
			}, null);
			return;
		}
		TavernBrawlManager.LoadCachedAssets(true, new TavernBrawlManager.LoadCachedAssetCallback(this.OnTavernBrawlScenarioLoaded), list.ToArray());
	}

	// Token: 0x170001FC RID: 508
	// (get) Token: 0x06000D61 RID: 3425 RVA: 0x00035A3C File Offset: 0x00033C3C
	public bool IsTavernBrawlInfoReady
	{
		get
		{
			return NetCache.Get().GetNetObject<NetCache.NetCacheClientOptions>() != null && this.ServerInfo != null;
		}
	}

	// Token: 0x170001FD RID: 509
	// (get) Token: 0x06000D62 RID: 3426 RVA: 0x00035A6C File Offset: 0x00033C6C
	public bool IsScenarioDataReady
	{
		get
		{
			return this.m_currentMission == null || this.ServerInfo == null || !this.m_scenarioAssetPendingLoad;
		}
	}

	// Token: 0x06000D63 RID: 3427 RVA: 0x00035A9F File Offset: 0x00033C9F
	public void RefreshServerData()
	{
		NetCache.Get().ReloadNetObject<NetCache.NetCacheTavernBrawlInfo>();
	}

	// Token: 0x170001FE RID: 510
	// (get) Token: 0x06000D64 RID: 3428 RVA: 0x00035AAC File Offset: 0x00033CAC
	private TavernBrawlInfo ServerInfo
	{
		get
		{
			NetCache.NetCacheTavernBrawlInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheTavernBrawlInfo>();
			return (netObject != null) ? netObject.Info : null;
		}
	}

	// Token: 0x170001FF RID: 511
	// (get) Token: 0x06000D65 RID: 3429 RVA: 0x00035AD8 File Offset: 0x00033CD8
	private TavernBrawlPlayerRecord MyRecord
	{
		get
		{
			NetCache.NetCacheTavernBrawlRecord netObject = NetCache.Get().GetNetObject<NetCache.NetCacheTavernBrawlRecord>();
			return (netObject != null) ? netObject.Record : null;
		}
	}

	// Token: 0x06000D66 RID: 3430 RVA: 0x00035B04 File Offset: 0x00033D04
	private void InitImpl()
	{
		string text = string.Format("{0}/Cached", FileUtils.PersistentDataPath);
		if (Directory.Exists(text))
		{
			try
			{
				Directory.Delete(text, true);
			}
			catch (Exception)
			{
			}
		}
		NetCache.Get().RegisterUpdatedListener(typeof(NetCache.NetCacheTavernBrawlInfo), new Action(this.NetCache_OnTavernBrawlInfo));
		NetCache.Get().RegisterUpdatedListener(typeof(NetCache.NetCacheHeroLevels), new Action(this.NetCache_OnClientOptions));
		Network.Get().RegisterNetHandler(322, new Network.NetHandler(this.Network_OnGetAssetResponse), null);
		GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
		this.RegisterOptionsListeners(true);
	}

	// Token: 0x06000D67 RID: 3431 RVA: 0x00035BD0 File Offset: 0x00033DD0
	private void RegisterOptionsListeners(bool register)
	{
		if (register)
		{
			NetCache.Get().RegisterUpdatedListener(typeof(NetCache.NetCacheClientOptions), new Action(this.NetCache_OnClientOptions));
			Options.Get().RegisterChangedListener(Option.LATEST_SEEN_TAVERNBRAWL_SEASON, new Options.ChangedCallback(this.OnOptionChangedCallback));
			Options.Get().RegisterChangedListener(Option.LATEST_SEEN_TAVERNBRAWL_SEASON_CHALKBOARD, new Options.ChangedCallback(this.OnOptionChangedCallback));
		}
		else
		{
			NetCache.Get().RemoveUpdatedListener(typeof(NetCache.NetCacheClientOptions), new Action(this.NetCache_OnClientOptions));
			Options.Get().UnregisterChangedListener(Option.LATEST_SEEN_TAVERNBRAWL_SEASON, new Options.ChangedCallback(this.OnOptionChangedCallback));
			Options.Get().UnregisterChangedListener(Option.LATEST_SEEN_TAVERNBRAWL_SEASON_CHALKBOARD, new Options.ChangedCallback(this.OnOptionChangedCallback));
		}
	}

	// Token: 0x06000D68 RID: 3432 RVA: 0x00035C8C File Offset: 0x00033E8C
	private void NetCache_OnClientOptions()
	{
		this.RegisterOptionsListeners(false);
		this.CheckLatestSeenSeason(true);
		this.RegisterOptionsListeners(true);
	}

	// Token: 0x06000D69 RID: 3433 RVA: 0x00035CA3 File Offset: 0x00033EA3
	private void OnOptionChangedCallback(Option option, object prevValue, bool existed, object userData)
	{
		this.RegisterOptionsListeners(false);
		this.CheckLatestSeenSeason(false);
		this.RegisterOptionsListeners(true);
	}

	// Token: 0x06000D6A RID: 3434 RVA: 0x00035CBC File Offset: 0x00033EBC
	private void CheckLatestSeenSeason(bool canSetOption)
	{
		if (!this.IsTavernBrawlInfoReady)
		{
			return;
		}
		bool flag = !this.m_hasGottenClientOptionsAtLeastOnce;
		this.m_hasGottenClientOptionsAtLeastOnce = true;
		bool isFirstTimeSeeingThisFeature = this.IsFirstTimeSeeingThisFeature;
		bool isFirstTimeSeeingCurrentSeason = this.IsFirstTimeSeeingCurrentSeason;
		this.s_isFirstTimeSeeingThisFeature = false;
		this.s_isFirstTimeSeeingCurrentSeason = false;
		TavernBrawlInfo serverInfo = this.ServerInfo;
		if (serverInfo.HasCurrentTavernBrawl)
		{
			NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
			bool flag2 = netObject != null && netObject.Games.TavernBrawl && this.HasUnlockedTavernBrawl;
			TavernBrawlSpec currentTavernBrawl = serverInfo.CurrentTavernBrawl;
			int @int = Options.Get().GetInt(Option.LATEST_SEEN_TAVERNBRAWL_SEASON);
			if (@int == 0 && flag2)
			{
				this.s_isFirstTimeSeeingThisFeature = true;
				NotificationManager.Get().ForceRemoveSoundFromPlayedList("VO_INNKEEPER_TAVERNBRAWL_PUSH_32");
				NotificationManager.Get().ForceRemoveSoundFromPlayedList("VO_INNKEEPER_TAVERNBRAWL_WELCOME1_27");
			}
			if (@int < currentTavernBrawl.SeasonId && flag2)
			{
				this.s_isFirstTimeSeeingCurrentSeason = true;
				NotificationManager.Get().ForceRemoveSoundFromPlayedList("VO_INNKEEPER_TAVERNBRAWL_DESC2_30");
				Hub.s_hasAlreadyShownTBAnimation = false;
				if (canSetOption)
				{
					Options.Get().SetInt(Option.LATEST_SEEN_TAVERNBRAWL_SEASON, currentTavernBrawl.SeasonId);
				}
			}
		}
		if ((flag || isFirstTimeSeeingThisFeature != this.IsFirstTimeSeeingThisFeature || isFirstTimeSeeingCurrentSeason != this.IsFirstTimeSeeingCurrentSeason) && this.OnTavernBrawlUpdated != null)
		{
			this.OnTavernBrawlUpdated.Invoke();
		}
	}

	// Token: 0x06000D6B RID: 3435 RVA: 0x00035E0C File Offset: 0x0003400C
	private void NetCache_OnTavernBrawlInfo()
	{
		this.IsRefreshingTavernBrawlInfo = false;
		TavernBrawlInfo serverInfo = this.ServerInfo;
		if (serverInfo == null)
		{
			return;
		}
		if (serverInfo.HasCurrentTavernBrawl)
		{
			this.m_currentMission = new TavernBrawlMission();
			TavernBrawlSpec currentTavernBrawl = serverInfo.CurrentTavernBrawl;
			this.m_currentMission.seasonId = currentTavernBrawl.SeasonId;
			this.m_currentMission.missionId = currentTavernBrawl.ScenarioId;
			this.m_currentMission.rewardType = currentTavernBrawl.RewardType;
			this.m_currentMission.rewardTrigger = currentTavernBrawl.RewardTrigger;
			this.m_currentMission.RewardData1 = currentTavernBrawl.RewardData1;
			this.m_currentMission.RewardData2 = currentTavernBrawl.RewardData2;
			this.CheckLatestSeenSeason(true);
			if (currentTavernBrawl.HasEndSecondsFromNow)
			{
				this.m_currentMission.endDateLocal = new DateTime?(DateTime.Now + new TimeSpan(0, 0, (int)currentTavernBrawl.EndSecondsFromNow));
			}
			else
			{
				this.m_currentMission.endDateLocal = default(DateTime?);
			}
			this.m_scenarioAssetPendingLoad = true;
		}
		else
		{
			this.m_currentMission = null;
		}
		if (serverInfo.HasNextStartSecondsFromNow)
		{
			this.m_nextSeasonStartDateLocal = new DateTime?(DateTime.Now + new TimeSpan(0, 0, (int)serverInfo.NextStartSecondsFromNow));
		}
		else
		{
			this.m_nextSeasonStartDateLocal = default(DateTime?);
		}
		ApplicationMgr.Get().CancelScheduledCallback(new ApplicationMgr.ScheduledCallback(this.ScheduledEndOfCurrentTBCallback), null);
		int currentTavernBrawlSeasonEnd = this.CurrentTavernBrawlSeasonEnd;
		if (this.IsTavernBrawlActive && currentTavernBrawlSeasonEnd > 0)
		{
			Log.EventTiming.Print("Scheduling end of current TB {0} secs from now.", new object[]
			{
				currentTavernBrawlSeasonEnd
			});
			ApplicationMgr.Get().ScheduleCallback((float)currentTavernBrawlSeasonEnd, true, new ApplicationMgr.ScheduledCallback(this.ScheduledEndOfCurrentTBCallback), null);
		}
		ApplicationMgr.Get().CancelScheduledCallback(new ApplicationMgr.ScheduledCallback(this.ScheduledRefreshTBSpecCallback), null);
		int nextTavernBrawlSeasonStart = this.NextTavernBrawlSeasonStart;
		if (nextTavernBrawlSeasonStart >= 0)
		{
			this.m_scheduledRefreshTimeLocal = new DateTime?(DateTime.Now + new TimeSpan(0, 0, 0, nextTavernBrawlSeasonStart, 0));
			Log.EventTiming.Print("Scheduling TB refresh for {0} secs from now.", new object[]
			{
				nextTavernBrawlSeasonStart
			});
			ApplicationMgr.Get().ScheduleCallback((float)nextTavernBrawlSeasonStart, true, new ApplicationMgr.ScheduledCallback(this.ScheduledRefreshTBSpecCallback), null);
		}
		this.FireTavernBrawlInfoReceived();
	}

	// Token: 0x06000D6C RID: 3436 RVA: 0x0003604C File Offset: 0x0003424C
	private void FireTavernBrawlInfoReceived()
	{
		if (TavernBrawlDisplay.Get() != null)
		{
			if (this.m_currentMission == null)
			{
				TavernBrawlDisplay.Get().RefreshDataBasedUI(0f);
			}
			else
			{
				this.EnsureScenarioDataReady(null);
			}
		}
		if (Box.Get() != null)
		{
			Box.Get().UpdateUI(false);
		}
		if (this.OnTavernBrawlUpdated != null)
		{
			this.OnTavernBrawlUpdated.Invoke();
		}
	}

	// Token: 0x06000D6D RID: 3437 RVA: 0x000360C0 File Offset: 0x000342C0
	private void ScheduledEndOfCurrentTBCallback(object userData)
	{
		Log.EventTiming.Print("ScheduledEndOfCurrentTBCallback: ending current TB now.", new object[0]);
		this.m_currentMission = null;
		if (GameMgr.Get().IsFindingGame())
		{
			GameMgr.Get().CancelFindGame();
		}
		this.FireTavernBrawlInfoReceived();
	}

	// Token: 0x06000D6E RID: 3438 RVA: 0x00036109 File Offset: 0x00034309
	private void ScheduledRefreshTBSpecCallback(object userData)
	{
		Log.EventTiming.Print("ScheduledRefreshTBSpecCallback: refreshing now.", new object[0]);
		this.RefreshServerData();
	}

	// Token: 0x06000D6F RID: 3439 RVA: 0x00036128 File Offset: 0x00034328
	private void OnTavernBrawlScenarioLoaded(AssetKey requestedKey, DatabaseResult code, byte[] assetBytes)
	{
		if (requestedKey == null || requestedKey.Type != 1)
		{
			Log.Henry.Print("OnTavernBrawlScenarioLoaded bad AssetType assetId={0} assetType={1} {2}", new object[]
			{
				(requestedKey != null) ? requestedKey.AssetId : 0,
				(requestedKey != null) ? requestedKey.Type : 0,
				(requestedKey != null) ? requestedKey.Type.ToString() : "(null)"
			});
			return;
		}
		if (assetBytes == null || assetBytes.Length == 0)
		{
			return;
		}
		if (this.m_currentMission == null)
		{
			return;
		}
		ScenarioDbRecord scenarioDbRecord = ProtobufUtil.ParseFrom<ScenarioDbRecord>(assetBytes, 0, assetBytes.Length);
		if (this.m_currentMission.missionId != scenarioDbRecord.Id)
		{
			return;
		}
		this.m_scenarioAssetPendingLoad = false;
		this.m_currentMission.canSelectHeroForDeck = false;
		this.m_currentMission.canCreateDeck = false;
		this.m_currentMission.canEditDeck = false;
		foreach (GameSetupRule gameSetupRule in scenarioDbRecord.Rules)
		{
			if (gameSetupRule.RuleType == 1)
			{
				this.m_currentMission.canSelectHeroForDeck = true;
				this.m_currentMission.canCreateDeck = false;
				this.m_currentMission.canEditDeck = false;
			}
			else if (gameSetupRule.RuleType == 2)
			{
				this.m_currentMission.canSelectHeroForDeck = true;
				this.m_currentMission.canCreateDeck = true;
				this.m_currentMission.canEditDeck = true;
			}
		}
		this.m_currentMission.deckRuleset = DeckRuleset.GetDeckRuleset(scenarioDbRecord.DeckRulesetId);
		ApplicationMgr.Get().StartCoroutine(this.OnTavernBrawlScenarioLoaded_EnsureDeckContentsLoaded());
	}

	// Token: 0x06000D70 RID: 3440 RVA: 0x000362F0 File Offset: 0x000344F0
	private IEnumerator OnTavernBrawlScenarioLoaded_EnsureDeckContentsLoaded()
	{
		if (this.m_currentMission.canCreateDeck && !this.HasValidDeck())
		{
			float timeAtStart = Time.realtimeSinceStartup;
			bool done = false;
			while (!done)
			{
				yield return null;
				float secondsWaited = Time.realtimeSinceStartup - timeAtStart;
				if (secondsWaited > 30f)
				{
					done = true;
				}
				else if (!this.IsScenarioDataReady)
				{
					done = true;
				}
				else if (!this.m_currentMission.canCreateDeck)
				{
					done = true;
				}
				else
				{
					CollectionDeck deck = this.CurrentDeck();
					if (deck == null || deck.NetworkContentsLoaded())
					{
						done = true;
					}
				}
			}
		}
		if (!this.IsScenarioDataReady)
		{
			yield break;
		}
		if (TavernBrawlDisplay.Get() != null)
		{
			TavernBrawlDisplay.Get().RefreshDataBasedUI(0f);
		}
		if (this.m_serverDataReadyCallbacks != null)
		{
			TavernBrawlManager.CallbackEnsureServerDataReady[] callbacks = this.m_serverDataReadyCallbacks.ToArray();
			this.m_serverDataReadyCallbacks.Clear();
			foreach (TavernBrawlManager.CallbackEnsureServerDataReady cb in callbacks)
			{
				cb();
			}
		}
		yield break;
	}

	// Token: 0x06000D71 RID: 3441 RVA: 0x0003630C File Offset: 0x0003450C
	private static string GetCachedAssetFolder(AssetType assetType)
	{
		string text;
		switch (assetType)
		{
		case 1:
			text = "Scenario";
			break;
		case 2:
			text = "Subset";
			break;
		case 3:
			text = "DeckRuleset";
			break;
		default:
			text = "Other";
			break;
		}
		string cachePath = FileUtils.CachePath;
		return string.Format("{0}/{1}", cachePath, text);
	}

	// Token: 0x06000D72 RID: 3442 RVA: 0x00036374 File Offset: 0x00034574
	private static string GetCachedAssetFileExtension(AssetType assetType)
	{
		switch (assetType)
		{
		case 1:
			return "scen";
		case 2:
			return "subset_card";
		case 3:
			return "deck_ruleset";
		default:
			return assetType.ToString().Replace("ASSET_TYPE_", string.Empty).ToLower();
		}
	}

	// Token: 0x06000D73 RID: 3443 RVA: 0x000363D0 File Offset: 0x000345D0
	private static string GetCachedAssetFilePath(AssetType assetType, int assetId, byte[] assetHash)
	{
		string cachedAssetFolder = TavernBrawlManager.GetCachedAssetFolder(assetType);
		string cachedAssetFileExtension = TavernBrawlManager.GetCachedAssetFileExtension(assetType);
		return string.Format("{0}/{1}_{2}.{3}", new object[]
		{
			cachedAssetFolder,
			assetId,
			assetHash.ToHexString(),
			cachedAssetFileExtension
		});
	}

	// Token: 0x17000200 RID: 512
	// (get) Token: 0x06000D74 RID: 3444 RVA: 0x00036417 File Offset: 0x00034617
	private static int NextCallbackToken
	{
		get
		{
			return ++TavernBrawlManager.s_nextCallbackToken;
		}
	}

	// Token: 0x06000D75 RID: 3445 RVA: 0x00036428 File Offset: 0x00034628
	private static bool LoadCachedAssets(bool canRequestFromServer, TavernBrawlManager.LoadCachedAssetCallback cb, params AssetRecordInfo[] assets)
	{
		if (assets.Length == 0)
		{
			return false;
		}
		List<AssetKey> list = new List<AssetKey>();
		byte[] array = null;
		foreach (AssetRecordInfo assetRecordInfo in assets)
		{
			if (assetRecordInfo != null)
			{
				if (assetRecordInfo.RecordHash == null)
				{
					if (assetRecordInfo.RecordByteSize == 0U)
					{
						list.Add(assetRecordInfo.Asset);
					}
				}
				else
				{
					bool flag = false;
					string cachedAssetFilePath = TavernBrawlManager.GetCachedAssetFilePath(assetRecordInfo.Asset.Type, assetRecordInfo.Asset.AssetId, assetRecordInfo.RecordHash);
					if (assetRecordInfo.RecordByteSize != 0U && !File.Exists(cachedAssetFilePath))
					{
						flag = true;
						try
						{
							string cachedAssetFolder = TavernBrawlManager.GetCachedAssetFolder(assetRecordInfo.Asset.Type);
							Directory.CreateDirectory(cachedAssetFolder);
						}
						catch (Exception ex)
						{
							Error.AddDevFatal("Error creating cached asset folder {0}:\n{1}", new object[]
							{
								cachedAssetFilePath,
								ex.ToString()
							});
							return false;
						}
					}
					else
					{
						try
						{
							FileInfo fileInfo = new FileInfo(cachedAssetFilePath);
							if (fileInfo.Length != (long)((ulong)assetRecordInfo.RecordByteSize))
							{
								flag = true;
							}
							else if (assetRecordInfo.RecordByteSize != 0U)
							{
								byte[] array2 = File.ReadAllBytes(cachedAssetFilePath);
								SHA1 sha = SHA1.Create();
								byte[] arr = sha.ComputeHash(array2, 0, array2.Length);
								if (GeneralUtils.AreArraysEqual<byte>(arr, assetRecordInfo.RecordHash))
								{
									Log.Henry.ConsolePrint("LoadCachedAsset: locally available=true {0} id={1} hash={2}", new object[]
									{
										assetRecordInfo.Asset.Type,
										assetRecordInfo.Asset.AssetId,
										(assetRecordInfo.RecordHash != null) ? assetRecordInfo.RecordHash.ToHexString() : "<null>"
									});
									if (array == null)
									{
										array = array2;
									}
									TavernBrawlManager.SetCachedAssetIntoDbfSystem(assetRecordInfo.Asset.Type, array2);
								}
								else
								{
									flag = true;
								}
							}
						}
						catch (Exception ex2)
						{
							Error.AddDevFatal("Error reading cached asset folder {0}:\n{1}", new object[]
							{
								cachedAssetFilePath,
								ex2.ToString()
							});
							list.Add(assetRecordInfo.Asset);
						}
					}
					if (flag)
					{
						list.Add(assetRecordInfo.Asset);
						if (canRequestFromServer)
						{
							Log.Henry.ConsolePrint("LoadCachedAsset: locally available=false, requesting from server {0} id={1} hash={2}", new object[]
							{
								assetRecordInfo.Asset.Type,
								assetRecordInfo.Asset.AssetId,
								(assetRecordInfo.RecordHash != null) ? assetRecordInfo.RecordHash.ToHexString() : "<null>"
							});
						}
						else
						{
							Log.Henry.ConsolePrint("LoadCachedAsset: locally available=false, not requesting from server yet - {0} id={1} hash={2}", new object[]
							{
								assetRecordInfo.Asset.Type,
								assetRecordInfo.Asset.AssetId,
								(assetRecordInfo.RecordHash != null) ? assetRecordInfo.RecordHash.ToHexString() : "<null>"
							});
						}
					}
				}
			}
		}
		AssetRecordInfo assetRecordInfo2 = assets[0];
		if (list.Count > 0)
		{
			if (canRequestFromServer)
			{
				int nextCallbackToken = TavernBrawlManager.NextCallbackToken;
				if (cb != null)
				{
					TavernBrawlManager.s_assetRequests[nextCallbackToken] = new KeyValuePair<AssetRecordInfo, TavernBrawlManager.LoadCachedAssetCallback>(assetRecordInfo2, cb);
				}
				ConnectAPI.SendAssetRequest(nextCallbackToken, list);
			}
		}
		else if (assetRecordInfo2 != null && cb != null)
		{
			if (array == null)
			{
				array = new byte[0];
			}
			cb(assetRecordInfo2.Asset, 1, array);
		}
		return list.Count == 0;
	}

	// Token: 0x06000D76 RID: 3446 RVA: 0x000367B4 File Offset: 0x000349B4
	private static void CachedReceivedAsset(AssetType assetType, int assetId, byte[] assetBytes, int assetBytesLength)
	{
		SHA1 sha = SHA1.Create();
		byte[] assetHash = sha.ComputeHash(assetBytes, 0, assetBytesLength);
		string cachedAssetFilePath = TavernBrawlManager.GetCachedAssetFilePath(assetType, assetId, assetHash);
		try
		{
			using (FileStream fileStream = new FileStream(cachedAssetFilePath, 2))
			{
				fileStream.Write(assetBytes, 0, assetBytesLength);
			}
		}
		catch (Exception ex)
		{
			Error.AddDevFatal("Error saving cached asset {0}:\n{1}", new object[]
			{
				cachedAssetFilePath,
				ex.ToString()
			});
		}
	}

	// Token: 0x06000D77 RID: 3447 RVA: 0x00036848 File Offset: 0x00034A48
	private static void SetCachedAssetIntoDbfSystem(AssetType assetType, byte[] assetBytes)
	{
		switch (assetType)
		{
		case 1:
		{
			ScenarioDbRecord scenarioDbRecord = ProtobufUtil.ParseFrom<ScenarioDbRecord>(assetBytes, 0, assetBytes.Length);
			ScenarioDbfRecord scenarioDbfRecord = DbfUtils.ConvertFromProtobuf(scenarioDbRecord);
			if (scenarioDbfRecord == null)
			{
				Log.MissingAssets.Print("DbfUtils.ConvertFromProtobuf(protoScenario) returned null:\n{0}", new object[]
				{
					(scenarioDbRecord != null) ? scenarioDbRecord.ToString() : "(null)"
				});
			}
			else
			{
				GameDbf.Scenario.ReplaceRecordByRecordId(scenarioDbfRecord);
			}
			break;
		}
		case 2:
		{
			SubsetCardListDbRecord subsetCardListDbRecord = ProtobufUtil.ParseFrom<SubsetCardListDbRecord>(assetBytes, 0, assetBytes.Length);
			SubsetDbfRecord dbf = GameDbf.Subset.GetRecord(subsetCardListDbRecord.SubsetId);
			if (dbf == null)
			{
				dbf = new SubsetDbfRecord();
				dbf.SetID(subsetCardListDbRecord.SubsetId);
				GameDbf.Subset.AddRecord(dbf);
			}
			GameDbf.SubsetCard.RemoveRecordsWhere((SubsetCardDbfRecord r) => r.SubsetId == dbf.ID);
			foreach (int cardId in subsetCardListDbRecord.CardIds)
			{
				SubsetCardDbfRecord subsetCardDbfRecord = new SubsetCardDbfRecord();
				subsetCardDbfRecord.SetSubsetId(dbf.ID);
				subsetCardDbfRecord.SetCardId(cardId);
				GameDbf.SubsetCard.AddRecord(subsetCardDbfRecord);
			}
			break;
		}
		case 3:
		{
			DeckRulesetDbRecord deckRulesetDbRecord = ProtobufUtil.ParseFrom<DeckRulesetDbRecord>(assetBytes, 0, assetBytes.Length);
			DeckRulesetDbfRecord deckRulesetDbfRecord = DbfUtils.ConvertFromProtobuf(deckRulesetDbRecord);
			if (deckRulesetDbfRecord == null)
			{
				Log.MissingAssets.Print("DbfUtils.ConvertFromProtobuf(proto) returned null:\n{0}", new object[]
				{
					(deckRulesetDbRecord != null) ? deckRulesetDbRecord.ToString() : "(null)"
				});
			}
			else
			{
				GameDbf.DeckRuleset.ReplaceRecordByRecordId(deckRulesetDbfRecord);
			}
			foreach (DeckRulesetRuleDbRecord proto in deckRulesetDbRecord.Rules)
			{
				List<int> list;
				DeckRulesetRuleDbfRecord dbfRule = DbfUtils.ConvertFromProtobuf(proto, out list);
				GameDbf.DeckRulesetRule.ReplaceRecordByRecordId(dbfRule);
				GameDbf.DeckRulesetRuleSubset.RemoveRecordsWhere((DeckRulesetRuleSubsetDbfRecord r) => r.DeckRulesetRuleId == dbfRule.ID);
				if (list != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						DeckRulesetRuleSubsetDbfRecord deckRulesetRuleSubsetDbfRecord = new DeckRulesetRuleSubsetDbfRecord();
						deckRulesetRuleSubsetDbfRecord.SetDeckRulesetRuleId(dbfRule.ID);
						deckRulesetRuleSubsetDbfRecord.SetSubsetId(list[i]);
						GameDbf.DeckRulesetRuleSubset.AddRecord(deckRulesetRuleSubsetDbfRecord);
					}
				}
			}
			break;
		}
		}
	}

	// Token: 0x06000D78 RID: 3448 RVA: 0x00036B04 File Offset: 0x00034D04
	private void Network_OnGetAssetResponse()
	{
		GetAssetResponse assetResponse = ConnectAPI.GetAssetResponse();
		if (assetResponse == null)
		{
			return;
		}
		DatabaseResult databaseResult = 1;
		Map<AssetKey, byte[]> map = new Map<AssetKey, byte[]>();
		for (int i = 0; i < assetResponse.Responses.Count; i++)
		{
			AssetResponse assetResponse2 = assetResponse.Responses[i];
			if (assetResponse2.ErrorCode != 1)
			{
				Debug.Log(string.Format("Network_OnGetAssetResponse: error={0}:{1} type={2}:{3} id={4}", new object[]
				{
					assetResponse2.ErrorCode,
					assetResponse2.ErrorCode.ToString(),
					assetResponse2.RequestedKey.Type,
					assetResponse2.RequestedKey.Type.ToString(),
					assetResponse2.RequestedKey.AssetId
				}));
				if (databaseResult == 1)
				{
					databaseResult = assetResponse2.ErrorCode;
				}
			}
			AssetKey requestedKey = assetResponse2.RequestedKey;
			byte[] array = null;
			if (assetResponse2.HasScenarioAsset)
			{
				array = ProtobufUtil.ToByteArray(assetResponse2.ScenarioAsset);
			}
			if (assetResponse2.HasSubsetCardListAsset)
			{
				array = ProtobufUtil.ToByteArray(assetResponse2.SubsetCardListAsset);
			}
			if (assetResponse2.HasDeckRulesetAsset)
			{
				array = ProtobufUtil.ToByteArray(assetResponse2.DeckRulesetAsset);
			}
			if (array != null)
			{
				map[requestedKey] = array;
				TavernBrawlManager.CachedReceivedAsset(requestedKey.Type, requestedKey.AssetId, array, array.Length);
				TavernBrawlManager.SetCachedAssetIntoDbfSystem(requestedKey.Type, array);
			}
		}
		ApplicationMgr.Get().CancelScheduledCallback(new ApplicationMgr.ScheduledCallback(TavernBrawlManager.PruneCachedAssetFiles), null);
		ApplicationMgr.Get().ScheduleCallback(5f, true, new ApplicationMgr.ScheduledCallback(TavernBrawlManager.PruneCachedAssetFiles), null);
		KeyValuePair<AssetRecordInfo, TavernBrawlManager.LoadCachedAssetCallback> keyValuePair;
		if (TavernBrawlManager.s_assetRequests.TryGetValue(assetResponse.ClientToken, out keyValuePair))
		{
			AssetRecordInfo key = keyValuePair.Key;
			TavernBrawlManager.LoadCachedAssetCallback value = keyValuePair.Value;
			TavernBrawlManager.s_assetRequests.Remove(assetResponse.ClientToken);
			byte[] assetBytes;
			if (!map.TryGetValue(key.Asset, out assetBytes))
			{
				if (TavernBrawlManager.LoadCachedAssets(false, value, new AssetRecordInfo[]
				{
					key
				}))
				{
					return;
				}
				assetBytes = new byte[0];
			}
			value(key.Asset, databaseResult, assetBytes);
		}
	}

	// Token: 0x06000D79 RID: 3449 RVA: 0x00036D30 File Offset: 0x00034F30
	private static void PruneCachedAssetFiles(object userData)
	{
		string cachePath = FileUtils.CachePath;
		string message = null;
		string text = null;
		try
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(cachePath);
			if (directoryInfo.Exists)
			{
				foreach (DirectoryInfo directoryInfo2 in directoryInfo.GetDirectories())
				{
					message = directoryInfo2.FullName;
					foreach (FileInfo fileInfo in directoryInfo2.GetFiles())
					{
						text = fileInfo.Name;
						TimeSpan timeSpan = DateTime.Now - fileInfo.LastWriteTime;
						if (fileInfo.LastWriteTime < DateTime.Now && timeSpan.TotalDays > 124.0)
						{
							fileInfo.Delete();
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Error.AddDevWarning("Error pruning dir={0} file={1}:\n{2}", message, new object[]
			{
				text,
				ex.ToString()
			});
		}
	}

	// Token: 0x17000201 RID: 513
	// (get) Token: 0x06000D7A RID: 3450 RVA: 0x00036E40 File Offset: 0x00035040
	public bool IsCheated
	{
		get
		{
			if (ApplicationMgr.IsPublic())
			{
				return false;
			}
			if (this.m_currentMission == null)
			{
				return this.ServerInfo != null && this.ServerInfo.CurrentTavernBrawl != null;
			}
			return this.ServerInfo == null || this.ServerInfo.CurrentTavernBrawl == null || this.m_currentMission.missionId != this.ServerInfo.CurrentTavernBrawl.ScenarioId;
		}
	}

	// Token: 0x06000D7B RID: 3451 RVA: 0x00036EC4 File Offset: 0x000350C4
	public void Cheat_SetScenario(int scenarioId)
	{
		if (ApplicationMgr.IsPublic())
		{
			return;
		}
		if (this.m_currentMission == null)
		{
			this.m_currentMission = new TavernBrawlMission();
		}
		this.m_currentMission.missionId = scenarioId;
		this.m_scenarioAssetPendingLoad = true;
		if (this.OnTavernBrawlUpdated != null)
		{
			this.OnTavernBrawlUpdated.Invoke();
		}
		AssetRecordInfo assetRecordInfo = new AssetRecordInfo();
		assetRecordInfo.Asset = new AssetKey();
		assetRecordInfo.Asset.Type = 1;
		assetRecordInfo.Asset.AssetId = scenarioId;
		assetRecordInfo.RecordByteSize = 0U;
		assetRecordInfo.RecordHash = null;
		TavernBrawlManager.LoadCachedAssets(true, new TavernBrawlManager.LoadCachedAssetCallback(this.OnTavernBrawlScenarioLoaded), new AssetRecordInfo[]
		{
			assetRecordInfo
		});
	}

	// Token: 0x06000D7C RID: 3452 RVA: 0x00036F70 File Offset: 0x00035170
	public void Cheat_ResetToServerData()
	{
		if (ApplicationMgr.IsPublic())
		{
			return;
		}
		this.NetCache_OnTavernBrawlInfo();
		if (this.m_currentMission != null)
		{
			AssetRecordInfo assetRecordInfo = new AssetRecordInfo();
			assetRecordInfo.Asset = new AssetKey();
			assetRecordInfo.Asset.Type = 1;
			assetRecordInfo.Asset.AssetId = this.m_currentMission.missionId;
			assetRecordInfo.RecordByteSize = 0U;
			assetRecordInfo.RecordHash = null;
			TavernBrawlManager.LoadCachedAssets(true, new TavernBrawlManager.LoadCachedAssetCallback(this.OnTavernBrawlScenarioLoaded), new AssetRecordInfo[]
			{
				assetRecordInfo
			});
		}
	}

	// Token: 0x06000D7D RID: 3453 RVA: 0x00036FF8 File Offset: 0x000351F8
	public void Cheat_ResetSeenStuff(int newValue)
	{
		if (ApplicationMgr.IsPublic())
		{
			return;
		}
		this.RegisterOptionsListeners(false);
		Options.Get().SetInt(Option.LATEST_SEEN_TAVERNBRAWL_SEASON_CHALKBOARD, newValue);
		Options.Get().SetInt(Option.LATEST_SEEN_TAVERNBRAWL_SEASON, newValue);
		Options.Get().SetInt(Option.TIMES_SEEN_TAVERNBRAWL_CRAZY_RULES_QUOTE, 0);
		this.CheckLatestSeenSeason(false);
		this.RegisterOptionsListeners(true);
	}

	// Token: 0x0400071D RID: 1821
	public const int MINIMUM_CLASS_LEVEL = 20;

	// Token: 0x0400071E RID: 1822
	private const float DEFAULT_REFRESH_SPEC_SLUSH_SECONDS_MIN = 0f;

	// Token: 0x0400071F RID: 1823
	private const float DEFAULT_REFRESH_SPEC_SLUSH_SECONDS_MAX = 120f;

	// Token: 0x04000720 RID: 1824
	private const int PRUNE_CACHED_ASSETS_MAX_AGE_DAYS = 124;

	// Token: 0x04000721 RID: 1825
	private static TavernBrawlManager s_instance;

	// Token: 0x04000722 RID: 1826
	private TavernBrawlMission m_currentMission;

	// Token: 0x04000723 RID: 1827
	private bool m_scenarioAssetPendingLoad;

	// Token: 0x04000724 RID: 1828
	private DateTime? m_scheduledRefreshTimeLocal;

	// Token: 0x04000725 RID: 1829
	private DateTime? m_nextSeasonStartDateLocal;

	// Token: 0x04000726 RID: 1830
	private List<TavernBrawlManager.CallbackEnsureServerDataReady> m_serverDataReadyCallbacks;

	// Token: 0x04000727 RID: 1831
	private bool m_hasGottenClientOptionsAtLeastOnce;

	// Token: 0x04000728 RID: 1832
	private bool s_isFirstTimeSeeingThisFeature;

	// Token: 0x04000729 RID: 1833
	private bool s_isFirstTimeSeeingCurrentSeason;

	// Token: 0x0400072A RID: 1834
	private static Map<int, KeyValuePair<AssetRecordInfo, TavernBrawlManager.LoadCachedAssetCallback>> s_assetRequests = new Map<int, KeyValuePair<AssetRecordInfo, TavernBrawlManager.LoadCachedAssetCallback>>();

	// Token: 0x0400072B RID: 1835
	private static int s_nextCallbackToken = -1;

	// Token: 0x020005CC RID: 1484
	// (Invoke) Token: 0x06004251 RID: 16977
	public delegate void CallbackEnsureServerDataReady();

	// Token: 0x020005D3 RID: 1491
	// (Invoke) Token: 0x06004268 RID: 17000
	public delegate void LoadCachedAssetCallback(AssetKey requestedKey, DatabaseResult code, byte[] assetBytes);
}
