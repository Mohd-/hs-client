using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BB1 RID: 2993
	[Tooltip("Sends Events based on the sign of a Float.")]
	[ActionCategory(27)]
	public class FloatSignTest : FsmStateAction
	{
		// Token: 0x06006434 RID: 25652 RVA: 0x001DD361 File Offset: 0x001DB561
		public override void Reset()
		{
			this.floatValue = 0f;
			this.isPositive = null;
			this.isNegative = null;
			this.everyFrame = false;
		}

		// Token: 0x06006435 RID: 25653 RVA: 0x001DD388 File Offset: 0x001DB588
		public override void OnEnter()
		{
			this.DoSignTest();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006436 RID: 25654 RVA: 0x001DD3A1 File Offset: 0x001DB5A1
		public override void OnUpdate()
		{
			this.DoSignTest();
		}

		// Token: 0x06006437 RID: 25655 RVA: 0x001DD3A9 File Offset: 0x001DB5A9
		private void DoSignTest()
		{
			if (this.floatValue == null)
			{
				return;
			}
			base.Fsm.Event((this.floatValue.Value >= 0f) ? this.isPositive : this.isNegative);
		}

		// Token: 0x06006438 RID: 25656 RVA: 0x001DD3E8 File Offset: 0x001DB5E8
		public override string ErrorCheck()
		{
			if (FsmEvent.IsNullOrEmpty(this.isPositive) && FsmEvent.IsNullOrEmpty(this.isNegative))
			{
				return "Action sends no events!";
			}
			return string.Empty;
		}

		// Token: 0x04004BAC RID: 19372
		[RequiredField]
		[UIHint(10)]
		[Tooltip("The float variable to test.")]
		public FsmFloat floatValue;

		// Token: 0x04004BAD RID: 19373
		[Tooltip("Event to send if the float variable is positive.")]
		public FsmEvent isPositive;

		// Token: 0x04004BAE RID: 19374
		[Tooltip("Event to send if the float variable is negative.")]
		public FsmEvent isNegative;

		// Token: 0x04004BAF RID: 19375
		[Tooltip("Repeat every frame. Useful if the variable is changing and you're waiting for a particular result.")]
		public bool everyFrame;
	}
}
