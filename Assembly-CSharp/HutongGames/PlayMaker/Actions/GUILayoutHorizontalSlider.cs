using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BD4 RID: 3028
	[ActionCategory(26)]
	[Tooltip("A Horizontal Slider linked to a Float Variable.")]
	public class GUILayoutHorizontalSlider : GUILayoutAction
	{
		// Token: 0x060064AA RID: 25770 RVA: 0x001DEDD4 File Offset: 0x001DCFD4
		public override void Reset()
		{
			base.Reset();
			this.floatVariable = null;
			this.leftValue = 0f;
			this.rightValue = 100f;
			this.changedEvent = null;
		}

		// Token: 0x060064AB RID: 25771 RVA: 0x001DEE18 File Offset: 0x001DD018
		public override void OnGUI()
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			if (this.floatVariable != null)
			{
				this.floatVariable.Value = GUILayout.HorizontalSlider(this.floatVariable.Value, this.leftValue.Value, this.rightValue.Value, base.LayoutOptions);
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

		// Token: 0x04004C32 RID: 19506
		[RequiredField]
		[UIHint(10)]
		public FsmFloat floatVariable;

		// Token: 0x04004C33 RID: 19507
		[RequiredField]
		public FsmFloat leftValue;

		// Token: 0x04004C34 RID: 19508
		[RequiredField]
		public FsmFloat rightValue;

		// Token: 0x04004C35 RID: 19509
		public FsmEvent changedEvent;
	}
}
