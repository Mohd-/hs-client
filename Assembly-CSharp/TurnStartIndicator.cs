using System;
using UnityEngine;

// Token: 0x02000901 RID: 2305
public class TurnStartIndicator : MonoBehaviour
{
	// Token: 0x06005622 RID: 22050 RVA: 0x0019E12C File Offset: 0x0019C32C
	private void Start()
	{
		iTween.FadeTo(base.gameObject, 0f, 0f);
		base.gameObject.transform.position = new Vector3(-7.8f, 8.2f, -5f);
		base.gameObject.transform.eulerAngles = new Vector3(90f, 0f, 0f);
		base.gameObject.SetActive(false);
		this.SetReminderText(string.Empty);
	}

	// Token: 0x06005623 RID: 22051 RVA: 0x0019E1AD File Offset: 0x0019C3AD
	public bool IsShown()
	{
		return base.gameObject.activeSelf;
	}

	// Token: 0x06005624 RID: 22052 RVA: 0x0019E1BC File Offset: 0x0019C3BC
	public void Show()
	{
		if (UniversalInputManager.UsePhoneUI)
		{
			base.gameObject.transform.position = new Vector3(-7.8f, 8.2f, -4.2f);
		}
		else
		{
			base.gameObject.transform.position = new Vector3(-7.8f, 8.2f, -5f);
		}
		base.gameObject.SetActive(true);
		this.m_labelTop.Text = GameStrings.Get("GAMEPLAY_YOUR_TURN");
		this.m_labelMiddle.Text = GameStrings.Get("GAMEPLAY_YOUR_TURN");
		this.m_labelBottom.Text = GameStrings.Get("GAMEPLAY_YOUR_TURN");
		iTween.FadeTo(base.gameObject, 1f, 0.25f);
		base.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		iTween.ScaleTo(base.gameObject, iTween.Hash(new object[]
		{
			"scale",
			new Vector3(10f, 10f, 10f),
			"time",
			0.25f,
			"oncomplete",
			"PunchTurnStartInstance",
			"oncompletetarget",
			base.gameObject
		}));
		iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
		{
			"position",
			base.gameObject.transform.position + new Vector3(0.02f, 0.02f, 0.02f),
			"time",
			1.5f,
			"oncomplete",
			"HideTurnStartInstance",
			"oncompletetarget",
			base.gameObject
		}));
		this.m_explosionFX.GetComponent<ParticleSystem>().Play();
	}

	// Token: 0x06005625 RID: 22053 RVA: 0x0019E3AE File Offset: 0x0019C5AE
	public void Hide()
	{
	}

	// Token: 0x06005626 RID: 22054 RVA: 0x0019E3B0 File Offset: 0x0019C5B0
	private void PunchTurnStartInstance()
	{
		iTween.ScaleTo(base.gameObject, new Vector3(9.8f, 9.8f, 9.8f), 0.15f);
	}

	// Token: 0x06005627 RID: 22055 RVA: 0x0019E3E4 File Offset: 0x0019C5E4
	private void HideTurnStartInstance()
	{
		iTween.FadeTo(base.gameObject, 0f, 0.25f);
		iTween.ScaleTo(base.gameObject, iTween.Hash(new object[]
		{
			"scale",
			new Vector3(1f, 1f, 1f),
			"time",
			0.25f,
			"oncomplete",
			"DeactivateTurnStartInstance",
			"oncompletetarget",
			base.gameObject
		}));
	}

	// Token: 0x06005628 RID: 22056 RVA: 0x0019E476 File Offset: 0x0019C676
	private void DeactivateTurnStartInstance()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06005629 RID: 22057 RVA: 0x0019E484 File Offset: 0x0019C684
	public void SetReminderText(string newText)
	{
		this.m_reminderText.Text = newText;
	}

	// Token: 0x04003C83 RID: 15491
	private const float START_SCALE_VAL = 1f;

	// Token: 0x04003C84 RID: 15492
	private const float AFTER_PUNCH_SCALE_VAL = 9.8f;

	// Token: 0x04003C85 RID: 15493
	private const float END_SCALE_VAL = 10f;

	// Token: 0x04003C86 RID: 15494
	public GameObject m_explosionFX;

	// Token: 0x04003C87 RID: 15495
	public GameObject m_godRays;

	// Token: 0x04003C88 RID: 15496
	public UberText m_labelTop;

	// Token: 0x04003C89 RID: 15497
	public UberText m_labelMiddle;

	// Token: 0x04003C8A RID: 15498
	public UberText m_labelBottom;

	// Token: 0x04003C8B RID: 15499
	public UberText m_reminderText;
}
