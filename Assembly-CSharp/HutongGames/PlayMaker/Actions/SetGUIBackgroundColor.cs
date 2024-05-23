using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CB8 RID: 3256
	[Tooltip("Sets the Tinting Color for all background elements rendered by the GUI. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
	[ActionCategory(5)]
	public class SetGUIBackgroundColor : FsmStateAction
	{
		// Token: 0x0600687F RID: 26751 RVA: 0x001EB8A7 File Offset: 0x001E9AA7
		public override void Reset()
		{
			this.backgroundColor = Color.white;
		}

		// Token: 0x06006880 RID: 26752 RVA: 0x001EB8B9 File Offset: 0x001E9AB9
		public override void OnGUI()
		{
			GUI.backgroundColor = this.backgroundColor.Value;
			if (this.applyGlobally.Value)
			{
				PlayMakerGUI.GUIBackgroundColor = GUI.backgroundColor;
			}
		}

		// Token: 0x0400505E RID: 20574
		[RequiredField]
		public FsmColor backgroundColor;

		// Token: 0x0400505F RID: 20575
		public FsmBool applyGlobally;
	}
}
