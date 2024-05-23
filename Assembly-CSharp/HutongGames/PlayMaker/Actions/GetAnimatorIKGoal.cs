using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D58 RID: 3416
	[Tooltip("Gets the position, rotation and weights of an IK goal. A GameObject can be set to use for the position and rotation")]
	[ActionCategory("Animator")]
	public class GetAnimatorIKGoal : FsmStateAction
	{
		// Token: 0x06006B44 RID: 27460 RVA: 0x001F8E29 File Offset: 0x001F7029
		public override void Reset()
		{
			this.gameObject = null;
			this.goal = null;
			this.position = null;
			this.rotation = null;
			this.positionWeight = null;
			this.rotationWeight = null;
			this.everyFrame = false;
		}

		// Token: 0x06006B45 RID: 27461 RVA: 0x001F8E5C File Offset: 0x001F705C
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
			GameObject value = this.goal.Value;
			if (value != null)
			{
				this._transform = value.transform;
			}
			this.DoGetIKGoal();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006B46 RID: 27462 RVA: 0x001F8EED File Offset: 0x001F70ED
		public override void OnUpdate()
		{
			this.DoGetIKGoal();
		}

		// Token: 0x06006B47 RID: 27463 RVA: 0x001F8EF8 File Offset: 0x001F70F8
		private void DoGetIKGoal()
		{
			if (this._animator == null)
			{
				return;
			}
			if (this._transform != null)
			{
				this._transform.position = this._animator.GetIKPosition(this.iKGoal);
				this._transform.rotation = this._animator.GetIKRotation(this.iKGoal);
			}
			if (!this.position.IsNone)
			{
				this.position.Value = this._animator.GetIKPosition(this.iKGoal);
			}
			if (!this.rotation.IsNone)
			{
				this.rotation.Value = this._animator.GetIKRotation(this.iKGoal);
			}
			if (!this.positionWeight.IsNone)
			{
				this.positionWeight.Value = this._animator.GetIKPositionWeight(this.iKGoal);
			}
			if (!this.rotationWeight.IsNone)
			{
				this.rotationWeight.Value = this._animator.GetIKRotationWeight(this.iKGoal);
			}
		}

		// Token: 0x040053BA RID: 21434
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target. An Animator component is required")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040053BB RID: 21435
		[Tooltip("The IK goal")]
		public AvatarIKGoal iKGoal;

		// Token: 0x040053BC RID: 21436
		[Tooltip("Repeat every frame. Useful for changing over time.")]
		public bool everyFrame;

		// Token: 0x040053BD RID: 21437
		[Tooltip("The gameObject to apply ik goal position and rotation to")]
		[ActionSection("Results")]
		public FsmGameObject goal;

		// Token: 0x040053BE RID: 21438
		[Tooltip("Gets The position of the ik goal. If Goal GameObject define, position is used as an offset from Goal")]
		[UIHint(10)]
		public FsmVector3 position;

		// Token: 0x040053BF RID: 21439
		[Tooltip("Gets The rotation of the ik goal.If Goal GameObject define, rotation is used as an offset from Goal")]
		[UIHint(10)]
		public FsmQuaternion rotation;

		// Token: 0x040053C0 RID: 21440
		[UIHint(10)]
		[Tooltip("Gets The translative weight of an IK goal (0 = at the original animation before IK, 1 = at the goal)")]
		public FsmFloat positionWeight;

		// Token: 0x040053C1 RID: 21441
		[UIHint(10)]
		[Tooltip("Gets the rotational weight of an IK goal (0 = rotation before IK, 1 = rotation at the IK goal)")]
		public FsmFloat rotationWeight;

		// Token: 0x040053C2 RID: 21442
		private Animator _animator;

		// Token: 0x040053C3 RID: 21443
		private Transform _transform;
	}
}
