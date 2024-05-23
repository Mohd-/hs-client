using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C23 RID: 3107
	[ActionCategory(12)]
	[Tooltip("Gets the name of the previously active state and stores it in a String Variable.")]
	public class GetPreviousStateName : FsmStateAction
	{
		// Token: 0x060065EB RID: 26091 RVA: 0x001E3161 File Offset: 0x001E1361
		public override void Reset()
		{
			this.storeName = null;
		}

		// Token: 0x060065EC RID: 26092 RVA: 0x001E316C File Offset: 0x001E136C
		public override void OnEnter()
		{
			this.storeName.Value = ((base.Fsm.PreviousActiveState != null) ? base.Fsm.PreviousActiveState.Name : null);
			base.Finish();
		}

		// Token: 0x04004DAF RID: 19887
		[UIHint(10)]
		public FsmString storeName;
	}
}
