using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B8A RID: 2954
	[Tooltip("Sends a log message to the PlayMaker Log Window.")]
	[ActionCategory(2)]
	public class DebugLog : FsmStateAction
	{
		// Token: 0x0600639C RID: 25500 RVA: 0x001DB7BD File Offset: 0x001D99BD
		public override void Reset()
		{
			this.logLevel = 0;
			this.text = string.Empty;
		}

		// Token: 0x0600639D RID: 25501 RVA: 0x001DB7D8 File Offset: 0x001D99D8
		public override void OnEnter()
		{
			if (!string.IsNullOrEmpty(this.text.Value))
			{
				ActionHelpers.DebugLog(base.Fsm, this.logLevel, this.text.Value);
			}
			base.Finish();
		}

		// Token: 0x04004B1C RID: 19228
		[Tooltip("Info, Warning, or Error.")]
		public LogLevel logLevel;

		// Token: 0x04004B1D RID: 19229
		[Tooltip("Text to print to the PlayMaker log window.")]
		public FsmString text;
	}
}
