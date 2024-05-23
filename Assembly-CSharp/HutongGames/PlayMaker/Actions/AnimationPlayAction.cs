using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D99 RID: 3481
	[ActionCategory("Pegasus")]
	[Tooltip("Plays an Animation on a Game Object and waits for the animation to finish.")]
	public class AnimationPlayAction : FsmStateAction
	{
		// Token: 0x06006C81 RID: 27777 RVA: 0x001FEAFC File Offset: 0x001FCCFC
		public override void Reset()
		{
			this.m_GameObject = null;
			this.m_AnimName = null;
			this.m_PhoneAnimName = null;
			this.m_PlayMode = 4;
			this.m_CrossFadeSec = 0.3f;
			this.m_FinishEvent = null;
			this.m_LoopEvent = null;
			this.m_StopOnExit = false;
		}

		// Token: 0x06006C82 RID: 27778 RVA: 0x001FEB4A File Offset: 0x001FCD4A
		public override void OnEnter()
		{
			if (!this.CacheAnim())
			{
				base.Finish();
				return;
			}
			this.StartAnimation();
		}

		// Token: 0x06006C83 RID: 27779 RVA: 0x001FEB64 File Offset: 0x001FCD64
		public override void OnUpdate()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
			if (ownerDefaultTarget == null)
			{
				base.Finish();
				return;
			}
			if (ownerDefaultTarget.GetComponent<Animation>() == null)
			{
				Debug.LogWarning(string.Format("AnimationPlayAction.OnUpdate() - GameObject {0} is missing an animation component", ownerDefaultTarget));
				base.Finish();
				return;
			}
			if (!this.m_animState.enabled || (this.m_animState.wrapMode == 8 && this.m_animState.time > this.m_animState.length))
			{
				base.Fsm.Event(this.m_FinishEvent);
				base.Finish();
			}
			if (this.m_animState.wrapMode != 8 && this.m_animState.time > this.m_animState.length && this.m_prevAnimTime < this.m_animState.length)
			{
				base.Fsm.Event(this.m_LoopEvent);
			}
		}

		// Token: 0x06006C84 RID: 27780 RVA: 0x001FEC64 File Offset: 0x001FCE64
		public override void OnExit()
		{
			if (this.m_StopOnExit)
			{
				this.StopAnimation();
			}
		}

		// Token: 0x06006C85 RID: 27781 RVA: 0x001FEC78 File Offset: 0x001FCE78
		private bool CacheAnim()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
			this.m_animName = this.m_AnimName.Value;
			if (UniversalInputManager.UsePhoneUI && !string.IsNullOrEmpty(this.m_PhoneAnimName.Value))
			{
				this.m_animName = this.m_PhoneAnimName.Value;
			}
			this.m_animState = ownerDefaultTarget.GetComponent<Animation>()[this.m_animName];
			return true;
		}

		// Token: 0x06006C86 RID: 27782 RVA: 0x001FECF8 File Offset: 0x001FCEF8
		private void StartAnimation()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
			float value = this.m_CrossFadeSec.Value;
			if (value <= Mathf.Epsilon)
			{
				ownerDefaultTarget.GetComponent<Animation>().Play(this.m_animName, this.m_PlayMode);
			}
			else
			{
				ownerDefaultTarget.GetComponent<Animation>().CrossFade(this.m_animName, value, this.m_PlayMode);
			}
			this.m_prevAnimTime = this.m_animState.time;
		}

		// Token: 0x06006C87 RID: 27783 RVA: 0x001FED74 File Offset: 0x001FCF74
		private void StopAnimation()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (ownerDefaultTarget.GetComponent<Animation>() == null)
			{
				return;
			}
			ownerDefaultTarget.GetComponent<Animation>().Stop(this.m_animName);
		}

		// Token: 0x04005517 RID: 21783
		[CheckForComponent(typeof(Animation))]
		[Tooltip("Game Object to play the animation on.")]
		[RequiredField]
		public FsmOwnerDefault m_GameObject;

		// Token: 0x04005518 RID: 21784
		[UIHint(6)]
		[Tooltip("The name of the animation to play.")]
		public FsmString m_AnimName;

		// Token: 0x04005519 RID: 21785
		public FsmString m_PhoneAnimName;

		// Token: 0x0400551A RID: 21786
		[Tooltip("How to treat previously playing animations.")]
		public PlayMode m_PlayMode;

		// Token: 0x0400551B RID: 21787
		[HasFloatSlider(0f, 5f)]
		[Tooltip("Time taken to cross fade to this animation.")]
		public FsmFloat m_CrossFadeSec;

		// Token: 0x0400551C RID: 21788
		[Tooltip("Event to send when the animation is finished playing. NOTE: Not sent with Loop or PingPong wrap modes!")]
		public FsmEvent m_FinishEvent;

		// Token: 0x0400551D RID: 21789
		[Tooltip("Event to send when the animation loops. If you want to send this event to another FSM use Set Event Target. NOTE: This event is only sent with Loop and PingPong wrap modes.")]
		public FsmEvent m_LoopEvent;

		// Token: 0x0400551E RID: 21790
		[Tooltip("Stop playing the animation when this state is exited.")]
		public bool m_StopOnExit;

		// Token: 0x0400551F RID: 21791
		private string m_animName;

		// Token: 0x04005520 RID: 21792
		private AnimationState m_animState;

		// Token: 0x04005521 RID: 21793
		private float m_prevAnimTime;
	}
}
