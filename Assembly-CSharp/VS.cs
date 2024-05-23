using System;
using UnityEngine;

// Token: 0x020002A9 RID: 681
public class VS : MonoBehaviour
{
	// Token: 0x06002564 RID: 9572 RVA: 0x000B81AF File Offset: 0x000B63AF
	private void Start()
	{
		this.SetDefaults();
	}

	// Token: 0x06002565 RID: 9573 RVA: 0x000B81B7 File Offset: 0x000B63B7
	private void OnDestroy()
	{
		this.SetDefaults();
	}

	// Token: 0x06002566 RID: 9574 RVA: 0x000B81BF File Offset: 0x000B63BF
	private void SetDefaults()
	{
		this.ActivateShadow(false);
	}

	// Token: 0x06002567 RID: 9575 RVA: 0x000B81C8 File Offset: 0x000B63C8
	public void ActivateShadow(bool active = true)
	{
		this.m_shadow.SetActive(active);
	}

	// Token: 0x06002568 RID: 9576 RVA: 0x000B81D6 File Offset: 0x000B63D6
	public void ActivateAnimation(bool active = true)
	{
		base.gameObject.GetComponentInChildren<Animation>().enabled = active;
	}

	// Token: 0x04001618 RID: 5656
	public GameObject m_shadow;
}
