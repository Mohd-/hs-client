using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D07 RID: 3335
	[Tooltip("Transforms a Position from a Game Object's local space to world space.")]
	[ActionCategory(14)]
	public class TransformPoint : FsmStateAction
	{
		// Token: 0x060069CB RID: 27083 RVA: 0x001EFF7D File Offset: 0x001EE17D
		public override void Reset()
		{
			this.gameObject = null;
			this.localPosition = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x060069CC RID: 27084 RVA: 0x001EFF9B File Offset: 0x001EE19B
		public override void OnEnter()
		{
			this.DoTransformPoint();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060069CD RID: 27085 RVA: 0x001EFFB4 File Offset: 0x001EE1B4
		public override void OnUpdate()
		{
			this.DoTransformPoint();
		}

		// Token: 0x060069CE RID: 27086 RVA: 0x001EFFBC File Offset: 0x001EE1BC
		private void DoTransformPoint()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.storeResult.Value = ownerDefaultTarget.transform.TransformPoint(this.localPosition.Value);
		}

		// Token: 0x040051A3 RID: 20899
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040051A4 RID: 20900
		[RequiredField]
		public FsmVector3 localPosition;

		// Token: 0x040051A5 RID: 20901
		[RequiredField]
		[UIHint(10)]
		public FsmVector3 storeResult;

		// Token: 0x040051A6 RID: 20902
		public bool everyFrame;
	}
}
