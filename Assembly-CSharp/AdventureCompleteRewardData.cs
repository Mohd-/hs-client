using System;

// Token: 0x020003C8 RID: 968
public class AdventureCompleteRewardData : RewardData
{
	// Token: 0x06003269 RID: 12905 RVA: 0x000FC76B File Offset: 0x000FA96B
	public AdventureCompleteRewardData() : this("AdventureCompleteReward_Naxxramas", string.Empty, false)
	{
	}

	// Token: 0x0600326A RID: 12906 RVA: 0x000FC780 File Offset: 0x000FA980
	public AdventureCompleteRewardData(string rewardObjectName, string bannerText, bool isBadlyHurt) : base(Reward.Type.CLASS_CHALLENGE)
	{
		this.RewardObjectName = rewardObjectName;
		this.BannerText = bannerText;
		this.IsBadlyHurt = isBadlyHurt;
	}

	// Token: 0x170003F5 RID: 1013
	// (get) Token: 0x0600326B RID: 12907 RVA: 0x000FC7A9 File Offset: 0x000FA9A9
	// (set) Token: 0x0600326C RID: 12908 RVA: 0x000FC7B1 File Offset: 0x000FA9B1
	public bool IsBadlyHurt { get; set; }

	// Token: 0x170003F6 RID: 1014
	// (get) Token: 0x0600326D RID: 12909 RVA: 0x000FC7BA File Offset: 0x000FA9BA
	// (set) Token: 0x0600326E RID: 12910 RVA: 0x000FC7C2 File Offset: 0x000FA9C2
	public string RewardObjectName { get; set; }

	// Token: 0x170003F7 RID: 1015
	// (get) Token: 0x0600326F RID: 12911 RVA: 0x000FC7CB File Offset: 0x000FA9CB
	// (set) Token: 0x06003270 RID: 12912 RVA: 0x000FC7D3 File Offset: 0x000FA9D3
	public string BannerText { get; set; }

	// Token: 0x06003271 RID: 12913 RVA: 0x000FC7DC File Offset: 0x000FA9DC
	public override string ToString()
	{
		return string.Format("[AdventureCompleteRewardData: RewardObjectName={0} Origin={1} OriginData={2}]", this.RewardObjectName, base.Origin, base.OriginData);
	}

	// Token: 0x06003272 RID: 12914 RVA: 0x000FC80F File Offset: 0x000FAA0F
	protected override string GetGameObjectName()
	{
		return this.RewardObjectName;
	}

	// Token: 0x04001F6E RID: 8046
	private const string s_DefaultRewardObject = "AdventureCompleteReward_Naxxramas";
}
