using System;

// Token: 0x020003C7 RID: 967
public class ClassChallengeUnlockData : RewardData
{
	// Token: 0x06003263 RID: 12899 RVA: 0x000FC701 File Offset: 0x000FA901
	public ClassChallengeUnlockData() : this(0)
	{
	}

	// Token: 0x06003264 RID: 12900 RVA: 0x000FC70A File Offset: 0x000FA90A
	public ClassChallengeUnlockData(int wingID) : base(Reward.Type.CLASS_CHALLENGE)
	{
		this.WingID = wingID;
	}

	// Token: 0x170003F4 RID: 1012
	// (get) Token: 0x06003265 RID: 12901 RVA: 0x000FC71A File Offset: 0x000FA91A
	// (set) Token: 0x06003266 RID: 12902 RVA: 0x000FC722 File Offset: 0x000FA922
	public int WingID { get; set; }

	// Token: 0x06003267 RID: 12903 RVA: 0x000FC72C File Offset: 0x000FA92C
	public override string ToString()
	{
		return string.Format("[ClassChallengeUnlockData: WingID={0} Origin={1} OriginData={2}]", this.WingID, base.Origin, base.OriginData);
	}

	// Token: 0x06003268 RID: 12904 RVA: 0x000FC764 File Offset: 0x000FA964
	protected override string GetGameObjectName()
	{
		return "ClassChallengeUnlocked";
	}
}
