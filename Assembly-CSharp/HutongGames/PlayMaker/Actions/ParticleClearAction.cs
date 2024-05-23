using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DC9 RID: 3529
	[ActionCategory("Pegasus")]
	[Tooltip("Remove all particles in a Particle System.")]
	public class ParticleClearAction : FsmStateAction
	{
		// Token: 0x06006D45 RID: 27973 RVA: 0x00202139 File Offset: 0x00200339
		public override void Reset()
		{
			this.m_GameObject = null;
			this.m_IncludeChildren = false;
		}

		// Token: 0x06006D46 RID: 27974 RVA: 0x00202150 File Offset: 0x00200350
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
			if (ownerDefaultTarget == null)
			{
				base.Finish();
				return;
			}
			ParticleSystem component = ownerDefaultTarget.GetComponent<ParticleSystem>();
			if (component != null)
			{
				Debug.LogWarning(string.Format("ParticlePlayAction.OnEnter() - GameObject {0} has no ParticleSystem component", ownerDefaultTarget));
				base.Finish();
				return;
			}
			component.Clear(this.m_IncludeChildren.Value);
			base.Finish();
		}

		// Token: 0x040055F5 RID: 22005
		[RequiredField]
		public FsmOwnerDefault m_GameObject;

		// Token: 0x040055F6 RID: 22006
		[Tooltip("Run this action on all child objects' Particle Systems.")]
		public FsmBool m_IncludeChildren;
	}
}
