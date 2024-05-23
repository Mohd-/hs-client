using System;
using UnityEngine;

// Token: 0x0200027C RID: 636
[Serializable]
public class BoxDrawerStateInfo
{
	// Token: 0x04001490 RID: 5264
	public GameObject m_ClosedBone;

	// Token: 0x04001491 RID: 5265
	public float m_ClosedDelaySec;

	// Token: 0x04001492 RID: 5266
	public float m_ClosedMoveSec = 1f;

	// Token: 0x04001493 RID: 5267
	public iTween.EaseType m_ClosedMoveEaseType = iTween.EaseType.linear;

	// Token: 0x04001494 RID: 5268
	public GameObject m_ClosedBoxOpenedBone;

	// Token: 0x04001495 RID: 5269
	public float m_ClosedBoxOpenedDelaySec;

	// Token: 0x04001496 RID: 5270
	public float m_ClosedBoxOpenedMoveSec = 1f;

	// Token: 0x04001497 RID: 5271
	public iTween.EaseType m_ClosedBoxOpenedMoveEaseType = iTween.EaseType.linear;

	// Token: 0x04001498 RID: 5272
	public GameObject m_OpenedBone;

	// Token: 0x04001499 RID: 5273
	public float m_OpenedDelaySec;

	// Token: 0x0400149A RID: 5274
	public float m_OpenedMoveSec = 1f;

	// Token: 0x0400149B RID: 5275
	public iTween.EaseType m_OpenedMoveEaseType = iTween.EaseType.easeOutBounce;
}
