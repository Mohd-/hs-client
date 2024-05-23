using System;

// Token: 0x0200001B RID: 27
public class CardRewardData : RewardData
{
	// Token: 0x060002A1 RID: 673 RVA: 0x0000CB42 File Offset: 0x0000AD42
	public CardRewardData() : this(string.Empty, TAG_PREMIUM.NORMAL, 0)
	{
	}

	// Token: 0x060002A2 RID: 674 RVA: 0x0000CB54 File Offset: 0x0000AD54
	public CardRewardData(string cardID, TAG_PREMIUM premium, int count) : base(Reward.Type.CARD)
	{
		this.CardID = cardID;
		this.Count = count;
		this.Premium = premium;
		this.InnKeeperLine = CardRewardData.InnKeeperTrigger.NONE;
	}

	// Token: 0x1700003A RID: 58
	// (get) Token: 0x060002A3 RID: 675 RVA: 0x0000CB84 File Offset: 0x0000AD84
	// (set) Token: 0x060002A4 RID: 676 RVA: 0x0000CB8C File Offset: 0x0000AD8C
	public string CardID { get; set; }

	// Token: 0x1700003B RID: 59
	// (get) Token: 0x060002A5 RID: 677 RVA: 0x0000CB95 File Offset: 0x0000AD95
	// (set) Token: 0x060002A6 RID: 678 RVA: 0x0000CB9D File Offset: 0x0000AD9D
	public TAG_PREMIUM Premium { get; set; }

	// Token: 0x1700003C RID: 60
	// (get) Token: 0x060002A7 RID: 679 RVA: 0x0000CBA6 File Offset: 0x0000ADA6
	// (set) Token: 0x060002A8 RID: 680 RVA: 0x0000CBAE File Offset: 0x0000ADAE
	public int Count { get; set; }

	// Token: 0x1700003D RID: 61
	// (get) Token: 0x060002A9 RID: 681 RVA: 0x0000CBB7 File Offset: 0x0000ADB7
	// (set) Token: 0x060002AA RID: 682 RVA: 0x0000CBBF File Offset: 0x0000ADBF
	public CardRewardData.InnKeeperTrigger InnKeeperLine { get; set; }

	// Token: 0x1700003E RID: 62
	// (get) Token: 0x060002AB RID: 683 RVA: 0x0000CBC8 File Offset: 0x0000ADC8
	// (set) Token: 0x060002AC RID: 684 RVA: 0x0000CBD0 File Offset: 0x0000ADD0
	public FixedRewardMapDbfRecord FixedReward { get; set; }

	// Token: 0x060002AD RID: 685 RVA: 0x0000CBDC File Offset: 0x0000ADDC
	public override string ToString()
	{
		EntityDef entityDef = DefLoader.Get().GetEntityDef(this.CardID);
		string text = (entityDef != null) ? entityDef.GetName() : "[UNKNOWN]";
		return string.Format("[CardRewardData: cardName={0} CardID={1}, Premium={2} Count={3} Origin={4} OriginData={5}]", new object[]
		{
			text,
			this.CardID,
			this.Premium,
			this.Count,
			base.Origin,
			base.OriginData
		});
	}

	// Token: 0x060002AE RID: 686 RVA: 0x0000CC68 File Offset: 0x0000AE68
	public bool Merge(CardRewardData other)
	{
		if (!this.CardID.Equals(other.CardID))
		{
			return false;
		}
		if (!this.Premium.Equals(other.Premium))
		{
			return false;
		}
		this.Count += other.Count;
		foreach (long noticeID in other.m_noticeIDs)
		{
			base.AddNoticeID(noticeID);
		}
		return true;
	}

	// Token: 0x060002AF RID: 687 RVA: 0x0000CD10 File Offset: 0x0000AF10
	protected override string GetGameObjectName()
	{
		return "CardReward";
	}

	// Token: 0x0200014C RID: 332
	public enum InnKeeperTrigger
	{
		// Token: 0x04000950 RID: 2384
		NONE,
		// Token: 0x04000951 RID: 2385
		CORE_CLASS_SET_COMPLETE,
		// Token: 0x04000952 RID: 2386
		SECOND_REWARD_EVER
	}
}
