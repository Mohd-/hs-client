using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CDA RID: 3290
	[Tooltip("Sets the value of an Object Variable.")]
	[ActionCategory(40)]
	public class SetObjectValue : FsmStateAction
	{
		// Token: 0x0600690A RID: 26890 RVA: 0x001ECC99 File Offset: 0x001EAE99
		public override void Reset()
		{
			this.objectVariable = null;
			this.objectValue = null;
			this.everyFrame = false;
		}

		// Token: 0x0600690B RID: 26891 RVA: 0x001ECCB0 File Offset: 0x001EAEB0
		public override void OnEnter()
		{
			this.objectVariable.Value = this.objectValue.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600690C RID: 26892 RVA: 0x001ECCD9 File Offset: 0x001EAED9
		public override void OnUpdate()
		{
			this.objectVariable.Value = this.objectValue.Value;
		}

		// Token: 0x040050BD RID: 20669
		[RequiredField]
		[UIHint(10)]
		public FsmObject objectVariable;

		// Token: 0x040050BE RID: 20670
		[RequiredField]
		public FsmObject objectValue;

		// Token: 0x040050BF RID: 20671
		public bool everyFrame;
	}
}
