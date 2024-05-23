using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B6D RID: 2925
	[Tooltip("Interpolate through an array of Colors over a specified amount of Time.")]
	[ActionCategory(24)]
	public class ColorInterpolate : FsmStateAction
	{
		// Token: 0x06006328 RID: 25384 RVA: 0x001DA1B4 File Offset: 0x001D83B4
		public override void Reset()
		{
			this.colors = new FsmColor[3];
			this.time = 1f;
			this.storeColor = null;
			this.finishEvent = null;
			this.realTime = false;
		}

		// Token: 0x06006329 RID: 25385 RVA: 0x001DA1E8 File Offset: 0x001D83E8
		public override void OnEnter()
		{
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.currentTime = 0f;
			if (this.colors.Length < 2)
			{
				if (this.colors.Length == 1)
				{
					this.storeColor.Value = this.colors[0].Value;
				}
				base.Finish();
			}
			else
			{
				this.storeColor.Value = this.colors[0].Value;
			}
		}

		// Token: 0x0600632A RID: 25386 RVA: 0x001DA264 File Offset: 0x001D8464
		public override void OnUpdate()
		{
			if (this.realTime)
			{
				this.currentTime = FsmTime.RealtimeSinceStartup - this.startTime;
			}
			else
			{
				this.currentTime += Time.deltaTime;
			}
			if (this.currentTime > this.time.Value)
			{
				base.Finish();
				this.storeColor.Value = this.colors[this.colors.Length - 1].Value;
				if (this.finishEvent != null)
				{
					base.Fsm.Event(this.finishEvent);
				}
				return;
			}
			float num = (float)(this.colors.Length - 1) * this.currentTime / this.time.Value;
			Color value;
			if (num.Equals(0f))
			{
				value = this.colors[0].Value;
			}
			else if (num.Equals((float)(this.colors.Length - 1)))
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

		// Token: 0x0600632B RID: 25387 RVA: 0x001DA3B9 File Offset: 0x001D85B9
		public override string ErrorCheck()
		{
			return (this.colors.Length >= 2) ? null : "Define at least 2 colors to make a gradient.";
		}

		// Token: 0x04004AA2 RID: 19106
		[RequiredField]
		[Tooltip("Array of colors to interpolate through.")]
		public FsmColor[] colors;

		// Token: 0x04004AA3 RID: 19107
		[RequiredField]
		[Tooltip("Interpolation time.")]
		public FsmFloat time;

		// Token: 0x04004AA4 RID: 19108
		[RequiredField]
		[Tooltip("Store the interpolated color in a Color variable.")]
		[UIHint(10)]
		public FsmColor storeColor;

		// Token: 0x04004AA5 RID: 19109
		[Tooltip("Event to send when the interpolation finishes.")]
		public FsmEvent finishEvent;

		// Token: 0x04004AA6 RID: 19110
		[Tooltip("Ignore TimeScale")]
		public bool realTime;

		// Token: 0x04004AA7 RID: 19111
		private float startTime;

		// Token: 0x04004AA8 RID: 19112
		private float currentTime;
	}
}
