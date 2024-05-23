using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C6D RID: 3181
	[ActionCategory("PlayerPrefs")]
	[Tooltip("Sets the value of the preference identified by key.")]
	public class PlayerPrefsSetString : FsmStateAction
	{
		// Token: 0x0600672A RID: 26410 RVA: 0x001E6FF1 File Offset: 0x001E51F1
		public override void Reset()
		{
			this.keys = new FsmString[1];
			this.values = new FsmString[1];
		}

		// Token: 0x0600672B RID: 26411 RVA: 0x001E700C File Offset: 0x001E520C
		public override void OnEnter()
		{
			for (int i = 0; i < this.keys.Length; i++)
			{
				if (!this.keys[i].IsNone || !this.keys[i].Value.Equals(string.Empty))
				{
					PlayerPrefs.SetString(this.keys[i].Value, (!this.values[i].IsNone) ? this.values[i].Value : string.Empty);
				}
			}
			base.Finish();
		}

		// Token: 0x04004EF4 RID: 20212
		[Tooltip("Case sensitive key.")]
		[CompoundArray("Count", "Key", "Value")]
		public FsmString[] keys;

		// Token: 0x04004EF5 RID: 20213
		public FsmString[] values;
	}
}
