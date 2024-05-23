using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000399 RID: 921
[CustomEditClass]
public class CollectionDeckBoxVisual : PegUIElement
{
	// Token: 0x0600300E RID: 12302 RVA: 0x000F1C2C File Offset: 0x000EFE2C
	protected override void Awake()
	{
		base.Awake();
		this.SetEnabled(false);
		this.m_deleteButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDeleteButtonPressed));
		this.m_deleteButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnDeleteButtonOver));
		this.m_deleteButton.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnDeleteButtonRollout));
		this.ShowDeleteButton(false);
		this.m_invalidDeckX.SetActive(this.m_isMissingCards);
		this.m_deckName.RichText = false;
		SoundManager.Get().Load("tiny_button_press_1");
		SoundManager.Get().Load("tiny_button_mouseover_1");
		this.m_customDeckTransform = base.transform.FindChild("CustomDeck");
		this.m_highlightState = this.m_highlight.GetComponentsInChildren<HighlightState>(true)[0];
		if (PlatformSettings.s_screen == ScreenCategory.Phone)
		{
			this.SCALED_UP_LOCAL_SCALE = new Vector3(1.1f, 1.1f, 1.1f);
			this.SCALED_UP_DECK_OFFSET = new Vector3(0f, -0.2f, 0f);
		}
		this.m_wasTouchModeEnabled = UniversalInputManager.Get().IsTouchMode();
	}

	// Token: 0x0600300F RID: 12303 RVA: 0x000F1D50 File Offset: 0x000EFF50
	private void Update()
	{
		if (this.m_wasTouchModeEnabled != UniversalInputManager.Get().IsTouchMode())
		{
			PegUIElement.InteractionState interactionState = base.GetInteractionState();
			if (this.m_wasTouchModeEnabled)
			{
				if (interactionState == PegUIElement.InteractionState.Down)
				{
					this.OnPressEvent();
				}
				else if (interactionState == PegUIElement.InteractionState.Over)
				{
					this.OnOverEvent();
				}
			}
			else
			{
				if (interactionState == PegUIElement.InteractionState.Down)
				{
					this.OnReleaseEvent();
				}
				else if (interactionState == PegUIElement.InteractionState.Over)
				{
					this.OnOutEvent();
				}
				this.ShowDeleteButton(false);
			}
			this.m_wasTouchModeEnabled = UniversalInputManager.Get().IsTouchMode();
		}
	}

	// Token: 0x06003010 RID: 12304 RVA: 0x000F1DDE File Offset: 0x000EFFDE
	public void Show()
	{
		base.gameObject.SetActive(true);
		this.m_isShown = true;
	}

	// Token: 0x06003011 RID: 12305 RVA: 0x000F1DF3 File Offset: 0x000EFFF3
	public void Hide()
	{
		base.gameObject.SetActive(false);
		this.m_isShown = false;
	}

	// Token: 0x06003012 RID: 12306 RVA: 0x000F1E08 File Offset: 0x000F0008
	public bool IsShown()
	{
		return this.m_isShown;
	}

	// Token: 0x06003013 RID: 12307 RVA: 0x000F1E10 File Offset: 0x000F0010
	public void SetDeckName(string deckName)
	{
		this.m_deckName.Text = deckName;
	}

	// Token: 0x06003014 RID: 12308 RVA: 0x000F1E1E File Offset: 0x000F001E
	public UberText GetDeckNameText()
	{
		return this.m_deckName;
	}

	// Token: 0x06003015 RID: 12309 RVA: 0x000F1E26 File Offset: 0x000F0026
	public void HideDeckName()
	{
		this.m_deckName.gameObject.SetActive(false);
	}

	// Token: 0x06003016 RID: 12310 RVA: 0x000F1E39 File Offset: 0x000F0039
	public void ShowDeckName()
	{
		this.m_deckName.gameObject.SetActive(true);
	}

	// Token: 0x06003017 RID: 12311 RVA: 0x000F1E4C File Offset: 0x000F004C
	public void HideRenameVisuals()
	{
		if (this.m_renameVisuals != null)
		{
			this.m_renameVisuals.SetActive(false);
		}
	}

	// Token: 0x06003018 RID: 12312 RVA: 0x000F1E6B File Offset: 0x000F006B
	public void ShowRenameVisuals()
	{
		if (this.m_renameVisuals != null)
		{
			this.m_renameVisuals.SetActive(true);
		}
	}

	// Token: 0x06003019 RID: 12313 RVA: 0x000F1E8A File Offset: 0x000F008A
	public void SetDeckID(long id)
	{
		this.m_deckID = id;
	}

	// Token: 0x0600301A RID: 12314 RVA: 0x000F1E93 File Offset: 0x000F0093
	public long GetDeckID()
	{
		return this.m_deckID;
	}

	// Token: 0x0600301B RID: 12315 RVA: 0x000F1E9B File Offset: 0x000F009B
	public CollectionDeck GetCollectionDeck()
	{
		return CollectionManager.Get().GetDeck(this.m_deckID);
	}

	// Token: 0x0600301C RID: 12316 RVA: 0x000F1EAD File Offset: 0x000F00AD
	public Texture GetHeroPortraitTexture()
	{
		if (this.m_cardDef == null)
		{
			return null;
		}
		return this.m_cardDef.GetPortraitTexture();
	}

	// Token: 0x0600301D RID: 12317 RVA: 0x000F1ECD File Offset: 0x000F00CD
	public CardDef GetCardDef()
	{
		return this.m_cardDef;
	}

	// Token: 0x0600301E RID: 12318 RVA: 0x000F1ED5 File Offset: 0x000F00D5
	public FullDef GetFullDef()
	{
		return this.m_fullDef;
	}

	// Token: 0x0600301F RID: 12319 RVA: 0x000F1EDD File Offset: 0x000F00DD
	public TAG_CLASS GetClass()
	{
		if (this.m_entityDef == null)
		{
			return TAG_CLASS.INVALID;
		}
		return this.m_entityDef.GetClass();
	}

	// Token: 0x06003020 RID: 12320 RVA: 0x000F1EF7 File Offset: 0x000F00F7
	public string GetHeroCardID()
	{
		return this.m_heroCardID;
	}

	// Token: 0x06003021 RID: 12321 RVA: 0x000F1EFF File Offset: 0x000F00FF
	public void SetHeroCardID(string heroCardID)
	{
		this.m_heroCardID = heroCardID;
		DefLoader.Get().LoadFullDef(heroCardID, new DefLoader.LoadDefCallback<FullDef>(this.OnHeroFullDefLoaded));
	}

	// Token: 0x06003022 RID: 12322 RVA: 0x000F1F1F File Offset: 0x000F011F
	public void SetShowGlow(bool showGlow)
	{
		this.m_showGlow = showGlow;
		if (this.m_showGlow)
		{
			this.SetHighlightState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
		}
	}

	// Token: 0x06003023 RID: 12323 RVA: 0x000F1F3B File Offset: 0x000F013B
	public bool IsWild()
	{
		return this.m_isWild;
	}

	// Token: 0x06003024 RID: 12324 RVA: 0x000F1F44 File Offset: 0x000F0144
	public void TransitionFromStandardToWild()
	{
		this.SetFormat(false);
		Animator component = base.GetComponent<Animator>();
		if (component != null)
		{
			component.enabled = true;
			component.Play("CustomDeck_GlowOut", 0, 0f);
		}
	}

	// Token: 0x06003025 RID: 12325 RVA: 0x000F1F84 File Offset: 0x000F0184
	public void OnTransitionFromStandardToWild()
	{
		this.m_isWild = true;
		this.m_isValid = false;
		this.ReparentElements(this.m_isWild);
		this.m_highlightState.m_StaticSilouetteTexture = ((!this.m_isWild) ? this.m_standardHighlight : this.m_wildHighlight);
		this.m_portraitObject.SetActive(false);
		if (this.m_wildPortraitObject != null)
		{
			this.m_wildPortraitObject.SetActive(true);
			if (!UniversalInputManager.UsePhoneUI)
			{
				this.m_wildPortraitObject.GetComponent<Animator>().Play("Wild_RolldownActivate", 0, 1f);
			}
			else
			{
				this.m_wildPortraitObject.GetComponent<Animator>().Play("WildActivate", 0, 1f);
			}
		}
		base.Invoke("PlayValidAnimation", 1f);
	}

	// Token: 0x06003026 RID: 12326 RVA: 0x000F2058 File Offset: 0x000F0258
	public void SetFormat(bool isWild)
	{
		this.m_isWild = isWild;
		this.ReparentElements(this.m_isWild);
		this.m_deleteButton.GetComponent<Renderer>().material = ((!isWild) ? this.m_xButtonMaterial : this.m_xButtonWildMaterial);
		this.m_portraitObject.SetActive(!isWild);
		this.m_wildPortraitObject.SetActive(isWild);
		this.m_highlightState.m_StaticSilouetteTexture = ((!isWild) ? this.m_standardHighlight : this.m_wildHighlight);
		this.PlayValidAnimation();
	}

	// Token: 0x06003027 RID: 12327 RVA: 0x000F20E2 File Offset: 0x000F02E2
	public void SetPositionIndex(int idx)
	{
		this.m_positionIndex = idx;
	}

	// Token: 0x06003028 RID: 12328 RVA: 0x000F20EB File Offset: 0x000F02EB
	public int GetPositionIndex()
	{
		return this.m_positionIndex;
	}

	// Token: 0x06003029 RID: 12329 RVA: 0x000F20F4 File Offset: 0x000F02F4
	public void SetBasicSetProgress(TAG_CLASS classTag)
	{
		int basicCardsIOwn = CollectionManager.Get().GetBasicCardsIOwn(classTag);
		int num = 20;
		if (basicCardsIOwn == num || SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER)
		{
			this.m_deckName.transform.position = this.m_bones.m_deckLabelOneLine.position;
			this.m_labelGradient.transform.parent = this.m_bones.m_gradientOneLine;
			this.m_labelGradient.transform.localPosition = Vector3.zero;
			this.m_labelGradient.transform.localScale = Vector3.one;
			this.m_setProgressLabel.gameObject.SetActive(false);
		}
		else
		{
			this.m_deckName.transform.position = this.m_bones.m_deckLabelTwoLine.position;
			this.m_labelGradient.transform.parent = this.m_bones.m_gradientTwoLine;
			this.m_labelGradient.transform.localPosition = Vector3.zero;
			this.m_labelGradient.transform.localScale = Vector3.one;
			this.m_setProgressLabel.gameObject.SetActive(true);
			this.m_setProgressLabel.Text = GameStrings.Format((!UniversalInputManager.UsePhoneUI) ? "GLUE_BASIC_SET_PROGRESS" : "GLUE_BASIC_SET_PROGRESS_PHONE", new object[]
			{
				basicCardsIOwn,
				num
			});
		}
	}

	// Token: 0x0600302A RID: 12330 RVA: 0x000F225E File Offset: 0x000F045E
	public bool IsMissingCards()
	{
		return this.m_isMissingCards;
	}

	// Token: 0x0600302B RID: 12331 RVA: 0x000F2266 File Offset: 0x000F0466
	public void SetIsMissingCards(bool isMissingCards)
	{
		if (this.m_isMissingCards == isMissingCards)
		{
			return;
		}
		this.m_isMissingCards = isMissingCards;
		this.m_invalidDeckX.SetActive(this.m_isMissingCards);
	}

	// Token: 0x0600302C RID: 12332 RVA: 0x000F228D File Offset: 0x000F048D
	public bool IsValid()
	{
		return this.m_isValid;
	}

	// Token: 0x0600302D RID: 12333 RVA: 0x000F2295 File Offset: 0x000F0495
	public void SetIsValid(bool isValid)
	{
		if (this.m_isValid == isValid)
		{
			return;
		}
		this.m_isValid = isValid;
		this.PlayValidAnimation();
	}

	// Token: 0x0600302E RID: 12334 RVA: 0x000F22B1 File Offset: 0x000F04B1
	public bool IsLocked()
	{
		return this.m_isLocked;
	}

	// Token: 0x0600302F RID: 12335 RVA: 0x000F22B9 File Offset: 0x000F04B9
	public void SetIsLocked(bool isLocked)
	{
		if (this.m_isLocked == isLocked)
		{
			return;
		}
		this.m_isLocked = isLocked;
		this.m_normalDeckVisuals.SetActive(!this.m_isLocked);
		this.m_lockedDeckVisuals.SetActive(this.m_isLocked);
	}

	// Token: 0x06003030 RID: 12336 RVA: 0x000F22F4 File Offset: 0x000F04F4
	public void EnableButtonAnimation()
	{
		this.m_animateButtonPress = true;
	}

	// Token: 0x06003031 RID: 12337 RVA: 0x000F22FD File Offset: 0x000F04FD
	public void DisableButtonAnimation()
	{
		this.m_animateButtonPress = false;
	}

	// Token: 0x06003032 RID: 12338 RVA: 0x000F2306 File Offset: 0x000F0506
	public void PlayScaleUpAnimation()
	{
		this.PlayScaleUpAnimation(null);
	}

	// Token: 0x06003033 RID: 12339 RVA: 0x000F230F File Offset: 0x000F050F
	public void PlayScaleUpAnimation(CollectionDeckBoxVisual.DelOnAnimationFinished callback)
	{
		this.PlayScaleUpAnimation(callback, null);
	}

	// Token: 0x06003034 RID: 12340 RVA: 0x000F231C File Offset: 0x000F051C
	public void PlayScaleUpAnimation(CollectionDeckBoxVisual.DelOnAnimationFinished callback, object callbackData)
	{
		CollectionDeckBoxVisual.OnScaleFinishedCallbackData onScaleFinishedCallbackData = new CollectionDeckBoxVisual.OnScaleFinishedCallbackData
		{
			m_callback = callback,
			m_callbackData = callbackData
		};
		Vector3 localPosition = base.transform.localPosition;
		localPosition.y = 3.238702f;
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			localPosition,
			"isLocal",
			true,
			"time",
			0.05f,
			"easetype",
			iTween.EaseType.linear,
			"oncomplete",
			"ScaleUpNow",
			"oncompletetarget",
			base.gameObject,
			"oncompleteparams",
			onScaleFinishedCallbackData
		});
		iTween.MoveTo(base.gameObject, args);
	}

	// Token: 0x06003035 RID: 12341 RVA: 0x000F23EB File Offset: 0x000F05EB
	private void ScaleUpNow(CollectionDeckBoxVisual.OnScaleFinishedCallbackData readyToScaleUpData)
	{
		this.ScaleDeckBox(true, readyToScaleUpData.m_callback, readyToScaleUpData.m_callbackData);
	}

	// Token: 0x06003036 RID: 12342 RVA: 0x000F2400 File Offset: 0x000F0600
	public void PlayScaleDownAnimation()
	{
		this.PlayScaleDownAnimation(null);
	}

	// Token: 0x06003037 RID: 12343 RVA: 0x000F2409 File Offset: 0x000F0609
	public void PlayScaleDownAnimation(CollectionDeckBoxVisual.DelOnAnimationFinished callback)
	{
		this.PlayScaleDownAnimation(callback, null);
	}

	// Token: 0x06003038 RID: 12344 RVA: 0x000F2414 File Offset: 0x000F0614
	public void PlayScaleDownAnimation(CollectionDeckBoxVisual.DelOnAnimationFinished callback, object callbackData)
	{
		CollectionDeckBoxVisual.OnScaleFinishedCallbackData callbackData2 = new CollectionDeckBoxVisual.OnScaleFinishedCallbackData
		{
			m_callback = callback,
			m_callbackData = callbackData
		};
		this.ScaleDeckBox(false, new CollectionDeckBoxVisual.DelOnAnimationFinished(this.OnScaledDown), callbackData2);
	}

	// Token: 0x06003039 RID: 12345 RVA: 0x000F244C File Offset: 0x000F064C
	private void OnScaledDown(object callbackData)
	{
		CollectionDeckBoxVisual.OnScaleFinishedCallbackData onScaleFinishedCallbackData = callbackData as CollectionDeckBoxVisual.OnScaleFinishedCallbackData;
		Vector3 localPosition = base.transform.localPosition;
		localPosition.y = 1.273138f;
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			localPosition,
			"isLocal",
			true,
			"time",
			0.05f,
			"easetype",
			iTween.EaseType.linear,
			"oncomplete",
			"ScaleDownComplete",
			"oncompletetarget",
			base.gameObject,
			"oncompleteparams",
			onScaleFinishedCallbackData
		});
		iTween.MoveTo(base.gameObject, args);
	}

	// Token: 0x0600303A RID: 12346 RVA: 0x000F250C File Offset: 0x000F070C
	private void ScaleDownComplete(CollectionDeckBoxVisual.OnScaleFinishedCallbackData onScaledDownData)
	{
		if (onScaledDownData.m_callback == null)
		{
			return;
		}
		onScaledDownData.m_callback(onScaledDownData.m_callbackData);
	}

	// Token: 0x0600303B RID: 12347 RVA: 0x000F252B File Offset: 0x000F072B
	public void PlayPopUpAnimation()
	{
		this.PlayPopUpAnimation(null);
	}

	// Token: 0x0600303C RID: 12348 RVA: 0x000F2534 File Offset: 0x000F0734
	public void PlayPopUpAnimation(CollectionDeckBoxVisual.DelOnAnimationFinished callback)
	{
		this.PlayPopUpAnimation(callback, null);
	}

	// Token: 0x0600303D RID: 12349 RVA: 0x000F2540 File Offset: 0x000F0740
	public void PlayPopUpAnimation(CollectionDeckBoxVisual.DelOnAnimationFinished callback, object callbackData)
	{
		if (this.m_isPoppedUp)
		{
			if (callback != null)
			{
				callback(callbackData);
			}
			return;
		}
		this.m_isPoppedUp = true;
		if (this.m_customDeckTransform != null)
		{
			this.m_customDeckTransform.localPosition += this.SCALED_UP_DECK_OFFSET;
		}
		base.GetComponent<Animation>()["Deck_PopUp"].time = 0f;
		base.GetComponent<Animation>()["Deck_PopUp"].speed = 6f;
		this.PlayPopAnimation("Deck_PopUp", callback, callbackData);
	}

	// Token: 0x0600303E RID: 12350 RVA: 0x000F25DB File Offset: 0x000F07DB
	public void PlayPopDownAnimation()
	{
		this.PlayPopDownAnimation(null);
	}

	// Token: 0x0600303F RID: 12351 RVA: 0x000F25E4 File Offset: 0x000F07E4
	public void PlayPopDownAnimation(CollectionDeckBoxVisual.DelOnAnimationFinished callback)
	{
		this.PlayPopDownAnimation(callback, null);
	}

	// Token: 0x06003040 RID: 12352 RVA: 0x000F25F0 File Offset: 0x000F07F0
	public void PlayPopDownAnimation(CollectionDeckBoxVisual.DelOnAnimationFinished callback, object callbackData)
	{
		if (!this.m_isPoppedUp)
		{
			if (callback != null)
			{
				callback(callbackData);
			}
			return;
		}
		this.m_isPoppedUp = false;
		if (this.m_customDeckTransform != null)
		{
			this.m_customDeckTransform.localPosition -= this.SCALED_UP_DECK_OFFSET;
		}
		base.GetComponent<Animation>()["Deck_PopDown"].time = 0f;
		base.GetComponent<Animation>()["Deck_PopDown"].speed = 6f;
		this.PlayPopAnimation("Deck_PopDown", callback, callbackData);
	}

	// Token: 0x06003041 RID: 12353 RVA: 0x000F268B File Offset: 0x000F088B
	public void PlayPopDownAnimationImmediately()
	{
		this.PlayPopDownAnimationImmediately(null);
	}

	// Token: 0x06003042 RID: 12354 RVA: 0x000F2694 File Offset: 0x000F0894
	public void PlayPopDownAnimationImmediately(CollectionDeckBoxVisual.DelOnAnimationFinished callback)
	{
		this.PlayPopDownAnimationImmediately(callback, null);
	}

	// Token: 0x06003043 RID: 12355 RVA: 0x000F26A0 File Offset: 0x000F08A0
	public void PlayPopDownAnimationImmediately(CollectionDeckBoxVisual.DelOnAnimationFinished callback, object callbackData)
	{
		if (!this.m_isPoppedUp)
		{
			if (callback != null)
			{
				callback(callbackData);
			}
			return;
		}
		this.m_isPoppedUp = false;
		base.GetComponent<Animation>()["Deck_PopDown"].time = base.GetComponent<Animation>()["Deck_PopDown"].length;
		base.GetComponent<Animation>()["Deck_PopDown"].speed = 1f;
		this.PlayPopAnimation("Deck_PopDown", callback, callbackData);
	}

	// Token: 0x06003044 RID: 12356 RVA: 0x000F2720 File Offset: 0x000F0920
	public void SetHighlightState(ActorStateType stateType)
	{
		if (this.m_highlightState != null)
		{
			if (!this.m_highlightState.IsReady())
			{
				base.StartCoroutine(this.ChangeHighlightStateWhenReady(stateType));
			}
			else
			{
				this.m_highlightState.ChangeState(stateType);
			}
		}
	}

	// Token: 0x06003045 RID: 12357 RVA: 0x000F2770 File Offset: 0x000F0970
	private IEnumerator ChangeHighlightStateWhenReady(ActorStateType stateType)
	{
		while (!this.m_highlightState.IsReady())
		{
			yield return null;
		}
		this.m_highlightState.ChangeState(stateType);
		yield break;
	}

	// Token: 0x06003046 RID: 12358 RVA: 0x000F2799 File Offset: 0x000F0999
	public void ShowDeleteButton(bool show)
	{
		this.m_deleteButton.gameObject.SetActive(show);
	}

	// Token: 0x06003047 RID: 12359 RVA: 0x000F27AC File Offset: 0x000F09AC
	public void SetOriginalButtonPosition()
	{
		if (this.m_isWild)
		{
			this.m_originalButtonPosition = this.m_portraitObject.transform.localPosition;
		}
		else if (this.m_wildPortraitObject != null)
		{
			this.m_originalButtonPosition = this.m_wildPortraitObject.transform.localPosition;
		}
	}

	// Token: 0x06003048 RID: 12360 RVA: 0x000F2808 File Offset: 0x000F0A08
	public void HideBanner()
	{
		this.m_classObject.SetActive(false);
		if (this.m_wildClassObject != null)
		{
			this.m_wildClassObject.SetActive(false);
		}
		if (this.m_topBannerRenderer != null)
		{
			this.m_topBannerRenderer.gameObject.SetActive(false);
		}
		if (this.m_wildTopBannerRenderer != null)
		{
			this.m_wildTopBannerRenderer.gameObject.SetActive(false);
		}
	}

	// Token: 0x06003049 RID: 12361 RVA: 0x000F2884 File Offset: 0x000F0A84
	public void ShowBanner()
	{
		this.m_classObject.SetActive(true);
		if (this.m_wildClassObject != null)
		{
			this.m_wildClassObject.SetActive(true);
		}
		if (this.m_topBannerRenderer != null)
		{
			this.m_topBannerRenderer.gameObject.SetActive(true);
		}
		if (this.m_wildTopBannerRenderer != null)
		{
			this.m_wildTopBannerRenderer.gameObject.SetActive(true);
		}
	}

	// Token: 0x0600304A RID: 12362 RVA: 0x000F2900 File Offset: 0x000F0B00
	public void AssignFromCollectionDeck(CollectionDeck deck, bool setTourneyValid)
	{
		if (deck == null)
		{
			return;
		}
		this.SetDeckName(deck.Name);
		this.SetDeckID(deck.ID);
		this.SetHeroCardID(deck.HeroCardID);
		this.SetShowGlow(deck.NeedsName);
		this.SetFormat(CollectionManager.Get().IsShowingWildTheming(deck));
		if (setTourneyValid)
		{
			this.SetIsValid(deck.IsTourneyValid);
		}
	}

	// Token: 0x0600304B RID: 12363 RVA: 0x000F2967 File Offset: 0x000F0B67
	private void OnDeleteButtonRollout(UIEvent e)
	{
		this.ShowDeleteButton(false);
	}

	// Token: 0x0600304C RID: 12364 RVA: 0x000F2970 File Offset: 0x000F0B70
	private void OnDeleteButtonOver(UIEvent e)
	{
		SoundManager.Get().LoadAndPlay("tiny_button_mouseover_1", base.gameObject);
	}

	// Token: 0x0600304D RID: 12365 RVA: 0x000F2988 File Offset: 0x000F0B88
	private void OnDeleteButtonPressed(UIEvent e)
	{
		if (CollectionDeckTray.Get().IsShowingDeckContents())
		{
			return;
		}
		SoundManager.Get().LoadAndPlay("tiny_button_press_1", base.gameObject);
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get("GLUE_COLLECTION_DELETE_CONFIRM_HEADER");
		popupInfo.m_showAlertIcon = false;
		if (AchieveManager.Get().HasUnlockedFeature(Achievement.UnlockableFeature.VANILLA_HEROES))
		{
			popupInfo.m_text = GameStrings.Get("GLUE_COLLECTION_DELETE_CONFIRM_DESC");
			popupInfo.m_alertTextAlignment = UberText.AlignmentOptions.Center;
			popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
			popupInfo.m_responseCallback = new AlertPopup.ResponseCallback(this.OnDeleteButtonConfirmationResponse);
		}
		else
		{
			popupInfo.m_text = GameStrings.Get("GLUE_COLLECTION_DELETE_UNAVAILABLE_DESC");
			popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
			popupInfo.m_responseCallback = null;
		}
		DialogManager.Get().ShowPopup(popupInfo);
	}

	// Token: 0x0600304E RID: 12366 RVA: 0x000F2A48 File Offset: 0x000F0C48
	private void OnDeleteButtonConfirmationResponse(AlertPopup.Response response, object userData)
	{
		if (response == AlertPopup.Response.CANCEL)
		{
			return;
		}
		this.SetEnabled(false);
		CollectionDeckTray.Get().GetDecksContent().DeleteDeck(this.GetDeckID());
	}

	// Token: 0x0600304F RID: 12367 RVA: 0x000F2A7C File Offset: 0x000F0C7C
	private void PlayValidAnimation()
	{
		if ((SceneMgr.Get().GetMode() == SceneMgr.Mode.TOURNAMENT || SceneMgr.Get().GetMode() == SceneMgr.Mode.FRIENDLY) && this.m_isWild && this.m_wildPortraitObject != null)
		{
			if (UniversalInputManager.UsePhoneUI)
			{
				Animator component = this.m_wildPortraitObject.GetComponent<Animator>();
				if (component != null)
				{
					component.enabled = true;
					string text = (!this.m_isValid) ? "WildDeactivate" : "WildActivate";
					component.Play(text);
				}
			}
			else
			{
				Animator component2 = this.m_wildPortraitObject.GetComponent<Animator>();
				if (component2 != null)
				{
					component2.enabled = true;
					string text2 = (!this.m_isValid) ? "Wild_RollupDeactivate" : "Wild_RolldownActivate";
					component2.Play(text2);
				}
			}
		}
		this.UpdateTextColors();
	}

	// Token: 0x06003050 RID: 12368 RVA: 0x000F2B61 File Offset: 0x000F0D61
	private void PlayPopAnimation(string animationName)
	{
		this.PlayPopAnimation(animationName, null, null);
	}

	// Token: 0x06003051 RID: 12369 RVA: 0x000F2B6C File Offset: 0x000F0D6C
	private void PlayPopAnimation(string animationName, CollectionDeckBoxVisual.DelOnAnimationFinished callback, object callbackData)
	{
		base.GetComponent<Animation>().Play(animationName);
		CollectionDeckBoxVisual.OnPopAnimationFinishedCallbackData onPopAnimationFinishedCallbackData = new CollectionDeckBoxVisual.OnPopAnimationFinishedCallbackData
		{
			m_callback = callback,
			m_callbackData = callbackData,
			m_animationName = animationName
		};
		base.StopCoroutine("WaitThenCallAnimationCallback");
		base.StartCoroutine("WaitThenCallAnimationCallback", onPopAnimationFinishedCallbackData);
	}

	// Token: 0x06003052 RID: 12370 RVA: 0x000F2BBC File Offset: 0x000F0DBC
	private IEnumerator WaitThenCallAnimationCallback(CollectionDeckBoxVisual.OnPopAnimationFinishedCallbackData callbackData)
	{
		yield return new WaitForSeconds(base.GetComponent<Animation>()[callbackData.m_animationName].length / base.GetComponent<Animation>()[callbackData.m_animationName].speed);
		bool enableInput = callbackData.m_animationName.Equals("Deck_PopUp");
		this.SetEnabled(enableInput);
		if (callbackData.m_callback == null)
		{
			yield break;
		}
		callbackData.m_callback(callbackData.m_callbackData);
		yield break;
	}

	// Token: 0x06003053 RID: 12371 RVA: 0x000F2BE8 File Offset: 0x000F0DE8
	private void ScaleDeckBox(bool scaleUp, CollectionDeckBoxVisual.DelOnAnimationFinished callback, object callbackData)
	{
		CollectionDeckBoxVisual.OnScaleFinishedCallbackData onScaleFinishedCallbackData = new CollectionDeckBoxVisual.OnScaleFinishedCallbackData
		{
			m_callback = callback,
			m_callbackData = callbackData
		};
		Vector3 vector = (!scaleUp) ? CollectionDeckBoxVisual.SCALED_DOWN_LOCAL_SCALE : this.SCALED_UP_LOCAL_SCALE;
		Hashtable args = iTween.Hash(new object[]
		{
			"scale",
			vector,
			"isLocal",
			true,
			"time",
			0.2f,
			"easetype",
			iTween.EaseType.linear,
			"oncomplete",
			"OnScaleComplete",
			"oncompletetarget",
			base.gameObject,
			"oncompleteparams",
			onScaleFinishedCallbackData,
			"name",
			"scale"
		});
		iTween.StopByName(base.gameObject, "scale");
		iTween.ScaleTo(base.gameObject, args);
	}

	// Token: 0x06003054 RID: 12372 RVA: 0x000F2CD8 File Offset: 0x000F0ED8
	private void OnScaleComplete(CollectionDeckBoxVisual.OnScaleFinishedCallbackData callbackData)
	{
		if (callbackData.m_callback == null)
		{
			return;
		}
		callbackData.m_callback(callbackData.m_callbackData);
	}

	// Token: 0x06003055 RID: 12373 RVA: 0x000F2CF8 File Offset: 0x000F0EF8
	private void OnHeroFullDefLoaded(string cardID, FullDef def, object userData)
	{
		Log.Kyle.Print("OnHeroFullDefLoaded cardID: {0},  m_heroCardID: {1}", new object[]
		{
			cardID,
			this.m_heroCardID
		});
		if (!cardID.Equals(this.m_heroCardID))
		{
			return;
		}
		this.m_fullDef = def;
		this.m_entityDef = def.GetEntityDef();
		this.m_cardDef = def.GetCardDef();
		this.SetPortrait(this.m_cardDef.GetCustomDeckPortrait());
		TAG_CLASS @class = this.m_entityDef.GetClass();
		this.SetClassDisplay(@class);
		this.SetBasicSetProgress(@class);
	}

	// Token: 0x06003056 RID: 12374 RVA: 0x000F2D84 File Offset: 0x000F0F84
	private void UpdatePortraitMaterial(GameObject portraitObject, Material portraitMaterial, int portraitMaterialIndex)
	{
		if (portraitMaterial == null)
		{
			Debug.LogError("Custom Deck Portrait Material is null!");
			return;
		}
		RenderUtils.SetSharedMaterial(portraitObject, portraitMaterialIndex, portraitMaterial);
		if (this.m_cardDef == null)
		{
			return;
		}
		TAG_PREMIUM bestCardPremium = CollectionManager.Get().GetBestCardPremium(this.m_heroCardID);
		if (bestCardPremium != TAG_PREMIUM.GOLDEN && this.m_entityDef.GetCardSet() != TAG_CARD_SET.HERO_SKINS)
		{
			return;
		}
		Material material = portraitObject.GetComponent<Renderer>().materials[portraitMaterialIndex];
		Texture texture = null;
		if (material.HasProperty("_ShadowTex"))
		{
			texture = material.GetTexture("_ShadowTex");
		}
		RenderUtils.SetMaterial(portraitObject, portraitMaterialIndex, this.m_cardDef.GetPremiumPortraitMaterial());
		portraitObject.GetComponent<Renderer>().materials[portraitMaterialIndex].SetTexture("_ShadowTex", texture);
	}

	// Token: 0x06003057 RID: 12375 RVA: 0x000F2E43 File Offset: 0x000F1043
	private void SetPortrait(Material portraitMaterial)
	{
		this.UpdatePortraitMaterial(this.m_portraitObject, portraitMaterial, this.m_portraitMaterialIndex);
		if (this.m_wildPortraitObject != null)
		{
			this.UpdatePortraitMaterial(this.m_wildPortraitObject, portraitMaterial, this.m_wildPortraitMaterialIndex);
		}
	}

	// Token: 0x06003058 RID: 12376 RVA: 0x000F2E7C File Offset: 0x000F107C
	private void SetClassDisplay(TAG_CLASS classTag)
	{
		MeshRenderer component = this.m_classObject.GetComponent<MeshRenderer>();
		if (component != null)
		{
			this.m_classIconMaterial = component.materials[this.m_classIconMaterialIndex];
			this.m_bannerMaterial = component.materials[this.m_classBannerMaterialIndex];
			if (this.m_classIconMaterial == null || this.m_bannerMaterial == null)
			{
				return;
			}
			this.m_classIconMaterial.mainTextureOffset = CollectionPageManager.s_classTextureOffsets[classTag];
			this.m_bannerMaterial.color = CollectionPageManager.s_classColors[classTag];
			if (this.m_topBannerRenderer != null)
			{
				Material material = this.m_topBannerRenderer.materials[this.m_topBannerMaterialIndex];
				material.color = CollectionPageManager.s_classColors[classTag];
			}
		}
		if (this.m_wildClassObject != null)
		{
			component = this.m_wildClassObject.GetComponent<MeshRenderer>();
			if (component != null)
			{
				this.m_classIconMaterial = component.materials[this.m_wildClassIconMaterialIndex];
				this.m_bannerMaterial = component.materials[this.m_wildClassBannerMaterialIndex];
				if (this.m_classIconMaterial == null || this.m_bannerMaterial == null)
				{
					return;
				}
				this.m_classIconMaterial.mainTextureOffset = CollectionPageManager.s_classTextureOffsets[classTag];
				this.m_bannerMaterial.color = CollectionPageManager.s_classColors[classTag];
				if (this.m_wildTopBannerRenderer != null)
				{
					Material material2 = this.m_wildTopBannerRenderer.materials[this.m_topBannerMaterialIndex];
					material2.color = CollectionPageManager.s_classColors[classTag];
				}
			}
		}
	}

	// Token: 0x06003059 RID: 12377 RVA: 0x000F301E File Offset: 0x000F121E
	protected override void OnPress()
	{
		if (!this.m_animateButtonPress || this.m_isLocked)
		{
			return;
		}
		this.OnPressEvent();
	}

	// Token: 0x0600305A RID: 12378 RVA: 0x000F3040 File Offset: 0x000F1240
	protected override void OnRelease()
	{
		if (this.m_isLocked)
		{
			return;
		}
		if (SceneMgr.Get().GetMode() != SceneMgr.Mode.TAVERN_BRAWL || UniversalInputManager.Get().IsTouchMode())
		{
			string text = (!this.m_isWild) ? this.m_standardDeckSelectSound : this.m_wildDeckSelectSound;
			if (!string.IsNullOrEmpty(text))
			{
				string soundName = FileUtils.GameAssetPathToName(text);
				SoundManager.Get().LoadAndPlay(soundName, base.gameObject);
			}
		}
		this.OnReleaseEvent();
	}

	// Token: 0x0600305B RID: 12379 RVA: 0x000F30C0 File Offset: 0x000F12C0
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		if (this.m_tooltipZone != null)
		{
			this.m_tooltipZone.HideTooltip();
		}
		this.OnOutEvent();
	}

	// Token: 0x0600305C RID: 12380 RVA: 0x000F30F0 File Offset: 0x000F12F0
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		if (this.m_tooltipZone != null)
		{
			SceneMgr.Mode mode = SceneMgr.Get().GetMode();
			if (mode == SceneMgr.Mode.COLLECTIONMANAGER && this.m_isLocked)
			{
				this.m_tooltipZone.ShowTooltip(GameStrings.Get("GLUE_HERO_LOCKED_NAME"), GameStrings.Get("GLUE_TOOLTIP_HOW_TO_UNLOCK_DECKS"), 4.5f, true);
			}
			else if (!this.m_isValid)
			{
				if (this.m_isMissingCards)
				{
					this.m_tooltipZone.ShowTooltip(GameStrings.Get("GLUE_INCOMPLETE_DECK_HEADER"), GameStrings.Get("GLUE_INCOMPLETE_DECK_DESC"), 4f, UniversalInputManager.UsePhoneUI);
				}
				else if (this.m_isWild && !Options.Get().GetBool(Option.IN_WILD_MODE))
				{
					this.m_tooltipZone.ShowTooltip(GameStrings.Get("GLUE_DISABLED_WILD_DECK_HEADER"), GameStrings.Get("GLUE_DISABLED_WILD_DECK_DESC"), 4f, UniversalInputManager.UsePhoneUI);
				}
			}
		}
		this.OnOverEvent();
	}

	// Token: 0x0600305D RID: 12381 RVA: 0x000F31F4 File Offset: 0x000F13F4
	public override void SetEnabled(bool enabled)
	{
		base.SetEnabled(enabled);
		if (!enabled && this.m_tooltipZone != null)
		{
			this.m_tooltipZone.HideTooltip();
		}
	}

	// Token: 0x0600305E RID: 12382 RVA: 0x000F3220 File Offset: 0x000F1420
	private void OnPressEvent()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			this.m_pressedBone.transform.localPosition,
			"isLocal",
			true,
			"time",
			0.1,
			"easeType",
			iTween.EaseType.linear
		});
		GameObject target = (!this.m_isWild || !(this.m_wildPortraitObject != null)) ? this.m_portraitObject : this.m_wildPortraitObject;
		iTween.MoveTo(target, args);
	}

	// Token: 0x0600305F RID: 12383 RVA: 0x000F32CC File Offset: 0x000F14CC
	private void OnReleaseEvent()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			this.m_originalButtonPosition,
			"isLocal",
			true,
			"time",
			0.1,
			"easeType",
			iTween.EaseType.linear
		});
		GameObject target = (!this.m_isWild || !(this.m_wildPortraitObject != null)) ? this.m_portraitObject : this.m_wildPortraitObject;
		iTween.MoveTo(target, args);
	}

	// Token: 0x06003060 RID: 12384 RVA: 0x000F336C File Offset: 0x000F156C
	private void OnOutEvent()
	{
		this.SetHighlightState(ActorStateType.HIGHLIGHT_OFF);
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			this.m_originalButtonPosition,
			"isLocal",
			true,
			"time",
			0.1,
			"easeType",
			iTween.EaseType.linear
		});
		GameObject target = (!this.m_isWild || !(this.m_wildPortraitObject != null)) ? this.m_portraitObject : this.m_wildPortraitObject;
		iTween.MoveTo(target, args);
	}

	// Token: 0x06003061 RID: 12385 RVA: 0x000F3414 File Offset: 0x000F1614
	private void OnOverEvent()
	{
		SoundManager.Get().LoadAndPlay("collection_manager_hero_mouse_over", base.gameObject);
		if (this.m_showGlow)
		{
			this.SetHighlightState(ActorStateType.HIGHLIGHT_PRIMARY_MOUSE_OVER);
			CollectionDeck collectionDeck = this.GetCollectionDeck();
			if (collectionDeck != null)
			{
				Log.JMac.Print(string.Format("Sending deck changes for deck {0}, to clear the NEEDS_NAME flag.", this.m_deckID), new object[0]);
				collectionDeck.SendChanges();
				collectionDeck.NeedsName = false;
			}
			this.m_showGlow = false;
		}
		else
		{
			this.SetHighlightState(ActorStateType.HIGHLIGHT_MOUSE_OVER);
		}
	}

	// Token: 0x06003062 RID: 12386 RVA: 0x000F349C File Offset: 0x000F169C
	private void ReparentElements(bool isWild)
	{
		Transform parent = (!isWild) ? this.m_portraitObject.transform : this.m_wildPortraitObject.transform;
		this.m_highlight.transform.parent = parent;
		this.m_deckName.gameObject.transform.parent = parent;
		this.m_setProgressLabel.gameObject.transform.parent = parent;
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_classObject.transform.parent = parent;
		}
		this.m_bones.m_gradientOneLine.parent = parent;
		this.m_bones.m_gradientTwoLine.parent = parent;
	}

	// Token: 0x06003063 RID: 12387 RVA: 0x000F354C File Offset: 0x000F174C
	private void UpdateTextColors()
	{
		if (Localization.GetLocale() == Locale.thTH)
		{
			this.m_deckName.TextColor = ((!this.m_isValid) ? CollectionDeckBoxVisual.DECK_NAME_DISABLED_COLOR_THAI : CollectionDeckBoxVisual.DECK_NAME_ENABLED_COLOR);
		}
		else
		{
			this.m_deckName.TextColor = ((!this.m_isValid) ? CollectionDeckBoxVisual.DECK_NAME_DISABLED_COLOR : CollectionDeckBoxVisual.DECK_NAME_ENABLED_COLOR);
		}
		this.m_setProgressLabel.TextColor = ((!this.m_isValid) ? CollectionDeckBoxVisual.BASIC_SET_PROGRESS_DISABLED_COLOR : CollectionDeckBoxVisual.BASIC_SET_PROGRESS_ENABLED_COLOR);
	}

	// Token: 0x04001DE0 RID: 7648
	public const float DECKBOX_SCALE = 0.95f;

	// Token: 0x04001DE1 RID: 7649
	private const float BUTTON_POP_SPEED = 6f;

	// Token: 0x04001DE2 RID: 7650
	private const string DECKBOX_POPUP_ANIM_NAME = "Deck_PopUp";

	// Token: 0x04001DE3 RID: 7651
	private const string DECKBOX_POPDOWN_ANIM_NAME = "Deck_PopDown";

	// Token: 0x04001DE4 RID: 7652
	private const float SCALE_TIME = 0.2f;

	// Token: 0x04001DE5 RID: 7653
	private const float ADJUST_Y_OFFSET_ANIM_TIME = 0.05f;

	// Token: 0x04001DE6 RID: 7654
	private const float SCALED_UP_LOCAL_Y_OFFSET = 3.238702f;

	// Token: 0x04001DE7 RID: 7655
	private const float SCALED_DOWN_LOCAL_Y_OFFSET = 1.273138f;

	// Token: 0x04001DE8 RID: 7656
	public UberText m_deckName;

	// Token: 0x04001DE9 RID: 7657
	public UberText m_setProgressLabel;

	// Token: 0x04001DEA RID: 7658
	public GameObject m_labelGradient;

	// Token: 0x04001DEB RID: 7659
	public PegUIElement m_deleteButton;

	// Token: 0x04001DEC RID: 7660
	public GameObject m_highlight;

	// Token: 0x04001DED RID: 7661
	public Texture2D m_standardHighlight;

	// Token: 0x04001DEE RID: 7662
	public Texture2D m_wildHighlight;

	// Token: 0x04001DEF RID: 7663
	public GameObject m_invalidDeckX;

	// Token: 0x04001DF0 RID: 7664
	public GameObject m_portraitObject;

	// Token: 0x04001DF1 RID: 7665
	public int m_portraitMaterialIndex;

	// Token: 0x04001DF2 RID: 7666
	public GameObject m_wildPortraitObject;

	// Token: 0x04001DF3 RID: 7667
	public int m_wildPortraitMaterialIndex;

	// Token: 0x04001DF4 RID: 7668
	public GameObject m_classObject;

	// Token: 0x04001DF5 RID: 7669
	public int m_classIconMaterialIndex;

	// Token: 0x04001DF6 RID: 7670
	public int m_classBannerMaterialIndex;

	// Token: 0x04001DF7 RID: 7671
	public GameObject m_wildClassObject;

	// Token: 0x04001DF8 RID: 7672
	public int m_wildClassIconMaterialIndex;

	// Token: 0x04001DF9 RID: 7673
	public int m_wildClassBannerMaterialIndex;

	// Token: 0x04001DFA RID: 7674
	public MeshRenderer m_topBannerRenderer;

	// Token: 0x04001DFB RID: 7675
	public MeshRenderer m_wildTopBannerRenderer;

	// Token: 0x04001DFC RID: 7676
	public int m_topBannerMaterialIndex;

	// Token: 0x04001DFD RID: 7677
	public Material m_xButtonMaterial;

	// Token: 0x04001DFE RID: 7678
	public Material m_xButtonWildMaterial;

	// Token: 0x04001DFF RID: 7679
	public GameObject m_pressedBone;

	// Token: 0x04001E00 RID: 7680
	public CustomDeckBones m_bones;

	// Token: 0x04001E01 RID: 7681
	public GameObject m_normalDeckVisuals;

	// Token: 0x04001E02 RID: 7682
	public GameObject m_lockedDeckVisuals;

	// Token: 0x04001E03 RID: 7683
	public TooltipZone m_tooltipZone;

	// Token: 0x04001E04 RID: 7684
	public GameObject m_renameVisuals;

	// Token: 0x04001E05 RID: 7685
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_standardDeckSelectSound;

	// Token: 0x04001E06 RID: 7686
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_wildDeckSelectSound;

	// Token: 0x04001E07 RID: 7687
	public static readonly float POPPED_UP_LOCAL_Z = 0f;

	// Token: 0x04001E08 RID: 7688
	public static readonly Vector3 POPPED_DOWN_LOCAL_POS = new Vector3(0f, -0.8598533f, 0f);

	// Token: 0x04001E09 RID: 7689
	private Vector3 SCALED_UP_LOCAL_SCALE = new Vector3(1.126f, 1.126f, 1.126f);

	// Token: 0x04001E0A RID: 7690
	private Vector3 SCALED_UP_DECK_OFFSET = new Vector3(0f, 0f, 0f);

	// Token: 0x04001E0B RID: 7691
	private static readonly Vector3 SCALED_DOWN_LOCAL_SCALE = new Vector3(0.95f, 0.95f, 0.95f);

	// Token: 0x04001E0C RID: 7692
	private static readonly Color BASIC_SET_PROGRESS_ENABLED_COLOR = new Color(0.97f, 0.82f, 0.22f);

	// Token: 0x04001E0D RID: 7693
	private static readonly Color BASIC_SET_PROGRESS_DISABLED_COLOR = new Color(0.36f, 0.31f, 0.08f);

	// Token: 0x04001E0E RID: 7694
	private static readonly Color DECK_NAME_ENABLED_COLOR = Color.white;

	// Token: 0x04001E0F RID: 7695
	private static readonly Color DECK_NAME_DISABLED_COLOR = new Color(0.3f, 0.3f, 0.3f, 1f);

	// Token: 0x04001E10 RID: 7696
	private static readonly Color DECK_NAME_DISABLED_COLOR_THAI = new Color(0.7f, 0.7f, 0.7f, 1f);

	// Token: 0x04001E11 RID: 7697
	private Material m_classIconMaterial;

	// Token: 0x04001E12 RID: 7698
	private Material m_bannerMaterial;

	// Token: 0x04001E13 RID: 7699
	private long m_deckID = -1L;

	// Token: 0x04001E14 RID: 7700
	private bool m_isPoppedUp;

	// Token: 0x04001E15 RID: 7701
	private bool m_isShown;

	// Token: 0x04001E16 RID: 7702
	private FullDef m_fullDef;

	// Token: 0x04001E17 RID: 7703
	private EntityDef m_entityDef;

	// Token: 0x04001E18 RID: 7704
	private CardDef m_cardDef;

	// Token: 0x04001E19 RID: 7705
	private bool m_isValid = true;

	// Token: 0x04001E1A RID: 7706
	private bool m_isMissingCards;

	// Token: 0x04001E1B RID: 7707
	private HighlightState m_highlightState;

	// Token: 0x04001E1C RID: 7708
	private string m_heroCardID = string.Empty;

	// Token: 0x04001E1D RID: 7709
	private bool m_isWild = true;

	// Token: 0x04001E1E RID: 7710
	private Vector3 m_originalButtonPosition;

	// Token: 0x04001E1F RID: 7711
	private bool m_animateButtonPress = true;

	// Token: 0x04001E20 RID: 7712
	private bool m_wasTouchModeEnabled;

	// Token: 0x04001E21 RID: 7713
	private int m_positionIndex;

	// Token: 0x04001E22 RID: 7714
	private bool m_showGlow;

	// Token: 0x04001E23 RID: 7715
	private bool m_isLocked;

	// Token: 0x04001E24 RID: 7716
	private Transform m_customDeckTransform;

	// Token: 0x0200074B RID: 1867
	private class OnPopAnimationFinishedCallbackData
	{
		// Token: 0x0400324A RID: 12874
		public string m_animationName;

		// Token: 0x0400324B RID: 12875
		public CollectionDeckBoxVisual.DelOnAnimationFinished m_callback;

		// Token: 0x0400324C RID: 12876
		public object m_callbackData;
	}

	// Token: 0x0200074C RID: 1868
	// (Invoke) Token: 0x06004B6F RID: 19311
	public delegate void DelOnAnimationFinished(object callbackData);

	// Token: 0x0200074D RID: 1869
	private class OnScaleFinishedCallbackData
	{
		// Token: 0x0400324D RID: 12877
		public CollectionDeckBoxVisual.DelOnAnimationFinished m_callback;

		// Token: 0x0400324E RID: 12878
		public object m_callbackData;
	}
}
