using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DD5 RID: 3541
	[Tooltip("Set scene ambient color")]
	[ActionCategory("Pegasus")]
	public class ResetAmbientColorAction : FsmStateAction
	{
		// Token: 0x06006D6E RID: 28014 RVA: 0x00202B1A File Offset: 0x00200D1A
		public override void Reset()
		{
		}

		// Token: 0x06006D6F RID: 28015 RVA: 0x00202B1C File Offset: 0x00200D1C
		public override void OnEnter()
		{
			Board board = Board.Get();
			if (board != null)
			{
				board.ResetAmbientColor();
			}
			base.Finish();
		}

		// Token: 0x0400561C RID: 22044
		private SetRenderSettings m_renderSettings;
	}
}
