using System;
using System.Collections.Generic;

// Token: 0x02000135 RID: 309
public class AdventureDataDbfRecord : DbfRecord
{
	// Token: 0x1700026E RID: 622
	// (get) Token: 0x06000FEB RID: 4075 RVA: 0x000452FE File Offset: 0x000434FE
	[DbfField("NOTE_DESC", "designer note")]
	public string NoteDesc
	{
		get
		{
			return this.m_NoteDesc;
		}
	}

	// Token: 0x1700026F RID: 623
	// (get) Token: 0x06000FEC RID: 4076 RVA: 0x00045306 File Offset: 0x00043506
	[DbfField("ADVENTURE_ID", "ASSET.ADVENTURE.ID")]
	public int AdventureId
	{
		get
		{
			return this.m_AdventureId;
		}
	}

	// Token: 0x17000270 RID: 624
	// (get) Token: 0x06000FED RID: 4077 RVA: 0x0004530E File Offset: 0x0004350E
	[DbfField("MODE_ID", "ASSET.ADVENTURE_MODE.ID")]
	public int ModeId
	{
		get
		{
			return this.m_ModeId;
		}
	}

	// Token: 0x17000271 RID: 625
	// (get) Token: 0x06000FEE RID: 4078 RVA: 0x00045316 File Offset: 0x00043516
	[DbfField("SORT_ORDER", "sort order of this adventure data in its adventure")]
	public int SortOrder
	{
		get
		{
			return this.m_SortOrder;
		}
	}

	// Token: 0x17000272 RID: 626
	// (get) Token: 0x06000FEF RID: 4079 RVA: 0x0004531E File Offset: 0x0004351E
	[DbfField("NAME", "")]
	public DbfLocValue Name
	{
		get
		{
			return this.m_Name;
		}
	}

	// Token: 0x17000273 RID: 627
	// (get) Token: 0x06000FF0 RID: 4080 RVA: 0x00045326 File Offset: 0x00043526
	[DbfField("SHORT_NAME", "")]
	public DbfLocValue ShortName
	{
		get
		{
			return this.m_ShortName;
		}
	}

	// Token: 0x17000274 RID: 628
	// (get) Token: 0x06000FF1 RID: 4081 RVA: 0x0004532E File Offset: 0x0004352E
	[DbfField("DESCRIPTION", "")]
	public DbfLocValue Description
	{
		get
		{
			return this.m_Description;
		}
	}

	// Token: 0x17000275 RID: 629
	// (get) Token: 0x06000FF2 RID: 4082 RVA: 0x00045336 File Offset: 0x00043536
	[DbfField("SHORT_DESCRIPTION", "")]
	public DbfLocValue ShortDescription
	{
		get
		{
			return this.m_ShortDescription;
		}
	}

	// Token: 0x17000276 RID: 630
	// (get) Token: 0x06000FF3 RID: 4083 RVA: 0x0004533E File Offset: 0x0004353E
	[DbfField("REQUIREMENTS_DESCRIPTION", "")]
	public DbfLocValue RequirementsDescription
	{
		get
		{
			return this.m_RequirementsDescription;
		}
	}

	// Token: 0x17000277 RID: 631
	// (get) Token: 0x06000FF4 RID: 4084 RVA: 0x00045346 File Offset: 0x00043546
	[DbfField("COMPLETE_BANNER_TEXT", "")]
	public DbfLocValue CompleteBannerText
	{
		get
		{
			return this.m_CompleteBannerText;
		}
	}

	// Token: 0x17000278 RID: 632
	// (get) Token: 0x06000FF5 RID: 4085 RVA: 0x0004534E File Offset: 0x0004354E
	[DbfField("SUBSCENE_PREFAB", "")]
	public string SubscenePrefab
	{
		get
		{
			return this.m_SubscenePrefab;
		}
	}

	// Token: 0x17000279 RID: 633
	// (get) Token: 0x06000FF6 RID: 4086 RVA: 0x00045356 File Offset: 0x00043556
	[DbfField("ADVENTURE_SUB_DEF_PREFAB", "")]
	public string AdventureSubDefPrefab
	{
		get
		{
			return this.m_AdventureSubDefPrefab;
		}
	}

	// Token: 0x06000FF7 RID: 4087 RVA: 0x0004535E File Offset: 0x0004355E
	public void SetNoteDesc(string v)
	{
		this.m_NoteDesc = v;
	}

	// Token: 0x06000FF8 RID: 4088 RVA: 0x00045367 File Offset: 0x00043567
	public void SetAdventureId(int v)
	{
		this.m_AdventureId = v;
	}

	// Token: 0x06000FF9 RID: 4089 RVA: 0x00045370 File Offset: 0x00043570
	public void SetModeId(int v)
	{
		this.m_ModeId = v;
	}

	// Token: 0x06000FFA RID: 4090 RVA: 0x00045379 File Offset: 0x00043579
	public void SetSortOrder(int v)
	{
		this.m_SortOrder = v;
	}

	// Token: 0x06000FFB RID: 4091 RVA: 0x00045382 File Offset: 0x00043582
	public void SetName(DbfLocValue v)
	{
		this.m_Name = v;
		v.SetDebugInfo(base.ID, "NAME");
	}

	// Token: 0x06000FFC RID: 4092 RVA: 0x0004539C File Offset: 0x0004359C
	public void SetShortName(DbfLocValue v)
	{
		this.m_ShortName = v;
		v.SetDebugInfo(base.ID, "SHORT_NAME");
	}

	// Token: 0x06000FFD RID: 4093 RVA: 0x000453B6 File Offset: 0x000435B6
	public void SetDescription(DbfLocValue v)
	{
		this.m_Description = v;
		v.SetDebugInfo(base.ID, "DESCRIPTION");
	}

	// Token: 0x06000FFE RID: 4094 RVA: 0x000453D0 File Offset: 0x000435D0
	public void SetShortDescription(DbfLocValue v)
	{
		this.m_ShortDescription = v;
		v.SetDebugInfo(base.ID, "SHORT_DESCRIPTION");
	}

	// Token: 0x06000FFF RID: 4095 RVA: 0x000453EA File Offset: 0x000435EA
	public void SetRequirementsDescription(DbfLocValue v)
	{
		this.m_RequirementsDescription = v;
		v.SetDebugInfo(base.ID, "REQUIREMENTS_DESCRIPTION");
	}

	// Token: 0x06001000 RID: 4096 RVA: 0x00045404 File Offset: 0x00043604
	public void SetCompleteBannerText(DbfLocValue v)
	{
		this.m_CompleteBannerText = v;
		v.SetDebugInfo(base.ID, "COMPLETE_BANNER_TEXT");
	}

	// Token: 0x06001001 RID: 4097 RVA: 0x0004541E File Offset: 0x0004361E
	public void SetSubscenePrefab(string v)
	{
		this.m_SubscenePrefab = v;
	}

	// Token: 0x06001002 RID: 4098 RVA: 0x00045427 File Offset: 0x00043627
	public void SetAdventureSubDefPrefab(string v)
	{
		this.m_AdventureSubDefPrefab = v;
	}

	// Token: 0x06001003 RID: 4099 RVA: 0x00045430 File Offset: 0x00043630
	public override object GetVar(string name)
	{
		if (name != null)
		{
			if (AdventureDataDbfRecord.<>f__switch$map4 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(13);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("ADVENTURE_ID", 2);
				dictionary.Add("MODE_ID", 3);
				dictionary.Add("SORT_ORDER", 4);
				dictionary.Add("NAME", 5);
				dictionary.Add("SHORT_NAME", 6);
				dictionary.Add("DESCRIPTION", 7);
				dictionary.Add("SHORT_DESCRIPTION", 8);
				dictionary.Add("REQUIREMENTS_DESCRIPTION", 9);
				dictionary.Add("COMPLETE_BANNER_TEXT", 10);
				dictionary.Add("SUBSCENE_PREFAB", 11);
				dictionary.Add("ADVENTURE_SUB_DEF_PREFAB", 12);
				AdventureDataDbfRecord.<>f__switch$map4 = dictionary;
			}
			int num;
			if (AdventureDataDbfRecord.<>f__switch$map4.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return base.ID;
				case 1:
					return this.NoteDesc;
				case 2:
					return this.AdventureId;
				case 3:
					return this.ModeId;
				case 4:
					return this.SortOrder;
				case 5:
					return this.Name;
				case 6:
					return this.ShortName;
				case 7:
					return this.Description;
				case 8:
					return this.ShortDescription;
				case 9:
					return this.RequirementsDescription;
				case 10:
					return this.CompleteBannerText;
				case 11:
					return this.SubscenePrefab;
				case 12:
					return this.AdventureSubDefPrefab;
				}
			}
		}
		return null;
	}

	// Token: 0x06001004 RID: 4100 RVA: 0x000455C0 File Offset: 0x000437C0
	public override void SetVar(string name, object val)
	{
		if (name != null)
		{
			if (AdventureDataDbfRecord.<>f__switch$map5 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(13);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("ADVENTURE_ID", 2);
				dictionary.Add("MODE_ID", 3);
				dictionary.Add("SORT_ORDER", 4);
				dictionary.Add("NAME", 5);
				dictionary.Add("SHORT_NAME", 6);
				dictionary.Add("DESCRIPTION", 7);
				dictionary.Add("SHORT_DESCRIPTION", 8);
				dictionary.Add("REQUIREMENTS_DESCRIPTION", 9);
				dictionary.Add("COMPLETE_BANNER_TEXT", 10);
				dictionary.Add("SUBSCENE_PREFAB", 11);
				dictionary.Add("ADVENTURE_SUB_DEF_PREFAB", 12);
				AdventureDataDbfRecord.<>f__switch$map5 = dictionary;
			}
			int num;
			if (AdventureDataDbfRecord.<>f__switch$map5.TryGetValue(name, ref num))
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
					this.SetAdventureId((int)val);
					break;
				case 3:
					this.SetModeId((int)val);
					break;
				case 4:
					this.SetSortOrder((int)val);
					break;
				case 5:
					this.SetName((DbfLocValue)val);
					break;
				case 6:
					this.SetShortName((DbfLocValue)val);
					break;
				case 7:
					this.SetDescription((DbfLocValue)val);
					break;
				case 8:
					this.SetShortDescription((DbfLocValue)val);
					break;
				case 9:
					this.SetRequirementsDescription((DbfLocValue)val);
					break;
				case 10:
					this.SetCompleteBannerText((DbfLocValue)val);
					break;
				case 11:
					this.SetSubscenePrefab((string)val);
					break;
				case 12:
					this.SetAdventureSubDefPrefab((string)val);
					break;
				}
			}
		}
	}

	// Token: 0x06001005 RID: 4101 RVA: 0x000457BC File Offset: 0x000439BC
	public override Type GetVarType(string name)
	{
		if (name != null)
		{
			if (AdventureDataDbfRecord.<>f__switch$map6 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(13);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("ADVENTURE_ID", 2);
				dictionary.Add("MODE_ID", 3);
				dictionary.Add("SORT_ORDER", 4);
				dictionary.Add("NAME", 5);
				dictionary.Add("SHORT_NAME", 6);
				dictionary.Add("DESCRIPTION", 7);
				dictionary.Add("SHORT_DESCRIPTION", 8);
				dictionary.Add("REQUIREMENTS_DESCRIPTION", 9);
				dictionary.Add("COMPLETE_BANNER_TEXT", 10);
				dictionary.Add("SUBSCENE_PREFAB", 11);
				dictionary.Add("ADVENTURE_SUB_DEF_PREFAB", 12);
				AdventureDataDbfRecord.<>f__switch$map6 = dictionary;
			}
			int num;
			if (AdventureDataDbfRecord.<>f__switch$map6.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return typeof(int);
				case 1:
					return typeof(string);
				case 2:
					return typeof(int);
				case 3:
					return typeof(int);
				case 4:
					return typeof(int);
				case 5:
					return typeof(DbfLocValue);
				case 6:
					return typeof(DbfLocValue);
				case 7:
					return typeof(DbfLocValue);
				case 8:
					return typeof(DbfLocValue);
				case 9:
					return typeof(DbfLocValue);
				case 10:
					return typeof(DbfLocValue);
				case 11:
					return typeof(string);
				case 12:
					return typeof(string);
				}
			}
		}
		return null;
	}

	// Token: 0x04000872 RID: 2162
	private string m_NoteDesc;

	// Token: 0x04000873 RID: 2163
	private int m_AdventureId;

	// Token: 0x04000874 RID: 2164
	private int m_ModeId;

	// Token: 0x04000875 RID: 2165
	private int m_SortOrder;

	// Token: 0x04000876 RID: 2166
	private DbfLocValue m_Name;

	// Token: 0x04000877 RID: 2167
	private DbfLocValue m_ShortName;

	// Token: 0x04000878 RID: 2168
	private DbfLocValue m_Description;

	// Token: 0x04000879 RID: 2169
	private DbfLocValue m_ShortDescription;

	// Token: 0x0400087A RID: 2170
	private DbfLocValue m_RequirementsDescription;

	// Token: 0x0400087B RID: 2171
	private DbfLocValue m_CompleteBannerText;

	// Token: 0x0400087C RID: 2172
	private string m_SubscenePrefab;

	// Token: 0x0400087D RID: 2173
	private string m_AdventureSubDefPrefab;
}
