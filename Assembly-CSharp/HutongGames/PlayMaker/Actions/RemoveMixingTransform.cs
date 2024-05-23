using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C7A RID: 3194
	[Tooltip("Removes a mixing transform previously added with Add Mixing Transform. If transform has been added as recursive, then it will be removed as recursive. Once you remove all mixing transforms added to animation state all curves become animated again.")]
	[ActionCategory(0)]
	public class RemoveMixingTransform : ComponentAction<Animation>
	{
		// Token: 0x06006761 RID: 26465 RVA: 0x001E7DD1 File Offset: 0x001E5FD1
		public override void Reset()
		{
			this.gameObject = null;
			this.animationName = string.Empty;
		}

		// Token: 0x06006762 RID: 26466 RVA: 0x001E7DEA File Offset: 0x001E5FEA
		public override void OnEnter()
		{
			this.DoRemoveMixingTransform();
			base.Finish();
		}

		// Token: 0x06006763 RID: 26467 RVA: 0x001E7DF8 File Offset: 0x001E5FF8
		private void DoRemoveMixingTransform()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			AnimationState animationState = base.animation[this.animationName.Value];
			if (animationState == null)
			{
				return;
			}
			Transform transform = ownerDefaultTarget.transform.Find(this.transfrom.Value);
			animationState.AddMixingTransform(transform);
		}

		// Token: 0x04004F3E RID: 20286
		[RequiredField]
		[Tooltip("The GameObject playing the animation.")]
		[CheckForComponent(typeof(Animation))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004F3F RID: 20287
		[Tooltip("The name of the animation.")]
		[RequiredField]
		public FsmString animationName;

		// Token: 0x04004F40 RID: 20288
		[Tooltip("The mixing transform to remove. E.g., root/upper_body/left_shoulder")]
		[RequiredField]
		public FsmString transfrom;
	}
}
