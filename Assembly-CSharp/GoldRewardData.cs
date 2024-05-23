using System;

// Token: 0x0200049E RID: 1182
public class GoldRewardData : RewardData
{
	// Token: 0x0600385F RID: 14431 RVA: 0x00113EFF File Offset: 0x001120FF
	public GoldRewardData() : this(0L)
	{
	}

	// Token: 0x06003860 RID: 14432 RVA: 0x00113F0C File Offset: 0x0011210C
	public GoldRewardData(long amount) : this(amount, default(DateTime?))
	{
	}

	// Token: 0x06003861 RID: 14433 RVA: 0x00113F29 File Offset: 0x00112129
	public GoldRewardData(long amount, DateTime? date) : base(Reward.Type.GOLD)
	{
		this.Amount = amount;
		this.Date = date;
	}

	// Token: 0x17000431 RID: 1073
	// (get) Token: 0x06003862 RID: 14434 RVA: 0x00113F40 File Offset: 0x00112140
	// (set) Token: 0x06003863 RID: 14435 RVA: 0x00113F48 File Offset: 0x00112148
	public long Amount { get; set; }

	// Token: 0x17000432 RID: 1074
	// (get) Token: 0x06003864 RID: 14436 RVA: 0x00113F51 File Offset: 0x00112151
	// (set) Token: 0x06003865 RID: 14437 RVA: 0x00113F59 File Offset: 0x00112159
	public DateTime? Date { get; set; }

	// Token: 0x06003866 RID: 14438 RVA: 0x00113F64 File Offset: 0x00112164
	public override string ToString()
	{
		return string.Format("[GoldRewardData: Amount={0} Origin={1} OriginData={2}]", this.Amount, base.Origin, base.OriginData);
	}

	// Token: 0x06003867 RID: 14439 RVA: 0x00113F9C File Offset: 0x0011219C
	protected override string GetGameObjectName()
	{
		return "GoldReward";
	}
}
