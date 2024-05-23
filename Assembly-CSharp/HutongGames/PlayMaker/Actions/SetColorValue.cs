using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CA1 RID: 3233
	[Tooltip("Sets the value of a Color Variable.")]
	[ActionCategory(24)]
	public class SetColorValue : FsmStateAction
	{
		// Token: 0x06006812 RID: 26642 RVA: 0x001EA1F1 File Offset: 0x001E83F1
		public override void Reset()
		{
			this.colorVariable = null;
			this.color = null;
			this.everyFrame = false;
		}

		// Token: 0x06006813 RID: 26643 RVA: 0x001EA208 File Offset: 0x001E8408
		public override void OnEnter()
		{
			this.DoSetColorValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006814 RID: 26644 RVA: 0x001EA221 File Offset: 0x001E8421
		public override void OnUpdate()
		{
			this.DoSetColorValue();
		}

		// Token: 0x06006815 RID: 26645 RVA: 0x001EA229 File Offset: 0x001E8429
		private void DoSetColorValue()
		{
			if (this.colorVariable != null)
			{
				this.colorVariable.Value = this.color.Value;
			}
		}

		// Token: 0x04004FDB RID: 20443
		[RequiredField]
		[UIHint(10)]
		public FsmColor colorVariable;

		// Token: 0x04004FDC RID: 20444
		[RequiredField]
		public FsmColor color;

		// Token: 0x04004FDD RID: 20445
		public bool everyFrame;
	}
}
