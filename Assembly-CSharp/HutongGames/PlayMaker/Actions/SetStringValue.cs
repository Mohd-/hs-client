using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CE6 RID: 3302
	[ActionCategory(16)]
	[Tooltip("Sets the value of a String Variable.")]
	public class SetStringValue : FsmStateAction
	{
		// Token: 0x06006942 RID: 26946 RVA: 0x001ED88F File Offset: 0x001EBA8F
		public override void Reset()
		{
			this.stringVariable = null;
			this.stringValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006943 RID: 26947 RVA: 0x001ED8A6 File Offset: 0x001EBAA6
		public override void OnEnter()
		{
			this.DoSetStringValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006944 RID: 26948 RVA: 0x001ED8BF File Offset: 0x001EBABF
		public override void OnUpdate()
		{
			this.DoSetStringValue();
		}

		// Token: 0x06006945 RID: 26949 RVA: 0x001ED8C8 File Offset: 0x001EBAC8
		private void DoSetStringValue()
		{
			if (this.stringVariable == null)
			{
				return;
			}
			if (this.stringValue == null)
			{
				return;
			}
			this.stringVariable.Value = this.stringValue.Value;
		}

		// Token: 0x040050F3 RID: 20723
		[UIHint(10)]
		[RequiredField]
		public FsmString stringVariable;

		// Token: 0x040050F4 RID: 20724
		[RequiredField]
		public FsmString stringValue;

		// Token: 0x040050F5 RID: 20725
		public bool everyFrame;
	}
}
