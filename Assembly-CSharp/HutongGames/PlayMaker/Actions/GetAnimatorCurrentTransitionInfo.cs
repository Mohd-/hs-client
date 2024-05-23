using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D50 RID: 3408
	[ActionCategory("Animator")]
	[Tooltip("Gets the current transition information on a specified layer. Only valid when during a transition.")]
	public class GetAnimatorCurrentTransitionInfo : FsmStateAction
	{
		// Token: 0x06006B16 RID: 27414 RVA: 0x001F842B File Offset: 0x001F662B
		public override void Reset()
		{
			this.gameObject = null;
			this.layerIndex = null;
			this.name = null;
			this.nameHash = null;
			this.userNameHash = null;
			this.normalizedTime = null;
			this.everyFrame = false;
		}

		// Token: 0x06006B17 RID: 27415 RVA: 0x001F8460 File Offset: 0x001F6660
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
			this.GetTransitionInfo();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006B18 RID: 27416 RVA: 0x001F8501 File Offset: 0x001F6701
		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.GetTransitionInfo();
			}
		}

		// Token: 0x06006B19 RID: 27417 RVA: 0x001F851A File Offset: 0x001F671A
		public void OnAnimatorMoveEvent()
		{
			if (this._animatorProxy != null)
			{
				this.GetTransitionInfo();
			}
		}

		// Token: 0x06006B1A RID: 27418 RVA: 0x001F8534 File Offset: 0x001F6734
		private void GetTransitionInfo()
		{
			if (this._animator != null)
			{
				AnimatorTransitionInfo animatorTransitionInfo = this._animator.GetAnimatorTransitionInfo(this.layerIndex.Value);
				if (!this.name.IsNone)
				{
					this.name.Value = this._animator.GetLayerName(this.layerIndex.Value);
				}
				this.nameHash.Value = animatorTransitionInfo.fullPathHash;
				this.userNameHash.Value = animatorTransitionInfo.userNameHash;
				this.normalizedTime.Value = animatorTransitionInfo.normalizedTime;
			}
		}

		// Token: 0x06006B1B RID: 27419 RVA: 0x001F85D0 File Offset: 0x001F67D0
		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent -= new Action(this.OnAnimatorMoveEvent);
			}
		}

		// Token: 0x04005386 RID: 21382
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		[CheckForComponent(typeof(Animator))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005387 RID: 21383
		[Tooltip("The layer's index")]
		[RequiredField]
		public FsmInt layerIndex;

		// Token: 0x04005388 RID: 21384
		[Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
		public bool everyFrame;

		// Token: 0x04005389 RID: 21385
		[ActionSection("Results")]
		[UIHint(10)]
		[Tooltip("The unique name of the Transition")]
		public FsmString name;

		// Token: 0x0400538A RID: 21386
		[UIHint(10)]
		[Tooltip("The unique name of the Transition")]
		public FsmInt nameHash;

		// Token: 0x0400538B RID: 21387
		[UIHint(10)]
		[Tooltip("The user-specidied name of the Transition")]
		public FsmInt userNameHash;

		// Token: 0x0400538C RID: 21388
		[Tooltip("Normalized time of the Transition")]
		[UIHint(10)]
		public FsmFloat normalizedTime;

		// Token: 0x0400538D RID: 21389
		private PlayMakerAnimatorMoveProxy _animatorProxy;

		// Token: 0x0400538E RID: 21390
		private Animator _animator;
	}
}
