using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D52 RID: 3410
	[ActionCategory("Animator")]
	[Tooltip("Check the active Transition user-specified name on a specified layer.")]
	public class GetAnimatorCurrentTransitionInfoIsUserName : FsmStateAction
	{
		// Token: 0x06006B22 RID: 27426 RVA: 0x001F874B File Offset: 0x001F694B
		public override void Reset()
		{
			this.gameObject = null;
			this.layerIndex = null;
			this.userName = null;
			this.nameMatch = null;
			this.nameMatchEvent = null;
			this.nameDoNotMatchEvent = null;
			this.everyFrame = false;
		}

		// Token: 0x06006B23 RID: 27427 RVA: 0x001F8780 File Offset: 0x001F6980
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
			this.IsName();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006B24 RID: 27428 RVA: 0x001F87ED File Offset: 0x001F69ED
		public override void OnUpdate()
		{
			this.IsName();
		}

		// Token: 0x06006B25 RID: 27429 RVA: 0x001F87F8 File Offset: 0x001F69F8
		private void IsName()
		{
			if (this._animator != null)
			{
				if (this._animator.GetAnimatorTransitionInfo(this.layerIndex.Value).IsUserName(this.userName.Value))
				{
					this.nameMatch.Value = true;
					base.Fsm.Event(this.nameMatchEvent);
				}
				else
				{
					this.nameMatch.Value = false;
					base.Fsm.Event(this.nameDoNotMatchEvent);
				}
			}
		}

		// Token: 0x04005398 RID: 21400
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005399 RID: 21401
		[RequiredField]
		[Tooltip("The layer's index")]
		public FsmInt layerIndex;

		// Token: 0x0400539A RID: 21402
		[Tooltip("The user-specified name to check the transition against.")]
		public FsmString userName;

		// Token: 0x0400539B RID: 21403
		public bool everyFrame;

		// Token: 0x0400539C RID: 21404
		[ActionSection("Results")]
		public FsmBool nameMatch;

		// Token: 0x0400539D RID: 21405
		public FsmEvent nameMatchEvent;

		// Token: 0x0400539E RID: 21406
		public FsmEvent nameDoNotMatchEvent;

		// Token: 0x0400539F RID: 21407
		private PlayMakerAnimatorMoveProxy _animatorProxy;

		// Token: 0x040053A0 RID: 21408
		private Animator _animator;
	}
}
