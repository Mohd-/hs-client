using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DF4 RID: 3572
	[ActionCategory("Pegasus")]
	[Tooltip("Put a Spell's Source or Target Card into a GameObject variable.")]
	public class SpellGetCardAction : SpellAction
	{
		// Token: 0x06006DF1 RID: 28145 RVA: 0x002048AE File Offset: 0x00202AAE
		protected override GameObject GetSpellOwner()
		{
			return base.Fsm.GetOwnerDefaultTarget(this.m_SpellObject);
		}

		// Token: 0x06006DF2 RID: 28146 RVA: 0x002048C1 File Offset: 0x00202AC1
		public override void Reset()
		{
			this.m_SpellObject = null;
			this.m_WhichCard = SpellAction.Which.SOURCE;
			this.m_GameObject = null;
		}

		// Token: 0x06006DF3 RID: 28147 RVA: 0x002048D8 File Offset: 0x00202AD8
		public override void OnEnter()
		{
			base.OnEnter();
			Card card = base.GetCard(this.m_WhichCard);
			if (card == null)
			{
				Error.AddDevFatal("SpellGetCardAction.OnEnter() - Card not found!", new object[0]);
				base.Finish();
				return;
			}
			if (!this.m_GameObject.IsNone)
			{
				this.m_GameObject.Value = card.gameObject;
			}
			base.Finish();
		}

		// Token: 0x04005696 RID: 22166
		public FsmOwnerDefault m_SpellObject;

		// Token: 0x04005697 RID: 22167
		public SpellAction.Which m_WhichCard;

		// Token: 0x04005698 RID: 22168
		public FsmGameObject m_GameObject;
	}
}
