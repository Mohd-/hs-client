using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000388 RID: 904
[CustomEditClass]
public class CollectionPageManager : MonoBehaviour
{
	// Token: 0x06002EF6 RID: 12022 RVA: 0x000EBEAC File Offset: 0x000EA0AC
	// Note: this type is marked as 'beforefieldinit'.
	static CollectionPageManager()
	{
		TAG_CLASS[] array = new TAG_CLASS[10];
		array[0] = TAG_CLASS.DRUID;
		array[1] = TAG_CLASS.HUNTER;
		array[2] = TAG_CLASS.MAGE;
		array[3] = TAG_CLASS.PALADIN;
		array[4] = TAG_CLASS.PRIEST;
		array[5] = TAG_CLASS.ROGUE;
		array[6] = TAG_CLASS.SHAMAN;
		array[7] = TAG_CLASS.WARLOCK;
		array[8] = TAG_CLASS.WARRIOR;
		CollectionPageManager.CLASS_TAB_ORDER = array;
		CollectionPageManager.TAG_ORDERING = new CollectionManagerDisplay.ViewMode[]
		{
			CollectionManagerDisplay.ViewMode.CARDS,
			CollectionManagerDisplay.ViewMode.CARD_BACKS,
			CollectionManagerDisplay.ViewMode.HERO_SKINS
		};
		CollectionPageManager.SELECT_TAB_ANIM_TIME = 0.2f;
		CollectionPageManager.CURRENT_PAGE_LOCAL_POS = new Vector3(0f, 0.25f, 0f);
		CollectionPageManager.NEXT_PAGE_LOCAL_POS = Vector3.zero;
		CollectionPageManager.CLASS_TAB_LOCAL_EULERS = new Vector3(0f, 180f, 0f);
		CollectionPageManager.HIDDEN_TAB_LOCAL_Z_POS = -0.42f;
		CollectionPageManager.ARROW_SCALE_TIME = 0.6f;
		CollectionPageManager.ANIMATE_TABS_COROUTINE_NAME = "AnimateTabs";
		CollectionPageManager.SELECT_TAB_COROUTINE_NAME = "SelectTabWhenReady";
		CollectionPageManager.SHOW_ARROWS_COROUTINE_NAME = "WaitThenShowArrows";
		CollectionPageManager.NUM_PAGE_FLIPS_BEFORE_STOP_SHOWING_ARROWS = 20;
		CollectionPageManager.MASS_DISENCHANT_PAGE_NUM = 1000;
		CollectionPageManager.CLASS_TO_TAB_IDX = null;
	}

	// Token: 0x06002EF7 RID: 12023 RVA: 0x000EC194 File Offset: 0x000EA394
	private void Awake()
	{
		if (CollectionPageManager.CLASS_TO_TAB_IDX == null)
		{
			CollectionPageManager.CLASS_TO_TAB_IDX = new Map<TAG_CLASS, int>();
			for (int i = 0; i < CollectionPageManager.CLASS_TAB_ORDER.Length; i++)
			{
				CollectionPageManager.CLASS_TO_TAB_IDX.Add(CollectionPageManager.CLASS_TAB_ORDER[i], i);
			}
		}
		this.m_cardsCollection.Init(CollectionPageManager.CLASS_TAB_ORDER, CollectionPageDisplay.GetMaxCardsPerPage(CollectionManagerDisplay.ViewMode.CARDS));
		this.m_heroesCollection.Init(CollectionPageDisplay.GetMaxCardsPerPage(CollectionManagerDisplay.ViewMode.HERO_SKINS));
		this.UpdateFilteredHeroes();
		this.UpdateFilteredCards();
		if (this.m_massDisenchant)
		{
			this.m_massDisenchant.Hide();
		}
		this.m_pageLeftClickableRegion.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPageLeftPressed));
		this.m_pageLeftClickableRegion.SetCursorOver(PegCursor.Mode.LEFTARROW);
		this.m_pageLeftClickableRegion.SetCursorDown(PegCursor.Mode.LEFTARROW);
		this.m_pageRightClickableRegion.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPageRightPressed));
		this.m_pageRightClickableRegion.SetCursorOver(PegCursor.Mode.RIGHTARROW);
		this.m_pageRightClickableRegion.SetCursorDown(PegCursor.Mode.RIGHTARROW);
		CollectionManagerDisplay collectionManagerDisplay = CollectionManagerDisplay.Get();
		collectionManagerDisplay.RegisterSwitchViewModeListener(new CollectionManagerDisplay.OnSwitchViewMode(this.OnCollectionManagerViewModeChanged));
		this.m_wasTouchModeEnabled = UniversalInputManager.Get().IsTouchMode();
		if (UniversalInputManager.Get().IsTouchMode())
		{
			base.gameObject.AddComponent<CollectionPageManagerTouchBehavior>();
		}
		this.m_pageA = Object.Instantiate<CollectionPageDisplay>(this.m_pageDisplayPrefab);
		this.m_pageB = Object.Instantiate<CollectionPageDisplay>(this.m_pageDisplayPrefab);
		TransformUtil.AttachAndPreserveLocalTransform(this.m_pageA.transform, base.transform);
		TransformUtil.AttachAndPreserveLocalTransform(this.m_pageB.transform, base.transform);
		CollectionManager collectionManager = CollectionManager.Get();
		collectionManager.RegisterFavoriteHeroChangedListener(new CollectionManager.FavoriteHeroChangedCallback(this.OnFavoriteHeroChanged));
		collectionManager.RegisterDefaultCardbackChangedListener(new CollectionManager.DefaultCardbackChangedCallback(this.OnDefaultCardbackChanged));
	}

	// Token: 0x06002EF8 RID: 12024 RVA: 0x000EC34C File Offset: 0x000EA54C
	private void Start()
	{
		this.SetUpClassTabs();
		CollectionPageDisplay alternatePage = this.GetAlternatePage();
		CollectionPageDisplay currentPage = this.GetCurrentPage();
		this.AssembleEmptyPageUI(alternatePage, false);
		this.AssembleEmptyPageUI(currentPage, false);
		this.PositionNextPage(alternatePage);
		this.PositionCurrentPage(currentPage);
		this.m_fullyLoaded = true;
	}

	// Token: 0x06002EF9 RID: 12025 RVA: 0x000EC394 File Offset: 0x000EA594
	private void Update()
	{
		bool flag = UniversalInputManager.Get().IsTouchMode();
		if (this.m_wasTouchModeEnabled != flag)
		{
			this.m_wasTouchModeEnabled = flag;
			if (flag)
			{
				base.gameObject.AddComponent<CollectionPageManagerTouchBehavior>();
			}
			else
			{
				Object.Destroy(base.gameObject.GetComponent<CollectionPageManagerTouchBehavior>());
			}
			foreach (CollectionClassTab collectionClassTab in this.m_allTabs)
			{
				collectionClassTab.SetReceiveReleaseWithoutMouseDown(flag);
			}
		}
	}

	// Token: 0x06002EFA RID: 12026 RVA: 0x000EC434 File Offset: 0x000EA634
	private void OnDestroy()
	{
		if (CollectionManagerDisplay.Get() != null)
		{
			CollectionManagerDisplay.Get().RemoveSwitchViewModeListener(new CollectionManagerDisplay.OnSwitchViewMode(this.OnCollectionManagerViewModeChanged));
		}
		CollectionManager collectionManager = CollectionManager.Get();
		if (collectionManager != null)
		{
			collectionManager.RemoveFavoriteHeroChangedListener(new CollectionManager.FavoriteHeroChangedCallback(this.OnFavoriteHeroChanged));
			collectionManager.RemoveDefaultCardbackChangedListener(new CollectionManager.DefaultCardbackChangedCallback(this.OnDefaultCardbackChanged));
		}
	}

	// Token: 0x06002EFB RID: 12027 RVA: 0x000EC49C File Offset: 0x000EA69C
	public bool HideNonDeckTemplateTabs(bool hide, bool updateTabs = false)
	{
		if (this.m_hideNonDeckTemplateTabs == hide)
		{
			return false;
		}
		this.m_hideNonDeckTemplateTabs = hide;
		if (updateTabs)
		{
			this.UpdateVisibleTabs();
		}
		return true;
	}

	// Token: 0x06002EFC RID: 12028 RVA: 0x000EC4CB File Offset: 0x000EA6CB
	public bool IsNonDeckTemplateTabsHidden()
	{
		return this.m_hideNonDeckTemplateTabs;
	}

	// Token: 0x06002EFD RID: 12029 RVA: 0x000EC4D3 File Offset: 0x000EA6D3
	public void OnCollectionLoaded()
	{
		this.ShowOnlyCardsIOwn();
	}

	// Token: 0x06002EFE RID: 12030 RVA: 0x000EC4DC File Offset: 0x000EA6DC
	public void OnBookOpening()
	{
		base.StopCoroutine(CollectionPageManager.SHOW_ARROWS_COROUTINE_NAME);
		base.StartCoroutine(CollectionPageManager.SHOW_ARROWS_COROUTINE_NAME);
		this.TransitionPageWhenReady(CollectionPageManager.PageTransitionType.NONE, true, null, null);
	}

	// Token: 0x06002EFF RID: 12031 RVA: 0x000EC50C File Offset: 0x000EA70C
	public void SetClassFilter(TAG_CLASS shownClass, bool skipPageTurn, bool updatePages, CollectionPageManager.DelOnPageTransitionComplete callback = null, object callbackData = null)
	{
		this.m_skipNextPageTurn = skipPageTurn;
		CollectibleCardFilter cardsCollection = this.m_cardsCollection;
		TAG_CLASS[] array = new TAG_CLASS[2];
		array[0] = shownClass;
		cardsCollection.FilterTheseClasses(array);
		this.m_heroesCollection.FilterTheseClasses(new TAG_CLASS[]
		{
			shownClass
		});
		this.m_heroesCollection.FilterOnlyOwned(true);
		this.UpdateFilteredCards();
		this.UpdateFilteredHeroes();
		if (updatePages)
		{
			if (CollectionManagerDisplay.Get().GetViewMode() == CollectionManagerDisplay.ViewMode.CARDS)
			{
				this.JumpToCollectionClassPage(shownClass, callback, callbackData);
			}
			else
			{
				this.TransitionPageWhenReady(CollectionPageManager.PageTransitionType.NONE, false, null, null);
			}
		}
	}

	// Token: 0x06002F00 RID: 12032 RVA: 0x000EC591 File Offset: 0x000EA791
	public bool JumpToPageWithCard(string cardID, TAG_PREMIUM premium)
	{
		return this.JumpToPageWithCard(cardID, premium, null, null);
	}

	// Token: 0x06002F01 RID: 12033 RVA: 0x000EC5A0 File Offset: 0x000EA7A0
	public bool JumpToPageWithCard(string cardID, TAG_PREMIUM premium, CollectionPageManager.DelOnPageTransitionComplete callback, object callbackData)
	{
		int num;
		List<CollectibleCard> pageContentsForCard = this.m_cardsCollection.GetPageContentsForCard(cardID, premium, out num);
		if (pageContentsForCard.Count == 0)
		{
			return false;
		}
		if (this.m_currentPageNum == num)
		{
			return false;
		}
		this.FlipToPage(num, callback, callbackData);
		return true;
	}

	// Token: 0x06002F02 RID: 12034 RVA: 0x000EC5E3 File Offset: 0x000EA7E3
	private void RemoveAllClassFilters()
	{
		this.RemoveAllClassFilters(null, null);
	}

	// Token: 0x06002F03 RID: 12035 RVA: 0x000EC5F0 File Offset: 0x000EA7F0
	private void RemoveAllClassFilters(CollectionPageManager.DelOnPageTransitionComplete callback, object callbackData)
	{
		this.m_cardsCollection.FilterTheseClasses(null);
		this.m_heroesCollection.FilterTheseClasses(null);
		this.m_heroesCollection.FilterOnlyOwned(false);
		this.UpdateFilteredCards();
		this.UpdateFilteredHeroes();
		CollectionPageManager.PageTransitionType transitionType = (CollectionManagerDisplay.Get().GetViewMode() != CollectionManagerDisplay.ViewMode.CARDS) ? CollectionPageManager.PageTransitionType.NONE : CollectionPageManager.PageTransitionType.SINGLE_PAGE_LEFT;
		this.TransitionPageWhenReady(transitionType, false, callback, callbackData);
	}

	// Token: 0x06002F04 RID: 12036 RVA: 0x000EC64E File Offset: 0x000EA84E
	public void FilterByManaCost(int cost)
	{
		this.FilterByManaCost(cost, null, null);
	}

	// Token: 0x06002F05 RID: 12037 RVA: 0x000EC65C File Offset: 0x000EA85C
	public void FilterByManaCost(int cost, CollectionPageManager.DelOnPageTransitionComplete callback, object callbackData)
	{
		if (cost == ManaFilterTab.ALL_TAB_IDX)
		{
			this.m_cardsCollection.FilterManaCost(default(int?));
		}
		else
		{
			this.m_cardsCollection.FilterManaCost(new int?(cost));
		}
		this.UpdateFilteredCards();
		this.TransitionPageWhenReady(CollectionPageManager.PageTransitionType.NONE, false, callback, callbackData);
	}

	// Token: 0x06002F06 RID: 12038 RVA: 0x000EC6AE File Offset: 0x000EA8AE
	public void FilterByCardSets(List<TAG_CARD_SET> cardSets)
	{
		this.FilterByCardSets(cardSets, null, null);
	}

	// Token: 0x06002F07 RID: 12039 RVA: 0x000EC6BC File Offset: 0x000EA8BC
	public void FilterByCardSets(List<TAG_CARD_SET> cardSets, CollectionPageManager.DelOnPageTransitionComplete callback, object callbackData)
	{
		TAG_CARD_SET[] cardSets2 = null;
		if (cardSets != null && cardSets.Count > 0)
		{
			cardSets2 = cardSets.ToArray();
		}
		this.m_cardsCollection.FilterTheseCardSets(cardSets2);
		this.UpdateFilteredCards();
		CollectionPageManager.PageTransitionType transitionType = (!SceneMgr.Get().IsTransitioning()) ? CollectionPageManager.PageTransitionType.SINGLE_PAGE_RIGHT : CollectionPageManager.PageTransitionType.NONE;
		this.TransitionPageWhenReady(transitionType, false, callback, callbackData);
	}

	// Token: 0x06002F08 RID: 12040 RVA: 0x000EC717 File Offset: 0x000EA917
	public bool CardSetFilterIncludesWild()
	{
		return this.m_cardsCollection.CardSetFilterIncludesWild();
	}

	// Token: 0x06002F09 RID: 12041 RVA: 0x000EC724 File Offset: 0x000EA924
	public void ChangeSearchTextFilter(string newSearchText, bool updateVisuals = true)
	{
		this.ChangeSearchTextFilter(newSearchText, null, null, updateVisuals);
	}

	// Token: 0x06002F0A RID: 12042 RVA: 0x000EC730 File Offset: 0x000EA930
	public void ChangeSearchTextFilter(string newSearchText, CollectionPageManager.DelOnPageTransitionComplete callback, object callbackData, bool updateVisuals = true)
	{
		this.m_cardsCollection.FilterSearchText(newSearchText);
		this.m_heroesCollection.FilterSearchText(newSearchText);
		this.UpdateFilteredCards();
		this.UpdateFilteredHeroes();
		if (updateVisuals)
		{
			this.TransitionPageWhenReady(CollectionPageManager.PageTransitionType.NONE, false, callback, callbackData);
		}
	}

	// Token: 0x06002F0B RID: 12043 RVA: 0x000EC772 File Offset: 0x000EA972
	public void RemoveSearchTextFilter()
	{
		this.RemoveSearchTextFilter(null, null);
	}

	// Token: 0x06002F0C RID: 12044 RVA: 0x000EC77C File Offset: 0x000EA97C
	public void RemoveSearchTextFilter(CollectionPageManager.DelOnPageTransitionComplete callback, object callbackData)
	{
		this.m_cardsCollection.FilterSearchText(null);
		this.m_heroesCollection.FilterSearchText(null);
		this.UpdateFilteredCards();
		this.UpdateFilteredHeroes();
		this.TransitionPageWhenReady(CollectionPageManager.PageTransitionType.NONE, false, callback, callbackData);
	}

	// Token: 0x06002F0D RID: 12045 RVA: 0x000EC7B7 File Offset: 0x000EA9B7
	public void ShowOnlyCardsIOwn()
	{
		this.ShowOnlyCardsIOwn(null, null);
	}

	// Token: 0x06002F0E RID: 12046 RVA: 0x000EC7C4 File Offset: 0x000EA9C4
	public void ShowOnlyCardsIOwn(CollectionPageManager.DelOnPageTransitionComplete callback, object callbackData)
	{
		this.m_cardsCollection.FilterOnlyOwned(true);
		this.m_cardsCollection.FilterPremium(default(TAG_PREMIUM?));
		this.m_cardsCollection.FilterOnlyCraftable(false);
		this.UpdateFilteredCards();
		this.TransitionPageWhenReady(CollectionPageManager.PageTransitionType.NONE, false, callback, callbackData);
	}

	// Token: 0x06002F0F RID: 12047 RVA: 0x000EC80D File Offset: 0x000EAA0D
	public void ShowCardsNotOwned(bool includePremiums)
	{
		this.ShowCardsNotOwned(includePremiums, null, null);
	}

	// Token: 0x06002F10 RID: 12048 RVA: 0x000EC818 File Offset: 0x000EAA18
	public void ShowCardsNotOwned(bool includePremiums, CollectionPageManager.DelOnPageTransitionComplete callback, object callbackData)
	{
		this.m_cardsCollection.FilterOnlyOwned(false);
		this.m_cardsCollection.FilterPremium(default(TAG_PREMIUM?));
		this.UpdateFilteredCards();
		this.TransitionPageWhenReady(CollectionPageManager.PageTransitionType.NONE, false, callback, callbackData);
	}

	// Token: 0x06002F11 RID: 12049 RVA: 0x000EC855 File Offset: 0x000EAA55
	public void ShowPremiumCardsOnly()
	{
		this.ShowPremiumCardsOnly(null, null);
	}

	// Token: 0x06002F12 RID: 12050 RVA: 0x000EC860 File Offset: 0x000EAA60
	public void ShowPremiumCardsOnly(CollectionPageManager.DelOnPageTransitionComplete callback, object callbackData)
	{
		this.m_cardsCollection.FilterOnlyOwned(false);
		this.m_cardsCollection.FilterPremium(new TAG_PREMIUM?(TAG_PREMIUM.GOLDEN));
		this.UpdateFilteredCards();
		this.TransitionPageWhenReady(CollectionPageManager.PageTransitionType.NONE, false, callback, callbackData);
	}

	// Token: 0x06002F13 RID: 12051 RVA: 0x000EC89A File Offset: 0x000EAA9A
	public void ShowCraftableCardsOnly(bool showCraftableCardsOnly)
	{
		this.ShowCraftableCardsOnly(showCraftableCardsOnly, null, null);
	}

	// Token: 0x06002F14 RID: 12052 RVA: 0x000EC8A8 File Offset: 0x000EAAA8
	public void ShowCraftableCardsOnly(bool showCraftableCardsOnly, CollectionPageManager.DelOnPageTransitionComplete callback, object callbackData)
	{
		this.m_cardsCollection.FilterOnlyCraftable(showCraftableCardsOnly);
		this.UpdateFilteredCards();
		this.TransitionPageWhenReady(CollectionPageManager.PageTransitionType.NONE, false, callback, callbackData);
	}

	// Token: 0x06002F15 RID: 12053 RVA: 0x000EC8D1 File Offset: 0x000EAAD1
	public void ShowCraftingModeCards(bool showCraftableCardsOnly, bool showGolden, bool updatePage = true)
	{
		this.ShowCraftingModeCards(showCraftableCardsOnly, showGolden, null, null, updatePage);
	}

	// Token: 0x06002F16 RID: 12054 RVA: 0x000EC8E0 File Offset: 0x000EAAE0
	public void ShowCraftingModeCards(bool showCraftableCardsOnly, bool showGolden, CollectionPageManager.DelOnPageTransitionComplete callback, object callbackData, bool updatePage = true)
	{
		this.m_cardsCollection.FilterPremium(new TAG_PREMIUM?((!showGolden) ? TAG_PREMIUM.NORMAL : TAG_PREMIUM.GOLDEN));
		this.m_cardsCollection.FilterOnlyOwned(false);
		this.m_cardsCollection.FilterOnlyCraftable(showCraftableCardsOnly);
		this.UpdateFilteredCards();
		if (updatePage)
		{
			this.TransitionPageWhenReady(CollectionPageManager.PageTransitionType.NONE, false, callback, callbackData);
		}
	}

	// Token: 0x06002F17 RID: 12055 RVA: 0x000EC93A File Offset: 0x000EAB3A
	public void UpdateCurrentPageCardLocks(bool playSound)
	{
		this.GetCurrentPage().UpdateCurrentPageCardLocks(playSound);
	}

	// Token: 0x06002F18 RID: 12056 RVA: 0x000EC948 File Offset: 0x000EAB48
	public void UpdateClassTabNewCardCounts()
	{
		foreach (CollectionClassTab collectionClassTab in this.m_classTabs)
		{
			TAG_CLASS @class = collectionClassTab.GetClass();
			int numNewItems = (collectionClassTab.m_tabViewMode != CollectionManagerDisplay.ViewMode.DECK_TEMPLATE) ? this.GetNumNewCardsForClass(@class) : 0;
			collectionClassTab.UpdateNewItemCount(numNewItems);
		}
	}

	// Token: 0x06002F19 RID: 12057 RVA: 0x000EC9C4 File Offset: 0x000EABC4
	public int GetNumNewCardsForClass(TAG_CLASS tagClass)
	{
		return this.m_cardsCollection.GetNumNewCardsForClass(tagClass);
	}

	// Token: 0x06002F1A RID: 12058 RVA: 0x000EC9D2 File Offset: 0x000EABD2
	public void NotifyOfCollectionChanged()
	{
		this.UpdateMassDisenchant();
	}

	// Token: 0x06002F1B RID: 12059 RVA: 0x000EC9DC File Offset: 0x000EABDC
	public void OnDoneEditingDeck()
	{
		this.RemoveAllClassFilters();
		this.UpdateCraftingModeButtonDustBottleVisibility();
		NotificationManager.Get().DestroyNotificationWithText(GameStrings.Get("GLUE_COLLECTION_TUTORIAL_TEMPLATE_REPLACE_1"), 0f);
		NotificationManager.Get().DestroyNotificationWithText(GameStrings.Get("GLUE_COLLECTION_TUTORIAL_TEMPLATE_REPLACE_2"), 0f);
		NotificationManager.Get().DestroyNotificationWithText(GameStrings.Get("GLUE_COLLECTION_TUTORIAL_REPLACE_WILD_CARDS"), 0f);
		CollectionDeckTray.Get().GetCardsContent().HideDeckHelpPopup();
	}

	// Token: 0x06002F1C RID: 12060 RVA: 0x000ECA50 File Offset: 0x000EAC50
	public void UpdateCraftingModeButtonDustBottleVisibility()
	{
		bool flag = CollectionManagerDisplay.Get().GetViewMode() == CollectionManagerDisplay.ViewMode.CARDS;
		bool show = flag && CollectionManager.Get().GetCardsToDisenchantCount() > 0 && CollectionManager.Get().GetTaggedDeck(CollectionManager.DeckTag.Editing) == null;
		CollectionManagerDisplay.Get().m_craftingModeButton.ShowDustBottle(show);
	}

	// Token: 0x06002F1D RID: 12061 RVA: 0x000ECAA3 File Offset: 0x000EACA3
	public int GetMassDisenchantAmount()
	{
		return CollectionManager.Get().GetCardsToDisenchantCount();
	}

	// Token: 0x06002F1E RID: 12062 RVA: 0x000ECAAF File Offset: 0x000EACAF
	public void RefreshCurrentPageContents()
	{
		this.RefreshCurrentPageContents(CollectionPageManager.PageTransitionType.NONE, null, null);
	}

	// Token: 0x06002F1F RID: 12063 RVA: 0x000ECABA File Offset: 0x000EACBA
	public void RefreshCurrentPageContents(CollectionPageManager.PageTransitionType transition)
	{
		this.RefreshCurrentPageContents(transition, null, null);
	}

	// Token: 0x06002F20 RID: 12064 RVA: 0x000ECAC5 File Offset: 0x000EACC5
	public void RefreshCurrentPageContents(CollectionPageManager.DelOnPageTransitionComplete callback, object callbackData)
	{
		this.RefreshCurrentPageContents(CollectionPageManager.PageTransitionType.NONE, null, null);
	}

	// Token: 0x06002F21 RID: 12065 RVA: 0x000ECAD0 File Offset: 0x000EACD0
	public void RefreshCurrentPageContents(CollectionPageManager.PageTransitionType transition, CollectionPageManager.DelOnPageTransitionComplete callback, object callbackData)
	{
		this.UpdateFilteredCards();
		this.TransitionPageWhenReady(transition, true, callback, callbackData);
	}

	// Token: 0x06002F22 RID: 12066 RVA: 0x000ECAE2 File Offset: 0x000EACE2
	public CollectionCardVisual GetCardVisual(string cardID, TAG_PREMIUM premium)
	{
		return this.GetCurrentPage().GetCardVisual(cardID, premium);
	}

	// Token: 0x06002F23 RID: 12067 RVA: 0x000ECAF1 File Offset: 0x000EACF1
	public bool IsFullyLoaded()
	{
		return this.m_fullyLoaded;
	}

	// Token: 0x06002F24 RID: 12068 RVA: 0x000ECAFC File Offset: 0x000EACFC
	public void LoadMassDisenchantScreen()
	{
		if (this.m_massDisenchant != null)
		{
			return;
		}
		GameObject gameObject = AssetLoader.Get().LoadUIScreen((!UniversalInputManager.UsePhoneUI) ? "MassDisenchant" : "MassDisenchant_phone", true, false);
		this.m_massDisenchant = gameObject.GetComponent<MassDisenchant>();
		this.m_massDisenchant.Hide();
	}

	// Token: 0x06002F25 RID: 12069 RVA: 0x000ECB5D File Offset: 0x000EAD5D
	public void ShowMassDisenchant()
	{
		this.m_lastPageNum = this.m_currentPageNum;
		this.m_currentPageNum = CollectionPageManager.MASS_DISENCHANT_PAGE_NUM;
		this.TransitionPageWhenReady(CollectionPageManager.PageTransitionType.MANY_PAGE_RIGHT, false, null, null);
	}

	// Token: 0x06002F26 RID: 12070 RVA: 0x000ECB80 File Offset: 0x000EAD80
	public void HideMassDisenchant()
	{
		bool flag = this.IsShowingMassDisenchant();
		this.m_currentPageNum = this.m_lastPageNum;
		this.m_cardsCollection.FilterOnlyCraftable(false);
		this.m_cardsCollection.FilterPremium(default(TAG_PREMIUM?));
		this.m_cardsCollection.FilterOnlyOwned(true);
		this.UpdateFilteredCards();
		this.TransitionPageWhenReady((!flag) ? CollectionPageManager.PageTransitionType.NONE : CollectionPageManager.PageTransitionType.MANY_PAGE_LEFT, false, null, null);
	}

	// Token: 0x06002F27 RID: 12071 RVA: 0x000ECBE8 File Offset: 0x000EADE8
	public bool IsShowingMassDisenchant()
	{
		return this.m_currentPageNum == CollectionPageManager.MASS_DISENCHANT_PAGE_NUM;
	}

	// Token: 0x06002F28 RID: 12072 RVA: 0x000ECBF7 File Offset: 0x000EADF7
	public bool ArePagesTurning()
	{
		return this.m_pagesCurrentlyTurning;
	}

	// Token: 0x06002F29 RID: 12073 RVA: 0x000ECBFF File Offset: 0x000EADFF
	public int GetNumPagesForClass(TAG_CLASS classTag)
	{
		return this.m_cardsCollection.GetNumPagesForClass(classTag);
	}

	// Token: 0x06002F2A RID: 12074 RVA: 0x000ECC10 File Offset: 0x000EAE10
	private bool ShouldShowTab(CollectionClassTab tab)
	{
		if (!this.m_initializedTabPositions)
		{
			return true;
		}
		if (this.m_hideNonDeckTemplateTabs)
		{
			return tab.m_tabViewMode == CollectionManagerDisplay.ViewMode.DECK_TEMPLATE;
		}
		if (tab.m_tabViewMode == CollectionManagerDisplay.ViewMode.CARDS)
		{
			return this.m_cardsCollection.GetNumPagesForClass(tab.GetClass()) > 0;
		}
		CollectionDeck taggedDeck = CollectionManager.Get().GetTaggedDeck(CollectionManager.DeckTag.Editing);
		bool flag = taggedDeck != null;
		if (tab.m_tabViewMode == CollectionManagerDisplay.ViewMode.DECK_TEMPLATE)
		{
			return flag && SceneMgr.Get().GetMode() != SceneMgr.Mode.TAVERN_BRAWL;
		}
		if (flag)
		{
			CollectionManagerDisplay.ViewMode tabViewMode = tab.m_tabViewMode;
			if (tabViewMode != CollectionManagerDisplay.ViewMode.HERO_SKINS)
			{
				if (tabViewMode == CollectionManagerDisplay.ViewMode.CARD_BACKS)
				{
					if (CardBackManager.Get().GetCardBacksOwned().Count <= 1)
					{
						return false;
					}
				}
			}
			else if (CollectionManager.Get().GetBestHeroesIOwn(taggedDeck.GetClass()).Count <= 1)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002F2B RID: 12075 RVA: 0x000ECCFC File Offset: 0x000EAEFC
	private void SetUpClassTabs()
	{
		if (UniversalInputManager.UsePhoneUI)
		{
			return;
		}
		bool receiveReleaseWithoutMouseDown = UniversalInputManager.Get().IsTouchMode();
		if (this.m_deckTemplateTab != null && this.m_deckTemplateTab.gameObject.activeSelf)
		{
			this.m_allTabs.Add(this.m_deckTemplateTab);
			this.m_classTabs.Add(this.m_deckTemplateTab);
			this.m_deckTemplateTab.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDeckTemplateTabPressed));
			this.m_deckTemplateTab.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnTabOver));
			this.m_deckTemplateTab.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnTabOut));
			this.m_deckTemplateTab.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnTabOver_Touch));
			this.m_deckTemplateTab.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnTabOut_Touch));
			this.m_deckTemplateTab.SetReceiveReleaseWithoutMouseDown(receiveReleaseWithoutMouseDown);
			this.m_tabVisibility[this.m_deckTemplateTab] = true;
		}
		for (int i = 0; i < CollectionPageManager.CLASS_TAB_ORDER.Length; i++)
		{
			TAG_CLASS tag_CLASS = CollectionPageManager.CLASS_TAB_ORDER[i];
			CollectionClassTab collectionClassTab = (CollectionClassTab)GameUtils.Instantiate(this.m_classTabPrefab, this.m_classTabContainer, false);
			collectionClassTab.Init(new TAG_CLASS?(tag_CLASS));
			collectionClassTab.transform.localScale = collectionClassTab.m_DeselectedLocalScale;
			collectionClassTab.transform.localEulerAngles = CollectionPageManager.CLASS_TAB_LOCAL_EULERS;
			collectionClassTab.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClassTabPressed));
			collectionClassTab.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnTabOver));
			collectionClassTab.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnTabOut));
			collectionClassTab.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnTabOver_Touch));
			collectionClassTab.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnTabOut_Touch));
			collectionClassTab.SetReceiveReleaseWithoutMouseDown(receiveReleaseWithoutMouseDown);
			collectionClassTab.gameObject.name = tag_CLASS.ToString();
			this.m_allTabs.Add(collectionClassTab);
			this.m_classTabs.Add(collectionClassTab);
			this.m_tabVisibility[collectionClassTab] = true;
			if (i <= 0)
			{
				this.m_deselectedClassTabHalfWidth = collectionClassTab.GetComponent<BoxCollider>().bounds.extents.x;
			}
		}
		if (this.m_heroSkinsTab != null)
		{
			this.m_heroSkinsTab.Init(default(TAG_CLASS?));
			this.m_heroSkinsTab.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnHeroSkinsTabPressed));
			this.m_heroSkinsTab.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnTabOver));
			this.m_heroSkinsTab.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnTabOut));
			this.m_heroSkinsTab.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnTabOver_Touch));
			this.m_heroSkinsTab.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnTabOut_Touch));
			this.m_heroSkinsTab.SetReceiveReleaseWithoutMouseDown(receiveReleaseWithoutMouseDown);
			this.m_allTabs.Add(this.m_heroSkinsTab);
			this.m_tabVisibility[this.m_heroSkinsTab] = true;
			this.m_heroSkinsTabPos = this.m_heroSkinsTab.transform.localPosition;
		}
		if (this.m_cardBacksTab != null)
		{
			this.m_cardBacksTab.Init(default(TAG_CLASS?));
			this.m_cardBacksTab.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCardBacksTabPressed));
			this.m_cardBacksTab.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnTabOver));
			this.m_cardBacksTab.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnTabOut));
			this.m_cardBacksTab.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnTabOver_Touch));
			this.m_cardBacksTab.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnTabOut_Touch));
			this.m_cardBacksTab.SetReceiveReleaseWithoutMouseDown(receiveReleaseWithoutMouseDown);
			this.m_allTabs.Add(this.m_cardBacksTab);
			this.m_tabVisibility[this.m_cardBacksTab] = true;
			this.m_cardBacksTabPos = this.m_cardBacksTab.transform.localPosition;
		}
		this.PositionClassTabs(false);
		this.m_initializedTabPositions = true;
	}

	// Token: 0x06002F2C RID: 12076 RVA: 0x000ED120 File Offset: 0x000EB320
	private void PositionClassTabs(bool animate)
	{
		Vector3 position = this.m_classTabContainer.transform.position;
		int num = CollectionPageManager.CLASS_TAB_ORDER.Length;
		if (this.m_deckTemplateTab != null && this.m_deckTemplateTab.gameObject.activeSelf)
		{
			num++;
		}
		for (int i = 0; i < num; i++)
		{
			CollectionClassTab collectionClassTab = this.m_classTabs[i];
			bool flag = this.ShouldShowTab(collectionClassTab);
			Vector3 vector;
			if (flag)
			{
				collectionClassTab.SetTargetVisibility(true);
				position.x += this.m_spaceBetweenTabs;
				position.x += this.m_deselectedClassTabHalfWidth;
				vector = this.m_classTabContainer.transform.InverseTransformPoint(position);
				if (collectionClassTab == this.m_currentClassTab)
				{
					vector.y = collectionClassTab.m_SelectedLocalYPos;
				}
				position.x += this.m_deselectedClassTabHalfWidth;
			}
			else
			{
				collectionClassTab.SetTargetVisibility(false);
				vector = collectionClassTab.transform.localPosition;
				vector.z = CollectionPageManager.HIDDEN_TAB_LOCAL_Z_POS;
			}
			if (animate)
			{
				collectionClassTab.SetTargetLocalPosition(vector);
			}
			else
			{
				collectionClassTab.SetIsVisible(collectionClassTab.ShouldBeVisible());
				collectionClassTab.transform.localPosition = vector;
			}
		}
		bool showTab = this.ShouldShowTab(this.m_heroSkinsTab);
		this.PositionFixedTab(showTab, this.m_heroSkinsTab, this.m_heroSkinsTabPos, animate);
		bool showTab2 = this.ShouldShowTab(this.m_cardBacksTab);
		this.PositionFixedTab(showTab2, this.m_cardBacksTab, this.m_cardBacksTabPos, animate);
		if (!animate)
		{
			return;
		}
		base.StopCoroutine(CollectionPageManager.ANIMATE_TABS_COROUTINE_NAME);
		base.StartCoroutine(CollectionPageManager.ANIMATE_TABS_COROUTINE_NAME);
	}

	// Token: 0x06002F2D RID: 12077 RVA: 0x000ED2C8 File Offset: 0x000EB4C8
	private void PositionFixedTab(bool showTab, CollectionClassTab tab, Vector3 originalPos, bool animate)
	{
		if (!showTab)
		{
			originalPos.z -= 0.5f;
		}
		tab.SetTargetVisibility(showTab);
		tab.SetTargetLocalPosition(originalPos);
		if (animate)
		{
			tab.AnimateToTargetPosition(0.4f, iTween.EaseType.easeOutQuad);
		}
		else
		{
			tab.SetIsVisible(tab.ShouldBeVisible());
			tab.transform.localPosition = originalPos;
		}
	}

	// Token: 0x06002F2E RID: 12078 RVA: 0x000ED32C File Offset: 0x000EB52C
	private IEnumerator AnimateTabs()
	{
		bool playSounds = HeroPickerDisplay.Get() == null || !HeroPickerDisplay.Get().IsShown();
		List<CollectionClassTab> tabsToHide = new List<CollectionClassTab>();
		List<CollectionClassTab> tabsToShow = new List<CollectionClassTab>();
		List<CollectionClassTab> tabsToMove = new List<CollectionClassTab>();
		foreach (CollectionClassTab tab in this.m_classTabs)
		{
			if (tab.IsVisible() || tab.ShouldBeVisible())
			{
				if (tab.IsVisible() && tab.ShouldBeVisible())
				{
					tabsToMove.Add(tab);
				}
				else if (tab.IsVisible() && !tab.ShouldBeVisible())
				{
					tabsToHide.Add(tab);
				}
				else
				{
					tabsToShow.Add(tab);
				}
				tab.SetIsVisible(tab.ShouldBeVisible());
			}
		}
		this.m_tabsAreAnimating = true;
		if (tabsToHide.Count > 0)
		{
			foreach (CollectionClassTab tab2 in tabsToHide)
			{
				if (playSounds)
				{
					SoundManager.Get().LoadAndPlay("class_tab_retract", tab2.gameObject);
				}
				yield return new WaitForSeconds(0.03f);
				tab2.AnimateToTargetPosition(0.1f, iTween.EaseType.easeOutQuad);
			}
			yield return new WaitForSeconds(0.1f);
		}
		if (tabsToMove.Count > 0)
		{
			foreach (CollectionClassTab tab3 in tabsToMove)
			{
				if (tab3.WillSlide() && playSounds)
				{
					SoundManager.Get().LoadAndPlay("class_tab_slides_across_top", tab3.gameObject);
				}
				tab3.AnimateToTargetPosition(0.25f, iTween.EaseType.easeOutQuad);
			}
			yield return new WaitForSeconds(0.25f);
		}
		if (tabsToShow.Count > 0)
		{
			foreach (CollectionClassTab tab4 in tabsToShow)
			{
				if (playSounds)
				{
					SoundManager.Get().LoadAndPlay("class_tab_retract", tab4.gameObject);
				}
				tab4.AnimateToTargetPosition(0.4f, iTween.EaseType.easeOutBounce);
			}
			yield return new WaitForSeconds(0.4f);
		}
		this.m_tabsAreAnimating = false;
		yield break;
	}

	// Token: 0x06002F2F RID: 12079 RVA: 0x000ED34A File Offset: 0x000EB54A
	private void DeselectCurrentClassTab()
	{
		if (this.m_currentClassTab == null)
		{
			return;
		}
		this.m_currentClassTab.SetSelected(false);
		this.m_currentClassTab.SetLargeTab(false);
		this.m_currentClassTab = null;
	}

	// Token: 0x06002F30 RID: 12080 RVA: 0x000ED380 File Offset: 0x000EB580
	private void SetCurrentClassTab(TAG_CLASS? tabClass)
	{
		CollectionClassTab collectionClassTab = null;
		CollectionManagerDisplay.ViewMode viewMode = CollectionManagerDisplay.Get().GetViewMode();
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_classFilterHeader.SetMode(viewMode, tabClass);
			return;
		}
		switch (viewMode)
		{
		case CollectionManagerDisplay.ViewMode.CARDS:
			if (tabClass != null)
			{
				collectionClassTab = this.m_classTabs.Find((CollectionClassTab obj) => obj.GetClass() == tabClass.Value && obj.m_tabViewMode != CollectionManagerDisplay.ViewMode.DECK_TEMPLATE);
			}
			break;
		case CollectionManagerDisplay.ViewMode.HERO_SKINS:
			collectionClassTab = this.m_heroSkinsTab;
			break;
		case CollectionManagerDisplay.ViewMode.CARD_BACKS:
			collectionClassTab = this.m_cardBacksTab;
			break;
		default:
			collectionClassTab = null;
			break;
		}
		if (collectionClassTab == this.m_currentClassTab)
		{
			return;
		}
		this.DeselectCurrentClassTab();
		this.m_currentClassTab = collectionClassTab;
		if (this.m_currentClassTab != null)
		{
			base.StopCoroutine(CollectionPageManager.SELECT_TAB_COROUTINE_NAME);
			base.StartCoroutine(CollectionPageManager.SELECT_TAB_COROUTINE_NAME, this.m_currentClassTab);
		}
	}

	// Token: 0x06002F31 RID: 12081 RVA: 0x000ED47C File Offset: 0x000EB67C
	public void SetDeckRuleset(DeckRuleset deckRuleset, bool refresh = false)
	{
		this.m_cardsCollection.SetDeckRuleset(deckRuleset);
		if (refresh)
		{
			this.UpdateFilteredCards();
			this.TransitionPageWhenReady(CollectionPageManager.PageTransitionType.NONE, false, null, null);
		}
	}

	// Token: 0x06002F32 RID: 12082 RVA: 0x000ED4AC File Offset: 0x000EB6AC
	private IEnumerator SelectTabWhenReady(CollectionClassTab tab)
	{
		while (this.m_tabsAreAnimating)
		{
			yield return 0;
		}
		if (this.m_currentClassTab != tab)
		{
			yield break;
		}
		tab.SetSelected(true);
		tab.SetLargeTab(true);
		yield break;
	}

	// Token: 0x06002F33 RID: 12083 RVA: 0x000ED4D8 File Offset: 0x000EB6D8
	private void UpdateVisibleTabs()
	{
		if (UniversalInputManager.UsePhoneUI)
		{
			return;
		}
		bool flag = false;
		foreach (CollectionClassTab collectionClassTab in this.m_allTabs)
		{
			bool flag2 = this.m_tabVisibility[collectionClassTab];
			bool flag3 = this.ShouldShowTab(collectionClassTab);
			if (flag2 != flag3)
			{
				flag = true;
				this.m_tabVisibility[collectionClassTab] = flag3;
			}
		}
		if (!flag)
		{
			return;
		}
		this.PositionClassTabs(true);
	}

	// Token: 0x06002F34 RID: 12084 RVA: 0x000ED57C File Offset: 0x000EB77C
	private void OnTabOver(UIEvent e)
	{
		CollectionClassTab collectionClassTab = e.GetElement() as CollectionClassTab;
		if (collectionClassTab == null)
		{
			return;
		}
		collectionClassTab.SetGlowActive(true);
	}

	// Token: 0x06002F35 RID: 12085 RVA: 0x000ED5AC File Offset: 0x000EB7AC
	private void OnTabOut(UIEvent e)
	{
		CollectionClassTab collectionClassTab = e.GetElement() as CollectionClassTab;
		if (collectionClassTab == null)
		{
			return;
		}
		collectionClassTab.SetGlowActive(false);
	}

	// Token: 0x06002F36 RID: 12086 RVA: 0x000ED5DC File Offset: 0x000EB7DC
	private void OnTabOver_Touch(UIEvent e)
	{
		if (!UniversalInputManager.Get().IsTouchMode())
		{
			return;
		}
		CollectionClassTab collectionClassTab = e.GetElement() as CollectionClassTab;
		collectionClassTab.SetLargeTab(true);
	}

	// Token: 0x06002F37 RID: 12087 RVA: 0x000ED60C File Offset: 0x000EB80C
	private void OnTabOut_Touch(UIEvent e)
	{
		if (!UniversalInputManager.Get().IsTouchMode())
		{
			return;
		}
		CollectionClassTab collectionClassTab = e.GetElement() as CollectionClassTab;
		if (collectionClassTab != this.m_currentClassTab)
		{
			collectionClassTab.SetLargeTab(false);
		}
	}

	// Token: 0x06002F38 RID: 12088 RVA: 0x000ED650 File Offset: 0x000EB850
	private void OnClassTabPressed(UIEvent e)
	{
		CollectionClassTab collectionClassTab = e.GetElement() as CollectionClassTab;
		if (collectionClassTab == null || collectionClassTab == this.m_currentClassTab)
		{
			return;
		}
		TAG_CLASS @class = collectionClassTab.GetClass();
		this.JumpToCollectionClassPage(@class);
	}

	// Token: 0x06002F39 RID: 12089 RVA: 0x000ED695 File Offset: 0x000EB895
	private void OnDeckTemplateTabPressed(UIEvent e)
	{
		CollectionManagerDisplay.Get().SetViewMode(CollectionManagerDisplay.ViewMode.DECK_TEMPLATE, null);
	}

	// Token: 0x06002F3A RID: 12090 RVA: 0x000ED6A4 File Offset: 0x000EB8A4
	private void OnHeroSkinsTabPressed(UIEvent e)
	{
		CollectionClassTab collectionClassTab = e.GetElement() as CollectionClassTab;
		if (collectionClassTab == null || collectionClassTab == this.m_currentClassTab)
		{
			return;
		}
		CollectionManagerDisplay.Get().SetViewMode(CollectionManagerDisplay.ViewMode.HERO_SKINS, null);
	}

	// Token: 0x06002F3B RID: 12091 RVA: 0x000ED6E8 File Offset: 0x000EB8E8
	private void OnCardBacksTabPressed(UIEvent e)
	{
		CollectionClassTab collectionClassTab = e.GetElement() as CollectionClassTab;
		if (collectionClassTab == null || collectionClassTab == this.m_currentClassTab)
		{
			return;
		}
		CollectionManagerDisplay.Get().SetViewMode(CollectionManagerDisplay.ViewMode.CARD_BACKS, null);
	}

	// Token: 0x06002F3C RID: 12092 RVA: 0x000ED72C File Offset: 0x000EB92C
	public void UpdateMassDisenchant()
	{
		if (this.m_massDisenchant == null)
		{
			this.UpdateCraftingModeButtonDustBottleVisibility();
			return;
		}
		this.m_massDisenchant.UpdateContents(CollectionManager.Get().GetMassDisenchantCards());
		if (CraftingTray.Get() != null)
		{
			CraftingTray.Get().SetMassDisenchantAmount();
		}
		this.UpdateCraftingModeButtonDustBottleVisibility();
	}

	// Token: 0x06002F3D RID: 12093 RVA: 0x000ED786 File Offset: 0x000EB986
	public void JumpToCollectionClassPage(TAG_CLASS pageClass)
	{
		this.JumpToCollectionClassPage(pageClass, null, null);
	}

	// Token: 0x06002F3E RID: 12094 RVA: 0x000ED794 File Offset: 0x000EB994
	public void JumpToCollectionClassPage(TAG_CLASS pageClass, CollectionPageManager.DelOnPageTransitionComplete callback, object callbackData)
	{
		CollectionManagerDisplay collectionManagerDisplay = CollectionManagerDisplay.Get();
		if (collectionManagerDisplay.GetViewMode() != CollectionManagerDisplay.ViewMode.CARDS)
		{
			collectionManagerDisplay.SetViewMode(CollectionManagerDisplay.ViewMode.CARDS, new CollectionManagerDisplay.ViewModeData
			{
				m_setPageByClass = new TAG_CLASS?(pageClass)
			});
		}
		else
		{
			int newCollectionPage = 0;
			this.m_cardsCollection.GetPageContentsForClass(pageClass, 1, true, out newCollectionPage);
			this.FlipToPage(newCollectionPage, callback, callbackData);
		}
	}

	// Token: 0x06002F3F RID: 12095 RVA: 0x000ED7F0 File Offset: 0x000EB9F0
	public void FlipToPage(int newCollectionPage, CollectionPageManager.DelOnPageTransitionComplete callback, object callbackData)
	{
		int num = newCollectionPage - this.m_currentPageNum;
		bool flag = num < 0;
		if (Math.Abs(num) != 1)
		{
			this.m_currentPageNum = newCollectionPage;
			this.TransitionPageWhenReady((!flag) ? CollectionPageManager.PageTransitionType.MANY_PAGE_RIGHT : CollectionPageManager.PageTransitionType.MANY_PAGE_LEFT, true, callback, callbackData);
			return;
		}
		if (flag)
		{
			this.PageLeft(callback, callbackData);
		}
		else
		{
			this.PageRight(callback, callbackData);
		}
	}

	// Token: 0x06002F40 RID: 12096 RVA: 0x000ED850 File Offset: 0x000EBA50
	private void SwapCurrentAndAltPages()
	{
		this.m_currentPageIsPageA = !this.m_currentPageIsPageA;
	}

	// Token: 0x06002F41 RID: 12097 RVA: 0x000ED861 File Offset: 0x000EBA61
	private CollectionPageDisplay GetCurrentPage()
	{
		return (!this.m_currentPageIsPageA) ? this.m_pageB : this.m_pageA;
	}

	// Token: 0x06002F42 RID: 12098 RVA: 0x000ED87F File Offset: 0x000EBA7F
	private CollectionPageDisplay GetAlternatePage()
	{
		return (!this.m_currentPageIsPageA) ? this.m_pageA : this.m_pageB;
	}

	// Token: 0x06002F43 RID: 12099 RVA: 0x000ED8A0 File Offset: 0x000EBAA0
	private void AssembleEmptyPageUI(CollectionPageDisplay page, bool displayNoMatchesText)
	{
		page.SetClass(default(TAG_CLASS?));
		page.ShowNoMatchesFound(displayNoMatchesText);
		if (CollectionManagerDisplay.Get().GetViewMode() == CollectionManagerDisplay.ViewMode.CARDS)
		{
			this.DeselectCurrentClassTab();
		}
		page.SetPageCountText(GameStrings.Get("GLUE_COLLECTION_EMPTY_PAGE"));
		this.ActivateArrows(false, false);
	}

	// Token: 0x06002F44 RID: 12100 RVA: 0x000ED8F0 File Offset: 0x000EBAF0
	private bool AssembleBasePage(CollectionPageManager.TransitionReadyCallbackData transitionReadyCallbackData, bool emptyPage, bool wildPage)
	{
		CollectionPageDisplay page = transitionReadyCallbackData.m_assembledPage;
		page.UpdateBasePage();
		page.SetIsWild(wildPage);
		bool flag = CollectionPageManager.MASS_DISENCHANT_PAGE_NUM == this.m_currentPageNum;
		if (flag)
		{
			base.StartCoroutine(this.ShowMassDisenchantPage());
			page.ActivatePageCountText(false);
		}
		else
		{
			if (this.m_massDisenchant != null)
			{
				this.m_massDisenchant.Hide();
			}
			page.ActivatePageCountText(true);
		}
		if (emptyPage)
		{
			this.AssembleEmptyPageUI(page, !flag);
			CollectionManagerDisplay.Get().CollectionPageContentsChanged(null, delegate(List<Actor> actorList, object data)
			{
				page.UpdateCollectionCards(actorList, CollectionManagerDisplay.Get().GetViewMode(), this.IsShowingMassDisenchant());
				this.TransitionPage(transitionReadyCallbackData);
			}, null);
			return true;
		}
		return false;
	}

	// Token: 0x06002F45 RID: 12101 RVA: 0x000ED9C4 File Offset: 0x000EBBC4
	private void AssembleCardPage(CollectionPageManager.TransitionReadyCallbackData transitionReadyCallbackData, List<CollectibleCard> cardsToDisplay, int totalNumPages)
	{
		bool emptyPage = cardsToDisplay == null || cardsToDisplay.Count == 0;
		CollectionManagerDisplay.ViewMode viewMode = CollectionManagerDisplay.Get().GetViewMode();
		bool wildPage = viewMode != CollectionManagerDisplay.ViewMode.HERO_SKINS && CollectionManager.Get().IsShowingWildTheming(null);
		if (this.AssembleBasePage(transitionReadyCallbackData, emptyPage, wildPage))
		{
			return;
		}
		CollectionPageDisplay page = transitionReadyCallbackData.m_assembledPage;
		this.m_lastCardAnchor = cardsToDisplay[0];
		EntityDef entityDef = DefLoader.Get().GetEntityDef(cardsToDisplay[0].CardId);
		bool flag = CollectionManager.Get().GetTaggedDeck(CollectionManager.DeckTag.Editing) != null;
		if (viewMode == CollectionManagerDisplay.ViewMode.HERO_SKINS)
		{
			page.SetHeroSkins((!flag) ? TAG_CLASS.INVALID : entityDef.GetClass());
		}
		else
		{
			page.SetClass(new TAG_CLASS?(entityDef.GetClass()));
		}
		page.SetPageCountText(GameStrings.Format("GLUE_COLLECTION_PAGE_NUM", new object[]
		{
			this.m_currentPageNum
		}));
		page.ShowNoMatchesFound(false);
		this.ActivateArrows(this.m_currentPageNum > 1, this.m_currentPageNum < totalNumPages);
		CollectionManagerDisplay.Get().CollectionPageContentsChanged(cardsToDisplay, delegate(List<Actor> actorList, object data)
		{
			page.UpdateCollectionCards(actorList, CollectionManagerDisplay.Get().GetViewMode(), this.IsShowingMassDisenchant());
			this.TransitionPage(transitionReadyCallbackData);
			if (this.m_deckTemplatePicker != null)
			{
				this.StartCoroutine(this.m_deckTemplatePicker.Show(false));
			}
		}, null);
	}

	// Token: 0x06002F46 RID: 12102 RVA: 0x000EDB28 File Offset: 0x000EBD28
	private void AssembleDeckTemplatePage(CollectionPageManager.TransitionReadyCallbackData transitionReadyCallbackData)
	{
		bool emptyPage = this.m_currentPageNum == CollectionPageManager.MASS_DISENCHANT_PAGE_NUM;
		if (this.AssembleBasePage(transitionReadyCallbackData, emptyPage, false))
		{
			return;
		}
		CollectionPageDisplay assembledPage = transitionReadyCallbackData.m_assembledPage;
		if (this.m_deckTemplatePicker == null && !string.IsNullOrEmpty(this.m_deckTemplatePickerPrefab))
		{
			this.m_deckTemplatePicker = GameUtils.LoadGameObjectWithComponent<DeckTemplatePicker>(this.m_deckTemplatePickerPrefab);
			this.m_deckTemplatePicker.RegisterOnTemplateDeckChosen(delegate
			{
				this.HideNonDeckTemplateTabs(false, true);
				CollectionManagerDisplay.Get().SetViewMode(CollectionManagerDisplay.ViewMode.CARDS, null);
			});
		}
		assembledPage.UpdateDeckTemplatePage(this.m_deckTemplatePicker);
		assembledPage.SetDeckTemplates();
		assembledPage.ShowNoMatchesFound(false);
		assembledPage.SetPageCountText(string.Empty);
		this.ActivateArrows(false, false);
		this.UpdateDeckTemplate(this.m_deckTemplatePicker);
		this.TransitionPage(transitionReadyCallbackData);
	}

	// Token: 0x06002F47 RID: 12103 RVA: 0x000EDBE1 File Offset: 0x000EBDE1
	public DeckTemplatePicker GetDeckTemplatePicker()
	{
		return this.m_deckTemplatePicker;
	}

	// Token: 0x06002F48 RID: 12104 RVA: 0x000EDBEC File Offset: 0x000EBDEC
	public void UpdateDeckTemplate(DeckTemplatePicker deckTemplatePicker)
	{
		if (deckTemplatePicker != null)
		{
			CollectionDeck taggedDeck = CollectionManager.Get().GetTaggedDeck(CollectionManager.DeckTag.Editing);
			if (taggedDeck != null)
			{
				deckTemplatePicker.SetDeckClass(taggedDeck.GetClass());
			}
			base.StartCoroutine(deckTemplatePicker.Show(true));
		}
	}

	// Token: 0x06002F49 RID: 12105 RVA: 0x000EDC34 File Offset: 0x000EBE34
	private void AssembleCardBackPage(CollectionPageManager.TransitionReadyCallbackData transitionReadyCallbackData)
	{
		bool emptyPage = this.m_currentPageNum == CollectionPageManager.MASS_DISENCHANT_PAGE_NUM;
		if (this.AssembleBasePage(transitionReadyCallbackData, emptyPage, false))
		{
			return;
		}
		CollectionPageDisplay page = transitionReadyCallbackData.m_assembledPage;
		int count = this.GetCurrentDeckTrayModeCardBackIds().Count;
		int maxCardsPerPage = CollectionPageDisplay.GetMaxCardsPerPage();
		int num = count / maxCardsPerPage + ((count % maxCardsPerPage <= 0) ? 0 : 1);
		this.m_currentPageNum = Mathf.Clamp(this.m_currentPageNum, 1, num);
		page.SetCardBacks();
		page.ShowNoMatchesFound(false);
		page.SetPageCountText(GameStrings.Format("GLUE_COLLECTION_PAGE_NUM", new object[]
		{
			this.m_currentPageNum
		}));
		this.ActivateArrows(this.m_currentPageNum > 1, this.m_currentPageNum < num);
		CollectionManagerDisplay.Get().CollectionPageContentsChangedToCardBacks(this.m_currentPageNum, maxCardsPerPage, delegate(List<Actor> actorList, object data)
		{
			page.UpdateCollectionCards(actorList, CollectionManagerDisplay.Get().GetViewMode(), this.IsShowingMassDisenchant());
			foreach (Actor actor in actorList)
			{
				CardBackManager.Get().UpdateCardBackWithInternalCardBack(actor);
			}
			this.TransitionPage(transitionReadyCallbackData);
			if (this.m_deckTemplatePicker != null)
			{
				this.StartCoroutine(this.m_deckTemplatePicker.Show(false));
			}
		}, null, !CollectionManager.Get().IsInEditMode());
	}

	// Token: 0x06002F4A RID: 12106 RVA: 0x000EDD50 File Offset: 0x000EBF50
	private IEnumerator ShowMassDisenchantPage()
	{
		yield return new WaitForEndOfFrame();
		this.LoadMassDisenchantScreen();
		this.m_massDisenchant.Show();
		yield break;
	}

	// Token: 0x06002F4B RID: 12107 RVA: 0x000EDD6B File Offset: 0x000EBF6B
	private void PositionCurrentPage(CollectionPageDisplay page)
	{
		page.transform.localPosition = CollectionPageManager.CURRENT_PAGE_LOCAL_POS;
	}

	// Token: 0x06002F4C RID: 12108 RVA: 0x000EDD7D File Offset: 0x000EBF7D
	private void PositionNextPage(CollectionPageDisplay page)
	{
		page.transform.localPosition = CollectionPageManager.NEXT_PAGE_LOCAL_POS;
	}

	// Token: 0x06002F4D RID: 12109 RVA: 0x000EDD90 File Offset: 0x000EBF90
	private void TransitionPageWhenReady(CollectionPageManager.PageTransitionType transitionType, bool useCurrentPageNum, CollectionPageManager.DelOnPageTransitionComplete callback, object callbackData)
	{
		if (HeroPickerDisplay.Get() != null && HeroPickerDisplay.Get().IsShown())
		{
			transitionType = CollectionPageManager.PageTransitionType.NONE;
		}
		this.m_pagesCurrentlyTurning = true;
		this.SwapCurrentAndAltPages();
		CollectionPageManager.TransitionReadyCallbackData transitionReadyCallbackData = new CollectionPageManager.TransitionReadyCallbackData
		{
			m_assembledPage = this.GetCurrentPage(),
			m_otherPage = this.GetAlternatePage(),
			m_transitionType = transitionType,
			m_callback = callback,
			m_callbackData = callbackData
		};
		switch (transitionType)
		{
		case CollectionPageManager.PageTransitionType.SINGLE_PAGE_RIGHT:
		case CollectionPageManager.PageTransitionType.MANY_PAGE_RIGHT:
			SoundManager.Get().LoadAndPlay("collection_manager_book_page_flip_forward");
			break;
		case CollectionPageManager.PageTransitionType.SINGLE_PAGE_LEFT:
		case CollectionPageManager.PageTransitionType.MANY_PAGE_LEFT:
			SoundManager.Get().LoadAndPlay("collection_manager_book_page_flip_back");
			break;
		}
		CollectionManagerDisplay.ViewMode viewMode = CollectionManagerDisplay.Get().GetViewMode();
		if (viewMode == CollectionManagerDisplay.ViewMode.CARD_BACKS)
		{
			if (this.m_currentPageNum < 1)
			{
				this.m_currentPageNum = 1;
			}
			this.AssembleCardBackPage(transitionReadyCallbackData);
		}
		else if (viewMode == CollectionManagerDisplay.ViewMode.DECK_TEMPLATE)
		{
			this.AssembleDeckTemplatePage(transitionReadyCallbackData);
		}
		else if (viewMode == CollectionManagerDisplay.ViewMode.HERO_SKINS)
		{
			List<CollectibleCard> heroesContents = this.m_heroesCollection.GetHeroesContents(this.m_currentPageNum);
			this.AssembleCardPage(transitionReadyCallbackData, heroesContents, this.m_heroesCollection.GetTotalNumPages());
		}
		else if (viewMode == CollectionManagerDisplay.ViewMode.CARDS)
		{
			List<CollectibleCard> list = null;
			if (useCurrentPageNum)
			{
				list = this.m_cardsCollection.GetPageContents(this.m_currentPageNum);
			}
			else if (this.m_currentPageNum != CollectionPageManager.MASS_DISENCHANT_PAGE_NUM)
			{
				if (this.m_lastCardAnchor == null)
				{
					this.m_currentPageNum = 1;
					list = this.m_cardsCollection.GetPageContents(this.m_currentPageNum);
				}
				else
				{
					int num;
					list = this.m_cardsCollection.GetPageContentsForCard(this.m_lastCardAnchor.CardId, this.m_lastCardAnchor.PremiumType, out num);
					if (list.Count == 0)
					{
						list = this.m_cardsCollection.GetPageContentsForClass(this.m_lastCardAnchor.Class, 1, true, out num);
					}
					if (list.Count == 0)
					{
						list = this.m_cardsCollection.GetPageContents(1);
						num = 1;
					}
					this.m_currentPageNum = ((list.Count != 0) ? num : 0);
				}
			}
			if (CollectionPageManager.MASS_DISENCHANT_PAGE_NUM != this.m_currentPageNum && (list == null || list.Count == 0))
			{
				int currentPageNum;
				list = this.m_cardsCollection.GetFirstNonEmptyPage(out currentPageNum);
				if (list.Count > 0)
				{
					this.m_currentPageNum = currentPageNum;
				}
			}
			this.AssembleCardPage(transitionReadyCallbackData, list, this.m_cardsCollection.GetTotalNumPages());
		}
	}

	// Token: 0x06002F4E RID: 12110 RVA: 0x000EDFFC File Offset: 0x000EC1FC
	private void TransitionPage(object callbackData)
	{
		CollectionPageManager.TransitionReadyCallbackData transitionReadyCallbackData = callbackData as CollectionPageManager.TransitionReadyCallbackData;
		CollectionPageDisplay assembledPage = transitionReadyCallbackData.m_assembledPage;
		CollectionPageDisplay otherPage = transitionReadyCallbackData.m_otherPage;
		this.m_pageTurn.SetBackPageMaterial(assembledPage.m_basePageRenderer.material);
		if (this.ANIMATE_PAGE_TRANSITIONS)
		{
			CollectionPageManager.PageTransitionType pageTransitionType = transitionReadyCallbackData.m_transitionType;
			if (TavernBrawlDisplay.IsTavernBrawlViewing())
			{
				pageTransitionType = CollectionPageManager.PageTransitionType.NONE;
			}
			if (this.m_skipNextPageTurn)
			{
				pageTransitionType = CollectionPageManager.PageTransitionType.NONE;
				this.m_skipNextPageTurn = false;
			}
			switch (pageTransitionType)
			{
			case CollectionPageManager.PageTransitionType.NONE:
				this.PositionNextPage(otherPage);
				this.PositionCurrentPage(assembledPage);
				this.OnPageTurnComplete(transitionReadyCallbackData);
				break;
			case CollectionPageManager.PageTransitionType.SINGLE_PAGE_RIGHT:
			case CollectionPageManager.PageTransitionType.MANY_PAGE_RIGHT:
				this.m_pageTurn.TurnRight(otherPage.gameObject, assembledPage.gameObject, new PageTurn.DelOnPageTurnComplete(this.OnPageTurnComplete), transitionReadyCallbackData);
				this.PositionCurrentPage(assembledPage);
				this.PositionNextPage(otherPage);
				break;
			case CollectionPageManager.PageTransitionType.SINGLE_PAGE_LEFT:
			case CollectionPageManager.PageTransitionType.MANY_PAGE_LEFT:
				this.m_pageTurn.TurnLeft(assembledPage.gameObject, otherPage.gameObject, new PageTurn.DelOnPageTurnComplete(this.OnPageTurnComplete), transitionReadyCallbackData);
				break;
			}
		}
		this.UpdateVisibleTabs();
		if (CollectionPageManager.MASS_DISENCHANT_PAGE_NUM == this.m_currentPageNum)
		{
			this.DeselectCurrentClassTab();
		}
		else
		{
			this.SetCurrentClassTab(assembledPage.GetFirstCardClass());
		}
		if (!this.ANIMATE_PAGE_TRANSITIONS)
		{
			this.PositionNextPage(otherPage);
			this.PositionCurrentPage(assembledPage);
			this.OnPageTurnComplete(transitionReadyCallbackData);
		}
	}

	// Token: 0x06002F4F RID: 12111 RVA: 0x000EE15C File Offset: 0x000EC35C
	private void OnPageTurnComplete(object callbackData)
	{
		Resources.UnloadUnusedAssets();
		CollectionPageManager.TransitionReadyCallbackData transitionReadyCallbackData = callbackData as CollectionPageManager.TransitionReadyCallbackData;
		CollectionPageDisplay assembledPage = transitionReadyCallbackData.m_assembledPage;
		CollectionPageDisplay otherPage = transitionReadyCallbackData.m_otherPage;
		switch (transitionReadyCallbackData.m_transitionType)
		{
		case CollectionPageManager.PageTransitionType.SINGLE_PAGE_LEFT:
		case CollectionPageManager.PageTransitionType.MANY_PAGE_LEFT:
			this.PositionCurrentPage(assembledPage);
			this.PositionNextPage(otherPage);
			break;
		}
		if (otherPage != this.GetCurrentPage())
		{
			otherPage.transform.position = new Vector3(-300f, 0f, -300f);
		}
		if (transitionReadyCallbackData.m_callback != null)
		{
			transitionReadyCallbackData.m_callback(transitionReadyCallbackData.m_callbackData);
		}
		if (transitionReadyCallbackData.m_assembledPage == this.GetCurrentPage())
		{
			this.m_pagesCurrentlyTurning = false;
		}
	}

	// Token: 0x06002F50 RID: 12112 RVA: 0x000EE230 File Offset: 0x000EC430
	private void UpdateFilteredHeroes()
	{
		this.m_heroesCollection.UpdateResults();
	}

	// Token: 0x06002F51 RID: 12113 RVA: 0x000EE23D File Offset: 0x000EC43D
	private void UpdateFilteredCards()
	{
		this.m_cardsCollection.UpdateResults();
		this.UpdateClassTabNewCardCounts();
	}

	// Token: 0x06002F52 RID: 12114 RVA: 0x000EE250 File Offset: 0x000EC450
	private void PageRight(CollectionPageManager.DelOnPageTransitionComplete callback, object callbackData)
	{
		this.m_currentPageNum++;
		this.TransitionPageWhenReady(CollectionPageManager.PageTransitionType.SINGLE_PAGE_RIGHT, true, callback, callbackData);
	}

	// Token: 0x06002F53 RID: 12115 RVA: 0x000EE26A File Offset: 0x000EC46A
	private void PageLeft(CollectionPageManager.DelOnPageTransitionComplete callback, object callbackData)
	{
		this.m_currentPageNum--;
		this.TransitionPageWhenReady(CollectionPageManager.PageTransitionType.SINGLE_PAGE_LEFT, true, callback, callbackData);
	}

	// Token: 0x06002F54 RID: 12116 RVA: 0x000EE284 File Offset: 0x000EC484
	private void ActivateArrows(bool leftArrow, bool rightArrow)
	{
		this.m_pageLeftClickableRegion.enabled = leftArrow;
		this.m_pageLeftClickableRegion.SetEnabled(leftArrow);
		this.m_pageRightClickableRegion.enabled = rightArrow;
		this.m_pageRightClickableRegion.SetEnabled(rightArrow);
		this.ShowArrow(this.m_pageLeftArrow, leftArrow, false);
		this.ShowArrow(this.m_pageRightArrow, rightArrow, true);
	}

	// Token: 0x06002F55 RID: 12117 RVA: 0x000EE2DD File Offset: 0x000EC4DD
	private void OnPageLeftPressed(UIEvent e)
	{
		this.OnPageFlip();
		this.PageLeft(null, null);
	}

	// Token: 0x06002F56 RID: 12118 RVA: 0x000EE2F0 File Offset: 0x000EC4F0
	private void OnPageFlip()
	{
		int @int = Options.Get().GetInt(Option.PAGE_MOUSE_OVERS);
		int val = @int + 1;
		if (@int < CollectionPageManager.NUM_PAGE_FLIPS_BEFORE_STOP_SHOWING_ARROWS)
		{
			Options.Get().SetInt(Option.PAGE_MOUSE_OVERS, val);
		}
	}

	// Token: 0x06002F57 RID: 12119 RVA: 0x000EE326 File Offset: 0x000EC526
	private void OnPageRightPressed(UIEvent e)
	{
		this.OnPageFlip();
		this.PageRight(null, null);
	}

	// Token: 0x06002F58 RID: 12120 RVA: 0x000EE338 File Offset: 0x000EC538
	public void LoadPagingArrows()
	{
		int @int = Options.Get().GetInt(Option.PAGE_MOUSE_OVERS);
		if (@int >= CollectionPageManager.NUM_PAGE_FLIPS_BEFORE_STOP_SHOWING_ARROWS & !this.ALWAYS_SHOW_PAGING_ARROWS)
		{
			return;
		}
		if (this.m_pageLeftArrow && this.m_pageRightArrow)
		{
			return;
		}
		AssetLoader.Get().LoadGameObject("PagingArrow", new AssetLoader.GameObjectCallback(this.OnPagingArrowLoaded), null, false);
	}

	// Token: 0x06002F59 RID: 12121 RVA: 0x000EE3B4 File Offset: 0x000EC5B4
	private void OnPagingArrowLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			return;
		}
		if (!this.m_pageLeftArrow)
		{
			this.m_pageLeftArrow = go;
			this.m_pageLeftArrow.transform.parent = base.transform;
			this.m_pageLeftArrow.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
			this.m_pageLeftArrow.transform.position = this.m_pageLeftArrowBone.transform.position;
			this.m_pageLeftArrow.transform.localScale = Vector3.zero;
			bool enabled = this.m_pageLeftClickableRegion.enabled;
			SceneUtils.SetLayer(this.m_pageLeftArrow, GameLayer.TransparentFX);
			this.ShowArrow(this.m_pageLeftArrow, enabled, false);
		}
		if (!this.m_pageRightArrow)
		{
			this.m_pageRightArrow = Object.Instantiate<GameObject>(this.m_pageLeftArrow);
			this.m_pageRightArrow.transform.parent = base.transform;
			this.m_pageRightArrow.transform.localEulerAngles = Vector3.zero;
			this.m_pageRightArrow.transform.position = this.m_pageRightArrowBone.transform.position;
			this.m_pageRightArrow.transform.localScale = Vector3.zero;
			bool enabled2 = this.m_pageRightClickableRegion.enabled;
			SceneUtils.SetLayer(this.m_pageRightArrow, GameLayer.TransparentFX);
			this.ShowArrow(this.m_pageRightArrow, enabled2, true);
		}
	}

	// Token: 0x06002F5A RID: 12122 RVA: 0x000EE524 File Offset: 0x000EC724
	private IEnumerator WaitThenShowArrows()
	{
		if (this.m_pageLeftArrow == null && this.m_pageRightArrow == null)
		{
			yield break;
		}
		this.m_delayShowingArrows = true;
		yield return new WaitForSeconds(1f);
		this.m_delayShowingArrows = false;
		bool showLeftArrow = this.m_pageLeftClickableRegion.enabled;
		this.ShowArrow(this.m_pageLeftArrow, showLeftArrow, false);
		bool showRightArrow = this.m_pageRightClickableRegion.enabled;
		this.ShowArrow(this.m_pageRightArrow, showRightArrow, true);
		yield break;
	}

	// Token: 0x06002F5B RID: 12123 RVA: 0x000EE540 File Offset: 0x000EC740
	private void ShowArrow(GameObject arrow, bool show, bool isRightArrow)
	{
		if (arrow == null)
		{
			return;
		}
		if (this.m_delayShowingArrows)
		{
			return;
		}
		if (isRightArrow)
		{
			if (this.m_rightArrowShown == show)
			{
				return;
			}
			this.m_rightArrowShown = show;
		}
		else
		{
			if (this.m_leftArrowShown == show)
			{
				return;
			}
			this.m_leftArrowShown = show;
		}
		GameObject gameObject = (!isRightArrow) ? this.m_pageLeftArrowBone : this.m_pageRightArrowBone;
		Vector3 localScale = gameObject.transform.localScale;
		Vector3 vector = (!show) ? Vector3.zero : localScale;
		iTween.EaseType easeType = (!show) ? iTween.EaseType.linear : iTween.EaseType.easeOutElastic;
		Hashtable args = iTween.Hash(new object[]
		{
			"scale",
			vector,
			"time",
			CollectionPageManager.ARROW_SCALE_TIME,
			"easetype",
			easeType,
			"name",
			"ArrowScale"
		});
		iTween.StopByName(arrow, "ArrowScale");
		iTween.ScaleTo(arrow, args);
	}

	// Token: 0x06002F5C RID: 12124 RVA: 0x000EE648 File Offset: 0x000EC848
	private void OnCollectionManagerViewModeChanged(CollectionManagerDisplay.ViewMode prevMode, CollectionManagerDisplay.ViewMode mode, CollectionManagerDisplay.ViewModeData userdata, bool triggerResponse)
	{
		if (!triggerResponse)
		{
			return;
		}
		this.UpdateCraftingModeButtonDustBottleVisibility();
		if (mode == CollectionManagerDisplay.ViewMode.DECK_TEMPLATE)
		{
			this.HideNonDeckTemplateTabs(true, false);
		}
		if (mode != CollectionManagerDisplay.ViewMode.CARDS)
		{
			CollectionDeckTray.Get().GetCardsContent().HideDeckHelpPopup();
		}
		if (this.m_useLastPage)
		{
			this.m_useLastPage = false;
			this.m_currentPageNum = this.GetLastPageInCurrentMode();
		}
		else
		{
			this.m_currentPageNum = 1;
			if (userdata != null)
			{
				TAG_CLASS? setPageByClass = userdata.m_setPageByClass;
				if (setPageByClass != null)
				{
					this.m_cardsCollection.GetPageContentsForClass(userdata.m_setPageByClass.Value, 1, true, out this.m_currentPageNum);
				}
				else if (userdata.m_setPageByCard != null)
				{
					this.m_cardsCollection.GetPageContentsForCard(userdata.m_setPageByCard, userdata.m_setPageByPremium, out this.m_currentPageNum);
				}
			}
		}
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < CollectionPageManager.TAG_ORDERING.Length; i++)
		{
			if (prevMode == CollectionPageManager.TAG_ORDERING[i])
			{
				num = i;
			}
			if (mode == CollectionPageManager.TAG_ORDERING[i])
			{
				num2 = i;
			}
		}
		CollectionPageManager.PageTransitionType transition = (num2 - num >= 0) ? CollectionPageManager.PageTransitionType.SINGLE_PAGE_RIGHT : CollectionPageManager.PageTransitionType.SINGLE_PAGE_LEFT;
		CollectionPageManager.DelOnPageTransitionComplete callback = null;
		object callbackData = null;
		if (userdata != null)
		{
			callback = userdata.m_pageTransitionCompleteCallback;
			callbackData = userdata.m_pageTransitionCompleteData;
		}
		if (this.m_turnPageCoroutine != null)
		{
			base.StopCoroutine(this.m_turnPageCoroutine);
		}
		CollectionDeckTray.Get().m_decksContent.UpdateDeckName(null);
		CollectionDeckTray.Get().UpdateDoneButtonText();
		this.m_turnPageCoroutine = base.StartCoroutine(this.ViewModeChangedWaitToTurnPage(transition, prevMode == CollectionManagerDisplay.ViewMode.DECK_TEMPLATE, callback, callbackData));
	}

	// Token: 0x06002F5D RID: 12125 RVA: 0x000EE7CC File Offset: 0x000EC9CC
	private IEnumerator ViewModeChangedWaitToTurnPage(CollectionPageManager.PageTransitionType transition, bool hideDeckTemplateBottomPanel, CollectionPageManager.DelOnPageTransitionComplete callback, object callbackData)
	{
		if (this.m_deckTemplatePicker != null && hideDeckTemplateBottomPanel)
		{
			CollectionManagerDisplay.Get().m_inputBlocker.gameObject.SetActive(true);
			this.m_deckTemplatePicker.ShowBottomPanel(false);
			while (this.m_deckTemplatePicker.IsShowingBottomPanel())
			{
				yield return null;
			}
			yield return base.StartCoroutine(this.m_deckTemplatePicker.ShowPacks(false));
			CollectionManagerDisplay.Get().m_inputBlocker.gameObject.SetActive(false);
		}
		this.TransitionPageWhenReady(transition, true, callback, callbackData);
		yield break;
	}

	// Token: 0x06002F5E RID: 12126 RVA: 0x000EE824 File Offset: 0x000ECA24
	public void OnFavoriteHeroChanged(TAG_CLASS heroClass, NetCache.CardDefinition favoriteHero, object userData)
	{
		CollectionPageDisplay currentPage = this.GetCurrentPage();
		currentPage.UpdateFavoriteHeroSkins(CollectionManagerDisplay.Get().GetViewMode(), this.IsShowingMassDisenchant());
	}

	// Token: 0x06002F5F RID: 12127 RVA: 0x000EE850 File Offset: 0x000ECA50
	public void OnDefaultCardbackChanged(int newDefaultCardBackID, object userData)
	{
		CollectionPageDisplay currentPage = this.GetCurrentPage();
		currentPage.UpdateFavoriteCardBack(CollectionManagerDisplay.Get().GetViewMode());
	}

	// Token: 0x06002F60 RID: 12128 RVA: 0x000EE874 File Offset: 0x000ECA74
	private int GetLastPageInCurrentMode()
	{
		switch (CollectionManagerDisplay.Get().GetViewMode())
		{
		case CollectionManagerDisplay.ViewMode.CARD_BACKS:
		{
			int count = this.GetCurrentDeckTrayModeCardBackIds().Count;
			int maxCardsPerPage = CollectionPageDisplay.GetMaxCardsPerPage();
			return count / maxCardsPerPage + ((count % maxCardsPerPage <= 0) ? 0 : 1);
		}
		}
		return this.m_cardsCollection.GetTotalNumPages();
	}

	// Token: 0x06002F61 RID: 12129 RVA: 0x000EE8DC File Offset: 0x000ECADC
	private HashSet<int> GetCurrentDeckTrayModeCardBackIds()
	{
		return CardBackManager.Get().GetCardBackIds(!CollectionManager.Get().IsInEditMode());
	}

	// Token: 0x04001D2C RID: 7468
	public static readonly Map<TAG_CLASS, Vector2> s_classTextureOffsets = new Map<TAG_CLASS, Vector2>
	{
		{
			TAG_CLASS.MAGE,
			new Vector2(0f, 0f)
		},
		{
			TAG_CLASS.PALADIN,
			new Vector2(0.205f, 0f)
		},
		{
			TAG_CLASS.PRIEST,
			new Vector2(0.392f, 0f)
		},
		{
			TAG_CLASS.ROGUE,
			new Vector2(0.58f, 0f)
		},
		{
			TAG_CLASS.SHAMAN,
			new Vector2(0.774f, 0f)
		},
		{
			TAG_CLASS.WARLOCK,
			new Vector2(0f, -0.2f)
		},
		{
			TAG_CLASS.WARRIOR,
			new Vector2(0.205f, -0.2f)
		},
		{
			TAG_CLASS.DRUID,
			new Vector2(0.392f, -0.2f)
		},
		{
			TAG_CLASS.HUNTER,
			new Vector2(0.58f, -0.2f)
		},
		{
			TAG_CLASS.INVALID,
			new Vector2(0.774f, -0.2f)
		}
	};

	// Token: 0x04001D2D RID: 7469
	public static readonly Map<TAG_CLASS, Color> s_classColors = new Map<TAG_CLASS, Color>
	{
		{
			TAG_CLASS.MAGE,
			new Color(0.12941177f, 0.26666668f, 0.3882353f)
		},
		{
			TAG_CLASS.PALADIN,
			new Color(0.4392157f, 0.29411766f, 0.09019608f)
		},
		{
			TAG_CLASS.PRIEST,
			new Color(0.52156866f, 0.52156866f, 0.52156866f)
		},
		{
			TAG_CLASS.ROGUE,
			new Color(0.09019608f, 0.07450981f, 0.07450981f)
		},
		{
			TAG_CLASS.SHAMAN,
			new Color(0.12941177f, 0.17254902f, 0.37254903f)
		},
		{
			TAG_CLASS.WARLOCK,
			new Color(0.21176471f, 0.10980392f, 0.28235295f)
		},
		{
			TAG_CLASS.WARRIOR,
			new Color(0.27450982f, 0.050980393f, 0.08235294f)
		},
		{
			TAG_CLASS.DRUID,
			new Color(0.23137255f, 0.16078432f, 0.08627451f)
		},
		{
			TAG_CLASS.HUNTER,
			new Color(0.07450981f, 0.23137255f, 0.06666667f)
		},
		{
			TAG_CLASS.INVALID,
			new Color(0f, 0f, 0f)
		}
	};

	// Token: 0x04001D2E RID: 7470
	public GameObject m_classTabContainer;

	// Token: 0x04001D2F RID: 7471
	public CollectionClassTab m_classTabPrefab;

	// Token: 0x04001D30 RID: 7472
	public GameObject m_pageRightArrowBone;

	// Token: 0x04001D31 RID: 7473
	public GameObject m_pageLeftArrowBone;

	// Token: 0x04001D32 RID: 7474
	public PegUIElement m_pageRightClickableRegion;

	// Token: 0x04001D33 RID: 7475
	public PegUIElement m_pageLeftClickableRegion;

	// Token: 0x04001D34 RID: 7476
	public PegUIElement m_pageDraggableRegion;

	// Token: 0x04001D35 RID: 7477
	public CollectionPageDisplay m_pageDisplayPrefab;

	// Token: 0x04001D36 RID: 7478
	public PageTurn m_pageTurn;

	// Token: 0x04001D37 RID: 7479
	public float m_turnLeftPageSwapTiming;

	// Token: 0x04001D38 RID: 7480
	public float m_spaceBetweenTabs;

	// Token: 0x04001D39 RID: 7481
	public CollectionClassTab m_heroSkinsTab;

	// Token: 0x04001D3A RID: 7482
	public CollectionClassTab m_cardBacksTab;

	// Token: 0x04001D3B RID: 7483
	public ClassFilterHeaderButton m_classFilterHeader;

	// Token: 0x04001D3C RID: 7484
	public CollectionClassTab m_deckTemplateTab;

	// Token: 0x04001D3D RID: 7485
	[CustomEditField(Sections = "Deck Template", T = EditType.GAME_OBJECT)]
	public string m_deckTemplatePickerPrefab;

	// Token: 0x04001D3E RID: 7486
	private static TAG_CLASS[] CLASS_TAB_ORDER;

	// Token: 0x04001D3F RID: 7487
	private static CollectionManagerDisplay.ViewMode[] TAG_ORDERING;

	// Token: 0x04001D40 RID: 7488
	public static readonly float SELECT_TAB_ANIM_TIME;

	// Token: 0x04001D41 RID: 7489
	private static readonly Vector3 CURRENT_PAGE_LOCAL_POS;

	// Token: 0x04001D42 RID: 7490
	private static readonly Vector3 NEXT_PAGE_LOCAL_POS;

	// Token: 0x04001D43 RID: 7491
	private static readonly Vector3 CLASS_TAB_LOCAL_EULERS;

	// Token: 0x04001D44 RID: 7492
	private static readonly float HIDDEN_TAB_LOCAL_Z_POS;

	// Token: 0x04001D45 RID: 7493
	private static readonly float ARROW_SCALE_TIME;

	// Token: 0x04001D46 RID: 7494
	private static readonly string ANIMATE_TABS_COROUTINE_NAME;

	// Token: 0x04001D47 RID: 7495
	private static readonly string SELECT_TAB_COROUTINE_NAME;

	// Token: 0x04001D48 RID: 7496
	private static readonly string SHOW_ARROWS_COROUTINE_NAME;

	// Token: 0x04001D49 RID: 7497
	private static readonly int NUM_PAGE_FLIPS_BEFORE_STOP_SHOWING_ARROWS;

	// Token: 0x04001D4A RID: 7498
	private static readonly int MASS_DISENCHANT_PAGE_NUM;

	// Token: 0x04001D4B RID: 7499
	private CollectionPageDisplay m_pageA;

	// Token: 0x04001D4C RID: 7500
	private CollectionPageDisplay m_pageB;

	// Token: 0x04001D4D RID: 7501
	private static Map<TAG_CLASS, int> CLASS_TO_TAB_IDX;

	// Token: 0x04001D4E RID: 7502
	private bool m_currentPageIsPageA;

	// Token: 0x04001D4F RID: 7503
	private int m_currentPageNum;

	// Token: 0x04001D50 RID: 7504
	private int m_lastPageNum;

	// Token: 0x04001D51 RID: 7505
	private List<CollectionClassTab> m_classTabs = new List<CollectionClassTab>();

	// Token: 0x04001D52 RID: 7506
	private List<CollectionClassTab> m_allTabs = new List<CollectionClassTab>();

	// Token: 0x04001D53 RID: 7507
	private Map<CollectionClassTab, bool> m_tabVisibility = new Map<CollectionClassTab, bool>();

	// Token: 0x04001D54 RID: 7508
	private CollectionClassTab m_currentClassTab;

	// Token: 0x04001D55 RID: 7509
	private bool m_tabsAreAnimating;

	// Token: 0x04001D56 RID: 7510
	private CollectibleCard m_lastCardAnchor;

	// Token: 0x04001D57 RID: 7511
	private GameObject m_pageRightArrow;

	// Token: 0x04001D58 RID: 7512
	private GameObject m_pageLeftArrow;

	// Token: 0x04001D59 RID: 7513
	private bool m_rightArrowShown;

	// Token: 0x04001D5A RID: 7514
	private bool m_leftArrowShown;

	// Token: 0x04001D5B RID: 7515
	private bool m_delayShowingArrows;

	// Token: 0x04001D5C RID: 7516
	private float m_deselectedClassTabHalfWidth;

	// Token: 0x04001D5D RID: 7517
	private bool m_initializedTabPositions;

	// Token: 0x04001D5E RID: 7518
	private bool m_fullyLoaded;

	// Token: 0x04001D5F RID: 7519
	private MassDisenchant m_massDisenchant;

	// Token: 0x04001D60 RID: 7520
	private DeckTemplatePicker m_deckTemplatePicker;

	// Token: 0x04001D61 RID: 7521
	private CollectibleCardClassFilter m_cardsCollection = new CollectibleCardClassFilter();

	// Token: 0x04001D62 RID: 7522
	private CollectibleCardHeroesFilter m_heroesCollection = new CollectibleCardHeroesFilter();

	// Token: 0x04001D63 RID: 7523
	private bool m_wasTouchModeEnabled;

	// Token: 0x04001D64 RID: 7524
	private bool m_useLastPage;

	// Token: 0x04001D65 RID: 7525
	private bool m_skipNextPageTurn;

	// Token: 0x04001D66 RID: 7526
	private bool m_pagesCurrentlyTurning;

	// Token: 0x04001D67 RID: 7527
	private Vector3 m_heroSkinsTabPos;

	// Token: 0x04001D68 RID: 7528
	private Vector3 m_cardBacksTabPos;

	// Token: 0x04001D69 RID: 7529
	private bool m_hideNonDeckTemplateTabs;

	// Token: 0x04001D6A RID: 7530
	private readonly PlatformDependentValue<bool> ANIMATE_PAGE_TRANSITIONS = new PlatformDependentValue<bool>(PlatformCategory.OS)
	{
		iOS = true,
		Android = true,
		PC = true,
		Mac = true
	};

	// Token: 0x04001D6B RID: 7531
	private readonly PlatformDependentValue<bool> ALWAYS_SHOW_PAGING_ARROWS = new PlatformDependentValue<bool>(PlatformCategory.OS)
	{
		iOS = true,
		Android = true,
		PC = false,
		Mac = false
	};

	// Token: 0x04001D6C RID: 7532
	private Coroutine m_turnPageCoroutine;

	// Token: 0x020005C3 RID: 1475
	// (Invoke) Token: 0x060041E7 RID: 16871
	public delegate void DelOnPageTransitionComplete(object callbackData);

	// Token: 0x020006F5 RID: 1781
	private enum ArrowClickType
	{
		// Token: 0x04003084 RID: 12420
		DISABLED,
		// Token: 0x04003085 RID: 12421
		ENABLED,
		// Token: 0x04003086 RID: 12422
		SWITCH_MODE
	}

	// Token: 0x020006F6 RID: 1782
	public enum PageTransitionType
	{
		// Token: 0x04003088 RID: 12424
		NONE,
		// Token: 0x04003089 RID: 12425
		SINGLE_PAGE_RIGHT,
		// Token: 0x0400308A RID: 12426
		SINGLE_PAGE_LEFT,
		// Token: 0x0400308B RID: 12427
		MANY_PAGE_RIGHT,
		// Token: 0x0400308C RID: 12428
		MANY_PAGE_LEFT
	}

	// Token: 0x020006F7 RID: 1783
	private class TransitionReadyCallbackData
	{
		// Token: 0x0400308D RID: 12429
		public CollectionPageDisplay m_assembledPage;

		// Token: 0x0400308E RID: 12430
		public CollectionPageDisplay m_otherPage;

		// Token: 0x0400308F RID: 12431
		public CollectionPageManager.PageTransitionType m_transitionType;

		// Token: 0x04003090 RID: 12432
		public CollectionPageManager.DelOnPageTransitionComplete m_callback;

		// Token: 0x04003091 RID: 12433
		public object m_callbackData;
	}
}
