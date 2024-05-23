using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C1B RID: 3099
	[Tooltip("Sends an Event when the specified Mouse Button is released. Optionally store the button state in a bool variable.")]
	[ActionCategory(6)]
	public class GetMouseButtonUp : FsmStateAction
	{
		// Token: 0x060065CA RID: 26058 RVA: 0x001E2CA8 File Offset: 0x001E0EA8
		public override void Reset()
		{
			this.button = 0;
			this.sendEvent = null;
			this.storeResult = null;
		}

		// Token: 0x060065CB RID: 26059 RVA: 0x001E2CC0 File Offset: 0x001E0EC0
		public override void OnUpdate()
		{
			bool mouseButtonUp = Input.GetMouseButtonUp(this.button);
			if (mouseButtonUp)
			{
				base.Fsm.Event(this.sendEvent);
			}
			this.storeResult.Value = mouseButtonUp;
		}

		// Token: 0x04004D95 RID: 19861
		[RequiredField]
		public MouseButton button;

		// Token: 0x04004D96 RID: 19862
		public FsmEvent sendEvent;

		// Token: 0x04004D97 RID: 19863
		[UIHint(10)]
		public FsmBool storeResult;
	}
}
