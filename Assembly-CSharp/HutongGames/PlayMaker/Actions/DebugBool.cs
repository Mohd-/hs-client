using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B83 RID: 2947
	[ActionCategory(2)]
	[Tooltip("Logs the value of a Bool Variable in the PlayMaker Log Window.")]
	public class DebugBool : FsmStateAction
	{
		// Token: 0x0600638A RID: 25482 RVA: 0x001DB467 File Offset: 0x001D9667
		public override void Reset()
		{
			this.logLevel = 0;
			this.boolVariable = null;
		}

		// Token: 0x0600638B RID: 25483 RVA: 0x001DB478 File Offset: 0x001D9678
		public override void OnEnter()
		{
			string text = "None";
			if (!this.boolVariable.IsNone)
			{
				text = this.boolVariable.Name + ": " + this.boolVariable.Value;
			}
			ActionHelpers.DebugLog(base.Fsm, this.logLevel, text);
			base.Finish();
		}

		// Token: 0x04004B08 RID: 19208
		[Tooltip("Info, Warning, or Error.")]
		public LogLevel logLevel;

		// Token: 0x04004B09 RID: 19209
		[UIHint(10)]
		[Tooltip("Prints the value of a Bool variable in the PlayMaker log window.")]
		public FsmBool boolVariable;
	}
}
