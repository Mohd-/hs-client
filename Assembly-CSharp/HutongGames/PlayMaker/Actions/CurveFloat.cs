using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B49 RID: 2889
	[Tooltip("Animates the value of a Float Variable FROM-TO with assistance of Deformation Curve.")]
	[ActionCategory(37)]
	public class CurveFloat : CurveFsmAction
	{
		// Token: 0x0600626C RID: 25196 RVA: 0x001D6094 File Offset: 0x001D4294
		public override void Reset()
		{
			base.Reset();
			this.floatVariable = new FsmFloat
			{
				UseVariable = true
			};
			this.toValue = new FsmFloat
			{
				UseVariable = true
			};
			this.fromValue = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x0600626D RID: 25197 RVA: 0x001D60E4 File Offset: 0x001D42E4
		public override void OnEnter()
		{
			base.OnEnter();
			this.finishInNextStep = false;
			this.resultFloats = new float[1];
			this.fromFloats = new float[1];
			this.fromFloats[0] = ((!this.fromValue.IsNone) ? this.fromValue.Value : 0f);
			this.toFloats = new float[1];
			this.toFloats[0] = ((!this.toValue.IsNone) ? this.toValue.Value : 0f);
			this.calculations = new CurveFsmAction.Calculation[1];
			this.calculations[0] = this.calculation;
			this.curves = new AnimationCurve[1];
			this.curves[0] = this.animCurve.curve;
			base.Init();
		}

		// Token: 0x0600626E RID: 25198 RVA: 0x001D61BB File Offset: 0x001D43BB
		public override void OnExit()
		{
		}

		// Token: 0x0600626F RID: 25199 RVA: 0x001D61C0 File Offset: 0x001D43C0
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.floatVariable.IsNone && this.isRunning)
			{
				this.floatVariable.Value = this.resultFloats[0];
			}
			if (this.finishInNextStep && !this.looping)
			{
				base.Finish();
				if (this.finishEvent != null)
				{
					base.Fsm.Event(this.finishEvent);
				}
			}
			if (this.finishAction && !this.finishInNextStep)
			{
				if (!this.floatVariable.IsNone)
				{
					this.floatVariable.Value = this.resultFloats[0];
				}
				this.finishInNextStep = true;
			}
		}

		// Token: 0x040049CE RID: 18894
		[RequiredField]
		[UIHint(10)]
		public FsmFloat floatVariable;

		// Token: 0x040049CF RID: 18895
		[RequiredField]
		public FsmFloat fromValue;

		// Token: 0x040049D0 RID: 18896
		[RequiredField]
		public FsmFloat toValue;

		// Token: 0x040049D1 RID: 18897
		[RequiredField]
		public FsmAnimationCurve animCurve;

		// Token: 0x040049D2 RID: 18898
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue and toValue.")]
		public CurveFsmAction.Calculation calculation;

		// Token: 0x040049D3 RID: 18899
		private bool finishInNextStep;
	}
}
