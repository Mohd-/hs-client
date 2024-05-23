using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B9C RID: 2972
	[Tooltip("Enables/Disables a Behaviour on a GameObject. Optionally reset the Behaviour on exit - useful if you only want the Behaviour to be active while this state is active.")]
	[ActionCategory(11)]
	public class EnableBehaviour : FsmStateAction
	{
		// Token: 0x060063D9 RID: 25561 RVA: 0x001DC19F File Offset: 0x001DA39F
		public override void Reset()
		{
			this.gameObject = null;
			this.behaviour = null;
			this.component = null;
			this.enable = true;
			this.resetOnExit = true;
		}

		// Token: 0x060063DA RID: 25562 RVA: 0x001DC1CE File Offset: 0x001DA3CE
		public override void OnEnter()
		{
			this.DoEnableBehaviour(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			base.Finish();
		}

		// Token: 0x060063DB RID: 25563 RVA: 0x001DC1F0 File Offset: 0x001DA3F0
		private void DoEnableBehaviour(GameObject go)
		{
			if (go == null)
			{
				return;
			}
			if (this.component != null)
			{
				this.componentTarget = (this.component as Behaviour);
			}
			else
			{
				this.componentTarget = (go.GetComponent(this.behaviour.Value) as Behaviour);
			}
			if (this.componentTarget == null)
			{
				this.LogWarning(" " + go.name + " missing behaviour: " + this.behaviour.Value);
				return;
			}
			this.componentTarget.enabled = this.enable.Value;
		}

		// Token: 0x060063DC RID: 25564 RVA: 0x001DC29C File Offset: 0x001DA49C
		public override void OnExit()
		{
			if (this.componentTarget == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.componentTarget.enabled = !this.enable.Value;
			}
		}

		// Token: 0x060063DD RID: 25565 RVA: 0x001DC2E4 File Offset: 0x001DA4E4
		public override string ErrorCheck()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null || this.component != null || this.behaviour.IsNone || string.IsNullOrEmpty(this.behaviour.Value))
			{
				return null;
			}
			Behaviour behaviour = ownerDefaultTarget.GetComponent(this.behaviour.Value) as Behaviour;
			return (!(behaviour != null)) ? "Behaviour missing" : null;
		}

		// Token: 0x04004B4E RID: 19278
		[Tooltip("The GameObject that owns the Behaviour.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004B4F RID: 19279
		[Tooltip("The name of the Behaviour to enable/disable.")]
		[UIHint(2)]
		public FsmString behaviour;

		// Token: 0x04004B50 RID: 19280
		[Tooltip("Optionally drag a component directly into this field (behavior name will be ignored).")]
		public Component component;

		// Token: 0x04004B51 RID: 19281
		[RequiredField]
		[Tooltip("Set to True to enable, False to disable.")]
		public FsmBool enable;

		// Token: 0x04004B52 RID: 19282
		public FsmBool resetOnExit;

		// Token: 0x04004B53 RID: 19283
		private Behaviour componentTarget;
	}
}
