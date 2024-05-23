using System;

// Token: 0x02000358 RID: 856
[Serializable]
public class SoundDuckedCategoryDef
{
	// Token: 0x04001B34 RID: 6964
	public SoundCategory m_Category;

	// Token: 0x04001B35 RID: 6965
	public float m_Volume = 0.2f;

	// Token: 0x04001B36 RID: 6966
	public float m_BeginSec = 0.7f;

	// Token: 0x04001B37 RID: 6967
	public iTween.EaseType m_BeginEaseType = iTween.EaseType.linear;

	// Token: 0x04001B38 RID: 6968
	public float m_RestoreSec = 0.7f;

	// Token: 0x04001B39 RID: 6969
	public iTween.EaseType m_RestoreEaseType = iTween.EaseType.linear;
}
