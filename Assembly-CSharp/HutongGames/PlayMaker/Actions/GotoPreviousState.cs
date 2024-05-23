using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C41 RID: 3137
	[ActionCategory(12)]
	[Tooltip("Immediately return to the previously active state.")]
	public class GotoPreviousState : FsmStateAction
	{
		// Token: 0x0600666E RID: 26222 RVA: 0x001E4650 File Offset: 0x001E2850
		public override void Reset()
		{
		}

		// Token: 0x0600666F RID: 26223 RVA: 0x001E4654 File Offset: 0x001E2854
		public override void OnEnter()
		{
			if (base.Fsm.PreviousActiveState != null)
			{
				this.Log("Goto Previous State: " + base.Fsm.PreviousActiveState.Name);
				base.Fsm.GotoPreviousState();
			}
			base.Finish();
		}
	}
}
