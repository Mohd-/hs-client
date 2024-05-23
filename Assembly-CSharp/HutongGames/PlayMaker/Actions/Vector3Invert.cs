using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D11 RID: 3345
	[Tooltip("Reverses the direction of a Vector3 Variable. Same as multiplying by -1.")]
	[ActionCategory(19)]
	public class Vector3Invert : FsmStateAction
	{
		// Token: 0x060069FE RID: 27134 RVA: 0x001F0A4C File Offset: 0x001EEC4C
		public override void Reset()
		{
			this.vector3Variable = null;
			this.everyFrame = false;
		}

		// Token: 0x060069FF RID: 27135 RVA: 0x001F0A5C File Offset: 0x001EEC5C
		public override void OnEnter()
		{
			this.vector3Variable.Value = this.vector3Variable.Value * -1f;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006A00 RID: 27136 RVA: 0x001F0A90 File Offset: 0x001EEC90
		public override void OnUpdate()
		{
			this.vector3Variable.Value = this.vector3Variable.Value * -1f;
		}

		// Token: 0x040051D1 RID: 20945
		[RequiredField]
		[UIHint(10)]
		public FsmVector3 vector3Variable;

		// Token: 0x040051D2 RID: 20946
		public bool everyFrame;
	}
}
