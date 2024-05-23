using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D4F RID: 3407
	[Tooltip("Does tag match the tag of the active state in the statemachine")]
	[ActionCategory("Animator")]
	public class GetAnimatorCurrentStateInfoIsTag : FsmStateAction
	{
		// Token: 0x06006B11 RID: 27409 RVA: 0x001F82EB File Offset: 0x001F64EB
		public override void Reset()
		{
			this.gameObject = null;
			this.layerIndex = null;
			this.tag = null;
			this.tagMatch = null;
			this.tagMatchEvent = null;
			this.tagDoNotMatchEvent = null;
			this.everyFrame = false;
		}

		// Token: 0x06006B12 RID: 27410 RVA: 0x001F8320 File Offset: 0x001F6520
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
			this.IsTag();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006B13 RID: 27411 RVA: 0x001F838D File Offset: 0x001F658D
		public override void OnUpdate()
		{
			this.IsTag();
		}

		// Token: 0x06006B14 RID: 27412 RVA: 0x001F8398 File Offset: 0x001F6598
		private void IsTag()
		{
			if (this._animator != null)
			{
				if (this._animator.GetCurrentAnimatorStateInfo(this.layerIndex.Value).IsTag(this.tag.Value))
				{
					this.tagMatch.Value = true;
					base.Fsm.Event(this.tagMatchEvent);
				}
				else
				{
					this.tagMatch.Value = false;
					base.Fsm.Event(this.tagDoNotMatchEvent);
				}
			}
		}

		// Token: 0x0400537D RID: 21373
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		[CheckForComponent(typeof(Animator))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400537E RID: 21374
		[Tooltip("The layer's index")]
		[RequiredField]
		public FsmInt layerIndex;

		// Token: 0x0400537F RID: 21375
		[Tooltip("The tag to check the layer against.")]
		public FsmString tag;

		// Token: 0x04005380 RID: 21376
		public bool everyFrame;

		// Token: 0x04005381 RID: 21377
		[ActionSection("Results")]
		public FsmBool tagMatch;

		// Token: 0x04005382 RID: 21378
		public FsmEvent tagMatchEvent;

		// Token: 0x04005383 RID: 21379
		public FsmEvent tagDoNotMatchEvent;

		// Token: 0x04005384 RID: 21380
		private PlayMakerAnimatorMoveProxy _animatorProxy;

		// Token: 0x04005385 RID: 21381
		private Animator _animator;
	}
}
