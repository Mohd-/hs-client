using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B4D RID: 2893
	[Tooltip("Ease base action - don't use!")]
	public abstract class EaseFsmAction : FsmStateAction
	{
		// Token: 0x06006280 RID: 25216 RVA: 0x001D6DC0 File Offset: 0x001D4FC0
		public override void Reset()
		{
			this.easeType = EaseFsmAction.EaseType.linear;
			this.time = new FsmFloat
			{
				Value = 1f
			};
			this.delay = new FsmFloat
			{
				UseVariable = true
			};
			this.speed = new FsmFloat
			{
				UseVariable = true
			};
			this.reverse = new FsmBool
			{
				Value = false
			};
			this.realTime = false;
			this.finishEvent = null;
			this.ease = null;
			this.runningTime = 0f;
			this.lastTime = 0f;
			this.percentage = 0f;
			this.fromFloats = new float[0];
			this.toFloats = new float[0];
			this.resultFloats = new float[0];
			this.finishAction = false;
			this.start = false;
			this.finished = false;
			this.isRunning = false;
		}

		// Token: 0x06006281 RID: 25217 RVA: 0x001D6EA0 File Offset: 0x001D50A0
		public override void OnEnter()
		{
			this.finished = false;
			this.isRunning = false;
			this.SetEasingFunction();
			this.runningTime = 0f;
			this.percentage = ((!this.reverse.IsNone) ? ((!this.reverse.Value) ? 0f : 1f) : 0f);
			this.finishAction = false;
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
			this.delayTime = ((!this.delay.IsNone) ? (this.delayTime = this.delay.Value) : 0f);
			this.start = true;
		}

		// Token: 0x06006282 RID: 25218 RVA: 0x001D6F6A File Offset: 0x001D516A
		public override void OnExit()
		{
		}

		// Token: 0x06006283 RID: 25219 RVA: 0x001D6F6C File Offset: 0x001D516C
		public override void OnUpdate()
		{
			if (this.start && !this.isRunning)
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
			if (this.isRunning && !this.finished)
			{
				if (this.reverse.IsNone || !this.reverse.Value)
				{
					this.UpdatePercentage();
					if (this.percentage < 1f)
					{
						for (int i = 0; i < this.fromFloats.Length; i++)
						{
							this.resultFloats[i] = this.ease(this.fromFloats[i], this.toFloats[i], this.percentage);
						}
					}
					else
					{
						this.finishAction = true;
						this.finished = true;
						this.isRunning = false;
					}
				}
				else
				{
					this.UpdatePercentage();
					if (this.percentage > 0f)
					{
						for (int j = 0; j < this.fromFloats.Length; j++)
						{
							this.resultFloats[j] = this.ease(this.fromFloats[j], this.toFloats[j], this.percentage);
						}
					}
					else
					{
						this.finishAction = true;
						this.finished = true;
						this.isRunning = false;
					}
				}
			}
		}

		// Token: 0x06006284 RID: 25220 RVA: 0x001D7154 File Offset: 0x001D5354
		protected void UpdatePercentage()
		{
			if (this.realTime)
			{
				this.deltaTime = FsmTime.RealtimeSinceStartup - this.startTime - this.lastTime;
				this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
				if (!this.speed.IsNone)
				{
					this.runningTime += this.deltaTime * this.speed.Value;
				}
				else
				{
					this.runningTime += this.deltaTime;
				}
			}
			else if (!this.speed.IsNone)
			{
				this.runningTime += Time.deltaTime * this.speed.Value;
			}
			else
			{
				this.runningTime += Time.deltaTime;
			}
			if (!this.reverse.IsNone && this.reverse.Value)
			{
				this.percentage = 1f - this.runningTime / this.time.Value;
			}
			else
			{
				this.percentage = this.runningTime / this.time.Value;
			}
		}

		// Token: 0x06006285 RID: 25221 RVA: 0x001D728C File Offset: 0x001D548C
		protected void SetEasingFunction()
		{
			switch (this.easeType)
			{
			case EaseFsmAction.EaseType.easeInQuad:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInQuad);
				break;
			case EaseFsmAction.EaseType.easeOutQuad:
				this.ease = new EaseFsmAction.EasingFunction(this.easeOutQuad);
				break;
			case EaseFsmAction.EaseType.easeInOutQuad:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInOutQuad);
				break;
			case EaseFsmAction.EaseType.easeInCubic:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInCubic);
				break;
			case EaseFsmAction.EaseType.easeOutCubic:
				this.ease = new EaseFsmAction.EasingFunction(this.easeOutCubic);
				break;
			case EaseFsmAction.EaseType.easeInOutCubic:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInOutCubic);
				break;
			case EaseFsmAction.EaseType.easeInQuart:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInQuart);
				break;
			case EaseFsmAction.EaseType.easeOutQuart:
				this.ease = new EaseFsmAction.EasingFunction(this.easeOutQuart);
				break;
			case EaseFsmAction.EaseType.easeInOutQuart:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInOutQuart);
				break;
			case EaseFsmAction.EaseType.easeInQuint:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInQuint);
				break;
			case EaseFsmAction.EaseType.easeOutQuint:
				this.ease = new EaseFsmAction.EasingFunction(this.easeOutQuint);
				break;
			case EaseFsmAction.EaseType.easeInOutQuint:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInOutQuint);
				break;
			case EaseFsmAction.EaseType.easeInSine:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInSine);
				break;
			case EaseFsmAction.EaseType.easeOutSine:
				this.ease = new EaseFsmAction.EasingFunction(this.easeOutSine);
				break;
			case EaseFsmAction.EaseType.easeInOutSine:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInOutSine);
				break;
			case EaseFsmAction.EaseType.easeInExpo:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInExpo);
				break;
			case EaseFsmAction.EaseType.easeOutExpo:
				this.ease = new EaseFsmAction.EasingFunction(this.easeOutExpo);
				break;
			case EaseFsmAction.EaseType.easeInOutExpo:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInOutExpo);
				break;
			case EaseFsmAction.EaseType.easeInCirc:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInCirc);
				break;
			case EaseFsmAction.EaseType.easeOutCirc:
				this.ease = new EaseFsmAction.EasingFunction(this.easeOutCirc);
				break;
			case EaseFsmAction.EaseType.easeInOutCirc:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInOutCirc);
				break;
			case EaseFsmAction.EaseType.linear:
				this.ease = new EaseFsmAction.EasingFunction(this.linear);
				break;
			case EaseFsmAction.EaseType.spring:
				this.ease = new EaseFsmAction.EasingFunction(this.spring);
				break;
			case EaseFsmAction.EaseType.bounce:
				this.ease = new EaseFsmAction.EasingFunction(this.bounce);
				break;
			case EaseFsmAction.EaseType.easeInBack:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInBack);
				break;
			case EaseFsmAction.EaseType.easeOutBack:
				this.ease = new EaseFsmAction.EasingFunction(this.easeOutBack);
				break;
			case EaseFsmAction.EaseType.easeInOutBack:
				this.ease = new EaseFsmAction.EasingFunction(this.easeInOutBack);
				break;
			case EaseFsmAction.EaseType.elastic:
				this.ease = new EaseFsmAction.EasingFunction(this.elastic);
				break;
			}
		}

		// Token: 0x06006286 RID: 25222 RVA: 0x001D759F File Offset: 0x001D579F
		protected float linear(float start, float end, float value)
		{
			return Mathf.Lerp(start, end, value);
		}

		// Token: 0x06006287 RID: 25223 RVA: 0x001D75AC File Offset: 0x001D57AC
		protected float clerp(float start, float end, float value)
		{
			float num = 0f;
			float num2 = 360f;
			float num3 = Mathf.Abs((num2 - num) / 2f);
			float result;
			if (end - start < -num3)
			{
				float num4 = (num2 - start + end) * value;
				result = start + num4;
			}
			else if (end - start > num3)
			{
				float num4 = -(num2 - end + start) * value;
				result = start + num4;
			}
			else
			{
				result = start + (end - start) * value;
			}
			return result;
		}

		// Token: 0x06006288 RID: 25224 RVA: 0x001D7624 File Offset: 0x001D5824
		protected float spring(float start, float end, float value)
		{
			value = Mathf.Clamp01(value);
			value = (Mathf.Sin(value * 3.1415927f * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + 1.2f * (1f - value));
			return start + (end - start) * value;
		}

		// Token: 0x06006289 RID: 25225 RVA: 0x001D7688 File Offset: 0x001D5888
		protected float easeInQuad(float start, float end, float value)
		{
			end -= start;
			return end * value * value + start;
		}

		// Token: 0x0600628A RID: 25226 RVA: 0x001D7696 File Offset: 0x001D5896
		protected float easeOutQuad(float start, float end, float value)
		{
			end -= start;
			return -end * value * (value - 2f) + start;
		}

		// Token: 0x0600628B RID: 25227 RVA: 0x001D76AC File Offset: 0x001D58AC
		protected float easeInOutQuad(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end / 2f * value * value + start;
			}
			value -= 1f;
			return -end / 2f * (value * (value - 2f) - 1f) + start;
		}

		// Token: 0x0600628C RID: 25228 RVA: 0x001D7703 File Offset: 0x001D5903
		protected float easeInCubic(float start, float end, float value)
		{
			end -= start;
			return end * value * value * value + start;
		}

		// Token: 0x0600628D RID: 25229 RVA: 0x001D7713 File Offset: 0x001D5913
		protected float easeOutCubic(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return end * (value * value * value + 1f) + start;
		}

		// Token: 0x0600628E RID: 25230 RVA: 0x001D7734 File Offset: 0x001D5934
		protected float easeInOutCubic(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end / 2f * value * value * value + start;
			}
			value -= 2f;
			return end / 2f * (value * value * value + 2f) + start;
		}

		// Token: 0x0600628F RID: 25231 RVA: 0x001D7788 File Offset: 0x001D5988
		protected float easeInQuart(float start, float end, float value)
		{
			end -= start;
			return end * value * value * value * value + start;
		}

		// Token: 0x06006290 RID: 25232 RVA: 0x001D779C File Offset: 0x001D599C
		protected float easeOutQuart(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return -end * (value * value * value * value - 1f) + start;
		}

		// Token: 0x06006291 RID: 25233 RVA: 0x001D77CC File Offset: 0x001D59CC
		protected float easeInOutQuart(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end / 2f * value * value * value * value + start;
			}
			value -= 2f;
			return -end / 2f * (value * value * value * value - 2f) + start;
		}

		// Token: 0x06006292 RID: 25234 RVA: 0x001D7825 File Offset: 0x001D5A25
		protected float easeInQuint(float start, float end, float value)
		{
			end -= start;
			return end * value * value * value * value * value + start;
		}

		// Token: 0x06006293 RID: 25235 RVA: 0x001D7839 File Offset: 0x001D5A39
		protected float easeOutQuint(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return end * (value * value * value * value * value + 1f) + start;
		}

		// Token: 0x06006294 RID: 25236 RVA: 0x001D785C File Offset: 0x001D5A5C
		protected float easeInOutQuint(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end / 2f * value * value * value * value * value + start;
			}
			value -= 2f;
			return end / 2f * (value * value * value * value * value + 2f) + start;
		}

		// Token: 0x06006295 RID: 25237 RVA: 0x001D78B8 File Offset: 0x001D5AB8
		protected float easeInSine(float start, float end, float value)
		{
			end -= start;
			return -end * Mathf.Cos(value / 1f * 1.5707964f) + end + start;
		}

		// Token: 0x06006296 RID: 25238 RVA: 0x001D78D8 File Offset: 0x001D5AD8
		protected float easeOutSine(float start, float end, float value)
		{
			end -= start;
			return end * Mathf.Sin(value / 1f * 1.5707964f) + start;
		}

		// Token: 0x06006297 RID: 25239 RVA: 0x001D78F8 File Offset: 0x001D5AF8
		protected float easeInOutSine(float start, float end, float value)
		{
			end -= start;
			return -end / 2f * (Mathf.Cos(3.1415927f * value / 1f) - 1f) + start;
		}

		// Token: 0x06006298 RID: 25240 RVA: 0x001D7930 File Offset: 0x001D5B30
		protected float easeInExpo(float start, float end, float value)
		{
			end -= start;
			return end * Mathf.Pow(2f, 10f * (value / 1f - 1f)) + start;
		}

		// Token: 0x06006299 RID: 25241 RVA: 0x001D7963 File Offset: 0x001D5B63
		protected float easeOutExpo(float start, float end, float value)
		{
			end -= start;
			return end * (-Mathf.Pow(2f, -10f * value / 1f) + 1f) + start;
		}

		// Token: 0x0600629A RID: 25242 RVA: 0x001D798C File Offset: 0x001D5B8C
		protected float easeInOutExpo(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end / 2f * Mathf.Pow(2f, 10f * (value - 1f)) + start;
			}
			value -= 1f;
			return end / 2f * (-Mathf.Pow(2f, -10f * value) + 2f) + start;
		}

		// Token: 0x0600629B RID: 25243 RVA: 0x001D79FF File Offset: 0x001D5BFF
		protected float easeInCirc(float start, float end, float value)
		{
			end -= start;
			return -end * (Mathf.Sqrt(1f - value * value) - 1f) + start;
		}

		// Token: 0x0600629C RID: 25244 RVA: 0x001D7A20 File Offset: 0x001D5C20
		protected float easeOutCirc(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return end * Mathf.Sqrt(1f - value * value) + start;
		}

		// Token: 0x0600629D RID: 25245 RVA: 0x001D7A50 File Offset: 0x001D5C50
		protected float easeInOutCirc(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return -end / 2f * (Mathf.Sqrt(1f - value * value) - 1f) + start;
			}
			value -= 2f;
			return end / 2f * (Mathf.Sqrt(1f - value * value) + 1f) + start;
		}

		// Token: 0x0600629E RID: 25246 RVA: 0x001D7AC0 File Offset: 0x001D5CC0
		protected float bounce(float start, float end, float value)
		{
			value /= 1f;
			end -= start;
			if (value < 0.36363637f)
			{
				return end * (7.5625f * value * value) + start;
			}
			if (value < 0.72727275f)
			{
				value -= 0.54545456f;
				return end * (7.5625f * value * value + 0.75f) + start;
			}
			if ((double)value < 0.9090909090909091)
			{
				value -= 0.8181818f;
				return end * (7.5625f * value * value + 0.9375f) + start;
			}
			value -= 0.95454544f;
			return end * (7.5625f * value * value + 0.984375f) + start;
		}

		// Token: 0x0600629F RID: 25247 RVA: 0x001D7B68 File Offset: 0x001D5D68
		protected float easeInBack(float start, float end, float value)
		{
			end -= start;
			value /= 1f;
			float num = 1.70158f;
			return end * value * value * ((num + 1f) * value - num) + start;
		}

		// Token: 0x060062A0 RID: 25248 RVA: 0x001D7B9C File Offset: 0x001D5D9C
		protected float easeOutBack(float start, float end, float value)
		{
			float num = 1.70158f;
			end -= start;
			value = value / 1f - 1f;
			return end * (value * value * ((num + 1f) * value + num) + 1f) + start;
		}

		// Token: 0x060062A1 RID: 25249 RVA: 0x001D7BDC File Offset: 0x001D5DDC
		protected float easeInOutBack(float start, float end, float value)
		{
			float num = 1.70158f;
			end -= start;
			value /= 0.5f;
			if (value < 1f)
			{
				num *= 1.525f;
				return end / 2f * (value * value * ((num + 1f) * value - num)) + start;
			}
			value -= 2f;
			num *= 1.525f;
			return end / 2f * (value * value * ((num + 1f) * value + num) + 2f) + start;
		}

		// Token: 0x060062A2 RID: 25250 RVA: 0x001D7C5C File Offset: 0x001D5E5C
		protected float punch(float amplitude, float value)
		{
			if (value == 0f)
			{
				return 0f;
			}
			if (value == 1f)
			{
				return 0f;
			}
			float num = 0.3f;
			float num2 = num / 6.2831855f * Mathf.Asin(0f);
			return amplitude * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * 1f - num2) * 6.2831855f / num);
		}

		// Token: 0x060062A3 RID: 25251 RVA: 0x001D7CD4 File Offset: 0x001D5ED4
		protected float elastic(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			float num2 = num * 0.3f;
			float num3 = 0f;
			if (value == 0f)
			{
				return start;
			}
			if ((value /= num) == 1f)
			{
				return start + end;
			}
			float num4;
			if (num3 == 0f || num3 < Mathf.Abs(end))
			{
				num3 = end;
				num4 = num2 / 4f;
			}
			else
			{
				num4 = num2 / 6.2831855f * Mathf.Asin(end / num3);
			}
			return num3 * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * num - num4) * 6.2831855f / num2) + end + start;
		}

		// Token: 0x040049F0 RID: 18928
		[RequiredField]
		public FsmFloat time;

		// Token: 0x040049F1 RID: 18929
		public FsmFloat speed;

		// Token: 0x040049F2 RID: 18930
		public FsmFloat delay;

		// Token: 0x040049F3 RID: 18931
		public EaseFsmAction.EaseType easeType = EaseFsmAction.EaseType.linear;

		// Token: 0x040049F4 RID: 18932
		public FsmBool reverse;

		// Token: 0x040049F5 RID: 18933
		[Tooltip("Optionally send an Event when the animation finishes.")]
		public FsmEvent finishEvent;

		// Token: 0x040049F6 RID: 18934
		[Tooltip("Ignore TimeScale. Useful if the game is paused.")]
		public bool realTime;

		// Token: 0x040049F7 RID: 18935
		protected EaseFsmAction.EasingFunction ease;

		// Token: 0x040049F8 RID: 18936
		protected float runningTime;

		// Token: 0x040049F9 RID: 18937
		protected float lastTime;

		// Token: 0x040049FA RID: 18938
		protected float startTime;

		// Token: 0x040049FB RID: 18939
		protected float deltaTime;

		// Token: 0x040049FC RID: 18940
		protected float delayTime;

		// Token: 0x040049FD RID: 18941
		protected float percentage;

		// Token: 0x040049FE RID: 18942
		protected float[] fromFloats = new float[0];

		// Token: 0x040049FF RID: 18943
		protected float[] toFloats = new float[0];

		// Token: 0x04004A00 RID: 18944
		protected float[] resultFloats = new float[0];

		// Token: 0x04004A01 RID: 18945
		protected bool finishAction;

		// Token: 0x04004A02 RID: 18946
		protected bool start;

		// Token: 0x04004A03 RID: 18947
		protected bool finished;

		// Token: 0x04004A04 RID: 18948
		protected bool isRunning;

		// Token: 0x02000B4E RID: 2894
		public enum EaseType
		{
			// Token: 0x04004A06 RID: 18950
			easeInQuad,
			// Token: 0x04004A07 RID: 18951
			easeOutQuad,
			// Token: 0x04004A08 RID: 18952
			easeInOutQuad,
			// Token: 0x04004A09 RID: 18953
			easeInCubic,
			// Token: 0x04004A0A RID: 18954
			easeOutCubic,
			// Token: 0x04004A0B RID: 18955
			easeInOutCubic,
			// Token: 0x04004A0C RID: 18956
			easeInQuart,
			// Token: 0x04004A0D RID: 18957
			easeOutQuart,
			// Token: 0x04004A0E RID: 18958
			easeInOutQuart,
			// Token: 0x04004A0F RID: 18959
			easeInQuint,
			// Token: 0x04004A10 RID: 18960
			easeOutQuint,
			// Token: 0x04004A11 RID: 18961
			easeInOutQuint,
			// Token: 0x04004A12 RID: 18962
			easeInSine,
			// Token: 0x04004A13 RID: 18963
			easeOutSine,
			// Token: 0x04004A14 RID: 18964
			easeInOutSine,
			// Token: 0x04004A15 RID: 18965
			easeInExpo,
			// Token: 0x04004A16 RID: 18966
			easeOutExpo,
			// Token: 0x04004A17 RID: 18967
			easeInOutExpo,
			// Token: 0x04004A18 RID: 18968
			easeInCirc,
			// Token: 0x04004A19 RID: 18969
			easeOutCirc,
			// Token: 0x04004A1A RID: 18970
			easeInOutCirc,
			// Token: 0x04004A1B RID: 18971
			linear,
			// Token: 0x04004A1C RID: 18972
			spring,
			// Token: 0x04004A1D RID: 18973
			bounce,
			// Token: 0x04004A1E RID: 18974
			easeInBack,
			// Token: 0x04004A1F RID: 18975
			easeOutBack,
			// Token: 0x04004A20 RID: 18976
			easeInOutBack,
			// Token: 0x04004A21 RID: 18977
			elastic,
			// Token: 0x04004A22 RID: 18978
			punch
		}

		// Token: 0x02000B4F RID: 2895
		// (Invoke) Token: 0x060062A5 RID: 25253
		protected delegate float EasingFunction(float start, float end, float value);
	}
}
