using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B7E RID: 2942
	[ActionCategory(17)]
	[Tooltip("Converts an String value to an Int value.")]
	public class ConvertStringToInt : FsmStateAction
	{
		// Token: 0x06006373 RID: 25459 RVA: 0x001DAFEE File Offset: 0x001D91EE
		public override void Reset()
		{
			this.intVariable = null;
			this.stringVariable = null;
			this.everyFrame = false;
		}

		// Token: 0x06006374 RID: 25460 RVA: 0x001DB005 File Offset: 0x001D9205
		public override void OnEnter()
		{
			this.DoConvertStringToInt();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006375 RID: 25461 RVA: 0x001DB01E File Offset: 0x001D921E
		public override void OnUpdate()
		{
			this.DoConvertStringToInt();
		}

		// Token: 0x06006376 RID: 25462 RVA: 0x001DB026 File Offset: 0x001D9226
		private void DoConvertStringToInt()
		{
			this.intVariable.Value = int.Parse(this.stringVariable.Value);
		}

		// Token: 0x04004AF5 RID: 19189
		[UIHint(10)]
		[Tooltip("The String variable to convert to an integer.")]
		[RequiredField]
		public FsmString stringVariable;

		// Token: 0x04004AF6 RID: 19190
		[RequiredField]
		[UIHint(10)]
		[Tooltip("Store the result in an Int variable.")]
		public FsmInt intVariable;

		// Token: 0x04004AF7 RID: 19191
		[Tooltip("Repeat every frame. Useful if the String variable is changing.")]
		public bool everyFrame;
	}
}
