using System;
using System.Collections.Generic;

// Token: 0x02000137 RID: 311
public class BannerDbfRecord : DbfRecord
{
	// Token: 0x1700027B RID: 635
	// (get) Token: 0x0600100D RID: 4109 RVA: 0x00045B13 File Offset: 0x00043D13
	[DbfField("NOTE_DESC", "designer note")]
	public string NoteDesc
	{
		get
		{
			return this.m_NoteDesc;
		}
	}

	// Token: 0x1700027C RID: 636
	// (get) Token: 0x0600100E RID: 4110 RVA: 0x00045B1B File Offset: 0x00043D1B
	[DbfField("TEXT", "")]
	public DbfLocValue Text
	{
		get
		{
			return this.m_Text;
		}
	}

	// Token: 0x1700027D RID: 637
	// (get) Token: 0x0600100F RID: 4111 RVA: 0x00045B23 File Offset: 0x00043D23
	[DbfField("PREFAB", "")]
	public string Prefab
	{
		get
		{
			return this.m_Prefab;
		}
	}

	// Token: 0x06001010 RID: 4112 RVA: 0x00045B2B File Offset: 0x00043D2B
	public void SetNoteDesc(string v)
	{
		this.m_NoteDesc = v;
	}

	// Token: 0x06001011 RID: 4113 RVA: 0x00045B34 File Offset: 0x00043D34
	public void SetText(DbfLocValue v)
	{
		this.m_Text = v;
		v.SetDebugInfo(base.ID, "TEXT");
	}

	// Token: 0x06001012 RID: 4114 RVA: 0x00045B4E File Offset: 0x00043D4E
	public void SetPrefab(string v)
	{
		this.m_Prefab = v;
	}

	// Token: 0x06001013 RID: 4115 RVA: 0x00045B58 File Offset: 0x00043D58
	public override object GetVar(string name)
	{
		if (name != null)
		{
			if (BannerDbfRecord.<>f__switch$map10 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("TEXT", 2);
				dictionary.Add("PREFAB", 3);
				BannerDbfRecord.<>f__switch$map10 = dictionary;
			}
			int num;
			if (BannerDbfRecord.<>f__switch$map10.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return base.ID;
				case 1:
					return this.NoteDesc;
				case 2:
					return this.Text;
				case 3:
					return this.Prefab;
				}
			}
		}
		return null;
	}

	// Token: 0x06001014 RID: 4116 RVA: 0x00045C04 File Offset: 0x00043E04
	public override void SetVar(string name, object val)
	{
		if (name != null)
		{
			if (BannerDbfRecord.<>f__switch$map11 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("TEXT", 2);
				dictionary.Add("PREFAB", 3);
				BannerDbfRecord.<>f__switch$map11 = dictionary;
			}
			int num;
			if (BannerDbfRecord.<>f__switch$map11.TryGetValue(name, ref num))
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
					this.SetText((DbfLocValue)val);
					break;
				case 3:
					this.SetPrefab((string)val);
					break;
				}
			}
		}
	}

	// Token: 0x06001015 RID: 4117 RVA: 0x00045CD4 File Offset: 0x00043ED4
	public override Type GetVarType(string name)
	{
		if (name != null)
		{
			if (BannerDbfRecord.<>f__switch$map12 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("TEXT", 2);
				dictionary.Add("PREFAB", 3);
				BannerDbfRecord.<>f__switch$map12 = dictionary;
			}
			int num;
			if (BannerDbfRecord.<>f__switch$map12.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return typeof(int);
				case 1:
					return typeof(string);
				case 2:
					return typeof(DbfLocValue);
				case 3:
					return typeof(string);
				}
			}
		}
		return null;
	}

	// Token: 0x04000885 RID: 2181
	private string m_NoteDesc;

	// Token: 0x04000886 RID: 2182
	private DbfLocValue m_Text;

	// Token: 0x04000887 RID: 2183
	private string m_Prefab;
}
