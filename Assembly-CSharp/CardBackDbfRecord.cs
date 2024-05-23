using System;
using System.Collections.Generic;

// Token: 0x0200013B RID: 315
public class CardBackDbfRecord : DbfRecord
{
	// Token: 0x17000296 RID: 662
	// (get) Token: 0x06001053 RID: 4179 RVA: 0x00046B22 File Offset: 0x00044D22
	[DbfField("NOTE_DESC", "designer note")]
	public string NoteDesc
	{
		get
		{
			return this.m_NoteDesc;
		}
	}

	// Token: 0x17000297 RID: 663
	// (get) Token: 0x06001054 RID: 4180 RVA: 0x00046B2A File Offset: 0x00044D2A
	[DbfField("DATA1", "depends on source")]
	public long Data1
	{
		get
		{
			return this.m_Data1;
		}
	}

	// Token: 0x17000298 RID: 664
	// (get) Token: 0x06001055 RID: 4181 RVA: 0x00046B32 File Offset: 0x00044D32
	[DbfField("SOURCE", "how is this card back earned? see server enum TCardBackSource")]
	public string Source
	{
		get
		{
			return this.m_Source;
		}
	}

	// Token: 0x17000299 RID: 665
	// (get) Token: 0x06001056 RID: 4182 RVA: 0x00046B3A File Offset: 0x00044D3A
	[DbfField("ENABLED", "")]
	public bool Enabled
	{
		get
		{
			return this.m_Enabled;
		}
	}

	// Token: 0x1700029A RID: 666
	// (get) Token: 0x06001057 RID: 4183 RVA: 0x00046B42 File Offset: 0x00044D42
	[DbfField("SORT_CATEGORY", "categorical display order of this card back in the collection manager")]
	public int SortCategory
	{
		get
		{
			return this.m_SortCategory;
		}
	}

	// Token: 0x1700029B RID: 667
	// (get) Token: 0x06001058 RID: 4184 RVA: 0x00046B4A File Offset: 0x00044D4A
	[DbfField("SORT_ORDER", "display order of this card back within its SORT_CATEGORY in the collection manager")]
	public int SortOrder
	{
		get
		{
			return this.m_SortOrder;
		}
	}

	// Token: 0x1700029C RID: 668
	// (get) Token: 0x06001059 RID: 4185 RVA: 0x00046B52 File Offset: 0x00044D52
	[DbfField("NAME", "")]
	public DbfLocValue Name
	{
		get
		{
			return this.m_Name;
		}
	}

	// Token: 0x1700029D RID: 669
	// (get) Token: 0x0600105A RID: 4186 RVA: 0x00046B5A File Offset: 0x00044D5A
	[DbfField("PREFAB_NAME", "")]
	public string PrefabName
	{
		get
		{
			return this.m_PrefabName;
		}
	}

	// Token: 0x1700029E RID: 670
	// (get) Token: 0x0600105B RID: 4187 RVA: 0x00046B62 File Offset: 0x00044D62
	[DbfField("DESCRIPTION", "")]
	public DbfLocValue Description
	{
		get
		{
			return this.m_Description;
		}
	}

	// Token: 0x1700029F RID: 671
	// (get) Token: 0x0600105C RID: 4188 RVA: 0x00046B6A File Offset: 0x00044D6A
	[DbfField("SOURCE_DESCRIPTION", "")]
	public DbfLocValue SourceDescription
	{
		get
		{
			return this.m_SourceDescription;
		}
	}

	// Token: 0x0600105D RID: 4189 RVA: 0x00046B72 File Offset: 0x00044D72
	public void SetNoteDesc(string v)
	{
		this.m_NoteDesc = v;
	}

	// Token: 0x0600105E RID: 4190 RVA: 0x00046B7B File Offset: 0x00044D7B
	public void SetData1(long v)
	{
		this.m_Data1 = v;
	}

	// Token: 0x0600105F RID: 4191 RVA: 0x00046B84 File Offset: 0x00044D84
	public void SetSource(string v)
	{
		this.m_Source = v;
	}

	// Token: 0x06001060 RID: 4192 RVA: 0x00046B8D File Offset: 0x00044D8D
	public void SetEnabled(bool v)
	{
		this.m_Enabled = v;
	}

	// Token: 0x06001061 RID: 4193 RVA: 0x00046B96 File Offset: 0x00044D96
	public void SetSortCategory(int v)
	{
		this.m_SortCategory = v;
	}

	// Token: 0x06001062 RID: 4194 RVA: 0x00046B9F File Offset: 0x00044D9F
	public void SetSortOrder(int v)
	{
		this.m_SortOrder = v;
	}

	// Token: 0x06001063 RID: 4195 RVA: 0x00046BA8 File Offset: 0x00044DA8
	public void SetName(DbfLocValue v)
	{
		this.m_Name = v;
		v.SetDebugInfo(base.ID, "NAME");
	}

	// Token: 0x06001064 RID: 4196 RVA: 0x00046BC2 File Offset: 0x00044DC2
	public void SetPrefabName(string v)
	{
		this.m_PrefabName = v;
	}

	// Token: 0x06001065 RID: 4197 RVA: 0x00046BCB File Offset: 0x00044DCB
	public void SetDescription(DbfLocValue v)
	{
		this.m_Description = v;
		v.SetDebugInfo(base.ID, "DESCRIPTION");
	}

	// Token: 0x06001066 RID: 4198 RVA: 0x00046BE5 File Offset: 0x00044DE5
	public void SetSourceDescription(DbfLocValue v)
	{
		this.m_SourceDescription = v;
		v.SetDebugInfo(base.ID, "SOURCE_DESCRIPTION");
	}

	// Token: 0x06001067 RID: 4199 RVA: 0x00046C00 File Offset: 0x00044E00
	public override object GetVar(string name)
	{
		if (name != null)
		{
			if (CardBackDbfRecord.<>f__switch$map19 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(11);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("DATA1", 2);
				dictionary.Add("SOURCE", 3);
				dictionary.Add("ENABLED", 4);
				dictionary.Add("SORT_CATEGORY", 5);
				dictionary.Add("SORT_ORDER", 6);
				dictionary.Add("NAME", 7);
				dictionary.Add("PREFAB_NAME", 8);
				dictionary.Add("DESCRIPTION", 9);
				dictionary.Add("SOURCE_DESCRIPTION", 10);
				CardBackDbfRecord.<>f__switch$map19 = dictionary;
			}
			int num;
			if (CardBackDbfRecord.<>f__switch$map19.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return base.ID;
				case 1:
					return this.NoteDesc;
				case 2:
					return this.Data1;
				case 3:
					return this.Source;
				case 4:
					return this.Enabled;
				case 5:
					return this.SortCategory;
				case 6:
					return this.SortOrder;
				case 7:
					return this.Name;
				case 8:
					return this.PrefabName;
				case 9:
					return this.Description;
				case 10:
					return this.SourceDescription;
				}
			}
		}
		return null;
	}

	// Token: 0x06001068 RID: 4200 RVA: 0x00046D64 File Offset: 0x00044F64
	public override void SetVar(string name, object val)
	{
		if (name != null)
		{
			if (CardBackDbfRecord.<>f__switch$map1A == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(11);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("DATA1", 2);
				dictionary.Add("SOURCE", 3);
				dictionary.Add("ENABLED", 4);
				dictionary.Add("SORT_CATEGORY", 5);
				dictionary.Add("SORT_ORDER", 6);
				dictionary.Add("NAME", 7);
				dictionary.Add("PREFAB_NAME", 8);
				dictionary.Add("DESCRIPTION", 9);
				dictionary.Add("SOURCE_DESCRIPTION", 10);
				CardBackDbfRecord.<>f__switch$map1A = dictionary;
			}
			int num;
			if (CardBackDbfRecord.<>f__switch$map1A.TryGetValue(name, ref num))
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
					this.SetData1((long)val);
					break;
				case 3:
					this.SetSource((string)val);
					break;
				case 4:
					this.SetEnabled((bool)val);
					break;
				case 5:
					this.SetSortCategory((int)val);
					break;
				case 6:
					this.SetSortOrder((int)val);
					break;
				case 7:
					this.SetName((DbfLocValue)val);
					break;
				case 8:
					this.SetPrefabName((string)val);
					break;
				case 9:
					this.SetDescription((DbfLocValue)val);
					break;
				case 10:
					this.SetSourceDescription((DbfLocValue)val);
					break;
				}
			}
		}
	}

	// Token: 0x06001069 RID: 4201 RVA: 0x00046F1C File Offset: 0x0004511C
	public override Type GetVarType(string name)
	{
		if (name != null)
		{
			if (CardBackDbfRecord.<>f__switch$map1B == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(11);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("DATA1", 2);
				dictionary.Add("SOURCE", 3);
				dictionary.Add("ENABLED", 4);
				dictionary.Add("SORT_CATEGORY", 5);
				dictionary.Add("SORT_ORDER", 6);
				dictionary.Add("NAME", 7);
				dictionary.Add("PREFAB_NAME", 8);
				dictionary.Add("DESCRIPTION", 9);
				dictionary.Add("SOURCE_DESCRIPTION", 10);
				CardBackDbfRecord.<>f__switch$map1B = dictionary;
			}
			int num;
			if (CardBackDbfRecord.<>f__switch$map1B.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return typeof(int);
				case 1:
					return typeof(string);
				case 2:
					return typeof(long);
				case 3:
					return typeof(string);
				case 4:
					return typeof(bool);
				case 5:
					return typeof(int);
				case 6:
					return typeof(int);
				case 7:
					return typeof(DbfLocValue);
				case 8:
					return typeof(string);
				case 9:
					return typeof(DbfLocValue);
				case 10:
					return typeof(DbfLocValue);
				}
			}
		}
		return null;
	}

	// Token: 0x040008AC RID: 2220
	private string m_NoteDesc;

	// Token: 0x040008AD RID: 2221
	private long m_Data1;

	// Token: 0x040008AE RID: 2222
	private string m_Source;

	// Token: 0x040008AF RID: 2223
	private bool m_Enabled;

	// Token: 0x040008B0 RID: 2224
	private int m_SortCategory;

	// Token: 0x040008B1 RID: 2225
	private int m_SortOrder;

	// Token: 0x040008B2 RID: 2226
	private DbfLocValue m_Name;

	// Token: 0x040008B3 RID: 2227
	private string m_PrefabName;

	// Token: 0x040008B4 RID: 2228
	private DbfLocValue m_Description;

	// Token: 0x040008B5 RID: 2229
	private DbfLocValue m_SourceDescription;
}
