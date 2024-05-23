using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004FD RID: 1277
public class EndGameScreen : MonoBehaviour
{
	// Token: 0x06003B97 RID: 15255 RVA: 0x001210B8 File Offset: 0x0011F2B8
	protected virtual void Awake()
	{
		EndGameScreen.s_instance = this;
		CollectionManager.Get().RegisterAchievesCompletedListener(new CollectionManager.DelOnAchievesCompleted(this.OnCollectionAchievesCompleted));
		AchieveManager.Get().TriggerLaunchDayEvent();
		AchieveManager.Get().UpdateActiveAchieves(new AchieveManager.ActiveAchievesUpdatedCallback(this.OnAchievesUpdated));
		this.m_hitbox.gameObject.SetActive(false);
		string key = "GLOBAL_CLICK_TO_CONTINUE";
		if (UniversalInputManager.Get().IsTouchMode())
		{
			key = "GLOBAL_CLICK_TO_CONTINUE_TOUCH";
		}
		this.m_continueText.Text = GameStrings.Get(key);
		this.m_continueText.gameObject.SetActive(false);
		PegUI.Get().SetInputCamera(CameraUtils.FindFirstByLayer(GameLayer.IgnoreFullScreenEffects));
		SceneUtils.SetLayer(this.m_hitbox.gameObject, GameLayer.IgnoreFullScreenEffects);
		SceneUtils.SetLayer(this.m_continueText.gameObject, GameLayer.IgnoreFullScreenEffects);
		if (!Network.ShouldBeConnectedToAurora())
		{
			this.UpdateRewards();
		}
	}

	// Token: 0x06003B98 RID: 15256 RVA: 0x00121194 File Offset: 0x0011F394
	private void OnDestroy()
	{
		if (AchieveManager.Get() != null)
		{
			AchieveManager.Get().RemoveActiveAchievesUpdatedListener(new AchieveManager.ActiveAchievesUpdatedCallback(this.OnAchievesUpdated));
		}
		if (EndGameScreen.OnTwoScoopsShown != null)
		{
			EndGameScreen.OnTwoScoopsShown(false, this.m_twoScoop);
		}
		EndGameScreen.s_instance = null;
	}

	// Token: 0x06003B99 RID: 15257 RVA: 0x001211E3 File Offset: 0x0011F3E3
	public static EndGameScreen Get()
	{
		return EndGameScreen.s_instance;
	}

	// Token: 0x06003B9A RID: 15258 RVA: 0x001211EC File Offset: 0x0011F3EC
	public virtual void Show()
	{
		this.m_shown = true;
		Network.DisconnectFromGameServer();
		InputManager.Get().DisableInput();
		this.m_hitbox.gameObject.SetActive(true);
		FullScreenFXMgr.Get().SetBlurDesaturation(0.5f);
		FullScreenFXMgr.Get().Blur(1f, 0.5f, iTween.EaseType.easeInCirc, null);
		if (GameState.Get() != null && GameState.Get().GetFriendlySidePlayer() != null)
		{
			GameState.Get().GetFriendlySidePlayer().GetHandZone().UpdateLayout(-1);
		}
		this.InitIfReady();
	}

	// Token: 0x06003B9B RID: 15259 RVA: 0x0012127B File Offset: 0x0011F47B
	public void NotifyOfAnimComplete()
	{
		this.m_animationReadyToSkip = true;
	}

	// Token: 0x06003B9C RID: 15260 RVA: 0x00121284 File Offset: 0x0011F484
	public void NotifyOfRewardAnimComplete()
	{
		this.m_animationReadyToSkip = true;
	}

	// Token: 0x06003B9D RID: 15261 RVA: 0x0012128D File Offset: 0x0011F48D
	private void ShowTutorialProgress()
	{
		this.HideTwoScoop();
		base.StartCoroutine(this.LoadTutorialProgress());
	}

	// Token: 0x06003B9E RID: 15262 RVA: 0x001212A4 File Offset: 0x0011F4A4
	private IEnumerator LoadTutorialProgress()
	{
		yield return new WaitForSeconds(0.25f);
		AssetLoader.Get().LoadActor("TutorialProgressScreen", new AssetLoader.GameObjectCallback(this.OnTutorialProgressScreenCallback), null, false);
		yield break;
	}

	// Token: 0x06003B9F RID: 15263 RVA: 0x001212C0 File Offset: 0x0011F4C0
	private void OnTutorialProgressScreenCallback(string name, GameObject go, object callbackData)
	{
		go.transform.parent = base.transform;
		go.GetComponent<TutorialProgressScreen>().StartTutorialProgress();
	}

	// Token: 0x06003BA0 RID: 15264 RVA: 0x001212E9 File Offset: 0x0011F4E9
	protected void ContinueButtonPress_Common()
	{
		LoadingScreen.Get().AddTransitionObject(this);
	}

	// Token: 0x06003BA1 RID: 15265 RVA: 0x001212F6 File Offset: 0x0011F4F6
	protected void ContinueButtonPress_PrevMode(UIEvent e)
	{
		this.ContinueEvents();
	}

	// Token: 0x06003BA2 RID: 15266 RVA: 0x00121300 File Offset: 0x0011F500
	public bool ContinueEvents()
	{
		if (this.ContinueDefaultEvents())
		{
			return true;
		}
		PlayMakerFSM component = this.m_twoScoop.GetComponent<PlayMakerFSM>();
		if (component != null)
		{
			component.SendEvent("Death");
		}
		this.ContinueButtonPress_Common();
		this.m_hitbox.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ContinueButtonPress_PrevMode));
		this.ReturnToPreviousMode();
		return false;
	}

	// Token: 0x06003BA3 RID: 15267 RVA: 0x00121363 File Offset: 0x0011F563
	protected void ContinueButtonPress_TutorialProgress(UIEvent e)
	{
		this.ContinueTutorialEvents();
	}

	// Token: 0x06003BA4 RID: 15268 RVA: 0x0012136C File Offset: 0x0011F56C
	public void ContinueTutorialEvents()
	{
		if (this.ContinueDefaultEvents())
		{
			return;
		}
		this.ContinueButtonPress_Common();
		this.m_hitbox.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ContinueButtonPress_TutorialProgress));
		this.m_continueText.gameObject.SetActive(false);
		this.ShowTutorialProgress();
	}

	// Token: 0x06003BA5 RID: 15269 RVA: 0x001213BC File Offset: 0x0011F5BC
	private bool ContinueDefaultEvents()
	{
		if (!this.m_haveShownTwoScoop)
		{
			return true;
		}
		if (!this.m_animationReadyToSkip)
		{
			return true;
		}
		this.HideTwoScoop();
		return (this.ShowGoldenHeroEvent() && this.m_goldenHeroEventReady) || this.ShowFixedRewards() || this.ShowNextCompletedQuest() || this.ShowNextReward();
	}

	// Token: 0x06003BA6 RID: 15270 RVA: 0x00121429 File Offset: 0x0011F629
	protected virtual void OnTwoScoopShown()
	{
	}

	// Token: 0x06003BA7 RID: 15271 RVA: 0x0012142B File Offset: 0x0011F62B
	protected virtual void OnTwoScoopHidden()
	{
	}

	// Token: 0x06003BA8 RID: 15272 RVA: 0x00121430 File Offset: 0x0011F630
	protected void BackToMode(SceneMgr.Mode mode)
	{
		CollectionManager.Get().RemoveAchievesCompletedListener(new CollectionManager.DelOnAchievesCompleted(this.OnCollectionAchievesCompleted));
		this.HideTwoScoop();
		if (!this.m_hasAlreadySetMode)
		{
			this.m_hasAlreadySetMode = true;
			base.StartCoroutine(this.ToMode(mode));
			Navigation.Clear();
		}
	}

	// Token: 0x06003BA9 RID: 15273 RVA: 0x00121480 File Offset: 0x0011F680
	private IEnumerator ToMode(SceneMgr.Mode mode)
	{
		yield return new WaitForSeconds(0.5f);
		SceneMgr.Get().SetNextMode(mode);
		yield break;
	}

	// Token: 0x06003BAA RID: 15274 RVA: 0x001214A4 File Offset: 0x0011F6A4
	private void ReturnToPreviousMode()
	{
		SceneMgr.Mode postGameSceneMode = GameMgr.Get().GetPostGameSceneMode();
		GameMgr.Get().PreparePostGameSceneMode(postGameSceneMode);
		this.BackToMode(postGameSceneMode);
	}

	// Token: 0x06003BAB RID: 15275 RVA: 0x001214D0 File Offset: 0x0011F6D0
	protected void HideTwoScoop()
	{
		if (!this.m_twoScoop.IsShown())
		{
			return;
		}
		this.m_twoScoop.Hide();
		this.OnTwoScoopHidden();
		if (EndGameScreen.OnTwoScoopsShown != null)
		{
			EndGameScreen.OnTwoScoopsShown(false, this.m_twoScoop);
		}
		if (InputManager.Get() != null)
		{
			InputManager.Get().EnableInput();
		}
	}

	// Token: 0x06003BAC RID: 15276 RVA: 0x00121534 File Offset: 0x0011F734
	private void ShowTwoScoop()
	{
		base.StartCoroutine(this.ShowTwoScoopWhenReady());
	}

	// Token: 0x06003BAD RID: 15277 RVA: 0x00121544 File Offset: 0x0011F744
	private IEnumerator ShowTwoScoopWhenReady()
	{
		if (this.ShouldMakeUtilRequests())
		{
			while (!this.m_netCacheReady)
			{
				yield return null;
			}
			while (!this.m_achievesReady)
			{
				yield return null;
			}
		}
		while (!this.m_rewardsLoaded)
		{
			yield return null;
		}
		while (!this.m_twoScoop.IsLoaded())
		{
			yield return null;
		}
		while (this.JustEarnedGoldenHero())
		{
			if (this.m_goldenHeroEventReady)
			{
				break;
			}
			yield return null;
		}
		this.m_twoScoop.Show();
		this.OnTwoScoopShown();
		this.m_haveShownTwoScoop = true;
		if (EndGameScreen.OnTwoScoopsShown != null)
		{
			EndGameScreen.OnTwoScoopsShown(true, this.m_twoScoop);
		}
		yield break;
	}

	// Token: 0x06003BAE RID: 15278 RVA: 0x0012155F File Offset: 0x0011F75F
	protected bool ShouldMakeUtilRequests()
	{
		return Network.ShouldBeConnectedToAurora();
	}

	// Token: 0x06003BAF RID: 15279 RVA: 0x00121570 File Offset: 0x0011F770
	protected bool IsReady()
	{
		return this.m_shown && this.m_netCacheReady && this.m_achievesReady && this.m_rewardsLoaded;
	}

	// Token: 0x06003BB0 RID: 15280 RVA: 0x001215A8 File Offset: 0x0011F7A8
	protected bool InitIfReady()
	{
		if (!this.IsReady() && (this.ShouldMakeUtilRequests() || !this.m_shown))
		{
			return false;
		}
		if (!GameMgr.Get().IsSpectator() && GameMgr.Get().IsPlay() && Options.Get().GetBool(Option.IN_RANKED_PLAY_MODE))
		{
			AssetLoader.Get().LoadGameObject("RankChangeTwoScoop", new AssetLoader.GameObjectCallback(this.OnRankChangeLoaded), null, false);
		}
		else
		{
			this.ShowStandardFlow();
		}
		return true;
	}

	// Token: 0x06003BB1 RID: 15281 RVA: 0x00121631 File Offset: 0x0011F831
	protected virtual void ShowStandardFlow()
	{
		this.ShowTwoScoop();
		if (!UniversalInputManager.UsePhoneUI)
		{
			this.m_continueText.gameObject.SetActive(true);
		}
	}

	// Token: 0x06003BB2 RID: 15282 RVA: 0x0012165C File Offset: 0x0011F85C
	protected virtual void OnNetCacheReady()
	{
		this.m_netCacheReady = true;
		NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
		this.MaybeUpdateRewards();
	}

	// Token: 0x06003BB3 RID: 15283 RVA: 0x00121690 File Offset: 0x0011F890
	private void OnRankChangeLoaded(string name, GameObject go, object callbackData)
	{
		NetCache.NetCacheMedalInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMedalInfo>();
		RankChangeTwoScoop component = go.GetComponent<RankChangeTwoScoop>();
		MedalInfoTranslator medalInfoTranslator = new MedalInfoTranslator(netObject, netObject.PreviousMedalInfo);
		component.Initialize(medalInfoTranslator, new RankChangeTwoScoop.RankChangeClosed(this.ShowStandardFlow));
	}

	// Token: 0x06003BB4 RID: 15284 RVA: 0x001216D0 File Offset: 0x0011F8D0
	private void OnAchievesUpdated(object userData)
	{
		base.StartCoroutine(this.WaitForAchieveManager());
	}

	// Token: 0x06003BB5 RID: 15285 RVA: 0x001216E0 File Offset: 0x0011F8E0
	private IEnumerator WaitForAchieveManager()
	{
		while (!AchieveManager.Get().IsReady())
		{
			yield return null;
		}
		this.m_achievesReady = true;
		this.MaybeUpdateRewards();
		yield break;
	}

	// Token: 0x06003BB6 RID: 15286 RVA: 0x001216FC File Offset: 0x0011F8FC
	private void OnCollectionAchievesCompleted(List<Achievement> achievements)
	{
		if (!GameUtils.AreAllTutorialsComplete())
		{
			return;
		}
		Achievement achieve;
		foreach (Achievement achieve2 in achievements)
		{
			achieve = achieve2;
			if (achieve.RewardTiming == RewardVisualTiming.IMMEDIATE)
			{
				Achievement achievement = this.m_completedQuests.Find((Achievement obj) => achieve.ID == obj.ID);
				if (achievement == null)
				{
					this.m_completedQuests.Add(achieve);
				}
			}
		}
	}

	// Token: 0x06003BB7 RID: 15287 RVA: 0x001217AC File Offset: 0x0011F9AC
	protected bool HasShownScoops()
	{
		return this.m_haveShownTwoScoop;
	}

	// Token: 0x06003BB8 RID: 15288 RVA: 0x001217B4 File Offset: 0x0011F9B4
	protected void SetGoldenHeroEventReady(bool isReady)
	{
		this.m_goldenHeroEventReady = isReady;
	}

	// Token: 0x06003BB9 RID: 15289 RVA: 0x001217BD File Offset: 0x0011F9BD
	private void MaybeUpdateRewards()
	{
		if (!this.m_achievesReady)
		{
			return;
		}
		if (!this.m_netCacheReady)
		{
			return;
		}
		this.UpdateRewards();
		this.InitIfReady();
	}

	// Token: 0x06003BBA RID: 15290 RVA: 0x001217E4 File Offset: 0x0011F9E4
	private void UpdateRewards()
	{
		bool flag = true;
		if (GameMgr.Get().IsTutorial())
		{
			flag = GameUtils.AreAllTutorialsComplete();
		}
		List<RewardData> list = null;
		if (flag)
		{
			NetCache.NetCacheProfileNotices netObject = NetCache.Get().GetNetObject<NetCache.NetCacheProfileNotices>();
			List<RewardData> rewards = RewardUtils.GetRewards(netObject.Notices);
			HashSet<RewardVisualTiming> hashSet = new HashSet<RewardVisualTiming>();
			hashSet.Add(RewardVisualTiming.IMMEDIATE);
			HashSet<RewardVisualTiming> rewardTimings = hashSet;
			RewardUtils.GetViewableRewards(rewards, rewardTimings, ref list, ref this.m_completedQuests);
		}
		else
		{
			list = new List<RewardData>();
		}
		if (this.JustEarnedGoldenHero())
		{
			this.LoadGoldenHeroEvent();
		}
		if (!GameMgr.Get().IsSpectator())
		{
			List<RewardData> customRewards = GameState.Get().GetGameEntity().GetCustomRewards();
			if (customRewards != null)
			{
				list.AddRange(customRewards);
			}
		}
		this.m_numRewardsToLoad = list.Count;
		if (this.m_numRewardsToLoad == 0)
		{
			this.m_rewardsLoaded = true;
			return;
		}
		foreach (RewardData rewardData in list)
		{
			rewardData.LoadRewardObject(new Reward.DelOnRewardLoaded(this.RewardObjectLoaded));
		}
	}

	// Token: 0x06003BBB RID: 15291 RVA: 0x0012190C File Offset: 0x0011FB0C
	private void PositionReward(Reward reward)
	{
		reward.transform.parent = base.transform;
		reward.transform.localRotation = Quaternion.identity;
		reward.transform.localPosition = EndGameScreen.REWARD_LOCAL_POS;
	}

	// Token: 0x06003BBC RID: 15292 RVA: 0x00121950 File Offset: 0x0011FB50
	private void RewardObjectLoaded(Reward reward, object callbackData)
	{
		reward.Hide(false);
		this.PositionReward(reward);
		this.m_rewards.Add(reward);
		this.m_numRewardsToLoad--;
		if (this.m_numRewardsToLoad > 0)
		{
			return;
		}
		RewardUtils.SortRewards(ref this.m_rewards);
		this.m_rewardsLoaded = true;
		this.InitIfReady();
	}

	// Token: 0x06003BBD RID: 15293 RVA: 0x001219AC File Offset: 0x0011FBAC
	private void ShowReward(Reward reward)
	{
		RewardUtils.ShowReward(UserAttentionBlocker.NONE, reward, true, EndGameScreen.REWARD_PUNCH_SCALE, EndGameScreen.REWARD_SCALE, string.Empty, null, null);
		base.StartCoroutine(this.NotifyEndGameScreenOfAnimComplete());
	}

	// Token: 0x06003BBE RID: 15294 RVA: 0x001219EC File Offset: 0x0011FBEC
	private IEnumerator NotifyEndGameScreenOfAnimComplete()
	{
		yield return new WaitForSeconds(0.35f);
		if (EndGameScreen.Get() == null)
		{
			yield break;
		}
		EndGameScreen.Get().NotifyOfRewardAnimComplete();
		yield break;
	}

	// Token: 0x06003BBF RID: 15295 RVA: 0x00121A00 File Offset: 0x0011FC00
	private void OnRewardHidden(Reward reward)
	{
		reward.Hide(false);
	}

	// Token: 0x06003BC0 RID: 15296 RVA: 0x00121A09 File Offset: 0x0011FC09
	protected virtual void LoadGoldenHeroEvent()
	{
	}

	// Token: 0x06003BC1 RID: 15297 RVA: 0x00121A0B File Offset: 0x0011FC0B
	protected virtual bool ShowGoldenHeroEvent()
	{
		return false;
	}

	// Token: 0x06003BC2 RID: 15298 RVA: 0x00121A10 File Offset: 0x0011FC10
	protected bool ShowFixedRewards()
	{
		HashSet<RewardVisualTiming> hashSet = new HashSet<RewardVisualTiming>();
		hashSet.Add(RewardVisualTiming.IMMEDIATE);
		HashSet<RewardVisualTiming> rewardVisualTimings = hashSet;
		return FixedRewardsMgr.Get().ShowFixedRewards(UserAttentionBlocker.NONE, rewardVisualTimings, delegate(object userData)
		{
			this.ContinueEvents();
		}, new FixedRewardsMgr.DelPositionNonToastReward(this.PositionReward), EndGameScreen.REWARD_PUNCH_SCALE, EndGameScreen.REWARD_SCALE);
	}

	// Token: 0x06003BC3 RID: 15299 RVA: 0x00121A68 File Offset: 0x0011FC68
	protected bool ShowNextCompletedQuest()
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
		QuestToast.ShowQuestToast(UserAttentionBlocker.NONE, new QuestToast.DelOnCloseQuestToast(this.ShowQuestToastCallback), true, quest);
		return true;
	}

	// Token: 0x06003BC4 RID: 15300 RVA: 0x00121AC9 File Offset: 0x0011FCC9
	protected void ShowQuestToastCallback(object userData)
	{
		this.ContinueEvents();
	}

	// Token: 0x06003BC5 RID: 15301 RVA: 0x00121AD4 File Offset: 0x0011FCD4
	protected bool ShowNextReward()
	{
		if (this.m_currentlyShowingReward != null)
		{
			this.m_currentlyShowingReward.Hide(true);
			this.m_currentlyShowingReward = null;
		}
		if (this.m_rewards.Count == 0)
		{
			return false;
		}
		this.m_animationReadyToSkip = false;
		this.m_currentlyShowingReward = this.m_rewards[0];
		this.m_rewards.RemoveAt(0);
		this.ShowReward(this.m_currentlyShowingReward);
		return true;
	}

	// Token: 0x06003BC6 RID: 15302 RVA: 0x00121B49 File Offset: 0x0011FD49
	protected virtual bool JustEarnedGoldenHero()
	{
		return false;
	}

	// Token: 0x0400261E RID: 9758
	public EndGameTwoScoop m_twoScoop;

	// Token: 0x0400261F RID: 9759
	public PegUIElement m_hitbox;

	// Token: 0x04002620 RID: 9760
	public UberText m_continueText;

	// Token: 0x04002621 RID: 9761
	public static EndGameScreen.OnTwoScoopsShownHandler OnTwoScoopsShown;

	// Token: 0x04002622 RID: 9762
	private static readonly PlatformDependentValue<Vector3> REWARD_SCALE = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
	{
		PC = Vector3.one,
		Phone = new Vector3(0.8f, 0.8f, 0.8f)
	};

	// Token: 0x04002623 RID: 9763
	private static readonly PlatformDependentValue<Vector3> REWARD_PUNCH_SCALE = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
	{
		PC = new Vector3(1.2f, 1.2f, 1.2f),
		Phone = new Vector3(1.25f, 1.25f, 1.25f)
	};

	// Token: 0x04002624 RID: 9764
	private static readonly PlatformDependentValue<Vector3> REWARD_LOCAL_POS = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
	{
		PC = new Vector3(-7.628078f, 8.371922f, -3.883112f),
		Phone = new Vector3(-7.628078f, 8.371922f, -3.94f)
	};

	// Token: 0x04002625 RID: 9765
	protected bool m_shown;

	// Token: 0x04002626 RID: 9766
	protected bool m_netCacheReady;

	// Token: 0x04002627 RID: 9767
	protected bool m_achievesReady;

	// Token: 0x04002628 RID: 9768
	protected bool m_goldenHeroEventReady;

	// Token: 0x04002629 RID: 9769
	protected List<Achievement> m_completedQuests = new List<Achievement>();

	// Token: 0x0400262A RID: 9770
	private List<Reward> m_rewards = new List<Reward>();

	// Token: 0x0400262B RID: 9771
	private int m_numRewardsToLoad;

	// Token: 0x0400262C RID: 9772
	private bool m_rewardsLoaded;

	// Token: 0x0400262D RID: 9773
	private Reward m_currentlyShowingReward;

	// Token: 0x0400262E RID: 9774
	private bool m_haveShownTwoScoop;

	// Token: 0x0400262F RID: 9775
	private bool m_hasAlreadySetMode;

	// Token: 0x04002630 RID: 9776
	protected bool m_animationReadyToSkip;

	// Token: 0x04002631 RID: 9777
	private static EndGameScreen s_instance;

	// Token: 0x020004FF RID: 1279
	// (Invoke) Token: 0x06003BCD RID: 15309
	public delegate void OnTwoScoopsShownHandler(bool shown, EndGameTwoScoop twoScoops);
}
