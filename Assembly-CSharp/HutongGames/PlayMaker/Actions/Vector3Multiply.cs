using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D14 RID: 3348
	[ActionCategory(19)]
	[Tooltip("Multiplies a Vector3 variable by a Float.")]
	public class Vector3Multiply : FsmStateAction
	{
		// Token: 0x06006A0B RID: 27147 RVA: 0x001F0CFE File Offset: 0x001EEEFE
		public override void Reset()
		{
			this.vector3Variable = null;
			this.multiplyBy = 1f;
			this.everyFrame = false;
		}

		// Token: 0x06006A0C RID: 27148 RVA: 0x001F0D20 File Offset: 0x001EEF20
		public override void OnEnter()
		{
			this.vector3Variable.Value = this.vector3Variable.Value * this.multiplyBy.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006A0D RID: 27149 RVA: 0x001F0D64 File Offset: 0x001EEF64
		public override void OnUpdate()
		{
			this.vector3Variable.Value = this.vector3Variable.Value * this.multiplyBy.Value;
		}

		// Token: 0x040051DB RID: 20955
		[UIHint(10)]
		[RequiredField]
		public FsmVector3 vector3Variable;

		// Token: 0x040051DC RID: 20956
		[RequiredField]
		public FsmFloat multiplyBy;

		// Token: 0x040051DD RID: 20957
		public bool everyFrame;
	}
}
