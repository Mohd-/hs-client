using System;
using UnityEngine;

// Token: 0x02000710 RID: 1808
public class DeckRule_DeckSize : DeckRule
{
	// Token: 0x06004A0A RID: 18954 RVA: 0x001620E1 File Offset: 0x001602E1
	public DeckRule_DeckSize(int size)
	{
		this.m_ruleType = DeckRule.RuleType.DECK_SIZE;
		this.m_minValue = size;
		this.m_maxValue = size;
	}

	// Token: 0x06004A0B RID: 18955 RVA: 0x00162100 File Offset: 0x00160300
	public DeckRule_DeckSize(DeckRulesetRuleDbfRecord record) : base(DeckRule.RuleType.DECK_SIZE, record)
	{
		if (this.m_ruleIsNot)
		{
			Debug.LogError("DECK_SIZE rules do not support \"is not\".");
		}
		if (this.m_appliesToSubset != null)
		{
			Debug.LogError("DECK_SIZE rules do not support \"applies to subset\".");
		}
	}

	// Token: 0x06004A0C RID: 18956 RVA: 0x00162140 File Offset: 0x00160340
	public override bool IsDeckValid(CollectionDeck deck, out string reason)
	{
		reason = string.Empty;
		int totalOwnedCardCount = deck.GetTotalOwnedCardCount();
		if (totalOwnedCardCount != this.m_maxValue)
		{
			reason = GameStrings.Format("GLUE_COLLECTION_DECK_RULE_MISSING_CARDS", new object[]
			{
				this.m_maxValue - totalOwnedCardCount
			});
			return false;
		}
		return true;
	}

	// Token: 0x06004A0D RID: 18957 RVA: 0x0016218C File Offset: 0x0016038C
	public int GetDeckSize()
	{
		return this.m_maxValue;
	}
}
