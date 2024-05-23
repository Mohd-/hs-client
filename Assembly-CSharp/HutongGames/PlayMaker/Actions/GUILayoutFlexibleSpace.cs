using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BD1 RID: 3025
	[Tooltip("Inserts a flexible space element.")]
	[ActionCategory(26)]
	public class GUILayoutFlexibleSpace : FsmStateAction
	{
		// Token: 0x060064A1 RID: 25761 RVA: 0x001DEBE8 File Offset: 0x001DCDE8
		public override void Reset()
		{
		}

		// Token: 0x060064A2 RID: 25762 RVA: 0x001DEBEA File Offset: 0x001DCDEA
		public override void OnGUI()
		{
			GUILayout.FlexibleSpace();
		}
	}
}
