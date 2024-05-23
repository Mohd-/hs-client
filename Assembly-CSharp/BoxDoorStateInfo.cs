using System;
using UnityEngine;

// Token: 0x02000279 RID: 633
[Serializable]
public class BoxDoorStateInfo
{
	// Token: 0x0400147F RID: 5247
	public Vector3 m_OpenedRotation = new Vector3(0f, 0f, 180f);

	// Token: 0x04001480 RID: 5248
	public float m_OpenedDelaySec;

	// Token: 0x04001481 RID: 5249
	public float m_OpenedRotateSec = 0.35f;

	// Token: 0x04001482 RID: 5250
	public iTween.EaseType m_OpenedRotateEaseType;

	// Token: 0x04001483 RID: 5251
	public Vector3 m_ClosedRotation = Vector3.zero;

	// Token: 0x04001484 RID: 5252
	public float m_ClosedDelaySec;

	// Token: 0x04001485 RID: 5253
	public float m_ClosedRotateSec = 0.35f;

	// Token: 0x04001486 RID: 5254
	public iTween.EaseType m_ClosedRotateEaseType;
}
