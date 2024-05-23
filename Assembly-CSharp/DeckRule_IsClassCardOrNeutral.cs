using System;
using UnityEngine;

// Token: 0x0200073D RID: 1853
public class DeckRule_IsClassCardOrNeutral : DeckRule
{
	// Token: 0x06004B40 RID: 19264 RVA: 0x00167A5D File Offset: 0x00165C5D
	public DeckRule_IsClassCardOrNeutral()
	{
		this.m_ruleType = DeckRule.RuleType.IS_CLASS_CARD_OR_NEUTRAL;
	}

	// Token: 0x06004B41 RID: 19265 RVA: 0x00167A6C File Offset: 0x00165C6C
	public DeckRule_IsClassCardOrNeutral(DeckRulesetRuleDbfRecord record) : base(DeckRule.RuleType.IS_CLASS_CARD_OR_NEUTRAL, record)
	{
		if (this.m_ruleIsNot)
		{
			Debug.LogError("IS_CLASS_CARD_OR_NEUTRAL rules do not support \"is not\".");
		}
	}

	// Token: 0x06004B42 RID: 19266 RVA: 0x00167A8C File Offset: 0x00165C8C
	public override bool Filter(EntityDef def)
	{
		if (!base.AppliesTo(def.GetCardId()))
		{
			return true;
		}
		TAG_CLASS @class = def.GetClass();
		CollectionDeck taggedDeck = CollectionManager.Get().GetTaggedDeck(CollectionManager.DeckTag.Editing);
		return taggedDeck == null || @class == TAG_CLASS.INVALID || @class == taggedDeck.GetClass();
	}
}
