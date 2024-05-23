using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DF1 RID: 3569
	[Tooltip("Tells the game that a Spell is finished, allowing the game to progress.")]
	[ActionCategory("Pegasus")]
	public class SpellFinishAction : SpellAction
	{
		// Token: 0x06006DE3 RID: 28131 RVA: 0x002046C5 File Offset: 0x002028C5
		protected override GameObject GetSpellOwner()
		{
			return base.Fsm.GetOwnerDefaultTarget(this.m_SpellObject);
		}

		// Token: 0x06006DE4 RID: 28132 RVA: 0x002046D8 File Offset: 0x002028D8
		public override void OnEnter()
		{
			base.OnEnter();
			if (this.m_spell == null)
			{
				Debug.LogError(string.Format("{0}.OnEnter() - FAILED to find Spell component on Owner \"{1}\"", this, base.Owner));
				return;
			}
			if (this.m_Delay.Value > 0f)
			{
				this.m_spell.StartCoroutine(this.DelaySpellFinished());
			}
			else
			{
				this.m_spell.OnSpellFinished();
			}
			base.Finish();
		}

		// Token: 0x06006DE5 RID: 28133 RVA: 0x00204750 File Offset: 0x00202950
		private IEnumerator DelaySpellFinished()
		{
			yield return new WaitForSeconds(this.m_Delay.Value);
			this.m_spell.OnSpellFinished();
			yield break;
		}

		// Token: 0x0400568E RID: 22158
		public FsmOwnerDefault m_SpellObject;

		// Token: 0x0400568F RID: 22159
		public FsmFloat m_Delay = 0f;
	}
}
