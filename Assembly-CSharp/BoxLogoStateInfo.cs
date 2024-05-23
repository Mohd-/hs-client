using System;

// Token: 0x02000277 RID: 631
[Serializable]
public class BoxLogoStateInfo
{
	// Token: 0x0400146F RID: 5231
	public float m_ShownAlpha = 1f;

	// Token: 0x04001470 RID: 5232
	public float m_ShownDelaySec;

	// Token: 0x04001471 RID: 5233
	public float m_ShownFadeSec = 0.3f;

	// Token: 0x04001472 RID: 5234
	public iTween.EaseType m_ShownFadeEaseType = iTween.EaseType.linear;

	// Token: 0x04001473 RID: 5235
	public float m_HiddenAlpha;

	// Token: 0x04001474 RID: 5236
	public float m_HiddenDelaySec;

	// Token: 0x04001475 RID: 5237
	public float m_HiddenFadeSec = 0.3f;

	// Token: 0x04001476 RID: 5238
	public iTween.EaseType m_HiddenFadeEaseType = iTween.EaseType.linear;
}
