using System;

// Token: 0x02000003 RID: 3
public class AdventureMission
{
	// Token: 0x06000005 RID: 5 RVA: 0x000028BC File Offset: 0x00000ABC
	public AdventureMission(int scenarioID, string description, AdventureMission.WingProgress requiredProgress, AdventureMission.WingProgress grantedProgress)
	{
		this.m_scenarioID = scenarioID;
		this.m_description = description;
		this.m_requiredProgress = ((!requiredProgress.IsEmpty()) ? requiredProgress : null);
		this.m_grantedProgress = ((!grantedProgress.IsEmpty()) ? grantedProgress : null);
	}

	// Token: 0x17000001 RID: 1
	// (get) Token: 0x06000006 RID: 6 RVA: 0x0000290F File Offset: 0x00000B0F
	public int ScenarioID
	{
		get
		{
			return this.m_scenarioID;
		}
	}

	// Token: 0x17000002 RID: 2
	// (get) Token: 0x06000007 RID: 7 RVA: 0x00002917 File Offset: 0x00000B17
	public string Description
	{
		get
		{
			return this.m_description;
		}
	}

	// Token: 0x17000003 RID: 3
	// (get) Token: 0x06000008 RID: 8 RVA: 0x0000291F File Offset: 0x00000B1F
	public AdventureMission.WingProgress RequiredProgress
	{
		get
		{
			return this.m_requiredProgress;
		}
	}

	// Token: 0x17000004 RID: 4
	// (get) Token: 0x06000009 RID: 9 RVA: 0x00002927 File Offset: 0x00000B27
	public AdventureMission.WingProgress GrantedProgress
	{
		get
		{
			return this.m_grantedProgress;
		}
	}

	// Token: 0x0600000A RID: 10 RVA: 0x0000292F File Offset: 0x00000B2F
	public bool HasRequiredProgress()
	{
		return this.m_requiredProgress != null;
	}

	// Token: 0x0600000B RID: 11 RVA: 0x0000293D File Offset: 0x00000B3D
	public bool HasGrantedProgress()
	{
		return this.m_grantedProgress != null;
	}

	// Token: 0x0600000C RID: 12 RVA: 0x0000294C File Offset: 0x00000B4C
	public override string ToString()
	{
		return string.Format("[AdventureMission: ScenarioID={0}, Description={1} RequiredProgress={2} GrantedProgress={3}]", new object[]
		{
			this.ScenarioID,
			this.Description,
			this.RequiredProgress,
			this.GrantedProgress
		});
	}

	// Token: 0x0400000C RID: 12
	private int m_scenarioID;

	// Token: 0x0400000D RID: 13
	private string m_description;

	// Token: 0x0400000E RID: 14
	private AdventureMission.WingProgress m_requiredProgress;

	// Token: 0x0400000F RID: 15
	private AdventureMission.WingProgress m_grantedProgress;

	// Token: 0x02000004 RID: 4
	public class WingProgress
	{
		// Token: 0x0600000D RID: 13 RVA: 0x00002992 File Offset: 0x00000B92
		public WingProgress(int wing, int progress, ulong flags)
		{
			this.m_wing = wing;
			this.m_progress = progress;
			this.m_flags = flags;
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000E RID: 14 RVA: 0x000029AF File Offset: 0x00000BAF
		public int Wing
		{
			get
			{
				return this.m_wing;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600000F RID: 15 RVA: 0x000029B7 File Offset: 0x00000BB7
		public int Progress
		{
			get
			{
				return this.m_progress;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000010 RID: 16 RVA: 0x000029BF File Offset: 0x00000BBF
		public ulong Flags
		{
			get
			{
				return this.m_flags;
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000029C8 File Offset: 0x00000BC8
		public bool IsEmpty()
		{
			return this.Wing == 0 || (this.Progress <= 0 && this.Flags == 0UL);
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000029FA File Offset: 0x00000BFA
		public bool IsOwned()
		{
			return this.MeetsFlagsRequirement(1UL);
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002A04 File Offset: 0x00000C04
		public bool MeetsProgressRequirement(int requiredProgress)
		{
			return this.Progress >= requiredProgress;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002A12 File Offset: 0x00000C12
		public bool MeetsFlagsRequirement(ulong requiredFlags)
		{
			return (this.Flags & requiredFlags) == requiredFlags;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002A1F File Offset: 0x00000C1F
		public bool MeetsProgressAndFlagsRequirements(int requiredProgress, ulong requiredFlags)
		{
			return this.MeetsProgressRequirement(requiredProgress) && this.MeetsFlagsRequirement(requiredFlags);
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002A38 File Offset: 0x00000C38
		public bool MeetsProgressAndFlagsRequirements(AdventureMission.WingProgress requiredProgress)
		{
			return requiredProgress == null || (requiredProgress.Wing == this.Wing && this.MeetsProgressAndFlagsRequirements(requiredProgress.Progress, requiredProgress.Flags));
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002A72 File Offset: 0x00000C72
		public void SetProgress(int progress)
		{
			if (this.m_progress > progress)
			{
				return;
			}
			this.m_progress = progress;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002A88 File Offset: 0x00000C88
		public void SetFlags(ulong flags)
		{
			this.m_flags = flags;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002A94 File Offset: 0x00000C94
		public override string ToString()
		{
			return string.Format("[AdventureMission.WingProgress: Wing={0}, Progress={1} Flags={2}]", this.Wing, this.Progress, this.Flags);
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002ACC File Offset: 0x00000CCC
		public AdventureMission.WingProgress Clone()
		{
			return new AdventureMission.WingProgress(this.Wing, this.Progress, this.Flags);
		}

		// Token: 0x04000010 RID: 16
		private int m_wing;

		// Token: 0x04000011 RID: 17
		private int m_progress;

		// Token: 0x04000012 RID: 18
		private ulong m_flags;
	}
}
