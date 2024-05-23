using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D79 RID: 3449
	[ActionCategory("Animator")]
	[Tooltip("Sets the playback speed of the Animator. 1 is normal playback speed")]
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1072")]
	public class SetAnimatorPlayBackSpeed : FsmStateAction
	{
		// Token: 0x06006BF6 RID: 27638 RVA: 0x001FB96D File Offset: 0x001F9B6D
		public override void Reset()
		{
			this.gameObject = null;
			this.playBackSpeed = null;
			this.everyFrame = false;
		}

		// Token: 0x06006BF7 RID: 27639 RVA: 0x001FB984 File Offset: 0x001F9B84
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
			this.DoPlayBackSpeed();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006BF8 RID: 27640 RVA: 0x001FB9F1 File Offset: 0x001F9BF1
		public override void OnUpdate()
		{
			this.DoPlayBackSpeed();
		}

		// Token: 0x06006BF9 RID: 27641 RVA: 0x001FB9FC File Offset: 0x001F9BFC
		private void DoPlayBackSpeed()
		{
			if (this._animator == null)
			{
				return;
			}
			this._animator.speed = this.playBackSpeed.Value;
		}

		// Token: 0x0400547B RID: 21627
		[Tooltip("The Target. An Animator component is required")]
		[CheckForComponent(typeof(Animator))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400547C RID: 21628
		[Tooltip("If true, automaticly stabilize feet during transition and blending")]
		public FsmFloat playBackSpeed;

		// Token: 0x0400547D RID: 21629
		[Tooltip("Repeat every frame. Useful for changing over time.")]
		public bool everyFrame;

		// Token: 0x0400547E RID: 21630
		private Animator _animator;
	}
}
