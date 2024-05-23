using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BCE RID: 3022
	[ActionCategory(26)]
	[Tooltip("Close a group started with BeginHorizontal.")]
	public class GUILayoutEndHorizontal : FsmStateAction
	{
		// Token: 0x06006499 RID: 25753 RVA: 0x001DEBB7 File Offset: 0x001DCDB7
		public override void Reset()
		{
		}

		// Token: 0x0600649A RID: 25754 RVA: 0x001DEBB9 File Offset: 0x001DCDB9
		public override void OnGUI()
		{
			GUILayout.EndHorizontal();
		}
	}
}
