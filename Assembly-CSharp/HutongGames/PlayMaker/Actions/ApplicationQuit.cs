using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B55 RID: 2901
	[ActionCategory(41)]
	[Tooltip("Quits the player application.")]
	public class ApplicationQuit : FsmStateAction
	{
		// Token: 0x060062BF RID: 25279 RVA: 0x001D8779 File Offset: 0x001D6979
		public override void Reset()
		{
		}

		// Token: 0x060062C0 RID: 25280 RVA: 0x001D877B File Offset: 0x001D697B
		public override void OnEnter()
		{
			Application.Quit();
			base.Finish();
		}
	}
}
