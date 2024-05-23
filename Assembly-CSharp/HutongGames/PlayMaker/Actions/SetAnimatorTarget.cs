using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D7D RID: 3453
	[ActionCategory("Animator")]
	[Tooltip("Sets an AvatarTarget and a targetNormalizedTime for the current state")]
	public class SetAnimatorTarget : FsmStateAction
	{
		// Token: 0x06006C09 RID: 27657 RVA: 0x001FBC85 File Offset: 0x001F9E85
		public override void Reset()
		{
			this.gameObject = null;
			this.avatarTarget = 1;
			this.targetNormalizedTime = null;
			this.everyFrame = false;
		}

		// Token: 0x06006C0A RID: 27658 RVA: 0x001FBCA4 File Offset: 0x001F9EA4
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				base.Finish();
				return;
			}
			this._animator = ownerDefaultTarget.GetComponent<Animator>();
			if (this._animator == null)
			{
				base.Finish();
				return;
			}
			this._animatorProxy = ownerDefaultTarget.GetComponent<PlayMakerAnimatorMoveProxy>();
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent += new Action(this.OnAnimatorMoveEvent);
			}
			this.SetTarget();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006C0B RID: 27659 RVA: 0x001FBD45 File Offset: 0x001F9F45
		public void OnAnimatorMoveEvent()
		{
			if (this._animatorProxy != null)
			{
				this.SetTarget();
			}
		}

		// Token: 0x06006C0C RID: 27660 RVA: 0x001FBD5E File Offset: 0x001F9F5E
		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.SetTarget();
			}
		}

		// Token: 0x06006C0D RID: 27661 RVA: 0x001FBD77 File Offset: 0x001F9F77
		private void SetTarget()
		{
			if (this._animator != null)
			{
				this._animator.SetTarget(this.avatarTarget, this.targetNormalizedTime.Value);
			}
		}

		// Token: 0x06006C0E RID: 27662 RVA: 0x001FBDA8 File Offset: 0x001F9FA8
		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent -= new Action(this.OnAnimatorMoveEvent);
			}
		}

		// Token: 0x0400548A RID: 21642
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		[CheckForComponent(typeof(Animator))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400548B RID: 21643
		[Tooltip("The avatar target")]
		public AvatarTarget avatarTarget;

		// Token: 0x0400548C RID: 21644
		[Tooltip("The current state Time that is queried")]
		public FsmFloat targetNormalizedTime;

		// Token: 0x0400548D RID: 21645
		[Tooltip("Repeat every frame. Useful when changing over time.")]
		public bool everyFrame;

		// Token: 0x0400548E RID: 21646
		private PlayMakerAnimatorMoveProxy _animatorProxy;

		// Token: 0x0400548F RID: 21647
		private Animator _animator;
	}
}
