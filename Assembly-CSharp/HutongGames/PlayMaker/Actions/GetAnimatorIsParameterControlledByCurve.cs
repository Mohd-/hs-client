using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D5E RID: 3422
	[ActionCategory("Animator")]
	[Tooltip("Returns true if a parameter is controlled by an additional curve on an animation")]
	public class GetAnimatorIsParameterControlledByCurve : FsmStateAction
	{
		// Token: 0x06006B63 RID: 27491 RVA: 0x001F95B9 File Offset: 0x001F77B9
		public override void Reset()
		{
			this.gameObject = null;
			this.parameterName = null;
			this.isControlledByCurve = null;
			this.isControlledByCurveEvent = null;
			this.isNotControlledByCurveEvent = null;
		}

		// Token: 0x06006B64 RID: 27492 RVA: 0x001F95E0 File Offset: 0x001F77E0
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
			this.DoCheckIsParameterControlledByCurve();
			base.Finish();
		}

		// Token: 0x06006B65 RID: 27493 RVA: 0x001F9644 File Offset: 0x001F7844
		private void DoCheckIsParameterControlledByCurve()
		{
			if (this._animator == null)
			{
				return;
			}
			bool flag = this._animator.IsParameterControlledByCurve(this.parameterName.Value);
			this.isControlledByCurve.Value = flag;
			if (flag)
			{
				base.Fsm.Event(this.isControlledByCurveEvent);
			}
			else
			{
				base.Fsm.Event(this.isNotControlledByCurveEvent);
			}
		}

		// Token: 0x040053DF RID: 21471
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target. An Animator component is required")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040053E0 RID: 21472
		public FsmString parameterName;

		// Token: 0x040053E1 RID: 21473
		[ActionSection("Results")]
		[UIHint(10)]
		[Tooltip("True if controlled by curve")]
		public FsmBool isControlledByCurve;

		// Token: 0x040053E2 RID: 21474
		[Tooltip("Event send if controlled by curve")]
		public FsmEvent isControlledByCurveEvent;

		// Token: 0x040053E3 RID: 21475
		[Tooltip("Event send if not controlled by curve")]
		public FsmEvent isNotControlledByCurveEvent;

		// Token: 0x040053E4 RID: 21476
		private Animator _animator;
	}
}
