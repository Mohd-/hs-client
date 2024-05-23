using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C7B RID: 3195
	[ActionCategory(5)]
	[Tooltip("Resets the GUI matrix. Useful if you've rotated or scaled the GUI and now want to reset it.")]
	public class ResetGUIMatrix : FsmStateAction
	{
		// Token: 0x06006765 RID: 26469 RVA: 0x001E7E70 File Offset: 0x001E6070
		public override void OnGUI()
		{
			Matrix4x4 identity = Matrix4x4.identity;
			GUI.matrix = identity;
			PlayMakerGUI.GUIMatrix = identity;
		}
	}
}
