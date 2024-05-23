using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D66 RID: 3430
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1056")]
	[ActionCategory("Animator")]
	[Tooltip("Gets the playback speed of the Animator. 1 is normal playback speed")]
	public class GetAnimatorPlayBackSpeed : FsmStateAction
	{
		// Token: 0x06006B8D RID: 27533 RVA: 0x001F9F39 File Offset: 0x001F8139
		public override void Reset()
		{
			this.gameObject = null;
			this.playBackSpeed = null;
			this.everyFrame = false;
		}

		// Token: 0x06006B8E RID: 27534 RVA: 0x001F9F50 File Offset: 0x001F8150
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
			this.GetPlayBackSpeed();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006B8F RID: 27535 RVA: 0x001F9FBD File Offset: 0x001F81BD
		public override void OnUpdate()
		{
			this.GetPlayBackSpeed();
		}

		// Token: 0x06006B90 RID: 27536 RVA: 0x001F9FC5 File Offset: 0x001F81C5
		private void GetPlayBackSpeed()
		{
			if (this._animator != null)
			{
				this.playBackSpeed.Value = this._animator.speed;
			}
		}

		// Token: 0x0400540E RID: 21518
		[RequiredField]
		[Tooltip("The Target. An Animator component is required")]
		[CheckForComponent(typeof(Animator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400540F RID: 21519
		[RequiredField]
		[Tooltip("The playBack speed of the animator. 1 is normal playback speed")]
		[UIHint(10)]
		public FsmFloat playBackSpeed;

		// Token: 0x04005410 RID: 21520
		[Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
		public bool everyFrame;

		// Token: 0x04005411 RID: 21521
		private Animator _animator;
	}
}
