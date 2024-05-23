using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D4A RID: 3402
	[Tooltip("Gets the GameObject mapped to this human bone id")]
	[ActionCategory("Animator")]
	public class GetAnimatorBoneGameObject : FsmStateAction
	{
		// Token: 0x06006AF6 RID: 27382 RVA: 0x001F7BC0 File Offset: 0x001F5DC0
		public override void Reset()
		{
			this.gameObject = null;
			this.bone = 0;
			this.boneAsString = new FsmString
			{
				UseVariable = false
			};
			this.boneGameObject = null;
		}

		// Token: 0x06006AF7 RID: 27383 RVA: 0x001F7BF8 File Offset: 0x001F5DF8
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
			this.GetBoneTransform();
			base.Finish();
		}

		// Token: 0x06006AF8 RID: 27384 RVA: 0x001F7C5C File Offset: 0x001F5E5C
		private void GetBoneTransform()
		{
			HumanBodyBones humanBodyBones = (!this.boneAsString.IsNone) ? ((int)Enum.Parse(typeof(HumanBodyBones), this.boneAsString.Value, true)) : this.bone;
			this.boneGameObject.Value = this._animator.GetBoneTransform(humanBodyBones).gameObject;
		}

		// Token: 0x04005357 RID: 21335
		[RequiredField]
		[Tooltip("The target. An Animator component is required")]
		[CheckForComponent(typeof(Animator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005358 RID: 21336
		[Tooltip("The bone reference")]
		public HumanBodyBones bone;

		// Token: 0x04005359 RID: 21337
		[Tooltip("The bone reference as a string, case insensitive")]
		public FsmString boneAsString;

		// Token: 0x0400535A RID: 21338
		[Tooltip("The Bone's GameObject")]
		[UIHint(10)]
		[ActionSection("Results")]
		public FsmGameObject boneGameObject;

		// Token: 0x0400535B RID: 21339
		private Animator _animator;
	}
}
