using System;

// Token: 0x02000EA2 RID: 3746
[Serializable]
public class TwistingNetherSqueezeInfo
{
	// Token: 0x04005A93 RID: 23187
	public float m_DelayMin;

	// Token: 0x04005A94 RID: 23188
	public float m_DelayMax;

	// Token: 0x04005A95 RID: 23189
	public float m_DurationMin = 1f;

	// Token: 0x04005A96 RID: 23190
	public float m_DurationMax = 1.5f;

	// Token: 0x04005A97 RID: 23191
	public iTween.EaseType m_EaseType = iTween.EaseType.easeInCubic;
}
