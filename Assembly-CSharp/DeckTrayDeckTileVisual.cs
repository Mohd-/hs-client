using System;
using UnityEngine;

// Token: 0x020005C4 RID: 1476
public class DeckTrayDeckTileVisual : PegUIElement
{
	// Token: 0x060041EC RID: 16876 RVA: 0x0013E30C File Offset: 0x0013C50C
	protected override void Awake()
	{
		base.Awake();
		string text = (!UniversalInputManager.UsePhoneUI) ? "DeckCardBar" : "DeckCardBar_phone";
		GameObject gameObject = AssetLoader.Get().LoadActor(text, false, false);
		if (gameObject == null)
		{
			Debug.LogWarning(string.Format("DeckTrayDeckTileVisual.OnDeckTileActorLoaded() - FAILED to load actor \"{0}\"", text));
			return;
		}
		this.m_actor = gameObject.GetComponent<CollectionDeckTileActor>();
		if (this.m_actor == null)
		{
			Debug.LogWarning(string.Format("DeckTrayDeckTileVisual.OnDeckTileActorLoaded() - ERROR game object \"{0}\" has no CollectionDeckTileActor component", text));
			return;
		}
		GameUtils.SetParent(this.m_actor, this, false);
		this.m_actor.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
		UIBScrollableItem component = this.m_actor.GetComponent<UIBScrollableItem>();
		if (component != null)
		{
			component.SetCustomActiveState(new UIBScrollableItem.ActiveStateCallback(this.IsInUse));
		}
		this.SetUpActor();
		if (base.gameObject.GetComponent<BoxCollider>() == null)
		{
			this.m_collider = base.gameObject.AddComponent<BoxCollider>();
			this.m_collider.size = this.BOX_COLLIDER_SIZE;
			this.m_collider.center = this.BOX_COLLIDER_CENTER;
		}
		this.Hide();
		SceneUtils.SetLayer(base.gameObject, DeckTrayDeckTileVisual.LAYER);
		base.SetDragTolerance(5f);
	}

	// Token: 0x060041ED RID: 16877 RVA: 0x0013E462 File Offset: 0x0013C662
	public string GetCardID()
	{
		return this.m_actor.GetEntityDef().GetCardId();
	}

	// Token: 0x060041EE RID: 16878 RVA: 0x0013E474 File Offset: 0x0013C674
	public TAG_PREMIUM GetPremium()
	{
		return this.m_actor.GetPremium();
	}

	// Token: 0x060041EF RID: 16879 RVA: 0x0013E481 File Offset: 0x0013C681
	public CollectionDeckSlot GetSlot()
	{
		return this.m_slot;
	}

	// Token: 0x060041F0 RID: 16880 RVA: 0x0013E489 File Offset: 0x0013C689
	public void SetSlot(CollectionDeckSlot s, bool useSliderAnimations)
	{
		this.m_slot = s;
		this.m_useSliderAnimations = useSliderAnimations;
		this.SetUpActor();
	}

	// Token: 0x060041F1 RID: 16881 RVA: 0x0013E49F File Offset: 0x0013C69F
	public CollectionDeckTileActor GetActor()
	{
		return this.m_actor;
	}

	// Token: 0x060041F2 RID: 16882 RVA: 0x0013E4A7 File Offset: 0x0013C6A7
	public Bounds GetBounds()
	{
		return this.m_collider.bounds;
	}

	// Token: 0x060041F3 RID: 16883 RVA: 0x0013E4B4 File Offset: 0x0013C6B4
	public void Show()
	{
		base.gameObject.SetActive(true);
	}

	// Token: 0x060041F4 RID: 16884 RVA: 0x0013E4C2 File Offset: 0x0013C6C2
	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x060041F5 RID: 16885 RVA: 0x0013E4D0 File Offset: 0x0013C6D0
	public void MarkAsUsed()
	{
		this.m_isInUse = true;
	}

	// Token: 0x060041F6 RID: 16886 RVA: 0x0013E4D9 File Offset: 0x0013C6D9
	public void MarkAsUnused()
	{
		this.m_isInUse = false;
		if (this.m_actor == null)
		{
			return;
		}
		this.m_actor.UpdateDeckCardProperties(false, 1, false);
	}

	// Token: 0x060041F7 RID: 16887 RVA: 0x0013E502 File Offset: 0x0013C702
	public bool IsInUse()
	{
		return this.m_isInUse;
	}

	// Token: 0x060041F8 RID: 16888 RVA: 0x0013E50A File Offset: 0x0013C70A
	public void SetInArena(bool inArena)
	{
		this.m_inArena = inArena;
	}

	// Token: 0x060041F9 RID: 16889 RVA: 0x0013E514 File Offset: 0x0013C714
	public void SetHighlight(bool highlight)
	{
		if (this.m_actor.m_highlight != null)
		{
			this.m_actor.m_highlight.SetActive(highlight);
		}
		if (this.m_actor.m_highlightGlow != null)
		{
			if (this.GetGhostedState() == CollectionDeckTileActor.GhostedState.RED)
			{
				this.m_actor.m_highlightGlow.SetActive(highlight);
			}
			else
			{
				this.m_actor.m_highlightGlow.SetActive(false);
			}
		}
	}

	// Token: 0x060041FA RID: 16890 RVA: 0x0013E591 File Offset: 0x0013C791
	public void UpdateGhostedState()
	{
		this.m_actor.SetGhosted(this.GetGhostedState());
		this.m_actor.UpdateGhostTileEffect();
	}

	// Token: 0x060041FB RID: 16891 RVA: 0x0013E5B0 File Offset: 0x0013C7B0
	private CollectionDeckTileActor.GhostedState GetGhostedState()
	{
		bool flag = SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER;
		if (flag)
		{
			if (!this.m_slot.Owned)
			{
				return CollectionDeckTileActor.GhostedState.BLUE;
			}
			if (!CollectionManager.Get().GetEditedDeck().IsValidSlot(this.m_slot))
			{
				return CollectionDeckTileActor.GhostedState.RED;
			}
		}
		return CollectionDeckTileActor.GhostedState.NONE;
	}

	// Token: 0x060041FC RID: 16892 RVA: 0x0013E600 File Offset: 0x0013C800
	private void SetUpActor()
	{
		if (this.m_actor == null)
		{
			return;
		}
		if (this.m_slot == null)
		{
			return;
		}
		if (string.IsNullOrEmpty(this.m_slot.CardID))
		{
			return;
		}
		EntityDef entityDef = DefLoader.Get().GetEntityDef(this.m_slot.CardID);
		this.m_actor.SetDisablePremiumPortrait(true);
		this.m_actor.SetEntityDef(entityDef);
		this.m_actor.SetPremium(this.m_slot.Premium);
		this.m_actor.SetGhosted(this.GetGhostedState());
		bool flag = entityDef != null && entityDef.IsElite();
		if (flag && this.m_inArena && this.m_slot.Count > 1)
		{
			flag = false;
		}
		this.m_actor.UpdateDeckCardProperties(flag, this.m_slot.Count, this.m_useSliderAnimations);
		DefLoader.Get().LoadCardDef(entityDef.GetCardId(), delegate(string cardID, CardDef cardDef, object data)
		{
			if (this.m_actor == null)
			{
				return;
			}
			if (!cardID.Equals(this.m_actor.GetEntityDef().GetCardId()))
			{
				return;
			}
			this.m_actor.SetCardDef(cardDef);
			this.m_actor.UpdateAllComponents();
			this.m_actor.UpdateMaterial(cardDef.GetDeckCardBarPortrait());
			this.m_actor.UpdateGhostTileEffect();
		}, null, new CardPortraitQuality(1, this.m_slot.Premium));
	}

	// Token: 0x040029EB RID: 10731
	protected const int DEFAULT_PORTRAIT_QUALITY = 1;

	// Token: 0x040029EC RID: 10732
	public static readonly GameLayer LAYER = GameLayer.CardRaycast;

	// Token: 0x040029ED RID: 10733
	private readonly Vector3 BOX_COLLIDER_SIZE = new Vector3(25.34f, 2.14f, 3.68f);

	// Token: 0x040029EE RID: 10734
	private readonly Vector3 BOX_COLLIDER_CENTER = new Vector3(-1.4f, 0f, 0f);

	// Token: 0x040029EF RID: 10735
	protected CollectionDeckSlot m_slot;

	// Token: 0x040029F0 RID: 10736
	protected BoxCollider m_collider;

	// Token: 0x040029F1 RID: 10737
	protected CollectionDeckTileActor m_actor;

	// Token: 0x040029F2 RID: 10738
	protected bool m_isInUse;

	// Token: 0x040029F3 RID: 10739
	protected bool m_useSliderAnimations;

	// Token: 0x040029F4 RID: 10740
	protected bool m_inArena;
}
