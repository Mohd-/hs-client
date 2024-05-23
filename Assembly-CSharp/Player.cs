using System;
using System.Collections.Generic;
using bgs;
using bgs.types;
using UnityEngine;

// Token: 0x02000195 RID: 405
public class Player : Entity
{
	// Token: 0x060019FE RID: 6654 RVA: 0x00079B40 File Offset: 0x00077D40
	public void InitPlayer(Network.HistCreateGame.PlayerData netPlayer)
	{
		this.SetPlayerId(netPlayer.ID);
		this.SetGameAccountId(netPlayer.GameAccountId);
		this.SetCardBackId(netPlayer.CardBackID);
		base.SetTags(netPlayer.Player.Tags);
		GameState.Get().RegisterTurnChangedListener(new GameState.TurnChangedCallback(this.OnTurnChanged));
		if (this.IsFriendlySide())
		{
			GameState.Get().RegisterOptionsReceivedListener(new GameState.OptionsReceivedCallback(this.OnFriendlyOptionsReceived));
			GameState.Get().RegisterOptionsSentListener(new GameState.OptionsSentCallback(this.OnFriendlyOptionsSent), null);
			GameState.Get().RegisterFriendlyTurnStartedListener(new GameState.FriendlyTurnStartedCallback(this.OnFriendlyTurnStarted), null);
		}
	}

	// Token: 0x060019FF RID: 6655 RVA: 0x00079BEB File Offset: 0x00077DEB
	public override string GetName()
	{
		return this.m_name;
	}

	// Token: 0x06001A00 RID: 6656 RVA: 0x00079BF3 File Offset: 0x00077DF3
	public MedalInfoTranslator GetRank()
	{
		return this.m_medalInfo;
	}

	// Token: 0x06001A01 RID: 6657 RVA: 0x00079BFB File Offset: 0x00077DFB
	public override string GetDebugName()
	{
		if (this.m_name != null)
		{
			return this.m_name;
		}
		if (this.IsAI())
		{
			return GameStrings.Get("GAMEPLAY_AI_OPPONENT_NAME");
		}
		return "UNKNOWN HUMAN PLAYER";
	}

	// Token: 0x06001A02 RID: 6658 RVA: 0x00079C2A File Offset: 0x00077E2A
	public void SetName(string name)
	{
		this.m_name = name;
	}

	// Token: 0x06001A03 RID: 6659 RVA: 0x00079C34 File Offset: 0x00077E34
	public void SetGameAccountId(BnetGameAccountId id)
	{
		this.m_gameAccountId = id;
		this.UpdateLocal();
		this.UpdateSide();
		if (this.IsDisplayable())
		{
			this.UpdateDisplayInfo();
		}
		else if (this.IsBnetPlayer())
		{
			BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnBnetPlayersChanged));
			if (!BnetFriendMgr.Get().IsFriend(this.m_gameAccountId) && (GameMgr.Get().IsSpectator() || GameMgr.Get().GetReconnectType() == ReconnectType.LOGIN))
			{
				this.RequestPlayerPresence();
			}
		}
	}

	// Token: 0x06001A04 RID: 6660 RVA: 0x00079CC8 File Offset: 0x00077EC8
	private void RequestPlayerPresence()
	{
		EntityId entityId = default(EntityId);
		entityId.hi = this.m_gameAccountId.GetHi();
		entityId.lo = this.m_gameAccountId.GetLo();
		List<PresenceFieldKey> list = new List<PresenceFieldKey>();
		PresenceFieldKey presenceFieldKey = default(PresenceFieldKey);
		presenceFieldKey.programId = BnetProgramId.BNET.GetValue();
		presenceFieldKey.groupId = 2U;
		presenceFieldKey.fieldId = 7U;
		presenceFieldKey.index = 0UL;
		list.Add(presenceFieldKey);
		presenceFieldKey.programId = BnetProgramId.BNET.GetValue();
		presenceFieldKey.groupId = 2U;
		presenceFieldKey.fieldId = 3U;
		presenceFieldKey.index = 0UL;
		list.Add(presenceFieldKey);
		presenceFieldKey.programId = BnetProgramId.BNET.GetValue();
		presenceFieldKey.groupId = 2U;
		presenceFieldKey.fieldId = 5U;
		presenceFieldKey.index = 0UL;
		list.Add(presenceFieldKey);
		if (GameUtils.ShouldShowRankedMedals())
		{
			PresenceFieldKey presenceFieldKey2 = default(PresenceFieldKey);
			presenceFieldKey2.programId = BnetProgramId.HEARTHSTONE.GetValue();
			presenceFieldKey2.groupId = 2U;
			presenceFieldKey2.fieldId = 18U;
			presenceFieldKey2.index = 0UL;
			list.Add(presenceFieldKey2);
		}
		PresenceFieldKey[] array = list.ToArray();
		BattleNet.RequestPresenceFields(true, entityId, array);
	}

	// Token: 0x06001A05 RID: 6661 RVA: 0x00079DF7 File Offset: 0x00077FF7
	public bool IsLocalUser()
	{
		return this.m_local;
	}

	// Token: 0x06001A06 RID: 6662 RVA: 0x00079DFF File Offset: 0x00077FFF
	public void SetLocalUser(bool local)
	{
		this.m_local = local;
		this.UpdateSide();
	}

	// Token: 0x06001A07 RID: 6663 RVA: 0x00079E0E File Offset: 0x0007800E
	public bool IsAI()
	{
		return !(this.m_gameAccountId == null) && !this.m_gameAccountId.IsValid();
	}

	// Token: 0x06001A08 RID: 6664 RVA: 0x00079E31 File Offset: 0x00078031
	public bool IsHuman()
	{
		return !(this.m_gameAccountId == null) && this.m_gameAccountId.IsValid();
	}

	// Token: 0x06001A09 RID: 6665 RVA: 0x00079E51 File Offset: 0x00078051
	public bool IsBnetPlayer()
	{
		return this.IsHuman() && Network.ShouldBeConnectedToAurora();
	}

	// Token: 0x06001A0A RID: 6666 RVA: 0x00079E65 File Offset: 0x00078065
	public bool IsGuestPlayer()
	{
		return this.IsHuman() && !Network.ShouldBeConnectedToAurora();
	}

	// Token: 0x06001A0B RID: 6667 RVA: 0x00079E7C File Offset: 0x0007807C
	public Player.Side GetSide()
	{
		return this.m_side;
	}

	// Token: 0x06001A0C RID: 6668 RVA: 0x00079E84 File Offset: 0x00078084
	public bool IsFriendlySide()
	{
		return this.m_side == Player.Side.FRIENDLY;
	}

	// Token: 0x06001A0D RID: 6669 RVA: 0x00079E8F File Offset: 0x0007808F
	public bool IsOpposingSide()
	{
		return this.m_side == Player.Side.OPPOSING;
	}

	// Token: 0x06001A0E RID: 6670 RVA: 0x00079E9A File Offset: 0x0007809A
	public bool IsRevealed()
	{
		return this.IsFriendlySide() || SpectatorManager.Get().IsSpectatingPlayer(this.m_gameAccountId);
	}

	// Token: 0x06001A0F RID: 6671 RVA: 0x00079EC1 File Offset: 0x000780C1
	public void SetSide(Player.Side side)
	{
		this.m_side = side;
	}

	// Token: 0x06001A10 RID: 6672 RVA: 0x00079ECA File Offset: 0x000780CA
	public int GetCardBackId()
	{
		return this.m_cardBackId;
	}

	// Token: 0x06001A11 RID: 6673 RVA: 0x00079ED2 File Offset: 0x000780D2
	public void SetCardBackId(int id)
	{
		this.m_cardBackId = id;
	}

	// Token: 0x06001A12 RID: 6674 RVA: 0x00079EDB File Offset: 0x000780DB
	public int GetPlayerId()
	{
		return base.GetTag(GAME_TAG.PLAYER_ID);
	}

	// Token: 0x06001A13 RID: 6675 RVA: 0x00079EE5 File Offset: 0x000780E5
	public void SetPlayerId(int playerId)
	{
		base.SetTag(GAME_TAG.PLAYER_ID, playerId);
	}

	// Token: 0x06001A14 RID: 6676 RVA: 0x00079EF0 File Offset: 0x000780F0
	public List<string> GetSecretDefinitions()
	{
		List<string> list = new List<string>();
		foreach (Zone zone in ZoneMgr.Get().GetZones())
		{
			if (zone is ZoneSecret && zone.m_Side == Player.Side.FRIENDLY)
			{
				foreach (Card card in zone.GetCards())
				{
					list.Add(card.GetEntity().GetCardId());
				}
			}
		}
		return list;
	}

	// Token: 0x06001A15 RID: 6677 RVA: 0x00079FC0 File Offset: 0x000781C0
	public bool IsCurrentPlayer()
	{
		return base.HasTag(GAME_TAG.CURRENT_PLAYER);
	}

	// Token: 0x06001A16 RID: 6678 RVA: 0x00079FCA File Offset: 0x000781CA
	public bool IsComboActive()
	{
		return base.HasTag(GAME_TAG.COMBO_ACTIVE);
	}

	// Token: 0x06001A17 RID: 6679 RVA: 0x00079FD7 File Offset: 0x000781D7
	public bool IsRealTimeComboActive()
	{
		return this.m_realTimeComboActive;
	}

	// Token: 0x06001A18 RID: 6680 RVA: 0x00079FDF File Offset: 0x000781DF
	public void SetRealTimeComboActive(int tagValue)
	{
		this.SetRealTimeComboActive(tagValue == 1);
	}

	// Token: 0x06001A19 RID: 6681 RVA: 0x00079FF5 File Offset: 0x000781F5
	public void SetRealTimeComboActive(bool active)
	{
		this.m_realTimeComboActive = active;
	}

	// Token: 0x06001A1A RID: 6682 RVA: 0x0007A000 File Offset: 0x00078200
	public int GetNumAvailableResources()
	{
		int tag = base.GetTag(GAME_TAG.TEMP_RESOURCES);
		int tag2 = base.GetTag(GAME_TAG.RESOURCES);
		int tag3 = base.GetTag(GAME_TAG.RESOURCES_USED);
		int num = tag2 + tag - tag3 - this.m_queuedSpentMana - this.m_usedTempMana;
		return (num >= 0) ? num : 0;
	}

	// Token: 0x06001A1B RID: 6683 RVA: 0x0007A050 File Offset: 0x00078250
	public bool HasWeapon()
	{
		foreach (Zone zone in ZoneMgr.Get().GetZones())
		{
			if (zone is ZoneWeapon)
			{
				if (zone.m_Side == this.m_side)
				{
					return zone.GetCards().Count > 0;
				}
			}
		}
		return false;
	}

	// Token: 0x06001A1C RID: 6684 RVA: 0x0007A0E4 File Offset: 0x000782E4
	public void SetHero(Entity hero)
	{
		this.m_hero = hero;
		if (this.m_startingHero == null && hero != null)
		{
			this.m_startingHero = hero;
		}
		if (this.ShouldUseHeroName())
		{
			this.UpdateDisplayInfo();
		}
		if (this.IsFriendlySide())
		{
			GameState.Get().FireHeroChangedEvent(this);
		}
	}

	// Token: 0x06001A1D RID: 6685 RVA: 0x0007A137 File Offset: 0x00078337
	public Entity GetStartingHero()
	{
		return this.m_startingHero;
	}

	// Token: 0x06001A1E RID: 6686 RVA: 0x0007A13F File Offset: 0x0007833F
	public override Entity GetHero()
	{
		return this.m_hero;
	}

	// Token: 0x06001A1F RID: 6687 RVA: 0x0007A148 File Offset: 0x00078348
	public EntityDef GetHeroEntityDef()
	{
		if (this.m_hero == null)
		{
			return null;
		}
		EntityDef entityDef = this.m_hero.GetEntityDef();
		if (entityDef == null)
		{
			return null;
		}
		return entityDef;
	}

	// Token: 0x06001A20 RID: 6688 RVA: 0x0007A177 File Offset: 0x00078377
	public override Card GetHeroCard()
	{
		if (this.m_hero == null)
		{
			return null;
		}
		return this.m_hero.GetCard();
	}

	// Token: 0x06001A21 RID: 6689 RVA: 0x0007A191 File Offset: 0x00078391
	public void SetHeroPower(Entity heroPower)
	{
		this.m_heroPower = heroPower;
	}

	// Token: 0x06001A22 RID: 6690 RVA: 0x0007A19A File Offset: 0x0007839A
	public override Entity GetHeroPower()
	{
		return this.m_heroPower;
	}

	// Token: 0x06001A23 RID: 6691 RVA: 0x0007A1A2 File Offset: 0x000783A2
	public override Card GetHeroPowerCard()
	{
		if (this.m_heroPower == null)
		{
			return null;
		}
		return this.m_heroPower.GetCard();
	}

	// Token: 0x06001A24 RID: 6692 RVA: 0x0007A1BC File Offset: 0x000783BC
	public bool IsHeroPowerAffectedByBonusDamage()
	{
		Card heroPowerCard = this.GetHeroPowerCard();
		if (heroPowerCard == null)
		{
			return false;
		}
		Entity entity = heroPowerCard.GetEntity();
		return entity.IsHeroPower() && TextUtils.HasBonusDamage(entity.GetStringTag(GAME_TAG.CARDTEXT_INHAND));
	}

	// Token: 0x06001A25 RID: 6693 RVA: 0x0007A204 File Offset: 0x00078404
	public override Card GetWeaponCard()
	{
		ZoneWeapon zoneWeapon = ZoneMgr.Get().FindZoneOfType<ZoneWeapon>(this.GetSide());
		return zoneWeapon.GetFirstCard();
	}

	// Token: 0x06001A26 RID: 6694 RVA: 0x0007A228 File Offset: 0x00078428
	public ZoneHand GetHandZone()
	{
		return ZoneMgr.Get().FindZoneOfType<ZoneHand>(this.GetSide());
	}

	// Token: 0x06001A27 RID: 6695 RVA: 0x0007A23A File Offset: 0x0007843A
	public ZonePlay GetBattlefieldZone()
	{
		return ZoneMgr.Get().FindZoneOfType<ZonePlay>(this.GetSide());
	}

	// Token: 0x06001A28 RID: 6696 RVA: 0x0007A24C File Offset: 0x0007844C
	public ZoneDeck GetDeckZone()
	{
		return ZoneMgr.Get().FindZoneOfType<ZoneDeck>(this.GetSide());
	}

	// Token: 0x06001A29 RID: 6697 RVA: 0x0007A25E File Offset: 0x0007845E
	public ZoneGraveyard GetGraveyardZone()
	{
		return ZoneMgr.Get().FindZoneOfType<ZoneGraveyard>(this.GetSide());
	}

	// Token: 0x06001A2A RID: 6698 RVA: 0x0007A270 File Offset: 0x00078470
	public ZoneSecret GetSecretZone()
	{
		return ZoneMgr.Get().FindZoneOfType<ZoneSecret>(this.GetSide());
	}

	// Token: 0x06001A2B RID: 6699 RVA: 0x0007A284 File Offset: 0x00078484
	public int GetNumMinionsInPlay()
	{
		int num = 0;
		foreach (Card card in this.GetBattlefieldZone().GetCards())
		{
			Entity entity = card.GetEntity();
			if (entity.GetControllerId() == this.GetPlayerId() && entity.IsMinion())
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06001A2C RID: 6700 RVA: 0x0007A308 File Offset: 0x00078508
	public int GetNumDragonsInHand()
	{
		int num = 0;
		foreach (Card card in this.GetHandZone().GetCards())
		{
			Entity entity = card.GetEntity();
			if (entity.GetRace() == TAG_RACE.DRAGON)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06001A2D RID: 6701 RVA: 0x0007A37C File Offset: 0x0007857C
	public bool HasReadyAttackers()
	{
		List<Card> cards = this.GetBattlefieldZone().GetCards();
		for (int i = 0; i < cards.Count; i++)
		{
			if (GameState.Get().HasResponse(cards[i].GetEntity()))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001A2E RID: 6702 RVA: 0x0007A3CC File Offset: 0x000785CC
	public bool HasATauntMinion()
	{
		List<Card> cards = this.GetBattlefieldZone().GetCards();
		for (int i = 0; i < cards.Count; i++)
		{
			if (cards[i].GetEntity().HasTaunt())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001A2F RID: 6703 RVA: 0x0007A418 File Offset: 0x00078618
	private int GetNumTotalMinionsInPlay()
	{
		int num = 0;
		foreach (Zone zone in ZoneMgr.Get().GetZones())
		{
			if (zone is ZonePlay)
			{
				foreach (Card card in zone.GetCards())
				{
					Entity entity = card.GetEntity();
					if (entity.IsMinion())
					{
						num++;
					}
				}
			}
		}
		return num;
	}

	// Token: 0x06001A30 RID: 6704 RVA: 0x0007A4E0 File Offset: 0x000786E0
	public void PlayConcedeEmote()
	{
		if (this.m_concedeEmotePlayed)
		{
			return;
		}
		Card heroCard = this.GetHeroCard();
		if (heroCard == null)
		{
			return;
		}
		heroCard.PlayEmote(EmoteType.CONCEDE);
		this.m_concedeEmotePlayed = true;
	}

	// Token: 0x06001A31 RID: 6705 RVA: 0x0007A51C File Offset: 0x0007871C
	public BnetGameAccountId GetGameAccountId()
	{
		return this.m_gameAccountId;
	}

	// Token: 0x06001A32 RID: 6706 RVA: 0x0007A524 File Offset: 0x00078724
	public BnetPlayer GetBnetPlayer()
	{
		return BnetPresenceMgr.Get().GetPlayer(this.m_gameAccountId);
	}

	// Token: 0x06001A33 RID: 6707 RVA: 0x0007A538 File Offset: 0x00078738
	public bool IsDisplayable()
	{
		if (this.m_gameAccountId == null)
		{
			return false;
		}
		if (!this.IsBnetPlayer())
		{
			return !this.ShouldUseHeroName() || this.GetHeroEntityDef() != null;
		}
		BnetPlayer player = BnetPresenceMgr.Get().GetPlayer(this.m_gameAccountId);
		if (player == null)
		{
			return false;
		}
		if (!player.IsDisplayable())
		{
			return false;
		}
		if (GameUtils.ShouldShowRankedMedals())
		{
			BnetGameAccount hearthstoneGameAccount = player.GetHearthstoneGameAccount();
			if (hearthstoneGameAccount == null)
			{
				return false;
			}
			if (!hearthstoneGameAccount.HasGameField(18U))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001A34 RID: 6708 RVA: 0x0007A5D4 File Offset: 0x000787D4
	public void WipeZzzs()
	{
		foreach (Card card in this.GetBattlefieldZone().GetCards())
		{
			Spell actorSpell = card.GetActorSpell(SpellType.Zzz, true);
			if (!(actorSpell == null))
			{
				actorSpell.ActivateState(SpellStateType.DEATH);
			}
		}
	}

	// Token: 0x06001A35 RID: 6709 RVA: 0x0007A650 File Offset: 0x00078850
	public void AddManaCrystal(int numCrystals, bool isTurnStart)
	{
		if (!this.IsFriendlySide())
		{
			return;
		}
		ManaCrystalMgr.Get().AddManaCrystals(numCrystals, isTurnStart);
	}

	// Token: 0x06001A36 RID: 6710 RVA: 0x0007A66A File Offset: 0x0007886A
	public void AddManaCrystal(int numCrystals)
	{
		this.AddManaCrystal(numCrystals, false);
	}

	// Token: 0x06001A37 RID: 6711 RVA: 0x0007A674 File Offset: 0x00078874
	public void DestroyManaCrystal(int numCrystals)
	{
		if (!this.IsFriendlySide())
		{
			return;
		}
		ManaCrystalMgr.Get().DestroyManaCrystals(numCrystals);
	}

	// Token: 0x06001A38 RID: 6712 RVA: 0x0007A68D File Offset: 0x0007888D
	public void AddTempManaCrystal(int numCrystals)
	{
		if (!this.IsFriendlySide())
		{
			return;
		}
		ManaCrystalMgr.Get().AddTempManaCrystals(numCrystals);
	}

	// Token: 0x06001A39 RID: 6713 RVA: 0x0007A6A6 File Offset: 0x000788A6
	public void DestroyTempManaCrystal(int numCrystals)
	{
		if (!this.IsFriendlySide())
		{
			return;
		}
		ManaCrystalMgr.Get().DestroyTempManaCrystals(numCrystals);
	}

	// Token: 0x06001A3A RID: 6714 RVA: 0x0007A6BF File Offset: 0x000788BF
	public void SpendManaCrystal(int numCrystals)
	{
		if (!this.IsFriendlySide())
		{
			return;
		}
		ManaCrystalMgr.Get().SpendManaCrystals(numCrystals);
	}

	// Token: 0x06001A3B RID: 6715 RVA: 0x0007A6D8 File Offset: 0x000788D8
	public void ReadyManaCrystal(int numCrystals)
	{
		if (!this.IsFriendlySide())
		{
			return;
		}
		ManaCrystalMgr.Get().ReadyManaCrystals(numCrystals);
	}

	// Token: 0x06001A3C RID: 6716 RVA: 0x0007A6F1 File Offset: 0x000788F1
	public void HandleSameTurnOverloadChanged(int crystalsChanged)
	{
		if (!this.IsFriendlySide())
		{
			return;
		}
		ManaCrystalMgr.Get().HandleSameTurnOverloadChanged(crystalsChanged);
	}

	// Token: 0x06001A3D RID: 6717 RVA: 0x0007A70A File Offset: 0x0007890A
	public void UnlockCrystals(int numCrystals)
	{
		if (!this.IsFriendlySide())
		{
			return;
		}
		ManaCrystalMgr.Get().UnlockCrystals(numCrystals);
	}

	// Token: 0x06001A3E RID: 6718 RVA: 0x0007A723 File Offset: 0x00078923
	public void CancelAllProposedMana(Entity entity)
	{
		if (!this.IsFriendlySide())
		{
			return;
		}
		ManaCrystalMgr.Get().CancelAllProposedMana(entity);
	}

	// Token: 0x06001A3F RID: 6719 RVA: 0x0007A73C File Offset: 0x0007893C
	public void ProposeManaCrystalUsage(Entity entity)
	{
		if (!this.IsFriendlySide())
		{
			return;
		}
		ManaCrystalMgr.Get().ProposeManaCrystalUsage(entity);
	}

	// Token: 0x06001A40 RID: 6720 RVA: 0x0007A755 File Offset: 0x00078955
	public void UpdateManaCounter()
	{
		if (this.m_manaCounter == null)
		{
			return;
		}
		this.m_manaCounter.UpdateText();
	}

	// Token: 0x06001A41 RID: 6721 RVA: 0x0007A774 File Offset: 0x00078974
	public void NotifyOfSpentMana(int spentMana)
	{
		this.m_queuedSpentMana += spentMana;
	}

	// Token: 0x06001A42 RID: 6722 RVA: 0x0007A784 File Offset: 0x00078984
	public void NotifyOfUsedTempMana(int usedMana)
	{
		this.m_usedTempMana += usedMana;
	}

	// Token: 0x06001A43 RID: 6723 RVA: 0x0007A794 File Offset: 0x00078994
	public int GetQueuedUsedTempMana()
	{
		return this.m_usedTempMana;
	}

	// Token: 0x06001A44 RID: 6724 RVA: 0x0007A79C File Offset: 0x0007899C
	public int GetQueuedSpentMana()
	{
		return this.m_queuedSpentMana;
	}

	// Token: 0x06001A45 RID: 6725 RVA: 0x0007A7A4 File Offset: 0x000789A4
	public void SetRealTimeTempMana(int tempMana)
	{
		this.m_realtimeTempMana = tempMana;
	}

	// Token: 0x06001A46 RID: 6726 RVA: 0x0007A7AD File Offset: 0x000789AD
	public int GetRealTimeTempMana()
	{
		return this.m_realtimeTempMana;
	}

	// Token: 0x06001A47 RID: 6727 RVA: 0x0007A7B5 File Offset: 0x000789B5
	public void OnBoardLoaded()
	{
		this.AssignPlayerBoardObjects();
	}

	// Token: 0x06001A48 RID: 6728 RVA: 0x0007A7C0 File Offset: 0x000789C0
	public override void OnRealTimeTagChanged(Network.HistTagChange change)
	{
		GAME_TAG tag = (GAME_TAG)change.Tag;
		if (tag != GAME_TAG.COMBO_ACTIVE)
		{
			if (tag == GAME_TAG.TEMP_RESOURCES)
			{
				this.SetRealTimeTempMana(change.Value);
			}
		}
		else
		{
			this.SetRealTimeComboActive(change.Value);
		}
	}

	// Token: 0x06001A49 RID: 6729 RVA: 0x0007A818 File Offset: 0x00078A18
	public override void OnTagsChanged(TagDeltaSet changeSet)
	{
		for (int i = 0; i < changeSet.Size(); i++)
		{
			TagDelta change = changeSet[i];
			this.OnTagChanged(change);
		}
	}

	// Token: 0x06001A4A RID: 6730 RVA: 0x0007A84C File Offset: 0x00078A4C
	public override void OnTagChanged(TagDelta change)
	{
		if (this.IsFriendlySide())
		{
			this.OnFriendlyPlayerTagChanged(change);
		}
		else
		{
			this.OnOpposingPlayerTagChanged(change);
		}
		GAME_TAG tag = (GAME_TAG)change.tag;
		switch (tag)
		{
		case GAME_TAG.LOCK_AND_LOAD:
		{
			Card heroCard = this.GetHeroCard();
			this.ToggleActorSpellOnCard(heroCard, change, SpellType.LOCK_AND_LOAD);
			break;
		}
		default:
			if (tag != GAME_TAG.RESOURCES_USED && tag != GAME_TAG.RESOURCES)
			{
				if (tag == GAME_TAG.PLAYSTATE)
				{
					TAG_PLAYSTATE newValue = (TAG_PLAYSTATE)change.newValue;
					if (newValue == TAG_PLAYSTATE.CONCEDED)
					{
						this.PlayConcedeEmote();
					}
					break;
				}
				if (tag == GAME_TAG.COMBO_ACTIVE)
				{
					foreach (Card card in this.GetHandZone().GetCards())
					{
						card.UpdateActorState();
					}
					break;
				}
				if (tag != GAME_TAG.TEMP_RESOURCES)
				{
					if (tag == GAME_TAG.MULLIGAN_STATE)
					{
						if (change.newValue == 4 && MulliganManager.Get() != null)
						{
							MulliganManager.Get().ServerHasDealtReplacementCards(this.IsFriendlySide());
						}
						break;
					}
					if (tag == GAME_TAG.STEADY_SHOT_CAN_TARGET)
					{
						Card heroPowerCard = this.GetHeroPowerCard();
						this.ToggleActorSpellOnCard(heroPowerCard, change, SpellType.STEADY_SHOT_CAN_TARGET);
						break;
					}
					if (tag == GAME_TAG.CURRENT_HEROPOWER_DAMAGE_BONUS)
					{
						if (this.IsHeroPowerAffectedByBonusDamage())
						{
							Card heroPowerCard2 = this.GetHeroPowerCard();
							this.ToggleActorSpellOnCard(heroPowerCard2, change, SpellType.CURRENT_HEROPOWER_DAMAGE_BONUS);
						}
						break;
					}
					if (tag == GAME_TAG.SPELLS_COST_HEALTH)
					{
						this.UpdateSpellsCostHealth(change);
						break;
					}
					if (tag != GAME_TAG.EMBRACE_THE_SHADOW)
					{
						break;
					}
					Card heroCard2 = this.GetHeroCard();
					this.ToggleActorSpellOnCard(heroCard2, change, SpellType.EMBRACE_THE_SHADOW);
					break;
				}
			}
			if (!GameState.Get().IsTurnStartManagerActive() || !this.IsFriendlySide())
			{
				this.UpdateManaCounter();
			}
			break;
		case GAME_TAG.SHADOWFORM:
		{
			Card heroCard3 = this.GetHeroCard();
			this.ToggleActorSpellOnCard(heroCard3, change, SpellType.SHADOWFORM);
			break;
		}
		case GAME_TAG.CHOOSE_BOTH:
			this.UpdateChooseBoth();
			break;
		}
	}

	// Token: 0x06001A4B RID: 6731 RVA: 0x0007AA78 File Offset: 0x00078C78
	private void OnFriendlyPlayerTagChanged(TagDelta change)
	{
		GAME_TAG tag = (GAME_TAG)change.tag;
		switch (tag)
		{
		case GAME_TAG.CURRENT_SPELLPOWER:
			break;
		default:
			switch (tag)
			{
			case GAME_TAG.CURRENT_PLAYER:
				if (change.newValue == 1)
				{
					ManaCrystalMgr.Get().OnCurrentPlayerChanged();
					this.m_queuedSpentMana = 0;
					if (GameState.Get().IsMainPhase())
					{
						TurnStartManager.Get().BeginListeningForTurnEvents();
					}
				}
				return;
			default:
				if (tag != GAME_TAG.SPELLPOWER_DOUBLE && tag != GAME_TAG.HEALING_DOUBLE)
				{
					if (tag == GAME_TAG.MULLIGAN_STATE)
					{
						if (change.newValue == 4 && MulliganManager.Get() == null)
						{
							foreach (Card card in this.GetHandZone().GetCards())
							{
								card.GetActor().TurnOnCollider();
							}
						}
						return;
					}
					if (tag != GAME_TAG.OVERLOAD_LOCKED)
					{
						return;
					}
					if (change.newValue < change.oldValue)
					{
						this.UnlockCrystals(change.oldValue - change.newValue);
					}
					return;
				}
				break;
			case GAME_TAG.RESOURCES_USED:
			{
				int num = change.oldValue + this.m_queuedSpentMana;
				int num2 = change.newValue - change.oldValue;
				if (num2 > 0)
				{
					this.m_queuedSpentMana -= num2;
				}
				if (this.m_queuedSpentMana < 0)
				{
					this.m_queuedSpentMana = 0;
				}
				int shownChangeAmount = change.newValue - num + this.m_queuedSpentMana;
				ManaCrystalMgr.Get().UpdateSpentMana(shownChangeAmount);
				return;
			}
			case GAME_TAG.RESOURCES:
				if (change.newValue > change.oldValue)
				{
					if (GameState.Get().IsTurnStartManagerActive() && this.IsFriendlySide())
					{
						TurnStartManager.Get().NotifyOfManaCrystalGained(change.newValue - change.oldValue);
					}
					else
					{
						this.AddManaCrystal(change.newValue - change.oldValue);
					}
				}
				else
				{
					this.DestroyManaCrystal(change.oldValue - change.newValue);
				}
				return;
			}
			break;
		case GAME_TAG.TEMP_RESOURCES:
		{
			int num3 = change.oldValue - this.m_usedTempMana;
			int num4 = change.newValue - change.oldValue;
			if (num4 < 0)
			{
				this.m_usedTempMana += num4;
			}
			if (this.m_usedTempMana < 0)
			{
				this.m_usedTempMana = 0;
			}
			if (num3 < 0)
			{
				num3 = 0;
			}
			int num5 = change.newValue - num3 - this.m_usedTempMana;
			if (num5 > 0)
			{
				this.AddTempManaCrystal(num5);
			}
			else
			{
				this.DestroyTempManaCrystal(-num5);
			}
			return;
		}
		case GAME_TAG.OVERLOAD_OWED:
			this.HandleSameTurnOverloadChanged(change.newValue - change.oldValue);
			return;
		}
		this.UpdateHandCardPowersText();
	}

	// Token: 0x06001A4C RID: 6732 RVA: 0x0007AD68 File Offset: 0x00078F68
	private void OnOpposingPlayerTagChanged(TagDelta change)
	{
		GAME_TAG tag = (GAME_TAG)change.tag;
		if (tag != GAME_TAG.PLAYSTATE)
		{
			if (tag == GAME_TAG.RESOURCES)
			{
				if (change.newValue > change.oldValue)
				{
					GameState.Get().GetGameEntity().NotifyOfEnemyManaCrystalSpawned();
				}
			}
		}
		else
		{
			TAG_PLAYSTATE newValue = (TAG_PLAYSTATE)change.newValue;
			if (newValue == TAG_PLAYSTATE.DISCONNECTED)
			{
				NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_ANNOUNCER_DISCONNECT_45"), "VO_ANNOUNCER_DISCONNECT_45", 0f, null);
			}
		}
	}

	// Token: 0x06001A4D RID: 6733 RVA: 0x0007ADF0 File Offset: 0x00078FF0
	private void UpdateName()
	{
		if (this.ShouldUseHeroName())
		{
			this.UpdateNameWithHeroName();
		}
		else if (this.IsAI())
		{
			this.m_name = GameStrings.Get("GAMEPLAY_AI_OPPONENT_NAME");
		}
		else if (this.IsBnetPlayer())
		{
			BnetPlayer player = BnetPresenceMgr.Get().GetPlayer(this.m_gameAccountId);
			this.m_name = player.GetBestName();
		}
		else
		{
			Debug.LogError(string.Format("Player.UpdateName() - unable to determine player name", new object[0]));
		}
	}

	// Token: 0x06001A4E RID: 6734 RVA: 0x0007AE78 File Offset: 0x00079078
	private bool ShouldUseHeroName()
	{
		return !this.IsBnetPlayer() && (!this.IsAI() || !GameMgr.Get().IsPractice());
	}

	// Token: 0x06001A4F RID: 6735 RVA: 0x0007AEB0 File Offset: 0x000790B0
	private void UpdateNameWithHeroName()
	{
		if (this.m_hero == null)
		{
			return;
		}
		EntityDef entityDef = this.m_hero.GetEntityDef();
		if (entityDef == null)
		{
			return;
		}
		this.m_name = entityDef.GetName();
	}

	// Token: 0x06001A50 RID: 6736 RVA: 0x0007AEE8 File Offset: 0x000790E8
	private bool ShouldUseBogusRank()
	{
		return !this.IsBnetPlayer();
	}

	// Token: 0x06001A51 RID: 6737 RVA: 0x0007AEF8 File Offset: 0x000790F8
	private void UpdateRank()
	{
		if (this.ShouldUseBogusRank())
		{
			this.m_medalInfo = new MedalInfoTranslator();
		}
		else
		{
			BnetPlayer player = BnetPresenceMgr.Get().GetPlayer(this.m_gameAccountId);
			this.m_medalInfo = RankMgr.Get().GetRankPresenceField(player);
		}
	}

	// Token: 0x06001A52 RID: 6738 RVA: 0x0007AF44 File Offset: 0x00079144
	private void UpdateDisplayInfo()
	{
		this.UpdateName();
		this.UpdateRank();
		if (this.IsBnetPlayer() && !this.IsLocalUser())
		{
			BnetPlayer player = BnetPresenceMgr.Get().GetPlayer(this.m_gameAccountId);
			if (BnetFriendMgr.Get().IsFriend(player))
			{
				ChatMgr.Get().AddRecentWhisperPlayerToBottom(player);
			}
		}
	}

	// Token: 0x06001A53 RID: 6739 RVA: 0x0007AFA0 File Offset: 0x000791A0
	private void OnBnetPlayersChanged(BnetPlayerChangelist changelist, object userData)
	{
		if (changelist.FindChange(this.m_gameAccountId) == null)
		{
			return;
		}
		if (!this.IsDisplayable())
		{
			return;
		}
		BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnBnetPlayersChanged));
		this.UpdateDisplayInfo();
	}

	// Token: 0x06001A54 RID: 6740 RVA: 0x0007AFEC File Offset: 0x000791EC
	private void UpdateLocal()
	{
		if (this.IsBnetPlayer())
		{
			BnetGameAccountId myGameAccountId = BnetPresenceMgr.Get().GetMyGameAccountId();
			this.m_local = (myGameAccountId == this.m_gameAccountId);
		}
		else
		{
			this.m_local = (this.m_gameAccountId.GetLo() == 1UL);
		}
	}

	// Token: 0x06001A55 RID: 6741 RVA: 0x0007B03C File Offset: 0x0007923C
	private void UpdateSide()
	{
		if (GameMgr.Get().IsSpectator())
		{
			if (this.m_gameAccountId == SpectatorManager.Get().GetSpectateeFriendlySide())
			{
				this.m_side = Player.Side.FRIENDLY;
			}
			else
			{
				this.m_side = Player.Side.OPPOSING;
			}
			return;
		}
		if (this.IsLocalUser())
		{
			this.m_side = Player.Side.FRIENDLY;
		}
		else
		{
			this.m_side = Player.Side.OPPOSING;
		}
	}

	// Token: 0x06001A56 RID: 6742 RVA: 0x0007B0A4 File Offset: 0x000792A4
	private void AssignPlayerBoardObjects()
	{
		ManaCounter[] componentsInChildren = BoardStandardGame.Get().gameObject.GetComponentsInChildren<ManaCounter>(true);
		foreach (ManaCounter manaCounter in componentsInChildren)
		{
			if (manaCounter.m_Side == this.m_side)
			{
				this.m_manaCounter = manaCounter;
				this.m_manaCounter.SetPlayer(this);
				this.m_manaCounter.UpdateText();
				break;
			}
		}
		this.InitManaCrystalMgr();
		foreach (Zone zone in ZoneMgr.Get().GetZones())
		{
			if (zone.m_Side == this.m_side)
			{
				zone.SetController(this);
			}
		}
	}

	// Token: 0x06001A57 RID: 6743 RVA: 0x0007B184 File Offset: 0x00079384
	private void InitManaCrystalMgr()
	{
		if (!this.IsFriendlySide())
		{
			return;
		}
		int tag = base.GetTag(GAME_TAG.TEMP_RESOURCES);
		int tag2 = base.GetTag(GAME_TAG.RESOURCES);
		int tag3 = base.GetTag(GAME_TAG.RESOURCES_USED);
		int tag4 = base.GetTag(GAME_TAG.OVERLOAD_OWED);
		ManaCrystalMgr.Get().AddManaCrystals(tag2, false);
		ManaCrystalMgr.Get().AddTempManaCrystals(tag);
		ManaCrystalMgr.Get().UpdateSpentMana(tag3);
		ManaCrystalMgr.Get().MarkCrystalsOwedForOverload(tag4);
	}

	// Token: 0x06001A58 RID: 6744 RVA: 0x0007B1F4 File Offset: 0x000793F4
	private void OnTurnChanged(int oldTurn, int newTurn, object userData)
	{
		this.WipeZzzs();
	}

	// Token: 0x06001A59 RID: 6745 RVA: 0x0007B1FC File Offset: 0x000793FC
	private void OnFriendlyOptionsReceived(object userData)
	{
		this.UpdateChooseBoth();
	}

	// Token: 0x06001A5A RID: 6746 RVA: 0x0007B204 File Offset: 0x00079404
	private void OnFriendlyOptionsSent(Network.Options.Option option, object userData)
	{
		this.UpdateChooseBoth();
		Entity entity = GameState.Get().GetEntity(option.Main.ID);
		this.CancelAllProposedMana(entity);
	}

	// Token: 0x06001A5B RID: 6747 RVA: 0x0007B234 File Offset: 0x00079434
	private void OnFriendlyTurnStarted(object userData)
	{
		this.UpdateChooseBoth();
	}

	// Token: 0x06001A5C RID: 6748 RVA: 0x0007B23C File Offset: 0x0007943C
	private void ToggleActorSpellOnCard(Card card, TagDelta change, SpellType spellType)
	{
		if (card == null)
		{
			return;
		}
		if (!card.CanShowActorVisuals())
		{
			return;
		}
		Actor actor = card.GetActor();
		if (change.newValue > 0)
		{
			actor.ActivateSpell(spellType);
		}
		else
		{
			actor.DeactivateSpell(spellType);
		}
	}

	// Token: 0x06001A5D RID: 6749 RVA: 0x0007B28C File Offset: 0x0007948C
	private void UpdateHandCardPowersText()
	{
		ZoneHand handZone = this.GetHandZone();
		List<Card> cards = handZone.GetCards();
		for (int i = 0; i < cards.Count; i++)
		{
			Card card = cards[i];
			if (card.GetEntity().IsSpell())
			{
				card.GetActor().UpdatePowersText();
			}
		}
	}

	// Token: 0x06001A5E RID: 6750 RVA: 0x0007B2E4 File Offset: 0x000794E4
	private void UpdateSpellsCostHealth(TagDelta change)
	{
		if (this.IsFriendlySide())
		{
			Card mousedOverCard = InputManager.Get().GetMousedOverCard();
			if (mousedOverCard != null)
			{
				Entity entity = mousedOverCard.GetEntity();
				if (entity.IsSpell())
				{
					if (change.newValue > 0)
					{
						ManaCrystalMgr.Get().CancelAllProposedMana(entity);
					}
					else
					{
						ManaCrystalMgr.Get().ProposeManaCrystalUsage(entity);
					}
				}
			}
		}
		ZoneHand handZone = this.GetHandZone();
		List<Card> cards = handZone.GetCards();
		for (int i = 0; i < cards.Count; i++)
		{
			Card card = cards[i];
			if (card.CanShowActorVisuals())
			{
				Entity entity2 = card.GetEntity();
				if (entity2.IsSpell())
				{
					Actor actor = card.GetActor();
					if (change.newValue > 0)
					{
						actor.ActivateSpell(SpellType.SPELLS_COST_HEALTH);
					}
					else
					{
						actor.DeactivateSpell(SpellType.SPELLS_COST_HEALTH);
					}
				}
			}
		}
	}

	// Token: 0x06001A5F RID: 6751 RVA: 0x0007B3D8 File Offset: 0x000795D8
	private void UpdateChooseBoth()
	{
		ZoneHand handZone = this.GetHandZone();
		List<Card> cards = handZone.GetCards();
		for (int i = 0; i < cards.Count; i++)
		{
			Card card = cards[i];
			if (card.CanShowActorVisuals())
			{
				Entity entity = card.GetEntity();
				if (entity.HasTag(GAME_TAG.CHOOSE_ONE))
				{
					Actor actor = card.GetActor();
					SpellType spellType = SpellType.CHOOSE_BOTH;
					if (base.HasTag(GAME_TAG.CHOOSE_BOTH) && GameState.Get().IsOption(entity))
					{
						Spell spell = actor.GetSpell(spellType);
						SpellUtils.ActivateBirthIfNecessary(spell);
					}
					else
					{
						Spell spellIfLoaded = actor.GetSpellIfLoaded(spellType);
						SpellUtils.ActivateDeathIfNecessary(spellIfLoaded);
					}
				}
			}
		}
	}

	// Token: 0x06001A60 RID: 6752 RVA: 0x0007B498 File Offset: 0x00079698
	public PlayErrors.PlayerInfo ConvertToPlayerInfo()
	{
		PlayErrors.PlayerInfo playerInfo = new PlayErrors.PlayerInfo();
		int numMinionsInPlay = this.GetNumMinionsInPlay();
		int numTotalMinionsInPlay = this.GetNumTotalMinionsInPlay();
		playerInfo.id = this.GetPlayerId();
		playerInfo.numResources = this.GetNumAvailableResources();
		playerInfo.numFriendlyMinionsInPlay = numMinionsInPlay;
		playerInfo.numEnemyMinionsInPlay = numTotalMinionsInPlay - numMinionsInPlay;
		playerInfo.numMinionSlotsPerPlayer = GameState.Get().GetMaxFriendlyMinionsPerPlayer();
		playerInfo.numOpenSecretSlots = GameState.Get().GetMaxSecretsPerPlayer() - this.GetSecretDefinitions().Count;
		playerInfo.numDragonsInHand = this.GetNumDragonsInHand();
		playerInfo.numFriendlyMinionsThatDiedThisTurn = base.GetTag(GAME_TAG.NUM_FRIENDLY_MINIONS_THAT_DIED_THIS_TURN);
		playerInfo.numFriendlyMinionsThatDiedThisGame = base.GetTag(GAME_TAG.NUM_FRIENDLY_MINIONS_THAT_DIED_THIS_GAME);
		playerInfo.currentDefense = this.GetHero().GetRemainingHealth();
		playerInfo.isCurrentPlayer = this.IsCurrentPlayer();
		playerInfo.weaponEquipped = this.HasWeapon();
		playerInfo.enemyWeaponEquipped = GameState.Get().GetOpposingSidePlayer().HasWeapon();
		playerInfo.comboActive = this.IsComboActive();
		playerInfo.steadyShotRequiresTarget = base.HasTag(GAME_TAG.STEADY_SHOT_CAN_TARGET);
		playerInfo.spellsCostHealth = base.HasTag(GAME_TAG.SPELLS_COST_HEALTH);
		return playerInfo;
	}

	// Token: 0x04000D03 RID: 3331
	private BnetGameAccountId m_gameAccountId;

	// Token: 0x04000D04 RID: 3332
	private bool m_waitingForHeroEntity;

	// Token: 0x04000D05 RID: 3333
	private string m_name;

	// Token: 0x04000D06 RID: 3334
	private bool m_local;

	// Token: 0x04000D07 RID: 3335
	private Player.Side m_side;

	// Token: 0x04000D08 RID: 3336
	private int m_cardBackId;

	// Token: 0x04000D09 RID: 3337
	private ManaCounter m_manaCounter;

	// Token: 0x04000D0A RID: 3338
	private Entity m_startingHero;

	// Token: 0x04000D0B RID: 3339
	private Entity m_hero;

	// Token: 0x04000D0C RID: 3340
	private Entity m_heroPower;

	// Token: 0x04000D0D RID: 3341
	private int m_queuedSpentMana;

	// Token: 0x04000D0E RID: 3342
	private int m_usedTempMana;

	// Token: 0x04000D0F RID: 3343
	private int m_realtimeTempMana;

	// Token: 0x04000D10 RID: 3344
	private bool m_realTimeComboActive;

	// Token: 0x04000D11 RID: 3345
	private MedalInfoTranslator m_medalInfo;

	// Token: 0x04000D12 RID: 3346
	private bool m_concedeEmotePlayed;

	// Token: 0x02000335 RID: 821
	public enum Side
	{
		// Token: 0x0400199D RID: 6557
		NEUTRAL,
		// Token: 0x0400199E RID: 6558
		FRIENDLY,
		// Token: 0x0400199F RID: 6559
		OPPOSING
	}
}
