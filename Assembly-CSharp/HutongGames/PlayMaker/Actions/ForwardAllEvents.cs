using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BB5 RID: 2997
	[ActionCategory(12)]
	[Tooltip("Forwards all event recieved by this FSM to another target. Optionally specify a list of events to ignore.")]
	public class ForwardAllEvents : FsmStateAction
	{
		// Token: 0x06006449 RID: 25673 RVA: 0x001DD688 File Offset: 0x001DB888
		public override void Reset()
		{
			this.forwardTo = new FsmEventTarget
			{
				target = 3
			};
			this.exceptThese = new FsmEvent[]
			{
				FsmEvent.Finished
			};
			this.eatEvents = true;
		}

		// Token: 0x0600644A RID: 25674 RVA: 0x001DD6C4 File Offset: 0x001DB8C4
		public override bool Event(FsmEvent fsmEvent)
		{
			if (this.exceptThese != null)
			{
				foreach (FsmEvent fsmEvent2 in this.exceptThese)
				{
					if (fsmEvent2 == fsmEvent)
					{
						return false;
					}
				}
			}
			base.Fsm.Event(this.forwardTo, fsmEvent);
			return this.eatEvents;
		}

		// Token: 0x04004BBD RID: 19389
		[Tooltip("Forward to this target.")]
		public FsmEventTarget forwardTo;

		// Token: 0x04004BBE RID: 19390
		[Tooltip("Don't forward these events.")]
		public FsmEvent[] exceptThese;

		// Token: 0x04004BBF RID: 19391
		[Tooltip("Should this action eat the events or pass them on.")]
		public bool eatEvents;
	}
}
