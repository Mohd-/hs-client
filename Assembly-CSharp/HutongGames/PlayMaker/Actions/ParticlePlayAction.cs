using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DCD RID: 3533
	[ActionCategory("Pegasus")]
	[Tooltip("Play a Particle System. mschweitzer: I think this is equivalent to Simulate with a 1.0 speed.")]
	public class ParticlePlayAction : FsmStateAction
	{
		// Token: 0x06006D54 RID: 27988 RVA: 0x00202503 File Offset: 0x00200703
		public override void Reset()
		{
			this.m_GameObject = null;
			this.m_IncludeChildren = false;
		}

		// Token: 0x06006D55 RID: 27989 RVA: 0x00202518 File Offset: 0x00200718
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
			if (ownerDefaultTarget == null)
			{
				base.Finish();
				return;
			}
			ParticleSystem component = ownerDefaultTarget.GetComponent<ParticleSystem>();
			if (component == null && !this.m_IncludeChildren.Value)
			{
				Debug.LogWarning(string.Format("ParticlePlayAction.OnEnter() - {0} has no ParticleSystem component. Owner={1}", ownerDefaultTarget, base.Owner));
				base.Finish();
				return;
			}
			if (component == null && this.m_IncludeChildren.Value)
			{
				foreach (ParticleSystem particleSystem in ownerDefaultTarget.GetComponentsInChildren<ParticleSystem>())
				{
					particleSystem.Play(this.m_IncludeChildren.Value);
				}
				base.Finish();
				return;
			}
			component.Play(this.m_IncludeChildren.Value);
			base.Finish();
		}

		// Token: 0x04005604 RID: 22020
		[RequiredField]
		public FsmOwnerDefault m_GameObject;

		// Token: 0x04005605 RID: 22021
		[Tooltip("Run this action on all child objects' Particle Systems.")]
		public FsmBool m_IncludeChildren;
	}
}
