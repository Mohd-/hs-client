using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C4B RID: 3147
	[Tooltip("Transforms a Direction from world space to a Game Object's local space. The opposite of TransformDirection.")]
	[ActionCategory(14)]
	public class InverseTransformDirection : FsmStateAction
	{
		// Token: 0x06006697 RID: 26263 RVA: 0x001E4CC8 File Offset: 0x001E2EC8
		public override void Reset()
		{
			this.gameObject = null;
			this.worldDirection = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006698 RID: 26264 RVA: 0x001E4CE6 File Offset: 0x001E2EE6
		public override void OnEnter()
		{
			this.DoInverseTransformDirection();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006699 RID: 26265 RVA: 0x001E4CFF File Offset: 0x001E2EFF
		public override void OnUpdate()
		{
			this.DoInverseTransformDirection();
		}

		// Token: 0x0600669A RID: 26266 RVA: 0x001E4D08 File Offset: 0x001E2F08
		private void DoInverseTransformDirection()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.storeResult.Value = ownerDefaultTarget.transform.InverseTransformDirection(this.worldDirection.Value);
		}

		// Token: 0x04004E4F RID: 20047
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004E50 RID: 20048
		[RequiredField]
		public FsmVector3 worldDirection;

		// Token: 0x04004E51 RID: 20049
		[RequiredField]
		[UIHint(10)]
		public FsmVector3 storeResult;

		// Token: 0x04004E52 RID: 20050
		public bool everyFrame;
	}
}
