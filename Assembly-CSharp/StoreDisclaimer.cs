using System;
using UnityEngine;

// Token: 0x02000AF3 RID: 2803
public class StoreDisclaimer : MonoBehaviour
{
	// Token: 0x06006067 RID: 24679 RVA: 0x001CE388 File Offset: 0x001CC588
	private void Awake()
	{
		this.m_headlineText.Text = GameStrings.Get("GLUE_STORE_DISCLAIMER_HEADLINE");
		this.m_warningText.Text = GameStrings.Get("GLUE_STORE_DISCLAIMER_WARNING");
		this.m_detailsText.Text = string.Empty;
	}

	// Token: 0x06006068 RID: 24680 RVA: 0x001CE3CF File Offset: 0x001CC5CF
	public void UpdateTextSize()
	{
		this.m_headlineText.UpdateNow();
		this.m_warningText.UpdateNow();
		this.m_detailsText.UpdateNow();
	}

	// Token: 0x06006069 RID: 24681 RVA: 0x001CE3F2 File Offset: 0x001CC5F2
	public void SetDetailsText(string detailsText)
	{
		this.m_detailsText.Text = detailsText;
	}

	// Token: 0x040047F9 RID: 18425
	public UberText m_headlineText;

	// Token: 0x040047FA RID: 18426
	public UberText m_warningText;

	// Token: 0x040047FB RID: 18427
	public UberText m_detailsText;

	// Token: 0x040047FC RID: 18428
	public GameObject m_root;
}
