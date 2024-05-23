using System;
using UnityEngine;

// Token: 0x02000EA0 RID: 3744
[Serializable]
public class TwistingNetherFloatInfo
{
	// Token: 0x04005A88 RID: 23176
	public Vector3 m_OffsetMin = new Vector3(-1.5f, -1.5f, -1.5f);

	// Token: 0x04005A89 RID: 23177
	public Vector3 m_OffsetMax = new Vector3(1.5f, 1.5f, 1.5f);

	// Token: 0x04005A8A RID: 23178
	public Vector2 m_RotationXZMin = new Vector2(-10f, -10f);

	// Token: 0x04005A8B RID: 23179
	public Vector2 m_RotationXZMax = new Vector2(10f, 10f);

	// Token: 0x04005A8C RID: 23180
	public float m_DurationMin = 1.5f;

	// Token: 0x04005A8D RID: 23181
	public float m_DurationMax = 2f;
}
