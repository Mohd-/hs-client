using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DBC RID: 3516
	[ActionCategory("Pegasus")]
	[Tooltip("Get shadow color")]
	public class GetShadowColor : FsmStateAction
	{
		// Token: 0x06006D0A RID: 27914 RVA: 0x00201320 File Offset: 0x001FF520
		public override void Reset()
		{
			this.m_Color = Color.black;
		}

		// Token: 0x06006D0B RID: 27915 RVA: 0x00201332 File Offset: 0x001FF532
		public override void OnEnter()
		{
			this.m_Color.Value = Board.Get().m_ShadowColor;
			base.Finish();
		}

		// Token: 0x06006D0C RID: 27916 RVA: 0x0020134F File Offset: 0x001FF54F
		public override void OnUpdate()
		{
			this.m_Color.Value = Board.Get().m_ShadowColor;
		}

		// Token: 0x040055BA RID: 21946
		public FsmColor m_Color;
	}
}
