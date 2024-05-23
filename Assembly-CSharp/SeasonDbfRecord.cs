using System;
using System.Collections.Generic;

// Token: 0x02000147 RID: 327
public class SeasonDbfRecord : DbfRecord
{
	// Token: 0x170002E6 RID: 742
	// (get) Token: 0x06001123 RID: 4387 RVA: 0x00049B10 File Offset: 0x00047D10
	[DbfField("NOTE_DESC", "designer note")]
	public string NoteDesc
	{
		get
		{
			return this.m_NoteDesc;
		}
	}

	// Token: 0x170002E7 RID: 743
	// (get) Token: 0x06001124 RID: 4388 RVA: 0x00049B18 File Offset: 0x00047D18
	[DbfField("NAME", "")]
	public DbfLocValue Name
	{
		get
		{
			return this.m_Name;
		}
	}

	// Token: 0x170002E8 RID: 744
	// (get) Token: 0x06001125 RID: 4389 RVA: 0x00049B20 File Offset: 0x00047D20
	[DbfField("SEASON_START_NAME", "")]
	public DbfLocValue SeasonStartName
	{
		get
		{
			return this.m_SeasonStartName;
		}
	}

	// Token: 0x06001126 RID: 4390 RVA: 0x00049B28 File Offset: 0x00047D28
	public void SetNoteDesc(string v)
	{
		this.m_NoteDesc = v;
	}

	// Token: 0x06001127 RID: 4391 RVA: 0x00049B31 File Offset: 0x00047D31
	public void SetName(DbfLocValue v)
	{
		this.m_Name = v;
		v.SetDebugInfo(base.ID, "NAME");
	}

	// Token: 0x06001128 RID: 4392 RVA: 0x00049B4B File Offset: 0x00047D4B
	public void SetSeasonStartName(DbfLocValue v)
	{
		this.m_SeasonStartName = v;
		v.SetDebugInfo(base.ID, "SEASON_START_NAME");
	}

	// Token: 0x06001129 RID: 4393 RVA: 0x00049B68 File Offset: 0x00047D68
	public override object GetVar(string name)
	{
		if (name != null)
		{
			if (SeasonDbfRecord.<>f__switch$map43 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("NAME", 2);
				dictionary.Add("SEASON_START_NAME", 3);
				SeasonDbfRecord.<>f__switch$map43 = dictionary;
			}
			int num;
			if (SeasonDbfRecord.<>f__switch$map43.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return base.ID;
				case 1:
					return this.NoteDesc;
				case 2:
					return this.Name;
				case 3:
					return this.SeasonStartName;
				}
			}
		}
		return null;
	}

	// Token: 0x0600112A RID: 4394 RVA: 0x00049C14 File Offset: 0x00047E14
	public override void SetVar(string name, object val)
	{
		if (name != null)
		{
			if (SeasonDbfRecord.<>f__switch$map44 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("NAME", 2);
				dictionary.Add("SEASON_START_NAME", 3);
				SeasonDbfRecord.<>f__switch$map44 = dictionary;
			}
			int num;
			if (SeasonDbfRecord.<>f__switch$map44.TryGetValue(name, ref num))
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
					this.SetName((DbfLocValue)val);
					break;
				case 3:
					this.SetSeasonStartName((DbfLocValue)val);
					break;
				}
			}
		}
	}

	// Token: 0x0600112B RID: 4395 RVA: 0x00049CE4 File Offset: 0x00047EE4
	public override Type GetVarType(string name)
	{
		if (name != null)
		{
			if (SeasonDbfRecord.<>f__switch$map45 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("NAME", 2);
				dictionary.Add("SEASON_START_NAME", 3);
				SeasonDbfRecord.<>f__switch$map45 = dictionary;
			}
			int num;
			if (SeasonDbfRecord.<>f__switch$map45.TryGetValue(name, ref num))
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
					return typeof(DbfLocValue);
				}
			}
		}
		return null;
	}

	// Token: 0x04000920 RID: 2336
	private string m_NoteDesc;

	// Token: 0x04000921 RID: 2337
	private DbfLocValue m_Name;

	// Token: 0x04000922 RID: 2338
	private DbfLocValue m_SeasonStartName;
}
