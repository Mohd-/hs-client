using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D96 RID: 3478
	[ActionCategory("Pegasus")]
	[Tooltip("Send an event based on an Actor's Card's rarity.")]
	public class ActorRarityEventAction : ActorAction
	{
		// Token: 0x06006C6D RID: 27757 RVA: 0x001FE582 File Offset: 0x001FC782
		protected override GameObject GetActorOwner()
		{
			return base.Fsm.GetOwnerDefaultTarget(this.m_ActorObject);
		}

		// Token: 0x06006C6E RID: 27758 RVA: 0x001FE598 File Offset: 0x001FC798
		public override void Reset()
		{
			this.m_ActorObject = null;
			this.m_FreeEvent = null;
			this.m_CommonEvent = null;
			this.m_RareEvent = null;
			this.m_EpicEvent = null;
			this.m_LegendaryEvent = null;
		}

		// Token: 0x06006C6F RID: 27759 RVA: 0x001FE5D0 File Offset: 0x001FC7D0
		public override void OnEnter()
		{
			base.OnEnter();
			if (this.m_actor == null)
			{
				base.Finish();
				return;
			}
			TAG_RARITY rarity = this.m_actor.GetRarity();
			switch (rarity)
			{
			case TAG_RARITY.COMMON:
				base.Fsm.Event(this.m_CommonEvent);
				break;
			case TAG_RARITY.FREE:
				base.Fsm.Event(this.m_FreeEvent);
				break;
			case TAG_RARITY.RARE:
				base.Fsm.Event(this.m_RareEvent);
				break;
			case TAG_RARITY.EPIC:
				base.Fsm.Event(this.m_EpicEvent);
				break;
			case TAG_RARITY.LEGENDARY:
				base.Fsm.Event(this.m_LegendaryEvent);
				break;
			default:
				Debug.LogError(string.Format("ActorRarityEventAction.OnEnter() - unknown rarity {0} on actor {1}", rarity, this.m_actor));
				break;
			}
			base.Finish();
		}

		// Token: 0x04005506 RID: 21766
		public FsmOwnerDefault m_ActorObject;

		// Token: 0x04005507 RID: 21767
		public FsmEvent m_FreeEvent;

		// Token: 0x04005508 RID: 21768
		public FsmEvent m_CommonEvent;

		// Token: 0x04005509 RID: 21769
		public FsmEvent m_RareEvent;

		// Token: 0x0400550A RID: 21770
		public FsmEvent m_EpicEvent;

		// Token: 0x0400550B RID: 21771
		public FsmEvent m_LegendaryEvent;
	}
}
