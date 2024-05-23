using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200062F RID: 1583
public class DeckMaker
{
	// Token: 0x060044F5 RID: 17653 RVA: 0x0014B1D0 File Offset: 0x001493D0
	// Note: this type is marked as 'beforefieldinit'.
	static DeckMaker()
	{
		DeckMaker.CardRequirements[] array = new DeckMaker.CardRequirements[6];
		array[0] = new DeckMaker.CardRequirements(8, (EntityDef e) => DeckMaker.IsMinion(e) && DeckMaker.HasMinCost(e, 1) && DeckMaker.HasMaxCost(e, 2), "GLUE_RDM_LOW_COST");
		array[1] = new DeckMaker.CardRequirements(5, (EntityDef e) => DeckMaker.IsMinion(e) && DeckMaker.HasMinCost(e, 3) && DeckMaker.HasMaxCost(e, 4), "GLUE_RDM_MEDIUM_COST");
		array[2] = new DeckMaker.CardRequirements(4, (EntityDef e) => DeckMaker.IsMinion(e) && DeckMaker.HasMinCost(e, 5), "GLUE_RDM_HIGH_COST");
		array[3] = new DeckMaker.CardRequirements(7, (EntityDef e) => DeckMaker.IsSpell(e), "GLUE_RDM_MORE_SPELLS");
		array[4] = new DeckMaker.CardRequirements(2, (EntityDef e) => DeckMaker.IsWeapon(e), "GLUE_RDM_MORE_WEAPONS");
		array[5] = new DeckMaker.CardRequirements(int.MaxValue, (EntityDef e) => DeckMaker.IsMinion(e), "GLUE_RDM_NO_SPECIFICS");
		DeckMaker.s_OrderedCardRequirements = array;
	}

	// Token: 0x060044F6 RID: 17654 RVA: 0x0014B2EE File Offset: 0x001494EE
	private static bool IsMinion(EntityDef e)
	{
		return e.GetCardType() == TAG_CARDTYPE.MINION;
	}

	// Token: 0x060044F7 RID: 17655 RVA: 0x0014B2F9 File Offset: 0x001494F9
	private static bool IsSpell(EntityDef e)
	{
		return e.GetCardType() == TAG_CARDTYPE.SPELL;
	}

	// Token: 0x060044F8 RID: 17656 RVA: 0x0014B304 File Offset: 0x00149504
	private static bool IsWeapon(EntityDef e)
	{
		return e.GetCardType() == TAG_CARDTYPE.WEAPON;
	}

	// Token: 0x060044F9 RID: 17657 RVA: 0x0014B30F File Offset: 0x0014950F
	private static bool HasMinCost(EntityDef e, int minCost)
	{
		return e.GetCost() >= minCost;
	}

	// Token: 0x060044FA RID: 17658 RVA: 0x0014B31D File Offset: 0x0014951D
	private static bool HasMaxCost(EntityDef e, int maxCost)
	{
		return e.GetCost() <= maxCost;
	}

	// Token: 0x060044FB RID: 17659 RVA: 0x0014B32C File Offset: 0x0014952C
	public static IEnumerable<DeckMaker.DeckFill> GetFillCards(CollectionDeck deck, DeckRuleset deckRuleset)
	{
		bool replaceInvalid = true;
		List<EntityDef> currentDeckCards;
		List<EntityDef> currentInvalidCards;
		List<EntityDef> cardsICanAddToDeck;
		DeckMaker.InitFromDeck(deck, deckRuleset, out currentDeckCards, out currentInvalidCards, out cardsICanAddToDeck);
		int remainingCardsToFill = CollectionManager.Get().GetDeckSize() - currentDeckCards.Count;
		if (remainingCardsToFill <= 0)
		{
			yield break;
		}
		if (replaceInvalid)
		{
			foreach (DeckMaker.DeckFill replace in DeckMaker.GetInvalidFillCards(cardsICanAddToDeck, currentDeckCards, currentInvalidCards))
			{
				remainingCardsToFill--;
				yield return replace;
			}
		}
		for (int i = 0; i < DeckMaker.s_OrderedCardRequirements.Length; i++)
		{
			if (remainingCardsToFill <= 0)
			{
				break;
			}
			DeckMaker.CardRequirements cardReq = DeckMaker.s_OrderedCardRequirements[i];
			DeckMaker.CardRequirementsCondition condition = cardReq.m_condition;
			int cardsInDeckThatMatchReq = currentDeckCards.FindAll((EntityDef e) => condition(e)).Count;
			int cardsToAddFromSet = Mathf.Min(cardReq.m_requiredCount - cardsInDeckThatMatchReq, remainingCardsToFill);
			if (cardsToAddFromSet > 0)
			{
				List<EntityDef> filteredCardList = cardsICanAddToDeck.FindAll((EntityDef e) => condition(e));
				foreach (EntityDef addCard in filteredCardList)
				{
					if (cardsToAddFromSet <= 0)
					{
						break;
					}
					cardsICanAddToDeck.Remove(addCard);
					currentDeckCards.Add(addCard);
					cardsToAddFromSet--;
					remainingCardsToFill--;
					yield return new DeckMaker.DeckFill
					{
						m_addCard = addCard,
						m_reason = cardReq.GetRequirementReason()
					};
				}
			}
		}
		for (int j = 0; j < cardsICanAddToDeck.Count; j++)
		{
			EntityDef addCard2 = cardsICanAddToDeck[j];
			if (addCard2 != null)
			{
				currentDeckCards.Add(addCard2);
				cardsICanAddToDeck[j] = null;
				yield return new DeckMaker.DeckFill
				{
					m_addCard = addCard2
				};
			}
		}
		yield break;
	}

	// Token: 0x060044FC RID: 17660 RVA: 0x0014B364 File Offset: 0x00149564
	public static DeckMaker.DeckChoiceFill GetFillCardChoices(CollectionDeck deck, EntityDef referenceCard, int choices, DeckRuleset deckRuleset = null)
	{
		if (deckRuleset == null)
		{
			deckRuleset = deck.GetRuleset();
		}
		List<EntityDef> currentDeckCards;
		List<EntityDef> currentInvalidCards;
		List<EntityDef> cardsICanAddToDeck;
		DeckMaker.InitFromDeck(deck, deckRuleset, out currentDeckCards, out currentInvalidCards, out cardsICanAddToDeck);
		return DeckMaker.GetFillCard(referenceCard, cardsICanAddToDeck, currentDeckCards, currentInvalidCards, choices);
	}

	// Token: 0x060044FD RID: 17661 RVA: 0x0014B398 File Offset: 0x00149598
	private static void InitFromDeck(CollectionDeck deck, DeckRuleset deckRuleset, out List<EntityDef> currentDeckCards, out List<EntityDef> currentInvalidCards, out List<EntityDef> distinctCardsICanAddToDeck)
	{
		CollectionManager collectionManager = CollectionManager.Get();
		List<DeckMaker.SortableEntityDef> list = new List<DeckMaker.SortableEntityDef>();
		currentDeckCards = new List<EntityDef>();
		currentInvalidCards = new List<EntityDef>();
		foreach (CollectionDeckSlot collectionDeckSlot in deck.GetSlots())
		{
			CollectibleCard card = CollectionManager.Get().GetCard(collectionDeckSlot.CardID, collectionDeckSlot.Premium);
			if (card != null)
			{
				EntityDef entityDef = card.GetEntityDef();
				for (int i = 0; i < collectionDeckSlot.Count; i++)
				{
					if (deck.IsValidSlot(collectionDeckSlot))
					{
						currentDeckCards.Add(entityDef);
					}
					else
					{
						currentInvalidCards.Add(entityDef);
					}
				}
			}
		}
		KeyValuePair<string, EntityDef> kvpair;
		foreach (KeyValuePair<string, EntityDef> kvpair2 in DefLoader.Get().GetAllEntityDefs())
		{
			kvpair = kvpair2;
			CollectibleCard card2 = collectionManager.GetCard(kvpair.Key, TAG_PREMIUM.NORMAL);
			if (card2 != null && !card2.IsHero && (card2.Class == deck.GetClass() || card2.Class == TAG_CLASS.INVALID))
			{
				if (deckRuleset == null || deckRuleset.Filter(kvpair.Value))
				{
					int maxCopiesPerDeck = card2.MaxCopiesPerDeck;
					int num = card2.OwnedCount;
					CollectibleCard card3 = collectionManager.GetCard(kvpair.Key, TAG_PREMIUM.GOLDEN);
					if (card3 != null)
					{
						num += card3.OwnedCount;
					}
					int count = currentDeckCards.FindAll((EntityDef e) => e == kvpair.Value).Count;
					int num2 = Mathf.Min(maxCopiesPerDeck, num) - count;
					for (int j = 0; j < num2; j++)
					{
						list.Add(new DeckMaker.SortableEntityDef
						{
							m_entityDef = kvpair.Value,
							m_suggestWeight = card2.SuggestWeight
						});
					}
				}
			}
		}
		list.Sort(delegate(DeckMaker.SortableEntityDef lhs, DeckMaker.SortableEntityDef rhs)
		{
			int num3 = rhs.m_suggestWeight - lhs.m_suggestWeight;
			if (num3 != 0)
			{
				return num3;
			}
			return Random.Range(-1, 2);
		});
		distinctCardsICanAddToDeck = new List<EntityDef>();
		foreach (DeckMaker.SortableEntityDef sortableEntityDef in list)
		{
			distinctCardsICanAddToDeck.Add(sortableEntityDef.m_entityDef);
		}
	}

	// Token: 0x060044FE RID: 17662 RVA: 0x0014B67C File Offset: 0x0014987C
	private static IEnumerable<DeckMaker.DeckFill> GetInvalidFillCards(List<EntityDef> cardsICanAddToDeck, List<EntityDef> currentDeckCards, List<EntityDef> currentInvalidCards)
	{
		EntityDef[] tplCards = currentInvalidCards.ToArray();
		for (int i = 0; i < tplCards.Length; i++)
		{
			DeckMaker.DeckChoiceFill choiceFill = DeckMaker.GetFillCard(tplCards[i], cardsICanAddToDeck, null, currentInvalidCards, 1);
			DeckMaker.DeckFill choice = choiceFill.GetDeckFillChoice(0);
			if (DeckMaker.ReplaceInvalidCard(choice, cardsICanAddToDeck, currentDeckCards, currentInvalidCards))
			{
				yield return choice;
			}
		}
		yield break;
	}

	// Token: 0x060044FF RID: 17663 RVA: 0x0014B6C4 File Offset: 0x001498C4
	private static bool ReplaceInvalidCard(DeckMaker.DeckFill choice, List<EntityDef> cardsICanAddToDeck, List<EntityDef> currentDeckCards, List<EntityDef> currentInvalidCards)
	{
		if (choice == null)
		{
			return false;
		}
		if (!currentInvalidCards.Remove(choice.m_removeTemplate))
		{
			return false;
		}
		cardsICanAddToDeck.Remove(choice.m_addCard);
		currentDeckCards.Add(choice.m_addCard);
		return true;
	}

	// Token: 0x06004500 RID: 17664 RVA: 0x0014B708 File Offset: 0x00149908
	private static DeckMaker.DeckChoiceFill GetFillCard(EntityDef referenceCard, List<EntityDef> cardsICanAddToDeck, List<EntityDef> currentDeckCards, List<EntityDef> currentInvalidCards, int totalNumChoices = 3)
	{
		if (referenceCard == null && currentInvalidCards != null && currentInvalidCards.Count > 0)
		{
			referenceCard = Enumerable.First<EntityDef>(currentInvalidCards);
		}
		int cardRequirementsStartIndex = DeckMaker.GetCardRequirementsStartIndex(referenceCard, currentDeckCards);
		DeckMaker.DeckChoiceFill deckChoiceFill = new DeckMaker.DeckChoiceFill(referenceCard, new EntityDef[0]);
		for (int i = cardRequirementsStartIndex; i < DeckMaker.s_OrderedCardRequirements.Length; i++)
		{
			if (totalNumChoices <= 0)
			{
				break;
			}
			DeckMaker.CardRequirements cardRequirements = DeckMaker.s_OrderedCardRequirements[i];
			DeckMaker.CardRequirementsCondition condition = cardRequirements.m_condition;
			List<EntityDef> list = cardsICanAddToDeck.FindAll((EntityDef e) => condition(e));
			if (list.Count > 0)
			{
				int num = 4 * totalNumChoices;
				List<EntityDef> list2 = new List<EntityDef>();
				List<EntityDef> list3 = new List<EntityDef>();
				int num2 = int.MaxValue;
				if (referenceCard != null)
				{
					CollectibleCard card = CollectionManager.Get().GetCard(referenceCard.GetCardId(), TAG_PREMIUM.NORMAL);
					num2 = card.SuggestWeight;
				}
				foreach (EntityDef entityDef in Enumerable.Distinct<EntityDef>(list))
				{
					if (num <= 0)
					{
						break;
					}
					CollectibleCard card2 = CollectionManager.Get().GetCard(entityDef.GetCardId(), TAG_PREMIUM.NORMAL);
					if (num2 - card2.SuggestWeight > 100)
					{
						list3.Add(entityDef);
					}
					else
					{
						list2.Add(entityDef);
					}
					num--;
				}
				list2.Sort((EntityDef l, EntityDef r) => Random.Range(-1, 2));
				list3.Sort((EntityDef l, EntityDef r) => Random.Range(-1, 2));
				int num3 = Mathf.Min(list2.Count, totalNumChoices);
				int num4 = Mathf.Min(list3.Count, totalNumChoices - num3);
				if (num3 > 0)
				{
					deckChoiceFill.m_addChoices.AddRange(list2.GetRange(0, num3));
				}
				if (num4 > 0)
				{
					deckChoiceFill.m_addChoices.AddRange(list3.GetRange(0, num4));
				}
				totalNumChoices -= num3 + num4;
				deckChoiceFill.m_reason = ((referenceCard != null) ? GameStrings.Format("GLUE_RDM_TEMPLATE_REPLACE", new object[]
				{
					referenceCard.GetName()
				}) : cardRequirements.GetRequirementReason());
			}
		}
		return deckChoiceFill;
	}

	// Token: 0x06004501 RID: 17665 RVA: 0x0014B964 File Offset: 0x00149B64
	private static int GetCardRequirementsStartIndex(EntityDef referenceCard, List<EntityDef> currentDeckCards)
	{
		if (referenceCard != null)
		{
			for (int i = 0; i < DeckMaker.s_OrderedCardRequirements.Length; i++)
			{
				if (DeckMaker.s_OrderedCardRequirements[i].m_condition(referenceCard))
				{
					return i;
				}
			}
		}
		else if (currentDeckCards != null)
		{
			for (int j = 0; j < DeckMaker.s_OrderedCardRequirements.Length; j++)
			{
				DeckMaker.CardRequirements cardRequirements = DeckMaker.s_OrderedCardRequirements[j];
				DeckMaker.CardRequirementsCondition condition = cardRequirements.m_condition;
				int count = currentDeckCards.FindAll((EntityDef e) => condition(e)).Count;
				if (count < cardRequirements.m_requiredCount)
				{
					return j;
				}
			}
		}
		return 0;
	}

	// Token: 0x04002BDA RID: 11226
	private const int s_preChoiceRandomizationMultipler = 4;

	// Token: 0x04002BDB RID: 11227
	private const int s_priorityWeightDifference = 100;

	// Token: 0x04002BDC RID: 11228
	private static readonly DeckMaker.CardRequirements[] s_OrderedCardRequirements;

	// Token: 0x02000630 RID: 1584
	public class DeckFill
	{
		// Token: 0x04002BE6 RID: 11238
		public EntityDef m_removeTemplate;

		// Token: 0x04002BE7 RID: 11239
		public EntityDef m_addCard;

		// Token: 0x04002BE8 RID: 11240
		public string m_reason;
	}

	// Token: 0x02000741 RID: 1857
	public class DeckChoiceFill
	{
		// Token: 0x06004B4B RID: 19275 RVA: 0x00167C94 File Offset: 0x00165E94
		public DeckChoiceFill(EntityDef remove, params EntityDef[] addChoices)
		{
			this.m_removeTemplate = remove;
			if (addChoices != null && addChoices.Length > 0)
			{
				this.m_addChoices = new List<EntityDef>(addChoices);
			}
		}

		// Token: 0x06004B4C RID: 19276 RVA: 0x00167CCC File Offset: 0x00165ECC
		public DeckMaker.DeckFill GetDeckFillChoice(int idx)
		{
			if (idx >= this.m_addChoices.Count)
			{
				return null;
			}
			return new DeckMaker.DeckFill
			{
				m_removeTemplate = this.m_removeTemplate,
				m_addCard = this.m_addChoices[idx],
				m_reason = this.m_reason
			};
		}

		// Token: 0x04003218 RID: 12824
		public EntityDef m_removeTemplate;

		// Token: 0x04003219 RID: 12825
		public List<EntityDef> m_addChoices = new List<EntityDef>();

		// Token: 0x0400321A RID: 12826
		public string m_reason;
	}

	// Token: 0x02000742 RID: 1858
	private class CardRequirements
	{
		// Token: 0x06004B4D RID: 19277 RVA: 0x00167D1D File Offset: 0x00165F1D
		public CardRequirements(int requiredCount, DeckMaker.CardRequirementsCondition condition, string reason = "")
		{
			this.m_requiredCount = requiredCount;
			this.m_condition = condition;
			this.m_reason = reason;
		}

		// Token: 0x06004B4E RID: 19278 RVA: 0x00167D3A File Offset: 0x00165F3A
		public string GetRequirementReason()
		{
			if (string.IsNullOrEmpty(this.m_reason))
			{
				return "No reason!";
			}
			return GameStrings.Get(this.m_reason);
		}

		// Token: 0x0400321B RID: 12827
		public int m_requiredCount;

		// Token: 0x0400321C RID: 12828
		public DeckMaker.CardRequirementsCondition m_condition;

		// Token: 0x0400321D RID: 12829
		private string m_reason;
	}

	// Token: 0x02000743 RID: 1859
	// (Invoke) Token: 0x06004B50 RID: 19280
	public delegate bool CardRequirementsCondition(EntityDef entityDef);

	// Token: 0x02000744 RID: 1860
	private class SortableEntityDef
	{
		// Token: 0x0400321E RID: 12830
		public EntityDef m_entityDef;

		// Token: 0x0400321F RID: 12831
		public int m_suggestWeight;
	}
}
