using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C74 RID: 3188
	[Tooltip("Sets a Bool Variable to True or False randomly.")]
	[ActionCategory(7)]
	public class RandomBool : FsmStateAction
	{
		// Token: 0x06006749 RID: 26441 RVA: 0x001E7715 File Offset: 0x001E5915
		public override void Reset()
		{
			this.storeResult = null;
		}

		// Token: 0x0600674A RID: 26442 RVA: 0x001E771E File Offset: 0x001E591E
		public override void OnEnter()
		{
			this.storeResult.Value = (Random.Range(0, 100) < 50);
			base.Finish();
		}

		// Token: 0x04004F18 RID: 20248
		[UIHint(10)]
		public FsmBool storeResult;
	}
}
