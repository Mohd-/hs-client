﻿using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C69 RID: 3177
	[Tooltip("Returns the value corresponding to key in the preference file if it exists.")]
	[ActionCategory("PlayerPrefs")]
	public class PlayerPrefsGetString : FsmStateAction
	{
		// Token: 0x0600671E RID: 26398 RVA: 0x001E6D22 File Offset: 0x001E4F22
		public override void Reset()
		{
			this.keys = new FsmString[1];
			this.variables = new FsmString[1];
		}

		// Token: 0x0600671F RID: 26399 RVA: 0x001E6D3C File Offset: 0x001E4F3C
		public override void OnEnter()
		{
			for (int i = 0; i < this.keys.Length; i++)
			{
				if (!this.keys[i].IsNone || !this.keys[i].Value.Equals(string.Empty))
				{
					this.variables[i].Value = PlayerPrefs.GetString(this.keys[i].Value, (!this.variables[i].IsNone) ? this.variables[i].Value : string.Empty);
				}
			}
			base.Finish();
		}

		// Token: 0x04004EEA RID: 20202
		[CompoundArray("Count", "Key", "Variable")]
		[Tooltip("Case sensitive key.")]
		public FsmString[] keys;

		// Token: 0x04004EEB RID: 20203
		[UIHint(10)]
		public FsmString[] variables;
	}
}
