using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CF8 RID: 3320
	[ActionCategory(27)]
	[Tooltip("Tests if the value of a string variable has changed. Use this to send an event on change, or store a bool that can be used in other operations.")]
	public class StringChanged : FsmStateAction
	{
		// Token: 0x06006993 RID: 27027 RVA: 0x001EEEF6 File Offset: 0x001ED0F6
		public override void Reset()
		{
			this.stringVariable = null;
			this.changedEvent = null;
			this.storeResult = null;
		}

		// Token: 0x06006994 RID: 27028 RVA: 0x001EEF0D File Offset: 0x001ED10D
		public override void OnEnter()
		{
			if (this.stringVariable.IsNone)
			{
				base.Finish();
				return;
			}
			this.previousValue = this.stringVariable.Value;
		}

		// Token: 0x06006995 RID: 27029 RVA: 0x001EEF38 File Offset: 0x001ED138
		public override void OnUpdate()
		{
			if (this.stringVariable.Value != this.previousValue)
			{
				this.storeResult.Value = true;
				base.Fsm.Event(this.changedEvent);
			}
		}

		// Token: 0x0400514B RID: 20811
		[RequiredField]
		[UIHint(10)]
		public FsmString stringVariable;

		// Token: 0x0400514C RID: 20812
		public FsmEvent changedEvent;

		// Token: 0x0400514D RID: 20813
		[UIHint(10)]
		public FsmBool storeResult;

		// Token: 0x0400514E RID: 20814
		private string previousValue;
	}
}
