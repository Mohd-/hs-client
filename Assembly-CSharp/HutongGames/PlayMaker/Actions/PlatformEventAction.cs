using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DD0 RID: 3536
	[Tooltip("Event based on current platform")]
	[ActionCategory("Pegasus")]
	public class PlatformEventAction : FsmStateAction
	{
		// Token: 0x06006D5D RID: 27997 RVA: 0x0020272F File Offset: 0x0020092F
		public override void Reset()
		{
			this.m_PhoneEvent = null;
			this.m_DefaultEvent = null;
		}

		// Token: 0x06006D5E RID: 27998 RVA: 0x00202740 File Offset: 0x00200940
		public override void OnEnter()
		{
			if (UniversalInputManager.UsePhoneUI && this.m_PhoneEvent != null)
			{
				base.Fsm.Event(this.m_PhoneEvent);
			}
			else
			{
				base.Fsm.Event(this.m_DefaultEvent);
			}
		}

		// Token: 0x06006D5F RID: 27999 RVA: 0x00202790 File Offset: 0x00200990
		public override string ErrorCheck()
		{
			return string.Empty;
		}

		// Token: 0x0400560B RID: 22027
		public FsmEvent m_DefaultEvent;

		// Token: 0x0400560C RID: 22028
		public FsmEvent m_PhoneEvent;
	}
}
