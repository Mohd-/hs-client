using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BA4 RID: 2980
	[Tooltip("Stop this FSM. If this FSM was launched by a Run FSM action, it will trigger a Finish event in that state.")]
	[Note("Stop this FSM. If this FSM was launched by a Run FSM action, it will trigger a Finish event in that state.")]
	[ActionCategory(12)]
	public class FinishFSM : FsmStateAction
	{
		// Token: 0x060063FF RID: 25599 RVA: 0x001DCA7C File Offset: 0x001DAC7C
		public override void OnEnter()
		{
			base.Fsm.Stop();
		}
	}
}
