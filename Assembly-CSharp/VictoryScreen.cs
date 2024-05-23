using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200088D RID: 2189
public class VictoryScreen : EndGameScreen
{
	// Token: 0x06005388 RID: 21384 RVA: 0x0018E8A8 File Offset: 0x0018CAA8
	protected override void Awake()
	{
		base.Awake();
		this.m_gamesWonIndicator.Hide();
		this.m_noGoldRewardText.gameObject.SetActive(false);
		if (base.ShouldMakeUtilRequests())
		{
			if (GameMgr.Get().IsTutorial() && !GameMgr.Get().IsSpectator())
			{
				NetCache.Get().RegisterTutorialEndGameScreen(new NetCache.NetCacheCallback(this.OnNetCacheReady));
			}
			else
			{
				NetCache.Get().RegisterScreenEndOfGame(new NetCache.NetCacheCallback(this.OnNetCacheReady));
			}
		}
	}

	// Token: 0x06005389 RID: 21385 RVA: 0x0018E934 File Offset: 0x0018CB34
	protected override void ShowStandardFlow()
	{
		base.ShowStandardFlow();
		if (!GameMgr.Get().IsTutorial() || GameMgr.Get().IsSpectator())
		{
			this.m_hitbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(base.ContinueButtonPress_PrevMode));
			return;
		}
		if (GameUtils.AreAllTutorialsComplete())
		{
			LoadingScreen.Get().SetFadeColor(Color.white);
			this.m_hitbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ContinueButtonPress_FirstTimeHub));
		}
		else
		{
			if (DemoMgr.Get().GetMode() == DemoMode.APPLE_STORE && GameUtils.GetNextTutorial() == 0)
			{
				base.StartCoroutine(DemoMgr.Get().CompleteAppleStoreDemo());
				return;
			}
			this.m_hitbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(base.ContinueButtonPress_TutorialProgress));
		}
	}

	// Token: 0x0600538A RID: 21386 RVA: 0x0018E9FB File Offset: 0x0018CBFB
	protected override void LoadGoldenHeroEvent()
	{
		AssetLoader.Get().LoadGameObject("Hero2GoldHero", new AssetLoader.GameObjectCallback(this.OnGoldenHeroEventLoaded), null, false);
	}

	// Token: 0x0600538B RID: 21387 RVA: 0x0018EA1C File Offset: 0x0018CC1C
	protected override bool ShowGoldenHeroEvent()
	{
		if (!this.JustEarnedGoldenHero())
		{
			return false;
		}
		if (this.m_goldenHeroEvent.gameObject.activeInHierarchy)
		{
			this.m_goldenHeroEvent.Hide();
			this.m_showGoldenHeroEvent = false;
			return false;
		}
		this.m_goldenHeroAchievement.AckCurrentProgressAndRewardNotices();
		this.m_animationReadyToSkip = false;
		this.m_goldenHeroEvent.RegisterAnimationDoneListener(new GoldenHeroEvent.AnimationDoneListener(this.NotifyOfGoldenHeroAnimComplete));
		this.m_twoScoop.StopAnimating();
		this.m_goldenHeroEvent.Show();
		this.m_twoScoop.m_heroActor.transform.parent = this.m_goldenHeroEvent.m_heroBone;
		this.m_twoScoop.m_heroActor.transform.localPosition = Vector3.zero;
		this.m_twoScoop.m_heroActor.transform.localScale = new Vector3(1.375f, 1.375f, 1.375f);
		return true;
	}

	// Token: 0x0600538C RID: 21388 RVA: 0x0018EB04 File Offset: 0x0018CD04
	protected override bool JustEarnedGoldenHero()
	{
		if (this.m_hasParsedCompletedQuests)
		{
			return this.m_showGoldenHeroEvent;
		}
		string goldenHeroCardID = this.GetGoldenHeroCardID();
		if (goldenHeroCardID != "none")
		{
			CardPortraitQuality quality = new CardPortraitQuality(3, TAG_PREMIUM.GOLDEN);
			DefLoader.Get().LoadCardDef(goldenHeroCardID, new DefLoader.LoadDefCallback<CardDef>(this.OnGoldenHeroCardDefLoaded), new object(), quality);
		}
		this.m_hasParsedCompletedQuests = true;
		this.m_showGoldenHeroEvent = (goldenHeroCardID != "none");
		return this.m_showGoldenHeroEvent;
	}

	// Token: 0x0600538D RID: 21389 RVA: 0x0018EB80 File Offset: 0x0018CD80
	protected void ContinueButtonPress_FirstTimeHub(UIEvent e)
	{
		if (!base.HasShownScoops())
		{
			return;
		}
		base.HideTwoScoop();
		if (base.ShowNextReward())
		{
			SoundManager.Get().LoadAndPlay("VO_INNKEEPER_TUT_COMPLETE_05");
			return;
		}
		if (base.ShowNextCompletedQuest())
		{
			return;
		}
		base.ContinueButtonPress_Common();
		this.m_hitbox.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ContinueButtonPress_FirstTimeHub));
		if (Network.ShouldBeConnectedToAurora())
		{
			base.BackToMode(SceneMgr.Mode.HUB);
		}
		else
		{
			NotificationManager.Get().CreateTutorialDialog("GLOBAL_MEDAL_REWARD_CONGRATULATIONS", "TUTORIAL_MOBILE_COMPLETE_CONGRATS", "GLOBAL_OKAY", new UIEvent.Handler(this.UserPressedStartButton), new Vector2(0.5f, 0f), true);
			this.m_hitbox.gameObject.SetActive(false);
			this.m_continueText.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600538E RID: 21390 RVA: 0x0018EC53 File Offset: 0x0018CE53
	protected void UserPressedStartButton(UIEvent e)
	{
		WebAuth.ClearLoginData();
		base.BackToMode(SceneMgr.Mode.RESET);
	}

	// Token: 0x0600538F RID: 21391 RVA: 0x0018EC64 File Offset: 0x0018CE64
	protected override void OnTwoScoopShown()
	{
		if (BnetBar.Get() != null)
		{
			BnetBar.Get().SuppressLoginTooltip(true);
		}
		if (base.ShouldMakeUtilRequests())
		{
			this.InitGoldRewardUI();
		}
		if (this.m_showNoGoldRewardText)
		{
			this.m_noGoldRewardText.gameObject.SetActive(true);
		}
		if (this.m_showWinProgress)
		{
			this.m_gamesWonIndicator.Show();
		}
	}

	// Token: 0x06005390 RID: 21392 RVA: 0x0018ECD0 File Offset: 0x0018CED0
	protected override void OnTwoScoopHidden()
	{
		if (this.m_showNoGoldRewardText)
		{
			this.m_noGoldRewardText.gameObject.SetActive(false);
		}
		if (this.m_showWinProgress)
		{
			this.m_gamesWonIndicator.Hide();
		}
	}

	// Token: 0x06005391 RID: 21393 RVA: 0x0018ED0F File Offset: 0x0018CF0F
	private void OnGoldenHeroEventLoaded(string name, GameObject go, object callbackData)
	{
		base.StartCoroutine(this.WaitUntilTwoScoopLoaded(name, go, callbackData));
	}

	// Token: 0x06005392 RID: 21394 RVA: 0x0018ED21 File Offset: 0x0018CF21
	public void NotifyOfGoldenHeroAnimComplete()
	{
		this.m_animationReadyToSkip = true;
		this.m_goldenHeroEvent.RemoveAnimationDoneListener(new GoldenHeroEvent.AnimationDoneListener(this.NotifyOfGoldenHeroAnimComplete));
	}

	// Token: 0x06005393 RID: 21395 RVA: 0x0018ED44 File Offset: 0x0018CF44
	private IEnumerator WaitUntilTwoScoopLoaded(string name, GameObject go, object callbackData)
	{
		while (this.m_twoScoop == null || !this.m_twoScoop.IsLoaded())
		{
			yield return null;
		}
		while (!this.m_goldenHeroCardDefReady)
		{
			yield return null;
		}
		go.SetActive(false);
		TransformUtil.AttachAndPreserveLocalTransform(go.transform, this.m_goldenHeroEventBone);
		this.m_goldenHeroEvent = go.GetComponent<GoldenHeroEvent>();
		Texture heroTexture = this.m_goldenHeroCardDef.GetPortraitTexture();
		this.m_goldenHeroEvent.SetHeroBurnAwayTexture(heroTexture);
		this.m_goldenHeroEvent.SetVictoryTwoScoop((VictoryTwoScoop)this.m_twoScoop);
		base.SetGoldenHeroEventReady(true);
		yield break;
	}

	// Token: 0x06005394 RID: 21396 RVA: 0x0018ED70 File Offset: 0x0018CF70
	private void InitGoldRewardUI()
	{
		NetCache.NetCacheRewardProgress netObject = NetCache.Get().GetNetObject<NetCache.NetCacheRewardProgress>();
		TAG_GOLD_REWARD_STATE tag = GameState.Get().GetFriendlySidePlayer().GetTag<TAG_GOLD_REWARD_STATE>(GAME_TAG.GOLD_REWARD_STATE);
		if (tag != TAG_GOLD_REWARD_STATE.ELIGIBLE)
		{
			Log.Rachelle.Print(string.Format("VictoryScreen.InitGoldRewardUI(): goldRewardState = {0}", tag), new object[0]);
			string text = string.Empty;
			switch (tag)
			{
			case TAG_GOLD_REWARD_STATE.ALREADY_CAPPED:
				text = GameStrings.Format("GLOBAL_GOLD_REWARD_ALREADY_CAPPED", new object[]
				{
					netObject.MaxGoldPerDay
				});
				break;
			case TAG_GOLD_REWARD_STATE.BAD_RATING:
				text = GameStrings.Get("GLOBAL_GOLD_REWARD_BAD_RATING");
				break;
			case TAG_GOLD_REWARD_STATE.SHORT_GAME:
				text = GameStrings.Get("GLOBAL_GOLD_REWARD_SHORT_GAME");
				break;
			case TAG_GOLD_REWARD_STATE.OVER_CAIS:
				text = GameStrings.Get("GLOBAL_GOLD_REWARD_OVER_CAIS");
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				this.m_showNoGoldRewardText = true;
				this.m_noGoldRewardText.Text = text;
			}
			return;
		}
		NetCache.NetCacheGamesPlayed netObject2 = NetCache.Get().GetNetObject<NetCache.NetCacheGamesPlayed>();
		Log.Rachelle.Print(string.Format("EndGameTwoScoop.UpdateData(): {0}/{1} wins towards {2} gold", netObject2.FreeRewardProgress, netObject.WinsPerGold, netObject.GoldPerReward), new object[0]);
		this.m_showWinProgress = true;
		this.m_gamesWonIndicator.Init(Reward.Type.GOLD, netObject.GoldPerReward, netObject.WinsPerGold, netObject2.FreeRewardProgress, GamesWonIndicator.InnKeeperTrigger.NONE);
	}

	// Token: 0x06005395 RID: 21397 RVA: 0x0018EEC8 File Offset: 0x0018D0C8
	private string GetGoldenHeroCardID()
	{
		int num = 0;
		foreach (Achievement achievement in this.m_completedQuests)
		{
			if (achievement.AchieveType == Achievement.AchType.UNLOCK_GOLDEN_HERO)
			{
				this.m_goldenHeroAchievement = achievement;
				foreach (RewardData rewardData in achievement.Rewards)
				{
					if (rewardData.RewardType == Reward.Type.CARD)
					{
						CardRewardData cardRewardData = rewardData as CardRewardData;
						CollectionManager.Get().AddCardReward(cardRewardData, false);
						this.m_completedQuests.RemoveAt(num);
						return cardRewardData.CardID;
					}
					num++;
				}
			}
		}
		return "none";
	}

	// Token: 0x06005396 RID: 21398 RVA: 0x0018EFC0 File Offset: 0x0018D1C0
	private void OnGoldenHeroCardDefLoaded(string cardId, CardDef def, object userData)
	{
		this.m_goldenHeroCardDef = def;
		this.m_goldenHeroCardDefReady = true;
	}

	// Token: 0x040039B3 RID: 14771
	private const string NO_GOLDEN_HERO = "none";

	// Token: 0x040039B4 RID: 14772
	public GamesWonIndicator m_gamesWonIndicator;

	// Token: 0x040039B5 RID: 14773
	public UberText m_noGoldRewardText;

	// Token: 0x040039B6 RID: 14774
	public Transform m_goldenHeroEventBone;

	// Token: 0x040039B7 RID: 14775
	private bool m_showWinProgress;

	// Token: 0x040039B8 RID: 14776
	private bool m_showNoGoldRewardText;

	// Token: 0x040039B9 RID: 14777
	private bool m_showGoldenHeroEvent;

	// Token: 0x040039BA RID: 14778
	private bool m_hasParsedCompletedQuests;

	// Token: 0x040039BB RID: 14779
	private bool m_goldenHeroCardDefReady;

	// Token: 0x040039BC RID: 14780
	private GoldenHeroEvent m_goldenHeroEvent;

	// Token: 0x040039BD RID: 14781
	private CardDef m_goldenHeroCardDef;

	// Token: 0x040039BE RID: 14782
	private Achievement m_goldenHeroAchievement;
}
