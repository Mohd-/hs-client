using System;

// Token: 0x02000712 RID: 1810
public class DeckRule_PlayerOwnsEachCopy : DeckRule
{
	// Token: 0x06004A12 RID: 18962 RVA: 0x00162324 File Offset: 0x00160524
	public DeckRule_PlayerOwnsEachCopy()
	{
		this.m_ruleType = DeckRule.RuleType.PLAYER_OWNS_EACH_COPY;
	}

	// Token: 0x06004A13 RID: 18963 RVA: 0x00162333 File Offset: 0x00160533
	public DeckRule_PlayerOwnsEachCopy(DeckRulesetRuleDbfRecord record) : base(DeckRule.RuleType.PLAYER_OWNS_EACH_COPY, record)
	{
	}

	// Token: 0x06004A14 RID: 18964 RVA: 0x00162340 File Offset: 0x00160540
	public override bool CanAddToDeck(EntityDef def, TAG_PREMIUM premium, CollectionDeck deck, out string reason)
	{
		reason = string.Empty;
		string cardId = def.GetCardId();
		if (!base.AppliesTo(cardId))
		{
			return base.GetResult(true);
		}
		CollectibleCard card = CollectionManager.Get().GetCard(cardId, premium);
		if (deck.GetOwnedCardCount(cardId, premium, true) >= card.OwnedCount)
		{
			reason = GameStrings.Get("GLUE_COLLECTION_LOCK_NO_MORE_INSTANCES");
			return base.GetResult(false);
		}
		return base.GetResult(true);
	}
}
