using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D77 RID: 3447
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1070")]
	[ActionCategory("Animator")]
	[Tooltip("If true, additionnal layers affects the mass center")]
	public class SetAnimatorLayersAffectMassCenter : FsmStateAction
	{
		// Token: 0x06006BEB RID: 27627 RVA: 0x001FB510 File Offset: 0x001F9710
		public override void Reset()
		{
			this.gameObject = null;
			this.affectMassCenter = null;
		}

		// Token: 0x06006BEC RID: 27628 RVA: 0x001FB520 File Offset: 0x001F9720
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
			this.SetAffectMassCenter();
			base.Finish();
		}

		// Token: 0x06006BED RID: 27629 RVA: 0x001FB584 File Offset: 0x001F9784
		private void SetAffectMassCenter()
		{
			if (this._animator == null)
			{
				return;
			}
			this._animator.layersAffectMassCenter = this.affectMassCenter.Value;
		}

		// Token: 0x0400546C RID: 21612
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The Target. An Animator component is required")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400546D RID: 21613
		[Tooltip("If true, additionnal layers affects the mass center")]
		public FsmBool affectMassCenter;

		// Token: 0x0400546E RID: 21614
		private Animator _animator;
	}
}
