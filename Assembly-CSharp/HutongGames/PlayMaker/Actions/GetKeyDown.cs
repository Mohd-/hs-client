using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C10 RID: 3088
	[ActionCategory(6)]
	[Tooltip("Sends an Event when a Key is pressed.")]
	public class GetKeyDown : FsmStateAction
	{
		// Token: 0x060065A3 RID: 26019 RVA: 0x001E2613 File Offset: 0x001E0813
		public override void Reset()
		{
			this.sendEvent = null;
			this.key = 0;
			this.storeResult = null;
		}

		// Token: 0x060065A4 RID: 26020 RVA: 0x001E262C File Offset: 0x001E082C
		public override void OnUpdate()
		{
			bool keyDown = Input.GetKeyDown(this.key);
			if (keyDown)
			{
				base.Fsm.Event(this.sendEvent);
			}
			this.storeResult.Value = keyDown;
		}

		// Token: 0x04004D73 RID: 19827
		[RequiredField]
		public KeyCode key;

		// Token: 0x04004D74 RID: 19828
		public FsmEvent sendEvent;

		// Token: 0x04004D75 RID: 19829
		[UIHint(10)]
		public FsmBool storeResult;
	}
}
