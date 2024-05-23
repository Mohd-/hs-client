using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C5C RID: 3164
	[Tooltip("Sends an Event in the next frame. Useful if you want to loop states every frame.")]
	[ActionCategory(12)]
	public class NextFrameEvent : FsmStateAction
	{
		// Token: 0x060066E7 RID: 26343 RVA: 0x001E616B File Offset: 0x001E436B
		public override void Reset()
		{
			this.sendEvent = null;
		}

		// Token: 0x060066E8 RID: 26344 RVA: 0x001E6174 File Offset: 0x001E4374
		public override void OnEnter()
		{
		}

		// Token: 0x060066E9 RID: 26345 RVA: 0x001E6176 File Offset: 0x001E4376
		public override void OnUpdate()
		{
			base.Finish();
			base.Fsm.Event(this.sendEvent);
		}

		// Token: 0x04004EBC RID: 20156
		[RequiredField]
		public FsmEvent sendEvent;
	}
}
