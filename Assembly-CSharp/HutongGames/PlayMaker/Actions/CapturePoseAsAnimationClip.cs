using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B6B RID: 2923
	[Tooltip("Captures the current pose of a hierarchy as an animation clip.\n\nUseful to blend from an arbitrary pose (e.g. a ragdoll death) back to a known animation (e.g. idle).")]
	[ActionCategory(0)]
	public class CapturePoseAsAnimationClip : FsmStateAction
	{
		// Token: 0x06006316 RID: 25366 RVA: 0x001D9C14 File Offset: 0x001D7E14
		public override void Reset()
		{
			this.gameObject = null;
			this.position = false;
			this.rotation = true;
			this.scale = false;
			this.storeAnimationClip = null;
		}

		// Token: 0x06006317 RID: 25367 RVA: 0x001D9C53 File Offset: 0x001D7E53
		public override void OnEnter()
		{
			this.DoCaptureAnimationClip();
			base.Finish();
		}

		// Token: 0x06006318 RID: 25368 RVA: 0x001D9C64 File Offset: 0x001D7E64
		private void DoCaptureAnimationClip()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			AnimationClip animationClip = new AnimationClip();
			foreach (object obj in ownerDefaultTarget.transform)
			{
				Transform transform = (Transform)obj;
				this.CaptureTransform(transform, string.Empty, animationClip);
			}
			this.storeAnimationClip.Value = animationClip;
		}

		// Token: 0x06006319 RID: 25369 RVA: 0x001D9D04 File Offset: 0x001D7F04
		private void CaptureTransform(Transform transform, string path, AnimationClip clip)
		{
			path += transform.name;
			if (this.position.Value)
			{
				this.CapturePosition(transform, path, clip);
			}
			if (this.rotation.Value)
			{
				this.CaptureRotation(transform, path, clip);
			}
			if (this.scale.Value)
			{
				this.CaptureScale(transform, path, clip);
			}
			foreach (object obj in transform)
			{
				Transform transform2 = (Transform)obj;
				this.CaptureTransform(transform2, path + "/", clip);
			}
		}

		// Token: 0x0600631A RID: 25370 RVA: 0x001D9DC8 File Offset: 0x001D7FC8
		private void CapturePosition(Transform transform, string path, AnimationClip clip)
		{
			this.SetConstantCurve(clip, path, "localPosition.x", transform.localPosition.x);
			this.SetConstantCurve(clip, path, "localPosition.y", transform.localPosition.y);
			this.SetConstantCurve(clip, path, "localPosition.z", transform.localPosition.z);
		}

		// Token: 0x0600631B RID: 25371 RVA: 0x001D9E28 File Offset: 0x001D8028
		private void CaptureRotation(Transform transform, string path, AnimationClip clip)
		{
			this.SetConstantCurve(clip, path, "localRotation.x", transform.localRotation.x);
			this.SetConstantCurve(clip, path, "localRotation.y", transform.localRotation.y);
			this.SetConstantCurve(clip, path, "localRotation.z", transform.localRotation.z);
			this.SetConstantCurve(clip, path, "localRotation.w", transform.localRotation.w);
		}

		// Token: 0x0600631C RID: 25372 RVA: 0x001D9EA4 File Offset: 0x001D80A4
		private void CaptureScale(Transform transform, string path, AnimationClip clip)
		{
			this.SetConstantCurve(clip, path, "localScale.x", transform.localScale.x);
			this.SetConstantCurve(clip, path, "localScale.y", transform.localScale.y);
			this.SetConstantCurve(clip, path, "localScale.z", transform.localScale.z);
		}

		// Token: 0x0600631D RID: 25373 RVA: 0x001D9F04 File Offset: 0x001D8104
		private void SetConstantCurve(AnimationClip clip, string childPath, string propertyPath, float value)
		{
			AnimationCurve animationCurve = AnimationCurve.Linear(0f, value, 100f, value);
			animationCurve.postWrapMode = 2;
			clip.SetCurve(childPath, typeof(Transform), propertyPath, animationCurve);
		}

		// Token: 0x04004A98 RID: 19096
		[RequiredField]
		[CheckForComponent(typeof(Animation))]
		[Tooltip("The GameObject root of the hierarchy to capture.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004A99 RID: 19097
		[Tooltip("Capture position keys.")]
		public FsmBool position;

		// Token: 0x04004A9A RID: 19098
		[Tooltip("Capture rotation keys.")]
		public FsmBool rotation;

		// Token: 0x04004A9B RID: 19099
		[Tooltip("Capture scale keys.")]
		public FsmBool scale;

		// Token: 0x04004A9C RID: 19100
		[UIHint(10)]
		[RequiredField]
		[Tooltip("Store the result in an Object variable of type AnimationClip.")]
		[ObjectType(typeof(AnimationClip))]
		public FsmObject storeAnimationClip;
	}
}
