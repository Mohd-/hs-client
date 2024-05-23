using System;
using UnityEngine;

// Token: 0x02000816 RID: 2070
public class QuestProgressToast : GameToast
{
	// Token: 0x06004FE1 RID: 20449 RVA: 0x0017B114 File Offset: 0x00179314
	private void Awake()
	{
		this.m_intensityMaterials.Add(this.m_questProgressCountBg.GetComponent<Renderer>().material);
		this.m_intensityMaterials.Add(this.m_background.GetComponent<Renderer>().material);
	}

	// Token: 0x06004FE2 RID: 20450 RVA: 0x0017B158 File Offset: 0x00179358
	public void UpdateDisplay(string title, string description, int progress, int maxProgress)
	{
		if (maxProgress > 1)
		{
			this.m_questProgressCountBg.SetActive(true);
			this.m_questProgressCount.Text = GameStrings.Format("GLOBAL_QUEST_PROGRESS_COUNT", new object[]
			{
				progress,
				maxProgress
			});
		}
		else
		{
			this.m_questProgressCountBg.SetActive(false);
		}
		this.m_questTitle.Text = title;
		this.m_questDescription.Text = description;
	}

	// Token: 0x040036AF RID: 13999
	public UberText m_questTitle;

	// Token: 0x040036B0 RID: 14000
	public UberText m_questDescription;

	// Token: 0x040036B1 RID: 14001
	public UberText m_questProgressCount;

	// Token: 0x040036B2 RID: 14002
	public GameObject m_questProgressCountBg;

	// Token: 0x040036B3 RID: 14003
	public GameObject m_background;
}
