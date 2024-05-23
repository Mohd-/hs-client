using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B50 RID: 2896
	[Tooltip("Easing Animation - Float")]
	[ActionCategory(37)]
	public class EaseFloat : EaseFsmAction
	{
		// Token: 0x060062A9 RID: 25257 RVA: 0x001D7D8C File Offset: 0x001D5F8C
		public override void Reset()
		{
			base.Reset();
			this.floatVariable = null;
			this.fromValue = null;
			this.toValue = null;
			this.finishInNextStep = false;
		}

		// Token: 0x060062AA RID: 25258 RVA: 0x001D7DBC File Offset: 0x001D5FBC
		public override void OnEnter()
		{
			base.OnEnter();
			this.fromFloats = new float[1];
			this.fromFloats[0] = this.fromValue.Value;
			this.toFloats = new float[1];
			this.toFloats[0] = this.toValue.Value;
			this.resultFloats = new float[1];
			this.finishInNextStep = false;
		}

		// Token: 0x060062AB RID: 25259 RVA: 0x001D7E20 File Offset: 0x001D6020
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x060062AC RID: 25260 RVA: 0x001D7E28 File Offset: 0x001D6028
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.floatVariable.IsNone && this.isRunning)
			{
				this.floatVariable.Value = this.resultFloats[0];
			}
			if (this.finishInNextStep)
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
					this.floatVariable.Value = ((!this.reverse.IsNone) ? ((!this.reverse.Value) ? this.toValue.Value : this.fromValue.Value) : this.toValue.Value);
				}
				this.finishInNextStep = true;
			}
		}

		// Token: 0x04004A23 RID: 18979
		[RequiredField]
		public FsmFloat fromValue;

		// Token: 0x04004A24 RID: 18980
		[RequiredField]
		public FsmFloat toValue;

		// Token: 0x04004A25 RID: 18981
		[UIHint(10)]
		public FsmFloat floatVariable;

		// Token: 0x04004A26 RID: 18982
		private bool finishInNextStep;
	}
}
