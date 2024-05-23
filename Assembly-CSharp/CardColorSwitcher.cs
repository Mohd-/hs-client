using System;
using UnityEngine;

// Token: 0x02000290 RID: 656
public class CardColorSwitcher : MonoBehaviour
{
	// Token: 0x060023D2 RID: 9170 RVA: 0x000AFB5D File Offset: 0x000ADD5D
	private void Awake()
	{
		CardColorSwitcher.s_instance = this;
	}

	// Token: 0x060023D3 RID: 9171 RVA: 0x000AFB65 File Offset: 0x000ADD65
	private void OnDestroy()
	{
		CardColorSwitcher.s_instance = null;
	}

	// Token: 0x060023D4 RID: 9172 RVA: 0x000AFB6D File Offset: 0x000ADD6D
	public static CardColorSwitcher Get()
	{
		return CardColorSwitcher.s_instance;
	}

	// Token: 0x060023D5 RID: 9173 RVA: 0x000AFB74 File Offset: 0x000ADD74
	public Texture GetWeaponTexture(CardColorSwitcher.CardColorType colorType)
	{
		if ((CardColorSwitcher.CardColorType)this.weaponCardTextures.Length <= colorType)
		{
			return null;
		}
		if (this.weaponCardTextures[(int)colorType] == null)
		{
			return null;
		}
		return this.weaponCardTextures[(int)colorType];
	}

	// Token: 0x060023D6 RID: 9174 RVA: 0x000AFBB0 File Offset: 0x000ADDB0
	public Texture GetMinionTexture(CardColorSwitcher.CardColorType colorType)
	{
		if ((CardColorSwitcher.CardColorType)this.minionCardTextures.Length <= colorType)
		{
			return null;
		}
		if (this.minionCardTextures[(int)colorType] == null)
		{
			return null;
		}
		return this.minionCardTextures[(int)colorType];
	}

	// Token: 0x060023D7 RID: 9175 RVA: 0x000AFBEC File Offset: 0x000ADDEC
	public Texture GetSpellTexture(CardColorSwitcher.CardColorType colorType)
	{
		if ((CardColorSwitcher.CardColorType)this.spellCardTextures.Length <= colorType)
		{
			return null;
		}
		if (this.spellCardTextures[(int)colorType] == null)
		{
			return null;
		}
		return this.spellCardTextures[(int)colorType];
	}

	// Token: 0x060023D8 RID: 9176 RVA: 0x000AFC28 File Offset: 0x000ADE28
	public Texture GetPremiumMinionTexture(CardColorSwitcher.CardColorType colorType)
	{
		if ((CardColorSwitcher.CardColorType)this.premiumMinionCardTextures.Length <= colorType)
		{
			return null;
		}
		if (this.premiumMinionCardTextures[(int)colorType] == null)
		{
			return null;
		}
		return this.premiumMinionCardTextures[(int)colorType];
	}

	// Token: 0x040014F5 RID: 5365
	private static CardColorSwitcher s_instance;

	// Token: 0x040014F6 RID: 5366
	public Texture[] spellCardTextures;

	// Token: 0x040014F7 RID: 5367
	public Texture[] minionCardTextures;

	// Token: 0x040014F8 RID: 5368
	public Texture[] weaponCardTextures;

	// Token: 0x040014F9 RID: 5369
	public Texture[] premiumMinionCardTextures;

	// Token: 0x02000341 RID: 833
	public enum CardColorType
	{
		// Token: 0x04001A63 RID: 6755
		TYPE_GENERIC,
		// Token: 0x04001A64 RID: 6756
		TYPE_WARLOCK,
		// Token: 0x04001A65 RID: 6757
		TYPE_ROGUE,
		// Token: 0x04001A66 RID: 6758
		TYPE_DRUID,
		// Token: 0x04001A67 RID: 6759
		TYPE_SHAMAN,
		// Token: 0x04001A68 RID: 6760
		TYPE_HUNTER,
		// Token: 0x04001A69 RID: 6761
		TYPE_MAGE,
		// Token: 0x04001A6A RID: 6762
		TYPE_PALADIN,
		// Token: 0x04001A6B RID: 6763
		TYPE_PRIEST,
		// Token: 0x04001A6C RID: 6764
		TYPE_WARRIOR
	}
}
