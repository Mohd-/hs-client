using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D59 RID: 3417
	[ActionCategory("Animator")]
	[Tooltip("Gets the value of a int parameter")]
	public class GetAnimatorInt : FsmStateAction
	{
		// Token: 0x06006B49 RID: 27465 RVA: 0x001F9018 File Offset: 0x001F7218
		public override void Reset()
		{
			this.gameObject = null;
			this.parameter = null;
			this.result = null;
			this.everyFrame = false;
		}

		// Token: 0x06006B4A RID: 27466 RVA: 0x001F9038 File Offset: 0x001F7238
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
			this._paramID = Animator.StringToHash(this.parameter.Value);
			this.GetParameter();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006B4B RID: 27467 RVA: 0x001F90EF File Offset: 0x001F72EF
		public void OnAnimatorMoveEvent()
		{
			if (this._animatorProxy != null)
			{
				this.GetParameter();
			}
		}

		// Token: 0x06006B4C RID: 27468 RVA: 0x001F9108 File Offset: 0x001F7308
		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.GetParameter();
			}
		}

		// Token: 0x06006B4D RID: 27469 RVA: 0x001F9121 File Offset: 0x001F7321
		private void GetParameter()
		{
			if (this._animator != null)
			{
				this.result.Value = this._animator.GetInteger(this._paramID);
			}
		}

		// Token: 0x06006B4E RID: 27470 RVA: 0x001F9150 File Offset: 0x001F7350
		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent -= new Action(this.OnAnimatorMoveEvent);
			}
		}

		// Token: 0x040053C4 RID: 21444
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040053C5 RID: 21445
		[Tooltip("The animator parameter")]
		public FsmString parameter;

		// Token: 0x040053C6 RID: 21446
		[Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
		public bool everyFrame;

		// Token: 0x040053C7 RID: 21447
		[Tooltip("The int value of the animator parameter")]
		[ActionSection("Results")]
		[UIHint(10)]
		[RequiredField]
		public FsmInt result;

		// Token: 0x040053C8 RID: 21448
		private PlayMakerAnimatorMoveProxy _animatorProxy;

		// Token: 0x040053C9 RID: 21449
		private Animator _animator;

		// Token: 0x040053CA RID: 21450
		private int _paramID;
	}
}
