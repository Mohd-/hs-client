using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CC5 RID: 3269
	[Tooltip("Sets the value of an integer variable using a float value.")]
	[ActionCategory(7)]
	public class SetIntFromFloat : FsmStateAction
	{
		// Token: 0x060068B3 RID: 26803 RVA: 0x001EBE3D File Offset: 0x001EA03D
		public override void Reset()
		{
			this.intVariable = null;
			this.floatValue = null;
			this.everyFrame = false;
		}

		// Token: 0x060068B4 RID: 26804 RVA: 0x001EBE54 File Offset: 0x001EA054
		public override void OnEnter()
		{
			this.intVariable.Value = (int)this.floatValue.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060068B5 RID: 26805 RVA: 0x001EBE89 File Offset: 0x001EA089
		public override void OnUpdate()
		{
			this.intVariable.Value = (int)this.floatValue.Value;
		}

		// Token: 0x0400507E RID: 20606
		[RequiredField]
		[UIHint(10)]
		public FsmInt intVariable;

		// Token: 0x0400507F RID: 20607
		public FsmFloat floatValue;

		// Token: 0x04005080 RID: 20608
		public bool everyFrame;
	}
}
