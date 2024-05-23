using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BB7 RID: 2999
	[Tooltip("Sets how subsequent events sent in this state are handled.")]
	[ActionCategory(12)]
	public class FsmEventOptions : FsmStateAction
	{
		// Token: 0x0600644F RID: 25679 RVA: 0x001DD7B4 File Offset: 0x001DB9B4
		public override void Reset()
		{
			this.sendToFsmComponent = null;
			this.sendToGameObject = null;
			this.fsmName = string.Empty;
			this.sendToChildren = false;
			this.broadcastToAll = false;
		}

		// Token: 0x06006450 RID: 25680 RVA: 0x001DD7F7 File Offset: 0x001DB9F7
		public override void OnUpdate()
		{
		}

		// Token: 0x04004BC3 RID: 19395
		public PlayMakerFSM sendToFsmComponent;

		// Token: 0x04004BC4 RID: 19396
		public FsmGameObject sendToGameObject;

		// Token: 0x04004BC5 RID: 19397
		public FsmString fsmName;

		// Token: 0x04004BC6 RID: 19398
		public FsmBool sendToChildren;

		// Token: 0x04004BC7 RID: 19399
		public FsmBool broadcastToAll;
	}
}
