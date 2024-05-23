using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C03 RID: 3075
	[ActionCategory(12)]
	[Tooltip("Get the value of a Material Variable from another FSM.")]
	public class GetFsmMaterial : FsmStateAction
	{
		// Token: 0x06006562 RID: 25954 RVA: 0x001E18DD File Offset: 0x001DFADD
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.variableName = string.Empty;
			this.storeValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006563 RID: 25955 RVA: 0x001E1914 File Offset: 0x001DFB14
		public override void OnEnter()
		{
			this.DoGetFsmVariable();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006564 RID: 25956 RVA: 0x001E192D File Offset: 0x001DFB2D
		public override void OnUpdate()
		{
			this.DoGetFsmVariable();
		}

		// Token: 0x06006565 RID: 25957 RVA: 0x001E1938 File Offset: 0x001DFB38
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
			FsmMaterial fsmMaterial = this.fsm.FsmVariables.GetFsmMaterial(this.variableName.Value);
			if (fsmMaterial != null)
			{
				this.storeValue.Value = fsmMaterial.Value;
			}
		}

		// Token: 0x04004D1C RID: 19740
		[Tooltip("The GameObject that owns the FSM.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004D1D RID: 19741
		[Tooltip("Optional name of FSM on Game Object")]
		[UIHint(15)]
		public FsmString fsmName;

		// Token: 0x04004D1E RID: 19742
		[RequiredField]
		[UIHint(25)]
		public FsmString variableName;

		// Token: 0x04004D1F RID: 19743
		[UIHint(10)]
		[RequiredField]
		public FsmMaterial storeValue;

		// Token: 0x04004D20 RID: 19744
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04004D21 RID: 19745
		private GameObject goLastFrame;

		// Token: 0x04004D22 RID: 19746
		protected PlayMakerFSM fsm;
	}
}
