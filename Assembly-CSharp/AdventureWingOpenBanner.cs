using System;
using UnityEngine;

// Token: 0x020003CB RID: 971
[CustomEditClass]
public class AdventureWingOpenBanner : MonoBehaviour
{
	// Token: 0x060032BB RID: 12987 RVA: 0x000FD1F0 File Offset: 0x000FB3F0
	private void Awake()
	{
		if (this.m_clickCatcher != null)
		{
			this.m_clickCatcher.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
			{
				this.HideBanner();
			});
		}
		if (this.m_root != null)
		{
			this.m_root.SetActive(false);
		}
		OverlayUI.Get().AddGameObject(base.gameObject, CanvasAnchor.CENTER, true, CanvasScaleMode.HEIGHT);
	}

	// Token: 0x060032BC RID: 12988 RVA: 0x000FD258 File Offset: 0x000FB458
	public void ShowBanner(AdventureWingOpenBanner.OnBannerHidden onBannerHiddenCallback = null)
	{
		if (this.m_root == null)
		{
			Debug.LogError("m_root not defined in banner!");
			return;
		}
		this.m_bannerHiddenCallback = onBannerHiddenCallback;
		this.m_originalScale = this.m_root.transform.localScale;
		this.m_root.SetActive(true);
		iTween.ScaleFrom(this.m_root, iTween.Hash(new object[]
		{
			"scale",
			new Vector3(0.01f, 0.01f, 0.01f),
			"time",
			this.m_showTime,
			"easetype",
			this.m_showEase
		}));
		if (!string.IsNullOrEmpty(this.m_showSound))
		{
			SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_showSound));
		}
		if (!string.IsNullOrEmpty(this.m_VOQuotePrefab) && !string.IsNullOrEmpty(this.m_VOQuoteLine))
		{
			NotificationManager.Get().CreateCharacterQuote(FileUtils.GameAssetPathToName(this.m_VOQuotePrefab), this.m_VOQuotePosition, GameStrings.Get(this.m_VOQuoteLine), this.m_VOQuoteLine, true, 0f, null, CanvasAnchor.CENTER);
		}
		FullScreenFXMgr.Get().StartStandardBlurVignette(this.m_showTime);
	}

	// Token: 0x060032BD RID: 12989 RVA: 0x000FD39C File Offset: 0x000FB59C
	public void HideBanner()
	{
		if (this.m_root == null)
		{
			Debug.LogError("m_root not defined in banner!");
			return;
		}
		FullScreenFXMgr.Get().EndStandardBlurVignette(this.m_hideTime, null);
		this.m_root.transform.localScale = this.m_originalScale;
		iTween.ScaleTo(this.m_root, iTween.Hash(new object[]
		{
			"scale",
			new Vector3(0.01f, 0.01f, 0.01f),
			"oncomplete",
			delegate(object o)
			{
				Object.Destroy(base.gameObject);
				if (this.m_bannerHiddenCallback != null)
				{
					this.m_bannerHiddenCallback();
				}
			},
			"time",
			this.m_hideTime
		}));
		if (!string.IsNullOrEmpty(this.m_hideSound))
		{
			SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_hideSound));
		}
	}

	// Token: 0x04001F8C RID: 8076
	public PegUIElement m_clickCatcher;

	// Token: 0x04001F8D RID: 8077
	public GameObject m_root;

	// Token: 0x04001F8E RID: 8078
	public iTween.EaseType m_showEase = iTween.EaseType.easeOutElastic;

	// Token: 0x04001F8F RID: 8079
	public float m_showTime = 0.5f;

	// Token: 0x04001F90 RID: 8080
	public float m_hideTime = 0.5f;

	// Token: 0x04001F91 RID: 8081
	public string m_VOQuotePrefab;

	// Token: 0x04001F92 RID: 8082
	public string m_VOQuoteLine;

	// Token: 0x04001F93 RID: 8083
	public Vector3 m_VOQuotePosition = new Vector3(0f, 0f, -55f);

	// Token: 0x04001F94 RID: 8084
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_showSound;

	// Token: 0x04001F95 RID: 8085
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_hideSound;

	// Token: 0x04001F96 RID: 8086
	private Vector3 m_originalScale;

	// Token: 0x04001F97 RID: 8087
	private AdventureWingOpenBanner.OnBannerHidden m_bannerHiddenCallback;

	// Token: 0x020003CC RID: 972
	// (Invoke) Token: 0x060032C1 RID: 12993
	public delegate void OnBannerHidden();
}
