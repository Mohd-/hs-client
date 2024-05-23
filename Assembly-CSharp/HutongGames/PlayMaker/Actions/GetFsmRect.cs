using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C06 RID: 3078
	[ActionCategory(12)]
	[Tooltip("Get the value of a Rect Variable from another FSM.")]
	public class GetFsmRect : FsmStateAction
	{
		// Token: 0x06006571 RID: 25969 RVA: 0x001E1C03 File Offset: 0x001DFE03
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.variableName = string.Empty;
			this.storeValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006572 RID: 25970 RVA: 0x001E1C3A File Offset: 0x001DFE3A
		public override void OnEnter()
		{
			this.DoGetFsmVariable();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006573 RID: 25971 RVA: 0x001E1C53 File Offset: 0x001DFE53
		public override void OnUpdate()
		{
			this.DoGetFsmVariable();
		}

		// Token: 0x06006574 RID: 25972 RVA: 0x001E1C5C File Offset: 0x001DFE5C
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
			FsmRect fsmRect = this.fsm.FsmVariables.GetFsmRect(this.variableName.Value);
			if (fsmRect != null)
			{
				this.storeValue.Value = fsmRect.Value;
			}
		}

		// Token: 0x04004D31 RID: 19761
		[Tooltip("The GameObject that owns the FSM.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004D32 RID: 19762
		[Tooltip("Optional name of FSM on Game Object")]
		[UIHint(15)]
		public FsmString fsmName;

		// Token: 0x04004D33 RID: 19763
		[UIHint(24)]
		[RequiredField]
		public FsmString variableName;

		// Token: 0x04004D34 RID: 19764
		[UIHint(10)]
		[RequiredField]
		public FsmRect storeValue;

		// Token: 0x04004D35 RID: 19765
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04004D36 RID: 19766
		private GameObject goLastFrame;

		// Token: 0x04004D37 RID: 19767
		protected PlayMakerFSM fsm;
	}
}
