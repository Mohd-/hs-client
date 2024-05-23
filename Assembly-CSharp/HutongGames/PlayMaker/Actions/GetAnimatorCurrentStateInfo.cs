using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D4D RID: 3405
	[Tooltip("Gets the current State information on a specified layer")]
	[ActionCategory("Animator")]
	public class GetAnimatorCurrentStateInfo : FsmStateAction
	{
		// Token: 0x06006B05 RID: 27397 RVA: 0x001F7F38 File Offset: 0x001F6138
		public override void Reset()
		{
			this.gameObject = null;
			this.layerIndex = null;
			this.name = null;
			this.nameHash = null;
			this.tagHash = null;
			this.length = null;
			this.normalizedTime = null;
			this.isStateLooping = null;
			this.loopCount = null;
			this.currentLoopProgress = null;
			this.everyFrame = false;
		}

		// Token: 0x06006B06 RID: 27398 RVA: 0x001F7F94 File Offset: 0x001F6194
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
			this._animatorProxy = ownerDefaultTarget.GetComponent<PlayMakerAnimatorMoveProxy>();
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent += new Action(this.OnAnimatorMoveEvent);
			}
			this.GetLayerInfo();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006B07 RID: 27399 RVA: 0x001F8035 File Offset: 0x001F6235
		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.GetLayerInfo();
			}
		}

		// Token: 0x06006B08 RID: 27400 RVA: 0x001F804E File Offset: 0x001F624E
		public void OnAnimatorMoveEvent()
		{
			if (this._animatorProxy != null)
			{
				this.GetLayerInfo();
			}
		}

		// Token: 0x06006B09 RID: 27401 RVA: 0x001F8068 File Offset: 0x001F6268
		private void GetLayerInfo()
		{
			if (this._animator != null)
			{
				AnimatorStateInfo currentAnimatorStateInfo = this._animator.GetCurrentAnimatorStateInfo(this.layerIndex.Value);
				this.nameHash.Value = currentAnimatorStateInfo.fullPathHash;
				if (!this.name.IsNone)
				{
					this.name.Value = this._animator.GetLayerName(this.layerIndex.Value);
				}
				this.tagHash.Value = currentAnimatorStateInfo.tagHash;
				this.length.Value = currentAnimatorStateInfo.length;
				this.isStateLooping.Value = currentAnimatorStateInfo.loop;
				this.normalizedTime.Value = currentAnimatorStateInfo.normalizedTime;
				if (!this.loopCount.IsNone || !this.currentLoopProgress.IsNone)
				{
					this.loopCount.Value = (int)Math.Truncate((double)currentAnimatorStateInfo.normalizedTime);
					this.currentLoopProgress.Value = currentAnimatorStateInfo.normalizedTime - (float)this.loopCount.Value;
				}
			}
		}

		// Token: 0x06006B0A RID: 27402 RVA: 0x001F8180 File Offset: 0x001F6380
		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent -= new Action(this.OnAnimatorMoveEvent);
			}
		}

		// Token: 0x04005368 RID: 21352
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		[CheckForComponent(typeof(Animator))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005369 RID: 21353
		[RequiredField]
		[Tooltip("The layer's index")]
		public FsmInt layerIndex;

		// Token: 0x0400536A RID: 21354
		[Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
		public bool everyFrame;

		// Token: 0x0400536B RID: 21355
		[UIHint(10)]
		[ActionSection("Results")]
		[Tooltip("The layer's name.")]
		public FsmString name;

		// Token: 0x0400536C RID: 21356
		[Tooltip("The layer's name Hash")]
		[UIHint(10)]
		public FsmInt nameHash;

		// Token: 0x0400536D RID: 21357
		[UIHint(10)]
		[Tooltip("The layer's tag hash")]
		public FsmInt tagHash;

		// Token: 0x0400536E RID: 21358
		[Tooltip("Is the state looping. All animations in the state must be looping")]
		[UIHint(10)]
		public FsmBool isStateLooping;

		// Token: 0x0400536F RID: 21359
		[UIHint(10)]
		[Tooltip("The Current duration of the state. In seconds, can vary when the State contains a Blend Tree ")]
		public FsmFloat length;

		// Token: 0x04005370 RID: 21360
		[UIHint(10)]
		[Tooltip("The integer part is the number of time a state has been looped. The fractional part is the % (0-1) of progress in the current loop")]
		public FsmFloat normalizedTime;

		// Token: 0x04005371 RID: 21361
		[Tooltip("The integer part is the number of time a state has been looped. This is extracted from the normalizedTime")]
		[UIHint(10)]
		public FsmInt loopCount;

		// Token: 0x04005372 RID: 21362
		[Tooltip("The progress in the current loop. This is extracted from the normalizedTime")]
		[UIHint(10)]
		public FsmFloat currentLoopProgress;

		// Token: 0x04005373 RID: 21363
		private PlayMakerAnimatorMoveProxy _animatorProxy;

		// Token: 0x04005374 RID: 21364
		private Animator _animator;
	}
}
