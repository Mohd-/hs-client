using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BD9 RID: 3033
	[Tooltip("GUILayout Repeat Button. Sends an Event while pressed. Optionally store the button state in a Bool Variable.")]
	[ActionCategory(26)]
	public class GUILayoutRepeatButton : GUILayoutAction
	{
		// Token: 0x060064B9 RID: 25785 RVA: 0x001DF230 File Offset: 0x001DD430
		public override void Reset()
		{
			base.Reset();
			this.sendEvent = null;
			this.storeButtonState = null;
			this.text = string.Empty;
			this.image = null;
			this.tooltip = string.Empty;
			this.style = string.Empty;
		}

		// Token: 0x060064BA RID: 25786 RVA: 0x001DF288 File Offset: 0x001DD488
		public override void OnGUI()
		{
			bool flag;
			if (string.IsNullOrEmpty(this.style.Value))
			{
				flag = GUILayout.RepeatButton(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), base.LayoutOptions);
			}
			else
			{
				flag = GUILayout.RepeatButton(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), this.style.Value, base.LayoutOptions);
			}
			if (flag)
			{
				base.Fsm.Event(this.sendEvent);
			}
			this.storeButtonState.Value = flag;
		}

		// Token: 0x04004C45 RID: 19525
		public FsmEvent sendEvent;

		// Token: 0x04004C46 RID: 19526
		[UIHint(10)]
		public FsmBool storeButtonState;

		// Token: 0x04004C47 RID: 19527
		public FsmTexture image;

		// Token: 0x04004C48 RID: 19528
		public FsmString text;

		// Token: 0x04004C49 RID: 19529
		public FsmString tooltip;

		// Token: 0x04004C4A RID: 19530
		public FsmString style;
	}
}
