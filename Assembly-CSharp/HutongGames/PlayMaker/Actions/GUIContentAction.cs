using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BBC RID: 3004
	[Tooltip("GUI base action - don't use!")]
	public abstract class GUIContentAction : GUIAction
	{
		// Token: 0x06006461 RID: 25697 RVA: 0x001DDC43 File Offset: 0x001DBE43
		public override void Reset()
		{
			base.Reset();
			this.image = null;
			this.text = string.Empty;
			this.tooltip = string.Empty;
			this.style = string.Empty;
		}

		// Token: 0x06006462 RID: 25698 RVA: 0x001DDC84 File Offset: 0x001DBE84
		public override void OnGUI()
		{
			base.OnGUI();
			this.content = new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value);
		}

		// Token: 0x04004BDF RID: 19423
		public FsmTexture image;

		// Token: 0x04004BE0 RID: 19424
		public FsmString text;

		// Token: 0x04004BE1 RID: 19425
		public FsmString tooltip;

		// Token: 0x04004BE2 RID: 19426
		public FsmString style;

		// Token: 0x04004BE3 RID: 19427
		internal GUIContent content;
	}
}
