using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D64 RID: 3428
	[ActionCategory("Animator")]
	[Tooltip("Gets the next State information on a specified layer")]
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1054")]
	public class GetAnimatorNextStateInfo : FsmStateAction
	{
		// Token: 0x06006B7F RID: 27519 RVA: 0x001F9B38 File Offset: 0x001F7D38
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

		// Token: 0x06006B80 RID: 27520 RVA: 0x001F9B94 File Offset: 0x001F7D94
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

		// Token: 0x06006B81 RID: 27521 RVA: 0x001F9C35 File Offset: 0x001F7E35
		public void OnAnimatorMoveEvent()
		{
			if (this._animatorProxy != null)
			{
				this.GetLayerInfo();
			}
		}

		// Token: 0x06006B82 RID: 27522 RVA: 0x001F9C4E File Offset: 0x001F7E4E
		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.GetLayerInfo();
			}
		}

		// Token: 0x06006B83 RID: 27523 RVA: 0x001F9C68 File Offset: 0x001F7E68
		private void GetLayerInfo()
		{
			if (this._animator != null)
			{
				AnimatorStateInfo nextAnimatorStateInfo = this._animator.GetNextAnimatorStateInfo(this.layerIndex.Value);
				this.nameHash.Value = nextAnimatorStateInfo.fullPathHash;
				if (!this.name.IsNone)
				{
					this.name.Value = this._animator.GetLayerName(this.layerIndex.Value);
				}
				this.tagHash.Value = nextAnimatorStateInfo.tagHash;
				this.length.Value = nextAnimatorStateInfo.length;
				this.isStateLooping.Value = nextAnimatorStateInfo.loop;
				this.normalizedTime.Value = nextAnimatorStateInfo.normalizedTime;
				if (!this.loopCount.IsNone || !this.currentLoopProgress.IsNone)
				{
					this.loopCount.Value = (int)Math.Truncate((double)nextAnimatorStateInfo.normalizedTime);
					this.currentLoopProgress.Value = nextAnimatorStateInfo.normalizedTime - (float)this.loopCount.Value;
				}
			}
		}

		// Token: 0x06006B84 RID: 27524 RVA: 0x001F9D80 File Offset: 0x001F7F80
		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent -= new Action(this.OnAnimatorMoveEvent);
			}
		}

		// Token: 0x040053FB RID: 21499
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040053FC RID: 21500
		[Tooltip("The layer's index")]
		[RequiredField]
		public FsmInt layerIndex;

		// Token: 0x040053FD RID: 21501
		[Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
		public bool everyFrame;

		// Token: 0x040053FE RID: 21502
		[ActionSection("Results")]
		[UIHint(10)]
		[Tooltip("The layer's name.")]
		public FsmString name;

		// Token: 0x040053FF RID: 21503
		[UIHint(10)]
		[Tooltip("The layer's name Hash")]
		public FsmInt nameHash;

		// Token: 0x04005400 RID: 21504
		[Tooltip("The layer's tag hash")]
		[UIHint(10)]
		public FsmInt tagHash;

		// Token: 0x04005401 RID: 21505
		[Tooltip("Is the state looping. All animations in the state must be looping")]
		[UIHint(10)]
		public FsmBool isStateLooping;

		// Token: 0x04005402 RID: 21506
		[UIHint(10)]
		[Tooltip("The Current duration of the state. In seconds, can vary when the State contains a Blend Tree ")]
		public FsmFloat length;

		// Token: 0x04005403 RID: 21507
		[UIHint(10)]
		[Tooltip("The integer part is the number of time a state has been looped. The fractional part is the % (0-1) of progress in the current loop")]
		public FsmFloat normalizedTime;

		// Token: 0x04005404 RID: 21508
		[Tooltip("The integer part is the number of time a state has been looped. This is extracted from the normalizedTime")]
		[UIHint(10)]
		public FsmInt loopCount;

		// Token: 0x04005405 RID: 21509
		[UIHint(10)]
		[Tooltip("The progress in the current loop. This is extracted from the normalizedTime")]
		public FsmFloat currentLoopProgress;

		// Token: 0x04005406 RID: 21510
		private PlayMakerAnimatorMoveProxy _animatorProxy;

		// Token: 0x04005407 RID: 21511
		private Animator _animator;
	}
}
