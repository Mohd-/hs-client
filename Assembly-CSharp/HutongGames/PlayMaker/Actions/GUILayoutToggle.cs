using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BDD RID: 3037
	[ActionCategory(26)]
	[Tooltip("Makes an on/off Toggle Button and stores the button state in a Bool Variable.")]
	public class GUILayoutToggle : GUILayoutAction
	{
		// Token: 0x060064C5 RID: 25797 RVA: 0x001DF4E4 File Offset: 0x001DD6E4
		public override void Reset()
		{
			base.Reset();
			this.storeButtonState = null;
			this.text = string.Empty;
			this.image = null;
			this.tooltip = string.Empty;
			this.style = "Toggle";
			this.changedEvent = null;
		}

		// Token: 0x060064C6 RID: 25798 RVA: 0x001DF53C File Offset: 0x001DD73C
		public override void OnGUI()
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			this.storeButtonState.Value = GUILayout.Toggle(this.storeButtonState.Value, new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), this.style.Value, base.LayoutOptions);
			if (GUI.changed)
			{
				base.Fsm.Event(this.changedEvent);
				GUIUtility.ExitGUI();
			}
			else
			{
				GUI.changed = changed;
			}
		}

		// Token: 0x04004C52 RID: 19538
		[RequiredField]
		[UIHint(10)]
		public FsmBool storeButtonState;

		// Token: 0x04004C53 RID: 19539
		public FsmTexture image;

		// Token: 0x04004C54 RID: 19540
		public FsmString text;

		// Token: 0x04004C55 RID: 19541
		public FsmString tooltip;

		// Token: 0x04004C56 RID: 19542
		public FsmString style;

		// Token: 0x04004C57 RID: 19543
		public FsmEvent changedEvent;
	}
}
