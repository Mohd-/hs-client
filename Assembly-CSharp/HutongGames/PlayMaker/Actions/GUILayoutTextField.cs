using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BDB RID: 3035
	[ActionCategory(26)]
	[Tooltip("GUILayout Text Field. Optionally send an event if the text has been edited.")]
	public class GUILayoutTextField : GUILayoutAction
	{
		// Token: 0x060064BF RID: 25791 RVA: 0x001DF37A File Offset: 0x001DD57A
		public override void Reset()
		{
			base.Reset();
			this.text = null;
			this.maxLength = 25;
			this.style = "TextField";
			this.changedEvent = null;
		}

		// Token: 0x060064C0 RID: 25792 RVA: 0x001DF3B0 File Offset: 0x001DD5B0
		public override void OnGUI()
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			this.text.Value = GUILayout.TextField(this.text.Value, this.maxLength.Value, this.style.Value, base.LayoutOptions);
			if (GUI.changed)
			{
				base.Fsm.Event(this.changedEvent);
				GUIUtility.ExitGUI();
			}
			else
			{
				GUI.changed = changed;
			}
		}

		// Token: 0x04004C4C RID: 19532
		[UIHint(10)]
		public FsmString text;

		// Token: 0x04004C4D RID: 19533
		public FsmInt maxLength;

		// Token: 0x04004C4E RID: 19534
		public FsmString style;

		// Token: 0x04004C4F RID: 19535
		public FsmEvent changedEvent;
	}
}
