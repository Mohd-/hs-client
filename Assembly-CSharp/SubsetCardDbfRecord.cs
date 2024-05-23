using System;
using System.Collections.Generic;

// Token: 0x02000149 RID: 329
public class SubsetCardDbfRecord : DbfRecord
{
	// Token: 0x170002E9 RID: 745
	// (get) Token: 0x06001131 RID: 4401 RVA: 0x00049ED9 File Offset: 0x000480D9
	[DbfField("SUBSET_ID", "the SUBSET.ID this card is for")]
	public int SubsetId
	{
		get
		{
			return this.m_SubsetId;
		}
	}

	// Token: 0x170002EA RID: 746
	// (get) Token: 0x06001132 RID: 4402 RVA: 0x00049EE1 File Offset: 0x000480E1
	[DbfField("CARD_ID", "a CARD.ID that validates against this subset's rules")]
	public int CardId
	{
		get
		{
			return this.m_CardId;
		}
	}

	// Token: 0x06001133 RID: 4403 RVA: 0x00049EE9 File Offset: 0x000480E9
	public void SetSubsetId(int v)
	{
		this.m_SubsetId = v;
	}

	// Token: 0x06001134 RID: 4404 RVA: 0x00049EF2 File Offset: 0x000480F2
	public void SetCardId(int v)
	{
		this.m_CardId = v;
	}

	// Token: 0x06001135 RID: 4405 RVA: 0x00049EFC File Offset: 0x000480FC
	public override object GetVar(string name)
	{
		if (name != null)
		{
			if (SubsetCardDbfRecord.<>f__switch$map46 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("SUBSET_ID", 0);
				dictionary.Add("CARD_ID", 1);
				SubsetCardDbfRecord.<>f__switch$map46 = dictionary;
			}
			int num;
			if (SubsetCardDbfRecord.<>f__switch$map46.TryGetValue(name, ref num))
			{
				if (num == 0)
				{
					return this.SubsetId;
				}
				if (num == 1)
				{
					return this.CardId;
				}
			}
		}
		return null;
	}

	// Token: 0x06001136 RID: 4406 RVA: 0x00049F80 File Offset: 0x00048180
	public override void SetVar(string name, object val)
	{
		if (name != null)
		{
			if (SubsetCardDbfRecord.<>f__switch$map47 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("SUBSET_ID", 0);
				dictionary.Add("CARD_ID", 1);
				SubsetCardDbfRecord.<>f__switch$map47 = dictionary;
			}
			int num;
			if (SubsetCardDbfRecord.<>f__switch$map47.TryGetValue(name, ref num))
			{
				if (num != 0)
				{
					if (num == 1)
					{
						this.SetCardId((int)val);
					}
				}
				else
				{
					this.SetSubsetId((int)val);
				}
			}
		}
	}

	// Token: 0x06001137 RID: 4407 RVA: 0x0004A00C File Offset: 0x0004820C
	public override Type GetVarType(string name)
	{
		if (name != null)
		{
			if (SubsetCardDbfRecord.<>f__switch$map48 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("SUBSET_ID", 0);
				dictionary.Add("CARD_ID", 1);
				SubsetCardDbfRecord.<>f__switch$map48 = dictionary;
			}
			int num;
			if (SubsetCardDbfRecord.<>f__switch$map48.TryGetValue(name, ref num))
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

	// Token: 0x04000929 RID: 2345
	private int m_SubsetId;

	// Token: 0x0400092A RID: 2346
	private int m_CardId;
}
