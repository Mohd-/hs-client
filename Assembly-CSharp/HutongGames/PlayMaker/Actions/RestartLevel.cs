using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C7D RID: 3197
	[ActionCategory(25)]
	[Tooltip("Restarts current level.")]
	public class RestartLevel : FsmStateAction
	{
		// Token: 0x0600676A RID: 26474 RVA: 0x001E7EAE File Offset: 0x001E60AE
		public override void OnEnter()
		{
			Application.LoadLevel(Application.loadedLevelName);
			base.Finish();
		}
	}
}
