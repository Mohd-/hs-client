using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D57 RID: 3415
	[ActionCategory("Animator")]
	[Tooltip("Returns the scale of the current Avatar for a humanoid rig, (1 by default if the rig is generic).\n The scale is relative to Unity's Default Avatar")]
	public class GetAnimatorHumanScale : FsmStateAction
	{
		// Token: 0x06006B40 RID: 27456 RVA: 0x001F8D75 File Offset: 0x001F6F75
		public override void Reset()
		{
			this.gameObject = null;
			this.humanScale = null;
		}

		// Token: 0x06006B41 RID: 27457 RVA: 0x001F8D88 File Offset: 0x001F6F88
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				base.Finish();
				return;
			}
			this._animator = ownerDefaultTarget.GetComponent<Animator>();
			if (this._animator == null)
			{
				base.Finish();
				return;
			}
			this.DoGetHumanScale();
			base.Finish();
		}

		// Token: 0x06006B42 RID: 27458 RVA: 0x001F8DEC File Offset: 0x001F6FEC
		private void DoGetHumanScale()
		{
			if (this._animator == null)
			{
				return;
			}
			this.humanScale.Value = this._animator.humanScale;
		}

		// Token: 0x040053B7 RID: 21431
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The Target. An Animator component is required")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040053B8 RID: 21432
		[UIHint(10)]
		[Tooltip("the scale of the current Avatar")]
		[ActionSection("Result")]
		public FsmFloat humanScale;

		// Token: 0x040053B9 RID: 21433
		private Animator _animator;
	}
}
