using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D65 RID: 3429
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1055")]
	[Tooltip("Returns the pivot weight and/or position. The pivot is the most stable point between the avatar's left and right foot.\n For a weight value of 0, the left foot is the most stable point For a value of 1, the right foot is the most stable point")]
	[ActionCategory("Animator")]
	public class GetAnimatorPivot : FsmStateAction
	{
		// Token: 0x06006B86 RID: 27526 RVA: 0x001F9DBD File Offset: 0x001F7FBD
		public override void Reset()
		{
			this.gameObject = null;
			this.pivotWeight = null;
			this.pivotPosition = null;
			this.everyFrame = false;
		}

		// Token: 0x06006B87 RID: 27527 RVA: 0x001F9DDC File Offset: 0x001F7FDC
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
			this.DoCheckPivot();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006B88 RID: 27528 RVA: 0x001F9E7D File Offset: 0x001F807D
		public void OnAnimatorMoveEvent()
		{
			if (this._animatorProxy != null)
			{
				this.DoCheckPivot();
			}
		}

		// Token: 0x06006B89 RID: 27529 RVA: 0x001F9E96 File Offset: 0x001F8096
		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.DoCheckPivot();
			}
		}

		// Token: 0x06006B8A RID: 27530 RVA: 0x001F9EB0 File Offset: 0x001F80B0
		private void DoCheckPivot()
		{
			if (this._animator == null)
			{
				return;
			}
			this.pivotWeight.Value = this._animator.pivotWeight;
			this.pivotPosition.Value = this._animator.pivotPosition;
		}

		// Token: 0x06006B8B RID: 27531 RVA: 0x001F9EFC File Offset: 0x001F80FC
		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent -= new Action(this.OnAnimatorMoveEvent);
			}
		}

		// Token: 0x04005408 RID: 21512
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005409 RID: 21513
		[Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
		public bool everyFrame;

		// Token: 0x0400540A RID: 21514
		[ActionSection("Results")]
		[UIHint(10)]
		[Tooltip("The pivot is the most stable point between the avatar's left and right foot.\n For a value of 0, the left foot is the most stable point For a value of 1, the right foot is the most stable point")]
		public FsmFloat pivotWeight;

		// Token: 0x0400540B RID: 21515
		[Tooltip("The pivot is the most stable point between the avatar's left and right foot.\n For a value of 0, the left foot is the most stable point For a value of 1, the right foot is the most stable point")]
		[UIHint(10)]
		public FsmVector3 pivotPosition;

		// Token: 0x0400540C RID: 21516
		private PlayMakerAnimatorMoveProxy _animatorProxy;

		// Token: 0x0400540D RID: 21517
		private Animator _animator;
	}
}
