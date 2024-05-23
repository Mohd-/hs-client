using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CAF RID: 3247
	[ActionCategory(12)]
	[Tooltip("Set the value of an Object Variable in another FSM.")]
	public class SetFsmObject : FsmStateAction
	{
		// Token: 0x06006853 RID: 26707 RVA: 0x001EADDF File Offset: 0x001E8FDF
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.variableName = string.Empty;
			this.setValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006854 RID: 26708 RVA: 0x001EAE16 File Offset: 0x001E9016
		public override void OnEnter()
		{
			this.DoSetFsmBool();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006855 RID: 26709 RVA: 0x001EAE30 File Offset: 0x001E9030
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
			FsmObject fsmObject = this.fsm.FsmVariables.GetFsmObject(this.variableName.Value);
			if (fsmObject != null)
			{
				fsmObject.Value = this.setValue.Value;
			}
			else
			{
				this.LogWarning("Could not find variable: " + this.variableName.Value);
			}
		}

		// Token: 0x06006856 RID: 26710 RVA: 0x001EAF17 File Offset: 0x001E9117
		public override void OnUpdate()
		{
			this.DoSetFsmBool();
		}

		// Token: 0x04005022 RID: 20514
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005023 RID: 20515
		[UIHint(15)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04005024 RID: 20516
		[RequiredField]
		[UIHint(28)]
		[Tooltip("The name of the FSM variable.")]
		public FsmString variableName;

		// Token: 0x04005025 RID: 20517
		[Tooltip("Set the value of the variable.")]
		public FsmObject setValue;

		// Token: 0x04005026 RID: 20518
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x04005027 RID: 20519
		private GameObject goLastFrame;

		// Token: 0x04005028 RID: 20520
		private PlayMakerFSM fsm;
	}
}
