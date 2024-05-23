using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B46 RID: 2886
	[Tooltip("Animates the value of a Color Variable FROM-TO with assistance of Deformation Curves.")]
	[ActionCategory(37)]
	public class CurveColor : CurveFsmAction
	{
		// Token: 0x06006262 RID: 25186 RVA: 0x001D4F50 File Offset: 0x001D3150
		public override void Reset()
		{
			base.Reset();
			this.colorVariable = new FsmColor
			{
				UseVariable = true
			};
			this.toValue = new FsmColor
			{
				UseVariable = true
			};
			this.fromValue = new FsmColor
			{
				UseVariable = true
			};
		}

		// Token: 0x06006263 RID: 25187 RVA: 0x001D4FA0 File Offset: 0x001D31A0
		public override void OnEnter()
		{
			base.OnEnter();
			this.finishInNextStep = false;
			this.resultFloats = new float[4];
			this.fromFloats = new float[4];
			this.fromFloats[0] = ((!this.fromValue.IsNone) ? this.fromValue.Value.r : 0f);
			this.fromFloats[1] = ((!this.fromValue.IsNone) ? this.fromValue.Value.g : 0f);
			this.fromFloats[2] = ((!this.fromValue.IsNone) ? this.fromValue.Value.b : 0f);
			this.fromFloats[3] = ((!this.fromValue.IsNone) ? this.fromValue.Value.a : 0f);
			this.toFloats = new float[4];
			this.toFloats[0] = ((!this.toValue.IsNone) ? this.toValue.Value.r : 0f);
			this.toFloats[1] = ((!this.toValue.IsNone) ? this.toValue.Value.g : 0f);
			this.toFloats[2] = ((!this.toValue.IsNone) ? this.toValue.Value.b : 0f);
			this.toFloats[3] = ((!this.toValue.IsNone) ? this.toValue.Value.a : 0f);
			this.curves = new AnimationCurve[4];
			this.curves[0] = this.curveR.curve;
			this.curves[1] = this.curveG.curve;
			this.curves[2] = this.curveB.curve;
			this.curves[3] = this.curveA.curve;
			this.calculations = new CurveFsmAction.Calculation[4];
			this.calculations[0] = this.calculationR;
			this.calculations[1] = this.calculationG;
			this.calculations[2] = this.calculationB;
			this.calculations[2] = this.calculationA;
			base.Init();
		}

		// Token: 0x06006264 RID: 25188 RVA: 0x001D522C File Offset: 0x001D342C
		public override void OnExit()
		{
		}

		// Token: 0x06006265 RID: 25189 RVA: 0x001D5230 File Offset: 0x001D3430
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.colorVariable.IsNone && this.isRunning)
			{
				this.clr = new Color(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2], this.resultFloats[3]);
				this.colorVariable.Value = this.clr;
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
				if (!this.colorVariable.IsNone)
				{
					this.clr = new Color(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2], this.resultFloats[3]);
					this.colorVariable.Value = this.clr;
				}
				this.finishInNextStep = true;
			}
		}

		// Token: 0x040049A1 RID: 18849
		[RequiredField]
		[UIHint(10)]
		public FsmColor colorVariable;

		// Token: 0x040049A2 RID: 18850
		[RequiredField]
		public FsmColor fromValue;

		// Token: 0x040049A3 RID: 18851
		[RequiredField]
		public FsmColor toValue;

		// Token: 0x040049A4 RID: 18852
		[RequiredField]
		public FsmAnimationCurve curveR;

		// Token: 0x040049A5 RID: 18853
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.Red and toValue.Rec.")]
		public CurveFsmAction.Calculation calculationR;

		// Token: 0x040049A6 RID: 18854
		[RequiredField]
		public FsmAnimationCurve curveG;

		// Token: 0x040049A7 RID: 18855
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.Green and toValue.Green.")]
		public CurveFsmAction.Calculation calculationG;

		// Token: 0x040049A8 RID: 18856
		[RequiredField]
		public FsmAnimationCurve curveB;

		// Token: 0x040049A9 RID: 18857
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.Blue and toValue.Blue.")]
		public CurveFsmAction.Calculation calculationB;

		// Token: 0x040049AA RID: 18858
		[RequiredField]
		public FsmAnimationCurve curveA;

		// Token: 0x040049AB RID: 18859
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.Alpha and toValue.Alpha.")]
		public CurveFsmAction.Calculation calculationA;

		// Token: 0x040049AC RID: 18860
		private Color clr;

		// Token: 0x040049AD RID: 18861
		private bool finishInNextStep;
	}
}
