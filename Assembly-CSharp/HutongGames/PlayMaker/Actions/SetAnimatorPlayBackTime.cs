using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D7A RID: 3450
	[ActionCategory("Animator")]
	[Tooltip("Sets the playback position in the recording buffer. When in playback mode (use AnimatorStartPlayback), this value is used for controlling the current playback position in the buffer (in seconds). The value can range between recordingStartTime and recordingStopTime ")]
	public class SetAnimatorPlayBackTime : FsmStateAction
	{
		// Token: 0x06006BFB RID: 27643 RVA: 0x001FBA39 File Offset: 0x001F9C39
		public override void Reset()
		{
			this.gameObject = null;
			this.playbackTime = null;
			this.everyFrame = false;
		}

		// Token: 0x06006BFC RID: 27644 RVA: 0x001FBA50 File Offset: 0x001F9C50
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
			this.DoPlaybackTime();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006BFD RID: 27645 RVA: 0x001FBABD File Offset: 0x001F9CBD
		public override void OnUpdate()
		{
			this.DoPlaybackTime();
		}

		// Token: 0x06006BFE RID: 27646 RVA: 0x001FBAC8 File Offset: 0x001F9CC8
		private void DoPlaybackTime()
		{
			if (this._animator == null)
			{
				return;
			}
			this._animator.playbackTime = this.playbackTime.Value;
		}

		// Token: 0x0400547F RID: 21631
		[RequiredField]
		[Tooltip("The Target. An Animator component is required")]
		[CheckForComponent(typeof(Animator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005480 RID: 21632
		[Tooltip("The playBack time")]
		public FsmFloat playbackTime;

		// Token: 0x04005481 RID: 21633
		[Tooltip("Repeat every frame. Useful for changing over time.")]
		public bool everyFrame;

		// Token: 0x04005482 RID: 21634
		private Animator _animator;
	}
}
