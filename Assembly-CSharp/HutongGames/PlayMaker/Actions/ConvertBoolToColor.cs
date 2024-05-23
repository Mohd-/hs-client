using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B74 RID: 2932
	[ActionCategory(17)]
	[Tooltip("Converts a Bool value to a Color.")]
	public class ConvertBoolToColor : FsmStateAction
	{
		// Token: 0x06006346 RID: 25414 RVA: 0x001DAA54 File Offset: 0x001D8C54
		public override void Reset()
		{
			this.boolVariable = null;
			this.colorVariable = null;
			this.falseColor = Color.black;
			this.trueColor = Color.white;
			this.everyFrame = false;
		}

		// Token: 0x06006347 RID: 25415 RVA: 0x001DAA8B File Offset: 0x001D8C8B
		public override void OnEnter()
		{
			this.DoConvertBoolToColor();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006348 RID: 25416 RVA: 0x001DAAA4 File Offset: 0x001D8CA4
		public override void OnUpdate()
		{
			this.DoConvertBoolToColor();
		}

		// Token: 0x06006349 RID: 25417 RVA: 0x001DAAAC File Offset: 0x001D8CAC
		private void DoConvertBoolToColor()
		{
			this.colorVariable.Value = ((!this.boolVariable.Value) ? this.falseColor.Value : this.trueColor.Value);
		}

		// Token: 0x04004ACB RID: 19147
		[Tooltip("The Bool variable to test.")]
		[RequiredField]
		[UIHint(10)]
		public FsmBool boolVariable;

		// Token: 0x04004ACC RID: 19148
		[Tooltip("The Color variable to set based on the bool variable value.")]
		[RequiredField]
		[UIHint(10)]
		public FsmColor colorVariable;

		// Token: 0x04004ACD RID: 19149
		[Tooltip("Color if Bool variable is false.")]
		public FsmColor falseColor;

		// Token: 0x04004ACE RID: 19150
		[Tooltip("Color if Bool variable is true.")]
		public FsmColor trueColor;

		// Token: 0x04004ACF RID: 19151
		[Tooltip("Repeat every frame. Useful if the Bool variable is changing.")]
		public bool everyFrame;
	}
}
