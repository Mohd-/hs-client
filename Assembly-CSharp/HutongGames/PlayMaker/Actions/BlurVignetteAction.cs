using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DAC RID: 3500
	[Tooltip("Trigger blur+vignette effects action.")]
	public class BlurVignetteAction : FsmStateAction
	{
		// Token: 0x06006CD2 RID: 27858 RVA: 0x002000DC File Offset: 0x001FE2DC
		public override void OnEnter()
		{
			if (this.m_isBlurred.Value)
			{
				BlurVignetteAction.ActionType actionType = this.m_actionType;
				if (actionType != BlurVignetteAction.ActionType.Start)
				{
					if (actionType == BlurVignetteAction.ActionType.End)
					{
						FullScreenFXMgr.Get().EndStandardBlurVignette(this.m_blurTime.Value, null);
					}
				}
				else
				{
					FullScreenFXMgr.Get().StartStandardBlurVignette(this.m_blurTime.Value);
				}
			}
			else
			{
				BlurVignetteAction.ActionType actionType = this.m_actionType;
				if (actionType != BlurVignetteAction.ActionType.Start)
				{
					if (actionType == BlurVignetteAction.ActionType.End)
					{
						FullScreenFXMgr.Get().StopVignette(this.m_blurTime.Value, iTween.EaseType.easeInOutCubic, null);
					}
				}
				else
				{
					FullScreenFXMgr.Get().Vignette(0.8f, this.m_blurTime.Value, iTween.EaseType.easeInOutCubic, null);
				}
			}
		}

		// Token: 0x0400556F RID: 21871
		public FsmFloat m_blurTime;

		// Token: 0x04005570 RID: 21872
		public FsmBool m_isBlurred;

		// Token: 0x04005571 RID: 21873
		public BlurVignetteAction.ActionType m_actionType;

		// Token: 0x02000DAD RID: 3501
		public enum ActionType
		{
			// Token: 0x04005573 RID: 21875
			Start,
			// Token: 0x04005574 RID: 21876
			End
		}
	}
}
