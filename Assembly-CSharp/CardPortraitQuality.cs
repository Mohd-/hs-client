using System;

// Token: 0x02000326 RID: 806
public class CardPortraitQuality
{
	// Token: 0x060029FB RID: 10747 RVA: 0x000CDAF3 File Offset: 0x000CBCF3
	public CardPortraitQuality(int quality, bool loadPremium)
	{
		this.TextureQuality = quality;
		this.LoadPremium = loadPremium;
	}

	// Token: 0x060029FC RID: 10748 RVA: 0x000CDB09 File Offset: 0x000CBD09
	public CardPortraitQuality(int quality, TAG_PREMIUM premiumType)
	{
		this.TextureQuality = quality;
		this.LoadPremium = (premiumType == TAG_PREMIUM.GOLDEN);
	}

	// Token: 0x060029FD RID: 10749 RVA: 0x000CDB22 File Offset: 0x000CBD22
	public static CardPortraitQuality GetUnloaded()
	{
		return new CardPortraitQuality(0, false);
	}

	// Token: 0x060029FE RID: 10750 RVA: 0x000CDB2B File Offset: 0x000CBD2B
	public static CardPortraitQuality GetDefault()
	{
		return new CardPortraitQuality(3, true);
	}

	// Token: 0x060029FF RID: 10751 RVA: 0x000CDB34 File Offset: 0x000CBD34
	public static CardPortraitQuality GetFromDef(CardDef def)
	{
		if (def == null)
		{
			return CardPortraitQuality.GetDefault();
		}
		return def.GetPortraitQuality();
	}

	// Token: 0x06002A00 RID: 10752 RVA: 0x000CDB50 File Offset: 0x000CBD50
	public override string ToString()
	{
		return string.Concat(new object[]
		{
			"(",
			this.TextureQuality,
			", ",
			this.LoadPremium,
			")"
		});
	}

	// Token: 0x06002A01 RID: 10753 RVA: 0x000CDB9C File Offset: 0x000CBD9C
	public static bool operator >=(CardPortraitQuality left, CardPortraitQuality right)
	{
		return left != null && (right == null || (left.TextureQuality >= right.TextureQuality && (left.LoadPremium || !right.LoadPremium)));
	}

	// Token: 0x06002A02 RID: 10754 RVA: 0x000CDBDC File Offset: 0x000CBDDC
	public static bool operator <=(CardPortraitQuality left, CardPortraitQuality right)
	{
		return left == null || (right != null && left.TextureQuality <= right.TextureQuality && (!left.LoadPremium || right.LoadPremium));
	}

	// Token: 0x0400186E RID: 6254
	public const int NOT_LOADED = 0;

	// Token: 0x0400186F RID: 6255
	public const int LOW = 1;

	// Token: 0x04001870 RID: 6256
	public const int MEDIUM = 2;

	// Token: 0x04001871 RID: 6257
	public const int HIGH = 3;

	// Token: 0x04001872 RID: 6258
	public int TextureQuality;

	// Token: 0x04001873 RID: 6259
	public bool LoadPremium;
}
