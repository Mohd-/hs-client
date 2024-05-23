using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D62 RID: 3426
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1053")]
	[Tooltip("Returns if additionnal layers affects the mass center")]
	[ActionCategory("Animator")]
	public class GetAnimatorLayersAffectMassCenter : FsmStateAction
	{
		// Token: 0x06006B76 RID: 27510 RVA: 0x001F9989 File Offset: 0x001F7B89
		public override void Reset()
		{
			this.gameObject = null;
			this.affectMassCenter = null;
			this.affectMassCenterEvent = null;
			this.doNotAffectMassCenterEvent = null;
		}

		// Token: 0x06006B77 RID: 27511 RVA: 0x001F99A8 File Offset: 0x001F7BA8
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
			this.CheckAffectMassCenter();
			base.Finish();
		}

		// Token: 0x06006B78 RID: 27512 RVA: 0x001F9A0C File Offset: 0x001F7C0C
		private void CheckAffectMassCenter()
		{
			if (this._animator == null)
			{
				return;
			}
			bool layersAffectMassCenter = this._animator.layersAffectMassCenter;
			this.affectMassCenter.Value = layersAffectMassCenter;
			if (layersAffectMassCenter)
			{
				base.Fsm.Event(this.affectMassCenterEvent);
			}
			else
			{
				base.Fsm.Event(this.doNotAffectMassCenterEvent);
			}
		}

		// Token: 0x040053F2 RID: 21490
		[Tooltip("The Target. An Animator component is required")]
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040053F3 RID: 21491
		[RequiredField]
		[ActionSection("Results")]
		[UIHint(10)]
		[Tooltip("If true, additionnal layers affects the mass center")]
		public FsmBool affectMassCenter;

		// Token: 0x040053F4 RID: 21492
		[Tooltip("Event send if additionnal layers affects the mass center")]
		public FsmEvent affectMassCenterEvent;

		// Token: 0x040053F5 RID: 21493
		[Tooltip("Event send if additionnal layers do no affects the mass center")]
		public FsmEvent doNotAffectMassCenterEvent;

		// Token: 0x040053F6 RID: 21494
		private Animator _animator;
	}
}
