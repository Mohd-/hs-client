using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B88 RID: 2952
	[Tooltip("Logs the value of a Game Object Variable in the PlayMaker Log Window.")]
	[ActionCategory(2)]
	public class DebugGameObject : FsmStateAction
	{
		// Token: 0x06006396 RID: 25494 RVA: 0x001DB6D1 File Offset: 0x001D98D1
		public override void Reset()
		{
			this.logLevel = 0;
			this.gameObject = null;
		}

		// Token: 0x06006397 RID: 25495 RVA: 0x001DB6E4 File Offset: 0x001D98E4
		public override void OnEnter()
		{
			string text = "None";
			if (!this.gameObject.IsNone)
			{
				text = this.gameObject.Name + ": " + this.gameObject;
			}
			ActionHelpers.DebugLog(base.Fsm, this.logLevel, text);
			base.Finish();
		}

		// Token: 0x04004B18 RID: 19224
		[Tooltip("Info, Warning, or Error.")]
		public LogLevel logLevel;

		// Token: 0x04004B19 RID: 19225
		[UIHint(10)]
		[Tooltip("Prints the value of a GameObject variable in the PlayMaker log window.")]
		public FsmGameObject gameObject;
	}
}
