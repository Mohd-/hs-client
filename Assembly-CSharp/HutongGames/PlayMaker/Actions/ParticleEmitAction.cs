using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DCA RID: 3530
	[ActionCategory("Pegasus")]
	[Tooltip("Emit particles in a Particle System immediately.\nIf the particle system is not playing it will start playing.")]
	public class ParticleEmitAction : FsmStateAction
	{
		// Token: 0x06006D48 RID: 27976 RVA: 0x002021CB File Offset: 0x002003CB
		public override void Reset()
		{
			this.m_GameObject = null;
			this.m_Count = 10;
		}

		// Token: 0x06006D49 RID: 27977 RVA: 0x002021E4 File Offset: 0x002003E4
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
			if (ownerDefaultTarget == null)
			{
				base.Finish();
				return;
			}
			if (this.m_IncludeChildren.Value)
			{
				this.EmitParticlesRecurse(ownerDefaultTarget);
			}
			else
			{
				this.EmitParticles(ownerDefaultTarget);
			}
			base.Finish();
		}

		// Token: 0x06006D4A RID: 27978 RVA: 0x00202240 File Offset: 0x00200440
		private void EmitParticles(GameObject go)
		{
			ParticleSystem component = go.GetComponent<ParticleSystem>();
			if (component == null)
			{
				Debug.LogWarning(string.Format("ParticleEmitAction.OnEnter() - GameObject {0} has no ParticleSystem component", go));
				return;
			}
			component.Emit(this.m_Count.Value);
		}

		// Token: 0x06006D4B RID: 27979 RVA: 0x00202284 File Offset: 0x00200484
		private void EmitParticlesRecurse(GameObject go)
		{
			ParticleSystem component = go.GetComponent<ParticleSystem>();
			if (component != null)
			{
				component.Emit(this.m_Count.Value);
			}
			foreach (object obj in go.transform)
			{
				Transform transform = (Transform)obj;
				this.EmitParticlesRecurse(transform.gameObject);
			}
		}

		// Token: 0x040055F7 RID: 22007
		[RequiredField]
		public FsmOwnerDefault m_GameObject;

		// Token: 0x040055F8 RID: 22008
		[Tooltip("The number of particles to emit.")]
		public FsmInt m_Count;

		// Token: 0x040055F9 RID: 22009
		[Tooltip("Run this action on all child objects' Particle Systems.")]
		public FsmBool m_IncludeChildren;
	}
}
