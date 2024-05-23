using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CFA RID: 3322
	[ActionCategory(27)]
	[Tooltip("Tests if a String contains another String.")]
	public class StringContains : FsmStateAction
	{
		// Token: 0x0600699C RID: 27036 RVA: 0x001EF080 File Offset: 0x001ED280
		public override void Reset()
		{
			this.stringVariable = null;
			this.containsString = string.Empty;
			this.trueEvent = null;
			this.falseEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x0600699D RID: 27037 RVA: 0x001EF0B5 File Offset: 0x001ED2B5
		public override void OnEnter()
		{
			this.DoStringContains();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600699E RID: 27038 RVA: 0x001EF0CE File Offset: 0x001ED2CE
		public override void OnUpdate()
		{
			this.DoStringContains();
		}

		// Token: 0x0600699F RID: 27039 RVA: 0x001EF0D8 File Offset: 0x001ED2D8
		private void DoStringContains()
		{
			if (this.stringVariable.IsNone || this.containsString.IsNone)
			{
				return;
			}
			bool flag = this.stringVariable.Value.Contains(this.containsString.Value);
			if (this.storeResult != null)
			{
				this.storeResult.Value = flag;
			}
			if (flag && this.trueEvent != null)
			{
				base.Fsm.Event(this.trueEvent);
				return;
			}
			if (!flag && this.falseEvent != null)
			{
				base.Fsm.Event(this.falseEvent);
			}
		}

		// Token: 0x04005155 RID: 20821
		[Tooltip("The String variable to test.")]
		[RequiredField]
		[UIHint(10)]
		public FsmString stringVariable;

		// Token: 0x04005156 RID: 20822
		[Tooltip("Test if the String variable contains this string.")]
		[RequiredField]
		public FsmString containsString;

		// Token: 0x04005157 RID: 20823
		[Tooltip("Event to send if true.")]
		public FsmEvent trueEvent;

		// Token: 0x04005158 RID: 20824
		[Tooltip("Event to send if false.")]
		public FsmEvent falseEvent;

		// Token: 0x04005159 RID: 20825
		[UIHint(10)]
		[Tooltip("Store the true/false result in a bool variable.")]
		public FsmBool storeResult;

		// Token: 0x0400515A RID: 20826
		[Tooltip("Repeat every frame. Useful if any of the strings are changing over time.")]
		public bool everyFrame;
	}
}
