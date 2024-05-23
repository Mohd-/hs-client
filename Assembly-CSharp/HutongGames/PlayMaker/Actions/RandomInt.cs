using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C77 RID: 3191
	[Tooltip("Sets an Integer Variable to a random value between Min/Max.")]
	[ActionCategory(7)]
	public class RandomInt : FsmStateAction
	{
		// Token: 0x06006754 RID: 26452 RVA: 0x001E78F9 File Offset: 0x001E5AF9
		public override void Reset()
		{
			this.min = 0;
			this.max = 100;
			this.storeResult = null;
			this.inclusiveMax = false;
		}

		// Token: 0x06006755 RID: 26453 RVA: 0x001E7924 File Offset: 0x001E5B24
		public override void OnEnter()
		{
			this.storeResult.Value = ((!this.inclusiveMax) ? Random.Range(this.min.Value, this.max.Value) : Random.Range(this.min.Value, this.max.Value + 1));
			base.Finish();
		}

		// Token: 0x04004F21 RID: 20257
		[RequiredField]
		public FsmInt min;

		// Token: 0x04004F22 RID: 20258
		[RequiredField]
		public FsmInt max;

		// Token: 0x04004F23 RID: 20259
		[RequiredField]
		[UIHint(10)]
		public FsmInt storeResult;

		// Token: 0x04004F24 RID: 20260
		[Tooltip("Should the Max value be included in the possible results?")]
		public bool inclusiveMax;
	}
}
