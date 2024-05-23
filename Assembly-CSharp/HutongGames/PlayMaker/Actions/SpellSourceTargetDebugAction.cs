using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DFB RID: 3579
	[ActionCategory("Pegasus")]
	[Tooltip("[DEBUG] Setup a Spell to go from a source to a target.")]
	public class SpellSourceTargetDebugAction : SpellAction
	{
		// Token: 0x06006E08 RID: 28168 RVA: 0x00204CBB File Offset: 0x00202EBB
		protected override GameObject GetSpellOwner()
		{
			return this.m_SpellObject.Value;
		}

		// Token: 0x06006E09 RID: 28169 RVA: 0x00204CC8 File Offset: 0x00202EC8
		public override void OnEnter()
		{
			base.OnEnter();
			if (this.m_spell == null)
			{
				return;
			}
			this.m_spell.RemoveAllTargets();
			this.m_spell.SetSource(this.m_SourceObject.Value);
			if (this.m_TargetObject.Value != null)
			{
				this.m_spell.AddTarget(this.m_TargetObject.Value);
			}
			base.Finish();
		}

		// Token: 0x040056A9 RID: 22185
		public FsmGameObject m_SpellObject;

		// Token: 0x040056AA RID: 22186
		public FsmGameObject m_SourceObject;

		// Token: 0x040056AB RID: 22187
		public FsmGameObject m_TargetObject;
	}
}
