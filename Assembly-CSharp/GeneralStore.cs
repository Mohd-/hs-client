using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000412 RID: 1042
[CustomEditClass]
public class GeneralStore : Store
{
	// Token: 0x0600354C RID: 13644 RVA: 0x0010911C File Offset: 0x0010731C
	protected override void Start()
	{
		base.Start();
		StoreManager.Get().RegisterStoreAchievesListener(new StoreManager.StoreAchievesCallback(this.SuccessfulPurchaseEvent));
		StoreManager.Get().RegisterSuccessfulPurchaseAckListener(new StoreManager.SuccessfulPurchaseAckCallback(this.SuccessfulPurchaseAckEvent));
		SoundManager.Get().Load("gold_spend_plate_flip_on");
		SoundManager.Get().Load("gold_spend_plate_flip_off");
		this.UpdateModeButtons(this.m_currentMode);
		foreach (GeneralStore.ModeObjects modeObjects in this.m_modeObjects)
		{
			if (modeObjects.m_content != null)
			{
				modeObjects.m_content.gameObject.SetActive(modeObjects.m_mode == this.m_currentMode);
			}
		}
		if (this.m_offClicker != null)
		{
			this.m_offClicker.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClosePressed));
		}
	}

	// Token: 0x0600354D RID: 13645 RVA: 0x00109228 File Offset: 0x00107428
	protected override void Awake()
	{
		GeneralStore.s_instance = this;
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_scaleMode = CanvasScaleMode.WIDTH;
		}
		base.Awake();
		if (this.m_shakeyObject != null)
		{
			this.m_shakeyObjectOriginalLocalRotation = this.m_shakeyObject.transform.localEulerAngles;
		}
		this.m_buyWithMoneyButton.SetText(GameStrings.Get("GLUE_STORE_BUY_TEXT"));
		this.m_buyWithGoldButton.SetText(GameStrings.Get("GLUE_STORE_BUY_TEXT"));
		foreach (GeneralStore.ModeObjects modeObjects in this.m_modeObjects)
		{
			GeneralStoreContent content = modeObjects.m_content;
			UIBButton button = modeObjects.m_button;
			GeneralStoreMode mode = modeObjects.m_mode;
			GeneralStorePane pane = modeObjects.m_pane;
			if (content != null)
			{
				content.SetParentStore(this);
				content.RegisterCurrentBundleChanged(delegate(NoGTAPPTransactionData goldBundle, Network.Bundle moneyBundle)
				{
					this.UpdateCostAndButtonState(goldBundle, moneyBundle);
				});
			}
			if (button != null)
			{
				button.AddEventListener(UIEventType.PRESS, delegate(UIEvent e)
				{
					this.SetMode(mode);
				});
			}
			if (pane != null)
			{
				pane.transform.localPosition = this.m_paneSwapOutOffset;
				this.m_paneStartPositions[mode] = pane.m_paneContainer.transform.localPosition;
			}
		}
		if (this.m_defaultPane != null)
		{
			this.m_defaultPane.transform.localPosition = this.m_paneSwapOutOffset;
		}
	}

	// Token: 0x0600354E RID: 13646 RVA: 0x001093D0 File Offset: 0x001075D0
	protected override void OnDestroy()
	{
		base.OnDestroy();
		StoreManager.Get().RemoveSuccessfulPurchaseAckListener(new StoreManager.SuccessfulPurchaseAckCallback(this.SuccessfulPurchaseAckEvent));
		StoreManager.Get().RemoveStoreAchievesListener(new StoreManager.StoreAchievesCallback(this.SuccessfulPurchaseEvent));
		this.m_mainPanel = null;
		GeneralStore.s_instance = null;
	}

	// Token: 0x0600354F RID: 13647 RVA: 0x0010941E File Offset: 0x0010761E
	public GeneralStoreContent GetCurrentContent()
	{
		return this.GetContent(this.m_currentMode);
	}

	// Token: 0x06003550 RID: 13648 RVA: 0x0010942C File Offset: 0x0010762C
	public GeneralStorePane GetCurrentPane()
	{
		return this.GetPane(this.m_currentMode);
	}

	// Token: 0x06003551 RID: 13649 RVA: 0x0010943C File Offset: 0x0010763C
	public GeneralStoreContent GetContent(GeneralStoreMode mode)
	{
		GeneralStore.ModeObjects modeObjects = this.m_modeObjects.Find((GeneralStore.ModeObjects obj) => obj.m_mode == mode);
		return (modeObjects == null) ? null : modeObjects.m_content;
	}

	// Token: 0x06003552 RID: 13650 RVA: 0x00109480 File Offset: 0x00107680
	public GeneralStorePane GetPane(GeneralStoreMode mode)
	{
		GeneralStore.ModeObjects modeObjects = this.m_modeObjects.Find((GeneralStore.ModeObjects obj) => obj.m_mode == mode);
		if (modeObjects != null && modeObjects.m_pane != null)
		{
			return modeObjects.m_pane;
		}
		return this.m_defaultPane;
	}

	// Token: 0x06003553 RID: 13651 RVA: 0x001094D8 File Offset: 0x001076D8
	public void Close(bool closeWithAnimation)
	{
		if (!this.m_shown)
		{
			return;
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			Navigation.PopUnique(new Navigation.NavigateBackHandler(GeneralStorePhoneCover.OnNavigateBack));
		}
		Navigation.Pop();
		this.CloseImpl(closeWithAnimation);
	}

	// Token: 0x06003554 RID: 13652 RVA: 0x0010951D File Offset: 0x0010771D
	public override void Close()
	{
		if (!this.m_shown)
		{
			return;
		}
		if (this.m_settingNewModeCount == 0)
		{
			Navigation.GoBack();
		}
	}

	// Token: 0x06003555 RID: 13653 RVA: 0x0010953C File Offset: 0x0010773C
	public void SetMode(GeneralStoreMode mode)
	{
		base.StartCoroutine(this.AnimateAndUpdateStoreMode(this.m_currentMode, mode));
	}

	// Token: 0x06003556 RID: 13654 RVA: 0x00109552 File Offset: 0x00107752
	public GeneralStoreMode GetMode()
	{
		return this.m_currentMode;
	}

	// Token: 0x06003557 RID: 13655 RVA: 0x0010955C File Offset: 0x0010775C
	public void ShakeStore(float xRotationAmount, float shakeTime, float delay = 0f)
	{
		if (this.m_shakeyObject == null)
		{
			return;
		}
		base.StartCoroutine(this.AnimateShakeyObjectCoroutine(xRotationAmount, shakeTime, delay));
	}

	// Token: 0x06003558 RID: 13656 RVA: 0x0010958C File Offset: 0x0010778C
	public void SetDescription(string title, string desc, string warning = null)
	{
		this.HideChooseDescription();
		if (this.m_productDetailsContainer != null)
		{
			this.m_productDetailsContainer.gameObject.SetActive(true);
		}
		bool flag = StoreManager.Get().IsKoreanCustomer();
		bool flag2 = !string.IsNullOrEmpty(title);
		this.m_productDetailsHeadlineText.gameObject.SetActive(flag2);
		this.m_productDetailsText.gameObject.SetActive(!flag);
		this.m_koreanProductDetailsText.gameObject.SetActive(flag);
		this.m_koreanWarningText.gameObject.SetActive(flag);
		this.m_productDetailsText.Height = ((!flag2) ? this.m_productDetailsExtendedHeight : this.m_productDetailsRegularHeight);
		this.m_productDetailsHeadlineText.Text = title;
		this.m_koreanProductDetailsText.Text = desc;
		this.m_productDetailsText.Text = desc;
		this.m_koreanProductDetailsText.Height = ((!flag2) ? this.m_koreanProductDetailsExtendedHeight : this.m_koreanProductDetailsRegularHeight);
		this.m_koreanWarningText.Text = ((warning != null) ? warning : string.Empty);
		if (this.m_productDetailsContainer != null)
		{
			this.m_productDetailsContainer.UpdateSlices();
		}
	}

	// Token: 0x06003559 RID: 13657 RVA: 0x001096C0 File Offset: 0x001078C0
	public void HideDescription()
	{
		if (this.m_productDetailsContainer != null)
		{
			this.m_productDetailsContainer.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600355A RID: 13658 RVA: 0x001096F0 File Offset: 0x001078F0
	public void SetChooseDescription(string chooseText)
	{
		this.HideDescription();
		this.SetAccentTexture(null);
		if (this.m_chooseArrowContainer != null)
		{
			this.m_chooseArrowContainer.SetActive(true);
		}
		if (this.m_chooseArrowText != null)
		{
			this.m_chooseArrowText.Text = chooseText;
		}
	}

	// Token: 0x0600355B RID: 13659 RVA: 0x00109744 File Offset: 0x00107944
	public void HideChooseDescription()
	{
		if (this.m_chooseArrowContainer != null)
		{
			this.m_chooseArrowContainer.SetActive(false);
		}
	}

	// Token: 0x0600355C RID: 13660 RVA: 0x00109764 File Offset: 0x00107964
	public void SetAccentTexture(Texture texture)
	{
		if (this.m_accentIcon != null)
		{
			bool flag = texture != null;
			this.m_accentIcon.gameObject.SetActive(flag);
			if (flag)
			{
				this.m_accentIcon.material.mainTexture = texture;
			}
		}
	}

	// Token: 0x0600355D RID: 13661 RVA: 0x001097B4 File Offset: 0x001079B4
	public void HideAccentTexture()
	{
		if (this.m_accentIcon != null)
		{
			this.m_accentIcon.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600355E RID: 13662 RVA: 0x001097E3 File Offset: 0x001079E3
	public void ResumePreviousMusicPlaylist()
	{
		if (this.m_prevPlaylist != MusicPlaylistType.Invalid)
		{
			MusicManager.Get().StartPlaylist(this.m_prevPlaylist);
		}
	}

	// Token: 0x0600355F RID: 13663 RVA: 0x00109801 File Offset: 0x00107A01
	public void RegisterModeChangedListener(GeneralStore.ModeChanged dlg)
	{
		this.m_modeChangedListeners.Add(dlg);
	}

	// Token: 0x06003560 RID: 13664 RVA: 0x0010980F File Offset: 0x00107A0F
	public void UnregisterModeChangedListener(GeneralStore.ModeChanged dlg)
	{
		this.m_modeChangedListeners.Remove(dlg);
	}

	// Token: 0x06003561 RID: 13665 RVA: 0x0010981E File Offset: 0x00107A1E
	public static GeneralStore Get()
	{
		return GeneralStore.s_instance;
	}

	// Token: 0x06003562 RID: 13666 RVA: 0x00109825 File Offset: 0x00107A25
	public override bool IsReady()
	{
		return true;
	}

	// Token: 0x06003563 RID: 13667 RVA: 0x00109828 File Offset: 0x00107A28
	public override void OnMoneySpent()
	{
		NetCache.Get().RefreshNetObject<NetCache.NetCacheBoosters>();
		GeneralStoreContent currentContent = this.GetCurrentContent();
		if (currentContent != null)
		{
			currentContent.Refresh();
		}
		GeneralStorePane currentPane = this.GetCurrentPane();
		if (currentPane != null)
		{
			currentPane.Refresh();
		}
	}

	// Token: 0x06003564 RID: 13668 RVA: 0x00109871 File Offset: 0x00107A71
	public override void OnGoldSpent()
	{
		NetCache.Get().RefreshNetObject<NetCache.NetCacheBoosters>();
	}

	// Token: 0x06003565 RID: 13669 RVA: 0x0010987D File Offset: 0x00107A7D
	public override void OnGoldBalanceChanged(NetCache.NetCacheGoldBalance balance)
	{
		this.UpdateGoldButtonState(balance);
	}

	// Token: 0x06003566 RID: 13670 RVA: 0x00109888 File Offset: 0x00107A88
	protected override void ShowImpl(Store.DelOnStoreShown onStoreShownCB, bool isTotallyFake)
	{
		if (this.m_shown)
		{
			return;
		}
		this.m_prevPlaylist = MusicManager.Get().GetCurrentPlaylist();
		foreach (GeneralStore.ModeObjects modeObjects in this.m_modeObjects)
		{
			GeneralStoreContent content = modeObjects.m_content;
			GeneralStorePane pane = modeObjects.m_pane;
			if (content != null)
			{
				content.StoreShown(this.GetCurrentContent() == content);
			}
			if (pane != null)
			{
				pane.StoreShown(this.GetCurrentPane() == pane);
			}
		}
		ShownUIMgr.Get().SetShownUI(ShownUIMgr.UI_WINDOW.GENERAL_STORE);
		FriendChallengeMgr.Get().OnStoreOpened();
		this.PreRender();
		PresenceMgr.Get().SetStatus(new Enum[]
		{
			PresenceStatus.STORE
		});
		if (!UniversalInputManager.UsePhoneUI && !Options.Get().GetBool(Option.HAS_SEEN_GOLD_QTY_INSTRUCTION, false) && UserAttentionManager.CanShowAttentionGrabber("GeneralStore.Show:" + Option.HAS_SEEN_GOLD_QTY_INSTRUCTION))
		{
			NetCache.NetCacheGoldBalance netObject = NetCache.Get().GetNetObject<NetCache.NetCacheGoldBalance>();
			if (netObject.GetTotal() >= (long)GeneralStore.MIN_GOLD_FOR_CHANGE_QTY_TOOLTIP)
			{
				AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
				popupInfo.m_attentionCategory = UserAttentionBlocker.NONE;
				popupInfo.m_headerText = GameStrings.Get("GLUE_STORE_GOLD_QTY_CHANGE_HEADER");
				if (UniversalInputManager.Get().IsTouchMode())
				{
					popupInfo.m_text = GameStrings.Get("GLUE_STORE_GOLD_QTY_CHANGE_DESC_TOUCH");
				}
				else
				{
					popupInfo.m_text = GameStrings.Get("GLUE_STORE_GOLD_QTY_CHANGE_DESC");
				}
				popupInfo.m_showAlertIcon = false;
				popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
				DialogManager.Get().ShowPopup(popupInfo);
				Options.Get().SetBool(Option.HAS_SEEN_GOLD_QTY_INSTRUCTION, true);
			}
		}
		this.UpdateGoldButtonState();
		this.m_shown = true;
		Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
		base.EnableFullScreenEffects(true);
		SoundManager.Get().LoadAndPlay("Store_window_expand", base.gameObject);
		base.DoShowAnimation(delegate()
		{
			if (onStoreShownCB != null)
			{
				onStoreShownCB();
			}
		});
	}

	// Token: 0x06003567 RID: 13671 RVA: 0x00109AAC File Offset: 0x00107CAC
	private void OnClosePressed(UIEvent e)
	{
		if (!this.m_shown)
		{
			return;
		}
		this.Close();
	}

	// Token: 0x06003568 RID: 13672 RVA: 0x00109AC0 File Offset: 0x00107CC0
	protected override void BuyWithMoney(UIEvent e)
	{
		GeneralStoreContent currentContent = this.GetCurrentContent();
		Network.Bundle bundle = currentContent.GetCurrentMoneyBundle();
		if (bundle == null)
		{
			Debug.LogWarning("GeneralStore.OnBuyWithMoneyPressed(): SelectedBundle is null");
			return;
		}
		GeneralStoreContent.BuyEvent successBuyCB = delegate()
		{
			this.FireBuyWithMoneyEvent(bundle.ProductID, 1);
		};
		currentContent.TryBuyWithMoney(bundle, successBuyCB, null);
	}

	// Token: 0x06003569 RID: 13673 RVA: 0x00109B20 File Offset: 0x00107D20
	protected override void BuyWithGold(UIEvent e)
	{
		GeneralStoreContent currentContent = this.GetCurrentContent();
		NoGTAPPTransactionData bundle = currentContent.GetCurrentGoldBundle();
		if (bundle == null)
		{
			Debug.LogWarning("GeneralStore.OnBuyWithGoldPressed(): SelectedGoldPrice is null");
			return;
		}
		GeneralStoreContent.BuyEvent buyEvent = delegate()
		{
			this.FireBuyWithGoldEventNoGTAPP(bundle);
		};
		currentContent.TryBuyWithGold(buyEvent, buyEvent);
	}

	// Token: 0x0600356A RID: 13674 RVA: 0x00109B78 File Offset: 0x00107D78
	private void UpdateMoneyButtonState()
	{
		Store.BuyButtonState moneyButtonState = Store.BuyButtonState.ENABLED;
		if (!StoreManager.Get().IsOpen())
		{
			moneyButtonState = Store.BuyButtonState.DISABLED;
		}
		else if (!StoreManager.Get().IsBattlePayFeatureEnabled())
		{
			moneyButtonState = Store.BuyButtonState.DISABLED_FEATURE;
		}
		else
		{
			Network.Bundle currentMoneyBundle = this.GetCurrentContent().GetCurrentMoneyBundle();
			if (currentMoneyBundle == null || StoreManager.Get().IsProductAlreadyOwned(currentMoneyBundle))
			{
				moneyButtonState = Store.BuyButtonState.DISABLED_OWNED;
			}
		}
		base.SetMoneyButtonState(moneyButtonState);
	}

	// Token: 0x0600356B RID: 13675 RVA: 0x00109BE0 File Offset: 0x00107DE0
	private void UpdateGoldButtonState(NetCache.NetCacheGoldBalance balance)
	{
		Store.BuyButtonState goldButtonState = Store.BuyButtonState.ENABLED;
		GeneralStoreContent currentContent = this.GetCurrentContent();
		if (currentContent == null)
		{
			return;
		}
		NoGTAPPTransactionData currentGoldBundle = currentContent.GetCurrentGoldBundle();
		long num;
		if (currentGoldBundle == null)
		{
			goldButtonState = Store.BuyButtonState.DISABLED;
		}
		else if (!StoreManager.Get().IsOpen())
		{
			goldButtonState = Store.BuyButtonState.DISABLED;
		}
		else if (!StoreManager.Get().IsBuyWithGoldFeatureEnabled())
		{
			goldButtonState = Store.BuyButtonState.DISABLED_FEATURE;
		}
		else if (balance == null)
		{
			goldButtonState = Store.BuyButtonState.DISABLED;
		}
		else if (!StoreManager.Get().GetGoldCostNoGTAPP(currentGoldBundle, out num))
		{
			goldButtonState = Store.BuyButtonState.DISABLED_NO_TOOLTIP;
		}
		else if (balance.GetTotal() < num)
		{
			goldButtonState = Store.BuyButtonState.DISABLED_NOT_ENOUGH_GOLD;
		}
		base.SetGoldButtonState(goldButtonState);
	}

	// Token: 0x0600356C RID: 13676 RVA: 0x00109C80 File Offset: 0x00107E80
	private void UpdateGoldButtonState()
	{
		NetCache.NetCacheGoldBalance netObject = NetCache.Get().GetNetObject<NetCache.NetCacheGoldBalance>();
		this.UpdateGoldButtonState(netObject);
	}

	// Token: 0x0600356D RID: 13677 RVA: 0x00109CA0 File Offset: 0x00107EA0
	private void UpdateCostDisplay(NoGTAPPTransactionData goldBundle)
	{
		long num;
		if (goldBundle == null || !StoreManager.Get().GetGoldCostNoGTAPP(goldBundle, out num))
		{
			this.UpdateCostDisplay(GeneralStore.BuyPanelState.BUY_GOLD, string.Empty);
			return;
		}
		this.UpdateCostDisplay(GeneralStore.BuyPanelState.BUY_GOLD, num.ToString());
	}

	// Token: 0x0600356E RID: 13678 RVA: 0x00109CE0 File Offset: 0x00107EE0
	private void UpdateCostDisplay(Network.Bundle moneyBundle)
	{
		if (moneyBundle == null)
		{
			this.UpdateCostDisplay(GeneralStore.BuyPanelState.BUY_MONEY, GameStrings.Get("GLUE_STORE_DUNGEON_BUTTON_COST_OWNED_TEXT"));
		}
		else
		{
			this.UpdateCostDisplay(GeneralStore.BuyPanelState.BUY_MONEY, StoreManager.Get().FormatCostBundle(moneyBundle));
		}
	}

	// Token: 0x0600356F RID: 13679 RVA: 0x00109D1C File Offset: 0x00107F1C
	private void UpdateCostDisplay(GeneralStore.BuyPanelState newPanelState, string costText = "")
	{
		if (newPanelState == GeneralStore.BuyPanelState.BUY_MONEY)
		{
			this.m_moneyCostText.Text = costText;
			this.m_moneyCostText.UpdateNow();
		}
		else if (newPanelState == GeneralStore.BuyPanelState.BUY_GOLD)
		{
			this.m_goldCostText.Text = costText;
			this.m_goldCostText.UpdateNow();
		}
		this.ShowBuyPanel(newPanelState);
	}

	// Token: 0x06003570 RID: 13680 RVA: 0x00109D74 File Offset: 0x00107F74
	private void ShowBuyPanel(GeneralStore.BuyPanelState setPanelState)
	{
		if (this.m_buyPanelState == setPanelState)
		{
			return;
		}
		GameObject buyPanelObject = this.GetBuyPanelObject(setPanelState);
		GameObject oldPanelObject = this.GetBuyPanelObject(this.m_buyPanelState);
		this.m_buyPanelState = setPanelState;
		iTween.StopByName(buyPanelObject, "rotation");
		iTween.StopByName(oldPanelObject, "rotation");
		buyPanelObject.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
		oldPanelObject.transform.localEulerAngles = Vector3.zero;
		buyPanelObject.SetActive(true);
		iTween.RotateTo(oldPanelObject, iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, 0f, 180f),
			"isLocal",
			true,
			"time",
			GeneralStore.FLIP_BUY_PANEL_ANIM_TIME,
			"easeType",
			iTween.EaseType.linear,
			"oncomplete",
			delegate(object o)
			{
				oldPanelObject.SetActive(false);
			},
			"name",
			"rotation"
		}));
		iTween.RotateTo(buyPanelObject, iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, 0f, 0f),
			"isLocal",
			true,
			"time",
			GeneralStore.FLIP_BUY_PANEL_ANIM_TIME,
			"easeType",
			iTween.EaseType.linear,
			"name",
			"rotation"
		}));
		SoundManager.Get().LoadAndPlay((setPanelState != GeneralStore.BuyPanelState.BUY_GOLD) ? "gold_spend_plate_flip_off" : "gold_spend_plate_flip_on");
	}

	// Token: 0x06003571 RID: 13681 RVA: 0x00109F48 File Offset: 0x00108148
	private GameObject GetBuyPanelObject(GeneralStore.BuyPanelState buyPanelState)
	{
		if (buyPanelState == GeneralStore.BuyPanelState.BUY_GOLD)
		{
			return this.m_buyGoldPanel;
		}
		if (buyPanelState != GeneralStore.BuyPanelState.BUY_MONEY)
		{
			return this.m_buyEmptyPanel;
		}
		return this.m_buyMoneyPanel;
	}

	// Token: 0x06003572 RID: 13682 RVA: 0x00109F80 File Offset: 0x00108180
	public void RefreshContent()
	{
		GeneralStoreContent currentContent = this.GetCurrentContent();
		GeneralStorePane currentPane = this.GetCurrentPane();
		StoreManager storeManager = StoreManager.Get();
		base.ActivateCover(storeManager.TransactionInProgress() || storeManager.IsPromptShowing());
		if (currentContent != null)
		{
			currentContent.Refresh();
		}
		if (currentPane != null)
		{
			currentPane.Refresh();
		}
	}

	// Token: 0x06003573 RID: 13683 RVA: 0x00109FE0 File Offset: 0x001081E0
	protected override void Hide(bool animate)
	{
		if (this.m_settingNewModeCount > 0)
		{
			return;
		}
		if (ShownUIMgr.Get() != null)
		{
			ShownUIMgr.Get().ClearShownUI();
		}
		FriendChallengeMgr.Get().OnStoreClosed();
		this.ResumePreviousMusicPlaylist();
		base.DoHideAnimation(!animate, new UIBPopup.OnAnimationComplete(this.OnHidden));
	}

	// Token: 0x06003574 RID: 13684 RVA: 0x0010A03C File Offset: 0x0010823C
	protected override void OnHidden()
	{
		this.m_shown = false;
		foreach (GeneralStore.ModeObjects modeObjects in this.m_modeObjects)
		{
			GeneralStorePane pane = modeObjects.m_pane;
			GeneralStoreContent content = modeObjects.m_content;
			if (pane != null)
			{
				pane.StoreHidden(this.GetCurrentPane() == pane);
			}
			if (content != null)
			{
				content.StoreHidden(this.GetCurrentContent() == content);
			}
		}
	}

	// Token: 0x06003575 RID: 13685 RVA: 0x0010A0E0 File Offset: 0x001082E0
	private void PreRender()
	{
		if (!this.m_staticTextResized)
		{
			this.m_buyWithMoneyButton.m_ButtonText.UpdateNow();
			this.m_buyWithGoldButton.m_ButtonText.UpdateNow();
			this.m_staticTextResized = true;
		}
		this.RefreshContent();
	}

	// Token: 0x06003576 RID: 13686 RVA: 0x0010A128 File Offset: 0x00108328
	private bool IsContentFlipClockwise(GeneralStoreMode oldMode, GeneralStoreMode newMode)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < GeneralStore.s_ContentOrdering.Length; i++)
		{
			if (GeneralStore.s_ContentOrdering[i] == oldMode)
			{
				num = i;
			}
			else if (GeneralStore.s_ContentOrdering[i] == newMode)
			{
				num2 = i;
			}
		}
		return num < num2;
	}

	// Token: 0x06003577 RID: 13687 RVA: 0x0010A178 File Offset: 0x00108378
	private IEnumerator AnimateAndUpdateStoreMode(GeneralStoreMode oldMode, GeneralStoreMode newMode)
	{
		while (this.m_settingNewModeCount > 0)
		{
			yield return null;
		}
		this.FireModeChangedEvent(oldMode, newMode);
		if (this.m_currentMode == newMode)
		{
			yield break;
		}
		this.m_settingNewModeCount++;
		if (this.m_modeButtonBlocker != null)
		{
			this.m_modeButtonBlocker.SetActive(true);
		}
		this.UpdateModeButtons(newMode);
		this.m_currentMode = newMode;
		base.StartCoroutine(this.AnimateAndUpdateStorePane(oldMode, newMode));
		GeneralStoreContent prevContent = this.GetContent(oldMode);
		GeneralStoreContent nextContent = this.GetContent(newMode);
		if (prevContent != null)
		{
			prevContent.SetContentActive(false);
			prevContent.PreStoreFlipOut();
			while (!prevContent.AnimateExitStart())
			{
				yield return null;
			}
			while (!prevContent.AnimateExitEnd())
			{
				yield return null;
			}
		}
		bool clockwise = this.IsContentFlipClockwise(oldMode, newMode);
		Vector3 contentPosition;
		Vector3 contentRotation;
		Vector3 lastPanelRotation;
		Vector3 newPanelRotation;
		this.GetContentPositionIndex(clockwise, out contentPosition, out contentRotation, out lastPanelRotation, out newPanelRotation);
		if (nextContent != null)
		{
			nextContent.transform.localPosition = contentPosition;
			nextContent.transform.localEulerAngles = contentRotation;
			nextContent.gameObject.SetActive(true);
		}
		iTween.StopByName(this.m_mainPanel, "PANEL_ROTATION");
		this.m_mainPanel.transform.localEulerAngles = lastPanelRotation;
		bool rotationDone = false;
		float flipAnimTime = this.m_contentFlipAnimationTime;
		float direction = (!clockwise) ? -1f : 1f;
		this.ShakeStore(10f * direction, 1.5f, flipAnimTime * 0.3f);
		if (!string.IsNullOrEmpty(this.m_contentFlipSound))
		{
			SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_contentFlipSound));
		}
		Action<object> completeAnimation = delegate(object o)
		{
			this.m_mainPanel.transform.localEulerAngles = newPanelRotation;
			rotationDone = true;
			if (prevContent != null)
			{
				prevContent.gameObject.SetActive(false);
			}
		};
		if (flipAnimTime > 0f)
		{
			iTween.RotateBy(this.m_mainPanel, iTween.Hash(new object[]
			{
				"name",
				"PANEL_ROTATION",
				"amount",
				GeneralStore.MAIN_PANEL_ANGLE_TO_ROTATE * direction,
				"time",
				flipAnimTime,
				"easetype",
				this.m_contentFlipEaseType,
				"oncomplete",
				completeAnimation
			}));
		}
		else
		{
			completeAnimation.Invoke(null);
		}
		if (nextContent != null)
		{
			nextContent.PreStoreFlipIn();
		}
		while (!rotationDone)
		{
			yield return null;
		}
		if (nextContent != null)
		{
			this.UpdateCostAndButtonState(nextContent.GetCurrentGoldBundle(), nextContent.GetCurrentMoneyBundle());
			while (!nextContent.AnimateEntranceStart())
			{
				yield return null;
			}
			while (!nextContent.AnimateEntranceEnd())
			{
				yield return null;
			}
			nextContent.SetContentActive(true);
			nextContent.PostStoreFlipIn(flipAnimTime > 0f);
		}
		if (prevContent != null)
		{
			prevContent.PostStoreFlipOut();
		}
		this.m_settingNewModeCount--;
		this.RefreshContent();
		while (this.m_settingNewModeCount > 0)
		{
			yield return null;
		}
		if (this.m_modeButtonBlocker != null)
		{
			this.m_modeButtonBlocker.SetActive(false);
		}
		if (newMode == GeneralStoreMode.NONE)
		{
			this.ResumePreviousMusicPlaylist();
		}
		yield break;
	}

	// Token: 0x06003578 RID: 13688 RVA: 0x0010A1B0 File Offset: 0x001083B0
	private IEnumerator AnimateAndUpdateStorePane(GeneralStoreMode oldMode, GeneralStoreMode newMode)
	{
		GeneralStorePane prevPane = this.GetPane(oldMode);
		GeneralStorePane nextPane = this.GetPane(newMode);
		if (oldMode == newMode)
		{
			yield break;
		}
		this.m_settingNewModeCount++;
		if (this.m_paneScrollbar != null)
		{
			this.m_paneScrollbar.SaveScroll("STORE_MODE_" + oldMode);
			this.m_paneScrollbar.m_ScrollObject = null;
		}
		if (prevPane != null)
		{
			prevPane.PrePaneSwappedOut();
			while (!prevPane.AnimateExitStart())
			{
				yield return null;
			}
			while (!prevPane.AnimateExitEnd())
			{
				yield return null;
			}
			prevPane.PostPaneSwappedOut();
		}
		if (this.m_paneSwapAnimationTime > 0f)
		{
			int swapCount = 0;
			float delayNextPane = 0f;
			if (prevPane != null)
			{
				swapCount++;
				prevPane.transform.localPosition = Vector3.zero;
				iTween.MoveTo(prevPane.gameObject, iTween.Hash(new object[]
				{
					"position",
					this.m_paneSwapOutOffset,
					"islocal",
					true,
					"time",
					this.m_paneSwapAnimationTime,
					"easetype",
					iTween.EaseType.linear,
					"oncomplete",
					delegate(object o)
					{
						swapCount--;
					}
				}));
				delayNextPane = this.m_paneSwapAnimationTime;
			}
			if (nextPane != null)
			{
				swapCount++;
				nextPane.transform.localPosition = this.m_paneSwapInOffset;
				iTween.MoveTo(nextPane.gameObject, iTween.Hash(new object[]
				{
					"position",
					Vector3.zero,
					"islocal",
					true,
					"time",
					this.m_paneSwapAnimationTime,
					"delay",
					delayNextPane,
					"oncomplete",
					delegate(object o)
					{
						swapCount--;
					}
				}));
			}
			while (swapCount > 0)
			{
				yield return null;
			}
		}
		else
		{
			prevPane.transform.localPosition = this.m_paneSwapOutOffset;
			nextPane.transform.localPosition = Vector3.zero;
		}
		if (this.m_paneScrollbar != null && nextPane.m_paneContainer != null)
		{
			Vector3 paneStartPos;
			this.m_paneStartPositions.TryGetValue(newMode, out paneStartPos);
			this.m_paneScrollbar.m_ScrollObject = nextPane.m_paneContainer;
			this.m_paneScrollbar.ResetScrollStartPosition(paneStartPos);
			this.m_paneScrollbar.LoadScroll("STORE_MODE_" + newMode);
			this.m_paneScrollbar.EnableIfNeeded();
		}
		if (nextPane != null)
		{
			nextPane.PrePaneSwappedIn();
			while (!nextPane.AnimateEntranceStart())
			{
				yield return null;
			}
			while (!nextPane.AnimateEntranceEnd())
			{
				yield return null;
			}
			nextPane.PostPaneSwappedIn();
		}
		this.m_settingNewModeCount--;
		yield break;
	}

	// Token: 0x06003579 RID: 13689 RVA: 0x0010A1E7 File Offset: 0x001083E7
	private void OnStopShaking(object obj)
	{
		this.m_stillShaking = false;
	}

	// Token: 0x0600357A RID: 13690 RVA: 0x0010A1F0 File Offset: 0x001083F0
	private IEnumerator AnimateShakeyObjectCoroutine(float xRotationAmount, float shakeTime, float delay)
	{
		float absRotation = Mathf.Abs(xRotationAmount);
		if (absRotation - this.m_lastShakeyAmount < this.m_multipleShakeTolerance && this.m_stillShaking)
		{
			yield break;
		}
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		this.m_lastShakeyAmount = absRotation;
		this.m_stillShaking = true;
		ApplicationMgr.Get().CancelScheduledCallback(new ApplicationMgr.ScheduledCallback(this.OnStopShaking), null);
		ApplicationMgr.Get().ScheduleCallback(shakeTime * 0.25f, false, new ApplicationMgr.ScheduledCallback(this.OnStopShaking), null);
		iTween.Stop(this.m_shakeyObject);
		this.m_shakeyObject.transform.localEulerAngles = this.m_shakeyObjectOriginalLocalRotation;
		iTween.PunchRotation(this.m_shakeyObject, iTween.Hash(new object[]
		{
			"x",
			xRotationAmount,
			"time",
			shakeTime,
			"delay",
			0.001f,
			"oncomplete",
			delegate(object o)
			{
				this.m_shakeyObject.transform.localEulerAngles = this.m_shakeyObjectOriginalLocalRotation;
				this.m_lastShakeyAmount = 0f;
			}
		}));
		yield break;
	}

	// Token: 0x0600357B RID: 13691 RVA: 0x0010A238 File Offset: 0x00108438
	private void UpdateModeButtons(GeneralStoreMode mode)
	{
		foreach (GeneralStore.ModeObjects modeObjects in this.m_modeObjects)
		{
			if (!(modeObjects.m_button == null))
			{
				UIBHighlight component = modeObjects.m_button.GetComponent<UIBHighlight>();
				if (!(component == null))
				{
					if (mode == modeObjects.m_mode)
					{
						component.SelectNoSound();
					}
					else
					{
						component.Reset();
					}
				}
			}
		}
	}

	// Token: 0x0600357C RID: 13692 RVA: 0x0010A2DC File Offset: 0x001084DC
	private void GetContentPositionIndex(bool clockwise, out Vector3 contentPosition, out Vector3 contentRotation, out Vector3 lastPanelRotation, out Vector3 newPanelRotation)
	{
		lastPanelRotation = GeneralStore.s_MainPanelTriangularRotations[this.m_currentContentPositionIdx];
		if (clockwise)
		{
			this.m_currentContentPositionIdx = (this.m_currentContentPositionIdx + 1) % GeneralStore.s_ContentTriangularPositions.Length;
		}
		else
		{
			this.m_currentContentPositionIdx--;
			if (this.m_currentContentPositionIdx < 0)
			{
				this.m_currentContentPositionIdx = GeneralStore.s_ContentTriangularPositions.Length - 1;
			}
		}
		contentPosition = GeneralStore.s_ContentTriangularPositions[this.m_currentContentPositionIdx];
		contentRotation = GeneralStore.s_ContentTriangularRotations[this.m_currentContentPositionIdx];
		newPanelRotation = GeneralStore.s_MainPanelTriangularRotations[this.m_currentContentPositionIdx];
	}

	// Token: 0x0600357D RID: 13693 RVA: 0x0010A3A1 File Offset: 0x001085A1
	private void SuccessfulPurchaseEvent(Network.Bundle bundle, PaymentMethod paymentMethod, object userData)
	{
		this.RefreshContent();
	}

	// Token: 0x0600357E RID: 13694 RVA: 0x0010A3AC File Offset: 0x001085AC
	private void SuccessfulPurchaseAckEvent(Network.Bundle bundle, PaymentMethod paymentMethod, object userData)
	{
		if (this.IsShown() && SceneMgr.Get().GetMode() == SceneMgr.Mode.ADVENTURE)
		{
			this.Close();
		}
		else
		{
			this.RefreshContent();
		}
	}

	// Token: 0x0600357F RID: 13695 RVA: 0x0010A3E8 File Offset: 0x001085E8
	private void UpdateCostAndButtonState(NoGTAPPTransactionData goldBundle, Network.Bundle moneyBundle)
	{
		if (moneyBundle != null && !StoreManager.Get().IsProductAlreadyOwned(moneyBundle))
		{
			this.UpdateCostDisplay(moneyBundle);
			this.UpdateMoneyButtonState();
		}
		else if (goldBundle != null)
		{
			this.UpdateCostDisplay(goldBundle);
			this.UpdateGoldButtonState();
		}
		else
		{
			GeneralStoreContent currentContent = this.GetCurrentContent();
			if (currentContent == null || currentContent.IsPurchaseDisabled())
			{
				this.UpdateCostDisplay(GeneralStore.BuyPanelState.DISABLED, string.Empty);
			}
			else
			{
				this.UpdateCostDisplay(GeneralStore.BuyPanelState.BUY_MONEY, currentContent.GetMoneyDisplayOwnedText());
				this.UpdateMoneyButtonState();
			}
		}
	}

	// Token: 0x06003580 RID: 13696 RVA: 0x0010A478 File Offset: 0x00108678
	private void FireModeChangedEvent(GeneralStoreMode oldMode, GeneralStoreMode newMode)
	{
		GeneralStore.ModeChanged[] array = this.m_modeChangedListeners.ToArray();
		foreach (GeneralStore.ModeChanged modeChanged in array)
		{
			modeChanged(oldMode, newMode);
		}
	}

	// Token: 0x06003581 RID: 13697 RVA: 0x0010A4B3 File Offset: 0x001086B3
	private bool OnNavigateBack()
	{
		this.CloseImpl(true);
		return true;
	}

	// Token: 0x06003582 RID: 13698 RVA: 0x0010A4C0 File Offset: 0x001086C0
	private void CloseImpl(bool closeWithAnimation)
	{
		if (this.m_settingNewModeCount > 0)
		{
			return;
		}
		PresenceMgr.Get().SetPrevStatus();
		this.Hide(closeWithAnimation);
		SoundManager.Get().LoadAndPlay("Store_window_shrink", base.gameObject);
		base.EnableFullScreenEffects(false);
		base.FireExitEvent(false);
	}

	// Token: 0x06003583 RID: 13699 RVA: 0x0010A510 File Offset: 0x00108710
	protected override string GetOwnedTooltipString()
	{
		switch (this.m_currentMode)
		{
		case GeneralStoreMode.CARDS:
			return GameStrings.Get("GLUE_STORE_PACK_BUTTON_TEXT_PURCHASED");
		case GeneralStoreMode.ADVENTURE:
			return GameStrings.Get("GLUE_STORE_DUNGEON_BUTTON_TEXT_PURCHASED");
		case GeneralStoreMode.HEROES:
			return GameStrings.Get("GLUE_STORE_HERO_BUTTON_TEXT_PURCHASED");
		default:
			return string.Empty;
		}
	}

	// Token: 0x0400213D RID: 8509
	[CustomEditField(Sections = "General Store")]
	public GameObject m_mainPanel;

	// Token: 0x0400213E RID: 8510
	[CustomEditField(Sections = "General Store")]
	public GameObject m_buyGoldPanel;

	// Token: 0x0400213F RID: 8511
	[CustomEditField(Sections = "General Store")]
	public GameObject m_buyMoneyPanel;

	// Token: 0x04002140 RID: 8512
	[CustomEditField(Sections = "General Store")]
	public GameObject m_buyEmptyPanel;

	// Token: 0x04002141 RID: 8513
	[CustomEditField(Sections = "General Store", ListTable = true)]
	public List<GeneralStore.ModeObjects> m_modeObjects = new List<GeneralStore.ModeObjects>();

	// Token: 0x04002142 RID: 8514
	[CustomEditField(Sections = "General Store")]
	public MeshRenderer m_accentIcon;

	// Token: 0x04002143 RID: 8515
	[CustomEditField(Sections = "General Store/Mode Buttons")]
	public GameObject m_modeButtonBlocker;

	// Token: 0x04002144 RID: 8516
	[CustomEditField(Sections = "General Store/Text")]
	public UberText m_moneyCostText;

	// Token: 0x04002145 RID: 8517
	[CustomEditField(Sections = "General Store/Text")]
	public UberText m_goldCostText;

	// Token: 0x04002146 RID: 8518
	[CustomEditField(Sections = "General Store/Text")]
	public MultiSliceElement m_productDetailsContainer;

	// Token: 0x04002147 RID: 8519
	[CustomEditField(Sections = "General Store/Text")]
	public UberText m_productDetailsHeadlineText;

	// Token: 0x04002148 RID: 8520
	[CustomEditField(Sections = "General Store/Text")]
	public UberText m_productDetailsText;

	// Token: 0x04002149 RID: 8521
	[CustomEditField(Sections = "General Store/Text")]
	public float m_productDetailsRegularHeight = 13f;

	// Token: 0x0400214A RID: 8522
	[CustomEditField(Sections = "General Store/Text")]
	public float m_productDetailsExtendedHeight = 15.5f;

	// Token: 0x0400214B RID: 8523
	[CustomEditField(Sections = "General Store/Text")]
	public UberText m_koreanProductDetailsText;

	// Token: 0x0400214C RID: 8524
	[CustomEditField(Sections = "General Store/Text")]
	public UberText m_koreanWarningText;

	// Token: 0x0400214D RID: 8525
	[CustomEditField(Sections = "General Store/Text")]
	public float m_koreanProductDetailsRegularHeight = 8f;

	// Token: 0x0400214E RID: 8526
	[CustomEditField(Sections = "General Store/Text")]
	public float m_koreanProductDetailsExtendedHeight = 10.5f;

	// Token: 0x0400214F RID: 8527
	[CustomEditField(Sections = "General Store/Text")]
	public GameObject m_chooseArrowContainer;

	// Token: 0x04002150 RID: 8528
	[CustomEditField(Sections = "General Store/Text")]
	public UberText m_chooseArrowText;

	// Token: 0x04002151 RID: 8529
	[CustomEditField(Sections = "General Store/Content")]
	public float m_contentFlipAnimationTime = 0.5f;

	// Token: 0x04002152 RID: 8530
	[CustomEditField(Sections = "General Store/Content")]
	public iTween.EaseType m_contentFlipEaseType = iTween.EaseType.easeOutBounce;

	// Token: 0x04002153 RID: 8531
	[CustomEditField(Sections = "General Store/Panes")]
	public GeneralStorePane m_defaultPane;

	// Token: 0x04002154 RID: 8532
	[CustomEditField(Sections = "General Store/Panes")]
	public Vector3 m_paneSwapOutOffset = new Vector3(0.05f, 0f, 0f);

	// Token: 0x04002155 RID: 8533
	[CustomEditField(Sections = "General Store/Panes")]
	public Vector3 m_paneSwapInOffset = new Vector3(0f, -0.05f, 0f);

	// Token: 0x04002156 RID: 8534
	[CustomEditField(Sections = "General Store/Panes")]
	public float m_paneSwapAnimationTime = 1f;

	// Token: 0x04002157 RID: 8535
	[CustomEditField(Sections = "General Store/Panes")]
	public UIBScrollable m_paneScrollbar;

	// Token: 0x04002158 RID: 8536
	[CustomEditField(Sections = "General Store/Shake Store")]
	public GameObject m_shakeyObject;

	// Token: 0x04002159 RID: 8537
	[CustomEditField(Sections = "General Store/Shake Store")]
	public float m_multipleShakeTolerance = 2.5f;

	// Token: 0x0400215A RID: 8538
	[CustomEditField(Sections = "General Store/Sounds", T = EditType.SOUND_PREFAB)]
	public string m_contentFlipSound;

	// Token: 0x0400215B RID: 8539
	private static readonly int MIN_GOLD_FOR_CHANGE_QTY_TOOLTIP = 500;

	// Token: 0x0400215C RID: 8540
	private static readonly float FLIP_BUY_PANEL_ANIM_TIME = 0.1f;

	// Token: 0x0400215D RID: 8541
	private static readonly Vector3 MAIN_PANEL_ANGLE_TO_ROTATE = new Vector3(0.33333334f, 0f, 0f);

	// Token: 0x0400215E RID: 8542
	private static readonly GeneralStoreMode[] s_ContentOrdering = new GeneralStoreMode[]
	{
		GeneralStoreMode.ADVENTURE,
		GeneralStoreMode.CARDS
	};

	// Token: 0x0400215F RID: 8543
	private static readonly Vector3[] s_ContentTriangularPositions = new Vector3[]
	{
		new Vector3(0f, 0.125f, 0f),
		new Vector3(0f, -0.064f, -0.109f),
		new Vector3(0f, -0.064f, 0.109f)
	};

	// Token: 0x04002160 RID: 8544
	private static readonly Vector3[] s_ContentTriangularRotations = new Vector3[]
	{
		new Vector3(-60f, 0f, -180f),
		new Vector3(0f, -180f, 0f),
		new Vector3(60f, 0f, 180f)
	};

	// Token: 0x04002161 RID: 8545
	private static readonly Vector3[] s_MainPanelTriangularRotations = new Vector3[]
	{
		new Vector3(0f, 0f, 0f),
		new Vector3(-240f, 0f, 0f),
		new Vector3(-120f, 0f, 0f)
	};

	// Token: 0x04002162 RID: 8546
	private static GeneralStore s_instance;

	// Token: 0x04002163 RID: 8547
	private GeneralStore.BuyPanelState m_buyPanelState;

	// Token: 0x04002164 RID: 8548
	private bool m_staticTextResized;

	// Token: 0x04002165 RID: 8549
	private GeneralStoreMode m_currentMode;

	// Token: 0x04002166 RID: 8550
	private int m_settingNewModeCount;

	// Token: 0x04002167 RID: 8551
	private Vector3 m_shakeyObjectOriginalLocalRotation = Vector3.zero;

	// Token: 0x04002168 RID: 8552
	private float m_lastShakeyAmount;

	// Token: 0x04002169 RID: 8553
	private bool m_stillShaking;

	// Token: 0x0400216A RID: 8554
	private List<GeneralStore.ModeChanged> m_modeChangedListeners = new List<GeneralStore.ModeChanged>();

	// Token: 0x0400216B RID: 8555
	private int m_currentContentPositionIdx;

	// Token: 0x0400216C RID: 8556
	private MusicPlaylistType m_prevPlaylist;

	// Token: 0x0400216D RID: 8557
	private Map<GeneralStoreMode, Vector3> m_paneStartPositions = new Map<GeneralStoreMode, Vector3>();

	// Token: 0x02000536 RID: 1334
	[Serializable]
	public class ModeObjects
	{
		// Token: 0x04002776 RID: 10102
		public GeneralStoreMode m_mode;

		// Token: 0x04002777 RID: 10103
		public GeneralStoreContent m_content;

		// Token: 0x04002778 RID: 10104
		public GeneralStorePane m_pane;

		// Token: 0x04002779 RID: 10105
		public UIBButton m_button;
	}

	// Token: 0x02000538 RID: 1336
	private enum BuyPanelState
	{
		// Token: 0x0400277D RID: 10109
		DISABLED,
		// Token: 0x0400277E RID: 10110
		BUY_GOLD,
		// Token: 0x0400277F RID: 10111
		BUY_MONEY
	}

	// Token: 0x02000539 RID: 1337
	// (Invoke) Token: 0x06003DF3 RID: 15859
	public delegate void ModeChanged(GeneralStoreMode oldMode, GeneralStoreMode newMode);
}
