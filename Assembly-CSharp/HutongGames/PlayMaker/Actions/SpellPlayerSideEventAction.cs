using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DF7 RID: 3575
	[Tooltip("Send an event based on the player side of a Spell.")]
	[ActionCategory("Pegasus")]
	public class SpellPlayerSideEventAction : SpellAction
	{
		// Token: 0x06006DFB RID: 28155 RVA: 0x00204AB3 File Offset: 0x00202CB3
		protected override GameObject GetSpellOwner()
		{
			return base.Fsm.GetOwnerDefaultTarget(this.m_SpellObject);
		}

		// Token: 0x06006DFC RID: 28156 RVA: 0x00204AC6 File Offset: 0x00202CC6
		public override void Reset()
		{
			this.m_SpellObject = null;
			this.m_WhichCard = SpellAction.Which.SOURCE;
			this.m_FriendlyEvent = null;
			this.m_OpponentEvent = null;
			this.m_NeutralEvent = null;
		}

		// Token: 0x06006DFD RID: 28157 RVA: 0x00204AEC File Offset: 0x00202CEC
		public override void OnEnter()
		{
			base.OnEnter();
			Card card = base.GetCard(this.m_WhichCard);
			if (card == null)
			{
				Error.AddDevFatal("SpellPlayerSideEventAction.OnEnter() - Card not found for spell {0}", new object[]
				{
					base.GetSpell()
				});
				base.Finish();
				return;
			}
			Player.Side controllerSide = card.GetEntity().GetControllerSide();
			Player.Side side = controllerSide;
			if (side != Player.Side.FRIENDLY)
			{
				if (side != Player.Side.OPPOSING)
				{
					base.Fsm.Event(this.m_NeutralEvent);
				}
				else
				{
					base.Fsm.Event(this.m_OpponentEvent);
				}
			}
			else
			{
				base.Fsm.Event(this.m_FriendlyEvent);
			}
			base.Finish();
		}

		// Token: 0x0400569F RID: 22175
		public FsmOwnerDefault m_SpellObject;

		// Token: 0x040056A0 RID: 22176
		public SpellAction.Which m_WhichCard;

		// Token: 0x040056A1 RID: 22177
		public FsmEvent m_FriendlyEvent;

		// Token: 0x040056A2 RID: 22178
		public FsmEvent m_OpponentEvent;

		// Token: 0x040056A3 RID: 22179
		public FsmEvent m_NeutralEvent;
	}
}
