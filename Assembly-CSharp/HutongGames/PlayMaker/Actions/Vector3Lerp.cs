using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D12 RID: 3346
	[Tooltip("Linearly interpolates between 2 vectors.")]
	[ActionCategory(19)]
	public class Vector3Lerp : FsmStateAction
	{
		// Token: 0x06006A02 RID: 27138 RVA: 0x001F0AC8 File Offset: 0x001EECC8
		public override void Reset()
		{
			this.fromVector = new FsmVector3
			{
				UseVariable = true
			};
			this.toVector = new FsmVector3
			{
				UseVariable = true
			};
			this.storeResult = null;
			this.everyFrame = true;
		}

		// Token: 0x06006A03 RID: 27139 RVA: 0x001F0B0B File Offset: 0x001EED0B
		public override void OnEnter()
		{
			this.DoVector3Lerp();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006A04 RID: 27140 RVA: 0x001F0B24 File Offset: 0x001EED24
		public override void OnUpdate()
		{
			this.DoVector3Lerp();
		}

		// Token: 0x06006A05 RID: 27141 RVA: 0x001F0B2C File Offset: 0x001EED2C
		private void DoVector3Lerp()
		{
			this.storeResult.Value = Vector3.Lerp(this.fromVector.Value, this.toVector.Value, this.amount.Value);
		}

		// Token: 0x040051D3 RID: 20947
		[RequiredField]
		[Tooltip("First Vector.")]
		public FsmVector3 fromVector;

		// Token: 0x040051D4 RID: 20948
		[Tooltip("Second Vector.")]
		[RequiredField]
		public FsmVector3 toVector;

		// Token: 0x040051D5 RID: 20949
		[RequiredField]
		[Tooltip("Interpolate between From Vector and ToVector by this amount. Value is clamped to 0-1 range. 0 = From Vector; 1 = To Vector; 0.5 = half way between.")]
		public FsmFloat amount;

		// Token: 0x040051D6 RID: 20950
		[Tooltip("Store the result in this vector variable.")]
		[UIHint(10)]
		[RequiredField]
		public FsmVector3 storeResult;

		// Token: 0x040051D7 RID: 20951
		[Tooltip("Repeat every frame. Useful if any of the values are changing.")]
		public bool everyFrame;
	}
}
