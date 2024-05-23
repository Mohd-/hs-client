using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BC9 RID: 3017
	[ActionCategory(26)]
	[Tooltip("GUILayout Button. Sends an Event when pressed. Optionally stores the button state in a Bool Variable.")]
	public class GUILayoutButton : GUILayoutAction
	{
		// Token: 0x0600648A RID: 25738 RVA: 0x001DE8B4 File Offset: 0x001DCAB4
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

		// Token: 0x0600648B RID: 25739 RVA: 0x001DE90C File Offset: 0x001DCB0C
		public override void OnGUI()
		{
			bool flag;
			if (string.IsNullOrEmpty(this.style.Value))
			{
				flag = GUILayout.Button(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), base.LayoutOptions);
			}
			else
			{
				flag = GUILayout.Button(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), this.style.Value, base.LayoutOptions);
			}
			if (flag)
			{
				base.Fsm.Event(this.sendEvent);
			}
			if (this.storeButtonState != null)
			{
				this.storeButtonState.Value = flag;
			}
		}

		// Token: 0x04004C1A RID: 19482
		public FsmEvent sendEvent;

		// Token: 0x04004C1B RID: 19483
		[UIHint(10)]
		public FsmBool storeButtonState;

		// Token: 0x04004C1C RID: 19484
		public FsmTexture image;

		// Token: 0x04004C1D RID: 19485
		public FsmString text;

		// Token: 0x04004C1E RID: 19486
		public FsmString tooltip;

		// Token: 0x04004C1F RID: 19487
		public FsmString style;
	}
}
