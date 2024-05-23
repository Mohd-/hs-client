using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C1A RID: 3098
	[Tooltip("Sends an Event when the specified Mouse Button is pressed. Optionally store the button state in a bool variable.")]
	[ActionCategory(6)]
	public class GetMouseButtonDown : FsmStateAction
	{
		// Token: 0x060065C7 RID: 26055 RVA: 0x001E2C4B File Offset: 0x001E0E4B
		public override void Reset()
		{
			this.button = 0;
			this.sendEvent = null;
			this.storeResult = null;
		}

		// Token: 0x060065C8 RID: 26056 RVA: 0x001E2C64 File Offset: 0x001E0E64
		public override void OnUpdate()
		{
			bool mouseButtonDown = Input.GetMouseButtonDown(this.button);
			if (mouseButtonDown)
			{
				base.Fsm.Event(this.sendEvent);
			}
			this.storeResult.Value = mouseButtonDown;
		}

		// Token: 0x04004D92 RID: 19858
		[RequiredField]
		public MouseButton button;

		// Token: 0x04004D93 RID: 19859
		public FsmEvent sendEvent;

		// Token: 0x04004D94 RID: 19860
		[UIHint(10)]
		public FsmBool storeResult;
	}
}
