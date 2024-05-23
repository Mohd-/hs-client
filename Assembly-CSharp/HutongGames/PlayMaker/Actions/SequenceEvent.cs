using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C92 RID: 3218
	[ActionCategory(12)]
	[Tooltip("Sends the next event on the state each time the state is entered.")]
	public class SequenceEvent : FsmStateAction
	{
		// Token: 0x060067CE RID: 26574 RVA: 0x001E975C File Offset: 0x001E795C
		public override void Reset()
		{
			this.delay = null;
		}

		// Token: 0x060067CF RID: 26575 RVA: 0x001E9768 File Offset: 0x001E7968
		public override void OnEnter()
		{
			int num = base.State.Transitions.Length;
			if (num > 0)
			{
				FsmEvent fsmEvent = base.State.Transitions[this.eventIndex].FsmEvent;
				if (this.delay.Value < 0.001f)
				{
					base.Fsm.Event(fsmEvent);
					base.Finish();
				}
				else
				{
					this.delayedEvent = base.Fsm.DelayedEvent(fsmEvent, this.delay.Value);
				}
				this.eventIndex++;
				if (this.eventIndex == num)
				{
					this.eventIndex = 0;
				}
			}
		}

		// Token: 0x060067D0 RID: 26576 RVA: 0x001E980C File Offset: 0x001E7A0C
		public override void OnUpdate()
		{
			if (DelayedEvent.WasSent(this.delayedEvent))
			{
				base.Finish();
			}
		}

		// Token: 0x04004FAA RID: 20394
		[HasFloatSlider(0f, 10f)]
		public FsmFloat delay;

		// Token: 0x04004FAB RID: 20395
		private DelayedEvent delayedEvent;

		// Token: 0x04004FAC RID: 20396
		private int eventIndex;
	}
}
