using System;
using System.Collections;
using System.Collections.Generic;
using PegasusShared;
using UnityEngine;

// Token: 0x0200036D RID: 877
[CustomEditClass]
public class DeckPickerTrayDisplay : MonoBehaviour
{
	// Token: 0x06002C81 RID: 11393 RVA: 0x000DCCC8 File Offset: 0x000DAEC8
	public DeckPickerTrayDisplay()
	{
		List<TAG_CLASS> list = new List<TAG_CLASS>();
		list.Add(TAG_CLASS.WARRIOR);
		list.Add(TAG_CLASS.SHAMAN);
		list.Add(TAG_CLASS.ROGUE);
		list.Add(TAG_CLASS.PALADIN);
		list.Add(TAG_CLASS.HUNTER);
		list.Add(TAG_CLASS.DRUID);
		list.Add(TAG_CLASS.WARLOCK);
		list.Add(TAG_CLASS.MAGE);
		list.Add(TAG_CLASS.PRIEST);
		this.HERO_CLASSES = list;
		this.m_heroButtons = new List<HeroPickerButton>();
		this.m_heroPowerDefs = new Hashtable();
		this.m_numPagesToShow = 1;
		this.m_heroDefsLoading = int.MaxValue;
		this.m_heroPowerDefsLoading = int.MaxValue;
		this.m_DeckTrayLoadedListeners = new List<DeckPickerTrayDisplay.DeckTrayLoaded>();
		this.m_numCardsPerClass = -1f;
		base..ctor();
	}

	// Token: 0x06002C82 RID: 11394 RVA: 0x000DCDBC File Offset: 0x000DAFBC
	// Note: this type is marked as 'beforefieldinit'.
	static DeckPickerTrayDisplay()
	{
		DeckPickerTrayDisplay.INNKEEPER_QUOTE_POS = new Vector3(103f, NotificationManager.DEPTH, 42f);
		DeckPickerTrayDisplay.s_selectHeroCoroutine = null;
		DeckPickerTrayDisplay.HighlightSelectedDeck = new PlatformDependentValue<bool>(PlatformCategory.Screen)
		{
			Phone = false,
			Tablet = true,
			PC = true
		};
	}

	// Token: 0x06002C83 RID: 11395 RVA: 0x000DCE14 File Offset: 0x000DB014
	private void Awake()
	{
		this.m_randomDeckPickerTray.transform.localPosition = this.m_randomDecksShownBone.transform.localPosition;
		SoundManager.Get().Load("hero_panel_slide_on");
		SoundManager.Get().Load("hero_panel_slide_off");
		if (SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.GAMEPLAY)
		{
			this.m_delayButtonAnims = true;
			LoadingScreen.Get().RegisterFinishedTransitionListener(new LoadingScreen.FinishedTransitionCallback(this.OnTransitionFromGameplayFinished));
		}
		DeckPickerTray.Get().RegisterHandlers();
		SceneMgr.Get().RegisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
		DeckPickerTrayDisplay.s_instance = this;
		if (this.m_heroPowerShadowQuad != null)
		{
			this.m_heroPowerShadowQuad.SetActive(false);
		}
		this.LoadHero();
		if (this.ShouldShowHeroPower())
		{
			this.LoadHeroPower();
			this.LoadGoldenHeroPower();
		}
		this.m_heroName.RichText = false;
		if (this.m_backButton != null)
		{
			this.m_backButton.SetText(GameStrings.Get("GLOBAL_BACK"));
			this.m_backButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.BackButtonPress));
		}
		if (this.m_collectionButton != null)
		{
			this.EnableCollectionButton(this.ShouldShowCollectionButton());
			if (this.m_collectionButton.IsEnabled())
			{
				this.m_collectionButton.SetText(GameStrings.Get("GLUE_MY_COLLECTION"));
				this.m_collectionButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.CollectionButtonPress));
			}
		}
		this.m_playButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.PlayGameButtonRelease));
		this.EnablePlayButton(false);
		this.m_heroName.Text = string.Empty;
		this.m_xpBar = Object.Instantiate<HeroXPBar>(this.m_xpBarPrefab);
		this.m_xpBar.m_soloLevelLimit = NetCache.Get().GetNetObject<NetCache.NetCacheRewardProgress>().XPSoloLimit;
		Navigation.PushIfNotOnTop(new Navigation.NavigateBackHandler(DeckPickerTrayDisplay.OnNavigateBack));
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_slidingTray = base.gameObject.GetComponentInChildren<SlidingTray>();
			this.m_slidingTray.RegisterTrayToggleListener(new SlidingTray.TrayToggledListener(this.OnSlidingTrayToggled));
		}
		base.StartCoroutine(this.InitModeWhenReady());
	}

	// Token: 0x06002C84 RID: 11396 RVA: 0x000DD048 File Offset: 0x000DB248
	private void Start()
	{
		GameObject gameObject = this.m_leftArrowNestedPrefab.PrefabGameObject();
		this.m_leftArrow = gameObject.GetComponent<UIBButton>();
		this.m_leftArrow.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnShowFirstPage));
		gameObject = this.m_rightArrowNestedPrefab.PrefabGameObject();
		this.m_rightArrow = gameObject.GetComponent<UIBButton>();
		this.m_rightArrow.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnShowSecondPage));
		if (this.m_numPagesToShow <= 1)
		{
			this.m_leftArrow.gameObject.SetActive(false);
			this.m_rightArrow.gameObject.SetActive(false);
		}
		if (this.m_switchFormatButtonContainer != null && this.m_switchFormatButtonContainer.PrefabGameObject() != null)
		{
			this.m_switchFormatButton = this.m_switchFormatButtonContainer.PrefabGameObject().GetComponent<SwitchFormatButton>();
			bool flag = (SceneMgr.Get().GetMode() == SceneMgr.Mode.TOURNAMENT || SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER) && CollectionManager.Get().ShouldAccountSeeStandardWild();
			if (flag)
			{
				this.m_switchFormatButton.Uncover();
				this.m_switchFormatButton.SetFormat(Options.Get().GetBool(Option.IN_WILD_MODE), true);
				if (flag)
				{
					this.m_switchFormatButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.SwitchFormatButtonPress));
					this.m_switchFormatButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.SwitchFormatButtonRollover));
					this.m_switchFormatButton.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.SwitchFormatButtonRollout));
				}
			}
			else
			{
				this.m_switchFormatButton.Cover();
				this.m_switchFormatButton.Disable();
			}
		}
	}

	// Token: 0x06002C85 RID: 11397 RVA: 0x000DD1E8 File Offset: 0x000DB3E8
	private void OnDestroy()
	{
		this.HideDemoQuotes();
		DeckPickerTray.Get().UnregisterHandlers();
		if (TournamentDisplay.Get() != null)
		{
			TournamentDisplay.Get().RemoveMedalChangedListener(new TournamentDisplay.DelMedalChanged(this.OnMedalChanged));
		}
		if (FriendChallengeMgr.Get() != null && DeckPickerTrayDisplay.Get() != null)
		{
			FriendChallengeMgr.Get().RemoveChangedListener(new FriendChallengeMgr.ChangedCallback(DeckPickerTrayDisplay.Get().OnFriendChallengeChanged));
		}
		DeckPickerTrayDisplay.s_instance = null;
	}

	// Token: 0x06002C86 RID: 11398 RVA: 0x000DD266 File Offset: 0x000DB466
	public static DeckPickerTrayDisplay Get()
	{
		return DeckPickerTrayDisplay.s_instance;
	}

	// Token: 0x06002C87 RID: 11399 RVA: 0x000DD26D File Offset: 0x000DB46D
	public void Init()
	{
		base.StartCoroutine(this.InitDeckDependentElements());
	}

	// Token: 0x06002C88 RID: 11400 RVA: 0x000DD27C File Offset: 0x000DB47C
	public bool IsShowingCustomDecks()
	{
		return this.m_deckPickerMode == DeckPickerMode.CUSTOM;
	}

	// Token: 0x06002C89 RID: 11401 RVA: 0x000DD287 File Offset: 0x000DB487
	public void SuckInFinished()
	{
		this.m_randomDeckPickerTray.SetActive(false);
		SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
	}

	// Token: 0x06002C8A RID: 11402 RVA: 0x000DD2A0 File Offset: 0x000DB4A0
	private void OnShowSecondPage(UIEvent e)
	{
		SoundManager.Get().LoadAndPlay("hero_panel_slide_off");
		this.ShowSecondPage();
	}

	// Token: 0x06002C8B RID: 11403 RVA: 0x000DD2B8 File Offset: 0x000DB4B8
	public void ResetCurrentMode()
	{
		if (this.IsShowingCustomDecks())
		{
			if (this.m_selectedCustomDeckBox != null)
			{
				this.EnablePlayButton(true);
				this.RaiseHero();
			}
			else
			{
				if (this.m_selectedHeroButton != null)
				{
					this.RaiseHero();
				}
				this.EnablePlayButton(false);
			}
		}
		else if (this.m_selectedHeroButton != null)
		{
			this.EnablePlayButton(true);
			this.RaiseHero();
		}
		else
		{
			this.EnablePlayButton(false);
		}
		this.EnableHeroButtons();
	}

	// Token: 0x06002C8C RID: 11404 RVA: 0x000DD348 File Offset: 0x000DB548
	public int GetSelectedHeroLevel()
	{
		if (this.m_selectedHeroButton == null)
		{
			return 0;
		}
		EntityDef entityDef = this.m_selectedHeroButton.GetFullDef().GetEntityDef();
		NetCache.HeroLevel heroLevel = GameUtils.GetHeroLevel(entityDef.GetClass());
		return heroLevel.CurrentLevel.Level;
	}

	// Token: 0x06002C8D RID: 11405 RVA: 0x000DD390 File Offset: 0x000DB590
	public void SetPlayButtonText(string text)
	{
		this.m_playButton.SetText(text);
	}

	// Token: 0x06002C8E RID: 11406 RVA: 0x000DD39E File Offset: 0x000DB59E
	public void ToggleRankedDetailsTray(bool shown)
	{
		if (!UniversalInputManager.UsePhoneUI)
		{
			return;
		}
		this.m_rankedDetailsTray.ToggleTraySlider(shown, null, true);
	}

	// Token: 0x06002C8F RID: 11407 RVA: 0x000DD3C0 File Offset: 0x000DB5C0
	public long GetSelectedDeckID()
	{
		if (this.IsShowingCustomDecks())
		{
			return (!(this.m_selectedCustomDeckBox == null)) ? this.m_selectedCustomDeckBox.GetDeckID() : 0L;
		}
		return (!(this.m_selectedHeroButton == null)) ? this.m_selectedHeroButton.GetPreconDeckID() : 0L;
	}

	// Token: 0x06002C90 RID: 11408 RVA: 0x000DD420 File Offset: 0x000DB620
	public bool GetSelectDeckIsWild()
	{
		return this.m_deckPickerMode == DeckPickerMode.CUSTOM && !(this.m_selectedCustomDeckBox == null) && this.m_selectedCustomDeckBox.IsWild();
	}

	// Token: 0x06002C91 RID: 11409 RVA: 0x000DD45D File Offset: 0x000DB65D
	public void SetHeaderText(string text)
	{
		this.m_modeName.Text = text;
	}

	// Token: 0x06002C92 RID: 11410 RVA: 0x000DD46C File Offset: 0x000DB66C
	public void UpdateCreateDeckText()
	{
		bool @bool = Options.Get().GetBool(Option.IN_WILD_MODE);
		string key = (!@bool) ? "GLUE_CREATE_STANDARD_DECK" : "GLUE_CREATE_WILD_DECK";
		if (SceneMgr.Get().GetMode() == SceneMgr.Mode.TAVERN_BRAWL)
		{
			key = "GLOBAL_TAVERN_BRAWL";
		}
		this.SetHeaderText(GameStrings.Get(key));
	}

	// Token: 0x06002C93 RID: 11411 RVA: 0x000DD4C4 File Offset: 0x000DB6C4
	public bool UpdateRankedClassWinsPlate()
	{
		if (SceneMgr.Get().GetMode() != SceneMgr.Mode.TOURNAMENT || !Options.Get().GetBool(Option.IN_RANKED_PLAY_MODE) || this.m_heroActor.GetEntityDef() == null)
		{
			this.m_rankedWinsPlate.SetActive(false);
			return false;
		}
		string heroCardID = this.m_heroActor.GetEntityDef().GetCardId();
		TAG_CARD_SET cardSet = this.m_heroActor.GetEntityDef().GetCardSet();
		if (cardSet == TAG_CARD_SET.HERO_SKINS)
		{
			heroCardID = CollectionManager.Get().GetVanillaHeroCardID(this.m_heroActor.GetEntityDef());
		}
		Achievement unlockGoldenHeroAchievement = AchieveManager.Get().GetUnlockGoldenHeroAchievement(heroCardID, TAG_PREMIUM.GOLDEN);
		int progress = unlockGoldenHeroAchievement.Progress;
		if (progress == 0 || progress >= unlockGoldenHeroAchievement.MaxProgress)
		{
			this.m_rankedWinsPlate.SetActive(false);
			return false;
		}
		this.m_rankedWins.Text = GameStrings.Format((!UniversalInputManager.UsePhoneUI) ? "GLOBAL_HERO_WINS" : "GLOBAL_HERO_WINS_PHONE", new object[]
		{
			progress,
			unlockGoldenHeroAchievement.MaxProgress
		});
		this.m_rankedWinsPlate.SetActive(true);
		return true;
	}

	// Token: 0x06002C94 RID: 11412 RVA: 0x000DD5DC File Offset: 0x000DB7DC
	public void UpdateMissingStandardDeckTray(bool animateInTray)
	{
		if (this.m_missingStandardDeckDisplay == null || this.m_setRotationTutorialState != DeckPickerTrayDisplay.SetRotationTutorialState.INACTIVE)
		{
			return;
		}
		if (!CollectionManager.Get().ShouldAccountSeeStandardWild())
		{
			this.m_missingStandardDeckDisplay.Hide();
			return;
		}
		bool flag = SceneMgr.Get().GetMode() == SceneMgr.Mode.TOURNAMENT && !Options.Get().GetBool(Option.IN_WILD_MODE) && this.GetNumValidStandardDecks() <= 0;
		if (flag)
		{
			if (animateInTray)
			{
				this.m_missingStandardDeckDisplay.Show();
			}
			else
			{
				this.m_missingStandardDeckDisplay.ShowImmediately();
			}
			base.StopCoroutine("ArrowDelayedActivate");
			this.m_rightArrow.gameObject.SetActive(false);
			this.m_leftArrow.gameObject.SetActive(false);
		}
		else
		{
			this.m_missingStandardDeckDisplay.Hide();
			this.m_leftArrow.gameObject.SetActive(this.m_showingSecondPage);
			this.m_rightArrow.gameObject.SetActive(!this.m_showingSecondPage && this.m_customPages.Length > 1);
		}
		this.UpdateCollectionButtonGlow();
		this.m_rightArrow.SetEnabled(!flag);
		this.m_leftArrow.SetEnabled(!flag);
		foreach (CustomDeckPage customDeckPage in this.m_customPages)
		{
			customDeckPage.EnableDeckButtons(!flag);
		}
	}

	// Token: 0x06002C95 RID: 11413 RVA: 0x000DD745 File Offset: 0x000DB945
	public bool IsMissingStandardDeckTrayShown()
	{
		return this.m_missingStandardDeckDisplay != null && this.m_missingStandardDeckDisplay.IsShown();
	}

	// Token: 0x06002C96 RID: 11414 RVA: 0x000DD765 File Offset: 0x000DB965
	public void Unload()
	{
		DeckPickerTray.Get().UnregisterHandlers();
	}

	// Token: 0x06002C97 RID: 11415 RVA: 0x000DD771 File Offset: 0x000DB971
	public void OnApplicationPause(bool pauseStatus)
	{
		if (GameMgr.Get().IsFindingGame())
		{
			GameMgr.Get().CancelFindGame();
		}
	}

	// Token: 0x06002C98 RID: 11416 RVA: 0x000DD78D File Offset: 0x000DB98D
	public void OnServerGameStarted()
	{
		FriendChallengeMgr.Get().RemoveChangedListener(new FriendChallengeMgr.ChangedCallback(this.OnFriendChallengeChanged));
	}

	// Token: 0x06002C99 RID: 11417 RVA: 0x000DD7A8 File Offset: 0x000DB9A8
	public void OnServerGameCanceled()
	{
		SceneMgr.Mode mode = SceneMgr.Get().GetMode();
		if (mode == SceneMgr.Mode.FRIENDLY || TavernBrawlManager.IsInTavernBrawlFriendlyChallenge())
		{
			return;
		}
		this.HandleGameStartupFailure();
	}

	// Token: 0x06002C9A RID: 11418 RVA: 0x000DD7D8 File Offset: 0x000DB9D8
	public bool IsLoaded()
	{
		return this.m_Loaded;
	}

	// Token: 0x06002C9B RID: 11419 RVA: 0x000DD7E0 File Offset: 0x000DB9E0
	public void AddDeckTrayLoadedListener(DeckPickerTrayDisplay.DeckTrayLoaded dlg)
	{
		this.m_DeckTrayLoadedListeners.Add(dlg);
	}

	// Token: 0x06002C9C RID: 11420 RVA: 0x000DD7EE File Offset: 0x000DB9EE
	public void RemoveDeckTrayLoadedListener(DeckPickerTrayDisplay.DeckTrayLoaded dlg)
	{
		this.m_DeckTrayLoadedListeners.Remove(dlg);
	}

	// Token: 0x06002C9D RID: 11421 RVA: 0x000DD800 File Offset: 0x000DBA00
	public void HandleGameStartupFailure()
	{
		this.EnablePlayButton(true);
		this.EnableBackButton(true);
		this.EnableHeroButtons();
		SceneMgr.Mode mode = SceneMgr.Get().GetMode();
		SceneMgr.Mode mode2 = mode;
		if (mode2 != SceneMgr.Mode.TOURNAMENT)
		{
			if (mode2 == SceneMgr.Mode.ADVENTURE)
			{
				if (AdventureConfig.Get().GetCurrentSubScene() == AdventureSubScenes.Practice)
				{
					PracticePickerTrayDisplay.Get().OnGameDenied();
				}
			}
		}
		else
		{
			PresenceMgr.Get().SetPrevStatus();
		}
	}

	// Token: 0x06002C9E RID: 11422 RVA: 0x000DD876 File Offset: 0x000DBA76
	public void UpdateRankedPlayDisplay()
	{
		this.m_rankedPlayButtons.UpdateMode();
	}

	// Token: 0x06002C9F RID: 11423 RVA: 0x000DD884 File Offset: 0x000DBA84
	public void ShowClickedWildDeckInStandardPopup()
	{
		if (this.m_switchFormatPopup == null && this.m_innkeeperQuote == null)
		{
			if (SceneMgr.Get().GetMode() == SceneMgr.Mode.TOURNAMENT)
			{
				base.StopCoroutine("ShowSwitchToWildTutorialAfterTransitionsComplete");
				Action action = delegate()
				{
					this.m_switchFormatPopup = null;
				};
				this.m_switchFormatPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.SET_ROTATION_INTRO, this.m_Switch_Format_Notification_Bone.position, this.m_Switch_Format_Notification_Bone.localScale, GameStrings.Get("GLUE_TOURNAMENT_SWITCH_TO_WILD"), true);
				if (this.m_switchFormatPopup != null)
				{
					Notification.PopUpArrowDirection direction = (!UniversalInputManager.UsePhoneUI) ? Notification.PopUpArrowDirection.Up : Notification.PopUpArrowDirection.RightUp;
					this.m_switchFormatPopup.ShowPopUpArrow(direction);
					Notification switchFormatPopup = this.m_switchFormatPopup;
					switchFormatPopup.OnFinishDeathState = (Action)Delegate.Combine(switchFormatPopup.OnFinishDeathState, action);
				}
			}
			Action finishCallback = delegate()
			{
				if (this.m_switchFormatButton != null)
				{
					NotificationManager.Get().DestroyNotification(this.m_switchFormatPopup, 0f);
				}
				this.m_innkeeperQuote = null;
			};
			this.m_innkeeperQuote = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.SET_ROTATION_INTRO, DeckPickerTrayDisplay.INNKEEPER_QUOTE_POS, GameStrings.Get("VO_INNKEEPER_WILD_DECK_WARNING"), "VO_INNKEEPER_Male_Dwarf_SetRotation_32", 0f, finishCallback);
		}
	}

	// Token: 0x06002CA0 RID: 11424 RVA: 0x000DD998 File Offset: 0x000DBB98
	public void ShowSwitchToWildTutorialIfNecessary()
	{
		if (this.m_switchFormatPopup != null)
		{
			return;
		}
		if (!UserAttentionManager.CanShowAttentionGrabber(UserAttentionBlocker.SET_ROTATION_INTRO, "DeckPickerTrayDisplay.ShowSwitchToWildTutorialIfNecessary"))
		{
			return;
		}
		if (Options.Get().GetBool(Option.IN_WILD_MODE))
		{
			Options.Get().SetBool(Option.SHOW_SWITCH_TO_WILD_ON_CREATE_DECK, false);
			Options.Get().SetBool(Option.SHOW_SWITCH_TO_WILD_ON_PLAY_SCREEN, false);
		}
		bool flag = false;
		SceneMgr.Mode mode = SceneMgr.Get().GetMode();
		if (Options.Get().GetBool(Option.SHOW_SWITCH_TO_WILD_ON_CREATE_DECK) && mode == SceneMgr.Mode.COLLECTIONMANAGER)
		{
			flag = true;
			Options.Get().SetBool(Option.SHOW_SWITCH_TO_WILD_ON_CREATE_DECK, false);
		}
		if (Options.Get().GetBool(Option.SHOW_SWITCH_TO_WILD_ON_PLAY_SCREEN) && mode == SceneMgr.Mode.TOURNAMENT)
		{
			flag = true;
			Options.Get().SetBool(Option.SHOW_SWITCH_TO_WILD_ON_PLAY_SCREEN, false);
		}
		if (flag)
		{
			base.StartCoroutine("ShowSwitchToWildTutorialAfterTransitionsComplete");
		}
	}

	// Token: 0x06002CA1 RID: 11425 RVA: 0x000DDA78 File Offset: 0x000DBC78
	private IEnumerator ShowSwitchToWildTutorialAfterTransitionsComplete()
	{
		yield return new WaitForSeconds(1f);
		this.m_switchFormatPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.SET_ROTATION_INTRO, this.m_Switch_Format_Notification_Bone, GameStrings.Get("GLUE_TOURNAMENT_SWITCH_TO_WILD"), true);
		Notification.PopUpArrowDirection arrowDirection = (!UniversalInputManager.UsePhoneUI) ? Notification.PopUpArrowDirection.Up : Notification.PopUpArrowDirection.RightUp;
		this.m_switchFormatPopup.ShowPopUpArrow(arrowDirection);
		this.m_switchFormatPopup.PulseReminderEveryXSeconds(3f);
		NotificationManager.Get().DestroyNotification(this.m_switchFormatPopup, 6f);
		yield break;
	}

	// Token: 0x06002CA2 RID: 11426 RVA: 0x000DDA94 File Offset: 0x000DBC94
	private IEnumerator InitDeckDependentElements()
	{
		if (!this.IsChoosingHero())
		{
			while (!NetCache.Get().IsNetObjectReady<NetCache.NetCacheDecks>())
			{
				yield return null;
			}
			CollectionManager.Get().RequestDeckContentsForDecksWithoutContentsLoaded(null);
			while (!CollectionManager.Get().AreAllDeckContentsReady())
			{
				yield return null;
			}
		}
		List<CollectionDeck> decks = CollectionManager.Get().GetDecks(1);
		this.m_numDecks = decks.Count;
		bool unlockedAllHeroes = AchieveManager.Get().HasUnlockedFeature(Achievement.UnlockableFeature.VANILLA_HEROES);
		this.m_deckPickerMode = ((!unlockedAllHeroes || this.IsChoosingHero()) ? DeckPickerMode.PRECON : DeckPickerMode.CUSTOM);
		this.m_needUnlockAllHeroesTransition = (this.m_deckPickerMode == DeckPickerMode.CUSTOM && !Options.Get().GetBool(Option.HAS_SEEN_UNLOCK_ALL_HEROES_TRANSITION));
		if (this.m_needUnlockAllHeroesTransition && !UserAttentionManager.CanShowAttentionGrabber("DeckPickerTrayDisplay.m_needUnlockAllHeroesTransition"))
		{
			this.m_needUnlockAllHeroesTransition = false;
			if (UserAttentionManager.IsBlockedBy(UserAttentionBlocker.SET_ROTATION_INTRO))
			{
				Options.Get().SetBool(Option.HAS_SEEN_UNLOCK_ALL_HEROES_TRANSITION, true);
			}
		}
		if (this.ShouldShowCustomDecks())
		{
			int pagesNeededForDecks = Mathf.CeilToInt((float)this.m_numDecks / 9f);
			this.m_numPagesToShow = Mathf.Min(pagesNeededForDecks, this.m_customDeckPageContainers.Count);
			this.m_numPagesToShow = Mathf.Max(this.m_numPagesToShow, 1);
			for (int i = 0; i < this.m_numPagesToShow; i++)
			{
				this.m_customDeckPageContainers[i].gameObject.SetActive(true);
			}
		}
		if (this.ShouldShowPreconDecks())
		{
			this.m_numPagesToShow = 1;
			this.m_basicDeckPageContainer.gameObject.SetActive(true);
		}
		if (this.ShouldShowCustomDecks())
		{
			this.InitCustomPages();
		}
		this.InitForMode(SceneMgr.Get().GetMode());
		this.InitHeroPickerButtons();
		yield break;
	}

	// Token: 0x06002CA3 RID: 11427 RVA: 0x000DDAB0 File Offset: 0x000DBCB0
	private void InitForMode(SceneMgr.Mode mode)
	{
		bool @bool = Options.Get().GetBool(Option.IN_WILD_MODE);
		switch (mode)
		{
		case SceneMgr.Mode.COLLECTIONMANAGER:
		case SceneMgr.Mode.ADVENTURE:
			this.SetPlayButtonText(GameStrings.Get("GLUE_CHOOSE"));
			if (Options.Get().GetBool(Option.SHOW_WILD_DISCLAIMER_POPUP_ON_CREATE_DECK) && @bool && UserAttentionManager.CanShowAttentionGrabber(string.Concat(new object[]
			{
				"DeckPickTrayDisplay.InitForMode:",
				mode,
				":",
				Option.SHOW_WILD_DISCLAIMER_POPUP_ON_CREATE_DECK
			})))
			{
				NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, NotificationManager.DEFAULT_CHARACTER_POS, GameStrings.Get("VO_INNKEEPER_PLAY_STANDARD_TO_WILD"), "VO_INNKEEPER_Male_Dwarf_SetRotation_43", 0f, null);
				Options.Get().SetBool(Option.SHOW_WILD_DISCLAIMER_POPUP_ON_CREATE_DECK, false);
			}
			break;
		case SceneMgr.Mode.TOURNAMENT:
		{
			AssetLoader.Get().LoadGameObject((!UniversalInputManager.UsePhoneUI) ? "RankedPlayButtons" : "RankButtons_phone", new AssetLoader.GameObjectCallback(this.RankedPlayButtonsLoaded), null, false);
			this.UpdateMissingStandardDeckTray(false);
			bool flag = CollectionManager.Get().ShouldAccountSeeStandardWild();
			bool bool2 = Options.Get().GetBool(Option.IN_RANKED_PLAY_MODE);
			if (!flag)
			{
				this.SetPlayButtonText((!bool2) ? GameStrings.Get("GLOBAL_PLAY") : GameStrings.Get("GLOBAL_PLAY_RANKED"));
			}
			else
			{
				this.SetPlayButtonText((!@bool) ? GameStrings.Get("GLOBAL_PLAY_STANDARD") : GameStrings.Get("GLOBAL_PLAY_WILD"));
			}
			if (this.m_playButton.IsEnabled())
			{
				this.m_playButton.m_newPlayButtonText.TextAlpha = 1f;
			}
			else
			{
				this.m_playButton.m_newPlayButtonText.TextAlpha = 0f;
			}
			this.UpdateRankedClassWinsPlate();
			if (@bool)
			{
				this.m_switchFormatButton.DoWildFlip();
			}
			break;
		}
		case SceneMgr.Mode.FRIENDLY:
			if (this.IsChoosingHeroForTavernBrawlChallenge())
			{
				this.SetHeaderForTavernBrawl();
			}
			else
			{
				this.SetPlayButtonText(GameStrings.Get("GLUE_CHOOSE"));
			}
			break;
		case SceneMgr.Mode.TAVERN_BRAWL:
			this.SetHeaderForTavernBrawl();
			break;
		}
		switch (mode)
		{
		case SceneMgr.Mode.COLLECTIONMANAGER:
			if (UniversalInputManager.UsePhoneUI)
			{
				this.m_detailsTrayFrame.GetComponent<MeshFilter>().mesh = this.m_alternateDetailsTrayMesh;
				Texture texture = AssetLoader.Get().LoadTexture("HeroPicker_TournamentAndCustom_phone", false);
				Texture wildTexture = AssetLoader.Get().LoadTexture("HeroPicker_TournamentAndCustom_phone_Wild", false);
				this.SetCustomTrayTextures(texture, wildTexture);
				Texture texture2 = AssetLoader.Get().LoadTexture("DeckBuild_DeckHeroTray_phone", false);
				Texture wildTexture2 = AssetLoader.Get().LoadTexture("DeckBuild_DeckHeroTray_phone_Wild", false);
				this.SetDetailsTrayTextures(texture2, wildTexture2);
			}
			else
			{
				Texture texture2 = AssetLoader.Get().LoadTexture("HeroPicker_CreateDeck", false);
				Texture wildTexture2 = AssetLoader.Get().LoadTexture("HeroPicker_CreateDeck_Wild", false);
				this.SetTrayTextures(texture2, wildTexture2);
			}
			this.UpdateTrayTransitionValues(@bool, false);
			this.m_keyholeTextureOffset = new Vector2(0f, 0f);
			break;
		case SceneMgr.Mode.TOURNAMENT:
			if (UniversalInputManager.UsePhoneUI)
			{
				Texture texture = AssetLoader.Get().LoadTexture("HeroPicker_TournamentAndCustom_phone", false);
				Texture wildTexture = AssetLoader.Get().LoadTexture("HeroPicker_TournamentAndCustom_phone_Wild", false);
				this.SetCustomTrayTextures(texture, wildTexture);
				Texture texture2 = AssetLoader.Get().LoadTexture("HeroPicker_Tournament_HeroTray_phone", false);
				Texture wildTexture2 = AssetLoader.Get().LoadTexture("HeroPicker_Tournament_HeroTray_phone_Wild", false);
				this.SetDetailsTrayTextures(texture2, wildTexture2);
			}
			else
			{
				if (this.ShouldShowCustomDecks())
				{
					Texture texture = AssetLoader.Get().LoadTexture("HeroPicker_Custom_Tournament", false);
					Texture wildTexture = AssetLoader.Get().LoadTexture("HeroPicker_Custom_Tournament_Wild", false);
					this.SetCustomTrayTextures(texture, wildTexture);
				}
				Texture texture2 = AssetLoader.Get().LoadTexture("HeroPicker_Tournament", false);
				Texture wildTexture2 = AssetLoader.Get().LoadTexture("HeroPicker_Tournament_Wild", false);
				this.SetTrayTextures(texture2, wildTexture2);
			}
			this.UpdateTrayTransitionValues(@bool, false);
			this.m_keyholeTextureOffset = new Vector2(0f, 0f);
			break;
		case SceneMgr.Mode.FRIENDLY:
			if (this.IsChoosingHeroForTavernBrawlChallenge())
			{
				this.SetTexturesForTavernBrawl();
			}
			else
			{
				if (UniversalInputManager.UsePhoneUI)
				{
					this.m_detailsTrayFrame.GetComponent<MeshFilter>().mesh = this.m_alternateDetailsTrayMesh;
					Texture texture = AssetLoader.Get().LoadTexture("HeroPicker_FriendlyAndCustom_phone", false);
					Texture wildTexture = AssetLoader.Get().LoadTexture("HeroPicker_FriendlyAndCustom_phone_Wild", false);
					this.SetCustomTrayTextures(texture, wildTexture);
					Texture texture2 = AssetLoader.Get().LoadTexture("Friendly_DeckHeroTray_phone", false);
					Texture wildTexture2 = AssetLoader.Get().LoadTexture("Friendly_DeckHeroTray_phone_Wild", false);
					this.SetDetailsTrayTextures(texture2, wildTexture2);
				}
				else
				{
					if (this.ShouldShowCustomDecks())
					{
						Texture texture = AssetLoader.Get().LoadTexture("HeroPicker_Custom_Friendly", false);
						Texture wildTexture = AssetLoader.Get().LoadTexture("HeroPicker_Custom_Friendly_Wild", false);
						this.SetCustomTrayTextures(texture, wildTexture);
					}
					Texture texture2 = AssetLoader.Get().LoadTexture("HeroPicker_Friendly", false);
					Texture wildTexture2 = AssetLoader.Get().LoadTexture("HeroPicker_Friendly_Wild", false);
					this.SetTrayTextures(texture2, wildTexture2);
				}
				this.UpdateTrayTransitionValues(@bool, false);
				this.m_keyholeTextureOffset = new Vector2(0f, 0.61f);
			}
			break;
		case SceneMgr.Mode.ADVENTURE:
			if (UniversalInputManager.UsePhoneUI)
			{
				this.m_detailsTrayFrame.GetComponent<MeshFilter>().mesh = this.m_alternateDetailsTrayMesh;
				Texture texture = AssetLoader.Get().LoadTexture("HeroPicker_PracticeAndCustom_phone", false);
				this.SetCustomTrayTextures(texture, texture);
				Texture texture2 = AssetLoader.Get().LoadTexture("Practice_DeckHeroTray_phone", false);
				this.SetDetailsTrayTextures(texture2, texture2);
			}
			else
			{
				if (this.ShouldShowCustomDecks())
				{
					Texture texture = AssetLoader.Get().LoadTexture("HeroPicker_Custom_Practice", false);
					this.SetCustomTrayTextures(texture, texture);
				}
				Texture texture2 = AssetLoader.Get().LoadTexture("HeroPicker_Practice", false);
				this.SetTrayTextures(texture2, texture2);
			}
			this.UpdateTrayTransitionValues(false, false);
			this.m_keyholeTextureOffset = new Vector2(0.5f, 0f);
			break;
		case SceneMgr.Mode.TAVERN_BRAWL:
			this.SetTexturesForTavernBrawl();
			break;
		}
	}

	// Token: 0x06002CA4 RID: 11428 RVA: 0x000DE0DC File Offset: 0x000DC2DC
	private void UpdateFormat_Tournament()
	{
		bool @bool = Options.Get().GetBool(Option.IN_WILD_MODE);
		bool flag = CollectionManager.Get().ShouldAccountSeeStandardWild();
		if (flag)
		{
			this.SetPlayButtonText((!@bool) ? GameStrings.Get("GLOBAL_PLAY_STANDARD") : GameStrings.Get("GLOBAL_PLAY_WILD"));
			this.m_switchFormatButton.Uncover();
			this.m_switchFormatButton.SetFormat(@bool, true);
			if (@bool && SceneMgr.Get().GetMode() == SceneMgr.Mode.TOURNAMENT && GameUtils.HasSeenStandardModeTutorial() && !Options.Get().GetBool(Option.HAS_SEEN_WILD_MODE_VO) && UserAttentionManager.CanShowAttentionGrabber("DeckPickerTrayDisplay.UpdateFormat_Tournament:" + Option.HAS_SEEN_WILD_MODE_VO))
			{
				NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, DeckPickerTrayDisplay.INNKEEPER_QUOTE_POS, GameStrings.Get("VO_INNKEEPER_WILD_GAME"), "VO_INNKEEPER_Male_Dwarf_SetRotation_35", 0f, null);
				Options.Get().SetBool(Option.HAS_SEEN_WILD_MODE_VO, true);
			}
			if (this.m_selectedCustomDeckBox != null && this.m_selectedCustomDeckBox.IsWild() && !Options.Get().GetBool(Option.IN_WILD_MODE))
			{
				this.Deselect();
			}
			this.UpdateMissingStandardDeckTray(true);
			this.UpdateCustomTournamentBackgroundAndDecks();
		}
		if (this.m_playButton.IsEnabled())
		{
			this.m_playButton.m_newPlayButtonText.TextAlpha = 1f;
		}
		else
		{
			this.m_playButton.m_newPlayButtonText.TextAlpha = 0f;
		}
		this.UpdateRankedClassWinsPlate();
	}

	// Token: 0x06002CA5 RID: 11429 RVA: 0x000DE260 File Offset: 0x000DC460
	private void UpdateFormat_CollectionManager()
	{
		bool @bool = Options.Get().GetBool(Option.IN_WILD_MODE);
		if (@bool)
		{
			NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, NotificationManager.DEFAULT_CHARACTER_POS, GameStrings.Get("VO_INNKEEPER_PLAY_STANDARD_TO_WILD"), "VO_INNKEEPER_Male_Dwarf_SetRotation_43", 0f, null);
		}
		this.m_switchFormatButton.SetFormat(@bool, true);
		this.UpdateTrayTransitionValues(@bool, true);
	}

	// Token: 0x06002CA6 RID: 11430 RVA: 0x000DE2C0 File Offset: 0x000DC4C0
	private void UpdateCustomTournamentBackgroundAndDecks()
	{
		if (this.m_setRotationTutorialState == DeckPickerTrayDisplay.SetRotationTutorialState.INACTIVE)
		{
			bool @bool = Options.Get().GetBool(Option.IN_WILD_MODE);
			this.UpdateTrayTransitionValues(@bool, true);
			foreach (CustomDeckPage customDeckPage in this.m_customPages)
			{
				customDeckPage.UpdateDeckVisuals(@bool, true, false);
			}
		}
	}

	// Token: 0x06002CA7 RID: 11431 RVA: 0x000DE318 File Offset: 0x000DC518
	private IEnumerator InitButtonAchievements()
	{
		List<Achievement> unlockHeroAchieves = AchieveManager.Get().GetAchievesInGroup(Achievement.AchType.UNLOCK_HERO);
		this.UpdateCollectionButtonGlow();
		foreach (Achievement achievement in unlockHeroAchieves)
		{
			HeroPickerButton button = this.m_heroButtons.Find((HeroPickerButton obj) => obj.GetFullDef().GetEntityDef().GetClass() == achievement.ClassRequirement.Value);
			if (button == null)
			{
				Debug.LogWarning(string.Format("DeckPickerTrayDisplay.InitButtonAchievements() - could not find hero picker button matching UnlockHeroAchievement with class {0}", achievement.ClassRequirement.Value));
			}
			else
			{
				if (achievement.ClassRequirement.Value == TAG_CLASS.MAGE)
				{
					achievement.AckCurrentProgressAndRewardNotices();
				}
				if (this.IsChoosingHero())
				{
					CollectionManager.PreconDeck preconDeck = CollectionManager.Get().GetPreconDeck(achievement.ClassRequirement.Value);
					long preconDeckID = 0L;
					if (preconDeck != null)
					{
						preconDeckID = preconDeck.ID;
					}
					button.SetPreconDeckID(preconDeckID);
					if (achievement.IsCompleted() && preconDeckID == 0L)
					{
						Debug.LogError(string.Format("DeckPickerTrayDisplay.InitButtonAchievements() - preconDeckID = 0 for achievement {0}", achievement));
					}
				}
				else
				{
					List<CollectionDeck> decks = CollectionManager.Get().GetDecksWithClass(achievement.ClassRequirement.Value, 1);
					if (decks.Count < 1 || decks[0] == null)
					{
						if (achievement.IsCompleted())
						{
							Debug.LogErrorFormat("DeckPickerTrayDisplay.InitButtonAchievements() - no normal deck found for hero {0}, despite having achievement {1}!", new object[]
							{
								achievement.ClassRequirement.Value,
								achievement
							});
						}
						button.SetPreconDeckID(0L);
						continue;
					}
					if (decks.Count > 1)
					{
						Debug.LogWarningFormat("DeckPickerTrayDisplay.InitButtonAchievements() - {0} decks found for hero {1}, should only have one!", new object[]
						{
							decks.Count,
							achievement.ClassRequirement.Value
						});
					}
					CollectionDeck deck = decks[0];
					button.SetPreconDeckID(deck.ID);
					button.SetIsDeckValid(deck.IsTourneyValid);
				}
				button.SetProgress(achievement.AcknowledgedProgress, achievement.Progress, achievement.MaxProgress, false);
			}
		}
		this.m_buttonAchievementsInitialized = true;
		while (this.m_delayButtonAnims)
		{
			yield return null;
		}
		foreach (Achievement achievement2 in unlockHeroAchieves)
		{
			HeroPickerButton button2 = this.m_heroButtons.Find((HeroPickerButton obj) => obj.GetFullDef().GetEntityDef().GetClass() == achievement2.ClassRequirement.Value);
			if (button2 == null)
			{
				Debug.LogWarning(string.Format("DeckPickerTrayDisplay.InitButtonAchievements() - could not find hero picker button matching UnlockHeroAchievement with class {0}", achievement2.ClassRequirement.Value));
			}
			else
			{
				if (!this.IsChoosingHero() && button2.GetPreconDeckID() != 0L)
				{
					button2.SetProgress(achievement2.AcknowledgedProgress, achievement2.Progress, achievement2.MaxProgress);
				}
				achievement2.AckCurrentProgressAndRewardNotices();
			}
		}
		yield break;
	}

	// Token: 0x06002CA8 RID: 11432 RVA: 0x000DE334 File Offset: 0x000DC534
	private void SetHeaderForTavernBrawl()
	{
		if (this.m_labelDecoration != null)
		{
			this.m_labelDecoration.SetActive(false);
		}
		string key = "GLUE_CHOOSE";
		if (FriendChallengeMgr.Get().IsChallengeTavernBrawl())
		{
			key = "GLUE_BRAWL_FRIEND";
		}
		else if (TavernBrawlManager.Get().SelectHeroBeforeMission())
		{
			key = "GLUE_BRAWL";
		}
		this.SetPlayButtonText(GameStrings.Get(key));
	}

	// Token: 0x06002CA9 RID: 11433 RVA: 0x000DE3A0 File Offset: 0x000DC5A0
	private void SetTexturesForTavernBrawl()
	{
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_detailsTrayFrame.GetComponent<MeshFilter>().mesh = this.m_alternateDetailsTrayMesh;
			Texture texture = AssetLoader.Get().LoadTexture("HeroPicker_TavernBrawl_phone", false);
			this.SetCustomTrayTextures(texture, texture);
			texture = AssetLoader.Get().LoadTexture("TavernBrawl_DeckHeroTray_phone", false);
			this.SetDetailsTrayTextures(texture, texture);
		}
		else
		{
			Texture texture2 = AssetLoader.Get().LoadTexture("HeroPicker_TavernBrawl", false);
			this.SetTrayTextures(texture2, texture2);
		}
		this.UpdateTrayTransitionValues(false, false);
		this.m_keyholeTextureOffset = new Vector2(0.5f, 0.61f);
	}

	// Token: 0x06002CAA RID: 11434 RVA: 0x000DE440 File Offset: 0x000DC640
	private void InitHeroPickerButtons()
	{
		int i = 0;
		Vector3 heroPickerButtonStart = this.m_heroPickerButtonStart;
		float heroPickerButtonHorizontalSpacing = this.m_heroPickerButtonHorizontalSpacing;
		float heroPickerButtonVerticalSpacing = this.m_heroPickerButtonVerticalSpacing;
		while (i < 9)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.m_heroPrefab);
			GameUtils.SetAutomationName(gameObject, new object[]
			{
				i
			});
			gameObject.transform.parent = this.m_basicDeckPageContainer.transform;
			gameObject.transform.localScale = this.m_heroPickerButtonScale;
			if (i == 0)
			{
				gameObject.transform.localPosition = heroPickerButtonStart;
			}
			else
			{
				float num = heroPickerButtonStart.x - (float)(i % 3) * heroPickerButtonHorizontalSpacing;
				float num2 = (float)Mathf.CeilToInt((float)(i / 3)) * heroPickerButtonVerticalSpacing + heroPickerButtonStart.z;
				gameObject.transform.localPosition = new Vector3(num, heroPickerButtonStart.y, num2);
			}
			HeroPickerButton component = gameObject.transform.FindChild("HeroPremade_Frame").gameObject.GetComponent<HeroPickerButton>();
			int num3 = (!UniversalInputManager.UsePhoneUI) ? 0 : 1;
			component.m_buttonFrame.GetComponent<Renderer>().materials[num3].mainTextureOffset = this.m_keyholeTextureOffset;
			this.m_heroButtons.Add(component);
			i++;
		}
		int num4 = 0;
		this.m_heroDefsLoading = this.m_heroButtons.Count;
		this.m_heroPowerDefsLoading = this.m_heroButtons.Count;
		List<CollectionDeck> decks = CollectionManager.Get().GetDecks(1);
		foreach (HeroPickerButton heroPickerButton in this.m_heroButtons)
		{
			if (num4 >= this.HERO_CLASSES.Count)
			{
				Log.Derek.Print("TournamentDisplay - more buttons than heroes", new object[0]);
				break;
			}
			heroPickerButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.HeroPressed));
			heroPickerButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.MouseOverHero));
			heroPickerButton.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.MouseOutHero));
			heroPickerButton.SetOriginalLocalPosition();
			heroPickerButton.Lock();
			heroPickerButton.SetProgress(0, 0, 1);
			TAG_CLASS tag_CLASS = this.HERO_CLASSES[num4];
			if (this.m_needUnlockAllHeroesTransition && num4 < decks.Count)
			{
				tag_CLASS = decks[num4].GetClass();
			}
			NetCache.CardDefinition favoriteHero = CollectionManager.Get().GetFavoriteHero(tag_CLASS);
			if (favoriteHero == null)
			{
				Debug.LogWarning("Couldn't find Favorite Hero for hero class: " + tag_CLASS + " defaulting to Normal Vanilla Hero!");
				string vanillaHeroCardIDFromClass = CollectionManager.Get().GetVanillaHeroCardIDFromClass(tag_CLASS);
				DeckPickerTrayDisplay.HeroFullDefLoadedCallbackData userData = new DeckPickerTrayDisplay.HeroFullDefLoadedCallbackData(heroPickerButton, TAG_PREMIUM.NORMAL);
				DefLoader.Get().LoadFullDef(vanillaHeroCardIDFromClass, new DefLoader.LoadDefCallback<FullDef>(this.OnHeroFullDefLoaded), userData);
			}
			else
			{
				DeckPickerTrayDisplay.HeroFullDefLoadedCallbackData userData2 = new DeckPickerTrayDisplay.HeroFullDefLoadedCallbackData(heroPickerButton, favoriteHero.Premium);
				DefLoader.Get().LoadFullDef(favoriteHero.Name, new DefLoader.LoadDefCallback<FullDef>(this.OnHeroFullDefLoaded), userData2);
			}
			num4++;
		}
	}

	// Token: 0x06002CAB RID: 11435 RVA: 0x000DE75C File Offset: 0x000DC95C
	private void InitCustomPages()
	{
		this.m_customPages = new CustomDeckPage[this.m_numPagesToShow];
		for (int i = 0; i < this.m_numPagesToShow; i++)
		{
			NestedPrefab nestedPrefab = this.m_customDeckPageContainers[i];
			GameObject gameObject = nestedPrefab.PrefabGameObject();
			if (gameObject == null)
			{
				Debug.LogErrorFormat("DeckPickerTrayDisplay - m_customDeckPageContainer[{0}]'s GameObject has not been loaded!", new object[]
				{
					i
				});
			}
			this.m_customPages[i] = gameObject.GetComponent<CustomDeckPage>();
			this.m_customPages[i].SetDeckButtonCallback(new CustomDeckPage.DeckButtonCallback(this.OnCustomDeckPressed));
		}
		List<CollectionDeck> decks = CollectionManager.Get().GetDecks(1);
		foreach (CustomDeckPage customDeckPage in this.m_customPages)
		{
			int num = Mathf.Min(decks.Count, 9);
			customDeckPage.SetDecks(decks.GetRange(0, num));
			decks.RemoveRange(0, num);
			if (decks.Count <= 0)
			{
				break;
			}
		}
		if (decks.Count > 0)
		{
			Debug.LogWarningFormat("DeckPickerTrayDisplay - {0} more decks than we can display!", new object[]
			{
				decks.Count
			});
		}
	}

	// Token: 0x06002CAC RID: 11436 RVA: 0x000DE888 File Offset: 0x000DCA88
	private void InitMode()
	{
		this.InitRichPresence();
		if (this.IsChoosingHero())
		{
			this.ShowFirstPage();
		}
		else
		{
			this.SetSelectionAndPageFromOptions();
		}
		this.InitExpoDemoMode();
		this.ShowSwitchToWildTutorialIfNecessary();
	}

	// Token: 0x06002CAD RID: 11437 RVA: 0x000DE8C4 File Offset: 0x000DCAC4
	private void InitExpoDemoMode()
	{
		if (!DemoMgr.Get().IsExpoDemo())
		{
			return;
		}
		this.m_leftArrow.gameObject.SetActive(false);
		this.m_rightArrow.gameObject.SetActive(false);
		this.EnableBackButton(false);
		base.StartCoroutine("ShowDemoQuotes");
	}

	// Token: 0x06002CAE RID: 11438 RVA: 0x000DE918 File Offset: 0x000DCB18
	private IEnumerator ShowDemoQuotes()
	{
		string thankQuote = Vars.Key("Demo.ThankQuote").GetStr(string.Empty);
		int thankQuoteTime = Vars.Key("Demo.ThankQuoteMsTime").GetInt(0);
		thankQuote = thankQuote.Replace("\\n", "\n");
		bool showThankQuote = !string.IsNullOrEmpty(thankQuote) && thankQuoteTime > 0;
		if (showThankQuote)
		{
			if (DemoMgr.Get().GetMode() == DemoMode.BLIZZCON_2015)
			{
				this.m_expoThankQuote = NotificationManager.Get().CreateCharacterQuote("Reno_Quote", new Vector3(0f, NotificationManager.DEPTH, 0f), thankQuote, string.Empty, true, (float)thankQuoteTime / 1000f, null, CanvasAnchor.CENTER);
			}
			else
			{
				this.m_expoThankQuote = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, new Vector3(158.1f, NotificationManager.DEPTH, 80.2f), thankQuote, string.Empty, (float)thankQuoteTime / 1000f, null);
			}
			this.EnableExpoClickBlocker(true);
			yield return new WaitForSeconds((float)thankQuoteTime / 1000f);
			this.EnableExpoClickBlocker(false);
		}
		this.ShowIntroQuote();
		yield break;
	}

	// Token: 0x06002CAF RID: 11439 RVA: 0x000DE934 File Offset: 0x000DCB34
	private void ShowIntroQuote()
	{
		this.HideIntroQuote();
		string text = Vars.Key("Demo.IntroQuote").GetStr(string.Empty);
		text = text.Replace("\\n", "\n");
		if (DemoMgr.Get().GetMode() == DemoMode.BLIZZCON_2015)
		{
			this.m_expoIntroQuote = NotificationManager.Get().CreateCharacterQuote("Reno_Quote", new Vector3(0f, NotificationManager.DEPTH, -54.22f), text, string.Empty, true, 0f, null, CanvasAnchor.CENTER);
		}
		else
		{
			this.m_expoIntroQuote = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, new Vector3(147.6f, NotificationManager.DEPTH, 23.1f), text, string.Empty, 0f, null);
		}
	}

	// Token: 0x06002CB0 RID: 11440 RVA: 0x000DE9EC File Offset: 0x000DCBEC
	private void EnableExpoClickBlocker(bool enable)
	{
		if (this.m_expoClickBlocker == null)
		{
			return;
		}
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		if (enable)
		{
			fullScreenFXMgr.SetBlurBrightness(1f);
			fullScreenFXMgr.SetBlurDesaturation(0f);
			fullScreenFXMgr.Vignette(0.4f, 0.4f, iTween.EaseType.easeOutCirc, null);
			fullScreenFXMgr.Blur(1f, 0.4f, iTween.EaseType.easeOutCirc, null);
		}
		else
		{
			fullScreenFXMgr.StopVignette(0.2f, iTween.EaseType.easeOutCirc, null);
			fullScreenFXMgr.StopBlur(0.2f, iTween.EaseType.easeOutCirc, null);
		}
		this.m_expoClickBlocker.gameObject.SetActive(enable);
	}

	// Token: 0x06002CB1 RID: 11441 RVA: 0x000DEA88 File Offset: 0x000DCC88
	private void HideDemoQuotes()
	{
		if (!DemoMgr.Get().IsExpoDemo())
		{
			return;
		}
		base.StopCoroutine("ShowDemoQuotes");
		if (this.m_expoThankQuote != null)
		{
			NotificationManager.Get().DestroyNotification(this.m_expoThankQuote, 0f);
			this.m_expoThankQuote = null;
			FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
			fullScreenFXMgr.StopVignette(0.2f, iTween.EaseType.easeOutCirc, null);
			fullScreenFXMgr.StopBlur(0.2f, iTween.EaseType.easeOutCirc, null);
		}
		this.HideIntroQuote();
	}

	// Token: 0x06002CB2 RID: 11442 RVA: 0x000DEB05 File Offset: 0x000DCD05
	private void HideIntroQuote()
	{
		if (this.m_expoIntroQuote != null)
		{
			NotificationManager.Get().DestroyNotification(this.m_expoIntroQuote, 0f);
			this.m_expoIntroQuote = null;
		}
	}

	// Token: 0x06002CB3 RID: 11443 RVA: 0x000DEB34 File Offset: 0x000DCD34
	private void HideSetRotationNotifications()
	{
		if (this.m_switchFormatPopup != null)
		{
			NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_switchFormatPopup);
			this.m_switchFormatPopup = null;
		}
		if (this.m_innkeeperQuote != null)
		{
			NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_innkeeperQuote);
			this.m_innkeeperQuote = null;
		}
	}

	// Token: 0x06002CB4 RID: 11444 RVA: 0x000DEB94 File Offset: 0x000DCD94
	private void OnTransitionFromGameplayFinished(bool cutoff, object userData)
	{
		SceneMgr.Mode mode = SceneMgr.Get().GetMode();
		if (mode == SceneMgr.Mode.FRIENDLY && !FriendChallengeMgr.Get().HasChallenge())
		{
			this.GoBackUntilOnNavigateBackCalled();
		}
		LoadingScreen.Get().UnregisterFinishedTransitionListener(new LoadingScreen.FinishedTransitionCallback(this.OnTransitionFromGameplayFinished));
		this.m_delayButtonAnims = false;
	}

	// Token: 0x06002CB5 RID: 11445 RVA: 0x000DEBE8 File Offset: 0x000DCDE8
	private void CollectionButtonPress(UIEvent e)
	{
		if (this.ShouldGlowCollectionButton())
		{
			if (!Options.Get().GetBool(Option.HAS_CLICKED_COLLECTION_BUTTON_FOR_NEW_DECK) && this.HaveDecksThatNeedNames())
			{
				Options.Get().SetBool(Option.HAS_CLICKED_COLLECTION_BUTTON_FOR_NEW_DECK, true);
			}
			else if (!Options.Get().GetBool(Option.HAS_CLICKED_COLLECTION_BUTTON_FOR_NEW_CARD) && this.HaveUnseenBasicCards())
			{
				Options.Get().SetBool(Option.HAS_CLICKED_COLLECTION_BUTTON_FOR_NEW_CARD, true);
			}
			if (Options.Get().GetBool(Option.GLOW_COLLECTION_BUTTON_AFTER_SET_ROTATION) && SceneMgr.Get().GetMode() == SceneMgr.Mode.TOURNAMENT)
			{
				Options.Get().SetBool(Option.GLOW_COLLECTION_BUTTON_AFTER_SET_ROTATION, false);
			}
		}
		if (PracticePickerTrayDisplay.Get() != null && PracticePickerTrayDisplay.Get().IsShown())
		{
			Navigation.GoBack();
		}
		SceneMgr.Get().SetNextMode(SceneMgr.Mode.COLLECTIONMANAGER);
	}

	// Token: 0x06002CB6 RID: 11446 RVA: 0x000DECC7 File Offset: 0x000DCEC7
	private void BackButtonPress(UIEvent e)
	{
		Navigation.GoBack();
	}

	// Token: 0x06002CB7 RID: 11447 RVA: 0x000DECD0 File Offset: 0x000DCED0
	private void SwitchFormatButtonPress(UIEvent e)
	{
		if (!this.m_switchFormatButton.IsRotating())
		{
			bool @bool = Options.Get().GetBool(Option.IN_WILD_MODE);
			Options.Get().SetBool(Option.IN_WILD_MODE, !@bool);
			if (SceneMgr.Get().GetMode() == SceneMgr.Mode.TOURNAMENT)
			{
				this.UpdateFormat_Tournament();
				TournamentDisplay.Get().UpdateHeaderText();
				this.m_rankedPlayButtons.OnSwitchFormat();
			}
			if (SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER)
			{
				this.UpdateCreateDeckText();
				this.UpdateFormat_CollectionManager();
			}
		}
	}

	// Token: 0x06002CB8 RID: 11448 RVA: 0x000DED57 File Offset: 0x000DCF57
	private void SwitchFormatButtonRollover(UIEvent e)
	{
		this.m_switchFormatButton.ShowPopUp();
		this.m_switchFormatButton.EnableHighlight(false);
	}

	// Token: 0x06002CB9 RID: 11449 RVA: 0x000DED70 File Offset: 0x000DCF70
	private void SwitchFormatButtonRollout(UIEvent e)
	{
		this.m_switchFormatButton.HidePopUp();
	}

	// Token: 0x06002CBA RID: 11450 RVA: 0x000DED80 File Offset: 0x000DCF80
	public static bool OnNavigateBack()
	{
		if (DeckPickerTrayDisplay.Get() != null && !DeckPickerTrayDisplay.Get().m_backButton.IsEnabled())
		{
			return false;
		}
		SceneMgr.Mode mode = SceneMgr.Get().GetMode();
		switch (mode)
		{
		case SceneMgr.Mode.COLLECTIONMANAGER:
		case SceneMgr.Mode.TAVERN_BRAWL:
			if (CollectionDeckTray.Get() != null)
			{
				CollectionDeckTray.Get().GetDecksContent().CreateNewDeckCancelled();
			}
			if (DeckPickerTrayDisplay.Get() != null && !DeckPickerTrayDisplay.Get().m_heroChosen && CollectionManagerDisplay.Get() != null)
			{
				CollectionManagerDisplay.Get().CancelSelectNewDeckHeroMode();
			}
			if (HeroPickerDisplay.Get() != null)
			{
				HeroPickerDisplay.Get().HideTray(0f);
			}
			PresenceMgr.Get().SetPrevStatus();
			if (mode == SceneMgr.Mode.TAVERN_BRAWL)
			{
				TavernBrawlDisplay.Get().EnablePlayButton();
			}
			if (CollectionManagerDisplay.Get() != null)
			{
				DeckTemplatePicker deckTemplatePicker = (!UniversalInputManager.UsePhoneUI) ? CollectionManagerDisplay.Get().m_pageManager.GetDeckTemplatePicker() : CollectionManagerDisplay.Get().GetPhoneDeckTemplateTray();
				if (deckTemplatePicker != null)
				{
					Navigation.PopUnique(new Navigation.NavigateBackHandler(deckTemplatePicker.OnNavigateBack));
				}
			}
			break;
		case SceneMgr.Mode.TOURNAMENT:
			DeckPickerTrayDisplay.BackOutToHub();
			GameMgr.Get().CancelFindGame();
			break;
		case SceneMgr.Mode.FRIENDLY:
			DeckPickerTrayDisplay.BackOutToHub();
			FriendChallengeMgr.Get().CancelChallenge();
			break;
		case SceneMgr.Mode.ADVENTURE:
			AdventureConfig.Get().ChangeToLastSubScene(true);
			if (AdventureConfig.Get().GetCurrentSubScene() == AdventureSubScenes.Practice)
			{
				PracticePickerTrayDisplay.Get().gameObject.SetActive(false);
			}
			GameMgr.Get().CancelFindGame();
			break;
		}
		return true;
	}

	// Token: 0x06002CBB RID: 11451 RVA: 0x000DEF53 File Offset: 0x000DD153
	private static bool IsBackingOut()
	{
		return SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB);
	}

	// Token: 0x06002CBC RID: 11452 RVA: 0x000DEF60 File Offset: 0x000DD160
	private static void BackOutToHub()
	{
		if (DeckPickerTrayDisplay.IsBackingOut())
		{
			return;
		}
		if (DeckPickerTrayDisplay.Get() != null)
		{
			FriendChallengeMgr.Get().RemoveChangedListener(new FriendChallengeMgr.ChangedCallback(DeckPickerTrayDisplay.Get().OnFriendChallengeChanged));
		}
		if (DeckPickerTrayDisplay.Get() != null && DeckPickerTrayDisplay.Get().m_showingSecondPage)
		{
			DeckPickerTrayDisplay.Get().SuckInPreconDecks();
		}
		else
		{
			SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			DeckPickerTrayDisplay.Get().m_slidingTray.ToggleTraySlider(false, null, true);
		}
	}

	// Token: 0x06002CBD RID: 11453 RVA: 0x000DEFFD File Offset: 0x000DD1FD
	public void PreUnload()
	{
		if (this.m_showingSecondPage && this.m_randomDeckPickerTray.activeSelf)
		{
			this.m_randomDeckPickerTray.SetActive(false);
		}
	}

	// Token: 0x06002CBE RID: 11454 RVA: 0x000DF028 File Offset: 0x000DD228
	private void ShowSecondPage()
	{
		if (iTween.Count(this.m_randomDeckPickerTray) > 0)
		{
			return;
		}
		if (this.m_customPages.Length < 2)
		{
			return;
		}
		this.m_customPages[1].gameObject.SetActive(true);
		this.HideAllPreconHighlights();
		this.LowerHeroButtons();
		if (this.ShouldHandleBoxTransition())
		{
			Box.Get().AddTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
			this.m_randomDeckPickerTray.SetActive(false);
			this.m_randomDeckPickerTray.transform.localPosition = this.m_randomDecksHiddenBone.transform.localPosition;
		}
		else
		{
			iTween.MoveTo(this.m_randomDeckPickerTray, iTween.Hash(new object[]
			{
				"time",
				0.25f,
				"position",
				this.m_randomDecksHiddenBone.transform.localPosition,
				"isLocal",
				true,
				"delay",
				0f
			}));
		}
		this.m_showingSecondPage = true;
		base.StartCoroutine(this.ArrowDelayedActivate(this.m_leftArrow, 0.25f));
		this.m_rightArrow.gameObject.SetActive(false);
		Options.Get().SetBool(Option.HAS_SEEN_CUSTOM_DECK_PICKER, true);
	}

	// Token: 0x06002CBF RID: 11455 RVA: 0x000DF178 File Offset: 0x000DD378
	private IEnumerator ArrowDelayedActivate(UIBButton arrow, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (this.m_missingStandardDeckDisplay != null && !this.m_missingStandardDeckDisplay.IsShown())
		{
			arrow.gameObject.SetActive(true);
		}
		yield break;
	}

	// Token: 0x06002CC0 RID: 11456 RVA: 0x000DF1B0 File Offset: 0x000DD3B0
	private bool ShouldHandleBoxTransition()
	{
		return SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.GAMEPLAY && (Box.Get().IsBusy() || Box.Get().GetState() == Box.State.LOADING || Box.Get().GetState() == Box.State.LOADING_HUB);
	}

	// Token: 0x06002CC1 RID: 11457 RVA: 0x000DF201 File Offset: 0x000DD401
	private void OnBoxTransitionFinished(object userData)
	{
		this.m_randomDeckPickerTray.SetActive(true);
		Box.Get().RemoveTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
	}

	// Token: 0x06002CC2 RID: 11458 RVA: 0x000DF226 File Offset: 0x000DD426
	private void OnScenePreUnload(SceneMgr.Mode prevMode, Scene prevScene, object userData)
	{
		this.HideSetRotationNotifications();
		SceneMgr.Get().UnregisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
	}

	// Token: 0x06002CC3 RID: 11459 RVA: 0x000DF248 File Offset: 0x000DD448
	private void LowerHeroButtons()
	{
		foreach (HeroPickerButton heroPickerButton in this.m_heroButtons)
		{
			if (heroPickerButton.gameObject.activeSelf)
			{
				heroPickerButton.Lower();
			}
		}
	}

	// Token: 0x06002CC4 RID: 11460 RVA: 0x000DF2B4 File Offset: 0x000DD4B4
	private void RaiseHeroButtons()
	{
		foreach (HeroPickerButton heroPickerButton in this.m_heroButtons)
		{
			if (heroPickerButton.gameObject.activeSelf)
			{
				heroPickerButton.Raise();
			}
		}
	}

	// Token: 0x06002CC5 RID: 11461 RVA: 0x000DF320 File Offset: 0x000DD520
	private void OnShowFirstPage(UIEvent e)
	{
		SoundManager.Get().LoadAndPlay("hero_panel_slide_on");
		this.ShowFirstPage();
	}

	// Token: 0x06002CC6 RID: 11462 RVA: 0x000DF338 File Offset: 0x000DD538
	private void ShowFirstPage()
	{
		if (iTween.Count(this.m_randomDeckPickerTray) > 0)
		{
			return;
		}
		this.m_showingSecondPage = false;
		if (this.ShouldShowPreconDecks())
		{
			this.ShowPreconHighlights();
		}
		SceneMgr.Mode mode = SceneMgr.Get().GetMode();
		if (this.IsChoosingHero())
		{
			this.m_leftArrow.gameObject.SetActive(false);
			this.m_rightArrow.gameObject.SetActive(false);
			if (this.m_modeLabelBg != null)
			{
				this.m_modeLabelBg.transform.localEulerAngles = new Vector3(180f, 0f, 0f);
			}
			iTween.MoveTo(this.m_randomDeckPickerTray, iTween.Hash(new object[]
			{
				"time",
				0.25f,
				"position",
				this.m_randomDecksShownBone.transform.localPosition,
				"isLocal",
				true,
				"oncomplete",
				"RaiseHeroButtons",
				"oncompletetarget",
				base.gameObject
			}));
		}
		else if (mode == SceneMgr.Mode.ADVENTURE || mode == SceneMgr.Mode.TOURNAMENT || mode == SceneMgr.Mode.FRIENDLY)
		{
			this.m_leftArrow.gameObject.SetActive(false);
			if (this.m_numPagesToShow > 1)
			{
				base.StartCoroutine(this.ArrowDelayedActivate(this.m_rightArrow, 0.25f));
			}
			else
			{
				this.m_rightArrow.gameObject.SetActive(false);
			}
			iTween.MoveTo(this.m_randomDeckPickerTray, iTween.Hash(new object[]
			{
				"time",
				0.25f,
				"position",
				this.m_randomDecksShownBone.transform.localPosition,
				"isLocal",
				true,
				"oncomplete",
				"RaiseHeroButtons",
				"oncompletetarget",
				base.gameObject
			}));
		}
	}

	// Token: 0x06002CC7 RID: 11463 RVA: 0x000DF540 File Offset: 0x000DD740
	private void SuckInPreconDecks()
	{
		iTween.MoveTo(this.m_randomDeckPickerTray, iTween.Hash(new object[]
		{
			"time",
			0.25f,
			"position",
			this.m_suckedInRandomDecksBone.transform.localPosition,
			"isLocal",
			true,
			"oncomplete",
			"SuckInFinished",
			"oncompletetarget",
			base.gameObject
		}));
	}

	// Token: 0x06002CC8 RID: 11464 RVA: 0x000DF5CC File Offset: 0x000DD7CC
	private void OnCustomDeckPressed(CollectionDeckBoxVisual deckbox, bool showTrayForPhone = true)
	{
		this.SelectCustomDeck(deckbox, showTrayForPhone);
		this.ShowBasicDeckWarning(deckbox);
	}

	// Token: 0x06002CC9 RID: 11465 RVA: 0x000DF5E0 File Offset: 0x000DD7E0
	private void SelectCustomDeck(CollectionDeckBoxVisual deckbox, bool showTrayForPhone = true)
	{
		this.HideDemoQuotes();
		if (!deckbox.IsValid())
		{
			return;
		}
		Options.Get().SetLong(Option.LAST_CUSTOM_DECK_CHOSEN, deckbox.GetDeckID());
		if (DeckPickerTrayDisplay.HighlightSelectedDeck)
		{
			deckbox.SetHighlightState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
		}
		if (!UniversalInputManager.UsePhoneUI)
		{
			deckbox.SetEnabled(false);
		}
		if (this.m_selectedCustomDeckBox != null && this.m_selectedCustomDeckBox != deckbox)
		{
			this.m_selectedCustomDeckBox.SetHighlightState(ActorStateType.HIGHLIGHT_OFF);
			this.m_selectedCustomDeckBox.SetEnabled(true);
		}
		this.m_selectedCustomDeckBox = deckbox;
		this.UpdateHeroInfo(deckbox);
		this.ShowPreconHero(true);
		this.EnablePlayButton(true);
		this.UpdateHeroLockedTooltip(false);
		if (UniversalInputManager.UsePhoneUI && showTrayForPhone)
		{
			this.m_slidingTray.ToggleTraySlider(true, null, true);
		}
	}

	// Token: 0x06002CCA RID: 11466 RVA: 0x000DF6C0 File Offset: 0x000DD8C0
	private void ShowBasicDeckWarning(CollectionDeckBoxVisual deckbox)
	{
		if (this.m_selectedCustomDeckBox != deckbox)
		{
			return;
		}
		if (SceneMgr.Get().GetMode() != SceneMgr.Mode.TOURNAMENT)
		{
			return;
		}
		if (Options.Get().GetBool(Option.HAS_SEEN_BASIC_DECK_WARNING))
		{
			return;
		}
		if (!UserAttentionManager.CanShowAttentionGrabber(UserAttentionBlocker.SET_ROTATION_INTRO, "DeckPickerTrayDisplay.SelectDeck"))
		{
			return;
		}
		if (!CollectionManager.Get().ShouldAccountSeeStandardWild())
		{
			return;
		}
		if (deckbox.GetCollectionDeck() == null || !deckbox.GetCollectionDeck().IsBasicDeck())
		{
			return;
		}
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get("GLUE_BASIC_DECK_WARNING_HEADER");
		popupInfo.m_text = GameStrings.Get("GLUE_BASIC_DECK_WARNING_BODY");
		popupInfo.m_showAlertIcon = true;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
		DialogManager.Get().ShowPopup(popupInfo);
		Options.Get().SetBool(Option.HAS_SEEN_BASIC_DECK_WARNING, true);
	}

	// Token: 0x06002CCB RID: 11467 RVA: 0x000DF794 File Offset: 0x000DD994
	private void HeroPressed(UIEvent e)
	{
		HeroPickerButton button = (HeroPickerButton)e.GetElement();
		this.SelectHero(button, true);
		SoundManager.Get().LoadAndPlay("tournament_screen_select_hero");
		this.HideDemoQuotes();
	}

	// Token: 0x06002CCC RID: 11468 RVA: 0x000DF7CC File Offset: 0x000DD9CC
	private void SelectHero(HeroPickerButton button, bool showTrayForPhone = true)
	{
		if (button == this.m_selectedHeroButton && !UniversalInputManager.UsePhoneUI)
		{
			return;
		}
		if (!this.IsChoosingHero() && !button.IsDeckValid())
		{
			return;
		}
		this.DeselectLastSelectedHero();
		if (DeckPickerTrayDisplay.HighlightSelectedDeck)
		{
			button.SetHighlightState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
		}
		else
		{
			button.SetHighlightState(ActorStateType.HIGHLIGHT_OFF);
		}
		this.m_selectedHeroButton = button;
		this.UpdateHeroInfo(button);
		button.SetSelected(true);
		if (!this.IsChoosingHero() && !button.IsLocked())
		{
			TAG_CLASS @class = button.GetFullDef().GetEntityDef().GetClass();
			Options.Get().SetInt(Option.LAST_PRECON_HERO_CHOSEN, (int)@class);
		}
		this.ShowPreconHero(true);
		if (this.m_tooltip != null)
		{
			Object.DestroyImmediate(this.m_tooltip.gameObject);
		}
		bool flag = button.IsLocked();
		this.EnablePlayButton(!flag);
		this.UpdateHeroLockedTooltip(flag);
		if (UniversalInputManager.UsePhoneUI && showTrayForPhone)
		{
			this.m_slidingTray.ToggleTraySlider(true, null, true);
		}
	}

	// Token: 0x06002CCD RID: 11469 RVA: 0x000DF8E8 File Offset: 0x000DDAE8
	private void UpdateHeroLockedTooltip(bool isLocked)
	{
		if (this.m_tooltip != null)
		{
			Object.DestroyImmediate(this.m_tooltip.gameObject);
		}
		if (!isLocked)
		{
			return;
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this.m_tooltipPrefab);
		SceneUtils.SetLayer(gameObject, GameLayer.Default);
		this.m_tooltip = gameObject.GetComponent<KeywordHelpPanel>();
		this.m_tooltip.Reset();
		this.m_tooltip.Initialize(GameStrings.Get("GLUE_HERO_LOCKED_NAME"), GameStrings.Get("GLUE_HERO_LOCKED_DESC"));
		GameUtils.SetParent(this.m_tooltip, this.m_tooltipBone, false);
	}

	// Token: 0x06002CCE RID: 11470 RVA: 0x000DF978 File Offset: 0x000DDB78
	private void DeselectLastSelectedHero()
	{
		if (this.m_selectedHeroButton == null)
		{
			return;
		}
		this.m_selectedHeroButton.SetHighlightState(ActorStateType.HIGHLIGHT_OFF);
		this.m_selectedHeroButton.SetSelected(false);
	}

	// Token: 0x06002CCF RID: 11471 RVA: 0x000DF9A8 File Offset: 0x000DDBA8
	private void Deselect()
	{
		if (this.m_selectedHeroButton == null && this.m_selectedCustomDeckBox == null)
		{
			return;
		}
		this.EnablePlayButton(false);
		if (this.m_selectedCustomDeckBox != null)
		{
			this.m_selectedCustomDeckBox.SetHighlightState(ActorStateType.HIGHLIGHT_OFF);
			this.m_selectedCustomDeckBox.SetEnabled(true);
			this.m_selectedCustomDeckBox = null;
		}
		this.m_heroActor.SetEntityDef(null);
		this.m_heroActor.SetCardDef(null);
		this.m_heroActor.Hide();
		if (this.m_selectedHeroButton != null)
		{
			this.m_selectedHeroButton.SetHighlightState(ActorStateType.HIGHLIGHT_OFF);
			this.m_selectedHeroButton.SetSelected(false);
			this.m_selectedHeroButton = null;
		}
		if (this.ShouldShowHeroPower())
		{
			this.m_heroPowerActor.SetCardDef(null);
			this.m_heroPowerActor.SetEntityDef(null);
			this.m_heroPowerActor.Hide();
			this.m_goldenHeroPowerActor.SetCardDef(null);
			this.m_goldenHeroPowerActor.SetEntityDef(null);
			this.m_goldenHeroPowerActor.Hide();
			this.m_heroPower.GetComponent<Collider>().enabled = false;
			this.m_goldenHeroPower.GetComponent<Collider>().enabled = false;
			if (this.m_heroPowerShadowQuad != null)
			{
				this.m_heroPowerShadowQuad.SetActive(false);
			}
		}
		this.m_selectedHeroPowerFullDef = null;
		if (this.m_heroPowerBigCard != null)
		{
			iTween.Stop(this.m_heroPowerBigCard.gameObject);
			this.m_heroPowerBigCard.Hide();
		}
		if (this.m_goldenHeroPowerBigCard != null)
		{
			iTween.Stop(this.m_goldenHeroPowerBigCard.gameObject);
			this.m_goldenHeroPowerBigCard.Hide();
		}
		this.m_selectedHeroName = null;
		this.m_heroName.Text = string.Empty;
	}

	// Token: 0x06002CD0 RID: 11472 RVA: 0x000DFB6C File Offset: 0x000DDD6C
	private void MouseOverHero(UIEvent e)
	{
		if (e == null)
		{
			return;
		}
		HeroPickerButton heroPickerButton = (HeroPickerButton)e.GetElement();
		heroPickerButton.SetHighlightState(ActorStateType.HIGHLIGHT_MOUSE_OVER);
		SoundManager.Get().LoadAndPlay("collection_manager_hero_mouse_over");
	}

	// Token: 0x06002CD1 RID: 11473 RVA: 0x000DFBA4 File Offset: 0x000DDDA4
	private void MouseOutHero(UIEvent e)
	{
		HeroPickerButton heroPickerButton = (HeroPickerButton)e.GetElement();
		if (!UniversalInputManager.UsePhoneUI || !heroPickerButton.IsSelected())
		{
			heroPickerButton.SetHighlightState(ActorStateType.HIGHLIGHT_OFF);
		}
	}

	// Token: 0x06002CD2 RID: 11474 RVA: 0x000DFBE0 File Offset: 0x000DDDE0
	private void UpdateHeroInfo(CollectionDeckBoxVisual deckBox)
	{
		FullDef fullDef = deckBox.GetFullDef();
		string text = deckBox.GetDeckNameText().Text;
		this.UpdateHeroInfo(fullDef, text, CollectionManager.Get().GetBestCardPremium(deckBox.GetHeroCardID()));
	}

	// Token: 0x06002CD3 RID: 11475 RVA: 0x000DFC18 File Offset: 0x000DDE18
	private void UpdateHeroInfo(HeroPickerButton button)
	{
		FullDef fullDef = button.GetFullDef();
		EntityDef entityDef = fullDef.GetEntityDef();
		string name = entityDef.GetName();
		TAG_PREMIUM premium = button.GetPremium();
		this.UpdateHeroInfo(fullDef, name, premium);
	}

	// Token: 0x06002CD4 RID: 11476 RVA: 0x000DFC4C File Offset: 0x000DDE4C
	private void UpdateHeroInfo(FullDef fullDef, string heroName, TAG_PREMIUM premium)
	{
		EntityDef entityDef = fullDef.GetEntityDef();
		CardDef cardDef = fullDef.GetCardDef();
		this.m_heroName.Text = heroName;
		this.m_selectedHeroName = entityDef.GetName();
		this.m_heroActor.SetPremium(premium);
		this.m_heroActor.SetEntityDef(entityDef);
		this.m_heroActor.SetCardDef(cardDef);
		this.m_heroActor.UpdateAllComponents();
		this.m_heroActor.SetUnlit();
		this.m_xpBar.m_heroLevel = GameUtils.GetHeroLevel(entityDef.GetClass());
		this.m_xpBar.UpdateDisplay();
		string heroPowerCardIdFromHero = GameUtils.GetHeroPowerCardIdFromHero(entityDef.GetCardId());
		if (this.ShouldShowHeroPower())
		{
			this.UpdateHeroPowerInfo((FullDef)this.m_heroPowerDefs[heroPowerCardIdFromHero], premium);
		}
		this.UpdateRankedClassWinsPlate();
	}

	// Token: 0x06002CD5 RID: 11477 RVA: 0x000DFD10 File Offset: 0x000DDF10
	private void UpdateHeroPowerInfo(FullDef def, TAG_PREMIUM premium)
	{
		this.m_heroPowerActor.SetCardDef(def.GetCardDef());
		this.m_heroPowerActor.SetEntityDef(def.GetEntityDef());
		this.m_heroPowerActor.UpdateAllComponents();
		this.m_selectedHeroPowerFullDef = def;
		this.m_heroPowerActor.SetUnlit();
		this.m_heroPowerActor.GetCardDef().m_AlwaysRenderPremiumPortrait = false;
		this.m_heroPowerActor.UpdateMaterials();
		this.m_goldenHeroPowerActor.SetCardDef(def.GetCardDef());
		this.m_goldenHeroPowerActor.SetEntityDef(def.GetEntityDef());
		this.m_goldenHeroPowerActor.UpdateAllComponents();
		this.m_selectedHeroPowerFullDef = def;
		this.m_goldenHeroPowerActor.SetUnlit();
		if (premium == TAG_PREMIUM.GOLDEN)
		{
			this.m_heroPowerActor.Hide();
			this.m_goldenHeroPowerActor.Show();
			this.m_goldenHeroPower.GetComponent<Collider>().enabled = true;
		}
		else
		{
			this.m_goldenHeroPowerActor.Hide();
			this.m_heroPowerActor.Show();
			this.m_heroPower.GetComponent<Collider>().enabled = true;
			TAG_CARD_SET cardSet = this.m_heroActor.GetEntityDef().GetCardSet();
			if (cardSet == TAG_CARD_SET.HERO_SKINS)
			{
				base.StartCoroutine(this.UpdateHeroSkinHeroPower());
			}
		}
		if (this.m_heroPowerShadowQuad != null)
		{
			this.m_heroPowerShadowQuad.SetActive(true);
		}
	}

	// Token: 0x06002CD6 RID: 11478 RVA: 0x000DFE54 File Offset: 0x000DE054
	private void MouseOverHeroPower(UIEvent e)
	{
		this.m_isMouseOverHeroPower = true;
		TAG_PREMIUM premium = this.m_heroActor.GetPremium();
		if (premium == TAG_PREMIUM.GOLDEN)
		{
			if (this.m_goldenHeroPowerBigCard == null)
			{
				AssetLoader.Get().LoadActor(ActorNames.GetNameWithPremiumType("History_HeroPower", TAG_PREMIUM.GOLDEN), new AssetLoader.GameObjectCallback(this.LoadGoldenHeroPowerCallback), null, false);
			}
			else
			{
				this.ShowGoldenHeroPowerBigCard();
			}
		}
		else if (this.m_heroPowerBigCard == null)
		{
			AssetLoader.Get().LoadActor("History_HeroPower", new AssetLoader.GameObjectCallback(this.LoadHeroPowerCallback), null, false);
		}
		else
		{
			this.ShowHeroPowerBigCard();
		}
	}

	// Token: 0x06002CD7 RID: 11479 RVA: 0x000DFEFC File Offset: 0x000DE0FC
	private void MouseOutHeroPower(UIEvent e)
	{
		this.m_isMouseOverHeroPower = false;
		if (this.m_heroPowerBigCard != null)
		{
			iTween.Stop(this.m_heroPowerBigCard.gameObject);
			this.m_heroPowerBigCard.Hide();
		}
		if (this.m_goldenHeroPowerBigCard != null)
		{
			iTween.Stop(this.m_goldenHeroPowerBigCard.gameObject);
			this.m_goldenHeroPowerBigCard.Hide();
		}
	}

	// Token: 0x06002CD8 RID: 11480 RVA: 0x000DFF68 File Offset: 0x000DE168
	private void LoadHeroPowerCallback(string actorName, GameObject actorObject, object callbackData)
	{
		if (actorObject == null)
		{
			Debug.LogWarning(string.Format("DeckPickerTrayDisplay.LoadHeroPowerCallback() - FAILED to load actor \"{0}\"", actorName));
			return;
		}
		Actor component = actorObject.GetComponent<Actor>();
		if (component == null)
		{
			Debug.LogWarning(string.Format("DeckPickerTrayDisplay.LoadHeroPowerCallback() - ERROR actor \"{0}\" has no Actor component", actorName));
			return;
		}
		component.transform.parent = this.m_heroPower.transform;
		component.TurnOffCollider();
		SceneUtils.SetLayer(component.gameObject, this.m_heroPower.gameObject.layer);
		UberText powersText = component.GetPowersText();
		if (powersText != null)
		{
			TransformUtil.SetLocalPosY(powersText.gameObject, powersText.transform.localPosition.y + 0.1f);
		}
		this.m_heroPowerBigCard = component;
		if (this.m_isMouseOverHeroPower)
		{
			this.ShowHeroPowerBigCard();
		}
	}

	// Token: 0x06002CD9 RID: 11481 RVA: 0x000E003C File Offset: 0x000DE23C
	private void LoadGoldenHeroPowerCallback(string actorName, GameObject actorObject, object callbackData)
	{
		if (actorObject == null)
		{
			Debug.LogWarning(string.Format("DeckPickerTrayDisplay.LoadHeroPowerCallback() - FAILED to load actor \"{0}\"", actorName));
			return;
		}
		Actor component = actorObject.GetComponent<Actor>();
		if (component == null)
		{
			Debug.LogWarning(string.Format("DeckPickerTrayDisplay.LoadHeroPowerCallback() - ERROR actor \"{0}\" has no Actor component", actorName));
			return;
		}
		component.transform.parent = this.m_heroPower.transform;
		component.TurnOffCollider();
		SceneUtils.SetLayer(component.gameObject, this.m_heroPower.gameObject.layer);
		UberText powersText = component.GetPowersText();
		if (powersText != null)
		{
			TransformUtil.SetLocalPosY(powersText.gameObject, powersText.transform.localPosition.y + 0.1f);
		}
		this.m_goldenHeroPowerBigCard = component;
		if (this.m_isMouseOverHeroPower)
		{
			this.ShowGoldenHeroPowerBigCard();
		}
	}

	// Token: 0x06002CDA RID: 11482 RVA: 0x000E0110 File Offset: 0x000DE310
	private void ShowHeroPowerBigCard()
	{
		FullDef selectedHeroPowerFullDef = this.m_selectedHeroPowerFullDef;
		if (selectedHeroPowerFullDef == null)
		{
			return;
		}
		CardDef cardDef = selectedHeroPowerFullDef.GetCardDef();
		if (cardDef == null)
		{
			return;
		}
		this.m_heroPowerBigCard.SetCardDef(cardDef);
		this.m_heroPowerBigCard.SetEntityDef(selectedHeroPowerFullDef.GetEntityDef());
		this.m_heroPowerBigCard.UpdateAllComponents();
		this.m_heroPowerBigCard.Show();
		if (this.m_goldenHeroPowerBigCard != null)
		{
			this.m_goldenHeroPowerBigCard.Hide();
		}
		this.UpdateCustomHeroPowerBigCard(this.m_heroPowerBigCard.gameObject);
		float num = 1f;
		float num2 = 1.5f;
		Vector3 vector = (!UniversalInputManager.Get().IsTouchMode()) ? new Vector3(0.019f, 0.54f, -1.12f) : new Vector3(0.019f, 0.54f, 3f);
		GameObject gameObject = this.m_heroPowerBigCard.gameObject;
		Vector3 vector2 = (!UniversalInputManager.Get().IsTouchMode() || UniversalInputManager.UsePhoneUI) ? new Vector3(0.1f, 0.1f, 0.1f) : new Vector3(0f, 0.1f, 0.1f);
		if (UniversalInputManager.UsePhoneUI)
		{
			gameObject.transform.localPosition = new Vector3(-11.4f, 0.6f, -0.14f);
			gameObject.transform.localScale = Vector3.one * 3.2f;
			Vector3 vector3 = TransformUtil.ComputeWorldScale(gameObject.transform.parent);
			Vector3 driftOffset = Vector3.Scale(vector2 * 2f, vector3);
			AnimationUtil.GrowThenDrift(gameObject, this.m_HeroPower_Bone.transform.position, driftOffset);
		}
		else
		{
			gameObject.transform.localPosition = vector;
			gameObject.transform.localScale = Vector3.one * num;
			iTween.ScaleTo(gameObject, Vector3.one * num2, 0.15f);
			iTween.MoveTo(gameObject, iTween.Hash(new object[]
			{
				"position",
				vector + vector2,
				"isLocal",
				true,
				"time",
				10
			}));
		}
	}

	// Token: 0x06002CDB RID: 11483 RVA: 0x000E0360 File Offset: 0x000DE560
	private void UpdateCustomHeroPowerBigCard(GameObject heroPowerBigCard)
	{
		CardDef cardDef = this.m_heroActor.GetCardDef();
		if (cardDef == null)
		{
			Debug.LogWarning("DeckPickerTrayDisplay.UpdateCustomHeroPowerBigCard heroCardDef = null!");
			return;
		}
		Actor componentInChildren = heroPowerBigCard.GetComponentInChildren<Actor>();
		TAG_CARD_SET cardSet = this.m_heroActor.GetEntityDef().GetCardSet();
		if (cardSet == TAG_CARD_SET.HERO_SKINS)
		{
			componentInChildren.GetCardDef().m_AlwaysRenderPremiumPortrait = true;
		}
		else
		{
			componentInChildren.GetCardDef().m_AlwaysRenderPremiumPortrait = false;
		}
		componentInChildren.UpdateMaterials();
	}

	// Token: 0x06002CDC RID: 11484 RVA: 0x000E03D4 File Offset: 0x000DE5D4
	private void ShowGoldenHeroPowerBigCard()
	{
		FullDef selectedHeroPowerFullDef = this.m_selectedHeroPowerFullDef;
		if (selectedHeroPowerFullDef == null)
		{
			return;
		}
		CardDef cardDef = selectedHeroPowerFullDef.GetCardDef();
		if (cardDef == null)
		{
			return;
		}
		this.m_goldenHeroPowerBigCard.SetCardDef(cardDef);
		this.m_goldenHeroPowerBigCard.SetEntityDef(selectedHeroPowerFullDef.GetEntityDef());
		this.m_goldenHeroPowerBigCard.SetPremium(TAG_PREMIUM.GOLDEN);
		this.m_goldenHeroPowerBigCard.UpdateAllComponents();
		this.m_goldenHeroPowerBigCard.Show();
		if (this.m_heroPowerBigCard != null)
		{
			this.m_heroPowerBigCard.Hide();
		}
		float num = 1f;
		float num2 = 1.5f;
		Vector3 vector = (!UniversalInputManager.Get().IsTouchMode()) ? new Vector3(0.019f, 0.54f, -1.12f) : new Vector3(0.019f, 0.54f, 3f);
		GameObject gameObject = this.m_goldenHeroPowerBigCard.gameObject;
		gameObject.transform.localPosition = vector;
		this.m_goldenHeroPowerBigCard.transform.localScale = new Vector3(num, num, num);
		iTween.ScaleTo(gameObject, new Vector3(num2, num2, num2), 0.15f);
		Vector3 vector2 = (!UniversalInputManager.Get().IsTouchMode()) ? new Vector3(0.1f, 0.1f, 0.1f) : new Vector3(0f, 0.1f, 0.1f);
		iTween.MoveTo(gameObject, iTween.Hash(new object[]
		{
			"position",
			vector + vector2,
			"isLocal",
			true,
			"time",
			10
		}));
	}

	// Token: 0x06002CDD RID: 11485 RVA: 0x000E057C File Offset: 0x000DE77C
	private void UpdateTrayTransitionValues(bool isWild, bool showVineGlowBurst = true)
	{
		this.m_isUsingWildVisuals = isWild;
		float num = (!isWild) ? 0f : 1f;
		if (UniversalInputManager.UsePhoneUI && this.m_slidingTray.IsShown())
		{
			this.m_detailsTrayFrame.GetComponentInChildren<Renderer>().material.SetFloat("_Transistion", num);
		}
		if (showVineGlowBurst)
		{
			if (this.m_customPages != null)
			{
				if (this.m_customPages.Length > 1 && this.m_showingSecondPage)
				{
					this.m_customPages[1].PlayVineGlowBurst(!isWild);
				}
				else if (this.m_customPages.Length > 0)
				{
					this.m_customPages[0].PlayVineGlowBurst(!isWild);
				}
			}
			if (this.m_vineGlowBurst != null)
			{
				bool active = !this.ShouldShowCustomDecks();
				foreach (GameObject gameObject in this.m_premadeDeckGlowBurstObjects)
				{
					gameObject.SetActive(active);
				}
				string text = (!isWild) ? "GlowVines_Premade" : "GlowVines_PremadeNoFX";
				this.m_vineGlowBurst.SendEvent(text);
			}
			string text2 = (!isWild) ? this.m_standardTransitionSound : this.m_wildTransitionSound;
			if (!string.IsNullOrEmpty(text2))
			{
				string soundName = FileUtils.GameAssetPathToName(text2);
				SoundManager.Get().LoadAndPlay(soundName);
			}
		}
		base.StopCoroutine("TransitionTrayMaterial");
		base.StartCoroutine(this.TransitionTrayMaterial(num, 2f));
	}

	// Token: 0x06002CDE RID: 11486 RVA: 0x000E0724 File Offset: 0x000DE924
	private IEnumerator TransitionTrayMaterial(float targetValue, float speed)
	{
		Material trayMat = null;
		Material randomTrayMat = null;
		float currentValue;
		if (UniversalInputManager.UsePhoneUI)
		{
			trayMat = null;
			randomTrayMat = this.m_randomTray.GetComponent<Renderer>().material;
			currentValue = randomTrayMat.GetFloat("_Transistion");
		}
		else
		{
			trayMat = this.m_trayFrame.GetComponentInChildren<Renderer>().material;
			currentValue = trayMat.GetFloat("_Transistion");
			Renderer renderer = this.m_randomTray.GetComponentInChildren<Renderer>();
			if (renderer != null)
			{
				randomTrayMat = renderer.material;
			}
		}
		do
		{
			currentValue = Mathf.MoveTowards(currentValue, targetValue, speed * Time.deltaTime);
			if (trayMat != null)
			{
				trayMat.SetFloat("_Transistion", currentValue);
			}
			if (randomTrayMat != null)
			{
				randomTrayMat.SetFloat("_Transistion", currentValue);
			}
			if (this.m_customPages != null)
			{
				foreach (CustomDeckPage page in this.m_customPages)
				{
					page.UpdateTrayTransitionValue(currentValue);
				}
			}
			yield return null;
		}
		while (currentValue != targetValue);
		yield break;
	}

	// Token: 0x06002CDF RID: 11487 RVA: 0x000E075C File Offset: 0x000DE95C
	private void SetTrayTextures(Texture standardTexture, Texture wildTexture)
	{
		Material material = this.m_trayFrame.GetComponentInChildren<Renderer>().material;
		material.mainTexture = standardTexture;
		material.SetTexture("_MainTex2", wildTexture);
		Renderer componentInChildren = this.m_randomTray.GetComponentInChildren<Renderer>();
		if (componentInChildren != null)
		{
			componentInChildren.material.mainTexture = standardTexture;
			componentInChildren.material.SetTexture("_MainTex2", wildTexture);
		}
	}

	// Token: 0x06002CE0 RID: 11488 RVA: 0x000E07C4 File Offset: 0x000DE9C4
	private void SetCustomTrayTextures(Texture standardTexture, Texture wildTexture)
	{
		if (UniversalInputManager.UsePhoneUI)
		{
			Material material = this.m_randomTray.GetComponent<Renderer>().material;
			material.mainTexture = standardTexture;
			material.SetTexture("_MainTex2", wildTexture);
		}
		if (this.m_customPages != null)
		{
			foreach (CustomDeckPage customDeckPage in this.m_customPages)
			{
				customDeckPage.SetTrayTextures(standardTexture, wildTexture);
			}
		}
	}

	// Token: 0x06002CE1 RID: 11489 RVA: 0x000E0838 File Offset: 0x000DEA38
	private void SetDetailsTrayTextures(Texture standardTexture, Texture wildTexture)
	{
		Material sharedMaterial = this.m_detailsTrayFrame.GetComponent<Renderer>().sharedMaterial;
		sharedMaterial.mainTexture = standardTexture;
		sharedMaterial.SetTexture("_MainTex2", wildTexture);
	}

	// Token: 0x06002CE2 RID: 11490 RVA: 0x000E086C File Offset: 0x000DEA6C
	private void DisableHeroButtons()
	{
		foreach (HeroPickerButton heroPickerButton in this.m_heroButtons)
		{
			heroPickerButton.SetEnabled(false);
		}
	}

	// Token: 0x06002CE3 RID: 11491 RVA: 0x000E08C8 File Offset: 0x000DEAC8
	private void EnableHeroButtons()
	{
		foreach (HeroPickerButton heroPickerButton in this.m_heroButtons)
		{
			heroPickerButton.SetEnabled(true);
		}
	}

	// Token: 0x06002CE4 RID: 11492 RVA: 0x000E0924 File Offset: 0x000DEB24
	private HeroPickerButton GetPreconButtonForClass(TAG_CLASS classType)
	{
		foreach (HeroPickerButton heroPickerButton in this.m_heroButtons)
		{
			TAG_CLASS @class = heroPickerButton.GetFullDef().GetEntityDef().GetClass();
			if (@class == classType)
			{
				return heroPickerButton;
			}
		}
		return null;
	}

	// Token: 0x06002CE5 RID: 11493 RVA: 0x000E099C File Offset: 0x000DEB9C
	private void RankedPlayButtonsLoaded(string name, GameObject go, object callbackData)
	{
		this.m_rankedPlayButtons = go.GetComponent<RankedPlayDisplay>();
		this.m_rankedPlayButtons.transform.parent = this.m_hierarchyDetails.transform;
		this.m_rankedPlayButtons.transform.localScale = this.m_rankedPlayButtonsBone.localScale;
		this.m_rankedPlayButtons.transform.localPosition = this.m_rankedPlayButtonsBone.localPosition;
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_rankedPlayButtons.SetRankedMedalTransform(this.m_medalBone_phone);
		}
		this.m_rankedPlayButtons.UpdateMode();
		base.StartCoroutine(this.SetRankedMedalWhenReady());
	}

	// Token: 0x06002CE6 RID: 11494 RVA: 0x000E0A40 File Offset: 0x000DEC40
	private IEnumerator SetRankedMedalWhenReady()
	{
		while (TournamentDisplay.Get().GetCurrentMedalInfo() == null)
		{
			yield return null;
		}
		this.OnMedalChanged(TournamentDisplay.Get().GetCurrentMedalInfo());
		TournamentDisplay.Get().RegisterMedalChangedListener(new TournamentDisplay.DelMedalChanged(this.OnMedalChanged));
		yield break;
	}

	// Token: 0x06002CE7 RID: 11495 RVA: 0x000E0A5B File Offset: 0x000DEC5B
	private void OnMedalChanged(NetCache.NetCacheMedalInfo medalInfo)
	{
		this.m_rankedPlayButtons.SetRankedMedal(medalInfo);
	}

	// Token: 0x06002CE8 RID: 11496 RVA: 0x000E0A6C File Offset: 0x000DEC6C
	private void PlayGameButtonRelease(UIEvent e)
	{
		this.HideDemoQuotes();
		this.HideSetRotationNotifications();
		this.m_playButton.SetEnabled(false);
		this.DisableHeroButtons();
		this.m_heroChosen = true;
		this.PlayGame();
	}

	// Token: 0x06002CE9 RID: 11497 RVA: 0x000E0AA4 File Offset: 0x000DECA4
	private void EnableBackButton(bool enable)
	{
		if (DemoMgr.Get().IsExpoDemo())
		{
			if (enable)
			{
				return;
			}
			enable = false;
		}
		this.m_backButton.SetEnabled(enable);
		this.m_backButton.Flip(enable);
	}

	// Token: 0x06002CEA RID: 11498 RVA: 0x000E0AD8 File Offset: 0x000DECD8
	private void EnablePlayButton(bool enable)
	{
		if (enable)
		{
			SceneMgr.Mode mode = SceneMgr.Get().GetMode();
			if (mode == SceneMgr.Mode.FRIENDLY && !FriendChallengeMgr.Get().HasChallenge())
			{
				return;
			}
			this.m_playButton.Enable();
		}
		else
		{
			this.m_playButton.Disable();
		}
	}

	// Token: 0x06002CEB RID: 11499 RVA: 0x000E0B28 File Offset: 0x000DED28
	private void EnableCollectionButton(bool enable)
	{
		this.m_collectionButton.SetEnabled(enable);
		this.m_collectionButton.Flip(enable);
		this.UpdateCollectionButtonGlow();
	}

	// Token: 0x06002CEC RID: 11500 RVA: 0x000E0B48 File Offset: 0x000DED48
	private void UpdateCollectionButtonGlow()
	{
		if (this.ShouldGlowCollectionButton())
		{
			this.m_collectionButtonGlow.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
		}
		else
		{
			this.m_collectionButtonGlow.ChangeState(ActorStateType.HIGHLIGHT_OFF);
		}
	}

	// Token: 0x06002CED RID: 11501 RVA: 0x000E0B84 File Offset: 0x000DED84
	private void ShowHero()
	{
		if (this.IsShowingCustomDecks())
		{
			this.UpdateHeroInfo(this.m_selectedCustomDeckBox);
		}
		else
		{
			this.UpdateHeroInfo(this.m_selectedHeroButton);
		}
		this.m_heroActor.Show();
		if (this.ShouldShowHeroPower())
		{
			TAG_PREMIUM premium = this.m_heroActor.GetPremium();
			if (premium == TAG_PREMIUM.GOLDEN)
			{
				this.m_heroPowerActor.Hide();
				this.m_goldenHeroPowerActor.Show();
				this.m_goldenHeroPower.GetComponent<Collider>().enabled = true;
			}
			else
			{
				this.m_goldenHeroPowerActor.Hide();
				this.m_heroPowerActor.Show();
				this.m_heroPower.GetComponent<Collider>().enabled = true;
			}
		}
		if (this.m_selectedHeroName == null)
		{
			this.m_heroName.Text = string.Empty;
		}
	}

	// Token: 0x06002CEE RID: 11502 RVA: 0x000E0C50 File Offset: 0x000DEE50
	private void ShowPreconHero(bool show)
	{
		if (show && SceneMgr.Get().GetMode() == SceneMgr.Mode.ADVENTURE && AdventureConfig.Get().GetCurrentSubScene() == AdventureSubScenes.Practice && PracticePickerTrayDisplay.Get() != null && PracticePickerTrayDisplay.Get().IsShown())
		{
			return;
		}
		if (show)
		{
			this.ShowHero();
		}
		else
		{
			if (this.m_heroActor)
			{
				this.m_heroActor.Hide();
			}
			if (this.m_heroPowerActor)
			{
				this.m_heroPowerActor.Hide();
			}
			if (this.m_goldenHeroPowerActor)
			{
				this.m_goldenHeroPowerActor.Hide();
			}
			if (this.m_heroPower)
			{
				this.m_heroPower.GetComponent<Collider>().enabled = false;
			}
			if (this.m_goldenHeroPower)
			{
				this.m_goldenHeroPower.GetComponent<Collider>().enabled = false;
			}
			this.m_heroName.Text = string.Empty;
		}
	}

	// Token: 0x06002CEF RID: 11503 RVA: 0x000E0D58 File Offset: 0x000DEF58
	private void RaiseHero()
	{
		this.m_xpBar.SetEnabled(true);
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			this.m_Hero_Bone.localPosition,
			"time",
			0.25f,
			"easeType",
			iTween.EaseType.easeOutExpo,
			"islocal",
			true
		});
		iTween.MoveTo(this.m_heroActor.gameObject, args);
		Hashtable args2 = iTween.Hash(new object[]
		{
			"position",
			this.m_HeroPower_Bone.localPosition,
			"time",
			0.25f,
			"easeType",
			iTween.EaseType.easeOutExpo,
			"islocal",
			true
		});
		if (this.ShouldShowHeroPower())
		{
			iTween.MoveTo(this.m_heroPowerActor.gameObject, args2);
			this.m_heroPower.GetComponent<Collider>().enabled = true;
			if (this.m_goldenHeroPowerActor == null)
			{
				return;
			}
			Hashtable args3 = iTween.Hash(new object[]
			{
				"position",
				this.m_HeroPower_Bone.localPosition,
				"time",
				0.25f,
				"easeType",
				iTween.EaseType.easeOutExpo,
				"islocal",
				true
			});
			iTween.MoveTo(this.m_goldenHeroPowerActor.gameObject, args3);
			this.m_goldenHeroPower.GetComponent<Collider>().enabled = true;
		}
	}

	// Token: 0x06002CF0 RID: 11504 RVA: 0x000E0F00 File Offset: 0x000DF100
	private void LowerHero()
	{
		this.m_xpBar.SetEnabled(false);
		if (!this.m_heroActor.gameObject.activeInHierarchy)
		{
			return;
		}
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			this.m_Hero_BoneDown.localPosition,
			"time",
			0.25f,
			"easeType",
			iTween.EaseType.easeOutExpo,
			"islocal",
			true
		});
		iTween.MoveTo(this.m_heroActor.gameObject, args);
		Hashtable args2 = iTween.Hash(new object[]
		{
			"position",
			this.m_HeroPower_BoneDown.localPosition,
			"time",
			0.25f,
			"easeType",
			iTween.EaseType.easeOutExpo,
			"islocal",
			true
		});
		if (this.ShouldShowHeroPower())
		{
			iTween.MoveTo(this.m_heroPowerActor.gameObject, args2);
			this.m_heroPower.GetComponent<Collider>().enabled = false;
			if (this.m_goldenHeroPowerActor == null)
			{
				return;
			}
			Hashtable args3 = iTween.Hash(new object[]
			{
				"position",
				this.m_HeroPower_BoneDown.localPosition,
				"time",
				0.25f,
				"easeType",
				iTween.EaseType.easeOutExpo,
				"islocal",
				true
			});
			iTween.MoveTo(this.m_goldenHeroPowerActor.gameObject, args3);
			this.m_goldenHeroPower.GetComponent<Collider>().enabled = false;
		}
	}

	// Token: 0x06002CF1 RID: 11505 RVA: 0x000E10C0 File Offset: 0x000DF2C0
	private void HideAllPreconHighlights()
	{
		foreach (HeroPickerButton heroPickerButton in this.m_heroButtons)
		{
			heroPickerButton.SetHighlightState(ActorStateType.HIGHLIGHT_OFF);
		}
	}

	// Token: 0x06002CF2 RID: 11506 RVA: 0x000E111C File Offset: 0x000DF31C
	private void ShowPreconHighlights()
	{
		if (!DeckPickerTrayDisplay.HighlightSelectedDeck)
		{
			return;
		}
		foreach (HeroPickerButton heroPickerButton in this.m_heroButtons)
		{
			if (heroPickerButton == this.m_selectedHeroButton)
			{
				heroPickerButton.SetHighlightState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
			}
		}
	}

	// Token: 0x06002CF3 RID: 11507 RVA: 0x000E1198 File Offset: 0x000DF398
	private void PlayGame()
	{
		if (UniversalInputManager.UsePhoneUI && (SceneMgr.Get().GetMode() != SceneMgr.Mode.ADVENTURE || AdventureConfig.Get().GetCurrentSubScene() != AdventureSubScenes.Practice))
		{
			this.m_slidingTray.ToggleTraySlider(false, null, true);
		}
		switch (SceneMgr.Get().GetMode())
		{
		case SceneMgr.Mode.COLLECTIONMANAGER:
			this.SelectHeroForCollectionManager();
			break;
		case SceneMgr.Mode.TOURNAMENT:
		{
			long selectedDeckID = this.GetSelectedDeckID();
			if (selectedDeckID == 0L)
			{
				Debug.LogError("Trying to play game with deck ID 0!");
				return;
			}
			this.EnableBackButton(false);
			GameType type;
			if (Options.Get().GetBool(Option.IN_RANKED_PLAY_MODE))
			{
				type = 7;
			}
			else
			{
				type = 8;
			}
			GameMgr.Get().FindGame(type, 2, selectedDeckID, 0L);
			PresenceMgr.Get().SetStatus(new Enum[]
			{
				PresenceStatus.PLAY_QUEUE
			});
			break;
		}
		case SceneMgr.Mode.FRIENDLY:
		{
			long selectedDeckID2 = this.GetSelectedDeckID();
			if (selectedDeckID2 == 0L)
			{
				Debug.LogError("Trying to play friendly game with deck ID 0!");
				return;
			}
			FriendChallengeMgr.Get().SelectDeck(selectedDeckID2);
			FriendlyChallengeHelper.Get().StartChallengeOrWaitForOpponent("GLOBAL_FRIEND_CHALLENGE_OPPONENT_WAITING_DECK", new AlertPopup.ResponseCallback(this.OnFriendChallengeWaitingForOpponentDialogResponse));
			break;
		}
		case SceneMgr.Mode.ADVENTURE:
		{
			long selectedDeckID3 = this.GetSelectedDeckID();
			AdventureConfig adventureConfig = AdventureConfig.Get();
			if (adventureConfig.GetMission() == ScenarioDbId.NAXX_ANUBREKHAN && !Options.Get().GetBool(Option.HAS_PLAYED_NAXX))
			{
				AdTrackingManager.Get().TrackAdventureProgress(Option.HAS_PLAYED_NAXX.ToString());
				Options.Get().SetBool(Option.HAS_PLAYED_NAXX, true);
			}
			AdventureSubScenes currentSubScene = adventureConfig.GetCurrentSubScene();
			if (currentSubScene != AdventureSubScenes.Practice)
			{
				if (currentSubScene == AdventureSubScenes.MissionDeckPicker)
				{
					if (DemoMgr.Get().GetMode() != DemoMode.BLIZZCON_2015)
					{
						adventureConfig.ChangeToLastSubScene(false);
					}
					GameMgr.Get().FindGame(1, (int)adventureConfig.GetMission(), selectedDeckID3, 0L);
				}
			}
			else
			{
				PracticePickerTrayDisplay.Get().Show();
				this.LowerHero();
			}
			break;
		}
		case SceneMgr.Mode.TAVERN_BRAWL:
			if (TavernBrawlManager.Get().SelectHeroBeforeMission())
			{
				long selectedDeckID4 = this.GetSelectedDeckID();
				if (selectedDeckID4 == 0L)
				{
					Debug.LogError("Trying to play Tavern Brawl game with deck ID 0!");
					return;
				}
				if (FriendChallengeMgr.Get().IsChallengeTavernBrawl())
				{
					FriendChallengeMgr.Get().SelectDeck(selectedDeckID4);
					FriendlyChallengeHelper.Get().StartChallengeOrWaitForOpponent("GLOBAL_FRIEND_CHALLENGE_TAVERN_BRAWL_OPPONENT_WAITING_READY", new AlertPopup.ResponseCallback(this.OnFriendChallengeWaitingForOpponentDialogResponse));
				}
				else
				{
					TavernBrawlManager.Get().StartGame(selectedDeckID4);
				}
			}
			else
			{
				this.SelectHeroForCollectionManager();
			}
			break;
		}
	}

	// Token: 0x06002CF4 RID: 11508 RVA: 0x000E1428 File Offset: 0x000DF628
	private void SelectHeroForCollectionManager()
	{
		this.m_backButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.BackButtonPress));
		Navigation.PopUnique(new Navigation.NavigateBackHandler(DeckPickerTrayDisplay.OnNavigateBack));
		if (DeckPickerTrayDisplay.s_selectHeroCoroutine != null)
		{
			SceneMgr.Get().StopCoroutine(DeckPickerTrayDisplay.s_selectHeroCoroutine);
		}
		DeckPickerTrayDisplay.s_selectHeroCoroutine = SceneMgr.Get().StartCoroutine(DeckPickerTrayDisplay.SelectHeroForCollectionManagerImpl(this.m_selectedHeroButton.GetFullDef()));
	}

	// Token: 0x06002CF5 RID: 11509 RVA: 0x000E1498 File Offset: 0x000DF698
	private static IEnumerator SelectHeroForCollectionManagerImpl(FullDef heroDef)
	{
		HeroPickerDisplay.Get().HideTray((!UniversalInputManager.UsePhoneUI) ? 0f : 0.25f);
		CollectionDeckTray deckTray = CollectionDeckTray.Get();
		DeckTrayDeckListContent decksContent = deckTray.GetDecksContent();
		if (SceneMgr.Get().GetMode() == SceneMgr.Mode.TAVERN_BRAWL)
		{
			decksContent.CreateNewDeckFromUserSelection(heroDef.GetEntityDef().GetClass(), heroDef.GetEntityDef().GetCardId(), null);
			CollectionManagerDisplay.Get().EnableInput(true);
			yield break;
		}
		DeckTemplatePicker deckTemplatePicker = (!UniversalInputManager.UsePhoneUI) ? CollectionManagerDisplay.Get().m_pageManager.GetDeckTemplatePicker() : CollectionManagerDisplay.Get().GetPhoneDeckTemplateTray();
		if (UniversalInputManager.UsePhoneUI)
		{
			deckTemplatePicker.m_phoneBackButton.SetEnabled(false);
		}
		deckTray.m_doneButton.SetEnabled(false);
		while (deckTray.IsUpdatingTrayMode() || decksContent.NumDecksToDelete() > 0 || deckTray.IsWaitingToDeleteDeck())
		{
			yield return null;
		}
		decksContent.CreateNewDeckFromUserSelection(heroDef.GetEntityDef().GetClass(), heroDef.GetEntityDef().GetCardId(), null);
		while (deckTemplatePicker != null && !deckTemplatePicker.IsShowingPacks())
		{
			yield return null;
		}
		CollectionManagerDisplay.Get().EnableInput(true);
		while (deckTray.IsUpdatingTrayMode())
		{
			yield return null;
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			deckTemplatePicker.m_phoneBackButton.SetEnabled(true);
		}
		deckTray.m_doneButton.SetEnabled(true);
		yield break;
	}

	// Token: 0x06002CF6 RID: 11510 RVA: 0x000E14BC File Offset: 0x000DF6BC
	private void OnSlidingTrayToggled(bool isShowing)
	{
		if (!isShowing && PracticePickerTrayDisplay.Get() != null && PracticePickerTrayDisplay.Get().IsShown())
		{
			Navigation.GoBack();
		}
		else if (isShowing)
		{
			this.UpdateTrayTransitionValues(this.m_isUsingWildVisuals, false);
		}
	}

	// Token: 0x06002CF7 RID: 11511 RVA: 0x000E150C File Offset: 0x000DF70C
	private IEnumerator InitModeWhenReady()
	{
		while (this.m_heroDefsLoading > 0 || this.m_heroPowerDefsLoading > 0 || this.m_heroActor == null || (this.ShouldShowPreconDecks() && !this.m_buttonAchievementsInitialized) || (this.ShouldShowCustomDecks() && !this.CustomPagesReady()) || ((this.m_heroPowerActor == null || this.m_goldenHeroPowerActor == null) && this.ShouldShowHeroPower()) || (SceneMgr.Get().GetMode() == SceneMgr.Mode.TOURNAMENT && this.m_rankedPlayButtons == null))
		{
			yield return null;
		}
		if (!this.IsChoosingHero())
		{
			while (!NetCache.Get().IsNetObjectReady<NetCache.NetCacheDecks>())
			{
				yield return null;
			}
		}
		this.m_Loaded = true;
		this.InitMode();
		PlayGameScene scene = SceneMgr.Get().GetScene() as PlayGameScene;
		if (scene != null)
		{
			scene.OnDeckPickerLoaded();
		}
		this.FireDeckTrayLoadedEvent();
		if (SceneMgr.Get().GetMode() == SceneMgr.Mode.FRIENDLY || TavernBrawlManager.IsInTavernBrawlFriendlyChallenge())
		{
			if (!FriendChallengeMgr.Get().HasChallenge())
			{
				this.GoBackUntilOnNavigateBackCalled();
				yield break;
			}
			FriendChallengeMgr.Get().AddChangedListener(new FriendChallengeMgr.ChangedCallback(this.OnFriendChallengeChanged));
		}
		if (this.m_needUnlockAllHeroesTransition)
		{
			this.m_customDeckPageContainers[0].gameObject.SetActive(false);
			base.StartCoroutine(this.ShowUnlockAllHeroesCelebration());
		}
		yield break;
	}

	// Token: 0x06002CF8 RID: 11512 RVA: 0x000E1528 File Offset: 0x000DF728
	private bool CustomPagesReady()
	{
		foreach (CustomDeckPage customDeckPage in this.m_customPages)
		{
			if (!customDeckPage.PageReady())
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002CF9 RID: 11513 RVA: 0x000E1564 File Offset: 0x000DF764
	private void InitRichPresence()
	{
		PresenceStatus? presenceStatus = default(PresenceStatus?);
		switch (SceneMgr.Get().GetMode())
		{
		case SceneMgr.Mode.TOURNAMENT:
			presenceStatus = new PresenceStatus?(PresenceStatus.PLAY_DECKPICKER);
			break;
		case SceneMgr.Mode.FRIENDLY:
			presenceStatus = new PresenceStatus?(PresenceStatus.FRIENDLY_DECKPICKER);
			if (FriendChallengeMgr.Get().IsChallengeTavernBrawl())
			{
				presenceStatus = new PresenceStatus?(PresenceStatus.TAVERN_BRAWL_FRIENDLY_WAITING);
			}
			break;
		case SceneMgr.Mode.ADVENTURE:
		{
			AdventureSubScenes currentSubScene = AdventureConfig.Get().GetCurrentSubScene();
			if (currentSubScene == AdventureSubScenes.Practice)
			{
				presenceStatus = new PresenceStatus?(PresenceStatus.PRACTICE_DECKPICKER);
			}
			break;
		}
		case SceneMgr.Mode.TAVERN_BRAWL:
			if (FriendChallengeMgr.Get().IsChallengeTavernBrawl())
			{
				presenceStatus = new PresenceStatus?(PresenceStatus.TAVERN_BRAWL_FRIENDLY_WAITING);
			}
			break;
		}
		if (presenceStatus != null)
		{
			PresenceMgr.Get().SetStatus(new Enum[]
			{
				presenceStatus.Value
			});
		}
	}

	// Token: 0x06002CFA RID: 11514 RVA: 0x000E1658 File Offset: 0x000DF858
	private void SetSelectionAndPageFromOptions()
	{
		bool flag = UniversalInputManager.UsePhoneUI && SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.GAMEPLAY;
		if (this.ShouldShowCustomDecks())
		{
			int num = 0;
			CollectionDeckBoxVisual deckboxWithDeckID = this.GetDeckboxWithDeckID(Options.Get().GetLong(Option.LAST_CUSTOM_DECK_CHOSEN), out num);
			if (this.m_needUnlockAllHeroesTransition || num <= 0)
			{
				this.ShowFirstPage();
			}
			else
			{
				this.ShowSecondPage();
			}
			if (deckboxWithDeckID != null && !flag)
			{
				this.SelectCustomDeck(deckboxWithDeckID, true);
			}
		}
		else
		{
			this.ShowFirstPage();
			HeroPickerButton preconButtonForClass = this.GetPreconButtonForClass((TAG_CLASS)Options.Get().GetInt(Option.LAST_PRECON_HERO_CHOSEN));
			if (preconButtonForClass != null && !flag)
			{
				this.SelectHero(preconButtonForClass, true);
			}
		}
	}

	// Token: 0x06002CFB RID: 11515 RVA: 0x000E1720 File Offset: 0x000DF920
	private CollectionDeckBoxVisual GetDeckboxWithDeckID(long deckID, out int pageNum)
	{
		for (pageNum = 0; pageNum < this.m_customPages.Length; pageNum++)
		{
			CustomDeckPage customDeckPage = this.m_customPages[pageNum];
			CollectionDeckBoxVisual deckboxWithDeckID = customDeckPage.GetDeckboxWithDeckID(deckID);
			if (deckboxWithDeckID != null)
			{
				return deckboxWithDeckID;
			}
		}
		pageNum = 0;
		return null;
	}

	// Token: 0x06002CFC RID: 11516 RVA: 0x000E1770 File Offset: 0x000DF970
	private void OnFriendChallengeWaitingForOpponentDialogResponse(AlertPopup.Response response, object userData)
	{
		if (response != AlertPopup.Response.CANCEL)
		{
			return;
		}
		this.EnableHeroButtons();
		this.Deselect();
		FriendChallengeMgr.Get().DeselectDeck();
		FriendlyChallengeHelper.Get().StopWaitingForFriendChallenge();
	}

	// Token: 0x06002CFD RID: 11517 RVA: 0x000E17A8 File Offset: 0x000DF9A8
	private void OnFriendChallengeChanged(FriendChallengeEvent challengeEvent, BnetPlayer player, object userData)
	{
		if (challengeEvent == FriendChallengeEvent.SELECTED_DECK)
		{
			if (SceneMgr.Get().GetMode() != SceneMgr.Mode.TAVERN_BRAWL)
			{
				if (player == BnetPresenceMgr.Get().GetMyPlayer())
				{
					return;
				}
				if (!FriendChallengeMgr.Get().DidISelectDeck())
				{
					return;
				}
				FriendlyChallengeHelper.Get().HideFriendChallengeWaitingForOpponentDialog();
				FriendlyChallengeHelper.Get().WaitForFriendChallengeToStart();
			}
		}
		else if (challengeEvent == FriendChallengeEvent.OPPONENT_CANCELED_CHALLENGE || challengeEvent == FriendChallengeEvent.OPPONENT_REMOVED_FROM_FRIENDS)
		{
			FriendlyChallengeHelper.Get().StopWaitingForFriendChallenge();
			this.GoBackUntilOnNavigateBackCalled();
		}
	}

	// Token: 0x06002CFE RID: 11518 RVA: 0x000E1828 File Offset: 0x000DFA28
	private void GoBackUntilOnNavigateBackCalled()
	{
		while (Navigation.BackStackContainsHandler(new Navigation.NavigateBackHandler(DeckPickerTrayDisplay.OnNavigateBack)))
		{
			if (!Navigation.GoBack())
			{
				break;
			}
		}
	}

	// Token: 0x06002CFF RID: 11519 RVA: 0x000E1864 File Offset: 0x000DFA64
	private void OnHeroFullDefLoaded(string cardId, FullDef fullDef, object userData)
	{
		EntityDef entityDef = fullDef.GetEntityDef();
		DeckPickerTrayDisplay.HeroFullDefLoadedCallbackData heroFullDefLoadedCallbackData = userData as DeckPickerTrayDisplay.HeroFullDefLoadedCallbackData;
		TAG_PREMIUM premium = (fullDef.GetEntityDef().GetCardSet() != TAG_CARD_SET.HERO_SKINS) ? CollectionManager.Get().GetBestCardPremium(cardId) : TAG_PREMIUM.GOLDEN;
		heroFullDefLoadedCallbackData.HeroPickerButton.UpdateDisplay(fullDef, premium);
		heroFullDefLoadedCallbackData.HeroPickerButton.SetOriginalLocalPosition();
		string heroPowerCardIdFromHero = GameUtils.GetHeroPowerCardIdFromHero(entityDef.GetCardId());
		DefLoader.Get().LoadFullDef(heroPowerCardIdFromHero, new DefLoader.LoadDefCallback<FullDef>(this.OnHeroPowerFullDefLoaded));
		this.m_heroDefsLoading--;
		if (this.m_heroDefsLoading > 0)
		{
			return;
		}
		if (this.ShouldShowPreconDecks())
		{
			base.StartCoroutine(this.InitButtonAchievements());
		}
	}

	// Token: 0x06002D00 RID: 11520 RVA: 0x000E1911 File Offset: 0x000DFB11
	private void OnHeroPowerFullDefLoaded(string cardId, FullDef def, object userData)
	{
		this.m_heroPowerDefs[cardId] = def;
		this.m_heroPowerDefsLoading--;
	}

	// Token: 0x06002D01 RID: 11521 RVA: 0x000E192E File Offset: 0x000DFB2E
	private void LoadHero()
	{
		AssetLoader.Get().LoadActor("Card_Play_Hero", new AssetLoader.GameObjectCallback(this.OnHeroActorLoaded), null, false);
	}

	// Token: 0x06002D02 RID: 11522 RVA: 0x000E1950 File Offset: 0x000DFB50
	private void OnHeroActorLoaded(string actorName, GameObject actorObject, object callbackData)
	{
		if (actorObject == null)
		{
			Debug.LogWarning(string.Format("DeckPickerTrayDisplay.OnHeroActorLoaded() - FAILED to load actor \"{0}\"", actorName));
			return;
		}
		this.m_heroActor = actorObject.GetComponent<Actor>();
		if (this.m_heroActor == null)
		{
			Debug.LogWarning(string.Format("DeckPickerTrayDisplay.OnHeroActorLoaded() - ERROR actor \"{0}\" has no Actor component", actorName));
			return;
		}
		actorObject.transform.parent = this.m_hierarchyDetails.transform;
		actorObject.transform.localScale = this.m_Hero_Bone.localScale;
		actorObject.transform.localPosition = this.m_Hero_Bone.localPosition;
		this.m_heroActor.SetUnlit();
		Object.Destroy(this.m_heroActor.m_healthObject);
		Object.Destroy(this.m_heroActor.m_attackObject);
		this.m_xpBar.transform.parent = this.m_heroActor.GetRootObject().transform;
		this.m_xpBar.transform.localScale = new Vector3(0.89f, 0.89f, 0.89f);
		this.m_xpBar.transform.localPosition = new Vector3(-0.1776525f, 0.2245596f, -0.7309282f);
		this.m_xpBar.m_isOnDeck = false;
		this.m_heroActor.Hide();
	}

	// Token: 0x06002D03 RID: 11523 RVA: 0x000E1A94 File Offset: 0x000DFC94
	private void LoadHeroPower()
	{
		AssetLoader.Get().LoadActor("Card_Play_HeroPower", new AssetLoader.GameObjectCallback(this.OnHeroPowerActorLoaded), null, false);
	}

	// Token: 0x06002D04 RID: 11524 RVA: 0x000E1AB4 File Offset: 0x000DFCB4
	private void OnHeroPowerActorLoaded(string actorName, GameObject actorObject, object callbackData)
	{
		if (actorObject == null)
		{
			Debug.LogWarning(string.Format("DeckPickerTrayDisplay.OnHeroPowerActorLoaded() - FAILED to load actor \"{0}\"", actorName));
			return;
		}
		this.m_heroPowerActor = actorObject.GetComponent<Actor>();
		if (this.m_heroPowerActor == null)
		{
			Debug.LogWarning(string.Format("DeckPickerTrayDisplay.OnHeroPowerActorLoaded() - ERROR actor \"{0}\" has no Actor component", actorName));
			return;
		}
		this.m_heroPower = actorObject.AddComponent<PegUIElement>();
		actorObject.AddComponent<BoxCollider>();
		actorObject.transform.parent = this.m_hierarchyDetails.transform;
		actorObject.transform.localScale = this.m_HeroPower_Bone.localScale;
		actorObject.transform.localPosition = this.m_HeroPower_Bone.localPosition;
		this.m_heroPowerActor.SetUnlit();
		this.m_heroPower.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.MouseOverHeroPower));
		this.m_heroPower.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.MouseOutHeroPower));
		this.m_heroPowerActor.Hide();
		this.m_heroPower.GetComponent<Collider>().enabled = false;
		this.m_heroName.Text = string.Empty;
		base.StartCoroutine(this.UpdateHeroSkinHeroPower());
	}

	// Token: 0x06002D05 RID: 11525 RVA: 0x000E1BD7 File Offset: 0x000DFDD7
	private void LoadGoldenHeroPower()
	{
		AssetLoader.Get().LoadActor(ActorNames.GetNameWithPremiumType("Card_Play_HeroPower", TAG_PREMIUM.GOLDEN), new AssetLoader.GameObjectCallback(this.OnGoldenHeroPowerActorLoaded), null, false);
	}

	// Token: 0x06002D06 RID: 11526 RVA: 0x000E1C00 File Offset: 0x000DFE00
	private void OnGoldenHeroPowerActorLoaded(string actorName, GameObject actorObject, object callbackData)
	{
		if (actorObject == null)
		{
			Debug.LogWarning(string.Format("DeckPickerTrayDisplay.OnHeroPowerActorLoaded() - FAILED to load actor \"{0}\"", actorName));
			return;
		}
		this.m_goldenHeroPowerActor = actorObject.GetComponent<Actor>();
		if (this.m_goldenHeroPowerActor == null)
		{
			Debug.LogWarning(string.Format("DeckPickerTrayDisplay.OnHeroPowerActorLoaded() - ERROR actor \"{0}\" has no Actor component", actorName));
			return;
		}
		this.m_goldenHeroPower = actorObject.AddComponent<PegUIElement>();
		actorObject.AddComponent<BoxCollider>();
		actorObject.transform.parent = this.m_hierarchyDetails.transform;
		actorObject.transform.localScale = this.m_HeroPower_Bone.localScale;
		actorObject.transform.localPosition = this.m_HeroPower_Bone.localPosition;
		this.m_goldenHeroPowerActor.SetUnlit();
		this.m_goldenHeroPowerActor.SetPremium(TAG_PREMIUM.GOLDEN);
		this.m_goldenHeroPower.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.MouseOverHeroPower));
		this.m_goldenHeroPower.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.MouseOutHeroPower));
		this.m_goldenHeroPowerActor.Hide();
		this.m_goldenHeroPower.GetComponent<Collider>().enabled = false;
		this.m_heroName.Text = string.Empty;
	}

	// Token: 0x06002D07 RID: 11527 RVA: 0x000E1D24 File Offset: 0x000DFF24
	private IEnumerator UpdateHeroSkinHeroPower()
	{
		while (this.m_heroActor == null)
		{
			yield return null;
		}
		CardDef heroCardDef = this.m_heroActor.GetCardDef();
		while (heroCardDef == null)
		{
			heroCardDef = this.m_heroActor.GetCardDef();
			yield return null;
		}
		HeroSkinHeroPower hshp = this.m_heroPowerActor.gameObject.GetComponentInChildren<HeroSkinHeroPower>();
		if (hshp == null)
		{
			yield break;
		}
		TAG_CARD_SET cardSet = this.m_heroActor.GetEntityDef().GetCardSet();
		if (cardSet == TAG_CARD_SET.HERO_SKINS)
		{
			hshp.m_Actor.GetCardDef().m_AlwaysRenderPremiumPortrait = true;
		}
		else
		{
			hshp.m_Actor.GetCardDef().m_AlwaysRenderPremiumPortrait = false;
		}
		hshp.m_Actor.UpdateMaterials();
		yield break;
	}

	// Token: 0x06002D08 RID: 11528 RVA: 0x000E1D40 File Offset: 0x000DFF40
	private void FireDeckTrayLoadedEvent()
	{
		DeckPickerTrayDisplay.DeckTrayLoaded[] array = this.m_DeckTrayLoadedListeners.ToArray();
		foreach (DeckPickerTrayDisplay.DeckTrayLoaded deckTrayLoaded in array)
		{
			deckTrayLoaded();
		}
	}

	// Token: 0x06002D09 RID: 11529 RVA: 0x000E1D79 File Offset: 0x000DFF79
	private bool ShouldShowHeroPower()
	{
		return !UniversalInputManager.UsePhoneUI || this.IsChoosingHero();
	}

	// Token: 0x06002D0A RID: 11530 RVA: 0x000E1D94 File Offset: 0x000DFF94
	private bool IsChoosingHero()
	{
		SceneMgr.Mode mode = SceneMgr.Get().GetMode();
		return mode == SceneMgr.Mode.COLLECTIONMANAGER || mode == SceneMgr.Mode.TAVERN_BRAWL || this.IsChoosingHeroForTavernBrawlChallenge();
	}

	// Token: 0x06002D0B RID: 11531 RVA: 0x000E1DC4 File Offset: 0x000DFFC4
	private bool ShouldShowCollectionButton()
	{
		if (this.IsChoosingHero())
		{
			return false;
		}
		if (SceneMgr.Get().GetMode() == SceneMgr.Mode.FRIENDLY)
		{
			return false;
		}
		if (AchieveManager.Get().HasUnlockedFeature(Achievement.UnlockableFeature.VANILLA_HEROES))
		{
			return true;
		}
		if (this.m_numCardsPerClass < 0f)
		{
			CollectionManager collectionManager = CollectionManager.Get();
			TAG_CLASS[] theseClassTypes = new TAG_CLASS[]
			{
				TAG_CLASS.MAGE,
				TAG_CLASS.DRUID,
				TAG_CLASS.HUNTER,
				TAG_CLASS.PALADIN,
				TAG_CLASS.PRIEST,
				TAG_CLASS.ROGUE,
				TAG_CLASS.SHAMAN,
				TAG_CLASS.WARLOCK,
				TAG_CLASS.WARRIOR
			};
			List<CollectibleCard> list = collectionManager.FindCards(null, default(TAG_PREMIUM?), default(int?), null, theseClassTypes, null, default(TAG_RARITY?), default(TAG_RACE?), default(bool?), new int?(1), default(bool?), default(bool?), null, null, false);
			int count = list.Count;
			int count2 = AchieveManager.Get().GetAchievesInGroup(Achievement.AchType.UNLOCK_HERO, true).Count;
			this.m_numCardsPerClass = (float)count / (float)count2;
		}
		return this.m_numCardsPerClass > 6f;
	}

	// Token: 0x06002D0C RID: 11532 RVA: 0x000E1ED0 File Offset: 0x000E00D0
	private bool IsChoosingHeroForTavernBrawlChallenge()
	{
		return SceneMgr.Get().GetMode() == SceneMgr.Mode.FRIENDLY && FriendChallengeMgr.Get().IsChallengeTavernBrawl();
	}

	// Token: 0x06002D0D RID: 11533 RVA: 0x000E1EF0 File Offset: 0x000E00F0
	private bool ShouldGlowCollectionButton()
	{
		return this.ShouldShowCollectionButton() && this.m_collectionButton.IsEnabled() && ((!Options.Get().GetBool(Option.HAS_CLICKED_COLLECTION_BUTTON_FOR_NEW_DECK) && this.HaveDecksThatNeedNames()) || (!Options.Get().GetBool(Option.HAS_CLICKED_COLLECTION_BUTTON_FOR_NEW_CARD) && this.HaveUnseenBasicCards()) || (Options.Get().GetBool(Option.GLOW_COLLECTION_BUTTON_AFTER_SET_ROTATION) && SceneMgr.Get().GetMode() == SceneMgr.Mode.TOURNAMENT) || (this.m_missingStandardDeckDisplay != null && this.m_missingStandardDeckDisplay.IsShown()));
	}

	// Token: 0x06002D0E RID: 11534 RVA: 0x000E1FA8 File Offset: 0x000E01A8
	private bool HaveDecksThatNeedNames()
	{
		List<CollectionDeck> decks = CollectionManager.Get().GetDecks(1);
		foreach (CollectionDeck collectionDeck in decks)
		{
			if (collectionDeck.NeedsName)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002D0F RID: 11535 RVA: 0x000E2018 File Offset: 0x000E0218
	private int GetNumValidStandardDecks()
	{
		int num = 0;
		List<CollectionDeck> decks = CollectionManager.Get().GetDecks(1);
		foreach (CollectionDeck collectionDeck in decks)
		{
			if (collectionDeck.IsValidForFormat(false) && collectionDeck.IsTourneyValid)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06002D10 RID: 11536 RVA: 0x000E2090 File Offset: 0x000E0290
	private bool HaveUnseenBasicCards()
	{
		List<CollectibleCard> list = CollectionManager.Get().FindCards(null, default(TAG_PREMIUM?), default(int?), new TAG_CARD_SET[]
		{
			TAG_CARD_SET.CORE
		}, null, null, default(TAG_RARITY?), default(TAG_RACE?), new bool?(false), new int?(1), new bool?(true), default(bool?), null, null, true);
		return list.Count > 0;
	}

	// Token: 0x06002D11 RID: 11537 RVA: 0x000E2104 File Offset: 0x000E0304
	private IEnumerator ShowUnlockAllHeroesCelebration()
	{
		this.m_expoClickBlocker.gameObject.SetActive(true);
		FullScreenFXMgr.Get().Vignette(1.4f, 0.5f, iTween.EaseType.easeOutCirc, null);
		yield return new WaitForSeconds(0.6f);
		NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, new Vector3(100f, NotificationManager.DEPTH, 66f), GameStrings.Get("VO_INNKEEPER_UNLOCKED_ALL_HEROES"), "VO_INNKEEPER_Male_Dwarf_UNLOCKED_HEROES_01", 0f, new Action(this.InnkeeperCelebrationFinished));
		while (!this.m_innkeeperQuoteFinished)
		{
			yield return null;
		}
		if (this.m_premadeDeckGlowAnimator != null)
		{
			SoundManager.Get().LoadAndPlay("AllNineHeroesUnlock");
			this.m_premadeDeckGlowAnimator.enabled = true;
			yield return new WaitForSeconds(2.4f);
		}
		this.m_basicDeckPageContainer.gameObject.SetActive(false);
		this.m_customDeckPageContainers[0].gameObject.SetActive(true);
		this.m_needUnlockAllHeroesTransition = false;
		Options.Get().SetBool(Option.HAS_SEEN_UNLOCK_ALL_HEROES_TRANSITION, true);
		if (this.m_premadeDeckGlowAnimator != null)
		{
			yield return new WaitForSeconds(1.3f);
		}
		FullScreenFXMgr.Get().StopVignette(0.8f, iTween.EaseType.easeInOutCubic, null);
		this.m_expoClickBlocker.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x06002D12 RID: 11538 RVA: 0x000E211F File Offset: 0x000E031F
	private void InnkeeperCelebrationFinished()
	{
		this.m_innkeeperQuoteFinished = true;
	}

	// Token: 0x06002D13 RID: 11539 RVA: 0x000E2128 File Offset: 0x000E0328
	private bool ShouldShowCustomDecks()
	{
		if (this.m_deckPickerMode == DeckPickerMode.INVALID)
		{
			Debug.LogWarning("DeckPickerTrayDisplay.ShowCustomDecks() - querying m_deckPickerMode when it hasn't been set yet!");
		}
		return this.m_deckPickerMode == DeckPickerMode.CUSTOM || this.m_needUnlockAllHeroesTransition;
	}

	// Token: 0x06002D14 RID: 11540 RVA: 0x000E2160 File Offset: 0x000E0360
	private bool ShouldShowPreconDecks()
	{
		if (this.m_deckPickerMode == DeckPickerMode.INVALID)
		{
			Debug.LogWarning("DeckPickerTrayDisplay.ShowPreconDecks() - querying m_deckPickerMode when it hasn't been set yet!");
		}
		return this.m_deckPickerMode == DeckPickerMode.PRECON || this.m_needUnlockAllHeroesTransition;
	}

	// Token: 0x06002D15 RID: 11541 RVA: 0x000E2198 File Offset: 0x000E0398
	public void InitSetRotationTutorial()
	{
		if (this.m_setRotationTutorialState != DeckPickerTrayDisplay.SetRotationTutorialState.INACTIVE)
		{
			Debug.LogError("Tried to call DeckPickerTrayDisplay.InitTutorial() when m_setRotationTutorialState was " + this.m_setRotationTutorialState.ToString());
			return;
		}
		if (!AchieveManager.Get().HasUnlockedFeature(Achievement.UnlockableFeature.VANILLA_HEROES))
		{
			Debug.LogError("Tried to call DeckPickerTrayDisplay.InitTutorial() for an account that has not unlocked all 9 heroes.");
			return;
		}
		Options.Get().SetBool(Option.IN_WILD_MODE, false);
		this.Deselect();
		this.ShowFirstPage();
		this.m_rankedPlayButtons.StartSetRotationTutorial();
		this.EnablePlayButton(false);
		this.EnableBackButton(false);
		this.EnableCollectionButton(false);
		this.m_rightArrow.gameObject.SetActive(false);
		this.m_leftArrow.gameObject.SetActive(false);
		this.m_rightArrow.SetEnabled(false);
		this.m_leftArrow.SetEnabled(false);
		this.m_switchFormatButton.SetFormat(true, false);
		this.m_switchFormatButton.Disable();
		this.m_switchFormatButton.gameObject.SetActive(false);
		this.SetHeaderText(GameStrings.Get("GLUE_TOURNAMENT"));
		if (this.m_heroPower != null)
		{
			this.m_heroPower.GetComponent<Collider>().enabled = false;
		}
		if (this.m_goldenHeroPower != null)
		{
			this.m_goldenHeroPower.GetComponent<Collider>().enabled = false;
		}
		Options.Get().SetBool(Option.IN_WILD_MODE, true);
		this.m_missingStandardDeckDisplay.Hide();
		this.UpdateTrayTransitionValues(false, false);
		foreach (CustomDeckPage customDeckPage in this.m_customPages)
		{
			customDeckPage.UpdateDeckVisuals(false, false, true);
			customDeckPage.EnableDeckButtons(false);
		}
		this.m_setRotationTutorialState = DeckPickerTrayDisplay.SetRotationTutorialState.READY;
	}

	// Token: 0x06002D16 RID: 11542 RVA: 0x000E2338 File Offset: 0x000E0538
	public void StartSetRotationTutorial()
	{
		if (this.m_setRotationTutorialState == DeckPickerTrayDisplay.SetRotationTutorialState.READY)
		{
			base.StartCoroutine(this.ShowFormatTutorialPopUp());
		}
		else
		{
			Debug.LogError("Tried to start Play Screen Set Rotation Tutorial without calling DeckPickerTrayDisplay.InitTutorial()");
		}
	}

	// Token: 0x06002D17 RID: 11543 RVA: 0x000E2370 File Offset: 0x000E0570
	private IEnumerator ShowFormatTutorialPopUp()
	{
		this.m_setRotationTutorialState = DeckPickerTrayDisplay.SetRotationTutorialState.SHOW_FORMAT_TUTORIAL_POPUP;
		this.m_switchFormatButton.gameObject.SetActive(true);
		this.m_switchFormatButton.SetFormat(true, true);
		this.m_dimQuad.GetComponent<Renderer>().enabled = true;
		this.m_dimQuad.enabled = true;
		this.m_dimQuad.StopPlayback();
		this.m_dimQuad.Play("DimQuad_FadeIn");
		yield return new WaitForEndOfFrame();
		float animDuration = this.m_dimQuad.GetCurrentAnimatorStateInfo(0).length;
		yield return new WaitForSeconds(animDuration);
		GameObject m_popUpObject = Object.Instantiate<GameObject>(this.m_formatTutorialPopUpPrefab);
		m_popUpObject.transform.parent = this.m_formatTutorialPopUpBone;
		m_popUpObject.transform.localPosition = Vector3.zero;
		m_popUpObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
		bool shouldContinue = false;
		Vector3 popUpScale = (!UniversalInputManager.UsePhoneUI) ? this.m_formatTutorialPopupScale : this.m_formatTutorialPopupScalePhone;
		Action<object> continueCallback = delegate(object o)
		{
			shouldContinue = true;
		};
		Hashtable args = iTween.Hash(new object[]
		{
			"scale",
			popUpScale,
			"time",
			0.3f,
			"easetype",
			iTween.EaseType.easeOutBack,
			"oncomplete",
			continueCallback
		});
		iTween.ScaleTo(m_popUpObject, args);
		if (!string.IsNullOrEmpty(this.m_formatPopUpShowSound))
		{
			string soundFileName = FileUtils.GameAssetPathToName(this.m_formatPopUpShowSound);
			SoundManager.Get().LoadAndPlay(soundFileName);
		}
		while (!shouldContinue)
		{
			yield return null;
		}
		shouldContinue = false;
		UIEvent.Handler continueCallback2 = delegate(UIEvent o)
		{
			shouldContinue = true;
		};
		this.m_clickCatcher.AddEventListener(UIEventType.RELEASE, continueCallback2);
		this.m_clickCatcher.gameObject.SetActive(true);
		while (!shouldContinue)
		{
			yield return null;
		}
		this.m_clickCatcher.RemoveEventListener(UIEventType.RELEASE, continueCallback2);
		this.m_clickCatcher.gameObject.SetActive(false);
		shouldContinue = false;
		args = iTween.Hash(new object[]
		{
			"scale",
			new Vector3(0.01f, 0.01f, 0.01f),
			"time",
			0.5f,
			"easetype",
			iTween.EaseType.easeInBack,
			"oncomplete",
			continueCallback
		});
		iTween.ScaleTo(m_popUpObject, args);
		if (!string.IsNullOrEmpty(this.m_formatPopUpHideSound))
		{
			string soundFileName2 = FileUtils.GameAssetPathToName(this.m_formatPopUpHideSound);
			SoundManager.Get().LoadAndPlay(soundFileName2);
		}
		while (!shouldContinue)
		{
			yield return null;
		}
		Object.Destroy(m_popUpObject);
		this.StartSwitchToStandard();
		yield break;
	}

	// Token: 0x06002D18 RID: 11544 RVA: 0x000E238B File Offset: 0x000E058B
	private void StartSwitchToStandard()
	{
		this.m_setRotationTutorialState = DeckPickerTrayDisplay.SetRotationTutorialState.SWITCH_TO_STANDARD;
		base.StartCoroutine(this.TutorialSwitchToStandard());
	}

	// Token: 0x06002D19 RID: 11545 RVA: 0x000E23A4 File Offset: 0x000E05A4
	private IEnumerator TutorialSwitchToStandard()
	{
		this.m_switchFormatPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.SET_ROTATION_INTRO, this.m_Switch_Format_Notification_Bone.position, this.m_Switch_Format_Notification_Bone.localScale, GameStrings.Get("GLUE_TOURNAMENT_SWITCH_TO_STANDARD"), true);
		if (this.m_switchFormatPopup != null)
		{
			Notification.PopUpArrowDirection arrowDirection = (!UniversalInputManager.UsePhoneUI) ? Notification.PopUpArrowDirection.Up : Notification.PopUpArrowDirection.RightUp;
			this.m_switchFormatPopup.ShowPopUpArrow(arrowDirection);
		}
		this.m_switchFormatButton.EnableHighlight(true);
		this.m_switchFormatButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnSwitchFormatReleased));
		this.m_switchFormatButton.Enable();
		yield break;
	}

	// Token: 0x06002D1A RID: 11546 RVA: 0x000E23C0 File Offset: 0x000E05C0
	private void OnSwitchFormatReleased(UIEvent e)
	{
		if (this.m_setRotationTutorialState == DeckPickerTrayDisplay.SetRotationTutorialState.SWITCH_TO_STANDARD)
		{
			this.m_switchFormatButton.Disable();
			this.m_switchFormatButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnSwitchFormatReleased));
			foreach (CustomDeckPage customDeckPage in this.m_customPages)
			{
				customDeckPage.TransitionWildDecks();
			}
			this.PlayTransitionSounds();
			this.UpdateTrayTransitionValues(false, false);
			base.StartCoroutine(this.ShowQuestLog());
		}
		else
		{
			Debug.Log("OnSwitchFormatReleased called when not in SWITCH_TO_STANDARD Set Rotation Tutorial state");
		}
	}

	// Token: 0x06002D1B RID: 11547 RVA: 0x000E244C File Offset: 0x000E064C
	private void PlayTransitionSounds()
	{
		CustomDeckPage customDeckPage = (!this.m_showingSecondPage) ? this.m_customPages[0] : this.m_customPages[1];
		if (customDeckPage.HasWildDeck() && !string.IsNullOrEmpty(this.m_wildDeckTransitionSound))
		{
			string soundName = FileUtils.GameAssetPathToName(this.m_wildDeckTransitionSound);
			SoundManager.Get().LoadAndPlay(soundName);
		}
	}

	// Token: 0x06002D1C RID: 11548 RVA: 0x000E24AC File Offset: 0x000E06AC
	private IEnumerator ShowQuestLog()
	{
		this.m_setRotationTutorialState = DeckPickerTrayDisplay.SetRotationTutorialState.INACTIVE;
		Options.Get().SetBool(Option.HAS_SEEN_STANDARD_MODE_TUTORIAL, true);
		this.m_switchFormatButton.EnableHighlight(false);
		NotificationManager.Get().DestroyNotification(this.m_switchFormatPopup, 0f);
		this.m_switchFormatPopup = null;
		this.m_dimQuad.StopPlayback();
		this.m_dimQuad.Play("DimQuad_FadeOut");
		yield return new WaitForEndOfFrame();
		float animDuration = this.m_dimQuad.GetCurrentAnimatorStateInfo(0).length;
		yield return new WaitForSeconds(animDuration);
		this.m_dimQuad.GetComponent<Renderer>().enabled = false;
		this.m_dimQuad.enabled = false;
		yield return new WaitForSeconds(this.m_showQuestPause);
		bool foundAchieve = false;
		foreach (Achievement achieve in AchieveManager.Get().GetActiveQuests(false))
		{
			if (achieve.ID == DeckPickerTrayDisplay.STANDARD_GAME_ACHIEVE_ID)
			{
				WelcomeQuests.ShowSpecialQuest(UserAttentionBlocker.SET_ROTATION_INTRO, achieve, new WelcomeQuests.DelOnWelcomeQuestsClosed(this.OnWelcomeQuestDismiss), false);
				foundAchieve = true;
				break;
			}
		}
		if (!foundAchieve)
		{
			this.OnWelcomeQuestDismiss();
		}
		yield break;
	}

	// Token: 0x06002D1D RID: 11549 RVA: 0x000E24C7 File Offset: 0x000E06C7
	private void OnWelcomeQuestDismiss()
	{
		base.StartCoroutine(this.PlayVOAndEndTutorial());
	}

	// Token: 0x06002D1E RID: 11550 RVA: 0x000E24D8 File Offset: 0x000E06D8
	private IEnumerator PlayVOAndEndTutorial()
	{
		yield return new WaitForSeconds(this.m_playVOPause);
		if (this.m_heroPower != null)
		{
			this.m_heroPower.GetComponent<Collider>().enabled = true;
		}
		if (this.m_goldenHeroPower != null)
		{
			this.m_goldenHeroPower.GetComponent<Collider>().enabled = true;
		}
		this.UpdateMissingStandardDeckTray(true);
		this.EnableBackButton(true);
		this.EnableCollectionButton(true);
		this.m_rightArrow.SetEnabled(true);
		this.m_leftArrow.SetEnabled(true);
		this.m_leftArrow.gameObject.SetActive(this.m_showingSecondPage);
		this.m_rightArrow.gameObject.SetActive(!this.m_showingSecondPage && this.m_customPages.Length > 1);
		this.m_rankedPlayButtons.EndSetRotationTutorial();
		Options.Get().SetBool(Option.GLOW_COLLECTION_BUTTON_AFTER_SET_ROTATION, true);
		this.UpdateCollectionButtonGlow();
		int numValidStandardDecks = this.GetNumValidStandardDecks();
		if (numValidStandardDecks == 0)
		{
			this.m_switchFormatButton.Enable();
		}
		else if (numValidStandardDecks == 1)
		{
			NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.SET_ROTATION_INTRO, DeckPickerTrayDisplay.INNKEEPER_QUOTE_POS, GameStrings.Get("VO_INNKEEPER_HAVE_ONE_STANDARD_DECK"), "VO_INNKEEPER_Male_Dwarf_HAVE_STANDARD_DECK_07", 0f, new Action(this.OnTutorialVOEnded));
		}
		else if (numValidStandardDecks > 1)
		{
			NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.SET_ROTATION_INTRO, DeckPickerTrayDisplay.INNKEEPER_QUOTE_POS, GameStrings.Get("VO_INNKEEPER_HAVE_STANDARD_DECKS"), "VO_INNKEEPER_Male_Dwarf_HAVE_STANDARD_DECKS_08", 0f, new Action(this.OnTutorialVOEnded));
		}
		else
		{
			this.OnTutorialVOEnded();
		}
		yield break;
	}

	// Token: 0x06002D1F RID: 11551 RVA: 0x000E24F4 File Offset: 0x000E06F4
	private void OnTutorialVOEnded()
	{
		if (this.m_switchFormatButton != null)
		{
			this.m_switchFormatButton.Enable();
		}
		ApplicationMgr.Get().ScheduleCallback(5f, false, delegate(object userdata)
		{
			UserAttentionManager.StopBlocking(UserAttentionBlocker.SET_ROTATION_INTRO);
		}, null);
	}

	// Token: 0x04001B87 RID: 7047
	private const int MAX_PRECON_DECKS_TO_DISPLAY = 9;

	// Token: 0x04001B88 RID: 7048
	private const float TRAY_SLIDE_TIME = 0.25f;

	// Token: 0x04001B89 RID: 7049
	private const float TRAY_SINK_TIME = 0f;

	// Token: 0x04001B8A RID: 7050
	public GameObject m_randomDeckPickerTray;

	// Token: 0x04001B8B RID: 7051
	public Transform m_Hero_Bone;

	// Token: 0x04001B8C RID: 7052
	public Transform m_Hero_BoneDown;

	// Token: 0x04001B8D RID: 7053
	public Transform m_HeroPower_Bone;

	// Token: 0x04001B8E RID: 7054
	public Transform m_HeroPower_BoneDown;

	// Token: 0x04001B8F RID: 7055
	public GameObject m_heroPowerShadowQuad;

	// Token: 0x04001B90 RID: 7056
	public Transform m_rankedPlayButtonsBone;

	// Token: 0x04001B91 RID: 7057
	public Texture m_emptyHeroTexture;

	// Token: 0x04001B92 RID: 7058
	public UberText m_heroName;

	// Token: 0x04001B93 RID: 7059
	public UberText m_modeName;

	// Token: 0x04001B94 RID: 7060
	public UIBButton m_backButton;

	// Token: 0x04001B95 RID: 7061
	public PlayButton m_playButton;

	// Token: 0x04001B96 RID: 7062
	public NestedPrefab m_leftArrowNestedPrefab;

	// Token: 0x04001B97 RID: 7063
	public NestedPrefab m_rightArrowNestedPrefab;

	// Token: 0x04001B98 RID: 7064
	public GameObject m_randomTray;

	// Token: 0x04001B99 RID: 7065
	public GameObject m_trayFrame;

	// Token: 0x04001B9A RID: 7066
	public GameObject m_modeLabelBg;

	// Token: 0x04001B9B RID: 7067
	public GameObject m_randomDecksShownBone;

	// Token: 0x04001B9C RID: 7068
	public GameObject m_randomDecksHiddenBone;

	// Token: 0x04001B9D RID: 7069
	public GameObject m_suckedInRandomDecksBone;

	// Token: 0x04001B9E RID: 7070
	public GameObject m_heroPrefab;

	// Token: 0x04001B9F RID: 7071
	public Vector3 m_heroPickerButtonStart;

	// Token: 0x04001BA0 RID: 7072
	public Vector3 m_heroPickerButtonScale;

	// Token: 0x04001BA1 RID: 7073
	public float m_heroPickerButtonHorizontalSpacing;

	// Token: 0x04001BA2 RID: 7074
	public float m_heroPickerButtonVerticalSpacing;

	// Token: 0x04001BA3 RID: 7075
	public GameObject m_basicDeckPageContainer;

	// Token: 0x04001BA4 RID: 7076
	public List<NestedPrefab> m_customDeckPageContainers;

	// Token: 0x04001BA5 RID: 7077
	public GameObject m_tooltipPrefab;

	// Token: 0x04001BA6 RID: 7078
	public Transform m_tooltipBone;

	// Token: 0x04001BA7 RID: 7079
	public HeroXPBar m_xpBarPrefab;

	// Token: 0x04001BA8 RID: 7080
	public GameObject m_rankedWinsPlate;

	// Token: 0x04001BA9 RID: 7081
	public UberText m_rankedWins;

	// Token: 0x04001BAA RID: 7082
	public BoxCollider m_expoClickBlocker;

	// Token: 0x04001BAB RID: 7083
	public Animator m_premadeDeckGlowAnimator;

	// Token: 0x04001BAC RID: 7084
	public GameObject m_hierarchyDeckTray;

	// Token: 0x04001BAD RID: 7085
	public GameObject m_hierarchyDetails;

	// Token: 0x04001BAE RID: 7086
	public MissingStandardDeckDisplay m_missingStandardDeckDisplay;

	// Token: 0x04001BAF RID: 7087
	public UIBButton m_collectionButton;

	// Token: 0x04001BB0 RID: 7088
	public HighlightState m_collectionButtonGlow;

	// Token: 0x04001BB1 RID: 7089
	public GameObject m_labelDecoration;

	// Token: 0x04001BB2 RID: 7090
	public PlayMakerFSM m_vineGlowBurst;

	// Token: 0x04001BB3 RID: 7091
	public List<GameObject> m_premadeDeckGlowBurstObjects;

	// Token: 0x04001BB4 RID: 7092
	public NestedPrefab m_switchFormatButtonContainer;

	// Token: 0x04001BB5 RID: 7093
	private SwitchFormatButton m_switchFormatButton;

	// Token: 0x04001BB6 RID: 7094
	public GameObject m_TheClockButtonBone;

	// Token: 0x04001BB7 RID: 7095
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_standardTransitionSound;

	// Token: 0x04001BB8 RID: 7096
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_wildTransitionSound;

	// Token: 0x04001BB9 RID: 7097
	[CustomEditField(Sections = "Phone Only")]
	public SlidingTray m_rankedDetailsTray;

	// Token: 0x04001BBA RID: 7098
	[CustomEditField(Sections = "Phone Only")]
	public GameObject m_detailsTrayFrame;

	// Token: 0x04001BBB RID: 7099
	[CustomEditField(Sections = "Phone Only")]
	public Transform m_medalBone_phone;

	// Token: 0x04001BBC RID: 7100
	[CustomEditField(Sections = "Phone Only")]
	public Mesh m_alternateDetailsTrayMesh;

	// Token: 0x04001BBD RID: 7101
	[CustomEditField(Sections = "Phone Only")]
	public Material m_arrowButtonShadowMaterial;

	// Token: 0x04001BBE RID: 7102
	private static readonly Vector3 INNKEEPER_QUOTE_POS;

	// Token: 0x04001BBF RID: 7103
	private bool m_heroChosen;

	// Token: 0x04001BC0 RID: 7104
	private static Coroutine s_selectHeroCoroutine;

	// Token: 0x04001BC1 RID: 7105
	private readonly List<TAG_CLASS> HERO_CLASSES;

	// Token: 0x04001BC2 RID: 7106
	private List<HeroPickerButton> m_heroButtons;

	// Token: 0x04001BC3 RID: 7107
	private Actor m_heroActor;

	// Token: 0x04001BC4 RID: 7108
	private UIBButton m_leftArrow;

	// Token: 0x04001BC5 RID: 7109
	private UIBButton m_rightArrow;

	// Token: 0x04001BC6 RID: 7110
	private PegUIElement m_heroPower;

	// Token: 0x04001BC7 RID: 7111
	private Actor m_heroPowerActor;

	// Token: 0x04001BC8 RID: 7112
	private PegUIElement m_goldenHeroPower;

	// Token: 0x04001BC9 RID: 7113
	private Actor m_goldenHeroPowerActor;

	// Token: 0x04001BCA RID: 7114
	private bool m_isMouseOverHeroPower;

	// Token: 0x04001BCB RID: 7115
	private FullDef m_selectedHeroPowerFullDef;

	// Token: 0x04001BCC RID: 7116
	private Actor m_heroPowerBigCard;

	// Token: 0x04001BCD RID: 7117
	private Actor m_goldenHeroPowerBigCard;

	// Token: 0x04001BCE RID: 7118
	private HeroXPBar m_xpBar;

	// Token: 0x04001BCF RID: 7119
	private Hashtable m_heroPowerDefs;

	// Token: 0x04001BD0 RID: 7120
	private HeroPickerButton m_selectedHeroButton;

	// Token: 0x04001BD1 RID: 7121
	private CollectionDeckBoxVisual m_selectedCustomDeckBox;

	// Token: 0x04001BD2 RID: 7122
	private DeckPickerMode m_deckPickerMode;

	// Token: 0x04001BD3 RID: 7123
	private bool m_showingSecondPage;

	// Token: 0x04001BD4 RID: 7124
	private static DeckPickerTrayDisplay s_instance;

	// Token: 0x04001BD5 RID: 7125
	private string gameMode;

	// Token: 0x04001BD6 RID: 7126
	private Vector2 m_keyholeTextureOffset;

	// Token: 0x04001BD7 RID: 7127
	private string m_selectedHeroName;

	// Token: 0x04001BD8 RID: 7128
	private RankedPlayDisplay m_rankedPlayButtons;

	// Token: 0x04001BD9 RID: 7129
	private int m_numDecks;

	// Token: 0x04001BDA RID: 7130
	private int m_numPagesToShow;

	// Token: 0x04001BDB RID: 7131
	private KeywordHelpPanel m_tooltip;

	// Token: 0x04001BDC RID: 7132
	private CustomDeckPage[] m_customPages;

	// Token: 0x04001BDD RID: 7133
	private int m_heroDefsLoading;

	// Token: 0x04001BDE RID: 7134
	private int m_heroPowerDefsLoading;

	// Token: 0x04001BDF RID: 7135
	private bool m_buttonAchievementsInitialized;

	// Token: 0x04001BE0 RID: 7136
	private bool m_delayButtonAnims;

	// Token: 0x04001BE1 RID: 7137
	private bool m_Loaded;

	// Token: 0x04001BE2 RID: 7138
	private Notification m_expoThankQuote;

	// Token: 0x04001BE3 RID: 7139
	private Notification m_expoIntroQuote;

	// Token: 0x04001BE4 RID: 7140
	private List<DeckPickerTrayDisplay.DeckTrayLoaded> m_DeckTrayLoadedListeners;

	// Token: 0x04001BE5 RID: 7141
	private bool m_needUnlockAllHeroesTransition;

	// Token: 0x04001BE6 RID: 7142
	private float m_numCardsPerClass;

	// Token: 0x04001BE7 RID: 7143
	private bool m_isUsingWildVisuals;

	// Token: 0x04001BE8 RID: 7144
	private Notification m_switchFormatPopup;

	// Token: 0x04001BE9 RID: 7145
	private Notification m_innkeeperQuote;

	// Token: 0x04001BEA RID: 7146
	private SlidingTray m_slidingTray;

	// Token: 0x04001BEB RID: 7147
	public static readonly PlatformDependentValue<bool> HighlightSelectedDeck;

	// Token: 0x04001BEC RID: 7148
	private bool m_innkeeperQuoteFinished;

	// Token: 0x04001BED RID: 7149
	[CustomEditField(Sections = "Set Rotation Tutorial")]
	public GameObject m_formatTutorialPopUpPrefab;

	// Token: 0x04001BEE RID: 7150
	[CustomEditField(Sections = "Set Rotation Tutorial")]
	public Transform m_formatTutorialPopUpBone;

	// Token: 0x04001BEF RID: 7151
	[CustomEditField(Sections = "Set Rotation Tutorial")]
	public Transform m_Switch_Format_Notification_Bone;

	// Token: 0x04001BF0 RID: 7152
	[CustomEditField(Sections = "Set Rotation Tutorial")]
	public Animator m_dimQuad;

	// Token: 0x04001BF1 RID: 7153
	[CustomEditField(Sections = "Set Rotation Tutorial")]
	public PegUIElement m_clickCatcher;

	// Token: 0x04001BF2 RID: 7154
	[CustomEditField(Sections = "Set Rotation Tutorial", T = EditType.SOUND_PREFAB)]
	public string m_formatPopUpShowSound;

	// Token: 0x04001BF3 RID: 7155
	[CustomEditField(Sections = "Set Rotation Tutorial", T = EditType.SOUND_PREFAB)]
	public string m_formatPopUpHideSound;

	// Token: 0x04001BF4 RID: 7156
	[CustomEditField(Sections = "Set Rotation Tutorial", T = EditType.SOUND_PREFAB)]
	public string m_wildDeckTransitionSound;

	// Token: 0x04001BF5 RID: 7157
	private DeckPickerTrayDisplay.SetRotationTutorialState m_setRotationTutorialState;

	// Token: 0x04001BF6 RID: 7158
	private float m_showQuestPause = 1f;

	// Token: 0x04001BF7 RID: 7159
	private float m_playVOPause = 1f;

	// Token: 0x04001BF8 RID: 7160
	private Vector3 m_formatTutorialPopupScale = new Vector3(1.1f, 1.1f, 1.1f);

	// Token: 0x04001BF9 RID: 7161
	private Vector3 m_formatTutorialPopupScalePhone = new Vector3(2.3f, 2.3f, 2.3f);

	// Token: 0x04001BFA RID: 7162
	private static readonly int STANDARD_GAME_ACHIEVE_ID = 340;

	// Token: 0x0200036E RID: 878
	// (Invoke) Token: 0x06002D24 RID: 11556
	public delegate void DeckTrayLoaded();

	// Token: 0x0200036F RID: 879
	private class HeroFullDefLoadedCallbackData
	{
		// Token: 0x06002D27 RID: 11559 RVA: 0x000E258C File Offset: 0x000E078C
		public HeroFullDefLoadedCallbackData(HeroPickerButton button, TAG_PREMIUM premium)
		{
			this.HeroPickerButton = button;
			this.Premium = premium;
		}

		// Token: 0x1700039A RID: 922
		// (get) Token: 0x06002D28 RID: 11560 RVA: 0x000E25A2 File Offset: 0x000E07A2
		// (set) Token: 0x06002D29 RID: 11561 RVA: 0x000E25AA File Offset: 0x000E07AA
		public HeroPickerButton HeroPickerButton { get; private set; }

		// Token: 0x1700039B RID: 923
		// (get) Token: 0x06002D2A RID: 11562 RVA: 0x000E25B3 File Offset: 0x000E07B3
		// (set) Token: 0x06002D2B RID: 11563 RVA: 0x000E25BB File Offset: 0x000E07BB
		public TAG_PREMIUM Premium { get; private set; }
	}

	// Token: 0x02000371 RID: 881
	private enum SetRotationTutorialState
	{
		// Token: 0x04001C12 RID: 7186
		INACTIVE,
		// Token: 0x04001C13 RID: 7187
		READY,
		// Token: 0x04001C14 RID: 7188
		SHOW_FORMAT_TUTORIAL_POPUP,
		// Token: 0x04001C15 RID: 7189
		SWITCH_TO_STANDARD,
		// Token: 0x04001C16 RID: 7190
		SHOW_QUEST_LOG
	}
}
