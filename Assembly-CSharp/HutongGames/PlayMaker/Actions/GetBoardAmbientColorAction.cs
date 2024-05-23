using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DB7 RID: 3511
	[ActionCategory("Pegasus")]
	[Tooltip("Get scene ambient color")]
	public class GetBoardAmbientColorAction : FsmStateAction
	{
		// Token: 0x06006CF8 RID: 27896 RVA: 0x00200E7D File Offset: 0x001FF07D
		public override void Reset()
		{
			this.m_Color = Color.white;
		}

		// Token: 0x06006CF9 RID: 27897 RVA: 0x00200E90 File Offset: 0x001FF090
		public override void OnEnter()
		{
			this.m_Color.Value = RenderSettings.ambientLight;
			Board board = Board.Get();
			if (board != null)
			{
				this.m_Color.Value = board.m_AmbientColor;
			}
			base.Finish();
		}

		// Token: 0x040055AE RID: 21934
		public FsmColor m_Color;
	}
}
