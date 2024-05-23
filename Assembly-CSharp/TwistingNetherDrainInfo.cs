using System;

// Token: 0x02000EA1 RID: 3745
[Serializable]
public class TwistingNetherDrainInfo
{
	// Token: 0x04005A8E RID: 23182
	public float m_DelayMin;

	// Token: 0x04005A8F RID: 23183
	public float m_DelayMax;

	// Token: 0x04005A90 RID: 23184
	public float m_DurationMin = 1.5f;

	// Token: 0x04005A91 RID: 23185
	public float m_DurationMax = 2f;

	// Token: 0x04005A92 RID: 23186
	public iTween.EaseType m_EaseType = iTween.EaseType.easeInOutCubic;
}
