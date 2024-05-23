using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C31 RID: 3121
	[Tooltip("Gets the Right n characters from a String.")]
	[ActionCategory(16)]
	public class GetStringRight : FsmStateAction
	{
		// Token: 0x06006628 RID: 26152 RVA: 0x001E3A25 File Offset: 0x001E1C25
		public override void Reset()
		{
			this.stringVariable = null;
			this.charCount = 0;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006629 RID: 26153 RVA: 0x001E3A48 File Offset: 0x001E1C48
		public override void OnEnter()
		{
			this.DoGetStringRight();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600662A RID: 26154 RVA: 0x001E3A61 File Offset: 0x001E1C61
		public override void OnUpdate()
		{
			this.DoGetStringRight();
		}

		// Token: 0x0600662B RID: 26155 RVA: 0x001E3A6C File Offset: 0x001E1C6C
		private void DoGetStringRight()
		{
			if (this.stringVariable == null)
			{
				return;
			}
			if (this.storeResult == null)
			{
				return;
			}
			string value = this.stringVariable.Value;
			this.storeResult.Value = value.Substring(value.Length - this.charCount.Value, this.charCount.Value);
		}

		// Token: 0x04004DDF RID: 19935
		[RequiredField]
		[UIHint(10)]
		public FsmString stringVariable;

		// Token: 0x04004DE0 RID: 19936
		public FsmInt charCount;

		// Token: 0x04004DE1 RID: 19937
		[UIHint(10)]
		[RequiredField]
		public FsmString storeResult;

		// Token: 0x04004DE2 RID: 19938
		public bool everyFrame;
	}
}
