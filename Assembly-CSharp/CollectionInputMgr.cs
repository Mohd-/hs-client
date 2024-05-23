using System;
using UnityEngine;

// Token: 0x02000208 RID: 520
public class CollectionInputMgr : MonoBehaviour
{
	// Token: 0x06001F8E RID: 8078 RVA: 0x0009A563 File Offset: 0x00098763
	private void Awake()
	{
		CollectionInputMgr.s_instance = this;
		UniversalInputManager.Get().RegisterMouseOnOrOffScreenListener(new UniversalInputManager.MouseOnOrOffScreenCallback(this.OnMouseOnOrOffScreen));
	}

	// Token: 0x06001F8F RID: 8079 RVA: 0x0009A582 File Offset: 0x00098782
	private void OnDestroy()
	{
		CollectionInputMgr.s_instance = null;
	}

	// Token: 0x06001F90 RID: 8080 RVA: 0x0009A58A File Offset: 0x0009878A
	private void Start()
	{
	}

	// Token: 0x06001F91 RID: 8081 RVA: 0x0009A58C File Offset: 0x0009878C
	private void Update()
	{
		this.UpdateHeldCard();
	}

	// Token: 0x06001F92 RID: 8082 RVA: 0x0009A594 File Offset: 0x00098794
	public static CollectionInputMgr Get()
	{
		return CollectionInputMgr.s_instance;
	}

	// Token: 0x06001F93 RID: 8083 RVA: 0x0009A59B File Offset: 0x0009879B
	public void Unload()
	{
		UniversalInputManager.Get().UnregisterMouseOnOrOffScreenListener(new UniversalInputManager.MouseOnOrOffScreenCallback(this.OnMouseOnOrOffScreen));
	}

	// Token: 0x06001F94 RID: 8084 RVA: 0x0009A5B4 File Offset: 0x000987B4
	public bool HandleKeyboardInput()
	{
		if (Input.GetKeyUp(27) && CraftingManager.Get() != null && CraftingManager.Get().IsCardShowing())
		{
			Navigation.GoBack();
			return true;
		}
		return false;
	}

	// Token: 0x06001F95 RID: 8085 RVA: 0x0009A5F8 File Offset: 0x000987F8
	public bool GrabCard(CollectionCardVisual cardVisual)
	{
		Actor actor = cardVisual.GetActor();
		if (!this.CanGrabItem(actor))
		{
			return false;
		}
		if (!this.m_heldCardVisual.ChangeActor(actor, cardVisual.GetVisualType()))
		{
			return false;
		}
		this.m_scrollBar.Pause(true);
		PegCursor.Get().SetMode(PegCursor.Mode.DRAG);
		CollectionCardBack component = actor.GetComponent<CollectionCardBack>();
		this.m_heldCardVisual.SetSlot(null);
		if (component != null)
		{
			this.m_heldCardVisual.SetCardBackId(component.GetCardBackId());
		}
		this.m_heldCardVisual.transform.position = actor.transform.position;
		this.m_heldCardVisual.Show(this.m_mouseIsOverDeck);
		SoundManager.Get().LoadAndPlay("collection_manager_pick_up_card", this.m_heldCardVisual.gameObject);
		return true;
	}

	// Token: 0x06001F96 RID: 8086 RVA: 0x0009A6C4 File Offset: 0x000988C4
	public bool GrabCard(DeckTrayDeckTileVisual deckTileVisual)
	{
		Actor actor = deckTileVisual.GetActor();
		if (!this.CanGrabItem(actor))
		{
			return false;
		}
		if (!this.m_heldCardVisual.ChangeActor(actor, CollectionManagerDisplay.ViewMode.CARDS))
		{
			return false;
		}
		CollectionDeck editingDeck = CollectionDeckTray.Get().GetCardsContent().GetEditingDeck();
		this.m_scrollBar.Pause(true);
		PegCursor.Get().SetMode(PegCursor.Mode.DRAG);
		this.m_heldCardVisual.SetSlot(deckTileVisual.GetSlot());
		this.m_heldCardVisual.transform.position = actor.transform.position;
		this.m_heldCardVisual.Show(this.m_mouseIsOverDeck);
		SoundManager.Get().LoadAndPlay("collection_manager_pick_up_card", this.m_heldCardVisual.gameObject);
		CollectionDeckTray.Get().RemoveCard(this.m_heldCardVisual.GetCardID(), this.m_heldCardVisual.GetPremium(), editingDeck.IsValidSlot(deckTileVisual.GetSlot()));
		if (!Options.Get().GetBool(Option.HAS_REMOVED_CARD_FROM_DECK, false))
		{
			CollectionDeckTray.Get().GetCardsContent().HideDeckHelpPopup();
			Options.Get().SetBool(Option.HAS_REMOVED_CARD_FROM_DECK, true);
		}
		return true;
	}

	// Token: 0x06001F97 RID: 8087 RVA: 0x0009A7DA File Offset: 0x000989DA
	public void DropCard(DeckTrayDeckTileVisual deckTileToRemove)
	{
		this.DropCard(false, deckTileToRemove);
	}

	// Token: 0x06001F98 RID: 8088 RVA: 0x0009A7E4 File Offset: 0x000989E4
	public void SetScrollbar(UIBScrollable scrollbar)
	{
		this.m_scrollBar = scrollbar;
	}

	// Token: 0x06001F99 RID: 8089 RVA: 0x0009A7ED File Offset: 0x000989ED
	public bool IsDraggingScrollbar()
	{
		return this.m_scrollBar != null && this.m_scrollBar.IsDragging();
	}

	// Token: 0x06001F9A RID: 8090 RVA: 0x0009A80E File Offset: 0x00098A0E
	public bool HasHeldCard()
	{
		return this.m_heldCardVisual.IsShown();
	}

	// Token: 0x06001F9B RID: 8091 RVA: 0x0009A81C File Offset: 0x00098A1C
	private bool CanGrabItem(Actor actor)
	{
		return !this.IsDraggingScrollbar() && !this.m_heldCardVisual.IsShown() && !(actor == null);
	}

	// Token: 0x06001F9C RID: 8092 RVA: 0x0009A858 File Offset: 0x00098A58
	private void UpdateHeldCard()
	{
		if (!this.m_heldCardVisual.IsShown())
		{
			return;
		}
		RaycastHit raycastHit;
		if (!UniversalInputManager.Get().GetInputHitInfo(GameLayer.DragPlane.LayerBit(), out raycastHit))
		{
			return;
		}
		this.m_heldCardVisual.transform.position = raycastHit.point;
		this.m_mouseIsOverDeck = CollectionDeckTray.Get().MouseIsOver();
		this.m_heldCardVisual.UpdateVisual(this.m_mouseIsOverDeck);
		if (Input.GetMouseButtonUp(0))
		{
			this.DropCard(this.m_heldCardVisual.GetDeckTileToRemove());
		}
	}

	// Token: 0x06001F9D RID: 8093 RVA: 0x0009A8E8 File Offset: 0x00098AE8
	private void DropCard(bool dragCanceled, DeckTrayDeckTileVisual deckTileToRemove)
	{
		PegCursor.Get().SetMode(PegCursor.Mode.STOPDRAG);
		if (!dragCanceled)
		{
			if (this.m_mouseIsOverDeck)
			{
				switch (this.m_heldCardVisual.GetVisualType())
				{
				case CollectionManagerDisplay.ViewMode.CARDS:
					CollectionDeckTray.Get().AddCard(this.m_heldCardVisual.GetEntityDef(), this.m_heldCardVisual.GetPremium(), deckTileToRemove, true, null);
					break;
				case CollectionManagerDisplay.ViewMode.HERO_SKINS:
					CollectionDeckTray.Get().GetHeroSkinContent().UpdateHeroSkin(this.m_heldCardVisual.GetEntityDef(), this.m_heldCardVisual.GetPremium(), true);
					break;
				case CollectionManagerDisplay.ViewMode.CARD_BACKS:
				{
					object obj = this.m_heldCardVisual.GetCardBackId();
					if (obj == null)
					{
						Debug.LogWarning("Cardback ID not set for dragging card back.");
					}
					else
					{
						CollectionDeckTray.Get().GetCardBackContent().UpdateCardBack((int)obj, true, null);
					}
					break;
				}
				}
			}
			else
			{
				SoundManager.Get().LoadAndPlay("collection_manager_drop_card", this.m_heldCardVisual.gameObject);
			}
		}
		this.m_heldCardVisual.Hide();
		this.m_scrollBar.Pause(false);
	}

	// Token: 0x06001F9E RID: 8094 RVA: 0x0009AA04 File Offset: 0x00098C04
	private void OnMouseOnOrOffScreen(bool onScreen)
	{
		if (onScreen)
		{
			return;
		}
		if (!this.m_heldCardVisual.IsShown())
		{
			return;
		}
		this.DropCard(true, null);
		CollectionDeckTray.Get().GetDeckBigCard().ForceHide();
	}

	// Token: 0x04001166 RID: 4454
	public CollectionDraggableCardVisual m_heldCardVisual;

	// Token: 0x04001167 RID: 4455
	public Collider TooltipPlane;

	// Token: 0x04001168 RID: 4456
	private static CollectionInputMgr s_instance;

	// Token: 0x04001169 RID: 4457
	private bool m_showingDeckTile;

	// Token: 0x0400116A RID: 4458
	private Vector3 m_heldCardScreenSpace;

	// Token: 0x0400116B RID: 4459
	private bool m_mouseIsOverDeck;

	// Token: 0x0400116C RID: 4460
	private UIBScrollable m_scrollBar;

	// Token: 0x0400116D RID: 4461
	private bool m_cardsDraggable;
}
