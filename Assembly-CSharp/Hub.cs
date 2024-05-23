using System;
using System.Collections;
using UnityEngine;

// Token: 0x020005D8 RID: 1496
public class Hub : Scene
{
	// Token: 0x0600427A RID: 17018 RVA: 0x0014074C File Offset: 0x0013E94C
	private void Start()
	{
		if (CollectionManager.Get().GetPreconDeck(TAG_CLASS.MAGE) == null)
		{
			Error.AddFatalLoc("GLOBAL_ERROR_NO_MAGE_PRECON", new object[0]);
			return;
		}
		PresenceMgr.Get().SetStatus(new Enum[]
		{
			PresenceStatus.HUB
		});
		Box box = Box.Get();
		box.AddButtonPressListener(new Box.ButtonPressCallback(this.OnBoxButtonPressed));
		box.m_QuestLogButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.HideTooltipNotification));
		box.m_StoreButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.HideTooltipNotification));
		if (UniversalInputManager.UsePhoneUI)
		{
			box.m_ribbonButtons.m_questLogRibbon.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.HideTooltipNotification));
			box.m_ribbonButtons.m_storeRibbon.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.HideTooltipNotification));
		}
		SceneMgr.Get().NotifySceneLoaded();
		if (SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.LOGIN)
		{
			MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_MainTitle);
		}
		if (!Options.Get().GetBool(Option.HAS_SEEN_HUB, false) && UserAttentionManager.CanShowAttentionGrabber("Hub.Start:" + Option.HAS_SEEN_HUB))
		{
			base.StartCoroutine(this.DoFirstTimeHubWelcome());
		}
		else if (!Options.Get().GetBool(Option.HAS_SEEN_100g_REMINDER, false))
		{
			NetCache.NetCacheGoldBalance netObject = NetCache.Get().GetNetObject<NetCache.NetCacheGoldBalance>();
			if (netObject.GetTotal() >= 100L && UserAttentionManager.CanShowAttentionGrabber("Hub.Start:" + Option.HAS_SEEN_100g_REMINDER))
			{
				NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_FIRST_100_GOLD"), "VO_INNKEEPER_FIRST_100_GOLD", 0f, null);
				Options.Get().SetBool(Option.HAS_SEEN_100g_REMINDER, true);
			}
		}
		else if (TavernBrawlManager.Get().IsFirstTimeSeeingThisFeature)
		{
			Hub.DoTavernBrawlIntroVO();
		}
		StoreManager.Get().RegisterSuccessfulPurchaseListener(new StoreManager.SuccessfulPurchaseCallback(this.OnAdventureBundlePurchase));
		SpecialEventType activeEventType = SpecialEventVisualMgr.GetActiveEventType();
		if (activeEventType != SpecialEventType.IGNORE && AchieveManager.Get().HasUnlockedFeature(Achievement.UnlockableFeature.FORGE))
		{
			SpecialEventVisualMgr.Get().LoadEvent(activeEventType);
			SceneMgr.Get().RegisterSceneUnloadedEvent(new SceneMgr.SceneUnloadedCallback(this.OnSceneUnloaded));
		}
		if (TavernBrawlManager.Get().IsFirstTimeSeeingCurrentSeason && UserAttentionManager.CanShowAttentionGrabber("Hub.TavernBrawl.IsFirstTimeSeeingCurrentSeason") && !Hub.s_hasAlreadyShownTBAnimation)
		{
			Hub.s_hasAlreadyShownTBAnimation = true;
			base.StartCoroutine(this.DoTavernBrawlAnims());
		}
		TavernBrawlManager.Get().OnTavernBrawlUpdated += new Action(this.DoTavernBrawlAnimsCB);
		NetCache.Get().RegisterUpdatedListener(typeof(NetCache.NetCacheFeatures), new Action(this.DoTavernBrawlAnimsCB));
		NetCache.Get().RegisterUpdatedListener(typeof(NetCache.NetCacheHeroLevels), new Action(this.DoTavernBrawlAnimsCB));
	}

	// Token: 0x0600427B RID: 17019 RVA: 0x00140A04 File Offset: 0x0013EC04
	private void OnDestroy()
	{
		TavernBrawlManager.Get().OnTavernBrawlUpdated -= new Action(this.DoTavernBrawlAnimsCB);
		NetCache.Get().RemoveUpdatedListener(typeof(NetCache.NetCacheFeatures), new Action(this.DoTavernBrawlAnimsCB));
		NetCache.Get().RemoveUpdatedListener(typeof(NetCache.NetCacheHeroLevels), new Action(this.DoTavernBrawlAnimsCB));
	}

	// Token: 0x0600427C RID: 17020 RVA: 0x00140A68 File Offset: 0x0013EC68
	private static void DoTavernBrawlIntroVO()
	{
		if (!NotificationManager.Get().HasSoundPlayedThisSession("VO_INNKEEPER_TAVERNBRAWL_PUSH_32"))
		{
			Action finishCallback = delegate()
			{
				NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_TAVERNBRAWL_DESC1_29"), "VO_INNKEEPER_TAVERNBRAWL_DESC1_29", 0f, null);
			};
			NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_TAVERNBRAWL_PUSH_32"), "VO_INNKEEPER_TAVERNBRAWL_PUSH_32", finishCallback);
			NotificationManager.Get().ForceAddSoundToPlayedList("VO_INNKEEPER_TAVERNBRAWL_PUSH_32");
		}
	}

	// Token: 0x0600427D RID: 17021 RVA: 0x00140AD2 File Offset: 0x0013ECD2
	private void DoTavernBrawlAnimsCB()
	{
		base.StartCoroutine(this.DoTavernBrawlAnims());
	}

	// Token: 0x0600427E RID: 17022 RVA: 0x00140AE4 File Offset: 0x0013ECE4
	private IEnumerator DoTavernBrawlAnims()
	{
		Box theBox = Box.Get();
		if (!theBox.UpdateTavernBrawlButtonState(true))
		{
			yield break;
		}
		if (TavernBrawlManager.Get().IsTavernBrawlActive)
		{
			bool isFirstTimeSeason = TavernBrawlManager.Get().IsFirstTimeSeeingCurrentSeason;
			if (isFirstTimeSeason || theBox.IsTavernBrawlButtonDeactivated)
			{
				theBox.UpdateTavernBrawlButtonState(false);
				if (isFirstTimeSeason)
				{
					yield return new WaitForSeconds(1.5f);
				}
				if (TavernBrawlManager.Get().IsFirstTimeSeeingThisFeature)
				{
					Hub.DoTavernBrawlIntroVO();
				}
				if (theBox.m_tavernBrawlActivateSound != string.Empty)
				{
					SoundManager.Get().LoadAndPlay(theBox.m_tavernBrawlActivateSound);
				}
				theBox.PlayTavernBrawlButtonActivation(true, false);
				yield return new WaitForSeconds(0.65f);
				CameraShakeMgr.Shake(Camera.main, new Vector3(0.5f, 0.5f, 0.5f), 0.3f);
				theBox.UpdateTavernBrawlButtonState(true);
			}
		}
		else if (!theBox.IsTavernBrawlButtonDeactivated)
		{
			if (theBox.m_tavernBrawlDeactivateSound != string.Empty)
			{
				SoundManager.Get().LoadAndPlay(Box.Get().m_tavernBrawlDeactivateSound);
			}
			theBox.PlayTavernBrawlButtonActivation(false, false);
		}
		yield break;
	}

	// Token: 0x0600427F RID: 17023 RVA: 0x00140AF8 File Offset: 0x0013ECF8
	private void Update()
	{
		Network.Get().ProcessNetwork();
	}

	// Token: 0x06004280 RID: 17024 RVA: 0x00140B04 File Offset: 0x0013ED04
	public override void Unload()
	{
		StoreManager.Get().RemoveSuccessfulPurchaseListener(new StoreManager.SuccessfulPurchaseCallback(this.OnAdventureBundlePurchase));
		this.HideTooltipNotification(null);
		Box box = Box.Get();
		box.m_QuestLogButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.HideTooltipNotification));
		box.m_StoreButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.HideTooltipNotification));
		box.RemoveButtonPressListener(new Box.ButtonPressCallback(this.OnBoxButtonPressed));
	}

	// Token: 0x06004281 RID: 17025 RVA: 0x00140B7C File Offset: 0x0013ED7C
	private void OnSceneUnloaded(SceneMgr.Mode prevMode, Scene prevScene, object userData)
	{
		SpecialEventType activeEventType = SpecialEventVisualMgr.GetActiveEventType();
		if (activeEventType != SpecialEventType.IGNORE)
		{
			SpecialEventVisualMgr.Get().UnloadEvent(activeEventType);
		}
		SceneMgr.Get().UnregisterSceneUnloadedEvent(new SceneMgr.SceneUnloadedCallback(this.OnSceneUnloaded));
	}

	// Token: 0x06004282 RID: 17026 RVA: 0x00140BB8 File Offset: 0x0013EDB8
	private void OnBoxButtonPressed(Box.ButtonType buttonType, object userData)
	{
		if (buttonType == Box.ButtonType.TOURNAMENT)
		{
			SceneMgr.Get().SetNextMode(SceneMgr.Mode.TOURNAMENT);
			Tournament.Get().NotifyOfBoxTransitionStart();
		}
		else if (buttonType == Box.ButtonType.FORGE)
		{
			SceneMgr.Get().SetNextMode(SceneMgr.Mode.DRAFT);
		}
		else if (buttonType == Box.ButtonType.ADVENTURE)
		{
			AdventureConfig.Get().ResetSubScene();
			SceneMgr.Get().SetNextMode(SceneMgr.Mode.ADVENTURE);
		}
		else if (buttonType == Box.ButtonType.COLLECTION)
		{
			CollectionManager.Get().NotifyOfBoxTransitionStart();
			SceneMgr.Get().SetNextMode(SceneMgr.Mode.COLLECTIONMANAGER);
		}
		else if (buttonType == Box.ButtonType.OPEN_PACKS)
		{
			SceneMgr.Get().SetNextMode(SceneMgr.Mode.PACKOPENING);
		}
		else if (buttonType == Box.ButtonType.TAVERN_BRAWL)
		{
			SceneMgr.Get().SetNextMode(SceneMgr.Mode.TAVERN_BRAWL);
		}
		else if (buttonType == Box.ButtonType.SET_ROTATION)
		{
			Log.Kyle.Print("OnBoxButtonPressed  Box.ButtonType.SET_ROTATION", new object[0]);
			SceneMgr.Get().SetNextMode(SceneMgr.Mode.TOURNAMENT);
			Tournament.Get().NotifyOfBoxTransitionStart();
		}
	}

	// Token: 0x06004283 RID: 17027 RVA: 0x00140CA4 File Offset: 0x0013EEA4
	private IEnumerator DoFirstTimeHubWelcome()
	{
		yield return new WaitForSeconds(0.5f);
		while ((StoreManager.Get() != null && StoreManager.Get().IsShown()) || (QuestLog.Get() != null && QuestLog.Get().IsShown()))
		{
			yield return null;
		}
		while (AchieveManager.Get().HasActiveQuests(true) || WelcomeQuests.Get() != null)
		{
			yield return null;
		}
		NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_1ST_HUB_06"), "VO_INNKEEPER_1ST_HUB_06", 3f, null);
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_PracticeNotification = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, new Vector3(-30.46f, 33.5f, 3f), 25f * Vector3.one, GameStrings.Get("GLUE_PRACTICE_HINT"), true);
		}
		else
		{
			this.m_PracticeNotification = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, new Vector3(-33.62785f, 33.52365f, 3f), 15f * Vector3.one, GameStrings.Get("GLUE_PRACTICE_HINT"), true);
		}
		this.m_PracticeNotification.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
		Options.Get().SetBool(Option.HAS_SEEN_HUB, true);
		AdTrackingManager.Get().TrackFirstLogin();
		yield break;
	}

	// Token: 0x06004284 RID: 17028 RVA: 0x00140CC0 File Offset: 0x0013EEC0
	private void OnAdventureBundlePurchase(Network.Bundle bundle, PaymentMethod purchaseMethod, object userData)
	{
		if (bundle == null || bundle.Items == null)
		{
			return;
		}
		foreach (Network.BundleItem bundleItem in bundle.Items)
		{
			if (bundleItem.Product == 3)
			{
				Options.Get().SetBool(Option.BUNDLE_JUST_PURCHASE_IN_HUB, true);
				AdventureConfig.Get().SetSelectedAdventureMode(AdventureDbId.NAXXRAMAS, AdventureModeDbId.NORMAL);
				break;
			}
		}
	}

	// Token: 0x06004285 RID: 17029 RVA: 0x00140D54 File Offset: 0x0013EF54
	private void HideTooltipNotification(UIEvent e)
	{
		if (this.m_PracticeNotification != null)
		{
			NotificationManager.Get().DestroyNotification(this.m_PracticeNotification, 0f);
		}
	}

	// Token: 0x04002A52 RID: 10834
	public static bool s_hasAlreadyShownTBAnimation;

	// Token: 0x04002A53 RID: 10835
	private Notification m_PracticeNotification;
}
