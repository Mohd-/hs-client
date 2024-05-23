using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BD0 RID: 3024
	[Tooltip("Close a group started with BeginVertical.")]
	[ActionCategory(26)]
	public class GUILayoutEndVertical : FsmStateAction
	{
		// Token: 0x0600649E RID: 25758 RVA: 0x001DEBD7 File Offset: 0x001DCDD7
		public override void Reset()
		{
		}

		// Token: 0x0600649F RID: 25759 RVA: 0x001DEBD9 File Offset: 0x001DCDD9
		public override void OnGUI()
		{
			GUILayout.EndVertical();
		}
	}
}
