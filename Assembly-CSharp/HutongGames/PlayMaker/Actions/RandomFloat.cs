using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C76 RID: 3190
	[Tooltip("Sets a Float Variable to a random value between Min/Max.")]
	[ActionCategory(7)]
	public class RandomFloat : FsmStateAction
	{
		// Token: 0x06006751 RID: 26449 RVA: 0x001E788D File Offset: 0x001E5A8D
		public override void Reset()
		{
			this.min = 0f;
			this.max = 1f;
			this.storeResult = null;
		}

		// Token: 0x06006752 RID: 26450 RVA: 0x001E78B8 File Offset: 0x001E5AB8
		public override void OnEnter()
		{
			this.storeResult.Value = Random.Range(this.min.Value, this.max.Value);
			base.Finish();
		}

		// Token: 0x04004F1E RID: 20254
		[RequiredField]
		public FsmFloat min;

		// Token: 0x04004F1F RID: 20255
		[RequiredField]
		public FsmFloat max;

		// Token: 0x04004F20 RID: 20256
		[UIHint(10)]
		[RequiredField]
		public FsmFloat storeResult;
	}
}
