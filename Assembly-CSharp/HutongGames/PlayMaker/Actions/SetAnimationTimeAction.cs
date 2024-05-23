using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DDB RID: 3547
	[Tooltip("Sets the speed of an Animation.")]
	[ActionCategory("Pegasus")]
	public class SetAnimationTimeAction : FsmStateAction
	{
		// Token: 0x06006D8B RID: 28043 RVA: 0x00203008 File Offset: 0x00201208
		public override void Reset()
		{
			this.m_GameObject = null;
			this.m_AnimName = null;
			this.m_PhoneAnimName = null;
			this.m_Time = 0f;
			this.m_EveryFrame = false;
		}

		// Token: 0x06006D8C RID: 28044 RVA: 0x00203041 File Offset: 0x00201241
		public override void OnEnter()
		{
			if (!this.CacheAnim())
			{
				base.Finish();
				return;
			}
			this.UpdateTime();
			if (!this.m_EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D8D RID: 28045 RVA: 0x0020306C File Offset: 0x0020126C
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
			this.UpdateTime();
		}

		// Token: 0x06006D8E RID: 28046 RVA: 0x002030CC File Offset: 0x002012CC
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

		// Token: 0x06006D8F RID: 28047 RVA: 0x00203148 File Offset: 0x00201348
		private void UpdateTime()
		{
			this.m_animState.time = this.m_Time.Value;
		}

		// Token: 0x04005631 RID: 22065
		[Tooltip("Game Object to play the animation on.")]
		[CheckForComponent(typeof(Animation))]
		[RequiredField]
		public FsmOwnerDefault m_GameObject;

		// Token: 0x04005632 RID: 22066
		[UIHint(6)]
		[Tooltip("The name of the animation to play.")]
		[RequiredField]
		public FsmString m_AnimName;

		// Token: 0x04005633 RID: 22067
		public FsmString m_PhoneAnimName;

		// Token: 0x04005634 RID: 22068
		public FsmFloat m_Time;

		// Token: 0x04005635 RID: 22069
		public bool m_EveryFrame;

		// Token: 0x04005636 RID: 22070
		private AnimationState m_animState;
	}
}
