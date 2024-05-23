using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C50 RID: 3152
	[Tooltip("Kill all queued delayed events. Delayed events are 'fire and forget', but sometimes this can cause problems.")]
	[Note("Kill all queued delayed events.")]
	[ActionCategory(12)]
	public class KillDelayedEvents : FsmStateAction
	{
		// Token: 0x060066B0 RID: 26288 RVA: 0x001E50E9 File Offset: 0x001E32E9
		public override void OnEnter()
		{
			base.Fsm.KillDelayedEvents();
			base.Finish();
		}
	}
}
