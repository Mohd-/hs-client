using System;
using UnityEngine;

// Token: 0x020006E0 RID: 1760
public class CollectionCardBack : MonoBehaviour
{
	// Token: 0x060048C9 RID: 18633 RVA: 0x0015C2C0 File Offset: 0x0015A4C0
	public void Awake()
	{
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_nameFrame.SetActive(false);
		}
	}

	// Token: 0x060048CA RID: 18634 RVA: 0x0015C2DD File Offset: 0x0015A4DD
	public void SetCardBackId(int id)
	{
		this.m_cardBackId = id;
	}

	// Token: 0x060048CB RID: 18635 RVA: 0x0015C2E6 File Offset: 0x0015A4E6
	public int GetCardBackId()
	{
		return this.m_cardBackId;
	}

	// Token: 0x060048CC RID: 18636 RVA: 0x0015C2EE File Offset: 0x0015A4EE
	public void SetSeasonId(int seasonId)
	{
		this.m_seasonId = seasonId;
	}

	// Token: 0x060048CD RID: 18637 RVA: 0x0015C2F7 File Offset: 0x0015A4F7
	public int GetSeasonId()
	{
		return this.m_seasonId;
	}

	// Token: 0x060048CE RID: 18638 RVA: 0x0015C2FF File Offset: 0x0015A4FF
	public void SetCardBackName(string name)
	{
		if (this.m_name == null)
		{
			return;
		}
		this.m_name.Text = name;
	}

	// Token: 0x060048CF RID: 18639 RVA: 0x0015C31F File Offset: 0x0015A51F
	public void ShowFavoriteBanner(bool show)
	{
		if (this.m_favoriteBanner == null)
		{
			return;
		}
		this.m_favoriteBanner.SetActive(show);
	}

	// Token: 0x04002FE9 RID: 12265
	public UberText m_name;

	// Token: 0x04002FEA RID: 12266
	public GameObject m_favoriteBanner;

	// Token: 0x04002FEB RID: 12267
	public GameObject m_nameFrame;

	// Token: 0x04002FEC RID: 12268
	private int m_cardBackId = -1;

	// Token: 0x04002FED RID: 12269
	private int m_seasonId = -1;
}
