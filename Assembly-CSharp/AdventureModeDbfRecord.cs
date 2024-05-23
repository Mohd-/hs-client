using System;
using System.Collections.Generic;

// Token: 0x02000136 RID: 310
public class AdventureModeDbfRecord : DbfRecord
{
	// Token: 0x1700027A RID: 634
	// (get) Token: 0x06001007 RID: 4103 RVA: 0x00045972 File Offset: 0x00043B72
	[DbfField("NOTE_DESC", "designer note")]
	public string NoteDesc
	{
		get
		{
			return this.m_NoteDesc;
		}
	}

	// Token: 0x06001008 RID: 4104 RVA: 0x0004597A File Offset: 0x00043B7A
	public void SetNoteDesc(string v)
	{
		this.m_NoteDesc = v;
	}

	// Token: 0x06001009 RID: 4105 RVA: 0x00045984 File Offset: 0x00043B84
	public override object GetVar(string name)
	{
		if (name != null)
		{
			if (AdventureModeDbfRecord.<>f__switch$mapD == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				AdventureModeDbfRecord.<>f__switch$mapD = dictionary;
			}
			int num;
			if (AdventureModeDbfRecord.<>f__switch$mapD.TryGetValue(name, ref num))
			{
				if (num == 0)
				{
					return base.ID;
				}
				if (num == 1)
				{
					return this.NoteDesc;
				}
			}
		}
		return null;
	}

	// Token: 0x0600100A RID: 4106 RVA: 0x00045A00 File Offset: 0x00043C00
	public override void SetVar(string name, object val)
	{
		if (name != null)
		{
			if (AdventureModeDbfRecord.<>f__switch$mapE == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				AdventureModeDbfRecord.<>f__switch$mapE = dictionary;
			}
			int num;
			if (AdventureModeDbfRecord.<>f__switch$mapE.TryGetValue(name, ref num))
			{
				if (num != 0)
				{
					if (num == 1)
					{
						this.SetNoteDesc((string)val);
					}
				}
				else
				{
					base.SetID((int)val);
				}
			}
		}
	}

	// Token: 0x0600100B RID: 4107 RVA: 0x00045A8C File Offset: 0x00043C8C
	public override Type GetVarType(string name)
	{
		if (name != null)
		{
			if (AdventureModeDbfRecord.<>f__switch$mapF == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				AdventureModeDbfRecord.<>f__switch$mapF = dictionary;
			}
			int num;
			if (AdventureModeDbfRecord.<>f__switch$mapF.TryGetValue(name, ref num))
			{
				if (num == 0)
				{
					return typeof(int);
				}
				if (num == 1)
				{
					return typeof(string);
				}
			}
		}
		return null;
	}

	// Token: 0x04000881 RID: 2177
	private string m_NoteDesc;
}
