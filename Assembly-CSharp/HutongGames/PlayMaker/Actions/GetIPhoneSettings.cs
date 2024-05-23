using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C0E RID: 3086
	[ActionCategory(33)]
	[Tooltip("Get various iPhone settings.")]
	public class GetIPhoneSettings : FsmStateAction
	{
		// Token: 0x0600659B RID: 26011 RVA: 0x001E2574 File Offset: 0x001E0774
		public override void Reset()
		{
			this.getScreenCanDarken = null;
			this.getUniqueIdentifier = null;
			this.getName = null;
			this.getModel = null;
			this.getSystemName = null;
			this.getGeneration = null;
		}

		// Token: 0x0600659C RID: 26012 RVA: 0x001E25AB File Offset: 0x001E07AB
		public override void OnEnter()
		{
			base.Finish();
		}

		// Token: 0x04004D6A RID: 19818
		[UIHint(10)]
		[Tooltip("Allows device to fall into 'sleep' state with screen being dim if no touches occurred. Default value is true.")]
		public FsmBool getScreenCanDarken;

		// Token: 0x04004D6B RID: 19819
		[Tooltip("A unique device identifier string. It is guaranteed to be unique for every device (Read Only).")]
		[UIHint(10)]
		public FsmString getUniqueIdentifier;

		// Token: 0x04004D6C RID: 19820
		[Tooltip("The user defined name of the device (Read Only).")]
		[UIHint(10)]
		public FsmString getName;

		// Token: 0x04004D6D RID: 19821
		[UIHint(10)]
		[Tooltip("The model of the device (Read Only).")]
		public FsmString getModel;

		// Token: 0x04004D6E RID: 19822
		[UIHint(10)]
		[Tooltip("The name of the operating system running on the device (Read Only).")]
		public FsmString getSystemName;

		// Token: 0x04004D6F RID: 19823
		[Tooltip("The generation of the device (Read Only).")]
		[UIHint(10)]
		public FsmString getGeneration;
	}
}
