using System;
using UnityEngine;

// Token: 0x020002A8 RID: 680
public class GemObject : MonoBehaviour
{
	// Token: 0x06002558 RID: 9560 RVA: 0x000B8005 File Offset: 0x000B6205
	private void Awake()
	{
		this.startingScale = base.transform.localScale;
	}

	// Token: 0x06002559 RID: 9561 RVA: 0x000B8018 File Offset: 0x000B6218
	public void Enlarge(float scaleFactor)
	{
		iTween.Stop(base.gameObject);
		iTween.ScaleTo(base.gameObject, iTween.Hash(new object[]
		{
			"scale",
			new Vector3(this.startingScale.x * scaleFactor, this.startingScale.y * scaleFactor, this.startingScale.z * scaleFactor),
			"time",
			0.5f,
			"easetype",
			iTween.EaseType.easeOutElastic
		}));
	}

	// Token: 0x0600255A RID: 9562 RVA: 0x000B80A9 File Offset: 0x000B62A9
	public void Shrink()
	{
		iTween.ScaleTo(base.gameObject, this.startingScale, 0.5f);
	}

	// Token: 0x0600255B RID: 9563 RVA: 0x000B80C4 File Offset: 0x000B62C4
	public void ScaleToZero()
	{
		iTween.Stop(base.gameObject);
		iTween.ScaleTo(base.gameObject, Vector3.zero, 0.5f);
	}

	// Token: 0x0600255C RID: 9564 RVA: 0x000B80F1 File Offset: 0x000B62F1
	public void SetToZeroThenEnlarge()
	{
		base.transform.localScale = Vector3.zero;
		this.Enlarge(1f);
	}

	// Token: 0x0600255D RID: 9565 RVA: 0x000B810E File Offset: 0x000B630E
	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600255E RID: 9566 RVA: 0x000B811C File Offset: 0x000B631C
	public void Show()
	{
		base.gameObject.SetActive(true);
	}

	// Token: 0x0600255F RID: 9567 RVA: 0x000B812A File Offset: 0x000B632A
	public void Initialize()
	{
		this.initialized = true;
	}

	// Token: 0x06002560 RID: 9568 RVA: 0x000B8133 File Offset: 0x000B6333
	public void SetHideNumberFlag(bool enable)
	{
		this.m_hiddenFlag = enable;
	}

	// Token: 0x06002561 RID: 9569 RVA: 0x000B813C File Offset: 0x000B633C
	public bool IsNumberHidden()
	{
		return this.m_hiddenFlag;
	}

	// Token: 0x06002562 RID: 9570 RVA: 0x000B8144 File Offset: 0x000B6344
	public void Jiggle()
	{
		if (!this.initialized)
		{
			this.initialized = true;
			return;
		}
		iTween.Stop(base.gameObject);
		base.transform.localScale = this.startingScale;
		iTween.PunchScale(base.gameObject, new Vector3(this.jiggleAmount, this.jiggleAmount, this.jiggleAmount), 1f);
	}

	// Token: 0x04001614 RID: 5652
	public Vector3 startingScale;

	// Token: 0x04001615 RID: 5653
	public float jiggleAmount;

	// Token: 0x04001616 RID: 5654
	private bool initialized;

	// Token: 0x04001617 RID: 5655
	private bool m_hiddenFlag;
}
