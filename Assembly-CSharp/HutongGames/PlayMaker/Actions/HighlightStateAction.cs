using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DC0 RID: 3520
	[Tooltip("Used to control the state of the Pegasus Highlight system")]
	[ActionCategory("Pegasus")]
	public class HighlightStateAction : FsmStateAction
	{
		// Token: 0x06006D18 RID: 27928 RVA: 0x00201590 File Offset: 0x001FF790
		public override void Reset()
		{
			this.m_gameObj = null;
			this.m_state = ActorStateType.HIGHLIGHT_OFF;
		}

		// Token: 0x06006D19 RID: 27929 RVA: 0x002015A4 File Offset: 0x001FF7A4
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_gameObj);
			if (ownerDefaultTarget == null)
			{
				base.Finish();
				return;
			}
			HighlightState[] componentsInChildren = ownerDefaultTarget.GetComponentsInChildren<HighlightState>();
			if (componentsInChildren == null)
			{
				base.Finish();
				return;
			}
			foreach (HighlightState highlightState in componentsInChildren)
			{
				highlightState.ChangeState(this.m_state);
			}
			base.Finish();
		}

		// Token: 0x040055C3 RID: 21955
		[RequiredField]
		[Tooltip("GameObject to send highlight states to")]
		public FsmOwnerDefault m_gameObj;

		// Token: 0x040055C4 RID: 21956
		[Tooltip("State to send")]
		[RequiredField]
		public ActorStateType m_state = ActorStateType.HIGHLIGHT_OFF;

		// Token: 0x040055C5 RID: 21957
		private DelayedEvent delayedEvent;
	}
}
