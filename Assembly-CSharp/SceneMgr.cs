using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

// Token: 0x02000123 RID: 291
public class SceneMgr : MonoBehaviour
{
	// Token: 0x06000ED9 RID: 3801 RVA: 0x00040440 File Offset: 0x0003E640
	private void Awake()
	{
		SceneMgr.s_instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
		FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
		this.m_transitioning = true;
		ApplicationMgr.Get().WillReset += new Action(this.WillReset);
	}

	// Token: 0x06000EDA RID: 3802 RVA: 0x00040492 File Offset: 0x0003E692
	private void OnDestroy()
	{
		ApplicationMgr.Get().WillReset -= new Action(this.WillReset);
		SceneMgr.s_instance = null;
	}

	// Token: 0x06000EDB RID: 3803 RVA: 0x000404B0 File Offset: 0x0003E6B0
	private void Start()
	{
		if (this.IsModeRequested(SceneMgr.Mode.FATAL_ERROR))
		{
			return;
		}
		base.StartCoroutine("LoadAssetsWhenReady");
	}

	// Token: 0x06000EDC RID: 3804 RVA: 0x000404CC File Offset: 0x0003E6CC
	private IEnumerator LoadAssetsWhenReady()
	{
		while (!AssetLoader.Get().IsReady())
		{
			yield return new WaitForSeconds(0.1f);
		}
		FontTable.Get().AddInitializedCallback(new FontTable.InitializedCallback(this.OnFontTableInitialized));
		FontTable.Get().Initialize();
		UberText.LoadCachedData();
		yield break;
	}

	// Token: 0x06000EDD RID: 3805 RVA: 0x000404E7 File Offset: 0x0003E6E7
	public void LoadShaderPreCompiler()
	{
	}

	// Token: 0x06000EDE RID: 3806 RVA: 0x000404EC File Offset: 0x0003E6EC
	private void Update()
	{
		if (!this.m_reloadMode)
		{
			if (this.m_nextMode == SceneMgr.Mode.INVALID)
			{
				return;
			}
			if (this.m_mode == this.m_nextMode)
			{
				this.m_nextMode = SceneMgr.Mode.INVALID;
				return;
			}
		}
		this.m_transitioning = true;
		this.m_performFullCleanup = !this.m_reloadMode;
		this.m_prevMode = this.m_mode;
		this.m_mode = this.m_nextMode;
		this.m_nextMode = SceneMgr.Mode.INVALID;
		this.m_reloadMode = false;
		if (this.m_scene != null)
		{
			base.StopCoroutine("SwitchMode");
			base.StartCoroutine("SwitchMode");
		}
		else
		{
			this.LoadMode();
		}
	}

	// Token: 0x06000EDF RID: 3807 RVA: 0x00040598 File Offset: 0x0003E798
	public static SceneMgr Get()
	{
		return SceneMgr.s_instance;
	}

	// Token: 0x06000EE0 RID: 3808 RVA: 0x000405A0 File Offset: 0x0003E7A0
	private void WillReset()
	{
		Log.Reset.Print("SceneMgr.WillReset()", new object[0]);
		if (ApplicationMgr.IsPublic())
		{
			Time.timeScale = 1f;
		}
		base.StopAllCoroutines();
		FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
		this.m_mode = SceneMgr.Mode.STARTUP;
		this.m_nextMode = SceneMgr.Mode.INVALID;
		this.m_prevMode = SceneMgr.Mode.INVALID;
		this.m_reloadMode = false;
		Scene scene = this.m_scene;
		if (scene != null)
		{
			scene.PreUnload();
		}
		this.FireScenePreUnloadEvent(scene);
		if (this.m_scene != null)
		{
			this.m_scene.Unload();
			this.m_scene = null;
			this.m_sceneLoaded = false;
		}
		this.FireSceneUnloadedEvent(scene);
		this.PostUnloadCleanup();
		base.StartCoroutine("WaitThenLoadAssets");
		Log.Reset.Print("\tSceneMgr.WillReset() completed", new object[0]);
	}

	// Token: 0x06000EE1 RID: 3809 RVA: 0x00040688 File Offset: 0x0003E888
	private IEnumerator WaitThenLoadAssets()
	{
		yield return 0;
		base.StartCoroutine("LoadAssetsWhenReady");
		yield break;
	}

	// Token: 0x06000EE2 RID: 3810 RVA: 0x000406A3 File Offset: 0x0003E8A3
	public void SetNextMode(SceneMgr.Mode mode)
	{
		if (this.IsModeRequested(SceneMgr.Mode.FATAL_ERROR))
		{
			return;
		}
		this.CacheModeForResume(mode);
		this.m_nextMode = mode;
		this.m_reloadMode = false;
	}

	// Token: 0x06000EE3 RID: 3811 RVA: 0x000406C8 File Offset: 0x0003E8C8
	public void ReloadMode()
	{
		if (this.IsModeRequested(SceneMgr.Mode.FATAL_ERROR))
		{
			return;
		}
		this.m_nextMode = this.m_mode;
		this.m_reloadMode = true;
	}

	// Token: 0x06000EE4 RID: 3812 RVA: 0x000406EB File Offset: 0x0003E8EB
	public SceneMgr.Mode GetPrevMode()
	{
		return this.m_prevMode;
	}

	// Token: 0x06000EE5 RID: 3813 RVA: 0x000406F3 File Offset: 0x0003E8F3
	public SceneMgr.Mode GetMode()
	{
		return this.m_mode;
	}

	// Token: 0x06000EE6 RID: 3814 RVA: 0x000406FB File Offset: 0x0003E8FB
	public SceneMgr.Mode GetNextMode()
	{
		return this.m_nextMode;
	}

	// Token: 0x06000EE7 RID: 3815 RVA: 0x00040703 File Offset: 0x0003E903
	public Scene GetScene()
	{
		return this.m_scene;
	}

	// Token: 0x06000EE8 RID: 3816 RVA: 0x0004070B File Offset: 0x0003E90B
	public void SetScene(Scene scene)
	{
		this.m_scene = scene;
	}

	// Token: 0x06000EE9 RID: 3817 RVA: 0x00040714 File Offset: 0x0003E914
	public bool IsSceneLoaded()
	{
		return this.m_sceneLoaded;
	}

	// Token: 0x06000EEA RID: 3818 RVA: 0x0004071C File Offset: 0x0003E91C
	public bool WillTransition()
	{
		return this.m_reloadMode || (this.m_nextMode != SceneMgr.Mode.INVALID && this.m_nextMode != this.m_mode);
	}

	// Token: 0x06000EEB RID: 3819 RVA: 0x00040757 File Offset: 0x0003E957
	public bool IsTransitioning()
	{
		return this.m_transitioning;
	}

	// Token: 0x06000EEC RID: 3820 RVA: 0x0004075F File Offset: 0x0003E95F
	public bool IsTransitionNowOrPending()
	{
		return this.IsTransitioning() || this.WillTransition();
	}

	// Token: 0x06000EED RID: 3821 RVA: 0x0004077C File Offset: 0x0003E97C
	public bool IsModeRequested(SceneMgr.Mode mode)
	{
		return this.m_mode == mode || this.m_nextMode == mode;
	}

	// Token: 0x06000EEE RID: 3822 RVA: 0x0004079B File Offset: 0x0003E99B
	public bool IsInGame()
	{
		return this.IsModeRequested(SceneMgr.Mode.GAMEPLAY);
	}

	// Token: 0x06000EEF RID: 3823 RVA: 0x000407A4 File Offset: 0x0003E9A4
	public void NotifySceneLoaded()
	{
		this.m_sceneLoaded = true;
		if (this.ShouldUseSceneLoadDelays())
		{
			base.StartCoroutine(this.WaitThenFireSceneLoadedEvent());
		}
		else
		{
			this.FireSceneLoadedEvent();
		}
	}

	// Token: 0x06000EF0 RID: 3824 RVA: 0x000407D0 File Offset: 0x0003E9D0
	public void RegisterScenePreUnloadEvent(SceneMgr.ScenePreUnloadCallback callback)
	{
		this.RegisterScenePreUnloadEvent(callback, null);
	}

	// Token: 0x06000EF1 RID: 3825 RVA: 0x000407DC File Offset: 0x0003E9DC
	public void RegisterScenePreUnloadEvent(SceneMgr.ScenePreUnloadCallback callback, object userData)
	{
		SceneMgr.ScenePreUnloadListener scenePreUnloadListener = new SceneMgr.ScenePreUnloadListener();
		scenePreUnloadListener.SetCallback(callback);
		scenePreUnloadListener.SetUserData(userData);
		if (this.m_scenePreUnloadListeners.Contains(scenePreUnloadListener))
		{
			return;
		}
		this.m_scenePreUnloadListeners.Add(scenePreUnloadListener);
	}

	// Token: 0x06000EF2 RID: 3826 RVA: 0x0004081B File Offset: 0x0003EA1B
	public bool UnregisterScenePreUnloadEvent(SceneMgr.ScenePreUnloadCallback callback)
	{
		return this.UnregisterScenePreUnloadEvent(callback, null);
	}

	// Token: 0x06000EF3 RID: 3827 RVA: 0x00040828 File Offset: 0x0003EA28
	public bool UnregisterScenePreUnloadEvent(SceneMgr.ScenePreUnloadCallback callback, object userData)
	{
		SceneMgr.ScenePreUnloadListener scenePreUnloadListener = new SceneMgr.ScenePreUnloadListener();
		scenePreUnloadListener.SetCallback(callback);
		scenePreUnloadListener.SetUserData(userData);
		return this.m_scenePreUnloadListeners.Remove(scenePreUnloadListener);
	}

	// Token: 0x06000EF4 RID: 3828 RVA: 0x00040855 File Offset: 0x0003EA55
	public void RegisterSceneUnloadedEvent(SceneMgr.SceneUnloadedCallback callback)
	{
		this.RegisterSceneUnloadedEvent(callback, null);
	}

	// Token: 0x06000EF5 RID: 3829 RVA: 0x00040860 File Offset: 0x0003EA60
	public void RegisterSceneUnloadedEvent(SceneMgr.SceneUnloadedCallback callback, object userData)
	{
		SceneMgr.SceneUnloadedListener sceneUnloadedListener = new SceneMgr.SceneUnloadedListener();
		sceneUnloadedListener.SetCallback(callback);
		sceneUnloadedListener.SetUserData(userData);
		if (this.m_sceneUnloadedListeners.Contains(sceneUnloadedListener))
		{
			return;
		}
		this.m_sceneUnloadedListeners.Add(sceneUnloadedListener);
	}

	// Token: 0x06000EF6 RID: 3830 RVA: 0x0004089F File Offset: 0x0003EA9F
	public bool UnregisterSceneUnloadedEvent(SceneMgr.SceneUnloadedCallback callback)
	{
		return this.UnregisterSceneUnloadedEvent(callback, null);
	}

	// Token: 0x06000EF7 RID: 3831 RVA: 0x000408AC File Offset: 0x0003EAAC
	public bool UnregisterSceneUnloadedEvent(SceneMgr.SceneUnloadedCallback callback, object userData)
	{
		SceneMgr.SceneUnloadedListener sceneUnloadedListener = new SceneMgr.SceneUnloadedListener();
		sceneUnloadedListener.SetCallback(callback);
		sceneUnloadedListener.SetUserData(userData);
		return this.m_sceneUnloadedListeners.Remove(sceneUnloadedListener);
	}

	// Token: 0x06000EF8 RID: 3832 RVA: 0x000408D9 File Offset: 0x0003EAD9
	public void RegisterScenePreLoadEvent(SceneMgr.ScenePreLoadCallback callback)
	{
		this.RegisterScenePreLoadEvent(callback, null);
	}

	// Token: 0x06000EF9 RID: 3833 RVA: 0x000408E4 File Offset: 0x0003EAE4
	public void RegisterScenePreLoadEvent(SceneMgr.ScenePreLoadCallback callback, object userData)
	{
		SceneMgr.ScenePreLoadListener scenePreLoadListener = new SceneMgr.ScenePreLoadListener();
		scenePreLoadListener.SetCallback(callback);
		scenePreLoadListener.SetUserData(userData);
		if (this.m_scenePreLoadListeners.Contains(scenePreLoadListener))
		{
			return;
		}
		this.m_scenePreLoadListeners.Add(scenePreLoadListener);
	}

	// Token: 0x06000EFA RID: 3834 RVA: 0x00040923 File Offset: 0x0003EB23
	public bool UnregisterScenePreLoadEvent(SceneMgr.ScenePreLoadCallback callback)
	{
		return this.UnregisterScenePreLoadEvent(callback, null);
	}

	// Token: 0x06000EFB RID: 3835 RVA: 0x00040930 File Offset: 0x0003EB30
	public bool UnregisterScenePreLoadEvent(SceneMgr.ScenePreLoadCallback callback, object userData)
	{
		SceneMgr.ScenePreLoadListener scenePreLoadListener = new SceneMgr.ScenePreLoadListener();
		scenePreLoadListener.SetCallback(callback);
		scenePreLoadListener.SetUserData(userData);
		return this.m_scenePreLoadListeners.Remove(scenePreLoadListener);
	}

	// Token: 0x06000EFC RID: 3836 RVA: 0x0004095D File Offset: 0x0003EB5D
	public void RegisterSceneLoadedEvent(SceneMgr.SceneLoadedCallback callback)
	{
		this.RegisterSceneLoadedEvent(callback, null);
	}

	// Token: 0x06000EFD RID: 3837 RVA: 0x00040968 File Offset: 0x0003EB68
	public void RegisterSceneLoadedEvent(SceneMgr.SceneLoadedCallback callback, object userData)
	{
		SceneMgr.SceneLoadedListener sceneLoadedListener = new SceneMgr.SceneLoadedListener();
		sceneLoadedListener.SetCallback(callback);
		sceneLoadedListener.SetUserData(userData);
		if (this.m_sceneLoadedListeners.Contains(sceneLoadedListener))
		{
			return;
		}
		this.m_sceneLoadedListeners.Add(sceneLoadedListener);
	}

	// Token: 0x06000EFE RID: 3838 RVA: 0x000409A7 File Offset: 0x0003EBA7
	public bool UnregisterSceneLoadedEvent(SceneMgr.SceneLoadedCallback callback)
	{
		return this.UnregisterSceneLoadedEvent(callback, null);
	}

	// Token: 0x06000EFF RID: 3839 RVA: 0x000409B4 File Offset: 0x0003EBB4
	public bool UnregisterSceneLoadedEvent(SceneMgr.SceneLoadedCallback callback, object userData)
	{
		SceneMgr.SceneLoadedListener sceneLoadedListener = new SceneMgr.SceneLoadedListener();
		sceneLoadedListener.SetCallback(callback);
		sceneLoadedListener.SetUserData(userData);
		return this.m_sceneLoadedListeners.Remove(sceneLoadedListener);
	}

	// Token: 0x06000F00 RID: 3840 RVA: 0x000409E4 File Offset: 0x0003EBE4
	private IEnumerator WaitThenFireSceneLoadedEvent()
	{
		yield return new WaitForSeconds(0.15f);
		this.FireSceneLoadedEvent();
		yield break;
	}

	// Token: 0x06000F01 RID: 3841 RVA: 0x00040A00 File Offset: 0x0003EC00
	private void FireScenePreUnloadEvent(Scene prevScene)
	{
		foreach (SceneMgr.ScenePreUnloadListener scenePreUnloadListener in this.m_scenePreUnloadListeners.ToArray())
		{
			scenePreUnloadListener.Fire(this.m_prevMode, prevScene);
		}
	}

	// Token: 0x06000F02 RID: 3842 RVA: 0x00040A40 File Offset: 0x0003EC40
	private void FireSceneUnloadedEvent(Scene prevScene)
	{
		foreach (SceneMgr.SceneUnloadedListener sceneUnloadedListener in this.m_sceneUnloadedListeners.ToArray())
		{
			sceneUnloadedListener.Fire(this.m_prevMode, prevScene);
		}
	}

	// Token: 0x06000F03 RID: 3843 RVA: 0x00040A80 File Offset: 0x0003EC80
	private void FireScenePreLoadEvent()
	{
		foreach (SceneMgr.ScenePreLoadListener scenePreLoadListener in this.m_scenePreLoadListeners.ToArray())
		{
			scenePreLoadListener.Fire(this.m_prevMode, this.m_mode);
		}
	}

	// Token: 0x06000F04 RID: 3844 RVA: 0x00040AC4 File Offset: 0x0003ECC4
	private void FireSceneLoadedEvent()
	{
		this.m_transitioning = false;
		foreach (SceneMgr.SceneLoadedListener sceneLoadedListener in this.m_sceneLoadedListeners.ToArray())
		{
			sceneLoadedListener.Fire(this.m_mode, this.m_scene);
		}
	}

	// Token: 0x06000F05 RID: 3845 RVA: 0x00040B10 File Offset: 0x0003ED10
	private void OnFontTableInitialized(object userData)
	{
		if (OverlayUI.Get() == null)
		{
			AssetLoader.Get().LoadUIScreen("OverlayUI", new AssetLoader.GameObjectCallback(this.OnOverlayUILoaded), null, false);
		}
		AssetLoader.Get().LoadGameObject("SplashScreen", new AssetLoader.GameObjectCallback(this.OnSplashScreenLoaded), null, false);
	}

	// Token: 0x06000F06 RID: 3846 RVA: 0x00040B69 File Offset: 0x0003ED69
	private void OnBaseUILoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("SceneMgr.OnBaseUILoaded() - FAILED to load \"{0}\"", name));
			return;
		}
	}

	// Token: 0x06000F07 RID: 3847 RVA: 0x00040B88 File Offset: 0x0003ED88
	private void OnOverlayUILoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("SceneMgr.OnOverlayUILoaded() - FAILED to load \"{0}\"", name));
			return;
		}
	}

	// Token: 0x06000F08 RID: 3848 RVA: 0x00040BA8 File Offset: 0x0003EDA8
	private void OnSplashScreenLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("SceneMgr.OnSplashScreenLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		go.GetComponent<SplashScreen>().AddFinishedListener(new SplashScreen.FinishedHandler(this.OnSplashScreenFinished));
		if (BaseUI.Get() == null)
		{
			AssetLoader.Get().LoadUIScreen("BaseUI", new AssetLoader.GameObjectCallback(this.OnBaseUILoaded), null, false);
		}
	}

	// Token: 0x06000F09 RID: 3849 RVA: 0x00040C18 File Offset: 0x0003EE18
	private void OnSplashScreenFinished()
	{
		this.LoadStartupAssets();
		this.m_StartupCamera.SetActive(false);
	}

	// Token: 0x06000F0A RID: 3850 RVA: 0x00040C2C File Offset: 0x0003EE2C
	private void LoadStartupAssets()
	{
		this.m_startupAssetLoads = 6;
		if (SoundManager.Get().GetConfig() == null)
		{
			AssetLoader.Get().LoadGameObject("SoundConfig", new AssetLoader.GameObjectCallback(this.OnSoundConfigLoaded), null, false);
		}
		else
		{
			this.m_startupAssetLoads--;
		}
		if (MusicConfig.Get() == null)
		{
			AssetLoader.Get().LoadGameObject("MusicConfig", new AssetLoader.GameObjectCallback(this.OnStartupAssetLoaded<MusicConfig>), null, false);
		}
		else
		{
			this.m_startupAssetLoads--;
		}
		if (AdventureConfig.Get() == null)
		{
			AssetLoader.Get().LoadGameObject("AdventureConfig", new AssetLoader.GameObjectCallback(this.OnStartupAssetLoaded<AdventureConfig>), null, false);
		}
		else
		{
			this.m_startupAssetLoads--;
		}
		if (CardColorSwitcher.Get() == null)
		{
			AssetLoader.Get().LoadGameObject("CardColorSwitcher", new AssetLoader.GameObjectCallback(this.OnColorSwitcherLoaded), null, false);
		}
		else
		{
			this.m_startupAssetLoads--;
		}
		if (SpecialEventVisualMgr.Get() == null)
		{
			AssetLoader.Get().LoadGameObject("SpecialEventVisualMgr", new AssetLoader.GameObjectCallback(this.OnSpecialEventVisualMgrLoaded), null, false);
		}
		else
		{
			this.m_startupAssetLoads--;
		}
		if (!this.m_textInputGUISkinLoaded)
		{
			AssetLoader.Get().LoadGameObject("TextInputGUISkin", new AssetLoader.GameObjectCallback(this.OnTextInputGUISkinLoaded), null, false);
		}
		else
		{
			this.m_startupAssetLoads--;
		}
		if (this.m_startupAssetLoads == 0)
		{
			this.OnStartupAssetFinishedLoading();
		}
	}

	// Token: 0x06000F0B RID: 3851 RVA: 0x00040DD8 File Offset: 0x0003EFD8
	private void OnColorSwitcherLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("SceneMgr.OnColorSwitcherLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		go.transform.parent = base.transform;
		this.OnStartupAssetFinishedLoading();
	}

	// Token: 0x06000F0C RID: 3852 RVA: 0x00040E1C File Offset: 0x0003F01C
	private void OnSpecialEventVisualMgrLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("SceneMgr.OnSpecialEventMgrLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		go.transform.parent = base.transform;
		this.OnStartupAssetFinishedLoading();
	}

	// Token: 0x06000F0D RID: 3853 RVA: 0x00040E60 File Offset: 0x0003F060
	private void OnSoundConfigLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("SceneMgr.OnSoundConfigLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		SoundConfig component = go.GetComponent<SoundConfig>();
		if (component == null)
		{
			Debug.LogError(string.Format("SceneMgr.OnSoundConfigLoaded() - ERROR \"{0}\" has no {1} component", name, typeof(SoundConfig)));
			return;
		}
		go.transform.parent = base.transform;
		SoundManager.Get().SetConfig(component);
		this.OnStartupAssetFinishedLoading();
	}

	// Token: 0x06000F0E RID: 3854 RVA: 0x00040EDC File Offset: 0x0003F0DC
	private void OnStartupAssetLoaded<T>(string name, GameObject go, object callbackData) where T : Component
	{
		if (go == null)
		{
			Debug.LogError(string.Format("SceneMgr.OnStartupAssetLoaded<{0}>() - FAILED to load \"{1}\"", typeof(T).Name, name));
			return;
		}
		T component = go.GetComponent<T>();
		if (component == null)
		{
			Debug.LogError(string.Format("SceneMgr.OnStartupAssetLoaded<{0}>() - ERROR \"{1}\" has no {2} component", typeof(T).Name, name, typeof(T)));
			return;
		}
		go.transform.parent = base.transform;
		this.OnStartupAssetFinishedLoading();
	}

	// Token: 0x06000F0F RID: 3855 RVA: 0x00040F70 File Offset: 0x0003F170
	private void OnTextInputGUISkinLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("SceneMgr.OnTextGUISkinLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		this.m_textInputGUISkinLoaded = true;
		GUISkinContainer component = go.GetComponent<GUISkinContainer>();
		UniversalInputManager.Get().SetGUISkin(component);
		this.OnStartupAssetFinishedLoading();
	}

	// Token: 0x06000F10 RID: 3856 RVA: 0x00040FB9 File Offset: 0x0003F1B9
	private void OnStartupAssetFinishedLoading()
	{
		if (this.IsModeRequested(SceneMgr.Mode.FATAL_ERROR))
		{
			return;
		}
		this.m_startupAssetLoads--;
		if (this.m_startupAssetLoads > 0)
		{
			return;
		}
		this.LoadBox(new AssetLoader.GameObjectCallback(this.OnBoxLoaded));
	}

	// Token: 0x06000F11 RID: 3857 RVA: 0x00040FF8 File Offset: 0x0003F1F8
	private void OnBoxLoaded(string name, GameObject screen, object callbackData)
	{
		if (screen == null)
		{
			Debug.LogError(string.Format("SceneMgr.OnBoxLoaded() - failed to load {0}", name));
			return;
		}
		if (this.IsModeRequested(SceneMgr.Mode.FATAL_ERROR))
		{
			return;
		}
		this.m_nextMode = SceneMgr.Mode.LOGIN;
	}

	// Token: 0x06000F12 RID: 3858 RVA: 0x00041038 File Offset: 0x0003F238
	private void LoadMode()
	{
		this.FireScenePreLoadEvent();
		string @string = EnumUtils.GetString<SceneMgr.Mode>(this.m_mode);
		Application.LoadLevelAdditiveAsync(@string);
	}

	// Token: 0x06000F13 RID: 3859 RVA: 0x00041060 File Offset: 0x0003F260
	private IEnumerator SwitchMode()
	{
		if (this.m_scene.IsUnloading())
		{
			yield break;
		}
		Scene prevScene = this.m_scene;
		prevScene.PreUnload();
		this.FireScenePreUnloadEvent(prevScene);
		if (LoadingScreen.Get().GetPhase() == LoadingScreen.Phase.WAITING_FOR_SCENE_UNLOAD)
		{
			Camera freezeFrameCamera = LoadingScreen.Get().GetFreezeFrameCamera();
			if (freezeFrameCamera != null)
			{
				yield return new WaitForEndOfFrame();
			}
		}
		if (this.ShouldUseSceneUnloadDelays())
		{
			if (Box.Get() != null)
			{
				while (Box.Get().HasPendingEffects())
				{
					yield return 0;
				}
			}
			else
			{
				yield return new WaitForSeconds(0.15f);
			}
		}
		this.m_scene.Unload();
		this.m_scene = null;
		this.m_sceneLoaded = false;
		this.FireSceneUnloadedEvent(prevScene);
		this.PostUnloadCleanup();
		this.LoadModeFromModeSwitch();
		yield break;
	}

	// Token: 0x06000F14 RID: 3860 RVA: 0x0004107B File Offset: 0x0003F27B
	private bool ShouldUseSceneUnloadDelays()
	{
		return this.m_prevMode != this.m_mode;
	}

	// Token: 0x06000F15 RID: 3861 RVA: 0x00041094 File Offset: 0x0003F294
	private bool ShouldUseSceneLoadDelays()
	{
		return this.m_mode != SceneMgr.Mode.LOGIN && this.m_mode != SceneMgr.Mode.HUB && this.m_mode != SceneMgr.Mode.FATAL_ERROR;
	}

	// Token: 0x06000F16 RID: 3862 RVA: 0x000410CD File Offset: 0x0003F2CD
	private void PostUnloadCleanup()
	{
		Time.captureFramerate = 0;
		if (this.m_performFullCleanup)
		{
			AssetCache.ClearAllCaches(false, true);
		}
		this.DestroyAllObjectsOnModeSwitch();
		if (this.m_performFullCleanup)
		{
			ApplicationMgr.Get().UnloadUnusedAssets();
		}
	}

	// Token: 0x06000F17 RID: 3863 RVA: 0x00041104 File Offset: 0x0003F304
	private void DestroyAllObjectsOnModeSwitch()
	{
		GameObject[] array = (GameObject[])Object.FindObjectsOfType(typeof(GameObject));
		foreach (GameObject gameObject in array)
		{
			if (this.ShouldDestroyOnModeSwitch(gameObject))
			{
				Object.DestroyImmediate(gameObject);
			}
		}
	}

	// Token: 0x06000F18 RID: 3864 RVA: 0x00041158 File Offset: 0x0003F358
	private bool ShouldDestroyOnModeSwitch(GameObject go)
	{
		return !(go == null) && !(go.transform.parent != null) && !(go == base.gameObject) && (!(PegUI.Get() != null) || !(go == PegUI.Get().gameObject)) && (!(OverlayUI.Get() != null) || !(go == OverlayUI.Get().gameObject)) && (!(Box.Get() != null) || !(go == Box.Get().gameObject) || !this.DoesModeShowBox(this.m_mode)) && !DefLoader.Get().HasDef(go) && !AssetLoader.Get().IsWaitingOnObject(go) && !(go == iTweenManager.Get().gameObject);
	}

	// Token: 0x06000F19 RID: 3865 RVA: 0x00041260 File Offset: 0x0003F460
	private void CacheModeForResume(SceneMgr.Mode mode)
	{
		if (PlatformSettings.OS != OSCategory.iOS && PlatformSettings.OS != OSCategory.Android)
		{
			return;
		}
		switch (mode)
		{
		case SceneMgr.Mode.HUB:
		case SceneMgr.Mode.FRIENDLY:
			Options.Get().SetInt(Option.LAST_SCENE_MODE, 0);
			break;
		case SceneMgr.Mode.COLLECTIONMANAGER:
		case SceneMgr.Mode.TOURNAMENT:
		case SceneMgr.Mode.DRAFT:
		case SceneMgr.Mode.CREDITS:
		case SceneMgr.Mode.ADVENTURE:
		case SceneMgr.Mode.TAVERN_BRAWL:
			Options.Get().SetInt(Option.LAST_SCENE_MODE, (int)mode);
			break;
		}
	}

	// Token: 0x06000F1A RID: 3866 RVA: 0x000412E8 File Offset: 0x0003F4E8
	private bool DoesModeShowBox(SceneMgr.Mode mode)
	{
		switch (mode)
		{
		case SceneMgr.Mode.STARTUP:
		case SceneMgr.Mode.GAMEPLAY:
			break;
		default:
			switch (mode)
			{
			case SceneMgr.Mode.FATAL_ERROR:
			case SceneMgr.Mode.RESET:
				return false;
			}
			return true;
		}
		return false;
	}

	// Token: 0x06000F1B RID: 3867 RVA: 0x00041330 File Offset: 0x0003F530
	private void LoadModeFromModeSwitch()
	{
		bool flag = this.DoesModeShowBox(this.m_prevMode);
		bool flag2 = this.DoesModeShowBox(this.m_mode);
		bool flag3 = !flag && flag2;
		if (flag3)
		{
			this.LoadBox(new AssetLoader.GameObjectCallback(this.OnBoxReloaded));
			return;
		}
		bool flag4 = flag && !flag2;
		if (flag4)
		{
			LoadingScreen.Get().SetAssetLoadStartTimestamp(this.m_boxLoadTimestamp);
			this.m_boxLoadTimestamp = 0L;
		}
		this.LoadMode();
	}

	// Token: 0x06000F1C RID: 3868 RVA: 0x000413AD File Offset: 0x0003F5AD
	private void OnBoxReloaded(string name, GameObject screen, object callbackData)
	{
		if (screen == null)
		{
			Debug.LogError(string.Format("SceneMgr.OnBoxReloaded() - failed to load {0}", name));
			return;
		}
		this.LoadMode();
	}

	// Token: 0x06000F1D RID: 3869 RVA: 0x000413D2 File Offset: 0x0003F5D2
	private void LoadBox(AssetLoader.GameObjectCallback callback)
	{
		this.m_boxLoadTimestamp = TimeUtils.BinaryStamp();
		AssetLoader.Get().LoadUIScreen("TheBox", callback, null, false);
	}

	// Token: 0x06000F1E RID: 3870 RVA: 0x000413F4 File Offset: 0x0003F5F4
	private void OnFatalError(FatalErrorMessage message, object userData)
	{
		FatalErrorMgr.Get().RemoveErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
		if (ApplicationMgr.Get().ResetOnErrorIfNecessary())
		{
			return;
		}
		this.SetNextMode(SceneMgr.Mode.FATAL_ERROR);
	}

	// Token: 0x06000F1F RID: 3871 RVA: 0x00041430 File Offset: 0x0003F630
	public void ClearCachesAndFreeMemory(int severity)
	{
		Debug.LogWarning(string.Format("Clearing Caches; this will force assets to be reloaded off disk and may be slow! {0}", severity));
		if (StoreManager.Get() != null)
		{
			StoreManager.Get().UnloadAndFreeMemory();
		}
		if (severity > 15)
		{
			if (SpellCache.Get() != null)
			{
				SpellCache.Get().Clear();
			}
			AssetCache.ClearAllCaches(true, false);
		}
		ApplicationMgr.Get().UnloadUnusedAssets();
	}

	// Token: 0x06000F20 RID: 3872 RVA: 0x0004149C File Offset: 0x0003F69C
	public void LowMemoryWarning(string msg)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float num = realtimeSinceStartup - this.m_lastMemoryWarning;
		int num2 = 0;
		if (!int.TryParse(msg, ref num2) && (num <= 2f || num > 120f))
		{
			num2 = 10;
			if (num < 0.5f)
			{
				num2 = 15;
			}
		}
		Debug.Log(string.Concat(new object[]
		{
			"receiving low memory warning ",
			num2,
			" ",
			num
		}));
		if (num2 > 5)
		{
			this.ClearCachesAndFreeMemory(num2);
			this.m_lastMemoryWarning = realtimeSinceStartup;
		}
	}

	// Token: 0x06000F21 RID: 3873 RVA: 0x00041535 File Offset: 0x0003F735
	public void FatalMobileError(string msg)
	{
		Error.AddFatal(msg);
	}

	// Token: 0x040007EE RID: 2030
	private const float SCENE_UNLOAD_DELAY = 0.15f;

	// Token: 0x040007EF RID: 2031
	private const float SCENE_LOADED_DELAY = 0.15f;

	// Token: 0x040007F0 RID: 2032
	private const int TRIM_MEMORY_RUNNING_CRITICAL = 15;

	// Token: 0x040007F1 RID: 2033
	private const int TRIM_MEMORY_RUNNING_LOW = 10;

	// Token: 0x040007F2 RID: 2034
	private const int TRIM_MEMORY_RUNNING_MODERATE = 5;

	// Token: 0x040007F3 RID: 2035
	public GameObject m_StartupCamera;

	// Token: 0x040007F4 RID: 2036
	private static SceneMgr s_instance;

	// Token: 0x040007F5 RID: 2037
	private int m_startupAssetLoads;

	// Token: 0x040007F6 RID: 2038
	private SceneMgr.Mode m_mode = SceneMgr.Mode.STARTUP;

	// Token: 0x040007F7 RID: 2039
	private SceneMgr.Mode m_nextMode;

	// Token: 0x040007F8 RID: 2040
	private SceneMgr.Mode m_prevMode;

	// Token: 0x040007F9 RID: 2041
	private bool m_reloadMode;

	// Token: 0x040007FA RID: 2042
	private Scene m_scene;

	// Token: 0x040007FB RID: 2043
	private bool m_sceneLoaded;

	// Token: 0x040007FC RID: 2044
	private bool m_transitioning;

	// Token: 0x040007FD RID: 2045
	private bool m_performFullCleanup;

	// Token: 0x040007FE RID: 2046
	private List<SceneMgr.ScenePreUnloadListener> m_scenePreUnloadListeners = new List<SceneMgr.ScenePreUnloadListener>();

	// Token: 0x040007FF RID: 2047
	private List<SceneMgr.SceneUnloadedListener> m_sceneUnloadedListeners = new List<SceneMgr.SceneUnloadedListener>();

	// Token: 0x04000800 RID: 2048
	private List<SceneMgr.ScenePreLoadListener> m_scenePreLoadListeners = new List<SceneMgr.ScenePreLoadListener>();

	// Token: 0x04000801 RID: 2049
	private List<SceneMgr.SceneLoadedListener> m_sceneLoadedListeners = new List<SceneMgr.SceneLoadedListener>();

	// Token: 0x04000802 RID: 2050
	private long m_boxLoadTimestamp;

	// Token: 0x04000803 RID: 2051
	private bool m_textInputGUISkinLoaded;

	// Token: 0x04000804 RID: 2052
	private float m_lastMemoryWarning;

	// Token: 0x02000124 RID: 292
	public enum Mode
	{
		// Token: 0x04000806 RID: 2054
		INVALID,
		// Token: 0x04000807 RID: 2055
		STARTUP,
		// Token: 0x04000808 RID: 2056
		[Description("Login")]
		LOGIN,
		// Token: 0x04000809 RID: 2057
		[Description("Hub")]
		HUB,
		// Token: 0x0400080A RID: 2058
		[Description("Gameplay")]
		GAMEPLAY,
		// Token: 0x0400080B RID: 2059
		[Description("CollectionManager")]
		COLLECTIONMANAGER,
		// Token: 0x0400080C RID: 2060
		[Description("PackOpening")]
		PACKOPENING,
		// Token: 0x0400080D RID: 2061
		[Description("Tournament")]
		TOURNAMENT,
		// Token: 0x0400080E RID: 2062
		[Description("Friendly")]
		FRIENDLY,
		// Token: 0x0400080F RID: 2063
		[Description("FatalError")]
		FATAL_ERROR,
		// Token: 0x04000810 RID: 2064
		[Description("Draft")]
		DRAFT,
		// Token: 0x04000811 RID: 2065
		[Description("Credits")]
		CREDITS,
		// Token: 0x04000812 RID: 2066
		[Description("Reset")]
		RESET,
		// Token: 0x04000813 RID: 2067
		[Description("Adventure")]
		ADVENTURE,
		// Token: 0x04000814 RID: 2068
		[Description("TavernBrawl")]
		TAVERN_BRAWL
	}

	// Token: 0x02000263 RID: 611
	// (Invoke) Token: 0x0600225A RID: 8794
	public delegate void ScenePreUnloadCallback(SceneMgr.Mode prevMode, Scene prevScene, object userData);

	// Token: 0x02000264 RID: 612
	// (Invoke) Token: 0x0600225E RID: 8798
	public delegate void SceneLoadedCallback(SceneMgr.Mode mode, Scene scene, object userData);

	// Token: 0x0200027E RID: 638
	// (Invoke) Token: 0x06002370 RID: 9072
	public delegate void SceneUnloadedCallback(SceneMgr.Mode prevMode, Scene prevScene, object userData);

	// Token: 0x0200027F RID: 639
	private class ScenePreUnloadListener : EventListener<SceneMgr.ScenePreUnloadCallback>
	{
		// Token: 0x06002374 RID: 9076 RVA: 0x000AE975 File Offset: 0x000ACB75
		public void Fire(SceneMgr.Mode prevMode, Scene prevScene)
		{
			this.m_callback(prevMode, prevScene, this.m_userData);
		}
	}

	// Token: 0x02000280 RID: 640
	private class SceneUnloadedListener : EventListener<SceneMgr.SceneUnloadedCallback>
	{
		// Token: 0x06002376 RID: 9078 RVA: 0x000AE992 File Offset: 0x000ACB92
		public void Fire(SceneMgr.Mode prevMode, Scene prevScene)
		{
			this.m_callback(prevMode, prevScene, this.m_userData);
		}
	}

	// Token: 0x02000281 RID: 641
	private class ScenePreLoadListener : EventListener<SceneMgr.ScenePreLoadCallback>
	{
		// Token: 0x06002378 RID: 9080 RVA: 0x000AE9AF File Offset: 0x000ACBAF
		public void Fire(SceneMgr.Mode prevMode, SceneMgr.Mode mode)
		{
			this.m_callback(prevMode, mode, this.m_userData);
		}
	}

	// Token: 0x02000282 RID: 642
	// (Invoke) Token: 0x0600237A RID: 9082
	public delegate void ScenePreLoadCallback(SceneMgr.Mode prevMode, SceneMgr.Mode mode, object userData);

	// Token: 0x02000283 RID: 643
	private class SceneLoadedListener : EventListener<SceneMgr.SceneLoadedCallback>
	{
		// Token: 0x0600237E RID: 9086 RVA: 0x000AE9CC File Offset: 0x000ACBCC
		public void Fire(SceneMgr.Mode mode, Scene scene)
		{
			this.m_callback(mode, scene, this.m_userData);
		}
	}
}
