using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C4D RID: 3149
	[Tooltip("Invokes a Method in a Behaviour attached to a Game Object. See Unity InvokeMethod docs.")]
	[ActionCategory(11)]
	public class InvokeMethod : FsmStateAction
	{
		// Token: 0x060066A1 RID: 26273 RVA: 0x001E4DF4 File Offset: 0x001E2FF4
		public override void Reset()
		{
			this.gameObject = null;
			this.behaviour = null;
			this.methodName = string.Empty;
			this.delay = null;
			this.repeating = false;
			this.repeatDelay = 1f;
			this.cancelOnExit = false;
		}

		// Token: 0x060066A2 RID: 26274 RVA: 0x001E4E4E File Offset: 0x001E304E
		public override void OnEnter()
		{
			this.DoInvokeMethod(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			base.Finish();
		}

		// Token: 0x060066A3 RID: 26275 RVA: 0x001E4E70 File Offset: 0x001E3070
		private void DoInvokeMethod(GameObject go)
		{
			if (go == null)
			{
				return;
			}
			this.component = (go.GetComponent(this.behaviour.Value) as MonoBehaviour);
			if (this.component == null)
			{
				this.LogWarning("InvokeMethod: " + go.name + " missing behaviour: " + this.behaviour.Value);
				return;
			}
			if (this.repeating.Value)
			{
				this.component.InvokeRepeating(this.methodName.Value, this.delay.Value, this.repeatDelay.Value);
			}
			else
			{
				this.component.Invoke(this.methodName.Value, this.delay.Value);
			}
		}

		// Token: 0x060066A4 RID: 26276 RVA: 0x001E4F40 File Offset: 0x001E3140
		public override void OnExit()
		{
			if (this.component == null)
			{
				return;
			}
			if (this.cancelOnExit.Value)
			{
				this.component.CancelInvoke(this.methodName.Value);
			}
		}

		// Token: 0x04004E57 RID: 20055
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004E58 RID: 20056
		[UIHint(3)]
		[RequiredField]
		public FsmString behaviour;

		// Token: 0x04004E59 RID: 20057
		[RequiredField]
		[UIHint(4)]
		public FsmString methodName;

		// Token: 0x04004E5A RID: 20058
		[HasFloatSlider(0f, 10f)]
		public FsmFloat delay;

		// Token: 0x04004E5B RID: 20059
		public FsmBool repeating;

		// Token: 0x04004E5C RID: 20060
		[HasFloatSlider(0f, 10f)]
		public FsmFloat repeatDelay;

		// Token: 0x04004E5D RID: 20061
		public FsmBool cancelOnExit;

		// Token: 0x04004E5E RID: 20062
		private MonoBehaviour component;
	}
}
