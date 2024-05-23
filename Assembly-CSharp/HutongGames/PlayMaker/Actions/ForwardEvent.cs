using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BB6 RID: 2998
	[ActionCategory(12)]
	[Tooltip("Forward an event recieved by this FSM to another target.")]
	public class ForwardEvent : FsmStateAction
	{
		// Token: 0x0600644C RID: 25676 RVA: 0x001DD724 File Offset: 0x001DB924
		public override void Reset()
		{
			this.forwardTo = new FsmEventTarget
			{
				target = 3
			};
			this.eventsToForward = null;
			this.eatEvents = true;
		}

		// Token: 0x0600644D RID: 25677 RVA: 0x001DD754 File Offset: 0x001DB954
		public override bool Event(FsmEvent fsmEvent)
		{
			if (this.eventsToForward != null)
			{
				foreach (FsmEvent fsmEvent2 in this.eventsToForward)
				{
					if (fsmEvent2 == fsmEvent)
					{
						base.Fsm.Event(this.forwardTo, fsmEvent);
						return this.eatEvents;
					}
				}
			}
			return false;
		}

		// Token: 0x04004BC0 RID: 19392
		[Tooltip("Forward to this target.")]
		public FsmEventTarget forwardTo;

		// Token: 0x04004BC1 RID: 19393
		[Tooltip("The events to forward.")]
		public FsmEvent[] eventsToForward;

		// Token: 0x04004BC2 RID: 19394
		[Tooltip("Should this action eat the events or pass them on.")]
		public bool eatEvents;
	}
}
