using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BD8 RID: 3032
	[Tooltip("GUILayout Password Field. Optionally send an event if the text has been edited.")]
	[ActionCategory(26)]
	public class GUILayoutPasswordField : GUILayoutAction
	{
		// Token: 0x060064B6 RID: 25782 RVA: 0x001DF161 File Offset: 0x001DD361
		public override void Reset()
		{
			this.text = null;
			this.maxLength = 25;
			this.style = "TextField";
			this.mask = "*";
			this.changedEvent = null;
		}

		// Token: 0x060064B7 RID: 25783 RVA: 0x001DF1A0 File Offset: 0x001DD3A0
		public override void OnGUI()
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			this.text.Value = GUILayout.PasswordField(this.text.Value, this.mask.Value.get_Chars(0), this.style.Value, base.LayoutOptions);
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

		// Token: 0x04004C40 RID: 19520
		[UIHint(10)]
		public FsmString text;

		// Token: 0x04004C41 RID: 19521
		public FsmInt maxLength;

		// Token: 0x04004C42 RID: 19522
		public FsmString style;

		// Token: 0x04004C43 RID: 19523
		public FsmEvent changedEvent;

		// Token: 0x04004C44 RID: 19524
		public FsmString mask;
	}
}
