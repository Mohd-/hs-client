using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BAC RID: 2988
	[ActionCategory(7)]
	[Tooltip("Divides one Float by another.")]
	public class FloatDivide : FsmStateAction
	{
		// Token: 0x06006423 RID: 25635 RVA: 0x001DCFD9 File Offset: 0x001DB1D9
		public override void Reset()
		{
			this.floatVariable = null;
			this.divideBy = null;
			this.everyFrame = false;
		}

		// Token: 0x06006424 RID: 25636 RVA: 0x001DCFF0 File Offset: 0x001DB1F0
		public override void OnEnter()
		{
			this.floatVariable.Value /= this.divideBy.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006425 RID: 25637 RVA: 0x001DD020 File Offset: 0x001DB220
		public override void OnUpdate()
		{
			this.floatVariable.Value /= this.divideBy.Value;
		}

		// Token: 0x04004B91 RID: 19345
		[Tooltip("The float variable to divide.")]
		[UIHint(10)]
		[RequiredField]
		public FsmFloat floatVariable;

		// Token: 0x04004B92 RID: 19346
		[RequiredField]
		[Tooltip("Divide the float variable by this value.")]
		public FsmFloat divideBy;

		// Token: 0x04004B93 RID: 19347
		[Tooltip("Repeate every frame. Useful if the variables are changing.")]
		public bool everyFrame;
	}
}
