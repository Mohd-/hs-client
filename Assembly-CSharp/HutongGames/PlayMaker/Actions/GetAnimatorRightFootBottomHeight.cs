using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D68 RID: 3432
	[ActionCategory("Animator")]
	[Tooltip("Get the right foot bottom height.")]
	public class GetAnimatorRightFootBottomHeight : FsmStateAction
	{
		// Token: 0x06006B97 RID: 27543 RVA: 0x001FA0B6 File Offset: 0x001F82B6
		public override void Reset()
		{
			this.gameObject = null;
			this.rightFootHeight = null;
			this.everyFrame = false;
		}

		// Token: 0x06006B98 RID: 27544 RVA: 0x001FA0D0 File Offset: 0x001F82D0
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
			this._getRightFootBottonHeight();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006B99 RID: 27545 RVA: 0x001FA13D File Offset: 0x001F833D
		public override void OnLateUpdate()
		{
			this._getRightFootBottonHeight();
		}

		// Token: 0x06006B9A RID: 27546 RVA: 0x001FA145 File Offset: 0x001F8345
		private void _getRightFootBottonHeight()
		{
			if (this._animator != null)
			{
				this.rightFootHeight.Value = this._animator.rightFeetBottomHeight;
			}
		}

		// Token: 0x04005416 RID: 21526
		[Tooltip("The Target. An Animator component is required")]
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005417 RID: 21527
		[ActionSection("Result")]
		[UIHint(10)]
		[Tooltip("The right foot bottom height.")]
		[RequiredField]
		public FsmFloat rightFootHeight;

		// Token: 0x04005418 RID: 21528
		[Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
		public bool everyFrame;

		// Token: 0x04005419 RID: 21529
		private Animator _animator;
	}
}
