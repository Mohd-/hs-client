using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BBD RID: 3005
	[Tooltip("GUI button. Sends an Event when pressed. Optionally store the button state in a Bool Variable.")]
	[ActionCategory(5)]
	public class GUIButton : GUIContentAction
	{
		// Token: 0x06006464 RID: 25700 RVA: 0x001DDCCC File Offset: 0x001DBECC
		public override void Reset()
		{
			base.Reset();
			this.sendEvent = null;
			this.storeButtonState = null;
			this.style = "Button";
		}

		// Token: 0x06006465 RID: 25701 RVA: 0x001DDD00 File Offset: 0x001DBF00
		public override void OnGUI()
		{
			base.OnGUI();
			bool value = false;
			if (GUI.Button(this.rect, this.content, this.style.Value))
			{
				base.Fsm.Event(this.sendEvent);
				value = true;
			}
			if (this.storeButtonState != null)
			{
				this.storeButtonState.Value = value;
			}
		}

		// Token: 0x04004BE4 RID: 19428
		public FsmEvent sendEvent;

		// Token: 0x04004BE5 RID: 19429
		[UIHint(10)]
		public FsmBool storeButtonState;
	}
}
