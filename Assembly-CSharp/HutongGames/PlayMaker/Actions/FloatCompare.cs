using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BAB RID: 2987
	[ActionCategory(27)]
	[Tooltip("Sends Events based on the comparison of 2 Floats.")]
	public class FloatCompare : FsmStateAction
	{
		// Token: 0x0600641D RID: 25629 RVA: 0x001DCE74 File Offset: 0x001DB074
		public override void Reset()
		{
			this.float1 = 0f;
			this.float2 = 0f;
			this.tolerance = 0f;
			this.equal = null;
			this.lessThan = null;
			this.greaterThan = null;
			this.everyFrame = false;
		}

		// Token: 0x0600641E RID: 25630 RVA: 0x001DCECD File Offset: 0x001DB0CD
		public override void OnEnter()
		{
			this.DoCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600641F RID: 25631 RVA: 0x001DCEE6 File Offset: 0x001DB0E6
		public override void OnUpdate()
		{
			this.DoCompare();
		}

		// Token: 0x06006420 RID: 25632 RVA: 0x001DCEF0 File Offset: 0x001DB0F0
		private void DoCompare()
		{
			if (Mathf.Abs(this.float1.Value - this.float2.Value) <= this.tolerance.Value)
			{
				base.Fsm.Event(this.equal);
				return;
			}
			if (this.float1.Value < this.float2.Value)
			{
				base.Fsm.Event(this.lessThan);
				return;
			}
			if (this.float1.Value > this.float2.Value)
			{
				base.Fsm.Event(this.greaterThan);
			}
		}

		// Token: 0x06006421 RID: 25633 RVA: 0x001DCF94 File Offset: 0x001DB194
		public override string ErrorCheck()
		{
			if (FsmEvent.IsNullOrEmpty(this.equal) && FsmEvent.IsNullOrEmpty(this.lessThan) && FsmEvent.IsNullOrEmpty(this.greaterThan))
			{
				return "Action sends no events!";
			}
			return string.Empty;
		}

		// Token: 0x04004B8A RID: 19338
		[RequiredField]
		[Tooltip("The first float variable.")]
		public FsmFloat float1;

		// Token: 0x04004B8B RID: 19339
		[Tooltip("The second float variable.")]
		[RequiredField]
		public FsmFloat float2;

		// Token: 0x04004B8C RID: 19340
		[RequiredField]
		[Tooltip("Tolerance for the Equal test (almost equal).")]
		public FsmFloat tolerance;

		// Token: 0x04004B8D RID: 19341
		[Tooltip("Event sent if Float 1 equals Float 2 (within Tolerance)")]
		public FsmEvent equal;

		// Token: 0x04004B8E RID: 19342
		[Tooltip("Event sent if Float 1 is less than Float 2")]
		public FsmEvent lessThan;

		// Token: 0x04004B8F RID: 19343
		[Tooltip("Event sent if Float 1 is greater than Float 2")]
		public FsmEvent greaterThan;

		// Token: 0x04004B90 RID: 19344
		[Tooltip("Repeat every frame. Useful if the variables are changing and you're waiting for a particular result.")]
		public bool everyFrame;
	}
}
