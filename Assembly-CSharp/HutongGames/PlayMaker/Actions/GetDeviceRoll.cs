using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BFA RID: 3066
	[ActionCategory(33)]
	[Tooltip("Gets the rotation of the device around its z axis (into the screen). For example when you steer with the iPhone in a driving game.")]
	public class GetDeviceRoll : FsmStateAction
	{
		// Token: 0x0600653C RID: 25916 RVA: 0x001E0FB4 File Offset: 0x001DF1B4
		public override void Reset()
		{
			this.baseOrientation = GetDeviceRoll.BaseOrientation.LandscapeLeft;
			this.storeAngle = null;
			this.limitAngle = new FsmFloat
			{
				UseVariable = true
			};
			this.smoothing = 5f;
			this.everyFrame = true;
		}

		// Token: 0x0600653D RID: 25917 RVA: 0x001E0FFA File Offset: 0x001DF1FA
		public override void OnEnter()
		{
			this.DoGetDeviceRoll();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600653E RID: 25918 RVA: 0x001E1013 File Offset: 0x001DF213
		public override void OnUpdate()
		{
			this.DoGetDeviceRoll();
		}

		// Token: 0x0600653F RID: 25919 RVA: 0x001E101C File Offset: 0x001DF21C
		private void DoGetDeviceRoll()
		{
			float x = Input.acceleration.x;
			float y = Input.acceleration.y;
			float num = 0f;
			switch (this.baseOrientation)
			{
			case GetDeviceRoll.BaseOrientation.Portrait:
				num = -Mathf.Atan2(x, -y);
				break;
			case GetDeviceRoll.BaseOrientation.LandscapeLeft:
				num = Mathf.Atan2(y, -x);
				break;
			case GetDeviceRoll.BaseOrientation.LandscapeRight:
				num = -Mathf.Atan2(y, x);
				break;
			}
			if (!this.limitAngle.IsNone)
			{
				num = Mathf.Clamp(57.29578f * num, -this.limitAngle.Value, this.limitAngle.Value);
			}
			if (this.smoothing.Value > 0f)
			{
				num = Mathf.LerpAngle(this.lastZAngle, num, this.smoothing.Value * Time.deltaTime);
			}
			this.lastZAngle = num;
			this.storeAngle.Value = num;
		}

		// Token: 0x04004CDC RID: 19676
		[Tooltip("How the user is expected to hold the device (where angle will be zero).")]
		public GetDeviceRoll.BaseOrientation baseOrientation;

		// Token: 0x04004CDD RID: 19677
		[UIHint(10)]
		public FsmFloat storeAngle;

		// Token: 0x04004CDE RID: 19678
		public FsmFloat limitAngle;

		// Token: 0x04004CDF RID: 19679
		public FsmFloat smoothing;

		// Token: 0x04004CE0 RID: 19680
		public bool everyFrame;

		// Token: 0x04004CE1 RID: 19681
		private float lastZAngle;

		// Token: 0x02000BFB RID: 3067
		public enum BaseOrientation
		{
			// Token: 0x04004CE3 RID: 19683
			Portrait,
			// Token: 0x04004CE4 RID: 19684
			LandscapeLeft,
			// Token: 0x04004CE5 RID: 19685
			LandscapeRight
		}
	}
}
