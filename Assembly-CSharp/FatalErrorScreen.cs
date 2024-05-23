using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000894 RID: 2196
public class FatalErrorScreen : MonoBehaviour
{
	// Token: 0x060053AF RID: 21423 RVA: 0x0018F4E0 File Offset: 0x0018D6E0
	private void Awake()
	{
		SplashScreen splashScreen = SplashScreen.Get();
		if (splashScreen != null)
		{
			splashScreen.HideLogo();
		}
		List<FatalErrorMessage> messages = FatalErrorMgr.Get().GetMessages();
		Log.JMac.Print(LogLevel.Warning, string.Format("Showing Fatal Error Screen with {0} messages", messages.Count), new object[0]);
		string text = messages[0].m_text;
		this.m_closedSignText.Text = text;
		this.m_closedSignTitle.Text = GameStrings.Get("GLOBAL_SPLASH_CLOSED_SIGN_TITLE");
		this.m_allowClick = messages[0].m_allowClick;
		this.m_redirectToStore = messages[0].m_redirectToStore;
		this.m_delayBeforeNextReset = messages[0].m_delayBeforeNextReset;
	}

	// Token: 0x060053B0 RID: 21424 RVA: 0x0018F59A File Offset: 0x0018D79A
	private void Start()
	{
		base.StartCoroutine(this.WaitForUIThenFinishSetup());
	}

	// Token: 0x060053B1 RID: 21425 RVA: 0x0018F5AC File Offset: 0x0018D7AC
	private void Update()
	{
		if (ApplicationMgr.AllowResetFromFatalError && this.m_allowClick)
		{
			if (this.m_redirectToStore)
			{
				this.m_reconnectTip.SetGameStringText("GLOBAL_MOBILE_TAP_TO_UPDATE");
			}
			else
			{
				this.m_reconnectTip.SetGameStringText("GLOBAL_MOBILE_TAP_TO_RECONNECT");
			}
			this.m_reconnectTip.gameObject.SetActive(true);
			float num = 1f;
			this.m_reconnectTip.TextAlpha = (Mathf.Sin(Time.time * 3.1415927f / num) + 1f) / 2f;
		}
	}

	// Token: 0x060053B2 RID: 21426 RVA: 0x0018F644 File Offset: 0x0018D844
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
	}

	// Token: 0x060053B3 RID: 21427 RVA: 0x0018F6B0 File Offset: 0x0018D8B0
	private IEnumerator WaitForUIThenFinishSetup()
	{
		while (PegUI.Get() == null || OverlayUI.Get() == null)
		{
			yield return null;
		}
		OverlayUI.Get().AddGameObject(base.gameObject, CanvasAnchor.CENTER, false, CanvasScaleMode.HEIGHT);
		this.Show();
		this.m_camera = CameraUtils.FindFirstByLayer(base.gameObject.layer);
		PegUI.Get().SetInputCamera(this.m_camera);
		GameObject inputBlockerObject = CameraUtils.CreateInputBlocker(this.m_camera, "ClosedSignInputBlocker", this);
		SceneUtils.SetLayer(inputBlockerObject, base.gameObject.layer);
		this.m_inputBlocker = inputBlockerObject.AddComponent<PegUIElement>();
		if (this.m_allowClick)
		{
			this.m_inputBlocker.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClick));
		}
		if (FatalErrorMgr.Get().GetFormattedErrorCode() != null)
		{
			this.m_errorCodeText.gameObject.SetActive(true);
			this.m_errorCodeText.Text = FatalErrorMgr.Get().GetFormattedErrorCode();
			OverlayUI.Get().AddGameObject(this.m_errorCodeText.gameObject, CanvasAnchor.TOP_RIGHT, false, CanvasScaleMode.HEIGHT);
		}
		yield break;
	}

	// Token: 0x060053B4 RID: 21428 RVA: 0x0018F6CC File Offset: 0x0018D8CC
	private void OnClick(UIEvent e)
	{
		if (ApplicationMgr.AllowResetFromFatalError)
		{
			if (this.m_redirectToStore)
			{
				PlatformDependentValue<string> platformDependentValue = new PlatformDependentValue<string>(PlatformCategory.OS)
				{
					iOS = "https://itunes.apple.com/app/hearthstone-heroes-warcraft/id625257520?ls=1&mt=8",
					Android = "https://play.google.com/store/apps/details?id=com.blizzard.wtcg.hearthstone"
				};
				PlatformDependentValue<string> platformDependentValue2 = new PlatformDependentValue<string>(PlatformCategory.OS)
				{
					iOS = "https://itunes.apple.com/cn/app/lu-shi-chuan-shuo-mo-shou/id841140063?ls=1&mt=8",
					Android = "https://www.battlenet.com.cn/account/download/hearthstone/android?style=hearthstone"
				};
				if (ApplicationMgr.GetAndroidStore() == AndroidStore.AMAZON)
				{
					platformDependentValue.Android = "http://www.amazon.com/gp/mas/dl/android?p=com.blizzard.wtcg.hearthstone";
				}
				if (MobileDeviceLocale.GetCurrentRegionId() == 5)
				{
					platformDependentValue = platformDependentValue2;
				}
				Application.OpenURL(platformDependentValue);
			}
			else
			{
				float num = ApplicationMgr.Get().LastResetTime() + this.m_delayBeforeNextReset - Time.realtimeSinceStartup;
				Log.JMac.Print("Remaining time to wait before allowing a reconnect attempt: " + num, new object[0]);
				if (num > 0f)
				{
					this.m_inputBlocker.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClick));
					this.m_closedSignText.Text = GameStrings.Get("GLOBAL_SPLASH_CLOSED_RECONNECTING");
					this.m_allowClick = false;
					this.m_reconnectTip.gameObject.SetActive(false);
					base.StartCoroutine(this.WaitBeforeReconnecting(num));
				}
				else
				{
					Debug.Log("resetting!");
					this.m_inputBlocker.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClick));
					ApplicationMgr.Get().Reset();
				}
			}
		}
		else
		{
			this.m_inputBlocker.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClick));
			ApplicationMgr.Get().Exit();
		}
	}

	// Token: 0x060053B5 RID: 21429 RVA: 0x0018F858 File Offset: 0x0018DA58
	private IEnumerator WaitBeforeReconnecting(float waitDuration)
	{
		yield return new WaitForSeconds(waitDuration);
		ApplicationMgr.Get().Reset();
		yield break;
	}

	// Token: 0x040039DE RID: 14814
	public UberText m_closedSignText;

	// Token: 0x040039DF RID: 14815
	public UberText m_closedSignTitle;

	// Token: 0x040039E0 RID: 14816
	public UberText m_reconnectTip;

	// Token: 0x040039E1 RID: 14817
	public UberText m_errorCodeText;

	// Token: 0x040039E2 RID: 14818
	private Camera m_camera;

	// Token: 0x040039E3 RID: 14819
	private PegUIElement m_inputBlocker;

	// Token: 0x040039E4 RID: 14820
	private bool m_allowClick;

	// Token: 0x040039E5 RID: 14821
	private bool m_redirectToStore;

	// Token: 0x040039E6 RID: 14822
	public float m_delayBeforeNextReset;
}
