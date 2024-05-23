using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D9D RID: 3485
	[Tooltip("Disables an Animator.")]
	[ActionCategory("Pegasus")]
	public class AnimatorStopAction : FsmStateAction
	{
		// Token: 0x06006C94 RID: 27796 RVA: 0x001FF15F File Offset: 0x001FD35F
		public override void Reset()
		{
			this.m_GameObject = null;
		}

		// Token: 0x06006C95 RID: 27797 RVA: 0x001FF168 File Offset: 0x001FD368
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
			if (!ownerDefaultTarget)
			{
				base.Finish();
				return;
			}
			Animator component = ownerDefaultTarget.GetComponent<Animator>();
			if (component)
			{
				component.enabled = false;
			}
			base.Finish();
		}

		// Token: 0x04005532 RID: 21810
		[CheckForComponent(typeof(Animator))]
		[RequiredField]
		[Tooltip("Game Object to play the animation on.")]
		public FsmOwnerDefault m_GameObject;
	}
}
