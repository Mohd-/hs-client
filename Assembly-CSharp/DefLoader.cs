using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200014E RID: 334
public class DefLoader
{
	// Token: 0x060011C4 RID: 4548 RVA: 0x0004CE40 File Offset: 0x0004B040
	public static DefLoader Get()
	{
		if (DefLoader.s_instance == null)
		{
			DefLoader.s_instance = new DefLoader();
			if (ApplicationMgr.Get() != null)
			{
				ApplicationMgr.Get().WillReset += new Action(DefLoader.s_instance.WillReset);
			}
			if (SceneMgr.Get() != null)
			{
				SceneMgr.Get().RegisterSceneUnloadedEvent(new SceneMgr.SceneUnloadedCallback(DefLoader.s_instance.OnSceneUnloaded));
			}
		}
		return DefLoader.s_instance;
	}

	// Token: 0x060011C5 RID: 4549 RVA: 0x0004CEBA File Offset: 0x0004B0BA
	public void Initialize()
	{
		this.LoadAllEntityDefs();
	}

	// Token: 0x060011C6 RID: 4550 RVA: 0x0004CEC2 File Offset: 0x0004B0C2
	public void Clear()
	{
		this.ClearEntityDefs();
		this.ClearCardDefs();
	}

	// Token: 0x060011C7 RID: 4551 RVA: 0x0004CED0 File Offset: 0x0004B0D0
	public bool HasDef(GameObject go)
	{
		return this.HasCardDef(go);
	}

	// Token: 0x060011C8 RID: 4552 RVA: 0x0004CEE1 File Offset: 0x0004B0E1
	public Map<string, EntityDef> GetAllEntityDefs()
	{
		return this.m_entityDefCache;
	}

	// Token: 0x060011C9 RID: 4553 RVA: 0x0004CEE9 File Offset: 0x0004B0E9
	public void ClearEntityDefs()
	{
		this.m_entityDefCache.Clear();
		this.m_loadedEntityDefs = false;
	}

	// Token: 0x060011CA RID: 4554 RVA: 0x0004CF00 File Offset: 0x0004B100
	public EntityDef GetEntityDef(string cardId)
	{
		if (string.IsNullOrEmpty(cardId))
		{
			return null;
		}
		EntityDef entityDef = null;
		this.m_entityDefCache.TryGetValue(cardId, out entityDef);
		if (entityDef == null)
		{
			if (ApplicationMgr.UseDevWorkarounds())
			{
				Debug.LogErrorFormat("DefLoader.GetEntityDef() - Failed to load {0}. Loading {1} instead.", new object[]
				{
					cardId,
					"PlaceholderCard"
				});
				EntityDef entityDef2;
				this.m_entityDefCache.TryGetValue("PlaceholderCard", out entityDef2);
				if (entityDef2 == null)
				{
					Error.AddDevFatal("DefLoader.GetEntityDef() - Failed to load {0} in place of {1}", new object[]
					{
						"PlaceholderCard",
						cardId
					});
					return null;
				}
				entityDef = entityDef2.Clone();
				entityDef.SetCardId(cardId);
				this.m_entityDefCache[cardId] = entityDef;
			}
			else
			{
				Error.AddDevFatal("DefLoader.GetEntityDef() - Failed to load {0}", new object[]
				{
					cardId
				});
			}
		}
		return entityDef;
	}

	// Token: 0x060011CB RID: 4555 RVA: 0x0004CFC4 File Offset: 0x0004B1C4
	public EntityDef GetEntityDef(int dbId)
	{
		string text = GameUtils.TranslateDbIdToCardId(dbId);
		if (text == null)
		{
			Debug.LogErrorFormat("DefLoader.GetEntityDef() - dbId {0} does not map to a cardId", new object[]
			{
				dbId
			});
			return null;
		}
		return this.GetEntityDef(text);
	}

	// Token: 0x060011CC RID: 4556 RVA: 0x0004D000 File Offset: 0x0004B200
	public void LoadAllEntityDefs()
	{
		int num = 0;
		List<string> allCardIds = GameUtils.GetAllCardIds();
		this.m_entityDefCache = AssetLoader.Get().LoadBatchCardXmls(allCardIds, out num);
		this.m_loadedEntityDefs = true;
		if (num > 0)
		{
			Error.AddDevWarning("Missing Cards", "Failed to load {0} card(s) on startup.  Proceed with caution.  Check errors in the console for more details.", new object[]
			{
				num
			});
		}
	}

	// Token: 0x060011CD RID: 4557 RVA: 0x0004D054 File Offset: 0x0004B254
	public bool HasLoadedEntityDefs()
	{
		return this.m_loadedEntityDefs;
	}

	// Token: 0x060011CE RID: 4558 RVA: 0x0004D05C File Offset: 0x0004B25C
	public bool HasCardDef(GameObject go)
	{
		CardDef cardDef = SceneUtils.FindComponentInThisOrParents<CardDef>(go);
		return !(cardDef == null) && this.m_cachedCardDefs.ContainsValue(cardDef);
	}

	// Token: 0x060011CF RID: 4559 RVA: 0x0004D08C File Offset: 0x0004B28C
	public void ClearCardDef(string cardID)
	{
		if (this.m_cachedCardDefs.ContainsKey(cardID))
		{
			CardDef cardDef = this.m_cachedCardDefs[cardID];
			this.m_cachedCardDefs.Remove(cardID);
			Object.Destroy(cardDef.gameObject);
		}
	}

	// Token: 0x060011D0 RID: 4560 RVA: 0x0004D0D0 File Offset: 0x0004B2D0
	public void ClearCardDefs()
	{
		if (this.m_cachedCardDefs == null)
		{
			return;
		}
		foreach (CardDef cardDef in this.m_cachedCardDefs.Values)
		{
			if (cardDef != null && cardDef.gameObject != null)
			{
				Object.Destroy(cardDef.gameObject);
			}
		}
		this.m_cachedCardDefs.Clear();
	}

	// Token: 0x060011D1 RID: 4561 RVA: 0x0004D168 File Offset: 0x0004B368
	public void LoadCardDef(string cardId, DefLoader.LoadDefCallback<CardDef> callback, object userData = null, CardPortraitQuality quality = null)
	{
		CardDef cardDef = this.GetCardDef(cardId, quality);
		callback(cardId, cardDef, userData);
	}

	// Token: 0x060011D2 RID: 4562 RVA: 0x0004D188 File Offset: 0x0004B388
	public CardDef GetCardDef(int dbId)
	{
		string text = GameUtils.TranslateDbIdToCardId(dbId);
		if (text == null)
		{
			Debug.LogError(string.Format("DefLoader.GetCardDef() - dbId {0} does not map to a cardId", dbId));
			return null;
		}
		return this.GetCardDef(text, null);
	}

	// Token: 0x060011D3 RID: 4563 RVA: 0x0004D1C4 File Offset: 0x0004B3C4
	public CardDef GetCardDef(string cardId, CardPortraitQuality quality = null)
	{
		if (string.IsNullOrEmpty(cardId))
		{
			return null;
		}
		if (quality == null)
		{
			quality = CardPortraitQuality.GetDefault();
		}
		bool flag = true;
		if (flag)
		{
			quality.TextureQuality = 3;
		}
		CardDef cardDef = null;
		CardDef cardDef2;
		if (this.m_cachedCardDefs.TryGetValue(cardId, out cardDef2))
		{
			cardDef = cardDef2;
			CardPortraitQuality fromDef = CardPortraitQuality.GetFromDef(cardDef2);
			if (fromDef >= quality)
			{
				return cardDef2;
			}
		}
		if (cardDef == null)
		{
			GameObject gameObject = AssetLoader.Get().LoadCardPrefab(cardId, true, false);
			if (gameObject)
			{
				cardDef = gameObject.GetComponent<CardDef>();
			}
			if (cardDef == null)
			{
				if (gameObject)
				{
					Object.Destroy(gameObject);
				}
				if (!ApplicationMgr.UseDevWorkarounds())
				{
					Error.AddDevFatal("DefLoader.GetCardDef() - Failed to load {0}", new object[]
					{
						cardId
					});
					return null;
				}
				Debug.LogErrorFormat("DefLoader.GetCardDef() - Failed to load {0}. Loading {1} instead.", new object[]
				{
					cardId,
					"PlaceholderCard"
				});
				gameObject = this.LoadPlaceholderCardPrefab();
				if (gameObject == null)
				{
					Error.AddDevFatal("DefLoader.GetCardDef() - Failed to load {0} in place of {1}", new object[]
					{
						"PlaceholderCard",
						cardId
					});
					return null;
				}
				cardDef = gameObject.GetComponent<CardDef>();
			}
			this.m_cachedCardDefs.Add(cardId, cardDef);
		}
		this.UpdateCardAssets(cardDef, quality);
		return cardDef;
	}

	// Token: 0x060011D4 RID: 4564 RVA: 0x0004D308 File Offset: 0x0004B508
	private GameObject LoadPlaceholderCardPrefab()
	{
		if (this.m_placeholderCardPrefab != null)
		{
			return this.m_placeholderCardPrefab;
		}
		this.m_placeholderCardPrefab = AssetLoader.Get().LoadCardPrefab("PlaceholderCard", true, false);
		if (this.m_placeholderCardPrefab == null)
		{
			Debug.LogErrorFormat("DefLoader.LoadPlaceholderCardPrefab() - Failed to load {0}", new object[]
			{
				"PlaceholderCard"
			});
			return null;
		}
		return this.m_placeholderCardPrefab;
	}

	// Token: 0x060011D5 RID: 4565 RVA: 0x0004D378 File Offset: 0x0004B578
	private static string GetTextureName(string path, int quality)
	{
		switch (quality)
		{
		case 1:
		case 2:
		{
			int num = path.LastIndexOf('/');
			string text = path.Substring(0, num);
			string text2 = path.Substring(num + 1);
			return string.Format("{0}/LowResPortrait/{1}", text, text2);
		}
		case 3:
			return path;
		default:
			Debug.LogError("Invalid texture quality value.");
			return string.Empty;
		}
	}

	// Token: 0x060011D6 RID: 4566 RVA: 0x0004D3DC File Offset: 0x0004B5DC
	private void UpdateCardAssets(CardDef cardDef, CardPortraitQuality quality)
	{
		CardPortraitQuality portraitQuality = cardDef.GetPortraitQuality();
		if (quality <= portraitQuality || string.IsNullOrEmpty(cardDef.m_PortraitTexturePath))
		{
			return;
		}
		if (portraitQuality.TextureQuality < quality.TextureQuality)
		{
			string textureName = DefLoader.GetTextureName(cardDef.m_PortraitTexturePath, quality.TextureQuality);
			Texture texture = AssetLoader.Get().LoadCardTexture(textureName, false);
			if (texture == null)
			{
				Error.AddDevFatal("DefLoader.UpdateCardTextures() - Failed to load {0} for card {1}", new object[]
				{
					cardDef.m_PortraitTexturePath,
					cardDef
				});
				return;
			}
			cardDef.OnPortraitLoaded(texture, quality.TextureQuality);
		}
		if (((quality.LoadPremium && !portraitQuality.LoadPremium) || cardDef.m_AlwaysRenderPremiumPortrait) && !string.IsNullOrEmpty(cardDef.m_PremiumPortraitMaterialPath))
		{
			Material material = AssetLoader.Get().LoadPremiumMaterial(cardDef.m_PremiumPortraitMaterialPath, false);
			Texture texture2 = null;
			if (material == null)
			{
				Error.AddDevFatal("DefLoader.UpdateCardTextures() - Failed to load {0} for card {1}", new object[]
				{
					cardDef.m_PremiumPortraitMaterialPath,
					cardDef
				});
				return;
			}
			if (!string.IsNullOrEmpty(cardDef.m_PremiumPortraitTexturePath))
			{
				texture2 = AssetLoader.Get().LoadCardTexture(cardDef.m_PremiumPortraitTexturePath, false);
				if (texture2 == null)
				{
					Error.AddDevFatal("DefLoader.UpdateCardTextures() - Failed to load {0} for card {1}", new object[]
					{
						cardDef.m_PremiumPortraitTexturePath,
						cardDef
					});
					return;
				}
			}
			cardDef.OnPremiumMaterialLoaded(material, texture2);
		}
	}

	// Token: 0x060011D7 RID: 4567 RVA: 0x0004D53B File Offset: 0x0004B73B
	private void WillReset()
	{
		this.ClearEntityDefs();
	}

	// Token: 0x060011D8 RID: 4568 RVA: 0x0004D543 File Offset: 0x0004B743
	private void OnSceneUnloaded(SceneMgr.Mode prevMode, Scene prevScene, object userData)
	{
		this.ClearCardDefs();
	}

	// Token: 0x060011D9 RID: 4569 RVA: 0x0004D54B File Offset: 0x0004B74B
	public void LoadFullDef(string cardId, DefLoader.LoadDefCallback<FullDef> callback)
	{
		this.LoadFullDef(cardId, callback, null);
	}

	// Token: 0x060011DA RID: 4570 RVA: 0x0004D558 File Offset: 0x0004B758
	public FullDef GetFullDef(string cardId, CardPortraitQuality quality = null)
	{
		EntityDef entityDef = this.GetEntityDef(cardId);
		CardDef cardDef = this.GetCardDef(cardId, quality);
		FullDef fullDef = new FullDef();
		fullDef.SetEntityDef(entityDef);
		fullDef.SetCardDef(cardDef);
		return fullDef;
	}

	// Token: 0x060011DB RID: 4571 RVA: 0x0004D58B File Offset: 0x0004B78B
	public void LoadFullDef(string cardId, DefLoader.LoadDefCallback<FullDef> callback, object userData)
	{
		callback(cardId, this.GetFullDef(cardId, null), userData);
	}

	// Token: 0x0400095D RID: 2397
	private static DefLoader s_instance;

	// Token: 0x0400095E RID: 2398
	private bool m_loadedEntityDefs;

	// Token: 0x0400095F RID: 2399
	private Map<string, EntityDef> m_entityDefCache = new Map<string, EntityDef>();

	// Token: 0x04000960 RID: 2400
	private Map<string, CardDef> m_cachedCardDefs = new Map<string, CardDef>();

	// Token: 0x04000961 RID: 2401
	private GameObject m_placeholderCardPrefab;

	// Token: 0x020002A6 RID: 678
	// (Invoke) Token: 0x060024B8 RID: 9400
	public delegate void LoadDefCallback<T>(string cardId, T def, object userData);
}
