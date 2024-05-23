using System;
using System.ComponentModel;

// Token: 0x02000157 RID: 343
public enum FixedRewardType
{
	// Token: 0x0400097D RID: 2429
	UNKNOWN,
	// Token: 0x0400097E RID: 2430
	[Description("card")]
	VIRTUAL_CARD,
	// Token: 0x0400097F RID: 2431
	[Description("real_card")]
	REAL_CARD,
	// Token: 0x04000980 RID: 2432
	[Description("cardback")]
	CARD_BACK,
	// Token: 0x04000981 RID: 2433
	[Description("craftable_card")]
	CRAFTABLE_CARD,
	// Token: 0x04000982 RID: 2434
	[Description("meta_action_flags")]
	META_ACTION_FLAGS
}
