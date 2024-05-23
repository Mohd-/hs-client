using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B91 RID: 2961
	[ActionCategory(33)]
	[Tooltip("Sends an Event based on the Orientation of the mobile device.")]
	public class DeviceOrientationEvent : FsmStateAction
	{
		// Token: 0x060063B4 RID: 25524 RVA: 0x001DBB1B File Offset: 0x001D9D1B
		public override void Reset()
		{
			this.orientation = 1;
			this.sendEvent = null;
			this.everyFrame = false;
		}

		// Token: 0x060063B5 RID: 25525 RVA: 0x001DBB32 File Offset: 0x001D9D32
		public override void OnEnter()
		{
			this.DoDetectDeviceOrientation();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060063B6 RID: 25526 RVA: 0x001DBB4B File Offset: 0x001D9D4B
		public override void OnUpdate()
		{
			this.DoDetectDeviceOrientation();
		}

		// Token: 0x060063B7 RID: 25527 RVA: 0x001DBB53 File Offset: 0x001D9D53
		private void DoDetectDeviceOrientation()
		{
			if (Input.deviceOrientation == this.orientation)
			{
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x04004B2A RID: 19242
		[Tooltip("Note: If device is physically situated between discrete positions, as when (for example) rotated diagonally, system will report Unknown orientation.")]
		public DeviceOrientation orientation;

		// Token: 0x04004B2B RID: 19243
		[Tooltip("The event to send if the device orientation matches Orientation.")]
		public FsmEvent sendEvent;

		// Token: 0x04004B2C RID: 19244
		[Tooltip("Repeat every frame. Useful if you want to wait for the orientation to be true.")]
		public bool everyFrame;
	}
}
