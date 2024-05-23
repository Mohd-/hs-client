using System;
using UnityEngine;

// Token: 0x02000481 RID: 1153
public class ExistingAccountPopup : DialogBase
{
	// Token: 0x060037F1 RID: 14321 RVA: 0x00112BF4 File Offset: 0x00110DF4
	private void Start()
	{
		base.transform.position = new Vector3(base.transform.position.x, -525f, 800f);
		this.m_haveAccountButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.HaveAccountButtonRelease));
		this.m_noAccountButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.NoAccountButtonRelease));
		this.m_haveAccountButton.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.HaveAccountButtonPress));
		this.m_noAccountButton.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.NoAccountButtonPress));
		this.FadeEffectsIn();
	}

	// Token: 0x060037F2 RID: 14322 RVA: 0x00112C98 File Offset: 0x00110E98
	public override void Show()
	{
		base.Show();
		BaseUI.Get().m_BnetBar.Disable();
		this.m_bubble.SetActive(true);
		iTween.FadeTo(this.m_bubble, iTween.Hash(new object[]
		{
			"time",
			0f,
			"amount",
			1f,
			"oncomplete",
			"ShowBubble",
			"oncompletetarget",
			base.gameObject
		}));
		this.m_showAnimState = DialogBase.ShowAnimState.IN_PROGRESS;
		UniversalInputManager.Get().SetSystemDialogActive(true);
		SoundManager.Get().LoadAndPlay(this.m_sound.m_popupShow);
		SoundManager.Get().LoadAndPlay(this.m_sound.m_innkeeperWelcome);
	}

	// Token: 0x060037F3 RID: 14323 RVA: 0x00112D63 File Offset: 0x00110F63
	public void SetInfo(ExistingAccountPopup.Info info)
	{
		this.m_responseCallback = info.m_callback;
	}

	// Token: 0x060037F4 RID: 14324 RVA: 0x00112D74 File Offset: 0x00110F74
	protected void FadeBubble()
	{
		iTween.FadeTo(this.m_bubble, iTween.Hash(new object[]
		{
			"delay",
			6f,
			"time",
			1f,
			"amount",
			0f
		}));
	}

	// Token: 0x060037F5 RID: 14325 RVA: 0x00112DD8 File Offset: 0x00110FD8
	protected void ShowBubble()
	{
		iTween.FadeFrom(this.m_bubble, iTween.Hash(new object[]
		{
			"delay",
			1f,
			"time",
			0.5f,
			"amount",
			0f,
			"oncomplete",
			"FadeBubble",
			"oncompletetarget",
			base.gameObject
		}));
	}

	// Token: 0x060037F6 RID: 14326 RVA: 0x00112E60 File Offset: 0x00111060
	protected void DownScale()
	{
		iTween.ScaleTo(base.gameObject, iTween.Hash(new object[]
		{
			"scale",
			new Vector3(0f, 0f, 0f),
			"delay",
			0.1,
			"easetype",
			iTween.EaseType.easeInOutCubic,
			"oncomplete",
			"OnHideAnimFinished",
			"time",
			0.2f
		}));
	}

	// Token: 0x060037F7 RID: 14327 RVA: 0x00112EF8 File Offset: 0x001110F8
	protected override void OnHideAnimFinished()
	{
		base.OnHideAnimFinished();
		this.m_shown = false;
		SoundManager.Get().LoadAndPlay(this.m_sound.m_popupHide);
		BaseUI.Get().m_BnetBar.Enable();
		this.m_responseCallback(this.m_haveAccount);
	}

	// Token: 0x060037F8 RID: 14328 RVA: 0x00112F48 File Offset: 0x00111148
	private void HaveAccountButtonRelease(UIEvent e)
	{
		this.m_haveAccount = true;
		this.m_haveAccountButton.transform.localPosition -= this.m_buttonOffset;
		this.ScaleAway();
	}

	// Token: 0x060037F9 RID: 14329 RVA: 0x00112F84 File Offset: 0x00111184
	private void NoAccountButtonRelease(UIEvent e)
	{
		this.m_haveAccount = false;
		this.m_noAccountButton.transform.localPosition -= this.m_buttonOffset;
		this.FadeEffectsOut();
	}

	// Token: 0x060037FA RID: 14330 RVA: 0x00112FC0 File Offset: 0x001111C0
	private void HaveAccountButtonPress(UIEvent e)
	{
		SoundManager.Get().LoadAndPlay(this.m_sound.m_buttonClick);
		this.m_haveAccountButton.transform.localPosition += this.m_buttonOffset;
	}

	// Token: 0x060037FB RID: 14331 RVA: 0x00113004 File Offset: 0x00111204
	private void NoAccountButtonPress(UIEvent e)
	{
		SoundManager.Get().LoadAndPlay(this.m_sound.m_buttonClick);
		this.m_noAccountButton.transform.localPosition += this.m_buttonOffset;
	}

	// Token: 0x060037FC RID: 14332 RVA: 0x00113048 File Offset: 0x00111248
	private void ScaleAway()
	{
		iTween.ScaleTo(base.gameObject, iTween.Hash(new object[]
		{
			"scale",
			Vector3.Scale(this.PUNCH_SCALE, base.gameObject.transform.localScale),
			"easetype",
			iTween.EaseType.easeInOutCubic,
			"oncomplete",
			"DownScale",
			"time",
			0.1f
		}));
	}

	// Token: 0x060037FD RID: 14333 RVA: 0x001130CC File Offset: 0x001112CC
	private void FadeEffectsIn()
	{
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		if (fullScreenFXMgr != null)
		{
			fullScreenFXMgr.SetBlurBrightness(1f);
			fullScreenFXMgr.SetBlurDesaturation(0f);
			fullScreenFXMgr.Vignette(0.4f, 0.4f, iTween.EaseType.easeOutCirc, null);
			fullScreenFXMgr.Blur(1f, 0.4f, iTween.EaseType.easeOutCirc, null);
		}
	}

	// Token: 0x060037FE RID: 14334 RVA: 0x00113128 File Offset: 0x00111328
	private void FadeEffectsOut()
	{
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		if (fullScreenFXMgr != null)
		{
			fullScreenFXMgr.StopVignette(0.2f, iTween.EaseType.easeOutCirc, null);
			fullScreenFXMgr.StopBlur(0.2f, iTween.EaseType.easeOutCirc, null);
		}
		this.ScaleAway();
	}

	// Token: 0x040023E3 RID: 9187
	public PegUIElement m_haveAccountButton;

	// Token: 0x040023E4 RID: 9188
	public PegUIElement m_noAccountButton;

	// Token: 0x040023E5 RID: 9189
	public GameObject m_bubble;

	// Token: 0x040023E6 RID: 9190
	public ExistingAccoundSound m_sound;

	// Token: 0x040023E7 RID: 9191
	private Vector3 m_buttonOffset = new Vector3(0.2f, 0f, 0.6f);

	// Token: 0x040023E8 RID: 9192
	private bool m_haveAccount;

	// Token: 0x040023E9 RID: 9193
	private ExistingAccountPopup.ResponseCallback m_responseCallback;

	// Token: 0x02000482 RID: 1154
	// (Invoke) Token: 0x06003800 RID: 14336
	public delegate void ResponseCallback(bool hasAccount);

	// Token: 0x02000483 RID: 1155
	public class Info
	{
		// Token: 0x040023EA RID: 9194
		public ExistingAccountPopup.ResponseCallback m_callback;
	}
}
