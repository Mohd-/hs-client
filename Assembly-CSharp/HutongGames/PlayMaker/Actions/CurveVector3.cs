using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B4B RID: 2891
	[Tooltip("Animates the value of a Vector3 Variable FROM-TO with assistance of Deformation Curves.")]
	[ActionCategory(37)]
	public class CurveVector3 : CurveFsmAction
	{
		// Token: 0x06006276 RID: 25206 RVA: 0x001D6678 File Offset: 0x001D4878
		public override void Reset()
		{
			base.Reset();
			this.vectorVariable = new FsmVector3
			{
				UseVariable = true
			};
			this.toValue = new FsmVector3
			{
				UseVariable = true
			};
			this.fromValue = new FsmVector3
			{
				UseVariable = true
			};
		}

		// Token: 0x06006277 RID: 25207 RVA: 0x001D66C8 File Offset: 0x001D48C8
		public override void OnEnter()
		{
			base.OnEnter();
			this.finishInNextStep = false;
			this.resultFloats = new float[3];
			this.fromFloats = new float[3];
			this.fromFloats[0] = ((!this.fromValue.IsNone) ? this.fromValue.Value.x : 0f);
			this.fromFloats[1] = ((!this.fromValue.IsNone) ? this.fromValue.Value.y : 0f);
			this.fromFloats[2] = ((!this.fromValue.IsNone) ? this.fromValue.Value.z : 0f);
			this.toFloats = new float[3];
			this.toFloats[0] = ((!this.toValue.IsNone) ? this.toValue.Value.x : 0f);
			this.toFloats[1] = ((!this.toValue.IsNone) ? this.toValue.Value.y : 0f);
			this.toFloats[2] = ((!this.toValue.IsNone) ? this.toValue.Value.z : 0f);
			this.curves = new AnimationCurve[3];
			this.curves[0] = this.curveX.curve;
			this.curves[1] = this.curveY.curve;
			this.curves[2] = this.curveZ.curve;
			this.calculations = new CurveFsmAction.Calculation[3];
			this.calculations[0] = this.calculationX;
			this.calculations[1] = this.calculationY;
			this.calculations[2] = this.calculationZ;
			base.Init();
		}

		// Token: 0x06006278 RID: 25208 RVA: 0x001D68C7 File Offset: 0x001D4AC7
		public override void OnExit()
		{
		}

		// Token: 0x06006279 RID: 25209 RVA: 0x001D68CC File Offset: 0x001D4ACC
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

		// Token: 0x040049E1 RID: 18913
		[RequiredField]
		[UIHint(10)]
		public FsmVector3 vectorVariable;

		// Token: 0x040049E2 RID: 18914
		[RequiredField]
		public FsmVector3 fromValue;

		// Token: 0x040049E3 RID: 18915
		[RequiredField]
		public FsmVector3 toValue;

		// Token: 0x040049E4 RID: 18916
		[RequiredField]
		public FsmAnimationCurve curveX;

		// Token: 0x040049E5 RID: 18917
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.x and toValue.x.")]
		public CurveFsmAction.Calculation calculationX;

		// Token: 0x040049E6 RID: 18918
		[RequiredField]
		public FsmAnimationCurve curveY;

		// Token: 0x040049E7 RID: 18919
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.y and toValue.y.")]
		public CurveFsmAction.Calculation calculationY;

		// Token: 0x040049E8 RID: 18920
		[RequiredField]
		public FsmAnimationCurve curveZ;

		// Token: 0x040049E9 RID: 18921
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.z and toValue.z.")]
		public CurveFsmAction.Calculation calculationZ;

		// Token: 0x040049EA RID: 18922
		private Vector3 vct;

		// Token: 0x040049EB RID: 18923
		private bool finishInNextStep;
	}
}
