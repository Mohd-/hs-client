using System;
using System.Collections.Generic;
using bgs;
using bgs.types;
using PegasusGame;
using PegasusShared;
using SpectatorProto;
using UnityEngine;

// Token: 0x02000115 RID: 277
public class GameMgr
{
	// Token: 0x06000CDB RID: 3291 RVA: 0x00033386 File Offset: 0x00031586
	public static GameMgr Get()
	{
		return GameMgr.s_instance;
	}

	// Token: 0x06000CDC RID: 3292 RVA: 0x00033390 File Offset: 0x00031590
	public void Initialize()
	{
		this.MATCHING_POPUP_NAME = new PlatformDependentValue<string>(PlatformCategory.Screen)
		{
			PC = "MatchingPopup3D",
			Phone = "MatchingPopup3D_phone"
		};
		ApplicationMgr.Get().WillReset += new Action(this.WillReset);
	}

	// Token: 0x06000CDD RID: 3293 RVA: 0x000333D8 File Offset: 0x000315D8
	public void OnLoggedIn()
	{
		Network network = Network.Get();
		network.RegisterGameQueueHandler(new Network.GameQueueHandler(this.OnGameQueueEvent));
		network.RegisterNetHandler(16, new Network.NetHandler(this.OnGameSetup), null);
		network.RegisterNetHandler(12, new Network.NetHandler(this.OnGameCanceled), null);
		network.RegisterNetHandler(23, new Network.NetHandler(this.OnServerResult), null);
		network.AddBnetErrorListener(2, new Network.BnetErrorCallback(this.OnBnetError));
		SceneMgr.Get().RegisterSceneUnloadedEvent(new SceneMgr.SceneUnloadedCallback(this.OnSceneUnloaded));
		SceneMgr.Get().RegisterScenePreLoadEvent(new SceneMgr.ScenePreLoadCallback(this.OnScenePreLoad));
		ReconnectMgr.Get().AddTimeoutListener(new ReconnectMgr.TimeoutCallback(this.OnReconnectTimeout));
	}

	// Token: 0x06000CDE RID: 3294 RVA: 0x000334A4 File Offset: 0x000316A4
	public GameType GetGameType()
	{
		return this.m_gameType;
	}

	// Token: 0x06000CDF RID: 3295 RVA: 0x000334AC File Offset: 0x000316AC
	public GameType GetPreviousGameType()
	{
		return this.m_prevGameType;
	}

	// Token: 0x06000CE0 RID: 3296 RVA: 0x000334B4 File Offset: 0x000316B4
	public GameType GetNextGameType()
	{
		return this.m_nextGameType;
	}

	// Token: 0x06000CE1 RID: 3297 RVA: 0x000334BC File Offset: 0x000316BC
	public int GetMissionId()
	{
		return this.m_missionId;
	}

	// Token: 0x06000CE2 RID: 3298 RVA: 0x000334C4 File Offset: 0x000316C4
	public int GetPreviousMissionId()
	{
		return this.m_prevMissionId;
	}

	// Token: 0x06000CE3 RID: 3299 RVA: 0x000334CC File Offset: 0x000316CC
	public int GetNextMissionId()
	{
		return this.m_nextMissionId;
	}

	// Token: 0x06000CE4 RID: 3300 RVA: 0x000334D4 File Offset: 0x000316D4
	public ReconnectType GetReconnectType()
	{
		return this.m_reconnectType;
	}

	// Token: 0x06000CE5 RID: 3301 RVA: 0x000334DC File Offset: 0x000316DC
	public ReconnectType GetPreviousReconnectType()
	{
		return this.m_prevReconnectType;
	}

	// Token: 0x06000CE6 RID: 3302 RVA: 0x000334E4 File Offset: 0x000316E4
	public ReconnectType GetNextReconnectType()
	{
		return this.m_nextReconnectType;
	}

	// Token: 0x06000CE7 RID: 3303 RVA: 0x000334EC File Offset: 0x000316EC
	public bool IsReconnect()
	{
		return this.m_reconnectType != ReconnectType.INVALID;
	}

	// Token: 0x06000CE8 RID: 3304 RVA: 0x000334FA File Offset: 0x000316FA
	public bool IsPreviousReconnect()
	{
		return this.m_prevReconnectType != ReconnectType.INVALID;
	}

	// Token: 0x06000CE9 RID: 3305 RVA: 0x00033508 File Offset: 0x00031708
	public bool IsNextReconnect()
	{
		return this.m_nextReconnectType != ReconnectType.INVALID;
	}

	// Token: 0x06000CEA RID: 3306 RVA: 0x00033516 File Offset: 0x00031716
	public bool IsSpectator()
	{
		return this.m_spectator;
	}

	// Token: 0x06000CEB RID: 3307 RVA: 0x0003351E File Offset: 0x0003171E
	public bool WasSpectator()
	{
		return this.m_prevSpectator;
	}

	// Token: 0x06000CEC RID: 3308 RVA: 0x00033526 File Offset: 0x00031726
	public bool IsNextSpectator()
	{
		return this.m_nextSpectator;
	}

	// Token: 0x06000CED RID: 3309 RVA: 0x0003352E File Offset: 0x0003172E
	public uint GetLastEnterGameError()
	{
		return this.m_lastEnterGameError;
	}

	// Token: 0x06000CEE RID: 3310 RVA: 0x00033536 File Offset: 0x00031736
	public bool IsPendingAutoConcede()
	{
		return this.m_pendingAutoConcede;
	}

	// Token: 0x06000CEF RID: 3311 RVA: 0x0003353E File Offset: 0x0003173E
	public void SetPendingAutoConcede(bool pendingAutoConcede)
	{
		if (!Network.IsConnectedToGameServer())
		{
			return;
		}
		this.m_pendingAutoConcede = pendingAutoConcede;
	}

	// Token: 0x06000CF0 RID: 3312 RVA: 0x00033552 File Offset: 0x00031752
	public Network.GameSetup GetGameSetup()
	{
		return this.m_gameSetup;
	}

	// Token: 0x06000CF1 RID: 3313 RVA: 0x0003355A File Offset: 0x0003175A
	public FindGameState GetFindGameState()
	{
		return this.m_findGameState;
	}

	// Token: 0x06000CF2 RID: 3314 RVA: 0x00033562 File Offset: 0x00031762
	public bool IsFindingGame()
	{
		return this.m_findGameState != FindGameState.INVALID;
	}

	// Token: 0x06000CF3 RID: 3315 RVA: 0x00033570 File Offset: 0x00031770
	public bool IsAboutToStopFindingGame()
	{
		switch (this.m_findGameState)
		{
		case FindGameState.CLIENT_CANCELED:
		case FindGameState.CLIENT_ERROR:
		case FindGameState.BNET_QUEUE_CANCELED:
		case FindGameState.BNET_ERROR:
		case FindGameState.SERVER_GAME_STARTED:
		case FindGameState.SERVER_GAME_CANCELED:
			return true;
		}
		return false;
	}

	// Token: 0x06000CF4 RID: 3316 RVA: 0x000335BC File Offset: 0x000317BC
	public void RegisterFindGameEvent(GameMgr.FindGameCallback callback)
	{
		this.RegisterFindGameEvent(callback, null);
	}

	// Token: 0x06000CF5 RID: 3317 RVA: 0x000335C8 File Offset: 0x000317C8
	public void RegisterFindGameEvent(GameMgr.FindGameCallback callback, object userData)
	{
		GameMgr.FindGameListener findGameListener = new GameMgr.FindGameListener();
		findGameListener.SetCallback(callback);
		findGameListener.SetUserData(userData);
		if (this.m_findGameListeners.Contains(findGameListener))
		{
			return;
		}
		this.m_findGameListeners.Add(findGameListener);
	}

	// Token: 0x06000CF6 RID: 3318 RVA: 0x00033607 File Offset: 0x00031807
	public bool UnregisterFindGameEvent(GameMgr.FindGameCallback callback)
	{
		return this.UnregisterFindGameEvent(callback, null);
	}

	// Token: 0x06000CF7 RID: 3319 RVA: 0x00033614 File Offset: 0x00031814
	public bool UnregisterFindGameEvent(GameMgr.FindGameCallback callback, object userData)
	{
		GameMgr.FindGameListener findGameListener = new GameMgr.FindGameListener();
		findGameListener.SetCallback(callback);
		findGameListener.SetUserData(userData);
		return this.m_findGameListeners.Remove(findGameListener);
	}

	// Token: 0x06000CF8 RID: 3320 RVA: 0x00033644 File Offset: 0x00031844
	public void FindGame(GameType type, int missionId, long deckId = 0L, long aiDeckId = 0L)
	{
		this.m_lastEnterGameError = 0U;
		this.ChangeFindGameState(FindGameState.CLIENT_STARTED);
		this.m_nextGameType = type;
		this.m_nextMissionId = missionId;
		string text = this.DetermineTransitionPopupForFindGame(type, missionId);
		if (text != null)
		{
			this.ShowTransitionPopup(text);
		}
		Network.Get().FindGame(type, missionId, deckId, aiDeckId);
	}

	// Token: 0x06000CF9 RID: 3321 RVA: 0x00033694 File Offset: 0x00031894
	public void WaitForFriendChallengeToStart(int missionId)
	{
		this.m_nextGameType = 2;
		this.m_nextMissionId = missionId;
		this.m_lastEnterGameError = 0U;
		this.ChangeFindGameState(FindGameState.CLIENT_STARTED);
		this.ShowTransitionPopup("LoadingPopup");
	}

	// Token: 0x06000CFA RID: 3322 RVA: 0x000336CC File Offset: 0x000318CC
	public void SpectateGame(JoinInfo joinInfo)
	{
		GameServerInfo gameServerInfo = new GameServerInfo();
		gameServerInfo.Address = joinInfo.ServerIpAddress;
		gameServerInfo.Port = (int)joinInfo.ServerPort;
		gameServerInfo.GameHandle = joinInfo.GameHandle;
		gameServerInfo.SpectatorPassword = joinInfo.SecretKey;
		gameServerInfo.SpectatorMode = true;
		this.m_nextGameType = joinInfo.GameType;
		this.m_nextMissionId = joinInfo.MissionId;
		this.m_nextSpectator = true;
		this.m_lastEnterGameError = 0U;
		this.ChangeFindGameState(FindGameState.CLIENT_STARTED);
		this.ShowTransitionPopup("LoadingPopup");
		this.ChangeFindGameState(FindGameState.SERVER_GAME_CONNECTING, gameServerInfo);
		if (Gameplay.Get() == null)
		{
			Network.Get().SetGameServerDisconnectEventListener(new Network.GameServerDisconnectEvent(this.OnGameServerDisconnect));
		}
	}

	// Token: 0x06000CFB RID: 3323 RVA: 0x0003377F File Offset: 0x0003197F
	private void OnGameServerDisconnect(BattleNetErrors error)
	{
		this.OnGameCanceled();
	}

	// Token: 0x06000CFC RID: 3324 RVA: 0x00033788 File Offset: 0x00031988
	public void ReconnectGame(GameType type, ReconnectType reconnectType, GameServerInfo serverInfo)
	{
		this.m_nextGameType = type;
		this.m_nextMissionId = serverInfo.Mission;
		this.m_nextReconnectType = reconnectType;
		this.m_nextSpectator = serverInfo.SpectatorMode;
		this.m_lastEnterGameError = 0U;
		this.ChangeFindGameState(FindGameState.CLIENT_STARTED);
		this.ChangeFindGameState(FindGameState.SERVER_GAME_CONNECTING, serverInfo);
	}

	// Token: 0x06000CFD RID: 3325 RVA: 0x000337D4 File Offset: 0x000319D4
	public bool CancelFindGame()
	{
		if (!GameUtils.IsMatchmadeGameType(this.m_nextGameType))
		{
			return false;
		}
		if (!Network.Get().IsFindingGame())
		{
			return false;
		}
		Network.Get().CancelFindGame();
		this.ChangeFindGameState(FindGameState.CLIENT_CANCELED);
		return true;
	}

	// Token: 0x06000CFE RID: 3326 RVA: 0x00033818 File Offset: 0x00031A18
	public GameEntity CreateGameEntity()
	{
		ScenarioDbId missionId = (ScenarioDbId)this.m_missionId;
		switch (missionId)
		{
		case ScenarioDbId.TUTORIAL_ILLIDAN:
			return new Tutorial_05();
		case ScenarioDbId.TUTORIAL_CHO:
			return new Tutorial_06();
		default:
			switch (missionId)
			{
			case ScenarioDbId.LOE_CHALLENGE_DRUID_V_SCARVASH:
			case ScenarioDbId.LOE_HEROIC_SCARVASH:
				goto IL_69B;
			case ScenarioDbId.LOE_CHALLENGE_MAGE_V_SENTINEL:
			case ScenarioDbId.LOE_HEROIC_STEEL_SENTINEL:
				goto IL_6B1;
			default:
				switch (missionId)
				{
				case ScenarioDbId.LOE_SKELESAURUS:
					goto IL_6F3;
				default:
					switch (missionId)
					{
					case ScenarioDbId.LOE_SCARVASH:
						goto IL_69B;
					case ScenarioDbId.LOE_TEMPLE_ESCAPE:
						goto IL_6BC;
					case ScenarioDbId.LOE_MINE_CART:
						goto IL_6C7;
					case ScenarioDbId.TB_INSP_V_JOUST:
						return new TB03_InspireVSJoust();
					default:
						switch (missionId)
						{
						case ScenarioDbId.LOE_ARCHAEDAS:
							goto IL_6A6;
						default:
							switch (missionId)
							{
							case ScenarioDbId.TB_SHADOWTOWERS_1P_TEST:
							case ScenarioDbId.TB_SHADOWTOWERS:
								break;
							default:
								if (missionId == ScenarioDbId.TUTORIAL_HOGGER)
								{
									return new Tutorial_01();
								}
								if (missionId == ScenarioDbId.TUTORIAL_MILLHOUSE)
								{
									return new Tutorial_02();
								}
								if (missionId == ScenarioDbId.LOE_SUN_RAIDER_PHAERIX)
								{
									goto IL_690;
								}
								if (missionId == ScenarioDbId.LOE_ZINAAR)
								{
									goto IL_685;
								}
								if (missionId == ScenarioDbId.TUTORIAL_MUKLA)
								{
									return new Tutorial_03();
								}
								if (missionId == ScenarioDbId.TUTORIAL_NESINGWARY)
								{
									return new Tutorial_04();
								}
								if (missionId == ScenarioDbId.TB_CO_OP_TEST || missionId == ScenarioDbId.TB_CO_OP || missionId == ScenarioDbId.TB_CO_OP_TEST2)
								{
									goto IL_62D;
								}
								if (missionId == ScenarioDbId.LOE_GIANTFIN)
								{
									goto IL_66F;
								}
								if (missionId == ScenarioDbId.LOE_RAFAAM_2)
								{
									goto IL_6E8;
								}
								if (missionId == ScenarioDbId.LOE_STEEL_SENTINEL)
								{
									goto IL_6B1;
								}
								if (missionId == ScenarioDbId.LOE_SLITHERSPEAR)
								{
									goto IL_67A;
								}
								if (missionId == ScenarioDbId.LOE_CHALLENGE_PALADIN_V_ARCHAEDUS)
								{
									goto IL_6A6;
								}
								if (missionId == ScenarioDbId.LOE_CHALLENGE_WARLOCK_V_SUN_RAIDER)
								{
									goto IL_690;
								}
								if (missionId == ScenarioDbId.TB_KELTHUZADRAFAAM)
								{
									return new TB_KelthuzadRafaam();
								}
								if (missionId != ScenarioDbId.TB_SHADOWTOWERS_TEST)
								{
									return new StandardGameEntity();
								}
								break;
							}
							return new TB09_ShadowTowers();
						case ScenarioDbId.LOE_LADY_NAZJAR:
							goto IL_6D2;
						}
						break;
					}
					break;
				case ScenarioDbId.LOE_RAFAAM_1:
					goto IL_6DD;
				case ScenarioDbId.TB_CO_OP_PRECON:
					break;
				}
				break;
			case ScenarioDbId.LOE_CHALLENGE_ROGUE_V_SKELESAURUS:
			case ScenarioDbId.LOE_HEROIC_SKELESAURUS:
				goto IL_6F3;
			case ScenarioDbId.TB_CO_OP_1P_TEST:
			case ScenarioDbId.TB_CO_OP_V2:
				break;
			case ScenarioDbId.LOE_CHALLENGE_SHAMAN_V_GIANTFIN:
			case ScenarioDbId.LOE_HEROIC_GIANTFIN:
				goto IL_66F;
			case ScenarioDbId.LOE_CHALLENGE_WARRIOR_V_ZINAAR:
			case ScenarioDbId.LOE_HEROIC_ZINAAR:
				goto IL_685;
			case ScenarioDbId.LOE_CHALLENGE_HUNTER_V_SLITHERSPEAR:
			case ScenarioDbId.LOE_HEROIC_SLITHERSPEAR:
				goto IL_67A;
			case ScenarioDbId.LOE_CHALLENGE_PRIEST_V_NAZJAR:
			case ScenarioDbId.LOE_HEROIC_LADY_NAZJAR:
				goto IL_6D2;
			case ScenarioDbId.LOE_HEROIC_SUN_RAIDER_PHAERIX:
				goto IL_690;
			case ScenarioDbId.LOE_HEROIC_TEMPLE_ESCAPE:
				goto IL_6BC;
			case ScenarioDbId.TB_DECKBUILDING_1P_TEST:
				goto IL_638;
			case ScenarioDbId.TB_GIFTEXCHANGE_1P_TEST:
			case ScenarioDbId.TB_GIFTEXCHANGE:
				return new TB05_GiftExchange();
			case ScenarioDbId.LOE_HEROIC_ARCHAEDAS:
				goto IL_6A6;
			case ScenarioDbId.TB_CHOOSEFATEBUILD_1P_TEST:
			case ScenarioDbId.TB_CHOOSEFATEBUILD:
				return new TB_ChooseYourFateBuildaround();
			case ScenarioDbId.LOE_HEROIC_MINE_CART:
				goto IL_6C7;
			case ScenarioDbId.LOE_HEROIC_RAFAAM_1:
				goto IL_6DD;
			case ScenarioDbId.LOE_HEROIC_RAFAAM_2:
				goto IL_6E8;
			case ScenarioDbId.TB_CHOOSEFATERANDOM_1P_TEST:
			case ScenarioDbId.TB_CHOOSEFATERANDOM:
				return new TB_ChooseYourFateRandom();
			}
			IL_62D:
			return new TB02_CoOp();
			IL_66F:
			return new LOE10_Giantfin();
			IL_67A:
			return new LOE09_LordSlitherspear();
			IL_685:
			return new LOE01_Zinaar();
			IL_690:
			return new LOE02_Sun_Raider_Phaerix();
			IL_69B:
			return new LOE04_Scarvash();
			IL_6A6:
			return new LOE08_Archaedas();
			IL_6B1:
			return new LOE14_Steel_Sentinel();
			IL_6BC:
			return new LOE03_AncientTemple();
			IL_6C7:
			return new LOE07_MineCart();
			IL_6D2:
			return new LOE12_Naga();
			IL_6DD:
			return new LOE15_Boss1();
			IL_6E8:
			return new LOE16_Boss2();
			IL_6F3:
			return new LOE13_Skelesaurus();
		case ScenarioDbId.NAXX_ANUBREKHAN:
		case ScenarioDbId.NAXX_HEROIC_ANUBREKHAN:
			return new NAX01_AnubRekhan();
		case ScenarioDbId.NAXX_FAERLINA:
		case ScenarioDbId.NAXX_CHALLENGE_DRUID_V_FAERLINA:
		case ScenarioDbId.NAXX_HEROIC_FAERLINA:
			return new NAX02_Faerlina();
		case ScenarioDbId.NAXX_NOTH:
		case ScenarioDbId.NAXX_HEROIC_NOTH:
			return new NAX04_Noth();
		case ScenarioDbId.NAXX_HEIGAN:
		case ScenarioDbId.NAXX_CHALLENGE_MAGE_V_HEIGAN:
		case ScenarioDbId.NAXX_HEROIC_HEIGAN:
			return new NAX05_Heigan();
		case ScenarioDbId.NAXX_LOATHEB:
		case ScenarioDbId.NAXX_CHALLENGE_HUNTER_V_LOATHEB:
		case ScenarioDbId.NAXX_HEROIC_LOATHEB:
			return new NAX06_Loatheb();
		case ScenarioDbId.NAXX_MAEXXNA:
		case ScenarioDbId.NAXX_CHALLENGE_ROGUE_V_MAEXXNA:
		case ScenarioDbId.NAXX_HEROIC_MAEXXNA:
			return new NAX03_Maexxna();
		case ScenarioDbId.NAXX_RAZUVIOUS:
		case ScenarioDbId.NAXX_HEROIC_RAZUVIOUS:
			return new NAX07_Razuvious();
		case ScenarioDbId.NAXX_GOTHIK:
		case ScenarioDbId.NAXX_CHALLENGE_SHAMAN_V_GOTHIK:
		case ScenarioDbId.NAXX_HEROIC_GOTHIK:
			return new NAX08_Gothik();
		case ScenarioDbId.NAXX_HORSEMEN:
		case ScenarioDbId.NAXX_CHALLENGE_WARLOCK_V_HORSEMEN:
		case ScenarioDbId.NAXX_HEROIC_HORSEMEN:
			return new NAX09_Horsemen();
		case ScenarioDbId.NAXX_PATCHWERK:
		case ScenarioDbId.NAXX_HEROIC_PATCHWERK:
			return new NAX10_Patchwerk();
		case ScenarioDbId.NAXX_GROBBULUS:
		case ScenarioDbId.NAXX_CHALLENGE_WARRIOR_V_GROBBULUS:
		case ScenarioDbId.NAXX_HEROIC_GROBBULUS:
			return new NAX11_Grobbulus();
		case ScenarioDbId.NAXX_GLUTH:
		case ScenarioDbId.NAXX_HEROIC_GLUTH:
			return new NAX12_Gluth();
		case ScenarioDbId.NAXX_THADDIUS:
		case ScenarioDbId.NAXX_CHALLENGE_PRIEST_V_THADDIUS:
		case ScenarioDbId.NAXX_HEROIC_THADDIUS:
			return new NAX13_Thaddius();
		case ScenarioDbId.NAXX_KELTHUZAD:
		case ScenarioDbId.NAXX_CHALLENGE_PALADIN_V_KELTHUZAD:
		case ScenarioDbId.NAXX_HEROIC_KELTHUZAD:
			return new NAX15_KelThuzad();
		case ScenarioDbId.NAXX_SAPPHIRON:
		case ScenarioDbId.NAXX_HEROIC_SAPPHIRON:
			return new NAX14_Sapphiron();
		case ScenarioDbId.BRM_GRIM_GUZZLER:
		case ScenarioDbId.BRM_HEROIC_GRIM_GUZZLER:
		case ScenarioDbId.BRM_CHALLENGE_HUNTER_V_GUZZLER:
			return new BRM01_GrimGuzzler();
		case ScenarioDbId.BRM_DARK_IRON_ARENA:
		case ScenarioDbId.BRM_HEROIC_DARK_IRON_ARENA:
		case ScenarioDbId.BRM_CHALLENGE_MAGE_V_DARK_IRON_ARENA:
			return new BRM02_DarkIronArena();
		case ScenarioDbId.BRM_THAURISSAN:
		case ScenarioDbId.BRM_HEROIC_THAURISSAN:
			return new BRM03_Thaurissan();
		case ScenarioDbId.BRM_GARR:
		case ScenarioDbId.BRM_HEROIC_GARR:
		case ScenarioDbId.BRM_CHALLENGE_WARRIOR_V_GARR:
			return new BRM04_Garr();
		case ScenarioDbId.BRM_BARON_GEDDON:
		case ScenarioDbId.BRM_HEROIC_BARON_GEDDON:
		case ScenarioDbId.BRM_CHALLENGE_SHAMAN_V_GEDDON:
			return new BRM05_BaronGeddon();
		case ScenarioDbId.BRM_MAJORDOMO:
		case ScenarioDbId.BRM_HEROIC_MAJORDOMO:
			return new BRM06_Majordomo();
		case ScenarioDbId.BRM_OMOKK:
		case ScenarioDbId.BRM_HEROIC_OMOKK:
			return new BRM07_Omokk();
		case ScenarioDbId.BRM_DRAKKISATH:
		case ScenarioDbId.BRM_CHALLENGE_PRIEST_V_DRAKKISATH:
		case ScenarioDbId.BRM_HEROIC_DRAKKISATH:
			return new BRM08_Drakkisath();
		case ScenarioDbId.BRM_REND_BLACKHAND:
		case ScenarioDbId.BRM_CHALLENGE_DRUID_V_BLACKHAND:
		case ScenarioDbId.BRM_HEROIC_REND_BLACKHAND:
			return new BRM09_RendBlackhand();
		case ScenarioDbId.BRM_RAZORGORE:
		case ScenarioDbId.BRM_HEROIC_RAZORGORE:
		case ScenarioDbId.BRM_CHALLENGE_WARLOCK_V_RAZORGORE:
			return new BRM10_Razorgore();
		case ScenarioDbId.BRM_VAELASTRASZ:
		case ScenarioDbId.BRM_HEROIC_VAELASTRASZ:
		case ScenarioDbId.BRM_CHALLENGE_ROGUE_V_VAELASTRASZ:
			return new BRM11_Vaelastrasz();
		case ScenarioDbId.BRM_CHROMAGGUS:
		case ScenarioDbId.BRM_HEROIC_CHROMAGGUS:
			return new BRM12_Chromaggus();
		case ScenarioDbId.BRM_NEFARIAN:
		case ScenarioDbId.BRM_HEROIC_NEFARIAN:
			return new BRM13_Nefarian();
		case ScenarioDbId.BRM_OMNOTRON:
		case ScenarioDbId.BRM_HEROIC_OMNOTRON:
		case ScenarioDbId.BRM_CHALLENGE_PALADIN_V_OMNOTRON:
			return new BRM14_Omnotron();
		case ScenarioDbId.BRM_MALORIAK:
		case ScenarioDbId.BRM_HEROIC_MALORIAK:
			return new BRM15_Maloriak();
		case ScenarioDbId.BRM_ATRAMEDES:
		case ScenarioDbId.BRM_HEROIC_ATRAMEDES:
			return new BRM16_Atramedes();
		case ScenarioDbId.BRM_ZOMBIE_NEF:
		case ScenarioDbId.BRM_HEROIC_ZOMBIE_NEF:
			return new BRM17_ZombieNef();
		case ScenarioDbId.TB_RAG_V_NEF:
			return new TB01_RagVsNef();
		case ScenarioDbId.TB_DECKBUILDING:
			break;
		}
		IL_638:
		return new TB04_DeckBuilding();
	}

	// Token: 0x06000CFF RID: 3327 RVA: 0x00033F3A File Offset: 0x0003213A
	public bool IsAI()
	{
		return GameUtils.IsAIMission(this.m_missionId);
	}

	// Token: 0x06000D00 RID: 3328 RVA: 0x00033F47 File Offset: 0x00032147
	public bool WasAI()
	{
		return GameUtils.IsAIMission(this.m_prevMissionId);
	}

	// Token: 0x06000D01 RID: 3329 RVA: 0x00033F54 File Offset: 0x00032154
	public bool IsNextAI()
	{
		return GameUtils.IsAIMission(this.m_nextMissionId);
	}

	// Token: 0x06000D02 RID: 3330 RVA: 0x00033F61 File Offset: 0x00032161
	public bool IsTutorial()
	{
		return GameUtils.IsTutorialMission(this.m_missionId);
	}

	// Token: 0x06000D03 RID: 3331 RVA: 0x00033F6E File Offset: 0x0003216E
	public bool WasTutorial()
	{
		return GameUtils.IsTutorialMission(this.m_prevMissionId);
	}

	// Token: 0x06000D04 RID: 3332 RVA: 0x00033F7B File Offset: 0x0003217B
	public bool IsNextTutorial()
	{
		return GameUtils.IsTutorialMission(this.m_nextMissionId);
	}

	// Token: 0x06000D05 RID: 3333 RVA: 0x00033F88 File Offset: 0x00032188
	public bool IsPractice()
	{
		return GameUtils.IsPracticeMission(this.m_missionId);
	}

	// Token: 0x06000D06 RID: 3334 RVA: 0x00033F95 File Offset: 0x00032195
	public bool WasPractice()
	{
		return GameUtils.IsPracticeMission(this.m_prevMissionId);
	}

	// Token: 0x06000D07 RID: 3335 RVA: 0x00033FA2 File Offset: 0x000321A2
	public bool IsNextPractice()
	{
		return GameUtils.IsPracticeMission(this.m_nextMissionId);
	}

	// Token: 0x06000D08 RID: 3336 RVA: 0x00033FAF File Offset: 0x000321AF
	public bool IsClassicMission()
	{
		return GameUtils.IsClassicMission(this.m_missionId);
	}

	// Token: 0x06000D09 RID: 3337 RVA: 0x00033FBC File Offset: 0x000321BC
	public bool WasClassicMission()
	{
		return GameUtils.IsClassicMission(this.m_prevMissionId);
	}

	// Token: 0x06000D0A RID: 3338 RVA: 0x00033FC9 File Offset: 0x000321C9
	public bool IsNextClassicMission()
	{
		return GameUtils.IsClassicMission(this.m_nextMissionId);
	}

	// Token: 0x06000D0B RID: 3339 RVA: 0x00033FD6 File Offset: 0x000321D6
	public bool IsClassChallengeMission()
	{
		return GameUtils.IsClassChallengeMission(this.m_missionId);
	}

	// Token: 0x06000D0C RID: 3340 RVA: 0x00033FE3 File Offset: 0x000321E3
	public bool IsExpansionMission()
	{
		return GameUtils.IsExpansionMission(this.m_missionId);
	}

	// Token: 0x06000D0D RID: 3341 RVA: 0x00033FF0 File Offset: 0x000321F0
	public bool WasExpansionMission()
	{
		return GameUtils.IsExpansionMission(this.m_prevMissionId);
	}

	// Token: 0x06000D0E RID: 3342 RVA: 0x00033FFD File Offset: 0x000321FD
	public bool IsNextExpansionMission()
	{
		return GameUtils.IsExpansionMission(this.m_nextMissionId);
	}

	// Token: 0x06000D0F RID: 3343 RVA: 0x0003400A File Offset: 0x0003220A
	public bool IsPlay()
	{
		return this.IsRankedPlay() || this.IsUnrankedPlay();
	}

	// Token: 0x06000D10 RID: 3344 RVA: 0x00034020 File Offset: 0x00032220
	public bool WasPlay()
	{
		return this.WasRankedPlay() || this.WasUnrankedPlay();
	}

	// Token: 0x06000D11 RID: 3345 RVA: 0x00034036 File Offset: 0x00032236
	public bool IsNextPlay()
	{
		return this.IsNextRankedPlay() || this.IsNextUnrankedPlay();
	}

	// Token: 0x06000D12 RID: 3346 RVA: 0x0003404C File Offset: 0x0003224C
	public bool IsRankedPlay()
	{
		return this.m_gameType == 7;
	}

	// Token: 0x06000D13 RID: 3347 RVA: 0x00034057 File Offset: 0x00032257
	public bool WasRankedPlay()
	{
		return this.m_prevGameType == 7;
	}

	// Token: 0x06000D14 RID: 3348 RVA: 0x00034062 File Offset: 0x00032262
	public bool IsNextRankedPlay()
	{
		return this.m_nextGameType == 7;
	}

	// Token: 0x06000D15 RID: 3349 RVA: 0x0003406D File Offset: 0x0003226D
	public bool IsUnrankedPlay()
	{
		return this.m_gameType == 8;
	}

	// Token: 0x06000D16 RID: 3350 RVA: 0x00034078 File Offset: 0x00032278
	public bool WasUnrankedPlay()
	{
		return this.m_prevGameType == 8;
	}

	// Token: 0x06000D17 RID: 3351 RVA: 0x00034083 File Offset: 0x00032283
	public bool IsNextUnrankedPlay()
	{
		return this.m_nextGameType == 8;
	}

	// Token: 0x06000D18 RID: 3352 RVA: 0x0003408E File Offset: 0x0003228E
	public bool IsArena()
	{
		return this.m_gameType == 5;
	}

	// Token: 0x06000D19 RID: 3353 RVA: 0x00034099 File Offset: 0x00032299
	public bool WasArena()
	{
		return this.m_prevGameType == 5;
	}

	// Token: 0x06000D1A RID: 3354 RVA: 0x000340A4 File Offset: 0x000322A4
	public bool IsNextArena()
	{
		return this.m_nextGameType == 5;
	}

	// Token: 0x06000D1B RID: 3355 RVA: 0x000340AF File Offset: 0x000322AF
	public bool IsFriendly()
	{
		return this.m_gameType == 2;
	}

	// Token: 0x06000D1C RID: 3356 RVA: 0x000340BA File Offset: 0x000322BA
	public bool WasFriendly()
	{
		return this.m_prevGameType == 2;
	}

	// Token: 0x06000D1D RID: 3357 RVA: 0x000340C5 File Offset: 0x000322C5
	public bool IsNextFriendly()
	{
		return this.m_nextGameType == 2;
	}

	// Token: 0x06000D1E RID: 3358 RVA: 0x000340D0 File Offset: 0x000322D0
	public bool IsTavernBrawl()
	{
		return this.m_gameType == 16;
	}

	// Token: 0x06000D1F RID: 3359 RVA: 0x000340DC File Offset: 0x000322DC
	public bool IsNextTavernBrawl()
	{
		return this.m_nextGameType == 16;
	}

	// Token: 0x06000D20 RID: 3360 RVA: 0x000340E8 File Offset: 0x000322E8
	public SceneMgr.Mode GetPostGameSceneMode()
	{
		if (!this.IsSpectator())
		{
			SceneMgr.Mode result = SceneMgr.Mode.HUB;
			GameType gameType = this.m_gameType;
			switch (gameType)
			{
			case 1:
				result = SceneMgr.Mode.ADVENTURE;
				break;
			case 2:
				if (FriendChallengeMgr.Get().HasChallenge())
				{
					if (FriendChallengeMgr.Get().IsChallengeTavernBrawl())
					{
						result = SceneMgr.Mode.TAVERN_BRAWL;
					}
					else
					{
						result = SceneMgr.Mode.FRIENDLY;
					}
				}
				break;
			default:
				if (gameType == 16)
				{
					result = SceneMgr.Mode.TAVERN_BRAWL;
				}
				break;
			case 5:
				result = SceneMgr.Mode.DRAFT;
				break;
			case 7:
			case 8:
				result = SceneMgr.Mode.TOURNAMENT;
				break;
			}
			return result;
		}
		if (GameUtils.AreAllTutorialsComplete())
		{
			return SceneMgr.Mode.HUB;
		}
		return SceneMgr.Mode.INVALID;
	}

	// Token: 0x06000D21 RID: 3361 RVA: 0x0003419C File Offset: 0x0003239C
	public SceneMgr.Mode GetPostDisconnectSceneMode()
	{
		if (this.IsSpectator())
		{
			if (GameUtils.AreAllTutorialsComplete())
			{
				return SceneMgr.Mode.HUB;
			}
			return SceneMgr.Mode.INVALID;
		}
		else
		{
			if (this.IsTutorial())
			{
				return SceneMgr.Mode.INVALID;
			}
			return this.GetPostGameSceneMode();
		}
	}

	// Token: 0x06000D22 RID: 3362 RVA: 0x000341D8 File Offset: 0x000323D8
	public void PreparePostGameSceneMode(SceneMgr.Mode mode)
	{
		if (mode == SceneMgr.Mode.ADVENTURE && AdventureConfig.Get().GetCurrentSubScene() == AdventureSubScenes.Chooser)
		{
			int missionId = GameMgr.Get().GetMissionId();
			ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(missionId);
			if (record == null)
			{
				return;
			}
			int adventureId = record.AdventureId;
			if (adventureId == 0)
			{
				return;
			}
			int modeId = record.ModeId;
			if (modeId == 0)
			{
				return;
			}
			AdventureConfig.Get().SetSelectedAdventureMode((AdventureDbId)adventureId, (AdventureModeDbId)modeId);
			AdventureConfig.Get().ChangeSubSceneToSelectedAdventure();
			AdventureConfig.Get().SetMission((ScenarioDbId)missionId, false);
		}
	}

	// Token: 0x06000D23 RID: 3363 RVA: 0x00034258 File Offset: 0x00032458
	public bool IsTransitionPopupShown()
	{
		return !(this.m_transitionPopup == null) && this.m_transitionPopup.IsShown();
	}

	// Token: 0x06000D24 RID: 3364 RVA: 0x00034278 File Offset: 0x00032478
	public TransitionPopup GetTransitionPopup()
	{
		return this.m_transitionPopup;
	}

	// Token: 0x06000D25 RID: 3365 RVA: 0x00034280 File Offset: 0x00032480
	public void UpdatePresence()
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			return;
		}
		if (this.IsSpectator())
		{
			PresenceMgr presenceMgr = PresenceMgr.Get();
			if (this.IsTutorial())
			{
				presenceMgr.SetStatus(new Enum[]
				{
					PresenceStatus.SPECTATING_GAME_TUTORIAL
				});
			}
			else if (this.IsPractice())
			{
				presenceMgr.SetStatus(new Enum[]
				{
					PresenceStatus.SPECTATING_GAME_PRACTICE
				});
			}
			else if (this.IsPlay())
			{
				presenceMgr.SetStatus(new Enum[]
				{
					PresenceStatus.SPECTATING_GAME_PLAY
				});
			}
			else if (this.IsArena())
			{
				presenceMgr.SetStatus(new Enum[]
				{
					PresenceStatus.SPECTATING_GAME_ARENA
				});
			}
			else if (this.IsFriendly())
			{
				presenceMgr.SetStatus(new Enum[]
				{
					PresenceStatus.SPECTATING_GAME_FRIENDLY
				});
			}
			else if (this.IsTavernBrawl())
			{
				presenceMgr.SetStatus(new Enum[]
				{
					PresenceStatus.SPECTATING_GAME_TAVERN_BRAWL
				});
			}
			else if (this.IsExpansionMission())
			{
				ScenarioDbId missionId = (ScenarioDbId)this.m_missionId;
				presenceMgr.SetStatus_SpectatingMission(missionId);
			}
			SpectatorManager.Get().UpdateMySpectatorInfo();
			return;
		}
		if (this.IsTutorial())
		{
			ScenarioDbId missionId2 = (ScenarioDbId)this.m_missionId;
			if (missionId2 != ScenarioDbId.TUTORIAL_HOGGER)
			{
				if (missionId2 != ScenarioDbId.TUTORIAL_MILLHOUSE)
				{
					if (missionId2 != ScenarioDbId.TUTORIAL_ILLIDAN)
					{
						if (missionId2 != ScenarioDbId.TUTORIAL_CHO)
						{
							if (missionId2 != ScenarioDbId.TUTORIAL_MUKLA)
							{
								if (missionId2 == ScenarioDbId.TUTORIAL_NESINGWARY)
								{
									PresenceMgr.Get().SetStatus(new Enum[]
									{
										PresenceStatus.TUTORIAL_GAME,
										PresenceTutorial.HEMET
									});
								}
							}
							else
							{
								PresenceMgr.Get().SetStatus(new Enum[]
								{
									PresenceStatus.TUTORIAL_GAME,
									PresenceTutorial.MUKLA
								});
							}
						}
						else
						{
							PresenceMgr.Get().SetStatus(new Enum[]
							{
								PresenceStatus.TUTORIAL_GAME,
								PresenceTutorial.CHO
							});
						}
					}
					else
					{
						PresenceMgr.Get().SetStatus(new Enum[]
						{
							PresenceStatus.TUTORIAL_GAME,
							PresenceTutorial.ILLIDAN
						});
					}
				}
				else
				{
					PresenceMgr.Get().SetStatus(new Enum[]
					{
						PresenceStatus.TUTORIAL_GAME,
						PresenceTutorial.MILLHOUSE
					});
				}
			}
			else
			{
				PresenceMgr.Get().SetStatus(new Enum[]
				{
					PresenceStatus.TUTORIAL_GAME,
					PresenceTutorial.HOGGER
				});
			}
		}
		else if (this.IsPractice())
		{
			PresenceMgr.Get().SetStatus(new Enum[]
			{
				PresenceStatus.PRACTICE_GAME
			});
		}
		else if (this.IsPlay())
		{
			PresenceMgr.Get().SetStatus(new Enum[]
			{
				PresenceStatus.PLAY_GAME
			});
		}
		else if (this.IsArena())
		{
			int wins = DraftManager.Get().GetWins();
			if (wins >= 8)
			{
				int losses = DraftManager.Get().GetLosses();
				BnetPresenceMgr.Get().SetGameField(3U, string.Concat(new object[]
				{
					wins,
					",",
					losses,
					",0"
				}));
			}
			PresenceMgr.Get().SetStatus(new Enum[]
			{
				PresenceStatus.ARENA_GAME
			});
		}
		else if (this.IsFriendly())
		{
			PresenceStatus presenceStatus = PresenceStatus.FRIENDLY_GAME;
			if (FriendChallengeMgr.Get().IsChallengeTavernBrawl())
			{
				presenceStatus = PresenceStatus.TAVERN_BRAWL_FRIENDLY_GAME;
			}
			PresenceMgr.Get().SetStatus(new Enum[]
			{
				presenceStatus
			});
		}
		else if (this.IsTavernBrawl())
		{
			PresenceMgr.Get().SetStatus(new Enum[]
			{
				PresenceStatus.TAVERN_BRAWL_GAME
			});
		}
		else if (this.IsExpansionMission())
		{
			ScenarioDbId missionId3 = (ScenarioDbId)this.m_missionId;
			PresenceMgr.Get().SetStatus_PlayingMission(missionId3);
		}
		SpectatorManager.Get().UpdateMySpectatorInfo();
	}

	// Token: 0x06000D26 RID: 3366 RVA: 0x00034674 File Offset: 0x00032874
	private void WillReset()
	{
		this.m_gameType = 0;
		this.m_prevGameType = 0;
		this.m_nextGameType = 0;
		this.m_missionId = 0;
		this.m_prevMissionId = 0;
		this.m_nextMissionId = 0;
		this.m_reconnectType = ReconnectType.INVALID;
		this.m_prevReconnectType = ReconnectType.INVALID;
		this.m_nextReconnectType = ReconnectType.INVALID;
		this.m_nextSpectator = false;
		this.m_lastEnterGameError = 0U;
		this.m_findGameState = FindGameState.INVALID;
		this.m_gameSetup = null;
	}

	// Token: 0x06000D27 RID: 3367 RVA: 0x000346DC File Offset: 0x000328DC
	private void OnSceneUnloaded(SceneMgr.Mode prevMode, Scene prevScene, object userData)
	{
		if (prevMode == SceneMgr.Mode.GAMEPLAY && SceneMgr.Get().GetMode() != SceneMgr.Mode.GAMEPLAY)
		{
			this.OnGameEnded();
		}
	}

	// Token: 0x06000D28 RID: 3368 RVA: 0x000346FC File Offset: 0x000328FC
	private void OnScenePreLoad(SceneMgr.Mode prevMode, SceneMgr.Mode mode, object userData)
	{
		this.PreloadTransitionPopup();
		if (SceneMgr.Get().GetMode() == SceneMgr.Mode.HUB)
		{
			this.DestroyTransitionPopup();
		}
		if (mode == SceneMgr.Mode.GAMEPLAY && prevMode != SceneMgr.Mode.GAMEPLAY)
		{
			Screen.sleepTimeout = -1;
		}
		else if (prevMode == SceneMgr.Mode.GAMEPLAY && mode != SceneMgr.Mode.GAMEPLAY && !SpectatorManager.Get().IsInSpectatorMode())
		{
			Screen.sleepTimeout = -2;
		}
	}

	// Token: 0x06000D29 RID: 3369 RVA: 0x00034764 File Offset: 0x00032964
	private void OnServerResult()
	{
		if (!this.IsFindingGame())
		{
			return;
		}
		ServerResult serverResult = ConnectAPI.GetServerResult();
		if (serverResult.ResultCode == 1)
		{
			float num = (!serverResult.HasRetryDelaySeconds) ? 2f : serverResult.RetryDelaySeconds;
			num = Mathf.Max(num, 0.5f);
			ApplicationMgr.Get().CancelScheduledCallback(new ApplicationMgr.ScheduledCallback(this.OnServerResult_Retry), null);
			ApplicationMgr.Get().ScheduleCallback(num, true, new ApplicationMgr.ScheduledCallback(this.OnServerResult_Retry), null);
		}
		else if (serverResult.ResultCode == 2)
		{
			this.OnGameCanceled();
		}
	}

	// Token: 0x06000D2A RID: 3370 RVA: 0x00034800 File Offset: 0x00032A00
	private void OnServerResult_Retry(object userData)
	{
		Network.Get().RetryGotoGameServer();
	}

	// Token: 0x06000D2B RID: 3371 RVA: 0x00034810 File Offset: 0x00032A10
	private void ChangeBoardIfNecessary()
	{
		int board = this.m_gameSetup.Board;
		string text = Cheats.Get().GetBoard();
		bool flag = false;
		if (!string.IsNullOrEmpty(text))
		{
			board = GameUtils.GetBoardIdFromAssetName(text);
			flag = true;
		}
		if (!flag && DemoMgr.Get().IsExpoDemo())
		{
			text = Vars.Key("Demo.ForceBoard").GetStr(null);
			if (text != null)
			{
				board = GameUtils.GetBoardIdFromAssetName(text);
			}
		}
		this.m_gameSetup.Board = board;
	}

	// Token: 0x06000D2C RID: 3372 RVA: 0x0003488C File Offset: 0x00032A8C
	private void PreloadTransitionPopup()
	{
		switch (SceneMgr.Get().GetMode())
		{
		case SceneMgr.Mode.TOURNAMENT:
		case SceneMgr.Mode.DRAFT:
		case SceneMgr.Mode.TAVERN_BRAWL:
			this.LoadTransitionPopup(this.MATCHING_POPUP_NAME);
			break;
		case SceneMgr.Mode.FRIENDLY:
		case SceneMgr.Mode.ADVENTURE:
			this.LoadTransitionPopup("LoadingPopup");
			break;
		}
	}

	// Token: 0x06000D2D RID: 3373 RVA: 0x000348FC File Offset: 0x00032AFC
	private string DetermineTransitionPopupForFindGame(GameType gameType, int missionId)
	{
		if (gameType == 16)
		{
			switch (Network.TranslateGameTypeToBnet(gameType, missionId))
			{
			case 16:
			case 18:
				return this.MATCHING_POPUP_NAME;
			}
			return "LoadingPopup";
		}
		switch (gameType)
		{
		case 4:
			return null;
		case 5:
		case 7:
		case 8:
			return this.MATCHING_POPUP_NAME;
		}
		return "LoadingPopup";
	}

	// Token: 0x06000D2E RID: 3374 RVA: 0x00034980 File Offset: 0x00032B80
	private void LoadTransitionPopup(string popupName)
	{
		GameObject gameObject = AssetLoader.Get().LoadActor(popupName, false, false);
		if (gameObject == null)
		{
			Error.AddDevFatal("GameMgr.LoadTransitionPopup() - Failed to load {0}", new object[]
			{
				popupName
			});
			return;
		}
		this.m_transitionPopup = gameObject.GetComponent<TransitionPopup>();
		this.m_initialTransitionPopupPos = this.m_transitionPopup.transform.position;
		this.m_transitionPopup.RegisterMatchCanceledEvent(new TransitionPopup.MatchCanceledEvent(this.OnTransitionPopupCanceled));
		SceneUtils.SetLayer(this.m_transitionPopup, GameLayer.IgnoreFullScreenEffects);
	}

	// Token: 0x06000D2F RID: 3375 RVA: 0x00034A04 File Offset: 0x00032C04
	private void ShowTransitionPopup(string popupName)
	{
		Type type = this.s_transitionPopupNameToType[popupName];
		if (!this.m_transitionPopup || this.m_transitionPopup.GetType() != type)
		{
			this.DestroyTransitionPopup();
			this.LoadTransitionPopup(popupName);
		}
		if (this.m_transitionPopup.IsShown())
		{
			return;
		}
		if (Box.Get() != null && Box.Get().GetState() != Box.State.OPEN)
		{
			Vector3 cameraPosition = Box.Get().m_Camera.GetCameraPosition(BoxCamera.State.OPENED);
			Vector3 vector = cameraPosition - this.m_initialTransitionPopupPos;
			Vector3 position = Box.Get().GetBoxCamera().transform.position;
			Vector3 position2 = position - vector;
			this.m_transitionPopup.transform.position = position2;
		}
		AdventureDbId adventureId = GameUtils.GetAdventureId(this.m_nextMissionId);
		this.m_transitionPopup.SetAdventureId(adventureId);
		this.m_transitionPopup.Show();
	}

	// Token: 0x06000D30 RID: 3376 RVA: 0x00034AF0 File Offset: 0x00032CF0
	private void OnTransitionPopupCanceled()
	{
		bool flag = Network.Get().IsFindingGame();
		if (flag)
		{
			Network.Get().CancelFindGame();
		}
		this.ChangeFindGameState(FindGameState.CLIENT_CANCELED);
		if (!flag)
		{
			this.ChangeFindGameState(FindGameState.INVALID);
		}
	}

	// Token: 0x06000D31 RID: 3377 RVA: 0x00034B2E File Offset: 0x00032D2E
	private void HideTransitionPopup()
	{
		if (this.m_transitionPopup)
		{
			this.m_transitionPopup.Hide();
		}
	}

	// Token: 0x06000D32 RID: 3378 RVA: 0x00034B4C File Offset: 0x00032D4C
	private void DestroyTransitionPopup()
	{
		if (this.m_transitionPopup)
		{
			Object.Destroy(this.m_transitionPopup.gameObject);
		}
	}

	// Token: 0x06000D33 RID: 3379 RVA: 0x00034B7C File Offset: 0x00032D7C
	private bool GetFriendlyErrorMessage(int errorCode, ref string headerKey, ref string messageKey, ref object[] messageParams)
	{
		switch (errorCode)
		{
		case 1000500:
			headerKey = "GLOBAL_ERROR_GENERIC_HEADER";
			messageKey = "GLOBAL_ERROR_FIND_GAME_SCENARIO_INCORRECT_NUM_PLAYERS";
			return true;
		case 1000501:
			headerKey = "GLOBAL_ERROR_GENERIC_HEADER";
			messageKey = "GLOBAL_ERROR_FIND_GAME_SCENARIO_NO_DECK_SPECIFIED";
			return true;
		case 1000502:
			headerKey = "GLOBAL_ERROR_GENERIC_HEADER";
			messageKey = "GLOBAL_ERROR_FIND_GAME_SCENARIO_MISCONFIGURED";
			return true;
		default:
			if (errorCode == 1001000)
			{
				headerKey = "GLOBAL_TAVERN_BRAWL";
				messageKey = "GLOBAL_TAVERN_BRAWL_ERROR_SEASON_INCREMENTED";
				TavernBrawlManager.Get().RefreshServerData();
				return true;
			}
			if (errorCode == 1001001)
			{
				headerKey = "GLOBAL_TAVERN_BRAWL";
				messageKey = "GLOBAL_TAVERN_BRAWL_ERROR_NOT_ACTIVE";
				TavernBrawlManager.Get().RefreshServerData();
				return true;
			}
			if (errorCode != 1002002 && errorCode != 1002007)
			{
				return false;
			}
			headerKey = "GLOBAL_ERROR_GENERIC_HEADER";
			messageKey = "GLUE_ERROR_DECK_RULESET_RULE_VIOLATION";
			return true;
		}
	}

	// Token: 0x06000D34 RID: 3380 RVA: 0x00034C4C File Offset: 0x00032E4C
	private void OnGameQueueEvent(QueueEvent queueEvent)
	{
		FindGameState? findGameState = default(FindGameState?);
		GameMgr.s_bnetToFindGameResultMap.TryGetValue(queueEvent.EventType, out findGameState);
		if (queueEvent.BnetError != 0)
		{
			this.m_lastEnterGameError = (uint)queueEvent.BnetError;
		}
		if (findGameState == null)
		{
			return;
		}
		if (queueEvent.EventType == 5)
		{
			int bnetError = queueEvent.BnetError;
			if (bnetError == 25017)
			{
				return;
			}
			string empty = string.Empty;
			string messageKey = null;
			object[] messageArgs = new object[0];
			if (this.GetFriendlyErrorMessage(queueEvent.BnetError, ref empty, ref messageKey, ref messageArgs))
			{
				Error.AddWarningLoc(empty, messageKey, messageArgs);
				findGameState = new FindGameState?(FindGameState.BNET_QUEUE_CANCELED);
				this.HandleGameCanceled();
			}
		}
		if (queueEvent.EventType == 9)
		{
			queueEvent.GameServer.Mission = this.m_nextMissionId;
			this.ChangeFindGameState(findGameState.Value, queueEvent, queueEvent.GameServer, null);
		}
		else
		{
			this.ChangeFindGameState(findGameState.Value, queueEvent);
		}
	}

	// Token: 0x06000D35 RID: 3381 RVA: 0x00034D48 File Offset: 0x00032F48
	private void OnGameSetup()
	{
		if (SpectatorManager.Get().IsSpectatingOpposingSide() && this.m_gameSetup != null)
		{
			return;
		}
		this.m_gameSetup = Network.GetGameSetupInfo();
		this.ChangeBoardIfNecessary();
		if (this.m_findGameState == FindGameState.INVALID && this.m_gameType == null)
		{
			Debug.LogError(string.Format("GameMgr.OnGameStarting() - Received {0} packet even though we're not looking for a game.", 16));
			return;
		}
		this.m_prevGameType = this.m_gameType;
		this.m_gameType = this.m_nextGameType;
		this.m_nextGameType = 0;
		this.m_prevMissionId = this.m_missionId;
		this.m_missionId = this.m_nextMissionId;
		this.m_nextMissionId = 0;
		this.m_prevReconnectType = this.m_reconnectType;
		this.m_reconnectType = this.m_nextReconnectType;
		this.m_nextReconnectType = ReconnectType.INVALID;
		this.m_prevSpectator = this.m_spectator;
		this.m_spectator = this.m_nextSpectator;
		this.m_nextSpectator = false;
		this.ChangeFindGameState(FindGameState.SERVER_GAME_STARTED);
	}

	// Token: 0x06000D36 RID: 3382 RVA: 0x00034E34 File Offset: 0x00033034
	private void OnGameCanceled()
	{
		this.HandleGameCanceled();
		Network.GameCancelInfo gameCancelInfo = Network.GetGameCancelInfo();
		this.ChangeFindGameState(FindGameState.SERVER_GAME_CANCELED, gameCancelInfo);
	}

	// Token: 0x06000D37 RID: 3383 RVA: 0x00034E58 File Offset: 0x00033058
	public bool OnBnetError(BnetErrorInfo info, object userData)
	{
		BnetFeature feature = info.GetFeature();
		if (feature == 2)
		{
			BattleNetErrors error = info.GetError();
			this.m_lastEnterGameError = error;
			string text = null;
			bool flag = false;
			FindGameState state = FindGameState.BNET_ERROR;
			BattleNetErrors battleNetErrors = error;
			if (battleNetErrors == 25000 || battleNetErrors == 25004 || battleNetErrors == 25009)
			{
				text = error.ToString();
				flag = true;
			}
			if (!flag)
			{
				string empty = string.Empty;
				string messageKey = null;
				object[] messageArgs = new object[0];
				if (this.GetFriendlyErrorMessage((int)this.m_lastEnterGameError, ref empty, ref messageKey, ref messageArgs))
				{
					Error.AddWarningLoc(empty, messageKey, messageArgs);
					text = this.m_lastEnterGameError.ToString();
					state = FindGameState.BNET_QUEUE_CANCELED;
					flag = true;
				}
			}
			if (!flag && info.GetFeatureEvent() == 16)
			{
				flag = true;
			}
			if (flag)
			{
				string text2 = string.Format("GameMgr.OnBnetError() - received error {0} {1}", this.m_lastEnterGameError, text);
				Log.BattleNet.Print(LogLevel.Error, text2, new object[0]);
				if (!Log.BattleNet.CanPrint(LogTarget.CONSOLE, LogLevel.Error, false))
				{
					string text3 = string.Format("[{0}] {1}", "BattleNet", text2);
					Debug.LogError(text3);
				}
				this.HandleGameCanceled();
				this.ChangeFindGameState(state);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000D38 RID: 3384 RVA: 0x00034F9C File Offset: 0x0003319C
	private void HandleGameCanceled()
	{
		this.m_nextGameType = 0;
		this.m_nextMissionId = 0;
		this.m_nextReconnectType = ReconnectType.INVALID;
		this.m_nextSpectator = false;
		Network.Get().ClearLastGameServerJoined();
	}

	// Token: 0x06000D39 RID: 3385 RVA: 0x00034FCF File Offset: 0x000331CF
	private bool OnReconnectTimeout(object userData)
	{
		this.HandleGameCanceled();
		this.ChangeFindGameState(FindGameState.CLIENT_CANCELED);
		this.ChangeFindGameState(FindGameState.INVALID);
		return false;
	}

	// Token: 0x06000D3A RID: 3386 RVA: 0x00034FE8 File Offset: 0x000331E8
	private bool ChangeFindGameState(FindGameState state)
	{
		return this.ChangeFindGameState(state, null, null, null);
	}

	// Token: 0x06000D3B RID: 3387 RVA: 0x00034FF4 File Offset: 0x000331F4
	private bool ChangeFindGameState(FindGameState state, QueueEvent queueEvent)
	{
		return this.ChangeFindGameState(state, queueEvent, null, null);
	}

	// Token: 0x06000D3C RID: 3388 RVA: 0x00035000 File Offset: 0x00033200
	private bool ChangeFindGameState(FindGameState state, GameServerInfo serverInfo)
	{
		return this.ChangeFindGameState(state, null, serverInfo, null);
	}

	// Token: 0x06000D3D RID: 3389 RVA: 0x0003500C File Offset: 0x0003320C
	private bool ChangeFindGameState(FindGameState state, Network.GameCancelInfo cancelInfo)
	{
		return this.ChangeFindGameState(state, null, null, cancelInfo);
	}

	// Token: 0x06000D3E RID: 3390 RVA: 0x00035018 File Offset: 0x00033218
	private bool ChangeFindGameState(FindGameState state, QueueEvent queueEvent, GameServerInfo serverInfo, Network.GameCancelInfo cancelInfo)
	{
		this.m_findGameState = state;
		FindGameEventData findGameEventData = new FindGameEventData();
		findGameEventData.m_state = state;
		findGameEventData.m_gameServer = serverInfo;
		findGameEventData.m_cancelInfo = cancelInfo;
		if (queueEvent != null)
		{
			findGameEventData.m_queueMinSeconds = queueEvent.MinSeconds;
			findGameEventData.m_queueMaxSeconds = queueEvent.MaxSeconds;
		}
		switch (state)
		{
		case FindGameState.CLIENT_CANCELED:
		case FindGameState.CLIENT_ERROR:
		case FindGameState.BNET_QUEUE_CANCELED:
		case FindGameState.BNET_ERROR:
		case FindGameState.SERVER_GAME_STARTED:
		case FindGameState.SERVER_GAME_CANCELED:
			Network.Get().RemoveGameServerDisconnectEventListener(new Network.GameServerDisconnectEvent(this.OnGameServerDisconnect));
			break;
		}
		bool flag = this.FireFindGameEvent(findGameEventData);
		if (!flag)
		{
			this.DoDefaultFindGameEventBehavior(findGameEventData);
		}
		this.FinalizeState(findGameEventData);
		return flag;
	}

	// Token: 0x06000D3F RID: 3391 RVA: 0x000350D8 File Offset: 0x000332D8
	private bool FireFindGameEvent(FindGameEventData eventData)
	{
		bool flag = false;
		foreach (GameMgr.FindGameListener findGameListener in this.m_findGameListeners.ToArray())
		{
			flag = (findGameListener.Fire(eventData) || flag);
		}
		return flag;
	}

	// Token: 0x06000D40 RID: 3392 RVA: 0x00035120 File Offset: 0x00033320
	private void DoDefaultFindGameEventBehavior(FindGameEventData eventData)
	{
		switch (eventData.m_state)
		{
		case FindGameState.CLIENT_ERROR:
		case FindGameState.BNET_ERROR:
			Error.AddWarningLoc("GLOBAL_ERROR_GENERIC_HEADER", "GLOBAL_ERROR_GAME_DENIED", new object[0]);
			this.HideTransitionPopup();
			break;
		case FindGameState.BNET_QUEUE_CANCELED:
			this.HideTransitionPopup();
			break;
		case FindGameState.SERVER_GAME_CONNECTING:
			Network.Get().GotoGameServer(eventData.m_gameServer, this.IsNextReconnect());
			break;
		case FindGameState.SERVER_GAME_STARTED:
			if (Box.Get() != null)
			{
				LoadingScreen.Get().SetFreezeFrameCamera(Box.Get().GetCamera());
				LoadingScreen.Get().SetTransitionAudioListener(Box.Get().GetAudioListener());
			}
			if (SceneMgr.Get().GetMode() == SceneMgr.Mode.GAMEPLAY)
			{
				if (!SpectatorManager.Get().IsSpectatingOpposingSide())
				{
					SceneMgr.Get().ReloadMode();
				}
			}
			else
			{
				SceneMgr.Get().SetNextMode(SceneMgr.Mode.GAMEPLAY);
			}
			break;
		case FindGameState.SERVER_GAME_CANCELED:
			if (eventData.m_cancelInfo != null)
			{
				Network.GameCancelInfo.Reason cancelReason = eventData.m_cancelInfo.CancelReason;
				if (cancelReason != Network.GameCancelInfo.Reason.OPPONENT_TIMEOUT)
				{
					Error.AddDevWarning("GAME ERROR", "The Game Server canceled the game. Error: {0}", new object[]
					{
						eventData.m_cancelInfo.CancelReason
					});
				}
				else
				{
					Error.AddWarningLoc("GLOBAL_ERROR_GENERIC_HEADER", "GLOBAL_ERROR_GAME_OPPONENT_TIMEOUT", new object[0]);
				}
			}
			this.HideTransitionPopup();
			break;
		}
	}

	// Token: 0x06000D41 RID: 3393 RVA: 0x0003529C File Offset: 0x0003349C
	private void FinalizeState(FindGameEventData eventData)
	{
		switch (eventData.m_state)
		{
		case FindGameState.CLIENT_ERROR:
		case FindGameState.BNET_QUEUE_CANCELED:
		case FindGameState.BNET_ERROR:
		case FindGameState.SERVER_GAME_STARTED:
		case FindGameState.SERVER_GAME_CANCELED:
			this.ChangeFindGameState(FindGameState.INVALID);
			break;
		}
	}

	// Token: 0x06000D42 RID: 3394 RVA: 0x000352F0 File Offset: 0x000334F0
	private void OnGameEnded()
	{
		this.m_prevGameType = this.m_gameType;
		this.m_gameType = 0;
		this.m_prevMissionId = this.m_missionId;
		this.m_missionId = 0;
		this.m_prevReconnectType = this.m_reconnectType;
		this.m_reconnectType = ReconnectType.INVALID;
		this.m_prevSpectator = this.m_spectator;
		this.m_spectator = false;
		this.m_lastEnterGameError = 0U;
		this.m_pendingAutoConcede = false;
		this.m_gameSetup = null;
	}

	// Token: 0x04000703 RID: 1795
	private const string MATCHING_POPUP_PC_NAME = "MatchingPopup3D";

	// Token: 0x04000704 RID: 1796
	private const string MATCHING_POPUP_PHONE_NAME = "MatchingPopup3D_phone";

	// Token: 0x04000705 RID: 1797
	private const string LOADING_POPUP_NAME = "LoadingPopup";

	// Token: 0x04000706 RID: 1798
	private PlatformDependentValue<string> MATCHING_POPUP_NAME;

	// Token: 0x04000707 RID: 1799
	private readonly Map<string, Type> s_transitionPopupNameToType = new Map<string, Type>
	{
		{
			"MatchingPopup3D",
			typeof(MatchingPopupDisplay)
		},
		{
			"MatchingPopup3D_phone",
			typeof(MatchingPopupDisplay)
		},
		{
			"LoadingPopup",
			typeof(LoadingPopupDisplay)
		}
	};

	// Token: 0x04000708 RID: 1800
	private static GameMgr s_instance = new GameMgr();

	// Token: 0x04000709 RID: 1801
	private GameType m_gameType;

	// Token: 0x0400070A RID: 1802
	private GameType m_prevGameType;

	// Token: 0x0400070B RID: 1803
	private GameType m_nextGameType;

	// Token: 0x0400070C RID: 1804
	private int m_missionId;

	// Token: 0x0400070D RID: 1805
	private int m_prevMissionId;

	// Token: 0x0400070E RID: 1806
	private int m_nextMissionId;

	// Token: 0x0400070F RID: 1807
	private ReconnectType m_reconnectType;

	// Token: 0x04000710 RID: 1808
	private ReconnectType m_prevReconnectType;

	// Token: 0x04000711 RID: 1809
	private ReconnectType m_nextReconnectType;

	// Token: 0x04000712 RID: 1810
	private bool m_spectator;

	// Token: 0x04000713 RID: 1811
	private bool m_prevSpectator;

	// Token: 0x04000714 RID: 1812
	private bool m_nextSpectator;

	// Token: 0x04000715 RID: 1813
	private uint m_lastEnterGameError;

	// Token: 0x04000716 RID: 1814
	private bool m_pendingAutoConcede;

	// Token: 0x04000717 RID: 1815
	private FindGameState m_findGameState;

	// Token: 0x04000718 RID: 1816
	private List<GameMgr.FindGameListener> m_findGameListeners = new List<GameMgr.FindGameListener>();

	// Token: 0x04000719 RID: 1817
	private TransitionPopup m_transitionPopup;

	// Token: 0x0400071A RID: 1818
	private Vector3 m_initialTransitionPopupPos;

	// Token: 0x0400071B RID: 1819
	private Network.GameSetup m_gameSetup;

	// Token: 0x0400071C RID: 1820
	private static Map<QueueEvent.Type, FindGameState?> s_bnetToFindGameResultMap = new Map<QueueEvent.Type, FindGameState?>
	{
		{
			0,
			default(FindGameState?)
		},
		{
			1,
			new FindGameState?(FindGameState.BNET_QUEUE_ENTERED)
		},
		{
			2,
			default(FindGameState?)
		},
		{
			3,
			new FindGameState?(FindGameState.BNET_QUEUE_DELAYED)
		},
		{
			4,
			new FindGameState?(FindGameState.BNET_QUEUE_UPDATED)
		},
		{
			5,
			new FindGameState?(FindGameState.BNET_ERROR)
		},
		{
			6,
			new FindGameState?(FindGameState.BNET_ERROR)
		},
		{
			7,
			default(FindGameState?)
		},
		{
			8,
			new FindGameState?(FindGameState.BNET_QUEUE_CANCELED)
		},
		{
			9,
			new FindGameState?(FindGameState.SERVER_GAME_CONNECTING)
		},
		{
			10,
			new FindGameState?(FindGameState.BNET_ERROR)
		}
	};

	// Token: 0x020002A5 RID: 677
	// (Invoke) Token: 0x060024B4 RID: 9396
	public delegate bool FindGameCallback(FindGameEventData eventData, object userData);

	// Token: 0x020002DD RID: 733
	private class FindGameListener : EventListener<GameMgr.FindGameCallback>
	{
		// Token: 0x0600267A RID: 9850 RVA: 0x000BB9BC File Offset: 0x000B9BBC
		public bool Fire(FindGameEventData eventData)
		{
			return this.m_callback(eventData, this.m_userData);
		}
	}
}
