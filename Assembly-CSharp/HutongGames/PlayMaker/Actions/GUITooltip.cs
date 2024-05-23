using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BE0 RID: 3040
	[ActionCategory(5)]
	[Tooltip("Gets the Tooltip of the control the mouse is currently over and store it in a String Variable.")]
	public class GUITooltip : FsmStateAction
	{
		// Token: 0x060064D1 RID: 25809 RVA: 0x001DF986 File Offset: 0x001DDB86
		public override void Reset()
		{
			this.storeTooltip = null;
		}

		// Token: 0x060064D2 RID: 25810 RVA: 0x001DF98F File Offset: 0x001DDB8F
		public override void OnGUI()
		{
			this.storeTooltip.Value = GUI.tooltip;
		}

		// Token: 0x04004C64 RID: 19556
		[UIHint(10)]
		public FsmString storeTooltip;
	}
}
