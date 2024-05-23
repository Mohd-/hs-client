using System;
using System.Collections.Generic;
using System.Linq;
using PegasusShared;
using PegasusUtil;
using UnityEngine;

// Token: 0x0200015E RID: 350
public class CollectionManager
{
	// Token: 0x06001260 RID: 4704 RVA: 0x0004FDCC File Offset: 0x0004DFCC
	private void UpdateCardsWithNetData()
	{
		if (this.m_cardStacksRegistered)
		{
			return;
		}
		NetCache.NetCacheCollection netObject = NetCache.Get().GetNetObject<NetCache.NetCacheCollection>();
		foreach (NetCache.CardStack cardStack in netObject.Stacks)
		{
			string name = cardStack.Def.Name;
			if (GameUtils.IsCardCollectible(name))
			{
				EntityDef entityDef = DefLoader.Get().GetEntityDef(name);
				this.AddCounts(cardStack, entityDef);
			}
		}
		NetCache.NetCacheCardValues netObject2 = NetCache.Get().GetNetObject<NetCache.NetCacheCardValues>();
		if (netObject2 != null && netObject2.Values.Count > 0)
		{
			foreach (KeyValuePair<NetCache.CardDefinition, NetCache.CardValue> keyValuePair in netObject2.Values)
			{
				CollectibleCard card = this.GetCard(keyValuePair.Key.Name, keyValuePair.Key.Premium);
				if (card != null)
				{
					card.CraftBuyCost = keyValuePair.Value.Buy;
					card.CraftSellCost = keyValuePair.Value.Sell;
				}
			}
		}
		this.m_cardStacksRegistered = true;
	}

	// Token: 0x06001261 RID: 4705 RVA: 0x0004FF28 File Offset: 0x0004E128
	private void OnCardSale()
	{
		Network.CardSaleResult cardSaleResult = Network.GetCardSaleResult();
		bool flag;
		switch (cardSaleResult.Action)
		{
		case Network.CardSaleResult.SaleResult.GENERIC_FAILURE:
			CraftingManager.Get().OnCardGenericError(cardSaleResult);
			flag = false;
			break;
		case Network.CardSaleResult.SaleResult.CARD_WAS_SOLD:
			for (int i = 1; i <= cardSaleResult.Count; i++)
			{
				this.RemoveCollectionCard(cardSaleResult.AssetName, cardSaleResult.Premium, 1);
			}
			CraftingManager.Get().OnCardDisenchanted(cardSaleResult);
			if (AchieveManager.Get().HasIncompleteDisenchantAchieves())
			{
				AchieveManager.Get().UpdateActiveAchieves(new AchieveManager.ActiveAchievesUpdatedCallback(this.OnActiveAchievesUpdated));
			}
			flag = true;
			break;
		case Network.CardSaleResult.SaleResult.CARD_WAS_BOUGHT:
			for (int j = 1; j <= cardSaleResult.Count; j++)
			{
				this.InsertNewCollectionCard(cardSaleResult.AssetName, cardSaleResult.Premium, DateTime.Now, 1, true);
			}
			CraftingManager.Get().OnCardCreated(cardSaleResult);
			AchieveManager.Get().ValidateAchievesNow(new AchieveManager.ActiveAchievesUpdatedCallback(this.OnActiveAchievesUpdated));
			flag = true;
			break;
		case Network.CardSaleResult.SaleResult.SOULBOUND:
			CraftingManager.Get().OnCardDisenchantSoulboundError(cardSaleResult);
			flag = false;
			break;
		case Network.CardSaleResult.SaleResult.FAILED_WRONG_SELL_PRICE:
		{
			NetCache.CardValue cardValue = CraftingManager.Get().GetCardValue(cardSaleResult.AssetName, cardSaleResult.Premium);
			if (cardValue != null)
			{
				cardValue.Sell = cardSaleResult.UnitSellPrice;
				cardValue.Nerfed = cardSaleResult.Nerfed;
			}
			flag = false;
			break;
		}
		case Network.CardSaleResult.SaleResult.FAILED_WRONG_BUY_PRICE:
		{
			NetCache.CardValue cardValue2 = CraftingManager.Get().GetCardValue(cardSaleResult.AssetName, cardSaleResult.Premium);
			if (cardValue2 != null)
			{
				cardValue2.Buy = cardSaleResult.UnitBuyPrice;
				cardValue2.Nerfed = cardSaleResult.Nerfed;
			}
			flag = false;
			break;
		}
		case Network.CardSaleResult.SaleResult.FAILED_NO_PERMISSION:
			CraftingManager.Get().OnCardPermissionError(cardSaleResult);
			flag = false;
			break;
		case Network.CardSaleResult.SaleResult.FAILED_EVENT_NOT_ACTIVE:
			CraftingManager.Get().OnCardCraftingEventNotActiveError(cardSaleResult);
			flag = false;
			break;
		default:
			CraftingManager.Get().OnCardUnknownError(cardSaleResult);
			flag = false;
			break;
		}
		if (!flag)
		{
			Debug.LogWarning(string.Format("CollectionManager.OnCardSale {0} for card {1} (asset {2}) premium {3}", new object[]
			{
				cardSaleResult.Action,
				cardSaleResult.AssetName,
				cardSaleResult.AssetID,
				cardSaleResult.Premium
			}));
			return;
		}
		this.OnCollectionChanged();
	}

	// Token: 0x06001262 RID: 4706 RVA: 0x0005015C File Offset: 0x0004E35C
	private void OnMassDisenchantResponse()
	{
		Network.MassDisenchantResponse massDisenchantResponse = Network.GetMassDisenchantResponse();
		if (massDisenchantResponse.Amount == 0)
		{
			Debug.LogError("CollectionManager.OnMassDisenchantResponse(): Amount is 0. This means the backend failed to mass disenchant correctly.");
			return;
		}
		NetCache.Get().OnArcaneDustBalanceChanged((long)massDisenchantResponse.Amount);
		if (AchieveManager.Get().HasIncompleteDisenchantAchieves())
		{
			AchieveManager.Get().UpdateActiveAchieves(new AchieveManager.ActiveAchievesUpdatedCallback(this.OnActiveAchievesUpdated));
		}
		CollectionManager.OnMassDisenchant[] array = this.m_massDisenchantListeners.ToArray();
		foreach (CollectionManager.OnMassDisenchant onMassDisenchant in array)
		{
			onMassDisenchant(massDisenchantResponse.Amount);
		}
		List<CollectibleCard> massDisenchantCards = this.GetMassDisenchantCards();
		foreach (CollectibleCard collectibleCard in massDisenchantCards)
		{
			this.RemoveCollectionCard(collectibleCard.CardId, collectibleCard.PremiumType, collectibleCard.DisenchantCount);
		}
		this.OnCollectionChanged();
	}

	// Token: 0x06001263 RID: 4707 RVA: 0x00050260 File Offset: 0x0004E460
	private void OnSetFavoriteHeroResponse()
	{
		Network.SetFavoriteHeroResponse setFavoriteHeroResponse = Network.GetSetFavoriteHeroResponse();
		if (!setFavoriteHeroResponse.Success)
		{
			return;
		}
		if (setFavoriteHeroResponse.HeroClass == TAG_CLASS.INVALID || setFavoriteHeroResponse.Hero == null)
		{
			Debug.LogWarning(string.Format("CollectionManager.OnSetFavoriteHeroResponse: setting hero was a success, but message contains invalid class ({0}) and/or hero ({1})", setFavoriteHeroResponse.HeroClass, setFavoriteHeroResponse.Hero));
			return;
		}
		NetCache.NetCacheFavoriteHeroes netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFavoriteHeroes>();
		if (netObject != null)
		{
			netObject.FavoriteHeroes[setFavoriteHeroResponse.HeroClass] = setFavoriteHeroResponse.Hero;
			Log.Rachelle.Print("CollectionManager.OnSetFavoriteHeroResponse: favorite hero for class {0} updated to {1}", new object[]
			{
				setFavoriteHeroResponse.HeroClass,
				setFavoriteHeroResponse.Hero
			});
		}
		this.UpdateFavoriteHero(setFavoriteHeroResponse.HeroClass, setFavoriteHeroResponse.Hero.Name, setFavoriteHeroResponse.Hero.Premium);
	}

	// Token: 0x06001264 RID: 4708 RVA: 0x0005032C File Offset: 0x0004E52C
	private void NetCache_OnFavoriteHeroesReceived()
	{
		NetCache.NetCacheFavoriteHeroes netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFavoriteHeroes>();
		if (netObject == null || netObject.FavoriteHeroes == null)
		{
			return;
		}
		foreach (KeyValuePair<TAG_CLASS, NetCache.CardDefinition> keyValuePair in netObject.FavoriteHeroes)
		{
			TAG_CLASS key = keyValuePair.Key;
			NetCache.CardDefinition value = keyValuePair.Value;
			this.UpdateFavoriteHero(key, value.Name, value.Premium);
		}
	}

	// Token: 0x06001265 RID: 4709 RVA: 0x000503C4 File Offset: 0x0004E5C4
	private void UpdateFavoriteHero(TAG_CLASS heroClass, string heroCardId, TAG_PREMIUM premium)
	{
		if (NetCache.Get().IsNetObjectReady<NetCache.NetCacheDecks>())
		{
			NetCache.NetCacheDecks netObject = NetCache.Get().GetNetObject<NetCache.NetCacheDecks>();
			foreach (NetCache.DeckHeader deckHeader in netObject.Decks)
			{
				if (!deckHeader.HeroOverridden)
				{
					EntityDef entityDef = DefLoader.Get().GetEntityDef(deckHeader.Hero);
					if (heroClass == entityDef.GetClass())
					{
						deckHeader.Hero = heroCardId;
						deckHeader.HeroPremium = premium;
						CollectionDeck deck = this.GetDeck(deckHeader.ID);
						if (deck != null)
						{
							deck.HeroCardID = heroCardId;
							deck.HeroPremium = premium;
						}
						CollectionDeck baseDeck = this.GetBaseDeck(deckHeader.ID);
						if (baseDeck != null)
						{
							baseDeck.HeroCardID = heroCardId;
							baseDeck.HeroPremium = premium;
						}
					}
				}
			}
		}
		else
		{
			Log.JMac.PrintWarning("Received Favorite Heroes without NetCacheDecks being ready!", new object[0]);
		}
		if (this.m_favoriteHeroChangedListeners.Count > 0)
		{
			NetCache.CardDefinition cardDefinition = new NetCache.CardDefinition();
			cardDefinition.Name = heroCardId;
			cardDefinition.Premium = premium;
			CollectionManager.FavoriteHeroChangedListener[] array = this.m_favoriteHeroChangedListeners.ToArray();
			foreach (CollectionManager.FavoriteHeroChangedListener favoriteHeroChangedListener in array)
			{
				favoriteHeroChangedListener.Fire(heroClass, cardDefinition);
			}
		}
	}

	// Token: 0x06001266 RID: 4710 RVA: 0x00050538 File Offset: 0x0004E738
	public void NetCache_OnDecksReceived()
	{
		NetCache.NetCacheDecks netObject = NetCache.Get().GetNetObject<NetCache.NetCacheDecks>();
		foreach (NetCache.DeckHeader deckHeader in netObject.Decks)
		{
			if (deckHeader.Type == 1)
			{
				CollectionDeck deck = this.GetDeck(deckHeader.ID);
				if (deck == null)
				{
					if (DefLoader.Get().GetEntityDef(deckHeader.Hero) != null)
					{
						this.AddDeck(deckHeader, false);
					}
				}
			}
		}
	}

	// Token: 0x06001267 RID: 4711 RVA: 0x000505E8 File Offset: 0x0004E7E8
	private void OnDefaultCardBackSet()
	{
		Network.CardBackResponse cardBackReponse = ConnectAPI.GetCardBackReponse();
		if (!cardBackReponse.Success)
		{
			Log.Rachelle.Print("SetCardBack FAILED (cardBack = {0})", new object[]
			{
				cardBackReponse.CardBack
			});
			return;
		}
		NetCache.NetCacheCardBacks netObject = NetCache.Get().GetNetObject<NetCache.NetCacheCardBacks>();
		NetCache.NetCacheDecks netObject2 = NetCache.Get().GetNetObject<NetCache.NetCacheDecks>();
		netObject.DefaultCardBack = cardBackReponse.CardBack;
		foreach (NetCache.DeckHeader deckHeader in netObject2.Decks)
		{
			if (!deckHeader.CardBackOverridden)
			{
				deckHeader.CardBack = cardBackReponse.CardBack;
				CollectionDeck deck = this.GetDeck(deckHeader.ID);
				if (deck != null)
				{
					deck.CardBackID = deckHeader.CardBack;
				}
				CollectionDeck baseDeck = this.GetBaseDeck(deckHeader.ID);
				if (baseDeck != null)
				{
					baseDeck.CardBackID = deckHeader.CardBack;
				}
			}
		}
		CollectionManager.DefaultCardbackChangedListener[] array = this.m_defaultCardbackChangedListeners.ToArray();
		foreach (CollectionManager.DefaultCardbackChangedListener defaultCardbackChangedListener in array)
		{
			defaultCardbackChangedListener.Fire(netObject.DefaultCardBack);
		}
	}

	// Token: 0x06001268 RID: 4712 RVA: 0x00050734 File Offset: 0x0004E934
	private void OnGetDeckContentsResponse()
	{
		GetDeckContentsResponse deckContentsResponse = Network.GetDeckContentsResponse();
		for (int i = 0; i < deckContentsResponse.Decks.Count; i++)
		{
			Network.DeckContents netDeck = Network.DeckContents.FromPacket(deckContentsResponse.Decks[i]);
			if (this.m_pendingRequestDeckContents != null)
			{
				this.m_pendingRequestDeckContents.Remove(netDeck.Deck);
			}
			CollectionDeck collectionDeck = null;
			this.m_decks.TryGetValue(netDeck.Deck, out collectionDeck);
			CollectionDeck collectionDeck2 = null;
			this.m_baseDecks.TryGetValue(netDeck.Deck, out collectionDeck2);
			if (collectionDeck == null || collectionDeck2 == null)
			{
				if (!Enumerable.Any<KeyValuePair<TAG_CLASS, CollectionManager.PreconDeck>>(this.m_preconDecks, (KeyValuePair<TAG_CLASS, CollectionManager.PreconDeck> kv) => kv.Value.ID == netDeck.Deck))
				{
					Debug.LogErrorFormat("Got contents for an unknown deck or baseDeck: deckId={0}", new object[]
					{
						netDeck.Deck
					});
				}
			}
			else
			{
				collectionDeck.ClearSlotContents();
				collectionDeck2.ClearSlotContents();
				foreach (Network.CardUserData cardUserData in netDeck.Cards)
				{
					string text = GameUtils.TranslateDbIdToCardId(cardUserData.DbId);
					if (text == null)
					{
						Debug.LogError(string.Format("CollectionManager.OnDeck(): Could not find card with asset ID {0} in our card manifest", cardUserData.DbId));
					}
					else
					{
						for (int j = 0; j < cardUserData.Count; j++)
						{
							collectionDeck.AddCard(text, cardUserData.Premium, false);
							collectionDeck2.AddCard(text, cardUserData.Premium, false);
						}
					}
				}
				collectionDeck.MarkNetworkContentsLoaded();
			}
			this.FireDeckContentsEvent(netDeck.Deck);
		}
		foreach (CollectionDeck collectionDeck3 in this.GetDecks().Values)
		{
			if (!collectionDeck3.NetworkContentsLoaded())
			{
				return;
			}
		}
		if (this.m_pendingRequestDeckContents != null)
		{
			float now = Time.realtimeSinceStartup;
			long[] array = Enumerable.ToArray<long>(Enumerable.Select<KeyValuePair<long, float>, long>(Enumerable.Where<KeyValuePair<long, float>>(this.m_pendingRequestDeckContents, (KeyValuePair<long, float> kv) => now - kv.Value > 10f), (KeyValuePair<long, float> kv) => kv.Key));
			for (int k = 0; k < array.Length; k++)
			{
				this.m_pendingRequestDeckContents.Remove(array[k]);
			}
		}
		if (this.m_pendingRequestDeckContents == null || this.m_pendingRequestDeckContents.Count == 0)
		{
			this.FireAllDeckContentsEvent();
		}
	}

	// Token: 0x06001269 RID: 4713 RVA: 0x00050A20 File Offset: 0x0004EC20
	private void OnDBAction()
	{
		Network.DBAction deckResponse = Network.GetDeckResponse();
		Log.Rachelle.Print(string.Format("MetaData:{0} DBAction:{1} Result:{2}", deckResponse.MetaData, deckResponse.Action, deckResponse.Result), new object[0]);
		bool flag = false;
		bool flag2 = false;
		switch (deckResponse.Action)
		{
		case Network.DBAction.ActionType.CREATE_DECK:
			if (deckResponse.Result != Network.DBAction.ResultType.SUCCESS && CollectionDeckTray.Get() != null)
			{
				CollectionDeckTray.Get().GetDecksContent().CreateNewDeckCancelled();
			}
			break;
		case Network.DBAction.ActionType.RENAME_DECK:
			flag = true;
			break;
		case Network.DBAction.ActionType.SET_DECK:
			flag2 = true;
			break;
		}
		if (!flag && !flag2)
		{
			return;
		}
		long deckID = deckResponse.MetaData;
		CollectionDeck deck = this.GetDeck(deckID);
		CollectionDeck baseDeck = this.GetBaseDeck(deckID);
		if (deckResponse.Result == Network.DBAction.ResultType.SUCCESS)
		{
			Log.Rachelle.Print(string.Format("CollectionManager.OnDBAction(): overwriting baseDeck with {0} updated deck ({1}:{2})", (!deck.IsTourneyValid) ? "INVALID" : "valid", deck.ID, deck.Name), new object[0]);
			baseDeck.CopyFrom(deck);
			NetCache.NetCacheDecks netObject = NetCache.Get().GetNetObject<NetCache.NetCacheDecks>();
			NetCache.DeckHeader deckHeader2 = netObject.Decks.Find((NetCache.DeckHeader deckHeader) => deckHeader.ID == deckID);
			if (deckHeader2 != null)
			{
				deckHeader2.HeroOverridden = deck.HeroOverridden;
				deckHeader2.CardBackOverridden = deck.CardBackOverridden;
				deckHeader2.SeasonId = deck.SeasonId;
				deckHeader2.NeedsName = deck.NeedsName;
				deckHeader2.IsWild = deck.IsWild;
				deckHeader2.LastModified = new DateTime?(DateTime.Now);
			}
		}
		else
		{
			Log.Rachelle.Print(string.Format("CollectionManager.OnDBAction(): overwriting deck that failed to update with base deck ({0}:{1})", baseDeck.ID, baseDeck.Name), new object[0]);
			deck.CopyFrom(baseDeck);
		}
		if (flag)
		{
			deck.OnNameChangeComplete();
		}
		if (flag2)
		{
			deck.OnContentChangesComplete();
		}
	}

	// Token: 0x0600126A RID: 4714 RVA: 0x00050C40 File Offset: 0x0004EE40
	private void OnDeckCreated()
	{
		NetCache.DeckHeader createdDeck = Network.GetCreatedDeck();
		Log.Rachelle.Print(string.Format("DeckCreated:{0} ID:{1} Hero:{2}", createdDeck.Name, createdDeck.ID, createdDeck.Hero), new object[0]);
		CollectionDeck collectionDeck = this.AddDeck(createdDeck);
		collectionDeck.MarkNetworkContentsLoaded();
		foreach (CollectionManager.DelOnDeckCreated delOnDeckCreated in this.m_deckCreatedListeners)
		{
			delOnDeckCreated(createdDeck.ID);
		}
	}

	// Token: 0x0600126B RID: 4715 RVA: 0x00050CE4 File Offset: 0x0004EEE4
	private void OnDeckDeleted()
	{
		Log.Rachelle.Print("CollectionManager.OnDeckDeleted", new object[0]);
		long deletedDeckID = Network.GetDeletedDeckID();
		Log.Rachelle.Print(string.Format("DeckDeleted:{0}", deletedDeckID), new object[0]);
		this.RemoveDeck(deletedDeckID);
		if (CollectionDeckTray.Get() == null)
		{
			return;
		}
		CollectionManager.DelOnDeckDeleted[] array = this.m_deckDeletedListeners.ToArray();
		foreach (CollectionManager.DelOnDeckDeleted delOnDeckDeleted in array)
		{
			delOnDeckDeleted(deletedDeckID);
		}
	}

	// Token: 0x0600126C RID: 4716 RVA: 0x00050D78 File Offset: 0x0004EF78
	private void OnDeckRenamed()
	{
		Network.DeckName renamedDeck = Network.GetRenamedDeck();
		Log.Rachelle.Print(string.Format("OnDeckRenamed {0}", renamedDeck.Deck), new object[0]);
		long id = renamedDeck.Deck;
		string name = renamedDeck.Name;
		CollectionDeck baseDeck = this.GetBaseDeck(id);
		baseDeck.Name = name;
		CollectionDeck deck = this.GetDeck(id);
		deck.Name = name;
		NetCache.NetCacheDecks netObject = NetCache.Get().GetNetObject<NetCache.NetCacheDecks>();
		NetCache.DeckHeader deckHeader2 = netObject.Decks.Find((NetCache.DeckHeader deckHeader) => deckHeader.ID == id);
		if (deckHeader2 != null)
		{
			deckHeader2.Name = name;
			deckHeader2.LastModified = new DateTime?(DateTime.Now);
		}
		deck.OnNameChangeComplete();
	}

	// Token: 0x0600126D RID: 4717 RVA: 0x00050E44 File Offset: 0x0004F044
	public static void Init()
	{
		if (CollectionManager.s_instance == null)
		{
			CollectionManager.s_instance = new CollectionManager();
			ApplicationMgr.Get().WillReset += new Action(CollectionManager.s_instance.WillReset);
			NetCache netCache = NetCache.Get();
			netCache.RegisterUpdatedListener(typeof(NetCache.NetCacheFavoriteHeroes), new Action(CollectionManager.s_instance.NetCache_OnFavoriteHeroesReceived));
		}
		CollectionManager.s_instance.InitImpl();
	}

	// Token: 0x0600126E RID: 4718 RVA: 0x00050EAF File Offset: 0x0004F0AF
	public static CollectionManager Get()
	{
		return CollectionManager.s_instance;
	}

	// Token: 0x0600126F RID: 4719 RVA: 0x00050EB6 File Offset: 0x0004F0B6
	public bool IsFullyLoaded()
	{
		return this.m_collectionLoaded;
	}

	// Token: 0x06001270 RID: 4720 RVA: 0x00050EC0 File Offset: 0x0004F0C0
	public void RegisterCollectionNetHandlers()
	{
		Network network = Network.Get();
		network.RegisterNetHandler(216, new Network.NetHandler(this.OnDBAction), null);
		network.RegisterNetHandler(217, new Network.NetHandler(this.OnDeckCreated), null);
		network.RegisterNetHandler(218, new Network.NetHandler(this.OnDeckDeleted), null);
		network.RegisterNetHandler(219, new Network.NetHandler(this.OnDeckRenamed), null);
		network.RegisterNetHandler(258, new Network.NetHandler(this.OnCardSale), null);
		network.RegisterNetHandler(269, new Network.NetHandler(this.OnMassDisenchantResponse), null);
		network.RegisterNetHandler(320, new Network.NetHandler(this.OnSetFavoriteHeroResponse), null);
		network.RegisterNetHandler(292, new Network.NetHandler(this.OnDefaultCardBackSet), null);
	}

	// Token: 0x06001271 RID: 4721 RVA: 0x00050FC4 File Offset: 0x0004F1C4
	public void RemoveCollectionNetHandlers()
	{
		Network network = Network.Get();
		network.RemoveNetHandler(216, new Network.NetHandler(this.OnDBAction));
		network.RemoveNetHandler(217, new Network.NetHandler(this.OnDeckCreated));
		network.RemoveNetHandler(218, new Network.NetHandler(this.OnDeckDeleted));
		network.RemoveNetHandler(219, new Network.NetHandler(this.OnDeckRenamed));
		network.RemoveNetHandler(258, new Network.NetHandler(this.OnCardSale));
		network.RemoveNetHandler(269, new Network.NetHandler(this.OnMassDisenchantResponse));
		network.RemoveNetHandler(320, new Network.NetHandler(this.OnSetFavoriteHeroResponse));
		network.RemoveNetHandler(292, new Network.NetHandler(this.OnDefaultCardBackSet));
	}

	// Token: 0x06001272 RID: 4722 RVA: 0x000510BF File Offset: 0x0004F2BF
	public bool HasVisitedCollection()
	{
		return this.m_hasVisitedCollection;
	}

	// Token: 0x06001273 RID: 4723 RVA: 0x000510C7 File Offset: 0x0004F2C7
	public void SetHasVisitedCollection(bool enable)
	{
		this.m_hasVisitedCollection = enable;
	}

	// Token: 0x06001274 RID: 4724 RVA: 0x000510D0 File Offset: 0x0004F2D0
	public bool IsWaitingForBoxTransition()
	{
		return this.m_waitingForBoxTransition;
	}

	// Token: 0x06001275 RID: 4725 RVA: 0x000510D8 File Offset: 0x0004F2D8
	public void NotifyOfBoxTransitionStart()
	{
		Box.Get().AddTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
		this.m_waitingForBoxTransition = true;
	}

	// Token: 0x06001276 RID: 4726 RVA: 0x000510F7 File Offset: 0x0004F2F7
	public void OnBoxTransitionFinished(object userData)
	{
		Box.Get().RemoveTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
		this.m_waitingForBoxTransition = false;
	}

	// Token: 0x06001277 RID: 4727 RVA: 0x00051118 File Offset: 0x0004F318
	public void AddCardReward(CardRewardData cardReward, bool markAsNew)
	{
		List<CardRewardData> list = new List<CardRewardData>();
		list.Add(cardReward);
		this.AddCardRewards(list, markAsNew);
	}

	// Token: 0x06001278 RID: 4728 RVA: 0x0005113C File Offset: 0x0004F33C
	public void AddCardRewards(List<CardRewardData> cardRewards, bool markAsNew)
	{
		List<string> list = new List<string>();
		List<TAG_PREMIUM> list2 = new List<TAG_PREMIUM>();
		List<DateTime> list3 = new List<DateTime>();
		List<int> list4 = new List<int>();
		DateTime now = DateTime.Now;
		foreach (CardRewardData cardRewardData in cardRewards)
		{
			list.Add(cardRewardData.CardID);
			list2.Add(cardRewardData.Premium);
			list3.Add(now);
			list4.Add(cardRewardData.Count);
			CollectionManager.DelOnCardRewardInserted[] array = this.m_cardRewardListeners.ToArray();
			foreach (CollectionManager.DelOnCardRewardInserted delOnCardRewardInserted in array)
			{
				delOnCardRewardInserted(cardRewardData.CardID, cardRewardData.Premium);
			}
		}
		this.InsertNewCollectionCards(list, list2, list3, list4, !markAsNew);
		AchieveManager.Get().ValidateAchievesNow(new AchieveManager.ActiveAchievesUpdatedCallback(this.OnActiveAchievesUpdated));
	}

	// Token: 0x06001279 RID: 4729 RVA: 0x00051248 File Offset: 0x0004F448
	public float CollectionLastModifiedTime()
	{
		return this.m_collectionLastModifiedTime;
	}

	// Token: 0x0600127A RID: 4730 RVA: 0x00051250 File Offset: 0x0004F450
	public int EntityDefSortComparison(EntityDef entityDef1, EntityDef entityDef2)
	{
		int cost = entityDef1.GetCost();
		int cost2 = entityDef2.GetCost();
		if (cost != cost2)
		{
			return cost - cost2;
		}
		int cardTypeSortOrder = this.GetCardTypeSortOrder(entityDef1);
		int cardTypeSortOrder2 = this.GetCardTypeSortOrder(entityDef2);
		if (cardTypeSortOrder != cardTypeSortOrder2)
		{
			return cardTypeSortOrder - cardTypeSortOrder2;
		}
		return string.Compare(entityDef1.GetName(), entityDef2.GetName(), true);
	}

	// Token: 0x0600127B RID: 4731 RVA: 0x000512A4 File Offset: 0x0004F4A4
	public int GetCardTypeSortOrder(EntityDef entityDef)
	{
		switch (entityDef.GetCardType())
		{
		case TAG_CARDTYPE.MINION:
			return 3;
		case TAG_CARDTYPE.SPELL:
			return 2;
		case TAG_CARDTYPE.WEAPON:
			return 1;
		}
		return 0;
	}

	// Token: 0x0600127C RID: 4732 RVA: 0x000512DC File Offset: 0x0004F4DC
	public List<CollectibleCard> FindCards(string searchString = null, TAG_PREMIUM? premiumType = null, int? manaCost = null, TAG_CARD_SET[] theseCardSets = null, TAG_CLASS[] theseClassTypes = null, TAG_CARDTYPE[] theseCardTypes = null, TAG_RARITY? rarity = null, TAG_RACE? race = null, bool? isHero = null, int? minOwned = null, bool? notSeen = null, bool? isCraftable = null, CollectionManager.CollectibleCardFilterFunc[] priorityFilters = null, DeckRuleset deckRuleset = null, bool returnAfterFirstResult = false)
	{
		List<CollectionManager.CollectibleCardFilterFunc> filterFuncs = new List<CollectionManager.CollectibleCardFilterFunc>();
		if (priorityFilters != null)
		{
			filterFuncs.AddRange(priorityFilters);
		}
		if (minOwned != null)
		{
			filterFuncs.Add((CollectibleCard card) => card.OwnedCount >= minOwned.Value);
		}
		if (premiumType != null)
		{
			filterFuncs.Add((CollectibleCard card) => card.PremiumType == premiumType.Value);
		}
		if (manaCost != null)
		{
			int minManaCost = manaCost.Value;
			int maxManaCost = manaCost.Value;
			if (maxManaCost >= 7)
			{
				maxManaCost = int.MaxValue;
			}
			filterFuncs.Add((CollectibleCard card) => card.ManaCost >= minManaCost && card.ManaCost <= maxManaCost);
		}
		if (theseCardSets != null && theseCardSets.Length > 0)
		{
			filterFuncs.Add((CollectibleCard card) => Enumerable.Contains<TAG_CARD_SET>(theseCardSets, card.Set));
		}
		if (theseClassTypes != null && theseClassTypes.Length > 0)
		{
			filterFuncs.Add((CollectibleCard card) => Enumerable.Contains<TAG_CLASS>(theseClassTypes, card.Class));
		}
		if (theseCardTypes != null && theseCardTypes.Length > 0)
		{
			filterFuncs.Add((CollectibleCard card) => Enumerable.Contains<TAG_CARDTYPE>(theseCardTypes, card.CardType));
		}
		if (rarity != null)
		{
			filterFuncs.Add((CollectibleCard card) => card.Rarity == rarity.Value);
		}
		if (race != null)
		{
			filterFuncs.Add((CollectibleCard card) => card.Race == race.Value);
		}
		if (isHero != null)
		{
			filterFuncs.Add((CollectibleCard card) => card.IsHero == isHero.Value);
		}
		if (notSeen != null)
		{
			if (notSeen.Value)
			{
				filterFuncs.Add((CollectibleCard card) => card.SeenCount < card.OwnedCount);
			}
			else
			{
				filterFuncs.Add((CollectibleCard card) => card.SeenCount == card.OwnedCount);
			}
		}
		if (isCraftable != null)
		{
			filterFuncs.Add((CollectibleCard card) => card.IsCraftable == isCraftable.Value);
		}
		filterFuncs.AddRange(CollectibleCardFilter.FiltersFromSearchString(searchString));
		if (deckRuleset != null)
		{
			filterFuncs.Add((CollectibleCard card) => deckRuleset.Filter(card.GetEntityDef()));
		}
		Predicate<CollectibleCard> predicate = delegate(CollectibleCard card)
		{
			for (int i = 0; i < filterFuncs.Count; i++)
			{
				if (!filterFuncs[i](card))
				{
					return false;
				}
			}
			return true;
		};
		List<CollectibleCard> list;
		if (returnAfterFirstResult)
		{
			list = new List<CollectibleCard>();
			CollectibleCard collectibleCard = this.m_collectibleCards.Find(predicate);
			if (collectibleCard != null)
			{
				list.Add(collectibleCard);
			}
		}
		else
		{
			list = this.m_collectibleCards.FindAll(predicate);
		}
		string text = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_EXTRA");
		if (searchString != null && Enumerable.Contains<string>(searchString.ToLower().Split(new char[]
		{
			' '
		}), text))
		{
			this.FilterOnlyExtraCards(ref list);
		}
		return list;
	}

	// Token: 0x0600127D RID: 4733 RVA: 0x0005168C File Offset: 0x0004F88C
	public List<CollectibleCard> FindOrderedCards(string searchString = null, TAG_PREMIUM? premiumType = null, int? manaCost = null, TAG_CARD_SET[] theseCardSets = null, TAG_CLASS[] theseClassTypes = null, TAG_CARDTYPE[] theseCardTypes = null, TAG_RARITY? rarity = null, TAG_RACE? race = null, bool? isHero = null, int? minOwned = null, bool? notSeen = null, bool? isCraftable = null, CollectionManager.CollectibleCardFilterFunc[] priorityFilters = null, DeckRuleset deckRuleset = null)
	{
		List<CollectibleCard> list = this.FindCards(searchString, premiumType, manaCost, theseCardSets, theseClassTypes, theseCardTypes, rarity, race, isHero, minOwned, notSeen, isCraftable, priorityFilters, deckRuleset, false);
		IOrderedEnumerable<CollectibleCard> orderedEnumerable = Enumerable.ThenBy<CollectibleCard, string>(Enumerable.OrderBy<CollectibleCard, int>(list, (CollectibleCard c) => c.ManaCost), (CollectibleCard c) => c.Name);
		return Enumerable.ToList<CollectibleCard>(orderedEnumerable);
	}

	// Token: 0x0600127E RID: 4734 RVA: 0x00051708 File Offset: 0x0004F908
	public void FilterOnlyExtraCards(ref List<CollectibleCard> collectibleCards)
	{
		Map<string, int> collectibleCardCount = new Map<string, int>();
		foreach (CollectibleCard collectibleCard in collectibleCards)
		{
			if (collectibleCard.IsCraftable)
			{
				if (!collectibleCardCount.ContainsKey(collectibleCard.CardId))
				{
					collectibleCardCount.Add(collectibleCard.CardId, collectibleCard.OwnedCount);
				}
				else
				{
					Map<string, int> collectibleCardCount2;
					Map<string, int> map = collectibleCardCount2 = collectibleCardCount;
					string cardId;
					string key = cardId = collectibleCard.CardId;
					int num = collectibleCardCount2[cardId];
					map[key] = num + collectibleCard.OwnedCount;
				}
			}
		}
		collectibleCards.RemoveAll((CollectibleCard c) => !collectibleCardCount.ContainsKey(c.CardId) || collectibleCardCount[c.CardId] <= c.MaxCopiesPerDeck);
	}

	// Token: 0x0600127F RID: 4735 RVA: 0x000517E8 File Offset: 0x0004F9E8
	public List<CollectibleCard> GetAllCards()
	{
		return this.m_collectibleCards;
	}

	// Token: 0x06001280 RID: 4736 RVA: 0x000517F0 File Offset: 0x0004F9F0
	public void RegisterCollectionLoadedListener(CollectionManager.DelOnCollectionLoaded listener)
	{
		if (this.m_collectionLoadedListeners.Contains(listener))
		{
			return;
		}
		this.m_collectionLoadedListeners.Add(listener);
	}

	// Token: 0x06001281 RID: 4737 RVA: 0x00051810 File Offset: 0x0004FA10
	public bool RemoveCollectionLoadedListener(CollectionManager.DelOnCollectionLoaded listener)
	{
		return this.m_collectionLoadedListeners.Remove(listener);
	}

	// Token: 0x06001282 RID: 4738 RVA: 0x0005181E File Offset: 0x0004FA1E
	public void RegisterCollectionChangedListener(CollectionManager.DelOnCollectionChanged listener)
	{
		if (this.m_collectionChangedListeners.Contains(listener))
		{
			return;
		}
		this.m_collectionChangedListeners.Add(listener);
	}

	// Token: 0x06001283 RID: 4739 RVA: 0x0005183E File Offset: 0x0004FA3E
	public bool RemoveCollectionChangedListener(CollectionManager.DelOnCollectionChanged listener)
	{
		return this.m_collectionChangedListeners.Remove(listener);
	}

	// Token: 0x06001284 RID: 4740 RVA: 0x0005184C File Offset: 0x0004FA4C
	public void RegisterDeckCreatedListener(CollectionManager.DelOnDeckCreated listener)
	{
		if (this.m_deckCreatedListeners.Contains(listener))
		{
			return;
		}
		this.m_deckCreatedListeners.Add(listener);
	}

	// Token: 0x06001285 RID: 4741 RVA: 0x0005186C File Offset: 0x0004FA6C
	public bool RemoveDeckCreatedListener(CollectionManager.DelOnDeckCreated listener)
	{
		return this.m_deckCreatedListeners.Remove(listener);
	}

	// Token: 0x06001286 RID: 4742 RVA: 0x0005187A File Offset: 0x0004FA7A
	public void RegisterDeckDeletedListener(CollectionManager.DelOnDeckDeleted listener)
	{
		if (this.m_deckDeletedListeners.Contains(listener))
		{
			return;
		}
		this.m_deckDeletedListeners.Add(listener);
	}

	// Token: 0x06001287 RID: 4743 RVA: 0x0005189A File Offset: 0x0004FA9A
	public bool RemoveDeckDeletedListener(CollectionManager.DelOnDeckDeleted listener)
	{
		return this.m_deckDeletedListeners.Remove(listener);
	}

	// Token: 0x06001288 RID: 4744 RVA: 0x000518A8 File Offset: 0x0004FAA8
	public void RegisterDeckContentsListener(CollectionManager.DelOnDeckContents listener)
	{
		if (this.m_deckContentsListeners.Contains(listener))
		{
			return;
		}
		this.m_deckContentsListeners.Add(listener);
	}

	// Token: 0x06001289 RID: 4745 RVA: 0x000518C8 File Offset: 0x0004FAC8
	public bool RemoveDeckContentsListener(CollectionManager.DelOnDeckContents listener)
	{
		return this.m_deckContentsListeners.Remove(listener);
	}

	// Token: 0x0600128A RID: 4746 RVA: 0x000518D6 File Offset: 0x0004FAD6
	public void RegisterNewCardSeenListener(CollectionManager.DelOnNewCardSeen listener)
	{
		if (this.m_newCardSeenListeners.Contains(listener))
		{
			return;
		}
		this.m_newCardSeenListeners.Add(listener);
	}

	// Token: 0x0600128B RID: 4747 RVA: 0x000518F6 File Offset: 0x0004FAF6
	public bool RemoveNewCardSeenListener(CollectionManager.DelOnNewCardSeen listener)
	{
		return this.m_newCardSeenListeners.Remove(listener);
	}

	// Token: 0x0600128C RID: 4748 RVA: 0x00051904 File Offset: 0x0004FB04
	public void RegisterCardRewardInsertedListener(CollectionManager.DelOnCardRewardInserted listener)
	{
		if (this.m_cardRewardListeners.Contains(listener))
		{
			return;
		}
		this.m_cardRewardListeners.Add(listener);
	}

	// Token: 0x0600128D RID: 4749 RVA: 0x00051924 File Offset: 0x0004FB24
	public bool RemoveCardRewardInsertedListener(CollectionManager.DelOnCardRewardInserted listener)
	{
		return this.m_cardRewardListeners.Remove(listener);
	}

	// Token: 0x0600128E RID: 4750 RVA: 0x00051932 File Offset: 0x0004FB32
	public void RegisterAchievesCompletedListener(CollectionManager.DelOnAchievesCompleted listener)
	{
		if (this.m_achievesCompletedListeners.Contains(listener))
		{
			return;
		}
		this.m_achievesCompletedListeners.Add(listener);
	}

	// Token: 0x0600128F RID: 4751 RVA: 0x00051952 File Offset: 0x0004FB52
	public bool RemoveAchievesCompletedListener(CollectionManager.DelOnAchievesCompleted listener)
	{
		return this.m_achievesCompletedListeners.Remove(listener);
	}

	// Token: 0x06001290 RID: 4752 RVA: 0x00051960 File Offset: 0x0004FB60
	public void RegisterMassDisenchantListener(CollectionManager.OnMassDisenchant listener)
	{
		if (this.m_massDisenchantListeners.Contains(listener))
		{
			return;
		}
		this.m_massDisenchantListeners.Add(listener);
	}

	// Token: 0x06001291 RID: 4753 RVA: 0x00051980 File Offset: 0x0004FB80
	public void RemoveMassDisenchantListener(CollectionManager.OnMassDisenchant listener)
	{
		this.m_massDisenchantListeners.Remove(listener);
	}

	// Token: 0x06001292 RID: 4754 RVA: 0x0005198F File Offset: 0x0004FB8F
	public void RegisterTaggedDeckChanged(CollectionManager.OnTaggedDeckChanged listener)
	{
		this.m_taggedDeckChangedListeners.Add(listener);
	}

	// Token: 0x06001293 RID: 4755 RVA: 0x0005199D File Offset: 0x0004FB9D
	public void RemoveTaggedDeckChanged(CollectionManager.OnTaggedDeckChanged listener)
	{
		this.m_taggedDeckChangedListeners.Remove(listener);
	}

	// Token: 0x06001294 RID: 4756 RVA: 0x000519AC File Offset: 0x0004FBAC
	public bool RegisterDefaultCardbackChangedListener(CollectionManager.DefaultCardbackChangedCallback callback)
	{
		return this.RegisterDefaultCardbackChangedListener(callback, null);
	}

	// Token: 0x06001295 RID: 4757 RVA: 0x000519B8 File Offset: 0x0004FBB8
	public bool RegisterDefaultCardbackChangedListener(CollectionManager.DefaultCardbackChangedCallback callback, object userData)
	{
		CollectionManager.DefaultCardbackChangedListener defaultCardbackChangedListener = new CollectionManager.DefaultCardbackChangedListener();
		defaultCardbackChangedListener.SetCallback(callback);
		defaultCardbackChangedListener.SetUserData(userData);
		if (this.m_defaultCardbackChangedListeners.Contains(defaultCardbackChangedListener))
		{
			return false;
		}
		this.m_defaultCardbackChangedListeners.Add(defaultCardbackChangedListener);
		return true;
	}

	// Token: 0x06001296 RID: 4758 RVA: 0x000519F9 File Offset: 0x0004FBF9
	public bool RemoveDefaultCardbackChangedListener(CollectionManager.DefaultCardbackChangedCallback callback)
	{
		return this.RemoveDefaultCardbackChangedListener(callback, null);
	}

	// Token: 0x06001297 RID: 4759 RVA: 0x00051A04 File Offset: 0x0004FC04
	public bool RemoveDefaultCardbackChangedListener(CollectionManager.DefaultCardbackChangedCallback callback, object userData)
	{
		CollectionManager.DefaultCardbackChangedListener defaultCardbackChangedListener = new CollectionManager.DefaultCardbackChangedListener();
		defaultCardbackChangedListener.SetCallback(callback);
		defaultCardbackChangedListener.SetUserData(userData);
		return this.m_defaultCardbackChangedListeners.Remove(defaultCardbackChangedListener);
	}

	// Token: 0x06001298 RID: 4760 RVA: 0x00051A31 File Offset: 0x0004FC31
	public bool RegisterFavoriteHeroChangedListener(CollectionManager.FavoriteHeroChangedCallback callback)
	{
		return this.RegisterFavoriteHeroChangedListener(callback, null);
	}

	// Token: 0x06001299 RID: 4761 RVA: 0x00051A3C File Offset: 0x0004FC3C
	public bool RegisterFavoriteHeroChangedListener(CollectionManager.FavoriteHeroChangedCallback callback, object userData)
	{
		CollectionManager.FavoriteHeroChangedListener favoriteHeroChangedListener = new CollectionManager.FavoriteHeroChangedListener();
		favoriteHeroChangedListener.SetCallback(callback);
		favoriteHeroChangedListener.SetUserData(userData);
		if (this.m_favoriteHeroChangedListeners.Contains(favoriteHeroChangedListener))
		{
			return false;
		}
		this.m_favoriteHeroChangedListeners.Add(favoriteHeroChangedListener);
		return true;
	}

	// Token: 0x0600129A RID: 4762 RVA: 0x00051A7D File Offset: 0x0004FC7D
	public bool RemoveFavoriteHeroChangedListener(CollectionManager.FavoriteHeroChangedCallback callback)
	{
		return this.RemoveFavoriteHeroChangedListener(callback, null);
	}

	// Token: 0x0600129B RID: 4763 RVA: 0x00051A88 File Offset: 0x0004FC88
	public bool RemoveFavoriteHeroChangedListener(CollectionManager.FavoriteHeroChangedCallback callback, object userData)
	{
		CollectionManager.FavoriteHeroChangedListener favoriteHeroChangedListener = new CollectionManager.FavoriteHeroChangedListener();
		favoriteHeroChangedListener.SetCallback(callback);
		favoriteHeroChangedListener.SetUserData(userData);
		return this.m_favoriteHeroChangedListeners.Remove(favoriteHeroChangedListener);
	}

	// Token: 0x0600129C RID: 4764 RVA: 0x00051AB8 File Offset: 0x0004FCB8
	public TAG_PREMIUM GetBestCardPremium(string cardID)
	{
		CollectibleCard collectibleCard = null;
		if (this.m_collectibleCardIndex.TryGetValue(new CollectionManager.CollectibleCardIndex(cardID, TAG_PREMIUM.GOLDEN), out collectibleCard) && collectibleCard.OwnedCount > 0)
		{
			return TAG_PREMIUM.GOLDEN;
		}
		return TAG_PREMIUM.NORMAL;
	}

	// Token: 0x0600129D RID: 4765 RVA: 0x00051AF0 File Offset: 0x0004FCF0
	public CollectibleCard GetCard(string cardID, TAG_PREMIUM premium)
	{
		CollectibleCard result = null;
		this.m_collectibleCardIndex.TryGetValue(new CollectionManager.CollectibleCardIndex(cardID, premium), out result);
		return result;
	}

	// Token: 0x0600129E RID: 4766 RVA: 0x00051B18 File Offset: 0x0004FD18
	public List<CollectibleCard> GetHeroesIOwn(TAG_CLASS heroClass)
	{
		return new List<CollectibleCard>(this.FindCards(null, default(TAG_PREMIUM?), default(int?), null, null, null, default(TAG_RARITY?), default(TAG_RACE?), new bool?(true), new int?(1), default(bool?), default(bool?), null, null, false));
	}

	// Token: 0x0600129F RID: 4767 RVA: 0x00051B7C File Offset: 0x0004FD7C
	public List<CollectibleCard> GetBestHeroesIOwn(TAG_CLASS heroClass)
	{
		TAG_CLASS[] theseClassTypes = new TAG_CLASS[]
		{
			heroClass
		};
		List<CollectibleCard> list = this.FindCards(null, default(TAG_PREMIUM?), default(int?), null, theseClassTypes, null, default(TAG_RARITY?), default(TAG_RACE?), new bool?(true), new int?(1), default(bool?), default(bool?), null, null, false);
		IEnumerable<CollectibleCard> enumerable = Enumerable.Where<CollectibleCard>(list, (CollectibleCard h) => h.PremiumType == TAG_PREMIUM.GOLDEN);
		IEnumerable<CollectibleCard> enumerable2 = Enumerable.Where<CollectibleCard>(list, (CollectibleCard h) => h.PremiumType == TAG_PREMIUM.NORMAL);
		List<CollectibleCard> list2 = new List<CollectibleCard>();
		foreach (CollectibleCard collectibleCard in enumerable)
		{
			list2.Add(collectibleCard);
		}
		CollectibleCard heroCard;
		foreach (CollectibleCard heroCard2 in enumerable2)
		{
			heroCard = heroCard2;
			if (list2.Find((CollectibleCard e) => e.CardDbId == heroCard.CardDbId) == null)
			{
				list2.Add(heroCard);
			}
		}
		return list2;
	}

	// Token: 0x060012A0 RID: 4768 RVA: 0x00051CF8 File Offset: 0x0004FEF8
	public NetCache.CardDefinition GetFavoriteHero(TAG_CLASS heroClass)
	{
		NetCache.NetCacheFavoriteHeroes netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFavoriteHeroes>();
		if (netObject == null)
		{
			return null;
		}
		if (!netObject.FavoriteHeroes.ContainsKey(heroClass))
		{
			return null;
		}
		return netObject.FavoriteHeroes[heroClass];
	}

	// Token: 0x060012A1 RID: 4769 RVA: 0x00051D38 File Offset: 0x0004FF38
	public int GetBasicCardsIOwn(TAG_CLASS cardClass)
	{
		NetCache.NetCacheCollection netObject = NetCache.Get().GetNetObject<NetCache.NetCacheCollection>();
		return netObject.BasicCardsUnlockedPerClass[cardClass];
	}

	// Token: 0x060012A2 RID: 4770 RVA: 0x00051D5C File Offset: 0x0004FF5C
	public bool AllCardsInSetOwned(TAG_CARD_SET? cardSet, TAG_CLASS? cardClass, TAG_RARITY? cardRarity, TAG_RACE? cardRace, TAG_PREMIUM? premium)
	{
		TAG_CARD_SET[] array;
		if (cardSet != null)
		{
			(array = new TAG_CARD_SET[1])[0] = cardSet.Value;
		}
		else
		{
			array = null;
		}
		TAG_CARD_SET[] array2 = array;
		TAG_CLASS[] array3;
		if (cardClass != null)
		{
			(array3 = new TAG_CLASS[1])[0] = cardClass.Value;
		}
		else
		{
			array3 = null;
		}
		TAG_CLASS[] array4 = array3;
		TAG_CARD_SET[] theseCardSets = array2;
		TAG_CLASS[] theseClassTypes = array4;
		List<CollectibleCard> list = this.FindCards(null, premium, default(int?), theseCardSets, theseClassTypes, null, cardRarity, cardRace, default(bool?), default(int?), default(bool?), default(bool?), null, null, false);
		HashSet<int> hashSet = new HashSet<int>();
		foreach (CollectibleCard collectibleCard in list)
		{
			if (collectibleCard.OwnedCount == 0)
			{
				if (premium != null)
				{
					return false;
				}
				if (hashSet.Contains(collectibleCard.CardDbId))
				{
					return false;
				}
				hashSet.Add(collectibleCard.CardDbId);
			}
		}
		return true;
	}

	// Token: 0x060012A3 RID: 4771 RVA: 0x00051E98 File Offset: 0x00050098
	public List<CollectibleCard> GetOwnedCards()
	{
		return new List<CollectibleCard>(this.FindCards(null, default(TAG_PREMIUM?), default(int?), null, null, null, default(TAG_RARITY?), default(TAG_RACE?), default(bool?), new int?(1), default(bool?), default(bool?), null, null, false));
	}

	// Token: 0x060012A4 RID: 4772 RVA: 0x00051F00 File Offset: 0x00050100
	public void GetOwnedCardCount(string cardId, out int standard, out int golden)
	{
		standard = 0;
		golden = 0;
		CollectibleCard collectibleCard = null;
		if (this.m_collectibleCardIndex.TryGetValue(new CollectionManager.CollectibleCardIndex(cardId, TAG_PREMIUM.NORMAL), out collectibleCard))
		{
			standard += collectibleCard.OwnedCount;
		}
		if (this.m_collectibleCardIndex.TryGetValue(new CollectionManager.CollectibleCardIndex(cardId, TAG_PREMIUM.GOLDEN), out collectibleCard))
		{
			golden += collectibleCard.OwnedCount;
		}
	}

	// Token: 0x060012A5 RID: 4773 RVA: 0x00051F5D File Offset: 0x0005015D
	public List<TAG_CARD_SET> GetDisplayableCardSets()
	{
		return this.m_displayableCardSets;
	}

	// Token: 0x060012A6 RID: 4774 RVA: 0x00051F68 File Offset: 0x00050168
	public bool IsCardInCollection(string cardID, TAG_PREMIUM premium)
	{
		CollectibleCard collectibleCard = null;
		return this.m_collectibleCardIndex.TryGetValue(new CollectionManager.CollectibleCardIndex(cardID, premium), out collectibleCard) && collectibleCard.OwnedCount > 0;
	}

	// Token: 0x060012A7 RID: 4775 RVA: 0x00051F9C File Offset: 0x0005019C
	public int GetNumCopiesInCollection(string cardID, TAG_PREMIUM premium)
	{
		CollectibleCard collectibleCard = null;
		this.m_collectibleCardIndex.TryGetValue(new CollectionManager.CollectibleCardIndex(cardID, premium), out collectibleCard);
		return (collectibleCard == null) ? 0 : collectibleCard.OwnedCount;
	}

	// Token: 0x060012A8 RID: 4776 RVA: 0x00051FD4 File Offset: 0x000501D4
	public List<CollectibleCard> GetMassDisenchantCards()
	{
		List<CollectibleCard> list = new List<CollectibleCard>();
		List<CollectibleCard> ownedCards = this.GetOwnedCards();
		foreach (CollectibleCard collectibleCard in ownedCards)
		{
			if (collectibleCard.DisenchantCount > 0)
			{
				list.Add(collectibleCard);
			}
		}
		return list;
	}

	// Token: 0x060012A9 RID: 4777 RVA: 0x00052044 File Offset: 0x00050244
	public int GetCardsToDisenchantCount()
	{
		int num = 0;
		foreach (CollectibleCard collectibleCard in this.GetMassDisenchantCards())
		{
			num += collectibleCard.DisenchantCount;
		}
		return num;
	}

	// Token: 0x060012AA RID: 4778 RVA: 0x000520A4 File Offset: 0x000502A4
	public void MarkAllInstancesAsSeen(string cardID, TAG_PREMIUM premium)
	{
		NetCache.NetCacheCollection netObject = NetCache.Get().GetNetObject<NetCache.NetCacheCollection>();
		int num = GameUtils.TranslateCardIdToDbId(cardID);
		if (num == 0)
		{
			return;
		}
		CollectibleCard card = this.GetCard(cardID, premium);
		if (card.SeenCount == card.OwnedCount)
		{
			return;
		}
		Network.AckCardSeenBefore(num, premium);
		card.SeenCount = card.OwnedCount;
		NetCache.CardStack cardStack = netObject.Stacks.Find((NetCache.CardStack obj) => obj.Def.Name == card.CardId && obj.Def.Premium == card.PremiumType);
		if (cardStack != null)
		{
			cardStack.NumSeen = cardStack.Count;
		}
		foreach (CollectionManager.DelOnNewCardSeen delOnNewCardSeen in this.m_newCardSeenListeners)
		{
			delOnNewCardSeen(cardID, premium);
		}
	}

	// Token: 0x060012AB RID: 4779 RVA: 0x00052198 File Offset: 0x00050398
	public void OnBoosterOpened(List<NetCache.BoosterCard> cards)
	{
		if (Options.Get().GetBool(Option.FAKE_PACK_OPENING))
		{
			return;
		}
		List<string> list = new List<string>();
		List<TAG_PREMIUM> list2 = new List<TAG_PREMIUM>();
		List<DateTime> list3 = new List<DateTime>();
		List<int> list4 = new List<int>();
		foreach (NetCache.BoosterCard boosterCard in cards)
		{
			list.Add(boosterCard.Def.Name);
			list2.Add(boosterCard.Def.Premium);
			list3.Add(new DateTime(boosterCard.Date));
			list4.Add(1);
		}
		this.InsertNewCollectionCards(list, list2, list3, list4, false);
		AchieveManager.Get().ValidateAchievesNow(new AchieveManager.ActiveAchievesUpdatedCallback(this.OnActiveAchievesUpdated));
		this.OnCollectionChanged();
	}

	// Token: 0x060012AC RID: 4780 RVA: 0x00052278 File Offset: 0x00050478
	public void OnCardRewardOpened(string cardID, TAG_PREMIUM premium, int count)
	{
		this.InsertNewCollectionCard(cardID, premium, DateTime.Now, count, false);
		AchieveManager.Get().ValidateAchievesNow(new AchieveManager.ActiveAchievesUpdatedCallback(this.OnActiveAchievesUpdated));
		this.OnCollectionChanged();
	}

	// Token: 0x060012AD RID: 4781 RVA: 0x000522A5 File Offset: 0x000504A5
	public CollectionManager.PreconDeck GetPreconDeck(TAG_CLASS heroClass)
	{
		if (!this.m_preconDecks.ContainsKey(heroClass))
		{
			return null;
		}
		return this.m_preconDecks[heroClass];
	}

	// Token: 0x060012AE RID: 4782 RVA: 0x000522C8 File Offset: 0x000504C8
	public SortedDictionary<long, CollectionDeck> GetDecks()
	{
		SortedDictionary<long, CollectionDeck> sortedDictionary = new SortedDictionary<long, CollectionDeck>();
		foreach (KeyValuePair<long, CollectionDeck> keyValuePair in this.m_decks)
		{
			if (keyValuePair.Value != null)
			{
				if (keyValuePair.Value.Type != 6 || (TavernBrawlManager.Get().IsTavernBrawlActive && keyValuePair.Value.SeasonId == TavernBrawlManager.Get().CurrentMission().seasonId))
				{
					sortedDictionary.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}
		return sortedDictionary;
	}

	// Token: 0x060012AF RID: 4783 RVA: 0x00052390 File Offset: 0x00050590
	public int GetDeckCount()
	{
		return this.m_decks.Count;
	}

	// Token: 0x060012B0 RID: 4784 RVA: 0x000523A0 File Offset: 0x000505A0
	public List<CollectionDeck> GetDecks(DeckType deckType)
	{
		if (!NetCache.Get().IsNetObjectReady<NetCache.NetCacheDecks>())
		{
			Debug.LogWarning("Attempting to get decks from CollectionManager, even though NetCacheDecks is not ready (meaning it's waiting for the decks to be updated)!");
		}
		List<CollectionDeck> list = new List<CollectionDeck>();
		foreach (CollectionDeck collectionDeck in this.m_decks.Values)
		{
			if (collectionDeck.Type == deckType)
			{
				if (deckType == 6)
				{
					TavernBrawlMission tavernBrawlMission = (!TavernBrawlManager.Get().IsTavernBrawlActive) ? null : TavernBrawlManager.Get().CurrentMission();
					if (tavernBrawlMission == null || collectionDeck.SeasonId != tavernBrawlMission.seasonId)
					{
						continue;
					}
				}
				list.Add(collectionDeck);
			}
		}
		list.Sort(new CollectionManager.DeckSort());
		return list;
	}

	// Token: 0x060012B1 RID: 4785 RVA: 0x0005247C File Offset: 0x0005067C
	public List<CollectionDeck> GetDecksWithClass(TAG_CLASS classType, DeckType deckType)
	{
		List<CollectionDeck> decks = this.GetDecks(deckType);
		List<CollectionDeck> list = new List<CollectionDeck>();
		foreach (CollectionDeck collectionDeck in decks)
		{
			if (collectionDeck.GetClass() == classType)
			{
				list.Add(collectionDeck);
			}
		}
		return list;
	}

	// Token: 0x060012B2 RID: 4786 RVA: 0x000524EC File Offset: 0x000506EC
	public CollectionDeck GetDeck(long id)
	{
		CollectionDeck collectionDeck;
		if (this.m_decks.TryGetValue(id, out collectionDeck))
		{
			if (collectionDeck != null && collectionDeck.Type == 6)
			{
				TavernBrawlMission tavernBrawlMission = (!TavernBrawlManager.Get().IsTavernBrawlActive) ? null : TavernBrawlManager.Get().CurrentMission();
				if (tavernBrawlMission == null || collectionDeck.SeasonId != tavernBrawlMission.seasonId)
				{
					return null;
				}
			}
			return collectionDeck;
		}
		return null;
	}

	// Token: 0x060012B3 RID: 4787 RVA: 0x0005255C File Offset: 0x0005075C
	public bool AreAllDeckContentsReady()
	{
		if (!FixedRewardsMgr.Get().IsStartupFinished())
		{
			return false;
		}
		return Enumerable.FirstOrDefault<KeyValuePair<long, CollectionDeck>>(this.m_decks, (KeyValuePair<long, CollectionDeck> kv) => !kv.Value.NetworkContentsLoaded() && kv.Value.Type != 6).Value == null;
	}

	// Token: 0x060012B4 RID: 4788 RVA: 0x000525B4 File Offset: 0x000507B4
	public bool ShouldAccountSeeStandardWild()
	{
		return GameUtils.IsAnythingRotated() && AchieveManager.Get().HasUnlockedFeature(Achievement.UnlockableFeature.VANILLA_HEROES) && (this.AccountEverHadWildCards() || this.AccountHasRotatedItems());
	}

	// Token: 0x060012B5 RID: 4789 RVA: 0x000525FC File Offset: 0x000507FC
	public bool AccountHasRotatedItems()
	{
		if (this.m_accountHasRotatedItems)
		{
			return true;
		}
		NetCache.NetCacheBoosters netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBoosters>();
		if (netObject != null)
		{
			foreach (NetCache.BoosterStack boosterStack in netObject.BoosterStacks)
			{
				if (GameUtils.IsBoosterRotated((BoosterDbId)boosterStack.Id))
				{
					this.m_accountHasRotatedItems = true;
					return true;
				}
			}
		}
		foreach (AdventureDbId adventureID in GameUtils.GetRotatedAdventures())
		{
			if (AdventureProgressMgr.Get().OwnsOneOrMoreAdventureWings(adventureID))
			{
				this.m_accountHasRotatedItems = true;
				return true;
			}
		}
		return false;
	}

	// Token: 0x060012B6 RID: 4790 RVA: 0x000526F4 File Offset: 0x000508F4
	public bool AccountEverHadWildCards()
	{
		if (this.m_accountEverHadWildCards)
		{
			return true;
		}
		this.m_accountEverHadWildCards = (GameUtils.HasSeenStandardModeTutorial() || this.AccountHasWildCards());
		return this.m_accountEverHadWildCards;
	}

	// Token: 0x060012B7 RID: 4791 RVA: 0x00052730 File Offset: 0x00050930
	private bool AccountHasWildCards()
	{
		if (!GameUtils.IsAnythingRotated())
		{
			return false;
		}
		if (this.GetNumberOfWildDecks() > 0)
		{
			return true;
		}
		if (this.m_lastSearchForWildCardsTime > this.m_collectionLastModifiedTime)
		{
			return this.m_accountHasWildCards;
		}
		this.m_accountHasWildCards = Enumerable.Any<CollectibleCard>(this.m_collectibleCards, (CollectibleCard c) => c.OwnedCount > 0 && GameUtils.IsCardRotated(c.GetEntityDef()));
		this.m_lastSearchForWildCardsTime = Time.realtimeSinceStartup;
		return this.m_accountHasWildCards;
	}

	// Token: 0x060012B8 RID: 4792 RVA: 0x000527B0 File Offset: 0x000509B0
	public int GetNumberOfWildDecks()
	{
		int num = 0;
		foreach (KeyValuePair<long, CollectionDeck> keyValuePair in this.m_decks)
		{
			if (keyValuePair.Value.IsWild)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060012B9 RID: 4793 RVA: 0x0005281C File Offset: 0x00050A1C
	public int GetNumberOfStandardDecks()
	{
		int num = 0;
		foreach (KeyValuePair<long, CollectionDeck> keyValuePair in this.m_decks)
		{
			if (!keyValuePair.Value.IsWild)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060012BA RID: 4794 RVA: 0x00052888 File Offset: 0x00050A88
	public bool AccountHasValidStandardDeck()
	{
		List<CollectionDeck> decks = CollectionManager.Get().GetDecks(1);
		foreach (CollectionDeck collectionDeck in decks)
		{
			if (collectionDeck.IsTourneyValid && !collectionDeck.IsWild)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060012BB RID: 4795 RVA: 0x00052904 File Offset: 0x00050B04
	public bool AccountHasAnyValidDeck()
	{
		List<CollectionDeck> decks = CollectionManager.Get().GetDecks(1);
		foreach (CollectionDeck collectionDeck in decks)
		{
			if (collectionDeck.IsTourneyValid)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060012BC RID: 4796 RVA: 0x00052974 File Offset: 0x00050B74
	public CollectionDeck GetTaggedDeck(CollectionManager.DeckTag tag)
	{
		CollectionDeck collectionDeck = null;
		if (this.m_taggedDecks.TryGetValue(tag, out collectionDeck) && collectionDeck != null && collectionDeck.Type == 6)
		{
			TavernBrawlMission tavernBrawlMission = (!TavernBrawlManager.Get().IsTavernBrawlActive) ? null : TavernBrawlManager.Get().CurrentMission();
			if (tavernBrawlMission == null || collectionDeck.SeasonId != tavernBrawlMission.seasonId)
			{
				return null;
			}
		}
		return collectionDeck;
	}

	// Token: 0x060012BD RID: 4797 RVA: 0x000529E2 File Offset: 0x00050BE2
	public CollectionDeck GetEditedDeck()
	{
		return this.GetTaggedDeck(CollectionManager.DeckTag.Editing);
	}

	// Token: 0x060012BE RID: 4798 RVA: 0x000529EB File Offset: 0x00050BEB
	public int GetDeckSize()
	{
		if (this.m_deckRuleset == null)
		{
			return 30;
		}
		return this.m_deckRuleset.GetDeckSize();
	}

	// Token: 0x060012BF RID: 4799 RVA: 0x00052A08 File Offset: 0x00050C08
	public List<CollectionManager.TemplateDeck> GetTemplateDecks(TAG_CLASS classType)
	{
		List<CollectionManager.TemplateDeck> result = null;
		this.m_templateDecks.TryGetValue(classType, out result);
		return result;
	}

	// Token: 0x060012C0 RID: 4800 RVA: 0x00052A27 File Offset: 0x00050C27
	public bool IsInEditMode()
	{
		return this.m_editMode;
	}

	// Token: 0x060012C1 RID: 4801 RVA: 0x00052A30 File Offset: 0x00050C30
	public CollectionDeck StartEditingDeck(CollectionManager.DeckTag tag, long deckId, object callbackData = null)
	{
		this.m_editMode = true;
		CollectionDeck deck = this.GetDeck(deckId);
		DeckRuleset deckRuleset;
		if (SceneMgr.Get().GetMode() != SceneMgr.Mode.TAVERN_BRAWL)
		{
			deckRuleset = ((!deck.IsWild) ? DeckRuleset.GetStandardRuleset() : DeckRuleset.GetWildRuleset());
			PresenceMgr.Get().SetStatus(new Enum[]
			{
				PresenceStatus.DECKEDITOR
			});
		}
		else
		{
			deckRuleset = TavernBrawlManager.Get().GetDeckRuleset();
		}
		this.SetDeckRuleset(deckRuleset);
		CollectionManagerDisplay.Get().OnStartEditingDeck(deck.IsWild);
		return this.SetTaggedDeck(tag, deckId, callbackData);
	}

	// Token: 0x060012C2 RID: 4802 RVA: 0x00052AC4 File Offset: 0x00050CC4
	public void DoneEditing()
	{
		bool editMode = this.m_editMode;
		this.m_editMode = false;
		if (editMode && SceneMgr.Get() != null && SceneMgr.Get().GetMode() != SceneMgr.Mode.TAVERN_BRAWL)
		{
			PresenceMgr.Get().SetPrevStatus();
		}
		this.SetDeckRuleset(null);
		this.ClearTaggedDeck(CollectionManager.DeckTag.Editing);
	}

	// Token: 0x060012C3 RID: 4803 RVA: 0x00052B1F File Offset: 0x00050D1F
	public DeckRuleset GetDeckRuleset()
	{
		return this.m_deckRuleset;
	}

	// Token: 0x060012C4 RID: 4804 RVA: 0x00052B28 File Offset: 0x00050D28
	public bool IsShowingWildTheming(CollectionDeck deck = null)
	{
		if (SceneMgr.Get().GetMode() == SceneMgr.Mode.TAVERN_BRAWL)
		{
			return false;
		}
		if (deck == null)
		{
			deck = this.GetEditedDeck();
		}
		if (deck != null)
		{
			return deck.IsWild;
		}
		return CollectionManagerDisplay.Get() != null && CollectionManagerDisplay.Get().SetFilterTrayInitialized() && CollectionManagerDisplay.Get().m_pageManager.CardSetFilterIncludesWild();
	}

	// Token: 0x060012C5 RID: 4805 RVA: 0x00052B9C File Offset: 0x00050D9C
	public void SetDeckRuleset(DeckRuleset deckRuleset)
	{
		this.m_deckRuleset = deckRuleset;
		if (CollectionManagerDisplay.Get() != null)
		{
			CollectionManagerDisplay.Get().m_pageManager.SetDeckRuleset(deckRuleset, false);
		}
	}

	// Token: 0x060012C6 RID: 4806 RVA: 0x00052BD4 File Offset: 0x00050DD4
	public CollectionDeck SetTaggedDeck(CollectionManager.DeckTag tag, long deckId, object callbackData = null)
	{
		CollectionDeck collectionDeck = null;
		this.m_decks.TryGetValue(deckId, out collectionDeck);
		this.SetTaggedDeck(tag, collectionDeck, callbackData);
		return collectionDeck;
	}

	// Token: 0x060012C7 RID: 4807 RVA: 0x00052BFC File Offset: 0x00050DFC
	public void SetTaggedDeck(CollectionManager.DeckTag tag, CollectionDeck deck, object callbackData = null)
	{
		CollectionDeck taggedDeck = this.GetTaggedDeck(tag);
		if (deck == taggedDeck)
		{
			return;
		}
		this.m_taggedDecks[tag] = deck;
		CollectionManager.OnTaggedDeckChanged[] array = this.m_taggedDeckChangedListeners.ToArray();
		foreach (CollectionManager.OnTaggedDeckChanged onTaggedDeckChanged in array)
		{
			onTaggedDeckChanged(tag, deck, taggedDeck, callbackData);
		}
	}

	// Token: 0x060012C8 RID: 4808 RVA: 0x00052C5B File Offset: 0x00050E5B
	public void ClearTaggedDeck(CollectionManager.DeckTag tag)
	{
		this.SetTaggedDeck(tag, null, null);
	}

	// Token: 0x060012C9 RID: 4809 RVA: 0x00052C68 File Offset: 0x00050E68
	public void SendCreateDeck(DeckType deckType, string name, string heroCardID)
	{
		int num = GameUtils.TranslateCardIdToDbId(heroCardID);
		if (num == 0)
		{
			Debug.LogWarning(string.Format("CollectionManager.SendCreateDeck(): Unknown hero cardID {0}", heroCardID));
			return;
		}
		bool isWild = Options.Get().GetBool(Option.IN_WILD_MODE);
		if (SceneMgr.Get().GetMode() == SceneMgr.Mode.TAVERN_BRAWL)
		{
			isWild = true;
		}
		TAG_PREMIUM bestCardPremium = this.GetBestCardPremium(heroCardID);
		Network.Get().CreateDeck(deckType, name, num, bestCardPremium, isWild);
	}

	// Token: 0x060012CA RID: 4810 RVA: 0x00052CCD File Offset: 0x00050ECD
	public void SendDeleteDeck(long id)
	{
		Network.Get().DeleteDeck(id);
	}

	// Token: 0x060012CB RID: 4811 RVA: 0x00052CDC File Offset: 0x00050EDC
	public bool RequestDeckContentsForDecksWithoutContentsLoaded(CollectionManager.DelOnAllDeckContents callback = null)
	{
		float now = Time.realtimeSinceStartup;
		IEnumerable<KeyValuePair<long, CollectionDeck>> enumerable = Enumerable.Where<KeyValuePair<long, CollectionDeck>>(this.m_decks, (KeyValuePair<long, CollectionDeck> kv) => !kv.Value.NetworkContentsLoaded());
		if (!TavernBrawlManager.Get().IsTavernBrawlActive)
		{
			enumerable = Enumerable.Where<KeyValuePair<long, CollectionDeck>>(enumerable, (KeyValuePair<long, CollectionDeck> kv) => kv.Value.Type != 6);
		}
		if (!Enumerable.Any<KeyValuePair<long, CollectionDeck>>(enumerable))
		{
			if (callback != null)
			{
				callback();
			}
			return false;
		}
		if (callback != null && !this.m_allDeckContentsListeners.Contains(callback))
		{
			this.m_allDeckContentsListeners.Add(callback);
		}
		if (this.m_pendingRequestDeckContents != null)
		{
			enumerable = Enumerable.Where<KeyValuePair<long, CollectionDeck>>(enumerable, (KeyValuePair<long, CollectionDeck> kv) => !this.m_pendingRequestDeckContents.ContainsKey(kv.Value.ID) || now - this.m_pendingRequestDeckContents[kv.Value.ID] >= 10f);
		}
		IEnumerable<long> enumerable2 = Enumerable.Select<KeyValuePair<long, CollectionDeck>, long>(enumerable, (KeyValuePair<long, CollectionDeck> kv) => kv.Value.ID);
		if (Enumerable.Any<long>(enumerable2))
		{
			long[] array = Enumerable.ToArray<long>(enumerable2);
			if (this.m_pendingRequestDeckContents == null)
			{
				this.m_pendingRequestDeckContents = new Map<long, float>();
			}
			for (int i = 0; i < array.Length; i++)
			{
				this.m_pendingRequestDeckContents[array[i]] = now;
			}
			Network.Get().RequestDeckContents(array);
			return true;
		}
		return true;
	}

	// Token: 0x060012CC RID: 4812 RVA: 0x00052E40 File Offset: 0x00051040
	public void RequestDeckContents(long id)
	{
		CollectionDeck deck = this.GetDeck(id);
		if (deck != null && deck.NetworkContentsLoaded())
		{
			this.FireDeckContentsEvent(id);
			return;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float num;
		if (this.m_pendingRequestDeckContents != null && this.m_pendingRequestDeckContents.TryGetValue(id, out num))
		{
			if (realtimeSinceStartup - num < 10f)
			{
				return;
			}
			this.m_pendingRequestDeckContents.Remove(id);
		}
		if (this.m_pendingRequestDeckContents == null)
		{
			this.m_pendingRequestDeckContents = new Map<long, float>();
		}
		this.m_pendingRequestDeckContents[id] = realtimeSinceStartup;
		Network.Get().RequestDeckContents(new long[]
		{
			id
		});
	}

	// Token: 0x060012CD RID: 4813 RVA: 0x00052EE4 File Offset: 0x000510E4
	public CollectionDeck GetBaseDeck(long id)
	{
		CollectionDeck result;
		if (this.m_baseDecks.TryGetValue(id, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x060012CE RID: 4814 RVA: 0x00052F08 File Offset: 0x00051108
	public string AutoGenerateDeckName(TAG_CLASS classTag)
	{
		string className = GameStrings.GetClassName(classTag);
		int num = 1;
		string text;
		do
		{
			if (num == 1)
			{
				text = GameStrings.Format("GLUE_COLLECTION_CUSTOM_DECKNAME_TEMPLATE", new object[]
				{
					className,
					string.Empty
				});
			}
			else
			{
				text = GameStrings.Format("GLUE_COLLECTION_CUSTOM_DECKNAME_TEMPLATE", new object[]
				{
					className,
					num
				});
			}
			num++;
		}
		while (this.IsDeckNameTaken(text));
		return text;
	}

	// Token: 0x060012CF RID: 4815 RVA: 0x00052F78 File Offset: 0x00051178
	public string GetVanillaHeroCardIDFromClass(TAG_CLASS heroClass)
	{
		string result = string.Empty;
		switch (heroClass)
		{
		case TAG_CLASS.DRUID:
			result = "HERO_06";
			break;
		case TAG_CLASS.HUNTER:
			result = "HERO_05";
			break;
		case TAG_CLASS.MAGE:
			result = "HERO_08";
			break;
		case TAG_CLASS.PALADIN:
			result = "HERO_04";
			break;
		case TAG_CLASS.PRIEST:
			result = "HERO_09";
			break;
		case TAG_CLASS.ROGUE:
			result = "HERO_03";
			break;
		case TAG_CLASS.SHAMAN:
			result = "HERO_02";
			break;
		case TAG_CLASS.WARLOCK:
			result = "HERO_07";
			break;
		case TAG_CLASS.WARRIOR:
			result = "HERO_01";
			break;
		default:
			result = string.Empty;
			break;
		}
		return result;
	}

	// Token: 0x060012D0 RID: 4816 RVA: 0x00053030 File Offset: 0x00051230
	public string GetVanillaHeroCardID(EntityDef HeroSkinEntityDef)
	{
		TAG_CLASS @class = HeroSkinEntityDef.GetClass();
		return CollectionManager.Get().GetVanillaHeroCardIDFromClass(@class);
	}

	// Token: 0x060012D1 RID: 4817 RVA: 0x00053054 File Offset: 0x00051254
	public bool ShouldShowDeckTemplatePageForClass(TAG_CLASS classType)
	{
		int @int = Options.Get().GetInt(Option.SKIP_DECK_TEMPLATE_PAGE_FOR_CLASS_FLAGS, 0);
		int num = 1 << (int)classType;
		return (@int & num) == 0;
	}

	// Token: 0x060012D2 RID: 4818 RVA: 0x00053080 File Offset: 0x00051280
	public void SetShowDeckTemplatePageForClass(TAG_CLASS classType, bool show)
	{
		int num = Options.Get().GetInt(Option.SKIP_DECK_TEMPLATE_PAGE_FOR_CLASS_FLAGS, 0);
		int num2 = 1 << (int)classType;
		num |= num2;
		if (show)
		{
			num ^= num2;
		}
		Options.Get().SetInt(Option.SKIP_DECK_TEMPLATE_PAGE_FOR_CLASS_FLAGS, num);
	}

	// Token: 0x060012D3 RID: 4819 RVA: 0x000530C4 File Offset: 0x000512C4
	public bool ShouldShowWildToStandardTutorial(bool checkPrevSceneIsPlayMode = true)
	{
		return this.ShouldAccountSeeStandardWild() && SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER && (!checkPrevSceneIsPlayMode || SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.TOURNAMENT) && Options.Get().GetBool(Option.NEEDS_TO_MAKE_STANDARD_DECK);
	}

	// Token: 0x060012D4 RID: 4820 RVA: 0x00053118 File Offset: 0x00051318
	private void InitImpl()
	{
		foreach (string text in GameUtils.GetAllCollectibleCardIds())
		{
			EntityDef entityDef = DefLoader.Get().GetEntityDef(text);
			if (entityDef == null)
			{
				Error.AddDevFatal("Failed to find an EntityDef for collectible card {0}", new object[]
				{
					text
				});
				return;
			}
			this.RegisterCard(entityDef, text, TAG_PREMIUM.NORMAL);
			if (entityDef.GetCardSet() != TAG_CARD_SET.HERO_SKINS)
			{
				this.RegisterCard(entityDef, text, TAG_PREMIUM.GOLDEN);
			}
		}
		Network.Get().RegisterNetHandler(215, new Network.NetHandler(this.OnGetDeckContentsResponse), null);
		NetCache.Get().RegisterCollectionManager(new NetCache.NetCacheCallback(this.OnNetCacheReady));
	}

	// Token: 0x060012D5 RID: 4821 RVA: 0x000531F0 File Offset: 0x000513F0
	private void WillReset()
	{
		NetCache.Get().RemoveUpdatedListener(typeof(NetCache.NetCacheDecks), new Action(CollectionManager.s_instance.NetCache_OnDecksReceived));
		this.m_decks.Clear();
		this.m_baseDecks.Clear();
		this.m_preconDecks.Clear();
		this.m_defaultCardbackChangedListeners.Clear();
		this.m_favoriteHeroChangedListeners.Clear();
		this.m_templateDecks.Clear();
		this.m_templateDeckMap.Clear();
		this.m_collectibleCards = new List<CollectibleCard>();
		this.m_collectibleCardIndex = new Map<CollectionManager.CollectibleCardIndex, CollectibleCard>();
		this.m_cardStacksRegistered = false;
	}

	// Token: 0x060012D6 RID: 4822 RVA: 0x0005328C File Offset: 0x0005148C
	private void OnCollectionChanged()
	{
		CollectionManager.DelOnCollectionChanged[] array = this.m_collectionChangedListeners.ToArray();
		foreach (CollectionManager.DelOnCollectionChanged delOnCollectionChanged in array)
		{
			delOnCollectionChanged();
		}
	}

	// Token: 0x060012D7 RID: 4823 RVA: 0x000532C8 File Offset: 0x000514C8
	private List<string> GetCardIDsInSet(TAG_CARD_SET? cardSet, TAG_CLASS? cardClass, TAG_RARITY? cardRarity, TAG_RACE? cardRace)
	{
		List<string> nonHeroCollectibleCardIds = GameUtils.GetNonHeroCollectibleCardIds();
		List<string> list = new List<string>();
		foreach (string text in nonHeroCollectibleCardIds)
		{
			EntityDef entityDef = DefLoader.Get().GetEntityDef(text);
			if (cardClass == null || cardClass.Value == entityDef.GetClass())
			{
				if (cardRarity == null || cardRarity.Value == entityDef.GetRarity())
				{
					if (cardSet == null || cardSet.Value == entityDef.GetCardSet())
					{
						if (cardRace == null || cardRace.Value == entityDef.GetRace())
						{
							list.Add(text);
						}
					}
				}
			}
		}
		return list;
	}

	// Token: 0x060012D8 RID: 4824 RVA: 0x000533C8 File Offset: 0x000515C8
	public int NumCardsOwnedInSet(TAG_CARD_SET cardSet)
	{
		TAG_CARD_SET[] theseCardSets = new TAG_CARD_SET[]
		{
			cardSet
		};
		List<CollectibleCard> list = this.FindCards(null, default(TAG_PREMIUM?), default(int?), theseCardSets, null, null, default(TAG_RARITY?), default(TAG_RACE?), default(bool?), new int?(1), default(bool?), default(bool?), null, null, false);
		int num = 0;
		foreach (CollectibleCard collectibleCard in list)
		{
			num += collectibleCard.OwnedCount;
		}
		return num;
	}

	// Token: 0x060012D9 RID: 4825 RVA: 0x0005348C File Offset: 0x0005168C
	private CollectibleCard RegisterCard(EntityDef entityDef, string cardID, TAG_PREMIUM premium)
	{
		CollectionManager.CollectibleCardIndex key = new CollectionManager.CollectibleCardIndex(cardID, premium);
		CollectibleCard collectibleCard = null;
		if (!this.m_collectibleCardIndex.TryGetValue(key, out collectibleCard))
		{
			CardDbfRecord cardRecord = GameUtils.GetCardRecord(cardID);
			collectibleCard = new CollectibleCard(cardRecord, entityDef, premium);
			this.m_collectibleCards.Add(collectibleCard);
			this.m_collectibleCardIndex.Add(key, collectibleCard);
		}
		return collectibleCard;
	}

	// Token: 0x060012DA RID: 4826 RVA: 0x000534E4 File Offset: 0x000516E4
	private CollectibleCard AddCounts(NetCache.CardStack netStack, EntityDef entityDef)
	{
		return this.AddCounts(entityDef, netStack.Def.Name, netStack.Def.Premium, new DateTime(netStack.Date), netStack.Count, netStack.NumSeen);
	}

	// Token: 0x060012DB RID: 4827 RVA: 0x00053528 File Offset: 0x00051728
	private CollectibleCard AddCounts(EntityDef entityDef, string cardID, TAG_PREMIUM premium, DateTime insertDate, int count, int numSeen)
	{
		if (entityDef == null)
		{
			Debug.LogError(string.Format("CollectionManager.RegisterCardStack(): DefLoader failed to get entity def for {0}", cardID));
			return null;
		}
		this.m_collectionLastModifiedTime = Time.realtimeSinceStartup;
		CollectibleCard collectibleCard = this.RegisterCard(entityDef, cardID, premium);
		collectibleCard.AddCounts(count, numSeen, insertDate);
		return collectibleCard;
	}

	// Token: 0x060012DC RID: 4828 RVA: 0x00053570 File Offset: 0x00051770
	private void AddPreconDeckFromNotice(NetCache.ProfileNoticePreconDeck preconDeckNotice)
	{
		EntityDef entityDef = DefLoader.Get().GetEntityDef(preconDeckNotice.HeroAsset);
		if (entityDef == null)
		{
			return;
		}
		this.AddPreconDeck(entityDef.GetClass(), preconDeckNotice.DeckID);
		NetCache.NetCacheDecks netObject = NetCache.Get().GetNetObject<NetCache.NetCacheDecks>();
		if (netObject == null)
		{
			return;
		}
		NetCache.DeckHeader deckHeader = new NetCache.DeckHeader
		{
			ID = preconDeckNotice.DeckID,
			Name = "precon",
			Hero = entityDef.GetCardId(),
			HeroPower = GameUtils.GetHeroPowerCardIdFromHero(preconDeckNotice.HeroAsset),
			Type = 5,
			SortOrder = preconDeckNotice.DeckID,
			SourceType = 3
		};
		netObject.Decks.Add(deckHeader);
		Network.AckNotice(preconDeckNotice.NoticeID);
	}

	// Token: 0x060012DD RID: 4829 RVA: 0x00053628 File Offset: 0x00051828
	private void AddPreconDeck(TAG_CLASS heroClass, long deckID)
	{
		if (this.m_preconDecks.ContainsKey(heroClass))
		{
			Debug.LogWarning(string.Format("CollectionManager.AddPreconDeck(): Already have a precon deck for class {0}, cannot add deckID {1}", heroClass, deckID));
			return;
		}
		Log.Rachelle.Print(string.Format("CollectionManager.AddPreconDeck() heroClass={0} deckID={1}", heroClass, deckID), new object[0]);
		this.m_preconDecks[heroClass] = new CollectionManager.PreconDeck(deckID);
	}

	// Token: 0x060012DE RID: 4830 RVA: 0x0005369A File Offset: 0x0005189A
	private CollectionDeck AddDeck(NetCache.DeckHeader deckHeader)
	{
		return this.AddDeck(deckHeader, true);
	}

	// Token: 0x060012DF RID: 4831 RVA: 0x000536A4 File Offset: 0x000518A4
	private CollectionDeck AddDeck(NetCache.DeckHeader deckHeader, bool updateNetCache)
	{
		if (deckHeader.Type != 1 && deckHeader.Type != 6)
		{
			Debug.LogWarning(string.Format("CollectionManager.AddDeck(): deckHeader {0} is not of type NORMAL_DECK or TAVERN_BRAWL_DECK", deckHeader));
			return null;
		}
		CollectionDeck collectionDeck = new CollectionDeck
		{
			ID = deckHeader.ID,
			Type = deckHeader.Type,
			Name = deckHeader.Name,
			HeroCardID = deckHeader.Hero,
			HeroPremium = deckHeader.HeroPremium,
			HeroOverridden = deckHeader.HeroOverridden,
			CardBackID = deckHeader.CardBack,
			CardBackOverridden = deckHeader.CardBackOverridden,
			SeasonId = deckHeader.SeasonId,
			NeedsName = deckHeader.NeedsName,
			SortOrder = deckHeader.SortOrder,
			IsWild = deckHeader.IsWild,
			SourceType = deckHeader.SourceType
		};
		if (collectionDeck.NeedsName && string.IsNullOrEmpty(collectionDeck.Name))
		{
			collectionDeck.Name = GameStrings.Format("GLOBAL_BASIC_DECK_NAME", new object[]
			{
				GameStrings.GetClassName(collectionDeck.GetClass())
			});
			Log.JMac.Print(string.Format("Set deck name to {0}", collectionDeck.Name), new object[0]);
		}
		this.m_decks.Add(deckHeader.ID, collectionDeck);
		CollectionDeck value = new CollectionDeck
		{
			ID = deckHeader.ID,
			Type = deckHeader.Type,
			Name = deckHeader.Name,
			HeroCardID = deckHeader.Hero,
			HeroPremium = deckHeader.HeroPremium,
			HeroOverridden = deckHeader.HeroOverridden,
			CardBackID = deckHeader.CardBack,
			CardBackOverridden = deckHeader.CardBackOverridden,
			SeasonId = deckHeader.SeasonId,
			NeedsName = deckHeader.NeedsName,
			SortOrder = deckHeader.SortOrder,
			IsWild = deckHeader.IsWild,
			SourceType = deckHeader.SourceType
		};
		this.m_baseDecks.Add(deckHeader.ID, value);
		if (updateNetCache)
		{
			NetCache.NetCacheDecks netObject = NetCache.Get().GetNetObject<NetCache.NetCacheDecks>();
			netObject.Decks.Add(deckHeader);
		}
		return collectionDeck;
	}

	// Token: 0x060012E0 RID: 4832 RVA: 0x000538C4 File Offset: 0x00051AC4
	private void RemoveDeck(long id)
	{
		this.m_decks.Remove(id);
		this.m_baseDecks.Remove(id);
		NetCache.NetCacheDecks netObject = NetCache.Get().GetNetObject<NetCache.NetCacheDecks>();
		for (int i = 0; i < netObject.Decks.Count; i++)
		{
			NetCache.DeckHeader deckHeader = netObject.Decks[i];
			if (deckHeader.ID == id)
			{
				netObject.Decks.RemoveAt(i);
				break;
			}
		}
	}

	// Token: 0x060012E1 RID: 4833 RVA: 0x00053944 File Offset: 0x00051B44
	private bool IsDeckNameTaken(string name)
	{
		foreach (CollectionDeck collectionDeck in this.GetDecks().Values)
		{
			string name2 = collectionDeck.Name;
			if (name2.Trim().Equals(name, 3))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060012E2 RID: 4834 RVA: 0x000539C0 File Offset: 0x00051BC0
	private void FireDeckContentsEvent(long id)
	{
		CollectionManager.DelOnDeckContents[] array = this.m_deckContentsListeners.ToArray();
		foreach (CollectionManager.DelOnDeckContents delOnDeckContents in array)
		{
			delOnDeckContents(id);
		}
	}

	// Token: 0x060012E3 RID: 4835 RVA: 0x000539FC File Offset: 0x00051BFC
	private void FireAllDeckContentsEvent()
	{
		CollectionManager.DelOnAllDeckContents[] array = this.m_allDeckContentsListeners.ToArray();
		this.m_allDeckContentsListeners.Clear();
		foreach (CollectionManager.DelOnAllDeckContents delOnAllDeckContents in array)
		{
			delOnAllDeckContents();
		}
	}

	// Token: 0x060012E4 RID: 4836 RVA: 0x00053A40 File Offset: 0x00051C40
	private void OnNetCacheReady()
	{
		NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
		Log.Rachelle.Print("CollectionManager.OnNetCacheReady", new object[0]);
		List<string> nonHeroCollectibleCardIds = GameUtils.GetNonHeroCollectibleCardIds();
		foreach (string cardId in nonHeroCollectibleCardIds)
		{
			EntityDef entityDef = DefLoader.Get().GetEntityDef(cardId);
			TAG_CARD_SET cardSet = entityDef.GetCardSet();
			if (!this.m_displayableCardSets.Contains(cardSet))
			{
				this.m_displayableCardSets.Add(cardSet);
			}
		}
		AchieveManager.InitRequests();
		this.UpdateCardsWithNetData();
		NetCache.NetCacheDecks netObject = NetCache.Get().GetNetObject<NetCache.NetCacheDecks>();
		foreach (NetCache.DeckHeader deckHeader in netObject.Decks)
		{
			switch (deckHeader.Type)
			{
			case 1:
			case 6:
				this.AddDeck(deckHeader, false);
				continue;
			case 5:
			{
				EntityDef entityDef2 = DefLoader.Get().GetEntityDef(deckHeader.Hero);
				this.AddPreconDeck(entityDef2.GetClass(), deckHeader.ID);
				continue;
			}
			}
			Debug.LogWarning(string.Format("CollectionManager.OnNetCacheReady(): don't know how to handle deck type {0}", deckHeader.Type));
		}
		NetCache.Get().RegisterUpdatedListener(typeof(NetCache.NetCacheDecks), new Action(CollectionManager.s_instance.NetCache_OnDecksReceived));
		this.UpdateShowAdvancedCMOption();
		CollectionManager.DelOnCollectionLoaded[] array = this.m_collectionLoadedListeners.ToArray();
		foreach (CollectionManager.DelOnCollectionLoaded delOnCollectionLoaded in array)
		{
			delOnCollectionLoaded();
		}
		NetCache.Get().RegisterNewNoticesListener(new NetCache.DelNewNoticesListener(this.OnNewNotices));
		this.m_collectionLoaded = true;
		this.LoadTemplateDecks();
	}

	// Token: 0x060012E5 RID: 4837 RVA: 0x00053C64 File Offset: 0x00051E64
	public void FixedRewardsStartupComplete()
	{
	}

	// Token: 0x060012E6 RID: 4838 RVA: 0x00053C68 File Offset: 0x00051E68
	private void OnNewNotices(List<NetCache.ProfileNotice> newNotices)
	{
		List<NetCache.ProfileNotice> list = newNotices.FindAll((NetCache.ProfileNotice obj) => obj.Type == NetCache.ProfileNotice.NoticeType.PRECON_DECK);
		bool flag = false;
		foreach (NetCache.ProfileNotice profileNotice in list)
		{
			NetCache.ProfileNoticePreconDeck preconDeckNotice = profileNotice as NetCache.ProfileNoticePreconDeck;
			this.AddPreconDeckFromNotice(preconDeckNotice);
			flag = true;
		}
		if (flag)
		{
			NetCache.Get().NetCacheRequestReload(typeof(NetCache.NetCacheDecks));
		}
	}

	// Token: 0x060012E7 RID: 4839 RVA: 0x00053D08 File Offset: 0x00051F08
	private void UpdateShowAdvancedCMOption()
	{
		if (Options.Get().GetBool(Option.SHOW_ADVANCED_COLLECTIONMANAGER, false))
		{
			return;
		}
		NetCache.NetCacheCollection netObject = NetCache.Get().GetNetObject<NetCache.NetCacheCollection>();
		if (netObject.TotalCardsOwned < 116)
		{
			return;
		}
		Options.Get().SetBool(Option.SHOW_ADVANCED_COLLECTIONMANAGER, true);
	}

	// Token: 0x060012E8 RID: 4840 RVA: 0x00053D50 File Offset: 0x00051F50
	private void UpdateDeckHeroArt(string heroCardID)
	{
		TAG_PREMIUM bestCardPremium = this.GetBestCardPremium(heroCardID);
		EntityDef entityDef = DefLoader.Get().GetEntityDef(heroCardID);
		TAG_CLASS @class = entityDef.GetClass();
		CollectionManager.PreconDeck preconDeck = this.GetPreconDeck(@class);
		if (preconDeck != null)
		{
			this.m_preconDecks[@class] = new CollectionManager.PreconDeck(preconDeck.ID);
		}
		List<CollectionDeck> list = Enumerable.ToList<CollectionDeck>(this.m_baseDecks.Values).FindAll((CollectionDeck obj) => obj.HeroCardID.Equals(heroCardID));
		foreach (CollectionDeck collectionDeck in list)
		{
			collectionDeck.HeroPremium = bestCardPremium;
		}
		List<CollectionDeck> list2 = Enumerable.ToList<CollectionDeck>(this.m_decks.Values).FindAll((CollectionDeck obj) => obj.HeroCardID.Equals(heroCardID));
		foreach (CollectionDeck collectionDeck2 in list2)
		{
			collectionDeck2.HeroPremium = bestCardPremium;
		}
	}

	// Token: 0x060012E9 RID: 4841 RVA: 0x00053E98 File Offset: 0x00052098
	private void InsertNewCollectionCard(string cardID, TAG_PREMIUM premium, DateTime insertDate, int count, bool seenBefore)
	{
		EntityDef entityDef = DefLoader.Get().GetEntityDef(cardID);
		int numSeen = (!seenBefore) ? 0 : count;
		CollectibleCard collectibleCard = this.AddCounts(entityDef, cardID, premium, insertDate, count, numSeen);
		if (entityDef.IsHero())
		{
			if (premium == TAG_PREMIUM.GOLDEN)
			{
				this.UpdateDeckHeroArt(cardID);
			}
		}
		else
		{
			this.NotifyNetCacheOfNewCards(new NetCache.CardDefinition
			{
				Name = cardID,
				Premium = premium
			}, insertDate.Ticks, count, seenBefore);
			AchieveManager.Get().NotifyOfCardGained(entityDef, premium, collectibleCard.OwnedCount);
			this.UpdateShowAdvancedCMOption();
		}
	}

	// Token: 0x060012EA RID: 4842 RVA: 0x00053F2C File Offset: 0x0005212C
	private void InsertNewCollectionCards(List<string> cardIDs, List<TAG_PREMIUM> cardPremiums, List<DateTime> insertDates, List<int> counts, bool seenBefore)
	{
		List<EntityDef> list = new List<EntityDef>();
		bool flag = false;
		for (int i = 0; i < cardIDs.Count; i++)
		{
			string text = cardIDs[i];
			TAG_PREMIUM tag_PREMIUM = cardPremiums[i];
			DateTime insertDate = insertDates[i];
			int num = counts[i];
			EntityDef entityDef = DefLoader.Get().GetEntityDef(text);
			int numSeen = (!seenBefore) ? 0 : num;
			this.AddCounts(entityDef, text, tag_PREMIUM, insertDate, num, numSeen);
			flag |= (tag_PREMIUM == TAG_PREMIUM.GOLDEN);
			if (entityDef.IsHero())
			{
				if (tag_PREMIUM == TAG_PREMIUM.GOLDEN)
				{
					this.UpdateDeckHeroArt(text);
				}
			}
			else
			{
				this.NotifyNetCacheOfNewCards(new NetCache.CardDefinition
				{
					Name = text,
					Premium = tag_PREMIUM
				}, insertDate.Ticks, num, seenBefore);
				list.Add(entityDef);
				this.UpdateShowAdvancedCMOption();
			}
		}
		AchieveManager.Get().NotifyOfCardsGained(list, flag);
	}

	// Token: 0x060012EB RID: 4843 RVA: 0x0005401C File Offset: 0x0005221C
	private void RemoveCollectionCard(string cardID, TAG_PREMIUM premium, int count)
	{
		CollectibleCard card = this.GetCard(cardID, premium);
		card.RemoveCounts(count);
		this.m_collectionLastModifiedTime = Time.realtimeSinceStartup;
		int ownedCount = card.OwnedCount;
		foreach (CollectionDeck collectionDeck in this.GetDecks().Values)
		{
			int cardCount = collectionDeck.GetCardCount(cardID, premium);
			if (cardCount > ownedCount)
			{
				int num = cardCount - ownedCount;
				for (int i = 0; i < num; i++)
				{
					collectionDeck.RemoveCard(cardID, premium, true, false);
				}
				if (!CollectionDeckTray.Get().HandleDeletedCardDeckUpdate(cardID))
				{
					collectionDeck.SendChanges();
				}
			}
		}
		this.NotifyNetCacheOfRemovedCards(new NetCache.CardDefinition
		{
			Name = cardID,
			Premium = premium
		}, count);
	}

	// Token: 0x060012EC RID: 4844 RVA: 0x00054110 File Offset: 0x00052310
	private void UpdateCardCounts(NetCache.NetCacheCollection netCacheCards, NetCache.CardDefinition cardDef, int count, int newCount)
	{
		netCacheCards.TotalCardsOwned += count;
		if (cardDef.Premium == TAG_PREMIUM.NORMAL)
		{
			EntityDef entityDef = DefLoader.Get().GetEntityDef(cardDef.Name);
			if (entityDef.IsBasicCardUnlock())
			{
				int num = (!entityDef.IsElite()) ? 2 : 1;
				if (newCount < 0 || newCount > num)
				{
					Debug.LogError(string.Concat(new object[]
					{
						"CollectionManager.UpdateCardCounts: created an illegal stack size of ",
						newCount,
						" for card ",
						entityDef
					}));
					count = 0;
				}
				Map<TAG_CLASS, int> basicCardsUnlockedPerClass;
				Map<TAG_CLASS, int> map = basicCardsUnlockedPerClass = netCacheCards.BasicCardsUnlockedPerClass;
				TAG_CLASS @class;
				TAG_CLASS key = @class = entityDef.GetClass();
				int num2 = basicCardsUnlockedPerClass[@class];
				map[key] = num2 + count;
			}
		}
	}

	// Token: 0x060012ED RID: 4845 RVA: 0x000541C8 File Offset: 0x000523C8
	private void NotifyNetCacheOfRemovedCards(NetCache.CardDefinition cardDef, int count)
	{
		NetCache.NetCacheCollection netObject = NetCache.Get().GetNetObject<NetCache.NetCacheCollection>();
		NetCache.CardStack cardStack = netObject.Stacks.Find((NetCache.CardStack obj) => obj.Def.Name.Equals(cardDef.Name) && obj.Def.Premium == cardDef.Premium);
		if (cardStack == null)
		{
			Debug.LogError("CollectionManager.NotifyNetCacheOfRemovedCards() - trying to remove a card from an empty stack!");
			return;
		}
		cardStack.Count -= count;
		if (cardStack.Count <= 0)
		{
			netObject.Stacks.Remove(cardStack);
		}
		this.UpdateCardCounts(netObject, cardDef, -count, cardStack.Count);
	}

	// Token: 0x060012EE RID: 4846 RVA: 0x00054254 File Offset: 0x00052454
	private void NotifyNetCacheOfNewCards(NetCache.CardDefinition cardDef, long insertDate, int count, bool seenBefore)
	{
		NetCache.NetCacheCollection netObject = NetCache.Get().GetNetObject<NetCache.NetCacheCollection>();
		NetCache.CardStack cardStack = netObject.Stacks.Find((NetCache.CardStack obj) => obj.Def.Name.Equals(cardDef.Name) && obj.Def.Premium == cardDef.Premium);
		if (cardStack == null)
		{
			cardStack = new NetCache.CardStack
			{
				Def = cardDef,
				Date = insertDate,
				Count = count,
				NumSeen = ((!seenBefore) ? 0 : count)
			};
			netObject.Stacks.Add(cardStack);
		}
		else
		{
			if (insertDate > cardStack.Date)
			{
				cardStack.Date = insertDate;
			}
			cardStack.Count += count;
			if (seenBefore)
			{
				cardStack.NumSeen += count;
			}
		}
		this.UpdateCardCounts(netObject, cardDef, count, cardStack.Count);
	}

	// Token: 0x060012EF RID: 4847 RVA: 0x00054328 File Offset: 0x00052528
	private void OnActiveAchievesUpdated(object userData)
	{
		List<Achievement> newCompletedAchieves = AchieveManager.Get().GetNewCompletedAchieves();
		foreach (CollectionManager.DelOnAchievesCompleted delOnAchievesCompleted in this.m_achievesCompletedListeners)
		{
			delOnAchievesCompleted(newCompletedAchieves);
		}
	}

	// Token: 0x060012F0 RID: 4848 RVA: 0x00054390 File Offset: 0x00052590
	private void LoadTemplateDecks()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		List<DeckTemplateDbfRecord> records = GameDbf.DeckTemplate.GetRecords();
		foreach (DeckTemplateDbfRecord deckTemplateDbfRecord in records)
		{
			string @event = deckTemplateDbfRecord.Event;
			if (string.IsNullOrEmpty(@event) || !SpecialEventManager.Get().IsEventActive(@event, false))
			{
				int deckId = deckTemplateDbfRecord.DeckId;
				if (!this.m_templateDeckMap.ContainsKey(deckId))
				{
					DeckDbfRecord record = GameDbf.Deck.GetRecord(deckId);
					if (record == null)
					{
						Debug.LogError(string.Format("Unable to find deck with ID {0}", deckId));
					}
					else
					{
						Map<string, int> map = new Map<string, int>();
						DeckCardDbfRecord deckCardDbfRecord = GameDbf.DeckCard.GetRecord(record.TopCardId);
						while (deckCardDbfRecord != null)
						{
							int cardId = deckCardDbfRecord.CardId;
							CardDbfRecord record2 = GameDbf.Card.GetRecord(cardId);
							if (record2 != null)
							{
								string noteMiniGuid = record2.NoteMiniGuid;
								if (map.ContainsKey(noteMiniGuid))
								{
									Map<string, int> map3;
									Map<string, int> map2 = map3 = map;
									string key2;
									string key = key2 = noteMiniGuid;
									int num = map3[key2];
									map2[key] = num + 1;
								}
								else
								{
									map[noteMiniGuid] = 1;
								}
							}
							else
							{
								Debug.LogError(string.Format("Card ID in deck not found in CARD.XML: {0}", cardId));
							}
							int nextCard = deckCardDbfRecord.NextCard;
							if (nextCard == 0)
							{
								deckCardDbfRecord = null;
							}
							else
							{
								deckCardDbfRecord = GameDbf.DeckCard.GetRecord(nextCard);
							}
						}
						TAG_CLASS classId = (TAG_CLASS)deckTemplateDbfRecord.ClassId;
						List<CollectionManager.TemplateDeck> list = null;
						if (!this.m_templateDecks.TryGetValue(classId, out list))
						{
							list = new List<CollectionManager.TemplateDeck>();
							this.m_templateDecks.Add(classId, list);
						}
						CollectionManager.TemplateDeck templateDeck = new CollectionManager.TemplateDeck
						{
							m_id = deckId,
							m_class = classId,
							m_sortOrder = deckTemplateDbfRecord.SortOrder,
							m_cardIds = map,
							m_title = record.Name,
							m_description = record.Description,
							m_displayTexture = FileUtils.GameAssetPathToName(deckTemplateDbfRecord.DisplayTexture)
						};
						list.Add(templateDeck);
						this.m_templateDeckMap.Add(templateDeck.m_id, templateDeck);
					}
				}
			}
		}
		foreach (KeyValuePair<TAG_CLASS, List<CollectionManager.TemplateDeck>> keyValuePair in this.m_templateDecks)
		{
			keyValuePair.Value.Sort(delegate(CollectionManager.TemplateDeck lhs, CollectionManager.TemplateDeck rhs)
			{
				if (lhs.m_sortOrder != rhs.m_sortOrder)
				{
					return Mathf.Clamp(lhs.m_sortOrder - rhs.m_sortOrder, -1, 1);
				}
				return -Mathf.Clamp(lhs.m_id - rhs.m_id, -1, 1);
			});
		}
		float realtimeSinceStartup2 = Time.realtimeSinceStartup;
		Log.Cameron.Print("_decktemplate: Time spent loading template decks: " + (realtimeSinceStartup2 - realtimeSinceStartup), new object[0]);
	}

	// Token: 0x040009A1 RID: 2465
	private const int NUM_CARDS_GRANTED_POST_TUTORIAL = 96;

	// Token: 0x040009A2 RID: 2466
	private const int NUM_CARDS_TO_UNLOCK_ADVANCED_CM = 116;

	// Token: 0x040009A3 RID: 2467
	private const int NUM_EXPERT_CARDS_TO_UNLOCK_CRAFTING = 20;

	// Token: 0x040009A4 RID: 2468
	public const int NUM_EXPERT_CARDS_TO_UNLOCK_FORGE = 20;

	// Token: 0x040009A5 RID: 2469
	public const int NUM_BASIC_CARDS_PER_CLASS = 20;

	// Token: 0x040009A6 RID: 2470
	public const int NUM_CLASS_CARDS_GRANTED_PER_CLASS_UNLOCK = 6;

	// Token: 0x040009A7 RID: 2471
	public const int MAX_NUM_TEMPLATE_DECKS = 3;

	// Token: 0x040009A8 RID: 2472
	public const int MAX_DECKS_PER_PLAYER = 18;

	// Token: 0x040009A9 RID: 2473
	public const int NUM_CLASSES = 9;

	// Token: 0x040009AA RID: 2474
	public const int MAX_INSTANCES_PER_CARD_ID = 2;

	// Token: 0x040009AB RID: 2475
	public const int MAX_INSTANCES_PER_ELITE_CARD_ID = 1;

	// Token: 0x040009AC RID: 2476
	public const int DEFAULT_MAX_INSTANCES_PER_DECK = 30;

	// Token: 0x040009AD RID: 2477
	private const float PENDING_DECK_CONTENTS_REQUEST_THRESHOLD_SECONDS = 10f;

	// Token: 0x040009AE RID: 2478
	private static CollectionManager s_instance;

	// Token: 0x040009AF RID: 2479
	private bool m_cardStacksRegistered;

	// Token: 0x040009B0 RID: 2480
	private bool m_collectionLoaded;

	// Token: 0x040009B1 RID: 2481
	private Map<long, CollectionDeck> m_decks = new Map<long, CollectionDeck>();

	// Token: 0x040009B2 RID: 2482
	private Map<long, CollectionDeck> m_baseDecks = new Map<long, CollectionDeck>();

	// Token: 0x040009B3 RID: 2483
	private Map<TAG_CLASS, CollectionManager.PreconDeck> m_preconDecks = new Map<TAG_CLASS, CollectionManager.PreconDeck>();

	// Token: 0x040009B4 RID: 2484
	private Map<CollectionManager.DeckTag, CollectionDeck> m_taggedDecks = new Map<CollectionManager.DeckTag, CollectionDeck>();

	// Token: 0x040009B5 RID: 2485
	private Map<TAG_CLASS, List<CollectionManager.TemplateDeck>> m_templateDecks = new Map<TAG_CLASS, List<CollectionManager.TemplateDeck>>();

	// Token: 0x040009B6 RID: 2486
	private Map<int, CollectionManager.TemplateDeck> m_templateDeckMap = new Map<int, CollectionManager.TemplateDeck>();

	// Token: 0x040009B7 RID: 2487
	private List<TAG_CARD_SET> m_displayableCardSets = new List<TAG_CARD_SET>();

	// Token: 0x040009B8 RID: 2488
	private List<CollectionManager.DelOnCollectionLoaded> m_collectionLoadedListeners = new List<CollectionManager.DelOnCollectionLoaded>();

	// Token: 0x040009B9 RID: 2489
	private List<CollectionManager.DelOnCollectionChanged> m_collectionChangedListeners = new List<CollectionManager.DelOnCollectionChanged>();

	// Token: 0x040009BA RID: 2490
	private List<CollectionManager.DelOnDeckCreated> m_deckCreatedListeners = new List<CollectionManager.DelOnDeckCreated>();

	// Token: 0x040009BB RID: 2491
	private List<CollectionManager.DelOnDeckDeleted> m_deckDeletedListeners = new List<CollectionManager.DelOnDeckDeleted>();

	// Token: 0x040009BC RID: 2492
	private List<CollectionManager.DelOnDeckContents> m_deckContentsListeners = new List<CollectionManager.DelOnDeckContents>();

	// Token: 0x040009BD RID: 2493
	private List<CollectionManager.DelOnAllDeckContents> m_allDeckContentsListeners = new List<CollectionManager.DelOnAllDeckContents>();

	// Token: 0x040009BE RID: 2494
	private List<CollectionManager.DelOnNewCardSeen> m_newCardSeenListeners = new List<CollectionManager.DelOnNewCardSeen>();

	// Token: 0x040009BF RID: 2495
	private List<CollectionManager.DelOnCardRewardInserted> m_cardRewardListeners = new List<CollectionManager.DelOnCardRewardInserted>();

	// Token: 0x040009C0 RID: 2496
	private List<CollectionManager.DelOnAchievesCompleted> m_achievesCompletedListeners = new List<CollectionManager.DelOnAchievesCompleted>();

	// Token: 0x040009C1 RID: 2497
	private List<CollectionManager.OnMassDisenchant> m_massDisenchantListeners = new List<CollectionManager.OnMassDisenchant>();

	// Token: 0x040009C2 RID: 2498
	private List<CollectionManager.OnTaggedDeckChanged> m_taggedDeckChangedListeners = new List<CollectionManager.OnTaggedDeckChanged>();

	// Token: 0x040009C3 RID: 2499
	private Map<long, float> m_pendingRequestDeckContents;

	// Token: 0x040009C4 RID: 2500
	private List<CollectibleCard> m_collectibleCards = new List<CollectibleCard>();

	// Token: 0x040009C5 RID: 2501
	private Map<CollectionManager.CollectibleCardIndex, CollectibleCard> m_collectibleCardIndex = new Map<CollectionManager.CollectibleCardIndex, CollectibleCard>();

	// Token: 0x040009C6 RID: 2502
	private float m_collectionLastModifiedTime;

	// Token: 0x040009C7 RID: 2503
	private bool m_accountHasWildCards;

	// Token: 0x040009C8 RID: 2504
	private float m_lastSearchForWildCardsTime;

	// Token: 0x040009C9 RID: 2505
	private bool m_accountEverHadWildCards;

	// Token: 0x040009CA RID: 2506
	private bool m_accountHasRotatedItems;

	// Token: 0x040009CB RID: 2507
	private List<CollectionManager.DefaultCardbackChangedListener> m_defaultCardbackChangedListeners = new List<CollectionManager.DefaultCardbackChangedListener>();

	// Token: 0x040009CC RID: 2508
	private List<CollectionManager.FavoriteHeroChangedListener> m_favoriteHeroChangedListeners = new List<CollectionManager.FavoriteHeroChangedListener>();

	// Token: 0x040009CD RID: 2509
	private bool m_waitingForBoxTransition;

	// Token: 0x040009CE RID: 2510
	private bool m_hasVisitedCollection;

	// Token: 0x040009CF RID: 2511
	private bool m_editMode;

	// Token: 0x040009D0 RID: 2512
	private DeckRuleset m_deckRuleset;

	// Token: 0x02000375 RID: 885
	// (Invoke) Token: 0x06002D57 RID: 11607
	public delegate void DelOnAllDeckContents();

	// Token: 0x02000379 RID: 889
	public class PreconDeck
	{
		// Token: 0x06002D68 RID: 11624 RVA: 0x000E396A File Offset: 0x000E1B6A
		public PreconDeck(long id)
		{
			this.m_id = id;
		}

		// Token: 0x170003A2 RID: 930
		// (get) Token: 0x06002D69 RID: 11625 RVA: 0x000E3979 File Offset: 0x000E1B79
		public long ID
		{
			get
			{
				return this.m_id;
			}
		}

		// Token: 0x04001C42 RID: 7234
		private long m_id;
	}

	// Token: 0x020003A3 RID: 931
	// (Invoke) Token: 0x06003110 RID: 12560
	public delegate bool CollectibleCardFilterFunc(CollectibleCard card);

	// Token: 0x0200045E RID: 1118
	// (Invoke) Token: 0x06003714 RID: 14100
	public delegate void DefaultCardbackChangedCallback(int newDefaultCardBackID, object userData);

	// Token: 0x020005B3 RID: 1459
	// (Invoke) Token: 0x06004158 RID: 16728
	public delegate void DelOnDeckCreated(long id);

	// Token: 0x020005B4 RID: 1460
	// (Invoke) Token: 0x0600415C RID: 16732
	public delegate void DelOnDeckDeleted(long id);

	// Token: 0x020005B5 RID: 1461
	// (Invoke) Token: 0x06004160 RID: 16736
	public delegate void DelOnDeckContents(long id);

	// Token: 0x020005C0 RID: 1472
	// (Invoke) Token: 0x0600419F RID: 16799
	public delegate void OnMassDisenchant(int amount);

	// Token: 0x020005DC RID: 1500
	public enum DeckTag
	{
		// Token: 0x04002A62 RID: 10850
		Editing,
		// Token: 0x04002A63 RID: 10851
		Arena
	}

	// Token: 0x020005DD RID: 1501
	public class TemplateDeck
	{
		// Token: 0x04002A64 RID: 10852
		public int m_id;

		// Token: 0x04002A65 RID: 10853
		public TAG_CLASS m_class;

		// Token: 0x04002A66 RID: 10854
		public int m_sortOrder;

		// Token: 0x04002A67 RID: 10855
		public Map<string, int> m_cardIds = new Map<string, int>();

		// Token: 0x04002A68 RID: 10856
		public string m_title;

		// Token: 0x04002A69 RID: 10857
		public string m_description;

		// Token: 0x04002A6A RID: 10858
		public string m_displayTexture;
	}

	// Token: 0x020005DE RID: 1502
	// (Invoke) Token: 0x0600428E RID: 17038
	public delegate void OnTaggedDeckChanged(CollectionManager.DeckTag tag, CollectionDeck newDeck, CollectionDeck oldDeck, object callbackData);

	// Token: 0x020005DF RID: 1503
	private struct CollectibleCardIndex
	{
		// Token: 0x06004291 RID: 17041 RVA: 0x001410FB File Offset: 0x0013F2FB
		public CollectibleCardIndex(string cardId, TAG_PREMIUM premium)
		{
			this.CardId = cardId;
			this.Premium = premium;
		}

		// Token: 0x04002A6B RID: 10859
		public string CardId;

		// Token: 0x04002A6C RID: 10860
		public TAG_PREMIUM Premium;
	}

	// Token: 0x020005E0 RID: 1504
	private class DefaultCardbackChangedListener : EventListener<CollectionManager.DefaultCardbackChangedCallback>
	{
		// Token: 0x06004293 RID: 17043 RVA: 0x00141113 File Offset: 0x0013F313
		public void Fire(int newDefaultCardBackID)
		{
			this.m_callback(newDefaultCardBackID, this.m_userData);
		}
	}

	// Token: 0x020005E1 RID: 1505
	private class FavoriteHeroChangedListener : EventListener<CollectionManager.FavoriteHeroChangedCallback>
	{
		// Token: 0x06004295 RID: 17045 RVA: 0x0014112F File Offset: 0x0013F32F
		public void Fire(TAG_CLASS heroClass, NetCache.CardDefinition favoriteHero)
		{
			this.m_callback(heroClass, favoriteHero, this.m_userData);
		}
	}

	// Token: 0x020005E2 RID: 1506
	// (Invoke) Token: 0x06004297 RID: 17047
	public delegate void FavoriteHeroChangedCallback(TAG_CLASS heroClass, NetCache.CardDefinition favoriteHero, object userData);

	// Token: 0x020005E3 RID: 1507
	public class DeckSort : IComparer<CollectionDeck>
	{
		// Token: 0x0600429B RID: 17051 RVA: 0x0014114C File Offset: 0x0013F34C
		public int Compare(CollectionDeck a, CollectionDeck b)
		{
			if (!a.IsWild && b.IsWild)
			{
				return -1;
			}
			if (a.IsWild && !b.IsWild)
			{
				return 1;
			}
			return a.SortOrder.CompareTo(b.SortOrder);
		}
	}

	// Token: 0x020005E4 RID: 1508
	// (Invoke) Token: 0x0600429D RID: 17053
	public delegate void DelOnCollectionLoaded();

	// Token: 0x020005E5 RID: 1509
	// (Invoke) Token: 0x060042A1 RID: 17057
	public delegate void DelOnCollectionChanged();

	// Token: 0x020005E6 RID: 1510
	// (Invoke) Token: 0x060042A5 RID: 17061
	public delegate void DelOnNewCardSeen(string cardID, TAG_PREMIUM premium);

	// Token: 0x020005E7 RID: 1511
	// (Invoke) Token: 0x060042A9 RID: 17065
	public delegate void DelOnCardRewardInserted(string cardID, TAG_PREMIUM premium);

	// Token: 0x020005E8 RID: 1512
	// (Invoke) Token: 0x060042AD RID: 17069
	public delegate void DelOnAchievesCompleted(List<Achievement> achievements);
}
