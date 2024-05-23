using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D63 RID: 3427
	[ActionCategory("Animator")]
	[Tooltip("Get the left foot bottom height.")]
	public class GetAnimatorLeftFootBottomHeight : FsmStateAction
	{
		// Token: 0x06006B7A RID: 27514 RVA: 0x001F9A78 File Offset: 0x001F7C78
		public override void Reset()
		{
			this.gameObject = null;
			this.leftFootHeight = null;
			this.everyFrame = false;
		}

		// Token: 0x06006B7B RID: 27515 RVA: 0x001F9A90 File Offset: 0x001F7C90
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
			this._getLeftFootBottonHeight();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006B7C RID: 27516 RVA: 0x001F9AFD File Offset: 0x001F7CFD
		public override void OnLateUpdate()
		{
			this._getLeftFootBottonHeight();
		}

		// Token: 0x06006B7D RID: 27517 RVA: 0x001F9B05 File Offset: 0x001F7D05
		private void _getLeftFootBottonHeight()
		{
			if (this._animator != null)
			{
				this.leftFootHeight.Value = this._animator.leftFeetBottomHeight;
			}
		}

		// Token: 0x040053F7 RID: 21495
		[CheckForComponent(typeof(Animator))]
		[RequiredField]
		[Tooltip("The Target. An Animator component is required")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040053F8 RID: 21496
		[UIHint(10)]
		[ActionSection("Result")]
		[RequiredField]
		[Tooltip("the left foot bottom height.")]
		public FsmFloat leftFootHeight;

		// Token: 0x040053F9 RID: 21497
		[Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
		public bool everyFrame;

		// Token: 0x040053FA RID: 21498
		private Animator _animator;
	}
}
