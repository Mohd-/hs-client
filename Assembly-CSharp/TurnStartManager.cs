using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000618 RID: 1560
public class TurnStartManager : MonoBehaviour
{
	// Token: 0x060043C9 RID: 17353 RVA: 0x00144AC8 File Offset: 0x00142CC8
	private void Awake()
	{
		TurnStartManager.s_instance = this;
		this.m_turnStartInstance = Object.Instantiate<TurnStartIndicator>(this.m_turnStartPrefab);
		this.m_turnStartInstance.transform.parent = base.transform;
		GameState.Get().RegisterGameOverListener(new GameState.GameOverCallback(this.OnGameOver), null);
	}

	// Token: 0x060043CA RID: 17354 RVA: 0x00144B1A File Offset: 0x00142D1A
	private void OnDestroy()
	{
		TurnStartManager.s_instance = null;
	}

	// Token: 0x060043CB RID: 17355 RVA: 0x00144B22 File Offset: 0x00142D22
	public static TurnStartManager Get()
	{
		return TurnStartManager.s_instance;
	}

	// Token: 0x060043CC RID: 17356 RVA: 0x00144B29 File Offset: 0x00142D29
	public bool IsListeningForTurnEvents()
	{
		return this.m_listeningForTurnEvents;
	}

	// Token: 0x060043CD RID: 17357 RVA: 0x00144B31 File Offset: 0x00142D31
	public void BeginListeningForTurnEvents()
	{
		this.m_cardsToDraw.Clear();
		this.m_exhaustedChangesToHandle.Clear();
		this.m_manaCrystalsGained = 0;
		this.m_manaCrystalsFilled = 0;
		this.m_twoScoopsDisplayed = false;
		this.m_listeningForTurnEvents = true;
		this.m_blockingInput = true;
	}

	// Token: 0x060043CE RID: 17358 RVA: 0x00144B6C File Offset: 0x00142D6C
	public void NotifyOfManaCrystalGained(int amount)
	{
		this.m_manaCrystalsGained += amount;
	}

	// Token: 0x060043CF RID: 17359 RVA: 0x00144B7C File Offset: 0x00142D7C
	public void NotifyOfManaCrystalFilled(int amount)
	{
		this.m_manaCrystalsFilled += amount;
	}

	// Token: 0x060043D0 RID: 17360 RVA: 0x00144B8C File Offset: 0x00142D8C
	public void NotifyOfCardDrawn(Entity drawnEntity)
	{
		this.m_cardsToDraw.Add(drawnEntity.GetCard());
	}

	// Token: 0x060043D1 RID: 17361 RVA: 0x00144BA0 File Offset: 0x00142DA0
	public void NotifyOfExhaustedChange(Card card, TagDelta tagChange)
	{
		TurnStartManager.CardChange cardChange = new TurnStartManager.CardChange
		{
			m_card = card,
			m_tagDelta = tagChange
		};
		this.m_exhaustedChangesToHandle.Add(cardChange);
	}

	// Token: 0x060043D2 RID: 17362 RVA: 0x00144BCF File Offset: 0x00142DCF
	public void NotifyOfSpellController(SpellController spellController)
	{
		this.m_spellController = spellController;
		this.BeginPlayingTurnEvents();
	}

	// Token: 0x060043D3 RID: 17363 RVA: 0x00144BDE File Offset: 0x00142DDE
	public SpellController GetSpellController()
	{
		return this.m_spellController;
	}

	// Token: 0x060043D4 RID: 17364 RVA: 0x00144BE6 File Offset: 0x00142DE6
	public int GetNumCardsToDraw()
	{
		return this.m_cardsToDraw.Count;
	}

	// Token: 0x060043D5 RID: 17365 RVA: 0x00144BF3 File Offset: 0x00142DF3
	public List<Card> GetCardsToDraw()
	{
		return this.m_cardsToDraw;
	}

	// Token: 0x060043D6 RID: 17366 RVA: 0x00144BFB File Offset: 0x00142DFB
	public bool IsCardDrawHandled(Card card)
	{
		return !(card == null) && this.m_cardsToDraw.Contains(card);
	}

	// Token: 0x060043D7 RID: 17367 RVA: 0x00144C17 File Offset: 0x00142E17
	public void BeginPlayingTurnEvents()
	{
		base.StartCoroutine(this.RunTurnEventsWithTiming());
	}

	// Token: 0x060043D8 RID: 17368 RVA: 0x00144C26 File Offset: 0x00142E26
	public void NotifyOfTriggerVisual()
	{
		this.DisplayTwoScoops();
	}

	// Token: 0x060043D9 RID: 17369 RVA: 0x00144C2E File Offset: 0x00142E2E
	public bool IsBlockingInput()
	{
		return this.m_blockingInput;
	}

	// Token: 0x060043DA RID: 17370 RVA: 0x00144C38 File Offset: 0x00142E38
	private void DisplayTwoScoops()
	{
		if (this.m_twoScoopsDisplayed)
		{
			return;
		}
		this.m_twoScoopsDisplayed = true;
		this.m_turnStartInstance.SetReminderText(GameState.Get().GetGameEntity().GetTurnStartReminderText());
		this.m_turnStartInstance.Show();
		SoundManager.Get().LoadAndPlay("ALERT_YourTurn_0v2");
	}

	// Token: 0x060043DB RID: 17371 RVA: 0x00144C8C File Offset: 0x00142E8C
	private IEnumerator RunTurnEventsWithTiming()
	{
		if (!this.IsListeningForTurnEvents())
		{
			yield break;
		}
		this.m_listeningForTurnEvents = false;
		if (GameMgr.Get().IsAI() && !this.m_twoScoopsDisplayed)
		{
			yield return new WaitForSeconds(1f);
		}
		this.DisplayTwoScoops();
		Player friendlyPlayer = GameState.Get().GetFriendlySidePlayer();
		friendlyPlayer.ReadyManaCrystal(this.m_manaCrystalsFilled);
		friendlyPlayer.AddManaCrystal(this.m_manaCrystalsGained, true);
		friendlyPlayer.UpdateManaCounter();
		this.HandleExhaustedChanges();
		if (this.m_turnStartInstance.IsShown())
		{
			yield return new WaitForSeconds(1f);
		}
		if (this.m_cardsToDraw.Count > 0)
		{
			Card[] cardsToDraw = this.m_cardsToDraw.ToArray();
			this.m_cardsToDraw.Clear();
			ZoneHand handZone = friendlyPlayer.GetHandZone();
			handZone.UpdateLayout();
			foreach (Card card in cardsToDraw)
			{
				while (card.IsActorLoading())
				{
					yield return null;
				}
				card.DrawFriendlyCard();
			}
			while (!this.AreDrawnCardsReady(cardsToDraw))
			{
				yield return null;
			}
			if (this.HasActionsAfterCardDraw())
			{
				yield return new WaitForSeconds(0.35f);
			}
		}
		if (this.m_spellController)
		{
			this.m_spellController.DoPowerTaskList();
			while (this.m_spellController.IsProcessingTaskList())
			{
				yield return null;
			}
			this.m_spellController = null;
		}
		if (GameState.Get().IsFriendlySidePlayerTurn())
		{
			this.m_blockingInput = false;
			EndTurnButton.Get().OnTurnStartManagerFinished();
			if (GameState.Get().IsInMainOptionMode())
			{
				GameState.Get().EnterMainOptionMode();
			}
			GameState.Get().FireFriendlyTurnStartedEvent();
		}
		yield break;
	}

	// Token: 0x060043DC RID: 17372 RVA: 0x00144CA8 File Offset: 0x00142EA8
	private bool AreDrawnCardsReady(Card[] cardsToDraw)
	{
		Card card2 = Array.Find<Card>(cardsToDraw, (Card card) => !card.IsActorReady());
		return !card2;
	}

	// Token: 0x060043DD RID: 17373 RVA: 0x00144CE4 File Offset: 0x00142EE4
	private bool HasActionsAfterCardDraw()
	{
		if (this.m_spellController != null)
		{
			return true;
		}
		Network.EntityChoices friendlyEntityChoices = GameState.Get().GetFriendlyEntityChoices();
		return friendlyEntityChoices != null && friendlyEntityChoices.ChoiceType == CHOICE_TYPE.GENERAL;
	}

	// Token: 0x060043DE RID: 17374 RVA: 0x00144D24 File Offset: 0x00142F24
	private void HandleExhaustedChanges()
	{
		foreach (TurnStartManager.CardChange cardChange in this.m_exhaustedChangesToHandle)
		{
			Card card = cardChange.m_card;
			Entity entity = card.GetEntity();
			TAG_ZONE zone = entity.GetZone();
			if (zone == TAG_ZONE.PLAY || zone == TAG_ZONE.SECRET)
			{
				card.ShowExhaustedChange(cardChange.m_tagDelta.newValue);
			}
		}
		this.m_exhaustedChangesToHandle.Clear();
	}

	// Token: 0x060043DF RID: 17375 RVA: 0x00144DC0 File Offset: 0x00142FC0
	private void OnGameOver(object userData)
	{
		base.StopAllCoroutines();
	}

	// Token: 0x04002AFD RID: 11005
	public TurnStartIndicator m_turnStartPrefab;

	// Token: 0x04002AFE RID: 11006
	private static TurnStartManager s_instance;

	// Token: 0x04002AFF RID: 11007
	private TurnStartIndicator m_turnStartInstance;

	// Token: 0x04002B00 RID: 11008
	private bool m_listeningForTurnEvents;

	// Token: 0x04002B01 RID: 11009
	private int m_manaCrystalsGained;

	// Token: 0x04002B02 RID: 11010
	private int m_manaCrystalsFilled;

	// Token: 0x04002B03 RID: 11011
	private List<Card> m_cardsToDraw = new List<Card>();

	// Token: 0x04002B04 RID: 11012
	private List<TurnStartManager.CardChange> m_exhaustedChangesToHandle = new List<TurnStartManager.CardChange>();

	// Token: 0x04002B05 RID: 11013
	private SpellController m_spellController;

	// Token: 0x04002B06 RID: 11014
	private bool m_blockingInput;

	// Token: 0x04002B07 RID: 11015
	private bool m_twoScoopsDisplayed;

	// Token: 0x020008FF RID: 2303
	private class CardChange
	{
		// Token: 0x04003C78 RID: 15480
		public Card m_card;

		// Token: 0x04003C79 RID: 15481
		public TagDelta m_tagDelta;
	}
}
