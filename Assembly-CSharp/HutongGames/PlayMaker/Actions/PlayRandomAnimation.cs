using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C62 RID: 3170
	[ActionCategory(0)]
	[Tooltip("Plays a Random Animation on a Game Object. You can set the relative weight of each animation to control how often they are selected.")]
	public class PlayRandomAnimation : ComponentAction<Animation>
	{
		// Token: 0x06006702 RID: 26370 RVA: 0x001E660C File Offset: 0x001E480C
		public override void Reset()
		{
			this.gameObject = null;
			this.animations = new FsmString[0];
			this.weights = new FsmFloat[0];
			this.playMode = 4;
			this.blendTime = 0.3f;
			this.finishEvent = null;
			this.loopEvent = null;
			this.stopOnExit = false;
		}

		// Token: 0x06006703 RID: 26371 RVA: 0x001E6664 File Offset: 0x001E4864
		public override void OnEnter()
		{
			this.DoPlayRandomAnimation();
		}

		// Token: 0x06006704 RID: 26372 RVA: 0x001E666C File Offset: 0x001E486C
		private void DoPlayRandomAnimation()
		{
			if (this.animations.Length > 0)
			{
				int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
				if (randomWeightedIndex != -1)
				{
					this.DoPlayAnimation(this.animations[randomWeightedIndex].Value);
				}
			}
		}

		// Token: 0x06006705 RID: 26373 RVA: 0x001E66B0 File Offset: 0x001E48B0
		private void DoPlayAnimation(string animName)
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				base.Finish();
				return;
			}
			if (string.IsNullOrEmpty(animName))
			{
				this.LogWarning("Missing animName!");
				base.Finish();
				return;
			}
			this.anim = base.animation[animName];
			if (this.anim == null)
			{
				this.LogWarning("Missing animation: " + animName);
				base.Finish();
				return;
			}
			float value = this.blendTime.Value;
			if (value < 0.001f)
			{
				base.animation.Play(animName, this.playMode);
			}
			else
			{
				base.animation.CrossFade(animName, value, this.playMode);
			}
			this.prevAnimtTime = this.anim.time;
		}

		// Token: 0x06006706 RID: 26374 RVA: 0x001E6790 File Offset: 0x001E4990
		public override void OnUpdate()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null || this.anim == null)
			{
				return;
			}
			if (!this.anim.enabled || (this.anim.wrapMode == 8 && this.anim.time > this.anim.length))
			{
				base.Fsm.Event(this.finishEvent);
				base.Finish();
			}
			if (this.anim.wrapMode != 8 && this.anim.time > this.anim.length && this.prevAnimtTime < this.anim.length)
			{
				base.Fsm.Event(this.loopEvent);
			}
		}

		// Token: 0x06006707 RID: 26375 RVA: 0x001E6873 File Offset: 0x001E4A73
		public override void OnExit()
		{
			if (this.stopOnExit)
			{
				this.StopAnimation();
			}
		}

		// Token: 0x06006708 RID: 26376 RVA: 0x001E6888 File Offset: 0x001E4A88
		private void StopAnimation()
		{
			if (base.animation != null)
			{
				base.animation.Stop(this.anim.name);
			}
		}

		// Token: 0x04004ED2 RID: 20178
		[Tooltip("Game Object to play the animation on.")]
		[RequiredField]
		[CheckForComponent(typeof(Animation))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004ED3 RID: 20179
		[UIHint(6)]
		[CompoundArray("Animations", "Animation", "Weight")]
		public FsmString[] animations;

		// Token: 0x04004ED4 RID: 20180
		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] weights;

		// Token: 0x04004ED5 RID: 20181
		[Tooltip("How to treat previously playing animations.")]
		public PlayMode playMode;

		// Token: 0x04004ED6 RID: 20182
		[HasFloatSlider(0f, 5f)]
		[Tooltip("Time taken to blend to this animation.")]
		public FsmFloat blendTime;

		// Token: 0x04004ED7 RID: 20183
		[Tooltip("Event to send when the animation is finished playing. NOTE: Not sent with Loop or PingPong wrap modes!")]
		public FsmEvent finishEvent;

		// Token: 0x04004ED8 RID: 20184
		[Tooltip("Event to send when the animation loops. If you want to send this event to another FSM use Set Event Target. NOTE: This event is only sent with Loop and PingPong wrap modes.")]
		public FsmEvent loopEvent;

		// Token: 0x04004ED9 RID: 20185
		[Tooltip("Stop playing the animation when this state is exited.")]
		public bool stopOnExit;

		// Token: 0x04004EDA RID: 20186
		private AnimationState anim;

		// Token: 0x04004EDB RID: 20187
		private float prevAnimtTime;
	}
}
