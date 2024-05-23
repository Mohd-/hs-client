using System;
using System.Collections.Generic;

// Token: 0x02000029 RID: 41
public class AdventureMissionDbfRecord : DbfRecord
{
	// Token: 0x1700004B RID: 75
	// (get) Token: 0x060003CF RID: 975 RVA: 0x00011B49 File Offset: 0x0000FD49
	[DbfField("SCENARIO_ID", "primary key - SCENARIO.ID that this mission corresponds to")]
	public int ScenarioId
	{
		get
		{
			return this.m_ScenarioId;
		}
	}

	// Token: 0x1700004C RID: 76
	// (get) Token: 0x060003D0 RID: 976 RVA: 0x00011B51 File Offset: 0x0000FD51
	[DbfField("NOTE_DESC", "designer note")]
	public string NoteDesc
	{
		get
		{
			return this.m_NoteDesc;
		}
	}

	// Token: 0x1700004D RID: 77
	// (get) Token: 0x060003D1 RID: 977 RVA: 0x00011B59 File Offset: 0x0000FD59
	[DbfField("REQ_WING_ID", "adventure WING.ID requirement to play this mission. 0 is ignored.")]
	public int ReqWingId
	{
		get
		{
			return this.m_ReqWingId;
		}
	}

	// Token: 0x1700004E RID: 78
	// (get) Token: 0x060003D2 RID: 978 RVA: 0x00011B61 File Offset: 0x0000FD61
	[DbfField("REQ_PROGRESS", "player must have this wing_progress on the REQ_WING_ID in order to play this mission. 0 is ignored.")]
	public int ReqProgress
	{
		get
		{
			return this.m_ReqProgress;
		}
	}

	// Token: 0x1700004F RID: 79
	// (get) Token: 0x060003D3 RID: 979 RVA: 0x00011B69 File Offset: 0x0000FD69
	[DbfField("REQ_FLAGS", "player must have this wing_flags on the REQ_WING_ID in order to play this mission. 0 is ignored.")]
	public ulong ReqFlags
	{
		get
		{
			return this.m_ReqFlags;
		}
	}

	// Token: 0x17000050 RID: 80
	// (get) Token: 0x060003D4 RID: 980 RVA: 0x00011B71 File Offset: 0x0000FD71
	[DbfField("GRANTS_WING_ID", "adventure WING.ID granted by defeating this mission. 0 is ignored.")]
	public int GrantsWingId
	{
		get
		{
			return this.m_GrantsWingId;
		}
	}

	// Token: 0x17000051 RID: 81
	// (get) Token: 0x060003D5 RID: 981 RVA: 0x00011B79 File Offset: 0x0000FD79
	[DbfField("GRANTS_PROGRESS", "wing_progress granted for GRANTS_WING_ID by defeating this mission. 0 is ignored.")]
	public int GrantsProgress
	{
		get
		{
			return this.m_GrantsProgress;
		}
	}

	// Token: 0x17000052 RID: 82
	// (get) Token: 0x060003D6 RID: 982 RVA: 0x00011B81 File Offset: 0x0000FD81
	[DbfField("GRANTS_FLAGS", "wing_flags granted for GRANTS_WING_ID  by defeating this mission. 0 is ignored.")]
	public ulong GrantsFlags
	{
		get
		{
			return this.m_GrantsFlags;
		}
	}

	// Token: 0x17000053 RID: 83
	// (get) Token: 0x060003D7 RID: 983 RVA: 0x00011B89 File Offset: 0x0000FD89
	[DbfField("BOSS_DEF_ASSET_PATH", "")]
	public string BossDefAssetPath
	{
		get
		{
			return this.m_BossDefAssetPath;
		}
	}

	// Token: 0x17000054 RID: 84
	// (get) Token: 0x060003D8 RID: 984 RVA: 0x00011B91 File Offset: 0x0000FD91
	[DbfField("CLASS_CHALLENGE_PREFAB_POPUP", "")]
	public string ClassChallengePrefabPopup
	{
		get
		{
			return this.m_ClassChallengePrefabPopup;
		}
	}

	// Token: 0x060003D9 RID: 985 RVA: 0x00011B99 File Offset: 0x0000FD99
	public void SetScenarioId(int v)
	{
		this.m_ScenarioId = v;
	}

	// Token: 0x060003DA RID: 986 RVA: 0x00011BA2 File Offset: 0x0000FDA2
	public void SetNoteDesc(string v)
	{
		this.m_NoteDesc = v;
	}

	// Token: 0x060003DB RID: 987 RVA: 0x00011BAB File Offset: 0x0000FDAB
	public void SetReqWingId(int v)
	{
		this.m_ReqWingId = v;
	}

	// Token: 0x060003DC RID: 988 RVA: 0x00011BB4 File Offset: 0x0000FDB4
	public void SetReqProgress(int v)
	{
		this.m_ReqProgress = v;
	}

	// Token: 0x060003DD RID: 989 RVA: 0x00011BBD File Offset: 0x0000FDBD
	public void SetReqFlags(ulong v)
	{
		this.m_ReqFlags = v;
	}

	// Token: 0x060003DE RID: 990 RVA: 0x00011BC6 File Offset: 0x0000FDC6
	public void SetGrantsWingId(int v)
	{
		this.m_GrantsWingId = v;
	}

	// Token: 0x060003DF RID: 991 RVA: 0x00011BCF File Offset: 0x0000FDCF
	public void SetGrantsProgress(int v)
	{
		this.m_GrantsProgress = v;
	}

	// Token: 0x060003E0 RID: 992 RVA: 0x00011BD8 File Offset: 0x0000FDD8
	public void SetGrantsFlags(ulong v)
	{
		this.m_GrantsFlags = v;
	}

	// Token: 0x060003E1 RID: 993 RVA: 0x00011BE1 File Offset: 0x0000FDE1
	public void SetBossDefAssetPath(string v)
	{
		this.m_BossDefAssetPath = v;
	}

	// Token: 0x060003E2 RID: 994 RVA: 0x00011BEA File Offset: 0x0000FDEA
	public void SetClassChallengePrefabPopup(string v)
	{
		this.m_ClassChallengePrefabPopup = v;
	}

	// Token: 0x060003E3 RID: 995 RVA: 0x00011BF4 File Offset: 0x0000FDF4
	public override object GetVar(string name)
	{
		if (name != null)
		{
			if (AdventureMissionDbfRecord.<>f__switch$mapA == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(11);
				dictionary.Add("ID", 0);
				dictionary.Add("SCENARIO_ID", 1);
				dictionary.Add("NOTE_DESC", 2);
				dictionary.Add("REQ_WING_ID", 3);
				dictionary.Add("REQ_PROGRESS", 4);
				dictionary.Add("REQ_FLAGS", 5);
				dictionary.Add("GRANTS_WING_ID", 6);
				dictionary.Add("GRANTS_PROGRESS", 7);
				dictionary.Add("GRANTS_FLAGS", 8);
				dictionary.Add("BOSS_DEF_ASSET_PATH", 9);
				dictionary.Add("CLASS_CHALLENGE_PREFAB_POPUP", 10);
				AdventureMissionDbfRecord.<>f__switch$mapA = dictionary;
			}
			int num;
			if (AdventureMissionDbfRecord.<>f__switch$mapA.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return base.ID;
				case 1:
					return this.ScenarioId;
				case 2:
					return this.NoteDesc;
				case 3:
					return this.ReqWingId;
				case 4:
					return this.ReqProgress;
				case 5:
					return this.ReqFlags;
				case 6:
					return this.GrantsWingId;
				case 7:
					return this.GrantsProgress;
				case 8:
					return this.GrantsFlags;
				case 9:
					return this.BossDefAssetPath;
				case 10:
					return this.ClassChallengePrefabPopup;
				}
			}
		}
		return null;
	}

	// Token: 0x060003E4 RID: 996 RVA: 0x00011D68 File Offset: 0x0000FF68
	public override void SetVar(string name, object val)
	{
		if (name != null)
		{
			if (AdventureMissionDbfRecord.<>f__switch$mapB == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(11);
				dictionary.Add("ID", 0);
				dictionary.Add("SCENARIO_ID", 1);
				dictionary.Add("NOTE_DESC", 2);
				dictionary.Add("REQ_WING_ID", 3);
				dictionary.Add("REQ_PROGRESS", 4);
				dictionary.Add("REQ_FLAGS", 5);
				dictionary.Add("GRANTS_WING_ID", 6);
				dictionary.Add("GRANTS_PROGRESS", 7);
				dictionary.Add("GRANTS_FLAGS", 8);
				dictionary.Add("BOSS_DEF_ASSET_PATH", 9);
				dictionary.Add("CLASS_CHALLENGE_PREFAB_POPUP", 10);
				AdventureMissionDbfRecord.<>f__switch$mapB = dictionary;
			}
			int num;
			if (AdventureMissionDbfRecord.<>f__switch$mapB.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					base.SetID((int)val);
					break;
				case 1:
					this.SetScenarioId((int)val);
					break;
				case 2:
					this.SetNoteDesc((string)val);
					break;
				case 3:
					this.SetReqWingId((int)val);
					break;
				case 4:
					this.SetReqProgress((int)val);
					break;
				case 5:
					this.SetReqFlags((ulong)val);
					break;
				case 6:
					this.SetGrantsWingId((int)val);
					break;
				case 7:
					this.SetGrantsProgress((int)val);
					break;
				case 8:
					this.SetGrantsFlags((ulong)val);
					break;
				case 9:
					this.SetBossDefAssetPath((string)val);
					break;
				case 10:
					this.SetClassChallengePrefabPopup((string)val);
					break;
				}
			}
		}
	}

	// Token: 0x060003E5 RID: 997 RVA: 0x00011F20 File Offset: 0x00010120
	public override Type GetVarType(string name)
	{
		if (name != null)
		{
			if (AdventureMissionDbfRecord.<>f__switch$mapC == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(11);
				dictionary.Add("ID", 0);
				dictionary.Add("SCENARIO_ID", 1);
				dictionary.Add("NOTE_DESC", 2);
				dictionary.Add("REQ_WING_ID", 3);
				dictionary.Add("REQ_PROGRESS", 4);
				dictionary.Add("REQ_FLAGS", 5);
				dictionary.Add("GRANTS_WING_ID", 6);
				dictionary.Add("GRANTS_PROGRESS", 7);
				dictionary.Add("GRANTS_FLAGS", 8);
				dictionary.Add("BOSS_DEF_ASSET_PATH", 9);
				dictionary.Add("CLASS_CHALLENGE_PREFAB_POPUP", 10);
				AdventureMissionDbfRecord.<>f__switch$mapC = dictionary;
			}
			int num;
			if (AdventureMissionDbfRecord.<>f__switch$mapC.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return typeof(int);
				case 1:
					return typeof(int);
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
					return typeof(ulong);
				case 9:
					return typeof(string);
				case 10:
					return typeof(string);
				}
			}
		}
		return null;
	}

	// Token: 0x0400016D RID: 365
	private int m_ScenarioId;

	// Token: 0x0400016E RID: 366
	private string m_NoteDesc;

	// Token: 0x0400016F RID: 367
	private int m_ReqWingId;

	// Token: 0x04000170 RID: 368
	private int m_ReqProgress;

	// Token: 0x04000171 RID: 369
	private ulong m_ReqFlags;

	// Token: 0x04000172 RID: 370
	private int m_GrantsWingId;

	// Token: 0x04000173 RID: 371
	private int m_GrantsProgress;

	// Token: 0x04000174 RID: 372
	private ulong m_GrantsFlags;

	// Token: 0x04000175 RID: 373
	private string m_BossDefAssetPath;

	// Token: 0x04000176 RID: 374
	private string m_ClassChallengePrefabPopup;
}
