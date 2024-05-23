using System;

// Token: 0x02000278 RID: 632
[Serializable]
public class BoxStartButtonStateInfo
{
	// Token: 0x04001477 RID: 5239
	public float m_ShownAlpha = 1f;

	// Token: 0x04001478 RID: 5240
	public float m_ShownDelaySec;

	// Token: 0x04001479 RID: 5241
	public float m_ShownFadeSec = 0.3f;

	// Token: 0x0400147A RID: 5242
	public iTween.EaseType m_ShownFadeEaseType = iTween.EaseType.linear;

	// Token: 0x0400147B RID: 5243
	public float m_HiddenAlpha;

	// Token: 0x0400147C RID: 5244
	public float m_HiddenDelaySec;

	// Token: 0x0400147D RID: 5245
	public float m_HiddenFadeSec = 0.3f;

	// Token: 0x0400147E RID: 5246
	public iTween.EaseType m_HiddenFadeEaseType = iTween.EaseType.linear;
}
