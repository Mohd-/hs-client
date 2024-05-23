using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D5D RID: 3421
	[Tooltip("Returns true if automatic matching is active. Can also send events")]
	[ActionCategory("Animator")]
	public class GetAnimatorIsMatchingTarget : FsmStateAction
	{
		// Token: 0x06006B5C RID: 27484 RVA: 0x001F942D File Offset: 0x001F762D
		public override void Reset()
		{
			this.gameObject = null;
			this.isMatchingActive = null;
			this.matchingActivatedEvent = null;
			this.matchingDeactivedEvent = null;
			this.everyFrame = false;
		}

		// Token: 0x06006B5D RID: 27485 RVA: 0x001F9454 File Offset: 0x001F7654
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
			this._animatorProxy = ownerDefaultTarget.GetComponent<PlayMakerAnimatorMoveProxy>();
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent += new Action(this.OnAnimatorMoveEvent);
			}
			this.DoCheckIsMatchingActive();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006B5E RID: 27486 RVA: 0x001F94F5 File Offset: 0x001F76F5
		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.DoCheckIsMatchingActive();
			}
		}

		// Token: 0x06006B5F RID: 27487 RVA: 0x001F950E File Offset: 0x001F770E
		public void OnAnimatorMoveEvent()
		{
			this.DoCheckIsMatchingActive();
		}

		// Token: 0x06006B60 RID: 27488 RVA: 0x001F9518 File Offset: 0x001F7718
		private void DoCheckIsMatchingActive()
		{
			if (this._animator == null)
			{
				return;
			}
			bool isMatchingTarget = this._animator.isMatchingTarget;
			this.isMatchingActive.Value = isMatchingTarget;
			if (isMatchingTarget)
			{
				base.Fsm.Event(this.matchingActivatedEvent);
			}
			else
			{
				base.Fsm.Event(this.matchingDeactivedEvent);
			}
		}

		// Token: 0x06006B61 RID: 27489 RVA: 0x001F957C File Offset: 0x001F777C
		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent -= new Action(this.OnAnimatorMoveEvent);
			}
		}

		// Token: 0x040053D8 RID: 21464
		[CheckForComponent(typeof(Animator))]
		[RequiredField]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040053D9 RID: 21465
		[Tooltip("Repeat every frame. Warning: do not use the events in this action if you set everyFrame to true")]
		public bool everyFrame;

		// Token: 0x040053DA RID: 21466
		[ActionSection("Results")]
		[UIHint(10)]
		[Tooltip("True if automatic matching is active")]
		public FsmBool isMatchingActive;

		// Token: 0x040053DB RID: 21467
		[Tooltip("Event send if automatic matching is active")]
		public FsmEvent matchingActivatedEvent;

		// Token: 0x040053DC RID: 21468
		[Tooltip("Event send if automatic matching is not active")]
		public FsmEvent matchingDeactivedEvent;

		// Token: 0x040053DD RID: 21469
		private PlayMakerAnimatorMoveProxy _animatorProxy;

		// Token: 0x040053DE RID: 21470
		private Animator _animator;
	}
}
