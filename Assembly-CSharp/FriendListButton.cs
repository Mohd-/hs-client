using System;
using UnityEngine;

// Token: 0x0200057F RID: 1407
public class FriendListButton : FriendListUIElement
{
	// Token: 0x06003FFB RID: 16379 RVA: 0x0013643F File Offset: 0x0013463F
	public string GetText()
	{
		return this.m_Text.Text;
	}

	// Token: 0x06003FFC RID: 16380 RVA: 0x0013644C File Offset: 0x0013464C
	public void SetText(string text)
	{
		this.m_Text.Text = text;
		this.UpdateAll();
	}

	// Token: 0x06003FFD RID: 16381 RVA: 0x00136460 File Offset: 0x00134660
	public void ShowActiveGlow(bool show)
	{
		if (this.m_ActiveGlow != null)
		{
			HighlightState componentInChildren = this.m_ActiveGlow.GetComponentInChildren<HighlightState>();
			if (componentInChildren != null)
			{
				if (show)
				{
					componentInChildren.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
				}
				else
				{
					componentInChildren.ChangeState(ActorStateType.HIGHLIGHT_OFF);
				}
			}
		}
	}

	// Token: 0x06003FFE RID: 16382 RVA: 0x001364B3 File Offset: 0x001346B3
	private void UpdateAll()
	{
		base.UpdateHighlight();
	}

	// Token: 0x040028F9 RID: 10489
	public GameObject m_Background;

	// Token: 0x040028FA RID: 10490
	public UberText m_Text;

	// Token: 0x040028FB RID: 10491
	public GameObject m_ActiveGlow;
}
