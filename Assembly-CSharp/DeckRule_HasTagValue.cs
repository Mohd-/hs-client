using System;

// Token: 0x02000740 RID: 1856
public class DeckRule_HasTagValue : DeckRule
{
	// Token: 0x06004B49 RID: 19273 RVA: 0x00167C3A File Offset: 0x00165E3A
	public DeckRule_HasTagValue(DeckRulesetRuleDbfRecord record) : base(DeckRule.RuleType.IS_IN_ANY_SUBSET, record)
	{
	}

	// Token: 0x06004B4A RID: 19274 RVA: 0x00167C44 File Offset: 0x00165E44
	public override bool Filter(EntityDef def)
	{
		if (!base.AppliesTo(def.GetCardId()))
		{
			return true;
		}
		int tag = def.GetTag(this.m_tag);
		bool val = true;
		if (tag < this.m_tagMinValue || tag > this.m_tagMaxValue)
		{
			val = false;
		}
		return base.GetResult(val);
	}
}
