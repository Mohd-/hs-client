using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000808 RID: 2056
public class DraftDeckTray : MonoBehaviour
{
	// Token: 0x06004F8D RID: 20365 RVA: 0x00179EE0 File Offset: 0x001780E0
	private void Awake()
	{
		DraftDeckTray.s_instance = this;
		if (this.m_scrollbar != null)
		{
			this.m_scrollbar.Enable(false);
			this.m_scrollbar.AddTouchScrollStartedListener(new UIBScrollable.OnTouchScrollStarted(this.OnTouchScrollStarted));
		}
		this.m_cardsContent.SetInArena(true);
		this.m_cardsContent.RegisterCardTilePressListener(new DeckTrayCardListContent.CardTilePress(this.OnCardTilePress));
		this.m_cardsContent.RegisterCardTileOverListener(new DeckTrayCardListContent.CardTileOver(this.OnCardTileOver));
		this.m_cardsContent.RegisterCardTileOutListener(new DeckTrayCardListContent.CardTileOut(this.OnCardTileOut));
		this.m_cardsContent.RegisterCardTileReleaseListener(new DeckTrayCardListContent.CardTileRelease(this.OnCardTileRelease));
		this.m_cardsContent.RegisterCardCountUpdated(new DeckTrayCardListContent.CardCountChanged(this.OnCardCountUpdated));
		DraftManager.Get().RegisterDraftDeckSetListener(new DraftManager.DraftDeckSet(this.OnDraftDeckInitialized));
	}

	// Token: 0x06004F8E RID: 20366 RVA: 0x00179FBC File Offset: 0x001781BC
	private void OnDestroy()
	{
		DraftManager.Get().RemoveDraftDeckSetListener(new DraftManager.DraftDeckSet(this.OnDraftDeckInitialized));
		CollectionManager.Get().ClearTaggedDeck(CollectionManager.DeckTag.Arena);
		DraftDeckTray.s_instance = null;
	}

	// Token: 0x06004F8F RID: 20367 RVA: 0x00179FE5 File Offset: 0x001781E5
	public static DraftDeckTray Get()
	{
		return DraftDeckTray.s_instance;
	}

	// Token: 0x06004F90 RID: 20368 RVA: 0x00179FEC File Offset: 0x001781EC
	public void Initialize()
	{
		CollectionDeck draftDeck = DraftManager.Get().GetDraftDeck();
		if (draftDeck != null)
		{
			this.OnDraftDeckInitialized(draftDeck);
		}
	}

	// Token: 0x06004F91 RID: 20369 RVA: 0x0017A011 File Offset: 0x00178211
	public bool MouseIsOver()
	{
		return UniversalInputManager.Get().InputIsOver(base.gameObject);
	}

	// Token: 0x06004F92 RID: 20370 RVA: 0x0017A023 File Offset: 0x00178223
	public void AddCard(string cardID, Actor animateFromActor = null)
	{
		this.m_cardsContent.UpdateCardList(cardID, animateFromActor, null);
	}

	// Token: 0x06004F93 RID: 20371 RVA: 0x0017A038 File Offset: 0x00178238
	public DeckTrayCardListContent GetCardsContent()
	{
		return this.m_cardsContent;
	}

	// Token: 0x06004F94 RID: 20372 RVA: 0x0017A040 File Offset: 0x00178240
	public TooltipZone GetTooltipZone()
	{
		return this.m_deckHeaderTooltip;
	}

	// Token: 0x06004F95 RID: 20373 RVA: 0x0017A048 File Offset: 0x00178248
	private void OnCardCountUpdated(int cardCount)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		if (cardCount > 0)
		{
			if (this.m_headerLabel != null)
			{
				this.m_headerLabel.SetActive(true);
			}
			if (cardCount < CollectionManager.Get().GetDeckSize())
			{
				text = GameStrings.Get("GLUE_DECK_TRAY_CARD_COUNT_LABEL");
				text2 = GameStrings.Format("GLUE_DECK_TRAY_COUNT", new object[]
				{
					cardCount,
					CollectionManager.Get().GetDeckSize()
				});
			}
		}
		this.m_countLabelText.Text = text;
		if (UniversalInputManager.UsePhoneUI)
		{
			base.StartCoroutine(this.DelayCardCountUpdate(text2));
		}
		else
		{
			this.m_countText.Text = text2;
		}
	}

	// Token: 0x06004F96 RID: 20374 RVA: 0x0017A108 File Offset: 0x00178308
	private IEnumerator DelayCardCountUpdate(string count)
	{
		yield return new WaitForSeconds(0.5f);
		this.m_countText.Text = count;
		yield break;
	}

	// Token: 0x06004F97 RID: 20375 RVA: 0x0017A134 File Offset: 0x00178334
	private void ShowDeckBigCard(DeckTrayDeckTileVisual cardTile, float delay = 0f)
	{
		CollectionDeckTileActor actor = cardTile.GetActor();
		if (this.m_deckBigCard == null)
		{
			return;
		}
		EntityDef entityDef = actor.GetEntityDef();
		CardDef cardDef = DefLoader.Get().GetCardDef(entityDef.GetCardId(), null);
		this.m_deckBigCard.Show(entityDef, actor.GetPremium(), cardDef, actor.gameObject.transform.position, GhostCard.Type.NONE, delay);
		if (UniversalInputManager.Get().IsTouchMode())
		{
			cardTile.SetHighlight(true);
		}
	}

	// Token: 0x06004F98 RID: 20376 RVA: 0x0017A1B0 File Offset: 0x001783B0
	private void HideDeckBigCard(DeckTrayDeckTileVisual cardTile, bool force = false)
	{
		CollectionDeckTileActor actor = cardTile.GetActor();
		if (this.m_deckBigCard != null)
		{
			if (force)
			{
				this.m_deckBigCard.ForceHide();
			}
			else
			{
				this.m_deckBigCard.Hide(actor.GetEntityDef(), actor.GetPremium());
			}
			if (UniversalInputManager.Get().IsTouchMode())
			{
				cardTile.SetHighlight(false);
			}
		}
	}

	// Token: 0x06004F99 RID: 20377 RVA: 0x0017A218 File Offset: 0x00178418
	private void OnTouchScrollStarted()
	{
		if (this.m_deckBigCard != null)
		{
			this.m_deckBigCard.ForceHide();
		}
	}

	// Token: 0x06004F9A RID: 20378 RVA: 0x0017A238 File Offset: 0x00178438
	private void OnCardTilePress(DeckTrayDeckTileVisual cardTile)
	{
		if (UniversalInputManager.Get().IsTouchMode())
		{
			this.ShowDeckBigCard(cardTile, 0.2f);
		}
		else if (CollectionInputMgr.Get() != null)
		{
			this.HideDeckBigCard(cardTile, false);
		}
	}

	// Token: 0x06004F9B RID: 20379 RVA: 0x0017A27D File Offset: 0x0017847D
	private void OnCardTileOver(DeckTrayDeckTileVisual cardTile)
	{
		if (UniversalInputManager.Get().IsTouchMode())
		{
			return;
		}
		this.ShowDeckBigCard(cardTile, 0f);
	}

	// Token: 0x06004F9C RID: 20380 RVA: 0x0017A29B File Offset: 0x0017849B
	private void OnCardTileOut(DeckTrayDeckTileVisual cardTile)
	{
		this.HideDeckBigCard(cardTile, false);
	}

	// Token: 0x06004F9D RID: 20381 RVA: 0x0017A2A5 File Offset: 0x001784A5
	private void OnCardTileRelease(DeckTrayDeckTileVisual cardTile)
	{
		if (UniversalInputManager.Get().IsTouchMode())
		{
			this.HideDeckBigCard(cardTile, false);
		}
	}

	// Token: 0x06004F9E RID: 20382 RVA: 0x0017A2C0 File Offset: 0x001784C0
	private void OnDraftDeckInitialized(CollectionDeck draftDeck)
	{
		if (draftDeck == null)
		{
			Debug.LogError("Draft deck is null.");
			return;
		}
		CollectionManager.Get().SetTaggedDeck(CollectionManager.DeckTag.Arena, draftDeck, null);
		this.OnCardCountUpdated(draftDeck.GetTotalCardCount());
		this.m_cardsContent.UpdateCardList(true, null);
	}

	// Token: 0x0400366F RID: 13935
	public DeckTrayCardListContent m_cardsContent;

	// Token: 0x04003670 RID: 13936
	public UIBScrollable m_scrollbar;

	// Token: 0x04003671 RID: 13937
	public TooltipZone m_deckHeaderTooltip;

	// Token: 0x04003672 RID: 13938
	public DeckBigCard m_deckBigCard;

	// Token: 0x04003673 RID: 13939
	public UberText m_countLabelText;

	// Token: 0x04003674 RID: 13940
	public UberText m_countText;

	// Token: 0x04003675 RID: 13941
	public GameObject m_headerLabel;

	// Token: 0x04003676 RID: 13942
	private static DraftDeckTray s_instance;
}
