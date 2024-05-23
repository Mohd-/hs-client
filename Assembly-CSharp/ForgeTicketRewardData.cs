using System;

// Token: 0x0200049D RID: 1181
public class ForgeTicketRewardData : RewardData
{
	// Token: 0x06003859 RID: 14425 RVA: 0x00113E93 File Offset: 0x00112093
	public ForgeTicketRewardData() : this(0)
	{
	}

	// Token: 0x0600385A RID: 14426 RVA: 0x00113E9C File Offset: 0x0011209C
	public ForgeTicketRewardData(int quantity) : base(Reward.Type.FORGE_TICKET)
	{
		this.Quantity = quantity;
	}

	// Token: 0x17000430 RID: 1072
	// (get) Token: 0x0600385B RID: 14427 RVA: 0x00113EAC File Offset: 0x001120AC
	// (set) Token: 0x0600385C RID: 14428 RVA: 0x00113EB4 File Offset: 0x001120B4
	public int Quantity { get; set; }

	// Token: 0x0600385D RID: 14429 RVA: 0x00113EC0 File Offset: 0x001120C0
	public override string ToString()
	{
		return string.Format("[ForgeTicketRewardData: Quantity={0} Origin={1} OriginData={2}]", this.Quantity, base.Origin, base.OriginData);
	}

	// Token: 0x0600385E RID: 14430 RVA: 0x00113EF8 File Offset: 0x001120F8
	protected override string GetGameObjectName()
	{
		return "ArenaTicketReward";
	}
}
