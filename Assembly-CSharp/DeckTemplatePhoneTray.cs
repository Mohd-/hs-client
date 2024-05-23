using System;
using System.Collections;
using UnityEngine;

// Token: 0x020006DD RID: 1757
public class DeckTemplatePhoneTray : MonoBehaviour
{
	// Token: 0x060048B0 RID: 18608 RVA: 0x0015BCA8 File Offset: 0x00159EA8
	private void Awake()
	{
		DeckTemplatePhoneTray.s_instance = this;
		if (this.m_scrollbar != null)
		{
			this.m_scrollbar.Enable(false);
			this.m_scrollbar.AddTouchScrollStartedListener(new UIBScrollable.OnTouchScrollStarted(this.OnTouchScrollStarted));
		}
		this.m_cardsContent.RegisterCardTilePressListener(new DeckTrayCardListContent.CardTilePress(this.OnCardTilePress));
		this.m_cardsContent.RegisterCardTileOverListener(new DeckTrayCardListContent.CardTileOver(this.OnCardTileOver));
		this.m_cardsContent.RegisterCardTileOutListener(new DeckTrayCardListContent.CardTileOut(this.OnCardTileOut));
		this.m_cardsContent.RegisterCardTileReleaseListener(new DeckTrayCardListContent.CardTileRelease(this.OnCardTileRelease));
		this.m_cardsContent.ShowFakeDeck(true);
	}

	// Token: 0x060048B1 RID: 18609 RVA: 0x0015BD57 File Offset: 0x00159F57
	private void OnDestroy()
	{
		DeckTemplatePhoneTray.s_instance = null;
	}

	// Token: 0x060048B2 RID: 18610 RVA: 0x0015BD5F File Offset: 0x00159F5F
	public static DeckTemplatePhoneTray Get()
	{
		return DeckTemplatePhoneTray.s_instance;
	}

	// Token: 0x060048B3 RID: 18611 RVA: 0x0015BD66 File Offset: 0x00159F66
	public bool MouseIsOver()
	{
		return UniversalInputManager.Get().InputIsOver(base.gameObject);
	}

	// Token: 0x060048B4 RID: 18612 RVA: 0x0015BD78 File Offset: 0x00159F78
	public DeckTrayCardListContent GetCardsContent()
	{
		return this.m_cardsContent;
	}

	// Token: 0x060048B5 RID: 18613 RVA: 0x0015BD80 File Offset: 0x00159F80
	public TooltipZone GetTooltipZone()
	{
		return this.m_deckHeaderTooltip;
	}

	// Token: 0x060048B6 RID: 18614 RVA: 0x0015BD88 File Offset: 0x00159F88
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

	// Token: 0x060048B7 RID: 18615 RVA: 0x0015BE48 File Offset: 0x0015A048
	private IEnumerator DelayCardCountUpdate(string count)
	{
		yield return new WaitForSeconds(0.5f);
		this.m_countText.Text = count;
		yield break;
	}

	// Token: 0x060048B8 RID: 18616 RVA: 0x0015BE74 File Offset: 0x0015A074
	private void ShowDeckBigCard(DeckTrayDeckTileVisual cardTile, float delay = 0f)
	{
		CollectionDeckTileActor actor = cardTile.GetActor();
		if (this.m_deckBigCard == null)
		{
			return;
		}
		EntityDef entityDef = actor.GetEntityDef();
		CardDef cardDef = DefLoader.Get().GetCardDef(entityDef.GetCardId(), new CardPortraitQuality(3, actor.GetPremium()));
		GhostCard.Type ghostTypeFromSlot = GhostCard.GetGhostTypeFromSlot(this.m_cardsContent.GetEditingDeck(), cardTile.GetSlot());
		this.m_deckBigCard.Show(entityDef, actor.GetPremium(), cardDef, actor.gameObject.transform.position, ghostTypeFromSlot, delay);
		if (UniversalInputManager.Get().IsTouchMode())
		{
			cardTile.SetHighlight(true);
		}
	}

	// Token: 0x060048B9 RID: 18617 RVA: 0x0015BF10 File Offset: 0x0015A110
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

	// Token: 0x060048BA RID: 18618 RVA: 0x0015BF78 File Offset: 0x0015A178
	private void OnTouchScrollStarted()
	{
		if (this.m_deckBigCard != null)
		{
			this.m_deckBigCard.ForceHide();
		}
	}

	// Token: 0x060048BB RID: 18619 RVA: 0x0015BF98 File Offset: 0x0015A198
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

	// Token: 0x060048BC RID: 18620 RVA: 0x0015BFDD File Offset: 0x0015A1DD
	private void OnCardTileOver(DeckTrayDeckTileVisual cardTile)
	{
		if (UniversalInputManager.Get().IsTouchMode())
		{
			return;
		}
		this.ShowDeckBigCard(cardTile, 0f);
	}

	// Token: 0x060048BD RID: 18621 RVA: 0x0015BFFB File Offset: 0x0015A1FB
	private void OnCardTileOut(DeckTrayDeckTileVisual cardTile)
	{
		this.HideDeckBigCard(cardTile, false);
	}

	// Token: 0x060048BE RID: 18622 RVA: 0x0015C005 File Offset: 0x0015A205
	private void OnCardTileRelease(DeckTrayDeckTileVisual cardTile)
	{
		if (UniversalInputManager.Get().IsTouchMode())
		{
			this.HideDeckBigCard(cardTile, false);
		}
	}

	// Token: 0x060048BF RID: 18623 RVA: 0x0015C01E File Offset: 0x0015A21E
	public void FlashDeckTemplateHighlight()
	{
		if (this.m_deckTemplateChosenGlow != null)
		{
			this.m_deckTemplateChosenGlow.SendEvent("Flash");
		}
	}

	// Token: 0x04002FD4 RID: 12244
	public DeckTrayCardListContent m_cardsContent;

	// Token: 0x04002FD5 RID: 12245
	public UIBScrollable m_scrollbar;

	// Token: 0x04002FD6 RID: 12246
	public TooltipZone m_deckHeaderTooltip;

	// Token: 0x04002FD7 RID: 12247
	public DeckBigCard m_deckBigCard;

	// Token: 0x04002FD8 RID: 12248
	public UberText m_countLabelText;

	// Token: 0x04002FD9 RID: 12249
	public UberText m_countText;

	// Token: 0x04002FDA RID: 12250
	public GameObject m_headerLabel;

	// Token: 0x04002FDB RID: 12251
	public PlayMakerFSM m_deckTemplateChosenGlow;

	// Token: 0x04002FDC RID: 12252
	private static DeckTemplatePhoneTray s_instance;
}
