using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C30 RID: 3120
	[Tooltip("Gets the Length of a String.")]
	[ActionCategory(16)]
	public class GetStringLength : FsmStateAction
	{
		// Token: 0x06006623 RID: 26147 RVA: 0x001E39B0 File Offset: 0x001E1BB0
		public override void Reset()
		{
			this.stringVariable = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006624 RID: 26148 RVA: 0x001E39C7 File Offset: 0x001E1BC7
		public override void OnEnter()
		{
			this.DoGetStringLength();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006625 RID: 26149 RVA: 0x001E39E0 File Offset: 0x001E1BE0
		public override void OnUpdate()
		{
			this.DoGetStringLength();
		}

		// Token: 0x06006626 RID: 26150 RVA: 0x001E39E8 File Offset: 0x001E1BE8
		private void DoGetStringLength()
		{
			if (this.stringVariable == null)
			{
				return;
			}
			if (this.storeResult == null)
			{
				return;
			}
			this.storeResult.Value = this.stringVariable.Value.Length;
		}

		// Token: 0x04004DDC RID: 19932
		[RequiredField]
		[UIHint(10)]
		public FsmString stringVariable;

		// Token: 0x04004DDD RID: 19933
		[RequiredField]
		[UIHint(10)]
		public FsmInt storeResult;

		// Token: 0x04004DDE RID: 19934
		public bool everyFrame;
	}
}
