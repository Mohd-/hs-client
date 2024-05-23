using System;
using UnityEngine;

// Token: 0x02000EF3 RID: 3827
public class BounceScale : MonoBehaviour
{
	// Token: 0x06007275 RID: 29301 RVA: 0x00219EDC File Offset: 0x002180DC
	public void BounceyScale()
	{
		Vector3 localScale = base.transform.localScale;
		base.transform.localScale = Vector3.zero;
		iTween.ScaleTo(base.gameObject, iTween.Hash(new object[]
		{
			"scale",
			localScale,
			"time",
			this.m_Time,
			"easetype",
			iTween.EaseType.easeOutElastic
		}));
	}

	// Token: 0x04005C7A RID: 23674
	public float m_Time;
}
