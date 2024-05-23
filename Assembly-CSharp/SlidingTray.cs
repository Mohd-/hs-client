using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000398 RID: 920
[CustomEditClass]
public class SlidingTray : MonoBehaviour
{
	// Token: 0x06002FF9 RID: 12281 RVA: 0x000F1494 File Offset: 0x000EF694
	private void Awake()
	{
		if (UniversalInputManager.UsePhoneUI || true)
		{
			if (!this.m_startingPositionSet)
			{
				base.transform.localPosition = this.m_trayHiddenBone.localPosition;
				this.m_trayShown = false;
				if (this.m_inactivateOnHide)
				{
					base.gameObject.SetActive(false);
				}
				this.m_startingPositionSet = true;
			}
			if (this.m_traySliderButton != null)
			{
				this.m_traySliderButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnTraySliderPressed));
			}
			if (this.m_offClickCatcher != null)
			{
				this.m_offClickCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClickCatcherPressed));
			}
		}
		else if (this.m_traySliderButton != null && this.m_inactivateOnHide)
		{
			this.m_traySliderButton.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002FFA RID: 12282 RVA: 0x000F1582 File Offset: 0x000EF782
	private void Start()
	{
	}

	// Token: 0x06002FFB RID: 12283 RVA: 0x000F1584 File Offset: 0x000EF784
	private void OnDestroy()
	{
		if (this.m_offClickCatcher != null)
		{
			this.m_offClickCatcher.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClickCatcherPressed));
		}
		if (this.m_traySliderButton != null)
		{
			this.m_traySliderButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnTraySliderPressed));
		}
	}

	// Token: 0x06002FFC RID: 12284 RVA: 0x000F15E5 File Offset: 0x000EF7E5
	[ContextMenu("Show")]
	public void ShowTray()
	{
		this.ToggleTraySlider(true, null, true);
	}

	// Token: 0x06002FFD RID: 12285 RVA: 0x000F15F0 File Offset: 0x000EF7F0
	[ContextMenu("Hide")]
	public void HideTray()
	{
		this.ToggleTraySlider(false, null, true);
	}

	// Token: 0x06002FFE RID: 12286 RVA: 0x000F15FC File Offset: 0x000EF7FC
	public void ToggleTraySlider(bool show, Transform target = null, bool animate = true)
	{
		if (this.m_trayShown == show)
		{
			return;
		}
		if (show && target != null)
		{
			this.m_trayShownBone = target;
		}
		if (show)
		{
			if (this.m_useNavigationBack)
			{
				Navigation.Push(new Navigation.NavigateBackHandler(this.BackPressed));
			}
			base.gameObject.SetActive(true);
			if (base.gameObject.activeInHierarchy && animate)
			{
				Hashtable args = iTween.Hash(new object[]
				{
					"position",
					this.m_trayShownBone.localPosition,
					"isLocal",
					true,
					"time",
					this.m_traySlideDuration,
					"oncomplete",
					"OnTraySliderAnimFinished",
					"oncompletetarget",
					base.gameObject,
					"easetype",
					(!this.m_animateBounce) ? iTween.Defaults.easeType : iTween.EaseType.easeOutBounce
				});
				iTween.MoveTo(base.gameObject, args);
				this.m_traySliderAnimating = true;
				if (this.m_offClickCatcher != null)
				{
					this.FadeEffectsIn(0.4f);
					this.m_offClickCatcher.gameObject.SetActive(true);
				}
				if (this.m_playAudioOnSlide)
				{
					SoundManager.Get().LoadAndPlay("choose_opponent_panel_slide_on", base.gameObject);
				}
			}
			else
			{
				base.gameObject.transform.localPosition = this.m_trayShownBone.localPosition;
			}
		}
		else
		{
			if (this.m_useNavigationBack)
			{
				Navigation.PopUnique(new Navigation.NavigateBackHandler(this.BackPressed));
			}
			if (base.gameObject.activeInHierarchy && animate)
			{
				Hashtable args2 = iTween.Hash(new object[]
				{
					"position",
					this.m_trayHiddenBone.localPosition,
					"isLocal",
					true,
					"oncomplete",
					"OnTraySliderAnimFinished",
					"oncompletetarget",
					base.gameObject,
					"time",
					(!this.m_animateBounce) ? (this.m_traySlideDuration / 2f) : this.m_traySlideDuration,
					"easetype",
					(!this.m_animateBounce) ? iTween.EaseType.linear : iTween.EaseType.easeOutBounce
				});
				iTween.MoveTo(base.gameObject, args2);
				this.m_traySliderAnimating = true;
				if (this.m_offClickCatcher != null)
				{
					this.FadeEffectsOut(0.2f);
					this.m_offClickCatcher.gameObject.SetActive(false);
				}
				if (this.m_playAudioOnSlide)
				{
					SoundManager.Get().LoadAndPlay("choose_opponent_panel_slide_off", base.gameObject);
				}
			}
			else
			{
				base.gameObject.transform.localPosition = this.m_trayHiddenBone.localPosition;
				if (this.m_inactivateOnHide)
				{
					base.gameObject.SetActive(false);
				}
			}
		}
		this.m_trayShown = show;
		this.m_startingPositionSet = true;
		if (this.m_trayToggledListener != null)
		{
			this.m_trayToggledListener(show);
		}
	}

	// Token: 0x06002FFF RID: 12287 RVA: 0x000F1933 File Offset: 0x000EFB33
	public bool TraySliderIsAnimating()
	{
		return this.m_traySliderAnimating;
	}

	// Token: 0x06003000 RID: 12288 RVA: 0x000F193C File Offset: 0x000EFB3C
	public bool IsTrayInShownPosition()
	{
		return base.gameObject.transform.localPosition == this.m_trayShownBone.localPosition;
	}

	// Token: 0x06003001 RID: 12289 RVA: 0x000F1969 File Offset: 0x000EFB69
	public bool IsShown()
	{
		return this.m_trayShown;
	}

	// Token: 0x06003002 RID: 12290 RVA: 0x000F1971 File Offset: 0x000EFB71
	public void RegisterTrayToggleListener(SlidingTray.TrayToggledListener listener)
	{
		this.m_trayToggledListener = listener;
	}

	// Token: 0x06003003 RID: 12291 RVA: 0x000F197C File Offset: 0x000EFB7C
	public void UnregisterTrayToggleListener(SlidingTray.TrayToggledListener listener)
	{
		if (this.m_trayToggledListener == listener)
		{
			this.m_trayToggledListener = null;
		}
		else
		{
			Log.JMac.Print("Attempting to unregister a TrayToggleListener that has not been registered!", new object[0]);
		}
	}

	// Token: 0x06003004 RID: 12292 RVA: 0x000F19BB File Offset: 0x000EFBBB
	public void SetLayers(GameLayer visible, GameLayer hidden)
	{
		this.m_shownLayer = visible;
		this.m_hiddenLayer = hidden;
	}

	// Token: 0x06003005 RID: 12293 RVA: 0x000F19CB File Offset: 0x000EFBCB
	private bool BackPressed()
	{
		this.ToggleTraySlider(false, null, true);
		return true;
	}

	// Token: 0x06003006 RID: 12294 RVA: 0x000F19D7 File Offset: 0x000EFBD7
	private void OnTraySliderAnimFinished()
	{
		this.m_traySliderAnimating = false;
		if (!this.m_trayShown && this.m_inactivateOnHide)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06003007 RID: 12295 RVA: 0x000F1A04 File Offset: 0x000EFC04
	private void OnTraySliderPressed(UIEvent e)
	{
		if (this.m_useNavigationBack && this.m_trayShown)
		{
			return;
		}
		this.ToggleTraySlider(!this.m_trayShown, null, true);
	}

	// Token: 0x06003008 RID: 12296 RVA: 0x000F1A39 File Offset: 0x000EFC39
	private void OnClickCatcherPressed(UIEvent e)
	{
		this.ToggleTraySlider(false, null, true);
	}

	// Token: 0x06003009 RID: 12297 RVA: 0x000F1A44 File Offset: 0x000EFC44
	private void FadeEffectsIn(float time = 0.4f)
	{
		SceneUtils.SetLayer(base.gameObject, this.m_shownLayer);
		if (this.m_shownLayer == GameLayer.IgnoreFullScreenEffects)
		{
			SceneUtils.SetLayer(Box.Get().m_letterboxingContainer, this.m_shownLayer);
		}
		FullScreenFXMgr.Get().StartStandardBlurVignette(time);
	}

	// Token: 0x0600300A RID: 12298 RVA: 0x000F1A8F File Offset: 0x000EFC8F
	private void FadeEffectsOut(float time = 0.2f)
	{
		FullScreenFXMgr.Get().EndStandardBlurVignette(time, new FullScreenFXMgr.EffectListener(this.OnFadeFinished));
	}

	// Token: 0x0600300B RID: 12299 RVA: 0x000F1AA8 File Offset: 0x000EFCA8
	private void OnFadeFinished()
	{
		if (base.gameObject == null)
		{
			return;
		}
		SceneUtils.SetLayer(base.gameObject, this.m_shownLayer);
		if (this.m_hiddenLayer == GameLayer.Default)
		{
			SceneUtils.SetLayer(Box.Get().m_letterboxingContainer, this.m_hiddenLayer);
		}
	}

	// Token: 0x04001DD1 RID: 7633
	[CustomEditField(Sections = "Bones")]
	public Transform m_trayHiddenBone;

	// Token: 0x04001DD2 RID: 7634
	[CustomEditField(Sections = "Bones")]
	public Transform m_trayShownBone;

	// Token: 0x04001DD3 RID: 7635
	[CustomEditField(Sections = "Parameters")]
	public bool m_inactivateOnHide = true;

	// Token: 0x04001DD4 RID: 7636
	[CustomEditField(Sections = "Parameters")]
	public bool m_useNavigationBack;

	// Token: 0x04001DD5 RID: 7637
	[CustomEditField(Sections = "Parameters")]
	public bool m_playAudioOnSlide = true;

	// Token: 0x04001DD6 RID: 7638
	[CustomEditField(Sections = "Parameters")]
	public float m_traySlideDuration = 0.5f;

	// Token: 0x04001DD7 RID: 7639
	[CustomEditField(Sections = "Parameters")]
	public bool m_animateBounce;

	// Token: 0x04001DD8 RID: 7640
	[CustomEditField(Sections = "Optional Features")]
	public PegUIElement m_offClickCatcher;

	// Token: 0x04001DD9 RID: 7641
	[CustomEditField(Sections = "Optional Features")]
	public PegUIElement m_traySliderButton;

	// Token: 0x04001DDA RID: 7642
	private bool m_trayShown;

	// Token: 0x04001DDB RID: 7643
	private bool m_traySliderAnimating;

	// Token: 0x04001DDC RID: 7644
	private SlidingTray.TrayToggledListener m_trayToggledListener;

	// Token: 0x04001DDD RID: 7645
	private bool m_startingPositionSet;

	// Token: 0x04001DDE RID: 7646
	private GameLayer m_hiddenLayer;

	// Token: 0x04001DDF RID: 7647
	private GameLayer m_shownLayer = GameLayer.IgnoreFullScreenEffects;

	// Token: 0x0200039C RID: 924
	// (Invoke) Token: 0x06003075 RID: 12405
	public delegate void TrayToggledListener(bool shown);
}
