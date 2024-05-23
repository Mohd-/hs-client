using System;

// Token: 0x02000259 RID: 601
public class BoxMenuButton : PegUIElement
{
	// Token: 0x0600221D RID: 8733 RVA: 0x000A7E3F File Offset: 0x000A603F
	public string GetText()
	{
		return this.m_TextMesh.Text;
	}

	// Token: 0x0600221E RID: 8734 RVA: 0x000A7E4C File Offset: 0x000A604C
	public void SetText(string text)
	{
		this.m_TextMesh.Text = text;
	}

	// Token: 0x0600221F RID: 8735 RVA: 0x000A7E5A File Offset: 0x000A605A
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		if (this.m_Spell == null)
		{
			return;
		}
		this.m_Spell.ActivateState(SpellStateType.BIRTH);
	}

	// Token: 0x06002220 RID: 8736 RVA: 0x000A7E7A File Offset: 0x000A607A
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		if (this.m_Spell == null)
		{
			return;
		}
		this.m_Spell.ActivateState(SpellStateType.DEATH);
	}

	// Token: 0x06002221 RID: 8737 RVA: 0x000A7E9A File Offset: 0x000A609A
	protected override void OnPress()
	{
		if (this.m_Spell == null)
		{
			return;
		}
		this.m_Spell.ActivateState(SpellStateType.ACTION);
	}

	// Token: 0x04001385 RID: 4997
	public UberText m_TextMesh;

	// Token: 0x04001386 RID: 4998
	public Spell m_Spell;

	// Token: 0x04001387 RID: 4999
	public HighlightState m_HighlightState;
}
