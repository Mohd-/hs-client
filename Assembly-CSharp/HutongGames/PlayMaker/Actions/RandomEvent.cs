using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C75 RID: 3189
	[Tooltip("Sends a Random State Event after an optional delay. Use this to transition to a random state from the current state.")]
	[ActionCategory(12)]
	public class RandomEvent : FsmStateAction
	{
		// Token: 0x0600674C RID: 26444 RVA: 0x001E774C File Offset: 0x001E594C
		public override void Reset()
		{
			this.delay = null;
		}

		// Token: 0x0600674D RID: 26445 RVA: 0x001E7758 File Offset: 0x001E5958
		public override void OnEnter()
		{
			if (base.State.Transitions.Length == 0)
			{
				return;
			}
			if (this.lastEventIndex == -1)
			{
				this.lastEventIndex = Random.Range(0, base.State.Transitions.Length);
			}
			if (this.delay.Value < 0.001f)
			{
				base.Fsm.Event(this.GetRandomEvent());
				base.Finish();
			}
			else
			{
				this.delayedEvent = base.Fsm.DelayedEvent(this.GetRandomEvent(), this.delay.Value);
			}
		}

		// Token: 0x0600674E RID: 26446 RVA: 0x001E77F0 File Offset: 0x001E59F0
		public override void OnUpdate()
		{
			if (DelayedEvent.WasSent(this.delayedEvent))
			{
				base.Finish();
			}
		}

		// Token: 0x0600674F RID: 26447 RVA: 0x001E7808 File Offset: 0x001E5A08
		private FsmEvent GetRandomEvent()
		{
			do
			{
				this.randomEventIndex = Random.Range(0, base.State.Transitions.Length);
			}
			while (this.noRepeat.Value && base.State.Transitions.Length > 1 && this.randomEventIndex == this.lastEventIndex);
			this.lastEventIndex = this.randomEventIndex;
			return base.State.Transitions[this.randomEventIndex].FsmEvent;
		}

		// Token: 0x04004F19 RID: 20249
		[HasFloatSlider(0f, 10f)]
		[Tooltip("Delay before sending the event.")]
		public FsmFloat delay;

		// Token: 0x04004F1A RID: 20250
		[Tooltip("Don't repeat the same event twice in a row.")]
		public FsmBool noRepeat;

		// Token: 0x04004F1B RID: 20251
		private DelayedEvent delayedEvent;

		// Token: 0x04004F1C RID: 20252
		private int randomEventIndex;

		// Token: 0x04004F1D RID: 20253
		private int lastEventIndex = -1;
	}
}
