using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BC4 RID: 3012
	[Tooltip("Begin a centered GUILayout block. The block is centered inside a parent GUILayout Area. So to place the block in the center of the screen, first use a GULayout Area the size of the whole screen (the default setting). NOTE: Block must end with a corresponding GUILayoutEndCentered.")]
	[ActionCategory(26)]
	public class GUILayoutBeginCentered : FsmStateAction
	{
		// Token: 0x0600647B RID: 25723 RVA: 0x001DE567 File Offset: 0x001DC767
		public override void Reset()
		{
		}

		// Token: 0x0600647C RID: 25724 RVA: 0x001DE569 File Offset: 0x001DC769
		public override void OnGUI()
		{
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
		}
	}
}
