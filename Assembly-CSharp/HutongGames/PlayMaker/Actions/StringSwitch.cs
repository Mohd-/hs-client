using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CFC RID: 3324
	[ActionCategory(27)]
	[Tooltip("Sends an Event based on the value of a String Variable.")]
	public class StringSwitch : FsmStateAction
	{
		// Token: 0x060069A6 RID: 27046 RVA: 0x001EF240 File Offset: 0x001ED440
		public override void Reset()
		{
			this.stringVariable = null;
			this.compareTo = new FsmString[1];
			this.sendEvent = new FsmEvent[1];
			this.everyFrame = false;
		}

		// Token: 0x060069A7 RID: 27047 RVA: 0x001EF273 File Offset: 0x001ED473
		public override void OnEnter()
		{
			this.DoStringSwitch();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060069A8 RID: 27048 RVA: 0x001EF28C File Offset: 0x001ED48C
		public override void OnUpdate()
		{
			this.DoStringSwitch();
		}

		// Token: 0x060069A9 RID: 27049 RVA: 0x001EF294 File Offset: 0x001ED494
		private void DoStringSwitch()
		{
			if (this.stringVariable.IsNone)
			{
				return;
			}
			for (int i = 0; i < this.compareTo.Length; i++)
			{
				if (this.stringVariable.Value == this.compareTo[i].Value)
				{
					base.Fsm.Event(this.sendEvent[i]);
					return;
				}
			}
		}

		// Token: 0x04005160 RID: 20832
		[UIHint(10)]
		[RequiredField]
		public FsmString stringVariable;

		// Token: 0x04005161 RID: 20833
		[CompoundArray("String Switches", "Compare String", "Send Event")]
		public FsmString[] compareTo;

		// Token: 0x04005162 RID: 20834
		public FsmEvent[] sendEvent;

		// Token: 0x04005163 RID: 20835
		public bool everyFrame;
	}
}
