using System;
using System.Collections.Generic;

// Token: 0x0200013E RID: 318
public class DeckRulesetRuleDbfRecord : DbfRecord
{
	// Token: 0x170002A4 RID: 676
	// (get) Token: 0x0600107B RID: 4219 RVA: 0x000474C9 File Offset: 0x000456C9
	[DbfField("DECK_RULESET_ID", "which DECK_RULESET.ID does this rule belong to")]
	public int DeckRulesetId
	{
		get
		{
			return this.m_DeckRulesetId;
		}
	}

	// Token: 0x170002A5 RID: 677
	// (get) Token: 0x0600107C RID: 4220 RVA: 0x000474D1 File Offset: 0x000456D1
	[DbfField("APPLIES_TO_SUBSET_ID", "constrain this rule to apply only to cards that are in this specified SUBSET.ID. when this column value is 0, it means this rule applies to all cards within the deck.")]
	public int AppliesToSubsetId
	{
		get
		{
			return this.m_AppliesToSubsetId;
		}
	}

	// Token: 0x170002A6 RID: 678
	// (get) Token: 0x0600107D RID: 4221 RVA: 0x000474D9 File Offset: 0x000456D9
	[DbfField("APPLIES_TO_IS_NOT", "when 1 (true), means this the rule applies to all cards NOT in the specified APPLIES_TO_SUBSET_ID; otherwise, it applies to cards that ARE in the specified subset.")]
	public bool AppliesToIsNot
	{
		get
		{
			return this.m_AppliesToIsNot;
		}
	}

	// Token: 0x170002A7 RID: 679
	// (get) Token: 0x0600107E RID: 4222 RVA: 0x000474E1 File Offset: 0x000456E1
	[DbfField("RULE_TYPE", "which rule is being applied to this DECK_RULESET - the enum values and string mappings are in server code.")]
	public string RuleType
	{
		get
		{
			return this.m_RuleType;
		}
	}

	// Token: 0x170002A8 RID: 680
	// (get) Token: 0x0600107F RID: 4223 RVA: 0x000474E9 File Offset: 0x000456E9
	[DbfField("RULE_IS_NOT", "whether or not this rule should be validated as successful based on the negation of the parameters")]
	public bool RuleIsNot
	{
		get
		{
			return this.m_RuleIsNot;
		}
	}

	// Token: 0x170002A9 RID: 681
	// (get) Token: 0x06001080 RID: 4224 RVA: 0x000474F1 File Offset: 0x000456F1
	[DbfField("MIN_VALUE", "value range parameter for the rule.")]
	public int MinValue
	{
		get
		{
			return this.m_MinValue;
		}
	}

	// Token: 0x170002AA RID: 682
	// (get) Token: 0x06001081 RID: 4225 RVA: 0x000474F9 File Offset: 0x000456F9
	[DbfField("MAX_VALUE", "value range parameter for the rule.")]
	public int MaxValue
	{
		get
		{
			return this.m_MaxValue;
		}
	}

	// Token: 0x170002AB RID: 683
	// (get) Token: 0x06001082 RID: 4226 RVA: 0x00047501 File Offset: 0x00045701
	[DbfField("TAG", "for rules that specify tag values, this column specifies which tag.")]
	public int Tag
	{
		get
		{
			return this.m_Tag;
		}
	}

	// Token: 0x170002AC RID: 684
	// (get) Token: 0x06001083 RID: 4227 RVA: 0x00047509 File Offset: 0x00045709
	[DbfField("TAG_MIN_VALUE", "tag value range parameter for the rule.")]
	public int TagMinValue
	{
		get
		{
			return this.m_TagMinValue;
		}
	}

	// Token: 0x170002AD RID: 685
	// (get) Token: 0x06001084 RID: 4228 RVA: 0x00047511 File Offset: 0x00045711
	[DbfField("TAG_MAX_VALUE", "tag value range parameter for the rule.")]
	public int TagMaxValue
	{
		get
		{
			return this.m_TagMaxValue;
		}
	}

	// Token: 0x170002AE RID: 686
	// (get) Token: 0x06001085 RID: 4229 RVA: 0x00047519 File Offset: 0x00045719
	[DbfField("STRING_VALUE", "string parameter for the rule.")]
	public string StringValue
	{
		get
		{
			return this.m_StringValue;
		}
	}

	// Token: 0x170002AF RID: 687
	// (get) Token: 0x06001086 RID: 4230 RVA: 0x00047521 File Offset: 0x00045721
	[DbfField("ERROR_STRING", "when deck validation fails on this rule, this error message is displayed to the player.")]
	public DbfLocValue ErrorString
	{
		get
		{
			return this.m_ErrorString;
		}
	}

	// Token: 0x06001087 RID: 4231 RVA: 0x00047529 File Offset: 0x00045729
	public void SetDeckRulesetId(int v)
	{
		this.m_DeckRulesetId = v;
	}

	// Token: 0x06001088 RID: 4232 RVA: 0x00047532 File Offset: 0x00045732
	public void SetAppliesToSubsetId(int v)
	{
		this.m_AppliesToSubsetId = v;
	}

	// Token: 0x06001089 RID: 4233 RVA: 0x0004753B File Offset: 0x0004573B
	public void SetAppliesToIsNot(bool v)
	{
		this.m_AppliesToIsNot = v;
	}

	// Token: 0x0600108A RID: 4234 RVA: 0x00047544 File Offset: 0x00045744
	public void SetRuleType(string v)
	{
		this.m_RuleType = v;
	}

	// Token: 0x0600108B RID: 4235 RVA: 0x0004754D File Offset: 0x0004574D
	public void SetRuleIsNot(bool v)
	{
		this.m_RuleIsNot = v;
	}

	// Token: 0x0600108C RID: 4236 RVA: 0x00047556 File Offset: 0x00045756
	public void SetMinValue(int v)
	{
		this.m_MinValue = v;
	}

	// Token: 0x0600108D RID: 4237 RVA: 0x0004755F File Offset: 0x0004575F
	public void SetMaxValue(int v)
	{
		this.m_MaxValue = v;
	}

	// Token: 0x0600108E RID: 4238 RVA: 0x00047568 File Offset: 0x00045768
	public void SetTag(int v)
	{
		this.m_Tag = v;
	}

	// Token: 0x0600108F RID: 4239 RVA: 0x00047571 File Offset: 0x00045771
	public void SetTagMinValue(int v)
	{
		this.m_TagMinValue = v;
	}

	// Token: 0x06001090 RID: 4240 RVA: 0x0004757A File Offset: 0x0004577A
	public void SetTagMaxValue(int v)
	{
		this.m_TagMaxValue = v;
	}

	// Token: 0x06001091 RID: 4241 RVA: 0x00047583 File Offset: 0x00045783
	public void SetStringValue(string v)
	{
		this.m_StringValue = v;
	}

	// Token: 0x06001092 RID: 4242 RVA: 0x0004758C File Offset: 0x0004578C
	public void SetErrorString(DbfLocValue v)
	{
		this.m_ErrorString = v;
		v.SetDebugInfo(base.ID, "ERROR_STRING");
	}

	// Token: 0x06001093 RID: 4243 RVA: 0x000475A8 File Offset: 0x000457A8
	public override object GetVar(string name)
	{
		if (name != null)
		{
			if (DeckRulesetRuleDbfRecord.<>f__switch$map28 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(13);
				dictionary.Add("ID", 0);
				dictionary.Add("DECK_RULESET_ID", 1);
				dictionary.Add("APPLIES_TO_SUBSET_ID", 2);
				dictionary.Add("APPLIES_TO_IS_NOT", 3);
				dictionary.Add("RULE_TYPE", 4);
				dictionary.Add("RULE_IS_NOT", 5);
				dictionary.Add("MIN_VALUE", 6);
				dictionary.Add("MAX_VALUE", 7);
				dictionary.Add("TAG", 8);
				dictionary.Add("TAG_MIN_VALUE", 9);
				dictionary.Add("TAG_MAX_VALUE", 10);
				dictionary.Add("STRING_VALUE", 11);
				dictionary.Add("ERROR_STRING", 12);
				DeckRulesetRuleDbfRecord.<>f__switch$map28 = dictionary;
			}
			int num;
			if (DeckRulesetRuleDbfRecord.<>f__switch$map28.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return base.ID;
				case 1:
					return this.DeckRulesetId;
				case 2:
					return this.AppliesToSubsetId;
				case 3:
					return this.AppliesToIsNot;
				case 4:
					return this.RuleType;
				case 5:
					return this.RuleIsNot;
				case 6:
					return this.MinValue;
				case 7:
					return this.MaxValue;
				case 8:
					return this.Tag;
				case 9:
					return this.TagMinValue;
				case 10:
					return this.TagMaxValue;
				case 11:
					return this.StringValue;
				case 12:
					return this.ErrorString;
				}
			}
		}
		return null;
	}

	// Token: 0x06001094 RID: 4244 RVA: 0x00047754 File Offset: 0x00045954
	public override void SetVar(string name, object val)
	{
		if (name != null)
		{
			if (DeckRulesetRuleDbfRecord.<>f__switch$map29 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(13);
				dictionary.Add("ID", 0);
				dictionary.Add("DECK_RULESET_ID", 1);
				dictionary.Add("APPLIES_TO_SUBSET_ID", 2);
				dictionary.Add("APPLIES_TO_IS_NOT", 3);
				dictionary.Add("RULE_TYPE", 4);
				dictionary.Add("RULE_IS_NOT", 5);
				dictionary.Add("MIN_VALUE", 6);
				dictionary.Add("MAX_VALUE", 7);
				dictionary.Add("TAG", 8);
				dictionary.Add("TAG_MIN_VALUE", 9);
				dictionary.Add("TAG_MAX_VALUE", 10);
				dictionary.Add("STRING_VALUE", 11);
				dictionary.Add("ERROR_STRING", 12);
				DeckRulesetRuleDbfRecord.<>f__switch$map29 = dictionary;
			}
			int num;
			if (DeckRulesetRuleDbfRecord.<>f__switch$map29.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					base.SetID((int)val);
					break;
				case 1:
					this.SetDeckRulesetId((int)val);
					break;
				case 2:
					this.SetAppliesToSubsetId((int)val);
					break;
				case 3:
					this.SetAppliesToIsNot((bool)val);
					break;
				case 4:
					this.SetRuleType((string)val);
					break;
				case 5:
					this.SetRuleIsNot((bool)val);
					break;
				case 6:
					this.SetMinValue((int)val);
					break;
				case 7:
					this.SetMaxValue((int)val);
					break;
				case 8:
					this.SetTag((int)val);
					break;
				case 9:
					this.SetTagMinValue((int)val);
					break;
				case 10:
					this.SetTagMaxValue((int)val);
					break;
				case 11:
					this.SetStringValue((string)val);
					break;
				case 12:
					this.SetErrorString((DbfLocValue)val);
					break;
				}
			}
		}
	}

	// Token: 0x06001095 RID: 4245 RVA: 0x00047950 File Offset: 0x00045B50
	public override Type GetVarType(string name)
	{
		if (name != null)
		{
			if (DeckRulesetRuleDbfRecord.<>f__switch$map2A == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(13);
				dictionary.Add("ID", 0);
				dictionary.Add("DECK_RULESET_ID", 1);
				dictionary.Add("APPLIES_TO_SUBSET_ID", 2);
				dictionary.Add("APPLIES_TO_IS_NOT", 3);
				dictionary.Add("RULE_TYPE", 4);
				dictionary.Add("RULE_IS_NOT", 5);
				dictionary.Add("MIN_VALUE", 6);
				dictionary.Add("MAX_VALUE", 7);
				dictionary.Add("TAG", 8);
				dictionary.Add("TAG_MIN_VALUE", 9);
				dictionary.Add("TAG_MAX_VALUE", 10);
				dictionary.Add("STRING_VALUE", 11);
				dictionary.Add("ERROR_STRING", 12);
				DeckRulesetRuleDbfRecord.<>f__switch$map2A = dictionary;
			}
			int num;
			if (DeckRulesetRuleDbfRecord.<>f__switch$map2A.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return typeof(int);
				case 1:
					return typeof(int);
				case 2:
					return typeof(int);
				case 3:
					return typeof(bool);
				case 4:
					return typeof(string);
				case 5:
					return typeof(bool);
				case 6:
					return typeof(int);
				case 7:
					return typeof(int);
				case 8:
					return typeof(int);
				case 9:
					return typeof(int);
				case 10:
					return typeof(int);
				case 11:
					return typeof(string);
				case 12:
					return typeof(DbfLocValue);
				}
			}
		}
		return null;
	}

	// Token: 0x040008C3 RID: 2243
	private int m_DeckRulesetId;

	// Token: 0x040008C4 RID: 2244
	private int m_AppliesToSubsetId;

	// Token: 0x040008C5 RID: 2245
	private bool m_AppliesToIsNot;

	// Token: 0x040008C6 RID: 2246
	private string m_RuleType;

	// Token: 0x040008C7 RID: 2247
	private bool m_RuleIsNot;

	// Token: 0x040008C8 RID: 2248
	private int m_MinValue;

	// Token: 0x040008C9 RID: 2249
	private int m_MaxValue;

	// Token: 0x040008CA RID: 2250
	private int m_Tag;

	// Token: 0x040008CB RID: 2251
	private int m_TagMinValue;

	// Token: 0x040008CC RID: 2252
	private int m_TagMaxValue;

	// Token: 0x040008CD RID: 2253
	private string m_StringValue;

	// Token: 0x040008CE RID: 2254
	private DbfLocValue m_ErrorString;
}
