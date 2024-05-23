using System;
using UnityEngine;

// Token: 0x02000F6E RID: 3950
public class MenuButton : PegUIElement
{
	// Token: 0x0600752C RID: 29996 RVA: 0x0022961F File Offset: 0x0022781F
	public void SetText(string s)
	{
		this.m_label.text = s;
	}

	// Token: 0x04005FAE RID: 24494
	public TextMesh m_label;
}
