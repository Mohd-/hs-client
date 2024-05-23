using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BAD RID: 2989
	[ActionCategory(7)]
	[Tooltip("Interpolates between 2 Float values over a specified Time.")]
	public class FloatInterpolate : FsmStateAction
	{
		// Token: 0x06006427 RID: 25639 RVA: 0x001DD048 File Offset: 0x001DB248
		public override void Reset()
		{
			this.mode = 0;
			this.fromFloat = null;
			this.toFloat = null;
			this.time = 1f;
			this.storeResult = null;
			this.finishEvent = null;
			this.realTime = false;
		}

		// Token: 0x06006428 RID: 25640 RVA: 0x001DD090 File Offset: 0x001DB290
		public override void OnEnter()
		{
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.currentTime = 0f;
			if (this.storeResult == null)
			{
				base.Finish();
			}
			else
			{
				this.storeResult.Value = this.fromFloat.Value;
			}
		}

		// Token: 0x06006429 RID: 25641 RVA: 0x001DD0E0 File Offset: 0x001DB2E0
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
			float num = this.currentTime / this.time.Value;
			InterpolationType interpolationType = this.mode;
			if (interpolationType != null)
			{
				if (interpolationType == 1)
				{
					this.storeResult.Value = Mathf.SmoothStep(this.fromFloat.Value, this.toFloat.Value, num);
				}
			}
			else
			{
				this.storeResult.Value = Mathf.Lerp(this.fromFloat.Value, this.toFloat.Value, num);
			}
			if (num > 1f)
			{
				if (this.finishEvent != null)
				{
					base.Fsm.Event(this.finishEvent);
				}
				base.Finish();
			}
		}

		// Token: 0x04004B94 RID: 19348
		[Tooltip("Interpolation mode: Linear or EaseInOut.")]
		public InterpolationType mode;

		// Token: 0x04004B95 RID: 19349
		[Tooltip("Interpolate from this value.")]
		[RequiredField]
		public FsmFloat fromFloat;

		// Token: 0x04004B96 RID: 19350
		[RequiredField]
		[Tooltip("Interpolate to this value.")]
		public FsmFloat toFloat;

		// Token: 0x04004B97 RID: 19351
		[RequiredField]
		[Tooltip("Interpolate over this amount of time in seconds.")]
		public FsmFloat time;

		// Token: 0x04004B98 RID: 19352
		[Tooltip("Store the current value in a float variable.")]
		[UIHint(10)]
		[RequiredField]
		public FsmFloat storeResult;

		// Token: 0x04004B99 RID: 19353
		[Tooltip("Event to send when the interpolation is finished.")]
		public FsmEvent finishEvent;

		// Token: 0x04004B9A RID: 19354
		[Tooltip("Ignore TimeScale. Useful if the game is paused (Time scaled to 0).")]
		public bool realTime;

		// Token: 0x04004B9B RID: 19355
		private float startTime;

		// Token: 0x04004B9C RID: 19356
		private float currentTime;
	}
}
