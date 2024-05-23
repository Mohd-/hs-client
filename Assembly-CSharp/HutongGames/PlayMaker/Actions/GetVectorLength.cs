using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C3D RID: 3133
	[Tooltip("Get Vector3 Length.")]
	[ActionCategory(19)]
	public class GetVectorLength : FsmStateAction
	{
		// Token: 0x0600665B RID: 26203 RVA: 0x001E433C File Offset: 0x001E253C
		public override void Reset()
		{
			this.vector3 = null;
			this.storeLength = null;
		}

		// Token: 0x0600665C RID: 26204 RVA: 0x001E434C File Offset: 0x001E254C
		public override void OnEnter()
		{
			this.DoVectorLength();
			base.Finish();
		}

		// Token: 0x0600665D RID: 26205 RVA: 0x001E435C File Offset: 0x001E255C
		private void DoVectorLength()
		{
			if (this.vector3 == null)
			{
				return;
			}
			if (this.storeLength == null)
			{
				return;
			}
			this.storeLength.Value = this.vector3.Value.magnitude;
		}

		// Token: 0x04004E15 RID: 19989
		public FsmVector3 vector3;

		// Token: 0x04004E16 RID: 19990
		[RequiredField]
		[UIHint(10)]
		public FsmFloat storeLength;
	}
}
