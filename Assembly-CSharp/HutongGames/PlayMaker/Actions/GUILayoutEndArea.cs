using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BCC RID: 3020
	[ActionCategory(26)]
	[Tooltip("Close a GUILayout group started with BeginArea.")]
	public class GUILayoutEndArea : FsmStateAction
	{
		// Token: 0x06006493 RID: 25747 RVA: 0x001DEB81 File Offset: 0x001DCD81
		public override void Reset()
		{
		}

		// Token: 0x06006494 RID: 25748 RVA: 0x001DEB83 File Offset: 0x001DCD83
		public override void OnGUI()
		{
			GUILayout.EndArea();
		}
	}
}
