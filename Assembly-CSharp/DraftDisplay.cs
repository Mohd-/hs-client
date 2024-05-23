using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000528 RID: 1320
[CustomEditClass]
public class DraftDisplay : MonoBehaviour
{
	// Token: 0x06003D45 RID: 15685 RVA: 0x00127780 File Offset: 0x00125980
	private void Awake()
	{
		DraftDisplay.s_instance = this;
		AssetLoader.Get().LoadActor("DraftHeroChooseButton", new AssetLoader.GameObjectCallback(this.OnConfirmButtonLoaded), null, false);
		AssetLoader.Get().LoadActor("History_HeroPower", new AssetLoader.GameObjectCallback(this.LoadHeroPowerCallback), null, false);
		if (UniversalInputManager.UsePhoneUI)
		{
			AssetLoader.Get().LoadGameObject("BackButton_phone", new AssetLoader.GameObjectCallback(this.OnPhoneBackButtonLoaded), null, false);
		}
		DraftManager.Get().RegisterNetHandlers();
		DraftManager.Get().RegisterMatchmakerHandlers();
		DraftManager.Get().RegisterStoreHandlers();
		this.m_forgeLabel.Text = GameStrings.Get("GLUE_TOOLTIP_BUTTON_FORGE_HEADLINE");
		this.m_instructionText.Text = string.Empty;
		this.m_pickArea.enabled = false;
		if (DemoMgr.Get().ArenaIs1WinMode())
		{
			Options.Get().SetBool(Option.HAS_SEEN_FORGE_PLAY_MODE, false);
			Options.Get().SetBool(Option.HAS_SEEN_FORGE_CARD_CHOICE, false);
			Options.Get().SetBool(Option.HAS_SEEN_FORGE, true);
			Options.Get().SetBool(Option.HAS_SEEN_FORGE_HERO_CHOICE, true);
			Options.Get().SetBool(Option.HAS_SEEN_FORGE_CARD_CHOICE2, false);
		}
		Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
	}

	// Token: 0x06003D46 RID: 15686 RVA: 0x001278AC File Offset: 0x00125AAC
	private void OnDestroy()
	{
		DraftDisplay.s_instance = null;
	}

	// Token: 0x06003D47 RID: 15687 RVA: 0x001278B4 File Offset: 0x00125AB4
	private void Start()
	{
		NetCache.Get().RegisterScreenForge(new NetCache.NetCacheCallback(this.OnNetCacheReady));
		this.SetupRetireButton();
		this.m_playButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.PlayButtonPress));
		this.m_manaCurve.GetComponent<PegUIElement>().AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.ManaCurveOver));
		this.m_manaCurve.GetComponent<PegUIElement>().AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.ManaCurveOut));
		this.m_playButton.SetText(GameStrings.Get("GLOBAL_PLAY"));
		this.ShowPhonePlayButton(false);
		if (!UniversalInputManager.UsePhoneUI)
		{
			this.SetupBackButton();
		}
		Network.FindOutCurrentDraftState();
		MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_Arena);
		base.StartCoroutine(this.NotifySceneLoadedWhenReady());
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_draftDeckTray.gameObject.SetActive(true);
		}
		ArenaTrayDisplay.Get().ShowPlainPaperBackground();
	}

	// Token: 0x06003D48 RID: 15688 RVA: 0x001279AC File Offset: 0x00125BAC
	private void Update()
	{
		Network.Get().ProcessNetwork();
	}

	// Token: 0x06003D49 RID: 15689 RVA: 0x001279B8 File Offset: 0x00125BB8
	public static DraftDisplay Get()
	{
		return DraftDisplay.s_instance;
	}

	// Token: 0x06003D4A RID: 15690 RVA: 0x001279BF File Offset: 0x00125BBF
	public void OnOpenRewardsComplete()
	{
		this.ExitDraftScene();
	}

	// Token: 0x06003D4B RID: 15691 RVA: 0x001279C7 File Offset: 0x00125BC7
	public void OnApplicationPause(bool pauseStatus)
	{
		if (GameMgr.Get().IsFindingGame())
		{
			this.CancelFindGame();
		}
	}

	// Token: 0x06003D4C RID: 15692 RVA: 0x001279E0 File Offset: 0x00125BE0
	public void Unload()
	{
		Box.Get().SetToIgnoreFullScreenEffects(false);
		if (this.m_confirmButton != null)
		{
			Object.Destroy(this.m_confirmButton.gameObject);
		}
		if (this.m_heroPower != null)
		{
			this.m_heroPower.Destroy();
		}
		if (this.m_chosenHero != null)
		{
			this.m_chosenHero.Destroy();
		}
		DraftManager.Get().RemoveNetHandlers();
		DraftManager.Get().RemoveMatchmakerHandlers();
		DraftManager.Get().RemoveStoreHandlers();
		DraftInputManager.Get().Unload();
	}

	// Token: 0x06003D4D RID: 15693 RVA: 0x00127A7C File Offset: 0x00125C7C
	public void AcceptNewChoices(List<NetCache.CardDefinition> choices)
	{
		this.DestroyOldChoices();
		this.UpdateInstructionText();
		base.StartCoroutine(this.WaitForAnimsToFinishAndThenDisplayNewChoices(choices));
	}

	// Token: 0x06003D4E RID: 15694 RVA: 0x00127AA4 File Offset: 0x00125CA4
	private IEnumerator WaitForAnimsToFinishAndThenDisplayNewChoices(List<NetCache.CardDefinition> choices)
	{
		while (!this.m_animationsComplete)
		{
			yield return null;
		}
		while (this.m_isHeroAnimating)
		{
			yield return null;
		}
		for (int i = 0; i < choices.Count; i++)
		{
			NetCache.CardDefinition cardDefinition = choices[i];
			DraftDisplay.DraftChoice newChoice = new DraftDisplay.DraftChoice
			{
				m_cardID = cardDefinition.Name,
				m_premium = cardDefinition.Premium
			};
			this.m_choices.Add(newChoice);
		}
		int currentDraftSlot = DraftManager.Get().GetSlot();
		this.m_skipHeroEmotes = false;
		for (int j = 0; j < this.m_choices.Count; j++)
		{
			DraftDisplay.DraftChoice choice = this.m_choices[j];
			DraftDisplay.ChoiceCallback callbackInfo = new DraftDisplay.ChoiceCallback();
			callbackInfo.choiceID = j + 1;
			callbackInfo.slot = currentDraftSlot;
			DefLoader.Get().LoadFullDef(choice.m_cardID, new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded), callbackInfo);
		}
		yield break;
	}

	// Token: 0x06003D4F RID: 15695 RVA: 0x00127AD0 File Offset: 0x00125CD0
	public void SetDraftMode(DraftDisplay.DraftMode mode)
	{
		bool flag = this.m_currentMode != mode;
		this.m_currentMode = mode;
		if (!flag)
		{
			return;
		}
		Log.Arena.Print("SetDraftMode - " + this.m_currentMode, new object[0]);
		this.InitializeDraftScreen();
	}

	// Token: 0x06003D50 RID: 15696 RVA: 0x00127B23 File Offset: 0x00125D23
	public DraftDisplay.DraftMode GetDraftMode()
	{
		return this.m_currentMode;
	}

	// Token: 0x06003D51 RID: 15697 RVA: 0x00127B2B File Offset: 0x00125D2B
	public void CancelFindGame()
	{
		GameMgr.Get().CancelFindGame();
		this.HandleGameStartupFailure();
	}

	// Token: 0x06003D52 RID: 15698 RVA: 0x00127B40 File Offset: 0x00125D40
	public void OnHeroClicked(int heroChoice)
	{
		SoundManager.Get().LoadAndPlay("tournament_screen_select_hero");
		this.m_isHeroAnimating = true;
		DraftDisplay.DraftChoice draftChoice = this.m_choices[heroChoice - 1];
		this.m_zoomedHero = draftChoice.m_actor.GetCollider().gameObject.GetComponent<DraftCardVisual>();
		draftChoice.m_actor.SetUnlit();
		iTween.MoveTo(draftChoice.m_actor.gameObject, this.m_bigHeroBone.position, 0.25f);
		iTween.ScaleTo(draftChoice.m_actor.gameObject, this.m_bigHeroBone.localScale, 0.25f);
		SoundManager.Get().LoadAndPlay("forge_hero_portrait_plate_rises");
		this.FadeEffectsIn();
		SceneUtils.SetLayer(draftChoice.m_actor.gameObject, GameLayer.IgnoreFullScreenEffects);
		UniversalInputManager.Get().SetGameDialogActive(true);
		this.m_confirmButton.gameObject.SetActive(true);
		this.m_confirmButton.m_button.GetComponent<PlayMakerFSM>().SendEvent("Birth");
		this.m_confirmButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnConfirmButtonClicked));
		this.m_heroClickCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCancelButtonClicked));
		this.m_heroClickCatcher.gameObject.SetActive(true);
		draftChoice.m_actor.TurnOffCollider();
		draftChoice.m_actor.SetActorState(ActorStateType.CARD_IDLE);
		bool flag = this.IsHeroEmoteSpellReady(heroChoice - 1);
		base.StartCoroutine(this.WaitForSpellToLoadAndPlay(heroChoice - 1));
		this.ShowHeroPowerBigCard();
		if (this.CanAutoDraft() && flag)
		{
			this.OnConfirmButtonClicked(null);
		}
	}

	// Token: 0x06003D53 RID: 15699 RVA: 0x00127CC7 File Offset: 0x00125EC7
	private bool IsHeroEmoteSpellReady(int index)
	{
		return this.m_heroEmotes[index] != null || this.m_skipHeroEmotes;
	}

	// Token: 0x06003D54 RID: 15700 RVA: 0x00127CE8 File Offset: 0x00125EE8
	private IEnumerator WaitForSpellToLoadAndPlay(int index)
	{
		bool wasEmoteAlreadyReady = this.IsHeroEmoteSpellReady(index);
		while (!this.IsHeroEmoteSpellReady(index))
		{
			yield return null;
		}
		if (!this.m_skipHeroEmotes)
		{
			CardSoundSpell emote = this.m_heroEmotes[index];
			emote.Reactivate();
		}
		if (this.CanAutoDraft() && !wasEmoteAlreadyReady)
		{
			this.OnConfirmButtonClicked(null);
		}
		yield break;
	}

	// Token: 0x06003D55 RID: 15701 RVA: 0x00127D11 File Offset: 0x00125F11
	private void OnConfirmButtonClicked(UIEvent e)
	{
		if (GameUtils.IsAnyTransitionActive())
		{
			return;
		}
		this.EnableBackButton(false);
		this.DoHeroSelectAnimation();
	}

	// Token: 0x06003D56 RID: 15702 RVA: 0x00127D2B File Offset: 0x00125F2B
	private void OnCancelButtonClicked(UIEvent e)
	{
		if (this.IsInHeroSelectMode())
		{
			this.DoHeroCancelAnimation();
		}
		else
		{
			Navigation.GoBack();
		}
	}

	// Token: 0x06003D57 RID: 15703 RVA: 0x00127D4C File Offset: 0x00125F4C
	private void RemoveListeners()
	{
		this.m_confirmButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnConfirmButtonClicked));
		this.m_confirmButton.m_button.GetComponent<PlayMakerFSM>().SendEvent("Death");
		this.m_confirmButton.gameObject.SetActive(false);
		this.m_heroClickCatcher.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCancelButtonClicked));
		this.m_heroClickCatcher.gameObject.SetActive(false);
	}

	// Token: 0x06003D58 RID: 15704 RVA: 0x00127DC8 File Offset: 0x00125FC8
	private void FadeEffectsIn()
	{
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		fullScreenFXMgr.SetBlurBrightness(1f);
		fullScreenFXMgr.SetBlurDesaturation(0f);
		fullScreenFXMgr.Vignette(0.8f, 0.4f, iTween.EaseType.easeOutCirc, null);
		fullScreenFXMgr.Blur(1f, 0.4f, iTween.EaseType.easeOutCirc, null);
	}

	// Token: 0x06003D59 RID: 15705 RVA: 0x00127E18 File Offset: 0x00126018
	private void FadeEffectsOut()
	{
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		fullScreenFXMgr.StopVignette(0.2f, iTween.EaseType.easeOutCirc, new FullScreenFXMgr.EffectListener(this.OnFadeFinished));
		fullScreenFXMgr.StopBlur(0.2f, iTween.EaseType.easeOutCirc, null);
	}

	// Token: 0x06003D5A RID: 15706 RVA: 0x00127E52 File Offset: 0x00126052
	private void OnFadeFinished()
	{
		if (this.m_chosenHero == null)
		{
			return;
		}
		SceneUtils.SetLayer(this.m_chosenHero.gameObject, GameLayer.Default);
	}

	// Token: 0x06003D5B RID: 15707 RVA: 0x00127E78 File Offset: 0x00126078
	public void DoHeroCancelAnimation()
	{
		this.RemoveListeners();
		this.m_heroPower.Hide();
		DraftDisplay.DraftChoice draftChoice = this.m_choices[this.m_zoomedHero.GetChoiceNum() - 1];
		SceneUtils.SetLayer(draftChoice.m_actor.gameObject, GameLayer.Default);
		draftChoice.m_actor.TurnOnCollider();
		this.FadeEffectsOut();
		UniversalInputManager.Get().SetGameDialogActive(false);
		this.m_isHeroAnimating = false;
		this.m_pickArea.enabled = true;
		iTween.MoveTo(this.m_zoomedHero.GetActor().gameObject, this.GetCardPosition(this.m_zoomedHero.GetChoiceNum() - 1, true), 0.25f);
		if (UniversalInputManager.UsePhoneUI)
		{
			iTween.ScaleTo(draftChoice.m_actor.gameObject, DraftDisplay.HERO_ACTOR_LOCAL_SCALE_PHONE, 0.25f);
		}
		else
		{
			iTween.ScaleTo(draftChoice.m_actor.gameObject, DraftDisplay.HERO_ACTOR_LOCAL_SCALE, 0.25f);
		}
		this.m_pickArea.enabled = false;
		this.m_zoomedHero = null;
	}

	// Token: 0x06003D5C RID: 15708 RVA: 0x00127F78 File Offset: 0x00126178
	public bool IsInHeroSelectMode()
	{
		return this.m_zoomedHero != null;
	}

	// Token: 0x06003D5D RID: 15709 RVA: 0x00127F88 File Offset: 0x00126188
	private void DoHeroSelectAnimation()
	{
		this.RemoveListeners();
		this.m_heroPower.Hide();
		this.FadeEffectsOut();
		UniversalInputManager.Get().SetGameDialogActive(false);
		this.m_chosenHero = this.m_zoomedHero.GetActor();
		this.m_zoomedHero.SetChosenFlag(true);
		DraftManager.Get().MakeChoice(this.m_zoomedHero.GetChoiceNum());
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_zoomedHero.GetActor().transform.parent = this.m_socketHeroBone;
			iTween.MoveTo(this.m_zoomedHero.GetActor().gameObject, iTween.Hash(new object[]
			{
				"position",
				Vector3.zero,
				"time",
				0.25f,
				"isLocal",
				true,
				"easeType",
				iTween.EaseType.easeInCubic,
				"oncomplete",
				"PhoneHeroAnimationFinished",
				"oncompletetarget",
				base.gameObject
			}));
			iTween.ScaleTo(this.m_zoomedHero.GetActor().gameObject, iTween.Hash(new object[]
			{
				"scale",
				Vector3.one,
				"time",
				0.25f,
				"easeType",
				iTween.EaseType.easeInCubic
			}));
		}
		else
		{
			this.m_zoomedHero.GetActor().ActivateSpell(SpellType.CONSTRUCT);
			this.m_zoomedHero = null;
			this.m_isHeroAnimating = false;
		}
		SoundManager.Get().LoadAndPlay("forge_hero_portrait_plate_descend_and_impact");
		if (!Options.Get().GetBool(Option.HAS_SEEN_FORGE_CARD_CHOICE, false) && UserAttentionManager.CanShowAttentionGrabber("DraftDisplay.DoHeroSelectAnimation:" + Option.HAS_SEEN_FORGE_CARD_CHOICE))
		{
			NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_FORGE_INST2_20"), "VO_INNKEEPER_FORGE_INST2_20", 3f, null);
			Options.Get().SetBool(Option.HAS_SEEN_FORGE_CARD_CHOICE, true);
		}
	}

	// Token: 0x06003D5E RID: 15710 RVA: 0x0012818F File Offset: 0x0012638F
	private void PhoneHeroAnimationFinished()
	{
		Log.Arena.Print("Phone Hero animation complete", new object[0]);
		this.m_zoomedHero = null;
		this.m_isHeroAnimating = false;
	}

	// Token: 0x06003D5F RID: 15711 RVA: 0x001281B4 File Offset: 0x001263B4
	public void AddCardToManaCurve(EntityDef entityDef)
	{
		if (entityDef == null)
		{
			Debug.LogWarning("DraftDisplay.AddCardToManaCurve() - entityDef is null");
			return;
		}
		if (this.m_manaCurve == null)
		{
			Debug.LogWarning(string.Format("DraftDisplay.AddCardToManaCurve({0}) - m_manaCurve is null", entityDef));
			return;
		}
		this.m_manaCurve.AddCardOfCost(entityDef.GetCost());
	}

	// Token: 0x06003D60 RID: 15712 RVA: 0x00128208 File Offset: 0x00126408
	public List<DraftCardVisual> GetCardVisuals()
	{
		List<DraftCardVisual> list = new List<DraftCardVisual>();
		foreach (DraftDisplay.DraftChoice draftChoice in this.m_choices)
		{
			if (draftChoice.m_actor == null)
			{
				return null;
			}
			DraftCardVisual component = draftChoice.m_actor.GetCollider().gameObject.GetComponent<DraftCardVisual>();
			if (component == null)
			{
				return null;
			}
			list.Add(component);
		}
		return list;
	}

	// Token: 0x06003D61 RID: 15713 RVA: 0x001282B0 File Offset: 0x001264B0
	public void HandleGameStartupFailure()
	{
		this.m_playButton.Enable();
		this.ShowPhonePlayButton(true);
		PresenceMgr.Get().SetPrevStatus();
	}

	// Token: 0x06003D62 RID: 15714 RVA: 0x001282D0 File Offset: 0x001264D0
	public void DoDeckCompleteAnims()
	{
		SoundManager.Get().LoadAndPlay("forge_commit_deck");
		if (!UniversalInputManager.UsePhoneUI)
		{
			this.m_DeckCompleteSpell.Activate();
			if (this.m_draftDeckTray != null)
			{
				this.m_draftDeckTray.GetCardsContent().ShowDeckCompleteEffects();
			}
		}
	}

	// Token: 0x06003D63 RID: 15715 RVA: 0x00128328 File Offset: 0x00126528
	private IEnumerator NotifySceneLoadedWhenReady()
	{
		while (this.m_confirmButton == null)
		{
			yield return null;
		}
		while (this.m_heroPower == null)
		{
			yield return null;
		}
		while (this.m_currentMode == DraftDisplay.DraftMode.INVALID)
		{
			yield return null;
		}
		while (!this.m_netCacheReady)
		{
			yield return null;
		}
		while (!AchieveManager.Get().IsReady())
		{
			yield return null;
		}
		this.InitManaCurve();
		this.m_draftDeckTray.Initialize();
		PegUIElement deckHeader = this.m_draftDeckTray.GetTooltipZone().gameObject.GetComponent<PegUIElement>();
		deckHeader.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.DeckHeaderOver));
		deckHeader.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.DeckHeaderOut));
		this.ShowNewQuests();
		SceneMgr.Get().NotifySceneLoaded();
		yield break;
	}

	// Token: 0x06003D64 RID: 15716 RVA: 0x00128344 File Offset: 0x00126544
	private void ShowNewQuests()
	{
		if (this.m_questsHandled)
		{
			return;
		}
		if (this.m_currentMode == DraftDisplay.DraftMode.IN_REWARDS)
		{
			return;
		}
		this.m_questsHandled = true;
		if (AchieveManager.Get().HasActiveQuests(true))
		{
			WelcomeQuests.Show(UserAttentionBlocker.NONE, false, new WelcomeQuests.DelOnWelcomeQuestsClosed(this.OnNewQuestsShown), false);
			return;
		}
		this.OnNewQuestsShown();
		GameToastMgr.Get().UpdateQuestProgressToasts();
	}

	// Token: 0x06003D65 RID: 15717 RVA: 0x001283A6 File Offset: 0x001265A6
	private void OnNewQuestsShown()
	{
		this.m_newlyCompletedAchieves = AchieveManager.Get().GetNewCompletedAchieves();
		this.ShowNextCompletedQuestToast(null);
	}

	// Token: 0x06003D66 RID: 15718 RVA: 0x001283C0 File Offset: 0x001265C0
	private void ShowNextCompletedQuestToast(object userData)
	{
		if (this.m_newlyCompletedAchieves.Count == 0)
		{
			return;
		}
		Achievement quest = this.m_newlyCompletedAchieves[0];
		this.m_newlyCompletedAchieves.RemoveAt(0);
		QuestToast.ShowQuestToast(UserAttentionBlocker.NONE, new QuestToast.DelOnCloseQuestToast(this.ShowNextCompletedQuestToast), true, quest);
	}

	// Token: 0x06003D67 RID: 15719 RVA: 0x0012840C File Offset: 0x0012660C
	private void InitializeDraftScreen()
	{
		if (!this.m_firstTimeIntroComplete && !Options.Get().GetBool(Option.HAS_SEEN_FORGE, false) && UserAttentionManager.CanShowAttentionGrabber("DraftDisplay.InitializeDraftScreen:" + Option.HAS_SEEN_FORGE))
		{
			PresenceMgr.Get().SetStatus(new Enum[]
			{
				PresenceStatus.ARENA_PURCHASE
			});
			this.m_firstTimeIntroComplete = true;
			this.DoFirstTimeIntro();
			return;
		}
		switch (this.m_currentMode)
		{
		case DraftDisplay.DraftMode.NO_ACTIVE_DRAFT:
			PresenceMgr.Get().SetStatus(new Enum[]
			{
				PresenceStatus.ARENA_PURCHASE
			});
			this.ShowPurchaseScreen();
			break;
		case DraftDisplay.DraftMode.DRAFTING:
			if (StoreManager.Get().HasOutstandingPurchaseNotices(2))
			{
				this.ShowPurchaseScreen();
			}
			PresenceMgr.Get().SetStatus(new Enum[]
			{
				PresenceStatus.ARENA_FORGE
			});
			this.ShowCurrentlyDraftingScreen();
			break;
		case DraftDisplay.DraftMode.ACTIVE_DRAFT_DECK:
			PresenceMgr.Get().SetStatus(new Enum[]
			{
				PresenceStatus.ARENA_IDLE
			});
			base.StartCoroutine(this.ShowActiveDraftScreen());
			break;
		case DraftDisplay.DraftMode.IN_REWARDS:
			PresenceMgr.Get().SetStatus(new Enum[]
			{
				PresenceStatus.ARENA_REWARD
			});
			this.ShowDraftRewardsScreen();
			break;
		default:
			Debug.LogError(string.Format("DraftDisplay.InitializeDraftScreen(): don't know how to handle m_currentMode = {0}", this.m_currentMode));
			break;
		}
	}

	// Token: 0x06003D68 RID: 15720 RVA: 0x00128574 File Offset: 0x00126774
	private void OnConfirmButtonLoaded(string name, GameObject go, object callbackData)
	{
		this.m_confirmButton = go.GetComponent<NormalButton>();
		this.m_confirmButton.SetText(GameStrings.Get("GLUE_CHOOSE"));
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_confirmButton.transform.position = DraftDisplay.HERO_CONFIRM_BUTTON_POSITION_PHONE;
		}
		else
		{
			this.m_confirmButton.transform.position = DraftDisplay.HERO_CONFIRM_BUTTON_POSITION;
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_confirmButton.transform.localScale = DraftDisplay.HERO_CONFIRM_BUTTON_SCALE_PHONE;
		}
		else
		{
			this.m_confirmButton.transform.localScale = DraftDisplay.HERO_CONFIRM_BUTTON_SCALE;
		}
		this.m_confirmButton.gameObject.SetActive(false);
		SceneUtils.SetLayer(go, GameLayer.IgnoreFullScreenEffects);
	}

	// Token: 0x06003D69 RID: 15721 RVA: 0x00128638 File Offset: 0x00126838
	private void OnPhoneBackButtonLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError("Phone Back Button failed to load!");
			return;
		}
		this.m_backButton = go.GetComponent<UIBButton>();
		this.m_backButton.transform.parent = this.m_PhoneBackButtonBone;
		this.m_backButton.transform.position = this.m_PhoneBackButtonBone.position;
		this.m_backButton.transform.localScale = this.m_PhoneBackButtonBone.localScale;
		this.m_backButton.transform.rotation = Quaternion.identity;
		SceneUtils.SetLayer(go, GameLayer.Default);
		this.SetupBackButton();
	}

	// Token: 0x06003D6A RID: 15722 RVA: 0x001286D8 File Offset: 0x001268D8
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
		component.TurnOffCollider();
		SceneUtils.SetLayer(component.gameObject, GameLayer.IgnoreFullScreenEffects);
		this.m_heroPower = component;
		component.Hide();
	}

	// Token: 0x06003D6B RID: 15723 RVA: 0x00128748 File Offset: 0x00126948
	private void ShowHeroPowerBigCard()
	{
		if (this.m_heroPower == null)
		{
			return;
		}
		SceneUtils.SetLayer(this.m_heroPower.gameObject, GameLayer.IgnoreFullScreenEffects);
		this.m_heroPower.gameObject.transform.parent = this.m_zoomedHero.GetActor().transform;
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_heroPower.gameObject.transform.localPosition = DraftDisplay.HERO_POWER_START_POSITION_PHONE;
			this.m_heroPower.gameObject.transform.localScale = DraftDisplay.HERO_POWER_SCALE_PHONE;
		}
		else
		{
			this.m_heroPower.gameObject.transform.localPosition = DraftDisplay.HERO_POWER_START_POSITION;
			this.m_heroPower.gameObject.transform.localScale = DraftDisplay.HERO_POWER_SCALE;
		}
		base.StartCoroutine(this.ShowHeroPowerWhenDefIsLoaded());
	}

	// Token: 0x06003D6C RID: 15724 RVA: 0x00128828 File Offset: 0x00126A28
	private IEnumerator ShowHeroPowerWhenDefIsLoaded()
	{
		if (this.m_zoomedHero == null)
		{
			yield break;
		}
		while (this.m_heroPowerDefs[this.m_zoomedHero.GetChoiceNum() - 1] == null)
		{
			yield return null;
		}
		FullDef def = this.m_heroPowerDefs[this.m_zoomedHero.GetChoiceNum() - 1];
		this.m_heroPower.SetCardDef(def.GetCardDef());
		this.m_heroPower.SetEntityDef(def.GetEntityDef());
		this.m_heroPower.UpdateAllComponents();
		this.m_heroPower.Show();
		if (UniversalInputManager.UsePhoneUI)
		{
			iTween.MoveTo(this.m_heroPower.gameObject, iTween.Hash(new object[]
			{
				"position",
				DraftDisplay.HERO_POWER_POSITION_PHONE,
				"isLocal",
				true,
				"time",
				0.5f
			}));
		}
		else
		{
			iTween.MoveTo(this.m_heroPower.gameObject, iTween.Hash(new object[]
			{
				"position",
				DraftDisplay.HERO_POWER_POSITION,
				"isLocal",
				true,
				"time",
				0.5f
			}));
		}
		yield break;
	}

	// Token: 0x06003D6D RID: 15725 RVA: 0x00128844 File Offset: 0x00126A44
	private void DestroyOldChoices()
	{
		this.m_animationsComplete = false;
		foreach (DraftDisplay.DraftChoice draftChoice in this.m_choices)
		{
			Actor actor = draftChoice.m_actor;
			if (!(actor == null))
			{
				DraftCardVisual component = actor.GetCollider().gameObject.GetComponent<DraftCardVisual>();
				actor.TurnOffCollider();
				Spell spell = actor.GetSpell(this.GetSpellTypeForRarity(actor.GetEntityDef().GetRarity()));
				if (component.IsChosen())
				{
					if (!actor.GetEntityDef().IsHero())
					{
						this.AddCardToManaCurve(actor.GetEntityDef());
						Spell spell2 = actor.GetSpell(SpellType.SUMMON_OUT_FORGE);
						spell2.AddFinishedCallback(new Spell.FinishedCallback(this.DestroyChoiceOnSpellFinish), actor);
						actor.ActivateSpell(SpellType.SUMMON_OUT_FORGE);
						spell.ActivateState(SpellStateType.DEATH);
						SoundManager.Get().LoadAndPlay("forge_select_card_1");
						this.m_draftDeckTray.GetCardsContent().UpdateCardList(draftChoice.m_cardID, true, actor);
					}
					else
					{
						foreach (HeroLabel heroLabel in this.m_currentLabels)
						{
							heroLabel.FadeOut();
						}
					}
				}
				else
				{
					SoundManager.Get().LoadAndPlay("unselected_cards_dissipate");
					Spell spell3 = actor.GetSpell(SpellType.BURN);
					spell3.AddFinishedCallback(new Spell.FinishedCallback(this.DestroyChoiceOnSpellFinish), actor);
					actor.ActivateSpell(SpellType.BURN);
					if (actor.GetEntityDef().IsHero())
					{
						actor.Hide(true);
					}
					if (spell != null)
					{
						spell.ActivateState(SpellStateType.DEATH);
					}
				}
			}
		}
		base.StartCoroutine(this.CompleteAnims());
		this.m_choices.Clear();
	}

	// Token: 0x06003D6E RID: 15726 RVA: 0x00128A4C File Offset: 0x00126C4C
	private IEnumerator CompleteAnims()
	{
		yield return new WaitForSeconds(0.5f);
		this.m_animationsComplete = true;
		yield break;
	}

	// Token: 0x06003D6F RID: 15727 RVA: 0x00128A68 File Offset: 0x00126C68
	private void DestroyChoiceOnSpellFinish(Spell spell, object actorObject)
	{
		Actor actor = (Actor)actorObject;
		base.StartCoroutine(this.DestroyObjectAfterDelay(actor.gameObject));
	}

	// Token: 0x06003D70 RID: 15728 RVA: 0x00128A90 File Offset: 0x00126C90
	private IEnumerator DestroyObjectAfterDelay(GameObject gameObjectToDestroy)
	{
		yield return new WaitForSeconds(5f);
		Object.Destroy(gameObjectToDestroy);
		yield break;
	}

	// Token: 0x06003D71 RID: 15729 RVA: 0x00128AB4 File Offset: 0x00126CB4
	private void OnFullDefLoaded(string cardID, FullDef def, object userData)
	{
		DraftDisplay.ChoiceCallback choiceCallback = (DraftDisplay.ChoiceCallback)userData;
		choiceCallback.fullDef = def;
		if (def.GetEntityDef().IsHero())
		{
			AssetLoader.Get().LoadActor(ActorNames.GetZoneActor(def.GetEntityDef(), TAG_ZONE.PLAY), new AssetLoader.GameObjectCallback(this.OnActorLoaded), choiceCallback, false);
			AssetLoader.Get().LoadCardPrefab(def.GetEntityDef().GetCardId(), new AssetLoader.GameObjectCallback(this.OnCardDefLoaded), choiceCallback.choiceID, false);
			string heroPowerCardIdFromHero = GameUtils.GetHeroPowerCardIdFromHero(def.GetEntityDef().GetCardId());
			DefLoader.Get().LoadFullDef(heroPowerCardIdFromHero, new DefLoader.LoadDefCallback<FullDef>(this.OnHeroPowerFullDefLoaded), choiceCallback.choiceID);
		}
		else
		{
			AssetLoader.Get().LoadActor(ActorNames.GetHandActor(def.GetEntityDef()), new AssetLoader.GameObjectCallback(this.OnActorLoaded), choiceCallback, false);
		}
	}

	// Token: 0x06003D72 RID: 15730 RVA: 0x00128B90 File Offset: 0x00126D90
	private void OnHeroPowerFullDefLoaded(string cardID, FullDef def, object userData)
	{
		int num = (int)userData;
		this.m_heroPowerDefs[num - 1] = def;
	}

	// Token: 0x06003D73 RID: 15731 RVA: 0x00128BB0 File Offset: 0x00126DB0
	private void OnFullHeroDefLoaded(string cardID, FullDef def, object userData)
	{
		TAG_PREMIUM premium = (TAG_PREMIUM)((int)userData);
		DraftDisplay.LoadHeroActorCallbackInfo callbackData = new DraftDisplay.LoadHeroActorCallbackInfo
		{
			heroFullDef = def,
			premium = premium
		};
		AssetLoader.Get().LoadActor("Card_Play_Hero", new AssetLoader.GameObjectCallback(this.OnHeroActorLoaded), callbackData, false);
	}

	// Token: 0x06003D74 RID: 15732 RVA: 0x00128BF8 File Offset: 0x00126DF8
	private void UpdateInstructionText()
	{
		if (this.GetDraftMode() == DraftDisplay.DraftMode.DRAFTING)
		{
			if (DraftManager.Get().GetSlot() == 0)
			{
				if (!Options.Get().GetBool(Option.HAS_SEEN_FORGE_HERO_CHOICE, false) && UserAttentionManager.CanShowAttentionGrabber("DraftDisplay.UpdateInstructionText:" + Option.HAS_SEEN_FORGE_HERO_CHOICE))
				{
					NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_FORGE_INST1_19"), "VO_INNKEEPER_FORGE_INST1_19", 3f, null);
					Options.Get().SetBool(Option.HAS_SEEN_FORGE_HERO_CHOICE, true);
				}
			}
			else if (DraftManager.Get().GetSlot() == 2 && !Options.Get().GetBool(Option.HAS_SEEN_FORGE_CARD_CHOICE2, false) && UserAttentionManager.CanShowAttentionGrabber("DraftDisplay.UpdateInstructionText:" + Option.HAS_SEEN_FORGE_CARD_CHOICE2))
			{
				NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_FORGE_INST3_21"), "VO_INNKEEPER_FORGE_INST3_21", 3f, null);
				Options.Get().SetBool(Option.HAS_SEEN_FORGE_CARD_CHOICE2, true);
			}
			if (UniversalInputManager.UsePhoneUI)
			{
				if (DraftManager.Get().GetSlot() == 0)
				{
					this.m_PhoneDeckControl.SetMode(ArenaPhoneControl.ControlMode.ChooseHero);
				}
				else
				{
					CollectionDeck draftDeck = DraftManager.Get().GetDraftDeck();
					if (draftDeck.GetTotalCardCount() > 0)
					{
						this.m_PhoneDeckControl.SetMode(ArenaPhoneControl.ControlMode.CardCountViewDeck);
					}
					else
					{
						this.m_PhoneDeckControl.SetMode(ArenaPhoneControl.ControlMode.ChooseCard);
					}
				}
			}
			else
			{
				string text = (DraftManager.Get().GetSlot() != 0) ? GameStrings.Get("GLUE_DRAFT_INSTRUCTIONS") : GameStrings.Get("GLUE_DRAFT_HERO_INSTRUCTIONS");
				this.m_instructionText.Text = text;
			}
			return;
		}
		if (this.GetDraftMode() == DraftDisplay.DraftMode.ACTIVE_DRAFT_DECK)
		{
			if (UniversalInputManager.UsePhoneUI)
			{
				this.m_PhoneDeckControl.SetMode(ArenaPhoneControl.ControlMode.ViewDeck);
			}
			else
			{
				this.m_instructionText.Text = GameStrings.Get("GLUE_DRAFT_MATCH_PROG");
			}
			return;
		}
		this.m_instructionText.Text = string.Empty;
	}

	// Token: 0x06003D75 RID: 15733 RVA: 0x00128DDC File Offset: 0x00126FDC
	private void DoFirstTimeIntro()
	{
		Box.Get().SetToIgnoreFullScreenEffects(true);
		if (!UniversalInputManager.UsePhoneUI)
		{
			this.m_playButton.Disable();
		}
		this.ShowPhonePlayButton(false);
		this.m_retireButton.Disable();
		if (this.m_manaCurve)
		{
			this.m_manaCurve.ResetBars();
		}
		if (!UniversalInputManager.UsePhoneUI)
		{
			StoreManager.Get().StartArenaTransaction(new Store.ExitCallback(this.OnStoreBackButtonPressed), null, true);
		}
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get("GLUE_ARENA_1ST_TIME_HEADER");
		popupInfo.m_text = GameStrings.Get("GLUE_ARENA_1ST_TIME_DESC");
		popupInfo.m_showAlertIcon = false;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
		popupInfo.m_responseCallback = new AlertPopup.ResponseCallback(this.OnFirstTimeIntroOkButtonPressed);
		DialogManager.Get().ShowPopup(popupInfo);
		SoundManager.Get().LoadAndPlay("VO_INNKEEPER_ARENA_INTRO2");
	}

	// Token: 0x06003D76 RID: 15734 RVA: 0x00128EC4 File Offset: 0x001270C4
	private void OnFirstTimeIntroOkButtonPressed(AlertPopup.Response response, object userData)
	{
		StoreManager.Get().HideArenaStore();
		DraftManager.Get().RequestDraftStart();
		Options.Get().SetBool(Option.HAS_SEEN_FORGE, true);
	}

	// Token: 0x06003D77 RID: 15735 RVA: 0x00128EF4 File Offset: 0x001270F4
	private void ShowPurchaseScreen()
	{
		Box.Get().SetToIgnoreFullScreenEffects(true);
		if (!UniversalInputManager.UsePhoneUI)
		{
			this.m_playButton.Disable();
		}
		this.ShowPhonePlayButton(false);
		this.m_retireButton.Disable();
		if (this.m_manaCurve)
		{
			this.m_manaCurve.ResetBars();
		}
		if (DemoMgr.Get().ArenaIs1WinMode())
		{
			Network.PurchaseViaGold(1, 2, 0);
			return;
		}
		StoreManager.Get().StartArenaTransaction(new Store.ExitCallback(this.OnStoreBackButtonPressed), null, false);
	}

	// Token: 0x06003D78 RID: 15736 RVA: 0x00128F84 File Offset: 0x00127184
	private void ShowCurrentlyDraftingScreen()
	{
		this.m_wasDrafting = true;
		ArenaTrayDisplay.Get().ShowPlainPaperBackground();
		StoreManager.Get().HideArenaStore();
		this.UpdateInstructionText();
		this.m_retireButton.Disable();
		if (!UniversalInputManager.UsePhoneUI)
		{
			this.m_playButton.Disable();
		}
		this.ShowPhonePlayButton(false);
		this.LoadAndPositionHeroCard();
	}

	// Token: 0x06003D79 RID: 15737 RVA: 0x00128FE4 File Offset: 0x001271E4
	private IEnumerator ShowActiveDraftScreen()
	{
		StoreManager.Get().HideArenaStore();
		int losses = DraftManager.Get().GetLosses();
		this.DestroyOldChoices();
		this.m_retireButton.Enable();
		this.m_playButton.Enable();
		this.ShowPhonePlayButton(true);
		this.UpdateInstructionText();
		this.LoadAndPositionHeroCard();
		if (this.m_wasDrafting)
		{
			yield return new WaitForSeconds(0.3f);
		}
		ArenaTrayDisplay.Get().UpdateTray();
		if (UserAttentionManager.CanShowAttentionGrabber("DraftDisplay.ShowActiveDraftScreen"))
		{
			if (!Options.Get().GetBool(Option.HAS_SEEN_FORGE_PLAY_MODE, false))
			{
				if (DraftManager.Get().GetWins() == 0 && losses == 0)
				{
					NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_FORGE_COMPLETE_22"), "VO_INNKEEPER_ARENA_COMPLETE", 0f, null);
					Options.Get().SetBool(Option.HAS_SEEN_FORGE_PLAY_MODE, true);
				}
			}
			else if (losses == 2 && !Options.Get().GetBool(Option.HAS_SEEN_FORGE_2LOSS, false))
			{
				NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_FORGE_2LOSS_25"), "VO_INNKEEPER_FORGE_2LOSS_25", 3f, null);
				Options.Get().SetBool(Option.HAS_SEEN_FORGE_2LOSS, true);
			}
			else if (DraftManager.Get().GetWins() == 1 && !Options.Get().GetBool(Option.HAS_SEEN_FORGE_1WIN, false))
			{
				while (GameToastMgr.Get().AreToastsActive())
				{
					yield return null;
				}
				NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, new Vector3(133.1f, NotificationManager.DEPTH, 54.2f), GameStrings.Get("VO_INNKEEPER_FORGE_1WIN"), "VO_INNKEEPER_ARENA_1WIN", 0f, null);
				Options.Get().SetBool(Option.HAS_SEEN_FORGE_1WIN, true);
			}
		}
		yield break;
	}

	// Token: 0x06003D7A RID: 15738 RVA: 0x00129000 File Offset: 0x00127200
	private void ShowDraftRewardsScreen()
	{
		if (!UniversalInputManager.UsePhoneUI)
		{
			this.m_playButton.Disable();
		}
		this.ShowPhonePlayButton(false);
		this.m_retireButton.Disable();
		if (DemoMgr.Get().ArenaIs1WinMode())
		{
			base.StartCoroutine(this.RestartArena());
			return;
		}
		if (DraftManager.Get().DeckWasActiveDuringSession())
		{
			int maxWins = DraftManager.Get().GetMaxWins();
			int wins = DraftManager.Get().GetWins();
			if (wins >= maxWins && !Options.Get().GetBool(Option.HAS_SEEN_FORGE_MAX_WIN, false) && UserAttentionManager.CanShowAttentionGrabber("DraftDisplay.ShowDraftRewardsScreen:" + Option.HAS_SEEN_FORGE_MAX_WIN))
			{
				NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_MAX_ARENA_WINS_04"), "VO_INNKEEPER_MAX_ARENA_WINS_04", 0f, null);
				Options.Get().SetBool(Option.HAS_SEEN_FORGE_MAX_WIN, true);
			}
			ArenaTrayDisplay.Get().UpdateTray(false);
			ArenaTrayDisplay.Get().ActivateKey();
			if (this.m_PhoneDeckControl != null)
			{
				this.m_PhoneDeckControl.SetMode(ArenaPhoneControl.ControlMode.Rewards);
			}
		}
		else
		{
			ArenaTrayDisplay.Get().ShowRewardsOpenAtStart();
		}
		this.LoadAndPositionHeroCard();
	}

	// Token: 0x06003D7B RID: 15739 RVA: 0x00129130 File Offset: 0x00127330
	private IEnumerator RestartArena()
	{
		Debug.LogWarning("Restarting");
		int wins = DraftManager.Get().GetWins();
		if (wins < 5)
		{
			DemoMgr.Get().CreateDemoText(GameStrings.Get("GLUE_BLIZZCON2013_ARENA_NO_PRIZE"), true);
		}
		else if (wins < 9)
		{
			DemoMgr.Get().CreateDemoText(GameStrings.Get("GLUE_BLIZZCON2013_ARENA_PRIZE"), true);
		}
		else if (wins == 9)
		{
			DemoMgr.Get().CreateDemoText(GameStrings.Get("GLUE_BLIZZCON2013_ARENA_GRAND_PRIZE"), true);
		}
		AssetLoader.Get().LoadActor("NumberLabel", new AssetLoader.GameObjectCallback(this.LastArenaWinsLabelLoaded), wins, false);
		this.m_currentLabels = new List<HeroLabel>();
		yield return new WaitForSeconds(6f);
		this.SetDraftMode(DraftDisplay.DraftMode.NO_ACTIVE_DRAFT);
		yield return new WaitForSeconds(2f);
		DraftManager draftMgr = DraftManager.Get();
		Network.AckDraftRewards(draftMgr.GetDraftDeck().ID, draftMgr.GetSlot());
		yield return new WaitForSeconds(1f);
		ArenaTrayDisplay.Get().UpdateTray();
		if (this.m_chosenHero != null)
		{
			Object.Destroy(this.m_chosenHero.gameObject);
		}
		yield return new WaitForSeconds(1f);
		Network.PurchaseViaGold(1, 2, 0);
		yield return new WaitForSeconds(15f);
		if (wins >= 5)
		{
			DemoMgr.Get().MakeDemoTextClickable(true);
			DemoMgr.Get().NextDemoTipIsNewArenaMatch();
		}
		else
		{
			DemoMgr.Get().RemoveDemoTextDialog();
			DemoMgr.Get().CreateDemoText(GameStrings.Get("GLUE_BLIZZCON2013_ARENA"), false, true);
		}
		yield break;
	}

	// Token: 0x06003D7C RID: 15740 RVA: 0x0012914C File Offset: 0x0012734C
	private void LastArenaWinsLabelLoaded(string actorName, GameObject actorObject, object callbackData)
	{
		int num = (int)callbackData;
		actorObject.GetComponent<UberText>().Text = "Last Arena: " + num + " Wins";
		actorObject.transform.position = new Vector3(11.40591f, 1.341853f, 29.28797f);
		actorObject.transform.localScale = new Vector3(15f, 15f, 15f);
	}

	// Token: 0x06003D7D RID: 15741 RVA: 0x001291C0 File Offset: 0x001273C0
	private void LoadAndPositionHeroCard()
	{
		if (this.m_chosenHero != null)
		{
			return;
		}
		if (DraftManager.Get().GetDraftDeck() == null)
		{
			Log.Rachelle.Print("bug 8052, null exception", new object[0]);
			return;
		}
		string heroCardID = DraftManager.Get().GetDraftDeck().HeroCardID;
		if (!string.IsNullOrEmpty(heroCardID))
		{
			DefLoader.Get().LoadFullDef(heroCardID, new DefLoader.LoadDefCallback<FullDef>(this.OnFullHeroDefLoaded), DraftManager.Get().GetDraftDeck().HeroPremium);
		}
	}

	// Token: 0x06003D7E RID: 15742 RVA: 0x0012924C File Offset: 0x0012744C
	private void OnNetCacheReady()
	{
		NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
		NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
		if (netObject.Games.Forge)
		{
			this.m_netCacheReady = true;
			return;
		}
		if (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB))
		{
			return;
		}
		SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
		Error.AddWarningLoc("GLOBAL_FEATURE_DISABLED_TITLE", "GLOBAL_FEATURE_DISABLED_MESSAGE_FORGE", new object[0]);
	}

	// Token: 0x06003D7F RID: 15743 RVA: 0x001292C4 File Offset: 0x001274C4
	private void PositionAndShowChoices()
	{
		this.m_pickArea.enabled = true;
		for (int i = 0; i < this.m_choices.Count; i++)
		{
			DraftDisplay.DraftChoice draftChoice = this.m_choices[i];
			if (draftChoice.m_actor == null)
			{
				Debug.LogWarning(string.Format("DraftDisplay.PositionAndShowChoices(): WARNING found choice with null actor (cardID = {0}). Skipping...", draftChoice.m_cardID));
			}
			else
			{
				bool flag = draftChoice.m_actor.GetEntityDef().IsHero();
				draftChoice.m_actor.transform.position = this.GetCardPosition(i, flag);
				draftChoice.m_actor.Show();
				draftChoice.m_actor.ActivateSpell(SpellType.SUMMON_IN_FORGE);
				TAG_RARITY rarity = draftChoice.m_actor.GetEntityDef().GetRarity();
				draftChoice.m_actor.ActivateSpell(this.GetSpellTypeForRarity(rarity));
				switch (rarity)
				{
				case TAG_RARITY.COMMON:
				case TAG_RARITY.FREE:
					SoundManager.Get().LoadAndPlay("forge_normal_card_appears");
					break;
				case TAG_RARITY.RARE:
				case TAG_RARITY.EPIC:
				case TAG_RARITY.LEGENDARY:
					SoundManager.Get().LoadAndPlay("forge_rarity_card_appears");
					break;
				}
				if (flag)
				{
					if (i == 0 && DemoMgr.Get().ArenaIs1WinMode())
					{
						DemoMgr.Get().CreateDemoText(GameStrings.Get("GLUE_BLIZZCON2013_ARENA"), false, true);
					}
					draftChoice.m_actor.GetHealthObject().Hide();
					GameObject gameObject = Object.Instantiate<GameObject>(this.m_heroLabel);
					gameObject.transform.position = draftChoice.m_actor.GetMeshRenderer().transform.position;
					HeroLabel component = gameObject.GetComponent<HeroLabel>();
					if (UniversalInputManager.UsePhoneUI)
					{
						draftChoice.m_actor.transform.localScale = DraftDisplay.HERO_ACTOR_LOCAL_SCALE_PHONE;
						gameObject.transform.localScale = DraftDisplay.HERO_LABEL_SCALE_PHONE;
					}
					else
					{
						draftChoice.m_actor.transform.localScale = DraftDisplay.HERO_ACTOR_LOCAL_SCALE;
						gameObject.transform.localScale = DraftDisplay.HERO_LABEL_SCALE;
					}
					component.UpdateText(draftChoice.m_actor.GetEntityDef().GetName(), GameStrings.GetClassName(draftChoice.m_actor.GetEntityDef().GetClass()).ToUpper());
					this.m_currentLabels.Add(component);
				}
				else if (UniversalInputManager.UsePhoneUI)
				{
					draftChoice.m_actor.transform.localScale = DraftDisplay.CHOICE_ACTOR_LOCAL_SCALE_PHONE;
				}
				else
				{
					draftChoice.m_actor.transform.localScale = DraftDisplay.CHOICE_ACTOR_LOCAL_SCALE;
				}
			}
		}
		this.EnableBackButton(true);
		base.StartCoroutine(this.RunAutoDraftCheat());
		this.m_pickArea.enabled = false;
	}

	// Token: 0x06003D80 RID: 15744 RVA: 0x00129560 File Offset: 0x00127760
	private bool CanAutoDraft()
	{
		return ApplicationMgr.IsInternal() && Vars.Key("Arena.AutoDraft").GetBool(false);
	}

	// Token: 0x06003D81 RID: 15745 RVA: 0x00129594 File Offset: 0x00127794
	public IEnumerator RunAutoDraftCheat()
	{
		if (!this.CanAutoDraft())
		{
			yield break;
		}
		int frameStart = Time.frameCount;
		while (GameUtils.IsAnyTransitionActive() && Time.frameCount - frameStart < 120)
		{
			yield return null;
		}
		List<DraftCardVisual> draftChoices = this.GetCardVisuals();
		if (draftChoices != null && draftChoices.Count > 0)
		{
			Time.timeScale = SceneDebugger.Get().m_MaxTimeScale;
			int pickedIndex = Random.Range(0, draftChoices.Count - 1);
			DraftCardVisual visual = draftChoices[pickedIndex];
			frameStart = Time.frameCount;
			while (visual.GetActor() == null && Time.frameCount - frameStart < 120)
			{
				yield return null;
			}
			if (visual.GetActor() != null)
			{
				string info = string.Format("autodraft'ing {0}\nto stop, use cmd 'autodraft off'", visual.GetActor().GetEntityDef().GetStringTag(GAME_TAG.CARDNAME));
				UIStatus.Get().AddInfo(info, 2f);
				draftChoices[pickedIndex].ChooseThisCard();
			}
		}
		yield break;
	}

	// Token: 0x06003D82 RID: 15746 RVA: 0x001295B0 File Offset: 0x001277B0
	private Vector3 GetCardPosition(int cardChoice, bool isHero)
	{
		float num = this.m_pickArea.bounds.center.x - this.m_pickArea.bounds.extents.x;
		float x = this.m_pickArea.bounds.size.x;
		float num2 = x / 3f;
		float num3 = (this.m_choices.Count != 2) ? (-num2 / 2f) : 0f;
		float num4 = 0f;
		if (isHero)
		{
			num4 = 1f;
		}
		return new Vector3(num + (float)(cardChoice + 1) * num2 + num3, this.m_pickArea.transform.position.y, this.m_pickArea.transform.position.z + num4);
	}

	// Token: 0x06003D83 RID: 15747 RVA: 0x0012969C File Offset: 0x0012789C
	private SpellType GetSpellTypeForRarity(TAG_RARITY rarity)
	{
		switch (rarity)
		{
		case TAG_RARITY.RARE:
			return SpellType.BURST_RARE;
		case TAG_RARITY.EPIC:
			return SpellType.BURST_EPIC;
		case TAG_RARITY.LEGENDARY:
			return SpellType.BURST_LEGENDARY;
		default:
			return SpellType.BURST_COMMON;
		}
	}

	// Token: 0x06003D84 RID: 15748 RVA: 0x001296D0 File Offset: 0x001278D0
	private void OnHeroActorLoaded(string actorName, GameObject actorObject, object callbackData)
	{
		if (actorObject == null)
		{
			Debug.LogWarning(string.Format("DraftDisplay.OnHeroActorLoaded() - FAILED to load actor \"{0}\"", actorName));
			return;
		}
		Actor component = actorObject.GetComponent<Actor>();
		if (component == null)
		{
			Debug.LogWarning(string.Format("DraftDisplay.OnActorLoaded() - ERROR actor \"{0}\" has no Actor component", actorName));
			return;
		}
		DraftDisplay.LoadHeroActorCallbackInfo loadHeroActorCallbackInfo = callbackData as DraftDisplay.LoadHeroActorCallbackInfo;
		FullDef heroFullDef = loadHeroActorCallbackInfo.heroFullDef;
		EntityDef entityDef = heroFullDef.GetEntityDef();
		CardDef cardDef = heroFullDef.GetCardDef();
		this.m_chosenHero = component;
		this.m_chosenHero.SetPremium(loadHeroActorCallbackInfo.premium);
		this.m_chosenHero.SetEntityDef(entityDef);
		this.m_chosenHero.SetCardDef(cardDef);
		this.m_chosenHero.UpdateAllComponents();
		this.m_chosenHero.gameObject.name = cardDef.name + "_actor";
		this.m_chosenHero.transform.parent = this.m_socketHeroBone.transform;
		this.m_chosenHero.transform.localPosition = Vector3.zero;
		this.m_chosenHero.transform.localScale = Vector3.one;
		if (UniversalInputManager.UsePhoneUI)
		{
			SceneUtils.SetLayer(this.m_chosenHero.gameObject, GameLayer.IgnoreFullScreenEffects);
		}
		this.m_chosenHero.GetHealthObject().Hide();
	}

	// Token: 0x06003D85 RID: 15749 RVA: 0x00129810 File Offset: 0x00127A10
	private void OnActorLoaded(string actorName, GameObject actorObject, object callbackData)
	{
		if (actorObject == null)
		{
			Debug.LogWarning(string.Format("DraftDisplay.OnActorLoaded() - FAILED to load actor \"{0}\"", actorName));
			return;
		}
		Actor component = actorObject.GetComponent<Actor>();
		if (component == null)
		{
			Debug.LogWarning(string.Format("DraftDisplay.OnActorLoaded() - ERROR actor \"{0}\" has no Actor component", actorName));
			return;
		}
		DraftDisplay.ChoiceCallback choiceCallback = (DraftDisplay.ChoiceCallback)callbackData;
		FullDef fullDef = choiceCallback.fullDef;
		EntityDef entityDef = fullDef.GetEntityDef();
		CardDef cardDef = fullDef.GetCardDef();
		DraftDisplay.DraftChoice draftChoice = this.m_choices.Find((DraftDisplay.DraftChoice obj) => obj.m_cardID.Equals(entityDef.GetCardId()));
		if (draftChoice == null)
		{
			Debug.LogWarning(string.Format("DraftDisplay.OnActorLoaded(): Could not find draft choice {0} (cardID = {1}) in m_choices.", entityDef.GetName(), entityDef.GetCardId()));
			Object.Destroy(component);
			return;
		}
		draftChoice.m_actor = component;
		draftChoice.m_actor.SetPremium(draftChoice.m_premium);
		draftChoice.m_actor.SetEntityDef(entityDef);
		draftChoice.m_actor.SetCardDef(cardDef);
		draftChoice.m_actor.UpdateAllComponents();
		draftChoice.m_actor.gameObject.name = cardDef.name + "_actor";
		draftChoice.m_actor.ContactShadow(true);
		DraftCardVisual draftCardVisual = draftChoice.m_actor.GetCollider().gameObject.AddComponent<DraftCardVisual>();
		draftCardVisual.SetActor(draftChoice.m_actor);
		draftCardVisual.SetChoiceNum(choiceCallback.choiceID);
		if (this.HaveActorsForAllChoices())
		{
			this.PositionAndShowChoices();
		}
		else
		{
			draftChoice.m_actor.Hide();
		}
	}

	// Token: 0x06003D86 RID: 15750 RVA: 0x001299A0 File Offset: 0x00127BA0
	private void OnCardDefLoaded(string cardID, GameObject cardObject, object callbackData)
	{
		CardDef component = cardObject.GetComponent<CardDef>();
		foreach (EmoteEntryDef emoteEntryDef in component.m_EmoteDefs)
		{
			if (emoteEntryDef.m_emoteType == EmoteType.PICKED)
			{
				AssetLoader.Get().LoadSpell(emoteEntryDef.m_emoteSoundSpellPath, new AssetLoader.GameObjectCallback(this.OnStartEmoteLoaded), callbackData, false);
			}
		}
	}

	// Token: 0x06003D87 RID: 15751 RVA: 0x00129A28 File Offset: 0x00127C28
	private void OnStartEmoteLoaded(string name, GameObject go, object callbackData)
	{
		CardSoundSpell cardSoundSpell = null;
		if (go != null)
		{
			cardSoundSpell = go.GetComponent<CardSoundSpell>();
		}
		this.m_skipHeroEmotes |= (cardSoundSpell == null);
		if (this.m_skipHeroEmotes)
		{
			Object.Destroy(go);
		}
		else
		{
			int num = (int)callbackData;
			this.m_heroEmotes[num - 1] = cardSoundSpell;
		}
	}

	// Token: 0x06003D88 RID: 15752 RVA: 0x00129A88 File Offset: 0x00127C88
	private bool HaveActorsForAllChoices()
	{
		foreach (DraftDisplay.DraftChoice draftChoice in this.m_choices)
		{
			if (!(draftChoice.m_actor != null))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06003D89 RID: 15753 RVA: 0x00129AFC File Offset: 0x00127CFC
	private void InitManaCurve()
	{
		CollectionDeck draftDeck = DraftManager.Get().GetDraftDeck();
		if (draftDeck == null)
		{
			return;
		}
		foreach (CollectionDeckSlot collectionDeckSlot in draftDeck.GetSlots())
		{
			EntityDef entityDef = DefLoader.Get().GetEntityDef(collectionDeckSlot.CardID);
			for (int i = 0; i < collectionDeckSlot.Count; i++)
			{
				this.AddCardToManaCurve(entityDef);
			}
		}
	}

	// Token: 0x06003D8A RID: 15754 RVA: 0x00129B94 File Offset: 0x00127D94
	private void OnStoreBackButtonPressed(bool authorizationBackButtonPressed, object userData)
	{
		this.ExitDraftScene();
	}

	// Token: 0x06003D8B RID: 15755 RVA: 0x00129B9C File Offset: 0x00127D9C
	private bool OnNavigateBack()
	{
		if (this.IsInHeroSelectMode())
		{
			this.DoHeroCancelAnimation();
			return false;
		}
		if (ArenaTrayDisplay.Get() == null)
		{
			return false;
		}
		ArenaTrayDisplay.Get().KeyFXCancel();
		this.ExitDraftScene();
		return true;
	}

	// Token: 0x06003D8C RID: 15756 RVA: 0x00129BDF File Offset: 0x00127DDF
	private void BackButtonPress(UIEvent e)
	{
		Navigation.GoBack();
	}

	// Token: 0x06003D8D RID: 15757 RVA: 0x00129BE8 File Offset: 0x00127DE8
	private void ExitDraftScene()
	{
		GameMgr.Get().CancelFindGame();
		if (!UniversalInputManager.UsePhoneUI)
		{
			this.m_playButton.Disable();
		}
		this.ShowPhonePlayButton(false);
		StoreManager.Get().HideArenaStore();
		SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
	}

	// Token: 0x06003D8E RID: 15758 RVA: 0x00129C38 File Offset: 0x00127E38
	private void PlayButtonPress(UIEvent e)
	{
		if (!UniversalInputManager.UsePhoneUI)
		{
			this.m_playButton.Disable();
		}
		this.ShowPhonePlayButton(false);
		DraftManager.Get().FindGame();
		PresenceMgr.Get().SetStatus(new Enum[]
		{
			PresenceStatus.ARENA_QUEUE
		});
	}

	// Token: 0x06003D8F RID: 15759 RVA: 0x00129C8C File Offset: 0x00127E8C
	private void RetireButtonPress(UIEvent e)
	{
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get("GLUE_FORGE_RETIRE_WARNING_HEADER");
		popupInfo.m_text = GameStrings.Get("GLUE_FORGE_RETIRE_WARNING_DESC");
		popupInfo.m_showAlertIcon = false;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
		popupInfo.m_responseCallback = new AlertPopup.ResponseCallback(this.OnRetirePopupResponse);
		DialogManager.Get().ShowPopup(popupInfo);
	}

	// Token: 0x06003D90 RID: 15760 RVA: 0x00129CEC File Offset: 0x00127EEC
	private void OnRetirePopupResponse(AlertPopup.Response response, object userData)
	{
		if (response == AlertPopup.Response.CANCEL)
		{
			return;
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			SlidingTray component = this.m_draftDeckTray.gameObject.GetComponent<SlidingTray>();
			component.HideTray();
		}
		DraftManager draftManager = DraftManager.Get();
		this.m_retireButton.Disable();
		Network.RetireDraftDeck(draftManager.GetDraftDeck().ID, draftManager.GetSlot());
	}

	// Token: 0x06003D91 RID: 15761 RVA: 0x00129D50 File Offset: 0x00127F50
	private void ManaCurveOver(UIEvent e)
	{
		TooltipZone component = this.m_manaCurve.GetComponent<TooltipZone>();
		component.ShowTooltip(GameStrings.Get("GLUE_FORGE_MANATIP_HEADER"), GameStrings.Get("GLUE_FORGE_MANATIP_DESC"), (!UniversalInputManager.UsePhoneUI) ? KeywordHelpPanel.FORGE_SCALE : KeywordHelpPanel.BOX_SCALE, true);
	}

	// Token: 0x06003D92 RID: 15762 RVA: 0x00129DA8 File Offset: 0x00127FA8
	private void ManaCurveOut(UIEvent e)
	{
		TooltipZone component = this.m_manaCurve.GetComponent<TooltipZone>();
		component.HideTooltip();
	}

	// Token: 0x06003D93 RID: 15763 RVA: 0x00129DC8 File Offset: 0x00127FC8
	private void DeckHeaderOver(UIEvent e)
	{
		TooltipZone tooltipZone = this.m_draftDeckTray.GetTooltipZone();
		tooltipZone.ShowTooltip(GameStrings.Get("GLUE_ARENA_DECK_TOOLTIP_HEADER"), GameStrings.Get("GLUE_ARENA_DECK_TOOLTIP"), KeywordHelpPanel.FORGE_SCALE, true);
	}

	// Token: 0x06003D94 RID: 15764 RVA: 0x00129E07 File Offset: 0x00128007
	private void DeckHeaderOut(UIEvent e)
	{
		this.m_draftDeckTray.GetTooltipZone().HideTooltip();
	}

	// Token: 0x06003D95 RID: 15765 RVA: 0x00129E1C File Offset: 0x0012801C
	private void SetupBackButton()
	{
		if (DemoMgr.Get().CantExitArena())
		{
			this.m_backButton.SetText(string.Empty);
			return;
		}
		this.m_backButton.SetText(GameStrings.Get("GLOBAL_BACK"));
		this.m_backButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.BackButtonPress));
	}

	// Token: 0x06003D96 RID: 15766 RVA: 0x00129E77 File Offset: 0x00128077
	private void EnableBackButton(bool buttonEnabled)
	{
		this.m_backButton.enabled = buttonEnabled;
	}

	// Token: 0x06003D97 RID: 15767 RVA: 0x00129E88 File Offset: 0x00128088
	private void SetupRetireButton()
	{
		if (DemoMgr.Get().CantExitArena())
		{
			this.m_retireButton.SetText(string.Empty);
			return;
		}
		this.m_retireButton.SetText(GameStrings.Get("GLUE_DRAFT_RETIRE_BUTTON"));
		this.m_retireButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.RetireButtonPress));
	}

	// Token: 0x06003D98 RID: 15768 RVA: 0x00129EE4 File Offset: 0x001280E4
	private void ShowPhonePlayButton(bool show)
	{
		if (this.m_PhonePlayButtonTray == null)
		{
			return;
		}
		SlidingTray component = this.m_PhonePlayButtonTray.GetComponent<SlidingTray>();
		if (component == null)
		{
			return;
		}
		component.ToggleTraySlider(show, null, true);
	}

	// Token: 0x040026E7 RID: 9959
	public Collider m_pickArea;

	// Token: 0x040026E8 RID: 9960
	public UberText m_instructionText;

	// Token: 0x040026E9 RID: 9961
	public UberText m_forgeLabel;

	// Token: 0x040026EA RID: 9962
	public DraftManaCurve m_manaCurve;

	// Token: 0x040026EB RID: 9963
	public GameObject m_heroLabel;

	// Token: 0x040026EC RID: 9964
	public Spell m_DeckCompleteSpell;

	// Token: 0x040026ED RID: 9965
	public float m_DeckCardBarFlareUpDelay;

	// Token: 0x040026EE RID: 9966
	public PegUIElement m_heroClickCatcher;

	// Token: 0x040026EF RID: 9967
	public DraftDeckTray m_draftDeckTray;

	// Token: 0x040026F0 RID: 9968
	[CustomEditField(Sections = "Buttons")]
	public UIBButton m_backButton;

	// Token: 0x040026F1 RID: 9969
	public StandardPegButtonNew m_retireButton;

	// Token: 0x040026F2 RID: 9970
	public PlayButton m_playButton;

	// Token: 0x040026F3 RID: 9971
	[CustomEditField(Sections = "Bones")]
	public Transform m_bigHeroBone;

	// Token: 0x040026F4 RID: 9972
	public Transform m_socketHeroBone;

	// Token: 0x040026F5 RID: 9973
	[CustomEditField(Sections = "Phone")]
	public GameObject m_PhonePlayButtonTray;

	// Token: 0x040026F6 RID: 9974
	public Transform m_PhoneBackButtonBone;

	// Token: 0x040026F7 RID: 9975
	public Transform m_PhoneDeckTrayHiddenBone;

	// Token: 0x040026F8 RID: 9976
	public GameObject m_Phone3WayButtonRoot;

	// Token: 0x040026F9 RID: 9977
	public GameObject m_PhoneChooseHero;

	// Token: 0x040026FA RID: 9978
	public GameObject m_PhoneLargeViewDeckButton;

	// Token: 0x040026FB RID: 9979
	public ArenaPhoneControl m_PhoneDeckControl;

	// Token: 0x040026FC RID: 9980
	private static readonly Vector3 CHOICE_ACTOR_LOCAL_SCALE = new Vector3(7.2f, 7.2f, 7.2f);

	// Token: 0x040026FD RID: 9981
	private static readonly Vector3 HERO_ACTOR_LOCAL_SCALE = new Vector3(8.285825f, 8.285825f, 8.285825f);

	// Token: 0x040026FE RID: 9982
	private static readonly Vector3 HERO_LABEL_SCALE = new Vector3(8f, 8f, 8f);

	// Token: 0x040026FF RID: 9983
	private static readonly Vector3 HERO_CONFIRM_BUTTON_POSITION = new Vector3(0.03024703f, 107.4205f, -6.346496f);

	// Token: 0x04002700 RID: 9984
	private static readonly Vector3 HERO_CONFIRM_BUTTON_SCALE = new Vector3(3.28f, 3.28f, 3.28f);

	// Token: 0x04002701 RID: 9985
	private static readonly Vector3 HERO_POWER_START_POSITION = new Vector3(0f, 0f, -0.3410472f);

	// Token: 0x04002702 RID: 9986
	private static readonly Vector3 HERO_POWER_POSITION = new Vector3(1.40873f, 0f, -0.3410472f);

	// Token: 0x04002703 RID: 9987
	private static readonly Vector3 HERO_POWER_SCALE = new Vector3(0.3419997f, 0.3419997f, 0.3419997f);

	// Token: 0x04002704 RID: 9988
	private static readonly Vector3 CHOICE_ACTOR_LOCAL_SCALE_PHONE = new Vector3(14.5f, 14.5f, 14.5f);

	// Token: 0x04002705 RID: 9989
	private static readonly Vector3 HERO_ACTOR_LOCAL_SCALE_PHONE = new Vector3(15.5f, 15.5f, 15.5f);

	// Token: 0x04002706 RID: 9990
	private static readonly Vector3 HERO_LABEL_SCALE_PHONE = new Vector3(15f, 15f, 15f);

	// Token: 0x04002707 RID: 9991
	private static readonly Vector3 HERO_CONFIRM_BUTTON_POSITION_PHONE = new Vector3(-4.27f, 113f, -6f);

	// Token: 0x04002708 RID: 9992
	private static readonly Vector3 HERO_CONFIRM_BUTTON_SCALE_PHONE = new Vector3(4f, 4f, 4f);

	// Token: 0x04002709 RID: 9993
	private static readonly Vector3 HERO_POWER_START_POSITION_PHONE = new Vector3(1.6f, 0.3f, -0.15f);

	// Token: 0x0400270A RID: 9994
	private static readonly Vector3 HERO_POWER_POSITION_PHONE = new Vector3(1.07f, 0.3f, -0.15f);

	// Token: 0x0400270B RID: 9995
	private static readonly Vector3 HERO_POWER_SCALE_PHONE = new Vector3(0.5f, 0.5f, 0.5f);

	// Token: 0x0400270C RID: 9996
	private static DraftDisplay s_instance;

	// Token: 0x0400270D RID: 9997
	private List<DraftDisplay.DraftChoice> m_choices = new List<DraftDisplay.DraftChoice>();

	// Token: 0x0400270E RID: 9998
	private FullDef[] m_heroPowerDefs = new FullDef[3];

	// Token: 0x0400270F RID: 9999
	private DraftDisplay.DraftMode m_currentMode;

	// Token: 0x04002710 RID: 10000
	private NormalButton m_confirmButton;

	// Token: 0x04002711 RID: 10001
	private Actor m_heroPower;

	// Token: 0x04002712 RID: 10002
	private bool m_netCacheReady;

	// Token: 0x04002713 RID: 10003
	private bool m_questsHandled;

	// Token: 0x04002714 RID: 10004
	private Actor m_chosenHero;

	// Token: 0x04002715 RID: 10005
	private bool m_animationsComplete = true;

	// Token: 0x04002716 RID: 10006
	private List<HeroLabel> m_currentLabels = new List<HeroLabel>();

	// Token: 0x04002717 RID: 10007
	private CardSoundSpell[] m_heroEmotes = new CardSoundSpell[3];

	// Token: 0x04002718 RID: 10008
	private bool m_skipHeroEmotes;

	// Token: 0x04002719 RID: 10009
	private bool m_isHeroAnimating;

	// Token: 0x0400271A RID: 10010
	private DraftCardVisual m_zoomedHero;

	// Token: 0x0400271B RID: 10011
	private bool m_wasDrafting;

	// Token: 0x0400271C RID: 10012
	private List<Achievement> m_newlyCompletedAchieves = new List<Achievement>();

	// Token: 0x0400271D RID: 10013
	private bool m_firstTimeIntroComplete;

	// Token: 0x02000529 RID: 1321
	public enum DraftMode
	{
		// Token: 0x0400271F RID: 10015
		INVALID,
		// Token: 0x04002720 RID: 10016
		NO_ACTIVE_DRAFT,
		// Token: 0x04002721 RID: 10017
		DRAFTING,
		// Token: 0x04002722 RID: 10018
		ACTIVE_DRAFT_DECK,
		// Token: 0x04002723 RID: 10019
		IN_REWARDS
	}

	// Token: 0x02000802 RID: 2050
	private class ChoiceCallback
	{
		// Token: 0x04003650 RID: 13904
		public FullDef fullDef;

		// Token: 0x04003651 RID: 13905
		public int choiceID;

		// Token: 0x04003652 RID: 13906
		public int slot;
	}

	// Token: 0x02000803 RID: 2051
	private class LoadHeroActorCallbackInfo
	{
		// Token: 0x04003653 RID: 13907
		public FullDef heroFullDef;

		// Token: 0x04003654 RID: 13908
		public TAG_PREMIUM premium;
	}

	// Token: 0x02000804 RID: 2052
	private class DraftChoice
	{
		// Token: 0x04003655 RID: 13909
		public string m_cardID = string.Empty;

		// Token: 0x04003656 RID: 13910
		public TAG_PREMIUM m_premium;

		// Token: 0x04003657 RID: 13911
		public Actor m_actor;
	}
}
