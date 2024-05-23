using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D41 RID: 3393
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1034")]
	[Tooltip("Automatically adjust the gameobject position and rotation so that the AvatarTarget reaches the matchPosition when the current state is at the specified progress")]
	[ActionCategory("Animator")]
	public class AnimatorMatchTarget : FsmStateAction
	{
		// Token: 0x06006AD1 RID: 27345 RVA: 0x001F73BC File Offset: 0x001F55BC
		public override void Reset()
		{
			this.gameObject = null;
			this.bodyPart = 0;
			this.target = null;
			this.targetPosition = new FsmVector3
			{
				UseVariable = true
			};
			this.targetRotation = new FsmQuaternion
			{
				UseVariable = true
			};
			this.positionWeight = Vector3.one;
			this.rotationWeight = 0f;
			this.startNormalizedTime = null;
			this.targetNormalizedTime = null;
			this.everyFrame = true;
		}

		// Token: 0x06006AD2 RID: 27346 RVA: 0x001F743C File Offset: 0x001F563C
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
			this.DoMatchTarget();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006AD3 RID: 27347 RVA: 0x001F74CD File Offset: 0x001F56CD
		public override void OnUpdate()
		{
			this.DoMatchTarget();
		}

		// Token: 0x06006AD4 RID: 27348 RVA: 0x001F74D8 File Offset: 0x001F56D8
		private void DoMatchTarget()
		{
			if (this._animator == null)
			{
				return;
			}
			Vector3 vector = Vector3.zero;
			Quaternion quaternion = Quaternion.identity;
			if (this._transform != null)
			{
				vector = this._transform.position;
				quaternion = this._transform.rotation;
			}
			if (!this.targetPosition.IsNone)
			{
				vector += this.targetPosition.Value;
			}
			if (!this.targetRotation.IsNone)
			{
				quaternion *= this.targetRotation.Value;
			}
			MatchTargetWeightMask matchTargetWeightMask;
			matchTargetWeightMask..ctor(this.positionWeight.Value, this.rotationWeight.Value);
			this._animator.MatchTarget(vector, quaternion, this.bodyPart, matchTargetWeightMask, this.startNormalizedTime.Value, this.targetNormalizedTime.Value);
		}

		// Token: 0x04005330 RID: 21296
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The Target. An Animator component is required")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005331 RID: 21297
		[Tooltip("The body part that is involved in the match")]
		public AvatarTarget bodyPart;

		// Token: 0x04005332 RID: 21298
		[Tooltip("The gameObject target to match")]
		public FsmGameObject target;

		// Token: 0x04005333 RID: 21299
		[Tooltip("The position of the ik goal. If Goal GameObject set, position is used as an offset from Goal")]
		public FsmVector3 targetPosition;

		// Token: 0x04005334 RID: 21300
		[Tooltip("The rotation of the ik goal.If Goal GameObject set, rotation is used as an offset from Goal")]
		public FsmQuaternion targetRotation;

		// Token: 0x04005335 RID: 21301
		[Tooltip("The MatchTargetWeightMask Position XYZ weight")]
		public FsmVector3 positionWeight;

		// Token: 0x04005336 RID: 21302
		[Tooltip("The MatchTargetWeightMask Rotation weight")]
		public FsmFloat rotationWeight;

		// Token: 0x04005337 RID: 21303
		[Tooltip("Start time within the animation clip (0 - beginning of clip, 1 - end of clip)")]
		public FsmFloat startNormalizedTime;

		// Token: 0x04005338 RID: 21304
		[Tooltip("End time within the animation clip (0 - beginning of clip, 1 - end of clip), values greater than 1 can be set to trigger a match after a certain number of loops. Ex: 2.3 means at 30% of 2nd loop")]
		public FsmFloat targetNormalizedTime;

		// Token: 0x04005339 RID: 21305
		[Tooltip("Should always be true")]
		public bool everyFrame;

		// Token: 0x0400533A RID: 21306
		private Animator _animator;

		// Token: 0x0400533B RID: 21307
		private Transform _transform;
	}
}
