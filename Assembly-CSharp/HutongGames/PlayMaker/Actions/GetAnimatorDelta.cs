using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D53 RID: 3411
	[Tooltip("Gets the avatar delta position and rotation for the last evaluated frame.")]
	[ActionCategory("Animator")]
	public class GetAnimatorDelta : FsmStateAction
	{
		// Token: 0x06006B27 RID: 27431 RVA: 0x001F888B File Offset: 0x001F6A8B
		public override void Reset()
		{
			this.gameObject = null;
			this.deltaPosition = null;
			this.deltaRotation = null;
			this.everyFrame = false;
		}

		// Token: 0x06006B28 RID: 27432 RVA: 0x001F88AC File Offset: 0x001F6AAC
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
			this.DoGetDeltaPosition();
			base.Finish();
		}

		// Token: 0x06006B29 RID: 27433 RVA: 0x001F8942 File Offset: 0x001F6B42
		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.DoGetDeltaPosition();
			}
		}

		// Token: 0x06006B2A RID: 27434 RVA: 0x001F895B File Offset: 0x001F6B5B
		public void OnAnimatorMoveEvent()
		{
			this.DoGetDeltaPosition();
		}

		// Token: 0x06006B2B RID: 27435 RVA: 0x001F8964 File Offset: 0x001F6B64
		private void DoGetDeltaPosition()
		{
			if (this._animator == null)
			{
				return;
			}
			this.deltaPosition.Value = this._animator.deltaPosition;
			this.deltaRotation.Value = this._animator.deltaRotation;
		}

		// Token: 0x06006B2C RID: 27436 RVA: 0x001F89B0 File Offset: 0x001F6BB0
		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent -= new Action(this.OnAnimatorMoveEvent);
			}
		}

		// Token: 0x040053A1 RID: 21409
		[RequiredField]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		[CheckForComponent(typeof(Animator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040053A2 RID: 21410
		[UIHint(10)]
		[Tooltip("The avatar delta position for the last evaluated frame")]
		public FsmVector3 deltaPosition;

		// Token: 0x040053A3 RID: 21411
		[UIHint(10)]
		[Tooltip("The avatar delta position for the last evaluated frame")]
		public FsmQuaternion deltaRotation;

		// Token: 0x040053A4 RID: 21412
		[Tooltip("Repeat every frame. Useful when changing over time.")]
		public bool everyFrame;

		// Token: 0x040053A5 RID: 21413
		private PlayMakerAnimatorMoveProxy _animatorProxy;

		// Token: 0x040053A6 RID: 21414
		private Transform _transform;

		// Token: 0x040053A7 RID: 21415
		private Animator _animator;
	}
}
