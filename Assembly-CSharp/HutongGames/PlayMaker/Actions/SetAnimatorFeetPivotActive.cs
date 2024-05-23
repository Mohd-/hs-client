using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D72 RID: 3442
	[ActionCategory("Animator")]
	[Tooltip("Activates feet pivot. At 0% blending point is body mass center. At 100% blending point is feet pivot")]
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1065")]
	public class SetAnimatorFeetPivotActive : FsmStateAction
	{
		// Token: 0x06006BCE RID: 27598 RVA: 0x001FACFD File Offset: 0x001F8EFD
		public override void Reset()
		{
			this.gameObject = null;
			this.feetPivotActive = null;
		}

		// Token: 0x06006BCF RID: 27599 RVA: 0x001FAD10 File Offset: 0x001F8F10
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
			this.DoFeetPivotActive();
			base.Finish();
		}

		// Token: 0x06006BD0 RID: 27600 RVA: 0x001FAD74 File Offset: 0x001F8F74
		private void DoFeetPivotActive()
		{
			if (this._animator == null)
			{
				return;
			}
			this._animator.feetPivotActive = this.feetPivotActive.Value;
		}

		// Token: 0x0400544A RID: 21578
		[CheckForComponent(typeof(Animator))]
		[RequiredField]
		[Tooltip("The Target. An Animator component is required")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400544B RID: 21579
		[Tooltip("Activates feet pivot. At 0% blending point is body mass center. At 100% blending point is feet pivot")]
		public FsmFloat feetPivotActive;

		// Token: 0x0400544C RID: 21580
		private Animator _animator;
	}
}
