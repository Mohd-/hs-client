using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B8B RID: 2955
	[ActionCategory(2)]
	[Tooltip("Logs the value of an Object Variable in the PlayMaker Log Window.")]
	public class DebugObject : FsmStateAction
	{
		// Token: 0x0600639F RID: 25503 RVA: 0x001DB824 File Offset: 0x001D9A24
		public override void Reset()
		{
			this.logLevel = 0;
			this.fsmObject = null;
		}

		// Token: 0x060063A0 RID: 25504 RVA: 0x001DB834 File Offset: 0x001D9A34
		public override void OnEnter()
		{
			string text = "None";
			if (!this.fsmObject.IsNone)
			{
				text = this.fsmObject.Name + ": " + this.fsmObject;
			}
			ActionHelpers.DebugLog(base.Fsm, this.logLevel, text);
			base.Finish();
		}

		// Token: 0x04004B1E RID: 19230
		[Tooltip("Info, Warning, or Error.")]
		public LogLevel logLevel;

		// Token: 0x04004B1F RID: 19231
		[Tooltip("Prints the value of an Object variable in the PlayMaker log window.")]
		[UIHint(10)]
		public FsmObject fsmObject;
	}
}
