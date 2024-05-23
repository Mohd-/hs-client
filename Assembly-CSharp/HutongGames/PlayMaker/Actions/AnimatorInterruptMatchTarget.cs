using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D40 RID: 3392
	[ActionCategory("Animator")]
	[Tooltip("Interrupts the automatic target matching. CompleteMatch will make the gameobject match the target completely at the next frame.")]
	public class AnimatorInterruptMatchTarget : FsmStateAction
	{
		// Token: 0x06006ACE RID: 27342 RVA: 0x001F7340 File Offset: 0x001F5540
		public override void Reset()
		{
			this.gameObject = null;
			this.completeMatch = true;
		}

		// Token: 0x06006ACF RID: 27343 RVA: 0x001F7358 File Offset: 0x001F5558
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
				component.InterruptMatchTarget(this.completeMatch.Value);
			}
			base.Finish();
		}

		// Token: 0x0400532E RID: 21294
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400532F RID: 21295
		[Tooltip("Will make the gameobject match the target completely at the next frame")]
		public FsmBool completeMatch;
	}
}
