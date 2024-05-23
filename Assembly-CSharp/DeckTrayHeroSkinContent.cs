using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000721 RID: 1825
[CustomEditClass]
public class DeckTrayHeroSkinContent : DeckTrayContent
{
	// Token: 0x06004A9A RID: 19098 RVA: 0x00165374 File Offset: 0x00163574
	private void Awake()
	{
		this.m_originalLocalPosition = base.transform.localPosition;
		base.transform.localPosition = this.m_originalLocalPosition + this.m_trayHiddenOffset;
		this.m_root.SetActive(false);
		this.LoadHeroSkinActor();
	}

	// Token: 0x06004A9B RID: 19099 RVA: 0x001653C0 File Offset: 0x001635C0
	public void UpdateHeroSkin(EntityDef entityDef, TAG_PREMIUM premium, bool assigning)
	{
		this.UpdateHeroSkin(entityDef.GetCardId(), premium, assigning, null);
	}

	// Token: 0x06004A9C RID: 19100 RVA: 0x001653D4 File Offset: 0x001635D4
	public void UpdateHeroSkin(string cardId, TAG_PREMIUM premium, bool assigning, Actor baseActor = null)
	{
		CollectionDeck taggedDeck = CollectionManager.Get().GetTaggedDeck(CollectionManager.DeckTag.Editing);
		if (assigning)
		{
			if (!string.IsNullOrEmpty(this.m_socketSound))
			{
				SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_socketSound));
			}
			taggedDeck.HeroOverridden = true;
		}
		this.UpdateMissingEffect(taggedDeck.HeroOverridden);
		if (this.m_currentHeroCardId == cardId)
		{
			this.ShowSocketFX();
			return;
		}
		this.m_currentHeroCardId = cardId;
		taggedDeck.HeroCardID = cardId;
		taggedDeck.HeroPremium = premium;
		if (baseActor != null)
		{
			this.UpdateHeroSkinVisual(baseActor.GetEntityDef(), baseActor.GetCardDef(), baseActor.GetPremium(), assigning);
			return;
		}
		this.m_waitingToLoadHeroDef = true;
		DefLoader.Get().LoadFullDef(cardId, delegate(string cardID, FullDef fullDef, object callbackData)
		{
			this.m_waitingToLoadHeroDef = false;
			this.UpdateHeroSkinVisual(fullDef.GetEntityDef(), fullDef.GetCardDef(), premium, assigning);
		});
	}

	// Token: 0x06004A9D RID: 19101 RVA: 0x001654CC File Offset: 0x001636CC
	public void AnimateInNewHeroSkin(Actor actor)
	{
		GameObject gameObject = actor.gameObject;
		DeckTrayHeroSkinContent.AnimatedHeroSkin animatedHeroSkin = new DeckTrayHeroSkinContent.AnimatedHeroSkin();
		animatedHeroSkin.Actor = actor;
		animatedHeroSkin.GameObject = gameObject;
		animatedHeroSkin.OriginalScale = gameObject.transform.localScale;
		animatedHeroSkin.OriginalPosition = gameObject.transform.position;
		this.m_animData = animatedHeroSkin;
		gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.5f, gameObject.transform.position.z);
		gameObject.transform.localScale = this.m_heroSkinContainer.transform.lossyScale;
		Hashtable args = iTween.Hash(new object[]
		{
			"from",
			0f,
			"to",
			1f,
			"time",
			0.6f,
			"easetype",
			iTween.EaseType.easeOutCubic,
			"onupdate",
			"AnimateNewHeroSkinUpdate",
			"onupdatetarget",
			base.gameObject,
			"oncomplete",
			"AnimateNewHeroSkinFinished",
			"oncompleteparams",
			animatedHeroSkin,
			"oncompletetarget",
			base.gameObject
		});
		iTween.ValueTo(gameObject, args);
		CollectionHeroSkin component = actor.gameObject.GetComponent<CollectionHeroSkin>();
		if (component != null)
		{
			component.ShowSocketFX();
		}
		SoundManager.Get().LoadAndPlay("collection_manager_card_add_to_deck_instant", base.gameObject);
	}

	// Token: 0x06004A9E RID: 19102 RVA: 0x00165678 File Offset: 0x00163878
	private void AnimateNewHeroSkinFinished()
	{
		this.m_heroSkinObject.gameObject.SetActive(true);
		Actor actor = this.m_animData.Actor;
		this.UpdateHeroSkin(actor.GetEntityDef().GetCardId(), actor.GetPremium(), true, actor);
		Object.Destroy(this.m_animData.GameObject);
		this.m_animData = null;
	}

	// Token: 0x06004A9F RID: 19103 RVA: 0x001656D4 File Offset: 0x001638D4
	private void AnimateNewHeroSkinUpdate(float val)
	{
		GameObject gameObject = this.m_animData.GameObject;
		Vector3 originalPosition = this.m_animData.OriginalPosition;
		Vector3 position = this.m_heroSkinContainer.transform.position;
		if (val <= 0.85f)
		{
			val /= 0.85f;
			gameObject.transform.position = new Vector3(Mathf.Lerp(originalPosition.x, position.x, val), Mathf.Lerp(originalPosition.y, position.y, val) + Mathf.Sin(val * 3.1415927f) * 15f + val * 4f, Mathf.Lerp(originalPosition.z, position.z, val));
		}
		else
		{
			this.m_heroSkinObject.gameObject.SetActive(false);
			val = (val - 0.85f) / 0.14999998f;
			gameObject.transform.position = new Vector3(position.x, position.y + Mathf.Lerp(4f, 0f, val), position.z);
		}
	}

	// Token: 0x06004AA0 RID: 19104 RVA: 0x001657E4 File Offset: 0x001639E4
	public void SetNewHeroSkin(Actor actor)
	{
		if (this.m_animData != null)
		{
			return;
		}
		Actor actor2 = actor.Clone();
		actor2.SetCardDef(actor.GetCardDef());
		this.AnimateInNewHeroSkin(actor2);
	}

	// Token: 0x06004AA1 RID: 19105 RVA: 0x00165818 File Offset: 0x00163A18
	public override bool PreAnimateContentEntrance()
	{
		CollectionDeck taggedDeck = CollectionManager.Get().GetTaggedDeck(CollectionManager.DeckTag.Editing);
		this.UpdateHeroSkin(taggedDeck.HeroCardID, taggedDeck.HeroPremium, false, null);
		return true;
	}

	// Token: 0x06004AA2 RID: 19106 RVA: 0x00165848 File Offset: 0x00163A48
	public override bool AnimateContentEntranceStart()
	{
		if (this.m_waitingToLoadHeroDef)
		{
			return false;
		}
		this.m_root.SetActive(true);
		base.transform.localPosition = this.m_originalLocalPosition;
		this.m_animating = true;
		iTween.MoveFrom(base.gameObject, iTween.Hash(new object[]
		{
			"position",
			this.m_originalLocalPosition + this.m_trayHiddenOffset,
			"islocal",
			true,
			"time",
			this.m_traySlideAnimationTime,
			"easetype",
			this.m_traySlideSlideInAnimation,
			"oncomplete",
			delegate(object o)
			{
				this.m_animating = false;
			}
		}));
		return true;
	}

	// Token: 0x06004AA3 RID: 19107 RVA: 0x00165914 File Offset: 0x00163B14
	public override bool AnimateContentEntranceEnd()
	{
		return !this.m_animating;
	}

	// Token: 0x06004AA4 RID: 19108 RVA: 0x00165920 File Offset: 0x00163B20
	public override bool AnimateContentExitStart()
	{
		base.transform.localPosition = this.m_originalLocalPosition;
		this.m_animating = true;
		iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
		{
			"position",
			this.m_originalLocalPosition + this.m_trayHiddenOffset,
			"islocal",
			true,
			"time",
			this.m_traySlideAnimationTime,
			"easetype",
			this.m_traySlideSlideOutAnimation,
			"oncomplete",
			delegate(object o)
			{
				this.m_animating = false;
				this.m_root.SetActive(false);
			}
		}));
		return true;
	}

	// Token: 0x06004AA5 RID: 19109 RVA: 0x001659D3 File Offset: 0x00163BD3
	public override bool AnimateContentExitEnd()
	{
		return !this.m_animating;
	}

	// Token: 0x06004AA6 RID: 19110 RVA: 0x001659DE File Offset: 0x00163BDE
	public void RegisterHeroAssignedListener(DeckTrayHeroSkinContent.HeroAssigned dlg)
	{
		this.m_heroAssignedListeners.Add(dlg);
	}

	// Token: 0x06004AA7 RID: 19111 RVA: 0x001659EC File Offset: 0x00163BEC
	public void UnregisterHeroAssignedListener(DeckTrayHeroSkinContent.HeroAssigned dlg)
	{
		this.m_heroAssignedListeners.Remove(dlg);
	}

	// Token: 0x06004AA8 RID: 19112 RVA: 0x001659FC File Offset: 0x00163BFC
	private void LoadHeroSkinActor()
	{
		string heroSkinOrHandActor = ActorNames.GetHeroSkinOrHandActor(TAG_CARDTYPE.HERO, TAG_PREMIUM.NORMAL);
		AssetLoader.Get().LoadActor(heroSkinOrHandActor, delegate(string name, GameObject go, object callbackData)
		{
			if (go == null)
			{
				Debug.LogWarning(string.Format("DeckTrayHeroSkinContent.LoadHeroSkinActor - FAILED to load \"{0}\"", name));
				return;
			}
			Actor component = go.GetComponent<Actor>();
			if (component == null)
			{
				Debug.LogWarning(string.Format("HandActorCache.OnActorLoaded() - ERROR \"{0}\" has no Actor component", name));
				return;
			}
			GameUtils.SetParent(component, this.m_heroSkinContainer, false);
			this.m_heroSkinObject = component;
		}, null, false);
	}

	// Token: 0x06004AA9 RID: 19113 RVA: 0x00165A2C File Offset: 0x00163C2C
	private void UpdateHeroSkinVisual(EntityDef entityDef, CardDef cardDef, TAG_PREMIUM premium, bool assigning)
	{
		if (this.m_heroSkinObject == null)
		{
			Debug.LogError("Hero skin object not loaded yet! Cannot set portrait!");
			return;
		}
		this.m_heroSkinObject.SetEntityDef(entityDef);
		this.m_heroSkinObject.SetCardDef(cardDef);
		this.m_heroSkinObject.SetPremium(premium);
		this.m_heroSkinObject.UpdateAllComponents();
		CollectionHeroSkin component = this.m_heroSkinObject.GetComponent<CollectionHeroSkin>();
		if (component != null)
		{
			component.SetClass(entityDef.GetClass());
		}
		DeckTrayHeroSkinContent.HeroAssigned[] array = this.m_heroAssignedListeners.ToArray();
		foreach (DeckTrayHeroSkinContent.HeroAssigned heroAssigned in array)
		{
			heroAssigned(entityDef.GetCardId());
		}
		if (assigning)
		{
			GameUtils.LoadCardDefEmoteSound(cardDef, EmoteType.PICKED, new GameUtils.EmoteSoundLoaded(this.OnPickEmoteLoaded));
		}
		if (this.m_currentHeroSkinName != null)
		{
			this.m_currentHeroSkinName.Text = entityDef.GetName();
		}
		this.ShowSocketFX();
	}

	// Token: 0x06004AAA RID: 19114 RVA: 0x00165B23 File Offset: 0x00163D23
	private void OnPickEmoteLoaded(CardSoundSpell spell)
	{
		if (spell == null)
		{
			return;
		}
		spell.AddFinishedCallback(new Spell.FinishedCallback(this.OnPickEmoteFinished));
		spell.Reactivate();
	}

	// Token: 0x06004AAB RID: 19115 RVA: 0x00165B4A File Offset: 0x00163D4A
	private void OnPickEmoteFinished(Spell spell, object userData)
	{
		Object.Destroy(spell.gameObject);
	}

	// Token: 0x06004AAC RID: 19116 RVA: 0x00165B58 File Offset: 0x00163D58
	private void ShowSocketFX()
	{
		CollectionHeroSkin component = this.m_heroSkinObject.GetComponent<CollectionHeroSkin>();
		if (component != null)
		{
			component.ShowSocketFX();
		}
	}

	// Token: 0x06004AAD RID: 19117 RVA: 0x00165B83 File Offset: 0x00163D83
	private void UpdateMissingEffect(bool overriden)
	{
		if (overriden)
		{
			this.m_heroSkinObject.DisableMissingCardEffect();
		}
		else
		{
			this.m_heroSkinObject.SetMissingCardMaterial(this.m_sepiaCardMaterial);
			this.m_heroSkinObject.MissingCardEffect();
		}
		this.m_heroSkinObject.UpdateAllComponents();
	}

	// Token: 0x040031A3 RID: 12707
	private const string ADD_CARD_TO_DECK_SOUND = "collection_manager_card_add_to_deck_instant";

	// Token: 0x040031A4 RID: 12708
	[CustomEditField(Sections = "Positioning")]
	public GameObject m_root;

	// Token: 0x040031A5 RID: 12709
	[CustomEditField(Sections = "Positioning")]
	public Vector3 m_trayHiddenOffset;

	// Token: 0x040031A6 RID: 12710
	[CustomEditField(Sections = "Positioning")]
	public GameObject m_heroSkinContainer;

	// Token: 0x040031A7 RID: 12711
	[CustomEditField(Sections = "Animation & Sounds")]
	public iTween.EaseType m_traySlideSlideInAnimation = iTween.EaseType.easeOutBounce;

	// Token: 0x040031A8 RID: 12712
	[CustomEditField(Sections = "Animation & Sounds")]
	public iTween.EaseType m_traySlideSlideOutAnimation;

	// Token: 0x040031A9 RID: 12713
	[CustomEditField(Sections = "Animation & Sounds")]
	public float m_traySlideAnimationTime = 0.25f;

	// Token: 0x040031AA RID: 12714
	[CustomEditField(Sections = "Animation & Sounds", T = EditType.SOUND_PREFAB)]
	public string m_socketSound;

	// Token: 0x040031AB RID: 12715
	public UberText m_currentHeroSkinName;

	// Token: 0x040031AC RID: 12716
	[CustomEditField(Sections = "Card Effects")]
	public Material m_sepiaCardMaterial;

	// Token: 0x040031AD RID: 12717
	private string m_currentHeroCardId;

	// Token: 0x040031AE RID: 12718
	private Actor m_heroSkinObject;

	// Token: 0x040031AF RID: 12719
	private Vector3 m_originalLocalPosition;

	// Token: 0x040031B0 RID: 12720
	private bool m_animating;

	// Token: 0x040031B1 RID: 12721
	private bool m_waitingToLoadHeroDef;

	// Token: 0x040031B2 RID: 12722
	private List<DeckTrayHeroSkinContent.HeroAssigned> m_heroAssignedListeners = new List<DeckTrayHeroSkinContent.HeroAssigned>();

	// Token: 0x040031B3 RID: 12723
	private DeckTrayHeroSkinContent.AnimatedHeroSkin m_animData;

	// Token: 0x0200072B RID: 1835
	// (Invoke) Token: 0x06004AEF RID: 19183
	public delegate void HeroAssigned(string cardId);

	// Token: 0x0200076A RID: 1898
	private class AnimatedHeroSkin
	{
		// Token: 0x040032F1 RID: 13041
		public Actor Actor;

		// Token: 0x040032F2 RID: 13042
		public GameObject GameObject;

		// Token: 0x040032F3 RID: 13043
		public Vector3 OriginalScale;

		// Token: 0x040032F4 RID: 13044
		public Vector3 OriginalPosition;
	}
}
