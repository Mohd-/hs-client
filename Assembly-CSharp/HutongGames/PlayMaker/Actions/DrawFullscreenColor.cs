using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B98 RID: 2968
	[Tooltip("Fills the screen with a Color. NOTE: Uses OnGUI so you need a PlayMakerGUI component in the scene.")]
	[ActionCategory(5)]
	public class DrawFullscreenColor : FsmStateAction
	{
		// Token: 0x060063CB RID: 25547 RVA: 0x001DBDBF File Offset: 0x001D9FBF
		public override void Reset()
		{
			this.color = Color.white;
		}

		// Token: 0x060063CC RID: 25548 RVA: 0x001DBDD4 File Offset: 0x001D9FD4
		public override void OnGUI()
		{
			Color color = GUI.color;
			GUI.color = this.color.Value;
			GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), ActionHelpers.WhiteTexture);
			GUI.color = color;
		}

		// Token: 0x04004B3C RID: 19260
		[RequiredField]
		[Tooltip("Color. NOTE: Uses OnGUI so you need a PlayMakerGUI component in the scene.")]
		public FsmColor color;
	}
}
