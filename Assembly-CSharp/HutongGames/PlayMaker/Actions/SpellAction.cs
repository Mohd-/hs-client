using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DE6 RID: 3558
	[Tooltip("INTERNAL USE ONLY. Do not put this on your FSMs.")]
	[ActionCategory("Pegasus")]
	public abstract class SpellAction : FsmStateAction
	{
		// Token: 0x06006DB5 RID: 28085 RVA: 0x00203C6C File Offset: 0x00201E6C
		public Spell GetSpell()
		{
			if (this.m_spell == null)
			{
				GameObject spellOwner = this.GetSpellOwner();
				if (spellOwner != null)
				{
					this.m_spell = SceneUtils.FindComponentInThisOrParents<Spell>(spellOwner);
				}
			}
			return this.m_spell;
		}

		// Token: 0x06006DB6 RID: 28086 RVA: 0x00203CB0 File Offset: 0x00201EB0
		public Card GetCard(SpellAction.Which which)
		{
			Spell spell = this.GetSpell();
			if (spell == null)
			{
				return null;
			}
			if (which == SpellAction.Which.TARGET)
			{
				return spell.GetTargetCard();
			}
			Card sourceCard = spell.GetSourceCard();
			if (which == SpellAction.Which.SOURCE_HERO && sourceCard != null)
			{
				return sourceCard.GetHeroCard();
			}
			return sourceCard;
		}

		// Token: 0x06006DB7 RID: 28087 RVA: 0x00203D04 File Offset: 0x00201F04
		public Actor GetActor(SpellAction.Which which)
		{
			Card card = this.GetCard(which);
			if (card == null)
			{
				return null;
			}
			return card.GetActor();
		}

		// Token: 0x06006DB8 RID: 28088 RVA: 0x00203D30 File Offset: 0x00201F30
		public int GetIndexMatchingCardId(string cardId, string[] cardIds)
		{
			if (cardIds == null || cardIds.Length == 0)
			{
				return -1;
			}
			for (int i = 0; i < cardIds.Length; i++)
			{
				string text = cardIds[i].Trim();
				if (cardId.Equals(text, 5))
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06006DB9 RID: 28089
		protected abstract GameObject GetSpellOwner();

		// Token: 0x06006DBA RID: 28090 RVA: 0x00203D7C File Offset: 0x00201F7C
		public override void OnEnter()
		{
			this.GetSpell();
			if (this.m_spell == null)
			{
				Debug.LogError(string.Format("{0}.OnEnter() - FAILED to find Spell component on Owner \"{1}\"", this, base.Owner));
				return;
			}
		}

		// Token: 0x04005662 RID: 22114
		protected Spell m_spell;

		// Token: 0x02000DE7 RID: 3559
		public enum Which
		{
			// Token: 0x04005664 RID: 22116
			SOURCE,
			// Token: 0x04005665 RID: 22117
			TARGET,
			// Token: 0x04005666 RID: 22118
			SOURCE_HERO
		}
	}
}
