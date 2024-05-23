using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001A1 RID: 417
public class LoadingScreen : MonoBehaviour
{
	// Token: 0x06001B95 RID: 7061 RVA: 0x000822B4 File Offset: 0x000804B4
	private void Awake()
	{
		LoadingScreen.s_instance = this;
		this.InitializeFxCamera();
		ApplicationMgr.Get().WillReset += new Action(this.WillReset);
	}

	// Token: 0x06001B96 RID: 7062 RVA: 0x000822E3 File Offset: 0x000804E3
	private void OnDestroy()
	{
		ApplicationMgr.Get().WillReset -= new Action(this.WillReset);
		LoadingScreen.s_instance = null;
	}

	// Token: 0x06001B97 RID: 7063 RVA: 0x00082301 File Offset: 0x00080501
	private void Start()
	{
		FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
		this.RegisterSceneListeners();
	}

	// Token: 0x06001B98 RID: 7064 RVA: 0x00082320 File Offset: 0x00080520
	public static LoadingScreen Get()
	{
		return LoadingScreen.s_instance;
	}

	// Token: 0x06001B99 RID: 7065 RVA: 0x00082327 File Offset: 0x00080527
	public Camera GetFxCamera()
	{
		return this.m_fxCamera;
	}

	// Token: 0x06001B9A RID: 7066 RVA: 0x0008232F File Offset: 0x0008052F
	public CameraFade GetCameraFade()
	{
		return base.GetComponent<CameraFade>();
	}

	// Token: 0x06001B9B RID: 7067 RVA: 0x00082338 File Offset: 0x00080538
	public void RegisterSceneListeners()
	{
		SceneMgr.Get().RegisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
		SceneMgr.Get().RegisterSceneUnloadedEvent(new SceneMgr.SceneUnloadedCallback(this.OnSceneUnloaded));
		SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
	}

	// Token: 0x06001B9C RID: 7068 RVA: 0x00082388 File Offset: 0x00080588
	public void UnregisterSceneListeners()
	{
		SceneMgr.Get().UnregisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
		SceneMgr.Get().UnregisterSceneUnloadedEvent(new SceneMgr.SceneUnloadedCallback(this.OnSceneUnloaded));
		SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
	}

	// Token: 0x06001B9D RID: 7069 RVA: 0x000823DA File Offset: 0x000805DA
	public static bool DoesShowLoadingScreen(SceneMgr.Mode prevMode, SceneMgr.Mode nextMode)
	{
		return prevMode == SceneMgr.Mode.GAMEPLAY || nextMode == SceneMgr.Mode.GAMEPLAY;
	}

	// Token: 0x06001B9E RID: 7070 RVA: 0x000823EF File Offset: 0x000805EF
	private void WillReset()
	{
		FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
	}

	// Token: 0x06001B9F RID: 7071 RVA: 0x00082408 File Offset: 0x00080608
	public LoadingScreen.Phase GetPhase()
	{
		return this.m_phase;
	}

	// Token: 0x06001BA0 RID: 7072 RVA: 0x00082410 File Offset: 0x00080610
	public bool IsTransitioning()
	{
		return this.m_phase != LoadingScreen.Phase.INVALID;
	}

	// Token: 0x06001BA1 RID: 7073 RVA: 0x00082420 File Offset: 0x00080620
	public bool IsWaiting()
	{
		switch (this.m_phase)
		{
		case LoadingScreen.Phase.WAITING_FOR_SCENE_UNLOAD:
		case LoadingScreen.Phase.WAITING_FOR_SCENE_LOAD:
		case LoadingScreen.Phase.WAITING_FOR_BLOCKERS:
			return true;
		default:
			return false;
		}
	}

	// Token: 0x06001BA2 RID: 7074 RVA: 0x00082450 File Offset: 0x00080650
	public bool IsFadingOut()
	{
		return this.m_phase == LoadingScreen.Phase.FADING_OUT;
	}

	// Token: 0x06001BA3 RID: 7075 RVA: 0x0008245B File Offset: 0x0008065B
	public bool IsFadingIn()
	{
		return this.m_phase == LoadingScreen.Phase.FADING_IN;
	}

	// Token: 0x06001BA4 RID: 7076 RVA: 0x00082466 File Offset: 0x00080666
	public bool IsFading()
	{
		return this.IsFadingOut() || this.IsFadingIn();
	}

	// Token: 0x06001BA5 RID: 7077 RVA: 0x00082483 File Offset: 0x00080683
	public bool IsPreviousSceneActive()
	{
		return this.m_previousSceneActive;
	}

	// Token: 0x06001BA6 RID: 7078 RVA: 0x0008248B File Offset: 0x0008068B
	public bool IsTransitionEnabled()
	{
		return this.m_transitionParams.IsEnabled();
	}

	// Token: 0x06001BA7 RID: 7079 RVA: 0x00082498 File Offset: 0x00080698
	public void EnableTransition(bool enable)
	{
		this.m_transitionParams.Enable(enable);
	}

	// Token: 0x06001BA8 RID: 7080 RVA: 0x000824A6 File Offset: 0x000806A6
	public void AddTransitionObject(GameObject go)
	{
		this.m_transitionParams.AddObject(go);
	}

	// Token: 0x06001BA9 RID: 7081 RVA: 0x000824B4 File Offset: 0x000806B4
	public void AddTransitionObject(Component c)
	{
		this.m_transitionParams.AddObject(c.gameObject);
	}

	// Token: 0x06001BAA RID: 7082 RVA: 0x000824C7 File Offset: 0x000806C7
	public void AddTransitionBlocker()
	{
		this.m_transitionParams.AddBlocker();
	}

	// Token: 0x06001BAB RID: 7083 RVA: 0x000824D4 File Offset: 0x000806D4
	public void AddTransitionBlocker(int count)
	{
		this.m_transitionParams.AddBlocker(count);
	}

	// Token: 0x06001BAC RID: 7084 RVA: 0x000824E2 File Offset: 0x000806E2
	public Camera GetFreezeFrameCamera()
	{
		return this.m_transitionParams.GetFreezeFrameCamera();
	}

	// Token: 0x06001BAD RID: 7085 RVA: 0x000824EF File Offset: 0x000806EF
	public void SetFreezeFrameCamera(Camera camera)
	{
		this.m_transitionParams.SetFreezeFrameCamera(camera);
	}

	// Token: 0x06001BAE RID: 7086 RVA: 0x000824FD File Offset: 0x000806FD
	public AudioListener GetTransitionAudioListener()
	{
		return this.m_transitionParams.GetAudioListener();
	}

	// Token: 0x06001BAF RID: 7087 RVA: 0x0008250A File Offset: 0x0008070A
	public void SetTransitionAudioListener(AudioListener listener)
	{
		Log.LoadingScreen.Print("LoadingScreen.SetTransitionAudioListener() - {0}", new object[]
		{
			listener
		});
		this.m_transitionParams.SetAudioListener(listener);
	}

	// Token: 0x06001BB0 RID: 7088 RVA: 0x00082531 File Offset: 0x00080731
	public void EnableFadeOut(bool enable)
	{
		this.m_transitionParams.EnableFadeOut(enable);
	}

	// Token: 0x06001BB1 RID: 7089 RVA: 0x0008253F File Offset: 0x0008073F
	public void EnableFadeIn(bool enable)
	{
		this.m_transitionParams.EnableFadeIn(enable);
	}

	// Token: 0x06001BB2 RID: 7090 RVA: 0x0008254D File Offset: 0x0008074D
	public Color GetFadeColor()
	{
		return this.m_transitionParams.GetFadeColor();
	}

	// Token: 0x06001BB3 RID: 7091 RVA: 0x0008255A File Offset: 0x0008075A
	public void SetFadeColor(Color color)
	{
		this.m_transitionParams.SetFadeColor(color);
	}

	// Token: 0x06001BB4 RID: 7092 RVA: 0x00082568 File Offset: 0x00080768
	public void NotifyTransitionBlockerComplete()
	{
		if (this.m_prevTransitionParams == null)
		{
			return;
		}
		this.m_prevTransitionParams.RemoveBlocker();
		this.TransitionIfPossible();
	}

	// Token: 0x06001BB5 RID: 7093 RVA: 0x00082588 File Offset: 0x00080788
	public void NotifyTransitionBlockerComplete(int count)
	{
		if (this.m_prevTransitionParams == null)
		{
			return;
		}
		this.m_prevTransitionParams.RemoveBlocker(count);
		this.TransitionIfPossible();
	}

	// Token: 0x06001BB6 RID: 7094 RVA: 0x000825A9 File Offset: 0x000807A9
	public void NotifyMainSceneObjectAwoke(GameObject mainObject)
	{
		if (!this.IsPreviousSceneActive())
		{
			return;
		}
		this.DisableTransitionUnfriendlyStuff(mainObject);
	}

	// Token: 0x06001BB7 RID: 7095 RVA: 0x000825BE File Offset: 0x000807BE
	public long GetAssetLoadStartTimestamp()
	{
		return this.m_assetLoadStartTimestamp;
	}

	// Token: 0x06001BB8 RID: 7096 RVA: 0x000825C6 File Offset: 0x000807C6
	public void SetAssetLoadStartTimestamp(long timestamp)
	{
		this.m_assetLoadStartTimestamp = Math.Min(this.m_assetLoadStartTimestamp, timestamp);
		Log.LoadingScreen.Print("LoadingScreen.SetAssetLoadStartTimestamp() - m_assetLoadStartTimestamp={0}", new object[]
		{
			this.m_assetLoadStartTimestamp
		});
	}

	// Token: 0x06001BB9 RID: 7097 RVA: 0x000825FD File Offset: 0x000807FD
	public bool RegisterPreviousSceneDestroyedListener(LoadingScreen.PreviousSceneDestroyedCallback callback)
	{
		return this.RegisterPreviousSceneDestroyedListener(callback, null);
	}

	// Token: 0x06001BBA RID: 7098 RVA: 0x00082608 File Offset: 0x00080808
	public bool RegisterPreviousSceneDestroyedListener(LoadingScreen.PreviousSceneDestroyedCallback callback, object userData)
	{
		LoadingScreen.PreviousSceneDestroyedListener previousSceneDestroyedListener = new LoadingScreen.PreviousSceneDestroyedListener();
		previousSceneDestroyedListener.SetCallback(callback);
		previousSceneDestroyedListener.SetUserData(userData);
		if (this.m_prevSceneDestroyedListeners.Contains(previousSceneDestroyedListener))
		{
			return false;
		}
		this.m_prevSceneDestroyedListeners.Add(previousSceneDestroyedListener);
		return true;
	}

	// Token: 0x06001BBB RID: 7099 RVA: 0x00082649 File Offset: 0x00080849
	public bool UnregisterPreviousSceneDestroyedListener(LoadingScreen.PreviousSceneDestroyedCallback callback)
	{
		return this.UnregisterPreviousSceneDestroyedListener(callback, null);
	}

	// Token: 0x06001BBC RID: 7100 RVA: 0x00082654 File Offset: 0x00080854
	public bool UnregisterPreviousSceneDestroyedListener(LoadingScreen.PreviousSceneDestroyedCallback callback, object userData)
	{
		LoadingScreen.PreviousSceneDestroyedListener previousSceneDestroyedListener = new LoadingScreen.PreviousSceneDestroyedListener();
		previousSceneDestroyedListener.SetCallback(callback);
		previousSceneDestroyedListener.SetUserData(userData);
		return this.m_prevSceneDestroyedListeners.Remove(previousSceneDestroyedListener);
	}

	// Token: 0x06001BBD RID: 7101 RVA: 0x00082681 File Offset: 0x00080881
	public bool RegisterFinishedTransitionListener(LoadingScreen.FinishedTransitionCallback callback)
	{
		return this.RegisterFinishedTransitionListener(callback, null);
	}

	// Token: 0x06001BBE RID: 7102 RVA: 0x0008268C File Offset: 0x0008088C
	public bool RegisterFinishedTransitionListener(LoadingScreen.FinishedTransitionCallback callback, object userData)
	{
		LoadingScreen.FinishedTransitionListener finishedTransitionListener = new LoadingScreen.FinishedTransitionListener();
		finishedTransitionListener.SetCallback(callback);
		finishedTransitionListener.SetUserData(userData);
		if (this.m_finishedTransitionListeners.Contains(finishedTransitionListener))
		{
			return false;
		}
		this.m_finishedTransitionListeners.Add(finishedTransitionListener);
		return true;
	}

	// Token: 0x06001BBF RID: 7103 RVA: 0x000826CD File Offset: 0x000808CD
	public bool UnregisterFinishedTransitionListener(LoadingScreen.FinishedTransitionCallback callback)
	{
		return this.UnregisterFinishedTransitionListener(callback, null);
	}

	// Token: 0x06001BC0 RID: 7104 RVA: 0x000826D8 File Offset: 0x000808D8
	public bool UnregisterFinishedTransitionListener(LoadingScreen.FinishedTransitionCallback callback, object userData)
	{
		LoadingScreen.FinishedTransitionListener finishedTransitionListener = new LoadingScreen.FinishedTransitionListener();
		finishedTransitionListener.SetCallback(callback);
		finishedTransitionListener.SetUserData(userData);
		return this.m_finishedTransitionListeners.Remove(finishedTransitionListener);
	}

	// Token: 0x06001BC1 RID: 7105 RVA: 0x00082705 File Offset: 0x00080905
	private void OnFatalError(FatalErrorMessage message, object userData)
	{
		FatalErrorMgr.Get().RemoveErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
		this.EnableTransition(false);
	}

	// Token: 0x06001BC2 RID: 7106 RVA: 0x00082728 File Offset: 0x00080928
	private void OnScenePreUnload(SceneMgr.Mode prevMode, Scene prevScene, object userData)
	{
		Log.LoadingScreen.Print("LoadingScreen.OnScenePreUnload() - prevMode={0} nextMode={1} m_phase={2}", new object[]
		{
			prevMode,
			SceneMgr.Get().GetMode(),
			this.m_phase
		});
		if (!LoadingScreen.DoesShowLoadingScreen(prevMode, SceneMgr.Get().GetMode()))
		{
			this.CutoffTransition();
			return;
		}
		if (!this.m_transitionParams.IsEnabled())
		{
			this.CutoffTransition();
			return;
		}
		if (this.IsTransitioning())
		{
			this.DoInterruptionCleanUp();
		}
		this.m_assetLoadNextStartTimestamp = TimeUtils.BinaryStamp();
		if (this.IsTransitioning())
		{
			this.FireFinishedTransitionListeners(true);
			if (this.IsPreviousSceneActive())
			{
				return;
			}
		}
		this.m_phase = LoadingScreen.Phase.WAITING_FOR_SCENE_UNLOAD;
		this.m_previousSceneActive = true;
		this.ShowFreezeFrame(this.m_transitionParams.GetFreezeFrameCamera());
	}

	// Token: 0x06001BC3 RID: 7107 RVA: 0x00082800 File Offset: 0x00080A00
	private void OnSceneUnloaded(SceneMgr.Mode prevMode, Scene prevScene, object userData)
	{
		Log.LoadingScreen.Print("LoadingScreen.OnSceneUnloaded() - prevMode={0} nextMode={1} m_phase={2}", new object[]
		{
			prevMode,
			SceneMgr.Get().GetMode(),
			this.m_phase
		});
		if (this.m_phase != LoadingScreen.Phase.WAITING_FOR_SCENE_UNLOAD)
		{
			return;
		}
		this.m_assetLoadEndTimestamp = this.m_assetLoadNextStartTimestamp;
		Log.LoadingScreen.Print("LoadingScreen.OnSceneUnloaded() - m_assetLoadEndTimestamp={0}", new object[]
		{
			this.m_assetLoadEndTimestamp
		});
		this.m_phase = LoadingScreen.Phase.WAITING_FOR_SCENE_LOAD;
		this.m_prevTransitionParams = this.m_transitionParams;
		this.m_transitionParams = new LoadingScreen.TransitionParams();
		this.m_transitionParams.ClearPreviousAssets = (prevMode != SceneMgr.Get().GetMode());
		this.m_prevTransitionParams.AutoAddObjects();
		this.m_prevTransitionParams.FixupCameras(this.m_fxCamera);
		this.m_prevTransitionParams.PreserveObjects(base.transform);
		this.m_originalPosX = base.transform.position.x;
		TransformUtil.SetPosX(base.gameObject, 5000f);
	}

	// Token: 0x06001BC4 RID: 7108 RVA: 0x00082918 File Offset: 0x00080B18
	private void OnSceneLoaded(SceneMgr.Mode mode, Scene scene, object userData)
	{
		Log.LoadingScreen.Print("LoadingScreen.OnSceneLoaded() - prevMode={0} currMode={1}", new object[]
		{
			SceneMgr.Get().GetPrevMode(),
			mode
		});
		if (mode == SceneMgr.Mode.FATAL_ERROR)
		{
			Log.LoadingScreen.Print("LoadingScreen.OnSceneLoaded() - calling CutoffTransition()", new object[]
			{
				mode
			});
			this.CutoffTransition();
			return;
		}
		if (SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.STARTUP)
		{
			this.m_assetLoadStartTimestamp = TimeUtils.BinaryStamp();
			Log.LoadingScreen.Print("LoadingScreen.OnSceneLoaded() - m_assetLoadStartTimestamp={0}", new object[]
			{
				this.m_assetLoadStartTimestamp
			});
		}
		if (this.m_phase != LoadingScreen.Phase.WAITING_FOR_SCENE_LOAD)
		{
			Log.LoadingScreen.Print("LoadingScreen.OnSceneLoaded() - END - {0} != Phase.WAITING_FOR_SCENE_LOAD", new object[]
			{
				this.m_phase
			});
			return;
		}
		this.m_phase = LoadingScreen.Phase.WAITING_FOR_BLOCKERS;
		if (!this.TransitionIfPossible())
		{
			return;
		}
	}

	// Token: 0x06001BC5 RID: 7109 RVA: 0x00082A04 File Offset: 0x00080C04
	private bool TransitionIfPossible()
	{
		if (this.m_prevTransitionParams.GetBlockerCount() > 0)
		{
			return false;
		}
		base.StartCoroutine("HackWaitThenStartTransitionEffects");
		return true;
	}

	// Token: 0x06001BC6 RID: 7110 RVA: 0x00082A34 File Offset: 0x00080C34
	private IEnumerator HackWaitThenStartTransitionEffects()
	{
		Log.LoadingScreen.Print("LoadingScreen.HackWaitThenStartTransitionEffects() - START", new object[0]);
		yield return new WaitForEndOfFrame();
		if (this.m_phase != LoadingScreen.Phase.WAITING_FOR_BLOCKERS)
		{
			Log.LoadingScreen.Print("LoadingScreen.HackWaitThenStartTransitionEffects() - END - {0} != Phase.WAITING_FOR_BLOCKERS", new object[]
			{
				this.m_phase
			});
			yield break;
		}
		this.FadeOut();
		yield break;
	}

	// Token: 0x06001BC7 RID: 7111 RVA: 0x00082A50 File Offset: 0x00080C50
	private void FirePreviousSceneDestroyedListeners()
	{
		LoadingScreen.PreviousSceneDestroyedListener[] array = this.m_prevSceneDestroyedListeners.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Fire();
		}
	}

	// Token: 0x06001BC8 RID: 7112 RVA: 0x00082A88 File Offset: 0x00080C88
	private void FireFinishedTransitionListeners(bool cutoff)
	{
		LoadingScreen.FinishedTransitionListener[] array = this.m_finishedTransitionListeners.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Fire(cutoff);
		}
	}

	// Token: 0x06001BC9 RID: 7113 RVA: 0x00082AC0 File Offset: 0x00080CC0
	private void FadeOut()
	{
		Log.LoadingScreen.Print("LoadingScreen.FadeOut()", new object[0]);
		this.m_phase = LoadingScreen.Phase.FADING_OUT;
		if (!this.m_prevTransitionParams.IsFadeOutEnabled())
		{
			this.OnFadeOutComplete();
			return;
		}
		CameraFade cameraFade = base.GetComponent<CameraFade>();
		if (cameraFade == null)
		{
			Debug.LogError("LoadingScreen FadeOut(): Failed to find CameraFade component");
			return;
		}
		cameraFade.m_Color = this.m_prevTransitionParams.GetFadeColor();
		Action<object> action = delegate(object amount)
		{
			cameraFade.m_Fade = (float)amount;
		};
		Hashtable args = iTween.Hash(new object[]
		{
			"time",
			this.m_FadeOutSec,
			"from",
			cameraFade.m_Fade,
			"to",
			1f,
			"onupdate",
			action,
			"onupdatetarget",
			base.gameObject,
			"oncomplete",
			"OnFadeOutComplete",
			"oncompletetarget",
			base.gameObject,
			"name",
			"Fade"
		});
		iTween.ValueTo(base.gameObject, args);
	}

	// Token: 0x06001BCA RID: 7114 RVA: 0x00082C05 File Offset: 0x00080E05
	private void OnFadeOutComplete()
	{
		Log.LoadingScreen.Print("LoadingScreen.OnFadeOutComplete()", new object[0]);
		this.FinishPreviousScene();
		this.FadeIn();
	}

	// Token: 0x06001BCB RID: 7115 RVA: 0x00082C28 File Offset: 0x00080E28
	private void FadeIn()
	{
		Log.LoadingScreen.Print("LoadingScreen.FadeIn()", new object[0]);
		this.m_phase = LoadingScreen.Phase.FADING_IN;
		if (!this.m_prevTransitionParams.IsFadeInEnabled())
		{
			this.OnFadeInComplete();
			return;
		}
		CameraFade cameraFade = base.GetComponent<CameraFade>();
		if (cameraFade == null)
		{
			Debug.LogError("LoadingScreen FadeIn(): Failed to find CameraFade component");
			return;
		}
		cameraFade.m_Color = this.m_prevTransitionParams.GetFadeColor();
		Action<object> action = delegate(object amount)
		{
			cameraFade.m_Fade = (float)amount;
		};
		action.Invoke(1f);
		Hashtable args = iTween.Hash(new object[]
		{
			"time",
			this.m_FadeInSec,
			"from",
			1f,
			"to",
			0f,
			"onupdate",
			action,
			"onupdatetarget",
			base.gameObject,
			"oncomplete",
			"OnFadeInComplete",
			"oncompletetarget",
			base.gameObject,
			"name",
			"Fade"
		});
		iTween.ValueTo(base.gameObject, args);
	}

	// Token: 0x06001BCC RID: 7116 RVA: 0x00082D78 File Offset: 0x00080F78
	private void OnFadeInComplete()
	{
		Log.LoadingScreen.Print("LoadingScreen.OnFadeInComplete()", new object[0]);
		this.FinishFxCamera();
		this.m_prevTransitionParams = null;
		this.m_phase = LoadingScreen.Phase.INVALID;
		this.FireFinishedTransitionListeners(false);
	}

	// Token: 0x06001BCD RID: 7117 RVA: 0x00082DB5 File Offset: 0x00080FB5
	private void InitializeFxCamera()
	{
		this.m_fxCamera = base.GetComponent<Camera>();
	}

	// Token: 0x06001BCE RID: 7118 RVA: 0x00082DC4 File Offset: 0x00080FC4
	private void FinishFxCamera()
	{
		CameraFade cameraFade = base.GetComponent<CameraFade>();
		if (cameraFade == null)
		{
			Debug.LogError("LoadingScreen.FinishFxCamera(): Failed to find CameraFade component");
			return;
		}
		if (cameraFade.m_Fade > 0f)
		{
			Action<object> action = delegate(object amount)
			{
				cameraFade.m_Fade = (float)amount;
			};
			Hashtable args = iTween.Hash(new object[]
			{
				"time",
				0.3f,
				"from",
				cameraFade.m_Fade,
				"to",
				0f,
				"onupdate",
				action,
				"onupdatetarget",
				base.gameObject,
				"oncompletetarget",
				base.gameObject,
				"delay",
				0.5f,
				"name",
				"Fade"
			});
			iTween.ValueTo(base.gameObject, args);
		}
	}

	// Token: 0x06001BCF RID: 7119 RVA: 0x00082EDC File Offset: 0x000810DC
	private FullScreenEffects GetFullScreenEffects(Camera camera)
	{
		FullScreenEffects component = camera.GetComponent<FullScreenEffects>();
		if (component != null)
		{
			return component;
		}
		return camera.gameObject.AddComponent<FullScreenEffects>();
	}

	// Token: 0x06001BD0 RID: 7120 RVA: 0x00082F0C File Offset: 0x0008110C
	private void ShowFreezeFrame(Camera camera)
	{
		if (camera == null)
		{
			return;
		}
		FullScreenEffects fullScreenEffects = this.GetFullScreenEffects(camera);
		fullScreenEffects.Freeze();
	}

	// Token: 0x06001BD1 RID: 7121 RVA: 0x00082F34 File Offset: 0x00081134
	private void CutoffTransition()
	{
		if (!this.IsTransitioning())
		{
			this.m_transitionParams = new LoadingScreen.TransitionParams();
			return;
		}
		this.StopFading();
		this.FinishPreviousScene();
		this.FinishFxCamera();
		this.m_prevTransitionParams = null;
		this.m_transitionParams = new LoadingScreen.TransitionParams();
		this.m_phase = LoadingScreen.Phase.INVALID;
		this.FireFinishedTransitionListeners(true);
	}

	// Token: 0x06001BD2 RID: 7122 RVA: 0x00082F8A File Offset: 0x0008118A
	private void StopFading()
	{
		iTween.Stop(base.gameObject);
	}

	// Token: 0x06001BD3 RID: 7123 RVA: 0x00082F98 File Offset: 0x00081198
	private void DoInterruptionCleanUp()
	{
		bool flag = this.IsPreviousSceneActive();
		Log.LoadingScreen.Print("LoadingScreen.DoInterruptionCleanUp() - m_phase={0} previousSceneActive={1}", new object[]
		{
			this.m_phase,
			flag
		});
		if (this.m_phase == LoadingScreen.Phase.WAITING_FOR_BLOCKERS)
		{
			base.StopCoroutine("HackWaitThenStartTransitionEffects");
		}
		if (this.IsFading())
		{
			this.StopFading();
			if (this.IsFadingIn())
			{
				this.m_prevTransitionParams = null;
			}
		}
		if (flag)
		{
			long assetLoadNextStartTimestamp = this.m_assetLoadNextStartTimestamp;
			long endTimestamp = TimeUtils.BinaryStamp();
			this.ClearAssets(assetLoadNextStartTimestamp, endTimestamp);
			this.m_transitionUnfriendlyData.Clear();
			this.m_transitionParams = new LoadingScreen.TransitionParams();
			this.m_phase = LoadingScreen.Phase.WAITING_FOR_SCENE_LOAD;
		}
	}

	// Token: 0x06001BD4 RID: 7124 RVA: 0x0008304C File Offset: 0x0008124C
	private void FinishPreviousScene()
	{
		Log.LoadingScreen.Print("LoadingScreen.FinishPreviousScene()", new object[0]);
		if (this.m_prevTransitionParams != null)
		{
			this.m_prevTransitionParams.DestroyObjects();
			TransformUtil.SetPosX(base.gameObject, this.m_originalPosX);
		}
		if (this.m_transitionParams.ClearPreviousAssets)
		{
			this.ClearPreviousSceneAssets();
		}
		this.m_transitionUnfriendlyData.Restore();
		this.m_transitionUnfriendlyData.Clear();
		this.m_previousSceneActive = false;
		this.FirePreviousSceneDestroyedListeners();
	}

	// Token: 0x06001BD5 RID: 7125 RVA: 0x000830D0 File Offset: 0x000812D0
	private void ClearPreviousSceneAssets()
	{
		Log.LoadingScreen.Print("LoadingScreen.ClearPreviousSceneAssets() - START m_assetLoadStartTimestamp={0} m_assetLoadEndTimestamp={1}", new object[]
		{
			this.m_assetLoadStartTimestamp,
			this.m_assetLoadEndTimestamp
		});
		this.ClearAssets(this.m_assetLoadStartTimestamp, this.m_assetLoadEndTimestamp);
		this.m_assetLoadStartTimestamp = this.m_assetLoadNextStartTimestamp;
		this.m_assetLoadEndTimestamp = 0L;
		this.m_assetLoadNextStartTimestamp = 0L;
		Log.LoadingScreen.Print("LoadingScreen.ClearPreviousSceneAssets() - END m_assetLoadStartTimestamp={0} m_assetLoadEndTimestamp={1}", new object[]
		{
			this.m_assetLoadStartTimestamp,
			this.m_assetLoadEndTimestamp
		});
	}

	// Token: 0x06001BD6 RID: 7126 RVA: 0x00083170 File Offset: 0x00081370
	private void ClearAssets(long startTimestamp, long endTimestamp)
	{
		Log.LoadingScreen.Print("LoadingScreen.ClearAssets() - START startTimestamp={0} endTimestamp={1} diff={2}", new object[]
		{
			startTimestamp,
			endTimestamp,
			endTimestamp - startTimestamp
		});
		AssetCache.ClearAllCachesBetween(startTimestamp, endTimestamp);
		ApplicationMgr.Get().UnloadUnusedAssets();
	}

	// Token: 0x06001BD7 RID: 7127 RVA: 0x000831C0 File Offset: 0x000813C0
	private void DisableTransitionUnfriendlyStuff(GameObject mainObject)
	{
		Log.LoadingScreen.Print("LoadingScreen.DisableTransitionUnfriendlyStuff() - {0}", new object[]
		{
			mainObject
		});
		AudioListener[] componentsInChildren = base.GetComponentsInChildren<AudioListener>();
		bool flag = false;
		foreach (AudioListener audioListener in componentsInChildren)
		{
			flag |= (audioListener != null && audioListener.enabled);
		}
		if (flag)
		{
			AudioListener componentInChildren = mainObject.GetComponentInChildren<AudioListener>();
			this.m_transitionUnfriendlyData.SetAudioListener(componentInChildren);
		}
		Light[] componentsInChildren2 = mainObject.GetComponentsInChildren<Light>();
		this.m_transitionUnfriendlyData.AddLights(componentsInChildren2);
	}

	// Token: 0x04000E45 RID: 3653
	private const float MIDDLE_OF_NOWHERE_X = 5000f;

	// Token: 0x04000E46 RID: 3654
	public float m_FadeOutSec = 1f;

	// Token: 0x04000E47 RID: 3655
	public iTween.EaseType m_FadeOutEaseType = iTween.EaseType.linear;

	// Token: 0x04000E48 RID: 3656
	public float m_FadeInSec = 1f;

	// Token: 0x04000E49 RID: 3657
	public iTween.EaseType m_FadeInEaseType = iTween.EaseType.linear;

	// Token: 0x04000E4A RID: 3658
	private static LoadingScreen s_instance;

	// Token: 0x04000E4B RID: 3659
	private LoadingScreen.Phase m_phase;

	// Token: 0x04000E4C RID: 3660
	private bool m_previousSceneActive;

	// Token: 0x04000E4D RID: 3661
	private LoadingScreen.TransitionParams m_prevTransitionParams;

	// Token: 0x04000E4E RID: 3662
	private LoadingScreen.TransitionParams m_transitionParams = new LoadingScreen.TransitionParams();

	// Token: 0x04000E4F RID: 3663
	private LoadingScreen.TransitionUnfriendlyData m_transitionUnfriendlyData = new LoadingScreen.TransitionUnfriendlyData();

	// Token: 0x04000E50 RID: 3664
	private Camera m_fxCamera;

	// Token: 0x04000E51 RID: 3665
	private List<LoadingScreen.PreviousSceneDestroyedListener> m_prevSceneDestroyedListeners = new List<LoadingScreen.PreviousSceneDestroyedListener>();

	// Token: 0x04000E52 RID: 3666
	private List<LoadingScreen.FinishedTransitionListener> m_finishedTransitionListeners = new List<LoadingScreen.FinishedTransitionListener>();

	// Token: 0x04000E53 RID: 3667
	private float m_originalPosX;

	// Token: 0x04000E54 RID: 3668
	private long m_assetLoadStartTimestamp;

	// Token: 0x04000E55 RID: 3669
	private long m_assetLoadEndTimestamp;

	// Token: 0x04000E56 RID: 3670
	private long m_assetLoadNextStartTimestamp;

	// Token: 0x02000265 RID: 613
	// (Invoke) Token: 0x06002262 RID: 8802
	public delegate void PreviousSceneDestroyedCallback(object userData);

	// Token: 0x02000289 RID: 649
	public enum Phase
	{
		// Token: 0x040014BA RID: 5306
		INVALID,
		// Token: 0x040014BB RID: 5307
		WAITING_FOR_SCENE_UNLOAD,
		// Token: 0x040014BC RID: 5308
		WAITING_FOR_SCENE_LOAD,
		// Token: 0x040014BD RID: 5309
		WAITING_FOR_BLOCKERS,
		// Token: 0x040014BE RID: 5310
		FADING_OUT,
		// Token: 0x040014BF RID: 5311
		FADING_IN
	}

	// Token: 0x0200039B RID: 923
	// (Invoke) Token: 0x06003071 RID: 12401
	public delegate void FinishedTransitionCallback(bool cutoff, object userData);

	// Token: 0x02000682 RID: 1666
	private class PreviousSceneDestroyedListener : EventListener<LoadingScreen.PreviousSceneDestroyedCallback>
	{
		// Token: 0x060046B6 RID: 18102 RVA: 0x00153DF8 File Offset: 0x00151FF8
		public void Fire()
		{
			this.m_callback(this.m_userData);
		}
	}

	// Token: 0x02000683 RID: 1667
	private class FinishedTransitionListener : EventListener<LoadingScreen.FinishedTransitionCallback>
	{
		// Token: 0x060046B8 RID: 18104 RVA: 0x00153E13 File Offset: 0x00152013
		public void Fire(bool cutoff)
		{
			this.m_callback(cutoff, this.m_userData);
		}
	}

	// Token: 0x02000684 RID: 1668
	private class TransitionParams
	{
		// Token: 0x060046BA RID: 18106 RVA: 0x00153E83 File Offset: 0x00152083
		public bool IsEnabled()
		{
			return this.m_enabled;
		}

		// Token: 0x060046BB RID: 18107 RVA: 0x00153E8B File Offset: 0x0015208B
		public void Enable(bool enable)
		{
			this.m_enabled = enable;
		}

		// Token: 0x060046BC RID: 18108 RVA: 0x00153E94 File Offset: 0x00152094
		public void AddObject(Component c)
		{
			if (c == null)
			{
				return;
			}
			this.AddObject(c.gameObject);
		}

		// Token: 0x060046BD RID: 18109 RVA: 0x00153EB0 File Offset: 0x001520B0
		public void AddObject(GameObject go)
		{
			if (go == null)
			{
				return;
			}
			Transform transform = go.transform;
			while (transform != null)
			{
				if (this.m_objects.Contains(transform.gameObject))
				{
					return;
				}
				transform = transform.parent;
			}
			Camera[] componentsInChildren = go.GetComponentsInChildren<Camera>();
			foreach (Camera camera in componentsInChildren)
			{
				if (!this.m_cameras.Contains(camera))
				{
					this.m_cameras.Add(camera);
				}
			}
			this.m_objects.Add(go);
		}

		// Token: 0x060046BE RID: 18110 RVA: 0x00153F4F File Offset: 0x0015214F
		public void AddBlocker()
		{
			this.m_blockerCount++;
		}

		// Token: 0x060046BF RID: 18111 RVA: 0x00153F5F File Offset: 0x0015215F
		public void AddBlocker(int count)
		{
			this.m_blockerCount += count;
		}

		// Token: 0x060046C0 RID: 18112 RVA: 0x00153F6F File Offset: 0x0015216F
		public void RemoveBlocker()
		{
			this.m_blockerCount--;
		}

		// Token: 0x060046C1 RID: 18113 RVA: 0x00153F7F File Offset: 0x0015217F
		public void RemoveBlocker(int count)
		{
			this.m_blockerCount -= count;
		}

		// Token: 0x060046C2 RID: 18114 RVA: 0x00153F8F File Offset: 0x0015218F
		public int GetBlockerCount()
		{
			return this.m_blockerCount;
		}

		// Token: 0x060046C3 RID: 18115 RVA: 0x00153F98 File Offset: 0x00152198
		public void SetFreezeFrameCamera(Camera camera)
		{
			if (camera == null)
			{
				return;
			}
			this.m_freezeFrameCamera = camera;
			this.AddObject(camera.gameObject);
		}

		// Token: 0x060046C4 RID: 18116 RVA: 0x00153FC5 File Offset: 0x001521C5
		public Camera GetFreezeFrameCamera()
		{
			return this.m_freezeFrameCamera;
		}

		// Token: 0x060046C5 RID: 18117 RVA: 0x00153FCD File Offset: 0x001521CD
		public AudioListener GetAudioListener()
		{
			return this.m_audioListener;
		}

		// Token: 0x060046C6 RID: 18118 RVA: 0x00153FD5 File Offset: 0x001521D5
		public void SetAudioListener(AudioListener listener)
		{
			if (listener == null)
			{
				return;
			}
			this.m_audioListener = listener;
			this.AddObject(listener);
		}

		// Token: 0x060046C7 RID: 18119 RVA: 0x00153FF2 File Offset: 0x001521F2
		public void EnableFadeOut(bool enable)
		{
			this.m_fadeOut = enable;
		}

		// Token: 0x060046C8 RID: 18120 RVA: 0x00153FFB File Offset: 0x001521FB
		public bool IsFadeOutEnabled()
		{
			return this.m_fadeOut;
		}

		// Token: 0x060046C9 RID: 18121 RVA: 0x00154003 File Offset: 0x00152203
		public void EnableFadeIn(bool enable)
		{
			this.m_fadeIn = enable;
		}

		// Token: 0x060046CA RID: 18122 RVA: 0x0015400C File Offset: 0x0015220C
		public bool IsFadeInEnabled()
		{
			return this.m_fadeIn;
		}

		// Token: 0x060046CB RID: 18123 RVA: 0x00154014 File Offset: 0x00152214
		public void SetFadeColor(Color color)
		{
			this.m_fadeColor = color;
		}

		// Token: 0x060046CC RID: 18124 RVA: 0x0015401D File Offset: 0x0015221D
		public Color GetFadeColor()
		{
			return this.m_fadeColor;
		}

		// Token: 0x060046CD RID: 18125 RVA: 0x00154025 File Offset: 0x00152225
		public List<Camera> GetCameras()
		{
			return this.m_cameras;
		}

		// Token: 0x060046CE RID: 18126 RVA: 0x0015402D File Offset: 0x0015222D
		public List<Light> GetLights()
		{
			return this.m_lights;
		}

		// Token: 0x170004DE RID: 1246
		// (get) Token: 0x060046CF RID: 18127 RVA: 0x00154035 File Offset: 0x00152235
		// (set) Token: 0x060046D0 RID: 18128 RVA: 0x0015403D File Offset: 0x0015223D
		public bool ClearPreviousAssets
		{
			get
			{
				return this.m_clearPreviousAssets;
			}
			set
			{
				this.m_clearPreviousAssets = value;
			}
		}

		// Token: 0x060046D1 RID: 18129 RVA: 0x00154048 File Offset: 0x00152248
		public void FixupCameras(Camera fxCamera)
		{
			if (this.m_cameras.Count == 0)
			{
				return;
			}
			Camera camera = this.m_cameras[0];
			camera.tag = "Untagged";
			float depth = camera.depth;
			for (int i = 1; i < this.m_cameras.Count; i++)
			{
				Camera camera2 = this.m_cameras[i];
				camera2.tag = "Untagged";
				if (camera2.depth > depth)
				{
					depth = camera2.depth;
				}
			}
			float num = fxCamera.depth - 1f - depth;
			for (int j = 0; j < this.m_cameras.Count; j++)
			{
				Camera camera3 = this.m_cameras[j];
				camera3.depth += num;
			}
		}

		// Token: 0x060046D2 RID: 18130 RVA: 0x0015411C File Offset: 0x0015231C
		public void AutoAddObjects()
		{
			Light[] array = (Light[])Object.FindObjectsOfType(typeof(Light));
			foreach (Light light in array)
			{
				this.AddObject(light.gameObject);
				this.m_lights.Add(light);
			}
		}

		// Token: 0x060046D3 RID: 18131 RVA: 0x00154170 File Offset: 0x00152370
		public void PreserveObjects(Transform parent)
		{
			foreach (GameObject gameObject in this.m_objects)
			{
				gameObject.transform.parent = parent;
			}
		}

		// Token: 0x060046D4 RID: 18132 RVA: 0x001541D0 File Offset: 0x001523D0
		public void DestroyObjects()
		{
			foreach (GameObject gameObject in this.m_objects)
			{
				Object.DestroyImmediate(gameObject);
			}
		}

		// Token: 0x04002DE8 RID: 11752
		private bool m_enabled = true;

		// Token: 0x04002DE9 RID: 11753
		private List<GameObject> m_objects = new List<GameObject>();

		// Token: 0x04002DEA RID: 11754
		private List<Camera> m_cameras = new List<Camera>();

		// Token: 0x04002DEB RID: 11755
		private List<Light> m_lights = new List<Light>();

		// Token: 0x04002DEC RID: 11756
		private Camera m_freezeFrameCamera;

		// Token: 0x04002DED RID: 11757
		private AudioListener m_audioListener;

		// Token: 0x04002DEE RID: 11758
		private int m_blockerCount;

		// Token: 0x04002DEF RID: 11759
		private bool m_fadeOut = true;

		// Token: 0x04002DF0 RID: 11760
		private bool m_fadeIn = true;

		// Token: 0x04002DF1 RID: 11761
		private Color m_fadeColor = Color.black;

		// Token: 0x04002DF2 RID: 11762
		private bool m_clearPreviousAssets = true;
	}

	// Token: 0x02000685 RID: 1669
	private class TransitionUnfriendlyData
	{
		// Token: 0x060046D6 RID: 18134 RVA: 0x0015423F File Offset: 0x0015243F
		public void Clear()
		{
			this.m_audioListener = null;
			this.m_lights.Clear();
		}

		// Token: 0x060046D7 RID: 18135 RVA: 0x00154253 File Offset: 0x00152453
		public AudioListener GetAudioListener()
		{
			return this.m_audioListener;
		}

		// Token: 0x060046D8 RID: 18136 RVA: 0x0015425C File Offset: 0x0015245C
		public void SetAudioListener(AudioListener listener)
		{
			if (listener == null)
			{
				return;
			}
			if (!listener.enabled)
			{
				return;
			}
			this.m_audioListener = listener;
			this.m_audioListener.enabled = false;
		}

		// Token: 0x060046D9 RID: 18137 RVA: 0x00154295 File Offset: 0x00152495
		public List<Light> GetLights()
		{
			return this.m_lights;
		}

		// Token: 0x060046DA RID: 18138 RVA: 0x001542A0 File Offset: 0x001524A0
		public void AddLights(Light[] lights)
		{
			foreach (Light light in lights)
			{
				if (light.enabled)
				{
					light.enabled = false;
					Transform transform = light.transform;
					while (transform.parent != null)
					{
						transform = transform.parent;
					}
					this.m_lights.Add(light);
				}
			}
		}

		// Token: 0x060046DB RID: 18139 RVA: 0x00154310 File Offset: 0x00152510
		public void Restore()
		{
			for (int i = 0; i < this.m_lights.Count; i++)
			{
				Light light = this.m_lights[i];
				if (light == null)
				{
					Debug.LogError(string.Format("TransitionUnfriendlyData.Restore() - light {0} is null!", i));
				}
				else
				{
					Transform transform = light.transform;
					while (transform.parent != null)
					{
						transform = transform.parent;
					}
					light.enabled = true;
				}
			}
			if (this.m_audioListener != null)
			{
				this.m_audioListener.enabled = true;
			}
		}

		// Token: 0x04002DF3 RID: 11763
		private AudioListener m_audioListener;

		// Token: 0x04002DF4 RID: 11764
		private List<Light> m_lights = new List<Light>();
	}
}
