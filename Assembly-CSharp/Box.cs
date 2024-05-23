using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001A0 RID: 416
public class Box : MonoBehaviour
{
	// Token: 0x06001B0C RID: 6924 RVA: 0x0007EE38 File Offset: 0x0007D038
	private void Awake()
	{
		Log.LoadingScreen.Print("Box.Awake()", new object[0]);
		Box.s_instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
		this.InitializeStateConfigs();
		if (LoadingScreen.Get() != null)
		{
			LoadingScreen.Get().NotifyMainSceneObjectAwoke(base.gameObject);
		}
		this.m_originalLeftDoorLayer = (GameLayer)this.m_LeftDoor.gameObject.layer;
		this.m_originalRightDoorLayer = (GameLayer)this.m_RightDoor.gameObject.layer;
		this.m_originalDrawerLayer = (GameLayer)this.m_Drawer.gameObject.layer;
		if (UniversalInputManager.UsePhoneUI)
		{
			if (TransformUtil.PhoneAspectRatioScale() < 0.99f)
			{
				GameUtils.InstantiateGameObject("Letterboxing", this.m_letterboxingContainer, false);
			}
			GameObject gameObject = AssetLoader.Get().LoadGameObject("RibbonButtons_Phone", true, false);
			this.m_ribbonButtons = gameObject.GetComponent<RibbonButtonsUI>();
			this.m_ribbonButtons.Toggle(false);
			GameUtils.SetParent(gameObject, this.m_rootObject, false);
			AssetLoader.Get().LoadTexture("TheBox_Top_phone", new AssetLoader.ObjectCallback(this.OnBoxTopPhoneTextureLoaded), null, false);
		}
	}

	// Token: 0x06001B0D RID: 6925 RVA: 0x0007EF58 File Offset: 0x0007D158
	private void Start()
	{
		this.InitializeNet();
		this.InitializeComponents();
		this.InitializeState();
		bool flag = GameUtils.ShouldShowSetRotationIntro();
		if (flag)
		{
			this.m_DiskCenter.SetActive(false);
		}
		else
		{
			this.m_DiskCenter.SetActive(true);
		}
		this.InitializeUI();
		if (DemoMgr.Get().IsExpoDemo())
		{
			this.m_StoreButton.gameObject.SetActive(false);
			this.m_Drawer.gameObject.SetActive(false);
			this.m_QuestLogButton.gameObject.SetActive(false);
		}
		TavernBrawlManager tavernBrawlManager = TavernBrawlManager.Get();
		if (tavernBrawlManager.IsTavernBrawlInfoReady)
		{
			this.DoTavernBrawlButtonInitialization();
		}
		else
		{
			tavernBrawlManager.OnTavernBrawlUpdated += new Action(this.DoTavernBrawlButtonInitialization);
		}
	}

	// Token: 0x06001B0E RID: 6926 RVA: 0x0007F018 File Offset: 0x0007D218
	private void OnDestroy()
	{
		Log.LoadingScreen.Print("Box.OnDestroy()", new object[0]);
		TavernBrawlManager.Get().OnTavernBrawlUpdated -= new Action(this.DoTavernBrawlButtonInitialization);
		NetCache.Get().RemoveUpdatedListener(typeof(NetCache.NetCacheFeatures), new Action(this.DoTavernBrawlButtonInitialization));
		this.ShutdownState();
		this.ShutdownNet();
		Box.s_instance = null;
	}

	// Token: 0x06001B0F RID: 6927 RVA: 0x0007F082 File Offset: 0x0007D282
	public static Box Get()
	{
		return Box.s_instance;
	}

	// Token: 0x06001B10 RID: 6928 RVA: 0x0007F089 File Offset: 0x0007D289
	public Camera GetCamera()
	{
		return this.m_Camera.GetComponent<Camera>();
	}

	// Token: 0x06001B11 RID: 6929 RVA: 0x0007F096 File Offset: 0x0007D296
	public BoxCamera GetBoxCamera()
	{
		return this.m_Camera;
	}

	// Token: 0x06001B12 RID: 6930 RVA: 0x0007F09E File Offset: 0x0007D29E
	public Camera GetNoFxCamera()
	{
		return this.m_NoFxCamera;
	}

	// Token: 0x06001B13 RID: 6931 RVA: 0x0007F0A6 File Offset: 0x0007D2A6
	public AudioListener GetAudioListener()
	{
		return this.m_AudioListener;
	}

	// Token: 0x06001B14 RID: 6932 RVA: 0x0007F0AE File Offset: 0x0007D2AE
	public Box.State GetState()
	{
		return this.m_state;
	}

	// Token: 0x06001B15 RID: 6933 RVA: 0x0007F0B6 File Offset: 0x0007D2B6
	public Texture2D GetTextureCompressionTestTexture()
	{
		return this.m_textureCompressionTest;
	}

	// Token: 0x06001B16 RID: 6934 RVA: 0x0007F0C0 File Offset: 0x0007D2C0
	public bool ChangeState(Box.State state)
	{
		if (state == Box.State.INVALID)
		{
			return false;
		}
		if (this.m_state == state)
		{
			return false;
		}
		if (this.HasPendingEffects())
		{
			this.QueueStateChange(state);
		}
		else
		{
			this.ChangeStateNow(state);
		}
		return true;
	}

	// Token: 0x06001B17 RID: 6935 RVA: 0x0007F104 File Offset: 0x0007D304
	public void UpdateState()
	{
		if (this.m_state == Box.State.STARTUP)
		{
			this.UpdateState_Startup();
		}
		else if (this.m_state == Box.State.PRESS_START)
		{
			this.UpdateState_PressStart();
		}
		else if (this.m_state == Box.State.LOADING_HUB)
		{
			this.UpdateState_LoadingHub();
		}
		else if (this.m_state == Box.State.LOADING)
		{
			this.UpdateState_Loading();
		}
		else if (this.m_state == Box.State.HUB)
		{
			this.UpdateState_Hub();
		}
		else if (this.m_state == Box.State.HUB_WITH_DRAWER)
		{
			this.UpdateState_HubWithDrawer();
		}
		else if (this.m_state == Box.State.OPEN)
		{
			this.UpdateState_Open();
		}
		else if (this.m_state == Box.State.CLOSED)
		{
			this.UpdateState_Closed();
		}
		else if (this.m_state == Box.State.ERROR)
		{
			this.UpdateState_Error();
		}
		else if (this.m_state == Box.State.SET_ROTATION_LOADING)
		{
			this.UpdateState_SetRotation();
		}
		else if (this.m_state == Box.State.SET_ROTATION)
		{
			this.UpdateState_SetRotation();
		}
		else if (this.m_state == Box.State.SET_ROTATION_OPEN)
		{
			this.UpdateState_SetRotationOpen();
		}
		else
		{
			Debug.LogError(string.Format("Box.UpdateState() - unhandled state {0}", this.m_state));
		}
	}

	// Token: 0x06001B18 RID: 6936 RVA: 0x0007F243 File Offset: 0x0007D443
	public BoxLightMgr GetLightMgr()
	{
		return this.m_LightMgr;
	}

	// Token: 0x06001B19 RID: 6937 RVA: 0x0007F24B File Offset: 0x0007D44B
	public BoxLightStateType GetLightState()
	{
		return this.m_LightMgr.GetActiveState();
	}

	// Token: 0x06001B1A RID: 6938 RVA: 0x0007F258 File Offset: 0x0007D458
	public void ChangeLightState(BoxLightStateType stateType)
	{
		this.m_LightMgr.ChangeState(stateType);
	}

	// Token: 0x06001B1B RID: 6939 RVA: 0x0007F266 File Offset: 0x0007D466
	public void SetLightState(BoxLightStateType stateType)
	{
		this.m_LightMgr.SetState(stateType);
	}

	// Token: 0x06001B1C RID: 6940 RVA: 0x0007F274 File Offset: 0x0007D474
	public BoxEventMgr GetEventMgr()
	{
		return this.m_EventMgr;
	}

	// Token: 0x06001B1D RID: 6941 RVA: 0x0007F27C File Offset: 0x0007D47C
	public Spell GetEventSpell(BoxEventType eventType)
	{
		return this.m_EventMgr.GetEventSpell(eventType);
	}

	// Token: 0x06001B1E RID: 6942 RVA: 0x0007F28A File Offset: 0x0007D48A
	public bool HasPendingEffects()
	{
		return this.m_pendingEffects > 0;
	}

	// Token: 0x06001B1F RID: 6943 RVA: 0x0007F295 File Offset: 0x0007D495
	public bool IsBusy()
	{
		return this.HasPendingEffects() || this.m_stateQueue.Count > 0;
	}

	// Token: 0x06001B20 RID: 6944 RVA: 0x0007F2B3 File Offset: 0x0007D4B3
	public bool IsTransitioningToSceneMode()
	{
		return this.m_transitioningToSceneMode;
	}

	// Token: 0x06001B21 RID: 6945 RVA: 0x0007F2BB File Offset: 0x0007D4BB
	public void OnAnimStarted()
	{
		this.m_pendingEffects++;
	}

	// Token: 0x06001B22 RID: 6946 RVA: 0x0007F2CC File Offset: 0x0007D4CC
	public void OnAnimFinished()
	{
		this.m_pendingEffects--;
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_OuterFrame.SetActive(false);
		}
		if (this.HasPendingEffects())
		{
			return;
		}
		if (this.m_stateQueue.Count == 0)
		{
			this.UpdateUIEvents();
			if (this.m_transitioningToSceneMode)
			{
				if (UniversalInputManager.UsePhoneUI)
				{
					bool flag = this.m_state == Box.State.HUB_WITH_DRAWER;
					if (flag != this.m_showRibbonButtons)
					{
						this.ToggleRibbonUI(flag);
					}
				}
				this.FireTransitionFinishedEvent();
				this.m_transitioningToSceneMode = false;
				if (SceneMgr.Get().GetMode() == SceneMgr.Mode.HUB && AchieveManager.Get().HasActiveQuests(true))
				{
					WelcomeQuests.Show(UserAttentionBlocker.NONE, false, null, false);
				}
			}
		}
		else
		{
			this.ChangeStateQueued();
		}
	}

	// Token: 0x06001B23 RID: 6947 RVA: 0x0007F39C File Offset: 0x0007D59C
	public void OnLoggedIn()
	{
		this.InitializeNet();
	}

	// Token: 0x06001B24 RID: 6948 RVA: 0x0007F3A4 File Offset: 0x0007D5A4
	public void AddTransitionFinishedListener(Box.TransitionFinishedCallback callback)
	{
		this.AddTransitionFinishedListener(callback, null);
	}

	// Token: 0x06001B25 RID: 6949 RVA: 0x0007F3B0 File Offset: 0x0007D5B0
	public void AddTransitionFinishedListener(Box.TransitionFinishedCallback callback, object userData)
	{
		Box.TransitionFinishedListener transitionFinishedListener = new Box.TransitionFinishedListener();
		transitionFinishedListener.SetCallback(callback);
		transitionFinishedListener.SetUserData(userData);
		if (this.m_transitionFinishedListeners.Contains(transitionFinishedListener))
		{
			return;
		}
		this.m_transitionFinishedListeners.Add(transitionFinishedListener);
	}

	// Token: 0x06001B26 RID: 6950 RVA: 0x0007F3EF File Offset: 0x0007D5EF
	public bool RemoveTransitionFinishedListener(Box.TransitionFinishedCallback callback)
	{
		return this.RemoveTransitionFinishedListener(callback, null);
	}

	// Token: 0x06001B27 RID: 6951 RVA: 0x0007F3FC File Offset: 0x0007D5FC
	public bool RemoveTransitionFinishedListener(Box.TransitionFinishedCallback callback, object userData)
	{
		Box.TransitionFinishedListener transitionFinishedListener = new Box.TransitionFinishedListener();
		transitionFinishedListener.SetCallback(callback);
		transitionFinishedListener.SetUserData(userData);
		return this.m_transitionFinishedListeners.Remove(transitionFinishedListener);
	}

	// Token: 0x06001B28 RID: 6952 RVA: 0x0007F429 File Offset: 0x0007D629
	public void AddButtonPressListener(Box.ButtonPressCallback callback)
	{
		this.AddButtonPressListener(callback, null);
	}

	// Token: 0x06001B29 RID: 6953 RVA: 0x0007F434 File Offset: 0x0007D634
	public void AddButtonPressListener(Box.ButtonPressCallback callback, object userData)
	{
		Box.ButtonPressListener buttonPressListener = new Box.ButtonPressListener();
		buttonPressListener.SetCallback(callback);
		buttonPressListener.SetUserData(userData);
		if (this.m_buttonPressListeners.Contains(buttonPressListener))
		{
			return;
		}
		this.m_buttonPressListeners.Add(buttonPressListener);
	}

	// Token: 0x06001B2A RID: 6954 RVA: 0x0007F473 File Offset: 0x0007D673
	public bool RemoveButtonPressListener(Box.ButtonPressCallback callback)
	{
		return this.RemoveButtonPressListener(callback, null);
	}

	// Token: 0x06001B2B RID: 6955 RVA: 0x0007F480 File Offset: 0x0007D680
	public bool RemoveButtonPressListener(Box.ButtonPressCallback callback, object userData)
	{
		Box.ButtonPressListener buttonPressListener = new Box.ButtonPressListener();
		buttonPressListener.SetCallback(callback);
		buttonPressListener.SetUserData(userData);
		return this.m_buttonPressListeners.Remove(buttonPressListener);
	}

	// Token: 0x06001B2C RID: 6956 RVA: 0x0007F4B0 File Offset: 0x0007D6B0
	public void SetToIgnoreFullScreenEffects(bool ignoreEffects)
	{
		if (ignoreEffects)
		{
			SceneUtils.ReplaceLayer(this.m_LeftDoor.gameObject, GameLayer.IgnoreFullScreenEffects, this.m_originalLeftDoorLayer);
			SceneUtils.ReplaceLayer(this.m_RightDoor.gameObject, GameLayer.IgnoreFullScreenEffects, this.m_originalRightDoorLayer);
			SceneUtils.ReplaceLayer(this.m_Drawer.gameObject, GameLayer.IgnoreFullScreenEffects, this.m_originalDrawerLayer);
		}
		else
		{
			SceneUtils.ReplaceLayer(this.m_LeftDoor.gameObject, this.m_originalLeftDoorLayer, GameLayer.IgnoreFullScreenEffects);
			SceneUtils.ReplaceLayer(this.m_RightDoor.gameObject, this.m_originalRightDoorLayer, GameLayer.IgnoreFullScreenEffects);
			SceneUtils.ReplaceLayer(this.m_Drawer.gameObject, this.m_originalDrawerLayer, GameLayer.IgnoreFullScreenEffects);
		}
	}

	// Token: 0x06001B2D RID: 6957 RVA: 0x0007F558 File Offset: 0x0007D758
	private void InitializeStateConfigs()
	{
		int length = Enum.GetValues(typeof(Box.State)).Length;
		this.m_stateConfigs = new Box.BoxStateConfig[length];
		this.m_stateConfigs[1] = new Box.BoxStateConfig
		{
			m_logoState = 
			{
				m_state = BoxLogo.State.HIDDEN
			},
			m_startButtonState = 
			{
				m_state = BoxStartButton.State.HIDDEN
			},
			m_fullScreenBlackState = 
			{
				m_state = true
			}
		};
		this.m_stateConfigs[2] = new Box.BoxStateConfig
		{
			m_fullScreenBlackState = 
			{
				m_ignore = true
			}
		};
		this.m_stateConfigs[4] = new Box.BoxStateConfig
		{
			m_logoState = 
			{
				m_state = BoxLogo.State.HIDDEN
			},
			m_startButtonState = 
			{
				m_state = BoxStartButton.State.HIDDEN
			},
			m_fullScreenBlackState = 
			{
				m_ignore = true
			}
		};
		this.m_stateConfigs[3] = new Box.BoxStateConfig
		{
			m_logoState = 
			{
				m_state = BoxLogo.State.HIDDEN
			},
			m_startButtonState = 
			{
				m_state = BoxStartButton.State.HIDDEN
			},
			m_drawerState = 
			{
				m_ignore = true
			},
			m_camState = 
			{
				m_ignore = true
			},
			m_fullScreenBlackState = 
			{
				m_ignore = true
			}
		};
		this.m_stateConfigs[5] = new Box.BoxStateConfig
		{
			m_logoState = 
			{
				m_state = BoxLogo.State.HIDDEN
			},
			m_startButtonState = 
			{
				m_state = BoxStartButton.State.HIDDEN
			},
			m_diskState = 
			{
				m_state = BoxDisk.State.MAINMENU
			},
			m_fullScreenBlackState = 
			{
				m_ignore = true
			}
		};
		this.m_stateConfigs[6] = new Box.BoxStateConfig
		{
			m_logoState = 
			{
				m_state = BoxLogo.State.HIDDEN
			},
			m_startButtonState = 
			{
				m_state = BoxStartButton.State.HIDDEN
			},
			m_diskState = 
			{
				m_state = BoxDisk.State.MAINMENU
			},
			m_drawerState = 
			{
				m_state = BoxDrawer.State.OPENED
			},
			m_camState = 
			{
				m_state = BoxCamera.State.CLOSED_WITH_DRAWER
			},
			m_fullScreenBlackState = 
			{
				m_ignore = true
			}
		};
		this.m_stateConfigs[7] = new Box.BoxStateConfig
		{
			m_logoState = 
			{
				m_state = BoxLogo.State.HIDDEN
			},
			m_startButtonState = 
			{
				m_state = BoxStartButton.State.HIDDEN
			},
			m_doorState = 
			{
				m_state = BoxDoor.State.OPENED
			},
			m_drawerState = 
			{
				m_state = BoxDrawer.State.CLOSED_BOX_OPENED
			},
			m_camState = 
			{
				m_state = BoxCamera.State.OPENED
			},
			m_fullScreenBlackState = 
			{
				m_ignore = true
			}
		};
		this.m_stateConfigs[8] = new Box.BoxStateConfig
		{
			m_logoState = 
			{
				m_state = BoxLogo.State.HIDDEN
			},
			m_startButtonState = 
			{
				m_state = BoxStartButton.State.HIDDEN
			}
		};
		this.m_stateConfigs[9] = new Box.BoxStateConfig
		{
			m_logoState = 
			{
				m_state = BoxLogo.State.HIDDEN
			},
			m_startButtonState = 
			{
				m_state = BoxStartButton.State.HIDDEN
			}
		};
		this.m_stateConfigs[10] = new Box.BoxStateConfig
		{
			m_logoState = 
			{
				m_state = BoxLogo.State.HIDDEN
			},
			m_startButtonState = 
			{
				m_state = BoxStartButton.State.HIDDEN
			},
			m_fullScreenBlackState = 
			{
				m_ignore = true
			}
		};
		this.m_stateConfigs[11] = new Box.BoxStateConfig
		{
			m_logoState = 
			{
				m_state = BoxLogo.State.HIDDEN
			},
			m_startButtonState = 
			{
				m_state = BoxStartButton.State.HIDDEN
			},
			m_diskState = 
			{
				m_state = BoxDisk.State.MAINMENU
			},
			m_fullScreenBlackState = 
			{
				m_ignore = true
			}
		};
		this.m_stateConfigs[12] = new Box.BoxStateConfig
		{
			m_logoState = 
			{
				m_state = BoxLogo.State.HIDDEN
			},
			m_startButtonState = 
			{
				m_state = BoxStartButton.State.HIDDEN
			},
			m_doorState = 
			{
				m_state = BoxDoor.State.OPENED
			},
			m_camState = 
			{
				m_state = BoxCamera.State.OPENED
			}
		};
	}

	// Token: 0x06001B2E RID: 6958 RVA: 0x0007F844 File Offset: 0x0007DA44
	private void InitializeState()
	{
		this.m_state = Box.State.STARTUP;
		bool flag = GameMgr.Get().WasTutorial() && !GameMgr.Get().WasSpectator();
		SceneMgr sceneMgr = SceneMgr.Get();
		if (sceneMgr != null)
		{
			if (flag)
			{
				this.m_state = Box.State.LOADING;
			}
			else
			{
				sceneMgr.RegisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
				sceneMgr.RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
				this.m_state = this.TranslateSceneModeToBoxState(sceneMgr.GetMode());
			}
		}
		this.UpdateState();
		this.m_TopSpinner.Spin();
		this.m_BottomSpinner.Spin();
		if (flag)
		{
			LoadingScreen.Get().RegisterPreviousSceneDestroyedListener(new LoadingScreen.PreviousSceneDestroyedCallback(this.OnTutorialSceneDestroyed));
		}
		if (this.m_state == Box.State.HUB_WITH_DRAWER)
		{
			this.ToggleRibbonUI(true);
		}
	}

	// Token: 0x06001B2F RID: 6959 RVA: 0x0007F920 File Offset: 0x0007DB20
	private void OnTutorialSceneDestroyed(object userData)
	{
		LoadingScreen.Get().UnregisterPreviousSceneDestroyedListener(new LoadingScreen.PreviousSceneDestroyedCallback(this.OnTutorialSceneDestroyed));
		Spell eventSpell = this.GetEventSpell(BoxEventType.TUTORIAL_PLAY);
		eventSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnTutorialPlaySpellStateFinished));
		eventSpell.ActivateState(SpellStateType.DEATH);
	}

	// Token: 0x06001B30 RID: 6960 RVA: 0x0007F968 File Offset: 0x0007DB68
	private void OnTutorialPlaySpellStateFinished(Spell spell, SpellStateType prevStateType, object userData)
	{
		if (spell.GetActiveState() != SpellStateType.NONE)
		{
			return;
		}
		SceneMgr sceneMgr = SceneMgr.Get();
		sceneMgr.RegisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
		sceneMgr.RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
		this.ChangeStateToReflectSceneMode(SceneMgr.Get().GetMode(), false);
	}

	// Token: 0x06001B31 RID: 6961 RVA: 0x0007F9BC File Offset: 0x0007DBBC
	private void ShutdownState()
	{
		if (this.m_StoreButton != null)
		{
			this.m_StoreButton.Unload();
		}
		this.UnloadQuestLog();
		SceneMgr sceneMgr = SceneMgr.Get();
		if (sceneMgr != null)
		{
			sceneMgr.UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
			sceneMgr.UnregisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
		}
	}

	// Token: 0x06001B32 RID: 6962 RVA: 0x0007FA23 File Offset: 0x0007DC23
	private void QueueStateChange(Box.State state)
	{
		this.m_stateQueue.Enqueue(state);
	}

	// Token: 0x06001B33 RID: 6963 RVA: 0x0007FA34 File Offset: 0x0007DC34
	private void ChangeStateQueued()
	{
		Box.State state = this.m_stateQueue.Dequeue();
		this.ChangeStateNow(state);
	}

	// Token: 0x06001B34 RID: 6964 RVA: 0x0007FA54 File Offset: 0x0007DC54
	private void ChangeStateNow(Box.State state)
	{
		bool flag = GameUtils.ShouldShowSetRotationIntro();
		if (!flag)
		{
			this.m_DiskCenter.SetActive(true);
			if (this.m_setRotationDisk != null)
			{
				this.m_setRotationDisk.SetActive(false);
			}
		}
		if (state == Box.State.OPEN && flag)
		{
			state = Box.State.SET_ROTATION_OPEN;
		}
		this.m_state = state;
		if (state == Box.State.STARTUP)
		{
			this.ChangeState_Startup();
		}
		else if (state == Box.State.PRESS_START)
		{
			this.ChangeState_PressStart();
		}
		else if (state == Box.State.LOADING_HUB)
		{
			this.ChangeState_LoadingHub();
		}
		else if (state == Box.State.LOADING)
		{
			this.ChangeState_Loading();
		}
		else if (state == Box.State.HUB)
		{
			this.ChangeState_Hub();
		}
		else if (state == Box.State.HUB_WITH_DRAWER)
		{
			this.ChangeState_HubWithDrawer();
		}
		else if (state == Box.State.OPEN)
		{
			this.ChangeState_Open();
		}
		else if (state == Box.State.CLOSED)
		{
			this.ChangeState_Closed();
		}
		else if (state == Box.State.ERROR)
		{
			this.ChangeState_Error();
		}
		else if (state == Box.State.SET_ROTATION_LOADING)
		{
			this.ChangeState_SetRotationLoading();
		}
		else if (state == Box.State.SET_ROTATION)
		{
			this.ChangeState_SetRotation();
		}
		else if (state == Box.State.SET_ROTATION_OPEN)
		{
			this.ChangeState_SetRotationOpen();
		}
		else
		{
			Debug.LogError(string.Format("Box.ChangeStateNow() - unhandled state {0}", state));
		}
		this.UpdateUIEvents();
	}

	// Token: 0x06001B35 RID: 6965 RVA: 0x0007FBAC File Offset: 0x0007DDAC
	private void ChangeStateToReflectSceneMode(SceneMgr.Mode mode, bool isSceneActuallyLoaded)
	{
		Box.State state = this.TranslateSceneModeToBoxState(mode);
		bool flag = GameUtils.ShouldShowSetRotationIntro();
		if (mode == SceneMgr.Mode.HUB && flag)
		{
			this.ChangeState(Box.State.SET_ROTATION_LOADING);
			if (isSceneActuallyLoaded)
			{
				this.SetRotation_ShowUnAckedRewardsAndQuests();
			}
		}
		else if (mode == SceneMgr.Mode.TOURNAMENT && flag)
		{
			this.ChangeState(Box.State.SET_ROTATION_OPEN);
			this.m_transitioningToSceneMode = true;
		}
		else if (this.ChangeState(state))
		{
			this.m_transitioningToSceneMode = true;
		}
		BoxLightStateType stateType = this.TranslateSceneModeToLightState(mode);
		this.m_LightMgr.ChangeState(stateType);
	}

	// Token: 0x06001B36 RID: 6966 RVA: 0x0007FC38 File Offset: 0x0007DE38
	private Box.State TranslateSceneModeToBoxState(SceneMgr.Mode mode)
	{
		if (mode == SceneMgr.Mode.STARTUP)
		{
			return Box.State.STARTUP;
		}
		if (mode == SceneMgr.Mode.LOGIN)
		{
			return Box.State.INVALID;
		}
		if (mode == SceneMgr.Mode.HUB)
		{
			return Box.State.HUB_WITH_DRAWER;
		}
		if (mode == SceneMgr.Mode.TOURNAMENT)
		{
			return Box.State.OPEN;
		}
		if (mode == SceneMgr.Mode.ADVENTURE)
		{
			return Box.State.OPEN;
		}
		if (mode == SceneMgr.Mode.FRIENDLY)
		{
			return Box.State.OPEN;
		}
		if (mode == SceneMgr.Mode.DRAFT)
		{
			return Box.State.OPEN;
		}
		if (mode == SceneMgr.Mode.COLLECTIONMANAGER)
		{
			return Box.State.OPEN;
		}
		if (mode == SceneMgr.Mode.TAVERN_BRAWL)
		{
			return Box.State.OPEN;
		}
		if (mode == SceneMgr.Mode.PACKOPENING)
		{
			return Box.State.OPEN;
		}
		if (mode == SceneMgr.Mode.GAMEPLAY)
		{
			return Box.State.INVALID;
		}
		if (mode == SceneMgr.Mode.FATAL_ERROR)
		{
			return Box.State.ERROR;
		}
		return Box.State.OPEN;
	}

	// Token: 0x06001B37 RID: 6967 RVA: 0x0007FCB8 File Offset: 0x0007DEB8
	private BoxLightStateType TranslateSceneModeToLightState(SceneMgr.Mode mode)
	{
		if (mode == SceneMgr.Mode.LOGIN)
		{
			return BoxLightStateType.INVALID;
		}
		if (mode == SceneMgr.Mode.TOURNAMENT)
		{
			return BoxLightStateType.TOURNAMENT;
		}
		if (mode == SceneMgr.Mode.COLLECTIONMANAGER)
		{
			return BoxLightStateType.COLLECTION;
		}
		if (mode == SceneMgr.Mode.TAVERN_BRAWL)
		{
			return BoxLightStateType.COLLECTION;
		}
		if (mode == SceneMgr.Mode.PACKOPENING)
		{
			return BoxLightStateType.PACK_OPENING;
		}
		if (mode == SceneMgr.Mode.GAMEPLAY)
		{
			return BoxLightStateType.INVALID;
		}
		if (mode == SceneMgr.Mode.DRAFT)
		{
			return BoxLightStateType.FORGE;
		}
		if (mode == SceneMgr.Mode.ADVENTURE)
		{
			return BoxLightStateType.ADVENTURE;
		}
		if (mode == SceneMgr.Mode.FRIENDLY)
		{
			return BoxLightStateType.ADVENTURE;
		}
		return BoxLightStateType.DEFAULT;
	}

	// Token: 0x06001B38 RID: 6968 RVA: 0x0007FD1C File Offset: 0x0007DF1C
	private void OnScenePreUnload(SceneMgr.Mode prevMode, Scene prevScene, object userData)
	{
		SceneMgr.Mode mode = SceneMgr.Get().GetMode();
		if (mode == SceneMgr.Mode.GAMEPLAY || mode == SceneMgr.Mode.STARTUP || mode == SceneMgr.Mode.RESET)
		{
			return;
		}
		if (prevMode == SceneMgr.Mode.HUB)
		{
			this.ChangeState(Box.State.LOADING);
			this.m_StoreButton.Unload();
			this.UnloadQuestLog();
		}
		else if (mode == SceneMgr.Mode.HUB)
		{
			this.ChangeStateToReflectSceneMode(mode, false);
			this.m_waitingForSceneLoad = true;
		}
		else if ((prevMode == SceneMgr.Mode.COLLECTIONMANAGER && (mode == SceneMgr.Mode.ADVENTURE || mode == SceneMgr.Mode.TOURNAMENT)) || (mode == SceneMgr.Mode.COLLECTIONMANAGER && (prevMode == SceneMgr.Mode.ADVENTURE || prevMode == SceneMgr.Mode.TOURNAMENT)))
		{
			this.ChangeState(Box.State.LOADING_HUB);
		}
		else
		{
			this.ChangeState(Box.State.LOADING);
		}
		this.ClearQueuedButtonFireEvent();
		this.UpdateUIEvents();
	}

	// Token: 0x06001B39 RID: 6969 RVA: 0x0007FDD8 File Offset: 0x0007DFD8
	private void OnSceneLoaded(SceneMgr.Mode mode, Scene scene, object userData)
	{
		if (!TavernBrawlManager.Get().IsTavernBrawlActive && SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.STARTUP)
		{
			this.PlayTavernBrawlButtonActivation(false, true);
		}
		this.ChangeStateToReflectSceneMode(mode, true);
		if (this.m_waitingForSceneLoad)
		{
			this.m_waitingForSceneLoad = false;
			Box.ButtonType? queuedButtonFire = this.m_queuedButtonFire;
			if (queuedButtonFire != null)
			{
				Box.ButtonType? queuedButtonFire2 = this.m_queuedButtonFire;
				this.FireButtonPressEvent(queuedButtonFire2.Value);
				this.m_queuedButtonFire = default(Box.ButtonType?);
			}
		}
	}

	// Token: 0x06001B3A RID: 6970 RVA: 0x0007FE5C File Offset: 0x0007E05C
	private void ChangeState_Startup()
	{
		this.m_state = Box.State.STARTUP;
		this.ChangeStateUsingConfig();
	}

	// Token: 0x06001B3B RID: 6971 RVA: 0x0007FE6B File Offset: 0x0007E06B
	private void ChangeState_PressStart()
	{
		this.m_state = Box.State.PRESS_START;
		this.ChangeStateUsingConfig();
	}

	// Token: 0x06001B3C RID: 6972 RVA: 0x0007FE7A File Offset: 0x0007E07A
	private void ChangeState_SetRotationLoading()
	{
		this.m_state = Box.State.SET_ROTATION_LOADING;
		this.ChangeStateUsingConfig();
	}

	// Token: 0x06001B3D RID: 6973 RVA: 0x0007FE8A File Offset: 0x0007E08A
	private void ChangeState_SetRotation()
	{
		this.m_state = Box.State.SET_ROTATION;
		this.ChangeStateUsingConfig();
	}

	// Token: 0x06001B3E RID: 6974 RVA: 0x0007FE9A File Offset: 0x0007E09A
	private void ChangeState_SetRotationOpen()
	{
		this.m_state = Box.State.SET_ROTATION_OPEN;
		base.StartCoroutine(this.SetRotationOpen_ChangeState());
	}

	// Token: 0x06001B3F RID: 6975 RVA: 0x0007FEB1 File Offset: 0x0007E0B1
	private void ChangeState_LoadingHub()
	{
		this.m_state = Box.State.LOADING_HUB;
		this.ChangeStateUsingConfig();
	}

	// Token: 0x06001B40 RID: 6976 RVA: 0x0007FEC0 File Offset: 0x0007E0C0
	private void ChangeState_Loading()
	{
		this.m_state = Box.State.LOADING;
		this.ChangeStateUsingConfig();
	}

	// Token: 0x06001B41 RID: 6977 RVA: 0x0007FECF File Offset: 0x0007E0CF
	private void ChangeState_Hub()
	{
		this.m_state = Box.State.HUB;
		this.HackRequestNetFeatures();
		this.ChangeStateUsingConfig();
	}

	// Token: 0x06001B42 RID: 6978 RVA: 0x0007FEE4 File Offset: 0x0007E0E4
	private void ChangeState_HubWithDrawer()
	{
		this.m_state = Box.State.HUB_WITH_DRAWER;
		this.m_Camera.EnableAccelerometer();
		this.HackRequestNetData();
		this.ChangeStateUsingConfig();
	}

	// Token: 0x06001B43 RID: 6979 RVA: 0x0007FF04 File Offset: 0x0007E104
	private void ChangeState_Open()
	{
		this.m_state = Box.State.OPEN;
		this.ChangeStateUsingConfig();
	}

	// Token: 0x06001B44 RID: 6980 RVA: 0x0007FF13 File Offset: 0x0007E113
	private void ChangeState_Closed()
	{
		this.m_state = Box.State.CLOSED;
		this.ChangeStateUsingConfig();
	}

	// Token: 0x06001B45 RID: 6981 RVA: 0x0007FF22 File Offset: 0x0007E122
	private void ChangeState_Error()
	{
		this.m_state = Box.State.ERROR;
		this.ChangeStateUsingConfig();
	}

	// Token: 0x06001B46 RID: 6982 RVA: 0x0007FF32 File Offset: 0x0007E132
	private void UpdateState_Startup()
	{
		this.m_state = Box.State.STARTUP;
		this.UpdateStateUsingConfig();
	}

	// Token: 0x06001B47 RID: 6983 RVA: 0x0007FF41 File Offset: 0x0007E141
	private void UpdateState_PressStart()
	{
		this.m_state = Box.State.PRESS_START;
		this.UpdateStateUsingConfig();
	}

	// Token: 0x06001B48 RID: 6984 RVA: 0x0007FF50 File Offset: 0x0007E150
	private void UpdateState_SetRotationLoading()
	{
		this.m_state = Box.State.SET_ROTATION_LOADING;
		this.UpdateStateUsingConfig();
	}

	// Token: 0x06001B49 RID: 6985 RVA: 0x0007FF60 File Offset: 0x0007E160
	private void UpdateState_SetRotation()
	{
		this.m_state = Box.State.SET_ROTATION;
		this.UpdateStateUsingConfig();
	}

	// Token: 0x06001B4A RID: 6986 RVA: 0x0007FF70 File Offset: 0x0007E170
	private void UpdateState_SetRotationOpen()
	{
		this.m_state = Box.State.SET_ROTATION_OPEN;
		this.UpdateStateUsingConfig();
	}

	// Token: 0x06001B4B RID: 6987 RVA: 0x0007FF80 File Offset: 0x0007E180
	private void UpdateState_LoadingHub()
	{
		this.m_state = Box.State.LOADING_HUB;
		this.UpdateStateUsingConfig();
	}

	// Token: 0x06001B4C RID: 6988 RVA: 0x0007FF8F File Offset: 0x0007E18F
	private void UpdateState_Loading()
	{
		this.m_state = Box.State.LOADING;
		this.UpdateStateUsingConfig();
	}

	// Token: 0x06001B4D RID: 6989 RVA: 0x0007FF9E File Offset: 0x0007E19E
	private void UpdateState_Hub()
	{
		this.m_state = Box.State.HUB;
		this.HackRequestNetFeatures();
		this.UpdateStateUsingConfig();
	}

	// Token: 0x06001B4E RID: 6990 RVA: 0x0007FFB3 File Offset: 0x0007E1B3
	private void UpdateState_HubWithDrawer()
	{
		this.m_state = Box.State.HUB_WITH_DRAWER;
		this.m_Camera.EnableAccelerometer();
		this.HackRequestNetData();
		this.UpdateStateUsingConfig();
	}

	// Token: 0x06001B4F RID: 6991 RVA: 0x0007FFD3 File Offset: 0x0007E1D3
	private void UpdateState_Open()
	{
		this.m_state = Box.State.OPEN;
		this.UpdateStateUsingConfig();
	}

	// Token: 0x06001B50 RID: 6992 RVA: 0x0007FFE2 File Offset: 0x0007E1E2
	private void UpdateState_Closed()
	{
		this.m_state = Box.State.CLOSED;
		this.UpdateStateUsingConfig();
	}

	// Token: 0x06001B51 RID: 6993 RVA: 0x0007FFF1 File Offset: 0x0007E1F1
	private void UpdateState_Error()
	{
		this.m_state = Box.State.ERROR;
		this.UpdateStateUsingConfig();
	}

	// Token: 0x06001B52 RID: 6994 RVA: 0x00080004 File Offset: 0x0007E204
	private void ChangeStateUsingConfig()
	{
		Box.BoxStateConfig boxStateConfig = this.m_stateConfigs[(int)this.m_state];
		if (!boxStateConfig.m_logoState.m_ignore)
		{
			this.m_Logo.ChangeState(boxStateConfig.m_logoState.m_state);
		}
		if (!boxStateConfig.m_startButtonState.m_ignore)
		{
			this.m_StartButton.ChangeState(boxStateConfig.m_startButtonState.m_state);
		}
		if (!boxStateConfig.m_doorState.m_ignore)
		{
			this.m_LeftDoor.ChangeState(boxStateConfig.m_doorState.m_state);
			this.m_RightDoor.ChangeState(boxStateConfig.m_doorState.m_state);
		}
		if (!boxStateConfig.m_diskState.m_ignore)
		{
			this.m_Disk.ChangeState(boxStateConfig.m_diskState.m_state);
		}
		if (!boxStateConfig.m_drawerState.m_ignore)
		{
			if (!UniversalInputManager.UsePhoneUI)
			{
				this.m_Drawer.ChangeState(boxStateConfig.m_drawerState.m_state);
			}
			else
			{
				bool flag = this.m_state == Box.State.HUB_WITH_DRAWER;
				if (!flag && flag != this.m_showRibbonButtons)
				{
					this.ToggleRibbonUI(flag);
				}
			}
		}
		if (!boxStateConfig.m_camState.m_ignore)
		{
			this.m_Camera.ChangeState(boxStateConfig.m_camState.m_state);
		}
		if (!boxStateConfig.m_fullScreenBlackState.m_ignore)
		{
			this.FullScreenBlack_ChangeState(boxStateConfig.m_fullScreenBlackState.m_state);
		}
	}

	// Token: 0x06001B53 RID: 6995 RVA: 0x00080178 File Offset: 0x0007E378
	private void ToggleRibbonUI(bool show)
	{
		if (this.m_ribbonButtons == null)
		{
			return;
		}
		this.m_ribbonButtons.Toggle(show);
		this.m_showRibbonButtons = show;
	}

	// Token: 0x06001B54 RID: 6996 RVA: 0x000801A0 File Offset: 0x0007E3A0
	private void UpdateStateUsingConfig()
	{
		Box.BoxStateConfig boxStateConfig = this.m_stateConfigs[(int)this.m_state];
		if (!boxStateConfig.m_logoState.m_ignore)
		{
			this.m_Logo.UpdateState(boxStateConfig.m_logoState.m_state);
		}
		if (!boxStateConfig.m_startButtonState.m_ignore)
		{
			this.m_StartButton.UpdateState(boxStateConfig.m_startButtonState.m_state);
		}
		if (!boxStateConfig.m_doorState.m_ignore)
		{
			this.m_LeftDoor.ChangeState(boxStateConfig.m_doorState.m_state);
			this.m_RightDoor.ChangeState(boxStateConfig.m_doorState.m_state);
		}
		if (!boxStateConfig.m_diskState.m_ignore)
		{
			this.m_Disk.UpdateState(boxStateConfig.m_diskState.m_state);
		}
		this.m_TopSpinner.Reset();
		this.m_BottomSpinner.Reset();
		if (!boxStateConfig.m_drawerState.m_ignore)
		{
			this.m_Drawer.UpdateState(boxStateConfig.m_drawerState.m_state);
		}
		if (!boxStateConfig.m_camState.m_ignore)
		{
			this.m_Camera.UpdateState(boxStateConfig.m_camState.m_state);
		}
		if (!boxStateConfig.m_fullScreenBlackState.m_ignore)
		{
			this.FullScreenBlack_UpdateState(boxStateConfig.m_fullScreenBlackState.m_state);
		}
	}

	// Token: 0x06001B55 RID: 6997 RVA: 0x000802EE File Offset: 0x0007E4EE
	private void FullScreenBlack_ChangeState(bool enable)
	{
		this.FullScreenBlack_UpdateState(enable);
	}

	// Token: 0x06001B56 RID: 6998 RVA: 0x000802F8 File Offset: 0x0007E4F8
	private void FullScreenBlack_UpdateState(bool enable)
	{
		FullScreenEffects component = this.m_Camera.GetComponent<FullScreenEffects>();
		component.BlendToColorEnable = enable;
		if (!enable)
		{
			return;
		}
		component.BlendToColor = Color.black;
		component.BlendToColorAmount = 1f;
	}

	// Token: 0x06001B57 RID: 6999 RVA: 0x00080338 File Offset: 0x0007E538
	private void FireTransitionFinishedEvent()
	{
		foreach (Box.TransitionFinishedListener transitionFinishedListener in this.m_transitionFinishedListeners.ToArray())
		{
			transitionFinishedListener.Fire();
		}
	}

	// Token: 0x06001B58 RID: 7000 RVA: 0x00080370 File Offset: 0x0007E570
	private void InitializeUI()
	{
		PegUI.Get().SetInputCamera(this.m_Camera.GetComponent<Camera>());
		this.m_StartButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnStartButtonPressed));
		InputScheme inputScheme = InputUtil.GetInputScheme();
		if (inputScheme == InputScheme.TOUCH)
		{
			this.m_StartButton.SetText(GameStrings.Get("GLUE_START_TOUCH"));
		}
		else if (inputScheme == InputScheme.GAMEPAD)
		{
			this.m_StartButton.SetText(GameStrings.Get("GLUE_START_PRESS"));
		}
		else if (inputScheme == InputScheme.KEYBOARD_MOUSE)
		{
			this.m_StartButton.SetText(GameStrings.Get("GLUE_START_CLICK"));
		}
		this.m_TournamentButton.SetText(GameStrings.Get("GLUE_TOURNAMENT"));
		this.m_SoloAdventuresButton.SetText(GameStrings.Get("GLUE_ADVENTURE"));
		this.m_ForgeButton.SetText(GameStrings.Get("GLUE_FORGE"));
		this.m_TavernBrawlButton.SetText(GameStrings.Get("GLOBAL_TAVERN_BRAWL"));
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_ribbonButtons.m_collectionManagerRibbon.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCollectionButtonPressed));
			this.m_ribbonButtons.m_packOpeningRibbon.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnOpenPacksButtonPressed));
			this.m_ribbonButtons.m_questLogRibbon.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnQuestButtonPressed));
			this.m_ribbonButtons.m_storeRibbon.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnStoreButtonReleased));
		}
		else
		{
			this.m_OpenPacksButton.SetText(GameStrings.Get("GLUE_OPEN_PACKS"));
			this.m_CollectionButton.SetText(GameStrings.Get("GLUE_MY_COLLECTION"));
			this.m_QuestLogButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnQuestButtonPressed));
			this.m_StoreButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnStoreButtonReleased));
		}
		this.RegisterButtonEvents(this.m_TournamentButton);
		this.RegisterButtonEvents(this.m_SoloAdventuresButton);
		this.RegisterButtonEvents(this.m_ForgeButton);
		this.RegisterButtonEvents(this.m_TavernBrawlButton);
		this.RegisterButtonEvents(this.m_OpenPacksButton);
		this.RegisterButtonEvents(this.m_CollectionButton);
		this.UpdateUI(true);
	}

	// Token: 0x06001B59 RID: 7001 RVA: 0x00080599 File Offset: 0x0007E799
	public void UpdateUI(bool isInitialization = false)
	{
		this.UpdateUIState(isInitialization);
		this.UpdateUIEvents();
	}

	// Token: 0x06001B5A RID: 7002 RVA: 0x000805A8 File Offset: 0x0007E7A8
	private void UpdateUIState(bool isInitialization)
	{
		if (this.m_waitingForNetData)
		{
			this.SetPackCount(-1);
			this.HighlightButton(this.m_OpenPacksButton, false);
			this.HighlightButton(this.m_TournamentButton, false);
			this.HighlightButton(this.m_SoloAdventuresButton, false);
			this.HighlightButton(this.m_CollectionButton, false);
			this.HighlightButton(this.m_ForgeButton, false);
			this.HighlightButton(this.m_TavernBrawlButton, false);
		}
		else
		{
			NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
			if (DemoMgr.Get().GetMode() == DemoMode.BLIZZCON_2013)
			{
				netObject.Games.Practice = false;
				netObject.Games.Tournament = false;
			}
			int num = this.ComputeBoosterCount();
			this.SetPackCount(num);
			bool highlightOn = num > 0 && !Options.Get().GetBool(Option.HAS_SEEN_PACK_OPENING, false);
			this.HighlightButton(this.m_OpenPacksButton, highlightOn);
			bool flag = netObject.Games.Practice && (!Options.Get().GetBool(Option.HAS_SEEN_PRACTICE_MODE, false) || Options.Get().GetBool(Option.BUNDLE_JUST_PURCHASE_IN_HUB, false));
			this.HighlightButton(this.m_SoloAdventuresButton, flag);
			this.ToggleButtonTextureState(netObject.Games.Practice, this.m_SoloAdventuresButton);
			bool highlightOn2 = false;
			if (AchieveManager.Get() != null)
			{
				List<Achievement> activeQuests = AchieveManager.Get().GetActiveQuests(false);
				foreach (Achievement achievement in activeQuests)
				{
					if (achievement.ID == 11)
					{
						highlightOn2 = (!flag && netObject.Games.Tournament);
					}
				}
			}
			this.HighlightButton(this.m_TournamentButton, highlightOn2);
			this.ToggleButtonTextureState(netObject.Games.Tournament, this.m_TournamentButton);
			bool highlightOn3 = !flag && netObject.Collection.Manager && !Options.Get().GetBool(Option.HAS_SEEN_COLLECTIONMANAGER_AFTER_PRACTICE, false);
			this.HighlightButton(this.m_CollectionButton, highlightOn3);
			this.ToggleDrawerButtonState(netObject.Collection.Manager, this.m_CollectionButton);
			bool flag2 = netObject.Games.Forge && AchieveManager.Get() != null && AchieveManager.Get().HasUnlockedFeature(Achievement.UnlockableFeature.FORGE);
			if (flag2)
			{
				flag2 = HealthyGamingMgr.Get().isArenaEnabled();
			}
			bool highlightOn4 = !flag && flag2 && !Options.Get().GetBool(Option.HAS_SEEN_FORGE, false);
			this.HighlightButton(this.m_ForgeButton, highlightOn4);
			this.ToggleButtonTextureState(flag2, this.m_ForgeButton);
			this.UpdateTavernBrawlButtonState(true);
			SpecialEventType activeEventType = SpecialEventVisualMgr.GetActiveEventType();
			if (activeEventType == SpecialEventType.SPECIAL_EVENT_PRE_TAVERN_BRAWL)
			{
				this.m_TavernBrawlButton.gameObject.SetActive(false);
				this.m_EmptyFourthButton.SetActive(true);
			}
		}
	}

	// Token: 0x06001B5B RID: 7003 RVA: 0x0008088C File Offset: 0x0007EA8C
	private void DoTavernBrawlButtonInitialization()
	{
		TavernBrawlManager tavernBrawlManager = TavernBrawlManager.Get();
		NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
		if (netObject == null)
		{
			NetCache.Get().RegisterUpdatedListener(typeof(NetCache.NetCacheFeatures), new Action(this.DoTavernBrawlButtonInitialization));
			return;
		}
		NetCache.Get().RemoveUpdatedListener(typeof(NetCache.NetCacheFeatures), new Action(this.DoTavernBrawlButtonInitialization));
		if (netObject == null || !netObject.Games.TavernBrawl || !tavernBrawlManager.HasUnlockedTavernBrawl)
		{
			return;
		}
		if (!tavernBrawlManager.IsTavernBrawlInfoReady)
		{
			return;
		}
		tavernBrawlManager.OnTavernBrawlUpdated -= new Action(this.DoTavernBrawlButtonInitialization);
		if (!tavernBrawlManager.IsTavernBrawlActive || tavernBrawlManager.IsFirstTimeSeeingCurrentSeason)
		{
			this.PlayTavernBrawlButtonActivation(false, true);
		}
	}

	// Token: 0x17000321 RID: 801
	// (get) Token: 0x06001B5C RID: 7004 RVA: 0x00080954 File Offset: 0x0007EB54
	// (set) Token: 0x06001B5D RID: 7005 RVA: 0x0008095C File Offset: 0x0007EB5C
	public bool IsTavernBrawlButtonDeactivated { get; private set; }

	// Token: 0x06001B5E RID: 7006 RVA: 0x00080968 File Offset: 0x0007EB68
	public void PlayTavernBrawlButtonActivation(bool activate, bool isInitialization = false)
	{
		Animator component = this.m_TavernBrawlButtonVisual.GetComponent<Animator>();
		component.StopPlayback();
		if (activate)
		{
			component.Play("TavernBrawl_ButtonActivate");
			if (!isInitialization)
			{
				this.m_TavernBrawlButtonActivateFX.GetComponent<ParticleSystem>().Play();
			}
		}
		else
		{
			if (!isInitialization)
			{
				this.m_TavernBrawlButton.ClearHighlightAndTooltip();
			}
			component.Play("TavernBrawl_ButtonDeactivate");
			if (!isInitialization)
			{
				this.m_TavernBrawlButtonDeactivateFX.GetComponent<ParticleSystem>().Play();
			}
		}
		this.IsTavernBrawlButtonDeactivated = !activate;
	}

	// Token: 0x06001B5F RID: 7007 RVA: 0x000809F0 File Offset: 0x0007EBF0
	public bool UpdateTavernBrawlButtonState(bool highlightAllowed)
	{
		NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
		TavernBrawlManager tavernBrawlManager = TavernBrawlManager.Get();
		bool flag = netObject != null && netObject.Games.TavernBrawl && tavernBrawlManager.HasUnlockedTavernBrawl;
		bool highlightOn = highlightAllowed && flag && tavernBrawlManager.IsFirstTimeSeeingThisFeature && !tavernBrawlManager.HasSeenTavernBrawlScreen && !this.IsTavernBrawlButtonDeactivated;
		this.HighlightButton(this.m_TavernBrawlButton, highlightOn);
		this.ToggleButtonTextureState(flag, this.m_TavernBrawlButton);
		return flag;
	}

	// Token: 0x06001B60 RID: 7008 RVA: 0x00080A78 File Offset: 0x0007EC78
	private void UpdateUIEvents()
	{
		bool flag = GameUtils.ShouldShowSetRotationIntro();
		if (this.CanEnableUIEvents() && this.m_state == Box.State.PRESS_START)
		{
			this.EnableButton(this.m_StartButton);
		}
		else
		{
			this.DisableButton(this.m_StartButton);
		}
		if (!flag && this.m_state != Box.State.PRESS_START && this.m_state != Box.State.LOADING_HUB && this.CanEnableUIEvents())
		{
			this.m_StoreButton.gameObject.SetActive(true);
			this.m_QuestLogButton.gameObject.SetActive(true);
		}
		if (this.CanEnableUIEvents() && (this.m_state == Box.State.HUB || this.m_state == Box.State.HUB_WITH_DRAWER))
		{
			if (this.m_waitingForNetData)
			{
				this.DisableButton(this.m_TournamentButton);
				this.DisableButton(this.m_SoloAdventuresButton);
				this.DisableButton(this.m_ForgeButton);
				this.DisableButton(this.m_TavernBrawlButton);
				this.DisableButton(this.m_QuestLogButton);
				this.DisableButton(this.m_StoreButton);
			}
			else
			{
				this.EnableButton(this.m_TournamentButton);
				this.EnableButton(this.m_SoloAdventuresButton);
				this.EnableButton(this.m_ForgeButton);
				this.EnableButton(this.m_TavernBrawlButton);
				this.EnableButton(this.m_QuestLogButton);
				this.EnableButton(this.m_StoreButton);
			}
			if (this.m_state == Box.State.HUB_WITH_DRAWER)
			{
				if (this.m_waitingForNetData)
				{
					this.DisableButton(this.m_OpenPacksButton);
					this.DisableButton(this.m_CollectionButton);
				}
				else
				{
					this.EnableButton(this.m_OpenPacksButton);
					this.EnableButton(this.m_CollectionButton);
				}
			}
			else
			{
				this.DisableButton(this.m_OpenPacksButton);
				this.DisableButton(this.m_CollectionButton);
			}
		}
		else
		{
			this.DisableButton(this.m_TournamentButton);
			this.DisableButton(this.m_SoloAdventuresButton);
			this.DisableButton(this.m_ForgeButton);
			this.DisableButton(this.m_TavernBrawlButton);
			this.DisableButton(this.m_OpenPacksButton);
			this.DisableButton(this.m_CollectionButton);
			this.DisableButton(this.m_QuestLogButton);
			this.DisableButton(this.m_StoreButton);
		}
		if (DemoMgr.Get().GetMode() == DemoMode.BLIZZCON_2013)
		{
			this.DisableButton(this.m_TournamentButton);
			this.DisableButton(this.m_SoloAdventuresButton);
			this.DisableButton(this.m_OpenPacksButton);
			this.DisableButton(this.m_CollectionButton);
			this.DisableButton(this.m_QuestLogButton);
			this.DisableButton(this.m_StoreButton);
		}
		else if (DemoMgr.Get().GetMode() == DemoMode.BLIZZCON_2014)
		{
			this.DisableButton(this.m_SoloAdventuresButton);
			this.DisableButton(this.m_ForgeButton);
			this.DisableButton(this.m_TavernBrawlButton);
			this.DisableButton(this.m_OpenPacksButton);
			this.DisableButton(this.m_CollectionButton);
			this.DisableButton(this.m_QuestLogButton);
			this.DisableButton(this.m_StoreButton);
		}
		else if (DemoMgr.Get().GetMode() == DemoMode.BLIZZCON_2015)
		{
			this.DisableButton(this.m_TournamentButton);
			this.DisableButton(this.m_ForgeButton);
			this.DisableButton(this.m_TavernBrawlButton);
			this.DisableButton(this.m_OpenPacksButton);
			this.DisableButton(this.m_CollectionButton);
			this.DisableButton(this.m_QuestLogButton);
			this.DisableButton(this.m_StoreButton);
		}
		else if (DemoMgr.Get().GetMode() == DemoMode.ANNOUNCEMENT_5_0)
		{
			this.DisableButton(this.m_TournamentButton);
			this.DisableButton(this.m_SoloAdventuresButton);
			this.DisableButton(this.m_ForgeButton);
			this.DisableButton(this.m_TavernBrawlButton);
			this.DisableButton(this.m_OpenPacksButton);
			this.DisableButton(this.m_CollectionButton);
			this.DisableButton(this.m_QuestLogButton);
			this.DisableButton(this.m_StoreButton);
		}
		if (flag || this.m_state == Box.State.LOADING_HUB)
		{
			this.m_StoreButton.gameObject.SetActive(false);
			this.m_QuestLogButton.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001B61 RID: 7009 RVA: 0x00080E70 File Offset: 0x0007F070
	private bool CanEnableUIEvents()
	{
		return !this.HasPendingEffects() && this.m_stateQueue.Count <= 0 && this.m_state != Box.State.INVALID && this.m_state != Box.State.STARTUP && this.m_state != Box.State.LOADING && this.m_state != Box.State.LOADING_HUB && this.m_state != Box.State.OPEN;
	}

	// Token: 0x06001B62 RID: 7010 RVA: 0x00080EE4 File Offset: 0x0007F0E4
	private void RegisterButtonEvents(PegUIElement button)
	{
		button.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnButtonPressed));
		button.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnButtonMouseOver));
		button.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnButtonMouseOut));
	}

	// Token: 0x06001B63 RID: 7011 RVA: 0x00080F2D File Offset: 0x0007F12D
	private void ToggleButtonTextureState(bool enabled, BoxMenuButton button)
	{
		if (enabled)
		{
			button.m_TextMesh.TextColor = this.m_EnabledMaterial;
		}
		else
		{
			button.m_TextMesh.TextColor = this.m_DisabledMaterial;
		}
	}

	// Token: 0x06001B64 RID: 7012 RVA: 0x00080F5C File Offset: 0x0007F15C
	private void ToggleDrawerButtonState(bool enabled, BoxMenuButton button)
	{
		if (enabled)
		{
			button.m_TextMesh.TextColor = this.m_EnabledDrawerMaterial;
		}
		else
		{
			button.m_TextMesh.TextColor = this.m_DisabledDrawerMaterial;
		}
	}

	// Token: 0x06001B65 RID: 7013 RVA: 0x00080F8C File Offset: 0x0007F18C
	private void HighlightButton(BoxMenuButton button, bool highlightOn)
	{
		if (button.m_HighlightState == null)
		{
			Debug.LogWarning(string.Format("Box.HighlighButton {0} - highlight state is null", button));
			return;
		}
		ActorStateType stateType = (!highlightOn) ? ActorStateType.HIGHLIGHT_OFF : ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE;
		button.m_HighlightState.ChangeState(stateType);
	}

	// Token: 0x06001B66 RID: 7014 RVA: 0x00080FD8 File Offset: 0x0007F1D8
	private void SetPackCount(int n)
	{
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_ribbonButtons.SetPackCount(n);
		}
		else
		{
			this.m_OpenPacksButton.SetPackCount(n);
		}
	}

	// Token: 0x06001B67 RID: 7015 RVA: 0x00081011 File Offset: 0x0007F211
	public void EnableButton(PegUIElement button)
	{
		button.SetEnabled(true);
	}

	// Token: 0x06001B68 RID: 7016 RVA: 0x0008101C File Offset: 0x0007F21C
	public void DisableButton(PegUIElement button)
	{
		button.SetEnabled(false);
		TooltipZone component = button.GetComponent<TooltipZone>();
		if (component != null)
		{
			component.HideTooltip();
		}
	}

	// Token: 0x06001B69 RID: 7017 RVA: 0x0008104C File Offset: 0x0007F24C
	private void OnButtonPressed(UIEvent e)
	{
		PegUIElement element = e.GetElement();
		NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
		bool tournament = netObject.Games.Tournament;
		bool practice = netObject.Games.Practice;
		bool flag = netObject.Games.Forge && AchieveManager.Get() != null && AchieveManager.Get().HasUnlockedFeature(Achievement.UnlockableFeature.FORGE);
		if (flag)
		{
			flag = HealthyGamingMgr.Get().isArenaEnabled();
		}
		bool manager = netObject.Collection.Manager;
		bool flag2 = netObject.Games.TavernBrawl && TavernBrawlManager.Get().HasUnlockedTavernBrawl;
		BoxMenuButton boxMenuButton = (BoxMenuButton)element;
		if (boxMenuButton == this.m_StartButton)
		{
			this.OnStartButtonPressed(e);
		}
		else if (boxMenuButton == this.m_TournamentButton && tournament)
		{
			this.OnTournamentButtonPressed(e);
		}
		else if (boxMenuButton == this.m_SoloAdventuresButton && practice)
		{
			this.OnSoloAdventuresButtonPressed(e);
		}
		else if (boxMenuButton == this.m_ForgeButton && flag)
		{
			this.OnForgeButtonPressed(e);
		}
		else if (boxMenuButton == this.m_TavernBrawlButton && flag2)
		{
			this.OnTavernBrawlButtonPressed(e);
		}
		else if (boxMenuButton == this.m_OpenPacksButton)
		{
			this.OnOpenPacksButtonPressed(e);
		}
		else if (boxMenuButton == this.m_CollectionButton && manager)
		{
			this.OnCollectionButtonPressed(e);
		}
		else if (boxMenuButton == this.m_setRotationButton)
		{
			this.OnSetRotationButtonPressed(e);
		}
	}

	// Token: 0x06001B6A RID: 7018 RVA: 0x00081200 File Offset: 0x0007F400
	private void FireButtonPressEvent(Box.ButtonType buttonType)
	{
		if (this.m_waitingForSceneLoad)
		{
			this.m_queuedButtonFire = new Box.ButtonType?(buttonType);
			return;
		}
		foreach (Box.ButtonPressListener buttonPressListener in this.m_buttonPressListeners.ToArray())
		{
			buttonPressListener.Fire(buttonType);
		}
	}

	// Token: 0x06001B6B RID: 7019 RVA: 0x00081250 File Offset: 0x0007F450
	private void ClearQueuedButtonFireEvent()
	{
		this.m_queuedButtonFire = default(Box.ButtonType?);
	}

	// Token: 0x06001B6C RID: 7020 RVA: 0x0008126C File Offset: 0x0007F46C
	private void OnStartButtonPressed(UIEvent e)
	{
		if (SceneMgr.Get() == null)
		{
			this.ChangeState(Box.State.HUB);
		}
		else
		{
			this.FireButtonPressEvent(Box.ButtonType.START);
		}
	}

	// Token: 0x06001B6D RID: 7021 RVA: 0x000812A0 File Offset: 0x0007F4A0
	private void OnTournamentButtonPressed(UIEvent e)
	{
		if (SceneMgr.Get() == null)
		{
			this.ChangeState(Box.State.OPEN);
		}
		else
		{
			if (!Options.Get().HasOption(Option.HAS_CLICKED_TOURNAMENT))
			{
				Options.Get().SetBool(Option.HAS_CLICKED_TOURNAMENT, true);
			}
			AchieveManager.Get().NotifyOfClick(Achievement.ClickTriggerType.BUTTON_PLAY);
			this.FireButtonPressEvent(Box.ButtonType.TOURNAMENT);
		}
	}

	// Token: 0x06001B6E RID: 7022 RVA: 0x000812FC File Offset: 0x0007F4FC
	private void OnSetRotationButtonPressed(UIEvent e)
	{
		Log.Kyle.Print("Set Rotation Button Pressed!", new object[0]);
		if (SceneMgr.Get() == null)
		{
			this.ChangeState(Box.State.SET_ROTATION_OPEN);
		}
		else
		{
			if (!Options.Get().HasOption(Option.HAS_CLICKED_TOURNAMENT))
			{
				Options.Get().SetBool(Option.HAS_CLICKED_TOURNAMENT, true);
			}
			AchieveManager.Get().NotifyOfClick(Achievement.ClickTriggerType.BUTTON_PLAY);
			this.FireButtonPressEvent(Box.ButtonType.SET_ROTATION);
		}
	}

	// Token: 0x06001B6F RID: 7023 RVA: 0x0008136C File Offset: 0x0007F56C
	private void OnSoloAdventuresButtonPressed(UIEvent e)
	{
		if (SceneMgr.Get() == null)
		{
			this.ChangeState(Box.State.OPEN);
		}
		else
		{
			this.FireButtonPressEvent(Box.ButtonType.ADVENTURE);
		}
	}

	// Token: 0x06001B70 RID: 7024 RVA: 0x000813A0 File Offset: 0x0007F5A0
	private void OnForgeButtonPressed(UIEvent e)
	{
		if (SceneMgr.Get() == null)
		{
			this.ChangeState(Box.State.OPEN);
		}
		else
		{
			AchieveManager.Get().NotifyOfClick(Achievement.ClickTriggerType.BUTTON_ARENA);
			this.FireButtonPressEvent(Box.ButtonType.FORGE);
		}
	}

	// Token: 0x06001B71 RID: 7025 RVA: 0x000813DC File Offset: 0x0007F5DC
	private void OnTavernBrawlButtonPressed(UIEvent e)
	{
		if (SceneMgr.Get() == null)
		{
			this.ChangeState(Box.State.OPEN);
		}
		else
		{
			this.FireButtonPressEvent(Box.ButtonType.TAVERN_BRAWL);
		}
	}

	// Token: 0x06001B72 RID: 7026 RVA: 0x00081410 File Offset: 0x0007F610
	private void OnOpenPacksButtonPressed(UIEvent e)
	{
		if (SceneMgr.Get() == null)
		{
			this.ChangeState(Box.State.OPEN);
		}
		else
		{
			this.FireButtonPressEvent(Box.ButtonType.OPEN_PACKS);
		}
	}

	// Token: 0x06001B73 RID: 7027 RVA: 0x00081444 File Offset: 0x0007F644
	public void OnCollectionButtonPressed(UIEvent e)
	{
		if (SceneMgr.Get() == null)
		{
			this.ChangeState(Box.State.OPEN);
		}
		else
		{
			this.FireButtonPressEvent(Box.ButtonType.COLLECTION);
		}
	}

	// Token: 0x06001B74 RID: 7028 RVA: 0x00081478 File Offset: 0x0007F678
	public void OnQuestButtonPressed(UIEvent e)
	{
		if (ShownUIMgr.Get().HasShownUI())
		{
			return;
		}
		ShownUIMgr.Get().SetShownUI(ShownUIMgr.UI_WINDOW.QUEST_LOG);
		SoundManager.Get().LoadAndPlay("Small_Click", base.gameObject);
		this.m_tempInputBlocker = CameraUtils.CreateInputBlocker(Box.Get().GetCamera(), "QuestLogInputBlocker", null, 30f);
		SceneUtils.SetLayer(this.m_tempInputBlocker, GameLayer.IgnoreFullScreenEffects);
		this.m_tempInputBlocker.AddComponent<PegUIElement>();
		base.StopCoroutine("ShowQuestLogWhenReady");
		base.StartCoroutine("ShowQuestLogWhenReady");
	}

	// Token: 0x06001B75 RID: 7029 RVA: 0x00081508 File Offset: 0x0007F708
	private void OnButtonMouseOver(UIEvent e)
	{
		PegUIElement element = e.GetElement();
		TooltipZone component = element.gameObject.GetComponent<TooltipZone>();
		if (component == null)
		{
			return;
		}
		bool flag = AchieveManager.Get() != null && AchieveManager.Get().HasUnlockedFeature(Achievement.UnlockableFeature.FORGE);
		string text = GameStrings.Get("GLUE_TOOLTIP_BUTTON_DISABLED_DESC");
		string bodytext = text;
		NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
		bool tournament = netObject.Games.Tournament;
		bool practice = netObject.Games.Practice;
		bool flag2 = netObject.Games.Forge && flag;
		if (flag2)
		{
			flag2 = HealthyGamingMgr.Get().isArenaEnabled();
		}
		bool manager = netObject.Collection.Manager;
		if (component.targetObject == this.m_TournamentButton.gameObject && tournament)
		{
			bodytext = GameStrings.Get("GLUE_TOOLTIP_BUTTON_TOURNAMENT_DESC");
		}
		else if (component.targetObject == this.m_SoloAdventuresButton.gameObject && practice)
		{
			bodytext = GameStrings.Get("GLUE_TOOLTIP_BUTTON_ADVENTURE_DESC");
		}
		else if (component.targetObject == this.m_ForgeButton.gameObject)
		{
			if (flag2)
			{
				bodytext = GameStrings.Get("GLUE_TOOLTIP_BUTTON_FORGE_DESC");
			}
			else if (!flag)
			{
				bodytext = GameStrings.Format("GLUE_TOOLTIP_BUTTON_FORGE_NOT_UNLOCKED", new object[]
				{
					20
				});
			}
		}
		else if (component.targetObject == this.m_TavernBrawlButton.gameObject)
		{
			if (netObject.Games.TavernBrawl)
			{
				if (TavernBrawlManager.Get().HasUnlockedTavernBrawl)
				{
					bodytext = GameStrings.Get("GLUE_TOOLTIP_BUTTON_TAVERN_BRAWL_DESC");
				}
				else
				{
					bodytext = GameStrings.Get("GLUE_TOOLTIP_BUTTON_TAVERN_BRAWL_NOT_UNLOCKED");
				}
			}
			else
			{
				bodytext = text;
			}
		}
		else if (component.targetObject == this.m_OpenPacksButton.gameObject)
		{
			bodytext = GameStrings.Get("GLUE_TOOLTIP_BUTTON_PACKOPEN_DESC");
		}
		else if (component.targetObject == this.m_CollectionButton.gameObject && manager)
		{
			bodytext = GameStrings.Get("GLUE_TOOLTIP_BUTTON_COLLECTION_DESC");
		}
		if (component.targetObject == this.m_TournamentButton.gameObject)
		{
			component.ShowBoxTooltip(GameStrings.Get("GLUE_TOOLTIP_BUTTON_TOURNAMENT_HEADLINE"), bodytext);
		}
		else if (component.targetObject == this.m_SoloAdventuresButton.gameObject)
		{
			component.ShowBoxTooltip(GameStrings.Get("GLUE_TOOLTIP_BUTTON_ADVENTURE_HEADLINE"), bodytext);
		}
		else if (component.targetObject == this.m_ForgeButton.gameObject)
		{
			component.ShowBoxTooltip(GameStrings.Get("GLUE_TOOLTIP_BUTTON_FORGE_HEADLINE"), bodytext);
		}
		else if (component.targetObject == this.m_TavernBrawlButton.gameObject)
		{
			component.ShowBoxTooltip(GameStrings.Get("GLUE_TOOLTIP_BUTTON_TAVERN_BRAWL_HEADLINE"), bodytext);
		}
		else if (component.targetObject == this.m_OpenPacksButton.gameObject)
		{
			component.ShowBoxTooltip(GameStrings.Get("GLUE_TOOLTIP_BUTTON_PACKOPEN_HEADLINE"), bodytext);
		}
		else if (component.targetObject == this.m_CollectionButton.gameObject)
		{
			component.ShowBoxTooltip(GameStrings.Get("GLUE_TOOLTIP_BUTTON_COLLECTION_HEADLINE"), bodytext);
		}
	}

	// Token: 0x06001B76 RID: 7030 RVA: 0x00081864 File Offset: 0x0007FA64
	private void OnButtonMouseOut(UIEvent e)
	{
		PegUIElement element = e.GetElement();
		TooltipZone component = element.gameObject.GetComponent<TooltipZone>();
		if (component == null)
		{
			return;
		}
		component.HideTooltip();
	}

	// Token: 0x06001B77 RID: 7031 RVA: 0x00081898 File Offset: 0x0007FA98
	public void InitializeNet()
	{
		if (SceneMgr.Get() == null)
		{
			return;
		}
		this.m_waitingForNetData = true;
		if (SceneMgr.Get().GetMode() == SceneMgr.Mode.STARTUP)
		{
			return;
		}
		NetCache.Get().RegisterScreenBox(new NetCache.NetCacheCallback(this.OnNetCacheReady));
	}

	// Token: 0x06001B78 RID: 7032 RVA: 0x000818E4 File Offset: 0x0007FAE4
	private void ShutdownNet()
	{
		NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
	}

	// Token: 0x06001B79 RID: 7033 RVA: 0x000818FC File Offset: 0x0007FAFC
	private void HackRequestNetData()
	{
		this.m_waitingForNetData = true;
		this.UpdateUI(false);
		NetCache.Get().ReloadNetObject<NetCache.NetCacheBoosters>();
		NetCache.Get().ReloadNetObject<NetCache.NetCacheFeatures>();
	}

	// Token: 0x06001B7A RID: 7034 RVA: 0x00081920 File Offset: 0x0007FB20
	private void HackRequestNetFeatures()
	{
		this.m_waitingForNetData = true;
		this.UpdateUI(false);
		NetCache.Get().ReloadNetObject<NetCache.NetCacheFeatures>();
	}

	// Token: 0x06001B7B RID: 7035 RVA: 0x0008193C File Offset: 0x0007FB3C
	private void OnNetCacheReady()
	{
		this.m_waitingForNetData = false;
		if (Network.ShouldBeConnectedToAurora())
		{
			RankMgr.Get().SetRankPresenceField(NetCache.Get().GetNetObject<NetCache.NetCacheMedalInfo>());
		}
		this.UpdateUI(false);
	}

	// Token: 0x06001B7C RID: 7036 RVA: 0x00081978 File Offset: 0x0007FB78
	private int ComputeBoosterCount()
	{
		NetCache.NetCacheBoosters netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBoosters>();
		return netObject.GetTotalNumBoosters();
	}

	// Token: 0x06001B7D RID: 7037 RVA: 0x00081996 File Offset: 0x0007FB96
	public void UnloadQuestLog()
	{
		this.m_questLogNetCacheDataState = Box.DataState.UNLOADING;
		this.DestroyQuestLog();
	}

	// Token: 0x06001B7E RID: 7038 RVA: 0x000819A8 File Offset: 0x0007FBA8
	private IEnumerator ShowQuestLogWhenReady()
	{
		if (this.m_questLog == null && !this.m_questLogLoading)
		{
			this.m_questLogLoading = true;
			if (UniversalInputManager.UsePhoneUI)
			{
				AssetLoader.Get().LoadGameObject("QuestLog_phone", new AssetLoader.GameObjectCallback(this.OnQuestLogLoaded), null, false);
			}
			else
			{
				AssetLoader.Get().LoadGameObject("QuestLog", new AssetLoader.GameObjectCallback(this.OnQuestLogLoaded), null, false);
			}
		}
		if (this.ShouldRequestData(this.m_questLogNetCacheDataState))
		{
			this.m_questLogNetCacheDataState = Box.DataState.REQUEST_SENT;
			NetCache.Get().RegisterScreenQuestLog(new NetCache.NetCacheCallback(this.OnQuestLogNetCacheReady));
		}
		while (this.m_questLog == null)
		{
			yield return null;
		}
		while (this.m_questLogNetCacheDataState != Box.DataState.RECEIVED)
		{
			yield return null;
		}
		this.m_questLog.Show();
		yield return new WaitForSeconds(0.5f);
		Object.Destroy(this.m_tempInputBlocker);
		yield break;
	}

	// Token: 0x06001B7F RID: 7039 RVA: 0x000819C3 File Offset: 0x0007FBC3
	private void DestroyQuestLog()
	{
		base.StopCoroutine("ShowQuestLogWhenReady");
		if (this.m_questLog == null)
		{
			return;
		}
		Object.Destroy(this.m_questLog.gameObject);
	}

	// Token: 0x06001B80 RID: 7040 RVA: 0x000819F4 File Offset: 0x0007FBF4
	private void OnQuestLogLoaded(string name, GameObject go, object callbackData)
	{
		this.m_questLogLoading = false;
		if (go == null)
		{
			Debug.LogError(string.Format("QuestLogButton.OnQuestLogLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		this.m_questLog = go.GetComponent<QuestLog>();
		if (this.m_questLog != null)
		{
			return;
		}
		Debug.LogError(string.Format("QuestLogButton.OnQuestLogLoaded() - ERROR \"{0}\" has no {1} component", name, typeof(QuestLog)));
	}

	// Token: 0x06001B81 RID: 7041 RVA: 0x00081A5D File Offset: 0x0007FC5D
	private void OnQuestLogNetCacheReady()
	{
		if (this.m_questLogNetCacheDataState == Box.DataState.UNLOADING)
		{
			return;
		}
		this.m_questLogNetCacheDataState = Box.DataState.RECEIVED;
	}

	// Token: 0x06001B82 RID: 7042 RVA: 0x00081A73 File Offset: 0x0007FC73
	private bool ShouldRequestData(Box.DataState state)
	{
		return state == Box.DataState.NONE || state == Box.DataState.UNLOADING;
	}

	// Token: 0x06001B83 RID: 7043 RVA: 0x00081A88 File Offset: 0x0007FC88
	private void OnStoreButtonReleased(UIEvent e)
	{
		if (FriendChallengeMgr.Get().HasChallenge())
		{
			return;
		}
		if (this.m_StoreButton.IsVisualClosed())
		{
			return;
		}
		if (!StoreManager.Get().IsOpen())
		{
			SoundManager.Get().LoadAndPlay("Store_closed_button_click", base.gameObject);
			return;
		}
		FriendChallengeMgr.Get().OnStoreOpened();
		SoundManager.Get().LoadAndPlay("Small_Click", base.gameObject);
		StoreManager.Get().RegisterStoreShownListener(new StoreManager.StoreShownCallback(this.OnStoreShown));
		StoreManager.Get().StartGeneralTransaction();
	}

	// Token: 0x06001B84 RID: 7044 RVA: 0x00081B1B File Offset: 0x0007FD1B
	private void OnStoreShown(object userData)
	{
		StoreManager.Get().RemoveStoreShownListener(new StoreManager.StoreShownCallback(this.OnStoreShown));
	}

	// Token: 0x06001B85 RID: 7045 RVA: 0x00081B34 File Offset: 0x0007FD34
	private void SetRotation_ShowRotationDisk()
	{
		this.m_DiskCenter.SetActive(false);
		if (this.m_setRotationDisk != null)
		{
			this.m_setRotationDisk.SetActive(true);
			return;
		}
		this.m_setRotationDisk = AssetLoader.Get().LoadGameObject("TheBox_CenterDisk_SetRotation", true, false);
		this.m_setRotationDisk.SetActive(true);
		this.m_setRotationDisk.transform.parent = this.m_Disk.transform;
		this.m_setRotationDisk.transform.localPosition = Vector3.zero;
		this.m_setRotationDisk.transform.localRotation = Quaternion.identity;
		this.m_setRotationButton = this.m_setRotationDisk.GetComponentInChildren<BoxMenuButton>();
		this.m_StoreButton.gameObject.SetActive(false);
		this.m_QuestLogButton.gameObject.SetActive(false);
		HighlightState componentInChildren = this.m_setRotationButton.GetComponentInChildren<HighlightState>();
		if (componentInChildren != null)
		{
			componentInChildren.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
		}
		this.RegisterButtonEvents(this.m_setRotationButton);
	}

	// Token: 0x06001B86 RID: 7046 RVA: 0x00081C34 File Offset: 0x0007FE34
	private IEnumerator SetRotationOpen_ChangeState()
	{
		Box.BoxStateConfig config = this.m_stateConfigs[12];
		if (!config.m_logoState.m_ignore)
		{
			this.m_Logo.ChangeState(config.m_logoState.m_state);
		}
		if (!config.m_startButtonState.m_ignore)
		{
			this.m_StartButton.ChangeState(config.m_startButtonState.m_state);
		}
		if (!config.m_doorState.m_ignore)
		{
			this.m_LeftDoor.ChangeState(config.m_doorState.m_state);
			this.m_RightDoor.ChangeState(config.m_doorState.m_state);
		}
		if (!config.m_diskState.m_ignore)
		{
			this.m_Disk.ChangeState(config.m_diskState.m_state);
		}
		if (!config.m_camState.m_ignore)
		{
			this.m_Camera.ChangeState(BoxCamera.State.SET_ROTATION_OPENED);
		}
		if (!config.m_fullScreenBlackState.m_ignore)
		{
			this.FullScreenBlack_ChangeState(config.m_fullScreenBlackState.m_state);
		}
		SetRotationClock clock = SetRotationClock.Get();
		if (clock == null)
		{
			Debug.LogError("SetRotationOpen_ChangeState clock = null");
			yield break;
		}
		clock.StartTheClock();
		yield break;
	}

	// Token: 0x06001B87 RID: 7047 RVA: 0x00081C50 File Offset: 0x0007FE50
	private void SetRotation_ShowUnAckedRewardsAndQuests()
	{
		UserAttentionManager.StartBlocking(UserAttentionBlocker.SET_ROTATION_INTRO);
		NetCache.NetCacheProfileNotices netObject = NetCache.Get().GetNetObject<NetCache.NetCacheProfileNotices>();
		List<RewardData> list = new List<RewardData>();
		if (netObject != null)
		{
			List<RewardData> rewards = RewardUtils.GetRewards(netObject.Notices);
			rewards.RemoveAll((RewardData n) => n.Origin != NetCache.ProfileNotice.NoticeOrigin.ACHIEVEMENT || n.OriginData != 339L);
			HashSet<RewardVisualTiming> hashSet = new HashSet<RewardVisualTiming>();
			foreach (object obj in Enum.GetValues(typeof(RewardVisualTiming)))
			{
				RewardVisualTiming rewardVisualTiming = (RewardVisualTiming)((int)obj);
				hashSet.Add(rewardVisualTiming);
			}
			RewardUtils.GetViewableRewards(rewards, hashSet, ref list, ref this.m_completedQuests);
		}
		if (list.Count == 0)
		{
			this.SetRotation_ShowNextUnAckedRewardOrCompletedQuest();
			return;
		}
		this.m_numRewardsToLoad = list.Count;
		foreach (RewardData rewardData in list)
		{
			rewardData.LoadRewardObject(new Reward.DelOnRewardLoaded(this.SetRotation_OnRewardObjectLoaded));
		}
	}

	// Token: 0x06001B88 RID: 7048 RVA: 0x00081D9C File Offset: 0x0007FF9C
	private void SetRotation_OnRewardObjectLoaded(Reward reward, object callbackData)
	{
		reward.Hide(false);
		reward.transform.parent = base.transform;
		reward.transform.localRotation = Quaternion.identity;
		reward.transform.localPosition = Login.REWARD_LOCAL_POS;
		this.m_rewards.Add(reward);
		if (reward.RewardType == Reward.Type.CARD)
		{
			CardReward cardReward = reward as CardReward;
			cardReward.MakeActorsUnlit();
		}
		SceneUtils.SetLayer(reward.gameObject, GameLayer.Default);
		this.m_numRewardsToLoad--;
		if (this.m_numRewardsToLoad > 0)
		{
			return;
		}
		RewardUtils.SortRewards(ref this.m_rewards);
		this.SetRotation_ShowNextUnAckedRewardOrCompletedQuest();
	}

	// Token: 0x06001B89 RID: 7049 RVA: 0x00081E40 File Offset: 0x00080040
	private void SetRotation_ShowNextUnAckedRewardOrCompletedQuest()
	{
		if (BannerManager.Get().ShowABanner(new BannerManager.DelOnCloseBanner(this.SetRotation_ShowNextUnAckedRewardOrCompletedQuest)))
		{
			return;
		}
		if (this.SetRotation_ShowNextCompletedQuest())
		{
			return;
		}
		if (this.SetRotation_ShowNextUnAckedReward())
		{
			return;
		}
		if (!Login.ShowNerfedCards(new DialogBase.HideCallback(this.SetRotation_ShowNerfedCards_DialogHidden)))
		{
			this.SetRotation_FinishShowingRewards();
		}
	}

	// Token: 0x06001B8A RID: 7050 RVA: 0x00081E9D File Offset: 0x0008009D
	private void SetRotation_ShowNerfedCards_DialogHidden(DialogBase dialog, object userData)
	{
		this.SetRotation_FinishShowingRewards();
	}

	// Token: 0x06001B8B RID: 7051 RVA: 0x00081EA8 File Offset: 0x000800A8
	private void SetRotation_FinishShowingRewards()
	{
		this.ChangeState(Box.State.SET_ROTATION);
		this.SetRotation_ShowRotationDisk();
		this.m_completedQuests.Clear();
		this.m_rewards.Clear();
		this.m_numRewardsToLoad = 0;
	}

	// Token: 0x06001B8C RID: 7052 RVA: 0x00081EE4 File Offset: 0x000800E4
	private bool SetRotation_ShowNextCompletedQuest()
	{
		if (QuestToast.IsQuestActive())
		{
			QuestToast.GetCurrentToast().CloseQuestToast();
		}
		if (this.m_completedQuests.Count == 0)
		{
			return false;
		}
		Achievement quest = this.m_completedQuests[0];
		this.m_completedQuests.RemoveAt(0);
		QuestToast.ShowQuestToast(UserAttentionBlocker.SET_ROTATION_INTRO, delegate(object userData)
		{
			this.SetRotation_ShowNextUnAckedRewardOrCompletedQuest();
		}, false, quest);
		return true;
	}

	// Token: 0x06001B8D RID: 7053 RVA: 0x00081F48 File Offset: 0x00080148
	private bool SetRotation_ShowNextUnAckedReward()
	{
		if (this.m_rewards.Count == 0 || !UserAttentionManager.CanShowAttentionGrabber("Login.ShowNextUnAckedReward"))
		{
			return false;
		}
		Reward reward = this.m_rewards[0];
		this.m_rewards.RemoveAt(0);
		RewardUtils.ShowReward(UserAttentionBlocker.SET_ROTATION_INTRO, reward, false, Login.REWARD_PUNCH_SCALE, Login.REWARD_SCALE, "SetRotation_OnRewardShown", reward, base.gameObject);
		return true;
	}

	// Token: 0x06001B8E RID: 7054 RVA: 0x00081FB8 File Offset: 0x000801B8
	private void SetRotation_OnRewardShown(object callbackData)
	{
		Reward reward = callbackData as Reward;
		if (reward == null)
		{
			return;
		}
		reward.RegisterClickListener(new Reward.OnClickedCallback(this.SetRotation_OnRewardClicked));
		reward.EnableClickCatcher(true);
	}

	// Token: 0x06001B8F RID: 7055 RVA: 0x00081FF4 File Offset: 0x000801F4
	private void SetRotation_OnRewardClicked(Reward reward, object userData)
	{
		reward.RemoveClickListener(new Reward.OnClickedCallback(this.SetRotation_OnRewardClicked));
		reward.Hide(true);
		this.SetRotation_ShowNextUnAckedRewardOrCompletedQuest();
	}

	// Token: 0x06001B90 RID: 7056 RVA: 0x00082024 File Offset: 0x00080224
	private void InitializeComponents()
	{
		this.m_Logo.SetParent(this);
		this.m_Logo.SetInfo(this.m_StateInfoList.m_LogoInfo);
		this.m_StartButton.SetParent(this);
		this.m_StartButton.SetInfo(this.m_StateInfoList.m_StartButtonInfo);
		this.m_LeftDoor.SetParent(this);
		this.m_LeftDoor.SetInfo(this.m_StateInfoList.m_LeftDoorInfo);
		this.m_RightDoor.SetParent(this);
		this.m_RightDoor.SetInfo(this.m_StateInfoList.m_RightDoorInfo);
		this.m_RightDoor.EnableMaster(true);
		this.m_Disk.SetParent(this);
		this.m_Disk.SetInfo(this.m_StateInfoList.m_DiskInfo);
		this.m_TopSpinner.SetParent(this);
		this.m_TopSpinner.SetInfo(this.m_StateInfoList.m_SpinnerInfo);
		this.m_BottomSpinner.SetParent(this);
		this.m_BottomSpinner.SetInfo(this.m_StateInfoList.m_SpinnerInfo);
		this.m_Drawer.SetParent(this);
		this.m_Drawer.SetInfo(this.m_StateInfoList.m_DrawerInfo);
		this.m_Camera.SetParent(this);
		this.m_Camera.SetInfo(this.m_StateInfoList.m_CameraInfo);
		FullScreenEffects component = this.m_Camera.GetComponent<FullScreenEffects>();
		component.BlendToColorEnable = false;
	}

	// Token: 0x06001B91 RID: 7057 RVA: 0x00082184 File Offset: 0x00080384
	private void OnBoxTopPhoneTextureLoaded(string name, Object obj, object callbackData)
	{
		Texture texture = obj as Texture;
		MeshRenderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			Material sharedMaterial = meshRenderer.sharedMaterial;
			if (sharedMaterial != null)
			{
				Texture texture2 = sharedMaterial.GetTexture(0);
				if (texture2 != null && texture2.name.Equals("TheBox_Top"))
				{
					meshRenderer.material.SetTexture(0, texture);
				}
			}
		}
	}

	// Token: 0x04000E01 RID: 3585
	private const string SHOW_LOG_COROUTINE = "ShowQuestLogWhenReady";

	// Token: 0x04000E02 RID: 3586
	public GameObject m_rootObject;

	// Token: 0x04000E03 RID: 3587
	public BoxStateInfoList m_StateInfoList;

	// Token: 0x04000E04 RID: 3588
	public BoxLogo m_Logo;

	// Token: 0x04000E05 RID: 3589
	public BoxStartButton m_StartButton;

	// Token: 0x04000E06 RID: 3590
	public BoxDoor m_LeftDoor;

	// Token: 0x04000E07 RID: 3591
	public BoxDoor m_RightDoor;

	// Token: 0x04000E08 RID: 3592
	public BoxDisk m_Disk;

	// Token: 0x04000E09 RID: 3593
	public GameObject m_DiskCenter;

	// Token: 0x04000E0A RID: 3594
	public BoxSpinner m_TopSpinner;

	// Token: 0x04000E0B RID: 3595
	public BoxSpinner m_BottomSpinner;

	// Token: 0x04000E0C RID: 3596
	public BoxDrawer m_Drawer;

	// Token: 0x04000E0D RID: 3597
	public BoxCamera m_Camera;

	// Token: 0x04000E0E RID: 3598
	public Camera m_NoFxCamera;

	// Token: 0x04000E0F RID: 3599
	public AudioListener m_AudioListener;

	// Token: 0x04000E10 RID: 3600
	public BoxLightMgr m_LightMgr;

	// Token: 0x04000E11 RID: 3601
	public BoxEventMgr m_EventMgr;

	// Token: 0x04000E12 RID: 3602
	public BoxMenuButton m_TournamentButton;

	// Token: 0x04000E13 RID: 3603
	public BoxMenuButton m_SoloAdventuresButton;

	// Token: 0x04000E14 RID: 3604
	public BoxMenuButton m_ForgeButton;

	// Token: 0x04000E15 RID: 3605
	public TavernBrawlMenuButton m_TavernBrawlButton;

	// Token: 0x04000E16 RID: 3606
	public GameObject m_EmptyFourthButton;

	// Token: 0x04000E17 RID: 3607
	public GameObject m_TavernBrawlButtonVisual;

	// Token: 0x04000E18 RID: 3608
	public GameObject m_TavernBrawlButtonActivateFX;

	// Token: 0x04000E19 RID: 3609
	public GameObject m_TavernBrawlButtonDeactivateFX;

	// Token: 0x04000E1A RID: 3610
	public string m_tavernBrawlActivateSound;

	// Token: 0x04000E1B RID: 3611
	public string m_tavernBrawlDeactivateSound;

	// Token: 0x04000E1C RID: 3612
	public string m_tavernBrawlPopupSound;

	// Token: 0x04000E1D RID: 3613
	public string m_tavernBrawlPopdownSound;

	// Token: 0x04000E1E RID: 3614
	public PackOpeningButton m_OpenPacksButton;

	// Token: 0x04000E1F RID: 3615
	public BoxMenuButton m_CollectionButton;

	// Token: 0x04000E20 RID: 3616
	public StoreButton m_StoreButton;

	// Token: 0x04000E21 RID: 3617
	public QuestLogButton m_QuestLogButton;

	// Token: 0x04000E22 RID: 3618
	public Color m_EnabledMaterial;

	// Token: 0x04000E23 RID: 3619
	public Color m_DisabledMaterial;

	// Token: 0x04000E24 RID: 3620
	public Color m_EnabledDrawerMaterial;

	// Token: 0x04000E25 RID: 3621
	public Color m_DisabledDrawerMaterial;

	// Token: 0x04000E26 RID: 3622
	public GameObject m_OuterFrame;

	// Token: 0x04000E27 RID: 3623
	public Texture2D m_textureCompressionTest;

	// Token: 0x04000E28 RID: 3624
	public RibbonButtonsUI m_ribbonButtons;

	// Token: 0x04000E29 RID: 3625
	public GameObject m_letterboxingContainer;

	// Token: 0x04000E2A RID: 3626
	public GameObject m_tableTop;

	// Token: 0x04000E2B RID: 3627
	private static Box s_instance;

	// Token: 0x04000E2C RID: 3628
	private Box.BoxStateConfig[] m_stateConfigs;

	// Token: 0x04000E2D RID: 3629
	private Box.State m_state = Box.State.STARTUP;

	// Token: 0x04000E2E RID: 3630
	private int m_pendingEffects;

	// Token: 0x04000E2F RID: 3631
	private Queue<Box.State> m_stateQueue = new Queue<Box.State>();

	// Token: 0x04000E30 RID: 3632
	private bool m_transitioningToSceneMode;

	// Token: 0x04000E31 RID: 3633
	private List<Box.TransitionFinishedListener> m_transitionFinishedListeners = new List<Box.TransitionFinishedListener>();

	// Token: 0x04000E32 RID: 3634
	private List<Box.ButtonPressListener> m_buttonPressListeners = new List<Box.ButtonPressListener>();

	// Token: 0x04000E33 RID: 3635
	private Box.ButtonType? m_queuedButtonFire;

	// Token: 0x04000E34 RID: 3636
	private bool m_waitingForNetData;

	// Token: 0x04000E35 RID: 3637
	private GameLayer m_originalLeftDoorLayer;

	// Token: 0x04000E36 RID: 3638
	private GameLayer m_originalRightDoorLayer;

	// Token: 0x04000E37 RID: 3639
	private GameLayer m_originalDrawerLayer;

	// Token: 0x04000E38 RID: 3640
	private bool m_waitingForSceneLoad;

	// Token: 0x04000E39 RID: 3641
	private bool m_showRibbonButtons;

	// Token: 0x04000E3A RID: 3642
	private bool m_questLogLoading;

	// Token: 0x04000E3B RID: 3643
	private Box.DataState m_questLogNetCacheDataState;

	// Token: 0x04000E3C RID: 3644
	private QuestLog m_questLog;

	// Token: 0x04000E3D RID: 3645
	private GameObject m_tempInputBlocker;

	// Token: 0x04000E3E RID: 3646
	private GameObject m_setRotationDisk;

	// Token: 0x04000E3F RID: 3647
	private BoxMenuButton m_setRotationButton;

	// Token: 0x04000E40 RID: 3648
	private List<Achievement> m_completedQuests = new List<Achievement>();

	// Token: 0x04000E41 RID: 3649
	private List<Reward> m_rewards = new List<Reward>();

	// Token: 0x04000E42 RID: 3650
	private int m_numRewardsToLoad;

	// Token: 0x0200021A RID: 538
	// (Invoke) Token: 0x060020F1 RID: 8433
	public delegate void TransitionFinishedCallback(object userData);

	// Token: 0x0200023D RID: 573
	public enum State
	{
		// Token: 0x040012CA RID: 4810
		INVALID,
		// Token: 0x040012CB RID: 4811
		STARTUP,
		// Token: 0x040012CC RID: 4812
		PRESS_START,
		// Token: 0x040012CD RID: 4813
		LOADING,
		// Token: 0x040012CE RID: 4814
		LOADING_HUB,
		// Token: 0x040012CF RID: 4815
		HUB,
		// Token: 0x040012D0 RID: 4816
		HUB_WITH_DRAWER,
		// Token: 0x040012D1 RID: 4817
		OPEN,
		// Token: 0x040012D2 RID: 4818
		CLOSED,
		// Token: 0x040012D3 RID: 4819
		ERROR,
		// Token: 0x040012D4 RID: 4820
		SET_ROTATION_LOADING,
		// Token: 0x040012D5 RID: 4821
		SET_ROTATION,
		// Token: 0x040012D6 RID: 4822
		SET_ROTATION_OPEN
	}

	// Token: 0x0200023E RID: 574
	public enum ButtonType
	{
		// Token: 0x040012D8 RID: 4824
		START,
		// Token: 0x040012D9 RID: 4825
		TOURNAMENT,
		// Token: 0x040012DA RID: 4826
		ADVENTURE,
		// Token: 0x040012DB RID: 4827
		FORGE,
		// Token: 0x040012DC RID: 4828
		OPEN_PACKS,
		// Token: 0x040012DD RID: 4829
		COLLECTION,
		// Token: 0x040012DE RID: 4830
		TAVERN_BRAWL,
		// Token: 0x040012DF RID: 4831
		SET_ROTATION
	}

	// Token: 0x0200023F RID: 575
	private class TransitionFinishedListener : EventListener<Box.TransitionFinishedCallback>
	{
		// Token: 0x06002178 RID: 8568 RVA: 0x000A3B94 File Offset: 0x000A1D94
		public void Fire()
		{
			this.m_callback(this.m_userData);
		}
	}

	// Token: 0x02000240 RID: 576
	private class ButtonPressListener : EventListener<Box.ButtonPressCallback>
	{
		// Token: 0x0600217A RID: 8570 RVA: 0x000A3BAF File Offset: 0x000A1DAF
		public void Fire(Box.ButtonType buttonType)
		{
			this.m_callback(buttonType, this.m_userData);
		}
	}

	// Token: 0x02000241 RID: 577
	// (Invoke) Token: 0x0600217C RID: 8572
	public delegate void ButtonPressCallback(Box.ButtonType buttonType, object userData);

	// Token: 0x02000242 RID: 578
	private class BoxStateConfig
	{
		// Token: 0x040012E0 RID: 4832
		public Box.BoxStateConfig.Part<BoxLogo.State> m_logoState = new Box.BoxStateConfig.Part<BoxLogo.State>();

		// Token: 0x040012E1 RID: 4833
		public Box.BoxStateConfig.Part<BoxStartButton.State> m_startButtonState = new Box.BoxStateConfig.Part<BoxStartButton.State>();

		// Token: 0x040012E2 RID: 4834
		public Box.BoxStateConfig.Part<BoxDoor.State> m_doorState = new Box.BoxStateConfig.Part<BoxDoor.State>();

		// Token: 0x040012E3 RID: 4835
		public Box.BoxStateConfig.Part<BoxDisk.State> m_diskState = new Box.BoxStateConfig.Part<BoxDisk.State>();

		// Token: 0x040012E4 RID: 4836
		public Box.BoxStateConfig.Part<BoxDrawer.State> m_drawerState = new Box.BoxStateConfig.Part<BoxDrawer.State>();

		// Token: 0x040012E5 RID: 4837
		public Box.BoxStateConfig.Part<BoxCamera.State> m_camState = new Box.BoxStateConfig.Part<BoxCamera.State>();

		// Token: 0x040012E6 RID: 4838
		public Box.BoxStateConfig.Part<bool> m_fullScreenBlackState = new Box.BoxStateConfig.Part<bool>();

		// Token: 0x02000243 RID: 579
		public class Part<T>
		{
			// Token: 0x040012E7 RID: 4839
			public bool m_ignore;

			// Token: 0x040012E8 RID: 4840
			public T m_state;
		}
	}

	// Token: 0x02000250 RID: 592
	private enum DataState
	{
		// Token: 0x04001321 RID: 4897
		NONE,
		// Token: 0x04001322 RID: 4898
		REQUEST_SENT,
		// Token: 0x04001323 RID: 4899
		RECEIVED,
		// Token: 0x04001324 RID: 4900
		UNLOADING
	}
}
