using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CAA RID: 3242
	[ActionCategory(12)]
	[Tooltip("Set the value of a Color Variable in another FSM.")]
	public class SetFsmColor : FsmStateAction
	{
		// Token: 0x0600683A RID: 26682 RVA: 0x001EA7E3 File Offset: 0x001E89E3
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.setValue = null;
		}

		// Token: 0x0600683B RID: 26683 RVA: 0x001EA803 File Offset: 0x001E8A03
		public override void OnEnter()
		{
			this.DoSetFsmColor();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600683C RID: 26684 RVA: 0x001EA81C File Offset: 0x001E8A1C
		private void DoSetFsmColor()
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
			FsmColor fsmColor = this.fsm.FsmVariables.GetFsmColor(this.variableName.Value);
			if (fsmColor != null)
			{
				fsmColor.Value = this.setValue.Value;
			}
			else
			{
				this.LogWarning("Could not find variable: " + this.variableName.Value);
			}
		}

		// Token: 0x0600683D RID: 26685 RVA: 0x001EA903 File Offset: 0x001E8B03
		public override void OnUpdate()
		{
			this.DoSetFsmColor();
		}

		// Token: 0x04004FFF RID: 20479
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005000 RID: 20480
		[UIHint(15)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04005001 RID: 20481
		[Tooltip("The name of the FSM variable.")]
		[UIHint(23)]
		[RequiredField]
		public FsmString variableName;

		// Token: 0x04005002 RID: 20482
		[RequiredField]
		[Tooltip("Set the value of the variable.")]
		public FsmColor setValue;

		// Token: 0x04005003 RID: 20483
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x04005004 RID: 20484
		private GameObject goLastFrame;

		// Token: 0x04005005 RID: 20485
		private PlayMakerFSM fsm;
	}
}
