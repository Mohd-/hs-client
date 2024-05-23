using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D6C RID: 3436
	[Tooltip("Synchronize a NavMesh Agent velocity and rotation with the animator process.")]
	[ActionCategory("Animator")]
	public class NavMeshAgentAnimatorSynchronizer : FsmStateAction
	{
		// Token: 0x06006BAF RID: 27567 RVA: 0x001FA5E9 File Offset: 0x001F87E9
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x06006BB0 RID: 27568 RVA: 0x001FA5F4 File Offset: 0x001F87F4
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				base.Finish();
				return;
			}
			this._agent = ownerDefaultTarget.GetComponent<NavMeshAgent>();
			this._animator = ownerDefaultTarget.GetComponent<Animator>();
			if (this._animator == null)
			{
				base.Finish();
				return;
			}
			this._trans = ownerDefaultTarget.transform;
			this._animatorProxy = ownerDefaultTarget.GetComponent<PlayMakerAnimatorMoveProxy>();
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent += new Action(this.OnAnimatorMoveEvent);
			}
		}

		// Token: 0x06006BB1 RID: 27569 RVA: 0x001FA698 File Offset: 0x001F8898
		public void OnAnimatorMoveEvent()
		{
			this._agent.velocity = this._animator.deltaPosition / Time.deltaTime;
			this._trans.rotation = this._animator.rootRotation;
		}

		// Token: 0x06006BB2 RID: 27570 RVA: 0x001FA6DC File Offset: 0x001F88DC
		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent -= new Action(this.OnAnimatorMoveEvent);
			}
		}

		// Token: 0x0400542E RID: 21550
		[CheckForComponent(typeof(NavMeshAgent))]
		[RequiredField]
		[Tooltip("The Agent target. An Animator component and a PlayMakerAnimatorMoveProxy component are required")]
		[CheckForComponent(typeof(Animator))]
		[CheckForComponent(typeof(PlayMakerAnimatorMoveProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400542F RID: 21551
		private PlayMakerAnimatorMoveProxy _animatorProxy;

		// Token: 0x04005430 RID: 21552
		private Animator _animator;

		// Token: 0x04005431 RID: 21553
		private NavMeshAgent _agent;

		// Token: 0x04005432 RID: 21554
		private Transform _trans;
	}
}
