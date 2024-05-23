using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CB3 RID: 3251
	[Tooltip("Set the value of a Texture Variable in another FSM.")]
	[ActionCategory(12)]
	public class SetFsmTexture : FsmStateAction
	{
		// Token: 0x06006867 RID: 26727 RVA: 0x001EB2E7 File Offset: 0x001E94E7
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.variableName = string.Empty;
			this.setValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006868 RID: 26728 RVA: 0x001EB31E File Offset: 0x001E951E
		public override void OnEnter()
		{
			this.DoSetFsmBool();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006869 RID: 26729 RVA: 0x001EB338 File Offset: 0x001E9538
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
			FsmTexture fsmTexture = this.fsm.FsmVariables.FindFsmTexture(this.variableName.Value);
			if (fsmTexture != null)
			{
				fsmTexture.Value = this.setValue.Value;
			}
			else
			{
				this.LogWarning("Could not find variable: " + this.variableName.Value);
			}
		}

		// Token: 0x0600686A RID: 26730 RVA: 0x001EB41F File Offset: 0x001E961F
		public override void OnUpdate()
		{
			this.DoSetFsmBool();
		}

		// Token: 0x0400503E RID: 20542
		[Tooltip("The GameObject that owns the FSM.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400503F RID: 20543
		[UIHint(15)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04005040 RID: 20544
		[Tooltip("The name of the FSM variable.")]
		[RequiredField]
		[UIHint(26)]
		public FsmString variableName;

		// Token: 0x04005041 RID: 20545
		[Tooltip("Set the value of the variable.")]
		public FsmTexture setValue;

		// Token: 0x04005042 RID: 20546
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x04005043 RID: 20547
		private GameObject goLastFrame;

		// Token: 0x04005044 RID: 20548
		private PlayMakerFSM fsm;
	}
}
