using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B44 RID: 2884
	[Tooltip("Animates the value of a Rect Variable using an Animation Curve.")]
	[ActionCategory("AnimateVariables")]
	public class AnimateRect : AnimateFsmAction
	{
		// Token: 0x0600625A RID: 25178 RVA: 0x001D49EC File Offset: 0x001D2BEC
		public override void Reset()
		{
			base.Reset();
			this.rectVariable = new FsmRect
			{
				UseVariable = true
			};
		}

		// Token: 0x0600625B RID: 25179 RVA: 0x001D4A14 File Offset: 0x001D2C14
		public override void OnEnter()
		{
			base.OnEnter();
			this.finishInNextStep = false;
			this.resultFloats = new float[4];
			this.fromFloats = new float[4];
			this.fromFloats[0] = ((!this.rectVariable.IsNone) ? this.rectVariable.Value.x : 0f);
			this.fromFloats[1] = ((!this.rectVariable.IsNone) ? this.rectVariable.Value.y : 0f);
			this.fromFloats[2] = ((!this.rectVariable.IsNone) ? this.rectVariable.Value.width : 0f);
			this.fromFloats[3] = ((!this.rectVariable.IsNone) ? this.rectVariable.Value.height : 0f);
			this.curves = new AnimationCurve[4];
			this.curves[0] = this.curveX.curve;
			this.curves[1] = this.curveY.curve;
			this.curves[2] = this.curveW.curve;
			this.curves[3] = this.curveH.curve;
			this.calculations = new AnimateFsmAction.Calculation[4];
			this.calculations[0] = this.calculationX;
			this.calculations[1] = this.calculationY;
			this.calculations[2] = this.calculationW;
			this.calculations[3] = this.calculationH;
			base.Init();
		}

		// Token: 0x0600625C RID: 25180 RVA: 0x001D4BBC File Offset: 0x001D2DBC
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

		// Token: 0x0400498D RID: 18829
		[UIHint(10)]
		[RequiredField]
		public FsmRect rectVariable;

		// Token: 0x0400498E RID: 18830
		[RequiredField]
		public FsmAnimationCurve curveX;

		// Token: 0x0400498F RID: 18831
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to rectVariable.x.")]
		public AnimateFsmAction.Calculation calculationX;

		// Token: 0x04004990 RID: 18832
		[RequiredField]
		public FsmAnimationCurve curveY;

		// Token: 0x04004991 RID: 18833
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to rectVariable.y.")]
		public AnimateFsmAction.Calculation calculationY;

		// Token: 0x04004992 RID: 18834
		[RequiredField]
		public FsmAnimationCurve curveW;

		// Token: 0x04004993 RID: 18835
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to rectVariable.width.")]
		public AnimateFsmAction.Calculation calculationW;

		// Token: 0x04004994 RID: 18836
		[RequiredField]
		public FsmAnimationCurve curveH;

		// Token: 0x04004995 RID: 18837
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to rectVariable.height.")]
		public AnimateFsmAction.Calculation calculationH;

		// Token: 0x04004996 RID: 18838
		private bool finishInNextStep;

		// Token: 0x04004997 RID: 18839
		private Rect rct;
	}
}
