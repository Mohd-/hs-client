using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C11 RID: 3089
	[ActionCategory(6)]
	[Tooltip("Sends an Event when a Key is released.")]
	public class GetKeyUp : FsmStateAction
	{
		// Token: 0x060065A6 RID: 26022 RVA: 0x001E2670 File Offset: 0x001E0870
		public override void Reset()
		{
			this.sendEvent = null;
			this.key = 0;
			this.storeResult = null;
		}

		// Token: 0x060065A7 RID: 26023 RVA: 0x001E2688 File Offset: 0x001E0888
		public override void OnUpdate()
		{
			bool keyUp = Input.GetKeyUp(this.key);
			if (keyUp)
			{
				base.Fsm.Event(this.sendEvent);
			}
			this.storeResult.Value = keyUp;
		}

		// Token: 0x04004D76 RID: 19830
		[RequiredField]
		public KeyCode key;

		// Token: 0x04004D77 RID: 19831
		public FsmEvent sendEvent;

		// Token: 0x04004D78 RID: 19832
		[UIHint(10)]
		public FsmBool storeResult;
	}
}
