using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BD2 RID: 3026
	[Tooltip("GUILayout Text Field to edit a Float Variable. Optionally send an event if the text has been edited.")]
	[ActionCategory(26)]
	public class GUILayoutFloatField : GUILayoutAction
	{
		// Token: 0x060064A4 RID: 25764 RVA: 0x001DEBFC File Offset: 0x001DCDFC
		public override void Reset()
		{
			base.Reset();
			this.floatVariable = null;
			this.style = string.Empty;
			this.changedEvent = null;
		}

		// Token: 0x060064A5 RID: 25765 RVA: 0x001DEC30 File Offset: 0x001DCE30
		public override void OnGUI()
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			if (!string.IsNullOrEmpty(this.style.Value))
			{
				this.floatVariable.Value = float.Parse(GUILayout.TextField(this.floatVariable.Value.ToString(), this.style.Value, base.LayoutOptions));
			}
			else
			{
				this.floatVariable.Value = float.Parse(GUILayout.TextField(this.floatVariable.Value.ToString(), base.LayoutOptions));
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

		// Token: 0x04004C2C RID: 19500
		[Tooltip("Float Variable to show in the edit field.")]
		[UIHint(10)]
		public FsmFloat floatVariable;

		// Token: 0x04004C2D RID: 19501
		[Tooltip("Optional GUIStyle in the active GUISKin.")]
		public FsmString style;

		// Token: 0x04004C2E RID: 19502
		[Tooltip("Optional event to send when the value changes.")]
		public FsmEvent changedEvent;
	}
}
