using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DF5 RID: 3573
	[ActionCategory("Pegasus")]
	[Tooltip("Initialize a spell state, setting variables that reference the parent actor and its contents.")]
	public class SpellInitActorVariables : FsmStateAction
	{
		// Token: 0x06006DF5 RID: 28149 RVA: 0x0020494A File Offset: 0x00202B4A
		public override void Reset()
		{
		}

		// Token: 0x06006DF6 RID: 28150 RVA: 0x0020494C File Offset: 0x00202B4C
		public override void OnEnter()
		{
			Actor actor = SceneUtils.FindComponentInThisOrParents<Actor>(base.Owner);
			if (actor == null)
			{
				base.Finish();
				return;
			}
			GameObject gameObject = actor.gameObject;
			if (!this.m_actorObject.IsNone)
			{
				this.m_actorObject.Value = gameObject;
			}
			if (!this.m_rootObject.IsNone)
			{
				this.m_rootObject.Value = actor.GetRootObject();
			}
			if (!this.m_rootObjectMesh.IsNone)
			{
				this.m_rootObjectMesh.Value = actor.GetMeshRenderer().gameObject;
			}
			base.Finish();
		}

		// Token: 0x04005699 RID: 22169
		public FsmGameObject m_actorObject;

		// Token: 0x0400569A RID: 22170
		public FsmGameObject m_rootObject;

		// Token: 0x0400569B RID: 22171
		public FsmGameObject m_rootObjectMesh;
	}
}
