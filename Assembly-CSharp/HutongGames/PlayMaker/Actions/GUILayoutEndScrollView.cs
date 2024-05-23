using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BCF RID: 3023
	[Tooltip("Close a group started with GUILayout Begin ScrollView.")]
	[ActionCategory(26)]
	public class GUILayoutEndScrollView : FsmStateAction
	{
		// Token: 0x0600649C RID: 25756 RVA: 0x001DEBC8 File Offset: 0x001DCDC8
		public override void OnGUI()
		{
			GUILayout.EndScrollView();
		}
	}
}
