using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D43 RID: 3395
	[Tooltip("Sets the animator in playback mode.")]
	[ActionCategory("Animator")]
	public class AnimatorStartPlayback : FsmStateAction
	{
		// Token: 0x06006AD9 RID: 27353 RVA: 0x001F76C1 File Offset: 0x001F58C1
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x06006ADA RID: 27354 RVA: 0x001F76CC File Offset: 0x001F58CC
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
				component.StartPlayback();
			}
			base.Finish();
		}

		// Token: 0x04005341 RID: 21313
		[RequiredField]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		[CheckForComponent(typeof(Animator))]
		public FsmOwnerDefault gameObject;
	}
}
