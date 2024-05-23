using System;
using System.Collections.Generic;

// Token: 0x02000144 RID: 324
public class FixedRewardMapDbfRecord : DbfRecord
{
	// Token: 0x170002CE RID: 718
	// (get) Token: 0x060010E7 RID: 4327 RVA: 0x00048D3E File Offset: 0x00046F3E
	[DbfField("ACTION_ID", "specifies which FIXED_REWARD_ACTION.ID (conditions) triggers/grants what Reward.")]
	public int ActionId
	{
		get
		{
			return this.m_ActionId;
		}
	}

	// Token: 0x170002CF RID: 719
	// (get) Token: 0x060010E8 RID: 4328 RVA: 0x00048D46 File Offset: 0x00046F46
	[DbfField("REWARD_ID", "specifies what FIXED_REWARD.ID is granted by this action.")]
	public int RewardId
	{
		get
		{
			return this.m_RewardId;
		}
	}

	// Token: 0x170002D0 RID: 720
	// (get) Token: 0x060010E9 RID: 4329 RVA: 0x00048D4E File Offset: 0x00046F4E
	[DbfField("REWARD_COUNT", "how many copies of this reward are granted by this action")]
	public int RewardCount
	{
		get
		{
			return this.m_RewardCount;
		}
	}

	// Token: 0x170002D1 RID: 721
	// (get) Token: 0x060010EA RID: 4330 RVA: 0x00048D56 File Offset: 0x00046F56
	[DbfField("NOTE_DESC", "designer note")]
	public string NoteDesc
	{
		get
		{
			return this.m_NoteDesc;
		}
	}

	// Token: 0x170002D2 RID: 722
	// (get) Token: 0x060010EB RID: 4331 RVA: 0x00048D5E File Offset: 0x00046F5E
	[DbfField("USE_QUEST_TOAST", "used for client visuals")]
	public bool UseQuestToast
	{
		get
		{
			return this.m_UseQuestToast;
		}
	}

	// Token: 0x170002D3 RID: 723
	// (get) Token: 0x060010EC RID: 4332 RVA: 0x00048D66 File Offset: 0x00046F66
	[DbfField("REWARD_TIMING", "when should the client show the visual associated with this fixed reward?")]
	public string RewardTiming
	{
		get
		{
			return this.m_RewardTiming;
		}
	}

	// Token: 0x170002D4 RID: 724
	// (get) Token: 0x060010ED RID: 4333 RVA: 0x00048D6E File Offset: 0x00046F6E
	[DbfField("TOAST_NAME", "")]
	public DbfLocValue ToastName
	{
		get
		{
			return this.m_ToastName;
		}
	}

	// Token: 0x170002D5 RID: 725
	// (get) Token: 0x060010EE RID: 4334 RVA: 0x00048D76 File Offset: 0x00046F76
	[DbfField("TOAST_DESCRIPTION", "")]
	public DbfLocValue ToastDescription
	{
		get
		{
			return this.m_ToastDescription;
		}
	}

	// Token: 0x170002D6 RID: 726
	// (get) Token: 0x060010EF RID: 4335 RVA: 0x00048D7E File Offset: 0x00046F7E
	[DbfField("SORT_ORDER", "the order rewards will show up to players when client displays more than 1 reward at a time. this has a UNIQUE key because it is theoretically possible to get many rewards show up at once and a definitive order needs to be in place.")]
	public int SortOrder
	{
		get
		{
			return this.m_SortOrder;
		}
	}

	// Token: 0x060010F0 RID: 4336 RVA: 0x00048D86 File Offset: 0x00046F86
	public void SetActionId(int v)
	{
		this.m_ActionId = v;
	}

	// Token: 0x060010F1 RID: 4337 RVA: 0x00048D8F File Offset: 0x00046F8F
	public void SetRewardId(int v)
	{
		this.m_RewardId = v;
	}

	// Token: 0x060010F2 RID: 4338 RVA: 0x00048D98 File Offset: 0x00046F98
	public void SetRewardCount(int v)
	{
		this.m_RewardCount = v;
	}

	// Token: 0x060010F3 RID: 4339 RVA: 0x00048DA1 File Offset: 0x00046FA1
	public void SetNoteDesc(string v)
	{
		this.m_NoteDesc = v;
	}

	// Token: 0x060010F4 RID: 4340 RVA: 0x00048DAA File Offset: 0x00046FAA
	public void SetUseQuestToast(bool v)
	{
		this.m_UseQuestToast = v;
	}

	// Token: 0x060010F5 RID: 4341 RVA: 0x00048DB3 File Offset: 0x00046FB3
	public void SetRewardTiming(string v)
	{
		this.m_RewardTiming = v;
	}

	// Token: 0x060010F6 RID: 4342 RVA: 0x00048DBC File Offset: 0x00046FBC
	public void SetToastName(DbfLocValue v)
	{
		this.m_ToastName = v;
		v.SetDebugInfo(base.ID, "TOAST_NAME");
	}

	// Token: 0x060010F7 RID: 4343 RVA: 0x00048DD6 File Offset: 0x00046FD6
	public void SetToastDescription(DbfLocValue v)
	{
		this.m_ToastDescription = v;
		v.SetDebugInfo(base.ID, "TOAST_DESCRIPTION");
	}

	// Token: 0x060010F8 RID: 4344 RVA: 0x00048DF0 File Offset: 0x00046FF0
	public void SetSortOrder(int v)
	{
		this.m_SortOrder = v;
	}

	// Token: 0x060010F9 RID: 4345 RVA: 0x00048DFC File Offset: 0x00046FFC
	public override object GetVar(string name)
	{
		if (name != null)
		{
			if (FixedRewardMapDbfRecord.<>f__switch$map37 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(10);
				dictionary.Add("ID", 0);
				dictionary.Add("ACTION_ID", 1);
				dictionary.Add("REWARD_ID", 2);
				dictionary.Add("REWARD_COUNT", 3);
				dictionary.Add("NOTE_DESC", 4);
				dictionary.Add("USE_QUEST_TOAST", 5);
				dictionary.Add("REWARD_TIMING", 6);
				dictionary.Add("TOAST_NAME", 7);
				dictionary.Add("TOAST_DESCRIPTION", 8);
				dictionary.Add("SORT_ORDER", 9);
				FixedRewardMapDbfRecord.<>f__switch$map37 = dictionary;
			}
			int num;
			if (FixedRewardMapDbfRecord.<>f__switch$map37.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return base.ID;
				case 1:
					return this.ActionId;
				case 2:
					return this.RewardId;
				case 3:
					return this.RewardCount;
				case 4:
					return this.NoteDesc;
				case 5:
					return this.UseQuestToast;
				case 6:
					return this.RewardTiming;
				case 7:
					return this.ToastName;
				case 8:
					return this.ToastDescription;
				case 9:
					return this.SortOrder;
				}
			}
		}
		return null;
	}

	// Token: 0x060010FA RID: 4346 RVA: 0x00048F4C File Offset: 0x0004714C
	public override void SetVar(string name, object val)
	{
		if (name != null)
		{
			if (FixedRewardMapDbfRecord.<>f__switch$map38 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(10);
				dictionary.Add("ID", 0);
				dictionary.Add("ACTION_ID", 1);
				dictionary.Add("REWARD_ID", 2);
				dictionary.Add("REWARD_COUNT", 3);
				dictionary.Add("NOTE_DESC", 4);
				dictionary.Add("USE_QUEST_TOAST", 5);
				dictionary.Add("REWARD_TIMING", 6);
				dictionary.Add("TOAST_NAME", 7);
				dictionary.Add("TOAST_DESCRIPTION", 8);
				dictionary.Add("SORT_ORDER", 9);
				FixedRewardMapDbfRecord.<>f__switch$map38 = dictionary;
			}
			int num;
			if (FixedRewardMapDbfRecord.<>f__switch$map38.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					base.SetID((int)val);
					break;
				case 1:
					this.SetActionId((int)val);
					break;
				case 2:
					this.SetRewardId((int)val);
					break;
				case 3:
					this.SetRewardCount((int)val);
					break;
				case 4:
					this.SetNoteDesc((string)val);
					break;
				case 5:
					this.SetUseQuestToast((bool)val);
					break;
				case 6:
					this.SetRewardTiming((string)val);
					break;
				case 7:
					this.SetToastName((DbfLocValue)val);
					break;
				case 8:
					this.SetToastDescription((DbfLocValue)val);
					break;
				case 9:
					this.SetSortOrder((int)val);
					break;
				}
			}
		}
	}

	// Token: 0x060010FB RID: 4347 RVA: 0x000490E4 File Offset: 0x000472E4
	public override Type GetVarType(string name)
	{
		if (name != null)
		{
			if (FixedRewardMapDbfRecord.<>f__switch$map39 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(10);
				dictionary.Add("ID", 0);
				dictionary.Add("ACTION_ID", 1);
				dictionary.Add("REWARD_ID", 2);
				dictionary.Add("REWARD_COUNT", 3);
				dictionary.Add("NOTE_DESC", 4);
				dictionary.Add("USE_QUEST_TOAST", 5);
				dictionary.Add("REWARD_TIMING", 6);
				dictionary.Add("TOAST_NAME", 7);
				dictionary.Add("TOAST_DESCRIPTION", 8);
				dictionary.Add("SORT_ORDER", 9);
				FixedRewardMapDbfRecord.<>f__switch$map39 = dictionary;
			}
			int num;
			if (FixedRewardMapDbfRecord.<>f__switch$map39.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return typeof(int);
				case 1:
					return typeof(int);
				case 2:
					return typeof(int);
				case 3:
					return typeof(int);
				case 4:
					return typeof(string);
				case 5:
					return typeof(bool);
				case 6:
					return typeof(string);
				case 7:
					return typeof(DbfLocValue);
				case 8:
					return typeof(DbfLocValue);
				case 9:
					return typeof(int);
				}
			}
		}
		return null;
	}

	// Token: 0x040008FF RID: 2303
	private int m_ActionId;

	// Token: 0x04000900 RID: 2304
	private int m_RewardId;

	// Token: 0x04000901 RID: 2305
	private int m_RewardCount;

	// Token: 0x04000902 RID: 2306
	private string m_NoteDesc;

	// Token: 0x04000903 RID: 2307
	private bool m_UseQuestToast;

	// Token: 0x04000904 RID: 2308
	private string m_RewardTiming;

	// Token: 0x04000905 RID: 2309
	private DbfLocValue m_ToastName;

	// Token: 0x04000906 RID: 2310
	private DbfLocValue m_ToastDescription;

	// Token: 0x04000907 RID: 2311
	private int m_SortOrder;
}
