using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D15 RID: 3349
	[ActionCategory(19)]
	[Tooltip("Normalizes a Vector3 Variable.")]
	public class Vector3Normalize : FsmStateAction
	{
		// Token: 0x06006A0F RID: 27151 RVA: 0x001F0D94 File Offset: 0x001EEF94
		public override void Reset()
		{
			this.vector3Variable = null;
			this.everyFrame = false;
		}

		// Token: 0x06006A10 RID: 27152 RVA: 0x001F0DA4 File Offset: 0x001EEFA4
		public override void OnEnter()
		{
			this.vector3Variable.Value = this.vector3Variable.Value.normalized;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006A11 RID: 27153 RVA: 0x001F0DE0 File Offset: 0x001EEFE0
		public override void OnUpdate()
		{
			this.vector3Variable.Value = this.vector3Variable.Value.normalized;
		}

		// Token: 0x040051DE RID: 20958
		[RequiredField]
		[UIHint(10)]
		public FsmVector3 vector3Variable;

		// Token: 0x040051DF RID: 20959
		public bool everyFrame;
	}
}
