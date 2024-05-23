using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C67 RID: 3175
	[ActionCategory("PlayerPrefs")]
	[Tooltip("Returns the value corresponding to key in the preference file if it exists.")]
	public class PlayerPrefsGetFloat : FsmStateAction
	{
		// Token: 0x06006718 RID: 26392 RVA: 0x001E6B9D File Offset: 0x001E4D9D
		public override void Reset()
		{
			this.keys = new FsmString[1];
			this.variables = new FsmFloat[1];
		}

		// Token: 0x06006719 RID: 26393 RVA: 0x001E6BB8 File Offset: 0x001E4DB8
		public override void OnEnter()
		{
			for (int i = 0; i < this.keys.Length; i++)
			{
				if (!this.keys[i].IsNone || !this.keys[i].Value.Equals(string.Empty))
				{
					this.variables[i].Value = PlayerPrefs.GetFloat(this.keys[i].Value, (!this.variables[i].IsNone) ? this.variables[i].Value : 0f);
				}
			}
			base.Finish();
		}

		// Token: 0x04004EE6 RID: 20198
		[Tooltip("Case sensitive key.")]
		[CompoundArray("Count", "Key", "Variable")]
		public FsmString[] keys;

		// Token: 0x04004EE7 RID: 20199
		[UIHint(10)]
		public FsmFloat[] variables;
	}
}
