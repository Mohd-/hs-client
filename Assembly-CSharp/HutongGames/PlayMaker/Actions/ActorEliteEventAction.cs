using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D92 RID: 3474
	[Tooltip("Send an event based on an Actor's Card's elite flag.")]
	[ActionCategory("Pegasus")]
	public class ActorEliteEventAction : ActorAction
	{
		// Token: 0x06006C5D RID: 27741 RVA: 0x001FE290 File Offset: 0x001FC490
		protected override GameObject GetActorOwner()
		{
			return base.Fsm.GetOwnerDefaultTarget(this.m_ActorObject);
		}

		// Token: 0x06006C5E RID: 27742 RVA: 0x001FE2A3 File Offset: 0x001FC4A3
		public override void Reset()
		{
			this.m_ActorObject = null;
			this.m_EliteEvent = null;
			this.m_NonEliteEvent = null;
		}

		// Token: 0x06006C5F RID: 27743 RVA: 0x001FE2BC File Offset: 0x001FC4BC
		public override void OnEnter()
		{
			base.OnEnter();
			if (this.m_actor == null)
			{
				base.Finish();
				return;
			}
			if (this.m_actor.IsElite())
			{
				base.Fsm.Event(this.m_EliteEvent);
			}
			else
			{
				base.Fsm.Event(this.m_NonEliteEvent);
			}
			base.Finish();
		}

		// Token: 0x040054FA RID: 21754
		public FsmOwnerDefault m_ActorObject;

		// Token: 0x040054FB RID: 21755
		public FsmEvent m_EliteEvent;

		// Token: 0x040054FC RID: 21756
		public FsmEvent m_NonEliteEvent;
	}
}
