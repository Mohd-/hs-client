﻿using System;

// Token: 0x02000160 RID: 352
[Flags]
public enum UserAttentionBlocker
{
	// Token: 0x040009F3 RID: 2547
	NONE = 0,
	// Token: 0x040009F4 RID: 2548
	FATAL_ERROR_SCENE = 1,
	// Token: 0x040009F5 RID: 2549
	SET_ROTATION_INTRO = 2,
	// Token: 0x040009F6 RID: 2550
	SET_ROTATION_CM_TUTORIALS = 4,
	// Token: 0x040009F7 RID: 2551
	ALL = -1,
	// Token: 0x040009F8 RID: 2552
	ALL_EXCEPT_FATAL_ERROR_SCENE = -2
}
