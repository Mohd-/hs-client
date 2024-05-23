using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BEF RID: 3055
	[Tooltip("Sends an Event when a Button is pressed.")]
	[ActionCategory(6)]
	public class GetButtonDown : FsmStateAction
	{
		// Token: 0x0600650E RID: 25870 RVA: 0x001E05B3 File Offset: 0x001DE7B3
		public override void Reset()
		{
			this.buttonName = "Fire1";
			this.sendEvent = null;
			this.storeResult = null;
		}

		// Token: 0x0600650F RID: 25871 RVA: 0x001E05D4 File Offset: 0x001DE7D4
		public override void OnUpdate()
		{
			bool buttonDown = Input.GetButtonDown(this.buttonName.Value);
			if (buttonDown)
			{
				base.Fsm.Event(this.sendEvent);
			}
			this.storeResult.Value = buttonDown;
		}

		// Token: 0x04004CAA RID: 19626
		[Tooltip("The name of the button. Set in the Unity Input Manager.")]
		[RequiredField]
		public FsmString buttonName;

		// Token: 0x04004CAB RID: 19627
		[Tooltip("Event to send if the button is pressed.")]
		public FsmEvent sendEvent;

		// Token: 0x04004CAC RID: 19628
		[Tooltip("Set to True if the button is pressed.")]
		[UIHint(10)]
		public FsmBool storeResult;
	}
}
