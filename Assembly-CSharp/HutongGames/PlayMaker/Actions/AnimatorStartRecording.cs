using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D44 RID: 3396
	[ActionCategory("Animator")]
	[Tooltip("Sets the animator in recording mode, and allocates a circular buffer of size frameCount. After this call, the recorder starts collecting up to frameCount frames in the buffer. Note it is not possible to start playback until a call to StopRecording is made")]
	public class AnimatorStartRecording : FsmStateAction
	{
		// Token: 0x06006ADC RID: 27356 RVA: 0x001F7725 File Offset: 0x001F5925
		public override void Reset()
		{
			this.gameObject = null;
			this.frameCount = 0;
		}

		// Token: 0x06006ADD RID: 27357 RVA: 0x001F773C File Offset: 0x001F593C
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
				component.StartRecording(this.frameCount.Value);
			}
			base.Finish();
		}

		// Token: 0x04005342 RID: 21314
		[RequiredField]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		[CheckForComponent(typeof(Animator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005343 RID: 21315
		[Tooltip("The number of frames (updates) that will be recorded. If frameCount is 0, the recording will continue until the user calls StopRecording. The maximum value for frameCount is 10000.")]
		[RequiredField]
		public FsmInt frameCount;
	}
}
