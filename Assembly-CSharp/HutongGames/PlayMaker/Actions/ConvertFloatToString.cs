using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B7A RID: 2938
	[ActionCategory(17)]
	[Tooltip("Converts a Float value to a String value with optional format.")]
	public class ConvertFloatToString : FsmStateAction
	{
		// Token: 0x0600635F RID: 25439 RVA: 0x001DADAF File Offset: 0x001D8FAF
		public override void Reset()
		{
			this.floatVariable = null;
			this.stringVariable = null;
			this.everyFrame = false;
			this.format = null;
		}

		// Token: 0x06006360 RID: 25440 RVA: 0x001DADCD File Offset: 0x001D8FCD
		public override void OnEnter()
		{
			this.DoConvertFloatToString();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006361 RID: 25441 RVA: 0x001DADE6 File Offset: 0x001D8FE6
		public override void OnUpdate()
		{
			this.DoConvertFloatToString();
		}

		// Token: 0x06006362 RID: 25442 RVA: 0x001DADF0 File Offset: 0x001D8FF0
		private void DoConvertFloatToString()
		{
			if (this.format.IsNone || string.IsNullOrEmpty(this.format.Value))
			{
				this.stringVariable.Value = this.floatVariable.Value.ToString();
			}
			else
			{
				this.stringVariable.Value = this.floatVariable.Value.ToString(this.format.Value);
			}
		}

		// Token: 0x04004AE7 RID: 19175
		[Tooltip("The float variable to convert.")]
		[RequiredField]
		[UIHint(10)]
		public FsmFloat floatVariable;

		// Token: 0x04004AE8 RID: 19176
		[UIHint(10)]
		[Tooltip("A string variable to store the converted value.")]
		[RequiredField]
		public FsmString stringVariable;

		// Token: 0x04004AE9 RID: 19177
		[Tooltip("Optional Format, allows for leading zeroes. E.g., 0000")]
		public FsmString format;

		// Token: 0x04004AEA RID: 19178
		[Tooltip("Repeat every frame. Useful if the float variable is changing.")]
		public bool everyFrame;
	}
}
