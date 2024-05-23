using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D5B RID: 3419
	[Tooltip("Returns true if the current rig is humanoid, false if it is generic. Can also sends events")]
	[ActionCategory("Animator")]
	public class GetAnimatorIsHuman : FsmStateAction
	{
		// Token: 0x06006B51 RID: 27473 RVA: 0x001F9195 File Offset: 0x001F7395
		public override void Reset()
		{
			this.gameObject = null;
			this.isHuman = null;
			this.isHumanEvent = null;
			this.isGenericEvent = null;
		}

		// Token: 0x06006B52 RID: 27474 RVA: 0x001F91B4 File Offset: 0x001F73B4
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
			this.DoCheckIsHuman();
			base.Finish();
		}

		// Token: 0x06006B53 RID: 27475 RVA: 0x001F9218 File Offset: 0x001F7418
		private void DoCheckIsHuman()
		{
			if (this._animator == null)
			{
				return;
			}
			bool flag = this._animator.isHuman;
			this.isHuman.Value = flag;
			if (flag)
			{
				base.Fsm.Event(this.isHumanEvent);
			}
			else
			{
				base.Fsm.Event(this.isGenericEvent);
			}
		}

		// Token: 0x040053CB RID: 21451
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The Target. An Animator component is required")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040053CC RID: 21452
		[ActionSection("Results")]
		[Tooltip("True if the current rig is humanoid, False if it is generic")]
		[UIHint(10)]
		public FsmBool isHuman;

		// Token: 0x040053CD RID: 21453
		[Tooltip("Event send if rig is humanoid")]
		public FsmEvent isHumanEvent;

		// Token: 0x040053CE RID: 21454
		[Tooltip("Event send if rig is generic")]
		public FsmEvent isGenericEvent;

		// Token: 0x040053CF RID: 21455
		private Animator _animator;
	}
}
