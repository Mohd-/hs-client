using System;
using System.Collections.Generic;

// Token: 0x0200062C RID: 1580
public class DeckRule
{
	// Token: 0x060044E4 RID: 17636 RVA: 0x0014AE97 File Offset: 0x00149097
	public DeckRule()
	{
		this.m_ruleType = DeckRule.RuleType.UNKNOWN;
	}

	// Token: 0x060044E5 RID: 17637 RVA: 0x0014AEA8 File Offset: 0x001490A8
	public DeckRule(DeckRule.RuleType ruleType, DeckRulesetRuleDbfRecord record)
	{
		this.m_ruleType = ruleType;
		this.m_id = record.ID;
		this.m_deckRulesetId = record.DeckRulesetId;
		this.m_appliesToSubsetId = record.AppliesToSubsetId;
		this.m_appliesToIsNot = record.AppliesToIsNot;
		this.m_ruleIsNot = record.RuleIsNot;
		this.m_minValue = record.MinValue;
		this.m_maxValue = record.MaxValue;
		this.m_tag = record.Tag;
		this.m_tagMinValue = record.TagMinValue;
		this.m_tagMaxValue = record.TagMaxValue;
		this.m_stringValue = record.StringValue;
		this.m_errorString = ((record.ErrorString == null) ? string.Empty : record.ErrorString.GetString(true));
		this.m_subsets = new List<HashSet<string>>();
		if (this.m_appliesToSubsetId != 0)
		{
			this.m_appliesToSubset = GameDbf.GetIndex().GetSubsetById(this.m_appliesToSubsetId);
		}
		this.m_subsets = GameDbf.GetIndex().GetSubsetsForRule(this.m_id);
	}

	// Token: 0x060044E6 RID: 17638 RVA: 0x0014AFB0 File Offset: 0x001491B0
	public static DeckRule CreateFromDBF(DeckRulesetRuleDbfRecord record)
	{
		return DeckRule.GetRule(record);
	}

	// Token: 0x060044E7 RID: 17639 RVA: 0x0014AFC8 File Offset: 0x001491C8
	public static DeckRule GetRule(DeckRulesetRuleDbfRecord record)
	{
		string ruleType = record.RuleType;
		string text = ruleType;
		if (text != null)
		{
			if (DeckRule.<>f__switch$map7C == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(15);
				dictionary.Add("is_class_card_or_neutral", 0);
				dictionary.Add("is_in_any_subset", 1);
				dictionary.Add("count_cards_in_deck", 2);
				dictionary.Add("has_tag_value", 3);
				dictionary.Add("count_copies_of_each_card", 4);
				dictionary.Add("player_owns_each_copy", 5);
				dictionary.Add("deck_size", 6);
				dictionary.Add("is_not_rotated", 7);
				dictionary.Add("has_odd_numbered_tag_value", 8);
				dictionary.Add("count_cards_with_tag_value", 8);
				dictionary.Add("count_cards_with_tag_value_odd_numbered", 8);
				dictionary.Add("count_cards_with_same_tag_value", 8);
				dictionary.Add("count_unique_tag_values", 8);
				dictionary.Add("is_in_all_subsets", 8);
				dictionary.Add("card_text_contains_substring", 8);
				DeckRule.<>f__switch$map7C = dictionary;
			}
			int num;
			if (DeckRule.<>f__switch$map7C.TryGetValue(text, ref num))
			{
				switch (num)
				{
				case 0:
					return new DeckRule_IsClassCardOrNeutral(record);
				case 1:
					return new DeckRule_IsInAnySubset(record);
				case 2:
					return new DeckRule_CountCardsInDeck(record);
				case 3:
					return new DeckRule_HasTagValue(record);
				case 4:
					return new DeckRule_CountCopiesOfEachCard(record);
				case 5:
					return new DeckRule_PlayerOwnsEachCopy(record);
				case 6:
					return new DeckRule_DeckSize(record);
				case 7:
					return new DeckRule_IsNotRotated(record);
				}
			}
		}
		return new DeckRule(DeckRule.RuleType.UNKNOWN, record);
	}

	// Token: 0x060044E8 RID: 17640 RVA: 0x0014B135 File Offset: 0x00149335
	public DeckRule.RuleType GetRuleType()
	{
		return this.m_ruleType;
	}

	// Token: 0x060044E9 RID: 17641 RVA: 0x0014B13D File Offset: 0x0014933D
	public virtual bool Filter(EntityDef def)
	{
		return true;
	}

	// Token: 0x060044EA RID: 17642 RVA: 0x0014B140 File Offset: 0x00149340
	public virtual bool CanAddToDeck(EntityDef def, TAG_PREMIUM premium, CollectionDeck deck, out string reason)
	{
		reason = string.Empty;
		return true;
	}

	// Token: 0x060044EB RID: 17643 RVA: 0x0014B14B File Offset: 0x0014934B
	public virtual bool CanRemoveFromDeck(EntityDef def, CollectionDeck deck)
	{
		return true;
	}

	// Token: 0x060044EC RID: 17644 RVA: 0x0014B14E File Offset: 0x0014934E
	public virtual bool IsDeckValid(CollectionDeck deck, out string reason)
	{
		reason = string.Empty;
		return true;
	}

	// Token: 0x060044ED RID: 17645 RVA: 0x0014B158 File Offset: 0x00149358
	public override string ToString()
	{
		return string.Format("{0}, id:{1}, deckruleset:{2}", this.m_ruleType, this.m_id, this.m_deckRulesetId);
	}

	// Token: 0x060044EE RID: 17646 RVA: 0x0014B185 File Offset: 0x00149385
	protected bool GetResult(bool val)
	{
		return val == !this.m_ruleIsNot;
	}

	// Token: 0x060044EF RID: 17647 RVA: 0x0014B194 File Offset: 0x00149394
	protected bool AppliesTo(string cardId)
	{
		if (this.m_appliesToSubset == null)
		{
			return true;
		}
		bool flag = this.m_appliesToSubset.Contains(cardId);
		return flag == !this.m_appliesToIsNot;
	}

	// Token: 0x04002BC0 RID: 11200
	protected int m_id;

	// Token: 0x04002BC1 RID: 11201
	protected int m_deckRulesetId;

	// Token: 0x04002BC2 RID: 11202
	protected int m_appliesToSubsetId;

	// Token: 0x04002BC3 RID: 11203
	protected HashSet<string> m_appliesToSubset;

	// Token: 0x04002BC4 RID: 11204
	protected bool m_appliesToIsNot;

	// Token: 0x04002BC5 RID: 11205
	protected DeckRule.RuleType m_ruleType;

	// Token: 0x04002BC6 RID: 11206
	protected bool m_ruleIsNot;

	// Token: 0x04002BC7 RID: 11207
	protected int m_minValue;

	// Token: 0x04002BC8 RID: 11208
	protected int m_maxValue;

	// Token: 0x04002BC9 RID: 11209
	protected int m_tag;

	// Token: 0x04002BCA RID: 11210
	protected int m_tagMinValue;

	// Token: 0x04002BCB RID: 11211
	protected int m_tagMaxValue;

	// Token: 0x04002BCC RID: 11212
	protected string m_stringValue;

	// Token: 0x04002BCD RID: 11213
	protected string m_errorString;

	// Token: 0x04002BCE RID: 11214
	protected List<HashSet<string>> m_subsets;

	// Token: 0x0200062D RID: 1581
	public enum RuleType
	{
		// Token: 0x04002BD1 RID: 11217
		IS_IN_ANY_SUBSET,
		// Token: 0x04002BD2 RID: 11218
		IS_NOT_ROTATED,
		// Token: 0x04002BD3 RID: 11219
		COUNT_COPIES_OF_EACH_CARD,
		// Token: 0x04002BD4 RID: 11220
		PLAYER_OWNS_EACH_COPY,
		// Token: 0x04002BD5 RID: 11221
		IS_CLASS_CARD_OR_NEUTRAL,
		// Token: 0x04002BD6 RID: 11222
		COUNT_CARDS_IN_DECK,
		// Token: 0x04002BD7 RID: 11223
		HAS_TAG_VALUE,
		// Token: 0x04002BD8 RID: 11224
		DECK_SIZE,
		// Token: 0x04002BD9 RID: 11225
		UNKNOWN
	}
}
