using System;
using PegasusShared;

// Token: 0x0200019F RID: 415
public class TavernBrawlMission
{
	// Token: 0x04000DF6 RID: 3574
	public int seasonId;

	// Token: 0x04000DF7 RID: 3575
	public int missionId = -1;

	// Token: 0x04000DF8 RID: 3576
	public DateTime? endDateLocal;

	// Token: 0x04000DF9 RID: 3577
	public bool canCreateDeck;

	// Token: 0x04000DFA RID: 3578
	public bool canEditDeck;

	// Token: 0x04000DFB RID: 3579
	public bool canSelectHeroForDeck;

	// Token: 0x04000DFC RID: 3580
	public RewardType rewardType = 1;

	// Token: 0x04000DFD RID: 3581
	public RewardTrigger rewardTrigger = 1;

	// Token: 0x04000DFE RID: 3582
	public long RewardData1;

	// Token: 0x04000DFF RID: 3583
	public long RewardData2;

	// Token: 0x04000E00 RID: 3584
	public DeckRuleset deckRuleset;
}
