using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C4C RID: 3148
	[ActionCategory(14)]
	[Tooltip("Transforms position from world space to a Game Object's local space. The opposite of TransformPoint.")]
	public class InverseTransformPoint : FsmStateAction
	{
		// Token: 0x0600669C RID: 26268 RVA: 0x001E4D5D File Offset: 0x001E2F5D
		public override void Reset()
		{
			this.gameObject = null;
			this.worldPosition = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x0600669D RID: 26269 RVA: 0x001E4D7B File Offset: 0x001E2F7B
		public override void OnEnter()
		{
			this.DoInverseTransformPoint();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600669E RID: 26270 RVA: 0x001E4D94 File Offset: 0x001E2F94
		public override void OnUpdate()
		{
			this.DoInverseTransformPoint();
		}

		// Token: 0x0600669F RID: 26271 RVA: 0x001E4D9C File Offset: 0x001E2F9C
		private void DoInverseTransformPoint()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.storeResult.Value = ownerDefaultTarget.transform.InverseTransformPoint(this.worldPosition.Value);
		}

		// Token: 0x04004E53 RID: 20051
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004E54 RID: 20052
		[RequiredField]
		public FsmVector3 worldPosition;

		// Token: 0x04004E55 RID: 20053
		[UIHint(10)]
		[RequiredField]
		public FsmVector3 storeResult;

		// Token: 0x04004E56 RID: 20054
		public bool everyFrame;
	}
}
