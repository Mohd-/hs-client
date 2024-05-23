using System;
using UnityEngine;

// Token: 0x0200069F RID: 1695
public class RibbonButton : PegUIElement
{
	// Token: 0x06004766 RID: 18278 RVA: 0x00156C18 File Offset: 0x00154E18
	public void Start()
	{
		this.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnButtonOver));
		this.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnButtonOut));
	}

	// Token: 0x06004767 RID: 18279 RVA: 0x00156C4D File Offset: 0x00154E4D
	public void OnButtonOver(UIEvent e)
	{
		if (this.m_highlight != null)
		{
			this.m_highlight.SetActive(true);
		}
	}

	// Token: 0x06004768 RID: 18280 RVA: 0x00156C6C File Offset: 0x00154E6C
	public void OnButtonOut(UIEvent e)
	{
		if (this.m_highlight != null)
		{
			this.m_highlight.SetActive(false);
		}
	}

	// Token: 0x04002E61 RID: 11873
	public GameObject m_highlight;
}
