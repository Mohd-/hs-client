using System;
using UnityEngine;

// Token: 0x0200073F RID: 1855
public class DeckRule_CountCardsInDeck : DeckRule
{
	// Token: 0x06004B46 RID: 19270 RVA: 0x00167BA0 File Offset: 0x00165DA0
	public DeckRule_CountCardsInDeck(int min, int max)
	{
		this.m_ruleType = DeckRule.RuleType.COUNT_CARDS_IN_DECK;
		this.m_minValue = min;
		this.m_maxValue = max;
	}

	// Token: 0x06004B47 RID: 19271 RVA: 0x00167BBD File Offset: 0x00165DBD
	public DeckRule_CountCardsInDeck(DeckRulesetRuleDbfRecord record) : base(DeckRule.RuleType.COUNT_CARDS_IN_DECK, record)
	{
		if (this.m_appliesToSubset == null)
		{
			Debug.LogError("COUNT_CARDS_IN_DECK only supports rules with a defined \"applies to\" subset");
		}
	}

	// Token: 0x06004B48 RID: 19272 RVA: 0x00167BDC File Offset: 0x00165DDC
	public override bool IsDeckValid(CollectionDeck deck, out string reason)
	{
		reason = string.Empty;
		int cardCountInSet = deck.GetCardCountInSet(this.m_appliesToSubset, this.m_appliesToIsNot);
		bool val = cardCountInSet >= this.m_minValue && cardCountInSet <= this.m_maxValue;
		if (!base.GetResult(val))
		{
			reason = this.m_errorString;
		}
		return base.GetResult(val);
	}
}
