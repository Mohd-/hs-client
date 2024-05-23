using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BF0 RID: 3056
	[ActionCategory(6)]
	[Tooltip("Sends an Event when a Button is released.")]
	public class GetButtonUp : FsmStateAction
	{
		// Token: 0x06006511 RID: 25873 RVA: 0x001E061D File Offset: 0x001DE81D
		public override void Reset()
		{
			this.buttonName = "Fire1";
			this.sendEvent = null;
			this.storeResult = null;
		}

		// Token: 0x06006512 RID: 25874 RVA: 0x001E0640 File Offset: 0x001DE840
		public override void OnUpdate()
		{
			bool buttonUp = Input.GetButtonUp(this.buttonName.Value);
			if (buttonUp)
			{
				base.Fsm.Event(this.sendEvent);
			}
			this.storeResult.Value = buttonUp;
		}

		// Token: 0x04004CAD RID: 19629
		[Tooltip("The name of the button. Set in the Unity Input Manager.")]
		[RequiredField]
		public FsmString buttonName;

		// Token: 0x04004CAE RID: 19630
		[Tooltip("Event to send if the button is released.")]
		public FsmEvent sendEvent;

		// Token: 0x04004CAF RID: 19631
		[Tooltip("Set to True if the button is released.")]
		[UIHint(10)]
		public FsmBool storeResult;
	}
}
