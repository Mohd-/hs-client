using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CA9 RID: 3241
	[Tooltip("Set the value of a Bool Variable in another FSM.")]
	[ActionCategory(12)]
	public class SetFsmBool : FsmStateAction
	{
		// Token: 0x06006835 RID: 26677 RVA: 0x001EA6B3 File Offset: 0x001E88B3
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.setValue = null;
		}

		// Token: 0x06006836 RID: 26678 RVA: 0x001EA6D3 File Offset: 0x001E88D3
		public override void OnEnter()
		{
			this.DoSetFsmBool();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006837 RID: 26679 RVA: 0x001EA6EC File Offset: 0x001E88EC
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
			FsmBool fsmBool = this.fsm.FsmVariables.FindFsmBool(this.variableName.Value);
			if (fsmBool != null)
			{
				fsmBool.Value = this.setValue.Value;
			}
			else
			{
				this.LogWarning("Could not find variable: " + this.variableName.Value);
			}
		}

		// Token: 0x06006838 RID: 26680 RVA: 0x001EA7D3 File Offset: 0x001E89D3
		public override void OnUpdate()
		{
			this.DoSetFsmBool();
		}

		// Token: 0x04004FF8 RID: 20472
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004FF9 RID: 20473
		[UIHint(15)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04004FFA RID: 20474
		[RequiredField]
		[UIHint(19)]
		[Tooltip("The name of the FSM variable.")]
		public FsmString variableName;

		// Token: 0x04004FFB RID: 20475
		[Tooltip("Set the value of the variable.")]
		[RequiredField]
		public FsmBool setValue;

		// Token: 0x04004FFC RID: 20476
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x04004FFD RID: 20477
		private GameObject goLastFrame;

		// Token: 0x04004FFE RID: 20478
		private PlayMakerFSM fsm;
	}
}
