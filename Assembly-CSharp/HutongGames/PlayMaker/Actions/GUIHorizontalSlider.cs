using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BBF RID: 3007
	[Tooltip("GUI Horizontal Slider connected to a Float Variable.")]
	[ActionCategory(5)]
	public class GUIHorizontalSlider : GUIAction
	{
		// Token: 0x0600646C RID: 25708 RVA: 0x001DDF7C File Offset: 0x001DC17C
		public override void Reset()
		{
			base.Reset();
			this.floatVariable = null;
			this.leftValue = 0f;
			this.rightValue = 100f;
			this.sliderStyle = "horizontalslider";
			this.thumbStyle = "horizontalsliderthumb";
		}

		// Token: 0x0600646D RID: 25709 RVA: 0x001DDFD8 File Offset: 0x001DC1D8
		public override void OnGUI()
		{
			base.OnGUI();
			if (this.floatVariable != null)
			{
				this.floatVariable.Value = GUI.HorizontalSlider(this.rect, this.floatVariable.Value, this.leftValue.Value, this.rightValue.Value, (!(this.sliderStyle.Value != string.Empty)) ? "horizontalslider" : this.sliderStyle.Value, (!(this.thumbStyle.Value != string.Empty)) ? "horizontalsliderthumb" : this.thumbStyle.Value);
			}
		}

		// Token: 0x04004BF1 RID: 19441
		[UIHint(10)]
		[RequiredField]
		public FsmFloat floatVariable;

		// Token: 0x04004BF2 RID: 19442
		[RequiredField]
		public FsmFloat leftValue;

		// Token: 0x04004BF3 RID: 19443
		[RequiredField]
		public FsmFloat rightValue;

		// Token: 0x04004BF4 RID: 19444
		public FsmString sliderStyle;

		// Token: 0x04004BF5 RID: 19445
		public FsmString thumbStyle;
	}
}
