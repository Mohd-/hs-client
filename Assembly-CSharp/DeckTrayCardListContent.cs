using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005C5 RID: 1477
[CustomEditClass]
public class DeckTrayCardListContent : DeckTrayContent
{
	// Token: 0x060041FF RID: 16895 RVA: 0x0013E858 File Offset: 0x0013CA58
	private void Awake()
	{
		this.m_deckHelpButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDeckHelpButtonPress));
		this.m_deckHelpButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnDeckHelpButtonOver));
		this.m_deckHelpButton.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnDeckHelpButtonOut));
		if (this.m_deckTemplateHelpButton != null)
		{
			this.m_deckTemplateHelpButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDeckTemplateHelpButtonPress));
			this.m_deckTemplateHelpButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnDeckTemplateHelpButtonOver));
			this.m_deckTemplateHelpButton.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnDeckTemplateHelpButtonOut));
		}
		this.m_originalLocalPosition = base.transform.localPosition;
		this.m_hasFinishedEntering = false;
	}

	// Token: 0x06004200 RID: 16896 RVA: 0x0013E924 File Offset: 0x0013CB24
	public override bool AnimateContentEntranceStart()
	{
		if (this.m_loading)
		{
			return false;
		}
		this.m_animating = true;
		this.m_hasFinishedEntering = false;
		Action<object> action = delegate(object _1)
		{
			this.UpdateDeckCompleteHighlight();
			this.ShowDeckEditingTipsIfNeeded();
			this.m_animating = false;
		};
		CollectionDeck editingDeck = this.GetEditingDeck();
		if (editingDeck != null)
		{
			base.transform.localPosition = this.GetOffscreenLocalPosition();
			iTween.StopByName(base.gameObject, "position");
			iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
			{
				"position",
				this.m_originalLocalPosition,
				"isLocal",
				true,
				"time",
				0.3f,
				"easeType",
				iTween.EaseType.easeOutQuad,
				"oncomplete",
				action,
				"name",
				"position"
			}));
			if (editingDeck.GetTotalCardCount() > 0)
			{
				SoundManager.Get().LoadAndPlay("collection_manager_new_deck_moves_up_tray", base.gameObject);
			}
			this.UpdateCardList(false, null);
		}
		else
		{
			action.Invoke(null);
		}
		return true;
	}

	// Token: 0x06004201 RID: 16897 RVA: 0x0013EA40 File Offset: 0x0013CC40
	public override bool AnimateContentEntranceEnd()
	{
		if (this.m_animating)
		{
			return false;
		}
		this.m_hasFinishedEntering = true;
		this.FireCardCountChangedEvent();
		return true;
	}

	// Token: 0x06004202 RID: 16898 RVA: 0x0013EA60 File Offset: 0x0013CC60
	public override bool AnimateContentExitStart()
	{
		if (this.m_animating)
		{
			return false;
		}
		this.m_animating = true;
		this.m_hasFinishedExiting = false;
		if (this.m_deckCompleteHighlight != null)
		{
			this.m_deckCompleteHighlight.SetActive(false);
		}
		iTween.StopByName(base.gameObject, "position");
		iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
		{
			"position",
			this.GetOffscreenLocalPosition(),
			"isLocal",
			true,
			"time",
			0.3f,
			"easeType",
			iTween.EaseType.easeInQuad,
			"name",
			"position"
		}));
		if (HeroPickerDisplay.Get() == null || !HeroPickerDisplay.Get().IsShown())
		{
			SoundManager.Get().LoadAndPlay("panel_slide_off_deck_creation_screen", base.gameObject);
		}
		ApplicationMgr.Get().ScheduleCallback(0.5f, false, delegate(object o)
		{
			this.m_animating = false;
		}, null);
		return true;
	}

	// Token: 0x06004203 RID: 16899 RVA: 0x0013EB7D File Offset: 0x0013CD7D
	public override bool AnimateContentExitEnd()
	{
		this.m_hasFinishedExiting = true;
		return !this.m_animating;
	}

	// Token: 0x06004204 RID: 16900 RVA: 0x0013EB8F File Offset: 0x0013CD8F
	public bool HasFinishedEntering()
	{
		return this.m_hasFinishedEntering;
	}

	// Token: 0x06004205 RID: 16901 RVA: 0x0013EB97 File Offset: 0x0013CD97
	public bool HasFinishedExiting()
	{
		return this.m_hasFinishedExiting;
	}

	// Token: 0x06004206 RID: 16902 RVA: 0x0013EBA0 File Offset: 0x0013CDA0
	public override void OnTaggedDeckChanged(CollectionManager.DeckTag tag, CollectionDeck newDeck, CollectionDeck oldDeck, bool isNewDeck)
	{
		if (tag != CollectionManager.DeckTag.Editing || newDeck == null)
		{
			return;
		}
		List<CollectionDeckSlot> slots = newDeck.GetSlots();
		this.LoadCardPrefabs(slots);
		if (base.IsModeActive())
		{
			this.ShowDeckHelpButtonIfNeeded();
		}
	}

	// Token: 0x06004207 RID: 16903 RVA: 0x0013EBDC File Offset: 0x0013CDDC
	public void ShowDeckHelper(DeckTrayDeckTileVisual tileToRemove, bool continueAfterReplace, bool replacingCard = false)
	{
		if (!CollectionManager.Get().IsInEditMode())
		{
			return;
		}
		if (!DeckHelper.Get())
		{
			return;
		}
		DeckHelper.Get().Show(tileToRemove, continueAfterReplace, replacingCard);
	}

	// Token: 0x06004208 RID: 16904 RVA: 0x0013EC16 File Offset: 0x0013CE16
	public Vector3 GetCardVisualExtents()
	{
		return new Vector3(this.m_cardTileHeight, this.m_cardTileHeight, this.m_cardTileHeight);
	}

	// Token: 0x06004209 RID: 16905 RVA: 0x0013EC2F File Offset: 0x0013CE2F
	public List<DeckTrayDeckTileVisual> GetCardTiles()
	{
		return this.m_cardTiles;
	}

	// Token: 0x0600420A RID: 16906 RVA: 0x0013EC38 File Offset: 0x0013CE38
	public DeckTrayDeckTileVisual GetCardTileVisual(string cardID)
	{
		foreach (DeckTrayDeckTileVisual deckTrayDeckTileVisual in this.m_cardTiles)
		{
			if (!(deckTrayDeckTileVisual == null) && !(deckTrayDeckTileVisual.GetActor() == null) && deckTrayDeckTileVisual.GetActor().GetEntityDef() != null)
			{
				if (deckTrayDeckTileVisual.GetActor().GetEntityDef().GetCardId() == cardID)
				{
					return deckTrayDeckTileVisual;
				}
			}
		}
		return null;
	}

	// Token: 0x0600420B RID: 16907 RVA: 0x0013ECE4 File Offset: 0x0013CEE4
	public DeckTrayDeckTileVisual GetCardTileVisual(string cardID, TAG_PREMIUM premType)
	{
		foreach (DeckTrayDeckTileVisual deckTrayDeckTileVisual in this.m_cardTiles)
		{
			if (!(deckTrayDeckTileVisual == null) && !(deckTrayDeckTileVisual.GetActor() == null) && deckTrayDeckTileVisual.GetActor().GetEntityDef() != null)
			{
				if (deckTrayDeckTileVisual.GetActor().GetEntityDef().GetCardId() == cardID && deckTrayDeckTileVisual.GetActor().GetPremium() == premType)
				{
					return deckTrayDeckTileVisual;
				}
			}
		}
		return null;
	}

	// Token: 0x0600420C RID: 16908 RVA: 0x0013EDA0 File Offset: 0x0013CFA0
	public DeckTrayDeckTileVisual GetCardTileVisual(int index)
	{
		if (index < this.m_cardTiles.Count)
		{
			return this.m_cardTiles[index];
		}
		return null;
	}

	// Token: 0x0600420D RID: 16909 RVA: 0x0013EDC4 File Offset: 0x0013CFC4
	public DeckTrayDeckTileVisual GetCardTileVisualOrLastVisible(string cardID)
	{
		int num = 0;
		foreach (DeckTrayDeckTileVisual deckTrayDeckTileVisual in this.m_cardTiles)
		{
			num++;
			if (!(deckTrayDeckTileVisual == null) && !(deckTrayDeckTileVisual.GetActor() == null) && deckTrayDeckTileVisual.GetActor().GetEntityDef() != null)
			{
				if (num > 20)
				{
					return deckTrayDeckTileVisual;
				}
				if (deckTrayDeckTileVisual.GetActor().GetEntityDef().GetCardId() == cardID)
				{
					return deckTrayDeckTileVisual;
				}
			}
		}
		return null;
	}

	// Token: 0x0600420E RID: 16910 RVA: 0x0013EE84 File Offset: 0x0013D084
	public DeckTrayDeckTileVisual GetOrAddCardTileVisual(int index)
	{
		DeckTrayDeckTileVisual newTileVisual = this.GetCardTileVisual(index);
		if (newTileVisual != null)
		{
			return newTileVisual;
		}
		GameObject gameObject = new GameObject("DeckTileVisual" + index);
		GameUtils.SetParent(gameObject, this, false);
		gameObject.transform.localScale = this.m_cardTileSlotLocalScaleVec3;
		newTileVisual = gameObject.AddComponent<DeckTrayDeckTileVisual>();
		newTileVisual.AddEventListener(UIEventType.HOLD, delegate(UIEvent e)
		{
			this.FireCardTileHeldEvent(newTileVisual);
		});
		newTileVisual.AddEventListener(UIEventType.PRESS, delegate(UIEvent e)
		{
			this.FireCardTilePressEvent(newTileVisual);
		});
		newTileVisual.AddEventListener(UIEventType.TAP, delegate(UIEvent e)
		{
			this.FireCardTileTapEvent(newTileVisual);
		});
		newTileVisual.AddEventListener(UIEventType.ROLLOVER, delegate(UIEvent e)
		{
			this.FireCardTileOverEvent(newTileVisual);
		});
		newTileVisual.AddEventListener(UIEventType.ROLLOUT, delegate(UIEvent e)
		{
			this.FireCardTileOutEvent(newTileVisual);
		});
		newTileVisual.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.FireCardTileReleaseEvent(newTileVisual);
		});
		newTileVisual.AddEventListener(UIEventType.RIGHTCLICK, delegate(UIEvent e)
		{
			this.FireCardTileRightClickedEvent(newTileVisual);
		});
		this.m_cardTiles.Insert(index, newTileVisual);
		Vector3 extents;
		extents..ctor(this.m_cardTileHeight, this.m_cardTileHeight, this.m_cardTileHeight);
		if (this.m_scrollbar != null)
		{
			this.m_scrollbar.AddVisibleAffectedObject(gameObject, extents, true, new UIBScrollable.VisibleAffected(this.IsCardTileVisible));
		}
		return newTileVisual;
	}

	// Token: 0x0600420F RID: 16911 RVA: 0x0013F00C File Offset: 0x0013D20C
	public void UpdateTileVisuals()
	{
		foreach (DeckTrayDeckTileVisual deckTrayDeckTileVisual in this.m_cardTiles)
		{
			deckTrayDeckTileVisual.UpdateGhostedState();
		}
	}

	// Token: 0x06004210 RID: 16912 RVA: 0x0013F068 File Offset: 0x0013D268
	public override void Show(bool showAll = false)
	{
		foreach (DeckTrayDeckTileVisual deckTrayDeckTileVisual in this.m_cardTiles)
		{
			if (showAll || deckTrayDeckTileVisual.IsInUse())
			{
				deckTrayDeckTileVisual.Show();
			}
		}
	}

	// Token: 0x06004211 RID: 16913 RVA: 0x0013F0D4 File Offset: 0x0013D2D4
	public override void Hide(bool hideAll = false)
	{
		foreach (DeckTrayDeckTileVisual deckTrayDeckTileVisual in this.m_cardTiles)
		{
			if (hideAll || !deckTrayDeckTileVisual.IsInUse())
			{
				deckTrayDeckTileVisual.Hide();
			}
		}
	}

	// Token: 0x06004212 RID: 16914 RVA: 0x0013F140 File Offset: 0x0013D340
	public void CommitFakeDeckChanges()
	{
		CollectionDeck taggedDeck = CollectionManager.Get().GetTaggedDeck(this.m_deckType);
		taggedDeck.CopyContents(this.m_templateFakeDeck);
		taggedDeck.Name = this.m_templateFakeDeck.Name;
	}

	// Token: 0x06004213 RID: 16915 RVA: 0x0013F17C File Offset: 0x0013D37C
	public CollectionDeck GetEditingDeck()
	{
		return (!this.m_isShowingFakeDeck) ? CollectionManager.Get().GetTaggedDeck(this.m_deckType) : this.m_templateFakeDeck;
	}

	// Token: 0x06004214 RID: 16916 RVA: 0x0013F1AF File Offset: 0x0013D3AF
	public void ShowFakeDeck(bool show)
	{
		if (this.m_isShowingFakeDeck == show)
		{
			return;
		}
		this.m_isShowingFakeDeck = show;
		this.UpdateCardList(true, null);
	}

	// Token: 0x06004215 RID: 16917 RVA: 0x0013F1D0 File Offset: 0x0013D3D0
	public void ResetFakeDeck()
	{
		if (this.m_templateFakeDeck == null)
		{
			return;
		}
		CollectionDeck taggedDeck = CollectionManager.Get().GetTaggedDeck(this.m_deckType);
		if (taggedDeck == null)
		{
			return;
		}
		this.m_templateFakeDeck.CopyContents(taggedDeck);
		this.m_templateFakeDeck.Name = taggedDeck.Name;
	}

	// Token: 0x06004216 RID: 16918 RVA: 0x0013F21E File Offset: 0x0013D41E
	public void ShowDeckCompleteEffects()
	{
		base.StartCoroutine(this.ShowDeckCompleteEffectsWithInterval(this.m_deckCardBarFlareUpInterval));
	}

	// Token: 0x06004217 RID: 16919 RVA: 0x0013F233 File Offset: 0x0013D433
	public void SetInArena(bool inArena)
	{
		this.m_inArena = inArena;
	}

	// Token: 0x06004218 RID: 16920 RVA: 0x0013F23C File Offset: 0x0013D43C
	public bool AddCard(EntityDef cardEntityDef, TAG_PREMIUM premium, DeckTrayDeckTileVisual deckTileToRemove, bool playSound, Actor animateFromActor = null)
	{
		if (!base.IsModeActive())
		{
			return false;
		}
		if (cardEntityDef == null)
		{
			Debug.LogError("Trying to add card EntityDef that is null.");
			return false;
		}
		string cardId = cardEntityDef.GetCardId();
		CollectionDeck editingDeck = this.GetEditingDeck();
		if (editingDeck == null)
		{
			return false;
		}
		if (!editingDeck.CanAddOwnedCard(cardId, premium))
		{
			return false;
		}
		if (playSound)
		{
			SoundManager.Get().LoadAndPlay("collection_manager_place_card_in_deck", base.gameObject);
		}
		bool flag = editingDeck.GetTotalCardCount() == CollectionManager.Get().GetDeckSize();
		DeckTrayDeckTileVisual firstInvalidCard = this.GetFirstInvalidCard();
		if (flag && (deckTileToRemove == null || editingDeck.IsValidSlot(deckTileToRemove.GetSlot())) && firstInvalidCard != null)
		{
			deckTileToRemove = firstInvalidCard;
		}
		if (flag || (deckTileToRemove != null && !editingDeck.IsValidSlot(deckTileToRemove.GetSlot())))
		{
			if (deckTileToRemove == null)
			{
				Debug.LogWarning(string.Format("CollectionDeckTray.AddCard(): Cannot add card {0} (premium {1}) without removing one first.", cardEntityDef.GetCardId(), premium));
				return false;
			}
			deckTileToRemove.SetHighlight(false);
			string cardID = deckTileToRemove.GetCardID();
			TAG_PREMIUM premium2 = deckTileToRemove.GetPremium();
			if (!editingDeck.RemoveCard(cardID, premium2, editingDeck.IsValidSlot(deckTileToRemove.GetSlot()), false))
			{
				Debug.LogWarning(string.Format("CollectionDeckTray.AddCard({0},{1}): Tried to remove card {2} with premium {3}, but it failed!", new object[]
				{
					cardId,
					premium,
					cardID,
					premium2
				}));
				return false;
			}
		}
		if (!editingDeck.AddCard(cardEntityDef, premium, false))
		{
			Debug.LogWarning(string.Format("CollectionDeckTray.AddCard({0},{1}): deck.AddCard failed!", cardId, premium));
			return false;
		}
		if (editingDeck.GetTotalValidCardCount() == CollectionManager.Get().GetDeckSize())
		{
			DeckHelper.Get().Hide(true);
		}
		this.UpdateCardList(cardEntityDef, true, animateFromActor);
		CollectionManagerDisplay.Get().UpdateCurrentPageCardLocks(true);
		if (!Options.Get().GetBool(Option.HAS_ADDED_CARDS_TO_DECK, false) && editingDeck.GetTotalCardCount() >= 2 && !DeckHelper.Get().IsActive() && editingDeck.GetTotalCardCount() < 15 && UserAttentionManager.CanShowAttentionGrabber("DeckTrayCardListContent.AddCard:" + Option.HAS_ADDED_CARDS_TO_DECK))
		{
			NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_CM_PAGEFLIP_28"), "VO_INNKEEPER_CM_PAGEFLIP_28", 0f, null);
			Options.Get().SetBool(Option.HAS_ADDED_CARDS_TO_DECK, true);
		}
		return true;
	}

	// Token: 0x06004219 RID: 16921 RVA: 0x0013F488 File Offset: 0x0013D688
	public int RemoveClosestInvalidCard(EntityDef entityDef, int sameRemoveCount)
	{
		CollectionDeck editingDeck = this.GetEditingDeck();
		int cost = entityDef.GetCost();
		int num = int.MaxValue;
		string text = string.Empty;
		TAG_PREMIUM premium = TAG_PREMIUM.NORMAL;
		foreach (CollectionDeckSlot collectionDeckSlot in editingDeck.GetSlots())
		{
			if (!editingDeck.IsValidSlot(collectionDeckSlot))
			{
				EntityDef entityDef2 = DefLoader.Get().GetEntityDef(collectionDeckSlot.CardID);
				if (entityDef2 == entityDef)
				{
					text = entityDef.GetCardId();
					premium = collectionDeckSlot.Premium;
					break;
				}
				int num2 = Mathf.Abs(cost - entityDef2.GetCost());
				if (num2 < num)
				{
					num = num2;
					text = collectionDeckSlot.CardID;
				}
			}
		}
		int num3 = 0;
		if (!string.IsNullOrEmpty(text))
		{
			for (int i = 0; i < sameRemoveCount; i++)
			{
				if (editingDeck.RemoveCard(text, premium, false, false))
				{
					num3++;
				}
			}
		}
		this.UpdateCardList(true, null);
		return num3;
	}

	// Token: 0x0600421A RID: 16922 RVA: 0x0013F5A4 File Offset: 0x0013D7A4
	[ContextMenu("Update Card List")]
	public void UpdateCardList(bool updateHighlight = true, Actor animateFromActor = null)
	{
		this.UpdateCardList(string.Empty, updateHighlight, animateFromActor);
	}

	// Token: 0x0600421B RID: 16923 RVA: 0x0013F5B3 File Offset: 0x0013D7B3
	public void UpdateCardList(EntityDef justChangedCardEntityDef, bool updateHighlight = true, Actor animateFromActor = null)
	{
		this.UpdateCardList((justChangedCardEntityDef == null) ? string.Empty : justChangedCardEntityDef.GetCardId(), updateHighlight, animateFromActor);
	}

	// Token: 0x0600421C RID: 16924 RVA: 0x0013F5D4 File Offset: 0x0013D7D4
	public void UpdateCardList(string justChangedCardID, bool updateHighlight = true, Actor animateFromActor = null)
	{
		CollectionDeck editingDeck = this.GetEditingDeck();
		if (editingDeck == null)
		{
			Debug.LogError("No current deck set for DeckTrayCardListContent.");
			return;
		}
		foreach (DeckTrayDeckTileVisual deckTrayDeckTileVisual in this.m_cardTiles)
		{
			deckTrayDeckTileVisual.MarkAsUnused();
		}
		List<CollectionDeckSlot> slots = editingDeck.GetSlots();
		int num = 0;
		Vector3 cardTileOffset = this.GetCardTileOffset(editingDeck);
		for (int i = 0; i < slots.Count; i++)
		{
			CollectionDeckSlot collectionDeckSlot = slots[i];
			if (collectionDeckSlot.Count == 0)
			{
				Log.Rachelle.Print(string.Format("CollectionDeckTray.UpdateCardList(): Slot {0} of deck is empty! Skipping...", i), new object[0]);
			}
			else
			{
				num += collectionDeckSlot.Count;
				DeckTrayDeckTileVisual orAddCardTileVisual = this.GetOrAddCardTileVisual(i);
				orAddCardTileVisual.SetInArena(this.m_inArena);
				orAddCardTileVisual.gameObject.transform.localPosition = cardTileOffset + Vector3.down * (this.m_cardTileSlotLocalHeight * (float)i);
				orAddCardTileVisual.MarkAsUsed();
				orAddCardTileVisual.Show();
				orAddCardTileVisual.SetSlot(collectionDeckSlot, justChangedCardID.Equals(collectionDeckSlot.CardID));
			}
		}
		this.Hide(false);
		this.ShowDeckHelpButtonIfNeeded();
		this.FireCardCountChangedEvent();
		this.m_scrollbar.UpdateScroll();
		if (updateHighlight)
		{
			this.UpdateDeckCompleteHighlight();
		}
		if (animateFromActor != null)
		{
			base.StartCoroutine(this.ShowAddCardAnimationAfterTrayLoads(animateFromActor));
		}
	}

	// Token: 0x0600421D RID: 16925 RVA: 0x0013F768 File Offset: 0x0013D968
	public Vector3 GetCardTileOffset(CollectionDeck currentDeck)
	{
		Vector3 vector = Vector3.zero;
		if (!this.m_isShowingFakeDeck && currentDeck != null && this.m_deckTemplateHelpButton != null && currentDeck.GetTotalInvalidCardCount() > 0)
		{
			vector = Vector3.down * this.m_deckTemplateHelpButton.GetComponent<UIBScrollableItem>().m_size.y * this.m_cardTileSlotLocalScaleVec3.y;
		}
		return vector + this.m_cardTileOffset;
	}

	// Token: 0x0600421E RID: 16926 RVA: 0x0013F7E5 File Offset: 0x0013D9E5
	public void TriggerCardCountUpdate()
	{
		this.FireCardCountChangedEvent();
	}

	// Token: 0x0600421F RID: 16927 RVA: 0x0013F7F0 File Offset: 0x0013D9F0
	public void HideDeckHelpPopup()
	{
		if (this.m_deckHelpPopup != null)
		{
			NotificationManager.Get().DestroyNotification(this.m_deckHelpPopup, 0f);
		}
	}

	// Token: 0x06004220 RID: 16928 RVA: 0x0013F824 File Offset: 0x0013DA24
	public DeckTrayDeckTileVisual GetFirstInvalidCard()
	{
		CollectionDeck editingDeck = this.GetEditingDeck();
		if (editingDeck != null)
		{
			foreach (CollectionDeckSlot collectionDeckSlot in editingDeck.GetSlots())
			{
				if (!editingDeck.IsValidSlot(collectionDeckSlot))
				{
					return this.GetCardTileVisual(collectionDeckSlot.Index);
				}
			}
		}
		return null;
	}

	// Token: 0x06004221 RID: 16929 RVA: 0x0013F8A8 File Offset: 0x0013DAA8
	private IEnumerator ShowAddCardAnimationAfterTrayLoads(Actor cardToAnimate)
	{
		string cardID = cardToAnimate.GetEntityDef().GetCardId();
		TAG_PREMIUM premium = cardToAnimate.GetPremium();
		DeckTrayDeckTileVisual tile = this.GetCardTileVisual(cardID, premium);
		Vector3 cardPos = cardToAnimate.transform.position;
		while (tile == null)
		{
			yield return null;
			tile = this.GetCardTileVisual(cardID, premium);
		}
		GameObject cardTileObject = Object.Instantiate<GameObject>(tile.GetActor().gameObject);
		Actor movingCardTile = cardTileObject.GetComponent<Actor>();
		CollectionDeck currentDeck = this.GetEditingDeck();
		if (currentDeck.GetCardCount(cardID, premium) == 1)
		{
			tile.Hide();
		}
		else
		{
			tile.Show();
		}
		movingCardTile.transform.position = new Vector3(cardPos.x, cardPos.y + 2.5f, cardPos.z);
		if (UniversalInputManager.UsePhoneUI)
		{
			movingCardTile.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
		}
		else
		{
			movingCardTile.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
		}
		movingCardTile.DeactivateAllSpells();
		movingCardTile.ActivateSpell(SpellType.SUMMON_IN_LARGE);
		if (UniversalInputManager.UsePhoneUI && this.m_phoneDeckTileBone != null)
		{
			iTween.MoveTo(cardTileObject, iTween.Hash(new object[]
			{
				"position",
				this.m_phoneDeckTileBone.transform.position,
				"time",
				0.5f,
				"easetype",
				iTween.EaseType.easeInCubic,
				"oncomplete",
				delegate(object v)
				{
					tile.Show();
					tile.GetActor().GetSpell(SpellType.SUMMON_IN).ActivateState(SpellStateType.BIRTH);
					base.StartCoroutine(this.FinishPhoneMovingCardTile(cardTileObject, movingCardTile, 1f));
				}
			}));
			iTween.ScaleTo(cardTileObject, iTween.Hash(new object[]
			{
				"scale",
				new Vector3(0.5f, 1.1f, 1.1f),
				"time",
				0.5f,
				"easetype",
				iTween.EaseType.easeInCubic
			}));
		}
		else
		{
			Vector3[] newPath = new Vector3[3];
			Vector3 startSpot = movingCardTile.transform.position;
			newPath[0] = startSpot;
			iTween.ValueTo(cardTileObject, iTween.Hash(new object[]
			{
				"from",
				0f,
				"to",
				1f,
				"time",
				0.75f,
				"easetype",
				iTween.EaseType.easeOutCirc,
				"onupdate",
				delegate(object val)
				{
					Vector3 position = tile.transform.position;
					newPath[1] = new Vector3((startSpot.x + position.x) * 0.5f, (startSpot.y + position.y) * 0.5f + 60f, (startSpot.z + position.z) * 0.5f);
					newPath[2] = position;
					iTween.PutOnPath(cardTileObject, newPath, (float)val);
				},
				"oncomplete",
				delegate(object v)
				{
					tile.Show();
					tile.GetActor().GetSpell(SpellType.SUMMON_IN).ActivateState(SpellStateType.BIRTH);
					movingCardTile.Hide();
					Object.Destroy(cardTileObject);
				}
			}));
		}
		SoundManager.Get().LoadAndPlay("collection_manager_card_add_to_deck_instant", base.gameObject);
		yield break;
	}

	// Token: 0x06004222 RID: 16930 RVA: 0x0013F8D4 File Offset: 0x0013DAD4
	private IEnumerator FinishPhoneMovingCardTile(GameObject obj, Actor movingCardTile, float delay)
	{
		yield return new WaitForSeconds(delay);
		movingCardTile.Hide();
		Object.Destroy(obj);
		yield break;
	}

	// Token: 0x06004223 RID: 16931 RVA: 0x0013F914 File Offset: 0x0013DB14
	private IEnumerator ShowDeckCompleteEffectsWithInterval(float interval)
	{
		if (this.m_scrollbar == null)
		{
			yield break;
		}
		bool needScroll = this.m_scrollbar.IsScrollNeeded();
		if (needScroll)
		{
			this.m_scrollbar.Enable(false);
			this.m_scrollbar.ForceVisibleAffectedObjectsShow(true);
			this.m_scrollbar.SetScroll(0f, iTween.EaseType.easeOutSine, 0.25f, true, true);
			yield return new WaitForSeconds(0.3f);
			this.m_scrollbar.SetScroll(1f, iTween.EaseType.easeInOutQuart, interval * (float)this.m_cardTiles.Count, true, true);
		}
		foreach (DeckTrayDeckTileVisual tile in this.m_cardTiles)
		{
			if (!(tile == null) && tile.IsInUse())
			{
				tile.GetActor().ActivateSpell(SpellType.SUMMON_IN_FORGE);
				yield return new WaitForSeconds(interval);
			}
		}
		if (needScroll)
		{
			this.m_scrollbar.ForceVisibleAffectedObjectsShow(false);
			this.m_scrollbar.EnableIfNeeded();
		}
		yield break;
	}

	// Token: 0x06004224 RID: 16932 RVA: 0x0013F940 File Offset: 0x0013DB40
	private void IsCardTileVisible(GameObject obj, bool visible)
	{
		if (obj.activeSelf != visible)
		{
			obj.SetActive(visible && obj.GetComponent<DeckTrayDeckTileVisual>().IsInUse());
		}
	}

	// Token: 0x06004225 RID: 16933 RVA: 0x0013F974 File Offset: 0x0013DB74
	private void UpdateDeckCompleteHighlight()
	{
		CollectionDeck editingDeck = this.GetEditingDeck();
		bool flag = editingDeck != null && editingDeck.GetTotalValidCardCount() == CollectionManager.Get().GetDeckSize();
		if (this.m_deckCompleteHighlight != null)
		{
			this.m_deckCompleteHighlight.SetActive(flag);
		}
		if (flag && !Options.Get().GetBool(Option.HAS_FINISHED_A_DECK, false))
		{
			Options.Get().SetBool(Option.HAS_FINISHED_A_DECK, true);
		}
	}

	// Token: 0x06004226 RID: 16934 RVA: 0x0013F9EC File Offset: 0x0013DBEC
	private void ShowDeckEditingTipsIfNeeded()
	{
		if (Options.Get().GetBool(Option.HAS_REMOVED_CARD_FROM_DECK, false))
		{
			return;
		}
		if (CollectionManagerDisplay.Get().GetViewMode() != CollectionManagerDisplay.ViewMode.CARDS)
		{
			return;
		}
		if (this.m_cardTiles.Count <= 0)
		{
			return;
		}
		if (SceneMgr.Get().GetMode() == SceneMgr.Mode.TAVERN_BRAWL)
		{
			return;
		}
		Transform removeCardTutorialBone = CollectionDeckTray.Get().m_removeCardTutorialBone;
		if (this.m_deckHelpPopup == null)
		{
			this.m_deckHelpPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, removeCardTutorialBone.position, removeCardTutorialBone.localScale, GameStrings.Get("GLUE_COLLECTION_TUTORIAL08"), true);
			if (this.m_deckHelpPopup != null)
			{
				this.m_deckHelpPopup.PulseReminderEveryXSeconds(3f);
			}
		}
	}

	// Token: 0x06004227 RID: 16935 RVA: 0x0013FAA8 File Offset: 0x0013DCA8
	public void ShowDeckHelpButtonIfNeeded()
	{
		bool active = false;
		if (CollectionManagerDisplay.Get() == null)
		{
			return;
		}
		CollectionDeck editingDeck = this.GetEditingDeck();
		if (TavernBrawlDisplay.IsTavernBrawlViewing() || TavernBrawlDisplay.IsTavernBrawlEditing())
		{
			active = false;
		}
		else if (editingDeck != null && DeckHelper.Get() != null && editingDeck.GetTotalValidCardCount() < CollectionManager.Get().GetDeckSize())
		{
			active = true;
			Vector3 cardTileOffset = this.GetCardTileOffset(editingDeck);
			cardTileOffset.y -= this.m_cardTileSlotLocalHeight * (float)editingDeck.GetSlots().Count;
			this.m_deckHelpButton.transform.localPosition = cardTileOffset;
		}
		bool active2;
		if (editingDeck.GetTotalInvalidCardCount() > 0)
		{
			active = false;
			active2 = true;
		}
		else
		{
			active2 = false;
		}
		if (CollectionManagerDisplay.Get().GetViewMode() == CollectionManagerDisplay.ViewMode.DECK_TEMPLATE)
		{
			active = false;
			active2 = false;
		}
		this.m_deckHelpButton.gameObject.SetActive(active);
		if (this.m_deckTemplateHelpButton != null)
		{
			this.m_deckTemplateHelpButton.gameObject.SetActive(active2);
		}
		if (!Options.Get().GetBool(Option.HAS_FINISHED_A_DECK, false))
		{
			HighlightState componentInChildren = this.m_deckHelpButton.GetComponentInChildren<HighlightState>();
			if (componentInChildren != null)
			{
				componentInChildren.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
			}
			if (this.m_deckTemplateHelpButton != null)
			{
				componentInChildren = this.m_deckTemplateHelpButton.GetComponentInChildren<HighlightState>();
				if (componentInChildren != null)
				{
					componentInChildren.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
				}
			}
		}
	}

	// Token: 0x06004228 RID: 16936 RVA: 0x0013FC1C File Offset: 0x0013DE1C
	private void OnDeckHelpButtonPress(UIEvent e)
	{
		DeckTrayDeckTileVisual firstInvalidCard = this.GetFirstInvalidCard();
		this.ShowDeckHelper(firstInvalidCard, true, firstInvalidCard != null);
	}

	// Token: 0x06004229 RID: 16937 RVA: 0x0013FC3F File Offset: 0x0013DE3F
	private void OnDeckTemplateHelpButtonPress(UIEvent e)
	{
		Options.Get().SetBool(Option.HAS_CLICKED_DECK_TEMPLATE_REPLACE, true);
		this.OnDeckHelpButtonPress(e);
	}

	// Token: 0x0600422A RID: 16938 RVA: 0x0013FC58 File Offset: 0x0013DE58
	private void OnDeckHelpButtonOver(UIEvent e)
	{
		HighlightState componentInChildren = this.m_deckHelpButton.GetComponentInChildren<HighlightState>();
		if (componentInChildren != null)
		{
			if (!Options.Get().GetBool(Option.HAS_FINISHED_A_DECK, false))
			{
				componentInChildren.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
			}
			else
			{
				componentInChildren.ChangeState(ActorStateType.HIGHLIGHT_MOUSE_OVER);
			}
		}
		SoundManager.Get().LoadAndPlay("Small_Mouseover", base.gameObject);
	}

	// Token: 0x0600422B RID: 16939 RVA: 0x0013FCBC File Offset: 0x0013DEBC
	private void OnDeckHelpButtonOut(UIEvent e)
	{
		HighlightState componentInChildren = this.m_deckHelpButton.GetComponentInChildren<HighlightState>();
		if (componentInChildren != null)
		{
			if (!Options.Get().GetBool(Option.HAS_FINISHED_A_DECK, false))
			{
				componentInChildren.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
			}
			else
			{
				componentInChildren.ChangeState(ActorStateType.NONE);
			}
		}
	}

	// Token: 0x0600422C RID: 16940 RVA: 0x0013FD0C File Offset: 0x0013DF0C
	private void OnDeckTemplateHelpButtonOver(UIEvent e)
	{
		HighlightState componentInChildren = this.m_deckTemplateHelpButton.GetComponentInChildren<HighlightState>();
		if (componentInChildren != null)
		{
			if (!Options.Get().GetBool(Option.HAS_FINISHED_A_DECK, false))
			{
				componentInChildren.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
			}
			else
			{
				componentInChildren.ChangeState(ActorStateType.HIGHLIGHT_MOUSE_OVER);
			}
		}
		SoundManager.Get().LoadAndPlay("Small_Mouseover", base.gameObject);
	}

	// Token: 0x0600422D RID: 16941 RVA: 0x0013FD70 File Offset: 0x0013DF70
	private void OnDeckTemplateHelpButtonOut(UIEvent e)
	{
		HighlightState componentInChildren = this.m_deckTemplateHelpButton.GetComponentInChildren<HighlightState>();
		if (componentInChildren != null)
		{
			if (!Options.Get().GetBool(Option.HAS_CLICKED_DECK_TEMPLATE_REPLACE, false))
			{
				componentInChildren.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
			}
			else
			{
				componentInChildren.ChangeState(ActorStateType.NONE);
			}
		}
	}

	// Token: 0x0600422E RID: 16942 RVA: 0x0013FDC0 File Offset: 0x0013DFC0
	private void LoadCardPrefabs(List<CollectionDeckSlot> deckSlots)
	{
		if (deckSlots.Count == 0)
		{
			return;
		}
		int prefabsToLoad = deckSlots.Count;
		this.m_loading = true;
		for (int i = 0; i < deckSlots.Count; i++)
		{
			CollectionDeckSlot collectionDeckSlot = deckSlots[i];
			if (collectionDeckSlot.Count == 0)
			{
				Log.Rachelle.Print(string.Format("CollectionDeckTray.LoadCardPrefabs(): Slot {0} of deck is empty! Skipping...", i), new object[0]);
			}
			else
			{
				DefLoader.Get().LoadCardDef(collectionDeckSlot.CardID, delegate(string _1, CardDef _2, object _3)
				{
					prefabsToLoad--;
					if (prefabsToLoad == 0)
					{
						this.m_loading = false;
					}
				}, null, new CardPortraitQuality(1, false));
			}
		}
	}

	// Token: 0x0600422F RID: 16943 RVA: 0x0013FE6C File Offset: 0x0013E06C
	private Vector3 GetOffscreenLocalPosition()
	{
		Vector3 originalLocalPosition = this.m_originalLocalPosition;
		CollectionDeck editingDeck = this.GetEditingDeck();
		int num = (editingDeck == null) ? 0 : (editingDeck.GetSlotCount() + 2);
		originalLocalPosition.z -= this.m_cardTileHeight * (float)num - this.GetCardTileOffset(editingDeck).y / this.m_cardTileSlotLocalScaleVec3.y;
		return originalLocalPosition;
	}

	// Token: 0x06004230 RID: 16944 RVA: 0x0013FED0 File Offset: 0x0013E0D0
	public void RegisterCardTileHeldListener(DeckTrayCardListContent.CardTileHeld dlg)
	{
		this.m_cardTileHeldListeners.Add(dlg);
	}

	// Token: 0x06004231 RID: 16945 RVA: 0x0013FEDE File Offset: 0x0013E0DE
	public void RegisterCardTilePressListener(DeckTrayCardListContent.CardTilePress dlg)
	{
		this.m_cardTilePressListeners.Add(dlg);
	}

	// Token: 0x06004232 RID: 16946 RVA: 0x0013FEEC File Offset: 0x0013E0EC
	public void RegisterCardTileTapListener(DeckTrayCardListContent.CardTileTap dlg)
	{
		this.m_cardTileTapListeners.Add(dlg);
	}

	// Token: 0x06004233 RID: 16947 RVA: 0x0013FEFA File Offset: 0x0013E0FA
	public void RegisterCardTileOverListener(DeckTrayCardListContent.CardTileOver dlg)
	{
		this.m_cardTileOverListeners.Add(dlg);
	}

	// Token: 0x06004234 RID: 16948 RVA: 0x0013FF08 File Offset: 0x0013E108
	public void RegisterCardTileOutListener(DeckTrayCardListContent.CardTileOut dlg)
	{
		this.m_cardTileOutListeners.Add(dlg);
	}

	// Token: 0x06004235 RID: 16949 RVA: 0x0013FF16 File Offset: 0x0013E116
	public void RegisterCardTileReleaseListener(DeckTrayCardListContent.CardTileRelease dlg)
	{
		this.m_cardTileReleaseListeners.Add(dlg);
	}

	// Token: 0x06004236 RID: 16950 RVA: 0x0013FF24 File Offset: 0x0013E124
	public void RegisterCardTileRightClickedListener(DeckTrayCardListContent.CardTileRightClicked dlg)
	{
		this.m_cardTileRightClickedListeners.Add(dlg);
	}

	// Token: 0x06004237 RID: 16951 RVA: 0x0013FF32 File Offset: 0x0013E132
	public void RegisterCardCountUpdated(DeckTrayCardListContent.CardCountChanged dlg)
	{
		this.m_cardCountChangedListeners.Add(dlg);
	}

	// Token: 0x06004238 RID: 16952 RVA: 0x0013FF40 File Offset: 0x0013E140
	public void UnregisterCardTileHeldListener(DeckTrayCardListContent.CardTileHeld dlg)
	{
		this.m_cardTileHeldListeners.Remove(dlg);
	}

	// Token: 0x06004239 RID: 16953 RVA: 0x0013FF4F File Offset: 0x0013E14F
	public void UnregisterCardTileTapListener(DeckTrayCardListContent.CardTileTap dlg)
	{
		this.m_cardTileTapListeners.Remove(dlg);
	}

	// Token: 0x0600423A RID: 16954 RVA: 0x0013FF5E File Offset: 0x0013E15E
	public void UnregisterCardTilePressListener(DeckTrayCardListContent.CardTilePress dlg)
	{
		this.m_cardTilePressListeners.Remove(dlg);
	}

	// Token: 0x0600423B RID: 16955 RVA: 0x0013FF6D File Offset: 0x0013E16D
	public void UnregisterCardTileOverListener(DeckTrayCardListContent.CardTileOver dlg)
	{
		this.m_cardTileOverListeners.Remove(dlg);
	}

	// Token: 0x0600423C RID: 16956 RVA: 0x0013FF7C File Offset: 0x0013E17C
	public void UnregisterCardTileOutListener(DeckTrayCardListContent.CardTileOut dlg)
	{
		this.m_cardTileOutListeners.Remove(dlg);
	}

	// Token: 0x0600423D RID: 16957 RVA: 0x0013FF8B File Offset: 0x0013E18B
	public void UnregisterCardTileReleaseListener(DeckTrayCardListContent.CardTileRelease dlg)
	{
		this.m_cardTileReleaseListeners.Remove(dlg);
	}

	// Token: 0x0600423E RID: 16958 RVA: 0x0013FF9A File Offset: 0x0013E19A
	public void UnregisterCardTileRightClickedListener(DeckTrayCardListContent.CardTileRightClicked dlg)
	{
		this.m_cardTileRightClickedListeners.Remove(dlg);
	}

	// Token: 0x0600423F RID: 16959 RVA: 0x0013FFA9 File Offset: 0x0013E1A9
	public void UnregisterCardCountUpdated(DeckTrayCardListContent.CardCountChanged dlg)
	{
		this.m_cardCountChangedListeners.Remove(dlg);
	}

	// Token: 0x06004240 RID: 16960 RVA: 0x0013FFB8 File Offset: 0x0013E1B8
	private void FireCardTileHeldEvent(DeckTrayDeckTileVisual cardTile)
	{
		DeckTrayCardListContent.CardTileHeld[] array = this.m_cardTileHeldListeners.ToArray();
		foreach (DeckTrayCardListContent.CardTileHeld cardTileHeld in array)
		{
			cardTileHeld(cardTile);
		}
	}

	// Token: 0x06004241 RID: 16961 RVA: 0x0013FFF4 File Offset: 0x0013E1F4
	private void FireCardTilePressEvent(DeckTrayDeckTileVisual cardTile)
	{
		DeckTrayCardListContent.CardTilePress[] array = this.m_cardTilePressListeners.ToArray();
		foreach (DeckTrayCardListContent.CardTilePress cardTilePress in array)
		{
			cardTilePress(cardTile);
		}
	}

	// Token: 0x06004242 RID: 16962 RVA: 0x00140030 File Offset: 0x0013E230
	private void FireCardTileTapEvent(DeckTrayDeckTileVisual cardTile)
	{
		DeckTrayCardListContent.CardTileTap[] array = this.m_cardTileTapListeners.ToArray();
		foreach (DeckTrayCardListContent.CardTileTap cardTileTap in array)
		{
			cardTileTap(cardTile);
		}
	}

	// Token: 0x06004243 RID: 16963 RVA: 0x0014006C File Offset: 0x0013E26C
	private void FireCardTileOverEvent(DeckTrayDeckTileVisual cardTile)
	{
		DeckTrayCardListContent.CardTileOver[] array = this.m_cardTileOverListeners.ToArray();
		foreach (DeckTrayCardListContent.CardTileOver cardTileOver in array)
		{
			cardTileOver(cardTile);
		}
	}

	// Token: 0x06004244 RID: 16964 RVA: 0x001400A8 File Offset: 0x0013E2A8
	private void FireCardTileOutEvent(DeckTrayDeckTileVisual cardTile)
	{
		DeckTrayCardListContent.CardTileOut[] array = this.m_cardTileOutListeners.ToArray();
		foreach (DeckTrayCardListContent.CardTileOut cardTileOut in array)
		{
			cardTileOut(cardTile);
		}
	}

	// Token: 0x06004245 RID: 16965 RVA: 0x001400E4 File Offset: 0x0013E2E4
	private void FireCardTileReleaseEvent(DeckTrayDeckTileVisual cardTile)
	{
		DeckTrayCardListContent.CardTileRelease[] array = this.m_cardTileReleaseListeners.ToArray();
		foreach (DeckTrayCardListContent.CardTileRelease cardTileRelease in array)
		{
			cardTileRelease(cardTile);
		}
	}

	// Token: 0x06004246 RID: 16966 RVA: 0x00140120 File Offset: 0x0013E320
	private void FireCardTileRightClickedEvent(DeckTrayDeckTileVisual cardTile)
	{
		DeckTrayCardListContent.CardTileRightClicked[] array = this.m_cardTileRightClickedListeners.ToArray();
		foreach (DeckTrayCardListContent.CardTileRightClicked cardTileRightClicked in array)
		{
			cardTileRightClicked(cardTile);
		}
	}

	// Token: 0x06004247 RID: 16967 RVA: 0x0014015C File Offset: 0x0013E35C
	private void FireCardCountChangedEvent()
	{
		DeckTrayCardListContent.CardCountChanged[] array = this.m_cardCountChangedListeners.ToArray();
		CollectionDeck editingDeck = this.GetEditingDeck();
		int cardCount = 0;
		if (editingDeck != null)
		{
			cardCount = ((SceneMgr.Get().GetMode() != SceneMgr.Mode.COLLECTIONMANAGER) ? editingDeck.GetTotalCardCount() : editingDeck.GetTotalValidCardCount());
		}
		foreach (DeckTrayCardListContent.CardCountChanged cardCountChanged in array)
		{
			cardCountChanged(cardCount);
		}
	}

	// Token: 0x040029F5 RID: 10741
	private const string ADD_CARD_TO_DECK_SOUND = "collection_manager_card_add_to_deck_instant";

	// Token: 0x040029F6 RID: 10742
	private const float CARD_MOVEMENT_TIME = 0.3f;

	// Token: 0x040029F7 RID: 10743
	private const float DECK_HELP_BUTTON_EMPTY_DECK_Y_LOCAL_POS = -0.01194457f;

	// Token: 0x040029F8 RID: 10744
	private const float DECK_HELP_BUTTON_Y_TILE_OFFSET = -0.04915909f;

	// Token: 0x040029F9 RID: 10745
	[CustomEditField(Sections = "Card Tile Settings")]
	public float m_cardTileHeight = 2.45f;

	// Token: 0x040029FA RID: 10746
	[CustomEditField(Sections = "Card Tile Settings")]
	public float m_cardHelpButtonHeight = 3f;

	// Token: 0x040029FB RID: 10747
	[CustomEditField(Sections = "Card Tile Settings")]
	public float m_deckCardBarFlareUpInterval = 0.075f;

	// Token: 0x040029FC RID: 10748
	[CustomEditField(Sections = "Card Tile Settings")]
	public GameObject m_phoneDeckTileBone;

	// Token: 0x040029FD RID: 10749
	[CustomEditField(Sections = "Card Tile Settings")]
	public Vector3 m_cardTileOffset = Vector3.zero;

	// Token: 0x040029FE RID: 10750
	[CustomEditField(Sections = "Card Tile Settings")]
	public float m_cardTileSlotLocalHeight;

	// Token: 0x040029FF RID: 10751
	[CustomEditField(Sections = "Card Tile Settings")]
	public Vector3 m_cardTileSlotLocalScaleVec3 = new Vector3(0.01f, 0.02f, 0.01f);

	// Token: 0x04002A00 RID: 10752
	[CustomEditField(Sections = "Deck Help")]
	public UIBButton m_deckHelpButton;

	// Token: 0x04002A01 RID: 10753
	[CustomEditField(Sections = "Deck Help")]
	public UIBButton m_deckTemplateHelpButton;

	// Token: 0x04002A02 RID: 10754
	[CustomEditField(Sections = "Other Objects")]
	public GameObject m_deckCompleteHighlight;

	// Token: 0x04002A03 RID: 10755
	[CustomEditField(Sections = "Scroll Settings")]
	public UIBScrollable m_scrollbar;

	// Token: 0x04002A04 RID: 10756
	[CustomEditField(Sections = "Deck Type Settings")]
	public CollectionManager.DeckTag m_deckType;

	// Token: 0x04002A05 RID: 10757
	private Vector3 m_originalLocalPosition;

	// Token: 0x04002A06 RID: 10758
	private List<DeckTrayDeckTileVisual> m_cardTiles = new List<DeckTrayDeckTileVisual>();

	// Token: 0x04002A07 RID: 10759
	private List<DeckTrayCardListContent.CardTileHeld> m_cardTileHeldListeners = new List<DeckTrayCardListContent.CardTileHeld>();

	// Token: 0x04002A08 RID: 10760
	private List<DeckTrayCardListContent.CardTilePress> m_cardTilePressListeners = new List<DeckTrayCardListContent.CardTilePress>();

	// Token: 0x04002A09 RID: 10761
	private List<DeckTrayCardListContent.CardTileTap> m_cardTileTapListeners = new List<DeckTrayCardListContent.CardTileTap>();

	// Token: 0x04002A0A RID: 10762
	private List<DeckTrayCardListContent.CardTileOver> m_cardTileOverListeners = new List<DeckTrayCardListContent.CardTileOver>();

	// Token: 0x04002A0B RID: 10763
	private List<DeckTrayCardListContent.CardTileOut> m_cardTileOutListeners = new List<DeckTrayCardListContent.CardTileOut>();

	// Token: 0x04002A0C RID: 10764
	private List<DeckTrayCardListContent.CardTileRelease> m_cardTileReleaseListeners = new List<DeckTrayCardListContent.CardTileRelease>();

	// Token: 0x04002A0D RID: 10765
	private List<DeckTrayCardListContent.CardTileRightClicked> m_cardTileRightClickedListeners = new List<DeckTrayCardListContent.CardTileRightClicked>();

	// Token: 0x04002A0E RID: 10766
	private List<DeckTrayCardListContent.CardCountChanged> m_cardCountChangedListeners = new List<DeckTrayCardListContent.CardCountChanged>();

	// Token: 0x04002A0F RID: 10767
	private bool m_animating;

	// Token: 0x04002A10 RID: 10768
	private bool m_loading;

	// Token: 0x04002A11 RID: 10769
	private bool m_inArena;

	// Token: 0x04002A12 RID: 10770
	private CollectionDeck m_templateFakeDeck = new CollectionDeck();

	// Token: 0x04002A13 RID: 10771
	private bool m_isShowingFakeDeck;

	// Token: 0x04002A14 RID: 10772
	private bool m_hasFinishedEntering;

	// Token: 0x04002A15 RID: 10773
	private bool m_hasFinishedExiting = true;

	// Token: 0x04002A16 RID: 10774
	private Notification m_deckHelpPopup;

	// Token: 0x020006D3 RID: 1747
	// (Invoke) Token: 0x0600486F RID: 18543
	public delegate void CardTileRightClicked(DeckTrayDeckTileVisual cardTile);

	// Token: 0x0200072C RID: 1836
	// (Invoke) Token: 0x06004AF3 RID: 19187
	public delegate void CardTileHeld(DeckTrayDeckTileVisual cardTile);

	// Token: 0x0200072D RID: 1837
	// (Invoke) Token: 0x06004AF7 RID: 19191
	public delegate void CardTilePress(DeckTrayDeckTileVisual cardTile);

	// Token: 0x0200072E RID: 1838
	// (Invoke) Token: 0x06004AFB RID: 19195
	public delegate void CardTileTap(DeckTrayDeckTileVisual cardTile);

	// Token: 0x0200072F RID: 1839
	// (Invoke) Token: 0x06004AFF RID: 19199
	public delegate void CardTileOver(DeckTrayDeckTileVisual cardTile);

	// Token: 0x02000730 RID: 1840
	// (Invoke) Token: 0x06004B03 RID: 19203
	public delegate void CardTileOut(DeckTrayDeckTileVisual cardTile);

	// Token: 0x02000731 RID: 1841
	// (Invoke) Token: 0x06004B07 RID: 19207
	public delegate void CardTileRelease(DeckTrayDeckTileVisual cardTile);

	// Token: 0x02000732 RID: 1842
	// (Invoke) Token: 0x06004B0B RID: 19211
	public delegate void CardCountChanged(int cardCount);
}
