using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C22 RID: 3106
	[Tooltip("Gets the Position of a Game Object and stores it in a Vector3 Variable or each Axis in a Float Variable")]
	[ActionCategory(14)]
	public class GetPosition : FsmStateAction
	{
		// Token: 0x060065E6 RID: 26086 RVA: 0x001E306D File Offset: 0x001E126D
		public override void Reset()
		{
			this.gameObject = null;
			this.vector = null;
			this.x = null;
			this.y = null;
			this.z = null;
			this.space = 0;
			this.everyFrame = false;
		}

		// Token: 0x060065E7 RID: 26087 RVA: 0x001E30A0 File Offset: 0x001E12A0
		public override void OnEnter()
		{
			this.DoGetPosition();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060065E8 RID: 26088 RVA: 0x001E30B9 File Offset: 0x001E12B9
		public override void OnUpdate()
		{
			this.DoGetPosition();
		}

		// Token: 0x060065E9 RID: 26089 RVA: 0x001E30C4 File Offset: 0x001E12C4
		private void DoGetPosition()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 value = (this.space != null) ? ownerDefaultTarget.transform.localPosition : ownerDefaultTarget.transform.position;
			this.vector.Value = value;
			this.x.Value = value.x;
			this.y.Value = value.y;
			this.z.Value = value.z;
		}

		// Token: 0x04004DA8 RID: 19880
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004DA9 RID: 19881
		[UIHint(10)]
		public FsmVector3 vector;

		// Token: 0x04004DAA RID: 19882
		[UIHint(10)]
		public FsmFloat x;

		// Token: 0x04004DAB RID: 19883
		[UIHint(10)]
		public FsmFloat y;

		// Token: 0x04004DAC RID: 19884
		[UIHint(10)]
		public FsmFloat z;

		// Token: 0x04004DAD RID: 19885
		public Space space;

		// Token: 0x04004DAE RID: 19886
		public bool everyFrame;
	}
}
