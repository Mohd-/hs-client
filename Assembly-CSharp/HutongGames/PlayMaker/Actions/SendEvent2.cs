using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C8C RID: 3212
	[ActionCategory(12)]
	[Tooltip("Sends an Event after an optional delay. NOTE: To send events between FSMs they must be marked as Global in the Events Browser.")]
	public class SendEvent2 : FsmStateAction
	{
		// Token: 0x060067BA RID: 26554 RVA: 0x001E9057 File Offset: 0x001E7257
		public override void Reset()
		{
			this.sendEvent = null;
			this.delay = null;
		}

		// Token: 0x060067BB RID: 26555 RVA: 0x001E9068 File Offset: 0x001E7268
		public override void OnEnter()
		{
			if (this.delay.Value == 0f)
			{
				base.Fsm.Event(this.eventTarget, this.sendEvent.Value);
				base.Finish();
			}
			else
			{
				this.delayedEvent = new DelayedEvent(base.Fsm, this.eventTarget, this.sendEvent.Value, this.delay.Value);
			}
		}

		// Token: 0x060067BC RID: 26556 RVA: 0x001E90DE File Offset: 0x001E72DE
		public override void OnUpdate()
		{
			this.delayedEvent.Update();
			if (this.delayedEvent.Finished)
			{
				base.Finish();
			}
		}

		// Token: 0x04004F8D RID: 20365
		public FsmEventTarget eventTarget;

		// Token: 0x04004F8E RID: 20366
		[RequiredField]
		public FsmString sendEvent;

		// Token: 0x04004F8F RID: 20367
		[HasFloatSlider(0f, 10f)]
		public FsmFloat delay;

		// Token: 0x04004F90 RID: 20368
		private DelayedEvent delayedEvent;
	}
}
