using System;
using UnityEngine;

// Token: 0x02000E26 RID: 3622
[Serializable]
public class SpellMissileInfo
{
	// Token: 0x04005731 RID: 22321
	public bool m_Enabled = true;

	// Token: 0x04005732 RID: 22322
	public Spell m_Prefab;

	// Token: 0x04005733 RID: 22323
	public Spell m_ReversePrefab;

	// Token: 0x04005734 RID: 22324
	public float m_reverseDelay;

	// Token: 0x04005735 RID: 22325
	public bool m_UseSuperSpellLocation = true;

	// Token: 0x04005736 RID: 22326
	public float m_SpawnDelaySecMin;

	// Token: 0x04005737 RID: 22327
	public float m_SpawnDelaySecMax;

	// Token: 0x04005738 RID: 22328
	public bool m_SpawnInSequence;

	// Token: 0x04005739 RID: 22329
	public float m_PathDurationMin = 0.5f;

	// Token: 0x0400573A RID: 22330
	public float m_PathDurationMax = 1f;

	// Token: 0x0400573B RID: 22331
	public iTween.EaseType m_PathEaseType = iTween.EaseType.linear;

	// Token: 0x0400573C RID: 22332
	public bool m_OrientToPath;

	// Token: 0x0400573D RID: 22333
	public float m_CenterOffsetPercent = 50f;

	// Token: 0x0400573E RID: 22334
	public float m_CenterPointHeightMin;

	// Token: 0x0400573F RID: 22335
	public float m_CenterPointHeightMax;

	// Token: 0x04005740 RID: 22336
	public float m_RightMin;

	// Token: 0x04005741 RID: 22337
	public float m_RightMax;

	// Token: 0x04005742 RID: 22338
	public float m_LeftMin;

	// Token: 0x04005743 RID: 22339
	public float m_LeftMax;

	// Token: 0x04005744 RID: 22340
	public bool m_DebugForceMax;

	// Token: 0x04005745 RID: 22341
	public float m_DistanceScaleFactor = 8f;

	// Token: 0x04005746 RID: 22342
	public string m_TargetJoint = "TargetJoint";

	// Token: 0x04005747 RID: 22343
	public float m_TargetHeightOffset = 0.5f;

	// Token: 0x04005748 RID: 22344
	public Vector3 m_JointUpVector = Vector3.up;
}
