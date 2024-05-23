using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BB2 RID: 2994
	[ActionCategory(7)]
	[Tooltip("Subtracts a value from a Float Variable.")]
	public class FloatSubtract : FsmStateAction
	{
		// Token: 0x0600643A RID: 25658 RVA: 0x001DD41D File Offset: 0x001DB61D
		public override void Reset()
		{
			this.floatVariable = null;
			this.subtract = null;
			this.everyFrame = false;
			this.perSecond = false;
		}

		// Token: 0x0600643B RID: 25659 RVA: 0x001DD43B File Offset: 0x001DB63B
		public override void OnEnter()
		{
			this.DoFloatSubtract();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600643C RID: 25660 RVA: 0x001DD454 File Offset: 0x001DB654
		public override void OnUpdate()
		{
			this.DoFloatSubtract();
		}

		// Token: 0x0600643D RID: 25661 RVA: 0x001DD45C File Offset: 0x001DB65C
		private void DoFloatSubtract()
		{
			if (!this.perSecond)
			{
				this.floatVariable.Value -= this.subtract.Value;
			}
			else
			{
				this.floatVariable.Value -= this.subtract.Value * Time.deltaTime;
			}
		}

		// Token: 0x04004BB0 RID: 19376
		[RequiredField]
		[Tooltip("The float variable to subtract from.")]
		[UIHint(10)]
		public FsmFloat floatVariable;

		// Token: 0x04004BB1 RID: 19377
		[RequiredField]
		[Tooltip("Value to subtract from the float variable.")]
		public FsmFloat subtract;

		// Token: 0x04004BB2 RID: 19378
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		// Token: 0x04004BB3 RID: 19379
		[Tooltip("Used with Every Frame. Adds the value over one second to make the operation frame rate independent.")]
		public bool perSecond;
	}
}
