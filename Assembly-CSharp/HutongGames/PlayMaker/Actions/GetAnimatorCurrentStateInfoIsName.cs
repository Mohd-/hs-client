using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D4E RID: 3406
	[ActionCategory("Animator")]
	[Tooltip("Check the current State name on a specified layer, this is more than the layer name, it holds the current state as well.")]
	public class GetAnimatorCurrentStateInfoIsName : FsmStateAction
	{
		// Token: 0x06006B0C RID: 27404 RVA: 0x001F81C0 File Offset: 0x001F63C0
		public override void Reset()
		{
			this.gameObject = null;
			this.layerIndex = null;
			this.name = null;
			this.nameMatchEvent = null;
			this.nameDoNotMatchEvent = null;
			this.everyFrame = false;
		}

		// Token: 0x06006B0D RID: 27405 RVA: 0x001F81F8 File Offset: 0x001F63F8
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

		// Token: 0x06006B0E RID: 27406 RVA: 0x001F8265 File Offset: 0x001F6465
		public override void OnUpdate()
		{
			this.IsName();
		}

		// Token: 0x06006B0F RID: 27407 RVA: 0x001F8270 File Offset: 0x001F6470
		private void IsName()
		{
			if (this._animator != null)
			{
				if (this._animator.GetCurrentAnimatorStateInfo(this.layerIndex.Value).IsName(this.name.Value))
				{
					base.Fsm.Event(this.nameMatchEvent);
				}
				else
				{
					base.Fsm.Event(this.nameDoNotMatchEvent);
				}
			}
		}

		// Token: 0x04005375 RID: 21365
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005376 RID: 21366
		[RequiredField]
		[Tooltip("The layer's index")]
		public FsmInt layerIndex;

		// Token: 0x04005377 RID: 21367
		[Tooltip("The name to check the layer against.")]
		public FsmString name;

		// Token: 0x04005378 RID: 21368
		public bool everyFrame;

		// Token: 0x04005379 RID: 21369
		[ActionSection("Results")]
		public FsmEvent nameMatchEvent;

		// Token: 0x0400537A RID: 21370
		public FsmEvent nameDoNotMatchEvent;

		// Token: 0x0400537B RID: 21371
		private PlayMakerAnimatorMoveProxy _animatorProxy;

		// Token: 0x0400537C RID: 21372
		private Animator _animator;
	}
}
