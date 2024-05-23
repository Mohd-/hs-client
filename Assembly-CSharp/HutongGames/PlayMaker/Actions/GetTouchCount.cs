using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C38 RID: 3128
	[ActionCategory(33)]
	[Tooltip("Gets the number of Touches.")]
	public class GetTouchCount : FsmStateAction
	{
		// Token: 0x06006643 RID: 26179 RVA: 0x001E3E52 File Offset: 0x001E2052
		public override void Reset()
		{
			this.storeCount = null;
			this.everyFrame = false;
		}

		// Token: 0x06006644 RID: 26180 RVA: 0x001E3E62 File Offset: 0x001E2062
		public override void OnEnter()
		{
			this.DoGetTouchCount();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006645 RID: 26181 RVA: 0x001E3E7B File Offset: 0x001E207B
		public override void OnUpdate()
		{
			this.DoGetTouchCount();
		}

		// Token: 0x06006646 RID: 26182 RVA: 0x001E3E83 File Offset: 0x001E2083
		private void DoGetTouchCount()
		{
			this.storeCount.Value = Input.touchCount;
		}

		// Token: 0x04004DFC RID: 19964
		[UIHint(10)]
		[RequiredField]
		public FsmInt storeCount;

		// Token: 0x04004DFD RID: 19965
		public bool everyFrame;
	}
}
