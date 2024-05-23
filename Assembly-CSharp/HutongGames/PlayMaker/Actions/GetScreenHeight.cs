using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C2C RID: 3116
	[ActionCategory(41)]
	[Tooltip("Gets the Height of the Screen in pixels.")]
	public class GetScreenHeight : FsmStateAction
	{
		// Token: 0x06006613 RID: 26131 RVA: 0x001E3811 File Offset: 0x001E1A11
		public override void Reset()
		{
			this.storeScreenHeight = null;
		}

		// Token: 0x06006614 RID: 26132 RVA: 0x001E381A File Offset: 0x001E1A1A
		public override void OnEnter()
		{
			this.storeScreenHeight.Value = (float)Screen.height;
			base.Finish();
		}

		// Token: 0x04004DD3 RID: 19923
		[RequiredField]
		[UIHint(10)]
		public FsmFloat storeScreenHeight;
	}
}
