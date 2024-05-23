using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D54 RID: 3412
	[Tooltip("Returns the feet pivot. At 0% blending point is body mass center. At 100% blending point is feet pivot")]
	[ActionCategory("Animator")]
	public class GetAnimatorFeetPivotActive : FsmStateAction
	{
		// Token: 0x06006B2E RID: 27438 RVA: 0x001F89ED File Offset: 0x001F6BED
		public override void Reset()
		{
			this.gameObject = null;
			this.feetPivotActive = null;
		}

		// Token: 0x06006B2F RID: 27439 RVA: 0x001F8A00 File Offset: 0x001F6C00
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
			this.DoGetFeetPivotActive();
			base.Finish();
		}

		// Token: 0x06006B30 RID: 27440 RVA: 0x001F8A64 File Offset: 0x001F6C64
		private void DoGetFeetPivotActive()
		{
			if (this._animator == null)
			{
				return;
			}
			this.feetPivotActive.Value = this._animator.feetPivotActive;
		}

		// Token: 0x040053A8 RID: 21416
		[Tooltip("The Target. An Animator component is required")]
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040053A9 RID: 21417
		[Tooltip("The feet pivot Blending. At 0% blending point is body mass center. At 100% blending point is feet pivot")]
		[RequiredField]
		[UIHint(10)]
		public FsmFloat feetPivotActive;

		// Token: 0x040053AA RID: 21418
		private Animator _animator;
	}
}
