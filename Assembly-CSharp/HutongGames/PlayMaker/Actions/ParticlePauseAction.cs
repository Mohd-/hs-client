using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DCC RID: 3532
	[ActionCategory("Pegasus")]
	[Tooltip("Pause a Particle System.")]
	public class ParticlePauseAction : FsmStateAction
	{
		// Token: 0x06006D51 RID: 27985 RVA: 0x00202473 File Offset: 0x00200673
		public override void Reset()
		{
			this.m_GameObject = null;
			this.m_IncludeChildren = false;
		}

		// Token: 0x06006D52 RID: 27986 RVA: 0x00202488 File Offset: 0x00200688
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
			if (ownerDefaultTarget == null)
			{
				base.Finish();
				return;
			}
			ParticleSystem component = ownerDefaultTarget.GetComponent<ParticleSystem>();
			if (component == null)
			{
				Debug.LogWarning(string.Format("ParticlePauseAction.OnEnter() - GameObject {0} has no ParticleSystem component", ownerDefaultTarget));
				base.Finish();
				return;
			}
			component.Pause(this.m_IncludeChildren.Value);
			base.Finish();
		}

		// Token: 0x04005602 RID: 22018
		[RequiredField]
		public FsmOwnerDefault m_GameObject;

		// Token: 0x04005603 RID: 22019
		[Tooltip("Run this action on all child objects' Particle Systems.")]
		public FsmBool m_IncludeChildren;
	}
}
