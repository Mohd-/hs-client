using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000854 RID: 2132
public class EndTurnButtonReminder : MonoBehaviour
{
	// Token: 0x06005222 RID: 21026 RVA: 0x00188724 File Offset: 0x00186924
	public bool ShowFriendlySidePlayerTurnReminder()
	{
		GameState gameState = GameState.Get();
		if (gameState.IsMulliganManagerActive())
		{
			return false;
		}
		Player friendlySidePlayer = gameState.GetFriendlySidePlayer();
		if (friendlySidePlayer == null)
		{
			return false;
		}
		if (!friendlySidePlayer.IsCurrentPlayer())
		{
			return false;
		}
		ZoneMgr zoneMgr = ZoneMgr.Get();
		if (zoneMgr == null)
		{
			return false;
		}
		ZonePlay zonePlay = zoneMgr.FindZoneOfType<ZonePlay>(Player.Side.FRIENDLY);
		if (zonePlay == null)
		{
			return false;
		}
		List<Card> list = this.GenerateCardsToRemindList(gameState, zonePlay.GetCards());
		if (list.Count == 0)
		{
			return true;
		}
		this.PlayReminders(list);
		return true;
	}

	// Token: 0x06005223 RID: 21027 RVA: 0x001887B0 File Offset: 0x001869B0
	private List<Card> GenerateCardsToRemindList(GameState state, List<Card> originalList)
	{
		List<Card> list = new List<Card>();
		for (int i = 0; i < originalList.Count; i++)
		{
			Card card = originalList[i];
			if (state.HasResponse(card.GetEntity()))
			{
				list.Add(card);
			}
		}
		return list;
	}

	// Token: 0x06005224 RID: 21028 RVA: 0x001887FC File Offset: 0x001869FC
	private void PlayReminders(List<Card> cards)
	{
		int num;
		Card card;
		do
		{
			num = Random.Range(0, cards.Count);
			card = cards[num];
		}
		while (this.m_cardsWaitingToRemind.Contains(card));
		for (int i = 0; i < cards.Count; i++)
		{
			Card card2 = cards[i];
			Spell actorSpell = card2.GetActorSpell(SpellType.WIGGLE, true);
			if (!(actorSpell == null))
			{
				if (actorSpell.GetActiveState() == SpellStateType.NONE)
				{
					if (!this.m_cardsWaitingToRemind.Contains(card2))
					{
						if (i == num)
						{
							actorSpell.Activate();
						}
						else
						{
							float num2 = Random.Range(0f, this.m_MaxDelaySec);
							if (object.Equals(num2, 0f))
							{
								actorSpell.Activate();
							}
							else
							{
								this.m_cardsWaitingToRemind.Add(card2);
								base.StartCoroutine(this.WaitAndPlayReminder(card2, actorSpell, num2));
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06005225 RID: 21029 RVA: 0x001888FC File Offset: 0x00186AFC
	private IEnumerator WaitAndPlayReminder(Card card, Spell reminderSpell, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (!GameState.Get().IsFriendlySidePlayerTurn())
		{
			yield break;
		}
		if (!(card.GetZone() is ZonePlay))
		{
			yield break;
		}
		reminderSpell.Activate();
		this.m_cardsWaitingToRemind.Remove(card);
		yield break;
	}

	// Token: 0x0400386F RID: 14447
	public float m_MaxDelaySec = 0.3f;

	// Token: 0x04003870 RID: 14448
	private List<Card> m_cardsWaitingToRemind = new List<Card>();
}
