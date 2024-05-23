using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BCD RID: 3021
	[Tooltip("End a centered GUILayout block started with GUILayoutBeginCentered.")]
	[ActionCategory(26)]
	public class GUILayoutEndCentered : FsmStateAction
	{
		// Token: 0x06006496 RID: 25750 RVA: 0x001DEB92 File Offset: 0x001DCD92
		public override void Reset()
		{
		}

		// Token: 0x06006497 RID: 25751 RVA: 0x001DEB94 File Offset: 0x001DCD94
		public override void OnGUI()
		{
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
		}
	}
}
