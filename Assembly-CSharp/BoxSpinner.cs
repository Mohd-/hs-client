using System;
using UnityEngine;

// Token: 0x02000256 RID: 598
public class BoxSpinner : MonoBehaviour
{
	// Token: 0x06002205 RID: 8709 RVA: 0x000A75A7 File Offset: 0x000A57A7
	private void Awake()
	{
		this.m_spinnerMat = base.GetComponent<Renderer>().material;
	}

	// Token: 0x06002206 RID: 8710 RVA: 0x000A75BC File Offset: 0x000A57BC
	private void Update()
	{
		if (!this.IsSpinning())
		{
			return;
		}
		this.m_spinnerMat.SetFloat("_RotAngle", this.m_spinY);
		this.m_spinY += this.m_info.m_DegreesPerSec * Time.deltaTime * 0.01f;
	}

	// Token: 0x06002207 RID: 8711 RVA: 0x000A760F File Offset: 0x000A580F
	public Box GetParent()
	{
		return this.m_parent;
	}

	// Token: 0x06002208 RID: 8712 RVA: 0x000A7617 File Offset: 0x000A5817
	public void SetParent(Box parent)
	{
		this.m_parent = parent;
	}

	// Token: 0x06002209 RID: 8713 RVA: 0x000A7620 File Offset: 0x000A5820
	public BoxSpinnerStateInfo GetInfo()
	{
		return this.m_info;
	}

	// Token: 0x0600220A RID: 8714 RVA: 0x000A7628 File Offset: 0x000A5828
	public void SetInfo(BoxSpinnerStateInfo info)
	{
		this.m_info = info;
	}

	// Token: 0x0600220B RID: 8715 RVA: 0x000A7631 File Offset: 0x000A5831
	public void Spin()
	{
		this.m_spinning = true;
	}

	// Token: 0x0600220C RID: 8716 RVA: 0x000A763A File Offset: 0x000A583A
	public bool IsSpinning()
	{
		return this.m_spinning;
	}

	// Token: 0x0600220D RID: 8717 RVA: 0x000A7642 File Offset: 0x000A5842
	public void Stop()
	{
		this.m_spinning = false;
	}

	// Token: 0x0600220E RID: 8718 RVA: 0x000A764B File Offset: 0x000A584B
	public void Reset()
	{
		this.m_spinning = false;
		this.m_spinnerMat.SetFloat("_RotAngle", 0f);
	}

	// Token: 0x0400137C RID: 4988
	private Box m_parent;

	// Token: 0x0400137D RID: 4989
	private BoxSpinnerStateInfo m_info;

	// Token: 0x0400137E RID: 4990
	private bool m_spinning;

	// Token: 0x0400137F RID: 4991
	private float m_spinY;

	// Token: 0x04001380 RID: 4992
	private Material m_spinnerMat;
}
