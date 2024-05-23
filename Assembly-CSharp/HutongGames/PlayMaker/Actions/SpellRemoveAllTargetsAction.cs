using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DF8 RID: 3576
	[ActionCategory("Pegasus")]
	[Tooltip("Removes all targets from a Spell.")]
	public class SpellRemoveAllTargetsAction : SpellAction
	{
		// Token: 0x06006DFF RID: 28159 RVA: 0x00204BA9 File Offset: 0x00202DA9
		protected override GameObject GetSpellOwner()
		{
			return base.Fsm.GetOwnerDefaultTarget(this.m_SpellObject);
		}

		// Token: 0x06006E00 RID: 28160 RVA: 0x00204BBC File Offset: 0x00202DBC
		public override void OnEnter()
		{
			base.OnEnter();
			if (this.m_spell == null)
			{
				return;
			}
			this.m_spell.RemoveAllTargets();
			base.Finish();
		}

		// Token: 0x040056A4 RID: 22180
		public FsmOwnerDefault m_SpellObject;
	}
}
