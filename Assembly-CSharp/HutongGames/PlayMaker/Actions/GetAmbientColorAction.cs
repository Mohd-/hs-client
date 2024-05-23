using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DB6 RID: 3510
	[Tooltip("Get scene ambient color")]
	[ActionCategory("Pegasus")]
	public class GetAmbientColorAction : FsmStateAction
	{
		// Token: 0x06006CF4 RID: 27892 RVA: 0x00200E27 File Offset: 0x001FF027
		public override void Reset()
		{
			this.m_Color = Color.white;
			this.m_EveryFrame = false;
		}

		// Token: 0x06006CF5 RID: 27893 RVA: 0x00200E40 File Offset: 0x001FF040
		public override void OnEnter()
		{
			this.m_Color.Value = RenderSettings.ambientLight;
			if (!this.m_EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006CF6 RID: 27894 RVA: 0x00200E63 File Offset: 0x001FF063
		public override void OnUpdate()
		{
			this.m_Color.Value = RenderSettings.ambientLight;
		}

		// Token: 0x040055AC RID: 21932
		public FsmColor m_Color;

		// Token: 0x040055AD RID: 21933
		public bool m_EveryFrame;
	}
}
