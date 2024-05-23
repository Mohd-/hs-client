using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000385 RID: 901
public class DeckTemplatePicker : MonoBehaviour
{
	// Token: 0x06002E5C RID: 11868 RVA: 0x000E86AC File Offset: 0x000E68AC
	private void Awake()
	{
		this.m_currentSelectedDeck = this.m_customDeck;
		for (int i = 0; i < 3; i++)
		{
			int idx = i;
			DeckTemplatePickerButton deckTemplatePickerButton = (DeckTemplatePickerButton)GameUtils.Instantiate(this.m_pickerButtonTpl, this.m_pickerButtonRoot.gameObject, true);
			Vector3 zero = Vector3.zero;
			if (UniversalInputManager.UsePhoneUI)
			{
				zero.x = 0.75f;
			}
			this.m_pickerButtonRoot.AddObject(deckTemplatePickerButton, zero, true);
			deckTemplatePickerButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
			{
				this.SelectButtonWithIndex(idx);
			});
			deckTemplatePickerButton.gameObject.SetActive(true);
			this.m_pickerButtons.Add(deckTemplatePickerButton);
		}
		if (this.m_pickerButtons.Count > 0)
		{
			this.m_pickerButtons[this.m_pickerButtons.Count - 1].SetIsStarterDeck(true);
		}
		this.m_pickerButtonRoot.UpdatePositions();
		this.m_pickerButtonTpl.gameObject.SetActive(false);
		if (this.m_customDeckButton != null)
		{
			this.m_customDeckButton.gameObject.SetActive(true);
			this.m_customDeckButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
			{
				this.SelectCustomDeckButton(false);
			});
		}
		if (this.m_chooseButton != null)
		{
			this.m_chooseButton.Disable();
			this.m_chooseButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
			{
				this.ChooseRecipeAndFillInCards();
			});
		}
		if (this.m_phoneTray != null)
		{
			this.m_phoneTray.m_scrollbar.SaveScroll("start");
			this.m_phoneTray.gameObject.SetActive(false);
		}
		if (this.m_bottomPanel != null)
		{
			this.m_origBottomPanelPos = this.m_bottomPanel.transform.localPosition;
		}
		if (this.m_phoneBackButton != null)
		{
			this.m_phoneBackButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
			{
				this.OnBackButtonPressed(e);
			});
		}
		TransformUtil.CopyLocal(this.m_customDeckInitialPosition, this.m_customDeckButton.transform);
	}

	// Token: 0x06002E5D RID: 11869 RVA: 0x000E88BC File Offset: 0x000E6ABC
	private void OnBackButtonPressed(UIEvent e)
	{
		Navigation.GoBack();
	}

	// Token: 0x06002E5E RID: 11870 RVA: 0x000E88C4 File Offset: 0x000E6AC4
	private IEnumerator BackOut()
	{
		CollectionManagerDisplay.Get().EnableInput(false);
		Navigation.PopUnique(new Navigation.NavigateBackHandler(CollectionDeckTray.Get().OnBackOutOfDeckContents));
		yield return base.StartCoroutine(this.ShowPacks(false));
		CollectionDeckTray deckTray = CollectionDeckTray.Get();
		deckTray.OnBackOutOfDeckContentsImpl(true);
		while (!deckTray.m_cardsContent.HasFinishedExiting())
		{
			yield return null;
		}
		CollectionManagerDisplay.Get().EnterSelectNewDeckHeroMode();
		HeroPickerDisplay heroPickerDisplay = CollectionManagerDisplay.Get().GetHeroPickerDisplay();
		while (heroPickerDisplay != null && !heroPickerDisplay.IsShown())
		{
			yield return null;
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			base.StartCoroutine(this.HideTrays());
		}
		CollectionManagerDisplay.Get().EnableInput(true);
		yield break;
	}

	// Token: 0x06002E5F RID: 11871 RVA: 0x000E88DF File Offset: 0x000E6ADF
	public bool OnNavigateBack()
	{
		base.StartCoroutine(this.BackOut());
		return true;
	}

	// Token: 0x06002E60 RID: 11872 RVA: 0x000E88EF File Offset: 0x000E6AEF
	public void RegisterOnTemplateDeckChosen(DeckTemplatePicker.OnTemplateDeckChosen dlg)
	{
		this.m_templateDeckChosenListeners.Add(dlg);
	}

	// Token: 0x06002E61 RID: 11873 RVA: 0x000E88FD File Offset: 0x000E6AFD
	public void UnregisterOnTemplateDeckChosen(DeckTemplatePicker.OnTemplateDeckChosen dlg)
	{
		this.m_templateDeckChosenListeners.Remove(dlg);
	}

	// Token: 0x06002E62 RID: 11874 RVA: 0x000E890C File Offset: 0x000E6B0C
	public bool IsShowingBottomPanel()
	{
		return this.m_showingBottomPanel;
	}

	// Token: 0x06002E63 RID: 11875 RVA: 0x000E8914 File Offset: 0x000E6B14
	public bool IsShowingPacks()
	{
		return this.m_packsShown;
	}

	// Token: 0x06002E64 RID: 11876 RVA: 0x000E891C File Offset: 0x000E6B1C
	public IEnumerator Show(bool show)
	{
		if (show)
		{
			Log.Cameron.Print("showing deck template page " + show, new object[0]);
			this.m_root.SetActive(true);
			this.m_showingBottomPanel = false;
			this.m_packsShown = false;
			this.m_pickerButtonRoot.UpdatePositions();
			TransformUtil.CopyLocal(this.m_customDeckButton.transform, this.m_customDeckInitialPosition);
			UberText customDeckText = this.m_customDeckButton.GetComponentInChildren<UberText>();
			customDeckText.Text = GameStrings.Get(GameStrings.Get("GLUE_DECK_TEMPLATE_CUSTOM_DECK"));
			if (CollectionManager.Get().GetTaggedDeck(CollectionManager.DeckTag.Editing) == null)
			{
				yield break;
			}
			this.SetupTemplateButtons(this.m_customDeck);
			this.m_chooseButton.Disable();
			if (this.m_deckTemplateDescription != null)
			{
				this.m_deckTemplateDescription.Text = GameStrings.Get("GLUE_COLLECTION_DECK_TEMPLATE_SELECT_A_DECK");
			}
			DeckTrayCardListContent cardsContent = CollectionDeckTray.Get().GetCardsContent();
			cardsContent.ResetFakeDeck();
			this.FillWithCustomDeck();
			this.m_currentSelectedDeck = this.m_customDeck;
			if (!UniversalInputManager.UsePhoneUI)
			{
				this.OnTrayToggled(true);
			}
			Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
			if (!CollectionManager.Get().ShouldShowDeckTemplatePageForClass(this.m_currentSelectedClass) && !UniversalInputManager.UsePhoneUI)
			{
				this.SelectCustomDeckButton(true);
			}
			this.ShowBottomPanel(true);
			yield return base.StartCoroutine(this.ShowPacks(true));
			while (CollectionDeckTray.Get() == null || CollectionDeckTray.Get().GetCurrentContentType() != CollectionDeckTray.DeckContentTypes.Cards)
			{
				yield return null;
			}
		}
		else if (this.m_root.activeSelf)
		{
			yield return base.StartCoroutine(this.ShowPacks(false));
			DeckTrayCardListContent cardsContent2 = CollectionDeckTray.Get().GetCardsContent();
			cardsContent2.ResetFakeDeck();
			this.ShowBottomPanel(true);
			this.m_root.SetActive(false);
		}
		yield break;
	}

	// Token: 0x06002E65 RID: 11877 RVA: 0x000E8948 File Offset: 0x000E6B48
	private void SetupTemplateButtons(CollectionManager.TemplateDeck refDeck)
	{
		List<CollectionManager.TemplateDeck> templateDecks = CollectionManager.Get().GetTemplateDecks(this.m_currentSelectedClass);
		int num = 0;
		while (num < this.m_pickerButtons.Count && num < templateDecks.Count)
		{
			CollectionManager.TemplateDeck templateDeck = templateDecks[num];
			bool flag = refDeck == templateDeck;
			if (flag)
			{
				this.m_currentSelectedDeck = templateDeck;
			}
			this.m_pickerButtons[num].SetSelected(false);
			if (flag && this.m_deckTemplateDescription != null)
			{
				this.m_deckTemplateDescription.Text = templateDeck.m_description;
			}
			if (flag && this.m_deckTemplatePhoneName != null)
			{
				this.m_deckTemplatePhoneName.Text = templateDeck.m_title;
			}
			this.m_pickerButtons[num].transform.localEulerAngles = Vector3.zero;
			this.m_pickerButtons[num].GetComponent<RandomTransform>().Apply();
			this.m_pickerButtons[num].GetComponent<AnimatedLowPolyPack>().Init(0, this.m_pickerButtons[num].transform.localPosition, this.m_pickerButtons[num].transform.localPosition + this.m_offscreenPackOffset, false, false);
			this.m_pickerButtons[num].GetComponent<AnimatedLowPolyPack>().SetFlyingLocalRotations(this.m_pickerButtons[num].transform.localEulerAngles, this.m_pickerButtons[num].transform.localEulerAngles);
			num++;
		}
		if (this.m_customDeckButton != null)
		{
			this.m_customDeckButton.SetSelected(false);
			this.m_customDeckButton.transform.localEulerAngles = Vector3.zero;
			this.m_customDeckButton.GetComponent<AnimatedLowPolyPack>().Init(0, this.m_customDeckButton.transform.localPosition, this.m_customDeckButton.transform.localPosition + this.m_offscreenPackOffset, false, false);
			this.m_customDeckButton.GetComponent<AnimatedLowPolyPack>().SetFlyingLocalRotations(this.m_customDeckButton.transform.localEulerAngles, this.m_customDeckButton.transform.localEulerAngles);
		}
	}

	// Token: 0x06002E66 RID: 11878 RVA: 0x000E8B70 File Offset: 0x000E6D70
	public IEnumerator ShowPacks(bool show)
	{
		float delay = 0f;
		if (show)
		{
			HeroPickerDisplay heroPickerDisplay = CollectionManagerDisplay.Get().GetHeroPickerDisplay();
			while (heroPickerDisplay != null && !heroPickerDisplay.IsHidden())
			{
				yield return new WaitForEndOfFrame();
			}
		}
		DeckTemplatePickerButton[] randomButtons = this.m_pickerButtons.ToArray();
		GeneralUtils.Shuffle<DeckTemplatePickerButton>(randomButtons);
		foreach (DeckTemplatePickerButton selectorButton in randomButtons)
		{
			if (show)
			{
				selectorButton.GetComponent<AnimatedLowPolyPack>().FlyIn(this.m_packAnimInTime, delay);
			}
			else
			{
				selectorButton.GetComponent<AnimatedLowPolyPack>().FlyOut(this.m_packAnimOutTime, delay);
			}
			yield return new WaitForSeconds(Random.Range(0.2f * this.m_packAnimInTime, 0.4f * this.m_packAnimInTime));
		}
		if (show)
		{
			this.m_customDeckButton.GetComponent<AnimatedLowPolyPack>().FlyIn(this.m_packAnimInTime, delay);
			yield return new WaitForSeconds(this.m_packAnimInTime + delay);
		}
		else
		{
			this.m_customDeckButton.GetComponent<AnimatedLowPolyPack>().FlyOut(this.m_packAnimOutTime, delay);
			yield return new WaitForSeconds(this.m_packAnimOutTime + delay);
		}
		this.m_packsShown = show;
		yield break;
	}

	// Token: 0x06002E67 RID: 11879 RVA: 0x000E8B9C File Offset: 0x000E6D9C
	public void ShowBottomPanel(bool show)
	{
		if (this.m_bottomPanel != null)
		{
			Vector3 vector = this.m_origBottomPanelPos;
			Vector3 vector2 = this.m_origBottomPanelPos;
			float num = 0f;
			if (show)
			{
				vector2 += this.m_bottomPanelHideOffset;
				num = this.m_bottomPanelSlideInWaitDelay;
				this.m_showingBottomPanel = true;
			}
			else
			{
				vector += this.m_bottomPanelHideOffset;
				ApplicationMgr.Get().ScheduleCallback(this.m_bottomPanelAnimateTime, false, delegate(object o)
				{
					this.m_showingBottomPanel = show;
				}, null);
			}
			iTween.Stop(this.m_bottomPanel);
			this.m_bottomPanel.transform.localPosition = vector2;
			iTween.MoveTo(this.m_bottomPanel, iTween.Hash(new object[]
			{
				"position",
				vector,
				"isLocal",
				true,
				"time",
				this.m_bottomPanelAnimateTime,
				"delay",
				num
			}));
		}
	}

	// Token: 0x06002E68 RID: 11880 RVA: 0x000E8CB4 File Offset: 0x000E6EB4
	public void OnTrayToggled(bool shown)
	{
		if (shown)
		{
			base.StartCoroutine(this.ShowTutorialPopup());
		}
		else
		{
			CollectionManagerDisplay.Get().SetViewMode(CollectionManagerDisplay.ViewMode.CARDS, true, null);
			base.StartCoroutine(CollectionManagerDisplay.Get().ShowDeckTemplateTipsIfNeeded());
		}
	}

	// Token: 0x06002E69 RID: 11881 RVA: 0x000E8CF8 File Offset: 0x000E6EF8
	private IEnumerator ShowTutorialPopup()
	{
		yield return new WaitForSeconds(0.5f);
		if (!Options.Get().GetBool(Option.HAS_SEEN_DECK_TEMPLATE_SCREEN, false) && UserAttentionManager.CanShowAttentionGrabber("DeckTemplatePicker.ShowTutorialPopup:" + Option.HAS_SEEN_DECK_TEMPLATE_SCREEN))
		{
			Transform popupPosition = CollectionManagerDisplay.Get().m_deckTemplateTutorialWelcomeBone;
			NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, popupPosition.localPosition, GameStrings.Get("GLUE_COLLECTION_TUTORIAL_TEMPLATE_WELCOME"), "VO_INNKEEPER_Male_Dwarf_RECIPE1_01", 3f, null);
			Options.Get().SetBool(Option.HAS_SEEN_DECK_TEMPLATE_SCREEN, true);
		}
		yield break;
	}

	// Token: 0x06002E6A RID: 11882 RVA: 0x000E8D0C File Offset: 0x000E6F0C
	public void SetDeckClass(TAG_CLASS deckClass)
	{
		this.m_currentSelectedClass = deckClass;
		List<CollectionManager.TemplateDeck> templateDecks = CollectionManager.Get().GetTemplateDecks(this.m_currentSelectedClass);
		int num = (templateDecks == null) ? 0 : templateDecks.Count;
		Color color = CollectionPageManager.s_classColors[deckClass];
		this.m_pageHeaderText.Text = GameStrings.Format("GLUE_DECK_TEMPLATE_CHOOSE_DECK", new object[]
		{
			GameStrings.GetClassName(deckClass)
		});
		CollectionPageDisplay.SetClassFlavorTextures(this.m_pageHeader, CollectionPageDisplay.TagClassToHeaderClass(deckClass));
		for (int i = 0; i < this.m_pickerButtons.Count; i++)
		{
			DeckTemplatePickerButton deckTemplatePickerButton = this.m_pickerButtons[i];
			bool flag = i < num;
			deckTemplatePickerButton.gameObject.SetActive(flag);
			if (flag)
			{
				CollectionManager.TemplateDeck templateDeck = templateDecks[i];
				deckTemplatePickerButton.SetTitleText(templateDeck.m_title);
				int num2 = 0;
				foreach (KeyValuePair<string, int> keyValuePair in templateDeck.m_cardIds)
				{
					int num3;
					int num4;
					CollectionManager.Get().GetOwnedCardCount(keyValuePair.Key, out num3, out num4);
					int num5 = Mathf.Min(num3 + num4, keyValuePair.Value);
					num2 += num5;
				}
				deckTemplatePickerButton.SetCardCountText(num2);
				deckTemplatePickerButton.SetDeckTexture(templateDeck.m_displayTexture);
				deckTemplatePickerButton.m_packRibbon.material.color = color;
			}
		}
		if (this.m_customDeckButton != null)
		{
			this.m_customDeckButton.m_deckTexture.material.mainTextureOffset = CollectionPageManager.s_classTextureOffsets[deckClass];
			this.m_customDeckButton.m_packRibbon.material.color = color;
		}
	}

	// Token: 0x06002E6B RID: 11883 RVA: 0x000E8ED0 File Offset: 0x000E70D0
	private void SelectButtonWithIndex(int index)
	{
		Action action = delegate()
		{
			if (this.m_chooseButton != null)
			{
				this.m_chooseButton.Enable();
			}
			List<CollectionManager.TemplateDeck> templateDecks = CollectionManager.Get().GetTemplateDecks(this.m_currentSelectedClass);
			CollectionManager.TemplateDeck templateDeck = this.m_customDeck;
			if (templateDecks != null && index < templateDecks.Count)
			{
				templateDeck = templateDecks[index];
			}
			for (int i = 0; i < this.m_pickerButtons.Count; i++)
			{
				this.m_pickerButtons[i].SetSelected(i == index);
			}
			if (this.m_deckTemplateDescription != null)
			{
				this.m_deckTemplateDescription.Text = templateDeck.m_description;
			}
			if (this.m_deckTemplatePhoneName != null)
			{
				this.m_deckTemplatePhoneName.Text = templateDeck.m_title;
			}
			if (this.m_customDeckButton != null)
			{
				this.m_customDeckButton.SetSelected(false);
			}
			this.m_currentSelectedDeck = templateDeck;
			if (UniversalInputManager.UsePhoneUI)
			{
				if (this.m_phoneTray.GetComponent<SlidingTray>().TraySliderIsAnimating())
				{
					return;
				}
				this.m_phoneTray.gameObject.SetActive(true);
				this.m_phoneTray.GetComponent<SlidingTray>().ShowTray();
				this.m_phoneTray.m_scrollbar.LoadScroll("start");
				this.m_phoneTray.FlashDeckTemplateHighlight();
			}
			else
			{
				CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
				if (collectionDeckTray != null)
				{
					collectionDeckTray.FlashDeckTemplateHighlight();
				}
			}
			this.FillDeckWithTemplate(this.m_currentSelectedDeck);
			this.StartCoroutine(this.ShowTips());
		};
		if (index < this.m_pickerButtons.Count && this.m_pickerButtons[index].GetOwnedCardCount() < 10 && !this.m_pickerButtons[index].IsStarterDeck())
		{
			this.ShowLowCardsPopup(action, null);
		}
		else
		{
			action.Invoke();
		}
	}

	// Token: 0x06002E6C RID: 11884 RVA: 0x000E8F60 File Offset: 0x000E7160
	public IEnumerator ShowTips()
	{
		if (UniversalInputManager.UsePhoneUI)
		{
			while (this.m_phoneTray.GetComponent<SlidingTray>().TraySliderIsAnimating())
			{
				yield return null;
			}
		}
		yield return base.StartCoroutine(CollectionManagerDisplay.Get().ShowDeckTemplateTipsIfNeeded());
		yield break;
	}

	// Token: 0x06002E6D RID: 11885 RVA: 0x000E8F7C File Offset: 0x000E717C
	private void FillDeckWithTemplate(CollectionManager.TemplateDeck tplDeck)
	{
		CollectionDeck editingDeck = CollectionDeckTray.Get().GetCardsContent().GetEditingDeck();
		if (editingDeck == null)
		{
			return;
		}
		if (tplDeck == null)
		{
			CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
			editingDeck.CopyFrom(editedDeck);
		}
		else
		{
			editingDeck.FillFromTemplateDeck(tplDeck);
		}
		CollectionDeckTray.Get().m_cardsContent.UpdateCardList(true, null);
		CollectionDeckTray.Get().m_decksContent.UpdateDeckName(null);
		if (this.m_phoneTray != null)
		{
			CollectionDeck editingDeck2 = this.m_phoneTray.m_cardsContent.GetEditingDeck();
			if (tplDeck == null)
			{
				CollectionDeck editedDeck2 = CollectionManager.Get().GetEditedDeck();
				editingDeck2.CopyFrom(editedDeck2);
			}
			else
			{
				editingDeck2.FillFromTemplateDeck(tplDeck);
			}
			this.m_phoneTray.m_cardsContent.UpdateCardList(true, null);
			SceneUtils.SetLayer(this.m_phoneTray, GameLayer.IgnoreFullScreenEffects);
		}
	}

	// Token: 0x06002E6E RID: 11886 RVA: 0x000E904A File Offset: 0x000E724A
	private void FillWithCustomDeck()
	{
		this.FillDeckWithTemplate(null);
	}

	// Token: 0x06002E6F RID: 11887 RVA: 0x000E9054 File Offset: 0x000E7254
	private void FireOnTemplateDeckChosenEvent()
	{
		DeckTemplatePicker.OnTemplateDeckChosen[] array = this.m_templateDeckChosenListeners.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i]();
		}
	}

	// Token: 0x06002E70 RID: 11888 RVA: 0x000E908C File Offset: 0x000E728C
	private IEnumerator HideTrays()
	{
		SlidingTray phoneTray = this.m_phoneTray.GetComponent<SlidingTray>();
		phoneTray.HideTray();
		while (phoneTray.isActiveAndEnabled && !phoneTray.IsTrayInShownPosition())
		{
			yield return new WaitForEndOfFrame();
		}
		SlidingTray templateTray = base.GetComponent<SlidingTray>();
		templateTray.HideTray();
		yield break;
	}

	// Token: 0x06002E71 RID: 11889 RVA: 0x000E90A8 File Offset: 0x000E72A8
	private void ChooseRecipeAndFillInCards()
	{
		CollectionManager.Get().SetShowDeckTemplatePageForClass(this.m_currentSelectedClass, this.m_currentSelectedDeck != this.m_customDeck);
		CollectionDeckTray.Get().GetCardsContent().CommitFakeDeckChanges();
		this.FireOnTemplateDeckChosenEvent();
		CollectionDeck taggedDeck = CollectionManager.Get().GetTaggedDeck(CollectionManager.DeckTag.Editing);
		if (this.m_currentSelectedDeck != this.m_customDeck)
		{
			taggedDeck.SourceType = 2;
			Network.Get().SetDeckTemplateSource(taggedDeck.ID, this.m_currentSelectedDeck.m_id);
		}
		Navigation.PopUnique(new Navigation.NavigateBackHandler(this.OnNavigateBack));
		if (UniversalInputManager.UsePhoneUI)
		{
			base.StartCoroutine(this.EnterDeckPhone());
		}
		if (CollectionManager.Get().ShouldShowWildToStandardTutorial(true) && !taggedDeck.IsWild)
		{
			CollectionManagerDisplay.Get().ShowStandardInfoTutorial(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS);
		}
	}

	// Token: 0x06002E72 RID: 11890 RVA: 0x000E9180 File Offset: 0x000E7380
	private void ShowLowCardsPopup(Action confirmAction, Action cancelAction = null)
	{
		AlertPopup.PopupInfo info = new AlertPopup.PopupInfo
		{
			m_headerText = GameStrings.Get("GLUE_COLLECTION_DECK_TEMPLATE_LOW_CARD_WARNING"),
			m_text = GameStrings.Get("GLUE_COLLECTION_DECK_TEMPLATE_LOW_CARD_WARNING_MESSAGE"),
			m_cancelText = GameStrings.Get("GLUE_COLLECTION_DECK_TEMPLATE_LOW_CARD_WARNING_CANCEL"),
			m_confirmText = GameStrings.Get("GLUE_COLLECTION_DECK_TEMPLATE_LOW_CARD_WARNING_CONFIRM"),
			m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
			m_responseCallback = delegate(AlertPopup.Response response, object userData)
			{
				if (response == AlertPopup.Response.CONFIRM)
				{
					confirmAction.Invoke();
				}
				else if (response == AlertPopup.Response.CANCEL && cancelAction != null)
				{
					cancelAction.Invoke();
				}
			}
		};
		DialogManager.Get().ShowPopup(info);
	}

	// Token: 0x06002E73 RID: 11891 RVA: 0x000E9210 File Offset: 0x000E7410
	private void SelectCustomDeckButton(bool preselect = false)
	{
		CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
		if (collectionDeckTray != null && !preselect)
		{
			collectionDeckTray.FlashDeckTemplateHighlight();
		}
		if (this.m_chooseButton != null)
		{
			this.m_chooseButton.Enable();
		}
		for (int i = 0; i < this.m_pickerButtons.Count; i++)
		{
			this.m_pickerButtons[i].SetSelected(false);
		}
		if (this.m_customDeckButton != null)
		{
			this.m_customDeckButton.SetSelected(true);
		}
		if (this.m_deckTemplateDescription != null)
		{
			this.m_deckTemplateDescription.Text = GameStrings.Get("GLUE_DECK_TEMPLATE_CUSTOM_DECK_DESCRIPTION");
		}
		this.FillWithCustomDeck();
		this.m_currentSelectedDeck = this.m_customDeck;
		if (UniversalInputManager.UsePhoneUI && !preselect)
		{
			this.ChooseRecipeAndFillInCards();
		}
	}

	// Token: 0x06002E74 RID: 11892 RVA: 0x000E92F8 File Offset: 0x000E74F8
	private IEnumerator EnterDeckPhone()
	{
		yield return base.StartCoroutine(this.ShowPacks(false));
		yield return base.StartCoroutine(this.HideTrays());
		yield break;
	}

	// Token: 0x04001CCB RID: 7371
	public GameObject m_root;

	// Token: 0x04001CCC RID: 7372
	public GameObject m_pageHeader;

	// Token: 0x04001CCD RID: 7373
	public UberText m_pageHeaderText;

	// Token: 0x04001CCE RID: 7374
	public UIBObjectSpacing m_pickerButtonRoot;

	// Token: 0x04001CCF RID: 7375
	public DeckTemplatePickerButton m_pickerButtonTpl;

	// Token: 0x04001CD0 RID: 7376
	public DeckTemplatePickerButton m_customDeckButton;

	// Token: 0x04001CD1 RID: 7377
	public UberText m_deckTemplateDescription;

	// Token: 0x04001CD2 RID: 7378
	public UberText m_deckTemplatePhoneName;

	// Token: 0x04001CD3 RID: 7379
	public PlayButton m_chooseButton;

	// Token: 0x04001CD4 RID: 7380
	public GameObject m_bottomPanel;

	// Token: 0x04001CD5 RID: 7381
	public DeckTemplatePhoneTray m_phoneTray;

	// Token: 0x04001CD6 RID: 7382
	public UIBButton m_phoneBackButton;

	// Token: 0x04001CD7 RID: 7383
	public Vector3 m_bottomPanelHideOffset = new Vector3(0f, 0f, 25f);

	// Token: 0x04001CD8 RID: 7384
	public float m_bottomPanelSlideInWaitDelay = 0.25f;

	// Token: 0x04001CD9 RID: 7385
	public float m_bottomPanelAnimateTime = 0.25f;

	// Token: 0x04001CDA RID: 7386
	public float m_packAnimInTime = 0.25f;

	// Token: 0x04001CDB RID: 7387
	public float m_packAnimOutTime = 0.2f;

	// Token: 0x04001CDC RID: 7388
	public Vector3 m_offscreenPackOffset;

	// Token: 0x04001CDD RID: 7389
	public Transform m_ghostCardTipBone;

	// Token: 0x04001CDE RID: 7390
	private List<DeckTemplatePickerButton> m_pickerButtons = new List<DeckTemplatePickerButton>();

	// Token: 0x04001CDF RID: 7391
	private CollectionManager.TemplateDeck m_customDeck = new CollectionManager.TemplateDeck();

	// Token: 0x04001CE0 RID: 7392
	private TAG_CLASS m_currentSelectedClass;

	// Token: 0x04001CE1 RID: 7393
	private CollectionManager.TemplateDeck m_currentSelectedDeck;

	// Token: 0x04001CE2 RID: 7394
	private List<DeckTemplatePicker.OnTemplateDeckChosen> m_templateDeckChosenListeners = new List<DeckTemplatePicker.OnTemplateDeckChosen>();

	// Token: 0x04001CE3 RID: 7395
	private Vector3 m_origBottomPanelPos;

	// Token: 0x04001CE4 RID: 7396
	private bool m_showingBottomPanel;

	// Token: 0x04001CE5 RID: 7397
	private TransformProps m_customDeckInitialPosition = new TransformProps();

	// Token: 0x04001CE6 RID: 7398
	private bool m_packsShown;

	// Token: 0x02000708 RID: 1800
	// (Invoke) Token: 0x060049ED RID: 18925
	public delegate void OnTemplateDeckChosen();
}
