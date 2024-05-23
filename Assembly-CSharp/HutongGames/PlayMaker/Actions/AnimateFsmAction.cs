using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B41 RID: 2881
	[Tooltip("Animate base action - DON'T USE IT!")]
	public abstract class AnimateFsmAction : FsmStateAction
	{
		// Token: 0x06006250 RID: 25168 RVA: 0x001D3D20 File Offset: 0x001D1F20
		public override void Reset()
		{
			this.finishEvent = null;
			this.realTime = false;
			this.time = new FsmFloat
			{
				UseVariable = true
			};
			this.speed = new FsmFloat
			{
				UseVariable = true
			};
			this.delay = new FsmFloat
			{
				UseVariable = true
			};
			this.ignoreCurveOffset = new FsmBool
			{
				Value = true
			};
			this.resultFloats = new float[0];
			this.fromFloats = new float[0];
			this.toFloats = new float[0];
			this.endTimes = new float[0];
			this.keyOffsets = new float[0];
			this.curves = new AnimationCurve[0];
			this.finishAction = false;
			this.start = false;
		}

		// Token: 0x06006251 RID: 25169 RVA: 0x001D3DE4 File Offset: 0x001D1FE4
		public override void OnEnter()
		{
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
			this.deltaTime = 0f;
			this.currentTime = 0f;
			this.isRunning = false;
			this.finishAction = false;
			this.looping = false;
			this.delayTime = ((!this.delay.IsNone) ? (this.delayTime = this.delay.Value) : 0f);
			this.start = true;
		}

		// Token: 0x06006252 RID: 25170 RVA: 0x001D3E74 File Offset: 0x001D2074
		protected void Init()
		{
			this.endTimes = new float[this.curves.Length];
			this.keyOffsets = new float[this.curves.Length];
			this.largestEndTime = 0f;
			for (int i = 0; i < this.curves.Length; i++)
			{
				if (this.curves[i] != null && this.curves[i].keys.Length > 0)
				{
					this.keyOffsets[i] = ((this.curves[i].keys.Length <= 0) ? 0f : ((!this.time.IsNone) ? (this.time.Value / this.curves[i].keys[this.curves[i].length - 1].time * this.curves[i].keys[0].time) : this.curves[i].keys[0].time));
					this.currentTime = ((!this.ignoreCurveOffset.IsNone) ? ((!this.ignoreCurveOffset.Value) ? 0f : this.keyOffsets[i]) : 0f);
					if (!this.time.IsNone)
					{
						this.endTimes[i] = this.time.Value;
					}
					else
					{
						this.endTimes[i] = this.curves[i].keys[this.curves[i].length - 1].time;
					}
					if (this.largestEndTime < this.endTimes[i])
					{
						this.largestEndTime = this.endTimes[i];
					}
					if (!this.looping)
					{
						this.looping = ActionHelpers.IsLoopingWrapMode(this.curves[i].postWrapMode);
					}
				}
				else
				{
					this.endTimes[i] = -1f;
				}
			}
			for (int j = 0; j < this.curves.Length; j++)
			{
				if (this.largestEndTime > 0f && this.endTimes[j] == -1f)
				{
					this.endTimes[j] = this.largestEndTime;
				}
				else if (this.largestEndTime == 0f && this.endTimes[j] == -1f)
				{
					if (this.time.IsNone)
					{
						this.endTimes[j] = 1f;
					}
					else
					{
						this.endTimes[j] = this.time.Value;
					}
				}
			}
		}

		// Token: 0x06006253 RID: 25171 RVA: 0x001D4120 File Offset: 0x001D2320
		public override void OnUpdate()
		{
			if (!this.isRunning && this.start)
			{
				if (this.delayTime >= 0f)
				{
					if (this.realTime)
					{
						this.deltaTime = FsmTime.RealtimeSinceStartup - this.startTime - this.lastTime;
						this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
						this.delayTime -= this.deltaTime;
					}
					else
					{
						this.delayTime -= Time.deltaTime;
					}
				}
				else
				{
					this.isRunning = true;
					this.start = false;
				}
			}
			if (this.isRunning)
			{
				if (this.realTime)
				{
					this.deltaTime = FsmTime.RealtimeSinceStartup - this.startTime - this.lastTime;
					this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
					if (!this.speed.IsNone)
					{
						this.currentTime += this.deltaTime * this.speed.Value;
					}
					else
					{
						this.currentTime += this.deltaTime;
					}
				}
				else if (!this.speed.IsNone)
				{
					this.currentTime += Time.deltaTime * this.speed.Value;
				}
				else
				{
					this.currentTime += Time.deltaTime;
				}
				for (int i = 0; i < this.curves.Length; i++)
				{
					if (this.curves[i] != null && this.curves[i].keys.Length > 0)
					{
						if (this.calculations[i] != AnimateFsmAction.Calculation.None)
						{
							switch (this.calculations[i])
							{
							case AnimateFsmAction.Calculation.SetValue:
								if (!this.time.IsNone)
								{
									this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time);
								}
								else
								{
									this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime);
								}
								break;
							case AnimateFsmAction.Calculation.AddToValue:
								if (!this.time.IsNone)
								{
									this.resultFloats[i] = this.fromFloats[i] + this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time);
								}
								else
								{
									this.resultFloats[i] = this.fromFloats[i] + this.curves[i].Evaluate(this.currentTime);
								}
								break;
							case AnimateFsmAction.Calculation.SubtractFromValue:
								if (!this.time.IsNone)
								{
									this.resultFloats[i] = this.fromFloats[i] - this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time);
								}
								else
								{
									this.resultFloats[i] = this.fromFloats[i] - this.curves[i].Evaluate(this.currentTime);
								}
								break;
							case AnimateFsmAction.Calculation.SubtractValueFromCurve:
								if (!this.time.IsNone)
								{
									this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time) - this.fromFloats[i];
								}
								else
								{
									this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime) - this.fromFloats[i];
								}
								break;
							case AnimateFsmAction.Calculation.MultiplyValue:
								if (!this.time.IsNone)
								{
									this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time) * this.fromFloats[i];
								}
								else
								{
									this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime) * this.fromFloats[i];
								}
								break;
							case AnimateFsmAction.Calculation.DivideValue:
								if (!this.time.IsNone)
								{
									this.resultFloats[i] = ((this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time) == 0f) ? float.MaxValue : (this.fromFloats[i] / this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time)));
								}
								else
								{
									this.resultFloats[i] = ((this.curves[i].Evaluate(this.currentTime) == 0f) ? float.MaxValue : (this.fromFloats[i] / this.curves[i].Evaluate(this.currentTime)));
								}
								break;
							case AnimateFsmAction.Calculation.DivideCurveByValue:
								if (!this.time.IsNone)
								{
									this.resultFloats[i] = ((this.fromFloats[i] == 0f) ? float.MaxValue : (this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time) / this.fromFloats[i]));
								}
								else
								{
									this.resultFloats[i] = ((this.fromFloats[i] == 0f) ? float.MaxValue : (this.curves[i].Evaluate(this.currentTime) / this.fromFloats[i]));
								}
								break;
							}
						}
						else
						{
							this.resultFloats[i] = this.fromFloats[i];
						}
					}
					else
					{
						this.resultFloats[i] = this.fromFloats[i];
					}
				}
				if (this.isRunning && !this.looping)
				{
					this.finishAction = true;
					for (int j = 0; j < this.endTimes.Length; j++)
					{
						if (this.currentTime < this.endTimes[j])
						{
							this.finishAction = false;
						}
					}
					this.isRunning = !this.finishAction;
				}
			}
		}

		// Token: 0x04004969 RID: 18793
		[Tooltip("Define time to use your curve scaled to be stretched or shrinked.")]
		public FsmFloat time;

		// Token: 0x0400496A RID: 18794
		[Tooltip("If you define speed, your animation will be speeded up or slowed down.")]
		public FsmFloat speed;

		// Token: 0x0400496B RID: 18795
		[Tooltip("Delayed animimation start.")]
		public FsmFloat delay;

		// Token: 0x0400496C RID: 18796
		[Tooltip("Animation curve start from any time. If IgnoreCurveOffset is true the animation starts right after the state become entered.")]
		public FsmBool ignoreCurveOffset;

		// Token: 0x0400496D RID: 18797
		[Tooltip("Optionally send an Event when the animation finishes.")]
		public FsmEvent finishEvent;

		// Token: 0x0400496E RID: 18798
		[Tooltip("Ignore TimeScale. Useful if the game is paused.")]
		public bool realTime;

		// Token: 0x0400496F RID: 18799
		private float startTime;

		// Token: 0x04004970 RID: 18800
		private float currentTime;

		// Token: 0x04004971 RID: 18801
		private float[] endTimes;

		// Token: 0x04004972 RID: 18802
		private float lastTime;

		// Token: 0x04004973 RID: 18803
		private float deltaTime;

		// Token: 0x04004974 RID: 18804
		private float delayTime;

		// Token: 0x04004975 RID: 18805
		private float[] keyOffsets;

		// Token: 0x04004976 RID: 18806
		protected AnimationCurve[] curves;

		// Token: 0x04004977 RID: 18807
		protected AnimateFsmAction.Calculation[] calculations;

		// Token: 0x04004978 RID: 18808
		protected float[] resultFloats;

		// Token: 0x04004979 RID: 18809
		protected float[] fromFloats;

		// Token: 0x0400497A RID: 18810
		protected float[] toFloats;

		// Token: 0x0400497B RID: 18811
		protected bool finishAction;

		// Token: 0x0400497C RID: 18812
		protected bool isRunning;

		// Token: 0x0400497D RID: 18813
		protected bool looping;

		// Token: 0x0400497E RID: 18814
		private bool start;

		// Token: 0x0400497F RID: 18815
		private float largestEndTime;

		// Token: 0x02000B42 RID: 2882
		public enum Calculation
		{
			// Token: 0x04004981 RID: 18817
			None,
			// Token: 0x04004982 RID: 18818
			SetValue,
			// Token: 0x04004983 RID: 18819
			AddToValue,
			// Token: 0x04004984 RID: 18820
			SubtractFromValue,
			// Token: 0x04004985 RID: 18821
			SubtractValueFromCurve,
			// Token: 0x04004986 RID: 18822
			MultiplyValue,
			// Token: 0x04004987 RID: 18823
			DivideValue,
			// Token: 0x04004988 RID: 18824
			DivideCurveByValue
		}
	}
}
