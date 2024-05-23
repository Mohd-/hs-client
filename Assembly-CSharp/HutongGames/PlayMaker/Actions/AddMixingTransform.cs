using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B3C RID: 2876
	[ActionCategory(0)]
	[Tooltip("Play an animation on a subset of the hierarchy. E.g., A waving animation on the upper body.")]
	public class AddMixingTransform : FsmStateAction
	{
		// Token: 0x06006238 RID: 25144 RVA: 0x001D34BB File Offset: 0x001D16BB
		public override void Reset()
		{
			this.gameObject = null;
			this.animationName = string.Empty;
			this.transform = string.Empty;
			this.recursive = true;
		}

		// Token: 0x06006239 RID: 25145 RVA: 0x001D34F0 File Offset: 0x001D16F0
		public override void OnEnter()
		{
			this.DoAddMixingTransform();
			base.Finish();
		}

		// Token: 0x0600623A RID: 25146 RVA: 0x001D3500 File Offset: 0x001D1700
		private void DoAddMixingTransform()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Animation component = ownerDefaultTarget.GetComponent<Animation>();
			if (component == null)
			{
				return;
			}
			AnimationState animationState = component[this.animationName.Value];
			if (animationState == null)
			{
				return;
			}
			Transform transform = ownerDefaultTarget.transform.Find(this.transform.Value);
			animationState.AddMixingTransform(transform, this.recursive.Value);
		}

		// Token: 0x04004946 RID: 18758
		[CheckForComponent(typeof(Animation))]
		[Tooltip("The GameObject playing the animation.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004947 RID: 18759
		[Tooltip("The name of the animation to mix. NOTE: The animation should already be added to the Animation Component on the GameObject.")]
		[RequiredField]
		public FsmString animationName;

		// Token: 0x04004948 RID: 18760
		[RequiredField]
		[Tooltip("The mixing transform. E.g., root/upper_body/left_shoulder")]
		public FsmString transform;

		// Token: 0x04004949 RID: 18761
		[Tooltip("If recursive is true all children of the mix transform will also be animated.")]
		public FsmBool recursive;
	}
}
