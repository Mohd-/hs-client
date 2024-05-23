using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B37 RID: 2871
	[ActionCategory(0)]
	[Tooltip("Adds a named Animation Clip to a Game Object. Optionally trims the Animation.")]
	public class AddAnimationClip : FsmStateAction
	{
		// Token: 0x06006217 RID: 25111 RVA: 0x001D2E08 File Offset: 0x001D1008
		public override void Reset()
		{
			this.gameObject = null;
			this.animationClip = null;
			this.animationName = string.Empty;
			this.firstFrame = 0;
			this.lastFrame = 0;
			this.addLoopFrame = false;
		}

		// Token: 0x06006218 RID: 25112 RVA: 0x001D2E57 File Offset: 0x001D1057
		public override void OnEnter()
		{
			this.DoAddAnimationClip();
			base.Finish();
		}

		// Token: 0x06006219 RID: 25113 RVA: 0x001D2E68 File Offset: 0x001D1068
		private void DoAddAnimationClip()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			AnimationClip animationClip = this.animationClip.Value as AnimationClip;
			if (animationClip == null)
			{
				return;
			}
			Animation component = ownerDefaultTarget.GetComponent<Animation>();
			if (this.firstFrame.Value == 0 && this.lastFrame.Value == 0)
			{
				component.AddClip(animationClip, this.animationName.Value);
			}
			else
			{
				component.AddClip(animationClip, this.animationName.Value, this.firstFrame.Value, this.lastFrame.Value, this.addLoopFrame.Value);
			}
		}

		// Token: 0x04004929 RID: 18729
		[CheckForComponent(typeof(Animation))]
		[Tooltip("The GameObject to add the Animation Clip to.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400492A RID: 18730
		[ObjectType(typeof(AnimationClip))]
		[Tooltip("The animation clip to add. NOTE: Make sure the clip is compatible with the object's hierarchy.")]
		[RequiredField]
		public FsmObject animationClip;

		// Token: 0x0400492B RID: 18731
		[Tooltip("Name the animation. Used by other actions to reference this animation.")]
		[RequiredField]
		public FsmString animationName;

		// Token: 0x0400492C RID: 18732
		[Tooltip("Optionally trim the animation by specifying a first and last frame.")]
		public FsmInt firstFrame;

		// Token: 0x0400492D RID: 18733
		[Tooltip("Optionally trim the animation by specifying a first and last frame.")]
		public FsmInt lastFrame;

		// Token: 0x0400492E RID: 18734
		[Tooltip("Add an extra looping frame that matches the first frame.")]
		public FsmBool addLoopFrame;
	}
}
