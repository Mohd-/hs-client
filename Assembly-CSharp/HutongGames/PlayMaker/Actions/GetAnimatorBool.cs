using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D4B RID: 3403
	[ActionCategory("Animator")]
	[Tooltip("Gets the value of a bool parameter")]
	public class GetAnimatorBool : FsmStateAction
	{
		// Token: 0x06006AFA RID: 27386 RVA: 0x001F7CC9 File Offset: 0x001F5EC9
		public override void Reset()
		{
			this.gameObject = null;
			this.parameter = null;
			this.result = null;
			this.everyFrame = false;
		}

		// Token: 0x06006AFB RID: 27387 RVA: 0x001F7CE8 File Offset: 0x001F5EE8
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
			this._paramID = Animator.StringToHash(this.parameter.Value);
			this.GetParameter();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006AFC RID: 27388 RVA: 0x001F7D9F File Offset: 0x001F5F9F
		public void OnAnimatorMoveEvent()
		{
			if (this._animatorProxy != null)
			{
				this.GetParameter();
			}
		}

		// Token: 0x06006AFD RID: 27389 RVA: 0x001F7DB8 File Offset: 0x001F5FB8
		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.GetParameter();
			}
		}

		// Token: 0x06006AFE RID: 27390 RVA: 0x001F7DD1 File Offset: 0x001F5FD1
		private void GetParameter()
		{
			if (this._animator != null)
			{
				this.result.Value = this._animator.GetBool(this._paramID);
			}
		}

		// Token: 0x06006AFF RID: 27391 RVA: 0x001F7E00 File Offset: 0x001F6000
		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent -= new Action(this.OnAnimatorMoveEvent);
			}
		}

		// Token: 0x0400535C RID: 21340
		[RequiredField]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		[CheckForComponent(typeof(Animator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400535D RID: 21341
		[Tooltip("The animator parameter")]
		public FsmString parameter;

		// Token: 0x0400535E RID: 21342
		[Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
		public bool everyFrame;

		// Token: 0x0400535F RID: 21343
		[RequiredField]
		[UIHint(10)]
		[Tooltip("The bool value of the animator parameter")]
		[ActionSection("Results")]
		public FsmBool result;

		// Token: 0x04005360 RID: 21344
		private PlayMakerAnimatorMoveProxy _animatorProxy;

		// Token: 0x04005361 RID: 21345
		private Animator _animator;

		// Token: 0x04005362 RID: 21346
		private int _paramID;
	}
}
