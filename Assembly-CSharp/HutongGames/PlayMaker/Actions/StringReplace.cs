using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CFB RID: 3323
	[ActionCategory(16)]
	[Tooltip("Replace a substring with a new String.")]
	public class StringReplace : FsmStateAction
	{
		// Token: 0x060069A1 RID: 27041 RVA: 0x001EF186 File Offset: 0x001ED386
		public override void Reset()
		{
			this.stringVariable = null;
			this.replace = string.Empty;
			this.with = string.Empty;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x060069A2 RID: 27042 RVA: 0x001EF1BD File Offset: 0x001ED3BD
		public override void OnEnter()
		{
			this.DoReplace();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060069A3 RID: 27043 RVA: 0x001EF1D6 File Offset: 0x001ED3D6
		public override void OnUpdate()
		{
			this.DoReplace();
		}

		// Token: 0x060069A4 RID: 27044 RVA: 0x001EF1E0 File Offset: 0x001ED3E0
		private void DoReplace()
		{
			if (this.stringVariable == null)
			{
				return;
			}
			if (this.storeResult == null)
			{
				return;
			}
			this.storeResult.Value = this.stringVariable.Value.Replace(this.replace.Value, this.with.Value);
		}

		// Token: 0x0400515B RID: 20827
		[UIHint(10)]
		[RequiredField]
		public FsmString stringVariable;

		// Token: 0x0400515C RID: 20828
		public FsmString replace;

		// Token: 0x0400515D RID: 20829
		public FsmString with;

		// Token: 0x0400515E RID: 20830
		[RequiredField]
		[UIHint(10)]
		public FsmString storeResult;

		// Token: 0x0400515F RID: 20831
		public bool everyFrame;
	}
}
