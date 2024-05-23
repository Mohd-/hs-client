using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B75 RID: 2933
	[Tooltip("Converts a Bool value to a Float value.")]
	[ActionCategory(17)]
	public class ConvertBoolToFloat : FsmStateAction
	{
		// Token: 0x0600634B RID: 25419 RVA: 0x001DAAF7 File Offset: 0x001D8CF7
		public override void Reset()
		{
			this.boolVariable = null;
			this.floatVariable = null;
			this.falseValue = 0f;
			this.trueValue = 1f;
			this.everyFrame = false;
		}

		// Token: 0x0600634C RID: 25420 RVA: 0x001DAB2E File Offset: 0x001D8D2E
		public override void OnEnter()
		{
			this.DoConvertBoolToFloat();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600634D RID: 25421 RVA: 0x001DAB47 File Offset: 0x001D8D47
		public override void OnUpdate()
		{
			this.DoConvertBoolToFloat();
		}

		// Token: 0x0600634E RID: 25422 RVA: 0x001DAB50 File Offset: 0x001D8D50
		private void DoConvertBoolToFloat()
		{
			this.floatVariable.Value = ((!this.boolVariable.Value) ? this.falseValue.Value : this.trueValue.Value);
		}

		// Token: 0x04004AD0 RID: 19152
		[UIHint(10)]
		[Tooltip("The Bool variable to test.")]
		[RequiredField]
		public FsmBool boolVariable;

		// Token: 0x04004AD1 RID: 19153
		[Tooltip("The Float variable to set based on the Bool variable value.")]
		[RequiredField]
		[UIHint(10)]
		public FsmFloat floatVariable;

		// Token: 0x04004AD2 RID: 19154
		[Tooltip("Float value if Bool variable is false.")]
		public FsmFloat falseValue;

		// Token: 0x04004AD3 RID: 19155
		[Tooltip("Float value if Bool variable is true.")]
		public FsmFloat trueValue;

		// Token: 0x04004AD4 RID: 19156
		[Tooltip("Repeat every frame. Useful if the Bool variable is changing.")]
		public bool everyFrame;
	}
}
