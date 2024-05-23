using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B4C RID: 2892
	[ActionCategory(37)]
	[Tooltip("Easing Animation - Color")]
	public class EaseColor : EaseFsmAction
	{
		// Token: 0x0600627B RID: 25211 RVA: 0x001D69D0 File Offset: 0x001D4BD0
		public override void Reset()
		{
			base.Reset();
			this.colorVariable = null;
			this.fromValue = null;
			this.toValue = null;
			this.finishInNextStep = false;
		}

		// Token: 0x0600627C RID: 25212 RVA: 0x001D6A00 File Offset: 0x001D4C00
		public override void OnEnter()
		{
			base.OnEnter();
			this.fromFloats = new float[4];
			this.fromFloats[0] = this.fromValue.Value.r;
			this.fromFloats[1] = this.fromValue.Value.g;
			this.fromFloats[2] = this.fromValue.Value.b;
			this.fromFloats[3] = this.fromValue.Value.a;
			this.toFloats = new float[4];
			this.toFloats[0] = this.toValue.Value.r;
			this.toFloats[1] = this.toValue.Value.g;
			this.toFloats[2] = this.toValue.Value.b;
			this.toFloats[3] = this.toValue.Value.a;
			this.resultFloats = new float[4];
			this.finishInNextStep = false;
		}

		// Token: 0x0600627D RID: 25213 RVA: 0x001D6B1A File Offset: 0x001D4D1A
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x0600627E RID: 25214 RVA: 0x001D6B24 File Offset: 0x001D4D24
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.colorVariable.IsNone && this.isRunning)
			{
				this.colorVariable.Value = new Color(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2], this.resultFloats[3]);
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
				if (!this.colorVariable.IsNone)
				{
					this.colorVariable.Value = new Color((!this.reverse.IsNone) ? ((!this.reverse.Value) ? this.toValue.Value.r : this.fromValue.Value.r) : this.toValue.Value.r, (!this.reverse.IsNone) ? ((!this.reverse.Value) ? this.toValue.Value.g : this.fromValue.Value.g) : this.toValue.Value.g, (!this.reverse.IsNone) ? ((!this.reverse.Value) ? this.toValue.Value.b : this.fromValue.Value.b) : this.toValue.Value.b, (!this.reverse.IsNone) ? ((!this.reverse.Value) ? this.toValue.Value.a : this.fromValue.Value.a) : this.toValue.Value.a);
				}
				this.finishInNextStep = true;
			}
		}

		// Token: 0x040049EC RID: 18924
		[RequiredField]
		public FsmColor fromValue;

		// Token: 0x040049ED RID: 18925
		[RequiredField]
		public FsmColor toValue;

		// Token: 0x040049EE RID: 18926
		[UIHint(10)]
		public FsmColor colorVariable;

		// Token: 0x040049EF RID: 18927
		private bool finishInNextStep;
	}
}
