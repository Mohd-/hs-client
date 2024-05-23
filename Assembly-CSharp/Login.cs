using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using bgs;
using PegasusUtil;
using UnityEngine;

// Token: 0x0200015F RID: 351
public class Login : Scene
{
	// Token: 0x06001301 RID: 4865 RVA: 0x000548A2 File Offset: 0x00052AA2
	protected override void Awake()
	{
		Login.s_instance = this;
		base.Awake();
	}

	// Token: 0x06001302 RID: 4866 RVA: 0x000548B0 File Offset: 0x00052AB0
	private void Start()
	{
		Network network = Network.Get();
		network.RegisterNetHandler(330, new Network.NetHandler(this.OnAccountLicensesResponse), null);
		network.RegisterNetHandler(331, new Network.NetHandler(this.OnGameLicensesResponse), null);
		network.RegisterNetHandler(296, new Network.NetHandler(this.OnSetProgressResponse), null);
		network.RegisterNetHandler(304, new Network.NetHandler(this.OnAssetsVersion), null);
		network.RegisterNetHandler(307, new Network.NetHandler(this.OnUpdateLoginComplete), null);
		SceneMgr.Get().NotifySceneLoaded();
		Network.Get().OnLoginStarted();
	}

	// Token: 0x06001303 RID: 4867 RVA: 0x0005496D File Offset: 0x00052B6D
	private void OnDestroy()
	{
		if (Login.s_instance == this)
		{
			Login.s_instance = null;
		}
	}

	// Token: 0x06001304 RID: 4868 RVA: 0x00054988 File Offset: 0x00052B88
	private void Update()
	{
		Network.Get().ProcessNetwork();
		if (!this.m_waitingForBattleNet)
		{
			return;
		}
		Network.BnetLoginState bnetLoginState = Network.BattleNetStatus();
		if ((bnetLoginState == Network.BnetLoginState.BATTLE_NET_LOGGED_IN && BattleNet.GetAccountCountry() != null && BattleNet.GetAccountRegion() != -1) || !Network.ShouldBeConnectedToAurora())
		{
			this.m_waitingForBattleNet = false;
			this.LoginOk();
		}
		else if (bnetLoginState == Network.BnetLoginState.BATTLE_NET_LOGIN_FAILED || bnetLoginState == Network.BnetLoginState.BATTLE_NET_TIMEOUT)
		{
			this.m_waitingForBattleNet = false;
			Network.Get().ShowConnectionFailureError("GLOBAL_ERROR_NETWORK_LOGIN_FAILURE");
		}
	}

	// Token: 0x06001305 RID: 4869 RVA: 0x00054A0D File Offset: 0x00052C0D
	public static Login Get()
	{
		return Login.s_instance;
	}

	// Token: 0x06001306 RID: 4870 RVA: 0x00054A14 File Offset: 0x00052C14
	public static bool IsLoginSceneActive()
	{
		return Login.s_instance != null;
	}

	// Token: 0x06001307 RID: 4871 RVA: 0x00054A21 File Offset: 0x00052C21
	public bool isWaitingForBattleNet()
	{
		return this.m_waitingForBattleNet;
	}

	// Token: 0x06001308 RID: 4872 RVA: 0x00054A2C File Offset: 0x00052C2C
	public override void Unload()
	{
		NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
		Network network = Network.Get();
		network.RemoveNetHandler(330, new Network.NetHandler(this.OnAccountLicensesResponse));
		network.RemoveNetHandler(331, new Network.NetHandler(this.OnGameLicensesResponse));
		network.RemoveNetHandler(296, new Network.NetHandler(this.OnSetProgressResponse));
		network.RemoveNetHandler(304, new Network.NetHandler(this.OnAssetsVersion));
		network.RemoveNetHandler(307, new Network.NetHandler(this.OnUpdateLoginComplete));
		GameMgr.Get().UnregisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
	}

	// Token: 0x06001309 RID: 4873 RVA: 0x00054B00 File Offset: 0x00052D00
	private void AssetsVersionCheckCompleted()
	{
		if (!string.IsNullOrEmpty(UpdateManager.Get().GetError()) && UpdateManager.Get().UpdateIsRequired())
		{
			Error.AddFatalLoc("GLUE_PATCHING_ERROR", new object[0]);
			return;
		}
		if (Network.ShouldBeConnectedToAurora())
		{
			BnetPresenceMgr.Get().Initialize();
			BnetFriendMgr.Get().Initialize();
			BnetWhisperMgr.Get().Initialize();
			BnetNearbyPlayerMgr.Get().Initialize();
			FriendChallengeMgr.Get().OnLoggedIn();
			SpectatorManager.Get().InitializeConnectedToBnet();
			if (!Options.Get().GetBool(Option.CONNECT_TO_AURORA))
			{
				Options.Get().SetBool(Option.CONNECT_TO_AURORA, true);
			}
			TutorialProgress @enum = Options.Get().GetEnum<TutorialProgress>(Option.LOCAL_TUTORIAL_PROGRESS);
			if (@enum > TutorialProgress.NOTHING_COMPLETE)
			{
				this.m_waitingForSetProgress = true;
				ConnectAPI.SetProgress((long)@enum);
			}
			if (WebAuth.GetIsNewCreatedAccount())
			{
				AdTrackingManager.Get().TrackAccountCreated();
				WebAuth.SetIsNewCreatedAccount(false);
			}
		}
		ConnectAPI.RequestAccountLicenses();
		ConnectAPI.RequestGameLicenses();
		DefLoader.Get().Initialize();
		CollectionManager.Init();
		Box.Get().OnLoggedIn();
		BaseUI.Get().OnLoggedIn();
		InactivePlayerKicker.Get().OnLoggedIn();
		HealthyGamingMgr.Get().OnLoggedIn();
		AdventureProgressMgr.InitRequests();
		Tournament.Init();
		GameMgr.Get().OnLoggedIn();
		if (Network.ShouldBeConnectedToAurora())
		{
			StoreManager.Get().Init();
		}
		Network.ResetConnectionFailureCount();
		if (Network.ShouldBeConnectedToAurora())
		{
			ConnectAPI.DoLoginUpdate();
		}
		else
		{
			this.m_waitingForUpdateLoginComplete = false;
		}
		PresenceMgr.Get().SetStatus(new Enum[]
		{
			PresenceStatus.LOGIN
		});
		if (SplashScreen.Get() != null)
		{
			SplashScreen.Get().StopPatching();
			SplashScreen.Get().ShowRatings();
		}
		this.PreloadActors();
		if (!Network.ShouldBeConnectedToAurora())
		{
			base.StartCoroutine(this.RegisterScreenWhenReady());
		}
		SceneMgr.Get().LoadShaderPreCompiler();
	}

	// Token: 0x0600130A RID: 4874 RVA: 0x00054CCF File Offset: 0x00052ECF
	private void LoginOk()
	{
		if (Network.ShouldBeConnectedToAurora())
		{
			Network.LoginOk();
			Network.RequestAssetsVersion();
		}
		else
		{
			this.AssetsVersionCheckCompleted();
		}
	}

	// Token: 0x0600130B RID: 4875 RVA: 0x00054CF0 File Offset: 0x00052EF0
	private void PreloadActors()
	{
	}

	// Token: 0x0600130C RID: 4876 RVA: 0x00054CF2 File Offset: 0x00052EF2
	private void OnNetCacheReady()
	{
		NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
		base.StartCoroutine(this.WaitForAchievesThenInit());
	}

	// Token: 0x0600130D RID: 4877 RVA: 0x00054D18 File Offset: 0x00052F18
	private IEnumerator WaitForAchievesThenInit()
	{
		while (AchieveManager.Get() == null)
		{
			yield return null;
		}
		while (!AchieveManager.Get().IsReady())
		{
			yield return null;
		}
		while (SplashScreen.Get() != null && !SplashScreen.Get().IsRatingsScreenFinished())
		{
			yield return null;
		}
		FixedRewardsMgr.Get().InitStartupFixedRewards();
		if (DemoMgr.Get().GetMode() == DemoMode.APPLE_STORE)
		{
			yield return new WaitForSeconds(2f);
		}
		SplashScreen.Get().Hide();
		Log.Downloader.Print("LOADING PROCESS COMPLETE at " + Time.realtimeSinceStartup, new object[0]);
		if (this.ShouldDoIntro() && !Options.Get().GetBool(Option.HAS_SEEN_CINEMATIC, false))
		{
			Cinematic cine = SceneMgr.Get().GetComponentInChildren<Cinematic>();
			cine.Play(new Cinematic.MovieCallback(this.OnCinematicFinished));
		}
		else
		{
			this.ReconnectOrChangeMode();
		}
		yield break;
	}

	// Token: 0x0600130E RID: 4878 RVA: 0x00054D33 File Offset: 0x00052F33
	private void OnCinematicFinished()
	{
		this.ReconnectOrChangeMode();
	}

	// Token: 0x0600130F RID: 4879 RVA: 0x00054D3C File Offset: 0x00052F3C
	private void ReconnectOrChangeMode()
	{
		if (ReconnectMgr.Get().ReconnectFromLogin())
		{
			ReconnectMgr.Get().AddTimeoutListener(new ReconnectMgr.TimeoutCallback(this.OnReconnectTimeout));
			return;
		}
		this.ChangeMode();
	}

	// Token: 0x06001310 RID: 4880 RVA: 0x00054D78 File Offset: 0x00052F78
	private void ChangeMode()
	{
		if (this.m_skipToTutorialProgress != TutorialProgress.NOTHING_COMPLETE)
		{
			this.m_nextMissionId = GameUtils.GetNextTutorial(this.m_skipToTutorialProgress);
		}
		else
		{
			this.m_nextMissionId = GameUtils.GetNextTutorial();
		}
		MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_MainTitle);
		if (GameUtils.ShouldShowSetRotationIntro() && this.m_nextMissionId == 0)
		{
			this.ChangeMode_SetRotation();
			return;
		}
		if (this.m_nextMissionId == 0)
		{
			this.ChangeMode_Hub();
		}
		else
		{
			this.ChangeMode_Tutorial();
		}
	}

	// Token: 0x06001311 RID: 4881 RVA: 0x00054DF6 File Offset: 0x00052FF6
	private bool OnReconnectTimeout(object userData)
	{
		this.ChangeMode();
		return true;
	}

	// Token: 0x06001312 RID: 4882 RVA: 0x00054E00 File Offset: 0x00053000
	private void ChangeMode_Hub()
	{
		if (Options.Get().GetBool(Option.HAS_SEEN_HUB, false))
		{
			SoundManager.Get().LoadAndPlay("VO_INNKEEPER_INTRO_01");
		}
		if (this.ShouldDoIntro())
		{
			Spell eventSpell = Box.Get().GetEventSpell(BoxEventType.STARTUP_HUB);
			eventSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnStartupHubSpellFinished));
			eventSpell.Activate();
		}
		else
		{
			this.DoSkippedBoxIntro();
			base.StartCoroutine(this.ShowUnAckedRewardsAndQuests());
		}
	}

	// Token: 0x06001313 RID: 4883 RVA: 0x00054E75 File Offset: 0x00053075
	private bool ShouldDoIntro()
	{
		return ApplicationMgr.IsPublic() || Options.Get().GetBool(Option.INTRO);
	}

	// Token: 0x06001314 RID: 4884 RVA: 0x00054E90 File Offset: 0x00053090
	private void DoSkippedBoxIntro()
	{
		Spell fadeFromBlackSpell = Box.Get().GetBoxCamera().GetEventTable().m_FadeFromBlackSpell;
		Spell.StateFinishedCallback callback = delegate(Spell thisSpell, SpellStateType prevStateType, object userData)
		{
			float timeScale = (float)userData;
			Time.timeScale = timeScale;
		};
		fadeFromBlackSpell.AddStateFinishedCallback(callback, Time.timeScale);
		Time.timeScale = SceneDebugger.Get().m_MaxTimeScale;
		fadeFromBlackSpell.Activate();
	}

	// Token: 0x06001315 RID: 4885 RVA: 0x00054EF6 File Offset: 0x000530F6
	private void OnStartupHubSpellFinished(Spell spell, object userData)
	{
		base.StartCoroutine(this.ShowUnAckedRewardsAndQuests());
	}

	// Token: 0x06001316 RID: 4886 RVA: 0x00054F08 File Offset: 0x00053108
	private IEnumerator ShowUnAckedRewardsAndQuests()
	{
		DialogManager.Get().ReadyForSeasonEndPopup(true);
		while (DialogManager.Get().WaitingToShowSeasonEndDialog())
		{
			yield return null;
		}
		this.HandleUnAckedRewardsAndCompletedQuests();
		yield break;
	}

	// Token: 0x06001317 RID: 4887 RVA: 0x00054F24 File Offset: 0x00053124
	private void HandleUnAckedRewardsAndCompletedQuests()
	{
		CollectionManager.Get().RegisterAchievesCompletedListener(new CollectionManager.DelOnAchievesCompleted(this.OnCollectionAchievesCompleted));
		NetCache.NetCacheProfileNotices netObject = NetCache.Get().GetNetObject<NetCache.NetCacheProfileNotices>();
		List<RewardData> list = new List<RewardData>();
		if (netObject != null)
		{
			List<RewardData> rewards = RewardUtils.GetRewards(netObject.Notices);
			HashSet<RewardVisualTiming> hashSet = new HashSet<RewardVisualTiming>();
			foreach (object obj in Enum.GetValues(typeof(RewardVisualTiming)))
			{
				RewardVisualTiming rewardVisualTiming = (RewardVisualTiming)((int)obj);
				hashSet.Add(rewardVisualTiming);
			}
			RewardUtils.GetViewableRewards(rewards, hashSet, ref list, ref this.m_completedQuests);
		}
		if (list.Count == 0)
		{
			this.ShowNextUnAckedRewardOrCompletedQuest();
			return;
		}
		this.m_numRewardsToLoad = list.Count;
		foreach (RewardData rewardData in list)
		{
			rewardData.LoadRewardObject(new Reward.DelOnRewardLoaded(this.OnRewardObjectLoaded));
		}
	}

	// Token: 0x06001318 RID: 4888 RVA: 0x0005505C File Offset: 0x0005325C
	private void OnRewardObjectLoaded(Reward reward, object callbackData)
	{
		reward.Hide(false);
		reward.transform.parent = base.transform;
		reward.transform.localRotation = Quaternion.identity;
		reward.transform.localPosition = Login.REWARD_LOCAL_POS;
		this.m_rewards.Add(reward);
		if (reward.RewardType == Reward.Type.CARD)
		{
			CardReward cardReward = reward as CardReward;
			cardReward.MakeActorsUnlit();
		}
		SceneUtils.SetLayer(reward.gameObject, GameLayer.Default);
		this.m_numRewardsToLoad--;
		if (this.m_numRewardsToLoad > 0)
		{
			return;
		}
		RewardUtils.SortRewards(ref this.m_rewards);
		this.ShowNextUnAckedRewardOrCompletedQuest();
	}

	// Token: 0x06001319 RID: 4889 RVA: 0x00055100 File Offset: 0x00053300
	private void ShowNextUnAckedRewardOrCompletedQuest()
	{
		if (BannerManager.Get().ShowABanner(new BannerManager.DelOnCloseBanner(this.ShowNextUnAckedRewardOrCompletedQuest)))
		{
			return;
		}
		if (this.ShowNextCompletedQuest())
		{
			return;
		}
		if (this.ShowNextUnAckedReward())
		{
			return;
		}
		HashSet<RewardVisualTiming> hashSet = new HashSet<RewardVisualTiming>();
		hashSet.Add(RewardVisualTiming.IMMEDIATE);
		HashSet<RewardVisualTiming> rewardVisualTimings = hashSet;
		FixedRewardsMgr.DelOnAllFixedRewardsShown delOnAllFixedRewardsShown = delegate(object userData)
		{
			CollectionManager.Get().RemoveAchievesCompletedListener(new CollectionManager.DelOnAchievesCompleted(this.OnCollectionAchievesCompleted));
			this.ShowWelcomeQuests();
		};
		if (!FixedRewardsMgr.Get().ShowFixedRewards(UserAttentionBlocker.NONE, rewardVisualTimings, delOnAllFixedRewardsShown, null, Login.REWARD_PUNCH_SCALE, Login.REWARD_SCALE))
		{
			delOnAllFixedRewardsShown(null);
		}
	}

	// Token: 0x0600131A RID: 4890 RVA: 0x0005518C File Offset: 0x0005338C
	private bool ShowNextCompletedQuest()
	{
		if (QuestToast.IsQuestActive())
		{
			QuestToast.GetCurrentToast().CloseQuestToast();
		}
		if (this.m_completedQuests.Count == 0)
		{
			return false;
		}
		Achievement quest = this.m_completedQuests[0];
		this.m_completedQuests.RemoveAt(0);
		QuestToast.ShowQuestToast(UserAttentionBlocker.NONE, delegate(object userData)
		{
			this.ShowNextUnAckedRewardOrCompletedQuest();
		}, false, quest);
		return true;
	}

	// Token: 0x0600131B RID: 4891 RVA: 0x000551F0 File Offset: 0x000533F0
	private bool ShowNextUnAckedReward()
	{
		if (this.m_rewards.Count == 0 || !UserAttentionManager.CanShowAttentionGrabber("Login.ShowNextUnAckedReward"))
		{
			return false;
		}
		Reward reward = this.m_rewards[0];
		this.m_rewards.RemoveAt(0);
		RewardUtils.ShowReward(UserAttentionBlocker.NONE, reward, false, Login.REWARD_PUNCH_SCALE, Login.REWARD_SCALE, "OnRewardShown", reward, base.gameObject);
		return true;
	}

	// Token: 0x0600131C RID: 4892 RVA: 0x00055260 File Offset: 0x00053460
	private void OnRewardShown(object callbackData)
	{
		Reward reward = callbackData as Reward;
		if (reward == null)
		{
			return;
		}
		reward.RegisterClickListener(new Reward.OnClickedCallback(this.OnRewardClicked));
		reward.EnableClickCatcher(true);
	}

	// Token: 0x0600131D RID: 4893 RVA: 0x0005529C File Offset: 0x0005349C
	private void OnRewardClicked(Reward reward, object userData)
	{
		reward.RemoveClickListener(new Reward.OnClickedCallback(this.OnRewardClicked));
		reward.Hide(true);
		this.ShowNextUnAckedRewardOrCompletedQuest();
	}

	// Token: 0x0600131E RID: 4894 RVA: 0x000552CC File Offset: 0x000534CC
	private void OnCollectionAchievesCompleted(List<Achievement> achievements)
	{
		Achievement achieve;
		foreach (Achievement achieve2 in achievements)
		{
			achieve = achieve2;
			Achievement achievement = this.m_completedQuests.Find((Achievement obj) => achieve.ID == obj.ID);
			if (achievement == null)
			{
				this.m_completedQuests.Add(achieve);
			}
		}
	}

	// Token: 0x0600131F RID: 4895 RVA: 0x0005535C File Offset: 0x0005355C
	private void ShowWelcomeQuests()
	{
		if (!DemoMgr.Get().ShouldShowWelcomeQuests())
		{
			SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
			return;
		}
		if (this.ShouldDoIntro())
		{
			WelcomeQuests.Show(UserAttentionBlocker.NONE, true, new WelcomeQuests.DelOnWelcomeQuestsClosed(this.OnWelcomeQuestsCallback), true);
		}
		else
		{
			this.OnWelcomeQuestsCallback();
		}
	}

	// Token: 0x06001320 RID: 4896 RVA: 0x000553AE File Offset: 0x000535AE
	private void OnWelcomeQuestsCallback()
	{
		this.ShowAlertDialogs();
		SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
	}

	// Token: 0x06001321 RID: 4897 RVA: 0x000553C1 File Offset: 0x000535C1
	private void ShowAlertDialogs()
	{
		Login.ShowNerfedCards(null);
		this.ShowGoldCapAlert();
	}

	// Token: 0x06001322 RID: 4898 RVA: 0x000553D0 File Offset: 0x000535D0
	public static bool ShowNerfedCards(DialogBase.HideCallback callbackOnHide = null)
	{
		if (!UserAttentionManager.CanShowAttentionGrabber(UserAttentionBlocker.SET_ROTATION_INTRO, "ShowNerfedCards"))
		{
			return false;
		}
		bool flag = false;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		List<CollectibleCard> allCards = CollectionManager.Get().GetAllCards();
		List<CollectibleCard> list = allCards.FindAll((CollectibleCard card) => card.PremiumType == TAG_PREMIUM.NORMAL && card.IsCardChanged());
		List<CollectibleCard> list2 = new List<CollectibleCard>();
		foreach (CollectibleCard collectibleCard in list)
		{
			if (ChangedCardMgr.Get().AllowCard(collectibleCard.ChangeVersion, collectibleCard.CardDbId))
			{
				list2.Add(collectibleCard);
				if (!flag && (CollectionManager.Get().IsCardInCollection(collectibleCard.CardId, TAG_PREMIUM.NORMAL) || CollectionManager.Get().IsCardInCollection(collectibleCard.CardId, TAG_PREMIUM.GOLDEN)))
				{
					flag = true;
				}
			}
		}
		Log.ChangedCards.Print("Time to find all nerfed cards: " + (Time.realtimeSinceStartup - realtimeSinceStartup), new object[0]);
		if (flag)
		{
			IOrderedEnumerable<CollectibleCard> orderedEnumerable = Enumerable.ThenBy<CollectibleCard, string>(Enumerable.OrderByDescending<CollectibleCard, TAG_RARITY>(list2, (CollectibleCard c) => c.Rarity), (CollectibleCard c) => c.Name);
			list2 = Enumerable.ToList<CollectibleCard>(orderedEnumerable);
			for (int i = 0; i < list2.Count; i++)
			{
				if (list2[i].CardId == "NEW1_019")
				{
					CollectibleCard collectibleCard2 = list2[0];
					list2[0] = list2[i];
					list2[i] = collectibleCard2;
				}
			}
			foreach (CollectibleCard collectibleCard3 in list2)
			{
				Log.ChangedCards.Print("Displaying nerfed card " + collectibleCard3.Name, new object[0]);
			}
			CardListPopup.Info info = new CardListPopup.Info();
			info.m_description = GameStrings.Get("GLUE_CARDS_UPDATED");
			info.m_cards = list2;
			info.m_callbackOnHide = callbackOnHide;
			DialogManager.Get().ShowCardListPopup(UserAttentionBlocker.SET_ROTATION_INTRO, info);
			return true;
		}
		return false;
	}

	// Token: 0x06001323 RID: 4899 RVA: 0x0005564C File Offset: 0x0005384C
	private void ShowGoldCapAlert()
	{
		if (!UserAttentionManager.CanShowAttentionGrabber("Login.ShowGoldCapAlert"))
		{
			return;
		}
		NetCache.NetCacheGoldBalance netObject = NetCache.Get().GetNetObject<NetCache.NetCacheGoldBalance>();
		long cap = netObject.Cap;
		if (netObject.GetTotal() < cap)
		{
			return;
		}
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_attentionCategory = UserAttentionBlocker.NONE;
		popupInfo.m_headerText = GameStrings.Format("GLUE_GOLD_CAP_HEADER", new object[]
		{
			cap.ToString()
		});
		popupInfo.m_text = GameStrings.Format("GLUE_GOLD_CAP_BODY", new object[]
		{
			cap.ToString()
		});
		popupInfo.m_showAlertIcon = true;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
		DialogManager.Get().ShowPopup(popupInfo);
	}

	// Token: 0x06001324 RID: 4900 RVA: 0x000556F0 File Offset: 0x000538F0
	private void ShowTextureCompressionWarning()
	{
		if (!ApplicationMgr.IsInternal())
		{
			return;
		}
		if (!AndroidDeviceSettings.IsCurrentTextureFormatSupported())
		{
			AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
			popupInfo.m_headerText = GameStrings.Get("GLUE_TEXTURE_COMPRESSION_WARNING_TITLE");
			popupInfo.m_text = GameStrings.Get("GLUE_TEXTURE_COMPRESSION_WARNING");
			popupInfo.m_showAlertIcon = true;
			popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
			popupInfo.m_iconSet = AlertPopup.PopupInfo.IconSet.None;
			popupInfo.m_cancelText = GameStrings.Get("GLOBAL_OKAY");
			popupInfo.m_confirmText = GameStrings.Get("GLOBAL_CANCEL");
			popupInfo.m_responseCallback = delegate(AlertPopup.Response response, object data)
			{
				if (response == AlertPopup.Response.CANCEL)
				{
					Application.OpenURL("http://www.hearthstone.com.cn/download");
				}
			};
			DialogManager.Get().ShowPopup(popupInfo);
		}
	}

	// Token: 0x06001325 RID: 4901 RVA: 0x000557A0 File Offset: 0x000539A0
	private void ShowGraphicsDeviceWarning()
	{
		bool @bool = Options.Get().GetBool(Option.SHOWN_GFX_DEVICE_WARNING, false);
		if (@bool)
		{
			return;
		}
		Options.Get().SetBool(Option.SHOWN_GFX_DEVICE_WARNING, true);
		string text = SystemInfo.graphicsDeviceName.ToLower();
		if (!text.Contains("powervr"))
		{
			return;
		}
		if (!text.Contains("540") && !text.Contains("544"))
		{
			return;
		}
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get("GLUE_UNRELIABLE_GPU_WARNING_TITLE");
		popupInfo.m_text = GameStrings.Get("GLUE_UNRELIABLE_GPU_WARNING");
		popupInfo.m_showAlertIcon = true;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
		popupInfo.m_iconSet = AlertPopup.PopupInfo.IconSet.None;
		popupInfo.m_cancelText = GameStrings.Get("GLOBAL_SUPPORT");
		popupInfo.m_confirmText = GameStrings.Get("GLOBAL_OKAY");
		popupInfo.m_responseCallback = delegate(AlertPopup.Response response, object data)
		{
			if (response == AlertPopup.Response.CANCEL)
			{
				Application.OpenURL(NydusLink.GetSupportLink("system-requirements", false));
			}
		};
		DialogManager.Get().ShowPopup(popupInfo);
	}

	// Token: 0x06001326 RID: 4902 RVA: 0x00055898 File Offset: 0x00053A98
	private void ChangeMode_Tutorial()
	{
		PresenceMgr.Get().SetStatus(new Enum[]
		{
			PresenceStatus.TUTORIAL_PREGAME
		});
		Box.Get().SetLightState(BoxLightStateType.TUTORIAL);
		Spell eventSpell = Box.Get().GetEventSpell(BoxEventType.STARTUP_TUTORIAL);
		eventSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnStartupTutorialSpellFinished));
		eventSpell.Activate();
	}

	// Token: 0x06001327 RID: 4903 RVA: 0x000558F0 File Offset: 0x00053AF0
	private void OnStartupTutorialSpellFinished(Spell spell, object userData)
	{
		Box.Get().AddButtonPressListener(new Box.ButtonPressCallback(this.OnStartButtonPressed));
		Box.Get().ChangeState(Box.State.PRESS_START);
	}

	// Token: 0x06001328 RID: 4904 RVA: 0x00055920 File Offset: 0x00053B20
	private void OnStartButtonPressed(Box.ButtonType buttonType, object userData)
	{
		if (buttonType != Box.ButtonType.START)
		{
			return;
		}
		if (this.m_nextMissionId == 3)
		{
			AdTrackingManager.Get().TrackTutorialProgress(TutorialProgress.NOTHING_COMPLETE.ToString());
		}
		Box.Get().RemoveButtonPressListener(new Box.ButtonPressCallback(this.OnStartButtonPressed));
		if (this.m_nextMissionId == 3)
		{
			if (DemoMgr.Get().GetMode() == DemoMode.APPLE_STORE || Network.ShouldBeConnectedToAurora())
			{
				this.StartTutorial();
			}
			else
			{
				Box.Get().m_StartButton.ChangeState(BoxStartButton.State.HIDDEN);
				DialogManager.Get().ShowExistingAccountPopup(new ExistingAccountPopup.ResponseCallback(this.OnExistingAccountPopupResponse), new DialogManager.DialogProcessCallback(this.OnExistingAccountLoadedCallback));
			}
		}
		else
		{
			this.ShowTutorialProgressScreen();
		}
	}

	// Token: 0x06001329 RID: 4905 RVA: 0x000559DC File Offset: 0x00053BDC
	private void ShowTutorialProgressScreen()
	{
		Box.Get().m_StartButton.ChangeState(BoxStartButton.State.HIDDEN);
		AssetLoader.Get().LoadActor("TutorialProgressScreen", new AssetLoader.GameObjectCallback(this.OnTutorialProgressScreenCallback), null, false);
	}

	// Token: 0x0600132A RID: 4906 RVA: 0x00055A18 File Offset: 0x00053C18
	private void OnTutorialProgressScreenCallback(string name, GameObject go, object callbackData)
	{
		TutorialProgressScreen component = go.GetComponent<TutorialProgressScreen>();
		component.SetCoinPressCallback(new HeroCoin.CoinPressCallback(this.StartTutorial));
		component.StartTutorialProgress();
	}

	// Token: 0x0600132B RID: 4907 RVA: 0x00055A44 File Offset: 0x00053C44
	private void OnExistingAccountPopupResponse(bool hasAccount)
	{
		this.m_existingAccountPopup.gameObject.SetActive(false);
		if (hasAccount)
		{
			ApplicationMgr.Get().ResetAndForceLogin();
		}
		else
		{
			this.StartTutorial();
		}
	}

	// Token: 0x0600132C RID: 4908 RVA: 0x00055A80 File Offset: 0x00053C80
	private void StartTutorial()
	{
		MusicManager.Get().StopPlaylist();
		Box.Get().ChangeState(Box.State.CLOSED);
		GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
		GameMgr.Get().FindGame(4, this.m_nextMissionId, 0L, 0L);
	}

	// Token: 0x0600132D RID: 4909 RVA: 0x00055ACF File Offset: 0x00053CCF
	private bool OnExistingAccountLoadedCallback(DialogBase dialog, object userData)
	{
		this.m_existingAccountPopup = (ExistingAccountPopup)dialog;
		this.m_existingAccountPopup.gameObject.SetActive(true);
		return true;
	}

	// Token: 0x0600132E RID: 4910 RVA: 0x00055AF0 File Offset: 0x00053CF0
	private void ChangeMode_SetRotation()
	{
		UserAttentionManager.StartBlocking(UserAttentionBlocker.SET_ROTATION_INTRO);
		Spell eventSpell = Box.Get().GetEventSpell(BoxEventType.STARTUP_SET_ROTATION);
		Box.Get().m_StoreButton.gameObject.SetActive(false);
		Box.Get().m_QuestLogButton.gameObject.SetActive(false);
		eventSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnSetRotationSpellFinished));
		eventSpell.Activate();
	}

	// Token: 0x0600132F RID: 4911 RVA: 0x00055B52 File Offset: 0x00053D52
	private void OnSetRotationSpellFinished(Spell spell, object userData)
	{
		SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
	}

	// Token: 0x06001330 RID: 4912 RVA: 0x00055B60 File Offset: 0x00053D60
	private bool OnFindGameEvent(FindGameEventData eventData, object userData)
	{
		FindGameState state = eventData.m_state;
		if (state == FindGameState.SERVER_GAME_STARTED)
		{
			if (!GameMgr.Get().IsNextReconnect())
			{
				Spell eventSpell = Box.Get().GetEventSpell(BoxEventType.TUTORIAL_PLAY);
				eventSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnTutorialPlaySpellStateFinished));
				eventSpell.ActivateState(SpellStateType.BIRTH);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001331 RID: 4913 RVA: 0x00055BBD File Offset: 0x00053DBD
	private void OnUpdateLoginComplete()
	{
		ConnectAPI.GetUpdateLoginComplete();
		this.m_waitingForUpdateLoginComplete = false;
	}

	// Token: 0x06001332 RID: 4914 RVA: 0x00055BCC File Offset: 0x00053DCC
	private void OnAssetsVersion()
	{
		int num = (!ApplicationMgr.IsPublic()) ? Vars.Key("Application.AssetsVersion").GetInt(Network.GetAssetsVersion()) : Network.GetAssetsVersion();
		Log.UpdateManager.Print("UpdataManager: assetsVersion = {0}, gameVersion = {1}", new object[]
		{
			num,
			12574
		});
		if (num != 0 && num > 12574)
		{
			if (SplashScreen.Get() != null)
			{
				SplashScreen.Get().StartPatching();
			}
			UpdateManager.Get().StartInitialize(num, new UpdateManager.InitCallback(this.AssetsVersionCheckCompleted));
		}
		else
		{
			this.AssetsVersionCheckCompleted();
		}
	}

	// Token: 0x06001333 RID: 4915 RVA: 0x00055C7C File Offset: 0x00053E7C
	private void OnGameLicensesResponse()
	{
		CheckGameLicensesResponse checkGameLicensesResponse = ConnectAPI.GetCheckGameLicensesResponse();
		if (checkGameLicensesResponse != null)
		{
		}
	}

	// Token: 0x06001334 RID: 4916 RVA: 0x00055C98 File Offset: 0x00053E98
	private void OnAccountLicensesResponse()
	{
		CheckAccountLicensesResponse checkAccountLicensesResponse = ConnectAPI.GetCheckAccountLicensesResponse();
		if (ApplicationMgr.IsPublic() && !checkAccountLicensesResponse.Success)
		{
			AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
			popupInfo.m_headerText = GameStrings.Get("GLOBAL_ERROR_GENERIC_HEADER");
			popupInfo.m_text = GameStrings.Get("GLOBAL_ERROR_ACCOUNT_LICENSES");
			popupInfo.m_showAlertIcon = false;
			popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
			DialogManager.Get().ShowPopup(popupInfo);
		}
		base.StartCoroutine(this.RegisterScreenWhenReady());
	}

	// Token: 0x06001335 RID: 4917 RVA: 0x00055D0C File Offset: 0x00053F0C
	private void OnSetProgressResponse()
	{
		SetProgressResponse setProgressResponse = ConnectAPI.GetSetProgressResponse();
		switch (setProgressResponse.Result_)
		{
		case 1:
		case 3:
			if (setProgressResponse.HasProgress)
			{
				this.m_skipToTutorialProgress = (TutorialProgress)setProgressResponse.Progress;
			}
			Options.Get().DeleteOption(Option.LOCAL_TUTORIAL_PROGRESS);
			goto IL_6D;
		}
		Debug.LogWarning(string.Format("Login.OnSetProgressResponse(): received unexpected result {0}", setProgressResponse.Result_));
		IL_6D:
		this.m_waitingForSetProgress = false;
	}

	// Token: 0x06001336 RID: 4918 RVA: 0x00055D90 File Offset: 0x00053F90
	private IEnumerator RegisterScreenWhenReady()
	{
		while (!DefLoader.Get().HasLoadedEntityDefs())
		{
			yield return null;
		}
		while (this.m_waitingForSetProgress)
		{
			yield return null;
		}
		while (this.m_waitingForUpdateLoginComplete)
		{
			yield return null;
		}
		ConnectAPI.OnStartupPacketSequenceComplete();
		NetCache.Get().RegisterScreenLogin(new NetCache.NetCacheCallback(this.OnNetCacheReady));
		yield break;
	}

	// Token: 0x06001337 RID: 4919 RVA: 0x00055DAC File Offset: 0x00053FAC
	private void OnTutorialPlaySpellStateFinished(Spell spell, SpellStateType prevStateType, object userData)
	{
		SpellStateType activeState = spell.GetActiveState();
		if (prevStateType == SpellStateType.BIRTH)
		{
			LoadingScreen.Get().SetFadeColor(Color.white);
			LoadingScreen.Get().EnableFadeOut(false);
			LoadingScreen.Get().AddTransitionObject(Box.Get().gameObject);
			LoadingScreen.Get().AddTransitionBlocker();
			SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnMissionSceneLoaded));
			SceneMgr.Get().SetNextMode(SceneMgr.Mode.GAMEPLAY);
			return;
		}
		if (activeState == SpellStateType.NONE)
		{
			LoadingScreen.Get().NotifyTransitionBlockerComplete();
			return;
		}
	}

	// Token: 0x06001338 RID: 4920 RVA: 0x00055E34 File Offset: 0x00054034
	private void OnMissionSceneLoaded(SceneMgr.Mode mode, Scene scene, object userData)
	{
		SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnMissionSceneLoaded));
		Spell eventSpell = Box.Get().GetEventSpell(BoxEventType.TUTORIAL_PLAY);
		eventSpell.ActivateState(SpellStateType.ACTION);
	}

	// Token: 0x06001339 RID: 4921 RVA: 0x00055E6C File Offset: 0x0005406C
	private void ChangeMode_Resume(SceneMgr.Mode mode)
	{
		if (mode == SceneMgr.Mode.HUB && Options.Get().GetBool(Option.HAS_SEEN_HUB, false))
		{
			SoundManager.Get().LoadAndPlay("VO_INNKEEPER_INTRO_01");
		}
		if (mode == SceneMgr.Mode.COLLECTIONMANAGER)
		{
			CollectionManager.Get().NotifyOfBoxTransitionStart();
		}
		SceneMgr.Get().SetNextMode(mode);
		Box.Get().m_Camera.m_EventTable.m_FadeFromBlackSpell.Activate();
	}

	// Token: 0x040009DF RID: 2527
	public static readonly Vector3 REWARD_LOCAL_POS = new Vector3(0.1438589f, 31.27692f, 12.97332f);

	// Token: 0x040009E0 RID: 2528
	public static PlatformDependentValue<Vector3> REWARD_SCALE = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
	{
		PC = new Vector3(15f, 15f, 15f),
		Phone = new Vector3(8f, 8f, 8f)
	};

	// Token: 0x040009E1 RID: 2529
	public static PlatformDependentValue<Vector3> REWARD_PUNCH_SCALE = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
	{
		PC = new Vector3(15.1f, 15.1f, 15.1f),
		Phone = new Vector3(8.1f, 8.1f, 8.1f)
	};

	// Token: 0x040009E2 RID: 2530
	private int m_nextMissionId;

	// Token: 0x040009E3 RID: 2531
	private TutorialProgress m_skipToTutorialProgress;

	// Token: 0x040009E4 RID: 2532
	private bool m_waitingForBattleNet = true;

	// Token: 0x040009E5 RID: 2533
	private bool m_waitingForUpdateLoginComplete = true;

	// Token: 0x040009E6 RID: 2534
	private bool m_waitingForSetProgress;

	// Token: 0x040009E7 RID: 2535
	private List<Reward> m_rewards = new List<Reward>();

	// Token: 0x040009E8 RID: 2536
	private List<Achievement> m_completedQuests = new List<Achievement>();

	// Token: 0x040009E9 RID: 2537
	private int m_numRewardsToLoad;

	// Token: 0x040009EA RID: 2538
	private ExistingAccountPopup m_existingAccountPopup;

	// Token: 0x040009EB RID: 2539
	private static Login s_instance = null;
}
