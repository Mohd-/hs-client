using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using bgs;
using bgs.types;
using bnet.protocol.attribute;
using PegasusGame;
using PegasusShared;
using SpectatorProto;
using UnityEngine;

// Token: 0x02000322 RID: 802
public class SpectatorManager
{
	// Token: 0x06002954 RID: 10580 RVA: 0x000C85D8 File Offset: 0x000C67D8
	private SpectatorManager()
	{
	}

	// Token: 0x1400000C RID: 12
	// (add) Token: 0x06002956 RID: 10582 RVA: 0x000C868D File Offset: 0x000C688D
	// (remove) Token: 0x06002957 RID: 10583 RVA: 0x000C86A6 File Offset: 0x000C68A6
	public event SpectatorManager.InviteReceivedHandler OnInviteReceived;

	// Token: 0x1400000D RID: 13
	// (add) Token: 0x06002958 RID: 10584 RVA: 0x000C86BF File Offset: 0x000C68BF
	// (remove) Token: 0x06002959 RID: 10585 RVA: 0x000C86D8 File Offset: 0x000C68D8
	public event SpectatorManager.InviteSentHandler OnInviteSent;

	// Token: 0x1400000E RID: 14
	// (add) Token: 0x0600295A RID: 10586 RVA: 0x000C86F1 File Offset: 0x000C68F1
	// (remove) Token: 0x0600295B RID: 10587 RVA: 0x000C870A File Offset: 0x000C690A
	public event SpectatorManager.SpectatorToMyGameHandler OnSpectatorToMyGame;

	// Token: 0x1400000F RID: 15
	// (add) Token: 0x0600295C RID: 10588 RVA: 0x000C8723 File Offset: 0x000C6923
	// (remove) Token: 0x0600295D RID: 10589 RVA: 0x000C873C File Offset: 0x000C693C
	public event SpectatorManager.SpectatorModeChangedHandler OnSpectatorModeChanged;

	// Token: 0x0600295E RID: 10590 RVA: 0x000C8755 File Offset: 0x000C6955
	public static SpectatorManager Get()
	{
		if (SpectatorManager.s_instance == null)
		{
			SpectatorManager.CreateInstance();
		}
		return SpectatorManager.s_instance;
	}

	// Token: 0x0600295F RID: 10591 RVA: 0x000C876C File Offset: 0x000C696C
	public static JoinInfo GetSpectatorJoinInfo(BnetGameAccount gameAccount)
	{
		byte[] gameFieldBytes = gameAccount.GetGameFieldBytes(21U);
		if (gameFieldBytes != null && gameFieldBytes.Length > 0)
		{
			return ProtobufUtil.ParseFrom<JoinInfo>(gameFieldBytes, 0, -1);
		}
		return null;
	}

	// Token: 0x06002960 RID: 10592 RVA: 0x000C87A0 File Offset: 0x000C69A0
	public static bool IsSpectatorSlotAvailable(JoinInfo info)
	{
		if (info == null)
		{
			return false;
		}
		if (!info.HasPartyId)
		{
			if (!info.HasServerIpAddress || !info.HasSecretKey)
			{
				return false;
			}
			if (string.IsNullOrEmpty(info.SecretKey))
			{
				return false;
			}
		}
		return (!info.HasIsJoinable || info.IsJoinable) && (!info.HasMaxNumSpectators || !info.HasCurrentNumSpectators || info.CurrentNumSpectators < info.MaxNumSpectators);
	}

	// Token: 0x06002961 RID: 10593 RVA: 0x000C882C File Offset: 0x000C6A2C
	public static bool IsSpectatorSlotAvailable(BnetGameAccount gameAccount)
	{
		JoinInfo spectatorJoinInfo = SpectatorManager.GetSpectatorJoinInfo(gameAccount);
		return SpectatorManager.IsSpectatorSlotAvailable(spectatorJoinInfo);
	}

	// Token: 0x06002962 RID: 10594 RVA: 0x000C8848 File Offset: 0x000C6A48
	public void InitializeConnectedToBnet()
	{
		if (this.m_initialized)
		{
			return;
		}
		this.m_initialized = true;
		PartyInfo[] joinedParties = BnetParty.GetJoinedParties();
		foreach (PartyInfo party in joinedParties)
		{
			this.BnetParty_OnJoined(0, party, default(LeaveReason?));
		}
		PartyInvite[] receivedInvites = BnetParty.GetReceivedInvites();
		foreach (PartyInvite partyInvite in receivedInvites)
		{
			this.BnetParty_OnReceivedInvite(0, new PartyInfo(partyInvite.PartyId, partyInvite.PartyType), partyInvite.InviteId, default(InviteRemoveReason?));
		}
	}

	// Token: 0x06002963 RID: 10595 RVA: 0x000C88F0 File Offset: 0x000C6AF0
	public bool CanSpectate(BnetPlayer player)
	{
		BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
		if (player == myPlayer)
		{
			return false;
		}
		BnetGameAccountId hearthstoneGameAccountId = player.GetHearthstoneGameAccountId();
		if (this.IsSpectatingPlayer(hearthstoneGameAccountId))
		{
			return false;
		}
		if (this.m_spectateeOpposingSide != null)
		{
			return false;
		}
		if (this.HasPreviouslyKickedMeFromGame(hearthstoneGameAccountId) && !this.HasInvitedMeToSpectate(hearthstoneGameAccountId))
		{
			return false;
		}
		if (GameMgr.Get().IsFindingGame())
		{
			return false;
		}
		if (GameMgr.Get().IsNextSpectator())
		{
			return false;
		}
		if (!SpectatorManager.IsSpectatorSlotAvailable(player.GetBestGameAccount()) && !this.HasInvitedMeToSpectate(hearthstoneGameAccountId))
		{
			return false;
		}
		if (GameMgr.Get().IsSpectator())
		{
			if (!this.IsPlayerInGame(hearthstoneGameAccountId))
			{
				return false;
			}
		}
		else if (SceneMgr.Get().IsInGame())
		{
			return false;
		}
		if (!GameUtils.AreAllTutorialsComplete())
		{
			return false;
		}
		if (ApplicationMgr.IsPublic())
		{
			BnetGameAccount hearthstoneGameAccount = player.GetHearthstoneGameAccount();
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
		return SceneMgr.Get().GetMode() != SceneMgr.Mode.LOGIN;
	}

	// Token: 0x06002964 RID: 10596 RVA: 0x000C8A30 File Offset: 0x000C6C30
	public bool IsInSpectatorMode()
	{
		if (GameMgr.Get() != null && GameMgr.Get().IsSpectator())
		{
			return true;
		}
		if (this.m_spectatorPartyIdMain == null)
		{
			return false;
		}
		if (!BnetParty.IsInParty(this.m_spectatorPartyIdMain))
		{
			return false;
		}
		if (!this.m_initialized)
		{
			return false;
		}
		BnetGameAccountId partyCreator = this.GetPartyCreator(this.m_spectatorPartyIdMain);
		return !(partyCreator == null) && !this.ShouldBePartyLeader(this.m_spectatorPartyIdMain);
	}

	// Token: 0x06002965 RID: 10597 RVA: 0x000C8ABC File Offset: 0x000C6CBC
	public bool ShouldBeSpectatingInGame()
	{
		return !(this.m_spectatorPartyIdMain == null) && BnetParty.GetPartyAttributeBlob(this.m_spectatorPartyIdMain, "WTCG.Party.ServerInfo") != null;
	}

	// Token: 0x06002966 RID: 10598 RVA: 0x000C8AF8 File Offset: 0x000C6CF8
	public bool IsSpectatingPlayer(BnetGameAccountId gameAccountId)
	{
		return (this.m_spectateeFriendlySide != null && this.m_spectateeFriendlySide == gameAccountId) || (this.m_spectateeOpposingSide != null && this.m_spectateeOpposingSide == gameAccountId);
	}

	// Token: 0x06002967 RID: 10599 RVA: 0x000C8B50 File Offset: 0x000C6D50
	public bool IsSpectatingMe(BnetGameAccountId gameAccountId)
	{
		return !this.IsInSpectatorMode() && (this.m_gameServerKnownSpectators.Contains(gameAccountId) || (gameAccountId != BnetPresenceMgr.Get().GetMyGameAccountId() && BnetParty.IsMember(this.m_spectatorPartyIdMain, gameAccountId)));
	}

	// Token: 0x06002968 RID: 10600 RVA: 0x000C8BA8 File Offset: 0x000C6DA8
	public int GetCountSpectatingMe()
	{
		if (this.m_spectatorPartyIdMain != null && !this.ShouldBePartyLeader(this.m_spectatorPartyIdMain))
		{
			return 0;
		}
		int count = this.m_gameServerKnownSpectators.Count;
		int num = BnetParty.CountMembers(this.m_spectatorPartyIdMain);
		return Mathf.Max(num - 1, count);
	}

	// Token: 0x06002969 RID: 10601 RVA: 0x000C8BFA File Offset: 0x000C6DFA
	public bool IsBeingSpectated()
	{
		return this.GetCountSpectatingMe() > 0;
	}

	// Token: 0x0600296A RID: 10602 RVA: 0x000C8C08 File Offset: 0x000C6E08
	public BnetGameAccountId[] GetSpectatorPartyMembers(bool friendlySide = true, bool includeSelf = false)
	{
		List<BnetGameAccountId> list = new List<BnetGameAccountId>(this.m_gameServerKnownSpectators);
		PartyMember[] members = BnetParty.GetMembers((!friendlySide) ? this.m_spectatorPartyIdOpposingSide : this.m_spectatorPartyIdMain);
		BnetGameAccountId myGameAccountId = BnetPresenceMgr.Get().GetMyGameAccountId();
		foreach (PartyMember partyMember in members)
		{
			if ((includeSelf || partyMember.GameAccountId != myGameAccountId) && !list.Contains(partyMember.GameAccountId))
			{
				list.Add(partyMember.GameAccountId);
			}
		}
		return list.ToArray();
	}

	// Token: 0x0600296B RID: 10603 RVA: 0x000C8CA8 File Offset: 0x000C6EA8
	public bool IsInSpectatableGame()
	{
		return SceneMgr.Get().IsInGame() && !GameMgr.Get().IsSpectator() && !SpectatorManager.IsGameOver;
	}

	// Token: 0x0600296C RID: 10604 RVA: 0x000C8CE4 File Offset: 0x000C6EE4
	public bool CanAddSpectators()
	{
		if (GameMgr.Get() != null && GameMgr.Get().IsSpectator())
		{
			return false;
		}
		if (this.m_spectateeFriendlySide != null || this.m_spectateeOpposingSide != null)
		{
			return false;
		}
		int countSpectatingMe = this.GetCountSpectatingMe();
		if (!this.IsInSpectatableGame())
		{
			if (this.m_spectatorPartyIdMain == null)
			{
				return false;
			}
			if (countSpectatingMe <= 0)
			{
				return false;
			}
		}
		return countSpectatingMe < 10;
	}

	// Token: 0x0600296D RID: 10605 RVA: 0x000C8D6C File Offset: 0x000C6F6C
	public bool CanInviteToSpectateMyGame(BnetGameAccountId gameAccountId)
	{
		if (!this.CanAddSpectators())
		{
			return false;
		}
		BnetGameAccountId myGameAccountId = BnetPresenceMgr.Get().GetMyGameAccountId();
		if (gameAccountId == myGameAccountId)
		{
			return false;
		}
		if (this.IsSpectatingMe(gameAccountId))
		{
			return false;
		}
		if (this.IsInvitedToSpectateMyGame(gameAccountId))
		{
			return false;
		}
		BnetGameAccount gameAccount = BnetPresenceMgr.Get().GetGameAccount(gameAccountId);
		if (gameAccount == null || !gameAccount.IsOnline())
		{
			return false;
		}
		if (!gameAccount.CanBeInvitedToGame() && !this.IsPlayerSpectatingMyGamesOpposingSide(gameAccountId))
		{
			return false;
		}
		if (ApplicationMgr.IsPublic())
		{
			BnetGameAccount hearthstoneGameAccount = BnetPresenceMgr.Get().GetMyPlayer().GetHearthstoneGameAccount();
			if (string.Compare(gameAccount.GetClientVersion(), hearthstoneGameAccount.GetClientVersion()) != 0)
			{
				return false;
			}
			if (string.Compare(gameAccount.GetClientEnv(), hearthstoneGameAccount.GetClientEnv()) != 0)
			{
				return false;
			}
		}
		return SceneMgr.Get().IsInGame();
	}

	// Token: 0x0600296E RID: 10606 RVA: 0x000C8E58 File Offset: 0x000C7058
	public bool IsPlayerSpectatingMyGamesOpposingSide(BnetGameAccountId gameAccountId)
	{
		BnetGameAccount gameAccount = BnetPresenceMgr.Get().GetGameAccount(gameAccountId);
		if (gameAccount == null)
		{
			return false;
		}
		BnetGameAccountId myGameAccountId = BnetPresenceMgr.Get().GetMyGameAccountId();
		bool result = false;
		if (BnetFriendMgr.Get() != null && BnetFriendMgr.Get().IsFriend(gameAccountId))
		{
			JoinInfo spectatorJoinInfo = SpectatorManager.GetSpectatorJoinInfo(gameAccount);
			Map<int, Player>.ValueCollection valueCollection = (GameState.Get() != null) ? GameState.Get().GetPlayerMap().Values : null;
			if (spectatorJoinInfo != null && spectatorJoinInfo.SpectatedPlayers.Count > 0 && valueCollection != null && valueCollection.Count > 0)
			{
				for (int i = 0; i < spectatorJoinInfo.SpectatedPlayers.Count; i++)
				{
					BnetGameAccountId spectatedPlayerId = BnetUtils.CreateGameAccountId(spectatorJoinInfo.SpectatedPlayers[i]);
					if (spectatedPlayerId != myGameAccountId && Enumerable.Any<Player>(valueCollection, (Player p) => p.GetGameAccountId() == spectatedPlayerId))
					{
						result = true;
						break;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x0600296F RID: 10607 RVA: 0x000C8F6C File Offset: 0x000C716C
	public bool IsInvitedToSpectateMyGame(BnetGameAccountId gameAccountId)
	{
		PartyInvite[] sentInvites = BnetParty.GetSentInvites(this.m_spectatorPartyIdMain);
		return Enumerable.FirstOrDefault<PartyInvite>(sentInvites, (PartyInvite i) => i.InviteeId == gameAccountId) != null;
	}

	// Token: 0x06002970 RID: 10608 RVA: 0x000C8FAE File Offset: 0x000C71AE
	public bool CanKickSpectator(BnetGameAccountId gameAccountId)
	{
		return this.IsSpectatingMe(gameAccountId);
	}

	// Token: 0x06002971 RID: 10609 RVA: 0x000C8FC0 File Offset: 0x000C71C0
	public bool HasInvitedMeToSpectate(BnetGameAccountId gameAccountId)
	{
		return BnetParty.GetReceivedInviteFrom(gameAccountId, 2) != null;
	}

	// Token: 0x06002972 RID: 10610 RVA: 0x000C8FE0 File Offset: 0x000C71E0
	public bool HasAnyReceivedInvites()
	{
		PartyInvite[] array = Enumerable.ToArray<PartyInvite>(Enumerable.Where<PartyInvite>(BnetParty.GetReceivedInvites(), (PartyInvite i) => i.PartyType == 2));
		return array.Length > 0;
	}

	// Token: 0x06002973 RID: 10611 RVA: 0x000C9020 File Offset: 0x000C7220
	public bool MyGameHasSpectators()
	{
		return SceneMgr.Get().IsInGame() && this.m_gameServerKnownSpectators.Count > 0;
	}

	// Token: 0x06002974 RID: 10612 RVA: 0x000C904C File Offset: 0x000C724C
	public BnetGameAccountId GetSpectateeFriendlySide()
	{
		return this.m_spectateeFriendlySide;
	}

	// Token: 0x06002975 RID: 10613 RVA: 0x000C9054 File Offset: 0x000C7254
	public BnetGameAccountId GetSpectateeOpposingSide()
	{
		return this.m_spectateeOpposingSide;
	}

	// Token: 0x06002976 RID: 10614 RVA: 0x000C905C File Offset: 0x000C725C
	public bool IsSpectatingOpposingSide()
	{
		return this.m_spectateeOpposingSide != null;
	}

	// Token: 0x06002977 RID: 10615 RVA: 0x000C906C File Offset: 0x000C726C
	public bool HasPreviouslyKickedMeFromGame(BnetGameAccountId playerId)
	{
		if (this.m_kickedFromSpectatingList == null)
		{
			return false;
		}
		float num;
		if (this.m_kickedFromSpectatingList.TryGetValue(playerId, out num))
		{
			if (Time.realtimeSinceStartup >= num && Time.realtimeSinceStartup - num < 10f)
			{
				return true;
			}
			this.m_kickedFromSpectatingList.Remove(playerId);
			if (this.m_kickedFromSpectatingList.Count == 0)
			{
				this.m_kickedFromSpectatingList = null;
			}
		}
		return false;
	}

	// Token: 0x06002978 RID: 10616 RVA: 0x000C90DC File Offset: 0x000C72DC
	public void SpectatePlayer(BnetPlayer player)
	{
		if (this.m_pendingSpectatePlayerAfterLeave != null)
		{
			return;
		}
		if (!this.CanSpectate(player))
		{
			return;
		}
		BnetGameAccountId hearthstoneGameAccountId = player.GetHearthstoneGameAccountId();
		PartyInvite receivedInviteFrom = BnetParty.GetReceivedInviteFrom(hearthstoneGameAccountId, 2);
		if (receivedInviteFrom != null)
		{
			if (this.m_spectateeFriendlySide == null || (this.m_spectateeOpposingSide == null && this.IsPlayerInGame(hearthstoneGameAccountId)))
			{
				this.CloseWaitingForNextGameDialog();
				if (this.m_spectateeFriendlySide != null && hearthstoneGameAccountId != this.m_spectateeFriendlySide)
				{
					this.m_spectateeOpposingSide = hearthstoneGameAccountId;
				}
				BnetParty.AcceptReceivedInvite(receivedInviteFrom.InviteId);
			}
			else
			{
				this.LogInfoParty("SpectatePlayer: trying to accept an invite even though there is no room for another spectatee: player={0} spectatee1={1} spectatee2={2} isPlayerInGame={3} inviteId={4}", new object[]
				{
					string.Concat(new object[]
					{
						hearthstoneGameAccountId,
						" (",
						BnetUtils.GetPlayerBestName(hearthstoneGameAccountId),
						")"
					}),
					this.m_spectateeFriendlySide,
					this.m_spectateeOpposingSide,
					this.IsPlayerInGame(hearthstoneGameAccountId),
					receivedInviteFrom.InviteId
				});
				BnetParty.DeclineReceivedInvite(receivedInviteFrom.InviteId);
			}
			return;
		}
		JoinInfo spectatorJoinInfo = SpectatorManager.GetSpectatorJoinInfo(player.GetHearthstoneGameAccount());
		if (spectatorJoinInfo == null)
		{
			Error.AddWarningLoc("Bad Spectator Key", "Spectator key is blank!", new object[0]);
			return;
		}
		if (!spectatorJoinInfo.HasPartyId && string.IsNullOrEmpty(spectatorJoinInfo.SecretKey))
		{
			Error.AddWarningLoc("No Party/Bad Spectator Key", "No party information and Spectator key is blank!", new object[0]);
			return;
		}
		if (spectatorJoinInfo.HasPartyId && this.m_requestedInvite != null)
		{
			this.LogInfoParty("SpectatePlayer: already requesting invite from {0}:party={1}, cannot request another from {2}:party={3}", new object[]
			{
				this.m_requestedInvite.SpectateeId,
				this.m_requestedInvite.PartyId,
				hearthstoneGameAccountId,
				BnetUtils.CreatePartyId(spectatorJoinInfo.PartyId)
			});
			return;
		}
		ShownUIMgr shownUIMgr = ShownUIMgr.Get();
		if (shownUIMgr != null)
		{
			ShownUIMgr.UI_WINDOW shownUI = shownUIMgr.GetShownUI();
			if (shownUI != ShownUIMgr.UI_WINDOW.GENERAL_STORE)
			{
				if (shownUI == ShownUIMgr.UI_WINDOW.QUEST_LOG)
				{
					if (QuestLog.Get() != null)
					{
						QuestLog.Get().Hide();
					}
				}
			}
			else if (GeneralStore.Get() != null)
			{
				GeneralStore.Get().Close(false);
			}
		}
		if (!(this.m_spectateeFriendlySide != null) || !(this.m_spectateeOpposingSide == null) || GameMgr.Get() == null || !GameMgr.Get().IsSpectator())
		{
			if (this.m_spectatorPartyIdMain != null)
			{
				if (this.IsInSpectatorMode())
				{
					this.EndSpectatorMode(true);
				}
				else
				{
					this.LeaveParty(this.m_spectatorPartyIdMain, this.ShouldBePartyLeader(this.m_spectatorPartyIdMain));
				}
				this.m_pendingSpectatePlayerAfterLeave = new SpectatorManager.PendingSpectatePlayer(hearthstoneGameAccountId, spectatorJoinInfo);
				return;
			}
			if (this.m_spectatorPartyIdOpposingSide != null)
			{
				this.m_pendingSpectatePlayerAfterLeave = new SpectatorManager.PendingSpectatePlayer(hearthstoneGameAccountId, spectatorJoinInfo);
				this.LeaveParty(this.m_spectatorPartyIdOpposingSide, false);
				return;
			}
		}
		this.SpectatePlayer_Internal(hearthstoneGameAccountId, spectatorJoinInfo);
	}

	// Token: 0x06002979 RID: 10617 RVA: 0x000C93E0 File Offset: 0x000C75E0
	private void FireSpectatorModeChanged(OnlineEventType evt, BnetPlayer spectatee)
	{
		if (FriendChallengeMgr.Get() != null)
		{
			FriendChallengeMgr.Get().UpdateMyAvailability();
		}
		if (this.OnSpectatorModeChanged != null)
		{
			this.OnSpectatorModeChanged(evt, spectatee);
		}
		if (evt == null)
		{
			Screen.sleepTimeout = -1;
		}
		else if (evt == 1 && SceneMgr.Get().GetMode() != SceneMgr.Mode.GAMEPLAY)
		{
			Screen.sleepTimeout = -2;
		}
	}

	// Token: 0x0600297A RID: 10618 RVA: 0x000C9448 File Offset: 0x000C7648
	private void SpectatePlayer_Internal(BnetGameAccountId gameAccountId, JoinInfo joinInfo)
	{
		if (!this.m_initialized)
		{
			this.LogInfoParty("ERROR: SpectatePlayer_Internal called before initialized; spectatee={0}", new object[]
			{
				gameAccountId
			});
		}
		this.m_pendingSpectatePlayerAfterLeave = null;
		if (WelcomeQuests.Get() != null)
		{
			WelcomeQuests.Hide();
		}
		PartyInvite receivedInviteFrom = BnetParty.GetReceivedInviteFrom(gameAccountId, 2);
		if (this.m_spectateeFriendlySide == null)
		{
			this.LogInfoPower("================== Begin Spectating 1st player ==================", new object[0]);
			this.m_spectateeFriendlySide = gameAccountId;
			if (receivedInviteFrom != null)
			{
				this.CloseWaitingForNextGameDialog();
				BnetParty.AcceptReceivedInvite(receivedInviteFrom.InviteId);
			}
			else if (joinInfo.HasPartyId)
			{
				PartyId partyId = BnetUtils.CreatePartyId(joinInfo.PartyId);
				this.m_requestedInvite = new SpectatorManager.IntendedSpectateeParty(gameAccountId, partyId);
				BnetGameAccountId myGameAccountId = BnetPresenceMgr.Get().GetMyGameAccountId();
				BnetParty.RequestInvite(partyId, gameAccountId, myGameAccountId, 2);
				ApplicationMgr.Get().ScheduleCallback(5f, true, new ApplicationMgr.ScheduledCallback(this.SpectatePlayer_RequestInvite_FriendlySide_Timeout), null);
			}
			else
			{
				this.CloseWaitingForNextGameDialog();
				this.m_isExpectingArriveInGameplayAsSpectator = true;
				GameMgr.Get().SpectateGame(joinInfo);
			}
		}
		else if (this.m_spectateeOpposingSide == null)
		{
			if (!this.IsPlayerInGame(gameAccountId))
			{
				Error.AddWarning("Error", "Cannot spectate two different games at same time.", new object[0]);
			}
			else
			{
				if (this.m_spectateeFriendlySide == gameAccountId)
				{
					this.LogInfoParty("SpectatePlayer: already spectating player {0}", new object[]
					{
						gameAccountId
					});
					if (receivedInviteFrom != null)
					{
						BnetParty.AcceptReceivedInvite(receivedInviteFrom.InviteId);
					}
					return;
				}
				this.LogInfoPower("================== Begin Spectating 2nd player ==================", new object[0]);
				this.m_spectateeOpposingSide = gameAccountId;
				if (receivedInviteFrom != null)
				{
					BnetParty.AcceptReceivedInvite(receivedInviteFrom.InviteId);
				}
				else if (joinInfo.HasPartyId)
				{
					PartyId partyId2 = BnetUtils.CreatePartyId(joinInfo.PartyId);
					this.m_requestedInvite = new SpectatorManager.IntendedSpectateeParty(gameAccountId, partyId2);
					BnetGameAccountId myGameAccountId2 = BnetPresenceMgr.Get().GetMyGameAccountId();
					BnetParty.RequestInvite(partyId2, gameAccountId, myGameAccountId2, 2);
					ApplicationMgr.Get().ScheduleCallback(5f, true, new ApplicationMgr.ScheduledCallback(this.SpectatePlayer_RequestInvite_OpposingSide_Timeout), null);
				}
				else
				{
					this.SpectateSecondPlayer_Network(joinInfo);
				}
			}
		}
		else
		{
			if (this.m_spectateeFriendlySide == gameAccountId || this.m_spectateeOpposingSide == gameAccountId)
			{
				this.LogInfoParty("SpectatePlayer: already spectating player {0}", new object[]
				{
					gameAccountId
				});
				return;
			}
			Error.AddDevFatal("Cannot spectate more than 2 players.", new object[0]);
		}
	}

	// Token: 0x0600297B RID: 10619 RVA: 0x000C96A4 File Offset: 0x000C78A4
	private void SpectatePlayer_RequestInvite_FriendlySide_Timeout(object userData)
	{
		if (this.m_requestedInvite == null)
		{
			return;
		}
		this.m_spectateeFriendlySide = null;
		this.EndSpectatorMode(true);
		string header = GameStrings.Get("GLOBAL_SPECTATOR_SERVER_REJECTED_HEADER");
		string body = GameStrings.Get("GLOBAL_SPECTATOR_SERVER_REJECTED_TEXT");
		SpectatorManager.DisplayErrorDialog(header, body);
	}

	// Token: 0x0600297C RID: 10620 RVA: 0x000C96E8 File Offset: 0x000C78E8
	private void SpectatePlayer_RequestInvite_OpposingSide_Timeout(object userData)
	{
		if (this.m_requestedInvite == null)
		{
			return;
		}
		this.m_requestedInvite = null;
		this.m_spectateeOpposingSide = null;
		string header = GameStrings.Get("GLOBAL_SPECTATOR_SERVER_REJECTED_HEADER");
		string body = GameStrings.Get("GLOBAL_SPECTATOR_SERVER_REJECTED_TEXT");
		SpectatorManager.DisplayErrorDialog(header, body);
	}

	// Token: 0x0600297D RID: 10621 RVA: 0x000C972C File Offset: 0x000C792C
	private static JoinInfo CreateJoinInfo(PartyServerInfo serverInfo)
	{
		JoinInfo joinInfo = new JoinInfo();
		joinInfo.ServerIpAddress = serverInfo.ServerIpAddress;
		joinInfo.ServerPort = serverInfo.ServerPort;
		joinInfo.GameHandle = serverInfo.GameHandle;
		joinInfo.SecretKey = serverInfo.SecretKey;
		if (serverInfo.HasGameType)
		{
			joinInfo.GameType = serverInfo.GameType;
		}
		if (serverInfo.HasMissionId)
		{
			joinInfo.MissionId = serverInfo.MissionId;
		}
		return joinInfo;
	}

	// Token: 0x0600297E RID: 10622 RVA: 0x000C97A0 File Offset: 0x000C79A0
	private static bool IsSameGameAndServer(PartyServerInfo a, PartyServerInfo b)
	{
		if (a == null)
		{
			return b == null;
		}
		return b != null && a.ServerIpAddress == b.ServerIpAddress && a.GameHandle == b.GameHandle;
	}

	// Token: 0x0600297F RID: 10623 RVA: 0x000C97E8 File Offset: 0x000C79E8
	private static bool IsSameGameAndServer(PartyServerInfo a, GameServerInfo b)
	{
		if (a == null)
		{
			return b == null;
		}
		return b != null && a.ServerIpAddress == b.Address && a.GameHandle == b.GameHandle;
	}

	// Token: 0x06002980 RID: 10624 RVA: 0x000C9830 File Offset: 0x000C7A30
	private void SpectateSecondPlayer_Network(JoinInfo joinInfo)
	{
		GameServerInfo gameServerInfo = new GameServerInfo();
		gameServerInfo.Address = joinInfo.ServerIpAddress;
		gameServerInfo.Port = (int)joinInfo.ServerPort;
		gameServerInfo.GameHandle = joinInfo.GameHandle;
		gameServerInfo.SpectatorPassword = joinInfo.SecretKey;
		gameServerInfo.SpectatorMode = true;
		Network.Get().SpectateSecondPlayer(gameServerInfo);
	}

	// Token: 0x06002981 RID: 10625 RVA: 0x000C9888 File Offset: 0x000C7A88
	private void JoinPartyGame(PartyId partyId)
	{
		if (partyId == null)
		{
			return;
		}
		PartyInfo joinedParty = BnetParty.GetJoinedParty(partyId);
		if (joinedParty == null)
		{
			return;
		}
		this.BnetParty_OnPartyAttributeChanged_ServerInfo(joinedParty, "WTCG.Party.ServerInfo", BnetParty.GetPartyAttributeVariant(partyId, "WTCG.Party.ServerInfo"));
	}

	// Token: 0x06002982 RID: 10626 RVA: 0x000C98C8 File Offset: 0x000C7AC8
	public void LeaveSpectatorMode()
	{
		if (GameMgr.Get().IsSpectator())
		{
			if (Network.IsConnectedToGameServer())
			{
				Network.DisconnectFromGameServer();
			}
			else
			{
				this.LeaveGameScene();
			}
		}
		if (this.m_spectatorPartyIdOpposingSide != null)
		{
			this.LeaveParty(this.m_spectatorPartyIdOpposingSide, false);
		}
		if (this.m_spectatorPartyIdMain != null)
		{
			this.LeaveParty(this.m_spectatorPartyIdMain, false);
		}
	}

	// Token: 0x06002983 RID: 10627 RVA: 0x000C993C File Offset: 0x000C7B3C
	public void InviteToSpectateMe(BnetPlayer player)
	{
		if (player == null)
		{
			return;
		}
		BnetGameAccountId hearthstoneGameAccountId = player.GetHearthstoneGameAccountId();
		if (this.m_kickedPlayers != null && this.m_kickedPlayers.Contains(hearthstoneGameAccountId))
		{
			this.m_kickedPlayers.Remove(hearthstoneGameAccountId);
		}
		if (!this.CanInviteToSpectateMyGame(hearthstoneGameAccountId))
		{
			return;
		}
		if (this.m_userInitiatedOutgoingInvites == null)
		{
			this.m_userInitiatedOutgoingInvites = new HashSet<BnetGameAccountId>();
		}
		this.m_userInitiatedOutgoingInvites.Add(hearthstoneGameAccountId);
		if (this.m_spectatorPartyIdMain == null)
		{
			BnetGameAccountId myGameAccountId = BnetPresenceMgr.Get().GetMyGameAccountId();
			BnetId bnetId = BnetUtils.CreatePegasusBnetId(myGameAccountId);
			byte[] array = ProtobufUtil.ToByteArray(bnetId);
			BnetParty.CreateParty(2, 3, array, null);
		}
		else
		{
			BnetParty.SendInvite(this.m_spectatorPartyIdMain, hearthstoneGameAccountId);
		}
	}

	// Token: 0x06002984 RID: 10628 RVA: 0x000C99F4 File Offset: 0x000C7BF4
	public void KickSpectator(BnetPlayer player, bool regenerateSpectatorPassword)
	{
		this.KickSpectator_Internal(player, regenerateSpectatorPassword, true);
	}

	// Token: 0x06002985 RID: 10629 RVA: 0x000C9A00 File Offset: 0x000C7C00
	private void KickSpectator_Internal(BnetPlayer player, bool regenerateSpectatorPassword, bool addToKickList)
	{
		if (player == null)
		{
			return;
		}
		BnetGameAccountId hearthstoneGameAccountId = player.GetHearthstoneGameAccountId();
		if (!this.CanKickSpectator(hearthstoneGameAccountId))
		{
			return;
		}
		if (addToKickList)
		{
			if (this.m_kickedPlayers == null)
			{
				this.m_kickedPlayers = new HashSet<BnetGameAccountId>();
			}
			this.m_kickedPlayers.Add(hearthstoneGameAccountId);
		}
		if (Network.IsConnectedToGameServer())
		{
			Network.Get().SendRemoveSpectators(regenerateSpectatorPassword, new BnetGameAccountId[]
			{
				hearthstoneGameAccountId
			});
		}
		if (this.m_spectatorPartyIdMain != null && this.ShouldBePartyLeader(this.m_spectatorPartyIdMain) && BnetParty.IsMember(this.m_spectatorPartyIdMain, hearthstoneGameAccountId))
		{
			BnetParty.KickMember(this.m_spectatorPartyIdMain, hearthstoneGameAccountId);
		}
	}

	// Token: 0x06002986 RID: 10630 RVA: 0x000C9AAF File Offset: 0x000C7CAF
	public void UpdateMySpectatorInfo()
	{
		this.UpdateSpectatorPresence();
		this.UpdateSpectatorPartyServerInfo();
	}

	// Token: 0x06002987 RID: 10631 RVA: 0x000C9AC0 File Offset: 0x000C7CC0
	private JoinInfo GetMyGameJoinInfo()
	{
		JoinInfo result = null;
		JoinInfo joinInfo = new JoinInfo();
		if (this.IsInSpectatorMode())
		{
			if (this.m_spectateeFriendlySide != null)
			{
				BnetId bnetId = BnetUtils.CreatePegasusBnetId(this.m_spectateeFriendlySide);
				joinInfo.SpectatedPlayers.Add(bnetId);
			}
			if (this.m_spectateeOpposingSide != null)
			{
				BnetId bnetId2 = BnetUtils.CreatePegasusBnetId(this.m_spectateeOpposingSide);
				joinInfo.SpectatedPlayers.Add(bnetId2);
			}
			if (joinInfo.SpectatedPlayers.Count > 0)
			{
				result = joinInfo;
			}
		}
		else if (SceneMgr.Get().IsInGame())
		{
			int countSpectatingMe = this.GetCountSpectatingMe();
			if (this.CanAddSpectators())
			{
				GameServerInfo lastGameServerJoined = Network.Get().GetLastGameServerJoined();
				if (this.m_spectatorPartyIdMain == null && lastGameServerJoined != null && SceneMgr.Get().IsInGame() && !SpectatorManager.IsGameOver)
				{
					joinInfo.ServerIpAddress = lastGameServerJoined.Address;
					joinInfo.ServerPort = (uint)lastGameServerJoined.Port;
					joinInfo.GameHandle = lastGameServerJoined.GameHandle;
					joinInfo.SecretKey = (lastGameServerJoined.SpectatorPassword ?? string.Empty);
				}
				if (this.m_spectatorPartyIdMain != null)
				{
					BnetId partyId = BnetUtils.CreatePegasusBnetId(this.m_spectatorPartyIdMain);
					joinInfo.PartyId = partyId;
				}
			}
			joinInfo.CurrentNumSpectators = countSpectatingMe;
			joinInfo.MaxNumSpectators = 10;
			joinInfo.IsJoinable = (joinInfo.CurrentNumSpectators < joinInfo.MaxNumSpectators);
			joinInfo.GameType = GameMgr.Get().GetGameType();
			joinInfo.MissionId = GameMgr.Get().GetMissionId();
			result = joinInfo;
		}
		return result;
	}

	// Token: 0x06002988 RID: 10632 RVA: 0x000C9C58 File Offset: 0x000C7E58
	private static PartyServerInfo GetPartyServerInfo(PartyId partyId)
	{
		byte[] partyAttributeBlob = BnetParty.GetPartyAttributeBlob(partyId, "WTCG.Party.ServerInfo");
		return (partyAttributeBlob != null) ? ProtobufUtil.ParseFrom<PartyServerInfo>(partyAttributeBlob, 0, -1) : null;
	}

	// Token: 0x06002989 RID: 10633 RVA: 0x000C9C88 File Offset: 0x000C7E88
	public bool HandleDisconnectFromGameplay()
	{
		bool flag = this.m_expectedDisconnectReason != null;
		this.EndCurrentSpectatedGame(false);
		if (flag)
		{
			if (GameMgr.Get().IsTransitionPopupShown())
			{
				GameMgr.Get().GetTransitionPopup().Cancel();
			}
			else
			{
				this.LeaveGameScene();
			}
		}
		return flag;
	}

	// Token: 0x0600298A RID: 10634 RVA: 0x000C9CD8 File Offset: 0x000C7ED8
	public void OnRealTimeGameOver()
	{
		this.UpdateMySpectatorInfo();
	}

	// Token: 0x0600298B RID: 10635 RVA: 0x000C9CE0 File Offset: 0x000C7EE0
	private void EndCurrentSpectatedGame(bool isLeavingGameplay)
	{
		if (isLeavingGameplay && this.IsInSpectatorMode())
		{
			SoundManager.Get().LoadAndPlay("SpectatorMode_Exit");
		}
		this.m_expectedDisconnectReason = default(int?);
		this.m_isExpectingArriveInGameplayAsSpectator = false;
		this.ClearAllGameServerKnownSpectators();
		if (ApplicationMgr.Get() != null && !ApplicationMgr.Get().IsResetting())
		{
			this.UpdateSpectatorPresence();
		}
		if (GameMgr.Get() != null && GameMgr.Get().GetTransitionPopup() != null)
		{
			GameMgr.Get().GetTransitionPopup().OnHidden -= new Action<TransitionPopup>(this.EnterSpectatorMode_OnTransitionPopupHide);
		}
	}

	// Token: 0x0600298C RID: 10636 RVA: 0x000C9D88 File Offset: 0x000C7F88
	private void EndSpectatorMode(bool wasKnownSpectating = false)
	{
		bool isExpectingArriveInGameplayAsSpectator = this.m_isExpectingArriveInGameplayAsSpectator;
		bool flag = wasKnownSpectating || this.m_spectateeFriendlySide != null || this.m_spectateeOpposingSide != null;
		this.LeaveSpectatorMode();
		this.EndCurrentSpectatedGame(false);
		this.m_spectateeFriendlySide = null;
		this.m_spectateeOpposingSide = null;
		this.m_requestedInvite = null;
		this.CloseWaitingForNextGameDialog();
		this.m_pendingSpectatePlayerAfterLeave = null;
		this.m_isExpectingArriveInGameplayAsSpectator = false;
		if (flag)
		{
			this.LogInfoPower("================== End Spectator Mode ==================", new object[0]);
			BnetPlayer player = BnetUtils.GetPlayer(this.m_spectateeFriendlySide);
			this.FireSpectatorModeChanged(1, player);
		}
		if (isExpectingArriveInGameplayAsSpectator)
		{
			SceneMgr.Mode mode = SceneMgr.Mode.HUB;
			if (!GameUtils.AreAllTutorialsComplete())
			{
				Network.Get().ShowBreakingNewsOrError("GLOBAL_ERROR_NETWORK_LOST_GAME_CONNECTION", 0f);
			}
			else if (!SceneMgr.Get().IsModeRequested(mode))
			{
				SceneMgr.Get().SetNextMode(mode);
			}
			else if (SceneMgr.Get().GetMode() == mode)
			{
				SceneMgr.Get().ReloadMode();
			}
		}
	}

	// Token: 0x0600298D RID: 10637 RVA: 0x000C9E8C File Offset: 0x000C808C
	private void ClearAllCacheForReset()
	{
		this.EndSpectatorMode(false);
		this.m_initialized = false;
		this.m_spectatorPartyIdMain = null;
		this.m_spectatorPartyIdOpposingSide = null;
		this.m_requestedInvite = null;
		this.m_waitingForNextGameDialog = null;
		this.m_pendingSpectatePlayerAfterLeave = null;
		this.m_userInitiatedOutgoingInvites = null;
		this.m_kickedPlayers = null;
		this.m_kickedFromSpectatingList = null;
		this.m_expectedDisconnectReason = default(int?);
		this.m_isExpectingArriveInGameplayAsSpectator = false;
		this.m_isShowingRemovedAsSpectatorPopup = false;
		this.m_gameServerKnownSpectators.Clear();
	}

	// Token: 0x0600298E RID: 10638 RVA: 0x000C9F08 File Offset: 0x000C8108
	private void WillReset()
	{
		this.ClearAllCacheForReset();
		if (ApplicationMgr.Get() != null)
		{
			ApplicationMgr.Get().CancelScheduledCallback(new ApplicationMgr.ScheduledCallback(this.SpectatorManager_UpdatePresenceNextFrame), null);
		}
	}

	// Token: 0x0600298F RID: 10639 RVA: 0x000C9F44 File Offset: 0x000C8144
	private bool OnFindGameEvent(FindGameEventData eventData, object userData)
	{
		switch (eventData.m_state)
		{
		case FindGameState.CLIENT_CANCELED:
		case FindGameState.CLIENT_ERROR:
		case FindGameState.BNET_QUEUE_CANCELED:
		case FindGameState.BNET_ERROR:
			if (this.IsInSpectatorMode())
			{
				this.EndSpectatorMode(true);
			}
			break;
		case FindGameState.SERVER_GAME_CANCELED:
			if (this.IsInSpectatorMode())
			{
				string header = GameStrings.Get("GLOBAL_SPECTATOR_SERVER_REJECTED_HEADER");
				string body = GameStrings.Get("GLOBAL_SPECTATOR_SERVER_REJECTED_TEXT");
				SpectatorManager.DisplayErrorDialog(header, body);
				this.EndSpectatorMode(true);
			}
			break;
		}
		return false;
	}

	// Token: 0x06002990 RID: 10640 RVA: 0x000C9FD9 File Offset: 0x000C81D9
	private void GameState_InitializedEvent(GameState instance, object userData)
	{
		if (this.m_spectatorPartyIdOpposingSide != null)
		{
			GameState.Get().RegisterCreateGameListener(new GameState.CreateGameCallback(this.GameState_CreateGameEvent), null);
		}
	}

	// Token: 0x06002991 RID: 10641 RVA: 0x000CA004 File Offset: 0x000C8204
	private void GameState_CreateGameEvent(GameState.CreateGamePhase createGamePhase, object userData)
	{
		if (createGamePhase < GameState.CreateGamePhase.CREATED)
		{
			return;
		}
		GameState.Get().UnregisterCreateGameListener(new GameState.CreateGameCallback(this.GameState_CreateGameEvent));
		if (this.m_spectatorPartyIdOpposingSide != null)
		{
			this.AutoSpectateOpposingSide();
		}
	}

	// Token: 0x06002992 RID: 10642 RVA: 0x000CA048 File Offset: 0x000C8248
	private void AutoSpectateOpposingSide()
	{
		if (GameState.Get() == null)
		{
			return;
		}
		if (GameState.Get().GetCreateGamePhase() < GameState.CreateGamePhase.CREATED)
		{
			GameState.Get().RegisterCreateGameListener(new GameState.CreateGameCallback(this.GameState_CreateGameEvent), null);
			return;
		}
		if (SceneMgr.Get().GetMode() != SceneMgr.Mode.GAMEPLAY)
		{
			return;
		}
		if (GameMgr.Get().GetTransitionPopup() != null && GameMgr.Get().GetTransitionPopup().IsShown())
		{
			GameMgr.Get().GetTransitionPopup().OnHidden += new Action<TransitionPopup>(this.EnterSpectatorMode_OnTransitionPopupHide);
			return;
		}
		if (this.m_spectatorPartyIdOpposingSide != null && this.m_spectateeOpposingSide != null && this.IsStillInParty(this.m_spectatorPartyIdOpposingSide))
		{
			if (this.IsPlayerInGame(this.m_spectateeOpposingSide))
			{
				PartyServerInfo partyServerInfo = SpectatorManager.GetPartyServerInfo(this.m_spectatorPartyIdOpposingSide);
				JoinInfo joinInfo = (partyServerInfo != null) ? SpectatorManager.CreateJoinInfo(partyServerInfo) : null;
				if (joinInfo != null)
				{
					this.SpectateSecondPlayer_Network(joinInfo);
				}
			}
			else
			{
				this.LogInfoPower("================== End Spectating 2nd player ==================", new object[0]);
				this.LeaveParty(this.m_spectatorPartyIdOpposingSide, false);
			}
		}
	}

	// Token: 0x06002993 RID: 10643 RVA: 0x000CA174 File Offset: 0x000C8374
	private void OnSceneUnloaded(SceneMgr.Mode prevMode, Scene prevScene, object userData)
	{
		SceneMgr.Mode mode = SceneMgr.Get().GetMode();
		if (mode == SceneMgr.Mode.GAMEPLAY && prevMode != SceneMgr.Mode.GAMEPLAY)
		{
			if (this.m_spectateeFriendlySide != null)
			{
				BnetBar.Get().HideFriendList();
			}
			if (GameMgr.Get().IsSpectator())
			{
				if (GameMgr.Get().GetTransitionPopup() != null)
				{
					GameMgr.Get().GetTransitionPopup().OnHidden += new Action<TransitionPopup>(this.EnterSpectatorMode_OnTransitionPopupHide);
				}
				BnetPlayer player = BnetUtils.GetPlayer(this.m_spectateeOpposingSide ?? this.m_spectateeFriendlySide);
				this.FireSpectatorModeChanged(0, player);
			}
			else
			{
				this.m_kickedPlayers = null;
			}
			this.CloseWaitingForNextGameDialog();
			this.DeclineAllReceivedInvitations();
			this.UpdateMySpectatorInfo();
		}
		else if (prevMode == SceneMgr.Mode.GAMEPLAY && mode != SceneMgr.Mode.GAMEPLAY)
		{
			if (this.IsInSpectatorMode())
			{
				this.LogInfoPower("================== End Spectator Game ==================", new object[0]);
				if (SceneDebugger.Get())
				{
					Time.timeScale = SceneDebugger.GetDevTimescale();
				}
				else
				{
					Time.timeScale = 1f;
				}
			}
			this.EndCurrentSpectatedGame(true);
			this.UpdateMySpectatorInfo();
			if (this.m_spectatorPartyIdMain != null && this.IsStillInParty(this.m_spectatorPartyIdMain) && !this.ShouldBePartyLeader(this.m_spectatorPartyIdMain))
			{
				PartyServerInfo partyServerInfo = SpectatorManager.GetPartyServerInfo(this.m_spectatorPartyIdMain);
				if (partyServerInfo == null)
				{
					this.ShowWaitingForNextGameDialog();
				}
				else
				{
					GameServerInfo lastGameServerJoined = Network.Get().GetLastGameServerJoined();
					if (!SpectatorManager.IsSameGameAndServer(partyServerInfo, lastGameServerJoined))
					{
						this.LogInfoPower("================== OnSceneUnloaded: auto-spectating game after leaving game ==================", new object[0]);
						this.BnetParty_OnPartyAttributeChanged_ServerInfo(new PartyInfo(this.m_spectatorPartyIdMain, 2), "WTCG.Party.ServerInfo", BnetParty.GetPartyAttributeVariant(this.m_spectatorPartyIdMain, "WTCG.Party.ServerInfo"));
					}
					else
					{
						this.ShowWaitingForNextGameDialog();
					}
				}
			}
		}
	}

	// Token: 0x06002994 RID: 10644 RVA: 0x000CA344 File Offset: 0x000C8544
	public void ShowWaitingForNextGameDialog()
	{
		if (!Network.IsLoggedIn())
		{
			return;
		}
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_id = "SPECTATOR_WAITING_FOR_NEXT_GAME";
		popupInfo.m_layerToUse = new GameLayer?(GameLayer.BackgroundUI);
		popupInfo.m_headerText = GameStrings.Get("GLOBAL_SPECTATOR_WAITING_FOR_NEXT_GAME_HEADER");
		popupInfo.m_text = this.GetWaitingForNextGameDialogText();
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.CANCEL;
		popupInfo.m_cancelText = GameStrings.Get("GLOBAL_LEAVE_SPECTATOR_MODE");
		popupInfo.m_responseCallback = new AlertPopup.ResponseCallback(this.OnSceneUnloaded_AwaitingNextGame_LeaveSpectatorMode);
		popupInfo.m_keyboardEscIsCancel = false;
		DialogManager.Get().ShowUniquePopup(popupInfo, new DialogManager.DialogProcessCallback(this.OnSceneUnloaded_AwaitingNextGame_DialogProcessCallback));
		ApplicationMgr.Get().CancelScheduledCallback(new ApplicationMgr.ScheduledCallback(SpectatorManager.WaitingForNextGame_AutoLeaveSpectatorMode), null);
		if (SpectatorManager.WAITING_FOR_NEXT_GAME_AUTO_LEAVE_SECONDS >= 0f)
		{
			ApplicationMgr.Get().ScheduleCallback(SpectatorManager.WAITING_FOR_NEXT_GAME_AUTO_LEAVE_SECONDS, true, new ApplicationMgr.ScheduledCallback(SpectatorManager.WaitingForNextGame_AutoLeaveSpectatorMode), null);
		}
	}

	// Token: 0x06002995 RID: 10645 RVA: 0x000CA430 File Offset: 0x000C8630
	private void CloseWaitingForNextGameDialog()
	{
		if (SpectatorManager.DISABLE_MENU_BUTTON_WHILE_WAITING)
		{
			BnetBar.Get().m_menuButton.SetEnabled(true);
		}
		if (DialogManager.Get() != null)
		{
			DialogManager.Get().RemoveUniquePopupRequestFromQueue("SPECTATOR_WAITING_FOR_NEXT_GAME");
		}
		if (this.m_waitingForNextGameDialog != null)
		{
			this.m_waitingForNextGameDialog.Hide();
			this.m_waitingForNextGameDialog = null;
		}
		ApplicationMgr.Get().CancelScheduledCallback(new ApplicationMgr.ScheduledCallback(SpectatorManager.WaitingForNextGame_AutoLeaveSpectatorMode), null);
	}

	// Token: 0x06002996 RID: 10646 RVA: 0x000CA4B8 File Offset: 0x000C86B8
	private void UpdateWaitingForNextGameDialog()
	{
		if (this.m_waitingForNextGameDialog == null)
		{
			return;
		}
		string waitingForNextGameDialogText = this.GetWaitingForNextGameDialogText();
		this.m_waitingForNextGameDialog.BodyText = waitingForNextGameDialogText;
	}

	// Token: 0x06002997 RID: 10647 RVA: 0x000CA4EC File Offset: 0x000C86EC
	private string GetWaitingForNextGameDialogText()
	{
		BnetPlayer player = BnetUtils.GetPlayer(this.m_spectateeFriendlySide);
		string playerBestName = BnetUtils.GetPlayerBestName(this.m_spectateeFriendlySide);
		string text = (player == null || !player.IsOnline()) ? GameStrings.Get("GLOBAL_OFFLINE") : PresenceMgr.Get().GetStatusText(player);
		if (text != null)
		{
			text = text.Trim();
		}
		string key = "GLOBAL_SPECTATOR_WAITING_FOR_NEXT_GAME_TEXT_OFFLINE";
		if (player != null && player.IsOnline() && !string.IsNullOrEmpty(text))
		{
			key = "GLOBAL_SPECTATOR_WAITING_FOR_NEXT_GAME_TEXT";
			Enum[] statusEnums = PresenceMgr.Get().GetStatusEnums(player);
			if (statusEnums.Length > 0 && (PresenceStatus)statusEnums[0] == PresenceStatus.ADVENTURE_SCENARIO_SELECT)
			{
				key = "GLOBAL_SPECTATOR_WAITING_FOR_NEXT_GAME_TEXT_ENTERING";
			}
			else if (statusEnums.Length > 0 && (PresenceStatus)statusEnums[0] == PresenceStatus.ADVENTURE_SCENARIO_PLAYING_GAME)
			{
				if (statusEnums.Length > 1 && GameUtils.IsHeroicAdventureMission((int)((ScenarioDbId)statusEnums[1])))
				{
					key = "GLOBAL_SPECTATOR_WAITING_FOR_NEXT_GAME_TEXT_BATTLING";
				}
				else if (statusEnums.Length > 1 && GameUtils.IsClassChallengeMission((int)((ScenarioDbId)statusEnums[1])))
				{
					key = "GLOBAL_SPECTATOR_WAITING_FOR_NEXT_GAME_TEXT_PLAYING";
				}
			}
		}
		return GameStrings.Format(key, new object[]
		{
			playerBestName,
			text
		});
	}

	// Token: 0x06002998 RID: 10648 RVA: 0x000CA624 File Offset: 0x000C8824
	private bool OnSceneUnloaded_AwaitingNextGame_DialogProcessCallback(DialogBase dialog, object userData)
	{
		if (SceneMgr.Get().IsInGame() || (GameMgr.Get() != null && GameMgr.Get().IsFindingGame()))
		{
			return false;
		}
		this.m_waitingForNextGameDialog = (AlertPopup)dialog;
		this.UpdateWaitingForNextGameDialog();
		if (SpectatorManager.DISABLE_MENU_BUTTON_WHILE_WAITING)
		{
			BnetBar.Get().m_menuButton.SetEnabled(false);
		}
		return true;
	}

	// Token: 0x06002999 RID: 10649 RVA: 0x000CA690 File Offset: 0x000C8890
	private static void WaitingForNextGame_AutoLeaveSpectatorMode(object userData)
	{
		if (!SpectatorManager.Get().IsInSpectatorMode() || SceneMgr.Get().IsInGame())
		{
			return;
		}
		SpectatorManager.Get().LeaveSpectatorMode();
		string header = GameStrings.Get("GLOBAL_SPECTATOR_WAITING_FOR_NEXT_GAME_HEADER");
		string body = GameStrings.Format("GLOBAL_SPECTATOR_WAITING_FOR_NEXT_GAME_TIMEOUT", new object[0]);
		SpectatorManager.DisplayErrorDialog(header, body);
	}

	// Token: 0x0600299A RID: 10650 RVA: 0x000CA6E9 File Offset: 0x000C88E9
	private void OnSceneUnloaded_AwaitingNextGame_LeaveSpectatorMode(AlertPopup.Response response, object userData)
	{
		this.LeaveSpectatorMode();
	}

	// Token: 0x0600299B RID: 10651 RVA: 0x000CA6F4 File Offset: 0x000C88F4
	private void EnterSpectatorMode_OnTransitionPopupHide(TransitionPopup popup)
	{
		popup.OnHidden -= new Action<TransitionPopup>(this.EnterSpectatorMode_OnTransitionPopupHide);
		if (SoundManager.Get() != null)
		{
			SoundManager.Get().LoadAndPlay("SpectatorMode_Enter");
		}
		if (this.m_spectateeOpposingSide != null)
		{
			this.AutoSpectateOpposingSide();
		}
	}

	// Token: 0x0600299C RID: 10652 RVA: 0x000CA74C File Offset: 0x000C894C
	private void OnSpectatorOpenJoinOptionChanged(Option option, object prevValue, bool existed, object userData)
	{
		bool @bool = Options.Get().GetBool(Option.SPECTATOR_OPEN_JOIN);
		bool flag = !existed || (bool)prevValue != @bool;
		if (flag && SceneMgr.Get() && SceneMgr.Get().IsInGame() && (GameMgr.Get() == null || !GameMgr.Get().IsSpectator()))
		{
			JoinInfo protoMessage;
			if (@bool)
			{
				protoMessage = this.GetMyGameJoinInfo();
			}
			else
			{
				protoMessage = null;
			}
			if (Network.ShouldBeConnectedToAurora())
			{
				BnetPresenceMgr.Get().SetGameFieldBlob(21U, protoMessage);
			}
		}
	}

	// Token: 0x0600299D RID: 10653 RVA: 0x000CA7E8 File Offset: 0x000C89E8
	private void Network_OnSpectatorInviteReceived(Invite protoInvite)
	{
		BnetGameAccountId inviterId = BnetUtils.CreateGameAccountId(protoInvite.InviterGameAccountId);
		this.AddReceivedInvitation(inviterId, protoInvite.JoinInfo);
	}

	// Token: 0x0600299E RID: 10654 RVA: 0x000CA810 File Offset: 0x000C8A10
	private void Network_OnSpectatorInviteReceived_ResponseCallback(AlertPopup.Response response, object userData)
	{
		BnetGameAccountId bnetGameAccountId = (BnetGameAccountId)userData;
		if (response == AlertPopup.Response.CANCEL)
		{
			this.RemoveReceivedInvitation(bnetGameAccountId);
			return;
		}
		BnetPlayer player = BnetUtils.GetPlayer(bnetGameAccountId);
		if (player == null)
		{
			return;
		}
		this.SpectatePlayer(player);
	}

	// Token: 0x0600299F RID: 10655 RVA: 0x000CA848 File Offset: 0x000C8A48
	private void Network_OnSpectatorNotifyEvent()
	{
		SpectatorNotify spectatorNotify = Network.GetSpectatorNotify();
		if (spectatorNotify.HasSpectatorPasswordUpdate && !string.IsNullOrEmpty(spectatorNotify.SpectatorPasswordUpdate))
		{
			GameServerInfo lastGameServerJoined = Network.Get().GetLastGameServerJoined();
			if (!spectatorNotify.SpectatorPasswordUpdate.Equals(lastGameServerJoined.SpectatorPassword))
			{
				lastGameServerJoined.SpectatorPassword = spectatorNotify.SpectatorPasswordUpdate;
				this.UpdateMySpectatorInfo();
				this.RevokeAllSentInvitations();
			}
		}
		if (spectatorNotify.HasSpectatorRemoved)
		{
			this.m_expectedDisconnectReason = new int?(spectatorNotify.SpectatorRemoved.ReasonCode);
			bool flag = GameMgr.Get().IsTransitionPopupShown();
			if (spectatorNotify.SpectatorRemoved.ReasonCode == 0)
			{
				if (spectatorNotify.SpectatorRemoved.HasRemovedBy)
				{
					if (this.m_kickedFromSpectatingList == null)
					{
						this.m_kickedFromSpectatingList = new Map<BnetGameAccountId, float>();
					}
					BnetGameAccountId key = BnetUtils.CreateGameAccountId(spectatorNotify.SpectatorRemoved.RemovedBy);
					this.m_kickedFromSpectatingList[key] = Time.realtimeSinceStartup;
				}
				if (!this.m_isShowingRemovedAsSpectatorPopup)
				{
					AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
					popupInfo.m_headerText = GameStrings.Get("GLOBAL_SPECTATOR_REMOVED_PROMPT_HEADER");
					popupInfo.m_text = GameStrings.Get("GLOBAL_SPECTATOR_REMOVED_PROMPT_TEXT");
					popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
					if (flag)
					{
						popupInfo.m_responseCallback = new AlertPopup.ResponseCallback(this.Network_OnSpectatorNotifyEvent_Removed_GoToNextMode);
					}
					else
					{
						popupInfo.m_responseCallback = delegate(AlertPopup.Response r, object data)
						{
							SpectatorManager.Get().m_isShowingRemovedAsSpectatorPopup = false;
						};
					}
					this.m_isShowingRemovedAsSpectatorPopup = true;
					DialogManager.Get().ShowPopup(popupInfo);
				}
			}
			else if (flag)
			{
				this.Network_OnSpectatorNotifyEvent_Removed_GoToNextMode(AlertPopup.Response.OK, null);
			}
			SoundManager.Get().LoadAndPlay("SpectatorMode_Exit");
			this.EndSpectatorMode(true);
			this.m_expectedDisconnectReason = new int?(spectatorNotify.SpectatorRemoved.ReasonCode);
		}
		if (spectatorNotify == null || spectatorNotify.SpectatorChange.Count == 0)
		{
			return;
		}
		if (GameMgr.Get() != null && GameMgr.Get().IsSpectator())
		{
			return;
		}
		foreach (SpectatorChange spectatorChange in spectatorNotify.SpectatorChange)
		{
			BnetGameAccountId gameAccountId = BnetUtils.CreateGameAccountId(spectatorChange.GameAccountId);
			if (spectatorChange.IsRemoved)
			{
				this.RemoveKnownSpectator(gameAccountId);
			}
			else
			{
				this.AddKnownSpectator(gameAccountId);
				this.ReinviteKnownSpectatorsNotInParty();
			}
		}
	}

	// Token: 0x060029A0 RID: 10656 RVA: 0x000CAAB4 File Offset: 0x000C8CB4
	private void Network_OnSpectatorNotifyEvent_Removed_GoToNextMode(AlertPopup.Response response, object userData)
	{
		this.m_isShowingRemovedAsSpectatorPopup = false;
	}

	// Token: 0x060029A1 RID: 10657 RVA: 0x000CAAC0 File Offset: 0x000C8CC0
	private void ReceivedInvitation_ExpireTimeout(object userData)
	{
		this.PruneOldInvites();
		if (this.m_receivedSpectateMeInvites.Count > 0)
		{
			float num = Enumerable.Min<KeyValuePair<BnetGameAccountId, SpectatorManager.ReceivedInvite>>(this.m_receivedSpectateMeInvites, (KeyValuePair<BnetGameAccountId, SpectatorManager.ReceivedInvite> kv) => kv.Value.m_timestamp);
			float secondsToWait = Mathf.Max(0f, num + 300f - Time.realtimeSinceStartup);
			ApplicationMgr.Get().ScheduleCallback(secondsToWait, true, new ApplicationMgr.ScheduledCallback(this.ReceivedInvitation_ExpireTimeout), null);
		}
	}

	// Token: 0x060029A2 RID: 10658 RVA: 0x000CAB40 File Offset: 0x000C8D40
	private void Presence_OnGameAccountPresenceChange(PresenceUpdate[] updates)
	{
		for (int k = 0; k < updates.Length; k++)
		{
			PresenceUpdate presenceUpdate = updates[k];
			BnetGameAccountId entityId = BnetGameAccountId.CreateFromEntityId(presenceUpdate.entityId);
			bool flag = presenceUpdate.fieldId == 1U && presenceUpdate.programId == BnetProgramId.BNET;
			bool flag2 = presenceUpdate.programId == BnetProgramId.HEARTHSTONE && presenceUpdate.fieldId == 17U;
			if (this.m_waitingForNextGameDialog != null && this.m_spectateeFriendlySide != null && (flag || flag2) && entityId == this.m_spectateeFriendlySide)
			{
				this.UpdateWaitingForNextGameDialog();
			}
			if (flag && presenceUpdate.boolVal)
			{
				foreach (PartyId partyId in BnetParty.GetJoinedPartyIds())
				{
					if (BnetParty.IsLeader(partyId) && !BnetParty.IsMember(partyId, entityId))
					{
						BnetGameAccountId partyCreator = this.GetPartyCreator(partyId);
						if (partyCreator != null && partyCreator == entityId && !Enumerable.Any<PartyInvite>(BnetParty.GetSentInvites(partyId), (PartyInvite i) => i.InviteeId == entityId))
						{
							BnetParty.SendInvite(partyId, entityId);
						}
					}
				}
			}
		}
	}

	// Token: 0x060029A3 RID: 10659 RVA: 0x000CACD4 File Offset: 0x000C8ED4
	private void BnetFriendMgr_OnFriendsChanged(BnetFriendChangelist changelist, object userData)
	{
		if (changelist == null || !this.IsBeingSpectated())
		{
			return;
		}
		List<BnetPlayer> removedFriends = changelist.GetRemovedFriends();
		if (removedFriends == null)
		{
			return;
		}
		foreach (BnetPlayer bnetPlayer in removedFriends)
		{
			BnetGameAccountId hearthstoneGameAccountId = bnetPlayer.GetHearthstoneGameAccountId();
			if (this.IsSpectatingMe(hearthstoneGameAccountId))
			{
				this.KickSpectator_Internal(bnetPlayer, true, false);
			}
		}
	}

	// Token: 0x060029A4 RID: 10660 RVA: 0x000CAD60 File Offset: 0x000C8F60
	private void EndGameScreen_OnTwoScoopsShown(bool shown, EndGameTwoScoop twoScoops)
	{
		if (!this.IsInSpectatorMode())
		{
			return;
		}
		if (shown)
		{
			ApplicationMgr.Get().ScheduleCallback(5f, false, new ApplicationMgr.ScheduledCallback(this.EndGameScreen_OnTwoScoopsShown_AutoClose), null);
		}
		else
		{
			ApplicationMgr.Get().CancelScheduledCallback(new ApplicationMgr.ScheduledCallback(this.EndGameScreen_OnTwoScoopsShown_AutoClose), null);
		}
	}

	// Token: 0x060029A5 RID: 10661 RVA: 0x000CADBC File Offset: 0x000C8FBC
	private void EndGameScreen_OnTwoScoopsShown_AutoClose(object userData)
	{
		if (EndGameScreen.Get() == null)
		{
			return;
		}
		float num = SpectatorManager.WAITING_FOR_NEXT_GAME_AUTO_LEAVE_SECONDS;
		if (num >= 0f)
		{
			while (EndGameScreen.Get().ContinueEvents())
			{
			}
		}
		else
		{
			EndGameScreen.Get().ContinueEvents();
		}
	}

	// Token: 0x060029A6 RID: 10662 RVA: 0x000CAE14 File Offset: 0x000C9014
	private void BnetParty_OnError(PartyError error)
	{
		if (error.IsOperationCallback)
		{
			switch (error.FeatureEvent)
			{
			case 23:
				if (error.ErrorCode != 0)
				{
					this.m_userInitiatedOutgoingInvites = null;
					string header = GameStrings.Get("GLOBAL_ERROR_GENERIC_HEADER");
					string body = GameStrings.Format("GLOBAL_SPECTATOR_ERROR_CREATE_PARTY_TEXT", new object[0]);
					SpectatorManager.DisplayErrorDialog(header, body);
				}
				break;
			case 25:
			case 26:
				if (this.m_leavePartyIdsRequested != null)
				{
					this.m_leavePartyIdsRequested.Remove(error.PartyId);
				}
				if (this.m_pendingSpectatePlayerAfterLeave != null && error.ErrorCode != 0)
				{
					string playerBestName = BnetUtils.GetPlayerBestName(this.m_pendingSpectatePlayerAfterLeave.SpectateeId);
					string header2 = GameStrings.Get("GLOBAL_ERROR_GENERIC_HEADER");
					string body2 = GameStrings.Format("GLOBAL_SPECTATOR_ERROR_LEAVE_FOR_SPECTATE_PLAYER_TEXT", new object[]
					{
						playerBestName
					});
					SpectatorManager.DisplayErrorDialog(header2, body2);
					this.m_pendingSpectatePlayerAfterLeave = null;
				}
				break;
			}
		}
	}

	// Token: 0x060029A7 RID: 10663 RVA: 0x000CAF18 File Offset: 0x000C9118
	private static void DisplayErrorDialog(string header, string body)
	{
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = header;
		popupInfo.m_text = body;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
		DialogManager.Get().ShowPopup(popupInfo);
	}

	// Token: 0x060029A8 RID: 10664 RVA: 0x000CAF4C File Offset: 0x000C914C
	private void BnetParty_OnJoined(OnlineEventType evt, PartyInfo party, LeaveReason? reason)
	{
		if (!this.m_initialized)
		{
			return;
		}
		if (party.Type != 2)
		{
			return;
		}
		if (evt == 1)
		{
			bool flag = false;
			if (this.m_leavePartyIdsRequested != null)
			{
				flag = this.m_leavePartyIdsRequested.Remove(party.Id);
			}
			this.LogInfoParty("SpectatorParty_OnLeft: left party={0} current={1} reason={2} wasRequested={3}", new object[]
			{
				party,
				this.m_spectatorPartyIdMain,
				(reason == null) ? "null" : reason.Value.ToString(),
				flag
			});
			bool flag2 = false;
			if (party.Id == this.m_spectatorPartyIdOpposingSide)
			{
				this.m_spectatorPartyIdOpposingSide = null;
				flag2 = true;
			}
			else if (this.m_spectateeFriendlySide != null)
			{
				if (party.Id == this.m_spectatorPartyIdMain)
				{
					this.m_spectatorPartyIdMain = null;
					flag2 = true;
				}
			}
			else if (this.m_spectateeFriendlySide == null && this.m_spectateeOpposingSide == null)
			{
				if (party.Id != this.m_spectatorPartyIdMain)
				{
					this.CreatePartyIfNecessary();
					return;
				}
				this.m_userInitiatedOutgoingInvites = null;
				this.m_spectatorPartyIdMain = null;
				this.UpdateSpectatorPresence();
				if (reason != null && reason.Value != null && reason.Value != 2)
				{
					ApplicationMgr.Get().ScheduleCallback(1f, true, delegate(object userData)
					{
						this.CreatePartyIfNecessary();
					}, null);
				}
			}
			if (this.m_pendingSpectatePlayerAfterLeave != null && this.m_spectatorPartyIdMain == null && this.m_spectatorPartyIdOpposingSide == null)
			{
				this.SpectatePlayer_Internal(this.m_pendingSpectatePlayerAfterLeave.SpectateeId, this.m_pendingSpectatePlayerAfterLeave.JoinInfo);
			}
			else if (flag2 && this.m_spectatorPartyIdMain == null)
			{
				if (flag)
				{
					this.EndSpectatorMode(true);
				}
				else
				{
					bool flag3 = reason != null && reason.Value == 1;
					bool flag4 = this.m_expectedDisconnectReason != null && this.m_expectedDisconnectReason.Value == 0;
					this.EndSpectatorMode(true);
					if (flag3 && !flag4)
					{
						if (flag3)
						{
							BnetGameAccountId bnetGameAccountId = this.GetPartyCreator(party.Id);
							if (bnetGameAccountId == null)
							{
								PartyMember leader = BnetParty.GetLeader(party.Id);
								bnetGameAccountId = ((leader != null) ? leader.GameAccountId : null);
							}
							if (bnetGameAccountId != null)
							{
								if (this.m_kickedFromSpectatingList == null)
								{
									this.m_kickedFromSpectatingList = new Map<BnetGameAccountId, float>();
								}
								this.m_kickedFromSpectatingList[bnetGameAccountId] = Time.realtimeSinceStartup;
							}
						}
						if (!this.m_isShowingRemovedAsSpectatorPopup)
						{
							bool flag5 = GameMgr.Get().IsTransitionPopupShown();
							AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
							popupInfo.m_headerText = GameStrings.Get("GLOBAL_SPECTATOR_REMOVED_PROMPT_HEADER");
							popupInfo.m_text = GameStrings.Get("GLOBAL_SPECTATOR_REMOVED_PROMPT_TEXT");
							popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
							if (flag5)
							{
								popupInfo.m_responseCallback = new AlertPopup.ResponseCallback(this.Network_OnSpectatorNotifyEvent_Removed_GoToNextMode);
							}
							else
							{
								popupInfo.m_responseCallback = delegate(AlertPopup.Response r, object data)
								{
									SpectatorManager.Get().m_isShowingRemovedAsSpectatorPopup = false;
								};
							}
							this.m_isShowingRemovedAsSpectatorPopup = true;
							DialogManager.Get().ShowPopup(popupInfo);
						}
					}
				}
			}
			if (ApplicationMgr.Get() != null)
			{
				ApplicationMgr.Get().ScheduleCallback(0.5f, false, new ApplicationMgr.ScheduledCallback(this.BnetParty_OnLostPartyReference_RemoveKnownCreator), party.Id);
			}
		}
		if (evt == null)
		{
			BnetGameAccountId partyCreator = this.GetPartyCreator(party.Id);
			if (partyCreator == null)
			{
				this.LogInfoParty("SpectatorParty_OnJoined: joined party={0} without creator.", new object[]
				{
					party.Id
				});
				this.LeaveParty(party.Id, BnetParty.IsLeader(party.Id));
			}
			else
			{
				if (this.m_requestedInvite != null && this.m_requestedInvite.PartyId == party.Id)
				{
					this.m_requestedInvite = null;
					ApplicationMgr.Get().CancelScheduledCallback(new ApplicationMgr.ScheduledCallback(this.SpectatePlayer_RequestInvite_FriendlySide_Timeout), null);
					ApplicationMgr.Get().CancelScheduledCallback(new ApplicationMgr.ScheduledCallback(this.SpectatePlayer_RequestInvite_OpposingSide_Timeout), null);
				}
				bool flag6 = this.ShouldBePartyLeader(party.Id);
				bool flag7 = this.m_spectatorPartyIdMain == null;
				bool flag8 = flag7;
				if (this.m_spectatorPartyIdMain != null && this.m_spectatorPartyIdMain != party.Id && (flag6 || partyCreator != this.m_spectateeOpposingSide))
				{
					flag8 = true;
					string format = "SpectatorParty_OnJoined: joined party={0} when different current={1} (will be clobbered) joinedParties={2}";
					object[] array = new object[3];
					array[0] = party.Id;
					array[1] = this.m_spectatorPartyIdMain;
					array[2] = string.Join(", ", Enumerable.ToArray<string>(Enumerable.Select<PartyInfo, string>(BnetParty.GetJoinedParties(), (PartyInfo i) => i.ToString())));
					this.LogInfoParty(format, array);
				}
				if (flag6)
				{
					this.m_spectatorPartyIdMain = party.Id;
					if (flag8)
					{
						this.UpdateSpectatorPresence();
					}
					this.UpdateSpectatorPartyServerInfo();
					this.ReinviteKnownSpectatorsNotInParty();
					if (this.m_userInitiatedOutgoingInvites != null)
					{
						foreach (BnetGameAccountId bnetGameAccountId2 in this.m_userInitiatedOutgoingInvites)
						{
							BnetParty.SendInvite(this.m_spectatorPartyIdMain, bnetGameAccountId2);
						}
					}
					if (flag7 && this.OnSpectatorToMyGame != null)
					{
						foreach (PartyMember partyMember in BnetParty.GetMembers(this.m_spectatorPartyIdMain))
						{
							if (!(partyMember.GameAccountId == BnetPresenceMgr.Get().GetMyGameAccountId()))
							{
								ApplicationMgr.Get().StartCoroutine(this.WaitForPresenceThenToast(partyMember.GameAccountId, SocialToastMgr.TOAST_TYPE.SPECTATOR_ADDED));
								BnetPlayer player = BnetUtils.GetPlayer(partyMember.GameAccountId);
								this.OnSpectatorToMyGame(0, player);
							}
						}
					}
				}
				else
				{
					bool flag9 = true;
					if (this.m_spectateeFriendlySide == null)
					{
						this.m_spectateeFriendlySide = partyCreator;
						this.m_spectatorPartyIdMain = party.Id;
						flag9 = false;
					}
					else if (partyCreator == this.m_spectateeFriendlySide)
					{
						this.m_spectatorPartyIdMain = party.Id;
					}
					else if (partyCreator == this.m_spectateeOpposingSide)
					{
						this.m_spectatorPartyIdOpposingSide = party.Id;
					}
					byte[] partyAttributeBlob = BnetParty.GetPartyAttributeBlob(party.Id, "WTCG.Party.ServerInfo");
					if (partyAttributeBlob != null)
					{
						this.LogInfoParty("SpectatorParty_OnJoined: joined party={0} as spectator, begin spectating game.", new object[]
						{
							party.Id
						});
						if (!flag9)
						{
							if (partyCreator == this.m_spectateeOpposingSide)
							{
								this.LogInfoPower("================== Begin Spectating 2nd player ==================", new object[0]);
							}
							else
							{
								this.LogInfoPower("================== Begin Spectating 1st player ==================", new object[0]);
							}
						}
						this.JoinPartyGame(party.Id);
					}
					else
					{
						if (!SceneMgr.Get().IsInGame())
						{
							this.ShowWaitingForNextGameDialog();
						}
						BnetPlayer player2 = BnetUtils.GetPlayer(partyCreator);
						this.FireSpectatorModeChanged(0, player2);
					}
				}
			}
		}
	}

	// Token: 0x060029A9 RID: 10665 RVA: 0x000CB6A4 File Offset: 0x000C98A4
	private void BnetParty_OnLostPartyReference_RemoveKnownCreator(object userData)
	{
		PartyId partyId = userData as PartyId;
		if (partyId != null && !BnetParty.IsInParty(partyId) && !Enumerable.Any<PartyInvite>(BnetParty.GetReceivedInvites(), (PartyInvite i) => i.PartyId == partyId))
		{
			SpectatorManager.Get().m_knownPartyCreatorIds.Remove(partyId);
		}
	}

	// Token: 0x060029AA RID: 10666 RVA: 0x000CB718 File Offset: 0x000C9918
	private void BnetParty_OnReceivedInvite(OnlineEventType evt, PartyInfo party, ulong inviteId, InviteRemoveReason? reason)
	{
		if (!this.m_initialized)
		{
			return;
		}
		if (party.Type != 2)
		{
			return;
		}
		PartyInvite receivedInvite = BnetParty.GetReceivedInvite(inviteId);
		BnetPlayer inviter = (receivedInvite != null) ? BnetUtils.GetPlayer(receivedInvite.InviterId) : null;
		bool flag = true;
		if (evt == null)
		{
			if (receivedInvite == null)
			{
				return;
			}
			if (receivedInvite.IsRejoin || this.ShouldBePartyLeader(receivedInvite.PartyId))
			{
				BnetGameAccountId partyCreator = this.GetPartyCreator(receivedInvite.PartyId);
				this.LogInfoParty("Spectator_OnReceivedInvite rejoin={0} partyId={1} creatorId={2}", new object[]
				{
					receivedInvite.IsRejoin,
					receivedInvite.PartyId,
					partyCreator
				});
				bool flag2 = false;
				if (this.ShouldBePartyLeader(receivedInvite.PartyId))
				{
					BnetParty.AcceptReceivedInvite(inviteId);
					flag = false;
				}
				else if (this.m_spectatorPartyIdMain != null)
				{
					if (this.m_spectatorPartyIdMain == receivedInvite.PartyId)
					{
						BnetParty.AcceptReceivedInvite(inviteId);
						flag = false;
					}
					else
					{
						flag2 = true;
					}
				}
				else
				{
					flag2 = true;
					if (partyCreator != null && this.m_spectateeFriendlySide == null)
					{
						flag2 = false;
						this.m_spectateeFriendlySide = partyCreator;
						BnetParty.AcceptReceivedInvite(inviteId);
						flag = false;
					}
				}
				if (flag2)
				{
					BnetParty.DeclineReceivedInvite(inviteId);
				}
			}
			else if (receivedInvite.InviterId == this.m_spectateeFriendlySide || receivedInvite.InviterId == this.m_spectateeOpposingSide || (this.m_requestedInvite != null && this.m_requestedInvite.PartyId == receivedInvite.PartyId))
			{
				BnetParty.AcceptReceivedInvite(inviteId);
				flag = false;
				if (this.m_requestedInvite != null)
				{
					this.m_requestedInvite = null;
					ApplicationMgr.Get().CancelScheduledCallback(new ApplicationMgr.ScheduledCallback(this.SpectatePlayer_RequestInvite_FriendlySide_Timeout), null);
					ApplicationMgr.Get().CancelScheduledCallback(new ApplicationMgr.ScheduledCallback(this.SpectatePlayer_RequestInvite_OpposingSide_Timeout), null);
				}
			}
			else if (!UserAttentionManager.CanShowAttentionGrabber("SpectatorManager.BnetParty_OnReceivedInvite:" + evt))
			{
				BnetParty.DeclineReceivedInvite(inviteId);
			}
			else
			{
				if (this.m_kickedFromSpectatingList != null)
				{
					this.m_kickedFromSpectatingList.Remove(receivedInvite.InviterId);
				}
				if (SocialToastMgr.Get() != null)
				{
					string inviterBestName = BnetUtils.GetInviterBestName(receivedInvite);
					SocialToastMgr.Get().AddToast(UserAttentionBlocker.NONE, inviterBestName, SocialToastMgr.TOAST_TYPE.SPECTATOR_INVITE_RECEIVED);
				}
			}
		}
		else if (evt == 1 && (reason == null || reason.Value == null) && ApplicationMgr.Get() != null)
		{
			ApplicationMgr.Get().ScheduleCallback(0.5f, false, new ApplicationMgr.ScheduledCallback(this.BnetParty_OnLostPartyReference_RemoveKnownCreator), party.Id);
		}
		if (flag && this.OnInviteReceived != null)
		{
			this.OnInviteReceived(evt, inviter);
		}
	}

	// Token: 0x060029AB RID: 10667 RVA: 0x000CB9E8 File Offset: 0x000C9BE8
	private void BnetParty_OnSentInvite(OnlineEventType evt, PartyInfo party, ulong inviteId, bool senderIsMyself, InviteRemoveReason? reason)
	{
		if (party.Type != 2)
		{
			return;
		}
		if (!senderIsMyself)
		{
			return;
		}
		PartyInvite sentInvite = BnetParty.GetSentInvite(party.Id, inviteId);
		BnetPlayer invitee = (sentInvite != null) ? BnetUtils.GetPlayer(sentInvite.InviteeId) : null;
		if (evt == null)
		{
			bool flag = false;
			if (this.m_userInitiatedOutgoingInvites != null && sentInvite != null)
			{
				flag = this.m_userInitiatedOutgoingInvites.Remove(sentInvite.InviteeId);
			}
			if (flag && sentInvite != null && this.ShouldBePartyLeader(party.Id) && !this.m_gameServerKnownSpectators.Contains(sentInvite.InviteeId) && SocialToastMgr.Get() != null)
			{
				string playerBestName = BnetUtils.GetPlayerBestName(sentInvite.InviteeId);
				SocialToastMgr.Get().AddToast(UserAttentionBlocker.NONE, playerBestName, SocialToastMgr.TOAST_TYPE.SPECTATOR_INVITE_SENT);
			}
		}
		if (sentInvite != null && !this.m_gameServerKnownSpectators.Contains(sentInvite.InviteeId) && this.OnInviteSent != null)
		{
			this.OnInviteSent(evt, invitee);
		}
	}

	// Token: 0x060029AC RID: 10668 RVA: 0x000CBAEC File Offset: 0x000C9CEC
	private void BnetParty_OnReceivedInviteRequest(OnlineEventType evt, PartyInfo party, InviteRequest request, InviteRequestRemovedReason? reason)
	{
		if (party.Type != 2)
		{
			return;
		}
		if (evt == null)
		{
			bool flag = false;
			if (party.Id != this.m_spectatorPartyIdMain)
			{
				flag = true;
			}
			if (request.RequesterId != null && request.RequesterId == request.TargetId && !Options.Get().GetBool(Option.SPECTATOR_OPEN_JOIN))
			{
				flag = true;
			}
			if (!BnetFriendMgr.Get().IsFriend(request.RequesterId))
			{
				flag = true;
			}
			if (!BnetFriendMgr.Get().IsFriend(request.TargetId))
			{
				flag = true;
			}
			if (this.m_kickedPlayers != null && (this.m_kickedPlayers.Contains(request.RequesterId) || this.m_kickedPlayers.Contains(request.TargetId)))
			{
				flag = true;
			}
			if (flag)
			{
				BnetParty.IgnoreInviteRequest(party.Id, request.TargetId);
			}
			else
			{
				BnetParty.AcceptInviteRequest(party.Id, request.TargetId);
			}
		}
	}

	// Token: 0x060029AD RID: 10669 RVA: 0x000CBBF8 File Offset: 0x000C9DF8
	private void BnetParty_OnMemberEvent(OnlineEventType evt, PartyInfo party, BnetGameAccountId memberId, bool isRolesUpdate, LeaveReason? reason)
	{
		if (party.Id == null)
		{
			return;
		}
		if (party.Id != this.m_spectatorPartyIdMain && party.Id != this.m_spectatorPartyIdOpposingSide)
		{
			return;
		}
		if (evt == null && BnetParty.IsLeader(party.Id))
		{
			BnetGameAccountId partyCreator = this.GetPartyCreator(party.Id);
			if (partyCreator != null && partyCreator == memberId)
			{
				BnetParty.SetLeader(party.Id, memberId);
			}
		}
		if (this.m_initialized && evt != 2 && memberId != BnetPresenceMgr.Get().GetMyGameAccountId() && this.ShouldBePartyLeader(party.Id))
		{
			bool flag = !SceneMgr.Get().IsInGame() || !Network.IsConnectedToGameServer() || !this.m_gameServerKnownSpectators.Contains(memberId);
			if (flag)
			{
				SocialToastMgr.TOAST_TYPE toastType = (evt != null) ? SocialToastMgr.TOAST_TYPE.SPECTATOR_REMOVED : SocialToastMgr.TOAST_TYPE.SPECTATOR_ADDED;
				ApplicationMgr.Get().StartCoroutine(this.WaitForPresenceThenToast(memberId, toastType));
				if (this.OnSpectatorToMyGame != null)
				{
					BnetPlayer player = BnetUtils.GetPlayer(memberId);
					this.OnSpectatorToMyGame(evt, player);
				}
			}
		}
	}

	// Token: 0x060029AE RID: 10670 RVA: 0x000CBD37 File Offset: 0x000C9F37
	private void BnetParty_OnChatMessage(PartyInfo party, BnetGameAccountId speakerId, string chatMessage)
	{
	}

	// Token: 0x060029AF RID: 10671 RVA: 0x000CBD3C File Offset: 0x000C9F3C
	private void BnetParty_OnPartyAttributeChanged_ServerInfo(PartyInfo party, string attributeKey, Variant value)
	{
		if (party.Type != 2)
		{
			return;
		}
		if (value == null)
		{
			return;
		}
		byte[] array = (!value.HasBlobValue) ? null : value.BlobValue;
		if (array == null)
		{
			return;
		}
		PartyServerInfo partyServerInfo = ProtobufUtil.ParseFrom<PartyServerInfo>(array, 0, -1);
		if (partyServerInfo == null)
		{
			return;
		}
		if (!partyServerInfo.HasSecretKey || string.IsNullOrEmpty(partyServerInfo.SecretKey))
		{
			this.LogInfoParty("BnetParty_OnPartyAttributeChanged_ServerInfo: no secret key in serverInfo.", new object[0]);
			return;
		}
		GameServerInfo lastGameServerJoined = Network.Get().GetLastGameServerJoined();
		bool flag = Network.IsConnectedToGameServer() && SpectatorManager.IsSameGameAndServer(partyServerInfo, lastGameServerJoined);
		if (!flag && SceneMgr.Get().IsInGame())
		{
			this.LogInfoParty("BnetParty_OnPartyAttributeChanged_ServerInfo: cannot join game while in gameplay new={0} curr={1}.", new object[]
			{
				partyServerInfo.GameHandle,
				lastGameServerJoined.GameHandle
			});
			return;
		}
		JoinInfo joinInfo = SpectatorManager.CreateJoinInfo(partyServerInfo);
		if (party.Id == this.m_spectatorPartyIdOpposingSide)
		{
			if (GameMgr.Get().GetTransitionPopup() == null && GameMgr.Get().IsSpectator())
			{
				this.SpectateSecondPlayer_Network(joinInfo);
			}
		}
		else if (!flag && party.Id == this.m_spectatorPartyIdMain)
		{
			this.LogInfoPower("================== Start Spectator Game ==================", new object[0]);
			this.m_isExpectingArriveInGameplayAsSpectator = true;
			GameMgr.Get().SpectateGame(joinInfo);
			this.CloseWaitingForNextGameDialog();
		}
	}

	// Token: 0x17000371 RID: 881
	// (get) Token: 0x060029B0 RID: 10672 RVA: 0x000CBEB1 File Offset: 0x000CA0B1
	private static bool IsGameOver
	{
		get
		{
			return GameState.Get() != null && GameState.Get().IsGameOverNowOrPending();
		}
	}

	// Token: 0x060029B1 RID: 10673 RVA: 0x000CBED1 File Offset: 0x000CA0D1
	private void LogInfoParty(string format, params object[] args)
	{
		Log.Party.Print(format, args);
	}

	// Token: 0x060029B2 RID: 10674 RVA: 0x000CBEDF File Offset: 0x000CA0DF
	private void LogInfoPower(string format, params object[] args)
	{
		Log.Party.Print(format, args);
		Log.Power.Print(format, args);
	}

	// Token: 0x060029B3 RID: 10675 RVA: 0x000CBEFC File Offset: 0x000CA0FC
	private bool IsPlayerInGame(BnetGameAccountId gameAccountId)
	{
		GameState gameState = GameState.Get();
		if (gameState == null)
		{
			return false;
		}
		foreach (KeyValuePair<int, Player> keyValuePair in gameState.GetPlayerMap())
		{
			BnetPlayer bnetPlayer = keyValuePair.Value.GetBnetPlayer();
			if (bnetPlayer != null)
			{
				BnetGameAccountId hearthstoneGameAccountId = bnetPlayer.GetHearthstoneGameAccountId();
				if (hearthstoneGameAccountId == gameAccountId)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060029B4 RID: 10676 RVA: 0x000CBF98 File Offset: 0x000CA198
	private bool IsStillInParty(PartyId partyId)
	{
		return BnetParty.IsInParty(partyId) && (this.m_leavePartyIdsRequested == null || !this.m_leavePartyIdsRequested.Contains(partyId));
	}

	// Token: 0x060029B5 RID: 10677 RVA: 0x000CBFD4 File Offset: 0x000CA1D4
	private void PruneOldInvites()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		List<BnetGameAccountId> list = new List<BnetGameAccountId>();
		foreach (KeyValuePair<BnetGameAccountId, SpectatorManager.ReceivedInvite> keyValuePair in this.m_receivedSpectateMeInvites)
		{
			float timestamp = keyValuePair.Value.m_timestamp;
			float num = realtimeSinceStartup - timestamp;
			if (num > 300f)
			{
				list.Add(keyValuePair.Key);
			}
		}
		foreach (BnetGameAccountId inviterId in list)
		{
			this.RemoveReceivedInvitation(inviterId);
		}
		list.Clear();
		foreach (KeyValuePair<BnetGameAccountId, float> keyValuePair2 in this.m_sentSpectateMeInvites)
		{
			float value = keyValuePair2.Value;
			float num2 = realtimeSinceStartup - value;
			if (num2 > 30f)
			{
				list.Add(keyValuePair2.Key);
			}
		}
		foreach (BnetGameAccountId inviteeId in list)
		{
			this.RemoveSentInvitation(inviteeId);
		}
	}

	// Token: 0x060029B6 RID: 10678 RVA: 0x000CC168 File Offset: 0x000CA368
	private void AddReceivedInvitation(BnetGameAccountId inviterId, JoinInfo joinInfo)
	{
		bool flag = !this.m_receivedSpectateMeInvites.ContainsKey(inviterId);
		SpectatorManager.ReceivedInvite value = new SpectatorManager.ReceivedInvite(joinInfo);
		this.m_receivedSpectateMeInvites[inviterId] = value;
		if (flag)
		{
			BnetPlayer player = BnetUtils.GetPlayer(inviterId);
			if (SocialToastMgr.Get() != null)
			{
				SocialToastMgr.Get().AddToast(UserAttentionBlocker.NONE, BnetUtils.GetPlayerBestName(inviterId), SocialToastMgr.TOAST_TYPE.SPECTATOR_INVITE_RECEIVED);
			}
			if (this.OnInviteReceived != null)
			{
				this.OnInviteReceived(0, player);
			}
		}
		float num = Enumerable.Min<KeyValuePair<BnetGameAccountId, SpectatorManager.ReceivedInvite>>(this.m_receivedSpectateMeInvites, (KeyValuePair<BnetGameAccountId, SpectatorManager.ReceivedInvite> kv) => kv.Value.m_timestamp);
		float secondsToWait = Mathf.Max(0f, num + 300f - Time.realtimeSinceStartup);
		ApplicationMgr.Get().CancelScheduledCallback(new ApplicationMgr.ScheduledCallback(this.ReceivedInvitation_ExpireTimeout), null);
		ApplicationMgr.Get().ScheduleCallback(secondsToWait, true, new ApplicationMgr.ScheduledCallback(this.ReceivedInvitation_ExpireTimeout), null);
	}

	// Token: 0x060029B7 RID: 10679 RVA: 0x000CC258 File Offset: 0x000CA458
	private void RemoveReceivedInvitation(BnetGameAccountId inviterId)
	{
		if (inviterId == null)
		{
			return;
		}
		if (this.m_receivedSpectateMeInvites.Remove(inviterId))
		{
			BnetPlayer player = BnetUtils.GetPlayer(inviterId);
			if (this.OnInviteReceived != null)
			{
				this.OnInviteReceived(1, player);
			}
		}
	}

	// Token: 0x060029B8 RID: 10680 RVA: 0x000CC2A4 File Offset: 0x000CA4A4
	private void ClearAllReceivedInvitations()
	{
		BnetGameAccountId[] array = Enumerable.ToArray<BnetGameAccountId>(this.m_receivedSpectateMeInvites.Keys);
		this.m_receivedSpectateMeInvites.Clear();
		if (this.OnInviteReceived != null)
		{
			foreach (BnetGameAccountId id in array)
			{
				BnetPlayer player = BnetUtils.GetPlayer(id);
				this.OnInviteReceived(1, player);
			}
		}
	}

	// Token: 0x060029B9 RID: 10681 RVA: 0x000CC308 File Offset: 0x000CA508
	private void AddSentInvitation(BnetGameAccountId inviteeId)
	{
		if (inviteeId == null)
		{
			return;
		}
		bool flag = !this.m_sentSpectateMeInvites.ContainsKey(inviteeId);
		this.m_sentSpectateMeInvites[inviteeId] = Time.realtimeSinceStartup;
		if (flag)
		{
			BnetPlayer player = BnetUtils.GetPlayer(inviteeId);
			if (this.OnInviteSent != null)
			{
				this.OnInviteSent(0, player);
			}
		}
	}

	// Token: 0x060029BA RID: 10682 RVA: 0x000CC368 File Offset: 0x000CA568
	private void RemoveSentInvitation(BnetGameAccountId inviteeId)
	{
		if (inviteeId == null)
		{
			return;
		}
		if (this.m_sentSpectateMeInvites.Remove(inviteeId))
		{
			BnetPlayer player = BnetUtils.GetPlayer(inviteeId);
			if (this.OnInviteSent != null)
			{
				this.OnInviteSent(1, player);
			}
		}
	}

	// Token: 0x060029BB RID: 10683 RVA: 0x000CC3B4 File Offset: 0x000CA5B4
	private void DeclineAllReceivedInvitations()
	{
		PartyInvite[] receivedInvites = BnetParty.GetReceivedInvites();
		foreach (PartyInvite partyInvite in receivedInvites)
		{
			if (partyInvite.PartyType == 2)
			{
				BnetParty.DeclineReceivedInvite(partyInvite.InviteId);
			}
		}
	}

	// Token: 0x060029BC RID: 10684 RVA: 0x000CC400 File Offset: 0x000CA600
	private void RevokeAllSentInvitations()
	{
		this.ClearAllSentInvitations();
		BnetGameAccountId myGameAccountId = BnetPresenceMgr.Get().GetMyGameAccountId();
		foreach (PartyId partyId in new PartyId[]
		{
			this.m_spectatorPartyIdMain,
			this.m_spectatorPartyIdOpposingSide
		})
		{
			if (!(partyId == null))
			{
				PartyInvite[] sentInvites = BnetParty.GetSentInvites(partyId);
				foreach (PartyInvite partyInvite in sentInvites)
				{
					if (!(partyInvite.InviterId != myGameAccountId))
					{
						BnetParty.RevokeSentInvite(partyId, partyInvite.InviteId);
					}
				}
			}
		}
	}

	// Token: 0x060029BD RID: 10685 RVA: 0x000CC4B0 File Offset: 0x000CA6B0
	private void ClearAllSentInvitations()
	{
		BnetGameAccountId[] array = Enumerable.ToArray<BnetGameAccountId>(this.m_sentSpectateMeInvites.Keys);
		this.m_sentSpectateMeInvites.Clear();
		if (this.OnInviteSent != null)
		{
			foreach (BnetGameAccountId id in array)
			{
				BnetPlayer player = BnetUtils.GetPlayer(id);
				this.OnInviteSent(1, player);
			}
		}
	}

	// Token: 0x060029BE RID: 10686 RVA: 0x000CC514 File Offset: 0x000CA714
	private void AddKnownSpectator(BnetGameAccountId gameAccountId)
	{
		if (gameAccountId == null)
		{
			return;
		}
		bool flag = this.m_gameServerKnownSpectators.Add(gameAccountId);
		this.CreatePartyIfNecessary();
		this.RemoveSentInvitation(gameAccountId);
		this.RemoveReceivedInvitation(gameAccountId);
		if (flag)
		{
			if (SceneMgr.Get().IsInGame() && Network.IsConnectedToGameServer())
			{
				bool flag2 = BnetParty.IsMember(this.m_spectatorPartyIdMain, gameAccountId);
				BnetPlayer player = BnetUtils.GetPlayer(gameAccountId);
				if (!flag2)
				{
					ApplicationMgr.Get().StartCoroutine(this.WaitForPresenceThenToast(gameAccountId, SocialToastMgr.TOAST_TYPE.SPECTATOR_ADDED));
				}
				if (this.OnSpectatorToMyGame != null)
				{
					this.OnSpectatorToMyGame(0, player);
				}
			}
			this.UpdateSpectatorPresence();
		}
	}

	// Token: 0x060029BF RID: 10687 RVA: 0x000CC5BC File Offset: 0x000CA7BC
	private void RemoveKnownSpectator(BnetGameAccountId gameAccountId)
	{
		if (gameAccountId == null)
		{
			return;
		}
		bool flag = this.m_gameServerKnownSpectators.Remove(gameAccountId);
		if (flag)
		{
			if (SceneMgr.Get().IsInGame() && Network.IsConnectedToGameServer())
			{
				bool flag2 = BnetParty.IsMember(this.m_spectatorPartyIdMain, gameAccountId);
				BnetPlayer player = BnetUtils.GetPlayer(gameAccountId);
				if (!flag2)
				{
					ApplicationMgr.Get().StartCoroutine(this.WaitForPresenceThenToast(gameAccountId, SocialToastMgr.TOAST_TYPE.SPECTATOR_REMOVED));
				}
				if (this.OnSpectatorToMyGame != null)
				{
					this.OnSpectatorToMyGame(1, player);
				}
			}
			this.UpdateSpectatorPresence();
		}
	}

	// Token: 0x060029C0 RID: 10688 RVA: 0x000CC650 File Offset: 0x000CA850
	private void ClearAllGameServerKnownSpectators()
	{
		BnetGameAccountId[] array = Enumerable.ToArray<BnetGameAccountId>(this.m_gameServerKnownSpectators);
		this.m_gameServerKnownSpectators.Clear();
		if (this.OnSpectatorToMyGame != null && SceneMgr.Get().IsInGame() && Network.IsConnectedToGameServer())
		{
			foreach (BnetGameAccountId id in array)
			{
				BnetPlayer player = BnetUtils.GetPlayer(id);
				this.OnSpectatorToMyGame(1, player);
			}
		}
		if (array.Length > 0)
		{
			this.UpdateSpectatorPresence();
		}
	}

	// Token: 0x060029C1 RID: 10689 RVA: 0x000CC6D8 File Offset: 0x000CA8D8
	private void UpdateSpectatorPresence()
	{
		if (ApplicationMgr.Get() != null)
		{
			ApplicationMgr.Get().CancelScheduledCallback(new ApplicationMgr.ScheduledCallback(this.SpectatorManager_UpdatePresenceNextFrame), null);
			ApplicationMgr.Get().ScheduleCallback(0f, true, new ApplicationMgr.ScheduledCallback(this.SpectatorManager_UpdatePresenceNextFrame), null);
		}
		else
		{
			this.SpectatorManager_UpdatePresenceNextFrame(null);
		}
	}

	// Token: 0x060029C2 RID: 10690 RVA: 0x000CC738 File Offset: 0x000CA938
	private void SpectatorManager_UpdatePresenceNextFrame(object userData)
	{
		JoinInfo protoMessage = null;
		bool flag = Options.Get().GetBool(Option.SPECTATOR_OPEN_JOIN) || this.IsInSpectatorMode();
		if (flag)
		{
			protoMessage = this.GetMyGameJoinInfo();
		}
		if (Network.ShouldBeConnectedToAurora())
		{
			BnetPresenceMgr.Get().SetGameFieldBlob(21U, protoMessage);
		}
	}

	// Token: 0x060029C3 RID: 10691 RVA: 0x000CC78C File Offset: 0x000CA98C
	private void UpdateSpectatorPartyServerInfo()
	{
		if (this.m_spectatorPartyIdMain == null)
		{
			return;
		}
		if (!this.ShouldBePartyLeader(this.m_spectatorPartyIdMain))
		{
			if (BnetParty.IsLeader(this.m_spectatorPartyIdMain))
			{
				BnetParty.ClearPartyAttribute(this.m_spectatorPartyIdMain, "WTCG.Party.ServerInfo");
			}
			return;
		}
		byte[] partyAttributeBlob = BnetParty.GetPartyAttributeBlob(this.m_spectatorPartyIdMain, "WTCG.Party.ServerInfo");
		GameServerInfo lastGameServerJoined = Network.Get().GetLastGameServerJoined();
		if (SpectatorManager.IsGameOver || !SceneMgr.Get().IsInGame() || !Network.IsConnectedToGameServer() || lastGameServerJoined == null || string.IsNullOrEmpty(lastGameServerJoined.Address))
		{
			if (partyAttributeBlob != null)
			{
				BnetParty.ClearPartyAttribute(this.m_spectatorPartyIdMain, "WTCG.Party.ServerInfo");
			}
		}
		else
		{
			byte[] array = ProtobufUtil.ToByteArray(new PartyServerInfo
			{
				ServerIpAddress = lastGameServerJoined.Address,
				ServerPort = (uint)lastGameServerJoined.Port,
				GameHandle = lastGameServerJoined.GameHandle,
				SecretKey = (lastGameServerJoined.SpectatorPassword ?? string.Empty),
				GameType = GameMgr.Get().GetGameType(),
				MissionId = GameMgr.Get().GetMissionId()
			});
			if (!GeneralUtils.AreArraysEqual<byte>(array, partyAttributeBlob))
			{
				BnetParty.SetPartyAttributeBlob(this.m_spectatorPartyIdMain, "WTCG.Party.ServerInfo", array);
			}
		}
	}

	// Token: 0x060029C4 RID: 10692 RVA: 0x000CC8D4 File Offset: 0x000CAAD4
	private bool ShouldBePartyLeader(PartyId partyId)
	{
		if (GameMgr.Get().IsSpectator())
		{
			return false;
		}
		if (this.m_spectateeFriendlySide != null || this.m_spectateeOpposingSide != null)
		{
			return false;
		}
		BnetGameAccountId partyCreator = this.GetPartyCreator(partyId);
		return !(partyCreator == null) && !(partyCreator != BnetPresenceMgr.Get().GetMyGameAccountId());
	}

	// Token: 0x060029C5 RID: 10693 RVA: 0x000CC944 File Offset: 0x000CAB44
	private BnetGameAccountId GetPartyCreator(PartyId partyId)
	{
		if (partyId == null)
		{
			return null;
		}
		BnetGameAccountId bnetGameAccountId = null;
		if (this.m_knownPartyCreatorIds.TryGetValue(partyId, out bnetGameAccountId) && bnetGameAccountId != null)
		{
			return bnetGameAccountId;
		}
		byte[] partyAttributeBlob = BnetParty.GetPartyAttributeBlob(partyId, "WTCG.Party.Creator");
		if (partyAttributeBlob == null)
		{
			return null;
		}
		BnetId src = ProtobufUtil.ParseFrom<BnetId>(partyAttributeBlob, 0, -1);
		bnetGameAccountId = BnetUtils.CreateGameAccountId(src);
		if (bnetGameAccountId.IsValid())
		{
			this.m_knownPartyCreatorIds[partyId] = bnetGameAccountId;
		}
		return bnetGameAccountId;
	}

	// Token: 0x060029C6 RID: 10694 RVA: 0x000CC9C0 File Offset: 0x000CABC0
	private bool CreatePartyIfNecessary()
	{
		if (this.m_spectatorPartyIdMain != null)
		{
			if (this.GetPartyCreator(this.m_spectatorPartyIdMain) != null && !this.ShouldBePartyLeader(this.m_spectatorPartyIdMain))
			{
				return false;
			}
			PartyInfo[] joinedParties = BnetParty.GetJoinedParties();
			if (Enumerable.FirstOrDefault<PartyInfo>(joinedParties, (PartyInfo i) => i.Id == this.m_spectatorPartyIdMain && i.Type == 2) == null)
			{
				string format = "CreatePartyIfNecessary stored PartyId={0} is not in joined party list: {1}";
				object[] array = new object[2];
				array[0] = this.m_spectatorPartyIdMain;
				array[1] = string.Join(", ", Enumerable.ToArray<string>(Enumerable.Select<PartyInfo, string>(joinedParties, (PartyInfo i) => i.ToString())));
				this.LogInfoParty(format, array);
				this.m_spectatorPartyIdMain = null;
				this.UpdateSpectatorPresence();
			}
			PartyInfo partyInfo = Enumerable.FirstOrDefault<PartyInfo>(joinedParties, (PartyInfo i) => i.Type == 2);
			if (partyInfo != null && this.m_spectatorPartyIdMain != partyInfo.Id)
			{
				this.LogInfoParty("CreatePartyIfNecessary repairing mismatching PartyIds current={0} new={1}", new object[]
				{
					this.m_spectatorPartyIdMain,
					partyInfo.Id
				});
				this.m_spectatorPartyIdMain = partyInfo.Id;
				this.UpdateSpectatorPresence();
			}
			if (this.m_spectatorPartyIdMain != null)
			{
				return false;
			}
		}
		if (this.GetCountSpectatingMe() <= 0)
		{
			return false;
		}
		BnetGameAccountId myGameAccountId = BnetPresenceMgr.Get().GetMyGameAccountId();
		BnetId bnetId = BnetUtils.CreatePegasusBnetId(myGameAccountId);
		byte[] array2 = ProtobufUtil.ToByteArray(bnetId);
		BnetParty.CreateParty(2, 3, array2, null);
		return true;
	}

	// Token: 0x060029C7 RID: 10695 RVA: 0x000CCB40 File Offset: 0x000CAD40
	private void ReinviteKnownSpectatorsNotInParty()
	{
		if (this.m_spectatorPartyIdMain == null || !this.ShouldBePartyLeader(this.m_spectatorPartyIdMain))
		{
			return;
		}
		PartyMember[] members = BnetParty.GetMembers(this.m_spectatorPartyIdMain);
		BnetGameAccountId knownSpectator;
		foreach (BnetGameAccountId knownSpectator2 in this.m_gameServerKnownSpectators)
		{
			knownSpectator = knownSpectator2;
			PartyMember partyMember = Enumerable.FirstOrDefault<PartyMember>(members, (PartyMember m) => m.GameAccountId == knownSpectator);
			if (partyMember == null)
			{
				BnetParty.SendInvite(this.m_spectatorPartyIdMain, knownSpectator);
			}
		}
	}

	// Token: 0x060029C8 RID: 10696 RVA: 0x000CCBFC File Offset: 0x000CADFC
	private void LeaveParty(PartyId partyId, bool dissolve)
	{
		if (partyId == null)
		{
			return;
		}
		if (this.m_leavePartyIdsRequested == null)
		{
			this.m_leavePartyIdsRequested = new HashSet<PartyId>();
		}
		this.m_leavePartyIdsRequested.Add(partyId);
		if (dissolve)
		{
			BnetParty.DissolveParty(partyId);
		}
		else
		{
			BnetParty.Leave(partyId);
		}
	}

	// Token: 0x060029C9 RID: 10697 RVA: 0x000CCC50 File Offset: 0x000CAE50
	private void LeaveGameScene()
	{
		if (EndGameScreen.Get() != null)
		{
			EndGameScreen.Get().m_hitbox.TriggerPress();
			EndGameScreen.Get().m_hitbox.TriggerRelease();
		}
		else
		{
			SceneMgr.Mode postGameSceneMode = GameMgr.Get().GetPostGameSceneMode();
			SceneMgr.Get().SetNextMode(postGameSceneMode);
		}
	}

	// Token: 0x060029CA RID: 10698 RVA: 0x000CCCA8 File Offset: 0x000CAEA8
	private IEnumerator WaitForPresenceThenToast(BnetGameAccountId gameAccountId, SocialToastMgr.TOAST_TYPE toastType)
	{
		float timeStarted = Time.time;
		float timeElapsed = Time.time - timeStarted;
		while (timeElapsed < 30f && !BnetUtils.HasPlayerBestNamePresence(gameAccountId))
		{
			yield return null;
			timeElapsed = Time.time - timeStarted;
		}
		if (SocialToastMgr.Get() != null)
		{
			string playerName = BnetUtils.GetPlayerBestName(gameAccountId);
			SocialToastMgr.Get().AddToast(UserAttentionBlocker.NONE, playerName, toastType);
		}
		yield break;
	}

	// Token: 0x060029CB RID: 10699 RVA: 0x000CCCD8 File Offset: 0x000CAED8
	private static SpectatorManager CreateInstance()
	{
		SpectatorManager.s_instance = new SpectatorManager();
		ApplicationMgr.Get().WillReset += new Action(SpectatorManager.s_instance.WillReset);
		GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(SpectatorManager.s_instance.OnFindGameEvent));
		SceneMgr.Get().RegisterSceneUnloadedEvent(new SceneMgr.SceneUnloadedCallback(SpectatorManager.s_instance.OnSceneUnloaded));
		GameState.RegisterGameStateInitializedListener(new GameState.GameStateInitializedCallback(SpectatorManager.s_instance.GameState_InitializedEvent), null);
		Network.Get().SetSpectatorInviteReceivedHandler(new Network.SpectatorInviteReceivedHandler(SpectatorManager.s_instance.Network_OnSpectatorInviteReceived));
		Options.Get().RegisterChangedListener(Option.SPECTATOR_OPEN_JOIN, new Options.ChangedCallback(SpectatorManager.s_instance.OnSpectatorOpenJoinOptionChanged));
		BnetPresenceMgr.Get().OnGameAccountPresenceChange += new Action<PresenceUpdate[]>(SpectatorManager.s_instance.Presence_OnGameAccountPresenceChange);
		BnetFriendMgr.Get().AddChangeListener(new BnetFriendMgr.ChangeCallback(SpectatorManager.s_instance.BnetFriendMgr_OnFriendsChanged));
		EndGameScreen.OnTwoScoopsShown = (EndGameScreen.OnTwoScoopsShownHandler)Delegate.Combine(EndGameScreen.OnTwoScoopsShown, new EndGameScreen.OnTwoScoopsShownHandler(SpectatorManager.s_instance.EndGameScreen_OnTwoScoopsShown));
		Network.Get().RegisterNetHandler(24, new Network.NetHandler(SpectatorManager.s_instance.Network_OnSpectatorNotifyEvent), null);
		BnetParty.OnError += new BnetParty.PartyErrorHandler(SpectatorManager.s_instance.BnetParty_OnError);
		BnetParty.OnJoined += new BnetParty.JoinedHandler(SpectatorManager.s_instance.BnetParty_OnJoined);
		BnetParty.OnReceivedInvite += new BnetParty.ReceivedInviteHandler(SpectatorManager.s_instance.BnetParty_OnReceivedInvite);
		BnetParty.OnSentInvite += new BnetParty.SentInviteHandler(SpectatorManager.s_instance.BnetParty_OnSentInvite);
		BnetParty.OnReceivedInviteRequest += new BnetParty.ReceivedInviteRequestHandler(SpectatorManager.s_instance.BnetParty_OnReceivedInviteRequest);
		BnetParty.OnMemberEvent += new BnetParty.MemberEventHandler(SpectatorManager.s_instance.BnetParty_OnMemberEvent);
		BnetParty.OnChatMessage += new BnetParty.ChatMessageHandler(SpectatorManager.s_instance.BnetParty_OnChatMessage);
		BnetParty.RegisterAttributeChangedHandler("WTCG.Party.ServerInfo", new BnetParty.PartyAttributeChangedHandler(SpectatorManager.s_instance.BnetParty_OnPartyAttributeChanged_ServerInfo));
		return SpectatorManager.s_instance;
	}

	// Token: 0x04001833 RID: 6195
	public const int MAX_SPECTATORS_PER_SIDE = 10;

	// Token: 0x04001834 RID: 6196
	private const float RECEIVED_INVITE_TIMEOUT_SECONDS = 300f;

	// Token: 0x04001835 RID: 6197
	private const float SENT_INVITE_TIMEOUT_SECONDS = 30f;

	// Token: 0x04001836 RID: 6198
	private const float REQUEST_INVITE_TIMEOUT_SECONDS = 5f;

	// Token: 0x04001837 RID: 6199
	private const string ALERTPOPUPID_WAITINGFORNEXTGAME = "SPECTATOR_WAITING_FOR_NEXT_GAME";

	// Token: 0x04001838 RID: 6200
	private const float ENDGAMESCREEN_AUTO_CLOSE_SECONDS = 5f;

	// Token: 0x04001839 RID: 6201
	private const float KICKED_FROM_SPECTATING_BLACKOUT_DURATION_SECONDS = 10f;

	// Token: 0x0400183A RID: 6202
	private static readonly PlatformDependentValue<float> WAITING_FOR_NEXT_GAME_AUTO_LEAVE_SECONDS = new PlatformDependentValue<float>(PlatformCategory.OS)
	{
		iOS = 300f,
		Android = 300f,
		PC = -1f,
		Mac = -1f
	};

	// Token: 0x0400183B RID: 6203
	private static readonly PlatformDependentValue<bool> DISABLE_MENU_BUTTON_WHILE_WAITING = new PlatformDependentValue<bool>(PlatformCategory.OS)
	{
		iOS = true,
		Android = true,
		PC = false,
		Mac = false
	};

	// Token: 0x0400183C RID: 6204
	private static SpectatorManager s_instance = null;

	// Token: 0x0400183D RID: 6205
	private bool m_initialized;

	// Token: 0x0400183E RID: 6206
	private BnetGameAccountId m_spectateeFriendlySide;

	// Token: 0x0400183F RID: 6207
	private BnetGameAccountId m_spectateeOpposingSide;

	// Token: 0x04001840 RID: 6208
	private PartyId m_spectatorPartyIdMain;

	// Token: 0x04001841 RID: 6209
	private PartyId m_spectatorPartyIdOpposingSide;

	// Token: 0x04001842 RID: 6210
	private Map<PartyId, BnetGameAccountId> m_knownPartyCreatorIds = new Map<PartyId, BnetGameAccountId>();

	// Token: 0x04001843 RID: 6211
	private SpectatorManager.IntendedSpectateeParty m_requestedInvite;

	// Token: 0x04001844 RID: 6212
	private AlertPopup m_waitingForNextGameDialog;

	// Token: 0x04001845 RID: 6213
	private HashSet<PartyId> m_leavePartyIdsRequested;

	// Token: 0x04001846 RID: 6214
	private SpectatorManager.PendingSpectatePlayer m_pendingSpectatePlayerAfterLeave;

	// Token: 0x04001847 RID: 6215
	private HashSet<BnetGameAccountId> m_userInitiatedOutgoingInvites;

	// Token: 0x04001848 RID: 6216
	private HashSet<BnetGameAccountId> m_kickedPlayers;

	// Token: 0x04001849 RID: 6217
	private Map<BnetGameAccountId, float> m_kickedFromSpectatingList;

	// Token: 0x0400184A RID: 6218
	private int? m_expectedDisconnectReason;

	// Token: 0x0400184B RID: 6219
	private bool m_isExpectingArriveInGameplayAsSpectator;

	// Token: 0x0400184C RID: 6220
	private bool m_isShowingRemovedAsSpectatorPopup;

	// Token: 0x0400184D RID: 6221
	private HashSet<BnetGameAccountId> m_gameServerKnownSpectators = new HashSet<BnetGameAccountId>();

	// Token: 0x0400184E RID: 6222
	private Map<BnetGameAccountId, SpectatorManager.ReceivedInvite> m_receivedSpectateMeInvites = new Map<BnetGameAccountId, SpectatorManager.ReceivedInvite>();

	// Token: 0x0400184F RID: 6223
	private Map<BnetGameAccountId, float> m_sentSpectateMeInvites = new Map<BnetGameAccountId, float>();

	// Token: 0x020004E2 RID: 1250
	// (Invoke) Token: 0x06003AC6 RID: 15046
	public delegate void InviteReceivedHandler(OnlineEventType evt, BnetPlayer inviter);

	// Token: 0x020004E3 RID: 1251
	// (Invoke) Token: 0x06003ACA RID: 15050
	public delegate void SpectatorToMyGameHandler(OnlineEventType evt, BnetPlayer spectator);

	// Token: 0x020004E4 RID: 1252
	// (Invoke) Token: 0x06003ACE RID: 15054
	public delegate void SpectatorModeChangedHandler(OnlineEventType evt, BnetPlayer spectatee);

	// Token: 0x020004EE RID: 1262
	// (Invoke) Token: 0x06003B47 RID: 15175
	public delegate void InviteSentHandler(OnlineEventType evt, BnetPlayer invitee);

	// Token: 0x020004EF RID: 1263
	private struct ReceivedInvite
	{
		// Token: 0x06003B4A RID: 15178 RVA: 0x0011F6BB File Offset: 0x0011D8BB
		public ReceivedInvite(JoinInfo joinInfo)
		{
			this.m_timestamp = Time.realtimeSinceStartup;
			this.m_joinInfo = joinInfo;
		}

		// Token: 0x040025DE RID: 9694
		public float m_timestamp;

		// Token: 0x040025DF RID: 9695
		public JoinInfo m_joinInfo;
	}

	// Token: 0x020004F0 RID: 1264
	private class IntendedSpectateeParty
	{
		// Token: 0x06003B4B RID: 15179 RVA: 0x0011F6CF File Offset: 0x0011D8CF
		public IntendedSpectateeParty(BnetGameAccountId spectateeId, PartyId partyId)
		{
			this.SpectateeId = spectateeId;
			this.PartyId = partyId;
		}

		// Token: 0x040025E0 RID: 9696
		public BnetGameAccountId SpectateeId;

		// Token: 0x040025E1 RID: 9697
		public PartyId PartyId;
	}

	// Token: 0x020004F1 RID: 1265
	private class PendingSpectatePlayer
	{
		// Token: 0x06003B4C RID: 15180 RVA: 0x0011F6E5 File Offset: 0x0011D8E5
		public PendingSpectatePlayer(BnetGameAccountId spectateeId, JoinInfo joinInfo)
		{
			this.SpectateeId = spectateeId;
			this.JoinInfo = joinInfo;
		}

		// Token: 0x040025E2 RID: 9698
		public BnetGameAccountId SpectateeId;

		// Token: 0x040025E3 RID: 9699
		public JoinInfo JoinInfo;
	}
}
