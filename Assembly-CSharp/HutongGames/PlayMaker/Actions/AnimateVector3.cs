using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B45 RID: 2885
	[ActionCategory(37)]
	[Tooltip("Animates the value of a Vector3 Variable using an Animation Curve.")]
	public class AnimateVector3 : AnimateFsmAction
	{
		// Token: 0x0600625E RID: 25182 RVA: 0x001D4CD0 File Offset: 0x001D2ED0
		public override void Reset()
		{
			base.Reset();
			this.vectorVariable = new FsmVector3
			{
				UseVariable = true
			};
		}

		// Token: 0x0600625F RID: 25183 RVA: 0x001D4CF8 File Offset: 0x001D2EF8
		public override void OnEnter()
		{
			base.OnEnter();
			this.finishInNextStep = false;
			this.resultFloats = new float[3];
			this.fromFloats = new float[3];
			this.fromFloats[0] = ((!this.vectorVariable.IsNone) ? this.vectorVariable.Value.x : 0f);
			this.fromFloats[1] = ((!this.vectorVariable.IsNone) ? this.vectorVariable.Value.y : 0f);
			this.fromFloats[2] = ((!this.vectorVariable.IsNone) ? this.vectorVariable.Value.z : 0f);
			this.curves = new AnimationCurve[3];
			this.curves[0] = this.curveX.curve;
			this.curves[1] = this.curveY.curve;
			this.curves[2] = this.curveZ.curve;
			this.calculations = new AnimateFsmAction.Calculation[3];
			this.calculations[0] = this.calculationX;
			this.calculations[1] = this.calculationY;
			this.calculations[2] = this.calculationZ;
			base.Init();
		}

		// Token: 0x06006260 RID: 25184 RVA: 0x001D4E4C File Offset: 0x001D304C
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.vectorVariable.IsNone && this.isRunning)
			{
				this.vct = new Vector3(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2]);
				this.vectorVariable.Value = this.vct;
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
				if (!this.vectorVariable.IsNone)
				{
					this.vct = new Vector3(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2]);
					this.vectorVariable.Value = this.vct;
				}
				this.finishInNextStep = true;
			}
		}

		// Token: 0x04004998 RID: 18840
		[RequiredField]
		[UIHint(10)]
		public FsmVector3 vectorVariable;

		// Token: 0x04004999 RID: 18841
		[RequiredField]
		public FsmAnimationCurve curveX;

		// Token: 0x0400499A RID: 18842
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to vectorVariable.x.")]
		public AnimateFsmAction.Calculation calculationX;

		// Token: 0x0400499B RID: 18843
		[RequiredField]
		public FsmAnimationCurve curveY;

		// Token: 0x0400499C RID: 18844
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to vectorVariable.y.")]
		public AnimateFsmAction.Calculation calculationY;

		// Token: 0x0400499D RID: 18845
		[RequiredField]
		public FsmAnimationCurve curveZ;

		// Token: 0x0400499E RID: 18846
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to vectorVariable.z.")]
		public AnimateFsmAction.Calculation calculationZ;

		// Token: 0x0400499F RID: 18847
		private bool finishInNextStep;

		// Token: 0x040049A0 RID: 18848
		private Vector3 vct;
	}
}
