using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C45 RID: 3141
	[ActionCategory(27)]
	[Tooltip("Tests if the value of an integer variable changed. Use this to send an event on change, or store a bool that can be used in other operations.")]
	public class IntChanged : FsmStateAction
	{
		// Token: 0x0600667E RID: 26238 RVA: 0x001E487F File Offset: 0x001E2A7F
		public override void Reset()
		{
			this.intVariable = null;
			this.changedEvent = null;
			this.storeResult = null;
		}

		// Token: 0x0600667F RID: 26239 RVA: 0x001E4896 File Offset: 0x001E2A96
		public override void OnEnter()
		{
			if (this.intVariable.IsNone)
			{
				base.Finish();
				return;
			}
			this.previousValue = this.intVariable.Value;
		}

		// Token: 0x06006680 RID: 26240 RVA: 0x001E48C0 File Offset: 0x001E2AC0
		public override void OnUpdate()
		{
			this.storeResult.Value = false;
			if (this.intVariable.Value != this.previousValue)
			{
				this.previousValue = this.intVariable.Value;
				this.storeResult.Value = true;
				base.Fsm.Event(this.changedEvent);
			}
		}

		// Token: 0x04004E31 RID: 20017
		[RequiredField]
		[UIHint(10)]
		public FsmInt intVariable;

		// Token: 0x04004E32 RID: 20018
		public FsmEvent changedEvent;

		// Token: 0x04004E33 RID: 20019
		[UIHint(10)]
		public FsmBool storeResult;

		// Token: 0x04004E34 RID: 20020
		private int previousValue;
	}
}
