using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BB3 RID: 2995
	[ActionCategory(27)]
	[Tooltip("Sends an Event based on the value of a Float Variable. The float could represent distance, angle to a target, health left... The array sets up float ranges that correspond to Events.")]
	public class FloatSwitch : FsmStateAction
	{
		// Token: 0x0600643F RID: 25663 RVA: 0x001DD4C4 File Offset: 0x001DB6C4
		public override void Reset()
		{
			this.floatVariable = null;
			this.lessThan = new FsmFloat[1];
			this.sendEvent = new FsmEvent[1];
			this.everyFrame = false;
		}

		// Token: 0x06006440 RID: 25664 RVA: 0x001DD4F7 File Offset: 0x001DB6F7
		public override void OnEnter()
		{
			this.DoFloatSwitch();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006441 RID: 25665 RVA: 0x001DD510 File Offset: 0x001DB710
		public override void OnUpdate()
		{
			this.DoFloatSwitch();
		}

		// Token: 0x06006442 RID: 25666 RVA: 0x001DD518 File Offset: 0x001DB718
		private void DoFloatSwitch()
		{
			if (this.floatVariable.IsNone)
			{
				return;
			}
			for (int i = 0; i < this.lessThan.Length; i++)
			{
				if (this.floatVariable.Value < this.lessThan[i].Value)
				{
					base.Fsm.Event(this.sendEvent[i]);
					return;
				}
			}
		}

		// Token: 0x04004BB4 RID: 19380
		[RequiredField]
		[UIHint(10)]
		[Tooltip("The float variable to test.")]
		public FsmFloat floatVariable;

		// Token: 0x04004BB5 RID: 19381
		[CompoundArray("Float Switches", "Less Than", "Send Event")]
		public FsmFloat[] lessThan;

		// Token: 0x04004BB6 RID: 19382
		public FsmEvent[] sendEvent;

		// Token: 0x04004BB7 RID: 19383
		[Tooltip("Repeat every frame. Useful if the variable is changing.")]
		public bool everyFrame;
	}
}
