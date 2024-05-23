using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BD3 RID: 3027
	[ActionCategory(26)]
	[Tooltip("GUILayout Label for a Float Variable.")]
	public class GUILayoutFloatLabel : GUILayoutAction
	{
		// Token: 0x060064A7 RID: 25767 RVA: 0x001DED02 File Offset: 0x001DCF02
		public override void Reset()
		{
			base.Reset();
			this.prefix = string.Empty;
			this.style = string.Empty;
			this.floatVariable = null;
		}

		// Token: 0x060064A8 RID: 25768 RVA: 0x001DED34 File Offset: 0x001DCF34
		public override void OnGUI()
		{
			if (string.IsNullOrEmpty(this.style.Value))
			{
				GUILayout.Label(new GUIContent(this.prefix.Value + this.floatVariable.Value), base.LayoutOptions);
			}
			else
			{
				GUILayout.Label(new GUIContent(this.prefix.Value + this.floatVariable.Value), this.style.Value, base.LayoutOptions);
			}
		}

		// Token: 0x04004C2F RID: 19503
		[Tooltip("Text to put before the float variable.")]
		public FsmString prefix;

		// Token: 0x04004C30 RID: 19504
		[RequiredField]
		[UIHint(10)]
		[Tooltip("Float variable to display.")]
		public FsmFloat floatVariable;

		// Token: 0x04004C31 RID: 19505
		[Tooltip("Optional GUIStyle in the active GUISKin.")]
		public FsmString style;
	}
}
