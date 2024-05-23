using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000438 RID: 1080
[CustomEditClass]
public class BannerPopup : MonoBehaviour
{
	// Token: 0x06003635 RID: 13877 RVA: 0x0010B765 File Offset: 0x00109965
	private void Awake()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06003636 RID: 13878 RVA: 0x0010B774 File Offset: 0x00109974
	private void Start()
	{
		if (this.m_ShowSpell == null)
		{
			this.OnShowSpellFinished(null, null);
		}
		else
		{
			this.m_ShowSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnShowSpellFinished));
			this.m_ShowSpell.Activate();
		}
	}

	// Token: 0x06003637 RID: 13879 RVA: 0x0010B7C4 File Offset: 0x001099C4
	public void Show(string bannerText, BannerManager.DelOnCloseBanner onCloseCallback = null)
	{
		OverlayUI.Get().AddGameObject(base.gameObject, CanvasAnchor.CENTER, true, CanvasScaleMode.HEIGHT);
		this.m_text.Text = bannerText;
		this.m_onCloseBannerPopup = onCloseCallback;
		base.gameObject.SetActive(true);
		Animation component = this.m_root.GetComponent<Animation>();
		component.Play();
		Camera camera = CameraUtils.FindFirstByLayer(base.gameObject.layer);
		GameObject gameObject = CameraUtils.CreateInputBlocker(camera, "ClosedSignInputBlocker", this);
		SceneUtils.SetLayer(gameObject, base.gameObject.layer);
		this.m_inputBlocker = gameObject.AddComponent<PegUIElement>();
		iTween.ScaleFrom(base.gameObject, iTween.Hash(new object[]
		{
			"scale",
			new Vector3(0.01f, 0.01f, 0.01f),
			"time",
			0.25f,
			"oncompletetarget",
			base.gameObject,
			"oncomplete",
			"EnableClickHandler"
		}));
		this.FadeEffectsIn();
		this.m_showSpellComplete = false;
	}

	// Token: 0x06003638 RID: 13880 RVA: 0x0010B8D0 File Offset: 0x00109AD0
	private void FadeEffectsIn()
	{
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		fullScreenFXMgr.SetBlurBrightness(1f);
		fullScreenFXMgr.SetBlurDesaturation(0f);
		fullScreenFXMgr.Vignette(0.4f, 0.2f, iTween.EaseType.easeOutCirc, null);
		fullScreenFXMgr.Blur(1f, 0.2f, iTween.EaseType.easeOutCirc, null);
	}

	// Token: 0x06003639 RID: 13881 RVA: 0x0010B920 File Offset: 0x00109B20
	private void FadeEffectsOut()
	{
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		fullScreenFXMgr.StopVignette(0.2f, iTween.EaseType.easeOutCirc, null);
		fullScreenFXMgr.StopBlur(0.2f, iTween.EaseType.easeOutCirc, null);
	}

	// Token: 0x0600363A RID: 13882 RVA: 0x0010B950 File Offset: 0x00109B50
	private void CloseBannerPopup(UIEvent e)
	{
		this.FadeEffectsOut();
		this.m_inputBlocker.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.CloseBannerPopup));
		iTween.ScaleTo(base.gameObject, iTween.Hash(new object[]
		{
			"scale",
			Vector3.zero,
			"time",
			0.5f,
			"oncompletetarget",
			base.gameObject,
			"oncomplete",
			"DestroyAdventurePopup"
		}));
		SoundManager.Get().LoadAndPlay("new_quest_click_and_shrink");
		ParticleSystem[] componentsInChildren = base.gameObject.GetComponentsInChildren<ParticleSystem>();
		if (componentsInChildren != null)
		{
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				particleSystem.gameObject.SetActive(false);
			}
		}
		if (this.m_LoopingSpell != null)
		{
			this.m_LoopingSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnLoopingSpellFinished));
			this.m_LoopingSpell.ActivateState(SpellStateType.DEATH);
		}
		else if (this.m_HideSpell != null)
		{
			this.m_HideSpell.Activate();
		}
	}

	// Token: 0x0600363B RID: 13883 RVA: 0x0010BA79 File Offset: 0x00109C79
	private void EnableClickHandler()
	{
		this.m_inputBlocker.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.CloseBannerPopup));
	}

	// Token: 0x0600363C RID: 13884 RVA: 0x0010BA94 File Offset: 0x00109C94
	private void DestroyAdventurePopup()
	{
		this.m_onCloseBannerPopup();
		base.StartCoroutine(this.DestroyPopupObject());
	}

	// Token: 0x0600363D RID: 13885 RVA: 0x0010BAB0 File Offset: 0x00109CB0
	private IEnumerator DestroyPopupObject()
	{
		while (!this.m_showSpellComplete)
		{
			yield return null;
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x0600363E RID: 13886 RVA: 0x0010BACB File Offset: 0x00109CCB
	private void OnShowSpellFinished(Spell spell, object userData)
	{
		this.m_showSpellComplete = true;
		if (this.m_LoopingSpell == null)
		{
			this.OnLoopingSpellFinished(null, null);
		}
		else
		{
			this.m_LoopingSpell.ActivateState(SpellStateType.ACTION);
		}
	}

	// Token: 0x0600363F RID: 13887 RVA: 0x0010BAFE File Offset: 0x00109CFE
	private void OnLoopingSpellFinished(Spell spell, object userData)
	{
		if (this.m_HideSpell != null)
		{
			this.m_HideSpell.Activate();
		}
	}

	// Token: 0x040021BC RID: 8636
	public GameObject m_root;

	// Token: 0x040021BD RID: 8637
	public UberText m_text;

	// Token: 0x040021BE RID: 8638
	public Spell m_ShowSpell;

	// Token: 0x040021BF RID: 8639
	public Spell m_LoopingSpell;

	// Token: 0x040021C0 RID: 8640
	public Spell m_HideSpell;

	// Token: 0x040021C1 RID: 8641
	private BannerManager.DelOnCloseBanner m_onCloseBannerPopup;

	// Token: 0x040021C2 RID: 8642
	private PegUIElement m_inputBlocker;

	// Token: 0x040021C3 RID: 8643
	private bool m_showSpellComplete = true;
}
