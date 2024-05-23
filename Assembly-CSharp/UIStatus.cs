using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000239 RID: 569
public class UIStatus : MonoBehaviour
{
	// Token: 0x06002161 RID: 8545 RVA: 0x000A37B4 File Offset: 0x000A19B4
	private void Awake()
	{
		UIStatus.s_instance = this;
		this.m_Text.gameObject.SetActive(false);
		if (OverlayUI.Get())
		{
			OverlayUI.Get().AddGameObject(base.gameObject, CanvasAnchor.CENTER, false, CanvasScaleMode.HEIGHT);
			return;
		}
		throw new UnityException("Trying to create UIStatus before OverlayUI!");
	}

	// Token: 0x06002162 RID: 8546 RVA: 0x000A380A File Offset: 0x000A1A0A
	private void OnDestroy()
	{
		UIStatus.s_instance = null;
	}

	// Token: 0x06002163 RID: 8547 RVA: 0x000A3814 File Offset: 0x000A1A14
	public static UIStatus Get()
	{
		if (UIStatus.s_instance == null)
		{
			UIStatus.s_instance = AssetLoader.Get().LoadUIScreen("UIStatus", true, false).GetComponent<UIStatus>();
		}
		return UIStatus.s_instance;
	}

	// Token: 0x06002164 RID: 8548 RVA: 0x000A3851 File Offset: 0x000A1A51
	public void AddInfo(string message)
	{
		this.AddInfo(message, false);
	}

	// Token: 0x06002165 RID: 8549 RVA: 0x000A385B File Offset: 0x000A1A5B
	public void AddInfo(string message, float delay)
	{
		this.AddInfo(message, false, delay);
	}

	// Token: 0x06002166 RID: 8550 RVA: 0x000A3866 File Offset: 0x000A1A66
	public void AddInfo(string message, bool isScreenshot)
	{
		this.AddInfo(message, isScreenshot, -1f);
	}

	// Token: 0x06002167 RID: 8551 RVA: 0x000A3878 File Offset: 0x000A1A78
	public void AddInfo(string message, bool isScreenshot, float delay)
	{
		this.m_isScreenshot = isScreenshot;
		this.m_Text.TextColor = this.m_InfoColor;
		this.ShowMessage(message, delay);
	}

	// Token: 0x06002168 RID: 8552 RVA: 0x000A38A5 File Offset: 0x000A1AA5
	public void AddError(string message)
	{
		this.m_Text.TextColor = this.m_ErrorColor;
		this.ShowMessage(message);
	}

	// Token: 0x06002169 RID: 8553 RVA: 0x000A38C0 File Offset: 0x000A1AC0
	public void HideIfScreenshotMessage()
	{
		if (!this.m_isScreenshot)
		{
			return;
		}
		iTween.Stop(this.m_Text.gameObject);
		this.OnFadeComplete();
	}

	// Token: 0x0600216A RID: 8554 RVA: 0x000A38EF File Offset: 0x000A1AEF
	private void ShowMessage(string message)
	{
		this.ShowMessage(message, -1f);
	}

	// Token: 0x0600216B RID: 8555 RVA: 0x000A3900 File Offset: 0x000A1B00
	private void ShowMessage(string message, float delay)
	{
		this.m_Text.Text = string.Empty;
		if (message.Contains("\n"))
		{
			this.m_Text.ResizeToFit = false;
			this.m_Text.WordWrap = true;
			this.m_Text.ForceWrapLargeWords = true;
		}
		else
		{
			this.m_Text.ResizeToFit = true;
			this.m_Text.WordWrap = false;
			this.m_Text.ForceWrapLargeWords = false;
		}
		this.m_Text.Text = message;
		this.m_Text.gameObject.SetActive(true);
		this.m_Text.TextAlpha = 1f;
		iTween.Stop(this.m_Text.gameObject, true);
		if (delay < 0f)
		{
			delay = this.m_FadeDelaySec;
		}
		Hashtable args = iTween.Hash(new object[]
		{
			"amount",
			0f,
			"delay",
			delay,
			"time",
			this.m_FadeSec,
			"easeType",
			this.m_FadeEaseType,
			"oncomplete",
			"OnFadeComplete",
			"oncompletetarget",
			base.gameObject
		});
		iTween.FadeTo(this.m_Text.gameObject, args);
	}

	// Token: 0x0600216C RID: 8556 RVA: 0x000A3A5F File Offset: 0x000A1C5F
	private void OnFadeComplete()
	{
		this.m_isScreenshot = false;
		this.m_Text.gameObject.SetActive(false);
	}

	// Token: 0x040012B1 RID: 4785
	public UberText m_Text;

	// Token: 0x040012B2 RID: 4786
	public Color m_InfoColor;

	// Token: 0x040012B3 RID: 4787
	public Color m_ErrorColor;

	// Token: 0x040012B4 RID: 4788
	public float m_FadeDelaySec = 2f;

	// Token: 0x040012B5 RID: 4789
	public float m_FadeSec = 0.5f;

	// Token: 0x040012B6 RID: 4790
	public iTween.EaseType m_FadeEaseType = iTween.EaseType.linear;

	// Token: 0x040012B7 RID: 4791
	private static UIStatus s_instance;

	// Token: 0x040012B8 RID: 4792
	private bool m_isScreenshot;
}
