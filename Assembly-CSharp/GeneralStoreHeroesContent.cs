using System;
using UnityEngine;

// Token: 0x02000ACF RID: 2767
[CustomEditClass]
public class GeneralStoreHeroesContent : GeneralStoreContent
{
	// Token: 0x06005F6C RID: 24428 RVA: 0x001C8A30 File Offset: 0x001C6C30
	private void Awake()
	{
		this.m_heroDisplay1 = this.m_heroDisplay;
		this.m_heroDisplay2 = Object.Instantiate<GeneralStoreHeroesContentDisplay>(this.m_heroDisplay);
		this.m_heroDisplay2.transform.parent = this.m_heroDisplay1.transform.parent;
		this.m_heroDisplay2.transform.localPosition = this.m_heroDisplay1.transform.localPosition;
		this.m_heroDisplay2.transform.localScale = this.m_heroDisplay1.transform.localScale;
		this.m_heroDisplay2.transform.localRotation = this.m_heroDisplay1.transform.localRotation;
		this.m_heroDisplay2.gameObject.SetActive(false);
		this.m_heroDisplay1.SetParent(this);
		this.m_heroDisplay2.SetParent(this);
		this.m_heroDisplay1.SetKeyArtRenderer(this.m_renderQuad1);
		this.m_heroDisplay2.SetKeyArtRenderer(this.m_renderQuad2);
		this.m_renderToTexture1.GetComponent<RenderToTexture>().m_RenderToObject = this.m_heroDisplay1.m_renderArtQuad;
		this.m_renderToTexture2.GetComponent<RenderToTexture>().m_RenderToObject = this.m_heroDisplay2.m_renderArtQuad;
	}

	// Token: 0x06005F6D RID: 24429 RVA: 0x001C8B5B File Offset: 0x001C6D5B
	public override bool AnimateEntranceEnd()
	{
		this.m_parentStore.HideAccentTexture();
		return true;
	}

	// Token: 0x06005F6E RID: 24430 RVA: 0x001C8B69 File Offset: 0x001C6D69
	public HeroDbfRecord GetSelectedHero()
	{
		return this.m_currentDbfRecord;
	}

	// Token: 0x06005F6F RID: 24431 RVA: 0x001C8B74 File Offset: 0x001C6D74
	public void SelectHero(HeroDbfRecord heroDbfRecord, bool animate = true)
	{
		if (heroDbfRecord == this.m_currentDbfRecord)
		{
			return;
		}
		this.m_currentDbfRecord = heroDbfRecord;
		Network.Bundle bundle = null;
		StoreManager.Get().GetHeroBundleByCardMiniGuid(heroDbfRecord.CardId, out bundle);
		base.SetCurrentMoneyBundle(bundle, false);
		if (this.m_currentHeroDef != null)
		{
			Object.Destroy(this.m_currentHeroDef.gameObject);
			this.m_currentHeroDef = null;
		}
		this.m_currentCardBackPreview = heroDbfRecord.CardBackId;
		string herodefAssetPath = heroDbfRecord.HerodefAssetPath;
		GameObject gameObject = AssetLoader.Get().LoadGameObject(FileUtils.GameAssetPathToName(herodefAssetPath), true, false);
		this.m_currentHeroDef = gameObject.GetComponent<CollectionHeroDef>();
		bool purchased = !StoreManager.Get().CanBuyBundle(bundle);
		this.AnimateAndUpdateDisplays(heroDbfRecord, this.m_currentCardBackPreview, this.m_currentHeroDef, purchased);
		this.PlayHeroMusic();
		this.UpdateHeroDescription(purchased);
	}

	// Token: 0x06005F70 RID: 24432 RVA: 0x001C8C3C File Offset: 0x001C6E3C
	public void PlayCurrentHeroPurchaseEmote()
	{
		GeneralStoreHeroesContentDisplay currentDisplay = this.GetCurrentDisplay();
		if (currentDisplay != null)
		{
			currentDisplay.PlayPurchaseEmote();
		}
	}

	// Token: 0x06005F71 RID: 24433 RVA: 0x001C8C62 File Offset: 0x001C6E62
	public override void StoreShown(bool isCurrent)
	{
		if (this.m_currentDisplay == -1 || !isCurrent)
		{
			return;
		}
		this.PlayHeroMusic();
		this.ResetHeroPreview();
	}

	// Token: 0x06005F72 RID: 24434 RVA: 0x001C8C83 File Offset: 0x001C6E83
	public override void PreStoreFlipIn()
	{
		this.ResetHeroPreview();
	}

	// Token: 0x06005F73 RID: 24435 RVA: 0x001C8C8B File Offset: 0x001C6E8B
	public override void PostStoreFlipIn(bool animatedFlipIn)
	{
		this.PlayHeroMusic();
	}

	// Token: 0x06005F74 RID: 24436 RVA: 0x001C8C94 File Offset: 0x001C6E94
	public override void TryBuyWithMoney(Network.Bundle bundle, GeneralStoreContent.BuyEvent successBuyCB, GeneralStoreContent.BuyEvent failedBuyCB)
	{
		SpecialEventManager specialEventManager = SpecialEventManager.Get();
		if (!specialEventManager.IsEventActive(bundle.ProductEvent, false))
		{
			string key = "GLUE_STORE_PRODUCT_NOT_AVAILABLE_TEXT";
			if (specialEventManager.HasEventEnded(bundle.ProductEvent))
			{
				key = "GLUE_STORE_PRODUCT_NOT_AVAILABLE_TEXT_HAS_ENDED";
			}
			else if (specialEventManager.GetEventLocalStartTime(bundle.ProductEvent) != null && !specialEventManager.HasEventStarted(bundle.ProductEvent))
			{
				key = "GLUE_STORE_PRODUCT_NOT_AVAILABLE_TEXT_NOT_YET_STARTED";
			}
			AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
			popupInfo.m_headerText = GameStrings.Get("GLUE_STORE_PRODUCT_NOT_AVAILABLE_HEADER");
			popupInfo.m_text = GameStrings.Get(key);
			popupInfo.m_showAlertIcon = true;
			popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
			popupInfo.m_responseCallback = delegate(AlertPopup.Response response, object data)
			{
				this.m_parentStore.ActivateCover(false);
				if (failedBuyCB != null)
				{
					failedBuyCB();
				}
			};
			this.m_parentStore.ActivateCover(true);
			DialogManager.Get().ShowPopup(popupInfo);
			return;
		}
		if (successBuyCB != null)
		{
			successBuyCB();
		}
	}

	// Token: 0x06005F75 RID: 24437 RVA: 0x001C8D88 File Offset: 0x001C6F88
	protected override void OnRefresh()
	{
		Network.Bundle bundle = null;
		if (this.m_currentDbfRecord != null)
		{
			StoreManager.Get().GetHeroBundleByCardMiniGuid(this.m_currentDbfRecord.CardId, out bundle);
		}
		bool flag = !StoreManager.Get().CanBuyBundle(bundle);
		GeneralStoreHeroesContentDisplay currentDisplay = this.GetCurrentDisplay();
		currentDisplay.ShowPurchasedCheckmark(flag);
		base.SetCurrentMoneyBundle(bundle, false);
		this.UpdateHeroDescription(flag);
	}

	// Token: 0x06005F76 RID: 24438 RVA: 0x001C8DE6 File Offset: 0x001C6FE6
	public override bool IsPurchaseDisabled()
	{
		return this.m_currentDisplay == -1;
	}

	// Token: 0x06005F77 RID: 24439 RVA: 0x001C8DF1 File Offset: 0x001C6FF1
	public override string GetMoneyDisplayOwnedText()
	{
		return GameStrings.Get("GLUE_STORE_HERO_BUTTON_COST_OWNED_TEXT");
	}

	// Token: 0x06005F78 RID: 24440 RVA: 0x001C8DFD File Offset: 0x001C6FFD
	private GameObject GetCurrentDisplayContainer()
	{
		return this.GetCurrentDisplay().gameObject;
	}

	// Token: 0x06005F79 RID: 24441 RVA: 0x001C8E0C File Offset: 0x001C700C
	private GameObject GetNextDisplayContainer()
	{
		return ((this.m_currentDisplay + 1) % 2 != 0) ? this.m_heroDisplay2.gameObject : this.m_heroDisplay1.gameObject;
	}

	// Token: 0x06005F7A RID: 24442 RVA: 0x001C8E43 File Offset: 0x001C7043
	private GeneralStoreHeroesContentDisplay GetCurrentDisplay()
	{
		return (this.m_currentDisplay != 0) ? this.m_heroDisplay2 : this.m_heroDisplay1;
	}

	// Token: 0x06005F7B RID: 24443 RVA: 0x001C8E64 File Offset: 0x001C7064
	private void AnimateAndUpdateDisplays(HeroDbfRecord heroDbfRecord, int cardBackIdx, CollectionHeroDef heroDef, bool purchased)
	{
		GameObject currDisplay = null;
		if (this.m_currentDisplay == -1)
		{
			this.m_currentDisplay = 1;
			currDisplay = this.m_heroEmptyDisplay;
		}
		else
		{
			currDisplay = this.GetCurrentDisplayContainer();
		}
		GameObject nextDisplayContainer = this.GetNextDisplayContainer();
		GeneralStoreHeroesContentDisplay currentDisplay = this.GetCurrentDisplay();
		this.m_currentDisplay = (this.m_currentDisplay + 1) % 2;
		currDisplay.transform.localRotation = Quaternion.identity;
		nextDisplayContainer.transform.localEulerAngles = new Vector3(180f, 0f, 0f);
		nextDisplayContainer.SetActive(true);
		iTween.StopByName(currDisplay, "ROTATION_TWEEN");
		iTween.StopByName(nextDisplayContainer, "ROTATION_TWEEN");
		iTween.RotateBy(currDisplay, iTween.Hash(new object[]
		{
			"amount",
			new Vector3(0.5f, 0f, 0f),
			"time",
			0.5f,
			"name",
			"ROTATION_TWEEN",
			"oncomplete",
			delegate(object o)
			{
				currDisplay.SetActive(false);
			}
		}));
		if (this.m_currentSelectedHeroBannerFlare != null)
		{
			Object.Destroy(this.m_currentSelectedHeroBannerFlare);
			this.m_currentSelectedHeroBannerFlare = null;
		}
		if (this.m_currentDbfRecord != null && !string.IsNullOrEmpty(this.m_currentDbfRecord.StoreBannerPrefab))
		{
			string name = FileUtils.GameAssetPathToName(this.m_currentDbfRecord.StoreBannerPrefab);
			this.m_currentSelectedHeroBannerFlare = AssetLoader.Get().LoadGameObject(name, true, false);
			if (this.m_currentSelectedHeroBannerFlare != null)
			{
				GameUtils.SetParent(this.m_currentSelectedHeroBannerFlare, nextDisplayContainer, false);
				this.m_currentSelectedHeroBannerFlare.transform.localPosition = Vector3.zero;
				this.m_currentSelectedHeroBannerFlare.transform.localRotation = Quaternion.identity;
				this.m_currentSelectedHeroBannerFlare.gameObject.SetActive(true);
			}
		}
		iTween.RotateBy(nextDisplayContainer, iTween.Hash(new object[]
		{
			"amount",
			new Vector3(0.5f, 0f, 0f),
			"time",
			0.5f,
			"name",
			"ROTATION_TWEEN"
		}));
		if (!string.IsNullOrEmpty(this.m_backgroundFlipSound))
		{
			SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_backgroundFlipSound));
		}
		GeneralStoreHeroesContentDisplay currentDisplay2 = this.GetCurrentDisplay();
		currentDisplay2.UpdateFrame(heroDbfRecord, cardBackIdx, heroDef);
		currentDisplay2.ShowPurchasedCheckmark(purchased);
		currentDisplay2.ResetPreview();
		currentDisplay.ResetPreview();
	}

	// Token: 0x06005F7C RID: 24444 RVA: 0x001C9108 File Offset: 0x001C7308
	private void ResetHeroPreview()
	{
		GeneralStoreHeroesContentDisplay currentDisplay = this.GetCurrentDisplay();
		currentDisplay.ResetPreview();
	}

	// Token: 0x06005F7D RID: 24445 RVA: 0x001C9124 File Offset: 0x001C7324
	private void PlayHeroMusic()
	{
		if (this.m_currentHeroDef == null || this.m_currentHeroDef.m_heroPlaylist == MusicPlaylistType.Invalid || !MusicManager.Get().StartPlaylist(this.m_currentHeroDef.m_heroPlaylist))
		{
			this.m_parentStore.ResumePreviousMusicPlaylist();
		}
	}

	// Token: 0x06005F7E RID: 24446 RVA: 0x001C9178 File Offset: 0x001C7378
	private void UpdateHeroDescription(bool purchased)
	{
		if (this.m_currentDisplay == -1 || this.m_currentDbfRecord == null)
		{
			this.m_parentStore.SetChooseDescription(GameStrings.Get("GLUE_STORE_CHOOSE_HERO"));
		}
		else
		{
			string warning = (!StoreManager.Get().IsKoreanCustomer()) ? string.Empty : GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_HERO");
			this.m_parentStore.SetDescription(string.Empty, this.GetHeroDescriptionString(), warning);
		}
		this.m_parentStore.HideAccentTexture();
	}

	// Token: 0x06005F7F RID: 24447 RVA: 0x001C91FC File Offset: 0x001C73FC
	private string GetHeroDescriptionString()
	{
		if (!UniversalInputManager.UsePhoneUI)
		{
			return this.m_currentDbfRecord.StoreDescription;
		}
		return this.m_currentDbfRecord.StoreDescriptionPhone;
	}

	// Token: 0x040046CE RID: 18126
	public string m_keyArtFadeAnim = "HeroSkinArt_WipeAway";

	// Token: 0x040046CF RID: 18127
	public string m_keyArtAppearAnim = "HeroSkinArtGlowIn";

	// Token: 0x040046D0 RID: 18128
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_keyArtFadeSound;

	// Token: 0x040046D1 RID: 18129
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_keyArtAppearSound;

	// Token: 0x040046D2 RID: 18130
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_previewButtonClick;

	// Token: 0x040046D3 RID: 18131
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_backgroundFlipSound;

	// Token: 0x040046D4 RID: 18132
	public GameObject m_heroEmptyDisplay;

	// Token: 0x040046D5 RID: 18133
	public GeneralStoreHeroesContentDisplay m_heroDisplay;

	// Token: 0x040046D6 RID: 18134
	public MeshRenderer m_renderQuad1;

	// Token: 0x040046D7 RID: 18135
	public GameObject m_renderToTexture1;

	// Token: 0x040046D8 RID: 18136
	public MeshRenderer m_renderQuad2;

	// Token: 0x040046D9 RID: 18137
	public GameObject m_renderToTexture2;

	// Token: 0x040046DA RID: 18138
	private GameObject m_currentSelectedHeroBannerFlare;

	// Token: 0x040046DB RID: 18139
	private CollectionHeroDef m_currentHeroDef;

	// Token: 0x040046DC RID: 18140
	private int m_currentCardBackPreview = -1;

	// Token: 0x040046DD RID: 18141
	private int m_currentDisplay = -1;

	// Token: 0x040046DE RID: 18142
	private HeroDbfRecord m_currentDbfRecord;

	// Token: 0x040046DF RID: 18143
	private GeneralStoreHeroesContentDisplay m_heroDisplay1;

	// Token: 0x040046E0 RID: 18144
	private GeneralStoreHeroesContentDisplay m_heroDisplay2;
}
