using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C5D RID: 3165
	[ActionCategory(27)]
	[Tooltip("Compare 2 Object Variables and send events based on the result.")]
	public class ObjectCompare : FsmStateAction
	{
		// Token: 0x060066EB RID: 26347 RVA: 0x001E6198 File Offset: 0x001E4398
		public override void Reset()
		{
			this.objectVariable = null;
			this.compareTo = null;
			this.storeResult = null;
			this.equalEvent = null;
			this.notEqualEvent = null;
			this.everyFrame = false;
		}

		// Token: 0x060066EC RID: 26348 RVA: 0x001E61CF File Offset: 0x001E43CF
		public override void OnEnter()
		{
			this.DoObjectCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060066ED RID: 26349 RVA: 0x001E61E8 File Offset: 0x001E43E8
		public override void OnUpdate()
		{
			this.DoObjectCompare();
		}

		// Token: 0x060066EE RID: 26350 RVA: 0x001E61F0 File Offset: 0x001E43F0
		private void DoObjectCompare()
		{
			bool flag = this.objectVariable.Value == this.compareTo.Value;
			this.storeResult.Value = flag;
			base.Fsm.Event((!flag) ? this.notEqualEvent : this.equalEvent);
		}

		// Token: 0x04004EBD RID: 20157
		[RequiredField]
		[UIHint(10)]
		public FsmObject objectVariable;

		// Token: 0x04004EBE RID: 20158
		[RequiredField]
		public FsmObject compareTo;

		// Token: 0x04004EBF RID: 20159
		[Tooltip("Event to send if the 2 object values are equal.")]
		public FsmEvent equalEvent;

		// Token: 0x04004EC0 RID: 20160
		[Tooltip("Event to send if the 2 object values are not equal.")]
		public FsmEvent notEqualEvent;

		// Token: 0x04004EC1 RID: 20161
		[UIHint(10)]
		[Tooltip("Store the result in a variable.")]
		public FsmBool storeResult;

		// Token: 0x04004EC2 RID: 20162
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
