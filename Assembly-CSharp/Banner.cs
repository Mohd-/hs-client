using System;
using UnityEngine;

// Token: 0x0200068C RID: 1676
public class Banner : MonoBehaviour
{
	// Token: 0x060046F0 RID: 18160 RVA: 0x00154955 File Offset: 0x00152B55
	public void SetText(string headline)
	{
		this.m_headline.Text = headline;
		this.m_caption.gameObject.SetActive(false);
	}

	// Token: 0x060046F1 RID: 18161 RVA: 0x00154974 File Offset: 0x00152B74
	public void SetText(string headline, string caption)
	{
		this.m_headline.Text = headline;
		this.m_caption.Text = caption;
	}

	// Token: 0x060046F2 RID: 18162 RVA: 0x00154990 File Offset: 0x00152B90
	public void MoveGlowForBottomPlacement()
	{
		this.m_glowObject.transform.localPosition = new Vector3(this.m_glowObject.transform.localPosition.x, this.m_glowObject.transform.localPosition.y, 0f);
	}

	// Token: 0x04002E07 RID: 11783
	public UberText m_headline;

	// Token: 0x04002E08 RID: 11784
	public UberText m_caption;

	// Token: 0x04002E09 RID: 11785
	public GameObject m_glowObject;
}
