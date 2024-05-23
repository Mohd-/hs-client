using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000447 RID: 1095
[CustomEditClass]
public class GeneralStoreAdventureContent : GeneralStoreContent
{
	// Token: 0x0600367B RID: 13947 RVA: 0x0010C580 File Offset: 0x0010A780
	private void Awake()
	{
		this.m_adventureDisplay1 = this.m_adventureDisplay;
		this.m_adventureDisplay2 = Object.Instantiate<GeneralStoreAdventureContentDisplay>(this.m_adventureDisplay);
		this.m_adventureDisplay2.transform.parent = this.m_adventureDisplay1.transform.parent;
		this.m_adventureDisplay2.transform.localPosition = this.m_adventureDisplay1.transform.localPosition;
		this.m_adventureDisplay2.transform.localScale = this.m_adventureDisplay1.transform.localScale;
		this.m_adventureDisplay2.transform.localRotation = this.m_adventureDisplay1.transform.localRotation;
		this.m_adventureDisplay2.gameObject.SetActive(false);
		if (this.m_adventureDisplay1.m_rewardChest != null)
		{
			this.m_adventureDisplay1.m_rewardChest.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnAdventuresShowPreviewCard));
			this.m_adventureDisplay2.m_rewardChest.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnAdventuresShowPreviewCard));
			if (!UniversalInputManager.UsePhoneUI)
			{
				this.m_adventureDisplay1.m_rewardChest.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnAdventuresHidePreviewCard));
				this.m_adventureDisplay2.m_rewardChest.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnAdventuresHidePreviewCard));
			}
		}
		AdventureProgressMgr.Get().RegisterProgressUpdatedListener(new AdventureProgressMgr.AdventureProgressUpdatedCallback(this.OnAdventureProgressUpdated));
		this.m_adventureCardPreviewPanel.SetActive(false);
		this.m_parentStore.SetChooseDescription(GameStrings.Get("GLUE_STORE_CHOOSE_ADVENTURE"));
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_adventureCardPreviewOffClicker.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnAdventuresHidePreviewCard));
		}
		List<AdventureDbfRecord> sortedAdventureRecordsWithStorePrefab = GameUtils.GetSortedAdventureRecordsWithStorePrefab();
		foreach (AdventureDbfRecord adventureDbfRecord in sortedAdventureRecordsWithStorePrefab)
		{
			string storePrefab = adventureDbfRecord.StorePrefab;
			GameObject gameObject = AssetLoader.Get().LoadGameObject(FileUtils.GameAssetPathToName(storePrefab), true, false);
			if (!(gameObject == null))
			{
				StoreAdventureDef component = gameObject.GetComponent<StoreAdventureDef>();
				if (component == null)
				{
					Debug.LogError(string.Format("StoreAdventureDef not found in object: {0}", storePrefab));
				}
				else
				{
					this.m_storeAdvDefs.Add(adventureDbfRecord.ID, component);
				}
			}
		}
	}

	// Token: 0x0600367C RID: 13948 RVA: 0x0010C7EC File Offset: 0x0010A9EC
	private void OnDestroy()
	{
		AdventureProgressMgr.Get().RemoveProgressUpdatedListener(new AdventureProgressMgr.AdventureProgressUpdatedCallback(this.OnAdventureProgressUpdated));
	}

	// Token: 0x0600367D RID: 13949 RVA: 0x0010C808 File Offset: 0x0010AA08
	public void SetAdventureType(AdventureDbId adventure, bool forceImmediate = false)
	{
		if (this.m_selectedAdventureType == adventure)
		{
			return;
		}
		this.m_selectedAdventureType = adventure;
		Network.Bundle bundle = null;
		StoreManager.Get().GetAvailableAdventureBundle(this.m_selectedAdventureType, GeneralStoreAdventureContent.REQUIRE_REAL_MONEY_BUNDLE_OPTION, out bundle);
		base.SetCurrentMoneyBundle(bundle, false);
		this.AnimateAndUpdateDisplay((int)adventure, forceImmediate);
		this.AnimateAdventureRadioButtonBar();
		this.UpdateAdventureDescription(bundle);
		this.UpdateAdventureTypeMusic();
	}

	// Token: 0x0600367E RID: 13950 RVA: 0x0010C866 File Offset: 0x0010AA66
	public AdventureDbId GetAdventureType()
	{
		return this.m_selectedAdventureType;
	}

	// Token: 0x0600367F RID: 13951 RVA: 0x0010C870 File Offset: 0x0010AA70
	public StoreAdventureDef GetStoreAdventureDef(int advId)
	{
		StoreAdventureDef result = null;
		this.m_storeAdvDefs.TryGetValue(advId, out result);
		return result;
	}

	// Token: 0x06003680 RID: 13952 RVA: 0x0010C88F File Offset: 0x0010AA8F
	public Map<int, StoreAdventureDef> GetStoreAdventureDefs()
	{
		return this.m_storeAdvDefs;
	}

	// Token: 0x06003681 RID: 13953 RVA: 0x0010C898 File Offset: 0x0010AA98
	public override void PostStoreFlipIn(bool animateIn)
	{
		this.UpdateAdventureTypeMusic();
		if (this.m_preorderCardBackReward != null)
		{
			this.m_preorderCardBackReward.ShowCardBackReward();
		}
	}

	// Token: 0x06003682 RID: 13954 RVA: 0x0010C8C7 File Offset: 0x0010AAC7
	public override void PreStoreFlipIn()
	{
		if (this.m_preorderCardBackReward != null)
		{
			this.m_preorderCardBackReward.HideCardBackReward();
		}
	}

	// Token: 0x06003683 RID: 13955 RVA: 0x0010C8E5 File Offset: 0x0010AAE5
	public override void PreStoreFlipOut()
	{
		if (this.m_preorderCardBackReward != null)
		{
			this.m_preorderCardBackReward.HideCardBackReward();
		}
	}

	// Token: 0x06003684 RID: 13956 RVA: 0x0010C903 File Offset: 0x0010AB03
	public override bool AnimateEntranceEnd()
	{
		this.m_adventureRadioButton.gameObject.SetActive(true);
		return true;
	}

	// Token: 0x06003685 RID: 13957 RVA: 0x0010C917 File Offset: 0x0010AB17
	public override bool AnimateExitStart()
	{
		this.m_adventureRadioButton.gameObject.SetActive(false);
		return true;
	}

	// Token: 0x06003686 RID: 13958 RVA: 0x0010C92B File Offset: 0x0010AB2B
	public override bool AnimateExitEnd()
	{
		return true;
	}

	// Token: 0x06003687 RID: 13959 RVA: 0x0010C930 File Offset: 0x0010AB30
	public override void TryBuyWithMoney(Network.Bundle bundle, GeneralStoreContent.BuyEvent successBuyCB, GeneralStoreContent.BuyEvent failedBuyCB)
	{
		if (base.IsContentActive())
		{
			if (!AchieveManager.Get().HasUnlockedFeature(Achievement.UnlockableFeature.VANILLA_HEROES))
			{
				AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
				popupInfo.m_headerText = GameStrings.Get("GLUE_STORE_ADVENTURE_LOCKED_HEROES_WARNING_TITLE");
				popupInfo.m_text = GameStrings.Get("GLUE_STORE_ADVENTURE_LOCKED_HEROES_WARNING_TEXT");
				popupInfo.m_showAlertIcon = true;
				popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
				popupInfo.m_responseCallback = delegate(AlertPopup.Response response, object data)
				{
					if (response == AlertPopup.Response.CANCEL)
					{
						this.m_parentStore.ActivateCover(false);
						if (failedBuyCB != null)
						{
							failedBuyCB();
						}
					}
					else if (successBuyCB != null)
					{
						successBuyCB();
					}
				};
				this.m_parentStore.ActivateCover(true);
				DialogManager.Get().ShowPopup(popupInfo);
			}
			else if (successBuyCB != null)
			{
				successBuyCB();
			}
		}
		else if (failedBuyCB != null)
		{
			failedBuyCB();
		}
	}

	// Token: 0x06003688 RID: 13960 RVA: 0x0010CA07 File Offset: 0x0010AC07
	public override void TryBuyWithGold(GeneralStoreContent.BuyEvent successBuyCB = null, GeneralStoreContent.BuyEvent failedBuyCB = null)
	{
		if (successBuyCB != null)
		{
			successBuyCB();
		}
	}

	// Token: 0x06003689 RID: 13961 RVA: 0x0010CA18 File Offset: 0x0010AC18
	protected override void OnRefresh()
	{
		Network.Bundle bundle = null;
		StoreManager.Get().GetAvailableAdventureBundle(this.m_selectedAdventureType, GeneralStoreAdventureContent.REQUIRE_REAL_MONEY_BUNDLE_OPTION, out bundle);
		base.SetCurrentMoneyBundle(bundle, false);
		this.UpdateRadioButtonText(bundle);
		this.UpdateAdventureDescription(bundle);
	}

	// Token: 0x0600368A RID: 13962 RVA: 0x0010CA55 File Offset: 0x0010AC55
	protected override void OnBundleChanged(NoGTAPPTransactionData goldBundle, Network.Bundle moneyBundle)
	{
		this.UpdateRadioButtonText(moneyBundle);
		this.UpdateAdventureDescription(moneyBundle);
	}

	// Token: 0x0600368B RID: 13963 RVA: 0x0010CA65 File Offset: 0x0010AC65
	public override void StoreShown(bool isCurrent)
	{
		if (!isCurrent)
		{
			return;
		}
		this.UpdateAdventureTypeMusic();
	}

	// Token: 0x0600368C RID: 13964 RVA: 0x0010CA74 File Offset: 0x0010AC74
	public override void StoreHidden(bool isCurrent)
	{
		foreach (KeyValuePair<string, Actor> keyValuePair in this.m_loadedPreviewCards)
		{
			Object.Destroy(keyValuePair.Value.gameObject);
		}
		this.m_loadedPreviewCards.Clear();
		if (!isCurrent)
		{
			return;
		}
		this.HidePreviewCardPanel();
	}

	// Token: 0x0600368D RID: 13965 RVA: 0x0010CAF0 File Offset: 0x0010ACF0
	public override bool IsPurchaseDisabled()
	{
		return this.m_selectedAdventureType == AdventureDbId.INVALID;
	}

	// Token: 0x0600368E RID: 13966 RVA: 0x0010CAFB File Offset: 0x0010ACFB
	public override string GetMoneyDisplayOwnedText()
	{
		return GameStrings.Get("GLUE_STORE_DUNGEON_BUTTON_COST_OWNED_TEXT");
	}

	// Token: 0x0600368F RID: 13967 RVA: 0x0010CB07 File Offset: 0x0010AD07
	private GameObject GetCurrentDisplayContainer()
	{
		return this.GetCurrentDisplay().gameObject;
	}

	// Token: 0x06003690 RID: 13968 RVA: 0x0010CB14 File Offset: 0x0010AD14
	private GameObject GetNextDisplayContainer()
	{
		return ((this.m_currentDisplay + 1) % 2 != 0) ? this.m_adventureDisplay2.gameObject : this.m_adventureDisplay1.gameObject;
	}

	// Token: 0x06003691 RID: 13969 RVA: 0x0010CB4B File Offset: 0x0010AD4B
	private GeneralStoreAdventureContentDisplay GetCurrentDisplay()
	{
		return (this.m_currentDisplay != 0) ? this.m_adventureDisplay2 : this.m_adventureDisplay1;
	}

	// Token: 0x06003692 RID: 13970 RVA: 0x0010CB6C File Offset: 0x0010AD6C
	private void OnAdventuresShowPreviewCard(UIEvent e)
	{
		StoreAdventureDef storeAdventureDef = this.GetStoreAdventureDef((int)this.m_selectedAdventureType);
		if (storeAdventureDef == null)
		{
			Debug.LogError(string.Format("Unable to find preview cards for {0} adventure.", this.m_selectedAdventureType));
			return;
		}
		string[] previewCards = storeAdventureDef.m_previewCards.ToArray();
		if (previewCards.Length == 0)
		{
			Debug.LogError(string.Format("No preview cards defined for {0} adventure.", this.m_selectedAdventureType));
			return;
		}
		this.m_showPreviewCards = true;
		SoundManager.Get().LoadAndPlay("collection_manager_card_mouse_over");
		foreach (KeyValuePair<string, Actor> keyValuePair in this.m_loadedPreviewCards)
		{
			keyValuePair.Value.gameObject.SetActive(false);
		}
		int loadedPreviewCards = 0;
		int num = 0;
		string[] previewCards2 = previewCards;
		for (int i = 0; i < previewCards2.Length; i++)
		{
			string previewCard = previewCards2[i];
			int cardIndex = num;
			this.LoadAdventurePreviewCard(previewCard, delegate(Actor cardActor)
			{
				cardActor.transform.position = this.m_adventureCardPreviewBones[cardIndex].transform.position;
				cardActor.transform.rotation = this.m_adventureCardPreviewBones[cardIndex].transform.rotation;
				cardActor.transform.parent = this.m_adventureCardPreviewBones[cardIndex].transform;
				cardActor.transform.localScale = Vector3.one;
				loadedPreviewCards++;
				cardActor.gameObject.SetActive(this.m_showPreviewCards);
				if (this.m_showPreviewCards && loadedPreviewCards == previewCards.Length)
				{
					this.ShowPreviewCardPanel();
				}
			});
			num++;
		}
	}

	// Token: 0x06003693 RID: 13971 RVA: 0x0010CCD4 File Offset: 0x0010AED4
	private void LoadAdventurePreviewCard(string previewCard, GeneralStoreAdventureContent.DelOnAdventurePreviewCardLoaded onLoadComplete)
	{
		Actor previewCard2 = null;
		if (this.m_loadedPreviewCards.TryGetValue(previewCard, out previewCard2))
		{
			onLoadComplete(previewCard2);
		}
		else
		{
			DefLoader.Get().LoadFullDef(previewCard, delegate(string cardID, FullDef fullDef, object data)
			{
				AssetLoader.Get().LoadActor(ActorNames.GetHandActor(fullDef.GetEntityDef()), delegate(string actorName, GameObject actorObject, object data2)
				{
					if (actorObject == null)
					{
						Debug.LogWarning(string.Format("FAILED to load actor \"{0}\"", actorName));
						onLoadComplete(null);
						return;
					}
					Actor component = actorObject.GetComponent<Actor>();
					if (component == null)
					{
						Debug.LogWarning(string.Format("ERROR actor \"{0}\" has no Actor component", actorName));
						onLoadComplete(null);
						return;
					}
					component.SetCardDef(fullDef.GetCardDef());
					component.SetEntityDef(fullDef.GetEntityDef());
					component.UpdateAllComponents();
					SceneUtils.SetLayer(component.gameObject, this.gameObject.layer);
					component.Show();
					this.m_loadedPreviewCards.Add(previewCard, component);
					onLoadComplete(component);
				}, null, false);
			});
		}
	}

	// Token: 0x06003694 RID: 13972 RVA: 0x0010CD43 File Offset: 0x0010AF43
	private void OnAdventuresHidePreviewCard(UIEvent e)
	{
		this.m_showPreviewCards = false;
		this.HidePreviewCardPanel();
	}

	// Token: 0x06003695 RID: 13973 RVA: 0x0010CD54 File Offset: 0x0010AF54
	private void ShowPreviewCardPanel()
	{
		this.m_adventureCardPreviewPanel.SetActive(true);
		this.m_adventureCardPreviewPanel.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		iTween.StopByName(this.m_adventureCardPreviewPanel, "PreviewCardPanelScale");
		iTween.ScaleTo(this.m_adventureCardPreviewPanel, iTween.Hash(new object[]
		{
			"scale",
			Vector3.one,
			"time",
			0.1f,
			"name",
			"PreviewCardPanelScale",
			"easetype",
			iTween.EaseType.linear
		}));
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_parentStore.ActivateCover(true);
		}
	}

	// Token: 0x06003696 RID: 13974 RVA: 0x0010CE20 File Offset: 0x0010B020
	private void HidePreviewCardPanel()
	{
		iTween.StopByName(this.m_adventureCardPreviewPanel, "PreviewCardPanelScale");
		iTween.ScaleTo(this.m_adventureCardPreviewPanel, iTween.Hash(new object[]
		{
			"scale",
			new Vector3(0.02f, 0.02f, 0.02f),
			"time",
			0.1f,
			"name",
			"PreviewCardPanelScale",
			"oncomplete",
			delegate(object o)
			{
				this.m_adventureCardPreviewPanel.SetActive(false);
				if (UniversalInputManager.UsePhoneUI)
				{
					this.m_parentStore.ActivateCover(false);
				}
			},
			"easetype",
			iTween.EaseType.linear
		}));
	}

	// Token: 0x06003697 RID: 13975 RVA: 0x0010CEC8 File Offset: 0x0010B0C8
	private void UpdateRadioButtonText(Network.Bundle moneyBundle)
	{
		this.m_adventureRadioButton.SetSelected(true);
		if (moneyBundle == null)
		{
			this.m_adventureRadioButtonText.Text = GameStrings.Get("GLUE_STORE_DUNGEON_BUTTON_TEXT_PURCHASED");
			this.m_adventureRadioButtonCostText.Text = string.Empty;
		}
		else
		{
			bool flag = this.IsPreOrder();
			string key = (!flag) ? "GLUE_STORE_DUNGEON_BUTTON_TEXT" : "GLUE_STORE_DUNGEON_BUTTON_PREORDER_TEXT";
			this.m_adventureRadioButtonText.Text = GameStrings.Get(key);
			string text = StoreManager.Get().FormatCostBundle(moneyBundle);
			int nonPreorderItemCount = StoreManager.Get().GetNonPreorderItemCount(moneyBundle.Items);
			this.m_adventureRadioButtonCostText.Text = GameStrings.Format("GLUE_STORE_DUNGEON_BUTTON_COST_TEXT", new object[]
			{
				nonPreorderItemCount,
				text
			});
		}
		if (this.m_adventureOwnedCheckmark != null)
		{
			this.m_adventureOwnedCheckmark.SetActive(moneyBundle == null);
		}
	}

	// Token: 0x06003698 RID: 13976 RVA: 0x0010CFA4 File Offset: 0x0010B1A4
	private void UpdateAdventureDescription(Network.Bundle bundle)
	{
		if (this.m_selectedAdventureType != AdventureDbId.INVALID)
		{
			string title = string.Empty;
			string desc = string.Empty;
			string warning = string.Empty;
			AdventureDbfRecord record = GameDbf.Adventure.GetRecord((int)this.m_selectedAdventureType);
			if (record == null)
			{
				Debug.LogError(string.Format("Unable to find adventure record ID: {0}", this.m_selectedAdventureType));
			}
			else if (bundle == null)
			{
				title = record.StoreOwnedHeadline;
				desc = record.StoreOwnedDesc;
			}
			else if (this.IsPreOrder())
			{
				title = record.StorePreorderHeadline;
				desc = record.StorePreorderDesc;
			}
			else
			{
				int nonPreorderItemCount = StoreManager.Get().GetNonPreorderItemCount(bundle.Items);
				DbfLocValue dbfLocValue = record.GetVar(string.Format("STORE_BUY_WINGS_{0}_HEADLINE", nonPreorderItemCount)) as DbfLocValue;
				DbfLocValue dbfLocValue2 = record.GetVar(string.Format("STORE_BUY_WINGS_{0}_DESC", nonPreorderItemCount)) as DbfLocValue;
				title = ((dbfLocValue != null) ? dbfLocValue.GetString(true) : string.Empty);
				desc = ((dbfLocValue2 != null) ? dbfLocValue2.GetString(true) : string.Empty);
			}
			if (StoreManager.Get().IsKoreanCustomer())
			{
				warning = GameStrings.Get("GLUE_STORE_KOREAN_PRODUCT_DETAILS_ADVENTURE");
			}
			if (this.m_adventureCardPreviewText != null)
			{
				this.m_adventureCardPreviewText.Text = record.StorePreviewRewardsText;
			}
			this.m_parentStore.SetDescription(title, desc, warning);
			StoreAdventureDef storeAdventureDef = this.GetStoreAdventureDef((int)this.m_selectedAdventureType);
			if (storeAdventureDef != null)
			{
				Texture accentTexture = null;
				if (!string.IsNullOrEmpty(storeAdventureDef.m_accentTextureName))
				{
					accentTexture = AssetLoader.Get().LoadTexture(FileUtils.GameAssetPathToName(storeAdventureDef.m_accentTextureName), false);
				}
				this.m_parentStore.SetAccentTexture(accentTexture);
			}
		}
		else
		{
			this.m_parentStore.HideAccentTexture();
			this.m_parentStore.SetChooseDescription(GameStrings.Get("GLUE_STORE_CHOOSE_ADVENTURE"));
		}
	}

	// Token: 0x06003699 RID: 13977 RVA: 0x0010D19C File Offset: 0x0010B39C
	private void UpdateAdventureTypeMusic()
	{
		if (this.m_parentStore.GetMode() == GeneralStoreMode.NONE)
		{
			return;
		}
		StoreAdventureDef storeAdventureDef = this.GetStoreAdventureDef((int)this.m_selectedAdventureType);
		if (storeAdventureDef == null || storeAdventureDef.m_playlist == MusicPlaylistType.Invalid || !MusicManager.Get().StartPlaylist(storeAdventureDef.m_playlist))
		{
			this.m_parentStore.ResumePreviousMusicPlaylist();
		}
	}

	// Token: 0x0600369A RID: 13978 RVA: 0x0010D200 File Offset: 0x0010B400
	private void AnimateAndUpdateDisplay(int id, bool forceImmediate)
	{
		if (this.m_preorderCardBackReward != null)
		{
			this.m_preorderCardBackReward.HideCardBackReward();
		}
		GameObject currDisplay = null;
		if (this.m_currentDisplay == -1)
		{
			this.m_currentDisplay = 1;
			currDisplay = this.m_adventureEmptyDisplay;
		}
		else
		{
			currDisplay = this.GetCurrentDisplayContainer();
		}
		GameObject nextDisplayContainer = this.GetNextDisplayContainer();
		this.m_currentDisplay = (this.m_currentDisplay + 1) % 2;
		nextDisplayContainer.SetActive(true);
		if (!forceImmediate)
		{
			currDisplay.transform.localRotation = Quaternion.identity;
			nextDisplayContainer.transform.localEulerAngles = new Vector3(180f, 0f, 0f);
			iTween.StopByName(currDisplay, "ROTATION_TWEEN");
			iTween.StopByName(nextDisplayContainer, "ROTATION_TWEEN");
			iTween.RotateBy(currDisplay, iTween.Hash(new object[]
			{
				"amount",
				new Vector3(0.5f, 0f, 0f),
				"time",
				0.5f,
				"name",
				"ROTATION_TWEEN",
				"oncomplete",
				delegate(object o)
				{
					currDisplay.SetActive(false);
				}
			}));
			iTween.RotateBy(nextDisplayContainer, iTween.Hash(new object[]
			{
				"amount",
				new Vector3(0.5f, 0f, 0f),
				"time",
				0.5f,
				"name",
				"ROTATION_TWEEN"
			}));
			if (!string.IsNullOrEmpty(this.m_backgroundFlipSound))
			{
				SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_backgroundFlipSound));
			}
		}
		else
		{
			nextDisplayContainer.transform.localRotation = Quaternion.identity;
			currDisplay.transform.localEulerAngles = new Vector3(180f, 0f, 0f);
			currDisplay.SetActive(false);
		}
		AdventureDbfRecord record = GameDbf.Adventure.GetRecord(id);
		bool flag = this.IsPreOrder();
		StoreAdventureDef storeAdventureDef = this.GetStoreAdventureDef(id);
		GeneralStoreAdventureContentDisplay currentDisplay = this.GetCurrentDisplay();
		currentDisplay.UpdateAdventureType(storeAdventureDef, record);
		currentDisplay.SetPreOrder(flag);
		if (this.m_preorderCardBackReward != null && flag)
		{
			this.m_preorderCardBackReward.SetCardBack(storeAdventureDef.m_preorderCardBackId);
			this.m_preorderCardBackReward.ShowCardBackReward();
		}
	}

	// Token: 0x0600369B RID: 13979 RVA: 0x0010D488 File Offset: 0x0010B688
	private void AnimateAdventureRadioButtonBar()
	{
		if (this.m_adventureRadioButtonContainer == null)
		{
			return;
		}
		this.m_adventureRadioButtonContainer.SetActive(false);
		if (this.m_selectedAdventureType == AdventureDbId.INVALID)
		{
			return;
		}
		iTween.Stop(this.m_adventureRadioButtonContainer);
		this.m_adventureRadioButtonContainer.transform.localRotation = Quaternion.identity;
		this.m_adventureRadioButtonContainer.SetActive(true);
		this.m_adventureRadioButton.SetSelected(true);
		iTween.RotateBy(this.m_adventureRadioButtonContainer, iTween.Hash(new object[]
		{
			"amount",
			new Vector3(-1f, 0f, 0f),
			"time",
			this.m_backgroundFlipAnimTime,
			"delay",
			0.001f
		}));
	}

	// Token: 0x0600369C RID: 13980 RVA: 0x0010D55C File Offset: 0x0010B75C
	private void OnAdventureProgressUpdated(bool isStartupAction, AdventureMission.WingProgress oldProgress, AdventureMission.WingProgress newProgress, object userData)
	{
		if (newProgress == null)
		{
			return;
		}
		if ((oldProgress != null && oldProgress.IsOwned()) || !newProgress.IsOwned())
		{
			return;
		}
		WingDbfRecord record = GameDbf.Wing.GetRecord(newProgress.Wing);
		if (record == null)
		{
			return;
		}
		int adventureId = record.AdventureId;
		if (adventureId != (int)this.m_selectedAdventureType)
		{
			return;
		}
		Network.Bundle bundle = null;
		StoreManager.Get().GetAvailableAdventureBundle(this.GetAdventureType(), GeneralStoreAdventureContent.REQUIRE_REAL_MONEY_BUNDLE_OPTION, out bundle);
		base.SetCurrentMoneyBundle(bundle, false);
		if (this.m_parentStore != null)
		{
			this.m_parentStore.RefreshContent();
		}
	}

	// Token: 0x0600369D RID: 13981 RVA: 0x0010D5F8 File Offset: 0x0010B7F8
	private bool IsPreOrder()
	{
		Network.Bundle currentMoneyBundle = base.GetCurrentMoneyBundle();
		return currentMoneyBundle != null && StoreManager.Get().IsProductPrePurchase(currentMoneyBundle);
	}

	// Token: 0x040021E9 RID: 8681
	[CustomEditField(Sections = "General Store")]
	public GeneralStoreAdventureContentDisplay m_adventureDisplay;

	// Token: 0x040021EA RID: 8682
	[CustomEditField(Sections = "Animation/Preorder")]
	public GeneralStoreRewardsCardBack m_preorderCardBackReward;

	// Token: 0x040021EB RID: 8683
	[CustomEditField(Sections = "General Store")]
	public GameObject m_adventureEmptyDisplay;

	// Token: 0x040021EC RID: 8684
	[CustomEditField(Sections = "Rewards")]
	public GameObject m_adventureCardPreviewPanel;

	// Token: 0x040021ED RID: 8685
	[CustomEditField(Sections = "Rewards")]
	public UberText m_adventureCardPreviewText;

	// Token: 0x040021EE RID: 8686
	[CustomEditField(Sections = "Rewards")]
	public List<GameObject> m_adventureCardPreviewBones;

	// Token: 0x040021EF RID: 8687
	[CustomEditField(Sections = "Rewards")]
	public PegUIElement m_adventureCardPreviewOffClicker;

	// Token: 0x040021F0 RID: 8688
	[CustomEditField(Sections = "General Store/Buttons")]
	public GameObject m_adventureRadioButtonContainer;

	// Token: 0x040021F1 RID: 8689
	[CustomEditField(Sections = "General Store/Buttons")]
	public UberText m_adventureRadioButtonText;

	// Token: 0x040021F2 RID: 8690
	[CustomEditField(Sections = "General Store/Buttons")]
	public UberText m_adventureRadioButtonCostText;

	// Token: 0x040021F3 RID: 8691
	[CustomEditField(Sections = "General Store/Buttons")]
	public RadioButton m_adventureRadioButton;

	// Token: 0x040021F4 RID: 8692
	[CustomEditField(Sections = "General Store/Buttons")]
	public GameObject m_adventureOwnedCheckmark;

	// Token: 0x040021F5 RID: 8693
	[CustomEditField(Sections = "Sounds & Music", T = EditType.SOUND_PREFAB)]
	public string m_backgroundFlipSound;

	// Token: 0x040021F6 RID: 8694
	[CustomEditField(Sections = "Animation")]
	public float m_backgroundFlipAnimTime = 0.5f;

	// Token: 0x040021F7 RID: 8695
	private bool m_showPreviewCards;

	// Token: 0x040021F8 RID: 8696
	private Map<string, Actor> m_loadedPreviewCards = new Map<string, Actor>();

	// Token: 0x040021F9 RID: 8697
	private AdventureDbId m_selectedAdventureType;

	// Token: 0x040021FA RID: 8698
	private Map<int, StoreAdventureDef> m_storeAdvDefs = new Map<int, StoreAdventureDef>();

	// Token: 0x040021FB RID: 8699
	private int m_currentDisplay = -1;

	// Token: 0x040021FC RID: 8700
	private GeneralStoreAdventureContentDisplay m_adventureDisplay1;

	// Token: 0x040021FD RID: 8701
	private GeneralStoreAdventureContentDisplay m_adventureDisplay2;

	// Token: 0x040021FE RID: 8702
	public static readonly bool REQUIRE_REAL_MONEY_BUNDLE_OPTION = true;

	// Token: 0x0200044B RID: 1099
	// (Invoke) Token: 0x060036C4 RID: 14020
	public delegate void DelOnAdventurePreviewCardLoaded(Actor previewCard);
}
