using System;

// Token: 0x020004A1 RID: 1185
public class BoosterPackRewardData : RewardData
{
	// Token: 0x0600386E RID: 14446 RVA: 0x00114040 File Offset: 0x00112240
	public BoosterPackRewardData() : this(0, 0)
	{
	}

	// Token: 0x0600386F RID: 14447 RVA: 0x0011404A File Offset: 0x0011224A
	public BoosterPackRewardData(int id, int count) : base(Reward.Type.BOOSTER_PACK)
	{
		this.Id = id;
		this.Count = count;
	}

	// Token: 0x17000434 RID: 1076
	// (get) Token: 0x06003870 RID: 14448 RVA: 0x00114061 File Offset: 0x00112261
	// (set) Token: 0x06003871 RID: 14449 RVA: 0x00114069 File Offset: 0x00112269
	public int Id { get; set; }

	// Token: 0x17000435 RID: 1077
	// (get) Token: 0x06003872 RID: 14450 RVA: 0x00114072 File Offset: 0x00112272
	// (set) Token: 0x06003873 RID: 14451 RVA: 0x0011407A File Offset: 0x0011227A
	public int Count { get; set; }

	// Token: 0x06003874 RID: 14452 RVA: 0x00114084 File Offset: 0x00112284
	public override string ToString()
	{
		return string.Format("[BoosterPackRewardData: BoosterType={0} Count={1} Origin={2} OriginData={3}]", new object[]
		{
			this.Id,
			this.Count,
			base.Origin,
			base.OriginData
		});
	}

	// Token: 0x06003875 RID: 14453 RVA: 0x001140D9 File Offset: 0x001122D9
	protected override string GetGameObjectName()
	{
		return "BoosterPackReward";
	}
}
