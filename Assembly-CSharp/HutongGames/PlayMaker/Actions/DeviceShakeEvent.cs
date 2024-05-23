using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B93 RID: 2963
	[Tooltip("Sends an Event when the mobile device is shaken.")]
	[ActionCategory(33)]
	public class DeviceShakeEvent : FsmStateAction
	{
		// Token: 0x060063BC RID: 25532 RVA: 0x001DBBA6 File Offset: 0x001D9DA6
		public override void Reset()
		{
			this.shakeThreshold = 3f;
			this.sendEvent = null;
		}

		// Token: 0x060063BD RID: 25533 RVA: 0x001DBBC0 File Offset: 0x001D9DC0
		public override void OnUpdate()
		{
			if (Input.acceleration.sqrMagnitude > this.shakeThreshold.Value * this.shakeThreshold.Value)
			{
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x04004B30 RID: 19248
		[Tooltip("Amount of acceleration required to trigger the event. Higher numbers require a harder shake.")]
		[RequiredField]
		public FsmFloat shakeThreshold;

		// Token: 0x04004B31 RID: 19249
		[RequiredField]
		[Tooltip("Event to send when Shake Threshold is exceded.")]
		public FsmEvent sendEvent;
	}
}
