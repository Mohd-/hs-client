using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B4A RID: 2890
	[ActionCategory("AnimateVariables")]
	[Tooltip("Animates the value of a Rect Variable FROM-TO with assistance of Deformation Curves.")]
	public class CurveRect : CurveFsmAction
	{
		// Token: 0x06006271 RID: 25201 RVA: 0x001D6284 File Offset: 0x001D4484
		public override void Reset()
		{
			base.Reset();
			this.rectVariable = new FsmRect
			{
				UseVariable = true
			};
			this.toValue = new FsmRect
			{
				UseVariable = true
			};
			this.fromValue = new FsmRect
			{
				UseVariable = true
			};
		}

		// Token: 0x06006272 RID: 25202 RVA: 0x001D62D4 File Offset: 0x001D44D4
		public override void OnEnter()
		{
			base.OnEnter();
			this.finishInNextStep = false;
			this.resultFloats = new float[4];
			this.fromFloats = new float[4];
			this.fromFloats[0] = ((!this.fromValue.IsNone) ? this.fromValue.Value.x : 0f);
			this.fromFloats[1] = ((!this.fromValue.IsNone) ? this.fromValue.Value.y : 0f);
			this.fromFloats[2] = ((!this.fromValue.IsNone) ? this.fromValue.Value.width : 0f);
			this.fromFloats[3] = ((!this.fromValue.IsNone) ? this.fromValue.Value.height : 0f);
			this.toFloats = new float[4];
			this.toFloats[0] = ((!this.toValue.IsNone) ? this.toValue.Value.x : 0f);
			this.toFloats[1] = ((!this.toValue.IsNone) ? this.toValue.Value.y : 0f);
			this.toFloats[2] = ((!this.toValue.IsNone) ? this.toValue.Value.width : 0f);
			this.toFloats[3] = ((!this.toValue.IsNone) ? this.toValue.Value.height : 0f);
			this.curves = new AnimationCurve[4];
			this.curves[0] = this.curveX.curve;
			this.curves[1] = this.curveY.curve;
			this.curves[2] = this.curveW.curve;
			this.curves[3] = this.curveH.curve;
			this.calculations = new CurveFsmAction.Calculation[4];
			this.calculations[0] = this.calculationX;
			this.calculations[1] = this.calculationY;
			this.calculations[2] = this.calculationW;
			this.calculations[2] = this.calculationH;
			base.Init();
		}

		// Token: 0x06006273 RID: 25203 RVA: 0x001D6560 File Offset: 0x001D4760
		public override void OnExit()
		{
		}

		// Token: 0x06006274 RID: 25204 RVA: 0x001D6564 File Offset: 0x001D4764
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.rectVariable.IsNone && this.isRunning)
			{
				this.rct = new Rect(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2], this.resultFloats[3]);
				this.rectVariable.Value = this.rct;
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
				if (!this.rectVariable.IsNone)
				{
					this.rct = new Rect(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2], this.resultFloats[3]);
					this.rectVariable.Value = this.rct;
				}
				this.finishInNextStep = true;
			}
		}

		// Token: 0x040049D4 RID: 18900
		[UIHint(10)]
		[RequiredField]
		public FsmRect rectVariable;

		// Token: 0x040049D5 RID: 18901
		[RequiredField]
		public FsmRect fromValue;

		// Token: 0x040049D6 RID: 18902
		[RequiredField]
		public FsmRect toValue;

		// Token: 0x040049D7 RID: 18903
		[RequiredField]
		public FsmAnimationCurve curveX;

		// Token: 0x040049D8 RID: 18904
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.x and toValue.x.")]
		public CurveFsmAction.Calculation calculationX;

		// Token: 0x040049D9 RID: 18905
		[RequiredField]
		public FsmAnimationCurve curveY;

		// Token: 0x040049DA RID: 18906
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.y and toValue.y.")]
		public CurveFsmAction.Calculation calculationY;

		// Token: 0x040049DB RID: 18907
		[RequiredField]
		public FsmAnimationCurve curveW;

		// Token: 0x040049DC RID: 18908
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.width and toValue.width.")]
		public CurveFsmAction.Calculation calculationW;

		// Token: 0x040049DD RID: 18909
		[RequiredField]
		public FsmAnimationCurve curveH;

		// Token: 0x040049DE RID: 18910
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.height and toValue.height.")]
		public CurveFsmAction.Calculation calculationH;

		// Token: 0x040049DF RID: 18911
		private Rect rct;

		// Token: 0x040049E0 RID: 18912
		private bool finishInNextStep;
	}
}
