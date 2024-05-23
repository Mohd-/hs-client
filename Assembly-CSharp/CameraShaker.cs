using System;
using UnityEngine;

// Token: 0x02000EF7 RID: 3831
public class CameraShaker : MonoBehaviour
{
	// Token: 0x06007289 RID: 29321 RVA: 0x0021ADDC File Offset: 0x00218FDC
	public void StartShake()
	{
		float? holdAtTime = default(float?);
		if (this.m_Hold)
		{
			holdAtTime = new float?(this.m_HoldAtSec);
		}
		CameraShakeMgr.Shake(Camera.main, this.m_Amount, this.m_IntensityCurve, holdAtTime);
	}

	// Token: 0x04005C96 RID: 23702
	public Vector3 m_Amount;

	// Token: 0x04005C97 RID: 23703
	public AnimationCurve m_IntensityCurve;

	// Token: 0x04005C98 RID: 23704
	public bool m_Hold;

	// Token: 0x04005C99 RID: 23705
	public float m_HoldAtSec;
}
