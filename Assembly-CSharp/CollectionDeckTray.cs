using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000383 RID: 899
[CustomEditClass]
public class CollectionDeckTray : MonoBehaviour
{
	// Token: 0x06002DB7 RID: 11703 RVA: 0x000E5430 File Offset: 0x000E3630
	private void Awake()
	{
		CollectionDeckTray.s_instance = this;
		if (base.gameObject.GetComponent<AudioSource>() == null)
		{
			base.gameObject.AddComponent<AudioSource>();
		}
		if (this.m_scrollbar != null)
		{
			if (SceneMgr.Get().GetMode() == SceneMgr.Mode.TAVERN_BRAWL && !UniversalInputManager.UsePhoneUI)
			{
				Vector3 center = this.m_scrollbar.m_ScrollBounds.center;
				center.z = 3f;
				this.m_scrollbar.m_ScrollBounds.center = center;
				Vector3 size = this.m_scrollbar.m_ScrollBounds.size;
				size.z = 47.67f;
				this.m_scrollbar.m_ScrollBounds.size = size;
				if (this.m_cardsContent != null && this.m_cardsContent.m_deckCompleteHighlight != null)
				{
					Vector3 localPosition = this.m_cardsContent.m_deckCompleteHighlight.transform.localPosition;
					localPosition.z = -34.15f;
					this.m_cardsContent.m_deckCompleteHighlight.transform.localPosition = localPosition;
				}
			}
			this.m_scrollbar.Enable(false);
			this.m_scrollbar.AddTouchScrollStartedListener(new UIBScrollable.OnTouchScrollStarted(this.OnTouchScrollStarted));
			this.m_scrollbar.AddTouchScrollEndedListener(new UIBScrollable.OnTouchScrollEnded(this.OnTouchScrollEnded));
		}
		this.m_contents[CollectionDeckTray.DeckContentTypes.Decks] = this.m_decksContent;
		this.m_contents[CollectionDeckTray.DeckContentTypes.Cards] = this.m_cardsContent;
		if (this.m_heroSkinContent != null)
		{
			this.m_contents[CollectionDeckTray.DeckContentTypes.HeroSkin] = this.m_heroSkinContent;
			this.m_heroSkinContent.RegisterHeroAssignedListener(new DeckTrayHeroSkinContent.HeroAssigned(this.OnHeroAssigned));
		}
		if (this.m_cardBackContent != null)
		{
			this.m_contents[CollectionDeckTray.DeckContentTypes.CardBack] = this.m_cardBackContent;
		}
		this.m_cardsContent.RegisterCardTileHeldListener(new DeckTrayCardListContent.CardTileHeld(this.OnCardTileHeld));
		this.m_cardsContent.RegisterCardTilePressListener(new DeckTrayCardListContent.CardTilePress(this.OnCardTilePress));
		this.m_cardsContent.RegisterCardTileTapListener(new DeckTrayCardListContent.CardTileTap(this.OnCardTileTap));
		this.m_cardsContent.RegisterCardTileOverListener(new DeckTrayCardListContent.CardTileOver(this.OnCardTileOver));
		this.m_cardsContent.RegisterCardTileOutListener(new DeckTrayCardListContent.CardTileOut(this.OnCardTileOut));
		this.m_cardsContent.RegisterCardTileReleaseListener(new DeckTrayCardListContent.CardTileRelease(this.OnCardTileRelease));
		this.m_cardsContent.RegisterCardCountUpdated(new DeckTrayCardListContent.CardCountChanged(this.OnCardCountUpdated));
		this.m_decksContent.RegisterDeckCountUpdated(new DeckTrayDeckListContent.DeckCountChanged(this.OnDeckCountUpdated));
		this.m_decksContent.RegisterBusyWithDeck(new DeckTrayDeckListContent.BusyWithDeck(this.OnBusyWithDeck));
		this.SetMyDecksLabelText(GameStrings.Get((SceneMgr.Get().GetMode() != SceneMgr.Mode.TAVERN_BRAWL) ? "GLUE_COLLECTION_MY_DECKS" : "GLUE_COLLECTION_DECK"));
		this.m_doneButton.SetText(GameStrings.Get("GLOBAL_BACK"));
		this.m_doneButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.DoneButtonPress));
		CollectionManager.Get().RegisterTaggedDeckChanged(new CollectionManager.OnTaggedDeckChanged(this.OnTaggedDeckChanged));
		CollectionInputMgr.Get().SetScrollbar(this.m_scrollbar);
		CollectionManagerDisplay.Get().UpdateCurrentPageCardLocks(true);
		CollectionManagerDisplay.Get().RegisterSwitchViewModeListener(new CollectionManagerDisplay.OnSwitchViewMode(this.OnCMViewModeChanged));
		Navigation.Push(new Navigation.NavigateBackHandler(this.OnBackOutOfCollectionScreen));
		foreach (CollectionDeckTray.DeckContentScroll deckContentScroll in this.m_scrollables)
		{
			deckContentScroll.SaveStartPosition();
		}
	}

	// Token: 0x06002DB8 RID: 11704 RVA: 0x000E57DC File Offset: 0x000E39DC
	private void OnDestroy()
	{
		CollectionManager collectionManager = CollectionManager.Get();
		if (collectionManager != null)
		{
			collectionManager.RemoveTaggedDeckChanged(new CollectionManager.OnTaggedDeckChanged(this.OnTaggedDeckChanged));
			collectionManager.DoneEditing();
		}
		CollectionDeckTray.s_instance = null;
	}

	// Token: 0x06002DB9 RID: 11705 RVA: 0x000E5813 File Offset: 0x000E3A13
	private void Start()
	{
		SoundManager.Get().Load("panel_slide_off_deck_creation_screen");
	}

	// Token: 0x06002DBA RID: 11706 RVA: 0x000E5828 File Offset: 0x000E3A28
	public bool CanPickupCard()
	{
		CollectionDeckTray.DeckContentTypes currentContentType = this.GetCurrentContentType();
		CollectionManagerDisplay.ViewMode viewMode = CollectionManagerDisplay.Get().GetViewMode();
		return (currentContentType == CollectionDeckTray.DeckContentTypes.Cards && viewMode == CollectionManagerDisplay.ViewMode.CARDS) || (currentContentType == CollectionDeckTray.DeckContentTypes.CardBack && viewMode == CollectionManagerDisplay.ViewMode.CARD_BACKS) || (currentContentType == CollectionDeckTray.DeckContentTypes.HeroSkin && viewMode == CollectionManagerDisplay.ViewMode.HERO_SKINS);
	}

	// Token: 0x06002DBB RID: 11707 RVA: 0x000E5873 File Offset: 0x000E3A73
	public static CollectionDeckTray Get()
	{
		return CollectionDeckTray.s_instance;
	}

	// Token: 0x06002DBC RID: 11708 RVA: 0x000E587A File Offset: 0x000E3A7A
	public void Initialize()
	{
		this.SetTrayMode(CollectionDeckTray.DeckContentTypes.Decks);
	}

	// Token: 0x06002DBD RID: 11709 RVA: 0x000E5883 File Offset: 0x000E3A83
	public void Unload()
	{
		CollectionInputMgr.Get().SetScrollbar(null);
	}

	// Token: 0x06002DBE RID: 11710 RVA: 0x000E5890 File Offset: 0x000E3A90
	public bool AddCard(EntityDef cardEntityDef, TAG_PREMIUM premium, DeckTrayDeckTileVisual deckTileToRemove, bool playSound, Actor animateActor = null)
	{
		return this.GetCardsContent().AddCard(cardEntityDef, premium, deckTileToRemove, playSound, animateActor);
	}

	// Token: 0x06002DBF RID: 11711 RVA: 0x000E58AF File Offset: 0x000E3AAF
	public int RemoveClosestInvalidCard(EntityDef entityDef, int sameRemoveCount)
	{
		return this.GetCardsContent().RemoveClosestInvalidCard(entityDef, sameRemoveCount);
	}

	// Token: 0x06002DC0 RID: 11712 RVA: 0x000E58C0 File Offset: 0x000E3AC0
	public bool SetCardBack(Actor actor)
	{
		CollectionCardBack component = actor.gameObject.GetComponent<CollectionCardBack>();
		return !(component == null) && this.GetCardBackContent().SetNewCardBack(component.GetCardBackId(), actor.gameObject);
	}

	// Token: 0x06002DC1 RID: 11713 RVA: 0x000E58FE File Offset: 0x000E3AFE
	public void FlashDeckTemplateHighlight()
	{
		if (this.m_deckTemplateChosenGlow != null)
		{
			this.m_deckTemplateChosenGlow.SendEvent("Flash");
		}
	}

	// Token: 0x06002DC2 RID: 11714 RVA: 0x000E5921 File Offset: 0x000E3B21
	public void SetHeroSkin(Actor actor)
	{
		this.GetHeroSkinContent().SetNewHeroSkin(actor);
	}

	// Token: 0x06002DC3 RID: 11715 RVA: 0x000E5930 File Offset: 0x000E3B30
	public bool HandleDeletedCardDeckUpdate(string cardID)
	{
		if (!this.IsShowingDeckContents())
		{
			return false;
		}
		this.GetCardsContent().UpdateCardList(cardID, true, null);
		CollectionManagerDisplay.Get().UpdateCurrentPageCardLocks(true);
		return true;
	}

	// Token: 0x06002DC4 RID: 11716 RVA: 0x000E5964 File Offset: 0x000E3B64
	public bool RemoveCard(string cardID, TAG_PREMIUM premium, bool valid)
	{
		bool result = false;
		CollectionDeck taggedDeck = CollectionManager.Get().GetTaggedDeck(CollectionManager.DeckTag.Editing);
		if (taggedDeck != null)
		{
			result = taggedDeck.RemoveCard(cardID, premium, valid, false);
			this.HandleDeletedCardDeckUpdate(cardID);
		}
		return result;
	}

	// Token: 0x06002DC5 RID: 11717 RVA: 0x000E599C File Offset: 0x000E3B9C
	public void ShowDeck(CollectionManagerDisplay.ViewMode viewMode, long deckID, bool isNewDeck)
	{
		CollectionManager.Get().StartEditingDeck(CollectionManager.DeckTag.Editing, deckID, isNewDeck);
		if ((viewMode == CollectionManagerDisplay.ViewMode.HERO_SKINS && !CollectionManagerDisplay.Get().CanViewHeroSkins()) || (viewMode == CollectionManagerDisplay.ViewMode.CARD_BACKS && !CollectionManagerDisplay.Get().CanViewCardBacks()))
		{
			viewMode = CollectionManagerDisplay.ViewMode.CARDS;
			CollectionManagerDisplay.Get().SetViewMode(CollectionManagerDisplay.ViewMode.CARDS, null);
		}
		CollectionDeckTray.DeckContentTypes contentTypeFromViewMode = this.GetContentTypeFromViewMode(viewMode);
		this.SetTrayMode(contentTypeFromViewMode);
		if (SceneMgr.Get().GetMode() != SceneMgr.Mode.TAVERN_BRAWL)
		{
			Navigation.Push(new Navigation.NavigateBackHandler(this.OnBackOutOfDeckContents));
		}
		if (CollectionManager.Get().ShouldShowWildToStandardTutorial(false))
		{
			CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
			if (editedDeck.IsWild && CollectionManagerDisplay.Get().GetViewMode() != CollectionManagerDisplay.ViewMode.DECK_TEMPLATE)
			{
				CollectionManagerDisplay.Get().ShowConvertTutorial(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS);
			}
		}
	}

	// Token: 0x06002DC6 RID: 11718 RVA: 0x000E5A69 File Offset: 0x000E3C69
	public void EnterEditDeckModeForTavernBrawl()
	{
		Navigation.Push(new Navigation.NavigateBackHandler(this.OnBackOutOfDeckContents));
		this.UpdateDoneButtonText();
	}

	// Token: 0x06002DC7 RID: 11719 RVA: 0x000E5A82 File Offset: 0x000E3C82
	public void ExitEditDeckModeForTavernBrawl()
	{
		this.UpdateDoneButtonText();
	}

	// Token: 0x06002DC8 RID: 11720 RVA: 0x000E5A8A File Offset: 0x000E3C8A
	public void AllowInput(bool allowed)
	{
		this.m_inputBlocker.SetActive(!allowed);
	}

	// Token: 0x06002DC9 RID: 11721 RVA: 0x000E5A9B File Offset: 0x000E3C9B
	public bool MouseIsOver()
	{
		return UniversalInputManager.Get().InputIsOver(base.gameObject);
	}

	// Token: 0x06002DCA RID: 11722 RVA: 0x000E5AAD File Offset: 0x000E3CAD
	public bool IsShowingDeckContents()
	{
		return this.GetCurrentContentType() != CollectionDeckTray.DeckContentTypes.Decks;
	}

	// Token: 0x06002DCB RID: 11723 RVA: 0x000E5ABB File Offset: 0x000E3CBB
	public bool IsWaitingToDeleteDeck()
	{
		return this.m_decksContent.IsWaitingToDeleteDeck();
	}

	// Token: 0x06002DCC RID: 11724 RVA: 0x000E5AC8 File Offset: 0x000E3CC8
	public void DeleteEditingDeck(bool popNavigation = true)
	{
		if (popNavigation)
		{
			Navigation.Pop();
		}
		this.m_decksContent.DeleteEditingDeck();
		this.SetTrayMode(CollectionDeckTray.DeckContentTypes.Decks);
	}

	// Token: 0x06002DCD RID: 11725 RVA: 0x000E5AE7 File Offset: 0x000E3CE7
	public void CancelRenamingDeck()
	{
		this.m_decksContent.CancelRenameEditingDeck();
	}

	// Token: 0x06002DCE RID: 11726 RVA: 0x000E5AF4 File Offset: 0x000E3CF4
	public DeckBigCard GetDeckBigCard()
	{
		return this.m_deckBigCard;
	}

	// Token: 0x06002DCF RID: 11727 RVA: 0x000E5AFC File Offset: 0x000E3CFC
	public void ClearCountLabels()
	{
		this.m_countLabelText.Text = string.Empty;
		this.m_countText.Text = string.Empty;
	}

	// Token: 0x06002DD0 RID: 11728 RVA: 0x000E5B29 File Offset: 0x000E3D29
	public DeckTrayDeckTileVisual GetCardTileVisual(string cardID)
	{
		return this.m_cardsContent.GetCardTileVisual(cardID);
	}

	// Token: 0x06002DD1 RID: 11729 RVA: 0x000E5B37 File Offset: 0x000E3D37
	public DeckTrayDeckTileVisual GetCardTileVisual(string cardID, TAG_PREMIUM premType)
	{
		return this.m_cardsContent.GetCardTileVisual(cardID, premType);
	}

	// Token: 0x06002DD2 RID: 11730 RVA: 0x000E5B46 File Offset: 0x000E3D46
	public DeckTrayDeckTileVisual GetCardTileVisual(int index)
	{
		return this.m_cardsContent.GetCardTileVisual(index);
	}

	// Token: 0x06002DD3 RID: 11731 RVA: 0x000E5B54 File Offset: 0x000E3D54
	public DeckTrayDeckTileVisual GetOrAddCardTileVisual(int index, bool affectedByScrollbar = true)
	{
		DeckTrayDeckTileVisual orAddCardTileVisual = this.m_cardsContent.GetOrAddCardTileVisual(index);
		if (orAddCardTileVisual == null)
		{
			orAddCardTileVisual = this.m_cardsContent.GetOrAddCardTileVisual(index);
			if (affectedByScrollbar)
			{
				this.m_scrollbar.AddVisibleAffectedObject(orAddCardTileVisual.gameObject, this.m_cardsContent.GetCardVisualExtents(), true, new UIBScrollable.VisibleAffected(CollectionDeckTray.OnDeckTrayTileScrollVisibleAffected));
			}
		}
		return orAddCardTileVisual;
	}

	// Token: 0x06002DD4 RID: 11732 RVA: 0x000E5BB7 File Offset: 0x000E3DB7
	public DeckTrayContent GetCurrentContent()
	{
		return this.m_contents[this.m_currentContent];
	}

	// Token: 0x06002DD5 RID: 11733 RVA: 0x000E5BCA File Offset: 0x000E3DCA
	public CollectionDeckTray.DeckContentTypes GetCurrentContentType()
	{
		return this.m_currentContent;
	}

	// Token: 0x06002DD6 RID: 11734 RVA: 0x000E5BD2 File Offset: 0x000E3DD2
	public void SetMyDecksLabelText(string text)
	{
		this.m_myDecksLabel.Text = text;
	}

	// Token: 0x06002DD7 RID: 11735 RVA: 0x000E5BE0 File Offset: 0x000E3DE0
	public TooltipZone GetTooltipZone()
	{
		return this.m_deckHeaderTooltip;
	}

	// Token: 0x06002DD8 RID: 11736 RVA: 0x000E5BE8 File Offset: 0x000E3DE8
	public static void OnDeckTrayTileScrollVisibleAffected(GameObject obj, bool visible)
	{
		DeckTrayDeckTileVisual component = obj.GetComponent<DeckTrayDeckTileVisual>();
		if (component == null || !component.IsInUse())
		{
			return;
		}
		if (visible != component.gameObject.activeSelf)
		{
			component.gameObject.SetActive(visible);
		}
	}

	// Token: 0x06002DD9 RID: 11737 RVA: 0x000E5C31 File Offset: 0x000E3E31
	public DeckTrayDeckListContent GetDecksContent()
	{
		return this.m_decksContent;
	}

	// Token: 0x06002DDA RID: 11738 RVA: 0x000E5C39 File Offset: 0x000E3E39
	public DeckTrayCardListContent GetCardsContent()
	{
		return this.m_cardsContent;
	}

	// Token: 0x06002DDB RID: 11739 RVA: 0x000E5C41 File Offset: 0x000E3E41
	public DeckTrayCardBackContent GetCardBackContent()
	{
		return this.m_cardBackContent;
	}

	// Token: 0x06002DDC RID: 11740 RVA: 0x000E5C49 File Offset: 0x000E3E49
	public DeckTrayHeroSkinContent GetHeroSkinContent()
	{
		return this.m_heroSkinContent;
	}

	// Token: 0x06002DDD RID: 11741 RVA: 0x000E5C54 File Offset: 0x000E3E54
	public void SetTrayMode(CollectionDeckTray.DeckContentTypes contentType)
	{
		this.m_contentToSet = contentType;
		if (this.m_settingNewMode || this.m_currentContent == contentType)
		{
			return;
		}
		base.StartCoroutine(this.UpdateTrayMode());
	}

	// Token: 0x06002DDE RID: 11742 RVA: 0x000E5C8D File Offset: 0x000E3E8D
	public void RegisterModeSwitchedListener(CollectionDeckTray.ModeSwitched callback)
	{
		this.m_modeSwitchedListeners.Add(callback);
	}

	// Token: 0x06002DDF RID: 11743 RVA: 0x000E5C9B File Offset: 0x000E3E9B
	public void UnregisterModeSwitchedListener(CollectionDeckTray.ModeSwitched callback)
	{
		this.m_modeSwitchedListeners.Remove(callback);
	}

	// Token: 0x06002DE0 RID: 11744 RVA: 0x000E5CAA File Offset: 0x000E3EAA
	public void Exit()
	{
		if (!UniversalInputManager.UsePhoneUI)
		{
			this.HideUnseenDeckTrays();
		}
	}

	// Token: 0x06002DE1 RID: 11745 RVA: 0x000E5CC1 File Offset: 0x000E3EC1
	public void UpdateTileVisuals()
	{
		this.m_cardsContent.UpdateTileVisuals();
	}

	// Token: 0x06002DE2 RID: 11746 RVA: 0x000E5CD0 File Offset: 0x000E3ED0
	public CollectionDeckBoxVisual GetEditingDeckBox()
	{
		TraySection editingTraySection = this.GetDecksContent().GetEditingTraySection();
		if (editingTraySection == null)
		{
			return null;
		}
		return editingTraySection.m_deckBox;
	}

	// Token: 0x06002DE3 RID: 11747 RVA: 0x000E5CFD File Offset: 0x000E3EFD
	private void DoneButtonPress(UIEvent e)
	{
		Navigation.GoBack();
	}

	// Token: 0x06002DE4 RID: 11748 RVA: 0x000E5D05 File Offset: 0x000E3F05
	public bool OnBackOutOfDeckContents()
	{
		return this.OnBackOutOfDeckContentsImpl(false);
	}

	// Token: 0x06002DE5 RID: 11749 RVA: 0x000E5D10 File Offset: 0x000E3F10
	public bool OnBackOutOfDeckContentsImpl(bool deleteDeck)
	{
		if (this.GetCurrentContentType() != CollectionDeckTray.DeckContentTypes.INVALID && !this.GetCurrentContent().IsModeActive())
		{
			return false;
		}
		if (!this.IsShowingDeckContents())
		{
			return false;
		}
		Log.DeckTray.Print("backing out of deck contents " + deleteDeck, new object[0]);
		DeckHelper.Get().Hide(true);
		CollectionManagerDisplay.Get().HideConvertTutorial();
		CollectionDeck taggedDeck = CollectionManager.Get().GetTaggedDeck(CollectionManager.DeckTag.Editing);
		if (deleteDeck)
		{
			this.m_decksContent.DeleteDeck(taggedDeck.ID);
		}
		List<string> reasons = new List<string>();
		List<DeckRule> brokenRules = new List<DeckRule>();
		DeckRuleset deckRuleset = CollectionManager.Get().GetDeckRuleset();
		CollectionManagerDisplay.Get().m_pageManager.HideNonDeckTemplateTabs(false, false);
		bool flag = deckRuleset == null || deckRuleset.IsDeckValid(taggedDeck, out reasons, out brokenRules);
		if (!flag && !deleteDeck)
		{
			this.PopupInvalidDeckConfirmation(reasons, brokenRules);
		}
		else
		{
			if (!taggedDeck.IsWild && flag && CollectionManager.Get().ShouldShowWildToStandardTutorial(false) && UserAttentionManager.CanShowAttentionGrabber(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS, "CollectionDeckTray.OnBackOutOfDeckContentsImpl:ShowSetRotationTutorial"))
			{
				Options.Get().SetBool(Option.NEEDS_TO_MAKE_STANDARD_DECK, false);
				Options.Get().SetLong(Option.LAST_CUSTOM_DECK_CHOSEN, taggedDeck.ID);
				Vector3 vector = OverlayUI.Get().GetRelativePosition(this.m_doneButton.transform.position, null, null, 0f);
				vector += ((!UniversalInputManager.UsePhoneUI) ? new Vector3(0f, 0f, 23f) : new Vector3(-56.5f, 0f, 35f));
				Notification notification = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS, vector, NotificationManager.NOTIFICATITON_WORLD_SCALE, GameStrings.Get("GLUE_COLLECTION_TUTORIAL16"), false);
				if (UniversalInputManager.UsePhoneUI)
				{
					notification.ShowPopUpArrow(Notification.PopUpArrowDirection.RightDown);
				}
				else
				{
					notification.ShowPopUpArrow(Notification.PopUpArrowDirection.Down);
				}
				notification.PulseReminderEveryXSeconds(3f);
				UserAttentionManager.StopBlocking(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS);
				this.m_doneButton.GetComponentInChildren<HighlightState>().ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
			}
			this.SaveCurrentDeckAndEnterDeckListMode();
		}
		return true;
	}

	// Token: 0x06002DE6 RID: 11750 RVA: 0x000E5F24 File Offset: 0x000E4124
	private void PopupInvalidDeckConfirmation(List<string> reasons, List<DeckRule> brokenRules)
	{
		List<DeckRule.RuleType> list = new List<DeckRule.RuleType>(new DeckRule.RuleType[]
		{
			DeckRule.RuleType.IS_NOT_ROTATED,
			DeckRule.RuleType.DECK_SIZE
		});
		bool flag = false;
		string text = string.Empty;
		for (int i = 0; i < reasons.Count; i++)
		{
			DeckRule deckRule = brokenRules[i];
			if (list.Contains(deckRule.GetRuleType()))
			{
				flag = true;
				text = text + reasons[i] + "\n";
			}
		}
		brokenRules.Find((DeckRule rule) => rule.GetRuleType() == DeckRule.RuleType.IS_NOT_ROTATED || rule.GetRuleType() == DeckRule.RuleType.DECK_SIZE);
		CollectionManagerDisplay.Get().SetViewMode(CollectionManagerDisplay.ViewMode.CARDS, null);
		AlertPopup.PopupInfo info;
		if (flag)
		{
			info = new AlertPopup.PopupInfo
			{
				m_headerText = GameStrings.Get("GLUE_COLLECTION_DECK_INVALID_POPUP_HEADER"),
				m_text = text + "\n" + GameStrings.Get("GLUE_COLLECTION_DECK_RULE_FINISH_AUTOMATICALLY"),
				m_cancelText = GameStrings.Get("GLUE_COLLECTION_DECK_SAVE_ANYWAY"),
				m_confirmText = GameStrings.Get("GLUE_COLLECTION_DECK_FINISH_FOR_ME"),
				m_showAlertIcon = true,
				m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
				m_responseCallback = delegate(AlertPopup.Response response, object userData)
				{
					if (response == AlertPopup.Response.CANCEL)
					{
						this.SaveCurrentDeckAndEnterDeckListMode();
					}
					else
					{
						base.StartCoroutine(this.FinishMyDeckPress());
					}
				}
			};
		}
		else
		{
			info = new AlertPopup.PopupInfo
			{
				m_headerText = GameStrings.Get("GLUE_COLLECTION_DECK_INVALID_POPUP_HEADER"),
				m_text = text,
				m_okText = GameStrings.Get("GLOBAL_OKAY"),
				m_showAlertIcon = true,
				m_responseDisplay = AlertPopup.ResponseDisplay.OK,
				m_responseCallback = delegate(AlertPopup.Response response, object userData)
				{
					this.SaveCurrentDeckAndEnterDeckListMode();
				}
			};
		}
		DialogManager.Get().ShowPopup(info);
	}

	// Token: 0x06002DE7 RID: 11751 RVA: 0x000E60B0 File Offset: 0x000E42B0
	private bool OnBackOutOfCollectionScreen()
	{
		NotificationManager.Get().DestroyNotificationWithText(GameStrings.Get("GLUE_COLLECTION_TUTORIAL16"), 0f);
		this.m_doneButton.GetComponentInChildren<HighlightState>().ChangeState(ActorStateType.HIGHLIGHT_OFF);
		if (this.GetCurrentContentType() != CollectionDeckTray.DeckContentTypes.INVALID && !this.GetCurrentContent().IsModeActive())
		{
			return false;
		}
		if (this.IsShowingDeckContents() && SceneMgr.Get().GetMode() != SceneMgr.Mode.TAVERN_BRAWL)
		{
			return false;
		}
		AnimationUtil.DelayedActivate(base.gameObject, 0.25f, false);
		CollectionManagerDisplay.Get().Exit();
		return true;
	}

	// Token: 0x06002DE8 RID: 11752 RVA: 0x000E6144 File Offset: 0x000E4344
	private void SaveCurrentDeckAndEnterDeckListMode()
	{
		CollectionDeck taggedDeck = CollectionManager.Get().GetTaggedDeck(CollectionManager.DeckTag.Editing);
		if (taggedDeck != null)
		{
			taggedDeck.SendChanges();
		}
		if (SceneMgr.Get().GetMode() == SceneMgr.Mode.TAVERN_BRAWL)
		{
			if (TavernBrawlDisplay.Get() != null)
			{
				TavernBrawlDisplay.Get().BackFromDeckEdit(true);
			}
		}
		else
		{
			this.SetTrayMode(CollectionDeckTray.DeckContentTypes.Decks);
			CollectionManager.Get().DoneEditing();
			this.UpdateDoneButtonText();
			if (CollectionManagerDisplay.Get() != null)
			{
				CollectionManagerDisplay.Get().OnDoneEditingDeck();
			}
		}
	}

	// Token: 0x06002DE9 RID: 11753 RVA: 0x000E61CC File Offset: 0x000E43CC
	private IEnumerator FinishMyDeckPress()
	{
		CollectionDeck deck = CollectionManager.Get().GetTaggedDeck(CollectionManager.DeckTag.Editing);
		DeckRuleset deckRuleset = CollectionManager.Get().GetDeckRuleset();
		IEnumerable<DeckMaker.DeckFill> fillCards = DeckMaker.GetFillCards(deck, deckRuleset);
		yield return base.StartCoroutine(this.AutoAddCardsWithTiming(fillCards, deckRuleset));
		this.OnBackOutOfDeckContents();
		yield break;
	}

	// Token: 0x06002DEA RID: 11754 RVA: 0x000E61E8 File Offset: 0x000E43E8
	public void PopulateDeck(IEnumerable<DeckMaker.DeckFill> fillCards)
	{
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get("GLUE_COLLECTION_DECK_PASTE_POPUP_HEADER");
		popupInfo.m_text = GameStrings.Format("GLUE_COLLECTION_DECK_PASTE_POPUP_MESSAGE", new object[0]);
		popupInfo.m_showAlertIcon = true;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
		AlertPopup.ResponseCallback responseCallback = delegate(AlertPopup.Response response, object userdata)
		{
			if (response == AlertPopup.Response.CONFIRM)
			{
				CollectionDeck taggedDeck = CollectionManager.Get().GetTaggedDeck(CollectionManager.DeckTag.Editing);
				taggedDeck.ClearSlotContents();
				this.GetCardsContent().UpdateCardList(true, null);
				this.StartCoroutine(this.AutoAddCardsWithTiming(fillCards, null));
			}
		};
		popupInfo.m_responseCallback = responseCallback;
		DialogManager.Get().ShowPopup(popupInfo);
	}

	// Token: 0x06002DEB RID: 11755 RVA: 0x000E6264 File Offset: 0x000E4464
	private IEnumerator AutoAddCardsWithTiming(IEnumerable<DeckMaker.DeckFill> fillCards, DeckRuleset deckRuleset)
	{
		this.AllowInput(false);
		CollectionManagerDisplay.Get().m_inputBlocker.gameObject.SetActive(true);
		if (CollectionManager.Get().IsInEditMode())
		{
			CollectionDeck deck = CollectionManager.Get().GetTaggedDeck(CollectionManager.DeckTag.Editing);
			int maxDeckSize = (deckRuleset != null) ? deckRuleset.GetDeckSize() : int.MaxValue;
			foreach (DeckMaker.DeckFill fillCard in fillCards)
			{
				if (!deck.HasReplaceableSlot())
				{
					int currentDeckSize = deck.GetTotalCardCount();
					if (currentDeckSize >= maxDeckSize)
					{
						break;
					}
				}
				EntityDef addCard = fillCard.m_addCard;
				EntityDef removeCard = fillCard.m_removeTemplate;
				if (removeCard != null)
				{
					bool removed = false;
					removed = this.RemoveCard(removeCard.GetCardId(), TAG_PREMIUM.NORMAL, false);
					if (!removed)
					{
						this.RemoveCard(removeCard.GetCardId(), TAG_PREMIUM.GOLDEN, false);
					}
				}
				bool added = false;
				added |= this.AddCard(addCard, TAG_PREMIUM.GOLDEN, null, true, null);
				if (!added)
				{
					added |= this.AddCard(addCard, TAG_PREMIUM.NORMAL, null, true, null);
				}
				if (added)
				{
					yield return new WaitForSeconds(0.2f);
				}
			}
		}
		CollectionManagerDisplay.Get().m_inputBlocker.gameObject.SetActive(false);
		this.AllowInput(true);
		yield break;
	}

	// Token: 0x06002DEC RID: 11756 RVA: 0x000E629C File Offset: 0x000E449C
	public void UpdateDoneButtonText()
	{
		bool flag = !CollectionManager.Get().IsInEditMode() || CollectionManagerDisplay.Get().GetViewMode() == CollectionManagerDisplay.ViewMode.DECK_TEMPLATE;
		if (SceneMgr.Get().GetMode() == SceneMgr.Mode.TAVERN_BRAWL)
		{
			flag = (!TavernBrawlDisplay.Get().IsInDeckEditMode() && !UniversalInputManager.UsePhoneUI);
		}
		bool flag2 = this.m_backArrow != null;
		if (flag)
		{
			this.m_doneButton.SetText((!flag2) ? GameStrings.Get("GLOBAL_BACK") : string.Empty);
			if (flag2)
			{
				this.m_backArrow.gameObject.SetActive(true);
			}
		}
		else
		{
			this.m_doneButton.SetText(GameStrings.Get("GLOBAL_DONE"));
			if (flag2)
			{
				this.m_backArrow.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06002DED RID: 11757 RVA: 0x000E637A File Offset: 0x000E457A
	public bool IsUpdatingTrayMode()
	{
		return this.m_updatingTrayMode;
	}

	// Token: 0x06002DEE RID: 11758 RVA: 0x000E6384 File Offset: 0x000E4584
	private IEnumerator UpdateTrayMode()
	{
		CollectionDeckTray.DeckContentTypes oldContentType = this.m_currentContent;
		CollectionDeckTray.DeckContentTypes newContentType = this.m_contentToSet;
		if (this.m_settingNewMode || this.m_currentContent == this.m_contentToSet || this.m_contentToSet == CollectionDeckTray.DeckContentTypes.INVALID)
		{
			this.m_updatingTrayMode = false;
			yield break;
		}
		this.m_contentToSet = CollectionDeckTray.DeckContentTypes.INVALID;
		this.m_currentContent = CollectionDeckTray.DeckContentTypes.INVALID;
		this.m_settingNewMode = true;
		this.m_updatingTrayMode = true;
		DeckTrayContent oldContent = null;
		DeckTrayContent newContent = null;
		this.m_contents.TryGetValue(oldContentType, out oldContent);
		this.m_contents.TryGetValue(newContentType, out newContent);
		if (oldContent != null)
		{
			while (!oldContent.PreAnimateContentExit())
			{
				yield return null;
			}
		}
		if (newContent != null)
		{
			while (!newContent.PreAnimateContentEntrance())
			{
				yield return null;
			}
		}
		this.SaveScrollbarPosition(oldContentType);
		this.TryDisableScrollbar();
		if (oldContent != null)
		{
			oldContent.SetModeActive(false);
			while (!oldContent.AnimateContentExitStart())
			{
				yield return null;
			}
			Log.DeckTray.Print("OLD: {0} AnimateContentExitStart - finished", new object[]
			{
				oldContentType
			});
			while (!oldContent.AnimateContentExitEnd())
			{
				yield return null;
			}
			Log.DeckTray.Print("OLD: {0} AnimateContentExitEnd - finished", new object[]
			{
				oldContentType
			});
		}
		this.m_currentContent = newContentType;
		if (newContent != null)
		{
			newContent.SetModeTrying(true);
			while (!newContent.AnimateContentEntranceStart())
			{
				yield return null;
			}
			Log.DeckTray.Print("NEW: {0} AnimateContentEntranceStart - finished", new object[]
			{
				newContentType
			});
			while (!newContent.AnimateContentEntranceEnd())
			{
				yield return null;
			}
			Log.DeckTray.Print("NEW: {0} AnimateContentEntranceEnd - finished", new object[]
			{
				newContentType
			});
			newContent.SetModeActive(true);
			newContent.SetModeTrying(false);
		}
		this.TryEnableScrollbar();
		if (newContent != null)
		{
			while (!newContent.PostAnimateContentEntrance())
			{
				yield return null;
			}
		}
		if (oldContent != null)
		{
			while (!oldContent.PostAnimateContentExit())
			{
				yield return null;
			}
		}
		if (this.m_currentContent != CollectionDeckTray.DeckContentTypes.Decks)
		{
			this.m_cardsContent.TriggerCardCountUpdate();
		}
		this.m_settingNewMode = false;
		this.FireModeSwitchedEvent();
		this.UpdateDoneButtonText();
		if (this.m_contentToSet != CollectionDeckTray.DeckContentTypes.INVALID)
		{
			base.StartCoroutine(this.UpdateTrayMode());
			yield break;
		}
		this.m_updatingTrayMode = false;
		yield break;
	}

	// Token: 0x06002DEF RID: 11759 RVA: 0x000E63A0 File Offset: 0x000E45A0
	private void TryEnableScrollbar()
	{
		if (this.m_scrollbar == null)
		{
			return;
		}
		DeckTrayContent currentContent = this.GetCurrentContent();
		if (currentContent == null)
		{
			return;
		}
		CollectionDeckTray.DeckContentScroll deckContentScroll = this.m_scrollables.Find((CollectionDeckTray.DeckContentScroll type) => this.GetCurrentContentType() == type.m_contentType);
		if (deckContentScroll == null || deckContentScroll.m_scrollObject == null)
		{
			Debug.LogWarning("No scrollable object defined.");
			return;
		}
		this.m_scrollbar.m_ScrollObject = deckContentScroll.m_scrollObject;
		this.m_scrollbar.ResetScrollStartPosition(deckContentScroll.GetStartPosition());
		if (deckContentScroll.m_saveScrollPosition)
		{
			this.m_scrollbar.SetScrollSnap(deckContentScroll.GetCurrentScroll(), true);
		}
		this.m_scrollbar.EnableIfNeeded();
	}

	// Token: 0x06002DF0 RID: 11760 RVA: 0x000E6458 File Offset: 0x000E4658
	private void SaveScrollbarPosition(CollectionDeckTray.DeckContentTypes contentType)
	{
		CollectionDeckTray.DeckContentScroll deckContentScroll = this.m_scrollables.Find((CollectionDeckTray.DeckContentScroll type) => contentType == type.m_contentType);
		if (deckContentScroll != null && deckContentScroll.m_saveScrollPosition)
		{
			deckContentScroll.SaveCurrentScroll(this.m_scrollbar.GetScroll());
		}
	}

	// Token: 0x06002DF1 RID: 11761 RVA: 0x000E64AC File Offset: 0x000E46AC
	private void TryDisableScrollbar()
	{
		if (this.m_scrollbar == null || this.m_scrollbar.m_ScrollObject == null)
		{
			return;
		}
		this.m_scrollbar.Enable(false);
		this.m_scrollbar.m_ScrollObject = null;
	}

	// Token: 0x06002DF2 RID: 11762 RVA: 0x000E64F9 File Offset: 0x000E46F9
	private void HideUnseenDeckTrays()
	{
		if (this.m_currentContent != CollectionDeckTray.DeckContentTypes.Decks)
		{
			return;
		}
		this.m_decksContent.HideTraySectionsNotInBounds(this.m_scrollbar.m_ScrollBounds.bounds);
	}

	// Token: 0x06002DF3 RID: 11763 RVA: 0x000E6522 File Offset: 0x000E4722
	private void OnTouchScrollStarted()
	{
		if (this.m_deckBigCard != null)
		{
			this.m_deckBigCard.ForceHide();
		}
	}

	// Token: 0x06002DF4 RID: 11764 RVA: 0x000E6540 File Offset: 0x000E4740
	private void OnTouchScrollEnded()
	{
	}

	// Token: 0x06002DF5 RID: 11765 RVA: 0x000E6544 File Offset: 0x000E4744
	private void OnCardTilePress(DeckTrayDeckTileVisual cardTile)
	{
		if (UniversalInputManager.Get().IsTouchMode())
		{
			this.ShowDeckBigCard(cardTile, 0.2f);
		}
		else if (CollectionInputMgr.Get() != null)
		{
			this.HideDeckBigCard(cardTile, false);
		}
	}

	// Token: 0x06002DF6 RID: 11766 RVA: 0x000E658C File Offset: 0x000E478C
	private void OnCardTileTap(DeckTrayDeckTileVisual cardTile)
	{
		if (UniversalInputManager.Get().IsTouchMode() && CollectionManagerDisplay.Get().GetViewMode() != CollectionManagerDisplay.ViewMode.DECK_TEMPLATE)
		{
			CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
			if (!editedDeck.IsValidSlot(cardTile.GetSlot()))
			{
				this.m_cardsContent.ShowDeckHelper(cardTile, false, true);
				return;
			}
		}
	}

	// Token: 0x06002DF7 RID: 11767 RVA: 0x000E65E4 File Offset: 0x000E47E4
	private void OnCardTileHeld(DeckTrayDeckTileVisual cardTile)
	{
		if (CollectionInputMgr.Get() != null && !TavernBrawlDisplay.IsTavernBrawlViewing() && CollectionManagerDisplay.Get().GetViewMode() != CollectionManagerDisplay.ViewMode.DECK_TEMPLATE)
		{
			bool flag = CollectionInputMgr.Get().GrabCard(cardTile);
			if (flag && this.m_deckBigCard != null)
			{
				this.HideDeckBigCard(cardTile, true);
			}
		}
	}

	// Token: 0x06002DF8 RID: 11768 RVA: 0x000E6648 File Offset: 0x000E4848
	private void OnCardTileOver(DeckTrayDeckTileVisual cardTile)
	{
		if (UniversalInputManager.Get().IsTouchMode())
		{
			return;
		}
		if (CollectionInputMgr.Get() == null || !CollectionInputMgr.Get().HasHeldCard())
		{
			this.ShowDeckBigCard(cardTile, 0f);
		}
	}

	// Token: 0x06002DF9 RID: 11769 RVA: 0x000E6690 File Offset: 0x000E4890
	private void OnCardTileOut(DeckTrayDeckTileVisual cardTile)
	{
		this.HideDeckBigCard(cardTile, false);
	}

	// Token: 0x06002DFA RID: 11770 RVA: 0x000E669A File Offset: 0x000E489A
	public void OnCardTileRelease(DeckTrayDeckTileVisual cardTile)
	{
		this.RemoveCardTile(cardTile, false);
	}

	// Token: 0x06002DFB RID: 11771 RVA: 0x000E66A4 File Offset: 0x000E48A4
	public void RemoveCardTile(DeckTrayDeckTileVisual cardTile, bool removeAllCopies = false)
	{
		if (CollectionManagerDisplay.Get().GetViewMode() == CollectionManagerDisplay.ViewMode.DECK_TEMPLATE)
		{
			return;
		}
		CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
		if (UniversalInputManager.Get().IsTouchMode())
		{
			this.HideDeckBigCard(cardTile, false);
			return;
		}
		if (!editedDeck.IsValidSlot(cardTile.GetSlot()))
		{
			this.m_cardsContent.ShowDeckHelper(cardTile, false, true);
			return;
		}
		if (CollectionInputMgr.Get() == null || TavernBrawlDisplay.IsTavernBrawlViewing())
		{
			return;
		}
		CollectionDeckTileActor actor = cardTile.GetActor();
		Spell spell = actor.GetSpell(SpellType.SUMMON_IN);
		GameObject gameObject = Object.Instantiate<GameObject>(spell.gameObject);
		gameObject.transform.position = actor.transform.position + new Vector3(-2f, 0f, 0f);
		gameObject.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
		gameObject.GetComponent<Spell>().ActivateState(SpellStateType.BIRTH);
		base.StartCoroutine(this.DestroyAfterSeconds(gameObject));
		if (CollectionDeckTray.Get() != null)
		{
			CollectionDeckTray.Get().RemoveCard(cardTile.GetCardID(), cardTile.GetPremium(), editedDeck.IsValidSlot(cardTile.GetSlot()));
		}
		iTween.MoveTo(gameObject, new Vector3(gameObject.transform.position.x - 10f, gameObject.transform.position.y + 10f, gameObject.transform.position.z), 4f);
		SoundManager.Get().LoadAndPlay("collection_manager_card_remove_from_deck_instant", base.gameObject);
	}

	// Token: 0x06002DFC RID: 11772 RVA: 0x000E6844 File Offset: 0x000E4A44
	private void OnCardCountUpdated(int cardCount)
	{
		string text = GameStrings.Get("GLUE_DECK_TRAY_CARD_COUNT_LABEL");
		string text2 = GameStrings.Format("GLUE_DECK_TRAY_COUNT", new object[]
		{
			cardCount,
			CollectionManager.Get().GetDeckSize()
		});
		this.m_countLabelText.Text = text;
		this.m_countText.Text = text2;
	}

	// Token: 0x06002DFD RID: 11773 RVA: 0x000E68A0 File Offset: 0x000E4AA0
	private void OnDeckCountUpdated(int deckCount)
	{
		string text;
		string text2;
		if (AchieveManager.Get().HasUnlockedFeature(Achievement.UnlockableFeature.VANILLA_HEROES))
		{
			text = GameStrings.Get("GLUE_DECK_TRAY_DECK_COUNT_LABEL");
			text2 = GameStrings.Format("GLUE_DECK_TRAY_COUNT", new object[]
			{
				deckCount,
				18
			});
		}
		else
		{
			text = GameStrings.Get("GLUE_DECK_TRAY_HEROES_UNLOCKED_COUNT_LABEL");
			text2 = GameStrings.Format("GLUE_DECK_TRAY_COUNT", new object[]
			{
				deckCount,
				9
			});
		}
		this.m_countLabelText.Text = text;
		this.m_countText.Text = text2;
	}

	// Token: 0x06002DFE RID: 11774 RVA: 0x000E6939 File Offset: 0x000E4B39
	private void OnBusyWithDeck(bool busy)
	{
		this.m_inputBlocker.SetActive(busy);
	}

	// Token: 0x06002DFF RID: 11775 RVA: 0x000E6948 File Offset: 0x000E4B48
	private void OnTaggedDeckChanged(CollectionManager.DeckTag tag, CollectionDeck newDeck, CollectionDeck oldDeck, object callbackData)
	{
		bool isNewDeck = callbackData != null && callbackData is bool && (bool)callbackData;
		foreach (KeyValuePair<CollectionDeckTray.DeckContentTypes, DeckTrayContent> keyValuePair in this.m_contents)
		{
			keyValuePair.Value.OnTaggedDeckChanged(tag, newDeck, oldDeck, isNewDeck);
		}
	}

	// Token: 0x06002E00 RID: 11776 RVA: 0x000E69CC File Offset: 0x000E4BCC
	private IEnumerator DestroyAfterSeconds(GameObject go)
	{
		yield return new WaitForSeconds(5f);
		Object.Destroy(go);
		yield break;
	}

	// Token: 0x06002E01 RID: 11777 RVA: 0x000E69F0 File Offset: 0x000E4BF0
	private void ShowDeckBigCard(DeckTrayDeckTileVisual cardTile, float delay = 0f)
	{
		CollectionDeckTileActor actor = cardTile.GetActor();
		if (this.m_deckBigCard == null)
		{
			return;
		}
		CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
		EntityDef entityDef = actor.GetEntityDef();
		CardDef cardDef = DefLoader.Get().GetCardDef(entityDef.GetCardId(), null);
		GhostCard.Type ghostTypeFromSlot = GhostCard.GetGhostTypeFromSlot(editedDeck, cardTile.GetSlot());
		this.m_deckBigCard.Show(entityDef, actor.GetPremium(), cardDef, actor.gameObject.transform.position, ghostTypeFromSlot, delay);
		if (UniversalInputManager.Get().IsTouchMode())
		{
			cardTile.SetHighlight(true);
		}
		if (CollectionManagerDisplay.Get().m_deckTemplateCardReplacePopup != null)
		{
			CollectionManagerDisplay.Get().m_deckTemplateCardReplacePopup.Shrink(0.1f);
		}
	}

	// Token: 0x06002E02 RID: 11778 RVA: 0x000E6AB0 File Offset: 0x000E4CB0
	private void HideDeckBigCard(DeckTrayDeckTileVisual cardTile, bool force = false)
	{
		CollectionDeckTileActor actor = cardTile.GetActor();
		if (this.m_deckBigCard != null)
		{
			if (force)
			{
				this.m_deckBigCard.ForceHide();
			}
			else
			{
				this.m_deckBigCard.Hide(actor.GetEntityDef(), actor.GetPremium());
			}
			if (UniversalInputManager.Get().IsTouchMode())
			{
				cardTile.SetHighlight(false);
			}
			if (CollectionManagerDisplay.Get().m_deckTemplateCardReplacePopup != null)
			{
				CollectionManagerDisplay.Get().m_deckTemplateCardReplacePopup.Unshrink(0.1f);
			}
		}
	}

	// Token: 0x06002E03 RID: 11779 RVA: 0x000E6B44 File Offset: 0x000E4D44
	private void OnCMViewModeChanged(CollectionManagerDisplay.ViewMode prevMode, CollectionManagerDisplay.ViewMode mode, CollectionManagerDisplay.ViewModeData userdata, bool triggerResponse)
	{
		CollectionDeckTray.DeckContentTypes contentTypeFromViewMode = this.GetContentTypeFromViewMode(mode);
		this.m_cardsContent.ShowFakeDeck(mode == CollectionManagerDisplay.ViewMode.DECK_TEMPLATE);
		if (!triggerResponse)
		{
			return;
		}
		CollectionDeckTray.Get().m_decksContent.UpdateDeckName(null);
		if (this.m_currentContent == CollectionDeckTray.DeckContentTypes.Decks)
		{
			return;
		}
		this.SetTrayMode(contentTypeFromViewMode);
	}

	// Token: 0x06002E04 RID: 11780 RVA: 0x000E6B94 File Offset: 0x000E4D94
	private CollectionDeckTray.DeckContentTypes GetContentTypeFromViewMode(CollectionManagerDisplay.ViewMode viewMode)
	{
		if (viewMode == CollectionManagerDisplay.ViewMode.HERO_SKINS)
		{
			return CollectionDeckTray.DeckContentTypes.HeroSkin;
		}
		if (viewMode != CollectionManagerDisplay.ViewMode.CARD_BACKS)
		{
			return CollectionDeckTray.DeckContentTypes.Cards;
		}
		return CollectionDeckTray.DeckContentTypes.CardBack;
	}

	// Token: 0x06002E05 RID: 11781 RVA: 0x000E6BBC File Offset: 0x000E4DBC
	private void FireModeSwitchedEvent()
	{
		CollectionDeckTray.ModeSwitched[] array = this.m_modeSwitchedListeners.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i]();
		}
	}

	// Token: 0x06002E06 RID: 11782 RVA: 0x000E6BF1 File Offset: 0x000E4DF1
	private void OnHeroAssigned(string cardID)
	{
		this.m_decksContent.UpdateEditingDeckBoxVisual(cardID);
	}

	// Token: 0x04001C91 RID: 7313
	public UberText m_countLabelText;

	// Token: 0x04001C92 RID: 7314
	public UberText m_countText;

	// Token: 0x04001C93 RID: 7315
	public UIBButton m_doneButton;

	// Token: 0x04001C94 RID: 7316
	public GameObject m_backArrow;

	// Token: 0x04001C95 RID: 7317
	public UberText m_myDecksLabel;

	// Token: 0x04001C96 RID: 7318
	public DeckTrayDeckListContent m_decksContent;

	// Token: 0x04001C97 RID: 7319
	public DeckTrayCardListContent m_cardsContent;

	// Token: 0x04001C98 RID: 7320
	public DeckTrayCardBackContent m_cardBackContent;

	// Token: 0x04001C99 RID: 7321
	public DeckTrayHeroSkinContent m_heroSkinContent;

	// Token: 0x04001C9A RID: 7322
	public DeckBigCard m_deckBigCard;

	// Token: 0x04001C9B RID: 7323
	public UIBScrollable m_scrollbar;

	// Token: 0x04001C9C RID: 7324
	public GameObject m_inputBlocker;

	// Token: 0x04001C9D RID: 7325
	public TooltipZone m_deckHeaderTooltip;

	// Token: 0x04001C9E RID: 7326
	public GameObject m_topCardPositionBone;

	// Token: 0x04001C9F RID: 7327
	public Transform m_removeCardTutorialBone;

	// Token: 0x04001CA0 RID: 7328
	public List<CollectionDeckTray.DeckContentScroll> m_scrollables = new List<CollectionDeckTray.DeckContentScroll>();

	// Token: 0x04001CA1 RID: 7329
	public PlayMakerFSM m_deckTemplateChosenGlow;

	// Token: 0x04001CA2 RID: 7330
	private static CollectionDeckTray s_instance;

	// Token: 0x04001CA3 RID: 7331
	private Map<CollectionDeckTray.DeckContentTypes, DeckTrayContent> m_contents = new Map<CollectionDeckTray.DeckContentTypes, DeckTrayContent>();

	// Token: 0x04001CA4 RID: 7332
	private CollectionDeckTray.DeckContentTypes m_currentContent = CollectionDeckTray.DeckContentTypes.INVALID;

	// Token: 0x04001CA5 RID: 7333
	private CollectionDeckTray.DeckContentTypes m_contentToSet = CollectionDeckTray.DeckContentTypes.INVALID;

	// Token: 0x04001CA6 RID: 7334
	private bool m_settingNewMode;

	// Token: 0x04001CA7 RID: 7335
	private bool m_updatingTrayMode;

	// Token: 0x04001CA8 RID: 7336
	private List<CollectionDeckTray.ModeSwitched> m_modeSwitchedListeners = new List<CollectionDeckTray.ModeSwitched>();

	// Token: 0x020006D2 RID: 1746
	// (Invoke) Token: 0x0600486B RID: 18539
	public delegate void ModeSwitched();

	// Token: 0x020006DC RID: 1756
	public enum DeckContentTypes
	{
		// Token: 0x04002FCF RID: 12239
		Decks,
		// Token: 0x04002FD0 RID: 12240
		Cards,
		// Token: 0x04002FD1 RID: 12241
		HeroSkin,
		// Token: 0x04002FD2 RID: 12242
		CardBack,
		// Token: 0x04002FD3 RID: 12243
		INVALID
	}

	// Token: 0x02000723 RID: 1827
	[Serializable]
	public class DeckContentScroll
	{
		// Token: 0x06004ABB RID: 19131 RVA: 0x00166047 File Offset: 0x00164247
		public void SaveStartPosition()
		{
			if (this.m_scrollObject != null)
			{
				this.m_startPos = this.m_scrollObject.transform.localPosition;
			}
		}

		// Token: 0x06004ABC RID: 19132 RVA: 0x00166070 File Offset: 0x00164270
		public Vector3 GetStartPosition()
		{
			return this.m_startPos;
		}

		// Token: 0x06004ABD RID: 19133 RVA: 0x00166078 File Offset: 0x00164278
		public Vector3 GetCurrentPosition()
		{
			return (!(this.m_scrollObject != null)) ? Vector3.zero : this.m_scrollObject.transform.localPosition;
		}

		// Token: 0x06004ABE RID: 19134 RVA: 0x001660A5 File Offset: 0x001642A5
		public void SaveCurrentScroll(float scroll)
		{
			this.m_currentScroll = scroll;
		}

		// Token: 0x06004ABF RID: 19135 RVA: 0x001660AE File Offset: 0x001642AE
		public float GetCurrentScroll()
		{
			return this.m_currentScroll;
		}

		// Token: 0x040031C2 RID: 12738
		public CollectionDeckTray.DeckContentTypes m_contentType;

		// Token: 0x040031C3 RID: 12739
		public GameObject m_scrollObject;

		// Token: 0x040031C4 RID: 12740
		public bool m_saveScrollPosition;

		// Token: 0x040031C5 RID: 12741
		private Vector3 m_startPos;

		// Token: 0x040031C6 RID: 12742
		private float m_currentScroll;
	}
}
