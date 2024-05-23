using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using bgs;
using bgs.types;
using bnet.protocol.attribute;
using BobNetProto;
using PegasusGame;
using PegasusShared;
using PegasusUtil;
using UnityEngine;
using WTCG.BI;

// Token: 0x020000B7 RID: 183
public class ConnectAPI : IClientConnectionListener<PegasusPacket>
{
	// Token: 0x1700015B RID: 347
	// (get) Token: 0x060008A8 RID: 2216 RVA: 0x00021ECF File Offset: 0x000200CF
	// (set) Token: 0x060008A9 RID: 2217 RVA: 0x00021ED6 File Offset: 0x000200D6
	public static List<BattleNetErrors> GameServerDisconnectEvents { get; private set; }

	// Token: 0x060008AA RID: 2218 RVA: 0x00021EE0 File Offset: 0x000200E0
	public static bool ConnectInit()
	{
		ConnectAPI.s_gameStartState = ConnectAPI.GameStartState.INVALID;
		ConnectAPI.s_errorList.Clear();
		if (!ConnectAPI.s_initialized)
		{
			ConnectAPI.s_connectAPI = new ConnectAPI();
			ApplicationMgr.Get().WillReset += new Action(ConnectAPI.WillReset);
			ConnectAPI.s_gameServer = new ClientConnection<PegasusPacket>();
			ConnectAPI.s_gameServer.AddListener(ConnectAPI.s_connectAPI, ConnectAPI.s_gamePackets);
			ConnectAPI.s_gameServer.AddConnectHandler(new ConnectHandler(ConnectAPI.s_connectAPI.OnGameServerConnectCallback));
			ConnectAPI.s_gameServer.AddDisconnectHandler(new DisconnectHandler(ConnectAPI.s_connectAPI.OnGameServerDisconnectCallback));
			ConnectAPI.s_debugListener = new ServerConnection<PegasusPacket>();
			ConnectAPI.s_debugListener.Open(ConnectAPI.DEBUG_CLIENT_TCP_PORT);
			ConnectAPI.s_packetDecoders.Add(116, new ConnectAPI.PongPacketDecoder());
			ConnectAPI.s_packetDecoders.Add(169, new ConnectAPI.DefaultProtobufPacketDecoder<Deadend>());
			ConnectAPI.s_packetDecoders.Add(167, new ConnectAPI.DefaultProtobufPacketDecoder<DeadendUtil>());
			ConnectAPI.s_packetDecoders.Add(123, new ConnectAPI.DefaultProtobufPacketDecoder<DebugConsoleCommand>());
			ConnectAPI.s_packetDecoders.Add(124, new ConnectAPI.DefaultProtobufPacketDecoder<DebugConsoleResponse>());
			ConnectAPI.s_packetDecoders.Add(14, new ConnectAPI.DefaultProtobufPacketDecoder<AllOptions>());
			ConnectAPI.s_packetDecoders.Add(5, new ConnectAPI.DefaultProtobufPacketDecoder<DebugMessage>());
			ConnectAPI.s_packetDecoders.Add(17, new ConnectAPI.DefaultProtobufPacketDecoder<EntityChoices>());
			ConnectAPI.s_packetDecoders.Add(13, new ConnectAPI.DefaultProtobufPacketDecoder<EntitiesChosen>());
			ConnectAPI.s_packetDecoders.Add(16, new ConnectAPI.DefaultProtobufPacketDecoder<GameSetup>());
			ConnectAPI.s_packetDecoders.Add(19, new ConnectAPI.DefaultProtobufPacketDecoder<PowerHistory>());
			ConnectAPI.s_packetDecoders.Add(15, new ConnectAPI.DefaultProtobufPacketDecoder<UserUI>());
			ConnectAPI.s_packetDecoders.Add(9, new ConnectAPI.DefaultProtobufPacketDecoder<TurnTimer>());
			ConnectAPI.s_packetDecoders.Add(10, new ConnectAPI.DefaultProtobufPacketDecoder<NAckOption>());
			ConnectAPI.s_packetDecoders.Add(12, new ConnectAPI.DefaultProtobufPacketDecoder<GameCanceled>());
			ConnectAPI.s_packetDecoders.Add(23, new ConnectAPI.DefaultProtobufPacketDecoder<ServerResult>());
			ConnectAPI.s_packetDecoders.Add(24, new ConnectAPI.DefaultProtobufPacketDecoder<SpectatorNotify>());
			ConnectAPI.s_packetDecoders.Add(289, new ConnectAPI.DefaultProtobufPacketDecoder<Disconnected>());
			ConnectAPI.s_packetDecoders.Add(202, new ConnectAPI.DefaultProtobufPacketDecoder<DeckList>());
			ConnectAPI.s_packetDecoders.Add(207, new ConnectAPI.DefaultProtobufPacketDecoder<Collection>());
			ConnectAPI.s_packetDecoders.Add(215, new ConnectAPI.DefaultProtobufPacketDecoder<GetDeckContentsResponse>());
			ConnectAPI.s_packetDecoders.Add(216, new ConnectAPI.DefaultProtobufPacketDecoder<DBAction>());
			ConnectAPI.s_packetDecoders.Add(217, new ConnectAPI.DefaultProtobufPacketDecoder<DeckCreated>());
			ConnectAPI.s_packetDecoders.Add(218, new ConnectAPI.DefaultProtobufPacketDecoder<DeckDeleted>());
			ConnectAPI.s_packetDecoders.Add(219, new ConnectAPI.DefaultProtobufPacketDecoder<DeckRenamed>());
			ConnectAPI.s_packetDecoders.Add(212, new ConnectAPI.DefaultProtobufPacketDecoder<ProfileNotices>());
			ConnectAPI.s_packetDecoders.Add(224, new ConnectAPI.DefaultProtobufPacketDecoder<BoosterList>());
			ConnectAPI.s_packetDecoders.Add(226, new ConnectAPI.DefaultProtobufPacketDecoder<BoosterContent>());
			ConnectAPI.s_packetDecoders.Add(208, new ConnectAPI.DefaultProtobufPacketDecoder<GamesInfo>());
			ConnectAPI.s_packetDecoders.Add(231, new ConnectAPI.DefaultProtobufPacketDecoder<ProfileDeckLimit>());
			ConnectAPI.s_packetDecoders.Add(262, new ConnectAPI.DefaultProtobufPacketDecoder<ArcaneDustBalance>());
			ConnectAPI.s_packetDecoders.Add(278, new ConnectAPI.DefaultProtobufPacketDecoder<GoldBalance>());
			ConnectAPI.s_packetDecoders.Add(233, new ConnectAPI.DefaultProtobufPacketDecoder<ProfileProgress>());
			ConnectAPI.s_packetDecoders.Add(270, new ConnectAPI.DefaultProtobufPacketDecoder<PlayerRecords>());
			ConnectAPI.s_packetDecoders.Add(271, new ConnectAPI.DefaultProtobufPacketDecoder<RewardProgress>());
			ConnectAPI.s_packetDecoders.Add(232, new ConnectAPI.DefaultProtobufPacketDecoder<MedalInfo>());
			ConnectAPI.s_packetDecoders.Add(241, new ConnectAPI.DefaultProtobufPacketDecoder<ClientOptions>());
			ConnectAPI.s_packetDecoders.Add(246, new ConnectAPI.DefaultProtobufPacketDecoder<DraftBeginning>());
			ConnectAPI.s_packetDecoders.Add(247, new ConnectAPI.DefaultProtobufPacketDecoder<DraftRetired>());
			ConnectAPI.s_packetDecoders.Add(248, new ConnectAPI.DefaultProtobufPacketDecoder<DraftChoicesAndContents>());
			ConnectAPI.s_packetDecoders.Add(249, new ConnectAPI.DefaultProtobufPacketDecoder<DraftChosen>());
			ConnectAPI.s_packetDecoders.Add(288, new ConnectAPI.DefaultProtobufPacketDecoder<DraftRewardsAcked>());
			ConnectAPI.s_packetDecoders.Add(251, new ConnectAPI.DefaultProtobufPacketDecoder<DraftError>());
			ConnectAPI.s_packetDecoders.Add(252, new ConnectAPI.DefaultProtobufPacketDecoder<Achieves>());
			ConnectAPI.s_packetDecoders.Add(285, new ConnectAPI.DefaultProtobufPacketDecoder<ValidateAchieveResponse>());
			ConnectAPI.s_packetDecoders.Add(282, new ConnectAPI.DefaultProtobufPacketDecoder<CancelQuestResponse>());
			ConnectAPI.s_packetDecoders.Add(264, new ConnectAPI.DefaultProtobufPacketDecoder<GuardianVars>());
			ConnectAPI.s_packetDecoders.Add(260, new ConnectAPI.DefaultProtobufPacketDecoder<CardValues>());
			ConnectAPI.s_packetDecoders.Add(258, new ConnectAPI.DefaultProtobufPacketDecoder<BoughtSoldCard>());
			ConnectAPI.s_packetDecoders.Add(269, new ConnectAPI.DefaultProtobufPacketDecoder<MassDisenchantResponse>());
			ConnectAPI.s_packetDecoders.Add(265, new ConnectAPI.DefaultProtobufPacketDecoder<BattlePayStatusResponse>());
			ConnectAPI.s_packetDecoders.Add(295, new ConnectAPI.DefaultProtobufPacketDecoder<ThirdPartyPurchaseStatusResponse>());
			ConnectAPI.s_packetDecoders.Add(272, new ConnectAPI.DefaultProtobufPacketDecoder<PurchaseMethod>());
			ConnectAPI.s_packetDecoders.Add(275, new ConnectAPI.DefaultProtobufPacketDecoder<CancelPurchaseResponse>());
			ConnectAPI.s_packetDecoders.Add(256, new ConnectAPI.DefaultProtobufPacketDecoder<PurchaseResponse>());
			ConnectAPI.s_packetDecoders.Add(238, new ConnectAPI.DefaultProtobufPacketDecoder<BattlePayConfigResponse>());
			ConnectAPI.s_packetDecoders.Add(280, new ConnectAPI.DefaultProtobufPacketDecoder<PurchaseWithGoldResponse>());
			ConnectAPI.s_packetDecoders.Add(283, new ConnectAPI.DefaultProtobufPacketDecoder<HeroXP>());
			ConnectAPI.s_packetDecoders.Add(254, new ConnectAPI.NoOpPacketDecoder());
			ConnectAPI.s_packetDecoders.Add(286, new ConnectAPI.DefaultProtobufPacketDecoder<PlayQueue>());
			ConnectAPI.s_packetDecoders.Add(330, new ConnectAPI.DefaultProtobufPacketDecoder<CheckAccountLicensesResponse>());
			ConnectAPI.s_packetDecoders.Add(331, new ConnectAPI.DefaultProtobufPacketDecoder<CheckGameLicensesResponse>());
			ConnectAPI.s_packetDecoders.Add(236, new ConnectAPI.DefaultProtobufPacketDecoder<CardBacks>());
			ConnectAPI.s_packetDecoders.Add(292, new ConnectAPI.DefaultProtobufPacketDecoder<SetCardBackResponse>());
			ConnectAPI.s_packetDecoders.Add(296, new ConnectAPI.DefaultProtobufPacketDecoder<SetProgressResponse>());
			ConnectAPI.s_packetDecoders.Add(299, new ConnectAPI.DefaultProtobufPacketDecoder<TriggerEventResponse>());
			ConnectAPI.s_packetDecoders.Add(300, new ConnectAPI.DefaultProtobufPacketDecoder<NotSoMassiveLoginReply>());
			ConnectAPI.s_packetDecoders.Add(304, new ConnectAPI.DefaultProtobufPacketDecoder<AssetsVersionResponse>());
			ConnectAPI.s_packetDecoders.Add(306, new ConnectAPI.DefaultProtobufPacketDecoder<AdventureProgressResponse>());
			ConnectAPI.s_packetDecoders.Add(307, new ConnectAPI.DefaultProtobufPacketDecoder<UpdateLoginComplete>());
			ConnectAPI.s_packetDecoders.Add(311, new ConnectAPI.DefaultProtobufPacketDecoder<AccountLicenseAchieveResponse>());
			ConnectAPI.s_packetDecoders.Add(315, new ConnectAPI.DefaultProtobufPacketDecoder<SubscribeResponse>());
			ConnectAPI.s_packetDecoders.Add(316, new ConnectAPI.DefaultProtobufPacketDecoder<TavernBrawlInfo>());
			ConnectAPI.s_packetDecoders.Add(317, new ConnectAPI.DefaultProtobufPacketDecoder<TavernBrawlPlayerRecordResponse>());
			ConnectAPI.s_packetDecoders.Add(318, new ConnectAPI.DefaultProtobufPacketDecoder<FavoriteHeroesResponse>());
			ConnectAPI.s_packetDecoders.Add(320, new ConnectAPI.DefaultProtobufPacketDecoder<SetFavoriteHeroResponse>());
			ConnectAPI.s_packetDecoders.Add(324, new ConnectAPI.DefaultProtobufPacketDecoder<DebugCommandResponse>());
			ConnectAPI.s_packetDecoders.Add(325, new ConnectAPI.DefaultProtobufPacketDecoder<AccountLicensesInfoResponse>());
			ConnectAPI.s_packetDecoders.Add(326, new ConnectAPI.DefaultProtobufPacketDecoder<GenericResponse>());
			ConnectAPI.s_packetDecoders.Add(328, new ConnectAPI.DefaultProtobufPacketDecoder<ClientRequestResponse>());
			ConnectAPI.s_packetDecoders.Add(322, new ConnectAPI.DefaultProtobufPacketDecoder<GetAssetResponse>());
			ConnectAPI.s_initialized = true;
		}
		return true;
	}

	// Token: 0x060008AB RID: 2219 RVA: 0x000225C4 File Offset: 0x000207C4
	private static bool ConnectDebugConsole()
	{
		if (ConnectAPI.s_debugListener == null)
		{
			return false;
		}
		if (ConnectAPI.s_debugConsole != null && ConnectAPI.s_debugConsole.Active)
		{
			return true;
		}
		ClientConnection<PegasusPacket> nextAcceptedConnection = ConnectAPI.s_debugListener.GetNextAcceptedConnection();
		if (nextAcceptedConnection == null)
		{
			return false;
		}
		ConnectAPI.s_debugConsole = nextAcceptedConnection;
		nextAcceptedConnection.AddListener(ConnectAPI.s_connectAPI, ConnectAPI.s_debugPackets);
		nextAcceptedConnection.StartReceiving();
		return true;
	}

	// Token: 0x060008AC RID: 2220 RVA: 0x00022628 File Offset: 0x00020828
	private static void WillReset()
	{
		ConnectAPI.s_clientRequestManager.WillReset();
	}

	// Token: 0x060008AD RID: 2221 RVA: 0x00022634 File Offset: 0x00020834
	public static void Heartbeat()
	{
		ConnectAPI.GetBattleNetPackets();
		int count = ConnectAPI.s_errorList.Count;
		for (int i = 0; i < count; i++)
		{
			ConnectAPI.ConnectErrorParams connectErrorParams = ConnectAPI.s_errorList[i];
			if (connectErrorParams == null)
			{
				Debug.LogError("null error! " + ConnectAPI.s_errorList.Count);
			}
			else if (Time.realtimeSinceStartup >= connectErrorParams.m_creationTime + 0.4f)
			{
				ConnectAPI.s_errorList.RemoveAt(i);
				i--;
				count = ConnectAPI.s_errorList.Count;
				Error.AddFatal(connectErrorParams);
			}
		}
		if (ConnectAPI.s_gameServer != null)
		{
			ConnectAPI.s_gameServer.Update();
			ConnectAPI.UpdatePingPong();
		}
		ConnectAPI.s_clientRequestManager.Update();
		if (ConnectAPI.ConnectDebugConsole())
		{
			ConnectAPI.s_debugConsole.Update();
		}
	}

	// Token: 0x060008AE RID: 2222 RVA: 0x00022708 File Offset: 0x00020908
	private static void UpdatePingPong()
	{
		if (ConnectAPI.s_gameServerKeepAliveFrequencySeconds > 0)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (ConnectAPI.s_gameServer.Active && realtimeSinceStartup - ConnectAPI.s_lastPingSent > (float)ConnectAPI.s_gameServerKeepAliveFrequencySeconds)
			{
				if (ConnectAPI.s_lastPingSent <= ConnectAPI.s_lastPingReceived)
				{
					ConnectAPI.s_lastPingReceived = realtimeSinceStartup - 0.001f;
				}
				ConnectAPI.s_lastPingSent = realtimeSinceStartup;
				ConnectAPI.SendPing();
				if (ConnectAPI.s_reconnectAfterFailedPings && ConnectAPI.s_pingsSentSinceLastPong >= 3 && realtimeSinceStartup - ConnectAPI.s_lastPingReceived > 10f)
				{
					ConnectAPI.DisconnectFromGameServer();
				}
				ConnectAPI.s_pingsSentSinceLastPong++;
			}
		}
	}

	// Token: 0x060008AF RID: 2223 RVA: 0x000227AC File Offset: 0x000209AC
	private static void DecodeAndProcessPacket(PegasusPacket input)
	{
		ConnectAPI.PacketDecoder packetDecoder;
		if (ConnectAPI.s_packetDecoders.TryGetValue(input.Type, ref packetDecoder))
		{
			PegasusPacket pegasusPacket = packetDecoder.HandlePacket(input);
			if (pegasusPacket != null)
			{
				ConnectAPI.s_clientRequestManager.NotifyResponseReceived(pegasusPacket);
			}
		}
	}

	// Token: 0x060008B0 RID: 2224 RVA: 0x000227EC File Offset: 0x000209EC
	private static void GetBattleNetPackets()
	{
		GamesAPI.UtilResponse utilResponse;
		while ((utilResponse = BattleNet.NextUtilPacket()) != null)
		{
			Attribute attribute = utilResponse.m_response.AttributeList[0];
			Attribute attribute2 = utilResponse.m_response.AttributeList[1];
			int type = (int)attribute.Value.IntValue;
			byte[] blobValue = attribute2.Value.BlobValue;
			ConnectAPI.DecodeAndProcessPacket(new PegasusPacket(type, blobValue.Length, blobValue)
			{
				Context = utilResponse.m_context
			});
		}
	}

	// Token: 0x060008B1 RID: 2225 RVA: 0x0002286A File Offset: 0x00020A6A
	public static void QueueUtilNotificationPegasusPacket(PegasusPacket pegasusPacket)
	{
		ConnectAPI.DecodeAndProcessPacket(pegasusPacket);
	}

	// Token: 0x060008B2 RID: 2226 RVA: 0x00022872 File Offset: 0x00020A72
	public void PacketReceived(PegasusPacket p, object state)
	{
		ConnectAPI.QueuePacketReceived(p, (Queue<PegasusPacket>)state);
	}

	// Token: 0x060008B3 RID: 2227 RVA: 0x00022880 File Offset: 0x00020A80
	private static void QueuePacketReceived(PegasusPacket packet, Queue<PegasusPacket> queue)
	{
		if (queue == ConnectAPI.s_gamePackets && packet.Type == 16)
		{
			ConnectAPI.s_gameStartState = ConnectAPI.GameStartState.INVALID;
		}
		if (queue == ConnectAPI.s_gamePackets && packet.Type == 116)
		{
			ConnectAPI.s_lastPingReceived = Time.realtimeSinceStartup;
			ConnectAPI.s_pingsSentSinceLastPong = 0;
		}
		ConnectAPI.PacketDecoder packetDecoder;
		if (ConnectAPI.s_packetDecoders.TryGetValue(packet.Type, ref packetDecoder))
		{
			PegasusPacket pegasusPacket = packetDecoder.HandlePacket(packet);
			if (pegasusPacket != null)
			{
				queue.Enqueue(pegasusPacket);
			}
		}
		else
		{
			Debug.LogError("Could not find a packet decoder for a packet of type " + packet.Type);
		}
	}

	// Token: 0x060008B4 RID: 2228 RVA: 0x0002291E File Offset: 0x00020B1E
	public static void OnStartupPacketSequenceComplete()
	{
		ConnectAPI.s_clientRequestManager.NotifyStartupSequenceComplete();
	}

	// Token: 0x060008B5 RID: 2229 RVA: 0x0002292C File Offset: 0x00020B2C
	private void OnGameServerConnectCallback(BattleNetErrors error)
	{
		Log.GameMgr.Print("Connecting to game server with error code " + error, new object[0]);
		if (error != null)
		{
			ConnectAPI.GameStartState gameStartState = ConnectAPI.s_gameStartState;
			ConnectAPI.s_gameStartState = ConnectAPI.GameStartState.INVALID;
			if (Network.ShouldBeConnectedToAurora())
			{
				GameServerInfo lastGameServerJoined = Network.Get().GetLastGameServerJoined();
				if (lastGameServerJoined != null && gameStartState == ConnectAPI.GameStartState.RECONNECTING)
				{
					return;
				}
				Network.Get().ShowBreakingNewsOrError("GLOBAL_ERROR_NETWORK_NO_GAME_SERVER", 0f);
				Debug.LogError("Failed to connect to game server with error " + error);
			}
			else
			{
				Network.Get().ShowBreakingNewsOrError("GLOBAL_ERROR_NETWORK_NO_GAME_SERVER", 0f);
				Debug.LogError("Failed to connect to game server with error " + error);
			}
		}
	}

	// Token: 0x060008B6 RID: 2230 RVA: 0x000229E8 File Offset: 0x00020BE8
	private void OnGameServerDisconnectCallback(BattleNetErrors error)
	{
		Log.GameMgr.Print("Disconnected from game server with error {0} {1}", new object[]
		{
			error,
			error.ToString()
		});
		bool flag = false;
		if (error != null)
		{
			if (ConnectAPI.s_gameStartState == ConnectAPI.GameStartState.RECONNECTING)
			{
				flag = true;
			}
			else if (ConnectAPI.s_gameStartState == ConnectAPI.GameStartState.INITIAL_START)
			{
				GameServerInfo lastGameServerJoined = Network.Get().GetLastGameServerJoined();
				if (lastGameServerJoined == null || !lastGameServerJoined.SpectatorMode)
				{
					ConnectAPI.ConnectErrorParams connectErrorParams = new ConnectAPI.ConnectErrorParams();
					connectErrorParams.m_message = GameStrings.Format((error != 3007) ? "GLOBAL_ERROR_NETWORK_DISCONNECT_GAME_SERVER" : "GLOBAL_ERROR_NETWORK_CONNECTION_TIMEOUT", new object[0]);
					ConnectAPI.s_errorList.Add(connectErrorParams);
					Debug.LogError("Disconnected from game server with error " + error);
					flag = true;
				}
			}
			ConnectAPI.s_gameStartState = ConnectAPI.GameStartState.INVALID;
		}
		if (!flag)
		{
			if (ConnectAPI.GameServerDisconnectEvents == null)
			{
				ConnectAPI.GameServerDisconnectEvents = new List<BattleNetErrors>();
			}
			ConnectAPI.GameServerDisconnectEvents.Add(error);
		}
	}

	// Token: 0x060008B7 RID: 2231 RVA: 0x00022AE8 File Offset: 0x00020CE8
	private static bool CheckType(PegasusPacket p, int packetId)
	{
		bool flag = p == null || p.Type != packetId;
		if (flag)
		{
			Debug.LogError("ERROR: invalid type " + p);
		}
		return !flag;
	}

	// Token: 0x060008B8 RID: 2232 RVA: 0x00022B28 File Offset: 0x00020D28
	private static T Unpack<T>(PegasusPacket p)
	{
		if (p == null || !(p.Body is T))
		{
			return default(T);
		}
		return (T)((object)p.Body);
	}

	// Token: 0x060008B9 RID: 2233 RVA: 0x00022B60 File Offset: 0x00020D60
	private static T Unpack<T>(PegasusPacket p, int packetId)
	{
		if (p == null || p.Type != packetId || !(p.Body is T))
		{
			return default(T);
		}
		return (T)((object)p.Body);
	}

	// Token: 0x060008BA RID: 2234 RVA: 0x00022BA4 File Offset: 0x00020DA4
	private static void UtilOutbound(int type, int system, IProtoBuf body, ClientRequestManager.RequestPhase requestPhase = ClientRequestManager.RequestPhase.RUNNING, int subID = 0)
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			ConnectAPI.FakeUtilOutbound(type, system, body, subID);
			return;
		}
		ClientRequestManager.ClientRequestConfig clientRequestConfig = null;
		if (system == 1)
		{
			clientRequestConfig = new ClientRequestManager.ClientRequestConfig();
			clientRequestConfig.ShouldRetryOnError = false;
			clientRequestConfig.RequestedSystem = 1;
		}
		if (ConnectAPI.s_clientRequestManager.SendClientRequest(type, body, clientRequestConfig, requestPhase, subID))
		{
			if (type == 201)
			{
				GetAccountInfo getAccountInfo = (GetAccountInfo)body;
				Network.AddPendingRequestTimeout(type, getAccountInfo.Request_);
			}
			else
			{
				Network.AddPendingRequestTimeout(type, 0);
			}
		}
	}

	// Token: 0x060008BB RID: 2235 RVA: 0x00022C24 File Offset: 0x00020E24
	private static void FakeUtilOutboundGetAccountInfo(GetAccountInfo.Request request)
	{
		Enum @enum = null;
		switch (request)
		{
		case 2:
			@enum = 202;
			break;
		case 3:
			@enum = 207;
			break;
		case 4:
			@enum = 232;
			break;
		case 6:
			@enum = 224;
			break;
		case 7:
			@enum = 236;
			break;
		case 8:
			@enum = 270;
			break;
		case 10:
			@enum = 231;
			break;
		case 11:
			@enum = 233;
			break;
		case 12:
			@enum = 212;
			break;
		case 14:
			@enum = 241;
			break;
		case 15:
			@enum = 260;
			break;
		case 16:
			@enum = 289;
			break;
		case 17:
			@enum = 262;
			break;
		case 18:
			@enum = 264;
			break;
		case 19:
			@enum = 271;
			break;
		case 20:
			@enum = 278;
			break;
		case 21:
			@enum = 283;
			break;
		case 23:
			@enum = 300;
			break;
		case 25:
			@enum = 316;
			break;
		case 26:
			@enum = 317;
			break;
		case 27:
			@enum = 318;
			break;
		case 28:
			@enum = 325;
			break;
		}
		if (@enum == null)
		{
			Debug.LogError(string.Format("Fake response for request {0} of packet PegasusUtil.GetAccountInfo not handled", request));
		}
		else
		{
			Network.Get().FakeHandleType(@enum);
		}
	}

	// Token: 0x060008BC RID: 2236 RVA: 0x00022E3C File Offset: 0x0002103C
	private static void FakeUtilOutbound(int type, int system, IProtoBuf body, int subID)
	{
		if (type == 240)
		{
			Network.Get().FakeHandleType(241);
		}
		else if (type == 253)
		{
			Network.Get().FakeHandleType(252);
		}
		else if (type == 201)
		{
			ConnectAPI.FakeUtilOutboundGetAccountInfo(subID);
		}
		else if (type == 327)
		{
			GenericRequestList genericRequestList = (GenericRequestList)body;
			foreach (GenericRequest genericRequest in genericRequestList.Requests)
			{
				int requestId = genericRequest.RequestId;
				if (requestId != 201)
				{
					if (requestId != 240)
					{
						if (requestId == 253)
						{
							Network.Get().FakeHandleType(252);
						}
					}
					else
					{
						Network.Get().FakeHandleType(241);
					}
				}
				else
				{
					ConnectAPI.FakeUtilOutboundGetAccountInfo(genericRequest.RequestSubId);
				}
			}
		}
		else if (type != 305 && type != 267 && type != 276 && type != 239 && type != 284)
		{
			Debug.LogError(string.Format("Unhandled fake response for request {0} of packet ", type));
		}
	}

	// Token: 0x060008BD RID: 2237 RVA: 0x00022FD0 File Offset: 0x000211D0
	public static bool HaveUtilPackets()
	{
		return ConnectAPI.s_clientRequestManager.HasPendingDeliveryPackets();
	}

	// Token: 0x060008BE RID: 2238 RVA: 0x00022FDC File Offset: 0x000211DC
	public static bool HaveGamePackets()
	{
		return ConnectAPI.s_gamePackets.Count > 0;
	}

	// Token: 0x060008BF RID: 2239 RVA: 0x00022FEB File Offset: 0x000211EB
	public static bool HaveDebugPackets()
	{
		return ConnectAPI.s_debugPackets.Count > 0;
	}

	// Token: 0x060008C0 RID: 2240 RVA: 0x00022FFA File Offset: 0x000211FA
	public static bool HaveNotificationPackets()
	{
		return BattleNet.GetNotificationCount() > 0;
	}

	// Token: 0x060008C1 RID: 2241 RVA: 0x00023004 File Offset: 0x00021204
	public static Queue<PegasusPacket> GetGamePackets()
	{
		return ConnectAPI.s_gamePackets;
	}

	// Token: 0x060008C2 RID: 2242 RVA: 0x0002300B File Offset: 0x0002120B
	public static ClientConnection<PegasusPacket> GetGameServerConnection()
	{
		return ConnectAPI.s_gameServer;
	}

	// Token: 0x060008C3 RID: 2243 RVA: 0x00023012 File Offset: 0x00021212
	public static bool IsConnectedToGameServer()
	{
		return ConnectAPI.s_gameServer != null && ConnectAPI.s_gameServer.Active;
	}

	// Token: 0x060008C4 RID: 2244 RVA: 0x0002302B File Offset: 0x0002122B
	public static bool WasGameConceded()
	{
		return ConnectAPI.s_gameConceded;
	}

	// Token: 0x060008C5 RID: 2245 RVA: 0x00023032 File Offset: 0x00021232
	public static bool WasDisconnectRequested()
	{
		return ConnectAPI.s_disconnectRequested;
	}

	// Token: 0x060008C6 RID: 2246 RVA: 0x00023039 File Offset: 0x00021239
	private static int NextType(Queue<PegasusPacket> packets)
	{
		if (packets == null || packets.Count == 0)
		{
			return 0;
		}
		return packets.Peek().Type;
	}

	// Token: 0x060008C7 RID: 2247 RVA: 0x00023059 File Offset: 0x00021259
	public static int NextGameType()
	{
		return ConnectAPI.NextType(ConnectAPI.s_gamePackets);
	}

	// Token: 0x060008C8 RID: 2248 RVA: 0x00023065 File Offset: 0x00021265
	public static int NextUtilType()
	{
		return ConnectAPI.s_clientRequestManager.PeekNetClientRequestType();
	}

	// Token: 0x060008C9 RID: 2249 RVA: 0x00023071 File Offset: 0x00021271
	public static int NextDebugConsoleType()
	{
		return ConnectAPI.NextType(ConnectAPI.s_debugPackets);
	}

	// Token: 0x060008CA RID: 2250 RVA: 0x00023080 File Offset: 0x00021280
	public static PegasusPacket Next(Queue<PegasusPacket> packets, bool pop)
	{
		if (packets == null || packets.Count == 0)
		{
			return null;
		}
		if (pop)
		{
			return packets.Dequeue();
		}
		return packets.Peek();
	}

	// Token: 0x060008CB RID: 2251 RVA: 0x000230B3 File Offset: 0x000212B3
	public static PegasusPacket NextGame()
	{
		return ConnectAPI.NextGame(false);
	}

	// Token: 0x060008CC RID: 2252 RVA: 0x000230BB File Offset: 0x000212BB
	public static PegasusPacket NextGame(bool pop)
	{
		return ConnectAPI.Next(ConnectAPI.s_gamePackets, pop);
	}

	// Token: 0x060008CD RID: 2253 RVA: 0x000230C8 File Offset: 0x000212C8
	public static PegasusPacket NextUtil()
	{
		return ConnectAPI.s_clientRequestManager.GetNextClientRequest();
	}

	// Token: 0x060008CE RID: 2254 RVA: 0x000230D4 File Offset: 0x000212D4
	public static PegasusPacket NextDebug()
	{
		return ConnectAPI.NextDebug(false);
	}

	// Token: 0x060008CF RID: 2255 RVA: 0x000230DC File Offset: 0x000212DC
	public static PegasusPacket NextDebug(bool pop)
	{
		return ConnectAPI.Next(ConnectAPI.s_debugPackets, pop);
	}

	// Token: 0x060008D0 RID: 2256 RVA: 0x000230E9 File Offset: 0x000212E9
	public static void QueueGamePacket(int packetId, IProtoBuf body)
	{
		ConnectAPI.s_gameServer.QueuePacket(new PegasusPacket(packetId, 0, body));
	}

	// Token: 0x060008D1 RID: 2257 RVA: 0x000230FD File Offset: 0x000212FD
	private static void QueueDebugPacket(int packetId, IProtoBuf body)
	{
		ConnectAPI.s_debugConsole.QueuePacket(new PegasusPacket(packetId, 0, body));
	}

	// Token: 0x060008D2 RID: 2258 RVA: 0x00023114 File Offset: 0x00021314
	private static void DropPacket(ConnectAPI.ServerType type)
	{
		Queue<PegasusPacket> queue = null;
		switch (type)
		{
		case ConnectAPI.ServerType.GAME_SERVER:
			queue = ConnectAPI.s_gamePackets;
			break;
		case ConnectAPI.ServerType.UTIL_SERVER:
			ConnectAPI.s_clientRequestManager.DropNextClientRequest();
			break;
		case ConnectAPI.ServerType.DEBUG_CONSOLE:
			queue = ConnectAPI.s_debugPackets;
			break;
		}
		if (queue != null && queue.Count > 0)
		{
			queue.Dequeue();
		}
	}

	// Token: 0x060008D3 RID: 2259 RVA: 0x0002317C File Offset: 0x0002137C
	private static void DropAllPackets(ConnectAPI.ServerType type)
	{
		Queue<PegasusPacket> queue = null;
		switch (type)
		{
		case ConnectAPI.ServerType.GAME_SERVER:
			queue = ConnectAPI.s_gamePackets;
			break;
		case ConnectAPI.ServerType.DEBUG_CONSOLE:
			queue = ConnectAPI.s_debugPackets;
			break;
		}
		if (queue != null)
		{
			Log.LoadingScreen.Print("ConnectAPI.DropAllPackets() - {0} dropped {1} packets", new object[]
			{
				type,
				queue.Count
			});
			queue.Clear();
		}
	}

	// Token: 0x060008D4 RID: 2260 RVA: 0x000231F2 File Offset: 0x000213F2
	public static void DropUtilPacket()
	{
		ConnectAPI.DropPacket(ConnectAPI.ServerType.UTIL_SERVER);
	}

	// Token: 0x060008D5 RID: 2261 RVA: 0x000231FA File Offset: 0x000213FA
	public static void DropGamePacket()
	{
		ConnectAPI.DropPacket(ConnectAPI.ServerType.GAME_SERVER);
	}

	// Token: 0x060008D6 RID: 2262 RVA: 0x00023202 File Offset: 0x00021402
	public static void DropDebugPacket()
	{
		ConnectAPI.DropPacket(ConnectAPI.ServerType.DEBUG_CONSOLE);
	}

	// Token: 0x060008D7 RID: 2263 RVA: 0x0002320A File Offset: 0x0002140A
	public static void DropAllGamePackets()
	{
		ConnectAPI.DropAllPackets(ConnectAPI.ServerType.GAME_SERVER);
	}

	// Token: 0x060008D8 RID: 2264 RVA: 0x00023212 File Offset: 0x00021412
	public static bool ConnectsWithAurora()
	{
		return true;
	}

	// Token: 0x060008D9 RID: 2265 RVA: 0x00023215 File Offset: 0x00021415
	public static bool ConnectsWithBobnet()
	{
		return false;
	}

	// Token: 0x060008DA RID: 2266 RVA: 0x00023218 File Offset: 0x00021418
	public static void CloseAll()
	{
		if (ConnectAPI.m_ackCardSeenPacket.CardDefs.Count != 0)
		{
			ConnectAPI.SendAckCardsSeen();
		}
		ConnectAPI.s_clientRequestManager.Terminate();
		ConnectAPI.s_clientRequestManager.Update();
		if (ConnectAPI.s_gameServer != null)
		{
			ConnectAPI.s_gameServer.Update();
			int num = 0;
			while (ConnectAPI.s_gameServer.HasOutPacketsInFlight())
			{
				num += 10;
				Thread.Sleep(num);
				if (num > 1000)
				{
					break;
				}
			}
			ConnectAPI.s_gameServer.Disconnect();
		}
		if (ConnectAPI.s_debugConsole != null)
		{
			ConnectAPI.s_debugConsole.Disconnect();
		}
		if (ConnectAPI.s_debugListener != null)
		{
			ConnectAPI.s_debugListener.Disconnect();
		}
	}

	// Token: 0x060008DB RID: 2267 RVA: 0x000232C9 File Offset: 0x000214C9
	public static void ApplicationPaused()
	{
		if (ConnectAPI.m_ackCardSeenPacket.CardDefs.Count != 0)
		{
			ConnectAPI.SendAckCardsSeen();
		}
	}

	// Token: 0x060008DC RID: 2268 RVA: 0x000232E4 File Offset: 0x000214E4
	public static void ResetSubscription()
	{
		ConnectAPI.s_clientRequestManager.WillReset();
	}

	// Token: 0x060008DD RID: 2269 RVA: 0x000232F0 File Offset: 0x000214F0
	public static bool ShouldIgnoreError(BnetErrorInfo errorInfo)
	{
		return ConnectAPI.s_clientRequestManager.ShouldIgnoreError(errorInfo);
	}

	// Token: 0x060008DE RID: 2270 RVA: 0x000232FD File Offset: 0x000214FD
	public static bool HasErrors()
	{
		return ConnectAPI.s_clientRequestManager.HasErrors();
	}

	// Token: 0x060008DF RID: 2271 RVA: 0x00023309 File Offset: 0x00021509
	public static void RegisterThrottledPacketListener(ConnectAPI.ThrottledPacketListener listener)
	{
		if (ConnectAPI.m_throttledPacketListeners.Contains(listener))
		{
			return;
		}
		ConnectAPI.m_throttledPacketListeners.Add(listener);
	}

	// Token: 0x060008E0 RID: 2272 RVA: 0x00023327 File Offset: 0x00021527
	public static void RemoveThrottledPacketListener(ConnectAPI.ThrottledPacketListener listener)
	{
		ConnectAPI.m_throttledPacketListeners.Remove(listener);
	}

	// Token: 0x060008E1 RID: 2273 RVA: 0x00023335 File Offset: 0x00021535
	public static void OnLoginStarted()
	{
	}

	// Token: 0x060008E2 RID: 2274 RVA: 0x00023338 File Offset: 0x00021538
	public static Network.GameCancelInfo GetGameCancelInfo()
	{
		GameCanceled gameCanceled = ConnectAPI.Unpack<GameCanceled>(ConnectAPI.NextGame(), 12);
		if (gameCanceled == null)
		{
			return null;
		}
		return new Network.GameCancelInfo
		{
			CancelReason = gameCanceled.Reason_
		};
	}

	// Token: 0x060008E3 RID: 2275 RVA: 0x00023370 File Offset: 0x00021570
	public static SubscribeResponse GetSubscribeResponse()
	{
		SubscribeResponse subscribeResponse = ConnectAPI.Unpack<SubscribeResponse>(ConnectAPI.NextUtil(), 315);
		if (subscribeResponse == null)
		{
			return null;
		}
		return subscribeResponse;
	}

	// Token: 0x060008E4 RID: 2276 RVA: 0x00023398 File Offset: 0x00021598
	public static List<Network.PowerHistory> GetPowerHistory()
	{
		PowerHistory powerHistory = ConnectAPI.Unpack<PowerHistory>(ConnectAPI.NextGame(), 19);
		if (powerHistory == null)
		{
			return null;
		}
		List<Network.PowerHistory> list = new List<Network.PowerHistory>();
		foreach (PowerHistoryData powerHistoryData in powerHistory.List)
		{
			Network.PowerHistory powerHistory2 = null;
			if (powerHistoryData.HasFullEntity)
			{
				powerHistory2 = ConnectAPI.GetFullEntity(powerHistoryData.FullEntity);
			}
			else if (powerHistoryData.HasShowEntity)
			{
				powerHistory2 = ConnectAPI.GetShowEntity(powerHistoryData.ShowEntity);
			}
			else if (powerHistoryData.HasHideEntity)
			{
				powerHistory2 = ConnectAPI.GetHideEntity(powerHistoryData.HideEntity);
			}
			else if (powerHistoryData.HasChangeEntity)
			{
				powerHistory2 = ConnectAPI.GetChangeEntity(powerHistoryData.ChangeEntity);
			}
			else if (powerHistoryData.HasTagChange)
			{
				powerHistory2 = ConnectAPI.GetTagChange(powerHistoryData.TagChange);
			}
			else if (powerHistoryData.HasPowerStart)
			{
				powerHistory2 = ConnectAPI.GetBlockStart(powerHistoryData.PowerStart);
			}
			else if (powerHistoryData.HasPowerEnd)
			{
				powerHistory2 = ConnectAPI.GetBlockEnd(powerHistoryData.PowerEnd);
			}
			else if (powerHistoryData.HasCreateGame)
			{
				powerHistory2 = ConnectAPI.GetCreateGame(powerHistoryData.CreateGame);
			}
			else if (powerHistoryData.HasMetaData)
			{
				powerHistory2 = ConnectAPI.GetMetaData(powerHistoryData.MetaData);
			}
			else
			{
				Debug.LogError("Network.GetPowerHistory() - received invalid PowerHistoryData packet");
			}
			if (powerHistory2 != null)
			{
				list.Add(powerHistory2);
			}
		}
		return list;
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x00023534 File Offset: 0x00021734
	private static Network.HistFullEntity GetFullEntity(PowerHistoryEntity entity)
	{
		return new Network.HistFullEntity
		{
			Entity = Network.Entity.CreateFromProto(entity)
		};
	}

	// Token: 0x060008E6 RID: 2278 RVA: 0x00023554 File Offset: 0x00021754
	private static Network.HistShowEntity GetShowEntity(PowerHistoryEntity entity)
	{
		return new Network.HistShowEntity
		{
			Entity = Network.Entity.CreateFromProto(entity)
		};
	}

	// Token: 0x060008E7 RID: 2279 RVA: 0x00023574 File Offset: 0x00021774
	private static Network.HistHideEntity GetHideEntity(PowerHistoryHide hide)
	{
		return new Network.HistHideEntity
		{
			Entity = hide.Entity,
			Zone = hide.Zone
		};
	}

	// Token: 0x060008E8 RID: 2280 RVA: 0x000235A0 File Offset: 0x000217A0
	private static Network.HistChangeEntity GetChangeEntity(PowerHistoryEntity entity)
	{
		return new Network.HistChangeEntity
		{
			Entity = Network.Entity.CreateFromProto(entity)
		};
	}

	// Token: 0x060008E9 RID: 2281 RVA: 0x000235C0 File Offset: 0x000217C0
	private static Network.HistTagChange GetTagChange(PowerHistoryTagChange tagChange)
	{
		return new Network.HistTagChange
		{
			Entity = tagChange.Entity,
			Tag = tagChange.Tag,
			Value = tagChange.Value
		};
	}

	// Token: 0x060008EA RID: 2282 RVA: 0x000235F8 File Offset: 0x000217F8
	private static Network.HistBlockStart GetBlockStart(PowerHistoryStart start)
	{
		return new Network.HistBlockStart(start.Type)
		{
			Entity = start.Source,
			Target = start.Target,
			EffectCardId = start.EffectCardId,
			EffectIndex = start.Index
		};
	}

	// Token: 0x060008EB RID: 2283 RVA: 0x00023642 File Offset: 0x00021842
	private static Network.HistBlockEnd GetBlockEnd(PowerHistoryEnd end)
	{
		return new Network.HistBlockEnd();
	}

	// Token: 0x060008EC RID: 2284 RVA: 0x0002364C File Offset: 0x0002184C
	private static Network.HistCreateGame GetCreateGame(PowerHistoryCreateGame createGame)
	{
		return Network.HistCreateGame.CreateFromProto(createGame);
	}

	// Token: 0x060008ED RID: 2285 RVA: 0x00023664 File Offset: 0x00021864
	private static Network.HistMetaData GetMetaData(PowerHistoryMetaData metaData)
	{
		Network.HistMetaData histMetaData = new Network.HistMetaData();
		histMetaData.MetaType = ((!metaData.HasType) ? 0 : metaData.Type);
		histMetaData.Data = ((!metaData.HasData) ? 0 : metaData.Data);
		foreach (int num in metaData.Info)
		{
			histMetaData.Info.Add(num);
		}
		return histMetaData;
	}

	// Token: 0x060008EE RID: 2286 RVA: 0x00023704 File Offset: 0x00021904
	public static Network.GameSetup GetGameSetup()
	{
		GameSetup gameSetup = ConnectAPI.Unpack<GameSetup>(ConnectAPI.NextGame(), 16);
		if (gameSetup == null)
		{
			return null;
		}
		Network.GameSetup gameSetup2 = new Network.GameSetup();
		gameSetup2.Board = gameSetup.Board;
		gameSetup2.MaxSecretsPerPlayer = gameSetup.MaxSecretsPerPlayer;
		gameSetup2.MaxFriendlyMinionsPerPlayer = gameSetup.MaxFriendlyMinionsPerPlayer;
		if (gameSetup.HasKeepAliveFrequencySeconds)
		{
			ConnectAPI.s_gameServerKeepAliveFrequencySeconds = gameSetup.KeepAliveFrequencySeconds;
		}
		else
		{
			ConnectAPI.s_gameServerKeepAliveFrequencySeconds = 0;
		}
		if (gameSetup.HasDisconnectWhenStuckSeconds)
		{
			gameSetup2.DisconnectWhenStuckSeconds = gameSetup.DisconnectWhenStuckSeconds;
		}
		return gameSetup2;
	}

	// Token: 0x060008EF RID: 2287 RVA: 0x0002378C File Offset: 0x0002198C
	private static List<int> CopyIntList(IList<int> intList)
	{
		int[] array = new int[intList.Count];
		intList.CopyTo(array, 0);
		return new List<int>(array);
	}

	// Token: 0x060008F0 RID: 2288 RVA: 0x000237B4 File Offset: 0x000219B4
	public static Network.Options GetOptions()
	{
		AllOptions allOptions = ConnectAPI.Unpack<AllOptions>(ConnectAPI.NextGame(), 14);
		Network.Options options = new Network.Options();
		options.ID = allOptions.Id;
		foreach (Option option in allOptions.Options)
		{
			Network.Options.Option option2 = new Network.Options.Option();
			option2.Type = option.Type_;
			if (option.HasMainOption)
			{
				option2.Main.ID = option.MainOption.Id;
				option2.Main.Targets = ConnectAPI.CopyIntList(option.MainOption.Targets);
			}
			foreach (SubOption subOption in option.SubOptions)
			{
				Network.Options.Option.SubOption subOption2 = new Network.Options.Option.SubOption();
				subOption2.ID = subOption.Id;
				subOption2.Targets = ConnectAPI.CopyIntList(subOption.Targets);
				option2.Subs.Add(subOption2);
			}
			options.List.Add(option2);
		}
		return options;
	}

	// Token: 0x060008F1 RID: 2289 RVA: 0x00023904 File Offset: 0x00021B04
	public static Network.EntityChoices GetEntityChoices()
	{
		EntityChoices entityChoices = ConnectAPI.Unpack<EntityChoices>(ConnectAPI.NextGame(), 17);
		if (entityChoices == null)
		{
			return null;
		}
		return new Network.EntityChoices
		{
			ID = entityChoices.Id,
			ChoiceType = (CHOICE_TYPE)entityChoices.ChoiceType,
			CountMax = entityChoices.CountMax,
			CountMin = entityChoices.CountMin,
			Entities = ConnectAPI.CopyIntList(entityChoices.Entities),
			Source = entityChoices.Source,
			PlayerId = entityChoices.PlayerId
		};
	}

	// Token: 0x060008F2 RID: 2290 RVA: 0x00023988 File Offset: 0x00021B88
	public static Network.EntitiesChosen GetEntitiesChosen()
	{
		EntitiesChosen entitiesChosen = ConnectAPI.Unpack<EntitiesChosen>(ConnectAPI.NextGame(), 13);
		if (entitiesChosen == null)
		{
			return null;
		}
		return new Network.EntitiesChosen
		{
			ID = entitiesChosen.ChooseEntities.Id,
			Entities = ConnectAPI.CopyIntList(entitiesChosen.ChooseEntities.Entities),
			PlayerId = entitiesChosen.PlayerId
		};
	}

	// Token: 0x060008F3 RID: 2291 RVA: 0x000239E4 File Offset: 0x00021BE4
	public static Network.UserUI GetUserUI()
	{
		UserUI userUI = ConnectAPI.Unpack<UserUI>(ConnectAPI.NextGame(), 15);
		if (userUI == null)
		{
			return null;
		}
		Network.UserUI userUI2 = new Network.UserUI();
		if (userUI.HasPlayerId)
		{
			userUI2.playerId = new int?(userUI.PlayerId);
		}
		if (userUI.HasMouseInfo)
		{
			MouseInfo mouseInfo = userUI.MouseInfo;
			userUI2.mouseInfo = new Network.UserUI.MouseInfo();
			userUI2.mouseInfo.ArrowOriginID = mouseInfo.ArrowOrigin;
			userUI2.mouseInfo.HeldCardID = mouseInfo.HeldCard;
			userUI2.mouseInfo.OverCardID = mouseInfo.OverCard;
			userUI2.mouseInfo.X = mouseInfo.X;
			userUI2.mouseInfo.Y = mouseInfo.Y;
		}
		else if (userUI.HasEmote)
		{
			userUI2.emoteInfo = new Network.UserUI.EmoteInfo();
			userUI2.emoteInfo.Emote = userUI.Emote;
		}
		return userUI2;
	}

	// Token: 0x060008F4 RID: 2292 RVA: 0x00023AC8 File Offset: 0x00021CC8
	public static Network.TurnTimerInfo GetTurnTimerInfo()
	{
		TurnTimer turnTimer = ConnectAPI.Unpack<TurnTimer>(ConnectAPI.NextGame(), 9);
		if (turnTimer == null)
		{
			return null;
		}
		return new Network.TurnTimerInfo
		{
			Seconds = (float)turnTimer.Seconds,
			Turn = turnTimer.Turn,
			Show = turnTimer.Show
		};
	}

	// Token: 0x060008F5 RID: 2293 RVA: 0x00023B18 File Offset: 0x00021D18
	public static int GetNAckOption()
	{
		NAckOption nackOption = ConnectAPI.Unpack<NAckOption>(ConnectAPI.NextGame(), 10);
		if (nackOption == null)
		{
			return 0;
		}
		return nackOption.Id;
	}

	// Token: 0x060008F6 RID: 2294 RVA: 0x00023B40 File Offset: 0x00021D40
	public static ServerResult GetServerResult()
	{
		return ConnectAPI.Unpack<ServerResult>(ConnectAPI.NextGame(), 23);
	}

	// Token: 0x060008F7 RID: 2295 RVA: 0x00023B5C File Offset: 0x00021D5C
	public static SpectatorNotify GetSpectatorNotify()
	{
		return ConnectAPI.Unpack<SpectatorNotify>(ConnectAPI.NextGame(), 24);
	}

	// Token: 0x060008F8 RID: 2296 RVA: 0x00023B78 File Offset: 0x00021D78
	public static float TimeSinceLastPong()
	{
		if (!ConnectAPI.IsConnectedToGameServer() || ConnectAPI.s_gameServerKeepAliveFrequencySeconds <= 0 || ConnectAPI.s_lastPingSent <= 0f || ConnectAPI.s_lastPingSent <= ConnectAPI.s_lastPingReceived)
		{
			return 0f;
		}
		return Time.realtimeSinceStartup - ConnectAPI.s_lastPingReceived;
	}

	// Token: 0x060008F9 RID: 2297 RVA: 0x00023BCC File Offset: 0x00021DCC
	private static Platform GetPlatformBuilder()
	{
		Platform platform = new Platform();
		platform.Os = (int)PlatformSettings.OS;
		platform.Screen = (int)PlatformSettings.Screen;
		platform.Name = PlatformSettings.DeviceName;
		AndroidStore androidStore = ApplicationMgr.GetAndroidStore();
		if (androidStore != AndroidStore.NONE)
		{
			platform.Store = (int)androidStore;
		}
		return platform;
	}

	// Token: 0x060008FA RID: 2298 RVA: 0x00023C14 File Offset: 0x00021E14
	public static void GotoGameServer(GameServerInfo info, bool reconnecting)
	{
		if (ConnectAPI.s_gameStartState != ConnectAPI.GameStartState.INVALID)
		{
			string message = "GotoGameServer() was called when we're already waiting for a game to start.";
			Error.AddDevFatal(message, new object[0]);
			return;
		}
		string address = info.Address;
		int @int = Vars.Key("Application.GameServerPortOverride").GetInt(info.Port);
		Log.Net.Print(string.Concat(new object[]
		{
			"ConnectAPI.GotoGameServer -- address= ",
			address,
			":",
			@int,
			", game=",
			info.GameHandle,
			", client=",
			info.ClientHandle,
			", spectateKey=",
			info.SpectatorPassword,
			" reconncting=",
			reconnecting
		}), new object[0]);
		if (address == null)
		{
			return;
		}
		if (string.IsNullOrEmpty(address) || @int == 0 || info.GameHandle == 0)
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				"ConnectAPI.GotoGameServer: ERROR in ServerInfo address= ",
				address,
				":",
				@int,
				", game=",
				info.GameHandle,
				", client=",
				info.ClientHandle,
				" reconncting=",
				reconnecting
			}));
		}
		ConnectAPI.s_gameConceded = false;
		ConnectAPI.s_disconnectRequested = false;
		ConnectAPI.s_gameServerKeepAliveFrequencySeconds = 0;
		ConnectAPI.s_lastPingSent = 0f;
		ConnectAPI.s_lastPingReceived = 0f;
		ConnectAPI.s_pingsSentSinceLastPong = 0;
		if (ConnectAPI.GameServerDisconnectEvents != null)
		{
			ConnectAPI.GameServerDisconnectEvents.Clear();
		}
		ConnectAPI.s_gameServer.Connect(address, @int);
		if (!ConnectAPI.s_gameServer.Active)
		{
			return;
		}
		ConnectAPI.SendGameServerHandshake(info);
		ConnectAPI.s_gameStartState = ((!reconnecting) ? ConnectAPI.GameStartState.INITIAL_START : ConnectAPI.GameStartState.RECONNECTING);
	}

	// Token: 0x060008FB RID: 2299 RVA: 0x00023DE8 File Offset: 0x00021FE8
	public static bool RetryGotoGameServer(GameServerInfo info)
	{
		if (ConnectAPI.s_gameStartState == ConnectAPI.GameStartState.INVALID)
		{
			return false;
		}
		ConnectAPI.SendGameServerHandshake(info);
		return true;
	}

	// Token: 0x060008FC RID: 2300 RVA: 0x00023E00 File Offset: 0x00022000
	public static void SpectateSecondPlayer(GameServerInfo info)
	{
		info.SpectatorMode = true;
		if (!ConnectAPI.s_gameServer.Active)
		{
			ConnectAPI.GotoGameServer(info, false);
			return;
		}
		ConnectAPI.SendGameServerHandshake(info);
	}

	// Token: 0x060008FD RID: 2301 RVA: 0x00023E34 File Offset: 0x00022034
	private static void SendGameServerHandshake(GameServerInfo info)
	{
		NetCache.NetCacheClientOptions netObject = NetCache.Get().GetNetObject<NetCache.NetCacheClientOptions>();
		if (netObject != null)
		{
			netObject.DispatchClientOptionsToServer();
		}
		if (info.SpectatorMode)
		{
			SpectatorHandshake spectatorHandshake = new SpectatorHandshake();
			spectatorHandshake.GameHandle = info.GameHandle;
			spectatorHandshake.Password = info.SpectatorPassword;
			spectatorHandshake.Version = BattleNet.GetVersion();
			spectatorHandshake.Platform = ConnectAPI.GetPlatformBuilder();
			BnetGameAccountId myGameAccountId = BnetPresenceMgr.Get().GetMyGameAccountId();
			spectatorHandshake.GameAccountId = new BnetId
			{
				Hi = myGameAccountId.GetHi(),
				Lo = myGameAccountId.GetLo()
			};
			ConnectAPI.QueueGamePacket(22, spectatorHandshake);
		}
		else
		{
			ConnectAPI.QueueGamePacket(168, new Handshake
			{
				Password = info.AuroraPassword,
				GameHandle = info.GameHandle,
				ClientHandle = (long)((int)info.ClientHandle),
				Mission = info.Mission,
				Version = info.Version,
				Platform = ConnectAPI.GetPlatformBuilder()
			});
		}
	}

	// Token: 0x060008FE RID: 2302 RVA: 0x00023F36 File Offset: 0x00022136
	public static void DisconnectFromGameServer()
	{
		ConnectAPI.s_disconnectRequested = true;
		ConnectAPI.s_gameServer.Disconnect();
	}

	// Token: 0x060008FF RID: 2303 RVA: 0x00023F48 File Offset: 0x00022148
	public static void SimulateUncleanDisconnectFromGameServer()
	{
		if (ConnectAPI.s_gameServer != null)
		{
			ConnectAPI.s_gameServer.Disconnect();
		}
	}

	// Token: 0x06000900 RID: 2304 RVA: 0x00023F60 File Offset: 0x00022160
	public static void GetGameState()
	{
		GetGameState body = new GetGameState();
		ConnectAPI.QueueGamePacket(1, body);
	}

	// Token: 0x06000901 RID: 2305 RVA: 0x00023F7C File Offset: 0x0002217C
	public static void SendPing()
	{
		Ping body = new Ping();
		ConnectAPI.QueueGamePacket(115, body);
	}

	// Token: 0x06000902 RID: 2306 RVA: 0x00023F98 File Offset: 0x00022198
	public static void Concede()
	{
		ConnectAPI.s_gameConceded = true;
		Concede body = new Concede();
		ConnectAPI.QueueGamePacket(11, body);
	}

	// Token: 0x06000903 RID: 2307 RVA: 0x00023FBC File Offset: 0x000221BC
	public static void SendChoices(int id, List<int> picks)
	{
		ChooseEntities chooseEntities = new ChooseEntities();
		chooseEntities.Id = id;
		for (int i = 0; i < picks.Count; i++)
		{
			chooseEntities.Entities.Add(picks[i]);
		}
		ConnectAPI.QueueGamePacket(3, chooseEntities);
	}

	// Token: 0x06000904 RID: 2308 RVA: 0x00024008 File Offset: 0x00022208
	public static void SendOption(int ID, int index, int target, int sub, int pos)
	{
		ConnectAPI.QueueGamePacket(2, new ChooseOption
		{
			Id = ID,
			Index = index,
			Target = target,
			SubOption = sub,
			Position = pos
		});
	}

	// Token: 0x06000905 RID: 2309 RVA: 0x00024048 File Offset: 0x00022248
	public static void SendEmote(int emote)
	{
		ConnectAPI.QueueGamePacket(15, new UserUI
		{
			Emote = emote
		});
	}

	// Token: 0x06000906 RID: 2310 RVA: 0x0002406C File Offset: 0x0002226C
	public static void SendUserUI(int overCard, int heldCard, int arrowOrigin, int x, int y)
	{
		ConnectAPI.QueueGamePacket(15, new UserUI
		{
			MouseInfo = new MouseInfo
			{
				ArrowOrigin = arrowOrigin,
				OverCard = overCard,
				HeldCard = heldCard,
				X = x,
				Y = y
			}
		});
	}

	// Token: 0x06000907 RID: 2311 RVA: 0x000240B8 File Offset: 0x000222B8
	public static void SendSpectatorInvite(BnetAccountId bnetAccountId, BnetGameAccountId bnetGameAccountId)
	{
		InviteToSpectate inviteToSpectate = new InviteToSpectate();
		BnetId bnetId = new BnetId();
		bnetId.Hi = bnetAccountId.GetHi();
		bnetId.Lo = bnetAccountId.GetLo();
		BnetId bnetId2 = new BnetId();
		bnetId2.Hi = bnetGameAccountId.GetHi();
		bnetId2.Lo = bnetGameAccountId.GetLo();
		inviteToSpectate.TargetBnetAccountId = bnetId;
		inviteToSpectate.TargetGameAccountId = bnetId2;
		ConnectAPI.QueueGamePacket(25, inviteToSpectate);
	}

	// Token: 0x06000908 RID: 2312 RVA: 0x00024120 File Offset: 0x00022320
	public static void SendRemoveSpectators(bool regenerateSpectatorPassword, params BnetGameAccountId[] bnetGameAccountIds)
	{
		RemoveSpectators removeSpectators = new RemoveSpectators();
		foreach (BnetGameAccountId bnetGameAccountId in bnetGameAccountIds)
		{
			BnetId bnetId = new BnetId();
			bnetId.Hi = bnetGameAccountId.GetHi();
			bnetId.Lo = bnetGameAccountId.GetLo();
			removeSpectators.TargetGameaccountIds.Add(bnetId);
		}
		if (regenerateSpectatorPassword)
		{
			removeSpectators.RegenerateSpectatorPassword = regenerateSpectatorPassword;
		}
		ConnectAPI.QueueGamePacket(26, removeSpectators);
	}

	// Token: 0x06000909 RID: 2313 RVA: 0x00024190 File Offset: 0x00022390
	public static void SendRemoveAllSpectators(bool regenerateSpectatorPassword)
	{
		RemoveSpectators removeSpectators = new RemoveSpectators();
		removeSpectators.KickAllSpectators = true;
		if (regenerateSpectatorPassword)
		{
			removeSpectators.RegenerateSpectatorPassword = regenerateSpectatorPassword;
		}
		ConnectAPI.QueueGamePacket(26, removeSpectators);
	}

	// Token: 0x0600090A RID: 2314 RVA: 0x000241C0 File Offset: 0x000223C0
	public static NetCache.NetCacheDisconnectedGame GetDisconnectedGameInfo()
	{
		NetCache.NetCacheDisconnectedGame netCacheDisconnectedGame = new NetCache.NetCacheDisconnectedGame();
		if (!Network.ShouldBeConnectedToAurora())
		{
			return netCacheDisconnectedGame;
		}
		Disconnected disconnected = ConnectAPI.Unpack<Disconnected>(ConnectAPI.NextUtil(), 289);
		if (disconnected != null && disconnected.HasAddress)
		{
			netCacheDisconnectedGame.ServerInfo = new GameServerInfo();
			netCacheDisconnectedGame.ServerInfo.Address = disconnected.Address;
			netCacheDisconnectedGame.ServerInfo.GameHandle = disconnected.GameHandle;
			netCacheDisconnectedGame.ServerInfo.ClientHandle = disconnected.ClientHandle;
			netCacheDisconnectedGame.ServerInfo.Port = disconnected.Port;
			netCacheDisconnectedGame.ServerInfo.AuroraPassword = disconnected.AuroraPassword;
			netCacheDisconnectedGame.ServerInfo.Mission = disconnected.Scenario;
			netCacheDisconnectedGame.ServerInfo.Version = BattleNet.GetVersion();
			netCacheDisconnectedGame.ServerInfo.Resumable = true;
		}
		return netCacheDisconnectedGame;
	}

	// Token: 0x0600090B RID: 2315 RVA: 0x00024290 File Offset: 0x00022490
	public static long DateToUTC(Date date)
	{
		DateTime dateTime;
		dateTime..ctor(date.Year, date.Month, date.Day, date.Hours, date.Min, date.Sec);
		return dateTime.ToFileTimeUtc();
	}

	// Token: 0x0600090C RID: 2316 RVA: 0x000242D0 File Offset: 0x000224D0
	public static int DraftGetError()
	{
		DraftError draftError = ConnectAPI.Unpack<DraftError>(ConnectAPI.NextUtil(), 251);
		if (draftError == null)
		{
			return 0;
		}
		return draftError.ErrorCode_;
	}

	// Token: 0x0600090D RID: 2317 RVA: 0x00024300 File Offset: 0x00022500
	public static Network.BeginDraft DraftGetBeginning()
	{
		DraftBeginning draftBeginning = ConnectAPI.Unpack<DraftBeginning>(ConnectAPI.NextUtil(), 246);
		if (draftBeginning == null)
		{
			return null;
		}
		Network.BeginDraft beginDraft = new Network.BeginDraft();
		beginDraft.DeckID = draftBeginning.DeckId;
		foreach (CardDef cardDef in draftBeginning.ChoiceList)
		{
			NetCache.CardDefinition cardDefinition = new NetCache.CardDefinition
			{
				Name = GameUtils.TranslateDbIdToCardId(cardDef.Asset),
				Premium = (TAG_PREMIUM)cardDef.Premium
			};
			beginDraft.Heroes.Add(cardDefinition);
		}
		return beginDraft;
	}

	// Token: 0x0600090E RID: 2318 RVA: 0x000243B8 File Offset: 0x000225B8
	private static RewardData ConvertRewardBag(RewardBag bag)
	{
		if (bag.HasRewardBooster)
		{
			return new BoosterPackRewardData(bag.RewardBooster.BoosterType, bag.RewardBooster.BoosterCount);
		}
		if (bag.HasRewardCard)
		{
			string cardID = GameUtils.TranslateDbIdToCardId(bag.RewardCard.Card.Asset);
			return new CardRewardData(cardID, (TAG_PREMIUM)bag.RewardCard.Card.Premium, 1);
		}
		if (bag.HasRewardDust)
		{
			return new ArcaneDustRewardData(bag.RewardDust.Amount);
		}
		if (bag.HasRewardGold)
		{
			return new GoldRewardData((long)bag.RewardGold.Amount);
		}
		if (bag.HasRewardCardBack)
		{
			return new CardBackRewardData(bag.RewardCardBack.CardBack);
		}
		Debug.LogError("Unrecognized draft bag reward");
		return null;
	}

	// Token: 0x0600090F RID: 2319 RVA: 0x00024488 File Offset: 0x00022688
	private static Network.RewardChest ConvertRewardChest(RewardChest chest)
	{
		Network.RewardChest rewardChest = new Network.RewardChest();
		if (chest.HasBag1)
		{
			rewardChest.Rewards.Add(ConnectAPI.ConvertRewardBag(chest.Bag1));
		}
		if (chest.HasBag2)
		{
			rewardChest.Rewards.Add(ConnectAPI.ConvertRewardBag(chest.Bag2));
		}
		if (chest.HasBag3)
		{
			rewardChest.Rewards.Add(ConnectAPI.ConvertRewardBag(chest.Bag3));
		}
		if (chest.HasBag4)
		{
			rewardChest.Rewards.Add(ConnectAPI.ConvertRewardBag(chest.Bag4));
		}
		if (chest.HasBag5)
		{
			rewardChest.Rewards.Add(ConnectAPI.ConvertRewardBag(chest.Bag5));
		}
		return rewardChest;
	}

	// Token: 0x06000910 RID: 2320 RVA: 0x00024544 File Offset: 0x00022744
	public static Network.DraftRetired DraftHandleRetired()
	{
		DraftRetired draftRetired = ConnectAPI.Unpack<DraftRetired>(ConnectAPI.NextUtil(), 247);
		if (draftRetired == null)
		{
			return null;
		}
		return new Network.DraftRetired
		{
			Deck = draftRetired.DeckId,
			Chest = ConnectAPI.ConvertRewardChest(draftRetired.Chest)
		};
	}

	// Token: 0x06000911 RID: 2321 RVA: 0x00024590 File Offset: 0x00022790
	public static long DraftHandleRewardsAck()
	{
		DraftRewardsAcked draftRewardsAcked = ConnectAPI.Unpack<DraftRewardsAcked>(ConnectAPI.NextUtil(), 288);
		if (draftRewardsAcked == null)
		{
			return 0L;
		}
		return draftRewardsAcked.DeckId;
	}

	// Token: 0x06000912 RID: 2322 RVA: 0x000245C0 File Offset: 0x000227C0
	public static Network.DraftChoicesAndContents DraftGetChoicesAndContents()
	{
		DraftChoicesAndContents draftChoicesAndContents = ConnectAPI.Unpack<DraftChoicesAndContents>(ConnectAPI.NextUtil(), 248);
		if (draftChoicesAndContents == null)
		{
			return null;
		}
		Network.DraftChoicesAndContents draftChoicesAndContents2 = new Network.DraftChoicesAndContents();
		draftChoicesAndContents2.DeckInfo.Deck = draftChoicesAndContents.DeckId;
		draftChoicesAndContents2.Slot = draftChoicesAndContents.Slot;
		draftChoicesAndContents2.Hero.Name = ((draftChoicesAndContents.HeroDef.Asset != 0) ? GameUtils.TranslateDbIdToCardId(draftChoicesAndContents.HeroDef.Asset) : string.Empty);
		draftChoicesAndContents2.Hero.Premium = (TAG_PREMIUM)draftChoicesAndContents.HeroDef.Premium;
		draftChoicesAndContents2.Wins = draftChoicesAndContents.Wins;
		draftChoicesAndContents2.Losses = draftChoicesAndContents.Losses;
		draftChoicesAndContents2.MaxWins = ((!draftChoicesAndContents.HasMaxWins) ? int.MaxValue : draftChoicesAndContents.MaxWins);
		foreach (CardDef cardDef in draftChoicesAndContents.ChoiceList)
		{
			if (cardDef.Asset != 0)
			{
				NetCache.CardDefinition cardDefinition = new NetCache.CardDefinition
				{
					Name = GameUtils.TranslateDbIdToCardId(cardDef.Asset),
					Premium = (TAG_PREMIUM)cardDef.Premium
				};
				draftChoicesAndContents2.Choices.Add(cardDefinition);
			}
		}
		foreach (DeckCardData deckCardData in draftChoicesAndContents.Cards)
		{
			Network.CardUserData cardUserData = new Network.CardUserData();
			cardUserData.DbId = deckCardData.Def.Asset;
			cardUserData.Count = ((!deckCardData.HasQty) ? 1 : deckCardData.Qty);
			cardUserData.Premium = (TAG_PREMIUM)((!deckCardData.Def.HasPremium) ? 0 : deckCardData.Def.Premium);
			draftChoicesAndContents2.DeckInfo.Cards.Add(cardUserData);
		}
		draftChoicesAndContents2.Chest = ((!draftChoicesAndContents.HasChest) ? null : ConnectAPI.ConvertRewardChest(draftChoicesAndContents.Chest));
		return draftChoicesAndContents2;
	}

	// Token: 0x06000913 RID: 2323 RVA: 0x000247F8 File Offset: 0x000229F8
	public static Network.DraftChosen DraftCardChosen()
	{
		DraftChosen draftChosen = ConnectAPI.Unpack<DraftChosen>(ConnectAPI.NextUtil(), 249);
		if (draftChosen == null)
		{
			return null;
		}
		Network.DraftChosen draftChosen2 = new Network.DraftChosen();
		draftChosen2.ChosenCard.Name = GameUtils.TranslateDbIdToCardId(draftChosen.Chosen.Asset);
		draftChosen2.ChosenCard.Premium = (TAG_PREMIUM)draftChosen.Chosen.Premium;
		foreach (CardDef cardDef in draftChosen.NextChoiceList)
		{
			NetCache.CardDefinition cardDefinition = new NetCache.CardDefinition
			{
				Name = GameUtils.TranslateDbIdToCardId(cardDef.Asset),
				Premium = (TAG_PREMIUM)cardDef.Premium
			};
			draftChosen2.NextChoices.Add(cardDefinition);
		}
		return draftChosen2;
	}

	// Token: 0x06000914 RID: 2324 RVA: 0x000248D4 File Offset: 0x00022AD4
	public static Network.MassDisenchantResponse GetMassDisenchantResponse()
	{
		MassDisenchantResponse massDisenchantResponse = ConnectAPI.Unpack<MassDisenchantResponse>(ConnectAPI.NextUtil(), 269);
		if (massDisenchantResponse == null)
		{
			return null;
		}
		return new Network.MassDisenchantResponse
		{
			Amount = massDisenchantResponse.Amount
		};
	}

	// Token: 0x06000915 RID: 2325 RVA: 0x0002490C File Offset: 0x00022B0C
	public static void SetFavoriteHero(TAG_CLASS heroClass, NetCache.CardDefinition hero)
	{
		ConnectAPI.UtilOutbound(319, 0, new SetFavoriteHero
		{
			FavoriteHero = new FavoriteHero(),
			FavoriteHero = 
			{
				ClassId = (int)heroClass,
				Hero = new CardDef(),
				Hero = 
				{
					Asset = GameUtils.TranslateCardIdToDbId(hero.Name),
					Premium = (int)hero.Premium
				}
			}
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000916 RID: 2326 RVA: 0x00024988 File Offset: 0x00022B88
	public static Network.SetFavoriteHeroResponse GetSetFavoriteHeroResponse()
	{
		SetFavoriteHeroResponse setFavoriteHeroResponse = ConnectAPI.Unpack<SetFavoriteHeroResponse>(ConnectAPI.NextUtil(), 320);
		if (setFavoriteHeroResponse == null)
		{
			return null;
		}
		Network.SetFavoriteHeroResponse setFavoriteHeroResponse2 = new Network.SetFavoriteHeroResponse();
		setFavoriteHeroResponse2.Success = setFavoriteHeroResponse.Success;
		if (setFavoriteHeroResponse.HasFavoriteHero)
		{
			if (!EnumUtils.TryCast<TAG_CLASS>(setFavoriteHeroResponse.FavoriteHero.ClassId, out setFavoriteHeroResponse2.HeroClass))
			{
				Debug.LogWarning(string.Format("ConnectAPI.GetSetFavoriteHeroResponse() invalid class {0}", setFavoriteHeroResponse.FavoriteHero.ClassId));
			}
			TAG_PREMIUM premium;
			if (!EnumUtils.TryCast<TAG_PREMIUM>(setFavoriteHeroResponse.FavoriteHero.Hero.Premium, out premium))
			{
				Debug.LogWarning(string.Format("ConnectAPI.GetSetFavoriteHeroResponse() invalid heroPremium {0}", setFavoriteHeroResponse.FavoriteHero.Hero.Premium));
			}
			setFavoriteHeroResponse2.Hero = new NetCache.CardDefinition
			{
				Name = GameUtils.TranslateDbIdToCardId(setFavoriteHeroResponse.FavoriteHero.Hero.Asset),
				Premium = premium
			};
		}
		return setFavoriteHeroResponse2;
	}

	// Token: 0x06000917 RID: 2327 RVA: 0x00024A80 File Offset: 0x00022C80
	private static Network.PurchaseErrorInfo ConvertPurchaseError(PurchaseError purchaseError)
	{
		Network.PurchaseErrorInfo purchaseErrorInfo = new Network.PurchaseErrorInfo();
		purchaseErrorInfo.Error = purchaseError.Error_;
		if (purchaseError.HasPurchaseInProgress)
		{
			purchaseErrorInfo.PurchaseInProgressProductID = purchaseError.PurchaseInProgress;
		}
		if (purchaseError.HasErrorCode)
		{
			purchaseErrorInfo.ErrorCode = purchaseError.ErrorCode;
		}
		return purchaseErrorInfo;
	}

	// Token: 0x06000918 RID: 2328 RVA: 0x00024AD0 File Offset: 0x00022CD0
	public static Network.PurchaseCanceledResponse GetPurchaseCanceledResponse()
	{
		CancelPurchaseResponse cancelPurchaseResponse = ConnectAPI.Unpack<CancelPurchaseResponse>(ConnectAPI.NextUtil(), 275);
		if (cancelPurchaseResponse == null)
		{
			return null;
		}
		Network.PurchaseCanceledResponse purchaseCanceledResponse = new Network.PurchaseCanceledResponse();
		purchaseCanceledResponse.TransactionID = ((!cancelPurchaseResponse.HasTransactionId) ? 0L : cancelPurchaseResponse.TransactionId);
		purchaseCanceledResponse.ProductID = ((!cancelPurchaseResponse.HasProductId) ? string.Empty : cancelPurchaseResponse.ProductId);
		purchaseCanceledResponse.CurrencyType = (Currency)((!cancelPurchaseResponse.HasCurrency) ? 0 : cancelPurchaseResponse.Currency);
		switch (cancelPurchaseResponse.Result)
		{
		case 1:
			purchaseCanceledResponse.Result = Network.PurchaseCanceledResponse.CancelResult.SUCCESS;
			break;
		case 2:
			purchaseCanceledResponse.Result = Network.PurchaseCanceledResponse.CancelResult.NOT_ALLOWED;
			break;
		case 3:
			purchaseCanceledResponse.Result = Network.PurchaseCanceledResponse.CancelResult.NOTHING_TO_CANCEL;
			break;
		}
		return purchaseCanceledResponse;
	}

	// Token: 0x06000919 RID: 2329 RVA: 0x00024B9C File Offset: 0x00022D9C
	public static Network.BattlePayStatus GetBattlePayStatusResponse()
	{
		BattlePayStatusResponse battlePayStatusResponse = ConnectAPI.Unpack<BattlePayStatusResponse>(ConnectAPI.NextUtil(), 265);
		if (battlePayStatusResponse == null)
		{
			return null;
		}
		Network.BattlePayStatus battlePayStatus = new Network.BattlePayStatus();
		battlePayStatus.State = battlePayStatusResponse.Status;
		battlePayStatus.BattlePayAvailable = battlePayStatusResponse.BattlePayAvailable;
		battlePayStatus.CurrencyType = (Currency)((!battlePayStatusResponse.HasCurrency) ? 0 : battlePayStatusResponse.Currency);
		if (battlePayStatusResponse.HasTransactionId)
		{
			battlePayStatus.TransactionID = battlePayStatusResponse.TransactionId;
		}
		if (battlePayStatusResponse.HasProductId)
		{
			battlePayStatus.ProductID = battlePayStatusResponse.ProductId;
		}
		if (battlePayStatusResponse.HasPurchaseError)
		{
			battlePayStatus.PurchaseError = ConnectAPI.ConvertPurchaseError(battlePayStatusResponse.PurchaseError);
		}
		if (battlePayStatusResponse.HasThirdPartyId)
		{
			battlePayStatus.ThirdPartyID = battlePayStatusResponse.ThirdPartyId;
		}
		if (battlePayStatusResponse.HasProvider)
		{
			battlePayStatus.Provider = new BattlePayProvider?(battlePayStatusResponse.Provider);
		}
		return battlePayStatus;
	}

	// Token: 0x0600091A RID: 2330 RVA: 0x00024C7C File Offset: 0x00022E7C
	public static Network.BattlePayConfig GetBattlePayConfigResponse()
	{
		BattlePayConfigResponse battlePayConfigResponse = ConnectAPI.Unpack<BattlePayConfigResponse>(ConnectAPI.NextUtil(), 238);
		if (battlePayConfigResponse == null)
		{
			return null;
		}
		Network.BattlePayConfig battlePayConfig = new Network.BattlePayConfig();
		battlePayConfig.Available = (!battlePayConfigResponse.HasUnavailable || !battlePayConfigResponse.Unavailable);
		battlePayConfig.Currency = (Currency)((!battlePayConfigResponse.HasCurrency) ? 0 : battlePayConfigResponse.Currency);
		battlePayConfig.SecondsBeforeAutoCancel = ((!battlePayConfigResponse.HasSecsBeforeAutoCancel) ? StoreManager.DEFAULT_SECONDS_BEFORE_AUTO_CANCEL : battlePayConfigResponse.SecsBeforeAutoCancel);
		foreach (Bundle bundle in battlePayConfigResponse.Bundles)
		{
			Network.Bundle bundle2 = new Network.Bundle
			{
				ProductID = bundle.Id,
				Cost = default(double?),
				GoldCost = default(long?),
				AppleID = ((!bundle.HasAppleId) ? string.Empty : bundle.AppleId),
				GooglePlayID = ((!bundle.HasGooglePlayId) ? string.Empty : bundle.GooglePlayId),
				AmazonID = ((!bundle.HasAmazonId) ? string.Empty : bundle.AmazonId),
				ExclusiveProviders = bundle.ExclusiveProviders
			};
			if (bundle.HasCost && bundle.Cost > 0.0)
			{
				bundle2.Cost = new double?(bundle.Cost);
			}
			if (bundle.HasGoldCost && bundle.GoldCost > 0L)
			{
				bundle2.GoldCost = new long?(bundle.GoldCost);
			}
			if (bundle.HasProductEventName)
			{
				bundle2.ProductEvent = SpecialEventManager.GetEventType(bundle.ProductEventName, SpecialEventType.UNKNOWN);
			}
			if (bundle.HasRealMoneyProductEventName)
			{
				bundle2.RealMoneyProductEvent = SpecialEventManager.GetEventType(bundle.RealMoneyProductEventName, SpecialEventType.UNKNOWN);
			}
			foreach (BundleItem bundleItem in bundle.Items)
			{
				Network.BundleItem bundleItem2 = new Network.BundleItem
				{
					Product = bundleItem.ProductType,
					ProductData = bundleItem.Data,
					Quantity = bundleItem.Quantity
				};
				bundle2.Items.Add(bundleItem2);
			}
			battlePayConfig.Bundles.Add(bundle2);
		}
		foreach (GoldCostBooster goldCostBooster in battlePayConfigResponse.GoldCostBoosters)
		{
			Network.GoldCostBooster goldCostBooster2 = new Network.GoldCostBooster
			{
				ID = goldCostBooster.PackType
			};
			if (goldCostBooster.Cost > 0L)
			{
				goldCostBooster2.Cost = new long?(goldCostBooster.Cost);
			}
			else
			{
				goldCostBooster2.Cost = default(long?);
			}
			if (goldCostBooster.HasBuyWithGoldEventName)
			{
				goldCostBooster2.BuyWithGoldEvent = SpecialEventManager.GetEventType(goldCostBooster.BuyWithGoldEventName, SpecialEventType.UNKNOWN);
			}
			battlePayConfig.GoldCostBoosters.Add(goldCostBooster2);
		}
		if (battlePayConfigResponse.HasGoldCostArena && battlePayConfigResponse.GoldCostArena > 0L)
		{
			battlePayConfig.GoldCostArena = new long?(battlePayConfigResponse.GoldCostArena);
		}
		else
		{
			battlePayConfig.GoldCostArena = default(long?);
		}
		return battlePayConfig;
	}

	// Token: 0x0600091B RID: 2331 RVA: 0x00025048 File Offset: 0x00023248
	public static void PurchaseViaGold(int quantity, ProductType product, int data)
	{
		ConnectAPI.UtilOutbound(279, 0, new PurchaseWithGold
		{
			Product = product,
			Data = data,
			Quantity = quantity
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x0600091C RID: 2332 RVA: 0x00025080 File Offset: 0x00023280
	public static void BeginThirdPartyPurchase(BattlePayProvider provider, string productID, int quantity)
	{
		ConnectAPI.UtilOutbound(312, 1, new StartThirdPartyPurchase
		{
			Provider = provider,
			ProductId = productID,
			Quantity = quantity,
			DeviceId = SystemInfo.deviceUniqueIdentifier
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x0600091D RID: 2333 RVA: 0x000250C4 File Offset: 0x000232C4
	public static void BeginThirdPartyPurchaseWithReceipt(BattlePayProvider provider, string productID, int quantity, string thirdPartyID, string base64receipt, string thirdPartyUserID)
	{
		string message = string.Concat(new object[]
		{
			provider,
			"|",
			productID,
			"|",
			thirdPartyID,
			"|",
			thirdPartyUserID
		});
		BIReport.Get().Report_Telemetry(Telemetry.Level.LEVEL_INFO, BIReport.TelemetryEvent.EVENT_THIRD_PARTY_PURCHASE_RECEIPT_SUBMITTED_DANGLING, 0, message);
		ThirdPartyReceiptData thirdPartyReceiptData = new ThirdPartyReceiptData();
		thirdPartyReceiptData.ThirdPartyId = thirdPartyID;
		thirdPartyReceiptData.Receipt = base64receipt;
		if (!string.IsNullOrEmpty(thirdPartyUserID))
		{
			thirdPartyReceiptData.ThirdPartyUserId = thirdPartyUserID;
		}
		ConnectAPI.UtilOutbound(312, 1, new StartThirdPartyPurchase
		{
			Provider = provider,
			ProductId = productID,
			Quantity = quantity,
			DanglingReceiptData = thirdPartyReceiptData,
			DeviceId = SystemInfo.deviceUniqueIdentifier
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x00025184 File Offset: 0x00023384
	public static void SubmitThirdPartyPurchaseReceipt(long bpayID, string thirdPartyID, string base64receipt, string thirdPartyUserID)
	{
		string message = string.Concat(new object[]
		{
			bpayID,
			"|",
			thirdPartyID,
			"|",
			thirdPartyUserID
		});
		BIReport.Get().Report_Telemetry(Telemetry.Level.LEVEL_INFO, BIReport.TelemetryEvent.EVENT_THIRD_PARTY_PURCHASE_RECEIPT_SUBMITTED, 0, message);
		ThirdPartyReceiptData thirdPartyReceiptData = new ThirdPartyReceiptData();
		thirdPartyReceiptData.ThirdPartyId = thirdPartyID;
		thirdPartyReceiptData.Receipt = base64receipt;
		if (!string.IsNullOrEmpty(thirdPartyUserID))
		{
			thirdPartyReceiptData.ThirdPartyUserId = thirdPartyUserID;
		}
		ConnectAPI.UtilOutbound(293, 1, new SubmitThirdPartyReceipt
		{
			TransactionId = bpayID,
			ReceiptData = thirdPartyReceiptData
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x0600091F RID: 2335 RVA: 0x00025218 File Offset: 0x00023418
	public static void GetThirdPartyPurchaseStatus(string transactionID)
	{
		ConnectAPI.UtilOutbound(294, 1, new GetThirdPartyPurchaseStatus
		{
			ThirdPartyId = transactionID
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000920 RID: 2336 RVA: 0x00025240 File Offset: 0x00023440
	public static Network.ThirdPartyPurchaseStatusResponse GetThirdPartyPurchaseStatusResponse()
	{
		ThirdPartyPurchaseStatusResponse thirdPartyPurchaseStatusResponse = ConnectAPI.Unpack<ThirdPartyPurchaseStatusResponse>(ConnectAPI.NextUtil(), 295);
		if (thirdPartyPurchaseStatusResponse == null)
		{
			return null;
		}
		return new Network.ThirdPartyPurchaseStatusResponse
		{
			ThirdPartyID = thirdPartyPurchaseStatusResponse.ThirdPartyId,
			Status = thirdPartyPurchaseStatusResponse.Status_
		};
	}

	// Token: 0x06000921 RID: 2337 RVA: 0x00025284 File Offset: 0x00023484
	public static Network.PurchaseViaGoldResponse GetPurchaseWithGoldResponse()
	{
		PurchaseWithGoldResponse purchaseWithGoldResponse = ConnectAPI.Unpack<PurchaseWithGoldResponse>(ConnectAPI.NextUtil(), 280);
		if (purchaseWithGoldResponse == null)
		{
			return null;
		}
		Network.PurchaseViaGoldResponse purchaseViaGoldResponse = new Network.PurchaseViaGoldResponse();
		purchaseViaGoldResponse.Error = purchaseWithGoldResponse.Result;
		if (purchaseWithGoldResponse.HasGoldUsed)
		{
			purchaseViaGoldResponse.GoldUsed = purchaseWithGoldResponse.GoldUsed;
		}
		return purchaseViaGoldResponse;
	}

	// Token: 0x06000922 RID: 2338 RVA: 0x000252D4 File Offset: 0x000234D4
	public static Network.PurchaseMethod GetPurchaseMethodResponse()
	{
		PurchaseMethod purchaseMethod = ConnectAPI.Unpack<PurchaseMethod>(ConnectAPI.NextUtil(), 272);
		if (purchaseMethod == null)
		{
			return null;
		}
		Network.PurchaseMethod purchaseMethod2 = new Network.PurchaseMethod();
		if (purchaseMethod.HasTransactionId)
		{
			purchaseMethod2.TransactionID = purchaseMethod.TransactionId;
		}
		if (purchaseMethod.HasProductId)
		{
			purchaseMethod2.ProductID = purchaseMethod.ProductId;
		}
		if (purchaseMethod.HasQuantity)
		{
			purchaseMethod2.Quantity = purchaseMethod.Quantity;
		}
		if (purchaseMethod.HasCurrency)
		{
			purchaseMethod2.Currency = (Currency)purchaseMethod.Currency;
		}
		if (purchaseMethod.HasWalletName)
		{
			purchaseMethod2.WalletName = purchaseMethod.WalletName;
		}
		if (purchaseMethod.HasUseEbalance)
		{
			purchaseMethod2.UseEBalance = purchaseMethod.UseEbalance;
		}
		purchaseMethod2.IsZeroCostLicense = (purchaseMethod.HasIsZeroCostLicense && purchaseMethod.IsZeroCostLicense);
		if (purchaseMethod.HasChallengeId)
		{
			purchaseMethod2.ChallengeID = purchaseMethod.ChallengeId;
		}
		if (purchaseMethod.HasChallengeUrl)
		{
			purchaseMethod2.ChallengeURL = purchaseMethod.ChallengeUrl;
		}
		if (purchaseMethod.HasError)
		{
			purchaseMethod2.PurchaseError = ConnectAPI.ConvertPurchaseError(purchaseMethod.Error);
		}
		return purchaseMethod2;
	}

	// Token: 0x06000923 RID: 2339 RVA: 0x000253F0 File Offset: 0x000235F0
	public static Network.PurchaseResponse GetPurchaseResponse()
	{
		PurchaseResponse purchaseResponse = ConnectAPI.Unpack<PurchaseResponse>(ConnectAPI.NextUtil(), 256);
		if (purchaseResponse == null)
		{
			return null;
		}
		return new Network.PurchaseResponse
		{
			PurchaseError = ConnectAPI.ConvertPurchaseError(purchaseResponse.Error),
			TransactionID = ((!purchaseResponse.HasTransactionId) ? 0L : purchaseResponse.TransactionId),
			ProductID = ((!purchaseResponse.HasProductId) ? string.Empty : purchaseResponse.ProductId),
			ThirdPartyID = ((!purchaseResponse.HasThirdPartyId) ? string.Empty : purchaseResponse.ThirdPartyId),
			CurrencyType = (Currency)((!purchaseResponse.HasCurrency) ? 0 : purchaseResponse.Currency)
		};
	}

	// Token: 0x06000924 RID: 2340 RVA: 0x000254AC File Offset: 0x000236AC
	public static Network.CardBackResponse GetCardBackReponse()
	{
		SetCardBackResponse setCardBackResponse = ConnectAPI.Unpack<SetCardBackResponse>(ConnectAPI.NextUtil(), 292);
		if (setCardBackResponse == null)
		{
			return null;
		}
		return new Network.CardBackResponse
		{
			Success = setCardBackResponse.Success,
			CardBack = setCardBackResponse.CardBack
		};
	}

	// Token: 0x06000925 RID: 2341 RVA: 0x000254F0 File Offset: 0x000236F0
	private static bool AreDeckFlagsWild(ulong deckValidityFlags)
	{
		return (deckValidityFlags & 128UL) == 0UL;
	}

	// Token: 0x06000926 RID: 2342 RVA: 0x000254FE File Offset: 0x000236FE
	private static bool DeckNeedsName(ulong deckValidityFlags)
	{
		return (deckValidityFlags & 512UL) != 0UL;
	}

	// Token: 0x06000927 RID: 2343 RVA: 0x00025510 File Offset: 0x00023710
	public static NetCache.NetCacheCardBacks GetCardBacks()
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			return new NetCache.NetCacheCardBacks();
		}
		CardBacks cardBacks = ConnectAPI.Unpack<CardBacks>(ConnectAPI.NextUtil(), 236);
		if (cardBacks == null)
		{
			return null;
		}
		NetCache.NetCacheCardBacks netCacheCardBacks = new NetCache.NetCacheCardBacks();
		netCacheCardBacks.DefaultCardBack = cardBacks.DefaultCardBack;
		foreach (int num in cardBacks.CardBacks_)
		{
			netCacheCardBacks.CardBacks.Add(num);
		}
		return netCacheCardBacks;
	}

	// Token: 0x06000928 RID: 2344 RVA: 0x000255B0 File Offset: 0x000237B0
	public static NetCache.NetCacheDecks GetDeckHeaders()
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			return new NetCache.NetCacheDecks();
		}
		DeckList deckList = ConnectAPI.Unpack<DeckList>(ConnectAPI.NextUtil(), 202);
		if (deckList == null)
		{
			return null;
		}
		NetCache.NetCacheDecks netCacheDecks = new NetCache.NetCacheDecks();
		foreach (DeckInfo deckInfo in deckList.Decks)
		{
			NetCache.DeckHeader deckHeader = new NetCache.DeckHeader();
			deckHeader.ID = deckInfo.Id;
			deckHeader.Name = deckInfo.Name;
			deckHeader.Hero = GameUtils.TranslateDbIdToCardId(deckInfo.Hero);
			deckHeader.HeroPremium = (TAG_PREMIUM)deckInfo.HeroPremium;
			deckHeader.HeroPower = GameUtils.GetHeroPowerCardIdFromHero(deckInfo.Hero);
			deckHeader.Type = deckInfo.DeckType;
			deckHeader.CardBack = deckInfo.CardBack;
			deckHeader.CardBackOverridden = deckInfo.CardBackOverride;
			deckHeader.HeroOverridden = deckInfo.HeroOverride;
			deckHeader.SeasonId = deckInfo.SeasonId;
			deckHeader.NeedsName = ConnectAPI.DeckNeedsName(deckInfo.Validity);
			deckHeader.SortOrder = ((!deckInfo.HasSortOrder) ? deckInfo.Id : deckInfo.SortOrder);
			deckHeader.IsWild = ConnectAPI.AreDeckFlagsWild(deckInfo.Validity);
			deckHeader.SourceType = ((!deckInfo.HasSourceType) ? 0 : deckInfo.SourceType);
			if (deckInfo.HasCreateDate)
			{
				deckHeader.CreateDate = new DateTime?(GeneralUtils.UnixTimeStampToDateTime((ulong)deckInfo.CreateDate));
			}
			else
			{
				deckHeader.CreateDate = default(DateTime?);
			}
			if (deckInfo.HasLastModified)
			{
				deckHeader.LastModified = new DateTime?(GeneralUtils.UnixTimeStampToDateTime((ulong)deckInfo.LastModified));
			}
			else
			{
				deckHeader.LastModified = default(DateTime?);
			}
			netCacheDecks.Decks.Add(deckHeader);
		}
		return netCacheDecks;
	}

	// Token: 0x06000929 RID: 2345 RVA: 0x000257B8 File Offset: 0x000239B8
	public static GetDeckContentsResponse GetDeckContentsResponse()
	{
		return ConnectAPI.Unpack<GetDeckContentsResponse>(ConnectAPI.NextUtil(), 215);
	}

	// Token: 0x0600092A RID: 2346 RVA: 0x000257D8 File Offset: 0x000239D8
	public static Network.GenericResponse GetGenericResponse()
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			return new Network.GenericResponse
			{
				RequestId = 0,
				RequestSubId = 0,
				ResultCode = Network.GenericResponse.Result.OK
			};
		}
		GenericResponse genericResponse = ConnectAPI.Unpack<GenericResponse>(ConnectAPI.NextUtil(), 326);
		if (genericResponse == null)
		{
			return null;
		}
		return new Network.GenericResponse
		{
			ResultCode = genericResponse.ResultCode,
			RequestId = genericResponse.RequestId,
			RequestSubId = ((!genericResponse.HasRequestSubId) ? 0 : genericResponse.RequestSubId),
			GenericData = genericResponse.GenericData
		};
	}

	// Token: 0x0600092B RID: 2347 RVA: 0x0002586C File Offset: 0x00023A6C
	public static Network.DBAction DBAction()
	{
		DBAction dbaction = ConnectAPI.Unpack<DBAction>(ConnectAPI.NextUtil(), 216);
		if (dbaction == null)
		{
			return null;
		}
		return new Network.DBAction
		{
			Action = dbaction.Action,
			Result = dbaction.Result,
			MetaData = dbaction.MetaData
		};
	}

	// Token: 0x0600092C RID: 2348 RVA: 0x000258BC File Offset: 0x00023ABC
	public static NetCache.DeckHeader DeckCreated()
	{
		DeckCreated deckCreated = ConnectAPI.Unpack<DeckCreated>(ConnectAPI.NextUtil(), 217);
		if (deckCreated == null)
		{
			return null;
		}
		DeckInfo info = deckCreated.Info;
		return new NetCache.DeckHeader
		{
			ID = info.Id,
			Name = info.Name,
			Hero = GameUtils.TranslateDbIdToCardId(info.Hero),
			HeroPremium = (TAG_PREMIUM)info.HeroPremium,
			HeroPower = GameUtils.GetHeroPowerCardIdFromHero(info.Hero),
			Type = info.DeckType,
			CardBack = info.CardBack,
			CardBackOverridden = info.CardBackOverride,
			HeroOverridden = info.HeroOverride,
			SeasonId = info.SeasonId,
			NeedsName = ConnectAPI.DeckNeedsName(info.Validity),
			SortOrder = ((!info.HasSortOrder) ? info.Id : info.SortOrder),
			IsWild = ConnectAPI.AreDeckFlagsWild(info.Validity)
		};
	}

	// Token: 0x0600092D RID: 2349 RVA: 0x000259B8 File Offset: 0x00023BB8
	public static long DeckDeleted()
	{
		DeckDeleted deckDeleted = ConnectAPI.Unpack<DeckDeleted>(ConnectAPI.NextUtil(), 218);
		if (deckDeleted == null)
		{
			return 0L;
		}
		return deckDeleted.Deck;
	}

	// Token: 0x0600092E RID: 2350 RVA: 0x000259E4 File Offset: 0x00023BE4
	public static Network.DeckName DeckRenamed()
	{
		DeckRenamed deckRenamed = ConnectAPI.Unpack<DeckRenamed>(ConnectAPI.NextUtil(), 219);
		if (deckRenamed == null)
		{
			return null;
		}
		return new Network.DeckName
		{
			Deck = deckRenamed.Deck,
			Name = deckRenamed.Name
		};
	}

	// Token: 0x0600092F RID: 2351 RVA: 0x00025A28 File Offset: 0x00023C28
	public static Network.CardSaleResult GetCardSaleResult()
	{
		BoughtSoldCard boughtSoldCard = ConnectAPI.Unpack<BoughtSoldCard>(ConnectAPI.NextUtil(), 258);
		if (boughtSoldCard == null)
		{
			return null;
		}
		return new Network.CardSaleResult
		{
			AssetID = boughtSoldCard.Def.Asset,
			AssetName = GameUtils.TranslateDbIdToCardId(boughtSoldCard.Def.Asset),
			Premium = (TAG_PREMIUM)((!boughtSoldCard.Def.HasPremium) ? 0 : boughtSoldCard.Def.Premium),
			Action = boughtSoldCard.Result_,
			Amount = boughtSoldCard.Amount,
			Count = ((!boughtSoldCard.HasCount) ? 1 : boughtSoldCard.Count),
			Nerfed = (boughtSoldCard.HasNerfed && boughtSoldCard.Nerfed),
			UnitSellPrice = ((!boughtSoldCard.HasUnitSellPrice) ? 0 : boughtSoldCard.UnitSellPrice),
			UnitBuyPrice = ((!boughtSoldCard.HasUnitBuyPrice) ? 0 : boughtSoldCard.UnitBuyPrice)
		};
	}

	// Token: 0x06000930 RID: 2352 RVA: 0x00025B30 File Offset: 0x00023D30
	public static NetCache.NetCacheCollection GetCollectionCardStacks()
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			return new NetCache.NetCacheCollection();
		}
		Collection collection = ConnectAPI.Unpack<Collection>(ConnectAPI.NextUtil(), 207);
		if (collection == null)
		{
			return null;
		}
		NetCache.NetCacheCollection netCacheCollection = new NetCache.NetCacheCollection();
		foreach (CardStack cardStack in collection.Stacks)
		{
			NetCache.CardStack cardStack2 = new NetCache.CardStack();
			cardStack2.Def.Name = GameUtils.TranslateDbIdToCardId(cardStack.CardDef.Asset);
			cardStack2.Def.Premium = (TAG_PREMIUM)cardStack.CardDef.Premium;
			cardStack2.Date = ConnectAPI.DateToUTC(cardStack.LatestInsertDate);
			cardStack2.Count = cardStack.Count;
			cardStack2.NumSeen = cardStack.NumSeen;
			netCacheCollection.Stacks.Add(cardStack2);
			netCacheCollection.TotalCardsOwned += cardStack2.Count;
			if (cardStack2.Def.Premium == TAG_PREMIUM.NORMAL && cardStack2.Count > 0)
			{
				EntityDef entityDef = DefLoader.Get().GetEntityDef(cardStack2.Def.Name);
				if (entityDef.IsBasicCardUnlock())
				{
					int num = (!entityDef.IsElite()) ? 2 : 1;
					Map<TAG_CLASS, int> basicCardsUnlockedPerClass;
					Map<TAG_CLASS, int> map = basicCardsUnlockedPerClass = netCacheCollection.BasicCardsUnlockedPerClass;
					TAG_CLASS @class;
					TAG_CLASS key = @class = entityDef.GetClass();
					int num2 = basicCardsUnlockedPerClass[@class];
					map[key] = num2 + Math.Min(num, cardStack2.Count);
				}
			}
		}
		return netCacheCollection;
	}

	// Token: 0x06000931 RID: 2353 RVA: 0x00025CD4 File Offset: 0x00023ED4
	public static List<NetCache.BoosterCard> GetOpenedBooster()
	{
		BoosterContent boosterContent = ConnectAPI.Unpack<BoosterContent>(ConnectAPI.NextUtil(), 226);
		if (boosterContent == null)
		{
			return null;
		}
		List<NetCache.BoosterCard> list = new List<NetCache.BoosterCard>();
		foreach (BoosterCard boosterCard in boosterContent.List)
		{
			list.Add(new NetCache.BoosterCard
			{
				Def = 
				{
					Name = GameUtils.TranslateDbIdToCardId(boosterCard.CardDef.Asset),
					Premium = (TAG_PREMIUM)boosterCard.CardDef.Premium
				},
				Date = ConnectAPI.DateToUTC(boosterCard.InsertDate)
			});
		}
		return list;
	}

	// Token: 0x06000932 RID: 2354 RVA: 0x00025D9C File Offset: 0x00023F9C
	public static NetCache.NetCacheGamesPlayed GetGamesInfo()
	{
		GamesInfo gamesInfo = ConnectAPI.Unpack<GamesInfo>(ConnectAPI.NextUtil(), 208);
		if (gamesInfo == null)
		{
			return null;
		}
		return new NetCache.NetCacheGamesPlayed
		{
			GamesStarted = gamesInfo.GamesStarted,
			GamesWon = gamesInfo.GamesWon,
			GamesLost = gamesInfo.GamesLost,
			FreeRewardProgress = gamesInfo.FreeRewardProgress
		};
	}

	// Token: 0x06000933 RID: 2355 RVA: 0x00025DF8 File Offset: 0x00023FF8
	public static NetCache.NetCacheProfileProgress GetProfileProgress()
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			return new NetCache.NetCacheProfileProgress
			{
				CampaignProgress = Options.Get().GetEnum<TutorialProgress>(Option.LOCAL_TUTORIAL_PROGRESS)
			};
		}
		ProfileProgress profileProgress = ConnectAPI.Unpack<ProfileProgress>(ConnectAPI.NextUtil(), 233);
		if (profileProgress == null)
		{
			return null;
		}
		NetCache.NetCacheProfileProgress netCacheProfileProgress = new NetCache.NetCacheProfileProgress();
		netCacheProfileProgress.CampaignProgress = (TutorialProgress)profileProgress.Progress;
		netCacheProfileProgress.BestForgeWins = profileProgress.BestForge;
		netCacheProfileProgress.LastForgeDate = ((!profileProgress.HasLastForge) ? 0L : ConnectAPI.DateToUTC(profileProgress.LastForge));
		netCacheProfileProgress.DisplayBanner = profileProgress.DisplayBanner;
		if (profileProgress.AdventureOptions.Count > 0)
		{
			netCacheProfileProgress.AdventureOptions = new AdventureOption[profileProgress.AdventureOptions.Count];
			for (int i = 0; i < profileProgress.AdventureOptions.Count; i++)
			{
				AdventureOptions adventureOptions = profileProgress.AdventureOptions[i];
				AdventureOption adventureOption = new AdventureOption();
				adventureOption.AdventureID = adventureOptions.AdventureId;
				adventureOption.Options = adventureOptions.Options;
				netCacheProfileProgress.AdventureOptions[i] = adventureOption;
			}
		}
		return netCacheProfileProgress;
	}

	// Token: 0x06000934 RID: 2356 RVA: 0x00025F10 File Offset: 0x00024110
	public static NetCache.NetCachePlayQueue GetPlayQueue()
	{
		PlayQueue playQueue = ConnectAPI.Unpack<PlayQueue>(ConnectAPI.NextUtil(), 286);
		if (playQueue == null)
		{
			return null;
		}
		return new NetCache.NetCachePlayQueue
		{
			GameType = playQueue.Queue.GameType
		};
	}

	// Token: 0x06000935 RID: 2357 RVA: 0x00025F50 File Offset: 0x00024150
	public static NetCache.NetCachePlayerRecords GetPlayerRecords()
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			return new NetCache.NetCachePlayerRecords();
		}
		PlayerRecords playerRecords = ConnectAPI.Unpack<PlayerRecords>(ConnectAPI.NextUtil(), 270);
		if (playerRecords == null)
		{
			return null;
		}
		NetCache.NetCachePlayerRecords netCachePlayerRecords = new NetCache.NetCachePlayerRecords();
		List<PlayerRecord> records = playerRecords.Records;
		foreach (PlayerRecord playerRecord in records)
		{
			netCachePlayerRecords.Records.Add(new NetCache.PlayerRecord
			{
				RecordType = playerRecord.Type,
				Data = ((!playerRecord.HasData) ? 0 : playerRecord.Data),
				Wins = playerRecord.Wins,
				Losses = playerRecord.Losses,
				Ties = playerRecord.Ties
			});
		}
		return netCachePlayerRecords;
	}

	// Token: 0x06000936 RID: 2358 RVA: 0x00026048 File Offset: 0x00024248
	public static NetCache.NetCacheRewardProgress GetRewardProgress()
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			return new NetCache.NetCacheRewardProgress();
		}
		RewardProgress rewardProgress = ConnectAPI.Unpack<RewardProgress>(ConnectAPI.NextUtil(), 271);
		if (rewardProgress == null)
		{
			return null;
		}
		return new NetCache.NetCacheRewardProgress
		{
			Season = rewardProgress.SeasonNumber,
			SeasonEndDate = ConnectAPI.DateToUTC(rewardProgress.SeasonEnd),
			WinsPerGold = rewardProgress.WinsPerGold,
			GoldPerReward = rewardProgress.GoldPerReward,
			MaxGoldPerDay = rewardProgress.MaxGoldPerDay,
			PackRewardId = ((!rewardProgress.HasPackId) ? 1 : rewardProgress.PackId),
			XPSoloLimit = rewardProgress.XpSoloLimit,
			MaxHeroLevel = rewardProgress.MaxHeroLevel,
			NextQuestCancelDate = ConnectAPI.DateToUTC(rewardProgress.NextQuestCancel),
			SpecialEventTimingMod = rewardProgress.EventTimingMod
		};
	}

	// Token: 0x06000937 RID: 2359 RVA: 0x0002611C File Offset: 0x0002431C
	public static int GetDeckLimit()
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			return 0;
		}
		ProfileDeckLimit profileDeckLimit = ConnectAPI.Unpack<ProfileDeckLimit>(ConnectAPI.NextUtil(), 231);
		if (profileDeckLimit == null)
		{
			return 0;
		}
		return profileDeckLimit.DeckLimit;
	}

	// Token: 0x06000938 RID: 2360 RVA: 0x00026154 File Offset: 0x00024354
	public static long GetArcaneDustBalance()
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			return 0L;
		}
		ArcaneDustBalance arcaneDustBalance = ConnectAPI.Unpack<ArcaneDustBalance>(ConnectAPI.NextUtil(), 262);
		if (arcaneDustBalance == null)
		{
			return 0L;
		}
		return arcaneDustBalance.Balance;
	}

	// Token: 0x06000939 RID: 2361 RVA: 0x00026190 File Offset: 0x00024390
	public static NetCache.NetCacheGoldBalance GetGoldBalance()
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			return new NetCache.NetCacheGoldBalance();
		}
		GoldBalance goldBalance = ConnectAPI.Unpack<GoldBalance>(ConnectAPI.NextUtil(), 278);
		if (goldBalance == null)
		{
			return null;
		}
		return new NetCache.NetCacheGoldBalance
		{
			CappedBalance = goldBalance.CappedBalance,
			BonusBalance = goldBalance.BonusBalance,
			Cap = goldBalance.Cap,
			CapWarning = goldBalance.CapWarning
		};
	}

	// Token: 0x0600093A RID: 2362 RVA: 0x00026200 File Offset: 0x00024400
	public static List<NetCache.ProfileNotice> GetProfileNotices()
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			return new List<NetCache.ProfileNotice>();
		}
		List<NetCache.ProfileNotice> list = new List<NetCache.ProfileNotice>();
		ProfileNotices profileNotices = ConnectAPI.Unpack<ProfileNotices>(ConnectAPI.NextUtil(), 212);
		List<ProfileNotice> list2 = profileNotices.List;
		foreach (ProfileNotice profileNotice in list2)
		{
			NetCache.ProfileNotice profileNotice2 = null;
			if (profileNotice.HasMedal)
			{
				NetCache.ProfileNoticeMedal profileNoticeMedal = new NetCache.ProfileNoticeMedal();
				profileNoticeMedal.StarLevel = profileNotice.Medal.StarLevel;
				profileNoticeMedal.LegendRank = ((!profileNotice.Medal.HasLegendRank) ? 0 : profileNotice.Medal.LegendRank);
				profileNoticeMedal.BestStarLevel = ((!profileNotice.Medal.HasBestStarLevel) ? 0 : profileNotice.Medal.BestStarLevel);
				profileNoticeMedal.IsWild = (profileNotice.Medal.HasMedalType_ && profileNotice.Medal.MedalType_ == 2);
				if (profileNotice.Medal.HasChest)
				{
					profileNoticeMedal.Chest = ConnectAPI.ConvertRewardChest(profileNotice.Medal.Chest);
				}
				profileNotice2 = profileNoticeMedal;
			}
			else if (profileNotice.HasRewardBooster)
			{
				profileNotice2 = new NetCache.ProfileNoticeRewardBooster
				{
					Id = profileNotice.RewardBooster.BoosterType,
					Count = profileNotice.RewardBooster.BoosterCount
				};
			}
			else if (profileNotice.HasRewardCard)
			{
				profileNotice2 = new NetCache.ProfileNoticeRewardCard
				{
					CardID = GameUtils.TranslateDbIdToCardId(profileNotice.RewardCard.Card.Asset),
					Premium = (TAG_PREMIUM)((!profileNotice.RewardCard.Card.HasPremium) ? 0 : profileNotice.RewardCard.Card.Premium),
					Quantity = ((!profileNotice.RewardCard.HasQuantity) ? 1 : profileNotice.RewardCard.Quantity)
				};
			}
			else if (profileNotice.HasPreconDeck)
			{
				profileNotice2 = new NetCache.ProfileNoticePreconDeck
				{
					DeckID = profileNotice.PreconDeck.Deck,
					HeroAsset = profileNotice.PreconDeck.Hero
				};
			}
			else if (profileNotice.HasRewardDust)
			{
				profileNotice2 = new NetCache.ProfileNoticeRewardDust
				{
					Amount = profileNotice.RewardDust.Amount
				};
			}
			else if (profileNotice.HasRewardMount)
			{
				profileNotice2 = new NetCache.ProfileNoticeRewardMount
				{
					MountID = profileNotice.RewardMount.MountId
				};
			}
			else if (profileNotice.HasRewardForge)
			{
				profileNotice2 = new NetCache.ProfileNoticeRewardForge
				{
					Quantity = profileNotice.RewardForge.Quantity
				};
			}
			else if (profileNotice.HasRewardGold)
			{
				profileNotice2 = new NetCache.ProfileNoticeRewardGold
				{
					Amount = profileNotice.RewardGold.Amount
				};
			}
			else if (profileNotice.HasPurchase)
			{
				profileNotice2 = new NetCache.ProfileNoticePurchase
				{
					ProductID = profileNotice.Purchase.ProductId,
					Data = ((!profileNotice.Purchase.HasData) ? 0L : profileNotice.Purchase.Data),
					CurrencyType = (Currency)((!profileNotice.Purchase.HasCurrency) ? 0 : profileNotice.Purchase.Currency)
				};
			}
			else if (profileNotice.HasRewardCardBack)
			{
				profileNotice2 = new NetCache.ProfileNoticeRewardCardBack
				{
					CardBackID = profileNotice.RewardCardBack.CardBack
				};
			}
			else if (profileNotice.HasBonusStars)
			{
				profileNotice2 = new NetCache.ProfileNoticeBonusStars
				{
					StarLevel = profileNotice.BonusStars.StarLevel,
					Stars = profileNotice.BonusStars.Stars
				};
			}
			else if (profileNotice.HasDcGameResult)
			{
				NetCache.ProfileNoticeDisconnectedGame profileNoticeDisconnectedGame = new NetCache.ProfileNoticeDisconnectedGame();
				if (!profileNotice.DcGameResult.HasGameType)
				{
					Debug.LogError("ConnectAPI.GetProfileNotices(): Missing GameType");
					continue;
				}
				if (!profileNotice.DcGameResult.HasMissionId)
				{
					Debug.LogError("ConnectAPI.GetProfileNotices(): Missing GameType");
					continue;
				}
				if (!profileNotice.DcGameResult.HasGameResult_)
				{
					Debug.LogError("ConnectAPI.GetProfileNotices(): Missing GameResult");
					continue;
				}
				profileNoticeDisconnectedGame.GameType = profileNotice.DcGameResult.GameType;
				profileNoticeDisconnectedGame.MissionId = profileNotice.DcGameResult.MissionId;
				profileNoticeDisconnectedGame.GameResult = profileNotice.DcGameResult.GameResult_;
				if (profileNoticeDisconnectedGame.GameResult == 2)
				{
					if (!profileNotice.DcGameResult.HasYourResult || !profileNotice.DcGameResult.HasOpponentResult)
					{
						Debug.LogError("ConnectAPI.GetProfileNotices(): Missing PlayerResult");
						continue;
					}
					profileNoticeDisconnectedGame.YourResult = profileNotice.DcGameResult.YourResult;
					profileNoticeDisconnectedGame.OpponentResult = profileNotice.DcGameResult.OpponentResult;
				}
				profileNotice2 = profileNoticeDisconnectedGame;
			}
			else if (profileNotice.HasAdventureProgress)
			{
				NetCache.ProfileNoticeAdventureProgress profileNoticeAdventureProgress = new NetCache.ProfileNoticeAdventureProgress();
				profileNoticeAdventureProgress.Wing = profileNotice.AdventureProgress.WingId;
				NetCache.ProfileNotice.NoticeOrigin origin = (NetCache.ProfileNotice.NoticeOrigin)profileNotice.Origin;
				if (origin != NetCache.ProfileNotice.NoticeOrigin.ADVENTURE_PROGRESS)
				{
					if (origin == NetCache.ProfileNotice.NoticeOrigin.ADVENTURE_FLAGS)
					{
						profileNoticeAdventureProgress.Flags = new ulong?((ulong)((!profileNotice.HasOriginData) ? 0L : profileNotice.OriginData));
					}
				}
				else
				{
					profileNoticeAdventureProgress.Progress = new int?((!profileNotice.HasOriginData) ? 0 : ((int)profileNotice.OriginData));
				}
				profileNotice2 = profileNoticeAdventureProgress;
			}
			else if (profileNotice.HasLevelUp)
			{
				profileNotice2 = new NetCache.ProfileNoticeLevelUp
				{
					HeroClass = profileNotice.LevelUp.HeroClass,
					NewLevel = profileNotice.LevelUp.NewLevel
				};
			}
			else if (profileNotice.HasAccountLicense)
			{
				profileNotice2 = new NetCache.ProfileNoticeAcccountLicense
				{
					License = profileNotice.AccountLicense.License,
					CasID = profileNotice.AccountLicense.CasId
				};
			}
			else
			{
				Debug.LogError("ConnectAPI.GetProfileNotices(): Unrecognized profile notice");
			}
			if (profileNotice2 == null)
			{
				Debug.LogError("ConnectAPI.GetProfileNotices(): Unhandled notice type! This notice will be lost!");
			}
			else
			{
				profileNotice2.NoticeID = profileNotice.Entry;
				profileNotice2.Origin = (NetCache.ProfileNotice.NoticeOrigin)profileNotice.Origin;
				profileNotice2.OriginData = ((!profileNotice.HasOriginData) ? 0L : profileNotice.OriginData);
				profileNotice2.Date = ConnectAPI.DateToUTC(profileNotice.When);
				list.Add(profileNotice2);
			}
		}
		return list;
	}

	// Token: 0x0600093B RID: 2363 RVA: 0x000268EC File Offset: 0x00024AEC
	public static NetCache.NetCacheBoosters GetBoosters()
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			return new NetCache.NetCacheBoosters();
		}
		BoosterList boosterList = ConnectAPI.Unpack<BoosterList>(ConnectAPI.NextUtil(), 224);
		if (boosterList == null)
		{
			return null;
		}
		NetCache.NetCacheBoosters netCacheBoosters = new NetCache.NetCacheBoosters();
		foreach (BoosterInfo boosterInfo in boosterList.List)
		{
			NetCache.BoosterStack boosterStack = new NetCache.BoosterStack
			{
				Id = boosterInfo.Type,
				Count = boosterInfo.Count
			};
			netCacheBoosters.BoosterStacks.Add(boosterStack);
		}
		return netCacheBoosters;
	}

	// Token: 0x0600093C RID: 2364 RVA: 0x000269A4 File Offset: 0x00024BA4
	public static NetCache.NetCacheMedalInfo GetMedalInfo()
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			return new NetCache.NetCacheMedalInfo
			{
				Standard = new MedalInfoData(),
				Wild = new MedalInfoData()
			};
		}
		MedalInfo medalInfo = ConnectAPI.Unpack<MedalInfo>(ConnectAPI.NextUtil(), 232);
		if (medalInfo == null)
		{
			return null;
		}
		NetCache.NetCacheMedalInfo netCacheMedalInfo = new NetCache.NetCacheMedalInfo();
		netCacheMedalInfo.Standard = NetCache.NetCacheMedalInfo.CloneMedalInfoData(medalInfo.Standard);
		netCacheMedalInfo.Wild = NetCache.NetCacheMedalInfo.CloneMedalInfoData(medalInfo.Wild);
		int num = (!medalInfo.Standard.HasLegendRank) ? 0 : medalInfo.Standard.LegendRank;
		Log.Bob.Print(string.Format("legend rank {0}", num), new object[0]);
		return netCacheMedalInfo;
	}

	// Token: 0x0600093D RID: 2365 RVA: 0x00026A5C File Offset: 0x00024C5C
	public static NetCache.NetCacheFeatures GetFeatures()
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			return new NetCache.NetCacheFeatures();
		}
		GuardianVars guardianVars = ConnectAPI.Unpack<GuardianVars>(ConnectAPI.NextUtil(), 264);
		if (guardianVars == null)
		{
			return null;
		}
		return new NetCache.NetCacheFeatures
		{
			Games = 
			{
				Tournament = (!guardianVars.HasTourney || guardianVars.Tourney),
				Practice = (!guardianVars.HasPractice || guardianVars.Practice),
				Casual = (!guardianVars.HasCasual || guardianVars.Casual),
				Forge = (!guardianVars.HasForge || guardianVars.Forge),
				Friendly = (!guardianVars.HasFriendly || guardianVars.Friendly),
				TavernBrawl = (!guardianVars.HasTavernBrawl || guardianVars.TavernBrawl),
				ShowUserUI = ((!guardianVars.HasShowUserUI) ? 0 : guardianVars.ShowUserUI)
			},
			Collection = 
			{
				Manager = (!guardianVars.HasManager || guardianVars.Manager),
				Crafting = (!guardianVars.HasCrafting || guardianVars.Crafting)
			},
			Store = 
			{
				Store = (!guardianVars.HasStore || guardianVars.Store),
				BattlePay = (!guardianVars.HasBattlePay || guardianVars.BattlePay),
				BuyWithGold = (!guardianVars.HasBuyWithGold || guardianVars.BuyWithGold)
			},
			Heroes = 
			{
				Hunter = (!guardianVars.HasHunter || guardianVars.Hunter),
				Mage = (!guardianVars.HasMage || guardianVars.Mage),
				Paladin = (!guardianVars.HasPaladin || guardianVars.Paladin),
				Priest = (!guardianVars.HasPriest || guardianVars.Priest),
				Rogue = (!guardianVars.HasRogue || guardianVars.Rogue),
				Shaman = (!guardianVars.HasShaman || guardianVars.Shaman),
				Warlock = (!guardianVars.HasWarlock || guardianVars.Warlock),
				Warrior = (!guardianVars.HasWarrior || guardianVars.Warrior)
			},
			Misc = 
			{
				ClientOptionsUpdateIntervalSeconds = ((!guardianVars.HasClientOptionsUpdateIntervalSeconds) ? 0 : guardianVars.ClientOptionsUpdateIntervalSeconds)
			}
		};
	}

	// Token: 0x0600093E RID: 2366 RVA: 0x00026D2C File Offset: 0x00024F2C
	public static void ReadClientOptions(Map<ServerOption, NetCache.ClientOptionBase> clientState, Map<ServerOption, NetCache.ClientOptionBase> serverState)
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			return;
		}
		ClientOptions clientOptions = ConnectAPI.Unpack<ClientOptions>(ConnectAPI.NextUtil(), 241);
		if (clientOptions == null)
		{
			return;
		}
		if (clientOptions.HasFailed && clientOptions.Failed)
		{
			Debug.LogError("ReadClientOptions: packet.Failed=true. Unable to retrieve client options from UtilServer.");
			Network.Get().ShowConnectionFailureError("GLOBAL_ERROR_NETWORK_GENERIC");
			return;
		}
		foreach (ClientOption clientOption in clientOptions.Options)
		{
			ServerOption index = (ServerOption)clientOption.Index;
			if (clientOption.HasAsBool)
			{
				clientState[index] = new NetCache.ClientOptionBool(clientOption.AsBool);
				serverState[index] = new NetCache.ClientOptionBool(clientOption.AsBool);
			}
			else if (clientOption.HasAsInt32)
			{
				clientState[index] = new NetCache.ClientOptionInt(clientOption.AsInt32);
				serverState[index] = new NetCache.ClientOptionInt(clientOption.AsInt32);
			}
			else if (clientOption.HasAsInt64)
			{
				clientState[index] = new NetCache.ClientOptionLong(clientOption.AsInt64);
				serverState[index] = new NetCache.ClientOptionInt(clientOption.AsInt32);
			}
			else if (clientOption.HasAsFloat)
			{
				clientState[index] = new NetCache.ClientOptionFloat(clientOption.AsFloat);
				serverState[index] = new NetCache.ClientOptionInt(clientOption.AsInt32);
			}
			else if (clientOption.HasAsUint64)
			{
				clientState[index] = new NetCache.ClientOptionULong(clientOption.AsUint64);
				serverState[index] = new NetCache.ClientOptionInt(clientOption.AsInt32);
			}
		}
	}

	// Token: 0x0600093F RID: 2367 RVA: 0x00026EE4 File Offset: 0x000250E4
	public static NotSoMassiveLoginReply GetNotSoMassiveLoginReply()
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			return new NotSoMassiveLoginReply();
		}
		NotSoMassiveLoginReply notSoMassiveLoginReply = ConnectAPI.Unpack<NotSoMassiveLoginReply>(ConnectAPI.NextUtil(), 300);
		if (notSoMassiveLoginReply == null)
		{
			return null;
		}
		return notSoMassiveLoginReply;
	}

	// Token: 0x06000940 RID: 2368 RVA: 0x00026F1C File Offset: 0x0002511C
	public static TavernBrawlInfo GetTavernBrawlInfoPacket()
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			return new TavernBrawlInfo();
		}
		TavernBrawlInfo tavernBrawlInfo = ConnectAPI.Unpack<TavernBrawlInfo>(ConnectAPI.NextUtil(), 316);
		if (tavernBrawlInfo == null)
		{
			return null;
		}
		return tavernBrawlInfo;
	}

	// Token: 0x06000941 RID: 2369 RVA: 0x00026F54 File Offset: 0x00025154
	public static TavernBrawlPlayerRecord GetTavernBrawlRecordPacket()
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			return new TavernBrawlPlayerRecord();
		}
		TavernBrawlPlayerRecordResponse tavernBrawlPlayerRecordResponse = ConnectAPI.Unpack<TavernBrawlPlayerRecordResponse>(ConnectAPI.NextUtil(), 317);
		if (tavernBrawlPlayerRecordResponse == null)
		{
			return null;
		}
		return tavernBrawlPlayerRecordResponse.Record;
	}

	// Token: 0x06000942 RID: 2370 RVA: 0x00026F90 File Offset: 0x00025190
	public static FavoriteHeroesResponse GetFavoriteHeroesResponse()
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			return new FavoriteHeroesResponse();
		}
		FavoriteHeroesResponse favoriteHeroesResponse = ConnectAPI.Unpack<FavoriteHeroesResponse>(ConnectAPI.NextUtil(), 318);
		if (favoriteHeroesResponse == null)
		{
			return null;
		}
		return favoriteHeroesResponse;
	}

	// Token: 0x06000943 RID: 2371 RVA: 0x00026FC8 File Offset: 0x000251C8
	public static AccountLicensesInfoResponse GetAccountLicensesInfoResponse()
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			return new AccountLicensesInfoResponse();
		}
		AccountLicensesInfoResponse accountLicensesInfoResponse = ConnectAPI.Unpack<AccountLicensesInfoResponse>(ConnectAPI.NextUtil(), 325);
		if (accountLicensesInfoResponse == null)
		{
			return null;
		}
		return accountLicensesInfoResponse;
	}

	// Token: 0x06000944 RID: 2372 RVA: 0x00027000 File Offset: 0x00025200
	public static UpdateLoginComplete GetUpdateLoginComplete()
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			return new UpdateLoginComplete();
		}
		return ConnectAPI.Unpack<UpdateLoginComplete>(ConnectAPI.NextUtil(), 307);
	}

	// Token: 0x06000945 RID: 2373 RVA: 0x00027030 File Offset: 0x00025230
	public static NetCache.NetCacheHeroLevels GetAllHeroXP()
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			return new NetCache.NetCacheHeroLevels();
		}
		HeroXP heroXP = ConnectAPI.Unpack<HeroXP>(ConnectAPI.NextUtil(), 283);
		if (heroXP == null)
		{
			return null;
		}
		NetCache.NetCacheHeroLevels netCacheHeroLevels = new NetCache.NetCacheHeroLevels();
		foreach (HeroXPInfo heroXPInfo in heroXP.XpInfos)
		{
			NetCache.HeroLevel heroLevel = new NetCache.HeroLevel();
			heroLevel.Class = (TAG_CLASS)heroXPInfo.ClassId;
			heroLevel.CurrentLevel.Level = heroXPInfo.Level;
			heroLevel.CurrentLevel.XP = heroXPInfo.CurrXp;
			heroLevel.CurrentLevel.MaxXP = heroXPInfo.MaxXp;
			netCacheHeroLevels.Levels.Add(heroLevel);
			if (heroXPInfo.HasNextReward)
			{
				heroLevel.NextReward = new NetCache.HeroLevel.NextLevelReward();
				heroLevel.NextReward.Level = heroXPInfo.NextReward.Level;
				if (heroXPInfo.NextReward.HasRewardBooster)
				{
					heroLevel.NextReward.Reward = new BoosterPackRewardData(heroXPInfo.NextReward.RewardBooster.BoosterType, heroXPInfo.NextReward.RewardBooster.BoosterCount);
				}
				else if (heroXPInfo.NextReward.HasRewardCard)
				{
					string text = GameUtils.TranslateDbIdToCardId(heroXPInfo.NextReward.RewardCard.Card.Asset);
					TAG_PREMIUM tag_PREMIUM = (TAG_PREMIUM)((!heroXPInfo.NextReward.RewardCard.Card.HasPremium) ? 0 : heroXPInfo.NextReward.RewardCard.Card.Premium);
					EntityDef entityDef = DefLoader.Get().GetEntityDef(text);
					int count;
					if (entityDef.IsHero())
					{
						count = 1;
					}
					else if (tag_PREMIUM == TAG_PREMIUM.GOLDEN)
					{
						count = 1;
					}
					else
					{
						count = ((!entityDef.IsElite()) ? 2 : 1);
					}
					heroLevel.NextReward.Reward = new CardRewardData(text, tag_PREMIUM, count);
				}
				else if (heroXPInfo.NextReward.HasRewardDust)
				{
					heroLevel.NextReward.Reward = new ArcaneDustRewardData(heroXPInfo.NextReward.RewardDust.Amount);
				}
				else if (heroXPInfo.NextReward.HasRewardGold)
				{
					heroLevel.NextReward.Reward = new GoldRewardData((long)heroXPInfo.NextReward.RewardGold.Amount);
				}
				else if (heroXPInfo.NextReward.HasRewardMount)
				{
					heroLevel.NextReward.Reward = new MountRewardData((MountRewardData.MountType)heroXPInfo.NextReward.RewardMount.MountId);
				}
				else if (heroXPInfo.NextReward.HasRewardForge)
				{
					heroLevel.NextReward.Reward = new ForgeTicketRewardData(heroXPInfo.NextReward.RewardForge.Quantity);
				}
				else
				{
					Debug.LogWarning(string.Format("ConnectAPI.GetAllHeroXP(): next reward for hero {0} is at level {1} but has no recognized reward type in packet", heroLevel.Class, heroLevel.NextReward.Level));
				}
			}
		}
		return netCacheHeroLevels;
	}

	// Token: 0x06000946 RID: 2374 RVA: 0x00027360 File Offset: 0x00025560
	public static Network.AchieveList GetAchieves()
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			return new Network.AchieveList();
		}
		Achieves achieves = ConnectAPI.Unpack<Achieves>(ConnectAPI.NextUtil(), 252);
		if (achieves == null)
		{
			return null;
		}
		Network.AchieveList achieveList = new Network.AchieveList();
		foreach (Achieve achieve in achieves.List)
		{
			Network.AchieveList.Achieve achieve2 = new Network.AchieveList.Achieve();
			achieve2.ID = achieve.Id;
			achieve2.Progress = achieve.Progress;
			achieve2.AckProgress = achieve.AckProgress;
			achieve2.CompletionCount = ((!achieve.HasCompletionCount) ? 0 : achieve.CompletionCount);
			achieve2.Active = (achieve.HasActive && achieve.Active);
			achieve2.DateGiven = ((!achieve.HasDateGiven) ? 0L : ConnectAPI.DateToUTC(achieve.DateGiven));
			achieve2.DateCompleted = ((!achieve.HasDateCompleted) ? 0L : ConnectAPI.DateToUTC(achieve.DateCompleted));
			achieve2.CanAck = (!achieve.HasDoNotAck || !achieve.DoNotAck);
			achieveList.Achieves.Add(achieve2);
		}
		return achieveList;
	}

	// Token: 0x06000947 RID: 2375 RVA: 0x000274C8 File Offset: 0x000256C8
	public static ValidateAchieveResponse GetValidatedAchieve()
	{
		return ConnectAPI.Unpack<ValidateAchieveResponse>(ConnectAPI.NextUtil(), 285);
	}

	// Token: 0x06000948 RID: 2376 RVA: 0x000274E8 File Offset: 0x000256E8
	public static Network.CanceledQuest GetCanceledQuest()
	{
		CancelQuestResponse cancelQuestResponse = ConnectAPI.Unpack<CancelQuestResponse>(ConnectAPI.NextUtil(), 282);
		if (cancelQuestResponse == null)
		{
			return null;
		}
		return new Network.CanceledQuest
		{
			AchieveID = cancelQuestResponse.QuestId,
			Canceled = cancelQuestResponse.Success,
			NextQuestCancelDate = ((!cancelQuestResponse.HasNextQuestCancel) ? 0L : ConnectAPI.DateToUTC(cancelQuestResponse.NextQuestCancel))
		};
	}

	// Token: 0x06000949 RID: 2377 RVA: 0x00027550 File Offset: 0x00025750
	public static List<Network.AdventureProgress> GetAdventureProgressResponse()
	{
		AdventureProgressResponse adventureProgressResponse = ConnectAPI.Unpack<AdventureProgressResponse>(ConnectAPI.NextUtil(), 306);
		if (adventureProgressResponse == null)
		{
			return null;
		}
		List<Network.AdventureProgress> list = new List<Network.AdventureProgress>();
		foreach (AdventureProgress adventureProgress in adventureProgressResponse.List)
		{
			list.Add(new Network.AdventureProgress
			{
				Wing = adventureProgress.WingId,
				Progress = adventureProgress.Progress,
				Ack = adventureProgress.Ack,
				Flags = adventureProgress.Flags_
			});
		}
		return list;
	}

	// Token: 0x0600094A RID: 2378 RVA: 0x00027608 File Offset: 0x00025808
	public static Network.TriggeredEvent GetTriggerEventResponse()
	{
		TriggerEventResponse triggerEventResponse = ConnectAPI.Unpack<TriggerEventResponse>(ConnectAPI.NextUtil(), 299);
		if (triggerEventResponse == null)
		{
			return null;
		}
		return new Network.TriggeredEvent
		{
			EventID = triggerEventResponse.EventId,
			Success = triggerEventResponse.Success
		};
	}

	// Token: 0x0600094B RID: 2379 RVA: 0x0002764C File Offset: 0x0002584C
	public static CardValues GetCardValues()
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			return null;
		}
		return ConnectAPI.Unpack<CardValues>(ConnectAPI.NextUtil(), 260);
	}

	// Token: 0x0600094C RID: 2380 RVA: 0x00027678 File Offset: 0x00025878
	public static Deadend GetDeadendGame()
	{
		return ConnectAPI.Unpack<Deadend>(ConnectAPI.NextGame(), 169);
	}

	// Token: 0x0600094D RID: 2381 RVA: 0x00027698 File Offset: 0x00025898
	public static DeadendUtil GetDeadendUtil()
	{
		return ConnectAPI.Unpack<DeadendUtil>(ConnectAPI.NextUtil(), 167);
	}

	// Token: 0x0600094E RID: 2382 RVA: 0x000276B8 File Offset: 0x000258B8
	public static CheckGameLicensesResponse GetCheckGameLicensesResponse()
	{
		return ConnectAPI.Unpack<CheckGameLicensesResponse>(ConnectAPI.NextUtil(), 331);
	}

	// Token: 0x0600094F RID: 2383 RVA: 0x000276D8 File Offset: 0x000258D8
	public static CheckAccountLicensesResponse GetCheckAccountLicensesResponse()
	{
		return ConnectAPI.Unpack<CheckAccountLicensesResponse>(ConnectAPI.NextUtil(), 330);
	}

	// Token: 0x06000950 RID: 2384 RVA: 0x000276F8 File Offset: 0x000258F8
	public static SetProgressResponse GetSetProgressResponse()
	{
		return ConnectAPI.Unpack<SetProgressResponse>(ConnectAPI.NextUtil(), 296);
	}

	// Token: 0x06000951 RID: 2385 RVA: 0x00027718 File Offset: 0x00025918
	public static int GetAssetsVersion()
	{
		AssetsVersionResponse assetsVersionResponse = ConnectAPI.Unpack<AssetsVersionResponse>(ConnectAPI.NextUtil(), 304);
		if (assetsVersionResponse == null)
		{
			return 0;
		}
		return assetsVersionResponse.Version;
	}

	// Token: 0x06000952 RID: 2386 RVA: 0x00027748 File Offset: 0x00025948
	public static void RequestPurchaseMethod(string productID, int quantity, int currency)
	{
		ConnectAPI.UtilOutbound(250, 1, new GetPurchaseMethod
		{
			ProductId = productID,
			Quantity = quantity,
			Currency = currency,
			DeviceId = SystemInfo.deviceUniqueIdentifier,
			Platform = ConnectAPI.GetPlatformBuilder()
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000953 RID: 2387 RVA: 0x00027794 File Offset: 0x00025994
	public static void ConfirmPurchase()
	{
		DoPurchase body = new DoPurchase();
		ConnectAPI.UtilOutbound(273, 1, body, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000954 RID: 2388 RVA: 0x000277B8 File Offset: 0x000259B8
	public static void AbortBlizzardPurchase(bool isAutoCanceled, CancelPurchase.CancelReason? reason, string error)
	{
		CancelPurchase cancelPurchase = new CancelPurchase();
		cancelPurchase.IsAutoCancel = isAutoCanceled;
		cancelPurchase.DeviceId = SystemInfo.deviceUniqueIdentifier;
		if (reason != null)
		{
			cancelPurchase.Reason = reason.Value;
		}
		cancelPurchase.ErrorMessage = error;
		ConnectAPI.UtilOutbound(274, 1, cancelPurchase, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000955 RID: 2389 RVA: 0x0002780C File Offset: 0x00025A0C
	public static void AbortThirdPartyPurchase(CancelPurchase.CancelReason reason, string error)
	{
		ConnectAPI.UtilOutbound(274, 1, new CancelPurchase
		{
			IsAutoCancel = false,
			Reason = reason,
			DeviceId = SystemInfo.deviceUniqueIdentifier,
			ErrorMessage = error
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000956 RID: 2390 RVA: 0x00027850 File Offset: 0x00025A50
	public static void RequestBattlePayConfig()
	{
		GetBattlePayConfig body = new GetBattlePayConfig();
		ConnectAPI.UtilOutbound(237, 1, body, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000957 RID: 2391 RVA: 0x00027874 File Offset: 0x00025A74
	public static void RequestBattlePayStatus()
	{
		GetBattlePayStatus body = new GetBattlePayStatus();
		ConnectAPI.UtilOutbound(255, 1, body, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000958 RID: 2392 RVA: 0x00027898 File Offset: 0x00025A98
	public static void MassDisenchant()
	{
		MassDisenchantRequest body = new MassDisenchantRequest();
		ConnectAPI.UtilOutbound(268, 0, body, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000959 RID: 2393 RVA: 0x000278BC File Offset: 0x00025ABC
	public static void DraftBegin()
	{
		DraftBegin body = new DraftBegin();
		ConnectAPI.UtilOutbound(235, 0, body, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x0600095A RID: 2394 RVA: 0x000278E0 File Offset: 0x00025AE0
	public static void DraftRetire(long deckID, int slot)
	{
		ConnectAPI.UtilOutbound(242, 0, new DraftRetire
		{
			DeckId = deckID,
			Slot = slot
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x0600095B RID: 2395 RVA: 0x00027910 File Offset: 0x00025B10
	public static void DraftAckRewards(long deckID, int slot)
	{
		ConnectAPI.UtilOutbound(287, 0, new DraftAckRewards
		{
			DeckId = deckID,
			Slot = slot
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x0600095C RID: 2396 RVA: 0x00027940 File Offset: 0x00025B40
	public static void DraftGetPicksAndContents()
	{
		DraftGetPicksAndContents body = new DraftGetPicksAndContents();
		ConnectAPI.UtilOutbound(244, 0, body, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x0600095D RID: 2397 RVA: 0x00027964 File Offset: 0x00025B64
	public static void DraftMakePick(long deckID, int slot, int index)
	{
		ConnectAPI.UtilOutbound(245, 0, new DraftMakePick
		{
			DeckId = deckID,
			Slot = slot,
			Index = index
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x0600095E RID: 2398 RVA: 0x0002799C File Offset: 0x00025B9C
	public static void RequestNetCacheObject(GetAccountInfo.Request request)
	{
		ConnectAPI.UtilOutbound(201, 0, new GetAccountInfo
		{
			Request_ = request
		}, ClientRequestManager.RequestPhase.RUNNING, request);
	}

	// Token: 0x0600095F RID: 2399 RVA: 0x000279C4 File Offset: 0x00025BC4
	public static void RequestNetCacheObjectList(List<GetAccountInfo.Request> rl)
	{
		GenericRequestList genericRequestList = new GenericRequestList();
		foreach (GetAccountInfo.Request requestSubId in rl)
		{
			GenericRequest genericRequest = new GenericRequest();
			genericRequest.RequestId = 201;
			genericRequest.RequestSubId = requestSubId;
			genericRequestList.Requests.Add(genericRequest);
		}
		ConnectAPI.UtilOutbound(327, 0, genericRequestList, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000960 RID: 2400 RVA: 0x00027A4C File Offset: 0x00025C4C
	public static void DoLoginUpdate()
	{
		UpdateLogin updateLogin = new UpdateLogin();
		string str = Vars.Key("Application.Referral").GetStr("none");
		if (str != "none")
		{
			updateLogin.Referral = str;
		}
		else if (PlatformSettings.OS == OSCategory.PC || PlatformSettings.OS == OSCategory.Mac)
		{
			updateLogin.Referral = "Battle.net";
		}
		else if (PlatformSettings.OS == OSCategory.iOS)
		{
			updateLogin.Referral = "AppleAppStore";
		}
		else if (PlatformSettings.OS == OSCategory.Android)
		{
			AndroidStore androidStore = ApplicationMgr.GetAndroidStore();
			if (androidStore == AndroidStore.GOOGLE)
			{
				updateLogin.Referral = "GooglePlay";
			}
			else if (androidStore == AndroidStore.AMAZON)
			{
				updateLogin.Referral = "AmazonAppStore";
			}
			else if (androidStore == AndroidStore.BLIZZARD)
			{
			}
		}
		ConnectAPI.UtilOutbound(205, 0, updateLogin, ClientRequestManager.RequestPhase.STARTUP, 0);
	}

	// Token: 0x06000961 RID: 2401 RVA: 0x00027B28 File Offset: 0x00025D28
	public static void RequestDeckContents(params long[] deckIds)
	{
		GetDeckContents getDeckContents = new GetDeckContents();
		getDeckContents.DeckId.AddRange(deckIds);
		ConnectAPI.UtilOutbound(214, 0, getDeckContents, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000962 RID: 2402 RVA: 0x00027B58 File Offset: 0x00025D58
	public static void CreateDeck(DeckType deckType, string name, int hero, TAG_PREMIUM heroPremium, bool isWild)
	{
		ConnectAPI.UtilOutbound(209, 0, new CreateDeck
		{
			Name = name,
			Hero = hero,
			HeroPremium = (int)heroPremium,
			DeckType = deckType,
			TaggedStandard = !isWild
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000963 RID: 2403 RVA: 0x00027BA0 File Offset: 0x00025DA0
	public static void DeleteDeck(long deckID)
	{
		ConnectAPI.UtilOutbound(210, 0, new DeleteDeck
		{
			Deck = deckID
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000964 RID: 2404 RVA: 0x00027BC8 File Offset: 0x00025DC8
	public static void RenameDeck(long deckID, string name)
	{
		ConnectAPI.UtilOutbound(211, 0, new RenameDeck
		{
			Deck = deckID,
			Name = name
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000965 RID: 2405 RVA: 0x00027BF8 File Offset: 0x00025DF8
	public static void SendDeckTemplateSource(long deckID, int templateID)
	{
		ConnectAPI.UtilOutbound(332, 0, new DeckSetTemplateSource
		{
			Deck = deckID,
			TemplateId = templateID
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000966 RID: 2406 RVA: 0x00027C28 File Offset: 0x00025E28
	public static void SendDeckData(long deck, List<Network.CardUserData> cards, int newHeroAssetID, TAG_PREMIUM newHeroCardPremium, int newCardBackID, bool isWild)
	{
		DeckSetData deckSetData = new DeckSetData();
		deckSetData.Deck = deck;
		deckSetData.TaggedStandard = !isWild;
		foreach (Network.CardUserData cardUserData in cards)
		{
			DeckCardData deckCardData = new DeckCardData();
			CardDef cardDef = new CardDef();
			cardDef.Asset = cardUserData.DbId;
			if (cardUserData.Premium != TAG_PREMIUM.NORMAL)
			{
				cardDef.Premium = (int)cardUserData.Premium;
			}
			deckCardData.Def = cardDef;
			deckCardData.Qty = cardUserData.Count;
			deckSetData.Cards.Add(deckCardData);
		}
		if (ConnectAPI.SEND_DECK_DATA_NO_HERO_ASSET_CHANGE != newHeroAssetID)
		{
			deckSetData.Hero = new CardDef
			{
				Asset = newHeroAssetID,
				Premium = (int)newHeroCardPremium
			};
		}
		if (ConnectAPI.SEND_DECK_DATA_NO_CARD_BACK_CHANGE != newCardBackID)
		{
			deckSetData.CardBack = newCardBackID;
		}
		ConnectAPI.UtilOutbound(222, 0, deckSetData, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000967 RID: 2407 RVA: 0x00027D2C File Offset: 0x00025F2C
	public static void AckNotice(long ID)
	{
		ConnectAPI.UtilOutbound(213, 0, new AckNotice
		{
			Entry = ID
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000968 RID: 2408 RVA: 0x00027D54 File Offset: 0x00025F54
	public static void OpenBooster(int id)
	{
		ConnectAPI.UtilOutbound(225, 0, new OpenBooster
		{
			BoosterType = id
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000969 RID: 2409 RVA: 0x00027D7C File Offset: 0x00025F7C
	public static void SetProgress(long value)
	{
		ConnectAPI.UtilOutbound(230, 0, new SetProgress
		{
			Value = value
		}, ClientRequestManager.RequestPhase.STARTUP, 0);
	}

	// Token: 0x0600096A RID: 2410 RVA: 0x00027DA4 File Offset: 0x00025FA4
	public static void TriggerLaunchEvent(ulong lastPlayedBnetHi, ulong lastPlayedBnetLo, ulong lastPlayedStartTime, ulong otherPlayerBnetHi, ulong otherPlayerBnetLo, ulong otherPlayerStartTime)
	{
		NearbyPlayer nearbyPlayer = new NearbyPlayer();
		nearbyPlayer.BnetIdHi = lastPlayedBnetHi;
		nearbyPlayer.BnetIdLo = lastPlayedBnetLo;
		nearbyPlayer.SessionStartTime = lastPlayedStartTime;
		NearbyPlayer nearbyPlayer2 = new NearbyPlayer();
		nearbyPlayer2.BnetIdHi = otherPlayerBnetHi;
		nearbyPlayer2.BnetIdLo = otherPlayerBnetLo;
		nearbyPlayer2.SessionStartTime = otherPlayerStartTime;
		ConnectAPI.UtilOutbound(298, 0, new TriggerLaunchDayEvent
		{
			LastPlayed = nearbyPlayer,
			OtherPlayer = nearbyPlayer2
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x0600096B RID: 2411 RVA: 0x00027E0C File Offset: 0x0002600C
	public static void RequestAssetsVersion()
	{
		GetAssetsVersion body = new GetAssetsVersion();
		ConnectAPI.UtilOutbound(303, 0, body, ClientRequestManager.RequestPhase.STARTUP, 0);
	}

	// Token: 0x0600096C RID: 2412 RVA: 0x00027E2D File Offset: 0x0002602D
	public static void OnLoginCompleted()
	{
		ConnectAPI.s_clientRequestManager.NotifyLoginSequenceCompleted();
	}

	// Token: 0x0600096D RID: 2413 RVA: 0x00027E3C File Offset: 0x0002603C
	public static void AckCardSeen(int assetID, int premium)
	{
		CardDef cardDef = new CardDef();
		cardDef.Asset = assetID;
		if (premium != 0)
		{
			cardDef.Premium = premium;
		}
		ConnectAPI.m_ackCardSeenPacket.CardDefs.Add(cardDef);
		if (ConnectAPI.m_ackCardSeenPacket.CardDefs.Count > 10)
		{
			ConnectAPI.SendAckCardsSeen();
		}
	}

	// Token: 0x0600096E RID: 2414 RVA: 0x00027E90 File Offset: 0x00026090
	public static void AckWingProgress(int wing, int ack)
	{
		ConnectAPI.UtilOutbound(308, 0, new AckWingProgress
		{
			Wing = wing,
			Ack = ack
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x0600096F RID: 2415 RVA: 0x00027EC0 File Offset: 0x000260C0
	public static void AcknowledgeBanner(int banner)
	{
		ConnectAPI.UtilOutbound(309, 0, new AcknowledgeBanner
		{
			Banner = banner
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000970 RID: 2416 RVA: 0x00027EE8 File Offset: 0x000260E8
	public static void SetAdventureOptions(int id, ulong options)
	{
		ConnectAPI.UtilOutbound(310, 0, new SetAdventureOptions
		{
			AdventureOptions = new AdventureOptions
			{
				AdventureId = id,
				Options = options
			}
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000971 RID: 2417 RVA: 0x00027F24 File Offset: 0x00026124
	public static void SendAckCardsSeen()
	{
		if (ConnectAPI.m_ackCardSeenPacket.CardDefs.Count > 0)
		{
			ConnectAPI.UtilOutbound(223, 0, ConnectAPI.m_ackCardSeenPacket, ClientRequestManager.RequestPhase.RUNNING, 0);
			ConnectAPI.m_ackCardSeenPacket.CardDefs.Clear();
		}
	}

	// Token: 0x06000972 RID: 2418 RVA: 0x00027F68 File Offset: 0x00026168
	public static void GetAllClientOptions()
	{
		GetOptions body = new GetOptions();
		ConnectAPI.UtilOutbound(240, 0, body, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000973 RID: 2419 RVA: 0x00027F89 File Offset: 0x00026189
	public static void SetClientOptions(SetOptions packet)
	{
		ConnectAPI.UtilOutbound(239, 0, packet, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000974 RID: 2420 RVA: 0x00027F9C File Offset: 0x0002619C
	public static void RequestAchieves(bool activeOrNewCompleteOnly)
	{
		GetAchieves getAchieves = new GetAchieves();
		if (activeOrNewCompleteOnly)
		{
			getAchieves.OnlyActiveOrNewComplete = activeOrNewCompleteOnly;
		}
		ConnectAPI.UtilOutbound(253, 0, getAchieves, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000975 RID: 2421 RVA: 0x00027FD0 File Offset: 0x000261D0
	public static void ValidateAchieve(int achieveID)
	{
		ConnectAPI.UtilOutbound(284, 0, new ValidateAchieve
		{
			Achieve = achieveID
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000976 RID: 2422 RVA: 0x00027FF8 File Offset: 0x000261F8
	public static void RequestCancelQuest(int achieveID)
	{
		ConnectAPI.UtilOutbound(281, 0, new CancelQuest
		{
			QuestId = achieveID
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000977 RID: 2423 RVA: 0x00028020 File Offset: 0x00026220
	public static void AckAchieveProgress(int ID, int ackProgress)
	{
		ConnectAPI.UtilOutbound(243, 0, new AckAchieveProgress
		{
			Id = ID,
			AckProgress = ackProgress
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000978 RID: 2424 RVA: 0x00028050 File Offset: 0x00026250
	public static void CheckAccountLicenseAchieve(int achieveID)
	{
		ConnectAPI.UtilOutbound(297, 1, new CheckAccountLicenseAchieve
		{
			Achieve = achieveID
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000979 RID: 2425 RVA: 0x00028078 File Offset: 0x00026278
	public static Network.AccountLicenseAchieveResponse GetAccountLicenseAchieveResponse()
	{
		AccountLicenseAchieveResponse accountLicenseAchieveResponse = ConnectAPI.Unpack<AccountLicenseAchieveResponse>(ConnectAPI.NextUtil(), 311);
		if (accountLicenseAchieveResponse == null)
		{
			return null;
		}
		return new Network.AccountLicenseAchieveResponse
		{
			Achieve = accountLicenseAchieveResponse.Achieve,
			Result = accountLicenseAchieveResponse.Result_
		};
	}

	// Token: 0x0600097A RID: 2426 RVA: 0x000280BC File Offset: 0x000262BC
	public static void RequestAdventureProgress()
	{
		GetAdventureProgress body = new GetAdventureProgress();
		ConnectAPI.UtilOutbound(305, 0, body, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x0600097B RID: 2427 RVA: 0x000280E0 File Offset: 0x000262E0
	public static void BuyCard(int id, int premium, int count, int unitBuyPrice)
	{
		BuySellCard buySellCard = new BuySellCard();
		CardDef cardDef = new CardDef();
		cardDef.Asset = id;
		if (premium != 0)
		{
			cardDef.Premium = premium;
		}
		buySellCard.Def = cardDef;
		buySellCard.Buying = true;
		if (count != 1)
		{
			buySellCard.Count = count;
		}
		buySellCard.UnitBuyPrice = unitBuyPrice;
		ConnectAPI.UtilOutbound(257, 0, buySellCard, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x0600097C RID: 2428 RVA: 0x00028140 File Offset: 0x00026340
	public static void SellCard(int id, int premium, int count, int unitSellPrice)
	{
		BuySellCard buySellCard = new BuySellCard();
		CardDef cardDef = new CardDef();
		cardDef.Asset = id;
		if (premium != 0)
		{
			cardDef.Premium = premium;
		}
		buySellCard.Def = cardDef;
		buySellCard.Buying = false;
		if (count != 1)
		{
			buySellCard.Count = count;
		}
		buySellCard.UnitSellPrice = unitSellPrice;
		ConnectAPI.UtilOutbound(257, 0, buySellCard, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x0600097D RID: 2429 RVA: 0x000281A0 File Offset: 0x000263A0
	public static void RequestAccountLicenses()
	{
		CheckAccountLicenses body = new CheckAccountLicenses();
		ConnectAPI.UtilOutbound(267, 0, body, ClientRequestManager.RequestPhase.STARTUP, 0);
	}

	// Token: 0x0600097E RID: 2430 RVA: 0x000281C4 File Offset: 0x000263C4
	public static void RequestGameLicenses()
	{
		CheckGameLicenses body = new CheckGameLicenses();
		ConnectAPI.UtilOutbound(276, 1, body, ClientRequestManager.RequestPhase.STARTUP, 0);
	}

	// Token: 0x0600097F RID: 2431 RVA: 0x000281E8 File Offset: 0x000263E8
	public static void SetDefaultCardBack(int cardBack)
	{
		ConnectAPI.UtilOutbound(291, 0, new SetCardBack
		{
			CardBack = cardBack
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000980 RID: 2432 RVA: 0x00028210 File Offset: 0x00026410
	public static void SetDeckCardBack(int cardBack, long deck)
	{
		ConnectAPI.UtilOutbound(291, 0, new SetCardBack
		{
			CardBack = cardBack,
			DeckId = deck
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000981 RID: 2433 RVA: 0x00028240 File Offset: 0x00026440
	public static void SendAssetRequest(int clientToken, AssetType assetType, int assetId)
	{
		GetAssetRequest getAssetRequest = new GetAssetRequest();
		getAssetRequest.ClientToken = clientToken;
		AssetKey assetKey = new AssetKey();
		assetKey.Type = assetType;
		assetKey.AssetId = assetId;
		getAssetRequest.Requests.Add(assetKey);
		ConnectAPI.UtilOutbound(321, 0, getAssetRequest, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000982 RID: 2434 RVA: 0x00028288 File Offset: 0x00026488
	public static void SendAssetRequest(int clientToken, List<AssetKey> requestKeys)
	{
		if (requestKeys == null || requestKeys.Count == 0)
		{
			return;
		}
		ConnectAPI.UtilOutbound(321, 0, new GetAssetRequest
		{
			ClientToken = clientToken,
			Requests = requestKeys
		}, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000983 RID: 2435 RVA: 0x000282CC File Offset: 0x000264CC
	public static GetAssetResponse GetAssetResponse()
	{
		return ConnectAPI.Unpack<GetAssetResponse>(ConnectAPI.NextUtil(), 322);
	}

	// Token: 0x06000984 RID: 2436 RVA: 0x000282EA File Offset: 0x000264EA
	public static void SendUnsubcribeRequest(Unsubscribe packet, int systemChannel)
	{
		ConnectAPI.UtilOutbound(329, systemChannel, packet, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000985 RID: 2437 RVA: 0x000282FA File Offset: 0x000264FA
	public static void SendDebugCommandRequest(DebugCommandRequest packet)
	{
		ConnectAPI.UtilOutbound(322, 0, packet, ClientRequestManager.RequestPhase.RUNNING, 0);
	}

	// Token: 0x06000986 RID: 2438 RVA: 0x0002830C File Offset: 0x0002650C
	public static DebugCommandResponse GetDebugCommandResponse()
	{
		return ConnectAPI.Unpack<DebugCommandResponse>(ConnectAPI.NextUtil(), 324);
	}

	// Token: 0x06000987 RID: 2439 RVA: 0x0002832C File Offset: 0x0002652C
	public static string GetDebugConsoleCommand()
	{
		DebugConsoleCommand debugConsoleCommand = ConnectAPI.Unpack<DebugConsoleCommand>(ConnectAPI.NextDebug(), 123);
		if (debugConsoleCommand == null)
		{
			return string.Empty;
		}
		return debugConsoleCommand.Command;
	}

	// Token: 0x06000988 RID: 2440 RVA: 0x00028358 File Offset: 0x00026558
	public static Network.DebugConsoleResponse GetDebugConsoleResponse()
	{
		DebugConsoleResponse debugConsoleResponse = ConnectAPI.Unpack<DebugConsoleResponse>(ConnectAPI.NextGame(), 124);
		if (debugConsoleResponse == null)
		{
			return null;
		}
		return new Network.DebugConsoleResponse
		{
			Type = debugConsoleResponse.ResponseType_,
			Response = debugConsoleResponse.Response
		};
	}

	// Token: 0x06000989 RID: 2441 RVA: 0x0002839C File Offset: 0x0002659C
	public static void SendDebugConsoleResponse(int responseType, string message)
	{
		if (message == null)
		{
			return;
		}
		if (ConnectAPI.s_debugConsole == null)
		{
			return;
		}
		if (!ConnectAPI.s_debugConsole.Active)
		{
			Debug.LogWarning("Cannont send console response " + message + "; no debug console is active.");
			return;
		}
		ConnectAPI.QueueDebugPacket(124, new DebugConsoleResponse
		{
			ResponseType_ = responseType,
			Response = message
		});
	}

	// Token: 0x0600098A RID: 2442 RVA: 0x000283FC File Offset: 0x000265FC
	public static void SendIndirectConsoleCommand(string command)
	{
		if (command == null)
		{
			return;
		}
		if (!ConnectAPI.s_gameServer.Active)
		{
			Debug.LogWarning("Cannot send an indirect console command '" + command + "' to server; no game server is active.");
			return;
		}
		ConnectAPI.QueueGamePacket(123, new DebugConsoleCommand
		{
			Command = command
		});
	}

	// Token: 0x04000487 RID: 1159
	private const float ERROR_HANDLING_DELAY = 0.4f;

	// Token: 0x04000488 RID: 1160
	private const int MAX_SERVERS = 3;

	// Token: 0x04000489 RID: 1161
	private const int NO_CONTEXT = 0;

	// Token: 0x0400048A RID: 1162
	private const int STARTING_CONTEXT = 1;

	// Token: 0x0400048B RID: 1163
	private static ConnectAPI s_connectAPI;

	// Token: 0x0400048C RID: 1164
	private static int s_gameServerKeepAliveFrequencySeconds = 0;

	// Token: 0x0400048D RID: 1165
	private static float s_lastPingSent;

	// Token: 0x0400048E RID: 1166
	private static float s_lastPingReceived;

	// Token: 0x0400048F RID: 1167
	private static int s_pingsSentSinceLastPong = 0;

	// Token: 0x04000490 RID: 1168
	private static ClientConnection<PegasusPacket> s_gameServer;

	// Token: 0x04000491 RID: 1169
	private static bool s_gameConceded;

	// Token: 0x04000492 RID: 1170
	private static bool s_disconnectRequested;

	// Token: 0x04000493 RID: 1171
	private static int DEBUG_CLIENT_TCP_PORT = 1226;

	// Token: 0x04000494 RID: 1172
	private static ServerConnection<PegasusPacket> s_debugListener;

	// Token: 0x04000495 RID: 1173
	private static ClientConnection<PegasusPacket> s_debugConsole;

	// Token: 0x04000496 RID: 1174
	private static bool s_initialized = false;

	// Token: 0x04000497 RID: 1175
	private static ConnectAPI.GameStartState s_gameStartState = ConnectAPI.GameStartState.INVALID;

	// Token: 0x04000498 RID: 1176
	private static SortedDictionary<int, ConnectAPI.PacketDecoder> s_packetDecoders = new SortedDictionary<int, ConnectAPI.PacketDecoder>();

	// Token: 0x04000499 RID: 1177
	private static List<ConnectAPI.ThrottledPacketListener> m_throttledPacketListeners = new List<ConnectAPI.ThrottledPacketListener>();

	// Token: 0x0400049A RID: 1178
	private static Queue<PegasusPacket> s_gamePackets = new Queue<PegasusPacket>();

	// Token: 0x0400049B RID: 1179
	private static Queue<PegasusPacket> s_debugPackets = new Queue<PegasusPacket>();

	// Token: 0x0400049C RID: 1180
	private static ClientRequestManager s_clientRequestManager = new ClientRequestManager();

	// Token: 0x0400049D RID: 1181
	private static List<ConnectAPI.ConnectErrorParams> s_errorList = new List<ConnectAPI.ConnectErrorParams>();

	// Token: 0x0400049E RID: 1182
	private static PlatformDependentValue<bool> s_reconnectAfterFailedPings = new PlatformDependentValue<bool>(PlatformCategory.OS)
	{
		PC = true,
		Android = true,
		iOS = true
	};

	// Token: 0x0400049F RID: 1183
	public static int SEND_DECK_DATA_NO_HERO_ASSET_CHANGE = -1;

	// Token: 0x040004A0 RID: 1184
	public static int SEND_DECK_DATA_NO_CARD_BACK_CHANGE = -1;

	// Token: 0x040004A1 RID: 1185
	private static AckCardSeen m_ackCardSeenPacket = new AckCardSeen();

	// Token: 0x02000114 RID: 276
	// (Invoke) Token: 0x06000CD6 RID: 3286
	public delegate void ThrottledPacketListener(int packetID, long retryMillis);

	// Token: 0x020005A1 RID: 1441
	private enum ServerType
	{
		// Token: 0x04002957 RID: 10583
		GAME_SERVER,
		// Token: 0x04002958 RID: 10584
		UTIL_SERVER,
		// Token: 0x04002959 RID: 10585
		DEBUG_CONSOLE
	}

	// Token: 0x020005A2 RID: 1442
	private class QueuedPacket
	{
		// Token: 0x060040E5 RID: 16613 RVA: 0x00138EF2 File Offset: 0x001370F2
		public QueuedPacket(PegasusPacket p, Queue<PegasusPacket> t)
		{
			this.packet = p;
			this.targetQueue = t;
		}

		// Token: 0x0400295A RID: 10586
		public PegasusPacket packet;

		// Token: 0x0400295B RID: 10587
		public Queue<PegasusPacket> targetQueue;
	}

	// Token: 0x020005A3 RID: 1443
	private class OutboundUtilPacket
	{
		// Token: 0x060040E6 RID: 16614 RVA: 0x00138F08 File Offset: 0x00137108
		public OutboundUtilPacket(int type, int system, int subID, int context, IProtoBuf body)
		{
			this.Type = type;
			this.System = system;
			this.SubID = subID;
			this.Context = context;
			this.Body = body;
			this.ResendTime = 0L;
		}

		// Token: 0x0400295C RID: 10588
		public int Type;

		// Token: 0x0400295D RID: 10589
		public int System;

		// Token: 0x0400295E RID: 10590
		public IProtoBuf Body;

		// Token: 0x0400295F RID: 10591
		public int SubID;

		// Token: 0x04002960 RID: 10592
		public int Context;

		// Token: 0x04002961 RID: 10593
		public long ResendTime;
	}

	// Token: 0x020005A4 RID: 1444
	private class ConnectErrorParams : ErrorParams
	{
		// Token: 0x060040E7 RID: 16615 RVA: 0x00138F3D File Offset: 0x0013713D
		public ConnectErrorParams()
		{
			this.m_creationTime = Time.realtimeSinceStartup;
		}

		// Token: 0x04002962 RID: 10594
		public float m_creationTime;
	}

	// Token: 0x020005A5 RID: 1445
	private enum GameStartState
	{
		// Token: 0x04002964 RID: 10596
		INVALID,
		// Token: 0x04002965 RID: 10597
		INITIAL_START,
		// Token: 0x04002966 RID: 10598
		RECONNECTING
	}

	// Token: 0x020005A6 RID: 1446
	public abstract class PacketDecoder
	{
		// Token: 0x060040E8 RID: 16616 RVA: 0x00138F50 File Offset: 0x00137150
		public PacketDecoder()
		{
		}

		// Token: 0x060040E9 RID: 16617 RVA: 0x00138F58 File Offset: 0x00137158
		public static PegasusPacket HandleProtobuf<T>(PegasusPacket p) where T : IProtoBuf, new()
		{
			byte[] array = (byte[])p.Body;
			T t = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
			t.Deserialize(new MemoryStream(array));
			p.Body = t;
			return p;
		}

		// Token: 0x060040EA RID: 16618
		public abstract PegasusPacket HandlePacket(PegasusPacket p);
	}

	// Token: 0x020005A7 RID: 1447
	public class DefaultProtobufPacketDecoder<T> : ConnectAPI.PacketDecoder where T : IProtoBuf, new()
	{
		// Token: 0x060040EC RID: 16620 RVA: 0x00138FBD File Offset: 0x001371BD
		public override PegasusPacket HandlePacket(PegasusPacket p)
		{
			return ConnectAPI.PacketDecoder.HandleProtobuf<T>(p);
		}
	}

	// Token: 0x020005A8 RID: 1448
	public class NoOpPacketDecoder : ConnectAPI.PacketDecoder
	{
		// Token: 0x060040EE RID: 16622 RVA: 0x00138FD0 File Offset: 0x001371D0
		public override PegasusPacket HandlePacket(PegasusPacket p)
		{
			return new PegasusPacket
			{
				Type = 254,
				Context = p.Context
			};
		}
	}

	// Token: 0x020005A9 RID: 1449
	public class PongPacketDecoder : ConnectAPI.PacketDecoder
	{
		// Token: 0x060040F0 RID: 16624 RVA: 0x00139003 File Offset: 0x00137203
		public override PegasusPacket HandlePacket(PegasusPacket p)
		{
			return null;
		}
	}

	// Token: 0x020005AA RID: 1450
	public class DeprecatedPacketDecoder : ConnectAPI.PacketDecoder
	{
		// Token: 0x060040F2 RID: 16626 RVA: 0x0013900E File Offset: 0x0013720E
		public override PegasusPacket HandlePacket(PegasusPacket p)
		{
			Debug.LogWarning("Dropping deprecated packet of type: " + p.Type);
			return null;
		}
	}
}
