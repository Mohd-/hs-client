using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006F2 RID: 1778
[CustomEditClass]
public class HeroSkinInfoManager : MonoBehaviour
{
	// Token: 0x06004947 RID: 18759 RVA: 0x0015E2D8 File Offset: 0x0015C4D8
	public static HeroSkinInfoManager Get()
	{
		if (HeroSkinInfoManager.s_instance == null)
		{
			string name = (!UniversalInputManager.UsePhoneUI) ? "HeroSkinInfoManager" : "HeroSkinInfoManager_phone";
			HeroSkinInfoManager.s_instance = AssetLoader.Get().LoadGameObject(name, true, false).GetComponent<HeroSkinInfoManager>();
		}
		return HeroSkinInfoManager.s_instance;
	}

	// Token: 0x06004948 RID: 18760 RVA: 0x0015E330 File Offset: 0x0015C530
	private void Awake()
	{
		this.m_previewPane.SetActive(false);
		this.SetupUI();
	}

	// Token: 0x06004949 RID: 18761 RVA: 0x0015E344 File Offset: 0x0015C544
	private void OnDestroy()
	{
		HeroSkinInfoManager.s_instance = null;
	}

	// Token: 0x0600494A RID: 18762 RVA: 0x0015E34C File Offset: 0x0015C54C
	public void EnterPreview(CollectionCardVisual cardVisual)
	{
		if (this.m_animating)
		{
			return;
		}
		this.m_currentEntityDef = cardVisual.GetActor().GetEntityDef();
		this.m_currentPremium = cardVisual.GetActor().GetPremium();
		if (this.LoadHeroDef(this.m_currentEntityDef.GetCardId()))
		{
			if (this.m_currentEntityDef.GetCardSet() == TAG_CARD_SET.CORE)
			{
				this.m_vanillaHeroTitle.Text = this.m_currentEntityDef.GetName();
				this.m_vanillaHeroDescription.Text = this.m_currentHeroRecord.Description;
				Texture portraitTexture = null;
				Material material;
				if (this.m_currentPremium == TAG_PREMIUM.NORMAL)
				{
					material = this.m_vanillaHeroNonPremiumMaterial;
					portraitTexture = cardVisual.GetActor().GetCardDef().GetPortraitTexture();
				}
				else
				{
					material = cardVisual.GetActor().GetCardDef().GetPremiumPortraitMaterial();
				}
				this.m_newHeroFrame.SetActive(false);
				this.m_vanillaHeroFrame.SetActive(true);
				this.AssignVanillaHeroPreviewMaterial(material, portraitTexture);
			}
			else
			{
				this.m_newHeroTitle.Text = this.m_currentEntityDef.GetName();
				this.m_newHeroDescription.Text = this.m_currentHeroRecord.Description;
				this.m_newHeroFrame.SetActive(true);
				this.m_vanillaHeroFrame.SetActive(false);
				this.AssignNewHeroPreviewMaterial(this.m_currentHeroDef.m_previewTexture, cardVisual.GetActor().GetPortraitTexture());
			}
			if (this.m_currentHeroDef != null && this.m_currentHeroDef.m_collectionManagerPreviewEmote != EmoteType.INVALID)
			{
				GameUtils.LoadCardDefEmoteSound(cardVisual.GetActor().GetCardDef(), this.m_currentHeroDef.m_collectionManagerPreviewEmote, delegate(CardSoundSpell cardSpell)
				{
					if (cardSpell != null)
					{
						cardSpell.AddFinishedCallback(delegate(Spell spell, object data)
						{
							Object.Destroy(cardSpell.gameObject);
						});
						cardSpell.Reactivate();
					}
				});
			}
		}
		else
		{
			Debug.LogError("Could not load entity def for hero skin, preview will not be shown");
			this.m_newHeroFrame.SetActive(false);
			this.m_vanillaHeroFrame.SetActive(false);
		}
		this.m_previewPane.SetActive(true);
		this.m_offClicker.gameObject.SetActive(true);
		this.m_animating = true;
		iTween.ScaleFrom(this.m_previewPane, iTween.Hash(new object[]
		{
			"scale",
			new Vector3(0.01f, 0.01f, 0.01f),
			"time",
			this.m_animationTime,
			"easeType",
			iTween.EaseType.easeOutCirc,
			"oncomplete",
			delegate(object e)
			{
				this.m_animating = false;
			}
		}));
		bool flag = false;
		if (!CollectionManager.Get().IsInEditMode())
		{
			TAG_CLASS @class = this.m_currentEntityDef.GetClass();
			NetCache.CardDefinition favoriteHero = CollectionManager.Get().GetFavoriteHero(@class);
			List<CollectibleCard> bestHeroesIOwn = CollectionManager.Get().GetBestHeroesIOwn(@class);
			flag = (favoriteHero != null && favoriteHero.Name != this.m_currentCardId && bestHeroesIOwn.Count > 1);
		}
		if (this.m_currentEntityDef.GetCardSet() == TAG_CARD_SET.CORE)
		{
			this.m_vanillaHeroFavoriteButton.SetEnabled(flag);
			this.m_vanillaHeroFavoriteButton.Flip(flag);
		}
		else
		{
			this.m_newHeroFavoriteButton.SetEnabled(flag);
			this.m_newHeroFavoriteButton.Flip(flag);
		}
		if (!string.IsNullOrEmpty(this.m_enterPreviewSound))
		{
			SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_enterPreviewSound));
		}
		this.PlayHeroMusic();
		FullScreenFXMgr.Get().StartStandardBlurVignette(this.m_animationTime);
	}

	// Token: 0x0600494B RID: 18763 RVA: 0x0015E6A8 File Offset: 0x0015C8A8
	public void CancelPreview()
	{
		if (this.m_animating)
		{
			return;
		}
		Vector3 origScale = this.m_previewPane.transform.localScale;
		this.m_animating = true;
		iTween.ScaleTo(this.m_previewPane, iTween.Hash(new object[]
		{
			"scale",
			new Vector3(0.01f, 0.01f, 0.01f),
			"time",
			this.m_animationTime,
			"easeType",
			iTween.EaseType.easeOutCirc,
			"oncomplete",
			delegate(object e)
			{
				this.m_animating = false;
				this.m_previewPane.transform.localScale = origScale;
				this.m_previewPane.SetActive(false);
				this.m_offClicker.gameObject.SetActive(false);
			}
		}));
		if (!string.IsNullOrEmpty(this.m_exitPreviewSound))
		{
			SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_exitPreviewSound));
		}
		this.StopHeroMusic();
		FullScreenFXMgr.Get().EndStandardBlurVignette(this.m_animationTime, null);
	}

	// Token: 0x0600494C RID: 18764 RVA: 0x0015E7A0 File Offset: 0x0015C9A0
	private bool LoadHeroDef(string cardId)
	{
		if (this.m_currentCardId == cardId && string.IsNullOrEmpty(cardId))
		{
			return true;
		}
		HeroDbfRecord heroDbfRecord = GameDbf.Hero.GetRecords().Find((HeroDbfRecord r) => r.CardId == cardId);
		if (heroDbfRecord == null)
		{
			Debug.LogWarning(string.Format("Unable to find hero with ID: {0} in HERO.xml", cardId));
			return false;
		}
		string herodefAssetPath = heroDbfRecord.HerodefAssetPath;
		if (string.IsNullOrEmpty(herodefAssetPath))
		{
			Debug.LogWarning(string.Format("Hero record ID {0} does not have HERODEF_ASSET_PATH defined", cardId));
			return false;
		}
		GameObject gameObject = AssetLoader.Get().LoadGameObject(FileUtils.GameAssetPathToName(herodefAssetPath), true, false);
		CollectionHeroDef component = gameObject.GetComponent<CollectionHeroDef>();
		if (component == null)
		{
			Debug.LogWarning(string.Format("Hero def does not exist on object: {0}", herodefAssetPath));
			return false;
		}
		if (this.m_currentHeroDef != null)
		{
			Object.Destroy(this.m_currentHeroDef.gameObject);
		}
		this.m_currentCardId = cardId;
		this.m_currentHeroDef = component;
		this.m_currentHeroRecord = heroDbfRecord;
		return true;
	}

	// Token: 0x0600494D RID: 18765 RVA: 0x0015E8C0 File Offset: 0x0015CAC0
	private void SetupUI()
	{
		this.m_newHeroFavoriteButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.SetFavoriteHero();
			this.CancelPreview();
		});
		if (this.m_vanillaHeroFavoriteButton != null && this.m_vanillaHeroFavoriteButton != this.m_newHeroFavoriteButton)
		{
			this.m_vanillaHeroFavoriteButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
			{
				this.SetFavoriteHero();
				this.CancelPreview();
			});
		}
		this.m_offClicker.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.CancelPreview();
		});
		this.m_offClicker.AddEventListener(UIEventType.RIGHTCLICK, delegate(UIEvent e)
		{
			this.CancelPreview();
		});
	}

	// Token: 0x0600494E RID: 18766 RVA: 0x0015E958 File Offset: 0x0015CB58
	private void SetFavoriteHero()
	{
		NetCache.CardDefinition hero = new NetCache.CardDefinition
		{
			Name = this.m_currentEntityDef.GetCardId(),
			Premium = this.m_currentPremium
		};
		Network.SetFavoriteHero(this.m_currentEntityDef.GetClass(), hero);
	}

	// Token: 0x0600494F RID: 18767 RVA: 0x0015E99C File Offset: 0x0015CB9C
	private void AssignVanillaHeroPreviewMaterial(Material material, Texture portraitTexture)
	{
		if (portraitTexture != null)
		{
			Material[] materials = this.m_vanillaHeroPreviewQuad.GetComponent<Renderer>().materials;
			materials[1] = material;
			materials[1].SetTexture("_MainTex", portraitTexture);
			this.m_vanillaHeroPreviewQuad.GetComponent<Renderer>().materials = materials;
		}
		else
		{
			RenderUtils.SetMaterial(this.m_vanillaHeroPreviewQuad, 1, material);
		}
	}

	// Token: 0x06004950 RID: 18768 RVA: 0x0015E9FC File Offset: 0x0015CBFC
	private void AssignNewHeroPreviewMaterial(Material material, Texture portraitTexture)
	{
		if (material == null)
		{
			this.m_newHeroPreviewQuad.GetComponent<Renderer>().material = this.m_defaultPreviewMaterial;
			this.m_newHeroPreviewQuad.GetComponent<Renderer>().material.mainTexture = portraitTexture;
		}
		else
		{
			this.m_newHeroPreviewQuad.GetComponent<Renderer>().material = material;
		}
	}

	// Token: 0x06004951 RID: 18769 RVA: 0x0015EA58 File Offset: 0x0015CC58
	private void PlayHeroMusic()
	{
		MusicPlaylistType musicPlaylistType;
		if (this.m_currentHeroDef == null || this.m_currentHeroDef.m_heroPlaylist == MusicPlaylistType.Invalid)
		{
			musicPlaylistType = this.m_defaultHeroMusic;
		}
		else
		{
			musicPlaylistType = this.m_currentHeroDef.m_heroPlaylist;
		}
		if (musicPlaylistType != MusicPlaylistType.Invalid)
		{
			this.m_prevPlaylist = MusicManager.Get().GetCurrentPlaylist();
			MusicManager.Get().StartPlaylist(musicPlaylistType);
		}
	}

	// Token: 0x06004952 RID: 18770 RVA: 0x0015EAC0 File Offset: 0x0015CCC0
	private void StopHeroMusic()
	{
		MusicManager.Get().StartPlaylist(this.m_prevPlaylist);
	}

	// Token: 0x04003063 RID: 12387
	public GameObject m_previewPane;

	// Token: 0x04003064 RID: 12388
	public GameObject m_vanillaHeroFrame;

	// Token: 0x04003065 RID: 12389
	public MeshRenderer m_vanillaHeroPreviewQuad;

	// Token: 0x04003066 RID: 12390
	public UberText m_vanillaHeroTitle;

	// Token: 0x04003067 RID: 12391
	public UberText m_vanillaHeroDescription;

	// Token: 0x04003068 RID: 12392
	public UIBButton m_vanillaHeroFavoriteButton;

	// Token: 0x04003069 RID: 12393
	public GameObject m_newHeroFrame;

	// Token: 0x0400306A RID: 12394
	public MeshRenderer m_newHeroPreviewQuad;

	// Token: 0x0400306B RID: 12395
	public UberText m_newHeroTitle;

	// Token: 0x0400306C RID: 12396
	public UberText m_newHeroDescription;

	// Token: 0x0400306D RID: 12397
	public UIBButton m_newHeroFavoriteButton;

	// Token: 0x0400306E RID: 12398
	public PegUIElement m_offClicker;

	// Token: 0x0400306F RID: 12399
	public float m_animationTime = 0.5f;

	// Token: 0x04003070 RID: 12400
	public Material m_defaultPreviewMaterial;

	// Token: 0x04003071 RID: 12401
	public Material m_vanillaHeroNonPremiumMaterial;

	// Token: 0x04003072 RID: 12402
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_enterPreviewSound;

	// Token: 0x04003073 RID: 12403
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_exitPreviewSound;

	// Token: 0x04003074 RID: 12404
	public MusicPlaylistType m_defaultHeroMusic = MusicPlaylistType.UI_CMHeroSkinPreview;

	// Token: 0x04003075 RID: 12405
	private string m_currentCardId;

	// Token: 0x04003076 RID: 12406
	private CollectionHeroDef m_currentHeroDef;

	// Token: 0x04003077 RID: 12407
	private HeroDbfRecord m_currentHeroRecord;

	// Token: 0x04003078 RID: 12408
	private EntityDef m_currentEntityDef;

	// Token: 0x04003079 RID: 12409
	private TAG_PREMIUM m_currentPremium;

	// Token: 0x0400307A RID: 12410
	private static HeroSkinInfoManager s_instance;

	// Token: 0x0400307B RID: 12411
	private bool m_animating;

	// Token: 0x0400307C RID: 12412
	private MusicPlaylistType m_prevPlaylist;
}
