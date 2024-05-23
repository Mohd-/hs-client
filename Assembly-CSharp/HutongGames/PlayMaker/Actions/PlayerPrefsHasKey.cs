using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C6A RID: 3178
	[Tooltip("Returns true if key exists in the preferences.")]
	[ActionCategory("PlayerPrefs")]
	public class PlayerPrefsHasKey : FsmStateAction
	{
		// Token: 0x06006721 RID: 26401 RVA: 0x001E6DE6 File Offset: 0x001E4FE6
		public override void Reset()
		{
			this.key = string.Empty;
		}

		// Token: 0x06006722 RID: 26402 RVA: 0x001E6DF8 File Offset: 0x001E4FF8
		public override void OnEnter()
		{
			base.Finish();
			if (!this.key.IsNone && !this.key.Value.Equals(string.Empty))
			{
				this.variable.Value = PlayerPrefs.HasKey(this.key.Value);
			}
			base.Fsm.Event((!this.variable.Value) ? this.falseEvent : this.trueEvent);
		}

		// Token: 0x04004EEC RID: 20204
		[RequiredField]
		public FsmString key;

		// Token: 0x04004EED RID: 20205
		[Title("Store Result")]
		[UIHint(10)]
		public FsmBool variable;

		// Token: 0x04004EEE RID: 20206
		[Tooltip("Event to send if key exists.")]
		public FsmEvent trueEvent;

		// Token: 0x04004EEF RID: 20207
		[Tooltip("Event to send if key does not exist.")]
		public FsmEvent falseEvent;
	}
}
