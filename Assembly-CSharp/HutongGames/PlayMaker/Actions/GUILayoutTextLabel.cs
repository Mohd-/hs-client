using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BDC RID: 3036
	[ActionCategory(26)]
	[Tooltip("GUILayout Label for simple text.")]
	public class GUILayoutTextLabel : GUILayoutAction
	{
		// Token: 0x060064C2 RID: 25794 RVA: 0x001DF438 File Offset: 0x001DD638
		public override void Reset()
		{
			base.Reset();
			this.text = string.Empty;
			this.style = string.Empty;
		}

		// Token: 0x060064C3 RID: 25795 RVA: 0x001DF46C File Offset: 0x001DD66C
		public override void OnGUI()
		{
			if (string.IsNullOrEmpty(this.style.Value))
			{
				GUILayout.Label(new GUIContent(this.text.Value), base.LayoutOptions);
			}
			else
			{
				GUILayout.Label(new GUIContent(this.text.Value), this.style.Value, base.LayoutOptions);
			}
		}

		// Token: 0x04004C50 RID: 19536
		[Tooltip("Text to display.")]
		public FsmString text;

		// Token: 0x04004C51 RID: 19537
		[Tooltip("Optional GUIStyle in the active GUISkin.")]
		public FsmString style;
	}
}
