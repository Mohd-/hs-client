using System;
using System.Collections.Generic;

// Token: 0x0200073E RID: 1854
public class DeckRule_IsInAnySubset : DeckRule
{
	// Token: 0x06004B43 RID: 19267 RVA: 0x00167ADA File Offset: 0x00165CDA
	public DeckRule_IsInAnySubset(DeckRulesetRuleDbfRecord record) : base(DeckRule.RuleType.IS_IN_ANY_SUBSET, record)
	{
	}

	// Token: 0x06004B44 RID: 19268 RVA: 0x00167AE4 File Offset: 0x00165CE4
	public DeckRule_IsInAnySubset(int subsetId)
	{
		this.m_subsets = new List<HashSet<string>>();
		this.m_subsets.Add(GameDbf.GetIndex().GetSubsetById(subsetId));
	}

	// Token: 0x06004B45 RID: 19269 RVA: 0x00167B10 File Offset: 0x00165D10
	public override bool Filter(EntityDef def)
	{
		string cardId = def.GetCardId();
		if (!base.AppliesTo(cardId))
		{
			return base.GetResult(true);
		}
		foreach (HashSet<string> hashSet in this.m_subsets)
		{
			if (hashSet.Contains(cardId))
			{
				return base.GetResult(true);
			}
		}
		return base.GetResult(false);
	}
}
