using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CB6 RID: 3254
	[ActionCategory(12)]
	[Tooltip("Set the value of a Vector3 Variable in another FSM.")]
	public class SetFsmVector3 : FsmStateAction
	{
		// Token: 0x06006877 RID: 26743 RVA: 0x001EB6F7 File Offset: 0x001E98F7
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.setValue = null;
		}

		// Token: 0x06006878 RID: 26744 RVA: 0x001EB717 File Offset: 0x001E9917
		public override void OnEnter()
		{
			this.DoSetFsmVector3();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006879 RID: 26745 RVA: 0x001EB730 File Offset: 0x001E9930
		private void DoSetFsmVector3()
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
			FsmVector3 fsmVector = this.fsm.FsmVariables.GetFsmVector3(this.variableName.Value);
			if (fsmVector != null)
			{
				fsmVector.Value = this.setValue.Value;
			}
			else
			{
				this.LogWarning("Could not find variable: " + this.variableName.Value);
			}
		}

		// Token: 0x0600687A RID: 26746 RVA: 0x001EB817 File Offset: 0x001E9A17
		public override void OnUpdate()
		{
			this.DoSetFsmVector3();
		}

		// Token: 0x04005055 RID: 20565
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005056 RID: 20566
		[Tooltip("Optional name of FSM on Game Object")]
		[UIHint(15)]
		public FsmString fsmName;

		// Token: 0x04005057 RID: 20567
		[RequiredField]
		[UIHint(21)]
		[Tooltip("The name of the FSM variable.")]
		public FsmString variableName;

		// Token: 0x04005058 RID: 20568
		[Tooltip("Set the value of the variable.")]
		[RequiredField]
		public FsmVector3 setValue;

		// Token: 0x04005059 RID: 20569
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x0400505A RID: 20570
		private GameObject goLastFrame;

		// Token: 0x0400505B RID: 20571
		private PlayMakerFSM fsm;
	}
}
