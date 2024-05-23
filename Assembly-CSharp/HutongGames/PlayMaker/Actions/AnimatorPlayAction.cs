using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D9B RID: 3483
	[Tooltip("Enables an Animator and plays one of its states.")]
	[ActionCategory("Pegasus")]
	public class AnimatorPlayAction : FsmStateAction
	{
		// Token: 0x06006C8D RID: 27789 RVA: 0x001FEEF8 File Offset: 0x001FD0F8
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

		// Token: 0x06006C8E RID: 27790 RVA: 0x001FEF38 File Offset: 0x001FD138
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
			}
			base.Finish();
		}

		// Token: 0x04005527 RID: 21799
		[Tooltip("Game Object to play the animation on.")]
		[CheckForComponent(typeof(Animator))]
		[RequiredField]
		public FsmOwnerDefault m_GameObject;

		// Token: 0x04005528 RID: 21800
		public FsmString m_StateName;

		// Token: 0x04005529 RID: 21801
		public FsmString m_LayerName;

		// Token: 0x0400552A RID: 21802
		[HasFloatSlider(0f, 100f)]
		[Tooltip("Percent of time into the animation at which to start playing.")]
		public FsmFloat m_StartTimePercent;
	}
}
