using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C93 RID: 3219
	[ActionCategory(12)]
	[Tooltip("Set the value of a Game Object Variable in another All FSM. Accept null reference")]
	public class SetAllFsmGameObject : FsmStateAction
	{
		// Token: 0x060067D2 RID: 26578 RVA: 0x001E982C File Offset: 0x001E7A2C
		public override void Reset()
		{
		}

		// Token: 0x060067D3 RID: 26579 RVA: 0x001E982E File Offset: 0x001E7A2E
		public override void OnEnter()
		{
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060067D4 RID: 26580 RVA: 0x001E9841 File Offset: 0x001E7A41
		private void DoSetFsmGameObject()
		{
		}

		// Token: 0x04004FAD RID: 20397
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004FAE RID: 20398
		public bool everyFrame;
	}
}
