using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D6D RID: 3437
	[ActionCategory("Animator")]
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1061")]
	[Tooltip("Set Apply Root Motion: If true, Root is controlled by animations")]
	public class SetAnimatorApplyRootMotion : FsmStateAction
	{
		// Token: 0x06006BB4 RID: 27572 RVA: 0x001FA719 File Offset: 0x001F8919
		public override void Reset()
		{
			this.gameObject = null;
			this.applyRootMotion = null;
		}

		// Token: 0x06006BB5 RID: 27573 RVA: 0x001FA72C File Offset: 0x001F892C
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
			this.DoApplyRootMotion();
			base.Finish();
		}

		// Token: 0x06006BB6 RID: 27574 RVA: 0x001FA790 File Offset: 0x001F8990
		private void DoApplyRootMotion()
		{
			if (this._animator == null)
			{
				return;
			}
			this._animator.applyRootMotion = this.applyRootMotion.Value;
		}

		// Token: 0x04005433 RID: 21555
		[Tooltip("The Target. An Animator component is required")]
		[CheckForComponent(typeof(Animator))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005434 RID: 21556
		[Tooltip("If true, Root is controlled by animations")]
		public FsmBool applyRootMotion;

		// Token: 0x04005435 RID: 21557
		private Animator _animator;
	}
}
