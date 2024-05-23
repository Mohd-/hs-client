using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BF9 RID: 3065
	[Tooltip("Gets the last measured linear acceleration of a device and stores it in a Vector3 Variable.")]
	[ActionCategory(33)]
	public class GetDeviceAcceleration : FsmStateAction
	{
		// Token: 0x06006537 RID: 25911 RVA: 0x001E0EB5 File Offset: 0x001DF0B5
		public override void Reset()
		{
			this.storeVector = null;
			this.storeX = null;
			this.storeY = null;
			this.storeZ = null;
			this.multiplier = 1f;
			this.everyFrame = false;
		}

		// Token: 0x06006538 RID: 25912 RVA: 0x001E0EEA File Offset: 0x001DF0EA
		public override void OnEnter()
		{
			this.DoGetDeviceAcceleration();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006539 RID: 25913 RVA: 0x001E0F03 File Offset: 0x001DF103
		public override void OnUpdate()
		{
			this.DoGetDeviceAcceleration();
		}

		// Token: 0x0600653A RID: 25914 RVA: 0x001E0F0C File Offset: 0x001DF10C
		private void DoGetDeviceAcceleration()
		{
			Vector3 vector;
			vector..ctor(Input.acceleration.x, Input.acceleration.y, Input.acceleration.z);
			if (!this.multiplier.IsNone)
			{
				vector *= this.multiplier.Value;
			}
			this.storeVector.Value = vector;
			this.storeX.Value = vector.x;
			this.storeY.Value = vector.y;
			this.storeZ.Value = vector.z;
		}

		// Token: 0x04004CD6 RID: 19670
		[UIHint(10)]
		public FsmVector3 storeVector;

		// Token: 0x04004CD7 RID: 19671
		[UIHint(10)]
		public FsmFloat storeX;

		// Token: 0x04004CD8 RID: 19672
		[UIHint(10)]
		public FsmFloat storeY;

		// Token: 0x04004CD9 RID: 19673
		[UIHint(10)]
		public FsmFloat storeZ;

		// Token: 0x04004CDA RID: 19674
		public FsmFloat multiplier;

		// Token: 0x04004CDB RID: 19675
		public bool everyFrame;
	}
}
