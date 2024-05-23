using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BF5 RID: 3061
	[Tooltip("Get the RGBA channels of a Color Variable and store them in Float Variables.")]
	[ActionCategory(24)]
	public class GetColorRGBA : FsmStateAction
	{
		// Token: 0x06006525 RID: 25893 RVA: 0x001E0AD8 File Offset: 0x001DECD8
		public override void Reset()
		{
			this.color = null;
			this.storeRed = null;
			this.storeGreen = null;
			this.storeBlue = null;
			this.storeAlpha = null;
			this.everyFrame = false;
		}

		// Token: 0x06006526 RID: 25894 RVA: 0x001E0B0F File Offset: 0x001DED0F
		public override void OnEnter()
		{
			this.DoGetColorRGBA();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006527 RID: 25895 RVA: 0x001E0B28 File Offset: 0x001DED28
		public override void OnUpdate()
		{
			this.DoGetColorRGBA();
		}

		// Token: 0x06006528 RID: 25896 RVA: 0x001E0B30 File Offset: 0x001DED30
		private void DoGetColorRGBA()
		{
			if (this.color.IsNone)
			{
				return;
			}
			this.storeRed.Value = this.color.Value.r;
			this.storeGreen.Value = this.color.Value.g;
			this.storeBlue.Value = this.color.Value.b;
			this.storeAlpha.Value = this.color.Value.a;
		}

		// Token: 0x04004CBF RID: 19647
		[UIHint(10)]
		[Tooltip("The Color variable.")]
		[RequiredField]
		public FsmColor color;

		// Token: 0x04004CC0 RID: 19648
		[Tooltip("Store the red channel in a float variable.")]
		[UIHint(10)]
		public FsmFloat storeRed;

		// Token: 0x04004CC1 RID: 19649
		[UIHint(10)]
		[Tooltip("Store the green channel in a float variable.")]
		public FsmFloat storeGreen;

		// Token: 0x04004CC2 RID: 19650
		[UIHint(10)]
		[Tooltip("Store the blue channel in a float variable.")]
		public FsmFloat storeBlue;

		// Token: 0x04004CC3 RID: 19651
		[UIHint(10)]
		[Tooltip("Store the alpha channel in a float variable.")]
		public FsmFloat storeAlpha;

		// Token: 0x04004CC4 RID: 19652
		[Tooltip("Repeat every frame. Useful if the color variable is changing.")]
		public bool everyFrame;
	}
}
