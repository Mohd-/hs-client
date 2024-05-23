using System;
using UnityEngine;

// Token: 0x02000EEE RID: 3822
public class AnimSpeed : MonoBehaviour
{
	// Token: 0x06007265 RID: 29285 RVA: 0x00219B0C File Offset: 0x00217D0C
	private void Awake()
	{
		foreach (object obj in base.GetComponent<Animation>())
		{
			AnimationState animationState = (AnimationState)obj;
			animationState.speed = this.animspeed;
		}
	}

	// Token: 0x04005C70 RID: 23664
	public float animspeed = 1f;
}
