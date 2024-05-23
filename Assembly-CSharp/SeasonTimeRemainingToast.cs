using System;
using UnityEngine;

// Token: 0x02000817 RID: 2071
public class SeasonTimeRemainingToast : GameToast
{
	// Token: 0x06004FE4 RID: 20452 RVA: 0x0017B1D8 File Offset: 0x001793D8
	private void Awake()
	{
		this.m_intensityMaterials.Add(this.m_background.GetComponent<Renderer>().material);
	}

	// Token: 0x06004FE5 RID: 20453 RVA: 0x0017B1F5 File Offset: 0x001793F5
	public void UpdateDisplay(string title, string timeRemaining)
	{
		this.m_seasonTitle.Text = title;
		this.m_timeRemaining.Text = timeRemaining;
	}

	// Token: 0x040036B4 RID: 14004
	public UberText m_seasonTitle;

	// Token: 0x040036B5 RID: 14005
	public UberText m_timeRemaining;

	// Token: 0x040036B6 RID: 14006
	public GameObject m_background;
}
