using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C8B RID: 3211
	[Tooltip("Sends an Event after an optional delay. NOTE: To send events between FSMs they must be marked as Global in the Events Browser.")]
	[ActionCategory(12)]
	public class SendEvent : FsmStateAction
	{
		// Token: 0x060067B6 RID: 26550 RVA: 0x001E8FA2 File Offset: 0x001E71A2
		public override void Reset()
		{
			this.eventTarget = null;
			this.sendEvent = null;
			this.delay = null;
			this.everyFrame = false;
		}

		// Token: 0x060067B7 RID: 26551 RVA: 0x001E8FC0 File Offset: 0x001E71C0
		public override void OnEnter()
		{
			if (this.delay.Value < 0.001f)
			{
				base.Fsm.Event(this.eventTarget, this.sendEvent);
				base.Finish();
			}
			else
			{
				this.delayedEvent = base.Fsm.DelayedEvent(this.eventTarget, this.sendEvent, this.delay.Value);
			}
		}

		// Token: 0x060067B8 RID: 26552 RVA: 0x001E902C File Offset: 0x001E722C
		public override void OnUpdate()
		{
			if (!this.everyFrame && DelayedEvent.WasSent(this.delayedEvent))
			{
				base.Finish();
			}
		}

		// Token: 0x04004F88 RID: 20360
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04004F89 RID: 20361
		[RequiredField]
		[Tooltip("The event to send. NOTE: Events must be marked Global to send between FSMs.")]
		public FsmEvent sendEvent;

		// Token: 0x04004F8A RID: 20362
		[Tooltip("Optional delay in seconds.")]
		[HasFloatSlider(0f, 10f)]
		public FsmFloat delay;

		// Token: 0x04004F8B RID: 20363
		[Tooltip("Repeat every frame. Rarely needed.")]
		public bool everyFrame;

		// Token: 0x04004F8C RID: 20364
		private DelayedEvent delayedEvent;
	}
}
