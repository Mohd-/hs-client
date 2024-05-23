using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B76 RID: 2934
	[ActionCategory(17)]
	[Tooltip("Converts a Bool value to an Integer value.")]
	public class ConvertBoolToInt : FsmStateAction
	{
		// Token: 0x06006350 RID: 25424 RVA: 0x001DAB9B File Offset: 0x001D8D9B
		public override void Reset()
		{
			this.boolVariable = null;
			this.intVariable = null;
			this.falseValue = 0;
			this.trueValue = 1;
			this.everyFrame = false;
		}

		// Token: 0x06006351 RID: 25425 RVA: 0x001DABCA File Offset: 0x001D8DCA
		public override void OnEnter()
		{
			this.DoConvertBoolToInt();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006352 RID: 25426 RVA: 0x001DABE3 File Offset: 0x001D8DE3
		public override void OnUpdate()
		{
			this.DoConvertBoolToInt();
		}

		// Token: 0x06006353 RID: 25427 RVA: 0x001DABEC File Offset: 0x001D8DEC
		private void DoConvertBoolToInt()
		{
			this.intVariable.Value = ((!this.boolVariable.Value) ? this.falseValue.Value : this.trueValue.Value);
		}

		// Token: 0x04004AD5 RID: 19157
		[RequiredField]
		[UIHint(10)]
		[Tooltip("The Bool variable to test.")]
		public FsmBool boolVariable;

		// Token: 0x04004AD6 RID: 19158
		[RequiredField]
		[Tooltip("The Integer variable to set based on the Bool variable value.")]
		[UIHint(10)]
		public FsmInt intVariable;

		// Token: 0x04004AD7 RID: 19159
		[Tooltip("Integer value if Bool variable is false.")]
		public FsmInt falseValue;

		// Token: 0x04004AD8 RID: 19160
		[Tooltip("Integer value if Bool variable is false.")]
		public FsmInt trueValue;

		// Token: 0x04004AD9 RID: 19161
		[Tooltip("Repeat every frame. Useful if the Bool variable is changing.")]
		public bool everyFrame;
	}
}
