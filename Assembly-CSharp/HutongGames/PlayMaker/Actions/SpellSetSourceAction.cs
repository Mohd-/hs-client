using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DFA RID: 3578
	[Tooltip("Sets the source for a Spell.")]
	[ActionCategory("Pegasus")]
	public class SpellSetSourceAction : SpellAction
	{
		// Token: 0x06006E05 RID: 28165 RVA: 0x00204C5D File Offset: 0x00202E5D
		protected override GameObject GetSpellOwner()
		{
			return base.Fsm.GetOwnerDefaultTarget(this.m_SpellObject);
		}

		// Token: 0x06006E06 RID: 28166 RVA: 0x00204C70 File Offset: 0x00202E70
		public override void OnEnter()
		{
			base.OnEnter();
			if (this.m_spell == null)
			{
				return;
			}
			GameObject value = this.m_SourceObject.Value;
			this.m_spell.SetSource(value);
			base.Finish();
		}

		// Token: 0x040056A7 RID: 22183
		public FsmOwnerDefault m_SpellObject;

		// Token: 0x040056A8 RID: 22184
		public FsmGameObject m_SourceObject;
	}
}
