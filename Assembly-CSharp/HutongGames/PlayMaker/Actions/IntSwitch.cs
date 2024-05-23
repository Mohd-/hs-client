using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C4A RID: 3146
	[Tooltip("Sends an Event based on the value of an Integer Variable.")]
	[ActionCategory(27)]
	public class IntSwitch : FsmStateAction
	{
		// Token: 0x06006692 RID: 26258 RVA: 0x001E4C04 File Offset: 0x001E2E04
		public override void Reset()
		{
			this.intVariable = null;
			this.compareTo = new FsmInt[1];
			this.sendEvent = new FsmEvent[1];
			this.everyFrame = false;
		}

		// Token: 0x06006693 RID: 26259 RVA: 0x001E4C37 File Offset: 0x001E2E37
		public override void OnEnter()
		{
			this.DoIntSwitch();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006694 RID: 26260 RVA: 0x001E4C50 File Offset: 0x001E2E50
		public override void OnUpdate()
		{
			this.DoIntSwitch();
		}

		// Token: 0x06006695 RID: 26261 RVA: 0x001E4C58 File Offset: 0x001E2E58
		private void DoIntSwitch()
		{
			if (this.intVariable.IsNone)
			{
				return;
			}
			for (int i = 0; i < this.compareTo.Length; i++)
			{
				if (this.intVariable.Value == this.compareTo[i].Value)
				{
					base.Fsm.Event(this.sendEvent[i]);
					return;
				}
			}
		}

		// Token: 0x04004E4B RID: 20043
		[UIHint(10)]
		[RequiredField]
		public FsmInt intVariable;

		// Token: 0x04004E4C RID: 20044
		[CompoundArray("Int Switches", "Compare Int", "Send Event")]
		public FsmInt[] compareTo;

		// Token: 0x04004E4D RID: 20045
		public FsmEvent[] sendEvent;

		// Token: 0x04004E4E RID: 20046
		public bool everyFrame;
	}
}
