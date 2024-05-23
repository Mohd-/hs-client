using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BDF RID: 3039
	[Tooltip("A Vertical Slider linked to a Float Variable.")]
	[ActionCategory(26)]
	public class GUILayoutVerticalSlider : GUILayoutAction
	{
		// Token: 0x060064CE RID: 25806 RVA: 0x001DF8B4 File Offset: 0x001DDAB4
		public override void Reset()
		{
			base.Reset();
			this.floatVariable = null;
			this.topValue = 100f;
			this.bottomValue = 0f;
			this.changedEvent = null;
		}

		// Token: 0x060064CF RID: 25807 RVA: 0x001DF8F8 File Offset: 0x001DDAF8
		public override void OnGUI()
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			if (this.floatVariable != null)
			{
				this.floatVariable.Value = GUILayout.VerticalSlider(this.floatVariable.Value, this.topValue.Value, this.bottomValue.Value, base.LayoutOptions);
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

		// Token: 0x04004C60 RID: 19552
		[RequiredField]
		[UIHint(10)]
		public FsmFloat floatVariable;

		// Token: 0x04004C61 RID: 19553
		[RequiredField]
		public FsmFloat topValue;

		// Token: 0x04004C62 RID: 19554
		[RequiredField]
		public FsmFloat bottomValue;

		// Token: 0x04004C63 RID: 19555
		public FsmEvent changedEvent;
	}
}
