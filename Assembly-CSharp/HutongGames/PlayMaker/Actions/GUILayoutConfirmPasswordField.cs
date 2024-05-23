using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BCA RID: 3018
	[Tooltip("GUILayout Password Field. Optionally send an event if the text has been edited.")]
	[ActionCategory(26)]
	public class GUILayoutConfirmPasswordField : GUILayoutAction
	{
		// Token: 0x0600648D RID: 25741 RVA: 0x001DE9E0 File Offset: 0x001DCBE0
		public override void Reset()
		{
			this.text = null;
			this.maxLength = 25;
			this.style = "TextField";
			this.mask = "*";
			this.changedEvent = null;
			this.confirm = false;
			this.password = null;
		}

		// Token: 0x0600648E RID: 25742 RVA: 0x001DEA3C File Offset: 0x001DCC3C
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

		// Token: 0x04004C20 RID: 19488
		[UIHint(10)]
		public FsmString text;

		// Token: 0x04004C21 RID: 19489
		public FsmInt maxLength;

		// Token: 0x04004C22 RID: 19490
		public FsmString style;

		// Token: 0x04004C23 RID: 19491
		public FsmEvent changedEvent;

		// Token: 0x04004C24 RID: 19492
		public FsmString mask;

		// Token: 0x04004C25 RID: 19493
		public FsmBool confirm;

		// Token: 0x04004C26 RID: 19494
		public FsmString password;
	}
}
