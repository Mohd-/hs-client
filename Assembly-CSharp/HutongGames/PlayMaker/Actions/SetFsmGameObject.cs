using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CAC RID: 3244
	[ActionCategory(12)]
	[Tooltip("Set the value of a Game Object Variable in another FSM. Accept null reference")]
	public class SetFsmGameObject : FsmStateAction
	{
		// Token: 0x06006844 RID: 26692 RVA: 0x001EAA43 File Offset: 0x001E8C43
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.setValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006845 RID: 26693 RVA: 0x001EAA6A File Offset: 0x001E8C6A
		public override void OnEnter()
		{
			this.DoSetFsmGameObject();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006846 RID: 26694 RVA: 0x001EAA84 File Offset: 0x001E8C84
		private void DoSetFsmGameObject()
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
			if (this.fsm == null)
			{
				return;
			}
			FsmGameObject fsmGameObject = this.fsm.FsmVariables.FindFsmGameObject(this.variableName.Value);
			if (fsmGameObject != null)
			{
				fsmGameObject.Value = ((this.setValue != null) ? this.setValue.Value : null);
			}
			else
			{
				this.LogWarning("Could not find variable: " + this.variableName.Value);
			}
		}

		// Token: 0x06006847 RID: 26695 RVA: 0x001EAB55 File Offset: 0x001E8D55
		public override void OnUpdate()
		{
			this.DoSetFsmGameObject();
		}

		// Token: 0x0400500D RID: 20493
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400500E RID: 20494
		[UIHint(15)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x0400500F RID: 20495
		[RequiredField]
		[UIHint(22)]
		[Tooltip("The name of the FSM variable.")]
		public FsmString variableName;

		// Token: 0x04005010 RID: 20496
		[Tooltip("Set the value of the variable.")]
		public FsmGameObject setValue;

		// Token: 0x04005011 RID: 20497
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x04005012 RID: 20498
		private GameObject goLastFrame;

		// Token: 0x04005013 RID: 20499
		private PlayMakerFSM fsm;
	}
}
