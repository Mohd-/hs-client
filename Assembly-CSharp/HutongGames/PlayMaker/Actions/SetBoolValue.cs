using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C9D RID: 3229
	[ActionCategory(7)]
	[Tooltip("Sets the value of a Bool Variable.")]
	public class SetBoolValue : FsmStateAction
	{
		// Token: 0x060067FF RID: 26623 RVA: 0x001E9F19 File Offset: 0x001E8119
		public override void Reset()
		{
			this.boolVariable = null;
			this.boolValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006800 RID: 26624 RVA: 0x001E9F30 File Offset: 0x001E8130
		public override void OnEnter()
		{
			this.boolVariable.Value = this.boolValue.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006801 RID: 26625 RVA: 0x001E9F59 File Offset: 0x001E8159
		public override void OnUpdate()
		{
			this.boolVariable.Value = this.boolValue.Value;
		}

		// Token: 0x04004FCB RID: 20427
		[UIHint(10)]
		[RequiredField]
		public FsmBool boolVariable;

		// Token: 0x04004FCC RID: 20428
		[RequiredField]
		public FsmBool boolValue;

		// Token: 0x04004FCD RID: 20429
		public bool everyFrame;
	}
}
