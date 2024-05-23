using System;
using UnityEngine;

// Token: 0x02000EF1 RID: 3825
public class AnimationEventSetParent : MonoBehaviour
{
	// Token: 0x0600726F RID: 29295 RVA: 0x00219D80 File Offset: 0x00217F80
	private void Start()
	{
		if (!this.m_Parent)
		{
			Debug.LogError("Animation Event Set Parent is null!");
			base.enabled = false;
		}
	}

	// Token: 0x06007270 RID: 29296 RVA: 0x00219DA3 File Offset: 0x00217FA3
	public void SetParent()
	{
		if (this.m_Parent)
		{
			base.transform.parent = this.m_Parent.transform;
		}
	}

	// Token: 0x04005C78 RID: 23672
	public GameObject m_Parent;
}
