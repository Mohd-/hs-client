using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DBE RID: 3518
	[ActionCategory("Pegasus")]
	[Tooltip("Tells the Highlight system when the state is finished.")]
	public class HighlightFinishAction : FsmStateAction
	{
		// Token: 0x06006D11 RID: 27921 RVA: 0x0020141C File Offset: 0x001FF61C
		public HighlightState CacheHighlightState()
		{
			if (this.m_HighlightState == null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
				if (ownerDefaultTarget != null)
				{
					this.m_HighlightState = SceneUtils.FindComponentInThisOrParents<HighlightState>(ownerDefaultTarget);
				}
			}
			return this.m_HighlightState;
		}

		// Token: 0x06006D12 RID: 27922 RVA: 0x0020146A File Offset: 0x001FF66A
		public override void Reset()
		{
			this.m_GameObject = null;
		}

		// Token: 0x06006D13 RID: 27923 RVA: 0x00201474 File Offset: 0x001FF674
		public override void OnEnter()
		{
			this.CacheHighlightState();
			if (this.m_HighlightState == null)
			{
				Debug.LogError(string.Format("{0}.OnEnter() - FAILED to find {1} in hierarchy", this, typeof(HighlightState)));
				base.Finish();
				return;
			}
			this.m_HighlightState.OnActionFinished();
			base.Finish();
		}

		// Token: 0x040055BE RID: 21950
		[RequiredField]
		public FsmOwnerDefault m_GameObject;

		// Token: 0x040055BF RID: 21951
		protected HighlightState m_HighlightState;
	}
}
