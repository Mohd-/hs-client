using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DC3 RID: 3523
	[Tooltip("Sends Events based on whether or not an option is enabled by the player.")]
	[ActionCategory("Pegasus")]
	public class IsOptionEnabledAction : FsmStateAction
	{
		// Token: 0x06006D23 RID: 27939 RVA: 0x00201734 File Offset: 0x001FF934
		public override void Reset()
		{
			this.m_Option = Option.INVALID;
			this.m_TrueEvent = null;
			this.m_FalseEvent = null;
			this.m_EveryFrame = false;
		}

		// Token: 0x06006D24 RID: 27940 RVA: 0x00201752 File Offset: 0x001FF952
		public override void OnEnter()
		{
			this.FireEvents();
			if (!this.m_EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D25 RID: 27941 RVA: 0x0020176B File Offset: 0x001FF96B
		public override void OnUpdate()
		{
			this.FireEvents();
		}

		// Token: 0x06006D26 RID: 27942 RVA: 0x00201773 File Offset: 0x001FF973
		public override string ErrorCheck()
		{
			if (!FsmEvent.IsNullOrEmpty(this.m_TrueEvent))
			{
				return string.Empty;
			}
			if (!FsmEvent.IsNullOrEmpty(this.m_FalseEvent))
			{
				return string.Empty;
			}
			return "Action sends no events!";
		}

		// Token: 0x06006D27 RID: 27943 RVA: 0x002017A8 File Offset: 0x001FF9A8
		private void FireEvents()
		{
			if (Options.Get().GetBool(this.m_Option))
			{
				base.Fsm.Event(this.m_TrueEvent);
			}
			else
			{
				base.Fsm.Event(this.m_FalseEvent);
			}
		}

		// Token: 0x040055CB RID: 21963
		[Tooltip("The option to check.")]
		public Option m_Option;

		// Token: 0x040055CC RID: 21964
		[Tooltip("Event sent if the option is on.")]
		public FsmEvent m_TrueEvent;

		// Token: 0x040055CD RID: 21965
		[Tooltip("Event sent if the option is off.")]
		public FsmEvent m_FalseEvent;

		// Token: 0x040055CE RID: 21966
		[Tooltip("Check if the option is enabled every frame.")]
		public bool m_EveryFrame;
	}
}
