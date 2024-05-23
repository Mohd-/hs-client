using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CAB RID: 3243
	[Tooltip("Set the value of a Float Variable in another FSM.")]
	[ActionCategory(12)]
	public class SetFsmFloat : FsmStateAction
	{
		// Token: 0x0600683F RID: 26687 RVA: 0x001EA913 File Offset: 0x001E8B13
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.setValue = null;
		}

		// Token: 0x06006840 RID: 26688 RVA: 0x001EA933 File Offset: 0x001E8B33
		public override void OnEnter()
		{
			this.DoSetFsmFloat();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006841 RID: 26689 RVA: 0x001EA94C File Offset: 0x001E8B4C
		private void DoSetFsmFloat()
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
			FsmFloat fsmFloat = this.fsm.FsmVariables.GetFsmFloat(this.variableName.Value);
			if (fsmFloat != null)
			{
				fsmFloat.Value = this.setValue.Value;
			}
			else
			{
				this.LogWarning("Could not find variable: " + this.variableName.Value);
			}
		}

		// Token: 0x06006842 RID: 26690 RVA: 0x001EAA33 File Offset: 0x001E8C33
		public override void OnUpdate()
		{
			this.DoSetFsmFloat();
		}

		// Token: 0x04005006 RID: 20486
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005007 RID: 20487
		[Tooltip("Optional name of FSM on Game Object")]
		[UIHint(15)]
		public FsmString fsmName;

		// Token: 0x04005008 RID: 20488
		[Tooltip("The name of the FSM variable.")]
		[RequiredField]
		[UIHint(17)]
		public FsmString variableName;

		// Token: 0x04005009 RID: 20489
		[RequiredField]
		[Tooltip("Set the value of the variable.")]
		public FsmFloat setValue;

		// Token: 0x0400500A RID: 20490
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x0400500B RID: 20491
		private GameObject goLastFrame;

		// Token: 0x0400500C RID: 20492
		private PlayMakerFSM fsm;
	}
}
