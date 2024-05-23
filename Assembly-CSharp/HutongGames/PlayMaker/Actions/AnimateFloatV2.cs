using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B43 RID: 2883
	[ActionCategory(37)]
	[Tooltip("Animates the value of a Float Variable using an Animation Curve.")]
	public class AnimateFloatV2 : AnimateFsmAction
	{
		// Token: 0x06006255 RID: 25173 RVA: 0x001D4860 File Offset: 0x001D2A60
		public override void Reset()
		{
			base.Reset();
			this.floatVariable = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x06006256 RID: 25174 RVA: 0x001D4888 File Offset: 0x001D2A88
		public override void OnEnter()
		{
			base.OnEnter();
			this.finishInNextStep = false;
			this.resultFloats = new float[1];
			this.fromFloats = new float[1];
			this.fromFloats[0] = ((!this.floatVariable.IsNone) ? this.floatVariable.Value : 0f);
			this.calculations = new AnimateFsmAction.Calculation[1];
			this.calculations[0] = this.calculation;
			this.curves = new AnimationCurve[1];
			this.curves[0] = this.animCurve.curve;
			base.Init();
		}

		// Token: 0x06006257 RID: 25175 RVA: 0x001D4926 File Offset: 0x001D2B26
		public override void OnExit()
		{
		}

		// Token: 0x06006258 RID: 25176 RVA: 0x001D4928 File Offset: 0x001D2B28
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

		// Token: 0x04004989 RID: 18825
		[UIHint(10)]
		[RequiredField]
		public FsmFloat floatVariable;

		// Token: 0x0400498A RID: 18826
		[RequiredField]
		public FsmAnimationCurve animCurve;

		// Token: 0x0400498B RID: 18827
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to floatVariable")]
		public AnimateFsmAction.Calculation calculation;

		// Token: 0x0400498C RID: 18828
		private bool finishInNextStep;
	}
}
