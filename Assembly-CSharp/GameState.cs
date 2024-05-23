using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using bgs;
using PegasusGame;
using UnityEngine;

// Token: 0x02000194 RID: 404
public class GameState
{
	// Token: 0x060018FD RID: 6397 RVA: 0x00075015 File Offset: 0x00073215
	public static GameState Get()
	{
		return GameState.s_instance;
	}

	// Token: 0x060018FE RID: 6398 RVA: 0x0007501C File Offset: 0x0007321C
	public static GameState Initialize()
	{
		if (GameState.s_instance == null)
		{
			GameState.s_instance = new GameState();
			GameState.FireGameStateInitializedEvent();
		}
		return GameState.s_instance;
	}

	// Token: 0x060018FF RID: 6399 RVA: 0x0007503C File Offset: 0x0007323C
	public static void Shutdown()
	{
		if (GameState.s_instance == null)
		{
			return;
		}
		GameState.s_instance = null;
	}

	// Token: 0x06001900 RID: 6400 RVA: 0x0007504F File Offset: 0x0007324F
	public void Update()
	{
		if (this.CheckBlockingPowerProcessor())
		{
			return;
		}
		this.m_powerProcessor.ProcessPowerQueue();
	}

	// Token: 0x06001901 RID: 6401 RVA: 0x00075068 File Offset: 0x00073268
	public PowerProcessor GetPowerProcessor()
	{
		return this.m_powerProcessor;
	}

	// Token: 0x06001902 RID: 6402 RVA: 0x00075070 File Offset: 0x00073270
	public bool HasPowersToProcess()
	{
		return this.m_powerProcessor.GetCurrentTaskList() == null && this.m_powerProcessor.GetPowerQueue().Count <= 0;
	}

	// Token: 0x06001903 RID: 6403 RVA: 0x000750A8 File Offset: 0x000732A8
	public Entity GetEntity(int id)
	{
		Entity result;
		this.m_entityMap.TryGetValue(id, out result);
		return result;
	}

	// Token: 0x06001904 RID: 6404 RVA: 0x000750C8 File Offset: 0x000732C8
	public Player GetPlayer(int id)
	{
		Player result;
		this.m_playerMap.TryGetValue(id, out result);
		return result;
	}

	// Token: 0x06001905 RID: 6405 RVA: 0x000750E5 File Offset: 0x000732E5
	public GameEntity GetGameEntity()
	{
		return this.m_gameEntity;
	}

	// Token: 0x06001906 RID: 6406 RVA: 0x000750ED File Offset: 0x000732ED
	[Conditional("UNITY_EDITOR")]
	public void DebugSetGameEntity(GameEntity gameEntity)
	{
		this.m_gameEntity = gameEntity;
	}

	// Token: 0x06001907 RID: 6407 RVA: 0x000750F6 File Offset: 0x000732F6
	public bool WasGameCreated()
	{
		return this.m_gameEntity != null;
	}

	// Token: 0x06001908 RID: 6408 RVA: 0x00075104 File Offset: 0x00073304
	public Player GetFriendlySidePlayer()
	{
		foreach (Player player in this.m_playerMap.Values)
		{
			if (player.IsFriendlySide())
			{
				return player;
			}
		}
		return null;
	}

	// Token: 0x06001909 RID: 6409 RVA: 0x00075174 File Offset: 0x00073374
	public Player GetOpposingSidePlayer()
	{
		foreach (Player player in this.m_playerMap.Values)
		{
			if (player.IsOpposingSide())
			{
				return player;
			}
		}
		return null;
	}

	// Token: 0x0600190A RID: 6410 RVA: 0x000751E4 File Offset: 0x000733E4
	public int GetFriendlyPlayerId()
	{
		Player friendlySidePlayer = this.GetFriendlySidePlayer();
		if (friendlySidePlayer == null)
		{
			return 0;
		}
		return friendlySidePlayer.GetPlayerId();
	}

	// Token: 0x0600190B RID: 6411 RVA: 0x00075208 File Offset: 0x00073408
	public int GetOpposingPlayerId()
	{
		Player opposingSidePlayer = this.GetOpposingSidePlayer();
		if (opposingSidePlayer == null)
		{
			return 0;
		}
		return opposingSidePlayer.GetPlayerId();
	}

	// Token: 0x0600190C RID: 6412 RVA: 0x0007522C File Offset: 0x0007342C
	public bool IsFriendlySidePlayerTurn()
	{
		Player friendlySidePlayer = this.GetFriendlySidePlayer();
		return friendlySidePlayer != null && friendlySidePlayer.IsCurrentPlayer();
	}

	// Token: 0x0600190D RID: 6413 RVA: 0x00075250 File Offset: 0x00073450
	public Player GetCurrentPlayer()
	{
		foreach (Player player in this.m_playerMap.Values)
		{
			if (player.IsCurrentPlayer())
			{
				return player;
			}
		}
		return null;
	}

	// Token: 0x0600190E RID: 6414 RVA: 0x000752C0 File Offset: 0x000734C0
	public Player GetFirstOpponentPlayer(Player player)
	{
		foreach (Player player2 in this.m_playerMap.Values)
		{
			if (player2.GetSide() != player.GetSide())
			{
				return player2;
			}
		}
		return null;
	}

	// Token: 0x0600190F RID: 6415 RVA: 0x00075334 File Offset: 0x00073534
	public int GetTurn()
	{
		return (this.m_gameEntity != null) ? this.m_gameEntity.GetTag(GAME_TAG.TURN) : 0;
	}

	// Token: 0x06001910 RID: 6416 RVA: 0x00075364 File Offset: 0x00073564
	public bool IsResponsePacketBlocked()
	{
		if (this.IsMulliganManagerActive())
		{
			return false;
		}
		if (!this.IsFriendlySidePlayerTurn())
		{
			return true;
		}
		if (this.IsTurnStartManagerBlockingInput())
		{
			return true;
		}
		if (EndTurnButton.Get().IsInWaitingState())
		{
			return true;
		}
		switch (this.m_responseMode)
		{
		case GameState.ResponseMode.NONE:
			return true;
		case GameState.ResponseMode.OPTION:
		case GameState.ResponseMode.SUB_OPTION:
		case GameState.ResponseMode.OPTION_TARGET:
			if (this.m_options == null)
			{
				return true;
			}
			break;
		case GameState.ResponseMode.CHOICE:
			if (this.GetFriendlyEntityChoices() == null)
			{
				return true;
			}
			break;
		default:
			Debug.LogWarning(string.Format("GameState.IsResponsePacketBlocked() - unhandled response mode {0}", this.m_responseMode));
			break;
		}
		return false;
	}

	// Token: 0x06001911 RID: 6417 RVA: 0x00075417 File Offset: 0x00073617
	public Map<int, Entity> GetEntityMap()
	{
		return this.m_entityMap;
	}

	// Token: 0x06001912 RID: 6418 RVA: 0x0007541F File Offset: 0x0007361F
	public Map<int, Player> GetPlayerMap()
	{
		return this.m_playerMap;
	}

	// Token: 0x06001913 RID: 6419 RVA: 0x00075428 File Offset: 0x00073628
	public void AddPlayer(Player player)
	{
		this.m_playerMap.Add(player.GetPlayerId(), player);
		this.m_entityMap.Add(player.GetEntityId(), player);
	}

	// Token: 0x06001914 RID: 6420 RVA: 0x0007545C File Offset: 0x0007365C
	public void RemovePlayer(Player player)
	{
		player.Destroy();
		this.m_playerMap.Remove(player.GetPlayerId());
		this.m_entityMap.Remove(player.GetEntityId());
	}

	// Token: 0x06001915 RID: 6421 RVA: 0x00075493 File Offset: 0x00073693
	public void AddEntity(Entity entity)
	{
		this.m_entityMap.Add(entity.GetEntityId(), entity);
	}

	// Token: 0x06001916 RID: 6422 RVA: 0x000754A8 File Offset: 0x000736A8
	public void RemoveEntity(Entity entity)
	{
		if (entity.IsPlayer())
		{
			this.RemovePlayer(entity as Player);
			return;
		}
		if (entity.IsGame())
		{
			this.m_gameEntity = null;
			return;
		}
		if (entity.IsAttached())
		{
			Entity entity2 = this.GetEntity(entity.GetAttached());
			if (entity2 != null)
			{
				entity2.RemoveAttachment(entity);
			}
		}
		if (entity.IsHero())
		{
			Player player = this.GetPlayer(entity.GetControllerId());
			if (player != null)
			{
				player.SetHero(null);
			}
		}
		else if (entity.IsHeroPower())
		{
			Player player2 = this.GetPlayer(entity.GetControllerId());
			if (player2 != null)
			{
				player2.SetHeroPower(null);
			}
		}
		entity.Destroy();
	}

	// Token: 0x06001917 RID: 6423 RVA: 0x0007555A File Offset: 0x0007375A
	public int GetMaxSecretsPerPlayer()
	{
		return this.m_maxSecretsPerPlayer;
	}

	// Token: 0x06001918 RID: 6424 RVA: 0x00075562 File Offset: 0x00073762
	public int GetMaxFriendlyMinionsPerPlayer()
	{
		return this.m_maxFriendlyMinionsPerPlayer;
	}

	// Token: 0x06001919 RID: 6425 RVA: 0x0007556A File Offset: 0x0007376A
	public bool IsBusy()
	{
		return this.m_busy;
	}

	// Token: 0x0600191A RID: 6426 RVA: 0x00075572 File Offset: 0x00073772
	public void SetBusy(bool busy)
	{
		this.m_busy = busy;
	}

	// Token: 0x0600191B RID: 6427 RVA: 0x0007557B File Offset: 0x0007377B
	public bool IsMulliganBusy()
	{
		return this.m_mulliganBusy;
	}

	// Token: 0x0600191C RID: 6428 RVA: 0x00075583 File Offset: 0x00073783
	public void SetMulliganBusy(bool busy)
	{
		this.m_mulliganBusy = busy;
	}

	// Token: 0x0600191D RID: 6429 RVA: 0x0007558C File Offset: 0x0007378C
	public bool IsMulliganManagerActive()
	{
		return !(MulliganManager.Get() == null) && MulliganManager.Get().IsMulliganActive();
	}

	// Token: 0x0600191E RID: 6430 RVA: 0x000755AA File Offset: 0x000737AA
	public bool IsTurnStartManagerActive()
	{
		return !(TurnStartManager.Get() == null) && TurnStartManager.Get().IsListeningForTurnEvents();
	}

	// Token: 0x0600191F RID: 6431 RVA: 0x000755C8 File Offset: 0x000737C8
	public bool IsTurnStartManagerBlockingInput()
	{
		return !(TurnStartManager.Get() == null) && TurnStartManager.Get().IsBlockingInput();
	}

	// Token: 0x06001920 RID: 6432 RVA: 0x000755E6 File Offset: 0x000737E6
	public bool HasTheCoinBeenSpawned()
	{
		return this.m_coinHasSpawned;
	}

	// Token: 0x06001921 RID: 6433 RVA: 0x000755EE File Offset: 0x000737EE
	public void NotifyOfCoinSpawn()
	{
		this.m_coinHasSpawned = true;
	}

	// Token: 0x06001922 RID: 6434 RVA: 0x000755F8 File Offset: 0x000737F8
	public bool IsBeginPhase()
	{
		if (this.m_gameEntity == null)
		{
			return false;
		}
		TAG_STEP tag = this.m_gameEntity.GetTag<TAG_STEP>(GAME_TAG.STEP);
		return GameUtils.IsBeginPhase(tag);
	}

	// Token: 0x06001923 RID: 6435 RVA: 0x00075628 File Offset: 0x00073828
	public bool IsPastBeginPhase()
	{
		if (this.m_gameEntity == null)
		{
			return false;
		}
		TAG_STEP tag = this.m_gameEntity.GetTag<TAG_STEP>(GAME_TAG.STEP);
		return GameUtils.IsPastBeginPhase(tag);
	}

	// Token: 0x06001924 RID: 6436 RVA: 0x00075658 File Offset: 0x00073858
	public bool IsMainPhase()
	{
		if (this.m_gameEntity == null)
		{
			return false;
		}
		TAG_STEP tag = this.m_gameEntity.GetTag<TAG_STEP>(GAME_TAG.STEP);
		return GameUtils.IsMainPhase(tag);
	}

	// Token: 0x06001925 RID: 6437 RVA: 0x00075688 File Offset: 0x00073888
	public bool IsMulliganPhase()
	{
		if (this.m_gameEntity == null)
		{
			return false;
		}
		TAG_STEP tag = this.m_gameEntity.GetTag<TAG_STEP>(GAME_TAG.STEP);
		return tag == TAG_STEP.BEGIN_MULLIGAN;
	}

	// Token: 0x06001926 RID: 6438 RVA: 0x000756B4 File Offset: 0x000738B4
	public bool IsMulliganPhasePending()
	{
		if (this.m_gameEntity == null)
		{
			return false;
		}
		TAG_STEP tag = this.m_gameEntity.GetTag<TAG_STEP>(GAME_TAG.NEXT_STEP);
		if (tag == TAG_STEP.BEGIN_MULLIGAN)
		{
			return true;
		}
		bool foundMulliganStep = false;
		int gameEntityId = this.m_gameEntity.GetEntityId();
		this.m_powerProcessor.ForEachTaskList(delegate(int queueIndex, PowerTaskList taskList)
		{
			List<PowerTask> taskList2 = taskList.GetTaskList();
			for (int i = 0; i < taskList2.Count; i++)
			{
				PowerTask powerTask = taskList2[i];
				Network.HistTagChange histTagChange = powerTask.GetPower() as Network.HistTagChange;
				if (histTagChange != null)
				{
					if (histTagChange.Entity == gameEntityId)
					{
						GAME_TAG tag2 = (GAME_TAG)histTagChange.Tag;
						if (tag2 == GAME_TAG.STEP || tag2 == GAME_TAG.NEXT_STEP)
						{
							TAG_STEP value = (TAG_STEP)histTagChange.Value;
							if (value == TAG_STEP.BEGIN_MULLIGAN)
							{
								foundMulliganStep = true;
								return;
							}
						}
					}
				}
			}
		});
		return foundMulliganStep;
	}

	// Token: 0x06001927 RID: 6439 RVA: 0x00075723 File Offset: 0x00073923
	public bool IsMulliganPhaseNowOrPending()
	{
		return this.IsMulliganPhase() || this.IsMulliganPhasePending();
	}

	// Token: 0x06001928 RID: 6440 RVA: 0x00075740 File Offset: 0x00073940
	public GameState.CreateGamePhase GetCreateGamePhase()
	{
		return this.m_createGamePhase;
	}

	// Token: 0x06001929 RID: 6441 RVA: 0x00075748 File Offset: 0x00073948
	public bool IsGameCreating()
	{
		return this.m_createGamePhase == GameState.CreateGamePhase.CREATING;
	}

	// Token: 0x0600192A RID: 6442 RVA: 0x00075753 File Offset: 0x00073953
	public bool IsGameCreated()
	{
		return this.m_createGamePhase == GameState.CreateGamePhase.CREATED;
	}

	// Token: 0x0600192B RID: 6443 RVA: 0x0007575E File Offset: 0x0007395E
	public bool IsGameCreatedOrCreating()
	{
		return this.IsGameCreated() || this.IsGameCreating();
	}

	// Token: 0x0600192C RID: 6444 RVA: 0x00075774 File Offset: 0x00073974
	public bool IsConcedingAllowed()
	{
		return !GameMgr.Get().IsTutorial();
	}

	// Token: 0x0600192D RID: 6445 RVA: 0x00075788 File Offset: 0x00073988
	public bool WasConcedeRequested()
	{
		return this.m_concedeRequested;
	}

	// Token: 0x0600192E RID: 6446 RVA: 0x00075790 File Offset: 0x00073990
	public void Concede()
	{
		if (this.m_concedeRequested)
		{
			return;
		}
		this.m_concedeRequested = true;
		Network.Concede();
	}

	// Token: 0x0600192F RID: 6447 RVA: 0x000757AA File Offset: 0x000739AA
	public bool IsGameOver()
	{
		return this.m_gameOver;
	}

	// Token: 0x06001930 RID: 6448 RVA: 0x000757B2 File Offset: 0x000739B2
	public bool IsGameOverPending()
	{
		return this.m_realTimeGameOverTagChange != null;
	}

	// Token: 0x06001931 RID: 6449 RVA: 0x000757C0 File Offset: 0x000739C0
	public bool IsGameOverNowOrPending()
	{
		return this.IsGameOver() || this.IsGameOverPending();
	}

	// Token: 0x06001932 RID: 6450 RVA: 0x000757DD File Offset: 0x000739DD
	public Network.HistTagChange GetRealTimeGameOverTagChange()
	{
		return this.m_realTimeGameOverTagChange;
	}

	// Token: 0x06001933 RID: 6451 RVA: 0x000757E8 File Offset: 0x000739E8
	public void ShowEnemyTauntCharacters()
	{
		List<Zone> zones = ZoneMgr.Get().GetZones();
		for (int i = 0; i < zones.Count; i++)
		{
			Zone zone = zones[i];
			if (zone.m_ServerTag == TAG_ZONE.PLAY)
			{
				if (zone.m_Side == Player.Side.OPPOSING)
				{
					List<Card> cards = zone.GetCards();
					for (int j = 0; j < cards.Count; j++)
					{
						Card card = cards[j];
						Entity entity = card.GetEntity();
						if (entity.HasTaunt())
						{
							if (!entity.IsStealthed())
							{
								card.DoTauntNotification();
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06001934 RID: 6452 RVA: 0x000758A0 File Offset: 0x00073AA0
	public void GetTauntCounts(Player player, out int minionCount, out int heroCount)
	{
		minionCount = 0;
		heroCount = 0;
		List<Zone> zones = ZoneMgr.Get().GetZones();
		for (int i = 0; i < zones.Count; i++)
		{
			Zone zone = zones[i];
			if (zone.m_ServerTag == TAG_ZONE.PLAY)
			{
				if (player == zone.GetController())
				{
					List<Card> cards = zone.GetCards();
					for (int j = 0; j < cards.Count; j++)
					{
						Card card = cards[j];
						Entity entity = card.GetEntity();
						if (entity.HasTaunt())
						{
							if (!entity.IsStealthed())
							{
								TAG_CARDTYPE cardType = entity.GetCardType();
								TAG_CARDTYPE tag_CARDTYPE = cardType;
								if (tag_CARDTYPE != TAG_CARDTYPE.HERO)
								{
									if (tag_CARDTYPE == TAG_CARDTYPE.MINION)
									{
										minionCount++;
									}
								}
								else
								{
									heroCount++;
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06001935 RID: 6453 RVA: 0x0007598E File Offset: 0x00073B8E
	public Card GetFriendlyCardBeingDrawn()
	{
		return this.m_friendlyCardBeingDrawn;
	}

	// Token: 0x06001936 RID: 6454 RVA: 0x00075996 File Offset: 0x00073B96
	public void SetFriendlyCardBeingDrawn(Card card)
	{
		this.m_friendlyCardBeingDrawn = card;
	}

	// Token: 0x06001937 RID: 6455 RVA: 0x0007599F File Offset: 0x00073B9F
	public Card GetOpponentCardBeingDrawn()
	{
		return this.m_opponentCardBeingDrawn;
	}

	// Token: 0x06001938 RID: 6456 RVA: 0x000759A7 File Offset: 0x00073BA7
	public void SetOpponentCardBeingDrawn(Card card)
	{
		this.m_opponentCardBeingDrawn = card;
	}

	// Token: 0x06001939 RID: 6457 RVA: 0x000759B0 File Offset: 0x00073BB0
	public bool IsBeingDrawn(Card card)
	{
		return card == this.m_friendlyCardBeingDrawn || card == this.m_opponentCardBeingDrawn;
	}

	// Token: 0x0600193A RID: 6458 RVA: 0x000759D9 File Offset: 0x00073BD9
	public bool ClearCardBeingDrawn(Card card)
	{
		if (card == this.m_friendlyCardBeingDrawn)
		{
			this.m_friendlyCardBeingDrawn = null;
			return true;
		}
		if (card == this.m_opponentCardBeingDrawn)
		{
			this.m_opponentCardBeingDrawn = null;
			return true;
		}
		return false;
	}

	// Token: 0x0600193B RID: 6459 RVA: 0x00075A10 File Offset: 0x00073C10
	public int GetLastTurnRemindedOfFullHand()
	{
		return this.m_lastTurnRemindedOfFullHand;
	}

	// Token: 0x0600193C RID: 6460 RVA: 0x00075A18 File Offset: 0x00073C18
	public void SetLastTurnRemindedOfFullHand(int turn)
	{
		this.m_lastTurnRemindedOfFullHand = turn;
	}

	// Token: 0x0600193D RID: 6461 RVA: 0x00075A21 File Offset: 0x00073C21
	public bool IsUsingFastActorTriggers()
	{
		return this.m_usingFastActorTriggers;
	}

	// Token: 0x0600193E RID: 6462 RVA: 0x00075A29 File Offset: 0x00073C29
	public void SetUsingFastActorTriggers(bool enable)
	{
		this.m_usingFastActorTriggers = enable;
	}

	// Token: 0x0600193F RID: 6463 RVA: 0x00075A34 File Offset: 0x00073C34
	public bool HasHandPlays()
	{
		if (this.m_options == null)
		{
			return false;
		}
		foreach (Network.Options.Option option in this.m_options.List)
		{
			if (option.Type == Network.Options.Option.OptionType.POWER)
			{
				Entity entity = this.GetEntity(option.Main.ID);
				if (entity != null)
				{
					Card card = entity.GetCard();
					if (!(card == null))
					{
						ZoneHand zoneHand = card.GetZone() as ZoneHand;
						if (!(zoneHand == null))
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06001940 RID: 6464 RVA: 0x00075B0C File Offset: 0x00073D0C
	private void PreprocessRealTimeTagChange(Entity entity, Network.HistTagChange change)
	{
		GAME_TAG tag = (GAME_TAG)change.Tag;
		GAME_TAG game_TAG = tag;
		if (game_TAG == GAME_TAG.PLAYSTATE)
		{
			if (GameUtils.IsGameOverTag(change.Entity, change.Tag, change.Value))
			{
				this.OnRealTimeGameOver(change);
			}
		}
	}

	// Token: 0x06001941 RID: 6465 RVA: 0x00075B58 File Offset: 0x00073D58
	private void PreprocessTagChange(Entity entity, TagDelta change)
	{
		GAME_TAG tag = (GAME_TAG)change.tag;
		GAME_TAG game_TAG = tag;
		switch (game_TAG)
		{
		case GAME_TAG.PLAYSTATE:
		{
			Player player = (Player)entity;
			if (GameUtils.IsGameOverTag(player, change.tag, change.newValue))
			{
				this.OnGameOver((TAG_PLAYSTATE)change.newValue);
			}
			break;
		}
		default:
			if (game_TAG == GAME_TAG.CURRENT_PLAYER)
			{
				if (change.newValue == 1)
				{
					Player player2 = (Player)entity;
					this.OnCurrentPlayerChanged(player2);
				}
			}
			break;
		case GAME_TAG.TURN:
			this.OnTurnChanged(change.oldValue, change.newValue);
			break;
		}
	}

	// Token: 0x06001942 RID: 6466 RVA: 0x00075BFC File Offset: 0x00073DFC
	private void PreprocessEarlyConcedeTagChange(Entity entity, TagDelta change)
	{
		GAME_TAG tag = (GAME_TAG)change.tag;
		GAME_TAG game_TAG = tag;
		if (game_TAG == GAME_TAG.PLAYSTATE)
		{
			Player player = (Player)entity;
			if (GameUtils.IsGameOverTag(player, change.tag, change.newValue))
			{
				this.OnGameOver((TAG_PLAYSTATE)change.newValue);
			}
		}
	}

	// Token: 0x06001943 RID: 6467 RVA: 0x00075C50 File Offset: 0x00073E50
	private void ProcessEarlyConcedeTagChange(Entity entity, TagDelta change)
	{
		GAME_TAG tag = (GAME_TAG)change.tag;
		GAME_TAG game_TAG = tag;
		if (game_TAG == GAME_TAG.PLAYSTATE)
		{
			entity.OnTagChanged(change);
		}
	}

	// Token: 0x06001944 RID: 6468 RVA: 0x00075C7F File Offset: 0x00073E7F
	private void OnRealTimeGameOver(Network.HistTagChange change)
	{
		this.m_realTimeGameOverTagChange = change;
		if (Network.ShouldBeConnectedToAurora())
		{
			BnetPresenceMgr.Get().SetGameFieldBlob(21U, null);
		}
		SpectatorManager.Get().OnRealTimeGameOver();
	}

	// Token: 0x06001945 RID: 6469 RVA: 0x00075CAC File Offset: 0x00073EAC
	private void OnGameOver(TAG_PLAYSTATE playState)
	{
		this.m_gameOver = true;
		this.m_realTimeGameOverTagChange = null;
		this.m_gameEntity.NotifyOfGameOver(playState);
		this.FireGameOverEvent();
	}

	// Token: 0x06001946 RID: 6470 RVA: 0x00075CD9 File Offset: 0x00073ED9
	private void OnCurrentPlayerChanged(Player player)
	{
		this.FireCurrentPlayerChangedEvent(player);
	}

	// Token: 0x06001947 RID: 6471 RVA: 0x00075CE2 File Offset: 0x00073EE2
	private void OnTurnChanged(int oldTurn, int newTurn)
	{
		this.OnTurnChanged_TurnTimer(oldTurn, newTurn);
		this.FireTurnChangedEvent(oldTurn, newTurn);
	}

	// Token: 0x06001948 RID: 6472 RVA: 0x00075CF4 File Offset: 0x00073EF4
	public void AddServerBlockingSpell(Spell spell)
	{
		if (spell == null)
		{
			return;
		}
		if (this.m_serverBlockingSpells.Contains(spell))
		{
			return;
		}
		this.m_serverBlockingSpells.Add(spell);
	}

	// Token: 0x06001949 RID: 6473 RVA: 0x00075D21 File Offset: 0x00073F21
	public bool RemoveServerBlockingSpell(Spell spell)
	{
		return this.m_serverBlockingSpells.Remove(spell);
	}

	// Token: 0x0600194A RID: 6474 RVA: 0x00075D2F File Offset: 0x00073F2F
	public void AddServerBlockingSpellController(SpellController spellController)
	{
		if (spellController == null)
		{
			return;
		}
		if (this.m_serverBlockingSpellControllers.Contains(spellController))
		{
			return;
		}
		this.m_serverBlockingSpellControllers.Add(spellController);
	}

	// Token: 0x0600194B RID: 6475 RVA: 0x00075D5C File Offset: 0x00073F5C
	public bool RemoveServerBlockingSpellController(SpellController spellController)
	{
		return this.m_serverBlockingSpellControllers.Remove(spellController);
	}

	// Token: 0x0600194C RID: 6476 RVA: 0x00075D6C File Offset: 0x00073F6C
	public void DebugNukeServerBlocks()
	{
		while (this.m_serverBlockingSpells.Count > 0)
		{
			Spell spell = this.m_serverBlockingSpells[0];
			spell.OnSpellFinished();
		}
		while (this.m_serverBlockingSpellControllers.Count > 0)
		{
			SpellController spellController = this.m_serverBlockingSpellControllers[0];
			spellController.ForceKill();
		}
		this.m_powerProcessor.ForceStopHistoryBlocking();
		this.m_busy = false;
	}

	// Token: 0x0600194D RID: 6477 RVA: 0x00075DE0 File Offset: 0x00073FE0
	public bool IsBlockingPowerProcessor()
	{
		return this.m_serverBlockingSpells.Count > 0 || this.m_serverBlockingSpellControllers.Count > 0 || this.m_powerProcessor.IsHistoryBlocking();
	}

	// Token: 0x0600194E RID: 6478 RVA: 0x00075E28 File Offset: 0x00074028
	public bool CanProcessPowerQueue()
	{
		return !this.IsBlockingPowerProcessor() && !this.IsBusy() && this.m_powerProcessor.GetCurrentTaskList() == null && this.m_powerProcessor.GetPowerQueue().Count != 0;
	}

	// Token: 0x0600194F RID: 6479 RVA: 0x00075E7C File Offset: 0x0007407C
	private bool CheckBlockingPowerProcessor()
	{
		if (!this.IsBlockingPowerProcessor())
		{
			this.m_blockedSec = 0f;
			return false;
		}
		this.m_blockedSec += Time.deltaTime;
		if (this.ReconnectIfStuck())
		{
			return true;
		}
		this.ReportStuck();
		return true;
	}

	// Token: 0x06001950 RID: 6480 RVA: 0x00075EC8 File Offset: 0x000740C8
	private bool ReconnectIfStuck()
	{
		if (!ReconnectMgr.Get().IsReconnectEnabled())
		{
			return false;
		}
		Network.GameSetup gameSetup = GameMgr.Get().GetGameSetup();
		if (gameSetup.DisconnectWhenStuckSeconds > 0 && this.m_blockedSec < (float)gameSetup.DisconnectWhenStuckSeconds)
		{
			return false;
		}
		string devElapsedTimeString = TimeUtils.GetDevElapsedTimeString(this.m_blockedSec);
		string text = this.BuildServerBlockingCausesString();
		BnetGameAccountId myGameAccountId = BnetPresenceMgr.Get().GetMyGameAccountId();
		BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
		string text2;
		if (myPlayer != null && myPlayer.GetBattleTag() != null)
		{
			text2 = myPlayer.GetBattleTag().ToString();
		}
		else
		{
			text2 = "UNKNOWN";
		}
		string text3 = string.Format("Player {0} (Game Account: {1}) reconnected because they got stuck on client effects for more than {2}.\nCause: {3}", new object[]
		{
			text2,
			myGameAccountId,
			devElapsedTimeString,
			text
		});
		Log.Power.PrintWarning("GameState.ReconnectIfStuck() - Blocked more than {0}. Telemetry Message:\n{1}", new object[]
		{
			devElapsedTimeString,
			text3
		});
		BIReport bireport = BIReport.Get();
		string message = text3;
		bireport.TelemetryWarn(BIReport.TelemetryEvent.GAMEPLAY_STUCK_DISCONNECT, 0, message, -1);
		Network.DisconnectFromGameServer();
		return true;
	}

	// Token: 0x06001951 RID: 6481 RVA: 0x00075FCC File Offset: 0x000741CC
	private void ReportStuck()
	{
		if (this.m_blockedSec < 10f)
		{
			return;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float num = realtimeSinceStartup - this.m_lastBlockedReportTimestamp;
		if (num < 3f)
		{
			return;
		}
		this.m_lastBlockedReportTimestamp = realtimeSinceStartup;
		string devElapsedTimeString = TimeUtils.GetDevElapsedTimeString(this.m_blockedSec);
		string text = this.BuildServerBlockingCausesString();
		Log.Power.PrintWarning("GameState.ReportStuck() - Stuck for {0}. {1}", new object[]
		{
			devElapsedTimeString,
			text
		});
	}

	// Token: 0x06001952 RID: 6482 RVA: 0x0007603C File Offset: 0x0007423C
	private string BuildServerBlockingCausesString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		this.AppendServerBlockingSection<Spell>(stringBuilder, "Spells:", this.m_serverBlockingSpells, new GameState.AppendBlockingServerItemCallback<Spell>(this.AppendServerBlockingSpell), ref num);
		this.AppendServerBlockingSection<SpellController>(stringBuilder, "SpellControllers:", this.m_serverBlockingSpellControllers, new GameState.AppendBlockingServerItemCallback<SpellController>(this.AppendServerBlockingSpellController), ref num);
		this.AppendServerBlockingHistory(stringBuilder, ref num);
		if (this.m_busy)
		{
			if (num > 0)
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append("Busy=true");
			num++;
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06001953 RID: 6483 RVA: 0x000760CC File Offset: 0x000742CC
	private void AppendServerBlockingSection<T>(StringBuilder builder, string sectionPrefix, List<T> items, GameState.AppendBlockingServerItemCallback<T> itemCallback, ref int sectionCount) where T : Component
	{
		if (items.Count == 0)
		{
			return;
		}
		if (sectionCount > 0)
		{
			builder.Append(' ');
		}
		builder.Append('{');
		builder.Append(sectionPrefix);
		for (int i = 0; i < items.Count; i++)
		{
			builder.Append(' ');
			if (itemCallback == null)
			{
				T t = items[i];
				builder.Append(t.name);
			}
			else
			{
				itemCallback(builder, items[i]);
			}
		}
		builder.Append('}');
		sectionCount++;
	}

	// Token: 0x06001954 RID: 6484 RVA: 0x00076170 File Offset: 0x00074370
	private void AppendServerBlockingSpell(StringBuilder builder, Spell spell)
	{
		builder.Append('[');
		builder.Append(spell.name);
		builder.Append(' ');
		builder.AppendFormat("Source: {0}", spell.GetSource());
		builder.Append(' ');
		builder.Append("Targets:");
		List<GameObject> targets = spell.GetTargets();
		if (targets.Count == 0)
		{
			builder.Append(' ');
			builder.Append("none");
		}
		else
		{
			for (int i = 0; i < targets.Count; i++)
			{
				builder.Append(' ');
				GameObject gameObject = targets[i];
				builder.Append(gameObject.ToString());
			}
		}
		builder.Append(']');
	}

	// Token: 0x06001955 RID: 6485 RVA: 0x00076230 File Offset: 0x00074430
	private void AppendServerBlockingSpellController(StringBuilder builder, SpellController spellController)
	{
		builder.Append('[');
		builder.Append(spellController.name);
		builder.Append(' ');
		builder.AppendFormat("Source: {0}", spellController.GetSource());
		builder.Append(' ');
		builder.Append("Targets:");
		List<Card> targets = spellController.GetTargets();
		if (targets.Count == 0)
		{
			builder.Append(' ');
			builder.Append("none");
		}
		else
		{
			for (int i = 0; i < targets.Count; i++)
			{
				builder.Append(' ');
				Card card = targets[i];
				builder.Append(card.ToString());
			}
		}
		builder.Append(']');
	}

	// Token: 0x06001956 RID: 6486 RVA: 0x000762F0 File Offset: 0x000744F0
	private void AppendServerBlockingHistory(StringBuilder builder, ref int sectionCount)
	{
		if (!this.m_powerProcessor.IsHistoryBlocking())
		{
			return;
		}
		Entity pendingBigCardEntity = HistoryManager.Get().GetPendingBigCardEntity();
		PowerTaskList historyBlockingTaskList = this.m_powerProcessor.GetHistoryBlockingTaskList();
		PowerTaskList currentTaskList = this.m_powerProcessor.GetCurrentTaskList();
		if (sectionCount > 0)
		{
			builder.Append(' ');
		}
		builder.Append("History: ");
		builder.Append('{');
		builder.AppendFormat("PendingBigCard: {0}", pendingBigCardEntity);
		builder.Append(' ');
		builder.AppendFormat("BlockingTaskList: ", new object[0]);
		this.PrintBlockingTaskList(builder, historyBlockingTaskList);
		builder.Append(' ');
		builder.AppendFormat("CurrentTaskList: ", new object[0]);
		this.PrintBlockingTaskList(builder, currentTaskList);
		builder.Append('}');
		sectionCount++;
	}

	// Token: 0x06001957 RID: 6487 RVA: 0x000763BC File Offset: 0x000745BC
	public static bool RegisterGameStateInitializedListener(GameState.GameStateInitializedCallback callback, object userData = null)
	{
		if (callback == null)
		{
			return false;
		}
		GameState.GameStateInitializedListener gameStateInitializedListener = new GameState.GameStateInitializedListener();
		gameStateInitializedListener.SetCallback(callback);
		gameStateInitializedListener.SetUserData(userData);
		if (GameState.s_gameStateInitializedListeners == null)
		{
			GameState.s_gameStateInitializedListeners = new List<GameState.GameStateInitializedListener>();
		}
		else if (GameState.s_gameStateInitializedListeners.Contains(gameStateInitializedListener))
		{
			return false;
		}
		GameState.s_gameStateInitializedListeners.Add(gameStateInitializedListener);
		return true;
	}

	// Token: 0x06001958 RID: 6488 RVA: 0x0007641C File Offset: 0x0007461C
	public static bool UnregisterGameStateInitializedListener(GameState.GameStateInitializedCallback callback, object userData = null)
	{
		if (callback == null || GameState.s_gameStateInitializedListeners == null)
		{
			return false;
		}
		GameState.GameStateInitializedListener gameStateInitializedListener = new GameState.GameStateInitializedListener();
		gameStateInitializedListener.SetCallback(callback);
		gameStateInitializedListener.SetUserData(userData);
		return GameState.s_gameStateInitializedListeners.Remove(gameStateInitializedListener);
	}

	// Token: 0x06001959 RID: 6489 RVA: 0x0007645A File Offset: 0x0007465A
	public bool RegisterCreateGameListener(GameState.CreateGameCallback callback)
	{
		return this.RegisterCreateGameListener(callback, null);
	}

	// Token: 0x0600195A RID: 6490 RVA: 0x00076464 File Offset: 0x00074664
	public bool RegisterCreateGameListener(GameState.CreateGameCallback callback, object userData)
	{
		GameState.CreateGameListener createGameListener = new GameState.CreateGameListener();
		createGameListener.SetCallback(callback);
		createGameListener.SetUserData(userData);
		if (this.m_createGameListeners.Contains(createGameListener))
		{
			return false;
		}
		this.m_createGameListeners.Add(createGameListener);
		return true;
	}

	// Token: 0x0600195B RID: 6491 RVA: 0x000764A5 File Offset: 0x000746A5
	public bool UnregisterCreateGameListener(GameState.CreateGameCallback callback)
	{
		return this.UnregisterCreateGameListener(callback, null);
	}

	// Token: 0x0600195C RID: 6492 RVA: 0x000764B0 File Offset: 0x000746B0
	public bool UnregisterCreateGameListener(GameState.CreateGameCallback callback, object userData)
	{
		GameState.CreateGameListener createGameListener = new GameState.CreateGameListener();
		createGameListener.SetCallback(callback);
		createGameListener.SetUserData(userData);
		return this.m_createGameListeners.Remove(createGameListener);
	}

	// Token: 0x0600195D RID: 6493 RVA: 0x000764DD File Offset: 0x000746DD
	public bool RegisterOptionsReceivedListener(GameState.OptionsReceivedCallback callback)
	{
		return this.RegisterOptionsReceivedListener(callback, null);
	}

	// Token: 0x0600195E RID: 6494 RVA: 0x000764E8 File Offset: 0x000746E8
	public bool RegisterOptionsReceivedListener(GameState.OptionsReceivedCallback callback, object userData)
	{
		GameState.OptionsReceivedListener optionsReceivedListener = new GameState.OptionsReceivedListener();
		optionsReceivedListener.SetCallback(callback);
		optionsReceivedListener.SetUserData(userData);
		if (this.m_optionsReceivedListeners.Contains(optionsReceivedListener))
		{
			return false;
		}
		this.m_optionsReceivedListeners.Add(optionsReceivedListener);
		return true;
	}

	// Token: 0x0600195F RID: 6495 RVA: 0x00076529 File Offset: 0x00074729
	public bool UnregisterOptionsReceivedListener(GameState.OptionsReceivedCallback callback)
	{
		return this.UnregisterOptionsReceivedListener(callback, null);
	}

	// Token: 0x06001960 RID: 6496 RVA: 0x00076534 File Offset: 0x00074734
	public bool UnregisterOptionsReceivedListener(GameState.OptionsReceivedCallback callback, object userData)
	{
		GameState.OptionsReceivedListener optionsReceivedListener = new GameState.OptionsReceivedListener();
		optionsReceivedListener.SetCallback(callback);
		optionsReceivedListener.SetUserData(userData);
		return this.m_optionsReceivedListeners.Remove(optionsReceivedListener);
	}

	// Token: 0x06001961 RID: 6497 RVA: 0x00076564 File Offset: 0x00074764
	public bool RegisterOptionsSentListener(GameState.OptionsSentCallback callback, object userData = null)
	{
		GameState.OptionsSentListener optionsSentListener = new GameState.OptionsSentListener();
		optionsSentListener.SetCallback(callback);
		optionsSentListener.SetUserData(userData);
		if (this.m_optionsSentListeners.Contains(optionsSentListener))
		{
			return false;
		}
		this.m_optionsSentListeners.Add(optionsSentListener);
		return true;
	}

	// Token: 0x06001962 RID: 6498 RVA: 0x000765A8 File Offset: 0x000747A8
	public bool UnregisterOptionsReceivedListener(GameState.OptionsSentCallback callback, object userData = null)
	{
		GameState.OptionsSentListener optionsSentListener = new GameState.OptionsSentListener();
		optionsSentListener.SetCallback(callback);
		optionsSentListener.SetUserData(userData);
		return this.m_optionsSentListeners.Remove(optionsSentListener);
	}

	// Token: 0x06001963 RID: 6499 RVA: 0x000765D8 File Offset: 0x000747D8
	public bool RegisterOptionRejectedListener(GameState.OptionRejectedCallback callback, object userData = null)
	{
		GameState.OptionRejectedListener optionRejectedListener = new GameState.OptionRejectedListener();
		optionRejectedListener.SetCallback(callback);
		optionRejectedListener.SetUserData(userData);
		if (this.m_optionRejectedListeners.Contains(optionRejectedListener))
		{
			return false;
		}
		this.m_optionRejectedListeners.Add(optionRejectedListener);
		return true;
	}

	// Token: 0x06001964 RID: 6500 RVA: 0x0007661C File Offset: 0x0007481C
	public bool UnregisterOptionRejectedListener(GameState.OptionRejectedCallback callback, object userData = null)
	{
		GameState.OptionRejectedListener optionRejectedListener = new GameState.OptionRejectedListener();
		optionRejectedListener.SetCallback(callback);
		optionRejectedListener.SetUserData(userData);
		return this.m_optionRejectedListeners.Remove(optionRejectedListener);
	}

	// Token: 0x06001965 RID: 6501 RVA: 0x00076649 File Offset: 0x00074849
	public bool RegisterEntityChoicesReceivedListener(GameState.EntityChoicesReceivedCallback callback)
	{
		return this.RegisterEntityChoicesReceivedListener(callback, null);
	}

	// Token: 0x06001966 RID: 6502 RVA: 0x00076654 File Offset: 0x00074854
	public bool RegisterEntityChoicesReceivedListener(GameState.EntityChoicesReceivedCallback callback, object userData)
	{
		GameState.EntityChoicesReceivedListener entityChoicesReceivedListener = new GameState.EntityChoicesReceivedListener();
		entityChoicesReceivedListener.SetCallback(callback);
		entityChoicesReceivedListener.SetUserData(userData);
		if (this.m_entityChoicesReceivedListeners.Contains(entityChoicesReceivedListener))
		{
			return false;
		}
		this.m_entityChoicesReceivedListeners.Add(entityChoicesReceivedListener);
		return true;
	}

	// Token: 0x06001967 RID: 6503 RVA: 0x00076695 File Offset: 0x00074895
	public bool UnregisterEntityChoicesReceivedListener(GameState.EntityChoicesReceivedCallback callback)
	{
		return this.UnregisterEntityChoicesReceivedListener(callback, null);
	}

	// Token: 0x06001968 RID: 6504 RVA: 0x000766A0 File Offset: 0x000748A0
	public bool UnregisterEntityChoicesReceivedListener(GameState.EntityChoicesReceivedCallback callback, object userData)
	{
		GameState.EntityChoicesReceivedListener entityChoicesReceivedListener = new GameState.EntityChoicesReceivedListener();
		entityChoicesReceivedListener.SetCallback(callback);
		entityChoicesReceivedListener.SetUserData(userData);
		return this.m_entityChoicesReceivedListeners.Remove(entityChoicesReceivedListener);
	}

	// Token: 0x06001969 RID: 6505 RVA: 0x000766CD File Offset: 0x000748CD
	public bool RegisterEntitiesChosenReceivedListener(GameState.EntitiesChosenReceivedCallback callback)
	{
		return this.RegisterEntitiesChosenReceivedListener(callback, null);
	}

	// Token: 0x0600196A RID: 6506 RVA: 0x000766D8 File Offset: 0x000748D8
	public bool RegisterEntitiesChosenReceivedListener(GameState.EntitiesChosenReceivedCallback callback, object userData)
	{
		GameState.EntitiesChosenReceivedListener entitiesChosenReceivedListener = new GameState.EntitiesChosenReceivedListener();
		entitiesChosenReceivedListener.SetCallback(callback);
		entitiesChosenReceivedListener.SetUserData(userData);
		if (this.m_entitiesChosenReceivedListeners.Contains(entitiesChosenReceivedListener))
		{
			return false;
		}
		this.m_entitiesChosenReceivedListeners.Add(entitiesChosenReceivedListener);
		return true;
	}

	// Token: 0x0600196B RID: 6507 RVA: 0x00076719 File Offset: 0x00074919
	public bool UnregisterEntitiesChosenReceivedListener(GameState.EntitiesChosenReceivedCallback callback)
	{
		return this.UnregisterEntitiesChosenReceivedListener(callback, null);
	}

	// Token: 0x0600196C RID: 6508 RVA: 0x00076724 File Offset: 0x00074924
	public bool UnregisterEntitiesChosenReceivedListener(GameState.EntitiesChosenReceivedCallback callback, object userData)
	{
		GameState.EntitiesChosenReceivedListener entitiesChosenReceivedListener = new GameState.EntitiesChosenReceivedListener();
		entitiesChosenReceivedListener.SetCallback(callback);
		entitiesChosenReceivedListener.SetUserData(userData);
		return this.m_entitiesChosenReceivedListeners.Remove(entitiesChosenReceivedListener);
	}

	// Token: 0x0600196D RID: 6509 RVA: 0x00076751 File Offset: 0x00074951
	public bool RegisterCurrentPlayerChangedListener(GameState.CurrentPlayerChangedCallback callback)
	{
		return this.RegisterCurrentPlayerChangedListener(callback, null);
	}

	// Token: 0x0600196E RID: 6510 RVA: 0x0007675C File Offset: 0x0007495C
	public bool RegisterCurrentPlayerChangedListener(GameState.CurrentPlayerChangedCallback callback, object userData)
	{
		GameState.CurrentPlayerChangedListener currentPlayerChangedListener = new GameState.CurrentPlayerChangedListener();
		currentPlayerChangedListener.SetCallback(callback);
		currentPlayerChangedListener.SetUserData(userData);
		if (this.m_currentPlayerChangedListeners.Contains(currentPlayerChangedListener))
		{
			return false;
		}
		this.m_currentPlayerChangedListeners.Add(currentPlayerChangedListener);
		return true;
	}

	// Token: 0x0600196F RID: 6511 RVA: 0x0007679D File Offset: 0x0007499D
	public bool UnregisterCurrentPlayerChangedListener(GameState.CurrentPlayerChangedCallback callback)
	{
		return this.UnregisterCurrentPlayerChangedListener(callback, null);
	}

	// Token: 0x06001970 RID: 6512 RVA: 0x000767A8 File Offset: 0x000749A8
	public bool UnregisterCurrentPlayerChangedListener(GameState.CurrentPlayerChangedCallback callback, object userData)
	{
		GameState.CurrentPlayerChangedListener currentPlayerChangedListener = new GameState.CurrentPlayerChangedListener();
		currentPlayerChangedListener.SetCallback(callback);
		currentPlayerChangedListener.SetUserData(userData);
		return this.m_currentPlayerChangedListeners.Remove(currentPlayerChangedListener);
	}

	// Token: 0x06001971 RID: 6513 RVA: 0x000767D5 File Offset: 0x000749D5
	public bool RegisterTurnChangedListener(GameState.TurnChangedCallback callback)
	{
		return this.RegisterTurnChangedListener(callback, null);
	}

	// Token: 0x06001972 RID: 6514 RVA: 0x000767E0 File Offset: 0x000749E0
	public bool RegisterTurnChangedListener(GameState.TurnChangedCallback callback, object userData)
	{
		GameState.TurnChangedListener turnChangedListener = new GameState.TurnChangedListener();
		turnChangedListener.SetCallback(callback);
		turnChangedListener.SetUserData(userData);
		if (this.m_turnChangedListeners.Contains(turnChangedListener))
		{
			return false;
		}
		this.m_turnChangedListeners.Add(turnChangedListener);
		return true;
	}

	// Token: 0x06001973 RID: 6515 RVA: 0x00076821 File Offset: 0x00074A21
	public bool UnregisterTurnChangedListener(GameState.TurnChangedCallback callback)
	{
		return this.UnregisterTurnChangedListener(callback, null);
	}

	// Token: 0x06001974 RID: 6516 RVA: 0x0007682C File Offset: 0x00074A2C
	public bool UnregisterTurnChangedListener(GameState.TurnChangedCallback callback, object userData)
	{
		GameState.TurnChangedListener turnChangedListener = new GameState.TurnChangedListener();
		turnChangedListener.SetCallback(callback);
		turnChangedListener.SetUserData(userData);
		return this.m_turnChangedListeners.Remove(turnChangedListener);
	}

	// Token: 0x06001975 RID: 6517 RVA: 0x0007685C File Offset: 0x00074A5C
	public bool RegisterFriendlyTurnStartedListener(GameState.FriendlyTurnStartedCallback callback, object userData = null)
	{
		GameState.FriendlyTurnStartedListener friendlyTurnStartedListener = new GameState.FriendlyTurnStartedListener();
		friendlyTurnStartedListener.SetCallback(callback);
		friendlyTurnStartedListener.SetUserData(userData);
		if (this.m_friendlyTurnStartedListeners.Contains(friendlyTurnStartedListener))
		{
			return false;
		}
		this.m_friendlyTurnStartedListeners.Add(friendlyTurnStartedListener);
		return true;
	}

	// Token: 0x06001976 RID: 6518 RVA: 0x000768A0 File Offset: 0x00074AA0
	public bool UnregisterFriendlyTurnStartedListener(GameState.FriendlyTurnStartedCallback callback, object userData = null)
	{
		GameState.FriendlyTurnStartedListener friendlyTurnStartedListener = new GameState.FriendlyTurnStartedListener();
		friendlyTurnStartedListener.SetCallback(callback);
		friendlyTurnStartedListener.SetUserData(userData);
		return this.m_friendlyTurnStartedListeners.Remove(friendlyTurnStartedListener);
	}

	// Token: 0x06001977 RID: 6519 RVA: 0x000768CD File Offset: 0x00074ACD
	public bool RegisterTurnTimerUpdateListener(GameState.TurnTimerUpdateCallback callback)
	{
		return this.RegisterTurnTimerUpdateListener(callback, null);
	}

	// Token: 0x06001978 RID: 6520 RVA: 0x000768D8 File Offset: 0x00074AD8
	public bool RegisterTurnTimerUpdateListener(GameState.TurnTimerUpdateCallback callback, object userData)
	{
		GameState.TurnTimerUpdateListener turnTimerUpdateListener = new GameState.TurnTimerUpdateListener();
		turnTimerUpdateListener.SetCallback(callback);
		turnTimerUpdateListener.SetUserData(userData);
		if (this.m_turnTimerUpdateListeners.Contains(turnTimerUpdateListener))
		{
			return false;
		}
		this.m_turnTimerUpdateListeners.Add(turnTimerUpdateListener);
		return true;
	}

	// Token: 0x06001979 RID: 6521 RVA: 0x00076919 File Offset: 0x00074B19
	public bool UnregisterTurnTimerUpdateListener(GameState.TurnTimerUpdateCallback callback)
	{
		return this.UnregisterTurnTimerUpdateListener(callback, null);
	}

	// Token: 0x0600197A RID: 6522 RVA: 0x00076924 File Offset: 0x00074B24
	public bool UnregisterTurnTimerUpdateListener(GameState.TurnTimerUpdateCallback callback, object userData)
	{
		GameState.TurnTimerUpdateListener turnTimerUpdateListener = new GameState.TurnTimerUpdateListener();
		turnTimerUpdateListener.SetCallback(callback);
		turnTimerUpdateListener.SetUserData(userData);
		return this.m_turnTimerUpdateListeners.Remove(turnTimerUpdateListener);
	}

	// Token: 0x0600197B RID: 6523 RVA: 0x00076951 File Offset: 0x00074B51
	public bool RegisterMulliganTimerUpdateListener(GameState.TurnTimerUpdateCallback callback)
	{
		return this.RegisterMulliganTimerUpdateListener(callback, null);
	}

	// Token: 0x0600197C RID: 6524 RVA: 0x0007695C File Offset: 0x00074B5C
	public bool RegisterMulliganTimerUpdateListener(GameState.TurnTimerUpdateCallback callback, object userData)
	{
		GameState.TurnTimerUpdateListener turnTimerUpdateListener = new GameState.TurnTimerUpdateListener();
		turnTimerUpdateListener.SetCallback(callback);
		turnTimerUpdateListener.SetUserData(userData);
		if (this.m_mulliganTimerUpdateListeners.Contains(turnTimerUpdateListener))
		{
			return false;
		}
		this.m_mulliganTimerUpdateListeners.Add(turnTimerUpdateListener);
		return true;
	}

	// Token: 0x0600197D RID: 6525 RVA: 0x0007699D File Offset: 0x00074B9D
	public bool UnregisterMulliganTimerUpdateListener(GameState.TurnTimerUpdateCallback callback)
	{
		return this.UnregisterMulliganTimerUpdateListener(callback, null);
	}

	// Token: 0x0600197E RID: 6526 RVA: 0x000769A8 File Offset: 0x00074BA8
	public bool UnregisterMulliganTimerUpdateListener(GameState.TurnTimerUpdateCallback callback, object userData)
	{
		GameState.TurnTimerUpdateListener turnTimerUpdateListener = new GameState.TurnTimerUpdateListener();
		turnTimerUpdateListener.SetCallback(callback);
		turnTimerUpdateListener.SetUserData(userData);
		return this.m_mulliganTimerUpdateListeners.Remove(turnTimerUpdateListener);
	}

	// Token: 0x0600197F RID: 6527 RVA: 0x000769D8 File Offset: 0x00074BD8
	public bool RegisterSpectatorNotifyListener(GameState.SpectatorNotifyEventCallback callback, object userData = null)
	{
		GameState.SpectatorNotifyListener spectatorNotifyListener = new GameState.SpectatorNotifyListener();
		spectatorNotifyListener.SetCallback(callback);
		spectatorNotifyListener.SetUserData(userData);
		if (this.m_spectatorNotifyListeners.Contains(spectatorNotifyListener))
		{
			return false;
		}
		this.m_spectatorNotifyListeners.Add(spectatorNotifyListener);
		return true;
	}

	// Token: 0x06001980 RID: 6528 RVA: 0x00076A1C File Offset: 0x00074C1C
	public bool UnregisterSpectatorNotifyListener(GameState.SpectatorNotifyEventCallback callback, object userData = null)
	{
		GameState.SpectatorNotifyListener spectatorNotifyListener = new GameState.SpectatorNotifyListener();
		spectatorNotifyListener.SetCallback(callback);
		spectatorNotifyListener.SetUserData(userData);
		return this.m_spectatorNotifyListeners.Remove(spectatorNotifyListener);
	}

	// Token: 0x06001981 RID: 6529 RVA: 0x00076A4C File Offset: 0x00074C4C
	public bool RegisterGameOverListener(GameState.GameOverCallback callback, object userData = null)
	{
		GameState.GameOverListener gameOverListener = new GameState.GameOverListener();
		gameOverListener.SetCallback(callback);
		gameOverListener.SetUserData(userData);
		if (this.m_gameOverListeners.Contains(gameOverListener))
		{
			return false;
		}
		this.m_gameOverListeners.Add(gameOverListener);
		return true;
	}

	// Token: 0x06001982 RID: 6530 RVA: 0x00076A90 File Offset: 0x00074C90
	public bool UnregisterGameOverListener(GameState.GameOverCallback callback, object userData = null)
	{
		GameState.GameOverListener gameOverListener = new GameState.GameOverListener();
		gameOverListener.SetCallback(callback);
		gameOverListener.SetUserData(userData);
		return this.m_gameOverListeners.Remove(gameOverListener);
	}

	// Token: 0x06001983 RID: 6531 RVA: 0x00076AC0 File Offset: 0x00074CC0
	public bool RegisterHeroChangedListener(GameState.HeroChangedCallback callback, object userData = null)
	{
		GameState.HeroChangedListener heroChangedListener = new GameState.HeroChangedListener();
		heroChangedListener.SetCallback(callback);
		heroChangedListener.SetUserData(userData);
		if (this.m_heroChangedListeners.Contains(heroChangedListener))
		{
			return false;
		}
		this.m_heroChangedListeners.Add(heroChangedListener);
		return true;
	}

	// Token: 0x06001984 RID: 6532 RVA: 0x00076B04 File Offset: 0x00074D04
	public bool UnregisterHeroChangedListener(GameState.HeroChangedCallback callback, object userData = null)
	{
		GameState.HeroChangedListener heroChangedListener = new GameState.HeroChangedListener();
		heroChangedListener.SetCallback(callback);
		heroChangedListener.SetUserData(userData);
		return this.m_heroChangedListeners.Remove(heroChangedListener);
	}

	// Token: 0x06001985 RID: 6533 RVA: 0x00076B34 File Offset: 0x00074D34
	private static void FireGameStateInitializedEvent()
	{
		if (GameState.s_gameStateInitializedListeners == null)
		{
			return;
		}
		GameState.GameStateInitializedListener[] array = GameState.s_gameStateInitializedListeners.ToArray();
		foreach (GameState.GameStateInitializedListener gameStateInitializedListener in array)
		{
			gameStateInitializedListener.Fire(GameState.s_instance);
		}
	}

	// Token: 0x06001986 RID: 6534 RVA: 0x00076B7C File Offset: 0x00074D7C
	private void FireCreateGameEvent()
	{
		GameState.CreateGameListener[] array = this.m_createGameListeners.ToArray();
		foreach (GameState.CreateGameListener createGameListener in array)
		{
			createGameListener.Fire(this.m_createGamePhase);
		}
	}

	// Token: 0x06001987 RID: 6535 RVA: 0x00076BBC File Offset: 0x00074DBC
	private void FireOptionsReceivedEvent()
	{
		GameState.OptionsReceivedListener[] array = this.m_optionsReceivedListeners.ToArray();
		foreach (GameState.OptionsReceivedListener optionsReceivedListener in array)
		{
			optionsReceivedListener.Fire();
		}
	}

	// Token: 0x06001988 RID: 6536 RVA: 0x00076BF8 File Offset: 0x00074DF8
	private void FireOptionsSentEvent(Network.Options.Option option)
	{
		GameState.OptionsSentListener[] array = this.m_optionsSentListeners.ToArray();
		foreach (GameState.OptionsSentListener optionsSentListener in array)
		{
			optionsSentListener.Fire(option);
		}
	}

	// Token: 0x06001989 RID: 6537 RVA: 0x00076C34 File Offset: 0x00074E34
	private void FireOptionRejectedEvent(Network.Options.Option option)
	{
		GameState.OptionRejectedListener[] array = this.m_optionRejectedListeners.ToArray();
		foreach (GameState.OptionRejectedListener optionRejectedListener in array)
		{
			optionRejectedListener.Fire(option);
		}
	}

	// Token: 0x0600198A RID: 6538 RVA: 0x00076C70 File Offset: 0x00074E70
	private void FireEntityChoicesReceivedEvent(Network.EntityChoices choices, PowerTaskList preChoiceTaskList)
	{
		GameState.EntityChoicesReceivedListener[] array = this.m_entityChoicesReceivedListeners.ToArray();
		foreach (GameState.EntityChoicesReceivedListener entityChoicesReceivedListener in array)
		{
			entityChoicesReceivedListener.Fire(choices, preChoiceTaskList);
		}
	}

	// Token: 0x0600198B RID: 6539 RVA: 0x00076CAC File Offset: 0x00074EAC
	private bool FireEntitiesChosenReceivedEvent(Network.EntitiesChosen chosen)
	{
		GameState.EntitiesChosenReceivedListener[] array = this.m_entitiesChosenReceivedListeners.ToArray();
		Network.EntityChoices entityChoices = this.GetEntityChoices(chosen.PlayerId);
		bool flag = false;
		foreach (GameState.EntitiesChosenReceivedListener entitiesChosenReceivedListener in array)
		{
			flag = (entitiesChosenReceivedListener.Fire(chosen, entityChoices) || flag);
		}
		return flag;
	}

	// Token: 0x0600198C RID: 6540 RVA: 0x00076D0C File Offset: 0x00074F0C
	private void FireTurnChangedEvent(int oldTurn, int newTurn)
	{
		GameState.TurnChangedListener[] array = this.m_turnChangedListeners.ToArray();
		foreach (GameState.TurnChangedListener turnChangedListener in array)
		{
			turnChangedListener.Fire(oldTurn, newTurn);
		}
	}

	// Token: 0x0600198D RID: 6541 RVA: 0x00076D48 File Offset: 0x00074F48
	public void FireFriendlyTurnStartedEvent()
	{
		this.m_gameEntity.NotifyOfStartOfTurnEventsFinished();
		GameState.FriendlyTurnStartedListener[] array = this.m_friendlyTurnStartedListeners.ToArray();
		foreach (GameState.FriendlyTurnStartedListener friendlyTurnStartedListener in array)
		{
			friendlyTurnStartedListener.Fire();
		}
	}

	// Token: 0x0600198E RID: 6542 RVA: 0x00076D8C File Offset: 0x00074F8C
	private void FireTurnTimerUpdateEvent(TurnTimerUpdate update)
	{
		GameState.TurnTimerUpdateListener[] array;
		if (this.IsMulliganManagerActive())
		{
			array = this.m_mulliganTimerUpdateListeners.ToArray();
		}
		else
		{
			array = this.m_turnTimerUpdateListeners.ToArray();
		}
		foreach (GameState.TurnTimerUpdateListener turnTimerUpdateListener in array)
		{
			turnTimerUpdateListener.Fire(update);
		}
	}

	// Token: 0x0600198F RID: 6543 RVA: 0x00076DE4 File Offset: 0x00074FE4
	private void FireCurrentPlayerChangedEvent(Player player)
	{
		GameState.CurrentPlayerChangedListener[] array = this.m_currentPlayerChangedListeners.ToArray();
		foreach (GameState.CurrentPlayerChangedListener currentPlayerChangedListener in array)
		{
			currentPlayerChangedListener.Fire(player);
		}
	}

	// Token: 0x06001990 RID: 6544 RVA: 0x00076E20 File Offset: 0x00075020
	private void FireSpectatorNotifyEvent(SpectatorNotify notify)
	{
		GameState.SpectatorNotifyListener[] array = this.m_spectatorNotifyListeners.ToArray();
		foreach (GameState.SpectatorNotifyListener spectatorNotifyListener in array)
		{
			spectatorNotifyListener.Fire(notify);
		}
	}

	// Token: 0x06001991 RID: 6545 RVA: 0x00076E5C File Offset: 0x0007505C
	private void FireGameOverEvent()
	{
		GameState.GameOverListener[] array = this.m_gameOverListeners.ToArray();
		foreach (GameState.GameOverListener gameOverListener in array)
		{
			gameOverListener.Fire();
		}
	}

	// Token: 0x06001992 RID: 6546 RVA: 0x00076E98 File Offset: 0x00075098
	public void FireHeroChangedEvent(Player player)
	{
		GameState.HeroChangedListener[] array = this.m_heroChangedListeners.ToArray();
		foreach (GameState.HeroChangedListener heroChangedListener in array)
		{
			heroChangedListener.Fire(player);
		}
	}

	// Token: 0x06001993 RID: 6547 RVA: 0x00076ED2 File Offset: 0x000750D2
	public GameState.ResponseMode GetResponseMode()
	{
		return this.m_responseMode;
	}

	// Token: 0x06001994 RID: 6548 RVA: 0x00076EDC File Offset: 0x000750DC
	public Network.EntityChoices GetFriendlyEntityChoices()
	{
		int friendlyPlayerId = this.GetFriendlyPlayerId();
		return this.GetEntityChoices(friendlyPlayerId);
	}

	// Token: 0x06001995 RID: 6549 RVA: 0x00076EF8 File Offset: 0x000750F8
	public Network.EntityChoices GetOpponentEntityChoices()
	{
		int opposingPlayerId = this.GetOpposingPlayerId();
		return this.GetEntityChoices(opposingPlayerId);
	}

	// Token: 0x06001996 RID: 6550 RVA: 0x00076F14 File Offset: 0x00075114
	public Network.EntityChoices GetEntityChoices(int playerId)
	{
		Network.EntityChoices result;
		this.m_choicesMap.TryGetValue(playerId, out result);
		return result;
	}

	// Token: 0x06001997 RID: 6551 RVA: 0x00076F31 File Offset: 0x00075131
	public Map<int, Network.EntityChoices> GetEntityChoicesMap()
	{
		return this.m_choicesMap;
	}

	// Token: 0x06001998 RID: 6552 RVA: 0x00076F3C File Offset: 0x0007513C
	public bool IsChoosableEntity(Entity entity)
	{
		Network.EntityChoices friendlyEntityChoices = this.GetFriendlyEntityChoices();
		return friendlyEntityChoices != null && friendlyEntityChoices.Entities.Contains(entity.GetEntityId());
	}

	// Token: 0x06001999 RID: 6553 RVA: 0x00076F6C File Offset: 0x0007516C
	public bool IsChosenEntity(Entity entity)
	{
		return this.GetFriendlyEntityChoices() != null && this.m_chosenEntities.Contains(entity);
	}

	// Token: 0x0600199A RID: 6554 RVA: 0x00076F94 File Offset: 0x00075194
	public bool IsValidOptionTarget(Entity entity)
	{
		Network.Options.Option.SubOption selectedNetworkSubOption = this.GetSelectedNetworkSubOption();
		return selectedNetworkSubOption != null && selectedNetworkSubOption.Targets != null && selectedNetworkSubOption.Targets.Contains(entity.GetEntityId());
	}

	// Token: 0x0600199B RID: 6555 RVA: 0x00076FCC File Offset: 0x000751CC
	public bool AddChosenEntity(Entity entity)
	{
		if (this.m_chosenEntities.Contains(entity))
		{
			return false;
		}
		this.m_chosenEntities.Add(entity);
		Card card = entity.GetCard();
		if (card != null)
		{
			card.UpdateActorState();
		}
		return true;
	}

	// Token: 0x0600199C RID: 6556 RVA: 0x00077014 File Offset: 0x00075214
	public bool RemoveChosenEntity(Entity entity)
	{
		if (!this.m_chosenEntities.Remove(entity))
		{
			return false;
		}
		Card card = entity.GetCard();
		if (card != null)
		{
			card.UpdateActorState();
		}
		return true;
	}

	// Token: 0x0600199D RID: 6557 RVA: 0x0007704E File Offset: 0x0007524E
	public List<Entity> GetChosenEntities()
	{
		return this.m_chosenEntities;
	}

	// Token: 0x0600199E RID: 6558 RVA: 0x00077056 File Offset: 0x00075256
	public Network.Options GetOptionsPacket()
	{
		return this.m_options;
	}

	// Token: 0x0600199F RID: 6559 RVA: 0x0007705E File Offset: 0x0007525E
	public void EnterChoiceMode()
	{
		this.m_responseMode = GameState.ResponseMode.CHOICE;
		this.UpdateOptionHighlights();
		this.UpdateChoiceHighlights();
	}

	// Token: 0x060019A0 RID: 6560 RVA: 0x00077074 File Offset: 0x00075274
	public void EnterMainOptionMode()
	{
		GameState.ResponseMode responseMode = this.m_responseMode;
		this.m_responseMode = GameState.ResponseMode.OPTION;
		if (responseMode == GameState.ResponseMode.SUB_OPTION)
		{
			Network.Options.Option option = this.m_options.List[this.m_selectedOption.m_main];
			this.UpdateSubOptionHighlights(option);
		}
		else if (responseMode == GameState.ResponseMode.OPTION_TARGET)
		{
			Network.Options.Option option2 = this.m_options.List[this.m_selectedOption.m_main];
			this.UpdateTargetHighlights(option2.Main);
			if (this.m_selectedOption.m_sub != -1)
			{
				Network.Options.Option.SubOption subOption = option2.Subs[this.m_selectedOption.m_sub];
				this.UpdateTargetHighlights(subOption);
			}
		}
		this.UpdateOptionHighlights(this.m_lastOptions);
		this.UpdateOptionHighlights();
		this.m_selectedOption.Clear();
	}

	// Token: 0x060019A1 RID: 6561 RVA: 0x0007713C File Offset: 0x0007533C
	public void EnterSubOptionMode()
	{
		Network.Options.Option option = this.m_options.List[this.m_selectedOption.m_main];
		if (this.m_responseMode == GameState.ResponseMode.OPTION)
		{
			this.m_responseMode = GameState.ResponseMode.SUB_OPTION;
			this.UpdateOptionHighlights();
		}
		else if (this.m_responseMode == GameState.ResponseMode.OPTION_TARGET)
		{
			this.m_responseMode = GameState.ResponseMode.SUB_OPTION;
			Network.Options.Option.SubOption subOption = option.Subs[this.m_selectedOption.m_sub];
			this.UpdateTargetHighlights(subOption);
		}
		this.UpdateSubOptionHighlights(option);
	}

	// Token: 0x060019A2 RID: 6562 RVA: 0x000771BC File Offset: 0x000753BC
	public void EnterOptionTargetMode()
	{
		if (this.m_responseMode == GameState.ResponseMode.OPTION)
		{
			this.m_responseMode = GameState.ResponseMode.OPTION_TARGET;
			this.UpdateOptionHighlights();
			Network.Options.Option option = this.m_options.List[this.m_selectedOption.m_main];
			this.UpdateTargetHighlights(option.Main);
		}
		else if (this.m_responseMode == GameState.ResponseMode.SUB_OPTION)
		{
			this.m_responseMode = GameState.ResponseMode.OPTION_TARGET;
			Network.Options.Option option2 = this.m_options.List[this.m_selectedOption.m_main];
			this.UpdateSubOptionHighlights(option2);
			Network.Options.Option.SubOption subOption = option2.Subs[this.m_selectedOption.m_sub];
			this.UpdateTargetHighlights(subOption);
		}
	}

	// Token: 0x060019A3 RID: 6563 RVA: 0x00077264 File Offset: 0x00075464
	public void CancelCurrentOptionMode()
	{
		if (this.IsInTargetMode())
		{
			this.GetGameEntity().NotifyOfTargetModeCancelled();
		}
		this.CancelSelectedOptionProposedMana();
		this.EnterMainOptionMode();
	}

	// Token: 0x060019A4 RID: 6564 RVA: 0x00077293 File Offset: 0x00075493
	public bool IsInMainOptionMode()
	{
		return this.m_responseMode == GameState.ResponseMode.OPTION;
	}

	// Token: 0x060019A5 RID: 6565 RVA: 0x0007729E File Offset: 0x0007549E
	public bool IsInSubOptionMode()
	{
		return this.m_responseMode == GameState.ResponseMode.SUB_OPTION;
	}

	// Token: 0x060019A6 RID: 6566 RVA: 0x000772A9 File Offset: 0x000754A9
	public bool IsInTargetMode()
	{
		return this.m_responseMode == GameState.ResponseMode.OPTION_TARGET;
	}

	// Token: 0x060019A7 RID: 6567 RVA: 0x000772B4 File Offset: 0x000754B4
	public bool IsInChoiceMode()
	{
		return this.m_responseMode == GameState.ResponseMode.CHOICE;
	}

	// Token: 0x060019A8 RID: 6568 RVA: 0x000772C0 File Offset: 0x000754C0
	public void SetSelectedOption(ChooseOption packet)
	{
		this.m_selectedOption.m_main = packet.Index;
		this.m_selectedOption.m_sub = packet.SubOption;
		this.m_selectedOption.m_target = packet.Target;
		this.m_selectedOption.m_position = packet.Position;
	}

	// Token: 0x060019A9 RID: 6569 RVA: 0x00077314 File Offset: 0x00075514
	public void SetChosenEntities(ChooseEntities packet)
	{
		this.m_chosenEntities.Clear();
		foreach (int id in packet.Entities)
		{
			Entity entity = this.GetEntity(id);
			if (entity != null)
			{
				this.m_chosenEntities.Add(entity);
			}
		}
	}

	// Token: 0x060019AA RID: 6570 RVA: 0x0007738C File Offset: 0x0007558C
	public void SetSelectedOption(int index)
	{
		this.m_selectedOption.m_main = index;
	}

	// Token: 0x060019AB RID: 6571 RVA: 0x0007739A File Offset: 0x0007559A
	public int GetSelectedOption()
	{
		return this.m_selectedOption.m_main;
	}

	// Token: 0x060019AC RID: 6572 RVA: 0x000773A7 File Offset: 0x000755A7
	public void SetSelectedSubOption(int index)
	{
		this.m_selectedOption.m_sub = index;
	}

	// Token: 0x060019AD RID: 6573 RVA: 0x000773B5 File Offset: 0x000755B5
	public int GetSelectedSubOption()
	{
		return this.m_selectedOption.m_sub;
	}

	// Token: 0x060019AE RID: 6574 RVA: 0x000773C2 File Offset: 0x000755C2
	public void SetSelectedOptionTarget(int target)
	{
		this.m_selectedOption.m_target = target;
	}

	// Token: 0x060019AF RID: 6575 RVA: 0x000773D0 File Offset: 0x000755D0
	public int GetSelectedOptionTarget()
	{
		return this.m_selectedOption.m_target;
	}

	// Token: 0x060019B0 RID: 6576 RVA: 0x000773E0 File Offset: 0x000755E0
	public bool IsSelectedOptionFriendlyHero()
	{
		Entity hero = this.GetFriendlySidePlayer().GetHero();
		Network.Options.Option selectedNetworkOption = this.GetSelectedNetworkOption();
		return selectedNetworkOption != null && selectedNetworkOption.Main.ID == hero.GetEntityId();
	}

	// Token: 0x060019B1 RID: 6577 RVA: 0x0007741C File Offset: 0x0007561C
	public void SetSelectedOptionPosition(int position)
	{
		this.m_selectedOption.m_position = position;
	}

	// Token: 0x060019B2 RID: 6578 RVA: 0x0007742A File Offset: 0x0007562A
	public int GetSelectedOptionPosition()
	{
		return this.m_selectedOption.m_position;
	}

	// Token: 0x060019B3 RID: 6579 RVA: 0x00077438 File Offset: 0x00075638
	public Network.Options.Option GetSelectedNetworkOption()
	{
		if (this.m_selectedOption.m_main < 0)
		{
			return null;
		}
		return this.m_options.List[this.m_selectedOption.m_main];
	}

	// Token: 0x060019B4 RID: 6580 RVA: 0x00077474 File Offset: 0x00075674
	public Network.Options.Option.SubOption GetSelectedNetworkSubOption()
	{
		if (this.m_selectedOption.m_main < 0)
		{
			return null;
		}
		Network.Options.Option option = this.m_options.List[this.m_selectedOption.m_main];
		if (this.m_selectedOption.m_sub == -1)
		{
			return option.Main;
		}
		return option.Subs[this.m_selectedOption.m_sub];
	}

	// Token: 0x060019B5 RID: 6581 RVA: 0x000774E0 File Offset: 0x000756E0
	public bool EntityHasSubOptions(Entity entity)
	{
		int entityId = entity.GetEntityId();
		Network.Options optionsPacket = this.GetOptionsPacket();
		if (optionsPacket == null)
		{
			return false;
		}
		for (int i = 0; i < optionsPacket.List.Count; i++)
		{
			Network.Options.Option option = optionsPacket.List[i];
			if (option.Type == Network.Options.Option.OptionType.POWER)
			{
				if (option.Main.ID == entityId)
				{
					return option.Subs != null && option.Subs.Count > 0;
				}
			}
		}
		return false;
	}

	// Token: 0x060019B6 RID: 6582 RVA: 0x00077571 File Offset: 0x00075771
	public bool EntityHasTargets(Entity entity)
	{
		return this.EntityHasTargets(entity, false);
	}

	// Token: 0x060019B7 RID: 6583 RVA: 0x0007757B File Offset: 0x0007577B
	public bool SubEntityHasTargets(Entity subEntity)
	{
		return this.EntityHasTargets(subEntity, true);
	}

	// Token: 0x060019B8 RID: 6584 RVA: 0x00077588 File Offset: 0x00075788
	public bool HasSubOptions(Entity entity)
	{
		if (!this.IsEntityInputEnabled(entity))
		{
			return false;
		}
		int entityId = entity.GetEntityId();
		Network.Options optionsPacket = this.GetOptionsPacket();
		for (int i = 0; i < optionsPacket.List.Count; i++)
		{
			Network.Options.Option option = optionsPacket.List[i];
			if (option.Type == Network.Options.Option.OptionType.POWER)
			{
				if (option.Main.ID == entityId)
				{
					return option.Subs.Count > 0;
				}
			}
		}
		return false;
	}

	// Token: 0x060019B9 RID: 6585 RVA: 0x00077614 File Offset: 0x00075814
	public bool HasResponse(Entity entity)
	{
		switch (this.GetResponseMode())
		{
		case GameState.ResponseMode.OPTION:
			return this.IsOption(entity);
		case GameState.ResponseMode.SUB_OPTION:
			return this.IsSubOption(entity);
		case GameState.ResponseMode.OPTION_TARGET:
			return this.IsOptionTarget(entity);
		case GameState.ResponseMode.CHOICE:
			return this.IsChoice(entity);
		default:
			return false;
		}
	}

	// Token: 0x060019BA RID: 6586 RVA: 0x00077668 File Offset: 0x00075868
	public bool IsChoice(Entity entity)
	{
		return this.IsEntityInputEnabled(entity) && this.IsChoosableEntity(entity) && !this.IsChosenEntity(entity);
	}

	// Token: 0x060019BB RID: 6587 RVA: 0x000776A0 File Offset: 0x000758A0
	public bool IsOption(Entity entity)
	{
		if (!this.IsEntityInputEnabled(entity))
		{
			return false;
		}
		int entityId = entity.GetEntityId();
		Network.Options optionsPacket = this.GetOptionsPacket();
		if (optionsPacket == null)
		{
			return false;
		}
		for (int i = 0; i < optionsPacket.List.Count; i++)
		{
			Network.Options.Option option = optionsPacket.List[i];
			if (option.Type == Network.Options.Option.OptionType.POWER)
			{
				if (option.Main.ID == entityId)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060019BC RID: 6588 RVA: 0x00077724 File Offset: 0x00075924
	public bool IsSubOption(Entity entity)
	{
		if (!this.IsEntityInputEnabled(entity))
		{
			return false;
		}
		int entityId = entity.GetEntityId();
		Network.Options.Option selectedNetworkOption = this.GetSelectedNetworkOption();
		for (int i = 0; i < selectedNetworkOption.Subs.Count; i++)
		{
			Network.Options.Option.SubOption subOption = selectedNetworkOption.Subs[i];
			if (subOption.ID == entityId)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060019BD RID: 6589 RVA: 0x0007778C File Offset: 0x0007598C
	public bool IsOptionTarget(Entity entity)
	{
		if (!this.IsEntityInputEnabled(entity))
		{
			return false;
		}
		int entityId = entity.GetEntityId();
		Network.Options.Option.SubOption selectedNetworkSubOption = this.GetSelectedNetworkSubOption();
		if (selectedNetworkSubOption.Targets == null)
		{
			return false;
		}
		for (int i = 0; i < selectedNetworkSubOption.Targets.Count; i++)
		{
			int num = selectedNetworkSubOption.Targets[i];
			if (num == entityId)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060019BE RID: 6590 RVA: 0x000777FC File Offset: 0x000759FC
	public bool IsEntityInputEnabled(Entity entity)
	{
		if (this.IsResponsePacketBlocked())
		{
			return false;
		}
		if (entity.IsBusy())
		{
			return false;
		}
		Card card = entity.GetCard();
		if (card != null)
		{
			if (!card.IsInputEnabled())
			{
				return false;
			}
			Zone zone = card.GetZone();
			if (zone != null && !zone.IsInputEnabled())
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060019BF RID: 6591 RVA: 0x00077864 File Offset: 0x00075A64
	private bool EntityHasTargets(Entity entity, bool isSubEntity)
	{
		int entityId = entity.GetEntityId();
		Network.Options optionsPacket = this.GetOptionsPacket();
		if (optionsPacket == null)
		{
			return false;
		}
		for (int i = 0; i < optionsPacket.List.Count; i++)
		{
			Network.Options.Option option = optionsPacket.List[i];
			if (option.Type == Network.Options.Option.OptionType.POWER)
			{
				if (isSubEntity)
				{
					if (option.Subs != null)
					{
						for (int j = 0; j < option.Subs.Count; j++)
						{
							Network.Options.Option.SubOption subOption = option.Subs[j];
							if (subOption.ID == entityId)
							{
								return subOption.Targets != null && subOption.Targets.Count > 0;
							}
						}
					}
				}
				else if (option.Main.ID == entityId)
				{
					return option.Main.Targets != null && option.Main.Targets.Count > 0;
				}
			}
		}
		return false;
	}

	// Token: 0x060019C0 RID: 6592 RVA: 0x0007797C File Offset: 0x00075B7C
	private void CancelSelectedOptionProposedMana()
	{
		Network.Options.Option selectedNetworkOption = this.GetSelectedNetworkOption();
		if (selectedNetworkOption == null)
		{
			return;
		}
		this.GetFriendlySidePlayer().CancelAllProposedMana(this.GetEntity(selectedNetworkOption.Main.ID));
	}

	// Token: 0x060019C1 RID: 6593 RVA: 0x000779B4 File Offset: 0x00075BB4
	public void ClearResponseMode()
	{
		Log.Hand.Print("ClearResponseMode", new object[0]);
		this.m_responseMode = GameState.ResponseMode.NONE;
		if (this.m_options != null)
		{
			for (int i = 0; i < this.m_options.List.Count; i++)
			{
				Network.Options.Option option = this.m_options.List[i];
				if (option.Type == Network.Options.Option.OptionType.POWER)
				{
					Entity entity = this.GetEntity(option.Main.ID);
					if (entity != null)
					{
						entity.ClearBattlecryFlag();
					}
				}
			}
			this.UpdateHighlightsBasedOnSelection();
			this.UpdateOptionHighlights(this.m_options);
		}
		else
		{
			Network.EntityChoices friendlyEntityChoices = this.GetFriendlyEntityChoices();
			if (friendlyEntityChoices != null)
			{
				this.UpdateChoiceHighlights();
			}
		}
	}

	// Token: 0x060019C2 RID: 6594 RVA: 0x00077A74 File Offset: 0x00075C74
	public void UpdateChoiceHighlights()
	{
		foreach (Network.EntityChoices entityChoices in this.m_choicesMap.Values)
		{
			Entity entity = this.GetEntity(entityChoices.Source);
			if (entity != null)
			{
				Card card = entity.GetCard();
				if (card != null)
				{
					card.UpdateActorState();
				}
			}
			foreach (int id in entityChoices.Entities)
			{
				Entity entity2 = this.GetEntity(id);
				if (entity2 != null)
				{
					Card card2 = entity2.GetCard();
					if (!(card2 == null))
					{
						card2.UpdateActorState();
					}
				}
			}
		}
		foreach (Entity entity3 in this.m_chosenEntities)
		{
			Card card3 = entity3.GetCard();
			if (!(card3 == null))
			{
				card3.UpdateActorState();
			}
		}
	}

	// Token: 0x060019C3 RID: 6595 RVA: 0x00077BDC File Offset: 0x00075DDC
	private void UpdateHighlightsBasedOnSelection()
	{
		if (this.m_selectedOption.m_target != 0)
		{
			Network.Options.Option.SubOption selectedNetworkSubOption = this.GetSelectedNetworkSubOption();
			this.UpdateTargetHighlights(selectedNetworkSubOption);
		}
		else if (this.m_selectedOption.m_sub >= 0)
		{
			Network.Options.Option selectedNetworkOption = this.GetSelectedNetworkOption();
			this.UpdateSubOptionHighlights(selectedNetworkOption);
		}
	}

	// Token: 0x060019C4 RID: 6596 RVA: 0x00077C2B File Offset: 0x00075E2B
	public void UpdateOptionHighlights()
	{
		this.UpdateOptionHighlights(this.m_options);
	}

	// Token: 0x060019C5 RID: 6597 RVA: 0x00077C3C File Offset: 0x00075E3C
	public void UpdateOptionHighlights(Network.Options options)
	{
		if (options == null || options.List == null)
		{
			return;
		}
		for (int i = 0; i < options.List.Count; i++)
		{
			Network.Options.Option option = options.List[i];
			if (option.Type == Network.Options.Option.OptionType.POWER)
			{
				Entity entity = this.GetEntity(option.Main.ID);
				if (entity != null)
				{
					Card card = entity.GetCard();
					if (!(card == null))
					{
						card.UpdateActorState();
					}
				}
			}
		}
	}

	// Token: 0x060019C6 RID: 6598 RVA: 0x00077CD0 File Offset: 0x00075ED0
	private void UpdateSubOptionHighlights(Network.Options.Option option)
	{
		Entity entity = this.GetEntity(option.Main.ID);
		if (entity != null)
		{
			Card card = entity.GetCard();
			if (card != null)
			{
				card.UpdateActorState();
			}
		}
		foreach (Network.Options.Option.SubOption subOption in option.Subs)
		{
			Entity entity2 = this.GetEntity(subOption.ID);
			if (entity2 != null)
			{
				Card card2 = entity2.GetCard();
				if (!(card2 == null))
				{
					card2.UpdateActorState();
				}
			}
		}
	}

	// Token: 0x060019C7 RID: 6599 RVA: 0x00077D90 File Offset: 0x00075F90
	private void UpdateTargetHighlights(Network.Options.Option.SubOption subOption)
	{
		Entity entity = this.GetEntity(subOption.ID);
		if (entity != null)
		{
			Card card = entity.GetCard();
			if (card != null)
			{
				card.UpdateActorState();
			}
		}
		foreach (int id in subOption.Targets)
		{
			Entity entity2 = this.GetEntity(id);
			if (entity2 != null)
			{
				Card card2 = entity2.GetCard();
				if (!(card2 == null))
				{
					card2.UpdateActorState();
				}
			}
		}
	}

	// Token: 0x060019C8 RID: 6600 RVA: 0x00077E48 File Offset: 0x00076048
	public Network.Options GetLastOptions()
	{
		return this.m_lastOptions;
	}

	// Token: 0x060019C9 RID: 6601 RVA: 0x00077E50 File Offset: 0x00076050
	public bool FriendlyHeroIsTargetable()
	{
		if (this.m_responseMode == GameState.ResponseMode.OPTION_TARGET)
		{
			Network.Options.Option option = this.m_options.List[this.m_selectedOption.m_main];
			Network.Options.Option.SubOption subOption = (this.m_selectedOption.m_sub == -1) ? option.Main : option.Subs[this.m_selectedOption.m_sub];
			foreach (int id in subOption.Targets)
			{
				Entity entity = this.GetEntity(id);
				if (entity != null)
				{
					Card card = entity.GetCard();
					if (!(card == null))
					{
						if (entity.IsHero() && entity.IsControlledByFriendlySidePlayer())
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x060019CA RID: 6602 RVA: 0x00077F54 File Offset: 0x00076154
	private void ClearLastOptions()
	{
		this.m_lastOptions = null;
		this.m_lastSelectedOption = null;
	}

	// Token: 0x060019CB RID: 6603 RVA: 0x00077F64 File Offset: 0x00076164
	private void ClearOptions()
	{
		this.m_options = null;
		this.m_selectedOption.Clear();
	}

	// Token: 0x060019CC RID: 6604 RVA: 0x00077F78 File Offset: 0x00076178
	private void ClearFriendlyChoices()
	{
		this.m_chosenEntities.Clear();
		int friendlyPlayerId = this.GetFriendlyPlayerId();
		this.m_choicesMap.Remove(friendlyPlayerId);
	}

	// Token: 0x060019CD RID: 6605 RVA: 0x00077FA4 File Offset: 0x000761A4
	private void OnSelectedOptionsSent()
	{
		this.ClearResponseMode();
		this.m_lastOptions = new Network.Options();
		this.m_lastOptions.CopyFrom(this.m_options);
		this.m_lastSelectedOption = new GameState.SelectedOption();
		this.m_lastSelectedOption.CopyFrom(this.m_selectedOption);
		this.ClearOptions();
	}

	// Token: 0x060019CE RID: 6606 RVA: 0x00077FF5 File Offset: 0x000761F5
	private void OnTimeout()
	{
		if (this.m_responseMode == GameState.ResponseMode.NONE)
		{
			return;
		}
		this.ClearResponseMode();
		this.ClearLastOptions();
		this.ClearOptions();
	}

	// Token: 0x060019CF RID: 6607 RVA: 0x00078018 File Offset: 0x00076218
	public void OnRealTimeCreateGame(List<Network.PowerHistory> powerList, int index, Network.HistCreateGame createGame)
	{
		this.m_gameEntity = GameMgr.Get().CreateGameEntity();
		this.m_gameEntity.SetTags(createGame.Game.Tags);
		this.AddEntity(this.m_gameEntity);
		foreach (Network.HistCreateGame.PlayerData netPlayer in createGame.Players)
		{
			Player player = new Player();
			player.InitPlayer(netPlayer);
			this.AddPlayer(player);
		}
		this.m_createGamePhase = GameState.CreateGamePhase.CREATING;
		this.FireCreateGameEvent();
	}

	// Token: 0x060019D0 RID: 6608 RVA: 0x000780C0 File Offset: 0x000762C0
	public bool OnRealTimeFullEntity(Network.HistFullEntity fullEntity)
	{
		Entity entity = new Entity();
		entity.OnRealTimeFullEntity(fullEntity);
		this.AddEntity(entity);
		return true;
	}

	// Token: 0x060019D1 RID: 6609 RVA: 0x000780E4 File Offset: 0x000762E4
	public bool OnFullEntity(Network.HistFullEntity fullEntity)
	{
		Network.Entity entity = fullEntity.Entity;
		Entity entity2 = this.GetEntity(entity.ID);
		if (entity2 == null)
		{
			Debug.LogWarningFormat("GameState.OnFullEntity() - WARNING entity {0} DOES NOT EXIST!", new object[]
			{
				entity.ID
			});
			return false;
		}
		entity2.OnFullEntity(fullEntity);
		return true;
	}

	// Token: 0x060019D2 RID: 6610 RVA: 0x00078134 File Offset: 0x00076334
	public bool OnRealTimeShowEntity(Network.HistShowEntity showEntity)
	{
		Entity entity = this.GetEntity(showEntity.Entity.ID);
		if (entity == null)
		{
			return false;
		}
		entity.OnRealTimeShowEntity(showEntity);
		return true;
	}

	// Token: 0x060019D3 RID: 6611 RVA: 0x00078164 File Offset: 0x00076364
	public bool OnShowEntity(Network.HistShowEntity showEntity)
	{
		Network.Entity entity = showEntity.Entity;
		Entity entity2 = this.GetEntity(entity.ID);
		if (entity2 == null)
		{
			Debug.LogWarningFormat("GameState.OnShowEntity() - WARNING entity {0} DOES NOT EXIST!", new object[]
			{
				entity.ID
			});
			return false;
		}
		entity2.OnShowEntity(showEntity);
		return true;
	}

	// Token: 0x060019D4 RID: 6612 RVA: 0x000781B4 File Offset: 0x000763B4
	public bool OnEarlyConcedeShowEntity(Network.HistShowEntity showEntity)
	{
		Network.Entity entity = showEntity.Entity;
		Entity entity2 = this.GetEntity(entity.ID);
		if (entity2 == null)
		{
			Debug.LogWarningFormat("GameState.OnEarlyConcedeShowEntity() - WARNING entity {0} DOES NOT EXIST!", new object[]
			{
				entity.ID
			});
			return false;
		}
		entity2.SetTags(entity.Tags);
		return true;
	}

	// Token: 0x060019D5 RID: 6613 RVA: 0x00078208 File Offset: 0x00076408
	public bool OnHideEntity(Network.HistHideEntity hideEntity)
	{
		Entity entity = this.GetEntity(hideEntity.Entity);
		if (entity == null)
		{
			Debug.LogWarningFormat("GameState.OnHideEntity() - WARNING entity {0} DOES NOT EXIST! zone={1}", new object[]
			{
				hideEntity.Entity,
				hideEntity.Zone
			});
			return false;
		}
		entity.SetTagAndHandleChange<int>(GAME_TAG.ZONE, hideEntity.Zone);
		EntityDef entityDef = entity.GetEntityDef();
		entity.SetTag(GAME_TAG.ATK, entityDef.GetATK());
		entity.SetTag(GAME_TAG.HEALTH, entityDef.GetHealth());
		entity.SetTag(GAME_TAG.DAMAGE, 0);
		return true;
	}

	// Token: 0x060019D6 RID: 6614 RVA: 0x00078294 File Offset: 0x00076494
	public bool OnEarlyConcedeHideEntity(Network.HistHideEntity hideEntity)
	{
		Entity entity = this.GetEntity(hideEntity.Entity);
		if (entity == null)
		{
			Debug.LogWarningFormat("GameState.OnEarlyConcedeHideEntity() - WARNING entity {0} DOES NOT EXIST! zone={1}", new object[]
			{
				hideEntity.Entity,
				hideEntity.Zone
			});
			return false;
		}
		entity.SetTag(GAME_TAG.ZONE, hideEntity.Zone);
		return true;
	}

	// Token: 0x060019D7 RID: 6615 RVA: 0x000782F4 File Offset: 0x000764F4
	public bool OnRealTimeChangeEntity(Network.HistChangeEntity changeEntity)
	{
		Entity entity = this.GetEntity(changeEntity.Entity.ID);
		if (entity == null)
		{
			return false;
		}
		entity.OnRealTimeChangeEntity(changeEntity);
		return true;
	}

	// Token: 0x060019D8 RID: 6616 RVA: 0x00078324 File Offset: 0x00076524
	public bool OnChangeEntity(Network.HistChangeEntity changeEntity)
	{
		Network.Entity entity = changeEntity.Entity;
		Entity entity2 = this.GetEntity(entity.ID);
		if (entity2 == null)
		{
			Debug.LogWarningFormat("GameState.OnChangeEntity() - WARNING entity {0} DOES NOT EXIST!", new object[]
			{
				entity.ID
			});
			return false;
		}
		entity2.OnChangeEntity(changeEntity);
		return true;
	}

	// Token: 0x060019D9 RID: 6617 RVA: 0x00078374 File Offset: 0x00076574
	public bool OnEarlyConcedeChangeEntity(Network.HistChangeEntity changeEntity)
	{
		Network.Entity entity = changeEntity.Entity;
		Entity entity2 = this.GetEntity(entity.ID);
		if (entity2 == null)
		{
			Debug.LogWarningFormat("GameState.OnEarlyConcedeChangeEntity() - WARNING entity {0} DOES NOT EXIST!", new object[]
			{
				entity.ID
			});
			return false;
		}
		entity2.SetTags(entity.Tags);
		return true;
	}

	// Token: 0x060019DA RID: 6618 RVA: 0x000783C8 File Offset: 0x000765C8
	public bool OnRealTimeTagChange(Network.HistTagChange change)
	{
		Entity entity = GameState.Get().GetEntity(change.Entity);
		if (entity == null)
		{
			return false;
		}
		this.PreprocessRealTimeTagChange(entity, change);
		entity.OnRealTimeTagChanged(change);
		return true;
	}

	// Token: 0x060019DB RID: 6619 RVA: 0x00078400 File Offset: 0x00076600
	public bool OnTagChange(Network.HistTagChange netChange)
	{
		Entity entity = this.GetEntity(netChange.Entity);
		if (entity == null)
		{
			Debug.LogWarningFormat("GameState.OnTagChange() - WARNING Entity {0} does not exist", new object[]
			{
				netChange.Entity
			});
			return false;
		}
		TagDelta tagDelta = new TagDelta();
		tagDelta.tag = netChange.Tag;
		tagDelta.oldValue = entity.GetTag(netChange.Tag);
		tagDelta.newValue = netChange.Value;
		entity.SetTag(tagDelta.tag, tagDelta.newValue);
		this.PreprocessTagChange(entity, tagDelta);
		entity.OnTagChanged(tagDelta);
		return true;
	}

	// Token: 0x060019DC RID: 6620 RVA: 0x00078494 File Offset: 0x00076694
	public bool OnEarlyConcedeTagChange(Network.HistTagChange netChange)
	{
		Entity entity = this.GetEntity(netChange.Entity);
		if (entity == null)
		{
			Debug.LogWarningFormat("GameState.OnEarlyConcedeTagChange() - WARNING Entity {0} does not exist", new object[]
			{
				netChange.Entity
			});
			return false;
		}
		TagDelta tagDelta = new TagDelta();
		tagDelta.tag = netChange.Tag;
		tagDelta.oldValue = entity.GetTag(netChange.Tag);
		tagDelta.newValue = netChange.Value;
		entity.SetTag(tagDelta.tag, tagDelta.newValue);
		this.PreprocessEarlyConcedeTagChange(entity, tagDelta);
		this.ProcessEarlyConcedeTagChange(entity, tagDelta);
		return true;
	}

	// Token: 0x060019DD RID: 6621 RVA: 0x00078528 File Offset: 0x00076728
	public bool OnMetaData(Network.HistMetaData metaData)
	{
		foreach (int num in metaData.Info)
		{
			Entity entity = this.GetEntity(num);
			if (entity == null)
			{
				Debug.LogWarning(string.Format("GameState.OnMetaData() - WARNING Entity {0} does not exist", num));
				return false;
			}
			entity.OnMetaData(metaData);
		}
		return true;
	}

	// Token: 0x060019DE RID: 6622 RVA: 0x000785B0 File Offset: 0x000767B0
	public void OnTaskListEnded(PowerTaskList taskList)
	{
		if (taskList == null)
		{
			return;
		}
		foreach (PowerTask powerTask in taskList.GetTaskList())
		{
			Network.PowerHistory power = powerTask.GetPower();
			if (power.Type == Network.PowerType.CREATE_GAME)
			{
				this.m_createGamePhase = GameState.CreateGamePhase.CREATED;
				this.FireCreateGameEvent();
				this.m_createGameListeners.Clear();
			}
		}
	}

	// Token: 0x060019DF RID: 6623 RVA: 0x00078638 File Offset: 0x00076838
	public void OnPowerHistory(List<Network.PowerHistory> powerList)
	{
		this.DebugPrintPowerList(powerList);
		bool flag = this.m_powerProcessor.HasEarlyConcedeTaskList();
		this.m_powerProcessor.OnPowerHistory(powerList);
		this.ProcessAllQueuedChoices();
		bool flag2 = this.m_powerProcessor.HasEarlyConcedeTaskList();
		if (!flag && flag2)
		{
			this.OnReceivedEarlyConcede();
		}
	}

	// Token: 0x060019E0 RID: 6624 RVA: 0x00078688 File Offset: 0x00076888
	private void OnReceivedEarlyConcede()
	{
		this.ClearResponseMode();
		this.ClearLastOptions();
		this.ClearOptions();
	}

	// Token: 0x060019E1 RID: 6625 RVA: 0x0007869C File Offset: 0x0007689C
	public void OnAllOptions(Network.Options options)
	{
		this.m_responseMode = GameState.ResponseMode.OPTION;
		this.m_chosenEntities.Clear();
		if (this.m_options != null && (this.m_lastOptions == null || this.m_lastOptions.ID < this.m_options.ID))
		{
			this.m_lastOptions = new Network.Options();
			this.m_lastOptions.CopyFrom(this.m_options);
		}
		this.m_options = options;
		foreach (Network.Options.Option option in this.m_options.List)
		{
			if (option.Type == Network.Options.Option.OptionType.POWER)
			{
				Entity entity = this.GetEntity(option.Main.ID);
				if (entity != null)
				{
					if (option.Main.Targets != null && option.Main.Targets.Count > 0)
					{
						entity.UpdateUseBattlecryFlag(true);
					}
				}
			}
		}
		this.DebugPrintOptions();
		this.EnterMainOptionMode();
		this.FireOptionsReceivedEvent();
	}

	// Token: 0x060019E2 RID: 6626 RVA: 0x000787C8 File Offset: 0x000769C8
	public void OnEntityChoices(Network.EntityChoices choices)
	{
		PowerTaskList lastTaskList = this.m_powerProcessor.GetLastTaskList();
		if (!this.CanProcessEntityChoices(choices))
		{
			Log.Power.Print("GameState.OnEntityChoices() - id={0} playerId={1} queued", new object[]
			{
				choices.ID,
				choices.PlayerId
			});
			GameState.QueuedChoice queuedChoice = new GameState.QueuedChoice
			{
				m_packet = choices,
				m_eventData = lastTaskList
			};
			this.m_queuedChoices.Enqueue(queuedChoice);
			return;
		}
		this.ProcessEntityChoices(choices, lastTaskList);
	}

	// Token: 0x060019E3 RID: 6627 RVA: 0x0007884C File Offset: 0x00076A4C
	public void OnEntitiesChosen(Network.EntitiesChosen chosen)
	{
		if (!this.CanProcessEntitiesChosen(chosen))
		{
			Log.Power.Print("GameState.OnEntitiesChosen() - id={0} playerId={1} queued", new object[]
			{
				chosen.ID,
				chosen.PlayerId
			});
			GameState.QueuedChoice queuedChoice = new GameState.QueuedChoice
			{
				m_type = GameState.QueuedChoice.PacketType.ENTITIES_CHOSEN,
				m_packet = chosen
			};
			this.m_queuedChoices.Enqueue(queuedChoice);
			return;
		}
		this.ProcessEntitiesChosen(chosen);
	}

	// Token: 0x060019E4 RID: 6628 RVA: 0x000788C0 File Offset: 0x00076AC0
	private bool CanProcessEntityChoices(Network.EntityChoices choices)
	{
		int playerId = choices.PlayerId;
		if (!this.m_playerMap.ContainsKey(playerId))
		{
			return false;
		}
		foreach (int key in choices.Entities)
		{
			if (!this.m_entityMap.ContainsKey(key))
			{
				return false;
			}
		}
		return !this.m_choicesMap.ContainsKey(playerId);
	}

	// Token: 0x060019E5 RID: 6629 RVA: 0x0007895C File Offset: 0x00076B5C
	private bool CanProcessEntitiesChosen(Network.EntitiesChosen chosen)
	{
		int playerId = chosen.PlayerId;
		if (!this.m_playerMap.ContainsKey(playerId))
		{
			return false;
		}
		foreach (int key in chosen.Entities)
		{
			if (!this.m_entityMap.ContainsKey(key))
			{
				return false;
			}
		}
		Network.EntityChoices entityChoices;
		return !this.m_choicesMap.TryGetValue(playerId, out entityChoices) || entityChoices.ID == chosen.ID;
	}

	// Token: 0x060019E6 RID: 6630 RVA: 0x00078A0C File Offset: 0x00076C0C
	private void ProcessAllQueuedChoices()
	{
		while (this.m_queuedChoices.Count > 0)
		{
			GameState.QueuedChoice queuedChoice = this.m_queuedChoices.Peek();
			GameState.QueuedChoice.PacketType type = queuedChoice.m_type;
			if (type != GameState.QueuedChoice.PacketType.ENTITY_CHOICES)
			{
				if (type == GameState.QueuedChoice.PacketType.ENTITIES_CHOSEN)
				{
					Network.EntitiesChosen chosen = (Network.EntitiesChosen)queuedChoice.m_packet;
					if (!this.CanProcessEntitiesChosen(chosen))
					{
						return;
					}
					this.m_queuedChoices.Dequeue();
					this.ProcessEntitiesChosen(chosen);
				}
			}
			else
			{
				Network.EntityChoices choices = (Network.EntityChoices)queuedChoice.m_packet;
				if (!this.CanProcessEntityChoices(choices))
				{
					return;
				}
				this.m_queuedChoices.Dequeue();
				PowerTaskList preChoiceTaskList = (PowerTaskList)queuedChoice.m_eventData;
				this.ProcessEntityChoices(choices, preChoiceTaskList);
			}
		}
	}

	// Token: 0x060019E7 RID: 6631 RVA: 0x00078AC8 File Offset: 0x00076CC8
	private void ProcessEntityChoices(Network.EntityChoices choices, PowerTaskList preChoiceTaskList)
	{
		this.DebugPrintEntityChoices(choices, preChoiceTaskList);
		if (this.m_powerProcessor.HasEarlyConcedeTaskList())
		{
			return;
		}
		int playerId = choices.PlayerId;
		this.m_choicesMap[playerId] = choices;
		int friendlyPlayerId = this.GetFriendlyPlayerId();
		if (playerId == friendlyPlayerId)
		{
			this.m_responseMode = GameState.ResponseMode.CHOICE;
			this.m_chosenEntities.Clear();
			this.EnterChoiceMode();
		}
		this.FireEntityChoicesReceivedEvent(choices, preChoiceTaskList);
	}

	// Token: 0x060019E8 RID: 6632 RVA: 0x00078B30 File Offset: 0x00076D30
	private void ProcessEntitiesChosen(Network.EntitiesChosen chosen)
	{
		this.DebugPrintEntitiesChosen(chosen);
		if (this.m_powerProcessor.HasEarlyConcedeTaskList())
		{
			return;
		}
		if (this.FireEntitiesChosenReceivedEvent(chosen))
		{
			return;
		}
		this.OnEntitiesChosenProcessed(chosen);
	}

	// Token: 0x060019E9 RID: 6633 RVA: 0x00078B69 File Offset: 0x00076D69
	public void OnGameSetup(Network.GameSetup setup)
	{
		this.m_maxSecretsPerPlayer = setup.MaxSecretsPerPlayer;
		this.m_maxFriendlyMinionsPerPlayer = setup.MaxFriendlyMinionsPerPlayer;
	}

	// Token: 0x060019EA RID: 6634 RVA: 0x00078B84 File Offset: 0x00076D84
	public void OnOptionRejected(int optionId)
	{
		if (this.m_lastSelectedOption == null)
		{
			Debug.LogError("GameState.OnOptionRejected() - got an option rejection without a last selected option");
			return;
		}
		if (this.m_lastOptions.ID != optionId)
		{
			Debug.LogErrorFormat("GameState.OnOptionRejected() - rejected option id ({0}) does not match last option id ({1})", new object[]
			{
				optionId,
				this.m_lastOptions.ID
			});
			return;
		}
		Network.Options.Option option = this.m_lastOptions.List[this.m_lastSelectedOption.m_main];
		this.FireOptionRejectedEvent(option);
		this.ClearLastOptions();
	}

	// Token: 0x060019EB RID: 6635 RVA: 0x00078C10 File Offset: 0x00076E10
	public void OnTurnTimerUpdate(Network.TurnTimerInfo info)
	{
		TurnTimerUpdate turnTimerUpdate = new TurnTimerUpdate();
		turnTimerUpdate.SetSecondsRemaining(info.Seconds);
		turnTimerUpdate.SetEndTimestamp(Time.realtimeSinceStartup + info.Seconds);
		turnTimerUpdate.SetShow(info.Show);
		int num = (this.m_gameEntity != null) ? this.m_gameEntity.GetTag(GAME_TAG.TURN) : 0;
		if (info.Turn > num)
		{
			this.m_turnTimerUpdates[info.Turn] = turnTimerUpdate;
			return;
		}
		this.TriggerTurnTimerUpdate(turnTimerUpdate);
	}

	// Token: 0x060019EC RID: 6636 RVA: 0x00078C92 File Offset: 0x00076E92
	public void OnSpectatorNotifyEvent(SpectatorNotify notify)
	{
		this.FireSpectatorNotifyEvent(notify);
	}

	// Token: 0x060019ED RID: 6637 RVA: 0x00078C9C File Offset: 0x00076E9C
	public void SendChoices()
	{
		if (this.m_responseMode != GameState.ResponseMode.CHOICE)
		{
			return;
		}
		Network.EntityChoices friendlyEntityChoices = this.GetFriendlyEntityChoices();
		if (friendlyEntityChoices == null)
		{
			return;
		}
		Log.Power.Print("GameState.SendChoices() - id={0} ChoiceType={1}", new object[]
		{
			friendlyEntityChoices.ID,
			friendlyEntityChoices.ChoiceType
		});
		List<int> list = new List<int>();
		for (int i = 0; i < this.m_chosenEntities.Count; i++)
		{
			Entity entity = this.m_chosenEntities[i];
			int entityId = entity.GetEntityId();
			Log.Power.Print("GameState.SendChoices() -   m_chosenEntities[{0}]={1}", new object[]
			{
				i,
				entity
			});
			list.Add(entityId);
		}
		if (!GameMgr.Get().IsSpectator())
		{
			Network.Get().SendChoices(friendlyEntityChoices.ID, list);
		}
		this.ClearResponseMode();
		this.ClearFriendlyChoices();
		this.ProcessAllQueuedChoices();
	}

	// Token: 0x060019EE RID: 6638 RVA: 0x00078D88 File Offset: 0x00076F88
	public void OnEntitiesChosenProcessed(Network.EntitiesChosen chosen)
	{
		int playerId = chosen.PlayerId;
		int friendlyPlayerId = this.GetFriendlyPlayerId();
		if (playerId == friendlyPlayerId)
		{
			this.ClearResponseMode();
			this.ClearFriendlyChoices();
		}
		else
		{
			this.m_choicesMap.Remove(playerId);
		}
		this.ProcessAllQueuedChoices();
	}

	// Token: 0x060019EF RID: 6639 RVA: 0x00078DD0 File Offset: 0x00076FD0
	public void SendOption()
	{
		if (!GameMgr.Get().IsSpectator())
		{
			Network.Get().SendOption(this.m_options.ID, this.m_selectedOption.m_main, this.m_selectedOption.m_target, this.m_selectedOption.m_sub, this.m_selectedOption.m_position);
			Log.Power.Print("GameState.SendOption() - selectedOption={0} selectedSubOption={1} selectedTarget={2} selectedPosition={3}", new object[]
			{
				this.m_selectedOption.m_main,
				this.m_selectedOption.m_sub,
				this.m_selectedOption.m_target,
				this.m_selectedOption.m_position
			});
		}
		this.OnSelectedOptionsSent();
		Network.Options.Option option = this.m_lastOptions.List[this.m_lastSelectedOption.m_main];
		this.FireOptionsSentEvent(option);
	}

	// Token: 0x060019F0 RID: 6640 RVA: 0x00078EB8 File Offset: 0x000770B8
	public PlayErrors.GameStateInfo ConvertToGameStateInfo()
	{
		PlayErrors.GameStateInfo gameStateInfo = new PlayErrors.GameStateInfo();
		GameState gameState = GameState.Get();
		gameStateInfo.currentStep = (TAG_STEP)gameState.GetGameEntity().GetTag(GAME_TAG.STEP);
		return gameStateInfo;
	}

	// Token: 0x060019F1 RID: 6641 RVA: 0x00078EE8 File Offset: 0x000770E8
	private void OnTurnChanged_TurnTimer(int oldTurn, int newTurn)
	{
		if (this.m_turnTimerUpdates.Count == 0)
		{
			return;
		}
		TurnTimerUpdate turnTimerUpdate;
		if (!this.m_turnTimerUpdates.TryGetValue(newTurn, out turnTimerUpdate))
		{
			return;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float endTimestamp = turnTimerUpdate.GetEndTimestamp();
		float secondsRemaining = Mathf.Max(0f, endTimestamp - realtimeSinceStartup);
		turnTimerUpdate.SetSecondsRemaining(secondsRemaining);
		this.TriggerTurnTimerUpdate(turnTimerUpdate);
		this.m_turnTimerUpdates.Remove(newTurn);
	}

	// Token: 0x060019F2 RID: 6642 RVA: 0x00078F50 File Offset: 0x00077150
	private void TriggerTurnTimerUpdate(TurnTimerUpdate update)
	{
		this.FireTurnTimerUpdateEvent(update);
		if (update.GetSecondsRemaining() > Mathf.Epsilon)
		{
			return;
		}
		this.OnTimeout();
	}

	// Token: 0x060019F3 RID: 6643 RVA: 0x00078F70 File Offset: 0x00077170
	private void DebugPrintPowerList(List<Network.PowerHistory> powerList)
	{
		if (!Log.Power.CanPrint())
		{
			return;
		}
		string empty = string.Empty;
		Log.Power.Print(string.Format("GameState.DebugPrintPowerList() - Count={0}", powerList.Count), new object[0]);
		for (int i = 0; i < powerList.Count; i++)
		{
			Network.PowerHistory power = powerList[i];
			this.DebugPrintPower(Log.Power, "GameState", power, ref empty);
		}
	}

	// Token: 0x060019F4 RID: 6644 RVA: 0x00078FEC File Offset: 0x000771EC
	public void DebugPrintPower(Logger logger, string callerName, Network.PowerHistory power)
	{
		string empty = string.Empty;
		this.DebugPrintPower(logger, callerName, power, ref empty);
	}

	// Token: 0x060019F5 RID: 6645 RVA: 0x0007900C File Offset: 0x0007720C
	public void DebugPrintPower(Logger logger, string callerName, Network.PowerHistory power, ref string indentation)
	{
		if (!Log.Power.CanPrint())
		{
			return;
		}
		switch (power.Type)
		{
		case Network.PowerType.FULL_ENTITY:
		{
			Network.HistFullEntity histFullEntity = (Network.HistFullEntity)power;
			Network.Entity entity = histFullEntity.Entity;
			Entity entity2 = this.GetEntity(entity.ID);
			if (entity2 == null)
			{
				logger.Print("{0}.DebugPrintPower() - {1}FULL_ENTITY - Creating ID={2} CardID={3}", new object[]
				{
					callerName,
					indentation,
					entity.ID,
					entity.CardID
				});
			}
			else
			{
				logger.Print("{0}.DebugPrintPower() - {1}FULL_ENTITY - Updating {2} CardID={3}", new object[]
				{
					callerName,
					indentation,
					entity2,
					entity.CardID
				});
			}
			this.DebugPrintTags(logger, callerName, indentation, entity);
			break;
		}
		case Network.PowerType.SHOW_ENTITY:
		{
			Network.HistShowEntity histShowEntity = (Network.HistShowEntity)power;
			Network.Entity entity3 = histShowEntity.Entity;
			object printableEntity = this.GetPrintableEntity(entity3.ID);
			logger.Print("{0}.DebugPrintPower() - {1}SHOW_ENTITY - Updating Entity={2} CardID={3}", new object[]
			{
				callerName,
				indentation,
				printableEntity,
				entity3.CardID
			});
			this.DebugPrintTags(logger, callerName, indentation, entity3);
			break;
		}
		case Network.PowerType.HIDE_ENTITY:
		{
			Network.HistHideEntity histHideEntity = (Network.HistHideEntity)power;
			object printableEntity2 = this.GetPrintableEntity(histHideEntity.Entity);
			logger.Print("{0}.DebugPrintPower() - {1}HIDE_ENTITY - Entity={2} {3}", new object[]
			{
				callerName,
				indentation,
				printableEntity2,
				Tags.DebugTag(49, histHideEntity.Zone)
			});
			break;
		}
		case Network.PowerType.TAG_CHANGE:
		{
			Network.HistTagChange histTagChange = (Network.HistTagChange)power;
			object printableEntity3 = this.GetPrintableEntity(histTagChange.Entity);
			logger.Print("{0}.DebugPrintPower() - {1}TAG_CHANGE Entity={2} {3}", new object[]
			{
				callerName,
				indentation,
				printableEntity3,
				Tags.DebugTag(histTagChange.Tag, histTagChange.Value)
			});
			break;
		}
		case Network.PowerType.BLOCK_START:
		{
			Network.HistBlockStart histBlockStart = (Network.HistBlockStart)power;
			object printableEntity4 = this.GetPrintableEntity(histBlockStart.Entity);
			object printableEntity5 = this.GetPrintableEntity(histBlockStart.Target);
			logger.Print("{0}.DebugPrintPower() - {1}BLOCK_START BlockType={2} Entity={3} EffectCardId={4} EffectIndex={5} Target={6}", new object[]
			{
				callerName,
				indentation,
				histBlockStart.BlockType,
				printableEntity4,
				histBlockStart.EffectCardId,
				histBlockStart.EffectIndex,
				printableEntity5
			});
			indentation += "    ";
			break;
		}
		case Network.PowerType.BLOCK_END:
			if (indentation.Length >= "    ".Length)
			{
				indentation = indentation.Remove(indentation.Length - "    ".Length);
			}
			logger.Print("{0}.DebugPrintPower() - {1}BLOCK_END", new object[]
			{
				callerName,
				indentation
			});
			break;
		case Network.PowerType.CREATE_GAME:
		{
			Network.HistCreateGame histCreateGame = (Network.HistCreateGame)power;
			logger.Print("{0}.DebugPrintPower() - {1}CREATE_GAME", new object[]
			{
				callerName,
				indentation
			});
			indentation += "    ";
			logger.Print("{0}.DebugPrintPower() - {1}GameEntity EntityID={2}", new object[]
			{
				callerName,
				indentation,
				histCreateGame.Game.ID
			});
			this.DebugPrintTags(logger, callerName, indentation, histCreateGame.Game);
			foreach (Network.HistCreateGame.PlayerData playerData in histCreateGame.Players)
			{
				logger.Print("{0}.DebugPrintPower() - {1}Player EntityID={2} PlayerID={3} GameAccountId={4}", new object[]
				{
					callerName,
					indentation,
					playerData.Player.ID,
					playerData.ID,
					playerData.GameAccountId
				});
				this.DebugPrintTags(logger, callerName, indentation, playerData.Player);
			}
			indentation = indentation.Remove(indentation.Length - "    ".Length);
			break;
		}
		case Network.PowerType.META_DATA:
		{
			Network.HistMetaData histMetaData = (Network.HistMetaData)power;
			object obj = histMetaData.Data;
			HistoryMeta.Type metaType = histMetaData.MetaType;
			if (metaType == 3)
			{
				obj = this.GetPrintableEntity(histMetaData.Data);
			}
			logger.Print("{0}.DebugPrintPower() - {1}META_DATA - Meta={2} Data={3} Info={4}", new object[]
			{
				callerName,
				indentation,
				histMetaData.MetaType,
				obj,
				histMetaData.Info.Count
			});
			if (histMetaData.Info.Count > 0 && logger.IsVerbose())
			{
				indentation += "    ";
				for (int i = 0; i < histMetaData.Info.Count; i++)
				{
					int id = histMetaData.Info[i];
					object printableEntity6 = this.GetPrintableEntity(id);
					logger.Print(true, "{0}.DebugPrintPower() - {1}Info[{2}] = {3}", new object[]
					{
						callerName,
						indentation,
						i,
						printableEntity6
					});
				}
				indentation = indentation.Remove(indentation.Length - "    ".Length);
			}
			break;
		}
		case Network.PowerType.CHANGE_ENTITY:
		{
			Network.HistChangeEntity histChangeEntity = (Network.HistChangeEntity)power;
			Network.Entity entity4 = histChangeEntity.Entity;
			object printableEntity7 = this.GetPrintableEntity(entity4.ID);
			logger.Print("{0}.DebugPrintPower() - {1}CHANGE_ENTITY - Updating Entity={2} CardID={3}", new object[]
			{
				callerName,
				indentation,
				printableEntity7,
				entity4.CardID
			});
			this.DebugPrintTags(logger, callerName, indentation, entity4);
			break;
		}
		default:
			logger.Print("{0}.DebugPrintPower() - ERROR: unhandled PowType {1}", new object[]
			{
				callerName,
				power.Type
			});
			break;
		}
	}

	// Token: 0x060019F6 RID: 6646 RVA: 0x000795C4 File Offset: 0x000777C4
	private void DebugPrintTags(Logger logger, string callerName, string indentation, Network.Entity netEntity)
	{
		if (!Log.Power.CanPrint())
		{
			return;
		}
		if (indentation != null)
		{
			indentation += "    ";
		}
		for (int i = 0; i < netEntity.Tags.Count; i++)
		{
			Network.Entity.Tag tag = netEntity.Tags[i];
			logger.Print("{0}.DebugPrintPower() - {1}{2}", new object[]
			{
				callerName,
				indentation,
				Tags.DebugTag(tag.Name, tag.Value)
			});
		}
	}

	// Token: 0x060019F7 RID: 6647 RVA: 0x0007964C File Offset: 0x0007784C
	private void DebugPrintOptions()
	{
		if (!Log.Power.CanPrint())
		{
			return;
		}
		Log.Power.Print("GameState.DebugPrintOptions() - id={0}", new object[]
		{
			this.m_options.ID
		});
		for (int i = 0; i < this.m_options.List.Count; i++)
		{
			Network.Options.Option option = this.m_options.List[i];
			Entity entity = this.GetEntity(option.Main.ID);
			Log.Power.Print("GameState.DebugPrintOptions() -   option {0} type={1} mainEntity={2}", new object[]
			{
				i,
				option.Type,
				entity
			});
			if (option.Main.Targets != null)
			{
				for (int j = 0; j < option.Main.Targets.Count; j++)
				{
					int id = option.Main.Targets[j];
					Entity entity2 = this.GetEntity(id);
					Log.Power.Print("GameState.DebugPrintOptions() -     target {0} entity={1}", new object[]
					{
						j,
						entity2
					});
				}
			}
			for (int k = 0; k < option.Subs.Count; k++)
			{
				Network.Options.Option.SubOption subOption = option.Subs[k];
				Entity entity3 = this.GetEntity(subOption.ID);
				Log.Power.Print("GameState.DebugPrintOptions() -     subOption {0} entity={1}", new object[]
				{
					k,
					entity3
				});
				if (subOption.Targets != null)
				{
					for (int l = 0; l < subOption.Targets.Count; l++)
					{
						int id2 = subOption.Targets[l];
						Entity entity4 = this.GetEntity(id2);
						Log.Power.Print("GameState.DebugPrintOptions() -       target {0} entity={1}", new object[]
						{
							l,
							entity4
						});
					}
				}
			}
		}
	}

	// Token: 0x060019F8 RID: 6648 RVA: 0x00079844 File Offset: 0x00077A44
	private void DebugPrintEntityChoices(Network.EntityChoices choices, PowerTaskList preChoiceTaskList)
	{
		if (!Log.Power.CanPrint())
		{
			return;
		}
		Player player = this.GetPlayer(choices.PlayerId);
		object printableEntity = this.GetPrintableEntity(player.GetEntityId());
		object obj = null;
		if (preChoiceTaskList != null)
		{
			obj = preChoiceTaskList.GetId();
		}
		Log.Power.Print("GameState.DebugPrintEntityChoices() - id={0} Player={1} TaskList={2} ChoiceType={3} CountMin={4} CountMax={5}", new object[]
		{
			choices.ID,
			printableEntity,
			obj,
			choices.ChoiceType,
			choices.CountMin,
			choices.CountMax
		});
		object printableEntity2 = this.GetPrintableEntity(choices.Source);
		Log.Power.Print("GameState.DebugPrintEntityChoices() -   Source={0}", new object[]
		{
			printableEntity2
		});
		for (int i = 0; i < choices.Entities.Count; i++)
		{
			object printableEntity3 = this.GetPrintableEntity(choices.Entities[i]);
			Log.Power.Print("GameState.DebugPrintEntityChoices() -   Entities[{0}]={1}", new object[]
			{
				i,
				printableEntity3
			});
		}
	}

	// Token: 0x060019F9 RID: 6649 RVA: 0x00079964 File Offset: 0x00077B64
	private void DebugPrintEntitiesChosen(Network.EntitiesChosen chosen)
	{
		if (!Log.Power.CanPrint())
		{
			return;
		}
		Player player = this.GetPlayer(chosen.PlayerId);
		object printableEntity = this.GetPrintableEntity(player.GetEntityId());
		Log.Power.Print("GameState.DebugPrintEntitiesChosen() - id={0} Player={1} EntitiesCount={2}", new object[]
		{
			chosen.ID,
			printableEntity,
			chosen.Entities.Count
		});
		for (int i = 0; i < chosen.Entities.Count; i++)
		{
			object printableEntity2 = this.GetPrintableEntity(chosen.Entities[i]);
			Log.Power.Print("GameState.DebugPrintEntitiesChosen() -   Entities[{0}]={1}", new object[]
			{
				i,
				printableEntity2
			});
		}
	}

	// Token: 0x060019FA RID: 6650 RVA: 0x00079A28 File Offset: 0x00077C28
	private object GetPrintableEntity(int id)
	{
		Entity entity = this.GetEntity(id);
		if (entity == null)
		{
			return id;
		}
		return entity;
	}

	// Token: 0x060019FB RID: 6651 RVA: 0x00079A4C File Offset: 0x00077C4C
	private void PrintBlockingTaskList(StringBuilder builder, PowerTaskList taskList)
	{
		if (taskList == null)
		{
			builder.Append("null");
		}
		else
		{
			builder.AppendFormat("ID={0} ", taskList.GetId());
			builder.Append("Source=[");
			Network.HistBlockStart blockStart = taskList.GetBlockStart();
			if (blockStart == null)
			{
				builder.Append("null");
			}
			else
			{
				builder.AppendFormat("BlockType={0}", blockStart.BlockType);
				builder.Append(' ');
				object printableEntity = this.GetPrintableEntity(blockStart.Entity);
				builder.AppendFormat("Entity={0}", printableEntity);
				builder.Append(' ');
				object printableEntity2 = this.GetPrintableEntity(blockStart.Target);
				builder.AppendFormat("Target={0}", printableEntity2);
			}
			builder.Append(']');
			builder.AppendFormat(" Tasks={0}", taskList.GetTaskList().Count);
		}
	}

	// Token: 0x060019FC RID: 6652 RVA: 0x00079B33 File Offset: 0x00077D33
	private void QuickGameFlipHeroesCheat(List<Network.PowerHistory> powerList)
	{
	}

	// Token: 0x04000CD1 RID: 3281
	public const int DEFAULT_SUBOPTION = -1;

	// Token: 0x04000CD2 RID: 3282
	private const string INDENT = "    ";

	// Token: 0x04000CD3 RID: 3283
	private const float BLOCK_REPORT_START_SEC = 10f;

	// Token: 0x04000CD4 RID: 3284
	private const float BLOCK_REPORT_INTERVAL_SEC = 3f;

	// Token: 0x04000CD5 RID: 3285
	private static GameState s_instance;

	// Token: 0x04000CD6 RID: 3286
	private static List<GameState.GameStateInitializedListener> s_gameStateInitializedListeners;

	// Token: 0x04000CD7 RID: 3287
	private Map<int, Entity> m_entityMap = new Map<int, Entity>();

	// Token: 0x04000CD8 RID: 3288
	private Map<int, Player> m_playerMap = new Map<int, Player>();

	// Token: 0x04000CD9 RID: 3289
	private GameEntity m_gameEntity;

	// Token: 0x04000CDA RID: 3290
	private GameState.CreateGamePhase m_createGamePhase;

	// Token: 0x04000CDB RID: 3291
	private Network.HistTagChange m_realTimeGameOverTagChange;

	// Token: 0x04000CDC RID: 3292
	private bool m_gameOver;

	// Token: 0x04000CDD RID: 3293
	private bool m_concedeRequested;

	// Token: 0x04000CDE RID: 3294
	private int m_maxSecretsPerPlayer;

	// Token: 0x04000CDF RID: 3295
	private int m_maxFriendlyMinionsPerPlayer;

	// Token: 0x04000CE0 RID: 3296
	private GameState.ResponseMode m_responseMode;

	// Token: 0x04000CE1 RID: 3297
	private Map<int, Network.EntityChoices> m_choicesMap = new Map<int, Network.EntityChoices>();

	// Token: 0x04000CE2 RID: 3298
	private Queue<GameState.QueuedChoice> m_queuedChoices = new Queue<GameState.QueuedChoice>();

	// Token: 0x04000CE3 RID: 3299
	private List<Entity> m_chosenEntities = new List<Entity>();

	// Token: 0x04000CE4 RID: 3300
	private Network.Options m_options;

	// Token: 0x04000CE5 RID: 3301
	private GameState.SelectedOption m_selectedOption = new GameState.SelectedOption();

	// Token: 0x04000CE6 RID: 3302
	private Network.Options m_lastOptions;

	// Token: 0x04000CE7 RID: 3303
	private GameState.SelectedOption m_lastSelectedOption;

	// Token: 0x04000CE8 RID: 3304
	private bool m_coinHasSpawned;

	// Token: 0x04000CE9 RID: 3305
	private Card m_friendlyCardBeingDrawn;

	// Token: 0x04000CEA RID: 3306
	private Card m_opponentCardBeingDrawn;

	// Token: 0x04000CEB RID: 3307
	private int m_lastTurnRemindedOfFullHand;

	// Token: 0x04000CEC RID: 3308
	private bool m_usingFastActorTriggers;

	// Token: 0x04000CED RID: 3309
	private List<GameState.CreateGameListener> m_createGameListeners = new List<GameState.CreateGameListener>();

	// Token: 0x04000CEE RID: 3310
	private List<GameState.OptionsReceivedListener> m_optionsReceivedListeners = new List<GameState.OptionsReceivedListener>();

	// Token: 0x04000CEF RID: 3311
	private List<GameState.OptionsSentListener> m_optionsSentListeners = new List<GameState.OptionsSentListener>();

	// Token: 0x04000CF0 RID: 3312
	private List<GameState.OptionRejectedListener> m_optionRejectedListeners = new List<GameState.OptionRejectedListener>();

	// Token: 0x04000CF1 RID: 3313
	private List<GameState.EntityChoicesReceivedListener> m_entityChoicesReceivedListeners = new List<GameState.EntityChoicesReceivedListener>();

	// Token: 0x04000CF2 RID: 3314
	private List<GameState.EntitiesChosenReceivedListener> m_entitiesChosenReceivedListeners = new List<GameState.EntitiesChosenReceivedListener>();

	// Token: 0x04000CF3 RID: 3315
	private List<GameState.CurrentPlayerChangedListener> m_currentPlayerChangedListeners = new List<GameState.CurrentPlayerChangedListener>();

	// Token: 0x04000CF4 RID: 3316
	private List<GameState.FriendlyTurnStartedListener> m_friendlyTurnStartedListeners = new List<GameState.FriendlyTurnStartedListener>();

	// Token: 0x04000CF5 RID: 3317
	private List<GameState.TurnChangedListener> m_turnChangedListeners = new List<GameState.TurnChangedListener>();

	// Token: 0x04000CF6 RID: 3318
	private List<GameState.SpectatorNotifyListener> m_spectatorNotifyListeners = new List<GameState.SpectatorNotifyListener>();

	// Token: 0x04000CF7 RID: 3319
	private List<GameState.GameOverListener> m_gameOverListeners = new List<GameState.GameOverListener>();

	// Token: 0x04000CF8 RID: 3320
	private List<GameState.HeroChangedListener> m_heroChangedListeners = new List<GameState.HeroChangedListener>();

	// Token: 0x04000CF9 RID: 3321
	private PowerProcessor m_powerProcessor = new PowerProcessor();

	// Token: 0x04000CFA RID: 3322
	private float m_blockedSec;

	// Token: 0x04000CFB RID: 3323
	private float m_lastBlockedReportTimestamp;

	// Token: 0x04000CFC RID: 3324
	private bool m_busy;

	// Token: 0x04000CFD RID: 3325
	private bool m_mulliganBusy;

	// Token: 0x04000CFE RID: 3326
	private List<Spell> m_serverBlockingSpells = new List<Spell>();

	// Token: 0x04000CFF RID: 3327
	private List<SpellController> m_serverBlockingSpellControllers = new List<SpellController>();

	// Token: 0x04000D00 RID: 3328
	private List<GameState.TurnTimerUpdateListener> m_turnTimerUpdateListeners = new List<GameState.TurnTimerUpdateListener>();

	// Token: 0x04000D01 RID: 3329
	private List<GameState.TurnTimerUpdateListener> m_mulliganTimerUpdateListeners = new List<GameState.TurnTimerUpdateListener>();

	// Token: 0x04000D02 RID: 3330
	private Map<int, TurnTimerUpdate> m_turnTimerUpdates = new Map<int, TurnTimerUpdate>();

	// Token: 0x020004FA RID: 1274
	public enum CreateGamePhase
	{
		// Token: 0x0400260B RID: 9739
		INVALID,
		// Token: 0x0400260C RID: 9740
		CREATING,
		// Token: 0x0400260D RID: 9741
		CREATED
	}

	// Token: 0x020004FB RID: 1275
	// (Invoke) Token: 0x06003B7E RID: 15230
	public delegate void CreateGameCallback(GameState.CreateGamePhase phase, object userData);

	// Token: 0x020004FE RID: 1278
	// (Invoke) Token: 0x06003BC9 RID: 15305
	public delegate void GameStateInitializedCallback(GameState instance, object userData);

	// Token: 0x02000598 RID: 1432
	// (Invoke) Token: 0x0600409A RID: 16538
	public delegate void TurnChangedCallback(int oldTurn, int newTurn, object userData);

	// Token: 0x0200059A RID: 1434
	// (Invoke) Token: 0x060040A5 RID: 16549
	public delegate void TurnTimerUpdateCallback(TurnTimerUpdate update, object userData);

	// Token: 0x020005F7 RID: 1527
	// (Invoke) Token: 0x060042EF RID: 17135
	public delegate void GameOverCallback(object userData);

	// Token: 0x020005F8 RID: 1528
	// (Invoke) Token: 0x060042F3 RID: 17139
	public delegate void EntityChoicesReceivedCallback(Network.EntityChoices choices, PowerTaskList preChoiceTaskList, object userData);

	// Token: 0x020005F9 RID: 1529
	// (Invoke) Token: 0x060042F7 RID: 17143
	public delegate bool EntitiesChosenReceivedCallback(Network.EntitiesChosen chosen, Network.EntityChoices choices, object userData);

	// Token: 0x020005FA RID: 1530
	// (Invoke) Token: 0x060042FB RID: 17147
	public delegate void HeroChangedCallback(Player player, object userData);

	// Token: 0x020005FB RID: 1531
	// (Invoke) Token: 0x060042FF RID: 17151
	public delegate void OptionsReceivedCallback(object userData);

	// Token: 0x020005FC RID: 1532
	public enum ResponseMode
	{
		// Token: 0x04002A96 RID: 10902
		NONE,
		// Token: 0x04002A97 RID: 10903
		OPTION,
		// Token: 0x04002A98 RID: 10904
		SUB_OPTION,
		// Token: 0x04002A99 RID: 10905
		OPTION_TARGET,
		// Token: 0x04002A9A RID: 10906
		CHOICE
	}

	// Token: 0x020005FD RID: 1533
	private class SelectedOption
	{
		// Token: 0x06004303 RID: 17155 RVA: 0x00141E4E File Offset: 0x0014004E
		public void Clear()
		{
			this.m_main = -1;
			this.m_sub = -1;
			this.m_target = 0;
			this.m_position = 0;
		}

		// Token: 0x06004304 RID: 17156 RVA: 0x00141E6C File Offset: 0x0014006C
		public void CopyFrom(GameState.SelectedOption original)
		{
			this.m_main = original.m_main;
			this.m_sub = original.m_sub;
			this.m_target = original.m_target;
			this.m_position = original.m_position;
		}

		// Token: 0x04002A9B RID: 10907
		public int m_main = -1;

		// Token: 0x04002A9C RID: 10908
		public int m_sub = -1;

		// Token: 0x04002A9D RID: 10909
		public int m_target;

		// Token: 0x04002A9E RID: 10910
		public int m_position;
	}

	// Token: 0x020005FE RID: 1534
	private class QueuedChoice
	{
		// Token: 0x04002A9F RID: 10911
		public GameState.QueuedChoice.PacketType m_type;

		// Token: 0x04002AA0 RID: 10912
		public object m_packet;

		// Token: 0x04002AA1 RID: 10913
		public object m_eventData;

		// Token: 0x020005FF RID: 1535
		public enum PacketType
		{
			// Token: 0x04002AA3 RID: 10915
			ENTITY_CHOICES,
			// Token: 0x04002AA4 RID: 10916
			ENTITIES_CHOSEN
		}
	}

	// Token: 0x02000600 RID: 1536
	private class GameStateInitializedListener : EventListener<GameState.GameStateInitializedCallback>
	{
		// Token: 0x06004307 RID: 17159 RVA: 0x00141EB9 File Offset: 0x001400B9
		public void Fire(GameState instance)
		{
			this.m_callback(instance, this.m_userData);
		}
	}

	// Token: 0x02000601 RID: 1537
	private class CreateGameListener : EventListener<GameState.CreateGameCallback>
	{
		// Token: 0x06004309 RID: 17161 RVA: 0x00141ED5 File Offset: 0x001400D5
		public void Fire(GameState.CreateGamePhase phase)
		{
			this.m_callback(phase, this.m_userData);
		}
	}

	// Token: 0x02000602 RID: 1538
	private class OptionsReceivedListener : EventListener<GameState.OptionsReceivedCallback>
	{
		// Token: 0x0600430B RID: 17163 RVA: 0x00141EF1 File Offset: 0x001400F1
		public void Fire()
		{
			this.m_callback(this.m_userData);
		}
	}

	// Token: 0x02000603 RID: 1539
	private class OptionsSentListener : EventListener<GameState.OptionsSentCallback>
	{
		// Token: 0x0600430D RID: 17165 RVA: 0x00141F0C File Offset: 0x0014010C
		public void Fire(Network.Options.Option option)
		{
			this.m_callback(option, this.m_userData);
		}
	}

	// Token: 0x02000604 RID: 1540
	// (Invoke) Token: 0x0600430F RID: 17167
	public delegate void OptionsSentCallback(Network.Options.Option option, object userData);

	// Token: 0x02000605 RID: 1541
	private class OptionRejectedListener : EventListener<GameState.OptionRejectedCallback>
	{
		// Token: 0x06004313 RID: 17171 RVA: 0x00141F28 File Offset: 0x00140128
		public void Fire(Network.Options.Option option)
		{
			this.m_callback(option, this.m_userData);
		}
	}

	// Token: 0x02000606 RID: 1542
	// (Invoke) Token: 0x06004315 RID: 17173
	public delegate void OptionRejectedCallback(Network.Options.Option option, object userData);

	// Token: 0x02000607 RID: 1543
	private class EntityChoicesReceivedListener : EventListener<GameState.EntityChoicesReceivedCallback>
	{
		// Token: 0x06004319 RID: 17177 RVA: 0x00141F44 File Offset: 0x00140144
		public void Fire(Network.EntityChoices choices, PowerTaskList preChoiceTaskList)
		{
			this.m_callback(choices, preChoiceTaskList, this.m_userData);
		}
	}

	// Token: 0x02000608 RID: 1544
	private class EntitiesChosenReceivedListener : EventListener<GameState.EntitiesChosenReceivedCallback>
	{
		// Token: 0x0600431B RID: 17179 RVA: 0x00141F61 File Offset: 0x00140161
		public bool Fire(Network.EntitiesChosen chosen, Network.EntityChoices choices)
		{
			return this.m_callback(chosen, choices, this.m_userData);
		}
	}

	// Token: 0x02000609 RID: 1545
	private class CurrentPlayerChangedListener : EventListener<GameState.CurrentPlayerChangedCallback>
	{
		// Token: 0x0600431D RID: 17181 RVA: 0x00141F7E File Offset: 0x0014017E
		public void Fire(Player player)
		{
			this.m_callback(player, this.m_userData);
		}
	}

	// Token: 0x0200060A RID: 1546
	// (Invoke) Token: 0x0600431F RID: 17183
	public delegate void CurrentPlayerChangedCallback(Player player, object userData);

	// Token: 0x0200060B RID: 1547
	private class TurnChangedListener : EventListener<GameState.TurnChangedCallback>
	{
		// Token: 0x06004323 RID: 17187 RVA: 0x00141F9A File Offset: 0x0014019A
		public void Fire(int oldTurn, int newTurn)
		{
			this.m_callback(oldTurn, newTurn, this.m_userData);
		}
	}

	// Token: 0x0200060C RID: 1548
	private class FriendlyTurnStartedListener : EventListener<GameState.FriendlyTurnStartedCallback>
	{
		// Token: 0x06004325 RID: 17189 RVA: 0x00141FB7 File Offset: 0x001401B7
		public void Fire()
		{
			this.m_callback(this.m_userData);
		}
	}

	// Token: 0x0200060D RID: 1549
	// (Invoke) Token: 0x06004327 RID: 17191
	public delegate void FriendlyTurnStartedCallback(object userData);

	// Token: 0x0200060E RID: 1550
	private class TurnTimerUpdateListener : EventListener<GameState.TurnTimerUpdateCallback>
	{
		// Token: 0x0600432B RID: 17195 RVA: 0x00141FD2 File Offset: 0x001401D2
		public void Fire(TurnTimerUpdate update)
		{
			this.m_callback(update, this.m_userData);
		}
	}

	// Token: 0x0200060F RID: 1551
	private class SpectatorNotifyListener : EventListener<GameState.SpectatorNotifyEventCallback>
	{
		// Token: 0x0600432D RID: 17197 RVA: 0x00141FEE File Offset: 0x001401EE
		public void Fire(SpectatorNotify notify)
		{
			this.m_callback(notify, this.m_userData);
		}
	}

	// Token: 0x02000610 RID: 1552
	// (Invoke) Token: 0x0600432F RID: 17199
	public delegate void SpectatorNotifyEventCallback(SpectatorNotify notify, object userData);

	// Token: 0x02000611 RID: 1553
	private class GameOverListener : EventListener<GameState.GameOverCallback>
	{
		// Token: 0x06004333 RID: 17203 RVA: 0x0014200A File Offset: 0x0014020A
		public void Fire()
		{
			this.m_callback(this.m_userData);
		}
	}

	// Token: 0x02000612 RID: 1554
	private class HeroChangedListener : EventListener<GameState.HeroChangedCallback>
	{
		// Token: 0x06004335 RID: 17205 RVA: 0x00142025 File Offset: 0x00140225
		public void Fire(Player player)
		{
			this.m_callback(player, this.m_userData);
		}
	}

	// Token: 0x02000613 RID: 1555
	// (Invoke) Token: 0x06004337 RID: 17207
	private delegate void AppendBlockingServerItemCallback<T>(StringBuilder builder, T item);
}
