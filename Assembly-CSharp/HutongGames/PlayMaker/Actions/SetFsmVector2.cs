using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CB5 RID: 3253
	[ActionCategory(12)]
	[Tooltip("Set the value of a Vector2 Variable in another FSM.")]
	public class SetFsmVector2 : FsmStateAction
	{
		// Token: 0x06006872 RID: 26738 RVA: 0x001EB5C6 File Offset: 0x001E97C6
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.setValue = null;
		}

		// Token: 0x06006873 RID: 26739 RVA: 0x001EB5E6 File Offset: 0x001E97E6
		public override void OnEnter()
		{
			this.DoSetFsmVector2();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006874 RID: 26740 RVA: 0x001EB600 File Offset: 0x001E9800
		private void DoSetFsmVector2()
		{
			if (this.setValue == null)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (ownerDefaultTarget != this.goLastFrame)
			{
				this.goLastFrame = ownerDefaultTarget;
				this.fsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
			}
			if (this.fsm == null)
			{
				this.LogWarning("Could not find FSM: " + this.fsmName.Value);
				return;
			}
			FsmVector2 fsmVector = this.fsm.FsmVariables.GetFsmVector2(this.variableName.Value);
			if (fsmVector != null)
			{
				fsmVector.Value = this.setValue.Value;
			}
			else
			{
				this.LogWarning("Could not find variable: " + this.variableName.Value);
			}
		}

		// Token: 0x06006875 RID: 26741 RVA: 0x001EB6E7 File Offset: 0x001E98E7
		public override void OnUpdate()
		{
			this.DoSetFsmVector2();
		}

		// Token: 0x0400504E RID: 20558
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400504F RID: 20559
		[Tooltip("Optional name of FSM on Game Object")]
		[UIHint(15)]
		public FsmString fsmName;

		// Token: 0x04005050 RID: 20560
		[RequiredField]
		[Tooltip("The name of the FSM variable.")]
		[UIHint(29)]
		public FsmString variableName;

		// Token: 0x04005051 RID: 20561
		[RequiredField]
		[Tooltip("Set the value of the variable.")]
		public FsmVector2 setValue;

		// Token: 0x04005052 RID: 20562
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x04005053 RID: 20563
		private GameObject goLastFrame;

		// Token: 0x04005054 RID: 20564
		private PlayMakerFSM fsm;
	}
}
