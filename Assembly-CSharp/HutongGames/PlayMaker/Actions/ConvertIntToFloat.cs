using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B7B RID: 2939
	[Tooltip("Converts an Integer value to a Float value.")]
	[ActionCategory(17)]
	public class ConvertIntToFloat : FsmStateAction
	{
		// Token: 0x06006364 RID: 25444 RVA: 0x001DAE76 File Offset: 0x001D9076
		public override void Reset()
		{
			this.intVariable = null;
			this.floatVariable = null;
			this.everyFrame = false;
		}

		// Token: 0x06006365 RID: 25445 RVA: 0x001DAE8D File Offset: 0x001D908D
		public override void OnEnter()
		{
			this.DoConvertIntToFloat();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006366 RID: 25446 RVA: 0x001DAEA6 File Offset: 0x001D90A6
		public override void OnUpdate()
		{
			this.DoConvertIntToFloat();
		}

		// Token: 0x06006367 RID: 25447 RVA: 0x001DAEAE File Offset: 0x001D90AE
		private void DoConvertIntToFloat()
		{
			this.floatVariable.Value = (float)this.intVariable.Value;
		}

		// Token: 0x04004AEB RID: 19179
		[RequiredField]
		[UIHint(10)]
		[Tooltip("The Integer variable to convert to a float.")]
		public FsmInt intVariable;

		// Token: 0x04004AEC RID: 19180
		[Tooltip("Store the result in a Float variable.")]
		[UIHint(10)]
		[RequiredField]
		public FsmFloat floatVariable;

		// Token: 0x04004AED RID: 19181
		[Tooltip("Repeat every frame. Useful if the Integer variable is changing.")]
		public bool everyFrame;
	}
}
