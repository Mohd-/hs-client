using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000324 RID: 804
public class DraftManager
{
	// Token: 0x060029D7 RID: 10711 RVA: 0x000CCF8C File Offset: 0x000CB18C
	public static DraftManager Get()
	{
		if (DraftManager.s_instance == null)
		{
			DraftManager.s_instance = new DraftManager();
		}
		return DraftManager.s_instance;
	}

	// Token: 0x060029D8 RID: 10712 RVA: 0x000CCFA8 File Offset: 0x000CB1A8
	public void RegisterNetHandlers()
	{
		Network network = Network.Get();
		network.RegisterNetHandler(246, new Network.NetHandler(this.OnBegin), null);
		network.RegisterNetHandler(247, new Network.NetHandler(this.OnRetire), null);
		network.RegisterNetHandler(288, new Network.NetHandler(this.OnAckRewards), null);
		network.RegisterNetHandler(248, new Network.NetHandler(this.OnChoicesAndContents), null);
		network.RegisterNetHandler(249, new Network.NetHandler(this.OnChosen), null);
		network.RegisterNetHandler(251, new Network.NetHandler(this.OnError), null);
	}

	// Token: 0x060029D9 RID: 10713 RVA: 0x000CD070 File Offset: 0x000CB270
	public void RemoveNetHandlers()
	{
		Network network = Network.Get();
		network.RemoveNetHandler(246, new Network.NetHandler(this.OnBegin));
		network.RemoveNetHandler(247, new Network.NetHandler(this.OnRetire));
		network.RemoveNetHandler(288, new Network.NetHandler(this.OnAckRewards));
		network.RemoveNetHandler(248, new Network.NetHandler(this.OnChoicesAndContents));
		network.RemoveNetHandler(249, new Network.NetHandler(this.OnChosen));
		network.RemoveNetHandler(251, new Network.NetHandler(this.OnError));
	}

	// Token: 0x060029DA RID: 10714 RVA: 0x000CD131 File Offset: 0x000CB331
	public void RegisterMatchmakerHandlers()
	{
		GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
	}

	// Token: 0x060029DB RID: 10715 RVA: 0x000CD149 File Offset: 0x000CB349
	public void RemoveMatchmakerHandlers()
	{
		GameMgr.Get().UnregisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
	}

	// Token: 0x060029DC RID: 10716 RVA: 0x000CD164 File Offset: 0x000CB364
	public void RegisterStoreHandlers()
	{
		StoreManager.Get().RegisterSuccessfulPurchaseAckListener(new StoreManager.SuccessfulPurchaseAckCallback(this.OnDraftPurchaseAck));
		if (DemoMgr.Get().ArenaIs1WinMode())
		{
			StoreManager.Get().RegisterSuccessfulPurchaseListener(new StoreManager.SuccessfulPurchaseCallback(this.OnDraftPurchaseAck));
		}
	}

	// Token: 0x060029DD RID: 10717 RVA: 0x000CD1B0 File Offset: 0x000CB3B0
	public void RemoveStoreHandlers()
	{
		StoreManager.Get().RemoveSuccessfulPurchaseAckListener(new StoreManager.SuccessfulPurchaseAckCallback(this.OnDraftPurchaseAck));
		if (DemoMgr.Get().ArenaIs1WinMode())
		{
			StoreManager.Get().RemoveSuccessfulPurchaseListener(new StoreManager.SuccessfulPurchaseCallback(this.OnDraftPurchaseAck));
		}
	}

	// Token: 0x060029DE RID: 10718 RVA: 0x000CD1FA File Offset: 0x000CB3FA
	public void RegisterDraftDeckSetListener(DraftManager.DraftDeckSet dlg)
	{
		this.m_draftDeckSetListeners.Add(dlg);
	}

	// Token: 0x060029DF RID: 10719 RVA: 0x000CD208 File Offset: 0x000CB408
	public void RemoveDraftDeckSetListener(DraftManager.DraftDeckSet dlg)
	{
		this.m_draftDeckSetListeners.Remove(dlg);
	}

	// Token: 0x060029E0 RID: 10720 RVA: 0x000CD217 File Offset: 0x000CB417
	public CollectionDeck GetDraftDeck()
	{
		return this.m_draftDeck;
	}

	// Token: 0x060029E1 RID: 10721 RVA: 0x000CD21F File Offset: 0x000CB41F
	public int GetSlot()
	{
		return this.m_currentSlot;
	}

	// Token: 0x060029E2 RID: 10722 RVA: 0x000CD227 File Offset: 0x000CB427
	public int GetLosses()
	{
		return this.m_losses;
	}

	// Token: 0x060029E3 RID: 10723 RVA: 0x000CD22F File Offset: 0x000CB42F
	public int GetWins()
	{
		return this.m_wins;
	}

	// Token: 0x060029E4 RID: 10724 RVA: 0x000CD237 File Offset: 0x000CB437
	public int GetMaxWins()
	{
		return this.m_maxWins;
	}

	// Token: 0x060029E5 RID: 10725 RVA: 0x000CD23F File Offset: 0x000CB43F
	public bool GetIsNewKey()
	{
		return this.m_isNewKey;
	}

	// Token: 0x060029E6 RID: 10726 RVA: 0x000CD247 File Offset: 0x000CB447
	public bool DeckWasActiveDuringSession()
	{
		return this.m_deckActiveDuringSession;
	}

	// Token: 0x060029E7 RID: 10727 RVA: 0x000CD24F File Offset: 0x000CB44F
	public List<RewardData> GetRewards()
	{
		if (this.m_chest != null)
		{
			return this.m_chest.Rewards;
		}
		return new List<RewardData>();
	}

	// Token: 0x060029E8 RID: 10728 RVA: 0x000CD270 File Offset: 0x000CB470
	public void MakeChoice(int choiceNum)
	{
		if (this.m_draftDeck == null)
		{
			Debug.LogWarning("DraftManager.MakeChoice(): Trying to make a draft choice while the draft deck is null");
			return;
		}
		if (this.m_validSlot != this.m_currentSlot)
		{
			return;
		}
		this.m_validSlot++;
		Network.MakeDraftChoice(this.m_draftDeck.ID, this.m_currentSlot, choiceNum);
	}

	// Token: 0x060029E9 RID: 10729 RVA: 0x000CD2CA File Offset: 0x000CB4CA
	public void NotifyOfFinalGame(bool wonFinalGame)
	{
		if (wonFinalGame)
		{
			this.m_wins++;
		}
		else
		{
			this.m_losses++;
		}
	}

	// Token: 0x060029EA RID: 10730 RVA: 0x000CD2F3 File Offset: 0x000CB4F3
	public void FindGame()
	{
		GameMgr.Get().FindGame(5, 2, 0L, 0L);
	}

	// Token: 0x060029EB RID: 10731 RVA: 0x000CD305 File Offset: 0x000CB505
	private void ClearDeckInfo()
	{
		this.m_draftDeck = null;
		this.m_losses = 0;
		this.m_wins = 0;
		this.m_maxWins = int.MaxValue;
		this.m_isNewKey = false;
		this.m_chest = null;
		this.m_deckActiveDuringSession = false;
	}

	// Token: 0x060029EC RID: 10732 RVA: 0x000CD33C File Offset: 0x000CB53C
	private void OnBegin()
	{
		BnetPresenceMgr.Get().SetGameField(3U, "0,0,0");
		Network.BeginDraft newDraftDeckID = Network.GetNewDraftDeckID();
		this.m_draftDeck = new CollectionDeck
		{
			ID = newDraftDeckID.DeckID,
			Type = 4,
			IsWild = true
		};
		this.m_currentSlot = 0;
		this.m_validSlot = 0;
		Log.Arena.Print(string.Format("DraftManager.OnBegin - Got new draft deck with ID: {0}", this.m_draftDeck.ID), new object[0]);
		this.InformDraftDisplayOfChoices(newDraftDeckID.Heroes);
		this.FireDraftDeckSetEvent();
	}

	// Token: 0x060029ED RID: 10733 RVA: 0x000CD3D4 File Offset: 0x000CB5D4
	private void OnRetire()
	{
		Network.DraftRetired retiredDraft = Network.GetRetiredDraft();
		Log.Arena.Print(string.Format("DraftManager.OnRetire deckID={0}", retiredDraft.Deck), new object[0]);
		this.m_chest = retiredDraft.Chest;
		this.InformDraftDisplayOfChoices(new List<NetCache.CardDefinition>());
	}

	// Token: 0x060029EE RID: 10734 RVA: 0x000CD424 File Offset: 0x000CB624
	private void OnAckRewards()
	{
		BnetPresenceMgr.Get().SetGameField(3U, string.Concat(new object[]
		{
			this.m_wins,
			",",
			this.m_losses,
			",1"
		}));
		if (!Options.Get().GetBool(Option.HAS_ACKED_ARENA_REWARDS, false) && UserAttentionManager.CanShowAttentionGrabber("DraftManager.OnAckRewards:" + Option.HAS_ACKED_ARENA_REWARDS))
		{
			NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, new Vector3(155.3f, NotificationManager.DEPTH, 34.5f), GameStrings.Get("VO_INNKEEPER_ARENA_1ST_REWARD"), "VO_INNKEEPER_ARENA_1ST_REWARD", 0f, null);
			Options.Get().SetBool(Option.HAS_ACKED_ARENA_REWARDS, true);
		}
		Network.GetRewardsAckDraftID();
		this.ClearDeckInfo();
	}

	// Token: 0x060029EF RID: 10735 RVA: 0x000CD4F0 File Offset: 0x000CB6F0
	private void OnChoicesAndContents()
	{
		Network.DraftChoicesAndContents draftChoicesAndContents = Network.GetDraftChoicesAndContents();
		this.m_currentSlot = draftChoicesAndContents.Slot;
		this.m_validSlot = draftChoicesAndContents.Slot;
		this.m_draftDeck = new CollectionDeck
		{
			ID = draftChoicesAndContents.DeckInfo.Deck,
			Type = 4,
			HeroCardID = draftChoicesAndContents.Hero.Name,
			HeroPremium = draftChoicesAndContents.Hero.Premium,
			IsWild = true
		};
		Log.Arena.Print(string.Format("DraftManager.OnChoicesAndContents - Draft Deck ID: {0}, Hero Card = {1}", this.m_draftDeck.ID, this.m_draftDeck.HeroCardID), new object[0]);
		foreach (Network.CardUserData cardUserData in draftChoicesAndContents.DeckInfo.Cards)
		{
			string text = (cardUserData.DbId != 0) ? GameUtils.TranslateDbIdToCardId(cardUserData.DbId) : string.Empty;
			Log.Arena.Print(string.Format("DraftManager.OnChoicesAndContents - Draft deck contains card {0}", text), new object[0]);
			for (int i = 0; i < cardUserData.Count; i++)
			{
				if (!this.m_draftDeck.AddCard(text, cardUserData.Premium, false))
				{
					Debug.LogWarning(string.Format("DraftManager.OnChoicesAndContents() - Card {0} could not be added to draft deck", text));
				}
			}
		}
		this.m_losses = draftChoicesAndContents.Losses;
		if (draftChoicesAndContents.Wins > this.m_wins)
		{
			this.m_isNewKey = true;
		}
		else
		{
			this.m_isNewKey = false;
		}
		this.m_wins = draftChoicesAndContents.Wins;
		this.m_maxWins = draftChoicesAndContents.MaxWins;
		this.m_chest = draftChoicesAndContents.Chest;
		if (this.m_losses > 0 && DemoMgr.Get().ArenaIs1WinMode())
		{
			Network.RetireDraftDeck(this.GetDraftDeck().ID, this.GetSlot());
			return;
		}
		if (this.m_wins == 5 && DemoMgr.Get().GetMode() == DemoMode.BLIZZCON_2013)
		{
			DemoMgr.Get().CreateDemoText(GameStrings.Get("GLUE_BLIZZCON2013_ARENA_5_WINS"), false, false);
		}
		else if (this.m_losses == 3 && !Options.Get().GetBool(Option.HAS_LOST_IN_ARENA, false) && UserAttentionManager.CanShowAttentionGrabber("DraftManager.OnChoicesAndContents:" + Option.HAS_LOST_IN_ARENA))
		{
			NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, new Vector3(155.3f, NotificationManager.DEPTH, 34.5f), GameStrings.Get("VO_INNKEEPER_ARENA_3RD_LOSS"), "VO_INNKEEPER_ARENA_3RD_LOSS", 0f, null);
			Options.Get().SetBool(Option.HAS_LOST_IN_ARENA, true);
		}
		this.InformDraftDisplayOfChoices(draftChoicesAndContents.Choices);
	}

	// Token: 0x060029F0 RID: 10736 RVA: 0x000CD7B4 File Offset: 0x000CB9B4
	private void InformDraftDisplayOfChoices(List<NetCache.CardDefinition> choices)
	{
		DraftDisplay draftDisplay = DraftDisplay.Get();
		if (draftDisplay == null)
		{
			return;
		}
		if (choices.Count == 0)
		{
			DraftDisplay.DraftMode draftMode;
			if (this.m_chest == null)
			{
				draftMode = DraftDisplay.DraftMode.ACTIVE_DRAFT_DECK;
				this.m_deckActiveDuringSession = true;
			}
			else
			{
				draftMode = DraftDisplay.DraftMode.IN_REWARDS;
			}
			draftDisplay.SetDraftMode(draftMode);
			return;
		}
		draftDisplay.SetDraftMode(DraftDisplay.DraftMode.DRAFTING);
		draftDisplay.AcceptNewChoices(choices);
	}

	// Token: 0x060029F1 RID: 10737 RVA: 0x000CD810 File Offset: 0x000CBA10
	private void OnChosen()
	{
		Network.DraftChosen chosenAndNext = Network.GetChosenAndNext();
		if (this.m_currentSlot == 0)
		{
			Log.Arena.Print(string.Format("DraftManager.OnChosen(): hero={0} premium={1}", chosenAndNext.ChosenCard.Name, chosenAndNext.ChosenCard.Premium), new object[0]);
			this.m_draftDeck.HeroCardID = chosenAndNext.ChosenCard.Name;
			this.m_draftDeck.HeroPremium = chosenAndNext.ChosenCard.Premium;
		}
		else
		{
			this.m_draftDeck.AddCard(chosenAndNext.ChosenCard.Name, chosenAndNext.ChosenCard.Premium, false);
		}
		this.m_currentSlot++;
		if (this.m_currentSlot > 30 && DraftDisplay.Get() != null)
		{
			DraftDisplay.Get().DoDeckCompleteAnims();
		}
		this.InformDraftDisplayOfChoices(chosenAndNext.NextChoices);
	}

	// Token: 0x060029F2 RID: 10738 RVA: 0x000CD8F8 File Offset: 0x000CBAF8
	private void OnError()
	{
		if (!SceneMgr.Get().IsModeRequested(SceneMgr.Mode.DRAFT))
		{
			return;
		}
		Network.DraftError draftError = Network.GetDraftError();
		DraftDisplay draftDisplay = DraftDisplay.Get();
		switch (draftError)
		{
		case Network.DraftError.DE_UNKNOWN:
			Debug.LogError("DraftManager.OnError - UNKNOWN EXCEPTION - Talk to Brode or Fitch.");
			return;
		case Network.DraftError.DE_NO_LICENSE:
			Debug.LogWarning("DraftManager.OnError - No License.  What does this mean???");
			return;
		case Network.DraftError.DE_RETIRE_FIRST:
			Debug.LogError("DraftManager.OnError - You cannot start a new draft while one is in progress.");
			return;
		case Network.DraftError.DE_NOT_IN_DRAFT:
			if (draftDisplay != null)
			{
				draftDisplay.SetDraftMode(DraftDisplay.DraftMode.NO_ACTIVE_DRAFT);
			}
			return;
		case Network.DraftError.DE_NOT_IN_DRAFT_BUT_COULD_BE:
			if (Options.Get().GetBool(Option.HAS_SEEN_FORGE, false))
			{
				this.RequestDraftStart();
			}
			else
			{
				DraftDisplay.Get().SetDraftMode(DraftDisplay.DraftMode.NO_ACTIVE_DRAFT);
			}
			return;
		case Network.DraftError.DE_FEATURE_DISABLED:
			Debug.LogError("DraftManager.OnError - The Arena is currently disabled. Returning to the hub.");
			if (!SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB))
			{
				SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
				Error.AddWarningLoc("GLOBAL_FEATURE_DISABLED_TITLE", "GLOBAL_FEATURE_DISABLED_MESSAGE_FORGE", new object[0]);
			}
			return;
		}
		Debug.LogError("DraftManager.onError - UNHANDLED ERROR - please send this to Brode. ERROR: " + draftError.ToString());
	}

	// Token: 0x060029F3 RID: 10739 RVA: 0x000CDA24 File Offset: 0x000CBC24
	private bool OnFindGameEvent(FindGameEventData eventData, object userData)
	{
		switch (eventData.m_state)
		{
		case FindGameState.CLIENT_CANCELED:
			DraftDisplay.Get().HandleGameStartupFailure();
			break;
		case FindGameState.CLIENT_ERROR:
		case FindGameState.BNET_QUEUE_CANCELED:
		case FindGameState.BNET_ERROR:
		case FindGameState.SERVER_GAME_CANCELED:
			DraftDisplay.Get().HandleGameStartupFailure();
			break;
		}
		return false;
	}

	// Token: 0x060029F4 RID: 10740 RVA: 0x000CDA8C File Offset: 0x000CBC8C
	private void OnDraftPurchaseAck(Network.Bundle bundle, PaymentMethod paymentMethod, object userData)
	{
		if (this.m_draftDeck != null)
		{
			StoreManager.Get().HideArenaStore();
			return;
		}
		this.RequestDraftStart();
	}

	// Token: 0x060029F5 RID: 10741 RVA: 0x000CDAAA File Offset: 0x000CBCAA
	public void RequestDraftStart()
	{
		Network.StartANewDraft();
	}

	// Token: 0x060029F6 RID: 10742 RVA: 0x000CDAB4 File Offset: 0x000CBCB4
	private void FireDraftDeckSetEvent()
	{
		DraftManager.DraftDeckSet[] array = this.m_draftDeckSetListeners.ToArray();
		foreach (DraftManager.DraftDeckSet draftDeckSet in array)
		{
			draftDeckSet(this.m_draftDeck);
		}
	}

	// Token: 0x04001863 RID: 6243
	private static DraftManager s_instance;

	// Token: 0x04001864 RID: 6244
	private CollectionDeck m_draftDeck;

	// Token: 0x04001865 RID: 6245
	private int m_currentSlot;

	// Token: 0x04001866 RID: 6246
	private int m_validSlot;

	// Token: 0x04001867 RID: 6247
	private int m_losses;

	// Token: 0x04001868 RID: 6248
	private int m_wins;

	// Token: 0x04001869 RID: 6249
	private int m_maxWins = int.MaxValue;

	// Token: 0x0400186A RID: 6250
	private bool m_isNewKey;

	// Token: 0x0400186B RID: 6251
	private bool m_deckActiveDuringSession;

	// Token: 0x0400186C RID: 6252
	private Network.RewardChest m_chest;

	// Token: 0x0400186D RID: 6253
	private List<DraftManager.DraftDeckSet> m_draftDeckSetListeners = new List<DraftManager.DraftDeckSet>();

	// Token: 0x02000801 RID: 2049
	// (Invoke) Token: 0x06004F73 RID: 20339
	public delegate void DraftDeckSet(CollectionDeck deck);
}
