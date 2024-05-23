using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006F8 RID: 1784
public class CollectionPageDisplay : MonoBehaviour
{
	// Token: 0x06004963 RID: 18787 RVA: 0x0015EDFC File Offset: 0x0015CFFC
	private void Awake()
	{
		this.m_noMatchesFoundText.Text = GameStrings.Get("GLUE_COLLECTION_NO_MATCHES");
	}

	// Token: 0x06004964 RID: 18788 RVA: 0x0015EE14 File Offset: 0x0015D014
	public static int GetMaxCardsPerPage()
	{
		CollectionPageLayoutSettings.Variables currentPageLayoutSettings = CollectionManagerDisplay.Get().GetCurrentPageLayoutSettings();
		return currentPageLayoutSettings.m_ColumnCount * currentPageLayoutSettings.m_RowCount;
	}

	// Token: 0x06004965 RID: 18789 RVA: 0x0015EE3C File Offset: 0x0015D03C
	public static int GetMaxCardsPerPage(CollectionManagerDisplay.ViewMode viewMode)
	{
		CollectionPageLayoutSettings.Variables pageLayoutSettings = CollectionManagerDisplay.Get().GetPageLayoutSettings(viewMode);
		return pageLayoutSettings.m_ColumnCount * pageLayoutSettings.m_RowCount;
	}

	// Token: 0x06004966 RID: 18790 RVA: 0x0015EE64 File Offset: 0x0015D064
	public CollectionCardVisual GetCardVisual(string cardID, TAG_PREMIUM premium)
	{
		foreach (CollectionCardVisual collectionCardVisual in this.m_collectionCardVisuals)
		{
			if (collectionCardVisual.IsShown())
			{
				if (collectionCardVisual.GetVisualType() == CollectionManagerDisplay.ViewMode.CARDS)
				{
					Actor actor = collectionCardVisual.GetActor();
					if (actor.GetEntityDef().GetCardId().Equals(cardID))
					{
						if (actor.GetPremium() == premium)
						{
							return collectionCardVisual;
						}
					}
				}
			}
		}
		return null;
	}

	// Token: 0x06004967 RID: 18791 RVA: 0x0015EF14 File Offset: 0x0015D114
	public void UpdateCollectionCards(List<Actor> actorList, CollectionManagerDisplay.ViewMode mode, bool isMassDisenchanting)
	{
		int i;
		for (i = 0; i < actorList.Count; i++)
		{
			if (i >= CollectionPageDisplay.GetMaxCardsPerPage())
			{
				break;
			}
			Actor actor = actorList[i];
			CollectionCardVisual collectionCardVisual = this.GetCollectionCardVisual(i);
			collectionCardVisual.SetActor(actor, mode);
			collectionCardVisual.Show();
			if (mode == CollectionManagerDisplay.ViewMode.HERO_SKINS)
			{
				collectionCardVisual.SetHeroSkinBoxCollider();
			}
			else
			{
				collectionCardVisual.SetDefaultBoxCollider();
			}
		}
		for (int j = i; j < this.m_collectionCardVisuals.Count; j++)
		{
			CollectionCardVisual collectionCardVisual2 = this.GetCollectionCardVisual(j);
			collectionCardVisual2.SetActor(null, CollectionManagerDisplay.ViewMode.CARDS);
			collectionCardVisual2.Hide();
		}
		this.UpdateFavoriteCardBack(mode);
		this.UpdateFavoriteHeroSkins(mode, isMassDisenchanting);
		this.UpdateHeroSkinNames(mode);
		this.UpdateCurrentPageCardLocks(false);
	}

	// Token: 0x06004968 RID: 18792 RVA: 0x0015EFD0 File Offset: 0x0015D1D0
	public void UpdateFavoriteHeroSkins(CollectionManagerDisplay.ViewMode mode, bool isMassDisenchanting)
	{
		bool flag = mode == CollectionManagerDisplay.ViewMode.HERO_SKINS;
		if (this.m_heroSkinsDecor != null)
		{
			this.m_heroSkinsDecor.SetActive(flag && !isMassDisenchanting);
		}
		if (!flag)
		{
			return;
		}
		CollectionDeck taggedDeck = CollectionManager.Get().GetTaggedDeck(CollectionManager.DeckTag.Editing);
		bool flag2 = taggedDeck == null;
		foreach (CollectionCardVisual collectionCardVisual in this.m_collectionCardVisuals)
		{
			if (collectionCardVisual.IsShown())
			{
				Actor actor = collectionCardVisual.GetActor();
				CollectionHeroSkin component = actor.GetComponent<CollectionHeroSkin>();
				if (!(component == null))
				{
					component.ShowShadow(actor.IsShown());
					EntityDef entityDef = actor.GetEntityDef();
					if (entityDef != null)
					{
						component.SetClass(entityDef.GetClass());
						bool show = false;
						if (flag2)
						{
							NetCache.CardDefinition favoriteHero = CollectionManager.Get().GetFavoriteHero(entityDef.GetClass());
							if (favoriteHero != null)
							{
								int count = CollectionManager.Get().GetBestHeroesIOwn(entityDef.GetClass()).Count;
								show = (count > 1 && !string.IsNullOrEmpty(favoriteHero.Name) && favoriteHero.Name == entityDef.GetCardId());
							}
						}
						component.ShowFavoriteBanner(show);
					}
				}
			}
		}
	}

	// Token: 0x06004969 RID: 18793 RVA: 0x0015F13C File Offset: 0x0015D33C
	public void UpdateHeroSkinNames(CollectionManagerDisplay.ViewMode mode)
	{
		if (!UniversalInputManager.UsePhoneUI || mode != CollectionManagerDisplay.ViewMode.HERO_SKINS)
		{
			return;
		}
		foreach (CollectionCardVisual collectionCardVisual in this.m_collectionCardVisuals)
		{
			if (collectionCardVisual.IsShown())
			{
				Actor actor = collectionCardVisual.GetActor();
				CollectionHeroSkin component = actor.GetComponent<CollectionHeroSkin>();
				if (!(component == null))
				{
					component.ShowCollectionManagerText();
				}
			}
		}
	}

	// Token: 0x0600496A RID: 18794 RVA: 0x0015F1D8 File Offset: 0x0015D3D8
	public void UpdateFavoriteCardBack(CollectionManagerDisplay.ViewMode mode)
	{
		if (mode != CollectionManagerDisplay.ViewMode.CARD_BACKS)
		{
			return;
		}
		int num = -1;
		if (!CollectionManager.Get().IsInEditMode())
		{
			NetCache.NetCacheCardBacks netObject = NetCache.Get().GetNetObject<NetCache.NetCacheCardBacks>();
			num = netObject.DefaultCardBack;
		}
		foreach (CollectionCardVisual collectionCardVisual in this.m_collectionCardVisuals)
		{
			if (collectionCardVisual.IsShown())
			{
				Actor actor = collectionCardVisual.GetActor();
				CollectionCardBack component = actor.GetComponent<CollectionCardBack>();
				if (!(component == null))
				{
					component.ShowFavoriteBanner(num == component.GetCardBackId());
				}
			}
		}
	}

	// Token: 0x0600496B RID: 18795 RVA: 0x0015F29C File Offset: 0x0015D49C
	public void UpdateBasePage()
	{
		if (this.m_basePageMaterial != null && this.m_basePage != null)
		{
			this.m_basePage.GetComponent<MeshRenderer>().material = this.m_basePageMaterial;
		}
	}

	// Token: 0x0600496C RID: 18796 RVA: 0x0015F2E4 File Offset: 0x0015D4E4
	public void UpdateDeckTemplatePage(Component deckTemplatePicker)
	{
		this.HideHeroSkinsDecor();
		if (deckTemplatePicker != null && this.m_deckTemplateContainer != null)
		{
			foreach (CollectionCardVisual collectionCardVisual in this.m_collectionCardVisuals)
			{
				collectionCardVisual.SetActor(null, CollectionManagerDisplay.ViewMode.CARDS);
				collectionCardVisual.Hide();
			}
			if (this.m_basePage != null)
			{
				this.m_basePageMaterial = this.m_basePage.GetComponent<MeshRenderer>().material;
				this.m_basePage.GetComponent<MeshRenderer>().material = this.m_deckTemplatePageMaterial;
			}
			GameUtils.SetParent(deckTemplatePicker, this.m_deckTemplateContainer, false);
			GameUtils.ResetTransform(deckTemplatePicker);
		}
	}

	// Token: 0x0600496D RID: 18797 RVA: 0x0015F3B8 File Offset: 0x0015D5B8
	public void ShowNoMatchesFound(bool show)
	{
		this.m_noMatchesFoundText.gameObject.SetActive(show);
	}

	// Token: 0x0600496E RID: 18798 RVA: 0x0015F3CB File Offset: 0x0015D5CB
	public void HideHeroSkinsDecor()
	{
		if (this.m_heroSkinsDecor != null)
		{
			this.m_heroSkinsDecor.SetActive(false);
		}
	}

	// Token: 0x0600496F RID: 18799 RVA: 0x0015F3EC File Offset: 0x0015D5EC
	public void UpdateCurrentPageCardLocks(bool playSound = false)
	{
		CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
		if (collectionDeckTray.GetCurrentContentType() != CollectionDeckTray.DeckContentTypes.Cards)
		{
			foreach (CollectionCardVisual collectionCardVisual in this.m_collectionCardVisuals)
			{
				if (collectionCardVisual.IsShown())
				{
					collectionCardVisual.ShowLock(CollectionCardVisual.LockType.NONE);
				}
			}
			return;
		}
		CollectionDeck taggedDeck = CollectionManager.Get().GetTaggedDeck(CollectionManager.DeckTag.Editing);
		foreach (CollectionCardVisual collectionCardVisual2 in this.m_collectionCardVisuals)
		{
			if (collectionCardVisual2.GetVisualType() != CollectionManagerDisplay.ViewMode.CARD_BACKS && collectionCardVisual2.GetVisualType() != CollectionManagerDisplay.ViewMode.DECK_TEMPLATE)
			{
				if (!collectionCardVisual2.IsShown())
				{
					collectionCardVisual2.ShowLock(CollectionCardVisual.LockType.NONE);
				}
				else
				{
					Actor actor = collectionCardVisual2.GetActor();
					string cardId = actor.GetEntityDef().GetCardId();
					TAG_PREMIUM premium = actor.GetPremium();
					CollectibleCard card = CollectionManager.Get().GetCard(cardId, premium);
					if (card.OwnedCount <= 0)
					{
						collectionCardVisual2.ShowLock(CollectionCardVisual.LockType.NONE);
					}
					else
					{
						DeckRuleset deckRuleset = CollectionManager.Get().GetDeckRuleset();
						string empty = string.Empty;
						DeckRule deckRule;
						if (deckRuleset == null || deckRuleset.CanAddToDeck(actor.GetEntityDef(), premium, taggedDeck, out empty, out deckRule, null))
						{
							collectionCardVisual2.ShowLock(CollectionCardVisual.LockType.NONE, empty, playSound);
						}
						else
						{
							CollectionCardVisual.LockType lockType = (deckRule.GetRuleType() != DeckRule.RuleType.PLAYER_OWNS_EACH_COPY) ? CollectionCardVisual.LockType.MAX_COPIES_IN_DECK : CollectionCardVisual.LockType.NO_MORE_INSTANCES;
							collectionCardVisual2.ShowLock(lockType, empty, playSound);
						}
					}
				}
			}
			else
			{
				collectionCardVisual2.ShowLock(CollectionCardVisual.LockType.NONE);
			}
		}
	}

	// Token: 0x06004970 RID: 18800 RVA: 0x0015F5CC File Offset: 0x0015D7CC
	public void SetIsWild(bool isWild)
	{
		if (isWild == this.m_isWild)
		{
			return;
		}
		this.m_isWild = isWild;
		if (this.m_classFlavorHeader != null)
		{
			this.m_classFlavorHeader.GetComponent<Renderer>().material = ((!isWild) ? this.m_standardHeaderMaterial : this.m_wildHeaderMaterial);
		}
		if (this.m_pageCountText != null)
		{
			this.m_pageCountText.TextColor = ((!CollectionManager.Get().IsShowingWildTheming(null)) ? this.m_standardTextColor : this.m_wildTextColor);
		}
		this.m_basePageRenderer.material = ((!isWild) ? this.m_standardPageMaterial : this.m_wildPageMaterial);
	}

	// Token: 0x06004971 RID: 18801 RVA: 0x0015F684 File Offset: 0x0015D884
	public void SetClass(TAG_CLASS? classTag)
	{
		if (classTag == null)
		{
			this.SetClassNameText(string.Empty);
			if (this.m_classFlavorHeader != null)
			{
				this.m_classFlavorHeader.SetActive(false);
			}
			return;
		}
		TAG_CLASS value = classTag.Value;
		this.SetClassNameText(GameStrings.GetClassName(value));
		CollectionPageDisplay.SetClassFlavorTextures(this.m_classFlavorHeader, CollectionPageDisplay.TagClassToHeaderClass(value));
	}

	// Token: 0x06004972 RID: 18802 RVA: 0x0015F6EE File Offset: 0x0015D8EE
	public void SetHeroSkins(TAG_CLASS classTag)
	{
		this.SetClassNameText(GameStrings.Get("GLUE_COLLECTION_MANAGER_HERO_SKINS_TITLE"));
		CollectionPageDisplay.SetClassFlavorTextures(this.m_classFlavorHeader, CollectionPageDisplay.HEADER_CLASS.HEROSKINS);
	}

	// Token: 0x06004973 RID: 18803 RVA: 0x0015F70D File Offset: 0x0015D90D
	public void SetCardBacks()
	{
		this.SetClassNameText(GameStrings.Get("GLUE_COLLECTION_MANAGER_CARD_BACKS_TITLE"));
		CollectionPageDisplay.SetClassFlavorTextures(this.m_classFlavorHeader, CollectionPageDisplay.HEADER_CLASS.CARDBACKS);
	}

	// Token: 0x06004974 RID: 18804 RVA: 0x0015F72C File Offset: 0x0015D92C
	public void SetDeckTemplates()
	{
		this.SetClassNameText(string.Empty);
		if (this.m_classFlavorHeader != null)
		{
			this.m_classFlavorHeader.SetActive(false);
		}
	}

	// Token: 0x06004975 RID: 18805 RVA: 0x0015F761 File Offset: 0x0015D961
	public void SetPageCountText(string text)
	{
		if (this.m_pageCountText != null)
		{
			this.m_pageCountText.Text = text;
		}
	}

	// Token: 0x06004976 RID: 18806 RVA: 0x0015F780 File Offset: 0x0015D980
	public void ActivatePageCountText(bool active)
	{
		if (this.m_pageCountText != null)
		{
			this.m_pageCountText.gameObject.SetActive(active);
		}
	}

	// Token: 0x06004977 RID: 18807 RVA: 0x0015F7B0 File Offset: 0x0015D9B0
	public TAG_CLASS? GetFirstCardClass()
	{
		if (this.m_collectionCardVisuals.Count == 0)
		{
			return default(TAG_CLASS?);
		}
		CollectionCardVisual collectionCardVisual = this.m_collectionCardVisuals[0];
		if (!collectionCardVisual.IsShown())
		{
			return default(TAG_CLASS?);
		}
		Actor actor = collectionCardVisual.GetActor();
		if (!actor.IsShown())
		{
			return default(TAG_CLASS?);
		}
		EntityDef entityDef = actor.GetEntityDef();
		if (entityDef == null)
		{
			return default(TAG_CLASS?);
		}
		return new TAG_CLASS?(entityDef.GetClass());
	}

	// Token: 0x06004978 RID: 18808 RVA: 0x0015F83C File Offset: 0x0015DA3C
	private CollectionCardVisual GetCollectionCardVisual(int index)
	{
		CollectionPageLayoutSettings.Variables currentPageLayoutSettings = CollectionManagerDisplay.Get().GetCurrentPageLayoutSettings();
		float columnSpacing = currentPageLayoutSettings.m_ColumnSpacing;
		int columnCount = currentPageLayoutSettings.m_ColumnCount;
		float num = columnSpacing * (float)(columnCount - 1);
		float scale = currentPageLayoutSettings.m_Scale;
		float rowSpacing = currentPageLayoutSettings.m_RowSpacing;
		GameObject cardStartPositionEightCards = this.m_cardStartPositionEightCards;
		Vector3 localPosition = cardStartPositionEightCards.transform.localPosition;
		Vector3 vector = localPosition + currentPageLayoutSettings.m_Offset;
		int num2 = index / columnCount;
		vector.x += (float)(index % columnCount) * columnSpacing - num * 0.5f;
		vector.z -= rowSpacing * (float)num2;
		CollectionCardVisual collectionCardVisual;
		if (index == this.m_collectionCardVisuals.Count)
		{
			collectionCardVisual = (CollectionCardVisual)GameUtils.Instantiate(CollectionManagerDisplay.Get().GetCardVisualPrefab(), base.gameObject, false);
			this.m_collectionCardVisuals.Insert(index, collectionCardVisual);
		}
		else
		{
			collectionCardVisual = this.m_collectionCardVisuals[index];
		}
		collectionCardVisual.SetCMRow(num2);
		collectionCardVisual.transform.localScale = new Vector3(scale, scale, scale);
		collectionCardVisual.transform.position = base.transform.TransformPoint(vector);
		return collectionCardVisual;
	}

	// Token: 0x06004979 RID: 18809 RVA: 0x0015F95D File Offset: 0x0015DB5D
	private void SetClassNameText(string className)
	{
		if (this.m_classNameText != null)
		{
			this.m_classNameText.Text = className;
		}
	}

	// Token: 0x0600497A RID: 18810 RVA: 0x0015F97C File Offset: 0x0015DB7C
	public static CollectionPageDisplay.HEADER_CLASS TagClassToHeaderClass(TAG_CLASS classTag)
	{
		string text = classTag.ToString();
		if (Enum.IsDefined(typeof(CollectionPageDisplay.HEADER_CLASS), text))
		{
			return (CollectionPageDisplay.HEADER_CLASS)((int)Enum.Parse(typeof(CollectionPageDisplay.HEADER_CLASS), text));
		}
		return CollectionPageDisplay.HEADER_CLASS.INVALID;
	}

	// Token: 0x0600497B RID: 18811 RVA: 0x0015F9C4 File Offset: 0x0015DBC4
	public static void SetClassFlavorTextures(GameObject header, CollectionPageDisplay.HEADER_CLASS headerClass)
	{
		if (header == null)
		{
			return;
		}
		float num = ((float)headerClass >= 8f) ? 0.5f : 0f;
		float num2 = -(float)headerClass / 8f;
		header.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(num, num2));
		if (header != null)
		{
			header.SetActive(true);
		}
	}

	// Token: 0x04003092 RID: 12434
	public UberText m_noMatchesFoundText;

	// Token: 0x04003093 RID: 12435
	public GameObject m_cardStartPositionEightCards;

	// Token: 0x04003094 RID: 12436
	public UberText m_pageCountText;

	// Token: 0x04003095 RID: 12437
	public UberText m_classNameText;

	// Token: 0x04003096 RID: 12438
	public GameObject m_classFlavorHeader;

	// Token: 0x04003097 RID: 12439
	public GameObject m_heroSkinsDecor;

	// Token: 0x04003098 RID: 12440
	public GameObject m_deckTemplateContainer;

	// Token: 0x04003099 RID: 12441
	public GameObject m_basePage;

	// Token: 0x0400309A RID: 12442
	public Material m_deckTemplatePageMaterial;

	// Token: 0x0400309B RID: 12443
	private Material m_basePageMaterial;

	// Token: 0x0400309C RID: 12444
	public MeshRenderer m_basePageRenderer;

	// Token: 0x0400309D RID: 12445
	public Material m_wildHeaderMaterial;

	// Token: 0x0400309E RID: 12446
	public Material m_standardHeaderMaterial;

	// Token: 0x0400309F RID: 12447
	public Material m_wildPageMaterial;

	// Token: 0x040030A0 RID: 12448
	public Material m_standardPageMaterial;

	// Token: 0x040030A1 RID: 12449
	public Color m_standardTextColor;

	// Token: 0x040030A2 RID: 12450
	public Color m_wildTextColor;

	// Token: 0x040030A3 RID: 12451
	private List<CollectionCardVisual> m_collectionCardVisuals = new List<CollectionCardVisual>();

	// Token: 0x040030A4 RID: 12452
	private bool m_isWild;

	// Token: 0x02000794 RID: 1940
	public enum HEADER_CLASS
	{
		// Token: 0x0400339D RID: 13213
		INVALID,
		// Token: 0x0400339E RID: 13214
		SHAMAN,
		// Token: 0x0400339F RID: 13215
		PALADIN,
		// Token: 0x040033A0 RID: 13216
		MAGE,
		// Token: 0x040033A1 RID: 13217
		DRUID,
		// Token: 0x040033A2 RID: 13218
		HUNTER,
		// Token: 0x040033A3 RID: 13219
		ROGUE,
		// Token: 0x040033A4 RID: 13220
		WARRIOR,
		// Token: 0x040033A5 RID: 13221
		PRIEST,
		// Token: 0x040033A6 RID: 13222
		WARLOCK,
		// Token: 0x040033A7 RID: 13223
		HEROSKINS,
		// Token: 0x040033A8 RID: 13224
		CARDBACKS
	}
}
