using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BC5 RID: 3013
	[ActionCategory(26)]
	[Tooltip("GUILayout BeginHorizontal.")]
	public class GUILayoutBeginHorizontal : GUILayoutAction
	{
		// Token: 0x0600647E RID: 25726 RVA: 0x001DE59E File Offset: 0x001DC79E
		public override void Reset()
		{
			base.Reset();
			this.text = string.Empty;
			this.image = null;
			this.tooltip = string.Empty;
			this.style = string.Empty;
		}

		// Token: 0x0600647F RID: 25727 RVA: 0x001DE5E0 File Offset: 0x001DC7E0
		public override void OnGUI()
		{
			GUILayout.BeginHorizontal(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), this.style.Value, base.LayoutOptions);
		}

		// Token: 0x04004C07 RID: 19463
		public FsmTexture image;

		// Token: 0x04004C08 RID: 19464
		public FsmString text;

		// Token: 0x04004C09 RID: 19465
		public FsmString tooltip;

		// Token: 0x04004C0A RID: 19466
		public FsmString style;
	}
}
