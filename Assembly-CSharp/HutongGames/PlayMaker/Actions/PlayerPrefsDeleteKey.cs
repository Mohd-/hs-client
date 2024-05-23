using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C66 RID: 3174
	[Tooltip("Removes key and its corresponding value from the preferences.")]
	[ActionCategory("PlayerPrefs")]
	public class PlayerPrefsDeleteKey : FsmStateAction
	{
		// Token: 0x06006715 RID: 26389 RVA: 0x001E6B35 File Offset: 0x001E4D35
		public override void Reset()
		{
			this.key = string.Empty;
		}

		// Token: 0x06006716 RID: 26390 RVA: 0x001E6B48 File Offset: 0x001E4D48
		public override void OnEnter()
		{
			if (!this.key.IsNone && !this.key.Value.Equals(string.Empty))
			{
				PlayerPrefs.DeleteKey(this.key.Value);
			}
			base.Finish();
		}

		// Token: 0x04004EE5 RID: 20197
		public FsmString key;
	}
}
