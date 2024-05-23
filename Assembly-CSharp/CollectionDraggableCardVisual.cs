using System;
using UnityEngine;

// Token: 0x0200071F RID: 1823
public class CollectionDraggableCardVisual : MonoBehaviour
{
	// Token: 0x06004A72 RID: 19058 RVA: 0x001642E8 File Offset: 0x001624E8
	private void Awake()
	{
		base.gameObject.SetActive(false);
		this.LoadDeckTile();
		this.LoadCardBack();
		if (base.gameObject.GetComponent<AudioSource>() == null)
		{
			base.gameObject.AddComponent<AudioSource>();
		}
	}

	// Token: 0x06004A73 RID: 19059 RVA: 0x00164330 File Offset: 0x00162530
	private void Update()
	{
		if (this.m_deckTileToRemove != null)
		{
			this.m_deckTileToRemove.SetHighlight(false);
		}
		this.m_deckTileToRemove = null;
		if (this.m_activeActor != this.m_deckTile)
		{
			return;
		}
		CollectionDeck taggedDeck = CollectionManager.Get().GetTaggedDeck(CollectionManager.DeckTag.Editing);
		if (taggedDeck == null)
		{
			return;
		}
		RaycastHit raycastHit;
		if (!UniversalInputManager.Get().GetInputHitInfo(DeckTrayDeckTileVisual.LAYER.LayerBit(), out raycastHit))
		{
			return;
		}
		DeckTrayDeckTileVisual component = raycastHit.collider.gameObject.GetComponent<DeckTrayDeckTileVisual>();
		if (component == null)
		{
			return;
		}
		if (component == this.m_deckTileToRemove)
		{
			return;
		}
		if (this.m_deckTile != null && this.m_activeActor != null)
		{
			bool flag = taggedDeck.GetTotalValidCardCount() == CollectionManager.Get().GetDeckSize();
			bool flag2 = !taggedDeck.IsValidSlot(component.GetSlot()) || flag;
			if (flag2)
			{
				NotificationManager.Get().DestroyNotificationWithText(GameStrings.Get("GLUE_COLLECTION_TUTORIAL_TEMPLATE_REPLACE_1"), 0f);
				NotificationManager.Get().DestroyNotificationWithText(GameStrings.Get("GLUE_COLLECTION_TUTORIAL_TEMPLATE_REPLACE_2"), 0f);
				NotificationManager.Get().DestroyNotificationWithText(GameStrings.Get("GLUE_COLLECTION_TUTORIAL_REPLACE_WILD_CARDS"), 0f);
			}
			component.SetHighlight(flag2);
		}
		this.m_deckTileToRemove = component;
	}

	// Token: 0x06004A74 RID: 19060 RVA: 0x0016448A File Offset: 0x0016268A
	public void SetCardBackId(int cardBackId)
	{
		this.m_cardBackId = cardBackId;
	}

	// Token: 0x06004A75 RID: 19061 RVA: 0x00164493 File Offset: 0x00162693
	public int GetCardBackId()
	{
		return this.m_cardBackId;
	}

	// Token: 0x06004A76 RID: 19062 RVA: 0x0016449B File Offset: 0x0016269B
	public string GetCardID()
	{
		return this.m_entityDef.GetCardId();
	}

	// Token: 0x06004A77 RID: 19063 RVA: 0x001644A8 File Offset: 0x001626A8
	public TAG_PREMIUM GetPremium()
	{
		return this.m_premium;
	}

	// Token: 0x06004A78 RID: 19064 RVA: 0x001644B0 File Offset: 0x001626B0
	public EntityDef GetEntityDef()
	{
		return this.m_entityDef;
	}

	// Token: 0x06004A79 RID: 19065 RVA: 0x001644B8 File Offset: 0x001626B8
	public CollectionDeckSlot GetSlot()
	{
		return this.m_slot;
	}

	// Token: 0x06004A7A RID: 19066 RVA: 0x001644C0 File Offset: 0x001626C0
	public void SetSlot(CollectionDeckSlot slot)
	{
		this.m_slot = slot;
	}

	// Token: 0x06004A7B RID: 19067 RVA: 0x001644C9 File Offset: 0x001626C9
	public CollectionManagerDisplay.ViewMode GetVisualType()
	{
		return this.m_visualType;
	}

	// Token: 0x06004A7C RID: 19068 RVA: 0x001644D4 File Offset: 0x001626D4
	public bool ChangeActor(Actor actor, CollectionManagerDisplay.ViewMode vtype)
	{
		if (!this.m_actorCacheInit)
		{
			this.m_actorCacheInit = true;
			this.m_actorCache.AddActorLoadedListener(new HandActorCache.ActorLoadedCallback(this.OnCardActorLoaded));
			this.m_actorCache.Initialize();
		}
		if (this.m_actorCache.IsInitializing())
		{
			return false;
		}
		this.m_visualType = vtype;
		if (this.m_visualType != CollectionManagerDisplay.ViewMode.CARD_BACKS)
		{
			EntityDef entityDef = actor.GetEntityDef();
			TAG_PREMIUM premium = actor.GetPremium();
			bool flag = entityDef != this.m_entityDef;
			bool flag2 = premium != this.m_premium;
			if (!flag && !flag2)
			{
				return true;
			}
			this.m_entityDef = entityDef;
			this.m_premium = premium;
			this.m_cardActor = this.m_actorCache.GetActor(entityDef, premium);
			if (this.m_cardActor == null)
			{
				return false;
			}
			if (flag)
			{
				DefLoader.Get().LoadCardDef(this.m_entityDef.GetCardId(), new DefLoader.LoadDefCallback<CardDef>(this.OnCardDefLoaded), new CardPortraitQuality(1, this.m_premium), null);
			}
			else
			{
				this.InitDeckTileActor();
				this.InitCardActor();
			}
			return true;
		}
		else
		{
			if (actor != null)
			{
				this.m_entityDef = null;
				this.m_premium = TAG_PREMIUM.NORMAL;
				this.m_currentCardBack = actor.GetComponentInChildren<CardBack>();
				this.m_cardActor = this.m_cardBackActor;
				this.m_cardBackActor.SetCardbackUpdateIgnore(true);
				return true;
			}
			return false;
		}
	}

	// Token: 0x06004A7D RID: 19069 RVA: 0x0016462C File Offset: 0x0016282C
	public void UpdateVisual(bool isOverDeck)
	{
		Actor activeActor = this.m_activeActor;
		bool flag = this.m_visualType == CollectionManagerDisplay.ViewMode.CARDS;
		SpellType spellType;
		if (flag)
		{
			if (isOverDeck && this.m_entityDef != null && !this.m_entityDef.IsHero())
			{
				this.m_activeActor = this.m_deckTile;
				spellType = SpellType.SUMMON_IN;
			}
			else
			{
				this.m_activeActor = this.m_cardActor;
				spellType = SpellType.DEATHREVERSE;
			}
		}
		else
		{
			this.m_activeActor = this.m_cardActor;
			spellType = SpellType.DEATHREVERSE;
			if (this.m_deckTileToRemove != null)
			{
				this.m_deckTileToRemove.SetHighlight(false);
			}
		}
		if (activeActor == this.m_activeActor)
		{
			return;
		}
		if (activeActor != null)
		{
			activeActor.Hide();
			activeActor.gameObject.SetActive(false);
		}
		if (this.m_activeActor == null)
		{
			return;
		}
		this.m_activeActor.gameObject.SetActive(true);
		this.m_activeActor.Show();
		if (this.m_visualType == CollectionManagerDisplay.ViewMode.CARD_BACKS && this.m_currentCardBack != null)
		{
			CardBackManager.Get().UpdateCardBack(this.m_activeActor, this.m_currentCardBack);
		}
		Spell spell = this.m_activeActor.GetSpell(spellType);
		if (spell != null)
		{
			spell.ActivateState(SpellStateType.BIRTH);
		}
		if (this.m_entityDef != null && this.m_entityDef.IsHero())
		{
			CollectionHeroSkin component = this.m_activeActor.gameObject.GetComponent<CollectionHeroSkin>();
			component.SetClass(this.m_entityDef.GetClass());
			component.ShowSocketFX();
		}
	}

	// Token: 0x06004A7E RID: 19070 RVA: 0x001647BD File Offset: 0x001629BD
	public bool IsShown()
	{
		return base.gameObject.activeSelf;
	}

	// Token: 0x06004A7F RID: 19071 RVA: 0x001647CC File Offset: 0x001629CC
	public void Show(bool isOverDeck)
	{
		base.gameObject.SetActive(true);
		this.UpdateVisual(isOverDeck);
		if (this.m_deckTile != null && this.m_entityDef != null)
		{
			this.m_deckTile.UpdateDeckCardProperties(this.m_entityDef.IsElite(), 1, false);
		}
	}

	// Token: 0x06004A80 RID: 19072 RVA: 0x00164820 File Offset: 0x00162A20
	public void Hide()
	{
		base.gameObject.SetActive(false);
		if (this.m_activeActor == null)
		{
			return;
		}
		this.m_activeActor.Hide();
		this.m_activeActor.gameObject.SetActive(false);
		this.m_activeActor = null;
	}

	// Token: 0x06004A81 RID: 19073 RVA: 0x0016486E File Offset: 0x00162A6E
	public DeckTrayDeckTileVisual GetDeckTileToRemove()
	{
		return this.m_deckTileToRemove;
	}

	// Token: 0x06004A82 RID: 19074 RVA: 0x00164878 File Offset: 0x00162A78
	private void LoadDeckTile()
	{
		GameObject gameObject = AssetLoader.Get().LoadActor("DeckCardBar", false, false);
		if (gameObject == null)
		{
			Debug.LogWarning(string.Format("CollectionDraggableCardVisual.OnDeckTileActorLoaded() - FAILED to load actor \"{0}\"", "DeckCardBar"));
			return;
		}
		this.m_deckTile = gameObject.GetComponent<CollectionDeckTileActor>();
		if (this.m_deckTile == null)
		{
			Debug.LogWarning(string.Format("CollectionDraggableCardVisual.OnDeckTileActorLoaded() - ERROR game object \"{0}\" has no CollectionDeckTileActor component", "DeckCardBar"));
			return;
		}
		this.m_deckTile.Hide();
		this.m_deckTile.transform.parent = base.transform;
		this.m_deckTile.transform.localPosition = new Vector3(2.194931f, 0f, 0f);
		this.m_deckTile.transform.localScale = CollectionDraggableCardVisual.DECK_TILE_LOCAL_SCALE;
		this.m_deckTile.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
	}

	// Token: 0x06004A83 RID: 19075 RVA: 0x00164968 File Offset: 0x00162B68
	private void LoadCardBack()
	{
		GameObject gameObject = AssetLoader.Get().LoadActor("Card_Hidden", false, false);
		GameUtils.SetParent(gameObject, this, false);
		this.m_cardBackActor = gameObject.GetComponent<Actor>();
		this.m_cardBackActor.transform.localScale = CollectionDraggableCardVisual.CARD_ACTOR_LOCAL_SCALE;
		this.m_cardBackActor.TurnOffCollider();
		this.m_cardBackActor.Hide();
		gameObject.AddComponent<DragRotator>().SetInfo(this.m_CardDragRotatorInfo);
	}

	// Token: 0x06004A84 RID: 19076 RVA: 0x001649D8 File Offset: 0x00162BD8
	private void OnCardActorLoaded(string name, Actor actor, object callbackData)
	{
		if (actor == null)
		{
			Debug.LogWarning(string.Format("CollectionDraggableCardVisual.OnCardActorLoaded() - FAILED to load {0}", name));
			return;
		}
		actor.GetType();
		actor.TurnOffCollider();
		actor.Hide();
		if (name == "Card_Hero_Skin")
		{
			actor.transform.localScale = CollectionDraggableCardVisual.HERO_SKIN_ACTOR_LOCAL_SCALE;
		}
		else
		{
			actor.transform.localScale = CollectionDraggableCardVisual.CARD_ACTOR_LOCAL_SCALE;
		}
		actor.transform.parent = base.transform;
		actor.transform.localPosition = Vector3.zero;
		DragRotator dragRotator = actor.gameObject.AddComponent<DragRotator>();
		dragRotator.SetInfo(this.m_CardDragRotatorInfo);
	}

	// Token: 0x06004A85 RID: 19077 RVA: 0x00164A84 File Offset: 0x00162C84
	private void OnCardDefLoaded(string cardID, CardDef cardDef, object callbackData)
	{
		if (this.m_entityDef == null)
		{
			return;
		}
		if (this.m_entityDef.GetCardId() != cardID)
		{
			return;
		}
		this.m_cardDef = cardDef;
		this.InitDeckTileActor();
		this.InitCardActor();
	}

	// Token: 0x06004A86 RID: 19078 RVA: 0x00164AC8 File Offset: 0x00162CC8
	private void InitDeckTileActor()
	{
		this.InitActor(this.m_deckTile);
		this.m_deckTile.SetCardDef(this.m_cardDef);
		this.m_deckTile.SetDisablePremiumPortrait(true);
		this.m_deckTile.UpdateAllComponents();
		this.m_deckTile.UpdateMaterial(this.m_cardDef.GetDeckCardBarPortrait());
		this.m_deckTile.UpdateDeckCardProperties(this.m_entityDef.IsElite(), 1, false);
	}

	// Token: 0x06004A87 RID: 19079 RVA: 0x00164B37 File Offset: 0x00162D37
	private void InitCardActor()
	{
		this.InitActor(this.m_cardActor);
		this.m_cardActor.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x06004A88 RID: 19080 RVA: 0x00164B5A File Offset: 0x00162D5A
	private void InitActor(Actor actor)
	{
		actor.SetEntityDef(this.m_entityDef);
		actor.SetCardDef(this.m_cardDef);
		actor.SetPremium(this.m_premium);
		actor.UpdateAllComponents();
	}

	// Token: 0x04003182 RID: 12674
	public DragRotatorInfo m_CardDragRotatorInfo = new DragRotatorInfo
	{
		m_PitchInfo = new DragRotatorAxisInfo
		{
			m_ForceMultiplier = 3f,
			m_MinDegrees = -55f,
			m_MaxDegrees = 55f,
			m_RestSeconds = 2f
		},
		m_RollInfo = new DragRotatorAxisInfo
		{
			m_ForceMultiplier = 4.5f,
			m_MinDegrees = -60f,
			m_MaxDegrees = 60f,
			m_RestSeconds = 2f
		}
	};

	// Token: 0x04003183 RID: 12675
	private static readonly Vector3 DECK_TILE_LOCAL_SCALE = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
	{
		PC = new Vector3(0.6f, 0.6f, 0.6f),
		Phone = new Vector3(0.9f, 0.9f, 0.9f)
	};

	// Token: 0x04003184 RID: 12676
	private static readonly Vector3 CARD_ACTOR_LOCAL_SCALE = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
	{
		PC = new Vector3(6f, 6f, 6f),
		Phone = new Vector3(6.9f, 6.9f, 6.9f)
	};

	// Token: 0x04003185 RID: 12677
	private static readonly Vector3 HERO_SKIN_ACTOR_LOCAL_SCALE = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
	{
		PC = new Vector3(7.5f, 7.5f, 7.5f),
		Phone = new Vector3(8.2f, 8.2f, 8.2f)
	};

	// Token: 0x04003186 RID: 12678
	private CollectionDeckSlot m_slot;

	// Token: 0x04003187 RID: 12679
	private DeckTrayDeckTileVisual m_deckTileToRemove;

	// Token: 0x04003188 RID: 12680
	private Actor m_cardBackActor;

	// Token: 0x04003189 RID: 12681
	private CardBack m_currentCardBack;

	// Token: 0x0400318A RID: 12682
	private EntityDef m_entityDef;

	// Token: 0x0400318B RID: 12683
	private TAG_PREMIUM m_premium;

	// Token: 0x0400318C RID: 12684
	private CardDef m_cardDef;

	// Token: 0x0400318D RID: 12685
	private Actor m_activeActor;

	// Token: 0x0400318E RID: 12686
	private CollectionDeckTileActor m_deckTile;

	// Token: 0x0400318F RID: 12687
	private Actor m_cardActor;

	// Token: 0x04003190 RID: 12688
	private string m_cardActorName;

	// Token: 0x04003191 RID: 12689
	private HandActorCache m_actorCache = new HandActorCache();

	// Token: 0x04003192 RID: 12690
	private bool m_actorCacheInit;

	// Token: 0x04003193 RID: 12691
	private CollectionManagerDisplay.ViewMode m_visualType;

	// Token: 0x04003194 RID: 12692
	private int m_cardBackId;
}
