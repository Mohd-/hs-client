using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C04 RID: 3076
	[ActionCategory(12)]
	[Tooltip("Get the value of an Object Variable from another FSM.")]
	public class GetFsmObject : FsmStateAction
	{
		// Token: 0x06006567 RID: 25959 RVA: 0x001E19EB File Offset: 0x001DFBEB
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.variableName = string.Empty;
			this.storeValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006568 RID: 25960 RVA: 0x001E1A22 File Offset: 0x001DFC22
		public override void OnEnter()
		{
			this.DoGetFsmVariable();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006569 RID: 25961 RVA: 0x001E1A3B File Offset: 0x001DFC3B
		public override void OnUpdate()
		{
			this.DoGetFsmVariable();
		}

		// Token: 0x0600656A RID: 25962 RVA: 0x001E1A44 File Offset: 0x001DFC44
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
			FsmObject fsmObject = this.fsm.FsmVariables.GetFsmObject(this.variableName.Value);
			if (fsmObject != null)
			{
				this.storeValue.Value = fsmObject.Value;
			}
		}

		// Token: 0x04004D23 RID: 19747
		[Tooltip("The GameObject that owns the FSM.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004D24 RID: 19748
		[UIHint(15)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04004D25 RID: 19749
		[UIHint(28)]
		[RequiredField]
		public FsmString variableName;

		// Token: 0x04004D26 RID: 19750
		[RequiredField]
		[UIHint(10)]
		public FsmObject storeValue;

		// Token: 0x04004D27 RID: 19751
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04004D28 RID: 19752
		private GameObject goLastFrame;

		// Token: 0x04004D29 RID: 19753
		protected PlayMakerFSM fsm;
	}
}
