using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BC0 RID: 3008
	[ActionCategory(5)]
	[Tooltip("GUI Label.")]
	public class GUILabel : GUIContentAction
	{
		// Token: 0x0600646F RID: 25711 RVA: 0x001DE0A0 File Offset: 0x001DC2A0
		public override void OnGUI()
		{
			base.OnGUI();
			if (string.IsNullOrEmpty(this.style.Value))
			{
				GUI.Label(this.rect, this.content);
			}
			else
			{
				GUI.Label(this.rect, this.content, this.style.Value);
			}
		}
	}
}
