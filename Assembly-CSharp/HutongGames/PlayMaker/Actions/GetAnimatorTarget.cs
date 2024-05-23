using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D6B RID: 3435
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1058")]
	[Tooltip("Gets the position and rotation of the target specified by SetTarget(AvatarTarget targetIndex, float targetNormalizedTime)).\nThe position and rotation are only valid when a frame has being evaluated after the SetTarget call")]
	[ActionCategory("Animator")]
	public class GetAnimatorTarget : FsmStateAction
	{
		// Token: 0x06006BA8 RID: 27560 RVA: 0x001FA416 File Offset: 0x001F8616
		public override void Reset()
		{
			this.gameObject = null;
			this.targetPosition = null;
			this.targetRotation = null;
			this.targetGameObject = null;
			this.everyFrame = false;
		}

		// Token: 0x06006BA9 RID: 27561 RVA: 0x001FA43C File Offset: 0x001F863C
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
			GameObject value = this.targetGameObject.Value;
			if (value != null)
			{
				this._transform = value.transform;
			}
			this.DoGetTarget();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006BAA RID: 27562 RVA: 0x001FA501 File Offset: 0x001F8701
		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.DoGetTarget();
			}
		}

		// Token: 0x06006BAB RID: 27563 RVA: 0x001FA51A File Offset: 0x001F871A
		public void OnAnimatorMoveEvent()
		{
			this.DoGetTarget();
		}

		// Token: 0x06006BAC RID: 27564 RVA: 0x001FA524 File Offset: 0x001F8724
		private void DoGetTarget()
		{
			if (this._animator == null)
			{
				return;
			}
			this.targetPosition.Value = this._animator.targetPosition;
			this.targetRotation.Value = this._animator.targetRotation;
			if (this._transform != null)
			{
				this._transform.position = this._animator.targetPosition;
				this._transform.rotation = this._animator.targetRotation;
			}
		}

		// Token: 0x06006BAD RID: 27565 RVA: 0x001FA5AC File Offset: 0x001F87AC
		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent -= new Action(this.OnAnimatorMoveEvent);
			}
		}

		// Token: 0x04005426 RID: 21542
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005427 RID: 21543
		[Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
		public bool everyFrame;

		// Token: 0x04005428 RID: 21544
		[UIHint(10)]
		[Tooltip("The target position")]
		[ActionSection("Results")]
		public FsmVector3 targetPosition;

		// Token: 0x04005429 RID: 21545
		[Tooltip("The target rotation")]
		[UIHint(10)]
		public FsmQuaternion targetRotation;

		// Token: 0x0400542A RID: 21546
		[Tooltip("If set, apply the position and rotation to this gameObject")]
		public FsmGameObject targetGameObject;

		// Token: 0x0400542B RID: 21547
		private PlayMakerAnimatorMoveProxy _animatorProxy;

		// Token: 0x0400542C RID: 21548
		private Animator _animator;

		// Token: 0x0400542D RID: 21549
		private Transform _transform;
	}
}
