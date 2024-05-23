using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DEF RID: 3567
	[ActionCategory("Pegasus")]
	[Tooltip("Tells the game that a Spell is finished, allowing the game to progress.")]
	public class SpellEventAction : SpellAction
	{
		// Token: 0x06006DD8 RID: 28120 RVA: 0x002044EF File Offset: 0x002026EF
		protected override GameObject GetSpellOwner()
		{
			return base.Fsm.GetOwnerDefaultTarget(this.m_SpellObject);
		}

		// Token: 0x06006DD9 RID: 28121 RVA: 0x00204504 File Offset: 0x00202704
		public override void Reset()
		{
			this.m_SpellObject = null;
			this.m_Delay = 0f;
			this.m_EventName = string.Empty;
			this.m_EventData = null;
		}

		// Token: 0x06006DDA RID: 28122 RVA: 0x00204540 File Offset: 0x00202740
		public override void OnEnter()
		{
			base.OnEnter();
			if (this.m_spell == null)
			{
				Debug.LogErrorFormat("{0}.OnEnter() - FAILED to find Spell component on Owner \"{1}\"", new object[]
				{
					this,
					base.Owner
				});
				return;
			}
			if (this.m_Delay.Value > 0f)
			{
				this.m_spell.StartCoroutine(this.DelaySpellEvent());
			}
			else
			{
				this.m_spell.OnSpellEvent(this.m_EventName.Value, this.m_EventData.Value);
			}
			base.Finish();
		}

		// Token: 0x06006DDB RID: 28123 RVA: 0x002045D8 File Offset: 0x002027D8
		private IEnumerator DelaySpellEvent()
		{
			yield return new WaitForSeconds(this.m_Delay.Value);
			this.m_spell.OnSpellEvent(this.m_EventName.Value, this.m_EventData.Value);
			yield break;
		}

		// Token: 0x04005687 RID: 22151
		public FsmOwnerDefault m_SpellObject;

		// Token: 0x04005688 RID: 22152
		public FsmFloat m_Delay = 0f;

		// Token: 0x04005689 RID: 22153
		public FsmString m_EventName = string.Empty;

		// Token: 0x0400568A RID: 22154
		public FsmObject m_EventData;
	}
}
