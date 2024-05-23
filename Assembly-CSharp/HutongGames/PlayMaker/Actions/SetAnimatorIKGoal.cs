using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D74 RID: 3444
	[ActionCategory("Animator")]
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1067")]
	[Tooltip("Sets the position, rotation and weights of an IK goal. A GameObject can be set to control the position and rotation, or it can be manually expressed.")]
	public class SetAnimatorIKGoal : FsmStateAction
	{
		// Token: 0x06006BD9 RID: 27609 RVA: 0x001FAF84 File Offset: 0x001F9184
		public override void Reset()
		{
			this.gameObject = null;
			this.goal = null;
			this.position = new FsmVector3
			{
				UseVariable = true
			};
			this.rotation = new FsmQuaternion
			{
				UseVariable = true
			};
			this.positionWeight = 1f;
			this.rotationWeight = 1f;
			this.everyFrame = false;
		}

		// Token: 0x06006BDA RID: 27610 RVA: 0x001FAFF0 File Offset: 0x001F91F0
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
			this._animatorProxy = ownerDefaultTarget.GetComponent<PlayMakerAnimatorIKProxy>();
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorIKEvent += new Action<int>(this.OnAnimatorIKEvent);
			}
			else
			{
				Debug.LogWarning("This action requires a PlayMakerAnimatorIKProxy. It may not perform properly if not present.");
			}
			GameObject value = this.goal.Value;
			if (value != null)
			{
				this._transform = value.transform;
			}
			this.DoSetIKGoal();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006BDB RID: 27611 RVA: 0x001FB0C4 File Offset: 0x001F92C4
		public void OnAnimatorIKEvent(int layer)
		{
			if (this._animatorProxy != null)
			{
				this.DoSetIKGoal();
			}
		}

		// Token: 0x06006BDC RID: 27612 RVA: 0x001FB0E0 File Offset: 0x001F92E0
		private void DoSetIKGoal()
		{
			if (this._animator == null)
			{
				return;
			}
			if (this._transform != null)
			{
				if (this.position.IsNone)
				{
					this._animator.SetIKPosition(this.iKGoal, this._transform.position);
				}
				else
				{
					this._animator.SetIKPosition(this.iKGoal, this._transform.position + this.position.Value);
				}
				if (this.rotation.IsNone)
				{
					this._animator.SetIKRotation(this.iKGoal, this._transform.rotation);
				}
				else
				{
					this._animator.SetIKRotation(this.iKGoal, this._transform.rotation * this.rotation.Value);
				}
			}
			else
			{
				if (!this.position.IsNone)
				{
					this._animator.SetIKPosition(this.iKGoal, this.position.Value);
				}
				if (!this.rotation.IsNone)
				{
					this._animator.SetIKRotation(this.iKGoal, this.rotation.Value);
				}
			}
			if (!this.positionWeight.IsNone)
			{
				this._animator.SetIKPositionWeight(this.iKGoal, this.positionWeight.Value);
			}
			if (!this.rotationWeight.IsNone)
			{
				this._animator.SetIKRotationWeight(this.iKGoal, this.rotationWeight.Value);
			}
		}

		// Token: 0x06006BDD RID: 27613 RVA: 0x001FB280 File Offset: 0x001F9480
		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorIKEvent -= new Action<int>(this.OnAnimatorIKEvent);
			}
		}

		// Token: 0x04005455 RID: 21589
		[CheckForComponent(typeof(PlayMakerAnimatorIKProxy))]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorIKProxy component are required")]
		[CheckForComponent(typeof(Animator))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005456 RID: 21590
		[Tooltip("The IK goal")]
		public AvatarIKGoal iKGoal;

		// Token: 0x04005457 RID: 21591
		[Tooltip("The gameObject target of the ik goal")]
		public FsmGameObject goal;

		// Token: 0x04005458 RID: 21592
		[Tooltip("The position of the ik goal. If Goal GameObject set, position is used as an offset from Goal")]
		public FsmVector3 position;

		// Token: 0x04005459 RID: 21593
		[Tooltip("The rotation of the ik goal.If Goal GameObject set, rotation is used as an offset from Goal")]
		public FsmQuaternion rotation;

		// Token: 0x0400545A RID: 21594
		[Tooltip("The translative weight of an IK goal (0 = at the original animation before IK, 1 = at the goal)")]
		[HasFloatSlider(0f, 1f)]
		public FsmFloat positionWeight;

		// Token: 0x0400545B RID: 21595
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Sets the rotational weight of an IK goal (0 = rotation before IK, 1 = rotation at the IK goal)")]
		public FsmFloat rotationWeight;

		// Token: 0x0400545C RID: 21596
		[Tooltip("Repeat every frame. Useful when changing over time.")]
		public bool everyFrame;

		// Token: 0x0400545D RID: 21597
		private PlayMakerAnimatorIKProxy _animatorProxy;

		// Token: 0x0400545E RID: 21598
		private Animator _animator;

		// Token: 0x0400545F RID: 21599
		private Transform _transform;
	}
}
