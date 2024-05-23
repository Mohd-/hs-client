using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BFE RID: 3070
	[Tooltip("Get the value of a Bool Variable from another FSM.")]
	[ActionCategory(12)]
	public class GetFsmBool : FsmStateAction
	{
		// Token: 0x06006549 RID: 25929 RVA: 0x001E1400 File Offset: 0x001DF600
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.storeValue = null;
		}

		// Token: 0x0600654A RID: 25930 RVA: 0x001E1420 File Offset: 0x001DF620
		public override void OnEnter()
		{
			this.DoGetFsmBool();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600654B RID: 25931 RVA: 0x001E1439 File Offset: 0x001DF639
		public override void OnUpdate()
		{
			this.DoGetFsmBool();
		}

		// Token: 0x0600654C RID: 25932 RVA: 0x001E1444 File Offset: 0x001DF644
		private void DoGetFsmBool()
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
			FsmBool fsmBool = this.fsm.FsmVariables.GetFsmBool(this.variableName.Value);
			if (fsmBool == null)
			{
				return;
			}
			this.storeValue.Value = fsmBool.Value;
		}

		// Token: 0x04004CF9 RID: 19705
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004CFA RID: 19706
		[Tooltip("Optional name of FSM on Game Object")]
		[UIHint(15)]
		public FsmString fsmName;

		// Token: 0x04004CFB RID: 19707
		[RequiredField]
		[UIHint(19)]
		public FsmString variableName;

		// Token: 0x04004CFC RID: 19708
		[UIHint(10)]
		[RequiredField]
		public FsmBool storeValue;

		// Token: 0x04004CFD RID: 19709
		public bool everyFrame;

		// Token: 0x04004CFE RID: 19710
		private GameObject goLastFrame;

		// Token: 0x04004CFF RID: 19711
		private PlayMakerFSM fsm;
	}
}
