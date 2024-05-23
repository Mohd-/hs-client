using System;
using UnityEngine;

// Token: 0x02000E9F RID: 3743
[Serializable]
public class TwistingNetherLiftInfo
{
	// Token: 0x04005A7B RID: 23163
	public Vector3 m_OffsetMin = new Vector3(-3f, 3.5f, -3f);

	// Token: 0x04005A7C RID: 23164
	public Vector3 m_OffsetMax = new Vector3(3f, 5.5f, 3f);

	// Token: 0x04005A7D RID: 23165
	public float m_DelayMin;

	// Token: 0x04005A7E RID: 23166
	public float m_DelayMax = 0.3f;

	// Token: 0x04005A7F RID: 23167
	public float m_DurationMin = 0.1f;

	// Token: 0x04005A80 RID: 23168
	public float m_DurationMax = 0.3f;

	// Token: 0x04005A81 RID: 23169
	public float m_RotDelayMin;

	// Token: 0x04005A82 RID: 23170
	public float m_RotDelayMax = 0.3f;

	// Token: 0x04005A83 RID: 23171
	public float m_RotDurationMin = 1f;

	// Token: 0x04005A84 RID: 23172
	public float m_RotDurationMax = 3f;

	// Token: 0x04005A85 RID: 23173
	public float m_RotationMin;

	// Token: 0x04005A86 RID: 23174
	public float m_RotationMax = 90f;

	// Token: 0x04005A87 RID: 23175
	public iTween.EaseType m_EaseType = iTween.EaseType.easeOutExpo;
}
