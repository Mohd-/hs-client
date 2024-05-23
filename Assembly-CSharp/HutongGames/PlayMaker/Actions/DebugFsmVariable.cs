using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B87 RID: 2951
	[ActionCategory(2)]
	[Tooltip("Print the value of an FSM Variable in the PlayMaker Log Window.")]
	public class DebugFsmVariable : FsmStateAction
	{
		// Token: 0x06006393 RID: 25491 RVA: 0x001DB695 File Offset: 0x001D9895
		public override void Reset()
		{
			this.logLevel = 0;
			this.fsmVar = null;
		}

		// Token: 0x06006394 RID: 25492 RVA: 0x001DB6A5 File Offset: 0x001D98A5
		public override void OnEnter()
		{
			ActionHelpers.DebugLog(base.Fsm, this.logLevel, this.fsmVar.DebugString());
			base.Finish();
		}

		// Token: 0x04004B16 RID: 19222
		[Tooltip("Info, Warning, or Error.")]
		public LogLevel logLevel;

		// Token: 0x04004B17 RID: 19223
		[UIHint(10)]
		[HideTypeFilter]
		[Tooltip("Variable to print to the PlayMaker log window.")]
		public FsmVar fsmVar;
	}
}
