using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C47 RID: 3143
	[Tooltip("Sends Events based on the comparison of 2 Integers.")]
	[ActionCategory(27)]
	public class IntCompare : FsmStateAction
	{
		// Token: 0x06006687 RID: 26247 RVA: 0x001E49AC File Offset: 0x001E2BAC
		public override void Reset()
		{
			this.integer1 = 0;
			this.integer2 = 0;
			this.equal = null;
			this.lessThan = null;
			this.greaterThan = null;
			this.everyFrame = false;
		}

		// Token: 0x06006688 RID: 26248 RVA: 0x001E49ED File Offset: 0x001E2BED
		public override void OnEnter()
		{
			this.DoIntCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006689 RID: 26249 RVA: 0x001E4A06 File Offset: 0x001E2C06
		public override void OnUpdate()
		{
			this.DoIntCompare();
		}

		// Token: 0x0600668A RID: 26250 RVA: 0x001E4A10 File Offset: 0x001E2C10
		private void DoIntCompare()
		{
			if (this.integer1.Value == this.integer2.Value)
			{
				base.Fsm.Event(this.equal);
				return;
			}
			if (this.integer1.Value < this.integer2.Value)
			{
				base.Fsm.Event(this.lessThan);
				return;
			}
			if (this.integer1.Value > this.integer2.Value)
			{
				base.Fsm.Event(this.greaterThan);
			}
		}

		// Token: 0x0600668B RID: 26251 RVA: 0x001E4AA3 File Offset: 0x001E2CA3
		public override string ErrorCheck()
		{
			if (FsmEvent.IsNullOrEmpty(this.equal) && FsmEvent.IsNullOrEmpty(this.lessThan) && FsmEvent.IsNullOrEmpty(this.greaterThan))
			{
				return "Action sends no events!";
			}
			return string.Empty;
		}

		// Token: 0x04004E39 RID: 20025
		[RequiredField]
		public FsmInt integer1;

		// Token: 0x04004E3A RID: 20026
		[RequiredField]
		public FsmInt integer2;

		// Token: 0x04004E3B RID: 20027
		[Tooltip("Event sent if Int 1 equals Int 2")]
		public FsmEvent equal;

		// Token: 0x04004E3C RID: 20028
		[Tooltip("Event sent if Int 1 is less than Int 2")]
		public FsmEvent lessThan;

		// Token: 0x04004E3D RID: 20029
		[Tooltip("Event sent if Int 1 is greater than Int 2")]
		public FsmEvent greaterThan;

		// Token: 0x04004E3E RID: 20030
		public bool everyFrame;
	}
}
