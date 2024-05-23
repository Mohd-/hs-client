using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B94 RID: 2964
	[ActionCategory(33)]
	[Tooltip("Causes the device to vibrate for half a second.")]
	public class DeviceVibrate : FsmStateAction
	{
		// Token: 0x060063BF RID: 25535 RVA: 0x001DBC0F File Offset: 0x001D9E0F
		public override void Reset()
		{
		}

		// Token: 0x060063C0 RID: 25536 RVA: 0x001DBC11 File Offset: 0x001D9E11
		public override void OnEnter()
		{
			base.Finish();
		}
	}
}
