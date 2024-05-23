using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C02 RID: 3074
	[Tooltip("Get the value of an Integer Variable from another FSM.")]
	[ActionCategory(12)]
	public class GetFsmInt : FsmStateAction
	{
		// Token: 0x0600655D RID: 25949 RVA: 0x001E17E5 File Offset: 0x001DF9E5
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.storeValue = null;
		}

		// Token: 0x0600655E RID: 25950 RVA: 0x001E1805 File Offset: 0x001DFA05
		public override void OnEnter()
		{
			this.DoGetFsmInt();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600655F RID: 25951 RVA: 0x001E181E File Offset: 0x001DFA1E
		public override void OnUpdate()
		{
			this.DoGetFsmInt();
		}

		// Token: 0x06006560 RID: 25952 RVA: 0x001E1828 File Offset: 0x001DFA28
		private void DoGetFsmInt()
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
			FsmInt fsmInt = this.fsm.FsmVariables.GetFsmInt(this.variableName.Value);
			if (fsmInt == null)
			{
				return;
			}
			this.storeValue.Value = fsmInt.Value;
		}

		// Token: 0x04004D15 RID: 19733
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004D16 RID: 19734
		[Tooltip("Optional name of FSM on Game Object")]
		[UIHint(15)]
		public FsmString fsmName;

		// Token: 0x04004D17 RID: 19735
		[UIHint(18)]
		[RequiredField]
		public FsmString variableName;

		// Token: 0x04004D18 RID: 19736
		[UIHint(10)]
		[RequiredField]
		public FsmInt storeValue;

		// Token: 0x04004D19 RID: 19737
		public bool everyFrame;

		// Token: 0x04004D1A RID: 19738
		private GameObject goLastFrame;

		// Token: 0x04004D1B RID: 19739
		private PlayMakerFSM fsm;
	}
}
