using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C05 RID: 3077
	[Tooltip("Get the value of a Quaternion Variable from another FSM.")]
	[ActionCategory(12)]
	public class GetFsmQuaternion : FsmStateAction
	{
		// Token: 0x0600656C RID: 25964 RVA: 0x001E1AF7 File Offset: 0x001DFCF7
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.variableName = string.Empty;
			this.storeValue = null;
			this.everyFrame = false;
		}

		// Token: 0x0600656D RID: 25965 RVA: 0x001E1B2E File Offset: 0x001DFD2E
		public override void OnEnter()
		{
			this.DoGetFsmVariable();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600656E RID: 25966 RVA: 0x001E1B47 File Offset: 0x001DFD47
		public override void OnUpdate()
		{
			this.DoGetFsmVariable();
		}

		// Token: 0x0600656F RID: 25967 RVA: 0x001E1B50 File Offset: 0x001DFD50
		private void DoGetFsmVariable()
		{
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
			if (this.fsm == null || this.storeValue == null)
			{
				return;
			}
			FsmQuaternion fsmQuaternion = this.fsm.FsmVariables.GetFsmQuaternion(this.variableName.Value);
			if (fsmQuaternion != null)
			{
				this.storeValue.Value = fsmQuaternion.Value;
			}
		}

		// Token: 0x04004D2A RID: 19754
		[Tooltip("The GameObject that owns the FSM.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004D2B RID: 19755
		[Tooltip("Optional name of FSM on Game Object")]
		[UIHint(15)]
		public FsmString fsmName;

		// Token: 0x04004D2C RID: 19756
		[UIHint(27)]
		[RequiredField]
		public FsmString variableName;

		// Token: 0x04004D2D RID: 19757
		[UIHint(10)]
		[RequiredField]
		public FsmQuaternion storeValue;

		// Token: 0x04004D2E RID: 19758
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04004D2F RID: 19759
		private GameObject goLastFrame;

		// Token: 0x04004D30 RID: 19760
		protected PlayMakerFSM fsm;
	}
}
