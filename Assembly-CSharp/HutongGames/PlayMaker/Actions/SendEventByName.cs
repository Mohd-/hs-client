using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C8D RID: 3213
	[ActionCategory(12)]
	[Tooltip("Sends an Event by name after an optional delay. NOTE: Use this over Send Event if you store events as string variables.")]
	public class SendEventByName : FsmStateAction
	{
		// Token: 0x060067BE RID: 26558 RVA: 0x001E9109 File Offset: 0x001E7309
		public override void Reset()
		{
			this.eventTarget = null;
			this.sendEvent = null;
			this.delay = null;
			this.everyFrame = false;
		}

		// Token: 0x060067BF RID: 26559 RVA: 0x001E9128 File Offset: 0x001E7328
		public override void OnEnter()
		{
			if (this.delay.Value < 0.001f)
			{
				base.Fsm.Event(this.eventTarget, this.sendEvent.Value);
				base.Finish();
			}
			else
			{
				this.delayedEvent = base.Fsm.DelayedEvent(this.eventTarget, FsmEvent.GetFsmEvent(this.sendEvent.Value), this.delay.Value);
			}
		}

		// Token: 0x060067C0 RID: 26560 RVA: 0x001E91A3 File Offset: 0x001E73A3
		public override void OnUpdate()
		{
			if (!this.everyFrame && DelayedEvent.WasSent(this.delayedEvent))
			{
				base.Finish();
			}
		}

		// Token: 0x04004F91 RID: 20369
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04004F92 RID: 20370
		[Tooltip("The event to send. NOTE: Events must be marked Global to send between FSMs.")]
		[RequiredField]
		public FsmString sendEvent;

		// Token: 0x04004F93 RID: 20371
		[HasFloatSlider(0f, 10f)]
		[Tooltip("Optional delay in seconds.")]
		public FsmFloat delay;

		// Token: 0x04004F94 RID: 20372
		[Tooltip("Repeat every frame. Rarely needed.")]
		public bool everyFrame;

		// Token: 0x04004F95 RID: 20373
		private DelayedEvent delayedEvent;
	}
}
