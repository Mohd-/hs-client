using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CA0 RID: 3232
	[ActionCategory(24)]
	[Tooltip("Sets the RGBA channels of a Color Variable. To leave any channel unchanged, set variable to 'None'.")]
	public class SetColorRGBA : FsmStateAction
	{
		// Token: 0x0600680D RID: 26637 RVA: 0x001EA0B4 File Offset: 0x001E82B4
		public override void Reset()
		{
			this.colorVariable = null;
			this.red = 0f;
			this.green = 0f;
			this.blue = 0f;
			this.alpha = 1f;
			this.everyFrame = false;
		}

		// Token: 0x0600680E RID: 26638 RVA: 0x001EA10F File Offset: 0x001E830F
		public override void OnEnter()
		{
			this.DoSetColorRGBA();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600680F RID: 26639 RVA: 0x001EA128 File Offset: 0x001E8328
		public override void OnUpdate()
		{
			this.DoSetColorRGBA();
		}

		// Token: 0x06006810 RID: 26640 RVA: 0x001EA130 File Offset: 0x001E8330
		private void DoSetColorRGBA()
		{
			if (this.colorVariable == null)
			{
				return;
			}
			Color value = this.colorVariable.Value;
			if (!this.red.IsNone)
			{
				value.r = this.red.Value;
			}
			if (!this.green.IsNone)
			{
				value.g = this.green.Value;
			}
			if (!this.blue.IsNone)
			{
				value.b = this.blue.Value;
			}
			if (!this.alpha.IsNone)
			{
				value.a = this.alpha.Value;
			}
			this.colorVariable.Value = value;
		}

		// Token: 0x04004FD5 RID: 20437
		[RequiredField]
		[UIHint(10)]
		public FsmColor colorVariable;

		// Token: 0x04004FD6 RID: 20438
		[HasFloatSlider(0f, 1f)]
		public FsmFloat red;

		// Token: 0x04004FD7 RID: 20439
		[HasFloatSlider(0f, 1f)]
		public FsmFloat green;

		// Token: 0x04004FD8 RID: 20440
		[HasFloatSlider(0f, 1f)]
		public FsmFloat blue;

		// Token: 0x04004FD9 RID: 20441
		[HasFloatSlider(0f, 1f)]
		public FsmFloat alpha;

		// Token: 0x04004FDA RID: 20442
		public bool everyFrame;
	}
}
