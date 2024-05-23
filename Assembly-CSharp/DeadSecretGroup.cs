using System;
using System.Collections.Generic;

// Token: 0x020008AE RID: 2222
public class DeadSecretGroup
{
	// Token: 0x06005437 RID: 21559 RVA: 0x00192FA9 File Offset: 0x001911A9
	public Card GetMainCard()
	{
		return this.m_mainCard;
	}

	// Token: 0x06005438 RID: 21560 RVA: 0x00192FB1 File Offset: 0x001911B1
	public void SetMainCard(Card card)
	{
		this.m_mainCard = card;
	}

	// Token: 0x06005439 RID: 21561 RVA: 0x00192FBA File Offset: 0x001911BA
	public List<Card> GetCards()
	{
		return this.m_cards;
	}

	// Token: 0x0600543A RID: 21562 RVA: 0x00192FC2 File Offset: 0x001911C2
	public void AddCard(Card card)
	{
		this.m_cards.Add(card);
	}

	// Token: 0x04003A66 RID: 14950
	private Card m_mainCard;

	// Token: 0x04003A67 RID: 14951
	private List<Card> m_cards = new List<Card>();
}
