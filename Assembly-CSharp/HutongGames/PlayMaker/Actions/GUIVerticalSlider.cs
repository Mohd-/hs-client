using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BE1 RID: 3041
	[Tooltip("GUI Vertical Slider connected to a Float Variable.")]
	[ActionCategory(5)]
	public class GUIVerticalSlider : GUIAction
	{
		// Token: 0x060064D4 RID: 25812 RVA: 0x001DF9AC File Offset: 0x001DDBAC
		public override void Reset()
		{
			base.Reset();
			this.floatVariable = null;
			this.topValue = 100f;
			this.bottomValue = 0f;
			this.sliderStyle = "verticalslider";
			this.thumbStyle = "verticalsliderthumb";
			this.width = 0.1f;
		}

		// Token: 0x060064D5 RID: 25813 RVA: 0x001DFA18 File Offset: 0x001DDC18
		public override void OnGUI()
		{
			base.OnGUI();
			if (this.floatVariable != null)
			{
				this.floatVariable.Value = GUI.VerticalSlider(this.rect, this.floatVariable.Value, this.topValue.Value, this.bottomValue.Value, (!(this.sliderStyle.Value != string.Empty)) ? "verticalslider" : this.sliderStyle.Value, (!(this.thumbStyle.Value != string.Empty)) ? "verticalsliderthumb" : this.thumbStyle.Value);
			}
		}

		// Token: 0x04004C65 RID: 19557
		[RequiredField]
		[UIHint(10)]
		public FsmFloat floatVariable;

		// Token: 0x04004C66 RID: 19558
		[RequiredField]
		public FsmFloat topValue;

		// Token: 0x04004C67 RID: 19559
		[RequiredField]
		public FsmFloat bottomValue;

		// Token: 0x04004C68 RID: 19560
		public FsmString sliderStyle;

		// Token: 0x04004C69 RID: 19561
		public FsmString thumbStyle;
	}
}
