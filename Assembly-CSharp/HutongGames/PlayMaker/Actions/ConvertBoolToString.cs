using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B77 RID: 2935
	[ActionCategory(17)]
	[Tooltip("Converts a Bool value to a String value.")]
	public class ConvertBoolToString : FsmStateAction
	{
		// Token: 0x06006355 RID: 25429 RVA: 0x001DAC37 File Offset: 0x001D8E37
		public override void Reset()
		{
			this.boolVariable = null;
			this.stringVariable = null;
			this.falseString = "False";
			this.trueString = "True";
			this.everyFrame = false;
		}

		// Token: 0x06006356 RID: 25430 RVA: 0x001DAC6E File Offset: 0x001D8E6E
		public override void OnEnter()
		{
			this.DoConvertBoolToString();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006357 RID: 25431 RVA: 0x001DAC87 File Offset: 0x001D8E87
		public override void OnUpdate()
		{
			this.DoConvertBoolToString();
		}

		// Token: 0x06006358 RID: 25432 RVA: 0x001DAC90 File Offset: 0x001D8E90
		private void DoConvertBoolToString()
		{
			this.stringVariable.Value = ((!this.boolVariable.Value) ? this.falseString.Value : this.trueString.Value);
		}

		// Token: 0x04004ADA RID: 19162
		[Tooltip("The Bool variable to test.")]
		[UIHint(10)]
		[RequiredField]
		public FsmBool boolVariable;

		// Token: 0x04004ADB RID: 19163
		[UIHint(10)]
		[Tooltip("The String variable to set based on the Bool variable value.")]
		[RequiredField]
		public FsmString stringVariable;

		// Token: 0x04004ADC RID: 19164
		[Tooltip("String value if Bool variable is false.")]
		public FsmString falseString;

		// Token: 0x04004ADD RID: 19165
		[Tooltip("String value if Bool variable is true.")]
		public FsmString trueString;

		// Token: 0x04004ADE RID: 19166
		[Tooltip("Repeat every frame. Useful if the Bool variable is changing.")]
		public bool everyFrame;
	}
}
