using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DF9 RID: 3577
	[Tooltip("Removes a target from a Spell.")]
	[ActionCategory("Pegasus")]
	public class SpellRemoveTargetAction : SpellAction
	{
		// Token: 0x06006E02 RID: 28162 RVA: 0x00204BEF File Offset: 0x00202DEF
		protected override GameObject GetSpellOwner()
		{
			return base.Fsm.GetOwnerDefaultTarget(this.m_SpellObject);
		}

		// Token: 0x06006E03 RID: 28163 RVA: 0x00204C04 File Offset: 0x00202E04
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
			this.m_spell.RemoveTarget(value);
			base.Finish();
		}

		// Token: 0x040056A5 RID: 22181
		public FsmOwnerDefault m_SpellObject;

		// Token: 0x040056A6 RID: 22182
		public FsmGameObject m_TargetObject;
	}
}
