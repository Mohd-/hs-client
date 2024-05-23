using System;
using System.Collections.Generic;

// Token: 0x02000014 RID: 20
public class WingDbfRecord : DbfRecord
{
	// Token: 0x17000017 RID: 23
	// (get) Token: 0x06000236 RID: 566 RVA: 0x0000B20C File Offset: 0x0000940C
	[DbfField("NOTE_DESC", "designer note")]
	public string NoteDesc
	{
		get
		{
			return this.m_NoteDesc;
		}
	}

	// Token: 0x17000018 RID: 24
	// (get) Token: 0x06000237 RID: 567 RVA: 0x0000B214 File Offset: 0x00009414
	[DbfField("ADVENTURE_ID", "ASSET.ADVENTURE.ID")]
	public int AdventureId
	{
		get
		{
			return this.m_AdventureId;
		}
	}

	// Token: 0x17000019 RID: 25
	// (get) Token: 0x06000238 RID: 568 RVA: 0x0000B21C File Offset: 0x0000941C
	[DbfField("SORT_ORDER", "sort order of this wing in its adventure")]
	public int SortOrder
	{
		get
		{
			return this.m_SortOrder;
		}
	}

	// Token: 0x1700001A RID: 26
	// (get) Token: 0x06000239 RID: 569 RVA: 0x0000B224 File Offset: 0x00009424
	[DbfField("RELEASE", "first RELEASE.ID in which this asset appeared")]
	public string Release
	{
		get
		{
			return this.m_Release;
		}
	}

	// Token: 0x1700001B RID: 27
	// (get) Token: 0x0600023A RID: 570 RVA: 0x0000B22C File Offset: 0x0000942C
	[DbfField("REQUIRED_EVENT", "EVENT_TMING.EVENT that must have started before this wing can be played")]
	public string RequiredEvent
	{
		get
		{
			return this.m_RequiredEvent;
		}
	}

	// Token: 0x1700001C RID: 28
	// (get) Token: 0x0600023B RID: 571 RVA: 0x0000B234 File Offset: 0x00009434
	[DbfField("OWNERSHIP_PREREQ_WING_ID", "WING.ID of ANOTHER wing that needs to be owned before THIS wing can be bought.")]
	public int OwnershipPrereqWingId
	{
		get
		{
			return this.m_OwnershipPrereqWingId;
		}
	}

	// Token: 0x1700001D RID: 29
	// (get) Token: 0x0600023C RID: 572 RVA: 0x0000B23C File Offset: 0x0000943C
	[DbfField("NAME", "")]
	public DbfLocValue Name
	{
		get
		{
			return this.m_Name;
		}
	}

	// Token: 0x1700001E RID: 30
	// (get) Token: 0x0600023D RID: 573 RVA: 0x0000B244 File Offset: 0x00009444
	[DbfField("CLASS_CHALLENGE_REWARD_SOURCE", "")]
	public DbfLocValue ClassChallengeRewardSource
	{
		get
		{
			return this.m_ClassChallengeRewardSource;
		}
	}

	// Token: 0x1700001F RID: 31
	// (get) Token: 0x0600023E RID: 574 RVA: 0x0000B24C File Offset: 0x0000944C
	[DbfField("ADVENTURE_WING_DEF_PREFAB", "")]
	public string AdventureWingDefPrefab
	{
		get
		{
			return this.m_AdventureWingDefPrefab;
		}
	}

	// Token: 0x17000020 RID: 32
	// (get) Token: 0x0600023F RID: 575 RVA: 0x0000B254 File Offset: 0x00009454
	[DbfField("COMING_SOON_LABEL", "")]
	public DbfLocValue ComingSoonLabel
	{
		get
		{
			return this.m_ComingSoonLabel;
		}
	}

	// Token: 0x17000021 RID: 33
	// (get) Token: 0x06000240 RID: 576 RVA: 0x0000B25C File Offset: 0x0000945C
	[DbfField("REQUIRES_LABEL", "")]
	public DbfLocValue RequiresLabel
	{
		get
		{
			return this.m_RequiresLabel;
		}
	}

	// Token: 0x06000241 RID: 577 RVA: 0x0000B264 File Offset: 0x00009464
	public void SetNoteDesc(string v)
	{
		this.m_NoteDesc = v;
	}

	// Token: 0x06000242 RID: 578 RVA: 0x0000B26D File Offset: 0x0000946D
	public void SetAdventureId(int v)
	{
		this.m_AdventureId = v;
	}

	// Token: 0x06000243 RID: 579 RVA: 0x0000B276 File Offset: 0x00009476
	public void SetSortOrder(int v)
	{
		this.m_SortOrder = v;
	}

	// Token: 0x06000244 RID: 580 RVA: 0x0000B27F File Offset: 0x0000947F
	public void SetRelease(string v)
	{
		this.m_Release = v;
	}

	// Token: 0x06000245 RID: 581 RVA: 0x0000B288 File Offset: 0x00009488
	public void SetRequiredEvent(string v)
	{
		this.m_RequiredEvent = v;
	}

	// Token: 0x06000246 RID: 582 RVA: 0x0000B291 File Offset: 0x00009491
	public void SetOwnershipPrereqWingId(int v)
	{
		this.m_OwnershipPrereqWingId = v;
	}

	// Token: 0x06000247 RID: 583 RVA: 0x0000B29A File Offset: 0x0000949A
	public void SetName(DbfLocValue v)
	{
		this.m_Name = v;
		v.SetDebugInfo(base.ID, "NAME");
	}

	// Token: 0x06000248 RID: 584 RVA: 0x0000B2B4 File Offset: 0x000094B4
	public void SetClassChallengeRewardSource(DbfLocValue v)
	{
		this.m_ClassChallengeRewardSource = v;
		v.SetDebugInfo(base.ID, "CLASS_CHALLENGE_REWARD_SOURCE");
	}

	// Token: 0x06000249 RID: 585 RVA: 0x0000B2CE File Offset: 0x000094CE
	public void SetAdventureWingDefPrefab(string v)
	{
		this.m_AdventureWingDefPrefab = v;
	}

	// Token: 0x0600024A RID: 586 RVA: 0x0000B2D7 File Offset: 0x000094D7
	public void SetComingSoonLabel(DbfLocValue v)
	{
		this.m_ComingSoonLabel = v;
		v.SetDebugInfo(base.ID, "COMING_SOON_LABEL");
	}

	// Token: 0x0600024B RID: 587 RVA: 0x0000B2F1 File Offset: 0x000094F1
	public void SetRequiresLabel(DbfLocValue v)
	{
		this.m_RequiresLabel = v;
		v.SetDebugInfo(base.ID, "REQUIRES_LABEL");
	}

	// Token: 0x0600024C RID: 588 RVA: 0x0000B30C File Offset: 0x0000950C
	public override object GetVar(string name)
	{
		if (name != null)
		{
			if (WingDbfRecord.<>f__switch$map4C == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(12);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("ADVENTURE_ID", 2);
				dictionary.Add("SORT_ORDER", 3);
				dictionary.Add("RELEASE", 4);
				dictionary.Add("REQUIRED_EVENT", 5);
				dictionary.Add("OWNERSHIP_PREREQ_WING_ID", 6);
				dictionary.Add("NAME", 7);
				dictionary.Add("CLASS_CHALLENGE_REWARD_SOURCE", 8);
				dictionary.Add("ADVENTURE_WING_DEF_PREFAB", 9);
				dictionary.Add("COMING_SOON_LABEL", 10);
				dictionary.Add("REQUIRES_LABEL", 11);
				WingDbfRecord.<>f__switch$map4C = dictionary;
			}
			int num;
			if (WingDbfRecord.<>f__switch$map4C.TryGetValue(name, ref num))
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
					return this.SortOrder;
				case 4:
					return this.Release;
				case 5:
					return this.RequiredEvent;
				case 6:
					return this.OwnershipPrereqWingId;
				case 7:
					return this.Name;
				case 8:
					return this.ClassChallengeRewardSource;
				case 9:
					return this.AdventureWingDefPrefab;
				case 10:
					return this.ComingSoonLabel;
				case 11:
					return this.RequiresLabel;
				}
			}
		}
		return null;
	}

	// Token: 0x0600024D RID: 589 RVA: 0x0000B484 File Offset: 0x00009684
	public override void SetVar(string name, object val)
	{
		if (name != null)
		{
			if (WingDbfRecord.<>f__switch$map4D == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(12);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("ADVENTURE_ID", 2);
				dictionary.Add("SORT_ORDER", 3);
				dictionary.Add("RELEASE", 4);
				dictionary.Add("REQUIRED_EVENT", 5);
				dictionary.Add("OWNERSHIP_PREREQ_WING_ID", 6);
				dictionary.Add("NAME", 7);
				dictionary.Add("CLASS_CHALLENGE_REWARD_SOURCE", 8);
				dictionary.Add("ADVENTURE_WING_DEF_PREFAB", 9);
				dictionary.Add("COMING_SOON_LABEL", 10);
				dictionary.Add("REQUIRES_LABEL", 11);
				WingDbfRecord.<>f__switch$map4D = dictionary;
			}
			int num;
			if (WingDbfRecord.<>f__switch$map4D.TryGetValue(name, ref num))
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
					this.SetSortOrder((int)val);
					break;
				case 4:
					this.SetRelease((string)val);
					break;
				case 5:
					this.SetRequiredEvent((string)val);
					break;
				case 6:
					this.SetOwnershipPrereqWingId((int)val);
					break;
				case 7:
					this.SetName((DbfLocValue)val);
					break;
				case 8:
					this.SetClassChallengeRewardSource((DbfLocValue)val);
					break;
				case 9:
					this.SetAdventureWingDefPrefab((string)val);
					break;
				case 10:
					this.SetComingSoonLabel((DbfLocValue)val);
					break;
				case 11:
					this.SetRequiresLabel((DbfLocValue)val);
					break;
				}
			}
		}
	}

	// Token: 0x0600024E RID: 590 RVA: 0x0000B660 File Offset: 0x00009860
	public override Type GetVarType(string name)
	{
		if (name != null)
		{
			if (WingDbfRecord.<>f__switch$map4E == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(12);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("ADVENTURE_ID", 2);
				dictionary.Add("SORT_ORDER", 3);
				dictionary.Add("RELEASE", 4);
				dictionary.Add("REQUIRED_EVENT", 5);
				dictionary.Add("OWNERSHIP_PREREQ_WING_ID", 6);
				dictionary.Add("NAME", 7);
				dictionary.Add("CLASS_CHALLENGE_REWARD_SOURCE", 8);
				dictionary.Add("ADVENTURE_WING_DEF_PREFAB", 9);
				dictionary.Add("COMING_SOON_LABEL", 10);
				dictionary.Add("REQUIRES_LABEL", 11);
				WingDbfRecord.<>f__switch$map4E = dictionary;
			}
			int num;
			if (WingDbfRecord.<>f__switch$map4E.TryGetValue(name, ref num))
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
					return typeof(string);
				case 5:
					return typeof(string);
				case 6:
					return typeof(int);
				case 7:
					return typeof(DbfLocValue);
				case 8:
					return typeof(DbfLocValue);
				case 9:
					return typeof(string);
				case 10:
					return typeof(DbfLocValue);
				case 11:
					return typeof(DbfLocValue);
				}
			}
		}
		return null;
	}

	// Token: 0x0400008D RID: 141
	private string m_NoteDesc;

	// Token: 0x0400008E RID: 142
	private int m_AdventureId;

	// Token: 0x0400008F RID: 143
	private int m_SortOrder;

	// Token: 0x04000090 RID: 144
	private string m_Release;

	// Token: 0x04000091 RID: 145
	private string m_RequiredEvent;

	// Token: 0x04000092 RID: 146
	private int m_OwnershipPrereqWingId;

	// Token: 0x04000093 RID: 147
	private DbfLocValue m_Name;

	// Token: 0x04000094 RID: 148
	private DbfLocValue m_ClassChallengeRewardSource;

	// Token: 0x04000095 RID: 149
	private string m_AdventureWingDefPrefab;

	// Token: 0x04000096 RID: 150
	private DbfLocValue m_ComingSoonLabel;

	// Token: 0x04000097 RID: 151
	private DbfLocValue m_RequiresLabel;
}
