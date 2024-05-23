using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DFF RID: 3583
	[ActionCategory("Pegasus")]
	[Tooltip("Sets the global time scale.")]
	public class SetTimeScaleAction : FsmStateAction
	{
		// Token: 0x06006E14 RID: 28180 RVA: 0x00204E0D File Offset: 0x0020300D
		public override void Reset()
		{
			this.m_Scale = 1f;
			this.m_EveryFrame = false;
		}

		// Token: 0x06006E15 RID: 28181 RVA: 0x00204E26 File Offset: 0x00203026
		public override void OnEnter()
		{
			this.UpdateScale();
			if (!this.m_EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006E16 RID: 28182 RVA: 0x00204E3F File Offset: 0x0020303F
		public override void OnUpdate()
		{
			this.UpdateScale();
		}

		// Token: 0x06006E17 RID: 28183 RVA: 0x00204E47 File Offset: 0x00203047
		private void UpdateScale()
		{
			if (this.m_Scale.IsNone)
			{
				return;
			}
			Time.timeScale = this.m_Scale.Value;
		}

		// Token: 0x040056B3 RID: 22195
		public FsmFloat m_Scale;

		// Token: 0x040056B4 RID: 22196
		public bool m_EveryFrame;
	}
}
