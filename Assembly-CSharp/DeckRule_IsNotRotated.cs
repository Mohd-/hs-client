using System;

// Token: 0x02000713 RID: 1811
public class DeckRule_IsNotRotated : DeckRule
{
	// Token: 0x06004A15 RID: 18965 RVA: 0x001623AD File Offset: 0x001605AD
	public DeckRule_IsNotRotated()
	{
		this.m_ruleType = DeckRule.RuleType.IS_NOT_ROTATED;
	}

	// Token: 0x06004A16 RID: 18966 RVA: 0x001623BC File Offset: 0x001605BC
	public DeckRule_IsNotRotated(DeckRulesetRuleDbfRecord record) : base(DeckRule.RuleType.IS_IN_ANY_SUBSET, record)
	{
	}

	// Token: 0x06004A17 RID: 18967 RVA: 0x001623C8 File Offset: 0x001605C8
	public override bool IsDeckValid(CollectionDeck deck, out string reason)
	{
		reason = string.Empty;
		int totalValidCardCount = deck.GetTotalValidCardCount();
		int totalCardCount = deck.GetTotalCardCount();
		if (totalValidCardCount < totalCardCount)
		{
			reason = GameStrings.Format("GLUE_COLLECTION_DECK_RULE_INVALID_CARDS", new object[]
			{
				totalCardCount - totalValidCardCount
			});
			return false;
		}
		return true;
	}

	// Token: 0x06004A18 RID: 18968 RVA: 0x00162411 File Offset: 0x00160611
	public override bool Filter(EntityDef def)
	{
		return !GameUtils.IsCardRotated(def);
	}
}
