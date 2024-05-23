using System;
using UnityEngine;

// Token: 0x0200027A RID: 634
[Serializable]
public class BoxDiskStateInfo
{
	// Token: 0x04001487 RID: 5255
	public Vector3 m_MainMenuRotation = new Vector3(0f, 0f, 180f);

	// Token: 0x04001488 RID: 5256
	public float m_MainMenuDelaySec = 0.1f;

	// Token: 0x04001489 RID: 5257
	public float m_MainMenuRotateSec = 0.17f;

	// Token: 0x0400148A RID: 5258
	public iTween.EaseType m_MainMenuRotateEaseType;

	// Token: 0x0400148B RID: 5259
	public Vector3 m_LoadingRotation = new Vector3(0f, 0f, 0f);

	// Token: 0x0400148C RID: 5260
	public float m_LoadingDelaySec = 0.1f;

	// Token: 0x0400148D RID: 5261
	public float m_LoadingRotateSec = 0.17f;

	// Token: 0x0400148E RID: 5262
	public iTween.EaseType m_LoadingRotateEaseType;
}
