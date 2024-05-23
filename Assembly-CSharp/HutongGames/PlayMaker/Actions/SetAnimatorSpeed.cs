using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D7B RID: 3451
	[ActionCategory("Animator")]
	[Tooltip("Sets the playback speed of the Animator. 1 is normal playback speed")]
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1072")]
	public class SetAnimatorSpeed : FsmStateAction
	{
		// Token: 0x06006C00 RID: 27648 RVA: 0x001FBB05 File Offset: 0x001F9D05
		public override void Reset()
		{
			this.gameObject = null;
			this.speed = null;
			this.everyFrame = false;
		}

		// Token: 0x06006C01 RID: 27649 RVA: 0x001FBB1C File Offset: 0x001F9D1C
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
			this.DoPlaybackSpeed();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006C02 RID: 27650 RVA: 0x001FBB89 File Offset: 0x001F9D89
		public override void OnUpdate()
		{
			this.DoPlaybackSpeed();
		}

		// Token: 0x06006C03 RID: 27651 RVA: 0x001FBB94 File Offset: 0x001F9D94
		private void DoPlaybackSpeed()
		{
			if (this._animator == null)
			{
				return;
			}
			this._animator.speed = this.speed.Value;
		}

		// Token: 0x04005483 RID: 21635
		[RequiredField]
		[Tooltip("The Target. An Animator component is required")]
		[CheckForComponent(typeof(Animator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005484 RID: 21636
		[Tooltip("The playBack speed")]
		public FsmFloat speed;

		// Token: 0x04005485 RID: 21637
		[Tooltip("Repeat every frame. Useful for changing over time.")]
		public bool everyFrame;

		// Token: 0x04005486 RID: 21638
		private Animator _animator;
	}
}
