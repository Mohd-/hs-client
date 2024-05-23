using System;
using UnityEngine;

// Token: 0x02000853 RID: 2131
public class CardStandIn : MonoBehaviour
{
	// Token: 0x0600521F RID: 21023 RVA: 0x001886E9 File Offset: 0x001868E9
	public void DisableStandIn()
	{
		this.standInCollider.enabled = false;
	}

	// Token: 0x06005220 RID: 21024 RVA: 0x001886F7 File Offset: 0x001868F7
	public void EnableStandIn()
	{
		this.standInCollider.enabled = true;
	}

	// Token: 0x0400386C RID: 14444
	public int slot;

	// Token: 0x0400386D RID: 14445
	public Card linkedCard;

	// Token: 0x0400386E RID: 14446
	public Collider standInCollider;
}
