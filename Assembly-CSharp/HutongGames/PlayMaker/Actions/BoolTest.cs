using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B65 RID: 2917
	[ActionCategory(27)]
	[Tooltip("Sends Events based on the value of a Boolean Variable.")]
	public class BoolTest : FsmStateAction
	{
		// Token: 0x060062FA RID: 25338 RVA: 0x001D93AF File Offset: 0x001D75AF
		public override void Reset()
		{
			this.boolVariable = null;
			this.isTrue = null;
			this.isFalse = null;
			this.everyFrame = false;
		}

		// Token: 0x060062FB RID: 25339 RVA: 0x001D93D0 File Offset: 0x001D75D0
		public override void OnEnter()
		{
			base.Fsm.Event((!this.boolVariable.Value) ? this.isFalse : this.isTrue);
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060062FC RID: 25340 RVA: 0x001D941A File Offset: 0x001D761A
		public override void OnUpdate()
		{
			base.Fsm.Event((!this.boolVariable.Value) ? this.isFalse : this.isTrue);
		}

		// Token: 0x04004A72 RID: 19058
		[RequiredField]
		[UIHint(10)]
		[Tooltip("The Bool variable to test.")]
		public FsmBool boolVariable;

		// Token: 0x04004A73 RID: 19059
		[Tooltip("Event to send if the Bool variable is True.")]
		public FsmEvent isTrue;

		// Token: 0x04004A74 RID: 19060
		[Tooltip("Event to send if the Bool variable is False.")]
		public FsmEvent isFalse;

		// Token: 0x04004A75 RID: 19061
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;
	}
}
