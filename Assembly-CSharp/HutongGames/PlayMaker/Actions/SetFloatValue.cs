using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CA6 RID: 3238
	[Tooltip("Sets the value of a Float Variable.")]
	[ActionCategory(7)]
	public class SetFloatValue : FsmStateAction
	{
		// Token: 0x06006827 RID: 26663 RVA: 0x001EA5AB File Offset: 0x001E87AB
		public override void Reset()
		{
			this.floatVariable = null;
			this.floatValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006828 RID: 26664 RVA: 0x001EA5C2 File Offset: 0x001E87C2
		public override void OnEnter()
		{
			this.floatVariable.Value = this.floatValue.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006829 RID: 26665 RVA: 0x001EA5EB File Offset: 0x001E87EB
		public override void OnUpdate()
		{
			this.floatVariable.Value = this.floatValue.Value;
		}

		// Token: 0x04004FF1 RID: 20465
		[RequiredField]
		[UIHint(10)]
		public FsmFloat floatVariable;

		// Token: 0x04004FF2 RID: 20466
		[RequiredField]
		public FsmFloat floatValue;

		// Token: 0x04004FF3 RID: 20467
		public bool everyFrame;
	}
}
