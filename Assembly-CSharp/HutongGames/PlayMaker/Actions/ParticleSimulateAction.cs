using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DCE RID: 3534
	[Tooltip("Simulates a Particle System at a variable speed.")]
	[ActionCategory("Pegasus")]
	public class ParticleSimulateAction : FsmStateAction
	{
		// Token: 0x06006D57 RID: 27991 RVA: 0x00202601 File Offset: 0x00200801
		public override void Reset()
		{
			this.m_GameObject = null;
			this.m_TimeToFastForwardTo = 0f;
			this.m_IncludeChildren = false;
		}

		// Token: 0x06006D58 RID: 27992 RVA: 0x00202628 File Offset: 0x00200828
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			ParticleSystem component = ownerDefaultTarget.GetComponent<ParticleSystem>();
			if (component == null)
			{
				Debug.LogWarning(string.Format("ParticleSimulateAction.OnEnter() - GameObject {0} has no ParticleSystem component", ownerDefaultTarget));
				return;
			}
			component.Simulate(this.m_TimeToFastForwardTo.Value, this.m_IncludeChildren.Value);
		}

		// Token: 0x04005606 RID: 22022
		[RequiredField]
		public FsmOwnerDefault m_GameObject;

		// Token: 0x04005607 RID: 22023
		[Tooltip("Time at which this particle displays. This leave the system in a paused state.")]
		public FsmFloat m_TimeToFastForwardTo;

		// Token: 0x04005608 RID: 22024
		[Tooltip("Run this action on all child objects' Particle Systems.")]
		public FsmBool m_IncludeChildren;
	}
}
