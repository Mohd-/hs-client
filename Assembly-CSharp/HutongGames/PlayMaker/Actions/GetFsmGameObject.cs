using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C01 RID: 3073
	[ActionCategory(12)]
	[Tooltip("Get the value of a Game Object Variable from another FSM.")]
	public class GetFsmGameObject : FsmStateAction
	{
		// Token: 0x06006558 RID: 25944 RVA: 0x001E16EE File Offset: 0x001DF8EE
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.storeValue = null;
		}

		// Token: 0x06006559 RID: 25945 RVA: 0x001E170E File Offset: 0x001DF90E
		public override void OnEnter()
		{
			this.DoGetFsmGameObject();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600655A RID: 25946 RVA: 0x001E1727 File Offset: 0x001DF927
		public override void OnUpdate()
		{
			this.DoGetFsmGameObject();
		}

		// Token: 0x0600655B RID: 25947 RVA: 0x001E1730 File Offset: 0x001DF930
		private void DoGetFsmGameObject()
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
			FsmGameObject fsmGameObject = this.fsm.FsmVariables.GetFsmGameObject(this.variableName.Value);
			if (fsmGameObject == null)
			{
				return;
			}
			this.storeValue.Value = fsmGameObject.Value;
		}

		// Token: 0x04004D0E RID: 19726
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004D0F RID: 19727
		[Tooltip("Optional name of FSM on Game Object")]
		[UIHint(15)]
		public FsmString fsmName;

		// Token: 0x04004D10 RID: 19728
		[UIHint(22)]
		[RequiredField]
		public FsmString variableName;

		// Token: 0x04004D11 RID: 19729
		[RequiredField]
		[UIHint(10)]
		public FsmGameObject storeValue;

		// Token: 0x04004D12 RID: 19730
		public bool everyFrame;

		// Token: 0x04004D13 RID: 19731
		private GameObject goLastFrame;

		// Token: 0x04004D14 RID: 19732
		private PlayMakerFSM fsm;
	}
}
