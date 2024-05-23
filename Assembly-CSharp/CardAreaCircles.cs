using System;
using UnityEngine;

// Token: 0x02000F69 RID: 3945
public class CardAreaCircles : MonoBehaviour
{
	// Token: 0x06007517 RID: 29975 RVA: 0x0022913F File Offset: 0x0022733F
	private void Awake()
	{
		this.HideAll();
	}

	// Token: 0x06007518 RID: 29976 RVA: 0x00229148 File Offset: 0x00227348
	public void SetCirclesForCard(CollectibleCard card)
	{
		this.HideAll();
		if (card.IsAttackChanged)
		{
			this.m_attackGem.SetActive(true);
		}
		if (card.IsCardInHandTextChanged)
		{
			this.m_description.SetActive(true);
		}
		if (card.IsHealthChanged)
		{
			this.m_healthGem.SetActive(true);
		}
		if (card.IsManaCostChanged)
		{
			this.m_manaGem.SetActive(true);
		}
	}

	// Token: 0x06007519 RID: 29977 RVA: 0x002291B8 File Offset: 0x002273B8
	private void HideAll()
	{
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			transform.gameObject.SetActive(false);
		}
	}

	// Token: 0x04005FA0 RID: 24480
	public GameObject m_attackGem;

	// Token: 0x04005FA1 RID: 24481
	public GameObject m_description;

	// Token: 0x04005FA2 RID: 24482
	public GameObject m_healthGem;

	// Token: 0x04005FA3 RID: 24483
	public GameObject m_manaGem;

	// Token: 0x04005FA4 RID: 24484
	public GameObject m_minionPortrait;

	// Token: 0x04005FA5 RID: 24485
	public GameObject m_minionRace;

	// Token: 0x04005FA6 RID: 24486
	public GameObject m_nameBanner;

	// Token: 0x04005FA7 RID: 24487
	public GameObject m_rarityGem;
}
