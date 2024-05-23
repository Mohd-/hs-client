using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CC6 RID: 3270
	[ActionCategory(7)]
	[Tooltip("Sets the value of an Integer Variable.")]
	public class SetIntValue : FsmStateAction
	{
		// Token: 0x060068B7 RID: 26807 RVA: 0x001EBEAA File Offset: 0x001EA0AA
		public override void Reset()
		{
			this.intVariable = null;
			this.intValue = null;
			this.everyFrame = false;
		}

		// Token: 0x060068B8 RID: 26808 RVA: 0x001EBEC1 File Offset: 0x001EA0C1
		public override void OnEnter()
		{
			this.intVariable.Value = this.intValue.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060068B9 RID: 26809 RVA: 0x001EBEEA File Offset: 0x001EA0EA
		public override void OnUpdate()
		{
			this.intVariable.Value = this.intValue.Value;
		}

		// Token: 0x04005081 RID: 20609
		[RequiredField]
		[UIHint(10)]
		public FsmInt intVariable;

		// Token: 0x04005082 RID: 20610
		[RequiredField]
		public FsmInt intValue;

		// Token: 0x04005083 RID: 20611
		public bool everyFrame;
	}
}
