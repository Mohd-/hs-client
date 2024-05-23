using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C7C RID: 3196
	[Tooltip("Resets all Input. After ResetInputAxes all axes return to 0 and all buttons return to 0 for one frame")]
	[ActionCategory(6)]
	public class ResetInputAxes : FsmStateAction
	{
		// Token: 0x06006767 RID: 26471 RVA: 0x001E7E97 File Offset: 0x001E6097
		public override void Reset()
		{
		}

		// Token: 0x06006768 RID: 26472 RVA: 0x001E7E99 File Offset: 0x001E6099
		public override void OnEnter()
		{
			Input.ResetInputAxes();
			base.Finish();
		}
	}
}
