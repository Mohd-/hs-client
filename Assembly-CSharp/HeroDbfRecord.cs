using System;
using System.Collections.Generic;

// Token: 0x02000145 RID: 325
public class HeroDbfRecord : DbfRecord
{
	// Token: 0x170002D7 RID: 727
	// (get) Token: 0x060010FD RID: 4349 RVA: 0x00049246 File Offset: 0x00047446
	[DbfField("CARD_ID", "the ASSET.CARD.MINI_GUID, which is also the client folder")]
	public string CardId
	{
		get
		{
			return this.m_CardId;
		}
	}

	// Token: 0x170002D8 RID: 728
	// (get) Token: 0x060010FE RID: 4350 RVA: 0x0004924E File Offset: 0x0004744E
	[DbfField("CARD_BACK_ID", "ASSET.CARD_BACK.ID")]
	public int CardBackId
	{
		get
		{
			return this.m_CardBackId;
		}
	}

	// Token: 0x170002D9 RID: 729
	// (get) Token: 0x060010FF RID: 4351 RVA: 0x00049256 File Offset: 0x00047456
	[DbfField("DESCRIPTION", "this is the description that shows up when you look at the hero in Collection Manager heroes tab.")]
	public DbfLocValue Description
	{
		get
		{
			return this.m_Description;
		}
	}

	// Token: 0x170002DA RID: 730
	// (get) Token: 0x06001100 RID: 4352 RVA: 0x0004925E File Offset: 0x0004745E
	[DbfField("HERODEF_ASSET_PATH", "")]
	public string HerodefAssetPath
	{
		get
		{
			return this.m_HerodefAssetPath;
		}
	}

	// Token: 0x170002DB RID: 731
	// (get) Token: 0x06001101 RID: 4353 RVA: 0x00049266 File Offset: 0x00047466
	[DbfField("STORE_DESCRIPTION", "description of the hero for purchase on store")]
	public DbfLocValue StoreDescription
	{
		get
		{
			return this.m_StoreDescription;
		}
	}

	// Token: 0x170002DC RID: 732
	// (get) Token: 0x06001102 RID: 4354 RVA: 0x0004926E File Offset: 0x0004746E
	[DbfField("STORE_DESCRIPTION_PHONE", "description specifically for smaller screen phone UI")]
	public DbfLocValue StoreDescriptionPhone
	{
		get
		{
			return this.m_StoreDescriptionPhone;
		}
	}

	// Token: 0x170002DD RID: 733
	// (get) Token: 0x06001103 RID: 4355 RVA: 0x00049276 File Offset: 0x00047476
	[DbfField("STORE_BANNER_PREFAB", "if non-null, a prefab to be instantiated and attached to the GeneralStoreHeroesContentDisplay at localPosition (0, 0, 0) - used for adding additional flare to the hero purchase experience.")]
	public string StoreBannerPrefab
	{
		get
		{
			return this.m_StoreBannerPrefab;
		}
	}

	// Token: 0x170002DE RID: 734
	// (get) Token: 0x06001104 RID: 4356 RVA: 0x0004927E File Offset: 0x0004747E
	[DbfField("STORE_BACKGROUND_TEXTURE", "if null, uses the default purple background border around the hero frame in store. if non-null, specifies an alternative texture to be used.")]
	public string StoreBackgroundTexture
	{
		get
		{
			return this.m_StoreBackgroundTexture;
		}
	}

	// Token: 0x170002DF RID: 735
	// (get) Token: 0x06001105 RID: 4357 RVA: 0x00049286 File Offset: 0x00047486
	[DbfField("STORE_SORT_ORDER", "sort ordering when displayed in the store (ascending, as in 0 is first, then 1, etc)")]
	public int StoreSortOrder
	{
		get
		{
			return this.m_StoreSortOrder;
		}
	}

	// Token: 0x170002E0 RID: 736
	// (get) Token: 0x06001106 RID: 4358 RVA: 0x0004928E File Offset: 0x0004748E
	[DbfField("PURCHASE_COMPLETE_MSG", "A 'thank you' message displayed to the player after a successful purchase.")]
	public DbfLocValue PurchaseCompleteMsg
	{
		get
		{
			return this.m_PurchaseCompleteMsg;
		}
	}

	// Token: 0x06001107 RID: 4359 RVA: 0x00049296 File Offset: 0x00047496
	public void SetCardId(string v)
	{
		this.m_CardId = v;
	}

	// Token: 0x06001108 RID: 4360 RVA: 0x0004929F File Offset: 0x0004749F
	public void SetCardBackId(int v)
	{
		this.m_CardBackId = v;
	}

	// Token: 0x06001109 RID: 4361 RVA: 0x000492A8 File Offset: 0x000474A8
	public void SetDescription(DbfLocValue v)
	{
		this.m_Description = v;
		v.SetDebugInfo(base.ID, "DESCRIPTION");
	}

	// Token: 0x0600110A RID: 4362 RVA: 0x000492C2 File Offset: 0x000474C2
	public void SetHerodefAssetPath(string v)
	{
		this.m_HerodefAssetPath = v;
	}

	// Token: 0x0600110B RID: 4363 RVA: 0x000492CB File Offset: 0x000474CB
	public void SetStoreDescription(DbfLocValue v)
	{
		this.m_StoreDescription = v;
		v.SetDebugInfo(base.ID, "STORE_DESCRIPTION");
	}

	// Token: 0x0600110C RID: 4364 RVA: 0x000492E5 File Offset: 0x000474E5
	public void SetStoreDescriptionPhone(DbfLocValue v)
	{
		this.m_StoreDescriptionPhone = v;
		v.SetDebugInfo(base.ID, "STORE_DESCRIPTION_PHONE");
	}

	// Token: 0x0600110D RID: 4365 RVA: 0x000492FF File Offset: 0x000474FF
	public void SetStoreBannerPrefab(string v)
	{
		this.m_StoreBannerPrefab = v;
	}

	// Token: 0x0600110E RID: 4366 RVA: 0x00049308 File Offset: 0x00047508
	public void SetStoreBackgroundTexture(string v)
	{
		this.m_StoreBackgroundTexture = v;
	}

	// Token: 0x0600110F RID: 4367 RVA: 0x00049311 File Offset: 0x00047511
	public void SetStoreSortOrder(int v)
	{
		this.m_StoreSortOrder = v;
	}

	// Token: 0x06001110 RID: 4368 RVA: 0x0004931A File Offset: 0x0004751A
	public void SetPurchaseCompleteMsg(DbfLocValue v)
	{
		this.m_PurchaseCompleteMsg = v;
		v.SetDebugInfo(base.ID, "PURCHASE_COMPLETE_MSG");
	}

	// Token: 0x06001111 RID: 4369 RVA: 0x00049334 File Offset: 0x00047534
	public override object GetVar(string name)
	{
		if (name != null)
		{
			if (HeroDbfRecord.<>f__switch$map3A == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(11);
				dictionary.Add("ID", 0);
				dictionary.Add("CARD_ID", 1);
				dictionary.Add("CARD_BACK_ID", 2);
				dictionary.Add("DESCRIPTION", 3);
				dictionary.Add("HERODEF_ASSET_PATH", 4);
				dictionary.Add("STORE_DESCRIPTION", 5);
				dictionary.Add("STORE_DESCRIPTION_PHONE", 6);
				dictionary.Add("STORE_BANNER_PREFAB", 7);
				dictionary.Add("STORE_BACKGROUND_TEXTURE", 8);
				dictionary.Add("STORE_SORT_ORDER", 9);
				dictionary.Add("PURCHASE_COMPLETE_MSG", 10);
				HeroDbfRecord.<>f__switch$map3A = dictionary;
			}
			int num;
			if (HeroDbfRecord.<>f__switch$map3A.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return base.ID;
				case 1:
					return this.CardId;
				case 2:
					return this.CardBackId;
				case 3:
					return this.Description;
				case 4:
					return this.HerodefAssetPath;
				case 5:
					return this.StoreDescription;
				case 6:
					return this.StoreDescriptionPhone;
				case 7:
					return this.StoreBannerPrefab;
				case 8:
					return this.StoreBackgroundTexture;
				case 9:
					return this.StoreSortOrder;
				case 10:
					return this.PurchaseCompleteMsg;
				}
			}
		}
		return null;
	}

	// Token: 0x06001112 RID: 4370 RVA: 0x00049490 File Offset: 0x00047690
	public override void SetVar(string name, object val)
	{
		if (name != null)
		{
			if (HeroDbfRecord.<>f__switch$map3B == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(11);
				dictionary.Add("ID", 0);
				dictionary.Add("CARD_ID", 1);
				dictionary.Add("CARD_BACK_ID", 2);
				dictionary.Add("DESCRIPTION", 3);
				dictionary.Add("HERODEF_ASSET_PATH", 4);
				dictionary.Add("STORE_DESCRIPTION", 5);
				dictionary.Add("STORE_DESCRIPTION_PHONE", 6);
				dictionary.Add("STORE_BANNER_PREFAB", 7);
				dictionary.Add("STORE_BACKGROUND_TEXTURE", 8);
				dictionary.Add("STORE_SORT_ORDER", 9);
				dictionary.Add("PURCHASE_COMPLETE_MSG", 10);
				HeroDbfRecord.<>f__switch$map3B = dictionary;
			}
			int num;
			if (HeroDbfRecord.<>f__switch$map3B.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					base.SetID((int)val);
					break;
				case 1:
					this.SetCardId((string)val);
					break;
				case 2:
					this.SetCardBackId((int)val);
					break;
				case 3:
					this.SetDescription((DbfLocValue)val);
					break;
				case 4:
					this.SetHerodefAssetPath((string)val);
					break;
				case 5:
					this.SetStoreDescription((DbfLocValue)val);
					break;
				case 6:
					this.SetStoreDescriptionPhone((DbfLocValue)val);
					break;
				case 7:
					this.SetStoreBannerPrefab((string)val);
					break;
				case 8:
					this.SetStoreBackgroundTexture((string)val);
					break;
				case 9:
					this.SetStoreSortOrder((int)val);
					break;
				case 10:
					this.SetPurchaseCompleteMsg((DbfLocValue)val);
					break;
				}
			}
		}
	}

	// Token: 0x06001113 RID: 4371 RVA: 0x00049648 File Offset: 0x00047848
	public override Type GetVarType(string name)
	{
		if (name != null)
		{
			if (HeroDbfRecord.<>f__switch$map3C == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(11);
				dictionary.Add("ID", 0);
				dictionary.Add("CARD_ID", 1);
				dictionary.Add("CARD_BACK_ID", 2);
				dictionary.Add("DESCRIPTION", 3);
				dictionary.Add("HERODEF_ASSET_PATH", 4);
				dictionary.Add("STORE_DESCRIPTION", 5);
				dictionary.Add("STORE_DESCRIPTION_PHONE", 6);
				dictionary.Add("STORE_BANNER_PREFAB", 7);
				dictionary.Add("STORE_BACKGROUND_TEXTURE", 8);
				dictionary.Add("STORE_SORT_ORDER", 9);
				dictionary.Add("PURCHASE_COMPLETE_MSG", 10);
				HeroDbfRecord.<>f__switch$map3C = dictionary;
			}
			int num;
			if (HeroDbfRecord.<>f__switch$map3C.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return typeof(int);
				case 1:
					return typeof(string);
				case 2:
					return typeof(int);
				case 3:
					return typeof(DbfLocValue);
				case 4:
					return typeof(string);
				case 5:
					return typeof(DbfLocValue);
				case 6:
					return typeof(DbfLocValue);
				case 7:
					return typeof(string);
				case 8:
					return typeof(string);
				case 9:
					return typeof(int);
				case 10:
					return typeof(DbfLocValue);
				}
			}
		}
		return null;
	}

	// Token: 0x0400090B RID: 2315
	private string m_CardId;

	// Token: 0x0400090C RID: 2316
	private int m_CardBackId;

	// Token: 0x0400090D RID: 2317
	private DbfLocValue m_Description;

	// Token: 0x0400090E RID: 2318
	private string m_HerodefAssetPath;

	// Token: 0x0400090F RID: 2319
	private DbfLocValue m_StoreDescription;

	// Token: 0x04000910 RID: 2320
	private DbfLocValue m_StoreDescriptionPhone;

	// Token: 0x04000911 RID: 2321
	private string m_StoreBannerPrefab;

	// Token: 0x04000912 RID: 2322
	private string m_StoreBackgroundTexture;

	// Token: 0x04000913 RID: 2323
	private int m_StoreSortOrder;

	// Token: 0x04000914 RID: 2324
	private DbfLocValue m_PurchaseCompleteMsg;
}
