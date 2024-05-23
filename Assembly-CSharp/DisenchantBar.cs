using System;
using UnityEngine;

// Token: 0x020007A9 RID: 1961
[Serializable]
public class DisenchantBar
{
	// Token: 0x06004D43 RID: 19779 RVA: 0x0016FF50 File Offset: 0x0016E150
	public void Reset()
	{
		this.m_numCards = 0;
		this.m_amount = 0;
		this.m_numGoldCards = 0;
	}

	// Token: 0x06004D44 RID: 19780 RVA: 0x0016FF68 File Offset: 0x0016E168
	public void AddCards(int count, int sellAmount, TAG_PREMIUM premiumType)
	{
		this.m_numCards += count;
		if (this.m_premiumType != premiumType)
		{
			this.m_numGoldCards += count;
		}
		this.m_amount += sellAmount;
	}

	// Token: 0x06004D45 RID: 19781 RVA: 0x0016FFAC File Offset: 0x0016E1AC
	public void Init()
	{
		if (this.m_typeText != null)
		{
			string rarityText = GameStrings.GetRarityText(this.m_rarity);
			this.m_typeText.Text = ((this.m_premiumType != TAG_PREMIUM.GOLDEN) ? rarityText : GameStrings.Format("GLUE_MASS_DISENCHANT_PREMIUM_TITLE", new object[]
			{
				rarityText
			}));
		}
	}

	// Token: 0x06004D46 RID: 19782 RVA: 0x00170007 File Offset: 0x0016E207
	public int GetNumCards()
	{
		return this.m_numCards;
	}

	// Token: 0x06004D47 RID: 19783 RVA: 0x0017000F File Offset: 0x0016E20F
	public int GetAmountDust()
	{
		return this.m_amount;
	}

	// Token: 0x06004D48 RID: 19784 RVA: 0x00170018 File Offset: 0x0016E218
	public void UpdateVisuals(int totalNumCards)
	{
		this.m_numCardsText.Text = this.m_numCards.ToString();
		this.m_amountText.Text = this.m_amount.ToString();
		if (this.m_numGoldText != null)
		{
			if (this.m_numGoldCards > 0)
			{
				this.m_numGoldText.gameObject.SetActive(true);
				this.m_numGoldText.Text = GameStrings.Format("GLUE_MASS_DISENCHANT_NUM_GOLDEN_CARDS", new object[]
				{
					this.m_numGoldCards.ToString()
				});
				TransformUtil.SetLocalPosX(this.m_numCardsText, 7.638979f);
				MeshFilter component = this.m_barFrameMesh.GetComponent<MeshFilter>();
				component.mesh = MassDisenchant.Get().m_rarityBarGoldMesh;
				this.m_barFrameMesh.material = MassDisenchant.Get().m_rarityBarGoldMaterial;
			}
			else
			{
				this.m_numGoldText.gameObject.SetActive(false);
				TransformUtil.SetLocalPosX(this.m_numCardsText, 2.902672f);
				MeshFilter component2 = this.m_barFrameMesh.GetComponent<MeshFilter>();
				component2.mesh = MassDisenchant.Get().m_rarityBarNormalMesh;
				this.m_barFrameMesh.material = MassDisenchant.Get().m_rarityBarNormalMaterial;
			}
		}
		float num = ((float)totalNumCards <= 0f) ? 0f : ((float)this.m_numCards / (float)totalNumCards);
		this.m_amountBar.GetComponent<Renderer>().material.SetFloat("_Percent", num);
	}

	// Token: 0x04003455 RID: 13397
	public const float NUM_CARDS_TEXT_CENTER_X = 2.902672f;

	// Token: 0x04003456 RID: 13398
	public const float NUM_CARDS_TEXT_OFFSET_X = 7.638979f;

	// Token: 0x04003457 RID: 13399
	public TAG_PREMIUM m_premiumType;

	// Token: 0x04003458 RID: 13400
	public TAG_RARITY m_rarity;

	// Token: 0x04003459 RID: 13401
	public UberText m_typeText;

	// Token: 0x0400345A RID: 13402
	public UberText m_amountText;

	// Token: 0x0400345B RID: 13403
	public UberText m_numCardsText;

	// Token: 0x0400345C RID: 13404
	public GameObject m_amountBar;

	// Token: 0x0400345D RID: 13405
	public GameObject m_dustJar;

	// Token: 0x0400345E RID: 13406
	public GameObject m_rarityGem;

	// Token: 0x0400345F RID: 13407
	public GameObject m_glow;

	// Token: 0x04003460 RID: 13408
	public UberText m_numGoldText;

	// Token: 0x04003461 RID: 13409
	public MeshRenderer m_barFrameMesh;

	// Token: 0x04003462 RID: 13410
	private int m_numCards;

	// Token: 0x04003463 RID: 13411
	private int m_amount;

	// Token: 0x04003464 RID: 13412
	private int m_numGoldCards;
}
