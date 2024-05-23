using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C12 RID: 3090
	[ActionCategory(12)]
	[Tooltip("Gets the event that caused the transition to the current state, and stores it in a String Variable.")]
	public class GetLastEvent : FsmStateAction
	{
		// Token: 0x060065A9 RID: 26025 RVA: 0x001E26CC File Offset: 0x001E08CC
		public override void Reset()
		{
			this.storeEvent = null;
		}

		// Token: 0x060065AA RID: 26026 RVA: 0x001E26D8 File Offset: 0x001E08D8
		public override void OnEnter()
		{
			this.storeEvent.Value = ((base.Fsm.LastTransition != null) ? base.Fsm.LastTransition.EventName : "START");
			base.Finish();
		}

		// Token: 0x04004D79 RID: 19833
		[UIHint(10)]
		public FsmString storeEvent;
	}
}
