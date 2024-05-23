using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000545 RID: 1349
[CustomEditClass]
public class GeneralStorePhoneCover : MonoBehaviour
{
	// Token: 0x06003E21 RID: 15905 RVA: 0x0012CA84 File Offset: 0x0012AC84
	private void Awake()
	{
		GeneralStorePhoneCover.s_instance = this;
		StoreManager.Get().RegisterStoreShownListener(new StoreManager.StoreShownCallback(this.UpdateCoverPosition));
		this.m_parentStore.RegisterModeChangedListener(new GeneralStore.ModeChanged(this.OnGeneralStoreModeChanged));
		this.m_backToCoverButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			Navigation.GoBack();
		});
	}

	// Token: 0x06003E22 RID: 15906 RVA: 0x0012CAEF File Offset: 0x0012ACEF
	private void OnDestroy()
	{
		GeneralStorePhoneCover.s_instance = null;
		StoreManager.Get().RemoveStoreShownListener(new StoreManager.StoreShownCallback(this.UpdateCoverPosition));
	}

	// Token: 0x06003E23 RID: 15907 RVA: 0x0012CB0E File Offset: 0x0012AD0E
	private void Start()
	{
		this.ShowCover();
	}

	// Token: 0x06003E24 RID: 15908 RVA: 0x0012CB16 File Offset: 0x0012AD16
	public void ShowCover()
	{
		base.StopCoroutine("PlayAndWaitForAnimation");
		base.StartCoroutine("PlayAndWaitForAnimation", this.m_buttonEnterAnimation);
		this.m_coverClickArea.SetActive(true);
	}

	// Token: 0x06003E25 RID: 15909 RVA: 0x0012CB44 File Offset: 0x0012AD44
	public void HideCover(GeneralStoreMode selectedMode)
	{
		base.StartCoroutine(this.PushBackMethodWhenShown());
		GeneralStorePhoneCover.ModeAnimation modeAnimation = this.m_buttonExitAnimations.Find((GeneralStorePhoneCover.ModeAnimation o) => o.m_mode == selectedMode);
		if (modeAnimation == null)
		{
			Debug.LogError(string.Format("Unable to find animation for {0} mode.", selectedMode));
			return;
		}
		if (string.IsNullOrEmpty(modeAnimation.m_playAnimationName))
		{
			Debug.LogError(string.Format("Animation name not defined for {0} mode.", selectedMode));
			return;
		}
		base.StopCoroutine("PlayAndWaitForAnimation");
		base.StartCoroutine("PlayAndWaitForAnimation", modeAnimation.m_playAnimationName);
		this.m_coverClickArea.SetActive(false);
	}

	// Token: 0x06003E26 RID: 15910 RVA: 0x0012CBF8 File Offset: 0x0012ADF8
	private IEnumerator PushBackMethodWhenShown()
	{
		while (!this.m_parentStore.IsShown())
		{
			yield return null;
		}
		Navigation.Push(new Navigation.NavigateBackHandler(GeneralStorePhoneCover.OnNavigateBack));
		yield break;
	}

	// Token: 0x06003E27 RID: 15911 RVA: 0x0012CC13 File Offset: 0x0012AE13
	private void OnGeneralStoreModeChanged(GeneralStoreMode oldMode, GeneralStoreMode newMode)
	{
		if (newMode != GeneralStoreMode.NONE)
		{
			this.HideCover(newMode);
		}
		else
		{
			this.ShowCover();
		}
	}

	// Token: 0x06003E28 RID: 15912 RVA: 0x0012CC30 File Offset: 0x0012AE30
	private IEnumerator PlayAndWaitForAnimation(string animationName)
	{
		this.m_animationController.enabled = true;
		this.m_animationController.StopPlayback();
		this.m_animationClickBlocker.SetActive(true);
		yield return new WaitForEndOfFrame();
		this.m_animationController.Play(animationName);
		while (this.m_animationController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
		{
			yield return null;
		}
		this.m_animationClickBlocker.SetActive(false);
		yield break;
	}

	// Token: 0x06003E29 RID: 15913 RVA: 0x0012CC59 File Offset: 0x0012AE59
	private void UpdateCoverPosition(object data)
	{
		TransformUtil.SetLocalPosY(this, TransformUtil.GetAspectRatioDependentValue(this.m_yPos3to2, this.m_yPos16to9));
	}

	// Token: 0x06003E2A RID: 15914 RVA: 0x0012CC72 File Offset: 0x0012AE72
	public static bool OnNavigateBack()
	{
		if (GeneralStorePhoneCover.s_instance == null)
		{
			return false;
		}
		GeneralStorePhoneCover.s_instance.m_parentStore.SetMode(GeneralStoreMode.NONE);
		return true;
	}

	// Token: 0x040027B2 RID: 10162
	private const string s_coverAnimationCoroutine = "PlayAndWaitForAnimation";

	// Token: 0x040027B3 RID: 10163
	[CustomEditField(Sections = "General UI")]
	public GeneralStore m_parentStore;

	// Token: 0x040027B4 RID: 10164
	[CustomEditField(Sections = "General UI")]
	public PegUIElement m_backToCoverButton;

	// Token: 0x040027B5 RID: 10165
	[CustomEditField(Sections = "Animation")]
	public Animator m_animationController;

	// Token: 0x040027B6 RID: 10166
	[CustomEditField(Sections = "Animation")]
	public string m_buttonEnterAnimation = string.Empty;

	// Token: 0x040027B7 RID: 10167
	[CustomEditField(Sections = "Animation")]
	public List<GeneralStorePhoneCover.ModeAnimation> m_buttonExitAnimations = new List<GeneralStorePhoneCover.ModeAnimation>();

	// Token: 0x040027B8 RID: 10168
	[CustomEditField(Sections = "UI Blockers")]
	public GameObject m_coverClickArea;

	// Token: 0x040027B9 RID: 10169
	[CustomEditField(Sections = "UI Blockers")]
	public GameObject m_animationClickBlocker;

	// Token: 0x040027BA RID: 10170
	[CustomEditField(Sections = "Aspect Ratio Positioning")]
	public float m_yPos3to2;

	// Token: 0x040027BB RID: 10171
	[CustomEditField(Sections = "Aspect Ratio Positioning")]
	public float m_yPos16to9;

	// Token: 0x040027BC RID: 10172
	private static GeneralStorePhoneCover s_instance;

	// Token: 0x02000AC6 RID: 2758
	[Serializable]
	public class ModeAnimation
	{
		// Token: 0x040046AC RID: 18092
		public GeneralStoreMode m_mode;

		// Token: 0x040046AD RID: 18093
		public string m_playAnimationName;
	}

	// Token: 0x02000AC7 RID: 2759
	// (Invoke) Token: 0x06005F3B RID: 24379
	public delegate void AnimationCallback();
}
