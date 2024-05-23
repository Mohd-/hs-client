using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using WTCG.BI;

// Token: 0x02000011 RID: 17
public class ApplicationMgr : MonoBehaviour
{
	// Token: 0x14000001 RID: 1
	// (add) Token: 0x060001F0 RID: 496 RVA: 0x0000A61B File Offset: 0x0000881B
	// (remove) Token: 0x060001F1 RID: 497 RVA: 0x0000A634 File Offset: 0x00008834
	public event Action WillReset;

	// Token: 0x14000002 RID: 2
	// (add) Token: 0x060001F2 RID: 498 RVA: 0x0000A64D File Offset: 0x0000884D
	// (remove) Token: 0x060001F3 RID: 499 RVA: 0x0000A666 File Offset: 0x00008866
	public event Action Resetting;

	// Token: 0x14000003 RID: 3
	// (add) Token: 0x060001F4 RID: 500 RVA: 0x0000A67F File Offset: 0x0000887F
	// (remove) Token: 0x060001F5 RID: 501 RVA: 0x0000A698 File Offset: 0x00008898
	public event Action Paused;

	// Token: 0x14000004 RID: 4
	// (add) Token: 0x060001F6 RID: 502 RVA: 0x0000A6B1 File Offset: 0x000088B1
	// (remove) Token: 0x060001F7 RID: 503 RVA: 0x0000A6CA File Offset: 0x000088CA
	public event Action Unpaused;

	// Token: 0x060001F8 RID: 504 RVA: 0x0000A6E3 File Offset: 0x000088E3
	private void Awake()
	{
		ApplicationMgr.s_instance = this;
		this.Initialize();
	}

	// Token: 0x060001F9 RID: 505 RVA: 0x0000A6F1 File Offset: 0x000088F1
	private void OnDestroy()
	{
	}

	// Token: 0x060001FA RID: 506 RVA: 0x0000A6F3 File Offset: 0x000088F3
	private void Start()
	{
		AutomationInterpretor.Get().Start();
	}

	// Token: 0x060001FB RID: 507 RVA: 0x0000A6FF File Offset: 0x000088FF
	private void Update()
	{
		this.ProcessScheduledCallbacks();
		Network.Heartbeat();
		AutomationInterpretor.Get().Update();
	}

	// Token: 0x060001FC RID: 508 RVA: 0x0000A716 File Offset: 0x00008916
	private void OnGUI()
	{
		Debug.developerConsoleVisible = false;
		BugReporter.OnGUI();
	}

	// Token: 0x060001FD RID: 509 RVA: 0x0000A723 File Offset: 0x00008923
	private void OnApplicationQuit()
	{
		UberText.StoreCachedData();
		Network.AppQuit();
		W8Touch.AppQuit();
		this.UnloadUnusedAssets();
	}

	// Token: 0x060001FE RID: 510 RVA: 0x0000A73A File Offset: 0x0000893A
	private void OnApplicationFocus(bool focus)
	{
		if (this.m_focused == focus)
		{
			return;
		}
		this.m_focused = focus;
		this.FireFocusChangedEvent();
	}

	// Token: 0x060001FF RID: 511 RVA: 0x0000A758 File Offset: 0x00008958
	private void OnApplicationPause(bool pauseStatus)
	{
		if (Time.frameCount == 0)
		{
			return;
		}
		if (pauseStatus)
		{
			this.m_lastPauseTime = Time.realtimeSinceStartup;
			if (this.Paused != null)
			{
				this.Paused.Invoke();
			}
			UberText.StoreCachedData();
			Network.ApplicationPaused();
		}
		else
		{
			this.m_hasResetSinceLastResume = false;
			float num = Time.realtimeSinceStartup - this.m_lastPauseTime;
			Debug.Log("Time spent paused: " + num);
			if (DemoMgr.Get().GetMode() == DemoMode.APPLE_STORE && num > 180f)
			{
				this.ResetImmediately(false, false);
			}
			this.m_lastResumeTime = Time.realtimeSinceStartup;
			if (this.Unpaused != null)
			{
				this.Unpaused.Invoke();
			}
			Network.ApplicationUnpaused();
			if (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.FATAL_ERROR))
			{
				this.ResetImmediately(false, false);
			}
		}
	}

	// Token: 0x06000200 RID: 512 RVA: 0x0000A831 File Offset: 0x00008A31
	public static ApplicationMgr Get()
	{
		return ApplicationMgr.s_instance;
	}

	// Token: 0x06000201 RID: 513 RVA: 0x0000A838 File Offset: 0x00008A38
	public static ApplicationMode GetMode()
	{
		ApplicationMgr.InitializeMode();
		return ApplicationMgr.s_mode;
	}

	// Token: 0x06000202 RID: 514 RVA: 0x0000A844 File Offset: 0x00008A44
	public static bool IsInternal()
	{
		return ApplicationMgr.GetMode() == ApplicationMode.INTERNAL;
	}

	// Token: 0x06000203 RID: 515 RVA: 0x0000A84E File Offset: 0x00008A4E
	public static bool IsPublic()
	{
		return ApplicationMgr.GetMode() == ApplicationMode.PUBLIC;
	}

	// Token: 0x06000204 RID: 516 RVA: 0x0000A858 File Offset: 0x00008A58
	public static bool UseDevWorkarounds()
	{
		return Application.isEditor || ApplicationMgr.UsingStandaloneLocalData();
	}

	// Token: 0x06000205 RID: 517 RVA: 0x0000A874 File Offset: 0x00008A74
	public static MobileEnv GetMobileEnvironment()
	{
		string text = Vars.Key("Mobile.Mode").GetStr("undefined");
		if (text == "undefined")
		{
			text = "Production";
		}
		if (text == "Production")
		{
			return MobileEnv.PRODUCTION;
		}
		return MobileEnv.DEVELOPMENT;
	}

	// Token: 0x06000206 RID: 518 RVA: 0x0000A8BF File Offset: 0x00008ABF
	public static AndroidStore GetAndroidStore()
	{
		return AndroidStore.NONE;
	}

	// Token: 0x06000207 RID: 519 RVA: 0x0000A8C2 File Offset: 0x00008AC2
	public bool IsResetting()
	{
		return this.m_resetting;
	}

	// Token: 0x06000208 RID: 520 RVA: 0x0000A8CA File Offset: 0x00008ACA
	public void Reset()
	{
		base.StartCoroutine(this.WaitThenReset(false, false));
	}

	// Token: 0x06000209 RID: 521 RVA: 0x0000A8DB File Offset: 0x00008ADB
	public void ResetAndForceLogin()
	{
		ConnectAPI.ResetSubscription();
		base.StartCoroutine(this.WaitThenReset(true, false));
	}

	// Token: 0x0600020A RID: 522 RVA: 0x0000A8F1 File Offset: 0x00008AF1
	public void ResetAndGoBackToNoAccountTutorial()
	{
		ConnectAPI.ResetSubscription();
		base.StartCoroutine(this.WaitThenReset(false, true));
	}

	// Token: 0x0600020B RID: 523 RVA: 0x0000A907 File Offset: 0x00008B07
	public bool ResetOnErrorIfNecessary()
	{
		if (!this.m_hasResetSinceLastResume && Time.realtimeSinceStartup < this.m_lastResumeTime + 1f)
		{
			base.StartCoroutine(this.WaitThenReset(false, false));
			return true;
		}
		return false;
	}

	// Token: 0x0600020C RID: 524 RVA: 0x0000A93C File Offset: 0x00008B3C
	public void Exit()
	{
		this.m_exiting = true;
		if (ErrorReporter.Get().busy)
		{
			base.StartCoroutine(this.WaitThenExit());
		}
		else
		{
			GeneralUtils.ExitApplication();
		}
	}

	// Token: 0x0600020D RID: 525 RVA: 0x0000A96B File Offset: 0x00008B6B
	public bool IsExiting()
	{
		return this.m_exiting;
	}

	// Token: 0x0600020E RID: 526 RVA: 0x0000A973 File Offset: 0x00008B73
	public bool HasFocus()
	{
		return this.m_focused;
	}

	// Token: 0x0600020F RID: 527 RVA: 0x0000A97B File Offset: 0x00008B7B
	public bool AddFocusChangedListener(ApplicationMgr.FocusChangedCallback callback)
	{
		return this.AddFocusChangedListener(callback, null);
	}

	// Token: 0x06000210 RID: 528 RVA: 0x0000A988 File Offset: 0x00008B88
	public bool AddFocusChangedListener(ApplicationMgr.FocusChangedCallback callback, object userData)
	{
		ApplicationMgr.FocusChangedListener focusChangedListener = new ApplicationMgr.FocusChangedListener();
		focusChangedListener.SetCallback(callback);
		focusChangedListener.SetUserData(userData);
		if (this.m_focusChangedListeners.Contains(focusChangedListener))
		{
			return false;
		}
		this.m_focusChangedListeners.Add(focusChangedListener);
		return true;
	}

	// Token: 0x06000211 RID: 529 RVA: 0x0000A9C9 File Offset: 0x00008BC9
	public bool RemoveFocusChangedListener(ApplicationMgr.FocusChangedCallback callback)
	{
		return this.RemoveFocusChangedListener(callback, null);
	}

	// Token: 0x06000212 RID: 530 RVA: 0x0000A9D4 File Offset: 0x00008BD4
	public bool RemoveFocusChangedListener(ApplicationMgr.FocusChangedCallback callback, object userData)
	{
		ApplicationMgr.FocusChangedListener focusChangedListener = new ApplicationMgr.FocusChangedListener();
		focusChangedListener.SetCallback(callback);
		focusChangedListener.SetUserData(userData);
		return this.m_focusChangedListeners.Remove(focusChangedListener);
	}

	// Token: 0x06000213 RID: 531 RVA: 0x0000AA01 File Offset: 0x00008C01
	public float LastResetTime()
	{
		return this.m_lastResetTime;
	}

	// Token: 0x06000214 RID: 532 RVA: 0x0000AA09 File Offset: 0x00008C09
	public void UnloadUnusedAssets()
	{
		Resources.UnloadUnusedAssets();
	}

	// Token: 0x06000215 RID: 533 RVA: 0x0000AA14 File Offset: 0x00008C14
	public bool ScheduleCallback(float secondsToWait, bool realTime, ApplicationMgr.ScheduledCallback cb, object userData = null)
	{
		if (!GeneralUtils.IsCallbackValid(cb))
		{
			return false;
		}
		if (this.m_schedulerContexts == null)
		{
			this.m_schedulerContexts = new LinkedList<ApplicationMgr.SchedulerContext>();
		}
		else
		{
			foreach (ApplicationMgr.SchedulerContext schedulerContext in this.m_schedulerContexts)
			{
				if (!(schedulerContext.m_callback != cb))
				{
					if (schedulerContext.m_userData == userData)
					{
						return false;
					}
				}
			}
		}
		ApplicationMgr.SchedulerContext schedulerContext2 = new ApplicationMgr.SchedulerContext();
		schedulerContext2.m_startTime = Time.realtimeSinceStartup;
		schedulerContext2.m_secondsToWait = secondsToWait;
		schedulerContext2.m_realTime = realTime;
		schedulerContext2.m_callback = cb;
		schedulerContext2.m_userData = userData;
		float num = schedulerContext2.EstimateTargetTime();
		bool flag = false;
		for (LinkedListNode<ApplicationMgr.SchedulerContext> linkedListNode = this.m_schedulerContexts.Last; linkedListNode != null; linkedListNode = linkedListNode.Previous)
		{
			ApplicationMgr.SchedulerContext value = linkedListNode.Value;
			float num2 = value.EstimateTargetTime();
			if (num2 <= num)
			{
				flag = true;
				this.m_schedulerContexts.AddAfter(linkedListNode, schedulerContext2);
				break;
			}
		}
		if (!flag)
		{
			this.m_schedulerContexts.AddFirst(schedulerContext2);
		}
		return true;
	}

	// Token: 0x06000216 RID: 534 RVA: 0x0000AB64 File Offset: 0x00008D64
	public bool CancelScheduledCallback(ApplicationMgr.ScheduledCallback cb, object userData = null)
	{
		if (!GeneralUtils.IsCallbackValid(cb))
		{
			return false;
		}
		if (this.m_schedulerContexts == null)
		{
			return false;
		}
		if (this.m_schedulerContexts.Count == 0)
		{
			return false;
		}
		for (LinkedListNode<ApplicationMgr.SchedulerContext> linkedListNode = this.m_schedulerContexts.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			ApplicationMgr.SchedulerContext value = linkedListNode.Value;
			if (value.m_callback == cb && value.m_userData == userData)
			{
				this.m_schedulerContexts.Remove(linkedListNode);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000217 RID: 535 RVA: 0x0000ABF0 File Offset: 0x00008DF0
	private void ProcessScheduledCallbacks()
	{
		if (this.m_schedulerContexts == null)
		{
			return;
		}
		if (this.m_schedulerContexts.Count == 0)
		{
			return;
		}
		LinkedList<ApplicationMgr.SchedulerContext> linkedList = null;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		LinkedListNode<ApplicationMgr.SchedulerContext> linkedListNode = this.m_schedulerContexts.First;
		while (linkedListNode != null)
		{
			ApplicationMgr.SchedulerContext value = linkedListNode.Value;
			if (value.m_realTime)
			{
				value.m_secondsWaited = realtimeSinceStartup - value.m_startTime;
			}
			else
			{
				value.m_secondsWaited += Time.deltaTime;
			}
			if (value.m_secondsWaited >= value.m_secondsToWait)
			{
				if (linkedList == null)
				{
					linkedList = new LinkedList<ApplicationMgr.SchedulerContext>();
				}
				linkedList.AddLast(value);
				LinkedListNode<ApplicationMgr.SchedulerContext> next = linkedListNode.Next;
				this.m_schedulerContexts.Remove(linkedListNode);
				linkedListNode = next;
			}
			else if (!GeneralUtils.IsCallbackValid(value.m_callback))
			{
				LinkedListNode<ApplicationMgr.SchedulerContext> next2 = linkedListNode.Next;
				this.m_schedulerContexts.Remove(linkedListNode);
				linkedListNode = next2;
			}
			else
			{
				linkedListNode = linkedListNode.Next;
			}
		}
		if (linkedList != null)
		{
			foreach (ApplicationMgr.SchedulerContext schedulerContext in linkedList)
			{
				schedulerContext.m_callback(schedulerContext.m_userData);
			}
		}
	}

	// Token: 0x06000218 RID: 536 RVA: 0x0000AD44 File Offset: 0x00008F44
	public static bool UsingStandaloneLocalData()
	{
		return !string.IsNullOrEmpty(ApplicationMgr.GetStandaloneLocalDataPath());
	}

	// Token: 0x06000219 RID: 537 RVA: 0x0000AD54 File Offset: 0x00008F54
	public static bool TryGetStandaloneLocalDataPath(string subPath, out string outPath)
	{
		string standaloneLocalDataPath = ApplicationMgr.GetStandaloneLocalDataPath();
		if (!string.IsNullOrEmpty(standaloneLocalDataPath))
		{
			outPath = Path.Combine(standaloneLocalDataPath, subPath);
			return true;
		}
		outPath = null;
		return false;
	}

	// Token: 0x0600021A RID: 538 RVA: 0x0000AD81 File Offset: 0x00008F81
	private static string GetStandaloneLocalDataPath()
	{
		return null;
	}

	// Token: 0x0600021B RID: 539 RVA: 0x0000AD84 File Offset: 0x00008F84
	private static void InitializeMode()
	{
		if (ApplicationMgr.s_mode != ApplicationMode.INVALID)
		{
			return;
		}
		ApplicationMgr.s_mode = ApplicationMode.PUBLIC;
	}

	// Token: 0x0600021C RID: 540 RVA: 0x0000AD98 File Offset: 0x00008F98
	private void Initialize()
	{
		LogArchive.Init();
		ApplicationMgr.InitializeMode();
		this.InitializeUnity();
		this.InitializeGame();
		this.InitializeWindowTitle();
		this.InitializeOptionValues();
		this.WillReset = (Action)Delegate.Combine(this.WillReset, new Action(GameStrings.WillReset));
	}

	// Token: 0x0600021D RID: 541 RVA: 0x0000ADE9 File Offset: 0x00008FE9
	private void InitializeUnity()
	{
		Application.runInBackground = true;
		Application.targetFrameRate = 30;
		Application.backgroundLoadingPriority = 0;
	}

	// Token: 0x0600021E RID: 542 RVA: 0x0000AE00 File Offset: 0x00009000
	private void InitializeGame()
	{
		BugReporter.Init();
		GameDbf.Load();
		DemoMgr.Get().Initialize();
		LocalOptions.Get().Initialize();
		if (DemoMgr.Get().GetMode() == DemoMode.APPLE_STORE)
		{
			DemoMgr.Get().ApplyAppleStoreDemoDefaults();
		}
		if (Network.TUTORIALS_WITHOUT_ACCOUNT)
		{
			Network.SetShouldBeConnectedToAurora(Options.Get().GetBool(Option.CONNECT_TO_AURORA));
		}
		Localization.Initialize();
		GameStrings.LoadAll();
		Network.Initialize();
		if (!PlayErrors.Init())
		{
			Debug.LogError(string.Format("{0} failed to load!", "PlayErrors32"));
		}
		GameMgr.Get().Initialize();
		ChangedCardMgr.Get().Initialize();
		TavernBrawlManager.Init();
		AdventureProgressMgr.Init();
		AchieveManager.Init();
		FixedRewardsMgr.Initialize();
	}

	// Token: 0x0600021F RID: 543
	[DllImport("user32.dll")]
	public static extern int SetWindowTextW(IntPtr hWnd, [MarshalAs(21)] string text);

	// Token: 0x06000220 RID: 544
	[DllImport("user32.dll")]
	public static extern IntPtr FindWindow(string className, string windowName);

	// Token: 0x06000221 RID: 545 RVA: 0x0000AEBC File Offset: 0x000090BC
	private void InitializeWindowTitle()
	{
		IntPtr intPtr = ApplicationMgr.FindWindow(null, "Hearthstone");
		if (intPtr != IntPtr.Zero)
		{
			ApplicationMgr.SetWindowTextW(intPtr, GameStrings.Get("GLOBAL_PROGRAMNAME_HEARTHSTONE"));
		}
	}

	// Token: 0x06000222 RID: 546 RVA: 0x0000AEF8 File Offset: 0x000090F8
	private void InitializeOptionValues()
	{
		if (ApplicationMgr.IsPublic())
		{
			Options.Get().SetOption(Option.SOUND, OptionDataTables.s_defaultsMap[Option.SOUND]);
			Options.Get().SetOption(Option.MUSIC, OptionDataTables.s_defaultsMap[Option.MUSIC]);
		}
	}

	// Token: 0x06000223 RID: 547 RVA: 0x0000AF3C File Offset: 0x0000913C
	private IEnumerator WaitThenReset(bool forceLogin, bool forceNoAccountTutorial = false)
	{
		this.m_resetting = true;
		Navigation.Clear();
		yield return new WaitForEndOfFrame();
		this.ResetImmediately(forceLogin, forceNoAccountTutorial);
		yield break;
	}

	// Token: 0x06000224 RID: 548 RVA: 0x0000AF74 File Offset: 0x00009174
	private void ResetImmediately(bool forceLogin, bool forceNoAccountTutorial = false)
	{
		Log.Reset.Print(string.Concat(new object[]
		{
			"ApplicationMgr.ResetImmediately - forceLogin? ",
			forceLogin,
			"  Stack trace: ",
			Environment.StackTrace
		}), new object[0]);
		BIReport.Get().Report_Telemetry(Telemetry.Level.LEVEL_INFO, (!forceLogin) ? BIReport.TelemetryEvent.EVENT_ON_RESET : BIReport.TelemetryEvent.EVENT_ON_RESET_WITH_LOGIN);
		if (this.WillReset != null)
		{
			this.WillReset.Invoke();
		}
		this.m_resetting = true;
		this.m_lastResetTime = Time.realtimeSinceStartup;
		if (DialogManager.Get() != null)
		{
			DialogManager.Get().ClearAllImmediately();
		}
		if (Network.TUTORIALS_WITHOUT_ACCOUNT)
		{
			Network.SetShouldBeConnectedToAurora(forceLogin || Options.Get().GetBool(Option.CONNECT_TO_AURORA));
		}
		FatalErrorMgr.Get().ClearAllErrors();
		this.m_hasResetSinceLastResume = true;
		if (forceNoAccountTutorial)
		{
			Options.Get().SetBool(Option.CONNECT_TO_AURORA, false);
			Network.SetShouldBeConnectedToAurora(false);
		}
		if (this.Resetting != null)
		{
			this.Resetting.Invoke();
		}
		Network.Reset();
		Navigation.Clear();
		this.m_resetting = false;
		Log.Reset.Print("\tApplicationMgr.ResetImmediately completed", new object[0]);
	}

	// Token: 0x06000225 RID: 549 RVA: 0x0000B0B0 File Offset: 0x000092B0
	private IEnumerator WaitThenExit()
	{
		while (ErrorReporter.Get().busy)
		{
			yield return null;
		}
		GeneralUtils.ExitApplication();
		yield break;
	}

	// Token: 0x06000226 RID: 550 RVA: 0x0000B0C4 File Offset: 0x000092C4
	private void FireFocusChangedEvent()
	{
		ApplicationMgr.FocusChangedListener[] array = this.m_focusChangedListeners.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Fire(this.m_focused);
		}
	}

	// Token: 0x04000071 RID: 113
	private const ApplicationMode DEFAULT_MODE = ApplicationMode.INTERNAL;

	// Token: 0x04000072 RID: 114
	private const float AUTO_RESET_ON_ERROR_TIMEOUT = 1f;

	// Token: 0x04000073 RID: 115
	public static readonly PlatformDependentValue<bool> CanQuitGame = new PlatformDependentValue<bool>(PlatformCategory.OS)
	{
		PC = true,
		Mac = true,
		Android = false,
		iOS = false
	};

	// Token: 0x04000074 RID: 116
	public static readonly PlatformDependentValue<bool> AllowResetFromFatalError = new PlatformDependentValue<bool>(PlatformCategory.OS)
	{
		PC = false,
		Mac = false,
		Android = true,
		iOS = true
	};

	// Token: 0x04000075 RID: 117
	private static ApplicationMgr s_instance;

	// Token: 0x04000076 RID: 118
	private static ApplicationMode s_mode;

	// Token: 0x04000077 RID: 119
	private bool m_exiting;

	// Token: 0x04000078 RID: 120
	private bool m_focused = true;

	// Token: 0x04000079 RID: 121
	private List<ApplicationMgr.FocusChangedListener> m_focusChangedListeners = new List<ApplicationMgr.FocusChangedListener>();

	// Token: 0x0400007A RID: 122
	private float m_lastResumeTime = -999999f;

	// Token: 0x0400007B RID: 123
	private float m_lastPauseTime;

	// Token: 0x0400007C RID: 124
	private bool m_hasResetSinceLastResume;

	// Token: 0x0400007D RID: 125
	private bool m_resetting;

	// Token: 0x0400007E RID: 126
	private float m_lastResetTime;

	// Token: 0x0400007F RID: 127
	private LinkedList<ApplicationMgr.SchedulerContext> m_schedulerContexts;

	// Token: 0x020000ED RID: 237
	// (Invoke) Token: 0x06000BFB RID: 3067
	public delegate void ScheduledCallback(object userData);

	// Token: 0x02000117 RID: 279
	private class FocusChangedListener : EventListener<ApplicationMgr.FocusChangedCallback>
	{
		// Token: 0x06000D80 RID: 3456 RVA: 0x00037068 File Offset: 0x00035268
		public void Fire(bool focused)
		{
			this.m_callback(focused, this.m_userData);
		}
	}

	// Token: 0x02000118 RID: 280
	// (Invoke) Token: 0x06000D82 RID: 3458
	public delegate void FocusChangedCallback(bool focused, object userData);

	// Token: 0x02000119 RID: 281
	private class SchedulerContext
	{
		// Token: 0x06000D86 RID: 3462 RVA: 0x00037084 File Offset: 0x00035284
		public float EstimateTargetTime()
		{
			float startTime = this.m_startTime;
			return startTime + ((!this.m_realTime) ? (this.m_secondsToWait * Time.timeScale) : this.m_secondsToWait);
		}

		// Token: 0x0400072F RID: 1839
		public float m_startTime;

		// Token: 0x04000730 RID: 1840
		public float m_secondsToWait;

		// Token: 0x04000731 RID: 1841
		public bool m_realTime;

		// Token: 0x04000732 RID: 1842
		public ApplicationMgr.ScheduledCallback m_callback;

		// Token: 0x04000733 RID: 1843
		public object m_userData;

		// Token: 0x04000734 RID: 1844
		public float m_secondsWaited;
	}
}
