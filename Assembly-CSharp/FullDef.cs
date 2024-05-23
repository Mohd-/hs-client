using System;

// Token: 0x0200029E RID: 670
public class FullDef
{
	// Token: 0x06002442 RID: 9282 RVA: 0x000B1F0A File Offset: 0x000B010A
	public bool IsEmpty()
	{
		return this.m_entityDef == null && !(this.m_cardDef != null);
	}

	// Token: 0x06002443 RID: 9283 RVA: 0x000B1F2D File Offset: 0x000B012D
	public EntityDef GetEntityDef()
	{
		return this.m_entityDef;
	}

	// Token: 0x06002444 RID: 9284 RVA: 0x000B1F35 File Offset: 0x000B0135
	public void SetEntityDef(EntityDef entityDef)
	{
		this.m_entityDef = entityDef;
	}

	// Token: 0x06002445 RID: 9285 RVA: 0x000B1F3E File Offset: 0x000B013E
	public CardDef GetCardDef()
	{
		return this.m_cardDef;
	}

	// Token: 0x06002446 RID: 9286 RVA: 0x000B1F46 File Offset: 0x000B0146
	public void SetCardDef(CardDef cardDef)
	{
		this.m_cardDef = cardDef;
	}

	// Token: 0x04001567 RID: 5479
	private EntityDef m_entityDef;

	// Token: 0x04001568 RID: 5480
	private CardDef m_cardDef;
}
