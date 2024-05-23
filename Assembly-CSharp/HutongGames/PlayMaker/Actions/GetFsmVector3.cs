using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C0D RID: 3085
	[ActionCategory(12)]
	[Tooltip("Get the value of a Vector3 Variable from another FSM.")]
	public class GetFsmVector3 : FsmStateAction
	{
		// Token: 0x06006596 RID: 26006 RVA: 0x001E2479 File Offset: 0x001E0679
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.storeValue = null;
		}

		// Token: 0x06006597 RID: 26007 RVA: 0x001E2499 File Offset: 0x001E0699
		public override void OnEnter()
		{
			this.DoGetFsmVector3();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006598 RID: 26008 RVA: 0x001E24B2 File Offset: 0x001E06B2
		public override void OnUpdate()
		{
			this.DoGetFsmVector3();
		}

		// Token: 0x06006599 RID: 26009 RVA: 0x001E24BC File Offset: 0x001E06BC
		private void DoGetFsmVector3()
		{
			if (this.storeValue == null)
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
				return;
			}
			FsmVector3 fsmVector = this.fsm.FsmVariables.GetFsmVector3(this.variableName.Value);
			if (fsmVector == null)
			{
				return;
			}
			this.storeValue.Value = fsmVector.Value;
		}

		// Token: 0x04004D63 RID: 19811
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004D64 RID: 19812
		[Tooltip("Optional name of FSM on Game Object")]
		[UIHint(15)]
		public FsmString fsmName;

		// Token: 0x04004D65 RID: 19813
		[RequiredField]
		[UIHint(21)]
		public FsmString variableName;

		// Token: 0x04004D66 RID: 19814
		[RequiredField]
		[UIHint(10)]
		public FsmVector3 storeValue;

		// Token: 0x04004D67 RID: 19815
		public bool everyFrame;

		// Token: 0x04004D68 RID: 19816
		private GameObject goLastFrame;

		// Token: 0x04004D69 RID: 19817
		private PlayMakerFSM fsm;
	}
}
