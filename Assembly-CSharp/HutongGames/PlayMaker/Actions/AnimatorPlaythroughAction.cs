using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D9C RID: 3484
	[Tooltip("Enables an Animator and plays one of its states and waits for it to complete.")]
	[ActionCategory("Pegasus")]
	public class AnimatorPlaythroughAction : FsmStateAction
	{
		// Token: 0x06006C90 RID: 27792 RVA: 0x001FEFF8 File Offset: 0x001FD1F8
		public override void Reset()
		{
			this.m_GameObject = null;
			this.m_StateName = null;
			this.m_LayerName = new FsmString
			{
				UseVariable = true
			};
			this.m_StartTimePercent = 0f;
		}

		// Token: 0x06006C91 RID: 27793 RVA: 0x001FF038 File Offset: 0x001FD238
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
			if (!ownerDefaultTarget)
			{
				base.Finish();
				return;
			}
			Animator component = ownerDefaultTarget.GetComponent<Animator>();
			if (component)
			{
				int num = -1;
				if (!this.m_LayerName.IsNone)
				{
					num = AnimationUtil.GetLayerIndexFromName(component, this.m_LayerName.Value);
				}
				float num2 = float.NegativeInfinity;
				if (!this.m_StartTimePercent.IsNone)
				{
					num2 = 0.01f * this.m_StartTimePercent.Value;
				}
				component.enabled = true;
				component.Play(this.m_StateName.Value, num, num2);
				this.m_checkComplete = component;
				this.m_checkLayer = ((num != -1) ? num : 0);
			}
			else
			{
				base.Finish();
			}
		}

		// Token: 0x06006C92 RID: 27794 RVA: 0x001FF108 File Offset: 0x001FD308
		public override void OnUpdate()
		{
			if (this.m_checkComplete == null)
			{
				return;
			}
			if (this.m_checkComplete.GetCurrentAnimatorStateInfo(this.m_checkLayer).normalizedTime > 1f)
			{
				this.m_checkComplete = null;
				base.Finish();
			}
		}

		// Token: 0x0400552B RID: 21803
		[Tooltip("Game Object to play the animation on.")]
		[CheckForComponent(typeof(Animator))]
		[RequiredField]
		public FsmOwnerDefault m_GameObject;

		// Token: 0x0400552C RID: 21804
		public FsmString m_StateName;

		// Token: 0x0400552D RID: 21805
		public FsmString m_LayerName;

		// Token: 0x0400552E RID: 21806
		[Tooltip("Percent of time into the animation at which to start playing.")]
		[HasFloatSlider(0f, 100f)]
		public FsmFloat m_StartTimePercent;

		// Token: 0x0400552F RID: 21807
		private AnimatorStateInfo m_currentAnimationState;

		// Token: 0x04005530 RID: 21808
		private Animator m_checkComplete;

		// Token: 0x04005531 RID: 21809
		private int m_checkLayer = -1;
	}
}
