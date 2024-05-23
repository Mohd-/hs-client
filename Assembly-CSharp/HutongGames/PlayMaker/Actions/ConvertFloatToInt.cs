using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B78 RID: 2936
	[ActionCategory(17)]
	[Tooltip("Converts a Float value to an Integer value.")]
	public class ConvertFloatToInt : FsmStateAction
	{
		// Token: 0x0600635A RID: 25434 RVA: 0x001DACDB File Offset: 0x001D8EDB
		public override void Reset()
		{
			this.floatVariable = null;
			this.intVariable = null;
			this.rounding = ConvertFloatToInt.FloatRounding.Nearest;
			this.everyFrame = false;
		}

		// Token: 0x0600635B RID: 25435 RVA: 0x001DACF9 File Offset: 0x001D8EF9
		public override void OnEnter()
		{
			this.DoConvertFloatToInt();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600635C RID: 25436 RVA: 0x001DAD12 File Offset: 0x001D8F12
		public override void OnUpdate()
		{
			this.DoConvertFloatToInt();
		}

		// Token: 0x0600635D RID: 25437 RVA: 0x001DAD1C File Offset: 0x001D8F1C
		private void DoConvertFloatToInt()
		{
			switch (this.rounding)
			{
			case ConvertFloatToInt.FloatRounding.RoundDown:
				this.intVariable.Value = Mathf.FloorToInt(this.floatVariable.Value);
				break;
			case ConvertFloatToInt.FloatRounding.RoundUp:
				this.intVariable.Value = Mathf.CeilToInt(this.floatVariable.Value);
				break;
			case ConvertFloatToInt.FloatRounding.Nearest:
				this.intVariable.Value = Mathf.RoundToInt(this.floatVariable.Value);
				break;
			}
		}

		// Token: 0x04004ADF RID: 19167
		[Tooltip("The Float variable to convert to an integer.")]
		[UIHint(10)]
		[RequiredField]
		public FsmFloat floatVariable;

		// Token: 0x04004AE0 RID: 19168
		[RequiredField]
		[UIHint(10)]
		[Tooltip("Store the result in an Integer variable.")]
		public FsmInt intVariable;

		// Token: 0x04004AE1 RID: 19169
		public ConvertFloatToInt.FloatRounding rounding;

		// Token: 0x04004AE2 RID: 19170
		public bool everyFrame;

		// Token: 0x02000B79 RID: 2937
		public enum FloatRounding
		{
			// Token: 0x04004AE4 RID: 19172
			RoundDown,
			// Token: 0x04004AE5 RID: 19173
			RoundUp,
			// Token: 0x04004AE6 RID: 19174
			Nearest
		}
	}
}
