using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000720 RID: 1824
[CustomEditClass]
public class DeckTrayCardBackContent : DeckTrayContent
{
	// Token: 0x06004A8A RID: 19082 RVA: 0x00164BA4 File Offset: 0x00162DA4
	private void Awake()
	{
		this.m_originalLocalPosition = base.transform.localPosition;
		base.transform.localPosition = this.m_originalLocalPosition + this.m_trayHiddenOffset;
		this.m_root.SetActive(false);
	}

	// Token: 0x06004A8B RID: 19083 RVA: 0x00164BEC File Offset: 0x00162DEC
	public void AnimateInNewCardBack(CardBackManager.LoadCardBackData cardBackData, GameObject original)
	{
		GameObject gameObject = cardBackData.m_GameObject;
		Actor component = gameObject.GetComponent<Actor>();
		Spell spell = component.GetSpell(SpellType.DEATHREVERSE);
		spell.Reactivate();
		DeckTrayCardBackContent.AnimatedCardBack animatedCardBack = new DeckTrayCardBackContent.AnimatedCardBack();
		animatedCardBack.CardBackId = cardBackData.m_CardBackIndex;
		animatedCardBack.GameObject = gameObject;
		animatedCardBack.OriginalScale = gameObject.transform.localScale;
		animatedCardBack.OriginalPosition = original.transform.position;
		this.m_animData = animatedCardBack;
		gameObject.transform.position = new Vector3(original.transform.position.x, original.transform.position.y + 0.5f, original.transform.position.z);
		gameObject.transform.localScale = this.m_cardBackContainer.transform.lossyScale;
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
			"AnimateNewCardUpdate",
			"onupdatetarget",
			base.gameObject,
			"oncomplete",
			"AnimateNewCardFinished",
			"oncompleteparams",
			animatedCardBack,
			"oncompletetarget",
			base.gameObject
		});
		iTween.ValueTo(gameObject, args);
		SoundManager.Get().LoadAndPlay("collection_manager_card_add_to_deck_instant", base.gameObject);
	}

	// Token: 0x06004A8C RID: 19084 RVA: 0x00164D98 File Offset: 0x00162F98
	private void AnimateNewCardFinished(DeckTrayCardBackContent.AnimatedCardBack cardBack)
	{
		cardBack.GameObject.transform.localScale = cardBack.OriginalScale;
		this.UpdateCardBack(cardBack.CardBackId, true, cardBack.GameObject);
		this.m_animData = null;
	}

	// Token: 0x06004A8D RID: 19085 RVA: 0x00164DD8 File Offset: 0x00162FD8
	private void AnimateNewCardUpdate(float val)
	{
		GameObject gameObject = this.m_animData.GameObject;
		Vector3 originalPosition = this.m_animData.OriginalPosition;
		Vector3 position = this.m_cardBackContainer.transform.position;
		if (val <= 0.85f)
		{
			val /= 0.85f;
			gameObject.transform.position = new Vector3(Mathf.Lerp(originalPosition.x, position.x, val), Mathf.Lerp(originalPosition.y, position.y, val) + Mathf.Sin(val * 3.1415927f) * 15f + val * 4f, Mathf.Lerp(originalPosition.z, position.z, val));
		}
		else
		{
			if (this.m_currentCardBack != null)
			{
				Object.Destroy(this.m_currentCardBack);
				this.m_currentCardBack = null;
			}
			val = (val - 0.85f) / 0.14999998f;
			gameObject.transform.position = new Vector3(position.x, position.y + Mathf.Lerp(4f, 0f, val), position.z);
		}
	}

	// Token: 0x06004A8E RID: 19086 RVA: 0x00164EF8 File Offset: 0x001630F8
	public bool SetNewCardBack(int cardBackId, GameObject original)
	{
		if (this.m_animData != null)
		{
			return false;
		}
		if (!CardBackManager.Get().LoadCardBackByIndex(cardBackId, delegate(CardBackManager.LoadCardBackData cardBackData)
		{
			this.AnimateInNewCardBack(cardBackData, original);
		}, "Card_Hidden"))
		{
			Debug.LogError("Could not load CardBack " + cardBackId);
			return false;
		}
		return true;
	}

	// Token: 0x06004A8F RID: 19087 RVA: 0x00164F64 File Offset: 0x00163164
	public void UpdateCardBack(int cardBackId, bool assigning, GameObject obj = null)
	{
		CollectionDeck currentDeck = CollectionManager.Get().GetTaggedDeck(CollectionManager.DeckTag.Editing);
		if (assigning)
		{
			if (!string.IsNullOrEmpty(this.m_socketSound))
			{
				SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_socketSound));
			}
			currentDeck.CardBackOverridden = true;
		}
		currentDeck.CardBackID = cardBackId;
		if (obj != null)
		{
			this.SetCardBack(obj, currentDeck.CardBackOverridden, assigning);
			return;
		}
		this.m_waitingToLoadCardback = true;
		if (!CardBackManager.Get().LoadCardBackByIndex(cardBackId, delegate(CardBackManager.LoadCardBackData cardBackData)
		{
			this.m_waitingToLoadCardback = false;
			GameObject gameObject = cardBackData.m_GameObject;
			this.SetCardBack(gameObject, currentDeck.CardBackOverridden, assigning);
		}, "Card_Hidden"))
		{
			this.m_waitingToLoadCardback = false;
			Debug.LogWarning(string.Format("CardBackManager was unable to load card back ID: {0}", cardBackId));
		}
	}

	// Token: 0x06004A90 RID: 19088 RVA: 0x0016504C File Offset: 0x0016324C
	private void SetCardBack(GameObject go, bool overriden, bool assigning)
	{
		GameUtils.SetParent(go, this.m_cardBackContainer, true);
		Actor component = go.GetComponent<Actor>();
		if (component == null)
		{
			Object.Destroy(go);
			return;
		}
		if (assigning)
		{
			Spell spell = component.GetSpell(SpellType.DEATHREVERSE);
			if (spell != null)
			{
				spell.ActivateState(SpellStateType.BIRTH);
			}
		}
		if (this.m_currentCardBack != null)
		{
			Object.Destroy(this.m_currentCardBack);
		}
		this.m_currentCardBack = go;
		GameObject cardMesh = component.m_cardMesh;
		component.SetCardbackUpdateIgnore(true);
		component.SetUnlit();
		this.UpdateMissingEffect(component, overriden);
		if (cardMesh != null)
		{
			Material material = cardMesh.GetComponent<Renderer>().material;
			if (material.HasProperty("_SpecularIntensity"))
			{
				material.SetFloat("_SpecularIntensity", 0f);
			}
		}
	}

	// Token: 0x06004A91 RID: 19089 RVA: 0x0016511C File Offset: 0x0016331C
	public override bool PreAnimateContentEntrance()
	{
		CollectionDeck taggedDeck = CollectionManager.Get().GetTaggedDeck(CollectionManager.DeckTag.Editing);
		this.UpdateCardBack(taggedDeck.CardBackID, false, null);
		return true;
	}

	// Token: 0x06004A92 RID: 19090 RVA: 0x00165144 File Offset: 0x00163344
	public override bool AnimateContentEntranceStart()
	{
		if (this.m_waitingToLoadCardback)
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

	// Token: 0x06004A93 RID: 19091 RVA: 0x00165210 File Offset: 0x00163410
	public override bool AnimateContentEntranceEnd()
	{
		return !this.m_animating;
	}

	// Token: 0x06004A94 RID: 19092 RVA: 0x0016521C File Offset: 0x0016341C
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

	// Token: 0x06004A95 RID: 19093 RVA: 0x001652CF File Offset: 0x001634CF
	public override bool AnimateContentExitEnd()
	{
		return !this.m_animating;
	}

	// Token: 0x06004A96 RID: 19094 RVA: 0x001652DC File Offset: 0x001634DC
	private void UpdateMissingEffect(Actor cardBackActor, bool overriden)
	{
		if (cardBackActor == null)
		{
			return;
		}
		if (overriden)
		{
			cardBackActor.DisableMissingCardEffect();
		}
		else
		{
			cardBackActor.SetMissingCardMaterial(this.m_sepiaCardMaterial);
			cardBackActor.MissingCardEffect();
		}
		cardBackActor.UpdateAllComponents();
	}

	// Token: 0x04003195 RID: 12693
	private const string ADD_CARD_TO_DECK_SOUND = "collection_manager_card_add_to_deck_instant";

	// Token: 0x04003196 RID: 12694
	[CustomEditField(Sections = "Positioning")]
	public GameObject m_root;

	// Token: 0x04003197 RID: 12695
	[CustomEditField(Sections = "Positioning")]
	public Vector3 m_trayHiddenOffset;

	// Token: 0x04003198 RID: 12696
	[CustomEditField(Sections = "Positioning")]
	public GameObject m_cardBackContainer;

	// Token: 0x04003199 RID: 12697
	[CustomEditField(Sections = "Animation & Sounds")]
	public iTween.EaseType m_traySlideSlideInAnimation = iTween.EaseType.easeOutBounce;

	// Token: 0x0400319A RID: 12698
	[CustomEditField(Sections = "Animation & Sounds")]
	public iTween.EaseType m_traySlideSlideOutAnimation;

	// Token: 0x0400319B RID: 12699
	[CustomEditField(Sections = "Animation & Sounds")]
	public float m_traySlideAnimationTime = 0.25f;

	// Token: 0x0400319C RID: 12700
	[CustomEditField(Sections = "Animation & Sounds", T = EditType.SOUND_PREFAB)]
	public string m_socketSound;

	// Token: 0x0400319D RID: 12701
	[CustomEditField(Sections = "Card Effects")]
	public Material m_sepiaCardMaterial;

	// Token: 0x0400319E RID: 12702
	private GameObject m_currentCardBack;

	// Token: 0x0400319F RID: 12703
	private Vector3 m_originalLocalPosition;

	// Token: 0x040031A0 RID: 12704
	private bool m_animating;

	// Token: 0x040031A1 RID: 12705
	private bool m_waitingToLoadCardback;

	// Token: 0x040031A2 RID: 12706
	private DeckTrayCardBackContent.AnimatedCardBack m_animData;

	// Token: 0x02000767 RID: 1895
	private class AnimatedCardBack
	{
		// Token: 0x040032E8 RID: 13032
		public int CardBackId;

		// Token: 0x040032E9 RID: 13033
		public GameObject GameObject;

		// Token: 0x040032EA RID: 13034
		public Vector3 OriginalScale;

		// Token: 0x040032EB RID: 13035
		public Vector3 OriginalPosition;
	}
}
