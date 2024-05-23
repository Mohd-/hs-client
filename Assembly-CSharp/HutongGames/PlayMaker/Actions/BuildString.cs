using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B67 RID: 2919
	[Tooltip("Builds a String from other Strings.")]
	[ActionCategory(16)]
	public class BuildString : FsmStateAction
	{
		// Token: 0x06006301 RID: 25345 RVA: 0x001D9527 File Offset: 0x001D7727
		public override void Reset()
		{
			this.stringParts = new FsmString[3];
			this.separator = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006302 RID: 25346 RVA: 0x001D954A File Offset: 0x001D774A
		public override void OnEnter()
		{
			this.DoBuildString();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006303 RID: 25347 RVA: 0x001D9563 File Offset: 0x001D7763
		public override void OnUpdate()
		{
			this.DoBuildString();
		}

		// Token: 0x06006304 RID: 25348 RVA: 0x001D956C File Offset: 0x001D776C
		private void DoBuildString()
		{
			if (this.storeResult == null)
			{
				return;
			}
			this.result = string.Empty;
			foreach (FsmString fsmString in this.stringParts)
			{
				this.result += fsmString;
				this.result += this.separator.Value;
			}
			this.storeResult.Value = this.result;
		}

		// Token: 0x04004A7A RID: 19066
		[RequiredField]
		[Tooltip("Array of Strings to combine.")]
		public FsmString[] stringParts;

		// Token: 0x04004A7B RID: 19067
		[Tooltip("Separator to insert between each String. E.g. space character.")]
		public FsmString separator;

		// Token: 0x04004A7C RID: 19068
		[Tooltip("Store the final String in a variable.")]
		[RequiredField]
		[UIHint(10)]
		public FsmString storeResult;

		// Token: 0x04004A7D RID: 19069
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		// Token: 0x04004A7E RID: 19070
		private string result;
	}
}
