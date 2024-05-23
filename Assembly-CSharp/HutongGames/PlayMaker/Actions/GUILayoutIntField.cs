using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BD5 RID: 3029
	[ActionCategory(26)]
	[Tooltip("GUILayout Text Field to edit an Int Variable. Optionally send an event if the text has been edited.")]
	public class GUILayoutIntField : GUILayoutAction
	{
		// Token: 0x060064AD RID: 25773 RVA: 0x001DEEA8 File Offset: 0x001DD0A8
		public override void Reset()
		{
			base.Reset();
			this.intVariable = null;
			this.style = string.Empty;
			this.changedEvent = null;
		}

		// Token: 0x060064AE RID: 25774 RVA: 0x001DEEDC File Offset: 0x001DD0DC
		public override void OnGUI()
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			if (!string.IsNullOrEmpty(this.style.Value))
			{
				this.intVariable.Value = int.Parse(GUILayout.TextField(this.intVariable.Value.ToString(), this.style.Value, base.LayoutOptions));
			}
			else
			{
				this.intVariable.Value = int.Parse(GUILayout.TextField(this.intVariable.Value.ToString(), base.LayoutOptions));
			}
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

		// Token: 0x04004C36 RID: 19510
		[Tooltip("Int Variable to show in the edit field.")]
		[UIHint(10)]
		public FsmInt intVariable;

		// Token: 0x04004C37 RID: 19511
		[Tooltip("Optional GUIStyle in the active GUISKin.")]
		public FsmString style;

		// Token: 0x04004C38 RID: 19512
		[Tooltip("Optional event to send when the value changes.")]
		public FsmEvent changedEvent;
	}
}
