﻿using System;

// Token: 0x0200023C RID: 572
public enum FindGameState
{
	// Token: 0x040012BD RID: 4797
	INVALID,
	// Token: 0x040012BE RID: 4798
	CLIENT_STARTED,
	// Token: 0x040012BF RID: 4799
	CLIENT_CANCELED,
	// Token: 0x040012C0 RID: 4800
	CLIENT_ERROR,
	// Token: 0x040012C1 RID: 4801
	BNET_QUEUE_ENTERED,
	// Token: 0x040012C2 RID: 4802
	BNET_QUEUE_DELAYED,
	// Token: 0x040012C3 RID: 4803
	BNET_QUEUE_UPDATED,
	// Token: 0x040012C4 RID: 4804
	BNET_QUEUE_CANCELED,
	// Token: 0x040012C5 RID: 4805
	BNET_ERROR,
	// Token: 0x040012C6 RID: 4806
	SERVER_GAME_CONNECTING,
	// Token: 0x040012C7 RID: 4807
	SERVER_GAME_STARTED,
	// Token: 0x040012C8 RID: 4808
	SERVER_GAME_CANCELED
}
