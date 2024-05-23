using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CAE RID: 3246
	[Tooltip("Set the value of a Material Variable in another FSM.")]
	[ActionCategory(12)]
	public class SetFsmMaterial : FsmStateAction
	{
		// Token: 0x0600684E RID: 26702 RVA: 0x001EAC97 File Offset: 0x001E8E97
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.variableName = string.Empty;
			this.setValue = null;
			this.everyFrame = false;
		}

		// Token: 0x0600684F RID: 26703 RVA: 0x001EACCE File Offset: 0x001E8ECE
		public override void OnEnter()
		{
			this.DoSetFsmBool();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006850 RID: 26704 RVA: 0x001EACE8 File Offset: 0x001E8EE8
		private void DoSetFsmBool()
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
			FsmMaterial fsmMaterial = this.fsm.FsmVariables.GetFsmMaterial(this.variableName.Value);
			if (fsmMaterial != null)
			{
				fsmMaterial.Value = this.setValue.Value;
			}
			else
			{
				this.LogWarning("Could not find variable: " + this.variableName.Value);
			}
		}

		// Token: 0x06006851 RID: 26705 RVA: 0x001EADCF File Offset: 0x001E8FCF
		public override void OnUpdate()
		{
			this.DoSetFsmBool();
		}

		// Token: 0x0400501B RID: 20507
		[Tooltip("The GameObject that owns the FSM.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400501C RID: 20508
		[Tooltip("Optional name of FSM on Game Object")]
		[UIHint(15)]
		public FsmString fsmName;

		// Token: 0x0400501D RID: 20509
		[RequiredField]
		[UIHint(25)]
		[Tooltip("The name of the FSM variable.")]
		public FsmString variableName;

		// Token: 0x0400501E RID: 20510
		[RequiredField]
		[Tooltip("Set the value of the variable.")]
		public FsmMaterial setValue;

		// Token: 0x0400501F RID: 20511
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x04005020 RID: 20512
		private GameObject goLastFrame;

		// Token: 0x04005021 RID: 20513
		private PlayMakerFSM fsm;
	}
}
