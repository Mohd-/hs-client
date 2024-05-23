using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BBB RID: 3003
	[ActionCategory(5)]
	[Tooltip("GUI Box.")]
	public class GUIBox : GUIContentAction
	{
		// Token: 0x0600645F RID: 25695 RVA: 0x001DDBDC File Offset: 0x001DBDDC
		public override void OnGUI()
		{
			base.OnGUI();
			if (string.IsNullOrEmpty(this.style.Value))
			{
				GUI.Box(this.rect, this.content);
			}
			else
			{
				GUI.Box(this.rect, this.content, this.style.Value);
			}
		}
	}
}
