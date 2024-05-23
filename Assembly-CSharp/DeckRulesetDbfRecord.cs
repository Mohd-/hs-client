using System;
using System.Collections.Generic;

// Token: 0x0200013D RID: 317
public class DeckRulesetDbfRecord : DbfRecord
{
	// Token: 0x06001077 RID: 4215 RVA: 0x00047394 File Offset: 0x00045594
	public override object GetVar(string name)
	{
		if (name != null)
		{
			if (DeckRulesetDbfRecord.<>f__switch$map25 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
				dictionary.Add("ID", 0);
				DeckRulesetDbfRecord.<>f__switch$map25 = dictionary;
			}
			int num;
			if (DeckRulesetDbfRecord.<>f__switch$map25.TryGetValue(name, ref num))
			{
				if (num == 0)
				{
					return base.ID;
				}
			}
		}
		return null;
	}

	// Token: 0x06001078 RID: 4216 RVA: 0x000473F8 File Offset: 0x000455F8
	public override void SetVar(string name, object val)
	{
		if (name != null)
		{
			if (DeckRulesetDbfRecord.<>f__switch$map26 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
				dictionary.Add("ID", 0);
				DeckRulesetDbfRecord.<>f__switch$map26 = dictionary;
			}
			int num;
			if (DeckRulesetDbfRecord.<>f__switch$map26.TryGetValue(name, ref num))
			{
				if (num == 0)
				{
					base.SetID((int)val);
				}
			}
		}
	}

	// Token: 0x06001079 RID: 4217 RVA: 0x00047460 File Offset: 0x00045660
	public override Type GetVarType(string name)
	{
		if (name != null)
		{
			if (DeckRulesetDbfRecord.<>f__switch$map27 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
				dictionary.Add("ID", 0);
				DeckRulesetDbfRecord.<>f__switch$map27 = dictionary;
			}
			int num;
			if (DeckRulesetDbfRecord.<>f__switch$map27.TryGetValue(name, ref num))
			{
				if (num == 0)
				{
					return typeof(int);
				}
			}
		}
		return null;
	}
}
