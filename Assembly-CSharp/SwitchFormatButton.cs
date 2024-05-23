using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000393 RID: 915
public class SwitchFormatButton : UIBButton
{
	// Token: 0x06002FCB RID: 12235 RVA: 0x000F068E File Offset: 0x000EE88E
	protected override void Awake()
	{
		base.Awake();
	}

	// Token: 0x06002FCC RID: 12236 RVA: 0x000F0698 File Offset: 0x000EE898
	public void SetFormat(bool isWild, bool doAnimation)
	{
		if (this.m_isWild != isWild)
		{
			this.m_isWild = isWild;
			GameObject gameObject = this.m_buttonRenderer.gameObject;
			iTween.Stop(gameObject);
			if (this.m_isRotating)
			{
				this.m_isRotating = false;
				this.EnableHighlightImpl(this.m_cachedHighlightEnabled);
			}
			if (doAnimation)
			{
				this.m_isRotating = true;
				this.m_cachedHighlightEnabled = this.m_isHighlightEnabled;
				this.EnableHighlightImpl(false);
				this.HidePopUp();
				Vector3 eulerAngles = gameObject.transform.localRotation.eulerAngles;
				eulerAngles.z = (float)((!this.m_isWild) ? 180 : 0);
				gameObject.transform.localRotation = Quaternion.Euler(eulerAngles);
				Hashtable args = iTween.Hash(new object[]
				{
					"z",
					-180,
					"time",
					0.5f,
					"oncomplete",
					"OnRotateComplete",
					"oncompletetarget",
					base.gameObject
				});
				iTween.RotateAdd(gameObject, args);
			}
			else
			{
				Vector3 eulerAngles2 = gameObject.transform.rotation.eulerAngles;
				eulerAngles2.z = (float)((!this.m_isWild) ? 0 : 180);
				gameObject.transform.rotation = Quaternion.Euler(eulerAngles2);
			}
		}
	}

	// Token: 0x06002FCD RID: 12237 RVA: 0x000F07F9 File Offset: 0x000EE9F9
	private void OnRotateComplete()
	{
		this.m_isRotating = false;
		this.EnableHighlightImpl(this.m_cachedHighlightEnabled);
	}

	// Token: 0x06002FCE RID: 12238 RVA: 0x000F080E File Offset: 0x000EEA0E
	public void Disable()
	{
		this.m_uibHighlight.Reset();
		this.HidePopUp();
		this.SetEnabled(false);
	}

	// Token: 0x06002FCF RID: 12239 RVA: 0x000F0828 File Offset: 0x000EEA28
	public void Enable()
	{
		this.SetEnabled(true);
	}

	// Token: 0x06002FD0 RID: 12240 RVA: 0x000F0831 File Offset: 0x000EEA31
	public bool IsRotating()
	{
		return this.m_isRotating;
	}

	// Token: 0x06002FD1 RID: 12241 RVA: 0x000F0839 File Offset: 0x000EEA39
	public void Cover()
	{
		if (this.m_coverObject != null)
		{
			this.m_coverObject.SetActive(true);
		}
	}

	// Token: 0x06002FD2 RID: 12242 RVA: 0x000F0858 File Offset: 0x000EEA58
	public void Uncover()
	{
		if (this.m_coverObject != null)
		{
			this.m_coverObject.SetActive(false);
		}
	}

	// Token: 0x06002FD3 RID: 12243 RVA: 0x000F0878 File Offset: 0x000EEA78
	public void ShowPopUp()
	{
		if (this.m_popUpObject == null)
		{
			this.m_popUpObject = Object.Instantiate<GameObject>(this.m_popUpPrefab);
			Transform transform = (!UniversalInputManager.UsePhoneUI) ? this.m_popUpBone : this.m_popUpBonePhone;
			Vector3 relativePosition = OverlayUI.Get().GetRelativePosition(transform.position, Box.Get().m_Camera.GetComponent<Camera>(), OverlayUI.Get().m_heightScale.m_Center, 1f);
			this.m_popUpObject.transform.parent = OverlayUI.Get().m_heightScale.m_Center;
			this.m_popUpObject.transform.localPosition = relativePosition;
			this.m_popUpObject.transform.localScale = transform.localScale;
			this.m_popUpObject.SetActive(false);
		}
		int @int = Options.Get().GetInt(Option.TIMES_MOUSED_OVER_SWITCH_FORMAT_BUTTON);
		if (@int < SwitchFormatButton.TIMES_MOUSED_OVER_THRESHOLD)
		{
			this.m_popUpObject.SetActive(true);
			Options.Get().SetInt(Option.TIMES_MOUSED_OVER_SWITCH_FORMAT_BUTTON, @int + 1);
		}
		else
		{
			base.StartCoroutine("ShowPopUpAfterDelay");
		}
	}

	// Token: 0x06002FD4 RID: 12244 RVA: 0x000F0994 File Offset: 0x000EEB94
	private IEnumerator ShowPopUpAfterDelay()
	{
		yield return new WaitForSeconds(SwitchFormatButton.SHOW_POP_UP_DELAY_SEC);
		int numTimes = Options.Get().GetInt(Option.TIMES_MOUSED_OVER_SWITCH_FORMAT_BUTTON);
		Options.Get().SetInt(Option.TIMES_MOUSED_OVER_SWITCH_FORMAT_BUTTON, numTimes + 1);
		this.m_popUpObject.SetActive(true);
		yield break;
	}

	// Token: 0x06002FD5 RID: 12245 RVA: 0x000F09B0 File Offset: 0x000EEBB0
	public void HidePopUp()
	{
		if (this.m_popUpObject != null)
		{
			base.StopCoroutine("ShowPopUpAfterDelay");
			this.m_popUpObject.SetActive(false);
		}
	}

	// Token: 0x06002FD6 RID: 12246 RVA: 0x000F09E5 File Offset: 0x000EEBE5
	public void EnableHighlight(bool enabled)
	{
		if (!this.m_isRotating)
		{
			this.EnableHighlightImpl(enabled);
		}
		else
		{
			this.m_cachedHighlightEnabled = enabled;
		}
	}

	// Token: 0x06002FD7 RID: 12247 RVA: 0x000F0A05 File Offset: 0x000EEC05
	public void DoWildFlip()
	{
		base.StopCoroutine("DoWildFlipImpl");
		base.StartCoroutine("DoWildFlipImpl");
	}

	// Token: 0x06002FD8 RID: 12248 RVA: 0x000F0A20 File Offset: 0x000EEC20
	private IEnumerator DoWildFlipImpl()
	{
		this.SetFormat(false, false);
		this.Disable();
		yield return new WaitForSeconds(SwitchFormatButton.WILD_FLIP_DELAY_SEC);
		this.Enable();
		this.SetFormat(true, true);
		yield break;
	}

	// Token: 0x06002FD9 RID: 12249 RVA: 0x000F0A3C File Offset: 0x000EEC3C
	private void EnableHighlightImpl(bool enabled)
	{
		this.m_isHighlightEnabled = enabled;
		if (enabled)
		{
			this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
		}
		else
		{
			this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
		}
	}

	// Token: 0x04001DA6 RID: 7590
	public MeshRenderer m_buttonRenderer;

	// Token: 0x04001DA7 RID: 7591
	public HighlightState m_highlight;

	// Token: 0x04001DA8 RID: 7592
	public GameObject m_coverObject;

	// Token: 0x04001DA9 RID: 7593
	public UIBHighlight m_uibHighlight;

	// Token: 0x04001DAA RID: 7594
	public GameObject m_popUpPrefab;

	// Token: 0x04001DAB RID: 7595
	public Transform m_popUpBone;

	// Token: 0x04001DAC RID: 7596
	public Transform m_popUpBonePhone;

	// Token: 0x04001DAD RID: 7597
	private bool m_isHighlightEnabled;

	// Token: 0x04001DAE RID: 7598
	private bool m_cachedHighlightEnabled;

	// Token: 0x04001DAF RID: 7599
	private bool m_isWild;

	// Token: 0x04001DB0 RID: 7600
	private bool m_isRotating;

	// Token: 0x04001DB1 RID: 7601
	private GameObject m_popUpObject;

	// Token: 0x04001DB2 RID: 7602
	private static readonly int TIMES_MOUSED_OVER_THRESHOLD = 5;

	// Token: 0x04001DB3 RID: 7603
	private static readonly float SHOW_POP_UP_DELAY_SEC = 0.5f;

	// Token: 0x04001DB4 RID: 7604
	private static readonly float WILD_FLIP_DELAY_SEC = 1.5f;
}
