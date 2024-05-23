using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CAD RID: 3245
	[Tooltip("Set the value of an Integer Variable in another FSM.")]
	[ActionCategory(12)]
	public class SetFsmInt : FsmStateAction
	{
		// Token: 0x06006849 RID: 26697 RVA: 0x001EAB65 File Offset: 0x001E8D65
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.setValue = null;
		}

		// Token: 0x0600684A RID: 26698 RVA: 0x001EAB85 File Offset: 0x001E8D85
		public override void OnEnter()
		{
			this.DoSetFsmInt();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600684B RID: 26699 RVA: 0x001EABA0 File Offset: 0x001E8DA0
		private void DoSetFsmInt()
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
			FsmInt fsmInt = this.fsm.FsmVariables.GetFsmInt(this.variableName.Value);
			if (fsmInt != null)
			{
				fsmInt.Value = this.setValue.Value;
			}
			else
			{
				this.LogWarning("Could not find variable: " + this.variableName.Value);
			}
		}

		// Token: 0x0600684C RID: 26700 RVA: 0x001EAC87 File Offset: 0x001E8E87
		public override void OnUpdate()
		{
			this.DoSetFsmInt();
		}

		// Token: 0x04005014 RID: 20500
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005015 RID: 20501
		[UIHint(15)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04005016 RID: 20502
		[Tooltip("The name of the FSM variable.")]
		[RequiredField]
		[UIHint(18)]
		public FsmString variableName;

		// Token: 0x04005017 RID: 20503
		[RequiredField]
		[Tooltip("Set the value of the variable.")]
		public FsmInt setValue;

		// Token: 0x04005018 RID: 20504
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x04005019 RID: 20505
		private GameObject goLastFrame;

		// Token: 0x0400501A RID: 20506
		private PlayMakerFSM fsm;
	}
}
