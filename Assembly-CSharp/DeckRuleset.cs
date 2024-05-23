using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020003A4 RID: 932
public class DeckRuleset
{
	// Token: 0x170003DB RID: 987
	// (get) Token: 0x06003114 RID: 12564 RVA: 0x000F65CB File Offset: 0x000F47CB
	public int Id
	{
		get
		{
			return this.m_id;
		}
	}

	// Token: 0x170003DC RID: 988
	// (get) Token: 0x06003115 RID: 12565 RVA: 0x000F65D3 File Offset: 0x000F47D3
	public List<DeckRule> Rules
	{
		get
		{
			return this.m_rules;
		}
	}

	// Token: 0x06003116 RID: 12566 RVA: 0x000F65DC File Offset: 0x000F47DC
	public static DeckRuleset GetDeckRuleset(int id)
	{
		if (id <= 0)
		{
			return null;
		}
		if (!GameDbf.DeckRuleset.HasRecord(id))
		{
			Debug.LogErrorFormat("DeckRuleset not found for id {0}", new object[]
			{
				id
			});
			return null;
		}
		DeckRuleset deckRuleset = new DeckRuleset();
		deckRuleset.m_id = id;
		deckRuleset.m_rules = new List<DeckRule>();
		foreach (DeckRulesetRuleDbfRecord record in GameDbf.GetIndex().GetRulesForDeckRuleset(id))
		{
			DeckRule deckRule = DeckRule.CreateFromDBF(record);
			deckRuleset.m_rules.Add(deckRule);
		}
		return deckRuleset;
	}

	// Token: 0x06003117 RID: 12567 RVA: 0x000F666F File Offset: 0x000F486F
	public static DeckRuleset GetStandardRuleset()
	{
		if (DeckRuleset.s_standardRuleset == null)
		{
			DeckRuleset.s_standardRuleset = DeckRuleset.BuildStandardRuleset();
		}
		return DeckRuleset.s_standardRuleset;
	}

	// Token: 0x06003118 RID: 12568 RVA: 0x000F668C File Offset: 0x000F488C
	private static DeckRuleset BuildStandardRuleset()
	{
		DeckRuleset deckRuleset = new DeckRuleset();
		deckRuleset.m_id = 2;
		deckRuleset.m_rules = new List<DeckRule>();
		DeckRule_DeckSize deckRule_DeckSize = new DeckRule_DeckSize(30);
		DeckRule_CountCopiesOfEachCard deckRule_CountCopiesOfEachCard = new DeckRule_CountCopiesOfEachCard(0, 0, 2);
		DeckRule_CountCopiesOfEachCard deckRule_CountCopiesOfEachCard2 = new DeckRule_CountCopiesOfEachCard(7, 0, 1);
		DeckRule_PlayerOwnsEachCopy deckRule_PlayerOwnsEachCopy = new DeckRule_PlayerOwnsEachCopy();
		DeckRule_IsNotRotated deckRule_IsNotRotated = new DeckRule_IsNotRotated();
		deckRuleset.m_rules.Add(deckRule_DeckSize);
		deckRuleset.m_rules.Add(deckRule_CountCopiesOfEachCard);
		deckRuleset.m_rules.Add(deckRule_CountCopiesOfEachCard2);
		deckRuleset.m_rules.Add(deckRule_PlayerOwnsEachCopy);
		deckRuleset.m_rules.Add(deckRule_IsNotRotated);
		return deckRuleset;
	}

	// Token: 0x06003119 RID: 12569 RVA: 0x000F6718 File Offset: 0x000F4918
	public static DeckRuleset GetWildRuleset()
	{
		if (DeckRuleset.s_wildRuleset == null)
		{
			DeckRuleset.s_wildRuleset = DeckRuleset.BuildWildRuleset();
		}
		return DeckRuleset.s_wildRuleset;
	}

	// Token: 0x0600311A RID: 12570 RVA: 0x000F6734 File Offset: 0x000F4934
	private static DeckRuleset BuildWildRuleset()
	{
		DeckRuleset deckRuleset = new DeckRuleset();
		deckRuleset.m_id = 1;
		deckRuleset.m_rules = new List<DeckRule>();
		DeckRule_DeckSize deckRule_DeckSize = new DeckRule_DeckSize(30);
		DeckRule_CountCopiesOfEachCard deckRule_CountCopiesOfEachCard = new DeckRule_CountCopiesOfEachCard(0, 0, 2);
		DeckRule_CountCopiesOfEachCard deckRule_CountCopiesOfEachCard2 = new DeckRule_CountCopiesOfEachCard(7, 0, 1);
		DeckRule_PlayerOwnsEachCopy deckRule_PlayerOwnsEachCopy = new DeckRule_PlayerOwnsEachCopy();
		deckRuleset.m_rules.Add(deckRule_DeckSize);
		deckRuleset.m_rules.Add(deckRule_CountCopiesOfEachCard);
		deckRuleset.m_rules.Add(deckRule_CountCopiesOfEachCard2);
		deckRuleset.m_rules.Add(deckRule_PlayerOwnsEachCopy);
		return deckRuleset;
	}

	// Token: 0x0600311B RID: 12571 RVA: 0x000F67AC File Offset: 0x000F49AC
	public bool Filter(EntityDef entity)
	{
		foreach (DeckRule deckRule in this.m_rules)
		{
			if (!deckRule.Filter(entity))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600311C RID: 12572 RVA: 0x000F6818 File Offset: 0x000F4A18
	public bool CanAddToDeck(EntityDef def, TAG_PREMIUM premium, CollectionDeck deck, List<DeckRule.RuleType> ignoreRules = null)
	{
		string text;
		DeckRule deckRule;
		return this.CanAddToDeck(def, premium, deck, out text, out deckRule, ignoreRules);
	}

	// Token: 0x0600311D RID: 12573 RVA: 0x000F6834 File Offset: 0x000F4A34
	public bool CanAddToDeck(EntityDef def, TAG_PREMIUM premium, CollectionDeck deck, out string reason, out DeckRule brokenRule, List<DeckRule.RuleType> ignoreRules = null)
	{
		reason = string.Empty;
		brokenRule = null;
		foreach (DeckRule deckRule in this.m_rules)
		{
			if (ignoreRules == null || !ignoreRules.Contains(deckRule.GetRuleType()))
			{
				if (!deckRule.CanAddToDeck(def, premium, deck, out reason))
				{
					brokenRule = deckRule;
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x0600311E RID: 12574 RVA: 0x000F68D0 File Offset: 0x000F4AD0
	public bool IsDeckValid(CollectionDeck deck)
	{
		List<string> list = new List<string>();
		List<DeckRule> list2 = new List<DeckRule>();
		return this.IsDeckValid(deck, out list, out list2);
	}

	// Token: 0x0600311F RID: 12575 RVA: 0x000F68F4 File Offset: 0x000F4AF4
	public bool IsDeckValid(CollectionDeck deck, out List<string> reasons, out List<DeckRule> brokenRules)
	{
		reasons = new List<string>();
		brokenRules = new List<DeckRule>();
		bool result = true;
		foreach (DeckRule deckRule in this.m_rules)
		{
			string text;
			bool flag = deckRule.IsDeckValid(deck, out text);
			if (!flag)
			{
				reasons.Add(text);
				brokenRules.Add(deckRule);
				result = false;
			}
			Log.DeckRuleset.Print("validating rule={0} deck={1} result={2} reason={3}", new object[]
			{
				deckRule,
				deck,
				flag,
				text
			});
		}
		return result;
	}

	// Token: 0x06003120 RID: 12576 RVA: 0x000F69A8 File Offset: 0x000F4BA8
	public int GetDeckSize()
	{
		foreach (DeckRule deckRule in this.m_rules)
		{
			if (deckRule is DeckRule_DeckSize)
			{
				DeckRule_DeckSize deckRule_DeckSize = deckRule as DeckRule_DeckSize;
				return deckRule_DeckSize.GetDeckSize();
			}
		}
		return 30;
	}

	// Token: 0x06003121 RID: 12577 RVA: 0x000F6A20 File Offset: 0x000F4C20
	public bool AllowsWildCards()
	{
		DeckRule deckRule;
		if (this.m_rules == null)
		{
			deckRule = null;
		}
		else
		{
			deckRule = Enumerable.FirstOrDefault<DeckRule>(this.m_rules, (DeckRule r) => r.GetRuleType() == DeckRule.RuleType.IS_NOT_ROTATED);
		}
		DeckRule deckRule2 = deckRule;
		return deckRule2 == null;
	}

	// Token: 0x04001E89 RID: 7817
	private int m_id;

	// Token: 0x04001E8A RID: 7818
	private List<DeckRule> m_rules;

	// Token: 0x04001E8B RID: 7819
	private CollectionDeck m_editingDeck;

	// Token: 0x04001E8C RID: 7820
	private static DeckRuleset s_standardRuleset;

	// Token: 0x04001E8D RID: 7821
	private static DeckRuleset s_wildRuleset;

	// Token: 0x0200070F RID: 1807
	public enum DeckRulesetConstant
	{
		// Token: 0x04003123 RID: 12579
		UnknownRuleset,
		// Token: 0x04003124 RID: 12580
		WildRuleset,
		// Token: 0x04003125 RID: 12581
		StandardRuleset
	}
}
