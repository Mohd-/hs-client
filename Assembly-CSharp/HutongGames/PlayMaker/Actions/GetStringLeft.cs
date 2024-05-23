using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C2F RID: 3119
	[ActionCategory(16)]
	[Tooltip("Gets the Left n characters from a String Variable.")]
	public class GetStringLeft : FsmStateAction
	{
		// Token: 0x0600661E RID: 26142 RVA: 0x001E3917 File Offset: 0x001E1B17
		public override void Reset()
		{
			this.stringVariable = null;
			this.charCount = 0;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x0600661F RID: 26143 RVA: 0x001E393A File Offset: 0x001E1B3A
		public override void OnEnter()
		{
			this.DoGetStringLeft();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006620 RID: 26144 RVA: 0x001E3953 File Offset: 0x001E1B53
		public override void OnUpdate()
		{
			this.DoGetStringLeft();
		}

		// Token: 0x06006621 RID: 26145 RVA: 0x001E395C File Offset: 0x001E1B5C
		private void DoGetStringLeft()
		{
			if (this.stringVariable == null)
			{
				return;
			}
			if (this.storeResult == null)
			{
				return;
			}
			this.storeResult.Value = this.stringVariable.Value.Substring(0, this.charCount.Value);
		}

		// Token: 0x04004DD8 RID: 19928
		[RequiredField]
		[UIHint(10)]
		public FsmString stringVariable;

		// Token: 0x04004DD9 RID: 19929
		public FsmInt charCount;

		// Token: 0x04004DDA RID: 19930
		[UIHint(10)]
		[RequiredField]
		public FsmString storeResult;

		// Token: 0x04004DDB RID: 19931
		public bool everyFrame;
	}
}
