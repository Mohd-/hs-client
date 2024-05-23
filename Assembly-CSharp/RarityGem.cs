using System;
using UnityEngine;

// Token: 0x020006F3 RID: 1779
public class RarityGem : MonoBehaviour
{
	// Token: 0x0600495A RID: 18778 RVA: 0x0015EB60 File Offset: 0x0015CD60
	public void SetRarityGem(TAG_RARITY rarity, TAG_CARD_SET cardSet)
	{
		if (cardSet == TAG_CARD_SET.CORE)
		{
			base.GetComponent<Renderer>().enabled = false;
			return;
		}
		base.GetComponent<Renderer>().enabled = true;
		switch (rarity)
		{
		default:
			base.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0f, 0f);
			break;
		case TAG_RARITY.RARE:
			base.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0.118f, 0f);
			break;
		case TAG_RARITY.EPIC:
			base.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0.239f, 0f);
			break;
		case TAG_RARITY.LEGENDARY:
			base.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0.3575f, 0f);
			break;
		}
	}
}
