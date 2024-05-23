using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D47 RID: 3399
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1035")]
	[Tooltip("Gets the value of ApplyRootMotion of an avatar. If true, root is controlled by animations")]
	[ActionCategory("Animator")]
	public class GetAnimatorApplyRootMotion : FsmStateAction
	{
		// Token: 0x06006AE5 RID: 27365 RVA: 0x001F7897 File Offset: 0x001F5A97
		public override void Reset()
		{
			this.gameObject = null;
			this.rootMotionApplied = null;
			this.rootMotionIsAppliedEvent = null;
			this.rootMotionIsNotAppliedEvent = null;
		}

		// Token: 0x06006AE6 RID: 27366 RVA: 0x001F78B8 File Offset: 0x001F5AB8
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
			this.GetApplyMotionRoot();
			base.Finish();
		}

		// Token: 0x06006AE7 RID: 27367 RVA: 0x001F791C File Offset: 0x001F5B1C
		private void GetApplyMotionRoot()
		{
			if (this._animator != null)
			{
				bool applyRootMotion = this._animator.applyRootMotion;
				this.rootMotionApplied.Value = applyRootMotion;
				if (applyRootMotion)
				{
					base.Fsm.Event(this.rootMotionIsAppliedEvent);
				}
				else
				{
					base.Fsm.Event(this.rootMotionIsNotAppliedEvent);
				}
			}
		}

		// Token: 0x04005348 RID: 21320
		[Tooltip("The Target. An Animator component is required")]
		[CheckForComponent(typeof(Animator))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005349 RID: 21321
		[Tooltip("Is the rootMotionapplied. If true, root is controlled by animations")]
		[ActionSection("Results")]
		[RequiredField]
		[UIHint(10)]
		public FsmBool rootMotionApplied;

		// Token: 0x0400534A RID: 21322
		[Tooltip("Event send if the root motion is applied")]
		public FsmEvent rootMotionIsAppliedEvent;

		// Token: 0x0400534B RID: 21323
		[Tooltip("Event send if the root motion is not applied")]
		public FsmEvent rootMotionIsNotAppliedEvent;

		// Token: 0x0400534C RID: 21324
		private Animator _animator;
	}
}
