using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D5C RID: 3420
	[Tooltip("Returns true if the specified layer is in a transition. Can also send events")]
	[ActionCategory("Animator")]
	public class GetAnimatorIsLayerInTransition : FsmStateAction
	{
		// Token: 0x06006B55 RID: 27477 RVA: 0x001F9284 File Offset: 0x001F7484
		public override void Reset()
		{
			this.gameObject = null;
			this.isInTransition = null;
			this.isInTransitionEvent = null;
			this.isNotInTransitionEvent = null;
			this.everyFrame = false;
		}

		// Token: 0x06006B56 RID: 27478 RVA: 0x001F92AC File Offset: 0x001F74AC
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
			this.DoCheckIsInTransition();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006B57 RID: 27479 RVA: 0x001F934D File Offset: 0x001F754D
		public void OnAnimatorMoveEvent()
		{
			if (this._animatorProxy != null)
			{
				this.DoCheckIsInTransition();
			}
		}

		// Token: 0x06006B58 RID: 27480 RVA: 0x001F9366 File Offset: 0x001F7566
		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.DoCheckIsInTransition();
			}
		}

		// Token: 0x06006B59 RID: 27481 RVA: 0x001F9380 File Offset: 0x001F7580
		private void DoCheckIsInTransition()
		{
			if (this._animator == null)
			{
				return;
			}
			bool flag = this._animator.IsInTransition(this.layerIndex.Value);
			this.isInTransition.Value = flag;
			if (flag)
			{
				base.Fsm.Event(this.isInTransitionEvent);
			}
			else
			{
				base.Fsm.Event(this.isNotInTransitionEvent);
			}
		}

		// Token: 0x06006B5A RID: 27482 RVA: 0x001F93F0 File Offset: 0x001F75F0
		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent -= new Action(this.OnAnimatorMoveEvent);
			}
		}

		// Token: 0x040053D0 RID: 21456
		[CheckForComponent(typeof(Animator))]
		[RequiredField]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040053D1 RID: 21457
		[RequiredField]
		[Tooltip("The layer's index")]
		public FsmInt layerIndex;

		// Token: 0x040053D2 RID: 21458
		[Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
		public bool everyFrame;

		// Token: 0x040053D3 RID: 21459
		[ActionSection("Results")]
		[UIHint(10)]
		[Tooltip("True if automatic matching is active")]
		public FsmBool isInTransition;

		// Token: 0x040053D4 RID: 21460
		[Tooltip("Event send if automatic matching is active")]
		public FsmEvent isInTransitionEvent;

		// Token: 0x040053D5 RID: 21461
		[Tooltip("Event send if automatic matching is not active")]
		public FsmEvent isNotInTransitionEvent;

		// Token: 0x040053D6 RID: 21462
		private PlayMakerAnimatorMoveProxy _animatorProxy;

		// Token: 0x040053D7 RID: 21463
		private Animator _animator;
	}
}
