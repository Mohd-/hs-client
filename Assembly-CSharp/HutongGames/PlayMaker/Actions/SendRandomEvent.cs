using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C91 RID: 3217
	[Tooltip("Sends a Random Event picked from an array of Events. Optionally set the relative weight of each event.")]
	[ActionCategory(12)]
	public class SendRandomEvent : FsmStateAction
	{
		// Token: 0x060067CA RID: 26570 RVA: 0x001E965C File Offset: 0x001E785C
		public override void Reset()
		{
			this.events = new FsmEvent[3];
			this.weights = new FsmFloat[]
			{
				1f,
				1f,
				1f
			};
			this.delay = null;
		}

		// Token: 0x060067CB RID: 26571 RVA: 0x001E96B0 File Offset: 0x001E78B0
		public override void OnEnter()
		{
			if (this.events.Length > 0)
			{
				int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
				if (randomWeightedIndex != -1)
				{
					if (this.delay.Value < 0.001f)
					{
						base.Fsm.Event(this.events[randomWeightedIndex]);
						base.Finish();
					}
					else
					{
						this.delayedEvent = base.Fsm.DelayedEvent(this.events[randomWeightedIndex], this.delay.Value);
					}
					return;
				}
			}
			base.Finish();
		}

		// Token: 0x060067CC RID: 26572 RVA: 0x001E973C File Offset: 0x001E793C
		public override void OnUpdate()
		{
			if (DelayedEvent.WasSent(this.delayedEvent))
			{
				base.Finish();
			}
		}

		// Token: 0x04004FA6 RID: 20390
		[CompoundArray("Events", "Event", "Weight")]
		public FsmEvent[] events;

		// Token: 0x04004FA7 RID: 20391
		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] weights;

		// Token: 0x04004FA8 RID: 20392
		public FsmFloat delay;

		// Token: 0x04004FA9 RID: 20393
		private DelayedEvent delayedEvent;
	}
}
