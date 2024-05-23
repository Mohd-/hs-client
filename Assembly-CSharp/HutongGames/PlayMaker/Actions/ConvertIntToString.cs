using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B7C RID: 2940
	[Tooltip("Converts an Integer value to a String value with an optional format.")]
	[ActionCategory(17)]
	public class ConvertIntToString : FsmStateAction
	{
		// Token: 0x06006369 RID: 25449 RVA: 0x001DAECF File Offset: 0x001D90CF
		public override void Reset()
		{
			this.intVariable = null;
			this.stringVariable = null;
			this.everyFrame = false;
			this.format = null;
		}

		// Token: 0x0600636A RID: 25450 RVA: 0x001DAEED File Offset: 0x001D90ED
		public override void OnEnter()
		{
			this.DoConvertIntToString();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600636B RID: 25451 RVA: 0x001DAF06 File Offset: 0x001D9106
		public override void OnUpdate()
		{
			this.DoConvertIntToString();
		}

		// Token: 0x0600636C RID: 25452 RVA: 0x001DAF10 File Offset: 0x001D9110
		private void DoConvertIntToString()
		{
			if (this.format.IsNone || string.IsNullOrEmpty(this.format.Value))
			{
				this.stringVariable.Value = this.intVariable.Value.ToString();
			}
			else
			{
				this.stringVariable.Value = this.intVariable.Value.ToString(this.format.Value);
			}
		}

		// Token: 0x04004AEE RID: 19182
		[Tooltip("The Int variable to convert.")]
		[RequiredField]
		[UIHint(10)]
		public FsmInt intVariable;

		// Token: 0x04004AEF RID: 19183
		[UIHint(10)]
		[RequiredField]
		[Tooltip("A String variable to store the converted value.")]
		public FsmString stringVariable;

		// Token: 0x04004AF0 RID: 19184
		[Tooltip("Optional Format, allows for leading zeroes. E.g., 0000")]
		public FsmString format;

		// Token: 0x04004AF1 RID: 19185
		[Tooltip("Repeat every frame. Useful if the Int variable is changing.")]
		public bool everyFrame;
	}
}
