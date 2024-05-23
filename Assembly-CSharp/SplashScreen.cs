using System;
using System.Collections;
using System.Collections.Generic;
using bgs;
using UnityEngine;
using WTCG.BI;

// Token: 0x0200028C RID: 652
[CustomEditClass]
public class SplashScreen : MonoBehaviour
{
	// Token: 0x060023A1 RID: 9121 RVA: 0x000AED90 File Offset: 0x000ACF90
	private void Awake()
	{
		SplashScreen.s_instance = this;
		OverlayUI.Get().AddGameObject(base.gameObject, CanvasAnchor.CENTER, false, CanvasScaleMode.HEIGHT);
		this.m_logo = AssetLoader.Get().LoadGameObject(FileUtils.GameAssetPathToName(this.m_logoPrefab), true, false);
		GameUtils.SetParent(this.m_logo, this.m_logoContainer, true);
		this.m_logo.SetActive(false);
		this.m_webLoginCanvas = null;
		this.Show();
		this.UpdateLayout();
		if (Vars.Key("Aurora.ClientCheck").GetBool(true) && BattleNetClient.needsToRun)
		{
			BattleNetClient.quitHearthstoneAndRun();
			return;
		}
		Network.Get().RegisterQueueInfoHandler(new Network.QueueInfoHandler(this.QueueInfoHandler));
		if (DemoMgr.Get().GetMode() == DemoMode.APPLE_STORE)
		{
			this.m_demoDisclaimer.SetActive(true);
		}
		if (ApplicationMgr.IsInternal() && ApplicationMgr.AllowResetFromFatalError)
		{
			this.m_devClearLoginButton.gameObject.SetActive(true);
			this.m_devClearLoginButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ClearLogin));
		}
	}

	// Token: 0x060023A2 RID: 9122 RVA: 0x000AEE9E File Offset: 0x000AD09E
	private void OnDestroy()
	{
		if (this.m_webAuth != null)
		{
			this.m_webAuth.Close();
		}
		SplashScreen.s_instance = null;
	}

	// Token: 0x060023A3 RID: 9123 RVA: 0x000AEEBC File Offset: 0x000AD0BC
	private void Update()
	{
		Network.Get().ProcessNetwork();
		if (!this.m_queueFinished && !Network.ShouldBeConnectedToAurora())
		{
			this.m_queueFinished = true;
		}
		if (Network.ShouldBeConnectedToAurora())
		{
			this.UpdateWebAuth();
		}
		if (!this.m_inputCameraSet && PegUI.Get() != null)
		{
			this.m_inputCameraSet = true;
			PegUI.Get().SetInputCamera(OverlayUI.Get().m_UICamera);
		}
		if (!PlayErrors.IsInitialized())
		{
			PlayErrors.Init();
		}
		this.UpdatePatching();
		this.HandleKeyboardInput();
	}

	// Token: 0x060023A4 RID: 9124 RVA: 0x000AEF54 File Offset: 0x000AD154
	public void UpdatePatching()
	{
		if (this.m_patching)
		{
			UpdateManager.UpdateProgress progressForCurrentFile = UpdateManager.Get().GetProgressForCurrentFile();
			if (!this.m_patchingFrame.activeSelf && progressForCurrentFile.showProgressBar && progressForCurrentFile.numFilesToDownload > 0)
			{
				this.m_patchingFrame.SetActive(true);
				this.m_patchingBarShownTime = Time.realtimeSinceStartup;
			}
			if (this.m_patchingFrame.activeSelf && progressForCurrentFile.numFilesToDownload > progressForCurrentFile.numFilesDownloaded)
			{
				float progressBar = (float)progressForCurrentFile.numFilesDownloaded / (float)progressForCurrentFile.numFilesToDownload + progressForCurrentFile.downloadPercentage / (float)progressForCurrentFile.numFilesToDownload;
				this.m_patchingBar.SetProgressBar(progressBar);
				string label = GameStrings.Format("GLUE_PATCHING_LABEL", new object[]
				{
					progressForCurrentFile.numFilesDownloaded + 1,
					progressForCurrentFile.numFilesToDownload
				});
				this.m_patchingBar.SetLabel(label);
				if (!UpdateManager.Get().UpdateIsRequired() && Time.realtimeSinceStartup >= this.m_patchingBarShownTime + progressForCurrentFile.maxPatchingBarDisplayTime)
				{
					Log.UpdateManager.Print("Optional update is taking too long, no longer blocking.", new object[0]);
					UpdateManager.Get().StopWaitingForUpdate();
				}
			}
		}
		if (this.m_patchingFrame != null && this.m_patchingFrame.activeSelf && !this.m_patching && Time.realtimeSinceStartup >= this.m_patchingBarShownTime + 3f)
		{
			this.m_patchingFrame.SetActive(false);
		}
	}

	// Token: 0x060023A5 RID: 9125 RVA: 0x000AF0D0 File Offset: 0x000AD2D0
	public void UpdateWebAuth()
	{
		if (this.m_webAuth != null)
		{
			switch (this.m_webAuth.GetStatus())
			{
			case WebAuth.Status.ReadyToDisplay:
				if (!this.m_webLoginCanvas.activeSelf)
				{
					this.m_webLoginCanvas.SetActive(true);
				}
				if (!this.m_webAuthHidden && !this.m_webAuth.IsShown())
				{
					this.m_webAuth.Show();
				}
				break;
			case WebAuth.Status.Success:
				this.m_webToken = this.m_webAuth.GetLoginCode();
				Log.BattleNet.Print("Web token retrieved from web pane: {0}", new object[]
				{
					this.m_webToken
				});
				WebAuth.SetStoredToken(this.m_webToken);
				BattleNet.ProvideWebAuthToken(this.m_webToken);
				this.HideWebLoginCanvas();
				this.m_webAuth.Close();
				break;
			case WebAuth.Status.Error:
				BIReport.Get().Report_Telemetry(Telemetry.Level.LEVEL_ERROR, BIReport.TelemetryEvent.EVENT_WEB_LOGIN_ERROR);
				Network.Get().ShowConnectionFailureError("GLOBAL_ERROR_NETWORK_LOGIN_FAILURE");
				break;
			}
		}
		else if (BattleNet.CheckWebAuth(ref this.m_webAuthUrl))
		{
			string str = Vars.Key("Aurora.VerifyWebCredentials").GetStr(null);
			if (str != null)
			{
				BattleNet.ProvideWebAuthToken(str);
				this.m_hasProvidedWebAuthToken = true;
				Debug.Log(string.Format("Calling ProvideWebAuthToken with={0}. If this repeats, then web token is invalid!", str));
				return;
			}
			this.m_webToken = BattleNet.GetLaunchOption("HBS_TOKEN0", true);
			if (GameUtils.AreAllTutorialsComplete(Options.Get().GetEnum<TutorialProgress>(Option.LOCAL_TUTORIAL_PROGRESS)))
			{
				this.m_webAuthUrl = NydusLink.GetAccountCreationLink();
			}
			Debug.Log("Web URL for auth: " + this.m_webAuthUrl);
			MobileCallbackManager.Get().m_wasBreakingNewsShown = false;
			if (!string.IsNullOrEmpty(this.m_webToken))
			{
				BattleNet.ProvideWebAuthToken(this.m_webToken);
				this.m_triedToken = true;
				this.m_webToken = null;
				this.RequestBreakingNews();
			}
			else
			{
				this.m_webLoginCanvas = (GameObject)GameUtils.InstantiateGameObject(this.m_webLoginCanvasPrefab, null, false);
				WebLoginCanvas component = this.m_webLoginCanvas.GetComponent<WebLoginCanvas>();
				this.m_webLoginCanvas.SetActive(false);
				Camera uicamera = OverlayUI.Get().m_UICamera;
				Vector3 vector = uicamera.WorldToScreenPoint(component.m_topLeftBone.transform.position);
				Vector3 vector2 = uicamera.WorldToScreenPoint(component.m_bottomRightBone.transform.position);
				float x = vector.x;
				float y = (float)uicamera.pixelHeight - vector.y;
				float width = vector2.x - vector.x;
				float height = vector.y - vector2.y;
				this.m_webAuth = new WebAuth(this.m_webAuthUrl, x, y, width, height, this.m_webLoginCanvas.gameObject.name);
				string domain = (ApplicationMgr.GetMobileEnvironment() != MobileEnv.DEVELOPMENT) ? ".battle.net" : ".blizzard.net";
				this.m_webAuth.SetCountryCodeCookie(MobileDeviceLocale.GetCountryCode(), domain);
				this.m_webAuth.Load();
				this.RequestBreakingNews();
				MobileCallbackManager.Get().m_wasBreakingNewsShown = true;
				if (this.m_triedToken)
				{
					Log.BattleNet.Print("Submitted login token {0} but it was rejected, so deleting stored token.", new object[]
					{
						this.m_webToken
					});
					WebAuth.ClearLoginData();
				}
			}
		}
	}

	// Token: 0x060023A6 RID: 9126 RVA: 0x000AF3FD File Offset: 0x000AD5FD
	public void HideWebLoginCanvas()
	{
		if (this.m_webLoginCanvas != null)
		{
			Object.Destroy(this.m_webLoginCanvas);
			this.m_webLoginCanvas = null;
		}
	}

	// Token: 0x060023A7 RID: 9127 RVA: 0x000AF422 File Offset: 0x000AD622
	public static SplashScreen Get()
	{
		return SplashScreen.s_instance;
	}

	// Token: 0x060023A8 RID: 9128 RVA: 0x000AF42C File Offset: 0x000AD62C
	public void Show()
	{
		base.gameObject.SetActive(true);
		Hashtable args = iTween.Hash(new object[]
		{
			"amount",
			1f,
			"time",
			0.25f,
			"easeType",
			iTween.EaseType.easeOutCubic
		});
		iTween.FadeTo(base.gameObject, args);
		if (!this.m_fadingStarted)
		{
			this.FadeGlowsIn();
		}
	}

	// Token: 0x060023A9 RID: 9129 RVA: 0x000AF4AC File Offset: 0x000AD6AC
	public void Hide()
	{
		if (this.m_webAuth != null)
		{
			this.m_webAuth.Close();
		}
		this.HideWebLoginCanvas();
		base.StartCoroutine(this.HideCoroutine());
	}

	// Token: 0x060023AA RID: 9130 RVA: 0x000AF4E2 File Offset: 0x000AD6E2
	public void HideLogo()
	{
		this.m_logo.SetActive(false);
	}

	// Token: 0x060023AB RID: 9131 RVA: 0x000AF4F0 File Offset: 0x000AD6F0
	public void HideWebAuth()
	{
		Debug.Log("HideWebAuth");
		if (this.m_webAuth != null)
		{
			this.m_webAuth.Hide();
			this.m_webAuthHidden = true;
		}
	}

	// Token: 0x060023AC RID: 9132 RVA: 0x000AF51C File Offset: 0x000AD71C
	public void UnHideWebAuth()
	{
		Debug.Log("ShowWebAuth");
		if (this.m_webAuth != null && this.m_webAuth.GetStatus() >= WebAuth.Status.ReadyToDisplay)
		{
			this.m_webAuth.Show();
			this.m_webAuthHidden = false;
		}
	}

	// Token: 0x060023AD RID: 9133 RVA: 0x000AF561 File Offset: 0x000AD761
	public void StartPatching()
	{
		this.m_patching = true;
	}

	// Token: 0x060023AE RID: 9134 RVA: 0x000AF56A File Offset: 0x000AD76A
	public void StopPatching()
	{
		this.m_patching = false;
		if (this.m_patchingBar != null && this.m_patchingBar.gameObject.activeInHierarchy)
		{
			this.m_patchingBar.SetProgressBar(1f);
		}
	}

	// Token: 0x060023AF RID: 9135 RVA: 0x000AF5A9 File Offset: 0x000AD7A9
	public void ShowRatings()
	{
		base.StartCoroutine(this.RatingsScreen());
	}

	// Token: 0x060023B0 RID: 9136 RVA: 0x000AF5B8 File Offset: 0x000AD7B8
	public bool IsRatingsScreenFinished()
	{
		return this.m_RatingsFinished;
	}

	// Token: 0x060023B1 RID: 9137 RVA: 0x000AF5C0 File Offset: 0x000AD7C0
	public bool IsFinished()
	{
		return this.m_loginFinished;
	}

	// Token: 0x060023B2 RID: 9138 RVA: 0x000AF5C8 File Offset: 0x000AD7C8
	public bool AddFinishedListener(SplashScreen.FinishedHandler handler)
	{
		if (this.m_finishedListeners.Contains(handler))
		{
			return false;
		}
		this.m_finishedListeners.Add(handler);
		return true;
	}

	// Token: 0x060023B3 RID: 9139 RVA: 0x000AF5F5 File Offset: 0x000AD7F5
	public virtual bool RemoveFinishedListener(SplashScreen.FinishedHandler handler)
	{
		return this.m_finishedListeners.Remove(handler);
	}

	// Token: 0x060023B4 RID: 9140 RVA: 0x000AF604 File Offset: 0x000AD804
	public void UpdateLayout()
	{
		Bounds nearClipBounds = CameraUtils.GetNearClipBounds(OverlayUI.Get().m_UICamera);
		float num = this.m_blizzardLogo.GetComponent<Renderer>().bounds.size.x / 1.5f;
		TransformUtil.SetPosX(this.m_blizzardLogo, nearClipBounds.max.x - num);
	}

	// Token: 0x060023B5 RID: 9141 RVA: 0x000AF668 File Offset: 0x000AD868
	private IEnumerator HideCoroutine()
	{
		while (!this.m_queueFinished || !this.m_RatingsFinished)
		{
			yield return null;
		}
		BnetBar.Get().ToggleEnableButtons(true);
		Hashtable args = iTween.Hash(new object[]
		{
			"amount",
			0f,
			"delay",
			0.9f,
			"time",
			0.7f,
			"easeType",
			iTween.EaseType.linear,
			"oncomplete",
			"DestroySplashScreen",
			"oncompletetarget",
			base.gameObject
		});
		iTween.FadeTo(base.gameObject, args);
		Hashtable g1args = iTween.Hash(new object[]
		{
			"amount",
			0f,
			"delay",
			0f,
			"time",
			0.5f,
			"easeType",
			iTween.EaseType.linear,
			"oncompletetarget",
			base.gameObject
		});
		iTween.FadeTo(this.m_glow1.gameObject, g1args);
		Hashtable g2args = iTween.Hash(new object[]
		{
			"amount",
			0f,
			"delay",
			0f,
			"time",
			0.5f,
			"easeType",
			iTween.EaseType.linear,
			"oncompletetarget",
			base.gameObject
		});
		iTween.FadeTo(this.m_glow2.gameObject, g2args);
		yield return new WaitForSeconds(0.5f);
		this.m_glow1.gameObject.SetActive(false);
		this.m_glow2.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x060023B6 RID: 9142 RVA: 0x000AF683 File Offset: 0x000AD883
	private void DestroySplashScreen()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060023B7 RID: 9143 RVA: 0x000AF690 File Offset: 0x000AD890
	public void FinishSplashScreen()
	{
		this.m_loginFinished = true;
		base.StartCoroutine(this.FadeInLogo());
		base.StartCoroutine(this.FireFinishedEvent());
	}

	// Token: 0x060023B8 RID: 9144 RVA: 0x000AF6C0 File Offset: 0x000AD8C0
	private void QueueInfoHandler(Network.QueueInfo queueInfo)
	{
		this.m_queueInfo = queueInfo;
		if (queueInfo.position == 0)
		{
			Network.Get().RemoveQueueInfoHandler(new Network.QueueInfoHandler(this.QueueInfoHandler));
			this.m_queueFinished = true;
			this.m_queueShown = false;
			this.m_queueSign.SetActive(false);
			this.FinishSplashScreen();
		}
		else
		{
			this.ShowQueueInfo();
		}
	}

	// Token: 0x060023B9 RID: 9145 RVA: 0x000AF720 File Offset: 0x000AD920
	private void ShowQueueInfo()
	{
		if (!this.m_queueShown)
		{
			this.m_queueShown = true;
			this.m_queueTitle.Text = GameStrings.Get("GLUE_SPLASH_QUEUE_TITLE");
			this.m_queueText.Text = GameStrings.Get("GLUE_SPLASH_QUEUE_TEXT");
			this.m_quitButton.SetOriginalLocalPosition();
			this.m_quitButton.SetText(GameStrings.Get("GLOBAL_QUIT"));
			this.m_quitButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.QuitGame));
			RenderUtils.SetAlpha(this.m_queueSign, 0f);
			this.m_queueSign.SetActive(true);
			Hashtable args = iTween.Hash(new object[]
			{
				"amount",
				1f,
				"time",
				0.5f,
				"easeType",
				iTween.EaseType.easeOutCubic
			});
			iTween.FadeTo(this.m_queueSign, args);
			Hashtable args2 = iTween.Hash(new object[]
			{
				"amount",
				0f,
				"time",
				0.5f,
				"includechildren",
				true,
				"easeType",
				iTween.EaseType.easeOutCubic
			});
			iTween.FadeTo(this.m_logo, args2);
		}
		TimeUtils.ElapsedStringSet splashscreen_DATETIME_STRINGSET = TimeUtils.SPLASHSCREEN_DATETIME_STRINGSET;
		this.m_queueTime.Text = TimeUtils.GetElapsedTimeString((int)this.m_queueInfo.end, splashscreen_DATETIME_STRINGSET);
	}

	// Token: 0x060023BA RID: 9146 RVA: 0x000AF898 File Offset: 0x000ADA98
	private void QuitGame(UIEvent e)
	{
		ApplicationMgr.Get().Exit();
	}

	// Token: 0x060023BB RID: 9147 RVA: 0x000AF8A4 File Offset: 0x000ADAA4
	private void ClearLogin(UIEvent e)
	{
		Debug.LogWarning("Clear Login Button pressed from the Splash Screen!");
		WebAuth.ClearLoginData();
	}

	// Token: 0x060023BC RID: 9148 RVA: 0x000AF8B8 File Offset: 0x000ADAB8
	private IEnumerator FadeGlowInOut(Glow glow, float timeDelay, bool shouldStartOver)
	{
		yield return new WaitForSeconds(timeDelay);
		Hashtable args = iTween.Hash(new object[]
		{
			"time",
			1f,
			"easeType",
			iTween.EaseType.linear,
			"from",
			0f,
			"to",
			0.4f,
			"onupdate",
			"UpdateAlpha",
			"onupdatetarget",
			glow.gameObject
		});
		iTween.ValueTo(glow.gameObject, args);
		Hashtable args2 = iTween.Hash(new object[]
		{
			"delay",
			1f,
			"time",
			1f,
			"easeType",
			iTween.EaseType.linear,
			"from",
			0.4f,
			"to",
			0f,
			"onupdate",
			"UpdateAlpha",
			"onupdatetarget",
			glow.gameObject
		});
		if (shouldStartOver)
		{
			args2.Add("oncomplete", "FadeGlowsIn");
			args2.Add("oncompletetarget", base.gameObject);
		}
		iTween.ValueTo(glow.gameObject, args2);
		yield break;
	}

	// Token: 0x060023BD RID: 9149 RVA: 0x000AF900 File Offset: 0x000ADB00
	private void FadeGlowsIn()
	{
		this.m_fadingStarted = true;
		base.StartCoroutine(this.FadeGlowInOut(this.m_glow1, 0f, false));
		base.StartCoroutine(this.FadeGlowInOut(this.m_glow2, 1f, true));
	}

	// Token: 0x060023BE RID: 9150 RVA: 0x000AF948 File Offset: 0x000ADB48
	private IEnumerator FireFinishedEvent()
	{
		yield return null;
		foreach (SplashScreen.FinishedHandler listener in this.m_finishedListeners.ToArray())
		{
			listener();
		}
		yield break;
	}

	// Token: 0x060023BF RID: 9151 RVA: 0x000AF964 File Offset: 0x000ADB64
	private IEnumerator FadeInLogo()
	{
		this.m_logo.SetActive(true);
		Hashtable fadeInArgs = iTween.Hash(new object[]
		{
			"amount",
			1f,
			"time",
			0.5f,
			"includechildren",
			true,
			"easeType",
			iTween.EaseType.easeInCubic
		});
		iTween.FadeTo(this.m_logo, fadeInArgs);
		yield return new WaitForSeconds(0.8f);
		this.m_LogoFinished = true;
		yield break;
	}

	// Token: 0x060023C0 RID: 9152 RVA: 0x000AF980 File Offset: 0x000ADB80
	private IEnumerator RatingsScreen()
	{
		while (!this.m_LogoFinished)
		{
			yield return null;
		}
		string accountCountry = BattleNet.GetAccountCountry();
		bool useKoreanRating = accountCountry == "KOR";
		if (useKoreanRating)
		{
			this.KoreaRatingsScreen();
		}
		else
		{
			this.m_RatingsFinished = true;
		}
		while (!this.m_RatingsFinished)
		{
			yield return null;
		}
		yield break;
	}

	// Token: 0x060023C1 RID: 9153 RVA: 0x000AF99B File Offset: 0x000ADB9B
	private void KoreaRatingsScreen()
	{
		AssetLoader.Get().LoadGameObject("Korean_Ratings_SplashScreen", new AssetLoader.GameObjectCallback(this.OnKoreaRatingsScreenLoaded), null, false);
	}

	// Token: 0x060023C2 RID: 9154 RVA: 0x000AF9BC File Offset: 0x000ADBBC
	private void OnKoreaRatingsScreenLoaded(string name, GameObject go, object callbackData)
	{
		OverlayUI.Get().AddGameObject(go, CanvasAnchor.CENTER, false, CanvasScaleMode.HEIGHT);
		go.SetActive(false);
		this.m_queueSign.SetActive(false);
		this.m_queueTitle.gameObject.SetActive(false);
		this.m_queueText.gameObject.SetActive(false);
		this.m_queueTime.gameObject.SetActive(false);
		base.StartCoroutine(this.KoreaRatingsScreenWait(go));
	}

	// Token: 0x060023C3 RID: 9155 RVA: 0x000AFA2C File Offset: 0x000ADC2C
	private IEnumerator KoreaRatingsScreenWait(GameObject ratingsObject)
	{
		Hashtable args2 = iTween.Hash(new object[]
		{
			"amount",
			0f,
			"time",
			0.5f,
			"includechildren",
			true,
			"easeType",
			iTween.EaseType.easeOutCubic
		});
		iTween.FadeTo(this.m_logo, args2);
		yield return new WaitForSeconds(0.5f);
		this.m_logo.SetActive(false);
		ratingsObject.SetActive(true);
		Hashtable fadeInRatings = iTween.Hash(new object[]
		{
			"amount",
			1f,
			"time",
			0.5f,
			"includechildren",
			true,
			"easeType",
			iTween.EaseType.easeInCubic
		});
		iTween.FadeTo(ratingsObject, fadeInRatings);
		yield return new WaitForSeconds(5.5f);
		Hashtable fadeOutRatings = iTween.Hash(new object[]
		{
			"amount",
			0f,
			"time",
			0.5f,
			"includechildren",
			true,
			"easeType",
			iTween.EaseType.easeInCubic
		});
		iTween.FadeTo(ratingsObject, fadeOutRatings);
		yield return new WaitForSeconds(0.5f);
		ratingsObject.SetActive(false);
		Object.Destroy(ratingsObject);
		this.m_RatingsFinished = true;
		yield break;
	}

	// Token: 0x060023C4 RID: 9156 RVA: 0x000AFA58 File Offset: 0x000ADC58
	private void RequestBreakingNews()
	{
		string breakingNewsLocalized = GameStrings.Get("GLUE_MOBILE_SPLASH_SCREEN_BREAKING_NEWS");
		string breakingNewsLink = NydusLink.GetBreakingNewsLink();
		BreakingNews.FetchBreakingNews(breakingNewsLink, delegate(string response, bool error)
		{
			if (!error)
			{
				WebAuth.UpdateBreakingNews(breakingNewsLocalized, response, MobileCallbackManager.Get().name);
			}
		});
	}

	// Token: 0x060023C5 RID: 9157 RVA: 0x000AFA93 File Offset: 0x000ADC93
	public bool HandleKeyboardInput()
	{
		return false;
	}

	// Token: 0x040014C7 RID: 5319
	private const float KOREA_RATINGS_SCREEN_DISPLAY_TIME = 5f;

	// Token: 0x040014C8 RID: 5320
	private const float GLOW_FADE_TIME = 1f;

	// Token: 0x040014C9 RID: 5321
	private const float MinPatchingBarDisplayTime = 3f;

	// Token: 0x040014CA RID: 5322
	[CustomEditField(T = EditType.GAME_OBJECT)]
	public string m_logoPrefab;

	// Token: 0x040014CB RID: 5323
	public GameObject m_logoContainer;

	// Token: 0x040014CC RID: 5324
	public GameObject m_queueSign;

	// Token: 0x040014CD RID: 5325
	[CustomEditField(T = EditType.GAME_OBJECT)]
	public String_MobileOverride m_webLoginCanvasPrefab;

	// Token: 0x040014CE RID: 5326
	public GameObject m_quitButtonParent;

	// Token: 0x040014CF RID: 5327
	public UberText m_queueTitle;

	// Token: 0x040014D0 RID: 5328
	public UberText m_queueText;

	// Token: 0x040014D1 RID: 5329
	public UberText m_queueTime;

	// Token: 0x040014D2 RID: 5330
	public StandardPegButtonNew m_quitButton;

	// Token: 0x040014D3 RID: 5331
	public Glow m_glow1;

	// Token: 0x040014D4 RID: 5332
	public Glow m_glow2;

	// Token: 0x040014D5 RID: 5333
	public GameObject m_blizzardLogo;

	// Token: 0x040014D6 RID: 5334
	public GameObject m_patchingFrame;

	// Token: 0x040014D7 RID: 5335
	public ProgressBar m_patchingBar;

	// Token: 0x040014D8 RID: 5336
	public GameObject m_demoDisclaimer;

	// Token: 0x040014D9 RID: 5337
	public StandardPegButtonNew m_devClearLoginButton;

	// Token: 0x040014DA RID: 5338
	private static SplashScreen s_instance;

	// Token: 0x040014DB RID: 5339
	private Network.QueueInfo m_queueInfo;

	// Token: 0x040014DC RID: 5340
	private bool m_queueFinished;

	// Token: 0x040014DD RID: 5341
	private bool m_queueShown;

	// Token: 0x040014DE RID: 5342
	private bool m_fadingStarted;

	// Token: 0x040014DF RID: 5343
	private List<SplashScreen.FinishedHandler> m_finishedListeners = new List<SplashScreen.FinishedHandler>();

	// Token: 0x040014E0 RID: 5344
	private bool m_RatingsFinished;

	// Token: 0x040014E1 RID: 5345
	private bool m_LogoFinished;

	// Token: 0x040014E2 RID: 5346
	private bool m_loginFinished;

	// Token: 0x040014E3 RID: 5347
	private GameObject m_logo;

	// Token: 0x040014E4 RID: 5348
	private GameObject m_webLoginCanvas;

	// Token: 0x040014E5 RID: 5349
	private bool m_patching;

	// Token: 0x040014E6 RID: 5350
	private float m_patchingBarShownTime;

	// Token: 0x040014E7 RID: 5351
	private string m_webAuthUrl;

	// Token: 0x040014E8 RID: 5352
	private string m_webToken;

	// Token: 0x040014E9 RID: 5353
	private bool m_triedToken;

	// Token: 0x040014EA RID: 5354
	private WebAuth m_webAuth;

	// Token: 0x040014EB RID: 5355
	private bool m_webAuthHidden;

	// Token: 0x040014EC RID: 5356
	private bool m_inputCameraSet;

	// Token: 0x040014ED RID: 5357
	private bool m_hasProvidedWebAuthToken;

	// Token: 0x0200028D RID: 653
	// (Invoke) Token: 0x060023C7 RID: 9159
	public delegate void FinishedHandler();
}
