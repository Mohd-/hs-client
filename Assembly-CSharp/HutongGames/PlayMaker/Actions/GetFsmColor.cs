using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BFF RID: 3071
	[ActionCategory(12)]
	[Tooltip("Get the value of a Color Variable from another FSM.")]
	public class GetFsmColor : FsmStateAction
	{
		// Token: 0x0600654E RID: 25934 RVA: 0x001E14F9 File Offset: 0x001DF6F9
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.storeValue = null;
		}

		// Token: 0x0600654F RID: 25935 RVA: 0x001E1519 File Offset: 0x001DF719
		public override void OnEnter()
		{
			this.DoGetFsmColor();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006550 RID: 25936 RVA: 0x001E1532 File Offset: 0x001DF732
		public override void OnUpdate()
		{
			this.DoGetFsmColor();
		}

		// Token: 0x06006551 RID: 25937 RVA: 0x001E153C File Offset: 0x001DF73C
		private void DoGetFsmColor()
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
			FsmColor fsmColor = this.fsm.FsmVariables.GetFsmColor(this.variableName.Value);
			if (fsmColor == null)
			{
				return;
			}
			this.storeValue.Value = fsmColor.Value;
		}

		// Token: 0x04004D00 RID: 19712
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004D01 RID: 19713
		[UIHint(15)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04004D02 RID: 19714
		[UIHint(23)]
		[RequiredField]
		public FsmString variableName;

		// Token: 0x04004D03 RID: 19715
		[UIHint(10)]
		[RequiredField]
		public FsmColor storeValue;

		// Token: 0x04004D04 RID: 19716
		public bool everyFrame;

		// Token: 0x04004D05 RID: 19717
		private GameObject goLastFrame;

		// Token: 0x04004D06 RID: 19718
		private PlayMakerFSM fsm;
	}
}
