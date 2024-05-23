using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E00 RID: 3584
	[Tooltip("Interpolate 2 Colors over a specified amount of Time.")]
	[ActionCategory("Pegasus")]
	public class TwoColorInterpolateAction : FsmStateAction
	{
		// Token: 0x06006E19 RID: 28185 RVA: 0x00204E74 File Offset: 0x00203074
		public override void Reset()
		{
			this.color1 = new FsmColor();
			this.color2 = new FsmColor();
			this.color1.Value = Color.black;
			this.color2.Value = Color.white;
			this.time = 1f;
			this.storeColor = null;
			this.finishEvent = null;
			this.realTime = false;
		}

		// Token: 0x06006E1A RID: 28186 RVA: 0x00204EDC File Offset: 0x002030DC
		public override void OnEnter()
		{
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.currentTime = 0f;
			this.storeColor.Value = this.color1.Value;
		}

		// Token: 0x06006E1B RID: 28187 RVA: 0x00204F18 File Offset: 0x00203118
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
				this.storeColor.Value = this.color2.Value;
				if (this.finishEvent != null)
				{
					base.Fsm.Event(this.finishEvent);
				}
				return;
			}
			float num = this.currentTime / this.time.Value;
			Color value;
			if (num.Equals(0f))
			{
				value = this.color1.Value;
			}
			else if (num >= 1f)
			{
				value = this.color2.Value;
			}
			else
			{
				value = Color.Lerp(this.color1.Value, this.color2.Value, num);
			}
			this.storeColor.Value = value;
		}

		// Token: 0x040056B5 RID: 22197
		[Tooltip("Color 1")]
		[RequiredField]
		public FsmColor color1;

		// Token: 0x040056B6 RID: 22198
		[Tooltip("Color 2")]
		[RequiredField]
		public FsmColor color2;

		// Token: 0x040056B7 RID: 22199
		[Tooltip("Interpolation time.")]
		[RequiredField]
		public FsmFloat time;

		// Token: 0x040056B8 RID: 22200
		[RequiredField]
		[UIHint(10)]
		[Tooltip("Store the interpolated color in a Color variable.")]
		public FsmColor storeColor;

		// Token: 0x040056B9 RID: 22201
		[Tooltip("Event to send when the interpolation finishes.")]
		public FsmEvent finishEvent;

		// Token: 0x040056BA RID: 22202
		[Tooltip("Ignore TimeScale")]
		public bool realTime;

		// Token: 0x040056BB RID: 22203
		private float startTime;

		// Token: 0x040056BC RID: 22204
		private float currentTime;
	}
}
