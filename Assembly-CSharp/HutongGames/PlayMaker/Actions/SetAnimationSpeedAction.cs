using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DDA RID: 3546
	[ActionCategory("Pegasus")]
	[Tooltip("Sets the speed of an Animation.")]
	public class SetAnimationSpeedAction : FsmStateAction
	{
		// Token: 0x06006D85 RID: 28037 RVA: 0x00202EA8 File Offset: 0x002010A8
		public override void Reset()
		{
			this.m_GameObject = null;
			this.m_AnimName = null;
			this.m_PhoneAnimName = null;
			this.m_Speed = 1f;
			this.m_EveryFrame = false;
		}

		// Token: 0x06006D86 RID: 28038 RVA: 0x00202EE1 File Offset: 0x002010E1
		public override void OnEnter()
		{
			if (!this.CacheAnim())
			{
				base.Finish();
				return;
			}
			this.UpdateSpeed();
			if (!this.m_EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D87 RID: 28039 RVA: 0x00202F0C File Offset: 0x0020110C
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
				Debug.LogWarning(string.Format("SetAnimationSpeedAction.OnUpdate() - GameObject {0} is missing an animation component", ownerDefaultTarget));
				base.Finish();
				return;
			}
			this.UpdateSpeed();
		}

		// Token: 0x06006D88 RID: 28040 RVA: 0x00202F6C File Offset: 0x0020116C
		private bool CacheAnim()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
			if (ownerDefaultTarget == null)
			{
				return false;
			}
			string value = this.m_AnimName.Value;
			if (UniversalInputManager.UsePhoneUI && !string.IsNullOrEmpty(this.m_PhoneAnimName.Value))
			{
				value = this.m_PhoneAnimName.Value;
			}
			this.m_animState = ownerDefaultTarget.GetComponent<Animation>()[value];
			return true;
		}

		// Token: 0x06006D89 RID: 28041 RVA: 0x00202FE8 File Offset: 0x002011E8
		private void UpdateSpeed()
		{
			this.m_animState.speed = this.m_Speed.Value;
		}

		// Token: 0x0400562B RID: 22059
		[RequiredField]
		[CheckForComponent(typeof(Animation))]
		[Tooltip("Game Object to play the animation on.")]
		public FsmOwnerDefault m_GameObject;

		// Token: 0x0400562C RID: 22060
		[RequiredField]
		[Tooltip("The name of the animation to play.")]
		[UIHint(6)]
		public FsmString m_AnimName;

		// Token: 0x0400562D RID: 22061
		public FsmString m_PhoneAnimName;

		// Token: 0x0400562E RID: 22062
		public FsmFloat m_Speed;

		// Token: 0x0400562F RID: 22063
		public bool m_EveryFrame;

		// Token: 0x04005630 RID: 22064
		private AnimationState m_animState;
	}
}
