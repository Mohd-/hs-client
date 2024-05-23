using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DC1 RID: 3521
	[Tooltip("Used to control the state of the Pegasus Highlight system")]
	[ActionCategory("Pegasus")]
	public class HighlightUpdateAction : FsmStateAction
	{
		// Token: 0x06006D1B RID: 27931 RVA: 0x00201624 File Offset: 0x001FF824
		public override void Reset()
		{
			this.m_gameObj = null;
		}

		// Token: 0x06006D1C RID: 27932 RVA: 0x00201630 File Offset: 0x001FF830
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
				highlightState.ForceUpdate();
			}
			base.Finish();
		}

		// Token: 0x040055C6 RID: 21958
		[RequiredField]
		[Tooltip("GameObject to send highlight states to")]
		public FsmOwnerDefault m_gameObj;

		// Token: 0x040055C7 RID: 21959
		private DelayedEvent delayedEvent;
	}
}
