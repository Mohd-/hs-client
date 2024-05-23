using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D46 RID: 3398
	[ActionCategory("Animator")]
	[Tooltip("Stops the animator record mode. It will lock the recording buffer's contents in its current state. The data get saved for subsequent playback with StartPlayback.")]
	public class AnimatorStopRecording : FsmStateAction
	{
		// Token: 0x06006AE2 RID: 27362 RVA: 0x001F7805 File Offset: 0x001F5A05
		public override void Reset()
		{
			this.gameObject = null;
			this.recorderStartTime = null;
			this.recorderStopTime = null;
		}

		// Token: 0x06006AE3 RID: 27363 RVA: 0x001F781C File Offset: 0x001F5A1C
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				base.Finish();
				return;
			}
			Animator component = ownerDefaultTarget.GetComponent<Animator>();
			if (component != null)
			{
				component.StopRecording();
				this.recorderStartTime.Value = component.recorderStartTime;
				this.recorderStopTime.Value = component.recorderStopTime;
			}
			base.Finish();
		}

		// Token: 0x04005345 RID: 21317
		[RequiredField]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		[CheckForComponent(typeof(Animator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005346 RID: 21318
		[ActionSection("Results")]
		[UIHint(10)]
		[Tooltip("The recorder StartTime")]
		public FsmFloat recorderStartTime;

		// Token: 0x04005347 RID: 21319
		[Tooltip("The recorder StopTime")]
		[UIHint(10)]
		public FsmFloat recorderStopTime;
	}
}
