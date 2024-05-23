using System;
using System.Collections.Generic;

// Token: 0x02000134 RID: 308
public class AdventureDbfRecord : DbfRecord
{
	// Token: 0x17000257 RID: 599
	// (get) Token: 0x06000FB9 RID: 4025 RVA: 0x00044752 File Offset: 0x00042952
	[DbfField("NOTE_DESC", "designer note")]
	public string NoteDesc
	{
		get
		{
			return this.m_NoteDesc;
		}
	}

	// Token: 0x17000258 RID: 600
	// (get) Token: 0x06000FBA RID: 4026 RVA: 0x0004475A File Offset: 0x0004295A
	[DbfField("NAME", "")]
	public DbfLocValue Name
	{
		get
		{
			return this.m_Name;
		}
	}

	// Token: 0x17000259 RID: 601
	// (get) Token: 0x06000FBB RID: 4027 RVA: 0x00044762 File Offset: 0x00042962
	[DbfField("SORT_ORDER", "")]
	public int SortOrder
	{
		get
		{
			return this.m_SortOrder;
		}
	}

	// Token: 0x1700025A RID: 602
	// (get) Token: 0x06000FBC RID: 4028 RVA: 0x0004476A File Offset: 0x0004296A
	[DbfField("STORE_BUY_BUTTON_LABEL", "")]
	public DbfLocValue StoreBuyButtonLabel
	{
		get
		{
			return this.m_StoreBuyButtonLabel;
		}
	}

	// Token: 0x1700025B RID: 603
	// (get) Token: 0x06000FBD RID: 4029 RVA: 0x00044772 File Offset: 0x00042972
	[DbfField("STORE_BUY_WINGS_1_HEADLINE", "")]
	public DbfLocValue StoreBuyWings1Headline
	{
		get
		{
			return this.m_StoreBuyWings1Headline;
		}
	}

	// Token: 0x1700025C RID: 604
	// (get) Token: 0x06000FBE RID: 4030 RVA: 0x0004477A File Offset: 0x0004297A
	[DbfField("STORE_BUY_WINGS_2_HEADLINE", "")]
	public DbfLocValue StoreBuyWings2Headline
	{
		get
		{
			return this.m_StoreBuyWings2Headline;
		}
	}

	// Token: 0x1700025D RID: 605
	// (get) Token: 0x06000FBF RID: 4031 RVA: 0x00044782 File Offset: 0x00042982
	[DbfField("STORE_BUY_WINGS_3_HEADLINE", "")]
	public DbfLocValue StoreBuyWings3Headline
	{
		get
		{
			return this.m_StoreBuyWings3Headline;
		}
	}

	// Token: 0x1700025E RID: 606
	// (get) Token: 0x06000FC0 RID: 4032 RVA: 0x0004478A File Offset: 0x0004298A
	[DbfField("STORE_BUY_WINGS_4_HEADLINE", "")]
	public DbfLocValue StoreBuyWings4Headline
	{
		get
		{
			return this.m_StoreBuyWings4Headline;
		}
	}

	// Token: 0x1700025F RID: 607
	// (get) Token: 0x06000FC1 RID: 4033 RVA: 0x00044792 File Offset: 0x00042992
	[DbfField("STORE_BUY_WINGS_5_HEADLINE", "")]
	public DbfLocValue StoreBuyWings5Headline
	{
		get
		{
			return this.m_StoreBuyWings5Headline;
		}
	}

	// Token: 0x17000260 RID: 608
	// (get) Token: 0x06000FC2 RID: 4034 RVA: 0x0004479A File Offset: 0x0004299A
	[DbfField("STORE_OWNED_HEADLINE", "")]
	public DbfLocValue StoreOwnedHeadline
	{
		get
		{
			return this.m_StoreOwnedHeadline;
		}
	}

	// Token: 0x17000261 RID: 609
	// (get) Token: 0x06000FC3 RID: 4035 RVA: 0x000447A2 File Offset: 0x000429A2
	[DbfField("STORE_PREORDER_HEADLINE", "")]
	public DbfLocValue StorePreorderHeadline
	{
		get
		{
			return this.m_StorePreorderHeadline;
		}
	}

	// Token: 0x17000262 RID: 610
	// (get) Token: 0x06000FC4 RID: 4036 RVA: 0x000447AA File Offset: 0x000429AA
	[DbfField("STORE_BUY_WINGS_1_DESC", "")]
	public DbfLocValue StoreBuyWings1Desc
	{
		get
		{
			return this.m_StoreBuyWings1Desc;
		}
	}

	// Token: 0x17000263 RID: 611
	// (get) Token: 0x06000FC5 RID: 4037 RVA: 0x000447B2 File Offset: 0x000429B2
	[DbfField("STORE_BUY_WINGS_2_DESC", "")]
	public DbfLocValue StoreBuyWings2Desc
	{
		get
		{
			return this.m_StoreBuyWings2Desc;
		}
	}

	// Token: 0x17000264 RID: 612
	// (get) Token: 0x06000FC6 RID: 4038 RVA: 0x000447BA File Offset: 0x000429BA
	[DbfField("STORE_BUY_WINGS_3_DESC", "")]
	public DbfLocValue StoreBuyWings3Desc
	{
		get
		{
			return this.m_StoreBuyWings3Desc;
		}
	}

	// Token: 0x17000265 RID: 613
	// (get) Token: 0x06000FC7 RID: 4039 RVA: 0x000447C2 File Offset: 0x000429C2
	[DbfField("STORE_BUY_WINGS_4_DESC", "")]
	public DbfLocValue StoreBuyWings4Desc
	{
		get
		{
			return this.m_StoreBuyWings4Desc;
		}
	}

	// Token: 0x17000266 RID: 614
	// (get) Token: 0x06000FC8 RID: 4040 RVA: 0x000447CA File Offset: 0x000429CA
	[DbfField("STORE_BUY_WINGS_5_DESC", "")]
	public DbfLocValue StoreBuyWings5Desc
	{
		get
		{
			return this.m_StoreBuyWings5Desc;
		}
	}

	// Token: 0x17000267 RID: 615
	// (get) Token: 0x06000FC9 RID: 4041 RVA: 0x000447D2 File Offset: 0x000429D2
	[DbfField("STORE_OWNED_DESC", "")]
	public DbfLocValue StoreOwnedDesc
	{
		get
		{
			return this.m_StoreOwnedDesc;
		}
	}

	// Token: 0x17000268 RID: 616
	// (get) Token: 0x06000FCA RID: 4042 RVA: 0x000447DA File Offset: 0x000429DA
	[DbfField("STORE_PREORDER_DESC", "")]
	public DbfLocValue StorePreorderDesc
	{
		get
		{
			return this.m_StorePreorderDesc;
		}
	}

	// Token: 0x17000269 RID: 617
	// (get) Token: 0x06000FCB RID: 4043 RVA: 0x000447E2 File Offset: 0x000429E2
	[DbfField("STORE_PREVIEW_REWARDS_TEXT", "")]
	public DbfLocValue StorePreviewRewardsText
	{
		get
		{
			return this.m_StorePreviewRewardsText;
		}
	}

	// Token: 0x1700026A RID: 618
	// (get) Token: 0x06000FCC RID: 4044 RVA: 0x000447EA File Offset: 0x000429EA
	[DbfField("ADVENTURE_DEF_PREFAB", "")]
	public string AdventureDefPrefab
	{
		get
		{
			return this.m_AdventureDefPrefab;
		}
	}

	// Token: 0x1700026B RID: 619
	// (get) Token: 0x06000FCD RID: 4045 RVA: 0x000447F2 File Offset: 0x000429F2
	[DbfField("STORE_PREFAB", "")]
	public string StorePrefab
	{
		get
		{
			return this.m_StorePrefab;
		}
	}

	// Token: 0x1700026C RID: 620
	// (get) Token: 0x06000FCE RID: 4046 RVA: 0x000447FA File Offset: 0x000429FA
	[DbfField("LEAVING_SOON", "this adventure will be removed from the in-game store soon")]
	public bool LeavingSoon
	{
		get
		{
			return this.m_LeavingSoon;
		}
	}

	// Token: 0x1700026D RID: 621
	// (get) Token: 0x06000FCF RID: 4047 RVA: 0x00044802 File Offset: 0x00042A02
	[DbfField("LEAVING_SOON_TEXT", "localized text id explaining why this adventure will be removed from the in-game store soon")]
	public DbfLocValue LeavingSoonText
	{
		get
		{
			return this.m_LeavingSoonText;
		}
	}

	// Token: 0x06000FD0 RID: 4048 RVA: 0x0004480A File Offset: 0x00042A0A
	public void SetNoteDesc(string v)
	{
		this.m_NoteDesc = v;
	}

	// Token: 0x06000FD1 RID: 4049 RVA: 0x00044813 File Offset: 0x00042A13
	public void SetName(DbfLocValue v)
	{
		this.m_Name = v;
		v.SetDebugInfo(base.ID, "NAME");
	}

	// Token: 0x06000FD2 RID: 4050 RVA: 0x0004482D File Offset: 0x00042A2D
	public void SetSortOrder(int v)
	{
		this.m_SortOrder = v;
	}

	// Token: 0x06000FD3 RID: 4051 RVA: 0x00044836 File Offset: 0x00042A36
	public void SetStoreBuyButtonLabel(DbfLocValue v)
	{
		this.m_StoreBuyButtonLabel = v;
		v.SetDebugInfo(base.ID, "STORE_BUY_BUTTON_LABEL");
	}

	// Token: 0x06000FD4 RID: 4052 RVA: 0x00044850 File Offset: 0x00042A50
	public void SetStoreBuyWings1Headline(DbfLocValue v)
	{
		this.m_StoreBuyWings1Headline = v;
		v.SetDebugInfo(base.ID, "STORE_BUY_WINGS_1_HEADLINE");
	}

	// Token: 0x06000FD5 RID: 4053 RVA: 0x0004486A File Offset: 0x00042A6A
	public void SetStoreBuyWings2Headline(DbfLocValue v)
	{
		this.m_StoreBuyWings2Headline = v;
		v.SetDebugInfo(base.ID, "STORE_BUY_WINGS_2_HEADLINE");
	}

	// Token: 0x06000FD6 RID: 4054 RVA: 0x00044884 File Offset: 0x00042A84
	public void SetStoreBuyWings3Headline(DbfLocValue v)
	{
		this.m_StoreBuyWings3Headline = v;
		v.SetDebugInfo(base.ID, "STORE_BUY_WINGS_3_HEADLINE");
	}

	// Token: 0x06000FD7 RID: 4055 RVA: 0x0004489E File Offset: 0x00042A9E
	public void SetStoreBuyWings4Headline(DbfLocValue v)
	{
		this.m_StoreBuyWings4Headline = v;
		v.SetDebugInfo(base.ID, "STORE_BUY_WINGS_4_HEADLINE");
	}

	// Token: 0x06000FD8 RID: 4056 RVA: 0x000448B8 File Offset: 0x00042AB8
	public void SetStoreBuyWings5Headline(DbfLocValue v)
	{
		this.m_StoreBuyWings5Headline = v;
		v.SetDebugInfo(base.ID, "STORE_BUY_WINGS_5_HEADLINE");
	}

	// Token: 0x06000FD9 RID: 4057 RVA: 0x000448D2 File Offset: 0x00042AD2
	public void SetStoreOwnedHeadline(DbfLocValue v)
	{
		this.m_StoreOwnedHeadline = v;
		v.SetDebugInfo(base.ID, "STORE_OWNED_HEADLINE");
	}

	// Token: 0x06000FDA RID: 4058 RVA: 0x000448EC File Offset: 0x00042AEC
	public void SetStorePreorderHeadline(DbfLocValue v)
	{
		this.m_StorePreorderHeadline = v;
		v.SetDebugInfo(base.ID, "STORE_PREORDER_HEADLINE");
	}

	// Token: 0x06000FDB RID: 4059 RVA: 0x00044906 File Offset: 0x00042B06
	public void SetStoreBuyWings1Desc(DbfLocValue v)
	{
		this.m_StoreBuyWings1Desc = v;
		v.SetDebugInfo(base.ID, "STORE_BUY_WINGS_1_DESC");
	}

	// Token: 0x06000FDC RID: 4060 RVA: 0x00044920 File Offset: 0x00042B20
	public void SetStoreBuyWings2Desc(DbfLocValue v)
	{
		this.m_StoreBuyWings2Desc = v;
		v.SetDebugInfo(base.ID, "STORE_BUY_WINGS_2_DESC");
	}

	// Token: 0x06000FDD RID: 4061 RVA: 0x0004493A File Offset: 0x00042B3A
	public void SetStoreBuyWings3Desc(DbfLocValue v)
	{
		this.m_StoreBuyWings3Desc = v;
		v.SetDebugInfo(base.ID, "STORE_BUY_WINGS_3_DESC");
	}

	// Token: 0x06000FDE RID: 4062 RVA: 0x00044954 File Offset: 0x00042B54
	public void SetStoreBuyWings4Desc(DbfLocValue v)
	{
		this.m_StoreBuyWings4Desc = v;
		v.SetDebugInfo(base.ID, "STORE_BUY_WINGS_4_DESC");
	}

	// Token: 0x06000FDF RID: 4063 RVA: 0x0004496E File Offset: 0x00042B6E
	public void SetStoreBuyWings5Desc(DbfLocValue v)
	{
		this.m_StoreBuyWings5Desc = v;
		v.SetDebugInfo(base.ID, "STORE_BUY_WINGS_5_DESC");
	}

	// Token: 0x06000FE0 RID: 4064 RVA: 0x00044988 File Offset: 0x00042B88
	public void SetStoreOwnedDesc(DbfLocValue v)
	{
		this.m_StoreOwnedDesc = v;
		v.SetDebugInfo(base.ID, "STORE_OWNED_DESC");
	}

	// Token: 0x06000FE1 RID: 4065 RVA: 0x000449A2 File Offset: 0x00042BA2
	public void SetStorePreorderDesc(DbfLocValue v)
	{
		this.m_StorePreorderDesc = v;
		v.SetDebugInfo(base.ID, "STORE_PREORDER_DESC");
	}

	// Token: 0x06000FE2 RID: 4066 RVA: 0x000449BC File Offset: 0x00042BBC
	public void SetStorePreviewRewardsText(DbfLocValue v)
	{
		this.m_StorePreviewRewardsText = v;
		v.SetDebugInfo(base.ID, "STORE_PREVIEW_REWARDS_TEXT");
	}

	// Token: 0x06000FE3 RID: 4067 RVA: 0x000449D6 File Offset: 0x00042BD6
	public void SetAdventureDefPrefab(string v)
	{
		this.m_AdventureDefPrefab = v;
	}

	// Token: 0x06000FE4 RID: 4068 RVA: 0x000449DF File Offset: 0x00042BDF
	public void SetStorePrefab(string v)
	{
		this.m_StorePrefab = v;
	}

	// Token: 0x06000FE5 RID: 4069 RVA: 0x000449E8 File Offset: 0x00042BE8
	public void SetLeavingSoon(bool v)
	{
		this.m_LeavingSoon = v;
	}

	// Token: 0x06000FE6 RID: 4070 RVA: 0x000449F1 File Offset: 0x00042BF1
	public void SetLeavingSoonText(DbfLocValue v)
	{
		this.m_LeavingSoonText = v;
		v.SetDebugInfo(base.ID, "LEAVING_SOON_TEXT");
	}

	// Token: 0x06000FE7 RID: 4071 RVA: 0x00044A0C File Offset: 0x00042C0C
	public override object GetVar(string name)
	{
		if (name != null)
		{
			if (AdventureDbfRecord.<>f__switch$map7 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(24);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("NAME", 2);
				dictionary.Add("SORT_ORDER", 3);
				dictionary.Add("STORE_BUY_BUTTON_LABEL", 4);
				dictionary.Add("STORE_BUY_WINGS_1_HEADLINE", 5);
				dictionary.Add("STORE_BUY_WINGS_2_HEADLINE", 6);
				dictionary.Add("STORE_BUY_WINGS_3_HEADLINE", 7);
				dictionary.Add("STORE_BUY_WINGS_4_HEADLINE", 8);
				dictionary.Add("STORE_BUY_WINGS_5_HEADLINE", 9);
				dictionary.Add("STORE_OWNED_HEADLINE", 10);
				dictionary.Add("STORE_PREORDER_HEADLINE", 11);
				dictionary.Add("STORE_BUY_WINGS_1_DESC", 12);
				dictionary.Add("STORE_BUY_WINGS_2_DESC", 13);
				dictionary.Add("STORE_BUY_WINGS_3_DESC", 14);
				dictionary.Add("STORE_BUY_WINGS_4_DESC", 15);
				dictionary.Add("STORE_BUY_WINGS_5_DESC", 16);
				dictionary.Add("STORE_OWNED_DESC", 17);
				dictionary.Add("STORE_PREORDER_DESC", 18);
				dictionary.Add("STORE_PREVIEW_REWARDS_TEXT", 19);
				dictionary.Add("ADVENTURE_DEF_PREFAB", 20);
				dictionary.Add("STORE_PREFAB", 21);
				dictionary.Add("LEAVING_SOON", 22);
				dictionary.Add("LEAVING_SOON_TEXT", 23);
				AdventureDbfRecord.<>f__switch$map7 = dictionary;
			}
			int num;
			if (AdventureDbfRecord.<>f__switch$map7.TryGetValue(name, ref num))
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
					return this.SortOrder;
				case 4:
					return this.StoreBuyButtonLabel;
				case 5:
					return this.StoreBuyWings1Headline;
				case 6:
					return this.StoreBuyWings2Headline;
				case 7:
					return this.StoreBuyWings3Headline;
				case 8:
					return this.StoreBuyWings4Headline;
				case 9:
					return this.StoreBuyWings5Headline;
				case 10:
					return this.StoreOwnedHeadline;
				case 11:
					return this.StorePreorderHeadline;
				case 12:
					return this.StoreBuyWings1Desc;
				case 13:
					return this.StoreBuyWings2Desc;
				case 14:
					return this.StoreBuyWings3Desc;
				case 15:
					return this.StoreBuyWings4Desc;
				case 16:
					return this.StoreBuyWings5Desc;
				case 17:
					return this.StoreOwnedDesc;
				case 18:
					return this.StorePreorderDesc;
				case 19:
					return this.StorePreviewRewardsText;
				case 20:
					return this.AdventureDefPrefab;
				case 21:
					return this.StorePrefab;
				case 22:
					return this.LeavingSoon;
				case 23:
					return this.LeavingSoonText;
				}
			}
		}
		return null;
	}

	// Token: 0x06000FE8 RID: 4072 RVA: 0x00044CA0 File Offset: 0x00042EA0
	public override void SetVar(string name, object val)
	{
		if (name != null)
		{
			if (AdventureDbfRecord.<>f__switch$map8 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(24);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("NAME", 2);
				dictionary.Add("SORT_ORDER", 3);
				dictionary.Add("STORE_BUY_BUTTON_LABEL", 4);
				dictionary.Add("STORE_BUY_WINGS_1_HEADLINE", 5);
				dictionary.Add("STORE_BUY_WINGS_2_HEADLINE", 6);
				dictionary.Add("STORE_BUY_WINGS_3_HEADLINE", 7);
				dictionary.Add("STORE_BUY_WINGS_4_HEADLINE", 8);
				dictionary.Add("STORE_BUY_WINGS_5_HEADLINE", 9);
				dictionary.Add("STORE_OWNED_HEADLINE", 10);
				dictionary.Add("STORE_PREORDER_HEADLINE", 11);
				dictionary.Add("STORE_BUY_WINGS_1_DESC", 12);
				dictionary.Add("STORE_BUY_WINGS_2_DESC", 13);
				dictionary.Add("STORE_BUY_WINGS_3_DESC", 14);
				dictionary.Add("STORE_BUY_WINGS_4_DESC", 15);
				dictionary.Add("STORE_BUY_WINGS_5_DESC", 16);
				dictionary.Add("STORE_OWNED_DESC", 17);
				dictionary.Add("STORE_PREORDER_DESC", 18);
				dictionary.Add("STORE_PREVIEW_REWARDS_TEXT", 19);
				dictionary.Add("ADVENTURE_DEF_PREFAB", 20);
				dictionary.Add("STORE_PREFAB", 21);
				dictionary.Add("LEAVING_SOON", 22);
				dictionary.Add("LEAVING_SOON_TEXT", 23);
				AdventureDbfRecord.<>f__switch$map8 = dictionary;
			}
			int num;
			if (AdventureDbfRecord.<>f__switch$map8.TryGetValue(name, ref num))
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
					this.SetSortOrder((int)val);
					break;
				case 4:
					this.SetStoreBuyButtonLabel((DbfLocValue)val);
					break;
				case 5:
					this.SetStoreBuyWings1Headline((DbfLocValue)val);
					break;
				case 6:
					this.SetStoreBuyWings2Headline((DbfLocValue)val);
					break;
				case 7:
					this.SetStoreBuyWings3Headline((DbfLocValue)val);
					break;
				case 8:
					this.SetStoreBuyWings4Headline((DbfLocValue)val);
					break;
				case 9:
					this.SetStoreBuyWings5Headline((DbfLocValue)val);
					break;
				case 10:
					this.SetStoreOwnedHeadline((DbfLocValue)val);
					break;
				case 11:
					this.SetStorePreorderHeadline((DbfLocValue)val);
					break;
				case 12:
					this.SetStoreBuyWings1Desc((DbfLocValue)val);
					break;
				case 13:
					this.SetStoreBuyWings2Desc((DbfLocValue)val);
					break;
				case 14:
					this.SetStoreBuyWings3Desc((DbfLocValue)val);
					break;
				case 15:
					this.SetStoreBuyWings4Desc((DbfLocValue)val);
					break;
				case 16:
					this.SetStoreBuyWings5Desc((DbfLocValue)val);
					break;
				case 17:
					this.SetStoreOwnedDesc((DbfLocValue)val);
					break;
				case 18:
					this.SetStorePreorderDesc((DbfLocValue)val);
					break;
				case 19:
					this.SetStorePreviewRewardsText((DbfLocValue)val);
					break;
				case 20:
					this.SetAdventureDefPrefab((string)val);
					break;
				case 21:
					this.SetStorePrefab((string)val);
					break;
				case 22:
					this.SetLeavingSoon((bool)val);
					break;
				case 23:
					this.SetLeavingSoonText((DbfLocValue)val);
					break;
				}
			}
		}
	}

	// Token: 0x06000FE9 RID: 4073 RVA: 0x00045014 File Offset: 0x00043214
	public override Type GetVarType(string name)
	{
		if (name != null)
		{
			if (AdventureDbfRecord.<>f__switch$map9 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(24);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("NAME", 2);
				dictionary.Add("SORT_ORDER", 3);
				dictionary.Add("STORE_BUY_BUTTON_LABEL", 4);
				dictionary.Add("STORE_BUY_WINGS_1_HEADLINE", 5);
				dictionary.Add("STORE_BUY_WINGS_2_HEADLINE", 6);
				dictionary.Add("STORE_BUY_WINGS_3_HEADLINE", 7);
				dictionary.Add("STORE_BUY_WINGS_4_HEADLINE", 8);
				dictionary.Add("STORE_BUY_WINGS_5_HEADLINE", 9);
				dictionary.Add("STORE_OWNED_HEADLINE", 10);
				dictionary.Add("STORE_PREORDER_HEADLINE", 11);
				dictionary.Add("STORE_BUY_WINGS_1_DESC", 12);
				dictionary.Add("STORE_BUY_WINGS_2_DESC", 13);
				dictionary.Add("STORE_BUY_WINGS_3_DESC", 14);
				dictionary.Add("STORE_BUY_WINGS_4_DESC", 15);
				dictionary.Add("STORE_BUY_WINGS_5_DESC", 16);
				dictionary.Add("STORE_OWNED_DESC", 17);
				dictionary.Add("STORE_PREORDER_DESC", 18);
				dictionary.Add("STORE_PREVIEW_REWARDS_TEXT", 19);
				dictionary.Add("ADVENTURE_DEF_PREFAB", 20);
				dictionary.Add("STORE_PREFAB", 21);
				dictionary.Add("LEAVING_SOON", 22);
				dictionary.Add("LEAVING_SOON_TEXT", 23);
				AdventureDbfRecord.<>f__switch$map9 = dictionary;
			}
			int num;
			if (AdventureDbfRecord.<>f__switch$map9.TryGetValue(name, ref num))
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
					return typeof(int);
				case 4:
					return typeof(DbfLocValue);
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
					return typeof(DbfLocValue);
				case 12:
					return typeof(DbfLocValue);
				case 13:
					return typeof(DbfLocValue);
				case 14:
					return typeof(DbfLocValue);
				case 15:
					return typeof(DbfLocValue);
				case 16:
					return typeof(DbfLocValue);
				case 17:
					return typeof(DbfLocValue);
				case 18:
					return typeof(DbfLocValue);
				case 19:
					return typeof(DbfLocValue);
				case 20:
					return typeof(string);
				case 21:
					return typeof(string);
				case 22:
					return typeof(bool);
				case 23:
					return typeof(DbfLocValue);
				}
			}
		}
		return null;
	}

	// Token: 0x04000858 RID: 2136
	private string m_NoteDesc;

	// Token: 0x04000859 RID: 2137
	private DbfLocValue m_Name;

	// Token: 0x0400085A RID: 2138
	private int m_SortOrder;

	// Token: 0x0400085B RID: 2139
	private DbfLocValue m_StoreBuyButtonLabel;

	// Token: 0x0400085C RID: 2140
	private DbfLocValue m_StoreBuyWings1Headline;

	// Token: 0x0400085D RID: 2141
	private DbfLocValue m_StoreBuyWings2Headline;

	// Token: 0x0400085E RID: 2142
	private DbfLocValue m_StoreBuyWings3Headline;

	// Token: 0x0400085F RID: 2143
	private DbfLocValue m_StoreBuyWings4Headline;

	// Token: 0x04000860 RID: 2144
	private DbfLocValue m_StoreBuyWings5Headline;

	// Token: 0x04000861 RID: 2145
	private DbfLocValue m_StoreOwnedHeadline;

	// Token: 0x04000862 RID: 2146
	private DbfLocValue m_StorePreorderHeadline;

	// Token: 0x04000863 RID: 2147
	private DbfLocValue m_StoreBuyWings1Desc;

	// Token: 0x04000864 RID: 2148
	private DbfLocValue m_StoreBuyWings2Desc;

	// Token: 0x04000865 RID: 2149
	private DbfLocValue m_StoreBuyWings3Desc;

	// Token: 0x04000866 RID: 2150
	private DbfLocValue m_StoreBuyWings4Desc;

	// Token: 0x04000867 RID: 2151
	private DbfLocValue m_StoreBuyWings5Desc;

	// Token: 0x04000868 RID: 2152
	private DbfLocValue m_StoreOwnedDesc;

	// Token: 0x04000869 RID: 2153
	private DbfLocValue m_StorePreorderDesc;

	// Token: 0x0400086A RID: 2154
	private DbfLocValue m_StorePreviewRewardsText;

	// Token: 0x0400086B RID: 2155
	private string m_AdventureDefPrefab;

	// Token: 0x0400086C RID: 2156
	private string m_StorePrefab;

	// Token: 0x0400086D RID: 2157
	private bool m_LeavingSoon;

	// Token: 0x0400086E RID: 2158
	private DbfLocValue m_LeavingSoonText;
}
