using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DFE RID: 3582
	[ActionCategory("Pegasus")]
	[Tooltip("Gets the global time scale into a variable.")]
	public class GetTimeScaleAction : FsmStateAction
	{
		// Token: 0x06006E0F RID: 28175 RVA: 0x00204DB1 File Offset: 0x00202FB1
		public override void Reset()
		{
			this.m_Scale = null;
			this.m_EveryFrame = false;
		}

		// Token: 0x06006E10 RID: 28176 RVA: 0x00204DC1 File Offset: 0x00202FC1
		public override void OnEnter()
		{
			this.UpdateScale();
			if (!this.m_EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006E11 RID: 28177 RVA: 0x00204DDA File Offset: 0x00202FDA
		public override void OnUpdate()
		{
			this.UpdateScale();
		}

		// Token: 0x06006E12 RID: 28178 RVA: 0x00204DE2 File Offset: 0x00202FE2
		private void UpdateScale()
		{
			if (this.m_Scale.IsNone)
			{
				return;
			}
			this.m_Scale.Value = Time.timeScale;
		}

		// Token: 0x040056B1 RID: 22193
		[UIHint(10)]
		[RequiredField]
		public FsmFloat m_Scale;

		// Token: 0x040056B2 RID: 22194
		public bool m_EveryFrame;
	}
}
