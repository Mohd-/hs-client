using System;
using System.ComponentModel;

// Token: 0x0200018C RID: 396
public enum SpellStateType
{
	// Token: 0x04000B5D RID: 2909
	[Description("None")]
	NONE,
	// Token: 0x04000B5E RID: 2910
	[Description("Birth")]
	BIRTH,
	// Token: 0x04000B5F RID: 2911
	[Description("Idle")]
	IDLE,
	// Token: 0x04000B60 RID: 2912
	[Description("Action")]
	ACTION,
	// Token: 0x04000B61 RID: 2913
	[Description("Cancel")]
	CANCEL,
	// Token: 0x04000B62 RID: 2914
	[Description("Death")]
	DEATH
}
