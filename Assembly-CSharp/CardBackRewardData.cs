using System;

// Token: 0x02000158 RID: 344
public class CardBackRewardData : RewardData
{
	// Token: 0x06001240 RID: 4672 RVA: 0x0004F4BE File Offset: 0x0004D6BE
	public CardBackRewardData() : this(0)
	{
	}

	// Token: 0x06001241 RID: 4673 RVA: 0x0004F4C7 File Offset: 0x0004D6C7
	public CardBackRewardData(int cardBackID) : base(Reward.Type.CARD_BACK)
	{
		this.CardBackID = cardBackID;
	}

	// Token: 0x170002ED RID: 749
	// (get) Token: 0x06001242 RID: 4674 RVA: 0x0004F4D7 File Offset: 0x0004D6D7
	// (set) Token: 0x06001243 RID: 4675 RVA: 0x0004F4DF File Offset: 0x0004D6DF
	public int CardBackID { get; set; }

	// Token: 0x06001244 RID: 4676 RVA: 0x0004F4E8 File Offset: 0x0004D6E8
	public override string ToString()
	{
		return string.Format("[CardBackRewardData: CardBackID={0} Origin={1} OriginData={2}]", this.CardBackID, base.Origin, base.OriginData);
	}

	// Token: 0x06001245 RID: 4677 RVA: 0x0004F520 File Offset: 0x0004D720
	protected override string GetGameObjectName()
	{
		return "CardBackReward";
	}
}
