using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BCB RID: 3019
	[Tooltip("GUILayout Password Field. Optionally send an event if the text has been edited.")]
	[ActionCategory(26)]
	public class GUILayoutEmailField : GUILayoutAction
	{
		// Token: 0x06006490 RID: 25744 RVA: 0x001DEACA File Offset: 0x001DCCCA
		public override void Reset()
		{
			this.text = null;
			this.maxLength = 25;
			this.style = "TextField";
			this.valid = true;
			this.changedEvent = null;
		}

		// Token: 0x06006491 RID: 25745 RVA: 0x001DEB04 File Offset: 0x001DCD04
		public override void OnGUI()
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			this.text.Value = GUILayout.TextField(this.text.Value, this.style.Value, base.LayoutOptions);
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

		// Token: 0x04004C27 RID: 19495
		[UIHint(10)]
		public FsmString text;

		// Token: 0x04004C28 RID: 19496
		public FsmInt maxLength;

		// Token: 0x04004C29 RID: 19497
		public FsmString style;

		// Token: 0x04004C2A RID: 19498
		public FsmEvent changedEvent;

		// Token: 0x04004C2B RID: 19499
		public FsmBool valid;
	}
}
