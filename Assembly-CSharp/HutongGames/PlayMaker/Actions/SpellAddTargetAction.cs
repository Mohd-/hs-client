using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DE8 RID: 3560
	[ActionCategory("Pegasus")]
	[Tooltip("Adds a target to a Spell.")]
	public class SpellAddTargetAction : SpellAction
	{
		// Token: 0x06006DBC RID: 28092 RVA: 0x00203DC0 File Offset: 0x00201FC0
		protected override GameObject GetSpellOwner()
		{
			return base.Fsm.GetOwnerDefaultTarget(this.m_SpellObject);
		}

		// Token: 0x06006DBD RID: 28093 RVA: 0x00203DD3 File Offset: 0x00201FD3
		public override void Reset()
		{
			base.Reset();
			this.m_AllowDuplicates = false;
		}

		// Token: 0x06006DBE RID: 28094 RVA: 0x00203DE8 File Offset: 0x00201FE8
		public override void OnEnter()
		{
			base.OnEnter();
			if (this.m_spell == null)
			{
				return;
			}
			GameObject value = this.m_TargetObject.Value;
			if (value == null)
			{
				return;
			}
			if (!this.m_spell.IsTarget(value) || this.m_AllowDuplicates.Value)
			{
				this.m_spell.AddTarget(value);
			}
			base.Finish();
		}

		// Token: 0x04005667 RID: 22119
		public FsmOwnerDefault m_SpellObject;

		// Token: 0x04005668 RID: 22120
		public FsmGameObject m_TargetObject;

		// Token: 0x04005669 RID: 22121
		public FsmBool m_AllowDuplicates;
	}
}
