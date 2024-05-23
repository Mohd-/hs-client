using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C07 RID: 3079
	[Tooltip("Gets the name of the specified FSMs current state. Either reference the fsm component directly, or find it on a game object.")]
	[ActionCategory(12)]
	public class GetFsmState : FsmStateAction
	{
		// Token: 0x06006576 RID: 25974 RVA: 0x001E1D10 File Offset: 0x001DFF10
		public override void Reset()
		{
			this.fsmComponent = null;
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006577 RID: 25975 RVA: 0x001E1D49 File Offset: 0x001DFF49
		public override void OnEnter()
		{
			this.DoGetFsmState();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006578 RID: 25976 RVA: 0x001E1D62 File Offset: 0x001DFF62
		public override void OnUpdate()
		{
			this.DoGetFsmState();
		}

		// Token: 0x06006579 RID: 25977 RVA: 0x001E1D6C File Offset: 0x001DFF6C
		private void DoGetFsmState()
		{
			if (this.fsm == null)
			{
				if (this.fsmComponent != null)
				{
					this.fsm = this.fsmComponent;
				}
				else
				{
					GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
					if (ownerDefaultTarget != null)
					{
						this.fsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
					}
				}
				if (this.fsm == null)
				{
					this.storeResult.Value = string.Empty;
					return;
				}
			}
			this.storeResult.Value = this.fsm.ActiveStateName;
		}

		// Token: 0x04004D38 RID: 19768
		[Tooltip("Drag a PlayMakerFSM component here.")]
		public PlayMakerFSM fsmComponent;

		// Token: 0x04004D39 RID: 19769
		[Tooltip("If not specifyng the component above, specify the GameObject that owns the FSM")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004D3A RID: 19770
		[Tooltip("Optional name of Fsm on Game Object. If left blank it will find the first PlayMakerFSM on the GameObject.")]
		[UIHint(15)]
		public FsmString fsmName;

		// Token: 0x04004D3B RID: 19771
		[Tooltip("Store the state name in a string variable.")]
		[UIHint(10)]
		[RequiredField]
		public FsmString storeResult;

		// Token: 0x04004D3C RID: 19772
		[Tooltip("Repeat every frame. E.g.,  useful if you're waiting for the state to change.")]
		public bool everyFrame;

		// Token: 0x04004D3D RID: 19773
		private PlayMakerFSM fsm;
	}
}
