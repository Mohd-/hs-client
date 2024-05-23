using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200092E RID: 2350
public class GameplayErrorCloud : MonoBehaviour
{
	// Token: 0x060056E6 RID: 22246 RVA: 0x001A0CD8 File Offset: 0x0019EED8
	private void Start()
	{
		RenderUtils.SetAlpha(base.gameObject, 0f);
		this.Hide();
	}

	// Token: 0x060056E7 RID: 22247 RVA: 0x001A0CF0 File Offset: 0x0019EEF0
	public void Show()
	{
		this.m_emitter.gameObject.SetActive(true);
	}

	// Token: 0x060056E8 RID: 22248 RVA: 0x001A0D03 File Offset: 0x0019EF03
	public void Hide()
	{
		this.m_coroutine = null;
		this.m_emitter.gameObject.SetActive(false);
	}

	// Token: 0x060056E9 RID: 22249 RVA: 0x001A0D20 File Offset: 0x0019EF20
	public void ShowMessage(string message, float timeToDisplay)
	{
		if (this.m_coroutine != null)
		{
			base.StopCoroutine(this.START_COROUTINE_NAME);
			this.Hide();
		}
		this.m_holdDuration = Mathf.Max(2f, timeToDisplay);
		ParticleEmitter emitter = this.m_emitter;
		float num = 0.15f + this.m_holdDuration * 1.4f + 0.5f;
		this.m_emitter.minEnergy = num;
		emitter.maxEnergy = num;
		this.Show();
		this.m_errorText.Text = message;
		iTween.FadeTo(base.gameObject, iTween.Hash(new object[]
		{
			"alpha",
			1f,
			"time",
			0.15f
		}));
		this.m_coroutine = base.StartCoroutine(this.START_COROUTINE_NAME);
	}

	// Token: 0x060056EA RID: 22250 RVA: 0x001A0DF0 File Offset: 0x0019EFF0
	public void HideMessage()
	{
		iTween.FadeTo(base.gameObject, iTween.Hash(new object[]
		{
			"alpha",
			0f,
			"time",
			0.5f,
			"oncomplete",
			"Hide"
		}));
	}

	// Token: 0x060056EB RID: 22251 RVA: 0x001A0E50 File Offset: 0x0019F050
	public IEnumerator StartHideMessageDelay()
	{
		yield return new WaitForSeconds(0.15f + this.m_holdDuration);
		this.HideMessage();
		yield break;
	}

	// Token: 0x04003D9E RID: 15774
	private const float ERROR_MESSAGE_DURATION = 2f;

	// Token: 0x04003D9F RID: 15775
	private const float ERROR_MESSAGE_FADEIN = 0.15f;

	// Token: 0x04003DA0 RID: 15776
	private const float ERROR_MESSAGE_FADEOUT = 0.5f;

	// Token: 0x04003DA1 RID: 15777
	public UberText m_errorText;

	// Token: 0x04003DA2 RID: 15778
	public float initTime;

	// Token: 0x04003DA3 RID: 15779
	public ParticleEmitter m_emitter;

	// Token: 0x04003DA4 RID: 15780
	private readonly string START_COROUTINE_NAME = "StartHideMessageDelay";

	// Token: 0x04003DA5 RID: 15781
	private float m_holdDuration;

	// Token: 0x04003DA6 RID: 15782
	private Coroutine m_coroutine;
}
