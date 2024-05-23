using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BB4 RID: 2996
	[ActionCategory(16)]
	[Tooltip("Replaces each format item in a specified string with the text equivalent of variable's value. Stores the result in a string variable.")]
	public class FormatString : FsmStateAction
	{
		// Token: 0x06006444 RID: 25668 RVA: 0x001DD588 File Offset: 0x001DB788
		public override void Reset()
		{
			this.format = null;
			this.variables = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006445 RID: 25669 RVA: 0x001DD5A8 File Offset: 0x001DB7A8
		public override void OnEnter()
		{
			this.objectArray = new object[this.variables.Length];
			this.DoFormatString();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006446 RID: 25670 RVA: 0x001DD5DF File Offset: 0x001DB7DF
		public override void OnUpdate()
		{
			this.DoFormatString();
		}

		// Token: 0x06006447 RID: 25671 RVA: 0x001DD5E8 File Offset: 0x001DB7E8
		private void DoFormatString()
		{
			for (int i = 0; i < this.variables.Length; i++)
			{
				this.variables[i].UpdateValue();
				this.objectArray[i] = this.variables[i].GetValue();
			}
			try
			{
				this.storeResult.Value = string.Format(this.format.Value, this.objectArray);
			}
			catch (FormatException ex)
			{
				this.LogError(ex.Message);
				base.Finish();
			}
		}

		// Token: 0x04004BB8 RID: 19384
		[Tooltip("E.g. Hello {0} and {1}\nWith 2 variables that replace {0} and {1}\nSee C# string.Format docs.")]
		[RequiredField]
		public FsmString format;

		// Token: 0x04004BB9 RID: 19385
		[Tooltip("Variables to use for each formatting item.")]
		public FsmVar[] variables;

		// Token: 0x04004BBA RID: 19386
		[UIHint(10)]
		[RequiredField]
		[Tooltip("Store the formatted result in a string variable.")]
		public FsmString storeResult;

		// Token: 0x04004BBB RID: 19387
		[Tooltip("Repeat every frame. This is useful if the variables are changing.")]
		public bool everyFrame;

		// Token: 0x04004BBC RID: 19388
		private object[] objectArray;
	}
}
