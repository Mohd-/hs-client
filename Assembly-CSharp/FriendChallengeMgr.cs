using System;
using System.Collections.Generic;
using System.Linq;
using bgs;
using bgs.types;
using bnet.protocol.attribute;
using PegasusShared;
using UnityEngine;

// Token: 0x02000270 RID: 624
public class FriendChallengeMgr
{
	// Token: 0x060022E7 RID: 8935 RVA: 0x000ABADC File Offset: 0x000A9CDC
	public static FriendChallengeMgr Get()
	{
		if (FriendChallengeMgr.s_instance == null)
		{
			FriendChallengeMgr.s_instance = new FriendChallengeMgr();
			ApplicationMgr.Get().WillReset += new Action(FriendChallengeMgr.s_instance.WillReset);
		}
		return FriendChallengeMgr.s_instance;
	}

	// Token: 0x060022E8 RID: 8936 RVA: 0x000ABB14 File Offset: 0x000A9D14
	public void OnLoggedIn()
	{
		Network.Get().SetPartyHandler(new Network.PartyHandler(this.OnPartyUpdate));
		NetCache.Get().RegisterFriendChallenge(new NetCache.NetCacheCallback(this.OnNetCacheReady));
		SceneMgr.Get().RegisterSceneUnloadedEvent(new SceneMgr.SceneUnloadedCallback(this.OnSceneUnloaded));
		SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
		BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
		BnetFriendMgr.Get().AddChangeListener(new BnetFriendMgr.ChangeCallback(this.OnFriendsChanged));
		BnetNearbyPlayerMgr.Get().AddChangeListener(new BnetNearbyPlayerMgr.ChangeCallback(this.OnNearbyPlayersChanged));
		BnetEventMgr.Get().AddChangeListener(new BnetEventMgr.ChangeCallback(this.OnBnetEventOccurred));
		GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
		BnetParty.OnJoined += new BnetParty.JoinedHandler(this.BnetParty_OnJoined);
		BnetParty.RegisterAttributeChangedHandler("WTCG.Friendly.DeclineReason", new BnetParty.PartyAttributeChangedHandler(this.BnetParty_OnPartyAttributeChanged_DeclineReason));
		BnetParty.RegisterAttributeChangedHandler("error", new BnetParty.PartyAttributeChangedHandler(this.BnetParty_OnPartyAttributeChanged_Error));
		this.AddChangedListener(new FriendChallengeMgr.ChangedCallback(this.OnChallengeChanged));
		BnetPresenceMgr.Get().SetGameField(19U, BattleNet.GetVersion());
		BnetPresenceMgr.Get().SetGameField(20U, BattleNet.GetEnvironment());
	}

	// Token: 0x060022E9 RID: 8937 RVA: 0x000ABC60 File Offset: 0x000A9E60
	private void BnetParty_OnJoined(OnlineEventType evt, PartyInfo party, LeaveReason? reason)
	{
		if (party.Type != 1)
		{
			return;
		}
		if (evt == null)
		{
			long? partyAttributeLong = BnetParty.GetPartyAttributeLong(party.Id, "WTCG.Game.ScenarioId");
			if (partyAttributeLong != null)
			{
				this.m_scenarioId = (int)partyAttributeLong.Value;
				TavernBrawlMission tavernBrawlMission = TavernBrawlManager.Get().CurrentMission();
				this.m_isChallengeTavernBrawl = (tavernBrawlMission != null && this.m_scenarioId == tavernBrawlMission.missionId);
			}
			long? partyAttributeLong2 = BnetParty.GetPartyAttributeLong(party.Id, "WTCG.Format.Type");
			if (partyAttributeLong2 != null)
			{
				this.m_challengeFormatType = (int)partyAttributeLong2.Value;
			}
			else
			{
				this.m_challengeFormatType = 0;
			}
		}
		else if (evt == 1)
		{
			if (!Enumerable.Any<PartyInfo>(BnetParty.GetJoinedParties(), (PartyInfo i) => i.Type == 1))
			{
				this.m_scenarioId = 2;
			}
		}
	}

	// Token: 0x060022EA RID: 8938 RVA: 0x000ABD4C File Offset: 0x000A9F4C
	private void BnetParty_OnPartyAttributeChanged_DeclineReason(PartyInfo party, string attributeKey, Variant value)
	{
		if (party.Type != 1)
		{
			return;
		}
		if (!this.DidSendChallenge())
		{
			return;
		}
		if (!value.HasIntValue)
		{
			return;
		}
		FriendChallengeMgr.DeclineReason declineReason = (FriendChallengeMgr.DeclineReason)value.IntValue;
		string text = null;
		switch (declineReason)
		{
		case FriendChallengeMgr.DeclineReason.NoValidDeck:
			text = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGEE_NO_DECK";
			break;
		case FriendChallengeMgr.DeclineReason.StandardNoValidDeck:
			text = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGEE_NO_STANDARD_DECK";
			break;
		case FriendChallengeMgr.DeclineReason.TavernBrawlNoValidDeck:
			text = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGEE_NO_TAVERN_BRAWL_DECK";
			break;
		case FriendChallengeMgr.DeclineReason.TavernBrawlNotUnlocked:
			text = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGEE_TAVERN_BRAWL_LOCKED";
			break;
		case FriendChallengeMgr.DeclineReason.UserIsBusy:
			text = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGEE_USER_IS_BUSY";
			break;
		case FriendChallengeMgr.DeclineReason.NotSeenWild:
			text = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGEE_NOT_SEEN_WILD";
			break;
		}
		if (text != null)
		{
			if (this.m_challengeDialog != null)
			{
				this.m_challengeDialog.Hide();
				this.m_challengeDialog = null;
			}
			this.m_hasSeenDeclinedReason = true;
			AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
			popupInfo.m_headerText = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER");
			popupInfo.m_text = GameStrings.Get(text);
			popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
			popupInfo.m_layerToUse = new GameLayer?(GameLayer.HighPriorityUI);
			DialogManager.Get().ShowPopup(popupInfo);
		}
	}

	// Token: 0x060022EB RID: 8939 RVA: 0x000ABE64 File Offset: 0x000AA064
	private void BnetParty_OnPartyAttributeChanged_Error(PartyInfo party, string attributeKey, Variant value)
	{
		if (party.Type != 1)
		{
			return;
		}
		if (this.DidReceiveChallenge() && value.HasIntValue)
		{
			Log.Party.Print(LogLevel.Error, "BnetParty_OnPartyAttributeChanged_Error - code={0}", new object[]
			{
				value.IntValue
			});
			BnetErrorInfo info = new BnetErrorInfo(2, 12, (uint)value.IntValue);
			GameMgr.Get().OnBnetError(info, null);
		}
		if (BnetParty.IsLeader(party.Id) && !ProtocolHelper.IsNone(value))
		{
			BnetParty.ClearPartyAttribute(party.Id, attributeKey);
		}
	}

	// Token: 0x060022EC RID: 8940 RVA: 0x000ABEFC File Offset: 0x000AA0FC
	public void OnStoreOpened()
	{
		this.UpdateMyAvailability();
	}

	// Token: 0x060022ED RID: 8941 RVA: 0x000ABF04 File Offset: 0x000AA104
	public void OnStoreClosed()
	{
		this.UpdateMyAvailability();
	}

	// Token: 0x060022EE RID: 8942 RVA: 0x000ABF0C File Offset: 0x000AA10C
	public BnetPlayer GetChallengee()
	{
		return this.m_challengee;
	}

	// Token: 0x060022EF RID: 8943 RVA: 0x000ABF14 File Offset: 0x000AA114
	public BnetPlayer GetChallenger()
	{
		return this.m_challenger;
	}

	// Token: 0x060022F0 RID: 8944 RVA: 0x000ABF1C File Offset: 0x000AA11C
	public bool DidReceiveChallenge()
	{
		return this.m_challengerPending || (this.m_challenger != null && this.m_challengee == BnetPresenceMgr.Get().GetMyPlayer());
	}

	// Token: 0x060022F1 RID: 8945 RVA: 0x000ABF4C File Offset: 0x000AA14C
	public bool DidSendChallenge()
	{
		return this.m_challengee != null && this.m_challenger == BnetPresenceMgr.Get().GetMyPlayer();
	}

	// Token: 0x060022F2 RID: 8946 RVA: 0x000ABF79 File Offset: 0x000AA179
	public bool HasChallenge()
	{
		return this.DidSendChallenge() || this.DidReceiveChallenge();
	}

	// Token: 0x060022F3 RID: 8947 RVA: 0x000ABF8F File Offset: 0x000AA18F
	public BnetPlayer GetOpponent(BnetPlayer player)
	{
		if (player == this.m_challenger)
		{
			return this.m_challengee;
		}
		if (player == this.m_challengee)
		{
			return this.m_challenger;
		}
		return null;
	}

	// Token: 0x060022F4 RID: 8948 RVA: 0x000ABFB8 File Offset: 0x000AA1B8
	public BnetPlayer GetMyOpponent()
	{
		BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
		return this.GetOpponent(myPlayer);
	}

	// Token: 0x060022F5 RID: 8949 RVA: 0x000ABFD8 File Offset: 0x000AA1D8
	public bool CanChallenge(BnetPlayer player)
	{
		if (player == null)
		{
			return false;
		}
		BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
		if (player == myPlayer)
		{
			return false;
		}
		if (!this.AmIAvailable())
		{
			return false;
		}
		if (TavernBrawlManager.Get().ShouldNewFriendlyChallengeBeTavernBrawl())
		{
			TavernBrawlMission tavernBrawlMission = TavernBrawlManager.Get().CurrentMission();
			if (tavernBrawlMission.canCreateDeck && !TavernBrawlManager.Get().HasValidDeck())
			{
				return false;
			}
		}
		BnetGameAccount hearthstoneGameAccount = player.GetHearthstoneGameAccount();
		if (hearthstoneGameAccount == null)
		{
			return false;
		}
		if (!hearthstoneGameAccount.IsOnline())
		{
			return false;
		}
		if (!hearthstoneGameAccount.CanBeInvitedToGame())
		{
			return false;
		}
		if (ApplicationMgr.IsPublic())
		{
			BnetGameAccount hearthstoneGameAccount2 = myPlayer.GetHearthstoneGameAccount();
			if (string.Compare(hearthstoneGameAccount.GetClientVersion(), hearthstoneGameAccount2.GetClientVersion()) != 0)
			{
				return false;
			}
			if (string.Compare(hearthstoneGameAccount.GetClientEnv(), hearthstoneGameAccount2.GetClientEnv()) != 0)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060022F6 RID: 8950 RVA: 0x000AC0B8 File Offset: 0x000AA2B8
	public bool AmIAvailable()
	{
		if (!this.m_netCacheReady)
		{
			return false;
		}
		if (!this.m_myPlayerReady)
		{
			return false;
		}
		if (SpectatorManager.Get().IsInSpectatorMode())
		{
			return false;
		}
		BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
		BnetGameAccount hearthstoneGameAccount = myPlayer.GetHearthstoneGameAccount();
		return !(hearthstoneGameAccount == null) && hearthstoneGameAccount.CanBeInvitedToGame();
	}

	// Token: 0x060022F7 RID: 8951 RVA: 0x000AC116 File Offset: 0x000AA316
	public bool DidISelectDeck()
	{
		if (this.DidSendChallenge())
		{
			return this.m_challengerDeckSelected;
		}
		return !this.DidReceiveChallenge() || this.m_challengeeDeckSelected;
	}

	// Token: 0x060022F8 RID: 8952 RVA: 0x000AC13D File Offset: 0x000AA33D
	public bool DidOpponentSelectDeck()
	{
		if (this.DidSendChallenge())
		{
			return this.m_challengeeDeckSelected;
		}
		return !this.DidReceiveChallenge() || this.m_challengerDeckSelected;
	}

	// Token: 0x060022F9 RID: 8953 RVA: 0x000AC164 File Offset: 0x000AA364
	public static void ShowChallengerNeedsToCreateTavernBrawlDeckAlert()
	{
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER");
		popupInfo.m_text = GameStrings.Format("GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGER_NO_TAVERN_BRAWL_DECK", new object[0]);
		popupInfo.m_showAlertIcon = true;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
		popupInfo.m_layerToUse = new GameLayer?(GameLayer.HighPriorityUI);
		DialogManager.Get().ShowPopup(popupInfo);
	}

	// Token: 0x060022FA RID: 8954 RVA: 0x000AC1C4 File Offset: 0x000AA3C4
	public void SendChallenge(BnetPlayer player, FormatType formatType, bool isTavernBrawl)
	{
		if (!this.CanChallenge(player))
		{
			return;
		}
		if (isTavernBrawl)
		{
			TavernBrawlManager.Get().EnsureScenarioDataReady(delegate
			{
				this.TavernBrawl_SendChallenge_OnEnsureServerDataReady(player, formatType);
			});
			return;
		}
		this.SendChallenge_Internal(player, formatType, isTavernBrawl);
	}

	// Token: 0x060022FB RID: 8955 RVA: 0x000AC230 File Offset: 0x000AA430
	private void TavernBrawl_SendChallenge_OnEnsureServerDataReady(BnetPlayer player, FormatType formatType)
	{
		TavernBrawlManager tavernBrawlManager = TavernBrawlManager.Get();
		if (!this.CanChallenge(player))
		{
			return;
		}
		if (!tavernBrawlManager.IsTavernBrawlActive)
		{
			return;
		}
		if (this.HasChallenge())
		{
			return;
		}
		if (tavernBrawlManager.CurrentMission().canCreateDeck && !tavernBrawlManager.HasValidDeck())
		{
			FriendChallengeMgr.ShowChallengerNeedsToCreateTavernBrawlDeckAlert();
			return;
		}
		this.SendChallenge_Internal(player, formatType, true);
	}

	// Token: 0x060022FC RID: 8956 RVA: 0x000AC294 File Offset: 0x000AA494
	private void SendChallenge_Internal(BnetPlayer player, FormatType formatType, bool isTavernBrawl)
	{
		this.m_challenger = BnetPresenceMgr.Get().GetMyPlayer();
		this.m_challengerId = this.m_challenger.GetHearthstoneGameAccount().GetId();
		this.m_challengee = player;
		this.m_hasSeenDeclinedReason = false;
		this.m_scenarioId = 2;
		this.m_isChallengeTavernBrawl = false;
		this.m_challengeFormatType = formatType;
		if (isTavernBrawl)
		{
			this.m_scenarioId = TavernBrawlManager.Get().CurrentMission().missionId;
			PresenceMgr.Get().SetStatus(new Enum[]
			{
				PresenceStatus.TAVERN_BRAWL_FRIENDLY_WAITING
			});
			this.m_isChallengeTavernBrawl = true;
		}
		Network.SendFriendChallenge(player.GetHearthstoneGameAccount().GetId(), this.m_scenarioId, formatType);
		this.UpdateMyAvailability();
		this.FireChangedEvent(FriendChallengeEvent.I_SENT_CHALLENGE, player);
	}

	// Token: 0x060022FD RID: 8957 RVA: 0x000AC34C File Offset: 0x000AA54C
	public void CancelChallenge()
	{
		if (!this.HasChallenge())
		{
			return;
		}
		if (this.DidSendChallenge())
		{
			this.RescindChallenge();
		}
		else if (this.DidReceiveChallenge())
		{
			this.DeclineChallenge();
		}
	}

	// Token: 0x060022FE RID: 8958 RVA: 0x000AC38C File Offset: 0x000AA58C
	public void AcceptChallenge()
	{
		if (!this.DidReceiveChallenge())
		{
			return;
		}
		this.m_challengeeAccepted = true;
		Network.AcceptFriendChallenge(this.m_partyId);
		this.FireChangedEvent(FriendChallengeEvent.I_ACCEPTED_CHALLENGE, this.m_challenger);
	}

	// Token: 0x060022FF RID: 8959 RVA: 0x000AC3BC File Offset: 0x000AA5BC
	public void DeclineChallenge()
	{
		if (!this.DidReceiveChallenge())
		{
			return;
		}
		this.RevertTavernBrawlStatus();
		Network.DeclineFriendChallenge(this.m_partyId);
		BnetPlayer challenger = this.m_challenger;
		this.CleanUpChallengeData(true);
		this.FireChangedEvent(FriendChallengeEvent.I_DECLINED_CHALLENGE, challenger);
	}

	// Token: 0x06002300 RID: 8960 RVA: 0x000AC400 File Offset: 0x000AA600
	public void RescindChallenge()
	{
		if (!this.DidSendChallenge())
		{
			return;
		}
		this.RevertTavernBrawlStatus();
		Network.RescindFriendChallenge(this.m_partyId);
		BnetPlayer challengee = this.m_challengee;
		this.CleanUpChallengeData(true);
		this.FireChangedEvent(FriendChallengeEvent.I_RESCINDED_CHALLENGE, challengee);
	}

	// Token: 0x06002301 RID: 8961 RVA: 0x000AC444 File Offset: 0x000AA644
	public bool IsChallengeStandardDuel()
	{
		return this.HasChallenge() && !this.m_isChallengeTavernBrawl && this.m_challengeFormatType == 2;
	}

	// Token: 0x06002302 RID: 8962 RVA: 0x000AC478 File Offset: 0x000AA678
	public bool IsChallengeWildDuel()
	{
		return this.HasChallenge() && !this.m_isChallengeTavernBrawl && this.m_challengeFormatType == 1;
	}

	// Token: 0x06002303 RID: 8963 RVA: 0x000AC4A9 File Offset: 0x000AA6A9
	public bool IsChallengeTavernBrawl()
	{
		return this.HasChallenge() && this.m_isChallengeTavernBrawl;
	}

	// Token: 0x06002304 RID: 8964 RVA: 0x000AC4BE File Offset: 0x000AA6BE
	public void SkipDeckSelection()
	{
		this.SelectDeck(1L);
	}

	// Token: 0x06002305 RID: 8965 RVA: 0x000AC4C8 File Offset: 0x000AA6C8
	public void SelectDeck(long deckId)
	{
		if (this.DidSendChallenge())
		{
			this.m_challengerDeckSelected = true;
		}
		else
		{
			if (!this.DidReceiveChallenge())
			{
				return;
			}
			this.m_challengeeDeckSelected = true;
		}
		Network.ChooseFriendChallengeDeck(this.m_partyId, deckId);
		this.FireChangedEvent(FriendChallengeEvent.SELECTED_DECK, BnetPresenceMgr.Get().GetMyPlayer());
	}

	// Token: 0x06002306 RID: 8966 RVA: 0x000AC524 File Offset: 0x000AA724
	public void DeselectDeck()
	{
		if (this.DidSendChallenge() && this.m_challengerDeckSelected)
		{
			this.m_challengerDeckSelected = false;
		}
		else
		{
			if (!this.DidReceiveChallenge() || !this.m_challengeeDeckSelected)
			{
				return;
			}
			this.m_challengeeDeckSelected = false;
		}
		Network.ChooseFriendChallengeDeck(this.m_partyId, 0L);
		this.FireChangedEvent(FriendChallengeEvent.DESELECTED_DECK, BnetPresenceMgr.Get().GetMyPlayer());
	}

	// Token: 0x06002307 RID: 8967 RVA: 0x000AC595 File Offset: 0x000AA795
	public int GetScenarioId()
	{
		return this.m_scenarioId;
	}

	// Token: 0x06002308 RID: 8968 RVA: 0x000AC59D File Offset: 0x000AA79D
	public bool AddChangedListener(FriendChallengeMgr.ChangedCallback callback)
	{
		return this.AddChangedListener(callback, null);
	}

	// Token: 0x06002309 RID: 8969 RVA: 0x000AC5A8 File Offset: 0x000AA7A8
	public bool AddChangedListener(FriendChallengeMgr.ChangedCallback callback, object userData)
	{
		FriendChallengeMgr.ChangedListener changedListener = new FriendChallengeMgr.ChangedListener();
		changedListener.SetCallback(callback);
		changedListener.SetUserData(userData);
		if (this.m_changedListeners.Contains(changedListener))
		{
			return false;
		}
		this.m_changedListeners.Add(changedListener);
		return true;
	}

	// Token: 0x0600230A RID: 8970 RVA: 0x000AC5E9 File Offset: 0x000AA7E9
	public bool RemoveChangedListener(FriendChallengeMgr.ChangedCallback callback)
	{
		return this.RemoveChangedListener(callback, null);
	}

	// Token: 0x0600230B RID: 8971 RVA: 0x000AC5F4 File Offset: 0x000AA7F4
	public bool RemoveChangedListener(FriendChallengeMgr.ChangedCallback callback, object userData)
	{
		FriendChallengeMgr.ChangedListener changedListener = new FriendChallengeMgr.ChangedListener();
		changedListener.SetCallback(callback);
		changedListener.SetUserData(userData);
		return this.m_changedListeners.Remove(changedListener);
	}

	// Token: 0x0600230C RID: 8972 RVA: 0x000AC624 File Offset: 0x000AA824
	private void OnPartyUpdate(PartyEvent[] updates)
	{
		foreach (PartyEvent partyEvent in updates)
		{
			BnetEntityId partyId = BnetEntityId.CreateFromEntityId(partyEvent.partyId);
			BnetGameAccountId otherMemberId = BnetGameAccountId.CreateFromEntityId(partyEvent.otherMemberId);
			if (partyEvent.eventName == "s1")
			{
				if (partyEvent.eventData == "wait")
				{
					this.OnPartyUpdate_CreatedParty(partyId, otherMemberId);
				}
				else if (partyEvent.eventData == "deck")
				{
					if (this.DidReceiveChallenge() && this.m_challengerDeckSelected)
					{
						this.m_challengerDeckSelected = false;
						this.FireChangedEvent(FriendChallengeEvent.DESELECTED_DECK, this.m_challenger);
					}
				}
				else if (partyEvent.eventData == "game")
				{
					if (this.DidReceiveChallenge())
					{
						this.m_challengerDeckSelected = true;
						this.FireChangedEvent(FriendChallengeEvent.SELECTED_DECK, this.m_challenger);
					}
				}
				else if (partyEvent.eventData == "goto")
				{
					this.m_challengerDeckSelected = false;
				}
			}
			else if (partyEvent.eventName == "s2")
			{
				if (partyEvent.eventData == "wait")
				{
					this.OnPartyUpdate_JoinedParty(partyId, otherMemberId);
				}
				else if (partyEvent.eventData == "deck")
				{
					if (this.DidSendChallenge())
					{
						if (this.m_challengeeAccepted)
						{
							this.m_challengeeDeckSelected = false;
							this.FireChangedEvent(FriendChallengeEvent.DESELECTED_DECK, this.m_challengee);
						}
						else
						{
							this.m_challengeeAccepted = true;
							this.FireChangedEvent(FriendChallengeEvent.OPPONENT_ACCEPTED_CHALLENGE, this.m_challengee);
						}
					}
				}
				else if (partyEvent.eventData == "game")
				{
					if (this.DidSendChallenge())
					{
						this.m_challengeeDeckSelected = true;
						this.FireChangedEvent(FriendChallengeEvent.SELECTED_DECK, this.m_challengee);
					}
				}
				else if (partyEvent.eventData == "goto")
				{
					this.m_challengeeDeckSelected = false;
				}
			}
			else if (partyEvent.eventName == "left")
			{
				if (this.DidSendChallenge())
				{
					BnetPlayer challengee = this.m_challengee;
					bool challengeeAccepted = this.m_challengeeAccepted;
					this.RevertTavernBrawlStatus();
					this.CleanUpChallengeData(true);
					if (challengeeAccepted)
					{
						this.FireChangedEvent(FriendChallengeEvent.OPPONENT_CANCELED_CHALLENGE, challengee);
					}
					else
					{
						this.FireChangedEvent(FriendChallengeEvent.OPPONENT_DECLINED_CHALLENGE, challengee);
					}
				}
				else if (this.DidReceiveChallenge())
				{
					BnetPlayer challenger = this.m_challenger;
					bool challengeeAccepted2 = this.m_challengeeAccepted;
					this.RevertTavernBrawlStatus();
					this.CleanUpChallengeData(true);
					if (challenger != null)
					{
						if (challengeeAccepted2)
						{
							this.FireChangedEvent(FriendChallengeEvent.OPPONENT_CANCELED_CHALLENGE, challenger);
						}
						else
						{
							this.FireChangedEvent(FriendChallengeEvent.OPPONENT_RESCINDED_CHALLENGE, challenger);
						}
					}
				}
			}
		}
	}

	// Token: 0x0600230D RID: 8973 RVA: 0x000AC8F0 File Offset: 0x000AAAF0
	private void OnPartyUpdate_CreatedParty(BnetEntityId partyId, BnetGameAccountId otherMemberId)
	{
		this.m_partyId = partyId;
		if (this.m_challengeePartyId != null && this.m_challengeePartyId != this.m_partyId)
		{
			this.ResolveChallengeConflict();
		}
		else
		{
			this.m_challengeePartyId = this.m_partyId;
			this.UpdateChallengeSentDialog();
		}
	}

	// Token: 0x0600230E RID: 8974 RVA: 0x000AC948 File Offset: 0x000AAB48
	private void OnPartyUpdate_JoinedParty(BnetEntityId partyId, BnetGameAccountId otherMemberId)
	{
		if (this.DidSendChallenge())
		{
			BnetGameAccountId id = this.m_challengee.GetHearthstoneGameAccount().GetId();
			if (id == otherMemberId)
			{
				if (partyId != this.m_partyId)
				{
					this.m_challengeePartyId = partyId;
					if (this.m_partyId == null)
					{
						return;
					}
					this.ResolveChallengeConflict();
				}
				return;
			}
		}
		if (!BnetUtils.CanReceiveChallengeFrom(otherMemberId))
		{
			Network.DeclineFriendChallenge(partyId);
			return;
		}
		if (!this.AmIAvailable())
		{
			Network.DeclineFriendChallenge(partyId);
			return;
		}
		this.HandleJoinedParty(partyId, otherMemberId);
	}

	// Token: 0x0600230F RID: 8975 RVA: 0x000AC9DA File Offset: 0x000AABDA
	private void OnNetCacheReady()
	{
		NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
		this.m_netCacheReady = true;
		if (SceneMgr.Get().GetMode() == SceneMgr.Mode.FATAL_ERROR)
		{
			return;
		}
		this.UpdateMyAvailability();
	}

	// Token: 0x06002310 RID: 8976 RVA: 0x000ACA11 File Offset: 0x000AAC11
	private void OnSceneUnloaded(SceneMgr.Mode prevMode, Scene prevScene, object userData)
	{
		if (Network.IsLoggedIn() && prevMode != SceneMgr.Mode.GAMEPLAY)
		{
			this.UpdateMyAvailability();
		}
	}

	// Token: 0x06002311 RID: 8977 RVA: 0x000ACA2C File Offset: 0x000AAC2C
	private void OnSceneLoaded(SceneMgr.Mode mode, Scene scene, object userData)
	{
		SceneMgr.Mode prevMode = SceneMgr.Get().GetPrevMode();
		if (prevMode != SceneMgr.Mode.GAMEPLAY)
		{
			return;
		}
		if (mode == SceneMgr.Mode.GAMEPLAY)
		{
			return;
		}
		if (mode == SceneMgr.Mode.FATAL_ERROR)
		{
			return;
		}
		this.m_netCacheReady = false;
		if (mode == SceneMgr.Mode.FRIENDLY || TavernBrawlManager.IsInTavernBrawlFriendlyChallenge())
		{
			this.UpdateMyAvailability();
		}
		else
		{
			this.CleanUpChallengeData(true);
		}
		NetCache.Get().RegisterFriendChallenge(new NetCache.NetCacheCallback(this.OnNetCacheReady));
	}

	// Token: 0x06002312 RID: 8978 RVA: 0x000ACAA0 File Offset: 0x000AACA0
	private void OnPlayersChanged(BnetPlayerChangelist changelist, object userData)
	{
		BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
		BnetPlayerChange bnetPlayerChange = changelist.FindChange(myPlayer);
		if (bnetPlayerChange != null)
		{
			BnetGameAccount hearthstoneGameAccount = myPlayer.GetHearthstoneGameAccount();
			if (hearthstoneGameAccount != null && !this.m_myPlayerReady && hearthstoneGameAccount.HasGameField(20U) && hearthstoneGameAccount.HasGameField(19U))
			{
				this.m_myPlayerReady = true;
				this.UpdateMyAvailability();
			}
			if (!this.AmIAvailable() && this.m_challengerPending)
			{
				Network.DeclineFriendChallenge(this.m_partyId);
				this.CleanUpChallengeData(true);
			}
		}
		if (this.m_challengerPending)
		{
			bnetPlayerChange = changelist.FindChange(this.m_challengerId);
			if (bnetPlayerChange != null)
			{
				BnetPlayer player = bnetPlayerChange.GetPlayer();
				if (player.IsDisplayable())
				{
					this.m_challenger = player;
					this.m_challengerPending = false;
					this.FireChangedEvent(FriendChallengeEvent.I_RECEIVED_CHALLENGE, this.m_challenger);
				}
			}
		}
	}

	// Token: 0x06002313 RID: 8979 RVA: 0x000ACB80 File Offset: 0x000AAD80
	private void OnFriendsChanged(BnetFriendChangelist changelist, object userData)
	{
		if (!this.HasChallenge())
		{
			return;
		}
		List<BnetPlayer> removedFriends = changelist.GetRemovedFriends();
		if (removedFriends == null)
		{
			return;
		}
		BnetPlayer opponent = this.GetOpponent(BnetPresenceMgr.Get().GetMyPlayer());
		if (opponent == null)
		{
			return;
		}
		foreach (BnetPlayer bnetPlayer in removedFriends)
		{
			if (bnetPlayer == opponent)
			{
				this.RevertTavernBrawlStatus();
				this.CleanUpChallengeData(true);
				this.FireChangedEvent(FriendChallengeEvent.OPPONENT_REMOVED_FROM_FRIENDS, opponent);
				break;
			}
		}
	}

	// Token: 0x06002314 RID: 8980 RVA: 0x000ACC28 File Offset: 0x000AAE28
	private void OnNearbyPlayersChanged(BnetNearbyPlayerChangelist changelist, object userData)
	{
		if (!this.HasChallenge())
		{
			return;
		}
		List<BnetPlayer> removedPlayers = changelist.GetRemovedPlayers();
		if (removedPlayers == null)
		{
			return;
		}
		BnetPlayer opponent = this.GetOpponent(BnetPresenceMgr.Get().GetMyPlayer());
		if (opponent == null)
		{
			return;
		}
		foreach (BnetPlayer bnetPlayer in removedPlayers)
		{
			if (bnetPlayer == opponent)
			{
				this.CleanUpChallengeData(true);
				this.FireChangedEvent(FriendChallengeEvent.OPPONENT_CANCELED_CHALLENGE, opponent);
				break;
			}
		}
	}

	// Token: 0x06002315 RID: 8981 RVA: 0x000ACCCC File Offset: 0x000AAECC
	private void OnBnetEventOccurred(BattleNet.BnetEvent bnetEvent, object userData)
	{
		if (bnetEvent == null)
		{
			if (this.m_challengeDialog != null)
			{
				this.m_challengeDialog.Hide();
				this.m_challengeDialog = null;
			}
			this.CleanUpChallengeData(true);
		}
	}

	// Token: 0x06002316 RID: 8982 RVA: 0x000ACD0C File Offset: 0x000AAF0C
	private void OnChallengeChanged(FriendChallengeEvent challengeEvent, BnetPlayer player, object userData)
	{
		switch (challengeEvent)
		{
		case FriendChallengeEvent.I_SENT_CHALLENGE:
			this.ShowISentChallengeDialog(player);
			break;
		case FriendChallengeEvent.OPPONENT_ACCEPTED_CHALLENGE:
			this.StartChallengeProcess();
			break;
		case FriendChallengeEvent.OPPONENT_DECLINED_CHALLENGE:
			this.ShowOpponentDeclinedChallengeDialog(player);
			break;
		case FriendChallengeEvent.I_RECEIVED_CHALLENGE:
			if (this.CanPromptReceivedChallenge())
			{
				if (this.IsChallengeTavernBrawl())
				{
					PresenceMgr.Get().SetStatus(new Enum[]
					{
						PresenceStatus.TAVERN_BRAWL_FRIENDLY_WAITING
					});
				}
				this.ShowIReceivedChallengeDialog(player);
			}
			break;
		case FriendChallengeEvent.I_ACCEPTED_CHALLENGE:
			this.StartChallengeProcess();
			break;
		case FriendChallengeEvent.OPPONENT_RESCINDED_CHALLENGE:
			this.ShowOpponentCanceledChallengeDialog(player);
			break;
		case FriendChallengeEvent.OPPONENT_CANCELED_CHALLENGE:
			this.ShowOpponentCanceledChallengeDialog(player);
			break;
		case FriendChallengeEvent.OPPONENT_REMOVED_FROM_FRIENDS:
			this.ShowOpponentRemovedFromFriendsDialog(player);
			break;
		}
	}

	// Token: 0x06002317 RID: 8983 RVA: 0x000ACDF0 File Offset: 0x000AAFF0
	private bool CanPromptReceivedChallenge()
	{
		if (!UserAttentionManager.CanShowAttentionGrabber("FriendlyChallengeMgr.CanPromptReceivedChallenge"))
		{
			BnetParty.SetPartyAttributeLong(this.m_partyId, "WTCG.Friendly.DeclineReason", 6L);
			this.DeclineChallenge();
			return false;
		}
		if (this.IsChallengeTavernBrawl())
		{
			if (!TavernBrawlManager.Get().HasUnlockedTavernBrawl)
			{
				FriendChallengeMgr.DeclineReason declineReason = FriendChallengeMgr.DeclineReason.TavernBrawlNotUnlocked;
				BnetParty.SetPartyAttributeLong(this.m_partyId, "WTCG.Friendly.DeclineReason", (long)declineReason);
				this.DeclineChallenge();
				return false;
			}
			TavernBrawlManager.Get().EnsureScenarioDataReady(new TavernBrawlManager.CallbackEnsureServerDataReady(this.TavernBrawl_ReceivedChallenge_OnEnsureServerDataReady));
			return false;
		}
		else
		{
			if (!CollectionManager.Get().AreAllDeckContentsReady())
			{
				CollectionManager.Get().RequestDeckContentsForDecksWithoutContentsLoaded(new CollectionManager.DelOnAllDeckContents(this.CanPromptReceivedChallenge_OnDeckContentsLoaded));
				return false;
			}
			if (this.IsChallengeStandardDuel() && !CollectionManager.Get().AccountHasValidStandardDeck())
			{
				FriendChallengeMgr.DeclineReason declineReason2 = FriendChallengeMgr.DeclineReason.StandardNoValidDeck;
				BnetParty.SetPartyAttributeLong(this.m_partyId, "WTCG.Friendly.DeclineReason", (long)declineReason2);
				this.DeclineChallenge();
				return false;
			}
			if (this.IsChallengeWildDuel())
			{
				if (!CollectionManager.Get().ShouldAccountSeeStandardWild())
				{
					FriendChallengeMgr.DeclineReason declineReason3 = FriendChallengeMgr.DeclineReason.NotSeenWild;
					BnetParty.SetPartyAttributeLong(this.m_partyId, "WTCG.Friendly.DeclineReason", (long)declineReason3);
					this.DeclineChallenge();
					return false;
				}
				if (!CollectionManager.Get().AccountHasAnyValidDeck())
				{
					FriendChallengeMgr.DeclineReason declineReason4 = FriendChallengeMgr.DeclineReason.NoValidDeck;
					BnetParty.SetPartyAttributeLong(this.m_partyId, "WTCG.Friendly.DeclineReason", (long)declineReason4);
					this.DeclineChallenge();
					return false;
				}
			}
			return true;
		}
	}

	// Token: 0x06002318 RID: 8984 RVA: 0x000ACF4D File Offset: 0x000AB14D
	private void CanPromptReceivedChallenge_OnDeckContentsLoaded()
	{
		if (!this.DidReceiveChallenge())
		{
			return;
		}
		if (this.CanPromptReceivedChallenge())
		{
			this.ShowIReceivedChallengeDialog(this.m_challenger);
		}
	}

	// Token: 0x06002319 RID: 8985 RVA: 0x000ACF74 File Offset: 0x000AB174
	private void TavernBrawl_ReceivedChallenge_OnEnsureServerDataReady()
	{
		TavernBrawlMission tavernBrawlMission = TavernBrawlManager.Get().CurrentMission();
		FriendChallengeMgr.DeclineReason? declineReason = default(FriendChallengeMgr.DeclineReason?);
		if (tavernBrawlMission == null)
		{
			declineReason = new FriendChallengeMgr.DeclineReason?(FriendChallengeMgr.DeclineReason.None);
		}
		if (tavernBrawlMission != null && tavernBrawlMission.canCreateDeck && !TavernBrawlManager.Get().HasValidDeck())
		{
			declineReason = new FriendChallengeMgr.DeclineReason?(FriendChallengeMgr.DeclineReason.TavernBrawlNoValidDeck);
		}
		if (declineReason != null)
		{
			BnetParty.SetPartyAttributeLong(this.m_partyId, "WTCG.Friendly.DeclineReason", (long)declineReason.Value);
			this.DeclineChallenge();
		}
		else
		{
			if (this.IsChallengeTavernBrawl())
			{
				PresenceMgr.Get().SetStatus(new Enum[]
				{
					PresenceStatus.TAVERN_BRAWL_FRIENDLY_WAITING
				});
			}
			this.ShowIReceivedChallengeDialog(this.m_challenger);
		}
	}

	// Token: 0x0600231A RID: 8986 RVA: 0x000AD030 File Offset: 0x000AB230
	private bool RevertTavernBrawlStatus()
	{
		Enum[] status = PresenceMgr.Get().GetStatus();
		if (this.IsChallengeTavernBrawl() && status != null && status.Length > 0 && (PresenceStatus)status[0] == PresenceStatus.TAVERN_BRAWL_FRIENDLY_WAITING)
		{
			PresenceMgr.Get().SetPrevStatus();
			return true;
		}
		return false;
	}

	// Token: 0x0600231B RID: 8987 RVA: 0x000AD080 File Offset: 0x000AB280
	private bool OnFindGameEvent(FindGameEventData eventData, object userData)
	{
		this.UpdateMyAvailability();
		FindGameState state = eventData.m_state;
		if (state == FindGameState.BNET_QUEUE_CANCELED || state == FindGameState.BNET_ERROR)
		{
			if (this.DidSendChallenge())
			{
				BnetParty.SetPartyAttributeLong(this.m_partyId, "error", (long)((ulong)GameMgr.Get().GetLastEnterGameError()));
				BnetParty.SetPartyAttributeString(this.m_partyId, "s1", "deck");
			}
			else if (this.DidReceiveChallenge())
			{
				BnetParty.SetPartyAttributeString(this.m_partyId, "s2", "deck");
			}
			SceneMgr.Mode mode = SceneMgr.Get().GetMode();
			if (mode != SceneMgr.Mode.FRIENDLY && mode != SceneMgr.Mode.TAVERN_BRAWL)
			{
				this.CancelChallenge();
			}
		}
		return false;
	}

	// Token: 0x0600231C RID: 8988 RVA: 0x000AD144 File Offset: 0x000AB344
	private void WillReset()
	{
		this.CleanUpChallengeData(false);
		if (this.m_challengeDialog != null)
		{
			this.m_challengeDialog.Hide();
			this.m_challengeDialog = null;
		}
	}

	// Token: 0x0600231D RID: 8989 RVA: 0x000AD17C File Offset: 0x000AB37C
	private void ShowISentChallengeDialog(BnetPlayer challengee)
	{
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER");
		popupInfo.m_text = GameStrings.Format("GLOBAL_FRIEND_CHALLENGE_OPPONENT_WAITING_RESPONSE", new object[]
		{
			FriendUtils.GetUniqueName(challengee)
		});
		popupInfo.m_showAlertIcon = false;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.NONE;
		popupInfo.m_responseCallback = new AlertPopup.ResponseCallback(this.OnChallengeSentDialogResponse);
		popupInfo.m_layerToUse = new GameLayer?(GameLayer.HighPriorityUI);
		DialogManager.Get().ShowPopup(popupInfo, new DialogManager.DialogProcessCallback(this.OnChallengeSentDialogProcessed));
	}

	// Token: 0x0600231E RID: 8990 RVA: 0x000AD204 File Offset: 0x000AB404
	private void ShowOpponentDeclinedChallengeDialog(BnetPlayer challengee)
	{
		if (this.m_challengeDialog != null)
		{
			this.m_challengeDialog.Hide();
			this.m_challengeDialog = null;
		}
		if (!this.m_hasSeenDeclinedReason)
		{
			AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
			popupInfo.m_headerText = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER");
			popupInfo.m_text = GameStrings.Format("GLOBAL_FRIEND_CHALLENGE_OPPONENT_DECLINED", new object[]
			{
				FriendUtils.GetUniqueName(challengee)
			});
			popupInfo.m_showAlertIcon = false;
			popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
			DialogManager.Get().ShowPopup(popupInfo);
		}
	}

	// Token: 0x0600231F RID: 8991 RVA: 0x000AD290 File Offset: 0x000AB490
	private void ShowIReceivedChallengeDialog(BnetPlayer challenger)
	{
		if (this.m_challengeDialog != null)
		{
			this.m_challengeDialog.Hide();
			this.m_challengeDialog = null;
		}
		DialogManager.Get().ShowFriendlyChallenge(this.m_challengeFormatType, challenger, this.IsChallengeTavernBrawl(), new FriendlyChallengeDialog.ResponseCallback(this.OnChallengeReceivedDialogResponse), new DialogManager.DialogProcessCallback(this.OnChallengeReceivedDialogProcessed));
	}

	// Token: 0x06002320 RID: 8992 RVA: 0x000AD2F0 File Offset: 0x000AB4F0
	private void ShowOpponentCanceledChallengeDialog(BnetPlayer otherPlayer)
	{
		if (this.m_challengeDialog != null)
		{
			this.m_challengeDialog.Hide();
			this.m_challengeDialog = null;
		}
		if (SceneMgr.Get() != null && SceneMgr.Get().IsInGame() && GameState.Get() != null && !GameState.Get().IsGameOverNowOrPending() && GameState.Get().GetOpposingSidePlayer() != null && GameState.Get().GetOpposingSidePlayer().GetBnetPlayer() != null && otherPlayer != null && otherPlayer.GetHearthstoneGameAccountId() == GameState.Get().GetOpposingSidePlayer().GetBnetPlayer().GetHearthstoneGameAccountId())
		{
			return;
		}
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER");
		popupInfo.m_text = GameStrings.Format("GLOBAL_FRIEND_CHALLENGE_OPPONENT_CANCELED", new object[]
		{
			FriendUtils.GetUniqueName(otherPlayer)
		});
		popupInfo.m_showAlertIcon = false;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
		DialogManager.Get().ShowPopup(popupInfo);
	}

	// Token: 0x06002321 RID: 8993 RVA: 0x000AD3F4 File Offset: 0x000AB5F4
	private void ShowOpponentRemovedFromFriendsDialog(BnetPlayer otherPlayer)
	{
		if (this.m_challengeDialog != null)
		{
			this.m_challengeDialog.Hide();
			this.m_challengeDialog = null;
		}
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER");
		popupInfo.m_text = GameStrings.Format("GLOBAL_FRIEND_CHALLENGE_OPPONENT_FRIEND_REMOVED", new object[]
		{
			FriendUtils.GetUniqueName(otherPlayer)
		});
		popupInfo.m_showAlertIcon = false;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
		DialogManager.Get().ShowPopup(popupInfo);
	}

	// Token: 0x06002322 RID: 8994 RVA: 0x000AD472 File Offset: 0x000AB672
	private bool OnChallengeSentDialogProcessed(DialogBase dialog, object userData)
	{
		if (!this.DidSendChallenge())
		{
			return false;
		}
		this.m_challengeDialog = dialog;
		this.UpdateChallengeSentDialog();
		return true;
	}

	// Token: 0x06002323 RID: 8995 RVA: 0x000AD490 File Offset: 0x000AB690
	private void UpdateChallengeSentDialog()
	{
		if (this.m_partyId == null)
		{
			return;
		}
		if (this.m_challengeDialog == null)
		{
			return;
		}
		AlertPopup alertPopup = (AlertPopup)this.m_challengeDialog;
		AlertPopup.PopupInfo info = alertPopup.GetInfo();
		if (info == null)
		{
			return;
		}
		info.m_responseDisplay = AlertPopup.ResponseDisplay.CANCEL;
		alertPopup.UpdateInfo(info);
	}

	// Token: 0x06002324 RID: 8996 RVA: 0x000AD4E9 File Offset: 0x000AB6E9
	private void OnChallengeSentDialogResponse(AlertPopup.Response response, object userData)
	{
		this.m_challengeDialog = null;
		this.RescindChallenge();
	}

	// Token: 0x06002325 RID: 8997 RVA: 0x000AD4F8 File Offset: 0x000AB6F8
	private bool OnChallengeReceivedDialogProcessed(DialogBase dialog, object userData)
	{
		if (!this.DidReceiveChallenge())
		{
			return false;
		}
		this.m_challengeDialog = dialog;
		return true;
	}

	// Token: 0x06002326 RID: 8998 RVA: 0x000AD50F File Offset: 0x000AB70F
	private void OnChallengeReceivedDialogResponse(bool accept)
	{
		this.m_challengeDialog = null;
		if (accept)
		{
			this.AcceptChallenge();
		}
		else
		{
			this.DeclineChallenge();
		}
	}

	// Token: 0x06002327 RID: 8999 RVA: 0x000AD530 File Offset: 0x000AB730
	private void HandleJoinedParty(BnetEntityId partyId, BnetGameAccountId otherMemberId)
	{
		this.m_partyId = partyId;
		this.m_challengeePartyId = partyId;
		this.m_challengerId = otherMemberId;
		this.m_challenger = BnetUtils.GetPlayer(this.m_challengerId);
		this.m_challengee = BnetPresenceMgr.Get().GetMyPlayer();
		this.m_hasSeenDeclinedReason = false;
		if (this.m_challenger == null || !this.m_challenger.IsDisplayable())
		{
			this.m_challengerPending = true;
			this.UpdateMyAvailability();
		}
		else
		{
			this.UpdateMyAvailability();
			this.FireChangedEvent(FriendChallengeEvent.I_RECEIVED_CHALLENGE, this.m_challenger);
		}
	}

	// Token: 0x06002328 RID: 9000 RVA: 0x000AD5BC File Offset: 0x000AB7BC
	private void ResolveChallengeConflict()
	{
		if (this.m_partyId.GetLo() < this.m_challengeePartyId.GetLo())
		{
			Network.DeclineFriendChallenge(this.m_challengeePartyId);
			this.m_challengeePartyId = this.m_partyId;
		}
		else
		{
			if (this.m_challengee == null)
			{
				return;
			}
			this.HandleJoinedParty(this.m_challengeePartyId, this.m_challengee.GetHearthstoneGameAccount().GetId());
		}
	}

	// Token: 0x06002329 RID: 9001 RVA: 0x000AD628 File Offset: 0x000AB828
	public void UpdateMyAvailability()
	{
		bool flag = !this.HasAvailabilityBlocker();
		BnetPresenceMgr.Get().SetGameField(1U, flag);
		BnetNearbyPlayerMgr.Get().SetAvailability(flag);
	}

	// Token: 0x0600232A RID: 9002 RVA: 0x000AD658 File Offset: 0x000AB858
	private bool HasAvailabilityBlocker()
	{
		return this.GetAvailabilityBlockerReason() != FriendChallengeMgr.FriendListAvailabilityBlockerReasons.NONE;
	}

	// Token: 0x0600232B RID: 9003 RVA: 0x000AD678 File Offset: 0x000AB878
	private FriendChallengeMgr.FriendListAvailabilityBlockerReasons GetAvailabilityBlockerReason()
	{
		if (!this.m_netCacheReady)
		{
			return FriendChallengeMgr.FriendListAvailabilityBlockerReasons.NETCACHE_NOT_READY;
		}
		if (!this.m_myPlayerReady)
		{
			return FriendChallengeMgr.FriendListAvailabilityBlockerReasons.MY_PLAYER_NOT_READY;
		}
		if (this.HasChallenge())
		{
			return FriendChallengeMgr.FriendListAvailabilityBlockerReasons.HAS_EXISTING_CHALLENGE;
		}
		if (SpectatorManager.Get().IsInSpectatorMode())
		{
			return FriendChallengeMgr.FriendListAvailabilityBlockerReasons.SPECTATING_GAME;
		}
		if (GameMgr.Get().IsFindingGame())
		{
			return FriendChallengeMgr.FriendListAvailabilityBlockerReasons.FINDING_GAME;
		}
		if (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.FATAL_ERROR))
		{
			return FriendChallengeMgr.FriendListAvailabilityBlockerReasons.HAS_FATAL_ERROR;
		}
		if (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.LOGIN))
		{
			return FriendChallengeMgr.FriendListAvailabilityBlockerReasons.LOGGING_IN;
		}
		if (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.STARTUP))
		{
			return FriendChallengeMgr.FriendListAvailabilityBlockerReasons.STARTING_UP;
		}
		if (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.GAMEPLAY))
		{
			if (GameMgr.Get().IsSpectator() || GameMgr.Get().IsNextSpectator())
			{
				return FriendChallengeMgr.FriendListAvailabilityBlockerReasons.SPECTATING_GAME;
			}
			if (GameMgr.Get().IsAI() || GameMgr.Get().IsNextAI())
			{
				return FriendChallengeMgr.FriendListAvailabilityBlockerReasons.PLAYING_AI_GAME;
			}
			return FriendChallengeMgr.FriendListAvailabilityBlockerReasons.PLAYING_NON_AI_GAME;
		}
		else
		{
			if (!GameUtils.AreAllTutorialsComplete())
			{
				return FriendChallengeMgr.FriendListAvailabilityBlockerReasons.TUTORIALS_INCOMPLETE;
			}
			if (ShownUIMgr.Get().GetShownUI() == ShownUIMgr.UI_WINDOW.GENERAL_STORE)
			{
				return FriendChallengeMgr.FriendListAvailabilityBlockerReasons.STORE_IS_SHOWN;
			}
			if (TavernBrawlDisplay.Get() != null && TavernBrawlDisplay.Get().IsInDeckEditMode())
			{
				return FriendChallengeMgr.FriendListAvailabilityBlockerReasons.EDITING_TAVERN_BRAWL;
			}
			return FriendChallengeMgr.FriendListAvailabilityBlockerReasons.NONE;
		}
	}

	// Token: 0x0600232C RID: 9004 RVA: 0x000AD7A0 File Offset: 0x000AB9A0
	private void FireChangedEvent(FriendChallengeEvent challengeEvent, BnetPlayer player)
	{
		foreach (FriendChallengeMgr.ChangedListener changedListener in this.m_changedListeners.ToArray())
		{
			changedListener.Fire(challengeEvent, player);
		}
	}

	// Token: 0x0600232D RID: 9005 RVA: 0x000AD7DC File Offset: 0x000AB9DC
	private void CleanUpChallengeData(bool updateAvailability = true)
	{
		this.m_partyId = null;
		this.m_challengeePartyId = null;
		this.m_challengerId = null;
		this.m_challengerPending = false;
		this.m_challenger = null;
		this.m_challengerDeckSelected = false;
		this.m_challengee = null;
		this.m_challengeeAccepted = false;
		this.m_challengeeDeckSelected = false;
		this.m_scenarioId = 2;
		this.m_isChallengeTavernBrawl = false;
		this.m_challengeFormatType = 0;
		if (updateAvailability)
		{
			this.UpdateMyAvailability();
		}
	}

	// Token: 0x0600232E RID: 9006 RVA: 0x000AD84C File Offset: 0x000ABA4C
	private void StartChallengeProcess()
	{
		if (this.m_challengeDialog != null)
		{
			this.m_challengeDialog.Hide();
			this.m_challengeDialog = null;
		}
		GameMgr.Get().SetPendingAutoConcede(true);
		if (this.IsChallengeTavernBrawl() && !TavernBrawlManager.Get().SelectHeroBeforeMission())
		{
			if (TavernBrawlManager.Get().CurrentMission().canCreateDeck)
			{
				if (!TavernBrawlManager.Get().HasValidDeck())
				{
					Debug.LogError("Attempting to start a Tavern Brawl challenge without a valid deck!  How did this happen?");
					return;
				}
				this.SelectDeck(TavernBrawlManager.Get().CurrentDeck().ID);
			}
			else
			{
				this.SkipDeckSelection();
			}
			FriendlyChallengeHelper.Get().WaitForFriendChallengeToStart();
		}
		else
		{
			if (!this.m_isChallengeTavernBrawl)
			{
				Options.Get().SetBool(Option.IN_WILD_MODE, this.m_challengeFormatType == 1);
			}
			Navigation.Clear();
			SceneMgr.Get().SetNextMode(SceneMgr.Mode.FRIENDLY);
		}
	}

	// Token: 0x0400143A RID: 5178
	private const int DEFAULT_SCENARIO_ID = 2;

	// Token: 0x0400143B RID: 5179
	private const float SPECTATE_GAME_SERVER_SETUP_TIMEOUT_SECONDS = 15f;

	// Token: 0x0400143C RID: 5180
	public const string ATTRIBUTE_STATE_PLAYER1 = "s1";

	// Token: 0x0400143D RID: 5181
	public const string ATTRIBUTE_STATE_PLAYER2 = "s2";

	// Token: 0x0400143E RID: 5182
	public const string ATTRIBUTE_DECK_PLAYER1 = "d1";

	// Token: 0x0400143F RID: 5183
	public const string ATTRIBUTE_DECK_PLAYER2 = "d2";

	// Token: 0x04001440 RID: 5184
	public const string ATTRIBUTE_PARM = "parm";

	// Token: 0x04001441 RID: 5185
	public const string ATTRIBUTE_DLL = "dll";

	// Token: 0x04001442 RID: 5186
	public const string ATTRIBUTE_LEFT = "left";

	// Token: 0x04001443 RID: 5187
	public const string ATTRIBUTE_CB = "cb";

	// Token: 0x04001444 RID: 5188
	public const string ATTRIBUTE_ERROR = "error";

	// Token: 0x04001445 RID: 5189
	public const string ATTRIBUTE_VALUE_STATE_WAIT = "wait";

	// Token: 0x04001446 RID: 5190
	public const string ATTRIBUTE_VALUE_STATE_DECK = "deck";

	// Token: 0x04001447 RID: 5191
	public const string ATTRIBUTE_VALUE_STATE_GAME = "game";

	// Token: 0x04001448 RID: 5192
	public const string ATTRIBUTE_VALUE_STATE_GOTO = "goto";

	// Token: 0x04001449 RID: 5193
	private static FriendChallengeMgr s_instance;

	// Token: 0x0400144A RID: 5194
	private bool m_netCacheReady;

	// Token: 0x0400144B RID: 5195
	private bool m_myPlayerReady;

	// Token: 0x0400144C RID: 5196
	private BnetEntityId m_partyId;

	// Token: 0x0400144D RID: 5197
	private BnetEntityId m_challengeePartyId;

	// Token: 0x0400144E RID: 5198
	private BnetGameAccountId m_challengerId;

	// Token: 0x0400144F RID: 5199
	private bool m_challengerPending;

	// Token: 0x04001450 RID: 5200
	private int m_scenarioId = 2;

	// Token: 0x04001451 RID: 5201
	private bool m_isChallengeTavernBrawl;

	// Token: 0x04001452 RID: 5202
	private FormatType m_challengeFormatType;

	// Token: 0x04001453 RID: 5203
	private BnetPlayer m_challenger;

	// Token: 0x04001454 RID: 5204
	private bool m_challengerDeckSelected;

	// Token: 0x04001455 RID: 5205
	private BnetPlayer m_challengee;

	// Token: 0x04001456 RID: 5206
	private bool m_challengeeAccepted;

	// Token: 0x04001457 RID: 5207
	private bool m_challengeeDeckSelected;

	// Token: 0x04001458 RID: 5208
	private List<FriendChallengeMgr.ChangedListener> m_changedListeners = new List<FriendChallengeMgr.ChangedListener>();

	// Token: 0x04001459 RID: 5209
	private DialogBase m_challengeDialog;

	// Token: 0x0400145A RID: 5210
	private bool m_hasSeenDeclinedReason;

	// Token: 0x0200038E RID: 910
	// (Invoke) Token: 0x06002FA6 RID: 12198
	public delegate void ChangedCallback(FriendChallengeEvent challengeEvent, BnetPlayer player, object userData);

	// Token: 0x020005C8 RID: 1480
	private enum FriendListAvailabilityBlockerReasons
	{
		// Token: 0x04002A20 RID: 10784
		NONE,
		// Token: 0x04002A21 RID: 10785
		NETCACHE_NOT_READY,
		// Token: 0x04002A22 RID: 10786
		MY_PLAYER_NOT_READY,
		// Token: 0x04002A23 RID: 10787
		HAS_EXISTING_CHALLENGE,
		// Token: 0x04002A24 RID: 10788
		FINDING_GAME,
		// Token: 0x04002A25 RID: 10789
		HAS_FATAL_ERROR,
		// Token: 0x04002A26 RID: 10790
		LOGGING_IN,
		// Token: 0x04002A27 RID: 10791
		STARTING_UP,
		// Token: 0x04002A28 RID: 10792
		PLAYING_NON_AI_GAME,
		// Token: 0x04002A29 RID: 10793
		TUTORIALS_INCOMPLETE,
		// Token: 0x04002A2A RID: 10794
		STORE_IS_SHOWN,
		// Token: 0x04002A2B RID: 10795
		PLAYING_AI_GAME,
		// Token: 0x04002A2C RID: 10796
		SPECTATING_GAME,
		// Token: 0x04002A2D RID: 10797
		EDITING_TAVERN_BRAWL
	}

	// Token: 0x020005C9 RID: 1481
	public enum DeclineReason
	{
		// Token: 0x04002A2F RID: 10799
		None,
		// Token: 0x04002A30 RID: 10800
		UserDeclined,
		// Token: 0x04002A31 RID: 10801
		NoValidDeck,
		// Token: 0x04002A32 RID: 10802
		StandardNoValidDeck,
		// Token: 0x04002A33 RID: 10803
		TavernBrawlNoValidDeck,
		// Token: 0x04002A34 RID: 10804
		TavernBrawlNotUnlocked,
		// Token: 0x04002A35 RID: 10805
		UserIsBusy,
		// Token: 0x04002A36 RID: 10806
		NotSeenWild
	}

	// Token: 0x020005CA RID: 1482
	private class ChangedListener : EventListener<FriendChallengeMgr.ChangedCallback>
	{
		// Token: 0x0600424D RID: 16973 RVA: 0x00140205 File Offset: 0x0013E405
		public void Fire(FriendChallengeEvent challengeEvent, BnetPlayer player)
		{
			this.m_callback(challengeEvent, player, this.m_userData);
		}
	}
}
