using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CC3 RID: 3267
	[Tooltip("Sets the gravity vector, or individual axis.")]
	[ActionCategory(9)]
	public class SetGravity : FsmStateAction
	{
		// Token: 0x060068A9 RID: 26793 RVA: 0x001EBCE4 File Offset: 0x001E9EE4
		public override void Reset()
		{
			this.vector = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.z = new FsmFloat
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		// Token: 0x060068AA RID: 26794 RVA: 0x001EBD3B File Offset: 0x001E9F3B
		public override void OnEnter()
		{
			this.DoSetGravity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060068AB RID: 26795 RVA: 0x001EBD54 File Offset: 0x001E9F54
		public override void OnUpdate()
		{
			this.DoSetGravity();
		}

		// Token: 0x060068AC RID: 26796 RVA: 0x001EBD5C File Offset: 0x001E9F5C
		private void DoSetGravity()
		{
			Vector3 value = this.vector.Value;
			if (!this.x.IsNone)
			{
				value.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				value.y = this.y.Value;
			}
			if (!this.z.IsNone)
			{
				value.z = this.z.Value;
			}
			Physics.gravity = value;
		}

		// Token: 0x04005077 RID: 20599
		public FsmVector3 vector;

		// Token: 0x04005078 RID: 20600
		public FsmFloat x;

		// Token: 0x04005079 RID: 20601
		public FsmFloat y;

		// Token: 0x0400507A RID: 20602
		public FsmFloat z;

		// Token: 0x0400507B RID: 20603
		public bool everyFrame;
	}
}
