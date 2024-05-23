using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D91 RID: 3473
	[Tooltip("INTERNAL USE ONLY. Do not put this on your FSMs.")]
	[ActionCategory("Pegasus")]
	public abstract class ActorAction : FsmStateAction
	{
		// Token: 0x06006C58 RID: 27736 RVA: 0x001FE204 File Offset: 0x001FC404
		public Actor GetActor()
		{
			if (this.m_actor == null)
			{
				GameObject actorOwner = this.GetActorOwner();
				if (actorOwner != null)
				{
					this.m_actor = SceneUtils.FindComponentInThisOrParents<Actor>(actorOwner);
				}
			}
			return this.m_actor;
		}

		// Token: 0x06006C59 RID: 27737
		protected abstract GameObject GetActorOwner();

		// Token: 0x06006C5A RID: 27738 RVA: 0x001FE247 File Offset: 0x001FC447
		public override void Reset()
		{
		}

		// Token: 0x06006C5B RID: 27739 RVA: 0x001FE24C File Offset: 0x001FC44C
		public override void OnEnter()
		{
			this.GetActor();
			if (this.m_actor == null)
			{
				Debug.LogError(string.Format("{0}.OnEnter() - FAILED to find Actor component on Owner \"{1}\"", this, base.Owner));
				return;
			}
		}

		// Token: 0x040054F9 RID: 21753
		protected Actor m_actor;
	}
}
