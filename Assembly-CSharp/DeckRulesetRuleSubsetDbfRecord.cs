using System;
using System.Collections.Generic;

// Token: 0x0200013F RID: 319
public class DeckRulesetRuleSubsetDbfRecord : DbfRecord
{
	// Token: 0x170002B0 RID: 688
	// (get) Token: 0x06001097 RID: 4247 RVA: 0x00047B06 File Offset: 0x00045D06
	[DbfField("DECK_RULESET_RULE_ID", "which DECK_RULESET_RULE.ID")]
	public int DeckRulesetRuleId
	{
		get
		{
			return this.m_DeckRulesetRuleId;
		}
	}

	// Token: 0x170002B1 RID: 689
	// (get) Token: 0x06001098 RID: 4248 RVA: 0x00047B0E File Offset: 0x00045D0E
	[DbfField("SUBSET_ID", "a SUBSET.ID parameter to the rule")]
	public int SubsetId
	{
		get
		{
			return this.m_SubsetId;
		}
	}

	// Token: 0x06001099 RID: 4249 RVA: 0x00047B16 File Offset: 0x00045D16
	public void SetDeckRulesetRuleId(int v)
	{
		this.m_DeckRulesetRuleId = v;
	}

	// Token: 0x0600109A RID: 4250 RVA: 0x00047B1F File Offset: 0x00045D1F
	public void SetSubsetId(int v)
	{
		this.m_SubsetId = v;
	}

	// Token: 0x0600109B RID: 4251 RVA: 0x00047B28 File Offset: 0x00045D28
	public override object GetVar(string name)
	{
		if (name != null)
		{
			if (DeckRulesetRuleSubsetDbfRecord.<>f__switch$map2B == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("DECK_RULESET_RULE_ID", 0);
				dictionary.Add("SUBSET_ID", 1);
				DeckRulesetRuleSubsetDbfRecord.<>f__switch$map2B = dictionary;
			}
			int num;
			if (DeckRulesetRuleSubsetDbfRecord.<>f__switch$map2B.TryGetValue(name, ref num))
			{
				if (num == 0)
				{
					return this.DeckRulesetRuleId;
				}
				if (num == 1)
				{
					return this.SubsetId;
				}
			}
		}
		return null;
	}

	// Token: 0x0600109C RID: 4252 RVA: 0x00047BAC File Offset: 0x00045DAC
	public override void SetVar(string name, object val)
	{
		if (name != null)
		{
			if (DeckRulesetRuleSubsetDbfRecord.<>f__switch$map2C == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("DECK_RULESET_RULE_ID", 0);
				dictionary.Add("SUBSET_ID", 1);
				DeckRulesetRuleSubsetDbfRecord.<>f__switch$map2C = dictionary;
			}
			int num;
			if (DeckRulesetRuleSubsetDbfRecord.<>f__switch$map2C.TryGetValue(name, ref num))
			{
				if (num != 0)
				{
					if (num == 1)
					{
						this.SetSubsetId((int)val);
					}
				}
				else
				{
					this.SetDeckRulesetRuleId((int)val);
				}
			}
		}
	}

	// Token: 0x0600109D RID: 4253 RVA: 0x00047C38 File Offset: 0x00045E38
	public override Type GetVarType(string name)
	{
		if (name != null)
		{
			if (DeckRulesetRuleSubsetDbfRecord.<>f__switch$map2D == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("DECK_RULESET_RULE_ID", 0);
				dictionary.Add("SUBSET_ID", 1);
				DeckRulesetRuleSubsetDbfRecord.<>f__switch$map2D = dictionary;
			}
			int num;
			if (DeckRulesetRuleSubsetDbfRecord.<>f__switch$map2D.TryGetValue(name, ref num))
			{
				if (num == 0)
				{
					return typeof(int);
				}
				if (num == 1)
				{
					return typeof(int);
				}
			}
		}
		return null;
	}

	// Token: 0x040008D2 RID: 2258
	private int m_DeckRulesetRuleId;

	// Token: 0x040008D3 RID: 2259
	private int m_SubsetId;
}
