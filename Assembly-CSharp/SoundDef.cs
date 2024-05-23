using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000362 RID: 866
[CustomEditClass]
public class SoundDef : MonoBehaviour
{
	// Token: 0x04001B61 RID: 7009
	public SoundCategory m_Category = SoundCategory.FX;

	// Token: 0x04001B62 RID: 7010
	public List<RandomAudioClip> m_RandomClips;

	// Token: 0x04001B63 RID: 7011
	public float m_RandomPitchMin = 1f;

	// Token: 0x04001B64 RID: 7012
	public float m_RandomPitchMax = 1f;

	// Token: 0x04001B65 RID: 7013
	public float m_RandomVolumeMin = 1f;

	// Token: 0x04001B66 RID: 7014
	public float m_RandomVolumeMax = 1f;

	// Token: 0x04001B67 RID: 7015
	public bool m_IgnoreDucking;
}
