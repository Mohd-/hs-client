using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B89 RID: 2953
	[Tooltip("Logs the value of an Integer Variable in the PlayMaker Log Window.")]
	[ActionCategory(2)]
	public class DebugInt : FsmStateAction
	{
		// Token: 0x06006399 RID: 25497 RVA: 0x001DB743 File Offset: 0x001D9943
		public override void Reset()
		{
			this.logLevel = 0;
			this.intVariable = null;
		}

		// Token: 0x0600639A RID: 25498 RVA: 0x001DB754 File Offset: 0x001D9954
		public override void OnEnter()
		{
			string text = "None";
			if (!this.intVariable.IsNone)
			{
				text = this.intVariable.Name + ": " + this.intVariable.Value;
			}
			ActionHelpers.DebugLog(base.Fsm, this.logLevel, text);
			base.Finish();
		}

		// Token: 0x04004B1A RID: 19226
		[Tooltip("Info, Warning, or Error.")]
		public LogLevel logLevel;

		// Token: 0x04004B1B RID: 19227
		[UIHint(10)]
		[Tooltip("Prints the value of an Int variable in the PlayMaker log window.")]
		public FsmInt intVariable;
	}
}
