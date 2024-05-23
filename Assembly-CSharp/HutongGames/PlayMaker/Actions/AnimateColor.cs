using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B40 RID: 2880
	[Tooltip("Animates the value of a Color Variable using an Animation Curve.")]
	[ActionCategory(37)]
	public class AnimateColor : AnimateFsmAction
	{
		// Token: 0x0600624C RID: 25164 RVA: 0x001D3A3C File Offset: 0x001D1C3C
		public override void Reset()
		{
			base.Reset();
			this.colorVariable = new FsmColor
			{
				UseVariable = true
			};
		}

		// Token: 0x0600624D RID: 25165 RVA: 0x001D3A64 File Offset: 0x001D1C64
		public override void OnEnter()
		{
			base.OnEnter();
			this.finishInNextStep = false;
			this.resultFloats = new float[4];
			this.fromFloats = new float[4];
			this.fromFloats[0] = ((!this.colorVariable.IsNone) ? this.colorVariable.Value.r : 0f);
			this.fromFloats[1] = ((!this.colorVariable.IsNone) ? this.colorVariable.Value.g : 0f);
			this.fromFloats[2] = ((!this.colorVariable.IsNone) ? this.colorVariable.Value.b : 0f);
			this.fromFloats[3] = ((!this.colorVariable.IsNone) ? this.colorVariable.Value.a : 0f);
			this.curves = new AnimationCurve[4];
			this.curves[0] = this.curveR.curve;
			this.curves[1] = this.curveG.curve;
			this.curves[2] = this.curveB.curve;
			this.curves[3] = this.curveA.curve;
			this.calculations = new AnimateFsmAction.Calculation[4];
			this.calculations[0] = this.calculationR;
			this.calculations[1] = this.calculationG;
			this.calculations[2] = this.calculationB;
			this.calculations[3] = this.calculationA;
			base.Init();
		}

		// Token: 0x0600624E RID: 25166 RVA: 0x001D3C0C File Offset: 0x001D1E0C
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

		// Token: 0x0400495E RID: 18782
		[RequiredField]
		[UIHint(10)]
		public FsmColor colorVariable;

		// Token: 0x0400495F RID: 18783
		[RequiredField]
		public FsmAnimationCurve curveR;

		// Token: 0x04004960 RID: 18784
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to colorVariable.r.")]
		public AnimateFsmAction.Calculation calculationR;

		// Token: 0x04004961 RID: 18785
		[RequiredField]
		public FsmAnimationCurve curveG;

		// Token: 0x04004962 RID: 18786
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to colorVariable.g.")]
		public AnimateFsmAction.Calculation calculationG;

		// Token: 0x04004963 RID: 18787
		[RequiredField]
		public FsmAnimationCurve curveB;

		// Token: 0x04004964 RID: 18788
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to colorVariable.b.")]
		public AnimateFsmAction.Calculation calculationB;

		// Token: 0x04004965 RID: 18789
		[RequiredField]
		public FsmAnimationCurve curveA;

		// Token: 0x04004966 RID: 18790
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to colorVariable.a.")]
		public AnimateFsmAction.Calculation calculationA;

		// Token: 0x04004967 RID: 18791
		private bool finishInNextStep;

		// Token: 0x04004968 RID: 18792
		private Color clr;
	}
}
