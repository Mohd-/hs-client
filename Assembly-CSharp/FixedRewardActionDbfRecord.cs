using System;
using System.Collections.Generic;

// Token: 0x02000143 RID: 323
public class FixedRewardActionDbfRecord : DbfRecord
{
	// Token: 0x170002C2 RID: 706
	// (get) Token: 0x060010CB RID: 4299 RVA: 0x0004870A File Offset: 0x0004690A
	[DbfField("NOTE_DESC", "designer note")]
	public string NoteDesc
	{
		get
		{
			return this.m_NoteDesc;
		}
	}

	// Token: 0x170002C3 RID: 707
	// (get) Token: 0x060010CC RID: 4300 RVA: 0x00048712 File Offset: 0x00046912
	[DbfField("TYPE", "action trigger")]
	public string Type
	{
		get
		{
			return this.m_Type;
		}
	}

	// Token: 0x170002C4 RID: 708
	// (get) Token: 0x060010CD RID: 4301 RVA: 0x0004871A File Offset: 0x0004691A
	[DbfField("WING_ID", "WING.ID required for this action. 0 means ignore.")]
	public int WingId
	{
		get
		{
			return this.m_WingId;
		}
	}

	// Token: 0x170002C5 RID: 709
	// (get) Token: 0x060010CE RID: 4302 RVA: 0x00048722 File Offset: 0x00046922
	[DbfField("WING_PROGRESS", "wing progress required for this action. 0 means ignore.")]
	public int WingProgress
	{
		get
		{
			return this.m_WingProgress;
		}
	}

	// Token: 0x170002C6 RID: 710
	// (get) Token: 0x060010CF RID: 4303 RVA: 0x0004872A File Offset: 0x0004692A
	[DbfField("WING_FLAGS", "wing flags required for this action. 0 means ignore. flags are defined by data in ADVENTURE_MISSION table's GRANTS_FLAGS column.")]
	public ulong WingFlags
	{
		get
		{
			return this.m_WingFlags;
		}
	}

	// Token: 0x170002C7 RID: 711
	// (get) Token: 0x060010D0 RID: 4304 RVA: 0x00048732 File Offset: 0x00046932
	[DbfField("CLASS_ID", "CLASS.ID required for this action. 0 means ignore.")]
	public int ClassId
	{
		get
		{
			return this.m_ClassId;
		}
	}

	// Token: 0x170002C8 RID: 712
	// (get) Token: 0x060010D1 RID: 4305 RVA: 0x0004873A File Offset: 0x0004693A
	[DbfField("HERO_LEVEL", "hero level required for this action. 0 means ignore.")]
	public int HeroLevel
	{
		get
		{
			return this.m_HeroLevel;
		}
	}

	// Token: 0x170002C9 RID: 713
	// (get) Token: 0x060010D2 RID: 4306 RVA: 0x00048742 File Offset: 0x00046942
	[DbfField("TUTORIAL_PROGRESS", "tutorial progress required for this action. 0 means ignore.")]
	public int TutorialProgress
	{
		get
		{
			return this.m_TutorialProgress;
		}
	}

	// Token: 0x170002CA RID: 714
	// (get) Token: 0x060010D3 RID: 4307 RVA: 0x0004874A File Offset: 0x0004694A
	[DbfField("META_ACTION_FLAGS", "flags required for this meta-action. 0 means ignore. flags are defined by data in FIXED_REWARD table's META_ACTION_FLAGS column only (not defined in code anywhere).")]
	public ulong MetaActionFlags
	{
		get
		{
			return this.m_MetaActionFlags;
		}
	}

	// Token: 0x170002CB RID: 715
	// (get) Token: 0x060010D4 RID: 4308 RVA: 0x00048752 File Offset: 0x00046952
	[DbfField("ACHIEVE_ID", "ACHIEVE.ID required for this action. 0 means ignore.")]
	public int AchieveId
	{
		get
		{
			return this.m_AchieveId;
		}
	}

	// Token: 0x170002CC RID: 716
	// (get) Token: 0x060010D5 RID: 4309 RVA: 0x0004875A File Offset: 0x0004695A
	[DbfField("ACCOUNT_LICENSE_ID", "ACCOUNT_LICENSE.ID required for this action. 0 means ignore.")]
	public long AccountLicenseId
	{
		get
		{
			return this.m_AccountLicenseId;
		}
	}

	// Token: 0x170002CD RID: 717
	// (get) Token: 0x060010D6 RID: 4310 RVA: 0x00048762 File Offset: 0x00046962
	[DbfField("ACCOUNT_LICENSE_FLAGS", "account license flags required for this action. 0 means ignore.")]
	public ulong AccountLicenseFlags
	{
		get
		{
			return this.m_AccountLicenseFlags;
		}
	}

	// Token: 0x060010D7 RID: 4311 RVA: 0x0004876A File Offset: 0x0004696A
	public void SetNoteDesc(string v)
	{
		this.m_NoteDesc = v;
	}

	// Token: 0x060010D8 RID: 4312 RVA: 0x00048773 File Offset: 0x00046973
	public void SetType(string v)
	{
		this.m_Type = v;
	}

	// Token: 0x060010D9 RID: 4313 RVA: 0x0004877C File Offset: 0x0004697C
	public void SetWingId(int v)
	{
		this.m_WingId = v;
	}

	// Token: 0x060010DA RID: 4314 RVA: 0x00048785 File Offset: 0x00046985
	public void SetWingProgress(int v)
	{
		this.m_WingProgress = v;
	}

	// Token: 0x060010DB RID: 4315 RVA: 0x0004878E File Offset: 0x0004698E
	public void SetWingFlags(ulong v)
	{
		this.m_WingFlags = v;
	}

	// Token: 0x060010DC RID: 4316 RVA: 0x00048797 File Offset: 0x00046997
	public void SetClassId(int v)
	{
		this.m_ClassId = v;
	}

	// Token: 0x060010DD RID: 4317 RVA: 0x000487A0 File Offset: 0x000469A0
	public void SetHeroLevel(int v)
	{
		this.m_HeroLevel = v;
	}

	// Token: 0x060010DE RID: 4318 RVA: 0x000487A9 File Offset: 0x000469A9
	public void SetTutorialProgress(int v)
	{
		this.m_TutorialProgress = v;
	}

	// Token: 0x060010DF RID: 4319 RVA: 0x000487B2 File Offset: 0x000469B2
	public void SetMetaActionFlags(ulong v)
	{
		this.m_MetaActionFlags = v;
	}

	// Token: 0x060010E0 RID: 4320 RVA: 0x000487BB File Offset: 0x000469BB
	public void SetAchieveId(int v)
	{
		this.m_AchieveId = v;
	}

	// Token: 0x060010E1 RID: 4321 RVA: 0x000487C4 File Offset: 0x000469C4
	public void SetAccountLicenseId(long v)
	{
		this.m_AccountLicenseId = v;
	}

	// Token: 0x060010E2 RID: 4322 RVA: 0x000487CD File Offset: 0x000469CD
	public void SetAccountLicenseFlags(ulong v)
	{
		this.m_AccountLicenseFlags = v;
	}

	// Token: 0x060010E3 RID: 4323 RVA: 0x000487D8 File Offset: 0x000469D8
	public override object GetVar(string name)
	{
		if (name != null)
		{
			if (FixedRewardActionDbfRecord.<>f__switch$map31 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(13);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("TYPE", 2);
				dictionary.Add("WING_ID", 3);
				dictionary.Add("WING_PROGRESS", 4);
				dictionary.Add("WING_FLAGS", 5);
				dictionary.Add("CLASS_ID", 6);
				dictionary.Add("HERO_LEVEL", 7);
				dictionary.Add("TUTORIAL_PROGRESS", 8);
				dictionary.Add("META_ACTION_FLAGS", 9);
				dictionary.Add("ACHIEVE_ID", 10);
				dictionary.Add("ACCOUNT_LICENSE_ID", 11);
				dictionary.Add("ACCOUNT_LICENSE_FLAGS", 12);
				FixedRewardActionDbfRecord.<>f__switch$map31 = dictionary;
			}
			int num;
			if (FixedRewardActionDbfRecord.<>f__switch$map31.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return base.ID;
				case 1:
					return this.NoteDesc;
				case 2:
					return this.Type;
				case 3:
					return this.WingId;
				case 4:
					return this.WingProgress;
				case 5:
					return this.WingFlags;
				case 6:
					return this.ClassId;
				case 7:
					return this.HeroLevel;
				case 8:
					return this.TutorialProgress;
				case 9:
					return this.MetaActionFlags;
				case 10:
					return this.AchieveId;
				case 11:
					return this.AccountLicenseId;
				case 12:
					return this.AccountLicenseFlags;
				}
			}
		}
		return null;
	}

	// Token: 0x060010E4 RID: 4324 RVA: 0x0004898C File Offset: 0x00046B8C
	public override void SetVar(string name, object val)
	{
		if (name != null)
		{
			if (FixedRewardActionDbfRecord.<>f__switch$map32 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(13);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("TYPE", 2);
				dictionary.Add("WING_ID", 3);
				dictionary.Add("WING_PROGRESS", 4);
				dictionary.Add("WING_FLAGS", 5);
				dictionary.Add("CLASS_ID", 6);
				dictionary.Add("HERO_LEVEL", 7);
				dictionary.Add("TUTORIAL_PROGRESS", 8);
				dictionary.Add("META_ACTION_FLAGS", 9);
				dictionary.Add("ACHIEVE_ID", 10);
				dictionary.Add("ACCOUNT_LICENSE_ID", 11);
				dictionary.Add("ACCOUNT_LICENSE_FLAGS", 12);
				FixedRewardActionDbfRecord.<>f__switch$map32 = dictionary;
			}
			int num;
			if (FixedRewardActionDbfRecord.<>f__switch$map32.TryGetValue(name, ref num))
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
					this.SetType((string)val);
					break;
				case 3:
					this.SetWingId((int)val);
					break;
				case 4:
					this.SetWingProgress((int)val);
					break;
				case 5:
					this.SetWingFlags((ulong)val);
					break;
				case 6:
					this.SetClassId((int)val);
					break;
				case 7:
					this.SetHeroLevel((int)val);
					break;
				case 8:
					this.SetTutorialProgress((int)val);
					break;
				case 9:
					this.SetMetaActionFlags((ulong)val);
					break;
				case 10:
					this.SetAchieveId((int)val);
					break;
				case 11:
					this.SetAccountLicenseId((long)val);
					break;
				case 12:
					this.SetAccountLicenseFlags((ulong)val);
					break;
				}
			}
		}
	}

	// Token: 0x060010E5 RID: 4325 RVA: 0x00048B88 File Offset: 0x00046D88
	public override Type GetVarType(string name)
	{
		if (name != null)
		{
			if (FixedRewardActionDbfRecord.<>f__switch$map33 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(13);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("TYPE", 2);
				dictionary.Add("WING_ID", 3);
				dictionary.Add("WING_PROGRESS", 4);
				dictionary.Add("WING_FLAGS", 5);
				dictionary.Add("CLASS_ID", 6);
				dictionary.Add("HERO_LEVEL", 7);
				dictionary.Add("TUTORIAL_PROGRESS", 8);
				dictionary.Add("META_ACTION_FLAGS", 9);
				dictionary.Add("ACHIEVE_ID", 10);
				dictionary.Add("ACCOUNT_LICENSE_ID", 11);
				dictionary.Add("ACCOUNT_LICENSE_FLAGS", 12);
				FixedRewardActionDbfRecord.<>f__switch$map33 = dictionary;
			}
			int num;
			if (FixedRewardActionDbfRecord.<>f__switch$map33.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return typeof(int);
				case 1:
					return typeof(string);
				case 2:
					return typeof(string);
				case 3:
					return typeof(int);
				case 4:
					return typeof(int);
				case 5:
					return typeof(ulong);
				case 6:
					return typeof(int);
				case 7:
					return typeof(int);
				case 8:
					return typeof(int);
				case 9:
					return typeof(ulong);
				case 10:
					return typeof(int);
				case 11:
					return typeof(long);
				case 12:
					return typeof(ulong);
				}
			}
		}
		return null;
	}

	// Token: 0x040008F0 RID: 2288
	private string m_NoteDesc;

	// Token: 0x040008F1 RID: 2289
	private string m_Type;

	// Token: 0x040008F2 RID: 2290
	private int m_WingId;

	// Token: 0x040008F3 RID: 2291
	private int m_WingProgress;

	// Token: 0x040008F4 RID: 2292
	private ulong m_WingFlags;

	// Token: 0x040008F5 RID: 2293
	private int m_ClassId;

	// Token: 0x040008F6 RID: 2294
	private int m_HeroLevel;

	// Token: 0x040008F7 RID: 2295
	private int m_TutorialProgress;

	// Token: 0x040008F8 RID: 2296
	private ulong m_MetaActionFlags;

	// Token: 0x040008F9 RID: 2297
	private int m_AchieveId;

	// Token: 0x040008FA RID: 2298
	private long m_AccountLicenseId;

	// Token: 0x040008FB RID: 2299
	private ulong m_AccountLicenseFlags;
}
