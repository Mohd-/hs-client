using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C46 RID: 3142
	[ActionCategory(7)]
	[Tooltip("Clamp the value of an Integer Variable to a Min/Max range.")]
	public class IntClamp : FsmStateAction
	{
		// Token: 0x06006682 RID: 26242 RVA: 0x001E4925 File Offset: 0x001E2B25
		public override void Reset()
		{
			this.intVariable = null;
			this.minValue = null;
			this.maxValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006683 RID: 26243 RVA: 0x001E4943 File Offset: 0x001E2B43
		public override void OnEnter()
		{
			this.DoClamp();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006684 RID: 26244 RVA: 0x001E495C File Offset: 0x001E2B5C
		public override void OnUpdate()
		{
			this.DoClamp();
		}

		// Token: 0x06006685 RID: 26245 RVA: 0x001E4964 File Offset: 0x001E2B64
		private void DoClamp()
		{
			this.intVariable.Value = Mathf.Clamp(this.intVariable.Value, this.minValue.Value, this.maxValue.Value);
		}

		// Token: 0x04004E35 RID: 20021
		[RequiredField]
		[UIHint(10)]
		public FsmInt intVariable;

		// Token: 0x04004E36 RID: 20022
		[RequiredField]
		public FsmInt minValue;

		// Token: 0x04004E37 RID: 20023
		[RequiredField]
		public FsmInt maxValue;

		// Token: 0x04004E38 RID: 20024
		public bool everyFrame;
	}
}
