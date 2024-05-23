using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CA4 RID: 3236
	[ActionCategory(12)]
	[Tooltip("Sets the target FSM for all subsequent events sent by this state. The default 'Self' sends events to this FSM.")]
	public class SetEventTarget : FsmStateAction
	{
		// Token: 0x0600681F RID: 26655 RVA: 0x001EA534 File Offset: 0x001E8734
		public override void Reset()
		{
		}

		// Token: 0x06006820 RID: 26656 RVA: 0x001EA536 File Offset: 0x001E8736
		public override void OnEnter()
		{
			base.Fsm.EventTarget = this.eventTarget;
			base.Finish();
		}

		// Token: 0x04004FEE RID: 20462
		public FsmEventTarget eventTarget;
	}
}
