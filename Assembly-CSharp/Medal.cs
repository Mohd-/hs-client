using System;
using UnityEngine;

// Token: 0x02000B06 RID: 2822
public class Medal
{
	// Token: 0x060060AC RID: 24748 RVA: 0x001CF02C File Offset: 0x001CD22C
	public Medal(Medal.Type medalType) : this(medalType, 0)
	{
	}

	// Token: 0x060060AD RID: 24749 RVA: 0x001CF036 File Offset: 0x001CD236
	public Medal(int grandMasterRank) : this(Medal.GRAND_MASTER_MEDAL_TYPE, grandMasterRank)
	{
	}

	// Token: 0x060060AE RID: 24750 RVA: 0x001CF044 File Offset: 0x001CD244
	private Medal(Medal.Type medalType, int grandMasterRank)
	{
		this.m_medalType = medalType;
		this.m_grandMasterRank = grandMasterRank;
	}

	// Token: 0x17000904 RID: 2308
	// (get) Token: 0x060060B0 RID: 24752 RVA: 0x001CF184 File Offset: 0x001CD384
	public Medal.Type MedalType
	{
		get
		{
			return this.m_medalType;
		}
	}

	// Token: 0x17000905 RID: 2309
	// (get) Token: 0x060060B1 RID: 24753 RVA: 0x001CF18C File Offset: 0x001CD38C
	public int GrandMasterRank
	{
		get
		{
			return this.m_grandMasterRank;
		}
	}

	// Token: 0x060060B2 RID: 24754 RVA: 0x001CF194 File Offset: 0x001CD394
	public bool IsGrandMaster()
	{
		return Medal.GRAND_MASTER_MEDAL_TYPE == this.MedalType && this.GrandMasterRank > 0;
	}

	// Token: 0x060060B3 RID: 24755 RVA: 0x001CF1B4 File Offset: 0x001CD3B4
	public int GetNumStars()
	{
		if (this.IsGrandMaster())
		{
			return 0;
		}
		switch (this.MedalType)
		{
		case Medal.Type.MEDAL_NOVICE:
		case Medal.Type.MEDAL_JOURNEYMAN:
		case Medal.Type.MEDAL_APPRENTICE:
			return 0;
		case Medal.Type.MEDAL_COPPER_A:
		case Medal.Type.MEDAL_SILVER_A:
		case Medal.Type.MEDAL_GOLD_A:
		case Medal.Type.MEDAL_PLATINUM_A:
		case Medal.Type.MEDAL_DIAMOND_A:
		case Medal.Type.MEDAL_MASTER_A:
			return 1;
		case Medal.Type.MEDAL_COPPER_B:
		case Medal.Type.MEDAL_SILVER_B:
		case Medal.Type.MEDAL_GOLD_B:
		case Medal.Type.MEDAL_PLATINUM_B:
		case Medal.Type.MEDAL_DIAMOND_B:
		case Medal.Type.MEDAL_MASTER_B:
			return 2;
		case Medal.Type.MEDAL_COPPER_C:
		case Medal.Type.MEDAL_SILVER_C:
		case Medal.Type.MEDAL_GOLD_C:
		case Medal.Type.MEDAL_PLATINUM_C:
		case Medal.Type.MEDAL_DIAMOND_C:
		case Medal.Type.MEDAL_MASTER_C:
			return 3;
		default:
			Debug.LogWarning(string.Format("Medal.GetNumStars(): Don't know how many stars are in medal {0}", this));
			return 0;
		}
	}

	// Token: 0x060060B4 RID: 24756 RVA: 0x001CF250 File Offset: 0x001CD450
	public string GetMedalName()
	{
		if (this.IsGrandMaster())
		{
			return GameStrings.Get("GLOBAL_MEDAL_GRANDMASTER");
		}
		string baseMedalName = this.GetBaseMedalName();
		string starString = this.GetStarString();
		if (string.IsNullOrEmpty(starString))
		{
			return baseMedalName;
		}
		return GameStrings.Format("GLOBAL_MEDAL_STARRED_FORMAT", new object[]
		{
			starString,
			baseMedalName
		});
	}

	// Token: 0x060060B5 RID: 24757 RVA: 0x001CF2A8 File Offset: 0x001CD4A8
	public string GetBaseMedalName()
	{
		if (this.IsGrandMaster())
		{
			return GameStrings.Get("GLOBAL_MEDAL_GRANDMASTER");
		}
		string result = string.Empty;
		if (Medal.s_medalNameKey.ContainsKey(this.MedalType))
		{
			result = GameStrings.Get(Medal.s_medalNameKey[this.MedalType]);
		}
		else
		{
			Debug.LogWarning(string.Format("Medal.GetMedalName(): don't have a medal name key for {0}", this));
			result = GameStrings.Get("GLOBAL_MEDAL_NO_NAME");
		}
		return result;
	}

	// Token: 0x060060B6 RID: 24758 RVA: 0x001CF320 File Offset: 0x001CD520
	public string GetNextMedalName()
	{
		Medal nextMedal = this.GetNextMedal();
		if (nextMedal == null)
		{
			return string.Empty;
		}
		return nextMedal.GetMedalName();
	}

	// Token: 0x060060B7 RID: 24759 RVA: 0x001CF346 File Offset: 0x001CD546
	public override string ToString()
	{
		return string.Format("[Medal: MedalType={0}, GrandMasterRank={1}]", this.MedalType, this.GrandMasterRank);
	}

	// Token: 0x060060B8 RID: 24760 RVA: 0x001CF368 File Offset: 0x001CD568
	private Medal GetNextMedal()
	{
		if (this.IsGrandMaster())
		{
			return null;
		}
		if (this.MedalType == Medal.GRAND_MASTER_MEDAL_TYPE)
		{
			return new Medal(1);
		}
		int medalType = (int)this.MedalType;
		int medalType2 = medalType + 1;
		return new Medal((Medal.Type)medalType2);
	}

	// Token: 0x060060B9 RID: 24761 RVA: 0x001CF3AC File Offset: 0x001CD5AC
	private string GetStarString()
	{
		int numStars = this.GetNumStars();
		switch (numStars)
		{
		case 0:
			return string.Empty;
		case 1:
			return GameStrings.Get("GLOBAL_MEDAL_ONE_STAR");
		case 2:
			return GameStrings.Get("GLOBAL_MEDAL_TWO_STAR");
		case 3:
			return GameStrings.Get("GLOBAL_MEDAL_THREE_STAR");
		default:
			Debug.LogWarning(string.Format("Medal.GetStarString(): don't have a star string for {0} stars", numStars));
			return string.Empty;
		}
	}

	// Token: 0x04004846 RID: 18502
	private static readonly Medal.Type GRAND_MASTER_MEDAL_TYPE = Medal.Type.MEDAL_MASTER_C;

	// Token: 0x04004847 RID: 18503
	private Medal.Type m_medalType;

	// Token: 0x04004848 RID: 18504
	private int m_grandMasterRank;

	// Token: 0x04004849 RID: 18505
	private static readonly Map<Medal.Type, string> s_medalNameKey = new Map<Medal.Type, string>
	{
		{
			Medal.Type.MEDAL_NOVICE,
			"GLOBAL_MEDAL_NOVICE"
		},
		{
			Medal.Type.MEDAL_APPRENTICE,
			"GLOBAL_MEDAL_APPRENTICE"
		},
		{
			Medal.Type.MEDAL_JOURNEYMAN,
			"GLOBAL_MEDAL_JOURNEYMAN"
		},
		{
			Medal.Type.MEDAL_COPPER_A,
			"GLOBAL_MEDAL_COPPER"
		},
		{
			Medal.Type.MEDAL_COPPER_B,
			"GLOBAL_MEDAL_COPPER"
		},
		{
			Medal.Type.MEDAL_COPPER_C,
			"GLOBAL_MEDAL_COPPER"
		},
		{
			Medal.Type.MEDAL_SILVER_A,
			"GLOBAL_MEDAL_SILVER"
		},
		{
			Medal.Type.MEDAL_SILVER_B,
			"GLOBAL_MEDAL_SILVER"
		},
		{
			Medal.Type.MEDAL_SILVER_C,
			"GLOBAL_MEDAL_SILVER"
		},
		{
			Medal.Type.MEDAL_GOLD_A,
			"GLOBAL_MEDAL_GOLD"
		},
		{
			Medal.Type.MEDAL_GOLD_B,
			"GLOBAL_MEDAL_GOLD"
		},
		{
			Medal.Type.MEDAL_GOLD_C,
			"GLOBAL_MEDAL_GOLD"
		},
		{
			Medal.Type.MEDAL_PLATINUM_A,
			"GLOBAL_MEDAL_PLATINUM"
		},
		{
			Medal.Type.MEDAL_PLATINUM_B,
			"GLOBAL_MEDAL_PLATINUM"
		},
		{
			Medal.Type.MEDAL_PLATINUM_C,
			"GLOBAL_MEDAL_PLATINUM"
		},
		{
			Medal.Type.MEDAL_DIAMOND_A,
			"GLOBAL_MEDAL_DIAMOND"
		},
		{
			Medal.Type.MEDAL_DIAMOND_B,
			"GLOBAL_MEDAL_DIAMOND"
		},
		{
			Medal.Type.MEDAL_DIAMOND_C,
			"GLOBAL_MEDAL_DIAMOND"
		},
		{
			Medal.Type.MEDAL_MASTER_A,
			"GLOBAL_MEDAL_MASTER"
		},
		{
			Medal.Type.MEDAL_MASTER_B,
			"GLOBAL_MEDAL_MASTER"
		},
		{
			Medal.Type.MEDAL_MASTER_C,
			"GLOBAL_MEDAL_MASTER"
		}
	};

	// Token: 0x02000B07 RID: 2823
	public enum Type
	{
		// Token: 0x0400484B RID: 18507
		MEDAL_NOVICE,
		// Token: 0x0400484C RID: 18508
		MEDAL_JOURNEYMAN,
		// Token: 0x0400484D RID: 18509
		MEDAL_COPPER_A,
		// Token: 0x0400484E RID: 18510
		MEDAL_COPPER_B,
		// Token: 0x0400484F RID: 18511
		MEDAL_COPPER_C,
		// Token: 0x04004850 RID: 18512
		MEDAL_SILVER_A,
		// Token: 0x04004851 RID: 18513
		MEDAL_SILVER_B,
		// Token: 0x04004852 RID: 18514
		MEDAL_SILVER_C,
		// Token: 0x04004853 RID: 18515
		MEDAL_GOLD_A,
		// Token: 0x04004854 RID: 18516
		MEDAL_GOLD_B,
		// Token: 0x04004855 RID: 18517
		MEDAL_GOLD_C,
		// Token: 0x04004856 RID: 18518
		MEDAL_PLATINUM_A,
		// Token: 0x04004857 RID: 18519
		MEDAL_PLATINUM_B,
		// Token: 0x04004858 RID: 18520
		MEDAL_PLATINUM_C,
		// Token: 0x04004859 RID: 18521
		MEDAL_DIAMOND_A,
		// Token: 0x0400485A RID: 18522
		MEDAL_DIAMOND_B,
		// Token: 0x0400485B RID: 18523
		MEDAL_DIAMOND_C,
		// Token: 0x0400485C RID: 18524
		MEDAL_MASTER_A,
		// Token: 0x0400485D RID: 18525
		MEDAL_MASTER_B,
		// Token: 0x0400485E RID: 18526
		MEDAL_MASTER_C,
		// Token: 0x0400485F RID: 18527
		MEDAL_APPRENTICE
	}
}
