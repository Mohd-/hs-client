using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BC7 RID: 3015
	[ActionCategory(26)]
	[Tooltip("Begins a vertical control group. The group must be closed with GUILayoutEndVertical action.")]
	public class GUILayoutBeginVertical : GUILayoutAction
	{
		// Token: 0x06006484 RID: 25732 RVA: 0x001DE738 File Offset: 0x001DC938
		public override void Reset()
		{
			base.Reset();
			this.text = string.Empty;
			this.image = null;
			this.tooltip = string.Empty;
			this.style = string.Empty;
		}

		// Token: 0x06006485 RID: 25733 RVA: 0x001DE778 File Offset: 0x001DC978
		public override void OnGUI()
		{
			GUILayout.BeginVertical(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), this.style.Value, base.LayoutOptions);
		}

		// Token: 0x04004C12 RID: 19474
		public FsmTexture image;

		// Token: 0x04004C13 RID: 19475
		public FsmString text;

		// Token: 0x04004C14 RID: 19476
		public FsmString tooltip;

		// Token: 0x04004C15 RID: 19477
		public FsmString style;
	}
}
