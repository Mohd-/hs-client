using System;
using UnityEngine;

// Token: 0x0200027D RID: 637
[Serializable]
public class BoxCameraStateInfo
{
	// Token: 0x0400149C RID: 5276
	public GameObject m_ClosedBone;

	// Token: 0x0400149D RID: 5277
	public GameObject m_ClosedMinAspectRatioBone;

	// Token: 0x0400149E RID: 5278
	public float m_ClosedDelaySec;

	// Token: 0x0400149F RID: 5279
	public float m_ClosedMoveSec = 0.7f;

	// Token: 0x040014A0 RID: 5280
	public iTween.EaseType m_ClosedMoveEaseType = iTween.EaseType.easeOutCubic;

	// Token: 0x040014A1 RID: 5281
	public GameObject m_ClosedWithDrawerBone;

	// Token: 0x040014A2 RID: 5282
	public GameObject m_ClosedWithDrawerMinAspectRatioBone;

	// Token: 0x040014A3 RID: 5283
	public float m_ClosedWithDrawerDelaySec;

	// Token: 0x040014A4 RID: 5284
	public float m_ClosedWithDrawerMoveSec = 0.7f;

	// Token: 0x040014A5 RID: 5285
	public iTween.EaseType m_ClosedWithDrawerMoveEaseType = iTween.EaseType.easeOutCubic;

	// Token: 0x040014A6 RID: 5286
	public GameObject m_OpenedBone;

	// Token: 0x040014A7 RID: 5287
	public GameObject m_OpenedMinAspectRatioBone;

	// Token: 0x040014A8 RID: 5288
	public float m_OpenedDelaySec;

	// Token: 0x040014A9 RID: 5289
	public float m_OpenedMoveSec = 0.7f;

	// Token: 0x040014AA RID: 5290
	public iTween.EaseType m_OpenedMoveEaseType = iTween.EaseType.easeOutCubic;
}
