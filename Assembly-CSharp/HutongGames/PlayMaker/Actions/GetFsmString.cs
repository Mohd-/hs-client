using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C08 RID: 3080
	[Tooltip("Get the value of a String Variable from another FSM.")]
	[ActionCategory(12)]
	public class GetFsmString : FsmStateAction
	{
		// Token: 0x0600657B RID: 25979 RVA: 0x001E1E21 File Offset: 0x001E0021
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.storeValue = null;
		}

		// Token: 0x0600657C RID: 25980 RVA: 0x001E1E41 File Offset: 0x001E0041
		public override void OnEnter()
		{
			this.DoGetFsmString();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600657D RID: 25981 RVA: 0x001E1E5A File Offset: 0x001E005A
		public override void OnUpdate()
		{
			this.DoGetFsmString();
		}

		// Token: 0x0600657E RID: 25982 RVA: 0x001E1E64 File Offset: 0x001E0064
		private void DoGetFsmString()
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
			FsmString fsmString = this.fsm.FsmVariables.GetFsmString(this.variableName.Value);
			if (fsmString == null)
			{
				return;
			}
			this.storeValue.Value = fsmString.Value;
		}

		// Token: 0x04004D3E RID: 19774
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004D3F RID: 19775
		[UIHint(15)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04004D40 RID: 19776
		[RequiredField]
		[UIHint(20)]
		public FsmString variableName;

		// Token: 0x04004D41 RID: 19777
		[UIHint(10)]
		[RequiredField]
		public FsmString storeValue;

		// Token: 0x04004D42 RID: 19778
		public bool everyFrame;

		// Token: 0x04004D43 RID: 19779
		private GameObject goLastFrame;

		// Token: 0x04004D44 RID: 19780
		private PlayMakerFSM fsm;
	}
}
