using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B6E RID: 2926
	[ActionCategory(24)]
	[Tooltip("Samples a Color on a continuous Colors gradient.")]
	public class ColorRamp : FsmStateAction
	{
		// Token: 0x0600632D RID: 25389 RVA: 0x001DA3DC File Offset: 0x001D85DC
		public override void Reset()
		{
			this.colors = new FsmColor[3];
			this.sampleAt = 0f;
			this.storeColor = null;
			this.everyFrame = false;
		}

		// Token: 0x0600632E RID: 25390 RVA: 0x001DA413 File Offset: 0x001D8613
		public override void OnEnter()
		{
			this.DoColorRamp();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600632F RID: 25391 RVA: 0x001DA42C File Offset: 0x001D862C
		public override void OnUpdate()
		{
			this.DoColorRamp();
		}

		// Token: 0x06006330 RID: 25392 RVA: 0x001DA434 File Offset: 0x001D8634
		private void DoColorRamp()
		{
			if (this.colors == null)
			{
				return;
			}
			if (this.colors.Length == 0)
			{
				return;
			}
			if (this.sampleAt == null)
			{
				return;
			}
			if (this.storeColor == null)
			{
				return;
			}
			float num = Mathf.Clamp(this.sampleAt.Value, 0f, (float)(this.colors.Length - 1));
			Color value;
			if (num == 0f)
			{
				value = this.colors[0].Value;
			}
			else if (num == (float)this.colors.Length)
			{
				value = this.colors[this.colors.Length - 1].Value;
			}
			else
			{
				Color value2 = this.colors[Mathf.FloorToInt(num)].Value;
				Color value3 = this.colors[Mathf.CeilToInt(num)].Value;
				num -= Mathf.Floor(num);
				value = Color.Lerp(value2, value3, num);
			}
			this.storeColor.Value = value;
		}

		// Token: 0x06006331 RID: 25393 RVA: 0x001DA521 File Offset: 0x001D8721
		public override string ErrorCheck()
		{
			if (this.colors.Length < 2)
			{
				return "Define at least 2 colors to make a gradient.";
			}
			return null;
		}

		// Token: 0x04004AA9 RID: 19113
		[RequiredField]
		[Tooltip("Array of colors to defining the gradient.")]
		public FsmColor[] colors;

		// Token: 0x04004AAA RID: 19114
		[Tooltip("Point on the gradient to sample. Should be between 0 and the number of colors in the gradient.")]
		[RequiredField]
		public FsmFloat sampleAt;

		// Token: 0x04004AAB RID: 19115
		[Tooltip("Store the sampled color in a Color variable.")]
		[UIHint(10)]
		[RequiredField]
		public FsmColor storeColor;

		// Token: 0x04004AAC RID: 19116
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;
	}
}
