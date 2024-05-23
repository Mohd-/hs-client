using System;

// Token: 0x0200049C RID: 1180
public class ArcaneDustRewardData : RewardData
{
	// Token: 0x06003853 RID: 14419 RVA: 0x00113E28 File Offset: 0x00112028
	public ArcaneDustRewardData() : this(0)
	{
	}

	// Token: 0x06003854 RID: 14420 RVA: 0x00113E31 File Offset: 0x00112031
	public ArcaneDustRewardData(int amount) : base(Reward.Type.ARCANE_DUST)
	{
		this.Amount = amount;
	}

	// Token: 0x1700042F RID: 1071
	// (get) Token: 0x06003855 RID: 14421 RVA: 0x00113E41 File Offset: 0x00112041
	// (set) Token: 0x06003856 RID: 14422 RVA: 0x00113E49 File Offset: 0x00112049
	public int Amount { get; set; }

	// Token: 0x06003857 RID: 14423 RVA: 0x00113E54 File Offset: 0x00112054
	public override string ToString()
	{
		return string.Format("[ArcaneDustRewardData: Amount={0} Origin={1} OriginData={2}]", this.Amount, base.Origin, base.OriginData);
	}

	// Token: 0x06003858 RID: 14424 RVA: 0x00113E8C File Offset: 0x0011208C
	protected override string GetGameObjectName()
	{
		return "ArcaneDustReward";
	}
}
