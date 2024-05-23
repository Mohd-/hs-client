using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B47 RID: 2887
	[Tooltip("Animate base action - DON'T USE IT!")]
	public abstract class CurveFsmAction : FsmStateAction
	{
		// Token: 0x06006267 RID: 25191 RVA: 0x001D5344 File Offset: 0x001D3544
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
			this.distances = new float[0];
			this.endTimes = new float[0];
			this.keyOffsets = new float[0];
			this.curves = new AnimationCurve[0];
			this.finishAction = false;
			this.start = false;
		}

		// Token: 0x06006268 RID: 25192 RVA: 0x001D5414 File Offset: 0x001D3614
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

		// Token: 0x06006269 RID: 25193 RVA: 0x001D54A4 File Offset: 0x001D36A4
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
			this.distances = new float[this.fromFloats.Length];
			for (int k = 0; k < this.fromFloats.Length; k++)
			{
				this.distances[k] = this.toFloats[k] - this.fromFloats[k];
			}
		}

		// Token: 0x0600626A RID: 25194 RVA: 0x001D5794 File Offset: 0x001D3994
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
					this.startTime = FsmTime.RealtimeSinceStartup;
					this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
				}
			}
			if (this.isRunning && !this.finishAction)
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
						if (this.calculations[i] != CurveFsmAction.Calculation.None)
						{
							switch (this.calculations[i])
							{
							case CurveFsmAction.Calculation.AddToValue:
								if (!this.time.IsNone)
								{
									this.resultFloats[i] = this.fromFloats[i] + (this.distances[i] * (this.currentTime / this.time.Value) + this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time));
								}
								else
								{
									this.resultFloats[i] = this.fromFloats[i] + (this.distances[i] * (this.currentTime / this.endTimes[i]) + this.curves[i].Evaluate(this.currentTime));
								}
								break;
							case CurveFsmAction.Calculation.SubtractFromValue:
								if (!this.time.IsNone)
								{
									this.resultFloats[i] = this.fromFloats[i] + (this.distances[i] * (this.currentTime / this.time.Value) - this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time));
								}
								else
								{
									this.resultFloats[i] = this.fromFloats[i] + (this.distances[i] * (this.currentTime / this.endTimes[i]) - this.curves[i].Evaluate(this.currentTime));
								}
								break;
							case CurveFsmAction.Calculation.SubtractValueFromCurve:
								if (!this.time.IsNone)
								{
									this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time) - this.distances[i] * (this.currentTime / this.time.Value) + this.fromFloats[i];
								}
								else
								{
									this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime) - this.distances[i] * (this.currentTime / this.endTimes[i]) + this.fromFloats[i];
								}
								break;
							case CurveFsmAction.Calculation.MultiplyValue:
								if (!this.time.IsNone)
								{
									this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time) * this.distances[i] * (this.currentTime / this.time.Value) + this.fromFloats[i];
								}
								else
								{
									this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime) * this.distances[i] * (this.currentTime / this.endTimes[i]) + this.fromFloats[i];
								}
								break;
							case CurveFsmAction.Calculation.DivideValue:
								if (!this.time.IsNone)
								{
									this.resultFloats[i] = ((this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time) == 0f) ? float.MaxValue : (this.fromFloats[i] + this.distances[i] * (this.currentTime / this.time.Value) / this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time)));
								}
								else
								{
									this.resultFloats[i] = ((this.curves[i].Evaluate(this.currentTime) == 0f) ? float.MaxValue : (this.fromFloats[i] + this.distances[i] * (this.currentTime / this.endTimes[i]) / this.curves[i].Evaluate(this.currentTime)));
								}
								break;
							case CurveFsmAction.Calculation.DivideCurveByValue:
								if (!this.time.IsNone)
								{
									this.resultFloats[i] = ((this.fromFloats[i] == 0f) ? float.MaxValue : (this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time) / (this.distances[i] * (this.currentTime / this.time.Value)) + this.fromFloats[i]));
								}
								else
								{
									this.resultFloats[i] = ((this.fromFloats[i] == 0f) ? float.MaxValue : (this.curves[i].Evaluate(this.currentTime) / (this.distances[i] * (this.currentTime / this.endTimes[i])) + this.fromFloats[i]));
								}
								break;
							}
						}
						else if (!this.time.IsNone)
						{
							this.resultFloats[i] = this.fromFloats[i] + this.distances[i] * (this.currentTime / this.time.Value);
						}
						else
						{
							this.resultFloats[i] = this.fromFloats[i] + this.distances[i] * (this.currentTime / this.endTimes[i]);
						}
					}
					else if (!this.time.IsNone)
					{
						this.resultFloats[i] = this.fromFloats[i] + this.distances[i] * (this.currentTime / this.time.Value);
					}
					else if (this.largestEndTime == 0f)
					{
						this.resultFloats[i] = this.fromFloats[i] + this.distances[i] * (this.currentTime / 1f);
					}
					else
					{
						this.resultFloats[i] = this.fromFloats[i] + this.distances[i] * (this.currentTime / this.largestEndTime);
					}
				}
				if (this.isRunning)
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

		// Token: 0x040049AE RID: 18862
		[Tooltip("Define time to use your curve scaled to be stretched or shrinked.")]
		public FsmFloat time;

		// Token: 0x040049AF RID: 18863
		[Tooltip("If you define speed, your animation will be speeded up or slowed down.")]
		public FsmFloat speed;

		// Token: 0x040049B0 RID: 18864
		[Tooltip("Delayed animimation start.")]
		public FsmFloat delay;

		// Token: 0x040049B1 RID: 18865
		[Tooltip("Animation curve start from any time. If IgnoreCurveOffset is true the animation starts right after the state become entered.")]
		public FsmBool ignoreCurveOffset;

		// Token: 0x040049B2 RID: 18866
		[Tooltip("Optionally send an Event when the animation finishes.")]
		public FsmEvent finishEvent;

		// Token: 0x040049B3 RID: 18867
		[Tooltip("Ignore TimeScale. Useful if the game is paused.")]
		public bool realTime;

		// Token: 0x040049B4 RID: 18868
		private float startTime;

		// Token: 0x040049B5 RID: 18869
		private float currentTime;

		// Token: 0x040049B6 RID: 18870
		private float[] endTimes;

		// Token: 0x040049B7 RID: 18871
		private float lastTime;

		// Token: 0x040049B8 RID: 18872
		private float deltaTime;

		// Token: 0x040049B9 RID: 18873
		private float delayTime;

		// Token: 0x040049BA RID: 18874
		private float[] keyOffsets;

		// Token: 0x040049BB RID: 18875
		protected AnimationCurve[] curves;

		// Token: 0x040049BC RID: 18876
		protected CurveFsmAction.Calculation[] calculations;

		// Token: 0x040049BD RID: 18877
		protected float[] resultFloats;

		// Token: 0x040049BE RID: 18878
		protected float[] fromFloats;

		// Token: 0x040049BF RID: 18879
		protected float[] toFloats;

		// Token: 0x040049C0 RID: 18880
		private float[] distances;

		// Token: 0x040049C1 RID: 18881
		protected bool finishAction;

		// Token: 0x040049C2 RID: 18882
		protected bool isRunning;

		// Token: 0x040049C3 RID: 18883
		protected bool looping;

		// Token: 0x040049C4 RID: 18884
		private bool start;

		// Token: 0x040049C5 RID: 18885
		private float largestEndTime;

		// Token: 0x02000B48 RID: 2888
		public enum Calculation
		{
			// Token: 0x040049C7 RID: 18887
			None,
			// Token: 0x040049C8 RID: 18888
			AddToValue,
			// Token: 0x040049C9 RID: 18889
			SubtractFromValue,
			// Token: 0x040049CA RID: 18890
			SubtractValueFromCurve,
			// Token: 0x040049CB RID: 18891
			MultiplyValue,
			// Token: 0x040049CC RID: 18892
			DivideValue,
			// Token: 0x040049CD RID: 18893
			DivideCurveByValue
		}
	}
}
