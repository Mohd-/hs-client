using System;
using System.Collections.Generic;
using bgs;
using bgs.types;
using PegasusShared;
using UnityEngine;

// Token: 0x020000BD RID: 189
public class ReconnectMgr : MonoBehaviour
{
	// Token: 0x06000A46 RID: 2630 RVA: 0x0002D5AC File Offset: 0x0002B7AC
	private void Awake()
	{
		ReconnectMgr.s_instance = this;
		FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
		GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
	}

	// Token: 0x06000A47 RID: 2631 RVA: 0x0002D5EC File Offset: 0x0002B7EC
	private void Start()
	{
		ApplicationMgr.Get().WillReset += new Action(this.WillReset);
	}

	// Token: 0x06000A48 RID: 2632 RVA: 0x0002D604 File Offset: 0x0002B804
	private void OnDestroy()
	{
		GameMgr.Get().UnregisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
		ApplicationMgr.Get().WillReset -= new Action(this.WillReset);
		ReconnectMgr.s_instance = null;
	}

	// Token: 0x06000A49 RID: 2633 RVA: 0x0002D644 File Offset: 0x0002B844
	private void Update()
	{
		this.CheckReconnectTimeout();
	}

	// Token: 0x06000A4A RID: 2634 RVA: 0x0002D64C File Offset: 0x0002B84C
	public static ReconnectMgr Get()
	{
		return ReconnectMgr.s_instance;
	}

	// Token: 0x06000A4B RID: 2635 RVA: 0x0002D653 File Offset: 0x0002B853
	public bool IsReconnectEnabled()
	{
		return ApplicationMgr.IsPublic() || Options.Get().GetBool(Option.RECONNECT);
	}

	// Token: 0x06000A4C RID: 2636 RVA: 0x0002D66D File Offset: 0x0002B86D
	public bool IsReconnecting()
	{
		return this.m_reconnectType != ReconnectType.INVALID;
	}

	// Token: 0x06000A4D RID: 2637 RVA: 0x0002D67C File Offset: 0x0002B87C
	public bool IsStartingReconnectGame()
	{
		if (GameMgr.Get().IsReconnect())
		{
			if (SceneMgr.Get().GetNextMode() == SceneMgr.Mode.GAMEPLAY)
			{
				return true;
			}
			if (SceneMgr.Get().GetMode() == SceneMgr.Mode.GAMEPLAY && !SceneMgr.Get().IsSceneLoaded())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000A4E RID: 2638 RVA: 0x0002D6CC File Offset: 0x0002B8CC
	public float GetTimeout()
	{
		if (ApplicationMgr.IsInternal())
		{
			return Options.Get().GetFloat(Option.RECONNECT_TIMEOUT);
		}
		return (float)OptionDataTables.s_defaultsMap[Option.RECONNECT_TIMEOUT];
	}

	// Token: 0x06000A4F RID: 2639 RVA: 0x0002D704 File Offset: 0x0002B904
	public float GetRetryTime()
	{
		if (ApplicationMgr.IsInternal())
		{
			return Options.Get().GetFloat(Option.RECONNECT_RETRY_TIME);
		}
		return (float)OptionDataTables.s_defaultsMap[Option.RECONNECT_RETRY_TIME];
	}

	// Token: 0x06000A50 RID: 2640 RVA: 0x0002D739 File Offset: 0x0002B939
	public bool AddTimeoutListener(ReconnectMgr.TimeoutCallback callback)
	{
		return this.AddTimeoutListener(callback, null);
	}

	// Token: 0x06000A51 RID: 2641 RVA: 0x0002D744 File Offset: 0x0002B944
	public bool AddTimeoutListener(ReconnectMgr.TimeoutCallback callback, object userData)
	{
		ReconnectMgr.TimeoutListener timeoutListener = new ReconnectMgr.TimeoutListener();
		timeoutListener.SetCallback(callback);
		timeoutListener.SetUserData(userData);
		if (this.m_timeoutListeners.Contains(timeoutListener))
		{
			return false;
		}
		this.m_timeoutListeners.Add(timeoutListener);
		return true;
	}

	// Token: 0x06000A52 RID: 2642 RVA: 0x0002D785 File Offset: 0x0002B985
	public bool RemoveTimeoutListener(ReconnectMgr.TimeoutCallback callback)
	{
		return this.RemoveTimeoutListener(callback, null);
	}

	// Token: 0x06000A53 RID: 2643 RVA: 0x0002D790 File Offset: 0x0002B990
	public bool RemoveTimeoutListener(ReconnectMgr.TimeoutCallback callback, object userData)
	{
		ReconnectMgr.TimeoutListener timeoutListener = new ReconnectMgr.TimeoutListener();
		timeoutListener.SetCallback(callback);
		timeoutListener.SetUserData(userData);
		return this.m_timeoutListeners.Remove(timeoutListener);
	}

	// Token: 0x06000A54 RID: 2644 RVA: 0x0002D7C0 File Offset: 0x0002B9C0
	public bool ReconnectFromLogin()
	{
		NetCache.ProfileNoticeDisconnectedGame dcgameNotice = this.GetDCGameNotice();
		if (dcgameNotice == null)
		{
			return false;
		}
		if (!this.IsReconnectEnabled())
		{
			return false;
		}
		if (dcgameNotice.GameResult != 1)
		{
			this.OnGameResult(dcgameNotice);
			return false;
		}
		if (dcgameNotice.GameType == null)
		{
			return false;
		}
		this.m_pendingReconnectNotice = dcgameNotice;
		ReconnectType reconnectType = ReconnectType.LOGIN;
		this.StartReconnecting(reconnectType);
		NetCache.Get().RegisterReconnectMgr(new NetCache.NetCacheCallback(this.OnNetCacheReady));
		return true;
	}

	// Token: 0x06000A55 RID: 2645 RVA: 0x0002D834 File Offset: 0x0002BA34
	public bool ReconnectFromGameplay()
	{
		if (!this.IsReconnectEnabled())
		{
			return false;
		}
		GameServerInfo lastGameServerJoined = Network.Get().GetLastGameServerJoined();
		if (lastGameServerJoined == null)
		{
			Debug.LogError("serverInfo in ReconnectMgr.ReconnectFromGameplay is null and should not be!");
			return false;
		}
		if (!lastGameServerJoined.Resumable)
		{
			return false;
		}
		this.HideDialog();
		GameType gameType = GameMgr.Get().GetGameType();
		ReconnectType reconnectType = ReconnectType.GAMEPLAY;
		this.StartReconnecting(reconnectType);
		this.StartGame(gameType, reconnectType, lastGameServerJoined);
		return true;
	}

	// Token: 0x06000A56 RID: 2646 RVA: 0x0002D89C File Offset: 0x0002BA9C
	public bool ShowDisconnectedGameResult(NetCache.ProfileNoticeDisconnectedGame dcGame)
	{
		if (!GameUtils.IsMatchmadeGameType(dcGame.GameType))
		{
			return false;
		}
		ProfileNoticeDisconnectedGameResult.GameResult gameResult = dcGame.GameResult;
		if (gameResult != 2 && gameResult != 3)
		{
			Debug.LogError(string.Format("ReconnectMgr.ShowDisconnectedGameResult() - unhandled game result {0}", dcGame.GameResult));
			return false;
		}
		if (dcGame.GameType == null)
		{
			return false;
		}
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get("GLUE_RECONNECT_RESULT_HEADER");
		string key;
		if (dcGame.GameResult == 3)
		{
			key = "GLUE_RECONNECT_RESULT_TIE";
		}
		else
		{
			switch (dcGame.YourResult)
			{
			case 1:
				key = "GLUE_RECONNECT_RESULT_WIN";
				break;
			case 2:
			case 4:
				key = "GLUE_RECONNECT_RESULT_LOSE";
				break;
			case 3:
				key = "GLUE_RECONNECT_RESULT_DISCONNECT";
				break;
			default:
				Debug.LogError(string.Format("ReconnectMgr.ShowDisconnectedGameResult() - unhandled player result {0}", dcGame.YourResult));
				return false;
			}
		}
		popupInfo.m_text = GameStrings.Format(key, new object[]
		{
			this.GetGameTypeName(dcGame.GameType, dcGame.MissionId)
		});
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
		popupInfo.m_showAlertIcon = true;
		DialogManager.Get().ShowPopup(popupInfo);
		return true;
	}

	// Token: 0x06000A57 RID: 2647 RVA: 0x0002D9D4 File Offset: 0x0002BBD4
	private string GetGameTypeName(GameType gameType, int missionId)
	{
		AdventureDbfRecord adventureRecord = GameUtils.GetAdventureRecord(missionId);
		if (adventureRecord != null)
		{
			switch (adventureRecord.ID)
			{
			case 1:
				return GameStrings.Get("GLUE_RECONNECT_GAME_TYPE_TUTORIAL");
			case 2:
				return GameStrings.Get("GLUE_RECONNECT_GAME_TYPE_PRACTICE");
			case 3:
				return GameStrings.Get("GLUE_RECONNECT_GAME_TYPE_NAXXRAMAS");
			case 4:
				return GameStrings.Get("GLUE_RECONNECT_GAME_TYPE_BRM");
			case 7:
				return GameStrings.Get("GLUE_RECONNECT_GAME_TYPE_TAVERN_BRAWL");
			}
			return adventureRecord.Name;
		}
		string key;
		if (this.m_gameTypeNameKeys.TryGetValue(gameType, out key))
		{
			return GameStrings.Get(key);
		}
		Error.AddDevFatal("ReconnectMgr.GetGameTypeName() - no name for mission {0} gameType {1}", new object[]
		{
			missionId,
			gameType
		});
		return string.Empty;
	}

	// Token: 0x06000A58 RID: 2648 RVA: 0x0002DAA4 File Offset: 0x0002BCA4
	private void WillReset()
	{
		this.m_dialog = null;
		this.ClearReconnectData();
		this.m_timeoutListeners.Clear();
		this.m_pendingReconnectNotice = null;
	}

	// Token: 0x06000A59 RID: 2649 RVA: 0x0002DAC8 File Offset: 0x0002BCC8
	private void StartReconnecting(ReconnectType reconnectType)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		this.m_reconnectType = reconnectType;
		this.m_reconnectStartTimestamp = realtimeSinceStartup;
		this.m_retryStartTimestamp = realtimeSinceStartup;
		this.ShowReconnectingDialog();
	}

	// Token: 0x06000A5A RID: 2650 RVA: 0x0002DAF8 File Offset: 0x0002BCF8
	private void CheckReconnectTimeout()
	{
		if (!this.IsReconnecting())
		{
			return;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float num = realtimeSinceStartup - this.m_reconnectStartTimestamp;
		float timeout = this.GetTimeout();
		if (num >= timeout)
		{
			this.OnReconnectTimeout();
			return;
		}
		ClientConnection<PegasusPacket> gameServerConnection = ConnectAPI.GetGameServerConnection();
		if (gameServerConnection.Active)
		{
			return;
		}
		if (gameServerConnection.HasEvents())
		{
			return;
		}
		float num2 = realtimeSinceStartup - this.m_retryStartTimestamp;
		float retryTime = this.GetRetryTime();
		if (num2 < retryTime)
		{
			return;
		}
		this.m_retryStartTimestamp = realtimeSinceStartup;
		this.StartGame();
	}

	// Token: 0x06000A5B RID: 2651 RVA: 0x0002DB7C File Offset: 0x0002BD7C
	private void OnReconnectTimeout()
	{
		this.ClearReconnectData();
		this.ChangeDialogToTimeout();
		if (this.m_pendingReconnectNotice != null)
		{
			this.AckNotice(this.m_pendingReconnectNotice);
			this.m_pendingReconnectNotice = null;
		}
		this.FireTimeoutEvent();
	}

	// Token: 0x06000A5C RID: 2652 RVA: 0x0002DBAE File Offset: 0x0002BDAE
	private void OnGameResult(NetCache.ProfileNoticeDisconnectedGame dcGameNotice)
	{
		this.ShowDisconnectedGameResult(dcGameNotice);
		this.AckNotice(dcGameNotice);
	}

	// Token: 0x06000A5D RID: 2653 RVA: 0x0002DBBF File Offset: 0x0002BDBF
	private void ClearReconnectData()
	{
		this.m_reconnectType = ReconnectType.INVALID;
		this.m_reconnectStartTimestamp = 0f;
		this.m_retryStartTimestamp = 0f;
	}

	// Token: 0x06000A5E RID: 2654 RVA: 0x0002DBE0 File Offset: 0x0002BDE0
	private void ShowReconnectingDialog()
	{
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get("GLOBAL_RECONNECT_RECONNECTING_HEADER");
		ReconnectType reconnectType = this.m_reconnectType;
		if (reconnectType != ReconnectType.LOGIN)
		{
			popupInfo.m_text = GameStrings.Get("GLOBAL_RECONNECT_RECONNECTING");
		}
		else
		{
			popupInfo.m_text = GameStrings.Get("GLOBAL_RECONNECT_RECONNECTING_LOGIN");
		}
		if (ApplicationMgr.CanQuitGame)
		{
			popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.CANCEL;
			popupInfo.m_cancelText = GameStrings.Get("GLOBAL_RECONNECT_EXIT_BUTTON");
		}
		else
		{
			popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.NONE;
		}
		popupInfo.m_showAlertIcon = true;
		popupInfo.m_responseCallback = new AlertPopup.ResponseCallback(this.OnReconnectingDialogResponse);
		DialogManager.Get().ShowPopup(popupInfo, new DialogManager.DialogProcessCallback(this.OnReconnectingDialogProcessed));
	}

	// Token: 0x06000A5F RID: 2655 RVA: 0x0002DCA2 File Offset: 0x0002BEA2
	private bool OnReconnectingDialogProcessed(DialogBase dialog, object userData)
	{
		if (!this.IsReconnecting())
		{
			return false;
		}
		this.m_dialog = (AlertPopup)dialog;
		if (this.IsStartingReconnectGame())
		{
			this.ChangeDialogToReconnected();
		}
		return true;
	}

	// Token: 0x06000A60 RID: 2656 RVA: 0x0002DCCF File Offset: 0x0002BECF
	private void OnReconnectingDialogResponse(AlertPopup.Response response, object userData)
	{
		this.m_dialog = null;
		ApplicationMgr.Get().Exit();
	}

	// Token: 0x06000A61 RID: 2657 RVA: 0x0002DCE4 File Offset: 0x0002BEE4
	private void ChangeDialogToReconnected()
	{
		if (this.m_dialog == null)
		{
			return;
		}
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get("GLOBAL_RECONNECT_RECONNECTED_HEADER");
		ReconnectType reconnectType = this.m_reconnectType;
		if (reconnectType != ReconnectType.LOGIN)
		{
			popupInfo.m_text = GameStrings.Get("GLOBAL_RECONNECT_RECONNECTED");
		}
		else
		{
			popupInfo.m_text = GameStrings.Get("GLOBAL_RECONNECT_RECONNECTED_LOGIN");
		}
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.NONE;
		popupInfo.m_showAlertIcon = true;
		this.m_dialog.UpdateInfo(popupInfo);
		LoadingScreen.Get().RegisterPreviousSceneDestroyedListener(new LoadingScreen.PreviousSceneDestroyedCallback(this.OnPreviousSceneDestroyed));
	}

	// Token: 0x06000A62 RID: 2658 RVA: 0x0002DD87 File Offset: 0x0002BF87
	private void OnPreviousSceneDestroyed(object userData)
	{
		LoadingScreen.Get().UnregisterPreviousSceneDestroyedListener(new LoadingScreen.PreviousSceneDestroyedCallback(this.OnPreviousSceneDestroyed));
		this.HideDialog();
	}

	// Token: 0x06000A63 RID: 2659 RVA: 0x0002DDA8 File Offset: 0x0002BFA8
	private void ChangeDialogToTimeout()
	{
		if (this.m_dialog == null)
		{
			return;
		}
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get("GLOBAL_RECONNECT_TIMEOUT_HEADER");
		popupInfo.m_text = GameStrings.Get("GLOBAL_RECONNECT_TIMEOUT");
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
		popupInfo.m_showAlertIcon = true;
		popupInfo.m_responseCallback = new AlertPopup.ResponseCallback(this.OnTimeoutDialogResponse);
		this.m_dialog.UpdateInfo(popupInfo);
	}

	// Token: 0x06000A64 RID: 2660 RVA: 0x0002DE19 File Offset: 0x0002C019
	private void OnTimeoutDialogResponse(AlertPopup.Response response, object userData)
	{
		this.m_dialog = null;
		if (!Network.IsLoggedIn())
		{
			if (ApplicationMgr.AllowResetFromFatalError)
			{
				ApplicationMgr.Get().Reset();
			}
			else
			{
				ApplicationMgr.Get().Exit();
			}
		}
	}

	// Token: 0x06000A65 RID: 2661 RVA: 0x0002DE54 File Offset: 0x0002C054
	private void HideDialog()
	{
		if (this.m_dialog == null)
		{
			return;
		}
		this.m_dialog.Hide();
		this.m_dialog = null;
	}

	// Token: 0x06000A66 RID: 2662 RVA: 0x0002DE88 File Offset: 0x0002C088
	private NetCache.ProfileNoticeDisconnectedGame GetDCGameNotice()
	{
		NetCache.NetCacheProfileNotices netObject = NetCache.Get().GetNetObject<NetCache.NetCacheProfileNotices>();
		if (netObject == null || netObject.Notices == null || netObject.Notices.Count == 0)
		{
			return null;
		}
		NetCache.ProfileNoticeDisconnectedGame profileNoticeDisconnectedGame = null;
		foreach (NetCache.ProfileNotice profileNotice in netObject.Notices)
		{
			NetCache.ProfileNoticeDisconnectedGame profileNoticeDisconnectedGame2 = profileNotice as NetCache.ProfileNoticeDisconnectedGame;
			if (profileNoticeDisconnectedGame2 != null)
			{
				if (profileNoticeDisconnectedGame == null)
				{
					profileNoticeDisconnectedGame = profileNoticeDisconnectedGame2;
				}
				else if (profileNoticeDisconnectedGame2.NoticeID > profileNoticeDisconnectedGame.NoticeID)
				{
					profileNoticeDisconnectedGame = profileNoticeDisconnectedGame2;
				}
			}
		}
		List<NetCache.ProfileNoticeDisconnectedGame> list = new List<NetCache.ProfileNoticeDisconnectedGame>();
		foreach (NetCache.ProfileNotice profileNotice2 in netObject.Notices)
		{
			NetCache.ProfileNoticeDisconnectedGame profileNoticeDisconnectedGame3 = profileNotice2 as NetCache.ProfileNoticeDisconnectedGame;
			if (profileNoticeDisconnectedGame3 != null)
			{
				if (profileNotice2.NoticeID != profileNoticeDisconnectedGame.NoticeID)
				{
					list.Add(profileNoticeDisconnectedGame3);
				}
			}
		}
		foreach (NetCache.ProfileNoticeDisconnectedGame notice in list)
		{
			this.AckNotice(notice);
		}
		return profileNoticeDisconnectedGame;
	}

	// Token: 0x06000A67 RID: 2663 RVA: 0x0002E008 File Offset: 0x0002C208
	private void OnNetCacheReady()
	{
		GameType gameType = this.m_pendingReconnectNotice.GameType;
		if (gameType == null)
		{
			return;
		}
		NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
		ReconnectType reconnectType = ReconnectType.LOGIN;
		NetCache.NetCacheDisconnectedGame netObject = NetCache.Get().GetNetObject<NetCache.NetCacheDisconnectedGame>();
		if (netObject == null || netObject.ServerInfo == null)
		{
			this.OnReconnectTimeout();
			return;
		}
		this.StartGame(gameType, reconnectType, netObject.ServerInfo);
	}

	// Token: 0x06000A68 RID: 2664 RVA: 0x0002E071 File Offset: 0x0002C271
	private void AckNotice(NetCache.ProfileNoticeDisconnectedGame notice)
	{
		Network.AckNotice(notice.NoticeID);
	}

	// Token: 0x06000A69 RID: 2665 RVA: 0x0002E080 File Offset: 0x0002C280
	private void StartGame(GameType gameType, ReconnectType reconnectType, GameServerInfo serverInfo)
	{
		this.m_savedStartGameParams.GameType = gameType;
		this.m_savedStartGameParams.ReconnectType = reconnectType;
		this.m_savedStartGameParams.ServerInfo = serverInfo;
		this.StartGame();
	}

	// Token: 0x06000A6A RID: 2666 RVA: 0x0002E0B7 File Offset: 0x0002C2B7
	private void StartGame()
	{
		GameMgr.Get().ReconnectGame(this.m_savedStartGameParams.GameType, this.m_savedStartGameParams.ReconnectType, this.m_savedStartGameParams.ServerInfo);
	}

	// Token: 0x06000A6B RID: 2667 RVA: 0x0002E0E4 File Offset: 0x0002C2E4
	private bool OnFindGameEvent(FindGameEventData eventData, object userData)
	{
		FindGameState state = eventData.m_state;
		if (state != FindGameState.SERVER_GAME_STARTED)
		{
			if (state == FindGameState.SERVER_GAME_CANCELED)
			{
				if (this.IsReconnecting())
				{
					this.OnReconnectTimeout();
					return true;
				}
			}
		}
		else if (this.IsReconnecting())
		{
			this.m_timeoutListeners.Clear();
			this.ChangeDialogToReconnected();
			this.ClearReconnectData();
			this.m_pendingReconnectNotice = null;
		}
		return false;
	}

	// Token: 0x06000A6C RID: 2668 RVA: 0x0002E15C File Offset: 0x0002C35C
	private void FireTimeoutEvent()
	{
		ReconnectMgr.TimeoutListener[] array = this.m_timeoutListeners.ToArray();
		this.m_timeoutListeners.Clear();
		bool flag = false;
		for (int i = 0; i < array.Length; i++)
		{
			flag = (array[i].Fire() || flag);
		}
		if (!flag && Network.IsLoggedIn())
		{
			SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
		}
	}

	// Token: 0x06000A6D RID: 2669 RVA: 0x0002E1C3 File Offset: 0x0002C3C3
	private void OnFatalError(FatalErrorMessage message, object userData)
	{
		this.ClearReconnectData();
	}

	// Token: 0x040004ED RID: 1261
	private Map<GameType, string> m_gameTypeNameKeys = new Map<GameType, string>
	{
		{
			2,
			"GLUE_RECONNECT_GAME_TYPE_FRIENDLY"
		},
		{
			5,
			"GLUE_RECONNECT_GAME_TYPE_ARENA"
		},
		{
			7,
			"GLUE_RECONNECT_GAME_TYPE_RANKED"
		},
		{
			8,
			"GLUE_RECONNECT_GAME_TYPE_UNRANKED"
		},
		{
			16,
			"GLUE_RECONNECT_GAME_TYPE_TAVERN_BRAWL"
		}
	};

	// Token: 0x040004EE RID: 1262
	private static ReconnectMgr s_instance;

	// Token: 0x040004EF RID: 1263
	private AlertPopup m_dialog;

	// Token: 0x040004F0 RID: 1264
	private ReconnectType m_reconnectType;

	// Token: 0x040004F1 RID: 1265
	private float m_reconnectStartTimestamp;

	// Token: 0x040004F2 RID: 1266
	private float m_retryStartTimestamp;

	// Token: 0x040004F3 RID: 1267
	private ReconnectMgr.SavedStartGameParameters m_savedStartGameParams = new ReconnectMgr.SavedStartGameParameters();

	// Token: 0x040004F4 RID: 1268
	private List<ReconnectMgr.TimeoutListener> m_timeoutListeners = new List<ReconnectMgr.TimeoutListener>();

	// Token: 0x040004F5 RID: 1269
	private NetCache.ProfileNoticeDisconnectedGame m_pendingReconnectNotice;

	// Token: 0x020002E2 RID: 738
	// (Invoke) Token: 0x060026D5 RID: 9941
	public delegate bool TimeoutCallback(object userData);

	// Token: 0x02000902 RID: 2306
	private class TimeoutListener : EventListener<ReconnectMgr.TimeoutCallback>
	{
		// Token: 0x0600562B RID: 22059 RVA: 0x0019E49A File Offset: 0x0019C69A
		public bool Fire()
		{
			return this.m_callback(this.m_userData);
		}
	}

	// Token: 0x02000903 RID: 2307
	private class SavedStartGameParameters
	{
		// Token: 0x04003C8C RID: 15500
		public GameType GameType;

		// Token: 0x04003C8D RID: 15501
		public ReconnectType ReconnectType;

		// Token: 0x04003C8E RID: 15502
		public GameServerInfo ServerInfo;
	}
}
