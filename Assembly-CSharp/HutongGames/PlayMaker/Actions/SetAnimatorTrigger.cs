using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D7E RID: 3454
	[ActionCategory("Animator")]
	[Tooltip("Sets a trigger parameter to active or inactive. Triggers are parameters that act mostly like booleans, but get resets to inactive when they are used in a transition.")]
	public class SetAnimatorTrigger : FsmStateAction
	{
		// Token: 0x06006C10 RID: 27664 RVA: 0x001FBDE5 File Offset: 0x001F9FE5
		public override void Reset()
		{
			this.gameObject = null;
			this.trigger = null;
		}

		// Token: 0x06006C11 RID: 27665 RVA: 0x001FBDF8 File Offset: 0x001F9FF8
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
			this.SetTrigger();
			base.Finish();
		}

		// Token: 0x06006C12 RID: 27666 RVA: 0x001FBE5A File Offset: 0x001FA05A
		private void SetTrigger()
		{
			if (this._animator != null)
			{
				this._animator.SetTrigger(this.trigger.Value);
			}
		}

		// Token: 0x04005490 RID: 21648
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target. An Animator component is required")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005491 RID: 21649
		[Tooltip("The trigger name")]
		public FsmString trigger;

		// Token: 0x04005492 RID: 21650
		private Animator _animator;

		// Token: 0x04005493 RID: 21651
		private int _paramID;
	}
}
