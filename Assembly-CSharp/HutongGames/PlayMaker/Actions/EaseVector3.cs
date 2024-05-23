using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B52 RID: 2898
	[Tooltip("Easing Animation - Vector3")]
	[ActionCategory(37)]
	public class EaseVector3 : EaseFsmAction
	{
		// Token: 0x060062B3 RID: 25267 RVA: 0x001D82DC File Offset: 0x001D64DC
		public override void Reset()
		{
			base.Reset();
			this.vector3Variable = null;
			this.fromValue = null;
			this.toValue = null;
			this.finishInNextStep = false;
		}

		// Token: 0x060062B4 RID: 25268 RVA: 0x001D830C File Offset: 0x001D650C
		public override void OnEnter()
		{
			base.OnEnter();
			this.fromFloats = new float[3];
			this.fromFloats[0] = this.fromValue.Value.x;
			this.fromFloats[1] = this.fromValue.Value.y;
			this.fromFloats[2] = this.fromValue.Value.z;
			this.toFloats = new float[3];
			this.toFloats[0] = this.toValue.Value.x;
			this.toFloats[1] = this.toValue.Value.y;
			this.toFloats[2] = this.toValue.Value.z;
			this.resultFloats = new float[3];
			this.finishInNextStep = false;
		}

		// Token: 0x060062B5 RID: 25269 RVA: 0x001D83EE File Offset: 0x001D65EE
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x060062B6 RID: 25270 RVA: 0x001D83F8 File Offset: 0x001D65F8
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.vector3Variable.IsNone && this.isRunning)
			{
				this.vector3Variable.Value = new Vector3(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2]);
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
				if (!this.vector3Variable.IsNone)
				{
					this.vector3Variable.Value = new Vector3((!this.reverse.IsNone) ? ((!this.reverse.Value) ? this.toValue.Value.x : this.fromValue.Value.x) : this.toValue.Value.x, (!this.reverse.IsNone) ? ((!this.reverse.Value) ? this.toValue.Value.y : this.fromValue.Value.y) : this.toValue.Value.y, (!this.reverse.IsNone) ? ((!this.reverse.Value) ? this.toValue.Value.z : this.fromValue.Value.z) : this.toValue.Value.z);
				}
				this.finishInNextStep = true;
			}
		}

		// Token: 0x04004A2B RID: 18987
		[RequiredField]
		public FsmVector3 fromValue;

		// Token: 0x04004A2C RID: 18988
		[RequiredField]
		public FsmVector3 toValue;

		// Token: 0x04004A2D RID: 18989
		[UIHint(10)]
		public FsmVector3 vector3Variable;

		// Token: 0x04004A2E RID: 18990
		private bool finishInNextStep;
	}
}
