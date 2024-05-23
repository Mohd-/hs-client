using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DD8 RID: 3544
	[Tooltip("Sets the alpha on a game object and its children.")]
	[ActionCategory("Pegasus")]
	public class SetAlphaRecursiveAction : FsmStateAction
	{
		// Token: 0x06006D7C RID: 28028 RVA: 0x00202D86 File Offset: 0x00200F86
		public override void Reset()
		{
			this.m_GameObject = null;
			this.m_Alpha = 0f;
			this.m_EveryFrame = false;
			this.m_IncludeChildren = false;
		}

		// Token: 0x06006D7D RID: 28029 RVA: 0x00202DB0 File Offset: 0x00200FB0
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
			if (ownerDefaultTarget == null)
			{
				base.Finish();
				return;
			}
			this.UpdateAlpha();
			if (!this.m_EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D7E RID: 28030 RVA: 0x00202DF9 File Offset: 0x00200FF9
		public override void OnUpdate()
		{
			this.UpdateAlpha();
		}

		// Token: 0x06006D7F RID: 28031 RVA: 0x00202E04 File Offset: 0x00201004
		private void UpdateAlpha()
		{
			if (this.m_Alpha.IsNone)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			RenderUtils.SetAlpha(ownerDefaultTarget, this.m_Alpha.Value);
		}

		// Token: 0x04005625 RID: 22053
		public FsmOwnerDefault m_GameObject;

		// Token: 0x04005626 RID: 22054
		[HasFloatSlider(0f, 1f)]
		public FsmFloat m_Alpha;

		// Token: 0x04005627 RID: 22055
		public bool m_EveryFrame;

		// Token: 0x04005628 RID: 22056
		public bool m_IncludeChildren;
	}
}
