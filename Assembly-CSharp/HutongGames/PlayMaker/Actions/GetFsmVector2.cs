using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C0C RID: 3084
	[ActionCategory(12)]
	[Tooltip("Get the value of a Vector2 Variable from another FSM.")]
	public class GetFsmVector2 : FsmStateAction
	{
		// Token: 0x06006591 RID: 26001 RVA: 0x001E2382 File Offset: 0x001E0582
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.storeValue = null;
		}

		// Token: 0x06006592 RID: 26002 RVA: 0x001E23A2 File Offset: 0x001E05A2
		public override void OnEnter()
		{
			this.DoGetFsmVector2();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006593 RID: 26003 RVA: 0x001E23BB File Offset: 0x001E05BB
		public override void OnUpdate()
		{
			this.DoGetFsmVector2();
		}

		// Token: 0x06006594 RID: 26004 RVA: 0x001E23C4 File Offset: 0x001E05C4
		private void DoGetFsmVector2()
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
			FsmVector2 fsmVector = this.fsm.FsmVariables.GetFsmVector2(this.variableName.Value);
			if (fsmVector == null)
			{
				return;
			}
			this.storeValue.Value = fsmVector.Value;
		}

		// Token: 0x04004D5C RID: 19804
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004D5D RID: 19805
		[UIHint(15)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04004D5E RID: 19806
		[UIHint(29)]
		[RequiredField]
		public FsmString variableName;

		// Token: 0x04004D5F RID: 19807
		[UIHint(10)]
		[RequiredField]
		public FsmVector2 storeValue;

		// Token: 0x04004D60 RID: 19808
		public bool everyFrame;

		// Token: 0x04004D61 RID: 19809
		private GameObject goLastFrame;

		// Token: 0x04004D62 RID: 19810
		private PlayMakerFSM fsm;
	}
}
