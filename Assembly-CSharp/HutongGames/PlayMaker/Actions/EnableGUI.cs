using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B9F RID: 2975
	[ActionCategory(5)]
	[Tooltip("Enables/Disables the PlayMakerGUI component in the scene. Note, you need a PlayMakerGUI component in the scene to see OnGUI actions. However, OnGUI can be very expensive on mobile devices. This action lets you turn OnGUI on/off (e.g., turn it on for a menu, and off during gameplay).")]
	public class EnableGUI : FsmStateAction
	{
		// Token: 0x060063E8 RID: 25576 RVA: 0x001DC54A File Offset: 0x001DA74A
		public override void Reset()
		{
			this.enableGUI = true;
		}

		// Token: 0x060063E9 RID: 25577 RVA: 0x001DC558 File Offset: 0x001DA758
		public override void OnEnter()
		{
			PlayMakerGUI.Instance.enabled = this.enableGUI.Value;
			base.Finish();
		}

		// Token: 0x04004B5B RID: 19291
		[Tooltip("Set to True to enable, False to disable.")]
		public FsmBool enableGUI;
	}
}
