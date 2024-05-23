using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B51 RID: 2897
	[Tooltip("Easing Animation - Rect.")]
	[ActionCategory("AnimateVariables")]
	public class EaseRect : EaseFsmAction
	{
		// Token: 0x060062AE RID: 25262 RVA: 0x001D7F24 File Offset: 0x001D6124
		public override void Reset()
		{
			base.Reset();
			this.rectVariable = null;
			this.fromValue = null;
			this.toValue = null;
			this.finishInNextStep = false;
		}

		// Token: 0x060062AF RID: 25263 RVA: 0x001D7F54 File Offset: 0x001D6154
		public override void OnEnter()
		{
			base.OnEnter();
			this.fromFloats = new float[4];
			this.fromFloats[0] = this.fromValue.Value.x;
			this.fromFloats[1] = this.fromValue.Value.y;
			this.fromFloats[2] = this.fromValue.Value.width;
			this.fromFloats[3] = this.fromValue.Value.height;
			this.toFloats = new float[4];
			this.toFloats[0] = this.toValue.Value.x;
			this.toFloats[1] = this.toValue.Value.y;
			this.toFloats[2] = this.toValue.Value.width;
			this.toFloats[3] = this.toValue.Value.height;
			this.resultFloats = new float[4];
			this.finishInNextStep = false;
		}

		// Token: 0x060062B0 RID: 25264 RVA: 0x001D806E File Offset: 0x001D626E
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x060062B1 RID: 25265 RVA: 0x001D8078 File Offset: 0x001D6278
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.rectVariable.IsNone && this.isRunning)
			{
				this.rectVariable.Value = new Rect(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2], this.resultFloats[3]);
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
				if (!this.rectVariable.IsNone)
				{
					this.rectVariable.Value = new Rect((!this.reverse.IsNone) ? ((!this.reverse.Value) ? this.toValue.Value.x : this.fromValue.Value.x) : this.toValue.Value.x, (!this.reverse.IsNone) ? ((!this.reverse.Value) ? this.toValue.Value.y : this.fromValue.Value.y) : this.toValue.Value.y, (!this.reverse.IsNone) ? ((!this.reverse.Value) ? this.toValue.Value.width : this.fromValue.Value.width) : this.toValue.Value.width, (!this.reverse.IsNone) ? ((!this.reverse.Value) ? this.toValue.Value.height : this.fromValue.Value.height) : this.toValue.Value.height);
				}
				this.finishInNextStep = true;
			}
		}

		// Token: 0x04004A27 RID: 18983
		[RequiredField]
		public FsmRect fromValue;

		// Token: 0x04004A28 RID: 18984
		[RequiredField]
		public FsmRect toValue;

		// Token: 0x04004A29 RID: 18985
		[UIHint(10)]
		public FsmRect rectVariable;

		// Token: 0x04004A2A RID: 18986
		private bool finishInNextStep;
	}
}
