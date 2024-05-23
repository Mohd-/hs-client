using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BD7 RID: 3031
	[Tooltip("GUILayout Label.")]
	[ActionCategory(26)]
	public class GUILayoutLabel : GUILayoutAction
	{
		// Token: 0x060064B3 RID: 25779 RVA: 0x001DF07F File Offset: 0x001DD27F
		public override void Reset()
		{
			base.Reset();
			this.text = string.Empty;
			this.image = null;
			this.tooltip = string.Empty;
			this.style = string.Empty;
		}

		// Token: 0x060064B4 RID: 25780 RVA: 0x001DF0C0 File Offset: 0x001DD2C0
		public override void OnGUI()
		{
			if (string.IsNullOrEmpty(this.style.Value))
			{
				GUILayout.Label(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), base.LayoutOptions);
			}
			else
			{
				GUILayout.Label(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), this.style.Value, base.LayoutOptions);
			}
		}

		// Token: 0x04004C3C RID: 19516
		public FsmTexture image;

		// Token: 0x04004C3D RID: 19517
		public FsmString text;

		// Token: 0x04004C3E RID: 19518
		public FsmString tooltip;

		// Token: 0x04004C3F RID: 19519
		public FsmString style;
	}
}
