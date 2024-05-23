using System;
using System.Collections;
using System.Collections.Generic;
using bgs;
using PegasusGame;
using UnityEngine;

// Token: 0x020002E3 RID: 739
public class Gameplay : Scene
{
	// Token: 0x060026D9 RID: 9945 RVA: 0x000BD0D4 File Offset: 0x000BB2D4
	protected override void Awake()
	{
		Log.LoadingScreen.Print("Gameplay.Awake()", new object[0]);
		base.Awake();
		Gameplay.s_instance = this;
		GameState gameState = GameState.Initialize();
		if (this.ShouldHandleDisconnect())
		{
			Log.LoadingScreen.Print(LogLevel.Warning, "Gameplay.Awake() - DISCONNECTED", new object[0]);
			this.HandleDisconnect();
			return;
		}
		Network.Get().SetGameServerDisconnectEventListener(new Network.GameServerDisconnectEvent(this.OnDisconnect));
		CheatMgr.Get().RegisterCheatHandler("saveme", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_saveme), null, null, null);
		gameState.RegisterCreateGameListener(new GameState.CreateGameCallback(this.OnCreateGame));
		AssetLoader.Get().LoadGameObject("InputManager", new AssetLoader.GameObjectCallback(this.OnInputManagerLoaded), null, false);
		AssetLoader.Get().LoadGameObject("MulliganManager", new AssetLoader.GameObjectCallback(this.OnMulliganManagerLoaded), null, false);
		AssetLoader.Get().LoadGameObject("ThinkEmoteController", true, false);
		AssetLoader.Get().LoadActor("Card_Hidden", new AssetLoader.GameObjectCallback(this.OnCardDrawStandinLoaded), null, false);
		AssetLoader.Get().LoadGameObject("TurnStartManager", new AssetLoader.GameObjectCallback(this.OnTurnStartManagerLoaded), null, false);
		AssetLoader.Get().LoadGameObject("TargetReticleManager", new AssetLoader.GameObjectCallback(this.OnTargetReticleManagerLoaded), null, false);
		AssetLoader.Get().LoadGameObject("GameplayErrorManager", new AssetLoader.GameObjectCallback(this.OnGameplayErrorManagerLoaded), null, false);
		AssetLoader.Get().LoadGameObject("RemoteActionHandler", new AssetLoader.GameObjectCallback(this.OnRemoteActionHandlerLoaded), null, false);
		AssetLoader.Get().LoadGameObject("ChoiceCardMgr", new AssetLoader.GameObjectCallback(this.OnChoiceCardMgrLoaded), null, false);
		LoadingScreen.Get().RegisterFinishedTransitionListener(new LoadingScreen.FinishedTransitionCallback(this.OnTransitionFinished));
		this.m_boardProgress = -1f;
		this.ProcessGameSetupPacket();
	}

	// Token: 0x060026DA RID: 9946 RVA: 0x000BD2A1 File Offset: 0x000BB4A1
	private void OnDestroy()
	{
		Log.LoadingScreen.Print("Gameplay.OnDestroy()", new object[0]);
		Gameplay.s_instance = null;
	}

	// Token: 0x060026DB RID: 9947 RVA: 0x000BD2C0 File Offset: 0x000BB4C0
	private void Start()
	{
		Log.LoadingScreen.Print("Gameplay.Start()", new object[0]);
		Network network = Network.Get();
		network.AddBnetErrorListener(new Network.BnetErrorCallback(this.OnBnetError));
		network.RegisterNetHandler(19, new Network.NetHandler(this.OnPowerHistory), null);
		network.RegisterNetHandler(14, new Network.NetHandler(this.OnAllOptions), null);
		network.RegisterNetHandler(17, new Network.NetHandler(this.OnEntityChoices), null);
		network.RegisterNetHandler(13, new Network.NetHandler(this.OnEntitiesChosen), null);
		network.RegisterNetHandler(15, new Network.NetHandler(this.OnUserUI), null);
		network.RegisterNetHandler(10, new Network.NetHandler(this.OnOptionRejected), null);
		network.RegisterNetHandler(9, new Network.NetHandler(this.OnTurnTimerUpdate), null);
		network.RegisterNetHandler(24, new Network.NetHandler(this.OnSpectatorNotify), null);
		network.GetGameState();
	}

	// Token: 0x060026DC RID: 9948 RVA: 0x000BD3D8 File Offset: 0x000BB5D8
	private void Update()
	{
		this.CheckCriticalAssetLoads();
		Network.Get().ProcessNetwork();
		if (this.IsDoneUpdatingGame())
		{
			this.HandleLastFatalBnetError();
			return;
		}
		if (GameMgr.Get().IsFindingGame())
		{
			return;
		}
		if (this.m_unloading)
		{
			return;
		}
		if (SceneMgr.Get().WillTransition())
		{
			return;
		}
		if (!this.AreCriticalAssetsLoaded())
		{
			return;
		}
		GameState.Get().Update();
	}

	// Token: 0x060026DD RID: 9949 RVA: 0x000BD44A File Offset: 0x000BB64A
	private void OnGUI()
	{
		this.LayoutProgressGUI();
	}

	// Token: 0x060026DE RID: 9950 RVA: 0x000BD454 File Offset: 0x000BB654
	private void LayoutProgressGUI()
	{
		if (this.m_boardProgress < 0f)
		{
			return;
		}
		Vector2 vector;
		vector..ctor(150f, 30f);
		Vector2 vector2;
		vector2..ctor((float)Screen.width * 0.5f - vector.x * 0.5f, (float)Screen.height * 0.5f - vector.y * 0.5f);
		GUI.Box(new Rect(vector2.x, vector2.y, vector.x, vector.y), string.Empty);
		GUI.Box(new Rect(vector2.x, vector2.y, this.m_boardProgress * vector.x, vector.y), string.Empty);
		GUI.TextField(new Rect(vector2.x, vector2.y, vector.x, vector.y), string.Format("{0:0}%", this.m_boardProgress * 100f));
	}

	// Token: 0x060026DF RID: 9951 RVA: 0x000BD55D File Offset: 0x000BB75D
	public static Gameplay Get()
	{
		return Gameplay.s_instance;
	}

	// Token: 0x060026E0 RID: 9952 RVA: 0x000BD564 File Offset: 0x000BB764
	public override void PreUnload()
	{
		this.m_unloading = true;
		if (Board.Get() != null && BoardCameras.Get() != null)
		{
			LoadingScreen.Get().SetFreezeFrameCamera(Camera.main);
			LoadingScreen.Get().SetTransitionAudioListener(BoardCameras.Get().GetAudioListener());
		}
	}

	// Token: 0x060026E1 RID: 9953 RVA: 0x000BD5BB File Offset: 0x000BB7BB
	public override bool IsUnloading()
	{
		return this.m_unloading;
	}

	// Token: 0x060026E2 RID: 9954 RVA: 0x000BD5C4 File Offset: 0x000BB7C4
	public override void Unload()
	{
		Log.LoadingScreen.Print("Gameplay.Unload()", new object[0]);
		bool flag = this.IsLeavingGameUnfinished();
		GameState.Shutdown();
		Network network = Network.Get();
		network.RemoveGameServerDisconnectEventListener(new Network.GameServerDisconnectEvent(this.OnDisconnect));
		network.RemoveBnetErrorListener(new Network.BnetErrorCallback(this.OnBnetError));
		network.RemoveNetHandler(19, new Network.NetHandler(this.OnPowerHistory));
		network.RemoveNetHandler(14, new Network.NetHandler(this.OnAllOptions));
		network.RemoveNetHandler(17, new Network.NetHandler(this.OnEntityChoices));
		network.RemoveNetHandler(13, new Network.NetHandler(this.OnEntitiesChosen));
		network.RemoveNetHandler(15, new Network.NetHandler(this.OnUserUI));
		network.RemoveNetHandler(10, new Network.NetHandler(this.OnOptionRejected));
		network.RemoveNetHandler(9, new Network.NetHandler(this.OnTurnTimerUpdate));
		network.RemoveNetHandler(24, new Network.NetHandler(this.OnSpectatorNotify));
		CheatMgr.Get().UnregisterCheatHandler("saveme", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_saveme));
		if (flag)
		{
			if (GameMgr.Get().IsPendingAutoConcede())
			{
				Network.AutoConcede();
				GameMgr.Get().SetPendingAutoConcede(false);
			}
			Network.DisconnectFromGameServer();
		}
		foreach (NameBanner nameBanner in this.m_nameBanners)
		{
			nameBanner.Unload();
		}
		if (this.m_nameBannerGamePlayPhone != null)
		{
			this.m_nameBannerGamePlayPhone.Unload();
		}
		this.m_unloading = false;
	}

	// Token: 0x060026E3 RID: 9955 RVA: 0x000BD7A4 File Offset: 0x000BB9A4
	public void RemoveClassNames()
	{
		foreach (NameBanner nameBanner in this.m_nameBanners)
		{
			nameBanner.FadeClass();
		}
	}

	// Token: 0x060026E4 RID: 9956 RVA: 0x000BD800 File Offset: 0x000BBA00
	public void RemoveNameBanners()
	{
		foreach (NameBanner nameBanner in this.m_nameBanners)
		{
			Object.Destroy(nameBanner.gameObject);
		}
		this.m_nameBanners.Clear();
	}

	// Token: 0x060026E5 RID: 9957 RVA: 0x000BD86C File Offset: 0x000BBA6C
	public void AddGamePlayNameBannerPhone()
	{
		if (this.m_nameBannerGamePlayPhone == null)
		{
			AssetLoader.Get().LoadGameObject("NameBannerGamePlay_phone", new AssetLoader.GameObjectCallback(this.OnPlayerBannerLoaded), Player.Side.OPPOSING, false);
		}
	}

	// Token: 0x060026E6 RID: 9958 RVA: 0x000BD8A2 File Offset: 0x000BBAA2
	public void RemoveGamePlayNameBannerPhone()
	{
		if (this.m_nameBannerGamePlayPhone != null)
		{
			this.m_nameBannerGamePlayPhone.Unload();
		}
	}

	// Token: 0x060026E7 RID: 9959 RVA: 0x000BD8C0 File Offset: 0x000BBAC0
	public void UpdateFriendlySideMedalChange(MedalInfoTranslator medalInfo)
	{
		foreach (NameBanner nameBanner in this.m_nameBanners)
		{
			if (nameBanner.GetPlayerSide() == Player.Side.FRIENDLY)
			{
				nameBanner.UpdateMedalChange(medalInfo);
			}
		}
	}

	// Token: 0x060026E8 RID: 9960 RVA: 0x000BD92C File Offset: 0x000BBB2C
	public void UpdateEnemySideNameBannerName(string newName)
	{
		foreach (NameBanner nameBanner in this.m_nameBanners)
		{
			if (nameBanner.GetPlayerSide() == Player.Side.OPPOSING)
			{
				nameBanner.SetName(newName);
			}
		}
	}

	// Token: 0x060026E9 RID: 9961 RVA: 0x000BD998 File Offset: 0x000BBB98
	public Actor GetCardDrawStandIn()
	{
		return this.m_cardDrawStandIn;
	}

	// Token: 0x060026EA RID: 9962 RVA: 0x000BD9A0 File Offset: 0x000BBBA0
	public void SetGameStateBusy(bool busy, float delay)
	{
		if (delay <= Mathf.Epsilon)
		{
			GameState.Get().SetBusy(busy);
		}
		else
		{
			base.StartCoroutine(this.SetGameStateBusyWithDelay(busy, delay));
		}
	}

	// Token: 0x060026EB RID: 9963 RVA: 0x000BD9CC File Offset: 0x000BBBCC
	public void SwapCardBacks()
	{
		int cardBackId = GameState.Get().GetOpposingSidePlayer().GetCardBackId();
		int cardBackId2 = GameState.Get().GetFriendlySidePlayer().GetCardBackId();
		GameState.Get().GetOpposingSidePlayer().SetCardBackId(cardBackId2);
		GameState.Get().GetFriendlySidePlayer().SetCardBackId(cardBackId);
		CardBackManager.Get().SetGameCardBackIDs(cardBackId, cardBackId2);
	}

	// Token: 0x060026EC RID: 9964 RVA: 0x000BDA28 File Offset: 0x000BBC28
	private void ProcessGameSetupPacket()
	{
		Network.GameSetup gameSetup = GameMgr.Get().GetGameSetup();
		this.LoadBoard(gameSetup);
		GameState.Get().OnGameSetup(gameSetup);
	}

	// Token: 0x060026ED RID: 9965 RVA: 0x000BDA52 File Offset: 0x000BBC52
	private bool IsHandlingNetworkProblem()
	{
		return this.ShouldHandleDisconnect() || this.m_handleLastFatalBnetErrorNow;
	}

	// Token: 0x060026EE RID: 9966 RVA: 0x000BDA70 File Offset: 0x000BBC70
	private bool ShouldHandleDisconnect()
	{
		return !Network.IsConnectedToGameServer() && !Network.WasGameConceded() && ((Network.WasDisconnectRequested() && GameMgr.Get() != null && GameMgr.Get().IsSpectator() && !GameState.Get().IsGameOverNowOrPending()) || GameState.Get() == null || !GameState.Get().IsGameOverNowOrPending());
	}

	// Token: 0x060026EF RID: 9967 RVA: 0x000BDAE8 File Offset: 0x000BBCE8
	private void OnDisconnect(BattleNetErrors error)
	{
		if (!this.ShouldHandleDisconnect())
		{
			return;
		}
		if (error != 3005)
		{
			return;
		}
		Network.Get().RemoveGameServerDisconnectEventListener(new Network.GameServerDisconnectEvent(this.OnDisconnect));
		this.HandleDisconnect();
	}

	// Token: 0x060026F0 RID: 9968 RVA: 0x000BDB2C File Offset: 0x000BBD2C
	private void HandleDisconnect()
	{
		Log.GameMgr.PrintWarning("Gameplay is handling a game disconnect.", new object[0]);
		if (ReconnectMgr.Get().ReconnectFromGameplay())
		{
			return;
		}
		if (SpectatorManager.Get().HandleDisconnectFromGameplay())
		{
			return;
		}
		DisconnectMgr.Get().DisconnectFromGameplay();
	}

	// Token: 0x060026F1 RID: 9969 RVA: 0x000BDB78 File Offset: 0x000BBD78
	private bool IsDoneUpdatingGame()
	{
		return this.m_handleLastFatalBnetErrorNow || (!Network.IsConnectedToGameServer() && !GameState.Get().HasPowersToProcess() && GameState.Get().IsGameOver());
	}

	// Token: 0x060026F2 RID: 9970 RVA: 0x000BDBC4 File Offset: 0x000BBDC4
	private bool OnBnetError(BnetErrorInfo info, object userData)
	{
		if (Network.Get().OnIgnorableBnetError(info))
		{
			return true;
		}
		if (this.m_handleLastFatalBnetErrorNow)
		{
			return true;
		}
		this.m_lastFatalBnetErrorInfo = info;
		BattleNetErrors error = info.GetError();
		BattleNetErrors battleNetErrors = error;
		if (battleNetErrors == 11 || battleNetErrors == 60)
		{
			this.m_handleLastFatalBnetErrorNow = true;
		}
		return true;
	}

	// Token: 0x060026F3 RID: 9971 RVA: 0x000BDC24 File Offset: 0x000BBE24
	private void OnBnetErrorResponse(AlertPopup.Response response, object userData)
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

	// Token: 0x060026F4 RID: 9972 RVA: 0x000BDC5C File Offset: 0x000BBE5C
	private void HandleLastFatalBnetError()
	{
		if (this.m_lastFatalBnetErrorInfo == null)
		{
			return;
		}
		if (this.m_handleLastFatalBnetErrorNow)
		{
			Network.Get().OnFatalBnetError(this.m_lastFatalBnetErrorInfo);
			this.m_handleLastFatalBnetErrorNow = false;
		}
		else
		{
			string key = (!ApplicationMgr.AllowResetFromFatalError) ? "GAMEPLAY_DISCONNECT_BODY" : "GAMEPLAY_DISCONNECT_BODY_RESET";
			if (GameMgr.Get().IsSpectator())
			{
				key = ((!ApplicationMgr.AllowResetFromFatalError) ? "GAMEPLAY_SPECTATOR_DISCONNECT_BODY" : "GAMEPLAY_SPECTATOR_DISCONNECT_BODY_RESET");
			}
			AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
			popupInfo.m_headerText = GameStrings.Get("GAMEPLAY_DISCONNECT_HEADER");
			popupInfo.m_text = GameStrings.Get(key);
			popupInfo.m_showAlertIcon = true;
			popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
			popupInfo.m_responseCallback = new AlertPopup.ResponseCallback(this.OnBnetErrorResponse);
			DialogManager.Get().ShowPopup(popupInfo);
		}
		this.m_lastFatalBnetErrorInfo = null;
	}

	// Token: 0x060026F5 RID: 9973 RVA: 0x000BDD40 File Offset: 0x000BBF40
	private void OnPowerHistory()
	{
		List<Network.PowerHistory> powerHistory = Network.GetPowerHistory();
		Log.LoadingScreen.Print("Gameplay.OnPowerHistory() - powerList={0}", new object[]
		{
			powerHistory.Count
		});
		if (this.AreCriticalAssetsLoaded())
		{
			GameState.Get().OnPowerHistory(powerHistory);
		}
		else
		{
			this.m_queuedPowerHistory.Enqueue(powerHistory);
		}
	}

	// Token: 0x060026F6 RID: 9974 RVA: 0x000BDDA0 File Offset: 0x000BBFA0
	private void OnAllOptions()
	{
		Network.Options options = Network.GetOptions();
		Log.LoadingScreen.Print("Gameplay.OnAllOptions() - id={0}", new object[]
		{
			options.ID
		});
		GameState.Get().OnAllOptions(options);
	}

	// Token: 0x060026F7 RID: 9975 RVA: 0x000BDDE4 File Offset: 0x000BBFE4
	private void OnEntityChoices()
	{
		Network.EntityChoices entityChoices = Network.GetEntityChoices();
		Log.LoadingScreen.Print("Gameplay.OnEntityChoices() - id={0}", new object[]
		{
			entityChoices.ID
		});
		GameState.Get().OnEntityChoices(entityChoices);
	}

	// Token: 0x060026F8 RID: 9976 RVA: 0x000BDE28 File Offset: 0x000BC028
	private void OnEntitiesChosen()
	{
		Network.EntitiesChosen entitiesChosen = Network.GetEntitiesChosen();
		GameState.Get().OnEntitiesChosen(entitiesChosen);
	}

	// Token: 0x060026F9 RID: 9977 RVA: 0x000BDE46 File Offset: 0x000BC046
	private void OnUserUI()
	{
		if (RemoteActionHandler.Get())
		{
			RemoteActionHandler.Get().HandleAction(Network.GetUserUI());
		}
	}

	// Token: 0x060026FA RID: 9978 RVA: 0x000BDE68 File Offset: 0x000BC068
	private void OnOptionRejected()
	{
		int nackOption = Network.GetNAckOption();
		GameState.Get().OnOptionRejected(nackOption);
	}

	// Token: 0x060026FB RID: 9979 RVA: 0x000BDE88 File Offset: 0x000BC088
	private void OnTurnTimerUpdate()
	{
		Network.TurnTimerInfo turnTimerInfo = Network.GetTurnTimerInfo();
		GameState.Get().OnTurnTimerUpdate(turnTimerInfo);
	}

	// Token: 0x060026FC RID: 9980 RVA: 0x000BDEA8 File Offset: 0x000BC0A8
	private void OnSpectatorNotify()
	{
		SpectatorNotify spectatorNotify = Network.GetSpectatorNotify();
		GameState.Get().OnSpectatorNotifyEvent(spectatorNotify);
	}

	// Token: 0x060026FD RID: 9981 RVA: 0x000BDEC6 File Offset: 0x000BC0C6
	private bool AreCriticalAssetsLoaded()
	{
		return this.m_criticalAssetsLoaded;
	}

	// Token: 0x060026FE RID: 9982 RVA: 0x000BDED0 File Offset: 0x000BC0D0
	private bool CheckCriticalAssetLoads()
	{
		if (this.m_criticalAssetsLoaded)
		{
			return true;
		}
		if (Board.Get() == null)
		{
			return false;
		}
		if (BoardCameras.Get() == null)
		{
			return false;
		}
		if (BoardStandardGame.Get() == null)
		{
			return false;
		}
		if (GameMgr.Get().IsTutorial() && BoardTutorial.Get() == null)
		{
			return false;
		}
		if (MulliganManager.Get() == null)
		{
			return false;
		}
		if (TurnStartManager.Get() == null)
		{
			return false;
		}
		if (TargetReticleManager.Get() == null)
		{
			return false;
		}
		if (GameplayErrorManager.Get() == null)
		{
			return false;
		}
		if (EndTurnButton.Get() == null)
		{
			return false;
		}
		if (BigCard.Get() == null)
		{
			return false;
		}
		if (CardTypeBanner.Get() == null)
		{
			return false;
		}
		if (TurnTimer.Get() == null)
		{
			return false;
		}
		if (CardColorSwitcher.Get() == null)
		{
			return false;
		}
		if (RemoteActionHandler.Get() == null)
		{
			return false;
		}
		if (ChoiceCardMgr.Get() == null)
		{
			return false;
		}
		if (InputManager.Get() == null)
		{
			return false;
		}
		this.m_criticalAssetsLoaded = true;
		this.ProcessQueuedPowerHistory();
		return true;
	}

	// Token: 0x060026FF RID: 9983 RVA: 0x000BE028 File Offset: 0x000BC228
	private void InitCardBacks()
	{
		int cardBackId = GameState.Get().GetFriendlySidePlayer().GetCardBackId();
		int cardBackId2 = GameState.Get().GetOpposingSidePlayer().GetCardBackId();
		CardBackManager.Get().SetGameCardBackIDs(cardBackId, cardBackId2);
	}

	// Token: 0x06002700 RID: 9984 RVA: 0x000BE064 File Offset: 0x000BC264
	private void LoadBoard(Network.GameSetup setup)
	{
		string board = Cheats.Get().GetBoard();
		string text;
		if (string.IsNullOrEmpty(board))
		{
			BoardDbfRecord record = GameDbf.Board.GetRecord(setup.Board);
			text = FileUtils.GameAssetPathToName(record.Prefab);
		}
		else
		{
			text = board;
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			text = string.Format("{0}_phone", text);
		}
		AssetLoader.Get().LoadBoard(text, new AssetLoader.GameObjectCallback(this.OnBoardLoaded), null, false);
	}

	// Token: 0x06002701 RID: 9985 RVA: 0x000BE0E0 File Offset: 0x000BC2E0
	private IEnumerator NotifyPlayersOfBoardLoad()
	{
		while (BoardStandardGame.Get() == null)
		{
			yield return null;
		}
		Map<int, Player> playerMap = GameState.Get().GetPlayerMap();
		foreach (Player player in playerMap.Values)
		{
			player.OnBoardLoaded();
		}
		yield break;
	}

	// Token: 0x06002702 RID: 9986 RVA: 0x000BE0F4 File Offset: 0x000BC2F4
	private void OnBoardLoaded(string name, GameObject go, object callbackData)
	{
		this.m_boardProgress = -1f;
		if (go == null)
		{
			Debug.LogError(string.Format("Gameplay.OnBoardLoaded() - FAILED to load board \"{0}\"", name));
			return;
		}
		if (this.IsHandlingNetworkProblem())
		{
			Object.Destroy(go);
			return;
		}
		go.GetComponent<Board>().SetBoardDbId(GameMgr.Get().GetGameSetup().Board);
		string name2 = (!UniversalInputManager.UsePhoneUI) ? "BoardCameras" : "BoardCameras_phone";
		AssetLoader.Get().LoadGameObject(name2, new AssetLoader.GameObjectCallback(this.OnBoardCamerasLoaded), null, false);
		AssetLoader.Get().LoadGameObject("BoardStandardGame", new AssetLoader.GameObjectCallback(this.OnBoardStandardGameLoaded), null, false);
		if (GameMgr.Get().IsTutorial())
		{
			AssetLoader.Get().LoadGameObject("BoardTutorial", new AssetLoader.GameObjectCallback(this.OnBoardTutorialLoaded), null, false);
		}
	}

	// Token: 0x06002703 RID: 9987 RVA: 0x000BE1D9 File Offset: 0x000BC3D9
	private void OnBoardProgressUpdate(string name, float progress, object callbackData)
	{
		this.m_boardProgress = progress;
	}

	// Token: 0x06002704 RID: 9988 RVA: 0x000BE1E4 File Offset: 0x000BC3E4
	private void OnBoardCamerasLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("Gameplay.OnBoardCamerasLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		if (this.IsHandlingNetworkProblem())
		{
			Object.Destroy(go);
			return;
		}
		go.transform.parent = Board.Get().transform;
		PegUI.Get().SetInputCamera(Camera.main);
		AssetLoader.Get().LoadActor("CardTypeBanner", false, false);
		AssetLoader.Get().LoadActor("BigCard", false, false);
	}

	// Token: 0x06002705 RID: 9989 RVA: 0x000BE268 File Offset: 0x000BC468
	private void OnBoardStandardGameLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("Gameplay.OnBoardStandardGameLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		if (this.IsHandlingNetworkProblem())
		{
			Object.Destroy(go);
			return;
		}
		go.transform.parent = Board.Get().transform;
		AssetLoader.Get().LoadActor("EndTurnButton", new AssetLoader.GameObjectCallback(this.OnEndTurnButtonLoaded), null, false);
		AssetLoader.Get().LoadActor("TurnTimer", new AssetLoader.GameObjectCallback(this.OnTurnTimerLoaded), null, false);
	}

	// Token: 0x06002706 RID: 9990 RVA: 0x000BE2F8 File Offset: 0x000BC4F8
	private void OnBoardTutorialLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("Gameplay.OnBoardTutorialLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		if (this.IsHandlingNetworkProblem())
		{
			Object.Destroy(go);
			return;
		}
		go.transform.parent = Board.Get().transform;
	}

	// Token: 0x06002707 RID: 9991 RVA: 0x000BE34C File Offset: 0x000BC54C
	private void OnEndTurnButtonLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("Gameplay.OnEndTurnButtonLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		if (this.IsHandlingNetworkProblem())
		{
			Object.Destroy(go);
			return;
		}
		EndTurnButton component = go.GetComponent<EndTurnButton>();
		if (component == null)
		{
			Debug.LogError(string.Format("Gameplay.OnEndTurnButtonLoaded() - ERROR \"{0}\" has no {1} component", name, typeof(EndTurnButton)));
			return;
		}
		component.transform.position = Board.Get().FindBone("EndTurnButton").position;
		foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>())
		{
			if (!renderer.gameObject.GetComponent<TextMesh>())
			{
				renderer.material.color = Board.Get().m_EndTurnButtonColor;
			}
		}
	}

	// Token: 0x06002708 RID: 9992 RVA: 0x000BE420 File Offset: 0x000BC620
	private void OnTurnTimerLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("Gameplay.OnTurnTimerLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		if (this.IsHandlingNetworkProblem())
		{
			Object.Destroy(go);
			return;
		}
		TurnTimer component = go.GetComponent<TurnTimer>();
		if (component == null)
		{
			Debug.LogError(string.Format("Gameplay.OnTurnTimerLoaded() - ERROR \"{0}\" has no {1} component", name, typeof(TurnTimer)));
			return;
		}
		component.transform.position = Board.Get().FindBone("TurnTimerBone").position;
	}

	// Token: 0x06002709 RID: 9993 RVA: 0x000BE4AC File Offset: 0x000BC6AC
	private void OnRemoteActionHandlerLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("Gameplay.OnRemoteActionHandlerLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		if (this.IsHandlingNetworkProblem())
		{
			Object.Destroy(go);
			return;
		}
		go.transform.parent = base.transform;
	}

	// Token: 0x0600270A RID: 9994 RVA: 0x000BE4FC File Offset: 0x000BC6FC
	private void OnChoiceCardMgrLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("Gameplay.OnChoiceCardMgrLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		if (this.IsHandlingNetworkProblem())
		{
			Object.Destroy(go);
			return;
		}
		go.transform.parent = base.transform;
	}

	// Token: 0x0600270B RID: 9995 RVA: 0x000BE54C File Offset: 0x000BC74C
	private void OnInputManagerLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("Gameplay.OnInputManagerLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		if (this.IsHandlingNetworkProblem())
		{
			Object.Destroy(go);
			return;
		}
		go.transform.parent = base.transform;
	}

	// Token: 0x0600270C RID: 9996 RVA: 0x000BE59C File Offset: 0x000BC79C
	private void OnMulliganManagerLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("Gameplay.OnMulliganManagerLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		if (this.IsHandlingNetworkProblem())
		{
			Object.Destroy(go);
			return;
		}
		go.transform.parent = base.transform;
	}

	// Token: 0x0600270D RID: 9997 RVA: 0x000BE5EC File Offset: 0x000BC7EC
	private void OnTurnStartManagerLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("Gameplay.OnTurnStartManagerLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		if (this.IsHandlingNetworkProblem())
		{
			Object.Destroy(go);
			return;
		}
		go.transform.parent = base.transform;
	}

	// Token: 0x0600270E RID: 9998 RVA: 0x000BE63C File Offset: 0x000BC83C
	private void OnTargetReticleManagerLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("Gameplay.OnTargetReticleManagerLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		if (this.IsHandlingNetworkProblem())
		{
			Object.Destroy(go);
			return;
		}
		go.transform.parent = base.transform;
		TargetReticleManager.Get().PreloadTargetArrows();
	}

	// Token: 0x0600270F RID: 9999 RVA: 0x000BE694 File Offset: 0x000BC894
	private void OnGameplayErrorManagerLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("Gameplay.GameplayErrorManagerLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		if (this.IsHandlingNetworkProblem())
		{
			Object.Destroy(go);
			return;
		}
		go.transform.parent = base.transform;
	}

	// Token: 0x06002710 RID: 10000 RVA: 0x000BE6E4 File Offset: 0x000BC8E4
	private void OnPlayerBannerLoaded(string name, GameObject go, object callbackData)
	{
		Player.Side side = (Player.Side)((int)callbackData);
		if (go == null)
		{
			Debug.LogError(string.Format("Gameplay.OnPlayerBannerLoaded() - FAILED to load \"{0}\" side={1}", name, side.ToString()));
			return;
		}
		if (this.IsHandlingNetworkProblem())
		{
			Object.Destroy(go);
			return;
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			NameBannerGamePlayPhone component = go.GetComponent<NameBannerGamePlayPhone>();
			if (component != null)
			{
				this.m_nameBannerGamePlayPhone = component;
				this.m_nameBannerGamePlayPhone.SetPlayerSide(side);
			}
			else
			{
				NameBanner component2 = go.GetComponent<NameBanner>();
				component2.SetPlayerSide(side);
				this.m_nameBanners.Add(component2);
			}
		}
		else
		{
			NameBanner component3 = go.GetComponent<NameBanner>();
			component3.SetPlayerSide(side);
			this.m_nameBanners.Add(component3);
		}
	}

	// Token: 0x06002711 RID: 10001 RVA: 0x000BE7A8 File Offset: 0x000BC9A8
	private void OnCardDrawStandinLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("Gameplay.OnCardDrawStandinLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		if (this.IsHandlingNetworkProblem())
		{
			Object.Destroy(go);
			return;
		}
		this.m_cardDrawStandIn = go.GetComponent<Actor>();
		CardBackDisplay componentInChildren = go.GetComponentInChildren<CardBackDisplay>();
		componentInChildren.SetCardBack(true);
		this.m_cardDrawStandIn.Hide();
	}

	// Token: 0x06002712 RID: 10002 RVA: 0x000BE80C File Offset: 0x000BCA0C
	private void OnTransitionFinished(bool cutoff, object userData)
	{
		LoadingScreen.Get().UnregisterFinishedTransitionListener(new LoadingScreen.FinishedTransitionCallback(this.OnTransitionFinished));
		if (cutoff)
		{
			return;
		}
		if (this.IsHandlingNetworkProblem())
		{
			return;
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			if (GameState.Get().IsMulliganPhase())
			{
				AssetLoader.Get().LoadGameObject("NameBannerRight_phone", new AssetLoader.GameObjectCallback(this.OnPlayerBannerLoaded), Player.Side.FRIENDLY, false);
				AssetLoader.Get().LoadGameObject("NameBanner_phone", new AssetLoader.GameObjectCallback(this.OnPlayerBannerLoaded), Player.Side.OPPOSING, false);
			}
			else if (!GameMgr.Get().IsTutorial())
			{
				this.AddGamePlayNameBannerPhone();
			}
		}
		else
		{
			string name = "NameBanner";
			if (!string.IsNullOrEmpty(GameState.Get().GetGameEntity().GetAlternatePlayerName()))
			{
				name = "NameBannerLong";
			}
			AssetLoader.Get().LoadGameObject(name, new AssetLoader.GameObjectCallback(this.OnPlayerBannerLoaded), Player.Side.FRIENDLY, false);
			AssetLoader.Get().LoadGameObject("NameBanner", new AssetLoader.GameObjectCallback(this.OnPlayerBannerLoaded), Player.Side.OPPOSING, false);
		}
	}

	// Token: 0x06002713 RID: 10003 RVA: 0x000BE930 File Offset: 0x000BCB30
	private void ProcessQueuedPowerHistory()
	{
		while (this.m_queuedPowerHistory.Count > 0)
		{
			List<Network.PowerHistory> powerList = this.m_queuedPowerHistory.Dequeue();
			GameState.Get().OnPowerHistory(powerList);
		}
	}

	// Token: 0x06002714 RID: 10004 RVA: 0x000BE96C File Offset: 0x000BCB6C
	private bool IsLeavingGameUnfinished()
	{
		return (GameState.Get() == null || !GameState.Get().IsGameOver()) && !GameMgr.Get().IsReconnect() && !SceneMgr.Get().IsModeRequested(SceneMgr.Mode.FATAL_ERROR);
	}

	// Token: 0x06002715 RID: 10005 RVA: 0x000BE9BC File Offset: 0x000BCBBC
	private void OnCreateGame(GameState.CreateGamePhase phase, object userData)
	{
		if (phase == GameState.CreateGamePhase.CREATING)
		{
			this.InitCardBacks();
			base.StartCoroutine(this.NotifyPlayersOfBoardLoad());
		}
		else if (phase == GameState.CreateGamePhase.CREATED)
		{
			CardBackManager.Get().UpdateAllCardBacks();
		}
	}

	// Token: 0x06002716 RID: 10006 RVA: 0x000BE9FC File Offset: 0x000BCBFC
	private IEnumerator SetGameStateBusyWithDelay(bool busy, float delay)
	{
		yield return new WaitForSeconds(delay);
		GameState.Get().SetBusy(busy);
		yield break;
	}

	// Token: 0x06002717 RID: 10007 RVA: 0x000BEA2C File Offset: 0x000BCC2C
	private bool OnProcessCheat_saveme(string func, string[] args, string rawArgs)
	{
		GameState.Get().DebugNukeServerBlocks();
		return true;
	}

	// Token: 0x04001729 RID: 5929
	private static Gameplay s_instance;

	// Token: 0x0400172A RID: 5930
	private bool m_unloading;

	// Token: 0x0400172B RID: 5931
	private BnetErrorInfo m_lastFatalBnetErrorInfo;

	// Token: 0x0400172C RID: 5932
	private bool m_handleLastFatalBnetErrorNow;

	// Token: 0x0400172D RID: 5933
	private float m_boardProgress;

	// Token: 0x0400172E RID: 5934
	private List<NameBanner> m_nameBanners = new List<NameBanner>();

	// Token: 0x0400172F RID: 5935
	private NameBannerGamePlayPhone m_nameBannerGamePlayPhone;

	// Token: 0x04001730 RID: 5936
	private Actor m_cardDrawStandIn;

	// Token: 0x04001731 RID: 5937
	private bool m_criticalAssetsLoaded;

	// Token: 0x04001732 RID: 5938
	private Queue<List<Network.PowerHistory>> m_queuedPowerHistory = new Queue<List<Network.PowerHistory>>();
}
