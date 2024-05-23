using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C33 RID: 3123
	[ActionCategory(31)]
	[Tooltip("Gets system date and time info and stores it in a string variable. An optional format string gives you a lot of control over the formatting (see online docs for format syntax).")]
	public class GetSystemDateTime : FsmStateAction
	{
		// Token: 0x06006632 RID: 26162 RVA: 0x001E3B82 File Offset: 0x001E1D82
		public override void Reset()
		{
			this.storeString = null;
			this.format = "MM/dd/yyyy HH:mm";
		}

		// Token: 0x06006633 RID: 26163 RVA: 0x001E3B9C File Offset: 0x001E1D9C
		public override void OnEnter()
		{
			this.storeString.Value = DateTime.Now.ToString(this.format.Value);
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006634 RID: 26164 RVA: 0x001E3BE0 File Offset: 0x001E1DE0
		public override void OnUpdate()
		{
			this.storeString.Value = DateTime.Now.ToString(this.format.Value);
		}

		// Token: 0x04004DE8 RID: 19944
		[Tooltip("Store System DateTime as a string.")]
		[UIHint(10)]
		public FsmString storeString;

		// Token: 0x04004DE9 RID: 19945
		[Tooltip("Optional format string. E.g., MM/dd/yyyy HH:mm")]
		public FsmString format;

		// Token: 0x04004DEA RID: 19946
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
