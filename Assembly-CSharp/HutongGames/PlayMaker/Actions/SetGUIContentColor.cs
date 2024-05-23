using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CBA RID: 3258
	[Tooltip("Sets the Tinting Color for all text rendered by the GUI. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
	[ActionCategory(5)]
	public class SetGUIContentColor : FsmStateAction
	{
		// Token: 0x06006885 RID: 26757 RVA: 0x001EB933 File Offset: 0x001E9B33
		public override void Reset()
		{
			this.contentColor = Color.white;
		}

		// Token: 0x06006886 RID: 26758 RVA: 0x001EB945 File Offset: 0x001E9B45
		public override void OnGUI()
		{
			GUI.contentColor = this.contentColor.Value;
			if (this.applyGlobally.Value)
			{
				PlayMakerGUI.GUIContentColor = GUI.contentColor;
			}
		}

		// Token: 0x04005062 RID: 20578
		[RequiredField]
		public FsmColor contentColor;

		// Token: 0x04005063 RID: 20579
		public FsmBool applyGlobally;
	}
}
