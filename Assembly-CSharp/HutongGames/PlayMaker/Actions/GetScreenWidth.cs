using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C2D RID: 3117
	[ActionCategory(41)]
	[Tooltip("Gets the Width of the Screen in pixels.")]
	public class GetScreenWidth : FsmStateAction
	{
		// Token: 0x06006616 RID: 26134 RVA: 0x001E383B File Offset: 0x001E1A3B
		public override void Reset()
		{
			this.storeScreenWidth = null;
		}

		// Token: 0x06006617 RID: 26135 RVA: 0x001E3844 File Offset: 0x001E1A44
		public override void OnEnter()
		{
			this.storeScreenWidth.Value = (float)Screen.width;
			base.Finish();
		}

		// Token: 0x04004DD4 RID: 19924
		[RequiredField]
		[UIHint(10)]
		public FsmFloat storeScreenWidth;
	}
}
