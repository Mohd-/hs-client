using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D04 RID: 3332
	[Tooltip("Transforms a Direction from a Game Object's local space to world space.")]
	[ActionCategory(14)]
	public class TransformDirection : FsmStateAction
	{
		// Token: 0x060069C3 RID: 27075 RVA: 0x001EFCDB File Offset: 0x001EDEDB
		public override void Reset()
		{
			this.gameObject = null;
			this.localDirection = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x060069C4 RID: 27076 RVA: 0x001EFCF9 File Offset: 0x001EDEF9
		public override void OnEnter()
		{
			this.DoTransformDirection();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060069C5 RID: 27077 RVA: 0x001EFD12 File Offset: 0x001EDF12
		public override void OnUpdate()
		{
			this.DoTransformDirection();
		}

		// Token: 0x060069C6 RID: 27078 RVA: 0x001EFD1C File Offset: 0x001EDF1C
		private void DoTransformDirection()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.storeResult.Value = ownerDefaultTarget.transform.TransformDirection(this.localDirection.Value);
		}

		// Token: 0x04005194 RID: 20884
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005195 RID: 20885
		[RequiredField]
		public FsmVector3 localDirection;

		// Token: 0x04005196 RID: 20886
		[UIHint(10)]
		[RequiredField]
		public FsmVector3 storeResult;

		// Token: 0x04005197 RID: 20887
		public bool everyFrame;
	}
}
