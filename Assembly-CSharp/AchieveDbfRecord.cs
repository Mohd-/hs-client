using System;
using System.Collections.Generic;

// Token: 0x02000133 RID: 307
public class AchieveDbfRecord : DbfRecord
{
	// Token: 0x17000242 RID: 578
	// (get) Token: 0x06000F8B RID: 3979 RVA: 0x00043D44 File Offset: 0x00041F44
	[DbfField("NOTE_DESC", "designer description of achievement")]
	public string NoteDesc
	{
		get
		{
			return this.m_NoteDesc;
		}
	}

	// Token: 0x17000243 RID: 579
	// (get) Token: 0x06000F8C RID: 3980 RVA: 0x00043D4C File Offset: 0x00041F4C
	[DbfField("ACH_TYPE", "a comment field to show the type of quest for designers")]
	public string AchType
	{
		get
		{
			return this.m_AchType;
		}
	}

	// Token: 0x17000244 RID: 580
	// (get) Token: 0x06000F8D RID: 3981 RVA: 0x00043D54 File Offset: 0x00041F54
	[DbfField("ENABLED", "is awarding this achievement and making progress on it currently enabled?")]
	public bool Enabled
	{
		get
		{
			return this.m_Enabled;
		}
	}

	// Token: 0x17000245 RID: 581
	// (get) Token: 0x06000F8E RID: 3982 RVA: 0x00043D5C File Offset: 0x00041F5C
	[DbfField("PARENT_ACH", "NOTE_DESC of the parent of this achieve, which means progressing in this achieve also progresses the parent achieve.")]
	public string ParentAch
	{
		get
		{
			return this.m_ParentAch;
		}
	}

	// Token: 0x17000246 RID: 582
	// (get) Token: 0x06000F8F RID: 3983 RVA: 0x00043D64 File Offset: 0x00041F64
	[DbfField("CLIENT_FLAGS", "additional properties bitfield. 0x01=is_legendary, see client enum Achievement.ClientFlags")]
	public int ClientFlags
	{
		get
		{
			return this.m_ClientFlags;
		}
	}

	// Token: 0x17000247 RID: 583
	// (get) Token: 0x06000F90 RID: 3984 RVA: 0x00043D6C File Offset: 0x00041F6C
	[DbfField("TRIGGERED", "trigger known to server, gets converted for text to an enum on the server; default is 'none' (see server s_achieveTriggerNames)")]
	public string Triggered
	{
		get
		{
			return this.m_Triggered;
		}
	}

	// Token: 0x17000248 RID: 584
	// (get) Token: 0x06000F91 RID: 3985 RVA: 0x00043D74 File Offset: 0x00041F74
	[DbfField("ACH_QUOTA", "how many times must whatever causes this to advance be done before it's completed?")]
	public int AchQuota
	{
		get
		{
			return this.m_AchQuota;
		}
	}

	// Token: 0x17000249 RID: 585
	// (get) Token: 0x06000F92 RID: 3986 RVA: 0x00043D7C File Offset: 0x00041F7C
	[DbfField("EVENT", "event required for trigger condition. events are managed server-side since they are case-by-case specific; default is 'none'")]
	public string Event
	{
		get
		{
			return this.m_Event;
		}
	}

	// Token: 0x1700024A RID: 586
	// (get) Token: 0x06000F93 RID: 3987 RVA: 0x00043D84 File Offset: 0x00041F84
	[DbfField("RACE", "CARD.RACE required for the trigger condition")]
	public int Race
	{
		get
		{
			return this.m_Race;
		}
	}

	// Token: 0x1700024B RID: 587
	// (get) Token: 0x06000F94 RID: 3988 RVA: 0x00043D8C File Offset: 0x00041F8C
	[DbfField("CARD_SET", "CARD.CARD_SET_ID required for the trigger condition")]
	public int CardSet
	{
		get
		{
			return this.m_CardSet;
		}
	}

	// Token: 0x1700024C RID: 588
	// (get) Token: 0x06000F95 RID: 3989 RVA: 0x00043D94 File Offset: 0x00041F94
	[DbfField("BOOSTER", "BOOSTER.ID required for the trigger condition")]
	public int Booster
	{
		get
		{
			return this.m_Booster;
		}
	}

	// Token: 0x1700024D RID: 589
	// (get) Token: 0x06000F96 RID: 3990 RVA: 0x00043D9C File Offset: 0x00041F9C
	[DbfField("REWARD_TIMING", "when should the client show the reward associated with this achievement? see client enum RewardVisualTiming; default is 'immediate'")]
	public string RewardTiming
	{
		get
		{
			return this.m_RewardTiming;
		}
	}

	// Token: 0x1700024E RID: 590
	// (get) Token: 0x06000F97 RID: 3991 RVA: 0x00043DA4 File Offset: 0x00041FA4
	[DbfField("REWARD", "reward name that we give for completing this achieve; can be 'none' for those achieves that reveal other achieves (see server sParseRewardType)")]
	public string Reward
	{
		get
		{
			return this.m_Reward;
		}
	}

	// Token: 0x1700024F RID: 591
	// (get) Token: 0x06000F98 RID: 3992 RVA: 0x00043DAC File Offset: 0x00041FAC
	[DbfField("REWARD_DATA1", "reward parameter (used by server for the reward ID for this achieve)")]
	public long RewardData1
	{
		get
		{
			return this.m_RewardData1;
		}
	}

	// Token: 0x17000250 RID: 592
	// (get) Token: 0x06000F99 RID: 3993 RVA: 0x00043DB4 File Offset: 0x00041FB4
	[DbfField("REWARD_DATA2", "reward parameter (used by server for the reward ID for this achieve)")]
	public long RewardData2
	{
		get
		{
			return this.m_RewardData2;
		}
	}

	// Token: 0x17000251 RID: 593
	// (get) Token: 0x06000F9A RID: 3994 RVA: 0x00043DBC File Offset: 0x00041FBC
	[DbfField("UNLOCKS", "game system/feature that is unlocked by this; default is 'none' (see server ParseAchieveUnlockableFeature)")]
	public string Unlocks
	{
		get
		{
			return this.m_Unlocks;
		}
	}

	// Token: 0x17000252 RID: 594
	// (get) Token: 0x06000F9B RID: 3995 RVA: 0x00043DC4 File Offset: 0x00041FC4
	[DbfField("NAME", "")]
	public DbfLocValue Name
	{
		get
		{
			return this.m_Name;
		}
	}

	// Token: 0x17000253 RID: 595
	// (get) Token: 0x06000F9C RID: 3996 RVA: 0x00043DCC File Offset: 0x00041FCC
	[DbfField("DESCRIPTION", "")]
	public DbfLocValue Description
	{
		get
		{
			return this.m_Description;
		}
	}

	// Token: 0x17000254 RID: 596
	// (get) Token: 0x06000F9D RID: 3997 RVA: 0x00043DD4 File Offset: 0x00041FD4
	[DbfField("ALT_TEXT_PREDICATE", "how to determine whether or not use alternate text fields, see client enum Achievement.Predicate")]
	public string AltTextPredicate
	{
		get
		{
			return this.m_AltTextPredicate;
		}
	}

	// Token: 0x17000255 RID: 597
	// (get) Token: 0x06000F9E RID: 3998 RVA: 0x00043DDC File Offset: 0x00041FDC
	[DbfField("ALT_NAME", "if not-null, this name is used when the condition specified by ALT_TEXT_PREDICATE evaluates to true.")]
	public DbfLocValue AltName
	{
		get
		{
			return this.m_AltName;
		}
	}

	// Token: 0x17000256 RID: 598
	// (get) Token: 0x06000F9F RID: 3999 RVA: 0x00043DE4 File Offset: 0x00041FE4
	[DbfField("ALT_DESCRIPTION", "if not-null, this description is used when the condition specified by ALT_TEXT_PREDICATE evaluates to true.")]
	public DbfLocValue AltDescription
	{
		get
		{
			return this.m_AltDescription;
		}
	}

	// Token: 0x06000FA0 RID: 4000 RVA: 0x00043DEC File Offset: 0x00041FEC
	public void SetNoteDesc(string v)
	{
		this.m_NoteDesc = v;
	}

	// Token: 0x06000FA1 RID: 4001 RVA: 0x00043DF5 File Offset: 0x00041FF5
	public void SetAchType(string v)
	{
		this.m_AchType = v;
	}

	// Token: 0x06000FA2 RID: 4002 RVA: 0x00043DFE File Offset: 0x00041FFE
	public void SetEnabled(bool v)
	{
		this.m_Enabled = v;
	}

	// Token: 0x06000FA3 RID: 4003 RVA: 0x00043E07 File Offset: 0x00042007
	public void SetParentAch(string v)
	{
		this.m_ParentAch = v;
	}

	// Token: 0x06000FA4 RID: 4004 RVA: 0x00043E10 File Offset: 0x00042010
	public void SetClientFlags(int v)
	{
		this.m_ClientFlags = v;
	}

	// Token: 0x06000FA5 RID: 4005 RVA: 0x00043E19 File Offset: 0x00042019
	public void SetTriggered(string v)
	{
		this.m_Triggered = v;
	}

	// Token: 0x06000FA6 RID: 4006 RVA: 0x00043E22 File Offset: 0x00042022
	public void SetAchQuota(int v)
	{
		this.m_AchQuota = v;
	}

	// Token: 0x06000FA7 RID: 4007 RVA: 0x00043E2B File Offset: 0x0004202B
	public void SetEvent(string v)
	{
		this.m_Event = v;
	}

	// Token: 0x06000FA8 RID: 4008 RVA: 0x00043E34 File Offset: 0x00042034
	public void SetRace(int v)
	{
		this.m_Race = v;
	}

	// Token: 0x06000FA9 RID: 4009 RVA: 0x00043E3D File Offset: 0x0004203D
	public void SetCardSet(int v)
	{
		this.m_CardSet = v;
	}

	// Token: 0x06000FAA RID: 4010 RVA: 0x00043E46 File Offset: 0x00042046
	public void SetBooster(int v)
	{
		this.m_Booster = v;
	}

	// Token: 0x06000FAB RID: 4011 RVA: 0x00043E4F File Offset: 0x0004204F
	public void SetRewardTiming(string v)
	{
		this.m_RewardTiming = v;
	}

	// Token: 0x06000FAC RID: 4012 RVA: 0x00043E58 File Offset: 0x00042058
	public void SetReward(string v)
	{
		this.m_Reward = v;
	}

	// Token: 0x06000FAD RID: 4013 RVA: 0x00043E61 File Offset: 0x00042061
	public void SetRewardData1(long v)
	{
		this.m_RewardData1 = v;
	}

	// Token: 0x06000FAE RID: 4014 RVA: 0x00043E6A File Offset: 0x0004206A
	public void SetRewardData2(long v)
	{
		this.m_RewardData2 = v;
	}

	// Token: 0x06000FAF RID: 4015 RVA: 0x00043E73 File Offset: 0x00042073
	public void SetUnlocks(string v)
	{
		this.m_Unlocks = v;
	}

	// Token: 0x06000FB0 RID: 4016 RVA: 0x00043E7C File Offset: 0x0004207C
	public void SetName(DbfLocValue v)
	{
		this.m_Name = v;
		v.SetDebugInfo(base.ID, "NAME");
	}

	// Token: 0x06000FB1 RID: 4017 RVA: 0x00043E96 File Offset: 0x00042096
	public void SetDescription(DbfLocValue v)
	{
		this.m_Description = v;
		v.SetDebugInfo(base.ID, "DESCRIPTION");
	}

	// Token: 0x06000FB2 RID: 4018 RVA: 0x00043EB0 File Offset: 0x000420B0
	public void SetAltTextPredicate(string v)
	{
		this.m_AltTextPredicate = v;
	}

	// Token: 0x06000FB3 RID: 4019 RVA: 0x00043EB9 File Offset: 0x000420B9
	public void SetAltName(DbfLocValue v)
	{
		this.m_AltName = v;
		v.SetDebugInfo(base.ID, "ALT_NAME");
	}

	// Token: 0x06000FB4 RID: 4020 RVA: 0x00043ED3 File Offset: 0x000420D3
	public void SetAltDescription(DbfLocValue v)
	{
		this.m_AltDescription = v;
		v.SetDebugInfo(base.ID, "ALT_DESCRIPTION");
	}

	// Token: 0x06000FB5 RID: 4021 RVA: 0x00043EF0 File Offset: 0x000420F0
	public override object GetVar(string name)
	{
		if (name != null)
		{
			if (AchieveDbfRecord.<>f__switch$map1 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(22);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("ACH_TYPE", 2);
				dictionary.Add("ENABLED", 3);
				dictionary.Add("PARENT_ACH", 4);
				dictionary.Add("CLIENT_FLAGS", 5);
				dictionary.Add("TRIGGERED", 6);
				dictionary.Add("ACH_QUOTA", 7);
				dictionary.Add("EVENT", 8);
				dictionary.Add("RACE", 9);
				dictionary.Add("CARD_SET", 10);
				dictionary.Add("BOOSTER", 11);
				dictionary.Add("REWARD_TIMING", 12);
				dictionary.Add("REWARD", 13);
				dictionary.Add("REWARD_DATA1", 14);
				dictionary.Add("REWARD_DATA2", 15);
				dictionary.Add("UNLOCKS", 16);
				dictionary.Add("NAME", 17);
				dictionary.Add("DESCRIPTION", 18);
				dictionary.Add("ALT_TEXT_PREDICATE", 19);
				dictionary.Add("ALT_NAME", 20);
				dictionary.Add("ALT_DESCRIPTION", 21);
				AchieveDbfRecord.<>f__switch$map1 = dictionary;
			}
			int num;
			if (AchieveDbfRecord.<>f__switch$map1.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return base.ID;
				case 1:
					return this.NoteDesc;
				case 2:
					return this.AchType;
				case 3:
					return this.Enabled;
				case 4:
					return this.ParentAch;
				case 5:
					return this.ClientFlags;
				case 6:
					return this.Triggered;
				case 7:
					return this.AchQuota;
				case 8:
					return this.Event;
				case 9:
					return this.Race;
				case 10:
					return this.CardSet;
				case 11:
					return this.Booster;
				case 12:
					return this.RewardTiming;
				case 13:
					return this.Reward;
				case 14:
					return this.RewardData1;
				case 15:
					return this.RewardData2;
				case 16:
					return this.Unlocks;
				case 17:
					return this.Name;
				case 18:
					return this.Description;
				case 19:
					return this.AltTextPredicate;
				case 20:
					return this.AltName;
				case 21:
					return this.AltDescription;
				}
			}
		}
		return null;
	}

	// Token: 0x06000FB6 RID: 4022 RVA: 0x00044170 File Offset: 0x00042370
	public override void SetVar(string name, object val)
	{
		if (name != null)
		{
			if (AchieveDbfRecord.<>f__switch$map2 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(22);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("ACH_TYPE", 2);
				dictionary.Add("ENABLED", 3);
				dictionary.Add("PARENT_ACH", 4);
				dictionary.Add("CLIENT_FLAGS", 5);
				dictionary.Add("TRIGGERED", 6);
				dictionary.Add("ACH_QUOTA", 7);
				dictionary.Add("EVENT", 8);
				dictionary.Add("RACE", 9);
				dictionary.Add("CARD_SET", 10);
				dictionary.Add("BOOSTER", 11);
				dictionary.Add("REWARD_TIMING", 12);
				dictionary.Add("REWARD", 13);
				dictionary.Add("REWARD_DATA1", 14);
				dictionary.Add("REWARD_DATA2", 15);
				dictionary.Add("UNLOCKS", 16);
				dictionary.Add("NAME", 17);
				dictionary.Add("DESCRIPTION", 18);
				dictionary.Add("ALT_TEXT_PREDICATE", 19);
				dictionary.Add("ALT_NAME", 20);
				dictionary.Add("ALT_DESCRIPTION", 21);
				AchieveDbfRecord.<>f__switch$map2 = dictionary;
			}
			int num;
			if (AchieveDbfRecord.<>f__switch$map2.TryGetValue(name, ref num))
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
					this.SetAchType((string)val);
					break;
				case 3:
					this.SetEnabled((bool)val);
					break;
				case 4:
					this.SetParentAch((string)val);
					break;
				case 5:
					this.SetClientFlags((int)val);
					break;
				case 6:
					this.SetTriggered((string)val);
					break;
				case 7:
					this.SetAchQuota((int)val);
					break;
				case 8:
					this.SetEvent((string)val);
					break;
				case 9:
					this.SetRace((int)val);
					break;
				case 10:
					this.SetCardSet((int)val);
					break;
				case 11:
					this.SetBooster((int)val);
					break;
				case 12:
					this.SetRewardTiming((string)val);
					break;
				case 13:
					this.SetReward((string)val);
					break;
				case 14:
					this.SetRewardData1((long)val);
					break;
				case 15:
					this.SetRewardData2((long)val);
					break;
				case 16:
					this.SetUnlocks((string)val);
					break;
				case 17:
					this.SetName((DbfLocValue)val);
					break;
				case 18:
					this.SetDescription((DbfLocValue)val);
					break;
				case 19:
					this.SetAltTextPredicate((string)val);
					break;
				case 20:
					this.SetAltName((DbfLocValue)val);
					break;
				case 21:
					this.SetAltDescription((DbfLocValue)val);
					break;
				}
			}
		}
	}

	// Token: 0x06000FB7 RID: 4023 RVA: 0x000444A0 File Offset: 0x000426A0
	public override Type GetVarType(string name)
	{
		if (name != null)
		{
			if (AchieveDbfRecord.<>f__switch$map3 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(22);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("ACH_TYPE", 2);
				dictionary.Add("ENABLED", 3);
				dictionary.Add("PARENT_ACH", 4);
				dictionary.Add("CLIENT_FLAGS", 5);
				dictionary.Add("TRIGGERED", 6);
				dictionary.Add("ACH_QUOTA", 7);
				dictionary.Add("EVENT", 8);
				dictionary.Add("RACE", 9);
				dictionary.Add("CARD_SET", 10);
				dictionary.Add("BOOSTER", 11);
				dictionary.Add("REWARD_TIMING", 12);
				dictionary.Add("REWARD", 13);
				dictionary.Add("REWARD_DATA1", 14);
				dictionary.Add("REWARD_DATA2", 15);
				dictionary.Add("UNLOCKS", 16);
				dictionary.Add("NAME", 17);
				dictionary.Add("DESCRIPTION", 18);
				dictionary.Add("ALT_TEXT_PREDICATE", 19);
				dictionary.Add("ALT_NAME", 20);
				dictionary.Add("ALT_DESCRIPTION", 21);
				AchieveDbfRecord.<>f__switch$map3 = dictionary;
			}
			int num;
			if (AchieveDbfRecord.<>f__switch$map3.TryGetValue(name, ref num))
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
					return typeof(bool);
				case 4:
					return typeof(string);
				case 5:
					return typeof(int);
				case 6:
					return typeof(string);
				case 7:
					return typeof(int);
				case 8:
					return typeof(string);
				case 9:
					return typeof(int);
				case 10:
					return typeof(int);
				case 11:
					return typeof(int);
				case 12:
					return typeof(string);
				case 13:
					return typeof(string);
				case 14:
					return typeof(long);
				case 15:
					return typeof(long);
				case 16:
					return typeof(string);
				case 17:
					return typeof(DbfLocValue);
				case 18:
					return typeof(DbfLocValue);
				case 19:
					return typeof(string);
				case 20:
					return typeof(DbfLocValue);
				case 21:
					return typeof(DbfLocValue);
				}
			}
		}
		return null;
	}

	// Token: 0x04000840 RID: 2112
	private string m_NoteDesc;

	// Token: 0x04000841 RID: 2113
	private string m_AchType;

	// Token: 0x04000842 RID: 2114
	private bool m_Enabled;

	// Token: 0x04000843 RID: 2115
	private string m_ParentAch;

	// Token: 0x04000844 RID: 2116
	private int m_ClientFlags;

	// Token: 0x04000845 RID: 2117
	private string m_Triggered;

	// Token: 0x04000846 RID: 2118
	private int m_AchQuota;

	// Token: 0x04000847 RID: 2119
	private string m_Event;

	// Token: 0x04000848 RID: 2120
	private int m_Race;

	// Token: 0x04000849 RID: 2121
	private int m_CardSet;

	// Token: 0x0400084A RID: 2122
	private int m_Booster;

	// Token: 0x0400084B RID: 2123
	private string m_RewardTiming;

	// Token: 0x0400084C RID: 2124
	private string m_Reward;

	// Token: 0x0400084D RID: 2125
	private long m_RewardData1;

	// Token: 0x0400084E RID: 2126
	private long m_RewardData2;

	// Token: 0x0400084F RID: 2127
	private string m_Unlocks;

	// Token: 0x04000850 RID: 2128
	private DbfLocValue m_Name;

	// Token: 0x04000851 RID: 2129
	private DbfLocValue m_Description;

	// Token: 0x04000852 RID: 2130
	private string m_AltTextPredicate;

	// Token: 0x04000853 RID: 2131
	private DbfLocValue m_AltName;

	// Token: 0x04000854 RID: 2132
	private DbfLocValue m_AltDescription;
}
