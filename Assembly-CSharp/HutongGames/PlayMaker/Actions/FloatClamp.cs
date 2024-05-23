using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BAA RID: 2986
	[Tooltip("Clamps the value of Float Variable to a Min/Max range.")]
	[ActionCategory(7)]
	public class FloatClamp : FsmStateAction
	{
		// Token: 0x06006418 RID: 25624 RVA: 0x001DCDED File Offset: 0x001DAFED
		public override void Reset()
		{
			this.floatVariable = null;
			this.minValue = null;
			this.maxValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006419 RID: 25625 RVA: 0x001DCE0B File Offset: 0x001DB00B
		public override void OnEnter()
		{
			this.DoClamp();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600641A RID: 25626 RVA: 0x001DCE24 File Offset: 0x001DB024
		public override void OnUpdate()
		{
			this.DoClamp();
		}

		// Token: 0x0600641B RID: 25627 RVA: 0x001DCE2C File Offset: 0x001DB02C
		private void DoClamp()
		{
			this.floatVariable.Value = Mathf.Clamp(this.floatVariable.Value, this.minValue.Value, this.maxValue.Value);
		}

		// Token: 0x04004B86 RID: 19334
		[Tooltip("Float variable to clamp.")]
		[RequiredField]
		[UIHint(10)]
		public FsmFloat floatVariable;

		// Token: 0x04004B87 RID: 19335
		[RequiredField]
		[Tooltip("The minimum value.")]
		public FsmFloat minValue;

		// Token: 0x04004B88 RID: 19336
		[Tooltip("The maximum value.")]
		[RequiredField]
		public FsmFloat maxValue;

		// Token: 0x04004B89 RID: 19337
		[Tooltip("Repeate every frame. Useful if the float variable is changing.")]
		public bool everyFrame;
	}
}
