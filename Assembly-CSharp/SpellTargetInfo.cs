using System;

// Token: 0x02000E32 RID: 3634
[Serializable]
public class SpellTargetInfo
{
	// Token: 0x0400579F RID: 22431
	public SpellTargetBehavior m_Behavior;

	// Token: 0x040057A0 RID: 22432
	public int m_RandomTargetCountMin = 8;

	// Token: 0x040057A1 RID: 22433
	public int m_RandomTargetCountMax = 10;

	// Token: 0x040057A2 RID: 22434
	public bool m_SuppressPlaySounds;

	// Token: 0x040057A3 RID: 22435
	public bool m_OnlyAddMetaDataTargets;
}
