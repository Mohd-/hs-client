using System;
using UnityEngine;

// Token: 0x02000AD2 RID: 2770
[CustomEditClass]
public class GeneralStoreHeroesContentDisplay : MonoBehaviour
{
	// Token: 0x06005F85 RID: 24453 RVA: 0x001C9290 File Offset: 0x001C7490
	private void Awake()
	{
		if (this.m_defaultBackgroundTexture == null && this.m_backgroundFrame != null && this.m_backgroundMaterialIndex >= 0 && this.m_backgroundMaterialIndex < this.m_backgroundFrame.materials.Length)
		{
			this.m_defaultBackgroundTexture = this.m_backgroundFrame.materials[this.m_backgroundMaterialIndex].GetTexture("_MainTex");
		}
		this.m_previewToggle.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.TogglePreview();
		});
	}

	// Token: 0x06005F86 RID: 24454 RVA: 0x001C931E File Offset: 0x001C751E
	public void SetKeyArtRenderer(MeshRenderer keyArtRenderer)
	{
		this.m_keyArt = keyArtRenderer;
	}

	// Token: 0x06005F87 RID: 24455 RVA: 0x001C9328 File Offset: 0x001C7528
	public void PlayPreviewEmote()
	{
		if (this.m_previewEmote == null)
		{
			return;
		}
		this.m_previewEmote.SetPosition(Box.Get().GetCamera().transform.position);
		this.m_previewEmote.Reactivate();
	}

	// Token: 0x06005F88 RID: 24456 RVA: 0x001C9374 File Offset: 0x001C7574
	public void PlayPurchaseEmote()
	{
		if (this.m_purchaseEmote == null)
		{
			return;
		}
		this.m_purchaseEmote.SetPosition(Box.Get().GetCamera().transform.position);
		this.m_purchaseEmote.Reactivate();
	}

	// Token: 0x06005F89 RID: 24457 RVA: 0x001C93BD File Offset: 0x001C75BD
	public void SetParent(GeneralStoreHeroesContent parent)
	{
		this.m_parent = parent;
	}

	// Token: 0x06005F8A RID: 24458 RVA: 0x001C93C8 File Offset: 0x001C75C8
	public void Init()
	{
		if (this.m_heroActor == null)
		{
			GameObject gameObject = AssetLoader.Get().LoadActor("Card_Play_Hero", false, false);
			this.m_heroActor = gameObject.GetComponent<Actor>();
			this.m_heroActor.SetUnlit();
			this.m_heroActor.Show();
			this.m_heroActor.GetHealthObject().Hide();
			this.m_heroActor.GetAttackObject().Hide();
			GameUtils.SetParent(this.m_heroActor, this.m_heroContainer, true);
			SceneUtils.SetLayer(this.m_heroActor, this.m_heroContainer.layer);
		}
		if (this.m_heroPowerActor == null)
		{
			GameObject gameObject2 = AssetLoader.Get().LoadActor("Card_Play_HeroPower", false, false);
			this.m_heroPowerActor = gameObject2.GetComponent<Actor>();
			this.m_heroPowerActor.SetUnlit();
			this.m_heroPowerActor.Show();
			GameUtils.SetParent(this.m_heroPowerActor, this.m_heroPowerContainer, true);
			SceneUtils.SetLayer(this.m_heroPowerActor, this.m_heroPowerContainer.layer);
		}
	}

	// Token: 0x06005F8B RID: 24459 RVA: 0x001C94CF File Offset: 0x001C76CF
	public void ShowPurchasedCheckmark(bool show)
	{
		if (this.m_purchasedCheckMark != null)
		{
			this.m_purchasedCheckMark.SetActive(show);
		}
	}

	// Token: 0x06005F8C RID: 24460 RVA: 0x001C94F0 File Offset: 0x001C76F0
	public void UpdateFrame(HeroDbfRecord heroDbfRecord, int cardBackIdx, CollectionHeroDef heroDef)
	{
		this.Init();
		this.m_keyArt.material = heroDef.m_previewTexture;
		if (heroDef.m_fauxPlateTexture != null)
		{
			this.m_fauxPlateTexture.material.SetTexture("_MainTex", heroDef.m_fauxPlateTexture);
		}
		DefLoader.Get().LoadFullDef(heroDbfRecord.CardId, delegate(string heroCardId, FullDef heroFullDef, object data1)
		{
			CardDef cardDef = heroFullDef.GetCardDef();
			this.m_heroActor.SetPremium(TAG_PREMIUM.NORMAL);
			this.m_heroActor.SetCardDef(cardDef);
			this.m_heroActor.SetEntityDef(heroFullDef.GetEntityDef());
			this.m_heroActor.UpdateAllComponents();
			this.m_heroActor.Hide();
			this.m_heroName.Text = heroFullDef.GetEntityDef().GetName();
			this.m_className.Text = GameStrings.GetClassName(heroFullDef.GetEntityDef().GetClass());
			string heroPowerCardIdFromHero = GameUtils.GetHeroPowerCardIdFromHero(heroCardId);
			DefLoader.Get().LoadFullDef(heroPowerCardIdFromHero, delegate(string powerCardId, FullDef powerDef, object data2)
			{
				this.m_heroPowerActor.SetPremium(TAG_PREMIUM.GOLDEN);
				this.m_heroPowerActor.SetCardDef(powerDef.GetCardDef());
				this.m_heroPowerActor.SetEntityDef(powerDef.GetEntityDef());
				this.m_heroPowerActor.UpdateAllComponents();
				this.m_heroPowerActor.Hide();
			});
			Vector2 vector;
			if (CollectionPageManager.s_classTextureOffsets.TryGetValue(heroFullDef.GetEntityDef().GetClass(), out vector))
			{
				this.m_classIcon.material.SetTextureOffset("_MainTex", vector);
			}
			this.ClearEmotes();
			if (heroDef.m_storePreviewEmote != EmoteType.INVALID)
			{
				GameUtils.LoadCardDefEmoteSound(cardDef, heroDef.m_storePreviewEmote, delegate(CardSoundSpell spell)
				{
					if (spell == null)
					{
						return;
					}
					this.m_previewEmote = spell;
					GameUtils.SetParent(this.m_previewEmote, this, false);
				});
			}
			if (heroDef.m_storePurchaseEmote != EmoteType.INVALID)
			{
				GameUtils.LoadCardDefEmoteSound(cardDef, heroDef.m_storePurchaseEmote, delegate(CardSoundSpell spell)
				{
					if (spell == null)
					{
						return;
					}
					this.m_purchaseEmote = spell;
					GameUtils.SetParent(this.m_purchaseEmote, this, false);
				});
			}
		});
		if (this.m_cardBack != null)
		{
			Object.Destroy(this.m_cardBack);
			this.m_cardBack = null;
		}
		if (cardBackIdx != 0)
		{
			CardBackManager.Get().LoadCardBackByIndex(cardBackIdx, delegate(CardBackManager.LoadCardBackData cardBackData)
			{
				GameObject gameObject = cardBackData.m_GameObject;
				gameObject.name = "CARD_BACK_" + cardBackIdx;
				this.m_cardBack = gameObject;
				SceneUtils.SetLayer(gameObject, this.m_cardBackContainer.gameObject.layer);
				GameUtils.SetParent(gameObject, this.m_cardBackContainer, false);
				this.m_cardBack.transform.localPosition = Vector3.zero;
				this.m_cardBack.transform.localScale = Vector3.one;
				this.m_cardBack.transform.localRotation = Quaternion.identity;
				AnimationUtil.FloatyPosition(this.m_cardBack, 0.05f, 10f);
			}, "Card_Hidden");
		}
		if (this.m_backgroundFrame != null && this.m_backgroundMaterialIndex >= 0 && this.m_backgroundMaterialIndex <= this.m_backgroundFrame.materials.Length)
		{
			Texture texture = this.m_defaultBackgroundTexture;
			if (heroDbfRecord.StoreBackgroundTexture != null)
			{
				string name = FileUtils.GameAssetPathToName(heroDbfRecord.StoreBackgroundTexture);
				Texture texture2 = AssetLoader.Get().LoadTexture(name, false);
				if (texture2 != null)
				{
					texture = texture2;
				}
			}
			if (texture != null)
			{
				this.m_backgroundFrame.materials[this.m_backgroundMaterialIndex].SetTexture("_MainTex", texture);
			}
		}
	}

	// Token: 0x06005F8D RID: 24461 RVA: 0x001C9670 File Offset: 0x001C7870
	public void TogglePreview()
	{
		if (!string.IsNullOrEmpty(this.m_parent.m_previewButtonClick))
		{
			SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_parent.m_previewButtonClick));
		}
		this.PlayKeyArtAnimation(this.m_keyArtShowing);
		this.m_keyArtShowing = !this.m_keyArtShowing;
		if (!this.m_keyArtShowing)
		{
			this.m_heroActor.Show();
			this.m_heroPowerActor.Show();
			this.PlayPreviewEmote();
		}
		else
		{
			this.m_heroActor.Hide();
			this.m_heroPowerActor.Hide();
		}
	}

	// Token: 0x06005F8E RID: 24462 RVA: 0x001C970C File Offset: 0x001C790C
	public void ResetPreview()
	{
		this.m_keyArtShowing = true;
		this.m_keyArtAnimation.enabled = true;
		this.m_keyArtAnimation.StopPlayback();
		this.m_keyArtAnimation.Play(this.m_parent.m_keyArtAppearAnim, -1, 1f);
		this.m_previewButtonFX.SetActive(false);
	}

	// Token: 0x06005F8F RID: 24463 RVA: 0x001C9760 File Offset: 0x001C7960
	private void PlayKeyArtAnimation(bool showPreview)
	{
		string text;
		string text2;
		if (showPreview)
		{
			text = this.m_parent.m_keyArtFadeAnim;
			text2 = this.m_parent.m_keyArtFadeSound;
		}
		else
		{
			text = this.m_parent.m_keyArtAppearAnim;
			text2 = this.m_parent.m_keyArtAppearSound;
		}
		this.m_previewButtonFX.SetActive(showPreview);
		if (!string.IsNullOrEmpty(text2))
		{
			SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(text2));
		}
		this.m_keyArtAnimation.enabled = true;
		this.m_keyArtAnimation.StopPlayback();
		this.m_keyArtAnimation.Play(text, -1, 0f);
	}

	// Token: 0x06005F90 RID: 24464 RVA: 0x001C97F8 File Offset: 0x001C79F8
	private void ClearEmotes()
	{
		if (this.m_previewEmote != null)
		{
			Object.Destroy(this.m_previewEmote.gameObject);
		}
		if (this.m_purchaseEmote != null)
		{
			Object.Destroy(this.m_purchaseEmote.gameObject);
		}
	}

	// Token: 0x040046E4 RID: 18148
	public UberText m_heroName;

	// Token: 0x040046E5 RID: 18149
	public UberText m_className;

	// Token: 0x040046E6 RID: 18150
	public GameObject m_renderArtQuad;

	// Token: 0x040046E7 RID: 18151
	public UIBButton m_previewToggle;

	// Token: 0x040046E8 RID: 18152
	public Animator m_keyArtAnimation;

	// Token: 0x040046E9 RID: 18153
	public MeshRenderer m_classIcon;

	// Token: 0x040046EA RID: 18154
	public MeshRenderer m_fauxPlateTexture;

	// Token: 0x040046EB RID: 18155
	public MeshRenderer m_backgroundFrame;

	// Token: 0x040046EC RID: 18156
	public int m_backgroundMaterialIndex;

	// Token: 0x040046ED RID: 18157
	private Texture m_defaultBackgroundTexture;

	// Token: 0x040046EE RID: 18158
	public GameObject m_heroContainer;

	// Token: 0x040046EF RID: 18159
	public GameObject m_heroPowerContainer;

	// Token: 0x040046F0 RID: 18160
	public GameObject m_cardBackContainer;

	// Token: 0x040046F1 RID: 18161
	public GameObject m_previewButtonFX;

	// Token: 0x040046F2 RID: 18162
	public GameObject m_purchasedCheckMark;

	// Token: 0x040046F3 RID: 18163
	private GeneralStoreHeroesContent m_parent;

	// Token: 0x040046F4 RID: 18164
	private CollectionHeroDef m_currentHeroAsset;

	// Token: 0x040046F5 RID: 18165
	private GameObject m_cardBack;

	// Token: 0x040046F6 RID: 18166
	private Actor m_heroActor;

	// Token: 0x040046F7 RID: 18167
	private Actor m_heroPowerActor;

	// Token: 0x040046F8 RID: 18168
	private bool m_keyArtShowing = true;

	// Token: 0x040046F9 RID: 18169
	private CardSoundSpell m_previewEmote;

	// Token: 0x040046FA RID: 18170
	private CardSoundSpell m_purchaseEmote;

	// Token: 0x040046FB RID: 18171
	private MeshRenderer m_keyArt;
}
