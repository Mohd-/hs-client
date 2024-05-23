using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CB0 RID: 3248
	[ActionCategory(12)]
	[Tooltip("Set the value of a Quaternion Variable in another FSM.")]
	public class SetFsmQuaternion : FsmStateAction
	{
		// Token: 0x06006858 RID: 26712 RVA: 0x001EAF27 File Offset: 0x001E9127
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.variableName = string.Empty;
			this.setValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006859 RID: 26713 RVA: 0x001EAF5E File Offset: 0x001E915E
		public override void OnEnter()
		{
			this.DoSetFsmQuaternion();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600685A RID: 26714 RVA: 0x001EAF78 File Offset: 0x001E9178
		private void DoSetFsmQuaternion()
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
			FsmQuaternion fsmQuaternion = this.fsm.FsmVariables.GetFsmQuaternion(this.variableName.Value);
			if (fsmQuaternion != null)
			{
				fsmQuaternion.Value = this.setValue.Value;
			}
			else
			{
				this.LogWarning("Could not find variable: " + this.variableName.Value);
			}
		}

		// Token: 0x0600685B RID: 26715 RVA: 0x001EB05F File Offset: 0x001E925F
		public override void OnUpdate()
		{
			this.DoSetFsmQuaternion();
		}

		// Token: 0x04005029 RID: 20521
		[Tooltip("The GameObject that owns the FSM.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400502A RID: 20522
		[UIHint(15)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x0400502B RID: 20523
		[UIHint(27)]
		[RequiredField]
		[Tooltip("The name of the FSM variable.")]
		public FsmString variableName;

		// Token: 0x0400502C RID: 20524
		[RequiredField]
		[Tooltip("Set the value of the variable.")]
		public FsmQuaternion setValue;

		// Token: 0x0400502D RID: 20525
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x0400502E RID: 20526
		private GameObject goLastFrame;

		// Token: 0x0400502F RID: 20527
		private PlayMakerFSM fsm;
	}
}
