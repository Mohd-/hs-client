using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DF6 RID: 3574
	[ActionCategory("Pegasus")]
	[Tooltip("[DEBUG] Setup a Spell to affect multiple targets.")]
	public class SpellMultiTargetDebugAction : SpellAction
	{
		// Token: 0x06006DF8 RID: 28152 RVA: 0x002049F0 File Offset: 0x00202BF0
		protected override GameObject GetSpellOwner()
		{
			return this.m_SpellObject.Value;
		}

		// Token: 0x06006DF9 RID: 28153 RVA: 0x00204A00 File Offset: 0x00202C00
		public override void OnEnter()
		{
			base.OnEnter();
			if (this.m_spell == null)
			{
				return;
			}
			this.m_spell.SetSource(this.m_SourceObject.Value);
			this.m_spell.RemoveAllTargets();
			for (int i = 0; i < this.m_TargetObjects.Length; i++)
			{
				FsmGameObject fsmGameObject = this.m_TargetObjects[i];
				if (!(fsmGameObject.Value == null))
				{
					if (!this.m_spell.IsTarget(fsmGameObject.Value))
					{
						this.m_spell.AddTarget(fsmGameObject.Value);
					}
				}
			}
			base.Finish();
		}

		// Token: 0x0400569C RID: 22172
		public FsmGameObject m_SpellObject;

		// Token: 0x0400569D RID: 22173
		public FsmGameObject m_SourceObject;

		// Token: 0x0400569E RID: 22174
		public FsmGameObject[] m_TargetObjects;
	}
}
