using System;
using System.Collections.Generic;

// Token: 0x02000943 RID: 2371
public class DyingSecretGroup
{
	// Token: 0x06005724 RID: 22308 RVA: 0x001A1F44 File Offset: 0x001A0144
	public Card GetMainCard()
	{
		return this.m_mainCard;
	}

	// Token: 0x06005725 RID: 22309 RVA: 0x001A1F4C File Offset: 0x001A014C
	public List<Actor> GetActors()
	{
		return this.m_actors;
	}

	// Token: 0x06005726 RID: 22310 RVA: 0x001A1F54 File Offset: 0x001A0154
	public List<Card> GetCards()
	{
		return this.m_cards;
	}

	// Token: 0x06005727 RID: 22311 RVA: 0x001A1F5C File Offset: 0x001A015C
	public void AddCard(Card card)
	{
		if (this.m_mainCard == null)
		{
			Zone zone = card.GetZone();
			this.m_mainCard = zone.GetCards().Find((Card currCard) => currCard.IsShown());
		}
		this.m_cards.Add(card);
		this.m_actors.Add(card.GetActor());
	}

	// Token: 0x04003DF3 RID: 15859
	private Card m_mainCard;

	// Token: 0x04003DF4 RID: 15860
	private List<Card> m_cards = new List<Card>();

	// Token: 0x04003DF5 RID: 15861
	private List<Actor> m_actors = new List<Actor>();
}
