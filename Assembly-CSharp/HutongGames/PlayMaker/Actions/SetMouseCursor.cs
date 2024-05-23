using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CD8 RID: 3288
	[Tooltip("Controls the appearance of Mouse Cursor.")]
	[ActionCategory(5)]
	public class SetMouseCursor : FsmStateAction
	{
		// Token: 0x06006903 RID: 26883 RVA: 0x001ECBC7 File Offset: 0x001EADC7
		public override void Reset()
		{
			this.cursorTexture = null;
			this.hideCursor = false;
			this.lockCursor = false;
		}

		// Token: 0x06006904 RID: 26884 RVA: 0x001ECBE8 File Offset: 0x001EADE8
		public override void OnEnter()
		{
			PlayMakerGUI.LockCursor = this.lockCursor.Value;
			PlayMakerGUI.HideCursor = this.hideCursor.Value;
			PlayMakerGUI.MouseCursor = this.cursorTexture.Value;
			base.Finish();
		}

		// Token: 0x040050B8 RID: 20664
		public FsmTexture cursorTexture;

		// Token: 0x040050B9 RID: 20665
		public FsmBool hideCursor;

		// Token: 0x040050BA RID: 20666
		public FsmBool lockCursor;
	}
}
