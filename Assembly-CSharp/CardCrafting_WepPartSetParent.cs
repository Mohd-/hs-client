using System;
using UnityEngine;

// Token: 0x02000EF8 RID: 3832
public class CardCrafting_WepPartSetParent : MonoBehaviour
{
	// Token: 0x0600728B RID: 29323 RVA: 0x0021AE29 File Offset: 0x00219029
	private void Start()
	{
		if (!this.m_Parent)
		{
			Debug.LogError("Animation Event Set Parent is null!");
			base.enabled = false;
		}
	}

	// Token: 0x0600728C RID: 29324 RVA: 0x0021AE4C File Offset: 0x0021904C
	public void SetParentWepParts()
	{
		if (this.m_Parent)
		{
			this.m_WepParts.transform.parent = this.m_Parent.transform;
		}
	}

	// Token: 0x04005C9A RID: 23706
	public GameObject m_Parent;

	// Token: 0x04005C9B RID: 23707
	public GameObject m_WepParts;
}
