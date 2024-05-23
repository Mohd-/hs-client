using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CF6 RID: 3318
	[ActionCategory(33)]
	[Tooltip("Stops location service updates. This could be useful for saving battery life.")]
	public class StopLocationServiceUpdates : FsmStateAction
	{
		// Token: 0x0600698D RID: 27021 RVA: 0x001EEE9A File Offset: 0x001ED09A
		public override void Reset()
		{
		}

		// Token: 0x0600698E RID: 27022 RVA: 0x001EEE9C File Offset: 0x001ED09C
		public override void OnEnter()
		{
			base.Finish();
		}
	}
}
