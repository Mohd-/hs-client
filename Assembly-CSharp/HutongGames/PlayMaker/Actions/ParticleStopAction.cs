using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DCF RID: 3535
	[ActionCategory("Pegasus")]
	[Tooltip("Stop a Particle System.")]
	public class ParticleStopAction : FsmStateAction
	{
		// Token: 0x06006D5A RID: 27994 RVA: 0x0020269C File Offset: 0x0020089C
		public override void Reset()
		{
			this.m_GameObject = null;
			this.m_IncludeChildren = false;
		}

		// Token: 0x06006D5B RID: 27995 RVA: 0x002026B4 File Offset: 0x002008B4
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
				Debug.LogWarning(string.Format("ParticleStopAction.OnEnter() - GameObject {0} has no ParticleSystem component", ownerDefaultTarget));
				base.Finish();
				return;
			}
			component.Stop(this.m_IncludeChildren.Value);
			base.Finish();
		}

		// Token: 0x04005609 RID: 22025
		[RequiredField]
		public FsmOwnerDefault m_GameObject;

		// Token: 0x0400560A RID: 22026
		[Tooltip("Run this action on all child objects' Particle Systems.")]
		public FsmBool m_IncludeChildren;
	}
}
