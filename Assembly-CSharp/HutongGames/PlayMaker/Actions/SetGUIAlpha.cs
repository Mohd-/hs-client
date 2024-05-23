using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CB7 RID: 3255
	[ActionCategory(5)]
	[Tooltip("Sets the global Alpha for the GUI. Useful for fading GUI up/down. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
	public class SetGUIAlpha : FsmStateAction
	{
		// Token: 0x0600687C RID: 26748 RVA: 0x001EB827 File Offset: 0x001E9A27
		public override void Reset()
		{
			this.alpha = 1f;
		}

		// Token: 0x0600687D RID: 26749 RVA: 0x001EB83C File Offset: 0x001E9A3C
		public override void OnGUI()
		{
			GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, this.alpha.Value);
			if (this.applyGlobally.Value)
			{
				PlayMakerGUI.GUIColor = GUI.color;
			}
		}

		// Token: 0x0400505C RID: 20572
		[RequiredField]
		public FsmFloat alpha;

		// Token: 0x0400505D RID: 20573
		public FsmBool applyGlobally;
	}
}
