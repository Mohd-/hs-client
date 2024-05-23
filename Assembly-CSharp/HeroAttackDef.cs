using System;

// Token: 0x0200095E RID: 2398
[Serializable]
public class HeroAttackDef
{
	// Token: 0x04003EA9 RID: 16041
	public float m_MoveToTargetDuration = 0.12f;

	// Token: 0x04003EAA RID: 16042
	public iTween.EaseType m_MoveToTargetEaseType = iTween.EaseType.linear;

	// Token: 0x04003EAB RID: 16043
	public float m_OrientToTargetDuration = 0.3f;

	// Token: 0x04003EAC RID: 16044
	public iTween.EaseType m_OrientToTargetEaseType = iTween.EaseType.linear;

	// Token: 0x04003EAD RID: 16045
	public float m_MoveBackDuration = 0.15f;

	// Token: 0x04003EAE RID: 16046
	public iTween.EaseType m_MoveBackEaseType = iTween.Defaults.easeType;

	// Token: 0x04003EAF RID: 16047
	public float m_OrientBackDuration = 0.3f;

	// Token: 0x04003EB0 RID: 16048
	public iTween.EaseType m_OrientBackEaseType = iTween.EaseType.linear;
}
