using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C65 RID: 3173
	[ActionCategory("PlayerPrefs")]
	[Tooltip("Removes all keys and values from the preferences. Use with caution.")]
	public class PlayerPrefsDeleteAll : FsmStateAction
	{
		// Token: 0x06006712 RID: 26386 RVA: 0x001E6B1E File Offset: 0x001E4D1E
		public override void Reset()
		{
		}

		// Token: 0x06006713 RID: 26387 RVA: 0x001E6B20 File Offset: 0x001E4D20
		public override void OnEnter()
		{
			PlayerPrefs.DeleteAll();
			base.Finish();
		}
	}
}
