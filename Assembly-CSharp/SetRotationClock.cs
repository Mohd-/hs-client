using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000254 RID: 596
public class SetRotationClock : MonoBehaviour
{
	// Token: 0x060021DE RID: 8670 RVA: 0x000A642C File Offset: 0x000A462C
	private void Awake()
	{
		SetRotationClock.s_instance = this;
		if (UniversalInputManager.UsePhoneUI)
		{
			base.transform.position = new Vector3(-60.7f, -18.939f, -43f);
			base.transform.localScale = new Vector3(9.043651f, 9.043651f, 9.043651f);
		}
		else
		{
			base.transform.position = new Vector3(-47.234f, -18.939f, -31.837f);
			base.transform.localScale = new Vector3(6.970411f, 6.970411f, 6.970411f);
		}
		this.m_overlayText.HideImmediate();
		this.m_clickCatcher.gameObject.SetActive(false);
		this.m_clickCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClick));
		this.m_buttonBannerScale = this.m_ButtonBanner.transform.localScale;
		this.m_ButtonBannerStandard.TextColor = this.m_ButtonBannerTextColor;
		this.m_ButtonBannerWild.TextColor = this.m_ButtonBannerTextColor;
		this.m_ButtonBanner.SetActive(false);
		this.m_ButtonBannerStandard.gameObject.SetActive(false);
		this.m_ButtonBannerWild.gameObject.SetActive(false);
	}

	// Token: 0x060021DF RID: 8671 RVA: 0x000A656A File Offset: 0x000A476A
	public static SetRotationClock Get()
	{
		return SetRotationClock.s_instance;
	}

	// Token: 0x060021E0 RID: 8672 RVA: 0x000A6571 File Offset: 0x000A4771
	public void StartTheClock()
	{
		this.m_SetRotationButton.SetActive(true);
		base.StartCoroutine(this.ClockAnimation());
	}

	// Token: 0x060021E1 RID: 8673 RVA: 0x000A658C File Offset: 0x000A478C
	public void ShakeCamera()
	{
		CameraShakeMgr.Shake(Camera.main, new Vector3(0.1f, 0.1f, 0.1f), 0.4f);
	}

	// Token: 0x060021E2 RID: 8674 RVA: 0x000A65B4 File Offset: 0x000A47B4
	public IEnumerator ClockAnimation()
	{
		AudioSource clickSound = null;
		if (this.m_ClickSound != null)
		{
			clickSound = Object.Instantiate<AudioSource>(this.m_ClickSound);
		}
		DeckPickerTrayDisplay.Get().InitSetRotationTutorial();
		this.PlayClockAnimation();
		if (this.m_Stage1Sound != null)
		{
			SoundManager.Get().Play(Object.Instantiate<AudioSource>(this.m_Stage1Sound));
		}
		if (this.m_TheClockAmbientSound != null)
		{
			this.FadeInAmbientSound();
		}
		yield return new WaitForSeconds(this.m_AnimationWaitTime);
		this.VignetteBackground(0.5f);
		this.m_clickCatcher.gameObject.SetActive(true);
		this.m_clickCaptured = false;
		this.m_overlayText.UpdateText(0);
		this.m_overlayText.Show();
		yield return new WaitForSeconds(this.m_TextDelayTime);
		while (!this.m_clickCaptured)
		{
			yield return null;
		}
		if (clickSound != null)
		{
			SoundManager.Get().Play(clickSound);
		}
		if (this.m_Stage2Sound != null)
		{
			SoundManager.Get().Play(Object.Instantiate<AudioSource>(this.m_Stage2Sound));
		}
		this.StopVignetteBackground(0.5f);
		this.m_clickCatcher.gameObject.SetActive(false);
		this.m_overlayText.Hide();
		yield return new WaitForSeconds(this.m_TextDelayTime);
		this.FlipCenterPanelButton();
		yield return new WaitForSeconds(this.m_ButtonRotationHoldTime);
		this.RaiseButton();
		yield return new WaitForSeconds(this.m_BlurScreenDelay);
		this.BlurBackground(this.m_BlurScreenTime);
		yield return new WaitForSeconds(this.m_BlurScreenTime);
		this.m_clickCatcher.gameObject.SetActive(true);
		this.m_clickCaptured = false;
		this.m_overlayText.UpdateText(1);
		this.m_overlayText.Show();
		yield return new WaitForSeconds(this.m_TextDelayTime);
		while (!this.m_clickCaptured)
		{
			yield return null;
		}
		if (clickSound != null)
		{
			SoundManager.Get().Play(clickSound);
		}
		this.m_clickCatcher.gameObject.SetActive(false);
		this.m_overlayText.Hide();
		if (this.m_Stage3Sound != null)
		{
			SoundManager.Get().Play(Object.Instantiate<AudioSource>(this.m_Stage3Sound));
		}
		this.MoveButtonUp();
		yield return new WaitForSeconds(this.m_TextDelayTime);
		this.m_clickCatcher.gameObject.SetActive(true);
		this.m_clickCaptured = false;
		this.ShowButtonBanner();
		this.ShowButtonYellowGlow();
		TournamentDisplay.Get().SetRotationSlideIn();
		this.FadeOutAmbientSound();
		while (!this.m_clickCaptured)
		{
			yield return null;
		}
		if (clickSound != null)
		{
			SoundManager.Get().Play(clickSound);
		}
		this.m_clickCatcher.gameObject.SetActive(false);
		if (this.m_Stage4Sound != null)
		{
			SoundManager.Get().Play(Object.Instantiate<AudioSource>(this.m_Stage4Sound));
		}
		this.m_clickCatcher.gameObject.SetActive(true);
		this.m_clickCaptured = false;
		this.FlipButton();
		this.ButtonBannerCrossFadeText();
		this.CrossFadeToGreenGlow();
		this.ButtonBannerPunch();
		while (!this.m_clickCaptured)
		{
			yield return null;
		}
		if (clickSound != null)
		{
			SoundManager.Get().Play(clickSound);
		}
		this.m_clickCatcher.gameObject.SetActive(false);
		if (this.m_Stage5Sound != null)
		{
			SoundManager.Get().Play(Object.Instantiate<AudioSource>(this.m_Stage5Sound));
		}
		this.HideButtonBanner();
		this.StopBlurBackground(this.m_EndBlurScreenTime);
		this.MoveButtonToDeckPickerTray();
		yield return new WaitForSeconds(this.m_ButtonToTrayAnimTime + this.m_ButtonGlowAnimation.keys[this.m_ButtonGlowAnimation.length - 1].time);
		yield break;
	}

	// Token: 0x060021E3 RID: 8675 RVA: 0x000A65D0 File Offset: 0x000A47D0
	private void FadeInAmbientSound()
	{
		if (this.m_TheClockAmbientSound == null)
		{
			return;
		}
		this.m_ambientSound = Object.Instantiate<AudioSource>(this.m_TheClockAmbientSound);
		SoundManager.Get().SetVolume(this.m_ambientSound, 0.01f);
		Action<object> action = delegate(object amount)
		{
			SoundManager.Get().SetVolume(this.m_ambientSound, (float)amount);
			Log.Kyle.Print("ambient vol: {0}, {1}", new object[]
			{
				this.m_ambientSound.volume,
				(float)amount
			});
		};
		iTween.ValueTo(base.gameObject, iTween.Hash(new object[]
		{
			"name",
			"TheClockAmbientSound",
			"from",
			0.01f,
			"to",
			this.m_TheClockAmbientSoundVolume,
			"time",
			this.m_TheClockAmbientSoundFadeInTime,
			"easetype",
			iTween.EaseType.linear,
			"onupdate",
			action,
			"onupdatetarget",
			base.gameObject
		}));
		SoundManager.Get().Play(this.m_ambientSound);
	}

	// Token: 0x060021E4 RID: 8676 RVA: 0x000A66D0 File Offset: 0x000A48D0
	private void FadeOutAmbientSound()
	{
		if (this.m_ambientSound == null)
		{
			return;
		}
		Action<object> action = delegate(object amount)
		{
			SoundManager.Get().SetVolume(this.m_ambientSound, (float)amount);
		};
		iTween.ValueTo(base.gameObject, iTween.Hash(new object[]
		{
			"name",
			"TheClockAmbientSound",
			"from",
			this.m_TheClockAmbientSoundVolume,
			"to",
			0f,
			"time",
			this.m_TheClockAmbientSoundFadeOutTime,
			"easetype",
			iTween.EaseType.linear,
			"onupdate",
			action,
			"onupdatetarget",
			base.gameObject,
			"oncompletetarget",
			base.gameObject,
			"oncomplete",
			"StopAmbientSound"
		}));
	}

	// Token: 0x060021E5 RID: 8677 RVA: 0x000A67BD File Offset: 0x000A49BD
	private void StopAmbientSound()
	{
		if (this.m_ambientSound == null)
		{
			return;
		}
		SoundManager.Get().Stop(this.m_ambientSound);
	}

	// Token: 0x060021E6 RID: 8678 RVA: 0x000A67E4 File Offset: 0x000A49E4
	private void PlayClockAnimation()
	{
		Animator component = base.GetComponent<Animator>();
		if (component == null)
		{
			return;
		}
		component.SetTrigger("StartClock");
	}

	// Token: 0x060021E7 RID: 8679 RVA: 0x000A6810 File Offset: 0x000A4A10
	private void AnimateButtonToTournamentTray()
	{
		TournamentDisplay.Get().SetRotationSlideIn();
	}

	// Token: 0x060021E8 RID: 8680 RVA: 0x000A681C File Offset: 0x000A4A1C
	private void FlipCenterPanelButton()
	{
		iTween.RotateTo(this.m_CenterPanel, iTween.Hash(new object[]
		{
			"z",
			180f,
			"time",
			this.m_CenterPanelFlipTime,
			"islocal",
			true,
			"easetype",
			iTween.EaseType.easeOutBounce
		}));
		this.m_SetRotationButton.transform.localEulerAngles = new Vector3(0f, 0f, -10f);
		iTween.RotateTo(this.m_SetRotationButton, iTween.Hash(new object[]
		{
			"z",
			0f,
			"delay",
			this.m_SetRotationButtonDelay,
			"time",
			this.m_SetRotationButtonWobbleTime,
			"islocal",
			true,
			"easetype",
			iTween.EaseType.easeOutBounce
		}));
	}

	// Token: 0x060021E9 RID: 8681 RVA: 0x000A6930 File Offset: 0x000A4B30
	private void RaiseButton()
	{
		Animator component = base.GetComponent<Animator>();
		component.SetTrigger("RaiseButton");
		SceneUtils.SetLayer(this.m_SetRotationButton, GameLayer.IgnoreFullScreenEffects);
		iTween.MoveTo(this.m_SetRotationButton, iTween.Hash(new object[]
		{
			"position",
			this.m_ButtonRiseBone.transform.position,
			"delay",
			0f,
			"time",
			this.m_ButtonRiseTime,
			"islocal",
			false,
			"easetype",
			iTween.EaseType.easeInOutQuint,
			"oncompletetarget",
			base.gameObject,
			"oncomplete",
			"RaiseButtonComplete"
		}));
	}

	// Token: 0x060021EA RID: 8682 RVA: 0x000A6A08 File Offset: 0x000A4C08
	private void RaiseButtonComplete()
	{
		TokyoDrift componentInChildren = this.m_SetRotationButton.GetComponentInChildren<TokyoDrift>();
		if (componentInChildren == null)
		{
			return;
		}
		componentInChildren.enabled = true;
	}

	// Token: 0x060021EB RID: 8683 RVA: 0x000A6A38 File Offset: 0x000A4C38
	private void StopButtonDrift()
	{
		this.m_ButtonBanner.SetActive(false);
		this.m_ButtonBannerStandard.gameObject.SetActive(false);
		this.m_ButtonBannerWild.gameObject.SetActive(false);
		TokyoDrift componentInChildren = this.m_SetRotationButton.GetComponentInChildren<TokyoDrift>();
		if (componentInChildren == null)
		{
			return;
		}
		componentInChildren.enabled = false;
	}

	// Token: 0x060021EC RID: 8684 RVA: 0x000A6A94 File Offset: 0x000A4C94
	private void ShowButtonBanner()
	{
		this.m_ButtonBanner.SetActive(true);
		this.m_ButtonBannerStandard.gameObject.SetActive(true);
		this.m_ButtonBanner.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
		iTween.ScaleTo(this.m_ButtonBanner, iTween.Hash(new object[]
		{
			"scale",
			this.m_buttonBannerScale,
			"time",
			0.15f,
			"easetype",
			iTween.EaseType.easeOutQuad
		}));
	}

	// Token: 0x060021ED RID: 8685 RVA: 0x000A6B34 File Offset: 0x000A4D34
	private void ShowButtonYellowGlow()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"islocal",
			true,
			"from",
			0f,
			"to",
			1f,
			"time",
			0.3f,
			"easeType",
			iTween.EaseType.easeOutExpo,
			"onupdate",
			delegate(object value)
			{
				this.m_ButtonGlowPlaneYellow.GetComponent<Renderer>().material.SetFloat("_Intensity", (float)value);
			},
			"onupdatetarget",
			base.gameObject
		});
		iTween.ValueTo(base.gameObject, args);
	}

	// Token: 0x060021EE RID: 8686 RVA: 0x000A6BEC File Offset: 0x000A4DEC
	private void CrossFadeToGreenGlow()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"islocal",
			true,
			"from",
			1f,
			"to",
			0f,
			"time",
			0.3f,
			"easeType",
			iTween.EaseType.easeOutExpo,
			"onupdate",
			delegate(object value)
			{
				this.m_ButtonGlowPlaneYellow.GetComponent<Renderer>().material.SetFloat("_Intensity", (float)value);
			},
			"onupdatetarget",
			this.m_ButtonGlowPlaneYellow
		});
		iTween.ValueTo(base.gameObject, args);
		Hashtable args2 = iTween.Hash(new object[]
		{
			"islocal",
			true,
			"from",
			0f,
			"to",
			1f,
			"time",
			0.3f,
			"easeType",
			iTween.EaseType.easeOutExpo,
			"onupdate",
			delegate(object value)
			{
				this.m_ButtonGlowPlaneGreen.GetComponent<Renderer>().material.SetFloat("_Intensity", (float)value);
			},
			"onupdatetarget",
			base.gameObject
		});
		iTween.ValueTo(this.m_ButtonGlowPlaneGreen, args2);
	}

	// Token: 0x060021EF RID: 8687 RVA: 0x000A6D4C File Offset: 0x000A4F4C
	private void ButtonBannerCrossFadeText()
	{
		this.m_ButtonBannerStandard.gameObject.SetActive(true);
		this.m_ButtonBannerWild.gameObject.SetActive(true);
		Color textColor = this.m_ButtonBannerWild.TextColor;
		textColor.a = 0f;
		this.m_ButtonBannerWild.TextColor = textColor;
		iTween.FadeTo(this.m_ButtonBannerStandard.gameObject, 0f, this.m_ButtonFlipTime * 0.1f);
		iTween.FadeTo(this.m_ButtonBannerWild.gameObject, 1f, this.m_ButtonFlipTime * 0.1f);
	}

	// Token: 0x060021F0 RID: 8688 RVA: 0x000A6DE4 File Offset: 0x000A4FE4
	private void ButtonBannerPunch()
	{
		Vector3 localScale = this.m_ButtonBanner.transform.localScale;
		iTween.ScaleTo(this.m_ButtonBanner, iTween.Hash(new object[]
		{
			"scale",
			localScale * 1.5f,
			"time",
			0.075f,
			"delay",
			this.m_ButtonFlipTime * 0.25f,
			"easetype",
			iTween.EaseType.easeOutQuad,
			"onupdatetarget",
			base.gameObject
		}));
		iTween.ScaleTo(this.m_ButtonBanner, iTween.Hash(new object[]
		{
			"scale",
			localScale,
			"time",
			0.25f,
			"delay",
			this.m_ButtonFlipTime * 0.25f + 0.075f,
			"easetype",
			iTween.EaseType.easeInOutQuad,
			"onupdatetarget",
			base.gameObject
		}));
	}

	// Token: 0x060021F1 RID: 8689 RVA: 0x000A6F0C File Offset: 0x000A510C
	private void HideButtonBanner()
	{
		iTween.ScaleTo(this.m_ButtonBanner, iTween.Hash(new object[]
		{
			"scale",
			Vector3.zero,
			"time",
			0.25f,
			"easetype",
			iTween.EaseType.easeInQuad,
			"oncompletetarget",
			base.gameObject,
			"oncomplete",
			"HideButtonBannerComplete"
		}));
	}

	// Token: 0x060021F2 RID: 8690 RVA: 0x000A6F8D File Offset: 0x000A518D
	private void HideButtonBannerComplete()
	{
		this.m_ButtonBanner.SetActive(false);
	}

	// Token: 0x060021F3 RID: 8691 RVA: 0x000A6F9C File Offset: 0x000A519C
	private void FlipButton()
	{
		iTween.RotateTo(this.m_SetRotationButtonMesh, iTween.Hash(new object[]
		{
			"z",
			0f,
			"time",
			this.m_ButtonFlipTime,
			"islocal",
			true,
			"easetype",
			iTween.EaseType.easeOutElastic
		}));
	}

	// Token: 0x060021F4 RID: 8692 RVA: 0x000A7010 File Offset: 0x000A5210
	private void MoveButtonUp()
	{
		float num = this.m_MoveButtonUpZ;
		if (UniversalInputManager.UsePhoneUI)
		{
			num = this.m_MoveButtonUpZphone;
		}
		iTween.MoveTo(this.m_SetRotationButton, iTween.Hash(new object[]
		{
			"z",
			num,
			"delay",
			0f,
			"time",
			this.m_MoveButtonUpTime,
			"islocal",
			true,
			"easetype",
			iTween.EaseType.easeInOutQuint
		}));
	}

	// Token: 0x060021F5 RID: 8693 RVA: 0x000A70B4 File Offset: 0x000A52B4
	private void VignetteBackground(float time)
	{
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		if (fullScreenFXMgr == null)
		{
			Debug.LogWarning("FullScreenFXMgr is NULL!");
			return;
		}
		fullScreenFXMgr.Vignette(0.99f, time, iTween.EaseType.easeOutCubic, null);
	}

	// Token: 0x060021F6 RID: 8694 RVA: 0x000A70EC File Offset: 0x000A52EC
	private void StopVignetteBackground(float time)
	{
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		if (fullScreenFXMgr == null)
		{
			Debug.LogWarning("FullScreenFXMgr is NULL!");
			return;
		}
		fullScreenFXMgr.Vignette(0f, time, iTween.EaseType.easeInCubic, null);
	}

	// Token: 0x060021F7 RID: 8695 RVA: 0x000A7124 File Offset: 0x000A5324
	private void BlurBackground(float time)
	{
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		if (fullScreenFXMgr == null)
		{
			Debug.LogWarning("FullScreenFXMgr is NULL!");
			return;
		}
		fullScreenFXMgr.StartStandardBlurVignette(time);
	}

	// Token: 0x060021F8 RID: 8696 RVA: 0x000A7158 File Offset: 0x000A5358
	private void StopBlurBackground(float time)
	{
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		if (fullScreenFXMgr == null)
		{
			Debug.LogWarning("FullScreenFXMgr is NULL!");
			return;
		}
		fullScreenFXMgr.EndStandardBlurVignette(time, null);
	}

	// Token: 0x060021F9 RID: 8697 RVA: 0x000A718C File Offset: 0x000A538C
	private void MoveButtonToDeckPickerTray()
	{
		this.StopButtonDrift();
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.one;
		DeckPickerTrayDisplay deckPickerTrayDisplay = DeckPickerTrayDisplay.Get();
		GameObject theClockButtonBone = deckPickerTrayDisplay.m_TheClockButtonBone;
		if (theClockButtonBone != null)
		{
			vector = theClockButtonBone.transform.position;
			vector2 = theClockButtonBone.transform.localScale;
		}
		Vector3 vector3 = Vector3.Lerp(this.m_SetRotationButton.transform.position, vector, 0.75f);
		vector3..ctor(vector3.x + 7f, vector3.y, vector3.z);
		Vector3[] array = new Vector3[]
		{
			this.m_SetRotationButton.transform.position,
			vector3,
			vector
		};
		Animator component = base.GetComponent<Animator>();
		component.SetTrigger("SocketButton");
		Hashtable args = iTween.Hash(new object[]
		{
			"path",
			array,
			"delay",
			0f,
			"time",
			this.m_ButtonToTrayAnimTime,
			"islocal",
			false,
			"easetype",
			iTween.EaseType.easeInOutQuint,
			"oncompletetarget",
			base.gameObject,
			"oncomplete",
			"ButtonImpactAndShutdownTheClock"
		});
		iTween.MoveTo(this.m_SetRotationButton, args);
		iTween.RotateTo(this.m_SetRotationButtonMesh, iTween.Hash(new object[]
		{
			"rotation",
			Vector3.zero,
			"time",
			this.m_ButtonToTrayAnimTime,
			"islocal",
			true,
			"easetype",
			iTween.EaseType.easeInOutQuint
		}));
		iTween.RotateTo(this.m_SetRotationButton, iTween.Hash(new object[]
		{
			"rotation",
			Vector3.zero,
			"time",
			this.m_ButtonToTrayAnimTime,
			"islocal",
			true,
			"easetype",
			iTween.EaseType.easeInOutQuint
		}));
		iTween.ScaleTo(this.m_SetRotationButton, iTween.Hash(new object[]
		{
			"scale",
			vector2,
			"delay",
			0f,
			"time",
			this.m_ButtonToTrayAnimTime,
			"easetype",
			iTween.EaseType.easeInOutQuint
		}));
	}

	// Token: 0x060021FA RID: 8698 RVA: 0x000A743C File Offset: 0x000A563C
	private void ButtonImpactAndShutdownTheClock()
	{
		this.ShakeCamera();
		this.m_ImpactParticles.Play();
		base.StartCoroutine(this.FinalGlowAndDisableTheClock());
	}

	// Token: 0x060021FB RID: 8699 RVA: 0x000A7468 File Offset: 0x000A5668
	private IEnumerator FinalGlowAndDisableTheClock()
	{
		this.m_SetRotationButtonMesh.GetComponent<Renderer>().enabled = false;
		this.EndClockStartTutorial();
		Renderer glowRenderer = this.m_ButtonGlowPlaneGreen.GetComponent<Renderer>();
		Material glowMat = glowRenderer.material;
		float animLength = this.m_ButtonGlowAnimation.keys[this.m_ButtonGlowAnimation.length - 1].time;
		float animTime = 0f;
		while (animTime < animLength)
		{
			animTime += Time.deltaTime;
			glowMat.SetFloat("_Intensity", this.m_ButtonGlowAnimation.Evaluate(animTime));
			yield return null;
		}
		yield return new WaitForSeconds(3f);
		base.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x060021FC RID: 8700 RVA: 0x000A7484 File Offset: 0x000A5684
	private void EndClockStartTutorial()
	{
		Options.Get().SetInt(Option.SET_ROTATION_INTRO_PROGRESS, 1);
		Options.Get().SetBool(Option.SHOW_SET_ROTATION_INTRO_VISUALS, false);
		DeckPickerTrayDisplay.Get().StartSetRotationTutorial();
	}

	// Token: 0x060021FD RID: 8701 RVA: 0x000A74B5 File Offset: 0x000A56B5
	private void OnClick(UIEvent e)
	{
		this.m_clickCaptured = true;
	}

	// Token: 0x04001347 RID: 4935
	public float m_AnimationWaitTime = 5.5f;

	// Token: 0x04001348 RID: 4936
	public GameObject m_CenterPanel;

	// Token: 0x04001349 RID: 4937
	public float m_CenterPanelFlipTime = 1f;

	// Token: 0x0400134A RID: 4938
	public GameObject m_SetRotationButton;

	// Token: 0x0400134B RID: 4939
	public GameObject m_SetRotationButtonMesh;

	// Token: 0x0400134C RID: 4940
	public float m_SetRotationButtonDelay = 0.75f;

	// Token: 0x0400134D RID: 4941
	public float m_SetRotationButtonWobbleTime = 0.5f;

	// Token: 0x0400134E RID: 4942
	public float m_ButtonRotationHoldTime = 1.5f;

	// Token: 0x0400134F RID: 4943
	public GameObject m_ButtonRiseBone;

	// Token: 0x04001350 RID: 4944
	public GameObject m_ButtonBanner;

	// Token: 0x04001351 RID: 4945
	public UberText m_ButtonBannerStandard;

	// Token: 0x04001352 RID: 4946
	public UberText m_ButtonBannerWild;

	// Token: 0x04001353 RID: 4947
	public Color m_ButtonBannerTextColor = Color.white;

	// Token: 0x04001354 RID: 4948
	public float m_ButtonRiseTime = 1.75f;

	// Token: 0x04001355 RID: 4949
	public float m_BlurScreenDelay = 0.5f;

	// Token: 0x04001356 RID: 4950
	public float m_BlurScreenTime = 1f;

	// Token: 0x04001357 RID: 4951
	public float m_MoveButtonUpZ = -0.1f;

	// Token: 0x04001358 RID: 4952
	public float m_MoveButtonUpZphone = -0.3f;

	// Token: 0x04001359 RID: 4953
	public float m_MoveButtonUpTime = 1f;

	// Token: 0x0400135A RID: 4954
	public float m_ButtonFlipTime = 0.5f;

	// Token: 0x0400135B RID: 4955
	public float m_ButtonToTrayAnimTime = 0.5f;

	// Token: 0x0400135C RID: 4956
	public float m_EndBlurScreenDelay = 0.5f;

	// Token: 0x0400135D RID: 4957
	public float m_EndBlurScreenTime = 1f;

	// Token: 0x0400135E RID: 4958
	public float m_MoveButtonToTrayDelay = 1.5f;

	// Token: 0x0400135F RID: 4959
	public float m_TextDelayTime = 1f;

	// Token: 0x04001360 RID: 4960
	public ClockOverlayText m_overlayText;

	// Token: 0x04001361 RID: 4961
	public GameObject m_ButtonGlowPlaneYellow;

	// Token: 0x04001362 RID: 4962
	public GameObject m_ButtonGlowPlaneGreen;

	// Token: 0x04001363 RID: 4963
	public ParticleSystem m_ImpactParticles;

	// Token: 0x04001364 RID: 4964
	public AnimationCurve m_ButtonGlowAnimation;

	// Token: 0x04001365 RID: 4965
	public PegUIElement m_clickCatcher;

	// Token: 0x04001366 RID: 4966
	public AudioSource m_TheClockAmbientSound;

	// Token: 0x04001367 RID: 4967
	public float m_TheClockAmbientSoundVolume = 1f;

	// Token: 0x04001368 RID: 4968
	public float m_TheClockAmbientSoundFadeInTime = 2f;

	// Token: 0x04001369 RID: 4969
	public float m_TheClockAmbientSoundFadeOutTime = 1f;

	// Token: 0x0400136A RID: 4970
	public AudioSource m_ClickSound;

	// Token: 0x0400136B RID: 4971
	public AudioSource m_Stage1Sound;

	// Token: 0x0400136C RID: 4972
	public AudioSource m_Stage2Sound;

	// Token: 0x0400136D RID: 4973
	public AudioSource m_Stage3Sound;

	// Token: 0x0400136E RID: 4974
	public AudioSource m_Stage4Sound;

	// Token: 0x0400136F RID: 4975
	public AudioSource m_Stage5Sound;

	// Token: 0x04001370 RID: 4976
	private bool m_clickCaptured;

	// Token: 0x04001371 RID: 4977
	private Vector3 m_buttonBannerScale;

	// Token: 0x04001372 RID: 4978
	private AudioSource m_ambientSound;

	// Token: 0x04001373 RID: 4979
	private static SetRotationClock s_instance;
}
