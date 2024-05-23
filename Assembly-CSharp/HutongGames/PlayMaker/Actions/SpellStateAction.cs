using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020003E5 RID: 997
	[Tooltip("Handles communication between a Spell and the SpellStates in an FSM.")]
	[ActionCategory("Pegasus")]
	public class SpellStateAction : SpellAction
	{
		// Token: 0x06003396 RID: 13206 RVA: 0x00100F25 File Offset: 0x000FF125
		protected override GameObject GetSpellOwner()
		{
			return base.Fsm.GetOwnerDefaultTarget(this.m_SpellObject);
		}

		// Token: 0x06003397 RID: 13207 RVA: 0x00100F38 File Offset: 0x000FF138
		public override void OnEnter()
		{
			base.OnEnter();
			if (this.m_spell == null)
			{
				return;
			}
			this.DiscoverSpellStateType();
			if (this.m_stateInvalid)
			{
				return;
			}
			this.m_spell.OnFsmStateStarted(base.State, this.m_spellStateType);
			base.Finish();
		}

		// Token: 0x06003398 RID: 13208 RVA: 0x00100F8C File Offset: 0x000FF18C
		private void DiscoverSpellStateType()
		{
			if (!this.m_stateDirty)
			{
				return;
			}
			string name = base.State.Name;
			foreach (FsmTransition fsmTransition in base.Fsm.GlobalTransitions)
			{
				if (name.Equals(fsmTransition.ToState))
				{
					this.m_spellStateType = EnumUtils.GetEnum<SpellStateType>(fsmTransition.EventName);
					this.m_stateDirty = false;
					return;
				}
			}
			this.m_stateInvalid = true;
		}

		// Token: 0x0400200C RID: 8204
		public FsmOwnerDefault m_SpellObject;

		// Token: 0x0400200D RID: 8205
		private SpellStateType m_spellStateType;

		// Token: 0x0400200E RID: 8206
		private bool m_stateInvalid;

		// Token: 0x0400200F RID: 8207
		private bool m_stateDirty = true;
	}
}
