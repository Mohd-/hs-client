using System;
using System.Collections.Generic;

// Token: 0x02000148 RID: 328
public class SubsetDbfRecord : DbfRecord
{
	// Token: 0x0600112D RID: 4397 RVA: 0x00049DA4 File Offset: 0x00047FA4
	public override object GetVar(string name)
	{
		if (name != null)
		{
			if (SubsetDbfRecord.<>f__switch$map49 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
				dictionary.Add("ID", 0);
				SubsetDbfRecord.<>f__switch$map49 = dictionary;
			}
			int num;
			if (SubsetDbfRecord.<>f__switch$map49.TryGetValue(name, ref num))
			{
				if (num == 0)
				{
					return base.ID;
				}
			}
		}
		return null;
	}

	// Token: 0x0600112E RID: 4398 RVA: 0x00049E08 File Offset: 0x00048008
	public override void SetVar(string name, object val)
	{
		if (name != null)
		{
			if (SubsetDbfRecord.<>f__switch$map4A == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
				dictionary.Add("ID", 0);
				SubsetDbfRecord.<>f__switch$map4A = dictionary;
			}
			int num;
			if (SubsetDbfRecord.<>f__switch$map4A.TryGetValue(name, ref num))
			{
				if (num == 0)
				{
					base.SetID((int)val);
				}
			}
		}
	}

	// Token: 0x0600112F RID: 4399 RVA: 0x00049E70 File Offset: 0x00048070
	public override Type GetVarType(string name)
	{
		if (name != null)
		{
			if (SubsetDbfRecord.<>f__switch$map4B == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
				dictionary.Add("ID", 0);
				SubsetDbfRecord.<>f__switch$map4B = dictionary;
			}
			int num;
			if (SubsetDbfRecord.<>f__switch$map4B.TryGetValue(name, ref num))
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
