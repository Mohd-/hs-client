using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CB9 RID: 3257
	[Tooltip("Sets the Tinting Color for the GUI. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
	[ActionCategory(5)]
	public class SetGUIColor : FsmStateAction
	{
		// Token: 0x06006882 RID: 26754 RVA: 0x001EB8ED File Offset: 0x001E9AED
		public override void Reset()
		{
			this.color = Color.white;
		}

		// Token: 0x06006883 RID: 26755 RVA: 0x001EB8FF File Offset: 0x001E9AFF
		public override void OnGUI()
		{
			GUI.color = this.color.Value;
			if (this.applyGlobally.Value)
			{
				PlayMakerGUI.GUIColor = GUI.color;
			}
		}

		// Token: 0x04005060 RID: 20576
		[RequiredField]
		public FsmColor color;

		// Token: 0x04005061 RID: 20577
		public FsmBool applyGlobally;
	}
}
