using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CB1 RID: 3249
	[ActionCategory(12)]
	[Tooltip("Set the value of a Rect Variable in another FSM.")]
	public class SetFsmRect : FsmStateAction
	{
		// Token: 0x0600685D RID: 26717 RVA: 0x001EB06F File Offset: 0x001E926F
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.variableName = string.Empty;
			this.setValue = null;
			this.everyFrame = false;
		}

		// Token: 0x0600685E RID: 26718 RVA: 0x001EB0A6 File Offset: 0x001E92A6
		public override void OnEnter()
		{
			this.DoSetFsmBool();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600685F RID: 26719 RVA: 0x001EB0C0 File Offset: 0x001E92C0
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
			FsmRect fsmRect = this.fsm.FsmVariables.GetFsmRect(this.variableName.Value);
			if (fsmRect != null)
			{
				fsmRect.Value = this.setValue.Value;
			}
			else
			{
				this.LogWarning("Could not find variable: " + this.variableName.Value);
			}
		}

		// Token: 0x06006860 RID: 26720 RVA: 0x001EB1A7 File Offset: 0x001E93A7
		public override void OnUpdate()
		{
			this.DoSetFsmBool();
		}

		// Token: 0x04005030 RID: 20528
		[Tooltip("The GameObject that owns the FSM.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005031 RID: 20529
		[UIHint(15)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04005032 RID: 20530
		[Tooltip("The name of the FSM variable.")]
		[RequiredField]
		[UIHint(24)]
		public FsmString variableName;

		// Token: 0x04005033 RID: 20531
		[Tooltip("Set the value of the variable.")]
		[RequiredField]
		public FsmRect setValue;

		// Token: 0x04005034 RID: 20532
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x04005035 RID: 20533
		private GameObject goLastFrame;

		// Token: 0x04005036 RID: 20534
		private PlayMakerFSM fsm;
	}
}
