using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DBD RID: 3517
	[ActionCategory("Pegasus")]
	[Tooltip("Used to control the state of the Pegasus Highlight system")]
	public class HighlightContinuousUpdateAction : FsmStateAction
	{
		// Token: 0x06006D0E RID: 27918 RVA: 0x0020137E File Offset: 0x001FF57E
		public override void Reset()
		{
			this.m_gameObj = null;
			this.m_updateTime = 1f;
		}

		// Token: 0x06006D0F RID: 27919 RVA: 0x00201398 File Offset: 0x001FF598
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
				highlightState.ContinuousUpdate(this.m_updateTime.Value);
			}
			base.Finish();
		}

		// Token: 0x040055BB RID: 21947
		[Tooltip("GameObject to send highlight states to")]
		[RequiredField]
		public FsmOwnerDefault m_gameObj;

		// Token: 0x040055BC RID: 21948
		[Tooltip("Amount of time to render")]
		[RequiredField]
		public FsmFloat m_updateTime = 1f;

		// Token: 0x040055BD RID: 21949
		private DelayedEvent delayedEvent;
	}
}
