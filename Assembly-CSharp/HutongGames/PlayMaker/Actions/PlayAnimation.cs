using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C60 RID: 3168
	[ActionCategory(0)]
	[Tooltip("Plays an Animation on a Game Object. You can add named animation clips to the object in the Unity editor, or with the Add Animation Clip action.")]
	public class PlayAnimation : ComponentAction<Animation>
	{
		// Token: 0x060066F8 RID: 26360 RVA: 0x001E6310 File Offset: 0x001E4510
		public override void Reset()
		{
			this.gameObject = null;
			this.animName = null;
			this.playMode = 4;
			this.blendTime = 0.3f;
			this.finishEvent = null;
			this.loopEvent = null;
			this.stopOnExit = false;
		}

		// Token: 0x060066F9 RID: 26361 RVA: 0x001E6357 File Offset: 0x001E4557
		public override void OnEnter()
		{
			this.DoPlayAnimation();
		}

		// Token: 0x060066FA RID: 26362 RVA: 0x001E6360 File Offset: 0x001E4560
		private void DoPlayAnimation()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				base.Finish();
				return;
			}
			if (string.IsNullOrEmpty(this.animName.Value))
			{
				this.LogWarning("Missing animName!");
				base.Finish();
				return;
			}
			this.anim = base.animation[this.animName.Value];
			if (this.anim == null)
			{
				this.LogWarning("Missing animation: " + this.animName.Value);
				base.Finish();
				return;
			}
			float value = this.blendTime.Value;
			if (value < 0.001f)
			{
				base.animation.Play(this.animName.Value, this.playMode);
			}
			else
			{
				base.animation.CrossFade(this.animName.Value, value, this.playMode);
			}
			this.prevAnimtTime = this.anim.time;
		}

		// Token: 0x060066FB RID: 26363 RVA: 0x001E6470 File Offset: 0x001E4670
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

		// Token: 0x060066FC RID: 26364 RVA: 0x001E6553 File Offset: 0x001E4753
		public override void OnExit()
		{
			if (this.stopOnExit)
			{
				this.StopAnimation();
			}
		}

		// Token: 0x060066FD RID: 26365 RVA: 0x001E6568 File Offset: 0x001E4768
		private void StopAnimation()
		{
			if (base.animation != null)
			{
				base.animation.Stop(this.animName.Value);
			}
		}

		// Token: 0x04004EC7 RID: 20167
		[CheckForComponent(typeof(Animation))]
		[Tooltip("Game Object to play the animation on.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004EC8 RID: 20168
		[UIHint(6)]
		[Tooltip("The name of the animation to play.")]
		public FsmString animName;

		// Token: 0x04004EC9 RID: 20169
		[Tooltip("How to treat previously playing animations.")]
		public PlayMode playMode;

		// Token: 0x04004ECA RID: 20170
		[Tooltip("Time taken to blend to this animation.")]
		[HasFloatSlider(0f, 5f)]
		public FsmFloat blendTime;

		// Token: 0x04004ECB RID: 20171
		[Tooltip("Event to send when the animation is finished playing. NOTE: Not sent with Loop or PingPong wrap modes!")]
		public FsmEvent finishEvent;

		// Token: 0x04004ECC RID: 20172
		[Tooltip("Event to send when the animation loops. If you want to send this event to another FSM use Set Event Target. NOTE: This event is only sent with Loop and PingPong wrap modes.")]
		public FsmEvent loopEvent;

		// Token: 0x04004ECD RID: 20173
		[Tooltip("Stop playing the animation when this state is exited.")]
		public bool stopOnExit;

		// Token: 0x04004ECE RID: 20174
		private AnimationState anim;

		// Token: 0x04004ECF RID: 20175
		private float prevAnimtTime;
	}
}
