using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D94 RID: 3476
	[ActionCategory("Pegasus")]
	[Tooltip("Send an event based on an Actor's Card's Golden state.")]
	public class ActorGoldEventAction : ActorAction
	{
		// Token: 0x06006C65 RID: 27749 RVA: 0x001FE470 File Offset: 0x001FC670
		protected override GameObject GetActorOwner()
		{
			return base.Fsm.GetOwnerDefaultTarget(this.m_ActorObject);
		}

		// Token: 0x06006C66 RID: 27750 RVA: 0x001FE483 File Offset: 0x001FC683
		public override void Reset()
		{
			this.m_ActorObject = null;
			this.m_GoldenCardEvent = null;
			this.m_StandardCardEvent = null;
		}

		// Token: 0x06006C67 RID: 27751 RVA: 0x001FE49C File Offset: 0x001FC69C
		public override void OnEnter()
		{
			base.OnEnter();
			if (this.m_actor == null)
			{
				base.Finish();
				return;
			}
			if (this.m_actor.GetPremium() == TAG_PREMIUM.GOLDEN)
			{
				base.Fsm.Event(this.m_GoldenCardEvent);
			}
			else
			{
				base.Fsm.Event(this.m_StandardCardEvent);
			}
			base.Finish();
		}

		// Token: 0x04005501 RID: 21761
		public FsmOwnerDefault m_ActorObject;

		// Token: 0x04005502 RID: 21762
		public FsmEvent m_GoldenCardEvent;

		// Token: 0x04005503 RID: 21763
		public FsmEvent m_StandardCardEvent;
	}
}
