using System;
using System.Collections.Generic;

// Token: 0x02000138 RID: 312
public class BoardDbfRecord : DbfRecord
{
	// Token: 0x1700027E RID: 638
	// (get) Token: 0x06001017 RID: 4119 RVA: 0x00045D92 File Offset: 0x00043F92
	[DbfField("NOTE_DESC", "designer note")]
	public string NoteDesc
	{
		get
		{
			return this.m_NoteDesc;
		}
	}

	// Token: 0x1700027F RID: 639
	// (get) Token: 0x06001018 RID: 4120 RVA: 0x00045D9A File Offset: 0x00043F9A
	[DbfField("PREFAB", "")]
	public string Prefab
	{
		get
		{
			return this.m_Prefab;
		}
	}

	// Token: 0x06001019 RID: 4121 RVA: 0x00045DA2 File Offset: 0x00043FA2
	public void SetNoteDesc(string v)
	{
		this.m_NoteDesc = v;
	}

	// Token: 0x0600101A RID: 4122 RVA: 0x00045DAB File Offset: 0x00043FAB
	public void SetPrefab(string v)
	{
		this.m_Prefab = v;
	}

	// Token: 0x0600101B RID: 4123 RVA: 0x00045DB4 File Offset: 0x00043FB4
	public override object GetVar(string name)
	{
		if (name != null)
		{
			if (BoardDbfRecord.<>f__switch$map13 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("PREFAB", 2);
				BoardDbfRecord.<>f__switch$map13 = dictionary;
			}
			int num;
			if (BoardDbfRecord.<>f__switch$map13.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return base.ID;
				case 1:
					return this.NoteDesc;
				case 2:
					return this.Prefab;
				}
			}
		}
		return null;
	}

	// Token: 0x0600101C RID: 4124 RVA: 0x00045E48 File Offset: 0x00044048
	public override void SetVar(string name, object val)
	{
		if (name != null)
		{
			if (BoardDbfRecord.<>f__switch$map14 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("PREFAB", 2);
				BoardDbfRecord.<>f__switch$map14 = dictionary;
			}
			int num;
			if (BoardDbfRecord.<>f__switch$map14.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					base.SetID((int)val);
					break;
				case 1:
					this.SetNoteDesc((string)val);
					break;
				case 2:
					this.SetPrefab((string)val);
					break;
				}
			}
		}
	}

	// Token: 0x0600101D RID: 4125 RVA: 0x00045EF4 File Offset: 0x000440F4
	public override Type GetVarType(string name)
	{
		if (name != null)
		{
			if (BoardDbfRecord.<>f__switch$map15 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("PREFAB", 2);
				BoardDbfRecord.<>f__switch$map15 = dictionary;
			}
			int num;
			if (BoardDbfRecord.<>f__switch$map15.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return typeof(int);
				case 1:
					return typeof(string);
				case 2:
					return typeof(string);
				}
			}
		}
		return null;
	}

	// Token: 0x0400088B RID: 2187
	private string m_NoteDesc;

	// Token: 0x0400088C RID: 2188
	private string m_Prefab;
}
