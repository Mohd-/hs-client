using System;
using System.Collections.Generic;
using System.Diagnostics;
using bgs;
using PegasusUtil;

// Token: 0x020005AC RID: 1452
public class ClientRequestManager
{
	// Token: 0x060040FD RID: 16637 RVA: 0x00139344 File Offset: 0x00137544
	public bool SendClientRequest(int type, IProtoBuf body, ClientRequestManager.ClientRequestConfig clientRequestConfig, ClientRequestManager.RequestPhase requestPhase = ClientRequestManager.RequestPhase.RUNNING, int subID = 0)
	{
		return this.SendClientRequestImpl(type, body, clientRequestConfig, requestPhase, subID);
	}

	// Token: 0x060040FE RID: 16638 RVA: 0x00139353 File Offset: 0x00137553
	public void NotifyResponseReceived(PegasusPacket packet)
	{
		this.NotifyResponseReceivedImpl(packet);
	}

	// Token: 0x060040FF RID: 16639 RVA: 0x0013935C File Offset: 0x0013755C
	public void NotifyStartupSequenceComplete()
	{
		this.NotifyStartupSequenceCompleteImpl();
	}

	// Token: 0x06004100 RID: 16640 RVA: 0x00139364 File Offset: 0x00137564
	public bool HasPendingDeliveryPackets()
	{
		return this.HasPendingDeliveryPacketsImpl();
	}

	// Token: 0x06004101 RID: 16641 RVA: 0x0013936C File Offset: 0x0013756C
	public int PeekNetClientRequestType()
	{
		return this.PeekNetClientRequestTypeImpl();
	}

	// Token: 0x06004102 RID: 16642 RVA: 0x00139374 File Offset: 0x00137574
	public PegasusPacket GetNextClientRequest()
	{
		return this.GetNextClientRequestImpl();
	}

	// Token: 0x06004103 RID: 16643 RVA: 0x0013937C File Offset: 0x0013757C
	public void DropNextClientRequest()
	{
		this.DropNextClientRequestImpl();
	}

	// Token: 0x06004104 RID: 16644 RVA: 0x00139384 File Offset: 0x00137584
	public void NotifyLoginSequenceCompleted()
	{
		this.NotifyLoginSequenceCompletedImpl();
	}

	// Token: 0x06004105 RID: 16645 RVA: 0x0013938C File Offset: 0x0013758C
	public bool ShouldIgnoreError(BnetErrorInfo errorInfo)
	{
		return this.ShouldIgnoreErrorImpl(errorInfo);
	}

	// Token: 0x06004106 RID: 16646 RVA: 0x00139398 File Offset: 0x00137598
	public void ScheduleResubscribe()
	{
		foreach (KeyValuePair<int, ClientRequestManager.SystemChannel> keyValuePair in this.m_state.m_systems.Systems)
		{
			this.ScheduleResubscribeWithNewRoute(keyValuePair.Value);
		}
	}

	// Token: 0x06004107 RID: 16647 RVA: 0x00139404 File Offset: 0x00137604
	public void Terminate()
	{
		this.TerminateImpl();
	}

	// Token: 0x06004108 RID: 16648 RVA: 0x0013940C File Offset: 0x0013760C
	public void WillReset()
	{
		this.m_state = new ClientRequestManager.InternalState();
	}

	// Token: 0x06004109 RID: 16649 RVA: 0x00139419 File Offset: 0x00137619
	public void Update()
	{
		this.UpdateImpl();
	}

	// Token: 0x0600410A RID: 16650 RVA: 0x00139421 File Offset: 0x00137621
	public bool HasErrors()
	{
		return this.HasErrorsImpl();
	}

	// Token: 0x0600410B RID: 16651 RVA: 0x0013942C File Offset: 0x0013762C
	private bool ShouldIgnoreErrorImpl(BnetErrorInfo errorInfo)
	{
		uint context = (uint)errorInfo.GetContext();
		if (context == 0U)
		{
			return false;
		}
		ClientRequestManager.ClientRequestType clientRequest = this.GetClientRequest(context, "should_ignore_error", true);
		if (clientRequest == null)
		{
			return this.GetDroppedRequest(context, "should_ignore", true) || this.GetPendingSendRequest(context, "should_ignore", true) != null;
		}
		BattleNetErrors error = errorInfo.GetError();
		if (clientRequest.IsSubsribeRequest)
		{
			return (ulong)clientRequest.System.SubscribeAttempt < clientRequest.System.MaxResubscribeAttempts || !clientRequest.ShouldRetryOnError;
		}
		if (!clientRequest.ShouldRetryOnError)
		{
			return true;
		}
		if (clientRequest.System.PendingResponseTimeout == 0UL)
		{
			return false;
		}
		BattleNetErrors battleNetErrors = error;
		return (battleNetErrors == 1 || battleNetErrors == 3006 || battleNetErrors == 34200) && this.RescheduleSubscriptionAndRetryRequest(clientRequest, "received_error_util_lost");
	}

	// Token: 0x0600410C RID: 16652 RVA: 0x00139511 File Offset: 0x00137711
	private bool RescheduleSubscriptionAndRetryRequest(ClientRequestManager.ClientRequestType clientRequest, string errorReason)
	{
		this.AddRequestToPendingResponse(clientRequest, "resubscribe_and_retry_request");
		this.ScheduleResubscribeWithNewRoute(clientRequest.System);
		return true;
	}

	// Token: 0x0600410D RID: 16653 RVA: 0x0013952C File Offset: 0x0013772C
	private void ProcessServiceUnavailable(ClientRequestResponse response, ClientRequestManager.ClientRequestType clientRequest)
	{
		this.RescheduleSubscriptionAndRetryRequest(clientRequest, "received_CRRF_SERVICE_UNAVAILABLE");
	}

	// Token: 0x0600410E RID: 16654 RVA: 0x0013953C File Offset: 0x0013773C
	private void ProcessClientRequestResponse(PegasusPacket packet, ClientRequestManager.ClientRequestType clientRequest)
	{
		if (!(packet.Body is ClientRequestResponse))
		{
			return;
		}
		ClientRequestResponse clientRequestResponse = (ClientRequestResponse)packet.Body;
		ClientRequestResponse.ClientRequestResponseFlags clientRequestResponseFlags = 1;
		if ((clientRequestResponse.ResponseFlags & clientRequestResponseFlags) != null)
		{
			this.ProcessServiceUnavailable(clientRequestResponse, clientRequest);
		}
		ClientRequestResponse.ClientRequestResponseFlags clientRequestResponseFlags2 = 2;
		if ((clientRequestResponse.ResponseFlags & clientRequestResponseFlags2) != null)
		{
			this.m_state.m_receivedErrorSignal = true;
		}
	}

	// Token: 0x0600410F RID: 16655 RVA: 0x00139598 File Offset: 0x00137798
	[Conditional("CLIENTREQUESTMANAGER_LOGGING")]
	private void PopulateStringMap()
	{
		ClientRequestManager.s_typeToStringMap.Add(201, "GetAccountInfo");
		ClientRequestManager.s_typeToStringMap.Add(202, "DeckList");
		ClientRequestManager.s_typeToStringMap.Add(203, "UtilHandshake");
		ClientRequestManager.s_typeToStringMap.Add(204, "UtilAuth");
		ClientRequestManager.s_typeToStringMap.Add(205, "UpdateLogin");
		ClientRequestManager.s_typeToStringMap.Add(206, "DebugAuth");
		ClientRequestManager.s_typeToStringMap.Add(207, "Collection");
		ClientRequestManager.s_typeToStringMap.Add(208, "GamesInfo");
		ClientRequestManager.s_typeToStringMap.Add(209, "CreateDeck");
		ClientRequestManager.s_typeToStringMap.Add(210, "DeleteDeck");
		ClientRequestManager.s_typeToStringMap.Add(211, "RenameDeck");
		ClientRequestManager.s_typeToStringMap.Add(212, "ProfileNotices");
		ClientRequestManager.s_typeToStringMap.Add(213, "AckNotice");
		ClientRequestManager.s_typeToStringMap.Add(214, "GetDeck");
		ClientRequestManager.s_typeToStringMap.Add(215, "DeckContents");
		ClientRequestManager.s_typeToStringMap.Add(216, "DBAction");
		ClientRequestManager.s_typeToStringMap.Add(217, "DeckCreated");
		ClientRequestManager.s_typeToStringMap.Add(218, "DeckDeleted");
		ClientRequestManager.s_typeToStringMap.Add(219, "DeckRenamed");
		ClientRequestManager.s_typeToStringMap.Add(220, "DeckGainedCard");
		ClientRequestManager.s_typeToStringMap.Add(221, "DeckLostCard");
		ClientRequestManager.s_typeToStringMap.Add(222, "DeckSetData");
		ClientRequestManager.s_typeToStringMap.Add(223, "AckCardSeen");
		ClientRequestManager.s_typeToStringMap.Add(224, "BoosterList");
		ClientRequestManager.s_typeToStringMap.Add(225, "OpenBooster");
		ClientRequestManager.s_typeToStringMap.Add(226, "BoosterContent");
		ClientRequestManager.s_typeToStringMap.Add(227, "ProfileLastLogin");
		ClientRequestManager.s_typeToStringMap.Add(228, "ClientTracking");
		ClientRequestManager.s_typeToStringMap.Add(229, "unused");
		ClientRequestManager.s_typeToStringMap.Add(230, "SetProgress");
		ClientRequestManager.s_typeToStringMap.Add(231, "ProfileDeckLimit");
		ClientRequestManager.s_typeToStringMap.Add(232, "MedalInfo");
		ClientRequestManager.s_typeToStringMap.Add(233, "ProfileProgress");
		ClientRequestManager.s_typeToStringMap.Add(234, "MedalHistory");
		ClientRequestManager.s_typeToStringMap.Add(235, "DraftBegin");
		ClientRequestManager.s_typeToStringMap.Add(236, "CardBacks");
		ClientRequestManager.s_typeToStringMap.Add(237, "GetBattlePayConfig");
		ClientRequestManager.s_typeToStringMap.Add(238, "BattlePayConfigResponse");
		ClientRequestManager.s_typeToStringMap.Add(239, "SetOptions");
		ClientRequestManager.s_typeToStringMap.Add(240, "GetOptions");
		ClientRequestManager.s_typeToStringMap.Add(241, "ClientOptions");
		ClientRequestManager.s_typeToStringMap.Add(242, "DraftRetire");
		ClientRequestManager.s_typeToStringMap.Add(243, "AckAchieveProgress");
		ClientRequestManager.s_typeToStringMap.Add(244, "DraftGetPicksAndContents");
		ClientRequestManager.s_typeToStringMap.Add(245, "DraftMakePick");
		ClientRequestManager.s_typeToStringMap.Add(246, "DraftBeginning");
		ClientRequestManager.s_typeToStringMap.Add(247, "DraftRetired");
		ClientRequestManager.s_typeToStringMap.Add(248, "DraftChoicesAndContents");
		ClientRequestManager.s_typeToStringMap.Add(249, "DraftChosen");
		ClientRequestManager.s_typeToStringMap.Add(250, "GetPurchaseMethod");
		ClientRequestManager.s_typeToStringMap.Add(251, "DraftError");
		ClientRequestManager.s_typeToStringMap.Add(252, "Achieves");
		ClientRequestManager.s_typeToStringMap.Add(253, "GetAchieves");
		ClientRequestManager.s_typeToStringMap.Add(254, "NOP");
		ClientRequestManager.s_typeToStringMap.Add(255, "GetBattlePayStatus");
		ClientRequestManager.s_typeToStringMap.Add(256, "PurchaseResponse");
		ClientRequestManager.s_typeToStringMap.Add(257, "BuySellCard");
		ClientRequestManager.s_typeToStringMap.Add(258, "BoughtSoldCard");
		ClientRequestManager.s_typeToStringMap.Add(259, "DevBnetIdentify");
		ClientRequestManager.s_typeToStringMap.Add(260, "CardValues");
		ClientRequestManager.s_typeToStringMap.Add(261, "GuardianTrack");
		ClientRequestManager.s_typeToStringMap.Add(262, "ArcaneDustBalance");
		ClientRequestManager.s_typeToStringMap.Add(263, "CloseCardMarket");
		ClientRequestManager.s_typeToStringMap.Add(264, "GuardianVars");
		ClientRequestManager.s_typeToStringMap.Add(265, "BattlePayStatusResponse");
		ClientRequestManager.s_typeToStringMap.Add(266, "Error37 (deprecated)");
		ClientRequestManager.s_typeToStringMap.Add(267, "CheckAccountLicenses");
		ClientRequestManager.s_typeToStringMap.Add(268, "MassDisenchant");
		ClientRequestManager.s_typeToStringMap.Add(269, "MassDisenchantResponse");
		ClientRequestManager.s_typeToStringMap.Add(270, "PlayerRecords");
		ClientRequestManager.s_typeToStringMap.Add(271, "RewardProgress");
		ClientRequestManager.s_typeToStringMap.Add(272, "PurchaseMethod");
		ClientRequestManager.s_typeToStringMap.Add(273, "DoPurchase");
		ClientRequestManager.s_typeToStringMap.Add(274, "CancelPurchase");
		ClientRequestManager.s_typeToStringMap.Add(275, "CancelPurchaseResponse");
		ClientRequestManager.s_typeToStringMap.Add(276, "CheckGameLicenses");
		ClientRequestManager.s_typeToStringMap.Add(277, "CheckLicensesResponse");
		ClientRequestManager.s_typeToStringMap.Add(278, "GoldBalance");
		ClientRequestManager.s_typeToStringMap.Add(279, "PurchaseWithGold");
		ClientRequestManager.s_typeToStringMap.Add(280, "PurchaseWithGoldResponse");
		ClientRequestManager.s_typeToStringMap.Add(281, "CancelQuest");
		ClientRequestManager.s_typeToStringMap.Add(282, "CancelQuestResponse");
		ClientRequestManager.s_typeToStringMap.Add(283, "HeroXP");
		ClientRequestManager.s_typeToStringMap.Add(284, "ValidateAchieve");
		ClientRequestManager.s_typeToStringMap.Add(285, "ValidateAchieveResponse");
		ClientRequestManager.s_typeToStringMap.Add(286, "PlayQueue");
		ClientRequestManager.s_typeToStringMap.Add(287, "DraftAckRewards");
		ClientRequestManager.s_typeToStringMap.Add(288, "DraftRewardsAcked");
		ClientRequestManager.s_typeToStringMap.Add(289, "Disconnected");
		ClientRequestManager.s_typeToStringMap.Add(290, "Deadend");
		ClientRequestManager.s_typeToStringMap.Add(291, "SetCardBack");
		ClientRequestManager.s_typeToStringMap.Add(292, "SetCardBackResponse");
		ClientRequestManager.s_typeToStringMap.Add(293, "SubmitThirdPartyReceipt");
		ClientRequestManager.s_typeToStringMap.Add(294, "GetThirdPartyPurchaseStatus");
		ClientRequestManager.s_typeToStringMap.Add(295, "ThirdPartyPurchaseStatusResponse");
		ClientRequestManager.s_typeToStringMap.Add(296, "SetProgressResponse");
		ClientRequestManager.s_typeToStringMap.Add(297, "CheckAccountLicenseAchieve");
		ClientRequestManager.s_typeToStringMap.Add(298, "TriggerLaunchDayEvent");
		ClientRequestManager.s_typeToStringMap.Add(299, "EventResponse");
		ClientRequestManager.s_typeToStringMap.Add(300, "MassiveLoginReply");
		ClientRequestManager.s_typeToStringMap.Add(301, "(used in Console.proto)");
		ClientRequestManager.s_typeToStringMap.Add(302, "(used in Console.proto)");
		ClientRequestManager.s_typeToStringMap.Add(303, "GetAssetsVersion");
		ClientRequestManager.s_typeToStringMap.Add(304, "AssetsVersionResponse");
		ClientRequestManager.s_typeToStringMap.Add(305, "GetAdventureProgress");
		ClientRequestManager.s_typeToStringMap.Add(306, "AdventureProgressResponse");
		ClientRequestManager.s_typeToStringMap.Add(307, "UpdateLoginComplete");
		ClientRequestManager.s_typeToStringMap.Add(308, "AckWingProgress");
		ClientRequestManager.s_typeToStringMap.Add(309, "SetPlayerAdventureProgress");
		ClientRequestManager.s_typeToStringMap.Add(310, "SetAdventureOptions");
		ClientRequestManager.s_typeToStringMap.Add(311, "AccountLicenseAchieveResponse");
		ClientRequestManager.s_typeToStringMap.Add(312, "StartThirdPartyPurchase");
		ClientRequestManager.s_typeToStringMap.Add(313, "BoosterTally");
		ClientRequestManager.s_typeToStringMap.Add(314, "Subscribe");
		ClientRequestManager.s_typeToStringMap.Add(315, "SubscribeResponse");
		ClientRequestManager.s_typeToStringMap.Add(316, "TavernBrawlInfo");
		ClientRequestManager.s_typeToStringMap.Add(317, "TavernBrawlPlayerRecordResponse");
		ClientRequestManager.s_typeToStringMap.Add(318, "FavoriteHeroesResponse");
		ClientRequestManager.s_typeToStringMap.Add(319, "SetFavoriteHero");
		ClientRequestManager.s_typeToStringMap.Add(320, "SetFavoriteHeroResponse");
		ClientRequestManager.s_typeToStringMap.Add(321, "GetAssetRequest");
		ClientRequestManager.s_typeToStringMap.Add(322, "GetAssetResponse");
		ClientRequestManager.s_typeToStringMap.Add(323, "DebugCommandRequest");
		ClientRequestManager.s_typeToStringMap.Add(324, "DebugCommandResponse");
		ClientRequestManager.s_typeToStringMap.Add(325, "AccountLicensesInfoResponse");
		ClientRequestManager.s_typeToStringMap.Add(326, "GenericResponse");
		ClientRequestManager.s_typeToStringMap.Add(327, "GenericRequestList");
		ClientRequestManager.s_typeToStringMap.Add(328, "ClientRequestResponse");
	}

	// Token: 0x06004110 RID: 16656 RVA: 0x00139FA8 File Offset: 0x001381A8
	private string GetTypeName(int type)
	{
		string text = type.ToString();
		string text2;
		if (ClientRequestManager.s_typeToStringMap.Count > 0 && ClientRequestManager.s_typeToStringMap.TryGetValue(type, out text2))
		{
			return text2 + ":" + text;
		}
		return text;
	}

	// Token: 0x06004111 RID: 16657 RVA: 0x00139FF0 File Offset: 0x001381F0
	[Conditional("CLIENTREQUESTMANAGER_LOGGING")]
	private void LOG_DEBUG(string format, params object[] args)
	{
		string text = GeneralUtils.SafeFormat(format, args);
		Log.ClientRequestManager.Print("D " + text, new object[0]);
	}

	// Token: 0x06004112 RID: 16658 RVA: 0x0013A020 File Offset: 0x00138220
	[Conditional("CLIENTREQUESTMANAGER_LOGGING")]
	private void LOG_WARN(string format, params object[] args)
	{
		string text = GeneralUtils.SafeFormat(format, args);
		Log.ClientRequestManager.Print("W " + text, new object[0]);
	}

	// Token: 0x06004113 RID: 16659 RVA: 0x0013A050 File Offset: 0x00138250
	[Conditional("CLIENTREQUESTMANAGER_LOGGING")]
	private void LOG_ERROR(string format, params object[] args)
	{
		string text = GeneralUtils.SafeFormat(format, args);
		Log.ClientRequestManager.Print("E " + text, new object[0]);
	}

	// Token: 0x06004114 RID: 16660 RVA: 0x0013A080 File Offset: 0x00138280
	private bool HasPendingDeliveryPacketsImpl()
	{
		return this.m_state.m_responsesPendingDelivery.Count > 0;
	}

	// Token: 0x06004115 RID: 16661 RVA: 0x0013A098 File Offset: 0x00138298
	private int PeekNetClientRequestTypeImpl()
	{
		if (this.m_state.m_responsesPendingDelivery.Count == 0)
		{
			return 0;
		}
		return this.m_state.m_responsesPendingDelivery.Peek().Type;
	}

	// Token: 0x06004116 RID: 16662 RVA: 0x0013A0D4 File Offset: 0x001382D4
	private PegasusPacket GetNextClientRequestImpl()
	{
		if (this.m_state.m_responsesPendingDelivery.Count == 0)
		{
			return null;
		}
		return this.m_state.m_responsesPendingDelivery.Peek();
	}

	// Token: 0x06004117 RID: 16663 RVA: 0x0013A10C File Offset: 0x0013830C
	private void DropNextClientRequestImpl()
	{
		if (this.m_state.m_responsesPendingDelivery.Count == 0)
		{
			return;
		}
		PegasusPacket pegasusPacket = this.m_state.m_responsesPendingDelivery.Dequeue();
	}

	// Token: 0x06004118 RID: 16664 RVA: 0x0013A140 File Offset: 0x00138340
	private bool HasErrorsImpl()
	{
		return this.m_state.m_receivedErrorSignal;
	}

	// Token: 0x06004119 RID: 16665 RVA: 0x0013A150 File Offset: 0x00138350
	private void UpdateImpl()
	{
		if (!this.m_state.m_loginCompleteNotificationReceived)
		{
			return;
		}
		foreach (KeyValuePair<int, ClientRequestManager.SystemChannel> keyValuePair in this.m_state.m_systems.Systems)
		{
			if (this.UpdateStateSubscribeImpl(keyValuePair.Value))
			{
				this.ProcessClientRequests(keyValuePair.Value);
			}
		}
	}

	// Token: 0x0600411A RID: 16666 RVA: 0x0013A1E4 File Offset: 0x001383E4
	private bool SendClientRequestImpl(int type, IProtoBuf body, ClientRequestManager.ClientRequestConfig clientRequestConfig, ClientRequestManager.RequestPhase requestPhase, int subID)
	{
		if (type == 0)
		{
			return false;
		}
		if (requestPhase < ClientRequestManager.RequestPhase.STARTUP || requestPhase > ClientRequestManager.RequestPhase.RUNNING)
		{
			return false;
		}
		ClientRequestManager.ClientRequestConfig clientRequestConfig2 = (clientRequestConfig != null) ? clientRequestConfig : this.m_defaultConfig;
		int requestedSystem = clientRequestConfig2.RequestedSystem;
		ClientRequestManager.SystemChannel orCreateSystem = this.GetOrCreateSystem(requestedSystem);
		if (requestPhase < orCreateSystem.CurrentPhase)
		{
			return false;
		}
		if (orCreateSystem.WasEverInRunningPhase && requestPhase < ClientRequestManager.RequestPhase.RUNNING)
		{
			return false;
		}
		if (body == null)
		{
			return false;
		}
		ClientRequestManager.ClientRequestType clientRequestType = new ClientRequestManager.ClientRequestType(orCreateSystem);
		clientRequestType.Type = type;
		clientRequestType.ShouldRetryOnError = clientRequestConfig2.ShouldRetryOnError;
		clientRequestType.SubID = subID;
		clientRequestType.Body = ProtobufUtil.ToByteArray(body);
		clientRequestType.Phase = requestPhase;
		clientRequestType.SendCount = 0U;
		clientRequestType.RequestId = this.GetNextRequestId();
		if (clientRequestType.Phase == ClientRequestManager.RequestPhase.STARTUP)
		{
			orCreateSystem.Phases.StartUp.PendingSend.Enqueue(clientRequestType);
		}
		else
		{
			orCreateSystem.Phases.Running.PendingSend.Enqueue(clientRequestType);
		}
		return true;
	}

	// Token: 0x0600411B RID: 16667 RVA: 0x0013A2E0 File Offset: 0x001384E0
	private ClientRequestManager.SystemChannel GetOrCreateSystem(int systemId)
	{
		ClientRequestManager.SystemChannel systemChannel = null;
		if (this.m_state.m_systems.Systems.TryGetValue(systemId, out systemChannel))
		{
			return systemChannel;
		}
		systemChannel = new ClientRequestManager.SystemChannel();
		systemChannel.SystemId = systemId;
		this.m_state.m_systems.Systems[systemId] = systemChannel;
		return systemChannel;
	}

	// Token: 0x0600411C RID: 16668 RVA: 0x0013A334 File Offset: 0x00138534
	private uint GenerateContextId()
	{
		return this.m_nextContexId += 1U;
	}

	// Token: 0x0600411D RID: 16669 RVA: 0x0013A354 File Offset: 0x00138554
	private void NotifyResponseReceivedImpl(PegasusPacket packet)
	{
		uint context = (uint)packet.Context;
		ClientRequestManager.ClientRequestType clientRequest = this.GetClientRequest(context, "received_response", true);
		if (clientRequest == null)
		{
			if (packet.Context != 0)
			{
				if (this.GetDroppedRequest(context, "received_response", true))
				{
					return;
				}
			}
			this.m_state.m_responsesPendingDelivery.Enqueue(packet);
			return;
		}
		int type = packet.Type;
		if (type != 315)
		{
			if (type != 328)
			{
				this.ProcessResponse(packet, clientRequest);
			}
			else
			{
				this.ProcessClientRequestResponse(packet, clientRequest);
			}
		}
		else
		{
			this.ProcessSubscribeResponse(packet, clientRequest);
		}
	}

	// Token: 0x0600411E RID: 16670 RVA: 0x0013A3FA File Offset: 0x001385FA
	private void NotifyStartupSequenceCompleteImpl()
	{
		this.m_state.m_runningPhaseEnabled = true;
	}

	// Token: 0x0600411F RID: 16671 RVA: 0x0013A408 File Offset: 0x00138608
	private void NotifyLoginSequenceCompletedImpl()
	{
		this.m_state.m_loginCompleteNotificationReceived = true;
	}

	// Token: 0x06004120 RID: 16672 RVA: 0x0013A418 File Offset: 0x00138618
	private uint SendToUtil(ClientRequestManager.ClientRequestType request)
	{
		uint num = this.GenerateContextId();
		byte[] utilPacketBytes = request.GetUtilPacketBytes();
		BattleNet.SendUtilPacket(request.Type, request.System.SystemId, utilPacketBytes, utilPacketBytes.Length, request.SubID, (int)num, request.System.Route);
		request.Context = num;
		request.SendTime = DateTime.Now;
		request.SendCount += 1U;
		this.AddRequestToPendingResponse(request, "send_to_util");
		string text = (!request.IsSubsribeRequest) ? request.Phase.ToString() : "SUBSCRIBE";
		return num;
	}

	// Token: 0x06004121 RID: 16673 RVA: 0x0013A4B4 File Offset: 0x001386B4
	private void MoveRequestsFromPendingResponseToSend(ClientRequestManager.SystemChannel system, ClientRequestManager.RequestPhase phase, ClientRequestManager.PendingMapType pendingMap)
	{
		List<uint> list = new List<uint>();
		List<uint> list2 = new List<uint>();
		foreach (KeyValuePair<uint, ClientRequestManager.ClientRequestType> keyValuePair in this.m_state.m_activePendingResponseMap)
		{
			uint key = keyValuePair.Key;
			ClientRequestManager.ClientRequestType value = keyValuePair.Value;
			if (value.System.SystemId == system.SystemId && phase == value.Phase)
			{
				if (value.ShouldRetryOnError)
				{
					list.Add(key);
				}
				else
				{
					list2.Add(key);
				}
			}
		}
		string reason = "move_pending_response_to_pending_send";
		foreach (uint num in list)
		{
			ClientRequestManager.ClientRequestType clientRequest = this.GetClientRequest(num, reason, true);
			pendingMap.PendingSend.Enqueue(clientRequest);
			this.m_state.m_ignorePendingResponseMap.Add(num);
		}
		foreach (uint num2 in list2)
		{
			this.GetClientRequest(num2, reason, true);
			this.m_state.m_ignorePendingResponseMap.Add(num2);
		}
	}

	// Token: 0x06004122 RID: 16674 RVA: 0x0013A644 File Offset: 0x00138844
	private uint GetNextRequestId()
	{
		return this.m_nextRequestId += 1U;
	}

	// Token: 0x06004123 RID: 16675 RVA: 0x0013A664 File Offset: 0x00138864
	private void SendSubscriptionRequest(ClientRequestManager.SystemChannel system)
	{
		int systemId = system.SystemId;
		if (system.Route == 0UL)
		{
			this.MoveRequestsFromPendingResponseToSend(system, ClientRequestManager.RequestPhase.STARTUP, system.Phases.StartUp);
			this.MoveRequestsFromPendingResponseToSend(system, ClientRequestManager.RequestPhase.RUNNING, system.Phases.Running);
		}
		ClientRequestManager.ClientRequestType clientRequestType = new ClientRequestManager.ClientRequestType(system);
		clientRequestType.Type = 314;
		clientRequestType.SubID = 0;
		clientRequestType.Body = ProtobufUtil.ToByteArray(this.m_subscribePacket);
		clientRequestType.RequestId = this.GetNextRequestId();
		clientRequestType.IsSubsribeRequest = true;
		system.SubscriptionStatus.CurrentState = ClientRequestManager.SubscriptionStatusType.State.PENDING_RESPONSE;
		system.SubscriptionStatus.LastSend = DateTime.Now;
		system.SubscriptionStatus.ContexId = this.SendToUtil(clientRequestType);
		system.SubscribeAttempt += 1U;
		this.m_state.m_subscribePacketsSent += 1U;
	}

	// Token: 0x06004124 RID: 16676 RVA: 0x0013A735 File Offset: 0x00138935
	private void ScheduleResubscribeWithNewRoute(ClientRequestManager.SystemChannel system)
	{
		system.Route = 0UL;
		system.SubscriptionStatus.CurrentState = ClientRequestManager.SubscriptionStatusType.State.PENDING_SEND;
	}

	// Token: 0x06004125 RID: 16677 RVA: 0x0013A74B File Offset: 0x0013894B
	private void ScheduleResubscribeKeepRoute(ClientRequestManager.SystemChannel system)
	{
		system.SubscriptionStatus.CurrentState = ClientRequestManager.SubscriptionStatusType.State.PENDING_SEND;
	}

	// Token: 0x06004126 RID: 16678 RVA: 0x0013A75C File Offset: 0x0013895C
	private void TerminateImpl()
	{
		Unsubscribe packet = new Unsubscribe();
		foreach (KeyValuePair<int, ClientRequestManager.SystemChannel> keyValuePair in this.m_state.m_systems.Systems)
		{
			ClientRequestManager.SystemChannel value = keyValuePair.Value;
			if (value.SubscriptionStatus.CurrentState == ClientRequestManager.SubscriptionStatusType.State.SUBSCRIBED)
			{
				if (value.Route != 0UL)
				{
					ConnectAPI.SendUnsubcribeRequest(packet, value.SystemId);
				}
			}
		}
	}

	// Token: 0x06004127 RID: 16679 RVA: 0x0013A7FC File Offset: 0x001389FC
	private bool UpdateStateSubscribeImpl(ClientRequestManager.SystemChannel system)
	{
		switch (system.SubscriptionStatus.CurrentState)
		{
		case ClientRequestManager.SubscriptionStatusType.State.PENDING_SEND:
			return this.ProcessSubscribeStatePendingSend(system);
		case ClientRequestManager.SubscriptionStatusType.State.PENDING_RESPONSE:
			return this.ProcessSubscribeStatePendingResponse(system);
		case ClientRequestManager.SubscriptionStatusType.State.SUBSCRIBED:
			return this.ProcessSubscribeStateSubscribed(system);
		default:
			return system.SubscriptionStatus.CurrentState == ClientRequestManager.SubscriptionStatusType.State.SUBSCRIBED;
		}
	}

	// Token: 0x06004128 RID: 16680 RVA: 0x0013A854 File Offset: 0x00138A54
	private bool ProcessSubscribeStatePendingSend(ClientRequestManager.SystemChannel system)
	{
		if ((DateTime.Now - system.SubscriptionStatus.LastSend).TotalSeconds > system.PendingSubscribeTimeout)
		{
			this.SendSubscriptionRequest(system);
		}
		return system.Route != 0UL;
	}

	// Token: 0x06004129 RID: 16681 RVA: 0x0013A8A0 File Offset: 0x00138AA0
	private bool ProcessSubscribeStatePendingResponse(ClientRequestManager.SystemChannel system)
	{
		if ((DateTime.Now - system.SubscriptionStatus.LastSend).TotalSeconds > system.PendingSubscribeTimeout)
		{
			this.ScheduleResubscribeKeepRoute(system);
		}
		return system.Route != 0UL;
	}

	// Token: 0x0600412A RID: 16682 RVA: 0x0013A8EC File Offset: 0x00138AEC
	private int CountPendingResponsesForSystemId(ClientRequestManager.SystemChannel system)
	{
		int num = 0;
		foreach (KeyValuePair<uint, ClientRequestManager.ClientRequestType> keyValuePair in this.m_state.m_activePendingResponseMap)
		{
			ClientRequestManager.ClientRequestType value = keyValuePair.Value;
			if (value.System.SystemId == system.SystemId)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x0600412B RID: 16683 RVA: 0x0013A96C File Offset: 0x00138B6C
	private bool ProcessSubscribeStateSubscribed(ClientRequestManager.SystemChannel system)
	{
		if ((ulong)(DateTime.Now - system.SubscriptionStatus.SubscribedTime).TotalSeconds < system.KeepAliveSecs)
		{
			return true;
		}
		if (this.CountPendingResponsesForSystemId(system) > 0)
		{
			return true;
		}
		if (system.KeepAliveSecs > 0UL)
		{
			system.SubscriptionStatus.CurrentState = ClientRequestManager.SubscriptionStatusType.State.PENDING_SEND;
		}
		return true;
	}

	// Token: 0x0600412C RID: 16684 RVA: 0x0013A9D0 File Offset: 0x00138BD0
	private void ProcessSubscribeResponse(PegasusPacket packet, ClientRequestManager.ClientRequestType request)
	{
		if (!(packet.Body is SubscribeResponse))
		{
			return;
		}
		ClientRequestManager.SystemChannel system = request.System;
		int systemId = system.SystemId;
		SubscribeResponse subscribeResponse = (SubscribeResponse)packet.Body;
		if (subscribeResponse.Result == 2)
		{
			this.ScheduleResubscribeWithNewRoute(system);
			return;
		}
		system.SubscriptionStatus.CurrentState = ClientRequestManager.SubscriptionStatusType.State.SUBSCRIBED;
		system.SubscriptionStatus.SubscribedTime = DateTime.Now;
		system.Route = subscribeResponse.Route;
		system.CurrentPhase = ClientRequestManager.RequestPhase.STARTUP;
		system.SubscribeAttempt = 0U;
		system.KeepAliveSecs = subscribeResponse.KeepAliveSecs;
		system.MaxResubscribeAttempts = subscribeResponse.MaxResubscribeAttempts;
		system.PendingResponseTimeout = subscribeResponse.PendingResponseTimeout;
		system.PendingSubscribeTimeout = subscribeResponse.PendingSubscribeTimeout;
		this.m_state.m_responsesPendingDelivery.Enqueue(packet);
		system.m_subscribePacketsReceived += 1U;
	}

	// Token: 0x0600412D RID: 16685 RVA: 0x0013AAA4 File Offset: 0x00138CA4
	private void ProcessClientRequests(ClientRequestManager.SystemChannel system)
	{
		ClientRequestManager.PendingMapType pendingMapType = (system.CurrentPhase != ClientRequestManager.RequestPhase.STARTUP) ? system.Phases.Running : system.Phases.StartUp;
		foreach (KeyValuePair<uint, ClientRequestManager.ClientRequestType> keyValuePair in this.m_state.m_activePendingResponseMap)
		{
			ClientRequestManager.ClientRequestType value = keyValuePair.Value;
			if (!value.IsSubsribeRequest)
			{
				if (value.System != null)
				{
					if (value.System.SystemId == system.SystemId)
					{
						if (system.PendingResponseTimeout != 0UL)
						{
							if ((DateTime.Now - value.SendTime).TotalSeconds >= system.PendingResponseTimeout)
							{
								this.ScheduleResubscribeWithNewRoute(system);
								return;
							}
						}
					}
				}
			}
		}
		bool flag = pendingMapType.PendingSend.Count > 0;
		while (pendingMapType.PendingSend.Count > 0)
		{
			ClientRequestManager.ClientRequestType request = pendingMapType.PendingSend.Dequeue();
			this.SendToUtil(request);
		}
		if (flag)
		{
			return;
		}
		if (system.CurrentPhase == ClientRequestManager.RequestPhase.STARTUP && this.m_state.m_runningPhaseEnabled)
		{
			system.CurrentPhase = ClientRequestManager.RequestPhase.RUNNING;
		}
	}

	// Token: 0x0600412E RID: 16686 RVA: 0x0013AC10 File Offset: 0x00138E10
	private void ProcessResponse(PegasusPacket packet, ClientRequestManager.ClientRequestType clientRequest)
	{
		if (packet.Type != 254)
		{
			this.m_state.m_responsesPendingDelivery.Enqueue(packet);
		}
	}

	// Token: 0x0600412F RID: 16687 RVA: 0x0013AC34 File Offset: 0x00138E34
	private ClientRequestManager.ClientRequestType GetClientRequest(uint contextId, string reason, bool removeIfFound = true)
	{
		if (contextId == 0U)
		{
			return null;
		}
		ClientRequestManager.ClientRequestType result;
		if (!this.m_state.m_activePendingResponseMap.TryGetValue(contextId, out result))
		{
			if (!this.GetDroppedRequest(contextId, "get_client_request", false) || this.GetPendingSendRequest(contextId, "get_client_request", false) == null)
			{
			}
			return null;
		}
		if (removeIfFound)
		{
			this.m_state.m_activePendingResponseMap.Remove(contextId);
		}
		return result;
	}

	// Token: 0x06004130 RID: 16688 RVA: 0x0013ACA0 File Offset: 0x00138EA0
	private void AddRequestToPendingResponse(ClientRequestManager.ClientRequestType clientRequest, string reason)
	{
		if (!this.m_state.m_activePendingResponseMap.ContainsKey(clientRequest.Context))
		{
			this.m_state.m_activePendingResponseMap.Add(clientRequest.Context, clientRequest);
		}
	}

	// Token: 0x06004131 RID: 16689 RVA: 0x0013ACD9 File Offset: 0x00138ED9
	private bool GetDroppedRequest(uint contextId, string reason, bool removeIfFound = true)
	{
		if (this.m_state.m_ignorePendingResponseMap.Contains(contextId) && removeIfFound)
		{
			this.m_state.m_ignorePendingResponseMap.Remove(contextId);
			return true;
		}
		return false;
	}

	// Token: 0x06004132 RID: 16690 RVA: 0x0013AD0C File Offset: 0x00138F0C
	private ClientRequestManager.ClientRequestType GetPendingSendRequestForPhase(uint contextId, bool removeIfFound, ClientRequestManager.PendingMapType pendingMap)
	{
		ClientRequestManager.ClientRequestType clientRequestType = null;
		Queue<ClientRequestManager.ClientRequestType> queue = new Queue<ClientRequestManager.ClientRequestType>();
		foreach (ClientRequestManager.ClientRequestType clientRequestType2 in pendingMap.PendingSend)
		{
			if (clientRequestType == null && clientRequestType2.Context == contextId)
			{
				clientRequestType = clientRequestType2;
				if (!removeIfFound)
				{
					queue.Enqueue(clientRequestType2);
				}
			}
			else
			{
				queue.Enqueue(clientRequestType2);
			}
		}
		pendingMap.PendingSend = queue;
		return clientRequestType;
	}

	// Token: 0x06004133 RID: 16691 RVA: 0x0013AD9C File Offset: 0x00138F9C
	private ClientRequestManager.ClientRequestType GetPendingSendRequest(uint contextId, string reason, bool removeIfFound = true)
	{
		ClientRequestManager.ClientRequestType clientRequestType = null;
		foreach (KeyValuePair<int, ClientRequestManager.SystemChannel> keyValuePair in this.m_state.m_systems.Systems)
		{
			ClientRequestManager.SystemChannel value = keyValuePair.Value;
			clientRequestType = this.GetPendingSendRequestForPhase(contextId, removeIfFound, value.Phases.Running);
			if (clientRequestType != null)
			{
				break;
			}
			clientRequestType = this.GetPendingSendRequestForPhase(contextId, removeIfFound, value.Phases.StartUp);
		}
		return clientRequestType;
	}

	// Token: 0x0400296C RID: 10604
	private static Map<int, string> s_typeToStringMap = new Map<int, string>();

	// Token: 0x0400296D RID: 10605
	private readonly ClientRequestManager.ClientRequestConfig m_defaultConfig = new ClientRequestManager.ClientRequestConfig
	{
		ShouldRetryOnError = true,
		RequestedSystem = 0
	};

	// Token: 0x0400296E RID: 10606
	public uint m_nextContexId;

	// Token: 0x0400296F RID: 10607
	public uint m_nextRequestId;

	// Token: 0x04002970 RID: 10608
	private ClientRequestManager.InternalState m_state = new ClientRequestManager.InternalState();

	// Token: 0x04002971 RID: 10609
	private readonly Subscribe m_subscribePacket = new Subscribe();

	// Token: 0x020005AD RID: 1453
	public enum RequestPhase
	{
		// Token: 0x04002973 RID: 10611
		STARTUP,
		// Token: 0x04002974 RID: 10612
		RUNNING
	}

	// Token: 0x020005AE RID: 1454
	public class ClientRequestConfig
	{
		// Token: 0x1700049D RID: 1181
		// (get) Token: 0x06004135 RID: 16693 RVA: 0x0013AE44 File Offset: 0x00139044
		// (set) Token: 0x06004136 RID: 16694 RVA: 0x0013AE4C File Offset: 0x0013904C
		public bool ShouldRetryOnError { get; set; }

		// Token: 0x1700049E RID: 1182
		// (get) Token: 0x06004137 RID: 16695 RVA: 0x0013AE55 File Offset: 0x00139055
		// (set) Token: 0x06004138 RID: 16696 RVA: 0x0013AE5D File Offset: 0x0013905D
		public int RequestedSystem { get; set; }
	}

	// Token: 0x02000A6E RID: 2670
	private class ClientRequestType
	{
		// Token: 0x06005D61 RID: 23905 RVA: 0x001C051A File Offset: 0x001BE71A
		public ClientRequestType(ClientRequestManager.SystemChannel system)
		{
			this.System = system;
		}

		// Token: 0x06005D62 RID: 23906 RVA: 0x001C052C File Offset: 0x001BE72C
		public byte[] GetUtilPacketBytes()
		{
			RpcHeader rpcHeader = new RpcHeader();
			rpcHeader.Type = (ulong)((long)this.Type);
			if (this.SendCount > 0U)
			{
				rpcHeader.RetryCount = (ulong)this.SendCount;
			}
			RpcMessage rpcMessage = new RpcMessage();
			rpcMessage.RpcHeader = rpcHeader;
			if (this.Body != null && this.Body.Length > 0)
			{
				rpcMessage.MessageBody = this.Body;
			}
			return ProtobufUtil.ToByteArray(rpcMessage);
		}

		// Token: 0x04004504 RID: 17668
		public int Type;

		// Token: 0x04004505 RID: 17669
		public int SubID;

		// Token: 0x04004506 RID: 17670
		public byte[] Body;

		// Token: 0x04004507 RID: 17671
		public uint Context;

		// Token: 0x04004508 RID: 17672
		public ClientRequestManager.RequestPhase Phase;

		// Token: 0x04004509 RID: 17673
		public uint SendCount;

		// Token: 0x0400450A RID: 17674
		public DateTime SendTime;

		// Token: 0x0400450B RID: 17675
		public uint RequestId;

		// Token: 0x0400450C RID: 17676
		public bool IsSubsribeRequest;

		// Token: 0x0400450D RID: 17677
		public ClientRequestManager.SystemChannel System;

		// Token: 0x0400450E RID: 17678
		public bool ShouldRetryOnError;
	}

	// Token: 0x02000A6F RID: 2671
	private class SystemChannel
	{
		// Token: 0x0400450F RID: 17679
		public ClientRequestManager.PhaseMapType Phases = new ClientRequestManager.PhaseMapType();

		// Token: 0x04004510 RID: 17680
		public ClientRequestManager.SubscriptionStatusType SubscriptionStatus = new ClientRequestManager.SubscriptionStatusType();

		// Token: 0x04004511 RID: 17681
		public ulong Route;

		// Token: 0x04004512 RID: 17682
		public ClientRequestManager.RequestPhase CurrentPhase;

		// Token: 0x04004513 RID: 17683
		public ulong KeepAliveSecs;

		// Token: 0x04004514 RID: 17684
		public ulong MaxResubscribeAttempts;

		// Token: 0x04004515 RID: 17685
		public ulong PendingResponseTimeout;

		// Token: 0x04004516 RID: 17686
		public ulong PendingSubscribeTimeout = 15UL;

		// Token: 0x04004517 RID: 17687
		public uint SubscribeAttempt;

		// Token: 0x04004518 RID: 17688
		public bool WasEverInRunningPhase;

		// Token: 0x04004519 RID: 17689
		public int SystemId;

		// Token: 0x0400451A RID: 17690
		public uint m_subscribePacketsReceived;
	}

	// Token: 0x02000A70 RID: 2672
	private class PhaseMapType
	{
		// Token: 0x0400451B RID: 17691
		public ClientRequestManager.PendingMapType StartUp = new ClientRequestManager.PendingMapType();

		// Token: 0x0400451C RID: 17692
		public ClientRequestManager.PendingMapType Running = new ClientRequestManager.PendingMapType();
	}

	// Token: 0x02000A71 RID: 2673
	private class SubscriptionStatusType
	{
		// Token: 0x0400451D RID: 17693
		public ClientRequestManager.SubscriptionStatusType.State CurrentState;

		// Token: 0x0400451E RID: 17694
		public DateTime LastSend = DateTime.MinValue;

		// Token: 0x0400451F RID: 17695
		public DateTime SubscribedTime;

		// Token: 0x04004520 RID: 17696
		public uint ContexId;

		// Token: 0x02000A72 RID: 2674
		public enum State
		{
			// Token: 0x04004522 RID: 17698
			PENDING_SEND,
			// Token: 0x04004523 RID: 17699
			PENDING_RESPONSE,
			// Token: 0x04004524 RID: 17700
			SUBSCRIBED
		}
	}

	// Token: 0x02000A73 RID: 2675
	private class PendingMapType
	{
		// Token: 0x04004525 RID: 17701
		public Queue<ClientRequestManager.ClientRequestType> PendingSend = new Queue<ClientRequestManager.ClientRequestType>();
	}

	// Token: 0x02000A74 RID: 2676
	private class SystemMap
	{
		// Token: 0x04004526 RID: 17702
		public Map<int, ClientRequestManager.SystemChannel> Systems = new Map<int, ClientRequestManager.SystemChannel>();
	}

	// Token: 0x02000A75 RID: 2677
	private class InternalState
	{
		// Token: 0x04004527 RID: 17703
		public Queue<PegasusPacket> m_responsesPendingDelivery = new Queue<PegasusPacket>();

		// Token: 0x04004528 RID: 17704
		public ClientRequestManager.SystemMap m_systems = new ClientRequestManager.SystemMap();

		// Token: 0x04004529 RID: 17705
		public uint m_subscribePacketsSent;

		// Token: 0x0400452A RID: 17706
		public bool m_loginCompleteNotificationReceived;

		// Token: 0x0400452B RID: 17707
		public Map<uint, ClientRequestManager.ClientRequestType> m_activePendingResponseMap = new Map<uint, ClientRequestManager.ClientRequestType>();

		// Token: 0x0400452C RID: 17708
		public HashSet<uint> m_ignorePendingResponseMap = new HashSet<uint>();

		// Token: 0x0400452D RID: 17709
		public bool m_runningPhaseEnabled;

		// Token: 0x0400452E RID: 17710
		public bool m_receivedErrorSignal;
	}
}
