using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C09 RID: 3081
	[ActionCategory(12)]
	[Tooltip("Get the value of a Texture Variable from another FSM.")]
	public class GetFsmTexture : FsmStateAction
	{
		// Token: 0x06006580 RID: 25984 RVA: 0x001E1F19 File Offset: 0x001E0119
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.variableName = string.Empty;
			this.storeValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006581 RID: 25985 RVA: 0x001E1F50 File Offset: 0x001E0150
		public override void OnEnter()
		{
			this.DoGetFsmVariable();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006582 RID: 25986 RVA: 0x001E1F69 File Offset: 0x001E0169
		public override void OnUpdate()
		{
			this.DoGetFsmVariable();
		}

		// Token: 0x06006583 RID: 25987 RVA: 0x001E1F74 File Offset: 0x001E0174
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
			FsmTexture fsmTexture = this.fsm.FsmVariables.GetFsmTexture(this.variableName.Value);
			if (fsmTexture != null)
			{
				this.storeValue.Value = fsmTexture.Value;
			}
		}

		// Token: 0x04004D45 RID: 19781
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004D46 RID: 19782
		[UIHint(15)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04004D47 RID: 19783
		[UIHint(26)]
		[RequiredField]
		public FsmString variableName;

		// Token: 0x04004D48 RID: 19784
		[UIHint(10)]
		[RequiredField]
		public FsmTexture storeValue;

		// Token: 0x04004D49 RID: 19785
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04004D4A RID: 19786
		private GameObject goLastFrame;

		// Token: 0x04004D4B RID: 19787
		protected PlayMakerFSM fsm;
	}
}
