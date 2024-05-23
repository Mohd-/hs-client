using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D55 RID: 3413
	[ActionCategory("Animator")]
	[Tooltip("Gets the value of a float parameter")]
	public class GetAnimatorFloat : FsmStateAction
	{
		// Token: 0x06006B32 RID: 27442 RVA: 0x001F8AA1 File Offset: 0x001F6CA1
		public override void Reset()
		{
			this.gameObject = null;
			this.parameter = null;
			this.result = null;
			this.everyFrame = false;
		}

		// Token: 0x06006B33 RID: 27443 RVA: 0x001F8AC0 File Offset: 0x001F6CC0
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

		// Token: 0x06006B34 RID: 27444 RVA: 0x001F8B77 File Offset: 0x001F6D77
		public void OnAnimatorMoveEvent()
		{
			if (this._animatorProxy != null)
			{
				this.GetParameter();
			}
		}

		// Token: 0x06006B35 RID: 27445 RVA: 0x001F8B90 File Offset: 0x001F6D90
		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.GetParameter();
			}
		}

		// Token: 0x06006B36 RID: 27446 RVA: 0x001F8BA9 File Offset: 0x001F6DA9
		private void GetParameter()
		{
			if (this._animator != null)
			{
				this.result.Value = this._animator.GetFloat(this._paramID);
			}
		}

		// Token: 0x06006B37 RID: 27447 RVA: 0x001F8BD8 File Offset: 0x001F6DD8
		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent -= new Action(this.OnAnimatorMoveEvent);
			}
		}

		// Token: 0x040053AB RID: 21419
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		[CheckForComponent(typeof(Animator))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040053AC RID: 21420
		[RequiredField]
		[Tooltip("The animator parameter")]
		public FsmString parameter;

		// Token: 0x040053AD RID: 21421
		[Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
		public bool everyFrame;

		// Token: 0x040053AE RID: 21422
		[Tooltip("The float value of the animator parameter")]
		[ActionSection("Results")]
		[RequiredField]
		[UIHint(10)]
		public FsmFloat result;

		// Token: 0x040053AF RID: 21423
		private PlayMakerAnimatorMoveProxy _animatorProxy;

		// Token: 0x040053B0 RID: 21424
		private Animator _animator;

		// Token: 0x040053B1 RID: 21425
		private int _paramID;
	}
}
