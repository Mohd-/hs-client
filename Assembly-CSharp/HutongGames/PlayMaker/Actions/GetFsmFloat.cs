using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C00 RID: 3072
	[ActionCategory(12)]
	[Tooltip("Get the value of a Float Variable from another FSM.")]
	public class GetFsmFloat : FsmStateAction
	{
		// Token: 0x06006553 RID: 25939 RVA: 0x001E15F1 File Offset: 0x001DF7F1
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.storeValue = null;
		}

		// Token: 0x06006554 RID: 25940 RVA: 0x001E1611 File Offset: 0x001DF811
		public override void OnEnter()
		{
			this.DoGetFsmFloat();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006555 RID: 25941 RVA: 0x001E162A File Offset: 0x001DF82A
		public override void OnUpdate()
		{
			this.DoGetFsmFloat();
		}

		// Token: 0x06006556 RID: 25942 RVA: 0x001E1634 File Offset: 0x001DF834
		private void DoGetFsmFloat()
		{
			if (this.storeValue.IsNone)
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
				this.fsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
				this.goLastFrame = ownerDefaultTarget;
			}
			if (this.fsm == null)
			{
				return;
			}
			FsmFloat fsmFloat = this.fsm.FsmVariables.GetFsmFloat(this.variableName.Value);
			if (fsmFloat == null)
			{
				return;
			}
			this.storeValue.Value = fsmFloat.Value;
		}

		// Token: 0x04004D07 RID: 19719
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004D08 RID: 19720
		[UIHint(15)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04004D09 RID: 19721
		[UIHint(17)]
		[RequiredField]
		public FsmString variableName;

		// Token: 0x04004D0A RID: 19722
		[RequiredField]
		[UIHint(10)]
		public FsmFloat storeValue;

		// Token: 0x04004D0B RID: 19723
		public bool everyFrame;

		// Token: 0x04004D0C RID: 19724
		private GameObject goLastFrame;

		// Token: 0x04004D0D RID: 19725
		private PlayMakerFSM fsm;
	}
}
