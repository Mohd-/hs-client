using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DB4 RID: 3508
	[Tooltip("Trigger Event by name after an optional delay. NOTE: Use this over Send Event if you store events as string variables.")]
	[ActionCategory("Pegasus")]
	public class EventByNameAction : FsmStateAction
	{
		// Token: 0x06006CEB RID: 27883 RVA: 0x00200C47 File Offset: 0x001FEE47
		public override void Reset()
		{
			this.sendEvent = null;
			this.delay = null;
			this.fallbackEvent = null;
			this.everyFrame = false;
		}

		// Token: 0x06006CEC RID: 27884 RVA: 0x00200C68 File Offset: 0x001FEE68
		public override void OnEnter()
		{
			string text = "FINISHED";
			if (this.fallbackEvent != null)
			{
				text = this.fallbackEvent.Value;
			}
			FsmState state = base.State;
			if (state != null)
			{
				FsmTransition[] transitions = state.Transitions;
				foreach (FsmTransition fsmTransition in transitions)
				{
					if (fsmTransition.EventName == this.sendEvent.Value)
					{
						text = this.sendEvent.Value;
					}
				}
			}
			if (this.delay.Value < 0.001f)
			{
				base.Fsm.Event(text);
				base.Finish();
			}
			else
			{
				this.delayedEvent = base.Fsm.DelayedEvent(FsmEvent.GetFsmEvent(text), this.delay.Value);
			}
		}

		// Token: 0x06006CED RID: 27885 RVA: 0x00200D3D File Offset: 0x001FEF3D
		public override void OnUpdate()
		{
			if (!this.everyFrame && DelayedEvent.WasSent(this.delayedEvent))
			{
				base.Finish();
			}
		}

		// Token: 0x040055A2 RID: 21922
		[Tooltip("The event to send. NOTE: Events must be marked Global to send between FSMs.")]
		[RequiredField]
		public FsmString sendEvent;

		// Token: 0x040055A3 RID: 21923
		[Tooltip("Optional delay in seconds.")]
		[HasFloatSlider(0f, 10f)]
		public FsmFloat delay;

		// Token: 0x040055A4 RID: 21924
		[Tooltip("The event to send if the send event is not found. NOTE: Events must be marked Global to send between FSMs.")]
		[RequiredField]
		public FsmString fallbackEvent;

		// Token: 0x040055A5 RID: 21925
		[Tooltip("Repeat every frame. Rarely needed.")]
		public bool everyFrame;

		// Token: 0x040055A6 RID: 21926
		private DelayedEvent delayedEvent;
	}
}
