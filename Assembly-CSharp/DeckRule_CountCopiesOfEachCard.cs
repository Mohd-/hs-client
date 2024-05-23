using System;
using System.Collections.Generic;

// Token: 0x02000711 RID: 1809
public class DeckRule_CountCopiesOfEachCard : DeckRule
{
	// Token: 0x06004A0E RID: 18958 RVA: 0x00162194 File Offset: 0x00160394
	public DeckRule_CountCopiesOfEachCard(int subset, int min, int max)
	{
		this.m_ruleType = DeckRule.RuleType.COUNT_COPIES_OF_EACH_CARD;
		this.m_appliesToSubsetId = subset;
		this.m_minValue = min;
		this.m_maxValue = max;
		if (this.m_appliesToSubsetId != 0)
		{
			this.m_appliesToSubset = GameDbf.GetIndex().GetSubsetById(this.m_appliesToSubsetId);
		}
	}

	// Token: 0x06004A0F RID: 18959 RVA: 0x001621E4 File Offset: 0x001603E4
	public DeckRule_CountCopiesOfEachCard(DeckRulesetRuleDbfRecord record) : base(DeckRule.RuleType.COUNT_COPIES_OF_EACH_CARD, record)
	{
	}

	// Token: 0x06004A10 RID: 18960 RVA: 0x001621F0 File Offset: 0x001603F0
	public override bool CanAddToDeck(EntityDef def, TAG_PREMIUM premium, CollectionDeck deck, out string reason)
	{
		reason = string.Empty;
		if (!base.AppliesTo(def.GetCardId()))
		{
			return true;
		}
		bool flag = deck.GetCardIdCount(def.GetCardId(), true) >= this.m_maxValue;
		if (flag)
		{
			reason = GameStrings.Format("GLUE_COLLECTION_LOCK_MAX_DECK_COPIES", new object[]
			{
				this.m_maxValue
			});
		}
		return base.GetResult(!flag);
	}

	// Token: 0x06004A11 RID: 18961 RVA: 0x00162264 File Offset: 0x00160464
	public override bool IsDeckValid(CollectionDeck deck, out string reason)
	{
		reason = string.Empty;
		bool flag = true;
		List<CollectionDeckSlot> slots = deck.GetSlots();
		foreach (CollectionDeckSlot collectionDeckSlot in slots)
		{
			string cardID = collectionDeckSlot.CardID;
			if (base.AppliesTo(cardID))
			{
				int cardIdCount = deck.GetCardIdCount(cardID, true);
				if (cardIdCount < this.m_minValue)
				{
					flag = false;
				}
				else if (cardIdCount > this.m_maxValue)
				{
					flag = false;
				}
			}
		}
		flag = base.GetResult(flag);
		if (!flag)
		{
			reason = this.m_errorString;
		}
		return flag;
	}
}
