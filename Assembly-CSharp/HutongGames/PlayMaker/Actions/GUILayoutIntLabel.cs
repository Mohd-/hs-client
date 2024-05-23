using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BD6 RID: 3030
	[Tooltip("GUILayout Label for an Int Variable.")]
	[ActionCategory(26)]
	public class GUILayoutIntLabel : GUILayoutAction
	{
		// Token: 0x060064B0 RID: 25776 RVA: 0x001DEFAE File Offset: 0x001DD1AE
		public override void Reset()
		{
			base.Reset();
			this.prefix = string.Empty;
			this.style = string.Empty;
			this.intVariable = null;
		}

		// Token: 0x060064B1 RID: 25777 RVA: 0x001DEFE0 File Offset: 0x001DD1E0
		public override void OnGUI()
		{
			if (string.IsNullOrEmpty(this.style.Value))
			{
				GUILayout.Label(new GUIContent(this.prefix.Value + this.intVariable.Value), base.LayoutOptions);
			}
			else
			{
				GUILayout.Label(new GUIContent(this.prefix.Value + this.intVariable.Value), this.style.Value, base.LayoutOptions);
			}
		}

		// Token: 0x04004C39 RID: 19513
		[Tooltip("Text to put before the int variable.")]
		public FsmString prefix;

		// Token: 0x04004C3A RID: 19514
		[RequiredField]
		[Tooltip("Int variable to display.")]
		[UIHint(10)]
		public FsmInt intVariable;

		// Token: 0x04004C3B RID: 19515
		[Tooltip("Optional GUIStyle in the active GUISKin.")]
		public FsmString style;
	}
}
