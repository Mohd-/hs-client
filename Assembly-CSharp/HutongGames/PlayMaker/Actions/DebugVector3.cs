using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B8C RID: 2956
	[ActionCategory(2)]
	[Tooltip("Logs the value of a Vector3 Variable in the PlayMaker Log Window.")]
	public class DebugVector3 : FsmStateAction
	{
		// Token: 0x060063A2 RID: 25506 RVA: 0x001DB893 File Offset: 0x001D9A93
		public override void Reset()
		{
			this.logLevel = 0;
			this.vector3Variable = null;
		}

		// Token: 0x060063A3 RID: 25507 RVA: 0x001DB8A4 File Offset: 0x001D9AA4
		public override void OnEnter()
		{
			string text = "None";
			if (!this.vector3Variable.IsNone)
			{
				text = this.vector3Variable.Name + ": " + this.vector3Variable.Value;
			}
			ActionHelpers.DebugLog(base.Fsm, this.logLevel, text);
			base.Finish();
		}

		// Token: 0x04004B20 RID: 19232
		[Tooltip("Info, Warning, or Error.")]
		public LogLevel logLevel;

		// Token: 0x04004B21 RID: 19233
		[Tooltip("Prints the value of a Vector3 variable in the PlayMaker log window.")]
		[UIHint(10)]
		public FsmVector3 vector3Variable;
	}
}
