using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B6F RID: 2927
	[Tooltip("Adds a text area to the action list. NOTE: Doesn't do anything, just for notes...")]
	[ActionCategory(2)]
	public class Comment : FsmStateAction
	{
		// Token: 0x06006333 RID: 25395 RVA: 0x001DA540 File Offset: 0x001D8740
		public override void Reset()
		{
			this.comment = string.Empty;
		}

		// Token: 0x06006334 RID: 25396 RVA: 0x001DA54D File Offset: 0x001D874D
		public override void OnEnter()
		{
			base.Finish();
		}

		// Token: 0x04004AAD RID: 19117
		[UIHint(12)]
		public string comment;
	}
}
