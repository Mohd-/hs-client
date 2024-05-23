using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C32 RID: 3122
	[Tooltip("Gets a sub-string from a String Variable.")]
	[ActionCategory(16)]
	public class GetSubstring : FsmStateAction
	{
		// Token: 0x0600662D RID: 26157 RVA: 0x001E3AD3 File Offset: 0x001E1CD3
		public override void Reset()
		{
			this.stringVariable = null;
			this.startIndex = 0;
			this.length = 1;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x0600662E RID: 26158 RVA: 0x001E3B02 File Offset: 0x001E1D02
		public override void OnEnter()
		{
			this.DoGetSubstring();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600662F RID: 26159 RVA: 0x001E3B1B File Offset: 0x001E1D1B
		public override void OnUpdate()
		{
			this.DoGetSubstring();
		}

		// Token: 0x06006630 RID: 26160 RVA: 0x001E3B24 File Offset: 0x001E1D24
		private void DoGetSubstring()
		{
			if (this.stringVariable == null)
			{
				return;
			}
			if (this.storeResult == null)
			{
				return;
			}
			this.storeResult.Value = this.stringVariable.Value.Substring(this.startIndex.Value, this.length.Value);
		}

		// Token: 0x04004DE3 RID: 19939
		[RequiredField]
		[UIHint(10)]
		public FsmString stringVariable;

		// Token: 0x04004DE4 RID: 19940
		[RequiredField]
		public FsmInt startIndex;

		// Token: 0x04004DE5 RID: 19941
		[RequiredField]
		public FsmInt length;

		// Token: 0x04004DE6 RID: 19942
		[RequiredField]
		[UIHint(10)]
		public FsmString storeResult;

		// Token: 0x04004DE7 RID: 19943
		public bool everyFrame;
	}
}
