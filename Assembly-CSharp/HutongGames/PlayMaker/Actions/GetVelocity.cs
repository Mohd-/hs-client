using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C3E RID: 3134
	[ActionCategory(9)]
	[Tooltip("Gets the Velocity of a Game Object and stores it in a Vector3 Variable or each Axis in a Float Variable. NOTE: The Game Object must have a Rigid Body.")]
	public class GetVelocity : ComponentAction<Rigidbody>
	{
		// Token: 0x0600665F RID: 26207 RVA: 0x001E43A7 File Offset: 0x001E25A7
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

		// Token: 0x06006660 RID: 26208 RVA: 0x001E43DA File Offset: 0x001E25DA
		public override void OnEnter()
		{
			this.DoGetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006661 RID: 26209 RVA: 0x001E43F3 File Offset: 0x001E25F3
		public override void OnUpdate()
		{
			this.DoGetVelocity();
		}

		// Token: 0x06006662 RID: 26210 RVA: 0x001E43FC File Offset: 0x001E25FC
		private void DoGetVelocity()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			Vector3 vector = base.rigidbody.velocity;
			if (this.space == 1)
			{
				vector = ownerDefaultTarget.transform.InverseTransformDirection(vector);
			}
			this.vector.Value = vector;
			this.x.Value = vector.x;
			this.y.Value = vector.y;
			this.z.Value = vector.z;
		}

		// Token: 0x04004E17 RID: 19991
		[CheckForComponent(typeof(Rigidbody))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004E18 RID: 19992
		[UIHint(10)]
		public FsmVector3 vector;

		// Token: 0x04004E19 RID: 19993
		[UIHint(10)]
		public FsmFloat x;

		// Token: 0x04004E1A RID: 19994
		[UIHint(10)]
		public FsmFloat y;

		// Token: 0x04004E1B RID: 19995
		[UIHint(10)]
		public FsmFloat z;

		// Token: 0x04004E1C RID: 19996
		public Space space;

		// Token: 0x04004E1D RID: 19997
		public bool everyFrame;
	}
}
