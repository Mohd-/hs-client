using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CB2 RID: 3250
	[Tooltip("Set the value of a String Variable in another FSM.")]
	[ActionCategory(12)]
	public class SetFsmString : FsmStateAction
	{
		// Token: 0x06006862 RID: 26722 RVA: 0x001EB1B7 File Offset: 0x001E93B7
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.setValue = null;
		}

		// Token: 0x06006863 RID: 26723 RVA: 0x001EB1D7 File Offset: 0x001E93D7
		public override void OnEnter()
		{
			this.DoSetFsmString();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006864 RID: 26724 RVA: 0x001EB1F0 File Offset: 0x001E93F0
		private void DoSetFsmString()
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
			FsmString fsmString = this.fsm.FsmVariables.GetFsmString(this.variableName.Value);
			if (fsmString != null)
			{
				fsmString.Value = this.setValue.Value;
			}
			else
			{
				this.LogWarning("Could not find variable: " + this.variableName.Value);
			}
		}

		// Token: 0x06006865 RID: 26725 RVA: 0x001EB2D7 File Offset: 0x001E94D7
		public override void OnUpdate()
		{
			this.DoSetFsmString();
		}

		// Token: 0x04005037 RID: 20535
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005038 RID: 20536
		[UIHint(15)]
		[Tooltip("Optional name of FSM on Game Object.")]
		public FsmString fsmName;

		// Token: 0x04005039 RID: 20537
		[RequiredField]
		[Tooltip("The name of the FSM variable.")]
		[UIHint(20)]
		public FsmString variableName;

		// Token: 0x0400503A RID: 20538
		[Tooltip("Set the value of the variable.")]
		public FsmString setValue;

		// Token: 0x0400503B RID: 20539
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x0400503C RID: 20540
		private GameObject goLastFrame;

		// Token: 0x0400503D RID: 20541
		private PlayMakerFSM fsm;
	}
}
