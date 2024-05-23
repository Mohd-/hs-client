using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C6C RID: 3180
	[ActionCategory("PlayerPrefs")]
	[Tooltip("Sets the value of the preference identified by key.")]
	public class PlayerPrefsSetInt : FsmStateAction
	{
		// Token: 0x06006727 RID: 26407 RVA: 0x001E6F3D File Offset: 0x001E513D
		public override void Reset()
		{
			this.keys = new FsmString[1];
			this.values = new FsmInt[1];
		}

		// Token: 0x06006728 RID: 26408 RVA: 0x001E6F58 File Offset: 0x001E5158
		public override void OnEnter()
		{
			for (int i = 0; i < this.keys.Length; i++)
			{
				if (!this.keys[i].IsNone || !this.keys[i].Value.Equals(string.Empty))
				{
					PlayerPrefs.SetInt(this.keys[i].Value, (!this.values[i].IsNone) ? this.values[i].Value : 0);
				}
			}
			base.Finish();
		}

		// Token: 0x04004EF2 RID: 20210
		[Tooltip("Case sensitive key.")]
		[CompoundArray("Count", "Key", "Value")]
		public FsmString[] keys;

		// Token: 0x04004EF3 RID: 20211
		public FsmInt[] values;
	}
}
