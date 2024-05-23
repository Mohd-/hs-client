using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D6A RID: 3434
	[ActionCategory("Animator")]
	[Tooltip("Gets the playback speed of the Animator. 1 is normal playback speed")]
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1056")]
	public class GetAnimatorSpeed : FsmStateAction
	{
		// Token: 0x06006BA3 RID: 27555 RVA: 0x001FA359 File Offset: 0x001F8559
		public override void Reset()
		{
			this.gameObject = null;
			this.speed = null;
			this.everyFrame = false;
		}

		// Token: 0x06006BA4 RID: 27556 RVA: 0x001FA370 File Offset: 0x001F8570
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
			this.GetPlaybackSpeed();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006BA5 RID: 27557 RVA: 0x001FA3DD File Offset: 0x001F85DD
		public override void OnUpdate()
		{
			this.GetPlaybackSpeed();
		}

		// Token: 0x06006BA6 RID: 27558 RVA: 0x001FA3E5 File Offset: 0x001F85E5
		private void GetPlaybackSpeed()
		{
			if (this._animator != null)
			{
				this.speed.Value = this._animator.speed;
			}
		}

		// Token: 0x04005422 RID: 21538
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The Target. An Animator component is required")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005423 RID: 21539
		[Tooltip("The playBack speed of the animator. 1 is normal playback speed")]
		[RequiredField]
		[UIHint(10)]
		public FsmFloat speed;

		// Token: 0x04005424 RID: 21540
		[Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
		public bool everyFrame;

		// Token: 0x04005425 RID: 21541
		private Animator _animator;
	}
}
