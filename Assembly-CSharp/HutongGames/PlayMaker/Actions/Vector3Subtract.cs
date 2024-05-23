using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D1A RID: 3354
	[ActionCategory(19)]
	[Tooltip("Subtracts a Vector3 value from a Vector3 variable.")]
	public class Vector3Subtract : FsmStateAction
	{
		// Token: 0x06006A1F RID: 27167 RVA: 0x001F11B0 File Offset: 0x001EF3B0
		public override void Reset()
		{
			this.vector3Variable = null;
			this.subtractVector = new FsmVector3
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		// Token: 0x06006A20 RID: 27168 RVA: 0x001F11E0 File Offset: 0x001EF3E0
		public override void OnEnter()
		{
			this.vector3Variable.Value = this.vector3Variable.Value - this.subtractVector.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006A21 RID: 27169 RVA: 0x001F1224 File Offset: 0x001EF424
		public override void OnUpdate()
		{
			this.vector3Variable.Value = this.vector3Variable.Value - this.subtractVector.Value;
		}

		// Token: 0x040051F9 RID: 20985
		[RequiredField]
		[UIHint(10)]
		public FsmVector3 vector3Variable;

		// Token: 0x040051FA RID: 20986
		[RequiredField]
		public FsmVector3 subtractVector;

		// Token: 0x040051FB RID: 20987
		public bool everyFrame;
	}
}
