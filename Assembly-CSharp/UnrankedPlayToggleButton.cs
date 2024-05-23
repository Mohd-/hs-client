using System;
using UnityEngine;

// Token: 0x020007EF RID: 2031
public class UnrankedPlayToggleButton : PegUIElement
{
	// Token: 0x06004EF6 RID: 20214 RVA: 0x00176C7C File Offset: 0x00174E7C
	public void SetRankedMode(bool isRankedMode)
	{
		this.m_isRankedMode = isRankedMode;
		if (isRankedMode)
		{
			this.m_xIcon.SetActive(true);
			SoundManager.Get().LoadAndPlay("checkbox_toggle_off");
		}
		else
		{
			SoundManager.Get().LoadAndPlay("checkbox_toggle_on");
			this.m_xIcon.SetActive(false);
		}
	}

	// Token: 0x06004EF7 RID: 20215 RVA: 0x00176CD1 File Offset: 0x00174ED1
	public bool GetIsRanked()
	{
		return this.m_isRankedMode;
	}

	// Token: 0x06004EF8 RID: 20216 RVA: 0x00176CDC File Offset: 0x00174EDC
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		SoundManager.Get().LoadAndPlay("Small_Mouseover");
		if (this.m_highlight != null)
		{
			this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_MOUSE_OVER);
		}
	}

	// Token: 0x06004EF9 RID: 20217 RVA: 0x00176D17 File Offset: 0x00174F17
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		if (this.m_highlight != null)
		{
			this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
		}
	}

	// Token: 0x040035CC RID: 13772
	public GameObject m_xIcon;

	// Token: 0x040035CD RID: 13773
	public HighlightState m_highlight;

	// Token: 0x040035CE RID: 13774
	private bool m_isRankedMode;
}
