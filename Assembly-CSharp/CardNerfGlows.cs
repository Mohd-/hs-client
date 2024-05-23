using System;
using UnityEngine;

// Token: 0x02000985 RID: 2437
public class CardNerfGlows : MonoBehaviour
{
	// Token: 0x06005839 RID: 22585 RVA: 0x001A6EE8 File Offset: 0x001A50E8
	private void Awake()
	{
		this.HideAll();
	}

	// Token: 0x0600583A RID: 22586 RVA: 0x001A6EF0 File Offset: 0x001A50F0
	public void SetGlowsForCard(CollectibleCard card)
	{
		this.HideAll();
		if (card.IsAttackChanged)
		{
			this.m_attack.SetActive(true);
		}
		if (card.IsCardInHandTextChanged)
		{
			this.m_cardText.SetActive(true);
		}
		if (card.IsHealthChanged)
		{
			this.m_health.SetActive(true);
		}
		if (card.IsManaCostChanged)
		{
			this.m_manaCost.SetActive(true);
		}
	}

	// Token: 0x0600583B RID: 22587 RVA: 0x001A6F60 File Offset: 0x001A5160
	private void HideAll()
	{
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			transform.gameObject.SetActive(false);
		}
	}

	// Token: 0x04003F83 RID: 16259
	public GameObject m_attack;

	// Token: 0x04003F84 RID: 16260
	public GameObject m_health;

	// Token: 0x04003F85 RID: 16261
	public GameObject m_manaCost;

	// Token: 0x04003F86 RID: 16262
	public GameObject m_rarityGem;

	// Token: 0x04003F87 RID: 16263
	public GameObject m_art;

	// Token: 0x04003F88 RID: 16264
	public GameObject m_cardText;

	// Token: 0x04003F89 RID: 16265
	public GameObject m_cardName;

	// Token: 0x04003F8A RID: 16266
	public GameObject m_race;
}
