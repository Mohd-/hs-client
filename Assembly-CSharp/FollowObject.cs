using System;
using UnityEngine;

// Token: 0x02000F0D RID: 3853
public class FollowObject : MonoBehaviour
{
	// Token: 0x06007312 RID: 29458 RVA: 0x0021DDF7 File Offset: 0x0021BFF7
	private void LateUpdate()
	{
		base.transform.position = this.targetObj.position;
	}

	// Token: 0x04005D66 RID: 23910
	public Transform targetObj;
}
