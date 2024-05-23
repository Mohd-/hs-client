using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DD9 RID: 3545
	[ActionCategory("Pegasus")]
	[Tooltip("Set scene ambient color")]
	public class SetAmbientColorAction : FsmStateAction
	{
		// Token: 0x06006D81 RID: 28033 RVA: 0x00202E5A File Offset: 0x0020105A
		public override void Reset()
		{
			this.m_Color = null;
			this.m_EveryFrame = false;
		}

		// Token: 0x06006D82 RID: 28034 RVA: 0x00202E6A File Offset: 0x0020106A
		public override void OnEnter()
		{
			RenderSettings.ambientLight = this.m_Color.Value;
			if (!this.m_EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D83 RID: 28035 RVA: 0x00202E8D File Offset: 0x0020108D
		public override void OnUpdate()
		{
			RenderSettings.ambientLight = this.m_Color.Value;
		}

		// Token: 0x04005629 RID: 22057
		public FsmColor m_Color;

		// Token: 0x0400562A RID: 22058
		public bool m_EveryFrame;
	}
}
