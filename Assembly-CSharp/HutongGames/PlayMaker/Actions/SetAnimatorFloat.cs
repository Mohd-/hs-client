using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D73 RID: 3443
	[ActionCategory("Animator")]
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1066")]
	[Tooltip("Sets the value of a float parameter")]
	public class SetAnimatorFloat : FsmStateAction
	{
		// Token: 0x06006BD2 RID: 27602 RVA: 0x001FADB1 File Offset: 0x001F8FB1
		public override void Reset()
		{
			this.gameObject = null;
			this.parameter = null;
			this.dampTime = null;
			this.Value = null;
			this.everyFrame = false;
		}

		// Token: 0x06006BD3 RID: 27603 RVA: 0x001FADD8 File Offset: 0x001F8FD8
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
			this.SetParameter();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006BD4 RID: 27604 RVA: 0x001FAE8F File Offset: 0x001F908F
		public void OnAnimatorMoveEvent()
		{
			if (this._animatorProxy != null)
			{
				this.SetParameter();
			}
		}

		// Token: 0x06006BD5 RID: 27605 RVA: 0x001FAEA8 File Offset: 0x001F90A8
		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.SetParameter();
			}
		}

		// Token: 0x06006BD6 RID: 27606 RVA: 0x001FAEC4 File Offset: 0x001F90C4
		private void SetParameter()
		{
			if (this._animator != null)
			{
				if (this.dampTime.Value > 0f)
				{
					this._animator.SetFloat(this._paramID, this.Value.Value, this.dampTime.Value, Time.deltaTime);
				}
				else
				{
					this._animator.SetFloat(this._paramID, this.Value.Value);
				}
			}
		}

		// Token: 0x06006BD7 RID: 27607 RVA: 0x001FAF44 File Offset: 0x001F9144
		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent -= new Action(this.OnAnimatorMoveEvent);
			}
		}

		// Token: 0x0400544D RID: 21581
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400544E RID: 21582
		[Tooltip("The animator parameter")]
		public FsmString parameter;

		// Token: 0x0400544F RID: 21583
		[Tooltip("The float value to assign to the animator parameter")]
		public FsmFloat Value;

		// Token: 0x04005450 RID: 21584
		[Tooltip("Optional: The time allowed to parameter to reach the value. Requires everyFrame Checked on")]
		public FsmFloat dampTime;

		// Token: 0x04005451 RID: 21585
		[Tooltip("Repeat every frame. Useful when changing over time.")]
		public bool everyFrame;

		// Token: 0x04005452 RID: 21586
		private PlayMakerAnimatorMoveProxy _animatorProxy;

		// Token: 0x04005453 RID: 21587
		private Animator _animator;

		// Token: 0x04005454 RID: 21588
		private int _paramID;
	}
}
