using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BAE RID: 2990
	[Tooltip("Multiplies one Float by another.")]
	[ActionCategory(7)]
	public class FloatMultiply : FsmStateAction
	{
		// Token: 0x0600642B RID: 25643 RVA: 0x001DD1DA File Offset: 0x001DB3DA
		public override void Reset()
		{
			this.floatVariable = null;
			this.multiplyBy = null;
			this.everyFrame = false;
		}

		// Token: 0x0600642C RID: 25644 RVA: 0x001DD1F1 File Offset: 0x001DB3F1
		public override void OnEnter()
		{
			this.floatVariable.Value *= this.multiplyBy.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600642D RID: 25645 RVA: 0x001DD221 File Offset: 0x001DB421
		public override void OnUpdate()
		{
			this.floatVariable.Value *= this.multiplyBy.Value;
		}

		// Token: 0x04004B9D RID: 19357
		[Tooltip("The float variable to multiply.")]
		[RequiredField]
		[UIHint(10)]
		public FsmFloat floatVariable;

		// Token: 0x04004B9E RID: 19358
		[Tooltip("Multiply the float variable by this value.")]
		[RequiredField]
		public FsmFloat multiplyBy;

		// Token: 0x04004B9F RID: 19359
		[Tooltip("Repeat every frame. Useful if the variables are changing.")]
		public bool everyFrame;
	}
}
