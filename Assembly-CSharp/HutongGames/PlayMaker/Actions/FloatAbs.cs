using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BA6 RID: 2982
	[ActionCategory(7)]
	[Tooltip("Sets a Float variable to its absolute value.")]
	public class FloatAbs : FsmStateAction
	{
		// Token: 0x06006405 RID: 25605 RVA: 0x001DCBC1 File Offset: 0x001DADC1
		public override void Reset()
		{
			this.floatVariable = null;
			this.everyFrame = false;
		}

		// Token: 0x06006406 RID: 25606 RVA: 0x001DCBD1 File Offset: 0x001DADD1
		public override void OnEnter()
		{
			this.DoFloatAbs();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006407 RID: 25607 RVA: 0x001DCBEA File Offset: 0x001DADEA
		public override void OnUpdate()
		{
			this.DoFloatAbs();
		}

		// Token: 0x06006408 RID: 25608 RVA: 0x001DCBF2 File Offset: 0x001DADF2
		private void DoFloatAbs()
		{
			this.floatVariable.Value = Mathf.Abs(this.floatVariable.Value);
		}

		// Token: 0x04004B79 RID: 19321
		[RequiredField]
		[Tooltip("The Float variable.")]
		[UIHint(10)]
		public FsmFloat floatVariable;

		// Token: 0x04004B7A RID: 19322
		[Tooltip("Repeat every frame. Useful if the Float variable is changing.")]
		public bool everyFrame;
	}
}
