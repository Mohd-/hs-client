using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000387 RID: 903
[CustomEditClass]
public class CollectionManagerDisplay : MonoBehaviour
{
	// Token: 0x06002E84 RID: 11908 RVA: 0x000E971C File Offset: 0x000E791C
	private void Start()
	{
		NetCache.Get().RegisterScreenCollectionManager(new NetCache.NetCacheCallback(this.OnNetCacheReady));
		CollectionManager.Get().RegisterCollectionNetHandlers();
		CollectionManager.Get().RegisterCollectionLoadedListener(new CollectionManager.DelOnCollectionLoaded(this.OnCollectionLoaded));
		CollectionManager.Get().RegisterCollectionChangedListener(new CollectionManager.DelOnCollectionChanged(this.OnCollectionChanged));
		CollectionManager.Get().RegisterDeckCreatedListener(new CollectionManager.DelOnDeckCreated(this.OnDeckCreated));
		CollectionManager.Get().RegisterDeckContentsListener(new CollectionManager.DelOnDeckContents(this.OnDeckContents));
		CollectionManager.Get().RegisterNewCardSeenListener(new CollectionManager.DelOnNewCardSeen(this.OnNewCardSeen));
		CollectionManager.Get().RegisterCardRewardInsertedListener(new CollectionManager.DelOnCardRewardInserted(this.OnCardRewardInserted));
		this.m_inputBlocker.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnInputBlockerRelease));
		this.m_search.RegisterActivatedListener(new CollectionSearch.ActivatedListener(this.OnSearchActivated));
		this.m_search.RegisterDeactivatedListener(new CollectionSearch.DeactivatedListener(this.OnSearchDeactivated));
		this.m_search.RegisterClearedListener(new CollectionSearch.ClearedListener(this.OnSearchCleared));
		this.m_pageManager.LoadPagingArrows();
		if (this.m_setFilterTrayContainer != null)
		{
			this.m_setFilterTray = this.m_setFilterTrayContainer.PrefabGameObject().GetComponentsInChildren<SetFilterTray>(true)[0];
		}
		bool @bool = Options.Get().GetBool(Option.SHOW_ADVANCED_COLLECTIONMANAGER, false);
		this.ShowAdvancedCollectionManager(@bool);
		if (!@bool)
		{
			Options.Get().RegisterChangedListener(Option.SHOW_ADVANCED_COLLECTIONMANAGER, new Options.ChangedCallback(this.OnShowAdvancedCMChanged));
		}
		CollectionManager.Get().RegisterAchievesCompletedListener(new CollectionManager.DelOnAchievesCompleted(this.OnCollectionAchievesCompleted));
		this.DoEnterCollectionManagerEvents();
		if (SceneMgr.Get().GetMode() != SceneMgr.Mode.TAVERN_BRAWL)
		{
			MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_CollectionManager);
		}
		if (CollectionManager.Get().ShouldShowWildToStandardTutorial(false))
		{
			UserAttentionManager.StartBlocking(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS);
		}
		base.StartCoroutine(this.WaitUntilReady());
	}

	// Token: 0x06002E85 RID: 11909 RVA: 0x000E98F4 File Offset: 0x000E7AF4
	private void Awake()
	{
		CollectionManagerDisplay.s_instance = this;
		if (GraphicsManager.Get().RenderQualityLevel != GraphicsQuality.Low && this.m_cover == null)
		{
			this.m_isCoverLoading = true;
			AssetLoader.Get().LoadGameObject("CollectionBookCover", new AssetLoader.GameObjectCallback(this.OnCoverLoaded), null, false);
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_deckTemplatePickerPhone = AssetLoader.Get().LoadGameObject("DeckTemplate_phone", true, false).GetComponent<DeckTemplatePicker>();
			SlidingTray component = this.m_deckTemplatePickerPhone.GetComponent<SlidingTray>();
			component.m_trayHiddenBone = this.m_deckTemplateHiddenBone.transform;
			component.m_trayShownBone = this.m_deckTemplateShownBone.transform;
		}
		this.LoadAllClassTextures();
		this.EnableInput(true);
		base.StartCoroutine(this.InitCollectionWhenReady());
		this.SetTavernBrawlTexturesIfNecessary();
	}

	// Token: 0x06002E86 RID: 11910 RVA: 0x000E99C8 File Offset: 0x000E7BC8
	private void OnDestroy()
	{
		if (this.m_deckTemplatePickerPhone != null)
		{
			Object.Destroy(this.m_deckTemplatePickerPhone.gameObject);
			this.m_deckTemplatePickerPhone = null;
		}
		CollectionManagerDisplay.s_instance = null;
		UserAttentionManager.StopBlocking(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS);
	}

	// Token: 0x06002E87 RID: 11911 RVA: 0x000E9A0C File Offset: 0x000E7C0C
	private void Update()
	{
		if (ApplicationMgr.IsInternal())
		{
			if (Input.GetKeyDown(49))
			{
				this.SetViewMode(CollectionManagerDisplay.ViewMode.HERO_SKINS, null);
			}
			else if (Input.GetKeyDown(50))
			{
				this.SetViewMode(CollectionManagerDisplay.ViewMode.CARDS, null);
			}
			else if (Input.GetKeyDown(51))
			{
				this.SetViewMode(CollectionManagerDisplay.ViewMode.CARD_BACKS, null);
			}
			else if (Input.GetKeyDown(52))
			{
				this.SetViewMode(CollectionManagerDisplay.ViewMode.DECK_TEMPLATE, null);
			}
			else if (Input.GetKeyDown(52))
			{
				this.OnCraftingModeButtonReleased(null);
			}
		}
	}

	// Token: 0x06002E88 RID: 11912 RVA: 0x000E9A9A File Offset: 0x000E7C9A
	public static CollectionManagerDisplay Get()
	{
		return CollectionManagerDisplay.s_instance;
	}

	// Token: 0x06002E89 RID: 11913 RVA: 0x000E9AA1 File Offset: 0x000E7CA1
	public Material GetGoldenCardNotOwnedMeshMaterial()
	{
		return this.m_goldenCardNotOwnedMeshMaterial;
	}

	// Token: 0x06002E8A RID: 11914 RVA: 0x000E9AA9 File Offset: 0x000E7CA9
	public Material GetCardNotOwnedMeshMaterial()
	{
		return this.m_cardNotOwnedMeshMaterial;
	}

	// Token: 0x06002E8B RID: 11915 RVA: 0x000E9AB1 File Offset: 0x000E7CB1
	public CollectionCardVisual GetCardVisualPrefab()
	{
		return this.m_cardVisualPrefab;
	}

	// Token: 0x06002E8C RID: 11916 RVA: 0x000E9AB9 File Offset: 0x000E7CB9
	public bool IsReady()
	{
		return this.m_isReady;
	}

	// Token: 0x06002E8D RID: 11917 RVA: 0x000E9AC4 File Offset: 0x000E7CC4
	public void Unload()
	{
		this.m_unloading = true;
		NotificationManager.Get().DestroyAllPopUps();
		this.UnloadAllClassTextures();
		CollectionDeckTray.Get().GetCardsContent().UnregisterCardTileRightClickedListener(new DeckTrayCardListContent.CardTileRightClicked(this.OnCardTileRightClicked));
		CollectionDeckTray.Get().Unload();
		CollectionInputMgr.Get().Unload();
		CollectionManager.Get().RemoveCollectionLoadedListener(new CollectionManager.DelOnCollectionLoaded(this.OnCollectionLoaded));
		CollectionManager.Get().RemoveCollectionChangedListener(new CollectionManager.DelOnCollectionChanged(this.OnCollectionChanged));
		CollectionManager.Get().RemoveDeckCreatedListener(new CollectionManager.DelOnDeckCreated(this.OnDeckCreated));
		CollectionManager.Get().RemoveDeckContentsListener(new CollectionManager.DelOnDeckContents(this.OnDeckContents));
		CollectionManager.Get().RemoveNewCardSeenListener(new CollectionManager.DelOnNewCardSeen(this.OnNewCardSeen));
		CollectionManager.Get().RemoveCardRewardInsertedListener(new CollectionManager.DelOnCardRewardInserted(this.OnCardRewardInserted));
		CollectionManager.Get().RemoveAchievesCompletedListener(new CollectionManager.DelOnAchievesCompleted(this.OnCollectionAchievesCompleted));
		CollectionManager.Get().RemoveCollectionNetHandlers();
		NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
		Options.Get().UnregisterChangedListener(Option.SHOW_ADVANCED_COLLECTIONMANAGER, new Options.ChangedCallback(this.OnShowAdvancedCMChanged));
		this.m_unloading = false;
	}

	// Token: 0x06002E8E RID: 11918 RVA: 0x000E9BF8 File Offset: 0x000E7DF8
	public void Exit()
	{
		this.EnableInput(false);
		NotificationManager.Get().DestroyAllPopUps();
		CollectionDeckTray.Get().Exit();
		SceneMgr.Mode mode = SceneMgr.Get().GetPrevMode();
		if (mode == SceneMgr.Mode.GAMEPLAY)
		{
			mode = SceneMgr.Mode.HUB;
		}
		SceneMgr.Get().SetNextMode(mode);
	}

	// Token: 0x06002E8F RID: 11919 RVA: 0x000E9C40 File Offset: 0x000E7E40
	public void GetClassTexture(TAG_CLASS classTag, CollectionManagerDisplay.DelTextureLoaded callback, object callbackData)
	{
		if (this.m_loadedClassTextures.ContainsKey(classTag))
		{
			callback(classTag, this.m_loadedClassTextures[classTag], callbackData);
			return;
		}
		CollectionManagerDisplay.TextureRequests textureRequests;
		if (this.m_requestedClassTextures.ContainsKey(classTag))
		{
			textureRequests = this.m_requestedClassTextures[classTag];
		}
		else
		{
			textureRequests = new CollectionManagerDisplay.TextureRequests();
			this.m_requestedClassTextures[classTag] = textureRequests;
		}
		textureRequests.m_requests.Add(new CollectionManagerDisplay.TextureRequests.Request
		{
			m_callback = callback,
			m_callbackData = callbackData
		});
	}

	// Token: 0x06002E90 RID: 11920 RVA: 0x000E9CC9 File Offset: 0x000E7EC9
	public void LoadCard(string cardID, DefLoader.LoadDefCallback<CardDef> callback, object callbackData, CardPortraitQuality quality = null)
	{
		DefLoader.Get().LoadCardDef(cardID, callback, callbackData, quality);
	}

	// Token: 0x06002E91 RID: 11921 RVA: 0x000E9CDC File Offset: 0x000E7EDC
	public void CollectionPageContentsChanged(List<CollectibleCard> cardsToDisplay, CollectionManagerDisplay.CollectionActorsReadyCallback callback, object callbackData)
	{
		if (this.m_displayRequestID == 2147483647)
		{
			this.m_displayRequestID = 0;
		}
		else
		{
			this.m_displayRequestID++;
		}
		bool flag = false;
		if (cardsToDisplay == null)
		{
			Log.Rachelle.Print("artStacksToDisplay is null!", new object[0]);
			flag = true;
		}
		else if (cardsToDisplay.Count == 0)
		{
			Log.Rachelle.Print("artStacksToDisplay has a count of 0!", new object[0]);
			flag = true;
		}
		if (flag)
		{
			List<Actor> actors = new List<Actor>();
			callback(actors, callbackData);
			return;
		}
		if (this.m_unloading)
		{
			return;
		}
		foreach (Actor actor in this.m_previousCardActors)
		{
			Object.Destroy(actor.gameObject);
		}
		this.m_previousCardActors.Clear();
		this.m_previousCardActors = this.m_cardActors;
		this.m_cardActors = new List<Actor>();
		long balance = NetCache.Get().GetNetObject<NetCache.NetCacheArcaneDustBalance>().Balance;
		foreach (CollectibleCard collectibleCard in cardsToDisplay)
		{
			EntityDef entityDef = DefLoader.Get().GetEntityDef(collectibleCard.CardId);
			CardDef cardDef = DefLoader.Get().GetCardDef(collectibleCard.CardId, null);
			string heroSkinOrHandActor = ActorNames.GetHeroSkinOrHandActor(entityDef.GetCardType(), collectibleCard.PremiumType);
			GameObject gameObject = AssetLoader.Get().LoadActor(heroSkinOrHandActor, false, false);
			if (gameObject == null)
			{
				Debug.LogError("Unable to load card actor.");
			}
			else
			{
				Actor component = gameObject.GetComponent<Actor>();
				if (component == null)
				{
					Debug.LogError("Actor object does not contain Actor component.");
				}
				else
				{
					component.SetEntityDef(entityDef);
					component.SetCardDef(cardDef);
					component.SetPremium(collectibleCard.PremiumType);
					if (collectibleCard.OwnedCount == 0)
					{
						if (collectibleCard.IsCraftable && balance >= (long)collectibleCard.CraftBuyCost)
						{
							component.GhostCardEffect(GhostCard.Type.MISSING);
						}
						else
						{
							component.MissingCardEffect();
						}
					}
					component.UpdateAllComponents();
					this.m_cardActors.Add(component);
				}
			}
		}
		if (callback != null)
		{
			callback(this.m_cardActors, callbackData);
		}
	}

	// Token: 0x06002E92 RID: 11922 RVA: 0x000E9F68 File Offset: 0x000E8168
	public void CollectionPageContentsChangedToCardBacks(int pageNumber, int numCardBacksPerPage, CollectionManagerDisplay.CollectionActorsReadyCallback callback, object callbackData, bool showAll)
	{
		CardBackManager cardBackManager = CardBackManager.Get();
		List<Actor> result = new List<Actor>();
		List<CardBackManager.OwnedCardBack> list = cardBackManager.GetOrderedEnabledCardBacks(!showAll);
		if (list.Count == 0)
		{
			if (callback != null)
			{
				callback(result, callbackData);
			}
			return;
		}
		int num = (pageNumber - 1) * numCardBacksPerPage;
		int num2 = Mathf.Min(list.Count - num, numCardBacksPerPage);
		list = list.GetRange(num, num2);
		int numCardBacksToLoad = list.Count;
		Action<int, CardBackManager.OwnedCardBack, Actor> cbLoadedCallback = delegate(int index, CardBackManager.OwnedCardBack cardBack, Actor actor)
		{
			if (actor != null)
			{
				result[index] = actor;
				actor.SetCardbackUpdateIgnore(true);
				if (!cardBack.m_owned)
				{
					actor.MissingCardEffect();
				}
				CollectionCardBack component = actor.GetComponent<CollectionCardBack>();
				if (component != null)
				{
					component.SetCardBackId(cardBack.m_cardBackId);
					component.SetCardBackName(CardBackManager.Get().GetCardBackName(cardBack.m_cardBackId));
				}
				else
				{
					Debug.LogError("CollectionCardBack component does not exist on actor!");
				}
			}
			numCardBacksToLoad--;
			if (numCardBacksToLoad == 0 && callback != null)
			{
				callback(result, callbackData);
			}
		};
		if (this.m_previousCardBackActors != null)
		{
			foreach (Actor actor2 in this.m_previousCardBackActors)
			{
				Object.Destroy(actor2.gameObject);
			}
			this.m_previousCardBackActors.Clear();
		}
		this.m_previousCardBackActors = this.m_cardBackActors;
		this.m_cardBackActors = new List<Actor>();
		for (int i = 0; i < list.Count; i++)
		{
			int currIndex = i;
			CardBackManager.OwnedCardBack cardBackLoad = list[i];
			int cardBackId = cardBackLoad.m_cardBackId;
			result.Add(null);
			if (!cardBackManager.LoadCardBackByIndex(cardBackId, delegate(CardBackManager.LoadCardBackData cardBackData)
			{
				GameObject gameObject = cardBackData.m_GameObject;
				gameObject.transform.parent = this.transform;
				gameObject.name = "CARD_BACK_" + cardBackData.m_CardBackIndex;
				Actor component = gameObject.GetComponent<Actor>();
				if (component == null)
				{
					Object.Destroy(gameObject);
				}
				else
				{
					GameObject cardMesh = component.m_cardMesh;
					component.SetCardbackUpdateIgnore(true);
					component.SetUnlit();
					if (cardMesh != null)
					{
						Material material = cardMesh.GetComponent<Renderer>().material;
						if (material.HasProperty("_SpecularIntensity"))
						{
							material.SetFloat("_SpecularIntensity", 0f);
						}
					}
					this.m_cardBackActors.Add(component);
				}
				cbLoadedCallback.Invoke(currIndex, cardBackLoad, component);
			}, "Collection_Card_Back"))
			{
				cbLoadedCallback.Invoke(currIndex, cardBackLoad, null);
			}
		}
	}

	// Token: 0x06002E93 RID: 11923 RVA: 0x000EA14C File Offset: 0x000E834C
	public void RequestContentsToShowDeck(long deckID)
	{
		this.m_showDeckContentsRequest = deckID;
		CollectionManager.Get().RequestDeckContents(this.m_showDeckContentsRequest);
	}

	// Token: 0x06002E94 RID: 11924 RVA: 0x000EA165 File Offset: 0x000E8365
	public CollectionPageLayoutSettings.Variables GetCurrentPageLayoutSettings()
	{
		return this.GetPageLayoutSettings(this.m_currentViewMode);
	}

	// Token: 0x06002E95 RID: 11925 RVA: 0x000EA173 File Offset: 0x000E8373
	public CollectionPageLayoutSettings.Variables GetPageLayoutSettings(CollectionManagerDisplay.ViewMode viewMode)
	{
		return this.m_pageLayoutSettings.GetVariables(viewMode);
	}

	// Token: 0x06002E96 RID: 11926 RVA: 0x000EA184 File Offset: 0x000E8384
	public void ShowPhoneDeckTemplateTray()
	{
		this.m_pageManager.UpdateDeckTemplate(this.m_deckTemplatePickerPhone);
		SlidingTray component = this.m_deckTemplatePickerPhone.GetComponent<SlidingTray>();
		component.RegisterTrayToggleListener(new SlidingTray.TrayToggledListener(this.m_deckTemplatePickerPhone.OnTrayToggled));
		component.ShowTray();
	}

	// Token: 0x06002E97 RID: 11927 RVA: 0x000EA1CB File Offset: 0x000E83CB
	public DeckTemplatePicker GetPhoneDeckTemplateTray()
	{
		return this.m_deckTemplatePickerPhone;
	}

	// Token: 0x06002E98 RID: 11928 RVA: 0x000EA1D4 File Offset: 0x000E83D4
	public void SetViewMode(CollectionManagerDisplay.ViewMode mode, bool triggerResponse, CollectionManagerDisplay.ViewModeData userdata = null)
	{
		if (this.m_currentViewMode == mode)
		{
			return;
		}
		if ((mode == CollectionManagerDisplay.ViewMode.HERO_SKINS || mode == CollectionManagerDisplay.ViewMode.CARD_BACKS) && CollectionDeckTray.Get().IsUpdatingTrayMode())
		{
			return;
		}
		if (mode == CollectionManagerDisplay.ViewMode.DECK_TEMPLATE)
		{
			if (!CollectionManager.Get().IsInEditMode() || SceneMgr.Get().GetMode() == SceneMgr.Mode.TAVERN_BRAWL)
			{
				return;
			}
			if (UniversalInputManager.UsePhoneUI)
			{
				this.ShowPhoneDeckTemplateTray();
			}
		}
		CollectionManagerDisplay.ViewMode currentViewMode = this.m_currentViewMode;
		this.m_currentViewMode = mode;
		this.OnSwitchViewModeResponse(triggerResponse, currentViewMode, mode, userdata);
	}

	// Token: 0x06002E99 RID: 11929 RVA: 0x000EA261 File Offset: 0x000E8461
	public void SetViewMode(CollectionManagerDisplay.ViewMode mode, CollectionManagerDisplay.ViewModeData userdata = null)
	{
		this.SetViewMode(mode, true, userdata);
	}

	// Token: 0x06002E9A RID: 11930 RVA: 0x000EA26C File Offset: 0x000E846C
	public CollectionManagerDisplay.ViewMode GetViewMode()
	{
		return this.m_currentViewMode;
	}

	// Token: 0x06002E9B RID: 11931 RVA: 0x000EA274 File Offset: 0x000E8474
	public bool SetFilterTrayInitialized()
	{
		return this.m_setFilterTrayInitialized;
	}

	// Token: 0x06002E9C RID: 11932 RVA: 0x000EA27C File Offset: 0x000E847C
	public void OnStartEditingDeck(bool isWild)
	{
		this.UpdateSetFilters(isWild, true, false);
	}

	// Token: 0x06002E9D RID: 11933 RVA: 0x000EA288 File Offset: 0x000E8488
	public void OnDoneEditingDeck()
	{
		this.UpdateSetFilters(CollectionManager.Get().ShouldAccountSeeStandardWild(), false, this.m_craftingTray != null && this.m_craftingTray.IsShown());
		if (this.m_currentViewMode == CollectionManagerDisplay.ViewMode.DECK_TEMPLATE)
		{
			this.SetViewMode(CollectionManagerDisplay.ViewMode.CARDS, false, null);
		}
		if (SceneMgr.Get().GetMode() != SceneMgr.Mode.TAVERN_BRAWL)
		{
			this.m_pageManager.SetDeckRuleset(null, false);
		}
		this.m_pageManager.OnDoneEditingDeck();
	}

	// Token: 0x06002E9E RID: 11934 RVA: 0x000EA304 File Offset: 0x000E8504
	public void FilterByManaCost(int cost)
	{
		bool active = cost != ManaFilterTab.ALL_TAB_IDX;
		string value = (cost >= 7) ? (cost.ToString() + "+") : cost.ToString();
		this.NotifyFilterUpdate(this.m_manaFilterListeners, active, value);
		this.m_pageManager.FilterByManaCost(cost);
	}

	// Token: 0x06002E9F RID: 11935 RVA: 0x000EA35C File Offset: 0x000E855C
	public void ShowOnlyCardsIOwn()
	{
		this.ShowOnlyCardsIOwn(null);
	}

	// Token: 0x06002EA0 RID: 11936 RVA: 0x000EA365 File Offset: 0x000E8565
	public void ShowOnlyCardsIOwn(object obj)
	{
		this.m_pageManager.ShowOnlyCardsIOwn();
	}

	// Token: 0x06002EA1 RID: 11937 RVA: 0x000EA374 File Offset: 0x000E8574
	public void HideAllTips()
	{
		if (this.m_innkeeperLClickReminder != null)
		{
			NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_innkeeperLClickReminder);
		}
		if (this.m_deckHelpPopup != null)
		{
			NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_deckHelpPopup);
		}
		if (this.m_convertTutorialPopup != null)
		{
			NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_convertTutorialPopup);
		}
		if (this.m_createDeckNotification != null)
		{
			NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_createDeckNotification);
		}
	}

	// Token: 0x06002EA2 RID: 11938 RVA: 0x000EA405 File Offset: 0x000E8605
	public void HideDeckHelpPopup()
	{
		if (this.m_deckHelpPopup != null)
		{
			NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_deckHelpPopup);
		}
	}

	// Token: 0x06002EA3 RID: 11939 RVA: 0x000EA428 File Offset: 0x000E8628
	public void ShowInnkeeeprLClickHelp(bool isHero)
	{
		if (CollectionDeckTray.Get().IsShowingDeckContents())
		{
			return;
		}
		if (isHero)
		{
			this.m_innkeeperLClickReminder = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_CM_LCLICK_HERO"), string.Empty, 3f, null);
		}
		else
		{
			this.m_innkeeperLClickReminder = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_CM_LCLICK"), string.Empty, 3f, null);
		}
	}

	// Token: 0x06002EA4 RID: 11940 RVA: 0x000EA49C File Offset: 0x000E869C
	public void ShowPremiumCardsNotOwned(bool show)
	{
		this.m_pageManager.ShowCardsNotOwned(show);
	}

	// Token: 0x06002EA5 RID: 11941 RVA: 0x000EA4AA File Offset: 0x000E86AA
	public void ShowPremiumCardsOnly()
	{
		this.m_pageManager.ShowPremiumCardsOnly();
	}

	// Token: 0x06002EA6 RID: 11942 RVA: 0x000EA4B8 File Offset: 0x000E86B8
	public void SetFilterCallback(object data, bool isWild)
	{
		if (isWild)
		{
			if (!AchieveManager.Get().HasUnlockedFeature(Achievement.UnlockableFeature.VANILLA_HEROES))
			{
				Debug.LogError("User selected a Wild set filter, without having unlocked all 9 heroes!");
				return;
			}
			if (!CollectionManager.Get().AccountEverHadWildCards())
			{
				AlertPopup.PopupInfo info = new AlertPopup.PopupInfo
				{
					m_headerText = GameStrings.Get("GLUE_COLLECTION_SET_FILTER_WILD_SET_HEADER"),
					m_text = GameStrings.Get("GLUE_COLLECTION_SET_FILTER_WILD_SET_BODY"),
					m_cancelText = GameStrings.Get("GLOBAL_CANCEL"),
					m_confirmText = GameStrings.Get("GLOBAL_BUTTON_YES"),
					m_showAlertIcon = true,
					m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
					m_responseCallback = delegate(AlertPopup.Response response, object userData)
					{
						if (response == AlertPopup.Response.CONFIRM)
						{
							this.ShowSet(data);
						}
						else
						{
							this.m_setFilterTray.SelectPreviouslySelectedItem();
						}
					}
				};
				DialogManager.Get().ShowPopup(info);
				return;
			}
		}
		this.ShowSet(data);
	}

	// Token: 0x06002EA7 RID: 11943 RVA: 0x000EA58C File Offset: 0x000E878C
	public void ShowSet(object data)
	{
		List<TAG_CARD_SET> cardSets = (List<TAG_CARD_SET>)data;
		this.m_pageManager.FilterByCardSets(cardSets);
		this.NotifyFilterUpdate(this.m_setFilterListeners, data != null, null);
	}

	// Token: 0x06002EA8 RID: 11944 RVA: 0x000EA5C0 File Offset: 0x000E87C0
	public void GoToPageWithCard(string cardID, TAG_PREMIUM premium)
	{
		if (this.m_currentViewMode == CollectionManagerDisplay.ViewMode.DECK_TEMPLATE)
		{
			this.SetViewMode(CollectionManagerDisplay.ViewMode.CARDS, new CollectionManagerDisplay.ViewModeData
			{
				m_setPageByCard = cardID,
				m_setPageByPremium = premium
			});
		}
		else
		{
			this.m_pageManager.JumpToPageWithCard(cardID, premium);
		}
	}

	// Token: 0x06002EA9 RID: 11945 RVA: 0x000EA608 File Offset: 0x000E8808
	public void UpdateCurrentPageCardLocks(bool playSound = false)
	{
		this.m_pageManager.UpdateCurrentPageCardLocks(playSound);
	}

	// Token: 0x06002EAA RID: 11946 RVA: 0x000EA616 File Offset: 0x000E8816
	public HeroPickerDisplay GetHeroPickerDisplay()
	{
		return this.m_heroPickerDisplay;
	}

	// Token: 0x06002EAB RID: 11947 RVA: 0x000EA620 File Offset: 0x000E8820
	public void EnterSelectNewDeckHeroMode()
	{
		if (this.m_selectingNewDeckHero)
		{
			return;
		}
		this.EnableInput(false);
		this.m_selectingNewDeckHero = true;
		this.m_heroPickerDisplay = AssetLoader.Get().LoadActor("HeroPicker", false, false).GetComponent<HeroPickerDisplay>();
		NotificationManager.Get().DestroyAllPopUps();
		if (SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER)
		{
			this.m_pageManager.HideNonDeckTemplateTabs(true, false);
		}
	}

	// Token: 0x06002EAC RID: 11948 RVA: 0x000EA68B File Offset: 0x000E888B
	public void ExitSelectNewDeckHeroMode()
	{
		this.m_selectingNewDeckHero = false;
	}

	// Token: 0x06002EAD RID: 11949 RVA: 0x000EA694 File Offset: 0x000E8894
	public void CancelSelectNewDeckHeroMode()
	{
		this.EnableInput(true);
		this.m_pageManager.HideNonDeckTemplateTabs(false, true);
		this.ExitSelectNewDeckHeroMode();
	}

	// Token: 0x06002EAE RID: 11950 RVA: 0x000EA6B4 File Offset: 0x000E88B4
	public bool ShouldShowNewCardGlow(string cardID, TAG_PREMIUM premium)
	{
		CollectibleCard card = CollectionManager.Get().GetCard(cardID, premium);
		return card != null && card.IsNewCard;
	}

	// Token: 0x06002EAF RID: 11951 RVA: 0x000EA6E0 File Offset: 0x000E88E0
	public bool CanViewHeroSkins()
	{
		CollectionManager collectionManager = CollectionManager.Get();
		CollectionDeck taggedDeck = CollectionManager.Get().GetTaggedDeck(CollectionManager.DeckTag.Editing);
		return taggedDeck == null || collectionManager.GetBestHeroesIOwn(taggedDeck.GetClass()).Count > 1;
	}

	// Token: 0x06002EB0 RID: 11952 RVA: 0x000EA71B File Offset: 0x000E891B
	public bool CanViewCardBacks()
	{
		return CardBackManager.Get().GetCardBacksOwned().Count > 1;
	}

	// Token: 0x06002EB1 RID: 11953 RVA: 0x000EA72F File Offset: 0x000E892F
	public void RegisterSwitchViewModeListener(CollectionManagerDisplay.OnSwitchViewMode listener)
	{
		this.m_switchViewModeListeners.Add(listener);
	}

	// Token: 0x06002EB2 RID: 11954 RVA: 0x000EA73D File Offset: 0x000E893D
	public void RemoveSwitchViewModeListener(CollectionManagerDisplay.OnSwitchViewMode listener)
	{
		this.m_switchViewModeListeners.Remove(listener);
	}

	// Token: 0x06002EB3 RID: 11955 RVA: 0x000EA74C File Offset: 0x000E894C
	public void RegisterManaFilterListener(CollectionManagerDisplay.FilterStateListener listener)
	{
		this.m_manaFilterListeners.Add(listener);
	}

	// Token: 0x06002EB4 RID: 11956 RVA: 0x000EA75A File Offset: 0x000E895A
	public void UnregisterManaFilterListener(CollectionManagerDisplay.FilterStateListener listener)
	{
		this.m_manaFilterListeners.Remove(listener);
	}

	// Token: 0x06002EB5 RID: 11957 RVA: 0x000EA769 File Offset: 0x000E8969
	public void RegisterSearchFilterListener(CollectionManagerDisplay.FilterStateListener listener)
	{
		this.m_searchFilterListeners.Add(listener);
	}

	// Token: 0x06002EB6 RID: 11958 RVA: 0x000EA777 File Offset: 0x000E8977
	public void UnregisterSearchFilterListener(CollectionManagerDisplay.FilterStateListener listener)
	{
		this.m_searchFilterListeners.Remove(listener);
	}

	// Token: 0x06002EB7 RID: 11959 RVA: 0x000EA786 File Offset: 0x000E8986
	public void RegisterSetFilterListener(CollectionManagerDisplay.FilterStateListener listener)
	{
		this.m_setFilterListeners.Add(listener);
	}

	// Token: 0x06002EB8 RID: 11960 RVA: 0x000EA794 File Offset: 0x000E8994
	public void UnregisterSetFilterListener(CollectionManagerDisplay.FilterStateListener listener)
	{
		this.m_setFilterListeners.Remove(listener);
	}

	// Token: 0x06002EB9 RID: 11961 RVA: 0x000EA7A3 File Offset: 0x000E89A3
	public void ResetFilters(bool updateVisuals = true)
	{
		this.m_search.ClearFilter(updateVisuals);
		this.m_manaTabManager.ClearFilter();
		if (this.m_setFilterTray != null)
		{
			this.m_setFilterTray.ClearFilter();
		}
	}

	// Token: 0x06002EBA RID: 11962 RVA: 0x000EA7D8 File Offset: 0x000E89D8
	public void ShowAppropriateSetFilters()
	{
		bool flag = CollectionManager.Get().IsInEditMode();
		bool showWild;
		if (flag)
		{
			CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
			showWild = (editedDeck != null && editedDeck.IsWild);
		}
		else
		{
			showWild = CollectionManager.Get().ShouldAccountSeeStandardWild();
		}
		this.UpdateSetFilters(showWild, flag, this.m_craftingTray != null && this.m_craftingTray.IsShown());
	}

	// Token: 0x06002EBB RID: 11963 RVA: 0x000EA848 File Offset: 0x000E8A48
	public void UpdateSetFilters(bool showWild, bool editingDeck, bool showUnownedSets = false)
	{
		this.m_setFilterTray.UpdateSetFilters(showWild, editingDeck, showUnownedSets);
	}

	// Token: 0x06002EBC RID: 11964 RVA: 0x000EA858 File Offset: 0x000E8A58
	private void OnCollectionLoaded()
	{
		this.m_pageManager.OnCollectionLoaded();
	}

	// Token: 0x06002EBD RID: 11965 RVA: 0x000EA865 File Offset: 0x000E8A65
	private void OnDeckContents(long deckID)
	{
		if (deckID != this.m_showDeckContentsRequest)
		{
			return;
		}
		this.m_showDeckContentsRequest = 0L;
		this.ShowDeck(deckID, false);
	}

	// Token: 0x06002EBE RID: 11966 RVA: 0x000EA884 File Offset: 0x000E8A84
	private void OnDeckCreated(long deckID)
	{
		this.ShowDeck(deckID, true);
	}

	// Token: 0x06002EBF RID: 11967 RVA: 0x000EA88E File Offset: 0x000E8A8E
	private void OnNewCardSeen(string cardID, TAG_PREMIUM premium)
	{
		this.m_pageManager.UpdateClassTabNewCardCounts();
	}

	// Token: 0x06002EC0 RID: 11968 RVA: 0x000EA89B File Offset: 0x000E8A9B
	private void OnCardRewardInserted(string cardID, TAG_PREMIUM premium)
	{
		this.m_pageManager.RefreshCurrentPageContents();
	}

	// Token: 0x06002EC1 RID: 11969 RVA: 0x000EA8A8 File Offset: 0x000E8AA8
	private void OnCollectionChanged()
	{
		if (!this.m_pageManager.IsShowingMassDisenchant())
		{
			this.m_pageManager.NotifyOfCollectionChanged();
		}
	}

	// Token: 0x06002EC2 RID: 11970 RVA: 0x000EA8C8 File Offset: 0x000E8AC8
	private void OnCollectionAchievesCompleted(List<Achievement> achievements)
	{
		Achievement achieve;
		foreach (Achievement achieve2 in achievements)
		{
			achieve = achieve2;
			Achievement achievement = this.m_completeAchievesToDisplay.Find((Achievement obj) => obj.ID == achieve.ID);
			if (achievement == null)
			{
				this.m_completeAchievesToDisplay.Add(achieve);
			}
		}
		if (QuestToast.GetCurrentToast() == null)
		{
			this.ShowCompleteAchieve(null);
		}
	}

	// Token: 0x06002EC3 RID: 11971 RVA: 0x000EA96C File Offset: 0x000E8B6C
	private void NotifyFilterUpdate(List<CollectionManagerDisplay.FilterStateListener> listeners, bool active, object value)
	{
		foreach (CollectionManagerDisplay.FilterStateListener filterStateListener in listeners)
		{
			filterStateListener(active, value);
		}
	}

	// Token: 0x06002EC4 RID: 11972 RVA: 0x000EA9C4 File Offset: 0x000E8BC4
	private IEnumerator WaitUntilReady()
	{
		while (!this.m_netCacheReady)
		{
			yield return 0;
		}
		CollectionDeckTray deckTray = CollectionDeckTray.Get();
		deckTray.Initialize();
		deckTray.RegisterModeSwitchedListener(delegate
		{
			this.UpdateCurrentPageCardLocks(false);
		});
		deckTray.GetCardsContent().RegisterCardTileRightClickedListener(new DeckTrayCardListContent.CardTileRightClicked(this.OnCardTileRightClicked));
		this.m_isReady = true;
		yield break;
	}

	// Token: 0x06002EC5 RID: 11973 RVA: 0x000EA9E0 File Offset: 0x000E8BE0
	private IEnumerator InitCollectionWhenReady()
	{
		while (!this.m_pageManager.IsFullyLoaded())
		{
			yield return null;
		}
		this.m_pageManager.LoadMassDisenchantScreen();
		this.m_pageManager.OnCollectionLoaded();
		yield break;
	}

	// Token: 0x06002EC6 RID: 11974 RVA: 0x000EA9FC File Offset: 0x000E8BFC
	private void OnNetCacheReady()
	{
		NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
		NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
		if (netObject.Collection.Manager)
		{
			this.m_netCacheReady = true;
			return;
		}
		if (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB))
		{
			return;
		}
		SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
		Error.AddWarningLoc("GLOBAL_FEATURE_DISABLED_TITLE", "GLOBAL_FEATURE_DISABLED_MESSAGE_COLLECTION", new object[0]);
	}

	// Token: 0x06002EC7 RID: 11975 RVA: 0x000EAA74 File Offset: 0x000E8C74
	private void OnShowAdvancedCMChanged(Option option, object prevValue, bool existed, object userData)
	{
		bool @bool = Options.Get().GetBool(Option.SHOW_ADVANCED_COLLECTIONMANAGER, false);
		if (@bool)
		{
			Options.Get().UnregisterChangedListener(Option.SHOW_ADVANCED_COLLECTIONMANAGER, new Options.ChangedCallback(this.OnShowAdvancedCMChanged));
		}
		this.ShowAdvancedCollectionManager(@bool);
		this.m_manaTabManager.ActivateTabs(true);
	}

	// Token: 0x06002EC8 RID: 11976 RVA: 0x000EAAC4 File Offset: 0x000E8CC4
	private void OnCardTileRightClicked(DeckTrayDeckTileVisual cardTile)
	{
		if (this.GetViewMode() == CollectionManagerDisplay.ViewMode.DECK_TEMPLATE)
		{
			return;
		}
		if (!cardTile.GetSlot().Owned)
		{
			CraftingManager.Get().EnterCraftMode(cardTile.GetActor());
		}
		this.GoToPageWithCard(cardTile.GetCardID(), cardTile.GetPremium());
	}

	// Token: 0x06002EC9 RID: 11977 RVA: 0x000EAB10 File Offset: 0x000E8D10
	private void LoadAllClassTextures()
	{
		foreach (object obj in Enum.GetValues(typeof(TAG_CLASS)))
		{
			TAG_CLASS tag_CLASS = (TAG_CLASS)((int)obj);
			AssetLoader.Get().LoadTexture(CollectionManagerDisplay.GetClassTextureName(tag_CLASS), new AssetLoader.ObjectCallback(this.OnClassTextureLoaded), tag_CLASS, false);
		}
	}

	// Token: 0x06002ECA RID: 11978 RVA: 0x000EAB9C File Offset: 0x000E8D9C
	private void UnloadAllClassTextures()
	{
		if (this.m_loadedClassTextures.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (TAG_CLASS classTag in this.m_loadedClassTextures.Keys)
		{
			list.Add(CollectionManagerDisplay.GetClassTextureName(classTag));
		}
		AssetCache.ClearTextures(list);
		this.m_loadedClassTextures.Clear();
	}

	// Token: 0x06002ECB RID: 11979 RVA: 0x000EAC28 File Offset: 0x000E8E28
	public static string GetClassTextureName(TAG_CLASS classTag)
	{
		switch (classTag)
		{
		case TAG_CLASS.DEATHKNIGHT:
			return "DeathKnight";
		case TAG_CLASS.DRUID:
			return "Druid";
		case TAG_CLASS.HUNTER:
			return "Hunter";
		case TAG_CLASS.MAGE:
			return "Mage";
		case TAG_CLASS.PALADIN:
			return "Paladin";
		case TAG_CLASS.PRIEST:
			return "Priest";
		case TAG_CLASS.ROGUE:
			return "Rogue";
		case TAG_CLASS.SHAMAN:
			return "Shaman";
		case TAG_CLASS.WARLOCK:
			return "Warlock";
		case TAG_CLASS.WARRIOR:
			return "Warrior";
		default:
			return string.Empty;
		}
	}

	// Token: 0x06002ECC RID: 11980 RVA: 0x000EACB0 File Offset: 0x000E8EB0
	private void SetTavernBrawlTexturesIfNecessary()
	{
		if (SceneMgr.Get().GetMode() == SceneMgr.Mode.TAVERN_BRAWL)
		{
			if (this.m_bookBack != null && !string.IsNullOrEmpty(this.m_corkBackTexture) && this.m_tavernBrawlBookBackMesh != null)
			{
				this.m_bookBack.GetComponent<MeshFilter>().mesh = this.m_tavernBrawlBookBackMesh;
				string name = FileUtils.GameAssetPathToName(this.m_corkBackTexture);
				Texture texture = AssetLoader.Get().LoadTexture(name, false);
				this.m_bookBack.GetComponent<MeshRenderer>().material.SetTexture(0, texture);
			}
			if (!UniversalInputManager.UsePhoneUI)
			{
				foreach (GameObject gameObject in this.m_tavernBrawlObjectsToSwap)
				{
					gameObject.GetComponent<Renderer>().material = this.m_tavernBrawlElements;
				}
			}
		}
	}

	// Token: 0x06002ECD RID: 11981 RVA: 0x000EADAC File Offset: 0x000E8FAC
	private void OnClassTextureLoaded(string assetName, Object asset, object callbackData)
	{
		if (asset == null)
		{
			Debug.LogWarning(string.Format("CollectionManagerDisplay.OnClassTextureLoaded(): asset for {0} is null!", assetName));
			return;
		}
		TAG_CLASS tag_CLASS = (TAG_CLASS)((int)callbackData);
		Texture texture = asset as Texture;
		if (texture == null)
		{
			Debug.LogWarning(string.Format("CollectionManagerDisplay.OnClassTextureLoaded(): classTexture for {0} is null (asset is not a texture)!", assetName));
			return;
		}
		if (this.m_loadedClassTextures.ContainsKey(tag_CLASS))
		{
			Debug.LogWarning(string.Format("CollectionManagerDisplay.OnClassTextureLoaded(): classTexture for {0} ({1}) has already been loaded!", tag_CLASS, assetName));
			return;
		}
		this.m_loadedClassTextures[tag_CLASS] = texture;
		if (!this.m_requestedClassTextures.ContainsKey(tag_CLASS))
		{
			return;
		}
		CollectionManagerDisplay.TextureRequests textureRequests = this.m_requestedClassTextures[tag_CLASS];
		this.m_requestedClassTextures.Remove(tag_CLASS);
		foreach (CollectionManagerDisplay.TextureRequests.Request request in textureRequests.m_requests)
		{
			request.m_callback(tag_CLASS, texture, request.m_callbackData);
		}
	}

	// Token: 0x06002ECE RID: 11982 RVA: 0x000EAEC0 File Offset: 0x000E90C0
	public void EnableInput(bool enable)
	{
		if (!enable)
		{
			this.m_inputBlockers++;
		}
		else if (this.m_inputBlockers > 0)
		{
			this.m_inputBlockers--;
		}
		bool active = this.m_inputBlockers > 0;
		this.m_inputBlocker.gameObject.SetActive(active);
	}

	// Token: 0x06002ECF RID: 11983 RVA: 0x000EAF1C File Offset: 0x000E911C
	private void ShowDeck(long deckID, bool isNewDeck)
	{
		if (CollectionManager.Get().GetDeck(deckID) == null)
		{
			return;
		}
		bool flag = isNewDeck && SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER;
		if (!flag)
		{
			this.m_pageManager.HideNonDeckTemplateTabs(false, false);
		}
		CollectionManagerDisplay.ViewMode? viewMode = default(CollectionManagerDisplay.ViewMode?);
		if (flag)
		{
			viewMode = new CollectionManagerDisplay.ViewMode?(CollectionManagerDisplay.ViewMode.DECK_TEMPLATE);
		}
		else if ((this.m_currentViewMode == CollectionManagerDisplay.ViewMode.HERO_SKINS && !this.CanViewHeroSkins()) || (this.m_currentViewMode == CollectionManagerDisplay.ViewMode.CARD_BACKS && !this.CanViewCardBacks()))
		{
			viewMode = new CollectionManagerDisplay.ViewMode?(CollectionManagerDisplay.ViewMode.CARDS);
		}
		CollectionDeckTray.Get().ShowDeck((viewMode != null) ? viewMode.Value : this.GetViewMode(), deckID, isNewDeck);
		TAG_CLASS deckHeroClass = this.GetDeckHeroClass(deckID);
		this.m_pageManager.SetClassFilter(deckHeroClass, isNewDeck, viewMode == null, null, null);
		this.m_pageManager.UpdateCraftingModeButtonDustBottleVisibility();
		if (viewMode != null)
		{
			this.SetViewMode(viewMode.Value, null);
		}
		NotificationManager.Get().DestroyNotification(this.m_createDeckNotification, 0.25f);
		base.StartCoroutine(this.ShowDeckTemplateTipsIfNeeded());
	}

	// Token: 0x06002ED0 RID: 11984 RVA: 0x000EB050 File Offset: 0x000E9250
	private TAG_CLASS GetDeckHeroClass(long deckID)
	{
		CollectionDeck deck = CollectionManager.Get().GetDeck(deckID);
		if (deck == null)
		{
			Log.Derek.Print(string.Format("CollectionManagerDisplay no deck with ID {0}!", deckID), new object[0]);
			return TAG_CLASS.INVALID;
		}
		EntityDef entityDef = DefLoader.Get().GetEntityDef(deck.HeroCardID);
		if (entityDef == null)
		{
			Log.Derek.Print(string.Format("CollectionManagerDisplay: CollectionManager doesn't have an entity def for {0}!", deck.HeroCardID), new object[0]);
			return TAG_CLASS.INVALID;
		}
		return entityDef.GetClass();
	}

	// Token: 0x06002ED1 RID: 11985 RVA: 0x000EB0D0 File Offset: 0x000E92D0
	private IEnumerator DoBookOpeningAnimations()
	{
		while (this.m_isCoverLoading)
		{
			yield return null;
		}
		if (this.m_cover != null)
		{
			this.m_cover.Open(new CollectionCoverDisplay.DelOnOpened(this.OnCoverOpened));
		}
		else
		{
			this.OnCoverOpened();
		}
		this.m_manaTabManager.ActivateTabs(true);
		yield break;
	}

	// Token: 0x06002ED2 RID: 11986 RVA: 0x000EB0EC File Offset: 0x000E92EC
	private IEnumerator SetBookToOpen()
	{
		while (this.m_isCoverLoading)
		{
			yield return null;
		}
		if (this.m_cover != null)
		{
			this.m_cover.SetOpenState();
		}
		this.m_manaTabManager.ActivateTabs(true);
		yield break;
	}

	// Token: 0x06002ED3 RID: 11987 RVA: 0x000EB108 File Offset: 0x000E9308
	private void DoBookClosingAnimations()
	{
		if (this.m_cover != null)
		{
			this.m_cover.Close();
		}
		this.m_manaTabManager.ActivateTabs(false);
	}

	// Token: 0x06002ED4 RID: 11988 RVA: 0x000EB140 File Offset: 0x000E9340
	private void ShowAdvancedCollectionManager(bool show)
	{
		show |= UniversalInputManager.UsePhoneUI;
		this.m_search.gameObject.SetActive(show);
		this.m_manaTabManager.gameObject.SetActive(show);
		if (this.m_setFilterTray != null)
		{
			bool buttonShown = show && !UniversalInputManager.UsePhoneUI;
			this.m_setFilterTray.SetButtonShown(buttonShown);
		}
		if (this.m_craftingTray == null)
		{
			AssetLoader.Get().LoadGameObject((!UniversalInputManager.UsePhoneUI) ? "CraftingTray" : "CraftingTray_phone", new AssetLoader.GameObjectCallback(this.OnCraftingTrayLoaded), null, false);
		}
		this.m_craftingModeButton.gameObject.SetActive(true);
		this.m_craftingModeButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCraftingModeButtonReleased));
		if (this.m_setFilterTray != null && show && !this.m_setFilterTrayInitialized)
		{
			this.m_setFilterTray.AddItem(GameStrings.Get("GLUE_COLLECTION_ALL_STANDARD_CARDS"), new Vector2?(this.m_allSetsIconOffset), new SetFilterItem.ItemSelectedCallback(this.SetFilterCallback), new List<TAG_CARD_SET>(GameUtils.GetStandardSets()), false, true);
			this.m_setFilterTray.AddItem(GameStrings.Get("GLUE_COLLECTION_ALL_CARDS"), new Vector2?(this.m_wildSetsIconOffset), new SetFilterItem.ItemSelectedCallback(this.SetFilterCallback), null, true, false);
			this.m_setFilterTray.AddHeader(GameStrings.Get("GLUE_COLLECTION_STANDARD_SETS"), false);
			this.AddSetFilters(false);
			this.m_setFilterTray.AddHeader(GameStrings.Get("GLUE_COLLECTION_WILD_SETS"), true);
			this.AddSetFilters(true);
			this.AddSetFilter(TAG_CARD_SET.REWARD);
			if (ApplicationMgr.IsInternal())
			{
				this.AddSetFilter(TAG_CARD_SET.SLUSH);
			}
			this.m_setFilterTray.SelectFirstItem();
			this.m_setFilterTrayInitialized = true;
		}
		this.ShowAppropriateSetFilters();
		if (!show)
		{
			return;
		}
		this.m_manaTabManager.SetUpTabs();
	}

	// Token: 0x06002ED5 RID: 11989 RVA: 0x000EB32C File Offset: 0x000E952C
	private void AddSetFilters(bool isWild)
	{
		List<TAG_CARD_SET> displayableCardSets = CollectionManager.Get().GetDisplayableCardSets();
		for (int i = displayableCardSets.Count - 1; i >= 0; i--)
		{
			TAG_CARD_SET tag_CARD_SET = displayableCardSets[i];
			bool flag = GameUtils.IsSetRotated(tag_CARD_SET);
			if (flag == isWild && tag_CARD_SET != TAG_CARD_SET.REWARD && tag_CARD_SET != TAG_CARD_SET.SLUSH)
			{
				this.AddSetFilter(tag_CARD_SET);
			}
		}
	}

	// Token: 0x06002ED6 RID: 11990 RVA: 0x000EB390 File Offset: 0x000E9590
	private void AddSetFilter(TAG_CARD_SET cardSet)
	{
		List<TAG_CARD_SET> list = new List<TAG_CARD_SET>();
		CollectionManagerDisplay.CardSetIconMatOffset cardSetIconMatOffset = this.m_manaFilterCardSetIcons.Find((CollectionManagerDisplay.CardSetIconMatOffset t) => t.m_cardSet == cardSet);
		Vector2? iconOffset = default(Vector2?);
		if (cardSetIconMatOffset != null)
		{
			iconOffset = new Vector2?(cardSetIconMatOffset.m_offset);
		}
		if (cardSet == TAG_CARD_SET.PROMO)
		{
			return;
		}
		list.Add(cardSet);
		if (cardSet == TAG_CARD_SET.REWARD)
		{
			list.Add(TAG_CARD_SET.PROMO);
		}
		this.m_setFilterTray.AddItem(GameStrings.GetCardSetNameShortened(cardSet), iconOffset, new SetFilterItem.ItemSelectedCallback(this.SetFilterCallback), list, GameUtils.IsSetRotated(cardSet), false);
	}

	// Token: 0x06002ED7 RID: 11991 RVA: 0x000EB443 File Offset: 0x000E9643
	private void OnCoverLoaded(string name, GameObject go, object userData)
	{
		this.m_isCoverLoading = false;
		this.m_cover = go.GetComponent<CollectionCoverDisplay>();
	}

	// Token: 0x06002ED8 RID: 11992 RVA: 0x000EB458 File Offset: 0x000E9658
	private void OnInputBlockerRelease(UIEvent e)
	{
		this.m_search.Deactivate();
	}

	// Token: 0x06002ED9 RID: 11993 RVA: 0x000EB465 File Offset: 0x000E9665
	private void OnSearchActivated()
	{
		this.EnableInput(false);
	}

	// Token: 0x06002EDA RID: 11994 RVA: 0x000EB470 File Offset: 0x000E9670
	private void OnSearchDeactivated(string oldSearchText, string newSearchText)
	{
		this.EnableInput(true);
		if (oldSearchText == newSearchText)
		{
			return;
		}
		this.NotifyFilterUpdate(this.m_searchFilterListeners, !string.IsNullOrEmpty(newSearchText), newSearchText);
		this.m_pageManager.ChangeSearchTextFilter(newSearchText, true);
	}

	// Token: 0x06002EDB RID: 11995 RVA: 0x000EB4B4 File Offset: 0x000E96B4
	private void OnSearchCleared(bool updateVisuals)
	{
		this.NotifyFilterUpdate(this.m_searchFilterListeners, false, string.Empty);
		this.m_pageManager.ChangeSearchTextFilter(string.Empty, updateVisuals);
	}

	// Token: 0x06002EDC RID: 11996 RVA: 0x000EB4DC File Offset: 0x000E96DC
	public void ShowTavernBrawlDeck(long deckID)
	{
		CollectionDeckTray.Get().GetDecksContent().SetEditingTraySection(0);
		CollectionDeckTray.Get().SetTrayMode(CollectionDeckTray.DeckContentTypes.Decks);
		CollectionManagerDisplay.Get().RequestContentsToShowDeck(deckID);
	}

	// Token: 0x06002EDD RID: 11997 RVA: 0x000EB510 File Offset: 0x000E9710
	private void DoEnterCollectionManagerEvents()
	{
		if (CollectionManager.Get().HasVisitedCollection() || SceneMgr.Get().GetMode() == SceneMgr.Mode.TAVERN_BRAWL)
		{
			this.EnableInput(true);
			this.OpenBookImmediately();
		}
		else
		{
			CollectionManager.Get().SetHasVisitedCollection(true);
			this.EnableInput(false);
			base.StartCoroutine(this.OpenBookWhenReady());
		}
	}

	// Token: 0x06002EDE RID: 11998 RVA: 0x000EB570 File Offset: 0x000E9770
	private void OpenBookImmediately()
	{
		if (SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER)
		{
			PresenceMgr.Get().SetStatus(new Enum[]
			{
				PresenceStatus.COLLECTION
			});
		}
		base.StartCoroutine(this.SetBookToOpen());
		if (SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER)
		{
			base.StartCoroutine(this.ShowCollectionTipsIfNeeded());
		}
	}

	// Token: 0x06002EDF RID: 11999 RVA: 0x000EB5D4 File Offset: 0x000E97D4
	private IEnumerator OpenBookWhenReady()
	{
		while (CollectionManager.Get().IsWaitingForBoxTransition())
		{
			yield return null;
		}
		if (SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER)
		{
			PresenceMgr.Get().SetStatus(new Enum[]
			{
				PresenceStatus.COLLECTION
			});
		}
		this.m_pageManager.OnBookOpening();
		base.StartCoroutine(this.DoBookOpeningAnimations());
		if (SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER)
		{
			base.StartCoroutine(this.ShowCollectionTipsIfNeeded());
		}
		yield break;
	}

	// Token: 0x06002EE0 RID: 12000 RVA: 0x000EB5F0 File Offset: 0x000E97F0
	private void ShowCraftingTipIfNeeded()
	{
		if (Options.Get().GetBool(Option.TIP_CRAFTING_UNLOCKED, false) || !UserAttentionManager.CanShowAttentionGrabber("CollectionManagerDisplay.ShowCraftingTipIfNeeded"))
		{
			return;
		}
		NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_DISENCHANT_31"), "VO_INNKEEPER_DISENCHANT_31", 0f, null);
		Options.Get().SetBool(Option.TIP_CRAFTING_UNLOCKED, true);
	}

	// Token: 0x06002EE1 RID: 12001 RVA: 0x000EB650 File Offset: 0x000E9850
	private Vector3 GetNewDeckPosition()
	{
		Vector3 vector = (!UniversalInputManager.UsePhoneUI) ? new Vector3(17.5f, 0f, 0f) : new Vector3(25.7f, 2.6f, 0f);
		CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
		if (collectionDeckTray != null)
		{
			DeckTrayDeckListContent decksContent = collectionDeckTray.GetDecksContent();
			return decksContent.GetNewDeckButtonPosition() - vector;
		}
		return new Vector3(0f, 0f, 0f);
	}

	// Token: 0x06002EE2 RID: 12002 RVA: 0x000EB6D4 File Offset: 0x000E98D4
	private Vector3 GetLastDeckPosition()
	{
		Vector3 vector = (!UniversalInputManager.UsePhoneUI) ? new Vector3(9.6f, 0f, 3f) : new Vector3(15.8f, 0f, 6f);
		CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
		if (collectionDeckTray != null)
		{
			DeckTrayDeckListContent decksContent = collectionDeckTray.GetDecksContent();
			return decksContent.GetLastUsedTraySection().transform.position - vector;
		}
		return new Vector3(0f, 0f, 0f);
	}

	// Token: 0x06002EE3 RID: 12003 RVA: 0x000EB764 File Offset: 0x000E9964
	private Vector3 GetMiddleDeckPosition()
	{
		int index = 4;
		Vector3 vector = (!UniversalInputManager.UsePhoneUI) ? new Vector3(9.6f, 0f, 3f) : new Vector3(15.8f, 0f, 6f);
		CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
		if (collectionDeckTray != null)
		{
			DeckTrayDeckListContent decksContent = collectionDeckTray.GetDecksContent();
			TraySection traySection = decksContent.GetTraySection(index);
			return traySection.transform.position - vector;
		}
		return new Vector3(0f, 0f, 0f);
	}

	// Token: 0x06002EE4 RID: 12004 RVA: 0x000EB7FC File Offset: 0x000E99FC
	private void ShowSetRotationNewDeckIndicator(float f)
	{
		string text = string.Empty;
		Vector3 position;
		if (CollectionManager.Get().GetNumberOfWildDecks() >= 18)
		{
			text = GameStrings.Get("GLUE_COLLECTION_TUTORIAL15");
			position = this.GetMiddleDeckPosition();
		}
		else
		{
			if (CollectionManager.Get().GetNumberOfWildDecks() <= 0)
			{
				return;
			}
			if (CollectionManager.Get().GetNumberOfStandardDecks() > 0)
			{
				text = GameStrings.Get("GLUE_COLLECTION_TUTORIAL14");
				position = this.GetLastDeckPosition();
			}
			else
			{
				text = GameStrings.Get("GLUE_COLLECTION_TUTORIAL10");
				CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
				collectionDeckTray.GetDecksContent().m_newDeckButton.m_highlightState.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
				position = this.GetNewDeckPosition();
			}
		}
		this.m_createDeckNotification = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS, position, this.m_editDeckTutorialBone.localScale, text, true);
		if (this.m_createDeckNotification != null)
		{
			this.m_createDeckNotification.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
			this.m_createDeckNotification.PulseReminderEveryXSeconds(3f);
		}
	}

	// Token: 0x06002EE5 RID: 12005 RVA: 0x000EB8F0 File Offset: 0x000E9AF0
	public IEnumerator ShowCollectionTipsIfNeeded()
	{
		if (CollectionManager.Get().ShouldShowWildToStandardTutorial(true) && CollectionManager.Get().GetNumberOfWildDecks() > 0 && UserAttentionManager.CanShowAttentionGrabber(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS, "CollectionManagerDisplay.ShowCollectionTipsIfNeeded:ShowSetRotationTutorial"))
		{
			int deckCount = CollectionManager.Get().GetDeckCount();
			CollectionDeckTray deckTray = CollectionDeckTray.Get();
			while (deckTray.IsUpdatingTrayMode() || !deckTray.GetDecksContent().IsDoneEntering())
			{
				yield return null;
			}
			if (deckCount >= 18)
			{
				NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS, GameStrings.Get("GLUE_COLLECTION_TUTORIAL11"), "VO_INNKEEPER_Male_Dwarf_FULL_DECKS_06", 0f, null);
				this.ShowSetRotationNewDeckIndicator(0f);
			}
			else if (deckCount > this.m_onscreenDecks)
			{
				deckTray.m_scrollbar.SetScroll(1f, new UIBScrollable.OnScrollComplete(this.ShowSetRotationNewDeckIndicator), iTween.EaseType.easeOutBounce, 0.75f, true, true);
			}
			else
			{
				this.ShowSetRotationNewDeckIndicator(0f);
			}
			yield break;
		}
		if (Options.Get().GetBool(Option.HAS_SEEN_PRACTICE_MODE, false))
		{
			Options.Get().SetBool(Option.HAS_SEEN_COLLECTIONMANAGER_AFTER_PRACTICE, true);
		}
		if (!Options.Get().GetBool(Option.HAS_SEEN_COLLECTIONMANAGER, false) && UserAttentionManager.CanShowAttentionGrabber("UserAttentionManager.CanShowAttentionGrabber:" + Option.HAS_SEEN_COLLECTIONMANAGER))
		{
			NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_CM_WELCOME"), "VO_INNKEEPER_Male_Dwarf_CM_WELCOME_23", 0f, null);
			Options.Get().SetBool(Option.HAS_SEEN_COLLECTIONMANAGER, true);
			yield return new WaitForSeconds(3.5f);
		}
		else
		{
			yield return new WaitForSeconds(1f);
		}
		if (!Options.Get().GetBool(Option.HAS_STARTED_A_DECK, false) && UserAttentionManager.CanShowAttentionGrabber("CollectionManagerDisplay.ShowCollectionTipsIfNeeded:" + Option.HAS_STARTED_A_DECK))
		{
			this.m_deckHelpPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, this.m_editDeckTutorialBone.position, this.m_editDeckTutorialBone.localScale, GameStrings.Get("GLUE_COLLECTION_TUTORIAL07"), true);
			this.m_deckHelpPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
			this.m_deckHelpPopup.PulseReminderEveryXSeconds(3f);
		}
		else if (!Options.Get().GetBool(Option.HAS_SEEN_DELETE_DECK_REMINDER, false) && AchieveManager.Get().HasUnlockedFeature(Achievement.UnlockableFeature.VANILLA_HEROES) && UserAttentionManager.CanShowAttentionGrabber("CollectionManagerDisplay.ShowCollectionTipsIfNeeded:" + Option.HAS_SEEN_DELETE_DECK_REMINDER))
		{
			this.m_deckHelpPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, this.m_editDeckTutorialBone.position, this.m_editDeckTutorialBone.localScale, GameStrings.Get("GLUE_COLLECTION_TUTORIAL09"), true);
			this.m_deckHelpPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
			this.m_deckHelpPopup.PulseReminderEveryXSeconds(3f);
			Options.Get().SetBool(Option.HAS_SEEN_DELETE_DECK_REMINDER, true);
		}
		yield break;
	}

	// Token: 0x06002EE6 RID: 12006 RVA: 0x000EB90C File Offset: 0x000E9B0C
	public IEnumerator ShowDeckTemplateTipsIfNeeded()
	{
		NotificationManager.Get().DestroyNotificationWithText(GameStrings.Get("GLUE_COLLECTION_TUTORIAL_TEMPLATE_REPLACE_1"), 0f);
		NotificationManager.Get().DestroyNotificationWithText(GameStrings.Get("GLUE_COLLECTION_TUTORIAL_TEMPLATE_REPLACE_2"), 0f);
		NotificationManager.Get().DestroyNotificationWithText(GameStrings.Get("GLUE_COLLECTION_TUTORIAL_REPLACE_WILD_CARDS"), 0f);
		base.StopCoroutine("ShowDeckTemplateTipsIfNeeded");
		if (this.m_currentViewMode != CollectionManagerDisplay.ViewMode.DECK_TEMPLATE)
		{
			while (CollectionDeckTray.Get().GetCurrentContentType() != CollectionDeckTray.DeckContentTypes.Cards || !CollectionDeckTray.Get().GetCardsContent().HasFinishedEntering())
			{
				yield return new WaitForEndOfFrame();
			}
		}
		DeckTrayDeckTileVisual invalidCard = CollectionDeckTray.Get().GetCardsContent().GetFirstInvalidCard();
		if (invalidCard != null)
		{
			string tipText = string.Empty;
			if (invalidCard.GetSlot().Owned)
			{
				if (invalidCard.GetSlot().Owned && Options.Get().GetBool(Option.HAS_SEEN_INVALID_ROTATED_CARD))
				{
					yield break;
				}
				tipText = GameStrings.Get("GLUE_COLLECTION_TUTORIAL_REPLACE_WILD_CARDS");
			}
			else
			{
				if (Options.Get().GetBool(Option.HAS_SEEN_DECK_TEMPLATE_GHOST_CARD) || !UserAttentionManager.CanShowAttentionGrabber("CollectionManagerDisplay.ShowDeckTemplateTipsIfNeeded:" + Option.HAS_SEEN_DECK_TEMPLATE_GHOST_CARD))
				{
					yield break;
				}
				if (this.m_currentViewMode == CollectionManagerDisplay.ViewMode.DECK_TEMPLATE)
				{
					if (UniversalInputManager.UsePhoneUI)
					{
						invalidCard = this.m_deckTemplatePickerPhone.m_phoneTray.GetCardsContent().GetFirstInvalidCard();
					}
					tipText = GameStrings.Get("GLUE_COLLECTION_TUTORIAL_TEMPLATE_REPLACE_1");
					yield return new WaitForSeconds(0.5f);
				}
				else
				{
					tipText = GameStrings.Get("GLUE_COLLECTION_TUTORIAL_TEMPLATE_REPLACE_2");
					yield return new WaitForSeconds(1f);
				}
			}
			float invalidTipMaxHeightPhone = -60f;
			Vector3 invalidTipPosition = OverlayUI.Get().GetRelativePosition(invalidCard.transform.position, Box.Get().m_Camera.GetComponent<Camera>(), OverlayUI.Get().m_heightScale.m_Center, 0f);
			Vector3 invalidTipScale;
			if (UniversalInputManager.UsePhoneUI)
			{
				invalidTipPosition.x -= 95.395f;
				invalidTipPosition.z -= 0.25f;
				invalidTipScale = 27.5f * Vector3.one;
				if (invalidTipPosition.z < invalidTipMaxHeightPhone)
				{
					invalidTipPosition.z = invalidTipMaxHeightPhone;
				}
			}
			else
			{
				invalidTipPosition.x -= 50.5f;
				invalidTipPosition.z -= 0.25f;
				invalidTipScale = NotificationManager.NOTIFICATITON_WORLD_SCALE;
			}
			if (this.m_currentViewMode == CollectionManagerDisplay.ViewMode.DECK_TEMPLATE)
			{
				this.m_deckTemplateCardReplacePopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, invalidTipPosition, invalidTipScale, tipText, false);
				if (this.m_deckTemplateCardReplacePopup != null)
				{
					this.m_deckTemplateCardReplacePopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
					NotificationManager.Get().DestroyNotification(this.m_deckTemplateCardReplacePopup, 3.5f);
				}
			}
			else
			{
				this.m_deckTemplateCardReplacePopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS, invalidTipPosition, invalidTipScale, tipText, false);
				if (this.m_deckTemplateCardReplacePopup != null)
				{
					this.m_deckTemplateCardReplacePopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
					this.m_deckTemplateCardReplacePopup.PulseReminderEveryXSeconds(3f);
				}
			}
		}
		yield break;
	}

	// Token: 0x06002EE7 RID: 12007 RVA: 0x000EB927 File Offset: 0x000E9B27
	private void OnCoverOpened()
	{
		this.EnableInput(true);
	}

	// Token: 0x06002EE8 RID: 12008 RVA: 0x000EB930 File Offset: 0x000E9B30
	private void OnSwitchViewModeResponse(bool triggerResponse, CollectionManagerDisplay.ViewMode prevMode, CollectionManagerDisplay.ViewMode newMode, CollectionManagerDisplay.ViewModeData userdata)
	{
		CollectionManagerDisplay.OnSwitchViewMode[] array = this.m_switchViewModeListeners.ToArray();
		foreach (CollectionManagerDisplay.OnSwitchViewMode onSwitchViewMode in array)
		{
			onSwitchViewMode(prevMode, newMode, userdata, triggerResponse);
		}
		base.StartCoroutine(this.ShowDeckTemplateTipsIfNeeded());
		this.EnableSearchUI(newMode);
	}

	// Token: 0x06002EE9 RID: 12009 RVA: 0x000EB984 File Offset: 0x000E9B84
	private void EnableSearchUI(CollectionManagerDisplay.ViewMode viewMode)
	{
		bool flag = viewMode == CollectionManagerDisplay.ViewMode.CARDS;
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_craftingModeButton.Enable(flag);
		}
		this.m_manaTabManager.Enable(flag);
		if (this.m_setFilterTray != null)
		{
			this.m_setFilterTray.SetButtonEnabled(flag);
		}
		if (this.m_filterButton != null)
		{
			this.m_filterButton.SetEnabled(flag);
		}
		this.m_search.SetEnabled(viewMode != CollectionManagerDisplay.ViewMode.CARD_BACKS);
	}

	// Token: 0x06002EEA RID: 12010 RVA: 0x000EBA0C File Offset: 0x000E9C0C
	private void ShowCompleteAchieve(object userData)
	{
		if (this.m_completeAchievesToDisplay.Count == 0)
		{
			return;
		}
		Achievement quest = this.m_completeAchievesToDisplay[0];
		this.m_completeAchievesToDisplay.RemoveAt(0);
		QuestToast.ShowQuestToast(UserAttentionBlocker.NONE, new QuestToast.DelOnCloseQuestToast(this.ShowCompleteAchieve), true, quest);
	}

	// Token: 0x06002EEB RID: 12011 RVA: 0x000EBA58 File Offset: 0x000E9C58
	private void OnCraftingTrayLoaded(string name, GameObject go, object userData)
	{
		go.SetActive(false);
		this.m_craftingTray = go.GetComponent<CraftingTray>();
		go.transform.parent = this.m_craftingTrayShownBone.transform.parent;
		go.transform.localPosition = this.m_craftingTrayHiddenBone.transform.localPosition;
		go.transform.localScale = this.m_craftingTrayHiddenBone.transform.localScale;
		this.m_pageManager.UpdateMassDisenchant();
	}

	// Token: 0x06002EEC RID: 12012 RVA: 0x000EBAD4 File Offset: 0x000E9CD4
	private void OnCraftingModeButtonReleased(UIEvent e)
	{
		if (this.m_craftingTray.IsShown())
		{
			this.m_craftingTray.Hide();
		}
		else
		{
			this.ShowCraftingTray(default(bool?), default(bool?), true);
		}
	}

	// Token: 0x06002EED RID: 12013 RVA: 0x000EBB1A File Offset: 0x000E9D1A
	public void LoadCraftingManager(AssetLoader.GameObjectCallback callback)
	{
		callback(string.Empty, null, null);
	}

	// Token: 0x06002EEE RID: 12014 RVA: 0x000EBB2C File Offset: 0x000E9D2C
	public void ShowCraftingTray(bool? includeUncraftable = null, bool? showOnlyGolden = null, bool updatePage = true)
	{
		bool flag = CollectionManager.Get().IsInEditMode();
		bool showWild = AchieveManager.Get().HasUnlockedFeature(Achievement.UnlockableFeature.VANILLA_HEROES);
		if (flag)
		{
			CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
			showWild = (editedDeck != null && editedDeck.IsWild);
		}
		this.UpdateSetFilters(showWild, flag, true);
		CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
		if (collectionDeckTray != null)
		{
			DeckTrayDeckListContent decksContent = collectionDeckTray.GetDecksContent();
			if (decksContent != null)
			{
				decksContent.CancelRenameEditingDeck();
			}
		}
		this.HideDeckHelpPopup();
		this.m_craftingTray.gameObject.SetActive(true);
		this.m_craftingTray.Show(includeUncraftable, showOnlyGolden, updatePage);
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			this.m_craftingTrayShownBone.transform.localPosition,
			"isLocal",
			true,
			"time",
			0.25f,
			"easeType",
			iTween.EaseType.easeOutBounce
		});
		iTween.Stop(this.m_craftingTray.gameObject);
		iTween.MoveTo(this.m_craftingTray.gameObject, args);
		this.m_craftingModeButton.ShowActiveGlow(true);
	}

	// Token: 0x06002EEF RID: 12015 RVA: 0x000EBC64 File Offset: 0x000E9E64
	public void HideCraftingTray()
	{
		this.ShowAppropriateSetFilters();
		this.m_craftingTray.gameObject.SetActive(true);
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			this.m_craftingTrayHiddenBone.transform.localPosition,
			"isLocal",
			true,
			"time",
			0.25f,
			"easeType",
			iTween.EaseType.easeOutBounce,
			"oncomplete",
			delegate(object o)
			{
				this.m_craftingTray.gameObject.SetActive(false);
			}
		});
		iTween.Stop(this.m_craftingTray.gameObject);
		iTween.MoveTo(this.m_craftingTray.gameObject, args);
		this.m_craftingModeButton.ShowActiveGlow(false);
	}

	// Token: 0x06002EF0 RID: 12016 RVA: 0x000EBD32 File Offset: 0x000E9F32
	public void ShowConvertTutorial(UserAttentionBlocker blocker)
	{
		if (!UserAttentionManager.CanShowAttentionGrabber(blocker, "CollectionManagerDisplay.ShowConvertTutorial"))
		{
			return;
		}
		this.m_showConvertTutorialCoroutine = this.ShowConvertTutorialCoroutine(blocker);
		base.StartCoroutine(this.m_showConvertTutorialCoroutine);
	}

	// Token: 0x06002EF1 RID: 12017 RVA: 0x000EBD60 File Offset: 0x000E9F60
	private IEnumerator ShowConvertTutorialCoroutine(UserAttentionBlocker blocker)
	{
		if (this.m_createDeckNotification != null)
		{
			NotificationManager.Get().DestroyNotification(this.m_createDeckNotification, 0.25f);
		}
		CollectionDeckTray deckTray = CollectionDeckTray.Get();
		while (deckTray.IsUpdatingTrayMode() || !deckTray.GetDecksContent().IsDoneEntering())
		{
			yield return null;
		}
		yield return new WaitForSeconds(0.5f);
		if (this.m_currentViewMode == CollectionManagerDisplay.ViewMode.DECK_TEMPLATE)
		{
			yield break;
		}
		this.m_convertTutorialPopup = NotificationManager.Get().CreatePopupText(blocker, this.m_convertDeckTutorialBone.position, this.m_convertDeckTutorialBone.localScale, GameStrings.Get("GLUE_COLLECTION_TUTORIAL12"), true);
		if (this.m_convertTutorialPopup != null)
		{
			this.m_convertTutorialPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
			this.m_convertTutorialPopup.PulseReminderEveryXSeconds(3f);
		}
		this.m_showConvertTutorialCoroutine = null;
		yield break;
	}

	// Token: 0x06002EF2 RID: 12018 RVA: 0x000EBD89 File Offset: 0x000E9F89
	public void HideConvertTutorial()
	{
		if (this.m_showConvertTutorialCoroutine != null)
		{
			base.StopCoroutine(this.m_showConvertTutorialCoroutine);
		}
		if (this.m_convertTutorialPopup != null)
		{
			NotificationManager.Get().DestroyNotification(this.m_convertTutorialPopup, 0.25f);
		}
	}

	// Token: 0x06002EF3 RID: 12019 RVA: 0x000EBDC8 File Offset: 0x000E9FC8
	public void ShowStandardInfoTutorial(UserAttentionBlocker blocker)
	{
		NotificationManager.Get().CreateInnkeeperQuote(blocker, GameStrings.Get("GLUE_COLLECTION_TUTORIAL13"), "VO_INNKEEPER_Male_Dwarf_STANDARD_WELCOME3_14", 0f, null);
	}

	// Token: 0x04001CEC RID: 7404
	private const float CRAFTING_TRAY_SLIDE_IN_TIME = 0.25f;

	// Token: 0x04001CED RID: 7405
	[CustomEditField(Sections = "Prefabs")]
	public CollectionCardVisual m_cardVisualPrefab;

	// Token: 0x04001CEE RID: 7406
	[CustomEditField(Sections = "Bones")]
	public GameObject m_activeSearchBone;

	// Token: 0x04001CEF RID: 7407
	[CustomEditField(Sections = "Bones")]
	public GameObject m_activeSearchBone_Win8;

	// Token: 0x04001CF0 RID: 7408
	[CustomEditField(Sections = "Bones")]
	public GameObject m_craftingTrayHiddenBone;

	// Token: 0x04001CF1 RID: 7409
	[CustomEditField(Sections = "Bones")]
	public GameObject m_craftingTrayShownBone;

	// Token: 0x04001CF2 RID: 7410
	[CustomEditField(Sections = "Bones")]
	public Transform m_deckTemplateHiddenBone;

	// Token: 0x04001CF3 RID: 7411
	[CustomEditField(Sections = "Bones")]
	public Transform m_deckTemplateShownBone;

	// Token: 0x04001CF4 RID: 7412
	[CustomEditField(Sections = "Bones")]
	public Transform m_deckTemplateTutorialWelcomeBone;

	// Token: 0x04001CF5 RID: 7413
	[CustomEditField(Sections = "Bones")]
	public Transform m_deckTemplateTutorialReminderBone;

	// Token: 0x04001CF6 RID: 7414
	[CustomEditField(Sections = "Bones")]
	public Transform m_editDeckTutorialBone;

	// Token: 0x04001CF7 RID: 7415
	[CustomEditField(Sections = "Bones")]
	public Transform m_convertDeckTutorialBone;

	// Token: 0x04001CF8 RID: 7416
	[CustomEditField(Sections = "Objects")]
	public ManaFilterTabManager m_manaTabManager;

	// Token: 0x04001CF9 RID: 7417
	[CustomEditField(Sections = "Objects")]
	public CollectionPageManager m_pageManager;

	// Token: 0x04001CFA RID: 7418
	[CustomEditField(Sections = "Objects")]
	public CollectionCoverDisplay m_cover;

	// Token: 0x04001CFB RID: 7419
	[CustomEditField(Sections = "Objects")]
	public CollectionSearch m_search;

	// Token: 0x04001CFC RID: 7420
	[CustomEditField(Sections = "Objects")]
	public ActiveFilterButton m_filterButton;

	// Token: 0x04001CFD RID: 7421
	[CustomEditField(Sections = "Objects")]
	public CraftingModeButton m_craftingModeButton;

	// Token: 0x04001CFE RID: 7422
	[CustomEditField(Sections = "Objects")]
	public Notification m_deckTemplateCardReplacePopup;

	// Token: 0x04001CFF RID: 7423
	[CustomEditField(Sections = "Objects")]
	public PegUIElement m_inputBlocker;

	// Token: 0x04001D00 RID: 7424
	[CustomEditField(Sections = "Objects")]
	public NestedPrefab m_setFilterTrayContainer;

	// Token: 0x04001D01 RID: 7425
	[CustomEditField(Sections = "Controls")]
	public Vector2 m_allSetsIconOffset;

	// Token: 0x04001D02 RID: 7426
	[CustomEditField(Sections = "Controls")]
	public Vector2 m_wildSetsIconOffset;

	// Token: 0x04001D03 RID: 7427
	[CustomEditField(Sections = "Controls")]
	public CollectionPageLayoutSettings m_pageLayoutSettings = new CollectionPageLayoutSettings();

	// Token: 0x04001D04 RID: 7428
	[CustomEditField(Sections = "Materials")]
	public Material m_goldenCardNotOwnedMeshMaterial;

	// Token: 0x04001D05 RID: 7429
	[CustomEditField(Sections = "Materials")]
	public Material m_cardNotOwnedMeshMaterial;

	// Token: 0x04001D06 RID: 7430
	[CustomEditField(ListTable = true, Sections = "Materials")]
	public List<CollectionManagerDisplay.CardSetIconMatOffset> m_manaFilterCardSetIcons = new List<CollectionManagerDisplay.CardSetIconMatOffset>();

	// Token: 0x04001D07 RID: 7431
	[CustomEditField(Sections = "Tavern Brawl Changes")]
	public GameObject m_bookBack;

	// Token: 0x04001D08 RID: 7432
	[CustomEditField(Sections = "Tavern Brawl Changes", T = EditType.TEXTURE)]
	public string m_corkBackTexture;

	// Token: 0x04001D09 RID: 7433
	[CustomEditField(Sections = "Tavern Brawl Changes")]
	public Mesh m_tavernBrawlBookBackMesh;

	// Token: 0x04001D0A RID: 7434
	[CustomEditField(Sections = "Tavern Brawl Changes")]
	public Material m_tavernBrawlElements;

	// Token: 0x04001D0B RID: 7435
	[CustomEditField(Sections = "Tavern Brawl Changes")]
	public List<GameObject> m_tavernBrawlObjectsToSwap = new List<GameObject>();

	// Token: 0x04001D0C RID: 7436
	private static CollectionManagerDisplay s_instance;

	// Token: 0x04001D0D RID: 7437
	private bool m_netCacheReady;

	// Token: 0x04001D0E RID: 7438
	private bool m_isReady;

	// Token: 0x04001D0F RID: 7439
	private bool m_unloading;

	// Token: 0x04001D10 RID: 7440
	private Map<TAG_CLASS, Texture> m_loadedClassTextures = new Map<TAG_CLASS, Texture>();

	// Token: 0x04001D11 RID: 7441
	private Map<TAG_CLASS, CollectionManagerDisplay.TextureRequests> m_requestedClassTextures = new Map<TAG_CLASS, CollectionManagerDisplay.TextureRequests>();

	// Token: 0x04001D12 RID: 7442
	private List<Actor> m_cardBackActors = new List<Actor>();

	// Token: 0x04001D13 RID: 7443
	private List<Actor> m_previousCardBackActors;

	// Token: 0x04001D14 RID: 7444
	private List<Actor> m_cardActors = new List<Actor>();

	// Token: 0x04001D15 RID: 7445
	private List<Actor> m_previousCardActors = new List<Actor>();

	// Token: 0x04001D16 RID: 7446
	private int m_displayRequestID;

	// Token: 0x04001D17 RID: 7447
	private bool m_selectingNewDeckHero;

	// Token: 0x04001D18 RID: 7448
	private long m_showDeckContentsRequest;

	// Token: 0x04001D19 RID: 7449
	private Notification m_deckHelpPopup;

	// Token: 0x04001D1A RID: 7450
	private Notification m_innkeeperLClickReminder;

	// Token: 0x04001D1B RID: 7451
	private bool m_setFilterTrayInitialized;

	// Token: 0x04001D1C RID: 7452
	private bool m_isCoverLoading;

	// Token: 0x04001D1D RID: 7453
	private List<Achievement> m_completeAchievesToDisplay = new List<Achievement>();

	// Token: 0x04001D1E RID: 7454
	private CraftingTray m_craftingTray;

	// Token: 0x04001D1F RID: 7455
	private SetFilterTray m_setFilterTray;

	// Token: 0x04001D20 RID: 7456
	private List<CollectionManagerDisplay.OnSwitchViewMode> m_switchViewModeListeners = new List<CollectionManagerDisplay.OnSwitchViewMode>();

	// Token: 0x04001D21 RID: 7457
	private CollectionManagerDisplay.ViewMode m_currentViewMode;

	// Token: 0x04001D22 RID: 7458
	private List<CollectionManagerDisplay.FilterStateListener> m_searchFilterListeners = new List<CollectionManagerDisplay.FilterStateListener>();

	// Token: 0x04001D23 RID: 7459
	private List<CollectionManagerDisplay.FilterStateListener> m_setFilterListeners = new List<CollectionManagerDisplay.FilterStateListener>();

	// Token: 0x04001D24 RID: 7460
	private List<CollectionManagerDisplay.FilterStateListener> m_manaFilterListeners = new List<CollectionManagerDisplay.FilterStateListener>();

	// Token: 0x04001D25 RID: 7461
	private DeckTemplatePicker m_deckTemplatePickerPhone;

	// Token: 0x04001D26 RID: 7462
	private HeroPickerDisplay m_heroPickerDisplay;

	// Token: 0x04001D27 RID: 7463
	private int m_inputBlockers;

	// Token: 0x04001D28 RID: 7464
	private Notification m_createDeckNotification;

	// Token: 0x04001D29 RID: 7465
	private Notification m_convertTutorialPopup;

	// Token: 0x04001D2A RID: 7466
	private IEnumerator m_showConvertTutorialCoroutine;

	// Token: 0x04001D2B RID: 7467
	private PlatformDependentValue<int> m_onscreenDecks = new PlatformDependentValue<int>(PlatformCategory.Screen)
	{
		PC = 8,
		Phone = 4
	};

	// Token: 0x020005B6 RID: 1462
	public enum ViewMode
	{
		// Token: 0x0400298C RID: 10636
		CARDS,
		// Token: 0x0400298D RID: 10637
		HERO_SKINS,
		// Token: 0x0400298E RID: 10638
		CARD_BACKS,
		// Token: 0x0400298F RID: 10639
		DECK_TEMPLATE,
		// Token: 0x04002990 RID: 10640
		COUNT
	}

	// Token: 0x020005B7 RID: 1463
	public class ViewModeData
	{
		// Token: 0x04002991 RID: 10641
		public TAG_CLASS? m_setPageByClass;

		// Token: 0x04002992 RID: 10642
		public string m_setPageByCard;

		// Token: 0x04002993 RID: 10643
		public TAG_PREMIUM m_setPageByPremium;

		// Token: 0x04002994 RID: 10644
		public CollectionPageManager.DelOnPageTransitionComplete m_pageTransitionCompleteCallback;

		// Token: 0x04002995 RID: 10645
		public object m_pageTransitionCompleteData;

		// Token: 0x020006CA RID: 1738
		// (Invoke) Token: 0x06004851 RID: 18513
		public delegate bool WaitToTurnPageDelegate();
	}

	// Token: 0x020006BE RID: 1726
	// (Invoke) Token: 0x06004801 RID: 18433
	public delegate void FilterStateListener(bool filterActive, object value);

	// Token: 0x020006CB RID: 1739
	// (Invoke) Token: 0x06004855 RID: 18517
	public delegate void OnSwitchViewMode(CollectionManagerDisplay.ViewMode prevMode, CollectionManagerDisplay.ViewMode mode, CollectionManagerDisplay.ViewModeData userdata, bool triggerResponse);

	// Token: 0x020006CC RID: 1740
	[Serializable]
	public class CardSetIconMatOffset
	{
		// Token: 0x04002F9D RID: 12189
		public TAG_CARD_SET m_cardSet;

		// Token: 0x04002F9E RID: 12190
		public Vector2 m_offset;
	}

	// Token: 0x020006CD RID: 1741
	private class TextureRequests
	{
		// Token: 0x04002F9F RID: 12191
		public List<CollectionManagerDisplay.TextureRequests.Request> m_requests = new List<CollectionManagerDisplay.TextureRequests.Request>();

		// Token: 0x020006CE RID: 1742
		public class Request
		{
			// Token: 0x04002FA0 RID: 12192
			public CollectionManagerDisplay.DelTextureLoaded m_callback;

			// Token: 0x04002FA1 RID: 12193
			public object m_callbackData;
		}
	}

	// Token: 0x020006CF RID: 1743
	// (Invoke) Token: 0x0600485C RID: 18524
	public delegate void DelTextureLoaded(TAG_CLASS classTag, Texture classTexture, object callbackData);

	// Token: 0x020006D0 RID: 1744
	// (Invoke) Token: 0x06004860 RID: 18528
	public delegate void CollectionActorsReadyCallback(List<Actor> actors, object callbackData);
}
