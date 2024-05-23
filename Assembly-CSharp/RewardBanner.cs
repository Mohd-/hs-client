using System;
using UnityEngine;

// Token: 0x02000172 RID: 370
public class RewardBanner : MonoBehaviour
{
	// Token: 0x060013DF RID: 5087 RVA: 0x000583E8 File Offset: 0x000565E8
	private void Awake()
	{
		if (UniversalInputManager.UsePhoneUI && this.m_sourceText != null)
		{
			this.m_sourceText.gameObject.SetActive(false);
		}
		this.m_headlineHeight = this.m_headlineText.Height;
	}

	// Token: 0x1700030D RID: 781
	// (get) Token: 0x060013E0 RID: 5088 RVA: 0x00058437 File Offset: 0x00056637
	public string HeadlineText
	{
		get
		{
			return this.m_headlineText.Text;
		}
	}

	// Token: 0x1700030E RID: 782
	// (get) Token: 0x060013E1 RID: 5089 RVA: 0x00058444 File Offset: 0x00056644
	public string DetailsText
	{
		get
		{
			return this.m_detailsText.Text;
		}
	}

	// Token: 0x1700030F RID: 783
	// (get) Token: 0x060013E2 RID: 5090 RVA: 0x00058451 File Offset: 0x00056651
	public string SourceText
	{
		get
		{
			return this.m_sourceText.Text;
		}
	}

	// Token: 0x060013E3 RID: 5091 RVA: 0x00058460 File Offset: 0x00056660
	public void SetText(string headline, string details, string source)
	{
		this.m_headlineText.Text = headline;
		this.m_detailsText.Text = details;
		this.m_sourceText.Text = source;
		if (details == string.Empty)
		{
			this.AlignHeadlineToCenterBone();
			this.m_headlineText.Height = this.m_headlineHeight * 1.5f;
		}
	}

	// Token: 0x060013E4 RID: 5092 RVA: 0x000584C0 File Offset: 0x000566C0
	public void AlignHeadlineToCenterBone()
	{
		if (this.m_headlineCenterBone != null)
		{
			this.m_headlineText.transform.localPosition = this.m_headlineCenterBone.transform.localPosition;
		}
	}

	// Token: 0x04000A3F RID: 2623
	public UberText m_headlineText;

	// Token: 0x04000A40 RID: 2624
	public UberText m_detailsText;

	// Token: 0x04000A41 RID: 2625
	public UberText m_sourceText;

	// Token: 0x04000A42 RID: 2626
	public GameObject m_headlineCenterBone;

	// Token: 0x04000A43 RID: 2627
	private float m_headlineHeight;
}
