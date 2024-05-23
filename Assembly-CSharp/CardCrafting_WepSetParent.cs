using System;
using UnityEngine;

// Token: 0x02000EF9 RID: 3833
public class CardCrafting_WepSetParent : MonoBehaviour
{
	// Token: 0x0600728E RID: 29326 RVA: 0x0021AE81 File Offset: 0x00219081
	private void Start()
	{
		if (!this.m_Parent)
		{
			Debug.LogError("Animation Event Set Parent is null!");
			base.enabled = false;
		}
	}

	// Token: 0x0600728F RID: 29327 RVA: 0x0021AEA4 File Offset: 0x002190A4
	public void SetParentManaGem()
	{
		if (this.m_Parent)
		{
			this.m_ManaGem.transform.parent = this.m_Parent.transform;
		}
	}

	// Token: 0x06007290 RID: 29328 RVA: 0x0021AED1 File Offset: 0x002190D1
	public void SetParentPortrait()
	{
		if (this.m_Parent)
		{
			this.m_Portrait.transform.parent = this.m_Parent.transform;
		}
	}

	// Token: 0x06007291 RID: 29329 RVA: 0x0021AEFE File Offset: 0x002190FE
	public void SetParentNameBanner()
	{
		if (this.m_Parent)
		{
			this.m_NameBanner.transform.parent = this.m_Parent.transform;
		}
	}

	// Token: 0x06007292 RID: 29330 RVA: 0x0021AF2B File Offset: 0x0021912B
	public void SetParentRarityGem()
	{
		if (this.m_Parent)
		{
			this.m_RarityGem.transform.parent = this.m_Parent.transform;
		}
	}

	// Token: 0x06007293 RID: 29331 RVA: 0x0021AF58 File Offset: 0x00219158
	public void SetParentDiscription()
	{
		if (this.m_Parent)
		{
			this.m_Discription.transform.parent = this.m_Parent.transform;
		}
	}

	// Token: 0x06007294 RID: 29332 RVA: 0x0021AF85 File Offset: 0x00219185
	public void SetParentSwords()
	{
		if (this.m_Parent)
		{
			this.m_Swords.transform.parent = this.m_Parent.transform;
		}
	}

	// Token: 0x06007295 RID: 29333 RVA: 0x0021AFB2 File Offset: 0x002191B2
	public void SetParentShield()
	{
		if (this.m_Parent)
		{
			this.m_Shield.transform.parent = this.m_Parent.transform;
		}
	}

	// Token: 0x06007296 RID: 29334 RVA: 0x0021AFE0 File Offset: 0x002191E0
	public void SetBackToOrgParent()
	{
		if (this.m_OrgParent)
		{
			this.m_ManaGem.transform.parent = this.m_OrgParent;
		}
		this.m_Portrait.transform.parent = this.m_OrgParent;
		this.m_NameBanner.transform.parent = this.m_OrgParent;
		this.m_RarityGem.transform.parent = this.m_OrgParent;
		this.m_Discription.transform.parent = this.m_OrgParent;
		this.m_Swords.transform.parent = this.m_OrgParent;
		this.m_Shield.transform.parent = this.m_OrgParent;
	}

	// Token: 0x04005C9C RID: 23708
	public GameObject m_Parent;

	// Token: 0x04005C9D RID: 23709
	public Transform m_OrgParent;

	// Token: 0x04005C9E RID: 23710
	public GameObject m_ManaGem;

	// Token: 0x04005C9F RID: 23711
	public GameObject m_Portrait;

	// Token: 0x04005CA0 RID: 23712
	public GameObject m_NameBanner;

	// Token: 0x04005CA1 RID: 23713
	public GameObject m_RarityGem;

	// Token: 0x04005CA2 RID: 23714
	public GameObject m_Discription;

	// Token: 0x04005CA3 RID: 23715
	public GameObject m_Swords;

	// Token: 0x04005CA4 RID: 23716
	public GameObject m_Shield;
}
