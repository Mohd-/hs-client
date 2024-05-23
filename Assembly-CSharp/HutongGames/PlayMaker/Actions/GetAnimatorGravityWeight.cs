using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D56 RID: 3414
	[ActionCategory("Animator")]
	[Tooltip("Returns The current gravity weight based on current animations that are played")]
	public class GetAnimatorGravityWeight : FsmStateAction
	{
		// Token: 0x06006B39 RID: 27449 RVA: 0x001F8C15 File Offset: 0x001F6E15
		public override void Reset()
		{
			this.gameObject = null;
			this.gravityWeight = null;
			this.everyFrame = false;
		}

		// Token: 0x06006B3A RID: 27450 RVA: 0x001F8C2C File Offset: 0x001F6E2C
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
			this.DoGetGravityWeight();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006B3B RID: 27451 RVA: 0x001F8CCD File Offset: 0x001F6ECD
		public void OnAnimatorMoveEvent()
		{
			if (this._animatorProxy != null)
			{
				this.DoGetGravityWeight();
			}
		}

		// Token: 0x06006B3C RID: 27452 RVA: 0x001F8CE6 File Offset: 0x001F6EE6
		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.DoGetGravityWeight();
			}
		}

		// Token: 0x06006B3D RID: 27453 RVA: 0x001F8D00 File Offset: 0x001F6F00
		private void DoGetGravityWeight()
		{
			if (this._animator == null)
			{
				return;
			}
			this.gravityWeight.Value = this._animator.gravityWeight;
		}

		// Token: 0x06006B3E RID: 27454 RVA: 0x001F8D38 File Offset: 0x001F6F38
		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent -= new Action(this.OnAnimatorMoveEvent);
			}
		}

		// Token: 0x040053B2 RID: 21426
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040053B3 RID: 21427
		[Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
		public bool everyFrame;

		// Token: 0x040053B4 RID: 21428
		[ActionSection("Results")]
		[UIHint(10)]
		[Tooltip("The current gravity weight based on current animations that are played")]
		public FsmFloat gravityWeight;

		// Token: 0x040053B5 RID: 21429
		private PlayMakerAnimatorMoveProxy _animatorProxy;

		// Token: 0x040053B6 RID: 21430
		private Animator _animator;
	}
}
