using System;
using System.Collections.Generic;

// Token: 0x02000139 RID: 313
public class BoosterDbfRecord : DbfRecord
{
	// Token: 0x17000280 RID: 640
	// (get) Token: 0x0600101F RID: 4127 RVA: 0x00045F97 File Offset: 0x00044197
	[DbfField("NOTE_DESC", "designer name of booster contents")]
	public string NoteDesc
	{
		get
		{
			return this.m_NoteDesc;
		}
	}

	// Token: 0x17000281 RID: 641
	// (get) Token: 0x06001020 RID: 4128 RVA: 0x00045F9F File Offset: 0x0004419F
	[DbfField("SORT_ORDER", "client uses this column to determine order of this booster in a displayable list; also used by server to determine the 'current pack', which is defined as the pack with the greatest SORT_ORDER value (see sRandomizeRewardBoosterPack).")]
	public int SortOrder
	{
		get
		{
			return this.m_SortOrder;
		}
	}

	// Token: 0x17000282 RID: 642
	// (get) Token: 0x06001021 RID: 4129 RVA: 0x00045FA7 File Offset: 0x000441A7
	[DbfField("OPEN_PACK_EVENT", "EVENT_TIMING.EVENT that indicates when players are allowed to open packs of this type")]
	public string OpenPackEvent
	{
		get
		{
			return this.m_OpenPackEvent;
		}
	}

	// Token: 0x17000283 RID: 643
	// (get) Token: 0x06001022 RID: 4130 RVA: 0x00045FAF File Offset: 0x000441AF
	[DbfField("BUY_WITH_GOLD_EVENT", "EVENT_TIMING.EVENT that indicates when players are allowed to buy this pack type with gold (if GOLD_COST > 0)")]
	public string BuyWithGoldEvent
	{
		get
		{
			return this.m_BuyWithGoldEvent;
		}
	}

	// Token: 0x17000284 RID: 644
	// (get) Token: 0x06001023 RID: 4131 RVA: 0x00045FB7 File Offset: 0x000441B7
	[DbfField("NAME", "")]
	public DbfLocValue Name
	{
		get
		{
			return this.m_Name;
		}
	}

	// Token: 0x17000285 RID: 645
	// (get) Token: 0x06001024 RID: 4132 RVA: 0x00045FBF File Offset: 0x000441BF
	[DbfField("PACK_OPENING_PREFAB", "")]
	public string PackOpeningPrefab
	{
		get
		{
			return this.m_PackOpeningPrefab;
		}
	}

	// Token: 0x17000286 RID: 646
	// (get) Token: 0x06001025 RID: 4133 RVA: 0x00045FC7 File Offset: 0x000441C7
	[DbfField("PACK_OPENING_FX_PREFAB", "")]
	public string PackOpeningFxPrefab
	{
		get
		{
			return this.m_PackOpeningFxPrefab;
		}
	}

	// Token: 0x17000287 RID: 647
	// (get) Token: 0x06001026 RID: 4134 RVA: 0x00045FCF File Offset: 0x000441CF
	[DbfField("STORE_PREFAB", "")]
	public string StorePrefab
	{
		get
		{
			return this.m_StorePrefab;
		}
	}

	// Token: 0x17000288 RID: 648
	// (get) Token: 0x06001027 RID: 4135 RVA: 0x00045FD7 File Offset: 0x000441D7
	[DbfField("ARENA_PREFAB", "")]
	public string ArenaPrefab
	{
		get
		{
			return this.m_ArenaPrefab;
		}
	}

	// Token: 0x17000289 RID: 649
	// (get) Token: 0x06001028 RID: 4136 RVA: 0x00045FDF File Offset: 0x000441DF
	[DbfField("LEAVING_SOON", "this booster will be removed from the in-game store soon")]
	public bool LeavingSoon
	{
		get
		{
			return this.m_LeavingSoon;
		}
	}

	// Token: 0x1700028A RID: 650
	// (get) Token: 0x06001029 RID: 4137 RVA: 0x00045FE7 File Offset: 0x000441E7
	[DbfField("LEAVING_SOON_TEXT", "localized text id explaining why this booster will be removed from the in-game store soon")]
	public DbfLocValue LeavingSoonText
	{
		get
		{
			return this.m_LeavingSoonText;
		}
	}

	// Token: 0x0600102A RID: 4138 RVA: 0x00045FEF File Offset: 0x000441EF
	public void SetNoteDesc(string v)
	{
		this.m_NoteDesc = v;
	}

	// Token: 0x0600102B RID: 4139 RVA: 0x00045FF8 File Offset: 0x000441F8
	public void SetSortOrder(int v)
	{
		this.m_SortOrder = v;
	}

	// Token: 0x0600102C RID: 4140 RVA: 0x00046001 File Offset: 0x00044201
	public void SetOpenPackEvent(string v)
	{
		this.m_OpenPackEvent = v;
	}

	// Token: 0x0600102D RID: 4141 RVA: 0x0004600A File Offset: 0x0004420A
	public void SetBuyWithGoldEvent(string v)
	{
		this.m_BuyWithGoldEvent = v;
	}

	// Token: 0x0600102E RID: 4142 RVA: 0x00046013 File Offset: 0x00044213
	public void SetName(DbfLocValue v)
	{
		this.m_Name = v;
		v.SetDebugInfo(base.ID, "NAME");
	}

	// Token: 0x0600102F RID: 4143 RVA: 0x0004602D File Offset: 0x0004422D
	public void SetPackOpeningPrefab(string v)
	{
		this.m_PackOpeningPrefab = v;
	}

	// Token: 0x06001030 RID: 4144 RVA: 0x00046036 File Offset: 0x00044236
	public void SetPackOpeningFxPrefab(string v)
	{
		this.m_PackOpeningFxPrefab = v;
	}

	// Token: 0x06001031 RID: 4145 RVA: 0x0004603F File Offset: 0x0004423F
	public void SetStorePrefab(string v)
	{
		this.m_StorePrefab = v;
	}

	// Token: 0x06001032 RID: 4146 RVA: 0x00046048 File Offset: 0x00044248
	public void SetArenaPrefab(string v)
	{
		this.m_ArenaPrefab = v;
	}

	// Token: 0x06001033 RID: 4147 RVA: 0x00046051 File Offset: 0x00044251
	public void SetLeavingSoon(bool v)
	{
		this.m_LeavingSoon = v;
	}

	// Token: 0x06001034 RID: 4148 RVA: 0x0004605A File Offset: 0x0004425A
	public void SetLeavingSoonText(DbfLocValue v)
	{
		this.m_LeavingSoonText = v;
		v.SetDebugInfo(base.ID, "LEAVING_SOON_TEXT");
	}

	// Token: 0x06001035 RID: 4149 RVA: 0x00046074 File Offset: 0x00044274
	public override object GetVar(string name)
	{
		if (name != null)
		{
			if (BoosterDbfRecord.<>f__switch$map16 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(12);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("SORT_ORDER", 2);
				dictionary.Add("OPEN_PACK_EVENT", 3);
				dictionary.Add("BUY_WITH_GOLD_EVENT", 4);
				dictionary.Add("NAME", 5);
				dictionary.Add("PACK_OPENING_PREFAB", 6);
				dictionary.Add("PACK_OPENING_FX_PREFAB", 7);
				dictionary.Add("STORE_PREFAB", 8);
				dictionary.Add("ARENA_PREFAB", 9);
				dictionary.Add("LEAVING_SOON", 10);
				dictionary.Add("LEAVING_SOON_TEXT", 11);
				BoosterDbfRecord.<>f__switch$map16 = dictionary;
			}
			int num;
			if (BoosterDbfRecord.<>f__switch$map16.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return base.ID;
				case 1:
					return this.NoteDesc;
				case 2:
					return this.SortOrder;
				case 3:
					return this.OpenPackEvent;
				case 4:
					return this.BuyWithGoldEvent;
				case 5:
					return this.Name;
				case 6:
					return this.PackOpeningPrefab;
				case 7:
					return this.PackOpeningFxPrefab;
				case 8:
					return this.StorePrefab;
				case 9:
					return this.ArenaPrefab;
				case 10:
					return this.LeavingSoon;
				case 11:
					return this.LeavingSoonText;
				}
			}
		}
		return null;
	}

	// Token: 0x06001036 RID: 4150 RVA: 0x000461E8 File Offset: 0x000443E8
	public override void SetVar(string name, object val)
	{
		if (name != null)
		{
			if (BoosterDbfRecord.<>f__switch$map17 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(12);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("SORT_ORDER", 2);
				dictionary.Add("OPEN_PACK_EVENT", 3);
				dictionary.Add("BUY_WITH_GOLD_EVENT", 4);
				dictionary.Add("NAME", 5);
				dictionary.Add("PACK_OPENING_PREFAB", 6);
				dictionary.Add("PACK_OPENING_FX_PREFAB", 7);
				dictionary.Add("STORE_PREFAB", 8);
				dictionary.Add("ARENA_PREFAB", 9);
				dictionary.Add("LEAVING_SOON", 10);
				dictionary.Add("LEAVING_SOON_TEXT", 11);
				BoosterDbfRecord.<>f__switch$map17 = dictionary;
			}
			int num;
			if (BoosterDbfRecord.<>f__switch$map17.TryGetValue(name, ref num))
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
					this.SetSortOrder((int)val);
					break;
				case 3:
					this.SetOpenPackEvent((string)val);
					break;
				case 4:
					this.SetBuyWithGoldEvent((string)val);
					break;
				case 5:
					this.SetName((DbfLocValue)val);
					break;
				case 6:
					this.SetPackOpeningPrefab((string)val);
					break;
				case 7:
					this.SetPackOpeningFxPrefab((string)val);
					break;
				case 8:
					this.SetStorePrefab((string)val);
					break;
				case 9:
					this.SetArenaPrefab((string)val);
					break;
				case 10:
					this.SetLeavingSoon((bool)val);
					break;
				case 11:
					this.SetLeavingSoonText((DbfLocValue)val);
					break;
				}
			}
		}
	}

	// Token: 0x06001037 RID: 4151 RVA: 0x000463C4 File Offset: 0x000445C4
	public override Type GetVarType(string name)
	{
		if (name != null)
		{
			if (BoosterDbfRecord.<>f__switch$map18 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(12);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("SORT_ORDER", 2);
				dictionary.Add("OPEN_PACK_EVENT", 3);
				dictionary.Add("BUY_WITH_GOLD_EVENT", 4);
				dictionary.Add("NAME", 5);
				dictionary.Add("PACK_OPENING_PREFAB", 6);
				dictionary.Add("PACK_OPENING_FX_PREFAB", 7);
				dictionary.Add("STORE_PREFAB", 8);
				dictionary.Add("ARENA_PREFAB", 9);
				dictionary.Add("LEAVING_SOON", 10);
				dictionary.Add("LEAVING_SOON_TEXT", 11);
				BoosterDbfRecord.<>f__switch$map18 = dictionary;
			}
			int num;
			if (BoosterDbfRecord.<>f__switch$map18.TryGetValue(name, ref num))
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
					return typeof(string);
				case 4:
					return typeof(string);
				case 5:
					return typeof(DbfLocValue);
				case 6:
					return typeof(string);
				case 7:
					return typeof(string);
				case 8:
					return typeof(string);
				case 9:
					return typeof(string);
				case 10:
					return typeof(bool);
				case 11:
					return typeof(DbfLocValue);
				}
			}
		}
		return null;
	}

	// Token: 0x04000890 RID: 2192
	private string m_NoteDesc;

	// Token: 0x04000891 RID: 2193
	private int m_SortOrder;

	// Token: 0x04000892 RID: 2194
	private string m_OpenPackEvent;

	// Token: 0x04000893 RID: 2195
	private string m_BuyWithGoldEvent;

	// Token: 0x04000894 RID: 2196
	private DbfLocValue m_Name;

	// Token: 0x04000895 RID: 2197
	private string m_PackOpeningPrefab;

	// Token: 0x04000896 RID: 2198
	private string m_PackOpeningFxPrefab;

	// Token: 0x04000897 RID: 2199
	private string m_StorePrefab;

	// Token: 0x04000898 RID: 2200
	private string m_ArenaPrefab;

	// Token: 0x04000899 RID: 2201
	private bool m_LeavingSoon;

	// Token: 0x0400089A RID: 2202
	private DbfLocValue m_LeavingSoonText;
}
