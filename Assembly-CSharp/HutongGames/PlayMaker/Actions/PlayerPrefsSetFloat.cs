using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C6B RID: 3179
	[ActionCategory("PlayerPrefs")]
	[Tooltip("Sets the value of the preference identified by key.")]
	public class PlayerPrefsSetFloat : FsmStateAction
	{
		// Token: 0x06006724 RID: 26404 RVA: 0x001E6E84 File Offset: 0x001E5084
		public override void Reset()
		{
			this.keys = new FsmString[1];
			this.values = new FsmFloat[1];
		}

		// Token: 0x06006725 RID: 26405 RVA: 0x001E6EA0 File Offset: 0x001E50A0
		public override void OnEnter()
		{
			for (int i = 0; i < this.keys.Length; i++)
			{
				if (!this.keys[i].IsNone || !this.keys[i].Value.Equals(string.Empty))
				{
					PlayerPrefs.SetFloat(this.keys[i].Value, (!this.values[i].IsNone) ? this.values[i].Value : 0f);
				}
			}
			base.Finish();
		}

		// Token: 0x04004EF0 RID: 20208
		[Tooltip("Case sensitive key.")]
		[CompoundArray("Count", "Key", "Value")]
		public FsmString[] keys;

		// Token: 0x04004EF1 RID: 20209
		public FsmFloat[] values;
	}
}
