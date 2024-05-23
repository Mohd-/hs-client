using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D78 RID: 3448
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1071")]
	[ActionCategory("Animator")]
	[Tooltip("Sets look at position and weights. A GameObject can be set to control the look at position, or it can be manually expressed.")]
	public class SetAnimatorLookAt : FsmStateAction
	{
		// Token: 0x06006BEF RID: 27631 RVA: 0x001FB5C4 File Offset: 0x001F97C4
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.targetPosition = new FsmVector3
			{
				UseVariable = true
			};
			this.weight = 1f;
			this.bodyWeight = 0.3f;
			this.headWeight = 0.6f;
			this.eyesWeight = 1f;
			this.clampWeight = 0.5f;
			this.everyFrame = false;
		}

		// Token: 0x06006BF0 RID: 27632 RVA: 0x001FB64C File Offset: 0x001F984C
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
			GameObject value = this.target.Value;
			if (value != null)
			{
				this._transform = value.transform;
			}
			this._animatorProxy = ownerDefaultTarget.GetComponent<PlayMakerAnimatorMoveProxy>();
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent += new Action(this.OnAnimatorMoveEvent);
			}
			this.DoSetLookAt();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006BF1 RID: 27633 RVA: 0x001FB711 File Offset: 0x001F9911
		public void OnAnimatorMoveEvent()
		{
			if (this._animatorProxy != null)
			{
				this.DoSetLookAt();
			}
		}

		// Token: 0x06006BF2 RID: 27634 RVA: 0x001FB72A File Offset: 0x001F992A
		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.DoSetLookAt();
			}
		}

		// Token: 0x06006BF3 RID: 27635 RVA: 0x001FB744 File Offset: 0x001F9944
		private void DoSetLookAt()
		{
			if (this._animator == null)
			{
				return;
			}
			if (this._transform != null)
			{
				if (this.targetPosition.IsNone)
				{
					this._animator.SetLookAtPosition(this._transform.position);
				}
				else
				{
					this._animator.SetLookAtPosition(this._transform.position + this.targetPosition.Value);
				}
			}
			else if (!this.targetPosition.IsNone)
			{
				this._animator.SetLookAtPosition(this.targetPosition.Value);
			}
			if (!this.clampWeight.IsNone)
			{
				this._animator.SetLookAtWeight(this.weight.Value, this.bodyWeight.Value, this.headWeight.Value, this.eyesWeight.Value, this.clampWeight.Value);
			}
			else if (!this.eyesWeight.IsNone)
			{
				this._animator.SetLookAtWeight(this.weight.Value, this.bodyWeight.Value, this.headWeight.Value, this.eyesWeight.Value);
			}
			else if (!this.headWeight.IsNone)
			{
				this._animator.SetLookAtWeight(this.weight.Value, this.bodyWeight.Value, this.headWeight.Value);
			}
			else if (!this.bodyWeight.IsNone)
			{
				this._animator.SetLookAtWeight(this.weight.Value, this.bodyWeight.Value);
			}
			else if (!this.weight.IsNone)
			{
				this._animator.SetLookAtWeight(this.weight.Value);
			}
		}

		// Token: 0x06006BF4 RID: 27636 RVA: 0x001FB930 File Offset: 0x001F9B30
		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent -= new Action(this.OnAnimatorMoveEvent);
			}
		}

		// Token: 0x0400546F RID: 21615
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005470 RID: 21616
		[Tooltip("The gameObject to look at")]
		public FsmGameObject target;

		// Token: 0x04005471 RID: 21617
		[Tooltip("The lookat position. If Target GameObject set, targetPosition is used as an offset from Target")]
		public FsmVector3 targetPosition;

		// Token: 0x04005472 RID: 21618
		[HasFloatSlider(0f, 1f)]
		[Tooltip("The global weight of the LookAt, multiplier for other parameters. Range from 0 to 1")]
		public FsmFloat weight;

		// Token: 0x04005473 RID: 21619
		[Tooltip("determines how much the body is involved in the LookAt. Range from 0 to 1")]
		[HasFloatSlider(0f, 1f)]
		public FsmFloat bodyWeight;

		// Token: 0x04005474 RID: 21620
		[HasFloatSlider(0f, 1f)]
		[Tooltip("determines how much the head is involved in the LookAt. Range from 0 to 1")]
		public FsmFloat headWeight;

		// Token: 0x04005475 RID: 21621
		[HasFloatSlider(0f, 1f)]
		[Tooltip("determines how much the eyes are involved in the LookAt. Range from 0 to 1")]
		public FsmFloat eyesWeight;

		// Token: 0x04005476 RID: 21622
		[Tooltip("0.0 means the character is completely unrestrained in motion, 1.0 means he's completely clamped (look at becomes impossible), and 0.5 means he'll be able to move on half of the possible range (180 degrees).")]
		[HasFloatSlider(0f, 1f)]
		public FsmFloat clampWeight;

		// Token: 0x04005477 RID: 21623
		[Tooltip("Repeat every frame. Useful for changing over time.")]
		public bool everyFrame;

		// Token: 0x04005478 RID: 21624
		private PlayMakerAnimatorMoveProxy _animatorProxy;

		// Token: 0x04005479 RID: 21625
		private Animator _animator;

		// Token: 0x0400547A RID: 21626
		private Transform _transform;
	}
}
