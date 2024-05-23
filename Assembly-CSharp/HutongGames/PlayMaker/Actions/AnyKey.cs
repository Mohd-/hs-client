using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B54 RID: 2900
	[Tooltip("Sends an Event when the user hits any Key or Mouse Button.")]
	[ActionCategory(6)]
	public class AnyKey : FsmStateAction
	{
		// Token: 0x060062BC RID: 25276 RVA: 0x001D874B File Offset: 0x001D694B
		public override void Reset()
		{
			this.sendEvent = null;
		}

		// Token: 0x060062BD RID: 25277 RVA: 0x001D8754 File Offset: 0x001D6954
		public override void OnUpdate()
		{
			if (Input.anyKeyDown)
			{
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x04004A35 RID: 18997
		[RequiredField]
		[Tooltip("Event to send when any Key or Mouse Button is pressed.")]
		public FsmEvent sendEvent;
	}
}
