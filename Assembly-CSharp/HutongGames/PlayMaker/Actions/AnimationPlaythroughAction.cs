using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D9A RID: 3482
	[ActionCategory("Pegasus")]
	[Tooltip("Plays an Animation on a Game Object. Does not wait for the animation to finish.")]
	public class AnimationPlaythroughAction : FsmStateAction
	{
		// Token: 0x06006C89 RID: 27785 RVA: 0x001FEDCC File Offset: 0x001FCFCC
		public override void Reset()
		{
			this.m_GameObject = null;
			this.m_AnimName = null;
			this.m_PhoneAnimName = null;
			this.m_PlayMode = 4;
			this.m_CrossFadeSec = 0.3f;
		}

		// Token: 0x06006C8A RID: 27786 RVA: 0x001FEE08 File Offset: 0x001FD008
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
			if (ownerDefaultTarget == null)
			{
				Debug.LogWarning("AnimationPlaythroughAction GameObject is null!");
				base.Finish();
				return;
			}
			string value = this.m_AnimName.Value;
			if (UniversalInputManager.UsePhoneUI && !string.IsNullOrEmpty(this.m_PhoneAnimName.Value))
			{
				value = this.m_PhoneAnimName.Value;
			}
			if (string.IsNullOrEmpty(value))
			{
				base.Finish();
				return;
			}
			this.StartAnimation(ownerDefaultTarget, value);
			base.Finish();
		}

		// Token: 0x06006C8B RID: 27787 RVA: 0x001FEEA0 File Offset: 0x001FD0A0
		private void StartAnimation(GameObject go, string animName)
		{
			float value = this.m_CrossFadeSec.Value;
			if (value <= Mathf.Epsilon)
			{
				go.GetComponent<Animation>().Play(animName, this.m_PlayMode);
			}
			else
			{
				go.GetComponent<Animation>().CrossFade(animName, value, this.m_PlayMode);
			}
		}

		// Token: 0x04005522 RID: 21794
		[RequiredField]
		[Tooltip("Game Object to play the animation on.")]
		[CheckForComponent(typeof(Animation))]
		public FsmOwnerDefault m_GameObject;

		// Token: 0x04005523 RID: 21795
		[Tooltip("The name of the animation to play.")]
		[UIHint(6)]
		public FsmString m_AnimName;

		// Token: 0x04005524 RID: 21796
		public FsmString m_PhoneAnimName;

		// Token: 0x04005525 RID: 21797
		[Tooltip("How to treat previously playing animations.")]
		public PlayMode m_PlayMode;

		// Token: 0x04005526 RID: 21798
		[HasFloatSlider(0f, 5f)]
		[Tooltip("Time taken to cross fade to this animation.")]
		public FsmFloat m_CrossFadeSec;
	}
}
