using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D51 RID: 3409
	[ActionCategory("Animator")]
	[Tooltip("Check the active Transition name on a specified layer. Format is 'CURRENT_STATE -> NEXT_STATE'.")]
	public class GetAnimatorCurrentTransitionInfoIsName : FsmStateAction
	{
		// Token: 0x06006B1D RID: 27421 RVA: 0x001F860D File Offset: 0x001F680D
		public override void Reset()
		{
			this.gameObject = null;
			this.layerIndex = null;
			this.name = null;
			this.nameMatch = null;
			this.nameMatchEvent = null;
			this.nameDoNotMatchEvent = null;
			this.everyFrame = false;
		}

		// Token: 0x06006B1E RID: 27422 RVA: 0x001F8640 File Offset: 0x001F6840
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

		// Token: 0x06006B1F RID: 27423 RVA: 0x001F86AD File Offset: 0x001F68AD
		public override void OnUpdate()
		{
			this.IsName();
		}

		// Token: 0x06006B20 RID: 27424 RVA: 0x001F86B8 File Offset: 0x001F68B8
		private void IsName()
		{
			if (this._animator != null)
			{
				if (this._animator.GetAnimatorTransitionInfo(this.layerIndex.Value).IsName(this.name.Value))
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

		// Token: 0x0400538F RID: 21391
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005390 RID: 21392
		[Tooltip("The layer's index")]
		[RequiredField]
		public FsmInt layerIndex;

		// Token: 0x04005391 RID: 21393
		[Tooltip("The name to check the transition against.")]
		public FsmString name;

		// Token: 0x04005392 RID: 21394
		public bool everyFrame;

		// Token: 0x04005393 RID: 21395
		[ActionSection("Results")]
		public FsmBool nameMatch;

		// Token: 0x04005394 RID: 21396
		public FsmEvent nameMatchEvent;

		// Token: 0x04005395 RID: 21397
		public FsmEvent nameDoNotMatchEvent;

		// Token: 0x04005396 RID: 21398
		private PlayMakerAnimatorMoveProxy _animatorProxy;

		// Token: 0x04005397 RID: 21399
		private Animator _animator;
	}
}
