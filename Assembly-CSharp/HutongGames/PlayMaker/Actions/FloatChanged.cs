using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BA9 RID: 2985
	[Tooltip("Tests if the value of a Float variable changed. Use this to send an event on change, or store a bool that can be used in other operations.")]
	[ActionCategory(27)]
	public class FloatChanged : FsmStateAction
	{
		// Token: 0x06006414 RID: 25620 RVA: 0x001DCD45 File Offset: 0x001DAF45
		public override void Reset()
		{
			this.floatVariable = null;
			this.changedEvent = null;
			this.storeResult = null;
		}

		// Token: 0x06006415 RID: 25621 RVA: 0x001DCD5C File Offset: 0x001DAF5C
		public override void OnEnter()
		{
			if (this.floatVariable.IsNone)
			{
				base.Finish();
				return;
			}
			this.previousValue = this.floatVariable.Value;
		}

		// Token: 0x06006416 RID: 25622 RVA: 0x001DCD88 File Offset: 0x001DAF88
		public override void OnUpdate()
		{
			this.storeResult.Value = false;
			if (this.floatVariable.Value != this.previousValue)
			{
				this.previousValue = this.floatVariable.Value;
				this.storeResult.Value = true;
				base.Fsm.Event(this.changedEvent);
			}
		}

		// Token: 0x04004B82 RID: 19330
		[RequiredField]
		[Tooltip("The Float variable to watch for a change.")]
		[UIHint(10)]
		public FsmFloat floatVariable;

		// Token: 0x04004B83 RID: 19331
		[Tooltip("Event to send if the float variable changes.")]
		public FsmEvent changedEvent;

		// Token: 0x04004B84 RID: 19332
		[UIHint(10)]
		[Tooltip("Set to True if the float variable changes.")]
		public FsmBool storeResult;

		// Token: 0x04004B85 RID: 19333
		private float previousValue;
	}
}
