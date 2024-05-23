using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C44 RID: 3140
	[Tooltip("Adds a value to an Integer Variable.")]
	[ActionCategory(7)]
	public class IntAdd : FsmStateAction
	{
		// Token: 0x0600667A RID: 26234 RVA: 0x001E4811 File Offset: 0x001E2A11
		public override void Reset()
		{
			this.intVariable = null;
			this.add = null;
			this.everyFrame = false;
		}

		// Token: 0x0600667B RID: 26235 RVA: 0x001E4828 File Offset: 0x001E2A28
		public override void OnEnter()
		{
			this.intVariable.Value += this.add.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600667C RID: 26236 RVA: 0x001E4858 File Offset: 0x001E2A58
		public override void OnUpdate()
		{
			this.intVariable.Value += this.add.Value;
		}

		// Token: 0x04004E2E RID: 20014
		[RequiredField]
		[UIHint(10)]
		public FsmInt intVariable;

		// Token: 0x04004E2F RID: 20015
		[RequiredField]
		public FsmInt add;

		// Token: 0x04004E30 RID: 20016
		public bool everyFrame;
	}
}
