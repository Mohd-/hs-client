using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DBF RID: 3519
	[Tooltip("Used to Show and Hide card Highlights")]
	[ActionCategory("Pegasus")]
	public class HighlightShowHide : FsmStateAction
	{
		// Token: 0x06006D15 RID: 27925 RVA: 0x002014DF File Offset: 0x001FF6DF
		public override void Reset()
		{
			this.m_gameObj = null;
			this.m_Show = true;
		}

		// Token: 0x06006D16 RID: 27926 RVA: 0x002014F4 File Offset: 0x001FF6F4
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
				if (this.m_Show.Value)
				{
					highlightState.Show();
				}
				else
				{
					highlightState.Hide();
				}
			}
			base.Finish();
		}

		// Token: 0x040055C0 RID: 21952
		[RequiredField]
		[Tooltip("GameObject to send highlight states to")]
		public FsmOwnerDefault m_gameObj;

		// Token: 0x040055C1 RID: 21953
		[RequiredField]
		[Tooltip("Show or Hide")]
		public FsmBool m_Show = true;

		// Token: 0x040055C2 RID: 21954
		private DelayedEvent delayedEvent;
	}
}
