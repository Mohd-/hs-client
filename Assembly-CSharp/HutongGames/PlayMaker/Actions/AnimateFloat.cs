using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B3F RID: 2879
	[Tooltip("Animates the value of a Float Variable using an Animation Curve.")]
	[ActionCategory(37)]
	public class AnimateFloat : FsmStateAction
	{
		// Token: 0x06006248 RID: 25160 RVA: 0x001D387D File Offset: 0x001D1A7D
		public override void Reset()
		{
			this.animCurve = null;
			this.floatVariable = null;
			this.finishEvent = null;
			this.realTime = false;
		}

		// Token: 0x06006249 RID: 25161 RVA: 0x001D389C File Offset: 0x001D1A9C
		public override void OnEnter()
		{
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.currentTime = 0f;
			if (this.animCurve != null && this.animCurve.curve != null && this.animCurve.curve.keys.Length > 0)
			{
				this.endTime = this.animCurve.curve.keys[this.animCurve.curve.length - 1].time;
				this.looping = ActionHelpers.IsLoopingWrapMode(this.animCurve.curve.postWrapMode);
				this.floatVariable.Value = this.animCurve.curve.Evaluate(0f);
				return;
			}
			base.Finish();
		}

		// Token: 0x0600624A RID: 25162 RVA: 0x001D396C File Offset: 0x001D1B6C
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
			if (this.animCurve != null && this.animCurve.curve != null && this.floatVariable != null)
			{
				this.floatVariable.Value = this.animCurve.curve.Evaluate(this.currentTime);
			}
			if (this.currentTime >= this.endTime)
			{
				if (!this.looping)
				{
					base.Finish();
				}
				if (this.finishEvent != null)
				{
					base.Fsm.Event(this.finishEvent);
				}
			}
		}

		// Token: 0x04004956 RID: 18774
		[Tooltip("The animation curve to use.")]
		[RequiredField]
		public FsmAnimationCurve animCurve;

		// Token: 0x04004957 RID: 18775
		[UIHint(10)]
		[Tooltip("The float variable to set.")]
		[RequiredField]
		public FsmFloat floatVariable;

		// Token: 0x04004958 RID: 18776
		[Tooltip("Optionally send an Event when the animation finishes.")]
		public FsmEvent finishEvent;

		// Token: 0x04004959 RID: 18777
		[Tooltip("Ignore TimeScale. Useful if the game is paused.")]
		public bool realTime;

		// Token: 0x0400495A RID: 18778
		private float startTime;

		// Token: 0x0400495B RID: 18779
		private float currentTime;

		// Token: 0x0400495C RID: 18780
		private float endTime;

		// Token: 0x0400495D RID: 18781
		private bool looping;
	}
}
