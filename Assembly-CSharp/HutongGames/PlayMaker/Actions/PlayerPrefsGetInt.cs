using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C68 RID: 3176
	[Tooltip("Returns the value corresponding to key in the preference file if it exists.")]
	[ActionCategory("PlayerPrefs")]
	public class PlayerPrefsGetInt : FsmStateAction
	{
		// Token: 0x0600671B RID: 26395 RVA: 0x001E6C62 File Offset: 0x001E4E62
		public override void Reset()
		{
			this.keys = new FsmString[1];
			this.variables = new FsmInt[1];
		}

		// Token: 0x0600671C RID: 26396 RVA: 0x001E6C7C File Offset: 0x001E4E7C
		public override void OnEnter()
		{
			for (int i = 0; i < this.keys.Length; i++)
			{
				if (!this.keys[i].IsNone || !this.keys[i].Value.Equals(string.Empty))
				{
					this.variables[i].Value = PlayerPrefs.GetInt(this.keys[i].Value, (!this.variables[i].IsNone) ? this.variables[i].Value : 0);
				}
			}
			base.Finish();
		}

		// Token: 0x04004EE8 RID: 20200
		[Tooltip("Case sensitive key.")]
		[CompoundArray("Count", "Key", "Variable")]
		public FsmString[] keys;

		// Token: 0x04004EE9 RID: 20201
		[UIHint(10)]
		public FsmInt[] variables;
	}
}
