using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B86 RID: 2950
	[ActionCategory(2)]
	[Tooltip("Logs the value of a Float Variable in the PlayMaker Log Window.")]
	public class DebugFloat : FsmStateAction
	{
		// Token: 0x06006390 RID: 25488 RVA: 0x001DB61C File Offset: 0x001D981C
		public override void Reset()
		{
			this.logLevel = 0;
			this.floatVariable = null;
		}

		// Token: 0x06006391 RID: 25489 RVA: 0x001DB62C File Offset: 0x001D982C
		public override void OnEnter()
		{
			string text = "None";
			if (!this.floatVariable.IsNone)
			{
				text = this.floatVariable.Name + ": " + this.floatVariable.Value;
			}
			ActionHelpers.DebugLog(base.Fsm, this.logLevel, text);
			base.Finish();
		}

		// Token: 0x04004B14 RID: 19220
		[Tooltip("Info, Warning, or Error.")]
		public LogLevel logLevel;

		// Token: 0x04004B15 RID: 19221
		[Tooltip("Prints the value of a Float variable in the PlayMaker log window.")]
		[UIHint(10)]
		public FsmFloat floatVariable;
	}
}
