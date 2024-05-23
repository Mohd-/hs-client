using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B5C RID: 2908
	[ActionCategory(0)]
	[Tooltip("Blends an Animation towards a Target Weight over a specified Time.\nOptionally sends an Event when finished.")]
	public class BlendAnimation : FsmStateAction
	{
		// Token: 0x060062D5 RID: 25301 RVA: 0x001D8C89 File Offset: 0x001D6E89
		public override void Reset()
		{
			this.gameObject = null;
			this.animName = null;
			this.targetWeight = 1f;
			this.time = 0.3f;
			this.finishEvent = null;
		}

		// Token: 0x060062D6 RID: 25302 RVA: 0x001D8CC0 File Offset: 0x001D6EC0
		public override void OnEnter()
		{
			this.DoBlendAnimation((this.gameObject.OwnerOption != null) ? this.gameObject.GameObject.Value : base.Owner);
		}

		// Token: 0x060062D7 RID: 25303 RVA: 0x001D8CFE File Offset: 0x001D6EFE
		public override void OnUpdate()
		{
			if (DelayedEvent.WasSent(this.delayedFinishEvent))
			{
				base.Finish();
			}
		}

		// Token: 0x060062D8 RID: 25304 RVA: 0x001D8D18 File Offset: 0x001D6F18
		private void DoBlendAnimation(GameObject go)
		{
			if (go == null)
			{
				return;
			}
			Animation component = go.GetComponent<Animation>();
			if (component == null)
			{
				this.LogWarning("Missing Animation component on GameObject: " + go.name);
				base.Finish();
				return;
			}
			AnimationState animationState = component[this.animName.Value];
			if (animationState == null)
			{
				this.LogWarning("Missing animation: " + this.animName.Value);
				base.Finish();
				return;
			}
			float value = this.time.Value;
			component.Blend(this.animName.Value, this.targetWeight.Value, value);
			if (this.finishEvent != null)
			{
				this.delayedFinishEvent = base.Fsm.DelayedEvent(this.finishEvent, animationState.length);
			}
			else
			{
				base.Finish();
			}
		}

		// Token: 0x04004A48 RID: 19016
		[Tooltip("The GameObject to animate.")]
		[CheckForComponent(typeof(Animation))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004A49 RID: 19017
		[Tooltip("The name of the animation to blend.")]
		[RequiredField]
		[UIHint(6)]
		public FsmString animName;

		// Token: 0x04004A4A RID: 19018
		[RequiredField]
		[Tooltip("Target weight to blend to.")]
		[HasFloatSlider(0f, 1f)]
		public FsmFloat targetWeight;

		// Token: 0x04004A4B RID: 19019
		[Tooltip("How long should the blend take.")]
		[RequiredField]
		[HasFloatSlider(0f, 5f)]
		public FsmFloat time;

		// Token: 0x04004A4C RID: 19020
		[Tooltip("Event to send when the blend has finished.")]
		public FsmEvent finishEvent;

		// Token: 0x04004A4D RID: 19021
		private DelayedEvent delayedFinishEvent;
	}
}
