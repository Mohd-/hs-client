using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BDA RID: 3034
	[Tooltip("Inserts a space in the current layout group.")]
	[ActionCategory(26)]
	public class GUILayoutSpace : FsmStateAction
	{
		// Token: 0x060064BC RID: 25788 RVA: 0x001DF34E File Offset: 0x001DD54E
		public override void Reset()
		{
			this.space = 10f;
		}

		// Token: 0x060064BD RID: 25789 RVA: 0x001DF360 File Offset: 0x001DD560
		public override void OnGUI()
		{
			GUILayout.Space(this.space.Value);
		}

		// Token: 0x04004C4B RID: 19531
		public FsmFloat space;
	}
}
