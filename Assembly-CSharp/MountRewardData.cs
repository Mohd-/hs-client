using System;

// Token: 0x0200049F RID: 1183
public class MountRewardData : RewardData
{
	// Token: 0x06003868 RID: 14440 RVA: 0x00113FA3 File Offset: 0x001121A3
	public MountRewardData() : this(MountRewardData.MountType.UNKNOWN)
	{
	}

	// Token: 0x06003869 RID: 14441 RVA: 0x00113FAC File Offset: 0x001121AC
	public MountRewardData(MountRewardData.MountType mount) : base(Reward.Type.MOUNT)
	{
		this.Mount = mount;
	}

	// Token: 0x17000433 RID: 1075
	// (get) Token: 0x0600386A RID: 14442 RVA: 0x00113FBC File Offset: 0x001121BC
	// (set) Token: 0x0600386B RID: 14443 RVA: 0x00113FC4 File Offset: 0x001121C4
	public MountRewardData.MountType Mount { get; set; }

	// Token: 0x0600386C RID: 14444 RVA: 0x00113FD0 File Offset: 0x001121D0
	public override string ToString()
	{
		return string.Format("[MountRewardData Mount={0} Origin={1} OriginData={2}]", this.Mount, base.Origin, base.OriginData);
	}

	// Token: 0x0600386D RID: 14445 RVA: 0x00114008 File Offset: 0x00112208
	protected override string GetGameObjectName()
	{
		MountRewardData.MountType mount = this.Mount;
		if (mount == MountRewardData.MountType.WOW_HEARTHSTEED)
		{
			return "HearthSteedReward";
		}
		if (mount != MountRewardData.MountType.HEROES_MAGIC_CARPET_CARD)
		{
			return string.Empty;
		}
		return "CardMountReward";
	}

	// Token: 0x020004A0 RID: 1184
	public enum MountType
	{
		// Token: 0x04002439 RID: 9273
		UNKNOWN,
		// Token: 0x0400243A RID: 9274
		WOW_HEARTHSTEED,
		// Token: 0x0400243B RID: 9275
		HEROES_MAGIC_CARPET_CARD
	}
}
