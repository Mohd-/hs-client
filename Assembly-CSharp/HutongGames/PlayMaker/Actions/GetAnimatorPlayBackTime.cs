using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D67 RID: 3431
	[ActionCategory("Animator")]
	[Tooltip("Gets the playback position in the recording buffer. When in playback mode (use  AnimatorStartPlayback), this value is used for controlling the current playback position in the buffer (in seconds). The value can range between recordingStartTime and recordingStopTime See Also: StartPlayback, StopPlayback.")]
	public class GetAnimatorPlayBackTime : FsmStateAction
	{
		// Token: 0x06006B92 RID: 27538 RVA: 0x001F9FF6 File Offset: 0x001F81F6
		public override void Reset()
		{
			this.gameObject = null;
			this.playBackTime = null;
			this.everyFrame = false;
		}

		// Token: 0x06006B93 RID: 27539 RVA: 0x001FA010 File Offset: 0x001F8210
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
			this.GetPlayBackTime();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006B94 RID: 27540 RVA: 0x001FA07D File Offset: 0x001F827D
		public override void OnUpdate()
		{
			this.GetPlayBackTime();
		}

		// Token: 0x06006B95 RID: 27541 RVA: 0x001FA085 File Offset: 0x001F8285
		private void GetPlayBackTime()
		{
			if (this._animator != null)
			{
				this.playBackTime.Value = this._animator.playbackTime;
			}
		}

		// Token: 0x04005412 RID: 21522
		[CheckForComponent(typeof(Animator))]
		[RequiredField]
		[Tooltip("The Target. An Animator component is required")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005413 RID: 21523
		[UIHint(10)]
		[Tooltip("The playBack time of the animator.")]
		[RequiredField]
		[ActionSection("Result")]
		public FsmFloat playBackTime;

		// Token: 0x04005414 RID: 21524
		[Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
		public bool everyFrame;

		// Token: 0x04005415 RID: 21525
		private Animator _animator;
	}
}
