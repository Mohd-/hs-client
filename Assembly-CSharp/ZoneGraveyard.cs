using System;

// Token: 0x02000622 RID: 1570
public class ZoneGraveyard : Zone
{
	// Token: 0x0600448D RID: 17549 RVA: 0x00149A28 File Offset: 0x00147C28
	public override bool CanAcceptTags(int controllerId, TAG_ZONE zoneTag, TAG_CARDTYPE cardType)
	{
		return base.CanAcceptTags(controllerId, zoneTag, cardType) && (cardType == TAG_CARDTYPE.MINION || cardType == TAG_CARDTYPE.WEAPON || cardType == TAG_CARDTYPE.SPELL || cardType == TAG_CARDTYPE.HERO);
	}

	// Token: 0x0600448E RID: 17550 RVA: 0x00149A60 File Offset: 0x00147C60
	public override void UpdateLayout()
	{
		this.m_updatingLayout = true;
		if (base.IsBlockingLayout())
		{
			base.UpdateLayoutFinished();
			return;
		}
		for (int i = 0; i < this.m_cards.Count; i++)
		{
			Card card = this.m_cards[i];
			if (!card.IsDoNotSort())
			{
				card.HideCard();
				card.EnableTransitioningZones(false);
			}
		}
		base.UpdateLayoutFinished();
	}
}
