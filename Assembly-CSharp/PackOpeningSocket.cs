using System;

// Token: 0x02000A8B RID: 2699
public class PackOpeningSocket : PegUIElement
{
	// Token: 0x06005DDE RID: 24030 RVA: 0x001C20B6 File Offset: 0x001C02B6
	protected override void Awake()
	{
		base.Awake();
		this.m_alertSpell = base.GetComponent<Spell>();
	}

	// Token: 0x06005DDF RID: 24031 RVA: 0x001C20CA File Offset: 0x001C02CA
	public void OnPackHeld()
	{
		this.m_alertSpell.ActivateState(SpellStateType.BIRTH);
	}

	// Token: 0x06005DE0 RID: 24032 RVA: 0x001C20D8 File Offset: 0x001C02D8
	public void OnPackReleased()
	{
		this.m_alertSpell.ActivateState(SpellStateType.DEATH);
	}

	// Token: 0x0400458C RID: 17804
	private Spell m_alertSpell;
}
