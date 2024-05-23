using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D45 RID: 3397
	[Tooltip("Stops the animator playback mode. When playback stops, the avatar resumes getting control from game logic")]
	[ActionCategory("Animator")]
	public class AnimatorStopPlayback : FsmStateAction
	{
		// Token: 0x06006ADF RID: 27359 RVA: 0x001F77A0 File Offset: 0x001F59A0
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x06006AE0 RID: 27360 RVA: 0x001F77AC File Offset: 0x001F59AC
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
				component.StopPlayback();
			}
			base.Finish();
		}

		// Token: 0x04005344 RID: 21316
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		public FsmOwnerDefault gameObject;
	}
}
