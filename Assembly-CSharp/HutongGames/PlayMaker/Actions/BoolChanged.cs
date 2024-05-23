using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B60 RID: 2912
	[Tooltip("Tests if the value of a Bool Variable has changed. Use this to send an event on change, or store a bool that can be used in other operations.")]
	[ActionCategory(27)]
	public class BoolChanged : FsmStateAction
	{
		// Token: 0x060062E9 RID: 25321 RVA: 0x001D9124 File Offset: 0x001D7324
		public override void Reset()
		{
			this.boolVariable = null;
			this.changedEvent = null;
			this.storeResult = null;
		}

		// Token: 0x060062EA RID: 25322 RVA: 0x001D913B File Offset: 0x001D733B
		public override void OnEnter()
		{
			if (this.boolVariable.IsNone)
			{
				base.Finish();
				return;
			}
			this.previousValue = this.boolVariable.Value;
		}

		// Token: 0x060062EB RID: 25323 RVA: 0x001D9168 File Offset: 0x001D7368
		public override void OnUpdate()
		{
			this.storeResult.Value = false;
			if (this.boolVariable.Value != this.previousValue)
			{
				this.storeResult.Value = true;
				base.Fsm.Event(this.changedEvent);
			}
		}

		// Token: 0x04004A5F RID: 19039
		[RequiredField]
		[Tooltip("The Bool variable to watch for changes.")]
		[UIHint(10)]
		public FsmBool boolVariable;

		// Token: 0x04004A60 RID: 19040
		[Tooltip("Event to send if the variable changes.")]
		public FsmEvent changedEvent;

		// Token: 0x04004A61 RID: 19041
		[UIHint(10)]
		[Tooltip("Set to True if changed.")]
		public FsmBool storeResult;

		// Token: 0x04004A62 RID: 19042
		private bool previousValue;
	}
}
