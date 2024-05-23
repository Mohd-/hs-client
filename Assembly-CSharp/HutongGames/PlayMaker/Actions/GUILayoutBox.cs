using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BC8 RID: 3016
	[ActionCategory(26)]
	[Tooltip("GUILayout Box.")]
	public class GUILayoutBox : GUILayoutAction
	{
		// Token: 0x06006487 RID: 25735 RVA: 0x001DE7CE File Offset: 0x001DC9CE
		public override void Reset()
		{
			base.Reset();
			this.text = string.Empty;
			this.image = null;
			this.tooltip = string.Empty;
			this.style = string.Empty;
		}

		// Token: 0x06006488 RID: 25736 RVA: 0x001DE810 File Offset: 0x001DCA10
		public override void OnGUI()
		{
			if (string.IsNullOrEmpty(this.style.Value))
			{
				GUILayout.Box(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), base.LayoutOptions);
			}
			else
			{
				GUILayout.Box(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), this.style.Value, base.LayoutOptions);
			}
		}

		// Token: 0x04004C16 RID: 19478
		[Tooltip("Image to display in the Box.")]
		public FsmTexture image;

		// Token: 0x04004C17 RID: 19479
		[Tooltip("Text to display in the Box.")]
		public FsmString text;

		// Token: 0x04004C18 RID: 19480
		[Tooltip("Optional Tooltip string.")]
		public FsmString tooltip;

		// Token: 0x04004C19 RID: 19481
		[Tooltip("Optional GUIStyle in the active GUISkin.")]
		public FsmString style;
	}
}
