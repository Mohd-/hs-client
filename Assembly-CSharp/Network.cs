using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using bgs;
using bgs.types;
using BobNetProto;
using PegasusGame;
using PegasusShared;
using PegasusUtil;
using SpectatorProto;
using UnityEngine;
using WTCG.BI;

// Token: 0x0200000B RID: 11
public class Network
{
	// Token: 0x06000071 RID: 113 RVA: 0x000048F8 File Offset: 0x00002AF8
	// Note: this type is marked as 'beforefieldinit'.
	static Network()
	{
		SortedDictionary<int, int> sortedDictionary = new SortedDictionary<int, int>();
		sortedDictionary.Add(305, 306);
		sortedDictionary.Add(303, 304);
		sortedDictionary.Add(267, 330);
		sortedDictionary.Add(205, 307);
		sortedDictionary.Add(253, 252);
		sortedDictionary.Add(314, 315);
		Network.m_deferredMessageResponseMap = sortedDictionary;
		sortedDictionary = new SortedDictionary<int, int>();
		sortedDictionary.Add(11, 233);
		sortedDictionary.Add(6, 224);
		sortedDictionary.Add(14, 241);
		sortedDictionary.Add(18, 264);
		sortedDictionary.Add(4, 232);
		sortedDictionary.Add(12, 212);
		sortedDictionary.Add(2, 202);
		sortedDictionary.Add(3, 207);
		sortedDictionary.Add(10, 231);
		sortedDictionary.Add(15, 260);
		sortedDictionary.Add(17, 262);
		sortedDictionary.Add(23, 300);
		sortedDictionary.Add(19, 271);
		sortedDictionary.Add(8, 270);
		sortedDictionary.Add(20, 278);
		sortedDictionary.Add(21, 283);
		sortedDictionary.Add(7, 236);
		sortedDictionary.Add(27, 318);
		sortedDictionary.Add(28, 325);
		Network.m_deferredGetAccountInfoMessageResponseMap = sortedDictionary;
		Network.LAUNCHES_WITH_BNET_APP = new PlatformDependentValue<bool>(PlatformCategory.OS)
		{
			PC = true,
			Mac = true,
			iOS = false,
			Android = false
		};
		Network.TUTORIALS_WITHOUT_ACCOUNT = new PlatformDependentValue<bool>(PlatformCategory.OS)
		{
			PC = false,
			Mac = false,
			iOS = true,
			Android = true
		};
		Network.s_shouldBeConnectedToAurora = true;
	}

	// Token: 0x06000072 RID: 114 RVA: 0x00004B2A File Offset: 0x00002D2A
	public static int ProductVersion()
	{
		return 83886080;
	}

	// Token: 0x06000073 RID: 115 RVA: 0x00004B31 File Offset: 0x00002D31
	public static TimeSpan GetMaxDeferredWait()
	{
		return Network.MAX_DEFERRED_WAIT;
	}

	// Token: 0x06000074 RID: 116 RVA: 0x00004B38 File Offset: 0x00002D38
	private static void ProcessRequestTimeouts()
	{
		DateTime now = DateTime.Now;
		Network.m_inTransitRequests.ForEach(delegate(Network.RequestContext rc)
		{
			if (rc.m_timeoutHandler != null && rc.m_waitUntil < now)
			{
				Debug.LogWarning(string.Format("Encountered timeout waiting for {0} {1} {2}", rc.m_pendingResponseId, rc.m_requestId, rc.m_requestSubId));
				rc.m_timeoutHandler(rc.m_pendingResponseId, rc.m_requestId, rc.m_requestSubId);
			}
		});
		Network.m_inTransitRequests.RemoveAll((Network.RequestContext rc) => rc.m_waitUntil < now);
	}

	// Token: 0x06000075 RID: 117 RVA: 0x00004B84 File Offset: 0x00002D84
	public static void AddPendingRequestTimeout(int requestId, int requestSubId)
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			return;
		}
		int num = 0;
		if ((requestId != 201 || !Network.m_deferredGetAccountInfoMessageResponseMap.TryGetValue(requestSubId, ref num)) && !Network.m_deferredMessageResponseMap.TryGetValue(requestId, ref num))
		{
			return;
		}
		Network.TimeoutHandler timeoutHandler = null;
		if (Network.s_instance.m_netTimeoutHandlers.TryGetValue(num, out timeoutHandler))
		{
			Network.m_inTransitRequests.Add(new Network.RequestContext(num, requestId, requestSubId, timeoutHandler));
		}
		else
		{
			Network.m_inTransitRequests.Add(new Network.RequestContext(num, requestId, requestSubId, new Network.TimeoutHandler(Network.OnRequestTimeout)));
		}
	}

	// Token: 0x06000076 RID: 118 RVA: 0x00004C24 File Offset: 0x00002E24
	private static void RemovePendingRequestTimeout(int pendingResponseId)
	{
		Network.m_inTransitRequests.RemoveAll((Network.RequestContext pc) => pc.m_pendingResponseId == pendingResponseId);
	}

	// Token: 0x06000077 RID: 119 RVA: 0x00004C58 File Offset: 0x00002E58
	private static void OnRequestTimeout(int pendingResponseId, int requestId, int requestSubId)
	{
		if (Network.m_deferredMessageResponseMap.ContainsValue(pendingResponseId) || Network.m_deferredGetAccountInfoMessageResponseMap.ContainsValue(pendingResponseId))
		{
			Debug.LogError(string.Format("OnRequestTimeout pending ID {0} {1} {2}", pendingResponseId, requestId, requestSubId));
			FatalErrorMgr.Get().SetErrorCode("HS", "NT" + pendingResponseId.ToString(), requestId.ToString(), requestSubId.ToString());
			BIReport.Get().Report_Telemetry(Telemetry.Level.LEVEL_ERROR, BIReport.TelemetryEvent.EVENT_ERROR_NETWORK_UNAVAILABLE, 2, FatalErrorMgr.Get().GetFormattedErrorCode());
			Network.Get().ShowBreakingNewsOrError("GLOBAL_ERROR_NETWORK_UNAVAILABLE_UNKNOWN", 0f);
		}
		else
		{
			Debug.LogError(string.Format("Unhandled OnRequestTimeout pending ID {0} {1} {2}", pendingResponseId, requestId, requestSubId));
			FatalErrorMgr.Get().SetErrorCode("HS", "NU" + pendingResponseId.ToString(), requestId.ToString(), requestSubId.ToString());
			BIReport.Get().Report_Telemetry(Telemetry.Level.LEVEL_ERROR, BIReport.TelemetryEvent.EVENT_ERROR_NETWORK_UNAVAILABLE, 3, FatalErrorMgr.Get().GetFormattedErrorCode());
			Network.Get().ShowBreakingNewsOrError("GLOBAL_ERROR_NETWORK_UNAVAILABLE_UNKNOWN", 0f);
		}
	}

	// Token: 0x06000078 RID: 120 RVA: 0x00004D88 File Offset: 0x00002F88
	private static void OnGenericResponse()
	{
		Network.GenericResponse genericResponse = ConnectAPI.GetGenericResponse();
		if (genericResponse == null)
		{
			Debug.LogError(string.Format("Login - GenericResponse parse error", new object[0]));
			return;
		}
		bool flag = genericResponse.RequestId == 201 && Network.m_deferredGetAccountInfoMessageResponseMap.ContainsKey(genericResponse.RequestSubId);
		bool flag2 = Network.m_deferredMessageResponseMap.ContainsKey(genericResponse.RequestId);
		if (!flag && !flag2)
		{
			return;
		}
		if (genericResponse.ResultCode == Network.GenericResponse.Result.REQUEST_IN_PROCESS)
		{
			return;
		}
		Debug.LogError(string.Format("Unhandled resultCode {0} for requestId {1}:{2}", genericResponse.ResultCode, genericResponse.RequestId, genericResponse.RequestSubId));
		FatalErrorMgr.Get().SetErrorCode("HS", "NG" + genericResponse.ResultCode.ToString(), genericResponse.RequestId.ToString(), genericResponse.RequestSubId.ToString());
		BIReport.Get().Report_Telemetry(Telemetry.Level.LEVEL_ERROR, BIReport.TelemetryEvent.EVENT_ERROR_NETWORK_UNAVAILABLE, 4, FatalErrorMgr.Get().GetFormattedErrorCode());
		Network.Get().ShowBreakingNewsOrError("GLOBAL_ERROR_NETWORK_UNAVAILABLE_UNKNOWN", 0f);
	}

	// Token: 0x06000079 RID: 121 RVA: 0x00004EAB File Offset: 0x000030AB
	public static bool IsRunning()
	{
		return Network.s_running;
	}

	// Token: 0x0600007A RID: 122 RVA: 0x00004EB4 File Offset: 0x000030B4
	private static void OnSubscribeResponse()
	{
		SubscribeResponse subscribeResponse = ConnectAPI.GetSubscribeResponse();
		if (subscribeResponse != null && subscribeResponse.HasRequestMaxWaitSecs && subscribeResponse.RequestMaxWaitSecs >= 30UL)
		{
			Network.MAX_DEFERRED_WAIT = new TimeSpan(0, 0, (int)subscribeResponse.RequestMaxWaitSecs);
		}
	}

	// Token: 0x0600007B RID: 123 RVA: 0x00004EFC File Offset: 0x000030FC
	public static void Initialize()
	{
		Network.s_running = true;
		NetCache.Get().InitNetCache();
		Network.InitBattleNet();
		Network.s_instance.RegisterNetHandler(315, new Network.NetHandler(Network.OnSubscribeResponse), null);
		Network.s_instance.RegisterNetHandler(315, new Network.NetHandler(Network.OnSubscribeResponse), null);
		Network.s_instance.RegisterNetHandler(315, new Network.NetHandler(Network.OnSubscribeResponse), null);
		Network.s_instance.RegisterNetHandler(315, new Network.NetHandler(Network.OnSubscribeResponse), null);
		Network.s_instance.RegisterNetHandler(315, new Network.NetHandler(Network.OnSubscribeResponse), null);
		Network.s_instance.RegisterNetHandler(315, new Network.NetHandler(Network.OnSubscribeResponse), null);
		Network.s_instance.RegisterNetHandler(315, new Network.NetHandler(Network.OnSubscribeResponse), null);
		Network.s_instance.RegisterNetHandler(315, new Network.NetHandler(Network.OnSubscribeResponse), null);
		Network.s_instance.RegisterNetHandler(315, new Network.NetHandler(Network.OnSubscribeResponse), null);
		Network.s_instance.RegisterNetHandler(326, new Network.NetHandler(Network.OnGenericResponse), null);
	}

	// Token: 0x0600007C RID: 124 RVA: 0x00005074 File Offset: 0x00003274
	public static void Reset()
	{
		NetCache.Get().Clear();
		Network.s_instance.m_delayedError = null;
		Network.s_instance.m_timeBeforeAllowReset = 0f;
		Network network = Network.s_instance;
		Network.s_instance = new Network();
		Network.s_instance.m_netHandlers = network.m_netHandlers;
		Network.s_instance.m_gameQueueHandler = network.m_gameQueueHandler;
		Network.s_instance.m_spectatorInviteReceivedHandler = network.m_spectatorInviteReceivedHandler;
		Network.s_running = true;
		ConnectAPI.CloseAll();
		string username = Network.GetUsername();
		string targetServer = Network.GetTargetServer();
		int port = Network.GetPort();
		ClientInterface clientInterface = new Network.HSClientInterface();
		SslParameters sslparams = Network.GetSSLParams();
		if (BattleNet.Reset(ApplicationMgr.IsInternal(), username, targetServer, port, sslparams, clientInterface) || !Network.ShouldBeConnectedToAurora())
		{
			ConnectAPI.ConnectInit();
		}
	}

	// Token: 0x0600007D RID: 125 RVA: 0x0000513C File Offset: 0x0000333C
	public static void ApplicationPaused()
	{
		NetCache.NetCacheClientOptions netObject = NetCache.Get().GetNetObject<NetCache.NetCacheClientOptions>();
		if (netObject != null)
		{
			netObject.DispatchClientOptionsToServer();
		}
		BattleNet.ApplicationWasPaused();
	}

	// Token: 0x0600007E RID: 126 RVA: 0x00005165 File Offset: 0x00003365
	public static void ApplicationUnpaused()
	{
		BattleNet.ApplicationWasUnpaused();
	}

	// Token: 0x0600007F RID: 127 RVA: 0x0000516C File Offset: 0x0000336C
	public static void Heartbeat()
	{
		if (!Network.s_running)
		{
			return;
		}
		Network.ProcessRequestTimeouts();
		NetCache.Get().Heartbeat();
		ConnectAPI.Heartbeat();
		StoreManager.Get().Heartbeat();
		if (AchieveManager.Get() != null)
		{
			AchieveManager.Get().Heartbeat();
		}
		NetCache.Get().CheckSeasonForRoll();
		TimeSpan timeSpan = DateTime.Now - Network.s_instance.lastCall;
		if (timeSpan < Network.PROCESS_WARNING)
		{
			return;
		}
		TimeSpan timeSpan2 = DateTime.Now - Network.s_instance.lastCallReport;
		if (timeSpan2 < Network.PROCESS_WARNING_REPORT_GAP)
		{
			return;
		}
		Network.s_instance.lastCallReport = DateTime.Now;
		string devElapsedTimeString = TimeUtils.GetDevElapsedTimeString(timeSpan);
		Debug.LogWarning(string.Format("Network.ProcessNetwork not called for {0}", devElapsedTimeString));
	}

	// Token: 0x06000080 RID: 128 RVA: 0x00005234 File Offset: 0x00003434
	public static void AppQuit()
	{
		if (!Network.s_running)
		{
			return;
		}
		NetCache.NetCacheClientOptions netObject = NetCache.Get().GetNetObject<NetCache.NetCacheClientOptions>();
		if (netObject != null)
		{
			netObject.DispatchClientOptionsToServer();
		}
		Network.ConcedeIfReconnectDisabled();
		Network.s_instance.CancelFindGame();
		ConnectAPI.CloseAll();
		BattleNet.AppQuit();
		PlayErrors.AppQuit();
		BnetNearbyPlayerMgr.Get().Shutdown();
		Network.s_running = false;
	}

	// Token: 0x06000081 RID: 129 RVA: 0x00005294 File Offset: 0x00003494
	public static void AppAbort()
	{
		if (!Network.s_running)
		{
			return;
		}
		NetCache.NetCacheClientOptions netObject = NetCache.Get().GetNetObject<NetCache.NetCacheClientOptions>();
		if (netObject != null)
		{
			netObject.DispatchClientOptionsToServer();
		}
		Network.ConcedeIfReconnectDisabled();
		Network.s_instance.CancelFindGame();
		ConnectAPI.CloseAll();
		BattleNet.AppQuit();
		PlayErrors.AppQuit();
		BnetNearbyPlayerMgr.Get().Shutdown();
		Network.s_running = false;
	}

	// Token: 0x06000082 RID: 130 RVA: 0x000052F1 File Offset: 0x000034F1
	public static void ResetConnectionFailureCount()
	{
		Network.s_numConnectionFailures = 0;
	}

	// Token: 0x06000083 RID: 131 RVA: 0x000052F9 File Offset: 0x000034F9
	private static void ConcedeIfReconnectDisabled()
	{
		if (ReconnectMgr.Get().IsReconnectEnabled())
		{
			return;
		}
		Network.AutoConcede();
	}

	// Token: 0x06000084 RID: 132 RVA: 0x00005310 File Offset: 0x00003510
	public bool RegisterNetHandler(object enumId, Network.NetHandler handler, Network.TimeoutHandler timeoutHandler = null)
	{
		int key = (int)enumId;
		if (timeoutHandler != null)
		{
			if (this.m_netTimeoutHandlers.ContainsKey(key))
			{
				return false;
			}
			this.m_netTimeoutHandlers.Add(key, timeoutHandler);
		}
		List<Network.NetHandler> list;
		if (this.m_netHandlers.TryGetValue(key, out list))
		{
			if (list.Contains(handler))
			{
				return false;
			}
		}
		else
		{
			list = new List<Network.NetHandler>();
			this.m_netHandlers.Add(key, list);
		}
		list.Add(handler);
		return true;
	}

	// Token: 0x06000085 RID: 133 RVA: 0x0000538C File Offset: 0x0000358C
	public bool RemoveNetHandler(object enumId, Network.NetHandler handler)
	{
		int key = (int)enumId;
		List<Network.NetHandler> list;
		return this.m_netHandlers.TryGetValue(key, out list) && list.Remove(handler);
	}

	// Token: 0x06000086 RID: 134 RVA: 0x000053C2 File Offset: 0x000035C2
	public void RegisterGameQueueHandler(Network.GameQueueHandler handler)
	{
		if (this.m_gameQueueHandler != null)
		{
			Log.Net.Print("handler {0} would bash game queue handler {1}", new object[]
			{
				handler,
				this.m_gameQueueHandler
			});
			return;
		}
		this.m_gameQueueHandler = handler;
	}

	// Token: 0x06000087 RID: 135 RVA: 0x000053FC File Offset: 0x000035FC
	public void RemoveGameQueueHandler(Network.GameQueueHandler handler)
	{
		if (this.m_gameQueueHandler != handler)
		{
			Log.Net.Print("Removing game queue handler that is not active {0}", new object[]
			{
				handler
			});
			return;
		}
		this.m_gameQueueHandler = null;
	}

	// Token: 0x06000088 RID: 136 RVA: 0x0000543B File Offset: 0x0000363B
	public void RegisterQueueInfoHandler(Network.QueueInfoHandler handler)
	{
		if (this.m_queueInfoHandler != null)
		{
			Log.Net.Print("handler {0} would bash queue info handler {1}", new object[]
			{
				handler,
				this.m_queueInfoHandler
			});
			return;
		}
		this.m_queueInfoHandler = handler;
	}

	// Token: 0x06000089 RID: 137 RVA: 0x00005474 File Offset: 0x00003674
	public void RemoveQueueInfoHandler(Network.QueueInfoHandler handler)
	{
		if (this.m_queueInfoHandler != handler)
		{
			Log.Net.Print("Removing queue info handler that is not active {0}", new object[]
			{
				handler
			});
			return;
		}
		this.m_queueInfoHandler = null;
	}

	// Token: 0x0600008A RID: 138 RVA: 0x000054B4 File Offset: 0x000036B4
	public bool FakeHandleType(object enumId)
	{
		int id = (int)enumId;
		return this.FakeHandleType(id);
	}

	// Token: 0x0600008B RID: 139 RVA: 0x000054CF File Offset: 0x000036CF
	public bool FakeHandleType(int id)
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			this.HandleType(id);
			return true;
		}
		return false;
	}

	// Token: 0x0600008C RID: 140 RVA: 0x000054E8 File Offset: 0x000036E8
	private bool HandleType(int id)
	{
		Network.RemovePendingRequestTimeout(id);
		List<Network.NetHandler> list;
		if (!this.m_netHandlers.TryGetValue(id, out list) || list.Count == 0)
		{
			if (!this.CanIgnoreUnhandledPacket(id))
			{
				Debug.LogError(string.Format("Network.HandleType() - Received packet {0}, but there are no handlers for it.", id));
			}
			return false;
		}
		Network.NetHandler[] array = list.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i]();
		}
		return true;
	}

	// Token: 0x0600008D RID: 141 RVA: 0x00005560 File Offset: 0x00003760
	private bool CanIgnoreUnhandledPacket(int id)
	{
		return id == 15 || id == 116 || id == 254;
	}

	// Token: 0x0600008E RID: 142 RVA: 0x00005594 File Offset: 0x00003794
	private bool ProcessGameQueue()
	{
		QueueEvent queueEvent = BattleNet.GetQueueEvent();
		if (queueEvent == null)
		{
			return false;
		}
		switch (queueEvent.EventType)
		{
		case 2:
		case 5:
		case 6:
		case 8:
		case 9:
		case 10:
			this.m_findingBnetGameType = 0;
			break;
		}
		if (this.m_gameQueueHandler == null)
		{
			Debug.LogWarning("m_gameQueueHandler is null in Network.ProcessGameQueue!");
		}
		else
		{
			this.m_gameQueueHandler(queueEvent);
		}
		return true;
	}

	// Token: 0x0600008F RID: 143 RVA: 0x0000561C File Offset: 0x0000381C
	private bool ProcessGameServer()
	{
		int id = ConnectAPI.NextGameType();
		bool result = this.HandleType(id);
		ConnectAPI.DropGamePacket();
		return result;
	}

	// Token: 0x06000090 RID: 144 RVA: 0x00005640 File Offset: 0x00003840
	private bool ProcessUtilServer()
	{
		int id = ConnectAPI.NextUtilType();
		bool result = this.HandleType(id);
		ConnectAPI.DropUtilPacket();
		return result;
	}

	// Token: 0x06000091 RID: 145 RVA: 0x00005664 File Offset: 0x00003864
	private bool ProcessConsole()
	{
		int id = ConnectAPI.NextDebugConsoleType();
		bool result = this.HandleType(id);
		ConnectAPI.DropDebugPacket();
		return result;
	}

	// Token: 0x06000092 RID: 146 RVA: 0x00005688 File Offset: 0x00003888
	public static Network.UnavailableReason GetHearthstoneUnavailable(bool gamePacket)
	{
		Network.UnavailableReason unavailableReason = new Network.UnavailableReason();
		if (gamePacket)
		{
			Deadend deadendGame = ConnectAPI.GetDeadendGame();
			unavailableReason.mainReason = deadendGame.Reply1;
			unavailableReason.subReason = deadendGame.Reply2;
			unavailableReason.extraData = deadendGame.Reply3;
		}
		else
		{
			DeadendUtil deadendUtil = ConnectAPI.GetDeadendUtil();
			unavailableReason.mainReason = deadendUtil.Reply1;
			unavailableReason.subReason = deadendUtil.Reply2;
			unavailableReason.extraData = deadendUtil.Reply3;
		}
		return unavailableReason;
	}

	// Token: 0x06000093 RID: 147 RVA: 0x000056FB File Offset: 0x000038FB
	public static void BuyCard(int assetID, TAG_PREMIUM premium, int count, int unitBuyPrice)
	{
		ConnectAPI.BuyCard(assetID, (int)premium, count, unitBuyPrice);
	}

	// Token: 0x06000094 RID: 148 RVA: 0x00005706 File Offset: 0x00003906
	public static void SellCard(int assetID, TAG_PREMIUM premium, int count, int unitSellPrice)
	{
		ConnectAPI.SellCard(assetID, (int)premium, count, unitSellPrice);
	}

	// Token: 0x06000095 RID: 149 RVA: 0x00005711 File Offset: 0x00003911
	public static void CloseCardMarket()
	{
	}

	// Token: 0x06000096 RID: 150 RVA: 0x00005713 File Offset: 0x00003913
	public static void GetAllClientOptions()
	{
		ConnectAPI.GetAllClientOptions();
	}

	// Token: 0x06000097 RID: 151 RVA: 0x0000571A File Offset: 0x0000391A
	public static Network.BnetLoginState BattleNetStatus()
	{
		return (Network.BnetLoginState)BattleNet.BattleNetStatus();
	}

	// Token: 0x06000098 RID: 152 RVA: 0x00005724 File Offset: 0x00003924
	public static bool IsLoggedIn()
	{
		if (!BattleNet.IsInitialized())
		{
			return false;
		}
		Network.BnetLoginState bnetLoginState = (Network.BnetLoginState)BattleNet.BattleNetStatus();
		return bnetLoginState == Network.BnetLoginState.BATTLE_NET_LOGGED_IN;
	}

	// Token: 0x06000099 RID: 153 RVA: 0x00005748 File Offset: 0x00003948
	public bool HaveUnhandledPackets()
	{
		return ConnectAPI.HaveUtilPackets() || ConnectAPI.HaveGamePackets() || ConnectAPI.HaveDebugPackets() || ConnectAPI.HaveNotificationPackets();
	}

	// Token: 0x0600009A RID: 154 RVA: 0x00005788 File Offset: 0x00003988
	public void ProcessNetwork()
	{
		if (!Network.s_running)
		{
			return;
		}
		this.lastCall = DateTime.Now;
		if (this.m_loginWaiting && this.lastCall - this.m_loginStarted > Network.LOGIN_TIMEOUT)
		{
			this.m_loginWaiting = false;
		}
		if (!Network.InitBattleNet())
		{
			return;
		}
		Network.s_urlDownloader.Process();
		if (Network.ShouldBeConnectedToAurora())
		{
			this.ProcessAurora();
		}
		else
		{
			this.ProcessDelayedError();
		}
		if (this.ProcessGameQueue())
		{
			return;
		}
		if (ConnectAPI.HaveGamePackets())
		{
			this.ProcessGameServer();
			return;
		}
		if (ConnectAPI.GameServerDisconnectEvents != null && ConnectAPI.GameServerDisconnectEvents.Count > 0)
		{
			if (this.m_gameServerDisconnectEventListener != null)
			{
				BattleNetErrors[] array = ConnectAPI.GameServerDisconnectEvents.ToArray();
				foreach (BattleNetErrors errorCode in array)
				{
					this.m_gameServerDisconnectEventListener(errorCode);
				}
			}
			ConnectAPI.GameServerDisconnectEvents.Clear();
		}
		if (ConnectAPI.HaveUtilPackets())
		{
			this.ProcessUtilServer();
			return;
		}
		if (ConnectAPI.HaveDebugPackets())
		{
			this.ProcessConsole();
			return;
		}
		this.ProcessQueuePosition();
	}

	// Token: 0x0600009B RID: 155 RVA: 0x000058B8 File Offset: 0x00003AB8
	private static bool InitBattleNet()
	{
		bool flag = BattleNet.IsInitialized();
		if (!flag)
		{
			LogAdapter.SetLogger<Network.BattleNetLogger>(new Network.BattleNetLogger());
			string username = Network.GetUsername();
			string targetServer = Network.GetTargetServer();
			int port = Network.GetPort();
			SslParameters sslparams = Network.GetSSLParams();
			ClientInterface clientInterface = new Network.HSClientInterface();
			flag = BattleNet.Init(ApplicationMgr.IsInternal(), username, targetServer, port, sslparams, clientInterface);
			if (flag || !Network.ShouldBeConnectedToAurora())
			{
				Network.Get().AddBnetErrorListener(0, new Network.BnetErrorCallback(Network.Get().OnBnetAuthError));
				DebugConsole.Get().Init();
				ConnectAPI.ConnectInit();
			}
		}
		return flag;
	}

	// Token: 0x0600009C RID: 156 RVA: 0x00005949 File Offset: 0x00003B49
	public static bool ShouldBeConnectedToAurora()
	{
		return !Network.TUTORIALS_WITHOUT_ACCOUNT || Network.s_shouldBeConnectedToAurora;
	}

	// Token: 0x0600009D RID: 157 RVA: 0x00005962 File Offset: 0x00003B62
	public static void SetShouldBeConnectedToAurora(bool shouldBeConnected)
	{
		Network.s_shouldBeConnectedToAurora = shouldBeConnected;
	}

	// Token: 0x0600009E RID: 158 RVA: 0x0000596C File Offset: 0x00003B6C
	public void ProcessQueuePosition()
	{
		bgs.types.QueueInfo queueInfo = default(bgs.types.QueueInfo);
		BattleNet.GetQueueInfo(ref queueInfo);
		if (!queueInfo.changed)
		{
			return;
		}
		if (this.m_queueInfoHandler == null)
		{
			return;
		}
		Network.QueueInfo queueInfo2 = new Network.QueueInfo();
		queueInfo2.position = queueInfo.position;
		queueInfo2.end = queueInfo.end;
		queueInfo2.stdev = queueInfo.stdev;
		this.m_queueInfoHandler(queueInfo2);
	}

	// Token: 0x0600009F RID: 159 RVA: 0x000059DA File Offset: 0x00003BDA
	public Network.BnetEventHandler GetBnetEventHandler()
	{
		return this.m_bnetEventHandler;
	}

	// Token: 0x060000A0 RID: 160 RVA: 0x000059E2 File Offset: 0x00003BE2
	public void SetBnetStateHandler(Network.BnetEventHandler handler)
	{
		this.m_bnetEventHandler = handler;
	}

	// Token: 0x060000A1 RID: 161 RVA: 0x000059EB File Offset: 0x00003BEB
	public Network.FriendsHandler GetFriendsHandler()
	{
		return this.m_friendsHandler;
	}

	// Token: 0x060000A2 RID: 162 RVA: 0x000059F3 File Offset: 0x00003BF3
	public void SetFriendsHandler(Network.FriendsHandler handler)
	{
		this.m_friendsHandler = handler;
	}

	// Token: 0x060000A3 RID: 163 RVA: 0x000059FC File Offset: 0x00003BFC
	public Network.WhisperHandler GetWhisperHandler()
	{
		return this.m_whisperHandler;
	}

	// Token: 0x060000A4 RID: 164 RVA: 0x00005A04 File Offset: 0x00003C04
	public Network.PartyHandler GetPartyHandler()
	{
		return this.m_partyHandler;
	}

	// Token: 0x060000A5 RID: 165 RVA: 0x00005A0C File Offset: 0x00003C0C
	public void SetWhisperHandler(Network.WhisperHandler handler)
	{
		this.m_whisperHandler = handler;
	}

	// Token: 0x060000A6 RID: 166 RVA: 0x00005A15 File Offset: 0x00003C15
	public void SetPartyHandler(Network.PartyHandler handler)
	{
		this.m_partyHandler = handler;
	}

	// Token: 0x060000A7 RID: 167 RVA: 0x00005A1E File Offset: 0x00003C1E
	public Network.PresenceHandler GetPresenceHandler()
	{
		return this.m_presenceHandler;
	}

	// Token: 0x060000A8 RID: 168 RVA: 0x00005A26 File Offset: 0x00003C26
	public void SetPresenceHandler(Network.PresenceHandler handler)
	{
		this.m_presenceHandler = handler;
	}

	// Token: 0x060000A9 RID: 169 RVA: 0x00005A2F File Offset: 0x00003C2F
	public Network.ShutdownHandler GetShutdownHandler()
	{
		return this.m_shutdownHandler;
	}

	// Token: 0x060000AA RID: 170 RVA: 0x00005A37 File Offset: 0x00003C37
	public void SetShutdownHandler(Network.ShutdownHandler handler)
	{
		this.m_shutdownHandler = handler;
	}

	// Token: 0x060000AB RID: 171 RVA: 0x00005A40 File Offset: 0x00003C40
	public Network.ChallengeHandler GetChallengeHandler()
	{
		return this.m_challengeHandler;
	}

	// Token: 0x060000AC RID: 172 RVA: 0x00005A48 File Offset: 0x00003C48
	public void SetChallengeHandler(Network.ChallengeHandler handler)
	{
		this.m_challengeHandler = handler;
	}

	// Token: 0x060000AD RID: 173 RVA: 0x00005A51 File Offset: 0x00003C51
	public void SetSpectatorInviteReceivedHandler(Network.SpectatorInviteReceivedHandler handler)
	{
		this.m_spectatorInviteReceivedHandler = handler;
	}

	// Token: 0x060000AE RID: 174 RVA: 0x00005A5A File Offset: 0x00003C5A
	public void SetGameServerDisconnectEventListener(Network.GameServerDisconnectEvent handler)
	{
		this.m_gameServerDisconnectEventListener = handler;
	}

	// Token: 0x060000AF RID: 175 RVA: 0x00005A63 File Offset: 0x00003C63
	public void RemoveGameServerDisconnectEventListener(Network.GameServerDisconnectEvent handler)
	{
		if (this.m_gameServerDisconnectEventListener == handler)
		{
			this.m_gameServerDisconnectEventListener = null;
		}
	}

	// Token: 0x060000B0 RID: 176 RVA: 0x00005A7D File Offset: 0x00003C7D
	public void AddBnetErrorListener(BnetFeature feature, Network.BnetErrorCallback callback)
	{
		this.AddBnetErrorListener(feature, callback, null);
	}

	// Token: 0x060000B1 RID: 177 RVA: 0x00005A88 File Offset: 0x00003C88
	public void AddBnetErrorListener(BnetFeature feature, Network.BnetErrorCallback callback, object userData)
	{
		Network.BnetErrorListener bnetErrorListener = new Network.BnetErrorListener();
		bnetErrorListener.SetCallback(callback);
		bnetErrorListener.SetUserData(userData);
		List<Network.BnetErrorListener> list;
		if (!this.m_featureBnetErrorListeners.TryGetValue(feature, out list))
		{
			list = new List<Network.BnetErrorListener>();
			this.m_featureBnetErrorListeners.Add(feature, list);
		}
		else if (list.Contains(bnetErrorListener))
		{
			return;
		}
		list.Add(bnetErrorListener);
	}

	// Token: 0x060000B2 RID: 178 RVA: 0x00005AE8 File Offset: 0x00003CE8
	public void AddBnetErrorListener(Network.BnetErrorCallback callback)
	{
		this.AddBnetErrorListener(callback, null);
	}

	// Token: 0x060000B3 RID: 179 RVA: 0x00005AF4 File Offset: 0x00003CF4
	public void AddBnetErrorListener(Network.BnetErrorCallback callback, object userData)
	{
		Network.BnetErrorListener bnetErrorListener = new Network.BnetErrorListener();
		bnetErrorListener.SetCallback(callback);
		bnetErrorListener.SetUserData(userData);
		if (this.m_globalBnetErrorListeners.Contains(bnetErrorListener))
		{
			return;
		}
		this.m_globalBnetErrorListeners.Add(bnetErrorListener);
	}

	// Token: 0x060000B4 RID: 180 RVA: 0x00005B33 File Offset: 0x00003D33
	public bool RemoveBnetErrorListener(BnetFeature feature, Network.BnetErrorCallback callback)
	{
		return this.RemoveBnetErrorListener(feature, callback, null);
	}

	// Token: 0x060000B5 RID: 181 RVA: 0x00005B40 File Offset: 0x00003D40
	public bool RemoveBnetErrorListener(BnetFeature feature, Network.BnetErrorCallback callback, object userData)
	{
		List<Network.BnetErrorListener> list;
		if (!this.m_featureBnetErrorListeners.TryGetValue(feature, out list))
		{
			return false;
		}
		Network.BnetErrorListener bnetErrorListener = new Network.BnetErrorListener();
		bnetErrorListener.SetCallback(callback);
		bnetErrorListener.SetUserData(userData);
		return list.Remove(bnetErrorListener);
	}

	// Token: 0x060000B6 RID: 182 RVA: 0x00005B7D File Offset: 0x00003D7D
	public bool RemoveBnetErrorListener(Network.BnetErrorCallback callback)
	{
		return this.RemoveBnetErrorListener(callback, null);
	}

	// Token: 0x060000B7 RID: 183 RVA: 0x00005B88 File Offset: 0x00003D88
	public bool RemoveBnetErrorListener(Network.BnetErrorCallback callback, object userData)
	{
		Network.BnetErrorListener bnetErrorListener = new Network.BnetErrorListener();
		bnetErrorListener.SetCallback(callback);
		bnetErrorListener.SetUserData(userData);
		return this.m_globalBnetErrorListeners.Remove(bnetErrorListener);
	}

	// Token: 0x060000B8 RID: 184 RVA: 0x00005BB8 File Offset: 0x00003DB8
	public void ProcessAurora()
	{
		BattleNet.ProcessAurora();
		this.ProcessBnetEvents();
		if (Network.IsLoggedIn())
		{
			this.ProcessPresence();
			this.ProcessFriends();
			this.ProcessWhispers();
			this.ProcessChallenges();
			this.ProcessParties();
			this.ProcessBroadcasts();
			this.ProcessNotifications();
			BnetNearbyPlayerMgr.Get().Update();
		}
		this.ProcessErrors();
	}

	// Token: 0x060000B9 RID: 185 RVA: 0x00005C14 File Offset: 0x00003E14
	private void ProcessBnetEvents()
	{
		int bnetEventsSize = BattleNet.GetBnetEventsSize();
		if (bnetEventsSize <= 0)
		{
			return;
		}
		if (this.m_bnetEventHandler == null)
		{
			return;
		}
		BattleNet.BnetEvent[] array = new BattleNet.BnetEvent[bnetEventsSize];
		BattleNet.GetBnetEvents(array);
		this.m_bnetEventHandler(array);
		BattleNet.ClearBnetEvents();
	}

	// Token: 0x060000BA RID: 186 RVA: 0x00005C5C File Offset: 0x00003E5C
	private void ProcessWhispers()
	{
		WhisperInfo whisperInfo = default(WhisperInfo);
		BattleNet.GetWhisperInfo(ref whisperInfo);
		if (whisperInfo.whisperSize <= 0)
		{
			return;
		}
		if (this.m_whisperHandler == null)
		{
			return;
		}
		BnetWhisper[] array = new BnetWhisper[whisperInfo.whisperSize];
		BattleNet.GetWhispers(array);
		this.m_whisperHandler(array);
		BattleNet.ClearWhispers();
	}

	// Token: 0x060000BB RID: 187 RVA: 0x00005CB8 File Offset: 0x00003EB8
	private void ProcessParties()
	{
		BnetParty.Process();
		PartyInfo partyInfo = default(PartyInfo);
		BattleNet.GetPartyUpdatesInfo(ref partyInfo);
		if (partyInfo.size <= 0)
		{
			return;
		}
		if (this.m_partyHandler == null)
		{
			return;
		}
		PartyEvent[] array = new PartyEvent[partyInfo.size];
		BattleNet.GetPartyUpdates(array);
		this.m_partyHandler(array);
		BattleNet.ClearPartyUpdates();
	}

	// Token: 0x060000BC RID: 188 RVA: 0x00005D18 File Offset: 0x00003F18
	private void ProcessBroadcasts()
	{
		int shutdownMinutes = BattleNet.GetShutdownMinutes();
		if (shutdownMinutes > 0)
		{
			if (this.m_shutdownHandler == null)
			{
				return;
			}
			this.m_shutdownHandler(shutdownMinutes);
		}
	}

	// Token: 0x060000BD RID: 189 RVA: 0x00005D4C File Offset: 0x00003F4C
	private void ProcessNotifications()
	{
		int notificationCount = BattleNet.GetNotificationCount();
		if (notificationCount <= 0)
		{
			return;
		}
		BnetNotification[] array = new BnetNotification[notificationCount];
		BattleNet.GetNotifications(array);
		BattleNet.ClearNotifications();
		foreach (BnetNotification bnetNotification in array)
		{
			string notificationType = bnetNotification.NotificationType;
			if (notificationType != null)
			{
				if (Network.<>f__switch$map77 == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
					dictionary.Add("WTCG.UtilNotificationMessage", 0);
					Network.<>f__switch$map77 = dictionary;
				}
				int num;
				if (Network.<>f__switch$map77.TryGetValue(notificationType, ref num))
				{
					if (num == 0)
					{
						PegasusPacket pegasusPacket = new PegasusPacket(bnetNotification.MessageType, 0, bnetNotification.MessageSize, bnetNotification.BlobMessage);
						ConnectAPI.QueueUtilNotificationPegasusPacket(pegasusPacket);
					}
				}
			}
		}
	}

	// Token: 0x060000BE RID: 190 RVA: 0x00005E20 File Offset: 0x00004020
	private void ProcessFriends()
	{
		FriendsInfo friendsInfo = default(FriendsInfo);
		BattleNet.GetFriendsInfo(ref friendsInfo);
		if (friendsInfo.updateSize == 0)
		{
			return;
		}
		if (this.m_friendsHandler == null)
		{
			return;
		}
		FriendsUpdate[] array = new FriendsUpdate[friendsInfo.updateSize];
		BattleNet.GetFriendsUpdates(array);
		this.m_friendsHandler(array);
		BattleNet.ClearFriendsUpdates();
	}

	// Token: 0x060000BF RID: 191 RVA: 0x00005E7C File Offset: 0x0000407C
	private void ProcessPresence()
	{
		int num = BattleNet.PresenceSize();
		if (num == 0)
		{
			return;
		}
		if (this.m_presenceHandler == null)
		{
			return;
		}
		PresenceUpdate[] array = new PresenceUpdate[num];
		BattleNet.GetPresence(array);
		this.m_presenceHandler(array);
		BattleNet.ClearPresence();
	}

	// Token: 0x060000C0 RID: 192 RVA: 0x00005EC0 File Offset: 0x000040C0
	private void ProcessChallenges()
	{
		int num = BattleNet.NumChallenges();
		if (num == 0)
		{
			return;
		}
		if (this.m_challengeHandler == null)
		{
			return;
		}
		ChallengeInfo[] array = new ChallengeInfo[num];
		BattleNet.GetChallenges(array);
		this.m_challengeHandler(array);
		BattleNet.ClearChallenges();
	}

	// Token: 0x060000C1 RID: 193 RVA: 0x00005F04 File Offset: 0x00004104
	private void ProcessErrors()
	{
		this.ProcessDelayedError();
		BnetErrorInfo[] array;
		if (ConnectAPI.HasErrors())
		{
			BnetErrorInfo bnetErrorInfo = new BnetErrorInfo(2, 18, 34200);
			array = new BnetErrorInfo[]
			{
				bnetErrorInfo
			};
		}
		else
		{
			int errorsCount = BattleNet.GetErrorsCount();
			if (errorsCount == 0)
			{
				return;
			}
			array = new BnetErrorInfo[errorsCount];
			BattleNet.GetErrors(array);
		}
		foreach (BnetErrorInfo bnetErrorInfo2 in array)
		{
			BattleNetErrors error = bnetErrorInfo2.GetError();
			string text = (!ApplicationMgr.IsPublic()) ? error.ToString() : string.Empty;
			if (!ConnectAPI.HasErrors() && ConnectAPI.ShouldIgnoreError(bnetErrorInfo2))
			{
				if (!ApplicationMgr.IsPublic())
				{
					Log.BattleNet.PrintDebug("BattleNet/ConnectDLL generated error={0} {1} (can ignore)", new object[]
					{
						error,
						text
					});
				}
			}
			else if (!this.FireErrorListeners(bnetErrorInfo2))
			{
				if (ConnectAPI.HasErrors() || !this.OnIgnorableBnetError(bnetErrorInfo2))
				{
					this.OnFatalBnetError(bnetErrorInfo2);
				}
			}
		}
		BattleNet.ClearErrors();
	}

	// Token: 0x060000C2 RID: 194 RVA: 0x00006024 File Offset: 0x00004224
	private bool FireErrorListeners(BnetErrorInfo info)
	{
		bool flag = false;
		List<Network.BnetErrorListener> list;
		if (this.m_featureBnetErrorListeners.TryGetValue(info.GetFeature(), out list) && list.Count > 0)
		{
			foreach (Network.BnetErrorListener bnetErrorListener in list.ToArray())
			{
				flag = (bnetErrorListener.Fire(info) || flag);
			}
		}
		foreach (Network.BnetErrorListener bnetErrorListener2 in this.m_globalBnetErrorListeners.ToArray())
		{
			flag = (bnetErrorListener2.Fire(info) || flag);
		}
		return flag;
	}

	// Token: 0x060000C3 RID: 195 RVA: 0x000060C9 File Offset: 0x000042C9
	public void ShowConnectionFailureError(string error)
	{
		this.ShowBreakingNewsOrError(error, this.DelayForConnectionFailures(Network.s_numConnectionFailures++));
	}

	// Token: 0x060000C4 RID: 196 RVA: 0x000060E5 File Offset: 0x000042E5
	public void ShowBreakingNewsOrError(string error, float timeBeforeAllowReset = 0f)
	{
		this.m_delayedError = error;
		this.m_timeBeforeAllowReset = timeBeforeAllowReset;
		Debug.LogError(string.Format("Setting delayed error for Error Message: {0} and prevent reset for {1} seconds", error, timeBeforeAllowReset));
		this.ProcessDelayedError();
	}

	// Token: 0x060000C5 RID: 197 RVA: 0x00006114 File Offset: 0x00004314
	private bool ProcessDelayedError()
	{
		if (this.m_delayedError == null)
		{
			return false;
		}
		bool result = false;
		bool flag = BreakingNews.Get().GetStatus() != BreakingNews.Status.Fetching;
		if (flag)
		{
			ErrorParams errorParams = new ErrorParams();
			errorParams.m_delayBeforeNextReset = this.m_timeBeforeAllowReset;
			string text = BreakingNews.Get().GetText();
			if (string.IsNullOrEmpty(text))
			{
				if (BreakingNews.Get().GetError() != null && this.m_delayedError == "GLOBAL_ERROR_NETWORK_NO_GAME_SERVER")
				{
					errorParams.m_message = GameStrings.Format("GLOBAL_ERROR_NETWORK_NO_CONNECTION", new object[0]);
				}
				else if (ApplicationMgr.IsInternal() && this.m_delayedError == "GLOBAL_ERROR_UNKNOWN_ERROR")
				{
					errorParams.m_message = "Dev Message: Could not connect to Battle.net, and there was no breaking news to display. Maybe Battle.net is down?";
				}
				else
				{
					errorParams.m_message = GameStrings.Format(this.m_delayedError, new object[0]);
				}
			}
			else
			{
				errorParams.m_message = GameStrings.Format("GLOBAL_MOBILE_ERROR_BREAKING_NEWS", new object[]
				{
					text
				});
			}
			Error.AddFatal(errorParams);
			this.m_delayedError = null;
			this.m_timeBeforeAllowReset = 0f;
			result = true;
		}
		return result;
	}

	// Token: 0x060000C6 RID: 198 RVA: 0x00006230 File Offset: 0x00004430
	public bool OnIgnorableBnetError(BnetErrorInfo info)
	{
		BattleNetErrors error = info.GetError();
		BIReport.Get().Report_Telemetry(Telemetry.Level.LEVEL_ERROR, BIReport.TelemetryEvent.EVENT_IGNORABLE_BNET_ERROR, error, null);
		BattleNetErrors battleNetErrors = error;
		if (battleNetErrors == 0)
		{
			return true;
		}
		if (battleNetErrors == 9)
		{
			return true;
		}
		if (battleNetErrors == 20)
		{
			return info.GetFeature() == 1 && info.GetFeatureEvent() == 9;
		}
		if (battleNetErrors == 27)
		{
			return info.GetFeature() == 5;
		}
		if (battleNetErrors == 39)
		{
			Locale locale = Localization.GetLocale();
			if (locale == Locale.zhCN)
			{
				this.m_logSource.LogError("Network.IgnoreBnetError() - error={0} locale={1}", new object[]
				{
					info,
					locale
				});
			}
			return true;
		}
		if (battleNetErrors == 45)
		{
			return true;
		}
		if (battleNetErrors == 4005)
		{
			return true;
		}
		if (battleNetErrors != 34200)
		{
			return false;
		}
		this.m_logSource.LogError("Network.IgnoreBnetError() - error={0}", new object[]
		{
			info
		});
		return true;
	}

	// Token: 0x060000C7 RID: 199 RVA: 0x00006320 File Offset: 0x00004520
	public void OnFatalBnetError(BnetErrorInfo info)
	{
		BattleNetErrors error = info.GetError();
		this.m_logSource.LogError("Network.OnFatalBnetError() - error={0}", new object[]
		{
			info
		});
		BIReport.Get().Report_Telemetry(Telemetry.Level.LEVEL_ERROR, BIReport.TelemetryEvent.EVENT_FATAL_BNET_ERROR, error, null);
		BattleNetErrors battleNetErrors = error;
		switch (battleNetErrors)
		{
		case 3003:
			break;
		case 3004:
			BIReport.Get().Report_Telemetry(Telemetry.Level.LEVEL_ERROR, BIReport.TelemetryEvent.EVENT_ERROR_UNKNOWN_ERROR, 1, info.ToString());
			this.ShowConnectionFailureError("GLOBAL_ERROR_UNKNOWN_ERROR");
			Log.JMac.Print(LogLevel.Warning, string.Format("ERROR_RPC_PEER_UNAVAILABLE - {0} connection failures.", Network.s_numConnectionFailures), new object[0]);
			return;
		case 3005:
		{
			string text = "GLOBAL_ERROR_NETWORK_DISCONNECT";
			this.ShowConnectionFailureError(text);
			return;
		}
		case 3006:
		{
			string text = "GLOBAL_ERROR_NETWORK_UTIL_TIMEOUT";
			this.ShowConnectionFailureError(text);
			return;
		}
		case 3007:
			this.ShowConnectionFailureError("GLOBAL_ERROR_NETWORK_CONNECTION_TIMEOUT");
			return;
		default:
			if (battleNetErrors == 52)
			{
				WebAuth.DeleteStoredToken();
				WebAuth.DeleteCookies();
				Error.AddFatalLoc("GLOBAL_ERROR_NETWORK_ACCOUNT_BANNED", new object[0]);
				return;
			}
			if (battleNetErrors == 53)
			{
				WebAuth.DeleteStoredToken();
				WebAuth.DeleteCookies();
				Error.AddFatalLoc("GLOBAL_ERROR_NETWORK_ACCOUNT_SUSPENDED", new object[0]);
				return;
			}
			if (battleNetErrors == 60)
			{
				Error.AddFatalLoc("GLOBAL_ERROR_NETWORK_DUPLICATE_LOGIN", new object[0]);
				return;
			}
			if (battleNetErrors == 61)
			{
				string text = "GLOBAL_ERROR_NETWORK_DISCONNECT";
				Error.AddFatalLoc(text, new object[0]);
				return;
			}
			if (battleNetErrors == 3)
			{
				this.ShowConnectionFailureError("GLOBAL_ERROR_NETWORK_LOGIN_FAILURE");
				return;
			}
			if (battleNetErrors == 11)
			{
				WebAuth.DeleteStoredToken();
				WebAuth.DeleteCookies();
				Error.AddFatalLoc("GLOBAL_ERROR_NETWORK_PARENTAL_CONTROLS", new object[0]);
				return;
			}
			if (battleNetErrors == 33)
			{
				BIReport.Get().Report_Telemetry(Telemetry.Level.LEVEL_ERROR, BIReport.TelemetryEvent.EVENT_ERROR_UNKNOWN_ERROR, 0, info.ToString());
				this.ShowConnectionFailureError("GLOBAL_ERROR_UNKNOWN_ERROR");
				Log.JMac.Print(LogLevel.Warning, string.Format("ERROR_SERVER_IS_PRIVATE - {0} connection failures.", Network.s_numConnectionFailures), new object[0]);
				return;
			}
			if (battleNetErrors == 43)
			{
				Error.AddFatalLoc("GLOBAL_ERROR_NETWORK_PHONE_LOCK", new object[0]);
				return;
			}
			if (battleNetErrors == 521)
			{
				this.ShowConnectionFailureError("GLOBAL_MOBILE_ERROR_LOGON_WEB_TIMEOUT");
				return;
			}
			if (battleNetErrors == 42003)
			{
				WebAuth.DeleteStoredToken();
				WebAuth.DeleteCookies();
				Error.AddFatalLoc("GLOBAL_ERROR_NETWORK_RISK_ACCOUNT_LOCKED", new object[0]);
				return;
			}
			break;
		case 3014:
		{
			string text = "GLOBAL_ERROR_NETWORK_SPAM";
			Error.AddFatalLoc(text, new object[0]);
			return;
		}
		}
		string error2;
		if (ApplicationMgr.IsInternal())
		{
			error2 = string.Format("Unhandled Bnet Error: {0}", info);
		}
		else
		{
			Debug.LogError(string.Format("Unhandled Bnet Error: {0}", info));
			error2 = GameStrings.Format("GLOBAL_ERROR_UNKNOWN_ERROR", new object[0]);
		}
		BIReport.Get().Report_Telemetry(Telemetry.Level.LEVEL_ERROR, BIReport.TelemetryEvent.EVENT_ERROR_UNKNOWN_ERROR, 2, info.ToString());
		this.ShowConnectionFailureError(error2);
	}

	// Token: 0x060000C8 RID: 200 RVA: 0x00006618 File Offset: 0x00004818
	private float DelayForConnectionFailures(int numFailures)
	{
		float num = (float)(new Random().NextDouble() * 3.0) + 3.5f;
		return (float)Math.Min(numFailures, 3) * num;
	}

	// Token: 0x060000C9 RID: 201 RVA: 0x0000664B File Offset: 0x0000484B
	public void AnswerChallenge(ulong challengeID, string answer)
	{
		BattleNet.AnswerChallenge(challengeID, answer);
	}

	// Token: 0x060000CA RID: 202 RVA: 0x00006654 File Offset: 0x00004854
	public void CancelChallenge(ulong challengeID)
	{
		BattleNet.CancelChallenge(challengeID);
	}

	// Token: 0x060000CB RID: 203 RVA: 0x0000665C File Offset: 0x0000485C
	private bool OnBnetAuthError(BnetErrorInfo info, object userData)
	{
		return false;
	}

	// Token: 0x060000CC RID: 204 RVA: 0x0000665F File Offset: 0x0000485F
	public static void AcceptFriendInvite(BnetInvitationId inviteid)
	{
		BattleNet.ManageFriendInvite(1, inviteid.GetVal());
	}

	// Token: 0x060000CD RID: 205 RVA: 0x0000666D File Offset: 0x0000486D
	public static void RevokeFriendInvite(BnetInvitationId inviteid)
	{
		BattleNet.ManageFriendInvite(2, inviteid.GetVal());
	}

	// Token: 0x060000CE RID: 206 RVA: 0x0000667B File Offset: 0x0000487B
	public static void DeclineFriendInvite(BnetInvitationId inviteid)
	{
		BattleNet.ManageFriendInvite(3, inviteid.GetVal());
	}

	// Token: 0x060000CF RID: 207 RVA: 0x00006689 File Offset: 0x00004889
	public static void IgnoreFriendInvite(BnetInvitationId inviteid)
	{
		BattleNet.ManageFriendInvite(4, inviteid.GetVal());
	}

	// Token: 0x060000D0 RID: 208 RVA: 0x00006697 File Offset: 0x00004897
	private static void SendFriendInvite(string sender, string target, bool byEmail)
	{
		BattleNet.SendFriendInvite(sender, target, byEmail);
	}

	// Token: 0x060000D1 RID: 209 RVA: 0x000066A1 File Offset: 0x000048A1
	public static void SendFriendInviteByEmail(string sender, string target)
	{
		Network.SendFriendInvite(sender, target, true);
	}

	// Token: 0x060000D2 RID: 210 RVA: 0x000066AB File Offset: 0x000048AB
	public static void SendFriendInviteByBattleTag(string sender, string target)
	{
		Network.SendFriendInvite(sender, target, false);
	}

	// Token: 0x060000D3 RID: 211 RVA: 0x000066B5 File Offset: 0x000048B5
	public static void RemoveFriend(BnetAccountId id)
	{
		BattleNet.RemoveFriend(id);
	}

	// Token: 0x060000D4 RID: 212 RVA: 0x000066BD File Offset: 0x000048BD
	public static void SendWhisper(BnetGameAccountId gameAccount, string message)
	{
		BattleNet.SendWhisper(gameAccount, message);
	}

	// Token: 0x060000D5 RID: 213 RVA: 0x000066C8 File Offset: 0x000048C8
	public static void SendFriendChallenge(BnetGameAccountId gameAccountId, int scenarioId, FormatType formatType)
	{
		EntityId entityId = BnetEntityId.CreateEntityId(gameAccountId);
		BattleNet.SendFriendlyChallengeInvite(ref entityId, scenarioId, formatType);
	}

	// Token: 0x060000D6 RID: 214 RVA: 0x000066E8 File Offset: 0x000048E8
	public static void ChooseFriendChallengeDeck(BnetEntityId partyId, long deckID)
	{
		EntityId entityId = BnetEntityId.CreateEntityId(partyId);
		BattleNet.SetPartyDeck(ref entityId, deckID);
	}

	// Token: 0x060000D7 RID: 215 RVA: 0x00006704 File Offset: 0x00004904
	public static void RescindFriendChallenge(BnetEntityId partyId)
	{
		EntityId entityId = BnetEntityId.CreateEntityId(partyId);
		BattleNet.RescindFriendlyChallenge(ref entityId);
	}

	// Token: 0x060000D8 RID: 216 RVA: 0x00006720 File Offset: 0x00004920
	public static void AcceptFriendChallenge(BnetEntityId partyId)
	{
		EntityId entityId = BnetEntityId.CreateEntityId(partyId);
		BattleNet.AcceptFriendlyChallenge(ref entityId);
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x0000673C File Offset: 0x0000493C
	public static void DeclineFriendChallenge(BnetEntityId partyId)
	{
		EntityId entityId = BnetEntityId.CreateEntityId(partyId);
		BattleNet.DeclineFriendlyChallenge(ref entityId);
	}

	// Token: 0x060000DA RID: 218 RVA: 0x00006757 File Offset: 0x00004957
	public void GotoGameServer(GameServerInfo info, bool reconnecting)
	{
		this.m_lastGameServerInfo = info;
		ConnectAPI.GotoGameServer(info, reconnecting);
	}

	// Token: 0x060000DB RID: 219 RVA: 0x00006767 File Offset: 0x00004967
	public void SpectateSecondPlayer(GameServerInfo info)
	{
		ConnectAPI.SpectateSecondPlayer(info);
	}

	// Token: 0x060000DC RID: 220 RVA: 0x0000676F File Offset: 0x0000496F
	public bool RetryGotoGameServer()
	{
		return ConnectAPI.RetryGotoGameServer(this.m_lastGameServerInfo);
	}

	// Token: 0x060000DD RID: 221 RVA: 0x0000677C File Offset: 0x0000497C
	public GameServerInfo GetLastGameServerJoined()
	{
		return this.m_lastGameServerInfo;
	}

	// Token: 0x060000DE RID: 222 RVA: 0x00006784 File Offset: 0x00004984
	public void ClearLastGameServerJoined()
	{
		this.m_lastGameServerInfo = null;
	}

	// Token: 0x060000DF RID: 223 RVA: 0x0000678D File Offset: 0x0000498D
	public static Network Get()
	{
		return Network.s_instance;
	}

	// Token: 0x060000E0 RID: 224 RVA: 0x00006794 File Offset: 0x00004994
	public static string GetUsername()
	{
		string text = null;
		try
		{
			text = Network.GetStoredUserName();
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception while loading settings: " + ex.Message);
		}
		if (text == null)
		{
			text = Vars.Key("Aurora.Username").GetStr("NOT_PROVIDED_PLEASE_PROVIDE_VIA_CONFIG");
		}
		if (text != null && text.IndexOf("@") == -1)
		{
			text += "@blizzard.com";
		}
		return text;
	}

	// Token: 0x060000E1 RID: 225 RVA: 0x00006818 File Offset: 0x00004A18
	public static string GetTargetServer()
	{
		bool flag = Vars.Key("Aurora.Env.Override").GetInt(0) != 0;
		string text = "default";
		string text2 = null;
		if (flag)
		{
			text2 = Vars.Key("Aurora.Env").GetStr(text);
			if (string.IsNullOrEmpty(text2))
			{
				text2 = null;
			}
		}
		if (text2 == null)
		{
			string launchOption = BattleNet.GetLaunchOption("REGION", false);
			if (!string.IsNullOrEmpty(launchOption))
			{
				string text3 = launchOption;
				if (text3 != null)
				{
					if (Network.<>f__switch$map78 == null)
					{
						Dictionary<string, int> dictionary = new Dictionary<string, int>(5);
						dictionary.Add("US", 0);
						dictionary.Add("XX", 1);
						dictionary.Add("EU", 2);
						dictionary.Add("CN", 3);
						dictionary.Add("KR", 4);
						Network.<>f__switch$map78 = dictionary;
					}
					int num;
					if (Network.<>f__switch$map78.TryGetValue(text3, ref num))
					{
						switch (num)
						{
						case 0:
							text2 = "us.actual.battle.net";
							goto IL_13A;
						case 1:
							text2 = "beta.actual.battle.net";
							goto IL_13A;
						case 2:
							text2 = "hearthmod.com";
							goto IL_13A;
						case 3:
							text2 = "cn.actual.battle.net";
							goto IL_13A;
						case 4:
							text2 = "kr.actual.battle.net";
							goto IL_13A;
						}
					}
				}
				text2 = text;
			}
		}
		IL_13A:
		if (text2.ToLower() == text)
		{
			text2 = "bn11-01.battle.net";
		}
		return text2;
	}

	// Token: 0x060000E2 RID: 226 RVA: 0x0000697C File Offset: 0x00004B7C
	public static int GetPort()
	{
		int num = 0;
		bool flag = Vars.Key("Aurora.Env.Override").GetInt(0) != 0;
		if (flag)
		{
			num = Vars.Key("Aurora.Port").GetInt(0);
		}
		if (num == 0)
		{
			num = 1119;
		}
		return num;
	}

	// Token: 0x060000E3 RID: 227 RVA: 0x000069C8 File Offset: 0x00004BC8
	private static SslParameters GetSSLParams()
	{
		SslParameters sslParameters = new SslParameters();
		TextAsset textAsset = (TextAsset)Resources.Load("SSLCert/ssl_cert_bundle");
		if (textAsset != null)
		{
			sslParameters.bundleSettings.bundle = new SslCertBundle(textAsset.bytes);
		}
		sslParameters.bundleSettings.bundleDownloadConfig.numRetries = 3;
		sslParameters.bundleSettings.bundleDownloadConfig.timeoutMs = -1;
		return sslParameters;
	}

	// Token: 0x060000E4 RID: 228 RVA: 0x00006A30 File Offset: 0x00004C30
	public static bool IsVersionInt()
	{
		return Network.GetIsVersionIntFromConfig();
	}

	// Token: 0x060000E5 RID: 229 RVA: 0x00006A48 File Offset: 0x00004C48
	public static bool GetIsVersionIntFromConfig()
	{
		string text = Vars.Key("Aurora.Version.Source").GetStr("undefined");
		if (text == "undefined")
		{
			text = "product";
		}
		return text == "product" || text == "int" || !(text == "string");
	}

	// Token: 0x060000E6 RID: 230 RVA: 0x00006AB8 File Offset: 0x00004CB8
	public static string GetVersion()
	{
		return Network.GetVersionFromConfig();
	}

	// Token: 0x060000E7 RID: 231 RVA: 0x00006AD0 File Offset: 0x00004CD0
	private static string GetVersionFromConfig()
	{
		string text = Vars.Key("Aurora.Version.Source").GetStr("undefined");
		if (text == "undefined")
		{
			text = "product";
		}
		string text2;
		if (text == "product")
		{
			text2 = Network.ProductVersion().ToString();
		}
		else if (text == "int")
		{
			int maxValue = int.MaxValue;
			int @int = Vars.Key("Aurora.Version.Int").GetInt(maxValue);
			if (@int == maxValue)
			{
				Debug.LogError("Aurora.Version.Int undefined");
			}
			text2 = @int.ToString();
		}
		else if (text == "string")
		{
			string text3 = "undefined";
			text2 = Vars.Key("Aurora.Version.String").GetStr(text3);
			if (text2 == text3)
			{
				Debug.LogError("Aurora.Version.String undefined");
			}
		}
		else
		{
			Debug.LogError("unknown version source: " + text);
			text2 = "0";
		}
		return text2;
	}

	// Token: 0x060000E8 RID: 232 RVA: 0x00006BCF File Offset: 0x00004DCF
	public void OnLoginStarted()
	{
		ConnectAPI.OnLoginStarted();
		this.m_loginStarted = DateTime.Now;
	}

	// Token: 0x060000E9 RID: 233 RVA: 0x00006BE1 File Offset: 0x00004DE1
	public static QueueEvent GetQueueEvent()
	{
		return BattleNet.GetQueueEvent();
	}

	// Token: 0x060000EA RID: 234 RVA: 0x00006BE8 File Offset: 0x00004DE8
	public bool IsFindingGame()
	{
		return this.m_findingBnetGameType != 0;
	}

	// Token: 0x060000EB RID: 235 RVA: 0x00006BF6 File Offset: 0x00004DF6
	public BnetGameType GetFindingBnetGameType()
	{
		return this.m_findingBnetGameType;
	}

	// Token: 0x060000EC RID: 236 RVA: 0x00006C00 File Offset: 0x00004E00
	public static BnetGameType TranslateGameTypeToBnet(GameType gameType, int missionId)
	{
		switch (gameType)
		{
		case 1:
			return 4;
		case 2:
			return 1;
		default:
			if (gameType != 16)
			{
				Error.AddDevFatal("Network.TranslateGameTypeToBnet() - do not know how to translate {0}", new object[]
				{
					gameType
				});
				return 0;
			}
			if (GameUtils.IsAIMission(missionId))
			{
				return 17;
			}
			if (GameUtils.IsCoopMission(missionId))
			{
				return 18;
			}
			return 16;
		case 4:
			return 5;
		case 5:
			return 3;
		case 7:
			if (global::Options.Get().GetBool(Option.IN_WILD_MODE))
			{
				return 30;
			}
			return 2;
		case 8:
			if (global::Options.Get().GetBool(Option.IN_WILD_MODE))
			{
				return 31;
			}
			if (NetCache.Get().GetNetObject<NetCache.NetCachePlayQueue>().GameType == 9)
			{
				return 9;
			}
			return 10;
		}
	}

	// Token: 0x060000ED RID: 237 RVA: 0x00006CD4 File Offset: 0x00004ED4
	public void FindGame(GameType gameType, int missionId, long deckId, long aiDeckId)
	{
		if (gameType == 2)
		{
			Error.AddDevFatal("Network.FindGame() - friend games must go through the Party API", new object[0]);
			return;
		}
		BnetGameType bnetGameType = Network.TranslateGameTypeToBnet(gameType, missionId);
		if (bnetGameType == null)
		{
			return;
		}
		this.m_findingBnetGameType = bnetGameType;
		bool flag = Network.RequiresScenarioIdAttribute(bnetGameType);
		BattleNet.FindGame(bnetGameType, missionId, deckId, aiDeckId, flag);
	}

	// Token: 0x060000EE RID: 238 RVA: 0x00006D24 File Offset: 0x00004F24
	private static bool RequiresScenarioIdAttribute(BnetGameType bnetGameType)
	{
		switch (bnetGameType)
		{
		case 16:
		case 17:
		case 18:
			break;
		default:
			if (bnetGameType != 1)
			{
				return false;
			}
			break;
		}
		return true;
	}

	// Token: 0x060000EF RID: 239 RVA: 0x00006D58 File Offset: 0x00004F58
	public void CancelFindGame()
	{
		if (this.m_findingBnetGameType == null)
		{
			return;
		}
		BnetGameType findingBnetGameType = this.GetFindingBnetGameType();
		if (!this.IsNoAccountTutorialGame(findingBnetGameType))
		{
			BattleNet.CancelFindGame();
		}
		this.m_findingBnetGameType = 0;
	}

	// Token: 0x060000F0 RID: 240 RVA: 0x00006D90 File Offset: 0x00004F90
	private bool IsNoAccountTutorialGame(BnetGameType gameType)
	{
		return !Network.ShouldBeConnectedToAurora() && gameType == 5;
	}

	// Token: 0x060000F1 RID: 241 RVA: 0x00006DA8 File Offset: 0x00004FA8
	private void ResolveAddressAndGotoGameServer(GameServerInfo gameServer)
	{
		IPAddress ipaddress;
		if (IPAddress.TryParse(gameServer.Address, ref ipaddress))
		{
			gameServer.Address = ipaddress.ToString();
			Network.Get().GotoGameServer(gameServer, false);
			return;
		}
		try
		{
			IPHostEntry hostEntry = Dns.GetHostEntry(gameServer.Address);
			IPAddress[] addressList = hostEntry.AddressList;
			int num = 0;
			if (num < addressList.Length)
			{
				IPAddress ipaddress2 = addressList[num];
				gameServer.Address = ipaddress2.ToString();
				Network.Get().GotoGameServer(gameServer, false);
				return;
			}
		}
		catch (Exception ex)
		{
			this.m_logSource.LogError("Exception within ResolveAddressAndGotoGameServer: " + ex.Message);
		}
		this.ThrowDnsResolveError(gameServer.Address);
	}

	// Token: 0x060000F2 RID: 242 RVA: 0x00006E74 File Offset: 0x00005074
	private void ThrowDnsResolveError(string environment)
	{
		if (ApplicationMgr.IsInternal())
		{
			Error.AddDevFatal("Environment " + environment + " could not be resolved! Please check your environment and Internet connection!", new object[0]);
		}
		else
		{
			Error.AddFatalLoc("GLOBAL_ERROR_NETWORK_NO_CONNECTION", new object[0]);
		}
	}

	// Token: 0x060000F3 RID: 243 RVA: 0x00006EBB File Offset: 0x000050BB
	public static Network.GameCancelInfo GetGameCancelInfo()
	{
		return ConnectAPI.GetGameCancelInfo();
	}

	// Token: 0x060000F4 RID: 244 RVA: 0x00006EC2 File Offset: 0x000050C2
	public void GetGameState()
	{
		ConnectAPI.GetGameState();
	}

	// Token: 0x060000F5 RID: 245 RVA: 0x00006EC9 File Offset: 0x000050C9
	public static Network.TurnTimerInfo GetTurnTimerInfo()
	{
		return ConnectAPI.GetTurnTimerInfo();
	}

	// Token: 0x060000F6 RID: 246 RVA: 0x00006ED0 File Offset: 0x000050D0
	public static int GetNAckOption()
	{
		return ConnectAPI.GetNAckOption();
	}

	// Token: 0x060000F7 RID: 247 RVA: 0x00006ED7 File Offset: 0x000050D7
	public static SpectatorNotify GetSpectatorNotify()
	{
		return ConnectAPI.GetSpectatorNotify();
	}

	// Token: 0x060000F8 RID: 248 RVA: 0x00006EDE File Offset: 0x000050DE
	public static void DisconnectFromGameServer()
	{
		if (!Network.IsConnectedToGameServer())
		{
			return;
		}
		ConnectAPI.DisconnectFromGameServer();
	}

	// Token: 0x060000F9 RID: 249 RVA: 0x00006EF0 File Offset: 0x000050F0
	public static bool WasDisconnectRequested()
	{
		return ConnectAPI.WasDisconnectRequested();
	}

	// Token: 0x060000FA RID: 250 RVA: 0x00006EF7 File Offset: 0x000050F7
	public static bool IsConnectedToGameServer()
	{
		return ConnectAPI.IsConnectedToGameServer();
	}

	// Token: 0x060000FB RID: 251 RVA: 0x00006EFE File Offset: 0x000050FE
	public static bool WasGameConceded()
	{
		return ConnectAPI.WasGameConceded();
	}

	// Token: 0x060000FC RID: 252 RVA: 0x00006F05 File Offset: 0x00005105
	public static void Concede()
	{
		ConnectAPI.Concede();
	}

	// Token: 0x060000FD RID: 253 RVA: 0x00006F0C File Offset: 0x0000510C
	public static void AutoConcede()
	{
		if (!Network.IsConnectedToGameServer())
		{
			return;
		}
		if (Network.WasGameConceded())
		{
			return;
		}
		Network.Concede();
	}

	// Token: 0x060000FE RID: 254 RVA: 0x00006F29 File Offset: 0x00005129
	public static Network.EntityChoices GetEntityChoices()
	{
		return ConnectAPI.GetEntityChoices();
	}

	// Token: 0x060000FF RID: 255 RVA: 0x00006F30 File Offset: 0x00005130
	public static Network.EntitiesChosen GetEntitiesChosen()
	{
		return ConnectAPI.GetEntitiesChosen();
	}

	// Token: 0x06000100 RID: 256 RVA: 0x00006F37 File Offset: 0x00005137
	public void SendChoices(int ID, List<int> picks)
	{
		ConnectAPI.SendChoices(ID, picks);
	}

	// Token: 0x06000101 RID: 257 RVA: 0x00006F40 File Offset: 0x00005140
	public void SendOption(int ID, int index, int target, int sub, int pos)
	{
		ConnectAPI.SendOption(ID, index, target, sub, pos);
	}

	// Token: 0x06000102 RID: 258 RVA: 0x00006F4E File Offset: 0x0000514E
	public static Network.Options GetOptions()
	{
		return ConnectAPI.GetOptions();
	}

	// Token: 0x06000103 RID: 259 RVA: 0x00006F58 File Offset: 0x00005158
	public void SendUserUI(int overCard, int heldCard, int arrowOrigin, int x, int y)
	{
		NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
		int showUserUI = netObject.Games.ShowUserUI;
		if (showUserUI != 0)
		{
			ConnectAPI.SendUserUI(overCard, heldCard, arrowOrigin, x, y);
		}
	}

	// Token: 0x06000104 RID: 260 RVA: 0x00006F8E File Offset: 0x0000518E
	public void SendEmote(EmoteType emote)
	{
		ConnectAPI.SendEmote((int)emote);
	}

	// Token: 0x06000105 RID: 261 RVA: 0x00006F96 File Offset: 0x00005196
	public void SendSpectatorInvite(BnetAccountId bnetAccountId, BnetGameAccountId bnetGameAccountId)
	{
		ConnectAPI.SendSpectatorInvite(bnetAccountId, bnetGameAccountId);
	}

	// Token: 0x06000106 RID: 262 RVA: 0x00006F9F File Offset: 0x0000519F
	public void SendRemoveSpectators(bool regenerateSpectatorPassword, params BnetGameAccountId[] bnetGameAccountIds)
	{
		ConnectAPI.SendRemoveSpectators(regenerateSpectatorPassword, bnetGameAccountIds);
	}

	// Token: 0x06000107 RID: 263 RVA: 0x00006FA8 File Offset: 0x000051A8
	public void SendRemoveAllSpectators(bool regenerateSpectatorPassword)
	{
		ConnectAPI.SendRemoveAllSpectators(regenerateSpectatorPassword);
	}

	// Token: 0x06000108 RID: 264 RVA: 0x00006FB0 File Offset: 0x000051B0
	public static Network.UserUI GetUserUI()
	{
		return ConnectAPI.GetUserUI();
	}

	// Token: 0x06000109 RID: 265 RVA: 0x00006FB7 File Offset: 0x000051B7
	public static Network.GameSetup GetGameSetupInfo()
	{
		return ConnectAPI.GetGameSetup();
	}

	// Token: 0x0600010A RID: 266 RVA: 0x00006FBE File Offset: 0x000051BE
	private static void IntArrayToIntPtr(int[] src, IntPtr dst, int length)
	{
		Marshal.Copy(src, 0, dst, length);
	}

	// Token: 0x0600010B RID: 267 RVA: 0x00006FC9 File Offset: 0x000051C9
	private static void LongArrayToIntPtr(long[] src, IntPtr dst, int length)
	{
		Marshal.Copy(src, 0, dst, length);
	}

	// Token: 0x0600010C RID: 268 RVA: 0x00006FD4 File Offset: 0x000051D4
	private static int[] IntPtrToIntArray(IntPtr src, int length)
	{
		int[] array = new int[length];
		Marshal.Copy(src, array, 0, length);
		return array;
	}

	// Token: 0x0600010D RID: 269 RVA: 0x00006FF4 File Offset: 0x000051F4
	private static long[] IntPtrToLongArray(IntPtr src, int length)
	{
		long[] array = new long[length];
		Marshal.Copy(src, array, 0, length);
		return array;
	}

	// Token: 0x0600010E RID: 270 RVA: 0x00007012 File Offset: 0x00005212
	public static List<int> GetIntArray(List<int> ints, int size)
	{
		return ints;
	}

	// Token: 0x0600010F RID: 271 RVA: 0x00007018 File Offset: 0x00005218
	public static List<int> GetIntArray(IntPtr ints, int size)
	{
		List<int> list = new List<int>();
		int[] array = Network.IntPtrToIntArray(ints, size);
		for (int i = 0; i < size; i++)
		{
			list.Add(array[i]);
		}
		return list;
	}

	// Token: 0x06000110 RID: 272 RVA: 0x0000704F File Offset: 0x0000524F
	public static List<Network.Entity.Tag> GetTags(List<Network.Entity.Tag> tags)
	{
		return tags;
	}

	// Token: 0x06000111 RID: 273 RVA: 0x00007054 File Offset: 0x00005254
	public static List<Network.Entity.Tag> GetTags(IntPtr src, IntPtr flags)
	{
		List<Network.Entity.Tag> list = new List<Network.Entity.Tag>();
		int[] array = Network.IntPtrToIntArray(src, 512);
		int[] array2 = Network.IntPtrToIntArray(flags, 512);
		for (int i = 0; i < 512; i++)
		{
			if (array2[i] == 1)
			{
				list.Add(new Network.Entity.Tag
				{
					Name = i,
					Value = array[i]
				});
			}
		}
		return list;
	}

	// Token: 0x06000112 RID: 274 RVA: 0x000070C0 File Offset: 0x000052C0
	private static void SetTags(List<Network.Entity.Tag> dst, IntPtr src)
	{
		int[] array = Network.IntPtrToIntArray(src, 512);
		for (int i = 0; i < 512; i++)
		{
			if (array[i] != 0)
			{
				dst.Add(new Network.Entity.Tag
				{
					Name = i,
					Value = array[i]
				});
			}
		}
	}

	// Token: 0x06000113 RID: 275 RVA: 0x00007114 File Offset: 0x00005314
	public static List<Network.PowerHistory> GetPowerHistory()
	{
		return ConnectAPI.GetPowerHistory();
	}

	// Token: 0x06000114 RID: 276 RVA: 0x0000711C File Offset: 0x0000531C
	private static List<int> MakeChoicesList(int choice1, int choice2, int choice3)
	{
		List<int> list = new List<int>();
		if (choice1 == 0)
		{
			return null;
		}
		list.Add(choice1);
		if (choice2 == 0)
		{
			return list;
		}
		list.Add(choice2);
		if (choice3 == 0)
		{
			return list;
		}
		list.Add(choice3);
		return list;
	}

	// Token: 0x06000115 RID: 277 RVA: 0x0000715D File Offset: 0x0000535D
	public static void RequestAchieves(bool activeOrNewCompleteOnly)
	{
		ConnectAPI.RequestAchieves(activeOrNewCompleteOnly);
	}

	// Token: 0x06000116 RID: 278 RVA: 0x00007165 File Offset: 0x00005365
	public static Network.AchieveList Achieves()
	{
		return ConnectAPI.GetAchieves();
	}

	// Token: 0x06000117 RID: 279 RVA: 0x0000716C File Offset: 0x0000536C
	public static void ValidateAchieve(int achieveID)
	{
		ConnectAPI.ValidateAchieve(achieveID);
	}

	// Token: 0x06000118 RID: 280 RVA: 0x00007174 File Offset: 0x00005374
	public static ValidateAchieveResponse GetValidatedAchieve()
	{
		return ConnectAPI.GetValidatedAchieve();
	}

	// Token: 0x06000119 RID: 281 RVA: 0x0000717B File Offset: 0x0000537B
	public static void RequestCancelQuest(int achieveID)
	{
		ConnectAPI.RequestCancelQuest(achieveID);
	}

	// Token: 0x0600011A RID: 282 RVA: 0x00007183 File Offset: 0x00005383
	public static Network.CanceledQuest GetCanceledQuest()
	{
		return ConnectAPI.GetCanceledQuest();
	}

	// Token: 0x0600011B RID: 283 RVA: 0x0000718A File Offset: 0x0000538A
	public static Network.TriggeredEvent GetTriggerEventResponse()
	{
		return ConnectAPI.GetTriggerEventResponse();
	}

	// Token: 0x0600011C RID: 284 RVA: 0x00007191 File Offset: 0x00005391
	public static void RequestAdventureProgress()
	{
		ConnectAPI.RequestAdventureProgress();
	}

	// Token: 0x0600011D RID: 285 RVA: 0x00007198 File Offset: 0x00005398
	public static List<Network.AdventureProgress> GetAdventureProgressResponse()
	{
		return ConnectAPI.GetAdventureProgressResponse();
	}

	// Token: 0x0600011E RID: 286 RVA: 0x0000719F File Offset: 0x0000539F
	public static Network.BeginDraft GetNewDraftDeckID()
	{
		return ConnectAPI.DraftGetBeginning();
	}

	// Token: 0x0600011F RID: 287 RVA: 0x000071A6 File Offset: 0x000053A6
	public static Network.DraftError GetDraftError()
	{
		return (Network.DraftError)ConnectAPI.DraftGetError();
	}

	// Token: 0x06000120 RID: 288 RVA: 0x000071AD File Offset: 0x000053AD
	public static Network.DraftChoicesAndContents GetDraftChoicesAndContents()
	{
		return ConnectAPI.DraftGetChoicesAndContents();
	}

	// Token: 0x06000121 RID: 289 RVA: 0x000071B4 File Offset: 0x000053B4
	public static Network.DraftChosen GetChosenAndNext()
	{
		return ConnectAPI.DraftCardChosen();
	}

	// Token: 0x06000122 RID: 290 RVA: 0x000071BB File Offset: 0x000053BB
	public static void MakeDraftChoice(long deckID, int slot, int index)
	{
		ConnectAPI.DraftMakePick(deckID, slot, index);
	}

	// Token: 0x06000123 RID: 291 RVA: 0x000071C5 File Offset: 0x000053C5
	public static void FindOutCurrentDraftState()
	{
		ConnectAPI.DraftGetPicksAndContents();
	}

	// Token: 0x06000124 RID: 292 RVA: 0x000071CC File Offset: 0x000053CC
	public static void StartANewDraft()
	{
		ConnectAPI.DraftBegin();
	}

	// Token: 0x06000125 RID: 293 RVA: 0x000071D3 File Offset: 0x000053D3
	public static void RetireDraftDeck(long deckID, int slot)
	{
		ConnectAPI.DraftRetire(deckID, slot);
	}

	// Token: 0x06000126 RID: 294 RVA: 0x000071DC File Offset: 0x000053DC
	public static Network.DraftRetired GetRetiredDraft()
	{
		return ConnectAPI.DraftHandleRetired();
	}

	// Token: 0x06000127 RID: 295 RVA: 0x000071E3 File Offset: 0x000053E3
	public static void AckDraftRewards(long deckID, int slot)
	{
		ConnectAPI.DraftAckRewards(deckID, slot);
	}

	// Token: 0x06000128 RID: 296 RVA: 0x000071EC File Offset: 0x000053EC
	public static long GetRewardsAckDraftID()
	{
		return ConnectAPI.DraftHandleRewardsAck();
	}

	// Token: 0x06000129 RID: 297 RVA: 0x000071F3 File Offset: 0x000053F3
	public static void MassDisenchant()
	{
		ConnectAPI.MassDisenchant();
	}

	// Token: 0x0600012A RID: 298 RVA: 0x000071FA File Offset: 0x000053FA
	public static Network.MassDisenchantResponse GetMassDisenchantResponse()
	{
		return ConnectAPI.GetMassDisenchantResponse();
	}

	// Token: 0x0600012B RID: 299 RVA: 0x00007201 File Offset: 0x00005401
	public static void SetFavoriteHero(TAG_CLASS heroClass, NetCache.CardDefinition hero)
	{
		ConnectAPI.SetFavoriteHero(heroClass, hero);
	}

	// Token: 0x0600012C RID: 300 RVA: 0x0000720A File Offset: 0x0000540A
	public static Network.SetFavoriteHeroResponse GetSetFavoriteHeroResponse()
	{
		return ConnectAPI.GetSetFavoriteHeroResponse();
	}

	// Token: 0x0600012D RID: 301 RVA: 0x00007211 File Offset: 0x00005411
	public static void RequestBattlePayStatus()
	{
		ConnectAPI.RequestBattlePayStatus();
	}

	// Token: 0x0600012E RID: 302 RVA: 0x00007218 File Offset: 0x00005418
	public static Network.PurchaseCanceledResponse GetPurchaseCanceledResponse()
	{
		return ConnectAPI.GetPurchaseCanceledResponse();
	}

	// Token: 0x0600012F RID: 303 RVA: 0x0000721F File Offset: 0x0000541F
	public static Network.BattlePayStatus GetBattlePayStatusResponse()
	{
		return ConnectAPI.GetBattlePayStatusResponse();
	}

	// Token: 0x06000130 RID: 304 RVA: 0x00007226 File Offset: 0x00005426
	public static void RequestBattlePayConfig()
	{
		ConnectAPI.RequestBattlePayConfig();
	}

	// Token: 0x06000131 RID: 305 RVA: 0x0000722D File Offset: 0x0000542D
	public static Network.BattlePayConfig GetBattlePayConfigResponse()
	{
		return ConnectAPI.GetBattlePayConfigResponse();
	}

	// Token: 0x06000132 RID: 306 RVA: 0x00007234 File Offset: 0x00005434
	public static void PurchaseViaGold(int quantity, ProductType product, int data)
	{
		ConnectAPI.PurchaseViaGold(quantity, product, data);
	}

	// Token: 0x06000133 RID: 307 RVA: 0x0000723E File Offset: 0x0000543E
	public static void GetPurchaseMethod(string productID, int quantity, Currency currency)
	{
		ConnectAPI.RequestPurchaseMethod(productID, quantity, (int)currency);
	}

	// Token: 0x06000134 RID: 308 RVA: 0x00007248 File Offset: 0x00005448
	public static void ConfirmPurchase()
	{
		ConnectAPI.ConfirmPurchase();
	}

	// Token: 0x06000135 RID: 309 RVA: 0x0000724F File Offset: 0x0000544F
	public static void BeginThirdPartyPurchase(BattlePayProvider provider, string productID, int quantity)
	{
		ConnectAPI.BeginThirdPartyPurchase(provider, productID, quantity);
	}

	// Token: 0x06000136 RID: 310 RVA: 0x00007259 File Offset: 0x00005459
	public static void BeginThirdPartyPurchaseWithReceipt(BattlePayProvider provider, string productID, int quantity, string thirdPartyID, string base64receipt, string thirdPartyUserID = "")
	{
		ConnectAPI.BeginThirdPartyPurchaseWithReceipt(provider, productID, quantity, thirdPartyID, base64receipt, thirdPartyUserID);
	}

	// Token: 0x06000137 RID: 311 RVA: 0x00007268 File Offset: 0x00005468
	public static void SubmitThirdPartyReceipt(long bpayID, string thirdPartyID, string base64receipt, string thirdPartyUserID = "")
	{
		ConnectAPI.SubmitThirdPartyPurchaseReceipt(bpayID, thirdPartyID, base64receipt, thirdPartyUserID);
	}

	// Token: 0x06000138 RID: 312 RVA: 0x00007273 File Offset: 0x00005473
	public static void GetThirdPartyPurchaseStatus(string transactionID)
	{
		ConnectAPI.GetThirdPartyPurchaseStatus(transactionID);
	}

	// Token: 0x06000139 RID: 313 RVA: 0x0000727B File Offset: 0x0000547B
	public static void CancelBlizzardPurchase(bool isAutoCanceled, CancelPurchase.CancelReason? reason, string error)
	{
		ConnectAPI.AbortBlizzardPurchase(isAutoCanceled, reason, error);
	}

	// Token: 0x0600013A RID: 314 RVA: 0x00007285 File Offset: 0x00005485
	public static void CancelThirdPartyPurchase(CancelPurchase.CancelReason reason, string error)
	{
		ConnectAPI.AbortThirdPartyPurchase(reason, error);
	}

	// Token: 0x0600013B RID: 315 RVA: 0x0000728E File Offset: 0x0000548E
	public static Network.PurchaseMethod GetPurchaseMethodResponse()
	{
		return ConnectAPI.GetPurchaseMethodResponse();
	}

	// Token: 0x0600013C RID: 316 RVA: 0x00007295 File Offset: 0x00005495
	public static Network.PurchaseResponse GetPurchaseResponse()
	{
		return ConnectAPI.GetPurchaseResponse();
	}

	// Token: 0x0600013D RID: 317 RVA: 0x0000729C File Offset: 0x0000549C
	public static Network.PurchaseViaGoldResponse GetPurchaseWithGoldResponse()
	{
		return ConnectAPI.GetPurchaseWithGoldResponse();
	}

	// Token: 0x0600013E RID: 318 RVA: 0x000072A3 File Offset: 0x000054A3
	public static Network.ThirdPartyPurchaseStatusResponse GetThirdPartyPurchaseStatusResponse()
	{
		return ConnectAPI.GetThirdPartyPurchaseStatusResponse();
	}

	// Token: 0x0600013F RID: 319 RVA: 0x000072AA File Offset: 0x000054AA
	public static void OpenBooster(int id)
	{
		Log.Bob.Print("Network.OpenBooster", new object[0]);
		ConnectAPI.OpenBooster(id);
	}

	// Token: 0x06000140 RID: 320 RVA: 0x000072C8 File Offset: 0x000054C8
	public void CreateDeck(DeckType deckType, string name, int heroDatabaseAssetID, TAG_PREMIUM heroPremium, bool isWild)
	{
		Log.Rachelle.Print(string.Format("Network.CreateDeck hero={0},premium={1}", heroDatabaseAssetID, heroPremium), new object[0]);
		ConnectAPI.CreateDeck(deckType, name, heroDatabaseAssetID, heroPremium, isWild);
	}

	// Token: 0x06000141 RID: 321 RVA: 0x00007308 File Offset: 0x00005508
	public void RenameDeck(long deck, string name)
	{
		Log.Rachelle.Print(string.Format("Network.RenameDeck {0}", deck), new object[0]);
		ConnectAPI.RenameDeck(deck, name);
	}

	// Token: 0x06000142 RID: 322 RVA: 0x00007334 File Offset: 0x00005534
	public void DeleteDeck(long deck)
	{
		Log.Rachelle.Print(string.Format("Network.DeleteDeck {0}", deck), new object[0]);
		ConnectAPI.DeleteDeck(deck);
	}

	// Token: 0x06000143 RID: 323 RVA: 0x00007368 File Offset: 0x00005568
	public void RequestDeckContents(params long[] deckIds)
	{
		Logger bob = Log.Bob;
		string format = "Network.GetDeckContents {0}";
		object[] array = new object[1];
		array[0] = string.Join(", ", Enumerable.ToArray<string>(Enumerable.Select<long, string>(deckIds, (long id) => id.ToString())));
		bob.Print(format, array);
		ConnectAPI.RequestDeckContents(deckIds);
	}

	// Token: 0x06000144 RID: 324 RVA: 0x000073C5 File Offset: 0x000055C5
	public void SetDeckTemplateSource(long deck, int templateID)
	{
		Log.Bob.Print(string.Format("Network.SendDeckTemplateSource {0}, {1}", deck, templateID), new object[0]);
		ConnectAPI.SendDeckTemplateSource(deck, templateID);
	}

	// Token: 0x06000145 RID: 325 RVA: 0x000073F4 File Offset: 0x000055F4
	public void SetDeckContents(long deck, List<Network.CardUserData> cards, int newHeroAssetID, TAG_PREMIUM newHeroCardPremium, int newCardBackID, bool isWild)
	{
		Log.Bob.Print(string.Format("Network.DeckSetUserData {0}", deck), new object[0]);
		ConnectAPI.SendDeckData(deck, cards, newHeroAssetID, newHeroCardPremium, newCardBackID, isWild);
	}

	// Token: 0x06000146 RID: 326 RVA: 0x0000742F File Offset: 0x0000562F
	public static GetDeckContentsResponse GetDeckContentsResponse()
	{
		return ConnectAPI.GetDeckContentsResponse();
	}

	// Token: 0x06000147 RID: 327 RVA: 0x00007436 File Offset: 0x00005636
	public static List<NetCache.BoosterCard> OpenedBooster()
	{
		return ConnectAPI.GetOpenedBooster();
	}

	// Token: 0x06000148 RID: 328 RVA: 0x0000743D File Offset: 0x0000563D
	public static Network.DBAction GetDeckResponse()
	{
		return ConnectAPI.DBAction();
	}

	// Token: 0x06000149 RID: 329 RVA: 0x00007444 File Offset: 0x00005644
	public static NetCache.DeckHeader GetCreatedDeck()
	{
		return ConnectAPI.DeckCreated();
	}

	// Token: 0x0600014A RID: 330 RVA: 0x0000744B File Offset: 0x0000564B
	public static long GetDeletedDeckID()
	{
		return ConnectAPI.DeckDeleted();
	}

	// Token: 0x0600014B RID: 331 RVA: 0x00007452 File Offset: 0x00005652
	public static Network.DeckName GetRenamedDeck()
	{
		return ConnectAPI.DeckRenamed();
	}

	// Token: 0x0600014C RID: 332 RVA: 0x00007459 File Offset: 0x00005659
	public static void AckNotice(long ID)
	{
		if (!NetCache.Get().RemoveNotice(ID))
		{
			return;
		}
		ConnectAPI.AckNotice(ID);
	}

	// Token: 0x0600014D RID: 333 RVA: 0x00007472 File Offset: 0x00005672
	public static void AckAchieveProgress(int ID, int ackProgress)
	{
		ConnectAPI.AckAchieveProgress(ID, ackProgress);
	}

	// Token: 0x0600014E RID: 334 RVA: 0x0000747B File Offset: 0x0000567B
	public static void CheckAccountLicenseAchieve(int achieveID)
	{
		ConnectAPI.CheckAccountLicenseAchieve(achieveID);
	}

	// Token: 0x0600014F RID: 335 RVA: 0x00007483 File Offset: 0x00005683
	public static Network.AccountLicenseAchieveResponse GetAccountLicenseAchieveResponse()
	{
		return ConnectAPI.GetAccountLicenseAchieveResponse();
	}

	// Token: 0x06000150 RID: 336 RVA: 0x0000748A File Offset: 0x0000568A
	public static void AckCardSeenBefore(int assetID, TAG_PREMIUM premium)
	{
		ConnectAPI.AckCardSeen(assetID, (int)premium);
	}

	// Token: 0x06000151 RID: 337 RVA: 0x00007493 File Offset: 0x00005693
	public static void AckWingProgress(int wingId, int ackId)
	{
		ConnectAPI.AckWingProgress(wingId, ackId);
	}

	// Token: 0x06000152 RID: 338 RVA: 0x0000749C File Offset: 0x0000569C
	public static void AcknowledgeBanner(int banner)
	{
		ConnectAPI.AcknowledgeBanner(banner);
	}

	// Token: 0x06000153 RID: 339 RVA: 0x000074A4 File Offset: 0x000056A4
	public static void SetAdventureOptions(int id, ulong options)
	{
		ConnectAPI.SetAdventureOptions(id, options);
	}

	// Token: 0x06000154 RID: 340 RVA: 0x000074AD File Offset: 0x000056AD
	public static void SendAckCardsSeen()
	{
		ConnectAPI.SendAckCardsSeen();
	}

	// Token: 0x06000155 RID: 341 RVA: 0x000074B4 File Offset: 0x000056B4
	public static Network.CardSaleResult GetCardSaleResult()
	{
		return ConnectAPI.GetCardSaleResult();
	}

	// Token: 0x06000156 RID: 342 RVA: 0x000074BC File Offset: 0x000056BC
	public static void TriggerLaunchEvent(BnetGameAccountId lastOpponentHSGameAccountID, ulong lastOpponentSessionStartTime, BnetGameAccountId otherPlayerHSGameAccountID, ulong otherPlayerSessionStartTime)
	{
		ConnectAPI.TriggerLaunchEvent(lastOpponentHSGameAccountID.GetHi(), lastOpponentHSGameAccountID.GetLo(), lastOpponentSessionStartTime, otherPlayerHSGameAccountID.GetHi(), otherPlayerHSGameAccountID.GetLo(), otherPlayerSessionStartTime);
	}

	// Token: 0x06000157 RID: 343 RVA: 0x000074E8 File Offset: 0x000056E8
	public static void RequestAssetsVersion()
	{
		ConnectAPI.RequestAssetsVersion();
	}

	// Token: 0x06000158 RID: 344 RVA: 0x000074EF File Offset: 0x000056EF
	public static void LoginOk()
	{
		ConnectAPI.OnLoginCompleted();
	}

	// Token: 0x06000159 RID: 345 RVA: 0x000074F6 File Offset: 0x000056F6
	public static int GetAssetsVersion()
	{
		return ConnectAPI.GetAssetsVersion();
	}

	// Token: 0x0600015A RID: 346 RVA: 0x00007500 File Offset: 0x00005700
	public static bool SendConsoleCmdToServer(string command)
	{
		if (!Network.IsConnectedToGameServer())
		{
			Log.Rachelle.Print(string.Format("Cannot send command '{0}' to server; no game server is active.", command), new object[0]);
			return false;
		}
		ConnectAPI.SendIndirectConsoleCommand(command);
		return true;
	}

	// Token: 0x0600015B RID: 347 RVA: 0x0000753B File Offset: 0x0000573B
	private static string GetStoredUserName()
	{
		return null;
	}

	// Token: 0x0600015C RID: 348 RVA: 0x0000753E File Offset: 0x0000573E
	private static string GetStoredBNetIP()
	{
		return null;
	}

	// Token: 0x0600015D RID: 349 RVA: 0x00007541 File Offset: 0x00005741
	private static string GetStoredVersion()
	{
		return null;
	}

	// Token: 0x0400002A RID: 42
	public const int NoSubOption = -1;

	// Token: 0x0400002B RID: 43
	public const int NoPosition = 0;

	// Token: 0x0400002C RID: 44
	public const string CosmeticVersion = "5.0";

	// Token: 0x0400002D RID: 45
	public const string VersionPostfix = "";

	// Token: 0x0400002E RID: 46
	public const string DEFAULT_INTERNAL_ENVIRONMENT = "bn12-01.battle.net";

	// Token: 0x0400002F RID: 47
	public const string DEFAULT_PUBLIC_ENVIRONMENT = "us.actual.battle.net";

	// Token: 0x04000030 RID: 48
	private const int MIN_DEFERRED_WAIT = 30;

	// Token: 0x04000031 RID: 49
	public static string TutorialServer = "02";

	// Token: 0x04000032 RID: 50
	private static readonly TimeSpan LOGIN_TIMEOUT = new TimeSpan(0, 0, 5);

	// Token: 0x04000033 RID: 51
	private static readonly TimeSpan PROCESS_WARNING = new TimeSpan(0, 0, 15);

	// Token: 0x04000034 RID: 52
	private static readonly TimeSpan PROCESS_WARNING_REPORT_GAP = new TimeSpan(0, 0, 1);

	// Token: 0x04000035 RID: 53
	private static TimeSpan MAX_DEFERRED_WAIT = new TimeSpan(0, 0, 120);

	// Token: 0x04000036 RID: 54
	private static Network s_instance = new Network();

	// Token: 0x04000037 RID: 55
	private static bool s_running;

	// Token: 0x04000038 RID: 56
	private BattleNetLogSource m_logSource = new BattleNetLogSource("Network");

	// Token: 0x04000039 RID: 57
	private static UnityUrlDownloader s_urlDownloader = new UnityUrlDownloader();

	// Token: 0x0400003A RID: 58
	private Map<int, List<Network.NetHandler>> m_netHandlers = new Map<int, List<Network.NetHandler>>();

	// Token: 0x0400003B RID: 59
	private bool m_loginWaiting = true;

	// Token: 0x0400003C RID: 60
	private DateTime m_loginStarted;

	// Token: 0x0400003D RID: 61
	private BnetGameType m_findingBnetGameType;

	// Token: 0x0400003E RID: 62
	private DateTime lastCall = DateTime.Now;

	// Token: 0x0400003F RID: 63
	private DateTime lastCallReport = DateTime.Now;

	// Token: 0x04000040 RID: 64
	private Network.QueueInfoHandler m_queueInfoHandler;

	// Token: 0x04000041 RID: 65
	private Network.GameQueueHandler m_gameQueueHandler;

	// Token: 0x04000042 RID: 66
	private GameServerInfo m_lastGameServerInfo;

	// Token: 0x04000043 RID: 67
	private string m_delayedError;

	// Token: 0x04000044 RID: 68
	private float m_timeBeforeAllowReset;

	// Token: 0x04000045 RID: 69
	private static int s_numConnectionFailures = 0;

	// Token: 0x04000046 RID: 70
	private Map<int, Network.TimeoutHandler> m_netTimeoutHandlers = new Map<int, Network.TimeoutHandler>();

	// Token: 0x04000047 RID: 71
	private static List<Network.RequestContext> m_inTransitRequests = new List<Network.RequestContext>();

	// Token: 0x04000048 RID: 72
	private static SortedDictionary<int, int> m_deferredMessageResponseMap;

	// Token: 0x04000049 RID: 73
	private static SortedDictionary<int, int> m_deferredGetAccountInfoMessageResponseMap;

	// Token: 0x0400004A RID: 74
	public static readonly PlatformDependentValue<bool> LAUNCHES_WITH_BNET_APP;

	// Token: 0x0400004B RID: 75
	public static readonly PlatformDependentValue<bool> TUTORIALS_WITHOUT_ACCOUNT;

	// Token: 0x0400004C RID: 76
	private static bool s_shouldBeConnectedToAurora;

	// Token: 0x0400004D RID: 77
	private Network.BnetEventHandler m_bnetEventHandler;

	// Token: 0x0400004E RID: 78
	private Network.FriendsHandler m_friendsHandler;

	// Token: 0x0400004F RID: 79
	private Network.WhisperHandler m_whisperHandler;

	// Token: 0x04000050 RID: 80
	private Network.PartyHandler m_partyHandler;

	// Token: 0x04000051 RID: 81
	private Network.PresenceHandler m_presenceHandler;

	// Token: 0x04000052 RID: 82
	private Network.ShutdownHandler m_shutdownHandler;

	// Token: 0x04000053 RID: 83
	private Network.ChallengeHandler m_challengeHandler;

	// Token: 0x04000054 RID: 84
	private Network.SpectatorInviteReceivedHandler m_spectatorInviteReceivedHandler = delegate(Invite A_0)
	{
	};

	// Token: 0x04000055 RID: 85
	private Map<BnetFeature, List<Network.BnetErrorListener>> m_featureBnetErrorListeners = new Map<BnetFeature, List<Network.BnetErrorListener>>();

	// Token: 0x04000056 RID: 86
	private List<Network.BnetErrorListener> m_globalBnetErrorListeners = new List<Network.BnetErrorListener>();

	// Token: 0x04000057 RID: 87
	private Network.GameServerDisconnectEvent m_gameServerDisconnectEventListener;

	// Token: 0x0200000C RID: 12
	// (Invoke) Token: 0x06000161 RID: 353
	public delegate void NetHandler();

	// Token: 0x0200000D RID: 13
	// (Invoke) Token: 0x06000165 RID: 357
	public delegate void TimeoutHandler(int pendingResponseId, int requestId, int requestSubId);

	// Token: 0x0200002A RID: 42
	public class AdventureProgress
	{
		// Token: 0x060003E6 RID: 998 RVA: 0x00012098 File Offset: 0x00010298
		public AdventureProgress()
		{
			this.Wing = 0;
			this.Progress = 0;
			this.Ack = 0;
			this.Flags = 0UL;
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060003E7 RID: 999 RVA: 0x000120C8 File Offset: 0x000102C8
		// (set) Token: 0x060003E8 RID: 1000 RVA: 0x000120D0 File Offset: 0x000102D0
		public int Wing { get; set; }

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060003E9 RID: 1001 RVA: 0x000120D9 File Offset: 0x000102D9
		// (set) Token: 0x060003EA RID: 1002 RVA: 0x000120E1 File Offset: 0x000102E1
		public int Progress { get; set; }

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060003EB RID: 1003 RVA: 0x000120EA File Offset: 0x000102EA
		// (set) Token: 0x060003EC RID: 1004 RVA: 0x000120F2 File Offset: 0x000102F2
		public int Ack { get; set; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060003ED RID: 1005 RVA: 0x000120FB File Offset: 0x000102FB
		// (set) Token: 0x060003EE RID: 1006 RVA: 0x00012103 File Offset: 0x00010303
		public ulong Flags { get; set; }
	}

	// Token: 0x02000036 RID: 54
	// (Invoke) Token: 0x0600044D RID: 1101
	public delegate void PartyHandler(PartyEvent[] updates);

	// Token: 0x02000037 RID: 55
	public class RecruitInfo
	{
		// Token: 0x06000450 RID: 1104 RVA: 0x00013148 File Offset: 0x00011348
		public RecruitInfo()
		{
			this.ID = 0UL;
			this.RecruitID = new BnetAccountId();
			this.Nickname = string.Empty;
			this.Status = 0;
			this.Level = 0;
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000451 RID: 1105 RVA: 0x00013187 File Offset: 0x00011387
		// (set) Token: 0x06000452 RID: 1106 RVA: 0x0001318F File Offset: 0x0001138F
		public ulong ID { get; set; }

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000453 RID: 1107 RVA: 0x00013198 File Offset: 0x00011398
		// (set) Token: 0x06000454 RID: 1108 RVA: 0x000131A0 File Offset: 0x000113A0
		public BnetAccountId RecruitID { get; set; }

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000455 RID: 1109 RVA: 0x000131A9 File Offset: 0x000113A9
		// (set) Token: 0x06000456 RID: 1110 RVA: 0x000131B1 File Offset: 0x000113B1
		public string Nickname { get; set; }

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000457 RID: 1111 RVA: 0x000131BA File Offset: 0x000113BA
		// (set) Token: 0x06000458 RID: 1112 RVA: 0x000131C2 File Offset: 0x000113C2
		public int Status { get; set; }

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000459 RID: 1113 RVA: 0x000131CB File Offset: 0x000113CB
		// (set) Token: 0x0600045A RID: 1114 RVA: 0x000131D3 File Offset: 0x000113D3
		public int Level { get; set; }

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x0600045B RID: 1115 RVA: 0x000131DC File Offset: 0x000113DC
		// (set) Token: 0x0600045C RID: 1116 RVA: 0x000131E4 File Offset: 0x000113E4
		public ulong CreationTimeMicrosec { get; set; }

		// Token: 0x0600045D RID: 1117 RVA: 0x000131F0 File Offset: 0x000113F0
		public override string ToString()
		{
			return string.Format("[RecruitInfo: ID={0}, RecruitID={1}, Nickname={2}, Status={3}, Level={4}]", new object[]
			{
				this.ID,
				this.RecruitID,
				this.Nickname,
				this.Status,
				this.Level
			});
		}
	}

	// Token: 0x02000038 RID: 56
	// (Invoke) Token: 0x0600045F RID: 1119
	public delegate bool BnetErrorCallback(BnetErrorInfo info, object userData);

	// Token: 0x02000039 RID: 57
	// (Invoke) Token: 0x06000463 RID: 1123
	public delegate void ShutdownHandler(int minutes);

	// Token: 0x0200003A RID: 58
	public class CardUserData
	{
		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000467 RID: 1127 RVA: 0x00013251 File Offset: 0x00011451
		// (set) Token: 0x06000468 RID: 1128 RVA: 0x00013259 File Offset: 0x00011459
		public int DbId { get; set; }

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000469 RID: 1129 RVA: 0x00013262 File Offset: 0x00011462
		// (set) Token: 0x0600046A RID: 1130 RVA: 0x0001326A File Offset: 0x0001146A
		public int Count { get; set; }

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x0600046B RID: 1131 RVA: 0x00013273 File Offset: 0x00011473
		// (set) Token: 0x0600046C RID: 1132 RVA: 0x0001327B File Offset: 0x0001147B
		public TAG_PREMIUM Premium { get; set; }
	}

	// Token: 0x0200003C RID: 60
	public class DeckContents
	{
		// Token: 0x0600046D RID: 1133 RVA: 0x00013284 File Offset: 0x00011484
		public DeckContents()
		{
			this.Cards = new List<Network.CardUserData>();
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x0600046E RID: 1134 RVA: 0x00013297 File Offset: 0x00011497
		// (set) Token: 0x0600046F RID: 1135 RVA: 0x0001329F File Offset: 0x0001149F
		public long Deck { get; set; }

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000470 RID: 1136 RVA: 0x000132A8 File Offset: 0x000114A8
		// (set) Token: 0x06000471 RID: 1137 RVA: 0x000132B0 File Offset: 0x000114B0
		public List<Network.CardUserData> Cards { get; set; }

		// Token: 0x06000472 RID: 1138 RVA: 0x000132BC File Offset: 0x000114BC
		public static Network.DeckContents FromPacket(PegasusUtil.DeckContents packet)
		{
			Network.DeckContents deckContents = new Network.DeckContents();
			deckContents.Deck = packet.DeckId;
			foreach (DeckCardData deckCardData in packet.Cards)
			{
				Network.CardUserData cardUserData = new Network.CardUserData();
				cardUserData.DbId = deckCardData.Def.Asset;
				cardUserData.Count = ((!deckCardData.HasQty) ? 1 : deckCardData.Qty);
				cardUserData.Premium = (TAG_PREMIUM)((!deckCardData.Def.HasPremium) ? 0 : deckCardData.Def.Premium);
				deckContents.Cards.Add(cardUserData);
			}
			return deckContents;
		}
	}

	// Token: 0x0200003D RID: 61
	public class CardSaleResult
	{
		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000474 RID: 1140 RVA: 0x00013394 File Offset: 0x00011594
		// (set) Token: 0x06000475 RID: 1141 RVA: 0x0001339C File Offset: 0x0001159C
		public Network.CardSaleResult.SaleResult Action { get; set; }

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000476 RID: 1142 RVA: 0x000133A5 File Offset: 0x000115A5
		// (set) Token: 0x06000477 RID: 1143 RVA: 0x000133AD File Offset: 0x000115AD
		public int AssetID { get; set; }

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000478 RID: 1144 RVA: 0x000133B6 File Offset: 0x000115B6
		// (set) Token: 0x06000479 RID: 1145 RVA: 0x000133BE File Offset: 0x000115BE
		public string AssetName { get; set; }

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x0600047A RID: 1146 RVA: 0x000133C7 File Offset: 0x000115C7
		// (set) Token: 0x0600047B RID: 1147 RVA: 0x000133CF File Offset: 0x000115CF
		public TAG_PREMIUM Premium { get; set; }

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x0600047C RID: 1148 RVA: 0x000133D8 File Offset: 0x000115D8
		// (set) Token: 0x0600047D RID: 1149 RVA: 0x000133E0 File Offset: 0x000115E0
		public int Amount { get; set; }

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x0600047E RID: 1150 RVA: 0x000133E9 File Offset: 0x000115E9
		// (set) Token: 0x0600047F RID: 1151 RVA: 0x000133F1 File Offset: 0x000115F1
		public int Count { get; set; }

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000480 RID: 1152 RVA: 0x000133FA File Offset: 0x000115FA
		// (set) Token: 0x06000481 RID: 1153 RVA: 0x00013402 File Offset: 0x00011602
		public bool Nerfed { get; set; }

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000482 RID: 1154 RVA: 0x0001340B File Offset: 0x0001160B
		// (set) Token: 0x06000483 RID: 1155 RVA: 0x00013413 File Offset: 0x00011613
		public int UnitSellPrice { get; set; }

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000484 RID: 1156 RVA: 0x0001341C File Offset: 0x0001161C
		// (set) Token: 0x06000485 RID: 1157 RVA: 0x00013424 File Offset: 0x00011624
		public int UnitBuyPrice { get; set; }

		// Token: 0x06000486 RID: 1158 RVA: 0x00013430 File Offset: 0x00011630
		public override string ToString()
		{
			return string.Format("[CardSaleResult Action={0} assetName={1} premium={2} amount={3} count={4}]", new object[]
			{
				this.Action,
				this.AssetName,
				this.Premium,
				this.Amount,
				this.Count
			});
		}

		// Token: 0x0200003E RID: 62
		public enum SaleResult
		{
			// Token: 0x040001F5 RID: 501
			GENERIC_FAILURE = 1,
			// Token: 0x040001F6 RID: 502
			CARD_WAS_SOLD,
			// Token: 0x040001F7 RID: 503
			CARD_WAS_BOUGHT,
			// Token: 0x040001F8 RID: 504
			SOULBOUND,
			// Token: 0x040001F9 RID: 505
			FAILED_WRONG_SELL_PRICE,
			// Token: 0x040001FA RID: 506
			FAILED_WRONG_BUY_PRICE,
			// Token: 0x040001FB RID: 507
			FAILED_NO_PERMISSION,
			// Token: 0x040001FC RID: 508
			FAILED_EVENT_NOT_ACTIVE
		}
	}

	// Token: 0x0200003F RID: 63
	public class MassDisenchantResponse
	{
		// Token: 0x06000487 RID: 1159 RVA: 0x0001348E File Offset: 0x0001168E
		public MassDisenchantResponse()
		{
			this.Amount = 0;
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000488 RID: 1160 RVA: 0x0001349D File Offset: 0x0001169D
		// (set) Token: 0x06000489 RID: 1161 RVA: 0x000134A5 File Offset: 0x000116A5
		public int Amount { get; set; }
	}

	// Token: 0x02000040 RID: 64
	public class SetFavoriteHeroResponse
	{
		// Token: 0x0600048A RID: 1162 RVA: 0x000134AE File Offset: 0x000116AE
		public SetFavoriteHeroResponse()
		{
			this.Success = false;
			this.HeroClass = TAG_CLASS.INVALID;
			this.Hero = null;
		}

		// Token: 0x040001FE RID: 510
		public bool Success;

		// Token: 0x040001FF RID: 511
		public TAG_CLASS HeroClass;

		// Token: 0x04000200 RID: 512
		public NetCache.CardDefinition Hero;
	}

	// Token: 0x02000043 RID: 67
	public class CardBackResponse
	{
		// Token: 0x06000493 RID: 1171 RVA: 0x00013572 File Offset: 0x00011772
		public CardBackResponse()
		{
			this.Success = false;
			this.CardBack = 0;
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000494 RID: 1172 RVA: 0x00013588 File Offset: 0x00011788
		// (set) Token: 0x06000495 RID: 1173 RVA: 0x00013590 File Offset: 0x00011790
		public bool Success { get; set; }

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000496 RID: 1174 RVA: 0x00013599 File Offset: 0x00011799
		// (set) Token: 0x06000497 RID: 1175 RVA: 0x000135A1 File Offset: 0x000117A1
		public int CardBack { get; set; }
	}

	// Token: 0x02000044 RID: 68
	public class DBAction
	{
		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000499 RID: 1177 RVA: 0x000135B2 File Offset: 0x000117B2
		// (set) Token: 0x0600049A RID: 1178 RVA: 0x000135BA File Offset: 0x000117BA
		public Network.DBAction.ActionType Action { get; set; }

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x0600049B RID: 1179 RVA: 0x000135C3 File Offset: 0x000117C3
		// (set) Token: 0x0600049C RID: 1180 RVA: 0x000135CB File Offset: 0x000117CB
		public Network.DBAction.ResultType Result { get; set; }

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x0600049D RID: 1181 RVA: 0x000135D4 File Offset: 0x000117D4
		// (set) Token: 0x0600049E RID: 1182 RVA: 0x000135DC File Offset: 0x000117DC
		public long MetaData { get; set; }

		// Token: 0x02000045 RID: 69
		public enum ActionType
		{
			// Token: 0x04000216 RID: 534
			UNKNOWN,
			// Token: 0x04000217 RID: 535
			GET_DECK,
			// Token: 0x04000218 RID: 536
			CREATE_DECK,
			// Token: 0x04000219 RID: 537
			RENAME_DECK,
			// Token: 0x0400021A RID: 538
			DELETE_DECK,
			// Token: 0x0400021B RID: 539
			SET_DECK,
			// Token: 0x0400021C RID: 540
			OPEN_BOOSTER,
			// Token: 0x0400021D RID: 541
			GAMES_INFO
		}

		// Token: 0x02000046 RID: 70
		public enum ResultType
		{
			// Token: 0x0400021F RID: 543
			UNKNOWN,
			// Token: 0x04000220 RID: 544
			SUCCESS,
			// Token: 0x04000221 RID: 545
			NOT_OWNED,
			// Token: 0x04000222 RID: 546
			CONSTRAINT
		}
	}

	// Token: 0x02000047 RID: 71
	public class DeckName
	{
		// Token: 0x17000084 RID: 132
		// (get) Token: 0x060004A0 RID: 1184 RVA: 0x000135ED File Offset: 0x000117ED
		// (set) Token: 0x060004A1 RID: 1185 RVA: 0x000135F5 File Offset: 0x000117F5
		public long Deck { get; set; }

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x060004A2 RID: 1186 RVA: 0x000135FE File Offset: 0x000117FE
		// (set) Token: 0x060004A3 RID: 1187 RVA: 0x00013606 File Offset: 0x00011806
		public string Name { get; set; }
	}

	// Token: 0x02000048 RID: 72
	public class RewardChest
	{
		// Token: 0x060004A4 RID: 1188 RVA: 0x0001360F File Offset: 0x0001180F
		public RewardChest()
		{
			this.Rewards = new List<RewardData>();
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x060004A5 RID: 1189 RVA: 0x00013622 File Offset: 0x00011822
		// (set) Token: 0x060004A6 RID: 1190 RVA: 0x0001362A File Offset: 0x0001182A
		public List<RewardData> Rewards { get; set; }
	}

	// Token: 0x02000049 RID: 73
	public class Bundle
	{
		// Token: 0x060004A7 RID: 1191 RVA: 0x00013634 File Offset: 0x00011834
		public Bundle()
		{
			this.ProductID = string.Empty;
			this.Cost = default(double?);
			this.GoldCost = default(long?);
			this.AppleID = string.Empty;
			this.GooglePlayID = string.Empty;
			this.AmazonID = string.Empty;
			this.Items = new List<Network.BundleItem>();
			this.ProductEvent = SpecialEventType.UNKNOWN;
			this.RealMoneyProductEvent = SpecialEventType.UNKNOWN;
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x060004A8 RID: 1192 RVA: 0x000136AA File Offset: 0x000118AA
		// (set) Token: 0x060004A9 RID: 1193 RVA: 0x000136B2 File Offset: 0x000118B2
		public string ProductID { get; set; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x060004AA RID: 1194 RVA: 0x000136BB File Offset: 0x000118BB
		// (set) Token: 0x060004AB RID: 1195 RVA: 0x000136C3 File Offset: 0x000118C3
		public double? Cost { get; set; }

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x060004AC RID: 1196 RVA: 0x000136CC File Offset: 0x000118CC
		// (set) Token: 0x060004AD RID: 1197 RVA: 0x000136D4 File Offset: 0x000118D4
		public long? GoldCost { get; set; }

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x060004AE RID: 1198 RVA: 0x000136DD File Offset: 0x000118DD
		// (set) Token: 0x060004AF RID: 1199 RVA: 0x000136E5 File Offset: 0x000118E5
		public string AppleID { get; set; }

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x060004B0 RID: 1200 RVA: 0x000136EE File Offset: 0x000118EE
		// (set) Token: 0x060004B1 RID: 1201 RVA: 0x000136F6 File Offset: 0x000118F6
		public string GooglePlayID { get; set; }

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060004B2 RID: 1202 RVA: 0x000136FF File Offset: 0x000118FF
		// (set) Token: 0x060004B3 RID: 1203 RVA: 0x00013707 File Offset: 0x00011907
		public string AmazonID { get; set; }

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x060004B4 RID: 1204 RVA: 0x00013710 File Offset: 0x00011910
		// (set) Token: 0x060004B5 RID: 1205 RVA: 0x00013718 File Offset: 0x00011918
		public List<Network.BundleItem> Items { get; set; }

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x060004B6 RID: 1206 RVA: 0x00013721 File Offset: 0x00011921
		// (set) Token: 0x060004B7 RID: 1207 RVA: 0x00013729 File Offset: 0x00011929
		public SpecialEventType ProductEvent { get; set; }

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x060004B8 RID: 1208 RVA: 0x00013732 File Offset: 0x00011932
		// (set) Token: 0x060004B9 RID: 1209 RVA: 0x0001373A File Offset: 0x0001193A
		public SpecialEventType RealMoneyProductEvent { get; set; }

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x060004BA RID: 1210 RVA: 0x00013743 File Offset: 0x00011943
		// (set) Token: 0x060004BB RID: 1211 RVA: 0x0001374B File Offset: 0x0001194B
		public List<BattlePayProvider> ExclusiveProviders { get; set; }
	}

	// Token: 0x0200004A RID: 74
	public class BundleItem
	{
		// Token: 0x060004BC RID: 1212 RVA: 0x00013754 File Offset: 0x00011954
		public BundleItem()
		{
			this.Product = 0;
			this.ProductData = 0;
			this.Quantity = 0;
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060004BD RID: 1213 RVA: 0x0001377C File Offset: 0x0001197C
		// (set) Token: 0x060004BE RID: 1214 RVA: 0x00013784 File Offset: 0x00011984
		public ProductType Product { get; set; }

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060004BF RID: 1215 RVA: 0x0001378D File Offset: 0x0001198D
		// (set) Token: 0x060004C0 RID: 1216 RVA: 0x00013795 File Offset: 0x00011995
		public int ProductData { get; set; }

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060004C1 RID: 1217 RVA: 0x0001379E File Offset: 0x0001199E
		// (set) Token: 0x060004C2 RID: 1218 RVA: 0x000137A6 File Offset: 0x000119A6
		public int Quantity { get; set; }

		// Token: 0x060004C3 RID: 1219 RVA: 0x000137B0 File Offset: 0x000119B0
		public override bool Equals(object obj)
		{
			Network.BundleItem bundleItem = obj as Network.BundleItem;
			return bundleItem != null && bundleItem.Product == this.Product && bundleItem.ProductData == this.ProductData && bundleItem.Quantity == this.Quantity;
		}

		// Token: 0x060004C4 RID: 1220 RVA: 0x00013800 File Offset: 0x00011A00
		public override int GetHashCode()
		{
			return this.Product.GetHashCode() * this.ProductData.GetHashCode() * this.Quantity.GetHashCode();
		}
	}

	// Token: 0x0200004B RID: 75
	public class BeginDraft
	{
		// Token: 0x060004C5 RID: 1221 RVA: 0x0001383B File Offset: 0x00011A3B
		public BeginDraft()
		{
			this.Heroes = new List<NetCache.CardDefinition>();
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060004C6 RID: 1222 RVA: 0x0001384E File Offset: 0x00011A4E
		// (set) Token: 0x060004C7 RID: 1223 RVA: 0x00013856 File Offset: 0x00011A56
		public long DeckID { get; set; }

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x060004C8 RID: 1224 RVA: 0x0001385F File Offset: 0x00011A5F
		// (set) Token: 0x060004C9 RID: 1225 RVA: 0x00013867 File Offset: 0x00011A67
		public List<NetCache.CardDefinition> Heroes { get; set; }
	}

	// Token: 0x0200004C RID: 76
	public class DraftRetired
	{
		// Token: 0x060004CA RID: 1226 RVA: 0x00013870 File Offset: 0x00011A70
		public DraftRetired()
		{
			this.Deck = 0L;
			this.Chest = new Network.RewardChest();
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x060004CB RID: 1227 RVA: 0x0001388B File Offset: 0x00011A8B
		// (set) Token: 0x060004CC RID: 1228 RVA: 0x00013893 File Offset: 0x00011A93
		public long Deck { get; set; }

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x060004CD RID: 1229 RVA: 0x0001389C File Offset: 0x00011A9C
		// (set) Token: 0x060004CE RID: 1230 RVA: 0x000138A4 File Offset: 0x00011AA4
		public Network.RewardChest Chest { get; set; }
	}

	// Token: 0x0200004D RID: 77
	public class DraftChoicesAndContents
	{
		// Token: 0x060004CF RID: 1231 RVA: 0x000138B0 File Offset: 0x00011AB0
		public DraftChoicesAndContents()
		{
			this.Choices = new List<NetCache.CardDefinition>();
			this.Hero = new NetCache.CardDefinition();
			this.DeckInfo = new Network.DeckContents();
			this.Chest = null;
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x060004D0 RID: 1232 RVA: 0x000138EB File Offset: 0x00011AEB
		// (set) Token: 0x060004D1 RID: 1233 RVA: 0x000138F3 File Offset: 0x00011AF3
		public int Slot { get; set; }

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x060004D2 RID: 1234 RVA: 0x000138FC File Offset: 0x00011AFC
		// (set) Token: 0x060004D3 RID: 1235 RVA: 0x00013904 File Offset: 0x00011B04
		public List<NetCache.CardDefinition> Choices { get; set; }

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x060004D4 RID: 1236 RVA: 0x0001390D File Offset: 0x00011B0D
		// (set) Token: 0x060004D5 RID: 1237 RVA: 0x00013915 File Offset: 0x00011B15
		public NetCache.CardDefinition Hero { get; set; }

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x060004D6 RID: 1238 RVA: 0x0001391E File Offset: 0x00011B1E
		// (set) Token: 0x060004D7 RID: 1239 RVA: 0x00013926 File Offset: 0x00011B26
		public Network.DeckContents DeckInfo { get; set; }

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x060004D8 RID: 1240 RVA: 0x0001392F File Offset: 0x00011B2F
		// (set) Token: 0x060004D9 RID: 1241 RVA: 0x00013937 File Offset: 0x00011B37
		public int Wins { get; set; }

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x060004DA RID: 1242 RVA: 0x00013940 File Offset: 0x00011B40
		// (set) Token: 0x060004DB RID: 1243 RVA: 0x00013948 File Offset: 0x00011B48
		public int Losses { get; set; }

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x060004DC RID: 1244 RVA: 0x00013951 File Offset: 0x00011B51
		// (set) Token: 0x060004DD RID: 1245 RVA: 0x00013959 File Offset: 0x00011B59
		public Network.RewardChest Chest { get; set; }

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x060004DE RID: 1246 RVA: 0x00013962 File Offset: 0x00011B62
		// (set) Token: 0x060004DF RID: 1247 RVA: 0x0001396A File Offset: 0x00011B6A
		public int MaxWins { get; set; }
	}

	// Token: 0x0200004E RID: 78
	public class DraftChosen
	{
		// Token: 0x060004E0 RID: 1248 RVA: 0x00013973 File Offset: 0x00011B73
		public DraftChosen()
		{
			this.ChosenCard = new NetCache.CardDefinition();
			this.NextChoices = new List<NetCache.CardDefinition>();
		}

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x060004E1 RID: 1249 RVA: 0x00013991 File Offset: 0x00011B91
		// (set) Token: 0x060004E2 RID: 1250 RVA: 0x00013999 File Offset: 0x00011B99
		public NetCache.CardDefinition ChosenCard { get; set; }

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x060004E3 RID: 1251 RVA: 0x000139A2 File Offset: 0x00011BA2
		// (set) Token: 0x060004E4 RID: 1252 RVA: 0x000139AA File Offset: 0x00011BAA
		public List<NetCache.CardDefinition> NextChoices { get; set; }
	}

	// Token: 0x0200004F RID: 79
	public enum DraftError
	{
		// Token: 0x04000242 RID: 578
		DE_UNKNOWN,
		// Token: 0x04000243 RID: 579
		DE_NO_LICENSE,
		// Token: 0x04000244 RID: 580
		DE_RETIRE_FIRST,
		// Token: 0x04000245 RID: 581
		DE_NOT_IN_DRAFT,
		// Token: 0x04000246 RID: 582
		DE_BAD_DECK,
		// Token: 0x04000247 RID: 583
		DE_BAD_SLOT,
		// Token: 0x04000248 RID: 584
		DE_BAD_INDEX,
		// Token: 0x04000249 RID: 585
		DE_NOT_IN_DRAFT_BUT_COULD_BE,
		// Token: 0x0400024A RID: 586
		DE_FEATURE_DISABLED
	}

	// Token: 0x02000050 RID: 80
	public class EntityChoices
	{
		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x060004E6 RID: 1254 RVA: 0x000139BB File Offset: 0x00011BBB
		// (set) Token: 0x060004E7 RID: 1255 RVA: 0x000139C3 File Offset: 0x00011BC3
		public int ID { get; set; }

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x060004E8 RID: 1256 RVA: 0x000139CC File Offset: 0x00011BCC
		// (set) Token: 0x060004E9 RID: 1257 RVA: 0x000139D4 File Offset: 0x00011BD4
		public CHOICE_TYPE ChoiceType { get; set; }

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x060004EA RID: 1258 RVA: 0x000139DD File Offset: 0x00011BDD
		// (set) Token: 0x060004EB RID: 1259 RVA: 0x000139E5 File Offset: 0x00011BE5
		public int CountMin { get; set; }

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x060004EC RID: 1260 RVA: 0x000139EE File Offset: 0x00011BEE
		// (set) Token: 0x060004ED RID: 1261 RVA: 0x000139F6 File Offset: 0x00011BF6
		public int CountMax { get; set; }

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x060004EE RID: 1262 RVA: 0x000139FF File Offset: 0x00011BFF
		// (set) Token: 0x060004EF RID: 1263 RVA: 0x00013A07 File Offset: 0x00011C07
		public List<int> Entities { get; set; }

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x060004F0 RID: 1264 RVA: 0x00013A10 File Offset: 0x00011C10
		// (set) Token: 0x060004F1 RID: 1265 RVA: 0x00013A18 File Offset: 0x00011C18
		public int Source { get; set; }

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x060004F2 RID: 1266 RVA: 0x00013A21 File Offset: 0x00011C21
		// (set) Token: 0x060004F3 RID: 1267 RVA: 0x00013A29 File Offset: 0x00011C29
		public int PlayerId { get; set; }
	}

	// Token: 0x02000052 RID: 82
	public class EntitiesChosen
	{
		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x060004F5 RID: 1269 RVA: 0x00013A3A File Offset: 0x00011C3A
		// (set) Token: 0x060004F6 RID: 1270 RVA: 0x00013A42 File Offset: 0x00011C42
		public int ID { get; set; }

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x060004F7 RID: 1271 RVA: 0x00013A4B File Offset: 0x00011C4B
		// (set) Token: 0x060004F8 RID: 1272 RVA: 0x00013A53 File Offset: 0x00011C53
		public List<int> Entities { get; set; }

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x060004F9 RID: 1273 RVA: 0x00013A5C File Offset: 0x00011C5C
		// (set) Token: 0x060004FA RID: 1274 RVA: 0x00013A64 File Offset: 0x00011C64
		public int PlayerId { get; set; }
	}

	// Token: 0x02000053 RID: 83
	public class Options
	{
		// Token: 0x060004FB RID: 1275 RVA: 0x00013A6D File Offset: 0x00011C6D
		public Options()
		{
			this.List = new List<Network.Options.Option>();
		}

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x060004FC RID: 1276 RVA: 0x00013A80 File Offset: 0x00011C80
		// (set) Token: 0x060004FD RID: 1277 RVA: 0x00013A88 File Offset: 0x00011C88
		public int ID { get; set; }

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x060004FE RID: 1278 RVA: 0x00013A91 File Offset: 0x00011C91
		// (set) Token: 0x060004FF RID: 1279 RVA: 0x00013A99 File Offset: 0x00011C99
		public List<Network.Options.Option> List { get; set; }

		// Token: 0x06000500 RID: 1280 RVA: 0x00013AA4 File Offset: 0x00011CA4
		public void CopyFrom(Network.Options options)
		{
			this.ID = options.ID;
			if (options.List == null)
			{
				this.List = null;
			}
			else
			{
				if (this.List != null)
				{
					this.List.Clear();
				}
				else
				{
					this.List = new List<Network.Options.Option>();
				}
				for (int i = 0; i < options.List.Count; i++)
				{
					Network.Options.Option option = new Network.Options.Option();
					option.CopyFrom(options.List[i]);
					this.List.Add(option);
				}
			}
		}

		// Token: 0x02000054 RID: 84
		public class Option
		{
			// Token: 0x06000501 RID: 1281 RVA: 0x00013B3A File Offset: 0x00011D3A
			public Option()
			{
				this.Main = new Network.Options.Option.SubOption();
				this.Subs = new List<Network.Options.Option.SubOption>();
			}

			// Token: 0x170000AE RID: 174
			// (get) Token: 0x06000502 RID: 1282 RVA: 0x00013B58 File Offset: 0x00011D58
			// (set) Token: 0x06000503 RID: 1283 RVA: 0x00013B60 File Offset: 0x00011D60
			public Network.Options.Option.OptionType Type { get; set; }

			// Token: 0x170000AF RID: 175
			// (get) Token: 0x06000504 RID: 1284 RVA: 0x00013B69 File Offset: 0x00011D69
			// (set) Token: 0x06000505 RID: 1285 RVA: 0x00013B71 File Offset: 0x00011D71
			public Network.Options.Option.SubOption Main { get; set; }

			// Token: 0x170000B0 RID: 176
			// (get) Token: 0x06000506 RID: 1286 RVA: 0x00013B7A File Offset: 0x00011D7A
			// (set) Token: 0x06000507 RID: 1287 RVA: 0x00013B82 File Offset: 0x00011D82
			public List<Network.Options.Option.SubOption> Subs { get; set; }

			// Token: 0x06000508 RID: 1288 RVA: 0x00013B8C File Offset: 0x00011D8C
			public void CopyFrom(Network.Options.Option option)
			{
				this.Type = option.Type;
				if (this.Main == null)
				{
					this.Main = new Network.Options.Option.SubOption();
				}
				this.Main.CopyFrom(option.Main);
				if (option.Subs == null)
				{
					this.Subs = null;
				}
				else
				{
					if (this.Subs == null)
					{
						this.Subs = new List<Network.Options.Option.SubOption>();
					}
					else
					{
						this.Subs.Clear();
					}
					for (int i = 0; i < option.Subs.Count; i++)
					{
						Network.Options.Option.SubOption subOption = new Network.Options.Option.SubOption();
						subOption.CopyFrom(option.Subs[i]);
						this.Subs.Add(subOption);
					}
				}
			}

			// Token: 0x02000055 RID: 85
			public enum OptionType
			{
				// Token: 0x0400025F RID: 607
				PASS = 1,
				// Token: 0x04000260 RID: 608
				END_TURN,
				// Token: 0x04000261 RID: 609
				POWER
			}

			// Token: 0x02000056 RID: 86
			public class SubOption
			{
				// Token: 0x170000B1 RID: 177
				// (get) Token: 0x0600050A RID: 1290 RVA: 0x00013C51 File Offset: 0x00011E51
				// (set) Token: 0x0600050B RID: 1291 RVA: 0x00013C59 File Offset: 0x00011E59
				public int ID { get; set; }

				// Token: 0x170000B2 RID: 178
				// (get) Token: 0x0600050C RID: 1292 RVA: 0x00013C62 File Offset: 0x00011E62
				// (set) Token: 0x0600050D RID: 1293 RVA: 0x00013C6A File Offset: 0x00011E6A
				public List<int> Targets { get; set; }

				// Token: 0x0600050E RID: 1294 RVA: 0x00013C74 File Offset: 0x00011E74
				public void CopyFrom(Network.Options.Option.SubOption subOption)
				{
					this.ID = subOption.ID;
					if (subOption.Targets == null)
					{
						this.Targets = null;
					}
					else
					{
						if (this.Targets == null)
						{
							this.Targets = new List<int>();
						}
						else
						{
							this.Targets.Clear();
						}
						for (int i = 0; i < subOption.Targets.Count; i++)
						{
							this.Targets.Add(subOption.Targets[i]);
						}
					}
				}
			}
		}
	}

	// Token: 0x02000057 RID: 87
	public class HistTagChange : Network.PowerHistory
	{
		// Token: 0x0600050F RID: 1295 RVA: 0x00013CFD File Offset: 0x00011EFD
		public HistTagChange() : base(Network.PowerType.TAG_CHANGE)
		{
		}

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x06000510 RID: 1296 RVA: 0x00013D06 File Offset: 0x00011F06
		// (set) Token: 0x06000511 RID: 1297 RVA: 0x00013D0E File Offset: 0x00011F0E
		public int Entity { get; set; }

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000512 RID: 1298 RVA: 0x00013D17 File Offset: 0x00011F17
		// (set) Token: 0x06000513 RID: 1299 RVA: 0x00013D1F File Offset: 0x00011F1F
		public int Tag { get; set; }

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x06000514 RID: 1300 RVA: 0x00013D28 File Offset: 0x00011F28
		// (set) Token: 0x06000515 RID: 1301 RVA: 0x00013D30 File Offset: 0x00011F30
		public int Value { get; set; }

		// Token: 0x06000516 RID: 1302 RVA: 0x00013D3C File Offset: 0x00011F3C
		public override string ToString()
		{
			return string.Format("type={0} entity={1} tag={2} value={3}", new object[]
			{
				base.Type,
				this.Entity,
				this.Tag,
				this.Value
			});
		}
	}

	// Token: 0x02000058 RID: 88
	public class PowerHistory
	{
		// Token: 0x06000517 RID: 1303 RVA: 0x00013D91 File Offset: 0x00011F91
		public PowerHistory(Network.PowerType init)
		{
			this.Type = init;
		}

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x06000518 RID: 1304 RVA: 0x00013DA0 File Offset: 0x00011FA0
		// (set) Token: 0x06000519 RID: 1305 RVA: 0x00013DA8 File Offset: 0x00011FA8
		public Network.PowerType Type { get; set; }

		// Token: 0x0600051A RID: 1306 RVA: 0x00013DB1 File Offset: 0x00011FB1
		public override string ToString()
		{
			return string.Format("type={0}", this.Type);
		}
	}

	// Token: 0x02000059 RID: 89
	public enum PowerType
	{
		// Token: 0x04000269 RID: 617
		FULL_ENTITY = 1,
		// Token: 0x0400026A RID: 618
		SHOW_ENTITY,
		// Token: 0x0400026B RID: 619
		HIDE_ENTITY,
		// Token: 0x0400026C RID: 620
		TAG_CHANGE,
		// Token: 0x0400026D RID: 621
		BLOCK_START,
		// Token: 0x0400026E RID: 622
		BLOCK_END,
		// Token: 0x0400026F RID: 623
		CREATE_GAME,
		// Token: 0x04000270 RID: 624
		META_DATA,
		// Token: 0x04000271 RID: 625
		CHANGE_ENTITY
	}

	// Token: 0x0200005A RID: 90
	public class GameSetup
	{
		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x0600051C RID: 1308 RVA: 0x00013DD0 File Offset: 0x00011FD0
		// (set) Token: 0x0600051D RID: 1309 RVA: 0x00013DD8 File Offset: 0x00011FD8
		public int Board { get; set; }

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x0600051E RID: 1310 RVA: 0x00013DE1 File Offset: 0x00011FE1
		// (set) Token: 0x0600051F RID: 1311 RVA: 0x00013DE9 File Offset: 0x00011FE9
		public int MaxSecretsPerPlayer { get; set; }

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000520 RID: 1312 RVA: 0x00013DF2 File Offset: 0x00011FF2
		// (set) Token: 0x06000521 RID: 1313 RVA: 0x00013DFA File Offset: 0x00011FFA
		public int MaxFriendlyMinionsPerPlayer { get; set; }

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000522 RID: 1314 RVA: 0x00013E03 File Offset: 0x00012003
		// (set) Token: 0x06000523 RID: 1315 RVA: 0x00013E0B File Offset: 0x0001200B
		public int DisconnectWhenStuckSeconds { get; set; }
	}

	// Token: 0x0200005B RID: 91
	public class HistCreateGame : Network.PowerHistory
	{
		// Token: 0x06000524 RID: 1316 RVA: 0x00013E14 File Offset: 0x00012014
		public HistCreateGame() : base(Network.PowerType.CREATE_GAME)
		{
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x00013E20 File Offset: 0x00012020
		public static Network.HistCreateGame CreateFromProto(PowerHistoryCreateGame src)
		{
			Network.HistCreateGame histCreateGame = new Network.HistCreateGame();
			histCreateGame.Game = Network.Entity.CreateFromProto(src.GameEntity);
			if (src.Players != null)
			{
				histCreateGame.Players = new List<Network.HistCreateGame.PlayerData>();
				for (int i = 0; i < src.Players.Count; i++)
				{
					Player src2 = src.Players[i];
					Network.HistCreateGame.PlayerData playerData = Network.HistCreateGame.PlayerData.CreateFromProto(src2);
					histCreateGame.Players.Add(playerData);
				}
			}
			return histCreateGame;
		}

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000526 RID: 1318 RVA: 0x00013E97 File Offset: 0x00012097
		// (set) Token: 0x06000527 RID: 1319 RVA: 0x00013E9F File Offset: 0x0001209F
		public Network.Entity Game { get; set; }

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000528 RID: 1320 RVA: 0x00013EA8 File Offset: 0x000120A8
		// (set) Token: 0x06000529 RID: 1321 RVA: 0x00013EB0 File Offset: 0x000120B0
		public List<Network.HistCreateGame.PlayerData> Players { get; set; }

		// Token: 0x0600052A RID: 1322 RVA: 0x00013EBC File Offset: 0x000120BC
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("game={0}", this.Game);
			if (this.Players == null)
			{
				stringBuilder.Append(" players=(null)");
			}
			else if (this.Players.Count == 0)
			{
				stringBuilder.Append(" players=0");
			}
			else
			{
				for (int i = 0; i < this.Players.Count; i++)
				{
					stringBuilder.AppendFormat(" players[{0}]=[{1}]", i, this.Players[i]);
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0200005C RID: 92
		public class PlayerData
		{
			// Token: 0x170000BD RID: 189
			// (get) Token: 0x0600052C RID: 1324 RVA: 0x00013F66 File Offset: 0x00012166
			// (set) Token: 0x0600052D RID: 1325 RVA: 0x00013F6E File Offset: 0x0001216E
			public int ID { get; set; }

			// Token: 0x170000BE RID: 190
			// (get) Token: 0x0600052E RID: 1326 RVA: 0x00013F77 File Offset: 0x00012177
			// (set) Token: 0x0600052F RID: 1327 RVA: 0x00013F7F File Offset: 0x0001217F
			public BnetGameAccountId GameAccountId { get; set; }

			// Token: 0x170000BF RID: 191
			// (get) Token: 0x06000530 RID: 1328 RVA: 0x00013F88 File Offset: 0x00012188
			// (set) Token: 0x06000531 RID: 1329 RVA: 0x00013F90 File Offset: 0x00012190
			public Network.Entity Player { get; set; }

			// Token: 0x170000C0 RID: 192
			// (get) Token: 0x06000532 RID: 1330 RVA: 0x00013F99 File Offset: 0x00012199
			// (set) Token: 0x06000533 RID: 1331 RVA: 0x00013FA1 File Offset: 0x000121A1
			public int CardBackID { get; set; }

			// Token: 0x06000534 RID: 1332 RVA: 0x00013FAC File Offset: 0x000121AC
			public static Network.HistCreateGame.PlayerData CreateFromProto(Player src)
			{
				return new Network.HistCreateGame.PlayerData
				{
					ID = src.Id,
					GameAccountId = BnetUtils.CreateGameAccountId(src.GameAccountId),
					Player = Network.Entity.CreateFromProto(src.Entity),
					CardBackID = src.CardBack
				};
			}

			// Token: 0x06000535 RID: 1333 RVA: 0x00013FFC File Offset: 0x000121FC
			public override string ToString()
			{
				return string.Format("ID={0} GameAccountId={1} Player={2} CardBackID={3}", new object[]
				{
					this.ID,
					this.GameAccountId,
					this.Player,
					this.CardBackID
				});
			}
		}
	}

	// Token: 0x0200005D RID: 93
	public class Entity
	{
		// Token: 0x06000536 RID: 1334 RVA: 0x00014047 File Offset: 0x00012247
		public Entity()
		{
			this.Tags = new List<Network.Entity.Tag>();
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06000537 RID: 1335 RVA: 0x0001405A File Offset: 0x0001225A
		// (set) Token: 0x06000538 RID: 1336 RVA: 0x00014062 File Offset: 0x00012262
		public int ID { get; set; }

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06000539 RID: 1337 RVA: 0x0001406B File Offset: 0x0001226B
		// (set) Token: 0x0600053A RID: 1338 RVA: 0x00014073 File Offset: 0x00012273
		public List<Network.Entity.Tag> Tags { get; set; }

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x0600053B RID: 1339 RVA: 0x0001407C File Offset: 0x0001227C
		// (set) Token: 0x0600053C RID: 1340 RVA: 0x00014084 File Offset: 0x00012284
		public string CardID { get; set; }

		// Token: 0x0600053D RID: 1341 RVA: 0x00014090 File Offset: 0x00012290
		public static Network.Entity CreateFromProto(PegasusGame.Entity src)
		{
			return new Network.Entity
			{
				ID = src.Id,
				CardID = string.Empty,
				Tags = Network.Entity.CreateTagsFromProto(src.Tags)
			};
		}

		// Token: 0x0600053E RID: 1342 RVA: 0x000140CC File Offset: 0x000122CC
		public static Network.Entity CreateFromProto(PowerHistoryEntity src)
		{
			return new Network.Entity
			{
				ID = src.Entity,
				CardID = src.Name,
				Tags = Network.Entity.CreateTagsFromProto(src.Tags)
			};
		}

		// Token: 0x0600053F RID: 1343 RVA: 0x0001410C File Offset: 0x0001230C
		public static List<Network.Entity.Tag> CreateTagsFromProto(IList<PegasusGame.Tag> tagList)
		{
			List<Network.Entity.Tag> list = new List<Network.Entity.Tag>();
			for (int i = 0; i < tagList.Count; i++)
			{
				PegasusGame.Tag tag = tagList[i];
				list.Add(new Network.Entity.Tag
				{
					Name = tag.Name,
					Value = tag.Value
				});
			}
			return list;
		}

		// Token: 0x06000540 RID: 1344 RVA: 0x00014164 File Offset: 0x00012364
		public override string ToString()
		{
			return string.Format("id={0} cardId={1} tags={2}", this.ID, this.CardID, this.Tags.Count);
		}

		// Token: 0x0200005F RID: 95
		public class Tag
		{
			// Token: 0x170000C4 RID: 196
			// (get) Token: 0x0600054D RID: 1357 RVA: 0x000143E2 File Offset: 0x000125E2
			// (set) Token: 0x0600054E RID: 1358 RVA: 0x000143EA File Offset: 0x000125EA
			public int Name { get; set; }

			// Token: 0x170000C5 RID: 197
			// (get) Token: 0x0600054F RID: 1359 RVA: 0x000143F3 File Offset: 0x000125F3
			// (set) Token: 0x06000550 RID: 1360 RVA: 0x000143FB File Offset: 0x000125FB
			public int Value { get; set; }
		}
	}

	// Token: 0x02000060 RID: 96
	public class HistFullEntity : Network.PowerHistory
	{
		// Token: 0x06000551 RID: 1361 RVA: 0x00014404 File Offset: 0x00012604
		public HistFullEntity() : base(Network.PowerType.FULL_ENTITY)
		{
		}

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x06000552 RID: 1362 RVA: 0x0001440D File Offset: 0x0001260D
		// (set) Token: 0x06000553 RID: 1363 RVA: 0x00014415 File Offset: 0x00012615
		public Network.Entity Entity { get; set; }

		// Token: 0x06000554 RID: 1364 RVA: 0x0001441E File Offset: 0x0001261E
		public override string ToString()
		{
			return string.Format("type={0} entity=[{1}]", base.Type, this.Entity);
		}
	}

	// Token: 0x02000061 RID: 97
	public class HistShowEntity : Network.PowerHistory
	{
		// Token: 0x06000555 RID: 1365 RVA: 0x0001443B File Offset: 0x0001263B
		public HistShowEntity() : base(Network.PowerType.SHOW_ENTITY)
		{
		}

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x06000556 RID: 1366 RVA: 0x00014444 File Offset: 0x00012644
		// (set) Token: 0x06000557 RID: 1367 RVA: 0x0001444C File Offset: 0x0001264C
		public Network.Entity Entity { get; set; }

		// Token: 0x06000558 RID: 1368 RVA: 0x00014455 File Offset: 0x00012655
		public override string ToString()
		{
			return string.Format("type={0} entity=[{1}]", base.Type, this.Entity);
		}
	}

	// Token: 0x02000062 RID: 98
	public class HistHideEntity : Network.PowerHistory
	{
		// Token: 0x06000559 RID: 1369 RVA: 0x00014472 File Offset: 0x00012672
		public HistHideEntity() : base(Network.PowerType.HIDE_ENTITY)
		{
		}

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x0600055A RID: 1370 RVA: 0x0001447B File Offset: 0x0001267B
		// (set) Token: 0x0600055B RID: 1371 RVA: 0x00014483 File Offset: 0x00012683
		public int Entity { get; set; }

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x0600055C RID: 1372 RVA: 0x0001448C File Offset: 0x0001268C
		// (set) Token: 0x0600055D RID: 1373 RVA: 0x00014494 File Offset: 0x00012694
		public int Zone { get; set; }

		// Token: 0x0600055E RID: 1374 RVA: 0x000144A0 File Offset: 0x000126A0
		public override string ToString()
		{
			return string.Format("type={0} entity={1} zone={2}", base.Type, this.Entity, this.Zone);
		}
	}

	// Token: 0x02000063 RID: 99
	public class HistChangeEntity : Network.PowerHistory
	{
		// Token: 0x0600055F RID: 1375 RVA: 0x000144D8 File Offset: 0x000126D8
		public HistChangeEntity() : base(Network.PowerType.CHANGE_ENTITY)
		{
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000560 RID: 1376 RVA: 0x000144E2 File Offset: 0x000126E2
		// (set) Token: 0x06000561 RID: 1377 RVA: 0x000144EA File Offset: 0x000126EA
		public Network.Entity Entity { get; set; }

		// Token: 0x06000562 RID: 1378 RVA: 0x000144F3 File Offset: 0x000126F3
		public override string ToString()
		{
			return string.Format("type={0} entity=[{1}]", base.Type, this.Entity);
		}
	}

	// Token: 0x02000064 RID: 100
	public class HistMetaData : Network.PowerHistory
	{
		// Token: 0x06000563 RID: 1379 RVA: 0x00014510 File Offset: 0x00012710
		public HistMetaData() : base(Network.PowerType.META_DATA)
		{
			this.Info = new List<int>();
		}

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x06000564 RID: 1380 RVA: 0x00014524 File Offset: 0x00012724
		// (set) Token: 0x06000565 RID: 1381 RVA: 0x0001452C File Offset: 0x0001272C
		public HistoryMeta.Type MetaType { get; set; }

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x06000566 RID: 1382 RVA: 0x00014535 File Offset: 0x00012735
		// (set) Token: 0x06000567 RID: 1383 RVA: 0x0001453D File Offset: 0x0001273D
		public List<int> Info { get; set; }

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x06000568 RID: 1384 RVA: 0x00014546 File Offset: 0x00012746
		// (set) Token: 0x06000569 RID: 1385 RVA: 0x0001454E File Offset: 0x0001274E
		public int Data { get; set; }

		// Token: 0x0600056A RID: 1386 RVA: 0x00014558 File Offset: 0x00012758
		public override string ToString()
		{
			return string.Format("type={0} metaType={1} infoCount={2} data={3}", new object[]
			{
				base.Type,
				this.MetaType,
				this.Info.Count,
				this.Data
			});
		}
	}

	// Token: 0x02000065 RID: 101
	public class TurnTimerInfo
	{
		// Token: 0x170000CE RID: 206
		// (get) Token: 0x0600056C RID: 1388 RVA: 0x000145BA File Offset: 0x000127BA
		// (set) Token: 0x0600056D RID: 1389 RVA: 0x000145C2 File Offset: 0x000127C2
		public float Seconds { get; set; }

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x0600056E RID: 1390 RVA: 0x000145CB File Offset: 0x000127CB
		// (set) Token: 0x0600056F RID: 1391 RVA: 0x000145D3 File Offset: 0x000127D3
		public int Turn { get; set; }

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x06000570 RID: 1392 RVA: 0x000145DC File Offset: 0x000127DC
		// (set) Token: 0x06000571 RID: 1393 RVA: 0x000145E4 File Offset: 0x000127E4
		public bool Show { get; set; }
	}

	// Token: 0x02000066 RID: 102
	public class HistBlockStart : Network.PowerHistory
	{
		// Token: 0x06000572 RID: 1394 RVA: 0x000145ED File Offset: 0x000127ED
		public HistBlockStart(HistoryBlock.Type type) : base(Network.PowerType.BLOCK_START)
		{
			this.BlockType = type;
		}

		// Token: 0x06000573 RID: 1395 RVA: 0x00014600 File Offset: 0x00012800
		public override string ToString()
		{
			return string.Format("type={0} blockType={1} entity={2} target={3}", new object[]
			{
				base.Type,
				this.BlockType,
				this.Entity,
				this.Target
			});
		}

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x06000574 RID: 1396 RVA: 0x00014655 File Offset: 0x00012855
		// (set) Token: 0x06000575 RID: 1397 RVA: 0x0001465D File Offset: 0x0001285D
		public HistoryBlock.Type BlockType { get; set; }

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06000576 RID: 1398 RVA: 0x00014666 File Offset: 0x00012866
		// (set) Token: 0x06000577 RID: 1399 RVA: 0x0001466E File Offset: 0x0001286E
		public int Entity { get; set; }

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x06000578 RID: 1400 RVA: 0x00014677 File Offset: 0x00012877
		// (set) Token: 0x06000579 RID: 1401 RVA: 0x0001467F File Offset: 0x0001287F
		public int Target { get; set; }

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x0600057A RID: 1402 RVA: 0x00014688 File Offset: 0x00012888
		// (set) Token: 0x0600057B RID: 1403 RVA: 0x00014690 File Offset: 0x00012890
		public string EffectCardId { get; set; }

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x0600057C RID: 1404 RVA: 0x00014699 File Offset: 0x00012899
		// (set) Token: 0x0600057D RID: 1405 RVA: 0x000146A1 File Offset: 0x000128A1
		public int EffectIndex { get; set; }
	}

	// Token: 0x02000067 RID: 103
	// (Invoke) Token: 0x0600057F RID: 1407
	public delegate void GameServerDisconnectEvent(BattleNetErrors errorCode);

	// Token: 0x02000068 RID: 104
	public class UserUI
	{
		// Token: 0x04000291 RID: 657
		public Network.UserUI.MouseInfo mouseInfo;

		// Token: 0x04000292 RID: 658
		public Network.UserUI.EmoteInfo emoteInfo;

		// Token: 0x04000293 RID: 659
		public int? playerId;

		// Token: 0x02000069 RID: 105
		public class MouseInfo
		{
			// Token: 0x170000D6 RID: 214
			// (get) Token: 0x06000584 RID: 1412 RVA: 0x000146BA File Offset: 0x000128BA
			// (set) Token: 0x06000585 RID: 1413 RVA: 0x000146C2 File Offset: 0x000128C2
			public int OverCardID { get; set; }

			// Token: 0x170000D7 RID: 215
			// (get) Token: 0x06000586 RID: 1414 RVA: 0x000146CB File Offset: 0x000128CB
			// (set) Token: 0x06000587 RID: 1415 RVA: 0x000146D3 File Offset: 0x000128D3
			public int HeldCardID { get; set; }

			// Token: 0x170000D8 RID: 216
			// (get) Token: 0x06000588 RID: 1416 RVA: 0x000146DC File Offset: 0x000128DC
			// (set) Token: 0x06000589 RID: 1417 RVA: 0x000146E4 File Offset: 0x000128E4
			public int ArrowOriginID { get; set; }

			// Token: 0x170000D9 RID: 217
			// (get) Token: 0x0600058A RID: 1418 RVA: 0x000146ED File Offset: 0x000128ED
			// (set) Token: 0x0600058B RID: 1419 RVA: 0x000146F5 File Offset: 0x000128F5
			public int X { get; set; }

			// Token: 0x170000DA RID: 218
			// (get) Token: 0x0600058C RID: 1420 RVA: 0x000146FE File Offset: 0x000128FE
			// (set) Token: 0x0600058D RID: 1421 RVA: 0x00014706 File Offset: 0x00012906
			public int Y { get; set; }
		}

		// Token: 0x0200006A RID: 106
		public class EmoteInfo
		{
			// Token: 0x170000DB RID: 219
			// (get) Token: 0x0600058F RID: 1423 RVA: 0x00014717 File Offset: 0x00012917
			// (set) Token: 0x06000590 RID: 1424 RVA: 0x0001471F File Offset: 0x0001291F
			public int Emote { get; set; }
		}
	}

	// Token: 0x0200006B RID: 107
	public class HistBlockEnd : Network.PowerHistory
	{
		// Token: 0x06000591 RID: 1425 RVA: 0x00014728 File Offset: 0x00012928
		public HistBlockEnd() : base(Network.PowerType.BLOCK_END)
		{
		}
	}

	// Token: 0x0200006C RID: 108
	public enum BnetLoginState
	{
		// Token: 0x0400029B RID: 667
		BATTLE_NET_UNKNOWN,
		// Token: 0x0400029C RID: 668
		BATTLE_NET_LOGGING_IN,
		// Token: 0x0400029D RID: 669
		BATTLE_NET_TIMEOUT,
		// Token: 0x0400029E RID: 670
		BATTLE_NET_LOGIN_FAILED,
		// Token: 0x0400029F RID: 671
		BATTLE_NET_LOGGED_IN
	}

	// Token: 0x0200006D RID: 109
	public class GameCancelInfo
	{
		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06000593 RID: 1427 RVA: 0x00014739 File Offset: 0x00012939
		// (set) Token: 0x06000594 RID: 1428 RVA: 0x00014741 File Offset: 0x00012941
		public Network.GameCancelInfo.Reason CancelReason { get; set; }

		// Token: 0x0200006E RID: 110
		public enum Reason
		{
			// Token: 0x040002A2 RID: 674
			OPPONENT_TIMEOUT = 1
		}
	}

	// Token: 0x0200006F RID: 111
	public class PurchaseErrorInfo
	{
		// Token: 0x06000595 RID: 1429 RVA: 0x0001474C File Offset: 0x0001294C
		public PurchaseErrorInfo()
		{
			this.Error = Network.PurchaseErrorInfo.ErrorType.UNKNOWN;
			this.PurchaseInProgressProductID = string.Empty;
			this.ErrorCode = string.Empty;
		}

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x06000596 RID: 1430 RVA: 0x0001477C File Offset: 0x0001297C
		// (set) Token: 0x06000597 RID: 1431 RVA: 0x00014784 File Offset: 0x00012984
		public Network.PurchaseErrorInfo.ErrorType Error { get; set; }

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x06000598 RID: 1432 RVA: 0x0001478D File Offset: 0x0001298D
		// (set) Token: 0x06000599 RID: 1433 RVA: 0x00014795 File Offset: 0x00012995
		public string PurchaseInProgressProductID { get; set; }

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x0600059A RID: 1434 RVA: 0x0001479E File Offset: 0x0001299E
		// (set) Token: 0x0600059B RID: 1435 RVA: 0x000147A6 File Offset: 0x000129A6
		public string ErrorCode { get; set; }

		// Token: 0x02000070 RID: 112
		public enum ErrorType
		{
			// Token: 0x040002A7 RID: 679
			UNKNOWN = -1,
			// Token: 0x040002A8 RID: 680
			SUCCESS,
			// Token: 0x040002A9 RID: 681
			STILL_IN_PROGRESS,
			// Token: 0x040002AA RID: 682
			INVALID_BNET,
			// Token: 0x040002AB RID: 683
			SERVICE_NA,
			// Token: 0x040002AC RID: 684
			PURCHASE_IN_PROGRESS,
			// Token: 0x040002AD RID: 685
			DATABASE,
			// Token: 0x040002AE RID: 686
			INVALID_QUANTITY,
			// Token: 0x040002AF RID: 687
			DUPLICATE_LICENSE,
			// Token: 0x040002B0 RID: 688
			REQUEST_NOT_SENT,
			// Token: 0x040002B1 RID: 689
			NO_ACTIVE_BPAY,
			// Token: 0x040002B2 RID: 690
			FAILED_RISK,
			// Token: 0x040002B3 RID: 691
			CANCELED,
			// Token: 0x040002B4 RID: 692
			WAIT_MOP,
			// Token: 0x040002B5 RID: 693
			WAIT_CONFIRM,
			// Token: 0x040002B6 RID: 694
			WAIT_RISK,
			// Token: 0x040002B7 RID: 695
			PRODUCT_NA,
			// Token: 0x040002B8 RID: 696
			RISK_TIMEOUT,
			// Token: 0x040002B9 RID: 697
			PRODUCT_ALREADY_OWNED,
			// Token: 0x040002BA RID: 698
			WAIT_THIRD_PARTY_RECEIPT,
			// Token: 0x040002BB RID: 699
			PRODUCT_EVENT_HAS_ENDED,
			// Token: 0x040002BC RID: 700
			BP_GENERIC_FAIL = 100,
			// Token: 0x040002BD RID: 701
			BP_INVALID_CC_EXPIRY,
			// Token: 0x040002BE RID: 702
			BP_RISK_ERROR,
			// Token: 0x040002BF RID: 703
			BP_NO_VALID_PAYMENT,
			// Token: 0x040002C0 RID: 704
			BP_PAYMENT_AUTH,
			// Token: 0x040002C1 RID: 705
			BP_PROVIDER_DENIED,
			// Token: 0x040002C2 RID: 706
			BP_PURCHASE_BAN,
			// Token: 0x040002C3 RID: 707
			BP_SPENDING_LIMIT,
			// Token: 0x040002C4 RID: 708
			BP_PARENTAL_CONTROL,
			// Token: 0x040002C5 RID: 709
			BP_THROTTLED,
			// Token: 0x040002C6 RID: 710
			BP_THIRD_PARTY_BAD_RECEIPT,
			// Token: 0x040002C7 RID: 711
			BP_THIRD_PARTY_RECEIPT_USED,
			// Token: 0x040002C8 RID: 712
			BP_PRODUCT_UNIQUENESS_VIOLATED,
			// Token: 0x040002C9 RID: 713
			BP_REGION_IS_DOWN,
			// Token: 0x040002CA RID: 714
			E_BP_GENERIC_FAIL_RETRY_CONTACT_CS_IF_PERSISTS = 115,
			// Token: 0x040002CB RID: 715
			E_BP_CHALLENGE_ID_FAILED_VERIFICATION
		}
	}

	// Token: 0x02000071 RID: 113
	public class PurchaseCanceledResponse
	{
		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x0600059D RID: 1437 RVA: 0x000147B7 File Offset: 0x000129B7
		// (set) Token: 0x0600059E RID: 1438 RVA: 0x000147BF File Offset: 0x000129BF
		public Network.PurchaseCanceledResponse.CancelResult Result { get; set; }

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x0600059F RID: 1439 RVA: 0x000147C8 File Offset: 0x000129C8
		// (set) Token: 0x060005A0 RID: 1440 RVA: 0x000147D0 File Offset: 0x000129D0
		public long TransactionID { get; set; }

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x060005A1 RID: 1441 RVA: 0x000147D9 File Offset: 0x000129D9
		// (set) Token: 0x060005A2 RID: 1442 RVA: 0x000147E1 File Offset: 0x000129E1
		public string ProductID { get; set; }

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x060005A3 RID: 1443 RVA: 0x000147EA File Offset: 0x000129EA
		// (set) Token: 0x060005A4 RID: 1444 RVA: 0x000147F2 File Offset: 0x000129F2
		public Currency CurrencyType { get; set; }

		// Token: 0x02000072 RID: 114
		public enum CancelResult
		{
			// Token: 0x040002D1 RID: 721
			SUCCESS,
			// Token: 0x040002D2 RID: 722
			NOT_ALLOWED,
			// Token: 0x040002D3 RID: 723
			NOTHING_TO_CANCEL
		}
	}

	// Token: 0x02000074 RID: 116
	public class BattlePayStatus
	{
		// Token: 0x060005A5 RID: 1445 RVA: 0x000147FC File Offset: 0x000129FC
		public BattlePayStatus()
		{
			this.State = Network.BattlePayStatus.PurchaseState.UNKNOWN;
			this.TransactionID = 0L;
			this.ThirdPartyID = string.Empty;
			this.ProductID = string.Empty;
			this.PurchaseError = new Network.PurchaseErrorInfo();
			this.BattlePayAvailable = false;
			this.CurrencyType = Currency.UNKNOWN;
			this.Provider = MoneyOrGTAPPTransaction.UNKNOWN_PROVIDER;
		}

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x060005A6 RID: 1446 RVA: 0x00014858 File Offset: 0x00012A58
		// (set) Token: 0x060005A7 RID: 1447 RVA: 0x00014860 File Offset: 0x00012A60
		public Network.BattlePayStatus.PurchaseState State { get; set; }

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x060005A8 RID: 1448 RVA: 0x00014869 File Offset: 0x00012A69
		// (set) Token: 0x060005A9 RID: 1449 RVA: 0x00014871 File Offset: 0x00012A71
		public long TransactionID { get; set; }

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x060005AA RID: 1450 RVA: 0x0001487A File Offset: 0x00012A7A
		// (set) Token: 0x060005AB RID: 1451 RVA: 0x00014882 File Offset: 0x00012A82
		public string ThirdPartyID { get; set; }

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x060005AC RID: 1452 RVA: 0x0001488B File Offset: 0x00012A8B
		// (set) Token: 0x060005AD RID: 1453 RVA: 0x00014893 File Offset: 0x00012A93
		public string ProductID { get; set; }

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x060005AE RID: 1454 RVA: 0x0001489C File Offset: 0x00012A9C
		// (set) Token: 0x060005AF RID: 1455 RVA: 0x000148A4 File Offset: 0x00012AA4
		public Network.PurchaseErrorInfo PurchaseError { get; set; }

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x060005B0 RID: 1456 RVA: 0x000148AD File Offset: 0x00012AAD
		// (set) Token: 0x060005B1 RID: 1457 RVA: 0x000148B5 File Offset: 0x00012AB5
		public bool BattlePayAvailable { get; set; }

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x060005B2 RID: 1458 RVA: 0x000148BE File Offset: 0x00012ABE
		// (set) Token: 0x060005B3 RID: 1459 RVA: 0x000148C6 File Offset: 0x00012AC6
		public Currency CurrencyType { get; set; }

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x060005B4 RID: 1460 RVA: 0x000148CF File Offset: 0x00012ACF
		// (set) Token: 0x060005B5 RID: 1461 RVA: 0x000148D7 File Offset: 0x00012AD7
		public BattlePayProvider? Provider { get; set; }

		// Token: 0x02000075 RID: 117
		public enum PurchaseState
		{
			// Token: 0x040002F2 RID: 754
			UNKNOWN = -1,
			// Token: 0x040002F3 RID: 755
			READY,
			// Token: 0x040002F4 RID: 756
			CHECK_RESULTS,
			// Token: 0x040002F5 RID: 757
			ERROR
		}
	}

	// Token: 0x02000077 RID: 119
	public class BattlePayConfig
	{
		// Token: 0x060005C6 RID: 1478 RVA: 0x00014ADC File Offset: 0x00012CDC
		public BattlePayConfig()
		{
			this.Available = false;
			this.Currency = Currency.UNKNOWN;
			this.Bundles = new List<Network.Bundle>();
			this.GoldCostBoosters = new List<Network.GoldCostBooster>();
			this.GoldCostArena = default(long?);
			this.SecondsBeforeAutoCancel = StoreManager.DEFAULT_SECONDS_BEFORE_AUTO_CANCEL;
		}

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x060005C7 RID: 1479 RVA: 0x00014B2D File Offset: 0x00012D2D
		// (set) Token: 0x060005C8 RID: 1480 RVA: 0x00014B35 File Offset: 0x00012D35
		public bool Available { get; set; }

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x060005C9 RID: 1481 RVA: 0x00014B3E File Offset: 0x00012D3E
		// (set) Token: 0x060005CA RID: 1482 RVA: 0x00014B46 File Offset: 0x00012D46
		public Currency Currency { get; set; }

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x060005CB RID: 1483 RVA: 0x00014B4F File Offset: 0x00012D4F
		// (set) Token: 0x060005CC RID: 1484 RVA: 0x00014B57 File Offset: 0x00012D57
		public List<Network.Bundle> Bundles { get; set; }

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x060005CD RID: 1485 RVA: 0x00014B60 File Offset: 0x00012D60
		// (set) Token: 0x060005CE RID: 1486 RVA: 0x00014B68 File Offset: 0x00012D68
		public List<Network.GoldCostBooster> GoldCostBoosters { get; set; }

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x060005CF RID: 1487 RVA: 0x00014B71 File Offset: 0x00012D71
		// (set) Token: 0x060005D0 RID: 1488 RVA: 0x00014B79 File Offset: 0x00012D79
		public long? GoldCostArena { get; set; }

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x060005D1 RID: 1489 RVA: 0x00014B82 File Offset: 0x00012D82
		// (set) Token: 0x060005D2 RID: 1490 RVA: 0x00014B8A File Offset: 0x00012D8A
		public int SecondsBeforeAutoCancel { get; set; }
	}

	// Token: 0x02000078 RID: 120
	public class GoldCostBooster
	{
		// Token: 0x060005D3 RID: 1491 RVA: 0x00014B94 File Offset: 0x00012D94
		public GoldCostBooster()
		{
			this.Cost = default(long?);
			this.ID = 0;
			this.BuyWithGoldEvent = SpecialEventType.UNKNOWN;
		}

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x060005D4 RID: 1492 RVA: 0x00014BC4 File Offset: 0x00012DC4
		// (set) Token: 0x060005D5 RID: 1493 RVA: 0x00014BCC File Offset: 0x00012DCC
		public long? Cost { get; set; }

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x060005D6 RID: 1494 RVA: 0x00014BD5 File Offset: 0x00012DD5
		// (set) Token: 0x060005D7 RID: 1495 RVA: 0x00014BDD File Offset: 0x00012DDD
		public int ID { get; set; }

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x060005D8 RID: 1496 RVA: 0x00014BE6 File Offset: 0x00012DE6
		// (set) Token: 0x060005D9 RID: 1497 RVA: 0x00014BEE File Offset: 0x00012DEE
		public SpecialEventType BuyWithGoldEvent { get; set; }
	}

	// Token: 0x0200007A RID: 122
	public class ThirdPartyPurchaseStatusResponse
	{
		// Token: 0x0600069C RID: 1692 RVA: 0x0001AB7E File Offset: 0x00018D7E
		public ThirdPartyPurchaseStatusResponse()
		{
			this.ThirdPartyID = string.Empty;
			this.Status = Network.ThirdPartyPurchaseStatusResponse.PurchaseStatus.UNKNOWN;
		}

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x0600069D RID: 1693 RVA: 0x0001AB98 File Offset: 0x00018D98
		// (set) Token: 0x0600069E RID: 1694 RVA: 0x0001ABA0 File Offset: 0x00018DA0
		public string ThirdPartyID { get; set; }

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x0600069F RID: 1695 RVA: 0x0001ABA9 File Offset: 0x00018DA9
		// (set) Token: 0x060006A0 RID: 1696 RVA: 0x0001ABB1 File Offset: 0x00018DB1
		public Network.ThirdPartyPurchaseStatusResponse.PurchaseStatus Status { get; set; }

		// Token: 0x0200007B RID: 123
		public enum PurchaseStatus
		{
			// Token: 0x04000358 RID: 856
			UNKNOWN = -1,
			// Token: 0x04000359 RID: 857
			NOT_FOUND = 1,
			// Token: 0x0400035A RID: 858
			SUCCEEDED,
			// Token: 0x0400035B RID: 859
			FAILED,
			// Token: 0x0400035C RID: 860
			IN_PROGRESS
		}
	}

	// Token: 0x0200007C RID: 124
	public class PurchaseViaGoldResponse
	{
		// Token: 0x060006A1 RID: 1697 RVA: 0x0001ABBA File Offset: 0x00018DBA
		public PurchaseViaGoldResponse()
		{
			this.Error = Network.PurchaseViaGoldResponse.ErrorType.UNKNOWN;
			this.GoldUsed = 0L;
		}

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x060006A2 RID: 1698 RVA: 0x0001ABD1 File Offset: 0x00018DD1
		// (set) Token: 0x060006A3 RID: 1699 RVA: 0x0001ABD9 File Offset: 0x00018DD9
		public Network.PurchaseViaGoldResponse.ErrorType Error { get; set; }

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x060006A4 RID: 1700 RVA: 0x0001ABE2 File Offset: 0x00018DE2
		// (set) Token: 0x060006A5 RID: 1701 RVA: 0x0001ABEA File Offset: 0x00018DEA
		public long GoldUsed { get; set; }

		// Token: 0x0200007D RID: 125
		public enum ErrorType
		{
			// Token: 0x04000360 RID: 864
			UNKNOWN = -1,
			// Token: 0x04000361 RID: 865
			SUCCESS = 1,
			// Token: 0x04000362 RID: 866
			INSUFFICIENT_GOLD,
			// Token: 0x04000363 RID: 867
			PRODUCT_NA,
			// Token: 0x04000364 RID: 868
			FEATURE_NA,
			// Token: 0x04000365 RID: 869
			INVALID_QUANTITY
		}
	}

	// Token: 0x0200007E RID: 126
	public class PurchaseMethod
	{
		// Token: 0x060006A6 RID: 1702 RVA: 0x0001ABF4 File Offset: 0x00018DF4
		public PurchaseMethod()
		{
			this.TransactionID = 0L;
			this.ProductID = string.Empty;
			this.Quantity = 0;
			this.Currency = Currency.UNKNOWN;
			this.WalletName = string.Empty;
			this.UseEBalance = false;
			this.IsZeroCostLicense = false;
			this.ChallengeID = string.Empty;
			this.ChallengeURL = string.Empty;
			this.PurchaseError = null;
		}

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x060006A7 RID: 1703 RVA: 0x0001AC5E File Offset: 0x00018E5E
		// (set) Token: 0x060006A8 RID: 1704 RVA: 0x0001AC66 File Offset: 0x00018E66
		public long TransactionID { get; set; }

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x060006A9 RID: 1705 RVA: 0x0001AC6F File Offset: 0x00018E6F
		// (set) Token: 0x060006AA RID: 1706 RVA: 0x0001AC77 File Offset: 0x00018E77
		public string ProductID { get; set; }

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x060006AB RID: 1707 RVA: 0x0001AC80 File Offset: 0x00018E80
		// (set) Token: 0x060006AC RID: 1708 RVA: 0x0001AC88 File Offset: 0x00018E88
		public int Quantity { get; set; }

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x060006AD RID: 1709 RVA: 0x0001AC91 File Offset: 0x00018E91
		// (set) Token: 0x060006AE RID: 1710 RVA: 0x0001AC99 File Offset: 0x00018E99
		public Currency Currency { get; set; }

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x060006AF RID: 1711 RVA: 0x0001ACA2 File Offset: 0x00018EA2
		// (set) Token: 0x060006B0 RID: 1712 RVA: 0x0001ACAA File Offset: 0x00018EAA
		public string WalletName { get; set; }

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x060006B1 RID: 1713 RVA: 0x0001ACB3 File Offset: 0x00018EB3
		// (set) Token: 0x060006B2 RID: 1714 RVA: 0x0001ACBB File Offset: 0x00018EBB
		public bool UseEBalance { get; set; }

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x060006B3 RID: 1715 RVA: 0x0001ACC4 File Offset: 0x00018EC4
		// (set) Token: 0x060006B4 RID: 1716 RVA: 0x0001ACCC File Offset: 0x00018ECC
		public bool IsZeroCostLicense { get; set; }

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x060006B5 RID: 1717 RVA: 0x0001ACD5 File Offset: 0x00018ED5
		// (set) Token: 0x060006B6 RID: 1718 RVA: 0x0001ACDD File Offset: 0x00018EDD
		public string ChallengeID { get; set; }

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x060006B7 RID: 1719 RVA: 0x0001ACE6 File Offset: 0x00018EE6
		// (set) Token: 0x060006B8 RID: 1720 RVA: 0x0001ACEE File Offset: 0x00018EEE
		public string ChallengeURL { get; set; }

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x060006B9 RID: 1721 RVA: 0x0001ACF7 File Offset: 0x00018EF7
		// (set) Token: 0x060006BA RID: 1722 RVA: 0x0001ACFF File Offset: 0x00018EFF
		public Network.PurchaseErrorInfo PurchaseError { get; set; }
	}

	// Token: 0x0200007F RID: 127
	public class PurchaseResponse
	{
		// Token: 0x060006BB RID: 1723 RVA: 0x0001AD08 File Offset: 0x00018F08
		public PurchaseResponse()
		{
			this.PurchaseError = new Network.PurchaseErrorInfo();
			this.TransactionID = 0L;
			this.ProductID = string.Empty;
			this.ThirdPartyID = string.Empty;
			this.CurrencyType = Currency.UNKNOWN;
		}

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x060006BC RID: 1724 RVA: 0x0001AD4B File Offset: 0x00018F4B
		// (set) Token: 0x060006BD RID: 1725 RVA: 0x0001AD53 File Offset: 0x00018F53
		public Network.PurchaseErrorInfo PurchaseError { get; set; }

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x060006BE RID: 1726 RVA: 0x0001AD5C File Offset: 0x00018F5C
		// (set) Token: 0x060006BF RID: 1727 RVA: 0x0001AD64 File Offset: 0x00018F64
		public long TransactionID { get; set; }

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x060006C0 RID: 1728 RVA: 0x0001AD6D File Offset: 0x00018F6D
		// (set) Token: 0x060006C1 RID: 1729 RVA: 0x0001AD75 File Offset: 0x00018F75
		public string ProductID { get; set; }

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x060006C2 RID: 1730 RVA: 0x0001AD7E File Offset: 0x00018F7E
		// (set) Token: 0x060006C3 RID: 1731 RVA: 0x0001AD86 File Offset: 0x00018F86
		public string ThirdPartyID { get; set; }

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x060006C4 RID: 1732 RVA: 0x0001AD8F File Offset: 0x00018F8F
		// (set) Token: 0x060006C5 RID: 1733 RVA: 0x0001AD97 File Offset: 0x00018F97
		public Currency CurrencyType { get; set; }
	}

	// Token: 0x02000080 RID: 128
	public class GenericResponse
	{
		// Token: 0x17000113 RID: 275
		// (get) Token: 0x060006C7 RID: 1735 RVA: 0x0001ADA8 File Offset: 0x00018FA8
		// (set) Token: 0x060006C8 RID: 1736 RVA: 0x0001ADB0 File Offset: 0x00018FB0
		public int RequestId { get; set; }

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x060006C9 RID: 1737 RVA: 0x0001ADB9 File Offset: 0x00018FB9
		// (set) Token: 0x060006CA RID: 1738 RVA: 0x0001ADC1 File Offset: 0x00018FC1
		public int RequestSubId { get; set; }

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x060006CB RID: 1739 RVA: 0x0001ADCA File Offset: 0x00018FCA
		// (set) Token: 0x060006CC RID: 1740 RVA: 0x0001ADD2 File Offset: 0x00018FD2
		public Network.GenericResponse.Result ResultCode { get; set; }

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x060006CD RID: 1741 RVA: 0x0001ADDB File Offset: 0x00018FDB
		// (set) Token: 0x060006CE RID: 1742 RVA: 0x0001ADE3 File Offset: 0x00018FE3
		public object GenericData { get; set; }

		// Token: 0x02000081 RID: 129
		public enum Result
		{
			// Token: 0x0400037A RID: 890
			OK,
			// Token: 0x0400037B RID: 891
			REQUEST_IN_PROCESS,
			// Token: 0x0400037C RID: 892
			REQUEST_COMPLETE,
			// Token: 0x0400037D RID: 893
			FIRST_ERROR = 100,
			// Token: 0x0400037E RID: 894
			INTERNAL_ERROR,
			// Token: 0x0400037F RID: 895
			DB_ERROR,
			// Token: 0x04000380 RID: 896
			INVALID_REQUEST,
			// Token: 0x04000381 RID: 897
			LOGIN_LOAD,
			// Token: 0x04000382 RID: 898
			DATA_MIGRATION_ERROR
		}
	}

	// Token: 0x02000082 RID: 130
	public class AchieveList
	{
		// Token: 0x060006CF RID: 1743 RVA: 0x0001ADEC File Offset: 0x00018FEC
		public AchieveList()
		{
			this.Achieves = new List<Network.AchieveList.Achieve>();
		}

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x060006D0 RID: 1744 RVA: 0x0001ADFF File Offset: 0x00018FFF
		// (set) Token: 0x060006D1 RID: 1745 RVA: 0x0001AE07 File Offset: 0x00019007
		public List<Network.AchieveList.Achieve> Achieves { get; set; }

		// Token: 0x02000083 RID: 131
		public class Achieve
		{
			// Token: 0x17000118 RID: 280
			// (get) Token: 0x060006D3 RID: 1747 RVA: 0x0001AE18 File Offset: 0x00019018
			// (set) Token: 0x060006D4 RID: 1748 RVA: 0x0001AE20 File Offset: 0x00019020
			public int ID { get; set; }

			// Token: 0x17000119 RID: 281
			// (get) Token: 0x060006D5 RID: 1749 RVA: 0x0001AE29 File Offset: 0x00019029
			// (set) Token: 0x060006D6 RID: 1750 RVA: 0x0001AE31 File Offset: 0x00019031
			public int Progress { get; set; }

			// Token: 0x1700011A RID: 282
			// (get) Token: 0x060006D7 RID: 1751 RVA: 0x0001AE3A File Offset: 0x0001903A
			// (set) Token: 0x060006D8 RID: 1752 RVA: 0x0001AE42 File Offset: 0x00019042
			public int AckProgress { get; set; }

			// Token: 0x1700011B RID: 283
			// (get) Token: 0x060006D9 RID: 1753 RVA: 0x0001AE4B File Offset: 0x0001904B
			// (set) Token: 0x060006DA RID: 1754 RVA: 0x0001AE53 File Offset: 0x00019053
			public int CompletionCount { get; set; }

			// Token: 0x1700011C RID: 284
			// (get) Token: 0x060006DB RID: 1755 RVA: 0x0001AE5C File Offset: 0x0001905C
			// (set) Token: 0x060006DC RID: 1756 RVA: 0x0001AE64 File Offset: 0x00019064
			public bool Active { get; set; }

			// Token: 0x1700011D RID: 285
			// (get) Token: 0x060006DD RID: 1757 RVA: 0x0001AE6D File Offset: 0x0001906D
			// (set) Token: 0x060006DE RID: 1758 RVA: 0x0001AE75 File Offset: 0x00019075
			public long DateGiven { get; set; }

			// Token: 0x1700011E RID: 286
			// (get) Token: 0x060006DF RID: 1759 RVA: 0x0001AE7E File Offset: 0x0001907E
			// (set) Token: 0x060006E0 RID: 1760 RVA: 0x0001AE86 File Offset: 0x00019086
			public long DateCompleted { get; set; }

			// Token: 0x1700011F RID: 287
			// (get) Token: 0x060006E1 RID: 1761 RVA: 0x0001AE8F File Offset: 0x0001908F
			// (set) Token: 0x060006E2 RID: 1762 RVA: 0x0001AE97 File Offset: 0x00019097
			public bool CanAck { get; set; }
		}
	}

	// Token: 0x02000084 RID: 132
	public class CanceledQuest
	{
		// Token: 0x060006E3 RID: 1763 RVA: 0x0001AEA0 File Offset: 0x000190A0
		public CanceledQuest()
		{
			this.AchieveID = 0;
			this.Canceled = false;
			this.NextQuestCancelDate = 0L;
		}

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x060006E4 RID: 1764 RVA: 0x0001AEC9 File Offset: 0x000190C9
		// (set) Token: 0x060006E5 RID: 1765 RVA: 0x0001AED1 File Offset: 0x000190D1
		public int AchieveID { get; set; }

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x060006E6 RID: 1766 RVA: 0x0001AEDA File Offset: 0x000190DA
		// (set) Token: 0x060006E7 RID: 1767 RVA: 0x0001AEE2 File Offset: 0x000190E2
		public bool Canceled { get; set; }

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x060006E8 RID: 1768 RVA: 0x0001AEEB File Offset: 0x000190EB
		// (set) Token: 0x060006E9 RID: 1769 RVA: 0x0001AEF3 File Offset: 0x000190F3
		public long NextQuestCancelDate { get; set; }
	}

	// Token: 0x02000085 RID: 133
	public class TriggeredEvent
	{
		// Token: 0x060006EA RID: 1770 RVA: 0x0001AEFC File Offset: 0x000190FC
		public TriggeredEvent()
		{
			this.EventID = 0;
			this.Success = false;
		}

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x060006EB RID: 1771 RVA: 0x0001AF12 File Offset: 0x00019112
		// (set) Token: 0x060006EC RID: 1772 RVA: 0x0001AF1A File Offset: 0x0001911A
		public int EventID { get; set; }

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x060006ED RID: 1773 RVA: 0x0001AF23 File Offset: 0x00019123
		// (set) Token: 0x060006EE RID: 1774 RVA: 0x0001AF2B File Offset: 0x0001912B
		public bool Success { get; set; }
	}

	// Token: 0x02000086 RID: 134
	public class AccountLicenseAchieveResponse
	{
		// Token: 0x17000125 RID: 293
		// (get) Token: 0x060006F0 RID: 1776 RVA: 0x0001AF3C File Offset: 0x0001913C
		// (set) Token: 0x060006F1 RID: 1777 RVA: 0x0001AF44 File Offset: 0x00019144
		public int Achieve { get; set; }

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x060006F2 RID: 1778 RVA: 0x0001AF4D File Offset: 0x0001914D
		// (set) Token: 0x060006F3 RID: 1779 RVA: 0x0001AF55 File Offset: 0x00019155
		public Network.AccountLicenseAchieveResponse.AchieveResult Result { get; set; }

		// Token: 0x02000087 RID: 135
		public enum AchieveResult
		{
			// Token: 0x04000394 RID: 916
			INVALID_ACHIEVE = 1,
			// Token: 0x04000395 RID: 917
			NOT_ACTIVE,
			// Token: 0x04000396 RID: 918
			IN_PROGRESS,
			// Token: 0x04000397 RID: 919
			COMPLETE,
			// Token: 0x04000398 RID: 920
			STATUS_UNKNOWN
		}
	}

	// Token: 0x02000088 RID: 136
	public class DebugConsoleResponse
	{
		// Token: 0x060006F4 RID: 1780 RVA: 0x0001AF5E File Offset: 0x0001915E
		public DebugConsoleResponse()
		{
			this.Response = string.Empty;
		}

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x060006F5 RID: 1781 RVA: 0x0001AF71 File Offset: 0x00019171
		// (set) Token: 0x060006F6 RID: 1782 RVA: 0x0001AF79 File Offset: 0x00019179
		public int Type { get; set; }

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x060006F7 RID: 1783 RVA: 0x0001AF82 File Offset: 0x00019182
		// (set) Token: 0x060006F8 RID: 1784 RVA: 0x0001AF8A File Offset: 0x0001918A
		public string Response { get; set; }
	}

	// Token: 0x02000089 RID: 137
	public class UnavailableReason
	{
		// Token: 0x0400039B RID: 923
		public string mainReason;

		// Token: 0x0400039C RID: 924
		public string subReason;

		// Token: 0x0400039D RID: 925
		public string extraData;
	}

	// Token: 0x0200008A RID: 138
	public enum BoosterSource
	{
		// Token: 0x0400039F RID: 927
		UNKNOWN,
		// Token: 0x040003A0 RID: 928
		ARENA_REWARD = 3,
		// Token: 0x040003A1 RID: 929
		BOUGHT,
		// Token: 0x040003A2 RID: 930
		LICENSED = 6,
		// Token: 0x040003A3 RID: 931
		CS_GIFT = 8,
		// Token: 0x040003A4 RID: 932
		QUEST_REWARD = 10,
		// Token: 0x040003A5 RID: 933
		BOUGHT_GOLD
	}

	// Token: 0x0200008B RID: 139
	public enum Version
	{
		// Token: 0x040003A7 RID: 935
		Major = 5,
		// Token: 0x040003A8 RID: 936
		Minor = 0,
		// Token: 0x040003A9 RID: 937
		Patch = 0,
		// Token: 0x040003AA RID: 938
		Sku = 0
	}

	// Token: 0x0200008C RID: 140
	private class RequestContext
	{
		// Token: 0x060006FA RID: 1786 RVA: 0x0001AF9B File Offset: 0x0001919B
		public RequestContext(int pendingResponseId, int requestId, int requestSubId, Network.TimeoutHandler timeoutHandler)
		{
			this.m_waitUntil = DateTime.Now + Network.GetMaxDeferredWait();
			this.m_pendingResponseId = pendingResponseId;
			this.m_requestId = requestId;
			this.m_requestSubId = requestSubId;
			this.m_timeoutHandler = timeoutHandler;
		}

		// Token: 0x040003AB RID: 939
		public DateTime m_waitUntil;

		// Token: 0x040003AC RID: 940
		public int m_pendingResponseId;

		// Token: 0x040003AD RID: 941
		public int m_requestId;

		// Token: 0x040003AE RID: 942
		public int m_requestSubId;

		// Token: 0x040003AF RID: 943
		public Network.TimeoutHandler m_timeoutHandler;
	}

	// Token: 0x0200008D RID: 141
	private class BnetErrorListener : EventListener<Network.BnetErrorCallback>
	{
		// Token: 0x060006FC RID: 1788 RVA: 0x0001AFDD File Offset: 0x000191DD
		public bool Fire(BnetErrorInfo info)
		{
			return this.m_callback(info, this.m_userData);
		}
	}

	// Token: 0x0200008E RID: 142
	public enum AuthResult
	{
		// Token: 0x040003B1 RID: 945
		UNKNOWN,
		// Token: 0x040003B2 RID: 946
		ALLOWED,
		// Token: 0x040003B3 RID: 947
		INVALID,
		// Token: 0x040003B4 RID: 948
		SECOND,
		// Token: 0x040003B5 RID: 949
		OFFLINE
	}

	// Token: 0x0200008F RID: 143
	public class QueueInfo
	{
		// Token: 0x040003B6 RID: 950
		public int position;

		// Token: 0x040003B7 RID: 951
		public long end;

		// Token: 0x040003B8 RID: 952
		public long stdev;
	}

	// Token: 0x02000090 RID: 144
	public class Notification
	{
		// Token: 0x17000129 RID: 297
		// (get) Token: 0x060006FF RID: 1791 RVA: 0x0001B001 File Offset: 0x00019201
		// (set) Token: 0x06000700 RID: 1792 RVA: 0x0001B009 File Offset: 0x00019209
		public Network.Notification.Type NotificationType { get; set; }

		// Token: 0x02000091 RID: 145
		public enum Type
		{
			// Token: 0x040003BB RID: 955
			IN_HAND_CARD_CAP = 1,
			// Token: 0x040003BC RID: 956
			MANA_CAP
		}
	}

	// Token: 0x02000092 RID: 146
	public class DeckCard
	{
		// Token: 0x1700012A RID: 298
		// (get) Token: 0x06000702 RID: 1794 RVA: 0x0001B01A File Offset: 0x0001921A
		// (set) Token: 0x06000703 RID: 1795 RVA: 0x0001B022 File Offset: 0x00019222
		public long Deck { get; set; }

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x06000704 RID: 1796 RVA: 0x0001B02B File Offset: 0x0001922B
		// (set) Token: 0x06000705 RID: 1797 RVA: 0x0001B033 File Offset: 0x00019233
		public long Card { get; set; }
	}

	// Token: 0x02000093 RID: 147
	public class CardQuote
	{
		// Token: 0x1700012C RID: 300
		// (get) Token: 0x06000707 RID: 1799 RVA: 0x0001B044 File Offset: 0x00019244
		// (set) Token: 0x06000708 RID: 1800 RVA: 0x0001B04C File Offset: 0x0001924C
		public int AssetID { get; set; }

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x06000709 RID: 1801 RVA: 0x0001B055 File Offset: 0x00019255
		// (set) Token: 0x0600070A RID: 1802 RVA: 0x0001B05D File Offset: 0x0001925D
		public int BuyPrice { get; set; }

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x0600070B RID: 1803 RVA: 0x0001B066 File Offset: 0x00019266
		// (set) Token: 0x0600070C RID: 1804 RVA: 0x0001B06E File Offset: 0x0001926E
		public int SaleValue { get; set; }

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x0600070D RID: 1805 RVA: 0x0001B077 File Offset: 0x00019277
		// (set) Token: 0x0600070E RID: 1806 RVA: 0x0001B07F File Offset: 0x0001927F
		public Network.CardQuote.QuoteState Status { get; set; }

		// Token: 0x02000094 RID: 148
		public enum QuoteState
		{
			// Token: 0x040003C4 RID: 964
			SUCCESS,
			// Token: 0x040003C5 RID: 965
			UNKNOWN_ERROR
		}
	}

	// Token: 0x02000095 RID: 149
	public class GameEnd
	{
		// Token: 0x0600070F RID: 1807 RVA: 0x0001B088 File Offset: 0x00019288
		public GameEnd()
		{
			this.Notices = new List<NetCache.ProfileNotice>();
		}

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x06000710 RID: 1808 RVA: 0x0001B09B File Offset: 0x0001929B
		// (set) Token: 0x06000711 RID: 1809 RVA: 0x0001B0A3 File Offset: 0x000192A3
		public List<NetCache.ProfileNotice> Notices { get; set; }
	}

	// Token: 0x02000096 RID: 150
	public class ProfileNotices
	{
		// Token: 0x06000712 RID: 1810 RVA: 0x0001B0AC File Offset: 0x000192AC
		public ProfileNotices()
		{
			this.Notices = new List<NetCache.ProfileNotice>();
		}

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x06000713 RID: 1811 RVA: 0x0001B0BF File Offset: 0x000192BF
		// (set) Token: 0x06000714 RID: 1812 RVA: 0x0001B0C7 File Offset: 0x000192C7
		public List<NetCache.ProfileNotice> Notices { get; set; }
	}

	// Token: 0x02000097 RID: 151
	private class BattleNetLogger : LoggerInterface
	{
		// Token: 0x06000716 RID: 1814 RVA: 0x0001B0D8 File Offset: 0x000192D8
		public void Log(LogLevel logLevel, string str)
		{
			if (logLevel == 5)
			{
				string message = GameStrings.Get(str);
				if (ApplicationMgr.IsInternal())
				{
					Error.AddDevFatal(message, new object[0]);
				}
				else
				{
					Error.AddFatal(message);
				}
			}
			LogLevel logLevel2;
			switch (logLevel)
			{
			case 0:
				logLevel2 = LogLevel.None;
				break;
			case 1:
				logLevel2 = LogLevel.Debug;
				break;
			case 2:
				logLevel2 = LogLevel.Info;
				break;
			case 3:
				logLevel2 = LogLevel.Warning;
				break;
			case 4:
			case 5:
				logLevel2 = LogLevel.Error;
				break;
			default:
				logLevel2 = LogLevel.Error;
				break;
			}
			global::Log.BattleNet.Print(logLevel2, str, new object[0]);
			if (logLevel2 >= LogLevel.Warning && !global::Log.BattleNet.CanPrint(LogTarget.CONSOLE, logLevel2, false))
			{
				string text = string.Format("[{0}] {1}", "BattleNet", str);
				LogLevel logLevel3 = logLevel2;
				if (logLevel3 != LogLevel.Warning)
				{
					if (logLevel3 != LogLevel.Error)
					{
					}
					Debug.LogError(text);
				}
				else
				{
					Debug.LogWarning(text);
				}
			}
		}
	}

	// Token: 0x0200009C RID: 156
	private class HSClientInterface : ClientInterface
	{
		// Token: 0x06000759 RID: 1881 RVA: 0x0001D077 File Offset: 0x0001B277
		public string GetVersion()
		{
			return Network.GetVersion();
		}

		// Token: 0x0600075A RID: 1882 RVA: 0x0001D07E File Offset: 0x0001B27E
		public bool IsVersionInt()
		{
			return Network.IsVersionInt();
		}

		// Token: 0x0600075B RID: 1883 RVA: 0x0001D088 File Offset: 0x0001B288
		public string GetUserAgent()
		{
			string text = "Hearthstone/";
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"5.0.",
				12574,
				" ("
			});
			if (PlatformSettings.OS == OSCategory.iOS)
			{
				text += "iOS;";
			}
			else if (PlatformSettings.OS == OSCategory.Android)
			{
				text += "Android;";
			}
			else if (PlatformSettings.OS == OSCategory.PC)
			{
				text += "PC;";
			}
			else if (PlatformSettings.OS == OSCategory.Mac)
			{
				text += "Mac;";
			}
			text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				this.CleanUserAgentString(SystemInfo.deviceModel),
				";",
				SystemInfo.deviceType,
				";",
				this.CleanUserAgentString(SystemInfo.deviceUniqueIdentifier),
				";",
				SystemInfo.graphicsDeviceID,
				";",
				this.CleanUserAgentString(SystemInfo.graphicsDeviceName),
				";",
				this.CleanUserAgentString(SystemInfo.graphicsDeviceVendor),
				";",
				SystemInfo.graphicsDeviceVendorID,
				";",
				this.CleanUserAgentString(SystemInfo.graphicsDeviceVersion),
				";",
				SystemInfo.graphicsMemorySize,
				";",
				SystemInfo.graphicsShaderLevel,
				";",
				SystemInfo.npotSupport,
				";",
				this.CleanUserAgentString(SystemInfo.operatingSystem),
				";",
				SystemInfo.processorCount,
				";",
				this.CleanUserAgentString(SystemInfo.processorType),
				";",
				SystemInfo.supportedRenderTargetCount,
				";",
				SystemInfo.supports3DTextures,
				";",
				SystemInfo.supportsAccelerometer,
				";",
				SystemInfo.supportsComputeShaders,
				";",
				SystemInfo.supportsGyroscope,
				";",
				SystemInfo.supportsImageEffects,
				";",
				SystemInfo.supportsInstancing,
				";",
				SystemInfo.supportsLocationService,
				";",
				SystemInfo.supportsRenderTextures,
				";",
				SystemInfo.supportsRenderToCubemap,
				";",
				SystemInfo.supportsShadows,
				";",
				SystemInfo.supportsSparseTextures,
				";",
				SystemInfo.supportsStencil,
				";",
				SystemInfo.supportsVibration,
				";",
				SystemInfo.systemMemorySize,
				";",
				SystemInfo.SupportsRenderTextureFormat(2),
				";",
				SystemInfo.SupportsRenderTextureFormat(5),
				";",
				SystemInfo.SupportsRenderTextureFormat(1),
				";",
				SystemInfo.graphicsDeviceVersion.StartsWith("Metal"),
				";",
				Screen.currentResolution.width,
				";",
				Screen.currentResolution.height,
				";",
				Screen.dpi,
				";"
			});
			if (PlatformSettings.OS == OSCategory.iOS || PlatformSettings.OS == OSCategory.Android)
			{
				if (UniversalInputManager.UsePhoneUI)
				{
					text += "Phone;";
				}
				else
				{
					text += "Tablet;";
				}
			}
			else
			{
				text += "Desktop;";
			}
			text += Application.genuine;
			return text + ") Battle.net/CSharp";
		}

		// Token: 0x0600075C RID: 1884 RVA: 0x0001D514 File Offset: 0x0001B714
		private string CleanUserAgentString(string data)
		{
			return Regex.Replace(data, "[^a-zA-Z0-9_.]+", "_");
		}

		// Token: 0x0600075D RID: 1885 RVA: 0x0001D533 File Offset: 0x0001B733
		public string GetBasePersistentDataPath()
		{
			return FileUtils.PersistentDataPath;
		}

		// Token: 0x0600075E RID: 1886 RVA: 0x0001D53A File Offset: 0x0001B73A
		public string GetTemporaryCachePath()
		{
			return this.s_tempCachePath;
		}

		// Token: 0x0600075F RID: 1887 RVA: 0x0001D542 File Offset: 0x0001B742
		public bool GetDisableConnectionMetering()
		{
			return Vars.Key("Aurora.DisableConnectionMetering").GetBool(false);
		}

		// Token: 0x06000760 RID: 1888 RVA: 0x0001D554 File Offset: 0x0001B754
		public constants.MobileEnv GetMobileEnvironment()
		{
			MobileEnv mobileEnvironment = ApplicationMgr.GetMobileEnvironment();
			if (mobileEnvironment != MobileEnv.PRODUCTION)
			{
				return 0;
			}
			return 1;
		}

		// Token: 0x06000761 RID: 1889 RVA: 0x0001D578 File Offset: 0x0001B778
		public string GetAuroraVersionName()
		{
			return 12574.ToString();
		}

		// Token: 0x06000762 RID: 1890 RVA: 0x0001D592 File Offset: 0x0001B792
		public string GetLocaleName()
		{
			return Localization.GetLocaleName();
		}

		// Token: 0x06000763 RID: 1891 RVA: 0x0001D599 File Offset: 0x0001B799
		public string GetPlatformName()
		{
			return "Win";
		}

		// Token: 0x06000764 RID: 1892 RVA: 0x0001D5A0 File Offset: 0x0001B7A0
		public constants.RuntimeEnvironment GetRuntimeEnvironment()
		{
			return 0;
		}

		// Token: 0x06000765 RID: 1893 RVA: 0x0001D5A3 File Offset: 0x0001B7A3
		public IUrlDownloader GetUrlDownloader()
		{
			return Network.s_urlDownloader;
		}

		// Token: 0x040003E1 RID: 993
		private string s_tempCachePath = Application.temporaryCachePath;
	}

	// Token: 0x020000A7 RID: 167
	// (Invoke) Token: 0x06000836 RID: 2102
	public delegate void QueueInfoHandler(Network.QueueInfo queueInfo);

	// Token: 0x020000A8 RID: 168
	// (Invoke) Token: 0x0600083A RID: 2106
	public delegate void GameQueueHandler(QueueEvent queueEvent);

	// Token: 0x020000A9 RID: 169
	// (Invoke) Token: 0x0600083E RID: 2110
	public delegate void BnetEventHandler(BattleNet.BnetEvent[] updates);

	// Token: 0x020000AA RID: 170
	// (Invoke) Token: 0x06000842 RID: 2114
	public delegate void FriendsHandler(FriendsUpdate[] updates);

	// Token: 0x020000AB RID: 171
	// (Invoke) Token: 0x06000846 RID: 2118
	public delegate void WhisperHandler(BnetWhisper[] whispers);

	// Token: 0x020000AC RID: 172
	// (Invoke) Token: 0x0600084A RID: 2122
	public delegate void PresenceHandler(PresenceUpdate[] updates);

	// Token: 0x020000AD RID: 173
	// (Invoke) Token: 0x0600084E RID: 2126
	public delegate void ChallengeHandler(ChallengeInfo[] challenges);

	// Token: 0x020000AE RID: 174
	// (Invoke) Token: 0x06000852 RID: 2130
	public delegate void SpectatorInviteReceivedHandler(Invite invite);
}
