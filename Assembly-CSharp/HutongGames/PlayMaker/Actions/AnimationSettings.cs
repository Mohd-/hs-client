using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B53 RID: 2899
	[Tooltip("Set the Wrap Mode, Blend Mode, Layer and Speed of an Animation.\nNOTE: Settings are applied once, on entering the state, NOT continuously. To dynamically control an animation's settings, use Set Animation Speede etc.")]
	[ActionCategory(0)]
	public class AnimationSettings : FsmStateAction
	{
		// Token: 0x060062B8 RID: 25272 RVA: 0x001D85F0 File Offset: 0x001D67F0
		public override void Reset()
		{
			this.gameObject = null;
			this.animName = null;
			this.wrapMode = 2;
			this.blendMode = 0;
			this.speed = 1f;
			this.layer = 0;
		}

		// Token: 0x060062B9 RID: 25273 RVA: 0x001D8635 File Offset: 0x001D6835
		public override void OnEnter()
		{
			this.DoAnimationSettings();
			base.Finish();
		}

		// Token: 0x060062BA RID: 25274 RVA: 0x001D8644 File Offset: 0x001D6844
		private void DoAnimationSettings()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null || string.IsNullOrEmpty(this.animName.Value))
			{
				return;
			}
			Animation component = ownerDefaultTarget.GetComponent<Animation>();
			if (component == null)
			{
				this.LogWarning("Missing animation component: " + ownerDefaultTarget.name);
				return;
			}
			AnimationState animationState = component[this.animName.Value];
			if (animationState == null)
			{
				this.LogWarning("Missing animation: " + this.animName.Value);
				return;
			}
			animationState.wrapMode = this.wrapMode;
			animationState.blendMode = this.blendMode;
			if (!this.layer.IsNone)
			{
				animationState.layer = this.layer.Value;
			}
			if (!this.speed.IsNone)
			{
				animationState.speed = this.speed.Value;
			}
		}

		// Token: 0x04004A2F RID: 18991
		[Tooltip("A GameObject with an Animation Component.")]
		[RequiredField]
		[CheckForComponent(typeof(Animation))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004A30 RID: 18992
		[UIHint(6)]
		[Tooltip("The name of the animation.")]
		[RequiredField]
		public FsmString animName;

		// Token: 0x04004A31 RID: 18993
		[Tooltip("The behavior of the animation when it wraps.")]
		public WrapMode wrapMode;

		// Token: 0x04004A32 RID: 18994
		[Tooltip("How the animation is blended with other animations on the Game Object.")]
		public AnimationBlendMode blendMode;

		// Token: 0x04004A33 RID: 18995
		[HasFloatSlider(0f, 5f)]
		[Tooltip("The speed of the animation. 1 = normal; 2 = double speed...")]
		public FsmFloat speed;

		// Token: 0x04004A34 RID: 18996
		[Tooltip("The animation layer")]
		public FsmInt layer;
	}
}
