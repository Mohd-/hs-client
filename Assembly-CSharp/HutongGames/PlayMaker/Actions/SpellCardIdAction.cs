using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DE9 RID: 3561
	[Tooltip("Send an event based on a Spell's Card's ID.")]
	[ActionCategory("Pegasus")]
	public class SpellCardIdAction : SpellAction
	{
		// Token: 0x06006DC0 RID: 28096 RVA: 0x00203E61 File Offset: 0x00202061
		protected override GameObject GetSpellOwner()
		{
			return base.Fsm.GetOwnerDefaultTarget(this.m_SpellObject);
		}

		// Token: 0x06006DC1 RID: 28097 RVA: 0x00203E74 File Offset: 0x00202074
		public override void Reset()
		{
			this.m_SpellObject = null;
			this.m_WhichCard = SpellAction.Which.SOURCE;
			this.m_Events = new FsmEvent[2];
			this.m_CardIds = new string[2];
		}

		// Token: 0x06006DC2 RID: 28098 RVA: 0x00203EA8 File Offset: 0x002020A8
		public override void OnEnter()
		{
			base.OnEnter();
			Card card = base.GetCard(this.m_WhichCard);
			if (card == null)
			{
				Error.AddDevFatal("SpellCardIdAction.OnEnter() - Card not found!", new object[0]);
				base.Finish();
				return;
			}
			string cardId = card.GetEntity().GetCardId();
			int indexMatchingCardId = base.GetIndexMatchingCardId(cardId, this.m_CardIds);
			if (indexMatchingCardId >= 0 && this.m_Events[indexMatchingCardId] != null)
			{
				base.Fsm.Event(this.m_Events[indexMatchingCardId]);
			}
			base.Finish();
		}

		// Token: 0x0400566A RID: 22122
		public FsmOwnerDefault m_SpellObject;

		// Token: 0x0400566B RID: 22123
		[Tooltip("Which Card to check on the Spell.")]
		public SpellAction.Which m_WhichCard;

		// Token: 0x0400566C RID: 22124
		[RequiredField]
		[CompoundArray("Events", "Event", "Card Id")]
		public FsmEvent[] m_Events;

		// Token: 0x0400566D RID: 22125
		public string[] m_CardIds;
	}
}
