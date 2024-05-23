using System;
using UnityEngine;

// Token: 0x0200025B RID: 603
public class PackOpeningButton : BoxMenuButton
{
	// Token: 0x0600222B RID: 8747 RVA: 0x000A8153 File Offset: 0x000A6353
	public string GetGetPackCount()
	{
		return this.m_count.Text;
	}

	// Token: 0x0600222C RID: 8748 RVA: 0x000A8160 File Offset: 0x000A6360
	public void SetPackCount(int packs)
	{
		if (packs < 0)
		{
			this.m_count.Text = string.Empty;
		}
		else
		{
			this.m_count.Text = GameStrings.Format("GLUE_PACK_OPENING_BOOSTER_COUNT", new object[]
			{
				packs
			});
		}
	}

	// Token: 0x0400138B RID: 5003
	public UberText m_count;

	// Token: 0x0400138C RID: 5004
	public GameObject m_countFrame;
}
