using System;
using UnityEngine;

// Token: 0x020005BE RID: 1470
public class CardInfoPane : MonoBehaviour
{
	// Token: 0x0600419B RID: 16795 RVA: 0x0013C758 File Offset: 0x0013A958
	public void UpdateContent()
	{
		EntityDef entityDef;
		TAG_PREMIUM premium;
		if (!CraftingManager.Get().GetShownCardInfo(out entityDef, out premium))
		{
			return;
		}
		TAG_RARITY rarity = entityDef.GetRarity();
		TAG_CARD_SET cardSet = entityDef.GetCardSet();
		if (cardSet == TAG_CARD_SET.CORE)
		{
			this.m_rarityLabel.Text = string.Empty;
		}
		else
		{
			this.m_rarityLabel.Text = GameStrings.GetRarityText(rarity);
		}
		this.AssignRarityColors(rarity, cardSet);
		this.m_rarityGem.SetRarityGem(rarity, cardSet);
		this.m_setName.Text = GameStrings.GetCardSetName(cardSet);
		this.m_artistName.Text = GameStrings.Format("GLUE_COLLECTION_ARTIST", new object[]
		{
			entityDef.GetArtistName()
		});
		this.m_wildTheming.SetActive(GameUtils.IsCardRotated(entityDef));
		string text = "<color=#000000ff>" + entityDef.GetFlavorText() + "</color>";
		NetCache.CardValue cardValue = CraftingManager.Get().GetCardValue(entityDef.GetCardId(), premium);
		if (cardValue != null && cardValue.Nerfed)
		{
			if (!string.IsNullOrEmpty(text))
			{
				text += "\n\n";
			}
			text += GameStrings.Get("GLUE_COLLECTION_RECENTLY_NERFED");
		}
		this.m_flavorText.Text = text;
	}

	// Token: 0x0600419C RID: 16796 RVA: 0x0013C888 File Offset: 0x0013AA88
	private void AssignRarityColors(TAG_RARITY rarity, TAG_CARD_SET cardSet)
	{
		if (cardSet == TAG_CARD_SET.CORE)
		{
			this.m_rarityLabel.TextColor = new Color(0.53f, 0.52f, 0.51f, 1f);
			return;
		}
		switch (rarity)
		{
		default:
			this.m_rarityLabel.TextColor = Color.white;
			break;
		case TAG_RARITY.RARE:
			this.m_rarityLabel.TextColor = new Color(0.11f, 0.33f, 0.8f, 1f);
			break;
		case TAG_RARITY.EPIC:
			this.m_rarityLabel.TextColor = new Color(0.77f, 0.03f, 1f, 1f);
			break;
		case TAG_RARITY.LEGENDARY:
			this.m_rarityLabel.TextColor = new Color(1f, 0.56f, 0f, 1f);
			break;
		}
	}

	// Token: 0x040029C7 RID: 10695
	public UberText m_artistName;

	// Token: 0x040029C8 RID: 10696
	public UberText m_rarityLabel;

	// Token: 0x040029C9 RID: 10697
	public UberText m_flavorText;

	// Token: 0x040029CA RID: 10698
	public UberText m_setName;

	// Token: 0x040029CB RID: 10699
	public RarityGem m_rarityGem;

	// Token: 0x040029CC RID: 10700
	public GameObject m_wildTheming;
}
