using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D7C RID: 3452
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1074")]
	[Tooltip("If true, automaticaly stabilize feet during transition and blending")]
	[ActionCategory("Animator")]
	public class SetAnimatorStabilizeFeet : FsmStateAction
	{
		// Token: 0x06006C05 RID: 27653 RVA: 0x001FBBD1 File Offset: 0x001F9DD1
		public override void Reset()
		{
			this.gameObject = null;
			this.stabilizeFeet = null;
		}

		// Token: 0x06006C06 RID: 27654 RVA: 0x001FBBE4 File Offset: 0x001F9DE4
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
			this.DoStabilizeFeet();
			base.Finish();
		}

		// Token: 0x06006C07 RID: 27655 RVA: 0x001FBC48 File Offset: 0x001F9E48
		private void DoStabilizeFeet()
		{
			if (this._animator == null)
			{
				return;
			}
			this._animator.stabilizeFeet = this.stabilizeFeet.Value;
		}

		// Token: 0x04005487 RID: 21639
		[Tooltip("The Target. An Animator component is required")]
		[CheckForComponent(typeof(Animator))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005488 RID: 21640
		[Tooltip("If true, automaticaly stabilize feet during transition and blending")]
		public FsmBool stabilizeFeet;

		// Token: 0x04005489 RID: 21641
		private Animator _animator;
	}
}
